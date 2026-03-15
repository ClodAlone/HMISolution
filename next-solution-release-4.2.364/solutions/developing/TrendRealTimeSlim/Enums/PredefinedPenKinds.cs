using System;
using System.ComponentModel;
using TrendRealTimeSlim.Converters;

namespace TrendRealTimeSlim
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum PredefinedPenKinds
    {
        seriesLineStyle = 0,
        seriesStepLineStyle = 1,
        seriesColumnStyle = 2,
        seriesScatterStyle = 3,
        seriesAreaStyle = 4
    }
}
