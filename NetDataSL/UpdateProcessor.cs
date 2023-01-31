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
            { ChartImplementationType.LowTps, new List<DataSet>() },
            { ChartImplementationType.Players, new List<DataSet>() },
        };
    }

    private readonly Dictionary<ChartImplementationType, List<DataSet>> _dataSets = null!;
    internal void SendUpdate()
    {
        foreach (var x in _dataSets)
        {
            if(x.Value.Count != 0)
                ChartIntegration.Singleton!._updateChartData(x.Key, x.Value);
        }
    }


    internal void ProcessUpdate(NetDataPacket packet)
    {
        _addUpdate(ChartImplementationType.Cpu, packet.Port, packet.CpuUsage, true);
        _addUpdate(ChartImplementationType.Memory, packet.Port, (float) packet.MemoryUsage, true);
        _addUpdate(ChartImplementationType.Tps, packet.Port, packet.AverageTps, true);
        _addUpdate(ChartImplementationType.Players, packet.Port, packet.Players);
        _addUpdate(ChartImplementationType.LowTps, packet.Port, packet.AverageTps);
        
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