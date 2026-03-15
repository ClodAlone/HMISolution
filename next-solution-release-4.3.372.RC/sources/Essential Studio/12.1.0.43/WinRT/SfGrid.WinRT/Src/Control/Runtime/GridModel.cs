#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections;
using System.Reflection;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
#if WinRT
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
#if !WP
using PagedCollectionView = Syncfusion.Data.PagedCollectionView;
#endif
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using System.Collections.ObjectModel;

#endif

    [ClassReference(IsReviewed = false)]
    public class GridModel : IDisposable
    {
        #region Fields

        internal SfDataGrid dataGrid;
        private bool isSuspended = false;
        private bool isGroupDescriptionChanged = false;
        private bool isGroupColumnChanged = false;
#if WinRT
        internal double IndentColumnSize = 45d;
#else
        internal double IndentColumnSize = 24d;
#endif
        private bool filterSuspend = false;

#if SILVERLIGHT || WP7
        internal bool isGroupDescriptionMoved = false;
#endif
#if !WP
        internal GridAddNewRowController addNewRowController;
#endif
        #endregion

        #region Properties

        private ICollectionViewAdv View
        {
            get { return this.dataGrid.View; }
        }

        public bool IsInSort { get; internal set; }
        private bool isSortDescriptionChanged { get; set; }
        private bool isSortColumnChanged { get; set; }

        internal bool HasGroup
        {
            get
            {
                if (this.View != null)
                    return this.View.GroupDescriptions.Count > 0;
                return false;
            }
        }

        public bool SuspendForSortDescriptions { get; set; }

        internal bool FilterSuspend
        {
            get { return filterSuspend; }
            set { filterSuspend = value; }
        }

        #endregion

        #region Ctor

        public GridModel(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
#if !WP
            addNewRowController = new GridAddNewRowController(dataGrid);
#endif
        }

        #endregion

        #region WireEvents

        internal void WireEvents()
        {
            if (View != null)
            {
                var sortNotifyCollectionChanged = this.dataGrid.View.SortDescriptions as INotifyCollectionChanged;
                sortNotifyCollectionChanged.CollectionChanged += OnSortDescriptionsChanged;
                (View as CollectionViewAdv).CollectionChanged += OnRecordCollectionChanged;
                (View as CollectionViewAdv).TopLevelGroupCollectionChanged += OnTopLevelGroupCollectionChanged;
                this.View.GroupDescriptions.CollectionChanged += OnGroupDescriptionChanged;
                this.View.RecordPropertyChanged += OnRecordPropertyChanged;
                View.SourceCollectionChanged += OnSourceCollectionChanged;
                View.PropertyChanged += OnViewPropertyChanged;
#if !WP
                if (View is Syncfusion.Data.PagedCollectionView)
                {
                    (View as Syncfusion.Data.PagedCollectionView).PageChanged += OnPageChanged;
                }
#endif
                View.CurrentChanged += OnViewCurrentChanged;
            }
            if (dataGrid != null)
            {
                this.dataGrid.SortColumnDescriptions.CollectionChanged += OnSortColumnsChanged;
                this.dataGrid.GroupColumnDescriptions.CollectionChanged += OnGroupColumnDescriptionsChanged;
                this.dataGrid.GroupSummaryRows.CollectionChanged += OnSummaryRowsChanged;
                this.dataGrid.TableSummaryRows.CollectionChanged += OnTableSummaryRowsChanged;
                this.dataGrid.SortComparers.CollectionChanged += SortComparers_CollectionChanged;
                this.dataGrid.Columns.ForEach(x => WireColumnDescriptor(x));
                this.InitializeGridTableSummaryRow();
            }
        }

        /// <summary>
        /// Method which helps to Syncronnize the CurrentItem between view and DataGrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if !WinRT
        void OnViewCurrentChanged(object sender, EventArgs e)
#elif WinRT
        void OnViewCurrentChanged(object sender, object e)
#endif
        {
            if (this.View.CurrentItem != null)
            {
                var record = this.View.CurrentItem as RecordEntry;
                if (record != null)
                    this.dataGrid.CurrentItem = record.Data;
                else
                    this.dataGrid.CurrentItem = null;
            }
            else
                this.dataGrid.CurrentItem = null;
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemProperties")
            {
                if (this.dataGrid.AutoGenerateColumns && this.dataGrid.AutoGenerateColumnsMode != AutoGenerateColumnsMode.None && this.dataGrid.VisualContainer != null)
                {
                    this.dataGrid.GenerateGridColumns();
                    if (this.dataGrid.Columns.Count != this.dataGrid.VisualContainer.ColumnCount)
                        this.dataGrid.UpdateColumnCount(false);
                    UpdateHeaderCells();
                }
            }
        }

        void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            this.ResetEditItem(args);
            this.dataGrid.SelectionController.HandleCollectionChanged(args, CollectionChangedReason.SourceCollectionChanged);
        }

        private void ResetEditItem(NotifyCollectionChangedEventArgs args)
        {
            if (this.View.IsEditingItem)
            {
                if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    if (args.OldItems.Contains(View.CurrentEditItem))
                        this.EndEdit();
                }
#if !SILVERLIGHT && !WP
                else if (args.Action == NotifyCollectionChangedAction.Move)
                {
                    if (args.NewItems[0] == View.CurrentEditItem)
                        this.EndEdit();
                }
#endif
                else if (args.Action == NotifyCollectionChangedAction.Replace)
                {
                    if (args.NewItems.Contains(View.CurrentEditItem))
                        this.EndEdit();
                }
                else if (args.Action == NotifyCollectionChangedAction.Reset)
                    this.EndEdit();
            }
        }

        internal void EndEdit()
        {
#if !WP
            this.dataGrid.Validations.ResetValidations(true);
#endif
            //this.dataGrid.VisualContainer.SuspendManipulationScroll = false;
            var collectionview = this.View as CollectionViewAdv;

            if (collectionview == null || !collectionview.IsEditingItem) return;
            var datarow = this.dataGrid.RowGenerator.Items.FirstOrDefault(x => x.IsEditing && x.RowData.Equals(View.CurrentEditItem));

            if (datarow != null)
            {
                var column = datarow.VisibleColumns.FirstOrDefault(col => col.IsEditing);
                if (column != null)
                {
                    column.Renderer.EndEdit(this.dataGrid.SelectionController.CurrentCellManager.CurrentCellIndex, column.ColumnElement,
                                            column.GridColumn, datarow.RowData);
                    column.IsEditing = false;
                }
                datarow.IsEditing = false;
            }
            collectionview.EndEdit();
        }
#if !WP
        private void OnPageChanged(object sender, PageChangedEventArgs args)
        {
            this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Paging, null));
            RefreshView();
        }
#endif
        /// <summary>
        /// Event Hooks whenever the Sort comparer's Collection changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private void SortComparers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.View == null)
                return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var comparer in e.NewItems)
                        this.View.SortComparers.Add(comparer as SortComparer);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var comparer in e.OldItems)
                        this.View.SortComparers.Remove(comparer as SortComparer);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.View.SortComparers.Clear();
                    break;
            }
        }


        /// <summary>
        /// Event Hooks whenever collection changed while sorting is present
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private void OnRecordCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.HasGroup)
            {
                //Refreshing will be handled in OnTopLevelGroupCollectionChanged event
                if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                {
                    this.UpdateTableSummaries();
                    return;
                }
            }
#if !WP
            if (this.dataGrid is DetailsViewDataGrid && this.dataGrid.NotifyListener != null)
            {
                var parent = this.dataGrid.NotifyListener.GetParentDataGrid();
                this.dataGrid.DetailsViewManager.RefreshParentDataGrid(this.dataGrid);
            }
#endif
            this.ResetEditItem(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    {
                        var rowindex = this.dataGrid.ResolveToRowIndex(e.OldStartingIndex);
                        if (this.dataGrid.RowGenerator.Items.Any(row => row.RowIndex == rowindex))
                            UpdateDataRow(rowindex, 1, e.Action);
                        else
                            RefreshView(rowindex, 1, e.Action);
                        break;
                    }
                case NotifyCollectionChangedAction.Add:
                    {
                        var addedrowindex = this.dataGrid.ResolveToRowIndex(e.NewStartingIndex);
                        if (
                            this.dataGrid.RowGenerator.Items.Any(
                                row => row.RowIndex == addedrowindex && !row.IsAddNewRow))
                            UpdateDataRow(addedrowindex, 1, e.Action);
                        else
                            RefreshView(addedrowindex, 1, e.Action);
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.dataGrid.RowGenerator.ForceUpdateBinding = true;
                        RefreshView();
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        //if (!this.HasGroup)
                        //{
                        var replaceIndex = this.dataGrid.ResolveToRowIndex(e.NewStartingIndex);
                        this.UpdateDataRow(replaceIndex);
                        //if (this.dataGrid.RowGenerator.Items.Any(row => row.RowIndex == replaceIndex))
                        //    UpdateDataRow(replaceIndex, true);
                        //else
                        //    RefreshView();
                        //}
                        //else
                        //    RefreshView();
                    }

                    break;
            }

            this.dataGrid.SelectionController.HandleCollectionChanged(e, CollectionChangedReason.RecordCollectionChanged);
        }

        private void OnTopLevelGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !WP
            if (this.dataGrid is DetailsViewDataGrid && this.dataGrid.NotifyListener != null)
            {   
                this.dataGrid.DetailsViewManager.RefreshParentDataGrid(this.dataGrid);
            }
