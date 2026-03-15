using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides a base class for converters that output strings representing times.
  /// </summary>
  public class TimeStringConverterBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeStringConverterBase"/> class.
    /// </summary>
    public TimeStringConverterBase()
    {
      TimeDisplayMode = TimeDisplayMode.TwelveHourTime;
    }

    /// <summary>
    /// Gets or sets the <see cref="TimeDisplayMode"/> that specifies how to convert the <see cref="TimeOfDay"/>
    /// object into a string.
    /// </summary>
    public TimeDisplayMode TimeDisplayMode { get; set; }

    /// <summary>
    /// Gets or sets which end of a range is to be formatted.
    /// </summary>
    public TimeEnd TimeEnd { get; set; }

    /// <summary>
    /// Formats a time of day for display.
    /// </summary>
    /// <param name="timeToConvert">The time to be displayed.</param>
    /// <param name="culture">The culture to use in formatting.</param>
    /// <returns>A string representing the time of day part of the timeToConvert.</returns>
    protected string FormatTimeOfDay(DateTime timeToConvert, CultureInfo culture)
    {
      return FormatTimeOfDay(timeToConvert, culture, TimeDisplayMode);
    }

    /// <summary>
    /// Formats a time of day for display.
    /// </summary>
    /// <param name="timeToConvert">The time to be displayed.</param>
    /// <param name="culture">The culture to use in formatting.</param>
    /// <param name="timeMode">The time display mode.</param>
    /// <returns>A string representing the time of day part of the timeToConvert.</returns>
    protected static string FormatTimeOfDay(DateTime timeToConvert, CultureInfo culture, TimeDisplayMode timeMode)
    {
      return timeMode == TimeDisplayMode.TwelveHourTime ? Format12HourTimeOfDay(timeToConvert, culture) : Format24HourTimeOfDay(timeToConvert, culture);
    }

    private static string Format12HourTimeOfDay(DateTime timeToConvert, CultureInfo culture)
    {
      int hour = timeToConvert.Hour;
      if (hour == 0)
      {
        hour = 12;
      }
      if (hour > 12)
      {
        hour -= 12;
      }
      String result = hour + ":" + timeToConvert.Minute.ToString("D2", culture);
      if (timeToConvert.Hour < 12)
      {
        result += culture.DateTimeFormat.AMDesignator;
      }
      else
      {
        result += culture.DateTimeFormat.PMDesignator;
      }
      return result;
    }

    private static string Format24HourTimeOfDay(DateTime timeToConvert, CultureInfo culture)
    {
      string s = "";
      if (timeToConvert.Hour < 10)
      {
        s = "0";
      }
      s += timeToConvert.Hour + ":";
      s += timeToConvert.Minute.ToString("D2", culture);
      return s;
    }
  }
}
