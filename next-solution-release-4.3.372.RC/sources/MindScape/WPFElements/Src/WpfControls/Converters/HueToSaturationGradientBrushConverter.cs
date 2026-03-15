using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts the hue value of a color to a <see cref="LinearGradientBrush"/> used by a <see cref="ColorSquare"/>.
  /// </summary>
  public class HueToSaturationGradientBrushConverter : IValueConverter
  {
    /// <summary>
    /// Returns a <see cref="LinearGradientBrush"/> based on the given hue value of a color.
    /// </summary>
    /// <param name="value">The integer value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The gradient brush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double hue = (double)value;
      Color darkColor = ColorUtils.ColorFromHSV(hue, 1, 1);
      Color lightColor = ColorUtils.ColorFromHSV(hue, 0, 1);
      LinearGradientBrush gradientBrush = new LinearGradientBrush();
      gradientBrush.StartPoint = new Point(0, 0);
      gradientBrush.EndPoint = new Point(1, 0);
      gradientBrush.GradientStops.Add(new GradientStop() { Offset = 0, Color = lightColor });
      gradientBrush.GradientStops.Add(new GradientStop() { Offset = 1, Color = darkColor });
      return gradientBrush;
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
