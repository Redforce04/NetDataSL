// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Data.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 6:10 PM
//    Created Date:     01/28/2023 6:10 PM
// -----------------------------------------

using NetDataSL.API.Members;

namespace NetDataSL.API.Structs;

public struct DataSet
{
    public DataSet(Dimension dimension)
    {
        DimensionId = dimension.Id;
    }

    public DataSet(string dimensionId)
    {
        DimensionId = dimensionId;
    }
    
    public DataSet(Dimension dimension, float value)
    {
        DimensionId = dimension.Id;
        Value = (int)(value * 1000);
    }
    public DataSet(string dimensionId, float value)
    {
        DimensionId = dimensionId;
        Value = (int)(value * 1000);
    }
    public DataSet(Dimension dimension, int value)
    {
        DimensionId = dimension.Id;
        Value = value;
    }
    public DataSet(string dimensionId, int value)
    {
        DimensionId = dimensionId;
        Value = value;
    }
    public string DimensionId { get; }
    public int? Value { get; }
}