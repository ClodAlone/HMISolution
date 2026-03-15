#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using Syncfusion.Data.Extensions;

#if !WinRT
using System.Windows.Data;
#endif

namespace Syncfusion.Data
{
    public class PagedCollectionView : CollectionViewAdv, IPagedCollectionView, IFilterExt
    {
        #region Private Members

        private int pageIndex;
        private int pageSize;
        private int maxItemsCount;
        private bool isInGetInternalList;

        private bool useOnDemandPaging;

        private List<int> virtualLoadedIndexes = new List<int>();

        #endregion

        #region Internal Members

        internal IList InternalList;

        public int PageCount
        {
            get { return (this.PageSize > 0) ? Math.Max(1, (int) Math.Ceiling((double) this.ItemCount/this.PageSize)) : 0; }
        }

        #endregion

        #region Public Members

        public int MaxItemsCount
        {
            get { return maxItemsCount; }
            set
            {
                maxItemsCount = value;
                this.RefreshSource();
            }
        }

        public bool UseOnDemandPaging
        {
            get { return useOnDemandPaging; }
            set { useOnDemandPaging = value; }
        }

        public override bool CanAddNew
        {
            get
            {
                return false;
            }
        }
        public override bool EnableSummaryOptimization
        {
            get
            {
                return false;
            }           
        }
        #endregion

        #region Public Events

        public event OnDemandItemsLoadingEventHandler OnDemandItemsLoading;

        public event PageChangingEventHandler PageChanging;

        public event PageChangedEventHandler PageChanged;

        #endregion

        #region Ctor

        public PagedCollectionView(IEnumerable source) : base(source)
        {
            if (!UseOnDemandPaging)
                this.InitializeInternalList(source);
        }

        public PagedCollectionView()
        {
            this.InternalList = new List<object>();
            this.SetSource(this.InternalList);
            this.InitiateCollectionViewAdv();
        }

        #endregion

        #region Overrides

        public override IEnumerator<object> GetEnumerator()
        {
            if (this.PageSize > 0)
            {
                var pagedSource = this.InternalList.AsQueryable().Page(this.PageIndex, this.pageSize);
                return pagedSource.Cast<object>().GetEnumerator();
            }
            else
            {
                return this.InternalList.Cast<object>().GetEnumerator();
            }
        }

        protected override IRecordsList CreateRecords()
        {
            return EnumerableRecordsWrapper.CreateNew(this.InternalList, this);
        }

        protected override void OnSortDescriptionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSort();
            base.OnSortDescriptionChanged(e);
        }

        protected override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            var groupSortDescriptions = this.SortDescriptions.Where(item => groupBy.Any(groupName => groupName == item.PropertyName));
            var sortDesc = this.SortDescriptions.Except(groupSortDescriptions);
            IQueryable queryableSource=null;

            if (InternalList.Count > 0)
            {
                if (!UseOnDemandPaging)
                {
                    queryableSource = this.InternalList.OfQueryable(this.SourceType).AsQueryable();
                    queryableSource = this.SortQueryable(queryableSource, sortDesc.ToList());
                    queryableSource = this.SortQueryable(queryableSource, groupSortDescriptions.ToList());
                    this.InternalList = queryableSource.Cast<object>().ToList();
                    queryableSource = queryableSource.Page(this.PageIndex, PageSize);
                }
                else
                {
                    queryableSource = this.InternalList.AsQueryable().Page(this.PageIndex, PageSize).OfQueryable(this.SourceType).AsQueryable();
                    queryableSource = this.SortQueryable(queryableSource, sortDesc.ToList());
                    queryableSource = this.SortQueryable(queryableSource, groupSortDescriptions.ToList());
                }

                Func<string, Expression> groupConverterFunc = (property) => this.GetGroupConverterExpressionFunc(property);
                var hasGroupConverter =
                    this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.Converter != null) !=
                    null;

                var result =
                    queryableSource.GroupByMany(this.SourceType,
                                                hasGroupConverter
                                                    ? groupConverterFunc
                                                    : (property) => this.GetExpressionFunc(property), groupBy).ToList();

