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
  /// Implementations of <see cref="IScheduleItemBuilder"/> can add custom schedule items to a schedule.
  /// </summary>
  public interface IScheduleItemBuilder
  {
    /// <summary>
    /// Creates a custom schedule item and adds it to the schedule found in the given args.
    /// </summary>
    /// <param name="args"><see cref="CreateScheduleItemArgs"/> holding information about creating and adding an item to the schedule.</param>
    /// <returns><see cref="CreateScheduleItem"/> holding information about the result of creating and adding an item to the schedule.</returns>
    CreateScheduleItemResult CreateScheduleItem(CreateScheduleItemArgs args);
  }
}
