#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#if !WP
using System.Collections.Concurrent;
#endif
#endif

#if WinRT
using Windows.System.Threading;
using System.ComponentModel;
#else
using System.ComponentModel;
using System.Windows.Data;
using System.Threading;
#if !WP
using System.Runtime.Remoting.Messaging;
#endif
#endif

namespace Syncfusion.Data
{
    public class VirtualizingCollectionView : CollectionViewAdv, IFilterExt
    {
        #region Fields

        protected IList InternalList;
        private Predicate<object> rowFilter;
       
        int fetchSize = 10;

        private bool IsCurrentInView
        {
            get
            {
                return (0 <= this.CurrentPosition) && (this.CurrentPosition < this.GetViewRecordCount());
            }
        }


        #endregion

        #region Public Members

        public int FetchSize 
        {
            get { return fetchSize; }
            set { fetchSize = value; }
        }
        
        public Predicate<object> RowFilter
        {
            get { return rowFilter; }
            set { rowFilter = value; }
        }

        public Dictionary<int, RecordEntry> RecordDictionary { get; internal set; }

        public override bool EnableSummaryOptimization
        {
            get
            {
                return false;
            }           
        }
        #endregion

        #region Ctor

        public VirtualizingCollectionView(IEnumerable source)
            : base(source)
        {
            InternalList = new List<object>(source.Cast<object>());
            this.InitializeCollection();           
        }

        public VirtualizingCollectionView()
            : base()
        {
            this.InitiateCollectionViewAdv();
            this.InitializeCollection();
        }
        
        #endregion

        #region Virtual Methods

        public override object GetItemAt(int index)
        {
            return InternalList[index];
        }

        protected virtual void ProcessSort(SortDescriptionCollection sortDescription)
        {
            if (sortDescription.Count == 0)
            {
                this.InternalList = this.SourceCollection.Cast<object>().ToList();
                return;
            }
            var sortedSource = GetSortSource();
            
            this.InternalList = new List<object>(sortedSource);
        }

        protected virtual void ApplyFilter(Predicate<object> RowFilter)
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
                this.ProcessSort(this.SortDescriptions);
        }

        protected virtual IEnumerable<GroupResult> GetGroupedSource(string[] groupBy)
        {
            if (InternalList.Count == 0)
                return null;
            var unsortedGroups = this.GetSortWithoutGroups();

            var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(), propertyAccessProvider, this.SortComparers, this.Culture, this.GetFunc(string.Empty));
            IQueryable queryable = this.InternalList.OfQueryable(this.SourceType).AsQueryable();

