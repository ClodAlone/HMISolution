using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Converts a double to a <see cref="Thickness"/>.
  /// </summary>
  public class DoubleToThicknessConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets whether or not the double is applied to the left of the <see cref="Thickness"/>.
    /// </summary>
    public bool IsLeft { get; set; }

    /// <summary>
    /// Gets or sets whether or not the double is applied to the top of the <see cref="Thickness"/>.
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// Gets or sets whether or not the double is applied to the bottom of the <see cref="Thickness"/>.
    /// </summary>
    public bool IsBottom { get; set; }

    /// <summary>
    /// Gets or sets whether or not the double is applied to the right of the <see cref="Thickness"/>.
    /// </summary>
    public bool IsRight { get; set; }

    /// <summary>
    /// Returns a <see cref="Thickness"/>.
    /// </summary>
    /// <param name="value">The double value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="Thickness"/> based on the given double.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double d = (double)value;
      return new Thickness(IsLeft ? d : 0, IsTop ? d : 0, IsRight ? d : 0, IsBottom ? d : 0);
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
