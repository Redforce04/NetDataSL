// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         UpdateProcessor.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/29/2023 3:09 PM
// -----------------------------------------

namespace NetDataSL;

using System.Collections.Concurrent;

// ReSharper disable three RedundantNameQualifier
using NetDataSL.API.Members;
using NetDataSL.API.Structs;
using NetDataSL.Enums;
using NetDataSL.StructsAndClasses;

/// <summary>
/// Process Server Updates.
/// </summary>
public class UpdateProcessor
{
    /// <summary>
    /// Instance of the Update Processor to use.
    /// </summary>
#pragma warning disable SA1401
    internal static UpdateProcessor? Singleton;
#pragma warning restore SA1401
    private ConcurrentDictionary<ChartImplementationType, ConcurrentDictionary<int, Data>> _dataSets = null!;
    private ConcurrentDictionary<int, ConcurrentQueue<Data>> _serverStats = null!;
    private DateTimeOffset _lastUpdate;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProcessor"/> class.
    /// </summary>
    public UpdateProcessor()
    {
        if (Singleton != null)
        {
            return;
        }

        Singleton = this;
        this._lastUpdate = DateTimeOffset.UtcNow;
        this._dataSets = new ConcurrentDictionary<ChartImplementationType, ConcurrentDictionary<int, Data>>();
        this._dataSets.GetOrAdd(ChartImplementationType.Cpu, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.Memory, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.Tps, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.LowTps, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.Players, new ConcurrentDictionary<int, Data>());

        this._serverStats = new ConcurrentDictionary<int, ConcurrentQueue<Data>>();
        foreach (var server in Config.Singleton!.ServerInstances)
        {
            this._serverStats.GetOrAdd(server.Port, new ConcurrentQueue<Data>());
        }
    }

