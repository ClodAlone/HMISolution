using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a color by changing the value of one of its channels.
  /// </summary>
  public class ColorChannelConverter : IValueConverter
  {
    private ColorChannel _channel = ColorChannel.Red;

    /// <summary>
    /// Gets or sets the channel that is affected by this <see cref="ColorChannelConverter"/>.
    /// </summary>
    public ColorChannel Channel
    {
      get { return _channel; }
      set { _channel = value; }
    }

    /// <summary>
    /// Converts the input <see cref="Color"/> by modifying the channel specified by the <see cref="Channel"/>
    /// property to the value specified in the parameter argument.
    /// </summary>
    /// <param name="value">The Color value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The value to which to set the specified <see cref="Channel"/>.
    /// This value must be convertible to <see cref="Byte"/>.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The modified color.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      byte channelValue = System.Convert.ToByte(parameter, CultureInfo.InvariantCulture);
      Color result = (Color)value;
      switch (_channel)
      {
        case ColorChannel.Alpha:
          result.A = channelValue;
          break;
        case ColorChannel.Red:
          result.R = channelValue;
          break;
        case ColorChannel.Green:
          result.G = channelValue;
          break;
        case ColorChannel.Blue:
          result.B = channelValue;
          break;
      }
      return result;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to multiple binding source.
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

  /// <summary>
  /// Identifies one of the channels of a <see cref="Color"/>.
  /// </summary>
  public enum ColorChannel
  {
    /// <summary>
    /// The alpha channel.
    /// </summary>
    Alpha,

    /// <summary>
    /// The red channel.
    /// </summary>
    Red,

    /// <summary>
    /// The green channel.
    /// </summary>
    Green,

    /// <summary>
    /// The saturation channel
    /// </summary>
    Saturation,

    /// <summary>
    /// The value channel.
    /// </summary>
    Value,

    /// <summary>
    /// The blue channel.
    /// </summary>
    Blue
  };
}
