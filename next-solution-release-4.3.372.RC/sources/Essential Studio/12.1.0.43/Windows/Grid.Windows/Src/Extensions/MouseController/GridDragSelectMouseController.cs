//-------------------------------------------------------------------------------------------------
// <copyright file="GridDragSelectMouseController.cs" company="syncfusion">
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

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Indicates if rows or columns are dragged.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum GridDragMode
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents Columns.
        /// </summary>
        DragColumn = 1,

        /// <summary>
        /// Represents Rows.
        /// </summary>
        DragRow = 2,
    }

    /// <summary>
    /// Implements the dragging of selected rows or columns in a grid control.
    /// </summary>
    public class GridDragSelectMouseController : GridMouseController
    {
        private DragHelper dragHelper = null;
        private int selDragRow, selDragCol;
        private GridRangeInfo selDragRange = GridRangeInfo.Empty;
        internal new GridControlBase Grid;

        /// <summary>
        /// Initializes a new GridDragSelectMouseController and attaches it to a grid.
        /// </summary>
        /// <param name="Grid">The grid control.</param>
        public GridDragSelectMouseController(GridControlBase Grid)
            : base(Grid)
        {
            this.Grid = Grid;
        }

        /// <summary>
        /// Gets if rows or columns are dragged.
        /// </summary>
        GridDragMode DragSelectionMode
        {
            get
            {
                if (this.hitTestInfo == null)
                {
                    return 0;
                }
                else if (this.selDragRange.IsRows)
                {
                    return GridDragMode.DragRow;
                }
                else
                {
                    return GridDragMode.DragColumn;
                }
            }
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
            return this.hitTestInfo == null;
        }

        void LeftMouseDown(Point point, MouseEventArgs e, int ht, int rowIndex, int colIndex)
        {
            // start dragging
            if (BeginDragSelection(hitTestInfo.rowSelected ? rowIndex : 0, hitTestInfo.colSelected ? colIndex : 0))
            {
                // No, I do initialize it myself (default processing)
                int dy = 0;
                int dx = 0;
                Rectangle r = Grid.GridBounds;

                ScrollBars ab = ScrollBars.None;
                if (hitTestInfo.rowSelected && !Grid.InternalIsFrozenRow(rowIndex))
                {
                    dy = Grid.ViewLayout.GetRowRangeHeight(0, Grid.InternalGetFrozenRows(), GridCellSizeKind.VisibleSize);
                    ab |= ScrollBars.Vertical;
                }

                if (hitTestInfo.colSelected && !Grid.InternalIsFrozenCol(colIndex))
                {
                    dx = Grid.ViewLayout.GetColRangeWidth(0, Grid.InternalGetFrozenCols(), GridCellSizeKind.VisibleSize);
                    ab |= ScrollBars.Horizontal;
                }

                Grid.AutoScrolling = ab;
                Grid.AutoScrollBounds = Rectangle.FromLTRB(dx, dy, r.Right, r.Bottom);

                if (!Grid.CurrentCell.StaticDrawing)
                {
                    Grid.Capture = true;
                }
            }
            else
            {
                Grid.NotifyCancelMode();
            }
        }

        void LeftMouseMove(Point point, MouseEventArgs e)
        {
            int rowIndex, colIndex;
            point = GridUtil.MinMax(point, Grid.GridBounds.Location, new Point(Grid.GridBounds.Right, Grid.GridBounds.Bottom));
            GridRangeInfo range = Grid.PointToRangeInfo(point, 1);

            if (this.selDragRange.IsRows)
            {
                rowIndex = Math.Max(Grid.Model.Rows.HeaderCount + 1, range.Top);
                colIndex = 0;
            }
            else
            {
                colIndex = Math.Max(Grid.Model.Cols.HeaderCount + 1, range.Left);
                rowIndex = 0;
            }

            if (colIndex > Grid.ViewLayout.LastVisibleCol)
            {
                if (Grid.Model.colHidden[colIndex])
                {
                    colIndex = Grid.Model.ColCount + 1;
                }
            }
            
            MoveTarget(this.selDragRow, this.selDragCol, rowIndex, colIndex);
        }

        void LeftMouseUp(Point point, MouseEventArgs e)
        {
            if (Grid.Capture)
            {
                Grid.Capture = false;
            }

            int rowIndex, colIndex;
            point = GridUtil.MinMax(point, Grid.GridBounds.Location, new Point(Grid.GridBounds.Right, Grid.GridBounds.Bottom));
            GridRangeInfo range = Grid.PointToRangeInfo(point, 1);

            if (this.selDragRange.IsRows)
            {
                rowIndex = range.Top;
                if (Grid.TopRowIndex > Grid.InternalGetFrozenRows() + 1)
                {
                    rowIndex = Math.Max(Grid.TopRowIndex, rowIndex);
                }
                else
                {
                    rowIndex = Math.Max(Grid.InternalGetHeaderRows() + 1, rowIndex);
                }

                colIndex = 0;
            }
            else
            {
                colIndex = range.Left;
                if (colIndex > Grid.InternalGetFrozenCols() && Grid.LeftColIndex > Grid.InternalGetFrozenCols() + 1)
                {
                    colIndex = Math.Max(Grid.LeftColIndex, colIndex);
                }
                else
                {
                    colIndex = Math.Max(Grid.InternalGetHeaderCols() + 1, colIndex);
                }

                rowIndex = 0;
            }
             if (colIndex > Grid.ViewLayout.LastVisibleCol)
             {
                 if (Grid.Model.colHidden[colIndex])
                 {
                     colIndex = Grid.Model.ColCount + 1;
                 }
             }

            GridRangeInfo pRange = this.selDragRange;
            EndDragSelection(pRange.Top, pRange.Left, rowIndex, colIndex);
        }

        Point CalcCenterPoint(int rowIndex, int colIndex)
        {
            Grid.ViewLayout.Reset();
            int dx, dy;
            if (Grid.RightToLeft == RightToLeft.Yes)
            {
                Point loc = new Point(Grid.GridBounds.Right, Grid.GridBounds.Y);
                dx = loc.X - (Grid.GetColWidth(0) / 2);
                dy = loc.Y + (Grid.GetRowHeight(0) / 2);
            }
            else
            {
                Point loc = Grid.GridBounds.Location;
                dx = (Grid.GetColWidth(0) / 2) + loc.X;
                dy = (Grid.GetRowHeight(0) / 2) + loc.Y;
            }

            if (this.selDragRange.IsRows)
            {
                dy = Grid.ViewLayout.RowColToPoint(rowIndex, 0, GridCellSizeKind.ActualSize).Y;
            }
            else
            {
                dx = Grid.ViewLayout.RowColToPoint(0, colIndex, GridCellSizeKind.ActualSize).X;
            }

            Point pt = new Point(dx, dy);
            pt = Grid.GridPointToScreen(pt);
            return pt;
        }

        GridRangeInfo CalcDestRange(GridRangeInfo sourceRange, int rowIndex, int colIndex)
        {
            GridRangeInfo destRange;
            if (sourceRange.IsRows)
            {
                destRange = GridRangeInfo.Row(rowIndex);
                if (sourceRange.Contains(destRange))
                {
                    destRange = GridRangeInfo.Row(sourceRange.Bottom + 1);
                }
            }
            else
            {
                destRange = GridRangeInfo.Col(colIndex);
                if (sourceRange.Contains(destRange))
                {
                    destRange = GridRangeInfo.Col(sourceRange.Right + 1);
                }
            }

            return destRange;
        }

        bool BeginDragSelection(int rowIndex, int colIndex)
        {
            this.selDragRow = Math.Max(rowIndex, 1);
            this.selDragCol = Math.Max(colIndex, 1);

            GridRangeInfoList selections = Grid.Model.Selections.Ranges;

            // Find the Selection Rectangle.
            this.selDragRange = selections.GetRangesIntersecting(GridRangeInfo.Cell(rowIndex, colIndex)).ActiveRange;

            //// Trigger selection dragging event.
            //// The programmer can override OnBeginDragSelection.
            //// If the method returns False, dragging will not be processed.

            if (!(this.selDragRange.IsRows || this.selDragRange.IsCols))
            {
                return false;
            }

            GridRangeInfo destRange = CalcDestRange(this.selDragRange, rowIndex, colIndex);

            if (!RaiseSelectionDragging(this.selDragRange, ref destRange, GridDragSelectionReason.MouseDown))
            {
                return false;
            }

            if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(selDragRange))
            {
                GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(selDragRange);
                foreach (GridRangeInfo info in list)
                {
                    if (selDragRange.Right != info.Right && !(selDragRange.Right > info.Right))
                        return false;
                }
            }
            this.selDragRow = this.selDragRange.Top;
            this.selDragCol = this.selDragRange.Left;

            Point pt = CalcCenterPoint(this.selDragRow, this.selDragCol);
            Bitmap bm = CreateHeaderBitmap(this.selDragRow, this.selDragCol);
            dragHelper = new DragHelper();
            dragHelper.StartDrag(
                bm,
                pt,
                DragDropEffects.Move);

            Grid.Model.FloatingCells.LockEvaluate();

            return true;
        }

        internal bool RaiseSelectionDragging(GridRangeInfo range, ref GridRangeInfo destination, GridDragSelectionReason reason)
        {
            GridSelectionDragEventArgs e = new GridSelectionDragEventArgs(range, destination, reason);
            Grid.RaiseSelectionDragging(e);
            destination = e.Destination;
            return !e.Cancel;
        }

        bool RaiseSelectionDragged(GridRangeInfo range, ref GridRangeInfo destination, GridDragSelectionReason reason)
        {
            GridSelectionDragEventArgs e = new GridSelectionDragEventArgs(range, destination, reason);
            Grid.RaiseSelectionDragged(e);
            destination = e.Destination;
            return !e.Cancel;
        }

        bool forceMoveTarget = false;

        bool MoveTarget(int nRow1, int nCol1, int rowIndex, int colIndex)
        {
            if (Grid.LeftColIndex > Grid.InternalGetFrozenCols() + 1)
            {
                colIndex = Math.Max(Grid.LeftColIndex, colIndex);
            }
            else
            {
                colIndex = Math.Max(Grid.InternalGetHeaderCols() + 1, colIndex);
            }

            if (Grid.TopRowIndex > Grid.InternalGetFrozenRows() + 1)
            {
                rowIndex = Math.Max(Grid.TopRowIndex, rowIndex);
            }
            else
            {
                rowIndex = Math.Max(Grid.InternalGetHeaderRows() + 1, rowIndex);
            }
            
            GridRangeInfo destRange = CalcDestRange(this.selDragRange, rowIndex, colIndex);

            if (this.dragHelper == null || !RaiseSelectionDragging(this.selDragRange, ref destRange, GridDragSelectionReason.MouseDown))
            {
                return false;
            }

            // Check if nothing changed.
            if (destRange.Top == this.selDragRow && destRange.Left == this.selDragCol
                && !forceMoveTarget)
            {
                return true;
            }

            this.selDragRow = destRange.Top;
            this.selDragCol = destRange.Left;

            // Trigger selection dragging event, programmer can change / disallow destination.
            DragDropEffects effect;
            if (RaiseSelectionDragging(this.selDragRange, ref destRange, GridDragSelectionReason.MouseMove))
            {
                effect = DragDropEffects.Move;
            }
            else
            {
                effect = DragDropEffects.None;
            }

            Point pt = CalcCenterPoint(destRange.Top, destRange.Left);

            Rectangle gb = Grid.GridRectangleToScreen(Grid.GridBounds);
            if (gb.Contains(pt))
            {
                this.dragHelper.DoDrag(pt, effect);
                forceMoveTarget = false;
            }
            else
            {
                this.dragHelper.DoDrag(new Point(-1000, -1000), effect);
                forceMoveTarget = true;
            }

            return true;
        }

        bool EndDragSelection(int nRow1, int nCol1, int rowIndex, int colIndex)
        {
            Grid.Model.FloatingCells.UnlockEvaluate();

            if (this.dragHelper != null)
            {
                this.dragHelper.EndDrag();
                this.dragHelper = null;
            }

            if (this.selDragRange.IsRows && rowIndex == 0)
            {
                return false;
            }

            if (this.selDragRange.IsCols && colIndex == 0)
            {
                return false;
            }

            colIndex = Math.Max(1, colIndex);
            rowIndex = Math.Max(1, rowIndex);

            GridRangeInfo destRange = CalcDestRange(this.selDragRange, rowIndex, colIndex);

            GridRangeInfo r = this.selDragRange;

            // If programmer wants to perform custom operation when selection is dragged,
            // he can override OnSelectionDragging, check for MouseUp reason, and return False.
            if (!RaiseSelectionDragging(r, ref destRange, GridDragSelectionReason.MouseUp))
            {
                return false;
            }

            if (RaiseSelectionDragged(r, ref destRange, GridDragSelectionReason.MouseUp))
            {
                this.selDragRow = destRange.Top;
                this.selDragCol = destRange.Left;

                if (this.selDragRange.Contains(destRange))
                {
                    return false;
                }

                GridRangeInfo range = GridRangeInfo.Empty;
                if (this.selDragRange.IsRows)
                {
                    int row = selDragRange.Height;
                    if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(selDragRange) || this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(destRange) )
                    {
                        GridRangeInfo dragRange = GridRangeInfo.Rows(destRange.Top, destRange.Top + selDragRange.Height - 1);
                        if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(dragRange))
                        {
                            GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(dragRange);
                            foreach (GridRangeInfo info in list)
                            {
                                if (info.Top != info.Bottom && (info.Bottom - info.Top + 1) > row)
                                {
                                    row = info.Bottom - info.Top + 1;
                                }
                            }
                        }
                        if (row != selDragRange.Height)
                        {
                            if (this.Grid.ShowMessageBoxOnDrop)
                                MessageBoxAdv.Show(SR.GetString("Cannotchangepartofamergedcell"));

                            GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(dragRange);
                            foreach (GridRangeInfo info in list)
                            {
                                if (info.Top != destRange.Top)
                                    selDragRow = r.Top;
                            }
                        }
                        else if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(selDragRange) && this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(destRange))
                        {
                            GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(dragRange);
                            foreach (GridRangeInfo info in list)
                            {
                                if (info.Top != destRange.Top)
                                    selDragRow = r.Top;
                            }
                        }
                    }
                    Grid.Model.Rows.MoveRange(r.Top, r.Height, this.selDragRow > r.Top ? this.selDragRow - r.Height : this.selDragRow);
                }
                else
                {
                    if (this.selDragRange.IsCols)
                    {
                        int col = selDragRange.Width;
                        if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(selDragRange) || this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(destRange))
                        {
                            GridRangeInfo dragRange = GridRangeInfo.Cols(destRange.Left, destRange.Left + selDragRange.Width - 1);
                            if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(dragRange))
                            {
                                GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(dragRange);
                                foreach (GridRangeInfo info in list)
                                {
                                    if (info.Left != info.Right && (info.Right - info.Left + 1) > col)
                                    {
                                        col = info.Right - info.Left + 1;
                                    }
                                }
                            }
                            if (col != selDragRange.Width)
                            {
                                if (this.Grid.ShowMessageBoxOnDrop)
                                    MessageBoxAdv.Show(SR.GetString("Cannotchangepartofamergedcell"));

                                GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(dragRange);
                                foreach (GridRangeInfo info in list)
                                {
                                    if (info.Left != destRange.Left)
                                        selDragCol = r.Left;
                                }
                            }
                            else if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(selDragRange) && this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(destRange))
                            {
                                GridRangeInfoList list = this.Grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(dragRange);
                                foreach (GridRangeInfo info in list)
                                {
                                    if (info.Left != destRange.Left)
                                        selDragCol = r.Left;
                                }
                            }
                        }
                    }
                    Grid.Model.Cols.MoveRange(r.Left, r.Width, this.selDragCol > r.Left ? this.selDragCol - r.Width : this.selDragCol);
                }

                return true;
            }

            return false;
        }

        void CancelDragSelection(GridControlBase Grid)
        {
            Grid.Model.FloatingCells.UnlockEvaluate();

            if (this.dragHelper != null)
            {
                this.dragHelper.CancelDrag();
            }

            this.dragHelper = null;

            // REVIEW: OnSelectionDragging or OnSelectionDragged?
            GridRangeInfo emptyRange = GridRangeInfo.Empty;
            RaiseSelectionDragged(this.selDragRange, ref emptyRange, GridDragSelectionReason.CancelMode);
        }

        private Bitmap CreateHeaderBitmap(int rowIndex, int colIndex)
        {
            Bitmap bm = null;
            Graphics g = null;
            Size size = new Size(
                rowIndex > 0 ? Grid.GetColWidth(colIndex) : 6,
                colIndex > 0 ? Grid.GetRowHeight(rowIndex) : 6);

            Rectangle bounds = new Rectangle(Point.Empty, size);

            try
            {
                bm = new Bitmap(size.Width, size.Height);
                g = Graphics.FromImage(bm);
                GridStyleInfo style = Grid.Model[rowIndex, colIndex];

                Color backColor = Color.FromArgb(255, style.Interior.BackColor);
                g.FillRectangle(new SolidBrush(backColor), new Rectangle(0, 0, 1000, 1000));

                Brush br = new SolidBrush(backColor != Color.Black ? Color.Black : Color.Blue);
                Rectangle r;
                Size lineSize;
                if (this.selDragRange.IsRows)
                {
                    lineSize = new Size(bounds.Width, 2);
                    Rectangle centerRect = GridUtil.CenterInRect(bounds, lineSize);
                    g.FillRectangle(br, centerRect);
                    r = new Rectangle(0, centerRect.Top - 3, 1, centerRect.Height + 6);
                    g.FillRectangle(br, r);
                    r = new Rectangle(1, centerRect.Top - 2, 1, centerRect.Height + 4);
                    g.FillRectangle(br, r);
                    r = new Rectangle(2, centerRect.Top - 1, 1, centerRect.Height + 2);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Right - 3, centerRect.Top - 1, 1, centerRect.Height + 2);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Right - 2, centerRect.Top - 2, 1, centerRect.Height + 4);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Right - 1, centerRect.Top - 3, 1, centerRect.Height + 6);
                    g.FillRectangle(br, r);
                }
                else
                {
                    lineSize = new Size(2, bounds.Height);
                    Rectangle centerRect = GridUtil.CenterInRect(bounds, lineSize);
                    g.FillRectangle(br, centerRect);
                    r = new Rectangle(centerRect.Left - 3, 0, centerRect.Width + 6, 1);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Left - 2, 1, centerRect.Width + 4, 1);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Left - 1, 2, centerRect.Width + 2, 1);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Left - 1, centerRect.Bottom - 3, centerRect.Width + 2, 1);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Left - 2, centerRect.Bottom - 2, centerRect.Width + 4, 1);
                    g.FillRectangle(br, r);
                    r = new Rectangle(centerRect.Left - 3, centerRect.Bottom - 1, centerRect.Width + 6, 1);
                    g.FillRectangle(br, r);
                }

                bm.MakeTransparent(backColor);
                br.Dispose();
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }

            return bm;
        }

        GridDragSelectHitTestInfo hitTestInfo = null;

        // IMouseController implementation

        /// <override/>
        /// <summary>
        /// The name of this mouse controller.
        /// </summary>
        public override string Name
        {
            get
            {
                return "DragSelect";
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
                return (hitTestInfo == null) ? null : GridCursors.DragSelectionCursor;
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
        /// <param name="e">A MouseEventArgs holding event data.</param>
        public override void MouseHover(MouseEventArgs e)
        {
        }

        /// <override/>
        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">A reference to <see cref="EventArgs"/> holding event data.</param>
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
        /// <param name="e">A MouseEventArgs holding event data.</param>
        public override void MouseDown(MouseEventArgs e)
        {
            if (hitTestInfo != null && CheckMouseButtons(e))
            {
                if (e.Clicks == 1)
                {
                    this.LeftMouseDown(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
                }
            }
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A MouseEventArgs holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            this.LeftMouseMove(point, e);
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A MouseEventArgs holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            this.LeftMouseUp(point, e);
            Grid.AutoScrolling = ScrollBars.None;
        }

        /// <override/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
            this.CancelDragSelection(Grid);
        }

        bool CheckMouseButtons(MouseEventArgs e)
        {
            return (e.Button == MouseButtons.None && Grid.Model.Options.DragSelectedCellsMouseButtonsMask != MouseButtons.None)
                || (e.Button & Grid.Model.Options.DragSelectedCellsMouseButtonsMask) != MouseButtons.None;
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
            // This HitTest code has higher priority than "SelectCells".
            Point pt = new Point(e.X, e.Y);
            hitTestInfo = null;
            if (CheckMouseButtons(e) &&
                e.Clicks < 2 &&
                (controller == null || controller.Name == "SelectCells"))
            {
                hitTestInfo = new GridDragSelectHitTestInfo(this, Grid, pt);
                if (hitTestInfo.hitTestResult == GridHitTestContext.None)
                {
                    hitTestInfo = null;
                }
            }

            return hitTestInfo != null ? hitTestInfo.hitTestResult : 0;
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    internal sealed class GridDragSelectHitTestInfo
    {
        internal GridRangeInfoList selections;
        internal GridRangeInfo cellRange;
        internal int rowIndex;
        internal int colIndex;
        internal int headerRowCount;
        internal int headerColCount;
        internal bool cellSelected;
        internal bool tableSelected;
        internal bool colSelected;
        internal bool rowSelected;
        internal int hitTestResult = GridHitTestContext.None;
        internal Point point;

        internal GridDragSelectHitTestInfo(GridDragSelectMouseController dsc, GridControlBase Grid, Point point)
        {
            selections = Grid.Model.Selections.Ranges;
            cellRange = Grid.PointToRangeInfo(point, -1);
            rowIndex = cellRange.Top;
            colIndex = cellRange.Left;
            this.point = point;
            headerRowCount = Grid.InternalGetHeaderRows();
            headerColCount = Grid.InternalGetHeaderCols();
            cellSelected = selections.AnyRangeContains(cellRange);
            tableSelected = selections.AnyRangeContains(GridRangeInfo.Table());
            colSelected = rowIndex <= 0 && colIndex > headerColCount && selections.AnyRangeContains(GridRangeInfo.Col(colIndex));
            rowSelected = colIndex <= 0 && rowIndex > headerRowCount && selections.AnyRangeContains(GridRangeInfo.Row(rowIndex));

            if ((!tableSelected && cellSelected && colSelected && Grid.Model.Options.AllowDragSelectedCols)
                || (!tableSelected && cellSelected && rowSelected && Grid.Model.Options.AllowDragSelectedRows))
            {
                GridRangeInfo selDragRange = selections.GetRangesIntersecting(GridRangeInfo.Cell(rowIndex, colIndex)).ActiveRange;
                if (selDragRange.IsRows || selDragRange.IsCols)
                {
                    GridRangeInfo destRange = GridRangeInfo.Empty;
                    if (dsc.RaiseSelectionDragging(selDragRange, ref destRange, GridDragSelectionReason.HitTest))
                    {
                        hitTestResult = GridHitTestContext.SelectedRange;
                    }
                }
            }
        }
    }
}
