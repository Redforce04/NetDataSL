// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NetDataPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/30/2023 2:08 PM
// -----------------------------------------


namespace NetDataSL.Structs;

internal class NetDataPacketHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetDataPacketHandler"/> class.
    /// </summary>
    public NetDataPacketHandler()
    {
    }

    internal int Port;
    internal string ServerName = null!;
    internal float RefreshSpeed;
    internal long Epoch;
    internal DateTime DateTime;
    internal float AverageTps;
    internal float AverageDeltaTime;
    internal long MemoryUsage;
    internal float CpuUsage;
    internal int Players;
    internal int LowTpsWarnCount;
}