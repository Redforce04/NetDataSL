// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ChartIntegration.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 9:52 PM
// -----------------------------------------

namespace NetDataSL;

using System.Collections.Concurrent;

// ReSharper disable three RedundantNameQualifier
using NetDataSL.API.Members;
using NetDataSL.API.Structs;
using NetDataSL.ApiImplementation;
using NetDataSL.Enums;

/// <summary>
/// Used to implement the NetData API for the plugin.
/// </summary>
public class ChartIntegration
{
#pragma warning disable SA1401
    /// <summary>
    /// The main instance of the ChartIntegration.
    /// </summary>
    internal static ChartIntegration? Singleton;
    private readonly List<KeyValuePair<int, string>> _servers = new();
    private readonly Dictionary<ChartImplementationType, Chart> _charts = new();
#pragma warning restore SA1401

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartIntegration"/> class.
    /// </summary>
    /// <param name="servers">The servers to initialize.</param>
    internal ChartIntegration(List<KeyValuePair<int, string>> servers)
    {
        if (Singleton != null)
        {
            return;
        }

        Singleton = this;
        this._servers = servers;
        this.Init();
    }

    /// <summary>
    /// Gets a chart via the type of chart it is.
    /// </summary>
    /// <param name="implementationType">The type of chart.</param>
    /// <returns>The instance of the chart.</returns>
    public Chart? GetChartByChartType(ChartImplementationType implementationType)
    {
        if (this._charts.ContainsKey(implementationType))
        {
            return this._charts[implementationType];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets a dimension from a chart, based off of what server port it is and what type of chart it is.
    /// </summary>
    /// <param name="implementationType">The chart type.</param>
    /// <param name="server">The server port.</param>
    /// <returns>The dimension. Null if no dimension was found.</returns>
    public Dimension? GetDimensionByChartTypeAndServer(ChartImplementationType implementationType, int server)
    {
        Chart? chart = this.GetChartByChartType(implementationType);
        if (chart is null)
        {
            Log.Error($"Chart is null when getting dimension. {implementationType} {server}");
            return null;
        }

        string dimensionId = $"{implementationType.ToString().ToLower().Replace(" ", "_")}.{server}";
        return chart.Dimensions.FirstOrDefault(x => x.Id == dimensionId);
    }

    /// <summary>
    /// Updates values for a chart.
    /// </summary>
    /// <param name="implementationType">The type of chart to update.</param>
    /// <param name="dataSets">A list of all data to update.</param>
    public void UpdateChartData(ChartImplementationType implementationType, ConcurrentBag<DataSet> dataSets)
    {
        Chart? chart = this.GetChartByChartType(implementationType);
        if (chart == null)
        {
            Log.Error($"Chart is null but should not be. Cannot Update Chart Data. Chart: {implementationType}");
            return;
        }

        Log.Debug($"Sending Data for chart {chart} ({dataSets.Count})");
        var unused = new Data(chart, dataSets.ToList());
    }

    private void Init()
    {
        this.BuildPlayerCharts();
        this.BuildTpsCharts();
        this.BuildMemoryCharts();
        this.BuildCpuCharts();
        this.BuildLowTpsCharts();
        Chart.RegisterAllCharts();
    }

    private void BuildPlayerCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in this._servers)
        {
            dimensions.Add(new PlayersChartDimensions(server.Key, server.Value));
        }

        this._charts.Add(ChartImplementationType.Players, new PlayersChart(dimensions));
    }

    private void BuildTpsCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in this._servers)
        {
            dimensions.Add(new TpsChartDimensions(server.Key, server.Value));
        }

        this._charts.Add(ChartImplementationType.Tps, new TpsChart(dimensions));
    }

    private void BuildMemoryCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in this._servers)
        {
            dimensions.Add(new MemoryChartDimensions(server.Key, server.Value));
        }

        this._charts.Add(ChartImplementationType.Memory, new MemoryChart(dimensions));
    }

    private void BuildCpuCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in this._servers)
        {
            dimensions.Add(new CpuChartDimensions(server.Key, server.Value));
        }

        this._charts.Add(ChartImplementationType.Cpu, new CpuChart(dimensions));
    }

    private void BuildLowTpsCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in this._servers)
        {
            dimensions.Add(new LowTpsChartDimensions(server.Key, server.Value));
        }

        this._charts.Add(ChartImplementationType.LowTps, new LowTpsChart(dimensions));
    }
}