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
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on a monthly
  /// basis on specific day of the month (e.g. the 20th of every third month).
  /// </summary>
  public class NthDayOfMonthRecurrencePattern : MonthlyRecurrencePattern
  {
    private readonly int _dayOfMonth = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="NthDayOfMonthRecurrencePattern"/> class.
    /// </summary>
    /// <param name="monthlyInterval">The number of months between recurrences.</param>
    /// <param name="dayOfMonth">The day of the month on which the item occurs.</param>
    public NthDayOfMonthRecurrencePattern(int monthlyInterval, int dayOfMonth)
      : base(monthlyInterval)
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
    protected override bool IncludesCore(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount)
    {
      int year = startDate.Year;
      int month = startDate.Month;
      int dayOfMonth = startDate.Day;
      if (dayOfMonth > _dayOfMonth)
      {
        ++month;
        if (month > 12)
        {
          month = 1;
          ++year;
        }
      }
      dayOfMonth = Math.Min(_dayOfMonth, DateTime.DaysInMonth(year, month));
      DateTime firstDate = new DateTime(year, month, dayOfMonth);

      int months = DateTimeUtils.MonthsBetween(firstDate, day);
      if (months % MonthlyInterval != 0)
      {
        return false;
      }

      if (day.Day != Math.Min(_dayOfMonth, DateTime.DaysInMonth(day.Year, day.Month)))
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
  }
}
