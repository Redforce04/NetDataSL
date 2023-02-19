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

using System.Xml.Linq;
using Microsoft.AspNetCore.Connections.Features;

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
    private ConcurrentDictionary<int, ConcurrentBag<Data>> _serverStats = null!;
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

        this._dataSets = new ConcurrentDictionary<ChartImplementationType, ConcurrentDictionary<int, Data>>();
        this._dataSets.GetOrAdd(ChartImplementationType.Cpu, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.Memory, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.Tps, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.LowTps, new ConcurrentDictionary<int, Data>());
        this._dataSets.GetOrAdd(ChartImplementationType.Players, new ConcurrentDictionary<int, Data>());

        this._serverStats = new ConcurrentDictionary<int, ConcurrentBag<Data>>();
        foreach (var server in Config.Singleton!.ServerInstances)
        {
            this._serverStats.GetOrAdd(server.Port, new ConcurrentBag<Data>());
        }
    }

    /// <summary>
    /// Sends an update.
    /// </summary>
    internal void SendUpdate()
    {
        List<int> servers = new List<int>();
        foreach (var x in Config.Singleton!.ServerInstances)
        {
            servers.Add(x.Port);
        }

        foreach (var chartDataSet in this._dataSets)
        {
            List<int> server = servers;
            List<Data> data = chartDataSet.Value.Values.ToList();
            foreach (var dataset in chartDataSet.Value)
            {
                if (server.Contains(dataset.Key))
                {
                    server.Remove(dataset.Key);
                }
            }

            foreach (var emptyServer in server)
            {
                var chart = ChartIntegration.Singleton!.GetChartByChartType(chartDataSet.Key);
                if (chart != null)
                {
                    var dimension =
                        ChartIntegration.Singleton!.GetDimensionByChartTypeAndServer(chartDataSet.Key, emptyServer);
                    if (dimension != null)
                    {
                        data.Add(new Data(chart, new List<DataSet>(), (uint)(this._lastUpdate.ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())));
                    }
                    else
                    {
                        Log.Error($"Dimension is null when sending empty updates for servers.");
                    }
                }
                else
                {
                    Log.Error($"Chart is null when sending empty updates for servers.");
                }
            }

            chartDataSet.Value.Clear();
        }

        foreach (ServerConfig server in Config.Singleton.ServerInstances)
        {
            if (!this._serverStats.ContainsKey(server.Port) || this._serverStats[server.Port].Count == 0)
            {
                Chart? chart = ChartIntegration.Singleton!.GetChartByChartType(ChartImplementationType.Server, server.Port);
                if (chart is null)
                {
                    Log.Error($"Could not send blank server stat update because the chart was null. Port: {server.Port}");
                    return;
                }

                List<DataSet> datasets = new List<DataSet>();
                datasets.Add(new DataSet($"stats.{server.Port}.cpu", (float)0f));
                datasets.Add(new DataSet($"stats.{server.Port}.memory", (float)0f));
                datasets.Add(new DataSet($"stats.{server.Port}.players", (int)0));
                datasets.Add(new DataSet($"stats.{server.Port}.tps", (float)0f));
                datasets.Add(new DataSet($"stats.{server.Port}.lowtps", (int)0));
                var timeSinceLastUpdate = (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (uint)UpdateProcessor.Singleton!._lastUpdate.ToUnixTimeMilliseconds();
                var data = new Data(chart, datasets, timeSinceLastUpdate);
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
        datasets.Add(new DataSet($"stats.{packet.Port}.cpu", (float)packet.CpuUsage));
        datasets.Add(new DataSet($"stats.{packet.Port}.memory", (float)packet.MemoryUsage));
        datasets.Add(new DataSet($"stats.{packet.Port}.players", (int)packet.Players));
        datasets.Add(new DataSet($"stats.{packet.Port}.tps", (float)packet.AverageTps));
        datasets.Add(new DataSet($"stats.{packet.Port}.lowtps", (int)packet.LowTpsWarnCount));
        var timeSinceLastUpdate = (uint)packet.Epoch - (uint)UpdateProcessor.Singleton!._lastUpdate.ToUnixTimeMilliseconds();
        var data = new Data(chart, datasets, timeSinceLastUpdate, false);
        this._serverStats[packet.Port].Add(data);
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
            ChartIntegration.Singleton!.GetDimensionByChartTypeAndServer(type, server);
        if (dimension is null)
        {
            Log.Error(
                $"Dimension is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        this._dataSets[type].TryAdd(server, new Data(chart, new List<DataSet> { (isFloat ? new DataSet(dimension, (float)value) : new DataSet(dimension, (int)value)) }, (uint)timeSinceLastUpdate, false));
    }
}