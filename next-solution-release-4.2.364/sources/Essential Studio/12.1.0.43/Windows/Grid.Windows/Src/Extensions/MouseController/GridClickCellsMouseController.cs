//-------------------------------------------------------------------------------------------------
// <copyright file="GridClickCellsMouseController.cs" company="syncfusion">
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

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
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
    /// This MouseController is not used by the GridControl by default. Instead, all this functionality is provided by the
    /// <see cref="GridSelectCellsMouseController"/>. However, the grouping grid control uses this controller to forward
    /// events to cell renderers.
    /// </remarks>
    /// <example>This example shows code samples from GroupDropAreaDragHeaderMouseController where the mouse events are forwarded to the GridClickCellsMouseController.
    /// <code lang="C#">
    ///     public class GroupDropAreaDragHeaderMouseController : GroupDragHeaderMouseControllerBase
    ///     {
    ///         public GroupDropAreaDragHeaderMouseController(GridGroupDropArea grid)
    ///         {
    ///             this.groupDropArea = grid;
    ///             isGroupAreaOrigin = true;
    ///             clickCellsController = new GridClickCellsMouseController(grid);
    ///         }
    /// <para/>
    ///         internal GridClickCellsMouseController clickCellsController;
    ///         int clickCellsControllerHitTest = 0;
    /// <para/> 
    ///         public GridClickCellsMouseController ClickCellsController
    ///         {
    ///             get
    ///             {
    ///                 return clickCellsController;
    ///             }
    ///         }
    ///  <para/>
    ///         public void ResetClickCellsController()
    ///         {
    ///             if (this.entered)
    ///                 clickCellsController.MouseHoverLeave(EventArgs.Empty);
    ///             else if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.CancelMode();
    ///             entered = false;
    ///             clickCellsControllerHitTest = 0;
    ///         }
    ///  <para/>
    ///            // unrelated code left out here ...
    ///  <para/>
    ///         #region IMouseController implementation
    ///  <para/>
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
    ///  <para/>
    ///         bool entered = false;
    ///  <para/>
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
    ///  <para/>
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
    ///  <para/>
    ///         public virtual void CancelMode()
    ///         {
    ///             if (clickCellsControllerHitTest != 0)
    ///                 clickCellsController.CancelMode();
    ///         }
    ///  <para/>
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
    public class GridClickCellsMouseController : GridMouseController
    {
        ////GridControlBase Grid;
        internal ClickCellsHitTestInfo hitTestInfo = null;
        ClickCellsHitTestInfo mouseHoverInfo = null;
        GridCellRendererBase cellRenderer = null;

        /// <summary>
        /// Initializes a new <see cref="GridClickCellsMouseController"/>.
        /// </summary>
        /// <param name="grid">The parent grid.</param>
        public GridClickCellsMouseController(GridControlBase grid)
            : base(grid)
        {
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

            internal ClickCellsHitTestInfo(GridControlBase grid, MouseEventArgs e, IMouseController controller)
            {
                this.point = new Point(e.X, e.Y);
                clientCol = grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                clientRow = grid.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                cellRenderer = null;
                hitTestResult = 0;

                if (clientCol >= 0 && clientRow >= 0)
                {
                    rowIndex = grid.GetRow(clientRow);
                    colIndex = grid.GetCol(clientCol);

                    if (clientCol < grid.ViewLayout.VisibleCols && clientRow < grid.ViewLayout.VisibleRows
                        && rowIndex <= grid.Model.RowCount && colIndex <= grid.Model.ColCount)
                    {
                        GridRangeInfo spannedRange;
                        if (grid.Model.GetSpannedRangeInfo(rowIndex, colIndex, out spannedRange))
                        {
                            rowIndex = spannedRange.Top;
                            colIndex = spannedRange.Left;
                            cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
                        }
                        else
                        {
                            cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.None);
                        }

                        cellRenderer = grid.GetCellRenderer(rowIndex, colIndex);
                        hitTestResult = cellRenderer.RaiseHitTest(rowIndex, colIndex, e, controller);

                        if (hitTestResult == GridHitTestContext.None)
                        {
                            hitTestResult = GridHitTestContext.Cell;
                        }
                    }
                }
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
                return "CellRenderer";
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
                if (hitTestInfo == null || hitTestInfo.cellRenderer == null)
                {
                    return null;
                }

                return hitTestInfo.cellRenderer.RaiseGetCursor(hitTestInfo.rowIndex, hitTestInfo.colIndex);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
        public override void MouseHoverEnter()
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose);
            mouseHoverInfo = this.hitTestInfo;
            hitTestInfo.cellRenderer.RaiseMouseHoverEnter(hitTestInfo.rowIndex, hitTestInfo.colIndex);
        }

        /// <override/>
        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding the event data.</param>
        public override void MouseHover(MouseEventArgs e)
        {
            if (mouseHoverInfo == null
                || mouseHoverInfo.rowIndex != hitTestInfo.rowIndex
                || mouseHoverInfo.colIndex != hitTestInfo.colIndex
                || mouseHoverInfo.cellRenderer != hitTestInfo.cellRenderer)
            {
                if (mouseHoverInfo != null)
                {
                    mouseHoverInfo.cellRenderer.RaiseMouseHoverLeave(mouseHoverInfo.rowIndex, mouseHoverInfo.colIndex, e);
                }

                hitTestInfo.cellRenderer.RaiseMouseHoverEnter(hitTestInfo.rowIndex, hitTestInfo.colIndex);
                mouseHoverInfo = this.hitTestInfo;
            }

            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose, e.Button, e.X, e.Y, hitTestInfo.rowIndex, hitTestInfo.colIndex, cellRenderer);
            cellRenderer.RaiseMouseHover(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
        }

        /// <override/>
        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The event args.</param>
        public override void MouseHoverLeave(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose);
            mouseHoverInfo.cellRenderer.RaiseMouseHoverLeave(mouseHoverInfo.rowIndex, mouseHoverInfo.colIndex, e);
            mouseHoverInfo = null;
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
            if (hitTestInfo != null)
            {
                cellRenderer.RaiseMouseDown(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
                //// Single, double clicked treated the same ...
                ////this.ProcessMouseDown(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
            cellRenderer.RaiseMouseMove(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            cellRenderer.RaiseMouseUp(hitTestInfo.rowIndex, hitTestInfo.colIndex, e);
        }

        /// <override/>
        /// <summary>Occurs when the current mouse operation is cancelled.</summary>
        public override void CancelMode()
        {
            cellRenderer.RaiseCancelMode(hitTestInfo.rowIndex, hitTestInfo.colIndex);
        }

        /// <override/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <remarks>
        /// The current winner of the vote is specified through the controller paramter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        /// <param name="e">A MouseEventArgs holding event data.</param>
        /// <param name="controller">A mouse controller.</param>
        /// <returns>A non-zero value if the controller can and wants to handle the mouse event; 0 otherwise.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            // This HitTest code has higher priority than any other controller than "ResizeCells".
            hitTestInfo = null;
            if (this.Grid.IsDesignMode() && e.Button != MouseButtons.Left)
            {
                return 0;
            }

            if (controller == null || controller.Name != "ResizeCells")
            {
                hitTestInfo = new ClickCellsHitTestInfo(Grid, e, controller);
                if (hitTestInfo.cellRenderer != null)
                {
                    cellRenderer = hitTestInfo.cellRenderer;
                }
            }

            return hitTestInfo != null ? hitTestInfo.hitTestResult : 0;
        }
    }
}

