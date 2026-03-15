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
using System.Windows.Threading;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  internal static class SchedulerUpdateTimer
  {
    private static DispatcherTimer _timer;

    static SchedulerUpdateTimer()
    {
      _timer = new DispatcherTimer();
      _timer.Interval = new TimeSpan(0, 1, 0);
      _timer.Start();
    }

    public static DispatcherTimer MinuteTimer { get { return _timer; } }
  }
}
