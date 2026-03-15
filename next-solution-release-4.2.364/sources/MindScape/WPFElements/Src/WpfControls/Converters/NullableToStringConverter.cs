using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Handles nullable types.
  /// </summary>
  public class NullableToStringConverter : IValueConverter
  {
    /// <summary>
    /// returns the given value with no conversion.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The unconverted value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    /// <summary>
    /// Returns null if the target type is nullable and the given value is the empty string. Otherwise returns the given value.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>Null if the target type is nullable and the given value is the empty string. Otherwise returns the given value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType.Name.Contains("Nullable"))
      {
        if ("".Equals(value))
        {
          return null;
        }
      }
      return value;
    }
  }
}
