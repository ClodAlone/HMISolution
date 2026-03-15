using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a <see cref="Color"/> or a <see cref="SolidColorBrush"/> into a <see cref="LinearGradientBrush"/> that displays the effects of changing
  /// a specified channel of the given color. The second value in the mutlti binding is a hue value that is considered for the 'Saturation' and 'Value' channels.
  /// This is used for displaying mixer channels in a <see cref="ChannelColorPicker"/> template.
  /// </summary>
  public class ColorChannelAdjustmentBrushMultiConverter : IMultiValueConverter
  {
    private ColorChannel _channel = ColorChannel.Red;

    /// <summary>
    /// Gets or sets the channel that is affected by this <see cref="ColorChannelAdjustmentBrushMultiConverter"/>.
    /// </summary>
    public ColorChannel Channel
    {
      get { return _channel; }
      set { _channel = value; }
    }

    /// <summary>
    /// Converts a <see cref="Color"/> or a <see cref="SolidColorBrush"/> into a <see cref="LinearGradientBrush"/> that displays the effects of changing
    /// a specified channel of the given color. The second value in the mutlti binding is a hue value that is considered for the 'Saturation' and 'Value' channels.
    /// </summary>
    /// <param name="values">The values produced by the binding sources.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="LinearGradientBrush"/> for displaying mixer channels in a <see cref="ChannelColorPicker"/>.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      byte channelValue = System.Convert.ToByte(parameter, CultureInfo.InvariantCulture);
      Color color = new Color();
      if (values[0] is SolidColorBrush)
      {
        SolidColorBrush brush = (SolidColorBrush)values[0];
        color = brush.Color;
      }
      else if (values[0] is Color)
      {
        color = (Color)values[0];
      }
      else
      {
        return new SolidColorBrush();
      }
      double hue = (double)NumericalUtils.ConvertToDouble(values[1]);
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
          color = ColorUtils.ColorFromHSV(hue, channelValue, v);
          color.A = originalColor.A;
          break;
        case ColorChannel.Value:
          ColorUtils.ColorToHSV(color, out h, out s, out v);
          color = ColorUtils.ColorFromHSV(hue, s, channelValue);
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
    /// Converts a value from a binding target for writing to multiple binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetTypes">The types to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