#endif
            this.ResetEditItem(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var rowindex = this.dataGrid.ResolveToRowIndex(e.NewStartingIndex);
                        var count = e.NewItems.Count;
                        var recordStartIndex = 0;
                        var recordCount = 0;
#if !WP
                        if (this.dataGrid.DetailsViewManager.HasDetailsView)
                        {
                            recordCount = e.NewItems.OfType<RecordEntry>().Count();
                            count += recordCount*(this.dataGrid.DetailsViewDefinition.Count);
                            if(e.NewItems.OfType<RecordEntry>().Any())
                            recordStartIndex = rowindex+e.NewItems.IndexOf(e.NewItems.OfType<RecordEntry>().FirstOrDefault());
                        }
#endif
                        RefreshView(rowindex, count, e.Action, recordStartIndex,recordCount);
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        var rowindex = this.dataGrid.ResolveToRowIndex(e.OldStartingIndex);
                        var count = e.OldItems.Count;
#if !WP
                        if (this.dataGrid.DetailsViewManager.HasDetailsView)
                        {
                            var recordCount = e.OldItems.OfType<RecordEntry>().Count();
                            var nestedRecordCount = e.OldItems.OfType<NestedRecordEntry>().Count();
                            var expandedRecordCount = (nestedRecordCount/this.dataGrid.DetailsViewDefinition.Count);
                            count += (recordCount - expandedRecordCount)*this.dataGrid.DetailsViewDefinition.Count;
                        }
#endif
                        RefreshView(rowindex, count, e.Action);
                        break;
                    }
            }
            this.dataGrid.SelectionController.HandleCollectionChanged(e, CollectionChangedReason.RecordCollectionChanged);
        }

        internal void UnWireEvents()
        {
            if (View != null)
            {
                var sortNotifyCollectionChanged = this.dataGrid.View.SortDescriptions as INotifyCollectionChanged;
                sortNotifyCollectionChanged.CollectionChanged -= OnSortDescriptionsChanged;
                (View as CollectionViewAdv).CollectionChanged -= OnRecordCollectionChanged;
                (View as CollectionViewAdv).TopLevelGroupCollectionChanged -= OnTopLevelGroupCollectionChanged;
                this.View.GroupDescriptions.CollectionChanged -= OnGroupDescriptionChanged;
                this.View.RecordPropertyChanged -= OnRecordPropertyChanged;
                View.SourceCollectionChanged -= OnSourceCollectionChanged;
                View.PropertyChanged -= OnViewPropertyChanged;

#if !WP
                if (View is Syncfusion.Data.PagedCollectionView)
                {
                    (View as Syncfusion.Data.PagedCollectionView).PageChanged -= OnPageChanged;
                }
#endif
                View.CurrentChanged -= OnViewCurrentChanged;
            }
            if (dataGrid != null)
            {
                this.dataGrid.SortColumnDescriptions.CollectionChanged -= OnSortColumnsChanged;
                this.dataGrid.GroupColumnDescriptions.CollectionChanged -= OnGroupColumnDescriptionsChanged;
                this.dataGrid.GroupSummaryRows.CollectionChanged -= OnSummaryRowsChanged;
                this.dataGrid.TableSummaryRows.CollectionChanged -= OnTableSummaryRowsChanged;
                this.dataGrid.SortComparers.CollectionChanged -= SortComparers_CollectionChanged;
            }
        }

        #endregion

        #region Sorting Codes

        #region Sorting Operation

        /// <summary>
        /// All Sorting Operation Done Here Except for TriState Condition
        /// </summary>
        /// <param name="column"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal void MakeSort(GridColumn column)
        {
            if (this.View == null)
                return;
#if WP
            if (!this.dataGrid.CheckColumnNameinItemProperties(column.MappingName) && !column.IsUnbound)
               return;
#else
            if (!this.dataGrid.CheckColumnNameinItemProperties(column.MappingName) && !column.IsUnbound && !this.View.IsDynamicBound)
               return;
#endif
           
            if (column.MappingName == null)
            {
                throw new InvalidOperationException("Mapping Name neccessary for Sorting,Grouping & Filtering");
            }
            var cancelScroll = false;
            if (this.View != null && column.AllowSorting)
            {
#if !WP
                this.dataGrid.SelectionController.CurrentCellManager.EndEdit();
                addNewRowController.CommitAddNew();
#endif

                IsInSort = true;
                this.View.BeginInit();

                var sortColumName = column.MappingName;
                var allowMultiSort = CheckControlKeyPressed();

                if (dataGrid.SortColumnDescriptions.Count > 0 && allowMultiSort)
                {
                    var sortedColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == sortColumName);
                    if (sortedColumn == default(SortColumnDescription))
                    {
                        var newSortColumn = new SortColumnDescription { ColumnName = sortColumName, SortDirection = ListSortDirection.Ascending };
                        var sortDescription = new SortDescription(sortColumName, ListSortDirection.Ascending);
                        if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { newSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Add, out cancelScroll))
                        {
                            this.dataGrid.SortColumnDescriptions.Add(newSortColumn);
                            this.dataGrid.View.SortDescriptions.Add(sortDescription);
                            this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { newSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Add);
                        }
                    }
                    else
                    {
                        if (sortedColumn.SortDirection == ListSortDirection.Descending && this.dataGrid.AllowTriStateSorting)
                        {
                            var removedSortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == sortColumName);
                            var removedSortDescription = this.dataGrid.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == sortColumName);
                            if (removedSortColumn != null)
                            {
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>(), new List<SortColumnDescription>() { removedSortColumn }, NotifyCollectionChangedAction.Remove, out cancelScroll))
                                {
                                    this.dataGrid.SortColumnDescriptions.Remove(removedSortColumn);
                                    if (removedSortDescription != default(SortDescription))
                                        this.dataGrid.View.SortDescriptions.Remove(removedSortDescription);
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>(), new List<SortColumnDescription>() { removedSortColumn }, NotifyCollectionChangedAction.Remove);
                                }
                            }
                        }
                        else
                        {
                            sortedColumn.SortDirection = sortedColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                            var sortDescription = new SortDescription(sortColumName, sortedColumn.SortDirection);
                            var removedSortDescription = this.dataGrid.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == sortColumName);
                            var removedSortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == sortedColumn.ColumnName);
                            if (this.RaiseSortColumnsChanging(new List<SortColumnDescription> { sortedColumn }, new List<SortColumnDescription>() { removedSortColumn }, NotifyCollectionChangedAction.Replace, out cancelScroll))
                            {
                                this.dataGrid.SortColumnDescriptions.Remove(removedSortColumn);
                                this.dataGrid.SortColumnDescriptions.Add(sortedColumn);
                                if (removedSortDescription != default(SortDescription))
                                    this.dataGrid.View.SortDescriptions.Remove(removedSortDescription);
                                this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                this.RaiseSortColumnsChanged(new List<SortColumnDescription> { sortedColumn }, new List<SortColumnDescription>() { removedSortColumn }, NotifyCollectionChangedAction.Replace);
                            }
                        }
                    }
                }
                else
                {
                    var currentSortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == sortColumName);
                    if (this.dataGrid.SortColumnDescriptions.Count > 0 && currentSortColumn != default(SortColumnDescription))
                    {
                        if (currentSortColumn.SortDirection == ListSortDirection.Descending && this.dataGrid.AllowTriStateSorting)
                        {
                            if (!this.HasGroup)
                            {
                                var sortColumnsClone = this.dataGrid.SortColumnDescriptions.ToList();
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>(), sortColumnsClone, NotifyCollectionChangedAction.Remove, out cancelScroll))
                                {
                                    this.dataGrid.SortColumnDescriptions.Clear();
                                    this.dataGrid.View.SortDescriptions.Clear();
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>(), sortColumnsClone, NotifyCollectionChangedAction.Remove);
                                }
                            }
                            else
                            {
                                var remainingSortColumns = this.GetSortColumnsNotInGroupColumns();
                                var remainingSortDescriptions = this.GetSortDescriptionNotInGroupDescription();
                                var removedSortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == currentSortColumn.ColumnName);
                                var removedSortDescription = this.dataGrid.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == currentSortColumn.ColumnName);
                                var sortDescription = new SortDescription();
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription> { currentSortColumn },
                                    new List<SortColumnDescription>(), NotifyCollectionChangedAction.Replace, out cancelScroll))
                                {
                                    if (this.dataGrid.GroupColumnDescriptions.Any(col => col.ColumnName.Equals(currentSortColumn.ColumnName)))
                                    {
                                        this.dataGrid.SortColumnDescriptions.Remove(removedSortColumn);
                                        currentSortColumn.SortDirection = currentSortColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                                        sortDescription.PropertyName = currentSortColumn.ColumnName;
                                        sortDescription.Direction = currentSortColumn.SortDirection;
                                        this.dataGrid.SortColumnDescriptions.Add(currentSortColumn);
                                    }
                                    else
                                    {
                                        foreach (var item in remainingSortColumns)
                                            this.dataGrid.SortColumnDescriptions.Remove(item);
                                    }

                                    if (this.View.GroupDescriptions.Any(col => (col as PropertyGroupDescription).PropertyName.Equals(sortDescription.PropertyName)))
                                    {
                                        this.dataGrid.View.SortDescriptions.Remove(removedSortDescription);
                                        this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                    }
                                    else
                                    {
                                        foreach (var item in remainingSortDescriptions)
                                        {
                                            if (removedSortDescription != default(SortDescription))
                                                this.View.SortDescriptions.Remove(item);
                                        }
                                    }
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { currentSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Replace);
                                }
                            }
                        }
                        else
                        {
                            currentSortColumn.SortDirection = currentSortColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                            var sortDescription = new SortDescription(sortColumName, currentSortColumn.SortDirection);
                            // clear it before adding the current sort column
                            if (!this.HasGroup)
                            {
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { currentSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Replace, out cancelScroll))
                                {
                                    this.dataGrid.SortColumnDescriptions.Clear();
                                    this.dataGrid.SortColumnDescriptions.Add(currentSortColumn);
                                    this.dataGrid.View.SortDescriptions.Clear();
                                    this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { currentSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Replace);
                                }
                            }
                            else
                            {
                                var remainingSortColumns = this.GetSortColumnsNotInGroupColumns();
                                var remainingSortDescriptions = this.GetSortDescriptionNotInGroupDescription();
                                var removedSortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == currentSortColumn.ColumnName);
                                var removedSortDescription = this.dataGrid.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == currentSortColumn.ColumnName);

                                if (removedSortColumn != default(SortColumnDescription))
                                {
                                    if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { currentSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Replace, out cancelScroll))
                                    {
                                        if (this.dataGrid.GroupColumnDescriptions.Any(col => col.ColumnName.Equals(currentSortColumn.ColumnName)))
                                        {
                                            this.dataGrid.SortColumnDescriptions.Remove(removedSortColumn);
                                        }
                                        else
                                        {
                                            foreach (var item in remainingSortColumns)
                                            {
                                                this.dataGrid.SortColumnDescriptions.Remove(item);
                                            }
                                        }
                                        this.dataGrid.SortColumnDescriptions.Add(currentSortColumn);
                                        if (this.View.GroupDescriptions.Any(col => (col as PropertyGroupDescription).PropertyName.Equals(sortDescription.PropertyName)))
                                        {
                                            this.View.SortDescriptions.Remove(removedSortDescription);
                                        }
                                        else
                                        {
                                            foreach (var item in remainingSortDescriptions)
                                            {
                                                if (removedSortDescription != default(SortDescription))
                                                    this.View.SortDescriptions.Remove(item);
                                            }
                                        }
                                        this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                        this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { currentSortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Replace);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var sortColumn = new SortColumnDescription()
                        {
                            ColumnName = sortColumName,
                            SortDirection = ListSortDirection.Ascending
                        };

                        var sortDescription = new SortDescription(sortColumName, ListSortDirection.Ascending);

                        if (!this.HasGroup)
                        {
                            if (this.dataGrid.SortColumnDescriptions.Count > 0)
                            {
                                var sortColumnsClone = this.dataGrid.SortColumnDescriptions.ToList();
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { sortColumn }, sortColumnsClone, NotifyCollectionChangedAction.Add, out cancelScroll))
                                {
                                    this.dataGrid.SortColumnDescriptions.Clear();
                                    this.dataGrid.SortColumnDescriptions.Add(sortColumn);
                                    this.dataGrid.View.SortDescriptions.Clear();
                                    this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { sortColumn }, sortColumnsClone, NotifyCollectionChangedAction.Add);
                                }
                            }
                            else
                            {
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { sortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Add, out cancelScroll))
                                {
                                    this.dataGrid.SortColumnDescriptions.Add(sortColumn);
                                    this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { sortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Add);
                                }
                            }
                        }
                        else
                        {
                            var sortColumnClone = new List<SortColumnDescription>();
                            this.GetSortColumnsNotInGroupColumns().ForEach(sortColumnClone.Add);
                            if (sortColumnClone.Any())
                            {
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { sortColumn }, sortColumnClone, NotifyCollectionChangedAction.Add, out cancelScroll))
                                {
                                    foreach (var removecolumns in sortColumnClone)
                                    {
                                        this.dataGrid.SortColumnDescriptions.Remove(removecolumns);
                                        var sortDesc = this.dataGrid.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == removecolumns.ColumnName);
                                        if (sortDesc != default(SortDescription))
                                            this.dataGrid.View.SortDescriptions.Remove(sortDesc);
                                    }
                                    this.dataGrid.SortColumnDescriptions.Add(sortColumn);
                                    this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { sortColumn }, sortColumnClone, NotifyCollectionChangedAction.Add);
                                }
                            }
                            else
                            {
                                if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { sortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Add, out cancelScroll))
                                {
                                    this.dataGrid.SortColumnDescriptions.Add(sortColumn);
                                    this.dataGrid.View.SortDescriptions.Add(sortDescription);
                                    this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { sortColumn }, new List<SortColumnDescription>(), NotifyCollectionChangedAction.Add);
                                }
                            }
                        }
                    }
                }

