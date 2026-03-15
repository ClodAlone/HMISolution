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
  /// Provides data for an event pertaining to a <see cref="ScheduleItem"/>.
  /// </summary>
  public class ScheduleItemEventArgs : EventArgs
  {
    private readonly ScheduleItem _item;

    internal ScheduleItemEventArgs(ScheduleItem item)
    {
      _item = item;
    }

    /// <summary>
    /// The <see cref="ScheduleItem"/> to which the event pertains.
    /// </summary>
    public ScheduleItem Item
    {
      get { return _item; }
    }
  }
}
