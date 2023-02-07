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
#pragma warning disable SA1201, SA1313, SA1114, CS8618
    [JsonConstructor]
    public NetDataPacket()
    {
    }
#pragma warning restore CS8618, CS8618

    // ReSharper disable UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// Initializes a new instance of the <see cref="NetDataPacket"/> class.
    /// </summary>
    /// <param name="Port">port.</param>
    /// <param name="ServerName">server name.</param>
    /// <param name="RefreshSpeed">refresh speed.</param>
    /// <param name="Epoch">epoch.</param>
    /// <param name="AverageTps">average tps.</param>
    /// <param name="AverageDeltaTime">average delta time.</param>
    /// <param name="MemoryUsage">memory usage.</param>
    /// <param name="CpuUsage">cpu usage.</param>
    /// <param name="Players">players.</param>
    /// <param name="LowTpsWarnCount">low tps warn count.</param>
    /// <param name="ApiKey">The api key of the sl server plugin.</param>
    /// <param name="ApiVersion">The api version on the sl server plugin.</param>
    /// <param name="PluginVersion">The version of the plugin on the sl server.</param>
    public NetDataPacket(

        // ReSharper disable InconsistentNaming
        int Port,
        string ServerName,
        double RefreshSpeed,
        int Epoch,
        double AverageTps,
        double AverageDeltaTime,
        int MemoryUsage,
        double CpuUsage,
        int Players,
        int LowTpsWarnCount,
        string ApiKey,
        string ApiVersion,
        string PluginVersion)
    {
        this.Port = Port;
        this.ServerName = ServerName;
        this.RefreshSpeed = (float)RefreshSpeed;
        this.Epoch = Epoch;
        this.DateTime = DateTimeOffset.FromUnixTimeSeconds(Epoch).DateTime;
        this.AverageTps = (float)AverageTps;
        this.AverageDeltaTime = (float)AverageDeltaTime;
        this.MemoryUsage = MemoryUsage;
        this.CpuUsage = (float)CpuUsage;
        this.Players = Players;
        this.LowTpsWarnCount = LowTpsWarnCount;
        this.ApiKey = ApiKey;
        this.ApiVersion = ApiVersion;
        this.PluginVersion = PluginVersion;
    }

    /// <summary>
    /// Gets or sets the port of the sending server.
    /// </summary>
    [JsonPropertyName("Port")]
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the name of the sending server.
    /// </summary>
    [JsonPropertyName("ServerName")]
    public string ServerName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the refresh speed of the sending server.
    /// </summary>
    [JsonPropertyName("RefreshSpeed")]
    public float RefreshSpeed { get; set; }

    /// <summary>
    /// Gets or sets the epoch of when this packet was made.
    /// </summary>
    [JsonPropertyName("Epoch")]
    public long Epoch { get; set; }

    /// <summary>
    /// Gets or sets the datetime of when this packet was made.
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// Gets or sets the average tps of when this packet was made.
    /// </summary>
    [JsonPropertyName("AverageTps")]
    public float AverageTps { get; set; }

    /// <summary>
    /// Gets or sets the average delta time of when this packet was made.
    /// </summary>
    [JsonPropertyName("AverageDeltaTime")]
    public float AverageDeltaTime { get; set; }

    /// <summary>
    /// Gets or sets the memory usage at the time this packet was made.
    /// </summary>
    [JsonPropertyName("MemoryUsage")]
    public long MemoryUsage { get; set; }

    /// <summary>
    /// Gets or sets the cpu usage at the time this packet was made.
    /// </summary>
    [JsonPropertyName("CpuUsage")]
    public float CpuUsage { get; set; }

    /// <summary>
    /// Gets or sets the amount of players on the server at the time this packet was made.
    /// </summary>
    [JsonPropertyName("Players")]
    public int Players { get; set; }

    /// <summary>
    /// Gets or sets how many low tps warnings have gone off since the last time packets were collected.
    /// </summary>
    [JsonPropertyName("LowTpsWarnCount")]
    public int LowTpsWarnCount { get; set; }

    /// <summary>
    /// Gets or sets the API Key of the server.
    /// </summary>
    [JsonPropertyName("ApiKey")]
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the version of the api on the sl server plugin.
    /// </summary>
    [JsonPropertyName("ApiVersion")]
    public string ApiVersion { get; set; }

    /// <summary>
    /// Gets or sets the version of the plugin on the sl server plugin.
    /// </summary>
    [JsonPropertyName("PluginVersion")]
    public string PluginVersion { get; set; }

#pragma warning restore SA1201, SA1313, SA1114, SA1401

}

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
[JsonSerializable(typeof(NetDataPacket))]
#pragma warning disable SA1402, SA1601

// ReSharper disable once UnusedType.Global
internal partial class PacketSerializerContext
    : JsonSerializerContext
{
}
#pragma warning restore SA1402, SA1601
