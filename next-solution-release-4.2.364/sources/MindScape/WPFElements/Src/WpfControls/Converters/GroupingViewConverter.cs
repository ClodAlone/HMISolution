using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Converts a PropertyGrid to a boolean value indicating if grouping is enabled or disabled
  /// </summary>
  [ValueConversion(typeof(ObservableCollection<GroupDescription>), typeof(bool))]
  public class GroupingViewConverter : IValueConverter
  {
    /// <summary>
    /// Converts a ObservableCollection of generic type GroupDescription to a boolean value indicating if grouping is enabled or disabled
    /// </summary>
    /// <param name="value">The ObservableCollection of generic type GroupDescription value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A Boolean whose value is true if grouping is enabled, otherwise false.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var count = (int) value;

      return (count == 0);
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
