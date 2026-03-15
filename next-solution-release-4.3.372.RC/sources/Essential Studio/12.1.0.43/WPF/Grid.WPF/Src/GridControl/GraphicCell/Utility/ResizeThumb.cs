#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Grid
{

#if SILVERLIGHT
    [DesignTimeVisible(false)]
    public class ThumbControl: Control
    {
        
        public ThumbControl()
        {
            DefaultStyleKey = typeof(ThumbControl);
        }

        private Thumb thumb;
        public Thumb Thumb
        {
            get
            {
                return thumb;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            thumb = GetTemplateChild("thumb") as Thumb;
        }
    }
#endif

#if !SILVERLIGHT
    [DesignTimeVisible(false)]
    public class ResizeThumb : Thumb
#else
    [DesignTimeVisible(false)]
    public class ResizeThumb : ThumbControl
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

        public ResizeThumb()
        {
            this.Loaded += new RoutedEventHandler(ResizeThumb_Loaded);
            this.Unloaded += new RoutedEventHandler(ResizeThumb_Unloaded);

        }

        void ResizeThumb_Unloaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            DragStarted -= new DragStartedEventHandler(ResizeThumb_DragStarted);
            DragDelta -= new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted -= new DragCompletedEventHandler(ResizeThumb_DragCompleted);
#else
            this.Thumb.DragStarted -= new DragStartedEventHandler(ResizeThumb_DragStarted);
            this.Thumb.DragDelta -= new DragDeltaEventHandler(ResizeThumb_DragDelta);
            this.Thumb.DragCompleted -= new DragCompletedEventHandler(ResizeThumb_DragCompleted);
#endif
        }

        void ResizeThumb_Loaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            DragStarted += new DragStartedEventHandler(ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
#else
            this.Thumb.DragStarted += new DragStartedEventHandler(ResizeThumb_DragStarted);
            this.Thumb.DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
            this.Thumb.DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
#endif
        }

        void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            cursor = this.Cursor;
            graphicCellControl = this.DataContext as GraphicCellControl;
            if (graphicCellControl == null)
                return;
            graphicCellControl.Focus();
            cellSpanInfo = GraphicCellHelper.GetCellSpanInfo(graphicCellControl);
            IGraphicCellRenderer GraphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            grid = GraphicCellRenderer.GridControl;

            if (!grid.Model.GraphicModel.RaiseGraphicCellResizing(cellSpanInfo, cellSpanInfo.Name))
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

        void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
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
            styleInfo.GraphicModel.RaiseGraphicCellResized(cellSpanInfo, cellSpanInfo.Name);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Cursor = cursor;
#if !SILVERLIGHT
            var point = Mouse.GetPosition(this.grid);
            if (!grid.AutoScroller.AutoScrollBounds.Contains(point))
            {
                this.Cursor = Cursors.No;
                return;
            }
#else
#endif
            graphicCellControl = this.DataContext as GraphicCellControl;
            CalculateRect(graphicCellControl);
            if (graphicCellControl == null)
                return;
            GraphicCellSpanInfo CellSpanInfo = GraphicCellHelper.GetCellSpanInfo(graphicCellControl);
            IGraphicCellRenderer graphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            if (CellSpanInfo == null || graphicCellRenderer == null)
                return;
            if (graphicCellControl != null)
            {
                double horizontalOffset = grid.HorizontalOffset;
                double verticalOffset = grid.VerticalOffset;
                double deltaVertical, deltaHorizontal;
                //Rect rect = AssociatedSpreadsheetChartControl.GraphicCellRenderer.GetArrangeRect();
                Rect rect = VisualContainer.GetRenderBounds(graphicCellControl);
                if (rect.IsEmpty)
                    return;
                Debug.WriteLine("rect - " + rect);
                double x = rect.X;
                double y = rect.Y;
                double minheight = 3, minwidth = 3;
                double height = rect.Height;
                double width = rect.Width;
                Rect ArrangeRect = new Rect(x, y, width, height);
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, graphicCellControl.ActualHeight - graphicCellControl.MinHeight);
                        height -= deltaVertical;
                        ArrangeRect = new Rect(x, y, width, height);
                        if (ArrangeRect.Bottom > lastrowrect.Bottom)
                            height = graphicCellControl.Height + (lastrowrect.Bottom - ArrangeRect.Bottom);
                        if (height < minheight)
                            height = minheight;

                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, graphicCellControl.ActualHeight - graphicCellControl.MinHeight);
                        y += deltaVertical;
                        height -= deltaVertical;
                        if (y < firstrowrect.Top)
                        {
                            height -= (firstrowrect.Top - y);
                            y = firstrowrect.Top;
                        }

                        ArrangeRect = new Rect(x, y, width, height);
                        if (ArrangeRect.Top < firstrowrect.Top)
                        {
                            height = graphicCellControl.Height + (firstrowrect.Top - ArrangeRect.Top);
                            y = firstrowrect.Top;
                        }
                        if (height < minheight)
                        {
                            y = y - (minheight - height);
                            height = minheight;

                        }
                        break;
                    default:
                        break;
                }
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, graphicCellControl.ActualWidth - graphicCellControl.MinWidth);
                        x += deltaHorizontal;
                        width -= deltaHorizontal;
                        if (x < firstcolrect.Left)
                        {
                            width -= (firstcolrect.Left - x);
                            x = firstcolrect.Left;
                        }

                        ArrangeRect = new Rect(x, y, width, height);
                        if (ArrangeRect.Left < firstcolrect.Left)
                        {
                            width = graphicCellControl.Width + (firstcolrect.Left - rect.Left);
                            x = firstcolrect.Left;
                        }
                        if (width < minwidth)
                        {
                            x = x - (minwidth - width);
                            width = minwidth;
                        }
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, graphicCellControl.ActualWidth - graphicCellControl.MinWidth);
                        width -= deltaHorizontal;
                        if (width < minwidth)
                            width = minwidth;

                        ArrangeRect = new Rect(x, y, width, height);
                        if (ArrangeRect.Right > lastcolrect.Right)
                            width = graphicCellControl.Width + (lastcolrect.Right - rect.Right);
                        if (width < minwidth)
                            width = minwidth;
                        break;
                    default:
                        break;
                }

                ArrangeRect = new Rect(x, y, width, height);
                graphicCellControl.Height = height;
                graphicCellControl.Width = width;
                var content = graphicCellControl.Content as FrameworkElement;
                var contentHeight = graphicCellControl.Height - graphicCellControl.BorderThickness.Bottom;
                if (content != null)
                    content.Height = contentHeight;
                var contentWidth = graphicCellControl.Width - graphicCellControl.BorderThickness.Right;
                if (content != null)
                    content.Width = contentWidth;

                graphicCellRenderer.SetBounds(graphicCellControl, ArrangeRect, true, false);
                graphicCellControl.InvalidateArrange();

                RowColumnIndex rowcol = grid.PointToCellRowColumnIndex(new Point(x, y));
                if (!rowcol.IsEmpty && (CellSpanInfo.RowIndex != rowcol.RowIndex || CellSpanInfo.ColumnIndex != rowcol.ColumnIndex))
                {
                    if (rowcol.RowIndex > 0 && VerticalAlignment != VerticalAlignment.Bottom)
                        CellSpanInfo.RowIndex = rowcol.RowIndex;
                    if (rowcol.ColumnIndex > 0 && HorizontalAlignment != HorizontalAlignment.Right)
                        CellSpanInfo.ColumnIndex = rowcol.ColumnIndex;
                }
                Rect cellrect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(cellSpanInfo.RowIndex, cellSpanInfo.ColumnIndex), true, true);

                CellSpanInfo.OffsetX = ArrangeRect.X - cellrect.X;
                CellSpanInfo.OffsetY = ArrangeRect.Y - cellrect.Y;
                CellSpanInfo.Width = graphicCellControl.Width;
                CellSpanInfo.Height = graphicCellControl.Height;
            }
