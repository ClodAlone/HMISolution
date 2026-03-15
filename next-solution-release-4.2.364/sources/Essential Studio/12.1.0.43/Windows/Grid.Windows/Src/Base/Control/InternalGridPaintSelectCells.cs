//-------------------------------------------------------------------------------------------------
// <copyright file="InternalGridPaintSelectCells.cs" company="syncfusion">
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
using System.IO;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal sealed class GridPaintSelectCellsInternal
    {
        internal GridControlBase grid = null;

        public void UpdateSelectRange(GridRangeInfo range, GridRangeInfoList pOldRangeList)
        {
            if (grid.Updating)
            {
                grid.UpdateSelectRange_Range = range;
                grid.UpdateSelectRange_OldRange = pOldRangeList;
                return;
            }

            GridRangeInfoList pSelList = grid.Model.SelectedRanges;

            // Visible range of cells.
            int bottomRow = grid.ViewLayout.LastVisibleRow;
            int rightCol = grid.ViewLayout.LastVisibleCol;

            // Expand fixed Cols and Rows (..Ex are absolute index in Grid).
            // and sort them (left < right, top < bottom)
            GridRangeInfo rangeEx = range.ExpandRange(0, 0, grid.Model.RowCount, grid.Model.ColCount);

            // Intersect with displayable Rect  (absolute Index).
            int nTop = (rangeEx.Top <= grid.InternalGetFrozenRows()) ? rangeEx.Top : Math.Max(rangeEx.Top, grid.TopRowIndex);
            int nLeft = (rangeEx.Left <= grid.InternalGetFrozenCols()) ? rangeEx.Left : Math.Max(rangeEx.Left, grid.LeftColIndex);
            int nBottom = Math.Min(rangeEx.Bottom, bottomRow);
            int nRight = Math.Min(rangeEx.Right, rightCol);

            // Get client rows and cols (relative Index to TopLeft).
            nTop = grid.GetClientRow(nTop);
            nLeft = grid.GetClientCol(nLeft);
            nBottom = grid.GetClientRow(nBottom);
            nRight = grid.GetClientCol(nRight);

            if (nBottom == 0)
            {
                nBottom = grid.InternalGetFrozenRows();
            }

            if (nRight == 0)
            {
                nRight = grid.InternalGetFrozenCols();
            }

            if (pOldRangeList != null && pOldRangeList.Count > 0 && pOldRangeList.AnyRangeContains(GridRangeInfo.Table()))
            {
                nTop = nLeft = 0;
            }

            Graphics g = grid.CreateGridGraphics();
            Rectangle excludeClip = grid.InvalidBounds;
            grid.NotifySelectionFrameChanging(g);

            // Check each cell if it should be inverted.
            for (int nClientRow = nTop; nClientRow <= nBottom; nClientRow++)
            {
                int rowIndex = grid.GetRow(nClientRow);
                Rectangle rowBounds = grid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex));

                for (int nClientCol = nLeft; nClientCol <= nRight; nClientCol++)
                {
                    int colIndex = grid.GetCol(nClientCol); // absolute Index
                    GridRangeInfo rg = GridRangeInfo.Cell(rowIndex, colIndex);
                    Rectangle rectItem = grid.RangeInfoToRectangle(rg);

                    bool bIsSel = grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pOldRangeList);
                    bool bNewSel = grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList);

#if DEBUG
                    Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("UpdateSelectRange({0},{1}) : is {2}, new {3}", new object[] { rowIndex, colIndex, bIsSel, bNewSel }));
