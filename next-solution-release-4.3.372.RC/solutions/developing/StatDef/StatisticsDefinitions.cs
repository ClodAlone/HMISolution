using System;
using System.Collections.Generic;
using System.ComponentModel;
using StatDef.Converters;

namespace StatDef
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum StatProps
    {
        None = 0,
        Min = 1,
        Max = 2,
        Average = 3,
        NumUpdates = 4,
        TotalTimeOn = 5
    }

    public static partial class StatisticsDefinitions
    {
        public static Dictionary<StatProps, string> statParams = new Dictionary<StatProps, string> {
            { StatProps.Min, "Min" },
            { StatProps.Max, "Max" },
            { StatProps.Average, "Average" },
            { StatProps.NumUpdates, "NumUpdates" },
            { StatProps.TotalTimeOn, "TotalTimeOn" }
        };
        public static Dictionary<StatProps, Func<object, object>> statFormats = new Dictionary<StatProps, Func<object, object>>
        {
            { StatProps.TotalTimeOn, value => TimeSpan.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture).ToString("g") }
        };
    }
}
