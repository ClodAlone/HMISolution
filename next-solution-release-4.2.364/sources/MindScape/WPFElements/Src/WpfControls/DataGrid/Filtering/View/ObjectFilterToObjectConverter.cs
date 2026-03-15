using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Converts an enum/boolean <see cref="IFilter"/> into an object and vice versa.
  /// </summary>
  public class ObjectFilterToObjectConverter : IValueConverter
  {
    /// <summary>
    /// Converts an <see cref="IFilter"/> to an object.
    /// </summary>
    /// <param name="value">The <see cref="IFilter"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The comparison object of the given <see cref="IFilter"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is EqualsFilter)
      {
        EqualsFilter filter = (EqualsFilter)value;
        if (filter != null)
        {
          return filter.Value;
        }
      }
      return null;
    }

    /// <summary>
    /// Converts an object to an <see cref="IFilter"/>.
    /// </summary>
    /// <param name="value">The object value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An <see cref="IFilter"/> based on the object.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || "".Equals(value)) // TODO: check for white space.
      {
        return null;
      }
      return new EqualsFilter(value);
    }
  }
}
