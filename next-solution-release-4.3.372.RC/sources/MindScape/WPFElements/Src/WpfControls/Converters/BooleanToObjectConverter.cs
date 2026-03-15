using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a boolean to an object based on the TrueObject and FalseObject properties.
  /// </summary>
  public class BooleanToObjectConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the object to be returned if the value is true.
    /// </summary>
    public object TrueObject { get; set; }

    /// <summary>
    /// Gets or sets the object to be returned if the value is false.
    /// </summary>
    public object FalseObject { get; set; }

    /// <summary>
    /// Converts a boolean to an object.
    /// </summary>
    /// <param name="value">The boolean value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The object mapped to the boolean value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool b = (bool)value;
      return b ? TrueObject : FalseObject;
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
