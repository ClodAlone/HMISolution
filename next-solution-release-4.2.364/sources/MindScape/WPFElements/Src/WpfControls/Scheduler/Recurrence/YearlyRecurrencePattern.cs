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
  /// Represents the recurrence of a <see cref="ScheduleItem"/> on a yearly basis.
  /// </summary>
  public abstract class YearlyRecurrencePattern : RecurrencePattern
  {
    private readonly int _yearlyInterval;
    private readonly int _month;

    /// <summary>
    /// Initializes a new instance of the <see cref="YearlyRecurrencePattern"/> class.
    /// </summary>
    /// <param name="yearlyInterval">The number of years between recurrences.</param>
    /// <param name="month">The month in which the item recurs. (1 through 12)</param>
    protected YearlyRecurrencePattern(int yearlyInterval, int month)
    {
      _yearlyInterval = yearlyInterval;
      _month = month;
    }

    /// <summary>
    /// Gets the number of years between recurrences.
    /// </summary>
    public int YearlyInterval
    {
      get { return _yearlyInterval; }
    }

    /// <summary>
    /// Gets the month in which the item recurs.
    /// </summary>
    public int Month
    {
      get { return _month; }
    }
  }
}
