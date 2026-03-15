using System;
using System.ComponentModel;
using UFUAModel.Converters;

namespace UFUAModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ConditionType
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        NotEqual,
        Between
    }
}