#if !WP
                this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
                this.View.EndInit();
                this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Sorting, null));

                if (!cancelScroll)
                    this.ScrollToCurrentItem();

                if (this.dataGrid.GroupDropArea != null)
                    this.dataGrid.GroupDropArea.UpdateGroupDropItemSortIcon(column);

                UpdateHeaderCells();
                IsInSort = false;
            }
        }

        internal void MakeSort(GridColumn column, ListSortDirection sortDirection)
        {
            if (this.View == null)
                return;
#if WP
            if (!this.dataGrid.CheckColumnNameinItemProperties(column.MappingName) && !column.IsUnbound)
                return;
#else
            if (!this.dataGrid.CheckColumnNameinItemProperties(column.MappingName) && !column.IsUnbound && !this.View.IsDynamicBound)
                return;
#endif
           
#if !WP
            this.dataGrid.SelectionController.CurrentCellManager.EndEdit();
            addNewRowController.CommitAddNew();
#endif


            bool cancelScroll = false;
            IsInSort = true;
            this.View.BeginInit();
            var action = NotifyCollectionChangedAction.Add;
            if (this.dataGrid.GroupColumnDescriptions.Count > 0 && this.dataGrid.SortColumnDescriptions.Count > 0)
            {
                if (dataGrid.GroupColumnDescriptions.Any(col => col.ColumnName.Equals(column.MappingName)))
                {
                    dataGrid.SortColumnDescriptions.Remove(dataGrid.SortColumnDescriptions.FirstOrDefault(col => col.ColumnName.Equals(column.MappingName)));
                    View.SortDescriptions.Remove(View.SortDescriptions.FirstOrDefault(col => col.PropertyName.Equals(column.MappingName)));
                    action = NotifyCollectionChangedAction.Replace;
                }
                else
                {
                    if (dataGrid.SortColumnDescriptions.Any(col => col.ColumnName.Equals(column.MappingName)))
                        action = NotifyCollectionChangedAction.Replace;
                    var sortedColumns = this.GetSortColumnsNotInGroupColumns();
                    sortedColumns.ForEach(x => this.dataGrid.SortColumnDescriptions.Remove(x));
                    var sortDescriptions = this.GetSortDescriptionNotInGroupDescription();
                    sortDescriptions.ForEach(x => this.View.SortDescriptions.Remove(x));
                }
            }
            else
            {
                this.dataGrid.SortColumnDescriptions.Clear();
                this.View.SortDescriptions.Clear();
            }
            var newSortColumn = new SortColumnDescription() { ColumnName = column.MappingName, SortDirection = sortDirection };
            var newSortDescription = new SortDescription() { PropertyName = column.MappingName, Direction = sortDirection };
            if (this.RaiseSortColumnsChanging(new List<SortColumnDescription>() { newSortColumn }, new List<SortColumnDescription>(), action, out cancelScroll))
            {
                this.dataGrid.SortColumnDescriptions.Add(newSortColumn);
                this.View.SortDescriptions.Add(newSortDescription);
                this.RaiseSortColumnsChanged(new List<SortColumnDescription>() { newSortColumn }, new List<SortColumnDescription>(), action);
            }
#if !WP
            this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
            this.View.EndInit();
            this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Sorting, null));
            if (!cancelScroll)
                this.ScrollToCurrentItem();
            if (this.dataGrid.GroupDropArea != null)
                this.dataGrid.GroupDropArea.UpdateGroupDropItemSortIcon(column);

            UpdateHeaderCells();
            IsInSort = false;
        }

        #endregion

        #region Sorting Event

        private bool RaiseSortColumnsChanging(IList<SortColumnDescription> addedColumns, IList<SortColumnDescription> removedColumns, NotifyCollectionChangedAction action, out bool cancelScroll)
        {
            return this.dataGrid.RaiseSortColumnsChanging(addedColumns, removedColumns, action, out cancelScroll);
        }

        private void RaiseSortColumnsChanged(IList<SortColumnDescription> addedColumns, IList<SortColumnDescription> removedColumns, NotifyCollectionChangedAction action)
        {
            this.dataGrid.RaiseSortColumnsChanged(addedColumns, removedColumns, action);
        }

        internal void ScrollToCurrentItem()
        {
#if WinRT
            if (this.dataGrid.View.CurrentItem != null && (this.dataGrid.View.CurrentItem as RecordEntry).Data != null)
#else
            if (this.dataGrid.View.CurrentItem != null)
#endif
            {
                var currentRowIndex = this.dataGrid.ResolveToRowIndex((this.dataGrid.View.CurrentItem as RecordEntry).Data);
                if (currentRowIndex < 0)
                    currentRowIndex = 0;
                this.dataGrid.VisualContainer.ScrollRows.ScrollInView(currentRowIndex);
            }
        }

        #endregion

        #region Refresh DataRows

        public void RefreshDataRow()
        {
            if (this.dataGrid.VisualContainer == null)
                return;
            this.dataGrid.RowGenerator.Items.ForEach(ResetRowIndex);
            this.dataGrid.VisualContainer.InvalidateMeasureInfo();
        }

        /// <summary>
        /// Method which helps to refresh the rows from the corresponding row index.
        /// </summary>
        /// <param name="fromRowIndex"></param>
        /// <remarks></remarks>
        public void RefreshDataRow(int fromRowIndex, bool invalidateMeasure)
        {
            if (this.dataGrid.VisualContainer == null) return;

            this.dataGrid.RowGenerator.Items.ForEach(row =>
                {
                    if (row.RowIndex >= fromRowIndex)
                        ResetRowIndex(row);
                });
            this.dataGrid.VisualContainer.InvalidateMeasureInfo();
        }

        /// <summary>
        /// reseting row index except header
        /// </summary>
        /// <param name="dr"></param>
        /// <remarks></remarks>
        private void ResetRowIndex(DataRowBase dr)
        {
            if (dr.RowRegion == RowRegion.Body || (dr.RowType == RowType.TableSummaryRow || dr.RowType == RowType.TableSummaryCoveredRow))
                dr.RowIndex = -1;
        }

        public void UpdateDataRow(int index)
        {
            if (this.dataGrid.VisualContainer == null)
                return;

            var datarow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == index);
            if (datarow != null)
            {
                datarow.RowIndex = -1;
                //this.dataGrid.VisualContainer.UpdateScrollBars();
                this.dataGrid.VisualContainer.InvalidateMeasureInfo();
            }
        }

        private void UpdateDataRow(int index, int count, NotifyCollectionChangedAction action)
        {
            if (this.dataGrid.VisualContainer == null)
                return;

            var datarow =
                this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == index && !row.IsAddNewRow);
            var level = count;
#if !WP
            if (this.dataGrid.DetailsViewManager.HasDetailsView)
                level += this.dataGrid.DetailsViewDefinition.Count;
