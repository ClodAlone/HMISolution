using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Specifies how the user can cause a data grid cell to go into edit mode using the mouse.
  /// </summary>
  public enum DataGridCellEditModeBehavior
  {
    /// <summary>
    /// Data grid cells will be editable after the user clicks on them.
    /// </summary>
    OnClick,

    /// <summary>
    /// Clicking on a highlighted data grid cell will cause it to go into edit mode.
    /// Cells can be highlighted by clicking on them with the mouse, or by using navigation keys to change which cell is highlighted.
    /// </summary>
    OnClickHighlightedCell
  }
}
