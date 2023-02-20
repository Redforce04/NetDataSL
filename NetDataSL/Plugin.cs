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
    internal static readonly string PluginName = "ScpSL.plugin";

    /// <summary>
    /// How often the server should send updates.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    internal float ServerRefreshTime = 5f;

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
        if (this.ServerRefreshTime > Config.Singleton!.SendRate)
        {
            this.ServerRefreshTime = Config.Singleton.SendRate;
        }

        Log.AddBreadcrumb("Loading Plugin", "Plugin", new Dictionary<string, string>());
        Log.AddBreadcrumb("Loading Log", "Plugin", new Dictionary<string, string>());
        var unused = new Log();
        Log.AddBreadcrumb("Logging System Loaded", "Plugin", new Dictionary<string, string>());
        Log.AddBreadcrumb("Loading Network Handler", "Plugin", new Dictionary<string, string>() { { "host", $"{host}" } });
        var unused2 = new NetworkHandler(host);
        Log.AddBreadcrumb("Network Handler Loaded", "Plugin", new Dictionary<string, string>() { { "host", $"{host}" } });
        this.ServerRefreshTime = refreshRate;
        Log.AddBreadcrumb("Init Integration", "Plugin", new Dictionary<string, string>());
        this.InitNetDataIntegration();
        Log.AddBreadcrumb("Integration Initialized", "Plugin", new Dictionary<string, string>());
        Log.Debug($"Starting Net-data Integration");
        Log.AddBreadcrumb("Start Main Loop", "Plugin", new Dictionary<string, string>());
        this.StartMainRunningLoop();
    }

    private void InitNetDataIntegration()
    {
        Log.AddBreadcrumb("Getting Servers", "Plugin", new Dictionary<string, string>());
        this.GetServers(out var servers);
        Log.AddBreadcrumb("Got Servers", "Plugin", new Dictionary<string, string>() { { "server count", servers.Count.ToString() } });
        Log.AddBreadcrumb("Creating Chart Integration", "Plugin", new Dictionary<string, string>());
        var unused = new ChartIntegration(servers);
        Log.AddBreadcrumb("Chart Integration Created", "Plugin", new Dictionary<string, string>());
        Log.AddBreadcrumb("Creating Update Processor", "Plugin", new Dictionary<string, string>());
        var unused2 = new UpdateProcessor();
        Log.AddBreadcrumb("Update Processor Created", "Plugin", new Dictionary<string, string>());
    }

    private void GetServers(out List<KeyValuePair<int, string>> servers)
    {
        servers = new List<KeyValuePair<int, string>>();

        foreach (ServerConfig conf in Config.Singleton!.ServerInstances)
        {
            this.Servers.TryAdd(conf.Port, conf);
            servers.Add(new KeyValuePair<int, string>(conf.Port, conf.ServerName));
        }
    }

    private void StartMainRunningLoop()
    {
        DateTime restartEveryHour = DateTime.UtcNow.AddHours(1);
        while (DateTime.UtcNow < restartEveryHour)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            now = now.Add(new TimeSpan(0, 0, (int)this.ServerRefreshTime));

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
        if (NetworkHandler.Singleton is not null)
        {
            NetworkHandler.Singleton.Stop();
        }

        Environment.Exit(0);
    }
}