// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         VariableScope.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 2:59 PM
// -----------------------------------------

namespace NetDataSL.API.Enums;

/// <summary>
/// The scope of a variable.
/// </summary>
public enum VariableScope
{
    /// <summary>
    /// The variable is non chart dependent.
    /// </summary>
    Global = 0,

    /// <summary>
    /// The variable is non chart dependent.
    /// </summary>
    Host = 0,

    /// <summary>
    /// The variable is tied to a chart.
    /// </summary>
    Local = 1,

    /// <summary>
    /// The variable is tied to a chart.
    /// </summary>
    Chart = 1,
}