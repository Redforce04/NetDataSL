// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Data.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 6:10 PM
// -----------------------------------------

using System.Diagnostics;
using NetDataSL.API.Members;

namespace NetDataSL.API.Structs;

public struct Data
{
    public Data(Chart chart, List<DataSet> dataSet, uint microseconds = 5000)
    {
        if (dataSet.Count == 0)
        {
            var trace = new StackTrace();
            string caller = "";
                caller += $"        {trace.GetFrame(1)?.GetMethod()} ";
                caller += $"        {trace.GetFrame(2)?.GetMethod()} ";
            Log.Error($"Date constructor doesnt have any DataSets! Cannot call Data.Call unless data is present!  Called from: {caller}  ");
            return;
        }
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
        var context = $"BEGIN '{ChartTypeId}' {Microseconds}";
        Log.Line(context);
    }
    private void Set()
    {
        foreach (DataSet data in DataSet)
        {
            var context = $"SET '{data.DimensionId}' = {(data.Value.HasValue ? data.Value : "0")}";
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
    private string ChartTypeId { get; } = null!;
    private uint Microseconds { get; }
    private List<DataSet> DataSet { get; } = null!;
}