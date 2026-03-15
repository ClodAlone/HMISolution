using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Determines whether or not to display the filter button for a given <see cref="DataGridColumn"/>.
  /// </summary>
  public class DataGridColumnToFilterVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="DataGridColumn"/> into the visibility of the column header filter button.
    /// </summary>
    /// <param name="value">The value to be modified.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The visibility of the filter button for the given <see cref="DataGridColumn"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      DataGridColumn column = (DataGridColumn)value;
      Visibility visibility = Visibility.Collapsed;
      if (column.PropertyInfo != null)
      {
        // String values
        Type type = column.PropertyInfo.PropertyType;
        if (type == typeof(string))
        {
          visibility = Visibility.Visible;
        }

        // Enum values
        TypeConverter converter = column.PropertyInfo.Converter;
        if (converter == null && type != null)
        {
          converter = TypeDescriptor.GetConverter(type);
        }
        if (ReflectionUtilities.ShouldUseStandardValues(converter, type))
        {
          visibility = Visibility.Visible;
        }

        // Comparable values
        if (IsValidTypeCode(type))
        {
          visibility = Visibility.Visible;
        }
      }
      return visibility;
    }

    private bool IsValidTypeCode(Type type)
    {
      TypeCode code = Type.GetTypeCode(type);
      switch (code)
      {
        //case TypeCode.Byte:
        case TypeCode.Decimal:
        case TypeCode.Double:
        //case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        //case TypeCode.SByte:
        //case TypeCode.Single:
        //case TypeCode.UInt16:
        //case TypeCode.UInt32:
        //case TypeCode.UInt64:
        case TypeCode.DateTime:
          return true;
      }
      return false;
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
