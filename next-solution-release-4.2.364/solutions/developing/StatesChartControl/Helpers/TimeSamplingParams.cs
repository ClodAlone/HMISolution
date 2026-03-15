using System;
using System.Collections.Generic;

namespace StatesChartControl.Helpers
{
    public enum RDateSpan
    {
        Minute = 1,
        Hour = 2,
        Day = 3,
        Week = 4,
        Month = 5
    }
    public static class TimeSamplingParams
    {
        public static readonly Dictionary<RDateSpan, TimeSpan> dateSpanViewTimeFrame = new Dictionary<RDateSpan, TimeSpan> { //Total view time frame
            { RDateSpan.Minute, new TimeSpan(0,1,0) },
            { RDateSpan.Hour, new TimeSpan(1,0,0) },
            { RDateSpan.Day, new TimeSpan(1,0,0,0) },
            { RDateSpan.Week, new TimeSpan(7,0,0,0) },
            { RDateSpan.Month, new TimeSpan(30,0,0,0)}
        };
        public static readonly Dictionary<RDateSpan, TimeSpan> samplingUnit = new Dictionary<RDateSpan, TimeSpan> { //Cell's unit of measure
            { RDateSpan.Minute, new TimeSpan(0,0,1) },
            { RDateSpan.Hour, new TimeSpan(0,1,0) },
            { RDateSpan.Day, new TimeSpan(0,15,0) },
            { RDateSpan.Week, new TimeSpan(4,0,0) },
            { RDateSpan.Month, new TimeSpan(12,0,0) },
        };
        public static readonly Dictionary<RDateSpan, TimeSpan> dateRangeRoundingUnit = new Dictionary<RDateSpan, TimeSpan> { //Applied on every start/enddate changes: custom date range fetch, relative ranges, ... (prev/next buttons will work on exact intervals)
            { RDateSpan.Minute, new TimeSpan(0,0,1) },
            { RDateSpan.Hour, new TimeSpan(0,1,0) },
            { RDateSpan.Day, new TimeSpan(0,30,0) },
            { RDateSpan.Week, new TimeSpan(1,0,0,0) },
            { RDateSpan.Month, new TimeSpan(1,0,0,0) },
        };
        public static readonly Dictionary<RDateSpan, TimeSpan> timeRangeMinTimeMultiple = new Dictionary<RDateSpan, TimeSpan> //Minimum time interval at which the time label can be printed
        {
            { RDateSpan.Month, new TimeSpan(1,0,0,0) }, //Don't want to show the time labels between two days
            { RDateSpan.Week, new TimeSpan(1,0,0,0) },
            { RDateSpan.Day, new TimeSpan(0,30,0) },
            { RDateSpan.Hour, new TimeSpan(0,5,0) },
            { RDateSpan.Minute, new TimeSpan(0,0,5) }
        };
        public static int GetTimeRangeMinColsMultiple(RDateSpan timeSpan) //Minimum number of columns at which the time label can be printed
        {
            return (int)(timeRangeMinTimeMultiple[timeSpan].Ticks / samplingUnit[timeSpan].Ticks);
        }
    }
}
