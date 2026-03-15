//-------------------------------------------------------------------------------------------------
// <copyright file="GridOutlineCurrentCellHeader.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridOutlineCurrentCellHeader: GridSubComponent
    {
        GridControlBase grid;

////        private GridOutlineCurrentCellHeader outlineCurrentCellHeader;
        public GridOutlineCurrentCellHeader(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        public void OutlineCurrentCellHeader(int currentCellRow, int currentCellCol, ScrollBars direction)
        {
            // Invalidate the row and column header.
#if DEBUG
            Trace.WriteLineIf(Switches.OutlineCurrentCellHeader.TraceVerbose, String.Format("BEGIN OutlineCurrentCellHeader({0}, {1}, {2})\n", currentCellRow, currentCellCol, direction));
#endif

            int bottomRow = grid.ViewLayout.LastVisibleRow;
            int rightCol = grid.ViewLayout.LastVisibleCol;

            bool bVert = direction == ScrollBars.None || direction == ScrollBars.Vertical;
            bool bHorz = direction == ScrollBars.None || direction == ScrollBars.Horizontal;

            // Row Header
            if (bVert && grid.Model.Properties.MarkRowHeader
                && grid.ViewLayout.IsRangeVisible(GridRangeInfo.Row(currentCellRow)))
            {
                Rectangle rectItem = grid.ViewLayout.RangeInfoToRectangle(GridRangeInfo.Cells(currentCellRow, 0, currentCellRow, grid.InternalGetHeaderCols()), GridCellSizeKind.VisibleSize);
                grid.Invalidate(rectItem);
                grid.Update();
            }

            // Column Header
            if (bHorz && grid.Model.Properties.MarkColHeader
                && grid.ViewLayout.IsRangeVisible(GridRangeInfo.Col(currentCellCol)))
            {
                Rectangle rectItem = grid.ViewLayout.RangeInfoToRectangle(GridRangeInfo.Cells(0, currentCellCol, grid.InternalGetHeaderRows(), currentCellCol), GridCellSizeKind.VisibleSize);
                grid.Invalidate(rectItem);
                grid.Update();
            }
            
#if DEBUG
            Trace.WriteLineIf(Switches.OutlineCurrentCellHeader.TraceVerbose, String.Format("END   OutlineCurrentCellHeader({0}, {1}, {2})\n", currentCellRow, currentCellCol, direction));
#endif
        }

        public bool GetMarkHeaderState(GridCellRendererBase pControl, int rowIndex, int colIndex, GridStyleInfo style)
        {
            bool bPressed = false;
            int ncRow, ncCol;
            bool bCurrent = grid.CurrentCell.GetCurrentCell(out ncRow, out ncCol);

            GridRangeInfo r;
            grid.Model.GetSpannedRangeInfo(rowIndex, colIndex, out r);

            if (ncRow <= grid.InternalGetHeaderRows() || ncCol <= grid.InternalGetHeaderCols())
            {  // headers is active Edit Cell
                bPressed = false; // bPressed = grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);
            }
            else
            {
                // Row or Column of active Edit Cell.
                bPressed |= rowIndex > 0
                    && grid.Model.Options.ListBoxSelectionMode == SelectionMode.None
                    && bCurrent && r.IntersectsWith(GridRangeInfo.Row(ncRow))
                    && grid.Model.Properties.MarkRowHeader;

                bPressed |= colIndex > 0
                    && bCurrent && r.IntersectsWith(GridRangeInfo.Col(ncCol))
                    && grid.Model.Properties.MarkColHeader;
            }

            // TODO: Currently resizing size of this Row or Col ==> draw a pressed button.
#if obsolete
            int nResizingCellsMode;
            int nResizingCells;
            bool b = grid.ResizeCellsUI.IsResizingCells(out nResizingCellsMode, out nResizingCells);
            bPressed |= b &&
                (GridUtil.IsSet(nResizingCellsMode, GridResizingCellsMode.ResizeColumn)
                && grid.ResizeCellsUI.IsEnableResizeCols(GridResizeCellsBehavior.OutlineHeaders)
                && r.IntersectsWith(GridRangeInfo.Col(nResizingCells))
                || GridUtil.IsSet(nResizingCellsMode, GridResizingCellsMode.ResizeRow)
                && grid.ResizeCellsUI.IsEnableResizeRows(GridResizeCellsBehavior.OutlineHeaders)
                && r.IntersectsWith(GridRangeInfo.Row(nResizingCells))
                );
#endif
            return bPressed;
        }
    }
}
