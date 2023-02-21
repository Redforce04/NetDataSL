// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         CpuChart.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 10:43 PM
// -----------------------------------------

namespace NetDataSL.ApiImplementation;

using NetDataSL.API.Enums;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.API.Members;

/// <inheritdoc />
public class CpuChart : Chart
{
    // string type, string id, string title, string name = "",
    // string module = "stats", string units = "percentage", string family = "";
    // string context = "", ChartType chartType = ChartType.Line, int priority = 1000;
    // float updateEvery = 5f, ChartOptions options = ChartOptions.None
    private const string ChartInfo = "Cpu";

    /// <inheritdoc/>
    public override string Type => "Scp Secret Laboratory";

    /// <inheritdoc/>
    public override string Id => $"cpu";

    /// <inheritdoc/>
    public override string Name => ChartInfo;

    /// <inheritdoc/>
    public override string Title => $"{ChartInfo} Usage";

    /// <inheritdoc/>
    public override string Units => "percent of cpu cores used";

    /// <inheritdoc/>
    public override string Family => $"Cpu Usage";

    /// <inheritdoc/>
    public override string Context => "scpsl.cpu";

    /// <inheritdoc/>
    public override ChartType ChartType => ChartType.Line;

    /// <inheritdoc/>
    public override int Priority => 1000;

    /// <inheritdoc/>
    public override float UpdateEvery => 5f;

    /// <inheritdoc/>
    public override ChartOptions Options => ChartOptions.None;

    /// <inheritdoc/>
    public override string Module => "Cpu";

    /// <summary>
    /// Initializes a new instance of the <see cref="CpuChart"/> class.
    /// </summary>
    /// <param name="dimensions">The dimensions for this chart.</param>
#pragma warning disable SA1201
    internal CpuChart(List<Dimension> dimensions)
#pragma warning restore SA1201
    {
        this.Dimensions = dimensions;
        foreach (Dimension dimension in this.Dimensions)
        {
            dimension.AssignChart(this);
        }

        foreach (Variable variable in this.Variables)
        {
            variable.AssignChart(this);
        }

        foreach (CLabel label in this.CLabels)
        {
            label.AssignChart(this);
        }
    }
}

/// <inheritdoc />
#pragma warning disable SA1402
internal class CpuChartDimensions : Dimension
#pragma warning restore SA1402
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CpuChartDimensions"/> class.
    /// </summary>
    /// <param name="server">The server port.</param>
    /// <param name="serverName">The server name.</param>
    public CpuChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    /// <inheritdoc/>
    public override string Id => $"cpu.{this.Server}";

    /// <inheritdoc/>
    public override string Name => $"{this.ServerName}";

    /// <inheritdoc/>
    public override Algorithm Algorithm => Algorithm.Absolute;

    /// <inheritdoc/>
    public override int Multiplier => 1;

    /// <inheritdoc/>
    public override int Divisor => 1000;

    /// <inheritdoc/>
    public override bool Obsolete => false;

    /// <inheritdoc/>
    public override bool Hidden => false;

    private int Server { get; }

    private string ServerName { get; }
}
