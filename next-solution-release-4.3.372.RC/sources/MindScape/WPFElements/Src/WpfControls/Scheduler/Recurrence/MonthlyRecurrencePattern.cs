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
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on a monthly basis.
  /// </summary>
  public abstract class MonthlyRecurrencePattern : RecurrencePattern
  {
    private readonly int _monthlyInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonthlyRecurrencePattern"/> class.
    /// </summary>
    /// <param name="monthlyInterval">The number of months between recurrences.</param>
    protected MonthlyRecurrencePattern(int monthlyInterval)
    {
      _monthlyInterval = monthlyInterval;
    }

    /// <summary>
    /// Gets the number of months between recurrences.
    /// </summary>
    public int MonthlyInterval
    {
      get { return _monthlyInterval; }
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
      TimeSpan span = date - startDate;
      if (span.Days < 0)
      {
        return false;
      }

      return IncludesCore(startDate, day, limitOccurrences, occurrenceCount);
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
    protected abstract bool IncludesCore(DateTime startDate, DateTime day, bool limitOccurrences, int? occurrenceCount);
  }
}
