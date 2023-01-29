// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Data.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 5:47 PM
//    Created Date:     01/28/2023 5:47 PM
// -----------------------------------------

using NetDataSL.API.Members;

namespace NetDataSL.API.Structs;

public struct Data
{
    public Data(Chart chart, List<DataSet> dataSet, uint microseconds = 5000)
    {
        ChartTypeId = chart.TypeId;
        Microseconds = microseconds;
        DataSet = dataSet;
        _cancel = false;
        _sent = false;
        Call();
    }

    private void Call()
    {
        Begin();
        Set();
        if(!_cancel)
            End();
    }
    private void Begin()
    {
        var context = $"BEGIN {ChartTypeId} {Microseconds}";
        Log.Line(context);
    }
    private void Set()
    {
        foreach (DataSet data in DataSet)
        {
            var context = $"SET {data.DimensionId} = {(data.Value.HasValue ? data.Value : "")}";
            _sent = true;
            Log.Line(context);
        }
    }

    public void Flush()
    {
        if (!_sent)
        {
            _cancel = true;
            var context = $"FLUSH";
            Log.Line(context);
        }
    }
    private void End()
    {
        var context = "END";
        Log.Line(context);
    }

    private bool _sent;
    private bool _cancel;
    private string ChartTypeId { get; }
    private uint Microseconds { get; }
    private List<DataSet> DataSet { get; }
}