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

using NetDataSL.API.Enums;
using NetDataSL.API.Members;


namespace NetDataSL.ApiImplementation;

public class CpuChart : Chart
{
    
    // string type, string id, string title, string name = "",
    // string module = "stats", string units = "percentage", string family = "", 
    // string context = "", ChartType chartType = ChartType.Line, int priority = 1000, 
    // float updateEvery = 5f, ChartOptions options = ChartOptions.None
    
    internal CpuChart(List<Dimension> dimensions)
    {
        Dimensions = dimensions;
        foreach(Dimension dimension in Dimensions)
            dimension.AssignChart(this);
        foreach(Variable variable in Variables)
            variable.AssignChart(this);
        foreach (CLabel label in CLabels)
            label.AssignChart(this);
    }
    


    private const string ChartInfo = "Cpu";

    public override string Type => "scpsl";

    public override string Id => $"{ChartInfo}";

    public override string Name => ChartInfo;

    public override string Title => $"{ChartInfo}";

    public override string Units => "percent cpu used";

    public override string Family => $"{ChartInfo}";

    public override string Context => ChartInfo;

    public override ChartType ChartType => ChartType.Line;

    public override int Priority => 1000;

    public override float UpdateEvery => 5f;

    public override ChartOptions Options => ChartOptions.None;

    public override string Module => "cpu";
}

class CpuChartDimensions : Dimension
{
    public CpuChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    private int Server { get; }
    private string ServerName { get; }
    public override string Id => $"cpu-{Server}";
    public override string Name => $"\"{ServerName}\" Cpu Usage";
    public override Algorithm Algorithm => Algorithm.Absolute;
    public override int Multiplier => 1;
    public override int Divisor => 1000;
    public override bool Obsolete => false;
    public override bool Hidden => false;
}