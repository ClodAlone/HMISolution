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
  /// Provides information about the result of creating and adding a schedule item to a <see cref="Schedule"/>.
  /// </summary>
  public class CreateScheduleItemResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateScheduleItemResult"/> class.
    /// </summary>
    /// <param name="item">The new schedule item. This can be null if no schedule item was added to the schedule.</param>
    /// <param name="addDefaultItem">Whether or not to add a default schedule item to the schedule.</param>
    public CreateScheduleItemResult(ScheduleItem item, bool addDefaultItem)
    {
      Item = item;
      AddDefaultItem = addDefaultItem;
    }

    /// <summary>
    /// Gets the schedule item that was created and added to the schedule. This can be null if no schedule item was added.
    /// </summary>
    public ScheduleItem Item { get; private set; }

    /// <summary>
    /// Gets whether or not to add a default schedule item to the schedule.
    /// </summary>
    public bool AddDefaultItem { get; private set; }
  }
}
