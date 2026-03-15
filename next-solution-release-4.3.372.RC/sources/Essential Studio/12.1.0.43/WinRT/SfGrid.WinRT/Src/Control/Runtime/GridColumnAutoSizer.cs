#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Linq;
using System;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
#if WinRT
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Syncfusion.UI.Xaml.Grid.Utility;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    public class GridColumnSizer : IDisposable
    {
        #region Fields
        private SfDataGrid dataGrid;
        private bool gridRowsFitInToView;
        #endregion

        #region ctor
        public GridColumnSizer(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Initialize Delegate for GridColumn
        /// </summary>
        /// <param name="columns"></param>
        /// <remarks></remarks>
        internal void InitializeColumnWPropertyChangedDelegate(Columns columns)
        {
            foreach (var column in columns)
            {
                if (column.ColumnPropertyChanged == null)
                    column.ColumnPropertyChanged = OnColumnPropertyChanged;
            }
        }

        internal void InitialRefreshAll(double AvailableWidth)
        {
            RefreshAll(AvailableWidth);

            if (this.dataGrid.VisualContainer != null)
            {
                if (this.dataGrid.VisualContainer.ViewportHeight > this.dataGrid.VisualContainer.ExtentHeight)
                    gridRowsFitInToView = true;
                this.dataGrid.VisualContainer.RowHeights.LineHiddenChanged += RowHeights_LineHiddenChanged;
            }

            if (!(this.dataGrid.AllowResizingColumns && this.dataGrid.AllowResizingHiddenColumns))
                return;
            this.dataGrid.Columns.ForEach(col => 
            {
                if (col.IsHidden)
                    this.dataGrid.GridColumnResizingController.ProcessResizeStateManager(col);
            });
        }

        void RowHeights_LineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            if (!this.dataGrid.Columns.Any()) return;

            var lastColumnIndex = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.Count - 1);
            if (this.dataGrid.VisualContainer.ColumnCount <= lastColumnIndex)
                return;

            if (gridRowsFitInToView)
            {
                if (this.dataGrid.VisualContainer.ViewportHeight < this.dataGrid.VisualContainer.ExtentHeight)
                {
                    RefreshAll();
                    gridRowsFitInToView = false;
                }
            }
            else
            {
                if (this.dataGrid.VisualContainer.ViewportHeight > this.dataGrid.VisualContainer.ExtentHeight)
                {
                    RefreshAll();
                    gridRowsFitInToView = true;
                }
            }
        }

        
        private void RefreshAll(double AvailableWidth)
        {
            var sizerColumns = this.dataGrid.Columns.Where(x => x.ReadLocalValue(GridColumn.ColumnSizerProperty) == DependencyProperty.UnsetValue);
            if ((int)this.dataGrid.ColumnSizer != -1 || sizerColumns.Count() > 0)
            {
                
                InitializeColumnWPropertyChangedDelegate(this.dataGrid.Columns);
                InitializeUnboundColumnPropertiesDelegate(this.dataGrid.Columns);
                this.SetSizerWidth(AvailableWidth);
                if (this.dataGrid.VisualContainer.ScrollOwner != null)
                {
                    this.dataGrid.VisualContainer.NeedToRefreshColumn = true;
                    this.dataGrid.VisualContainer.InvalidateMeasureInfo();
                    this.dataGrid.VisualContainer.ScrollOwner.InvalidateMeasure();
                    this.dataGrid.VisualContainer.UpdateScrollBars();
                }
            }
        }

        /// <summary>
        /// Refresh ColumnWidth for each column
        /// </summary>
        /// <remarks></remarks>
        internal void RefreshAll()
        {
            (this.dataGrid.VisualContainer.ColumnWidths as LineSizeCollection).SuspendUpdates();
            RefreshAll(0);
            (this.dataGrid.VisualContainer.ColumnWidths as LineSizeCollection).ResumeUpdates();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Refresh all column width
        /// </summary>
        /// <remarks>invokes when ColumnSizer,Width changed for GridColumn</remarks>
        private void OnColumnPropertyChanged(GridColumn column, string property)
        {
            switch (property)
            {
                case "Width":
                case "MinimumWidth":
                case "ColumnSizer":
                case "MaximumWidth":
                case "AllowSorting":
                    this.SetSizerWidth(0);
                    this.dataGrid.VisualContainer.NeedToRefreshColumn = true;
                    this.dataGrid.VisualContainer.InvalidateMeasureInfo();
                    break;
                case "IsHidden":
                    if (!column.IsHidden)
                    {
                        var index = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.IndexOf(column));
                        this.dataGrid.VisualContainer.ColumnWidths.SetHidden(index, index, false);
                    }
                    this.SetSizerWidth(0);
                    if (dataGrid.AllowResizingColumns && dataGrid.AllowResizingHiddenColumns)
                        this.dataGrid.GridColumnResizingController.ProcessResizeStateManager(column);
                    this.dataGrid.VisualContainer.NeedToRefreshColumn = true;
                    this.dataGrid.VisualContainer.InvalidateMeasureInfo();
                    if (this.dataGrid.GridColumnDragDropController != null)
                        this.dataGrid.GridColumnDragDropController.ColumnHiddenChanged(column);
                    break;
#if !WP
                case "AllowFiltering":
                    if (this.dataGrid.View is Syncfusion.Data.PagedCollectionView)
                    {
                        if ((this.dataGrid.View as Syncfusion.Data.PagedCollectionView).UseOnDemandPaging)
                            return;
                    }
                    var header = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.dataGrid.GetHeaderIndex());
                    if (header != null)
                    {
                        var columnBase = header.VisibleColumns.FirstOrDefault(col => col.GridColumn != null && col.GridColumn.MappingName.Equals(column.MappingName));
                        if (columnBase != null)
                        {
                            (columnBase.ColumnElement as GridHeaderCellControl).FilterIconVisiblity = column.AllowFiltering ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    break;
#endif
                case "HeaderTemplate":
                case "CellStyle":
                case "CellStyleSelector":
                case "HeaderStyle":
                case"CellTemplate":
                case "CellTemplateSelector":
                    this.dataGrid.OnColumnStyleChanged(column);
                    break;
            }
        }

        /// <summary>
        /// Set Width according to ColumnSizer
        /// </summary>
        /// <remarks></remarks>
        private void SetSizerWidth(double viewPortWidth)
        {
            double totalColumnSize = 0d;
            var calculatedColumns = new List<GridColumn>();

            var hiddenColumns = this.dataGrid.Columns.Where(column => column.IsHidden);
            foreach (var column in hiddenColumns)
            {
                var index = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.IndexOf(column));
                this.dataGrid.VisualContainer.ColumnWidths.SetHidden(index, index, true);
                calculatedColumns.Add(column);
            }

            var widthColumns = this.dataGrid.Columns.Where(column => !double.IsNaN(column.Width) && !column.IsHidden);
            foreach (var column in widthColumns)
            {
                //Set Column width
                double width = column.Width;
                //add
                totalColumnSize += SetColumnWidth(column,width);
                //add to collection
                calculatedColumns.Add(column);
            }

            var sizeToCellColumns = this.dataGrid.Columns.Where(column => column.ColumnSizer == GridLengthUnitType.SizeToCells);
            foreach (var column in sizeToCellColumns)
            {
                //Caluction
                double width = SetSizeToCellsWidth(column);
                if (double.IsNaN(column.Width) && !column.IsHidden)
                {
                    //set column Width
                    totalColumnSize += SetColumnWidth(column, width);
                    //add to collection
                    calculatedColumns.Add(column);
                }
            }

            var sizeToHeaderColumns = this.dataGrid.Columns.Where(column => column.ColumnSizer == GridLengthUnitType.SizeToHeader);
            foreach (GridColumn column in sizeToHeaderColumns)
            {
                //Caluction
                double width = SetSizeToHeaderWidth(column);
                //set column Width
                if (double.IsNaN(column.Width) && !column.IsHidden)
                {
                    //set column Width
                    totalColumnSize += SetColumnWidth(column, width);
                    //add to collection
                    calculatedColumns.Add(column);
                }
            }

            var lastColumn = this.dataGrid.Columns.LastOrDefault(x => !x.IsHidden);
            var autoColumns = this.dataGrid.Columns.Where(column => column.ColumnSizer == GridLengthUnitType.Auto);
            var autoWithLastColumnFills = this.dataGrid.Columns.Where(col => CheckAutoWithLastColumnFill(col,lastColumn)).ToList();
            autoColumns = autoColumns.Union(autoWithLastColumnFills);
            foreach (var column in autoColumns)
            {
                //Calculation
                double headerWidth;
                double cellWidth;
                double width;
                headerWidth = SetSizeToHeaderWidth(column);
                cellWidth = SetSizeToCellsWidth(column);
                if (cellWidth > headerWidth)
                    width = cellWidth;
                else
                {
                    width = headerWidth;
                    column.ActualWidth = width;
                }
                if (double.IsNaN(column.Width) && !column.IsHidden)
                {
                    //set column Width
                    totalColumnSize += SetColumnWidth(column, width);
                    //add to collection
                    calculatedColumns.Add(column);
                }
            }
            if (this.dataGrid.View !=null && this.dataGrid.View.GroupDescriptions.Count > 0)
            {
#if !WP
                totalColumnSize += (this.dataGrid.GridModel.IndentColumnSize) * this.dataGrid.View.GroupDescriptions.Count;
#else
                for (int i = 0; i < this.dataGrid.View.GroupDescriptions.Count; i++)
                {
                    int isHidden = -1;
                    this.dataGrid.VisualContainer.ColumnWidths.GetHidden(i, out isHidden);
                    if(isHidden > this.dataGrid.View.GroupDescriptions.Count)
                        totalColumnSize += (this.dataGrid.GridModel.IndentColumnSize);
                }
#endif
            }

            if (this.dataGrid.ShowRowHeader)
                totalColumnSize += this.dataGrid.RowHeaderWidth;
#if!WP
            if (this.dataGrid.DetailsViewManager.HasDetailsView)
            {
                totalColumnSize += this.dataGrid.GridModel.IndentColumnSize;
            }
#endif
            SetGridSizerWidth(totalColumnSize, calculatedColumns, lastColumn, viewPortWidth);
        }


        private void SetGridSizerWidth(double totalColumnSize, List<GridColumn> calculatedColumns, GridColumn lastColumn, double viewPortWidth)
        {
            foreach (var column in this.dataGrid.Columns)
            {
                if (!calculatedColumns.Contains(column))
                {
                    double cellWidth;
                    double headerWidth;
                    double width = 0d;
                    if (column.ColumnSizer != GridLengthUnitType.Star)
                    {
                        switch (this.dataGrid.ColumnSizer)
                        {
                            case GridLengthUnitType.SizeToCells:
                                width = SetSizeToCellsWidth(column);
                                totalColumnSize += SetColumnWidth(column, width);
                                calculatedColumns.Add(column);
                                break;
                            case GridLengthUnitType.SizeToHeader:
                                width = SetSizeToHeaderWidth(column);
                                totalColumnSize += SetColumnWidth(column, width);
                                calculatedColumns.Add(column);
                                break;
                            case GridLengthUnitType.Auto:
                                headerWidth = SetSizeToHeaderWidth(column);
                                cellWidth = SetSizeToCellsWidth(column);
                                if (cellWidth > headerWidth)
                                    width = cellWidth;
                                else
                                {
                                    width = headerWidth;
                                    column.ActualWidth = width;
                                }
                                totalColumnSize += SetColumnWidth(column, width);
                                calculatedColumns.Add(column);
                                break;
                            case GridLengthUnitType.AutoWithLastColumnFill:
                                if (column.MappingName != lastColumn.MappingName)
                                {
                                    headerWidth = SetSizeToHeaderWidth(column);
                                    cellWidth = SetSizeToCellsWidth(column);
                                    if (cellWidth > headerWidth)
                                        width = cellWidth;
                                    else
                                    {
                                        width = headerWidth;
                                        column.ActualWidth = width;
                                    }
                                    totalColumnSize += SetColumnWidth(column, width);
                                    calculatedColumns.Add(column);
                                }
                                break;
                            case GridLengthUnitType.None:
                                if (!column.IsHidden)
                                {
                                    column.ActualWidth = SetColumnWidth(column, this.dataGrid.VisualContainer.ColumnWidths.DefaultLineSize);
                                    totalColumnSize += column.ActualWidth;
                                    calculatedColumns.Add(column);
                                }
                                break;
                        }
                    }
                }
            }
            if (lastColumn != null)
            {
                if (lastColumn.ColumnSizer == GridLengthUnitType.AutoWithLastColumnFill)
                {
                    if (!lastColumn.IsHidden)
                    {
                        totalColumnSize -= lastColumn.ActualWidth;
                        calculatedColumns.Remove(lastColumn);
                        if (totalColumnSize < 0)
                            totalColumnSize = 0;
                    }
                }
            }
            var remainingColumns = this.dataGrid.Columns.Except(calculatedColumns);
            if (viewPortWidth == 0)
            {
                if (this.dataGrid.VisualContainer.ScrollOwner != null && this.dataGrid.VisualContainer.ScrollOwner.ActualWidth != 0)
                    viewPortWidth = this.dataGrid.VisualContainer.ScrollOwner.ActualWidth;
#if WPF
                var _vScrollBarWidth = SystemParameters.ScrollWidth;
                if (this.dataGrid.VisualContainer.ViewportHeight < this.dataGrid.VisualContainer.ExtentHeight)
                    viewPortWidth -= _vScrollBarWidth != 0 ? _vScrollBarWidth : SystemParameters.VerticalScrollBarWidth;
#elif SILVERLIGHT
                var vScrollBarWidth = double.NaN;
                if (this.dataGrid.VisualContainer.ScrollOwner != null)
                {
                    var verticalScrollBar = GridUtil.GetChildObject<ScrollBar>(this.dataGrid.VisualContainer.ScrollOwner, "VerticalScrollBar");
                    if (this.dataGrid.VisualContainer.ViewportHeight < this.dataGrid.VisualContainer.ExtentHeight)
                        vScrollBarWidth = verticalScrollBar.ActualWidth;
                    else
                        vScrollBarWidth = this.dataGrid.BorderThickness.Left + this.dataGrid.BorderThickness.Right;
                }
                viewPortWidth -= double.IsNaN(vScrollBarWidth) ? 17 : vScrollBarWidth;
                
#endif

            }
            double remainingColumnWidths = viewPortWidth- totalColumnSize;

            if (totalColumnSize != 0 || (this.dataGrid.Columns.Any(col => col.ColumnSizer == GridLengthUnitType.Star) || this.dataGrid.ColumnSizer == GridLengthUnitType.Star))
                SetStarWidth(remainingColumnWidths, remainingColumns);
            else
                SetNoneWidth(remainingColumns);
        }

        /// <summary>
        /// Set Width for column when ColumnSizer is set to None
        /// </summary>
        /// <param name="remainingColumns"></param>
        /// <remarks></remarks>
        private void SetNoneWidth(IEnumerable<GridColumn> remainingColumns)
        {
            double TotalColumnSize = 0;
            var remCols = new List<GridColumn>();
            foreach (var col in remainingColumns)
            {
                var index = this.dataGrid.Columns.IndexOf(col);
                if (col.MappingName == this.dataGrid.Columns[this.dataGrid.Columns.Count - 1].MappingName && col.ColumnSizer == GridLengthUnitType.AutoWithLastColumnFill)
                {
                    double remainingColumnWidths = this.dataGrid.VisualContainer.ScrollOwner.ActualWidth - TotalColumnSize;
                    remCols.Add(col);
                    SetStarWidth(remainingColumnWidths, remCols);
                }
                else
                {
                    TotalColumnSize += SetColumnWidth(col, this.dataGrid.VisualContainer.ColumnWidths.DefaultLineSize);
                }
            }
        }

        /// <summary>
        /// Set Column Width by considering Minimum & Maximum Width
        /// </summary>
        /// <param name="column"></param>
        /// <param name="Width"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double SetColumnWidth(GridColumn column,double Width)
        {
            var colIndex = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.IndexOf(column));
            if (!double.IsNaN(column.ExtendedWidth))
            {
                return Width;
            }
            if (!double.IsNaN(column.MinimumWidth) || !double.IsNaN(column.MaximumWidth))
            {
                if (!double.IsNaN(column.MaximumWidth))
                {
                    if (!double.IsNaN(Width) && column.MaximumWidth > Width)
                    {
                        if (Width != this.dataGrid.VisualContainer.ColumnWidths[colIndex])
                            this.dataGrid.VisualContainer.ColumnWidths[colIndex] = Width;
                    }
                    else
                    {
                        if (column.MaximumWidth != this.dataGrid.VisualContainer.ColumnWidths[colIndex])
                            this.dataGrid.VisualContainer.ColumnWidths[colIndex] = column.MaximumWidth;
                    }
                }
                if (!double.IsNaN(column.MinimumWidth))
                {
                    if (!double.IsNaN(Width) && column.MinimumWidth < Width)
                    {
                        if (Width != this.dataGrid.VisualContainer.ColumnWidths[colIndex])
                            this.dataGrid.VisualContainer.ColumnWidths[colIndex] = Width;
                    }
                    else
                    {
                        if (column.MinimumWidth != this.dataGrid.VisualContainer.ColumnWidths[colIndex])
                            this.dataGrid.VisualContainer.ColumnWidths[colIndex] = column.MinimumWidth;
                    }
                }
            }
            else
            {
                if (!double.IsNaN(Width) && Width != this.dataGrid.VisualContainer.ColumnWidths[colIndex])
                    this.dataGrid.VisualContainer.ColumnWidths[colIndex] = Width;
            }
            column.ActualWidth = this.dataGrid.VisualContainer.ColumnWidths[colIndex];
            return this.dataGrid.VisualContainer.ColumnWidths[colIndex];
        }

        /// <summary>
        /// Calculate Width for column when ColumnSizer is SizeToHeader
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double SetSizeToHeaderWidth(GridColumn column)
        {
            column.ActualWidth = this.GetWidthBasedOnIndexHeader(column, 0);
#if !WP
            if (this.dataGrid.CanSetAllowFilters(column))
                column.ActualWidth += 28;
#endif
            double width = 0d;
            bool tempWidthSortFlag = false;
            if (dataGrid.AllowSorting && column.AllowSorting)
            {
                width += 25;
                tempWidthSortFlag = true;
            }
           
            if (this.dataGrid.RowGenerator.Items.Count > 0)
            {
                DataRowBase dataRow = this.dataGrid.RowGenerator.Items[this.dataGrid.GetHeaderIndex()];
                if (dataRow != null)
                {
                    if (dataRow.VisibleColumns.Any(col => col.GridColumn != null && col.GridColumn.MappingName == column.MappingName))
                    {
                        DataColumnBase dataColumn = dataRow.VisibleColumns.FirstOrDefault(col => col.GridColumn != null && col.GridColumn.MappingName == column.MappingName);
                        if (dataColumn != null)
                        {
                            if (dataColumn.ColumnElement is GridHeaderCellControl)
                            {
                                if (!tempWidthSortFlag && (dataColumn.ColumnElement as GridHeaderCellControl).SortDirection != null)
                                    width += 25;
                            }
                        }
                    }
                }
            }

            column.ActualWidth += width;
            return column.ActualWidth;
        }

        internal double SetAutoFitWidth(GridColumn column)
        {
            double headerWidth = SetSizeToHeaderWidth(column);
            double cellWidth = SetSizeToCellsWidth(column);
            double width;
            if (cellWidth > headerWidth)
            {
                width = cellWidth;
                column.ActualWidth = width;
                column.Width = SetColumnWidth(column, width);
            }
            else
            {
                width = headerWidth;
                column.ActualWidth = width;
                column.Width = SetColumnWidth(column, width);
            }
            return column.Width;
        }

        /// <summary>
        /// Calculate Width for Column When Column Sizer is SizeToCells
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double SetSizeToCellsWidth(GridColumn column)
        {
            if (this.dataGrid.View != null)
               column.ActualWidth = this.GetWidthBasedOnIndexCell(column, 0);
            return column.ActualWidth;
        }

        /// <summary>
        /// Calculate width for header based on given column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="resultWidth"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double GetWidthBasedOnIndexHeader(GridColumn column, double resultWidth)
        {
            var colIndex = this.dataGrid.Columns.IndexOf(column);
            int rowCount = this.dataGrid.HeaderLineCount;
            double colWidth = this.dataGrid.VisualContainer.ColumnWidths[colIndex];
            string text;
            Size textSize;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                double rowHeight = this.dataGrid.VisualContainer.RowHeights[rowIndex];
                var clientSize = new Size(colWidth, rowHeight);
                text = column.HeaderText ?? column.MappingName;
                textSize = MeasureHeaderText(clientSize, text, GridQueryBounds.Width, column);
                var width = textSize.Width;
                if (resultWidth < width)
                {
                    resultWidth = width;
                }
            }
            return resultWidth;
        }

        /// <summary>
        /// Calculate width for Cells based on given column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="resultWidth"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double GetWidthBasedOnIndexCell(GridColumn column, double resultWidth)
        {
            var colIndex = this.dataGrid.Columns.IndexOf(column);
            var recordCount = this.dataGrid.View.Records.Count;
            if (recordCount == 0)
                return double.NaN;
            int scrollColumnIndex = this.dataGrid.ResolveToScrollColumnIndex(colIndex);
            double colWidth = this.dataGrid.VisualContainer.ColumnWidths[scrollColumnIndex];
            double rowHeight = this.dataGrid.VisualContainer.RowHeights.DefaultLineSize;
            int textLength = 0;
            for (int recordIndex = 0; recordIndex < recordCount; recordIndex++)
            {
                var clientSize = new Size(colWidth, rowHeight);
                var data = this.dataGrid.View.Records[recordIndex].Data;
                Size textSize = Size.Empty;
                if (data != null)
                {
                    string text = string.Empty;
                    if (!column.IsUnbound)
                    {
                        var unBoundText = this.dataGrid.View.GetPropertyAccessProvider().GetFormattedValue(data, column.MappingName);
                        if (unBoundText != null)
                            text = unBoundText.ToString();
                        else
                            text = string.Empty;
                    }
                    else
                        text = this.dataGrid.GetUnBoundCellValue(column, data).ToString();
#if WPF
                    if (column is GridTemplateColumn)
                    {
                        ContentControl ctrl = new ContentControl();
                        ctrl.Content = data;
                        ctrl.ContentTemplate = (column as GridTemplateColumn).CellTemplate;
                        ctrl.Measure(new Size(double.MaxValue, double.MaxValue));
                        textSize = ctrl.DesiredSize;
                        var width = textSize.Width;
                        if (resultWidth < width)
                        {
                            resultWidth = width;
                        }
                    }
                    else
                        if (text.Length >= textLength)
                        {
                            textSize = MeasureText(clientSize, text, GridQueryBounds.Width, true, column);
                            var width = textSize.Width;
                            if (resultWidth < width)
                            {
                                resultWidth = width;
                            }
                            textLength = text.Length;
                        }
#else
                    // since size not calculated correctly when template applied, so previous implementation not changed for winrt
                    if (text.Length > textLength)
                    {
                        textSize = MeasureText(clientSize, text, GridQueryBounds.Width, true, column);
                        var width = textSize.Width;
                        if (resultWidth < width)
                        {
                            resultWidth = width;
                        }
                        textLength = text.Length;
                    }
#endif
                }
            }
            return resultWidth;
        }

        /// <summary>
        /// Measure Size For HeaderCell
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="displayText"></param>
        /// <param name="queryBound"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks>calculation based on text diplayed on header</remarks>
        private Size MeasureHeaderText(Size rectangle, string displayText, GridQueryBounds queryBound, GridColumn column)
        {
            var headerSize = MeasureText(rectangle, displayText, queryBound,false,column);
            return headerSize;
        }

        /// <summary>
        /// Measure Size for Cell
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="displayText"></param>
        /// <param name="queryBound"></param>
        /// <param name="cell">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <returns></returns>
        /// <remarks>calculation based on text diplayed on Cell</remarks>
        private Size MeasureText(Size rectangle, string displayText, GridQueryBounds queryBound,bool cell,GridColumn column)
        {
            var textBlock = new TextBlock
                {
                    Text = displayText,
                    FontFamily = new FontFamily("Segoe UI"),
#if WinRT
                    Margin = new Thickness(8, 0, 8, 0),
                    FontSize = 16
#elif WP
                    Margin = new Thickness(10, 3, 10, 3),
                    FontSize = 24
#else
                    Margin = new Thickness(5, 1, 5, 1),
                    FontSize = 12
#endif
                };
            if (!cell)
            {
                textBlock.HorizontalAlignment = column.HorizontalHeaderContentAlignment;
#if WinRT
                textBlock.FontWeight = FontWeights.SemiBold;
                textBlock.Margin = new Thickness(10, 3, 10, 3);
#elif WP
                   textBlock.Margin = new Thickness(10, 3, 10, 3);
                   textBlock.FontSize = 24;
#else
                textBlock.FontWeight = FontWeights.Normal;
                textBlock.Margin = new Thickness(10, 3, 2, 3);
#endif
            }
            var parentBorder = new Border { Child = textBlock };
            if (queryBound == GridQueryBounds.Height)
            {
                // heights scenario done in here
                return new Size(textBlock.Width, textBlock.Height);
            }
            else
            {
                textBlock.MaxHeight = rectangle.Height;
                //Calculating MaxWidth for ParentBorder was created an issue when the Text size is Greater than VisualContainer.TotalExtent value. Hence we removing this code.
                //var totalWidth = this.dataGrid.VisualContainer.ColumnWidths.TotalExtent;
                //parentBorder.MaxWidth = totalWidth;
                parentBorder.Measure(new Size(double.MaxValue, double.MaxValue));
                return parentBorder.DesiredSize;
            }
        }

        /// <summary>
        /// Check for Existance of ColumnSizer Star for given Column
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CheckForStarColumn(GridColumn col)
        {
            if ((col.ColumnSizer == GridLengthUnitType.Star || dataGrid.ColumnSizer == GridLengthUnitType.Star) && (col.ColumnSizer == GridLengthUnitType.Star || col.ReadLocalValue(GridColumn.ColumnSizerProperty) == DependencyProperty.UnsetValue))
                return true;
            return false;
        }

        /// <summary>
        /// Checks for AutoWithLastColumnFill column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CheckAutoWithLastColumnFill(GridColumn column,GridColumn lastColumn)
        {
            if (lastColumn != null && lastColumn.MappingName != column.MappingName && column.ColumnSizer == GridLengthUnitType.AutoWithLastColumnFill && !column.IsHidden)
                return true;
            return false;
        }

        /// <summary>
        /// Set Star Width for Columns
        /// </summary>
        /// <param name="remainingColWidth"></param>
        /// <param name="remCols"></param>
        /// <remarks></remarks>
        private void SetStarWidth(double remainingColWidth, IEnumerable<GridColumn> remCols)
        {
            var removedColumn = new List<GridColumn>();
            var columns = remCols.ToList();
            var totalRemainingStarValue = remainingColWidth;
            double removedWidth = 0;
            bool isremoved;
            while (columns.Count > 0)
            {
                isremoved = false;
                removedWidth = 0;
                double starWidth = Math.Floor((totalRemainingStarValue / columns.Count));
                var column = columns.First();
                double computedWidth = SetColumnWidth(column, starWidth);
                if (starWidth != computedWidth && starWidth > 0)
                {
                    isremoved = true;
                    columns.Remove(column);
                    foreach (var remColumn in removedColumn)
                    {
                        if (!columns.Contains(remColumn))
                        {
                            removedWidth += remColumn.ActualWidth;
                            columns.Add(remColumn);
                        }
                    }
                    totalRemainingStarValue += removedWidth;
                }
                column.ActualWidth = computedWidth;
                totalRemainingStarValue = totalRemainingStarValue - computedWidth;
                if (!isremoved)
                {
                    columns.Remove(column);
                    if(!removedColumn.Contains(column))
                        removedColumn.Add(column);
                }
            }
        }

        #endregion

        #region Disposable Method
        public void Dispose()
        {
            if (this.dataGrid.VisualContainer != null)
                this.dataGrid.VisualContainer.RowHeights.LineHiddenChanged -= RowHeights_LineHiddenChanged;
           
            this.dataGrid = null;
        }
        #endregion

        #region UnBoundColumn Refreshing
        /// <summary>
        /// Refresh UnBoundColumn value
        /// </summary>
        /// <param name="column"></param>
        /// <remarks>invokes when Unbound Expression,Format changedS</remarks>
        private void OnUnBoundPropertiesChanged(GridUnBoundColumn column)
        {
            if (!column.IsHidden)
            {
                this.dataGrid.RowGenerator.Items.ForEach(row => row.VisibleColumns.ForEach(col =>
                {
                    if (col.GridColumn.IsUnbound && col.GridColumn.MappingName == column.MappingName)
                    {
                        col.UpdateBinding(row.RowData);
                    }
                }));
            }
        }

        /// <summary>
        /// Initialize UnBoundColumnProperties delegate
        /// </summary>
        /// <param name="columns"></param>
        /// <remarks></remarks>
        internal void InitializeUnboundColumnPropertiesDelegate(Columns columns)
        {
            foreach (var column in columns)
            {
                if (column is GridUnBoundColumn)
                {
                    GridUnBoundColumn col = (column as GridUnBoundColumn);
                    if (col.UnboundPropertiesChanged == null)
                        col.UnboundPropertiesChanged = OnUnBoundPropertiesChanged;
                }
            }
        }
        #endregion
    }
}
