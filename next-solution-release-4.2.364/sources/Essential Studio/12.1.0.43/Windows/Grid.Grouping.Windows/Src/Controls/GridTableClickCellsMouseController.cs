//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableClickCellsMouseController.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// This MouseController handles mouse events for cell elements. In its HitTest method, the
    /// cell renderer for the cell under the mouse cursor is determined and based on the cell renderer's
    /// HitTest result, mouse events will be forwarded to that cell.
    /// </summary>
    /// <remarks>
    /// This mouse controller manages the calls to each Cell Renderer's OnHitTest, OnMouseDown, OnMouseHover,
    /// OnMouseUp, OnMouseHoverEnter, OnMouseHoverLeave, and OnCancelMode methods. If a cell renderer's OnHitTest
    /// method returns a non-zero value, the cell renderer will receive mouse events associated with that cell.<para/>
    /// If a cell has cell button elements and the mouse is over a cell button element, the cell button element will
    /// receive all mouse events associated with that cell button element.<para/>
    /// If the user clicks on a cell button element, the cell button element will raise a Click event in its OnMouseUp
    /// event handler.
    /// <para/>
    /// This MouseController is not used by the GridControl by default. Instead all this functionality is provided by
    /// <see cref="GridSelectCellsMouseController"/>. However, the grouping grid control uses this controller to forward
    /// events to cell renderers.
    /// </remarks>
    /// <example>This example shows some code samples from GroupDropAreaDragHeaderMouseController where the mouse events are forwarded to the GridTableClickCellsMouseController:
    /// <code lang="C#">
    ///     public class GroupDropAreaDragHeaderMouseController : GroupDragHeaderMouseControllerBase
    ///     {
    ///         public GroupDropAreaDragHeaderMouseController(GridGroupDropArea grid)
    ///         {
    ///             this.groupDropArea = grid;
    ///             isGroupAreaOrigin = true;
    ///             clickCellsController = new GridTableClickCellsMouseController(grid);
    ///         }
    /// <para/>
    ///         internal GridTableClickCellsMouseController clickCellsController;
    ///         int clickCellsControllerHitTest = 0;
    /// <para/>
    ///         public GridTableClickCellsMouseController ClickCellsController
    ///         {
    ///             get
    ///             {
    ///                 return clickCellsController;
    ///             }
    ///         }
    /// <para/>
    ///         public void ResetClickCellsController()
    ///         {
    ///             if (this.entered)
    ///                 clickCellsController.MouseHoverLeave(EventArgs.Empty);
    ///             else if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.CancelMode();
    ///             entered = false;
    ///             clickCellsControllerHitTest = 0;
    ///         }
    /// <para/>
    ///            // unrelated code left out here ...
    /// <para/>
    ///         #region IMouseController implementation
    /// <para/>
    ///         public virtual string Name
    ///         {
    ///             get
    ///             {
    ///                 return "DragGroupHeader";
    ///             }
    ///         }
    /// <para/>
    ///         public virtual Cursor Cursor
    ///         {
    ///             get
    ///             {
    ///                 if (cursor != null)
    ///                     return cursor;
    ///                 return clickCellsController.Cursor;
    ///             }
    ///         }
    /// <para/>
    ///         bool entered = false;
    /// <para/>
    ///         public virtual void MouseHoverEnter()
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///             {
    ///                 entered = true;
    ///                 clickCellsController.MouseHoverEnter();
    ///             }
    ///         }
    /// <para/>
    ///         public virtual void MouseHover(MouseEventArgs e)
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.MouseHover(e);
    ///         }
    /// <para/>
    ///         public virtual void MouseHoverLeave(EventArgs e)
    ///         {
    ///             if (entered)
    ///             {
    ///                 clickCellsController.MouseHoverLeave(e);
    ///                 entered = false;
    ///             }
    ///         }
    /// <para/>
    ///         public virtual void MouseDown(MouseEventArgs e)
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.MouseDown(e);
    ///         }
    /// <para/>
    ///         public virtual void MouseMove(MouseEventArgs e)
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.MouseMove(e);
    ///         }
    /// <para/>
    ///         public virtual void MouseUp(MouseEventArgs e)
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.MouseUp(e);
    ///         }
    /// <para/>
    ///         public virtual void CancelMode()
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.CancelMode();
    ///         }
    /// <para/>
    ///         public virtual int HitTest(MouseEventArgs e, IMouseController controller)
    ///         {
    ///             clickCellsControllerHitTest = clickCellsController.HitTest(e, controller);
    ///             return clickCellsControllerHitTest;
    ///         }
    /// <para/>
    ///         #endregion
    ///        }
    /// </code>
    /// </example>
    public class GridTableClickCellsMouseController : GridMouseController
    {
        ////GridControlBase Grid;
        internal ClickCellsHitTestInfo hitTestInfo = null;
        ClickCellsHitTestInfo mouseHoverInfo = null;
        GridCellRendererBase cellRenderer = null;
        GridTableControl gridWindow;

        /// <summary>
        /// Initializes a new <see cref="GridTableClickCellsMouseController"/>.
        /// </summary>
        /// <param name="grid">The parent grid.</param>
        public GridTableClickCellsMouseController(GridControlBase grid)
            : base(grid)
        {
            this.gridWindow = (GridTableControl) Grid.GetGridWindow();
        }

        [Syncfusion.Documentation.DocumentationExclude()]
            internal sealed class ClickCellsHitTestInfo
        {
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point = Point.Empty;
            internal Rectangle cellBounds = Rectangle.Empty;
            private int clientCol = 0;
            private int clientRow = 0;
            internal int rowIndex = 0;
            internal int colIndex = 0;
            internal GridCellRendererBase cellRenderer = null;
            internal int cellRendererHitTest = 0;

            internal ClickCellsHitTestInfo(GridControlBase grid, MouseEventArgs e, IMouseController controller)
            {
                this.point = new Point(e.X, e.Y);
                this.clientCol = grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                this.clientRow = grid.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                this.cellRenderer = null;
                this.hitTestResult = 0;

                if (this.clientCol >= 0 && clientRow >= 0)
                {
                    this.rowIndex = grid.GetRow(clientRow);
                    this.colIndex = grid.GetCol(clientCol);

                    if (this.clientCol < grid.ViewLayout.VisibleCols && clientRow < grid.ViewLayout.VisibleRows
                        && this.rowIndex <= grid.Model.RowCount && colIndex <= grid.Model.ColCount)
                    {
                        GridRangeInfo spannedRange;
                        if (grid.Model.GetSpannedRangeInfo(this.rowIndex, colIndex, out spannedRange))
                        {
                            this.rowIndex = spannedRange.Top;
                            this.colIndex = spannedRange.Left;
                            this.cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
                        }
                        else
                        {
                            this.cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.None);
                        }

                        this.cellRenderer = grid.GetCellRenderer(rowIndex, colIndex);
                        this.hitTestResult = cellRenderer.RaiseHitTest(rowIndex, colIndex, e, controller);
                        this.cellRendererHitTest = hitTestResult;

                        if (this.hitTestResult == GridHitTestContext.None)
                        {
                            this.hitTestResult = GridHitTestContext.Cell;
                        }
                    }
                }
            }
        }

        /// <override/>
        /// <summary>Gets the name of the mouse controller.</summary>
        public override string Name
        {
            get
            {
                return "SelectCells";
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the cursor.
        /// </summary>
        public override Cursor Cursor
        {
            get
            {
                if (this.hitTestInfo == null || this.hitTestInfo.cellRenderer == null)
                {
                    return null;
                }

                return this.hitTestInfo.cellRenderer.RaiseGetCursor(this.hitTestInfo.rowIndex, this.hitTestInfo.colIndex);
            }
        }

        /// <override/>
        /// <summary>
        /// Called before the first time MouseHover is called.
        /// </summary>
        public override void MouseHoverEnter()
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose);
            this.mouseHoverInfo = this.hitTestInfo;
            if (this.hitTestInfo.cellRenderer != null)
            {
                this.hitTestInfo.cellRenderer.RaiseMouseHoverEnter(this.hitTestInfo.rowIndex, this.hitTestInfo.colIndex);
            }
        }

        /// <override/>
        /// <summary>
        /// Called after MouseHoverEnter is called.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding the event data.</param>
        public override void MouseHover(MouseEventArgs e)
        {
            if (this.mouseHoverInfo == null
                || this.mouseHoverInfo.rowIndex != hitTestInfo.rowIndex
                || this.mouseHoverInfo.colIndex != hitTestInfo.colIndex
                || this.mouseHoverInfo.cellRenderer != hitTestInfo.cellRenderer)
            {
                if (this.mouseHoverInfo != null && this.mouseHoverInfo.cellRenderer != null)
                {
                    if (this.mouseHoverInfo.rowIndex > Grid.Model.RowCount)
                    {
                        //// Record has most likely been deleted since mouse moved over it last time
                        //// Debug.WriteLine("Catched out of range issue in SwitchNestedTable.");
                    }
                    else
                    {
                        this.mouseHoverInfo.cellRenderer.RaiseMouseHoverLeave(this.mouseHoverInfo.rowIndex, this.mouseHoverInfo.colIndex, e);
                    }
                }

                if (this.hitTestInfo.cellRenderer != null)
                {
                    this.hitTestInfo.cellRenderer.RaiseMouseHoverEnter(this.hitTestInfo.rowIndex, this.hitTestInfo.colIndex);
                }

                this.mouseHoverInfo = this.hitTestInfo;
            }
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose, e.Button, e.X, e.Y, hitTestInfo.rowIndex, hitTestInfo.colIndex, cellRenderer);
            if (this.cellRenderer != null)
            {
                this.cellRenderer.RaiseMouseHover(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> holding event data.</param>
        public override void MouseHoverLeave(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose);
            if (this.hitTestInfo != null && this.hitTestInfo.cellRenderer != null)
            {
                this.mouseHoverInfo.cellRenderer.RaiseMouseHoverLeave(this.mouseHoverInfo.rowIndex, this.mouseHoverInfo.colIndex, e);
            }

            this.mouseHoverInfo = null;
        }

        bool forwardMouseEvents = false;

        /// <override/>
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseDown(MouseEventArgs e)
        {
            this.forwardMouseEvents = false;
            if (this.hitTestInfo != null)
            {
////                if (!(hitTestInfo.cellRenderer is IGridWindowlessObject)
////                    && !Grid.RaiseMouseActivating())
////                    return;
                if (this.hitTestInfo.cellRenderer != null)
                {
                    GridControlBase gc = this.hitTestInfo.cellRenderer.Control as GridControlBase;
                        if (gc == null || !gc.IsWindowless)
                        {
                            this.forwardMouseEvents = true;
                            this.gridWindow.ProcessTableClickCellsMouseDown(new GridTableClickCellsEventArgs(this.gridWindow, this.cellRenderer, e));
                        }

                        if (this.hitTestInfo.cellRendererHitTest != 0)
                        {
                            this.hitTestInfo.cellRenderer.RaiseMouseDown(this.hitTestInfo.rowIndex, this.hitTestInfo.colIndex, e);
                        }

                    int sRowIndex = this.hitTestInfo.rowIndex;
                    int sColIndex = this.hitTestInfo.colIndex;

                    // Refresh cached HitTest information in case layout was changed (e.g. rows added).
                    this.HitTest(e, null);

                    if ((this.hitTestInfo == null) || (this.hitTestInfo.rowIndex != sRowIndex) || (this.hitTestInfo.colIndex != sColIndex))
                    {
                        Grid.MouseControllerDispatcher.ProcessCancelMode();
                    }
                }
            }
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
            if (this.forwardMouseEvents)
            {
                this.gridWindow.ProcessTableClickCellsMouseMove(new GridTableClickCellsEventArgs(this.gridWindow, this.cellRenderer, e));
            }

            if (this.cellRenderer != null && hitTestInfo.cellRendererHitTest != 0)
            {
                this.cellRenderer.RaiseMouseMove(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            if (this.forwardMouseEvents)
            {
                GridTableClickCellsEventArgs te = new GridTableClickCellsEventArgs(this.gridWindow, this.cellRenderer, e);
                this.gridWindow.ProcessTableClickCellsMouseUp(te);
                if (te.Cancel)
                {
                    this.cellRenderer.RaiseCancelMode(hitTestInfo.rowIndex, hitTestInfo.colIndex);
                    return;
                }
            }

            if (this.cellRenderer != null)
            {
                if (this.hitTestInfo.cellRendererHitTest != 0)
                {
                    this.cellRenderer.RaiseMouseUp(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
                }
                else
                {
                    this.cellRenderer.RaiseClick(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
                }
            }

            this.forwardMouseEvents = false;
        }

        /// <override/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
            if (this.forwardMouseEvents)
            {
                this.gridWindow.ProcessTableClickCellsCancelMode(new GridTableClickCellsEventArgs(this.gridWindow, this.cellRenderer, null));
            }

            if (this.cellRenderer != null && hitTestInfo.cellRendererHitTest != 0)
            {
                this.cellRenderer.RaiseCancelMode(hitTestInfo.rowIndex, hitTestInfo.colIndex);
            }

            this.forwardMouseEvents = false;
        }

        /// <override/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            this.gridWindow.Model.CoveredRanges.ResetCache();

            // if GridSelectionFlags are specified old selection mechanism must be used
            // and GridTableClickCellsMouseController will not work.
            if (this.gridWindow.Table.TableOptions.AllowSelection != GridSelectionFlags.None
                || this.gridWindow.Table.TableOptions.ListBoxSelectionMode == SelectionMode.None)
            {
                return 0;
            }

            // This HitTest code has higher priority than any other controller than "ResizeCells"
            this.hitTestInfo = null;
            if (this.Grid.IsDesignMode() && e.Button != MouseButtons.Left)
            {
                return 0;
            }

            if (controller == null || controller.Name != "ResizeCells")
            {
                this.hitTestInfo = new ClickCellsHitTestInfo(Grid, e, controller);
                if (this.hitTestInfo.cellRenderer != null)
                {
                    this.cellRenderer = hitTestInfo.cellRenderer;
                }
            }

            return this.hitTestInfo != null ? this.hitTestInfo.hitTestResult : 0;
        }
    }
}

