using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts the content of a data grid cell based on a <see cref="TypeConverter"/>.
  /// </summary>
  public class CellDisplayConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the <see cref="TypeConverter"/> used in the conversion.
    /// </summary>
    public TypeConverter TypeConverter { get; set; }

    /// <summary>
    /// Converts the value to a string using the <see cref="TypeConverter"/>.
    /// </summary>
    /// <param name="value">The boolean value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The given value converted to a string.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (TypeConverter != null && !(value is DBNull))
      {
        if (TypeConverter != null && TypeConverter.CanConvertTo(typeof(string)))
        {
          return TypeConverter.ConvertToString(value);
        }
      }

      if (value == null)
      {
        return null;
      }

      return value.ToString();
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
