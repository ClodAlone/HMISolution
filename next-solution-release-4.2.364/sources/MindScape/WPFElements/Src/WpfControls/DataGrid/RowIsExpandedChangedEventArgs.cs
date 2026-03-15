using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Event arguments for when a row in a <see cref="DataGrid"/> is expanded or collapsed.
  /// </summary>
  public class RowIsExpandedChangedEventArgs : EventArgs
  {
    private readonly object _item;
    private readonly bool _isExpanded;

    internal RowIsExpandedChangedEventArgs(object item, bool isExpanded)
    {
      _item = item;
      _isExpanded = isExpanded;
    }

    /// <summary>
    /// Gets the item that expanded or collapsed.
    /// </summary>
    public object Item
    {
      get { return _item; }
    }

    /// <summary>
    /// Gets whether or not the item is expanded.
    /// </summary>
    public bool IsExpanded
    {
      get { return _isExpanded; }
    }
  }
}
