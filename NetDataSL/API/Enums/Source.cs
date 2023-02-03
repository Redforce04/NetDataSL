// <copyright file="Log.cs" company="Redforce04#4091">
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

public enum LabelSource
{
    Automatically = 1,
    Manually = 2,
    K8Label = 4,
    NetDataAgentCloudLink = 8,
}