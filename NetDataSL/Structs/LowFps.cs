// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         LowFps.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:30 PM
// -----------------------------------------

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