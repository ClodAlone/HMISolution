#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Linq;
using System.Collections.Generic;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Syncfusion.Data.Extensions;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class  OrientedCellsPanel : Panel, IDisposable
    {
        #region Fields

        internal Func<IList<IColumnElement>> GetVisibleColumns;
        internal Func<int, VisibleLineInfo> GetColumnVisibleLineInfo;
        internal Func<int,bool, double> GetVisibleColumnSize;

        #endregion

        #region Ctor

        public OrientedCellsPanel()
        {

        }

        #endregion

        #region override Methods

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.GetVisibleColumns == null)
                return base.MeasureOverride(availableSize);
            EnsureItems(availableSize);
            return availableSize;
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.GetVisibleColumns == null)
                return base.ArrangeOverride(finalSize);
            ArrangeColumns(finalSize);
            return finalSize;
        }

        #endregion

        #endregion

        #region private methods

        private void EnsureItems(Size availableSize)
        {
            double rowHeight = availableSize.Height;
            if (this.GetVisibleColumns == null)
                return;
            var visibleColumns = this.GetVisibleColumns();
            //if (VisibleColumns.Count != this.Children.Count)
            {
                foreach (IColumnElement column in visibleColumns)
                {
                    if (column.Element.Visibility == Visibility.Visible && column.Index>=0)
                    {
                        if (!this.Children.Contains(column.Element))
                        {
                            this.Children.Add(column.Element);
                            column.UpdateCellStyle();
                        }
                        var size = new Size(GetVisibleColumnSize(column.Index,false), rowHeight);
                        if (column.Renderer != null) //Right now SpannedRows won't have the CellRenderers
                            column.Renderer.Measure(new RowColumnIndex(column.RowIndex, column.Index), column.Element, size);
                        else
                            column.Element.Measure(size);
                    }
                }
            }
        }

        private void ArrangeColumns(Size finalSize)
        {
            var rowHeight = finalSize.Height;
            if (this.GetVisibleColumns == null)
                return;
            var visibleColumns = this.GetVisibleColumns();
            foreach (var column in visibleColumns)
            {
                if (column.Element.Visibility != Visibility.Visible) continue;
                double newOrigin = 0;
                double ClippedWidth = 0;
                var lineInfo = new List<VisibleLineInfo>();
                var line = GetColumnVisibleLineInfo(column.Index);
                
                var lineSize = this.GetVisibleColumnSize(column.Index,false);
                double clippedSize = 0;

                // calculating clipping and origin for covered column
                if (line == null)
                {
                    for (int i = column.Index; i <= column.ColumnSpan + column.Index; i++)
                    {
                        var newLine = GetColumnVisibleLineInfo(i);
                        if (newLine != null)
                        {
                            newOrigin = newLine.Origin;
                            if ((newLine.IsClippedBody && newLine.IsClippedOrigin))
                                column.Element.Clip = new RectangleGeometry() { Rect = new Rect(clippedSize + (newLine.Size - newLine.ClippedSize), 0, lineSize, (rowHeight + (column.RowSpan * rowHeight))) };
                            else if (newLine.IsClippedBody && newLine.IsClippedCorner)
                                column.Element.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, newLine.ClippedSize + clippedSize, (rowHeight + (column.RowSpan * rowHeight))) };
                            else
                            {
                                if (clippedSize != 0)
                                    column.Element.Clip = new RectangleGeometry() { Rect = new Rect(clippedSize, 0, lineSize, (rowHeight + (column.RowSpan * rowHeight))) };
                                else
                                    if (column.Element.Clip != null)
                                        column.Element.Clip = null;
                            }

                            break;
                        }
                        else
                        {
                            clippedSize += this.GetVisibleColumnSize(i, true);
                            continue;
                        }
                    }
                }

                // if the column is not a covered column
                if (line != null)
                {
                    if (line.IsClippedBody && line.IsClippedOrigin && line.IsClippedCorner)
                    {
                        if (clippedSize == 0)
                            column.Element.Clip = new RectangleGeometry() { Rect = new Rect(line.Size - (line.ClippedSize + line.ClippedCornerExtent), 0, lineSize, (rowHeight + (column.RowSpan * rowHeight))) };
                        else
                            column.Element.Clip = new RectangleGeometry() { Rect = new Rect(lineSize - (clippedSize + line.ClippedCornerExtent), 0, lineSize, (rowHeight + (column.RowSpan * rowHeight))) };
                    }
                    else if (line.IsClippedBody && line.IsClippedOrigin)
                    {
                        if (clippedSize == 0)
                            column.Element.Clip = new RectangleGeometry() { Rect = new Rect(line.Size - line.ClippedSize, 0, lineSize, (rowHeight + (column.RowSpan * rowHeight))) };
                        else
                            column.Element.Clip = new RectangleGeometry() { Rect = new Rect(lineSize - clippedSize, 0, lineSize, (rowHeight + (column.RowSpan * rowHeight))) };
                    }
                    else if (line.IsClippedBody && line.IsClippedCorner)
                        column.Element.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, line.ClippedSize, (rowHeight + (column.RowSpan * rowHeight))) };
                    else
                    {
                        if (column.Element.Clip != null)
                            column.Element.Clip = null;
                    }
                    Rect rect;
                    if (column.RowSpan != 0)
                        rect = new Rect(line.Origin, -(column.RowSpan * rowHeight), line.Size, (rowHeight + (column.RowSpan * rowHeight)));
                    else
                        rect = new Rect(line.Origin, 0, line.Size, rowHeight);
                    rect.Width = this.GetVisibleColumnSize(column.Index, false) - ClippedWidth;
                    if (column.Renderer != null)
                    {
                        column.Renderer.Arrange(new RowColumnIndex(column.RowIndex, column.Index), column.Element, rect);
                    }
                    else
                        column.Element.Arrange(rect);
                }
                else
                {
                    newOrigin = newOrigin - clippedSize;
                    Rect rect;
                    if (column.RowSpan != 0)
                        rect = new Rect(newOrigin, -(column.RowSpan * rowHeight), lineSize - ClippedWidth, (rowHeight + (column.RowSpan * rowHeight)));
                    else
                        rect = new Rect(newOrigin, 0, lineSize - ClippedWidth, rowHeight);
                    if (column.Renderer != null)
                        column.Renderer.Arrange(new RowColumnIndex(column.RowIndex, column.Index), column.Element, rect);
                    else
                        column.Element.Arrange(rect);
                }
            }
        }
    
        #endregion


        public void Dispose()
        {
            this.GetVisibleColumns = null;
            this.GetColumnVisibleLineInfo = null;
            this.GetVisibleColumnSize = null;
            this.Children.Clear();
        }
    }
}
