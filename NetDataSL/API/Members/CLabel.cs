// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         CLabel.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 5:22 PM
// -----------------------------------------

namespace NetDataSL.API.Members;

#pragma warning disable
// ReSharper disable twice RedundantNameQualifier
using NetDataSL.API.Enums;
using NetDataSL.API.Extensions;

public class CLabel
{
    public CLabel(string name, string value, LabelSource source = LabelSource.Automatically)
    {
        Name = name;
        Value = value;
        Source = source;
    }

    public static CLabel Create(string name, string value, LabelSource source = LabelSource.Automatically)
    {
        return new CLabel(name, value, source);
    }
    protected CLabel()
    {
        // This is specifically for members that are going to override all of the members regardless
    }
    private void Send(bool localSend = true)
    {
        if (_chart == null)
        {
            Log.Error($"CLabel isn't assigned to a chart! It will not be registered unless assigned to a chart.");
            return;
        }

        try
        {

            var content = $"CLABEL '{Process(Name, FieldType.CLabelName)}' " +
                          $"'{Process(Value, FieldType.CLabelValue)}' " +
                          $"'{Process(Source, FieldType.CLabelSource)}'";
            Log.Line(content.Replace("[","").Replace("]",""));
            Commit();
            if (localSend)
                _chart.ReloadOtherTriggerSend();

        }
        catch (ArgumentNullException)
        {
            Log.Error("Cannot create the clabel because a value was empty. The clabel will not be sent.");
        }

    }

    private void Commit()
    {
        var content = "CLABEL_COMMIT";
        Log.Line(content);
    }

    private Chart? _chart;

    internal void AssignChart(Chart chart)
    {
        _chart = chart;
    }

    internal void ChartTriggerSend()
    {
        Send(false);
    }
    
    internal virtual string Name { get; } = null!;

    internal virtual string Value { get; } = null!;

    internal virtual LabelSource Source { get; }
    
    private string Process(object value, FieldType field) => Field.Process(value, field);
}
#pragma warning restore