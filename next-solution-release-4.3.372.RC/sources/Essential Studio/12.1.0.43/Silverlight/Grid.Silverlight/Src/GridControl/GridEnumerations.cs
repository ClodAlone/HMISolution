#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !WinRT
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{

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

    // Summary:
    //     Specifies which scroll bars will be visible on a control.
    public enum GridOrientation
    {
        // Summary:
        //     No scroll bars are shown.
        None = 0,
        //
        // Summary:
        //     Only horizontal scroll bars are shown.
        Horizontal = 1,
        //
        // Summary:
        //     Only vertical scroll bars are shown.
        Vertical = 2,
        //
        // Summary:
        //     Both horizontal and vertical scroll bars are shown.
        Both = 3,
    }

    // Summary:
    //     Specifies the selection behavior of a list box.
    public enum GridSelectionMode
    {
        // Summary:
        //     No items can be selected.
        None = 0,
        //
        // Summary:
        //     Only one item can be selected.
        One = 1,
        //
        // Summary:
        //     Multiple items can be selected.
        MultiSimple = 2,
        //
        // Summary:
        //     Multiple items can be selected, and the user can use the SHIFT, CTRL, and
        //     arrow keys to make selections
        MultiExtended = 3,
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
        /// Move up.
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
        /// Page up.
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
        BottomRight = 12,
        /// <summary>
        /// Page down.
        /// </summary>
        PageRight = 13,
        /// <summary>
        /// Page up.
        /// </summary>
        PageLeft = 14,
    }


    /// <summary>
    /// Specifies options for a ResizeColumnsToFit or ResizeRowsToFit method call. <para/>
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
        IncludeCellsWithinCoveredRange = 8,
        /// <summary>
        /// Include hidden rows in row or column resizing behavior
        /// </summary>
        IncludeHiddenCells = 10
    }

    public enum PrecedenceStyle
    {
        Row,
        Column
    }

}
