using System;
using System.ComponentModel;
using UFUAModel.Converters;

namespace UFUAModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DeviationType
    {
        AbsoluteValue,
        PercentOfValue,
        PercentOfRange,
        PercentOfEURange
    }
}
