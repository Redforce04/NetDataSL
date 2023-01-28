namespace NetDataSL.Structs;


public struct LowFps
{
    internal DateTime DateTime;
    internal long Epoch;
    internal int InstanceNumber;
    internal float Fps;
    internal float DeltaTime;
    internal int Players;
    internal int Server;
    internal string ServerName;
}
public struct LoggingInfo
{
    internal DateTime DateTime;
    internal long Epoch;
    internal float AverageFps;
    internal float AverageDeltaTime;
    internal long MemoryUsage;
    internal float CpuUsage;
    internal int Players;
    internal int Server;
    internal string ServerName;
}