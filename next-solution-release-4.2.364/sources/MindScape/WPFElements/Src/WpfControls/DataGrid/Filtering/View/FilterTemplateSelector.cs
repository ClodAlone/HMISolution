using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A <see cref="DataTemplateSelector"/> for selecting filter templates.
  /// </summary>
  public class FilterTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the filter template used for string columns.
    /// </summary>
    public DataTemplate StringFilterTemplate { get; set; }

    /// <summary>
    /// Gets or sets the filter template used for numeric columns.
    /// </summary>
    public DataTemplate NumericFilterTemplate { get; set; }

    /// <summary>
    /// Gets or sets the filter template used for enum and boolean columns.
    /// </summary>
    public DataTemplate EnumFilterTemplate { get; set; }

    /// <summary>
    /// Gets or sets the filter template used for columns that can't be filtered.
    /// </summary>
    public DataTemplate NoFilterTemplate { get; set; }

    /// <summary>
    /// Selects a filter template for the given <see cref="DataGridColumn"/>.
    /// </summary>
    /// <param name="item">The <see cref="DataGridColumn"/>.</param>
    /// <param name="container">The template container.</param>
    /// <returns>The filter template to be used by the given <see cref="DataGridColumn"/>.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      DataGridColumn column = (DataGridColumn)item;
      if (column != null && column.PropertyInfo != null)
      {
        Type type = column.PropertyInfo.PropertyType;
        if (type == typeof(string))
        {
          return StringFilterTemplate;
        }

        if (type != null && !column.PropertyInfo.IsDefined(typeof(TypeConverterAttribute), true))
        {
          if (type.IsEnum || type == typeof(bool))
          {
            return EnumFilterTemplate;
          }
        }

        TypeConverter converter = column.PropertyInfo.Converter;
        if (converter == null && type != null)
        {
          converter = TypeDescriptor.GetConverter(type);
        }
        if (ReflectionUtilities.ShouldUseStandardValues(converter, type))
        {
          /*if (converter.GetStandardValuesExclusive())
          {
            return EnumFilterTemplate;
          }
          else
          {
            return EnumFilterTemplate;
          }*/
          return EnumFilterTemplate;
        }

        if (IsNumericType(type))
        {
          return NumericFilterTemplate;
        }
      }
      return NoFilterTemplate;
    }

    private bool IsNumericType(Type type)
    {
      TypeCode code = Type.GetTypeCode(type);
      switch (code)
      {
        case TypeCode.Byte:
        case TypeCode.Decimal:
        case TypeCode.Double:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.SByte:
        case TypeCode.Single:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          return true;
      }
      return false;
    }
  }
}
