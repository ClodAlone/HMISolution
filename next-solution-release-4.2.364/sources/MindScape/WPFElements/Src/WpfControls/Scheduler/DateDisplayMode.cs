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
  /// Specifies whether dates should be formatted with the day of the month
  /// or as the month name only.
  /// </summary>
  public enum DateDisplayMode
  {
    /// <summary>
    /// Dates should be formatted with the day of the month.
    /// </summary>
    Day,

    /// <summary>
    /// Dates should be formatted using the month name only.
    /// </summary>
    Month
  }
}
