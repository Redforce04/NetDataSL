// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         LoggingInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/28/2023 1:20 PM
// -----------------------------------------

namespace NetDataSL.Structs;

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