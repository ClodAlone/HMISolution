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
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on a monthly schedule
  /// defined by a pattern (e.g. last Friday of every second month).
  /// </summary>
  public class MonthlyPatternRecurrencePattern : MonthlyRecurrencePattern
  {
    private readonly Occurrence _occurrence;
    private readonly DayOfRecurrence _dayOfRecurrence;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonthlyPatternRecurrencePattern"/> class.
    /// </summary>
    /// <param name="monthlyInterval">The number of months between recurrences.</param>
    /// <param name="occurrence">Which occurrence of the <paramref name="dayOfRecurrence"/> the item recurs on (e.g. first, last).</param>
    /// <param name="dayOfRecurrence">The days counted by <paramref name="occurrence"/> (e.g. weekdays, Fridays).</param>
    public MonthlyPatternRecurrencePattern(int monthlyInterval, Occurrence occurrence, DayOfRecurrence dayOfRecurrence)
      : base(monthlyInterval)
    {
      _occurrence = occurrence;
      _dayOfRecurrence = dayOfRecurrence;
    }

    /// <summary>
    /// Gets which occurrence of the <see cref="DayOfRecurrence"/> the item recurs on (e.g. first, last).
    /// </summary>
    public Occurrence Occurrence
    {
      get { return _occurrence; }
    }

    /// <summary>
    /// Gets the days counted by <see cref="Occurrence"/> (e.g. weekdays, Fridays).
    /// </summary>
    public DayOfRecurrence DayOfRecurrence
    {
      get { return _dayOfRecurrence; }
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
    protected override bool IncludesCore(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount)
    {
      int firstDay = GetDayOfMonth(startDate.Year, startDate.Month);
      DateTime firstDate = new DateTime(startDate.Year, startDate.Month, firstDay);
      if (firstDate < startDate)
      {
        firstDate = firstDate.AddMonths(MonthlyInterval);
        firstDay = GetDayOfMonth(firstDate.Year, firstDate.Month);
        firstDate = new DateTime(firstDate.Year, firstDate.Month, firstDay);
      }
      int months = DateTimeUtils.MonthsBetween(firstDate, day);
      float dividedMonths = (float)months / (float)MonthlyInterval;
      if (dividedMonths - (int)dividedMonths != 0)
      {
        return false;
      }

      int dayOfMonth = GetDayOfMonth(day.Year, day.Month);
      if (day.Day != dayOfMonth)
      {
        return false;
      }

      if (limitOccurrences && occurrenceCount.HasValue)
      {
        DateTime lastDate = firstDate.AddMonths(MonthlyInterval * occurrenceCount.Value - 1);
        lastDate = new DateTime(lastDate.Year, lastDate.Month, DateTime.DaysInMonth(lastDate.Year, lastDate.Month), 23, 59, 59);
        if (lastDate < day)
        {
          return false;
        }
      }
      return true;
    }

    private int GetDayOfMonth(int year, int month)
    {
      switch (DayOfRecurrence)
      {
        case DayOfRecurrence.Day: return DateTimeUtils.GetNthDayOfMonth(year, month, Occurrence);
        case DayOfRecurrence.Weekday: return DateTimeUtils.GetNthWeekdayOfMonth(year, month, Occurrence);
        case DayOfRecurrence.WeekendDay: return DateTimeUtils.GetNthWeekendDayOfMonth(year, month, Occurrence);
        default:
          DayOfWeek dayOfWeek = ConvertDayOfRecurrence(DayOfRecurrence);
          return DateTimeUtils.GetDayOfMonth(year, month, Occurrence, dayOfWeek);
      }
    }

    private DayOfWeek ConvertDayOfRecurrence(DayOfRecurrence dayOfRecurrence)
    {
      switch (dayOfRecurrence)
      {
        case DayOfRecurrence.Monday: return DayOfWeek.Monday;
        case DayOfRecurrence.Tuesday: return DayOfWeek.Tuesday;
        case DayOfRecurrence.Wednesday: return DayOfWeek.Wednesday;
        case DayOfRecurrence.Thursday: return DayOfWeek.Thursday;
        case DayOfRecurrence.Friday: return DayOfWeek.Friday;
        case DayOfRecurrence.Saturday: return DayOfWeek.Saturday;
        case DayOfRecurrence.Sunday: return DayOfWeek.Sunday;
        default: throw new InvalidOperationException(dayOfRecurrence + " can not be converted to a DayOfWeek");
      }
    }
  }
}
