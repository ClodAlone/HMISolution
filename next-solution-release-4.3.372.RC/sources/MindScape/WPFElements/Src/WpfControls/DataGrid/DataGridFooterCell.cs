using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Represents a cell on a <see cref="DataGridFooter"/> lined up with a particular column.
  /// </summary>
  public class DataGridFooterCell : ViewModelBase
  {
    private readonly DataGridColumn _column;

    internal DataGridFooterCell(DataGridColumn column)
    {
      _column = column;
    }

    /// <summary>
    /// Gets the <see cref="DataGridColumn"/>.
    /// </summary>
    public DataGridColumn Column
    {
      get { return _column; }
    }

    /// <summary>
    /// Gets the <see cref="IAggregate"/> for displaying in the footer of the <see cref="DataGridColumn"/>.
    /// </summary>
    public IAggregate Aggregate
    {
      get { return _column.FooterAggregate; }
    }
  }
}
