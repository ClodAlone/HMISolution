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
using System.Collections.Specialized;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides event data for when a <see cref="ScheduleItem"/> is added to or removed
  /// from a collection.
  /// </summary>
  public class ScheduleItemCollectionChangedEventArgs : ScheduleItemEventArgs
  {
    private readonly NotifyCollectionChangedAction _action;
    private readonly bool _isLong;

    internal ScheduleItemCollectionChangedEventArgs(ScheduleItem item, NotifyCollectionChangedAction action, bool isLong)
      : base(item)
    {
      _action = action;
      _isLong = isLong;
    }

    /// <summary>
    /// Whether the <see cref="ScheduleItem"/> is being added to or
    /// removed from the collection.
    /// </summary>
    public NotifyCollectionChangedAction Action
    {
      get { return _action; }
    }

    internal bool IsLong
    {
      get { return _isLong; }
    }
  }
}
