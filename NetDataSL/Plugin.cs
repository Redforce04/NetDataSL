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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using NetDataService;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.Networking;
using Newtonsoft.Json;

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
    public readonly Dictionary<int, string> Servers = new()
    {
        { 0, "Unknown Server" },
        { 9011, "Net 1" },
        { 9012, "Net 2" },
        { 9017, "Testing Net" },
    };

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
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
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

    private static void ReadFileContent(string filePath, out string content)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        content = sr.ReadToEnd();
        sr.Close();
        fs.Close();
    }

    private void InitNetDataIntegration()
    {
        this.GetServers(out var servers);
        var unused = new ChartIntegration(servers);
        var unused2 = new UpdateProcessor();
    }

    private void GetServers(out List<KeyValuePair<int, string>> servers)
    {
        servers = new List<KeyValuePair<int, string>>();
        foreach (var filePath in Directory.GetFiles(this._tempDirectory))
        {
            ReadFileContent(filePath, out var content);
            if (content.Length < 2)
            {
                continue;
            }

            try
            {
                NetDataPacket packet = JsonConvert.DeserializeObject<NetDataPacket>(content);
                servers.Add(new KeyValuePair<int, string>(packet.Port, packet.ServerName));
            }
            catch (Exception)
            {
                // Log.Error($"Could not deserialize content of File {filePath}. {e}");
            }
        }
    }

    private void Init()
    {
        Log.Debug($"Starting Net-data Integration");
        this.CreateDirectories();
        this.StartMainListenLoop();
    }

    private void CreateDirectories()
    {
        if (!Directory.Exists(this._tempDirectory))
        {
            Directory.CreateDirectory(this._tempDirectory);
        }
    }

    private void StartMainListenLoop()
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

#pragma warning disable SA1300

    // ReSharper disable once UnusedMember.Local
    private void _processFile(string filePath)
#pragma warning restore SA1300
    {
        try
        {
            // Use this instead of file.ReadAllText() because it has FileShare settings.
            ReadFileContent(filePath, out var content);
            if (content.Length < 2)
            {
                return;
            }

            NetDataPacket packet = JsonConvert.DeserializeObject<NetDataPacket>(content);
            this.ProcessTextEntry(packet);
        }
        catch (Exception)
        {
            // Log.Error($"Could not read or deserialize file '{filePath}'. Exception {e}");
        }
    }

    private void ProcessTextEntry(NetDataPacket packet)
    {
        try
        {
            if (packet.Epoch + this.UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                Log.Debug($"Server {packet.Port} cannot be refreshed, as it hasn't refreshed in 5 seconds.");
                return;
            }

            this.UpdateRefreshTime(packet.RefreshSpeed);
            this.ProcessStats(packet);
        }
        catch (Exception e)
        {
            Log.Error($"could not parse structs.\n{e}");
        }
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