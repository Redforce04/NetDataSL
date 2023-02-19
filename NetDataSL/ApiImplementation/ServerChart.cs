// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         CpuChart.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 10:43 PM
// -----------------------------------------

namespace NetDataSL.ApiImplementation;

using NetDataSL.API.Enums;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.API.Members;

/// <inheritdoc />
public class ServerChart : Chart
{
    // string type, string id, string title, string name = "",
    // string module = "stats", string units = "percentage", string family = "";
    // string context = "", ChartType chartType = ChartType.Line, int priority = 1000;
    // float updateEvery = 5f, ChartOptions options = ChartOptions.None

    /// <summary>
    /// Gets or sets the name of the server this chart represents.
    /// </summary>
    private readonly string _serverName;

    /// <summary>
    /// Gets or sets the port of the server this chart represents.
    /// </summary>
    private readonly int _serverPort;

    /// <inheritdoc/>
    public override string Type => "scpsl";

    /// <inheritdoc/>
    public override string Id => $"server.{this._serverPort}";

    /// <inheritdoc/>
    public override string Name => $"{this._serverName} Stats";

    /// <inheritdoc/>
    public override string Title => $"{this._serverName} Stats";

    /// <inheritdoc/>
    public override string Units => "Server Stats";

    /// <inheritdoc/>
    public override string Family => $"{this._serverName}";

    /// <inheritdoc/>
    public override string Context => $"scpsl.{this._serverName}";

    /// <inheritdoc/>
    public override ChartType ChartType => ChartType.Line;

    /// <inheritdoc/>
    public override int Priority => 1000;

    /// <inheritdoc/>
    public override float UpdateEvery => 5f;

    /// <inheritdoc/>
    public override ChartOptions Options => ChartOptions.None;

    /// <inheritdoc/>
    public override string Module => "ServerStats";

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerChart"/> class.
    /// </summary>
    /// <param name="dimensions">The dimensions for this chart.</param>
    /// <param name="port">The port of the server this chart represents.</param>
    /// <param name="serverName">The name of the server this chart represents.</param>
#pragma warning disable SA1201
    internal ServerChart(List<Dimension> dimensions, int port, string serverName)
    {
        this._serverPort = port;
        this._serverName = serverName;
        this.Dimensions = dimensions;
        foreach (Dimension dimension in this.Dimensions)
        {
            dimension.AssignChart(this);
        }

        foreach (Variable variable in this.Variables)
        {
            variable.AssignChart(this);
        }

        foreach (CLabel label in this.CLabels)
        {
            label.AssignChart(this);
        }
    }
#pragma warning restore SA1201
}

#pragma warning disable SA1402
/// <summary>
/// A dimension for individual server charts measuring the cpu usage.
/// </summary>
internal class ServerCpuChartDimensions : Dimension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerCpuChartDimensions"/> class.
    /// </summary>
    /// <param name="server">The server port.</param>
    /// <param name="serverName">The server name.</param>
    public ServerCpuChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    /// <inheritdoc/>
    public override string Id => $"stats.{this.Server}.cpu";

    /// <inheritdoc/>
    public override string Name => $"Cpu Usage";

    /// <inheritdoc/>
    public override Algorithm Algorithm => Algorithm.Absolute;

    /// <inheritdoc/>
    public override int Multiplier => 1;

    /// <inheritdoc/>
    public override int Divisor => 1000;

    /// <inheritdoc/>
    public override bool Obsolete => false;

    /// <inheritdoc/>
    public override bool Hidden => false;

    private int Server { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string ServerName { get; }
}

/// <summary>
/// A dimension for individual server charts measuring the average tps.
/// </summary>
internal class ServerTpsChartDimensions : Dimension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerTpsChartDimensions"/> class.
    /// </summary>
    /// <param name="server">The server port.</param>
    /// <param name="serverName">The server name.</param>
    public ServerTpsChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    /// <inheritdoc/>
    public override string Id => $"stats.{this.Server}.tps";

    /// <inheritdoc/>
    public override string Name => $"Average Tps";

    /// <inheritdoc/>
    public override Algorithm Algorithm => Algorithm.Absolute;

    /// <inheritdoc/>
    public override int Multiplier => 1;

    /// <inheritdoc/>
    public override int Divisor => 1000;

    /// <inheritdoc/>
    public override bool Obsolete => false;

    /// <inheritdoc/>
    public override bool Hidden => false;

    private int Server { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string ServerName { get; }
}

/// <summary>
/// A dimension for individual server charts measuring the memory usage.
/// </summary>
internal class ServerMemoryChartDimensions : Dimension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerMemoryChartDimensions"/> class.
    /// </summary>
    /// <param name="server">The server port.</param>
    /// <param name="serverName">The server name.</param>
    public ServerMemoryChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    /// <inheritdoc/>
    public override string Id => $"stats.{this.Server}.memory";

    /// <inheritdoc/>
    public override string Name => $"Memory Usage";

    /// <inheritdoc/>
    public override Algorithm Algorithm => Algorithm.Absolute;

    /// <inheritdoc/>
    public override int Multiplier => 1;

    /// <inheritdoc/>
    public override int Divisor => 1000;

    /// <inheritdoc/>
    public override bool Obsolete => false;

    /// <inheritdoc/>
    public override bool Hidden => false;

    private int Server { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string ServerName { get; }
}

/// <summary>
/// A dimension for individual server charts measuring the player count.
/// </summary>
internal class ServerPlayersChartDimensions : Dimension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerPlayersChartDimensions"/> class.
    /// </summary>
    /// <param name="server">The server port.</param>
    /// <param name="serverName">The server name.</param>
    public ServerPlayersChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    /// <inheritdoc/>
    public override string Id => $"stats.{this.Server}.players";

    /// <inheritdoc/>
    public override string Name => $"Players";

    /// <inheritdoc/>
    public override Algorithm Algorithm => Algorithm.Absolute;

    /// <inheritdoc/>
    public override int Multiplier => 1;

    /// <inheritdoc/>
    public override int Divisor => 1;

    /// <inheritdoc/>
    public override bool Obsolete => false;

    /// <inheritdoc/>
    public override bool Hidden => false;

    private int Server { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string ServerName { get; }
}

/// <summary>
/// A dimension for individual server charts measuring the instances of low tps warnings.
/// </summary>
internal class ServerLowTpsChartDimensions : Dimension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerLowTpsChartDimensions"/> class.
    /// </summary>
    /// <param name="server">The server port.</param>
    /// <param name="serverName">The server name.</param>
    public ServerLowTpsChartDimensions(int server, string serverName)
    {
        this.Server = server;
        this.ServerName = serverName;
    }

    /// <inheritdoc/>
    public override string Id => $"stats.{this.Server}.lowtps";

    /// <inheritdoc/>
    public override string Name => $"Low Tps Warnings";

    /// <inheritdoc/>
    public override Algorithm Algorithm => Algorithm.Absolute;

    /// <inheritdoc/>
    public override int Multiplier => 1;

    /// <inheritdoc/>
    public override int Divisor => 1;

    /// <inheritdoc/>
    public override bool Obsolete => false;

    /// <inheritdoc/>
    public override bool Hidden => false;

    private int Server { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string ServerName { get; }
}
#pragma warning restore SA1402
