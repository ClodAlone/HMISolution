using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Converts a <see cref="Type"/> into the name of a filter builder.
  /// </summary>
  public class TypeToFilterBuilderNameConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="Type"/> to the name of a filter builder.
    /// </summary>
    /// <param name="value">The type value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The name of a filter builder.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Type)
      {
        Type type = (Type)value;
        if (type != null)
        {
          IFilterBuilder builder = type.GetConstructor(Type.EmptyTypes).Invoke(null) as IFilterBuilder;
          if (builder != null)
          {
            return builder.Name;
          }
        }
      }
      else if (value is string)
      {
        return value;
      }
      return "";
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
