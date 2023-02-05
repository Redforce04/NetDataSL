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

namespace NetDataSL.StructsAndClasses;

using System.Text.Json.Serialization;

/// <summary>
/// The packet of information sent from the sl server to the NetData integration.
/// </summary>
public class NetDataPacket
{
#pragma warning disable SA1401

    /// <summary>
    /// Initializes a new instance of the <see cref="NetDataPacket"/> class.
    /// </summary>
#pragma warning disable SA1201
    public NetDataPacket()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetDataPacket"/> class.
    /// </summary>
    /// <param name="port">port.</param>
    /// <param name="serverName">server name.</param>
    /// <param name="refreshSpeed">refresh speed.</param>
    /// <param name="epoch">epoch.</param>
    /// <param name="averageTps">average tps.</param>
    /// <param name="averageDeltaTime">average delta time.</param>
    /// <param name="memoryUsage">memory usage.</param>
    /// <param name="cpuUsage">cpu usage.</param>
    /// <param name="players">players.</param>
    /// <param name="lowTpsWarnCount">low tps warn count.</param>
    [JsonConstructor]
    public NetDataPacket(
        int port,
        string serverName,
        double refreshSpeed,
        int epoch,
        double averageTps,
        double averageDeltaTime,
        int memoryUsage,
        double cpuUsage,
        int players,
        int lowTpsWarnCount)
    {
        this.Port = port;
        this.ServerName = serverName;
        this.RefreshSpeed = (float)refreshSpeed;
        this.Epoch = epoch;
        this.AverageTps = (float)averageTps;
        this.AverageDeltaTime = (float)averageDeltaTime;
        this.MemoryUsage = memoryUsage;
        this.CpuUsage = (float)cpuUsage;
        this.Players = players;
        this.LowTpsWarnCount = lowTpsWarnCount;
    }
#pragma warning restore SA1201

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

}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(NetDataPacket))]
#pragma warning disable SA1402, SA1601

// ReSharper disable once UnusedType.Global
internal partial class PacketSerializerContext : JsonSerializerContext
{
}
#pragma warning restore SA1402, SA1601
