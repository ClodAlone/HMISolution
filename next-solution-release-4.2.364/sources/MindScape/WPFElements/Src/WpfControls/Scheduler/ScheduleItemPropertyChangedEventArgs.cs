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
  /// Contains event data relating to changing the value of one of the properties of a <see cref="ScheduleItem"/>.
  /// </summary>
  public class ScheduleItemPropertyChangedEventArgs : ScheduleItemEventArgs
  {
    private readonly string _propertyName;

    internal ScheduleItemPropertyChangedEventArgs(ScheduleItem item, string propertyName)
      : base(item)
    {
      _propertyName = propertyName;
    }

    /// <summary>
    /// Gets the name of the property that changed.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }
  }
}
