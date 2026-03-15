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
  /// Formats the time component of a <see cref="SchedulerElementDisplayState"/>
  /// for display.
  /// </summary>
  public class SchedulerElementDisplayStateToTimeStringConverter : TimeStringConverterBase, IValueConverter
  {
    /// <summary>
    /// Formats the time component of a <see cref="SchedulerElementDisplayState"/>
    /// for display.
    /// </summary>
    /// <param name="value">The <see cref="SchedulerElementDisplayState"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A formatted representation of the time selected by the <see cref="TimeStringConverterBase.TimeEnd"/> property.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        SchedulerElementDisplayState state = (SchedulerElementDisplayState)value;
        DateTime start = state.ScheduleItemStartTime;
        DateTime end = state.ScheduleItemEndTime;

        if (!state.IsScheduleItemStartVisible && TimeEnd == TimeEnd.StartTime)
        {
          return "";
        }
        if (!state.IsScheduleItemEndVisible && TimeEnd == TimeEnd.EndTime)
        {
          return "";
        }

        if (start.Hour == 0 && start.Minute == 0 && end.Hour == 0 && end.Minute == 0)
        {
          return "";
        }

        DateTime timeToConvert = TimeEnd == TimeEnd.StartTime ? start : end;
        return FormatTimeOfDay(timeToConvert, culture);
      }
      return "";
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
