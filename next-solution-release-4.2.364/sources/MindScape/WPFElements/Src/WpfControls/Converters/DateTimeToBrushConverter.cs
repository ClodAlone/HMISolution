using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a DateTime to a Brush based on whether or not the DateTime is DateTime.Now.
  /// </summary>
  public class DateTimeToBrushConverter : IValueConverter
  {
    /// <summary>
    /// The brush to select when the source is DateTime.Now
    /// </summary>
    public Brush TodayBrush { get; set; }

    /// <summary>
    /// The brush to select when the source is not DateTime.Now.
    /// </summary>
    public Brush NeutralBrush { get; set; }

    /// <summary>
    /// Selects a brush according to whether or not the given DateTime is DateTime.Now.
    /// </summary>
    /// <param name="value">The DateTime value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The selected brush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return ((DateTime)value).Date.Equals(DateTime.Now.Date) ? TodayBrush : NeutralBrush;
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
