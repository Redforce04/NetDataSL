// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ChartIntegration.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:52 PM
// -----------------------------------------

using NetDataSL.API.Members;
using NetDataSL.API.Structs;
using NetDataSL.ApiImplementation;
using NetDataSL.Enums;

namespace NetDataSL;

public class ChartIntegration
{
    internal static ChartIntegration? Singleton;

    internal ChartIntegration(List<KeyValuePair<int, string>> servers)
    {
        if (Singleton != null)
            return;
        Singleton = this;
        _servers = servers;
        _init();
    }

    private readonly List<KeyValuePair<int, string>> _servers = new();
    private readonly Dictionary<ChartImplementationType, Chart> _charts = new();

    private void _init()
    {
        _buildPlayerCharts();
        _buildTpsCharts();
        _buildMemoryCharts();
        _buildCpuCharts();
        _buildLowFpsCharts();
        Chart.RegisterAllCharts();
    }

    public Chart? GetChartByChartType(ChartImplementationType implementationType)
    {
        if (_charts.ContainsKey(implementationType))
            return _charts[implementationType];
        else
            return null;
    }

    public Dimension? GetDimensionByChartTypeAndServer(ChartImplementationType implementationType, int server)
    {
        Chart? chart = GetChartByChartType(implementationType);
        if (chart is null)
            return null;
        string dimensionId = $"{implementationType.ToString().ToLower()}-{server}";
        return chart.Dimensions.FirstOrDefault(x => x.Id == dimensionId);
    }
    public void _updateChartData(ChartImplementationType implementationType, List<DataSet> dataSets)
    {
        Chart? chart = GetChartByChartType(implementationType);
        if (chart == null)
        {
            Log.Error($"Chart is null but should not be. Cannot Update Chart Data. Chart: {implementationType}");
            return;
        }
        var unused = new Data(chart, dataSets);
    }

    private void _buildPlayerCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new PlayersChartDimensions(server.Key, server.Value));
        _charts.Add(ChartImplementationType.Players, new PlayersChart(dimensions));    
    }

    private void _buildTpsCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new TpsChartDimensions(server.Key, server.Value));
        _charts.Add(ChartImplementationType.Tps, new TpsChart(dimensions));    
    }

    private void _buildMemoryCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new MemoryChartDimensions(server.Key, server.Value));
        _charts.Add(ChartImplementationType.Memory, new MemoryChart(dimensions));
    }

    private void _buildCpuCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new CpuChartDimensions(server.Key, server.Value));
        _charts.Add(ChartImplementationType.Cpu, new CpuChart(dimensions));    
    }

    private void _buildLowFpsCharts()
    {
        List<Dimension> dimensions = new List<Dimension>();
        foreach (var server in _servers) 
            dimensions.Add(new LowFpsChartDimensions(server.Key, server.Value));
        _charts.Add(ChartImplementationType.LowFps, new LowFpsChart(dimensions));    
    }
}