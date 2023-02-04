// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NetDataPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/30/2023 2:08 PM
// -----------------------------------------


using System.Text.Json.Serialization;

namespace NetDataSL.Structs;

public class NetDataPacketHandler
{
#pragma warning disable SA1401

    /// <summary>
    /// Gets or sets the port of the sending server.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the name of the sending server.
    /// </summary>
    public string ServerName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the refresh speed of the sending server.
    /// </summary>
    public float RefreshSpeed { get; set; }

    /// <summary>
    /// Gets or sets the epoch of when this packet was made.
    /// </summary>
    public long Epoch { get; set; }

    /// <summary>
    /// Gets or sets the datetime of when this packet was made.
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// Gets or sets the average tps of when this packet was made.
    /// </summary>
    public float AverageTps { get; set; }

    /// <summary>
    /// Gets or sets the average delta time of when this packet was made.
    /// </summary>
    public float AverageDeltaTime { get; set; }

    /// <summary>
    /// Gets or sets the memory usage at the time this packet was made.
    /// </summary>
    public long MemoryUsage { get; set; }

    /// <summary>
    /// Gets or sets the cpu usage at the time this packet was made.
    /// </summary>
    public float CpuUsage { get; set; }

    /// <summary>
    /// Gets or sets the amount of players on the server at the time this packet was made.
    /// </summary>
    public int Players { get; set; }

    /// <summary>
    /// Gets or sets how many low tps warnings have gone off since the last time packets were collected.
    /// </summary>
    public int LowTpsWarnCount { get; set; }
#pragma warning restore SA1401
    /// <summary>
    /// Initializes a new instance of the <see cref="NetDataPacketHandler"/> class.
    /// </summary>
#pragma warning disable SA1201
    [JsonConstructor]
    public NetDataPacketHandler()
#pragma warning restore SA1201
    {
    }
}
