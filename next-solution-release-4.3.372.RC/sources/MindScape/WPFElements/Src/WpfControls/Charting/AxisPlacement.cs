using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Specifies where an axis is placed around the chart canvas.
  /// </summary>
  public enum AxisPlacement
  {
    /// <summary>
    /// The axis is placed on the left edge of the chart canvas. Only supported by vertical axes.
    /// </summary>
    Left,

    /// <summary>
    /// The axis is placed on the top edge of the chart canvas. Only supported by horizontal axes.
    /// </summary>
    Top,

    /// <summary>
    /// The axis is placed on the right edge of the chart canvas. Only supported by vertical axes.
    /// </summary>
    Right,

    /// <summary>
    /// The axis is placed on the bottom edge of the chart canvas. Only supported by horizontal axes.
    /// </summary>
    Bottom,

    /// <summary>
    /// The axis is placed automatically.
    /// </summary>
    Auto,

    /// <summary>
    /// The axis overlays the charting canvas.
    /// </summary>
    Overlay
  }
}
