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
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on a yearly
  /// basis according to a pattern (e.g. the first Tuesday of November every four years).
  /// </summary>
  public class YearlyPatternRecurrencePattern : YearlyRecurrencePattern
  {
    private readonly Occurrence _occurrence;
    private readonly DayOfRecurrence _dayOfRecurrence;

    /// <summary>
    /// Initializes a new instance of the <see cref="YearlyPatternRecurrencePattern"/> class.
    /// </summary>
    /// <param name="yearlyInterval">The number of years between recurrences.</param>
    /// <param name="month">The month in which the item recurs. (1 through 12)</param>
    /// <param name="occurrence">Which occurrence of the <paramref name="dayOfRecurrence"/> the item recurs on (e.g. first, last).</param>
    /// <param name="dayOfRecurrence">The days counted by <paramref name="occurrence"/> (e.g. weekdays, Fridays).</param>
    public YearlyPatternRecurrencePattern(int yearlyInterval, int month, Occurrence occurrence, DayOfRecurrence dayOfRecurrence)
      : base(yearlyInterval, month)
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
    protected override bool Includes(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount)
    {
      DateTime date = day.Date;
      if (date < startDate)
      {
        return false;
      }

      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(12 * YearlyInterval, Occurrence, DayOfRecurrence);

      int startDay = 1;
      if (startDate.Month == Month)
      {
        startDay = startDate.Day;
      }
      DateTime firstDay = new DateTime(startDate.Year, Month, startDay);
      if (firstDay < startDate)
      {
        firstDay = new DateTime(startDate.Year + YearlyInterval, Month, 1);
      }
      if (limitOccurrences && occurrenceCount.HasValue)
      {
        return pattern.Includes(firstDay, day, occurrenceCount.Value, DayOfWeek.Monday);
      }
      return pattern.Includes(firstDay, day, DayOfWeek.Monday);
    }
  }
}
