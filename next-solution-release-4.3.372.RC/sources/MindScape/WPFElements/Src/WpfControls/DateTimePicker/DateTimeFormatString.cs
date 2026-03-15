using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal static class DateTimeFormatString
  {
    private static readonly string[] _isSelectPrefixes = new string[]
    {
      "MMM",
      "t"
    };

    internal static bool IsSelect(string formatString)
    {
      foreach (string prefix in _isSelectPrefixes)
      {
        if (formatString.StartsWith(prefix, StringComparison.Ordinal))
        {
          return true;
        }
      }

      return false;
    }

    internal static IEnumerable<string> GetPermittedValues(string formatString, CultureInfo culture, DateTime contextValue)
    {
      if (formatString == "MMM")
      {
        return GetShortMonthNames(culture, contextValue);
      }
      else if (formatString.StartsWith("MMMM", StringComparison.Ordinal))
      {
        return GetLongMonthNames(culture, contextValue);
      }
      else if (formatString == "ddd")
      {
        return GetShortDayOfWeekNames(culture);
      }
      else if (formatString.StartsWith("dddd", StringComparison.Ordinal))
      {
        return GetLongDayOfWeekNames(culture);
      }
      else if (formatString == "t")
      {
        return GetShortDesignatorNames(culture);
      }
      else if (formatString.StartsWith("tt", StringComparison.Ordinal))
      {
        return GetLongDesignatorNames(culture);
      }

      return new string[0];
    }

    internal static IEnumerable<string> GetShortMonthNames(CultureInfo culture, DateTime contextValue)
    {
      int monthCount = culture.DateTimeFormat.Calendar.GetMonthsInYear(contextValue.Year);
      for (int month = 1; month <= monthCount; ++month)
      {
        yield return culture.DateTimeFormat.GetAbbreviatedMonthName(month);
      }
    }

    internal static IEnumerable<string> GetLongMonthNames(CultureInfo culture, DateTime contextValue)
    {
      int monthCount = culture.DateTimeFormat.Calendar.GetMonthsInYear(contextValue.Year);
      for (int month = 1; month <= monthCount; ++month)
      {
        yield return culture.DateTimeFormat.GetMonthName(month);
      }
    }

    internal static IEnumerable<string> GetShortDayOfWeekNames(CultureInfo culture)
    {
      return culture.DateTimeFormat.AbbreviatedDayNames;
    }

    internal static IEnumerable<string> GetLongDayOfWeekNames(CultureInfo culture)
    {
      return culture.DateTimeFormat.DayNames;
    }

    internal static IEnumerable<string> GetShortDesignatorNames(CultureInfo culture)
    {
      yield return culture.DateTimeFormat.AMDesignator.Substring(0, 1);
      yield return culture.DateTimeFormat.PMDesignator.Substring(0, 1);
    }

    internal static IEnumerable<string> GetLongDesignatorNames(CultureInfo culture)
    {
      yield return culture.DateTimeFormat.AMDesignator;
      yield return culture.DateTimeFormat.PMDesignator;
    }

    internal static Range<int> GetValueRange(string formatString, CultureInfo culture, DateTime contextValue)
    {
      if (formatString == "d" || formatString == "dd")
      {
        int daysInMonth = culture.DateTimeFormat.Calendar.GetDaysInMonth(contextValue.Year, contextValue.Month);
        return new Range<int>(1, daysInMonth);
      }
      else if (formatString == "M" || formatString == "MM")
      {
        int monthsInYear = culture.DateTimeFormat.Calendar.GetMonthsInYear(contextValue.Year);
        return new Range<int>(1, monthsInYear);
      }
      else if (formatString.StartsWith("y", StringComparison.Ordinal))
      {
        DateTime minDate = culture.DateTimeFormat.Calendar.MinSupportedDateTime;
        DateTime maxDate = culture.DateTimeFormat.Calendar.MaxSupportedDateTime;
        return new Range<int>(minDate.Year, maxDate.Year);
      }
      else if (formatString.StartsWith("h", StringComparison.Ordinal))
      {
        return new Range<int>(0, 12);
      }
      else if (formatString.StartsWith("H", StringComparison.Ordinal))
      {
        return new Range<int>(0, 23);
      }
      else if (formatString.StartsWith("m", StringComparison.Ordinal)
        || formatString.StartsWith("s", StringComparison.Ordinal))
      {
        return new Range<int>(0, 59);
      }

      return new Range<int>(Int32.MinValue, Int32.MaxValue);
    }
  }
}
