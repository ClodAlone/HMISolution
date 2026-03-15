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
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on a weekly basis.
  /// </summary>
  public class WeeklyRecurrencePattern : RecurrencePattern
  {
    private readonly int _weeklyInterval;
    private readonly ReadOnlyCollection<DayOfWeek> _daysOfWeek;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyRecurrencePattern"/> class.
    /// </summary>
    /// <param name="weeklyInterval">The number of weeks between recurrences.</param>
    /// <param name="daysOfWeek">The days of the week on which the item recurs.</param>
    public WeeklyRecurrencePattern(int weeklyInterval, params DayOfWeek[] daysOfWeek)
    {
      if (daysOfWeek == null)
      {
        throw new ArgumentNullException("daysOfWeek");
      }

      _weeklyInterval = weeklyInterval;
      _daysOfWeek = new ReadOnlyCollection<DayOfWeek>(daysOfWeek);
    }

    /// <summary>
    /// Gets the number of weeks between recurrences.
    /// </summary>
    public int WeeklyInterval
    {
      get { return _weeklyInterval; }
    }

    /// <summary>
    /// Gets the days of the week on which the item recurs.
    /// </summary>
    public ReadOnlyCollection<DayOfWeek> DaysOfWeek
    {
      get { return _daysOfWeek; }
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
      return Includes(startDate, day, limitOccurrences, occurrenceCount, DayOfWeek.Monday);
    }

    /// <summary>
    /// Gets whether the pattern occurs on the specified day, assuming a given start date
    /// and maximum occurrence count.
    /// </summary>
    /// <param name="startDate">The start date of recurrence.</param>
    /// <param name="day">The day for which to check whether the recurrence occurs.</param>
    /// <param name="limitOccurrences">Whether to limit the number of occurrences considered.</param>
    /// <param name="occurrenceCount">The maximum number of occurrences to consider, if <paramref name="limitOccurrences"/> is true.</param>
    /// <param name="firstDayOfWeek">The first day of a week.</param>
    /// <returns>true if the pattern occurs on <paramref name="day"/>; otherwise false.</returns>
    protected override bool Includes(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount, DayOfWeek firstDayOfWeek)
    {
      DateTime date = day.Date;
      if (!_daysOfWeek.Contains(date.DayOfWeek))
      {
        return false;
      }
      int appointmentsPerWeek = _daysOfWeek.Count;

      DateTime currentDay = startDate.Date;
      while (!_daysOfWeek.Contains(currentDay.DayOfWeek))
      {
        if (currentDay.DayOfWeek == DateTimeUtils.LastDayOfWeek(firstDayOfWeek))
        {
          currentDay = currentDay.AddDays(7 * (_weeklyInterval - 1));
        }
        currentDay = currentDay.AddDays(1);
      }
      TimeSpan timeToCalculateWeek = date.StartOfWeek(firstDayOfWeek) - currentDay.StartOfWeek(firstDayOfWeek);
      TimeSpan time = date - currentDay;
      if (time.Days < 0)
      {
        return false;
      }
      float weeks = timeToCalculateWeek.Days / 7f;
      weeks = (int)Math.Ceiling(weeks);
      float test = weeks / (float)_weeklyInterval;
      if (test - (int)test > 0)
      {
        return false;
      }

      if (limitOccurrences && occurrenceCount.HasValue)
      {
        TimeSpan fromFirstInstance = date - currentDay;
        weeks = fromFirstInstance.Days / 7f;

        int totalWeeks = (occurrenceCount.Value / appointmentsPerWeek) * _weeklyInterval;
        DateTime endDate = startDate.Date;
        endDate = endDate.AddDays(totalWeeks * 7 - 1);
        int remainingOccurrences = occurrenceCount.Value % appointmentsPerWeek;
        while (remainingOccurrences > 0)
        {
          endDate = endDate.AddDays(1);
          if (_daysOfWeek.Contains(endDate.DayOfWeek))
          {
            remainingOccurrences--;
          }
        }

        if (!DateTimeUtils.IsSameDay(date, endDate) && date > endDate)
        {
          return false;
        }
      }

      return true;
    }
  }
}
