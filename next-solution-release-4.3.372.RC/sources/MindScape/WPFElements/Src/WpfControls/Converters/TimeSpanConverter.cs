using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A <see cref="TypeConverter"/> for converting a string into a <see cref="TimeSpan"/>.
  /// </summary>
  public class TimeSpanConverter : TypeConverter
  {
    /// <summary>
    /// Converts the given string value into a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="Object"/> to convert.</param>
    /// <returns>A <see cref="TimeSpan"/>.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string s = value as string;
      if (s != null)
      {
        TimeSpan timeSpan = DateTimeUtils.ConvertStringToTimeSpan(s);
        if (timeSpan.Ticks == 0)
        {
          string[] parts = s.Split(':');
          int[] numericalParts;
          bool success = GetNumericParts(parts, out numericalParts);
          if (success)
          {
            if (numericalParts.Length == 3)
            {
              timeSpan = new TimeSpan(numericalParts[0], numericalParts[1], numericalParts[2]);
            }
            else if (numericalParts.Length == 4)
            {
              timeSpan = new TimeSpan(numericalParts[0], numericalParts[1], numericalParts[2], numericalParts[3]);
            }
          }
        }
        return timeSpan;
      }
      return base.ConvertFrom(context, culture, value);
    }

    private bool GetNumericParts(string[] parts, out int[] numericParts)
    {
      numericParts = new int[parts.Length];
      int index = 0;
      foreach (string s in parts)
      {
        int value;
        bool success = int.TryParse(s, out value);
        if (!success)
        {
          return false;
        }
        numericParts[index] = value;
        index++;
      }
      return true;
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
  }
}
