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
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
#endif

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Formats a week as a date range string.
  /// </summary>
  public class WeekToStringConverter : IValueConverter
  {
    /// <summary>
    /// Formats a week as a date range string, e.g. "1 - 7 January 2010" or
    /// "31 December 2009 - 6 January 2010".
    /// </summary>
    /// <param name="value">The <see cref="WeekModel"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string containing the date range represented by the input value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      WeekModel week = (WeekModel)value;
      DateTime startDate = week.StartDate;
      DateTime endDate = week.EndDate;

      string startFormat = "%d";
      if (startDate.Year != endDate.Year)
      {
        startFormat = "d MMMM yyyy";
      }
      else if (startDate.Month != endDate.Month)
      {
        startFormat = "d MMMM";
      }

      string result = startDate.ToString(startFormat, culture);
      result += " - ";
      result += endDate.ToString("d MMMM yyyy", culture);
      return result;
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