    /// <summary>
    /// Sends an update.
    /// </summary>
    internal void SendUpdate()
    {
        uint timeSinceLastUpdate = (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (uint)this._lastUpdate.ToUnixTimeMilliseconds();
        Dictionary<ChartImplementationType, List<int>> servers = new()
        {
            { ChartImplementationType.Cpu, new List<int>() },
            { ChartImplementationType.Memory, new List<int>() },
            { ChartImplementationType.Players, new List<int>() },
            { ChartImplementationType.Tps, new List<int>() },
            { ChartImplementationType.LowTps, new List<int>() },
        };
        foreach (var server in Config.Singleton!.ServerInstances)
        {
            servers[ChartImplementationType.Cpu].Add(server.Port);
            servers[ChartImplementationType.Memory].Add(server.Port);
            servers[ChartImplementationType.Players].Add(server.Port);
            servers[ChartImplementationType.Tps].Add(server.Port);
            servers[ChartImplementationType.LowTps].Add(server.Port);

            if (!this._serverStats.ContainsKey(server.Port) || this._serverStats[server.Port].Count == 0)
            {
                Chart? chart = ChartIntegration.Singleton!.GetChartByChartType(ChartImplementationType.Server, server.Port);
                if (chart is null)
                {
                    Log.Error($"Could not send blank server stat update because the chart was null. Port: {server.Port}");
                    return;
                }

                List<DataSet> datasets = new List<DataSet>();
                datasets.Add(new DataSet($"stats.{server.Port}.cpu", 0f));
                datasets.Add(new DataSet($"stats.{server.Port}.memory", 0f));
                datasets.Add(new DataSet($"stats.{server.Port}.players", 0));
                datasets.Add(new DataSet($"stats.{server.Port}.tps", 0f));
                datasets.Add(new DataSet($"stats.{server.Port}.lowtps", 0));
                var unused2 = new Data(chart, datasets, timeSinceLastUpdate);
            }
            else
            {
                foreach (var data in this._serverStats[server.Port])
                {
                    data.Call();
                }

                this._serverStats[server.Port].Clear();
            }
        }

        foreach (var chartDataSet in this._dataSets)
        {
            foreach (var dataset in chartDataSet.Value)
            {
                if (servers[chartDataSet.Key].Contains(dataset.Key))
                {
                    servers[chartDataSet.Key].Remove(dataset.Key);
                }

                dataset.Value.Call();
            }

            chartDataSet.Value.Clear();
        }

        foreach (var chartType in servers)
        {
            var chart = ChartIntegration.Singleton!.GetChartByChartType(chartType.Key);
            if (chart != null)
            {
                foreach (var emptyServer in chartType.Value)
                {
                    var dimension =
                        ChartIntegration.Singleton.GetDimensionByChartTypeAndServer(chartType.Key, emptyServer);
                    if (dimension != null)
                    {
                        var unused = new Data(chart, new List<DataSet>() { new DataSet(dimension, 0) }, timeSinceLastUpdate);
                    }
                    else
                    {
                        Log.Error($"Dimension is null when sending empty updates for servers.");
                    }
                }
            }
            else
            {
                Log.Error($"Chart is null when sending empty updates for servers.");
            }
        }

        this._lastUpdate = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Process updates for netdata packets.
    /// </summary>
    /// <param name="packet">the packet to update.</param>
    internal void ProcessUpdate(NetDataPacket packet)
    {
        this.AddUpdate(ChartImplementationType.Cpu, packet.Port, packet.CpuUsage, true, (uint)packet.Epoch);
        this.AddUpdate(ChartImplementationType.Memory, packet.Port, (float)packet.MemoryUsage, true, (uint)packet.Epoch);
        this.AddUpdate(ChartImplementationType.Tps, packet.Port, packet.AverageTps, true, (uint)packet.Epoch);
        this.AddUpdate(ChartImplementationType.Players, packet.Port, packet.Players, false, (uint)packet.Epoch);
        this.AddUpdate(ChartImplementationType.LowTps, packet.Port, packet.LowTpsWarnCount, false, (uint)packet.Epoch);
        this.AddServerStatUpdate(packet);
    }

    private void AddServerStatUpdate(NetDataPacket packet)
    {
        Chart? chart = ChartIntegration.Singleton!.GetChartByChartType(ChartImplementationType.Server, packet.Port);
        if (chart is null)
        {
            Log.Error($"Could not process server stat update because the chart was null. Port: {packet.Port}");
            return;
        }

        if (!this._serverStats.ContainsKey(packet.Port))
        {
            Log.Error($"Server stats does not contain an instance of this server. Port: {packet.Port}");
        }

        List<DataSet> datasets = new List<DataSet>();
        datasets.Add(new DataSet($"stats.{packet.Port}.cpu", packet.CpuUsage));
        datasets.Add(new DataSet($"stats.{packet.Port}.memory", packet.MemoryUsage));
        datasets.Add(new DataSet($"stats.{packet.Port}.players", packet.Players));
        datasets.Add(new DataSet($"stats.{packet.Port}.tps", packet.AverageTps));
        datasets.Add(new DataSet($"stats.{packet.Port}.lowtps", packet.LowTpsWarnCount));
        var timeSinceLastUpdate = (uint)packet.Epoch - (uint)UpdateProcessor.Singleton!._lastUpdate.ToUnixTimeMilliseconds();
        var data = new Data(chart, datasets, timeSinceLastUpdate, false);
        this._serverStats[packet.Port].Enqueue(data);
    }

    private void AddUpdate(ChartImplementationType type, int server, object value, bool isFloat = false, uint timeSinceLastUpdate = 5000)
    {
        Chart? chart = ChartIntegration.Singleton!.GetChartByChartType(type);
        if (chart is null)
        {
            Log.Error($"Chart is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        Dimension? dimension =
            ChartIntegration.Singleton.GetDimensionByChartTypeAndServer(type, server);
        if (dimension is null)
        {
            Log.Error(
                $"Dimension is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        this._dataSets[type].TryAdd(server, new Data(chart, new List<DataSet> { (isFloat ? new DataSet(dimension, (float)value) : new DataSet(dimension, (int)value)) }, timeSinceLastUpdate, false));
    }
}