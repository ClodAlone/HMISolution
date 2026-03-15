using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Represents a cell on a <see cref="DataGrid"/> group header lined up with a particular column.
  /// </summary>
  public class GroupHeaderCell
  {
    private readonly DataGridGroup _group;
    private readonly IAggregate _aggregate;
    private readonly DataGridColumn _column;

    internal GroupHeaderCell(DataGridGroup group, DataGridColumn column, IAggregate aggregate)
    {
      _group = group;
      _column = column;
      _aggregate = aggregate;
    }

    /// <summary>
    /// Gets the <see cref="DataGridGroup"/>.
    /// </summary>
    public DataGridGroup Group
    {
      get { return _group; }
    }

    /// <summary>
    /// Gets the <see cref="DataGridColumn"/>.
    /// </summary>
    public DataGridColumn Column
    {
      get { return _column; }
    }

    /// <summary>
    /// gets the <see cref="IAggregate"/>.
    /// </summary>
    public IAggregate Aggregate
    {
      get { return _aggregate; }
    }
  }
}
