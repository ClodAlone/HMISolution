using System;
using System.Windows;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Negates the Left and Right values of a Thickness.
  /// </summary>
  [ValueConversion(typeof(Thickness), typeof(Thickness))]
  public class MarginInversionConverter : IValueConverter
  {
    /// <summary>
    /// Converts a Thickness to another Thickness which is equivalent except for negating
    /// the Left and Right properties.
    /// </summary>
    /// <param name="value">The value for which an inverted Thickness is required.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A Thickness with the same Top and Bottom values as the source,
    /// but whose Left and Right values have been negated (sign inverted).</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Thickness margin = (Thickness)value;
      return new Thickness(-margin.Left, margin.Top, -margin.Right, margin.Bottom);
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
