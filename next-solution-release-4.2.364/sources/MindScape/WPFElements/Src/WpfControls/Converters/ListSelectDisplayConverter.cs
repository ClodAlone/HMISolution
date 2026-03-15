using System;
using System.Windows.Data;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Derives display text for items in a list-select editor.
  /// </summary>
  public class ListSelectDisplayConverter : IMultiValueConverter
  {
    /// <summary>
    /// Converts a value to a string, using any available property metadata.
    /// </summary>
    /// <param name="values">An array of two objects: the value to be converted, and
    /// the property metadata expressed as an <see cref="IPropertyInfo"/>.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string representation of the value.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      object value = values[0];
      IPropertyInfo propertyInfo = values[1] as IPropertyInfo;

      if (propertyInfo != null)
      {
        TypeConverter tc = propertyInfo.Converter;
        if (tc != null && tc.CanConvertTo(typeof(string)) && !"".Equals(value))
        {
          return tc.ConvertToString(value);
        }
      }

      if (value == null)
      {
        return null;
      }

      return value.ToString();
    }

    /// <summary>
    /// Converts a value from a binding target for writing to multiple binding sources.
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
