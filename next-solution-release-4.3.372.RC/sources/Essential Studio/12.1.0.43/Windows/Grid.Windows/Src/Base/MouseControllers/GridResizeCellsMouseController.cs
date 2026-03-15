//-------------------------------------------------------------------------------------------------
// <copyright file="GridResizeCellsMouseController.cs" company="syncfusion">
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
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum GridResizingCellsMode
    {
        /// <summary>
        /// Represents None
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents Resize Row
        /// </summary>
        ResizeRow = 1,

        /// <summary>
        /// Represents Resize Column
        /// </summary>
        ResizeColumn = 2
    }

    /// <summary>
    /// Implements the dragging of selected rows or columns in a grid control.
    /// </summary>
    public class GridResizeCellsMouseController : GridMouseController
    {
        private bool inResizingCells = false;
        private Rectangle resizeBounds = Rectangle.Empty;
        private Rectangle originalInvertBarBounds = Rectangle.Empty;
        private GridResizingCellsMode resizingCellsMode = GridResizingCellsMode.None;
        private int rowResizing = 0;
        private int colResizing = 0;
        private bool outlineBounds = false;
        private bool drawInvertLine = false;
        private bool outlineHeader = false;
        private Size originalSize = Size.Empty;
        private Rectangle invertBarBounds = Rectangle.Empty;
        private bool movedMarker = false;
        private bool defaultSize = false;
        private ResizeCellsHitTestInfo hitTestInfo = null;
        private GridControlBase grid;
        private int dragOutsideValue = 0;

        [ThreadStaticAttribute]
        private static GridBorder sizeIndicatorBorder = null;
        [ThreadStaticAttribute]
        private static GridBorder boundsIndicatorBorder = null;
        [ThreadStaticAttribute]
        private static GridDragLineWindow _dragWindow = null;
        [ThreadStaticAttribute]
        private static GridDragLineWindow _oldBoundsWindow = null;

        private static GridDragLineWindow dragWindow
        {
            get
            {
                if (_dragWindow == null)
                {
                    _dragWindow = new GridDragLineWindow();
                }

                return _dragWindow;
            }
        }

        private static GridDragLineWindow oldBoundsWindow
        {
            get
            {
                if (_oldBoundsWindow == null)
                {
                    _oldBoundsWindow = new GridDragLineWindow();
                }

                return _oldBoundsWindow;
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
            return !inResizingCells;
        }

        /// <summary>
        /// Initializes a new GridResizeCellsMouseController and attaches it to a grid.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridResizeCellsMouseController(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        bool HasResizeColOptions(GridResizeCellsBehavior flags)
        {
            return (grid.Model.Options.ResizeColsBehavior & flags) != GridResizeCellsBehavior.None;
        }

        bool HasResizeRowOptions(GridResizeCellsBehavior flags)
        {
            return (grid.Model.Options.ResizeRowsBehavior & flags) != GridResizeCellsBehavior.None;
        }

        bool CanResizeRows
        {
            get
            {
                return HasResizeRowOptions(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.ResizeAll);
            }
        }

        bool CanResizeCols
        {
            get
            {
                return HasResizeColOptions(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.ResizeAll);
            }
        }

        void LeftMouseDown(Point point, MouseEventArgs e, int ht, int rowIndex, int colIndex)
        {
            if (ht == GridHitTestContext.VerticalLine && CanResizeCols)
            {
                if (BeginResizing(rowIndex, colIndex, GridResizingCellsMode.ResizeColumn, point)
                    && !grid.Capture && !grid.CurrentCell.StaticDrawing)
                {
                    grid.Capture = true;
                }
            }
            else if (ht == GridHitTestContext.HorizontalLine && CanResizeRows)
            {
                if (BeginResizing(rowIndex, colIndex, GridResizingCellsMode.ResizeRow, point)
                    && !grid.Capture && !grid.CurrentCell.StaticDrawing)
                {
                    grid.Capture = true;
                }
            }
        }

        void LeftMouseMove(Point point, MouseEventArgs e)
        {
            try
            {
                if (inResizingCells)
                {
                    Rectangle r = this.resizeBounds;
                    Size size;
                    if (grid.IsRightToLeft())
                    {
                        size = new Size(r.Right - point.X, point.Y - r.Y);
                    }
                    else
                    {
                        size = new Size(point.X - r.X, point.Y - r.Y);
                    }

                    if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
                    {
                        size.Width = 0;
                    }
                    else
                    {
                        size.Height = 0;
                    }

                    MoveMarker(size, point);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        void LeftMouseUp(Point point, MouseEventArgs e)
        {
            if (inResizingCells)
            {
                Rectangle r = this.resizeBounds;
                Size size;
                if (grid.IsRightToLeft())
                {
                    size = new Size(r.Right - point.X, point.Y - r.Y);
                }
                else
                {
                    size = new Size(point.X - r.X, point.Y - r.Y);
                }

                if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
                {
                    size.Width = 0;
                }
                else
                {
                    size.Height = 0;
                }

                EndResizing(size, point);
                grid.Capture = false;
            }
        }

        void LeftMouseDoubleClick(Point point, EventArgs e, int ht, int rowIndex, int colIndex)
        {
            int undefined = -1;

            //// check, if user clicked on a vertical line to resize or restore the column
            //// ... (behaviour is depending on Model.Options.ResizeColsBehavior and HeaderCount)

            if (ht == GridHitTestContext.VerticalLine && CanResizeCols)
            {
                GridRangeInfoList pSelList = grid.Model.Selections.Ranges;

                // Can column be resized?
                if (RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref undefined, GridResizeCellsReason.DoubleClick, point))
                {
                    const int nDefault = -1;
                    undefined = -1;

                    if (colIndex > 0 && HasResizeColOptions(GridResizeCellsBehavior.ResizeAll))
                    {
                    }
                    else if (grid.GetColHidden(colIndex)
                        || (colIndex < grid.Model.ColCount && grid.GetColHidden(colIndex + 1)))
                    {
                        int fromColIndex = colIndex;
                        int toColIndex = colIndex + 1;
                        int nCount = grid.Model.ColCount;
                        while (fromColIndex > 0 && grid.GetColHidden(fromColIndex - 1)
                            && RaiseResizingColumns(GridRangeInfo.Col(fromColIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            fromColIndex--;
                        }

                        while (toColIndex <= nCount && grid.GetColHidden(toColIndex)
                            && RaiseResizingColumns(GridRangeInfo.Col(toColIndex), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            toColIndex++;
                        }

                        if (toColIndex > fromColIndex &&
                            RaiseResizingColumns(GridRangeInfo.Cols(fromColIndex, toColIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            grid.Model.HideCols.SetRange(fromColIndex, toColIndex - 1, false);
                        }
                    }
                    else if (colIndex > 0 && pSelList.AnyRangeContains(GridRangeInfo.Col(colIndex)))
                    {
                        if (!pSelList.AnyRangeContains(GridRangeInfo.Table()))
                        {
                            GridRangeInfoList rl = pSelList.GetColRanges(GridRangeInfoType.Cols);

                            //// TODO: WaitCursor?

                            foreach (GridRangeInfo range in rl)
                            {
                                if (grid.GetColHidden(range.Left))
                                {
                                    if (RaiseResizingColumns(range, ref undefined, GridResizeCellsReason.ResetHide, point))
                                    {
                                        grid.Model.HideCols.SetRange(range.Left, range.Right, false);
                                    }
                                }
                                else
                                {
                                    if (RaiseResizingColumns(range, ref undefined, GridResizeCellsReason.ResetDefault, point))
                                    {
                                        grid.Model.ColWidths.SetRange(range.Left, range.Right, nDefault);
                                    }
                                }
                            }
                        }
                    }
                    else if (RaiseResizingColumns(GridRangeInfo.Col(this.colResizing), ref undefined, GridResizeCellsReason.ResetDefault, point))
                    {
                        grid.Model.ColWidths[this.colResizing] = nDefault;
                    }
                }
            }             
            else if (ht == GridHitTestContext.HorizontalLine && CanResizeRows)
            {
                //// check, if user clicked on a horizontal line to resize or restore the row
                //// ... (behaviour is depending oh Model.Options.ResizeRowsBehavior and HeaderCount)

                GridRangeInfoList pSelList = grid.Model.Selections.Ranges;

                // Can row be resized ?
                if (RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref undefined, GridResizeCellsReason.DoubleClick, point))
                {
                    const int nDefault = -1;
                    undefined = -1;

                    if (rowIndex > 0 && HasResizeRowOptions(GridResizeCellsBehavior.ResizeAll))
                    {
                    }
                    else if (grid.GetRowHidden(rowIndex)
                        || (rowIndex < grid.Model.RowCount && grid.GetRowHidden(rowIndex + 1)
                        && RaiseResizingRows(GridRangeInfo.Row(rowIndex + 1), ref undefined, GridResizeCellsReason.ResetHide, point)))
                    {
                        int fromRowIndex = rowIndex;
                        int toRowIndex = rowIndex + 1;
                        int nCount = grid.Model.RowCount;
                        while (fromRowIndex > 0 && grid.GetRowHidden(fromRowIndex - 1)
                            && RaiseResizingRows(GridRangeInfo.Row(fromRowIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            fromRowIndex--;
                        }

                        while (toRowIndex <= nCount && grid.GetRowHidden(toRowIndex)
                           && RaiseResizingRows(GridRangeInfo.Row(toRowIndex), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            toRowIndex++;
                        }

                        if (toRowIndex > fromRowIndex &&
                            RaiseResizingRows(GridRangeInfo.Rows(fromRowIndex, toRowIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            grid.Model.HideRows.SetRange(fromRowIndex, toRowIndex - 1, false);
                        }
                    }
                    else if (rowIndex > 0 && pSelList.AnyRangeContains(GridRangeInfo.Row(rowIndex)))
                    {
                        if (!pSelList.AnyRangeContains(GridRangeInfo.Table()))
                        {
                            GridRangeInfoList rl = pSelList.GetRowRanges(GridRangeInfoType.Rows);

                            //// TODO: WaitCursor ?

                            foreach (GridRangeInfo range in rl)
                            {
                                if (grid.GetRowHidden(range.Top))
                                {
                                    if (RaiseResizingRows(range, ref undefined, GridResizeCellsReason.ResetHide, point))
                                    {
                                        grid.Model.HideRows.SetRange(range.Top, range.Bottom, false);
                                    }
                                }
                                else
                                {
                                    if (RaiseResizingRows(range, ref undefined, GridResizeCellsReason.ResetDefault, point))
                                    {
                                        grid.Model.RowHeights.SetRange(range.Top, range.Bottom, nDefault);
                                    }
                                }
                            }
                        }
                    }
                    else if (RaiseResizingRows(GridRangeInfo.Row(this.rowResizing), ref undefined, GridResizeCellsReason.ResetDefault, point))
                    {
                        grid.Model.RowHeights[this.rowResizing] = nDefault;
                    }
                }
            }
        }

        void DrawInvertRect(Rectangle rect, bool setOrReset)
        {
            if (!setOrReset || sizeIndicatorBorder == null || sizeIndicatorBorder.Style == GridBorderStyle.None)
            {
                dragWindow.Fade();
            }
            else
            {
                int x = sizeIndicatorBorder.Width - 1;
                rect.Inflate(x / 2, x / 2);
                rect.Intersect(grid.GetVisibleBounds());
                Rectangle bounds = grid.GridRectangleToScreen(rect);

                // The grid could be embedded inside a panel or some other parent 
                // which could be clipped by the parent form. We need to avoid
                // the marker to draw over the bounds of the parent form.
                Control parentOfGrid = grid.GetWindow().TopLevelControl;
                if (parentOfGrid != null)
                {
                    bounds.Intersect(parentOfGrid.RectangleToScreen(parentOfGrid.ClientRectangle));
                }

                if (bounds == oldBoundsWindow.Bounds)
                {
                    dragWindow.Fade();
                }
                else
                {
                    dragWindow.ShowAt(bounds, sizeIndicatorBorder);
                }
                ////TraceUtil.TraceCurrentMethodInfo(rect, bounds, dragWindow.Bounds, dragWindow.ClientRectangle);
            }
        }

        void DrawMarkerRect(Rectangle rect, bool setOrReset)
        {
            if (!setOrReset || boundsIndicatorBorder == null || boundsIndicatorBorder.Style == GridBorderStyle.None)
            {
                oldBoundsWindow.Fade();
            }
            else
            {
                int x = boundsIndicatorBorder.Width - 1;
                rect.Inflate(x / 2, x / 2);
                rect.Intersect(grid.GridBounds);
                rect.Intersect(grid.GetVisibleBounds());
                Rectangle bounds = grid.GridRectangleToScreen(rect);

                // The grid could be embedded inside a panel or some other parent 
                // which could be clipped by the parent form. We need to avoid
                // the marker to draw over the bounds of the parent form.
                Control parentOfGrid = grid.GetWindow().TopLevelControl;
                if (parentOfGrid != null)
                {
                    bounds.Intersect(parentOfGrid.RectangleToScreen(parentOfGrid.ClientRectangle));
                }

                oldBoundsWindow.ShowAt(bounds, boundsIndicatorBorder);
            }
        }

        bool BeginResizing(int rowIndex, int colIndex, GridResizingCellsMode nResizingCellsMode, Point point)
        {
            sizeIndicatorBorder = new GridBorder(GridBorderStyle.Dashed, grid.Model.Properties.ResizingCellsLinesColor, GridBorderWeight.Medium);
            boundsIndicatorBorder = new GridBorder(GridBorderStyle.Dotted, grid.Model.Properties.GridLineColor, GridBorderWeight.Thick);
            this.dragOutsideValue = 0;

            // Check for headers in case of hidden columns.
            if ((nResizingCellsMode == GridResizingCellsMode.ResizeColumn
                && !(colIndex > grid.InternalGetHeaderCols() || !HasResizeColOptions(GridResizeCellsBehavior.IgnoreHeaders)))
                || (nResizingCellsMode == GridResizingCellsMode.ResizeRow
                && !(rowIndex > grid.InternalGetHeaderRows() || !HasResizeRowOptions(GridResizeCellsBehavior.IgnoreHeaders))))
            {
                return false;
            }

            // Trigger events and check return values.
            int undefined = -1;
            if ((nResizingCellsMode == GridResizingCellsMode.ResizeColumn
                && !RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref undefined, GridResizeCellsReason.MouseDown, point))
                || (nResizingCellsMode == GridResizingCellsMode.ResizeRow
                && !RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref undefined, GridResizeCellsReason.MouseDown, point)))
            {
                return false;
            }

            //// Action is allowed.

            if (nResizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                rowIndex = 0;
            }

            if (nResizingCellsMode == GridResizingCellsMode.ResizeRow)
            {
                colIndex = 0;
            }

            Rectangle gridBounds = grid.GridBounds;
            Rectangle rectHit = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
            this.resizingCellsMode = nResizingCellsMode;
            this.rowResizing = rowIndex;
            this.colResizing = colIndex;

            rectHit.Offset(-1, -1);

            // ResizingCells line and bounding rectangle.
            if (this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                // Cache the options.
                this.outlineBounds = grid.Model.Properties.DisplayVertLines && HasResizeColOptions(GridResizeCellsBehavior.OutlineBounds);
                this.drawInvertLine = true;
                this.outlineHeader = HasResizeColOptions(GridResizeCellsBehavior.OutlineHeaders);
                this.originalSize = new Size(grid.GetColWidth(colIndex), 0);

                if (this.HasResizeColOptions(GridResizeCellsBehavior.AllowDragOutside))
                {
                    this.dragOutsideValue = 1000;
                }

                // Check if column has an individual width
                this.defaultSize = !grid.Model.ColWidths.IsDefault(this.colResizing);

                // determine the bounding rectangle
                int yMax = grid.ViewLayout.Corner.Y;
                if (grid.IsRightToLeft())
                {
                    this.resizeBounds = Rectangle.FromLTRB(rectHit.Right - 1, gridBounds.Top, rectHit.Right, yMax);
                    this.originalInvertBarBounds = Rectangle.FromLTRB(rectHit.Left - 1, gridBounds.Top, rectHit.Left, yMax);
                }
                else
                {
                    this.resizeBounds = Rectangle.FromLTRB(rectHit.Left, gridBounds.Top, rectHit.Left + 1, yMax);
                    this.originalInvertBarBounds = Rectangle.FromLTRB(rectHit.Right, gridBounds.Top, rectHit.Right + 1, yMax);
                }
            }
            else if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
            {
                // Cache the options.
                this.outlineBounds = grid.Model.Properties.DisplayHorzLines
                    && HasResizeRowOptions(GridResizeCellsBehavior.OutlineBounds);
                this.drawInvertLine = true;
                this.outlineHeader = HasResizeRowOptions(GridResizeCellsBehavior.OutlineHeaders);
                this.originalSize = new Size(0, grid.GetRowHeight(rowIndex));

                if (this.HasResizeRowOptions(GridResizeCellsBehavior.AllowDragOutside))
                {
                    this.dragOutsideValue = 1000;
                }

                // Check if row has an individual height.
                this.defaultSize = !grid.Model.RowHeights.IsDefault(this.rowResizing);

                // Determine the bounding rectangle.
                int xMax = grid.ViewLayout.Corner.X;
                if (grid.IsRightToLeft())
                {
                    this.resizeBounds = Rectangle.FromLTRB(xMax, rectHit.Top, gridBounds.Right, rectHit.Top + 1);
                    this.originalInvertBarBounds = Rectangle.FromLTRB(xMax, rectHit.Bottom, gridBounds.Right, rectHit.Bottom + 1);
                }
                else
                {
                    this.resizeBounds = Rectangle.FromLTRB(gridBounds.Left, rectHit.Top, xMax, rectHit.Top + 1);
                    this.originalInvertBarBounds = Rectangle.FromLTRB(gridBounds.Left, rectHit.Bottom, xMax, rectHit.Bottom + 1);
                }
            }

            // Mark the line of the column which will be sized.
            if (this.outlineBounds)
            {
                this.DrawMarkerRect(originalInvertBarBounds, true);
            }

            this.movedMarker = false;   // will be set true when user moved the line

            this.invertBarBounds = this.originalInvertBarBounds;

            inResizingCells = true;

            if (this.outlineHeader)
            {
                grid.InternalInvalidate(rectHit); // Redraw button (in pressed state)
            }

            return true;
        }

        void MoveMarker(Size size, Point point)
        {
            this.movedMarker = true;    // indicates that user has moved the cursor
            int rowIndex = this.rowResizing;
            int colIndex = this.colResizing;
            bool b = false;

            // Trigger events.
            if (this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                if (size.Width < (int)grid.Model.Options.MinResizeColSize)
                {
                    size.Width = (int)grid.Model.Options.MinResizeColSize;
                }

                int width = size.Width;
                b = RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref width, GridResizeCellsReason.MouseMove, point);
                size.Width = width;
            }
            else if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
            {
                if (size.Height < (int)grid.Model.Options.MinResizeRowSize)
                {
                    size.Height = (int)grid.Model.Options.MinResizeRowSize;
                }

                int height = size.Height;
                b = RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref height, GridResizeCellsReason.MouseMove, point);
                size.Height = height;
            }

            if (!b)
            {
                return;
            }

            ////bool bLock = grid.ScrollGrid.LockScrollbars(true);

            Rectangle gridBounds = grid.GridBounds;
            Rectangle resizeBounds = this.resizeBounds;
            Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
            Size originalSize = this.originalSize;
            Point pt;
            if (grid.IsRightToLeft() && this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                pt = resizeBounds.Location - size;
            }
            else
            {
                pt = resizeBounds.Location + size;
            }

            Rectangle oldRect = this.invertBarBounds;

            if (this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                // New grid line.
                Rectangle invertBarBounds;
                if (grid.IsRightToLeft())
                {
                    invertBarBounds = Rectangle.FromLTRB(
                         Math.Max(gridBounds.Left - dragOutsideValue + 6, Math.Min(resizeBounds.Left, pt.X)),
                         originalInvertBarBounds.Top,
                         Math.Max(gridBounds.Left - dragOutsideValue + 7, Math.Min(pt.X + 1, resizeBounds.Right)),
                         originalInvertBarBounds.Bottom);
                }
                else
                {
                    invertBarBounds = Rectangle.FromLTRB(
                          Math.Min(gridBounds.Right + dragOutsideValue - 7, Math.Max(resizeBounds.Left, pt.X)),
                          originalInvertBarBounds.Top,
                          Math.Min(gridBounds.Right + dragOutsideValue - 6, Math.Max(pt.X + 1, resizeBounds.Right)),
                          originalInvertBarBounds.Bottom);
                }

                this.invertBarBounds = invertBarBounds;

                if (oldRect != invertBarBounds)
                {
                    if (this.drawInvertLine)
                    {
                        // Undo previous inversion.
                        if (oldRect != originalInvertBarBounds)
                        {
                            DrawInvertRect(oldRect, false);
                        }

                        // Show new size.
                        if (invertBarBounds != originalInvertBarBounds)
                        {
                            DrawInvertRect(invertBarBounds, true);
                        }
                    }
                }
            }
            else if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
            {
                // Compute new grid line.
                Rectangle invertBarBounds = Rectangle.FromLTRB(
                    originalInvertBarBounds.Left,
                    Math.Min(gridBounds.Bottom + dragOutsideValue - 7, Math.Max(pt.Y, resizeBounds.Top)),
                    originalInvertBarBounds.Right,
                    Math.Min(gridBounds.Bottom + dragOutsideValue - 6, Math.Max(pt.Y + 1, resizeBounds.Bottom)));

                this.invertBarBounds = invertBarBounds;

                if (oldRect != this.invertBarBounds)
                {
                    // Undo previous inversion.
                    if (this.drawInvertLine)
                    {
                        if (oldRect != this.originalInvertBarBounds)
                        {
                            DrawInvertRect(oldRect, false);
                        }

                        // Show new size.
                        if (this.drawInvertLine && this.invertBarBounds != this.originalInvertBarBounds)
                        {
                            DrawInvertRect(invertBarBounds, true);
                        }
                    }
                }
            }

            ////grid.Update();
            ////grid.ScrollGrid.LockScrollbars(bLock);
        }

        void CancelResizing()
        {
            inResizingCells = false;

            // Redraw the marked grid line.
            DrawMarkerRect(Rectangle.Empty, false);
            DrawInvertRect(Rectangle.Empty, false);

            Rectangle gridBounds = grid.GridBounds;
            int rowIndex = this.rowResizing;
            int colIndex = this.colResizing;

            //// Undo previous inversion.
            ////            Rectangle invertBarBounds = this.invertBarBounds;
            ////            Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
            ////            if (this.drawInvertLine && invertBarBounds != originalInvertBarBounds)
            ////            {
            ////                grid.Invalidate(invertBarBounds);
            ////                //InvertRect(invertBarBounds);
            ////            }

            Size originalSize = this.originalSize;

            int undefined = -1;
            if (this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                RaiseResizingColumns(GridRangeInfo.Col(this.colResizing), ref undefined, GridResizeCellsReason.CancelMode, Point.Empty);

                if (this.outlineHeader)
                {
                    grid.InvalidateRange(GridRangeInfo.Cell(0, this.colResizing));
                }
            }
            else if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
            {
                RaiseResizingRows(GridRangeInfo.Row(this.rowResizing), ref undefined, GridResizeCellsReason.CancelMode, Point.Empty);

                // Redraw the Cells
                if (this.outlineHeader)
                {
                    grid.InvalidateRange(GridRangeInfo.Cell(this.rowResizing, 0));
                }
            }
        }

        void EndResizing(Size size, Point point)
        {
            if (this.movedMarker)
            {
                MoveMarker(size, point);
            }

            DrawMarkerRect(Rectangle.Empty, false);
            DrawInvertRect(Rectangle.Empty, false);

            int rowIndex = this.rowResizing;
            int colIndex = this.colResizing;

            if (this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                if (size.Width < (int)grid.Model.Options.MinResizeColSize)
                {
                    size.Width = (int)grid.Model.Options.MinResizeColSize;
                }
            }
            else if (this.resizingCellsMode == GridResizingCellsMode.ResizeRow)
            {
                if (size.Height < (int)grid.Model.Options.MinResizeRowSize)
                {
                    size.Height = (int)grid.Model.Options.MinResizeRowSize;
                }
            }

            Rectangle resizeBounds = this.resizeBounds;
            Size originalSize = this.originalSize;
            Point pt;
            if (grid.IsRightToLeft() && this.resizingCellsMode == GridResizingCellsMode.ResizeColumn)
            {
                pt = resizeBounds.Location - size;
            }
            else
            {
                pt = resizeBounds.Location + size;
            }

            Rectangle gridBounds = grid.GridBounds;

            GridRangeInfoList pSelList = grid.Model.Selections.Ranges;

            inResizingCells = false;

            //// Undo previous inversion.
            ////            if (this.drawInvertLine && this.invertBarBounds != this.originalInvertBarBounds)
            ////            {
            ////                Rectangle r = this.invertBarBounds;
            ////                r.Intersect(grid.GridBounds);
            ////                DrawInvertRect(r, false);
            ////            }

            if ((this.resizingCellsMode & GridResizingCellsMode.ResizeColumn) != 0)
            {
                // Change the column widths and update the display.
                Rectangle invertBarBounds = this.invertBarBounds;
                Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
                int dx = invertBarBounds.Left - originalInvertBarBounds.Left;
                if (grid.IsRightToLeft())
                {
                    dx = -dx;
                }

                int newWidth = dx + grid.GetColWidth(this.colResizing);
                Debug.Assert(dx + grid.GetColWidth(this.colResizing) >= 0);

                // Trigger event and check return value.
                if (RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref newWidth, GridResizeCellsReason.MouseUp, point)
                    && Math.Abs(dx) > 1 && this.movedMarker)
                {
                    if (this.colResizing > 0 && HasResizeColOptions(GridResizeCellsBehavior.ResizeAll))
                    {
                        if (RaiseResizingColumns(GridRangeInfo.Table(), ref newWidth, GridResizeCellsReason.MouseUp, point))
                        {
                            grid.Model.Cols.DefaultSize = newWidth;
                        }
                    }
                    else if (this.colResizing > 0 && pSelList.AnyRangeContains(GridRangeInfo.Col(this.colResizing)))
                    {
                        if (pSelList.AnyRangeContains(GridRangeInfo.Table()))
                        {
                            if (RaiseResizingColumns(GridRangeInfo.Table(), ref newWidth, GridResizeCellsReason.MouseUp, point))
                            {
                                grid.Model.Cols.DefaultSize = newWidth;
                            }
                        }
                        else
                        {
                            GridRangeInfoList rl = pSelList.GetColRanges(GridRangeInfoType.Cols);

                            //// TODO: WaitCursor?

                            foreach (GridRangeInfo range in rl)
                            {
                                if (RaiseResizingColumns(range, ref newWidth, GridResizeCellsReason.MouseUp, point))
                                {
                                    if (newWidth > 0)
                                    {
                                        grid.Model.ColWidths.SetRange(range.Left, range.Right, newWidth);
                                    }
                                    else
                                    {
                                        grid.Model.HideCols.SetRange(range.Left, range.Right, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (newWidth > 0)
                        {
                            grid.Model.ColWidths[this.colResizing] = newWidth;
                        }
                        else
                        {
                            grid.Model.HideCols[this.colResizing] = true;
                        }
                    }
                }
                else
                {
                    grid.InternalInvalidate(grid.RangeInfoToRectangle(GridRangeInfo.Cell(0, this.colResizing)));
                    int nOldWidth = grid.GetColWidth(this.colResizing);
                    UpdateColWidths(this.colResizing, this.colResizing, new int[] { nOldWidth });
                }

                // Just in some rare case that column is not in view area.
                grid.ScrollCellInView(grid.TopRowIndex, this.colResizing, GridScrollCurrentCellReason.ResizedCells);
            }
            else
            {
                // Change the row heights and update the display.
                Rectangle invertBarBounds = this.invertBarBounds;
                Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
                int dy = invertBarBounds.Top - originalInvertBarBounds.Top;
                Debug.Assert(dy + grid.GetRowHeight(this.rowResizing) >= 0);
                int newHeight = dy + grid.GetRowHeight(this.rowResizing);

                // Trigger event and check return value.
                if (RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref newHeight, GridResizeCellsReason.MouseUp, point)
                    && Math.Abs(dy) > 1 && this.movedMarker)
                {
                    if (this.rowResizing > 0 && HasResizeRowOptions(GridResizeCellsBehavior.ResizeAll))
                    {
                        if (RaiseResizingRows(GridRangeInfo.Table(), ref newHeight, GridResizeCellsReason.MouseUp, point))
                        {
                            grid.Model.Rows.DefaultSize = newHeight;
                        }
                    }
                    else if (this.rowResizing > 0 && pSelList.AnyRangeContains(GridRangeInfo.Row(this.rowResizing)))
                    {
                        if (pSelList.AnyRangeContains(GridRangeInfo.Table()))
                        {
                            if (RaiseResizingRows(GridRangeInfo.Table(), ref newHeight, GridResizeCellsReason.MouseUp, point))
                            {
                                grid.Model.Rows.DefaultSize = newHeight;
                            }
                        }
                        else
                        {
                            GridRangeInfoList rl = pSelList.GetRowRanges(GridRangeInfoType.Rows);

                            //// TODO: WaitCursor? 

                            foreach (GridRangeInfo range in rl)
                            {
                                if (RaiseResizingRows(range, ref newHeight, GridResizeCellsReason.MouseUp, point))
                                {
                                    if (newHeight > 0)
                                    {
                                        grid.Model.RowHeights.SetRange(range.Top, range.Bottom, newHeight);
                                    }
                                    else
                                    {
                                        grid.Model.HideRows.SetRange(range.Top, range.Bottom, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (newHeight > 0)
                        {
                            grid.Model.RowHeights[this.rowResizing] = newHeight;
                        }
                        else
                        {
                            grid.Model.HideRows[this.rowResizing] = true;
                        }
                    }
                }
                else
                {
                    grid.InternalInvalidate(grid.RangeInfoToRectangle(GridRangeInfo.Cell(this.rowResizing, 0)));
                    int nOldHeight = grid.GetRowHeight(this.rowResizing);
                    UpdateRowHeights(this.rowResizing, this.rowResizing, new int[] { nOldHeight });
                }

                // just in some rare case that row is not in view area
                grid.ScrollCellInView(this.rowResizing, grid.LeftColIndex, GridScrollCurrentCellReason.ResizedCells);
            }
        }

        void UpdateRowHeights(int fromRowIndex, int toRowIndex, int[] savedHeights)
        {
            grid.ViewLayout.Reset();
            GridRangeInfo rowRange = GridRangeInfo.Rows(fromRowIndex, toRowIndex);
            GridRangeInfo range = grid.ViewLayout.CombineSpannedRanges(rowRange);
            if (grid.ViewLayout.IsRangeVisible(range))
            {
                grid.InternalInvalidate(grid.ViewLayout.RectangleBottomOfRow(range.Top, GridCellSizeKind.VisibleSize));
            }
        }

        void UpdateColWidths(int fromColIndex, int toColIndex, int[] savedWidths)
        {
            grid.ViewLayout.Reset();
            Rectangle bounds = grid.GridBounds;

            GridRangeInfo colRange = GridRangeInfo.Cols(fromColIndex, toColIndex);

            GridRangeInfo range = grid.ViewLayout.CombineSpannedRanges(colRange);
            if (grid.Model.Options.FloatCellsMode != GridFloatCellsMode.None)
            {
                grid.Model.FloatingCells.DelayFloatCells(range);
                grid.InternalInvalidate(grid.GridBounds);
            }
            else if (grid.ViewLayout.IsRangeVisible(range))
            {
                grid.InternalInvalidate(grid.ViewLayout.RectangleRightOfCol(range.Left, GridCellSizeKind.VisibleSize));
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal sealed class ResizeCellsHitTestInfo
        {
            const int hitTestFrame = 4;
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point;
            internal Rectangle cellBounds = Rectangle.Empty;
            internal int clientCol;
            internal int clientRow;
            internal int rowIndex;
            internal int colIndex;
            internal GridResizeCellsMouseController resizeUI;

            internal ResizeCellsHitTestInfo(GridResizeCellsMouseController resizeUI, GridControlBase grid, Point point, GridResizeCellsReason reason)
            {
                this.resizeUI = resizeUI;
                this.point = point;
                clientCol = grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                ////if (grid.IsRightToLeft())
                ////    clientCol--;
                clientRow = grid.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                if (clientCol >= 0 && clientRow >= 0)
                {
                    rowIndex = grid.GetRow(clientRow);
                    colIndex = grid.GetCol(clientCol);

                    // special case for last row and column
                    if (colIndex == grid.Model.ColCount + 1 && rowIndex == grid.Model.RowCount + 1)
                    {
                        cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex - 1, colIndex - 1));
                        cellBounds.Offset(grid.Model.ColWidths[colIndex - 1], grid.Model.RowHeights[rowIndex - 1]);
                    }
                    else if (colIndex == grid.Model.ColCount + 1)
                    {
                        cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex - 1));
                        cellBounds.Offset(grid.Model.ColWidths[colIndex - 1], 0);
                    }
                    else if (rowIndex == grid.Model.RowCount + 1)
                    {
                        cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex - 1, colIndex));
                        cellBounds.Offset(0, grid.Model.RowHeights[rowIndex - 1]);
                    }
                    else
                    {
                        cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
                    }

                    if (clientCol <= grid.ViewLayout.VisibleCols && clientRow <= grid.ViewLayout.VisibleRows
                        && rowIndex <= grid.Model.RowCount + 1 && colIndex <= grid.Model.ColCount + 1)
                    {
                        bool shouldResizeCol = resizeUI.CanResizeCols
                            && (clientRow == 0 || resizeUI.HasResizeColOptions(GridResizeCellsBehavior.InsideGrid));

                        bool shouldResizeRow = resizeUI.CanResizeRows
                            && (clientCol == 0 || resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.InsideGrid));

                        if(grid.AllowRowResizeUsingCellBoundaries)
                            shouldResizeRow = resizeUI.CanResizeRows
                                && (clientCol >= 0 || resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.InsideGrid));
                        
                        if(grid.AllowColumnResizeUsingCellBoundaries)
                            shouldResizeCol = resizeUI.CanResizeCols
                                && (clientRow >= 0 || resizeUI.HasResizeColOptions(GridResizeCellsBehavior.InsideGrid));

                        if (grid.IsRightToLeft() && shouldResizeCol && Math.Abs(cellBounds.Left - point.X) <= hitTestFrame / 2)
                        {
                            if (clientCol > grid.InternalGetHeaderCols() || !resizeUI.HasResizeColOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                hitTestResult = GridHitTestContext.VerticalLine;
                            }
                        }
                        else if (grid.IsRightToLeft() && shouldResizeCol && Math.Abs(cellBounds.Right - point.X) <= hitTestFrame / 2 && clientCol > 0)
                        {
                            if (clientCol > grid.InternalGetHeaderCols() + 1 || !resizeUI.HasResizeColOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                while (clientCol > 0 && grid.GetColWidth(grid.GetCol(--clientCol)) == 0)
                                { 
                                }

                                if (clientCol > 0 || grid.ShouldDisplayHeaderCol())
                                {
                                    hitTestResult = GridHitTestContext.VerticalLine;
                                }

                                colIndex = grid.GetCol(clientCol);
                                cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
                            }
                        }
                        else if (!grid.IsRightToLeft() && shouldResizeCol && Math.Abs(cellBounds.Right - point.X) <= hitTestFrame / 2)
                        {
                            if (clientCol > grid.InternalGetHeaderCols() || !resizeUI.HasResizeColOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                hitTestResult = GridHitTestContext.VerticalLine;
                            }
                        }
                        else if (!grid.IsRightToLeft() && shouldResizeCol && Math.Abs(cellBounds.Left - point.X) <= hitTestFrame / 2 && clientCol > 0)
                        {
                            if (clientCol > grid.InternalGetHeaderCols() + 1 || !resizeUI.HasResizeColOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                while (clientCol > 0 && grid.GetColWidth(grid.GetCol(--clientCol)) == 0)
                                { 
                                }

                                if (clientCol > 0 || grid.ShouldDisplayHeaderCol())
                                {
                                    hitTestResult = GridHitTestContext.VerticalLine;
                                }

                                colIndex = grid.GetCol(clientCol);
                                cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
                            }
                        }
                        else if (shouldResizeRow && Math.Abs(cellBounds.Bottom - point.Y) <= hitTestFrame / 2)
                        {
                            if (clientRow > grid.InternalGetHeaderRows() || !resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                hitTestResult = GridHitTestContext.HorizontalLine;
                            }
                        }
                        else if (shouldResizeRow && Math.Abs(cellBounds.Top - point.Y) <= hitTestFrame / 2 && clientRow > 0)
                        {
                            if (clientRow > grid.InternalGetHeaderRows() + 1 || !resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                while (clientRow > 0 && grid.GetRowHeight(grid.GetRow(--clientRow)) == 0)
                                { 
                                }

                                if (clientRow > 0 || grid.ShouldDisplayHeaderRow())
                                {
                                    hitTestResult = GridHitTestContext.HorizontalLine;
                                }

                                rowIndex = grid.GetRow(clientRow);
                                cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
                            }
                        }

                        // special case for last row and column when mouse pointer is right of last column 
                        if (rowIndex > grid.Model.RowCount || colIndex > grid.Model.ColCount)
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
                    }

                    int undefined = -1;
                    if (hitTestResult == GridHitTestContext.VerticalLine)
                    {
                        if (!resizeUI.RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref undefined, reason, point))
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
                    }
                    else if (hitTestResult == GridHitTestContext.HorizontalLine)
                    {
                        if (!resizeUI.RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref undefined, reason, point))
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
                    }
                }
            }
        }

        // IMouseController implementation 

        /// <override/>
        /// <summary>
        /// The name of this mouse controller.
        /// </summary>
        public override string Name
        {
            get
            {
                return "ResizeCells";
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
                if (hitTestInfo != null)
                {
                    return hitTestInfo.hitTestResult == GridHitTestContext.HorizontalLine ? GridCursors.RowHeightCursor : GridCursors.ColumnWidthCursor;
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
                if (e.Clicks == 1)
                {
                    this.LeftMouseDown(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
                }
                else if (e.Clicks == 2)
                {
                    this.LeftMouseDoubleClick(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
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
            if (Control.ModifierKeys != Keys.None)
            {
                grid.NotifyCancelMode();
            }
            else
            {
                Point point = new Point(e.X, e.Y);
                this.LeftMouseMove(point, e);
            }
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            this.LeftMouseUp(point, e);
        }

        /// <override/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
            this.CancelResizing();
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
            // This HitTest code has higher priority than "SelectCells" and "DragCells".
            Point pt = new Point(e.X, e.Y);
            hitTestInfo = null;
            if (e.Button == MouseButtons.Left &&
                (controller == null || controller.Name == "SelectCells" || controller.Name == "DragCells"))
            {
                hitTestInfo = new ResizeCellsHitTestInfo(this, grid, pt, GridResizeCellsReason.HitTest);
                if (hitTestInfo.hitTestResult == GridHitTestContext.None)
                {
                    hitTestInfo = null;
                }
            }

            return hitTestInfo != null ? hitTestInfo.hitTestResult : 0;
        }

        bool RaiseResizingColumns(GridRangeInfo columns, ref int width, GridResizeCellsReason reason, Point point)
        {
            GridResizingColumnsEventArgs e = new GridResizingColumnsEventArgs(columns, width, reason, GridResizeCellsMouseController.sizeIndicatorBorder, GridResizeCellsMouseController.boundsIndicatorBorder, point);
            grid.RaiseResizingColumns(e);
            width = e.Width;
            GridResizeCellsMouseController.sizeIndicatorBorder = e.SizeIndicatorBorder;
            GridResizeCellsMouseController.boundsIndicatorBorder = e.BoundsIndicatorBorder;
            return !e.Cancel;
        }

        bool RaiseResizingRows(GridRangeInfo rows, ref int height, GridResizeCellsReason reason, Point point)
        {
            GridResizingRowsEventArgs e = new GridResizingRowsEventArgs(rows, height, reason, GridResizeCellsMouseController.sizeIndicatorBorder, GridResizeCellsMouseController.boundsIndicatorBorder, point);
            grid.RaiseResizingRows(e);
            height = e.Height;
            GridResizeCellsMouseController.sizeIndicatorBorder = e.SizeIndicatorBorder;
            GridResizeCellsMouseController.boundsIndicatorBorder = e.BoundsIndicatorBorder;
            return !e.Cancel;
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridDragLineWindow : TopLevelWindow
    {
        GridBorder border;
        bool firstTime = true;

        public GridDragLineWindow()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | Syncfusion.Windows.Forms.WhidbeyCompatibleControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.Selectable, false);
            this.TopLevel = true;  // Must be TopLevel to allow transparency
            this.TransparencyKey = Color.White;  // White will also look good on Win98 even though it is not transparent.
        }

        /// <override/>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x80/*WS_EX_TOOLWINDOW*/;
                return cp;
            }
        }

        /// <override/>
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (border == null)
            {
                return;
            }

            Graphics g = pe.Graphics;
            if (Height > Width)
            {
                GridBorderPaint.DrawRectangle(g, border, ClientRectangle, Color.White, GridBorderSide.Left);
            }
            else
            {
                GridBorderPaint.DrawRectangle(g, border, ClientRectangle, Color.White, GridBorderSide.Top);
            }
        }

        public void ShowAt(Rectangle bounds, GridBorder border)
        {
            this.Visible = false;
            this.border = border;
            this.Location = new Point(10000, 10000);
            this.Size = bounds.Size;
            this.ShowWindowTopMost();
            this.Update();
            this.Bounds = bounds;
            this.ShowWindowTopMost();
            if (firstTime)
            {
                this.Bounds = bounds;
                this.Update();
                firstTime = false;
            }
        }

        public void Fade()
        {
            this.Location = new Point(10000, 10000);
            this.Hide();
        }

        public GridBorder Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
            }
        }
    }
}
