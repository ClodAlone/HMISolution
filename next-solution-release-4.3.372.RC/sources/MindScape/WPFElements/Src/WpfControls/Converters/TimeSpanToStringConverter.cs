using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a <see cref="TimeSpan"/> to a formated string showing the number of minutes, hours, days or weeks.
  /// </summary>
  public class TimeSpanToStringConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a formated string.
    /// </summary>
    /// <param name="value">The <see cref="TimeSpan"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string containing the formatted <see cref="TimeSpan"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return "0 minutes";
      }
      TimeSpan time = (TimeSpan)value;
      String s = DateTimeUtils.ConvertTimeSpanToString(time);
      return s;
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
