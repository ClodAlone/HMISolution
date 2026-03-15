using System;
using System.ComponentModel;
using UFUAModel.Converters;

namespace UFUAModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AlarmType
    {
        //ExclusiveLimit,
        //NonExclusiveLimit,
        ExclusiveLevel = 2,
        NonExclusiveLevel,
        ExclusiveDeviation,
        NonExclusiveDeviation,
        ExclusiveRateOfChange,
        NonExclusiveRateOfChange,
        TripAlarm
    }
}