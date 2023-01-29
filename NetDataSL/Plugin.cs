// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:23 PM
// -----------------------------------------

using System.Text;
using NetDataSL.Structs;

namespace NetDataSL;

public class Plugin
{
    public static Plugin? Singleton;

    public Plugin(float refreshRate = 5f)
    {
        if (Singleton != null)
            return;
        Singleton = this;
        _refreshTime = refreshRate;
        InitNetDataIntegration();
        Init();
    }

    public readonly string PluginName = "scpsl.integration";
    private float _refreshTime = 5f;
    private readonly float _serverRefreshTime = -1f;
    private readonly string _tempDirectory = Path.GetTempPath() + "PwProfiler/";

    private readonly Dictionary<int, string> _servers = new()
    {
        { 0, "Unknown Server" },
        { 9011, "Net 1" },
        { 9012, "Net 2" },
        { 9017, "Testing Net" }
    };

    private void InitNetDataIntegration()
    {
        _getServers(out var servers);
        var unused = new ChartIntegration(servers);
    }

    private void _getServers(out List<KeyValuePair<int, string>> servers)
    {
        servers = new List<KeyValuePair<int, string>>();
        foreach (var filePath in Directory.GetFiles(_tempDirectory))
        {
            _getServerInfoFromName(filePath, out var server);
            servers.Add(server);
        }
    }

    private void Init()
    {
        Log.Debug($"Starting Net-data Integration");
        _createDirectories();
        _startMainListenLoop();
    }

    private void _createDirectories()
    {
        if (!Directory.Exists(_tempDirectory))
            Directory.CreateDirectory(_tempDirectory);
    }

    private void _startMainListenLoop()
    {
        for (;;)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            now = now.Add(new TimeSpan(0, 0, (int)UsableRefresh()));
            // Get the delay necessary.
            foreach (var filePath in Directory.GetFiles(_tempDirectory)) _processFile(filePath);

            var delay = now.Subtract(DateTime.UtcNow.TimeOfDay).TotalMilliseconds;
            if (delay < 1)
                delay = 1;
            Thread.Sleep((int)delay);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private float UsableRefresh()
    {
        return _serverRefreshTime > _refreshTime ? _serverRefreshTime : _refreshTime;
    }

    private void _getServerInfoFromName(string serverString, out KeyValuePair<int, string> server)
    {
        if (!int.TryParse(serverString.Substring(serverString.Length - 4), out var serverPort))
        {
            Log.Debug($"Server could not be identified for file {serverString}");
            serverPort = 0;
        }

        server = !_servers.ContainsKey(serverPort)
            ? new KeyValuePair<int, string>(serverPort, "unknown")
            : new KeyValuePair<int, string>(serverPort, _servers[serverPort]);
    }

    private void _processFile(string filePath)
    {
        _getServerInfoFromName(filePath, out var server);

        _readFileContent(filePath, out var content);

        foreach (var text in content.Split($"\n"))
        {
            if (text.StartsWith("#"))
                continue;
            _processTextEntry(text, server.Key, server.Value);
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

    private void _processTextEntry(string text, int server, string serverName)
    {
        //LogDebug($"read: '{text}'");
        // each line is an entry.
        var info = text.Split(" = ");
        try
        {
            switch (info[0].ToLower())
            {
                case "refresh":
                    _updateRefreshTime(info);
                    break;
                case "lowfps":
                    if (long.Parse(info[2]) + UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        return;
                    _processLowFps(info, server, serverName);
                    break;
                case "stats":
                    if (long.Parse(info[2]) + UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        return;
                    _processStats(info, server, serverName);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error($"could not parse structs.\n{e}");
        }
    }

    private void _updateRefreshTime(string[] info)
    {
        var time = float.Parse(info[1]);
        if (Math.Abs(time - _refreshTime) > .1f)
        {
            Log.Debug($"Updated Refresh Speed.");
            _refreshTime = time;
        }
    }

    private void _processLowFps(string[] info, int server, string serverName)
    {
        var lowfps = new LowFps()
        {
            DateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(info[2])).DateTime,
            Epoch = long.Parse(info[2]),
            InstanceNumber = int.Parse(info[3]),
            Fps = float.Parse(info[4]),
            DeltaTime = float.Parse(info[5]),
            Players = int.Parse(info[6]),
            Server = server,
            ServerName = serverName
        };
        ProcessLowFps(lowfps);
    }

    private void _processStats(string[] info, int server, string serverName)
    {
        var loggingInfo = new LoggingInfo()
        {
            DateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(info[2])).DateTime,
            Epoch = long.Parse(info[2]),
            AverageFps = float.Parse(info[3]),
            AverageDeltaTime = float.Parse(info[4]),
            MemoryUsage = long.Parse(info[5]),
            CpuUsage = float.Parse(info[6]),
            Players = int.Parse(info[7]),
            Server = server,
            ServerName = serverName
        };
        ProcessStats(loggingInfo);
    }

    private void ProcessLowFps(LowFps lowFps)
    {
        Log.Debug($"Received LowFPS Data");
    }

    private void ProcessStats(LoggingInfo loggingInfo)
    {
        Log.Debug($"Received Stats Data");
    }
}