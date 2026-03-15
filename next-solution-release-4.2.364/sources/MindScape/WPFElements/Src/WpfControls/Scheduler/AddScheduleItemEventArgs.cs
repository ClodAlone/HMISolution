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
  /// Contains event data relating to adding a <see cref="ScheduleItem"/> to a schedule.
  /// </summary>
  public class AddScheduleItemEventArgs : ScheduleItemEventArgs
  {
    internal AddScheduleItemEventArgs(ScheduleItem item)
      : base(item)
    {
    }

    /// <summary>
    /// Gets or sets whether to cancel adding the item.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets or sets whether to show the default editor (the <see cref="ScheduleItemDialog"/>.
    /// The default is that the default editor will be shown if the event
    /// is unhandled, but will be suppressed if the event is handled.  Event handlers
    /// can set this property to true to override this behavior.
    /// </summary>
    public bool ShowDefaultEditor { get; set; }
  }
}
