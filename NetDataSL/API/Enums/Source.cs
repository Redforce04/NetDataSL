﻿// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Source.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 6:32 PM
// -----------------------------------------

namespace NetDataSL.API.Enums;

#pragma warning disable SA1649

/// <summary>
/// The type of label.
/// </summary>
public enum LabelSource
{
    /// <summary>
    /// The label was automatically generated by Netdata.
    /// </summary>
    Automatically = 1,

    /// <summary>
    /// The label was manually generated by a plugin.
    /// </summary>
    Manually = 2,

    /// <summary>
    /// The label was a K8 label.
    /// </summary>
    K8Label = 4,

    /// <summary>
    /// The label was a netdata cloud label.
    /// </summary>
    NetDataAgentCloudLink = 8,
}
#pragma warning restore SA1649
