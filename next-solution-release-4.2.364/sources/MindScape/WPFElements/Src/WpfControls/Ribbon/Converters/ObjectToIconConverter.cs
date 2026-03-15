using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts an object into an icon. If the object is a string, it is used as the source of an image.
  /// </summary>
  public class ObjectToIconConverter : IValueConverter
  {
    /// <summary>
    /// Converts an object into an icon. If the object is a string, it is used as the source of an image.
    /// Otherwise the object is returned.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The converted icon.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      object icon = value;
      if (value is string)
      {
        string str = (string)value;
        BitmapImage source = new BitmapImage(new Uri(str, UriKind.Relative));
        Image image = new Image() { Source = source };
        icon = image;
      }
      return icon;
    }

    /// <summary>
    /// This conversion direction is not implemented by this converter.
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
