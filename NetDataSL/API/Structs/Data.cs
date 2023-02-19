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

namespace NetDataSL.API.Structs;

// ReSharper disable twice RedundantNameQualifier
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NetDataSL.API.Members;

/// <summary>
/// Data to update.
/// </summary>
public struct Data
{
    private bool _sent;

    private bool _cancel;

    private string ChartTypeId { get; } = null!;

    private uint Microseconds { get; }

    private List<DataSet> DataSet { get; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Data"/> struct.
    /// </summary>
    /// <param name="chart">The chart to update.</param>
    /// <param name="dataSet">The list of data sets to update.</param>
    /// <param name="microseconds">The milliseconds since the last update.</param>
    /// <param name="sendOnCreation">Should the dataset output the data on creation.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
#pragma warning disable SA1201
    public Data(Chart chart, List<DataSet> dataSet, uint microseconds = 5000, bool sendOnCreation = true)
#pragma warning restore SA1201
    {
        if (dataSet.Count == 0)
        {
            var trace = new StackTrace();
            string caller = string.Empty;
            caller += $"        {trace.GetFrame(1)?.GetMethod()} ";
            caller += $"        {trace.GetFrame(2)?.GetMethod()} ";
            Log.Error($"Date constructor doesnt have any DataSets! Cannot call Data.Call unless data is present!  Called from: {caller}  ");
            return;
        }

        this.ChartTypeId = chart.TypeId;
        this.Microseconds = microseconds;
        this.DataSet = dataSet;
        this._cancel = false;
        this._sent = false;
        if (sendOnCreation)
        {
            this.Call();
        }
    }

    /// <summary>
    /// Allows flushing all of the data. This cancels the sends and undos any part that has already been sent.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public void Flush()
    {
        if (!this._sent)
        {
            this._cancel = true;
            var context = $"FLUSH";
            Log.Line(context);
        }
    }

    /// <summary>
    /// Outputs data to console, thereby sending it to NetData.
    /// </summary>
    public void Call()
    {
        this.Begin();
        this.Set();
        if (!this._cancel)
        {
            this.End();
        }
    }

    private void Begin()
    {
        var context = $"BEGIN '{this.ChartTypeId}' {this.Microseconds}";
        Log.Line(context);
    }

    private void Set()
    {
        foreach (DataSet data in this.DataSet)
        {
            var context = $"SET '{data.DimensionId}' = {(data.Value.HasValue ? data.Value : "0")}";
            this._sent = true;
            Log.Line(context);
        }
    }

    private void End()
    {
        var context = "END";
        Log.Line(context);
    }
}