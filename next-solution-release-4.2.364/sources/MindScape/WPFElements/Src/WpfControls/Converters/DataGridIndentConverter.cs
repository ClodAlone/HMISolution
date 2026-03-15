using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a grouping level and an indent size into the total number of pixels to indent of data grid row.
  /// </summary>
  public class DataGridIndentConverter : IMultiValueConverter
  {
    /// <summary>
    /// Converts a grouping level and an indent size into the total number of pixels to indent of data grid row.
    /// </summary>
    /// <param name="values">The values produced by the binding sources.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The total number of pixels to indent of data grid row.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int level = (int)values[0];
      double indent = (double)values[1];
      return level * indent;
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
