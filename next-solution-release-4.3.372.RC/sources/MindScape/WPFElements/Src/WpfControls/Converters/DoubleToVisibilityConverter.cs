using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a double into a visibility based on a constraint on the double value.
  /// This is useful for hiding the gripper lines in a scroll bar thumb style if the thumb gets too small.
  /// </summary>
  public class DoubleToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the visibility threshold.
    /// </summary>
    public double VisibleThreshold { get; set; }

    /// <summary>
    /// Converts a double into a visibility based on the visible threshold.
    /// If the given value is greater than or equal to the visible threshold, then Visibility.Visble will be returned.
    /// Otherwise Visibility.Collapsed will be returned.
    /// </summary>
    /// <param name="value">The double value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>Visibility.Visible if the given value is greater than or equal to the visible threshold.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double d = (double)value;
      return d >= VisibleThreshold ? Visibility.Visible : Visibility.Collapsed;
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
