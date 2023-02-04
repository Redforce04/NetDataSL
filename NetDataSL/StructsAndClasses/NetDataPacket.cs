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

public class NetDataPacketHandler
{
#pragma warning disable SA1401

    /// <summary>
    /// The port of the sending server.
    /// </summary>
    public int Port;

    /// <summary>
    /// The name of the sending server.
    /// </summary>
    public string ServerName = null!;

    /// <summary>
    /// The refresh speed of the sending server.
    /// </summary>
    public float RefreshSpeed;

    /// <summary>
    /// The epoch of when this packet was made.
    /// </summary>
    public long Epoch;

    /// <summary>
    /// The datetime of when this packet was made.
    /// </summary>
    public DateTime DateTime;

    /// <summary>
    /// The average tps of when this packet was made.
    /// </summary>
    public float AverageTps;

    /// <summary>
    /// The average delta time of when this packet was made.
    /// </summary>
    public float AverageDeltaTime;

    /// <summary>
    /// The memory usage at the time this packet was made.
    /// </summary>
    public long MemoryUsage;

    /// <summary>
    /// The cpu usage at the time this packet was made.
    /// </summary>
    public float CpuUsage;

    /// <summary>
    /// The amount of players on the server at the time this packet was made.
    /// </summary>
    public int Players;

    /// <summary>
    /// How many low tps warnings have gone off since the last time packets were collected.
    /// </summary>
    public int LowTpsWarnCount;
#pragma warning restore SA1401
    /// <summary>
    /// Initializes a new instance of the <see cref="NetDataPacketHandler"/> class.
    /// </summary>
    public NetDataPacketHandler()
    {
    }


}
