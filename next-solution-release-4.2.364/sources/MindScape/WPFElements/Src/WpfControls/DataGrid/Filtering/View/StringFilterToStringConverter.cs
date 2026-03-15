using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Converts an <see cref="IFilter"/> to a string and vice versa.
  /// </summary>
  public class StringFilterToStringConverter : IValueConverter
  {
    /// <summary>
    /// Converts an <see cref="IFilter"/> to a string expression.
    /// </summary>
    /// <param name="value">The <see cref="IFilter"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string expression of the filter.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      IFilter filter = (IFilter)value;
      StartsWithFilter startsWith = filter as StartsWithFilter;
      if (startsWith != null)
      {
        return startsWith.Value;
      }
      return "";
    }

    /// <summary>
    /// Converts a string to an <see cref="IFilter"/>.
    /// </summary>
    /// <param name="value">The string value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An <see cref="IFilter"/> based on the string expression.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string str = (string)value;
      if (string.IsNullOrEmpty(str))
      {
        return null;
      }
      return new StartsWithFilter(str, false);
    }
  }
}
