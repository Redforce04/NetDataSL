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

[Flags]
public enum ChartOptions
{
    None = 0,
    Obsolete = 1,
    Detail = 2, 
    StoreFirst = 4,
    Hidden = 8
}