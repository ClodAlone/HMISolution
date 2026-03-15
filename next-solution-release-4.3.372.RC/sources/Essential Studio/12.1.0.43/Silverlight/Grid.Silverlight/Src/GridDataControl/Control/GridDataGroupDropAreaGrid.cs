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
    using Syncfusion.Linq;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows;
    using System.Windows;
    using System.Windows.Controls;

    public class GridDataGroupDropAreaGridImpl : GridControlBase
    {
        public GridDataGroupDropAreaGridImpl()
            : base()
        {
            var selectMouseController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            this.MouseControllerDispatcher.Remove(selectMouseController);
            //this.MouseControllerDispatcher.Remove(selectMouseController);
            var columnsController = this.MouseControllerDispatcher.Find("GridResizeColumnsMouseController") as GridResizeColumnsMouseController;
            this.MouseControllerDispatcher.Remove(columnsController);

            var rowsController = this.MouseControllerDispatcher.Find("GridResizeRowsMouseController") as GridResizeRowsMouseController;
            this.MouseControllerDispatcher.Remove(rowsController);
            this.Loaded += new System.Windows.RoutedEventHandler(GridDataGroupDropAreaGridImpl_Loaded);
        }

        void GridDataGroupDropAreaGridImpl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= new System.Windows.RoutedEventHandler(GridDataGroupDropAreaGridImpl_Loaded);
            var border = ((FrameworkElement)this.Parent).FindParentElementOfType<Border>();            
            if (border != null)
            {
                border.SizeChanged += new SizeChangedEventHandler(border_SizeChanged);
                this.ApplyWidths();
            }
        }

        void border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ApplyWidths();
        }

        internal void ApplyWidths()
        {
            if (this.Parent == null || this.Model == null)
            {
                return;
            }

            var border = ((FrameworkElement)this.Parent).FindParentElementOfType<Border>();
            var actualWidth = border.ActualWidth;
            double scrollColumnsTotalExtent = this.Model.ColumnWidths.TotalExtent;
            if (actualWidth == 0 || actualWidth < scrollColumnsTotalExtent)
            {
                return;
            }

            var width = actualWidth - scrollColumnsTotalExtent + 1000d;
            this.Model.ColumnWidths.SetRange(98, 99, width);
            this.Model.InvalidateVisual();
        }

        #region ColumnWidthsProvider
        public class GridDataColumnWidthsEventArgs : SyncfusionCancelEventArgs
        {
            public GridDataColumnWidthsEventArgs(int colIndex)
            {
                this.ColumnIndex = colIndex;
                this.Cancel = false;
            }

            public int ColumnIndex
            {
                get;
                private set;
            }

            public double Value
            {
                get;
                set;
            }
        }

        internal class GridDataCustomColumnWidthsProvider : IEditableLineSizeHost
        {
            LineSizeCollection _lineSizeCollection;
            GridDataGroupDropAreaGridImpl grid;
            public GridDataCustomColumnWidthsProvider(GridDataGroupDropAreaGridImpl grid, LineSizeCollection lineSizeCollection)
            {
                this._lineSizeCollection = lineSizeCollection;
                this.grid = grid;
            }

            double IEditableLineSizeHost.DefaultLineSize
            {
                get
                {
                    return _lineSizeCollection.DefaultLineSize;
                }
                set
                {
                    _lineSizeCollection.DefaultLineSize = value;
                }
            }

            int IEditableLineSizeHost.FooterLineCount
            {
                get
                {
                    return _lineSizeCollection.FooterLineCount;
                }
                set
                {
                    _lineSizeCollection.FooterLineCount = value;
                }
            }

            IEditableLineSizeHost IEditableLineSizeHost.GetNestedLines(int index)
            {
                return _lineSizeCollection.GetNestedLines(index);
            }

            int IEditableLineSizeHost.HeaderLineCount
            {
                get
                {
                    return _lineSizeCollection.HeaderLineCount;
                }
                set
                {
                    _lineSizeCollection.HeaderLineCount = value;
                }
            }

            void IEditableLineSizeHost.InsertLines(int insertAtLine, int count, IEditableLineSizeHost moveLines)
            {
                _lineSizeCollection.InsertLines(insertAtLine, count, moveLines);
            }

            int IEditableLineSizeHost.LineCount
            {
                get
                {
                    return _lineSizeCollection.LineCount;
                }
                set
                {
                    _lineSizeCollection.LineCount = value;
                }
            }

            void IEditableLineSizeHost.RemoveLines(int removeAtLine, int count, IEditableLineSizeHost moveLines)
            {
                _lineSizeCollection.RemoveLines(removeAtLine, count, moveLines);
            }

            void IEditableLineSizeHost.SetHidden(int from, int to, bool hide)
            {
                _lineSizeCollection.SetHidden(from, to, hide);
            }

            void IEditableLineSizeHost.SetNestedLines(int index, IEditableLineSizeHost nestedLines)
            {
                _lineSizeCollection.SetNestedLines(index, nestedLines);
            }

            void IEditableLineSizeHost.SetRange(int from, int to, double size)
            {
                _lineSizeCollection.SetRange(from, to, size);
            }

            bool IEditableLineSizeHost.SupportsInsertRemove
            {
                get { return _lineSizeCollection.SupportsInsertRemove; }
            }

            bool IEditableLineSizeHost.SupportsNestedLines
            {
                get { return _lineSizeCollection.SupportsNestedLines; }
            }

            double IEditableLineSizeHost.TotalExtent
            {
                get { return _lineSizeCollection.TotalExtent; }
            }

            double IEditableLineSizeHost.this[int index]
            {
                get
                {
                    return _lineSizeCollection[index];
                }
                set
                {
                    _lineSizeCollection[index] = value;
                }
            }

            event DefaultLineSizeChangedEventHandler ILineSizeHost.DefaultLineSizeChanged
            {
                add { _lineSizeCollection.DefaultLineSizeChanged += value; }
                remove { _lineSizeCollection.DefaultLineSizeChanged -= value; }
            }

            event EventHandler ILineSizeHost.FooterLineCountChanged
            {
                add { _lineSizeCollection.FooterLineCountChanged += value; }
                remove { _lineSizeCollection.FooterLineCountChanged -= value; }
            }

            double ILineSizeHost.GetDefaultLineSize()
            {
                return _lineSizeCollection.GetDefaultLineSize();
            }

            int ILineSizeHost.GetFooterLineCount()
            {
                return _lineSizeCollection.GetFooterLineCount();
            }

            int ILineSizeHost.GetHeaderLineCount()
            {
                return this._lineSizeCollection.GetHeaderLineCount();
            }

            bool ILineSizeHost.GetHidden(int index, out int repeatValueCount)
            {
                return _lineSizeCollection.GetHidden(index, out repeatValueCount);
            }

            int ILineSizeHost.GetLineCount()
            {
                return _lineSizeCollection.GetLineCount();
            }

            private bool suspend = false;
            double ILineSizeHost.GetSize(int index, out int repeatValueCount)
            {
                repeatValueCount = 1;
                var args = new GridDataColumnWidthsEventArgs(index);
                if (!this.grid.RaiseQueryColumnWidths(args) || suspend)
                {
                    var value = _lineSizeCollection.GetSize(index, out repeatValueCount);
                    return value;
                }
                else
                {
                    this.suspend = true;
                    if (this._lineSizeCollection.GetSize(index, out repeatValueCount) != args.Value)
                    {
                        this._lineSizeCollection.SetRange(index, index, args.Value);
                    }
                    this.suspend = false;
                    return args.Value;
                }
            }

            event EventHandler ILineSizeHost.HeaderLineCountChanged
            {
                add { _lineSizeCollection.HeaderLineCountChanged += value; }
                remove { _lineSizeCollection.HeaderLineCountChanged -= value; }
            }

            void ILineSizeHost.InitializeScrollAxis(ScrollAxisBase scrollAxis)
            {
                _lineSizeCollection.InitializeScrollAxis(scrollAxis);
            }

            event EventHandler ILineSizeHost.LineCountChanged
            {
                add { _lineSizeCollection.LineCountChanged += value; }
                remove { _lineSizeCollection.LineCountChanged -= value; }
            }

            event HiddenRangeChangedEventHandler ILineSizeHost.LineHiddenChanged
            {
                add { _lineSizeCollection.LineHiddenChanged += value; }
                remove { _lineSizeCollection.LineHiddenChanged -= value; }
            }

            event RangeChangedEventHandler ILineSizeHost.LineSizeChanged
            {
                add { _lineSizeCollection.LineSizeChanged += value; }
                remove { _lineSizeCollection.LineSizeChanged -= value; }
            }

            event LinesInsertedEventHandler ILineSizeHost.LinesInserted
            {
                add { _lineSizeCollection.LinesInserted += value; }
                remove { _lineSizeCollection.LinesInserted -= value; }
            }

            event LinesRemovedEventHandler ILineSizeHost.LinesRemoved
            {
                add { _lineSizeCollection.LinesRemoved += value; }
                remove { _lineSizeCollection.LinesRemoved -= value; }
            }

            public IEditableLineSizeHost CreateMoveLines()
            {
                return this._lineSizeCollection.CreateMoveLines();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
        
        #endregion

        internal new GridDataGroupDropAreaModel Model
        {
            get
            {
                if (base.Model is GridDataGroupDropAreaModel)
                {
                    return (GridDataGroupDropAreaModel)base.Model;
                }

                return null;
            }

            set
            {
                base.Model = value;
            }
        }

        internal void AttachParentGrid(GridDataTableModel tableModel, GridControlBase parentGrid)
        {
            this.Model = new GridDataGroupDropAreaModel(tableModel);
            this.ParentGrid = parentGrid;
            tableModel.TableProperties.GroupedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnGroupedColumnsCollectionChanged);
            if ((this.MouseControllerDispatcher.Find("GridDataGroupDropMouseController")) == null)
                this.MouseControllerDispatcher.Add(new GridDataGroupDropMouseController(this.ParentGrid));
            var groupDropAreaParent = this.Parent as FrameworkElement;
            groupDropAreaParent.SizeChanged += new SizeChangedEventHandler(groupDropAreaParent_SizeChanged);
            this.SetHeight();
        }

        void groupDropAreaParent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetHeight();
        }

        /// <summary>
        /// Sets the height.
        /// </summary>
        private void SetHeight()
        {
            var parentModel = this.ParentGrid.Model as GridDataTableModel;
            var grid = this.FindParentElementOfType<GridDataControl>();
            if (grid != null)
            {
                int index = parentModel.TableProperties.StackedHeaderRows != null ? parentModel.TableProperties.StackedHeaderRows.Count : 0;
                var headerHeight = this.ParentGrid.Model.RowHeights[index];
                //var totalHeight = (grid.GroupDropAreaHeight - 20) <= headerHeight ? headerHeight + 20 : grid.GroupDropAreaHeight;
                //grid.GroupDropAreaHeight = totalHeight;
                //The above has been commented because to give priority when the GroupDropAreaHeight is set in sample side.
                var paddingHeight = (grid.GroupDropAreaHeight - headerHeight) / 2;
                this.Model.RowHeights[0] = paddingHeight;
                this.Model.RowHeights[2] = paddingHeight;
                this.Model.RowHeights[1] = headerHeight;
            }
        }

        private void OnGroupedColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCells();
        }

        internal bool RaiseQueryColumnWidths(GridDataColumnWidthsEventArgs args)
        {
            this.OnQueryColumnWidths(args);
            return args.Cancel;
        }

        protected virtual void OnQueryColumnWidths(GridDataColumnWidthsEventArgs args)
        {
            if (args.ColumnIndex >= 0 && args.ColumnIndex < GridDataGroupDropAreaModel.OffsetColumn)
            {
                return;
            }

            if (args.ColumnIndex == 99)
            {
                return;
            }

            args.Cancel = true;
            if (this.Model.IsHeaderColIndex(args.ColumnIndex))
            {
                var style = this.Model[1, args.ColumnIndex];
                var size = style.CellModel.CalculatePreferredCellSize(1, args.ColumnIndex, style, GridQueryBounds.Width);
                if (!size.IsEmpty)
                {
                    args.Value = size.Width + GridDataHeaderCellControl.MinWidth;
                }
            }
            else
            {
                args.Value = 10d;
            }
        }

        protected override void WireModel()
        {
            base.WireModel();
            if (this.Model != null)
            {
                this.ColumnWidthsProvider = new GridDataCustomColumnWidthsProvider(this, (LineSizeCollection)this.Model.ColumnWidths);
            }
        }

        protected override void UnwireModel()
        {
            base.UnwireModel();
            if (this.Model != null)
            {
                var tableModel = this.Model.TableModel;
                tableModel.TableProperties.GroupedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnGroupedColumnsCollectionChanged);
            }
        }

        public GridControlBase ParentGrid
        {
            get;
            private set;
        }

        public void Refresh()
        {
            this.InvalidateCells();
        }
    }

    public interface IGridDataGroupDropAreaModel
    {
        GridDataTableModel TableModel
        {
            get;
        }
    }

    internal class GridDataGroupDropAreaModel : GridModel, IGridDataGroupDropAreaModel
    {
        public GridDataGroupDropAreaModel(GridDataTableModel tableModel)
        {
            this.Options.AllowSelection = GridSelectionFlags.None;
            this.Options.ActivateCurrentCellBehavior = GridCellActivateAction.None;
            this.Options.ShowCurrentCell = false;
            this.InitModel(tableModel);
        }

        private void InitModel(GridDataTableModel tableModel)
        {
            this.tableModel = tableModel;
            this.HeaderRows = 0;
            this.FrozenRows = 0;
            this.FrozenColumns = 0;
            this.RowCount = 3;
            this.ColumnCount = 100;
            this.RowHeights[0] = 10d;
            this.RowHeights[1] = 30d;
            this.RowHeights[2] = 10d;
            this.ColumnWidths[0] = 0d;
            this.FrozenRows = 0;
            this.FrozenColumns = 0;
            for (int i = 1; i < OffsetColumn; i++)
            {
                this.ColumnWidths[i] = 5d;
            }

            this.ColumnWidths.DefaultLineSize = 120d;
            this.TableStyle.CellType = "Static";
            this.TableStyle.Borders.All = new Pen();
            this.RefreshVisualStyles();
            this.GroupDropAreaText = GridDataResourceWrapper.DragDropText;//"Drag and Drop Columns here";
            var headerModel = new GridDataHeaderCellModel();
            headerModel.CanShowFilterButton = false;
            headerModel.CanShowColumnOptionsButton = false;
            this.CellModels.Add("SortableHeaderCell", headerModel);
        }

        internal void RefreshVisualStyles()
        {
            // this.TableStyle.Background = this.TableProperties.GetVisualStyle(this.TableProperties.VisualStyle).GroupAreaBackgroundBrush;
            var isInDesign = System.ComponentModel.DesignerProperties.IsInDesignTool;
            if (isInDesign)
            {
                this.CellSpanBackgrounds.Clear();
                var span = new Cells.CellSpanBackgroundInfo(0, 0, this.RowCount, this.ColumnCount - 1);
                span.Background = this.TableProperties.GetVisualStyle(VisualStyle.Default).GroupAreaBackgroundBrush;
                span.Border = new Pen(null, 0d);
                this.CellSpanBackgrounds.Add(span);
                //this.TableStyle.Background = this.TableProperties.GetVisualStyle(VisualStyle.Default).GroupAreaBackgroundBrush;
            }
            else
            {
                // this.TableStyle.Background = this.TableProperties.GetVisualStyle(this.TableProperties.VisualStyle).GroupAreaBackgroundBrush;
                this.CellSpanBackgrounds.Clear();
                var span = new Cells.CellSpanBackgroundInfo(0, 0, this.RowCount, this.ColumnCount - 1);
                span.Background = this.TableProperties.Model.GetGroupAreaBackgroundBrush();
                span.Border = new Pen(null, 0d);
                this.CellSpanBackgrounds.Add(span);
            }
        }

        public string GroupDropAreaText
        {
            get;
            set;
        }

        public double GroupDropAreaHeight
        {
            get;
            set;
        }

        private GridDataTableModel tableModel;
        public GridDataTableModel TableModel
        {
            get
            {
                return this.tableModel;
            }
        }

        public GridDataTable Table
        {
            get
            {
                return this.TableModel.Table;
            }
        }

        public GridDataTableProperties TableProperties
        {
            get
            {
                return this.TableModel.TableProperties;
            }
        }

        public const int OffsetColumn = 4;

        public bool IsHeaderColIndex(int colIndex)
        {
            return colIndex >= OffsetColumn && colIndex % 2 == 0 && (colIndex / 2 - 2) < Math.Max(1, this.TableProperties.GroupedColumns.Count);
        }

        public int ColIndexToField(int colIndex)
        {
            int ci = Math.Max(OffsetColumn, colIndex);
            int num = Math.Max(0, Math.Min(this.TableProperties.GroupedColumns.Count, ci / 2 - 2));
            return num;
        }

        public int ResolveVisibleColumnIndexToPosition(int colIdx)
        {
            if (this.TableProperties == null)
            {
                return -1;
            }

            if (this.Table.HasNestedTables)
            {
                colIdx = this.TableProperties.ShowRecordPlusMinus ? colIdx + 1 : colIdx;
            }

            if (this.Table.HasGroups && this.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = this.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx + maxLevel;
            }

            //// adjust the value of colIndex to get the exact column
            colIdx = this.TableProperties.ShowRowHeader ? colIdx + 1 : colIdx;
            return colIdx;
        }

        public int ResolvePositionToVisibleColumnIndex(int colIdx)
        {
            if (this.TableProperties == null)
            {
                return -1;
            }

            if (this.Table.HasNestedTables)
            {
                colIdx = this.TableProperties.ShowRecordPlusMinus ? colIdx - 1 : colIdx;
            }
            
            /// Adjusting the column index based on details view existance
            colIdx -= this.Table.HasDetailsView ? 1 : 0;

            if (this.Table.HasGroups && this.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = this.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx - maxLevel;
            }

            //// adjust the value of colIndex to get the exact column
            colIdx = this.TableProperties.ShowRowHeader ? colIdx - 1 : colIdx;
            return colIdx;
        }

        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (this.IsHeaderColIndex(e.Cell.ColumnIndex) && e.Cell.RowIndex == 1)
            {
                int num = this.ColIndexToField(e.Cell.ColumnIndex);
                if (num < this.TableProperties.GroupedColumns.Count)
                {
                    var groupedColumn = this.TableProperties.GroupedColumns[num];
                    var sortCol = this.TableProperties.GetSortColumnForGroup(groupedColumn);
                    var visibleColumn = this.TableProperties.VisibleColumns.Where(v => v.MappingName == groupedColumn.ColumnName).FirstOrDefault();
                    e.Style.CellType = "SortableHeaderCell";
                    e.Style.CellValue = visibleColumn != null ? visibleColumn.HeaderText : groupedColumn.ColumnName;
                    if (sortCol != null)
                    {
                        e.Style.Tag = sortCol.SortDirection;
                    }
                    e.Style.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    e.Style.Font = this.TableModel.GetHeaderFont();
                    e.Style.Borders = this.TableModel.GetGroupCellBorders();
                    e.Style.TextMargins = this.TableModel.GetHeaderTextMargins();
                    e.Style.Background = this.TableModel.GetHeaderBackground();
                    e.Style.Foreground = this.TableModel.GetHeaderForeground();
                }
                else if (num == 0 && this.TableProperties.GroupedColumns.Count == 0)
                {
                    e.Style.Text = this.GroupDropAreaText;
                    e.Style.CellType = "Static";
                    e.Style.Foreground = this.TableModel.GetGroupAreaForegroundBrush();
                    e.Style.Font = this.TableModel.GetValueFont();
                    e.Style.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                }
            }
            base.OnQueryCellInfo(e);
        }

        protected override void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            var headerColIndex = this.IsHeaderColIndex(e.CellRowColumnIndex.ColumnIndex);
            if (headerColIndex && e.CellRowColumnIndex.RowIndex == 1)
            {
                int num = this.ColIndexToField(e.CellRowColumnIndex.ColumnIndex);
                if ((num == 0 && this.TableProperties.GroupedColumns.Count == 0) || (num == this.TableProperties.GroupedColumns.Count))
                {
                    e.Range = new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(
                        e.CellRowColumnIndex.RowIndex,
                        OffsetColumn,
                        e.CellRowColumnIndex.RowIndex,
                        this.ColumnCount - 1);// { SpanWholeRow = true, SpanWholeColumn = true };
                    e.Handled = true;
                }
            }
            base.OnQueryCoveredRange(e);
        }
    }
}