#endif
                    if (bNewSel != bIsSel)
                    {
                        grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                    }
                }
            }

            ////            if (rangeEx.Contains(grid.CurrentCell.RangeInfo))
            ////                grid.DrawInvertCurrentCell(g, grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex);

            ////grid.m_rgLastSelectionFrame = GridRangeInfo.Empty;
            ////if (pSelList.Count > 0)
            grid.NotifySelectionFrameChanged(g);
            g.Dispose();
        }

        public void PrepareClearSelection()
        {
            int bottomRow = grid.ViewLayout.LastVisibleRow;
            int rightCol = grid.ViewLayout.LastVisibleCol;

            bottomRow = grid.GetClientRow(bottomRow);
            rightCol = grid.GetClientCol(rightCol);

            if (!grid.Updating)
            {
                grid.Update();
            }

            Graphics g = grid.CreateGridGraphics();
            Rectangle excludeClip = grid.InvalidBounds;

            //// Erase old rectangle.
            grid.NotifySelectionFrameChanging(g); ////, true, GridRangeInfo.Empty);
            ////grid.m_rgLastSelectionFrame = GridRangeInfo.Empty;

            GridRangeInfoList pSelList = grid.Model.SelectedRanges;
            if (grid.HScrollPixel || grid.VScrollPixel)
            {
                for (int nClientRow = 0; nClientRow <= bottomRow; nClientRow++)
                {
                    int rowIndex = grid.GetRow(nClientRow);
                    Rectangle rowBounds = grid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex));

                    int colIndex = grid.GetCol(0); // absolute Index
                    GridRangeInfo rg = GridRangeInfo.Cell(rowIndex, colIndex);
                    Rectangle rectItem = grid.RangeInfoToRectangle(rg);

                    bool bIsSel = grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList);

                    if (bIsSel)
                    {
                        grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                    }

                    for (int nClientCol = 1; nClientCol <= rightCol; nClientCol++)
                    {
                        colIndex = grid.GetCol(nClientCol); // absolute Index
                        rg = GridRangeInfo.Cell(rowIndex, colIndex);

                        bIsSel = grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList);

                        if (bIsSel)
                        {
                            rectItem = grid.RangeInfoToRectangle(rg);
                            grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                        }
                    }
                }
            }
            else
            {
                for (int nClientRow = 0; nClientRow <= bottomRow; nClientRow++)
                {
                    int rowIndex = grid.GetRow(nClientRow);
                    Rectangle rowBounds = grid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex));

                    int colIndex = grid.GetCol(0); // absolute Index
                    GridRangeInfo rg = GridRangeInfo.Cell(rowIndex, colIndex);
                    Rectangle rectItem = grid.RangeInfoToRectangle(rg);
                    rectItem.Intersect(rowBounds);

                    bool bIsSel = grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList);

                    if (bIsSel)
                    {
                        grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                    }

                    for (int nClientCol = 1; nClientCol <= rightCol; nClientCol++)
                    {
                        colIndex = grid.GetCol(nClientCol); // absolute Index
                        rg = GridRangeInfo.Cell(rowIndex, colIndex);
                        if (grid.IsRightToLeft())
                        {
                            rectItem.Width = grid.GetColWidth(colIndex);
                            rectItem.X -= rectItem.Width;
                        }
                        else
                        {
                            rectItem.X += rectItem.Width;
                            rectItem.Width = grid.GetColWidth(colIndex);
                        }

                        rectItem.Intersect(rowBounds);

                        bIsSel = grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList);

                        if (bIsSel)
                        {
                            grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                        }
                    }
                }
            }

            g.Dispose();
        }

        public void PrepareChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("GridSelectRange.PrepareChangeSelection({0}, {1})", oldRange, newRange));
#endif

            int bottomRow = grid.ViewLayout.LastVisibleRow;
            int rightCol = grid.ViewLayout.LastVisibleCol;

            grid.Model.FloatingCells.LockEvaluate();           //// Don't evaluate float cell state.

            GridRangeInfoList pSelList = grid.Model.SelectedRanges;
            int nRowCount = grid.Model.RowCount;
            int nColCount = grid.Model.ColCount;

            GridRangeInfo visibleClientCells = GridRangeInfo.Cells(0, 0, grid.GetClientRow(bottomRow), grid.GetClientCol(rightCol));
            GridRangeInfo union = grid.MakeClientRange(oldRange).UnionRange(grid.MakeClientRange(newRange));
            GridRangeInfo clientRange = union.IntersectRange(visibleClientCells);

