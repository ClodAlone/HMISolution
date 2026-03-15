using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a <see cref="SolidColorBrush"/> to a <see cref="Color"/>.
  /// </summary>
  public class BrushToColorConverter : IValueConverter
  {
    /// <summary>
    /// Converts a SolidColorBrush to a Color.
    /// </summary>
    /// <param name="value">The Color value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The Color extracted from the SolidColorBrush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      SolidColorBrush brush = (SolidColorBrush)value;
      return value == null ? new Color() : brush.Color;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>Converts the Color back into an equivalent SolidColorBrush.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return new SolidColorBrush((Color)value);
    }
  }
}
