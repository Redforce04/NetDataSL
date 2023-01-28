// See https://aka.ms/new-console-template for more information
//dotnet publish -r win-x64 -c Release
//dotnet publish -r linux-arm64 -c Release

using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading;

float RefreshTime = 5f;
float serverRefreshTime = -1f;
string tempDirectory = Path.GetTempPath() + "PwProfiler/";
if (args.Length > 0)
    float.TryParse(args[0], out RefreshTime);

Console.WriteLine($"Starting Netdata Integration");
if (!Directory.Exists(tempDirectory))
    Directory.CreateDirectory(tempDirectory);

for (;;)
{
    TimeSpan now = DateTime.UtcNow.TimeOfDay;
    now = now.Add(new TimeSpan(0, 0, (int)UsableRefresh()));
    foreach (string filePath in Directory.GetFiles(tempDirectory))
    {
        string content;
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using var sr = new StreamReader(fs, Encoding.UTF8);
            content = sr.ReadToEnd();
            sr.Close();
            fs.Close();
        }

        foreach (string text in content.Split($"\n"))
        {
            LogDebug($"read: '{text}'");
            if (text.StartsWith("#"))
                continue;
            //each line is an entry.
            string[] info = text.Split(" = ");
            try
            {
                switch (info[0].ToLower())
                {
                    case "refresh":
                        float time = float.Parse(info[1]);
                        if (time != RefreshTime)
                        {
                            LogDebug($"Updated Refresh Speed.");
                            RefreshTime = time;
                        }
                        break;
                    case "lowfps":
                        if(long.Parse(info[2]) + UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                            continue;
                        LowFPS lowfps = new LowFPS()
                        {
                            DateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(info[2])).DateTime,
                            Epoch = long.Parse(info[2]),
                            InstanceNumber = int.Parse(info[3]),
                            FPS = float.Parse(info[4]),
                            DeltaTime = float.Parse(info[5]),
                            Players = int.Parse(info[6])
                        };
                        ProcessLowFps(lowfps);
                        break;
                    case "stats":
                        if(long.Parse(info[2]) + UsableRefresh() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                            continue;
                        LoggingInfo loggingInfo = new LoggingInfo()
                        {
                            DateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(info[2])).DateTime,
                            Epoch = long.Parse(info[2]),
                            AverageFPS = float.Parse(info[3]),
                            AverageDeltaTime = float.Parse(info[4]),
                            MemoryUsage = long.Parse(info[5]),
                            CpuUsage = float.Parse(info[6]),
                            Players = int.Parse(info[7])
                        };
                        ProcessStats(loggingInfo);
                        break;
                }
            }
            catch (Exception e)
            {
                LogError($"could not parse structs.\n{e}");
            }
        }
            
    }

    double delay = now.Subtract(DateTime.UtcNow.TimeOfDay).TotalMilliseconds;
    if (delay < 1)
        delay = 1;
    Thread.Sleep((int)delay);
}

void LogDebug(string x)
{
    if(true)
        Console.WriteLine($"[Debug] {x}");
}

void LogError(string x) => Console.WriteLine($"[Error] {x}");
void ProcessLowFps(LowFPS lowFps)
{
    LogDebug($"Received LowFPS Data");
}

void ProcessStats(LoggingInfo loggingInfo)
{
    LogDebug($"Received Stats Data");
}
float UsableRefresh()
{
    return serverRefreshTime > RefreshTime ? serverRefreshTime : RefreshTime;
}

struct LowFPS
{
    internal DateTime DateTime;
    internal long Epoch;
    internal int InstanceNumber;
    internal float FPS;
    internal float DeltaTime;
    internal int Players;
}
struct LoggingInfo
{
    internal DateTime DateTime;
    internal long Epoch;
    internal float AverageFPS;
    internal float AverageDeltaTime;
    internal long MemoryUsage;
    internal float CpuUsage;
    internal int Players;
}