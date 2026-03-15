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
  /// Specifies status information used for rendering a <see cref="TimeSlot"/>.
  /// </summary>
  public enum TimeSlotState
  {
    /// <summary>
    /// The timeslot is unselected and within working hours.
    /// </summary>
    WorkTime,

    /// <summary>
    /// The timeslot is unselected and outside working hours.
    /// </summary>
    NotWorkTime,

    /// <summary>
    /// The timeslot is selected and within working hours.
    /// </summary>
    SelectedWorkTime,

    /// <summary>
    /// The timeslot is selected and outside working hours.
    /// </summary>
    SelectedNotWorkTime
  }
}