#endif

            if (datarow != null)
            {
                if (action == NotifyCollectionChangedAction.Add)
                {
                    this.dataGrid.RowGenerator.Items.ForEach(row =>
                        {
                            row.SuspendUpdateStyle();
                            if (row.RowRegion != RowRegion.Header && row.RowIndex >= index)
                                row.RowIndex += level;
                            if (row.RowType != RowType.DefaultRow && row.RowRegion != RowRegion.Header &&
                                row.RowRegion != RowRegion.Footer)
                                row.RowIndex = -1;
                            row.ResumeUpdateStyle();
                        });
                }
                else if (action == NotifyCollectionChangedAction.Remove)
                {
                    var rowIndex = datarow.RowIndex;
                    datarow.RowIndex = -1;
#if !WP
                    if (this.dataGrid.DetailsViewManager.HasDetailsView && datarow.IsExpanded)
                    {
                        for (int i = rowIndex + 1; i < rowIndex + level; i++)
                        {
                            var row = this.dataGrid.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == i);
                            if (row != null)
                                row.RowIndex = -1;
                        }
                    }
#endif
                    this.dataGrid.RowGenerator.Items.ForEach(row =>
                        {
                            row.SuspendUpdateStyle();

                            if (row.RowRegion != RowRegion.Header && row.RowIndex > index)
                                row.RowIndex -= level;
                            if (row.RowType != RowType.DefaultRow && row.RowRegion != RowRegion.Header &&
                                row.RowRegion != RowRegion.Footer)
                                row.RowIndex = -1;

                            row.ResumeUpdateStyle();
                        });
                }
                else
                    throw new NotImplementedException("UpdateDataRow is not implemented for" + action.ToString());
            }
            UpdateView(index, count, action);
        }

        /// <summary>
        /// Method which helps to update the View.
        /// </summary>
        /// <remarks></remarks>
        public void RefreshView()
        {
            if (this.View == null || this.dataGrid.VisualContainer == null) 
                return;

            this.dataGrid.RowGenerator.Items.Where(row => row.RowType == RowType.TableSummaryCoveredRow || row.RowType == RowType.TableSummaryRow).ForEach(dr => UpdateBindingTableSummary(dr));
            this.RefreshDataRow();
            this.dataGrid.UpdateRowCount();
            this.dataGrid.VisualContainer.UpdateScrollBars();
        }

        private void RefreshView(int rowIndex, int count, NotifyCollectionChangedAction action, int recordIndex = 0,int recordCount=0)
        {
            if (this.View == null || this.dataGrid.VisualContainer == null) 
                return;

            if (action == NotifyCollectionChangedAction.Add)
                this.dataGrid.InsertLine(rowIndex, count,recordIndex,recordCount);
            else if (action == NotifyCollectionChangedAction.Remove)
                this.dataGrid.RemoveLine(rowIndex, count);
            else
                throw new NotImplementedException("RefreshView not implemented for" + action.ToString());

            this.dataGrid.RowGenerator.Items.Where(row => row.RowType == RowType.TableSummaryCoveredRow || row.RowType == RowType.TableSummaryRow).ForEach(dr => UpdateBindingTableSummary(dr));
            this.RefreshDataRow();
            this.dataGrid.VisualContainer.UpdateScrollBars();
        }

        private void UpdateView(int index, int count, NotifyCollectionChangedAction action)
        {
            if (this.View == null || this.dataGrid.VisualContainer == null) 
                return;

            if (action == NotifyCollectionChangedAction.Add)
                this.dataGrid.InsertLine(index, count);
            else if(action == NotifyCollectionChangedAction.Remove)
                this.dataGrid.RemoveLine(index, count);
            else
                throw new NotImplementedException("RefreshView not implemented for" + action.ToString());

            this.dataGrid.RowGenerator.Items.Where(row => row.RowType == RowType.TableSummaryCoveredRow || row.RowType == RowType.TableSummaryRow).ForEach(dr => UpdateBindingTableSummary(dr));
            this.dataGrid.VisualContainer.UpdateScrollBars();
            this.dataGrid.VisualContainer.InvalidateMeasureInfo();
        }

        #endregion

        #region Numbering / Ordering the sorted columns

        /// <summary>
        /// Helps to make numbering for sorted column
        /// </summary>
        /// <remarks></remarks>
        internal void ShowSortNumbers()
        {
            var rowGeneratorItems = this.dataGrid.RowGenerator.Items;
            if (rowGeneratorItems == null || !rowGeneratorItems.Any()) return;
            var sortNumber = 1;
            var headerIndex = this.dataGrid.GetHeaderIndex();
            foreach (var item in View.SortDescriptions)
            {
                var sortColumn = rowGeneratorItems[headerIndex].VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == item.PropertyName);
                if (sortColumn != null)
                {
                    var currentCell = sortColumn.ColumnElement as GridHeaderCellControl;
                    if (currentCell != null)
                    {
                        currentCell.SortNumber = sortNumber.ToString();
                        currentCell.SortNumberVisibility = Visibility.Visible;
                    }
                }
                sortNumber += 1;
            }
        }

        /// <summary>
        /// Collapse the sort number visibility when ShowSortNumber property sets false
        /// </summary>
        /// <remarks></remarks>
        internal void CollapseSortNumber()
        {
            var rowGeneratorItems = this.dataGrid.RowGenerator.Items;
            if (rowGeneratorItems == null) return;
            var headerIndex = this.dataGrid.GetHeaderIndex();
            foreach (var visibleColumn in rowGeneratorItems[headerIndex].VisibleColumns)
            {
                var visibleCell = visibleColumn.ColumnElement as GridHeaderCellControl;
                if (visibleCell != null)
                    visibleCell.SortNumberVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Changing the sort_icon Visibility

        /// <summary>
        /// Change SortIcon Visibility On Collection changed
        /// </summary>
        /// <param name="column"></param>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks>Icon Visibility Changed according the collection change</remarks>
        private void ChangeSortIconVisibility(GridColumn column, object oldItem, NotifyCollectionChangedAction action)
        {
            if (this.dataGrid.RowGenerator.Items.Count <= 0) return;
            var headerIndex = this.dataGrid.GetHeaderIndex();
            var dataRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == headerIndex);
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (dataRow != null)
                    {
                        var sortDescription =
                            View.SortDescriptions.FirstOrDefault(item => item.PropertyName == column.MappingName);
                        var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == sortDescription.PropertyName);
                        if (dataColumn != null)
                        {
                            var currentCell = dataColumn.ColumnElement as GridHeaderCellControl;
                            if (currentCell != null)
                            {
                                currentCell.SortDirection = sortDescription.Direction;
                                if (this.View.SortDescriptions.Count > 1)
                                {
                                    var sortNumber = this.View.SortDescriptions.IndexOf(sortDescription) + 1;
                                    currentCell.SortNumber = sortNumber.ToString();
                                }
                                else
                                    currentCell.SortNumberVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (dataRow != null)
                    {
                        string columnName;
                        if (oldItem is SortColumnDescription)
                            columnName = (oldItem as SortColumnDescription).ColumnName;
                        else
                            columnName = ((SortDescription)oldItem).PropertyName;
                        var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == columnName);
                        if (dataColumn != null)
                        {
                            var currentCell = dataColumn.ColumnElement as GridHeaderCellControl;
                            if (currentCell != null)
                            {
                                currentCell.SortDirection = null;
                                currentCell.SortNumberVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var sortDescription in View.SortDescriptions)
                    {
                        var dataColumn = dataRow.VisibleColumns.SingleOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == sortDescription.PropertyName);
                        if (dataColumn != null)
                        {
                            var currentCell = dataColumn.ColumnElement as GridHeaderCellControl;
                            if (currentCell != null)
                            {
                                currentCell.SortDirection = null;
                                currentCell.SortNumberVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    foreach (var sortColumn in this.dataGrid.SortColumnDescriptions)
                    {
                        var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == sortColumn.ColumnName);
                        if (dataColumn != null)
                        {
                            var currentCell = dataColumn.ColumnElement as GridHeaderCellControl;
                            if (currentCell != null)
                            {
                                currentCell.SortDirection = null;
                                currentCell.SortNumberVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    break;
            }
            if (this.dataGrid.GroupDropArea != null && column != null)
                this.dataGrid.GroupDropArea.UpdateGroupDropItemSortIcon(column);
        }

        #endregion

        #region CollectionChanged Events

        private void OnSortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GridColumn column = null;
            if (IsInSort || isSortColumnChanged)
            {
                return;
            }
            isSortDescriptionChanged = true;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SortDescription sortItem in e.NewItems)
                    {
                        if (this.View.SortDescriptions.Count(desc => desc.PropertyName == sortItem.PropertyName) > 1)
                        {
                            throw new InvalidOperationException("SortDescription already exist in View.SortDescriptions");
                        }
                        var hasSortItem = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == sortItem.PropertyName) != null;
                        if (!hasSortItem)
                        {
                            this.dataGrid.SortColumnDescriptions.Insert(View.SortDescriptions.IndexOf(sortItem), new SortColumnDescription() { ColumnName = sortItem.PropertyName, SortDirection = sortItem.Direction });
                            column = this.dataGrid.Columns.FirstOrDefault(x => x.MappingName == sortItem.PropertyName);
                            ChangeSortIconVisibility(column, null, e.Action);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (SortDescription sortItem in e.OldItems)
                    {
                        var modelSortItem = this.dataGrid.SortColumnDescriptions.FirstOrDefault(s => s.ColumnName == sortItem.PropertyName);
                        if (modelSortItem != null)
                        {
                            this.dataGrid.SortColumnDescriptions.Remove(modelSortItem);
                            column = this.dataGrid.Columns.FirstOrDefault(x => x.MappingName == sortItem.PropertyName);
                            if (e.OldItems != null)
                                ChangeSortIconVisibility(column, e.OldItems[0], e.Action);
                            else
                                ChangeSortIconVisibility(column, null, e.Action);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ChangeSortIconVisibility(column, null, e.Action);
                    this.dataGrid.SortColumnDescriptions.Clear();
                    break;
            }
            if (this.dataGrid.ShowSortNumbers && this.dataGrid.View.SortDescriptions.Count > 1)
                ShowSortNumbers();
            if (!isSuspended)
                this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Sorting, null));
            isSortDescriptionChanged = false;
        }

        internal void OnSortColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !WP
            if (this.dataGrid.NotifyListener != null) 
                this.dataGrid.NotifyListener.NotifyCollectionChanged(this.dataGrid.SortColumnDescriptions, e, datagrid => datagrid.SortColumnDescriptions, this.dataGrid, typeof(SortColumnDescription));
#endif
            GridColumn column = null;
            if (IsInSort || isSortDescriptionChanged || View == null)
            {
                return;
            }
            
            isSortColumnChanged = true;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SortColumnDescription newItem in e.NewItems)
                    {
                        if (this.dataGrid.SortColumnDescriptions.Count(col => col.ColumnName == newItem.ColumnName) > 1)
                        {
                            throw new InvalidOperationException("SortColumnDescription already exist in DataGrid.SortColumnDescriptions");
                        }
                        var desc = this.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == newItem.ColumnName);
                        if (desc != default(SortDescription))
                            return;
                        var index = this.dataGrid.SortColumnDescriptions.IndexOf(newItem);
                        this.View.SortDescriptions.Insert(index, new SortDescription { PropertyName = newItem.ColumnName, Direction = newItem.SortDirection });
                        column = this.dataGrid.Columns.FirstOrDefault(x => x.MappingName == newItem.ColumnName);
                        ChangeSortIconVisibility(column, null, e.Action);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (SortColumnDescription item in e.OldItems)
                    {
                        var sortDesc = this.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == item.ColumnName);
                        if (sortDesc != default(SortDescription))
                        {
                            this.View.SortDescriptions.Remove(sortDesc);
                        }
                        column = this.dataGrid.Columns.FirstOrDefault(x => x.MappingName == item.ColumnName);
                        if (e.OldItems != null)
                            ChangeSortIconVisibility(column, e.OldItems[0], e.Action);
                        else
                            ChangeSortIconVisibility(column, null, e.Action);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ChangeSortIconVisibility(column, null, e.Action);
                    this.View.SortDescriptions.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    var oldItem = e.OldItems[0] as SortColumnDescription;
                    var replaceItem = e.NewItems[0] as SortColumnDescription;
                    if (this.View.SortDescriptions.Count(col => col.PropertyName == replaceItem.ColumnName) >= 1)
                    {
                        return;
                    }
                    var sortitemDesc = this.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == oldItem.ColumnName);
                    if (sortitemDesc != default(SortDescription))
                    {
                        this.View.SortDescriptions[e.OldStartingIndex] = new SortDescription { PropertyName = replaceItem.ColumnName, Direction = replaceItem.SortDirection };
                        column = this.dataGrid.Columns.FirstOrDefault(x => x.MappingName == oldItem.ColumnName);
                        ChangeSortIconVisibility(column, oldItem, NotifyCollectionChangedAction.Remove);
                        column = this.dataGrid.Columns.FirstOrDefault(x => x.MappingName == replaceItem.ColumnName);
                        ChangeSortIconVisibility(column, null, NotifyCollectionChangedAction.Add);
                    }
                    break;
            }
            if (this.dataGrid.ShowSortNumbers && this.dataGrid.SortColumnDescriptions.Count > 1)
                ShowSortNumbers();
            this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Sorting, null));
            isSortColumnChanged = false;
        }

        #endregion

        #endregion

        #region Grouping Codes

        #region Internal Methods

        /// <summary>
        /// Method which is helps to made the group by passing the column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="converter"></param>
        /// <remarks></remarks>
        internal void GroupBy(string columnName, IValueConverter converter)
        {
            if (this.View == null)
                return;

            var gridColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
#if WP
            if (!this.dataGrid.CheckColumnNameinItemProperties(columnName) && !gridColumn.IsUnbound)
                return;
#else
            if (!this.dataGrid.CheckColumnNameinItemProperties(columnName) && !gridColumn.IsUnbound && !this.View.IsDynamicBound)
                return;
#endif
           
            if (this.CheckForExistingGroupDescription(columnName))
                return;

#if !WP
            this.dataGrid.SelectionController.CurrentCellManager.EndEdit();
            addNewRowController.CommitAddNew();
#endif



            if (this.dataGrid.VisualContainer == null)
            {
                this.dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = columnName, Converter = converter });
                return;
            }

            this.Suspend();

            if (this.dataGrid.GroupColumnDescriptions.All(col => col.ColumnName != columnName))
            {
                this.dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = columnName, Converter = converter });
                this.View.BeginInit();
                this.View.GroupDescriptions.Add(new PropertyGroupDescription(columnName, converter));
                var sortColumn = this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == columnName);
                if (sortColumn == default(SortDescription))
                    this.View.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Ascending));
