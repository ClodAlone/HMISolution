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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Specifies the location on a <see cref="Chart"/> where the legend is displayed.
  /// </summary>
  public enum LegendPosition
  {
    /// <summary>
    /// The legend is placed to the left of the charting area.
    /// </summary>
    Left,

    /// <summary>
    /// The legend is placed to the right of the charting area.
    /// </summary>
    Right,

    /// <summary>
    /// The legend is placed above the charting area.
    /// </summary>
    Top,

    /// <summary>
    /// The legend is placed below the charting area.
    /// </summary>
    Bottom,

    /// <summary>
    /// The legend is placed within the charting area (though typically not
    /// centered on the chart itself).
    /// </summary>
    Center,

    /// <summary>
    /// The legend is invisible.
    /// </summary>
    None
  }
}
