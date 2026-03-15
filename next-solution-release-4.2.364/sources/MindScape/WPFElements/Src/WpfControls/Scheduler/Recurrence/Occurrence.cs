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
  /// Specifies on which occurrence of a day in a month a recurring schedule item recurs
  /// (as in "last Friday of the month" or "first weekday in January").
  /// This is used in conjunction with the <see cref="DayOfRecurrence"/> type.
  /// </summary>
  public enum Occurrence
  {
    /// <summary>
    /// The item occurs on the first DayOfRecurrence in the month.
    /// </summary>
    First,

    /// <summary>
    /// The item occurs on the second DayOfRecurrence in the month.
    /// </summary>
    Second,

    /// <summary>
    /// The item occurs on the third DayOfRecurrence in the month.
    /// </summary>
    Third,

    /// <summary>
    /// The item occurs on the fourth DayOfRecurrence in the month.
    /// </summary>
    Fourth,

    /// <summary>
    /// The item occurs on the last DayOfRecurrence in the month.
    /// </summary>
    Last
  }
}
