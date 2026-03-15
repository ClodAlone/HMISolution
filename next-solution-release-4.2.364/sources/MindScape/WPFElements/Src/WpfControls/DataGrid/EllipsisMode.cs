using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Specifies ellipsis mode of a <see cref="DataGridPager"/>.
  /// </summary>
  public enum EllipsisMode
  {
    /// <summary>
    /// The <see cref="DataGridPager"/> does not display an ellipsis.
    /// </summary>
    None,

    /// <summary>
    /// The <see cref="DataGridPager"/> displays an ellipsis on the first pager button if necessary.
    /// </summary>
    Before,

    /// <summary>
    /// The <see cref="DataGridPager"/> displays an ellipsis on the last pager button if necessary.
    /// </summary>
    After,

    /// <summary>
    /// The <see cref="DataGridPager"/> displays an ellipsis on the first and the last pager buttons where necessary.
    /// </summary>
    Both
  }
}
