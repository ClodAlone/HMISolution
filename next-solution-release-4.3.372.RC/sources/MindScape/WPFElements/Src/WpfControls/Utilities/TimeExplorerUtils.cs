using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal static class TimeExplorerUtils
  {
    // This need for special logic suggests that this approach needs to be revised:
    public static TimeSpan GetTimeSpan(TimeRangeUnit unit)
    {
      switch(unit)
      {
        case TimeRangeUnit.Year:
          //return new TimeSpan(365, 0, 0, 0);
          // Year uses special logic due to leap years. The time span needs to be 1 day:
          return new TimeSpan(1, 0, 0, 0);
        case TimeRangeUnit.Month:
          //return new TimeSpan(26280000000000);
          // Month uses special logic. The time span needs to be 1 day:
          return new TimeSpan(1, 0, 0, 0);
        case TimeRangeUnit.Week:
          return new TimeSpan(7, 0, 0, 0);
        case TimeRangeUnit.Day:
          return new TimeSpan(1, 0, 0, 0);
        case TimeRangeUnit.Hour:
          return new TimeSpan(1, 0, 0);
        case TimeRangeUnit.Minute:
          return new TimeSpan(0, 1, 0);
      }
      throw new InvalidOperationException("Unknown time range unit: " + unit);
    }

    public static DateTime StartOfTimeUnit(this DateTime dateTime, TimeRangeUnit unit)
    {
      switch(unit)
      {
        case TimeRangeUnit.Year:
          return new DateTime(dateTime.Year, 1, 1);
        case TimeRangeUnit.Month:
          return dateTime.StartOfMonth();
        case TimeRangeUnit.Week:
          return dateTime.StartOfWeek(DayOfWeek.Monday);
        case TimeRangeUnit.Day:
          return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        case TimeRangeUnit.Hour:
          return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        case TimeRangeUnit.Minute:
          return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
      }
      throw new InvalidOperationException("Unknown time range unit: " + unit);
    }

    // TODO: getting the label is currently based on the width. This makes an assuption on the font size. Should this be considered?

    public static string GetLabel(TimeRangeUnit unit, int unitCount, DateTime dateTime, double width, string shortWeek, string fullWeek)
    {
      switch(unit)
      {
        case TimeRangeUnit.Year:
          return GetYearLabel(dateTime);
        case TimeRangeUnit.Month:
          return GetMonthLabel(dateTime, width);
        case TimeRangeUnit.Week:
          return GetWeekLabel(dateTime, width, shortWeek, fullWeek);
        case TimeRangeUnit.Day:
          return GetDayLabel(dateTime, width);
        case TimeRangeUnit.Hour:
          return GetHourLabel(dateTime, unitCount, width);
        case TimeRangeUnit.Minute:
          return GetMinuteLabel(dateTime, unitCount, width);
      }
      throw new InvalidOperationException("Unknown time range unit: " + unit);
    }

    public static string GetMinuteLabel(DateTime dateTime, int unitCount, double width)
    {
      string label = "";
      if (unitCount > 1)
      {
        DateTime end = dateTime.AddMinutes(unitCount);
        label = String.Format(CultureInfo.CurrentCulture, "{0:h:mm}", dateTime) + " - " + String.Format(CultureInfo.CurrentCulture, "{0:h:mm}", end);
      }
      else
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:hh:mm}", dateTime);
      }
      return label;
    }

    public static string GetHourLabel(DateTime dateTime, int unitCount, double width)
    {
      string label = "";
      if (unitCount > 1)
      {
        DateTime end = dateTime.AddHours(unitCount);
        label = String.Format(CultureInfo.CurrentCulture, "{0:htt}", dateTime) + " - " + String.Format(CultureInfo.CurrentCulture, "{0:htt}", end);
      }
      else
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:h:mmtt}", dateTime);
      }
      return label;
    }

    public static string GetDayLabel(DateTime dateTime, double width)
    {
      string label = "";
      if (width < 21)
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:ddd}", dateTime).Substring(0, 1);
      }
      else if (width < 44)
      {
        label = dateTime.Day.ToString(CultureInfo.CurrentCulture);
      }
      else if (width < 75)
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:ddd, d}", dateTime);
      }
      else
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:dddd, d}", dateTime);
      }
      return label;
    }

    public static string GetWeekLabel(DateTime dateTime, double width, string shortWeek, string fullWeek)
    {
      string label = "";
      int week = dateTime.WeekOfYear();
      if (width < 43)
      {
        label = shortWeek + week;
      }
      else if (width < 77)
      {
        label = fullWeek + " " + week;
      }
      else if (width < 95)
      {
        label = fullWeek + " " + week + String.Format(CultureInfo.CurrentCulture, "{0:, MMM/yy}", dateTime);
      }
      else
      {
        label = fullWeek + " " + week + String.Format(CultureInfo.CurrentCulture, "{0:, MMM/yyyy}", dateTime);
      }
      return label;
    }

    public static string GetMonthLabel(DateTime dateTime, double width)
    {
      string label = "";
      if (width < 53)
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:MMM}", dateTime);
      }
      else if (width < 77)
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:MMMM}", dateTime);
      }
      else
      {
        label = String.Format(CultureInfo.CurrentCulture, "{0:MMMM, yyyy}", dateTime);
      }
      return label;
    }

    public static string GetYearLabel(DateTime dateTime)
    {
      return String.Format(CultureInfo.CurrentCulture, "{0:yyyy}", dateTime);
    }
  }
}
