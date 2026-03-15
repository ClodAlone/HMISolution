using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Holds information about the WidthChanged event of a <see cref="DataGridColumn"/>.
  /// </summary>
  public class ColumnWidthChangedEventArgs : EventArgs
  {
    internal ColumnWidthChangedEventArgs(double oldWidth, double newWidth)
    {
      OldWidth = oldWidth;
      NewWidth = newWidth;
    }

    /// <summary>
    /// Gets the width of the <see cref="DataGridColumn"/> before the width was changed.
    /// </summary>
    public double OldWidth { get; private set; }

    /// <summary>
    /// Gets the new width of the <see cref="DataGridColumn"/>.
    /// </summary>
    public double NewWidth { get; private set; }
  }
}