                return result;
            }
            return null;
        }

        private Predicate<object> rowFilter;
        public Predicate<object> RowFilter
        {
            get { return rowFilter; }
            set { rowFilter = value; }
        }

        public override bool FilterRecord(object record)
        {
            return (this.Filter == null || this.Filter(record)) && (this.RowFilter == null || this.RowFilter(record));
        }

        public override void RefreshFilter()
        {
            var filterPresent = this.FilterPredicates.Any(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0);
            if (filterPresent)
            {
                var source = this.SourceCollection.AsQueryable();
                ParameterExpression paramExpression;
                Expression predicate = this.GetPredicateExpression(source, out paramExpression);
                if (paramExpression != null && predicate != null)
                {
                    var lambda = Expression.Lambda(predicate, paramExpression);
                    var delg = lambda.Compile();
                    this.RowFilter = (o) =>
                    {
                        var result = (bool)delg.DynamicInvoke(o);
                        return result;
                    };
                }
                else
                    this.RowFilter = null;
            }
            else
            {
                this.RowFilter = null;
            }

            if(UseOnDemandPaging)
                return;
            this.UpdateItems();
            if (this.IsGrouping && !UseOnDemandPaging && !IsInEndeferal)
            {
                this.RefreshTopLevelGroup();

                var hasSort = this.SortDescriptions.Count > 0;
                if (hasSort)
                {
                    this.RefreshSort();
                }
            }

            if (!IsInEndeferal)
                this.Refresh();
            this.OnPropertyChanged("ItemsCount");
        }

        public virtual Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression)
        {
            return this.GetPredicateExpressionExt(source, out paramExpression);
        }

        public Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression, string columnName, bool returncolExpression)
        {
            return this.GetPredicateExpressionExt(source, out paramExpression, columnName, returncolExpression);
        }

        protected override void RefreshSort()
        {
            var filterPresent =
                this.FilterPredicates.Any(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0);
            if (!IsGrouping)
            {
                if (!filterPresent || this.Filter!=null)
                    ApplySort();
            }
            else
            {
                if (this.SortDescriptions.Count > 0)
                {
                    this.TopLevelGroup.SuspendEvents();
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshSortingOrder();
                    this.TopLevelGroup.ResumeEvents();
                }
            }
        }

        protected override void RemoveRecord(object record)
        {
            
        }

        protected override void UpdateCollectionView(object sender, NotifyCollectionChangedEventArgs e)
        {
            int currentPageLastIndex = (this.PageIndex*this.PageSize) + (this.PageSize - 1);
            int currentPageStartIndex = this.PageIndex*this.PageSize;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            AddRecord(e.NewItems[i], e.NewStartingIndex+i, currentPageStartIndex, currentPageLastIndex); 
                        }
                        this.OnPropertyChanged("ItemsCount");
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            RemoveRecord(e.OldItems[i], e.OldStartingIndex+i, currentPageStartIndex, currentPageLastIndex);
                        }
                        this.OnPropertyChanged("ItemsCount");
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var olditem = e.OldItems[0];
                        var recordIndex = this.Records.IndexOfRecord(olditem);
                        var newitem = e.NewItems[0];
                        var newindex = e.NewStartingIndex;

                        if (this.FilterRecord(newitem))
                        {
                            if (this.LiveDataUpdateMode != LiveDataUpdateMode.AllowDataShaping)
                            {
                                if (e.OldStartingIndex > -1 && e.OldStartingIndex < this.InternalList.Count)
                                {
                                    this.InternalList[e.OldStartingIndex] = newitem;
                                    if (recordIndex >= 0)
                                    {
                                        this.Records[recordIndex] = this.CreateRecordEntry(newitem);
                                    }
                                }
                            }
                            else
                            {
                                this.RemoveRecord(olditem, e.OldStartingIndex, currentPageStartIndex, currentPageLastIndex);
                                this.AddRecord(newitem, e.NewStartingIndex, currentPageStartIndex, currentPageLastIndex);
                                return;
                            }
                        }
                        else
                        {
                            if (e.OldStartingIndex > -1 && e.OldStartingIndex < this.InternalList.Count)
                            {
                                this.RemoveRecord(olditem, e.OldStartingIndex, currentPageStartIndex, currentPageLastIndex);
                            }
                        }
                        if (IsInSourceCollectionChange)
                        {
                            RemoveNotifyListener(olditem);
                            AddNotifyListener(newitem);
                        }
                    }
                    break;
#if !SILVERLIGHT && !WINDOWS_PHONE8 && !WINDOWS_PHONE
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException("Move action not implemented in UpdateCollectionView");
#endif
                case NotifyCollectionChangedAction.Reset:
                    this.InternalList.Clear();
                    this.Refresh();
                    this.OnPropertyChanged("ItemsCount");
                    break;
            }
        }

        internal override void UpdateGroupingModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            int currentPageLastIndex = (this.PageIndex * this.PageSize) + (this.PageSize - 1);
            int currentPageStartIndex = this.PageIndex * this.PageSize;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            AddGroupRecord(item,e.NewStartingIndex,currentPageStartIndex,currentPageLastIndex);
                        }
                        this.OnPropertyChanged("ItemsCount");
                    }
                    break;
#if !SILVERLIGHT && !WINDOWS_PHONE
                case NotifyCollectionChangedAction.Move:
                    this.GroupList.Remove(e.NewItems[0], IsInSourceCollectionChange);
                    this.GroupList.Add(e.NewItems[0], IsInSourceCollectionChange);
                    break;
