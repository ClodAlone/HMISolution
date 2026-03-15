using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Defines the work hours of a <see cref="Scheduler"/> control.
  /// </summary>
  public class WorkHours : ViewModelBase
  {
    private TimeOfDay _startTime;
    private TimeOfDay _endTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkHours"/> class.
    /// </summary>
    public WorkHours() : this(new TimeOfDay(9), new TimeOfDay(17)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkHours"/> class using the given start and end times.
    /// </summary>
    /// <param name="startTime">The start time of the WorkHours.</param>
    /// <param name="endTime">The end time of the WorkHours.</param>
    public WorkHours(TimeOfDay startTime, TimeOfDay endTime)
    {
      _startTime = startTime;
      _endTime = endTime;
    }

    /// <summary>
    /// Gets or sets the starting work time. The default is 9:00am.
    /// </summary>
    public TimeOfDay StartTime
    {
      get { return _startTime; }
      set { Set(ref _startTime, value, "StartTime"); }
    }

    /// <summary>
    /// Gets or sets the finishing work time. The default is 5:00pm.
    /// </summary>
    public TimeOfDay EndTime
    {
      get { return _endTime; }
      set { Set(ref _endTime, value, "EndTime"); }
    }
  }
}
