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
  /// Specifies whether to show a full week in Week view, or only the work
  /// week (Monday to Friday).
  /// </summary>
  public enum WeekViewMode
  {
    /// <summary>
    /// Show only the work week (Monday to Friday).
    /// </summary>
    WorkWeek,

    /// <summary>
    /// Show the full week.
    /// </summary>
    FullWeek
  }
}
