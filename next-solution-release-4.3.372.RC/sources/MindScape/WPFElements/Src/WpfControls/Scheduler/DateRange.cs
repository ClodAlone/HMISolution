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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents a range of dates.
  /// </summary>
  public struct DateRange
  {
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    internal DateRange(ScheduleItem scheduleItem)
    {
      _startDate = scheduleItem.StartTime;
      _endDate = scheduleItem.EndTime;
    }

    internal DateRange(DateTime startDate, DateTime endDate)
    {
      _startDate = startDate;
      _endDate = endDate;
    }

    /// <summary>
    /// Gets the start of the range.
    /// </summary>
    public DateTime StartDate
    {
      get { return _startDate; }
    }

    /// <summary>
    /// Gets the end of the range.
    /// </summary>
    public DateTime EndDate
    {
      get { return _endDate; }
    }

    internal bool Contains(DateRange other)
    {
      return other.StartDate >= StartDate && other.EndDate <= EndDate;
    }
  }
}
