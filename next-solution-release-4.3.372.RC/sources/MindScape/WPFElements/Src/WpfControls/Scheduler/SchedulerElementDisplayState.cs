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
  /// Contains display information for a <see cref="SchedulerElement"/>.
  /// </summary>
  public struct SchedulerElementDisplayState
  {
    private readonly bool _isScheduleItemStartVisible;
    private readonly bool _isScheduleItemEndVisible;
    private readonly DateTime _scheduleItemStart;
    private readonly DateTime _scheduleItemEnd;
    private readonly WeakReference _scheduleItem;  // WPF seems to hang on to SEDS objects in certain cases -- TODO: find out when and why

    internal SchedulerElementDisplayState(SchedulerElement element)
    {
      _isScheduleItemStartVisible = element.IsScheduleItemStartVisible;
      _isScheduleItemEndVisible = element.IsScheduleItemEndVisible;
      _scheduleItemStart = element.ScheduleItem.StartTime;
      _scheduleItemEnd = element.ScheduleItem.EndTime;
      _scheduleItem = new WeakReference(element.ScheduleItem);
    }

    /// <summary>
    /// Gets whether the start of the item is on a displayed date.
    /// </summary>
    public bool IsScheduleItemStartVisible { get { return _isScheduleItemStartVisible; } }

    /// <summary>
    /// Gets whether the end of the item is on a displayed date.
    /// </summary>
    public bool IsScheduleItemEndVisible { get { return _isScheduleItemEndVisible; } }

    /// <summary>
    /// Gets the start time of the item.
    /// </summary>
    public DateTime ScheduleItemStartTime { get { return _scheduleItemStart; } }

    /// <summary>
    /// Gets the end time of the item.
    /// </summary>
    public DateTime ScheduleItemEndTime { get { return _scheduleItemEnd; } }

    /// <summary>
    /// Gets the <see cref="ScheduleItem"/>.
    /// </summary>
    public ScheduleItem ScheduleItem { get { return _scheduleItem.IsAlive ? (ScheduleItem)(_scheduleItem.Target) : null; } }
  }
}
