// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ChartType.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 9:55 PM
// -----------------------------------------

namespace NetDataSL.API.Enums;

/// <summary>
/// The type of chart.
/// </summary>
public enum ChartType
{
    /// <summary>
    /// A line chart.
    /// </summary>
    Line = 0,

    /// <summary>
    /// The chart is an area.
    /// </summary>
    Area = 1,

    /// <summary>
    /// The chart is stacked.
    /// </summary>
    Stacked = 2,
}