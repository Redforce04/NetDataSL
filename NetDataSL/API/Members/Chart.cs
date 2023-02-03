// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Chart.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 9:21 PM
// -----------------------------------------

using NetDataSL.API.Enums;
using NetDataSL.API.Extensions;

namespace NetDataSL.API.Members;

/// <summary>
/// Creates or updates a chart.
/// </summary>
public class Chart
{
    public Chart(string type, string id, string title, List<Dimension> dimensions, 
        List<Variable>? variables = null, List<CLabel>? cLabels = null, string name = "",
        string module = "stats", string units = "percentage", string family = "", 
        string context = "", ChartType chartType = ChartType.Line, int priority = 1000, 
        float updateEvery = 5f, ChartOptions options = ChartOptions.None)
    {
        Type = type;
        Id = id;
        _title = title;
        Dimensions = dimensions;
        foreach (Dimension dimension in Dimensions)
        {
            dimension.AssignChart(this);
            Dimensions.Add(dimension);
        }
        if(variables != null)
            foreach (Variable variable in variables)
            {
                variable.AssignChart(this);
                Variables.Add(variable);
            }
        if(cLabels != null)
            foreach (CLabel label in cLabels)
            {
                label.AssignChart(this);
                CLabels.Add(label);
            }
        _name = name;
        _module = module;
        _units = units;
        _family = (family == "") ? id : family; 
        _context = context;
        _chartType = chartType;
        _priority = priority; 
        _updateEvery = updateEvery;
        _chartOptions = options;
        _addChart(this);
    }

    protected Chart()
    {
        // This is specifically for members that are going to override all of the members regardless
        _addChart(this);
    }

    public static Chart Create(string type, string id, string title, List<Dimension> dimensions,
        List<Variable>? variables = null, List<CLabel>? cLabels = null, string name = "",
        string module = "stats", string units = "percentage", string family = "", 
        string context = "", ChartType chartType = ChartType.Line, int priority = 1000, 
        float updateEvery = 5f, ChartOptions options = ChartOptions.None)
    {
        return new Chart(type, id, title, dimensions, variables, cLabels, name, module, units, family, context, chartType, priority, updateEvery, options);
    }
    /// <summary>
    /// The method used to create the chart.
    /// </summary>
    private void Send()
    {
        // CHART type.id name title units [family [context [charttype [priority [update_every [options [plugin [module]]]]]]]]
        try
        {

            var content = $"CHART '{Process(TypeId, FieldType.TypeId)}' " +
                          $"'{Process(Name, FieldType.Name)}' " +
                          $"'{Process(Title, FieldType.Title)}' " +
                          $"'{Process(Units, FieldType.Units)}' " +
                          $"'{Process(Family, FieldType.Family)}' " +
                          $"'{Process(Context, FieldType.Context)}' " +
                          $"'{Process(ChartType, FieldType.ChartType)}' " +
                          $"'{Process(Priority, FieldType.Priority)}' " +
                          $"'{Process(UpdateEvery, FieldType.UpdateEvery)}' '" +
                          Process(Obsolete, FieldType.Obsolete) +
                          Process(Obsolete, FieldType.Detail) +
                          Process(Obsolete, FieldType.StoreFirst) +
                          Process(Obsolete, FieldType.Hidden) +
                          $"' '{Process(_plugin, FieldType.Plugin)}' " +
                          $"'{Process(Module, FieldType.Module)}'";
                          Log.Line(content);
        }
        catch (ArgumentNullException)
        {
            Log.Error("Cannot create the chart because a value was empty. The chart will not be sent.");
            return;
        }

        foreach(Dimension dimension in Dimensions)
            dimension.ChartTriggerSend();
        foreach (Variable variable in Variables)
            variable.ChartTriggerSend();
        foreach (CLabel cLabel in CLabels)
            cLabel.ChartTriggerSend();
    }
    /// <summary>
    /// Uniquely identifies the chart, this is what will be needed to add values to the chart.
    /// The type part controls the menu the charts will appear in.
    /// </summary>
    /// <seealso cref="Type"/>
    /// <seealso cref="Id"/>
    /// <example></example>>
    public string TypeId => Type + "." + Id;

    /// <summary>
    /// Controls the menu the charts will appear in
    /// </summary>
    public virtual string Type { get; private set; } = null!;

    /// <summary>
    /// Uniquely identifies the chart.
    /// </summary>
    public virtual string Id { get; private set; } = null!;

    /// <summary>
    /// The name that will be presented to the user instead of <see cref="Id"/> in <see cref="TypeId"/>.
    /// This means that only the <see cref="Id"/> part of <see cref="TypeId"/> is changed.
    /// When a name has been given, the chart is indexed (and can be referred) as both
    /// <see cref="Type"/>.<see cref="Id"/> and <see cref="Type"/>.<see cref="Name"/>.
    /// You can set name to '', or null, or (null) to disable it. If a chart with the same name already exists,
    /// a serial number is automatically attached to the name to avoid naming collisions.
    /// </summary>
    public virtual string Name
    {
        get => _name;
        set
        {
            _name = value;
            Send();
        }
    }