#if DEBUG
            Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("visibleClientCells: {0}, union: {1}, clientRange: {2}", visibleClientCells, union, clientRange));
#endif
            if (!clientRange.IsEmpty)
            {
                if (!grid.Updating)
                {
                    grid.Update();
                }

                // Get Client Rows and Cols (relative Index to TopLeft).
                int nTop = clientRange.Top;
                int nLeft = clientRange.Left;
                int nBottom = clientRange.Bottom;
                int nRight = clientRange.Right;

                if (nBottom == 0)
                {
                    nBottom = grid.InternalGetFrozenRows();
                }

                if (nRight == 0)
                {
                    nRight = grid.InternalGetFrozenCols();
                }

                if (union.IsRows || union.IsTable)
                {
                    nLeft = 0;
                }

                if (union.IsCols || union.IsTable)
                {
                    nTop = 0;
                }

#if DEBUG
                Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("GridSelectRange.PrepareChangeSelection - union: {0}", union));
#endif
                Graphics g = grid.CreateGridGraphics();
                Rectangle excludeClip = grid.InvalidBounds;

                // Erase old and draw new rectangle.
                if (grid.ExcelLikeFrameSelections != null)
                {
                    grid.ExcelLikeFrameSelections.DrawSelectionFrame(grid, g, true, newRange);
                }

                // Check each cell if it should be inverted.
                for (int nClientRow = nTop; nClientRow <= nBottom; nClientRow++)
                {
                    int rowIndex = grid.GetRow(nClientRow);
                    Rectangle rowBounds = grid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex));

                    int colIndex = grid.GetCol(nLeft); // absolute Index
                    GridRangeInfo rg = GridRangeInfo.Cell(rowIndex, colIndex);
                    Rectangle rectItem = grid.RangeInfoToRectangle(rg);
                    rectItem.Intersect(rowBounds);

                    if (!grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList))
                    {
                        bool bIsSel = oldRange.Contains(rg);
                        bool bNewSel = newRange.Contains(rg);

                        if (bNewSel != bIsSel)
                        {
#if DEBUG
                            Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("{0}/{1}: {2}", rowIndex, colIndex, rectItem));
#endif
                            grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                        }
                    }

                    for (int nClientCol = nLeft + 1; nClientCol <= nRight; nClientCol++)
                    {
                        colIndex = grid.GetCol(nClientCol);
                        rg = GridRangeInfo.Cell(rowIndex, colIndex);
                        int colWidth = grid.GetColWidth(colIndex);
                        if (colIndex == grid.LeftColIndex)
                        {
                            colWidth -= grid.GetCurrentHScrollPixelDelta();
                        }

                        if (grid.IsRightToLeft())
                        {
                            rectItem.Width = colWidth;
                            rectItem.X -= rectItem.Width;
                        }
                        else
                        {
                            rectItem.X += rectItem.Width;
                            rectItem.Width = colWidth;
                        }

                        rectItem.Intersect(rowBounds);

                        if (grid.Selections.GetInvertStateRowCol(rowIndex, colIndex, pSelList))
                        {
                            continue;
                        }

                        bool bIsSel = oldRange.Contains(rg);
                        bool bNewSel = newRange.Contains(rg);

                        if (bNewSel != bIsSel)
                        {
#if DEBUG
                            Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("{0}/{1}: {2}", rowIndex, colIndex, rectItem));
#endif
                            grid.IntDrawInvertCell(g, rowIndex, colIndex, rectItem, false);
                        }
                    }
                }

                g.Dispose();
            }

            grid.Model.FloatingCells.UnlockEvaluate();
        }
    }
}
