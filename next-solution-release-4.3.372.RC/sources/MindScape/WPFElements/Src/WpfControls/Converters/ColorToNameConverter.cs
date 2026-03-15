using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts between Color values and names.
  /// </summary>
  [ValueConversion(typeof(Color), typeof(string))]
  public class ColorToNameConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets whether this <see cref="ColorToNameConverter"/> will use the closest color for returning a name.
    /// If set to false, this converter will only return the color name if an exact match is found, Color.ToString will be returned if
    /// an exact match is not found. The default is false.
    /// </summary>
    public bool UseClosestColor { get; set; }

    /// <summary>
    /// Converts a Color value to a name.
    /// </summary>
    /// <param name="value">The Color value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The name of the color, if it is a standard color (named in the
    /// Colors class); otherwise a string representation of the color.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Color color = (Color)value;
        if (UseClosestColor)
        {
          return NamedColor.GetClosestColorName(color);
        }
        else
        {
          string name;
          bool isNamed = NamedColor.ColorNames.TryGetValue(color, out name);
          if (isNamed)
          {
            return name;
          }
          return color.ToString();
        }
      }
      return "null";
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
