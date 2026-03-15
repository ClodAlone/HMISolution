using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Specifies the sort direction of a <see cref="DataGridColumn"/>.
  /// </summary>
  public enum SortDirection
  {
    /// <summary>
    /// Items are sorted in an ascending order. (smallest to largest or alphabetical)
    /// </summary>
    Ascending,

    /// <summary>
    /// Items are sorted in a descending order. (largest to smallest or reverse alphabetical)
    /// </summary>
    Descending,

    /// <summary>
    /// Items are not sorted.
    /// </summary>
    None
  }
}
