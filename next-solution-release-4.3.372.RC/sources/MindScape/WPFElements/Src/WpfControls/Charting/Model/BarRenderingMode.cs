using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Identifies how bars should be rendered.
  /// </summary>
  public enum BarRenderingMode
  {
    /// <summary>
    /// Bars from different series in the same X position will be overlapped.
    /// </summary>
    Overlap,

    /// <summary>
    /// Bars from different series in the same X position will be rendered side-by-side.
    /// </summary>
    SideBySide
  }
}
