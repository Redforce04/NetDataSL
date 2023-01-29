// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ChartIntegration.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:52 PM
// -----------------------------------------

using NetDataSL.API;
using NetDataSL.API.Members;
using NetDataSL.ApiImplementation;

namespace NetDataSL;

public class ChartIntegration
{
    private readonly ChartIntegration? _singleton;

    internal ChartIntegration(List<KeyValuePair<int, string>> servers)
    {
        if (_singleton != null)
            return;
        _singleton = this;
        _servers = servers;
        _init();
    }

    private readonly List<KeyValuePair<int, string>> _servers = new();
    private readonly List<Chart> _charts = new();

    private void _init()
    {
        _buildPlayerCharts();
        _buildTpsCharts();
        _buildMemoryCharts();
        _buildCpuCharts();
        _buildLowFpsCharts();
        Chart.RegisterAllCharts();
    }

    private void _buildPlayerCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new PlayersChartDimensions(server.Key, server.Value));
        _charts.Add(new PlayersChart(dimensions));    }

    private void _buildTpsCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new TpsChartDimensions(server.Key, server.Value));
        _charts.Add(new TpsChart(dimensions));    }

    private void _buildMemoryCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new MemoryChartDimensions(server.Key, server.Value));
        _charts.Add(new MemoryChart(dimensions));
    }

    private void _buildCpuCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new CpuChartDimensions(server.Key, server.Value));
        _charts.Add(new CpuChart(dimensions));    }

    private void _buildLowFpsCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new LowFpsChartDimensions(server.Key, server.Value));
        _charts.Add(new LowFpsChart(dimensions));    }
}