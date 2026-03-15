using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;
using System.Data;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts the items source of a data grid if necessary. This is useful for setting the items source to be a <see cref="DataTable"/>.
  /// </summary>
  public class DataGridItemsSourceConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="DataTable"/> into a <see cref="DataTableWrapper"/>.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An <see cref="IEnumerable"/> that can be used to set the items source of a <see cref="DataGrid"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is IEnumerable)
      {
        return value;
      }
      DataTable table = value as DataTable;
      if (table != null)
      {
        return new DataTableWrapper(table);
      }
      return new List<object>();
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
