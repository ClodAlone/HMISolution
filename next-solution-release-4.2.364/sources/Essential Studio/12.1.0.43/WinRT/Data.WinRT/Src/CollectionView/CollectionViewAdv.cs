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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Syncfusion.Data.Extensions;
using Syncfusion.Data.Helper;
#if WinRT
using Syncfusion.Dynamic;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#if !WP7
using Syncfusion.Dynamic;
#endif
#endif
#if WPF
using System.Data;
using System.Threading;
using System.Windows.Threading;
#if !SyncfusionFramework3_5
using System.Dynamic;
using Syncfusion.Dynamic;
#endif
#endif

namespace Syncfusion.Data
{
    public class CollectionViewAdv : ICollectionViewAdv, IUnboundExpressionFunc
    {
        #region PRIVATE
        private IEnumerable source;
        private IRecordsList records;
        private ISummaryRow captionSummaryRow;
        private ObservableCollection<ISummaryRow> summaryRows;
        private ObservableCollection<ISummaryRow> tableSummaryRows;
        private SortComparers sortComparers;
        private CultureInfo culture;
        private Predicate<object> filter;
        private ObservableCollection<GroupDescription> groupDescriptions;
        private SortDescriptionCollection sortDescriptions;
        protected object currentItem;
        protected int currentPosition;
        protected internal bool IsInSuspend = false;
        protected bool IsInEndeferal;
        protected internal bool IsInPropertyChange;
        private TopLevelGroup topLevelGroup;
        #endregion

#if !WP
        private bool isDynamicSourceEvaluated = false;
        private bool isDynamicBound = false;
        /// <summary>
        /// Gets a value indicating whether this instance is dynamic bound.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dynamic bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsDynamicBound
        {
            get
            {
                if (!this.isDynamicSourceEvaluated && this.SourceCollection != null)
                {
                    var enumerator = this.SourceCollection.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }

                    var record = enumerator.Current;

                    if (record != null)
                    {
                        this.isDynamicBound = DynamicHelper.CheckIsDynamicObject(record.GetType());
                        if (this.SourceType != null && this.SourceType.FullName == "IronRuby.Ruby")
                            this.isDynamicBound = false;
                    }
                    this.isDynamicSourceEvaluated = true;
                }
                return this.isDynamicBound;
            }
            set
            {
                if (value)
                {
                    this.isDynamicSourceEvaluated = true;
                    this.isDynamicBound = value;
                }
            }
        }

#if WPF
        internal bool validatedICustomTypeDescriptor = false;
        internal bool hasICustomTypeDescriptor = false;
        internal bool HasICustomTypeDescriptor
        {
            get
            {
                if (!this.validatedICustomTypeDescriptor)
                {
                    var enumerator = this.SourceCollection.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }

                    var record = enumerator.Current;

                    if (record != null)
                    {
                        if (typeof(ICustomTypeDescriptor).IsAssignableFrom(record.GetType()))
                        {
                            hasICustomTypeDescriptor = true;
                        }
                    }
                    this.validatedICustomTypeDescriptor = true;
                }

                return hasICustomTypeDescriptor;
            }
        }
#endif
#endif

        public CollectionViewAdv(IEnumerable _source)
        {
            this.SetSource(_source);
            this.InitiateCollectionViewAdv();
        }

        public CollectionViewAdv(IEnumerable _source, Type sourceType)
        {
            this.SetSource(_source);
            this.SetSourceType(sourceType);
            this.InitiateCollectionViewAdv();
        }

        internal CollectionViewAdv()
        {
        }

        public IRecordsList Records
        {
            get
            {
                if (records == null || this.flags == CollectionViewFlags.NeedsRefresh)
                {
                    this.EnsureInitialized();
                }
                return this.records;
            }
        }

        /// <summary>
        /// Gets a value to enable or disable summary calculation optimization
        /// </summary>
        /// <remarks></remarks>
        private bool enableSummaryOptimization = true;

        public virtual bool EnableSummaryOptimization
        {
            get { return enableSummaryOptimization; }
            set { enableSummaryOptimization = value; }
        }

        public bool IsGrouping
        {
            get
            {
                return this.GroupDescriptions.Count > 0;
            }
        }

        public IGroupList GroupList
        {
            get
            {
                return this.IsGrouping ? this.TopLevelGroup : null;
            }
        }

        public TopLevelGroup TopLevelGroup
        {
            get
            {
                if (this.topLevelGroup == null && this.IsGrouping)
                {
                    InitializeTopLevelGroup();
                }
                return this.topLevelGroup;
            }
        }

        public ISummaryRow CaptionSummaryRow
        {
            get { return captionSummaryRow; }
            set
            {
                captionSummaryRow = value;
                if (this.topLevelGroup != null)
                    this.topLevelGroup.UpdateCaptionSummaries();
            }
        }

        public ObservableCollection<ISummaryRow> SummaryRows
        {
            get { return summaryRows; }
        }

        public ObservableCollection<ISummaryRow> TableSummaryRows
        {
            get { return tableSummaryRows; }
        }

        public SortComparers SortComparers
        {
            get { return this.sortComparers ?? (this.sortComparers = new SortComparers()); }
        }

        private IComparer<Group> groupComparer;
        public IComparer<Group> GroupComparer
        {
            get { return groupComparer; }
            set { groupComparer = value; }
        }

        public virtual RecordEntry CreateRecordEntry(object data)
        {
            return new RecordEntry(null, -1, data);
        }

        public event PropertyChangedEventHandler RecordPropertyChanged;

        #region Reflection Helpers
        
        public Type SourceType
        {
            get;
            private set;
        }

        public void SetSourceType(Type sourceType)
        {
            if (sourceType == null || sourceType == SourceType)
                return;

            if (sourceType == typeof (Object))
                return;
            
            this.SourceType = sourceType;
            IsItemPropertiesTypeSet = true;
            if (this.SourceType.IsInterface())
                this.propertyAccessProvider = CreateItemPropertiesProvider();
        }

#if WPF
        private bool _islegacyTable = false;
        private bool _islegacyTableEvaluated = false;
        /// <summary>
        /// Gets a value indicating whether this instance is legacy data table.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is legacy data table; otherwise, <c>false</c>.
        /// </value>
        public bool IsLegacyDataTable
        {
            get
            {
                if (!_islegacyTableEvaluated)
                {
                    _islegacyTable = this.SourceCollection is DataTable || this.SourceCollection is DataView;
                    _islegacyTableEvaluated = true;
                }
                return _islegacyTable;
            }
            internal set { _islegacyTable = value; }
        }

        private PropertyDescriptorCollection itemProperties;
		/// <summary>
		/// Gets the item properties.
		/// </summary>
		/// <value>The item properties.</value>
		public PropertyDescriptorCollection ItemProperties
		{
			get
			{
				return this.itemProperties;
			}
		}
#else
        private PropertyInfoCollection itemProperties;
        public PropertyInfoCollection ItemProperties
        {
            get
            {
                return this.itemProperties;
            }
        }
#endif

        protected internal IPropertyAccessProvider propertyAccessProvider;

        public virtual IPropertyAccessProvider GetPropertyAccessProvider()
        {
            return this.propertyAccessProvider;
        }

        protected virtual IPropertyAccessProvider CreateItemPropertiesProvider()
        {
            IPropertyAccessProvider provider = null;
#if !WP
            if (this.IsDynamicBound)
            {
                provider = new DynamicPropertiesProvider(this);
                return provider;
            }
#endif
            provider = new ItemPropertiesProvider(this);
            return provider;
        }

#if WPF
        public virtual PropertyDescriptorCollection GetItemProperties()
#else
        public virtual PropertyInfoCollection GetItemProperties()
#endif
        {
            return this.ItemProperties;
        }

        protected bool ItemPropertiesSet = false;
#if WPF
        private bool isITypedListSource = false;
