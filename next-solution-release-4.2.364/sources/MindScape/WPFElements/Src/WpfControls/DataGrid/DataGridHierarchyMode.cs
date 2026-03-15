using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Specifies the default display of hierarchical data loaded in a <see cref="DataGrid"/>.
  /// </summary>
  public enum DataGridHierarchyMode
  {
    /// <summary>
    /// Hierarchical items are initially collapsed when loaded in a <see cref="DataGrid"/>.
    /// </summary>
    Collapsed,

    /// <summary>
    /// Hierarchical items are initially expanded when loaded in a <see cref="DataGrid"/>.
    /// </summary>
    Expanded
  }
}
