using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts an object into an icon. If the object is a string, it is used as the source of an image.
  /// </summary>
  public class ObjectToIconTypeConverter : TypeConverter
  {
    /// <summary>
    /// Converts an object into an icon. If the object is a string, it is used as the source of an image.
    /// Otherwise the object is returned.
    /// </summary>
    /// <param name="context">The context of the conversion.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted icon.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string)
      {
        string str = (string)value;
        BitmapImage source = new BitmapImage(new Uri(str, UriKind.Relative));
        Image image = new Image() { Source = source, Stretch = Stretch.None };
        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor); // TODO this may not be needed for .NET 4.0+ due to AllowLayoutRendering property.
        return image;
      }
      return base.ConvertFrom(context, culture, value);
    }
  }
}