#endif
        protected bool IsItemPropertiesTypeSet = false;
        protected void SetItemProperties(IEnumerable dataSource)
        {   
            if (dataSource == null)
            {
#if WPF
                this.itemProperties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
#else
                this.itemProperties = PropertyInfoCollection.Empty;
#endif
            }
            else
            {
                var list = dataSource;
#if WPF
                isITypedListSource = false;
                if (dataSource is IListSource)
                    list = ((IListSource) dataSource).GetList();

                if (dataSource is ITypedList)
                {
                    this.isITypedListSource = true;
                    this.itemProperties = ((ITypedList) (dataSource)).GetItemProperties(null);
                    this.ItemPropertiesSet = true;
                }
                else if (list is ITypedList)
                {
                    this.isITypedListSource = true;
                    this.itemProperties = ((ITypedList) (list)).GetItemProperties(null);
                    this.ItemPropertiesSet = true;
                }
                else if (list != null)
                {
#endif
                    var enumerator = list.GetEnumerator();
                    if (SourceType != null && SourceType != typeof(Object))
                    {
#if WPF
                        this.itemProperties = TypeDescriptor.GetProperties(SourceType);
#else
						this.itemProperties = new PropertyInfoCollection(SourceType);
#endif
                        this.ItemPropertiesSet = true;
                    }
                    else if (enumerator.MoveNext() && enumerator.Current != null)
                    {
                        var castType = EnumerableExtensions.CastToSourceType(list);
#if WPF
                        this.itemProperties = TypeDescriptor.GetProperties(castType ?? enumerator.Current.GetType());
#else
                        this.itemProperties = new PropertyInfoCollection(castType ?? enumerator.Current.GetType());
#endif
                        ItemPropertiesSet = true;
                    }
                    else
                    {
                        if (list.GetType().IsGenericType())
                        {
                            var genericType = list.GetType().GetGenericArguments().FirstOrDefault() as Type;
                            if (genericType != null)
                            {
                                ItemPropertiesSet = !(genericType == typeof(Object));
#if WPF
                                this.itemProperties = TypeDescriptor.GetProperties(genericType);
#else
                                this.itemProperties = new PropertyInfoCollection(genericType);
#endif
                            }
                        }

                        if(!ItemPropertiesSet)
                        {
                            //var prop = list.GetType().GetProperty("Item");
                            var prop = list.GetItemPropertyInfo();
                            if (prop != null)
                            {
                                ItemPropertiesSet = !(prop.PropertyType == typeof (Object));
#if WPF
                                this.itemProperties = TypeDescriptor.GetProperties(prop.PropertyType);
#else
                                this.itemProperties = new PropertyInfoCollection(prop.PropertyType);
#endif
                            }
                        }
                    }
                }

            this.propertyAccessProvider = this.CreateItemPropertiesProvider();
#if WPF
			}
#endif
            
            if (!IsItemPropertiesTypeSet)
            {
#if !WP
                if (this.isDynamicBound)
                    this.SourceType = typeof (object);
                else
#endif
                    this.SourceType = dataSource.GetElementType(ref isEmpty);
            }

            if (ItemPropertiesSet)
                this.OnPropertyChanged("ItemProperties");

        }

        #endregion

        public CultureInfo Culture
        {
            get { return culture ?? CultureInfo.CurrentCulture; }
            set { culture = value; }
        }

        public Predicate<object> Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                //this.Refresh();
            }
        }
        
        private ObservableCollection<IFilterDefinition> filterPredicates;
        public ObservableCollection<IFilterDefinition> FilterPredicates
        {
            get { return filterPredicates; }
            set
            {
                if (filterPredicates.Equals(value)) 
                    return;
                filterPredicates = value;
                this.OnPropertyChanged("FilterPredicates");
                ApplyFilters();
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                if (groupDescriptions == null)
                {
                    groupDescriptions = new ObservableCollection<GroupDescription>();
                    groupDescriptions.CollectionChanged += OnGroupDescriptionCollectionChanged;
                }
                return groupDescriptions;
            }
            internal set { groupDescriptions = value; }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get { return sortDescriptions; }
            internal set { sortDescriptions = value; }
        }

        public IEnumerable SourceCollection
        {
            get { return GetSource(); }
        }

        protected virtual IEnumerable GetSource()
        {
            return source;
        }

        protected IList GetSourceListCollection()
        {
            IList list = null;
            if ((this.SourceCollection as IList) != null)
            {
                list = this.SourceCollection as IList;
            }
#if WPF
            else if ((this.SourceCollection as IListSource) != null)
            {
                var listSource = this.SourceCollection as IListSource;
                list = listSource.GetList();
            }
#endif

            return list;
        }

        private IDisposable endDeferDisposable;
        private int deferRefreshCount = -1;
        public IDisposable DeferRefresh()
        {
            deferRefreshCount++;
            return new DeferHelper(this);
        }

        //Check
        public void Refresh()
        {
            this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
            //if (this.deferRefreshCount > -1)
            //    return;
            EnsureRecordsInitialized();
            this.RefreshView(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void EnsureRecordsInitialized()
        {
            if (this.Records == null || this.flags == CollectionViewFlags.NeedsRefresh)
                this.EnsureInitialized();
        }

        public bool Contains(object item)
        {
            if (item is RecordEntry)
                return this.Records.Contains(item as RecordEntry);
            var rec = this.Records.FirstOrDefault(o => o.Data == item);
            return this.Records.Contains(rec);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event NotifyCollectionChangedEventHandler SourceCollectionChanged;
        public event NotifyCollectionChangedEventHandler TopLevelGroupCollectionChanged;

        public void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.deferRefreshCount > -1)
                return;
            if (LiveDataUpdateMode != LiveDataUpdateMode.Default && this.IsInSourceCollectionChange)
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                    this.UpdateTableSummary(args.NewItems[0], args.Action);
                else if(args.Action == NotifyCollectionChangedAction.Remove)
                {
#if WPF
                    if (source is IBindingList || IsLegacyDataTable)
                        this.UpdateTableSummary();
                    else
#endif
                        this.UpdateTableSummary(args.OldItems[0], args.Action);
                }
                else
                    this.UpdateTableSummary();
            }
            this.RefreshView(args);
        }

        protected void RefreshView(NotifyCollectionChangedEventArgs args)
        {
#if WinRT
            VectorChangedEventArgs vectorEventArgs = null;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    vectorEventArgs = new VectorChangedEventArgs(CollectionChange.ItemInserted, (uint)args.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    vectorEventArgs = new VectorChangedEventArgs(CollectionChange.ItemChanged, (uint)args.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    vectorEventArgs = new VectorChangedEventArgs(CollectionChange.ItemRemoved, (uint)args.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    vectorEventArgs = new VectorChangedEventArgs(CollectionChange.ItemChanged, (uint)args.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    vectorEventArgs = new VectorChangedEventArgs(CollectionChange.Reset, (uint)args.OldStartingIndex);
                    break;
            }

            RaiseVectorChangedEvent(vectorEventArgs);
#endif
            RaiseCollectionChangedEvent(args);
        }

#if WinRT
        public event VectorChangedEventHandler<object> VectorChanged;

        protected void RaiseVectorChangedEvent(VectorChangedEventArgs args)
        {
            if (this.VectorChanged != null)
                this.VectorChanged(this, args);
        }
#endif

        protected internal void RaiseCollectionChangedEvent(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, args);
        }

        protected void RaiseSourceCollectionChangedEvent(NotifyCollectionChangedEventArgs args)
        {
            if (this.SourceCollectionChanged != null)
                this.SourceCollectionChanged(this, args);
        }

        private LiveDataUpdateMode liveDataUpdateMode = LiveDataUpdateMode.Default;
        public LiveDataUpdateMode LiveDataUpdateMode
        {
            get { return liveDataUpdateMode; }
            set { liveDataUpdateMode = value; }
        }

#if WinRT
        
        public IObservableVector<object> CollectionGroups
        {
            get
            {
                if (this.IsGrouping)
                    return this.TopLevelGroup.Groups.ToObservableVector<object>();
                return null;
            }
        }
#else
        public ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                if (this.IsGrouping)
                {
                    return
                        new ReadOnlyObservableCollection<object>(
                            this.TopLevelGroup.Groups.OfType<object>().ToObservableCollection());
                }
                return null;
            }
        }
#endif

        private bool isEmpty = false;
        public bool IsEmpty
        {
            get
            {
                return this.isEmpty;
            }
        }

        #region IAsyncOperation
        public bool HasMoreItems
        {
            get 
            {
                if (this.SourceCollection is ISupportIncrementalLoading)
                {
                    var originalSource = this.SourceCollection as ISupportIncrementalLoading;
                    return originalSource.HasMoreItems;
                }
                return false; 
            }
        }

#if WinRT
        public Windows.Foundation.IAsyncOperation<Windows.UI.Xaml.Data.LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
#else
        public void LoadMoreItemsAsync(uint count)
#endif
        {
            if (this.SourceCollection is ISupportIncrementalLoading)
            {
                var originalSource = this.SourceCollection as ISupportIncrementalLoading;
#if WinRT
                return originalSource.LoadMoreItemsAsync(count);
            }
            return null;
#else
                originalSource.LoadMoreItemsAsync(count);
            }
#endif
        }
        #endregion

        #region CurrentItem

#if WinRT
        public event EventHandler<object> CurrentChanged;
#else
        public event EventHandler CurrentChanged;
#endif

        public event CurrentChangingEventHandler CurrentChanging;

        public bool IsCurrentAfterLast
        {
            get { return this.currentPosition >= this.Records.Count; }
        }

        public bool IsCurrentBeforeFirst
        {
            get { return this.currentPosition < 0; }
        }

        public object CurrentItem
        {
            get { return currentItem; }
        }

        public int CurrentPosition
        {
            get { return currentPosition; }
        }

        private bool IsCurrentInView
        {
            get
            {
                return (0 <= this.CurrentPosition) && (this.CurrentPosition < this.Records.Count);
            }
        }

        protected bool IsCurrentInSync
        {
            get
            {
                if (this.IsCurrentInView)
                {
                    return this.GetItemAt(this.CurrentPosition) == this.CurrentItem;
                }

                return this.CurrentItem == null;
            }
        }

        public virtual bool MoveCurrentTo(object item)
        {
            if (item == null)
                return false;
            if (object.Equals(this.CurrentItem, item) && !IsGrouping && (item != null || this.IsCurrentInView))
            {
                return this.IsCurrentInView;
            }

            var position = -1;
            position = this.Records.IndexOfRecord(item);

            if (this.IsGrouping)
            {
                this.ExpandGroups(this.TopLevelGroup, item);
                this.TopLevelGroup.ResetDisplayElements();
                position = this.Records.IndexOfRecord(item);
            }
            return this.MoveCurrentToPosition(position);
        }

        private void ExpandGroups(Group group, object item)
        {
            var needsRefresh = false;
            if (!group.IsBottomLevel)
            {
                var pgd = (PropertyGroupDescription)this.GroupDescriptions[group.Level];
                foreach (var lowergroup in group.Groups)
                {
                    var c = -1;
                    var converter = pgd != null ? pgd.Converter : null;
                    var key = converter == null
                                  ? propertyAccessProvider.GetValue(item, pgd.PropertyName)
                                  : converter.Convert(item,
                                                      lowergroup.Key != null ? lowergroup.Key.GetType() : typeof(object), null,
                                                      this.Culture.GetCulture());
           
#if WinRT
                    if ((key == null && lowergroup.Key == null) || (key is Nullable && lowergroup.Key is Nullable) || (!(key is IComparable) && !(lowergroup.Key is IComparable)))
#else
                    if ((key == null && lowergroup.Key == null) || (key is DBNull && lowergroup.Key is DBNull) || (!(key is IComparable) && !(lowergroup.Key is IComparable)))
#endif
                    {
                        c = 0;
                    }
#if WinRT
                    else if (key == null || lowergroup.Key == null || key is Nullable || lowergroup.Key is Nullable || !(key is IComparable) || !(lowergroup.Key is IComparable))
#else
                    else if (key == null || lowergroup.Key == null || key is DBNull || lowergroup.Key is DBNull ||
                             !(key is IComparable) || !(lowergroup.Key is IComparable))
#endif
                    {
                        continue;
                    }
                    else
                        c = ((IComparable)lowergroup.Key).CompareTo((IComparable)key);

                    if (c != 0) continue;

                    if (!lowergroup.IsExpanded)
                    {
                        lowergroup.IsExpanded = true;
                        if (!needsRefresh)
                            needsRefresh = true;
                    }
                }
            }
            if (needsRefresh)
            {
                this.Refresh();
            }
        }

        public bool MoveCurrentToFirst()
        {
            return this.MoveCurrentToPosition(0);
        }

        public bool MoveCurrentToLast()
        {
            var position = this.Records.Count - 1;
            return this.MoveCurrentToPosition(position);
        }

        public bool MoveCurrentToNext()
        {
            var position = this.CurrentPosition + 1;
            var count = this.Records.Count;

            return (position <= count) && this.MoveCurrentToPosition(position);
        }

        public virtual bool MoveCurrentToPosition(int index)
        {
            var totalCount = this.IsGrouping ? this.TopLevelGroup.DisplayElements.Count : this.Records.Count;
            if ((index < -1) || (index > totalCount))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if ((index != this.CurrentPosition) || !this.IsCurrentInSync)
            {
                var newItem = ((0 <= index) && (index < this.Records.Count)) ? this.GetItemAt(index) : null;

                if (this.IsGrouping)
                {
                    var nodeEntry = this.TopLevelGroup.DisplayElements[index];
                    if (nodeEntry != null && nodeEntry.IsRecords)
                    {
                        newItem = nodeEntry;
                    }
                    else
                        newItem = null;
                }

                bool isCurrentAfterLast = this.IsCurrentAfterLast;
                bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                if (!this.RaiseCurrentChangingEvent())
                    return false;
                this.SetCurrent(newItem, index);
                this.RaiseCurrentChangedEvent();
                if (this.IsCurrentAfterLast != isCurrentAfterLast)
                {
                    this.OnPropertyChanged("IsCurrentAfterLast");
                }

                if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
                {
                    this.OnPropertyChanged("IsCurrentBeforeFirst");
                }

                this.OnPropertyChanged("CurrentPosition");
                this.OnPropertyChanged("CurrentItem");
            }

            return this.IsCurrentInView;
        }

        protected virtual void SetCurrent(object newItem, int newPosition)
        {
            int count = (newItem != null) ? 0 : (this.IsEmpty ? 0 : this.Records.Count);
            this.SetCurrent(newItem, newPosition, count);
        }

        protected void SetCurrent(object newItem, int newPosition, int count)
        {
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

            this.currentItem = newItem;
            if (this.GroupList == null)
            {
                this.currentPosition = currentItem != null ? this.Records.IndexOfRecord(this.currentItem) : -1;
            }
            else
            {
                this.currentPosition = currentItem != null ? this.TopLevelGroup.IndexOf(((RecordEntry)this.currentItem).Data) : -1;
            }
        }

        protected void RaiseCurrentChangedEvent()
        {
            if (this.CurrentChanged != null)
            {
                this.CurrentChanged(this, EventArgs.Empty);
            }
        }

        protected bool RaiseCurrentChangingEvent()
        {
            if (this.CurrentChanging != null)
            {
                var args = new CurrentChangingEventArgs();
                this.CurrentChanging(this, new CurrentChangingEventArgs());
                return !args.Cancel;
            }

            return true;
        }

        public bool MoveCurrentToPrevious()
        {
            int position = this.CurrentPosition - 1;
            return (position >= -1) && this.MoveCurrentToPosition(position);
        }

        #endregion

        #region IEnumerable

        public virtual int IndexOf(object item)
        {
            return this.Records.IndexOfRecord(item);
        }

        public void Insert(int index, object item)
        {
            var record = this.CreateRecordEntry(item);
            this.Records.Insert(index, record);
        }

        public void RemoveAt(int index)
        {
            this.Records.RemoveAt(index);
        }

        public virtual object GetItemAt(int index)
        {
            return this.Records[index];
        }

        public object this[int index]
        {
            get { return this.GetRecordAt(index); }
            set { Records[index] = CreateRecordEntry(value); }
        }

        public void Add(object item)
        {
            var record = this.CreateRecordEntry(item);
            this.Records.Add(record);
        }

        public void Clear()
        {
            this.Records.Clear();
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            this.Records.CopyTo(array as RecordEntry[], arrayIndex);
        }

