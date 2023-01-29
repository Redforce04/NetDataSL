// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         FieldProcessor.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 2:16 PM
//    Created Date:     01/28/2023 2:16 PM
// -----------------------------------------

using NetDataSL.API.Enums;

namespace NetDataSL.API.Extensions;

public static class Field
{
    public static string Process(object value, FieldType field)
    {
        if (value is string &&(string)value == "")
        {
            throw new ArgumentNullException(nameof(value), "All values must not be empty.");
        }
        try
        {
            switch (field)
            {
                case FieldType.TypeId or FieldType.Type or FieldType.Id
                    or FieldType.Name or FieldType.Title or FieldType.Units
                    or FieldType.Family or FieldType.Context or FieldType.Plugin
                    or FieldType.Module:
                    return ((string)value).Replace(" ", "_");
                
                case FieldType.Priority or FieldType.Multiplier
                    or FieldType.Divisor:
                    return ((int)value).ToString();
                
                case FieldType.UpdateEvery:
                    return ((float)value).ToString("0.00");
                
                case FieldType.ChartType:
                    return ((ChartType)value).ToString().ToLower();

                case FieldType.Obsolete or FieldType.Detail or FieldType.Hidden:
                    return (bool)value ? $" \"{field.ToString().ToLower()}\"" : "";

                case FieldType.StoreFirst:
                    return (bool)value ? " \"store_first\"" : "";
                
                case FieldType.Algorithm:
                    switch ((Algorithm) value)
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
                        .Replace(".", "");
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
                        .Replace("\\","/");
                case FieldType.CLabelSource:
                    return ((int)(LabelSource)value).ToString();
                default:
                    return (string)value;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Could not process field {field}, from \'{value}\' \n{e}");
            return "";
        }
    }
}