#if !WP
                this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
                this.View.EndInit();

                this.AddGroupDropItem(columnName);

                //Here we reset the column to add the extra columns to show the indent.
                this.ResetColumns();

                if (!this.dataGrid.ShowColumnWhenGrouped)
                {
                    var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
                    if (column != null)
                        column.IsHidden = true;
                }
                this.dataGrid.UpdateRowCountAndScrollBars();
            }
            this.Resume();
        }

        /// <summary>
        /// Add the initial Groupdescription to GropDropArea
        /// </summary>
        /// <remarks></remarks>
        internal void InitializeGrouping()
        {
            if (this.View == null) return;
            this.View.GroupDescriptions.ForEach(desc =>
                {
                    var gridcolumn =
                        this.dataGrid.Columns.FirstOrDefault(
                            col => col.MappingName == (desc as PropertyGroupDescription).PropertyName);
                    if (gridcolumn != null && !this.dataGrid.ShowColumnWhenGrouped)
                        gridcolumn.IsHidden = true;
                });

            if (this.dataGrid.GroupDropArea != null)
            {
                if (this.View == null) return;
                this.View.GroupDescriptions.ForEach(
                    desc => this.AddGroupDropItem((desc as PropertyGroupDescription).PropertyName));
            }
        }

        /// <summary>
        /// Method which is helps to made the group by passing the column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="converter"></param>
        /// <remarks></remarks>
        internal void GroupBy(string columnName, int insertAt, IValueConverter converter)
        {
            if (this.View == null)
                return;

            var gridColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
#if WP
            if (!this.dataGrid.CheckColumnNameinItemProperties(columnName) && !gridColumn.IsUnbound)
                return;
#else
            if (!this.dataGrid.CheckColumnNameinItemProperties(columnName) && !gridColumn.IsUnbound &&!this.View.IsDynamicBound)
                return;
#endif
           
            if (this.CheckForExistingGroupDescription(columnName))
                return;

#if !WP
            this.dataGrid.SelectionController.CurrentCellManager.EndEdit();
            addNewRowController.CommitAddNew();
#endif


            if (this.dataGrid.VisualContainer == null)
            {
                this.dataGrid.GroupColumnDescriptions.Insert(insertAt, new GroupColumnDescription() { ColumnName = columnName, Converter = converter });
                return;
            }

            this.Suspend();

            if (this.dataGrid.GroupColumnDescriptions.All(col => col.ColumnName != columnName))
            {

                this.dataGrid.GroupColumnDescriptions.Insert(insertAt, new GroupColumnDescription() { ColumnName = columnName, Converter = converter });

                this.View.BeginInit();
                this.View.GroupDescriptions.Insert(insertAt, new PropertyGroupDescription(columnName, converter));
                var sortColumn = this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == columnName);
                if (sortColumn == default(SortDescription))
                    this.View.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Ascending));
#if !WP
                this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
                this.View.EndInit();

                this.AddGroupDropItem(columnName);

                //Here we reset the column to add the extra columns to show the indent.
                this.ResetColumns();

                if (!this.dataGrid.ShowColumnWhenGrouped)
                {
                    var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
                    if (column != null)
                        column.IsHidden = true;
                }
                this.dataGrid.UpdateRowCountAndScrollBars();
            }
            this.Resume();
        }

        /// <summary>
        /// Method which is helps to remove the grouping by passing the corresponding column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <remarks></remarks>
        internal void RemoveGroup(string columnName)
        {
            this.Suspend();
            if (this.View.GroupDescriptions.Count > 0 && this.dataGrid.GroupColumnDescriptions.Any(col => col.ColumnName == columnName))
            {
                var groupColumn = this.dataGrid.GroupColumnDescriptions.FirstOrDefault(col => col.ColumnName == columnName);
                this.dataGrid.GroupColumnDescriptions.Remove(groupColumn);

                var groupDesc = this.View.GroupDescriptions.FirstOrDefault(desc => ((PropertyGroupDescription)desc).PropertyName == columnName);
                var sortDesc = this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == columnName);

                this.View.BeginInit();
                this.View.GroupDescriptions.Remove(groupDesc);
                this.View.SortDescriptions.Remove(sortDesc);
#if !WP
                this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
                this.addNewRowController.CommitAddNew();
#endif

                this.View.EndInit();

                this.RemoveGroupDropItem(columnName);

                this.ResetColumns();

                var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
                if (column != null && column.IsHidden)
                    column.IsHidden = false;

                this.dataGrid.UpdateRowCountAndScrollBars();
            }
            this.Resume();
        }

        /// <summary>
        /// Method which helps to expand all the groups in Specific level
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="groupLevel"></param>
        /// <remarks></remarks>
        internal void ExpandGroupsAtLevel(List<Group> groups, int groupLevel)
        {
            foreach (var group in groups)
            {
                if (group.Level <= groupLevel)
                {
                    group.IsExpanded = true;
                }
                if (!group.IsBottomLevel)
                    this.ExpandGroupsAtLevel(group.Groups, groupLevel);
            }
        }

        /// <summary>
        /// Method which helps to collapse all the groups in Specific level
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="groupLevel"></param>
        /// <remarks></remarks>
        internal void CollapseGroupsAtLevel(List<Group> groups, int groupLevel)
        {
            foreach (var group in groups)
            {
                if (group.Level == groupLevel)
                {
                    group.IsExpanded = false;
                }
                if (!group.IsBottomLevel)
                    this.CollapseGroupsAtLevel(group.Groups, groupLevel);
            }
        }

        /// <summary>
        /// Method which helps to expand the specific group by paasing the corresponding group
        /// </summary>
        /// <param name="group"></param>
        /// <remarks></remarks>
        internal void ExpandGroup(Group group)
        {
            if (group != null)
            {
                if (!this.dataGrid.RaiseGroupExpandingEvent(new GroupChangingEventArgs(this.dataGrid) { Group = group }))
                {
                    if (!group.IsExpanded)
                    {
                        var groupModel = this.View.TopLevelGroup;
                        var insertedItems = groupModel.ExpandGroup(group);
                        var startIdx = this.dataGrid.ResolveStartIndexOfGroup(group) + 1;
                        var lineSizeCollection = this.dataGrid.VisualContainer.RowHeights as LineSizeCollection;
                        lineSizeCollection.SuspendUpdates();
                        this.dataGrid.VisualContainer.InsertRows(startIdx, insertedItems);
                        lineSizeCollection.ResumeUpdates();
#if !WP
                        if (this.dataGrid.DetailsViewManager.HasDetailsView)
                        {
                            this.dataGrid.RowGenerator.Items.OfType<DetailsViewDataRow>().ForEach(row =>
                                {
                                    if (row.RowIndex > startIdx)
                                    {
                                        if (row.CatchedRowIndex != -1)
                                            row.CatchedRowIndex += insertedItems;
                                    }
                                });
                        }
#endif
                        this.dataGrid.SelectionController.HandleGroupExpandCollapse(startIdx, insertedItems, true);
                        this.dataGrid.VisualContainer.ScrollRows.MarkDirty();
#if !WP
                        if (this.dataGrid.DetailsViewManager.HasDetailsView)
                            RestExpandedState(group);
#endif
                        this.dataGrid.VisualContainer.InvalidateMeasureInfo();
                    }
                    this.dataGrid.RaiseGroupExpandedEvent(new GroupChangedEventArgs(this.dataGrid) { Group = group });
                }
            }
        }

#if !WP

        private void RestExpandedState(Group group)
        {
            if (!group.IsBottomLevel)
            {
                foreach (var childGroup in group.Groups)
                {
                    RestExpandedState(childGroup);
                }
            }
            else
            {
                if (group.IsExpanded)
                {
                    var lineSizeCollection = this.dataGrid.VisualContainer.RowHeights as LineSizeCollection;
                    int startIdx = this.dataGrid.ResolveStartIndexOfGroup(group) + 1;
                    var endIndex = startIdx + group.GetRecordCount();
                    lineSizeCollection.SetHiddenIntervalWithState(startIdx, endIndex, this.dataGrid.DetailsViewManager.GetHiddenPattern());
                    foreach (var record in group.Records)
                    {
                        if (record.IsExpanded)
                        {
                            foreach (var viewDefinition in this.dataGrid.DetailsViewDefinition)
                            {
                                startIdx++;
                                lineSizeCollection.SetHidden(startIdx, startIdx, false);
                            }
                        }
                        startIdx += (this.dataGrid.DetailsViewDefinition.Count + 1);
                    }
                }
            }
        }
