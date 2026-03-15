using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides data for row-related <see cref="DataGrid"/> events.
  /// </summary>
  public class DataGridRowEventArgs
  {
    private readonly object _rowContent;

    internal DataGridRowEventArgs(object rowContent)
    {
      _rowContent = rowContent;
    }

    /// <summary>
    /// Gets the content of the row associated with the event.
    /// </summary>
    public object RowContent
    {
      get { return _rowContent; }
    }
  }
}