#if WinRT
        public bool Remove(object item)
#else
        public void Remove(object item)
#endif
        {
            var sourcelist = GetSourceListCollection();
            if (CanRemove && sourcelist.Contains(item))
            {
                if (!(source is INotifyCollectionChanged)
#if WPF
                    && !(source is IBindingList)
#endif
                    )
                {
                    if (IsGrouping)
                        GroupList.Remove(item, true);
                    else
                    {
                        var record = Records.GetRecord(item);
                        Records.Remove(record);
                    }
                }
                sourcelist.Remove(item);
#if WinRT
                return true;
            }
            return false;
#else
            }
#endif
        }

        public virtual int Count
        {
            get { return this.Records.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public virtual IEnumerator<object> GetEnumerator()
        {
            return this.Records.Cast<object>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Records.GetEnumerator();
        }

        #endregion

        #region DeferRefresh

        public void BeginInit()
        {
            if (this.deferRefreshCount == -1)
                this.endDeferDisposable = this.DeferRefresh();
        }

        public void EndInit()
        {
            if (this.endDeferDisposable != null)
            {
                this.endDeferDisposable.Dispose();
            }
        }

        internal void EndDefer()
        {
            if (deferRefreshCount == 0)
            {
                this.EndDeferInternal();
            }
            this.deferRefreshCount--;
        }

        internal void EndDeferInternal()
        {
            if (!ItemPropertiesSet)
                this.SetItemProperties(this.source);

            IsInEndeferal = true;
            if (IsGrouping && this.topLevelGroup != null)
            {
                this.expandedGroups = new List<ExpandKey>();
                this.PopulateExpandedGroup(this.TopLevelGroup.Groups, 0, expandedGroups);
            }

            RefreshTopLevelGroup();
            this.RefreshSort();

            var hasFilters = this.FilterPredicates.FirstOrDefault(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0) != null;
            if (hasFilters)
            {
                this.RefreshFilter();
            }

            if (IsGrouping && this.expandedGroups != null && this.expandedGroups.Count > 0)
            {
                this.SetExpandedGroups(this.TopLevelGroup.Groups, 0, expandedGroups);
            }

            if (this.expandedGroups != null)
            {
                this.expandedGroups.Clear();
                this.expandedGroups = null;
            }

            this.Refresh();
            this.SetActiveComparer();
            IsInEndeferal = false;
        }

        private class DeferHelper : IDisposable
        {
            // Fields
            private CollectionViewAdv collectionView;
            // Methods
            public DeferHelper(CollectionViewAdv collectionView)
            {
                this.collectionView = collectionView;
            }

            public void Dispose()
            {
                if (this.collectionView != null)
                {
                    this.collectionView.EndDefer();
                    this.collectionView = null;
                }
            }
        }
        
        #endregion

        protected virtual IRecordsList CreateRecords()
        {
            return null;
        }

        protected virtual void EnsureInitialized()
        {
            this.SetFlag(CollectionViewFlags.NeedsRefresh, false);
            UnWireEvents();
            if (IsInEndeferal)
                UnwireNotifyPropertyChangedForUnderlyingSource();
            var temp = this.CreateRecords();

            if (records != null)
            {
                UnWireRecordEvents();
                records.Clear();
                records = null;
            }

            records = temp;
            WireRecordEvents();
            this.UpdateTableSummary();
            WireNotifyPropertyChangedForUnderlyingSource(IsInEndeferal);
            WireEvents();
        }

        private bool _isNotifyPropertyTagged = false;
        internal void WireNotifyPropertyChangedForUnderlyingSource(bool foreceupdate)
        {
#if WPF
            if (this.IsLegacyDataTable)
                return;
#endif
            if (foreceupdate) _isNotifyPropertyTagged = false;
            if (_isNotifyPropertyTagged || SourceCollection == null)
                return;

            _isNotifyPropertyTagged = true;
            foreach (var record in SourceCollection)
                AddNotifyListener(record);
        }

        internal void UnwireNotifyPropertyChangedForUnderlyingSource()
        {
#if WPF
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
            if (!_isNotifyPropertyTagged || SourceCollection == null)
                return;

            _isNotifyPropertyTagged = false;
            foreach (var record in SourceCollection)
                RemoveNotifyListener(record);
        }

        public void AddNotifyListener(object record)
        {
#if WPF
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
            var notifyPropertyChanging = record as INotifyPropertyChanging;
            if (notifyPropertyChanging != null)
                notifyPropertyChanging.PropertyChanging += OnPropertyChanging;
            var notifyPropertyChanged = record as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        public void RemoveNotifyListener(object record)
        {
#if WPF
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
            var notifyPropertyChanged = record as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
            var notifyPropertyChanging = record as INotifyPropertyChanging;
            if (notifyPropertyChanging != null)
                notifyPropertyChanging.PropertyChanging -= OnPropertyChanging;
        }

        protected void InitiateCollectionViewAdv()
        {
            this.sortDescriptions = new SortDescriptionCollection();
            this.summaryRows = new ObservableCollection<ISummaryRow>();
            this.tableSummaryRows = new ObservableCollection<ISummaryRow>();
            this.filterPredicates = new ObservableCollection<IFilterDefinition>();

            this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
            if (source != null)
                this.SetItemProperties(source);
            //this.WireEvents();
        }

        protected virtual void SetSource(IEnumerable _source)
        {
            this.source = _source;
        }

        /// <summary>
        /// Call this method in the derived CollectionView once the constructor is called.
        /// </summary>
        protected void EnsureSourceList()
        {
            this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
        }

        private CollectionViewFlags flags;
        protected void SetFlag(CollectionViewFlags flags, bool value)
        {
            if (value)
            {
                this.flags = flags;
            }
            else
            {
                this.flags &= ~flags;
            }
        }

        private void WireEvents()
        {
            if (this.source is INotifyCollectionChanged)
            {
                var notifyCollectionchanged = source as INotifyCollectionChanged;
                notifyCollectionchanged.CollectionChanged += SourceNotifyCollectionChanged;
            }
#if WPF
            else if (source is IBindingList)
            {
                var listChanged = source as IBindingList;
                listChanged.ListChanged += SourceListChanged;
            }
            if(IsLegacyDataTable)
            {
                if(source is DataTable)
                {
                    (source as DataTable).ColumnChanging += OnDataTableColumnChanging;
                }
                else if (source is DataView)
                {
                    (source as DataView).Table.ColumnChanging += OnDataTableColumnChanging;
                }
            }
#endif
            var sortNotifyCollectionChanged = this.SortDescriptions as INotifyCollectionChanged;
            sortNotifyCollectionChanged.CollectionChanged += OnSortCollectionChanged;

            if (this.TableSummaryRows != null)
            {
                this.TableSummaryRows.CollectionChanged += TableSummaryRows_CollectionChanged;
            }
        }

        public void UnWireEvents()
        {
            if (this.source is INotifyCollectionChanged)
            {
                var notifyCollectionchanged = source as INotifyCollectionChanged;
                notifyCollectionchanged.CollectionChanged -= SourceNotifyCollectionChanged;
            }
#if WPF
            else if (source is IBindingList)
            {
                var listChanged = source as IBindingList;
                listChanged.ListChanged -= SourceListChanged;
                if (IsLegacyDataTable)
                {
                    if (source is DataTable)
                    {
                        (source as DataTable).ColumnChanging -= OnDataTableColumnChanging;
                    }
                    if (source is DataView)
                    {
                        (source as DataView).Table.ColumnChanging -= OnDataTableColumnChanging;
                    }
                }
            }
#endif
            var sortNotifyCollectionChanged = this.SortDescriptions as INotifyCollectionChanged;
            sortNotifyCollectionChanged.CollectionChanged -= OnSortCollectionChanged;

            if (this.groupDescriptions != null)
                this.groupDescriptions.CollectionChanged -= OnGroupDescriptionCollectionChanged;

            UnWireRecordEvents();

            if (this.TableSummaryRows != null)
            {
                this.TableSummaryRows.CollectionChanged -= TableSummaryRows_CollectionChanged;
            }
        }

        private void WireRecordEvents()
        {
            if (this.records != null)
                this.records.CollectionChanged += OnRecordCollectionChanged;
        }

        private void UnWireRecordEvents()
        {
            if (this.records != null)
                this.records.CollectionChanged -= OnRecordCollectionChanged;
        }

        void TableSummaryRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.deferRefreshCount > -1 || this.IsInSuspend)
                return;    
            this.UpdateTableSummary();
        }

        private void OnSortCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SetActiveComparer();

            if (this.deferRefreshCount > -1)
                return;

            this.OnSortDescriptionChanged(e);
        }

        //Check
        void OnGroupDescriptionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        protected virtual void OnSortDescriptionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.deferRefreshCount > -1)
                return;
            this.Refresh();
        }