            Func<string, Expression> groupConverterFunc = (property) => this.GetGroupConverterExpressionFunc(property);
            var hasGroupConverter = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.Converter != null) != null;
            var expressionFunc = hasGroupConverter ? groupConverterFunc : (property) => this.GetExpressionFunc(property);
            var result = queryable.GroupByMany(this.SourceType, expressionFunc, groupBy).ToList();

            if (unsortedGroups.Count > 0)
            {
                foreach (var groupResult in result)
                {
                    if (groupResult.SubGroups == null)
                    {
                        groupResult.Items = SortItemsForBottomLevel(groupResult.Items, sortComparer);
                    }
                    else
                    {
                        groupResult.SubGroups = this.SetSortOrderForInnerGroup(groupResult.SubGroups, sortComparer);
                    }
                }
            }
            return result;
        }

        public virtual IEnumerable GetInternalSource()
        {
            return InternalList;
        }

        public virtual IEnumerable GetSourceListForFilteringItems()
        {
            return this.SourceCollection;
        }

        public virtual int GetViewRecordCount()
        {
            return InternalList.Count;
        }

        protected virtual int GetIndexOf(object item)
        {
            if (item is RecordEntry)
            {
                var record = item as RecordEntry;
                return InternalList.IndexOf(record.Data);
            }
            return this.InternalList.IndexOf(item);
        }

        #endregion

        #region Internal Methods

        internal void SetRecordValue(int index, object value)
        {
            this.RecordDictionary[index] = value as RecordEntry;
        }

        internal RecordEntry GetRecord(object data)
        {
            if (this.IsGrouping)
            {
                var group = (this.TopLevelGroup as VirtualizingTopLevelGroup).GetGroup(data, this.TopLevelGroup);
                if (group != null)
                {
                    var recordindex = group.GetRecordIndex(data);
                    if (recordindex < 0)
                        return null;
                    return group.Records[recordindex];
                }
            }
            else
            {
                var index = this.GetIndexOf(data);
                if (this.RecordDictionary.ContainsKey(index))
                    return RecordDictionary[index];
            }
            return null;
        }

        #endregion

        #region Overrides

        public override int Count
        {
            get
            {
                return GetViewRecordCount();
            }
        }

        public override int IndexOf(object item)
        {
            return GetIndexOf(item);
        }

        protected sealed override IRecordsList CreateRecords()
        {
            return new EnumerableRecordsWrapper(this);
        }

        public override bool FilterRecord(object record)
        {
            return (this.Filter == null || this.Filter(record)) && (this.RowFilter == null || this.RowFilter(record));
        }

        protected sealed override void OnRecordCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        public sealed override RecordEntry GetRecordAt(int index)
        {
            if (!this.RecordDictionary.ContainsKey(index))
            {
                this.LoadRecordForIndex(index);
            }
            if (RecordDictionary.ContainsKey(index))
                return RecordDictionary[index];
            return null;
        }

        public sealed override bool MoveCurrentTo(object item)
        {
            if (item == null)
                return false;
            if (object.Equals(this.CurrentItem, item) && !IsGrouping && (item != null || this.IsCurrentInView))
            {
                return this.IsCurrentInView;
            }

            var position = -1;
            position = this.InternalList.IndexOf(item);

            return this.MoveCurrentToPosition(position);
        }

        public sealed override bool MoveCurrentToPosition(int index)
        {
            var totalCount = this.IsGrouping ? this.TopLevelGroup.DisplayElements.Count : this.GetViewRecordCount();
            if ((index < -1) || (index > totalCount))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if ((index != this.CurrentPosition) || !IsCurrentInSync)
            {
                var newItem = ((0 <= index) && (index < this.GetViewRecordCount())) ? this.GetItemAt(index) : null;

                if (this.IsGrouping)
                {
                    var nodeEntry = this.TopLevelGroup.DisplayElements[index];
                    if (nodeEntry != null && nodeEntry.IsRecords)
                    {
                        newItem = nodeEntry;
                    }
                }

                if (!this.RaiseCurrentChangingEvent())
                    return false;
                this.SetCurrent(newItem, index);
                this.RaiseCurrentChangedEvent();

                this.OnPropertyChanged("CurrentPosition");
                this.OnPropertyChanged("CurrentItem");
            }
            return this.IsCurrentInView;
        }

        protected sealed override void RefreshSort()
        {
            if (!IsGrouping)
            {
                var hasFilters = this.FilterPredicates.FirstOrDefault(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0) != null;
                if (!CanFilter || !hasFilters)
                    this.ProcessSort(this.SortDescriptions);
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

        public override void RefreshFilter()
        {
            var filterPresent = this.FilterPredicates.Any(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0);
            if (filterPresent)
            {
                var source = this.SourceCollection.OfQueryable().AsQueryable();
                ParameterExpression paramExpression;

                Expression predicate = this.GetPredicateExpression(source, out paramExpression);
                if (paramExpression != null && predicate != null)
                {
                    var lamda = Expression.Lambda(predicate, paramExpression);
                    var delg = lamda.Compile();
                    this.RowFilter = (o) =>
                        {
                            var result = (bool) delg.DynamicInvoke(o);
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

            if (!this.IsGrouping)
            {
                this.ApplyFilter(this.RowFilter);
            }
            else
            {
                this.RefreshGroupAfterFiltering(this.TopLevelGroup.Groups);
                this.TopLevelGroup.SetDirty();
                var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                grpRefresh.RefreshFilters();
            }
            if (!this.IsInEndeferal)
                this.Refresh();
        }

        protected sealed override void OnSortDescriptionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSort();
            base.OnSortDescriptionChanged(e);
        }

        protected sealed override void EnsureInitialized()
        {
            if (this.RecordDictionary != null && this.RecordDictionary.Count > 0)
                RecordDictionary.Clear();

            if (IsGrouping && CanFilter)
            {
                this.InternalList.Clear();
                PopulateRecordsFromGroup(this.TopLevelGroup.Groups);
            }
            base.EnsureInitialized();
        }

        private void PopulateRecordsFromGroup(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    foreach (var record in (group as VirtualGroup).InternalList)
                    {
                        this.InternalList.Add(record);
                    }
                }
                else
                {
                    this.PopulateRecordsFromGroup(group.Groups);
                }
            }
        }

        protected sealed override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            return GetGroupedSource(groupBy);
        }

        internal sealed override void UpdateTableSummary()
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
                    summaryRecordEntry = new SummaryRecordEntry(null, -2) { SummaryRow = summaryRow };
                    this.Records.TableSummaries.Add(summaryRecordEntry);
                }

                IEnumerable items = this.GetInternalSource().OfQueryable(this.SourceType);
                foreach (var summaryColumn in summaryRow.SummaryColumns)
                {
                    if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
                    {
                        var summaryItems = new Dictionary<string, object>();
                        SummaryCreator.RaiseQuerySummaryAggregate(items, summaryColumn, summaryItems, this);
                        SummaryValue summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                        if (summaryValue == null)
                        {
                            summaryValue = new SummaryValue() { Name = summaryColumn.Name };
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

        protected override void UpdateCollectionView(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (this.FilterRecord(item))
                            {
                                if (!this.ItemPropertiesSet)
                                {
                                    this.SetItemProperties(this.SourceCollection);
                                }
                                int insertIndex = -1;
                                insertIndex = this.InternalList.Count;
                                this.InternalList.Add(item);
                                if (insertIndex >= 0)
                                {
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, insertIndex));
                                }
                            }
                            AddNotifyListener(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (e.OldItems.Count <= 0) continue;
                            var removeAtIndex = this.InternalList.IndexOf(item);
                            if (removeAtIndex > -1)
                            {
                                if (this.RecordDictionary.ContainsKey(removeAtIndex))
                                    this.UpdateRecordDictionary(removeAtIndex);
                                this.InternalList.RemoveAt(removeAtIndex);
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, removeAtIndex));
                            }
                            RemoveNotifyListener(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        var olditem = e.OldItems[0];
                        var index = this.InternalList.IndexOf(olditem);

                        var newitem = e.NewItems[0];
                        var newindex = e.NewStartingIndex;

                        if (this.FilterRecord(newitem))
                        {
                            if (index > -1 && index < this.InternalList.Count)
                            {
                                this.InternalList[index] = newitem;
                                if (this.RecordDictionary.ContainsKey(index))
                                    this.RecordDictionary[index] = this.CreateRecordEntry(newitem);
                            }
                            else
                            {
                                this.InternalList.Add(newitem);
                                if (this.RecordDictionary.ContainsKey(this.InternalList.Count - 1))
                                    this.RecordDictionary.Add(this.RecordDictionary.Count - 1, CreateRecordEntry(newitem));
                            }
                            this.OnCollectionChanged(e);
                        }
                        else
                        {
                            if (index > -1 && index < this.InternalList.Count)
                            {
                                var removeAtIndex = this.InternalList.IndexOf(olditem);
                                if (removeAtIndex > -1)
                                {
                                    if (this.RecordDictionary.ContainsKey(removeAtIndex))
                                        this.UpdateRecordDictionary(removeAtIndex);
                                    this.InternalList.RemoveAt(removeAtIndex);
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, olditem, removeAtIndex));
                                }
                            }
                        }
                        RemoveNotifyListener(olditem);
                        AddNotifyListener(newitem);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.UnwireNotifyPropertyChangedForUnderlyingSource();
                    if (!this.ItemPropertiesSet)
                    {
                        this.SetItemProperties(this.SourceCollection);
                    }
                    if ((sender as IEnumerable).AsQueryable().Count() > 0)
                    {
                        this.Refresh();
                    }
                    else
                    {
                        this.InternalList.Clear();
                        this.RecordDictionary.Clear();
                    }
                    this.WireNotifyPropertyChangedForUnderlyingSource(true);
                    this.OnCollectionChanged(e);
                    break;
            }
        }

        internal sealed override void UpdateGroupingModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.UpdateGroupingModel(sender, e);
        }

        protected sealed override void OnTopLevelGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var record in e.NewItems)
                    {
                        this.InternalList.Add((record as RecordEntry).Data);
                    }
                    this.OnCollectionChanged(e);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var record in e.OldItems)
                    {
                        var recordEntry = record as RecordEntry;
                        var recordindex = this.InternalList.IndexOf(recordEntry.Data);
                        if (recordindex >= 0)
                            this.InternalList.RemoveAt(recordindex);
                    }
                    this.OnCollectionChanged(e);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.InternalList.Clear();
                    this.UnWireEvents();
                    this.OnCollectionChanged(e);
                    break;
            }
        }

