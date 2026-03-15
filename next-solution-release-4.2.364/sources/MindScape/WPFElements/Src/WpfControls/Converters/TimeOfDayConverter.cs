using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A <see cref="TypeConverter"/> for converting a string into a <see cref="TimeOfDay"/>.
  /// </summary>
  public class TimeOfDayConverter : TypeConverter
  {
    /// <summary>
    /// Converts the given string value into a <see cref="TimeOfDay"/>.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="Object"/> to convert.</param>
    /// <returns>A <see cref="TimeOfDay"/>.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string s = value as string;
      if (s != null)
      {
        int colonIndex = s.IndexOf(':');
        if (colonIndex > 0)
        {
          string hourString = s.Substring(0, colonIndex);
          try
          {
            int hour = Int32.Parse(hourString, culture);
            int maxHour = 24;
            int minHour = -1;
            if (hour > minHour && hour < maxHour)
            {
              if (s.Length >= colonIndex + 3)
              {
                string minuteString = s.Substring(colonIndex + 1, 2);
                try
                {
                  int minute = Int32.Parse(minuteString, culture);
                  return new TimeOfDay(hour, minute);
                }
                catch (FormatException) { }
              }
            }
          }
          catch (FormatException) { }
        }
      }
      return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Returns true if the sourceType is a string.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
    /// <returns>True if the sourceType is a string.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Converts the given <see cref="TimeOfDay"/> into a string. If the destination type is not a string, then
    /// the default operation will be performed.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="Object"/> to convert. This should be a <see cref="TimeOfDay"/>.</param>
    /// <param name="destinationType">The destination conversion type.</param>
    /// <returns>A string representing the TimeOfDay.</returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string) && value != null)
      {
        TimeOfDay time = (TimeOfDay)value;
        String result = time.Hour + ":";
        if (time.Minute < 10)
        {
          result += "0";
        }
        result += time.Minute;
        return result;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