#if WPF
        public Dispatcher DispatchOwner { get; set; }

        private void OnDataTableColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            var oldValue = e.Row[e.Column];
            if (!_changedTableSummaryInfo.ContainsKey(e.Column.ColumnName))
                _changedTableSummaryInfo.Add(e.Column.ColumnName, oldValue);
            if (!_changedSummaryInfo.ContainsKey(e.Column.ColumnName))
                _changedSummaryInfo.Add(e.Column.ColumnName, oldValue);
            if (!_changedCaptionSummaryInfo.ContainsKey(e.Column.ColumnName))
                _changedCaptionSummaryInfo.Add(e.Column.ColumnName, oldValue);
        }
#endif

        protected bool IsInSourceCollectionChange = false;
        void SourceNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.deferRefreshCount > -1 || this.IsInSuspend)
                return;

            if (!ItemPropertiesSet)
                this.SetItemProperties(this.source);
#if WPF
            if (this.DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    DispatchOwner.Invoke(new Action(() => SourceNotifyCollectionChanged(sender, e)));
                    return;
                }
            }
#endif
            IsInSourceCollectionChange = true;
            RaiseSourceCollectionChangedEvent(e);
            if (this.IsGrouping)
                this.UpdateGroupingModel(sender, e);
            else
                this.UpdateCollectionView(sender, e);

            IsInSourceCollectionChange = false;
        }

#if WPF
        protected virtual void SourceListChanged(object sender, ListChangedEventArgs e)
        {
            if (this.deferRefreshCount > -1 || this.IsInSuspend)
                return;

            if (!ItemPropertiesSet)
                this.SetItemProperties(this.source);

            if (this.DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    DispatchOwner.Invoke(new Action(() => SourceListChanged(sender, e)));
                    return;
                }
            }

            IBindingList source;
            if (e.ListChangedType == ListChangedType.PropertyDescriptorAdded || e.ListChangedType == ListChangedType.PropertyDescriptorChanged ||
                e.ListChangedType == ListChangedType.PropertyDescriptorDeleted) return;
    
            source = sender as IBindingList;
            
            NotifyCollectionChangedEventArgs args = null;
            var newItems = new List<object>();

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (e.NewIndex > -1 && e.NewIndex < source.Count)
                    {
                        newItems.Add(source[e.NewIndex]);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewIndex);
                    }
                    break;

                case ListChangedType.ItemDeleted:
                    var deleteIndex = -1;
                    if (this.IsLegacyDataTable)
                    {
                        for (var i = 0; i < this.Records.Count; i++)
                        {
                            var item = this.Records[i];
                            deleteIndex++;
                            if (((DataRowView) item.Data).Row.RowState == DataRowState.Deleted ||
                                ((DataRowView) item.Data).Row.RowState == DataRowState.Detached)
                            {
                                newItems.Add(item.Data);
                                break;
                            }
                        }
                    }

                    if (newItems.Count == 0)
                    {
                        deleteIndex = -1;
                        var list = this.GetSourceListCollection();
                        foreach (var item in this.Records)
                        {
                            deleteIndex++;
                            if (list.Contains(item.Data)) continue;
                            newItems.Add(item.Data);
                            break;
                        }
                    }

                    if (newItems.Count > 0)
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems, deleteIndex);
                    else
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems, e.NewIndex);
					break;

                case ListChangedType.ItemChanged:
                        if (e.PropertyDescriptor == null)
                    {
                        var list = this.GetSourceListCollection();
                        if (e.OldIndex == -1) //Replace
                        {
                            foreach (var item in this.Records)
                            {
                                if (list.Contains(item.Data)) continue;
                                newItems.Add(item.Data);
                                break;
                            }
                        }
                        else
                            newItems.Add(list[e.NewIndex]);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                    source[e.NewIndex], newItems[0], e.NewIndex);
                    }
                    else                            
                    {
                        if (e.PropertyDescriptor != null)
                            NotifyPropertyChangedHandler(source[e.NewIndex],
                                                         new PropertyChangedEventArgs(e.PropertyDescriptor.Name));
                    }
                    break;

                case ListChangedType.ItemMoved:
                    if (e.NewIndex > -1 && e.NewIndex < source.Count)
                    {
                        newItems.Add(source[e.NewIndex]);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItems, e.NewIndex, e.OldIndex);
                    }
                    else
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
                case ListChangedType.Reset:
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
            }

            if (args != null)
            {
                IsInSourceCollectionChange = true;
                if (e.ListChangedType != ListChangedType.ItemChanged)
                    RaiseSourceCollectionChangedEvent(args);
                if (this.IsGrouping)
                {
                    this.UpdateGroupingModel(sender, args);
                }
                else
                {
                    this.UpdateCollectionView(sender, args);
                }
            }
            IsInSourceCollectionChange = false;
        }
#endif
        protected virtual void OnRecordCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.deferRefreshCount > -1)
                return;
            OnCollectionChanged(e);
        }

        //Updates the collection when source collection changed
        protected virtual void UpdateCollectionView(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecordEntry record = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (this.FilterRecord(item))
                            {
                                record = this.CreateRecordEntry(item);
                                if (!this.ItemPropertiesSet)                                
                                    this.SetItemProperties(this.SourceCollection);
                                
                                var newindex = e.NewStartingIndex;
                                if (this.SortDescriptions.Count > 0 &&
                                    LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping)
                                    newindex = AdjustBeforeAdd(item, e.NewStartingIndex);

                                if (newindex > -1 && newindex < this.Records.Count)
                                    this.Records.Insert(newindex, record);
                                else
                                    this.Records.Add(record);
                            }
                            AddNotifyListener(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            var removeAtIndex = this.Records.IndexOfRecord(item);
                            if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                                this.Records.RemoveAt(removeAtIndex);
                            
                            RemoveNotifyListener(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        var olditem = e.OldItems[0];
                        var index = this.Records.IndexOfRecord(olditem);

                        var newitem = e.NewItems[0];
                        var newindex = e.NewStartingIndex;

                        if (this.FilterRecord(newitem))
                        {
                            if (this.LiveDataUpdateMode != LiveDataUpdateMode.AllowDataShaping)
                            {
                                if (index > -1 && index < this.Records.Count)
                                    this.Records[index] = this.CreateRecordEntry(newitem);
                                else
                                    this.Records.Insert(newindex > -1 ? newindex : this.Records.Count,
                                                        this.CreateRecordEntry(newitem));
                            }
                            else
                            {
                                if (index > -1 && index < this.Records.Count)
                                    this.Records.RemoveAt(index);
                                var comparedindex = this.AdjustBeforeAdd(newitem, e.NewStartingIndex);
                                this.Records.Insert(comparedindex, this.CreateRecordEntry(newitem));
                            }
                        }
                        else
                        {
                            if(index > -1 && index < this.Records.Count)
                                this.Records.RemoveAt(index);
                        }
                        RemoveNotifyListener(olditem);
                        AddNotifyListener(newitem);
                    }
                    break;
#if !SILVERLIGHT && !WP
                case NotifyCollectionChangedAction.Move:
                      {
                        var item = e.OldItems[0];
                        var oldIndex = this.Records.IndexOfRecord(item);

                        var newIndex = e.NewStartingIndex;
                        if (this.FilterRecord(item))
                        {
                            if (oldIndex > -1 && oldIndex < this.Records.Count)
                                this.Records.RemoveAt(oldIndex);
                            record = this.CreateRecordEntry(item);
                            if (this.SortDescriptions.Count > 0 && LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping)
                                newIndex = AdjustBeforeAdd(item, e.NewStartingIndex);
                            if (newIndex > -1 && newIndex < this.Records.Count)
                                this.Records.Insert(newIndex, record);
                            else
                                this.Records.Add(record);
                        }
                    }
                    break;
#endif
                case NotifyCollectionChangedAction.Reset:
                    this.Refresh();
                    break;
            }
        }

        internal void RemoveRecordFromGroup(object record, int index)
        {
#if !SILVERLIGHT
            var removedList = new List<object> { record };
            this.UpdateGroupingModel(this,
                                     new NotifyCollectionChangedEventArgs(
                                         NotifyCollectionChangedAction.Remove, removedList, index));
#else
            this.UpdateGroupingModel(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, record, index));
#endif
        }

        internal void AddRecordToGroup(object record, int index)
        {
            this.UpdateGroupingModel(this,
                                         new NotifyCollectionChangedEventArgs(
                                             NotifyCollectionChangedAction.Add, record, index));
        }

        //Updates the collection when source collection changed
        internal virtual void UpdateGroupingModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var record in e.NewItems)
                    {
                        this.GroupList.Add(record, IsInSourceCollectionChange);
                        if (IsInSourceCollectionChange)
                            this.AddNotifyListener(record);
                    }
                    break;
#if !SILVERLIGHT && !WP7
                case NotifyCollectionChangedAction.Move:
                    break;
