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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on every weekday
  /// (Monday to Friday).
  /// </summary>
  public class EveryWeekdayRecurrencePattern : RecurrencePattern
  {
    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="limitOccurrences">Whether to limit the number of occurrences considered.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider, if <paramref name="limitOccurrences"/> is true.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    protected override bool Includes(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount)
    {
      DateTime date = day.Date;
      TimeSpan days = date - startDate;
      if (days.Days < 0)
      {
        return false;
      }

      if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
      {
        return false;
      }

      if (limitOccurrences && occurrenceCount.HasValue)
      {
        DateTime startOfWeekOfStart = startDate.StartOfWeek(DayOfWeek.Monday);
        DateTime startOfWeekOfDay = day.StartOfWeek(DayOfWeek.Monday);
        TimeSpan daysOfWeeks = startOfWeekOfDay - startOfWeekOfStart;
        int weekCount = daysOfWeeks.Days / 7;
        int weekendCount = weekCount * 2;
        if (startDate.DayOfWeek == DayOfWeek.Sunday)
        {
          weekendCount--;
        }
        return days.Days - weekendCount < occurrenceCount.Value;
      }

      return true;
    }
  }
}
