//-------------------------------------------------------------------------------------------------
// <copyright file="GridConstants.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines various integer constants to be used throughout the grid.
    /// </summary>
    public class GridConstants
    {
        GridConstants()
        {
        }

        /// <summary>
        /// Use for undefined index.
        /// </summary>
        public const int Undefined = int.MaxValue;

        /// <summary>
        /// Use for maximum row or column count.
        /// </summary>
        public const int MaxRowCol = int.MaxValue - 1;

        internal const int Grid_NXYFACTOR = 1000;
        internal const int Grid_NXAVGWIDTH = 1000;       // logical char width (average)
        internal const int Grid_NYHEIGHT = 1000;         // logical char height
    }

    /// <summary>
    /// Defines the reason for scrolling current cell into view.
    /// </summary>
    public enum GridScrollCurrentCellReason
    {
        /// <summary>
        /// Represents None
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Grid control was activated.
        /// </summary>
        GridFocus = 0x0001,

        /// <summary>
        /// CurrentCell.BeginEdit was called.
        /// </summary>
        BeginEdit = 0x0002,

        /// <summary>
        /// A key was pressed while current cell is active.
        /// </summary>
        KeyPress = 0x0004,

        /// <summary>
        /// Contents of current cell are modified.
        /// </summary>
        Modified = 0x0008,

        /// <summary>
        /// Current cell was activated.
        /// </summary>
        Activate = 0x0010,

        /// <summary>
        /// User clicked into cell.
        /// </summary>
        Click = 0x0020,

        /// <summary>
        /// CurrentCell.MoveTo was called.
        /// </summary>
        MoveTo = 0x0040,

        /// <summary>
        /// An error occurred and a message box will be displayed.
        /// </summary>
        Error = 0x0080,

        /// <summary>
        /// A undo or redo command was executed.
        /// </summary>
        Command = 0x0100,

        /// <summary>
        /// Columns or rows were resized.
        /// </summary>
        ResizedCells = 0x0200,

        /// <summary>
        /// FindDialog has found text in cell.
        /// </summary>
        FindText = 0x0400,

        /// <summary>
        /// Grouping Grid: SynchronizeCurrentCellWithRecord.
        /// </summary>
        SynchronizeRecord = 0x0400,

        /// <summary>
        /// Default: All of above.
        /// </summary>
        Any = 0x0fff,
    }

    /// <summary>
    /// Defines behavior of combo boxes and drop-down list in a cell.
    /// </summary>
    public enum GridDropDownStyle
    {
        /// <summary>
        /// The user can edit the text box contents and is not limited to values existing choices.
        /// </summary>
        Editable = 0,

        /// <summary>
        /// User input is restricted to items from the ChoiceList or DataSource.
        /// </summary>
        Exclusive = 1,

        /// <summary>
        /// The user input is restricted to items from the ChoiceList or DataSource but the user
        /// can type text into the text box and the text box will be filled with a matching choice.
        /// </summary>
        AutoComplete = 3,
    }

    /// <summary>
    /// Specifies whether a cell is asked about support for floating over another cell or
    /// being flooded by a previous cell. See <see cref="GridCellModelBase.OnQueryCanFloatCell"/>.
    /// </summary>
    public enum GridQueryFloatCell
    {
        /// <summary>
        /// Queries if cell supports floating another cell. 
        /// </summary>
        FloatCell,

        /// <summary>
        /// Queries if cell can be flooded by a previous cell.
        /// </summary>
        FloodCell
    }

    /// <summary>
    /// Controls what the grid does when a last or first column in a row.
    /// </summary>
    [Flags]
    public enum GridWrapCellBehavior
    {
        /// <summary>
        /// Don't move current cell.
        /// </summary>
        None = 0,

        /// <summary>
        /// Move to first column in next row or last column in previous row.
        /// </summary>
        WrapRow = 1,

        /// <summary>
        /// When at the last row and last column, move to first row and column or vice versa.
        /// </summary>
        WrapGrid = 2,

        /// <summary>
        /// When at the last row and last column activate next sibling control in the dialog 
        /// or when when at first row and column activate previous sibling control in dialog.
        /// </summary>
        NextControlInForm = 4
    }

    /// <summary>
    /// Specifies which cells to refresh when moving the current cell. When cell's appearance is changed if
    /// cell is moved to a new row (e.g. when GridShowButtons.ShowCurrentRow is used), you should specify
    /// <see cref="GridRefreshCurrentCellBehavior.RefreshRow"/>.
    /// </summary>
    public enum GridRefreshCurrentCellBehavior
    {
        /// <summary>
        /// No refresh necessary when moving the current cell. 
        /// </summary>
        None,

        /// <summary>
        /// Refreshes the current cell only.
        /// </summary>
        RefreshCell,

        /// <summary>
        /// Refreshes the whole row. Use this setting if you are using <see cref="GridShowButtons.ShowCurrentRow"/>.
        /// </summary>
        RefreshRow,
        ////Later:
        ////RefreshColumn
        ////RefreshRowHeader
        ////RefreshColHeader
    }

    /// <summary>
    /// Specifies options for a <see cref="GridModelRowColSizeIndexer.ResizeToFit(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> method call. <para/>
    /// The options can be combined.
    /// </summary>
    [Flags]
    public enum GridResizeToFitOptions
    {
        /// <summary>
        /// Uses default behavior for resizing cells to fit contents. Ignores covered cells, does shrink size, does not include headers.
        /// </summary>
        None = 0,

        /// <summary>
        /// Include covered cells for resizing cells. When using this mode, only the last row or column 
        /// of a covered range is resized.
        /// </summary>
        ResizeCoveredCells = 1,

        /// <summary>
        /// Do not shrink size of cells.
        /// </summary>
        NoShrinkSize = 2,

        /// <summary>
        /// Include also row or column header for resizing the cells.
        /// </summary>
        IncludeHeaders = 4,

        /// <summary>
        /// ResizeCoveredCells mode only resizes the last row or column of a covered range. Use this option
        /// to also resize the columns or rows before the last one.
        /// </summary>
        IncludeCellsWithinCoveredRange = 8
    }
    /// <summary>
    /// Specifies options for a <see cref="GridModelRowColSizeIndexer.ResizeToFit(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> method call. <para/>
    /// The options can be combined.
    /// </summary>
    [Flags]
    public enum GridTextOptions
    {
        /// <summary>
        /// The default text is considered for calculating the Cell size 
        /// </summary>
        Text = 0,
        /// <summary>
        /// The Formatted text is considered for calculating the Cell size 
        /// </summary>
        FormattedText = 1,
    }
    /// <summary>
    /// Used by <see cref="GridCellModelBase.CalculatePreferredCellSize"/>.
    /// </summary>
    public enum GridQueryBounds
    {
        /// <summary>
        /// Queries height of cell.
        /// </summary>
        Height,

        /// <summary>
        /// Queries width of cell.
        /// </summary>
        Width
    }

    /// <summary>
    /// Provides additional hints about a call to <see cref="GridStyleInfo.GetFormattedText(object)"/>,
    /// or <see cref="GridStyleInfo.ApplyFormattedText(string)"/>.
    /// </summary>
    public class GridCellBaseTextInfo
    {
        private GridCellBaseTextInfo()
        {
        }

        /// <summary>
        /// No hint specified.
        /// </summary>
        public const int None = 0;

        /// <summary>
        /// Display text operation.
        /// </summary>
        public const int DisplayText = 0;

        /// <summary>
        /// Paste text operation.
        /// </summary>
        public const int PasteText = 1;

        /// <summary>
        /// Copy text operation.
        /// </summary>
        public const int CopyText = 1;

        /// <summary>
        /// Clear cells operation.
        /// </summary>
        public const int ClearCells = 2;

        /// <summary>
        /// Current text query.
        /// </summary>
        public const int CurrentText = 3;

        /// <summary>
        /// Replace selection.
        /// </summary>
        public const int ReplaceSelection = 4;

        /// <summary>
        /// Initialize text box with text.
        /// </summary>
        public const int TextBox = 5;

        /// <summary>
        /// ValidateString checking if string is valid.
        /// </summary>
        public const int Validate = 6;
    }

    /// <summary>
    /// A <see cref="GridSelectionReason"/> is used by <see cref="GridSelectionChangingEventArgs"/> to give a hint
    /// about the current state of the user action and reason for this event (mouse, keyboard or programmatic).
    /// </summary>
    public enum GridSelectionReason
    {
        /// <summary>
        /// Indicates a <see cref="GridModelSelections.SelectRange"/> call.
        /// </summary>
        SelectRange,

        /// <summary>
        /// Indicates a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> call.
        /// </summary>
        SetCurrentCell,

        /// <summary>
        /// Indicates user is moving current cell with arrow keys.
        /// </summary>
        ArrowKey,

        /// <summary>
        /// Indicates user pressed mouse down.
        /// </summary>
        MouseDown,

        /// <summary>
        /// Indicates user is moving mouse.
        /// </summary>
        MouseMove,

        /// <summary>
        /// Indicates user released mouse.
        /// </summary>
        MouseUp,

        /// <summary>
        /// Indicates current operation was canceled.
        /// </summary>
        CancelMode,

        /// <summary>
        /// Indicates a <see cref="GridModelSelections.Clear()"/> call, e.g. when user hit Escape-key.
        /// </summary>
        Clear
    }

    /// <summary>
    /// A <see cref="GridDragSelectionReason"/> is used by <see cref="GridSelectionDragEventArgs"/> to give a hint
    /// about the current state of the user action and reason for this event.
    /// </summary>
    public enum GridDragSelectionReason
    {
        /// <summary>
        /// Indicates user pressed mouse down.
        /// </summary>
        MouseDown,

        /// <summary>
        /// Indicates user is moving mouse.
        /// </summary>
        MouseMove,

        /// <summary>
        /// Indicates user released mouse.
        /// </summary>
        MouseUp,

        /// <summary>
        /// Indicates current operation was canceled.
        /// </summary>
        CancelMode,

        /// <summary>
        /// Indicates this is a Hit-Test query.
        /// </summary>
        HitTest,
    }

    /// <summary>
    /// A <see cref="GridDragSelectionReason"/> is used by <see cref="GridResizingColumnsEventArgs"/> to give a hint
    /// about the current state of the user action and reason for this event.
    /// </summary>
    public enum GridResizeCellsReason
    {
        /// <summary>
        /// Indicates user pressed mouse down.
        /// </summary>
        MouseDown,

        /// <summary>
        /// Indicates user is moving mouse.
        /// </summary>
        MouseMove,

        /// <summary>
        /// Indicates user released mouse.
        /// </summary>
        MouseUp,

        /// <summary>
        /// Indicates current operation was canceled.
        /// </summary>
        CancelMode,

        /// <summary>
        /// Indicates this is a Hit-Test query.
        /// </summary>
        HitTest,

        /// <summary>
        /// Indicates used double clicked.
        /// </summary>
        DoubleClick,

        /// <summary>
        /// Indicates hidden rows or column will be made visible.
        /// </summary>
        ResetHide,

        /// <summary>
        /// Indicates changes row heights or column widths will be reset back to default value.
        /// </summary>
        ResetDefault,
    }

    /// <summary>
    /// Used by <see cref="GridControlBase.RangeInfoToRectangle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> to enlarge the affected range of cells to include covered and floating cells.
    /// </summary>
    [Flags]
    public enum GridRangeOptions
    {
        /// <summary>
        /// Use range as specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enlarge range with any covered cells that intersect with the original range.
        /// </summary>
        MergeCoveredCells = 1,

        /// <summary>
        /// Enlarge range with any merged cells (not implemented, reserved for future use) that intersect with the original range.
        /// </summary>
        MergeMergedCells = 2,

        /// <summary>
        /// Enlarge range with any floating cells that intersect with the original range.
        /// </summary>
        MergeFloatedCells = 4,

        /// <summary>
        /// Enlarge range with any bannered cells that intersect with the original range.
        /// </summary>
        MergeBanneredCells = 8,

        /// <summary>
        /// Combines <see cref="MergeCoveredCells"/>, <see cref="MergeMergedCells"/>, <see cref="MergeFloatedCells"/> and <see cref="MergeBanneredCells"/>.
        /// </summary>
        [Browsable(false)]
        MergeAllSpannedCells = MergeCoveredCells | MergeMergedCells | MergeFloatedCells | MergeBanneredCells,

        /// <summary>
        /// Included are outside of the current visible grid view. Otherwise <see cref="GridControlBase.RangeInfoToRectangle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> will
        /// ignore cells that are not visible.
        /// </summary>
        CalculateNonClientArea = 16
    }

    /// <summary>
    ///  Specifies how an image is positioned within a cell.
    /// </summary>
    public enum GridBackgroundImageMode
    {
        /// <summary>
        /// The image is placed in the upper-left corner of the cell. The image is clipped if it is larger than the cell it is contained in.
        /// </summary>
        Normal,

        /// <summary>
        /// The image is displayed in the center if the cell is larger than the image. If the image is larger than the cell, the picture is placed in the center of the cell and the outside edges are clipped.
        /// </summary>
        CenterImage,

        /// <summary>
        /// The image within the cell is stretched or shrunk to fit the size of the cell.
        /// </summary>
        StretchImage,

        /// <summary>
        /// The image is tiled across the Cell rectangle.
        /// </summary>
        TileImage
        ////TODO: AlphaBlend Interior
    }

    /// <summary>
    ///    <see cref="GridDirectionType"/> is used in various grid methods to specify direction of a movement.
    /// </summary>
    public enum GridDirectionType
    {
        /// <summary>
        /// No movement specified, use default behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents Move up.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Move to left.
        /// </summary>
        Left = 2,

        /// <summary>
        /// Move down.
        /// </summary>
        Down = 3,

        /// <summary>
        /// Move to right.
        /// </summary>
        Right = 4,

        /// <summary>
        /// Page down.
        /// </summary>
        PageDown = 5,

        /// <summary>
        /// Represents Page up.
        /// </summary>
        PageUp = 6,

        /// <summary>
        /// Go to top.
        /// </summary>
        Top = 7,

        /// <summary>
        /// Go to bottom.
        /// </summary>
        Bottom = 8,

        /// <summary>
        /// Go to most left.
        /// </summary>
        MostLeft = 9,

        /// <summary>
        /// Got to most right.
        /// </summary>
        MostRight = 10,

        /// <summary>
        /// Go to top-left corner.
        /// </summary>
        TopLeft = 11,

        /// <summary>
        /// Got to bottom-right corner.
        /// </summary>
        BottomRight = 12
    }

    /// <summary>
    /// Defines behavior for resizing rows or columns.
    /// </summary>
    /// <remarks>
    /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
    /// by subscribing to the <see cref="GridControlBase.ResizingColumns"/>
    /// and  <see cref="GridControlBase.ResizingRows"/> events.
    /// </remarks>
    [Flags]
    public enum GridResizeCellsBehavior
    {
        /// <summary>
        /// Turn off resizing rows and columns with mouse.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Resize all rows or columns when the user resizes one row or column with the mouse.
        /// </summary>
        ResizeAll = 0x01,

        /// <summary>
        /// Resize a single row or column (or the selected range) when the user resizes one row or column with the mouse.
        /// </summary>
        ResizeSingle = 0x02,

        /// <summary>
        /// Allow the user to resize rows or columns from anywhere inside the grid by grabbing the divider between
        /// row or column headers.
        /// </summary>
        InsideGrid = 0x04,

        /// <summary>
        /// Do not allow the user to resize rows or columns by grabbing the divider between row or column headers. Use this option
        /// only when combined with <see cref="GridResizeCellsBehavior.InsideGrid"/>.
        /// </summary>
        IgnoreHeaders = 0x08,

        /// <summary>
        /// Show a header pressed when the user resizes the associated row or column.
        /// </summary>
        OutlineHeaders = 0x10,   // show headers pressed 

        /// <summary>
        /// Emphasize original cell bounds of tracked row / column.
        /// </summary>
        OutlineBounds = 0x20,   // 

        /// <summary>
        /// Allow the user to drag the mouse outside the grid client area and resize the specific row or column.
        /// </summary>
        AllowDragOutside = 0x40,   // 
    }

    /// <summary>
    /// Defines default hit test context constants returned by the <see cref="MouseControllerDispatcher.HitTest(System.Drawing.Point)"/>
    /// of the grid's <see cref="ScrollControl.MouseControllerDispatcher"/>.
    /// </summary>
    public class GridHitTestContext
    {
        GridHitTestContext()
        {
        }

        /// <summary>
        /// Represents None. 
        /// </summary>
        public const int None = 0;

        /// <summary>
        /// Mouse is over a vertical grid line between headers.
        /// </summary>
        public const int VerticalLine = 1;

        /// <summary>
        /// Mouse is over a horizontal grid line between headers.
        /// </summary>
        public const int HorizontalLine = 2;

        /// <summary>
        /// Mouse is over a cell.
        /// </summary>
        public const int Cell = 3;

        /// <summary>
        /// Mouse is over a header cell.
        /// </summary>
        public const int Header = 4;

        /// <summary>
        /// Mouse is over a selected range.
        /// </summary>
        public const int SelectedRange = 5;

        /// <summary>
        /// Mouse is over the edge of a selected range.
        /// </summary>
        public const int SelectedRangeEdge = 6;

        /// <summary>
        /// Mouse is over a cell button element (see <see cref="GridCellButton"/>).
        /// </summary>
        public const int CellButtonElement = 7;

        /// <summary>
        /// Mouse is over the checker box in a check box cell (see <see cref="GridCheckBoxCellRenderer"/>).
        /// </summary>
        public const int CheckBoxChecker = 7;
    }

    /// <summary>
    /// Specifies placement of the sorticon.
    /// </summary>
    public enum SortIconPlacement
    {
        /// <summary>
        /// SortIcon is placed in right side of cell.
        /// </summary>
        Right = 0,

        /// <summary>
        /// SortIcon is placed in top side of cell.
        /// </summary>
        Top = 1,

        /// <summary>
        /// SortIcon is placed in left side of cell.
        /// </summary>
        Left = 2
    }
    /// <summary>
    /// Specifies placement of the Tree line
    /// </summary>
    public enum TreeLinePlacement
    {
        /// <summary>
        /// Treeline is placed in bottom of the cell
        /// </summary>
        Bottom = 0,

        /// <summary>
        /// Treeline is placed in top of the cell
        /// </summary>
        Top = 1
    }

    /// <summary>
    /// Specifies behavior for selecting cells for the grid by the user with mouse or keyboard.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.AllowSelection"/>.
    /// </remarks>
    [Flags]
    public enum GridSelectionFlags
    {
        /// <summary>
        /// Disable selecting cells.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Rows can be selected.
        /// </summary>
        Row = 0x01,

        /// <summary>
        /// Columns can be selected.
        /// </summary>
        Column = 0x02,

        /// <summary>
        /// Whole table can be selected.
        /// </summary>
        Table = 0x04,

        /// <summary>
        /// Individual cells can be selected.
        /// </summary>
        Cell = 0x08,

        /// <summary>
        /// Multiple ranges of cells can be selected. The user has to press Control Key to select multiple ranges.
        /// </summary>
        Multiple = 0x10,

        /// <summary>
        /// Allow extend existing selection when user holds Shift Key and clicks on a cell.
        /// </summary>
        Shift = 0x20,

        /// <summary>
        /// Allow extend existing selection when user holds Shift Key and arrow keys.
        /// </summary>
        Keyboard = 0x40,

        /// <summary>
        /// Use alpha blending to highlight selected cells.
        /// </summary>
        AlphaBlend = 0x80,

        /// <summary>
        /// Allow both rows and columns to be selected at same time when <see cref="Multiple"/> is specified. By default, the grid does not allow having rows and column
        /// ranges be selected at the same time.
        /// </summary>
        MixRangeType = 0x100,

        /// <summary>
        /// Default behavior for selecting cells: Rows, Columns, Table, Cell, Multiple, Extends Shift Key support, and alphablending.
        /// </summary>
        [Browsable(false)]
        Any = 0xff,
    }

    /// <summary>
    /// This enumeration specifies floating cell's behavior in a <see cref="GridModel"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.FloatCellsMode"/>.
    /// </remarks>
    public enum GridFloatCellsMode
    {
        /// <summary>
        /// Floating cell's behavior is disabled.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Floating cells are calculated before they are displayed and results are saved. Floating cells will
        /// only be recalculated if the width or contents of cells change.
        /// </summary>
        OnDemandCalculation = 0x01,

        /// <summary>
        /// Floating cells are always calculated before cells are displayed.
        /// </summary>
        BeforeDisplayCalculation = 0x02
    }

    /// <summary>
    /// This enumeration specifies merge behavior for an individual cell when merging cells feature has been enabled in a <see cref="GridModel"/> with  <see cref="GridModelOptions.MergeCellsMode"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.MergeCellsMode"/>
    /// </remarks>
    [Flags]
    public enum GridMergeCellDirection
    {
        /// <summary>
        /// Merging cell is disabled.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Merge with neighboring columns in same row.
        /// </summary>
        ColumnsInRow = 0x01,

        /// <summary>
        /// Merge with neighboring rows in same column.
        /// </summary>
        RowsInColumn = 0x02,

        /// <summary>
        /// Represents Both.
        /// </summary>
        [Browsable(false)]
        Both = 3,
    }
    /// <summary>
    /// This enumeration specifies merge behavior for an grid cells when merging cells feature has been enabled in a <see cref="GridModel"/> with  <see cref="GridModelOptions.MergeCellsMode"/>.
    /// </summary>
    public enum GridMergeCellsLayout
    {
        /// <summary>
        /// Merge cells only in visible range.
        /// </summary>
        VisibleRange = 0,

        /// <summary>
        /// Merge cells for entire grid.
        /// May affects performance for a large Grid
        /// </summary>
        Grid = 1
    }

    /// <summary>
    /// This enumeration specifies merge cells behavior in a <see cref="GridModel"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.MergeCellsMode"/>.
    /// </remarks>
    [Flags]
    public enum GridMergeCellsMode
    {
        /// <summary>
        /// Merge cells behavior is disabled.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Merge cells are calculated before they are displayed and results are saved. Floating cells will
        /// only be recalculated if the width or contents of cells change.
        /// </summary>
        OnDemandCalculation = 0x01,

        /// <summary>
        /// Merge cells are always calculated before cells are displayed.
        /// </summary>
        BeforeDisplayCalculation = 0x02,

        /// <summary>
        /// Enable merging of neighboring cells among rows in same column.
        /// </summary>
        MergeRowsInColumn = 4,

        /// <summary>
        /// Enable merging of neighboring cells among columns in same row.
        /// </summary>
        MergeColumnsInRow = 8,

        /// <summary>
        /// When comparing rows or columns, skip hidden rows or columns and allow merging
        /// across hidden rows or columns.
        /// </summary>
        SkipHiddencells = 16,
    }

    /// <summary>
    /// Later ... GridRemoveUndoOption
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridRemoveUndoOption
    {
        public const int RangeStyles = 1;
        public const int CellStyles = 2;
    }

    /// <summary>
    /// Specifies current cell activation behavior when moving the current cell or clicking inside a cell. Defines when to set the focus / toggle edit mode for the current cell.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.ActivateCurrentCellBehavior"/>
    /// </remarks>
    public enum GridCellActivateAction
    {
        /// <summary>
        /// Do not set focus to text box.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Begin editing / focus on text box after user clicked on cell.
        /// </summary>
        /// <remarks>
        /// The cancelable <see cref="GridControlBase.CurrentCellStartEditing"/> event may block editing mode for the current cell.
        /// </remarks>
        ClickOnCell = 0x01,

        /// <summary>
        /// Begin editing / focus on text box whenever a cell becomes current cell no matter if user clicked on cell or moved with arrow keys.
        /// </summary>
        /// <remarks>
        /// When GridCellActivateAction.SetCurrent is specified <see cref="GridCurrentCell.BeginEdit()"/> will be called
        /// before the <see cref="GridControlBase.CurrentCellActivated"/> event is raised. <para/>
        /// See the <see cref="GridControlBase.CurrentCellActivated"/> event if you want to programmatically call <see cref="GridCurrentCell.BeginEdit()"/>.<para/>
        /// The cancelable <see cref="GridControlBase.CurrentCellStartEditing"/> event may block editing mode for the current cell.
        /// </remarks>
        SetCurrent = 0x02,

        /// <summary>
        /// Begin editing / focus on text box when user double clicked on cell.
        /// </summary>
        /// <remarks>
        /// The cancelable <see cref="GridControlBase.CurrentCellStartEditing"/> event may block editing mode for the current cell.
        /// </remarks>
        DblClickOnCell = 0x04,

        /// <summary>
        /// Begin editing / focus on text box and select all text whenever a cell becomes current cell no matter if user clicked on cell or moved with arrow keys.
        /// </summary>
        SelectAll = 0x0a,   // (0x08 | SetCurrent)

        /// <summary>
        /// Forward mouse click to the text box so that the caret can be positioned at the character under the mouse pointer.
        /// </summary>
        PositionCaret = 0x11     // (0x10 | ClickOnCell)
    }

    /// <summary>
    /// Defines the order that cells are loaded before the grid is displayed. This is of use when
    /// using the virtual grid and it is more extensive to move from column to column than to 
    /// move from row to row in your custom data source.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.DrawOrder"/>.
    /// </remarks>
    public enum GridDrawOrder
    {
        /// <summary>
        /// Load grid cells row by row.
        /// </summary>
        Rows = 0,

        /// <summary>
        /// Load grid cells column by column.
        /// </summary>
        Columns = 1
    }

    /// <summary>
    /// This enumeration specifies when to show or hide current cell border / frame.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.ShowCurrentCellBorderBehavior"/>.
    /// </remarks>
    public enum GridShowCurrentCellBorder
    {
        /// <summary>
        /// Show current cell border / frame always.
        /// </summary>
        AlwaysVisible = 0x00,

        /// <summary>
        /// Never show current cell border / frame.
        /// </summary>
        HideAlways = 0x01,

        /// <summary>
        /// Show current cell border when grid is activated.
        /// </summary>
        WhenGridActive = 0x02,

        /// <summary>
        /// Show grayed current cell border when grid is not active control.
        /// </summary>
        GrayWhenLostFocus = 0x04,
        //
        //        /// <summary>
        //        /// Deactivate current cell border when grid is not active control.
        //        /// </summary>
        //        DeactivateWhenInactive  = WhenGridActive|GrayWhenLostFocus
    }

    /// <summary>
    /// Defines options for the <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> method call.
    /// </summary>
    [Flags]
    public enum GridSetCurrentCellOptions
    {
        /// <summary>
        /// No special options.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Do not give current cell a range selection (when using Excel-like current cell).
        /// </summary>
        NoSelectRange = 0x01,

        /// <summary>
        /// Scroll new current cell into view.
        /// </summary>
        ScrollInView = 0x02,

        /// <summary>
        /// Do not set focus on text box in new current cell, ignoring <see cref="GridModelOptions.ActivateCurrentCellBehavior"/>.
        /// </summary>
        NoSetFocus = 0x04,

        /// <summary>
        /// Try to set focus on text box in new current cell, ignoring <see cref="GridModelOptions.ActivateCurrentCellBehavior"/>.
        /// </summary>
        SetFocus = 0x08,

        /// <summary>
        /// Do not synchronize current cell among grid views showing the same model, ignoring <see cref="GridModelOptions.ShouldSynchronizeCurrentCell"/>.
        /// </summary>
        NoSyncCurrentCell = 0x10,

        /// <summary>
        /// Force new current cell to be redrawn.
        /// </summary>
        ForceRefresh = 0x20,

        /// <summary>
        /// Sandwich current cell movement with a BeginUpdate / EndUpdate method call pair to reduce flickering.
        /// </summary>
        BeginEndUpdate = 0x40,

        /// <summary>
        /// Do not active new current cell. Only store row and column index. 
        /// </summary>
        NoActivate = 0x80
    }

    /// <summary>
    /// Specifies scrollbar setting of the grid control with <see cref="GridControlBase.HScrollBehavior"/>
    /// and <see cref="GridControlBase.VScrollBehavior"/>. You can combine the various options.
    /// </summary>
    [Flags]
    public enum GridScrollbarMode
    {
        /// <summary>
        /// Initial setting. Detect parent view if it has shared scrollbars.
        /// </summary>
        DetectIfShared = -1,

        /// <summary>
        /// Disable scrollbars.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Always show scrollbars.
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// Show scrollbars only when necessary.
        /// </summary>
        Automatic = 2,

        /// <summary>
        /// Scrollbars are shared with a parent control.
        /// </summary>
        Shared = 4,

        /// <summary>
        /// When you resize cells in the grid or when you resize the grid window automatically and
        /// you are at the last row or column of the grid, automatically scroll the grid so that
        /// whitespace below or right of the grid is minimal. Starting with version 3.2.1.1 this 
        /// is now the default behavior. You need to explicitly DisableAutoScroll to disable
        /// AutoScroll.
        /// </summary>
        AutoScroll = 8,

        /// <summary>
        /// When you resize cells in the grid or when you resize the grid window automatically and
        /// you are at the last row or column of the grid, disable automatically scrolling the grid.
        /// </summary>
        DisableAutoScroll = 16
    }
    
    /// <summary>
    /// Enables to choose the Option in order to display special characters when the contents in cell exceeds it's width.
    /// The AutoFitOptions  includes Alphabets, Numeric, Both and None.
    /// </summary>
    public enum AutoFitOptions
    {
        /// <summary>
        /// Apply the Autofit option for numeric cell values
        /// </summary>
        Numeric,
        /// <summary>
        /// Apply the Autofit option for Alphabet cell values
        /// </summary>
        Alphabet,
        /// <summary>
        /// Apply the Autofit option for Both numeric and Alphabet cell values
        /// </summary>
        Both,
        /// <summary>
        /// cancel the Autofit option 
        /// </summary>
        None
    }

    /// <summary>
    /// Later ...
    /// Holds temporary information related to printing. This class will change in future versions.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum GridCountRecordsBehavior
    {
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        CountNever = 0,    // Let record count grow while printing

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        CountAlways = 1,    // Count records for both printing and print preview before printing.

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        CountPrint = 2    // Count records only for printing but not for print preview (default).
    }
    
    /// <summary>
    /// Drag-and-drop options used with <see cref="GridControlBaseImp.EnableOleDropTarget()"/>,
    /// <see cref="GridControlBaseImp.EnableOleDataSource()"/>, and <see cref="GridModelCutPaste"/>.
    /// </summary>
    public class GridDragDropFlags
    {
        GridDragDropFlags()
        {
        }

        /// <summary>
        /// Disable drop target.
        /// </summary>
        public const int Disabled = 0x00;

        /// <summary>
        /// Also copy / move column header cells.
        /// </summary>
        public const int ColHeader = 0x01;

        /// <summary>
        /// Also copy / move row header cells.
        /// </summary>
        public const int RowHeader = 0x02;

        /// <summary>
        /// Allow dragging multiple selections.
        /// </summary>
        public const int Multiple = 0x04;

        /// <summary>
        /// Force dragging of CF_TEXT clipboard format.
        /// </summary>
        public const int Text = 0x08;

        /// <summary>
        /// Force dragging of internal styles format.
        /// </summary>
        public const int Styles = 0x10;

        /// <summary>
        /// When copying internal styles, compose the full style of the cell and do not copy only the cell specific attributes.
        /// </summary>
        public const int Compose = 0x20;

        /// <summary>
        ///  Enable autoscroll when user drags out of windows.
        /// </summary>
        public const int AutoScroll = 0x40;

        /// <summary>
        /// Enable edgescroll when user drags to the corner of the window.
        /// </summary>
        public const int EdgeScroll = 0x80;

        /// <summary>
        /// If the user pastes (or drops) more rows than currently available, don't append as many rows as needed.
        /// </summary>
        public const int NoAppendRows = 0x200;

        /// <summary>
        /// If the user pastes (or drops) more columns than currently available, don't append as many columns as needed.
        /// </summary>
        public const int NoAppendCols = 0x400;

        /// <summary>
        /// By default if the user drags multiple rows to the bottom of the grid in an ole drag operation the
        /// outlined rectangle will be clipped at the bottom of the current available rows. If you specify
        /// this option the new rows will be outlined below the last visible row.
        /// </summary>
        public const int OutlineAppendRows = 0x800;

        /// <summary>
        /// By default if the user drags multiple columns to the right of the grid in an ole drag operation the
        /// outlined rectangle will be clipped at the right of the current available columns. If you specify
        /// this option the new columns will be outlined below the last visible column.
        /// </summary>
        public const int OutlineAppendCols = 0x1000;

        /// <summary>
        /// Later ... Paste only: Display "Selected Range is Different"-Dialog.
        /// </summary>
        internal const int CheckRangeDim = 0x100;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public const int EgdeScroll = 0x80;
    }

    /// <summary>
    /// Options for searching text in cells:
    /// </summary>
    public enum GridFindTextOptions
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Match Case.
        /// </summary>
        MatchCase = 1,

        /// <summary>
        /// Search string should match whole cell.
        /// </summary>
        MatchWholeCell = 2,

        /// <summary>
        /// Search up.
        /// </summary>
        SearchUp = 4,

        /// <summary>
        /// Search only current selection.
        /// </summary>
        SelectionOnly = 8,

        /// <summary>
        /// Search only current column.
        /// </summary>
        ColumnOnly = 16,

        /// <summary>
        /// Search whole table.
        /// </summary>
        WholeTable = 32,
    }

    /// <summary>
    /// Defines current undo logging context in the grid.
    /// </summary>
    public enum GridCommandMode
    {
        /// <summary>
        /// Grid is recording commands. This is the default state.
        /// </summary>
        Recording,

        /// <summary>
        /// Grid is currently in process of undoing commands. <see cref="GridModelCommandManager.Undo"/> will set initialize and reset this state.
        /// </summary>
        Undo,

        /// <summary>
        /// Grid is currently in process of redoing commands. <see cref="GridModelCommandManager.Redo"/> will set initialize and reset this state.
        /// </summary>
        Redo,

        /// <summary>
        /// Grid is currently in process of rolling back commands. <see cref="GridModelCommandManager.Rollback"/> will set initialize and reset this state.
        /// </summary>
        Rollback
    }

    /// <summary>
    /// Specifies which mouse controllers should be enabled for the grid.
    /// <para/>
    ///    This enumeration has a <see cref="System.FlagsAttribute"/> attribute that allows a bitwise combination of its member values.
    /// </summary>
    /// <remarks>
    /// When you assign this enumeration value to <see cref="GridModelOptions.ControllerOptions"/>,
    /// the grid will create or disable specified mouse controllers for the grid. Each of these 
    /// mouse controllers implements the <see cref="Syncfusion.Windows.Forms.IMouseController"/> interface and
    /// gets registered with <see cref="ScrollControl.MouseControllerDispatcher"/>.
    /// </remarks>
    [Flags]
    public enum GridControllerOptions
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None = 0,

        /// <summary>
        /// This option enables a mouse controller that handles mouse events for cell elements. Enable this controller if you want cell renderers and
        /// cell button elements to receive mouse events.
        /// </summary>
        /// <remarks>
        /// In its implementation of <see cref="IMouseController.HitTest"/> this mouse controller determines
        /// the cell renderer for the cell under the mouse cursor and calls this cell renderers <see cref="GridCellRendererBase.OnHitTest"/>
        /// method. Based on the cell renderers HitTest result, mouse events will be forwarded to that cell.
        /// </remarks>
        ClickCells = 1,

        /// <summary>
        /// This option enables a mouse controller that handles user interaction for dragging selected rows or columns.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controllers behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.SelectionDragging"/>
        /// and  <see cref="GridControlBase.SelectionDragged"/> events.
        /// <para/>
        /// You can change various options by changing <see cref="GridModelOptions.AllowDragSelectedCols"/>
        /// and <see cref="GridModelOptions.AllowDragSelectedRows"/> in <see cref="GridModel.Options"/>.
        /// </remarks>
        DragSelectRowOrColumn = 2,

        /// <summary>
        /// This option enables a mouse controller that handles user interaction OLE drag support for selected range of cells when the
        /// user clicks the mouse button on the edge of the selected range.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.QueryCanOleDragRange"/> event.
        /// <para/>
        /// You can change various options with <see cref="GridControlBaseImp.EnableOleDataSource()"/>.
        /// <para/>
        /// When you change 
        /// <see cref="GridModelOptions.ControllerOptions"/> in <see cref="GridModelOptions"/>,
        /// this will actually end up calling <see cref="GridControlBaseImp.EnableOleDataSource()"/>
        /// for each associated view.
        /// </remarks>
        OleDataSource = 4,

        /// <summary>
        /// This option enables enables a mouse controller that handles user interaction for OLE drop support when the user drags data 
        /// from within the grid or an outside application.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by creating an object that implements <see cref="IGridDataObjectConsumer"/> and register this object
        /// with <see cref="GridControlBaseImp.RegisterDataObjectConsumer"/>. This allows you to add support for additional
        /// clipboard formats to be dragged into the grid from outside applications.
        /// <para/>
        /// You can change various options with <see cref="GridControlBaseImp.EnableOleDropTarget()"/>. 
        /// <para/>
        /// When you change 
        /// <see cref="GridModelOptions.ControllerOptions"/> in <see cref="GridModelOptions"/>
        /// this will actually end up calling <see cref="GridControlBaseImp.EnableOleDropTarget()"/>
        /// for each associated view.
        /// </remarks>
        OleDropTarget = 8,

        /// <summary>
        /// This option enables a mouse controller that handles user interaction for selecting cells with the mouse.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridModel.SelectionChanging"/> and 
        /// <see cref="GridModel.SelectionChanged"/> events.
        /// <para/>
        /// You can change various options by changing <see cref="GridModelOptions.AllowSelection"/>.
        /// </remarks>
        SelectCells = 16,

        /// <summary>
        /// This option enables Excel-like selection behavior. (It is not really a controller ...) 
        /// When the user selects a range of cells,
        /// the active range is outlined with a selection margin and a cross on the bottom-right side.
        /// See <see cref="GridRangeInfoList.ActiveRange"/>
        /// for <see cref="GridModel.SelectedRanges"/>.
        /// </summary>
        ExcelLikeSelection = 32,

        /// <summary>
        /// This option enables a mouse controller that handles user interaction for resizing rows or columns with the mouse.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBase.ResizingColumns"/>
        /// and  <see cref="GridControlBase.ResizingRows"/> events.
        /// <para/>
        /// You can change various options by changing <see cref="GridModelOptions.ResizeRowsBehavior"/>
        /// and <see cref="GridModelOptions.ResizeColsBehavior"/> in <see cref="GridModel.Options"/>
        /// </remarks>
        ResizeCells = 64,

        /// <summary>
        /// This option enables a mouse controller that provides support for dragging column headers within the
        /// grid by clicking on a header and dragging it to a new position.
        /// </summary>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridControlBaseImp.QueryAllowDragColumnHeader"/> and event.
        /// </remarks>
        DragColumnHeader = 128,

        /// <summary>
        /// Enable support for all default mouse controllers.
        /// </summary>
        [Browsable(false)]
        All = 0x7f
    }
    
    /// <summary>
    /// Specifies which default data consumers should be enabled for the grid.
    /// <para/>
    ///    This enumeration has a <see cref="System.FlagsAttribute"/> attribute that allows a bitwise combination of its member values.
    /// </summary>
    /// <remarks>
    /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
    /// by creating an object that implements <see cref="IGridDataObjectConsumer"/> and register this object
    /// with <see cref="GridControlBaseImp.RegisterDataObjectConsumer"/>. This allows you to add support for additional
    /// clipboard formats to be dragged into the grid from outside applications.
    /// <para/>
    /// You can change various options with <see cref="GridControlBaseImp.EnableOleDropTarget()"/>. 
    /// <para/>
    /// When you change 
    /// <see cref="GridModelOptions.ControllerOptions"/> in <see cref="GridModelOptions"/>,
    /// this will actually end up calling <see cref="GridControlBaseImp.EnableOleDropTarget()"/>
    /// for each associated view.
    /// </remarks>
    [Flags]
    public enum GridDataObjectConsumerOptions
    {
        /// <summary>
        /// No default data objects supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enable styles (internal) data objects. This allows you to drag / copy / paste complete cell information.
        /// </summary>
        Styles = 1,

        /// <summary>
        /// Enable text data format. This allows you to drag cell values.
        /// </summary>
        Text = 2,

        /// <summary>
        /// Enable support for all default data objects.
        /// </summary>
        [Browsable(false)]
        All = 0x3
    }
    /// <summary>
    /// provides the options for combobox cell value selecton
    /// </summary>
    public enum GridComboSelectionOptions
    {
        /// <summary>
        /// values are automaticaally completed when type the value
        /// </summary>
        AutoComplete,
        /// <summary>
        /// automatically suggest the words when type the value
        /// </summary>
        AutoSuggest,
        /// <summary>
        /// provies the support for both AutoComplete and AutoSuggest
        /// </summary>
        Both,
        /// <summary>
        /// doesn't provide any Combo box Selection Option.
        /// </summary>
        None
    }

    /// <summary>
    /// provides the button alignment.
    /// </summary>
    public enum ButtonAlignment
    {
        /// <summary>
        /// Aligns the radio in Horizontal line wise.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Aligns the button in Vertical line wise.
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// Provides a set of names that can be assigned to a
    /// <see cref="GridStyleInfo.CellType"/> property of a <see cref="GridStyleInfo"/>.
    /// </summary>
    /// <remarks>
    /// The class defines a default set of names for cell types that are included
    /// with Essential Grid. 
    /// </remarks>
    public class GridCellTypeName
    {
        /// <summary>
        /// A Header cell. See <see cref="GridHeaderCellRenderer"/>
        /// </summary>
        public readonly static string Header = "Header";

        /// <summary>
        /// A Static cell. See <see cref="GridStaticCellRenderer"/>
        /// </summary>
        public readonly static string Static = "Static";

        /// <summary>
        /// A TextBox cell. See <see cref="GridTextBoxCellRenderer"/>
        /// </summary>
        public readonly static string TextBox = "TextBox";

        /// <summary>
        /// A Image cell. See <see cref="GridImageCellRenderer"/>
        /// </summary>
        public readonly static string Image = "Image";

        /// <summary>
        /// A CheckBox cell. See <see cref="GridCheckBoxCellRenderer"/>
        /// </summary>
        public readonly static string CheckBox = "CheckBox";

        /// <summary>
        /// A PushButton cell. See <see cref="GridPushButtonCellRenderer"/>
        /// </summary>
        public readonly static string PushButton = "PushButton";

        /// <summary>
        /// A NumericUpDown cell. See <see cref="GridNumericUpDownCellRenderer"/>
        /// </summary>
        public readonly static string NumericUpDown = "NumericUpDown";

        /// <summary>
        /// A DropDownGrid cell. See <see cref="GridDropDownGridCellRenderer"/>
        /// </summary>
        public readonly static string DropDownGrid = "DropDownGrid";

        /// <summary>
        /// A GridListControl cell. See <see cref="GridDropDownGridListControlCellRenderer"/>
        /// </summary>
        public readonly static string GridListControl = "GridListControl";

        /// <summary>
        /// A ComboBox cell. See <see cref="GridComboBoxCellRenderer"/>
        /// </summary>
        public readonly static string ComboBox = "ComboBox";

        /// <summary>
        /// A ColorEdit cell. See <see cref="GridDropDownColorUICellRenderer"/>
        /// </summary>
        public readonly static string ColorEdit = "ColorEdit";

        /// <summary>
        /// A MonthCalendar cell. See <see cref="GridDropDownMonthCalendarCellRenderer"/>
        /// </summary>
        public readonly static string MonthCalendar = "MonthCalendar";

        /// <summary>
        /// A FormulaCell cell. See <see cref="GridFormulaCellRenderer"/>
        /// </summary>
        public readonly static string FormulaCell = "FormulaCell";

        /// <summary>
        /// A Currency cell. See <see cref="GridCurrencyTextBoxCellRenderer"/>
        /// </summary>
        public readonly static string Currency = "Currency";

        /// <summary>
        /// A MaskEdit cell. See <see cref="GridMaskEditCellRenderer"/>
        /// </summary>
        public readonly static string MaskEdit = "MaskEdit";

        /// <summary>
        /// A RichText cell. See <see cref="GridRichTextBoxCellRenderer"/>
        /// </summary>
        public readonly static string RichText = "RichText";

        /// <summary>
        /// A generic Control cell. See <see cref="GridGenericControlCellRenderer"/>
        /// </summary>
        public readonly static string Control = "Control";

        /// <summary>
        /// A OriginalTextBox cell. See <see cref="GridOriginalTextBoxCellRenderer"/>
        /// </summary>
        public readonly static string OriginalTextBox = "OriginalTextBox";

        /// <summary>
        /// A ProgressBar cell. See <see cref="GridProgressBarCellRenderer"/>
        /// </summary>
        public readonly static string ProgressBar = "ProgressBar";

        /// <summary>
        /// A RadioButton cell. See <see cref="GridRadioButtonCellRenderer"/>
        /// </summary>
        public readonly static string RadioButton = "RadioButton";

        /// <summary>
        /// A StandardValuesCell cell. See <see cref="GridDropDownStandardValuesCellRenderer"/>
        /// </summary>
        public readonly static string StandardValuesCell = "GridDropDownStandardValuesCellRenderer";

        /// <summary>
        /// A UITypeEditorCell cell. See <see cref="GridUITypeEditorCellRenderer"/>
        /// </summary>
        public readonly static string UITypeEditorCell = "GridUITypeEditorCellRenderer";

        /// <summary>
        /// A PropertyGridCell cell. See <see cref="GridPropertyGridCellRenderer"/>
        /// </summary>
        public readonly static string PropertyGridCell = "GridPropertyGridCellRenderer";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridCellTypeName()
            : base()
        {
        }
    }
}
