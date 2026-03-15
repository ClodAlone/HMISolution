using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Contains event information for when a cell is requesting to validate its value.
  /// </summary>
  public class DataGridValidateCellEventArgs : EventArgs
  {
    internal DataGridValidateCellEventArgs(DataGridCell cell)
    {
      Cell = cell;
      IsValid = true;
      ValidationMessage = null;
    }

    /// <summary>
    /// Gets the <see cref="DataGridCellContainer"/> being validated.
    /// </summary>
    public DataGridCell Cell { get; private set; }

    /// <summary>
    /// Gets or sets a validation message.
    /// </summary>
    public string ValidationMessage { get; set; }

    /// <summary>
    /// Gets or sets if the cell value is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets whether the event was handled.  The <see cref="DataGrid"/> sets this
    /// by default.
    /// </summary>
    public bool Handled { get; set; }
  }
}
