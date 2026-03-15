using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Holds information about the DataGrid HighlightedCellChanging event.
  /// </summary>
  public class HighlightedCellChangingEventArgs : RoutedEventArgs
  {
    internal HighlightedCellChangingEventArgs(bool isCurrentCellValiid)
    {
      IsCurrentCellValid = isCurrentCellValiid;
    }

    /// <summary>
    /// Gets or sets whethor or not to cancel the highlighted cell changing operation.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets whether or not the current highlighted cell holds valid data.
    /// </summary>
    public bool IsCurrentCellValid { get; private set; }
  }
}
