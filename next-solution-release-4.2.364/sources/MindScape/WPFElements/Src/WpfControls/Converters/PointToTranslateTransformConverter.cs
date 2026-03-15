using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a <see cref="Point"/> to a <see cref="TranslateTransform"/>.
  /// </summary>
  public class PointToTranslateTransformConverter : IValueConverter
  {
    /// <summary>
    /// Returns a <see cref="TranslateTransform"/> based on the given <see cref="Point"/>.
    /// </summary>
    /// <param name="value">The <see cref="Point"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="TranslateTransform"/> based on the given <see cref="Point"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Point point = (Point)value;
      TranslateTransform translation = new TranslateTransform() { X = point.X, Y = point.Y };
      return translation;
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
