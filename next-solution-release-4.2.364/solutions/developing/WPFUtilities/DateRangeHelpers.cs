using System;
using System.Runtime.Serialization;


namespace WPFUtilities
{
    public enum HorizontalComparisonAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }

    public enum DateSpan
    {
        None = -1,
        All = 0,
        Minute = 1,
        Hour = 2,
        Day = 3,
        Week = 4,
        Month = 5,
        Year = 6
    }

    public static class DateRangeHelpers
    {
        public static void SetStartDate(out DateTime startDate, DateTime endDate, DateSpan rangeType)
        {
            startDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            try
            {
                switch (rangeType)
                {
                    case DateSpan.Minute:
                        startDate = endDate.AddMinutes(-1);
                        break;
                    case DateSpan.Hour:
                        startDate = endDate.AddHours(-1);
                        break;
                    case DateSpan.Day:
                        startDate = endDate.AddDays(-1);
                        break;
                    case DateSpan.Week:
                        startDate = endDate.AddDays(-7);
                        break;
                    case DateSpan.Month:
                        startDate = endDate.AddMonths(-1);
                        break;
                    case DateSpan.Year:
                        startDate = endDate.AddYears(-1);
                        break;
                }
            }
            catch { }
        }

        public static void SetDateRange(out DateTime date1, out DateTime date2, DateTime startDate, DateSpan rangeType, bool useAbsoluteRange = true)
        {
            date1 = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            date2 = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            try
            {
                switch (rangeType)
                {
                    //case DateSpan.Custom:
                    //    if (customTimeFrame != null)
                    //    {
                    //        long timeframeTicks_rounded = (long)Math.Round((double)(((TimeSpan)customTimeFrame).Ticks / TimeSpan.TicksPerSecond)) * TimeSpan.TicksPerSecond;
                    //        long nowTicks_rounded = (long)Math.Round((double)(DateTime.Now.Ticks / TimeSpan.TicksPerSecond)) * TimeSpan.TicksPerSecond;
                    //        long nowMinusTicks_rounded = nowTicks_rounded - timeframeTicks_rounded;
                    //        date1 = new DateTime(nowMinusTicks_rounded);
                    //        date2 = new DateTime(nowTicks_rounded);
                    //    }
                    //    break;
                    case DateSpan.Minute:
                        if (useAbsoluteRange)
                        {
                            date1 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date2 = date1.AddMinutes(1);
                        }
                        else
                        {
                            date2 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date1 = date2.AddMinutes(-1);
                        }
                        break;
                    case DateSpan.Hour:
                        if (useAbsoluteRange)
                        {
                            date1 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 0, 0);
                            date2 = date1.AddHours(1);
                        }
                        else
                        {
                            date2 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date1 = date2.AddHours(-1);
                        }
                        break;
                    case DateSpan.Day:
                        if (useAbsoluteRange)
                        {
                            date1 = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                            date2 = date1.AddHours(24);
                        }
                        else
                        {
                            date2 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date1 = date2.AddHours(-24);
                        }
                        break;
                    case DateSpan.Week:
                        if (useAbsoluteRange)
                        {
                            date1 = FirstDayOfWeek(new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0));
                            date2 = date1.AddDays(7);
                        }
                        else
                        {
                            date2 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date1 = date2.AddDays(-7);
                        }
                        break;
                    case DateSpan.Month:
                        if (useAbsoluteRange)
                        {
                            date1 = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);
                            date2 = date1.AddMonths(1);
                        }
                        else
                        {
                            date2 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date1 = date2.AddMonths(-1);
                        }
                        break;
                    case DateSpan.Year:
                        if (useAbsoluteRange)
                        {
                            date1 = new DateTime(startDate.Year, 1, 1, 0, 0, 0);
                            date2 = date1.AddYears(1);
                        }
                        else
                        {
                            date2 = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
                            date1 = date2.AddYears(-1);
                        }
                        break;
                }
            }
            catch { }
        }

        public static DateTime FirstDayOfWeek(DateTime date)
        {
            DateTime dt = date;
            while (dt.DayOfWeek != DayOfWeek.Monday) dt = dt.AddDays(-1);
            return dt;
        }

        public static DateTime RoundToFloorTimeSpan(DateTime dt, TimeSpan t)
        {
            return new DateTime((dt.Ticks / t.Ticks) * t.Ticks);
        }
        public static DateTime RoundToNearestTimeSpan(DateTime dt, TimeSpan t)
        {
            return new DateTime(((dt.Ticks + (t.Ticks / 2) + 1) / t.Ticks) * t.Ticks);
        }
        public static DateTime RoundToCeilingTimeSpan(DateTime dt, TimeSpan t)
        {
            return new DateTime(((dt.Ticks + t.Ticks - 1) / t.Ticks) * t.Ticks);
        }
    }

    [DataContract(Name = "TimeRange")]
    public class TimeRange
    {
        [DataMember]
        DateTime dateTimeStart = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeEnd = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeStartCompare = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeEndCompare = DateTime.MinValue;
        [DataMember]
        bool compare = false;
        [DataMember]
        HorizontalComparisonAlignment comparingAlignment;

        public DateTime DateTimeStart
        {
            get { return dateTimeStart; }
            set
            {
                if (dateTimeStart == value)
                    return;
                dateTimeStart = value;
            }
        }

        public DateTime DateTimeEnd
        {
            get { return dateTimeEnd; }
            set
            {
                if (dateTimeEnd == value)
                    return;
                dateTimeEnd = value;
            }
        }

        public bool Compare
        {
            get { return compare; }
            set
            {
                if (compare == value)
                    return;
                compare = value;
            }
        }


        public HorizontalComparisonAlignment ComparingAlignment
        {
            get { return comparingAlignment; }
            set
            {
                if (comparingAlignment == value)
                    return;
                comparingAlignment = value;
            }
        }

        public DateTime DateTimeStartCompare
        {
            get { return dateTimeStartCompare; }
            set
            {
                if (dateTimeStartCompare == value)
                    return;
                dateTimeStartCompare = value;
            }
        }

        public DateTime DateTimeEndCompare
        {
            get { return dateTimeEndCompare; }
            set
            {
                if (dateTimeEndCompare == value)
                    return;
                dateTimeEndCompare = value;
            }
        }
    }

    public static class DateRangesComparer
    {
        public static bool StartsInside(DateTime d1_start, DateTime d2_start, DateTime d2_end)
        {
            return d1_start >= d2_start && d1_start <= d2_end;
        }
        public static bool EndsInside(DateTime d1_end, DateTime d2_end, DateTime d2_start)
        {
            return d1_end <= d2_end && d1_end >= d2_start;
        }
        public static bool IsSubset(DateTime d1_start, DateTime d1_end, DateTime d2_start, DateTime d2_end)
        {
            return StartsInside(d1_start, d2_start, d2_end) && EndsInside(d1_end, d2_end, d2_start);
        }
        public static bool IsNotSubset(DateTime d1_start, DateTime d1_end, DateTime d2_start, DateTime d2_end)
        {
            return !(StartsInside(d1_start, d2_start, d2_end) && EndsInside(d1_end, d2_end, d2_start));
        }
        public static bool AreDisjointed(DateTime d1_start, DateTime d1_end, DateTime d2_start, DateTime d2_end)
        {
            return !StartsInside(d1_start, d2_start, d2_end) && !EndsInside(d1_end, d2_end, d2_start);
        }
        public static bool AreIntersecting(DateTime d1_start, DateTime d1_end, DateTime d2_start, DateTime d2_end)
        {
            return StartsInside(d1_start, d2_start, d2_end) || EndsInside(d1_end, d2_end, d2_start);
        }
    }

    public class LocalizedTimeRange
    {
        public string Content { get; set; }
        public DateSpan Value { get; set; }
        public bool IsEnabled { get; set; }
        public LocalizedTimeRange(string content, DateSpan value)
        {
            Content = content;
            Value = value;
            IsEnabled = true;
        }
        public override string ToString()
        {
            return Content;
        }
    }
}
