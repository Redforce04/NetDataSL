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

using NetDataService;
using NetDataSL.Networking;

namespace NetDataSL;

using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Sentry;


public class Plugin
{
    /// <summary>
    /// The Plugin Singleton
    /// </summary>
#pragma warning disable SA1401
    public static Plugin? Singleton;
#pragma warning restore SA1401

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="refreshRate">The refresh rate of the plugin.</param>
    public Plugin(float refreshRate = 5f)
    {
        if (Singleton != null)
        {
            return;
        }

        Singleton = this;
        var unused = new Log();
        var unused2 = new NetworkHandler();
        this.refreshTime = refreshRate;
        this.InitNetDataIntegration();
        this.Init();
    }


    internal readonly string pluginName = "scpsl.plugin";
    private float refreshTime = 5f;
    private readonly float _serverRefreshTime = -1f;
    private readonly string _tempDirectory = Path.GetTempPath() + "PwProfiler/";

    public readonly Dictionary<int, string> Servers = new()
    {
        { 0, "Unknown Server" },
        { 9011, "Net 1" },
        { 9012, "Net 2" },
        { 9017, "Testing Net" },
    };

    private void InitNetDataIntegration()
    {
        this._getServers(out var servers);
        var unused = new ChartIntegration(servers);
        var unused2 = new UpdateProcessor();
    }

    private void _getServers(out List<KeyValuePair<int, string>> servers)
    {
        servers = new List<KeyValuePair<int, string>>();
        foreach (var filePath in Directory.GetFiles(this._tempDirectory))
        {
            this._readFileContent(filePath, out var content);
            if (content.Length < 2)
            {
                continue;
            }

            try
            {
                NetDataPacket packet = JsonConvert.DeserializeObject<NetDataPacket>(content);
                servers.Add(new KeyValuePair<int, string>(packet.Port, packet.ServerName));
            }
            catch (Exception e)
            {
                // Log.Error($"Could not deserialize content of File {filePath}. {e}");
                continue;
            }
        }
    }

    private void Init()
    {
        Log.Debug($"Starting Net-data Integration");
        this.CreateDirectories();
        this._startMainListenLoop();
    }

    private void CreateDirectories()
    {
        if (!Directory.Exists(this._tempDirectory))
        {
            Directory.CreateDirectory(this._tempDirectory);
        }
    }

    private void _startMainListenLoop()
    {
        DateTime restartEveryHour = DateTime.UtcNow.AddHours(1);
        while (DateTime.UtcNow < restartEveryHour)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            now = now.Add(new TimeSpan(0, 0, (int)UsableRefresh()));
            // Get the delay necessary.
            foreach (var filePath in Directory.GetFiles(_tempDirectory)) _processFile(filePath);
            
            UpdateProcessor.Singleton!.SendUpdate();
            Log.Singleton!.LogMessages();
            var delay = now.Subtract(DateTime.UtcNow.TimeOfDay).TotalMilliseconds;
            if (delay < 1)
                delay = 1;
            Thread.Sleep((int)delay);
            Log.Debug($"Iteration Time: {new TimeSpan(0, 0,0,0,(int)(UsableRefresh() * 1000) - (int)delay):g}");
        }
        Log.Debug($"Hourly Restart For Memory Preservation (As Recommended by Netdata API)");
    }

    private float UsableRefresh()
    {
        return _serverRefreshTime > refreshTime ? _serverRefreshTime : refreshTime;
    }
    

    private void _processFile(string filePath)
    {
        try
        {
            // Use this instead of file.ReadAllText() because it has FileShare settings.
            _readFileContent(filePath, out var content);
            if (content.Length < 2)
                return;
            NetDataPacket packet = JsonConvert.DeserializeObject<NetDataPacket>(content);
            _processTextEntry(packet);

        }
        catch (Exception e)
        {
            //Log.Error($"Could not read or deserialize file '{filePath}'. Exception {e}");
        }
    }

    private void _readFileContent(string filePath, out string content)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        content = sr.ReadToEnd();
        sr.Close();
        fs.Close();
    }

    private void _processTextEntry(NetDataPacket packet)
    { 
        try
        {
            if(packet.Epoch + UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                Log.Debug($"Server {packet.Port} cannot be refreshed, as it hasn't refreshed in 5 seconds.");
                return;
            }
            _updateRefreshTime(packet.RefreshSpeed);
            ProcessStats(packet);
            
        }
        catch (Exception e)
        {
            Log.Error($"could not parse structs.\n{e}");
        }
    }

    private void _updateRefreshTime(float updateTime)
    {
        if (Math.Abs(updateTime - refreshTime) > .1f)
        {
            Log.Debug($"Updated Refresh Speed.");
            refreshTime = updateTime;
        }
    }

    private void ProcessStats(NetDataPacket packet)
    {
        Log.Debug($"Received Stats Data");
        UpdateProcessor.Singleton!.ProcessUpdate(packet);
    }
}