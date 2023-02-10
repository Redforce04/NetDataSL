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
    private readonly ConcurrentDictionary<ChartImplementationType, ConcurrentBag<DataSet>> _dataSets = null!;

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
        this._dataSets = new ConcurrentDictionary<ChartImplementationType, ConcurrentBag<DataSet>>();
        this._dataSets.GetOrAdd(ChartImplementationType.Cpu, new ConcurrentBag<DataSet>());
        this._dataSets.GetOrAdd(ChartImplementationType.Memory, new ConcurrentBag<DataSet>());
        this._dataSets.GetOrAdd(ChartImplementationType.Tps, new ConcurrentBag<DataSet>());
        this._dataSets.GetOrAdd(ChartImplementationType.LowTps, new ConcurrentBag<DataSet>());
        this._dataSets.GetOrAdd(ChartImplementationType.Players, new ConcurrentBag<DataSet>());
    }

    /// <summary>
    /// Sends an update.
    /// </summary>
    internal void SendUpdate()
    {
        foreach (var x in this._dataSets)
        {
            if (x.Value.Count != 0)
            {
                ChartIntegration.Singleton!.UpdateChartData(x.Key, x.Value);
                x.Value.Clear();
            }
        }
    }

    /// <summary>
    /// Process updates for netdata packets.
    /// </summary>
    /// <param name="packet">the packet to update.</param>
    internal void ProcessUpdate(NetDataPacket packet)
    {
        Log.Debug($"Processing Update");
        this.AddUpdate(ChartImplementationType.Cpu, packet.Port, packet.CpuUsage, true);
        this.AddUpdate(ChartImplementationType.Memory, packet.Port, (float)packet.MemoryUsage, true);
        this.AddUpdate(ChartImplementationType.Tps, packet.Port, packet.AverageTps, true);
        this.AddUpdate(ChartImplementationType.Players, packet.Port, packet.Players, false);
        this.AddUpdate(ChartImplementationType.LowTps, packet.Port, packet.LowTpsWarnCount, false);
    }

    private void AddUpdate(ChartImplementationType type, int server, object value, bool isFloat = false)
    {
        Dimension? dimension =
            ChartIntegration.Singleton!.GetDimensionByChartTypeAndServer(type, server);
        if (dimension is null)
        {
            Log.Error(
                $"Dimension is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        this._dataSets[type].Add(isFloat ? new DataSet(dimension, (float)value) : new DataSet(dimension, (int)value));
    }
}