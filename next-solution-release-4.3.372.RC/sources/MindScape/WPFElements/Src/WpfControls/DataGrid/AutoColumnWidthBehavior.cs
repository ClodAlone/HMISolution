using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Defines the behavior of automatic column widths.
  /// </summary>
  public enum AutoColumnWidthBehavior
  {
    /// <summary>
    /// The column widths are calculated once whenever the items source changes.
    /// </summary>
    OneTime,

    /// <summary>
    /// The column widths are constantally recalculated as the data scrolls.
    /// </summary>
    Dynamic
  }
}
