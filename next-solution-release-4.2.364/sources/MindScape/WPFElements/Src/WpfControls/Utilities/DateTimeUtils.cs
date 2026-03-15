using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal static class DateTimeUtils
  {
    internal static bool SameMonth(DateTime date1, DateTime date2)
    {
      return (date1.Month == date2.Month) && (date1.Year == date2.Year);
    }

    public static DateTime StartOfWeek(this DateTime date, DayOfWeek firstDayOfWeek)
    {
      DateTime result = date;
      if (date.DayOfWeek == firstDayOfWeek)
      {
        result = date;
      }
      else
      {
        int dateIndex = DayOfWeekIndex(date.DayOfWeek);
        int startIndex = DayOfWeekIndex(firstDayOfWeek);
        int diff = startIndex - dateIndex;
        if (diff < 0)
        {
          result = date.AddDays(diff);
        }
        else
        {
          result = date.AddDays(-(7 - diff));
        }
      }
      return result;
    }

    private static int DayOfWeekIndex(DayOfWeek day)
    {
      switch (day)
      {
        case DayOfWeek.Monday: return 0;
        case DayOfWeek.Tuesday: return 1;
        case DayOfWeek.Wednesday: return 2;
        case DayOfWeek.Thursday: return 3;
        case DayOfWeek.Friday: return 4;
        case DayOfWeek.Saturday: return 5;
        case DayOfWeek.Sunday: return 6;
        default: throw new ArgumentException("Invalid day of week");
      }
    }

    // the reverse of the DayOfWeekIndex method.
    private static DayOfWeek GetDayOfWeek(int index)
    {
      switch (index)
      {
        case 0: return DayOfWeek.Monday;
        case 1: return DayOfWeek.Tuesday;
        case 2: return DayOfWeek.Wednesday;
        case 3: return DayOfWeek.Thursday;
        case 4: return DayOfWeek.Friday;
        case 5: return DayOfWeek.Saturday;
        case 6: return DayOfWeek.Sunday;
        default: throw new ArgumentException("Index out of range");
      }
    }

    public static DateTime EndOfWeek(this DateTime date, DayOfWeek firstDayOfWeek)
    {
      return date.StartOfWeek(firstDayOfWeek).AddDays(6);
    }

    public static DayOfWeek LastDayOfWeek(DayOfWeek firstDayOfWeek)
    {
      int index = DayOfWeekIndex(firstDayOfWeek);
      index = (index + 6) % 7;
      return GetDayOfWeek(index);
    }

    public static DateTime StartOfMonth(this DateTime date)
    {
      return date.AddDays(-(date.Day - 1));
    }

    public static DateTime EndOfMonth(this DateTime date)
    {
      return date.StartOfMonth().AddMonths(1).AddDays(-1);
    }

    public static int WeekOfYear(this DateTime dateTime)
    {
      int week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);
      int weekOfNextYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime.EndOfWeek(DayOfWeek.Monday), CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);
      if (week != weekOfNextYear)
      {
        week = 1;
      }
      return week;
    }

    public static bool IsSameDay(this DateTime time1, DateTime time2)
    {
      return time1.Date == time2.Date;
    }

    public static bool Contains(DateTime startTime, DateTime endTime, DateTime date)
    {
      DateTime start = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
      DateTime end = new DateTime(endTime.Year, endTime.Month, endTime.Day, 23, 59, 59);
      return date >= start && date <= end;
    }

    //TODO: this implementation is very basic...
    public static double DurationInHours(DateTime startTime, DateTime endTime)
    {
      return (endTime - startTime).TotalHours;
    }

    //TODO: this implementation is very basic...
    public static int DurationInMinutes(DateTime date1, DateTime date2)
    {
      double minutes = (date2 - date1).TotalMinutes;
      return (int)minutes;
    }
    public static int DaysBetween(DayOfWeek firstDay, DayOfWeek lastDay)
    {
      DateTime date = new DateTime();
      while (date.DayOfWeek != firstDay)
      {
        date = date.AddDays(1);
      }
      int count = 0;
      while (date.DayOfWeek != lastDay)
      {
        date = date.AddDays(1);
        count++;
      }
      return count;
    }

    public static int MonthsBetween(DateTime firstDate, DateTime lastDate)
    {
      int firstMonth = firstDate.Month;
      int lastMonth = lastDate.Month;
      int firstYear = firstDate.Year;
      int lastYear = lastDate.Year;
      int yearDiff = lastYear - firstYear;

      int months = lastMonth - firstMonth + 12 * yearDiff;
      return months;
    }

    // First Monday, third Wednesday, etc.
    public static int GetDayOfMonth(int year, int month, Occurrence which, DayOfWeek day)
    {
      DateTime date = new DateTime(year, month, 1);
      if (date.DayOfWeek != day)
      {
        int diff = DateTimeUtils.DaysBetween(date.DayOfWeek, day);
        date = date.AddDays(diff);
      }

      if (which == Occurrence.Last)
      {
        int daysToAdd = (date.Day + 28 <= DateTime.DaysInMonth(year, month) ? 28 : 21);
        return date.Day + daysToAdd;
      }
      else
      {
        int nth = GetNthFromOccurrence(which);
        return date.AddDays(7 * (nth - 1)).Day;
      }
    }

    private static int GetNthFromOccurrence(Occurrence occurrence)
    {
      Debug.Assert(occurrence != Occurrence.Last, "GetNthFromOccurrence can be called only with 'nth' occurrences");

      switch (occurrence)
      {
        case Occurrence.First: return 1;
        case Occurrence.Second: return 2;
        case Occurrence.Third: return 3;
        case Occurrence.Fourth: return 4;
      }
      throw new ArgumentException("occurrence");
    }

    public static int GetNthDayOfMonth(int year, int month, Occurrence which)
    {
      switch (which)
      {
        case Occurrence.First: return 1;
        case Occurrence.Second: return 2;
        case Occurrence.Third: return 3;
        case Occurrence.Fourth: return 4;
        case Occurrence.Last: return DateTime.DaysInMonth(year, month);
        default: throw new ArgumentException("Invalid Occurrence enum value " + (int)which, "which");
      }
    }

    // First weekday, last weekday, etc.
    public static int GetNthWeekdayOfMonth(int year, int month, Occurrence which)
    {
      if (which == Occurrence.Last)
      {
        return GetLastWeekdayOfMonth(year, month).Day;
      }
      else
      {
        int nth = GetNthFromOccurrence(which);
        DateTime date = GetFirstWeekdayOfMonth(year, month);
        for (int count = 1; count < nth; ++count)
        {
          date = GetNextWeekday(date);
        }
        return date.Day;
      }
    }

    private static DateTime GetFirstWeekdayOfMonth(int year, int month)
    {
      DateTime firstDayOfMonth = new DateTime(year, month, 1);
      switch (firstDayOfMonth.DayOfWeek)
      {
        case DayOfWeek.Saturday: return firstDayOfMonth.AddDays(2);
        case DayOfWeek.Sunday: return firstDayOfMonth.AddDays(1);
        default: return firstDayOfMonth;
      }
    }

    private static DateTime GetNextWeekday(DateTime after)
    {
      switch (after.DayOfWeek)
      {
        case DayOfWeek.Friday: return after.AddDays(3);
        case DayOfWeek.Saturday: return after.AddDays(2);
        default: return after.AddDays(1);
      }
    }

    private static DateTime GetLastWeekdayOfMonth(int year, int month)
    {
      DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
      switch (lastDayOfMonth.DayOfWeek)
      {
        case DayOfWeek.Saturday: return lastDayOfMonth.AddDays(-1);
        case DayOfWeek.Sunday: return lastDayOfMonth.AddDays(-2);
        default: return lastDayOfMonth;
      }
    }

    private static DateTime GetFirstWeekendDayOfMonth(int year, int month)
    {
      DateTime firstDayOfMonth = new DateTime(year, month, 1);
      switch (firstDayOfMonth.DayOfWeek)
      {
        case DayOfWeek.Monday: return firstDayOfMonth.AddDays(5);
        case DayOfWeek.Tuesday: return firstDayOfMonth.AddDays(4);
        case DayOfWeek.Wednesday: return firstDayOfMonth.AddDays(3);
        case DayOfWeek.Thursday: return firstDayOfMonth.AddDays(2);
        case DayOfWeek.Friday: return firstDayOfMonth.AddDays(1);
        default: return firstDayOfMonth;
      }
    }

    private static DateTime GetNextWeekendDay(DateTime after)
    {
      switch (after.DayOfWeek)
      {
        case DayOfWeek.Sunday: return after.AddDays(6);
        case DayOfWeek.Monday: return after.AddDays(5);
        case DayOfWeek.Tuesday: return after.AddDays(4);
        case DayOfWeek.Wednesday: return after.AddDays(3);
        case DayOfWeek.Thursday: return after.AddDays(2);
        default: return after.AddDays(1);
      }
    }

    public static int GetNthWeekendDayOfMonth(int year, int month, Occurrence which)
    {
      if (which == Occurrence.Last)
      {
        return GetLastWeekendDayOfMonth(year, month).Day;
      }
      else
      {
        int nth = GetNthFromOccurrence(which);
        DateTime date = GetFirstWeekendDayOfMonth(year, month);
        for (int count = 1; count < nth; ++count)
        {
          date = GetNextWeekendDay(date);
        }
        return date.Day;
      }
    }

    private static DateTime GetLastWeekendDayOfMonth(int year, int month)
    {
      DateTime date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
      if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
      {
        date = date.AddDays(-DateTimeUtils.DaysBetween(DayOfWeek.Sunday, date.DayOfWeek));
      }
      return date;
    }

    public static DateTime RoundDown(this DateTime time, TimeSpan interval)
    {
      TimeSpan timeOfDay = time.TimeOfDay;
      double multipleOfInterval = timeOfDay.TotalSeconds / interval.TotalSeconds;
      int numberOfCompleteIntervals = (int)multipleOfInterval;
      return time.Date.AddSeconds(interval.TotalSeconds * numberOfCompleteIntervals);
    }

    public static DateTime RoundUp(this DateTime time, TimeSpan interval)
    {
      return RoundDown(time, interval).Add(interval);
    }

    public static DateTime RoundNearest(this DateTime time, TimeSpan interval)
    {
      TimeSpan timeOfDay = time.TimeOfDay;
      double multipleOfInterval = timeOfDay.TotalSeconds / interval.TotalSeconds;
      int numberOfCompleteIntervals = (int)Math.Round(multipleOfInterval, 0);
      return time.Date.AddSeconds(interval.TotalSeconds * numberOfCompleteIntervals);
    }

    public static string ConvertTimeSpanToString(TimeSpan timeSpan)
    {
      string result = "";
      if (timeSpan.TotalMilliseconds == 0 && timeSpan.TotalSeconds == 0 && timeSpan.TotalMinutes == 0)
      {
        result = "0 minutes";
      }
      else if (timeSpan.TotalDays >= 7 && timeSpan.TotalDays % 7 == 0)
      {
        result = (timeSpan.TotalDays / 7) + " week";
        if (timeSpan.TotalDays != 7)
        {
          result += "s";
        }
      }
      else if (timeSpan.TotalDays >= 1 && HasTwoOrLessDecimalSpaces(timeSpan.TotalDays))
      {
        result = timeSpan.TotalDays + " day";
        if (timeSpan.TotalDays != 1)
        {
          result += "s";
        }
      }
      else if (timeSpan.TotalHours >= 1 && HasTwoOrLessDecimalSpaces(timeSpan.TotalHours))
      {
        if (timeSpan.TotalHours == 12)
        {
          result = "0.5 days";
        }
        else
        {
          result = timeSpan.TotalHours + " hour";
          if (timeSpan.TotalHours != 1)
          {
            result += "s";
          }
        }
      }
      else if (timeSpan.TotalMinutes >= 1 && HasTwoOrLessDecimalSpaces(timeSpan.TotalMinutes))
      {
        result = timeSpan.TotalMinutes + " minute";
        if (timeSpan.TotalMinutes != 1)
        {
          result += "s";
        }
      }
      else if (timeSpan.TotalSeconds >= 1 && HasTwoOrLessDecimalSpaces(timeSpan.TotalSeconds))
      {
        result = timeSpan.TotalSeconds + " second";
        if (timeSpan.TotalSeconds != 1)
        {
          result += "s";
        }
      }
      else if (HasTwoOrLessDecimalSpaces(timeSpan.TotalMilliseconds))
      {
        result = timeSpan.TotalMilliseconds + " milliseconds";
        if (timeSpan.TotalMilliseconds != 1)
        {
          result += "s";
        }
      }
      return result;
    }

    public static TimeSpan ConvertStringToTimeSpan(string s)
    {
      TimeSpan timeSpan = new TimeSpan();
      if (s.StartsWith("0.5 day", StringComparison.CurrentCultureIgnoreCase))
      {
        timeSpan = new TimeSpan(12, 0, 0);
      }
      else
      {
        int spaceIndex = s.IndexOf(' ');

        if (spaceIndex > 0)
        {
          string numberString = s.Substring(0, spaceIndex);
          double number = -1;

          bool success = Double.TryParse(numberString, NumberStyles.Float, CultureInfo.CurrentCulture, out number);
          /*try
          {
            number = Double.Parse(numberString, CultureInfo.CurrentUICulture);
          }
          catch (FormatException) { }*/

          if (success && number >= 0 && s.Length > spaceIndex + 1)
          {
            string domainString = s.Substring(spaceIndex + 1);
            if (domainString.StartsWith("millisecond", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan(0, 0, 0, 0, (int)number);
            }
            if (domainString.StartsWith("second", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan(0, 0, 0, (int)number);
            }
            if (domainString.StartsWith("minute", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan(0, 0, (int)(number * 60));
            }
            else if (domainString.StartsWith("hour", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan(0, (int)(number * 60), 0);
            }
            else if (domainString.StartsWith("day", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan((int)(number * 24), 0, 0);
            }
            else if (domainString.StartsWith("week", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan((int)(number * 7), 0, 0, 0);
            }
            else if (domainString.StartsWith("Month", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan((int)(number * 730.5), 0, 0);
            }
            else if (domainString.StartsWith("Year", StringComparison.OrdinalIgnoreCase))
            {
              timeSpan = new TimeSpan((int)(number * 365.25), 0, 0, 0);
            }
          }
        }
      }
      return timeSpan;
    }

    private static bool HasTwoOrLessDecimalSpaces(double number)
    {
      double integer = (int)(number * 100);
      return (number * 100) / integer == 1;
    }

    #region Total time unit helpers

    public static int TotalYears(this DateTime dateTime)
    {
      return dateTime.Year;
    }

    public static int TotalMonths(this DateTime dateTime)
    {
      return (dateTime.Year - 1) * 12 + dateTime.Month - 1;
    }

    public static DateTime DateTimeFromMonths(int monthCount)
    {
      int year = (int)(monthCount / 12.0) + 1;
      int month = (int)(monthCount % 12.0) + 1;
      if (year <= 0 || month <= 0 || month > 12)
      {
        return DateTime.MinValue;
      }
      return new DateTime(year, month, 1);
    }

    public static int TotalDays(this DateTime dateTime)
    {
      TimeSpan span = new TimeSpan(dateTime.Ticks);
      return (int)span.TotalDays;
    }

    public static DateTime DateTimeFromDays(int dayCount)
    {
      return new DateTime(new TimeSpan(dayCount, 0, 0, 0).Ticks);
    }

    public static int TotalHours(this DateTime dateTime)
    {
      return (int)new TimeSpan(dateTime.Ticks).TotalHours;
    }

    public static DateTime DateTimeFromHours(int hourCount)
    {
      return new DateTime(new TimeSpan(hourCount, 0, 0).Ticks);
    }

    public static int TotalMinutes(this DateTime dateTime)
    {
      return (int)new TimeSpan(dateTime.Ticks).TotalMinutes;
    }

    public static DateTime DateTimeFromMinutes(int minuteCount)
    {
      return new DateTime(new TimeSpan(0, minuteCount, 0).Ticks);
    }

    public static long TotalSeconds(this DateTime dateTime)
    {
      return (long)new TimeSpan(dateTime.Ticks).TotalSeconds;
    }

    public static DateTime DateTimeFromSeconds(long secondCount)
    {
      return new DateTime(TimeSpan.TicksPerSecond * secondCount);
    }

    public static long TotalMilliseconds(this DateTime dateTime)
    {
      return (long)new TimeSpan(dateTime.Ticks).TotalMilliseconds;
    }

    public static DateTime DateTimeFromMilliseconds(long millisecondCount)
    {
      return new DateTime(TimeSpan.TicksPerMillisecond * millisecondCount);
    }

    #endregion // Total time unit helpers
  }
}
