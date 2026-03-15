#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Syncfusion.Data.Extensions;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using System.Linq;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
using System.Diagnostics;


namespace Syncfusion.UI.Xaml.Grid
{
    public class DetailsViewDataRow : GridDataRow
    {
        #region Fields

        public DetailsViewDataGrid DetailsViewDataGrid { get; internal set; }

        public DetailsViewContentPresenter DetailsViewContentPresenter { get; internal set; }

        public GridDetailsViewIndentCell DetailsViewIndentCell { get; internal set; }

        internal int CatchedRowIndex;

        #endregion

        public DetailsViewDataRow()
        {
            DetailsViewContentPresenter = new DetailsViewContentPresenter();
            DetailsViewIndentCell = new GridDetailsViewIndentCell();
        }

        #region override Methods

        protected override VirtualizingCellsControl OnCreateRowElement()
        {
            DetailsViewContentPresenter.Content = DetailsViewDataGrid;
            var row = new DetailsViewRowControl
            {
                DataContext = this.RowData,
                Visibility = this.RowVisibility
            };
            UpdateRowStyles(row);
            row.InitializeDetailsViewRowControl(GetVisibleColumns, GetCellPosition, GetDetailsViewInfo);

            return row;
        }

        protected override void OnGenerateVisibleColumns(VisibleLinesCollection visibleColumnLines)
        {
            this.VisibleColumns.Clear();

            var groupCount = this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0;
            int startIndex = 0;
            if (this.DataGrid.ShowRowHeader)
            {
                if (visibleColumnLines.Any(line => line.LineIndex == 0))
                    CreateRowHeaderColumn(startIndex);
                startIndex++;
            }
            if (this.DataGrid.GridModel.HasGroup)
            {
                for (var index = startIndex; index < groupCount + startIndex; index++)
                {
                    if (visibleColumnLines.Any(line => line.LineIndex == index))
                        this.VisibleColumns.Add(CreateIndentColumn(index));
                }
            }
            var indentDataColumn = CreateDetailsViewIndentColumn(groupCount + (this.DataGrid.ShowRowHeader ? 1 : 0));
            this.VisibleColumns.Add(indentDataColumn);
            var dc = CreateDetailsViewContent(groupCount + 1 + (this.DataGrid.ShowRowHeader ? 1 : 0));
            this.VisibleColumns.Add(dc);
        }

        internal override void EnsureColumns(VisibleLinesCollection visibleColumnLines)
        {
            this.VisibleColumns.ForEach(dataColumn => dataColumn.IsEnsured = false);
            var groupCount = this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0;
            int startIndex = 0;
            if (this.DataGrid.ShowRowHeader)
            {
                if (visibleColumnLines.Any(line => line.LineIndex == 0))
                {
                    var dc = this.VisibleColumns.FirstOrDefault(dataColumn => dataColumn.ColumnIndex == 0);
                    if (dc != null)
                    {
                        dc.IsEnsured = true;
                        if (dc.ColumnVisibility == Visibility.Collapsed)
                            dc.ColumnVisibility = Visibility.Visible;
                    }
                    else
                        CreateRowHeaderColumn(startIndex);
                }
                startIndex++;
            }
            for (var index = startIndex; index < groupCount + startIndex; index++)
            {
                if (visibleColumnLines.Any(line => line.LineIndex == index))
                {
                    var dc = this.VisibleColumns.FirstOrDefault(dataColumn => dataColumn.ColumnIndex == index);
                    if (dc != null)
                    {
                        dc.IsEnsured = true;
                        if (dc.ColumnVisibility == Visibility.Collapsed)
                            dc.ColumnVisibility = Visibility.Visible;
                    }
                    else
                        this.VisibleColumns.Add(CreateIndentColumn(index));
                }

            }

            var indentcolumnindex = groupCount + (this.DataGrid.ShowRowHeader ? 1 : 0);
            if (visibleColumnLines.Any(line => line.LineIndex == indentcolumnindex))
            {
                var indentDataColumn = this.VisibleColumns.FirstOrDefault(dataColumn => dataColumn.ColumnIndex == indentcolumnindex);
                if (indentDataColumn != null)
                {
                    indentDataColumn.IsEnsured = true;
                    if (indentDataColumn.ColumnVisibility == Visibility.Collapsed)
                        indentDataColumn.ColumnVisibility = Visibility.Visible;
                }
                else
                    this.VisibleColumns.Add(CreateDetailsViewIndentColumn(groupCount + (this.DataGrid.ShowRowHeader ? 1 : 0)));
            }

            var detailsViewDataColumn = this.VisibleColumns.FirstOrDefault(dataColumn => dataColumn.ColumnIndex == groupCount + 1 + (this.DataGrid.ShowRowHeader ? 1 : 0));
            if (detailsViewDataColumn != null)
                detailsViewDataColumn.IsEnsured = true;
            else
                this.VisibleColumns.Add(CreateDetailsViewContent(groupCount + 1 + (this.DataGrid.ShowRowHeader ? 1 : 0)));
            this.VisibleColumns.ForEach(column =>
                {
                    if (!column.IsEnsured)
                        CollapseColumn(column);
                });

            if (this.WholeRowElement.ItemsPanel != null)
                this.WholeRowElement.ItemsPanel.InvalidateMeasure();
        }

