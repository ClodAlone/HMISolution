using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Threading;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts an integer into the name of a month.
  /// </summary>
  public class IntegerToMonthStringConverter : IValueConverter
  {
    /// <summary>
    /// Converts an integer to a month string. The integer needs to be between 0 and 11 inclusively.
    /// </summary>
    /// <param name="value">The integer value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A month name based on the current thread culture and the given integer.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int month = (int)value;
      culture = Thread.CurrentThread.CurrentCulture;
      if (culture.DateTimeFormat != null && culture.DateTimeFormat.MonthNames != null)
      {
        return culture.DateTimeFormat.MonthNames[month];
      }
      DateTime date = new DateTime(2000, month, 1);
      return date.ToString("MMMM", culture);
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
