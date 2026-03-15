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
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Collections.Specialized;
    using System.Windows;

    public class GridDataGroupDropAreaGridImpl : GridControlBase
    {

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupDropAreaGridImpl"/> class.
        /// </summary>
        public GridDataGroupDropAreaGridImpl()
            : base()
        {
            var selectMouseController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            selectMouseController.Dispose();
            this.MouseControllerDispatcher.Remove(selectMouseController);
            this.MouseControllerDispatcher.Remove(this.MouseControllerDispatcher.Find("ResizeColumnsMouseController"));
            this.ClearVisualsCacheWhenUnloaded = true;
            DependencyPropertyDescriptor pd = DependencyPropertyDescriptor.FromProperty(GridDataControl.ActualWidthProperty, typeof(GridDataControl));
            pd.AddValueChanged(this, Border_WidthChanged);
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the WidthChanged event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Border_WidthChanged(object sender, EventArgs e)
        {
            this.ApplyWidths();
        }

        /// <summary>
        /// Handles the SizeChanged event of the groupDropAreaParent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        //private void groupDropAreaParent_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    this.SetHeight();
        //}

        /// <summary>
        /// Called when [grouped columns collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnGroupedColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCells();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Applies the widths.
        /// </summary>
        internal void ApplyWidths()
        {
            if (this.Parent == null || this.Model == null)
            {
                return;
            }

            var scrollViewer = this.Parent.Parent() as Border;
            var actualWidth = scrollViewer.ActualWidth;
            double scrollColumnsTotalExtent = this.Model.ColumnWidths[this.Model.ColumnCount - 1];
            if (actualWidth == 0 || actualWidth < scrollColumnsTotalExtent)
            {
                return;
            }

            var width = actualWidth - scrollColumnsTotalExtent + 1000d;
            this.Model.ColumnWidths.SetRange(98, 99, width);
            
            this.Model.InvalidateVisual();
        }

        /// <summary>
        /// Attaches the parent grid.
        /// </summary>
        /// <param name="tableModel">The table model.</param>
        /// <param name="parentGrid">The parent grid.</param>
        internal void AttachParentGrid(GridDataTableModel tableModel, GridControlBase parentGrid)
        {
            this.isGroupModelLoaded = true;
            this.Model = new GridDataGroupDropAreaModel(tableModel);
            this.UseGuidelineSetToRenderBorder = true;
            //this.ParentGrid = parentGrid;
            ((INotifyCollectionChanged)tableModel.TableProperties.GroupedColumns).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnGroupedColumnsCollectionChanged);
            if ((this.MouseControllerDispatcher.Find("GridDataGroupDropMouseController")) == null)
                this.MouseControllerDispatcher.Add(new GridDataGroupDropMouseController(parentGrid));
            //var groupDropAreaParent = this.Parent as FrameworkElement;
            //groupDropAreaParent.SizeChanged += new SizeChangedEventHandler(groupDropAreaParent_SizeChanged);
            SetHeight(parentGrid);
        }
        private bool isGroupModelLoaded = false;
        /// <summary>
        /// Sets the height.
        /// </summary>
        private void SetHeight(GridControlBase parentGrid)
        {
            var parentModel = parentGrid.Model as GridDataTableModel;
            var grid = this.FindParentElementOfType<GridDataControl>();
            if (grid != null)
            {
                int index = parentModel.TableProperties.StackedHeaderRows != null ? parentModel.TableProperties.StackedHeaderRows.Count : 0;
                var headerHeight = parentGrid.Model.RowHeights[index];
                //var totalHeight = (grid.GroupDropAreaHeight - 20) <= headerHeight ? headerHeight + 20 : grid.GroupDropAreaHeight;
                //grid.GroupDropAreaHeight = totalHeight;
                //The above has been commented because to give priority when the GroupDropAreaHeight is set in sample side.
                var paddingHeight = (grid.GroupDropAreaHeight - headerHeight) / 2;
                this.Model.RowHeights[0] = paddingHeight;
                this.Model.RowHeights[2] = paddingHeight;
                this.Model.RowHeights[1] = headerHeight;
            }
        }

        /// <summary>
        /// Raises the query column widths.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridDataGroupDropAreaGridImpl.GridDataColumnWidthsEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        internal bool RaiseQueryColumnWidths(GridDataColumnWidthsEventArgs args)
        {
            this.OnQueryColumnWidths(args);
            return args.Cancel;
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Raises the <see cref="E:QueryColumnWidths"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridDataGroupDropAreaGridImpl.GridDataColumnWidthsEventArgs"/> instance containing the event data.</param>
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
                    args.Value = size.Width + GridDataHeaderCellControl.MinimumWidth;//Sorting Indicators and Grouping Indicators
                }
            }
            else
            {
                args.Value = 10d;
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Wires the model.
        /// </summary>
        protected override void WireModel()
        {
            base.WireModel();
            if (this.Model != null)
            {
                this.ColumnWidthsProvider = new GridDataCustomColumnWidthsProvider(this, (LineSizeCollection)this.Model.ColumnWidths);
            }
        }

        /// <summary>
        /// Unwires the model.
        /// </summary>
        protected override void UnwireModel()
        {
            base.UnwireModel();
            if (this.Model != null)
            {
                var tableModel = this.Model.TableModel;
                if (tableModel != null && tableModel.TableProperties != null && tableModel.TableProperties.GroupedColumns != null)
                    ((INotifyCollectionChanged)tableModel.TableProperties.GroupedColumns).CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnGroupedColumnsCollectionChanged);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Model != null)
                {
                    this.Model.Dispose();
                    this.Model = null;
                }
                this.ColumnWidthsProvider.Dispose();
            }
            base.Dispose(disposing);
            if (this.MouseControllerDispatcher != null)
            {
                this.MouseControllerDispatcher.Dispose();
            }
            DependencyPropertyDescriptor pd = DependencyPropertyDescriptor.FromProperty(GridDataControl.ActualWidthProperty, typeof(GridDataControl));
            pd.RemoveValueChanged(this, Border_WidthChanged);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the parent grid.
        /// </summary>
        /// <value>The parent grid.</value>
        //public GridControlBase ParentGrid
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public new GridDataGroupDropAreaModel Model
        {
            get
            {
                if (this.isGroupModelLoaded && base.Model is GridDataGroupDropAreaModel)
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            if (this.Model != null)
                this.Model.RefreshVisualStyles();
            this.InvalidateCells();
        }

        #endregion

        public class GridDataColumnWidthsEventArgs : SyncfusionCancelEventArgs
        {
            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="GridDataColumnWidthsEventArgs"/> class.
            /// </summary>
            /// <param name="colIndex">Index of the col.</param>
            public GridDataColumnWidthsEventArgs(int colIndex)
            {
                this.ColumnIndex = colIndex;
                this.Cancel = false;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the index of the column.
            /// </summary>
            /// <value>The index of the column.</value>
            public int ColumnIndex
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public double Value
            {
                get;
                set;
            }

            #endregion

        }

        internal class GridDataCustomColumnWidthsProvider : IEditableLineSizeHost
        {

            #region Variables

            LineSizeCollection _lineSizeCollection;
            GridDataGroupDropAreaGridImpl grid;
            private bool suspend = false;

            #endregion

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="GridDataCustomColumnWidthsProvider"/> class.
            /// </summary>
            /// <param name="grid">The grid.</param>
            /// <param name="lineSizeCollection">The line size collection.</param>
            public GridDataCustomColumnWidthsProvider(GridDataGroupDropAreaGridImpl grid, LineSizeCollection lineSizeCollection)
            {
                this._lineSizeCollection = lineSizeCollection;
                this.grid = grid;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Creates the object which holds temporary state when moving lines.
            /// </summary>
            /// <returns></returns>
            public IEditableLineSizeHost CreateMoveLines()
            {
                return this._lineSizeCollection.CreateMoveLines();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this._lineSizeCollection.Dispose();
            }

            #endregion

            #region Events

            /// <summary>
            /// Occurs when the default line size changed.
            /// </summary>
            event DefaultLineSizeChangedEventHandler ILineSizeHost.DefaultLineSizeChanged
            {
                add { _lineSizeCollection.DefaultLineSizeChanged += value; }
                remove { _lineSizeCollection.DefaultLineSizeChanged -= value; }
            }

            /// <summary>
            /// Occurs when the footer line count was changed.
            /// </summary>
            event EventHandler ILineSizeHost.FooterLineCountChanged
            {
                add { _lineSizeCollection.FooterLineCountChanged += value; }
                remove { _lineSizeCollection.FooterLineCountChanged -= value; }
            }

            /// <summary>
            /// Occurs when the header line count was changed.
            /// </summary>
            event EventHandler ILineSizeHost.HeaderLineCountChanged
            {
                add { _lineSizeCollection.HeaderLineCountChanged += value; }
                remove { _lineSizeCollection.HeaderLineCountChanged -= value; }
            }

            /// <summary>
            /// Occurs when the line count was changed.
            /// </summary>
            event EventHandler ILineSizeHost.LineCountChanged
            {
                add { _lineSizeCollection.LineCountChanged += value; }
                remove { _lineSizeCollection.LineCountChanged -= value; }
            }

            /// <summary>
            /// Occurs when a lines hidden state changed.
            /// </summary>
            event HiddenRangeChangedEventHandler ILineSizeHost.LineHiddenChanged
            {
                add { _lineSizeCollection.LineHiddenChanged += value; }
                remove { _lineSizeCollection.LineHiddenChanged -= value; }
            }

            /// <summary>
            /// Occurs when a lines size was changed.
            /// </summary>
            event RangeChangedEventHandler ILineSizeHost.LineSizeChanged
            {
                add { _lineSizeCollection.LineSizeChanged += value; }
                remove { _lineSizeCollection.LineSizeChanged -= value; }
            }

            /// <summary>
            /// Occurs when lines were inserted.
            /// </summary>
            event LinesInsertedEventHandler ILineSizeHost.LinesInserted
            {
                add { _lineSizeCollection.LinesInserted += value; }
                remove { _lineSizeCollection.LinesInserted -= value; }
            }

            /// <summary>
            /// Occurs when lines were removed.
            /// </summary>
            event LinesRemovedEventHandler ILineSizeHost.LinesRemoved
            {
                add { _lineSizeCollection.LinesRemoved += value; }
                remove { _lineSizeCollection.LinesRemoved -= value; }
            }

            #endregion

            #region Helper Methods

            /// <summary>
            /// Gets or sets the default size of lines.
            /// </summary>
            /// <value>The default size of lines.</value>
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

            /// <summary>
            /// Gets or sets the footer line count.
            /// </summary>
            /// <value>The footer line count.</value>
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

            /// <summary>
            /// Gets the nested lines.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns></returns>
            IEditableLineSizeHost IEditableLineSizeHost.GetNestedLines(int index)
            {
                return _lineSizeCollection.GetNestedLines(index);
            }

            /// <summary>
            /// Gets or sets the header line count.
            /// </summary>
            /// <value>The header line count.</value>
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

            /// <summary>
            /// Insert a number of lines.
            /// </summary>
            /// <param name="insertAtLine">The index of the first line to insert.</param>
            /// <param name="count">The count.</param>
            /// <param name="moveLines">A container with saved state from a preceeding <see cref="M:Syncfusion.Windows.Controls.Scroll.IEditableLineSizeHost.RemoveLines(System.Int32,System.Int32,Syncfusion.Windows.Controls.Scroll.IEditableLineSizeHost)"/> call when lines should be moved. When it is null empty lines with default size are inserted.</param>
            void IEditableLineSizeHost.InsertLines(int insertAtLine, int count, IEditableLineSizeHost moveLines)
            {
                _lineSizeCollection.InsertLines(insertAtLine, count, moveLines);
            }

            /// <summary>
            /// Gets or sets the line count.
            /// </summary>
            /// <value>The line count.</value>
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

            /// <summary>
            /// Removes a number of lines.
            /// </summary>
            /// <param name="removeAtLine">The index of the first line to be removed.</param>
            /// <param name="count">The count.</param>
            /// <param name="moveLines">A container to save state for a subsequent <see cref="M:Syncfusion.Windows.Controls.Scroll.IEditableLineSizeHost.InsertLines(System.Int32,System.Int32,Syncfusion.Windows.Controls.Scroll.IEditableLineSizeHost)"/> call when lines should be moved.</param>
            void IEditableLineSizeHost.RemoveLines(int removeAtLine, int count, IEditableLineSizeHost moveLines)
            {
                _lineSizeCollection.RemoveLines(removeAtLine, count, moveLines);
            }

            /// <summary>
            /// Sets the hidden state for a range of lines.
            /// </summary>
            /// <param name="from">From.</param>
            /// <param name="to">To.</param>
            /// <param name="hide">if set to <c>true</c> hide the lines.</param>
            void IEditableLineSizeHost.SetHidden(int from, int to, bool hide)
            {
                _lineSizeCollection.SetHidden(from, to, hide);
            }

            /// <summary>
            /// Sets the nested lines.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="nestedLines">The nested lines.</param>
            void IEditableLineSizeHost.SetNestedLines(int index, IEditableLineSizeHost nestedLines)
            {
                _lineSizeCollection.SetNestedLines(index, nestedLines);
            }

            /// <summary>
            /// Sets the line size for a range.
            /// </summary>
            /// <param name="from">From.</param>
            /// <param name="to">To.</param>
            /// <param name="size">The size.</param>
            void IEditableLineSizeHost.SetRange(int from, int to, double size)
            {
                _lineSizeCollection.SetRange(from, to, size);
            }

            /// <summary>
            /// Gets whether the host supports inserting and removing lines.
            /// </summary>
            /// <value></value>
            bool IEditableLineSizeHost.SupportsInsertRemove
            {
                get { return _lineSizeCollection.SupportsInsertRemove; }
            }

            /// <summary>
            /// Gets whether the host supports nesting.
            /// </summary>
            /// <value></value>
            bool IEditableLineSizeHost.SupportsNestedLines
            {
                get { return _lineSizeCollection.SupportsNestedLines; }
            }

            /// <summary>
            /// Gets the total extent which is the total of all line sizes. Note: This propert only
            /// works if the DistanceCollection has been setup for pixel scrolling; otherwise it returns
            /// double.NaN.
            /// </summary>
            /// <value>The total extent or double.NaN.</value>
            double IEditableLineSizeHost.TotalExtent
            {
                get { return _lineSizeCollection.TotalExtent; }
            }

            /// <summary>
            /// Gets or sets the <see cref="System.Double"/> at the specified index.
            /// </summary>
            /// <value></value>
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



            /// <summary>
            /// Returns the default line size.
            /// </summary>
            /// <returns></returns>
            double ILineSizeHost.GetDefaultLineSize()
            {
                return _lineSizeCollection.GetDefaultLineSize();
            }

            /// <summary>
            /// Gets the footer line count.
            /// </summary>
            /// <returns></returns>
            int ILineSizeHost.GetFooterLineCount()
            {
                return _lineSizeCollection.GetFooterLineCount();
            }

            /// <summary>
            /// Gets the header line count.
            /// </summary>
            /// <returns></returns>
            int ILineSizeHost.GetHeaderLineCount()
            {
                return this._lineSizeCollection.GetHeaderLineCount();
            }

            /// <summary>
            /// Gets the hidden state for a line.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="repeatValueCount">The number of subsequent lines with same state.</param>
            /// <returns></returns>
            bool ILineSizeHost.GetHidden(int index, out int repeatValueCount)
            {
                return _lineSizeCollection.GetHidden(index, out repeatValueCount);
            }

            /// <summary>
            /// Returns the line count.
            /// </summary>
            /// <returns></returns>
            int ILineSizeHost.GetLineCount()
            {
                return _lineSizeCollection.GetLineCount();
            }

            /// <summary>
            /// Gets the size.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="repeatValueCount">The number of subsequent values with same size.</param>
            /// <returns></returns>
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



            /// <summary>
            /// Initializes the scroll axis.
            /// </summary>
            /// <param name="scrollAxis">The scroll axis.</param>
            void ILineSizeHost.InitializeScrollAxis(ScrollAxisBase scrollAxis)
            {
                _lineSizeCollection.InitializeScrollAxis(scrollAxis);
            }

            #endregion

        }

    }

    public interface IGridDataGroupDropAreaModel
    {
        GridDataTableModel TableModel
        {
            get;
        }
    }

    public class GridDataGroupDropAreaModel : GridModel, IGridDataGroupDropAreaModel
    {

        #region Variables

        private GridDataTableModel tableModel;

        #endregion

        #region Const

        public const int OffsetColumn = 4;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupDropAreaModel"/> class.
        /// </summary>
        /// <param name="tableModel">The table model.</param>
        public GridDataGroupDropAreaModel(GridDataTableModel tableModel)
        {
            this.Options.AllowSelection = GridSelectionFlags.None;
            this.Options.ActivateCurrentCellBehavior = GridCellActivateAction.None;
            this.Options.ShowCurrentCell = false;
            this.InitModel(tableModel);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether [is header col index] [the specified col index].
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>
        /// 	<c>true</c> if [is header col index] [the specified col index]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsHeaderColIndex(int colIndex)
        {
            return colIndex >= OffsetColumn && colIndex % 2 == 0 && (colIndex / 2 - 2) < Math.Max(1, this.TableProperties.GroupedColumns.Count);
        }

        /// <summary>
        /// Cols the index to field.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns></returns>
        public int ColIndexToField(int colIndex)
        {
            int ci = Math.Max(OffsetColumn, colIndex);
            int num = Math.Max(0, Math.Min(this.TableProperties.GroupedColumns.Count, ci / 2 - 2));
            return num;
        }

        /// <summary>
        /// Resolves the visible column index to position.
        /// </summary>
        /// <param name="colIdx">The col idx.</param>
        /// <returns></returns>
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

            /// adjusting the column index based on existance of Details View
            colIdx += this.Table.HasDetailsView ? 1 : 0;

            if (this.Table.HasGroups && this.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = this.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx + maxLevel;
            }

            //// adjust the value of colIndex to get the exact column
            colIdx = this.TableProperties.ShowRowHeader ? colIdx + 1 : colIdx;
            return colIdx;
        }

        /// <summary>
        /// Resolves the index of the position to visible column.
        /// </summary>
        /// <param name="colIdx">The col idx.</param>
        /// <returns></returns>
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

            /// adjusting the column index based on existance of Details View
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

        #endregion

        #region Helper Methods

        /// <summary>
        /// Inits the model.
        /// </summary>
        /// <param name="tableModel">The table model.</param>
        private void InitModel(GridDataTableModel tableModel)
        {
            this.tableModel = tableModel;
            this.HeaderRows = 0;
            this.FrozenRows = 0;
            this.FrozenColumns = 0;
            this.TableStyle.FlowDirection = tableModel.Grid.FlowDirection;
            this.RowCount = 3;
            this.ColumnCount = 200;
            this.RowHeights[0] = 10d;
            this.RowHeights[1] = 20d;
            this.RowHeights[2] = 10d;
            this.ColumnWidths[0] = 0d;
            this.FrozenRows = 0;
            this.FrozenColumns = 0;
            for (int i = 1; i < OffsetColumn; i++)
                this.ColumnWidths[i] = 5d;

            this.ColumnWidths.DefaultLineSize = 120d;
            this.TableStyle.CellType = "Static";
            this.TableStyle.Borders.All = new System.Windows.Media.Pen();
            this.RefreshVisualStyles();
            this.GroupDropAreaText = GridDataResourceWrapper.DragDropText;
            var headerModel = new GridDataHeaderCellModel();
            headerModel.CanShowFilterButton = false;
            headerModel.CanShowColumnOptionsButton = false;
            this.CellModels.Add("SortableHeaderCell", headerModel);
        }

        /// <summary>
        /// Refreshes the visual styles.
        /// </summary>
        internal void RefreshVisualStyles()
        {
            if (GridDataTableModelHelper.IsInDesignMode)
            {
                this.CellSpanBackgrounds.Clear();
                var span = new Cells.CellSpanBackgroundInfo(0, 0, this.RowCount, this.ColumnCount - 1);
                span.Background = this.TableProperties.Model.GetGroupAreaBackgroundBrush();
                span.Border = new Pen(null, 0d);
                this.CellSpanBackgrounds.Add(span);
            }
            else
            {
                this.CellSpanBackgrounds.Clear();
                var span = new Cells.CellSpanBackgroundInfo(0, 0, this.RowCount, this.ColumnCount - 1);
                span.Background = this.TableProperties.Model.GetGroupAreaBackgroundBrush();
                span.Border = new Pen(null, 0d);
                this.CellSpanBackgrounds.Add(span);
            }

        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Raises the <see cref="E:QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventArgs"/> instance containing the event data.</param>
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            
            if (this.IsHeaderColIndex(e.Cell.ColumnIndex) && e.Cell.RowIndex == 1)
            {
                int num = this.ColIndexToField(e.Cell.ColumnIndex);
                if (num < this.TableProperties.GroupedColumns.Count)
                {
                    var groupedColumn = this.TableProperties.GroupedColumns[num];
                    var visibleColumn = this.TableProperties.VisibleColumns.Where(v => v.MappingName == groupedColumn.ColumnName).FirstOrDefault();
                    e.Style.CellType = "SortableHeaderCell";
                    e.Style.CellValue = visibleColumn != null ? (visibleColumn.HeaderText != null ? visibleColumn.HeaderText : visibleColumn.MappingName) : groupedColumn.ColumnName;
                    if (visibleColumn != null && visibleColumn.HeaderCellTemplate != null)
                    {
                        e.Style.CellItemTemplate = visibleColumn.HeaderCellTemplate;
                    }
                    var sortCol = this.TableProperties.GetSortColumnForGroup(groupedColumn);
                    if (sortCol != null)
                    {
                        e.Style.Tag = sortCol.SortDirection;
                    }

                    e.Style.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    if (this.TableModel.GridVisualStyle != null)
                    {
                        e.Style.Font = this.TableModel.GetGroupHeaderFont();
                        e.Style.Borders = this.TableModel.GetGroupCellBorders();
                        e.Style.BorderMargins = this.TableModel.GetGroupCellBorderMargins();
                        e.Style.TextMargins = this.TableModel.GetHeaderTextMargins();
                        e.Style.Foreground = this.TableModel.GetHeaderForeground();
                    }
                    
                }
                else if (num == 0 && this.TableProperties.GroupedColumns.Count == 0)
                {
                    e.Style.Text = this.GroupDropAreaText;
                    e.Style.CellType = "Static";
                    if (this.TableModel.GridVisualStyle != null)
                    {
                        e.Style.Foreground = this.TableModel.GetGroupAreaForegroundBrush();
                        e.Style.Font = this.TableModel.GetValueFont();
                    }
                    e.Style.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                }
                
            }
            base.OnQueryCellInfo(e);
        }

        /// <summary>
        /// Raises the <see cref="E:QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridQueryCoveredRangeEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.tableModel != null)
                {
                    this.tableModel.Dispose();
                    this.tableModel = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the group drop area text.
        /// </summary>
        /// <value>The group drop area text.</value>
        public string GroupDropAreaText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the table model.
        /// </summary>
        /// <value>The table model.</value>
        public GridDataTableModel TableModel
        {
            get
            {
                return this.tableModel;
            }
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>The table.</value>
        public GridDataTable Table
        {
            get
            {
                return this.TableModel.Table;
            }
        }

        /// <summary>
        /// Gets the table properties.
        /// </summary>
        /// <value>The table properties.</value>
        public GridDataTableProperties TableProperties
        {
            get
            {
                return this.TableModel.TableProperties;
            }
        }

        #endregion

    }

}

    