#if !SILVERLIGHT
            e.Handled = true;
#endif
        }

        void AutoScroller_AutoScrollerValueChanged(object sender, AutoScrollerValueChangedEventArgs args)
        {
            IGraphicCellRenderer graphicCellRenderer = GraphicCellHelper.GetGraphicCellRenderer(graphicCellControl);
            CalculateRect(graphicCellControl);
            Rect visibleRect = grid.AutoScroller.AutoScrollBounds;
            if (graphicCellRenderer == null)
                return;
            var graphicCellRect = VisualContainer.GetRenderBounds(graphicCellControl);
            double height = 0, width = 0;
            double minheight = 3, minwidth = 3;
            double x = graphicCellRect.X;
            double y = graphicCellRect.Y;
            if (args.IsLineDown)
            {
                if (VerticalAlignment == VerticalAlignment.Bottom)
                {
                    var diffHeight = visibleRect.Bottom - graphicCellRect.Bottom;
                    height = graphicCellControl.Height + diffHeight;
                    if (height < minheight)
                        height = minheight;
                }
                else
                {
                    var diffHeight = graphicCellRect.Top - visibleRect.Bottom;
                    height = graphicCellControl.Height + diffHeight;
                    y = visibleRect.Bottom;
                    if (height < minheight)
                    {
                        y = y - (minheight - height);
                        height = minheight;
                    }
                    RowColumnIndex rowcol = grid.PointToCellRowColumnIndex(new Point(x, y));
                    cellSpanInfo.RowIndex = rowcol.RowIndex;
                    cellSpanInfo.OffsetY = 0;
                }

                graphicCellControl.Height = height;
                var content = graphicCellControl.Content as FrameworkElement;
                var contentheight = graphicCellControl.Height - graphicCellControl.BorderThickness.Bottom;
                if (content != null)
                    content.Height = contentheight;
            }
            if (args.IsLineUp)
            {
                if (VerticalAlignment == VerticalAlignment.Top)
                {
                    var diffHeight = visibleRect.Bottom - graphicCellRect.Top;
                    height = graphicCellControl.Height + diffHeight;
                    y = visibleRect.Top;
                    if (height < minheight)
                    {
                        y = y - (minheight - height);
                        height = minheight;
                    }
                    RowColumnIndex rowcol = grid.PointToCellRowColumnIndex(new Point(x, y));
                    cellSpanInfo.RowIndex = rowcol.RowIndex;
                    cellSpanInfo.OffsetY = 0;
                }
                else
                {
                    var diffHeight = visibleRect.Top - graphicCellRect.Bottom;
                    height = graphicCellControl.Height + diffHeight;
                    if (height < minheight)
                        height = minheight;
                }
                graphicCellControl.Height = height;
                var content = graphicCellControl.Content as FrameworkElement;
                var contentheight = graphicCellControl.Height - graphicCellControl.BorderThickness.Top;
                if (content != null)
                    content.Height = contentheight;
            }
            if (args.IsLineLeft)
            {
                if (HorizontalAlignment == HorizontalAlignment.Left)
                {
                    var diffWidth = visibleRect.Right - graphicCellRect.Left;
                    width = graphicCellControl.Width + diffWidth;
                    x = visibleRect.Left;
                    if (width < minwidth)
                    {
                        x = x - (minwidth - width);
                        width = minwidth;
                    }
                    RowColumnIndex rowcol = grid.PointToCellRowColumnIndex(new Point(x, y));
                    cellSpanInfo.ColumnIndex = rowcol.ColumnIndex;
                    cellSpanInfo.OffsetX = 0;
                }
                else
                {
                    var diffWidth = visibleRect.Left - graphicCellRect.Right;
                    width = graphicCellControl.Width + diffWidth;
                    if (width < minwidth)
                        width = minwidth;
                }
                graphicCellControl.Width = width;
                var content = graphicCellControl.Content as FrameworkElement;
                var contentWidth = graphicCellControl.Width - graphicCellControl.BorderThickness.Left;
                if (content != null)
                    content.Width = contentWidth;
            }
            if (args.IsLineRight)
            {
                if (HorizontalAlignment == HorizontalAlignment.Right)
                {
                    var diffWidth = visibleRect.Right - graphicCellRect.Right;
                    width = graphicCellControl.Width + diffWidth;
                    if (width < minwidth)
                        width = minwidth;
                }
                else
                {
                    var diffWidth = graphicCellRect.Left - visibleRect.Right;
                    width = graphicCellControl.Width + diffWidth;
                    x = visibleRect.Right;
                    if (width < minwidth)
                    {
                        x = x - (minwidth - width);
                        width = minwidth;
                    }
                    RowColumnIndex rowcol = grid.PointToCellRowColumnIndex(new Point(x, y));
                    cellSpanInfo.ColumnIndex = rowcol.ColumnIndex;
                    cellSpanInfo.OffsetX = 0;
                }
                graphicCellControl.Width = width;
                var content = graphicCellControl.Content as FrameworkElement;
                var contentWidth = graphicCellControl.Width - graphicCellControl.BorderThickness.Right;
                if (content != null)
                    content.Width = contentWidth;
            }

            cellSpanInfo.Width = graphicCellControl.Width;
            cellSpanInfo.Height = graphicCellControl.Height;

            Rect ArrangeRect = new Rect(x, y, graphicCellControl.Width, graphicCellControl.Height);
            graphicCellRenderer.SetBounds(graphicCellControl, ArrangeRect, true, false);
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