#endif
                case NotifyCollectionChangedAction.Remove:
                    foreach (var record in e.OldItems)
                    {
                        this.GroupList.Remove(record, IsInSourceCollectionChange);
                        if (IsInSourceCollectionChange)
                            this.RemoveNotifyListener(record);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveNotifyListener(e.OldItems[0]);
                    this.GroupList.Remove(e.OldItems[0], IsInSourceCollectionChange);
                    this.GroupList.Add(e.NewItems[0], IsInSourceCollectionChange);
                    AddNotifyListener(e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.EndDeferInternal();
                    break;
            }
        }

        #region TopLevelGroup
        
        private void InitializeTopLevelGroup()
        {
            this.topLevelGroup = this.CreateTopLevelGroup();
            var groupBy = this.GroupDescriptions.OfType<PropertyGroupDescription>().Select(g => g.PropertyName).ToArray();

            if (groupBy.Any())
            {
                var result = this.GetGroupResult(groupBy);

                this.topLevelGroup.Populate(result);
                //this.topLevelGroup.CollectionChanged += OnTopLevelGroupCollectionChanged;
                this.OnTopLevelGroupPopulated(this.topLevelGroup);
                this.topLevelGroup.UpdateCaptionSummaries();
                //Check
                //if (this.CanFilter && !IsInEndeferal)
                //{
                //    this.RefreshFilter();
                //}
            }
        }

        protected virtual TopLevelGroup CreateTopLevelGroup()
        {
            return new TopLevelGroup(this);
        }

        internal Func<string, object, object> GetGroupConverterFunc(string propertyName)
        {
            var pgd = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.PropertyName == propertyName);
            if (pgd != null && pgd.Converter != null)
            {
                Func<string, object, object> groupConverterFunc = (columnName, record) =>
                {
                    return pgd.Converter.Convert(record, record.GetType(), null, CultureInfo.CurrentCulture.GetCulture());
                };
                return groupConverterFunc;
            }
            return null;
        }

        internal Expression<Func<string, object, object>> GetGroupConverterExpressionFunc(string propertyName)
        {
            var groupConvertFuncMethod = this.GetGroupConverterFunc(propertyName);
            if (groupConvertFuncMethod != null)
            {
                Expression<Func<string, object, object>> expFunc = (columnName, record) => groupConvertFuncMethod(columnName, record);
                return expFunc;
            }
            return null;
        }

        protected virtual IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            return null;
        }

        protected virtual void OnTopLevelGroupPopulated(TopLevelGroup topLevelGroup)
        {
        }

        protected void RefreshTopLevelGroup()
        {
            if (this.IsGrouping)
            {
                if (this.topLevelGroup != null)
                {
                    //this.topLevelGroup.CollectionChanged -= OnTopLevelGroupCollectionChanged;
                    this.topLevelGroup.Dispose();
                    this.topLevelGroup = null;
                }

                InitializeTopLevelGroup();
                var levelGroup = this.topLevelGroup;
                if (levelGroup != null) levelGroup.SetDirty();
            }
            else if (!this.IsGrouping && this.topLevelGroup != null)
            {
                if (this.topLevelGroup != null)
                {
                    //this.topLevelGroup.CollectionChanged -= OnTopLevelGroupCollectionChanged;
                    this.topLevelGroup.Dispose();
                    this.topLevelGroup = null;
                }

                var topLevelGroup = this.TopLevelGroup;
                if (topLevelGroup != null)
                {
                    topLevelGroup.SetDirty();
                }
            }
        }

        protected internal virtual void RaiseGroupCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.TopLevelGroupCollectionChanged != null)
                this.TopLevelGroupCollectionChanged(this, e);
        }

        protected virtual void OnTopLevelGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var record in e.NewItems)
                        this.Records.Add(record as RecordEntry);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var record in e.OldItems)
                    {
                        var recordindex = this.Records.IndexOfRecord(record);
                        if (recordindex >= 0)
                            this.Records.RemoveAt(recordindex);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Records.Clear();
                    this.UnWireEvents();
                    break;
            }
        }

        #endregion

        #region Filtering
        /// <summary>
        /// Return false if Record passed the Filter conditons
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public virtual bool FilterRecord(object record)
        {
            return this.Filter == null || this.Filter(record);
        }

        public virtual void RefreshFilter()
        {
            
        }

        //Check
        private void ApplyFilters()
        {
            if (this.deferRefreshCount > -1)
                return;
            this.RefreshFilter();
        }

        #endregion

        #region IPropertyChangingEventHandler
        internal Dictionary<string, object> _changedTableSummaryInfo = new Dictionary<string, object>();
        internal Dictionary<string, object> _changedSummaryInfo = new Dictionary<string, object>();
        internal Dictionary<string, object> _changedCaptionSummaryInfo = new Dictionary<string, object>();

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
#if WPF
            if (this.DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    DispatchOwner.Invoke(new Action(() => OnPropertyChanging(sender, e)));
                    return;
                }
            }
#endif
            var propertyChangingArgs = e as PropertyChangingEventArgs;
            if (propertyChangingArgs != null && !string.IsNullOrEmpty(propertyChangingArgs.PropertyName))
            {
                var oldValue = propertyAccessProvider.GetValue(sender, propertyChangingArgs.PropertyName);

                if (!_changedTableSummaryInfo.ContainsKey(propertyChangingArgs.PropertyName))
                    _changedTableSummaryInfo.Add(propertyChangingArgs.PropertyName, oldValue);
                if (!_changedSummaryInfo.ContainsKey(propertyChangingArgs.PropertyName))
                    _changedSummaryInfo.Add(propertyChangingArgs.PropertyName, oldValue);
                if (!_changedCaptionSummaryInfo.ContainsKey(propertyChangingArgs.PropertyName))
                    _changedCaptionSummaryInfo.Add(propertyChangingArgs.PropertyName, oldValue);
            }
            return;
        }

        #endregion

        #region IPropertyChangedEventHandler
        //IPropertyChangedEventHandler
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
#if WPF
            if (this.DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    DispatchOwner.Invoke(new Action(() => OnPropertyChanged(sender, e)));
                    return;
                }
            }
#endif
            if (this.LiveDataUpdateMode != LiveDataUpdateMode.Default)
                NotifyPropertyChangedHandler(sender, e);
            else
            {
                var propertyChangedArgs = e;
                this.OnRecordPropertyChanged(sender, propertyChangedArgs);
            }
            return;
        }

        protected virtual void NotifyPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            IsInPropertyChange = true;
            var propertyChangedArgs = e;

            var canrefreshtablesummary = true;
            var forcerefreshtablesummary = false;
            var canrefreshsummary = true;
            var forecerefreshsummary = false;

            var passesFilter = true;
#if WPF
            if (!IsLegacyDataTable && this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && !sender.Equals(this.CurrentEditItem))
#else
            if (this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && !sender.Equals(this.CurrentEditItem))
