// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:23 PM
// -----------------------------------------

using System.Reflection;
using System.Text;
using NetDataSL.Structs;
using Newtonsoft.Json;
using Sentry;

namespace NetDataSL;

public class Plugin
{
    public static Plugin? Singleton;

    public Plugin(float refreshRate = 5f)
    {
        if (Singleton != null)
            return;
        Singleton = this;
        var unused = new Log();
        _refreshTime = refreshRate;
        
        InitNetDataIntegration();
        Init();
    }

    public readonly string PluginName = "scpsl.plugin";
    private float _refreshTime = 5f;
    private readonly float _serverRefreshTime = -1f;
    private readonly string _tempDirectory = Path.GetTempPath() + "PwProfiler/";
    
    /// Todo Make servers autoinitialize port and name via the refresh data transfer method 
    public readonly Dictionary<int, string> Servers = new()
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
        var unused2 = new UpdateProcessor();
    }

    private void _getServers(out List<KeyValuePair<int, string>> servers)
    {
        servers = new List<KeyValuePair<int, string>>();
        foreach (var filePath in Directory.GetFiles(_tempDirectory))
        {
            _readFileContent(filePath, out var content);
            if (content.Length < 2)
                continue;
            try
            {
                NetDataPacket packet = JsonConvert.DeserializeObject<NetDataPacket>(content);
                servers.Add(new KeyValuePair<int, string>(packet.Port, packet.ServerName));
            }
            catch (Exception e)
            {
                //Log.Error($"Could not deserialize content of File {filePath}. {e}");
                continue;
            }
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
        return _serverRefreshTime > _refreshTime ? _serverRefreshTime : _refreshTime;
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
            packet.DateTime = DateTimeOffset.FromUnixTimeSeconds(packet.Epoch).DateTime;
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
        if (Math.Abs(updateTime - _refreshTime) > .1f)
        {
            Log.Debug($"Updated Refresh Speed.");
            _refreshTime = updateTime;
        }
    }

    private void ProcessStats(NetDataPacket packet)
    {
        Log.Debug($"Received Stats Data");
        UpdateProcessor.Singleton!.ProcessUpdate(packet);
    }
}