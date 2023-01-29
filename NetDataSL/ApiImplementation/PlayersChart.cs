// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         PlayersChart.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:57 PM
// -----------------------------------------

using NetDataSL.API;
using NetDataSL.API.Enums;
using NetDataSL.API.Members;

namespace NetDataSL.ApiImplementation;

public class PlayersChart : Chart
{
    internal PlayersChart(List<Dimension> dimensions)
    {        
        Dimensions = dimensions;
        foreach(Dimension dimension in Dimensions)
            dimension.AssignChart(this);
        foreach(Variable variable in Variables)
            variable.AssignChart(this);
        foreach (CLabel label in CLabels)
            label.AssignChart(this);
    }

    private const string ChartInfo = "players";

    public override string Type => "scpsl";

    public override string Id => $"{ChartInfo}";

    public override string Name => ChartInfo;

    public override string Title => $"{ChartInfo}";

    public override string Units => ChartInfo;

    public override string Family => $"{ChartInfo}";

    public override string Context => ChartInfo;

    public override ChartType ChartType => ChartType.Line;

    public override int Priority => 1000;

    public override float UpdateEvery => 5f;

    public override bool Obsolete => false;

    public override bool Detail => false;

    public override bool StoreFirst => false;

    public override bool Hidden => false;
    
    public override string Module => "players";

}
class PlayersChartDimensions : Dimension
{
    public PlayersChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    private int Server { get; }
    private string ServerName { get; }
    public override string Id => $"players-{Server}";
    public override string Name => $"\"{ServerName}\" Players";
    public override Algorithm Algorithm => Algorithm.Absolute;
    public override int Multiplier => 1;
    public override int Divisor => 1;
    public override bool Obsolete => false;
    public override bool Hidden => false;
}