        internal override void UpdateRowStyles(ContentControl row) { }

        protected override DataColumnBase CreateIndentColumn(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.IsIndentColumn = true;
            dc.IsEnsured = true;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.IsEditing = false;
            dc.GridColumn = null;
            dc.SelectionController = this.DataGrid.SelectionController;
            if (this.RowRegion == Grid.RowRegion.Header)
                dc.ColumnElement = new GridHeaderIndentCell();
            else
                dc.InitializeColumnElement(this.RowData, false);
            return dc;
        }

        private DataColumnBase CreateDetailsViewIndentColumn(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.IsEnsured = true;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.IsEditing = false;
            dc.GridColumn = null;
            dc.SelectionController = this.DataGrid.SelectionController;
            dc.ColumnElement = DetailsViewIndentCell;
            return dc;
        }

        private DataColumnBase CreateDetailsViewContent(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.IsEnsured = true;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.IsEditing = false;
            dc.GridColumn = null;
            dc.SelectionController = this.DataGrid.SelectionController;
            dc.ColumnElement = DetailsViewContentPresenter;
            return dc;
        }

        protected override Rect RowManupulation(Rect rect)
        {
            return rect;
        }

        public override void ArrangeElement(Rect rect)
        {
            var rowRect = GetDetailsViewRowPosition(rect);
            if (!rowRect.IsEmpty)
            {
                this.WholeRowElement.Clip = null;
                this.WholeRowElement.Arrange(rowRect);
            }
            else
                base.ArrangeElement(rect);
        }

        public override void MeasureElement(Size size)
        {
            var rowSize = GetDetailsViewRowSize(size);
            if (!rowSize.IsEmpty)
            {
                this.WholeRowElement.Clip = null;
                this.WholeRowElement.ItemsPanel.InvalidateMeasure();
                this.WholeRowElement.Measure(rowSize);
            }
            else
                base.MeasureElement(size);
        }

        #endregion

        internal void ApplyContentVisualState(string visualState)
        {
            this.DetailsViewContentPresenter.ApplyVisualState(visualState);
            this.DetailsViewIndentCell.ApplyVisualState(visualState);
        }

        #region private Methods

        private Size GetDetailsViewRowSize(Size size)
        {
            var visibleRow = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(RowIndex);
            if (visibleRow != null)
            {
                var height = visibleRow.ClippedCorner - visibleRow.ClippedOrigin;
                size.Height = height;
            }
            return size;
        }

        private Rect GetDetailsViewRowPosition(Rect rect)
        {
            var visibleRow = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(RowIndex);
            if (visibleRow != null)
            {
                var height = visibleRow.ClippedCorner - visibleRow.ClippedOrigin;
#if WinRT
                if (this.DataGrid.VisualContainer.ScrollOwner != null)
                    rect.Y = visibleRow.ClippedOrigin + this.DataGrid.VisualContainer.VerticalOffset;
                else
                    rect.Y = visibleRow.ClippedOrigin;
#else
                rect.Y = visibleRow.ClippedOrigin;
#endif
                rect.Height = height;
            }
            return rect;
        }

