using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Converts the level of a <see cref="PropertyGridRow"/> to a horizontal offset.
  /// </summary>
  public sealed class LevelToIndentConverter : IValueConverter
  {
    /// <summary>
    /// Converts a value from a binding source for use by a binding target.
    /// </summary>
    /// <param name="value">The level of the <see cref="PropertyGridRow"/>.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A horizontal offset.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int indent = (int)value * 14;

      if (targetType == typeof(Thickness))
      {
        return new Thickness(indent, 0, 0, 0);
      }

      return indent;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}