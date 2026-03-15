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
using System.Windows.Controls.Primitives;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls;
using Syncfusion.Windows.GridCommon;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Grid
{
#if !SILVERLIGHT
    [DesignTimeVisible(false)]
    public class MoveThumb : Thumb
#else
    [DesignTimeVisible(false)]
    public class MoveThumb : ThumbControl
#endif
    {
        private GraphicCellControl graphicCellControl;
        private GridControlBase grid;
        private GraphicCellSpanInfo cellSpanInfo;
        private Cursor cursor;
        private int firstcol;
        private int firstrow;
        private int lastcol;
        private int lastrow;
        private Rect headerCellRect;
        private Rect firstrowrect;
        private Rect lastrowrect;
        private Rect firstcolrect;
        private Rect lastcolrect;
        public MoveThumb()
        {
            this.Loaded += new RoutedEventHandler(MoveThumb_Loaded);
            this.Unloaded += new RoutedEventHandler(MoveThumb_Unloaded);
        }

        void MoveThumb_Unloaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            DragStarted -= new DragStartedEventHandler(MoveThumb_DragStarted);
            DragDelta -= new DragDeltaEventHandler(this.MoveThumb_DragDelta);
            DragCompleted -= new DragCompletedEventHandler(MoveThumb_DragCompleted);
#else
            if (Thumb != null)
            {
                this.Thumb.DragStarted -= new DragStartedEventHandler(MoveThumb_DragStarted);
                this.Thumb.DragDelta -= new DragDeltaEventHandler(MoveThumb_DragDelta);
                this.Thumb.DragCompleted -= new DragCompletedEventHandler(MoveThumb_DragCompleted);
            }
#endif
        }

        void MoveThumb_Loaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            DragStarted += new DragStartedEventHandler(MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(MoveThumb_DragCompleted);
#else
            if (Thumb != null)
            {
                this.Thumb.DragStarted += new DragStartedEventHandler(MoveThumb_DragStarted);
                this.Thumb.DragDelta += new DragDeltaEventHandler(MoveThumb_DragDelta);
                this.Thumb.DragCompleted += new DragCompletedEventHandler(MoveThumb_DragCompleted);
            }
#endif
        }

        void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            cursor = this.Cursor;
            graphicCellControl = this.DataContext as GraphicCellControl;
            if (graphicCellControl == null)
                return;
            graphicCellControl.Focus();
            cellSpanInfo = GraphicCellHelper.GetCellSpanInfo(graphicCellControl);
            IGraphicCellRenderer GraphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            grid = GraphicCellRenderer.GridControl;

            if (!grid.Model.GraphicModel.RaiseGraphicCellMoving(cellSpanInfo, cellSpanInfo.Name))
            {
#if !SILVERLIGHT
                e.Handled = true;
#endif
                return;
            }
            grid.AutoScroller.IntervalTime = new TimeSpan(0, 0, 0, 0, 100);
            grid.AutoScroller.AutoScrollBounds = GetAutoScrollBoundsRect(graphicCellControl);
            grid.AutoScroller.InsideScrollMargins = new Size(15, 15);
            grid.AutoScroller.AllowScrollOutsideBounds = false;
            grid.AutoScroller.AutoScrollerValueChanged += new AutoScroller.AutoScrollerValueChangedEventHandler(AutoScroller_AutoScrollerValueChanged);
        }

        void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.Cursor = cursor;
            graphicCellControl = this.DataContext as GraphicCellControl;
            if (graphicCellControl == null)
                return;
            cellSpanInfo = GraphicCellHelper.GetCellSpanInfo(graphicCellControl);
            IGraphicCellRenderer GraphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            grid = GraphicCellRenderer.GridControl;
            grid.AutoScroller.IntervalTime = new TimeSpan(0, 0, 0, 0, 300);
            grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            grid.AutoScroller.AllowScrollOutsideBounds = true;
            grid.AutoScroller.AutoScrollerValueChanged -= new AutoScroller.AutoScrollerValueChangedEventHandler(AutoScroller_AutoScrollerValueChanged);
            var styleInfo = GraphicCellHelper.GetStyleInfo(graphicCellControl);
            styleInfo.GraphicModel.RaiseGraphicCellMoved(cellSpanInfo, cellSpanInfo.Name);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
#if !SILVERLIGHT
            this.Cursor = cursor;
            var point = Mouse.GetPosition(this.grid);
            if (!grid.AutoScroller.AutoScrollBounds.Contains(point))
            {
                this.Cursor = Cursors.No;
                return;
            }
