// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         MemoryChart.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 10:43 PM
// -----------------------------------------

namespace NetDataSL.ApiImplementation;

// ReSharper disable twice RedundantNameQualifier
using NetDataSL.API.Enums;
using NetDataSL.API.Members;
#pragma warning disable
public class MemoryChart : Chart
{
    internal MemoryChart(List<Dimension> dimensions)
    {
        Dimensions = dimensions;
        foreach (Dimension dimension in Dimensions)
            dimension.AssignChart(this);
        foreach (Variable variable in Variables)
            variable.AssignChart(this);
        foreach (CLabel label in CLabels)
            label.AssignChart(this);
    }


    private const string ChartInfo = "Memory";

    public override string Type => "scpsl";

    public override string Id => $"memory";

    public override string Name => ChartInfo;

    public override string Title => $"{ChartInfo}";

    public override string Units => "mb of memory allocated";

    public override string Family => $"Memory Usage";

    public override string Context => "scpsl.memory_usage";

    public override ChartType ChartType => ChartType.Line;

    public override int Priority => 1000;

    public override float UpdateEvery => 5f;

    public override bool Obsolete => false;

    public override bool Detail => false;

    public override bool StoreFirst => false;

    public override bool Hidden => false;

    public override string Module => "memory";
}

class MemoryChartDimensions : Dimension
{
    public MemoryChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    private int Server { get; }
    private string ServerName { get; }
    public override string Id => $"memory.{Server}";
    public override string Name => $"{ServerName}";
    public override Algorithm Algorithm => Algorithm.Absolute;
    public override int Multiplier => 1;
    public override int Divisor => 1000;
    public override bool Obsolete => false;
    public override bool Hidden => false;
}
#pragma warning restore