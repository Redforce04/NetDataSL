using NetDataSL.API;

namespace NetDataSL.ApiImplementation;

public class TpsChart : Chart
{
    internal TpsChart(int server, string serverName)
    {
        _server = server;
        _serverName = serverName;
    }

    private readonly int _server;
    private readonly string _serverName;
    private const string ChartInfo = "tps";
    protected override string Type => "scpsl";
    protected override string Id => $"{ChartInfo}-{_server}";
    protected override string Name => ChartInfo;
    protected override string Title => _serverName + $" {ChartInfo}";
    protected override string Units => "average ticks per second";
    protected override string Family => $"{_serverName}";
    protected override string Context => ChartInfo;
    protected override ChartType ChartType => ChartType.Line;
    protected override int Priority => 1000;
    protected override float UpdateEvery => 5f;
    protected override bool Obsolete => false;
    protected override bool Detail => false;
    protected override bool StoreFirst => false;
    protected override bool Hidden => false;
}