#endif
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            RemoveGroupRecord(item, e.OldStartingIndex, currentPageStartIndex, currentPageLastIndex);
                        }
                        this.OnPropertyChanged("ItemsCount");
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                   
                    RemoveNotifyListener(e.OldItems[0]);
                    this.RemoveGroupRecord(e.OldItems[0], e.OldStartingIndex, currentPageStartIndex, currentPageLastIndex);
                    this.AddGroupRecord(e.NewItems[0], e.NewStartingIndex, currentPageStartIndex, currentPageLastIndex);
                    AddNotifyListener(e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.InternalList.Clear();
                    this.EndDeferInternal();
                    this.OnPropertyChanged("ItemsCount");
                    break;
            }
        }

        internal override void UpdateTableSummary()
        {
            if (this.TableSummaryRows == null || this.TableSummaryRows.Count == 0)
            {
                return;
            }

            int counter = 0;
            foreach (var summaryRow in this.TableSummaryRows)
            {
                SummaryRecordEntry summaryRecordEntry = null;
                if (counter < this.Records.TableSummaries.Count)
                {
                    summaryRecordEntry = this.Records.TableSummaries[counter];
                }
                else
                {
                    summaryRecordEntry = new SummaryRecordEntry(null, -2) {SummaryRow = summaryRow};
                    this.Records.TableSummaries.Add(summaryRecordEntry);
                }

                IEnumerable items;
                if (UseOnDemandPaging)
                    items = this.Records.Select(rec => rec.Data).OfQueryable(this.SourceType);
                else
                    items = this.InternalList.OfQueryable(this.SourceType);
                    //this.SourceCollection;//this.Records.Select(o => o.Data).ToArray().OfQueryable();
                foreach (var summaryColumn in summaryRow.SummaryColumns)
                {
                    if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
                    {
                        var summaryItems = new Dictionary<string, object>();
                        SummaryCreator.RaiseQuerySummaryAggregate(items, summaryColumn, summaryItems, this);
                        SummaryValue summaryValue =
                            summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                        if (summaryValue == null)
                        {
                            summaryValue = new SummaryValue() {Name = summaryColumn.Name};
                            summaryRecordEntry.SummaryValues.Add(summaryValue);
                        }

                        foreach (var kvp in summaryItems)
                        {
                            object summaryResult = null;
                            if (summaryValue.AggregateValues.TryGetValue(kvp.Key, out summaryResult))
                            {
                                summaryValue.AggregateValues[kvp.Key] = kvp.Value;
                            }
                            else
                            {
                                summaryValue.AggregateValues.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }
                }
                counter++;
            }
        }

        public override void CommitEdit()
        {
            if (IsInCommitEdit)
                return;

            if (this.editItem == null)
                throw new InvalidOperationException("IsEditingItem is not true to EndEdit");

            IsInCommitEdit = true;
            if (editItem is IEditableObject)
                (editItem as IEditableObject).EndEdit();

            var refreshtablesummary = false;
#if WPF
            if (this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && !this.IsLegacyDataTable)
#else
            if (this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping)
#endif
            {
                var passesFilter = true;
                if (this.CanFilter)
                    passesFilter = this.FilterRecord(editItem);

                var removeAtIndex = this.InternalList.IndexOf(editItem);
                if (!passesFilter)
                {
                    if (removeAtIndex > -1 && removeAtIndex < this.InternalList.Count)
                    {
                        var removeList = new List<object>();
                        removeList.Add(editItem);
                        if (!IsGrouping)
                        {

                            var collectionChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removeList, removeAtIndex);
                            this.UpdateCollectionView(this, collectionChangedArgs);
                        }
                        else
                        {
                            RemoveRecordFromGroup(removeList, -1);
                        }
                        this.RemoveRecord(editItem);
                        refreshtablesummary = true;
                    }
                }
                else
                {
                    if (this.IsGrouping)
                    {
                        var record = this.Records.GetRecord(editItem);
                        removeAtIndex = this.TopLevelGroup.DisplayElements.IndexOf((record));

                        var canreset = true;
                        var comparedIndex = -1;
                        if (this.TopLevelGroup.IsValidGroup(record.Parent as Group, editItem))
                        {
                            canreset = false;
                        }

                        if (canreset)
                        {
                            if (removeAtIndex < this.TopLevelGroup.DisplayElements.Count())
                                this.RemoveRecordFromGroup(editItem, -1);
                            else
                                throw new InvalidOperationException("EditItem is not in View to Commit");
                            this.AddRecordToGroup(editItem, comparedIndex);
                        }
                    }
                }
            }

            if (refreshtablesummary && this.LiveDataUpdateMode != LiveDataUpdateMode.Default)
                this.UpdateTableSummary();
            editItem = null;
            IsInCommitEdit = false;
        }

        protected override void NotifyPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            IsInPropertyChange = true;
            NotifyCollectionChangedEventArgs collectionChangedArgs = null;
            var propertyChangedArgs = e as PropertyChangedEventArgs;

            if ((propertyChangedArgs.PropertyName == null || propertyChangedArgs.PropertyName == string.Empty))
            {
                if (this.SortDescriptions.Count > 0)
                    propertyChangedArgs = new PropertyChangedEventArgs(this.SortDescriptions[0].PropertyName);
                if (this.TableSummaryRows != null && this.TableSummaryRows.Count > 0)
                    propertyChangedArgs = new PropertyChangedEventArgs(this.TableSummaryRows[0].SummaryColumns[0].MappingName.ToString());
                if (this.SummaryRows != null && this.SummaryRows.Count > 0)
                    propertyChangedArgs = new PropertyChangedEventArgs(this.SummaryRows[0].SummaryColumns[0].MappingName);
            }
            
            var hasGroup = this.GroupDescriptions != null && this.GroupDescriptions.Count > 0 && this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.PropertyName == propertyChangedArgs.PropertyName) != null;

            bool canRefreshTableSummary = true;
            bool forceRefreshTableSummary = false;
            bool canRefreshSummary = true;
            bool forceRefreshSummary = false;

            if (!this.IsGrouping)
            {
                if (this.CanFilter && sender != null && this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && !sender.Equals(this.CurrentEditItem))
                {
                    var passesFilter = this.FilterRecord(sender);
                    if (!passesFilter)
                    {
                        var removedList = new List<object>();
                        removedList.Add(sender);
                        var removeAtIndex = this.Records.IndexOfRecord(sender);
                        if (removeAtIndex < 0 || removeAtIndex > this.Records.Count)
                        {
                            removeAtIndex = this.InternalList.IndexOf(sender);
                        }

                        if (removeAtIndex > -1 && removeAtIndex < this.InternalList.Count)
                        {
                            forceRefreshTableSummary = true;
                        }
                        else
                            canRefreshTableSummary = false;

                        collectionChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedList,removeAtIndex);
                        this.UpdateCollectionView(this, collectionChangedArgs);
                        this.RemoveRecord(sender);
                    }
                    else
                    {
                        var recordIndex = this.InternalList.IndexOf(sender);
                        if (recordIndex <= -1 || recordIndex >= this.InternalList.Count)
                        {
                            forceRefreshTableSummary = true;
                            var comparerIndex =  this.InternalList.Count;
                             collectionChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sender,comparerIndex);
                             this.UpdateCollectionView(this, collectionChangedArgs);
                        }
                    }
                }
            }
            else
            {
                var passesFilter = true;
                if (this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && !sender.Equals(this.CurrentEditItem))
                {
                    if (this.CanFilter && sender != null)
                        passesFilter = this.FilterRecord(sender);

                    var removedList = new List<object> { sender };
                    var removeAtIndex = this.InternalList.IndexOf(sender);
                    if (!passesFilter)
                    {
                        if (removeAtIndex > -1 && removeAtIndex < this.InternalList.Count)
                        {
                            RemoveRecordFromGroup(sender, removeAtIndex);
                            this.RemoveRecord(sender);
                            forceRefreshTableSummary = true;
                            canRefreshSummary = false;
                        }
                        else
                        {
                            if (hasGroup)
                                this.TopLevelGroup.ResetGroup(sender, e.PropertyName);
                            canRefreshSummary = false;
                            canRefreshTableSummary = false;
                        }
                    }
                    else if (hasGroup)
                    {
                        var canremoveadd = true;
                        if (removeAtIndex > -1 && removeAtIndex < this.InternalList.Count)
                        {
                            if (canremoveadd)
                            {
                                this.RemoveRecordFromGroup(sender, removeAtIndex);
                                canRefreshSummary = false;
                            }
                        }
                        else
                        {
                            forceRefreshTableSummary = true;
                            if (hasGroup)
                                this.TopLevelGroup.ResetGroup(sender, e.PropertyName);
                        }

                        if (canremoveadd)
                            this.AddRecordToGroup(sender, removeAtIndex);
                    }
                    else
                    {
                        if (!(removeAtIndex > -1 && removeAtIndex < this.InternalList.Count))
                        {
                            forceRefreshTableSummary = true;
                            canRefreshSummary = false;
                            this.AddRecordToGroup(sender, this.InternalList.Count);
                        }
                    }
                }
            }

            if (this.IsGrouping && canRefreshSummary)
            {
                var isSummaryRowAffected = (from row in this.SummaryRows
                                            from col in row.SummaryColumns
                                            select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;

                var isCaptionSummaryAffected = this.CaptionSummaryRow != null && (from col in this.CaptionSummaryRow.SummaryColumns select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;

                if (isSummaryRowAffected || isCaptionSummaryAffected || forceRefreshSummary)
                {
                    var recordIndex = this.Records.IndexOfRecord(sender);
                    if (recordIndex > -1 && recordIndex < this.Records.Count)
                    {
                        var recordEntry = this.Records[recordIndex];
                        var parentGroup = recordEntry.Parent as Group;
                        if (parentGroup != null)
                        {
                            if (isSummaryRowAffected || forceRefreshSummary)
                            {
                                this.TopLevelGroup.UpdateSummaries(parentGroup);
                            }
                            if (isCaptionSummaryAffected || forceRefreshSummary)
                            {
                                this.TopLevelGroup.UpdateCaptionSummaries();
                            }
                        }
                    }
                }
            }

            if (canRefreshTableSummary && this.Records != null && this.TableSummaryRows != null)
            {
                var isTableSummaryAffected = (from row in this.TableSummaryRows
                                              from col in row.SummaryColumns
                                              select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;
                if (isTableSummaryAffected || forceRefreshTableSummary)
                {
                    this.UpdateTableSummary();
                }
            }
            this.OnRecordPropertyChanged(sender, propertyChangedArgs);
            IsInPropertyChange = false;
        }

        protected override int GetComparerIndex(object record, int index)
        {
            throw new NotImplementedException();
        }

        protected override int AdjustBeforeAdd(object record, int index)
        {
            if (this.UseOnDemandPaging)
                return base.AdjustBeforeAdd(record, index);
            return this.GetComparerIndex(this.InternalList, record, index, false);
        }

        internal int GetComparerIndex(IList recordsList, object record, int index, bool removeitembeforecheck)
        {
            IList internalList;
            if (removeitembeforecheck)
            {
                internalList = recordsList;
                if (index >= 0 && index < internalList.Count)
                    internalList.RemoveAt(index);
            }
            else
            {
                internalList = recordsList;
            }

            var comparer = new PageListOrdinalComparer(internalList, this.GetActiveComparer(), index);

            //Getting the index of record and if record was not present in the Records, it will return -1
            var comparerIndex = this.InternalBinarySearch(internalList, record, comparer);
            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }
            return comparerIndex;
        }

        internal int InternalBinarySearch(IList internalList, object record, IComparer<object> comparer)
        {
            var num = 0;
            var num2 = (internalList.Count) - 1;
            while (num <= num2)
            {
                var num3 = num + ((num2 - num) >> 1);
                var num4 = comparer.Compare(internalList[num3], record);
                if (num4 == 0)
                {
                    return num3;
                }
                if (num4 < 0)
                {
                    num = num3 + 1;
                }
                else
                {
                    num2 = num3 - 1;
                }
            }
            return ~num;
        }


        
        #endregion

        #region IPagedCollectionView Members

        public bool MoveToFirstPage()
        {
            return this.MoveToPage(0);
        }

        public bool MoveToLastPage()
        {
            var lastIndex = this.PageCount - 1;
            return this.MoveToPage(lastIndex);
        }

        public bool MoveToNextPage()
        {
            var nextIndex = this.pageIndex + 1 > this.PageCount - 1 ? this.PageCount - 1 : this.pageIndex + 1;
            return this.MoveToPage(nextIndex);
        }

        public bool MoveToPage(int pageIndex)
        {
            if (pageIndex < 0)
                return false;

            var changingArgs = new PageChangingEventArgs() { NewPageIndex = pageIndex };
            if (this.RaisePageChangingEvent(changingArgs))
                return false;

            this.pageIndex = pageIndex;

            if (UseOnDemandPaging)
            {
                var pageStartItemIndex = pageIndex * this.PageSize;
                this.LoadItemForTheIndex(pageStartItemIndex);
            }

            if (this.IsGrouping)
            {
                this.RefreshTopLevelGroup();
                this.RefreshSort();
            }

            this.Refresh();

            if (this.TableSummaryRows.Count > 0)
                this.UpdateTableSummary();

            var changedEventArgs = new PageChangedEventArgs() { NewPageIndex = pageIndex };
            this.RaisePageChangedEvent(changedEventArgs);

            return true;
        }

        public bool MoveToPreviousPage()
        {
            var previousIndex = this.pageIndex - 1 < 0 ? 0 : this.pageIndex - 1;
            return this.MoveToPage(previousIndex);
        }

        public int ItemCount
        {
            get { return this.InternalList.Count; }
        }

        public int PageIndex
        {
            get { return pageIndex; }
        }

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        public int TotalItemCount
        {
            get { return this.InternalList.Count; }
        }

        #endregion

        #region Private Methods

        private void InitializeInternalList(IEnumerable source)
        {
            this.InternalList = new List<object>(source.Cast<object>());
        }

        protected void UpdateItems()
        {
            this.InternalList.Clear();
            var enumerator = this.SourceCollection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (this.FilterRecord(enumerator.Current))
                {
                    this.InternalList.Add(enumerator.Current);
                }
            }
            if (SortDescriptions.Count > 0)
                ApplySort();
        }

        private void RefreshSource()
        {
            this.InternalList.Clear();
            this.ResetCache();
            int start = 0;
            while (start < this.MaxItemsCount)
            {
                this.InternalList.Add(default(object));
                start++;
            }
        }

        private bool LoadItemForTheIndex(int index)
        {
            var isAvailable = this.virtualLoadedIndexes.Contains(index);
            if (!isAvailable)
            {
                var args = new OnDemandItemsLoadingEventArgs() { StartIndex = index, PageSize = this.PageSize };
                this.RaiseOnDemandItemsLoadingEvent(args);
            }
            return isAvailable;
        }

        private void LoadItems(int startIndex, IEnumerable items)
        {
            foreach (var item in items)
            {
                this.InternalList[startIndex] = item;
                startIndex++;
            }
        }

        private void RaiseOnDemandItemsLoadingEvent(OnDemandItemsLoadingEventArgs args)
        {
            if (this.OnDemandItemsLoading != null)
            {
                this.OnDemandItemsLoading(this, args);
            }
        }

        private IEnumerable<object> SortItemsForBottomLevel(IEnumerable items, IComparer<object> sortComparer)
        {
            var provider = this.GetPropertyAccessProvider();
            var comparer = new SortFieldComparer(this.SortDescriptions, this.SortComparers, this.Culture, (rec, propName) =>
            {
                if (provider != null)
                {
                    var value = provider.GetValue(rec, propName);
                    if (value != null)
                    {
                        return value;
                    }
                }

                var func = this.GetFunc(propName);
                if (func != null)
                {
                    var value = func(propName, rec);
                    return value;
                }

                return null;
            });

            List<object> list = items.Cast<object>().ToList();

            list.Sort(comparer);
            return list;
        }

        private IQueryable SortQueryable(IQueryable source, IList<SortDescription> sortDescriptions)
        {
            var sortDescs = sortDescriptions != null ? sortDescriptions : this.SortDescriptions;
            for (int i = 0; i < this.SortDescriptions.Count; i++)
            {
                var s = this.SortDescriptions[i];
                var expressionFunc = this.GetExpressionFunc(s.PropertyName);
                var customComparer = this.SortComparers[s.PropertyName];
                if (s.Direction == ListSortDirection.Ascending)
                {
                    if (i == 0)
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.OrderBy(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderBy(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                                source = source.OrderBy(s.PropertyName, expressionFunc);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderBy(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                    else
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.ThenBy(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenBy(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                source = source.ThenBy(s.PropertyName, expressionFunc);
                            }
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenBy(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.OrderByDescending(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderByDescending(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                source = source.OrderByDescending(s.PropertyName, expressionFunc);
                            }
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderByDescending(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                    else
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.ThenByDescending(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenByDescending(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                source = source.ThenByDescending(s.PropertyName, expressionFunc);
                            }
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenByDescending(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                }

            }
            return source;
        }

        private bool RaisePageChangingEvent(PageChangingEventArgs args)
        {
            if (this.PageChanging != null)
            {
                this.PageChanging(this, args);
            }
            return args.Cancel;
        }

        private void RaisePageChangedEvent(PageChangedEventArgs args)
        {
            if (this.PageChanged != null)
            {
                this.PageChanged(this, args);
            }
        }

        private void WirePropertyChanged()
        {
            foreach (var data in this.SourceCollection)
            {
               this.AddNotifyListner(data); 
            }
        }

        private void UnwirePropertyChanged()
        {
            foreach (var data in this.SourceCollection)
            {
              this.RemoveNotifyListner(data);
            }
        }

        private void AddNotifyListner(object data)
        {
             var notifyPropertyChanged = data as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        private void RemoveNotifyListner(object data)
        {
            var notifyPropertyChanged = data as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
        }

        private void AddRecord(object item,int newIndex,int currentPageStartIndex,int currentPageEndindex)
        {
            RecordEntry record;
            if (this.FilterRecord(item))
            {
                if (IsInSourceCollectionChange)
                    this.AddNotifyListner(item);
                record = this.CreateRecordEntry(item);
                if (!this.ItemPropertiesSet)
                {
                    this.SetItemProperties(this.SourceCollection);
                }
                int collectionInsertIndex = newIndex;
                if (this.SortDescriptions.Count > 0 && LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping)
                    collectionInsertIndex = AdjustBeforeAdd(item, newIndex);

                if (collectionInsertIndex > -1 && collectionInsertIndex <= currentPageEndindex)
                {
                    var lastRecord = this.Records.LastOrDefault();
                    if (IsInSourceCollectionChange)
                        this.RemoveNotifyListener(lastRecord.Data);
                    if (this.Records.Count >= this.PageSize)
                    {
                        var recordRemovedIndex = this.Records.Count - 1;
                        this.Records.Remove(lastRecord);
                    }

                    this.InternalList.Insert(collectionInsertIndex, item);
                    var recordInsertIndex = (int)collectionInsertIndex % this.PageSize;
                    if (collectionInsertIndex < currentPageStartIndex)
                    {
                        recordInsertIndex = 0;
                        record = this.CreateRecordEntry(this.InternalList[currentPageStartIndex]);
                    }
                    this.Records.Insert(recordInsertIndex, record);
                }
                else
                {
                    if (collectionInsertIndex > -1)
                    {
                        if (collectionInsertIndex > this.InternalList.Count)
                        {
                            this.InternalList.Add(item);
                            var internalCollectionIndex = this.InternalList.Count - 1;
                            if (internalCollectionIndex < currentPageEndindex && internalCollectionIndex >= currentPageStartIndex)
                            {
                                this.Records.Add(record);
                            }
                        }
                        else
                            this.InternalList.Insert(collectionInsertIndex, item);
                        if (LiveDataUpdateMode != Data.LiveDataUpdateMode.Default)
                            this.UpdateTableSummary();
                    }
                }
            }
        }

        private void RemoveRecord(object item, int oldIndex,int currentPageStartIndex,int currentPageEndIndex)
        {
            RecordEntry record;
            RecordEntry oldRecord = this.Records.GetRecord(item);
            if (IsInSourceCollectionChange)
                this.RemoveNotifyListner(item);
            var collectionRemovedIndex = oldIndex;           
            if (collectionRemovedIndex > -1 && InternalList.Contains(item))
            {               
                        var recordRemovingIndex = Records.IndexOfRecord(item);
                        if (recordRemovingIndex < 0)
                        recordRemovingIndex = 0;                
                        InternalList.Remove(item);
                        Records.RemoveAt(recordRemovingIndex);                    
                        if (currentPageEndIndex < InternalList.Count && !UseOnDemandPaging)                        
                        {                            
                            var recordInsertIndex = PageSize - 1;                            
                            record = CreateRecordEntry(InternalList[currentPageEndIndex]);                            
                            Records.Insert(recordInsertIndex, record);
                        }                    
            }                         
        }

        private void AddGroupRecord(object item,int newIndex,int currentPageStartIndex, int currentPageLastIndex)
        {
            var insertItem = item;

            if (IsInSourceCollectionChange)
                this.AddNotifyListner(insertItem);

            int collectionInsertIndex = newIndex;
            if (this.SortDescriptions.Count > 0)
                collectionInsertIndex = AdjustBeforeAdd(item, newIndex);

            if (collectionInsertIndex > -1 && collectionInsertIndex <= currentPageLastIndex)
            {
                object lastRecord = null;
                int i = this.TopLevelGroup.DisplayElements.Count - 1;
                if (!UseOnDemandPaging)
                {
                while (i >= 0)
                {
                    if (this.TopLevelGroup.DisplayElements[i] is RecordEntry || this.TopLevelGroup.DisplayElements[i] is Group)
                    {
                        lastRecord = this.TopLevelGroup.DisplayElements[i];
                        break;
                    }
                    i--;
                }
                if (lastRecord != null)
                {
                    object lastData = null;
                    if (lastRecord is Group)
                    {
                        var group = lastRecord as Group;
                        while (!group.IsBottomLevel)
                        {
                            group = group.Groups.LastOrDefault();
                        }
                        if (group.Records.Count() != 0)
                            lastData = (group.Records.LastOrDefault() as RecordEntry).Data;
                    }
                    else
                    {
                        lastData = (lastRecord as RecordEntry).Data;
                    }
                    this.GroupList.Remove(lastData, IsInSourceCollectionChange);
                    if (IsInSourceCollectionChange)
                        this.RemoveNotifyListener(lastData);
                }
                }

                if (UseOnDemandPaging)
                    collectionInsertIndex = PageSize * PageIndex + collectionInsertIndex;

                this.InternalList.Insert(collectionInsertIndex, insertItem);
                var recordInsertIndex = (int)collectionInsertIndex % this.PageSize;

                if (collectionInsertIndex < currentPageStartIndex)
                {
                    recordInsertIndex = 0;
                    insertItem = this.InternalList[currentPageStartIndex];
                }
                this.GroupList.Add(insertItem, IsInSourceCollectionChange);

            }

            else
            {
                if (collectionInsertIndex > -1)
                    this.InternalList.Insert(collectionInsertIndex, item);
            }

        }

        private void RemoveGroupRecord(object item,int oldindex,int currentPageStartIndex,int currentPageLastIndex)
        {
            var removeditem = item;
            if (IsInSourceCollectionChange)
                this.RemoveNotifyListner(removeditem);
            var record = this.Records.GetRecord(item);
            var recordIndex = this.TopLevelGroup.DisplayElements.IndexOf(record);

            this.InternalList.Remove(removeditem);

            int isRemoved = this.GroupList.Remove(removeditem, IsInSourceCollectionChange);

            if ((currentPageLastIndex < this.InternalList.Count) && isRemoved != -1 && !UseOnDemandPaging)
            {
                var recordInsertIndex = this.PageSize - 1;
                this.GroupList.Add(this.InternalList[currentPageLastIndex], IsInSourceCollectionChange);
            }
        }

        private void ApplySort()
        {
            var source = UseOnDemandPaging ? this.InternalList.AsQueryable().Page(this.PageIndex, PageSize).OfQueryable(this.SourceType).AsQueryable() : this.InternalList.OfQueryable(this.SourceType).AsQueryable();
            if (this.SortDescriptions.Count == 0 || source.Count() == 0)
            {
                //Fix for the issue : WPF-12067
                //SourceCollection is not updated when changing pageize. So, It Shows empty Records in View.
                if (!useOnDemandPaging)
                {
                    this.InternalList = this.SourceCollection.Cast<object>().ToList();
                }
                return;
            }
            var sortedSource = SortQueryable(source, null).Cast<object>().ToList();
            if (!UseOnDemandPaging)
                this.InternalList = sortedSource;
            else
            {
                this.LoadItems(this.PageIndex * PageSize, sortedSource);
            }
        }

        #endregion

        #region Public Methods

        public void LoadDynamicItems(int startIndex, IEnumerable items)
        {
            if (this.SortDescriptions.Count > 0)
            {
                items = this.SortQueryable(items.OfQueryable().AsQueryable(), this.SortDescriptions);
            }
            if (startIndex >= 0 && startIndex < this.MaxItemsCount)
            {
                foreach (var item in items)
                {
                    if (startIndex >= this.MaxItemsCount)
                        break;
                    if (!isInGetInternalList)
                    {
                        this.AddNotifyListner(item);
                        if (!this.virtualLoadedIndexes.Contains(startIndex))
                        {
                            this.virtualLoadedIndexes.Add(startIndex);
                        }
                    }
                    if (this.InternalList.Count > startIndex )
                        this.InternalList[startIndex] = item;
                    else
                        this.InternalList.Add(item);
                    startIndex++;
                }
                if (!ItemPropertiesSet || this.ItemProperties.Count==0)
                {
                    var temp = this.InternalList.OfType<object>();
                    this.SetItemProperties(temp);
                    this.SetSourceType(temp.FirstOrDefault().GetType());
                }
            }
        }

        public void ResetCache()
        {
            this.virtualLoadedIndexes.Clear();
        }

        public IList GetInternalList()
        {
            return this.InternalList;
        }

        public IEnumerable GetInternalListForIndex(int pageIndex)
        {
            int startIndex = pageIndex * this.PageSize;
            if (this.UseOnDemandPaging)
            {
                IEnumerable list;
                isInGetInternalList = true;
                if (LoadItemForTheIndex(startIndex))
                    list = this.InternalList.ToList<object>().Skip(startIndex).Take(this.PageSize);
                else
                {
                    list = this.InternalList.ToList<object>().Skip(startIndex).Take(this.PageSize).ToList();
                    for (int i = startIndex; i < startIndex + this.pageSize; i++)
                        this.InternalList[i] = default(object);
                }
                isInGetInternalList = false;
                return list;
            }
            else
                return this.InternalList.ToList<object>().Skip(startIndex).Take(this.PageSize);
        }

        #endregion
    }

    #region Events

    public class OnDemandItemsLoadingEventArgs : EventArgs
    {
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class PageChangingEventArgs : CancelEventArgs
    {
        public int NewPageIndex { get; set; }
    }

    public class PageChangedEventArgs : EventArgs
    {
        public int NewPageIndex { get; set; }
    }

    public delegate void OnDemandItemsLoadingEventHandler(object sender, OnDemandItemsLoadingEventArgs args);

    public delegate void PageChangingEventHandler(object sender, PageChangingEventArgs args);

    public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs args);

    #endregion

    class PageListOrdinalComparer : IComparer<object>
    {
        // Fields
        private IList records;
        private IComparer<object> comparer;
        private int index;

        // Methods
        //internal ListOrdinalComparer(IRecordsList ilFull, IComparer<object> comparer, int removeAtIndex)
        internal PageListOrdinalComparer(IList ilFull, IComparer<object> comparer, int removeAtIndex)
        {
            this.records = ilFull;
            this.comparer = comparer;
            this.index = removeAtIndex;
        }

        public int Compare(object x, object y)
        {
            if (this.comparer != null)
            {
                var num = this.comparer.Compare(x, y);
                if (num == 0)
                {
                    var num2 = this.records.IndexOf(x);
                    return (num2 - index);
                }
                return num;
            }
            else
            {
                var num = Equals(x, y) ? this.index : this.records.IndexOf(x);
                return num - index;
            }
        }
    }
}