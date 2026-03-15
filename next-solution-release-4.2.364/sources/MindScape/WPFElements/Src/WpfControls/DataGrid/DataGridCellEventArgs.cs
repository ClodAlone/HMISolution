using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Contains event information for when mouse over occurs on a cell.
  /// </summary>
  public class DataGridCellEventArgs
  {
    internal DataGridCellEventArgs(DataGridCell cell)
    {
      Cell = cell;
    }

    /// <summary>
    /// Gets the <see cref="DataGridCellContainer"/> that the event relates to.
    /// </summary>
    public DataGridCell Cell { get; private set; }
  }
}
