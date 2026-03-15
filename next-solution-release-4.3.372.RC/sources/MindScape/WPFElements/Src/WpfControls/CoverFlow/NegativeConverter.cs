using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a double value to its negative.
  /// </summary>
  public class NegativeConverter : IValueConverter
  {
    /// <summary>
    /// Converts a double value from a binding source to its negative for use by a binding target.
    /// </summary>
    /// <param name="value">The double value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The negative of the original double value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return -((double)value);
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
