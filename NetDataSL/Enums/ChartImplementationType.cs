// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ChartImplementationType.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/31/2023 12:16 PM
// -----------------------------------------

namespace NetDataSL.Enums;

/// <summary>
/// The type of chart.
/// </summary>
public enum ChartImplementationType
{
    /// <summary>
    /// The chart is the cpu chart.
    /// </summary>
    Cpu = 0,

    /// <summary>
    /// The chart is the memory chart.
    /// </summary>
    Memory = 1,

    /// <summary>
    /// The chart is the tps chart.
    /// </summary>
    Tps = 2,

    /// <summary>
    /// The chart is the lowtps chart.
    /// </summary>
    LowTps = 3,

    /// <summary>
    /// The chart is the players chart.
    /// </summary>
    Players = 4,

    /// <summary>
    /// An individual server and the associated stats.
    /// </summary>
    Server = 5,
}