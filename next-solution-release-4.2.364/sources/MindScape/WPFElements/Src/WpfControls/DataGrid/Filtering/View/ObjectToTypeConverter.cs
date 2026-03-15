using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Converts an object to its <see cref="Type"/>.
  /// </summary>
  public class ObjectToTypeConverter : IValueConverter
  {
    /// <summary>
    /// Converts an object to its <see cref="Type"/>.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The <see cref="Type"/> of the object.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        return value.GetType();
      }
      return null;
    }

    /// <summary>
    /// Converts a <see cref="Type"/> to an object instance.
    /// </summary>
    /// <param name="value">The <see cref="Type"/> value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An object instance from the <see cref="Type"/>.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Type type = (Type)value;
      if (type != null)
      {
        return type.GetConstructor(Type.EmptyTypes).Invoke(null);
      }
      return null;
    }
  }
}
