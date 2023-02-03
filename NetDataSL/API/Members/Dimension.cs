// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Dimension.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 12:54 PM
// -----------------------------------------

using NetDataSL.API.Enums;
using NetDataSL.API.Extensions;

namespace NetDataSL.API.Members;

public class Dimension
{
    public Dimension(string id, string name = "", Algorithm algorithm = Algorithm.Absolute, int multiplier = 1, int divisor = 1, bool obsolete = false, bool hidden = false)
    {
        Id = id;
        _name = (name == "" ? id : name);
        _algorithm = algorithm;
        _multiplier = multiplier;
        _divisor = divisor;
        _obsolete = obsolete;
        _hidden = hidden;
        Send();
    }
    protected Dimension()
    {
        // This is specifically for members that are going to override all of the members regardless
    }
    public static Dimension Create(string id, string name = "", Algorithm algorithm = Algorithm.Absolute, int multiplier = 1, int divisor = 1, bool obsolete = false, bool hidden = false)
    {
        return new Dimension(id, name, algorithm, multiplier, divisor, obsolete, hidden);
    }

    public Chart? Chart;

    internal void AssignChart(Chart chart)
    {
        Chart = chart;
    }

    internal void ChartTriggerSend()
    {
        Send(false);
    }

    private void Send(bool localSend = true)
    {
        if (Chart == null)
        {
            Log.Error($"Dimension isn't assigned to a chart! It will not be registered unless assigned to a chart.");
            return;
        }

        try
        {

            // Needs to send the chart first
            // DIMENSION id [name [algorithm [multiplier [divisor [options]]]]]
            var content = $"DIMENSION '{Field.Process(Id, FieldType.DimensionId)}' " +
                          $"'{Process(Name, FieldType.Name)}' " +
                          $"'{Process(Algorithm, FieldType.Algorithm)}' " +
                          $"'{Process(Multiplier, FieldType.Multiplier)}' " +
                          $"'{Process(Divisor, FieldType.Divisor)}' '" +
                          Process(Obsolete, FieldType.Obsolete) +
                          Process(Hidden, FieldType.Hidden) +
                          $"'";
            Log.Line(content);
            if (localSend)
                Chart.ReloadOtherTriggerSend();

        }
        catch (ArgumentNullException)
        {
            Log.Error("Cannot create the dimension because a value was empty. The dimension will not be sent.");
        }
    }

    public virtual string Id { get; private set; } = null!;

    public virtual string Name
    {
        get => _name;
        set
        {
            _name = value;
            Send();
        }
    }
    
    private string _name = null!;

    public virtual Algorithm Algorithm
    {
        get => _algorithm;
        set
        {
            _algorithm = value;
            Send();
        }
    }

    private Algorithm _algorithm;

    public virtual int Multiplier
    {
        get => _multiplier;
        set
        {
            _multiplier = value;
            Send();
        }
    }
    private int _multiplier;

    public virtual int Divisor
    {
        get => _divisor;
        set
        {
            _divisor = value;
            Send();
        }
    }

    private int _divisor;

    public virtual bool Obsolete
    {
        get => _obsolete;
        set
        {
            _obsolete = value;
            Send();
        }
    }
    private bool _obsolete;

    public virtual bool Hidden
    {
        get => _hidden;
        set
        {
            _hidden = value;
            Send();
        }
    }

    private bool _hidden;
    
    private string Process(object value, FieldType field) => Field.Process(value, field);
}


