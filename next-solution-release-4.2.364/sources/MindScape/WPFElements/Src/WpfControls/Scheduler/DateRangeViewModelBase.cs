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
namespace Mindscape.SilverlightElements.Internal
#else
namespace Mindscape.WpfElements.Internal
#endif
{
#pragma warning disable 1591  // XML documentation comments

  /// <summary>
  /// This class supports the <see cref="Scheduler"/> control and is not intended for use from your code.
  /// </summary>
  public abstract class DateRangeViewModelBase : ViewModelBase, IScheduleViewModel
  {
    public abstract bool HasItems();

    public DateRangeDisplayInfo DateRangeDisplayInfo
    {
      get
      {
        return new DateRangeDisplayInfo(StartDateCore, EndDateCore, DateRangeDisplayMode);
      }
    }

    protected abstract DateTime StartDateCore { get; }
    protected abstract DateTime EndDateCore { get; }
    protected abstract DateDisplayMode DateRangeDisplayMode { get; }
    //protected abstract DayOfWeek FirstDayOfWeek { get; }
  }
}
