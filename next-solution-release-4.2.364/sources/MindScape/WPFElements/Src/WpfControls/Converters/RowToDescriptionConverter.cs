using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Obtains the description of a property from a <see cref="PropertyGridRow"/>.
  /// </summary>
  public class RowToDescriptionConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="PropertyGridRow"/> to the description of the contained property.
    /// </summary>
    /// <param name="value">The PropertyGridRow.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The description string of the property contained in the row.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      PropertyGridRow row = value as PropertyGridRow;
      if (row == null)
      {
        return String.Empty;
      }

      IPropertyInfo property = row.Node.Property;
      return property.Description;
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
