using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A row element that can be displayed in a <see cref="DataGrid"/>.
  /// </summary>
  public interface IDataGridRow
  {
    /// <summary>
    /// Gets the display index of this row on the current page.
    /// </summary>
    int Index { get; }
  }
}
