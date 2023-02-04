// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         DataSet.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 6:10 PM
// -----------------------------------------

namespace NetDataSL.API.Structs;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.API.Members;

/// <summary>
/// A dataset for updating and adding data..
/// </summary>
public struct DataSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataSet"/> struct.
    /// </summary>
    /// <param name="dimension">The dimension to update.</param>
    public DataSet(Dimension dimension)
    {
        this.DimensionId = dimension.Id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSet"/> struct.
    /// </summary>
    /// <param name="dimensionId">The id of the dimension to update.</param>
    public DataSet(string dimensionId)
    {
        this.DimensionId = dimensionId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSet"/> struct.
    /// </summary>
    /// <param name="dimension">The dimension to update.</param>
    /// <param name="value">The value to update. Specifically for percentages.</param>
    public DataSet(Dimension dimension, float value)
    {
        this.DimensionId = dimension.Id;
        this.Value = (int)(value * 1000);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSet"/> struct.
    /// </summary>
    /// <param name="dimensionId">The id of the dimension to update.</param>
    /// <param name="value">The value to update. Specifically for percentages.</param>
    public DataSet(string dimensionId, float value)
    {
        this.DimensionId = dimensionId;
        this.Value = (int)(value * 1000);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSet"/> struct.
    /// </summary>
    /// <param name="dimension">The dimension to update.</param>
    /// <param name="value">The value to update. Specifically for normal values (no percentages).</param>
    public DataSet(Dimension dimension, int value)
    {
        this.DimensionId = dimension.Id;
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSet"/> struct.
    /// </summary>
    /// <param name="dimensionId">The id of the dimension to update.</param>
    /// <param name="value">The value to update. Specifically for normal values (no percentages).</param>
    public DataSet(string dimensionId, int value)
    {
        this.DimensionId = dimensionId;
        this.Value = value;
    }

    /// <summary>
    /// Gets the id of the dimension to update.
    /// </summary>
    public string DimensionId { get; }

    /// <summary>
    /// Gets the value to update.
    /// </summary>
    public int? Value { get; }
}