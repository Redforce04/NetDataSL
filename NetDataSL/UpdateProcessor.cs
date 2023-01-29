// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         UpdateProcessor.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/29/2023 3:09 PM
//    Created Date:     01/29/2023 3:09 PM
// -----------------------------------------

using NetDataSL.API.Members;
using NetDataSL.API.Structs;
using NetDataSL.Enums;
using NetDataSL.Structs;

namespace NetDataSL;

public class UpdateProcessor
{
    public static UpdateProcessor? Singleton;
    public UpdateProcessor()
    {
        if(Singleton != null)
            return;
        Singleton = this;
        _dataSets = new Dictionary<ChartImplementationType, List<DataSet>>()
        {
            { ChartImplementationType.Cpu, new List<DataSet>() },
            { ChartImplementationType.Memory, new List<DataSet>() },
            { ChartImplementationType.Tps, new List<DataSet>() },
            { ChartImplementationType.LowFps, new List<DataSet>() },
            { ChartImplementationType.Players, new List<DataSet>() },
        };
        _lowFpsInstances = new Dictionary<int, int>();
    }

    private readonly Dictionary<ChartImplementationType, List<DataSet>> _dataSets = null!;
    private readonly Dictionary<int, int> _lowFpsInstances = null!;
    internal void SendUpdate()
    {
        List<DataSet>? dataSets = _getLowFpsInstancesDataSets();
        if (dataSets != null)
        {
            ChartIntegration.Singleton!._updateChartData(ChartImplementationType.LowFps, dataSets);
        }

        foreach (var x in _dataSets)
        {
            if(x.Value.Count != 0)
                ChartIntegration.Singleton!._updateChartData(x.Key, x.Value);
        }
        _clearDataSets();
    }

    private List<DataSet>? _getLowFpsInstancesDataSets()
    {
        List<DataSet> data = new List<DataSet>();
        foreach (var x in _lowFpsInstances)
        {
            Dimension? dimension =
                ChartIntegration.Singleton!.GetDimensionByChartTypeAndServer(ChartImplementationType.LowFps, x.Key);
            if (dimension == null)
            {
                Log.Error($"Dimension is null. Cannot process info. ChartType: {ChartImplementationType.LowFps}, Server: {x.Key}");
                continue;
            }
            data.Add(new DataSet(dimension, x.Value));
        }
        return data.Count == 0 ? null : data;
    }

    private void _clearDataSets()
    {
        foreach (List<DataSet> set in _dataSets.Values)
            set.Clear();
        _lowFpsInstances.Clear();
    }
    internal void ProcessUpdate(LowFps lowFps)
    {
        Log.Debug($"Processing Update: {lowFps.DateTime}");
        int server = lowFps.Server;
        if(!_lowFpsInstances.ContainsKey(server))
            _lowFpsInstances.Add(server, 0);
        _lowFpsInstances[server] = lowFps.InstanceNumber + 1;
    }

    internal void ProcessUpdate(LoggingInfo loggingInfo)
    {
        Log.Debug($"Processing Update: {loggingInfo.DateTime}");
        int server = loggingInfo.Server;
        _addUpdate(ChartImplementationType.Cpu, server, loggingInfo.CpuUsage, true);
        _addUpdate(ChartImplementationType.Memory, server, loggingInfo.MemoryUsage, true);
        _addUpdate(ChartImplementationType.Tps, server, loggingInfo.AverageFps, true);
        _addUpdate(ChartImplementationType.Players, server, loggingInfo.Players);
    }

    private void _addUpdate(ChartImplementationType type, int server, object value, bool isFloat = false)
    {
        Dimension? dimension =
            ChartIntegration.Singleton!.GetDimensionByChartTypeAndServer(type, server);
        if (dimension is null)
        {
            Log.Error(
                $"Dimension is null and should not be. AddUpdate cannot continue for this chart server combo. ChartType: {type}, Server: {server}");
            return;
        }

        _dataSets[type].Add(isFloat ? new DataSet(dimension, (float)value) : new DataSet(dimension, (int)value));
    }
}