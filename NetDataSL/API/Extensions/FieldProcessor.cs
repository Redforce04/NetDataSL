// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         FieldProcessor.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 2:16 PM
// -----------------------------------------

namespace NetDataSL.API.Extensions;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.API.Enums;
using Sentry;

/// <summary>
/// The processor for field information.
/// </summary>
#pragma warning disable SA1649
public static class Field
#pragma warning restore SA1649
{
    /// <summary>
    /// Process a value for a netdata safe string of data.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <param name="field">The type of field the value is.</param>
    /// <returns>The netdata safe string of data.</returns>
    /// <exception cref="ArgumentNullException">If the argument is invalid or empty this is thrown.</exception>
    public static string Process(object value, FieldType field)
    {
        if (value is string && (string)value == string.Empty)
        {
            throw new ArgumentNullException(nameof(value), "All values must not be empty.");
        }

        try
        {
            switch (field)
            {
                case FieldType.Name or FieldType.Title or FieldType.Units
                    or FieldType.Family or FieldType.Plugin or FieldType.Module
                    or FieldType.Context or FieldType.TypeId or FieldType.Type or FieldType.Id:
                    return (string)value;

                case FieldType.Priority or FieldType.Multiplier
                    or FieldType.Divisor:
                    return ((int)value).ToString();

                case FieldType.UpdateEvery:
                    return ((float)value).ToString("0");

                case FieldType.ChartType:
                    return ((ChartType)value).ToString().ToLower();

                case FieldType.Obsolete or FieldType.Detail or FieldType.Hidden:
                    return (bool)value ? $" \"{field.ToString().ToLower()}\"" : string.Empty;

                case FieldType.StoreFirst:
                    return (bool)value ? " \"store_first\"" : string.Empty;

                case FieldType.Algorithm:
                    switch ((Algorithm)value)
                    {
                        case Algorithm.PercentageOfAbsoluteRow:
                            return "percentage-of-absolute-row";
                        case Algorithm.PercentageOfIncrementalRow:
                            return "percentage-of-incremental-row";
                        default:
                            return ((Algorithm)value).ToString().ToLower();
                    }

                case FieldType.Scope:
                    return ((FieldType)value).ToString().ToUpper();
                case FieldType.VariableName:
                    return ((string)value).Replace(" ", "_")
                        .Replace("-", ".")
                        .Replace("=", ".");
                case FieldType.Value:
                    return ((double)value).ToString("0.00000");
                case FieldType.DimensionId:
                    return ((string)value)
                        .Replace(" ", "_");
                case FieldType.CLabelName:
                    return ((string)value)
                        .Replace("+", "_")
                        .Replace(":", "_")
                        .Replace(";", "_")
                        .Replace("=", "_")
                        .Replace(",", ".")
                        .Replace("\\", "/")
                        .Replace("@", "_")
                        .Replace(" ", "_")
                        .Replace("(", "_")
                        .Replace(")", "_");
                case FieldType.CLabelValue:
                    return ((string)value).ToLower()
                        .Replace(";", ":")
                        .Replace("=", ":")
                        .Replace(",", ".")
                        .Replace("\\", "/");
                case FieldType.CLabelSource:
                    return ((int)(LabelSource)value).ToString();
                default:
                    return (string)value;
            }
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            Log.Error($"Could not process field {field}, from \'{value}\' \n{e}");
            return string.Empty;
        }
    }
}
