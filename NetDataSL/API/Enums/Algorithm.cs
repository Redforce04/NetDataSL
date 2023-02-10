// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Algorithm.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 1:36 PM
// -----------------------------------------

namespace NetDataSL.API.Enums;

/// <summary>
/// The types of algorithms possible for dimensions.
/// </summary>
public enum Algorithm
{
    /// <summary>
    /// An absolute algorithm.
    /// </summary>
    Absolute = 0,

    /// <summary>
    /// An incremental algorithm.
    /// </summary>
    Incremental = 1,

    /// <summary>
    /// The algorithm is a percentage of an absolute row.
    /// </summary>
    PercentageOfAbsoluteRow = 2,

    /// <summary>
    /// The algorithm is a percentage of an incremental row.
    /// </summary>
    PercentageOfIncrementalRow = 3,
}