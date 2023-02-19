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
    private readonly Dictionary<ChartImplementationType, Dictionary<int, Data>> _dataSets = null!;
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
        this._dataSets = new Dictionary<ChartImplementationType, Dictionary<int, Data>>()
        {
            { ChartImplementationType.Cpu, new Dictionary<int, Data>() },
            { ChartImplementationType.Memory, new Dictionary<int, Data>() },
            { ChartImplementationType.Tps, new Dictionary<int, Data>() },
            { ChartImplementationType.LowTps, new Dictionary<int, Data>() },
            { ChartImplementationType.Players, new Dictionary<int, Data>() },
        };
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

            if (chartDataSet.Value.Count != 0)
            {
                foreach (var dataSet in data)
                {
                    dataSet.Call();
                }
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
        this.AddUpdate(ChartImplementationType.LowTps, packet.Port, packet.AverageTps, false, (uint)packet.Epoch);
    }

    private void AddUpdate(ChartImplementationType type, int server, object value, bool isFloat = false, uint timeSinceLastUpdate = 5000)
    {
        Chart? chart = ChartIntegration.Singleton!.GetChartByChartType(type);
        Dimension? dimension =
            ChartIntegration.Singleton!.GetDimensionByChartTypeAndServer(type, server);
        if (dimension is null)
        {
            Log.Error(
                $"Dimension is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        if (chart is null)
        {
            Log.Error($"Chart is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        this._dataSets[type].Add(server, new Data(chart, new List<DataSet> { (isFloat ? new DataSet(dimension, (float)value) : new DataSet(dimension, (int)value)) }, (uint)timeSinceLastUpdate, false));
    }
}