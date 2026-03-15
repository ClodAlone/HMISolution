using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts an object to a boolean based on whether or not the object is equal to the value of the Object property.
  /// </summary>
  public class ObjectToBooleanConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the object to be compared with the object being convertered.
    /// </summary>
    public object Object { get; set; }

    /// <summary>
    /// Converts an object to a boolean depending on if the object is equal to the value of the Object property.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A value suitable for use by the binding target.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null && Object == null) { return true; }
      if (value == null) { return false; }
      return value.Equals(Object);
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
      // Things don't seem to work without implementing this method in some way.
      bool boolean = (bool)value;
      return boolean ? Object : null;
    }
  }
}