#endif
            {
                var hasSort = this.SortDescriptions != null && this.SortDescriptions.Count > 0 &&
                              this.SortDescriptions.FirstOrDefault(
                                  s => s.PropertyName == propertyChangedArgs.PropertyName) !=
                              default(SortDescription);
                var hasGroup = this.GroupDescriptions != null && this.GroupDescriptions.Count > 0 &&
                               this.GroupDescriptions.OfType<PropertyGroupDescription>()
                                   .FirstOrDefault(g => g.PropertyName == propertyChangedArgs.PropertyName) != null;
                if (!this.IsGrouping)
                {
                    if (this.CanFilter)
                        passesFilter = this.FilterRecord(sender);

                    var removeAtIndex = this.Records.IndexOfRecord(sender);
                    if (!passesFilter)
                    {
                        if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                        {
                            this.RemoveRecord(sender);
                            forcerefreshtablesummary = true;
                        }
                        else
                            canrefreshtablesummary = false;
                    }
                    else
                    {
                        if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                        {
                            var comparerIndex = hasSort ? GetComparerIndex(sender, removeAtIndex) : removeAtIndex;
                            if (comparerIndex != removeAtIndex)
                            {
                                var move = false;
                                this.Records.RemoveAt(removeAtIndex);
                                if (this.CurrentItem == sender)
                                {
                                    this.currentItem = null;
                                    move = true;
                                }
                                this.Records.Insert(comparerIndex, Records.CreateRecordEntry(sender));
                                if (move)
                                    this.MoveCurrentTo(sender);
                            }
                        }
                        else
                        {
                            forcerefreshtablesummary = true;
                            var comparerIndex = this.SortDescriptions.Count > 0
                                                    ? AdjustBeforeAdd(sender, removeAtIndex)
                                                    : this.Records.Count;
                            this.Records.Insert(comparerIndex, Records.CreateRecordEntry(sender));
                        }
                    }
                }
                else
                {
                    if (this.CanFilter)
                        passesFilter = this.FilterRecord(sender);

                    var removeAtIndex = this.Records.IndexOfRecord(sender);
                    if (!passesFilter)
                    {
                        if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                        {
                            this.RemoveRecord(sender);
                            forcerefreshtablesummary = true;
                            canrefreshsummary = false;
                        }
                        else
                        {
                            if (hasGroup)
                                this.TopLevelGroup.ResetGroup(sender, e.PropertyName);
                            canrefreshsummary = false;
                            canrefreshtablesummary = false;
                        }
                    }
                    else if (hasGroup || hasSort)
                    {
                        var canremoveadd = true;
                        if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                        {
                            if (!hasGroup)
                            {
                                var group = this.Records[removeAtIndex].Parent as Group;
                                var groupRemoveAtIndex = group.Records.IndexOfRecord(sender);
                                var comparedIndex = this.TopLevelGroup.GetComparerIndex(group, sender,
                                                                                        groupRemoveAtIndex);
                                if (comparedIndex == removeAtIndex)
                                    canremoveadd = false;
                            }
                            if (canremoveadd)
                            {
                                this.RemoveRecordFromGroup(sender, -1);
                                canrefreshsummary = false;
                            }
                        }
                        else
                        {
                            forcerefreshtablesummary = true;
                            if (hasGroup)
                            {
                                canrefreshsummary = false;
                                this.TopLevelGroup.ResetGroup(sender, e.PropertyName);
                            }
                        }

                        if (canremoveadd)
                            this.AddRecordToGroup(sender, -1);
                    }
                    else
                    {
                        if (!(removeAtIndex > -1 && removeAtIndex < this.Records.Count))
                        {
                            forcerefreshtablesummary = true;
                            canrefreshsummary = false;
                            this.AddRecordToGroup(sender, -1);
                        }
                    }
                }
            }

            if (this.IsGrouping && canrefreshsummary)
            {
                var isSummaryRowAffected =
                    (from row in this.SummaryRows from col in row.SummaryColumns select col).FirstOrDefault(
                        s => s.MappingName == propertyChangedArgs.PropertyName) != null;
                var isCaptionSummaryAffected = captionSummaryRow != null &&
                                               (from col in captionSummaryRow.SummaryColumns select col)
                                                   .FirstOrDefault(
                                                       s => s.MappingName == propertyChangedArgs.PropertyName) != null;

                if (isSummaryRowAffected || isCaptionSummaryAffected || forecerefreshsummary)
                {
                    var recordIndex = this.Records.IndexOfRecord(sender);
                    if (recordIndex > -1 && recordIndex < this.Records.Count)
                    {
                        var recordEntry = this.Records[recordIndex];
                        var parentGroup = recordEntry.Parent as Group;
                        if (parentGroup != null)
                        {
                            if (isSummaryRowAffected || forecerefreshsummary)
                                this.UpdateSummaries(parentGroup, sender, propertyChangedArgs.PropertyName);
                            if (isCaptionSummaryAffected || forecerefreshsummary)
                                this.UpdateCaptionSummaries(parentGroup, sender, propertyChangedArgs.PropertyName);
                        }
                    }
                }
            }

            if (canrefreshtablesummary && this.Records != null && this.TableSummaryRows != null)
            {
                var isTableSummaryAffected = (from row in this.TableSummaryRows
                                              from col in row.SummaryColumns
                                              select col).FirstOrDefault(
                                                  s => s.MappingName == propertyChangedArgs.PropertyName) != null;
                if (isTableSummaryAffected || forcerefreshtablesummary)
                {
                    if (forcerefreshtablesummary)
                    {
                        var removeAtIndex = this.Records.IndexOfRecord(sender);
                        if (removeAtIndex < 0)
                        {
                            this.UpdateTableSummary(sender, NotifyCollectionChangedAction.Remove);
                        }
                        else
                        {
                            this.UpdateTableSummary(sender, NotifyCollectionChangedAction.Add);
                        }
                    }
                    else
                        this.UpdateTableSummary(sender, propertyChangedArgs.PropertyName);
                }
            }
            this.OnRecordPropertyChanged(sender, propertyChangedArgs);
            IsInPropertyChange = false;
            _changedTableSummaryInfo.Clear();
            _changedSummaryInfo.Clear();
            _changedCaptionSummaryInfo.Clear();
        }

        protected virtual void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = this.RecordPropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void RemoveRecord(object record)
        {
            if (!IsGrouping)
            {
                var removeAtIndex = this.Records.IndexOfRecord(record);
                if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                {
                    this.Records.RemoveAt(removeAtIndex);
                }
            }
            else
            {
                this.RemoveRecordFromGroup(record, -1);
            }
        }
        
        #endregion

        #region Sorting

        private IComparer<object> activeComparer;
        protected internal void SetActiveComparer()
        {
            if (this.SortDescriptions.Count > 0)
            {
                this.activeComparer = new SortFieldComparer(this.SortDescriptions, this.SortComparers, this.Culture,
                                                            (recrd, propName) =>
                                                                {
                                                                    var provider = this.GetPropertyAccessProvider();
                                                                    return provider != null
                                                                               ? provider.GetValue(recrd, propName)
                                                                               : null;
                                                                });
            }
            else
                this.activeComparer = null;
        }

        protected internal IComparer<object> GetActiveComparer()
        {
            return this.activeComparer;
        }

        protected virtual void RefreshSort()
        {
        }

        /// <summary>
        /// Call this method when you need to find new the ComparedIndex of item based on Sorting
        /// </summary>
        /// <param name="record"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int GetComparerIndex(object record, int index)
        {
            return this.GetComparerIndex(this.Records, record, index, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual int AdjustBeforeAdd(object record, int index)
        {
            return this.GetComparerIndex(this.Records, record, index, false);
        }

        internal virtual int GetComparerIndex(IRecordsEntryList recordsList, object record, int index, bool removeitembeforecheck)
        {
            IList<RecordEntry> internalList;
            if (removeitembeforecheck)
            {
                internalList = recordsList.ToList();
                if (index >= 0 && index < internalList.Count)
                    internalList.RemoveAt(index);
            }
            else
            {
                internalList = recordsList;
            }

            var comparer = new ListOrdinalComparer(internalList, this.GetActiveComparer(), index);

            //Getting the index of record and if record was not present in the Records, it will return -1
            var comparerIndex = this.InternalBinarySearch(internalList, record, comparer);
            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }
            return comparerIndex;
        }

        internal int InternalBinarySearch(IList<RecordEntry> internalList, object record, IComparer<object> comparer)
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

        private class ListOrdinalComparer : IComparer<object>
        {
            // Fields
            private IList<RecordEntry> records;
            private IComparer<object> comparer;
            private int index;

            // Methods
            //internal ListOrdinalComparer(IRecordsList ilFull, IComparer<object> comparer, int removeAtIndex)
            internal ListOrdinalComparer(IList<RecordEntry> ilFull, IComparer<object> comparer, int removeAtIndex)
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
                        var num2 = this.records.IndexOf((RecordEntry) x);
                        return (num2 - index);
                    }
                    return num;
                }
                else
                {
                    var o1 = x as RecordEntry;
                    var o2 = y as RecordEntry;
                    var data1 = o1 != null ? o1.Data : x;
                    var data2 = o2 != null ? o2.Data : y;
                    var num = Equals(data1, data2) ? this.index : this.records.IndexOf((RecordEntry) x);
                    return num - index;
                }
            }
        }

        #endregion
        
        #region UpdateSummaries by SummaryCalculation Optimization

        /// <summary>
        /// Update Table Summary for property changed cases.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="propertyName"></param>
        /// <param name="RecordAction"></param>
        /// <remarks></remarks>
        internal void UpdateTableSummary(object record, string propertyName)
        {
            if (this.TableSummaryRows.Count == 0)
                return;

            var canoptimize = this.EnableSummaryOptimization && _changedTableSummaryInfo.Count > 0;
            if (!canoptimize)
            {
                this.UpdateTableSummary();
                return;
            }

            var counter0 = 0;
            foreach (var summaryRow in this.TableSummaryRows)
            {
                SummaryRecordEntry summaryRecordEntry = null;
                if (counter0 < this.Records.TableSummaries.Count)
                {
                    summaryRecordEntry = this.Records.TableSummaries[counter0];
                    foreach (var summaryColumn in summaryRow.SummaryColumns)
                    {
                        if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName) && summaryColumn.MappingName.Equals(propertyName))
                        {
                            SummaryValue summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                            var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                            if (summaryValue != null && aggregator is ISummaryAdjustible)
                            {
                                if (!UpdateTableSummaryAdjustible(aggregator as ISummaryAdjustible, summaryValue, NotifyCollectionChangedAction.Replace, propertyName, record, _changedTableSummaryInfo))
                                {
                                    UpdateTableSummary();
                                    return;
                                }
                            }
                            //Custom summaries will be refreshed here
                            else if (aggregator is ISummaryAggregate)
                            {
                                CalculateTableSummary(summaryRecordEntry, summaryColumn);
                            }
                        }
                    }
                }
                counter0++;
            }
        }

        /// <summary>
        /// Update Table summary for Add / Remove cases.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="RecordAction"></param>
        /// <remarks></remarks>
        internal void UpdateTableSummary(object record, NotifyCollectionChangedAction RecordAction)
        {
            if (this.TableSummaryRows.Count == 0)
                return;
            
            if (!this.EnableSummaryOptimization)
            {
                this.UpdateTableSummary();
                return;
            }

            if (record is RecordEntry)
                record = (record as RecordEntry).Data;
            var counter0 = 0;
            foreach (var summaryRow in this.TableSummaryRows)
            {
                SummaryRecordEntry summaryRecordEntry = null;
                if (counter0 < this.Records.TableSummaries.Count)
                {
                    summaryRecordEntry = this.Records.TableSummaries[counter0];
                    foreach (var summaryColumn in summaryRow.SummaryColumns)
                    {
                        if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName))
                        {
                            SummaryValue summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                            var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                            if (summaryValue != null && aggregator is ISummaryAdjustible)
                            {
                                if (!UpdateTableSummaryAdjustible((aggregator as ISummaryAdjustible), summaryValue, RecordAction, summaryColumn.MappingName, record, _changedTableSummaryInfo))
                                {
                                    UpdateTableSummary();
                                    return;
                                }
                            }
                            //Custom summaries will be refreshed here
                            else if (aggregator is ISummaryAggregate)
                            {
                                CalculateTableSummary(summaryRecordEntry, summaryColumn);
                            }
                        }
                    }
                }
                counter0++;
            }
        }

        /// <summary>
        /// Update group summaries for replace case
        /// </summary>
        /// <param name="group"></param>
        /// <param name="record"></param>
        /// <param name="propertyName"></param>
        /// <remarks></remarks>
        internal void UpdateSummaries(Group group, object record, string propertyName)
        {
            if (this.SummaryRows.Count == 0)
                return;

            var canOptimize = this.EnableSummaryOptimization && _changedSummaryInfo.Count > 0;
            if (!canOptimize)
            {
                this.TopLevelGroup.UpdateSummaries(group);
                return;
            }
            
            var counter0 = 0;
            var groupRecordEntry = group.Details as GroupRecordEntry;
            foreach (var summaryRow in this.SummaryRows)
            {
                if (groupRecordEntry != null && counter0 < groupRecordEntry.Summaries.Count)
                {
                    if (groupRecordEntry != null && counter0 < this.SummaryRows.Count)
                    {
                        SummaryRecordEntry summaryRecordEntry = null;
                        if (counter0 < groupRecordEntry.Summaries.Count)
                        {
                            summaryRecordEntry = groupRecordEntry.Summaries[counter0];
                        }
                        else
                        {
                            summaryRecordEntry = new SummaryRecordEntry(group, group.Level) { SummaryRow = summaryRow };
                            groupRecordEntry.Summaries.Add(summaryRecordEntry);
                        }
                        foreach (var summaryColumn in summaryRow.SummaryColumns)
                        {
                            if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName) && summaryColumn.MappingName.Equals(propertyName))
                            {
                                var summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                                var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                                if (summaryValue != null && aggregator is ISummaryAdjustible)
                                {
                                    if (!UpdateSummaryAdjustibleForGroup(group, (aggregator as ISummaryAdjustible), summaryValue, NotifyCollectionChangedAction.Replace, propertyName, record, _changedSummaryInfo))
                                    {
                                        this.TopLevelGroup.UpdateSummaries(group);
                                        return;
                                    }
                                }
                                else
                                {
                                    this.TopLevelGroup.UpdateSummaries(group);
                                    return;
                                }
                            }
                        }
                    }
                }
                counter0++;
            }
        }

        /// <summary>
        /// Update Caption summary for replace case
        /// </summary>
        /// <param name="group"></param>
        /// <param name="record"></param>
        /// <param name="propertyName"></param>
        /// <remarks></remarks>
        internal void UpdateCaptionSummaries(Group group, object record, string propertyName)
        {
            if (this.CaptionSummaryRow == null)
                return;

            var canoptimize = this.EnableSummaryOptimization && _changedCaptionSummaryInfo.Count > 0;
            if (!canoptimize)
            {
                this.TopLevelGroup.UpdateCaptionSummariestoTopLevelGroup(group);
                return;
            }

            do
            {
                var summaryRecordEntry = group.SummaryDetails;
                foreach (var summaryColumn in captionSummaryRow.SummaryColumns)
                {
                    if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName) && summaryColumn.MappingName.Equals(propertyName))
                    {
                        SummaryValue summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                        var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                        if (summaryValue != null && aggregator is ISummaryAdjustible)
                        {
                            if (!UpdateSummaryAdjustibleForGroup(group, (aggregator as ISummaryAdjustible), summaryValue, NotifyCollectionChangedAction.Replace, propertyName, record, _changedCaptionSummaryInfo))
                            {
                                this.TopLevelGroup.UpdateCaptionSummariestoTopLevelGroup(group);
                                return;
                            }
                        }
                        else
                        {
                            this.TopLevelGroup.UpdateCaptionSummariestoTopLevelGroup(group);
                            return;
                        }
                    }
                }
                group.SummaryDetails = summaryRecordEntry;
                group = group.Parent;
            } while (group.Parent != null);
        }

        /// <summary>
        /// Update Group Summaries for Add / Remove case.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="record"></param>
        /// <param name="RecordAction"></param>
        /// <remarks></remarks>
        internal void UpdateSummaries(Group group, object record, NotifyCollectionChangedAction RecordAction)
        {
            if (this.SummaryRows.Count == 0)
                return;
            var canOptimize = this.EnableSummaryOptimization && IsInSourceCollectionChange
#if WPF
                              && !(RecordAction == NotifyCollectionChangedAction.Remove && (IsLegacyDataTable || SourceCollection is IBindingList))
#endif
                              ;
            if (!canOptimize)
            {
                this.TopLevelGroup.UpdateSummaries(group);
                return;
            }

            var counter0 = 0;
            var groupRecordEntry = group.Details as GroupRecordEntry;
            if (groupRecordEntry == null)
            {
                this.TopLevelGroup.UpdateSummaries(group);
                return;
            }

            foreach (var summaryRow in this.SummaryRows)
            {
                if (counter0 < this.SummaryRows.Count)
                {
                    SummaryRecordEntry summaryRecordEntry = null;
                    if (counter0 < groupRecordEntry.Summaries.Count)
                    {
                        summaryRecordEntry = groupRecordEntry.Summaries[counter0];
                    }
                    else
                    {
                        summaryRecordEntry = new SummaryRecordEntry(group, group.Level) { SummaryRow = summaryRow };
                        groupRecordEntry.Summaries.Add(summaryRecordEntry);
                    }
                    foreach (var summaryColumn in summaryRow.SummaryColumns)
                    {
                        if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName))
                        {
                            var summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                            var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                            if (summaryValue != null && aggregator is ISummaryAdjustible)
                            {
                                if (!UpdateSummaryAdjustibleForGroup(group, (aggregator as ISummaryAdjustible), summaryValue, RecordAction, summaryColumn.MappingName, record, _changedSummaryInfo))
                                {
                                    this.TopLevelGroup.UpdateSummaries(group);
                                    return;
                                }
                            }
                            else
                            {
                                this.TopLevelGroup.UpdateSummaries(group);
                                return;
                            }
                        }
                    }
                }
                counter0++;
            }
        }

        /// <summary>
        /// Update caption summary for Add / Remove case.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="record"></param>
        /// <param name="RecordAction"></param>
        /// <remarks></remarks>
        internal void UpdateCaptionSummaries(Group group, object record, NotifyCollectionChangedAction RecordAction)
        {
            if (this.CaptionSummaryRow == null)
                return;
            var canOptimize = EnableSummaryOptimization && IsInSourceCollectionChange
#if WPF
                              && !(RecordAction == NotifyCollectionChangedAction.Remove && (IsLegacyDataTable || SourceCollection is IBindingList))
#endif
                              ;
            if(!canOptimize)
            {
                this.TopLevelGroup.UpdateCaptionSummaries(group);
                return;
            }

            var summaryRecordEntry = group.SummaryDetails;
            if (summaryRecordEntry != null)
            {
                foreach (var summaryColumn in captionSummaryRow.SummaryColumns)
                {
                    if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName))
                    {
                        SummaryValue summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                        var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                        if (summaryValue != null && aggregator is ISummaryAdjustible)
                        {
                            if (!UpdateSummaryAdjustibleForGroup(group, (aggregator as ISummaryAdjustible), summaryValue, RecordAction, summaryColumn.MappingName, record, _changedCaptionSummaryInfo))
                            {
                                this.TopLevelGroup.UpdateCaptionSummaries(group);
                                return;
                            }
                        }
                        else
                        {
                            this.TopLevelGroup.UpdateCaptionSummaries(group);
                            return;
                        }
                    }
                }
            }
            else
            {
                this.TopLevelGroup.UpdateCaptionSummaries(group);
                return;
            }
        }

        /// <summary>
        /// Update Summary for Group by Adjustible aggregate.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="adjustible"></param>
        /// <param name="summaryValue"></param>
        /// <param name="RecordAction"></param>
        /// <param name="propertyName"></param>
        /// <param name="record"></param>
        /// <param name="_changedSummaryInfoDictionary"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool UpdateSummaryAdjustibleForGroup(Group group, ISummaryAdjustible adjustible, SummaryValue summaryValue, NotifyCollectionChangedAction RecordAction, string propertyName, object record, Dictionary<string, object> _changedSummaryInfoDictionary)
        {
            foreach (var key in summaryValue.AggregateValues.Keys.ToArray())
            {
                object oldValue = null;
                object newValue = null;

                var recordCount = GetRecordsCount(group, 0);
                var oldrecordCount = recordCount;
                if (RecordAction == NotifyCollectionChangedAction.Add)
                {
                    newValue = this.propertyAccessProvider.GetValue(record, propertyName);
                    oldrecordCount -= 1;
                }
                else if (RecordAction == NotifyCollectionChangedAction.Remove)
                {
                    if (key.Equals("Max") || key.Equals("Min"))
                        return false;
                    oldValue = this.propertyAccessProvider.GetValue(record, propertyName);
                    oldrecordCount += 1;
                }
                else if (RecordAction == NotifyCollectionChangedAction.Replace)
                {
                    if (_changedSummaryInfoDictionary.ContainsKey(propertyName))
                        oldValue = _changedSummaryInfoDictionary[propertyName];
                    else
                        return false;
                    newValue = this.propertyAccessProvider.GetValue(record, propertyName);
                }
                else
                    throw new NotImplementedException(RecordAction.ToString() + "NotImplementedException ");

                var value = summaryValue.AggregateValues[key];
                if (key.Equals("Average"))
                    summaryValue.AggregateValues[key] = (double)adjustible.AdjustSummaryCalculation((double)value * oldrecordCount, oldValue, newValue, key) / recordCount;
                else
                    summaryValue.AggregateValues[key] = adjustible.AdjustSummaryCalculation(value, oldValue, newValue, key);
            }
            return true;
        }

        /// <summary>
        /// Update Table Summary for Adjustible aggregate.
        /// </summary>
        /// <param name="adjustible"></param>
        /// <param name="summaryValue"></param>
        /// <param name="RecordAction"></param>
        /// <param name="propertyName"></param>
        /// <param name="record"></param>
        /// <param name="_changedSummaryInfoDictionary"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool UpdateTableSummaryAdjustible(ISummaryAdjustible adjustible, SummaryValue summaryValue, NotifyCollectionChangedAction RecordAction, string propertyName, object record, Dictionary<string, object> _changedSummaryInfoDictionary)
        {
            foreach (var key in summaryValue.AggregateValues.Keys.ToArray())
            {
                object oldValue = null;
                object newValue = null;

                var recordCount = this.Records.Count;
                var oldrecordCount = recordCount;
                if (RecordAction == NotifyCollectionChangedAction.Add)
                {
                    newValue = this.propertyAccessProvider.GetValue(record, propertyName);
                    oldrecordCount -= 1;
                }
                else if (RecordAction == NotifyCollectionChangedAction.Remove)
                {
                    if (key.Equals("Max") || key.Equals("Min"))
                        return false;
                    oldValue = this.propertyAccessProvider.GetValue(record, propertyName);
                    oldrecordCount += 1;
                }
                else if (RecordAction == NotifyCollectionChangedAction.Replace)
                {
                    if (_changedSummaryInfoDictionary.ContainsKey(propertyName))
                        oldValue = _changedSummaryInfoDictionary[propertyName];
                    else
                        return false;
                    newValue = this.propertyAccessProvider.GetValue(record, propertyName);
                }
                else
                    throw new NotImplementedException(RecordAction.ToString() + "NotImplementedException ");
                var value = summaryValue.AggregateValues[key];
                if (key.Equals("Average"))
                    summaryValue.AggregateValues[key] = (double)adjustible.AdjustSummaryCalculation((double)value * oldrecordCount, oldValue, newValue, key) / recordCount;
                else
                    summaryValue.AggregateValues[key] = adjustible.AdjustSummaryCalculation(value, oldValue, newValue, key);
            }
            return true;
        }

        /// <summary>
        /// Returns Records count for the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private int GetRecordsCount(Group group, int count)
        {
            if (!group.IsBottomLevel)
            {
                foreach (var childGroup in group.Groups)
                    count += GetRecordsCount(childGroup, count);
                return count;
            }
            else
            {
                count = group.Records.Count;
                return count;
            }
        }

        #endregion
        
        #region TableSummary

        /// <summary>
        /// Calculate and aggregate summary value for given summary record entry.
        /// </summary>
        /// <param name="summaryRecordEntry"></param>
        /// <param name="summaryColumn"></param>
        /// <remarks></remarks>
        private void CalculateTableSummary(SummaryRecordEntry summaryRecordEntry, ISummaryColumn summaryColumn)
        {
            var summaryItems = new Dictionary<string, object>();
            IEnumerable items = this.Records.Select(o => o.Data).ToArray().OfQueryable();
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

        internal virtual void UpdateTableSummary()
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

                IEnumerable items = this.Records.Select(o => o.Data).ToArray().OfQueryable();
                foreach (var summaryColumn in summaryRow.SummaryColumns)
                {
                    if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
                    {
                        CalculateTableSummary(summaryRecordEntry, summaryColumn);
                    }
                }
                counter++;
            }
        }

        #endregion

        #region Enums
        protected enum CollectionViewFlags
        {
            CachedIsEmpty = 0x200,
            IsCurrentAfterLast = 0x10,
            IsCurrentBeforeFirst = 8,
            IsDataInGroupOrder = 0x40,
            IsDynamic = 0x20,
            IsMultiThreadCollectionChangeAllowed = 0x100,
            NeedsRefresh = 0x80,
            ShouldProcessCollectionChanged = 4,
            UpdatedOutsideDispatcher = 2
        }
        #endregion

        #region Unbound Columns
       
        public virtual Func<string, object, object>  GetFunc(string propertyName)
        {
#if WPF
            if (this.isITypedListSource)
            {
                return this.GetITypedListFunc(propertyName);
            }
#endif
            return null;
        }

        public virtual Expression<Func<string, object, object>> GetExpressionFunc(string propertyName)
        {
#if WPF
            if (this.isITypedListSource)
            {
                return this.GetITypedListExpressionFunc(propertyName);
            }
#endif
            return null;
        }
