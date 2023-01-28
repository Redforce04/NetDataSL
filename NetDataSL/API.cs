using System.Text;
using NetDataSL.Structs;

namespace NetDataSL;

public class Plugin
{
    private static Plugin? _singleton;

    public Plugin(float refreshRate = 5f)
    {
        if (_singleton != null)
            return;
        _singleton = this;
        _refreshTime = refreshRate;
        InitNetData();
        Init();
    }

    private float _refreshTime = 5f;
    private readonly float _serverRefreshTime = -1f;
    private readonly string _tempDirectory = Path.GetTempPath() + "PwProfiler/";

    private readonly Dictionary<int, String> _servers = new Dictionary<int, string>()
    {
        { 0, "Unknown Server" },
        { 9011, "Net 1" },
        { 9012, "Net 2" },
        { 9017, "Testing Net" }
    };
    private void InitNetData()
    {
        
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
            TimeSpan now = DateTime.UtcNow.TimeOfDay;
            now = now.Add(new TimeSpan(0, 0, (int)UsableRefresh()));
            // Get the delay necessary.
            foreach (string filePath in Directory.GetFiles(_tempDirectory))
            {
                _processFile(filePath);
            }

            double delay = now.Subtract(DateTime.UtcNow.TimeOfDay).TotalMilliseconds;
            if (delay < 1)
                delay = 1;
            Thread.Sleep((int)delay);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    float UsableRefresh()
    {
        return _serverRefreshTime > _refreshTime ? _serverRefreshTime : _refreshTime;
    }
    
    private void _processFile(string filePath)
    {
        if (int.TryParse(filePath.Substring(filePath.Length - 5), out var server))
        {
            Log.Debug($"Server could not be identified for file {filePath}");
        }

        string serverName = _servers[server];
        
        _readFileContent(filePath, out string content);
        
        foreach (string text in content.Split($"\n"))
        {
            if (text.StartsWith("#"))
                continue;
            _processTextEntry(text, server, serverName);
            
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
            string[] info = text.Split(" = ");
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
                        _processStats(info, server,serverName);
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
        float time = float.Parse(info[1]);
        if (Math.Abs(time - _refreshTime) > .1f)
        {
            Log.Debug($"Updated Refresh Speed.");
            _refreshTime = time;
        }
    }

    private void _processLowFps(string[] info, int server, string serverName)
    {
        LowFps lowfps = new LowFps()
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
        LoggingInfo loggingInfo = new LoggingInfo()
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
