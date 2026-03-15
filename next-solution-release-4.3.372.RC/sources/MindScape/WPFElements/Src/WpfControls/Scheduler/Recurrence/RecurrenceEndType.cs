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
  /// Specifies when a recurring schedule item will end.
  /// </summary>
  public enum RecurrenceEndType
  {
    /// <summary>
    /// States that a recurrence has no end date.
    /// </summary>
    NoEndDate,

    /// <summary>
    /// States that a recurrence can last for some maximum number of occurrences.
    /// </summary>
    EndAfter,

    /// <summary>
    /// States that a recurrence must end by a particular date.
    /// </summary>
    EndBy
  }
}