#if WPF
        private Func<string, object, object> typedListFunc = null;
        internal Func<string, object, object> GetITypedListFunc(string propertyName)
        {
            var typedList = this.SourceCollection as ITypedList;
            if (this.typedListFunc == null)
            {
                this.typedListFunc = (columnName, record) =>
                    {
                        var pdc = typedList.GetItemProperties(null);
                        if (pdc != null)
                        {
                            var pd = pdc.GetPropertyDescriptor(columnName);
                            if (pd != null)
                                return pd.GetValue(record);
                        }
                        return null;
                    };
            }
            return this.typedListFunc;
        }

        internal Expression<Func<string, object, object>> GetITypedListExpressionFunc(string propertyName)
        {
            var recordFunc = this.GetITypedListFunc(propertyName);
            if (recordFunc != null)
            {
                Expression<Func<string, object, object>> expFunc =
                    (columnName, record) => recordFunc(columnName, record);
                return expFunc;
            }
            return null;
        }
#endif
        private Func<string, object, object> displayValueFunc;
        public virtual Func<string, object, object> GetDisplayValueFunc(string propertyName)
        {
            if (this.displayValueFunc == null)
            {
                this.displayValueFunc = (columnName, record) =>
                    {
                        var provider = this.GetPropertyAccessProvider();
                        return provider.GetFormattedValue(record, columnName);
                    };
            }
            return displayValueFunc;
        }

        public virtual Expression<Func<string, object, object>> GetDisplayValueExpressionFunc(string propertyName)
        {
            var func = this.GetDisplayValueFunc(propertyName);
            return (columnName, record) => func(propertyName, record);
        }

        #endregion

        #region PersistGroupExpand State

        private List<ExpandKey> expandedGroups;

        private void SetExpandedGroups(List<Group> groups, int level, List<ExpandKey> expandgroup)
        {
            if (expandgroup.Count <= 0)
                return;
            foreach (var g in groups)
            {
                var index = expandgroup.IndexOf(new ExpandKey() { Key = g.Key as IComparable, Level = level });
                var found = index >= 0;
                if (!found) continue;
                g.IsExpanded = true;
                if (g.Groups != null && expandgroup[index].expandedGroups != null)
                {
                    this.SetExpandedGroups(g.Groups, level + 1, expandgroup[index].expandedGroups);
                }
            }
        }

        private void PopulateExpandedGroup(List<Group> groups, int level, List<ExpandKey> expandgroup)
        {
            foreach (var g in groups)
            {
                if (!g.IsExpanded) continue;
                var expandkey = new ExpandKey { Key = g.Key as IComparable, Level = level };
                if (g.Groups != null)
                {
                    this.PopulateExpandedGroup(g.Groups, level + 1, expandkey);
                }
                if (expandgroup == null)
                    expandgroup = new List<ExpandKey>();
                expandgroup.Add(expandkey);
            }
        }

        private void PopulateExpandedGroup(List<Group> groups, int level, ExpandKey expandKey)
        {
            foreach (var g in groups)
            {
                if (!g.IsExpanded) continue;
                var expandchildkey = new ExpandKey { Key = g.Key as IComparable, Level = level };
                if (g.Groups != null)
                {
                    this.PopulateExpandedGroup(g.Groups, level + 1, expandchildkey);
                }
                if (expandKey.expandedGroups == null)
                    expandKey.expandedGroups = new List<ExpandKey>();
                expandKey.expandedGroups.Add(expandchildkey);
            }
        }

        private class ExpandKey : IComparable<ExpandKey>
        {
            public IComparable Key { get; set; }
            public int Level { get; set; }

            public List<ExpandKey> expandedGroups;

            public override bool Equals(object obj)
            {
                var otherK = obj as ExpandKey;
                return otherK != null && (this.Level == otherK.Level &&
                                          (
                                              (this.Key == null && otherK.Key == null) ||
                                              (this.Key != null && this.Key.Equals(otherK.Key))
                                          ));

                //return this.Level == otherK.Level && this.Key.Equals(otherK.Key);
                // return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return Level.GetHashCode() | Key.GetHashCode();
            }

            #region IComparable<ExpandKey> Members

            public int CompareTo(ExpandKey other)
            {
                int c = this.Level.CompareTo(other.Level);
                if (c == 0)
                    c = this.Key.CompareTo(other.Key);

                return c;
            }

            #endregion
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            UnwireNotifyPropertyChangedForUnderlyingSource();
            UnWireEvents();
            if (this.records != null)
            {
                this.records.ReomveAll();
                this.records.Dispose();
                this.records = null;
            }
            if (sortDescriptions != null)
            {
                sortDescriptions.Clear();
                sortDescriptions = null;
            }

            if (this.filterPredicates != null)
            {
                this.filterPredicates.Clear();
                this.filterPredicates = null;
            }

            if (this.groupDescriptions != null)
            {
                this.groupDescriptions.Clear();
                this.groupDescriptions = null;
            }

            if (this.SortComparers != null)
            {
                this.SortComparers.Clear();
            }

            if (this.source != null)
                this.source = null;

            if (this.Filter != null)
                this.Filter = null;

            this.propertyAccessProvider = null;
        }

        #endregion

        public bool CanFilter
        {
            get { return this.Filter != null || this.FilterPredicates.Count > 0; }
        }

