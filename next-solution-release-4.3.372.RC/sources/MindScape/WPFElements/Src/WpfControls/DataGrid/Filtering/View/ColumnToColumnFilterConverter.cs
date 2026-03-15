using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Links a <see cref="DataGridColumn"/> to an <see cref="IFilterDescription"/> via a <see cref="ColumnFilter"/>.
  /// </summary>
  public class ColumnToColumnFilterConverter : IValueConverter
  {
    private bool _useBooleanFilter = true;

    /// <summary>
    /// Gets or sets whether or not a boolean filter is used.
    /// </summary>
    public bool UseBooleanFilter
    {
      get { return _useBooleanFilter; }
      set
      {
        _useBooleanFilter = value;
      }
    }

    /// <summary>
    /// Converts a <see cref="DataGridColumn"/> to a <see cref="ColumnFilter"/>.
    /// </summary>
    /// <param name="value">The <see cref="DataGridColumn"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="ColumnFilter"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      DataGridColumn column = (DataGridColumn)value;
      return new ColumnFilter(column, UseBooleanFilter);
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
