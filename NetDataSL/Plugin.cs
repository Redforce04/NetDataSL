// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 9:23 PM
// -----------------------------------------

using Sentry;

namespace NetDataSL;

using System.Collections.Concurrent;

// ReSharper disable twice RedundantNameQualifier
using NetDataSL.Networking;
using NetDataSL.StructsAndClasses;

/// <summary>
/// The main plugin. Does all of the processing and is instantiated via <see cref="Program"/>.
/// </summary>
public class Plugin
{
    /// <summary>
    /// The Plugin Singleton.
    /// </summary>
#pragma warning disable SA1401
    public static Plugin? Singleton;

    /// <summary>
    /// A list of all servers present.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public readonly ConcurrentDictionary<int, ServerConfig> Servers = new();

    /// <summary>
    /// The name of the netdata plugin.
    /// </summary>
    internal static readonly string PluginName = "scpsl.plugin";
    private readonly float _serverRefreshTime = -1f;
    private readonly string _tempDirectory = Path.GetTempPath() + "PwProfiler/";
    private float _refreshTime = 5f;
#pragma warning restore SA1401

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="refreshRate">The refresh rate of the plugin.</param>
    /// <param name="host">The host that gRPC should use.</param>
    public Plugin(float refreshRate = 5f, string host = "")
    {
        if (Singleton != null)
        {
            return;
        }

        Singleton = this;
        var unused = new Log();
        var unused2 = new NetworkHandler(host);
        this._refreshTime = refreshRate;
        this.InitNetDataIntegration();
        this.Init();
    }

    /// <summary>
    /// Sends a NetData update for a packet.
    /// </summary>
    /// <param name="packet">The packet to update.</param>
    internal void ProcessPacket(NetDataPacket packet)
    {
        try
        {
            if (packet.Epoch + this.UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                Log.Debug($"Packet from port {packet.Port} is an old packet (older than {this.UsableRefresh()} seconds). This packet will not be processed.");
                return;
            }

            this.UpdateRefreshTime(packet.RefreshSpeed);
            this.ProcessStats(packet);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
        }
    }

    private void InitNetDataIntegration()
    {
        this.GetServers(out var servers);
        var unused = new ChartIntegration(servers);
        var unused2 = new UpdateProcessor();
    }

    private void GetServers(out List<KeyValuePair<int, string>> servers)
    {
        Thread.Sleep(7500);
        servers = new List<KeyValuePair<int, string>>();

        foreach (ServerConfig conf in Config.Singleton!.ServerInstances)
        {
            this.Servers.TryAdd(conf.Port, conf);
            servers.Add(new KeyValuePair<int, string>(conf.Port, conf.ServerName));
        }
    }

    private void Init()
    {
        Log.Debug($"Starting Net-data Integration");
        this.CreateDirectories();
        this.StartMainRunningLoop();
    }

    private void CreateDirectories()
    {
        if (!Directory.Exists(this._tempDirectory))
        {
            Directory.CreateDirectory(this._tempDirectory);
        }
    }

    private void StartMainRunningLoop()
    {
        DateTime restartEveryHour = DateTime.UtcNow.AddHours(1);
        while (DateTime.UtcNow < restartEveryHour)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            now = now.Add(new TimeSpan(0, 0, (int)this.UsableRefresh()));

            // Get the delay necessary.
            // foreach (var filePath in Directory.GetFiles(_tempDirectory)) _processFile(filePath);
            UpdateProcessor.Singleton!.SendUpdate();
            Log.Singleton!.LogMessages();
            var delay = now.Subtract(DateTime.UtcNow.TimeOfDay).TotalMilliseconds;
            if (delay < 1)
            {
                delay = 1;
            }

            Thread.Sleep((int)delay);

            // Log.Debug($"Iteration Time: {new TimeSpan(0, 0, 0, 0, (int)(this.UsableRefresh() * 1000) - (int)delay):g}");
        }

        Log.Debug($"Hourly Restart For Memory Preservation (As Recommended by Netdata API)");
    }

    private float UsableRefresh()
    {
        return this._serverRefreshTime > this._refreshTime ? this._serverRefreshTime : this._refreshTime;
    }

    private void UpdateRefreshTime(float updateTime)
    {
        if (Math.Abs(updateTime - this._refreshTime) > .1f)
        {
            Log.Debug($"Updated Refresh Speed.");
            this._refreshTime = updateTime;
        }
    }

    private void ProcessStats(NetDataPacket packet)
    {
        Log.Debug($"Received Stats Data");
        UpdateProcessor.Singleton!.ProcessUpdate(packet);
    }
}