    private string _name = "";

    /// <summary>
    /// The text above the chart.
    /// </summary>
    public virtual string Title
    {
        get => _title;
        set
        {
            _title = value;
            Send();
        }
    }

    private string _title = "";

    /// <summary>
    /// The label of the vertical axis of the chart, all dimensions added to a chart should have the same units of measurement.
    /// </summary>
    public virtual string Units
    {
        get => _units;
        set
        {
            _units = value;
            Send();
        }
    }

    private string _units = "";

    /// <summary>
    /// Used to group charts together, if empty or missing, the id part of type.id will be used
    /// This controls the sub-menu on the dashboard.
    /// </summary>
    /// <example>
    /// All eth0 charts should say: eth0
    /// </example>
    public virtual string Family
    {
        get => _family;
        set
        {
            _family = value;
            Send();
        }
    }

    private string _family = "";

    /// <summary>
    /// Gives the template of the chart. EX: multiple charts present same info for a different family.
    /// Ie. Context = Players, Families = Net 1, Net 2, Test Net etc... 
    /// </summary>
    public virtual string Context
    {
        get => _context;
        set
        {
            _context = value;
            Send();
        }
    }
    private string _context = "";

    public virtual ChartType ChartType
    {
        get => _chartType;
        set
        {
            _chartType = value;
            Send();
        }
    }

    private ChartType _chartType = ChartType.Line;

    /// <summary>
    /// Lower Number makes the chart appear higher on the page.
    /// </summary>
    public virtual int Priority
    {
        get => _priority;
        set
        {
            _priority = value;
            Send();
        }
    }

    private int _priority = 1000;

    /// <summary>
    /// How often it should be updated.
    /// </summary>
    public virtual float UpdateEvery
    {
        get => _updateEvery;
        set
        {
            _updateEvery = value;
            Send();
        }
    }

    private float _updateEvery = 5f;

    public virtual ChartOptions Options
    {
        get => _chartOptions;
        set
        {
            _chartOptions = value;
            Send();
        }
    }

    private ChartOptions _chartOptions = ChartOptions.None;

    public virtual bool Obsolete
    {
        get => _chartOptions.HasFlag(ChartOptions.Obsolete);
        set
        {
            if(!Obsolete && value)
                _chartOptions |= ChartOptions.Obsolete;
            else if (Obsolete && !value)
                _chartOptions &= ChartOptions.Obsolete;
        }
    }
    
    public virtual bool Detail
    {
        get => _chartOptions.HasFlag(ChartOptions.Detail);
        set
        {
            if(!Detail && value)
                _chartOptions |= ChartOptions.Detail;
            else if (Detail && !value)
                _chartOptions &= ChartOptions.Detail;
        }
    }
    
    public virtual bool StoreFirst
    {
        get => _chartOptions.HasFlag(ChartOptions.StoreFirst);
        set
        {
            if(!StoreFirst && value)
                _chartOptions |= ChartOptions.StoreFirst;
            else if (StoreFirst && !value)
                _chartOptions &= ChartOptions.StoreFirst;
        }
    }
    
    public virtual bool Hidden
    {
        get => _chartOptions.HasFlag(ChartOptions.Hidden);
        set
        {
            if(!Hidden && value)
                _chartOptions |= ChartOptions.Hidden;
            else if (Hidden && !value)
                _chartOptions &= ChartOptions.Hidden;
        }
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// proc.plugin,
    /// ebpf.plugin,
    /// go.d
    /// </example>
    private readonly string _plugin = Plugin.Singleton != null ? Plugin.Singleton.pluginName : "plugin";

    /// <summary>
    /// </summary>
    /// <example>
    /// /proc/meminfo,
    /// /proc/net/netstat,
    /// mount,
    /// docker
    /// </example>
    public virtual string Module
    {
        get => _module;
        set
        {
            _module = value;
            Send();
        }
    }

    private string _module = null!;

    public List<Dimension> Dimensions { get; protected set; } = new List<Dimension>();
    public List<Variable> Variables { get; protected set; } = new List<Variable>();
    public List<CLabel> CLabels { get; protected set; } = new List<CLabel>();

    /// <summary>
    /// For other methods to update.
    /// </summary>
    internal void ReloadOtherTriggerSend()
    {
        Send();
    }
    private string Process(object value, FieldType field) => Field.Process(value, field);

    private static readonly List<Chart> Charts = new List<Chart>();
    private static void _addChart(Chart chart) => Charts.Add(chart);

    public static void RegisterAllCharts()
    {
        foreach (var chart in Charts)
        {
            chart.Send();
        }
    }

    internal static Chart? GetChart(string type, string id)
    {
        return Charts.FirstOrDefault(x => x.Type == type && x.Id == id);
    }
}