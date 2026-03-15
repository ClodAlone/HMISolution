using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Contains event information for when a hierarchical <see cref="DataGrid"/> item is requesting to be expanded or collapsed.
  /// </summary>
  public class DataGridHierarchicalItemStateChangingEventArgs : EventArgs
  {
    private readonly object _item;
    private readonly bool _isExpanding;
    private bool _canChangeState = true;

    internal DataGridHierarchicalItemStateChangingEventArgs(object item, bool isExpanding)
    {
      _item = item;
      _isExpanding = isExpanding;
    }

    /// <summary>
    /// Gets the hierarchical data item that is changing state.
    /// </summary>
    public object Item { get { return _item; } }

    /// <summary>
    /// Gets whether or not the hirarchical item is requesting to be expanded.
    /// </summary>
    public bool IsExpanding { get { return _isExpanding; } }

    /// <summary>
    /// Gets or sets whether or not the hierarchical item is allowed to change state.
    /// </summary>
    public bool CanChangeState
    {
      get { return _canChangeState; }
      set
      {
        _canChangeState = value;
      }
    }
  }
}
