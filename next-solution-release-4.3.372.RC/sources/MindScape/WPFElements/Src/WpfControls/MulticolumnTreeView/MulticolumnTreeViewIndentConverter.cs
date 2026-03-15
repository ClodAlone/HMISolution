using System;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Determines the indent for a level in a <see cref="MulticolumnTreeView"/>.
  /// </summary>
  [ValueConversion(typeof(int), typeof(Thickness))]
  public class MulticolumnTreeViewIndentConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the indent to apply at each level.
    /// </summary>
    public double Indent { get; set; }

    /// <summary>
    /// Converts a level in the <see cref="MulticolumnTreeView"/> hierarchy to
    /// an indent.
    /// </summary>
    /// <param name="value">The level.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="Thickness"/> representing the indent.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double leftMargin = System.Convert.ToInt32(value, culture) * Indent;
      return new Thickness(leftMargin, 0, 0, 0);
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