#if !WinRT
     
        public bool CanGroup
        {
            get { return true; }
        }

        public bool CanSort
        {
            get { return true; }
        }
        
#endif
        bool autoExpandGroups;
        public bool AutoExpandGroups
        {
            get
            {
                return autoExpandGroups;
            }
            set
            {
                autoExpandGroups = value;
            }
        }

        #region IEditableCollectionView

        #region fields

        protected object editItem;

        #endregion

        public bool CanCancelEdit
        {
            get { return editItem is IEditableObject; }
        }

        public void CancelEdit()
        {
            if (editItem == null)
                throw new InvalidOperationException("EditItem is null to CancelEdit");

            if (editItem is IEditableObject)
                (editItem as IEditableObject).CancelEdit();

            this.editItem = null;
        }

        public virtual RecordEntry GetRecordAt(int index)
        {
            if (index < this.Records.Count)
            {
                return this.Records[index];
            }
            return null;
        }

        public virtual void EndEdit()
        {
            if (editItem == null)
                throw new InvalidOperationException("Already another Item is in EditMode");

            if (editItem is IEditableObject)
                (editItem as IEditableObject).EndEdit();
            editItem = null;
        }

        protected bool IsInCommitEdit = false;
        public virtual  void CommitEdit()
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
            if (this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && !this.IsLegacyDataTable && editItem!=CurrentAddItem)
#else
            if (this.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping && editItem != CurrentAddItem)
#endif
            {
                var passesFilter = true;
                if (this.CanFilter)
                    passesFilter = this.FilterRecord(editItem);
                
                var removeAtIndex = this.Records.IndexOfRecord(editItem);
                if (!passesFilter)
                {
                    if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                    {
                        this.RemoveRecord(editItem);
                        refreshtablesummary = true;
                    }
                }
                else
                {
                    if (!this.IsGrouping)
                    {
                        if (this.SortDescriptions.Count > 0)
                        {
                            var comparerIndex = GetComparerIndex(editItem, removeAtIndex);
                            if (comparerIndex != removeAtIndex)
                            {
                                if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                                    this.Records.RemoveAt(removeAtIndex);
                                else
                                    throw new InvalidOperationException("EditItem is not in View to Commit");
                                var record = this.Records.CreateRecordEntry(editItem);
                                if (comparerIndex > -1 && comparerIndex < this.Records.Count)
                                    this.Records.Insert(comparerIndex, record);
                                else
                                    this.Records.Add(record);
                            }
                        }
                    }
                    else
                    {
                        var group = this.Records[removeAtIndex].Parent as Group;
                        var canreset = true;
                        var comparedIndex = -1;
                        if (this.TopLevelGroup.IsValidGroup(group, editItem))
                        {
                            if (this.SortDescriptions.Count > 0)
                            {
                                var groupremoveAtIndex = group.Records.IndexOfRecord(editItem);
                                comparedIndex = this.TopLevelGroup.GetComparerIndex(group, editItem, groupremoveAtIndex);
                                if (groupremoveAtIndex == comparedIndex)
                                    canreset = false;
                            }
                            else
                                canreset = false;
                        }

                        if (canreset)
                        {
                            if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                                this.RemoveRecordFromGroup(editItem, -1);
                            else
                                throw new InvalidOperationException("EditItem is not in View to Commit");
                            this.AddRecordToGroup(editItem, comparedIndex);
                        }
                    }
                }
            }
#if WPF
            if (!IsLegacyDataTable && refreshtablesummary && this.LiveDataUpdateMode != LiveDataUpdateMode.Default)
#else
            if (refreshtablesummary && this.LiveDataUpdateMode != LiveDataUpdateMode.Default)
#endif
                this.UpdateTableSummary(editItem, NotifyCollectionChangedAction.Remove);
#if WPF
            else if (IsLegacyDataTable && LiveDataUpdateMode != LiveDataUpdateMode.Default)
                this.UpdateTableSummary();
#endif
            editItem = null;
            IsInCommitEdit = false;
        }

        public object CurrentEditItem
        {
            get { return editItem; }
        }

        public void EditItem(object item)
        {
            if (editItem != null)
                throw new InvalidOperationException("Not able to BeginEdit, EditItem is already set");
            editItem = item;
            if (item is IEditableObject)
                (item as IEditableObject).BeginEdit();

        }

        public bool IsEditingItem
        {
            get { return editItem != null; }
        }

        public virtual object AddNew()
        {
            if (CurrentAddItem != null)
                throw new InvalidOperationException("CurrentAddItem should be null");
            var itemType = this.SourceCollection.GetItemType(true);
            if (itemType != null)
            {
                CurrentAddItem = itemType.CreateNew();
            }

            return CurrentAddItem;
        }

        public virtual bool CanAddNew
        {
            get { return this.SourceCollection != null; }
        }
        
        public bool CanRemove
        {
           get { return (GetSourceListCollection() != null); }
        }

        public virtual void CancelNew()
        {
            if (CurrentAddItem == null)
                throw new InvalidOperationException("CurrentAddItem sholud not be null to perform this operation");
            CurrentAddItem = null;
        }

        public virtual void CommitNew()
        {
            if (CurrentAddItem == null)
                throw new InvalidOperationException("CurrentAddItem should not null when CommitNew");
            var passesFilter = true;
            if (this.CanFilter)
                passesFilter = this.FilterRecord(CurrentAddItem);
          
            this.IsInSuspend = true;
            this.IsInSourceCollectionChange = true;
            var source = this.GetSourceListCollection();
            source.Add(CurrentAddItem);
            this.AddNotifyListener(CurrentAddItem);
            if (passesFilter)
            {
                if (!this.IsGrouping)
                {
                    int newIndex = -1;
                    var newRecord = this.Records.CreateRecordEntry(CurrentAddItem);
                    if (!this.ItemPropertiesSet)
                        this.SetItemProperties(this.SourceCollection);
                    if (this.SortDescriptions.Count > 0 && LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping)
                        newIndex = AdjustBeforeAdd(CurrentAddItem, -1);

                    if (newIndex > -1 && newIndex < this.Records.Count)
                        this.Records.Insert(newIndex, newRecord);
                    else
                        this.Records.Add(newRecord);
                }
                else
                {
                    this.GroupList.Add(CurrentAddItem, true);
                }
            }
            this.CurrentAddItem = null;
            this.IsInSourceCollectionChange = false;
            this.IsInSuspend = false;
        }

        object currentAddItem;
        public object CurrentAddItem
        {
            get { return currentAddItem; }
            set { currentAddItem = value; OnPropertyChanged("CurrentAddItem"); }
        }

        public bool IsAddingNew
        {
            get { return CurrentAddItem != null; }
        }

        [Obsolete]
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }

    public interface IUnboundExpressionFunc
    {
        /// <summary>
        /// Custom Function to enable runtime customized objects over default operations other than LINQ queries.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        Func<string, object, object> GetFunc(string propertyName);

        /// <summary>
        /// Custom expression function to enable runtime customized objects into LINQ queries.
        /// </summary>
        Expression<Func<string, object, object>> GetExpressionFunc(string propertyName);
    }

    public interface IFilterExt
    {
        System.Linq.Expressions.Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression);

        System.Linq.Expressions.Expression GetPredicateExpression(IQueryable source,
                                                                  out ParameterExpression paramExpression,
                                                                  string columnName, bool returncolExpression);
    }
}
