using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a Color to an equivalent SolidColorBrush.
  /// </summary>
  [ValueConversion(typeof(Color), typeof(Brush))]
  public class ColorToBrushConverter : IValueConverter
  {
    /// <summary>
    /// Converts a Color to an equivalent SolidColorBrush.
    /// </summary>
    /// <param name="value">The Color value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A SolidColorBrush whose color is that of the binding source.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return new SolidColorBrush(value == null ? new Color() : (Color)value);
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
