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
using System.Threading;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Converts a date to a string suitable for display as the day caption in a monthly
  /// view.  By default, this is the day of the month (e.g. "15").  However, if the
  /// day is part of a week that spans a month boundary, then the month is appended if the
  /// day is the first day of that week, or the first day of the month.  For example,
  /// in the week beginning 29 March 2010, 29 March would be formatted as "29 Mar",
  /// 30 March as "30", 31 March as "31" and 1 April as "1 Apr".  Similar display
  /// logic applies to weeks that span a year boundary.
  /// </summary>
  public class DateTimeToDayOfMonthStringConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets whether or not to only display the number in the resulting string.
    /// </summary>
    public bool OnlyShowNumber { get; set; }

    /// <summary>
    /// Converts a date to a string suitable for display as the day caption in a monthly
    /// view.
    /// </summary>
    /// <param name="value">The <see cref="DateTime"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A string suitable for display as the day caption in a monthly view.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      culture = Thread.CurrentThread.CurrentCulture;
      DateTime date = (DateTime)value;
      int day = date.Day;
      int month = date.Month;
      int year = date.Year;

      if (OnlyShowNumber)
      {
        return day;
      }

      if (day == 1)
      {
        string result = day + " " + culture.DateTimeFormat.AbbreviatedMonthNames[month - 1];
        if (date.AddDays(-1).Year != year)
        {
          result += " " + year;
        }
        return result;
      }
      /*if (date.DayOfWeek == DayOfWeek.Monday)
      {
        DateTime endOfWeek = date.AddDays(6);
        if (date.Month != endOfWeek.Month)
        {
          return day + " " + culture.DateTimeFormat.AbbreviatedMonthNames[month - 1];
        }
      }*/
      return day;
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