#endif
        /// <summary>
        /// Method which helps to collapse the specific group by paasing the corresponding group
        /// </summary>
        /// <param name="group"></param>
        /// <remarks></remarks>
        internal void CollapseGroup(Group group)
        {
            if (group != null)
            {
                if (!this.dataGrid.RaiseGroupCollapsingEvent(new GroupChangingEventArgs(this.dataGrid) { Group = group }))
                {
                    if (group.IsExpanded)
                    {
                        var groupModel = this.View.TopLevelGroup;
                        int collapsedItems = 0;
                        groupModel.ComputeCount(group, ref collapsedItems);
                        var startIndex = this.dataGrid.ResolveStartIndexOfGroup(group) + 1;
                        this.dataGrid.SelectionController.HandleGroupExpandCollapse(startIndex, collapsedItems, false);
                        groupModel.CollapseGroup(group);
                        var lineSizeCollection = this.dataGrid.VisualContainer.RowHeights as LineSizeCollection;
                        lineSizeCollection.SuspendUpdates();
                        this.dataGrid.VisualContainer.RemoveRows(startIndex, collapsedItems);
                        lineSizeCollection.ResumeUpdates();
#if !WP
                        if (this.dataGrid.DetailsViewManager.HasDetailsView)
                        {
                            this.dataGrid.RowGenerator.Items.OfType<DetailsViewDataRow>().ForEach(row =>
                            {
                                if (row.RowIndex > startIndex + collapsedItems)
                                {
                                    if (row.CatchedRowIndex != -1)
                                        row.CatchedRowIndex -= collapsedItems;
                                }
                            });
                        }
#endif
                        this.dataGrid.VisualContainer.ScrollRows.MarkDirty();
                        this.dataGrid.VisualContainer.InvalidateMeasureInfo();
                    }
                    this.dataGrid.RaiseGroupCollapsedEvent(new GroupChangedEventArgs(this.dataGrid) { Group = group });
                }
            }
        }

        /// <summary>
        /// Method which helps to reset the column count and column indexes when group added or removed
        /// </summary>
        /// <remarks></remarks>
        internal void ResetColumns()
        {
            var columnCount = this.dataGrid.Columns.Count;
            columnCount += this.dataGrid.ShowRowHeader ? 1 : 0;
#if !WP
            columnCount += this.dataGrid.DetailsViewManager.HasDetailsView ? 1 : 0;
#endif
            if (this.View.TopLevelGroup != null)
                columnCount += this.View.TopLevelGroup.GetMaxLevel();

            int increasedIndexValue = columnCount - this.dataGrid.VisualContainer.ColumnCount;

            this.dataGrid.RowGenerator.Items.ForEach(row =>
                {
                    if ((row.RowType != RowType.TableSummaryCoveredRow && row.RowType != RowType.TableSummaryRow) && (row.RowRegion == RowRegion.Header && row.RowIndex < this.dataGrid.GetHeaderIndex()))
                    {
                        this.dataGrid.RowGenerator.UpdateStackedheaderCoveredRow(row as SpannedDataRow, increasedIndexValue);
                    }
                });

            (this.dataGrid.VisualContainer.ColumnWidths as LineSizeCollection).SuspendUpdates();

            if (increasedIndexValue > 0)
            {
                this.dataGrid.VisualContainer.InsertColumns(this.dataGrid.View.GroupDescriptions.Count - 1 + (this.dataGrid.ShowRowHeader ? 1 : 0), increasedIndexValue);
                this.dataGrid.VisualContainer.ColumnWidths[this.dataGrid.View.GroupDescriptions.Count - 1 + (this.dataGrid.ShowRowHeader ? 1 : 0)] = this.IndentColumnSize;
            }
            else
            {
                var indentcolIndex = this.dataGrid.ShowRowHeader ? this.dataGrid.View.GroupDescriptions.Count + 1 : this.dataGrid.View.GroupDescriptions.Count;
                this.dataGrid.VisualContainer.RemoveColumns(indentcolIndex, Math.Abs(increasedIndexValue));
                int startColindex = this.dataGrid.ShowRowHeader ? 1 : 0;
                int indentcolcount = this.View.GroupDescriptions.Count;

#if !WP
                if (dataGrid.DetailsViewManager.HasDetailsView)
                    indentcolcount++;
#endif
                for (int i = startColindex; i <= indentcolcount; i++)
                {
                    this.dataGrid.VisualContainer.ColumnWidths[i] = this.IndentColumnSize;
                }
            }

            if (this.dataGrid.FrozenColumnCount > 0)
                this.dataGrid.VisualContainer.FrozenColumns = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.FrozenColumnCount);

            (this.dataGrid.VisualContainer.ColumnWidths as LineSizeCollection).ResumeUpdates();

            this.dataGrid.GridColumnSizer.RefreshAll();
        }

        /// <summary>
        /// Method helps to suspand all the collection change update when doing grouping operatrions 
        /// </summary>
        /// <remarks></remarks>
        internal void Suspend()
        {
            isSuspended = true;
        }

        /// <summary>
        /// Method helps to suspand all the collection change update when doing grouping operatrions 
        /// </summary>
        /// <remarks></remarks>
        internal void Resume()
        {
            isSuspended = false;
        }

        internal void AddGroupDropItem(string columnName)
        {
            if (this.dataGrid.ShowGroupDropArea && this.dataGrid.GroupDropArea != null)
            {
                var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
                if (column != null)
                    this.dataGrid.GroupDropArea.AddGroupDropAreaItem(column, ListSortDirection.Ascending, false);
            }
        }

        internal void AddGroupDropItem(string columnName, int insertAt)
        {
            if (this.dataGrid.ShowGroupDropArea && this.dataGrid.GroupDropArea != null)
            {
                var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
                if (column != null)
                    this.dataGrid.GroupDropArea.AddGroupDropAreaItem(column, ListSortDirection.Ascending, insertAt, false);
            }
        }

        internal void RemoveGroupDropItem(string columnName)
        {
            if (this.dataGrid.ShowGroupDropArea && this.dataGrid.GroupDropArea != null)
            {
                var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == columnName);
                if (column != null)
                    this.dataGrid.GroupDropArea.RemoveGroupDropItem(column);
            }
        }

        internal void RemoveAllGroupDropItems()
        {
            if (this.dataGrid.ShowGroupDropArea && this.dataGrid.GroupDropArea != null)
                this.dataGrid.GroupDropArea.RemoveAllGroupDropItems();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method which helps to get the SortColumn which are not in the GroupedColumns
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private IEnumerable<SortColumnDescription> GetSortColumnsNotInGroupColumns()
        {
            var unAvailableColumns = new List<SortColumnDescription>();
            foreach (var sortColumn in this.dataGrid.SortColumnDescriptions)
            {
                if (this.dataGrid.GroupColumnDescriptions.All(grpColumn => grpColumn.ColumnName != sortColumn.ColumnName))
                    unAvailableColumns.Add(sortColumn);
            }
            return unAvailableColumns;
        }

        /// <summary>
        /// Method which helps to get the unavailable sort description which property names is not in group description
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private IEnumerable<SortDescription> GetSortDescriptionNotInGroupDescription()
        {
            var unAvailableSortDesc = new List<SortDescription>();
            foreach (var sortDesc in this.View.SortDescriptions)
            {
                if (this.View.GroupDescriptions.All(groupDesc => ((PropertyGroupDescription)groupDesc).PropertyName != sortDesc.PropertyName))
                    unAvailableSortDesc.Add(sortDesc);
            }
            return unAvailableSortDesc;
        }

        /// <summary>
        /// Check whether the GroupDescription is already present in Groupdescriptions or not
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CheckForExistingGroupDescription(string columnName)
        {
            return this.View.GroupDescriptions.Cast<PropertyGroupDescription>().Any(group => group.PropertyName == columnName);
        }

        /// <summary>
        /// Check whethe the corresponding column name already present in GropColumns  or not.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CheckForExistingGroupColumn(string columnName)
        {
            return this.dataGrid.GroupColumnDescriptions.Any(col => col.ColumnName == columnName);
        }

        #endregion

        #region Event Helper Methods

        /// <summary>
        /// Method which helps to update the view when  change the GroupColumn collection
        /// </summary>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void OnGroupColumnDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool isChanged = false;
#if !WP
            if (this.dataGrid.NotifyListener != null) 
                this.dataGrid.NotifyListener.NotifyCollectionChanged(this.dataGrid.GroupColumnDescriptions, e, datagrid => datagrid.GroupColumnDescriptions, this.dataGrid, typeof(GroupColumnDescription));
#endif
            if (isSuspended || isGroupDescriptionChanged || this.View == null)
                return;
#if !WP
            this.dataGrid.SelectionController.CurrentCellManager.EndEdit();
            addNewRowController.CommitAddNew();
#endif

            this.isGroupColumnChanged = true;
            Suspend();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var column in e.NewItems)
                        {
                            var groupColumn = column as GroupColumnDescription;
                            if (this.dataGrid.GroupColumnDescriptions.Count(col => col.ColumnName == groupColumn.ColumnName) > 1)
                            {
                                throw new InvalidOperationException("GroupColumnDescription already exist in DataGrid.GroupColumnDescriptions");
                            }
                            if (this.CheckForExistingGroupDescription(groupColumn.ColumnName))
                                continue;

                            var gridColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == groupColumn.ColumnName);
#if WP
                            if (View != null && this.dataGrid.CheckColumnNameinItemProperties(groupColumn.ColumnName) && !gridColumn.IsUnbound)
#else
                            if (View != null && this.dataGrid.CheckColumnNameinItemProperties(groupColumn.ColumnName) && !gridColumn.IsUnbound && !this.View.IsDynamicBound)
#endif
                            {
                                this.View.BeginInit();
                                this.View.GroupDescriptions.Insert(e.NewStartingIndex, new PropertyGroupDescription(groupColumn.ColumnName, groupColumn.Converter));
                                var sortColumn = this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == groupColumn.ColumnName);
                                if (sortColumn == default(SortDescription))
                                    this.View.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription(groupColumn.ColumnName, ListSortDirection.Ascending));
                                if (!this.dataGrid.ShowColumnWhenGrouped)
                                {
                                    var groupedColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == groupColumn.ColumnName);
                                    if (groupedColumn != null)
                                        groupedColumn.IsHidden = true;
                                }
#if SILVERLIGHT
                                if (!isGroupDescriptionMoved)
#endif
                                this.AddGroupDropItem(groupColumn.ColumnName, e.NewStartingIndex);
                                isChanged = true;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        this.View.BeginInit();
                        foreach (var column in e.OldItems)
                        {
                            var groupColumn = column as GroupColumnDescription;
                            var groupDesc = this.View.GroupDescriptions.FirstOrDefault(desc => ((PropertyGroupDescription)desc).PropertyName == groupColumn.ColumnName);
                            var sortDesc = this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == groupColumn.ColumnName);
                            this.View.GroupDescriptions.Remove(groupDesc);
                            if (sortDesc != default(SortDescription))
                                this.View.SortDescriptions.Remove(sortDesc);
                            if (!this.dataGrid.ShowColumnWhenGrouped)
                            {
                                var groupedColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == groupColumn.ColumnName);
                                if (groupedColumn != null && groupedColumn.IsHidden)
                                    groupedColumn.IsHidden = false;
                            }
#if SILVERLIGHT
                            if (!isGroupDescriptionMoved)
#endif
                            this.RemoveGroupDropItem(groupColumn.ColumnName);
                        }
                        isChanged = true;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.View.BeginInit();
                        var sortDescs = this.GetSortDescriptionNotInGroupDescription();
                        var groupedColumns = this.View.GroupDescriptions.ToList();
                        this.View.GroupDescriptions.Clear();
                        this.View.SortDescriptions.Clear();
                        foreach (var desc in sortDescs)
                            this.View.SortDescriptions.Add(desc);
                        this.RemoveAllGroupDropItems();
                        if (!this.dataGrid.ShowColumnWhenGrouped)
                        {
                            foreach (var item in groupedColumns)
                            {
                                var groupedColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == ((PropertyGroupDescription)item).PropertyName);
                                if (groupedColumn != null)
                                    groupedColumn.IsHidden = false;
                            }
                        }
                        isChanged = true;
                    }
                    break;
