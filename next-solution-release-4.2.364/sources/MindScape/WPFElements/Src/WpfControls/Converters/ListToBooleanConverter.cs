using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts an <see cref="IList"/> to a boolean depending on if the value of the Object property is contained
  /// within the list.
  /// </summary>
  public class ListToBooleanConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the object to be searched for within the list.
    /// </summary>
    public object Object { get; set; }

    /// <summary>
    /// Converts a list to a boolean depending on if the list contains the value of the Object property.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A value suitable for use by the binding target.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      IList list = value as IList;
      if (list == null)
      {
        return false;
      }
      foreach (object o in list)
      {
        if (o != null && o.Equals(Object))
        {
          return true;
        }
        if (o is TextDecoration && Object is TextDecoration)
        {
          TextDecoration td1 = (TextDecoration)o;
          TextDecoration td2 = (TextDecoration)Object;
          return td1.Location == td2.Location;
        }
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
      return null;
      // Things don't seem to work without implementing this method in some way.
      //bool boolean = (bool)value;
      //return boolean ? Object : null;
    }
  }
}