#if !WPF
        public override PropertyInfoCollection GetItemProperties()
#else
        public override PropertyDescriptorCollection GetItemProperties()
#endif
        {
            if (this.ItemPropertiesSet)
                return ItemProperties;
            else
            {
                var item = GetItemAt(0);
                var collection = new List<object>();
                collection.Add(item);
                this.SetItemProperties(collection);
                this.SetSourceType(item.GetType());
                return ItemProperties;
            }
        }

        protected override void NotifyPropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propertyChangedArgs = e;
            if (propertyChangedArgs != null && string.IsNullOrEmpty(propertyChangedArgs.PropertyName))
            {
                if (this.SortDescriptions.Count > 0)
                    propertyChangedArgs = new PropertyChangedEventArgs(this.SortDescriptions[0].PropertyName);
                if (this.TableSummaryRows != null && this.TableSummaryRows.Count > 0)
                    propertyChangedArgs = new PropertyChangedEventArgs(this.TableSummaryRows[0].SummaryColumns[0].MappingName);
                if (this.SummaryRows != null && this.SummaryRows.Count > 0)
                    propertyChangedArgs = new PropertyChangedEventArgs(this.SummaryRows[0].SummaryColumns[0].MappingName);
            }

            if (this.IsGrouping)
            {
                var isSummaryRowAffected = (from row in this.SummaryRows from col in row.SummaryColumns select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;
                var isCaptionSummaryAffected = CaptionSummaryRow != null && (from col in CaptionSummaryRow.SummaryColumns select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;

                if (isSummaryRowAffected || isCaptionSummaryAffected)
                {
                    var parentGroup = (this.TopLevelGroup as VirtualizingTopLevelGroup).GetGroup(sender, this.TopLevelGroup);
                    if (parentGroup != null)
                    {
                        if (isSummaryRowAffected)
                        {
                            this.TopLevelGroup.UpdateSummaries(parentGroup);
                        }
                        if (isCaptionSummaryAffected)
                        {
                            this.TopLevelGroup.UpdateCaptionSummaries();
                        }
                    }
                }
            }

            if (this.Records != null && this.TableSummaryRows != null)
            {
                var isTableSummaryAffected = (from row in this.TableSummaryRows
                                              from col in row.SummaryColumns
                                              select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;
                if (isTableSummaryAffected)
                {
                    this.UpdateTableSummary();
                }
            }
            this.OnRecordPropertyChanged(sender, propertyChangedArgs);
        }

        protected override void RemoveRecord(object record)
        {
            //As of now we do not provide support for AllowDataShapping in VirtualCollectionView. Hence record will not be removed in property change and we overriding empty method here to avoid the crash.
        }

        public override void CommitEdit()
        {
            //As of now we do not provide support for AllowDataShapping in VirtualCollectionView. So CommitEdit is not required for this. To avoid the crash we are overriding empty CommitEdit.
            if (IsInCommitEdit)
                return;

            if (this.editItem == null)
                throw new InvalidOperationException("IsEditingItem is not true to EndEdit");

            IsInCommitEdit = true;
            if (editItem is IEditableObject)
                (editItem as IEditableObject).EndEdit();
            editItem = null;
            IsInCommitEdit = false;
        }

        public override void CommitNew()
        {
            if (CurrentAddItem == null)
                throw new InvalidOperationException("CurrentAddItem should not null when CommitNew");
            this.IsInSuspend = true;
            var source = this.GetSourceListCollection();
            source.Add(CurrentAddItem);
            if (this.FilterRecord(CurrentAddItem))
            {
                if (!this.ItemPropertiesSet)
                {
                    this.SetItemProperties(this.SourceCollection);
                }
                this.IsInSourceCollectionChange = true;
                if (!IsGrouping)
                {
                    int insertIndex = -1;
                    insertIndex = this.InternalList.Count;
                    this.InternalList.Add(CurrentAddItem);
                    if (insertIndex >= 0)
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, CurrentAddItem, insertIndex));
                    }
                }
                else
                    this.GroupList.Add(CurrentAddItem, true);
                this.IsInSourceCollectionChange = false;
            }
            AddNotifyListener(CurrentAddItem);
            CurrentAddItem = null;
            this.IsInSuspend = false;
        }

        #endregion

        #region Private Methods

        private void InitializeCollection()
        {
            RecordDictionary = new Dictionary<int, RecordEntry>();
        }

        private void LoadRecordForIndex(int index)
        {
            var endIndex = index + FetchSize;
            endIndex = endIndex >= this.GetViewRecordCount() ? this.GetViewRecordCount() : endIndex;
            while (index < endIndex)
            {
                if (!RecordDictionary.ContainsKey(index))
                {
                    var item = GetItemAt(index);
                    RecordDictionary.Add(index, CreateRecordEntry(item));
                }
                index++;
            }
        }

        protected override void SetCurrent(object newItem, int newPosition)
        {
            int count = (newItem != null) ? 0 : (this.IsEmpty ? 0 : this.GetViewRecordCount());

            if (newItem != null)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, false);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, false);
            }
            else if (count == 0)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                newPosition = -1;
            }
            else
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
            }

            if (newItem is RecordEntry)
                this.currentItem = newItem;
            else
                this.currentItem = this.CreateRecordEntry(newItem);

            if (this.GroupList == null)
            {
                this.currentPosition = currentItem != null ? this.GetIndexOf(this.currentItem) : -1;
            }
            else
            {
                this.currentPosition = currentItem != null ? this.TopLevelGroup.IndexOf(((RecordEntry)this.currentItem).Data) : -1;
            }
        }

        private IEnumerable<object> GetSortSource()
        {
            var source = this.InternalList as IEnumerable<object>;

            for (int i = 0; i < this.SortDescriptions.Count; i++)
            {
                var s = this.SortDescriptions[i];
             //   var expressionFunc = this.GetExpressionFunc(s.PropertyName);
                var customComparer = this.SortComparers[s.PropertyName];
                if (s.Direction == ListSortDirection.Ascending)
                {
                    if (i == 0)
                    {
                        if (customComparer==null)
                            source = source.OrderBy(s.PropertyName, this.GetFunc(s.PropertyName));
                        else
                            source = source.OrderBy(s.PropertyName, this.GetFunc(s.PropertyName),customComparer);
                    }
                    else
                    {
                        if (customComparer==null)
                            source = (source as IOrderedEnumerable<object>).ThenBy(s.PropertyName, this.GetFunc(s.PropertyName));
                        else
                            source = (source as IOrderedEnumerable<object>).ThenBy(s.PropertyName, this.GetFunc(s.PropertyName),customComparer);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        if (customComparer==null)                        
                            source = source.OrderByDescending(s.PropertyName, this.GetFunc(s.PropertyName));
                        else
                            source = source.OrderByDescending(s.PropertyName, this.GetFunc(s.PropertyName),customComparer);
                    }
                    else
                    {
                        if (customComparer==null)
                            source = (source as IOrderedEnumerable<object>).ThenByDescending(s.PropertyName, this.GetFunc(s.PropertyName));
                        else
                            source = (source as IOrderedEnumerable<object>).ThenByDescending(s.PropertyName, this.GetFunc(s.PropertyName),customComparer);
                    }
                }
            }

            return source;
        }

        public virtual Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression)
        {
            return this.GetPredicateExpressionExt(source, out paramExpression);
        }

        public Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression, string columnName, bool returncolExpression)
        {
            return this.GetPredicateExpressionExt(source, out paramExpression, columnName, returncolExpression);
        }

        private ObservableCollection<SortDescription> GetSortWithoutGroups()
        {
            var sortFields = new ObservableCollection<SortDescription>();
            foreach (var sortDesc in this.SortDescriptions)
            {
                var unsortedGroup = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(s => s.PropertyName == sortDesc.PropertyName);
                if (unsortedGroup == null)
                {
                    sortFields.Add(new SortDescription(sortDesc.PropertyName, sortDesc.Direction));
                }
            }

            return sortFields;
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

        private IEnumerable<GroupResult> SetSortOrderForInnerGroup(IEnumerable<GroupResult> groups, IComparer<object> sortComparer)
        {
            IEnumerable<GroupResult> groupResults = groups.ToList();
            foreach (var groupResult in groupResults)
            {
                if (groupResult.SubGroups == null)
                {
                    if (this.SortDescriptions.Count > 0)
                    {
                        groupResult.Items = this.SortItemsForBottomLevel(groupResult.Items, sortComparer);
                    }
                }
                else
                {
                    groupResult.SubGroups = this.SetSortOrderForInnerGroup(groupResult.SubGroups, sortComparer);
                }
            }

            return groupResults;
        }

        private void UpdateRecordDictionary(int changedIndex)
        {
            List<int> needToRemoveIndex = new List<int>();
            foreach (var item in RecordDictionary)
            {
                if (item.Key >= changedIndex)
                    needToRemoveIndex.Add(item.Key);
            }
            foreach (var index in needToRemoveIndex)
            {
                RecordDictionary.Remove(index);
            }
        }

        #endregion

        #region Protected Methods

        protected void RefreshGroupAfterFiltering(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    var virtualGroup = group as VirtualGroup;
                    virtualGroup.ApplyFiltering(this.FilterRecord);
                    this.TopLevelGroup.UpdateSummaries(group);
                }
                else
                    RefreshGroupAfterFiltering(group.Groups);
            }
        }

        #endregion

    }
}

