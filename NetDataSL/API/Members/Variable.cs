// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Variable.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 2:55 PM
// -----------------------------------------

using Sentry;

namespace NetDataSL.API.Members;

// ReSharper disable twice RedundantNameQualifier
using NetDataSL.API.Enums;
using NetDataSL.API.Extensions;
#pragma warning disable
public class Variable
{
    private void Send(bool localSend = true)
    {
        if (_chart == null && (int)this.Scope == 1)
        {
            Log.Error($"Variable isn't assigned to a chart! It will not be registered unless assigned to a chart or the scope is changed to host or global.");
            return;
        }

        try
        {
            // VARIABLE [SCOPE] name = value
            var content = $"VARIABLE '{Process(Scope, FieldType.Scope)}' " +
                          $"'{Process(Name, FieldType.VariableName)}' = " +
                          $"'{Process(Value, FieldType.Value)}'";
            Log.Line(content);

            if (localSend && (int)this.Scope == 0 && _chart != null)
                _chart.ReloadOtherTriggerSend();

        }
        catch (ArgumentNullException err)
        {
            SentrySdk.CaptureException(err);
            Log.Error("Cannot create the variable because a value was empty. The variable will not be sent.");
        }
    }
    
    internal Variable(VariableScope scope, string name, double value)
    {
        Scope = scope;
        _name = name;
        _value = value;
        Send();
    }

    protected Variable()
    {
        
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

    internal static Variable Create(VariableScope scope, string name, double value)
    {
        return new Variable(scope, name, value);
    }

    internal VariableScope Scope { get; private set; }
    
    internal string Name
    {
        get => _name;
        set
        {
            _name = value;
            Send();
        }
    }

    private string _name = null!;
    internal double Value
    {
        get => _value;
        set
        {
            _value = value;
            Send();
        }
    }
    private double _value;
    private string Process(object value, FieldType field) => Field.Process(value, field);
}
#pragma warning restore