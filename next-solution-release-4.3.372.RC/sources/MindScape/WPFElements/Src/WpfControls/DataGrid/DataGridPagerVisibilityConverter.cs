using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Determines the actual visibility of a built in <see cref="DataGridPager"/>.
  /// </summary>
  public class DataGridPagerVisibilityConverter : IMultiValueConverter
  {
    /// <summary>
    /// Returns the visibility of a <see cref="DataGridPager"/>.
    /// The first value is the IsPagerVisible value from the <see cref="DataGrid"/>.
    /// The second value is the page count.
    /// </summary>
    /// <param name="values">The values produced by the binding sources.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The visibility of a <see cref="DataGridPager"/>.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
      {
        if (values[0] is bool? && values[1] is int)
        {
          bool? isPagerVisible = (bool?)values[0];
          int pageCount = (int)values[1];
          if (isPagerVisible == null)
          {
            return pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
          }
          return isPagerVisible.Value ? Visibility.Visible : Visibility.Collapsed;
        }
      }
      return Visibility.Collapsed;
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
