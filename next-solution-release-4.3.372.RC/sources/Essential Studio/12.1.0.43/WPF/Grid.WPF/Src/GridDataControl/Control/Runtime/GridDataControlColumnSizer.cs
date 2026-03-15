#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows;
    using System.Windows.Controls;
    using Syncfusion.Linq;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Windows.Controls.Primitives;

    public class GridDataControlColumnSizer : GridColumnAutoSizer
    {
        private bool isNestedModel = false;
        internal bool IsAutoOnLoad { get; set; }
        private bool isSizeChanged = false;
        private bool lineCountAdded = false;
        private bool lineCountRemoved = false;
        public GridDataControlColumnSizer(GridDataTableModel model)
            : base((GridModel)model)
        {
            this.Model = model;
            if (this.Model is GridDataChildTableModel)
            {
                this.isNestedModel = true;
            }
        }

        internal bool IsGridDataControlLoaded
        {
            get;
            set;
        }

        internal void WireEvents()
        {
            if (!(this.Model is GridDataChildTableModel))
            {
                var element = this.Model.Views.First() as GridControlBase;

                if (element.Parent != null)
                    (element.Parent as FrameworkElement).SizeChanged += new SizeChangedEventHandler(GridDataControlColumnSizer_SizeChanged);
#if !SILVERLIGHT
                if (this.Parent != null)
                {
                    var sv = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ComputedVerticalScrollBarVisibilityProperty, typeof(ScrollViewer));
                    sv.AddValueChanged(Parent, VisibilityChanged);
                }
#endif
            }
            else
            {
                var model = this.Model as GridDataChildTableModel;
                if (model.ParentTable.Model.Views.First().Parent != null)
                    (model.ParentTable.Model.Views.First().Parent as FrameworkElement).SizeChanged += new SizeChangedEventHandler(GridDataControlColumnSizer_SizeChanged);
            }
        }

        private void VisibilityChanged(object sender, EventArgs args)
        {
            if (!this.IsGridDataControlLoaded)
            {
                return;
            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None
                && this.Model.TableProperties.AutoPopulateColumns)
            {
                return;
            }

            this.SetStarWidth();
        }

        private ScrollViewer Parent
        {
            get
            {
                if (this.Model.Grid != null)
                {
                    return this.Model.Grid.Parent as ScrollViewer;
                }

                return null;
            }
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            if (dispose)
            {
                if (this.Model.Views != null && this.Model.Views.Count() >0)
                {
                    if (!(this.Model is GridDataChildTableModel))
                    {
                        var element = this.Model.Views.First() as GridControlBase;
                        (element.Parent as FrameworkElement).SizeChanged -= new SizeChangedEventHandler(GridDataControlColumnSizer_SizeChanged);
#if !SILVERLIGHT
                        if (this.Parent != null)
                        {
                            var sv = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ComputedVerticalScrollBarVisibilityProperty, typeof(ScrollViewer));
                            sv.RemoveValueChanged(Parent, VisibilityChanged);
                        }
#endif
                    }
                    else
                    {
                        var model = this.Model as GridDataChildTableModel;
                        (model.ParentTable.Model.Views.First().Parent as FrameworkElement).SizeChanged -= new SizeChangedEventHandler(GridDataControlColumnSizer_SizeChanged);
                    }
                }
            }
        }

        public void RefreshAll()
        {
            if (this.Model.TableProperties == null || (this.Model.TableProperties != null && this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None && this.Model.TableProperties.AutoPopulateColumns))
            {
                return;
            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            var totalextent = this.Model.ColumnWidths.TotalExtent;
            this.SetActualWidth();
            this.SetStarWidth();
            if (totalextent != this.Model.ColumnWidths.TotalExtent)
            {
                if (this.Model.Grid != null && totalextent < this.Model.Grid.ActualWidth)
                {
                    this.Model.Grid.InvalidateMeasure();
                }
            }
        }

        public void InvalidateCells(GridRangeInfo range)
        {
            if (!this.IsGridDataControlLoaded)
            {
                return;
            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None
                && this.Model.TableProperties.AutoPopulateColumns)
            {
                return;
            }

            foreach (var column in this.Model.TableProperties.VisibleColumns)
            {
                switch (column.Width.UnitType)
                {
                    case GridControlLengthUnitType.Auto:
                    case GridControlLengthUnitType.SizeToHeader:
                    case GridControlLengthUnitType.SizeToCells:
                        var resultWidth = this.GetWidthBasedOnRange(range, column);
                        if (resultWidth > column.ActualWidth)
                        {
                            column.ActualWidth = resultWidth;
                        }
                        break;
                    case GridControlLengthUnitType.None:
                        //Not required as actual width automatically set
                        //column.ActualWidth = column.Width.Value;                        
                        break;
                }
            }

            this.SetStarWidth();
        }

        private void SetActualWidth()
        {

            if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None
                && this.Model.TableProperties.AutoPopulateColumns)
            {
                return;
            }


            if (!this.IsGridDataControlLoaded && !this.isNestedModel)
            {
                return;
            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            //if (this.Model.Options.ColumnSizer != GridControlLengthUnitType.None)
            //{
            //    return;
            //}
            // To refresh the ColumnWidth When grid without itemsource.
            this.Model.RefreshColumns(true, false);
            foreach (var column in this.Model.TableProperties.VisibleColumns)
            {
                switch (column.Width.UnitType)
                {
                    case GridControlLengthUnitType.Auto:
                    case GridControlLengthUnitType.SizeToHeader:
                    case GridControlLengthUnitType.SizeToCells:
                        this.SetWidth(column);
                        break;
                    case GridControlLengthUnitType.Star:
                        break;
                    case GridControlLengthUnitType.None:
                        //Not required as actual width automatically set
                        //column.ActualWidth = column.Width.Value;
                        break;
                }
            }
        }

        private void GridDataControlColumnSizer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (this.Model.Options.ColumnSizer != GridControlLengthUnitType.None)
            //{
            //    return;
            //}

            this.SetStarWidth();
            this.isSizeChanged = true;
        }

        internal void SetWidthAndStar(GridDataVisibleColumn val)
        {

            if (!this.IsGridDataControlLoaded && !this.isNestedModel)
            {
                return;
            }
            //if (this.Model.Options.ColumnSizer != GridControlLengthUnitType.None)
            //{
            //    return;
            //}

            if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None
                && this.Model.TableProperties.AutoPopulateColumns)
            {
                return;
            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            if (!val.Width.IsStar)
            {
                this.SetWidth(val);
            }

            this.SetStarWidth();
        }

        private void SetWidth(GridDataVisibleColumn column)
        {
            if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None
                && this.Model.TableProperties.AutoPopulateColumns)
            {
                return;

            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            if (column.Width.IsNone)
            {
                column.ActualWidth = column.Width.Value;
                return;
            }

            var maxLength = this.Model.RowCount;
            if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
            {
                maxLength = this.Model.Options.MaxLength;
#if SILVERLIGHT
                //The default MaxLength is 1000. In silverlight it calculates the size of the cell by looping through the 1000 rows and get its size. This makes the grid to hang, so to overcome this we have setting the range for only in view.
                var maxrow = this.Model.Grid.ScrollRows.GetVisibleLines();
                if (maxrow.Count <= maxLength)
                {
                    maxLength = maxrow.Count;
                }
#endif
            }

            var range = GridRangeInfo.Rows(0, maxLength);
            column.isActualWidthSet = false;
            column.ActualWidth = this.GetWidthBasedOnRange(range, column);
            if (!column.isActualWidthSet)
            {
                if (this.Model != null && !this.Model.IsInColumnRefresh && !this.Model.TableProperties.VisibleColumns.IsInSuspend && !column.IsHidden)
                {
                    var colIdx = this.Model.ResolveVisibleColumnIndexToPosition(this.Model.TableProperties.VisibleColumns.IndexOf(column));
                    if (colIdx > -1)
                    {
                        if (this.Model.ColumnCount > colIdx)
                        {
                            if (column.MinimumWidth != 0 && column.MaximumWidth != 0)
                            {
                                if (column.MinimumWidth < column.MaximumWidth)
                                    this.Model.ColumnWidths[colIdx] = column.ActualWidth;
                                else if (column.MinimumWidth > column.MaximumWidth)
                                    this.Model.ColumnWidths[colIdx] = column.MaximumWidth;
                            }
                            else
                                this.Model.ColumnWidths[colIdx] = column.ActualWidth;
                            this.Model.InvalidateCell(GridRangeInfo.Col(colIdx));
                        }
                        //this.isActualWidthSet = true;
                    }
                }
            }
        }

        internal double GetWidth(GridDataVisibleColumn column)
        {
            var maxLength = this.Model.RowCount;
            if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
            {
                maxLength = this.Model.Options.MaxLength;
            }

            var range = GridRangeInfo.Rows(0, maxLength);
            var result = this.GetWidthBasedOnRange(range, column);
            return result;
        }

        private double GetWidthBasedOnRange(GridRangeInfo range, GridDataVisibleColumn column)
        {
            var colIndex = this.Model.ResolveVisibleColumnIndexToPosition(this.Model.TableProperties.VisibleColumns.IndexOf(column));
            range = range.ExpandRange(0, 0, this.Model.RowCount, this.Model.ColumnCount);
            var resultWidth = this.GetWidthBasedOnRange(range, colIndex, column, 0);

            if (this.Model.TableProperties.TableSummaryRows.Count > 0 && this.Model.RowCount > range.Bottom)
            {
                var val = this.Model.RowCount - this.Model.TableProperties.TableSummaryRows.Count - 1;
                var tableSummariesRange = GridRangeInfo.Cells(val, range.Left, this.Model.RowCount, range.Right);
                resultWidth = this.GetWidthBasedOnRange(tableSummariesRange, colIndex, column, resultWidth);
            }

            return resultWidth;
        }

        private double GetWidthBasedOnRange(GridRangeInfo range, int colIndex, GridDataVisibleColumn column, double resultWidth)
        {
            for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
            {
                var styleInfo = (GridDataStyleInfo)this.Model[rowIndex, colIndex];
                if (styleInfo == null || this.Model.RowHeights[rowIndex] == 0)
                {
                    continue;
                }

                var cellModel = styleInfo.CellModel;
                var tableCellIdentity = styleInfo.CellIdentity;
                if (this.CellTypeFilter(tableCellIdentity, column.Width.UnitType))
                {
                    var size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                    var width = size.Width;
                    if (resultWidth < width)
                    {
                        resultWidth = width;
                    }
                }
            }

            if (resultWidth == 0.0 && column.Width.UnitType == GridControlLengthUnitType.SizeToCells)
            {
                var styleInfo = (GridDataStyleInfo)this.Model[range.Top, colIndex];
                var size = styleInfo.CellModel.CalculatePreferredCellSize(range.Top,colIndex, styleInfo, GridQueryBounds.Width);
                var width = size.Width;
                if (resultWidth < width)
                {
                    resultWidth = width;
                }
            }

            return resultWidth;
        }


        internal void SetStarWidth()
        {

            if ((!this.IsGridDataControlLoaded && !this.isNestedModel)|| this.Model.TableProperties==null)
            {
                return;
            }

            if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.None
                && this.Model.TableProperties.AutoPopulateColumns)
            {
                return;
            }

            //if (this.Model.Options.ColumnSizer != GridControlLengthUnitType.None)
            //{
            //    return;
            //}
            if (this.IsAutoOnLoad && this.isSizeChanged)
            {
                return;
            }

            var hasStarColumn = this.Model.TableProperties.VisibleColumns.FirstOrDefault(v => v.Width.IsStar) != null;
            if (!hasStarColumn)
            {
                return;
            }
            var child = this.Model as GridDataChildTableModel;
            double parentWidth = 0;
            if (child != null)
            {
                if (child.ParentTable.Model.TableProperties.AllowNestedGridPadding)
                {
                    parentWidth = child.ParentTable.Model.ColumnWidths.TotalExtent - 12 -
                                     GridDataTableModel.ExpandCollapseCellWidth;
                }
                else
                {
                    parentWidth = child.ParentTable.Model.ColumnWidths.TotalExtent - GridDataTableModel.ExpandCollapseCellWidth;
                }
            }

            else
            {
                // The below commented codes are changes for the IR16439.
                var parent = this.Model.Grid.Parent as FrameworkElement;
                parentWidth = parent.ActualWidth;

#if !SILVERLIGHT
                if (parent is ScrollViewer)
                {
                    ScrollBar VScrollBar = ((ScrollViewer)parent).Template.FindName("PART_VerticalScrollBar", ((ScrollViewer)parent)) as ScrollBar;
                    //((ScrollViewer)parent).UpdateLayout();
                    if (VScrollBar != null)
                    {
                        if ((this.Model.RowHeights.TotalExtent > ((ScrollViewer)parent).ActualHeight)
                              || ((ScrollViewer)parent).ComputedVerticalScrollBarVisibility == Visibility.Visible)
                        {
                            parentWidth -= VScrollBar.ActualWidth != 0
                                                   ? (VScrollBar.ActualWidth + VScrollBar.Margin.Left +
                                                     VScrollBar.Margin.Right)
                                                   : GridDataControlColumnSizer.ScrollBarWidth;
                        }

                        if (lineCountAdded)
                        {
                            parentWidth -= VScrollBar.ActualWidth;
                            lineCountAdded = false;
                        }

                        if (lineCountRemoved)
                        {
                            parentWidth += VScrollBar.ActualWidth;
                            lineCountRemoved = false;
                        }
                    }
                }
#else
                if (parent is ScrollableContentViewer)
                {
                    if (this.Model.RowHeights.TotalExtent > ((ScrollableContentViewer)parent).ActualHeight)
                    {
                        parentWidth -= GridColumnAutoSizer.ScrollBarWidth;
                    }
                }
#endif
            }

            if (parentWidth > 0)
            {
                this.SetStarWidth(parentWidth - GridColumnAutoSizer.Border);
            }
        }

        void GridDataControlColumnSizerNew_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var parentWidth = (sender as FrameworkElement).ActualWidth;
