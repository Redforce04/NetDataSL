// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Field.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 2:13 PM
// -----------------------------------------

namespace NetDataSL.API.Enums;

/// <summary>
/// The type of field for the field processor.
/// </summary>
#pragma warning disable SA1649, SA1602
public enum FieldType
{
    TypeId,
    Type,
    Name,
    VariableName,
    Id,
    DimensionId,
    Title,
    Units,
    Family,
    Context,
    ChartType,
    Priority,
    UpdateEvery,
    Obsolete,
    Detail,
    StoreFirst,
    Hidden,
    Plugin,
    Module,
    Algorithm,
    Multiplier,
    Divisor,
    Value,
    Scope,
    CLabelName,
    CLabelValue,
    CLabelSource,
}
#pragma warning restore SA1649, SA1602