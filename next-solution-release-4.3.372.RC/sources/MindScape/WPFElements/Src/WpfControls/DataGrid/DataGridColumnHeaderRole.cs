using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Defines the state or role of a <see cref="DataGridColumnHeader"/> control.
  /// </summary>
  public enum DataGridColumnHeaderRole
  {
    /// <summary>
		/// The column header displays above its associated column.
		/// </summary>
		Normal,

		/// <summary>
		/// The column header is the object of a drag-and-drop operation to move a column.
		/// </summary>
		Floating,

		/// <summary>
		/// The column header is the last header in the row of column headers and is used
		/// for padding.
		/// </summary>
		Padding,

    /// <summary>
    /// The column header is displayed in a <see cref="DataGridGroupingPanel"/> and represents a grouped column.
    /// </summary>
    Grouping
  }
}
