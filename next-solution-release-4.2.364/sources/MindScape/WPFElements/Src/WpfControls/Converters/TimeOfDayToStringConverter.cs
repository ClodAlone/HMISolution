using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a <see cref="TimeOfDay"/> object into a formatted string displaying the hours, minutes and AM/PM designator.
  /// Hours are displayed in 12 hour clock time. Minutes are always displayed with 2 digits.
  /// </summary>
  public class TimeOfDayToStringConverter : TimeStringConverterBase, IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="TimeOfDay"/> to a display string.
    /// </summary>
    /// <param name="value">The <see cref="TimeOfDay"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string containing the formatted <see cref="TimeOfDay"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return String.Empty;
      }

      DateTime date;
      if (value is TimeOfDay)
      {
        TimeOfDay time = (TimeOfDay)value;
        date = new DateTime(2000, 1, 1, time.Hour, time.Minute, 0);
      }
      else
      {
        TimeSpan time = (TimeSpan)value;
        date = new DateTime(200, 1, 1) + time;
      }
      return FormatTimeOfDay(date, culture);
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
