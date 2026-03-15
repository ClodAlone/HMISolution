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
  /// Specifies which end (start or finish) of a schedule item is being referred to.
  /// </summary>
  public enum TimeEnd
  {
    /// <summary>
    /// The start of a schedule item.
    /// </summary>
    StartTime,

    /// <summary>
    /// The end of a schedule item.
    /// </summary>
    EndTime
  }
}
