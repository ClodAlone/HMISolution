#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
using System.Windows;
#else
using Windows.Foundation;
#endif
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Syncfusion.UI.Xaml.Grid
{
    public class DetailsViewManager : IDisposable
    {
        #region Fields

        internal SfDataGrid DataGrid;

        internal bool HasDetailsView
        {
            get { return DataGrid.DetailsViewDefinition != null && DataGrid.DetailsViewDefinition.Any(); }
        }

        /// <summary>
        /// Dispose the details view itemssource (ICollectionView) when the row moves out of the view.
        /// </summary>
        public static bool AllowDisposeCollectionView { get; set; }

        #endregion

        #region Ctor

        internal DetailsViewManager(SfDataGrid dataGrid)
        {
            DataGrid = dataGrid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Expands all the records with Details View.
        /// </summary>
        public void ExpandAllDetailsView()
        {
            if (this.DataGrid.View == null) return;
            if (!this.HasDetailsView) return;
            var lineSizeCollection = this.DataGrid.VisualContainer.RowHeights as LineSizeCollection;
            lineSizeCollection.SuspendUpdates();
            var lastRowIndex = this.DataGrid.VisualContainer.RowCount - this.DataGrid.DetailsViewDefinition.Count;
            for (var rowIndex = this.DataGrid.HeaderLineCount; rowIndex < lastRowIndex; rowIndex++)
            {
                RecordEntry record;
                if (this.DataGrid.GridModel.HasGroup)
                    record = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(rowIndex)] as RecordEntry;
                else
                    record = this.DataGrid.View.Records[this.DataGrid.ResolveToRecordIndex(rowIndex)];
                if (record == null) continue;
                record.IsExpanded = true;
                var actualRowIndex = rowIndex;
                foreach (var viewDefinition in this.DataGrid.DetailsViewDefinition)
                {
                    actualRowIndex++;
                    if (viewDefinition is GridViewDefinition)
                    {
                        var gridViewDefinition = viewDefinition as GridViewDefinition;
                        int repeatValueCount;
                        var isHidden = this.DataGrid.VisualContainer.RowHeights.GetHidden(actualRowIndex, out repeatValueCount);
                        LineSizeCollection lines;
                        if (!isHidden)
                            lines = this.DataGrid.VisualContainer.RowHeights.GetNestedLines(actualRowIndex) as LineSizeCollection;
                        else
                        {
                            var itemsource = GetChildSource(record.Data, gridViewDefinition.RelationalColumn);
                            var count = itemsource != null ? itemsource.AsQueryable().Count() * (gridViewDefinition.DataGrid.DetailsViewDefinition.Count + 1) : 0;
                            if (this.DataGrid.HideEmptyGridViewDefinition && count == 0) continue;
                            lines = new LineSizeCollection { LineCount = count + 1, DefaultLineSize = this.DataGrid.RowHeight };
                        }
                        ExpandNestedLines(record.Data, lines, gridViewDefinition,record);
                        this.DataGrid.VisualContainer.RowHeights.SetNestedLines(actualRowIndex, lines);
                        this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, false);

                        if (!record.ChildViews.ContainsKey(gridViewDefinition.RelationalColumn))
                        {
                            record.ChildViews.Add(gridViewDefinition.RelationalColumn, new NestedRecordEntry(record, ((actualRowIndex - rowIndex) - 1)));
                        }
                        record.ChildViews[gridViewDefinition.RelationalColumn].IsNestedLevelExpanded = true;
                    }
                }
                rowIndex += this.DataGrid.DetailsViewDefinition.Count;
            }
            lineSizeCollection.ResumeUpdates();
            this.DataGrid.GridModel.RefreshDataRow();
        }

        /// <summary>
        /// Collapse all the records with Details View.
        /// </summary>
        public void CollapseAllDetailsView()
        {
            if (this.DataGrid.View == null) return;
            var lineSizeCollection = this.DataGrid.VisualContainer.RowHeights as LineSizeCollection;
            lineSizeCollection.SuspendUpdates();

            var lastRowIndex = this.DataGrid.VisualContainer.RowCount - this.DataGrid.DetailsViewDefinition.Count;
            for (var rowIndex = this.DataGrid.HeaderLineCount; rowIndex < lastRowIndex; rowIndex++)
            {
                RecordEntry record;
                if (this.DataGrid.GridModel.HasGroup)
                    record = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(rowIndex)] as RecordEntry;
                else
                    record = this.DataGrid.View.Records[this.DataGrid.ResolveToRecordIndex(rowIndex)];
                if (record == null) continue;
                record.IsExpanded = false;
                var actualRowIndex = rowIndex;
                foreach (var viewDefinition in this.DataGrid.DetailsViewDefinition)
                {
                    if (!(viewDefinition is GridViewDefinition)) continue;
                    var gridViewDefinition = viewDefinition as GridViewDefinition;
                    if (record.ChildViews!=null && record.ChildViews.ContainsKey(viewDefinition.RelationalColumn))
                        record.ChildViews[viewDefinition.RelationalColumn].IsNestedLevelExpanded = false;
                    actualRowIndex++;
                    int repeatValueCount;
                    var isHidden = this.DataGrid.VisualContainer.RowHeights.GetHidden(actualRowIndex, out repeatValueCount);
                    if (isHidden) continue;
                    var dr = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == actualRowIndex) as DetailsViewDataRow;
                    if (dr != null) CollapeseNestedLines(dr.DetailsViewDataGrid, gridViewDefinition);
                    this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, true);
                    this.DataGrid.VisualContainer.RowHeights.SetNestedLines(actualRowIndex, null);
                }
                rowIndex += this.DataGrid.DetailsViewDefinition.Count;
            }
            lineSizeCollection.ResumeUpdates();
            this.DataGrid.GridModel.RefreshDataRow();
        }

        /// <summary>
        /// Expands the record at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public void ExpandDetailsViewAt(int recordIndex)
        {
            if (this.DataGrid.DetailsViewManager.HasDetailsView)
            {
                var rowIndex = this.DataGrid.ResolveToRowIndex(recordIndex);
                var dr = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowIndex);
                if (dr != null)
                    dr.IsExpanded = true;
                else
                {
                    RecordEntry record;
                    if (this.DataGrid.GridModel.HasGroup)
                        record = this.DataGrid.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                    else
                        record = this.DataGrid.View.Records[recordIndex];
                    if (record == null) return;
                    if (record.IsExpanded) return;
                    record.IsExpanded = true;
                    var actualRowIndex = rowIndex;
                    foreach (var viewDefinition in this.DataGrid.DetailsViewDefinition)
                    {
                        actualRowIndex++;
                        if (viewDefinition is GridViewDefinition)
                        {
                            var gridViewDefinition = viewDefinition as GridViewDefinition;
                            var nestedLinesitemsource = GetChildSource(record.Data, gridViewDefinition.RelationalColumn);
                            var count = nestedLinesitemsource.AsQueryable().Count() * (DataGrid.DetailsViewDefinition.Count + 1);
                            var lines = new LineSizeCollection { LineCount = count + 1, DefaultLineSize = this.DataGrid.RowHeight };
                            this.DataGrid.VisualContainer.RowHeights.SetNestedLines(actualRowIndex, lines);
                            this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Collapses the Details View at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public void CollapseDetailsViewAt(int recordIndex)
        {
            if (this.DataGrid.DetailsViewManager.HasDetailsView)
            {
                var rowIndex = this.DataGrid.ResolveToRowIndex(recordIndex);

                var dr = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowIndex);
                RecordEntry record;
                if (this.DataGrid.GridModel.HasGroup)
                    record = this.DataGrid.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                else
                    record = this.DataGrid.View.Records[recordIndex];
                if (dr != null)
                {
                    dr.IsExpanded = false;
                    if (record == null)
                        return;
                    foreach (var viewDefinition in this.DataGrid.DetailsViewDefinition)
                    {
                        if (viewDefinition is GridViewDefinition)
                        {
                            if (record.ChildViews!=null && record.ChildViews.ContainsKey(viewDefinition.RelationalColumn))
                                record.ChildViews[viewDefinition.RelationalColumn].IsNestedLevelExpanded = false;
                        }
                    }
                }
                else
                {
                    if (record == null) return;
                    if (!record.IsExpanded) return;
                    record.IsExpanded = false;
                    var actualRowIndex = rowIndex;
                    foreach (var viewDefinition in this.DataGrid.DetailsViewDefinition)
                    {
                        actualRowIndex++;
                        if (viewDefinition is GridViewDefinition)
                        {
                            int repeatValueCount;
                            if (record.ChildViews != null && record.ChildViews.ContainsKey(viewDefinition.RelationalColumn))
                                record.ChildViews[viewDefinition.RelationalColumn].IsNestedLevelExpanded = false;
                            var isHidden = this.DataGrid.VisualContainer.RowHeights.GetHidden(actualRowIndex, out repeatValueCount);
                            if (!isHidden)
                            {
                                this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, true);
                                this.DataGrid.VisualContainer.RowHeights.SetNestedLines(actualRowIndex, null);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Methods
        internal RecordEntry GetDetailsViewRecord(int rowIndex)
        {
            RecordEntry record = null;
            if (this.DataGrid.GridModel.HasGroup)
            {
                var recordIndex = this.DataGrid.ResolveToGroupRecordIndexForDetailsView(rowIndex);
                record = this.DataGrid.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
            }
            else
            {
                var recordIndex = this.DataGrid.ResolveToRecordIndex(rowIndex);
                record = this.DataGrid.View.Records[recordIndex];
            }
            return record;
        }

        internal DetailsViewDataRow CreateDetailsViewDataRow(int rowIndex)
        {
            return CreateDetailsViewDataRow(rowIndex, GetDetailsViewRecord(rowIndex), null);
        }

        internal void UpdateDetailsViewDataRow(IEnumerable<DataRowBase> rows, int rowIndex)
        {
            var detailsViewIndex = this.DataGrid.GetOrderForDetailsViewBasedOnIndex(rowIndex - 1);
            var detailsView = this.DataGrid.DetailsViewDefinition[detailsViewIndex];
            RecordEntry record = null;
            record = GetDetailsViewRecord(rowIndex);
            record.IsExpanded = true;
            CreateOrUpdateDetailsViewDataRow(rows, rowIndex, record, detailsView, detailsViewIndex, null);
        }

        internal void OnDetailsViewExpanderStateChanged(RowColumnIndex rowColumnIndex, bool isExpanded)
        {
            if (rowColumnIndex.RowIndex <= 0) return;
            var recordIndex = DataGrid.ResolveToRecordIndex(rowColumnIndex.RowIndex);
            var node = DataGrid.GridModel.HasGroup ? DataGrid.View.TopLevelGroup.DisplayElements[recordIndex] : DataGrid.View.Records[recordIndex];
            RecordEntry record = null;
            if (node is RecordEntry)
                record = node as RecordEntry;
            if (record == null) return;
            if (record.IsExpanded == isExpanded) return;
            var updateDataRow = new List<int>();
            if (isExpanded)
            {
                var detailsViewItemsSource = new Dictionary<string, IEnumerable>();
                foreach (var detailsView in this.DataGrid.DetailsViewDefinition)
                {
                    IEnumerable itemsSource = null;
                    if (record.ChildViews != null && record.ChildViews.ContainsKey(detailsView.RelationalColumn) && record.ChildViews[detailsView.RelationalColumn].View != null)
                        itemsSource = record.ChildViews[detailsView.RelationalColumn].View.SourceCollection;
                    if (itemsSource == null)
                        itemsSource = GetChildSource(record, detailsView.RelationalColumn);
                    detailsViewItemsSource.Add(detailsView.RelationalColumn, itemsSource);
                }
                var expandingEventArg = new GridDetailsViewExpandingEventArgs(this.DataGrid) { Record = record.Data, DetailsViewItemsSource = detailsViewItemsSource };
                if (!this.DataGrid.RaiseDetailsViewExpanding(expandingEventArg))
                {
                    ChangeDetailsViewExpanderState(rowColumnIndex.RowIndex, false);
                    return;
                }
                updateDataRow = ExpandDetailsView(rowColumnIndex.RowIndex, record, detailsViewItemsSource);
                var expandedEventArg = new GridDetailsViewExpandedEventArgs(this.DataGrid) { Record = record.Data, DetailsViewItemsSource = detailsViewItemsSource };
                this.DataGrid.RaiseDetailsViewExpanded(expandedEventArg);
            }
            else
            {
                var collapsingEventArgs = new GridDetailsViewCollapsingEventArgs(this.DataGrid) { Record = record.Data };
                if (!this.DataGrid.RaiseDetailsViewCollapsing(collapsingEventArgs))
                {
                    ChangeDetailsViewExpanderState(rowColumnIndex.RowIndex, true);
                    return;
                }
                CollapseDetailsView(rowColumnIndex.RowIndex, record);
                var collapsedEventArg = new GridDetailsViewCollapsedEventArgs(this.DataGrid) { Record = record.Data };
                this.DataGrid.RaiseDetailsViewCollapsed(collapsedEventArg);
            }
            this.RefreshDataRow(updateDataRow);
            this.RefreshParentDataGrid(this.DataGrid);
        }


        internal void ResetExpandedDetailsView()
        {
            if (!HasDetailsView) return;
            this.DataGrid.RowGenerator.Items.ForEach(row =>
                {
                    if (row is DetailsViewDataRow)
                        (row as DetailsViewDataRow).CatchedRowIndex = -1;
                });
            var lineSizeCollection = this.DataGrid.VisualContainer.RowHeights as LineSizeCollection;
            if (lineSizeCollection == null) return;
            lineSizeCollection.SuspendUpdates();
            lineSizeCollection.ResetNestedLines();
            lineSizeCollection.ResumeUpdates();
        }

        internal Rect SubtractDetailsViewPadding(Rect cellRect)
        {
            var thickness = this.DataGrid.DetailsViewPadding;
            if (cellRect.IsEmpty || cellRect.Width <= thickness.Left + thickness.Right || cellRect.Height <= thickness.Top + (2 * thickness.Bottom))
                return cellRect;

            cellRect.Height -= (thickness.Bottom + thickness.Top);
            cellRect.Y += thickness.Top;
            cellRect.Width -= (thickness.Right + thickness.Left);
            cellRect.X += thickness.Left;

            return cellRect;
        }

        internal void CollapsingDetailsViewDataRow(DetailsViewDataRow dataRow)
        {
            if (!AllowDisposeCollectionView) return;
            var rowIndex = dataRow.CatchedRowIndex;
            var recordIndex = this.DataGrid.ResolveToRecordIndex(rowIndex);
            var record = this.DataGrid.GridModel.HasGroup ? this.DataGrid.View.TopLevelGroup.DisplayElements[recordIndex] : this.DataGrid.View.Records[recordIndex];
            if (record is RecordEntry)
            {
                var nestedRecord = record as RecordEntry;
                if (nestedRecord.ChildViews != null)
                {
                    foreach (var item in nestedRecord.ChildViews)
                        item.Value.Dispose();
                    nestedRecord.ChildViews.Clear();
                }
            }
        }

        internal bool[] GetHiddenPattern()
        {
            var hiddenPattern = new List<bool> { false };
            this.DataGrid.DetailsViewDefinition.ForEach(r => hiddenPattern.Add(true));
            return hiddenPattern.ToArray();
        }

        internal static void AdjustParentsWidth(SfDataGrid parentDataGrid, SfDataGrid detailsViewDataGrid)
        {
            var detailsViewColumnWidth = detailsViewDataGrid.VisualContainer.ColumnWidths.TotalExtent +
                                    (parentDataGrid.DetailsViewPadding.Left + parentDataGrid.DetailsViewPadding.Right);
            var indentColumnCount = parentDataGrid.View.GroupDescriptions.Count +
                                    (parentDataGrid.DetailsViewManager.HasDetailsView ? 1 : 0);
            var ownerColumnWith = parentDataGrid.VisualContainer.ColumnWidths.TotalExtent -
                              (indentColumnCount * parentDataGrid.GridModel.IndentColumnSize);
            var lastIndex = parentDataGrid.VisualContainer.ScrollColumns.LineCount - 1;
            var lastColumn = parentDataGrid.Columns.LastOrDefault();
            if (lastColumn != null && !double.IsNaN(lastColumn.ExtendedWidth))
                ownerColumnWith -= lastColumn.ExtendedWidth;
            if (!(ownerColumnWith < detailsViewColumnWidth))
            {
                if (lastColumn != null && !double.IsNaN(lastColumn.ExtendedWidth))
                {
                    var lastDetailsViewColumn = detailsViewDataGrid.Columns.LastOrDefault();
                    if (lastDetailsViewColumn != null && !double.IsNaN(lastDetailsViewColumn.ExtendedWidth))
                        return;
                    parentDataGrid.VisualContainer.ColumnWidths[lastIndex] -= lastColumn.ExtendedWidth;
                    lastColumn.ActualWidth = parentDataGrid.VisualContainer.ColumnWidths[lastIndex];
                    lastColumn.ExtendedWidth = double.NaN;
                }
                return;
            }
            var extendedWidth = (detailsViewColumnWidth - ownerColumnWith) +
                                (parentDataGrid.DetailsViewPadding.Left + parentDataGrid.DetailsViewPadding.Right);
            var newExtendedWidth = extendedWidth;
            if (lastColumn != null && !double.IsNaN(lastColumn.ExtendedWidth))
                newExtendedWidth = extendedWidth - lastColumn.ExtendedWidth;
            parentDataGrid.VisualContainer.ColumnWidths[lastIndex] += newExtendedWidth;
            lastColumn.ExtendedWidth = extendedWidth;
            lastColumn.ActualWidth = parentDataGrid.VisualContainer.ColumnWidths[lastIndex];
        }

        internal static void ScrollInViewAllDetailsViewParent(SfDataGrid dataGrid, VisibleLineInfo lineInfo)
        {
            if (lineInfo == null || !(dataGrid is DetailsViewDataGrid) || dataGrid.NotifyListener == null) return;
            var index = ScrollDetailsViewParent(dataGrid, dataGrid.VisualContainer.HorizontalOffset + lineInfo.ClippedCorner);
            var parentGrid = dataGrid.NotifyListener.GetParentDataGrid();
            var line = parentGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(index);
            if (line != null && parentGrid is DetailsViewDataGrid && parentGrid.NotifyListener != null)
                ScrollInViewAllDetailsViewParent(parentGrid, line);
        }

        internal void RefreshParentDataGrid(SfDataGrid datagrid)
        {
            if (!(datagrid is DetailsViewDataGrid) || datagrid.NotifyListener == null) return;
            var parentGrid = datagrid.NotifyListener.GetParentDataGrid();
            RefreshParentDataGrid(parentGrid);
            parentGrid.VisualContainer.ScrollRows.MarkDirty();
            parentGrid.VisualContainer.InvalidateMeasure();
        }

        internal void BringIntoView(int parentRowIndex)
        {
            UpdateDetailsViewDataRow(this.DataGrid.RowGenerator.Items, parentRowIndex);
            var detailsViewRow = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == parentRowIndex);
            if (detailsViewRow is DetailsViewDataRow)
            {
                var detailsViewGrid = (detailsViewRow as DetailsViewDataRow).DetailsViewDataGrid;
                var height = detailsViewGrid.VisualContainer.ScrollRows.HeaderExtent +
                             detailsViewGrid.VisualContainer.ScrollRows.FooterExtent +
                             detailsViewGrid.VisualContainer.ScrollRows.DefaultLineSize;
                detailsViewGrid.VisualContainer.ScrollRows.RenderSize = height;
            }
        }

        #endregion

        #region private Methods

        private void CollapseDetailsView(int rowIndex, RecordEntry record)
        {
            var actualRowIdx = rowIndex;
            record.IsExpanded = false;
            foreach (var detailsView in this.DataGrid.DetailsViewDefinition)
            {
                actualRowIdx++;
                if (record.ChildViews.ContainsKey(detailsView.RelationalColumn))
                   record.ChildViews[detailsView.RelationalColumn].IsNestedLevelExpanded = false;             
                this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIdx, actualRowIdx, true);
                this.DataGrid.VisualContainer.RowHeights.SetNestedLines(actualRowIdx, null);
            }
        }

        private List<int> ExpandDetailsView(int rowIndex, RecordEntry record, Dictionary<string, IEnumerable> detailsViewItemsSource)
        {
            var actualRowIndex = rowIndex;
            record.IsExpanded = true;
            var count = 0;
            var updateDataRow = new List<int>();
            foreach (var detailsView in this.DataGrid.DetailsViewDefinition)
            {
                actualRowIndex++;
                var gridViewDefinition = detailsView as GridViewDefinition;
                IEnumerable itemsSource;
                if (detailsViewItemsSource.TryGetValue(gridViewDefinition.RelationalColumn, out itemsSource) && itemsSource != null)
                    CreateOrUpdateDetailsViewDataRow(this.DataGrid.RowGenerator.Items, actualRowIndex, record, detailsView, count, itemsSource);
                else if (!this.DataGrid.HideEmptyGridViewDefinition)
                    CreateOrUpdateDetailsViewDataRow(this.DataGrid.RowGenerator.Items, actualRowIndex, record, detailsView, count, null);
                updateDataRow.Add(actualRowIndex);
                var view = record.ChildViews[gridViewDefinition.RelationalColumn];
                if (view != null && !this.DataGrid.HideEmptyGridViewDefinition)
                    this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, false);
                else if (view != null && view.View.SourceCollection.AsQueryable().Count() > 0)
                    this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, false);
                else
                    this.DataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, true);
                count++;
            }

            if (this.DataGrid.HideEmptyGridViewDefinition)
            {
                if (record.ChildViews.Count == this.DataGrid.DetailsViewDefinition.Count)
                {
                    var isExpanded = record.ChildViews.Any(view => view.Value.View.SourceCollection.AsQueryable().Count() > 0);
                    if (!isExpanded)
                    {
                        var recordIndex = this.DataGrid.ResolveToRecordIndex(rowIndex);
                        this.DataGrid.DetailsViewManager.CollapseDetailsViewAt(recordIndex);
                    }
                }
            }


            return updateDataRow;
        }

        private DetailsViewDataRow CreateDetailsViewDataRow(int rowIndex, RecordEntry record, IEnumerable itemsSource)
        {
            var detailsViewDataRow = new DetailsViewDataRow();
            detailsViewDataRow.DataGrid = this.DataGrid;
            detailsViewDataRow.RowRegion = RowRegion.Body;
            detailsViewDataRow.RowLevel = 0;

            var detailsViewIndex = this.DataGrid.GetOrderForDetailsViewBasedOnIndex(rowIndex - 1);
            var detailsView = this.DataGrid.DetailsViewDefinition[detailsViewIndex];

            InitializeDetailsViewDataRow(rowIndex, record, detailsViewDataRow, detailsViewIndex, detailsView, itemsSource);

            detailsViewDataRow.InitializeDataRow(this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLines());
            bool isVisualStateApplied = CheckVisualState(rowIndex, record);
            if (!isVisualStateApplied)
                detailsViewDataRow.ApplyContentVisualState(detailsViewIndex ==
                                                                    this.DataGrid.DetailsViewDefinition.Count - 1
                                                                        ? "LastCell"
                                                                        : "NormalCell");
            return detailsViewDataRow;
        }


        private bool CheckVisualState(int rowIndex, RecordEntry record, DetailsViewDataRow detailsViewDataRow = null)
        {
            int count = this.DataGrid.DetailsViewDefinition.Count;
            var detaildefinition = this.DataGrid.DetailsViewDefinition[count - 1];
            if (record.ChildViews.Count == count && this.DataGrid.HideEmptyGridViewDefinition)
            {
                if (record.ChildViews[detaildefinition.RelationalColumn].View != null && record.ChildViews[detaildefinition.RelationalColumn].View.Records.Count == 0)
                {
                    for (int i = rowIndex - 1; i > rowIndex - count; i--)
                    {
                        var detailrow = this.DataGrid.RowGenerator.Items.OfType<DetailsViewDataRow>().FirstOrDefault(row => row.RowIndex == i);
                        if (detailrow == null && detailsViewDataRow.DetailsViewDataGrid.View.SourceCollection.AsQueryable().Count() > 0)
                            detailrow = detailsViewDataRow;
                        if (detailrow != null && detailrow.DetailsViewDataGrid.View.SourceCollection.AsQueryable().Count() > 0)
                        {
                            detailrow.ApplyContentVisualState("LastCell");
                            return true;
                        }
                    }
                }

            }
            return false;
        }
        private void CreateOrUpdateDetailsViewDataRow(IEnumerable<DataRowBase> rows, int rowIndex, RecordEntry record, ViewDefinition detailsView, int detailsViewIndex, IEnumerable itemsSource)
        {
            var dr = rows.FirstOrDefault(r => (r is DetailsViewDataRow) && (r as DetailsViewDataRow).CatchedRowIndex == rowIndex);
            if (dr != null)
            {
                InitializeDetailsViewDataRow(rowIndex, record, dr as DetailsViewDataRow, detailsViewIndex, detailsView, itemsSource);
                EnsureProperties((dr as DetailsViewDataRow).DetailsViewDataGrid);
                return;
            }

            if (detailsView is GridViewDefinition)
            {
                var gridViewDefinition = detailsView as GridViewDefinition;
                if (gridViewDefinition.NotifyListener != null)
                {
                    dr = rows.FirstOrDefault(r => !r.IsEnsured && (r is DetailsViewDataRow) && (r as DetailsViewDataRow).DetailsViewDataGrid != this.DataGrid.SelectedDetailsViewGrid && gridViewDefinition.NotifyListener.ClonedDataGrid.Contains((r as DetailsViewDataRow).DetailsViewDataGrid));
                    if (dr is DetailsViewDataRow)
                    {
                        var detailsViewRow = dr as DetailsViewDataRow;
                        int repeatValueCount;
                        var isHidden = this.DataGrid.VisualContainer.RowHeights.GetHidden(rowIndex, out repeatValueCount);
                        if (!isHidden)
                        {
                            var lines = this.DataGrid.VisualContainer.RowHeights.GetNestedLines(rowIndex) as LineSizeCollection;
                            detailsViewRow.DetailsViewDataGrid.VisualContainer.UpdateRowInfo(lines, detailsViewRow.DetailsViewDataGrid.RowHeight);
                        }
                        else
                        {
                            detailsViewRow.DetailsViewDataGrid.VisualContainer.UpdateRowInfo(null, detailsViewRow.DetailsViewDataGrid.RowHeight);
                        }
                        InitializeDetailsViewDataRow(rowIndex, record, dr as DetailsViewDataRow, detailsViewIndex, detailsView, itemsSource);
                        EnsureProperties((dr as DetailsViewDataRow).DetailsViewDataGrid);
                        return;
                    }
                }
            }

            dr = CreateDetailsViewDataRow(rowIndex, record, itemsSource);
            var detailsViewDataGrid = (dr as DetailsViewDataRow).DetailsViewDataGrid;
            if(detailsViewDataGrid != null && detailsViewDataGrid.AutoGenerateColumns && detailsViewDataGrid.AutoGenerateColumnsMode == AutoGenerateColumnsMode.ResetAll)
                EnsureProperties(detailsViewDataGrid);
            this.DataGrid.RowGenerator.Items.Add(dr);

        }

        private void InitializeDetailsViewDataRow(int rowIndex, RecordEntry record, DetailsViewDataRow detailsViewDataRow, int detailsViewIndex, ViewDefinition detailsView, IEnumerable itemsSource)
        {
            detailsViewDataRow.RowIndex = rowIndex;
            detailsViewDataRow.CatchedRowIndex = rowIndex;
            detailsViewDataRow.IsExpanded = true;

            if (detailsView is GridViewDefinition)
            {
                var gridViewDefinition = detailsView as GridViewDefinition;
                if (gridViewDefinition.NotifyListener == null)
                    gridViewDefinition.NotifyListener = new DetailsViewNotifyListener(gridViewDefinition.DataGrid, this.DataGrid);
                int repeatValueCount;
                var isHidden = this.DataGrid.VisualContainer.RowHeights.GetHidden(rowIndex, out repeatValueCount);
                if (detailsViewDataRow.DetailsViewDataGrid == null)
                {
                    var grid = gridViewDefinition.NotifyListener.CopyPropertiesFromRootGrid(gridViewDefinition.DataGrid);
                    if (!isHidden)
                    {
                        var lines = this.DataGrid.VisualContainer.RowHeights.GetNestedLines(rowIndex) as LineSizeCollection;
                        grid.VisualContainer.UpdateRowInfo(lines, grid.RowHeight);
                    }
                    //grid.VisualContainer.ScrollOwner = this.DataGrid.VisualContainer.ScrollOwner;
                    detailsViewDataRow.DetailsViewDataGrid = grid;
                }
                if (record.ChildViews != null && record.ChildViews.ContainsKey(gridViewDefinition.RelationalColumn) && record.ChildViews[gridViewDefinition.RelationalColumn].View != null)
                {
                    var recordView = record.ChildViews[gridViewDefinition.RelationalColumn].View;
                    if (detailsViewDataRow.DetailsViewDataGrid.View != recordView)
                    {
                        detailsViewDataRow.RowData = recordView.SourceCollection;
                        detailsViewDataRow.DetailsViewDataGrid.ItemsSource = recordView.SourceCollection;
                    }
                    if (record.ChildViews[gridViewDefinition.RelationalColumn].IsNestedLevelExpanded)
                        detailsViewDataRow.DetailsViewDataGrid.ExpandAllDetailsView();
                    this.DataGrid.VisualContainer.RowHeights.SetNestedLines(rowIndex, detailsViewDataRow.DetailsViewDataGrid.VisualContainer.RowHeights);
                }
                else
                {
                    if (itemsSource == null)
                        itemsSource = GetChildSource(record, gridViewDefinition.RelationalColumn);
                    if (detailsViewDataRow.DetailsViewDataGrid.ItemsSource == itemsSource)
                        detailsViewDataRow.DetailsViewDataGrid.ItemsSource = null;
                    detailsViewDataRow.RowData = itemsSource;
                    detailsViewDataRow.DetailsViewDataGrid.ItemsSource = detailsViewDataRow.RowData;
                    if (!isHidden) detailsViewDataRow.DetailsViewDataGrid.ExpandAllDetailsView();
                    var isNestedExpanded = false;
                    if (record.ChildViews.ContainsKey(gridViewDefinition.RelationalColumn))
                        isNestedExpanded = record.ChildViews[gridViewDefinition.RelationalColumn].IsNestedLevelExpanded;
                    record.ChildViews.Remove(gridViewDefinition.RelationalColumn);
                    record.PopulateChildView(detailsViewDataRow.DetailsViewDataGrid.View, detailsViewIndex, gridViewDefinition.RelationalColumn, isNestedExpanded);
                 
                    this.DataGrid.VisualContainer.RowHeights.SetNestedLines(rowIndex, detailsViewDataRow.DetailsViewDataGrid.VisualContainer.RowHeights);
                }
                if (detailsViewDataRow.WholeRowElement != null)
                {
                    bool isVisualStateApplied = CheckVisualState(rowIndex, record, detailsViewDataRow);
                    if (!isVisualStateApplied)
                        detailsViewDataRow.ApplyContentVisualState(detailsViewIndex ==
                                                                    this.DataGrid.DetailsViewDefinition.Count - 1
                                                                        ? "LastCell"
                                                                        : "NormalCell");

                }
                if (detailsViewDataRow.DetailsViewDataGrid.View != null && detailsViewDataRow.DetailsViewDataGrid.Columns.Any(column => column.FilterPredicates.Any()))
                    detailsViewDataRow.DetailsViewDataGrid.GridModel.ApplyFilter();
                AdjustParentsWidth(this.DataGrid, detailsViewDataRow.DetailsViewDataGrid);
            }
        }

        private void EnsureProperties(DetailsViewDataGrid detailsViewDataGrid)
        {
            (detailsViewDataGrid as IDetailsViewNotifier).SuspendNotifyListener();
            var sourceDataGrid = detailsViewDataGrid.NotifyListener.RootDataGrid;
            var isAdded = false;
            foreach (var sourceColumn in sourceDataGrid.Columns)
            {
                var detailsViewColumn = detailsViewDataGrid.Columns.FirstOrDefault(column => column.MappingName == sourceColumn.MappingName);
                if (detailsViewColumn != null)
                {
                    if (detailsViewColumn.GetType() == sourceColumn.GetType())
                    {
                        CloneHelper.CloneProperties(sourceColumn, detailsViewColumn, typeof(GridColumn));
                        CloneHelper.CloneCollection(sourceColumn.FilterPredicates, (detailsViewColumn as GridColumn).FilterPredicates, typeof(FilterPredicate));
                    }
                    else
                    {
                        detailsViewDataGrid.Columns.Remove(detailsViewColumn);
                        var destinationItem = CloneHelper.CreateClonedInstance(sourceColumn, typeof(GridColumn));
                        CloneHelper.CloneCollection(sourceColumn.FilterPredicates, (destinationItem as GridColumn).FilterPredicates, typeof(FilterPredicate));
                        detailsViewDataGrid.Columns.Add(destinationItem as GridColumn);
                        isAdded = true;
                    }
                }
                else
                {
                    var destinationItem = CloneHelper.CreateClonedInstance(sourceColumn, typeof(GridColumn));
                    CloneHelper.CloneCollection(sourceColumn.FilterPredicates, (destinationItem as GridColumn).FilterPredicates, typeof(FilterPredicate));
                    detailsViewDataGrid.Columns.Add(destinationItem as GridColumn);
                    isAdded = true;
                }
            }
            CloneHelper.EnsureCollection<SortColumnDescriptions, SortColumnDescription>(sourceDataGrid.SortColumnDescriptions, detailsViewDataGrid.SortColumnDescriptions, (target, source) => target.ColumnName == source.ColumnName);
            CloneHelper.EnsureCollection<GroupColumnDescriptions, GroupColumnDescription>(sourceDataGrid.GroupColumnDescriptions, detailsViewDataGrid.GroupColumnDescriptions, (target, source) => target.ColumnName == source.ColumnName);

            if (detailsViewDataGrid.VisualContainer.RowCount >= detailsViewDataGrid.HeaderLineCount)
            {
                for (var i = 0; i < detailsViewDataGrid.HeaderLineCount; i++)
                    detailsViewDataGrid.VisualContainer.RowHeights[i] = detailsViewDataGrid.HeaderRowHeight;
            }

            if(isAdded)
                AdjustParentsWidth(this.DataGrid, detailsViewDataGrid);

            (detailsViewDataGrid as IDetailsViewNotifier).ResumeNotifyListener();
        }

        private void ExpandNestedLines(object record, LineSizeCollection lines, GridViewDefinition gridViewDefinition,RecordEntry recordEntry)
        {
            var itemsource = GetChildSource(record, gridViewDefinition.RelationalColumn);
            if (recordEntry != null && recordEntry.ChildViews != null &&
                recordEntry.ChildViews.ContainsKey(gridViewDefinition.RelationalColumn) && recordEntry.ChildViews[gridViewDefinition.RelationalColumn].View!=null)
            {
                itemsource = recordEntry.ChildViews[gridViewDefinition.RelationalColumn].View.Records;
            }
            if (gridViewDefinition.DataGrid.DetailsViewDefinition != null &&
                gridViewDefinition.DataGrid.DetailsViewDefinition.Any())
            {
                var rowIndex = 1;
                foreach (var item in itemsource)
                {
                    var actualRowIndex = rowIndex;
                    foreach (var nestedViewDefinition in gridViewDefinition.DataGrid.DetailsViewDefinition)
                    {
                        if (nestedViewDefinition is GridViewDefinition)
                        {
                            var nestedgridViewDefinition = nestedViewDefinition as GridViewDefinition;
                            actualRowIndex++;

                            int repeatValueCount;
                            var isHidden = lines.GetHidden(actualRowIndex, out repeatValueCount);
                            LineSizeCollection nestedLines = null;
                            if (!isHidden)
                                nestedLines = lines.GetNestedLines(actualRowIndex) as LineSizeCollection;
                            if (nestedLines == null)
                            {
                                var detailsViewLinesitemsource = GetChildSource((item is RecordEntry)?(item as RecordEntry).Data:item, nestedgridViewDefinition.RelationalColumn);
                                var count = detailsViewLinesitemsource != null ? detailsViewLinesitemsource.AsQueryable().Count() * (nestedgridViewDefinition.DataGrid.DetailsViewDefinition.Count + 1) : 0;
                                nestedLines = new LineSizeCollection { LineCount = count + 1, DefaultLineSize = this.DataGrid.RowHeight };
                            }
                            ExpandNestedLines((item is RecordEntry) ? (item as RecordEntry).Data : item, nestedLines, nestedgridViewDefinition, (item is RecordEntry) ? item as RecordEntry:null);
                            lines.SetNestedLines(actualRowIndex, nestedLines);
                            lines.SetHidden(actualRowIndex, actualRowIndex, false);
                        }
                    }
                    rowIndex += gridViewDefinition.DataGrid.DetailsViewDefinition.Count + 1;
                }
            }
        }

        private static void CollapeseNestedLines(SfDataGrid dataGrid, GridViewDefinition gridViewDefinition)
        {
            var lines = dataGrid.VisualContainer.RowHeights as LineSizeCollection;
            lines.SuspendUpdates();
            var lastRowIndex = lines.LineCount - gridViewDefinition.DataGrid.DetailsViewDefinition.Count;
            for (var rowIndex = dataGrid.HeaderLineCount; rowIndex < lastRowIndex; rowIndex++)
            {
                var record = dataGrid.View.Records[dataGrid.ResolveToRecordIndex(rowIndex)];
                record.IsExpanded = false;
                var actualRowIndex = rowIndex;
                foreach (var viewDefinition in gridViewDefinition.DataGrid.DetailsViewDefinition)
                {
                    if (!(viewDefinition is GridViewDefinition)) continue;
                    var nestedGridViewDefinition = viewDefinition as GridViewDefinition;
                    actualRowIndex++;
                    int repeatValueCount;
                    var isHidden = dataGrid.VisualContainer.RowHeights.GetHidden(actualRowIndex, out repeatValueCount);
                    if (isHidden) continue;
                    var dr = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == actualRowIndex) as DetailsViewDataRow;
                    if (dr != null) CollapeseNestedLines(dr.DetailsViewDataGrid, nestedGridViewDefinition);
                    dataGrid.VisualContainer.RowHeights.SetHidden(actualRowIndex, actualRowIndex, true);
                    dataGrid.VisualContainer.RowHeights.SetNestedLines(actualRowIndex, null);
                }
                rowIndex += gridViewDefinition.DataGrid.DetailsViewDefinition.Count;
            }
            lines.ResumeUpdates();
            dataGrid.RowGenerator.Items.ForEach(row =>
            {
                if (row is DetailsViewDataRow)
                    row.RowIndex = -1;
            });
        }

        private void ChangeDetailsViewExpanderState(int rowIndex, bool isExpanded)
        {
            var dr = this.DataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowIndex);
            if (dr == null) return;
            var dc = dr.VisibleColumns.FirstOrDefault(column => column.IsExpanderColumn);
            if (dc == null) return;
            var expander = dc.ColumnElement as GridDetailsViewExpanderCell;
            if (expander == null) return;
            expander.SuspendChangedAction = true;
            dr.IsExpanded = isExpanded;
            expander.SuspendChangedAction = false;
        }

        private void RefreshDataRow(ICollection<int> skipRowIndex)
        {
            if (this.DataGrid.VisualContainer == null) return;
            this.DataGrid.RowGenerator.Items.ForEach(row =>
            {
                if (skipRowIndex.Contains(row.RowIndex) && row is DetailsViewDataRow) return;
                if (row.RowRegion == RowRegion.Body || row.RowRegion == RowRegion.Footer)
                    row.RowIndex = -1;
            });
            this.DataGrid.VisualContainer.InvalidateMeasureInfo();
        }

        private IEnumerable GetChildSource(RecordEntry record, string relationName)
        {
            if (string.IsNullOrEmpty(relationName))
                throw new InvalidOperationException("RelationalColumn cannot be null, must set the RelationalColumn.");
            var provider = this.DataGrid.View.GetPropertyAccessProvider();
            if (provider != null)
            {
                var childSource = provider.GetValue(record.Data, relationName) as IEnumerable;
                return childSource;
            }
            return null;
        }

        private static IEnumerable GetChildSource(object record, string relationName)
        {
            if (string.IsNullOrEmpty(relationName))
                throw new InvalidOperationException("RelationalColumn cannot be null, must set the RelationalColumn.");
            var propertyInfo = record.GetType().GetProperty(relationName);
            if (propertyInfo != null)
            {
                var childSource = propertyInfo.GetValue(record, null) as IEnumerable;
                return childSource;
            }
            return null;
        }

        private static int ScrollDetailsViewParent(SfDataGrid dataGrid, double point)
        {
            var parentGrid = dataGrid.NotifyListener.GetParentDataGrid();
            var lines = parentGrid.VisualContainer.ScrollColumns.GetVisibleLines();
            var startingPoint = lines[lines.firstBodyVisibleIndex].ClippedCorner + parentGrid.VisualContainer.HorizontalOffset;
            var lastpoint = lines[lines.LastBodyVisibleIndex].ClippedCorner;
            if (point < lastpoint && point > startingPoint)
                return -1;
            var index = 0;
            if (point > lastpoint)
            {
                index = lines[lines.LastBodyVisibleIndex].LineIndex;
                do
                {
                    int repeatValueConut;
                    index += 1;
                    if (index >= parentGrid.VisualContainer.ScrollColumns.LineCount)
                    {
                        index -= 1;
                        break;
                    }
                    lastpoint += parentGrid.VisualContainer.ColumnWidths.GetSize(index, out repeatValueConut);
                } while (point > lastpoint);
            }
            else if (point < startingPoint)
            {
                index = lines[lines.firstBodyVisibleIndex].LineIndex;
                do
                {
                    int repeatValueCount;
                    index -= 1;
                    if (index <= 0)
                    {
                        index += 1;
                        break;
                    }
                    startingPoint -= parentGrid.VisualContainer.ColumnWidths.GetSize(index, out repeatValueCount);
                } while (point < startingPoint);
            }
            parentGrid.VisualContainer.ScrollColumns.ScrollInView(index);
            parentGrid.VisualContainer.InvalidateMeasure();
            return index;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            DataGrid = null;
        }

        #endregion
    }
}
