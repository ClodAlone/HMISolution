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
  /// Provides status information for use in rendering a <see cref="MonthViewDayElement"/>.
  /// </summary>
  public enum ElementViewStatus
  {
    /// <summary>
    /// The element is not in any of the other states.
    /// </summary>
    Normal,

    /// <summary>
    /// The element is selected.
    /// </summary>
    Selected,

    /// <summary>
    /// The mouse is over the element.
    /// </summary>
    MouseOver,

    /// <summary>
    /// The element represents a day which is not part of the month being displayed.
    /// </summary>
    Padding,

    /// <summary>
    /// The mouse is over the element which also represents a day which is not part of the month being displayed.
    /// </summary>
    MouseOverPadding
  }
}
