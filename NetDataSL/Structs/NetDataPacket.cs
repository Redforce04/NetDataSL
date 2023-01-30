// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NetDataPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 2:08 PM
//    Created Date:     01/30/2023 2:08 PM
// -----------------------------------------

namespace NetDataSL.Structs;

internal struct NetDataPacket
{
    internal int Port;
    internal string ServerName;
    internal float RefreshSpeed;
    internal long Epoch;
    internal float AverageTps;
    internal float AverageDeltaTime;
    internal long MemoryUsage;
    internal float CpuUsage;
    internal int Players;
    internal int LowTpsWarnCount;
}