        private Rect GetDetailsViewContentPosition()
        {
            var visibleLines = this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
            var visibleRow = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(RowIndex);
            var startIndex = (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) + 1 + (this.DataGrid.ShowRowHeader ? 1 : 0);
            int repeatValue;
            if (this.DataGrid.VisualContainer.ColumnWidths.GetHidden(startIndex, out repeatValue))
                startIndex += repeatValue;
            var firstVisibleColumn = this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(startIndex) ?? visibleLines[visibleLines.FirstBodyVisibleIndex];
            var lastVisibleColumn = visibleLines[visibleLines.LastBodyVisibleIndex];
            if (visibleRow != null && firstVisibleColumn != null && lastVisibleColumn != null)
            {
                var rect = GridUtil.FromLTRB(firstVisibleColumn.ClippedOrigin, visibleRow.ClippedOrigin, lastVisibleColumn.ClippedCorner, visibleRow.ClippedCorner);
                rect.Y = 0;
                if (!visibleRow.IsClipped && visibleRow.isLastLine && visibleRow.ClippedCornerExtent < 0)
                    rect.Height += visibleRow.ClippedCornerExtent;
                return rect;
            }
            return Rect.Empty;
        }

        private Rect GetCellPosition(int index)
        {
            if (index == this.DataGrid.View.GroupDescriptions.Count + 1 + (this.DataGrid.ShowRowHeader ? 1 : 0))
                return GetDetailsViewContentPosition();
            var visibleRow = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(RowIndex);
            var visivleColumn = this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(index);
            if (visibleRow != null && visivleColumn != null)
            {
                var rect = GridUtil.FromLTRB(visivleColumn.ClippedOrigin, visibleRow.ClippedOrigin, visivleColumn.ClippedCorner, visibleRow.ClippedCorner);
                rect.Y = 0;
                if (!visibleRow.IsClipped && visibleRow.isLastLine && visibleRow.ClippedCornerExtent < 0)
                    rect.Height += visibleRow.ClippedCornerExtent;
                return rect;
            }
            return Rect.Empty;
        }

        private object GetDetailsViewInfo(string parameter)
        {
            switch (parameter)
            {
                case "Clip":
                    var rect = GetDetailsViewContentPosition();
                    return this.DataGrid.DetailsViewManager.SubtractDetailsViewPadding(rect);
                case "HorizontalOffset":
                    return GetHorizontalOffset();
                case "VerticalOffset":
                    return GetVerticalOffset();
                case "Padding":
                    return DataGrid.DetailsViewPadding;
            }
            return double.NaN;
        }

        private double GetHorizontalOffset()
        {
            var visibleLines = this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
            var firstVisibleColumn = (visibleLines.Count > visibleLines.firstBodyVisibleIndex) ? visibleLines[visibleLines.firstBodyVisibleIndex] : null;
            var scrollColumnsCount = this.DataGrid.VisualContainer.ScrollColumns.LineCount;
            var xSpan = this.DataGrid.VisualContainer.ScrollColumns.RangeToPoints(ScrollAxisRegion.Body, 1, scrollColumnsCount, true);
            if (firstVisibleColumn == null || xSpan.IsEmpty) return double.NaN;
            if (this.DataGrid.FrozenColumnCount > 0)
                return (firstVisibleColumn.ClippedOrigin - xSpan.Start) - this.DataGrid.VisualContainer.ScrollColumns.HeaderExtent;
            return firstVisibleColumn.ClippedOrigin - xSpan.Start;
        }

        private double GetVerticalOffset()
        {
            var visibleRow = this.DataGrid.VisualContainer.ScrollRows.GetVisibleLineAtLineIndex(RowIndex);
            if (visibleRow == null) return double.NaN;
            var verticalOffset = visibleRow.ClippedOrigin - visibleRow.Origin;
            return verticalOffset;
        }

        #endregion
    }
}
