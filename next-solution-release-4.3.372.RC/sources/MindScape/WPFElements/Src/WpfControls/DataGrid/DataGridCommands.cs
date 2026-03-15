using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A class containing commands used by a <see cref="DataGrid"/>.
  /// </summary>
  public static class DataGridCommands
  {
    #region Page commands

    // TODO: wait! can't these just be NavigationCommands?

    /// <summary>
    /// A command for navigating to the next page of data.
    /// </summary>
    public static readonly RoutedCommand NextPage = new RoutedCommand("NextPage", typeof(DataGrid));

    /// <summary>
    /// A command for navigating to the prevous page of data.
    /// </summary>
    public static readonly RoutedCommand PreviousPage = new RoutedCommand("PreviousPage", typeof(DataGrid));

    /// <summary>
    /// A command for navigating to the first page of data.
    /// </summary>
    public static readonly RoutedCommand FirstPage = new RoutedCommand("FirstPage", typeof(DataGrid));

    /// <summary>
    /// A command for navigating to the last page of data.
    /// </summary>
    public static readonly RoutedCommand LastPage = new RoutedCommand("LastPage", typeof(DataGrid));

    #endregion // Page commands

    #region Basic cell navigation commands

    // TODO: not sure about these command names:

    /// <summary>
    /// A command for navigating the highlighted cell up one row.
    /// </summary>
    public static readonly RoutedCommand HighlightUp = new RoutedCommand("HighlightUp", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell down one row.
    /// </summary>
    public static readonly RoutedCommand HighlightDown = new RoutedCommand("HighlightDown", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell left one column.
    /// </summary>
    public static readonly RoutedCommand HighlightLeft = new RoutedCommand("HighlightLeft", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell right one column.
    /// </summary>
    public static readonly RoutedCommand HighlightRight = new RoutedCommand("HighlightRight", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell left one column and wrapping to the next row if necessary.
    /// </summary>
    public static readonly RoutedCommand HighlightLeftWithWrapping = new RoutedCommand("HighlightLeftWithWrapping", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell right one column and wrapping to the previous row if necessary.
    /// </summary>
    public static readonly RoutedCommand HighlightRightWithWrapping = new RoutedCommand("HighlightRightWithWrapping", typeof(DataGrid));

    #endregion // Basic cell navigation commands

    #region Selection Commands

    /// <summary>
    /// A command for adding the next cell up to the selection.
    /// </summary>
    public static readonly RoutedCommand SelectUp = new RoutedCommand("SelectUp", typeof(DataGrid));

    /// <summary>
    /// A command for adding the next cell down to the selection.
    /// </summary>
    public static readonly RoutedCommand SelectDown = new RoutedCommand("SelectDown", typeof(DataGrid));

    /// <summary>
    /// A command for toggling the selection of all items.
    /// </summary>
    public static readonly RoutedCommand ToggleSelectAll = new RoutedCommand("ToggleSelectAll", typeof(DataGrid));

    #endregion // Selection Commands

    #region Cell navigation commands

    /// <summary>
    /// A command for navigating the highlighted cell to the start of the row.
    /// </summary>
    public static readonly RoutedCommand StartOfRow = new RoutedCommand("StartOfRow", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell to the first cell in the grid.
    /// </summary>
    public static readonly RoutedCommand StartOfGrid = new RoutedCommand("StartOfGrid", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell to the end of the row.
    /// </summary>
    public static readonly RoutedCommand EndOfRow = new RoutedCommand("EndOfRow", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell to the last cell in the grid.
    /// </summary>
    public static readonly RoutedCommand EndOfGrid = new RoutedCommand("EndOfGrid", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell up by the number of rows that currently fit within the view port.
    /// </summary>
    public static readonly RoutedCommand PageUp = new RoutedCommand("PageUp", typeof(DataGrid));

    /// <summary>
    /// A command for navigating the highlighted cell down by the number of rows that currently fit within the view port.
    /// </summary>
    public static readonly RoutedCommand PageDown = new RoutedCommand("PageDown", typeof(DataGrid));

    #endregion // Cell navigation commands

    /// <summary>
    /// A command for switching the current cell into edit mode.
    /// </summary>
    public static readonly RoutedCommand EnterEditMode = new RoutedCommand("EnterEditMode", typeof(DataGrid));

    /// <summary>
    /// A command for canceling cell edit mode and reverting any value changes.
    /// </summary>
    public static readonly RoutedCommand CancelEditMode = new RoutedCommand("CancelEditMode", typeof(DataGrid));

    /// <summary>
    /// A command for ungrouping a data grid column.
    /// </summary>
    public static readonly RoutedCommand UngroupColumn = new RoutedCommand("UngroupColumn", typeof(DataGrid));

    /// <summary>
    /// A command for changing the expanded state of hierarchical items.
    /// </summary>
    public static readonly RoutedCommand ToggleExpandedState = new RoutedCommand("ToggleExpandedState", typeof(DataGrid));

    /// <summary>
    /// A command for removing the filter from a <see cref="DataGridColumn"/>.
    /// </summary>
    public static readonly RoutedCommand RemoveFilter = new RoutedCommand("RemoveFilter", typeof(DataGrid));
  }
}
