#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
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
        /// Indicates a <see cref="GridCurrentCell.MoveTo"/> call.
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
        /// Indicates a <see cref="GridModelSelections.Clear"/> call, e.g. when user hit Escape-key.
        /// </summary>
        Clear,
        /// <summary>
        /// Indicates user delete current row.
        /// </summary>
        DeleteRow
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
        /// Indidvidual cells can be selected.
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
        //AlphaBlend = 0x80,
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
    };

    /// <summary>
    /// Defines the reason for scrolling current cell into view.
    /// </summary>
    public enum GridScrollCurrentCellReason
    {
        /// <summary>
        /// None
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
        /// Contents of current cell are mofied.
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
        /// When GridCellActivateAction.SetCurrent is specified <see cref="GridCurrentCell.BeginEdit"/> will be called
        /// before the <see cref="GridControlBase.CurrentCellActivated"/> event is raised. <para/>
        /// See the <see cref="GridControlBase.CurrentCellActivated"/> event if you want to programmatically call <see cref="GridCurrentCell.BeginEdit"/>.<para/>
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

    public enum EnableRenderOptimization
    {
        /// <summary>
        /// default
        /// </summary>
        None,
        /// <summary>
        /// turn off the refreshing of cell span backgrounds to improve the selection performance
        /// </summary>
        EnableOptimizations,
        /// <summary>
        /// turn off the rendering of background frame on real time updates and turn off the refreshing of cell span backgrounds
        /// </summary>
        DisableBackgroundFrameRendering 
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
}
