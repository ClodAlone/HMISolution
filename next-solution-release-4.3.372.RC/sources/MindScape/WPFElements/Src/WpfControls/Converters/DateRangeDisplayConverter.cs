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
using System.Globalization;
using System.Threading;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Formats the date range of a <see cref="ScheduleView"/> suitably for display
  /// in the <see cref="SchedulerNavigationBar"/>.
  /// </summary>
  public class DateRangeDisplayConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the format string to be used for displaying months.  The default is
    /// full month name (e.g. "January").
    /// </summary>
    public string MonthFormat { get; set; }

    /// <summary>
    /// Formats the <see cref="DateRangeDisplayInfo"/> value for display in a view title.
    /// </summary>
    /// <param name="value">The <see cref="DateRangeDisplayInfo"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A formatted representation of the date range.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      culture = Thread.CurrentThread.CurrentCulture;
      DateRangeDisplayInfo rangeInfo = (DateRangeDisplayInfo)value;

      if (rangeInfo.DisplayMode == DateDisplayMode.Day)
      {
        if (rangeInfo.IsSingleDate)
        {
          return FormatSingleDate(rangeInfo.StartDate, culture, MonthFormat ?? "MMMM");
        }
        return FormatDateRange(rangeInfo.StartDate, rangeInfo.EndDate, culture, MonthFormat ?? "MMMM");
      }
      else
      {
        return FormatMonth(rangeInfo.EndDate, culture);
      }
    }

    private string FormatSingleDate(DateTime dateToFormat, CultureInfo culture, string monthFormat)
    {
      return dateToFormat.ToString("d " + monthFormat + " yyyy", culture);
    }

    private string FormatDateRange(DateTime startDate, DateTime endDate, CultureInfo culture, string monthFormat)
    {
      string startFormat = "%d";
      if (startDate.Year != endDate.Year)
      {
        startFormat = "d " + monthFormat + " yyyy";
      }
      else if (startDate.Month != endDate.Month)
      {
        startFormat = "d " + monthFormat;
      }

      string result = startDate.ToString(startFormat, culture);
      result += " - ";
      result += endDate.ToString("d " + monthFormat + " yyyy", culture);
      return result;
    }

    private string FormatMonth(DateTime date, CultureInfo culture)
    {
      return date.ToString("y", culture);
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