#if !SILVERLIGHT && !WP7
                case NotifyCollectionChangedAction.Move:
                    {
                        this.View.BeginInit();
                        this.dataGrid.View.GroupDescriptions.Move(e.OldStartingIndex, e.NewStartingIndex);
                        isChanged = true;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    var oldgroupedColumn = e.OldItems[0] as GroupColumnDescription;
                    var replaceColumn = e.NewItems[0] as GroupColumnDescription;
                    if (this.dataGrid.Columns.Any(col => col.MappingName == replaceColumn.ColumnName) && oldgroupedColumn.ColumnName != replaceColumn.ColumnName)
                    {
                        this.View.BeginInit();
                        this.View.GroupDescriptions[e.OldStartingIndex] = new PropertyGroupDescription(replaceColumn.ColumnName, replaceColumn.Converter);
                        this.View.SortDescriptions[e.OldStartingIndex] = new SortDescription(replaceColumn.ColumnName, ListSortDirection.Ascending);
#if SILVERLIGHT
                        if (!isGroupDescriptionMoved)
                        {
#endif
                        this.RemoveGroupDropItem(oldgroupedColumn.ColumnName);
                        this.AddGroupDropItem(replaceColumn.ColumnName, e.NewStartingIndex);
#if SILVERLIGHT
                        }
#endif
                        isChanged = true;
                    }
                    break;
#endif

            }
            if (isChanged)
            {
#if !WP
                this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
                this.View.EndInit();
                if (dataGrid.VisualContainer != null)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                        ResetColumns();
                    dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, e));
                    dataGrid.UpdateRowCountAndScrollBars();
                }
            }
            Resume();
            this.isGroupColumnChanged = false;
        }

        /// <summary>
        /// Method which helps to update the view when  change the GroupDescription collection
        /// </summary>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void OnGroupDescriptionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool isChanged = false;
            if (isSuspended || isGroupColumnChanged)
                return;
            this.isGroupDescriptionChanged = true;
            Suspend();
#if !WP
            this.dataGrid.SelectionController.CurrentCellManager.EndEdit();
            addNewRowController.CommitAddNew();
#endif


            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (PropertyGroupDescription description in e.NewItems)
                        {
                            if (this.View.GroupDescriptions.Count(desc => ((PropertyGroupDescription)desc).PropertyName == description.PropertyName) > 1)
                            {
                                throw new InvalidOperationException("GroupDescription already exist in View.GroupDescriptions");
                            }
                            if (this.CheckForExistingGroupColumn(description.PropertyName))
                                continue;

                            var gridColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == description.PropertyName);

#if WP
                            if (View != null && this.dataGrid.CheckColumnNameinItemProperties(description.PropertyName) && !gridColumn.IsUnbound)
#else
                            if (View != null && this.dataGrid.CheckColumnNameinItemProperties(description.PropertyName) && !gridColumn.IsUnbound && !this.View.IsDynamicBound)
#endif
                            {
                                View.BeginInit();
                                this.dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = description.PropertyName, Converter = description.Converter });
                                this.AddGroupDropItem(description.PropertyName);
                                if (this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == description.PropertyName) == default(SortDescription))
                                    this.View.SortDescriptions.Add(new SortDescription(description.PropertyName, ListSortDirection.Ascending));
                                if (!this.dataGrid.ShowColumnWhenGrouped)
                                {
                                    var groupedColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == description.PropertyName);
                                    if (groupedColumn != null)
                                        groupedColumn.IsHidden = true;
                                }
                                isChanged = true;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    View.BeginInit();
                    foreach (PropertyGroupDescription description in e.OldItems)
                    {
                        var groupColumn = this.dataGrid.GroupColumnDescriptions.First(col => col.ColumnName == description.PropertyName);
                        this.dataGrid.GroupColumnDescriptions.Remove(groupColumn);
                        this.RemoveGroupDropItem(description.PropertyName);
                        var sortDescription = this.View.SortDescriptions.FirstOrDefault(desc => desc.PropertyName == description.PropertyName);
                        if (sortDescription != default(SortDescription))
                            this.View.SortDescriptions.Remove(sortDescription);
                        if (!this.dataGrid.ShowColumnWhenGrouped)
                        {
                            var groupedColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == description.PropertyName);
                            if (groupedColumn != null && groupedColumn.IsHidden)
                                groupedColumn.IsHidden = false;
                        }
                        isChanged = true;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var sortDescs = this.GetSortDescriptionNotInGroupDescription();
                    var groupedColumns = this.dataGrid.GroupColumnDescriptions.ToList();
                    this.View.BeginInit();
                    this.dataGrid.GroupColumnDescriptions.Clear();
                    this.View.SortDescriptions.Clear();
                    foreach (var desc in sortDescs)
                        this.View.SortDescriptions.Add(desc);
                    this.RemoveAllGroupDropItems();
                    if (!this.dataGrid.ShowColumnWhenGrouped)
                    {
                        foreach (var item in groupedColumns)
                        {
                            var groupedColumn = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == item.ColumnName);
                            if (groupedColumn != null)
                                groupedColumn.IsHidden = false;
                        }
                    }
                    isChanged = true;
                    break;
            }

            if (isChanged)
            {
#if !WP
                this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
                View.EndInit();
                if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                    this.ResetColumns();
                this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Grouping, e));
                this.dataGrid.UpdateRowCountAndScrollBars();
            }
            Resume();
            this.isGroupDescriptionChanged = false;
        }

        #endregion

        #endregion

        #region Summaries Codes

        /// <summary>
        /// Method which helps to update the TableSummary Values when  change the Record Property Change
        /// </summary>
        /// <param name="e">An <see cref="T:System.ComponentModel.PropertyChangedEventArgs">PropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var dataRowBase = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowData == sender);
            if (dataGrid.HasUnboundColumns)
            {
                if (this.dataGrid.RowGenerator.Items.Any(
                        row => row.VisibleColumns.Any(col => col.GridColumn != null && col.GridColumn.IsUnbound)))
                {

                    if (dataRowBase != null)
                    {
                        dataRowBase.VisibleColumns.ForEach(col =>
                            {
                                if (col.GridColumn != null && col.GridColumn.IsUnbound)
                                {
                                    var column = col.GridColumn as GridUnBoundColumn;
                                    if (column.Expression.ToLower().Contains(e.PropertyName.ToLower()) ||
                                        column.Format.ToLower().Contains(e.PropertyName.ToLower()))
                                    {
                                        col.UpdateBinding(dataRowBase.RowData);
                                    }
                                }
                            });
                    }
                }
            }

#if !WP
            if (dataRowBase != null && this.dataGrid.GridValidationMode != GridValidationMode.None)
            {
                var dc = dataRowBase.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == e.PropertyName);
                if (dc != null && !dc.IsEditing)
                {
                    (dataRowBase as GridDataRow).ApplyRowHeaderVisualState();
                    this.dataGrid.Validations.ValidateColumn(dataRowBase.RowData, e.PropertyName, dc.ColumnElement as GridCell, new RowColumnIndex(dc.RowIndex, dc.ColumnIndex));
                }
            }
#endif
            if (dataGrid.LiveDataUpdateMode == LiveDataUpdateMode.Default)
                return;

            if (this.dataGrid.View.TableSummaryRows.Count > 0)
            {
                var tableSummaryRows = this.dataGrid.RowGenerator.Items.Where(item => (item.RowType == RowType.TableSummaryCoveredRow || item.RowType == RowType.TableSummaryRow));
                foreach (SpannedDataRow row in tableSummaryRows)
                {
                    var record = row.RowData as SummaryRecordEntry;
                    this.UpdateSummaryCells(row, record, e.PropertyName);
                }
            }

            if (!this.HasGroup || this.dataGrid.LiveDataUpdateMode == LiveDataUpdateMode.AllowSummaryUpdate ||
              this.dataGrid.GroupColumnDescriptions.All(col => col.ColumnName != e.PropertyName)
#if WPF
 || (this.View.SourceCollection is IBindingList)
#endif
)
            {
                if (this.dataGrid.View.SummaryRows.Count > 0)
                {
                    var groupSummaryRows = this.dataGrid.RowGenerator.Items.Where(item => (item.RowType == RowType.SummaryCoveredRow || item.RowType == RowType.SummaryRow) && item.RowVisibility == Visibility.Visible);
                    foreach (SpannedDataRow row in groupSummaryRows)
                    {
                        var record = row.RowData as SummaryRecordEntry;
                        this.UpdateSummaryCells(row, record, e.PropertyName);
                    }
                }

                if (this.dataGrid.View.CaptionSummaryRow != null)
                {
                    var captionSummaryRows = this.dataGrid.RowGenerator.Items.Where(item => (item.RowType == RowType.CaptionCoveredRow || item.RowType == RowType.CaptionRow) && item.RowVisibility == Visibility.Visible);
                    foreach (SpannedDataRow row in captionSummaryRows)
                    {
                        var record = (row.RowData as Group).SummaryDetails;
                        this.UpdateSummaryCells(row, record, e.PropertyName);
                    }
                }
            }
        }

        /// <summary>
        /// Method which helps to update the view when  change the Table Summary Rows collection
        /// </summary>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void OnTableSummaryRowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !WP
            if (this.dataGrid.NotifyListener != null) 
                this.dataGrid.NotifyListener.NotifyCollectionChanged(this.dataGrid.TableSummaryRows, e, datagrid => datagrid.TableSummaryRows, this.dataGrid, typeof(GridSummaryRow));
#endif
            if (this.View == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (ISummaryRow row in e.NewItems)
                        {
                            this.View.TableSummaryRows.Add(row);
                            if (row is GridTableSummaryRow && (row as GridTableSummaryRow).Position == TableSummaryRowPosition.Top)
                            {
                                SetTableSummaryPositionChangedAction(row as GridTableSummaryRow);
                                this.dataGrid.headerLineCount += 1;
                                this.dataGrid.VisualContainer.FrozenRows += 1;
                            }
                            else
                                this.dataGrid.VisualContainer.FooterRows += 1;
                        }
                        this.RefreshView();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (ISummaryRow row in e.OldItems)
                        {
                            this.View.TableSummaryRows.Remove(row);
                            var needRemoveRecord = this.View.Records.TableSummaries.FirstOrDefault(record => record.SummaryRow == row);
                            this.View.Records.TableSummaries.Remove(needRemoveRecord);
                            if (row is GridTableSummaryRow && (row as GridTableSummaryRow).Position == TableSummaryRowPosition.Top)
                            {
                                (row as GridTableSummaryRow).TableSummaryPositionChanged = null;
                                this.dataGrid.headerLineCount -= 1;
                                this.dataGrid.VisualContainer.FrozenRows -= 1;
                            }
                            else
                                this.dataGrid.VisualContainer.FooterRows -= 1;
                        }
                        this.RefreshView();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.View.TableSummaryRows.Clear();
                        this.View.Records.TableSummaries.Clear();
                        this.dataGrid.VisualContainer.FooterRows = 0;
                        this.RefreshView();
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        this.View.TableSummaryRows[e.OldStartingIndex] = e.NewItems[0] as ISummaryRow;
                        this.View.Records.TableSummaries[e.OldStartingIndex].SummaryRow = e.NewItems[0] as ISummaryRow;
                        this.RefreshView();
                    }
                    break;
            }
        }

        /// <summary>
        /// Method which helps to update the view when  change the SummaryRows collection
        /// </summary>
        /// <param name="e">An <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs">NotifyCollectionChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void OnSummaryRowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !WP
            if (this.dataGrid.NotifyListener != null) 
                this.dataGrid.NotifyListener.NotifyCollectionChanged(this.dataGrid.GroupSummaryRows, e, datagrid => datagrid.GroupSummaryRows, this.dataGrid, typeof(GridSummaryRow));
#endif
            if (this.View == null)
            {
                return;
            }
            this.View.BeginInit();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (ISummaryRow row in e.NewItems)
                            this.View.SummaryRows.Add(row);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (ISummaryRow row in e.OldItems)
                            this.View.SummaryRows.Remove(row);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.View.SummaryRows.Clear();
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        this.View.SummaryRows[e.OldStartingIndex] = e.NewItems[0] as ISummaryRow;
                    }
                    break;
            }
