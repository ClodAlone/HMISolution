using System;
using System.ComponentModel;
using Trends.Converters;

namespace Trends
{
    [TypeConverter(typeof(LocalizedPenKindsEnumConverter))]
    public enum PredefinedPenKinds
    {
        seriesLineStyle = 0,
        seriesStepLineStyle = 1,
        seriesColumnStyle = 2,
        seriesScatterStyle = 3,
        seriesAreaStyle = 4
    }
}
