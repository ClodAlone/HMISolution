using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a color or solid brush to a gradient brush showing the effects
  /// of adjusting one channel on that color.  This is used for displaying
  /// mixer channels in a <see cref="ChannelColorPicker"/> template.
  /// </summary>
  public class ColorChannelAdjustmentBrushConverter : IValueConverter
  {
    private ColorChannel _channel = ColorChannel.Red;

    /// <summary>
    /// Gets or sets the channel that is affected by this <see cref="ColorChannelAdjustmentBrushConverter"/>.
    /// </summary>
    public ColorChannel Channel
    {
      get { return _channel; }
      set { _channel = value; }
    }

    /// <summary>
    /// Converts a solid color to a gradient brush.
    /// </summary>
    /// <param name="value">The Color or SolidColorBrush value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The value to which to set the specified <see cref="Channel"/>.
    /// This value must be convertible to <see cref="Byte"/>.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A gradient brush from the original to the adjusted color.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      byte channelValue = System.Convert.ToByte(parameter, CultureInfo.InvariantCulture);
      Color color = new Color();
      if (value is SolidColorBrush)
      {
        SolidColorBrush brush = (SolidColorBrush)value;
        color = brush.Color;
      }
      else if (value is Color)
      {
        color = (Color)value;
      }
      else
      {
        return new SolidColorBrush();
      }
      Color originalColor = new Color() { A = color.A, R = color.R, G = color.G, B = color.B };
      double h, s, v;
      switch (_channel)
      {
        case ColorChannel.Alpha:
          color.A = channelValue;
          break;
        case ColorChannel.Red:
          color.R = channelValue;
          break;
        case ColorChannel.Green:
          color.G = channelValue;
          break;
        case ColorChannel.Blue:
          color.B = channelValue;
          break;
        case ColorChannel.Saturation:
          ColorUtils.ColorToHSV(color, out h, out s, out v);
          color = ColorUtils.ColorFromHSV(h, channelValue, v);
          color.A = originalColor.A;
          break;
        case ColorChannel.Value:
          ColorUtils.ColorToHSV(color, out h, out s, out v);
          color = ColorUtils.ColorFromHSV(h, s, channelValue);
          color.A = originalColor.A;
          break;
      }
      LinearGradientBrush result = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
      if (channelValue == 0)
      {
        result.GradientStops.Add(new GradientStop() { Color = color, Offset = 0 });
        result.GradientStops.Add(new GradientStop() { Color = originalColor, Offset = 1 });
      }
      else
      {
        result.GradientStops.Add(new GradientStop() { Color = originalColor, Offset = 0 });
        result.GradientStops.Add(new GradientStop() { Color = color, Offset = 1 });
      }
      return result;
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