#if !WP
            this.dataGrid.DetailsViewManager.ResetExpandedDetailsView();
#endif
            this.View.EndInit();
            this.RefreshView();
        }

        /// <summary>
        /// Method which helps to update the view when change the Caption Summary Row
        /// </summary>
        /// <param name="row"></param>
        /// <remarks></remarks>
        internal void OnCaptionSummaryRowChanged(GridSummaryRow row)
        {
            var captionRows = this.dataGrid.RowGenerator.Items.Where(item => item.RowType == RowType.CaptionCoveredRow || item.RowType == RowType.CaptionRow);
            if (!captionRows.Any()) 
                return;

            captionRows.ForEach(caption => caption.RowIndex = -1);
            this.dataGrid.VisualContainer.InvalidateMeasureInfo();
        }

        internal void UpdateTableSummaries()
        {
            var footerRows = this.dataGrid.RowGenerator.Items.Where(item => item.RowRegion == RowRegion.Footer);
            foreach (var row in footerRows)
            {
                foreach (var column in row.VisibleColumns)
                    column.UpdateBinding(row.RowData, false);
            }
        }

        internal void UpdateHeaderCells(bool updateCellStyle = true)
        {
            var headerRows = this.dataGrid.RowGenerator.Items.Where(item => item.RowRegion == RowRegion.Header && item.RowIndex == dataGrid.GetHeaderIndex());
            foreach (var headerRow in headerRows)
            {
                foreach (var column in headerRow.VisibleColumns)
                    column.UpdateBinding(headerRow.RowData, updateCellStyle);
            }
        }

        private void UpdateSummaryCells(DataRowBase row, SummaryRecordEntry record, string propertyName)
        {
            if (record.SummaryRow.SummaryColumns.Any(summaryColumn => summaryColumn.MappingName == propertyName))
            {
                if (record.SummaryRow.ShowSummaryInRow)
                {
                    row.VisibleColumns.ForEach(col => col.UpdateCellStyle());
                }
                else
                {
                    var colum = row.VisibleColumns.FirstOrDefault(column => column.GridColumn != null && column.GridColumn.MappingName == propertyName);
                    if (colum != null)
                        colum.UpdateCellStyle();
                }
            }
        }

        private void UpdateBindingTableSummary(DataRowBase dr)
        {
            if (dr.RowType == RowType.TableSummaryCoveredRow || dr.RowType == RowType.TableSummaryRow)
                dr.VisibleColumns.ForEach(col => col.UpdateBinding(dr.RowData, false));
        }

        internal void InitializeGridTableSummaryRow()
        {
            this.dataGrid.TableSummaryRows.Where(row => row is GridTableSummaryRow).ForEach(tRow => SetTableSummaryPositionChangedAction(tRow as GridTableSummaryRow));
        }

        private void SetTableSummaryPositionChangedAction(GridTableSummaryRow row)
        {
            if (row.TableSummaryPositionChanged == null)
                row.TableSummaryPositionChanged = OnTableSummaryPositionChanged;
        }

        private void OnTableSummaryPositionChanged(GridSummaryRow summaryRow, TableSummaryRowPosition position)
        {
            var index = dataGrid.TableSummaryRows.IndexOf(summaryRow);
            if (position == TableSummaryRowPosition.Bottom)
            {
                dataGrid.headerLineCount -= 1;
                var tableSummaryRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowRegion == RowRegion.Header && (item.RowType == RowType.TableSummaryCoveredRow || item.RowType == RowType.TableSummaryRow));
                tableSummaryRow.RowIndex = -1;
                tableSummaryRow.RowRegion = RowRegion.Footer;
                dataGrid.VisualContainer.FooterRows += 1;

                if (tableSummaryRow.WholeRowElement is TableSummaryRowControl)
                    (tableSummaryRow.WholeRowElement as TableSummaryRowControl).TableSummaryRowType = TableSummaryRowType.FooterSummaryRow;

                var lastSummaryRow = dataGrid.RowGenerator.Items.LastOrDefault(row => row.RowRegion == RowRegion.Footer && (row.WholeRowElement is TableSummaryRowControl && (row.WholeRowElement as TableSummaryRowControl).TableSummaryRowType == TableSummaryRowType.LastFooterSummaryRow));
                if (lastSummaryRow != null)
                    (lastSummaryRow.WholeRowElement as TableSummaryRowControl).TableSummaryRowType = TableSummaryRowType.FooterSummaryRow;
            }
            else
            {
                dataGrid.headerLineCount += 1;
                var tableSummaryRow = dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowRegion == RowRegion.Footer && item.WholeRowElement is TableSummaryRowControl);
                tableSummaryRow.RowIndex = -1;
                tableSummaryRow.RowRegion = RowRegion.Header;
                dataGrid.VisualContainer.FooterRows -= 1;

                if (tableSummaryRow.WholeRowElement is TableSummaryRowControl)
                    (tableSummaryRow.WholeRowElement as TableSummaryRowControl).TableSummaryRowType = TableSummaryRowType.HeaderSummaryRow;

            }
            var addNewRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.IsAddNewRow);
            if (addNewRow != null)
                addNewRow.RowIndex = -1;
            dataGrid.VisualContainer.FrozenRows = dataGrid.HeaderLineCount;
            RefreshView();
            dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.TableSummary, null));
        }

        #endregion

        #region Helper Methods

        private static bool CheckControlKeyPressed()
        {
#if WinRT
            return (Window.Current.CoreWindow.GetAsyncKeyState(Key.Control).HasFlag(CoreVirtualKeyStates.Down));
#elif WPF
            return ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)));
#else
            return (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control));
#endif
        }

        #endregion

        #region Filters

#if !WP
        internal void InitialFiltering()
        {
            if (this.View == null)
                return;

            foreach (var column in this.dataGrid.Columns.Where(c => c.FilterPredicates.Any()))
            {
                if (!this.dataGrid.CheckColumnNameinItemProperties(column.MappingName) && !column.IsUnbound && !this.View.IsDynamicBound)
                    column.FilterPredicates.Clear();
            }

            if (this.View != null && this.dataGrid.Columns.Any(column => column.FilterPredicates.Any()))
            {
                ApplyFilter();
            }
        }
#endif

        internal void OnGridColumnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;
            foreach (GridColumn column in e.NewItems)
            {
                WireColumnDescriptor(column);
            }
        }

        internal void WireColumnDescriptor(GridColumn column)
        {
            if (column.DataGrid != null) return;
#if !WP
            column.FilterPredicates.CollectionChanged -= OnFiltersCollectionChanged;
            column.FilterPredicates.CollectionChanged += OnFiltersCollectionChanged;
#endif
            column.SetGrid(this.dataGrid);
        }

#if !WP
        private void OnFiltersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var column = this.dataGrid.Columns.FirstOrDefault(x => x.FilterPredicates.Equals(sender));
            if (column == null)
                return;
#if !WP
            if (this.dataGrid.NotifyListener != null)
                this.dataGrid.NotifyListener.NotifyCollectionChanged(column.FilterPredicates, e, datagrid => datagrid.Columns.FirstOrDefault(x => x.MappingName == column.MappingName).FilterPredicates, this.dataGrid, typeof(FilterPredicate));
#endif
            if (!FilterSuspend)
            {
                this.ApplyFilter();
                if (this.dataGrid.RowGenerator.Items.Count > 0)
                {
                    var dataRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.dataGrid.GetHeaderIndex());
                    var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == column.MappingName && x.GridColumn.Equals(column));
                    if (dataColumn != null)
                    {
                        var header = dataColumn.ColumnElement as GridHeaderCellControl;
                        header.ApplyFilterToggleButtonVisualState();
                    }
                }
            }
        }

        internal void ApplyFilter()
        {
            var filterPredicates = this.dataGrid.Columns.OfType<IFilterDefinition, GridColumn>();
            if (filterPredicates.Count > 0 && this.View != null)
                this.View.FilterPredicates = filterPredicates;
        }

        public void FilterColumn(GridColumn column, List<FilterPredicate> filterPredicates)
        {
            if (column == null)
                return;

            if (filterPredicates == null)
            {
                this.dataGrid.View.FilterPredicates = GetFilters();
                return;
            }

            var args = new GridFilterEventArgs(column, filterPredicates.Count == 0 ? null : filterPredicates, this.dataGrid);
            if (!dataGrid.RaiseFilterChanging(args))
            {
                FilterSuspend = true;
                this.ClearFilters(column);
                if (filterPredicates.Count > 0)
                    filterPredicates.ForEach(x => column.FilterPredicates.Add(x));
                if (this.dataGrid.RowGenerator.Items.Count > 0)
                {
                    var dataRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == this.dataGrid.GetHeaderIndex());
                    var dataColumn = dataRow.VisibleColumns.FirstOrDefault(x => x.GridColumn != null && x.GridColumn.MappingName == column.MappingName && x.GridColumn.Equals(column));
                    if (dataColumn != null)
                    {
                        var header = dataColumn.ColumnElement as GridHeaderCellControl;
                        header.ApplyFilterToggleButtonVisualState();
                    }
                }
                FilterSuspend = false;
            }

            this.dataGrid.View.FilterPredicates = GetFilters();
            if (!args.Handled)
                dataGrid.RaiseFilterChanged(args);
        }

        internal ObservableCollection<IFilterDefinition> GetFilters()
        {
            return this.dataGrid.Columns.OfType<IFilterDefinition, GridColumn>();
        }

        internal void ClearFilters(GridColumn column)
        {
            if (column.FilterPredicates.Count > 0)
            {
                if (!FilterSuspend && this.dataGrid.RaiseFilterChanging(new GridFilterEventArgs(column, null, this.dataGrid)))
                    return;

                column.FilterPredicates.Clear();
                if (!FilterSuspend)
                    this.dataGrid.RaiseFilterChanged(new GridFilterEventArgs(column, null, this.dataGrid));
            }
        }
#endif
        #endregion

        #region Dispose Method

        public void Dispose()
        {
            //UnwireEvents called from DataGrid UnWireEvents method. no need to call here
            //this.UnWireEvents();
            if (this.dataGrid != null)
            {
                if (this.dataGrid.GroupColumnDescriptions != null)
                {
                    this.dataGrid.GroupColumnDescriptions.Clear();
                    this.dataGrid.ClearValue(SfDataGrid.GroupColumnDescriptionsProperty);
                }

                if (this.dataGrid.SortColumnDescriptions != null)
                {
                    this.dataGrid.SortColumnDescriptions.Clear();
                    this.dataGrid.ClearValue(SfDataGrid.SortColumnDescriptionsProperty);
                }
                if (this.dataGrid.SortComparers != null)
                {
                    this.dataGrid.SortComparers.Clear();
                    this.dataGrid.ClearValue(SfDataGrid.SortComparersProperty);
                }
            }
#if !WP
            if (addNewRowController != null)
            {
                addNewRowController.Dispose();
                addNewRowController = null;
            }
#endif
            this.dataGrid = null;
        }

        #endregion
    }
}