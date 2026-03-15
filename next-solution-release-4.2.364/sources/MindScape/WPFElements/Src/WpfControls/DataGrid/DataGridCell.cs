using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  // TODO: given that DataGridCellContainer represents the actual physical cell control,
  // maybe that should be called DataGridCell and this should be something like
  // DataGridCellLocation?

  /// <summary>
  /// Represents the location of a cell in a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridCell
  {
    private object _rowContent;
    private DataGridColumn _column;
    private int _rowIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridCell"/> class.
    /// </summary>
    /// <param name="rowContent">The content of the row that the cell is within.</param>
    /// <param name="column">The <see cref="DataGridColumn"/> that the cell is within.</param>
    public DataGridCell(object rowContent, DataGridColumn column)
    {
      _rowContent = rowContent;
      _column = column;
    }

    /// <summary>
    /// Returns a <see cref="DataGridCell"/> model object at the given row and column.
    /// </summary>
    /// <param name="rowContent">The data object displayed by the row that the cell is in.</param>
    /// <param name="column">The <see cref="DataGridColumn"/> that the cell is in.</param>
    /// <returns>A <see cref="DataGridCell"/> at the given row and column.</returns>
    public static DataGridCell GetCell(object rowContent, DataGridColumn column)
    {
      return new DataGridCell(rowContent, column);
    }

    internal static DataGridCell GetCell(DataGridCellContainer cellContainer)
    {
      if (cellContainer != null)
      {
        return new DataGridCell(cellContainer.Content, cellContainer.Column) { RowIndex = cellContainer.Row.Index };
      }
      return null;
    }

    /// <summary>
    /// Gets the content of the row that the cell is within.
    /// </summary>
    public object RowContent
    {
      get { return _rowContent; }
    }

    /// <summary>
    /// Gets the <see cref="DataGridColumn"/> that the cell is within.
    /// </summary>
    public DataGridColumn Column
    {
      get { return _column; }
    }

    /// <summary>
    /// Gets the value of the cell.
    /// </summary>
    public object Value
    {
      get
      {
        if (RowContent != null && Column != null && Column.PropertyInfo != null)
        {
          return Column.PropertyInfo.GetValue(RowContent, System.Reflection.BindingFlags.GetProperty, null, null, null);
        }
        return null;
      }
    }

    internal int RowIndex
    {
      get { return _rowIndex; }
      set
      {
        _rowIndex = value;
      }
    }
  }
}
