using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Determines the vertical offset at which to display items corresponding to a particular time of day.
  /// </summary>
  public class TimeOfDayVerticalOffsetConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the vertical offset to be applied per hour of the time of day.
    /// </summary>
    public double HourSlotHeight { get; set; }

    /// <summary>
    /// Determines the vertical offset corresponding to the time of day.
    /// </summary>
    /// <param name="value">The <see cref="DateTime"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The vertical offset corresponding to the time of day of the input value.  (Date components
    /// of the input DateTime are ignored.)</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      DateTime time = (DateTime)value;
      int hour = time.Hour;
      int min = time.Minute;
      return new Thickness(5, hour * HourSlotHeight + min * (60 / HourSlotHeight) - 14, 0, 0); //TODO: 14 = height of the current time pointer - 1 (defined by a style, so should be moved to a property)
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
