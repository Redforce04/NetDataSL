using NetDataSL.API;
using NetDataSL.ApiImplementation;

namespace NetDataSL;

public class ChartIntegration
{
    private readonly ChartIntegration? _singleton;
    internal ChartIntegration(List<KeyValuePair<int,string>> servers)
    {
        if(_singleton != null)
            return;
        _singleton = this;
        _servers = servers;
        _init();
    }

    private readonly List<KeyValuePair<int, string>> _servers = new List<KeyValuePair<int, string>>();
    private readonly List<Chart> _charts = new List<Chart>();
    private void _init()
    {
        _buildPlayerCharts();
        _buildTpsCharts();
        _buildMemoryCharts();
        _buildCpuCharts();
        _buildLowFpsCharts();
        _registerAllCharts();
    }
    private void _buildPlayerCharts()
    {
        foreach (KeyValuePair<int, string> server in _servers)
        {
            _charts.Add(new PlayersChart(server.Key, server.Value));
        }
    }
    private void _buildTpsCharts()
    {
        foreach (KeyValuePair<int, string> server in _servers)
        {
            _charts.Add(new TpsChart(server.Key, server.Value));
        }
    }
    
    private void _buildMemoryCharts()
    {
        foreach (KeyValuePair<int, string> server in _servers)
        {
            _charts.Add(new MemoryChart(server.Key, server.Value));
        }
    }
    private void _buildCpuCharts()
    {
        foreach (KeyValuePair<int, string> server in _servers)
        {
            _charts.Add(new CpuChart(server.Key, server.Value));
        }
    }
    
    private void _buildLowFpsCharts()
    {
        foreach (KeyValuePair<int, string> server in _servers)
        {
            _charts.Add(new LowFpsChart(server.Key, server.Value));
        }
    }

    private void _registerAllCharts()
    {
        foreach (var chart in _charts)
        {
            chart.InitChart();
        }
    }

}