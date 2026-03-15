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
  /// basis on specific day of the year (e.g. every 25th of April).
  /// </summary>
  public class SpecificDateYearlyRecurrencePattern : YearlyRecurrencePattern
  {
    private readonly int _dayOfMonth;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificDateYearlyRecurrencePattern"/> class.
    /// </summary>
    /// <param name="yearlyInterval">The number of years between recurrences.</param>
    /// <param name="month">The month in which the item recurs. (1 through 12)</param>
    /// <param name="dayOfMonth">The day of the month on which the item occurs.</param>
    public SpecificDateYearlyRecurrencePattern(int yearlyInterval, int month, int dayOfMonth)
      : base(yearlyInterval, month)
    {
      _dayOfMonth = dayOfMonth;
    }

    /// <summary>
    /// Gets the day of the month on which the item occurs.
    /// </summary>
    public int DayOfMonth
    {
      get { return _dayOfMonth; }
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
      if (date.Month != Month)
      {
        return false;
      }
      if (date.Day != Math.Min(DateTime.DaysInMonth(date.Year, date.Month), DayOfMonth))
      {
        return false;
      }
      float yearlyRatio = date.Year / (float)YearlyInterval;
      if (yearlyRatio - (int)yearlyRatio > 0)
      {
        return false;
      }
      DateTime firstDate = new DateTime(startDate.Year, Month, Math.Min(DayOfMonth, DateTime.DaysInMonth(startDate.Year, Month)));
      if (firstDate < startDate)
      {
        firstDate = new DateTime(startDate.Year + 1, Month, Math.Min(DayOfMonth, DateTime.DaysInMonth(startDate.Year + 1, Month)));
      }
      if (limitOccurrences && occurrenceCount.HasValue)
      {
        int lastYear = firstDate.Year + ((occurrenceCount.Value - 1) * YearlyInterval);
        DateTime lastDate = new DateTime(lastYear, Month, DateTime.DaysInMonth(lastYear, Month), 23, 59, 59);
        if (lastDate < date)
        {
          return false;
        }
      }
      return true;
    }
  }
}
