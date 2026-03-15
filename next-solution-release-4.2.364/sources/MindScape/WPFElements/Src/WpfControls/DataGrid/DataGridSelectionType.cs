using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Specifies what users can select in a <see cref="DataGrid"/>.
  /// </summary>
  public enum DataGridSelectionType
  {
    /// <summary>
    /// Users can select rows.
    /// </summary>
    Row,

    /// <summary>
    /// Users can select individual cells.
    /// </summary>
    Cell,

    /// <summary>
    /// Users can select rows by clicking on the row headers, and also click on individual cells to select the cells.
    /// In single selection mode, only 1 row or 1 cell can be selected at any time.
    /// Multiple and Extended selection modes allow any number of rows and cells to be selected.
    /// </summary>
    RowAndCell
  }
}
