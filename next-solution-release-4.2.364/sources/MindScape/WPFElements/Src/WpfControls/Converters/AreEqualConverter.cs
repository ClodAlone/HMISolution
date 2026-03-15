using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Converts two objects to a boolean value indicating whether the two are reference-equal.
  /// </summary>
  public class AreEqualConverter : IMultiValueConverter
  {
    /// <summary>
    /// Converts two values to a boolean indicating whether they are reference-equal.
    /// </summary>
    /// <param name="values">The values produced by the binding sources.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>true if the two values are reference-equal; otherwise false.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return values[0] == values[1];
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
