// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         LowFpsChart.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 10:43 PM
// -----------------------------------------

using NetDataSL.API.Enums;
using NetDataSL.API.Members;


namespace NetDataSL.ApiImplementation;

public class LowTpsChart : Chart
{
    internal LowTpsChart(List<Dimension> dimensions)
    {
        Dimensions = dimensions;
        foreach(Dimension dimension in Dimensions)
            dimension.AssignChart(this);
        foreach(Variable variable in Variables)
            variable.AssignChart(this);
        foreach (CLabel label in CLabels)
            label.AssignChart(this);
    }


    private const string ChartInfo = "low_tps";

    public override string Type => "scpsl";

    public override string Id => $"{ChartInfo}";

    public override string Name => ChartInfo;

    public override string Title => $"{ChartInfo}";

    public override string Units => "low tps warnings";

    public override string Family => $"{ChartInfo}";

    public override string Context => ChartInfo;

    public override ChartType ChartType => ChartType.Line;

    public override int Priority => 1000;

    public override float UpdateEvery => 5f;

    public override bool Obsolete => false;

    public override bool Detail => false;

    public override bool StoreFirst => false;

    public override bool Hidden => false;
    public override string Module => "lowtps";

}
class LowTpsChartDimensions : Dimension
{
    public LowTpsChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    private int Server { get; }
    private string ServerName { get; }
    public override string Id => $"lowtps-{Server}";
    public override string Name => $"\"{ServerName}\" Low Tps Warnings";
    public override Algorithm Algorithm => Algorithm.Absolute;
    public override int Multiplier => 1;
    public override int Divisor => 1000;
    public override bool Obsolete => false;
    public override bool Hidden => false;
}