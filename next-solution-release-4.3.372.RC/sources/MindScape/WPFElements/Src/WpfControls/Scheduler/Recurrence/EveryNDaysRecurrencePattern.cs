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
  /// Represents the recurrence of a <see cref="ScheduleItem"/> at a regular
  /// daily interval.
  /// </summary>
  public class EveryNDaysRecurrencePattern : RecurrencePattern
  {
    private readonly int _dailyInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="EveryNDaysRecurrencePattern"/> class.
    /// </summary>
    /// <param name="dailyInterval">The number of days between occurrences.</param>
    public EveryNDaysRecurrencePattern(int dailyInterval)
    {
      _dailyInterval = dailyInterval;
    }

    /// <summary>
    /// Gets the number of days between occurrences.
    /// </summary>
    public int DailyInterval
    {
      get { return _dailyInterval; }
    }

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

      float dividedDays = days.Days / (float)_dailyInterval - 1;
      if (dividedDays - (int)dividedDays != 0)
      {
        return false;
      }
      if (limitOccurrences && dividedDays + 2 > occurrenceCount.Value)
      {
        return false;
      }
      return true;
    }
  }
}
