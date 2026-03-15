//-------------------------------------------------------------------------------------------------
// <copyright file="GridOleDataSourceMouseController.cs" company="syncfusion">
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

using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the datasource part of an OLE drag-and-drop operation in a grid control.
    /// </summary>
    public class GridOleDataSourceMouseController : GridMouseController
    {
        // Attributes - Data source options (Copy headers, Clipboard formats).
        private int dragDropFlags = GridDragDropFlags.Disabled;
        GridControlBase grid;
        OleDataSourceHitTestInfo hitTestInfo = null;

        /// <summary>
        /// Initializes a new GridDragSelectMouseController and attaches it to a grid.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridOleDataSourceMouseController(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Sets drag-and-drop flags. See <see cref="GridDragDropFlags"/>.
        /// </summary>
        /// <param name="flags">The <see cref="GridDragDropFlags"/>.</param>
        /// <returns>returns True.</returns>
        public bool EnableOleDataSource(int flags)
        {
            this.dragDropFlags = flags;
            grid.HitTestSelectionEdge |= flags != 0;

            return true;
        }

        /// <override/>
        /// <summary>
        /// Override this method in your <see cref="GridMouseController"/> and return False if it would interfere with your
        /// controller's state when the current cell would be focused and possibly scrolled into view.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> (True by default) that indicates if the grid is allowed to set the focus onto the current cells <see cref="Control"/>.
        /// </returns>
        public override bool GetAllowFixFocus()
        {
            return false;
        }

        /// <summary>
        /// Starts the drag-and-drop operation at the specified row and column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if operation was started; False otherwise.</returns>
        /// <remarks>
        /// If the grid supports drag-and-drop at the given coordinates, it
        /// will call Control.DoDragDrop.
        /// </remarks>
        public bool DndStartDragDrop(int rowIndex, int colIndex)
        {
            if (this.dragDropFlags == GridDragDropFlags.Disabled)
            {
                return false;
            }

            //
            // If there are no cells selected,
            // copy the current cell's coordinates.
            GridRangeInfoList selList;
            DragDropEffects dropEffect;

            // If user did click into a selected cell (or current cell), start OLE drag-and-drop.
            if (grid.Selections.GetSelectedRanges(out selList, true) && selList.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
            {
                bool bMulti = GridUtil.IsSet(this.dragDropFlags, GridDragDropFlags.Multiple);

                if (!bMulti)
                {
                    // Build up a single-range list.
                    GridRangeInfo rg = selList.GetRangesContaining(GridRangeInfo.Cell(rowIndex, colIndex)).ActiveRange;
                    selList = new GridRangeInfoList();
                    selList.Add(rg);
                }

                grid.Model.DragDropData.dndForceDropCol = GridConstants.MaxRowCol;
                grid.Model.DragDropData.dndForceDropRow = GridConstants.MaxRowCol;

                // Check if we should exclude headers.
                int nFirstRow = 0;
                int nFirstCol = 0;

                if (GridUtil.IsNotSet(this.dragDropFlags, GridDragDropFlags.RowHeader))
                {
                    nFirstCol = grid.InternalGetHeaderCols() + 1;
                }

                if (GridUtil.IsNotSet(this.dragDropFlags, GridDragDropFlags.ColHeader))
                {
                    nFirstRow = grid.InternalGetHeaderRows() + 1;
                }

                // Loop through all selected ranges and expand them.
                // Row and column headers will be excluded from expanded range.
                foreach (GridRangeInfo rangeItem in selList)
                {
                    // If a whole row or column is dragged, allow
                    // it only to paste it as a whole row or column
                    // if pasted into the same grid.
                    if (rangeItem.IsTable)
                    {
                        grid.Model.DragDropData.dndForceDropCol = nFirstCol;
                        grid.Model.DragDropData.dndForceDropRow = nFirstRow;
                    }
                    else if (rangeItem.IsRows)
                    {
                        grid.Model.DragDropData.dndForceDropCol = nFirstCol;
                    }
                    else if (rangeItem.IsCols)
                    {
                        grid.Model.DragDropData.dndForceDropRow = nFirstRow;
                    }
                }

                selList = selList.ExpandRanges(
                    nFirstRow,
                    nFirstCol,
                    grid.Model.RowCount,
                    grid.Model.ColCount);

                grid.Model.DragDropData.dndCurrentCellText = false;
                grid.Model.DragDropData.dndCurrentCellControl = null;

                // Copy data to the OLE data source object
                DataObject dataObject = new DataObject();

                int dndRowsCopied;
                int dndColsCopied;

                // Raises OnQueryOleDataSourceData event.
                this.OnDndCacheGlobalData(dataObject, selList, out dndRowsCopied, out dndColsCopied);

                GridModel.InternalGridDragDropData.dndRowsCopied = dndRowsCopied;
                GridModel.InternalGridDragDropData.dndColsCopied = dndColsCopied;

                //// Store settings in parameter object so that when the
                //// user drops data into a different grid window
                //// which is bound to the same parameter object can use
                //// this information.

                grid.Model.DragDropData.dndSource = true;
                grid.Model.DragDropData.dndSelList = selList;
                grid.Model.DragDropData.dndStartRow = Math.Min(grid.Model.DragDropData.dndForceDropRow, Math.Max(rowIndex, nFirstRow));
                grid.Model.DragDropData.dndStartCol = Math.Min(grid.Model.DragDropData.dndForceDropCol, Math.Max(colIndex, nFirstCol));
                GridRangeInfo pRange = selList.ActiveRange;
                if (selList.Count == 1 && !pRange.IsEmpty)
                {
                    // Adjust start row / col to avoid that when
                    // the programmer selects a range of cells and
                    // drags the cells by selecting the lower right corner
                    // of the range that this corner becomes the upper left corner.
                    grid.Model.DragDropData.dndRowOffset = Math.Max(grid.Model.DragDropData.dndStartRow, pRange.Top) - pRange.Top;
                    grid.Model.DragDropData.dndColOffset = Math.Max(grid.Model.DragDropData.dndStartCol, pRange.Left) - pRange.Left;
                    grid.Model.DragDropData.dndStartRow = Math.Min(grid.Model.DragDropData.dndStartRow, pRange.Top);
                    grid.Model.DragDropData.dndStartCol = Math.Min(grid.Model.DragDropData.dndStartCol, pRange.Left);
                }
                else
                {
                    grid.Model.DragDropData.dndRowOffset = 0;
                    grid.Model.DragDropData.dndColOffset = 0;
                }

                // Show a drop cursor as soon as the user drags out
                // of the current cell.
                Rectangle cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeCoveredCells);
                cellBounds = grid.GridRectangleToScreen(cellBounds);

                // Initiallize static member.
                GridModel.InternalGridDragDropData.dndGridSource = true; // data source is a GridControlBase grid
                GridModel.InternalGridDragDropData.dndGridTargetStyle = false; // Droptarget is a grid and style info is copied

                // Start the drag-and-drop operation.
                dropEffect = grid.DoDragDrop(dataObject, DragDropEffects.Copy | DragDropEffects.Move);

                // Remove selected cells if move operation.
                if ((dropEffect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    grid.Model.RaiseQueryDragDropMoveClearCells(e);
                    if (!e.Cancel)
                    {
                        // dndCurrentCellText is set in OnDndCacheGlobalData when
                        // selected text from the current cell is copied to clipboard.
                        if (grid.Model.DragDropData.dndCurrentCellText)
                        {
                            grid.Model.CommandStack.BeginTrans(SR.GetString("GRID_IDM_CUTDATA"));
                            GridCellRendererBase pCtrl = grid.Model.DragDropData.dndCurrentCellControl;
                            grid.CurrentCell.EndEdit();
                            grid.Model.CommandStack.CommitTrans();
                        }
                        else
                        {
                            grid.Model.ClearCells(selList, GridModel.InternalGridDragDropData.dndGridTargetStyle);
                        }
                    }
                }

                // Reset static member.
                GridModel.InternalGridDragDropData.dndGridSource = false;
                GridModel.InternalGridDragDropData.dndGridTargetStyle = false;

                // Reset settings in parameter object.
                grid.Model.DragDropData.dndSource = false;
                grid.Model.DragDropData.dndStartRow = 0;
                grid.Model.DragDropData.dndStartCol = 0;
                grid.Model.DragDropData.dndSelList = null;
                grid.Model.DragDropData.dndCurrentCellText = false;
                grid.Model.DragDropData.dndCurrentCellControl = null;

                if (dropEffect == DragDropEffects.None)
                    return false;
                return true; // Call was processed.
            }

            return false; // Grid shall continue to find another task to do.
        }

        bool OnDndCacheGlobalData(IDataObject dataObject, GridRangeInfoList selList, out int nDndRowExt, out int nDndColExt)
        {
            nDndRowExt = nDndColExt = 0;

            GridQueryOleDataSourceDataEventArgs e = new GridQueryOleDataSourceDataEventArgs(dataObject, selList, this.dragDropFlags, 0, 0);
            grid.Model.RaiseQueryOleDataSourceData(e);
            this.dragDropFlags = e.DragDropFlags;
            nDndRowExt = e.RowCount;
            nDndColExt = e.ColCount;
            if (e.Handled)
            {
                return e.Result;
            }

            bool bText = GridUtil.IsSet(this.dragDropFlags, GridDragDropFlags.Text);
            bool bStyles = GridUtil.IsSet(this.dragDropFlags, GridDragDropFlags.Styles);
            bool retVal = false;

            // Copy text
            if (bText)
            {
                string s;
                int rowIndex, colIndex;
                bool bCCell = grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex) && !e.IgnoreCurrentCell;

                if (selList.Count == 1 && bCCell && selList[0] == GridRangeInfo.Cell(rowIndex, colIndex)
                    && grid.CurrentCell.HasControlFocus && grid.CurrentCell.Renderer.GetSelectedText(out s)
                    && s.Length > 0)
                {
                    dataObject.SetData(DataFormats.UnicodeText, s);
                    bStyles = false;
                    nDndRowExt = 1;
                    nDndColExt = 1;

                    // Mark this attribute if selected text from an edit control is dragged.
                    grid.Model.DragDropData.dndCurrentCellText = true;
                    grid.Model.DragDropData.dndCurrentCellControl = grid.CurrentCell.Renderer;
                }
                else if (grid.Model.TextDataExchange.CopyTextToBuffer(out s, selList, out nDndRowExt, out nDndColExt))
                {
                    dataObject.SetData(DataFormats.UnicodeText, s);
                }
            }

            // Copy styles.
            if (bStyles)
            {
                GridData data;
                bool bSuccess = grid.Model.DataExchange.CopyCellsToDataObject(out data, selList, GridUtil.IsSet(this.dragDropFlags, GridDragDropFlags.Compose), this.dragDropFlags, out nDndRowExt, out nDndColExt);

                if (bSuccess)
                {
                    dataObject.SetData(data);
                    retVal = true;
                }
            }

            return retVal;
        }

        static int hitTestSelectionEdge = 4;

        /// <internalonly/>
        /// <summary>Gets or sets HitTestSelectionEdge. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int HitTestSelectionEdge
        {
            get
            {
                return hitTestSelectionEdge;
            }

            set
            {
                hitTestSelectionEdge = value;
            }
        }
        
        [Syncfusion.Documentation.DocumentationExclude()]
        internal sealed class OleDataSourceHitTestInfo
        {
            const int hitTestFrame = 8;
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point;
            internal Rectangle cellBounds = Rectangle.Empty;
            internal int clientCol;
            internal int clientRow;
            internal int rowIndex;
            internal int colIndex;
            internal GridRangeInfoList pSelList;
            internal GridRangeInfo activeRange;
            internal bool bTableSel;

            internal OleDataSourceHitTestInfo(GridControlBase grid, Point point)
            {
                this.point = point;
                clientCol = grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                clientRow = grid.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                if (clientCol >= 0 && clientRow >= 0)
                {
                    rowIndex = grid.GetRow(clientRow);
                    colIndex = grid.GetCol(clientCol);

                    if (grid.HitTestSelectionEdge &&
                        clientCol < grid.ViewLayout.VisibleCols && clientRow < grid.ViewLayout.VisibleRows
                        && rowIndex <= grid.Model.RowCount && colIndex <= grid.Model.ColCount)
                    {
                        ////                        cellBounds = new Rectangle(grid.ViewLayout.ClientRowColToPoint(clientRow, clientCol), 
                        ////                            new Size(grid.GetColWidth(colIndex), grid.GetRowHeight(rowIndex)));
                        cellBounds = grid.ViewLayout.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridCellSizeKind.VisibleSize);

                        int nEdge = hitTestSelectionEdge;

                        bool bSelEdge = false;
                        pSelList = grid.Model.Selections.Ranges;
                        activeRange = GridRangeInfo.Empty;

                        // check if whole table is selected
                        bTableSel = pSelList.AnyRangeContains(GridRangeInfo.Table());

                        if (bTableSel)
                        {
                            bSelEdge = rowIndex == 0 && colIndex == 0
                                // check left border
                                && ((point.X >= cellBounds.Left && point.X - cellBounds.Left <= nEdge)
                                // check top border
                                || (point.Y >= cellBounds.Top && point.Y - cellBounds.Top <= nEdge));
                        }
                        else
                        {
                            activeRange = pSelList.ActiveRange;
                            if (activeRange.IsEmpty)
                            {
                                GridRangeInfo rg;
                                grid.Model.CoveredRanges.Find(rowIndex, colIndex, out rg);

                                if (rg != null && grid.CurrentCell.HasCurrentCellAt(rg.Top, rg.Left))
                                {
                                    activeRange = rg;
                                }
                            }

                            GridRangeInfo rgCell = GridRangeInfo.Cell(rowIndex, colIndex);

                            if (activeRange.Contains(rgCell))
                            {
                                // Check left border.
                                if (!bSelEdge && point.X >= cellBounds.Left && point.X - cellBounds.Left <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsRows && activeRange.Left == colIndex;
                                }

                                // Check right border.
                                if (!bSelEdge && point.X <= cellBounds.Right && cellBounds.Right - point.X <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsRows && activeRange.Right == colIndex;
                                }

                                // Check top border.
                                if (!bSelEdge && point.Y >= cellBounds.Top && point.Y - cellBounds.Top <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsCols && activeRange.Top == rowIndex;
                                }

                                // Check bottom border.
                                if (!bSelEdge && point.Y <= cellBounds.Bottom && cellBounds.Bottom - point.Y <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsCols && activeRange.Bottom == rowIndex;
                                }
                            }
                        }

                        if (bSelEdge && RaiseQueryCanOleDragRange(grid, activeRange))
                        {
                            hitTestResult = GridHitTestContext.SelectedRangeEdge;
                        }
                        else
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
                    }
                }
            }

            bool RaiseQueryCanOleDragRange(GridControlBase grid, GridRangeInfo range)
            {
                GridQueryCanOleDragRangeEventArgs e = new GridQueryCanOleDragRangeEventArgs(range);
                grid.RaiseQueryCanOleDragRange(e);
                return !e.Cancel;
            }
        }
        
        /// <override/>
        /// <summary>
        /// The name of this mouse controller.
        /// </summary>
        public override string Name
        {
            get
            {
                return "OleDataSource";
            }
        }

        /// <override/>
        /// <summary>
        /// The cursor to be displayed.
        /// </summary>
        public override Cursor Cursor
        {
            get
            {
                if (hitTestInfo != null && hitTestInfo.hitTestResult == GridHitTestContext.SelectedRangeEdge)
                {
                    return GridCursors.DragSelectionCursor;
                }

                return null;
            }
        }

        /// <override/>
        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
        public override void MouseHoverEnter()
        {
        }

        /// <override/>
        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseHover(MouseEventArgs e)
        {
        }

        /// <override/>
        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> holding event data.</param>
        public override void MouseHoverLeave(EventArgs e)
        {
        }

        /// <override/>
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseDown(MouseEventArgs e)
        {
            if (hitTestInfo != null && e.Button == MouseButtons.Left)
            {
                GridRangeInfo rgCovered;
                grid.Model.CoveredRanges.Find(hitTestInfo.rowIndex, hitTestInfo.colIndex, out rgCovered);

                DndStartDragDrop(rgCovered.Top, rgCovered.Left);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
        }

        /// <override/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
        }

        /// <override/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <remarks>
        /// The current winner of the vote is specified through the controller paramter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data..</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            Point pt = new Point(e.X, e.Y);
            // This HitTest code has higher priority than "SelectCells"
            hitTestInfo = null;
            if (e.Button == MouseButtons.Left &&
                e.Clicks < 2)
            {
                if (controller == null
                    || controller.Name == "SelectCells"
                    || this.grid.MouseControllerDispatcher.LastHitTestCode != GridHitTestContext.CellButtonElement)
                {
                    hitTestInfo = new OleDataSourceHitTestInfo(grid, pt);
                    if (hitTestInfo.hitTestResult == GridHitTestContext.None)
                    {
                        hitTestInfo = null;
                    }
                }
            }

            return hitTestInfo != null ? hitTestInfo.hitTestResult : 0;
        }
    }
}

