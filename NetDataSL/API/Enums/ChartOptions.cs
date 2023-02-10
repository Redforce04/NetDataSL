// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ChartOptions.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 4:31 PM
// -----------------------------------------

namespace NetDataSL.API.Enums;

/// <summary>
/// The options a chart can have.
/// </summary>
[Flags]
public enum ChartOptions
{
    /// <summary>
    /// No options specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// The chart is marked obsolete and will not show up in the panel.
    /// </summary>
    Obsolete = 1,

    /// <summary>
    /// Changes the level of detail in Netdata.
    /// </summary>
    Detail = 2,

    /// <summary>
    /// Stores the first value for comparison.
    /// </summary>
    StoreFirst = 4,

    /// <summary>
    /// The chart will not show up in netdata but will be recorded to the db.
    /// </summary>
    Hidden = 8,
}