#if !SILVERLIGHT
            if ((sender as FrameworkElement) is ScrollViewer)
            {
                ScrollBar VScrollBar = ((ScrollViewer)sender).Template.FindName("PART_VerticalScrollBar", ((ScrollViewer)sender)) as ScrollBar;
                //((ScrollViewer)sender).UpdateLayout();
                if (VScrollBar != null)
                {
                    if (((ScrollViewer)sender).ComputedVerticalScrollBarVisibility == Visibility.Visible
                        || (this.Model.RowHeights.TotalExtent > ((ScrollViewer)sender).ActualHeight))
                    {
                        parentWidth -= (VScrollBar.ActualWidth + VScrollBar.Margin.Left + VScrollBar.Margin.Right);
                    }
                }
            }
#else
            if ((sender as FrameworkElement) is ScrollableContentViewer)
            {
                if (((ScrollableContentViewer)sender).Visibility == Visibility.Visible)
                {
                    parentWidth -= GridColumnAutoSizer.ScrollBarWidth;
                }
            }
#endif

            this.SetStarWidth(e.NewSize.Width - GridColumnAutoSizer.Border);
        }

        private double lastColumnAutoWidth = 0;
        private string lastColumnName = string.Empty;
        private bool IsLastColumnAutoWidth
        {
            get
            {
                if (this.Model.TableProperties.ColumnSizer == GridControlLengthUnitType.AutoWithLastColumnFill)
                {
                    if (lastColumnAutoWidth == 0)
                    {
                        var range = GridRangeInfo.Rows(0, this.Model.RowCount);
                        var column = this.Model.TableProperties.VisibleColumns[this.Model.TableProperties.VisibleColumns.Count - 1];
                        lastColumnName = column.MappingName;
                        column.Width.UnitType = GridControlLengthUnitType.SizeToHeader;
                        lastColumnAutoWidth = this.GetWidthBasedOnRange(range, column);
                        column.Width.UnitType = GridControlLengthUnitType.Star;
                    }
                    return true;
                }
                else
                    return false;
            }
        }

        private void SetStarWidth(double parentWidth)
        {
            double remainingColWidth = this.Model.TableProperties.VisibleColumns.Where(v => !v.Width.IsStar).Sum(v => v.ActualWidth);
            double starColumnWidthValue = this.Model.TableProperties.VisibleColumns.Where(v => v.Width.IsStar).Sum(v => v.Width.Value);
            parentWidth -= (GridDataTableModel.ExpandCollapseCellWidth * this.Model.TableProperties.GroupedColumns.Count);
            if (this.Model.Table.HasNestedTables||this.Model.Table.HasDetailsView)
            {
                parentWidth -= GridDataTableModel.ExpandCollapseCellWidth;
            }

            if (this.Model.TableProperties.ShowRowHeader)
            {
                parentWidth -= this.Model.TableProperties.RowHeaderWidth; //GridDataTableModel.ExpandCollapseCellWidth;
            }

            var cols = this.Model.TableProperties.VisibleColumns.Where(d => d.Width.IsStar);
            var columns = cols.ToObservableCollection();
            //calculation remaining columns.
            var remainingCols = this.Model.TableProperties.VisibleColumns.Where(d => !d.Width.IsStar);
            var hiddenColumnsWidth = 0d;
            this.Model.TableProperties.VisibleColumns.ForEach(d =>
                 {
                     if (d.IsHidden)
                     {
                         if (d.Width.IsStar)
                             starColumnWidthValue -= d.Width.Value;
                         else
                             hiddenColumnsWidth += d.ActualWidth;
                     }
                 });
            // synchronize the remaining visible columns with model.ColumnWidth
            foreach (var col in remainingCols)
            {
                if (!col.isActualWidthSet)
                {
                    if (this.Model != null && !this.Model.IsInColumnRefresh && !this.Model.TableProperties.VisibleColumns.IsInSuspend && !col.IsHidden)
                    {
                        var colIdx = this.Model.ResolveVisibleColumnIndexToPosition(this.Model.TableProperties.VisibleColumns.IndexOf(col));
                        if (colIdx > -1)
                        {
                            if (this.Model.ColumnCount > colIdx)
                            {
                                this.Model.ColumnWidths[colIdx] = col.ActualWidth;
                                this.Model.InvalidateCell(GridRangeInfo.Col(colIdx));
                            }
                        }
                    }
                }
            }
            var totalRemainingStarValue = parentWidth + hiddenColumnsWidth - remainingColWidth;
            // calculating and assigning star columns
            while(columns.Count > 0)
            {
                var val = (totalRemainingStarValue / starColumnWidthValue);
                var removeColumns = new ObservableCollection<GridDataVisibleColumn>();
                foreach (var column in columns)
                {
                    if (column.IsHidden)
                        continue;

                    column.isActualWidthSet = false;
                    bool isColumnRemove = false;
                    if (column.MinimumWidth != 0 && column.MaximumWidth != 0)
                    {
                        if (val < column.MinimumWidth)
                        {
                            foreach (var col in cols)
                            {
                                if (column.MappingName == col.MappingName)
                                {
                                    col.ActualWidth = col.MinimumWidth;
                                    isColumnRemove = true;
                                }
                            }
                            
                        }
                        if (val > column.MaximumWidth)
                        {
                            foreach (var col in cols)
                            {
                                if (column.MappingName == col.MappingName)
                                {
                                    col.ActualWidth = col.MaximumWidth;
                                    isColumnRemove = true;
                                }
                            }
                        }
                    }
                    else if (!(column.MinimumWidth == 0 && column.MaximumWidth == 0))
                    {
                        if (column.MinimumWidth == 0 && val > column.MaximumWidth)
                        {
                            foreach (var col in cols)
                            {
                                if (column.MappingName == col.MappingName)
                                {
                                    col.ActualWidth = col.MaximumWidth;
                                    isColumnRemove = true;
                                }
                            }
                        }
                        if (column.MaximumWidth == 0 && val < column.MinimumWidth)
                        {
                            foreach (var col in cols)
                            {
                                if (column.MappingName == col.MappingName)
                                {
                                    col.ActualWidth = col.MinimumWidth;
                                    isColumnRemove = true;
                                }
                            }
                        }
                    }
                    if (isColumnRemove)
                    {
                        foreach (var col in cols)
                        {
                            if (column.MappingName == col.MappingName)
                            {
                                totalRemainingStarValue -= col.ActualWidth;
                            }
                        }
                        starColumnWidthValue--;
                        removeColumns.Add(column);
                    }
                }
                if (removeColumns.Count <= 0)
                {
                    val = (totalRemainingStarValue / starColumnWidthValue);
                    foreach(var column in columns)
                    {
                        foreach (var col in cols)
                        {
                            if (column.MappingName == col.MappingName)
                            {
                                if (IsLastColumnAutoWidth && col.MappingName.Equals(lastColumnName) && val < lastColumnAutoWidth)
                                    col.ActualWidth = lastColumnAutoWidth;
                                else if (val > 0)
                                    col.ActualWidth = val*col.Width.Value;
                                else
                                    col.ActualWidth = 150; // set as default width.
                            }
                        }
                       
                    }
                    break;
                }
                else
                {
                    foreach (var rcolumn in removeColumns)
                    {
                        columns.Remove(rcolumn);
                    }
                }
            }
            foreach (var col in cols)
            {
                if (this.Model != null && !this.Model.IsInColumnRefresh && !this.Model.TableProperties.VisibleColumns.IsInSuspend && !col.IsHidden)
                {
                    var colIdx = this.Model.ResolveVisibleColumnIndexToPosition(this.Model.TableProperties.VisibleColumns.IndexOf(col));
                    if (colIdx > -1)
                    {
                        if (this.Model.ColumnCount > colIdx)
                        {
                            this.Model.ColumnWidths[colIdx] = col.ActualWidth;
                            this.Model.InvalidateCell(GridRangeInfo.Col(colIdx));
                        }
                    }
                }
            }
        }

        public new GridDataTableModel Model
        {
            get;
            private set;
        }

        //protected override void ApplyStarSizer(int group)
        //{
        //    base.ApplyStarSizer(this.Model.TableProperties.GroupedColumns.Count);
        //}

        //protected override void ApplySizes(GridRangeInfo range)
        //{
        //    double[] newWidths = LineSizeUtil.GetRange(this.Model.ColumnWidths, range.Left, range.Right);
        //    double[] oldWidths = (double[])newWidths.Clone();
        //    for (int n = 0; n < newWidths.Length; n++)
        //    {
        //        newWidths[n] = -1;
        //    }

        //    var maxLength = this.Model.RowCount;
        //    if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
        //    {
        //        maxLength = this.Model.Options.MaxLength;
        //    }

        //    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
        //    {
        //        for (int rowIndex = range.Top; rowIndex <= maxLength; rowIndex++)
        //        {
        //            var styleInfo = (GridDataStyleInfo)this.Model[rowIndex, colIndex];
        //            var cellModel = styleInfo.CellModel;
        //            var tableCellIdentity = styleInfo.CellIdentity;
        //            if (this.CellTypeFilter(tableCellIdentity, this.Model.Options.ColumnSizer))
        //            {
        //                var size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
        //                var width = size.Width;
        //                if (newWidths[colIndex - range.Left] < width)
        //                {
        //                    newWidths[colIndex - range.Left] = width;
        //                }
        //            }
        //        }
        //    }

        //    bool equal = true;

        //    for (int n = 0; n < newWidths.Length; n++)
        //    {
        //        if (newWidths[n] != -1)
        //        {
        //            equal &= newWidths[n] == oldWidths[n];
        //        }
        //    }

        //    for (int n = 0; n < this.Model.TableProperties.GroupedColumns.Count; n++)
        //    {
        //        newWidths[n] = GridDataTableModel.ExpandCollapseCellWidth;
        //    }

        //    //if (this.Model.Table.HasNestedTables)
        //    //{
        //    //    newWidths[0] = 24d;
        //    //}

        //    if (!equal)
        //    {
        //        LineSizeUtil.SetRange(this.Model.ColumnWidths, range.Left, range.Right, newWidths);
        //    }
        //}

        private bool CellTypeFilter(GridDataTableStyleInfoIdentity cellIdentity, GridControlLengthUnitType type)
        {
            bool flag = false;
            switch (type)
            {
                case GridControlLengthUnitType.Auto:
                case GridControlLengthUnitType.AutoWithLastColumnFill:
                    if (cellIdentity.TableCellType == GridDataTableCellType.RecordCell
                        || cellIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell
                        || cellIdentity.TableCellType == GridDataTableCellType.UnboundColumnHeaderCell
                        || cellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell
                        || cellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell
                        || cellIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell
                        || cellIdentity.TableCellType == GridDataTableCellType.SummaryRecordCell)
                    {
                        flag = true;
                    }

                    break;

                case GridControlLengthUnitType.SizeToCells:
                    if (cellIdentity.TableCellType == GridDataTableCellType.RecordCell
                        || cellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell)
                    {
                        flag = true;
                    }

                    break;

                case GridControlLengthUnitType.SizeToHeader:
                    if (cellIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell
                        || cellIdentity.TableCellType == GridDataTableCellType.UnboundColumnHeaderCell)
                    {
                        flag = true;
                    }

                    break;


                default:
                    flag = false;
                    break;
            }
            return flag;
        }

        internal void SetResizedColumnWidth(GridDataVisibleColumn visibleCol, double width)
        {
            if (this.Model.TableProperties.ColumnSizer != GridControlLengthUnitType.None)
            {
                return;
            }

            if (this.IsAutoOnLoad)
            {
                return;
            }

            var starColumnWidth = this.Model.TableProperties.VisibleColumns.Where(d => (d.Width.IsStar && d != visibleCol)).Sum(d => d.Width.Value);
            var remainingColWidth = this.Model.TableProperties.VisibleColumns.Where(d => !d.Width.IsStar && d != visibleCol).Sum(d => d.ActualWidth);
            var parentWidth = (this.Model.Grid.Parent as FrameworkElement).ActualWidth;

            if ((this.Model.Grid.Parent as FrameworkElement) is ScrollViewer)
            {
#if !SILVERLIGHT
                ScrollBar VScrollBar = ((ScrollViewer)this.Model.Grid.Parent).Template.FindName("PART_VerticalScrollBar", ((ScrollViewer)this.Model.Grid.Parent)) as ScrollBar;
                //((ScrollViewer)this.Model.Grid.Parent).UpdateLayout();
                if (VScrollBar != null)
                {
                    if (this.Model.RowHeights.TotalExtent > ((ScrollViewer)(this.Model.Grid.Parent as FrameworkElement)).ActualHeight)
                    {
                        parentWidth -= (VScrollBar.ActualWidth + VScrollBar.Margin.Left + VScrollBar.Margin.Right);
                    }
                }
#else
              if ((this.Model.Grid.Parent as FrameworkElement) is ScrollViewer)
                {
                    if (this.Model.RowHeights.TotalExtent > ((ScrollViewer)(this.Model.Grid.Parent as FrameworkElement)).ActualHeight)
                    {
                        parentWidth -= GridColumnAutoSizer.ScrollBarWidth;
                    }
                }
#endif
            }

            parentWidth -= (width + GridColumnAutoSizer.Border);
            parentWidth -= (this.Model.TableProperties.GroupedColumns.Count * GridDataTableModel.ExpandCollapseCellWidth);

            if (this.Model.Table.HasNestedTables)
            {
                parentWidth -= GridDataTableModel.ExpandCollapseCellWidth;
            }

            this.Model.TableProperties.VisibleColumns.Where(d => d.Width.IsStar && d != visibleCol).ForEach(d =>
                    {
                        var val = ((parentWidth - remainingColWidth) / starColumnWidth) * d.Width.Value;
                        if (val < GridDataVisibleColumn.MinWidth)
                        {
                            d.ActualWidth = GridDataVisibleColumn.MinWidth;
                        }
                        else
                        {
                            d.ActualWidth = val;
                        }
                    });
        }
    }
}