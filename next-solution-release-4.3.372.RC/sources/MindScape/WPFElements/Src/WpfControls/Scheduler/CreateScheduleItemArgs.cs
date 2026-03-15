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
  /// Provides information about creating and adding a schedule item to a <see cref="Schedule"/>.
  /// </summary>
  public class CreateScheduleItemArgs
  {
    internal CreateScheduleItemArgs(Schedule schedule, ScheduleItemCreationType creationType, DateTime start, DateTime end, string name)
    {
      Schedule = schedule;
      CreationType = creationType;
      StartTime = start;
      EndTime = end;
      ScheduleItemName = name;
    }

    /// <summary>
    /// Gets the <see cref="Schedule"/> for adding the new schedule item.
    /// </summary>
    public Schedule Schedule { get; private set; }

    /// <summary>
    /// Specifies what triggered the creation of a schedule item.
    /// </summary>
    public ScheduleItemCreationType CreationType { get; private set; }

    /// <summary>
    /// The potential start time of the new schedule item.
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// The potential end time of the new schedule item.
    /// </summary>
    public DateTime EndTime { get; private set; }

    /// <summary>
    /// The potential name of the new schedule item.
    /// </summary>
    public string ScheduleItemName { get; private set; }
  }
}