#endif
            graphicCellControl = this.DataContext as GraphicCellControl;
            CalculateRect(graphicCellControl);
            if (graphicCellControl == null)
                return;
            cellSpanInfo = GraphicCellHelper.GetCellSpanInfo(graphicCellControl);
            IGraphicCellRenderer graphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            if (cellSpanInfo == null || graphicCellRenderer == null)
                return;
            grid = graphicCellRenderer.GridControl;
            double diffX = e.HorizontalChange;
            double diffY = e.VerticalChange;
            RowColumnIndex rowcol = RowColumnIndex.Empty;
            var graphicCellRect = VisualContainer.GetRenderBounds(graphicCellControl);

            Rect newrect = new Rect(graphicCellRect.X + diffX, graphicCellRect.Y + diffY, graphicCellControl.Width, graphicCellControl.Height);
            double x = newrect.X;
            double y = newrect.Y;

            if (x <= firstcolrect.Left)
            {
                x = firstcolrect.Left;
                rowcol.ColumnIndex = firstcol;
                cellSpanInfo.ColumnIndex = firstcol;
            }
            else if (newrect.Right > lastcolrect.Right)
                x -= newrect.Right - lastcolrect.Right;

            if (x > headerCellRect.Right)
                rowcol.ColumnIndex = grid.PointToCellRowColumnIndex(new Point(x, y)).ColumnIndex;

            else if (rowcol.ColumnIndex <= 0)
            {
                var width = grid.GetColWidth(cellSpanInfo.ColumnIndex);
                var offsetx = cellSpanInfo.OffsetX;
                var diffchange = x - graphicCellRect.X;
                if (diffchange > 0 && (diffchange - offsetx) > width && cellSpanInfo.ColumnIndex < lastcol)
                    rowcol.ColumnIndex = cellSpanInfo.ColumnIndex + 1;
                else if (diffchange < 0 && (offsetx - diffchange) > width && cellSpanInfo.ColumnIndex > firstcol)
                    rowcol.ColumnIndex = cellSpanInfo.ColumnIndex - 1;
                else
                    rowcol.ColumnIndex = cellSpanInfo.ColumnIndex;
            }
            else if (rowcol.ColumnIndex != cellSpanInfo.ColumnIndex)
                rowcol.ColumnIndex = cellSpanInfo.ColumnIndex;

            if (y <= firstrowrect.Top)
            {
                y = firstrowrect.Top;
                rowcol.RowIndex = firstrow;
                cellSpanInfo.RowIndex = firstrow;
            }
            else if (newrect.Bottom > lastrowrect.Bottom)
                y -= newrect.Bottom - lastrowrect.Bottom;

            if (y > headerCellRect.Bottom)
                rowcol.RowIndex = grid.PointToCellRowColumnIndex(new Point(x, y)).RowIndex;
            else if (rowcol.RowIndex <= 0)
            {
                var height = grid.GetRowHeight(cellSpanInfo.RowIndex);
                var offsety = cellSpanInfo.OffsetY;
                var diffchange = y - graphicCellRect.Y;
                if (diffchange > 0 && (diffchange - offsety) > height && cellSpanInfo.RowIndex < lastrow)
                    rowcol.RowIndex = cellSpanInfo.RowIndex + 1;
                else if (diffchange < 0 && (offsety - diffchange) > height && cellSpanInfo.RowIndex > firstrow)
                    rowcol.RowIndex = cellSpanInfo.RowIndex - 1;
                else
                    rowcol.RowIndex = cellSpanInfo.RowIndex;
            }
            else if (rowcol.RowIndex != cellSpanInfo.RowIndex)
                rowcol.RowIndex = cellSpanInfo.RowIndex;

            Rect cellrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(rowcol.RowIndex, rowcol.ColumnIndex), true, true);
            Rect arrangeRect = new Rect(x, y, graphicCellControl.Width, graphicCellControl.Height);
            cellSpanInfo.RowIndex = rowcol.RowIndex;
            cellSpanInfo.ColumnIndex = rowcol.ColumnIndex;
            cellSpanInfo.OffsetX = arrangeRect.X - cellrect.X < 0 ? 0 : arrangeRect.X - cellrect.X;
            cellSpanInfo.OffsetY = arrangeRect.Y - cellrect.Y < 0 ? 0 : arrangeRect.Y - cellrect.Y;
            graphicCellRenderer.SetBounds(graphicCellControl, arrangeRect, true, false);
            graphicCellControl.InvalidateArrange();
        }

        void AutoScroller_AutoScrollerValueChanged(object sender, AutoScrollerValueChangedEventArgs args)
        {
            IGraphicCellRenderer graphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            if (graphicCellRenderer == null)
                return;
            var graphicCellRect = VisualContainer.GetRenderBounds(graphicCellControl);
            CalculateRect(graphicCellControl);
            double x = graphicCellRect.X;
            double y = graphicCellRect.Y;
            double offsetX = cellSpanInfo.OffsetX;
            double offsetY = cellSpanInfo.OffsetY;
            if (args.IsLineDown)
            {
                if (cellSpanInfo.RowIndex < lastrow)
                {
                    if (graphicCellRect.Bottom >= lastrowrect.Bottom)
                    {
                        y = graphicCellRect.Top + (lastrowrect.Bottom - graphicCellRect.Bottom);
                    }
                    else
                    {
                        y = graphicCellRect.Top + grid.GetRowHeight(cellSpanInfo.RowIndex);
                        cellSpanInfo.RowIndex += 1;
                        if (y + graphicCellRect.Height > lastrowrect.Bottom)
                            y = graphicCellRect.Top + (lastrowrect.Bottom - (y + graphicCellRect.Height));
                    }
                }
                else if (graphicCellRect.Bottom != lastrowrect.Bottom)
                    y = graphicCellRect.Top + (lastrowrect.Bottom - graphicCellRect.Bottom);
            }
            if (args.IsLineUp)
            {
                if (cellSpanInfo.RowIndex > firstrow)
                {
                    if (graphicCellRect.Top <= firstrowrect.Top)
                    {
                        y = firstrowrect.Top;
                        cellSpanInfo.RowIndex = firstrow;
                    }
                    else
                    {
                        y = graphicCellRect.Top - (grid.GetRowHeight(cellSpanInfo.RowIndex));// + offsetY);
                        cellSpanInfo.RowIndex -= 1;
                        if (y < firstrowrect.Top)
                            y = firstrowrect.Top;
                    }
                }
                else if (graphicCellRect.Top != firstrowrect.Top)
                    y = firstrowrect.Top;
            }
            if (args.IsLineLeft)
            {
                if (cellSpanInfo.ColumnIndex > firstcol)
                {
                    if (graphicCellRect.Left <= firstcolrect.Left)
                    {
                        x = firstcolrect.Left;
                        cellSpanInfo.ColumnIndex = firstcol;
                    }
                    else
                    {
                        x = graphicCellRect.Left - (grid.GetColWidth(cellSpanInfo.ColumnIndex));// + offsetX);
                        cellSpanInfo.ColumnIndex -= 1;
                        if (x < firstcolrect.Left)
                            x = firstcolrect.Left;
                    }
                }
                else if (graphicCellRect.Left != firstcolrect.Left)
                    x = firstcolrect.Left;

            }
            if (args.IsLineRight)
            {
                if (cellSpanInfo.ColumnIndex < lastcol)
                {
                    if (graphicCellRect.Right >= lastcolrect.Right)
                    {
                        x = graphicCellRect.Left + (lastcolrect.Right - graphicCellRect.Right);
                    }
                    else
                    {
                        x = graphicCellRect.Left + grid.GetColWidth(cellSpanInfo.ColumnIndex);
                        cellSpanInfo.ColumnIndex += 1;
                        if (x + graphicCellRect.Width > lastcolrect.Right)
                            x = graphicCellRect.Left + (lastcolrect.Right - (x + graphicCellRect.Width));
                    }
                }
                else if (graphicCellRect.Right != lastcolrect.Right)
                    x = graphicCellRect.Left + (lastcolrect.Right - graphicCellRect.Right);

            }

            cellSpanInfo.OffsetX = offsetX;
            cellSpanInfo.OffsetY = offsetY;
            Rect arrangeRect = new Rect(x, y, graphicCellControl.Width, graphicCellControl.Height);
            graphicCellRenderer.SetBounds(graphicCellControl, arrangeRect, true, false);
            graphicCellControl.InvalidateArrange();
        }

        Rect GetAutoScrollBoundsRect(UIElement element)
        {
            grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Both;
            Rect rect = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            
            ScrollControlChildFrame canvas = VisualTreeHelper.GetParent(element) as ScrollControlChildFrame;
            if (canvas.IsAtLeftSide && canvas.IsAtTop)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                rect = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header);
            }
            else if (canvas.IsAtLeftSide)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Vertical;
                rect = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Header);
            }
            else if (canvas.IsAtTop)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
                rect = grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body);
            }
            else if (canvas.IsAtRightSide && canvas.IsAtBottom)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
                rect = grid.GetClipRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Footer);
            }
            else if (canvas.IsAtRightSide)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Vertical;
                rect = grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Footer);
            }
            else if (canvas.IsAtBottom)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
                rect = grid.GetClipRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Body);
            }
            return rect;
        }

        void CalculateRect(UIElement element)
        {
            firstrow = grid.Model.FrozenRows;
            firstcol = grid.Model.FrozenColumns;
            lastrow = grid.Model.RowCount - 1;
            lastcol = grid.Model.ColumnCount - 1;
            headerCellRect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header, GridRangeInfo.Cell(firstrow - 1, firstcol - 1), true, true);
            firstrowrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Row(firstrow), true, true);
            lastrowrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Row(lastrow), true, true);
            firstcolrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Col(firstcol), true, true);
            lastcolrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Col(lastcol), true, true);

            ScrollControlChildFrame canvas = VisualTreeHelper.GetParent(element) as ScrollControlChildFrame;
            if (canvas.IsAtLeftSide && canvas.IsAtTop)
            {
                firstrow = grid.Model.HeaderRows;
                firstcol = grid.Model.HeaderColumns;
                lastrow = grid.Model.FrozenRows - 1;
                lastcol = grid.Model.FrozenColumns - 1;
                firstrowrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header, GridRangeInfo.Row(firstrow), true, true);
                lastrowrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header, GridRangeInfo.Row(lastrow), true, true);
                firstcolrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header, GridRangeInfo.Col(firstcol), true, true);
                lastcolrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header, GridRangeInfo.Col(lastcol), true, true);
            }
            else if (canvas.IsAtLeftSide)
            {
                firstrow = grid.Model.FrozenRows;
                firstcol = grid.Model.HeaderColumns;
                lastrow = grid.Model.RowCount - 1;
                lastcol = grid.Model.FrozenColumns - 1;
                firstrowrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Header, GridRangeInfo.Row(firstrow), true, true);
                lastrowrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Header, GridRangeInfo.Row(lastrow), true, true);
                firstcolrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Header, GridRangeInfo.Col(firstcol), true, true);
                lastcolrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Header, GridRangeInfo.Col(lastcol), true, true);
            }
            else if (canvas.IsAtTop)
            {
                firstrow = grid.Model.HeaderRows;
                firstcol = grid.Model.FrozenColumns;
                lastrow = grid.Model.FrozenRows - 1;
                lastcol = grid.Model.ColumnCount - 1;
                firstrowrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Row(firstrow), true, true);
                lastrowrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Row(lastrow), true, true);
                firstcolrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Col(firstcol), true, true);
                lastcolrect = grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Col(lastcol), true, true);
            }
            else if (canvas.IsAtRightSide && canvas.IsAtBottom)
            {
                firstrow = grid.Model.RowCount - grid.Model.FooterRows;
                firstcol = grid.Model.ColumnCount - grid.Model.FooterColumns;
                lastrow = grid.Model.RowCount - 1;
                lastcol = grid.Model.ColumnCount - 1;
                firstrowrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Footer, GridRangeInfo.Row(firstrow), true, true);
                lastrowrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Footer, GridRangeInfo.Row(lastrow), true, true);
                firstcolrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Footer, GridRangeInfo.Col(firstcol), true, true);
                lastcolrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Footer, GridRangeInfo.Col(lastcol), true, true);
            }
            else if (canvas.IsAtRightSide)
            {
                firstrow = grid.Model.FrozenRows;
                firstcol = grid.Model.ColumnCount - grid.Model.FooterColumns;
                lastrow = grid.Model.RowCount - grid.Model.FooterRows - 1;
                lastcol = grid.Model.ColumnCount - 1;
                firstrowrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Footer, GridRangeInfo.Row(firstrow), true, true);
                lastrowrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Footer, GridRangeInfo.Row(lastrow), true, true);
                firstcolrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Footer, GridRangeInfo.Col(firstcol), true, true);
                lastcolrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Footer, GridRangeInfo.Col(lastcol), true, true);
            }
            else if (canvas.IsAtBottom)
            {
                firstrow = grid.Model.RowCount - grid.Model.FooterRows;
                firstcol = grid.Model.FrozenColumns;
                lastrow = grid.Model.RowCount - 1;
                lastcol = grid.Model.ColumnCount - grid.Model.FooterColumns - 1;
                firstrowrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Body, GridRangeInfo.Row(firstrow), true, true);
                lastrowrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Body, GridRangeInfo.Row(lastrow), true, true);
                firstcolrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Body, GridRangeInfo.Col(firstcol), true, true);
                lastcolrect = grid.RangeToRect(ScrollAxisRegion.Footer, ScrollAxisRegion.Body, GridRangeInfo.Col(lastcol), true, true);
            }
        }
    }
}
