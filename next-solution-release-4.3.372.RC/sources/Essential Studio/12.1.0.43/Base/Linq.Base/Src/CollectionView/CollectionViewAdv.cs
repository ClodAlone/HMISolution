#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Windows.Data;
	using Syncfusion.Linq;
#if SyncfusionFramework4_0
	using Syncfusion.Dynamic;
#endif

#if !SILVERLIGHT
	using System.Data;
	using System.Windows;
	using System.Xml;
	using System.Globalization;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Syncfusion.ComponentModel;
	using System.Windows.Threading;
	using System.Threading;
#else
	using ArrayList = System.Collections.Generic.List<object>;
	using System.Globalization;

#endif

	/// <summary>
	/// ICollectionViewAdv is an extended interface from ICollectionView that implements
	/// support for Grouping structure, Summaries, Table Summaries, Filters.
	/// CollectionViewAdv is an abstract class that implements ICollectionViewAdv and
	/// implements most of the basic details. By sub-classing CollectionViewAdv the
	/// following properties could be extended, 
	/// <para> </para>
	/// <list type="bullet">
	/// <item>
	/// <description>Sorting</description></item>
	/// <item>
	/// <description>Filtering</description></item>
	/// <item>
	/// <description>Grouping</description></item></list>
	/// <para>Summaries</para>
	/// <para> </para>
	/// <para>It also implements IUnboundExpressionFunc that extends the usage for
	/// complex Grouping / Summaries / Filtering etc., If there is a complex data
	/// structure and that follows a common pattern, simply overriding the
	/// GetExpressionFunc should automatically process all LINQ queries with ease. The
	/// ITypedList data source is handled by generating an expression to retrieve the
	/// values,</para>
	/// <para></para>
	/// <code lang="C#">        
	/// internal Expression&lt;Func&lt;string, object, object&gt;&gt; GetITypedListFunc(string propertyName)
	///         {
	///             if (this.isITypedListSource)
	///             {
	///                 var typedList = this.SourceCollection as ITypedList;
	///                 Func&lt;string, object, object&gt; recordFunc = (columnName,
	/// record) =&gt;
	///                 {
	///                     var pdc = typedList.GetItemProperties(null);
	///                     if (pdc != null)
	///                     {
	///                         var pd = pdc.GetPropertyDescriptor(propertyName);
	///                         if (pd != null)
	///                         {
	///                             return pd.GetValue(record);
	///                         }
	///                     }
	/// 
	///                     return null;
	///                 };
	/// 
	///                 Expression&lt;Func&lt;string, object,
	/// object&gt;&gt; expFunc = (columnName, record) =&gt; recordFunc(columnName,
	/// record);
	///                 return expFunc;
	///             }
	/// 
	///             return null;
	///         }
	/// </code>         
	/// </summary>
	public abstract class CollectionViewAdv :  ICollectionViewAdv, IUnboundExpressionFunc
	{
		private enum CollectionViewFlags
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

		#region Helper classes

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
					//if (this.collectionView.records != null)
					//    if ((this.collectionView.records as EnumerableRecordsWrapper) != null)
					//        (this.collectionView.records as EnumerableRecordsWrapper).RemoveNotifyListener();
					//this.collectionView.records.SuspendUpdates();
					//this.collectionView.records.Clear();
					//this.collectionView.records.ResumeUpdates();
					this.collectionView.EndDefer();
					this.collectionView = null;
				}
			}
		}

		#endregion

		private int deferRefreshCount = -1;

		#region cTor

        public CollectionViewAdv(IEnumerable source, Type sourceType)
        {
#if SILVERLIGHT
			if (source is ICollectionView)
			{
				this.source = ((ICollectionView)source).SourceCollection;
				this.CollectionView = source as ICollectionView;
			}
			else
#endif
            this.source = source;
            this.summaryRows = new ObservableCollection<ISummaryRow>();
            this.filters = new ObservableCollection<IFilterDefinition>();
            this.tableSummaryRows = new ObservableCollection<ISummaryRow>();
            this.relations = new ObservableCollection<IRelationDefinition>();
            this.SetSourceType(sourceType);
            this.SetItemProperties(source);
            this.PrepareSortAndFilter();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionViewAdv"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		public CollectionViewAdv(IEnumerable source)
		{
#if SILVERLIGHT
			if (source is ICollectionView)
			{
				this.source = ((ICollectionView)source).SourceCollection;
				this.CollectionView = source as ICollectionView;
			}
			else
#endif
			this.source = source;
			this.summaryRows = new ObservableCollection<ISummaryRow>();
			this.filters = new ObservableCollection<IFilterDefinition>();
			this.tableSummaryRows = new ObservableCollection<ISummaryRow>();
			this.relations = new ObservableCollection<IRelationDefinition>();
			this.SetItemProperties(source);
			this.PrepareSortAndFilter();
		}

#if SILVERLIGHT
		private ICollectionView CollectionView
		{
			get;
			set;
		}
#endif

		void TableSummaryRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.deferRefreshCount > -1 || this.IsInSuspend)
			{
				return;
			}
			this.UpdateTableSummary();
		}
#if !SILVERLIGHT

		/// <summary>
		/// Gets or sets the Dispatcher of the UI object associated with this CollectionViewAdv.
		/// </summary>
		public Dispatcher DispatchOwner { get; set; }
#endif

		void notifyCollectionchanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
#if !SILVERLIGHT
			if (DispatchOwner != null)
			{
				if (DispatchOwner.Thread != Thread.CurrentThread)
				{
					DispatchOwner.Invoke(new NotifyCollectionChangedEventHandler(notifyCollectionchanged_CollectionChanged), new object[] { sender, e });
					return;
				}
			}
#endif
			if (this.deferRefreshCount > -1 || this.IsInSuspend)
			{
				return;
			}

			if (this.IsEditingItem && e.Action == NotifyCollectionChangedAction.Add)
			   this.CommitEdit();

            if (!ItemPropertiesSet)
                this.SetItemProperties(this.source);

            IsInSourceCollectionChange = true;
			if (this.IsGrouping)
			{
				this.IsInSuspend = true;
				this.UpdateGroupingModel(sender, e, String.Empty);
				this.IsInSuspend = false;
			}
			else
			{
				this.UpdateCollectionView(sender, e);
			}
            IsInSourceCollectionChange=false;

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (var newItem in e.NewItems)
					{
						this.AddNotifyListener(newItem);             
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var oldItem in e.OldItems)
					{
						this.RemoveNotifyListener(oldItem);
					}
					break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var oldItem in e.OldItems)
                        this.RemoveNotifyListener(oldItem);
                    foreach (var newItem in e.NewItems)
                        this.AddNotifyListener(newItem);
                    break;
			}

			//if (e.Action != NotifyCollectionChangedAction.Replace)
			//this.UpdateTableSummary();
		}

#if !SILVERLIGHT
		void listChanged_ListChanged(object sender, ListChangedEventArgs e)
		{
            if (DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    DispatchOwner.Invoke(new ListChangedEventHandler(listChanged_ListChanged), new object[] {sender, e});
                    return;
                }
            }
			if (this.deferRefreshCount > -1 || this.IsInSuspend)
			{
				return;
			}

            if (!ItemPropertiesSet)
                this.SetItemProperties(this.source);

			NotifyCollectionChangedEventArgs arg = null;
			var newItems = new List<object>();
			IList source = null;
			if (e.ListChangedType != ListChangedType.Reset && e.ListChangedType != ListChangedType.PropertyDescriptorAdded && e.ListChangedType != ListChangedType.PropertyDescriptorChanged && e.ListChangedType != ListChangedType.PropertyDescriptorDeleted)
			{
				source = sender as IList;
			}
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					if (e.NewIndex > -1 && e.NewIndex < source.Count)
					{
						newItems.Add(source[e.NewIndex]);
						arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewIndex);
						if (this.IsEditingItem)
							this.CommitEdit();
					}
					break;

				case ListChangedType.ItemDeleted:
                    int deleteIndex = -1;
                    object data = null;
                    bool isDataRowView = false;
                    for (int i = 0; i < this.Records.Count; i++) //foreach (RecordEntry item in this.Records)
                    {
                        RecordEntry item = this.Records[i];
                        deleteIndex++;
                        //If the underlying collection is Bindinglist then this loop will terminate. Else if the underlying collection is datatable then the else part will find the row which is deleted.
                        if (!(item.Data is DataRowView))
                        {
                            isDataRowView = true;
                            break;
                        }
                        else if (((DataRowView)item.Data).Row.RowState == DataRowState.Deleted || ((DataRowView)item.Data).Row.RowState == DataRowState.Detached)
                        {
                            data = item.Data;
                            break;
                        }
                        else if(((DataRowView)item.Data).Row.RowState == DataRowState.Added || ((DataRowView)item.Data).Row.RowState == DataRowState.Modified)
                        {
                            if (source.Contains(item.Data)) continue;
                            else
                            {
                                data = item.Data;
                                break;
                            }
                        }
                    }
                    //Since we cannot get the actual record that is being deleted, here we are looping through the sourcelist and our internal records list and get the row which is missing and deleting from the REcords list.
                    if (isDataRowView)
                    {
                        var list = this.GetSourceListCollection();
                        foreach (var item in this.Records)
                        {
                            if (!(list.Contains(item.Data)))
                                data = item.Data;
                        }
                    }                    
					if (data != null)
					{
						newItems.Add(data);
					}
                    if (newItems.Count > 0)
                    {
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems, deleteIndex);
                    }
                    else
                    {
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems, e.NewIndex);
                    }
					break;
				case ListChangedType.ItemChanged:
                    bool result = true;
                    if (this.IsLegacyDataTable)
                    {
                        var propertyName = string.Empty;
                        if (e.PropertyDescriptor != null)
                            propertyName = e.PropertyDescriptor.Name;
                        NotifyPropertyChangedHandler(source[e.NewIndex],
                                                         new PropertyChangedEventArgs(propertyName), ref result);
                    }
                    else
                    {
                        if (!this.IsEditingItem)
                        {
                            if (this.SortingOptions == Data.SortingOptions.Default)
                            {
                                if (e.OldIndex == -1)// According to binding list oldIndex returns as -1 when the data is replaced.
                                {
                                    foreach (var item in this.Records)
                                        if (source.Contains(item.Data)) continue;
                                        else
                                        {
                                            newItems.Add(item.Data);
                                            break;
                                        }
                                    if (newItems.Count == 0)
                                        newItems.Add(source[e.NewIndex]);
                                }
                                else // property changed
                                    newItems.Add(source[e.NewIndex]);
                                arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, source[e.NewIndex], newItems[0], e.NewIndex);
                            }
                        }

                        var propertyName = string.Empty;
                        if (e.PropertyDescriptor != null)
                            propertyName = e.PropertyDescriptor.Name;
                        this.OnRecordPropertyChanged(source[e.NewIndex], new PropertyChangedEventArgs(propertyName));
                    }
					break;

				case ListChangedType.ItemMoved:
                    if (e.NewIndex > -1 && e.NewIndex < source.Count)
                    {
                        newItems.Add(source[e.NewIndex]);
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItems, e.NewIndex, e.OldIndex);
                    }
                    else
                    {
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    }
					break;

				case ListChangedType.Reset:
					arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
					break;
			}
            var isBindingListInEdit = false;
#if !SILVERLIGHT
            if (!this.IsLegacyDataTable)
                isBindingListInEdit = this.IsEditingItem;
#endif
            if (arg != null && !isBindingListInEdit)
            {
                IsInSourceCollectionChange = true;
                if (this.IsGrouping)
                {
                    var propName = String.Empty;
                    if (e.PropertyDescriptor != null)
                    {
                        propName = e.PropertyDescriptor.Name;
                    }

                    this.IsInSuspend = true;
                    this.UpdateGroupingModel(sender, arg, propName);
                    this.IsInSuspend = false;
                }
                else
                    this.UpdateCollectionView(sender, arg);
                IsInSourceCollectionChange = false;
            }
		}
#endif

		private void UpdateGroupingModel(object sender, NotifyCollectionChangedEventArgs e, string propertyName)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
                    this.GroupList.Add(e.NewItems[0], IsInSourceCollectionChange);
					break;

#if !SILVERLIGHT
				case NotifyCollectionChangedAction.Move:
                    this.GroupList.Remove(e.NewItems[0], IsInSourceCollectionChange);
                    this.GroupList.Add(e.NewItems[0], IsInSourceCollectionChange);
					break;
#endif

				case NotifyCollectionChangedAction.Remove:
					this.UpdateCurrentItem();
					if (e.OldItems.Count > 0)
					{
						this.GroupList.Remove(e.OldItems[0],IsInSourceCollectionChange);
					}
					this.UpdateCurrentPosition();
					break;

				case NotifyCollectionChangedAction.Replace:
						this.GroupList.Remove(e.OldItems[0],IsInSourceCollectionChange);
                        this.GroupList.Add(e.NewItems[0], IsInSourceCollectionChange);
					break;

				case NotifyCollectionChangedAction.Reset:
					/*this.RefreshTopLevelGroup();
					this.RefreshSort();
					this.OnCollectionChanged(e);*/
					this.ForceEndDefer();
					break;
			}
		}

		private void ForceEndDefer()
		{
			this.IsInEndDefer = true;
			this.EndDeferInternal();
			this.IsInEndDefer = false;
		}

		private class ListOrdinalComparer : IComparer<object>
		{
			// Fields
			private IRecordsList _ilFull;
			private int _index;
			private object _item;

			// Methods
			internal ListOrdinalComparer(IRecordsList ilFull, object item, int index)
			{
				this._ilFull = ilFull;
				this._item = item;
				this._index = index;
			}

			public int Compare(object x, object y)
			{
				var o1 = x as RecordEntry;
				var o2 = y as RecordEntry;
				var data1 = o1 != null ? o1.Data : x;
				var data2 = o2 != null ? o2.Data : y;

				int num = object.Equals(data1, this._item) ? this._index : this._ilFull.IndexOfRecord(data1);
				int num2 = object.Equals(data2, this._item) ? this._index : this._ilFull.IndexOfRecord(data2);
				return (num - num2);
			}
		}

		/// <summary>
		/// Updates the collection view by handling the NotifyCollectionChangedEventArgs if there is no grouping.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void UpdateCollectionView(object sender, NotifyCollectionChangedEventArgs e)
		{
			RecordEntry record = null;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (var item in e.NewItems)
						{
							if (this.PassesFilter(item))
							{
								record = this.Records.CreateRecordEntry(item);
								if (!this.ItemPropertiesSet)
								{
									this.SetItemProperties(this.SourceCollection);
								}
								if (!this.isEmpty)
								{
									this.isEmpty = false;
								}
								int? insertIndex = null;
								if (this.SortDescriptions.Count > 0)
								{
									var comparerIndex = GetComparerIndex(item, e.NewStartingIndex);

                                    if (comparerIndex > this.Records.Count)
                                        this.Records.Add(record);
                                    else
                                        this.Records.Insert(comparerIndex, record);
									insertIndex = comparerIndex;
								}
								else
								{
									if (e.NewStartingIndex > -1 && e.NewStartingIndex < this.Records.Count)
									{
										this.Records.Insert(e.NewStartingIndex, record);
										insertIndex = e.NewStartingIndex;
									}
									else
									{   
										insertIndex = this.Records.Count;
										this.Records.Add(record);
									}
								}

								if (insertIndex != null)
								{
									this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, insertIndex.Value));
								}
							}
						}
					 
					}
					break;

#if !SILVERLIGHT
				case NotifyCollectionChangedAction.Move:
					{
						record = this.Records.CreateRecordEntry(e.NewItems[0]);
						//var removeAtIndex = this.Records.IndexOfRecord(e.OldItems[0]);
						this.Records.RemoveAt(e.OldStartingIndex);
						//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, record.Data, e.OldStartingIndex));
						var comparerIndex = GetComparerIndex(e.NewItems[0], e.NewStartingIndex);
						this.Records.Insert(comparerIndex, record);
						//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, record.Data, comparerIndex));
						//*Selection lost while moving the Record in the collection so added the below line and comment the above two line. this is regarding for the issue id 9913*/                   
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, record.Data, e.NewStartingIndex, e.OldStartingIndex));
					}
					break;
#endif
				case NotifyCollectionChangedAction.Remove:
					{
						if (e.OldStartingIndex == this.CurrentPosition)
							this.UpdateCurrentItem();

						if (e.OldItems.Count > 0)
						{
							foreach (var item in e.OldItems)
							{
								int removeAtIndex = -1;
								if (e.OldItems.Count > 0)
								{
									removeAtIndex = this.Records.IndexOfRecord(item);
									if (removeAtIndex > -1)
									{
										this.Records.RemoveAt(removeAtIndex);
                                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, removeAtIndex));
									}
								}
							}
						}
						else
						{
							var removeAtIndex = e.OldStartingIndex;
							this.records.RemoveAt(e.OldStartingIndex);
							this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<object>(), removeAtIndex));
						}

						this.UpdateCurrentPosition();
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					{
						 var index = this.Records.IndexOfRecord(e.OldItems[0]);
                         if (index == -1)
                             index = e.NewStartingIndex;
                         if (e.NewStartingIndex > -1 && index < this.Count)
                         {
                             var hasSort = this.SortDescriptions.Count > 0 ? true : false;
                             if (hasSort && this.SortingOptions == Data.SortingOptions.Default)
                             {
                                 var source = sender as IList;
                                 record = this.Records.GetRecord(e.OldItems[0]);
                                 if (record != null)
                                 {
                                     this.Records.Remove(record);
                                     this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<object>(), e.NewStartingIndex));
                                 }
                                 if (PassesFilter(e.NewItems[0]))
                                 {
                                     var newRecord = this.Records.CreateRecordEntry(e.NewItems[0]);
                                     var comparerIndex = GetComparerIndex(e.NewItems[0], e.NewStartingIndex);
                                     this.Records.Insert(comparerIndex, newRecord);
                                     this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newRecord, comparerIndex));
                                 }
                             }
                             else
                             {
                                 record = this.Records.GetRecord(e.OldItems[0]);
                                 if (record != null)
                                 {
                                     this.Records.Remove(record);
                                     this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<object>(), e.NewStartingIndex));
                                 }
                                 if (PassesFilter(e.NewItems[0]))
                                 {
                                     var newRecord = this.Records.CreateRecordEntry(e.NewItems[0]);
                                     this.Records.Insert(index, newRecord);
                                     this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newRecord, e.NewStartingIndex));
                                 }
                             }
                         }
					}
					break;

				case NotifyCollectionChangedAction.Reset:
#if SILVERLIGHT

					if (!this.ItemPropertiesSet)
					{
						this.SetItemProperties(this.SourceCollection);
					}

					//RiaAfterEdit
					this.canSort = true;
					this.canFilter = true;
#endif
					//this.Records.Clear();
					//this.EnsureInitialized();
					//this.OnCollectionChanged(e);
					this.records.ReomveAll();
					this.ForceEndDefer();
					break;
			}
		}

        internal int InternalBinarySearch(List<object> internalList, int removeatIndex, object value, IComparer<object> comparer)
        {
            if (removeatIndex >= 0)//if the value is already in the list, it will be removed
                internalList.RemoveAt(removeatIndex);
            int num = 0;
            int num2 = (internalList.Count) - 1;
            while (num <= num2)
            {
                int num3 = num + ((num2 - num) >> 1);
                int num4 = comparer.Compare(internalList[num3], value);
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

		private int GetComparerIndex(object record, int startIndex)
		{
			return GetComparerIndex(record, startIndex, string.Empty);
		}

		private int GetComparerIndex(object record, int startIndex, string fieldname)
		{
			var activeComparer = this.GetActiveComparer();
			var comparer = activeComparer != null ? activeComparer : new ListOrdinalComparer(this.records, record, startIndex);

			if (comparer is SortFieldComparer && !string.IsNullOrEmpty(fieldname))
			{
				bool enablesorting = false;
				foreach (var item in (comparer as SortFieldComparer).SortFields)
				{
					if (item.PropertyName == fieldname)
						enablesorting = true;
				}
				if (!enablesorting)
					return startIndex;
			}

            int removeAtIndex = this.Records.IndexOfRecord(record);
            //getting the index of record and if record was not present in the Records, it will return -1
            
            var comparerIndex = this.InternalBinarySearch(this.Records.ToArray().ToList<object>(), removeAtIndex, record, comparer);
            //var comparerIndex = Array.BinarySearch<object>(this.Records.ToArray(), record, comparer);

            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }

			return comparerIndex;
		}

        //Code to get the Comparerindex when Grouping.      
        private int GetComparerIndexInsideGroup(object record, int startIndex, string fieldname, Group group)
        {

            IRecordsEntryList groupRecords = group.Records as IRecordsEntryList;

            var activeComparer = this.GetActiveComparer();            
            var comparer = activeComparer != null ? activeComparer : new ListOrdinalComparer(groupRecords as IRecordsList, record, startIndex);

            if (comparer is SortFieldComparer && !string.IsNullOrEmpty(fieldname))
            {
                bool enablesorting = false;
                foreach (var item in (comparer as SortFieldComparer).SortFields)
                {
                    if (item.PropertyName == fieldname)
                        enablesorting = true;
                }
                if (!enablesorting)
                    return startIndex;
            }
            
            var comparerIndex = this.InternalBinarySearch(groupRecords.ToArray().ToList<object>(), startIndex, record, comparer);

            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }

            return comparerIndex;
        }

        protected bool IsInSourceCollectionChange = false;
		void listeventSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.IsInSuspend)
			{
				return;
			}
            IsInSourceCollectionChange = true;
			if (this.IsGrouping)
			{
				this.IsInSuspend = true;
				this.UpdateGroupingModel(sender, e, String.Empty);
				this.IsInSuspend = false;
			}
			else
			{
				this.UpdateCollectionView(sender, e);
			}
            IsInSourceCollectionChange = false;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (var newItem in e.NewItems)
					{
						this.AddNotifyListener(newItem);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var oldItem in e.OldItems)
					{
						this.RemoveNotifyListener(oldItem);
					}
					break;
			}

			//if (e.Action != NotifyCollectionChangedAction.Replace)
			this.UpdateTableSummary();
		}

		protected bool ItemPropertiesSet
		{
			get;
			private set;
		}

        protected bool IsItemPropertiesTypeSet = false;

		private void SetItemProperties(IEnumerable dataSource)
		{
			if (dataSource == null)
			{
#if !SILVERLIGHT
				this.itemProperties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
#else
				this.itemProperties = PropertyInfoCollection.Empty;
#endif
			    return;
			}
			else
			{
				var list = dataSource as IEnumerable;
#if !SILVERLIGHT
				if (dataSource is IListSource)
					list = ((IListSource)dataSource).GetList();

				if (dataSource is ITypedList)
				{
					this.isITypedListSource = true;
					this.itemProperties = ((ITypedList)(dataSource)).GetItemProperties(null);
					this.ItemPropertiesSet = true;
				}
				else if (list is ITypedList)
				{
					this.isITypedListSource = true;
					this.itemProperties = ((ITypedList)(list)).GetItemProperties(null);
					this.ItemPropertiesSet = true;
				}
				else if (list != null)
                {
#endif
                    var enumerator = list.GetEnumerator();
                    if (SourceType != null && !(SourceType == typeof(Object)))
                    {
#if !SILVERLIGHT
                        this.itemProperties = TypeDescriptor.GetProperties(SourceType);
#else
						this.itemProperties = new PropertyInfoCollection(SourceType);
#endif
                        this.ItemPropertiesSet = true;
                    }
					else if (enumerator.MoveNext())
					{
                        var castType = EnumerableExtensions.CastToSourceType(list);
#if !SILVERLIGHT
                        if(castType != null)
                            this.itemProperties = TypeDescriptor.GetProperties(castType);
                        else
                            this.itemProperties = TypeDescriptor.GetProperties(enumerator.Current);
#else
						this.itemProperties = new PropertyInfoCollection(castType ?? enumerator.Current.GetType());
#endif
                        this.ItemPropertiesSet = true;
					}
					else
					{
						var prop = list.GetType().GetProperty("Item");
						if (prop != null)
						{
                            ItemPropertiesSet = !(prop.PropertyType == typeof(Object));
#if !SILVERLIGHT
                            this.itemProperties = TypeDescriptor.GetProperties(prop.PropertyType);
#else
                            this.itemProperties = new PropertyInfoCollection(prop.PropertyType);
                            //following line is comment for GridRiaSilverlight data are no populated issue SD16184.
                            //this.ItemPropertiesSet = true;
#endif
						}
					}

				}
				this.propertyAccessProvider = this.CreateItemPropertiesProvider();
#if !SILVERLIGHT
			}
#endif
			if (!IsItemPropertiesTypeSet)
			{
#if SyncfusionFramework4_0
				if (!this.IsDynamicBound)
				{
#endif
                    this.SourceType = this.SourceCollection.GetElementType(ref isEmpty);
#if SyncfusionFramework4_0
				}
				else
				{
					this.SourceType = typeof(object);
				}
#endif
			}

		    if (ItemPropertiesSet)
		        this.OnPropertyChanged("ItemProperties");
		}

		/// <summary>
		/// Creates the item properties provider. Override this method to have return custom <see cref="IPropertyAccessProvider"/> and have customizations.
		/// </summary>
		/// <returns></returns>
		protected virtual IPropertyAccessProvider CreateItemPropertiesProvider()
		{
			IPropertyAccessProvider provider = null;
#if SyncfusionFramework4_0
			if (!this.IsDynamicBound)
			{
#endif
#if !SILVERLIGHT
			if (!this.IsXMLBound)
			{
#endif
				if (!this.IsInterfaceBound)
				{
					provider = new ItemPropertiesProvider(this);
				}
				else
				{
					provider = new InterfacePropertiesProvider(this);
				}
#if !SILVERLIGHT
			}
			else
			{
				provider = new XMLAttributesProvider(this);
			}
#endif
#if SyncfusionFramework4_0
			}
			else
			{
				provider = new DynamicPropertiesProvider(this);
			}
#endif
			return provider;

			//return new ItemPropertiesProvider(this);

		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="CollectionViewAdv"/> is reclaimed by garbage collection.
		/// </summary>
		~CollectionViewAdv()
		{
		   // this.Dispose(false); //Since the GRid can properly disposed by calling GridDataControl.Dispose(); this is not requiered. This will throw null exception.
		}

#if !SILVERLIGHT

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
#endif

#if !SILVERLIGHT
		internal bool validatedICustomTypeDescriptor = false;
		internal bool hasICustomTypeDescriptor = false;
		internal bool HasICustomTypeDescriptor
		{
			get
			{
				if (!this.validatedICustomTypeDescriptor)
				{
					if (this.Records.Count > 0)
					{
						this.hasICustomTypeDescriptor = ((RecordEntry)this.Records[0]).Data as ICustomTypeDescriptor != null ? true : false;
						this.validatedICustomTypeDescriptor = true;
					}
				}

				return this.hasICustomTypeDescriptor;
			}
		}

		bool hasCustomTypeDescriptionProvider = false;
		bool validatedCustomTypeDescriptionProvider = false;
		internal bool HasCustomTypeDescriptionProvider
		{
			get
			{
				if (!this.validatedCustomTypeDescriptionProvider)
				{
					IEnumerable list = this.SourceCollection as IEnumerable;
					IEnumerator enumerator = list.GetEnumerator();
					if (enumerator == null)
					{
						return false;
					}
					AttributeCollection atts = AttributeCollection.Empty;
					if (enumerator.MoveNext())
					{
						atts = TypeDescriptor.GetAttributes(enumerator.Current);
						foreach (Attribute at in atts)
						{
                            if (TypeDescriptor.GetProperties(at.TypeId).Find("Name", true) != null)
                            {
                                var value = TypeDescriptor.GetProperties(at.TypeId)["Name"].GetValue(at.TypeId) as string;

                                if (value.Equals("TypeDescriptionProviderAttribute"))
                                {
                                    this.hasCustomTypeDescriptionProvider = true;
                                    break;
                                }
                            }
						}

						this.validatedCustomTypeDescriptionProvider = true;
					}
				}

				return this.hasCustomTypeDescriptionProvider;
			}
		}


		private bool isITypedListSource = false;
		private Func<string, object, object> typedListFunc = null;
		internal Func<string, object, object> GetITypedListFunc(string propertyName)
		{
			if (this.isITypedListSource)
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
							{
                                var val = pd.GetValue(record);
                                if (val is DBNull)
                                    return null;
                                return val;
							}
						}

						return null;
					};
				}

				return this.typedListFunc;
			}

			return null;
		}
       
        private Func<string, object, object> ITypedListTypeFunc = null;
        internal Func<string, object, object> GetITypedListTypeFunc(string propertyName)
        {
            if (this.isITypedListSource)
            {
                var typedList = this.SourceCollection as ITypedList;
                if (this.ITypedListTypeFunc == null)
                {
                    this.ITypedListTypeFunc = (columnName, record) =>
                    {
                        var pdc = typedList.GetItemProperties(null);
                        if (pdc != null)
                        {
                            var pd = pdc.GetPropertyDescriptor(columnName);
                            if (pd != null)
                            {
                                return pd.PropertyType;
                            }
                        }

                        return null;
                    };
                }

                return this.ITypedListTypeFunc;
            }

            return null;
        }
#endif

		#region GroupConverter
		//private IValueConverter groupConverter;
		internal Func<string, object, object> GetGroupConverterFunc(string propertyName)
		{
			var pgd = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.PropertyName == propertyName);
			if (pgd != null && pgd.Converter != null)
			{
				Func<string, object, object> groupConverterFunc = (columnName, record) =>
				{
					return pgd.Converter.Convert(record, record.GetType(), null, CultureInfo.CurrentCulture);
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

		#endregion
#if !SILVERLIGHT
		internal Expression<Func<string, object, object>> GetITypedListExpressionFunc(string propertyName)
		{
			if (this.isITypedListSource)
			{
				var typedList = this.SourceCollection as ITypedList;
				var recordFunc = this.GetITypedListFunc(propertyName);
				if (recordFunc != null)
				{
					Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
					return expFunc;
				}
			}

			return null;
		}

        internal Expression<Func<string, object, object>> GetITypedListTypeExpressionFunc(string propertyName)
        {
            if (this.isITypedListSource)
            {
                var typedList = this.SourceCollection as ITypedList;
                var recordFunc = this.GetITypedListTypeFunc(propertyName);
                if (recordFunc != null)
                {
                    Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
                    return expFunc;
                }
            }
            return null;
        }

		private Func<string, object, object> iCustomTypeDescriptionProviderFunc = null;
		private Func<string, object, object> GetCustomTypeDescriptionProviderFunc(string propertyName)
		{
			if (this.HasCustomTypeDescriptionProvider)
			{
				IEnumerable list = this.SourceCollection as IEnumerable;
				IEnumerator enumerator = list.GetEnumerator();
				if (enumerator == null)
				{
					return null;
				}

				PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
				if (enumerator.MoveNext())
				{
					pdc = TypeDescriptor.GetProperties(enumerator.Current);
					if (this.iCustomTypeDescriptionProviderFunc == null)
					{
						this.iCustomTypeDescriptionProviderFunc = (columnName, record) =>
						{

							if (pdc != null)
							{
								var pd = pdc.GetPropertyDescriptor(columnName);
								if (pd != null)
								{
									return pd.GetValue(record);
								}
							}

							return null;
						};
					}

					return this.iCustomTypeDescriptionProviderFunc;
				}
			}

			return null;
		}

        private Func<string, object, object> iCustomTypeDescriptionProviderTypeFunc = null;
        private Func<string, object, object> GetCustomTypeDescriptionProviderTypeFunc(string propertyName)
        {
            if (this.HasCustomTypeDescriptionProvider)
            {
                IEnumerable list = this.SourceCollection as IEnumerable;
                IEnumerator enumerator = list.GetEnumerator();
                if (enumerator == null)
                {
                    return null;
                }

                PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                if (enumerator.MoveNext())
                {
                    pdc = TypeDescriptor.GetProperties(enumerator.Current);
                    if (this.iCustomTypeDescriptionProviderTypeFunc == null)
                    {
                        this.iCustomTypeDescriptionProviderTypeFunc = (columnName, record) =>
                        {

                            if (pdc != null)
                            {
                                var pd = pdc.GetPropertyDescriptor(columnName);
                                if (pd != null)
                                {
                                    return pd.PropertyType ;
                                }
                            }

                            return null;
                        };
                    }
                    return this.iCustomTypeDescriptionProviderTypeFunc;
                }
            }

            return null;
        }

		internal Expression<Func<string, object, object>> GetCustomTypeDescriptionProviderExpressionFunc(string propertyName)
		{
			if (this.HasCustomTypeDescriptionProvider)
			{
				var recordFunc = this.GetCustomTypeDescriptionProviderFunc(propertyName);
				Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
				return expFunc;
			}

			return null;
		}

        internal Expression<Func<string, object, object>> GetCustomTypeDescriptionProviderTypeExpressionFunc(string propertyName)
        {
            if (this.HasCustomTypeDescriptionProvider)
            {
                var recordFunc = this.GetCustomTypeDescriptionProviderTypeFunc(propertyName);
                Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
                return expFunc;
            }
            return null;
        }

		private Func<string, object, object> iCustomTypeDescriptorFunc = null;
		internal virtual Func<string, object, object> GetCustomTypeDescriptorFunc(string propertyName)
		{
			if (this.HasICustomTypeDescriptor)
			{
				if (this.iCustomTypeDescriptorFunc == null)
				{
					this.iCustomTypeDescriptorFunc = (columnName, record) =>
					{
						var typeDescriptor = record as ICustomTypeDescriptor;
						return this.propertyAccessProvider.GetValue(record, columnName);
						//var pdc = typeDescriptor.GetProperties();
						//if (pdc != null)
						//{
						//    var pd = pdc.GetPropertyDescriptor(columnName);
						//    if (pd != null)
						//    {
						//        return pd.GetValue(record);
						//    }
						//}
						//return null;
					};
				}

				return this.iCustomTypeDescriptorFunc;
			}

			return null;
		}

        private Func<string, object, object> iCustomTypeDescriptorTypeFunc = null;
        internal virtual Func<string, object, object> GetCustomTypeDescriptorTypeFunc(string propertyName)
        {
            if (this.HasICustomTypeDescriptor)
            {
                if (this.iCustomTypeDescriptorTypeFunc == null)
                {
                    this.iCustomTypeDescriptorTypeFunc = (columnName, record) =>
                    {
                        var typeDescriptor = record as ICustomTypeDescriptor;
                        var pdc = typeDescriptor.GetProperties();
                        if (pdc != null)
                        {
                            var pd = pdc.GetPropertyDescriptor(columnName);
                            if (pd != null)
                            {
                                return pd.PropertyType;
                            }
                        }
                        return null;
                    };
                }
                return this.iCustomTypeDescriptorTypeFunc;
            }
            return null;
        }

		internal virtual Expression<Func<string, object, object>> GetCustomTypeDescriptorExpressionFunc(string propertyName)
		{
			if (this.HasICustomTypeDescriptor)
			{
				var recordFunc = this.GetCustomTypeDescriptorFunc(propertyName);
				Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
				return expFunc;
			}

			return null;
		}

        internal virtual Expression<Func<string, object, object>> GetCustomTypeDescriptorTypeExpressionFunc(string propertyName)
        {
            if (this.HasICustomTypeDescriptor)
            {
                var recordFunc = this.GetCustomTypeDescriptorTypeFunc(propertyName);
                Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
                return expFunc;
            }

            return null;
        }
#endif


#if SILVERLIGHT
		private PropertyInfoCollection itemProperties;
		/// <summary>
		/// Gets the item properties.
		/// </summary>
		/// <value>The item properties.</value>
		public PropertyInfoCollection ItemProperties
		{
			get
			{
				return this.itemProperties;
			}
		}
#endif

		private int FindIndex(object newitem, SortDescription sd)
		{
			int index = -1;
			var isAscending = sd.Direction == ListSortDirection.Ascending;
#if !SILVERLIGHT
			if (!this.IsLegacyDataTable)
#endif
			{
				//var pd = this.ItemProperties[sd.PropertyName];
				var pd = this.ItemProperties.GetPropertyDescriptor(sd.PropertyName);
#if !SILVERLIGHT
				var newvalue = pd.GetValue(newitem);
#else
				var newvalue = pd.GetValue(newitem, null);
#endif
				if (newvalue != null)
				{
					if (this.Records.Count == 0)
					{
						index = 0;
					}
					else
					{
						foreach (var item in this.Records)
						{
#if !SILVERLIGHT
							var itemvalue = pd.GetValue(item.Data);
#else
							var itemvalue = pd.GetValue(item.Data, null);
#endif
							if (itemvalue != null)
							{

								int c = ((IComparable)itemvalue).CompareTo((IComparable)newvalue);
								index++;
								if (this.Check(c, isAscending))
								{
									break;
								}
							}
						}
					}
				}
				else
				{
					index = 0;
				}
			}
#if !SILVERLIGHT
			else
			{
				index = FindIndexForDataTable(newitem as DataRowView, isAscending, sd.PropertyName);
			}
#endif

			return index;
		}

#if!SILVERLIGHT

		private int FindIndexForDataTable(DataRowView newRow, bool isAscending, string propertyName)
		{
			int index = -1;
			var newValue = newRow[propertyName];
			if (newValue != null)
			{
				foreach (var item in this.Records)
				{
					var rowView = item.Data as DataRowView;
					var itemValue = rowView[propertyName];
					if (!(itemValue is DBNull) && itemValue != null)
					{
						int c = ((IComparable)itemValue).CompareTo((IComparable)newValue);
						index++;
						if (this.Check(c, isAscending))
						{
							break;
						}
					}
				}
			}
			else
			{
				index = 0;
			}
			return index;
		}
#endif
		private bool Check(int c, bool isAscending)
		{
			if (isAscending)
			{
				if (c <= 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				if (c >= 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

		}

		private IList GetSourceListCollection()
		{
			IList list = null;
			if ((this.SourceCollection as IList) != null)
			{
				list = this.SourceCollection as IList;
			}
#if !SILVERLIGHT
			else if ((this.SourceCollection as IListSource) != null)
			{
				var listSource = this.SourceCollection as IListSource;
				list = listSource.GetList();
			}
#endif

			return list;
		}

		/*public IList SourceList
		{
			get
			{
				return this.SourceCollection as IList;
			}
		}*/

		/// <summary>
		/// Adds the listener to an external ICollectionView. It will add set the ExternalCollectionView Propety n Wires the CurrentChanged event
		/// </summary>
		/// <param name="externalcollectionview">The externalcollectionview.</param>
		public void AddListener(ICollectionView externalcollectionview)
		{
			this.ExternalCollectionView = externalcollectionview;
			this.ExternalCollectionView.CurrentChanged += this.ExternalCollectionView_CurrentChanged;
		}

		// in this handler MoveCurrentTo function will reset the currentitem 
		// and eventually raises the CurrentChanged event of this CollectionViewAdv 
		void ExternalCollectionView_CurrentChanged(object sender, EventArgs e)
		{
            if (this.deferRefreshCount > -1 || this.IsInSuspend || IsInEndDefer)
			{
				return;
			}

			if (this.ExternalCollectionView.CurrentItem != null && this.Records != null && this.ExternalCollectionView.CurrentItem != null)
			{
				var record = this.Records.GetRecord(this.ExternalCollectionView.CurrentItem);

				if (record != null)
				{
					if (this.IsGrouping)
					{
						this.ExpandGroups(this.TopLevelGroup, record.Data);
					}
					this.MoveCurrentTo(record.Data);
				}
			}
		}

		private void ExpandGroups(Group group, object item)
		{
			var needsRefresh = false;
		    if (!group.IsBottomLevel)
		    {
                var pgd = ((PropertyGroupDescription)this.GroupDescriptions[group.Level]); 
		        object itemKey = null;
#if !SILVERLIGHT
                if (!this.IsLegacyDataTable)
                {
                    itemKey = propertyAccessProvider.GetValue(item, pgd.PropertyName);
                }
                else
                {
                    var rowView = item as DataRowView;
                    itemKey = rowView[pgd.PropertyName];
                }
#else
                itemKey = propertyAccessProvider.GetValue(item, pgd.PropertyName);
#endif
		        foreach (var lowergroup in group.Groups)
		        {
                    var c = -1;
                    var converter = pgd != null ? pgd.Converter : null;
		            var key = converter == null
		                          ? itemKey
                                  : converter.Convert(item, lowergroup.Key != null ? lowergroup.Key.GetType() : typeof(object), null,
		                                              this.Culture);

                    if ((key == null && lowergroup.Key == null) || (key is DBNull && lowergroup.Key is DBNull) ||
                        (!(key is IComparable) && !(lowergroup.Key is IComparable)))
		            {
		                c = 0;
		            }
                    else if (key == null || lowergroup.Key == null || key is DBNull || lowergroup.Key is DBNull ||
                             !(key is IComparable) || !(lowergroup.Key is IComparable))
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

		/// <summary>
		/// Gets or sets the external collection view.
		/// </summary>
		/// <value>The external collection view.</value>
		public ICollectionView ExternalCollectionView
		{
			get;
			set;
		}

		/// <summary>
		/// Call this method in the derived CollectionView once the constructor is called.
		/// </summary>
		protected void EnsureSourceList()
		{
			this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
			this.EnsureItemConstructor();
			this.WireEvents();
			//this.MoveCurrentToFirst();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
                if (this.ExternalCollectionView != null)
                    this.ExternalCollectionView.CurrentChanged -= this.ExternalCollectionView_CurrentChanged;
				this.UnwireNotifyPropertyChangedForUnderlyingSource();
				this.UnwireEvents();
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
				if (this.topLevelGroup != null)
				{
					this.topLevelGroup.Dispose();
					this.topLevelGroup = null;
				}
				this.propertyAccessProvider = null;
				this.PropertyChanged = null;
				if (groups != null)
				{
                    this.groups.CollectionChanged -= this.OnGroupsCollectionChanged;
					if (this.groups.Count > 0)
						this.groups.Clear();
					this.groups = null;
				}
				if (summaryRows != null)
				{
					this.summaryRows.Clear();
					this.summaryRows = null;
				}
				if (this.filters != null)
				{
					this.filters.Clear();
					this.filters = null;
				}
				if (this.tableSummaryRows != null)
				{
					this.tableSummaryRows.Clear();
					this.tableSummaryRows = null;
				}
				if (this.relations != null)
				{
					this.relations.Clear();
					this.relations = null;
				}
				if (sortComparers != null)
				{
					this.sortComparers.Clear();
					this.sortComparers = null;
				}
#if !SILVERLIGHT
				if (this._changedInfo != null)
				{
					this._changedInfo.Clear();
					this._changedInfo = null;
				}
				if (this._changedSummaryInfo != null)
				{
					this._changedSummaryInfo.Clear();
					this._changedSummaryInfo = null;
				}
				if (this._changedCaptionSummaryInfo != null)
				{
					this._changedCaptionSummaryInfo.Clear();
					this._changedCaptionSummaryInfo = null;
				}
				if (this.DispatchOwner != null)
				{
					DispatchOwner = null;
				}
#endif
				if (this.captionSummaryRow != null)
				{
					this.captionSummaryRow.SummaryColumns.Clear();
					this.captionSummaryRow = null;
				}
				if (this.source != null)
				{
					this.source = null;
				}
			}
		}
		#endregion

		private IRecordsList records;

		/// <summary>
		/// Gets the records list structure.
		/// </summary>
		/// <value>The records.</value>
		public IRecordsList Records
		{
			get
			{
				if (this.flags == CollectionViewFlags.NeedsRefresh || this.records == null)
				{
					this.EnsureInitialized();
				}

				return this.records;
			}
		}
#if !SILVERLIGHT
        private IList ComplexProperties
        {
            get;
            set;
        }
#endif
        /// <summary>
		/// Ensures the records are initialized properly.
		/// </summary>
		protected virtual void EnsureInitialized()
		{
			this.SetFlag(CollectionViewFlags.NeedsRefresh, false);

			this.UnwireEvents();
            if (IsInEndDefer)
                this.UnwireNotifyPropertyChangedForUnderlyingSource();
			// use a temp list when we are re-creating records, this helps when we have objects sorted with unbound expressions or custom expressions
			var tempRecordsList = this.CreateRecords();

			if (this.records != null)
			{
				//if (this.records != null)
				//    if ((this.records as EnumerableRecordsWrapper) != null)
				//        (this.records as EnumerableRecordsWrapper).RemoveNotifyListener();
				this.records.CollectionChanged -= records_CollectionChanged;
				this.records.Dispose();
				this.records = null;
			}

			this.records = tempRecordsList; //this.CreateRecords();
			this.records.CollectionChanged += records_CollectionChanged;
#if !SILVERLIGHT
            if (ComplexProperties != null)
                ComplexProperties.Clear();
            else
                ComplexProperties = new List<string>();

            var mappingNames = this.MappingNames;

            if (mappingNames != null)
            {
                foreach (var mappingname in this.MappingNames)
                {
                    if (mappingname != null && mappingname.Contains('.') && !mappingname.Contains('['))
                        ComplexProperties.Add(mappingname);
                }
            }
#endif
			this.UpdateTableSummary();
            this.WireNotifyPropertyChangedForUnderlyingSource(IsInEndDefer);
			this.WireEvents();
		}

        protected void SetSource(IEnumerable source)
        {
            this.source = source;
        }

		public event NotifyCollectionChangedEventHandler RecordsListCollectionChanged;

		void records_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var handler = this.RecordsListCollectionChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private int _eventTicks = 0;

		internal void UnwireEvents()
		{
			if (this._eventTicks > 0)
			{
				this._eventTicks -= 1;
				var sortNotifyCollectionChanged = this.SortDescriptions as INotifyCollectionChanged;
				sortNotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnSortChanged);

#if !SILVERLIGHT
				if (source is IListEvents)
				{
					var listeventSource = source as IListEvents;
					listeventSource.CollectionChanged -= this.listeventSource_CollectionChanged;
				}
#endif
				if (source is INotifyCollectionChanged)
				{
					var notifyCollectionchanged = source as INotifyCollectionChanged;
					notifyCollectionchanged.CollectionChanged -= this.notifyCollectionchanged_CollectionChanged;
				}
#if !SILVERLIGHT
				else if (source is IBindingList)
				{
					var listChanged = source as IBindingList;
					listChanged.ListChanged -= listChanged_ListChanged;
				}
#endif
                if (this.TableSummaryRows != null)
                    this.TableSummaryRows.CollectionChanged -= this.TableSummaryRows_CollectionChanged;
			}
		}
		

		private void WireEvents()
		{
			if (this._eventTicks == 0)
			{
				this._eventTicks += 1;
				var sortNotifyCollectionChanged = this.SortDescriptions as INotifyCollectionChanged;
				sortNotifyCollectionChanged.CollectionChanged += this.OnSortChanged;

#if !SILVERLIGHT
				if (source is IListEvents)
				{
					var listeventSource = source as IListEvents;
					listeventSource.CollectionChanged += this.listeventSource_CollectionChanged;
				}
#endif
				if (source is INotifyCollectionChanged)
				{
					var notifyCollectionchanged = source as INotifyCollectionChanged;
					notifyCollectionchanged.CollectionChanged += this.notifyCollectionchanged_CollectionChanged;
				}
#if !SILVERLIGHT
				else if (source is IBindingList)
				{
					var listChanged = source as IBindingList;
					listChanged.ListChanged += listChanged_ListChanged;
				}
#endif
                if (this.TableSummaryRows != null)
                {
                    this.TableSummaryRows.CollectionChanged += this.TableSummaryRows_CollectionChanged;
                }
			}
		}

		public event PropertyChangedEventHandler RecordPropertyChanged;

		protected virtual void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var handler = this.RecordPropertyChanged;
			if (handler != null)
			{
				handler(sender, e);
			}
		}

        private bool _notifyComplexPropertyChanges = true;
        internal bool NotifyComplexPropertyChanges
        {
            get { return _notifyComplexPropertyChanges; }
            set { _notifyComplexPropertyChanges = value; }
        }

		/// <summary>
		/// Receives events from the centralized event manager.
		/// </summary>
		/// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
		/// <param name="sender">Object that originated the event.</param>
		/// <param name="e">Event data.</param>
		/// <returns>
		/// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
		/// </returns>
#if !SILVERLIGHT
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
#else
		void IPropertyChangedEventHandler.OnPropertyChanged(object sender, EventArgs e)
#endif
		{
			var result = true;
#if !SILVERLIGHT
			if (managerType == typeof(PropertyChangingEventManager))
				NotifyPropertyChangingHandler(sender, e, ref result);
			else if (managerType == typeof(PropertyChangedEventManager))
#endif
				NotifyPropertyChangedHandler(sender, e, ref result);
#if !SILVERLIGHT
			return result;
#endif
		}

#if !SILVERLIGHT
		private Dictionary<string, object> _changedInfo = new Dictionary<string, object>();
		private Dictionary<string, object> _changedSummaryInfo = new Dictionary<string, object>();
		private Dictionary<string, object> _changedCaptionSummaryInfo = new Dictionary<string, object>();
		private void NotifyPropertyChangingHandler(object sender, EventArgs e, ref bool result)
		{
#if !SILVERLIGHT
            if (DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    DispatchOwner.Invoke(new PropertyChangingEventHandler(OnPropertyChanging), sender, e);
                    return;
                }
            }
#endif
			var propertyChangingArgs = e as PropertyChangingEventArgs;
            if (propertyChangingArgs != null && !string.IsNullOrEmpty(propertyChangingArgs.PropertyName))
            {
                var oldValue = propertyAccessProvider.GetValue(sender, propertyChangingArgs.PropertyName);

                if (!_changedInfo.ContainsKey(propertyChangingArgs.PropertyName))
                    _changedInfo.Add(propertyChangingArgs.PropertyName, oldValue);
                if (!_changedSummaryInfo.ContainsKey(propertyChangingArgs.PropertyName))
                    _changedSummaryInfo.Add(propertyChangingArgs.PropertyName, oldValue);
                if (!_changedCaptionSummaryInfo.ContainsKey(propertyChangingArgs.PropertyName))
                    _changedCaptionSummaryInfo.Add(propertyChangingArgs.PropertyName, oldValue);
            }
		}

        void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            var result = true;
            NotifyPropertyChangingHandler(sender, e, ref result);
        }

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			 var result = true;
			 NotifyPropertyChangedHandler(sender, e, ref result);
		}
        private void OnPropertyChanged(object sender, EventArgs e)
        {
            var result = true;
            NotifyPropertyChangedHandler(sender, e, ref result);
        }

	    private void OnComplexPropertyChanged(object sender, PropertyChangedEventArgs e)
	    {
	        var result = true;
            if (this.GetMappingNames() == null)
	            return;

	        foreach (var mappingname in this.GetMappingNames())
	        {
	            if (mappingname.Contains('.') && !mappingname.Contains('['))
	            {
	                var propertyNameList = mappingname.Split('.');
	                if (propertyNameList.Contains(e.PropertyName))
	                {
	                    var pd = this.GetItemProperties();
	                    foreach (var item in this.Records)
	                    {
	                        var kvp = pd.GetPropertyDescriptor(item.Data, mappingname);
	                        if (kvp.Value != null && kvp.Value.Equals(sender))
	                        {
	                            NotifyPropertyChangedHandler(item.Data, e, ref result);
	                        }
	                    }
	                    break;
	                }
	            }
	        }
	    }
#endif

        private void NotifyPropertyChangedHandler(object sender, EventArgs e, ref bool result)
		{
			IsInPropertyChange = true;
#if !SILVERLIGHT
			if (DispatchOwner != null)
			{
				if (DispatchOwner.Thread != Thread.CurrentThread)
				{
					DispatchOwner.Invoke(new PropertyChangedEventHandler(OnPropertyChanged), sender, e);
					return;
				}
			}
#endif
            var propertyChangedArgs = e as PropertyChangedEventArgs;

			var hasSort = this.SortDescriptions != null && this.sortDescriptions.Count > 0 && this.sortDescriptions.FirstOrDefault(s => propertyChangedArgs != null && s.PropertyName == propertyChangedArgs.PropertyName) != default(SortDescription);
			var hasGroup = this.GroupDescriptions != null && this.GroupDescriptions.Count > 0 && this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => propertyChangedArgs != null && g.PropertyName == propertyChangedArgs.PropertyName) != null;
			// specifies if the group model can remove / add in sequence for any change in data.
            var canAllowSort = true;
#if !SILVERLIGHT
            if (!this.IsLegacyDataTable)
                canAllowSort = ((this.sortingOptions & SortingOptions.DisableSortingOnPropertyChange) == 0);
#endif
            var canRemoveAndAddWhenGrouped = (hasSort || hasGroup) && canAllowSort;
            var canrefreshtablesummary = true;
            var forcerefreshtablesummary = false;
            var canrefreshsummary = true;
            var forcerefreshsummary = false;
#if !SILVERLIGHT
            if (!sender.Equals(this.CurrentEditItem) && !this.IsLegacyDataTable)
#else
			if (!this.IsEditingItem)
#endif
            {
                if (!this.IsGrouping)
                {
                    var refreshsort = true;
                    if (this.CanFilter)
                    {
                        var passesFilter = this.PassesFilter(sender);
                        if (!passesFilter)
                        {
                            var removedList = new List<object> { sender };
                            var removeAtIndex = this.Records.IndexOfRecord(sender);
                            if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                            {
                                var recordEntry = this.Records[removeAtIndex];
                                this.Records.RemoveAt(removeAtIndex);
                                forcerefreshtablesummary = true;
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedList, removeAtIndex));

                            }
                            else
                                canrefreshtablesummary = false;
                            refreshsort = false;
                        }
                        else
                        {
                            
                            var record = this.Contains(sender) == true ? null : this.CreateRecordEntry(sender);
                            var recordIndex = this.Records.IndexOfRecord(sender);
                            if (record != null && (recordIndex <= -1 || recordIndex >= this.Records.Count))
                            {
                                if (!hasSort)
                                {
                                    this.Records.Add(record);
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, record.Data, this.Records.Count - 1));
                                }
                                forcerefreshtablesummary = true;
                            }
                        }
                    }
                    if (refreshsort && hasSort && canAllowSort)
                    {
                        var removedList = new List<object> {sender};
                        bool move = false;
                        var item = sender;
                        var removeAtIndex = this.Records.IndexOfRecord(sender);
                        RecordEntry recordEntry = null;
                        bool expandedState = true;
                        if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                        {
                            recordEntry = this.Records[removeAtIndex];
                            expandedState = recordEntry.IsExpanded;
                            this.Records.RemoveAt(removeAtIndex);
                            move = false;
                            if (this.CurrentItem == sender)
                            {
                                this.currentItem = null;
                                move = true;
                            }
                            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedList, removeAtIndex));
                        }
                        var record = this.Records.CreateRecordEntry(item);
                        var comparerIndex = GetComparerIndex(item, 0);
                        if (comparerIndex > -1 && comparerIndex < this.Records.Count)
                        {
                            this.Records.Insert(comparerIndex, record);
                        }
                        else
                        {
                            this.Records.Add(record);
                        }
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, record.Data, comparerIndex));
                        var currentRecordAfterEdit = this.Records[comparerIndex];
                        currentRecordAfterEdit.IsExpanded = expandedState;

                        if (move)
                            this.MoveCurrentTo(item);
                    }
                }
                else
                {
                    var item = sender;
                    var passesFilter = true;

                    if (this.CanFilter)
                        passesFilter = this.PassesFilter(sender);
                    var removedList = new List<object> { sender };
                    var removeAtIndex = this.Records.IndexOfRecord(sender);
                    if (removeAtIndex > -1 && removeAtIndex < this.Records.Count)
                    {
                        if (hasSort || hasGroup || !passesFilter)
                        {
                            if (canRemoveAndAddWhenGrouped)
                            {
#if !SILVERLIGHT
                                this.UpdateGroupingModel(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedList, -1), propertyChangedArgs.PropertyName);
#else
                                 this.UpdateGroupingModel(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedList[0], -1), propertyChangedArgs.PropertyName);
#endif

                                if (!passesFilter)
                                {
                                    forcerefreshtablesummary = true;
                                    forcerefreshsummary = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        canrefreshtablesummary = passesFilter;
                        forcerefreshtablesummary = passesFilter;
                    }
                    if (hasGroup && passesFilter && canRemoveAndAddWhenGrouped)
                    {
                        this.UpdateGroupingModel(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, -1), propertyChangedArgs.PropertyName);
                        canrefreshsummary = false;
                    }
                    else if (hasSort && passesFilter && canRemoveAndAddWhenGrouped)
                    {
                        this.UpdateGroupingModel(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, -1), propertyChangedArgs.PropertyName);
                        canrefreshsummary = false;
                    }
                    else if (passesFilter)
                    {
                        if (removeAtIndex <= -1 || removeAtIndex >= this.Records.Count)
                        {
                            this.UpdateGroupingModel(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, -1), propertyChangedArgs.PropertyName);
                            canrefreshsummary = false;
                        }
                        else
                            canrefreshsummary = true;
                    }
                    else
                    {
                        if (hasGroup)
                            this.TopLevelGroup.ResetGroup(item, propertyChangedArgs.PropertyName);
                    }   
                }
			}
            else
            {
                result = true;
            }
            if (this.IsGrouping)
            {
                if (canrefreshsummary && this.Records != null && this.Records.Count > 0 && this.GroupDescriptions.Count > 0 && this.SummaryRows != null)
                {

                    var isSummaryRowAffected = (from row in this.SummaryRows
                                                from col in row.SummaryColumns
                                                select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;

                    var isCaptionSummaryAffected = captionSummaryRow != null && (from col in captionSummaryRow.SummaryColumns select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;
                    if (isSummaryRowAffected || isCaptionSummaryAffected || forcerefreshsummary)
                    {
                        var recordIndex = this.Records.IndexOfRecord(sender);
                        if (recordIndex > -1 && recordIndex < this.Records.Count)
                        {
                            var recordEntry = this.Records[recordIndex];
                            var parentGroup = recordEntry.Parent as Group;
                            if (parentGroup != null)
                            {
#if !SILVERLIGHT
                                if (isSummaryRowAffected || forcerefreshsummary)
                                {
                                    if (!this.IsLegacyDataTable)
                                    {
                                        if (sender is INotifyPropertyChanging && this._changedSummaryInfo.Count > 0)
                                            this.UpdateSummaries(parentGroup, sender, propertyChangedArgs.PropertyName);
                                        else
                                            this.TopLevelGroup.UpdateSummaries(parentGroup);
                                    }
                                    else
                                        this.TopLevelGroup.UpdateSummaries(parentGroup);
                                }
                                if (isCaptionSummaryAffected || forcerefreshsummary)
                                {
                                    if (!this.IsLegacyDataTable)
                                    {
                                        if (sender is INotifyPropertyChanging && this._changedCaptionSummaryInfo.Count > 0)
                                            this.UpdateCaptionSummaries(parentGroup, sender, propertyChangedArgs.PropertyName);
                                        else
                                            this.TopLevelGroup.UpdateCaptionSummaries(parentGroup);
                                    }
                                    else
                                        this.TopLevelGroup.UpdateCaptionSummaries();
                                }
#endif
                            }

                        }
                    }
                }
            }
			if (canrefreshtablesummary && this.Records != null && this.Records.Count > 0 && this.TableSummaryRows != null)
			{
				var isTableSummaryAffected = (from row in this.TableSummaryRows
											  from col in row.SummaryColumns
											  select col).FirstOrDefault(s => s.MappingName == propertyChangedArgs.PropertyName) != null;
			    if (isTableSummaryAffected)
			    {
#if !SILVERLIGHT
			        if (!this.IsLegacyDataTable)
			        {
			            if (sender is INotifyPropertyChanging && this._changedInfo.Count > 0)
			                this.UpdateTableSummary(sender, propertyChangedArgs.PropertyName);
			            else
			                this.UpdateTableSummary();
			        }
			        else
#endif
			            this.UpdateTableSummary();
			    }
			    else if (forcerefreshtablesummary)
			    {
			        this.UpdateTableSummary();
			    }
			}
            else if (forcerefreshtablesummary)
            {
                this.UpdateTableSummary();
            }

            this.OnRecordPropertyChanged(sender, propertyChangedArgs);
#if !SILVERLIGHT
            if(this._changedInfo != null)
                this._changedInfo.Clear();
            if(this._changedSummaryInfo != null)
                this._changedSummaryInfo.Clear();
            if(this._changedCaptionSummaryInfo != null )
                this._changedCaptionSummaryInfo.Clear();
#endif
			IsInPropertyChange = false;
		}

		private void OnSortChanged(object sender, NotifyCollectionChangedEventArgs e)
		{

			this._shouldUpdateCurrentPosition = true;
			if (this.deferRefreshCount > -1)
			{
				return;
			}

			// if (e.Action != NotifyCollectionChangedAction.Reset)
			{
				this.OnSortDescriptionChanged(e);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:SortDescriptionChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnSortDescriptionChanged(NotifyCollectionChangedEventArgs e)
		{
			this.UpdateCurrentPosition();
			this.Refresh();
		}

		private bool _shouldUpdateCurrentPosition = false;

		protected virtual void UpdateCurrentItem()
		{
            if (this.CurrentItem == null || (this.CurrentPosition == 0 && this.Records.Count <= 0))
            {
				return;
			}

			this._shouldUpdateCurrentPosition = true;
			if (this.IsGrouping)
			{
				var nodeEntry = this.TopLevelGroup.DisplayElements[this.CurrentPosition];
				if (nodeEntry != null)
				{
					var parentGroup = nodeEntry.Parent as Group;
					if (parentGroup != null && (nodeEntry.IsRecords && parentGroup.Records != null))
					{
						if (parentGroup.Records.Count > 0)
						{
							var recordIndex = parentGroup.Records.IndexOf((RecordEntry)nodeEntry);
							var position = this.CurrentPosition;
							position = recordIndex + 1 == parentGroup.GetRecordCount() ? position - 1 : position + 1;
							nodeEntry = this.TopLevelGroup.DisplayElements[position];
							if (nodeEntry != null)
							{
								if (nodeEntry.IsRecords)
								{
									this.currentItem = ((RecordEntry)nodeEntry).Data;
								}
								if (nodeEntry.IsGroups)
								{
									this.currentItem = this.UpdateCurrentItem(nodeEntry);
								}
							}
						}
					}
				}
			}
			else
			{
				var index = this.CurrentPosition;
				index = index + 1 == this.Count ? index - 1 : index + 1;
				this.currentItem = this.GetItemAt(index);
			}
		}

		private object UpdateCurrentItem(NodeEntry entry)
		{
			var g = entry.Parent as Group;
			if (g!=null && g.Groups.Count > 1)
			{
				var index = g.Groups.IndexOf((Group)entry);
                //index = index + 1 == g.Groups.Count ? index - 1 : index + 1;
                if (index + 1 == g.Groups.Count)
                    return this.ScanDown(g.Groups[index]);
                return null;
			}
		    if (g!=null && !(g is TopLevelGroup))
		    {
		        var pg = g.Parent;
		        var index = pg.Groups.IndexOf(g);
		        if (index == 0)
		        {
		            return this.UpdateCurrentItem(pg);
		        }

		        index = index + 1 == pg.Groups.Count ? index - 1 : index + 1;
		        return this.ScanDown(pg.Groups[index].Groups[0]);
		    }

		    return null;
		}

		private object ScanDown(Group group)
		{
			if (group.IsExpanded)
			{
				return @group.Groups == null ? @group.Records[0].Data : this.ScanDown(@group.Groups[0]);
			}

			return null;
		}

		protected virtual void UpdateCurrentPosition()
		{
		    if (this.currentItem == null) return;
		    var currentIndex = -1;
		    if (this.IsGrouping)
		    {
		        currentIndex = this.GroupList.IndexOf(this.currentItem);
		        currentIndex = currentIndex > this.TopLevelGroup.DisplayElements.Count ? -1 : currentIndex;
		    }
		    else
		    {
		        currentIndex = this.IndexOf(this.currentItem);
		    }

		    if (currentIndex > -1)
		    {
		        this.MoveCurrentToPosition(currentIndex);
		    }
		    else
		    {
		        this.currentItem = null;
		        this.currentPosition = -1;
		    }
		}

		/// <summary>
		/// Creates the records.
		/// </summary>
		/// <returns></returns>
		protected virtual IRecordsList CreateRecords()
		{
			return null;
		}

		#region ICollectionView Members

		private bool canFilter ;
		/// <summary>
		/// Gets a value that indicates whether this view supports filtering via the <see cref="P:System.ComponentModel.ICollectionView.Filter"/> property.
		/// </summary>
		/// <value></value>
		/// <returns>true if this view support filtering; otherwise, false.
		/// </returns>
		public bool CanFilter
		{
            get { return CanFilterRecord();}
		}
        protected virtual bool CanFilterRecord()
       {
            return this.Filter != null;
       }
		/// <summary>
		/// Gets a value that indicates whether this view supports grouping via the <see cref="P:System.ComponentModel.ICollectionView.GroupDescriptions"/> property.
		/// </summary>
		/// <value></value>
		/// <returns>true if this view supports grouping; otherwise, false.
		/// </returns>
		public bool CanGroup
		{
			get { return true; }
		}

		private bool canSort = true;
		/// <summary>
		/// Gets a value that indicates whether this view supports sorting via the <see cref="P:System.ComponentModel.ICollectionView.SortDescriptions"/> property.
		/// </summary>
		/// <value></value>
		/// <returns>true if this view supports sorting; otherwise, false.
		/// </returns>
		public bool CanSort
		{
			get
			{
				return this.canSort;
			}
			set
			{
				if (this.canSort != value)
					this.canSort = value;
			}
		}

		/// <summary>
		/// Returns a value that indicates whether a given item belongs to this collection view.
		/// </summary>
		/// <param name="item">The object to check.</param>
		/// <returns>
		/// true if the item belongs to this collection view; otherwise, false.
		/// </returns>
		public bool Contains(object item)
		{
			var rec = this.Records.FirstOrDefault(o => ((RecordEntry)o).Data == item);
			return this.Records.Contains(rec);
		}

		/// <summary>
		/// Gets the item at.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public object GetItemAt(int index)
		{
			if (this.IsGrouping)
			{
				var sourceList = this.GetSourceListCollection();
				if (sourceList != null && index < sourceList.Count)
				{
					return sourceList[index];
				}
			}
			else
			{
				return this.Records.GetItemAt(index);
			}

			return null;
		}

		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns></returns>
		public int IndexOf(object record)
		{
			var index = this.Records.IndexOfRecord(record);
			return index;
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				return this.Records.Count;
			}
		}

		/// <summary>
		/// Gets or sets the cultural info for any operations of the view that may differ by culture, such as sorting.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The culture to use during sorting.
		/// </returns>
		public CultureInfo Culture
		{
			get;
			set;
		}

		/// <summary>
		/// When implementing this interface, raise this event after the current item has been changed.
		/// </summary>
		public event EventHandler CurrentChanged;

		private void RaiseCurrentChangedEvent()
		{
			if (this.inSuspendStateCount == 0 && this.CurrentChanged != null)
			{
#if !SILVERLIGHT
                if (DispatchOwner != null)
                {
                    if (DispatchOwner.Thread != Thread.CurrentThread)
                    {
                        DispatchOwner.Invoke(
                            new Action(() =>
                            {
                                this.CurrentChanged(this, EventArgs.Empty);
                            }));
                        return;
                    }
                }
#endif
                this.CurrentChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// When implementing this interface, raise this event before changing the current item. Event handler can cancel this event.
		/// </summary>
		public event CurrentChangingEventHandler CurrentChanging;

		private bool RaiseCurrentChangingEvent()
		{
			if (this.inSuspendStateCount == 0 && this.CurrentChanging != null)
			{
				var args = new CurrentChangingEventArgs();
				this.CurrentChanging(this, args);
				return !args.Cancel;
			}

			return false;
		}

		private object currentItem;

		/// <summary>
		/// Gets the current item in the view.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The current item of the view or null if there is no current item.
		/// </returns>
		public object CurrentItem
		{
			get
			{
				this.VerifyRefreshNotDeferred();
				return this.currentItem;
			}
		}

		private int currentPosition = -1;

		/// <summary>
		/// Gets the ordinal position of the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> within the view.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The ordinal position of the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> within the view.
		/// </returns>
		public int CurrentPosition
		{
			get { return this.currentPosition; }
		}

		/// <summary>
		/// Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.IDisposable"/> object that you can use to dispose of the calling object.
		/// </returns>
		public IDisposable DeferRefresh()
		{
			this.deferRefreshCount++;
			return new DeferHelper(this);
		}

		/// <summary>
		/// Passeses the filter.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public virtual bool PassesFilter(object record)
		{
			return this.Filter == null || this.Filter(record);
		}

		private Predicate<object> filter = null;

		/// <summary>
		/// Gets or sets a callback used to determine if an item is suitable for inclusion in the view.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A method used to determine if an item is suitable for inclusion in the view.
		/// </returns>
		public Predicate<object> Filter
		{
			get
			{
				return this.filter;
			}

			set
			{
				if (this.filter != value)
				{
					this.filter = value;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is grouping.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is grouping; otherwise, <c>false</c>.
		/// </value>
		public bool IsGrouping
		{
			get
			{
				return this.GroupDescriptions.Count > 0;
			}
		}

		private ObservableCollection<GroupDescription> groups;

		/// <summary>
		/// Gets a collection of <see cref="T:System.ComponentModel.GroupDescription"/> objects that describe how the items in the collection are grouped in the view.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A collection of <see cref="T:System.ComponentModel.GroupDescription"/> objects that describe how the items in the collection are grouped in the view.
		/// </returns>
		public System.Collections.ObjectModel.ObservableCollection<GroupDescription> GroupDescriptions
		{
			get
			{
				if (this.groups == null)
				{
					this.groups = new ObservableCollection<GroupDescription>();
					this.groups.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupsCollectionChanged);
				}

				return this.groups;
			}
		}

		private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.deferRefreshCount > -1)
			{

				return;
			}

			if (this.topLevelGroup != null)
			{
				this.topLevelGroup.Dispose();
				this.topLevelGroup = null;
			}

			this.Refresh();
		}

		/// <summary>
		/// Gets the TopLevelGroup as <see cref="IGroupList"/>.
		/// </summary>
		/// <value>The group list.</value>
		public IGroupList GroupList
		{
			get
			{
				if (this.IsGrouping)
				{
					return this.TopLevelGroup as IGroupList;
				}

				return null;
			}
		}

		private TopLevelGroup topLevelGroup;

		/// <summary>
		/// Gets the top level group.
		/// </summary>
		/// <value>The top level group.</value>
		public TopLevelGroup TopLevelGroup
		{
			get
			{
				if (this.IsGrouping && this.topLevelGroup == null)
				{
					this.topLevelGroup = this.CreateTopLevelGroup();
					var groupBy = this.GroupDescriptions.OfType<PropertyGroupDescription>().Select(g => g.PropertyName).ToArray();
					IEnumerable<GroupResult> result = this.GetGroupResult(groupBy);

					//This is used to populate the group record for the paging.
					if (this.PagedSource != null && this.EnablePaging)
					{
						this.topLevelGroup.Populate(result, this.PagedSource,this.IsViewLevelPaging);
					}
					else

						this.topLevelGroup.Populate(result);
					// this.PopulateRecordsFromTopLevelGroup(); // this gets called in RefreshSort()
					this.topLevelGroup.CollectionChanged += OnTopLevelGroupCollectionChanged;
					this.OnTopLevelGroupPopulated(this.topLevelGroup);
					this.topLevelGroup.UpdateCaptionSummaries();
                    if (CanFilter)
					{
						this.RefreshFilters();
					}
					// this.UpdateTableSummary();
					if (!this.IsInEndDefer)
					{
						this.Refresh();
					}
				}

				return this.topLevelGroup;
			}
		}

		protected virtual void OnTopLevelGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					var record = e.NewItems[0] as RecordEntry;
					if (e.NewStartingIndex >= this.Count)
					{
						this.Records.Add(record);
					}
					else
					{
						this.Records.Insert(e.NewStartingIndex, record);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					var record1 = e.OldItems[0] as RecordEntry;
                    this.Records.Remove(record1);

					// sometimes the UI would raise a change notification for deleting a record, we should update the underlying sourcelist
					// sometimes when the underlying source updates with a delete command, this code should not execute
					//if (!this.IsInSuspend)
					//{
					//    this.IsInSuspend = true;
					//    var sourceList = this.GetSourceListCollection();
					//    var sourceIdx = this.FindItemIdxInSource(sourceList, record1.Data);
					//    if (sourceIdx > -1 && sourceIdx < sourceList.Count)
					//    {
					//        sourceList.RemoveAt(sourceIdx);
					//    }
					//    this.IsInSuspend = false;
					//}
					break;

				case NotifyCollectionChangedAction.Reset:
					this.Records.Clear();
					this.UnwireEvents();
					break;
			}
		}

		/// <summary>
		/// Gets the group result.
		/// </summary>
		/// <param name="groupBy">The group by.</param>
		/// <returns></returns>
		protected virtual IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
		{
			return null;
		}

		/// <summary>
		/// Called when the TopLevelGroup is populated with groups and records.
		/// </summary>
		/// <param name="topLevelGroup">The top level group.</param>
		protected virtual void OnTopLevelGroupPopulated(TopLevelGroup topLevelGroup)
		{
		}

		/// <summary>
		/// Creates the top level group.
		/// </summary>
		/// <returns></returns>
		protected virtual TopLevelGroup CreateTopLevelGroup()
		{
			return new TopLevelGroup(this);
		}

		/// <summary>
		/// Gets the top-level groups.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A read-only collection of the top-level groups or null if there are no groups.
		/// </returns>
		public ReadOnlyObservableCollection<object> Groups
		{
			get
			{
				if (this.IsGrouping)
				{
					//return this.TopLevelGroup.CollectionViewGroup.Items;
					return new ReadOnlyObservableCollection<object>(this.TopLevelGroup.Groups.OfType<object>().ToObservableCollection());
				}
				return null;
			}
		}

		#region Currency Manager

		/// <summary>
		/// Gets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the end of the collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// Returns true if the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the end of the collection; otherwise, false.
		/// </returns>
		private bool isCurrentAfterLast = false;

		/// <summary>
		/// Gets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the end of the collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// Returns true if the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the end of the collection; otherwise, false.
		/// </returns>
		public bool IsCurrentAfterLast
		{
			get { return this.isCurrentAfterLast; }
		}

		private bool isCurrentBeforeFirst = false;

		/// <summary>
		/// Gets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the beginning of the collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// Returns true if the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the beginning of the collection; otherwise, false.
		/// </returns>
		public bool IsCurrentBeforeFirst
		{
			get { return this.isCurrentBeforeFirst; }
		}

		/// <summary>
		/// Returns a value that indicates whether the resulting view is empty.
		/// </summary>
		/// <value></value>
		/// <returns>true if the resulting view is empty; otherwise, false.
		/// </returns>
		private bool isEmpty = false;
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		private int oldCurrentPosition = -1;

		private void MoveCurrencyOffDeletedElement(int currentPosition)
		{
			int num = this.Records.Count - 1;
			int newPosition = (this.oldCurrentPosition < num) ? this.oldCurrentPosition : num;
			this.currentElementWasRemoved = false;
			this.RaiseCurrentChangingEvent();
			if (newPosition < 0)
			{
				this.SetCurrent(null, newPosition);
			}
			else
			{
				this.SetCurrent(this.GetItemAt(newPosition), newPosition);
			}

			this.RaiseCurrentChangedEvent();
		}

		/// <summary>
		/// Sets the current item.
		/// </summary>
		/// <param name="newItem">The new item.</param>
		/// <param name="newPosition">The new position.</param>
		protected void SetCurrent(object newItem, int newPosition)
		{
			int count = (newItem != null) ? 0 : (this.IsEmpty ? 0 : this.Records.Count);
			this.SetCurrent(newItem, newPosition, count);
			if (this.ExternalCollectionView != null && this.ExternalCollectionView.CurrentItem != newItem)
			{
				this.ExternalCollectionView.MoveCurrentTo(newItem);
			}
		}

		/// <summary>
		/// Sets the current item.
		/// </summary>
		/// <param name="newItem">The new item.</param>
		/// <param name="newPosition">The new position.</param>
		/// <param name="count">The count.</param>
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
				this.currentPosition = this.Records.IndexOfRecord(this.currentItem);
			}
			else
			{
				this.currentPosition = this.GroupList.IndexOf(this.currentItem);
			}
		}

		private CollectionViewFlags flags;

		private void SetFlag(CollectionViewFlags flags, bool value)
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

		private bool IsCurrentInView
		{
			get
			{
				this.VerifyRefreshNotDeferred();
				return (0 <= this.CurrentPosition) && (this.CurrentPosition < this.Records.Count);
			}
		}

		/// <summary>
		/// Sets the specified item to be the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view.
		/// </summary>
		/// <param name="item">The item to set as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.</param>
		/// <returns>
		/// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is within the view; otherwise, false.
		/// </returns>
		public bool MoveCurrentTo(object item)
		{
			this.VerifyRefreshNotDeferred();

            if (item == null)
            {
                this.currentItem = null;
                this.currentPosition = -1;
            }

			if ((object.Equals(this.CurrentItem, item) && !IsGrouping || object.Equals(this.NewItemPlaceholder, item)) && (item != null || this.IsCurrentInView))
			{
				return this.IsCurrentInView;
			}

			int position = -1;
			if (item != null)
			{
				if (this.PassesFilter(item))
				{
					position = this.Records.IndexOfRecord(item);

					if (this.IsGrouping)
					{
						this.ExpandGroups(this.TopLevelGroup, item);
						this.TopLevelGroup.ResetDisplayElements();
						position = this.Records.IndexOfRecord(item);
						position = this.TopLevelGroup.DisplayElements.IndexOf(this.Records[position]);
					}

				}
			}

			return this.MoveCurrentToPosition(position);
		}

		/// <summary>
		/// Sets the first item in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item within the view; otherwise, false.
		/// </returns>
		public bool MoveCurrentToFirst()
		{
			this.VerifyRefreshNotDeferred();
			int position = 0;
			IEditableCollectionView view = this as IEditableCollectionView;
			if ((view != null) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning))
			{
				position = 1;
			}

			return this.MoveCurrentToPosition(position);
		}

		/// <summary>
		/// Sets the last item in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item within the view; otherwise, false.
		/// </returns>
		public bool MoveCurrentToLast()
		{
			this.VerifyRefreshNotDeferred();
			int position = this.Records.Count - 1;
			IEditableCollectionView view = this as IEditableCollectionView;
			if ((view != null) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd))
			{
				position--;
			}

			return this.MoveCurrentToPosition(position);
		}

		/// <summary>
		/// Sets the item after the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item within the view; otherwise, false.
		/// </returns>
		public bool MoveCurrentToNext()
		{
			this.VerifyRefreshNotDeferred();
			int position = this.CurrentPosition + 1;
			int count = this.Records.Count;
			IEditableCollectionView view = this as IEditableCollectionView;
			if (((view != null) && (position == 0)) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning))
			{
				position = 1;
			}

			if (((view != null) && (position == (count - 1))) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd))
			{
				position = count;
			}

			return (position <= count) && this.MoveCurrentToPosition(position);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is current in sync.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is current in sync; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Sets the item at the specified index to be the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view.
		/// </summary>
		/// <param name="position">The index to set the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> to.</param>
		/// <returns>
		/// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item within the view; otherwise, false.
		/// </returns>
		public bool MoveCurrentToPosition(int position)
		{
			this.VerifyRefreshNotDeferred();
			var totalCount = !this.IsGrouping ? this.Records.Count : this.TopLevelGroup.DisplayElements.Count;
			if ((position < -1) || (position > totalCount))
			{
				throw new ArgumentOutOfRangeException("position");
				//return false;
			}

			if ((position != this.CurrentPosition) || !this.IsCurrentInSync || this._shouldUpdateCurrentPosition)
			{
				this._shouldUpdateCurrentPosition = false;

				object newItem = ((0 <= position) && (position < this.Records.Count)) ? this.GetItemAt(position) : null;

				if (this.IsGrouping)
				{
					var nodeEntry = this.TopLevelGroup.DisplayElements[position];
					if (nodeEntry != null && nodeEntry.IsRecords)
					{
						newItem = ((RecordEntry)nodeEntry).Data;
					}
				}

#if !SILVERLIGHT
				if (newItem != CollectionView.NewItemPlaceholder && newItem != null)
#else
                if(newItem != null)
#endif
				{
					bool isCurrentAfterLast = this.IsCurrentAfterLast;
					bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
					this.RaiseCurrentChangingEvent();
					this.SetCurrent(newItem, position);
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
			}

			return this.IsCurrentInView;
		}

		private void _MoveCurrentToPosition(int position)
		{
			if (position < 0)
			{
				this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
				this.SetCurrent(null, -1);
			}
			else if (position >= this.Records.Count)
			{
				this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
				this.SetCurrent(null, this.Records.Count);
			}
			else
			{
				this.SetFlag(CollectionViewFlags.IsCurrentAfterLast | CollectionViewFlags.IsCurrentBeforeFirst, false);
				var record = this.Records[position];
				this.SetCurrent(((RecordEntry)record).Data, position);
			}
		}

		/// <summary>
		/// Sets the item before the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item within the view; otherwise, false.
		/// </returns>
		public bool MoveCurrentToPrevious()
		{
			this.VerifyRefreshNotDeferred();
			int position = this.CurrentPosition - 1;
			int count = this.Records.Count;
			IEditableCollectionView view = this as IEditableCollectionView;
			if (((view != null) && (position == (count - 1))) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd))
			{
				position = count - 2;
			}

			if (((view != null) && (position == 0)) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning))
			{
				position = -1;
			}

			return (position >= -1) && this.MoveCurrentToPosition(position);
		}

		internal void VerifyRefreshNotDeferred()
		{
			if (this.IsRefreshDeferred)
			{
				throw new InvalidOperationException("No Check Or Change When Deferred");
			}
		}

		private void AdjustCurrencyForAdd(int index)
		{
			if (this.Records.Count == 1)
			{
				this.SetCurrent(null, -1);
			}
			else if (index <= this.CurrentPosition)
			{
				int newPosition = this.CurrentPosition + 1;
				if (newPosition < this.Records.Count)
				{
					this.SetCurrent(this.GetItemAt(newPosition), newPosition);
				}
				else
				{
					this.SetCurrent(null, this.Records.Count);
				}
			}
		}

		private void AdjustCurrencyForMove(int oldIndex, int newIndex)
		{
			if (oldIndex == this.CurrentPosition)
			{
				this.SetCurrent(this.GetItemAt(newIndex), newIndex);
			}
			else if ((oldIndex < this.CurrentPosition) && (this.CurrentPosition <= newIndex))
			{
				this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1);
			}
			else if ((newIndex <= this.CurrentPosition) && (this.CurrentPosition < oldIndex))
			{
				this.SetCurrent(this.CurrentItem, this.CurrentPosition + 1);
			}
		}

		private void AdjustCurrencyForRemove(int index)
		{
			if (index < this.CurrentPosition)
			{
				this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1);
			}
			else if (index == this.CurrentPosition)
			{
				this.currentElementWasRemoved = true;
			}
		}

		private void AdjustCurrencyForReplace(int index)
		{
			if (index == this.CurrentPosition)
			{
				this.currentElementWasRemoved = true;
			}
		}

		private void ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems.Count != 1)
					{
						throw new NotSupportedException("Range Actions Not Supported");
					}

					break;

				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems.Count != 1)
					{
						throw new NotSupportedException("Range Actions Not Supported");
					}

					break;

				case NotifyCollectionChangedAction.Replace:
					if ((e.NewItems.Count != 1) || (e.OldItems.Count != 1))
					{
						throw new NotSupportedException("Range Actions Not Supported");
					}

					break;

#if !SILVERLIGHT
				case NotifyCollectionChangedAction.Move:
					if (e.NewItems.Count != 1)
					{
						throw new NotSupportedException("Range Actions Not Supported");
					}

					if (e.NewStartingIndex >= 0)
					{
						break;
					}

					throw new InvalidOperationException("CannotMoveToUnknownPosition");
#endif
				case NotifyCollectionChangedAction.Reset:
					break;

				default:
					throw new NotSupportedException("UnexpectedCollectionChangeAction");
			}
		}

		private object NewItemPlaceholder
		{
			get;
			set;
		}

		private int AdjustBefore(NotifyCollectionChangedAction action, object item, int index)
		{
			if (action == NotifyCollectionChangedAction.Reset)
			{
				return -1;
			}
			if (item == this.NewItemPlaceholder)
			{
				if (this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtBeginning)
				{
					return this.Records.Count - 1;
				}
				return 0;
			}
			if ((this.IsAddingNew && (this.NewItemPlaceholderPosition != NewItemPlaceholderPosition.None)) && object.Equals(item, this.newItem))
			{
				if (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
				{
					return 1;
				}
				if (!this.UsesLocalArray)
				{
					return index;
				}
				return this.Records.Count - 2;
			}
			int num = this.IsGrouping ? 0 : ((this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? (this.IsAddingNew ? 2 : 1) : 0);
			IList ilist = this.SourceCollection as IList;
			if (action == NotifyCollectionChangedAction.Add)
			{
				if (index >= 0)
				{
					if (!object.Equals(item, ((RecordEntry)this.Records[index]).Data))
					{
						throw new InvalidOperationException("AddedItemNotAtIndex");
					}
				}
				else
				{
					index = ilist.IndexOf(item);
					if (index < 0)
					{
						throw new InvalidOperationException("AddedItemNotInCollection");
					}
				}
			}
			if (!this.UsesLocalArray)
			{
				if (this.IsAddingNew && (index > this.newItemIndex))
				{
					index--;
				}
				return index + num;
			}
			if (action == NotifyCollectionChangedAction.Add)
			{
				if (!this.PassesFilter(item))
				{
					return -2;
				}
				else if (index < 0)
				{
					index = ~index;
				}

				//                IComparer<object> comparer = this.ActiveComparer;
				//                ArrayList internalList = ilist as ArrayList;
				//                if (internalList != null)
				//                {
				//#if !SILVERLIGHT
				//                    index = internalList.BinarySearch(item, comparer);
				//#else
				//                    index = internalList.BinarySearch(item, new ObjectComparer());
				//#endif
				//                    if (index < 0)
				//                    {
				//                        index = ~index;
				//                    }
				//                }
				//                else
				//                {
				//                    index = -1;
				//                }
			}
			else if (action == NotifyCollectionChangedAction.Remove)
			{
				if (!this.IsAddingNew || (item != this.newItem))
				{

					index = this.Records.IndexOfRecord(item);
					if (index < 0)
					{
						return -2;
					}
				}
				else
				{
					switch (this.NewItemPlaceholderPosition)
					{
						case NewItemPlaceholderPosition.None:
							return this.Records.Count - 1;

						case NewItemPlaceholderPosition.AtBeginning:
							return 1;

						case NewItemPlaceholderPosition.AtEnd:
							return this.Records.Count - 2;
					}
				}
			}
			else
			{
				index = -1;
			}
			if (index >= 0)
			{
				return index + num;
			}
			return index;
		}

		#endregion

		/// <summary>
		/// Recreates the view.
		/// </summary>
		public void Refresh()
		{
			//IEditableCollectionView view = this as IEditableCollectionView;
			//if ((view != null) && (view.IsAddingNew || view.IsEditingItem))
			//{
			//    throw new InvalidOperationException("Member Not Allowed During Add Or Edit");
			//}

			this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
			if (this.deferRefreshCount > -1)
			{
				return;
			}
            this.EnsureRecordsInitialized();
			this.RefreshOverride();
		}

        private void EnsureRecordsInitialized()
        {
            if (this.Records == null)
                this.EnsureInitialized();
        }

		//Method added to refresh the toplevel group when we apply filters for the fix [SD8300]
		public void RefreshFiltering()
		{
			this.RefreshTopLevelGroup();
		}

		private bool CheckFlag(CollectionViewFlags flags)
		{
			return (this.flags & flags) != 0;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is refresh deferred.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is refresh deferred; otherwise, <c>false</c>.
		/// </value>
		protected bool IsRefreshDeferred
		{
			get
			{
				return this.deferRefreshCount > 0;
			}
		}

		/// <summary>
		/// If the current view is in defer mode, the flags for refresh is set, otherwise it refreshes the view.
		/// </summary>
		protected void RefreshOrDefer()
		{
			if (this.IsRefreshDeferred)
			{
				this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
			}
			else
			{
				this.RefreshOverride();
				this.SetFlag(CollectionViewFlags.NeedsRefresh, false);
			}
		}

		private void PrepareSortAndFilter()
		{
			if (this.sortDescriptions != null && this.sortDescriptions.Count > 0)
			{
				this.activeComparer = new SortFieldComparer(this.sortDescriptions, this.SortComparers, this.Culture, (record, propName) =>
				{
					var provider = this.GetPropertyAccessProvider();
					if (provider != null)
					{
						return provider.GetValue(record, propName);
					}

					return null;
				});
			}
		}

		/// <summary>
		/// Override this method to modify actions during refresh.
		/// </summary>
		protected virtual void RefreshOverride()
		{
			object item = this.currentItem;
			bool flag = this.CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
			bool flag2 = this.CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
			int num = this.currentPosition;
			if (item == null)
				this.RaiseCurrentChangingEvent();
			this.PrepareSortAndFilter();
			if (this.IsEmpty || flag2)
			{
				this.MoveCurrentToPosition(-1);
			}
			else if (flag)
			{
				this.MoveCurrentToPosition(!this.IsGrouping ? this.Records.Count : this.TopLevelGroup.DisplayElements.Count);
			}

			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (item == null)
				this.RaiseCurrentChangedEvent();
			if (this.IsCurrentAfterLast != flag)
			{
				this.OnPropertyChanged("IsCurrentAfterLast");
			}

			if (this.IsCurrentBeforeFirst != flag2)
			{
				this.OnPropertyChanged("IsCurrentBeforeFirst");
			}

			if (num != this.CurrentPosition)
			{
				this.OnPropertyChanged("CurrentPosition");
			}

			if (item != this.CurrentItem)
			{
				this.OnPropertyChanged("CurrentItem");
			}
		}

		private SortDescriptionCollection sortDescriptions;

		/// <summary>
		/// Gets a collection of <see cref="T:System.ComponentModel.SortDescription"/> objects that describe how the items in the collection are sorted in the view.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A collection of <see cref="T:System.ComponentModel.SortDescription"/> objects that describe how the items in the collection are sorted in the view.
		/// </returns>
		public SortDescriptionCollection SortDescriptions
		{
			get { return this.sortDescriptions ?? (this.sortDescriptions = new SortDescriptionCollection()); }
		}

		private Dictionary<string, IComparer<object>> sortComparers;
		public Dictionary<string, IComparer<object>> SortComparers
		{
			get { return this.sortComparers ?? (this.sortComparers = new Dictionary<string, IComparer<object>>()); }
		}


		private bool _isNotifyPropertyTagged = false;
		private void WireNotifyPropertyChangedForUnderlyingSource(bool forceupdate)
		{
#if !SILVERLIGHT
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
            if (forceupdate) _isNotifyPropertyTagged = false;
            if (_isNotifyPropertyTagged || SourceCollection == null)
                return;
            _isNotifyPropertyTagged = true;
            foreach (var record in SourceCollection)
                AddNotifyListener(record);
		}

		private void UnwireNotifyPropertyChangedForUnderlyingSource()
		{
#if !SILVERLIGHT
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
            if (!_isNotifyPropertyTagged || SourceCollection == null)
                return;
            _isNotifyPropertyTagged = false;
            foreach (var record in SourceCollection)
                RemoveNotifyListener(record);
		}

		private void RemoveNotifyListener(object record)
		{
#if !SILVERLIGHT
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
			var notifyPropertyChanged = record as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
#if !SILVERLIGHT
                PropertyChangedEventManager.RemoveListener(notifyPropertyChanged, this, string.Empty);
                if (NotifyComplexPropertyChanges)
                {
                    var cp = this.ComplexProperties;
                    var pd = this.GetItemProperties();
                    foreach (var mappingname in cp)
                    {
                        var kvp = pd.GetPropertyDescriptor(record, mappingname.ToString());
                        if (kvp.Value is INotifyPropertyChanged)
                            (kvp.Value as INotifyPropertyChanged).PropertyChanged -= OnComplexPropertyChanged;
                    }
                }
#else
                notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
#endif
            }
#if !SILVERLIGHT
            else
            {
                if (record is ICustomTypeDescriptor)
                    foreach (PropertyDescriptor property in this.GetItemProperties())
                        property.RemoveValueChanged(record, OnPropertyChanged);
            }
            var notifyPropertyChanging = record as INotifyPropertyChanging;
            if (notifyPropertyChanging != null)
            {
                PropertyChangingEventManager.RemoveListener(notifyPropertyChanging, this, string.Empty);
            }
#endif
        }

		private void AddNotifyListener(object record)
		{
#if !SILVERLIGHT
            if (this.IsLegacyDataTable || source is IBindingList)
                return;
#endif
			var notifyPropertyChanged = record as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                if (!this._isNotifyPropertyTagged)
                {
                    this._isNotifyPropertyTagged = true;
                }
#if !SILVERLIGHT
                PropertyChangedEventManager.AddListener(notifyPropertyChanged, this, string.Empty);
                if (this.NotifyComplexPropertyChanges)
                {
                    var cp = this.ComplexProperties;
                    var pd = this.GetItemProperties();
                    foreach (var mappingname in cp)
                    {
                        var kvp = pd.GetPropertyDescriptor(record, mappingname.ToString());
                        if (kvp.Value is INotifyPropertyChanged)
                            (kvp.Value as INotifyPropertyChanged).PropertyChanged += OnComplexPropertyChanged;
                    }
                }
#else
                notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
#endif
            }
#if !SILVERLIGHT
            else
            {
                if (record is ICustomTypeDescriptor)
                {
                    foreach (PropertyDescriptor property in this.GetItemProperties())
                        property.AddValueChanged(record, OnPropertyChanged);
                }
            }
            var notifyPropertyChanging = record as INotifyPropertyChanging;
            if (notifyPropertyChanging != null)
            {
                PropertyChangingEventManager.AddListener(notifyPropertyChanging, this, string.Empty);
            }
#endif
        }

#if SILVERLIGHT

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var propertyChangedHandler = this as IPropertyChangedEventHandler;
			if (propertyChangedHandler != null)
			{
				propertyChangedHandler.OnPropertyChanged(sender, e);
			}
		}

#endif

        private IEnumerable source;

		/// <summary>
		/// Returns the underlying collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerable"/> object that is the underlying collection.
		/// </returns>
		public System.Collections.IEnumerable SourceCollection
		{
			get { return this.source; }
		}

	    protected void SetSourceType(Type sourceType)
	    {
	        if (sourceType == null || sourceType == SourceType)
	            return;

            if (sourceType == typeof(Object))
	            return;

	        this.SourceType = sourceType;
	        IsItemPropertiesTypeSet = true;

	        if (SourceType.IsInterface)
	            this.propertyAccessProvider = this.CreateItemPropertiesProvider();

	        if (SourceType.FullName == "IronRuby.Ruby")
	        {
#if SyncfusionFramework4_0
	            this.isDynamicSourceEvaluated = false;
#endif
	            this.propertyAccessProvider = this.CreateItemPropertiesProvider();
	        }
	    }

	    private Type _sourceType = null;
		public Type SourceType
		{
			get { return _sourceType; }
			private set { _sourceType = value; }
		}

#if !SILVERLIGHT
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
				return this.SourceCollection is DataTable || this.SourceCollection is DataView;
			}
		}

        public bool IsDataViewWrapper
        {
            get { return this.SourceCollection is DataViewWrapperList; }
        }
#endif

		/// <summary>
		/// Processes the collection during editing process.
		/// </summary>
		/// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			this.ValidateCollectionChangedEventArgs(args);
			int adjustedOldIndex = -1;
			int adjustedNewIndex = -1;

			if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				if (this.IsEditingItem)
				{
					this.ImplicitlyCancelEdit();
				}

				if (this.IsAddingNew)
				{
					var rec = this.Records.FirstOrDefault(o => ((RecordEntry)o).Data == this.newItem);
					if (rec == null)
					{
						this.EndAddNew(true);
					}
				}

				this.RefreshOrDefer();
			}
			else if ((args.Action == NotifyCollectionChangedAction.Add) && (this.newItemIndex == -2))
			{
				this.BeginAddNew(args.NewItems[0], args.NewStartingIndex);
			}
			else
			{
				if (args.Action != NotifyCollectionChangedAction.Remove)
				{
					adjustedNewIndex = this.AdjustBefore(NotifyCollectionChangedAction.Add, args.NewItems[0], args.NewStartingIndex);
				}
				if (args.Action != NotifyCollectionChangedAction.Add)
				{
					adjustedOldIndex = this.AdjustBefore(NotifyCollectionChangedAction.Remove, args.OldItems[0], args.OldStartingIndex);
					if ((adjustedOldIndex >= 0) && (adjustedOldIndex < adjustedNewIndex))
					{
						adjustedNewIndex--;
					}
				}
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						if (args.NewStartingIndex <= this.newItemIndex)
						{
							this.newItemIndex++;
						}

						break;

					case NotifyCollectionChangedAction.Remove:
						{
							if (args.OldStartingIndex < this.newItemIndex)
							{
								this.newItemIndex--;
							}

							object obj2 = args.OldItems[0];
							if (obj2 == this.CurrentEditItem)
							{
								this.ImplicitlyCancelEdit();
							}
							else if (obj2 == this.CurrentAddItem)
							{
								this.EndAddNew(true);
							}

							break;
						}
#if !SILVERLIGHT
					case NotifyCollectionChangedAction.Move:
						if ((args.OldStartingIndex >= this.newItemIndex) || (this.newItemIndex >= args.NewStartingIndex))
						{
							if ((args.NewStartingIndex <= this.newItemIndex) && (this.newItemIndex < args.OldStartingIndex))
							{
								this.newItemIndex++;
							}

							break;
						}
						this.newItemIndex--;
						break;
#endif
				}
				this.ProcessCollectionChangedWithAdjustedIndex(args, null, adjustedOldIndex, adjustedNewIndex);
			}
		}

		/// <summary>
		/// Gets the value if the editing mode is in suspend state. Use this variable for listening changes in other source list changes.
		/// </summary>
		public bool IsInSuspend
		{
			get;
			protected set;
		}

		private void ProcessCollectionChangedWithAdjustedIndex(NotifyCollectionChangedEventArgs args, NotifyCollectionChangedEventArgs args2, int adjustedOldIndex, int adjustedNewIndex)
		{
			int num = (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? (this.IsAddingNew ? 2 : 1) : 0;
			int currentPosition = this.CurrentPosition;
			int num3 = this.CurrentPosition;
			object currentItem = this.CurrentItem;
			bool isCurrentAfterLast = this.IsCurrentAfterLast;
			bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
			this.IsInSuspend = true;
			var isReplace = adjustedOldIndex == adjustedNewIndex;
			args = this.ProcessCollectionChangedAction(args, adjustedOldIndex, adjustedNewIndex);
			if (args == null)
			{
				// for null values we terminate this method
				return;
			}

			if (args2 != null && !isReplace)
			{
				args2 = this.ProcessCollectionChangedAction(args2, adjustedOldIndex, adjustedNewIndex);
				if (args2 == null)
				{
					return;
				}
			}

			bool flag4 = this.IsCurrentAfterLast != isCurrentAfterLast;
			bool flag5 = this.IsCurrentBeforeFirst != isCurrentBeforeFirst;
			bool flag6 = this.CurrentPosition != num3;
			bool flag7 = this.CurrentItem != currentItem;
			isCurrentAfterLast = this.IsCurrentAfterLast;
			isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
			num3 = this.CurrentPosition;
			currentItem = this.CurrentItem;

			if (!this.IsGrouping)
			{
				this.OnCollectionChanged(args);
				if (args2 != null && !isReplace)
				{
					this.OnCollectionChanged(args2);
				}

				if (this.IsCurrentAfterLast != isCurrentAfterLast)
				{
					flag4 = false;
					isCurrentAfterLast = this.IsCurrentAfterLast;
				}

				if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
				{
					flag5 = false;
					isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
				}

				if (this.CurrentPosition != num3)
				{
					flag6 = false;
					num3 = this.CurrentPosition;
				}

				if (this.CurrentItem != currentItem)
				{
					flag7 = false;
					currentItem = this.CurrentItem;
				}
			}

			if (this.currentElementWasRemoved)
			{
				//this.MoveCurrencyOffDeletedElement(currentPosition);
				flag4 = flag4 || (this.IsCurrentAfterLast != isCurrentAfterLast);
				flag5 = flag5 || (this.IsCurrentBeforeFirst != isCurrentBeforeFirst);
				flag6 = flag6 || (this.CurrentPosition != num3);
				flag7 = flag7 || (this.CurrentItem != currentItem);
			}

			if (flag4)
			{
				this.OnPropertyChanged("IsCurrentAfterLast");
			}

			if (flag5)
			{
				this.OnPropertyChanged("IsCurrentBeforeFirst");
			}

			if (flag6)
			{
				this.OnPropertyChanged("CurrentPosition");
			}

			if (flag7)
			{
				this.OnPropertyChanged("CurrentItem");
			}
			this.IsInSuspend = false;
		}

		private NotifyCollectionChangedEventArgs ProcessCollectionChangedAction(NotifyCollectionChangedEventArgs args, int adjustedOldIndex, int adjustedNewIndex)
		{
			NotifyCollectionChangedEventArgs changedArgs = args;
			NotifyCollectionChangedAction action = args.Action;
			if ((adjustedOldIndex == adjustedNewIndex) && (adjustedOldIndex >= 0))
			{
				action = NotifyCollectionChangedAction.Replace;
			}
			else if (adjustedOldIndex == -1)
			{
				if (adjustedNewIndex < 0)
				{
					if (args.Action == NotifyCollectionChangedAction.Add)
					{
						return null;
					}

					action = NotifyCollectionChangedAction.Remove;
				}
			}
			else if (adjustedOldIndex < -1)
			{
				if (adjustedNewIndex < 0)
				{
					return null;
				}

				action = NotifyCollectionChangedAction.Add;
			}
			else if (adjustedNewIndex < 0)
			{
				action = NotifyCollectionChangedAction.Remove;
			}

			int num = (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? (this.IsAddingNew ? 2 : 1) : 0;
			if (action == NotifyCollectionChangedAction.Add)
			{
				if (!this.CanAddNew)
				{
					return null;
				}

				if (!this.IsGrouping)
				{
					args = new NotifyCollectionChangedEventArgs(action, args.NewItems[0], adjustedNewIndex);
					if ((this.NewItemPlaceholder != args.NewItems[0]) && (!this.IsAddingNew || !Equals(this.newItem, args.NewItems[0])))
					{
						var record = this.Records.CreateRecordEntry(args.NewItems[0]);
                        this.Records.Insert(adjustedNewIndex - num, record);
					}
				}
				else
				{
                    this.GroupList.Add(args.NewItems[0], IsInSourceCollectionChange);
				}
			}
			else if (action == NotifyCollectionChangedAction.Remove)
			{
				if (!this.CanRemove)
				{
					return null;
				}

				int index = adjustedOldIndex - num;
				// var rec = this.Records.FirstOrDefault(o => ((RecordEntry)o).Data == args.OldItems[0]);
				if (index > -1 && index < this.Records.Count)
				{
					// if we are in grouping, first remove from the group list
					if (!this.IsGrouping)
					{
						this.Records.RemoveAt(index);
						this.MoveCurrentToPosition(index);
					}
					else
					{
						this.GroupList.Remove(args.OldItems[0],IsInSourceCollectionChange);
					}
				}

				if (!this.IsGrouping)
				{
					this.AdjustCurrencyForRemove(adjustedOldIndex);
					changedArgs = new NotifyCollectionChangedEventArgs(action, args.OldItems[0], adjustedOldIndex);
				}
			}
			else if (action == NotifyCollectionChangedAction.Replace)
			{
				var replaceItem = args.Action == NotifyCollectionChangedAction.Add ? args.NewItems[0] : args.OldItems[0];
				if (!this.IsGrouping)
				{
					this.AdjustCurrencyForReplace(adjustedOldIndex);
					changedArgs = new NotifyCollectionChangedEventArgs(action, replaceItem, replaceItem, adjustedOldIndex);
				}
				else
				{
                    this.GroupList.Remove(replaceItem, IsInSourceCollectionChange);
                    this.GroupList.Insert(replaceItem, adjustedNewIndex, IsInSourceCollectionChange);
				}
			}

			return changedArgs;
		}

		private void UpdateSourceList(NotifyCollectionChangedEventArgs args, int adjustedOldIndex, int adjustedNewIndex)
		{
			var action = args.Action;
			int num = (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? (this.IsAddingNew ? 2 : 1) : 0;
			// update the underlying source list after the CollectionChanged event
			switch (action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						if (!this.CanAddNew)
						{
							return;
						}

						var sourceList = this.GetSourceListCollection();
						if (!this.IsGrouping)
						{
							if ((this.NewItemPlaceholder != args.NewItems[0]) && (!this.IsAddingNew || !Equals(this.newItem, args.NewItems[0])))
							{
								sourceList.Insert(adjustedNewIndex - num, args.NewItems[0]);
							}
						}
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					{
						if (!this.CanRemove)
						{
							return;
						}
						var sourceList = this.GetSourceListCollection();
						int index = adjustedOldIndex - num;
						if (index < sourceList.Count)
						{
							var sourceIdx = this.FindItemIdxInSource(sourceList, args.OldItems[0]);
							sourceList.RemoveAt(sourceIdx);
						}

						if (this.IsGrouping)
						{
							this.GroupList.Remove(args.OldItems[0],true);
						}
					}
					break;

#if !SILVERLIGHT
				case NotifyCollectionChangedAction.Move:
					{
						if (!this.CanAddNew || !this.CanRemove)
						{
							return;
						}

						var sourceList = this.GetSourceListCollection();

						var sourceIdx = this.FindItemIdxInSource(sourceList, args.OldItems[0]);
						if (sourceIdx > -1 && sourceIdx < sourceList.Count)
						{
							sourceList.RemoveAt(sourceIdx);
						}
						if (this.NewItemPlaceholder != args.NewItems[0])
						{
							// the sourcelist only has the items, we don't have to maintain any currency index for items insert here
							sourceList.Add(args.NewItems[0]);
						}

						break;
					}
#endif
			}
		}

		private int FindItemIdxInSource(IList sourceList, object record)
		{
			var idx = -1;
			foreach (var item in sourceList)
			{
				idx += 1;
#if !SILVERLIGHT
				if (this.IsLegacyDataTable)
				{
					if (((DataRowView)item).Row == (((DataRowView)record).Row))
					{
						break;
					}
				}
				else
				{
#endif
					if (Equals(item, record))
					{
						break;
					}
#if !SILVERLIGHT
				}
#endif
			}

			return idx;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.source.GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private int inSuspendStateCount = 0;

		/// <summary>
		/// Suspends the events.
		/// </summary>
		public void SuspendEvents()
		{
			this.inSuspendStateCount += 1;
		}

		/// <summary>
		/// Resumes the events.
		/// </summary>
		public void ResumeEvents()
		{
			this.inSuspendStateCount -= 1;
		}

		/// <summary>
		/// Raises the collection changed event.
		/// </summary>
		/// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected void RaiseCollectionChangedEvent(NotifyCollectionChangedEventArgs args)
		{
			if (this.inSuspendStateCount == 0 & this.CollectionChanged != null)
			{
				this.CollectionChanged(this, args);
			}
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether this instance is in end defer.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is in end defer; otherwise, <c>false</c>.
		/// </value>
		public bool IsInEndDefer
		{
			get;
			private set;
		}

		internal void EndDefer()
		{
			this.deferRefreshCount--;
			if (this.deferRefreshCount == -1)
			{
				this.IsInEndDefer = true;
				this.EndDeferInternal();
				this.IsInEndDefer = false;
			}
		}


		private void EndDeferInternal()
		{
			this.UnwireEvents();
            if (this.CaptionSummaryRow != null)
                this.Refresh();
			if (this.PersistGroupsExpandedState && this.IsGrouping)
			{
				this.expandedGroups = new List<ExpandKey>();
				this.PopulateExpandedGroup(this.TopLevelGroup.Groups, 0);
			}
            //SD17668- Exception is thrown while ungrouping and GridDataControl itemssource is DataTable
            //Need to Refresh the Group before applying sort 
            if (this.IsGrouping)
                this.RefreshTopLevelGroup();
			var hasSort = this.SortDescriptions.Count > 0;
			//if (hasSort)
			{
				this.RefreshSort();
			}

            var hasFilters = this.FilterPredicates.FirstOrDefault(v => v.Filters != null && v.Filters.Count > 0) != null;
            if (hasFilters)
            {
                this.RefreshFilters();
            }
            //else if (this.Filter != null)
            //{
            //    if (!hasSort)
            //        this.Filter = null;
            //}
#if !SILVERLIGHT
            //After the source string set to Datatable, refreshing the group.
            if (this.IsLegacyDataTable)
                this.RefreshTopLevelGroup();
#endif
			if (this.PersistGroupsExpandedState)
			{
				if (this.IsGrouping && this.expandedGroups != null && this.expandedGroups.Count > 0)
				{
					this.SetExpandedGroups(this.TopLevelGroup.Groups, 0);
				}

				if (this.expandedGroups != null)
				{
					this.expandedGroups.Clear();
					this.expandedGroups = null;
				}
			}

            this.Refresh();
			this.UpdateCurrentPosition();
			this.WireEvents();
		}

		private class ExpandKey : IComparable<ExpandKey>
		{
			public IComparable Key { get; set; }
			public int Level { get; set; }

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

		private void SetExpandedGroups(List<Group> groups, int level)
		{
			foreach (var g in groups)
			{
				var found = this.expandedGroups.IndexOf(new ExpandKey() { Key = g.Key as IComparable, Level = level }) >= 0;
			    if (!found) continue;
			    g.IsExpanded = true;
			    if (g.Groups != null)
			    {
			        this.SetExpandedGroups(g.Groups, level + 1);
			    }
			}
		}

		private List<ExpandKey> expandedGroups;

		private void PopulateExpandedGroup(List<Group> groups, int level)
		{
			foreach (Group g in groups)
			{
				if (g.IsExpanded)
				{
					this.expandedGroups.Add(new ExpandKey { Key = g.Key as IComparable, Level = level });
					if (g.Groups != null)
					{
						this.PopulateExpandedGroup(g.Groups, level + 1);
					}
				}
			}
		}

		private void RefreshTopLevelGroup()
		{
			if (this.IsGrouping)
			{
				// when grouping refresh inside TopLevelGroup
				if (this.topLevelGroup != null)
				{
					this.topLevelGroup.CollectionChanged -= OnTopLevelGroupCollectionChanged;
					this.topLevelGroup.Dispose();
					this.topLevelGroup = null;
				}
				// calling this to refresh the TopLevelGroup
				var topLevelGroup = this.TopLevelGroup;
				topLevelGroup.SetDirty();
			}
			else if (!this.IsGrouping && this.topLevelGroup != null)
			{
                this.topLevelGroup.CollectionChanged -= OnTopLevelGroupCollectionChanged;
				this.topLevelGroup.Dispose();
				this.topLevelGroup = null;
				// calling this to refresh the TopLevelGroup
				var topLevelGroup = this.TopLevelGroup;
				if (topLevelGroup != null)
				{
					topLevelGroup.SetDirty();
				}
			}
		}

		/// <summary>
		/// Refreshes the filters.
		/// </summary>
		public virtual void RefreshFilters()
		{
		}

		/// <summary>
		/// Refreshes the sort.
		/// </summary>
		protected virtual void RefreshSort()
		{
		}

		#region IEditableCollectionView Members

		#region Add New

		private bool isItemConstructorValid = false;

		private ConstructorInfo itemConstructor;

		private object newItem = null;

		private object EndAddNew(bool cancel)
		{
			object obj2 = this.newItem;
			this.newItem = null;
			IEditableObject obj3 = obj2 as IEditableObject;
			if (obj3 != null)
			{
				if (cancel)
				{
					obj3.CancelEdit();
				}
				else
				{
					obj3.EndEdit();
				}
			}
#if !SILVERLIGHT
			ISupportInitialize initialize = obj2 as ISupportInitialize;
			if (initialize != null)
			{
				initialize.EndInit();
			}
#endif
			return obj2;
		}

		/// <summary>
		/// Adds a new item to the collection.
		/// </summary>
		/// <returns>
		/// The new item that is added to the collection.
		/// </returns>
		public object AddNew()
		{
			if (!this.CanAddNew)
			{
				return null;
			}

			this.VerifyRefreshNotDeferred();
			IList sourceList = this.GetSourceListCollection();
			if (this.IsEditingItem)
			{
				this.CommitEdit();
			}

			//this.CommitNew();
			if (!this.CanAddNew)
			{
				throw new InvalidOperationException("MemberNotAllowedForView");
			}

            object obj2 = this.itemConstructor != null ? this.itemConstructor.Invoke(null) : null;
			//this.newItemIndex = -2;
			//var newRecord = this.Records.CreateRecordEntry(obj2);
			//this.Records.Add(newRecord);
			//int index = this.Records.IndexOfRecord(newRecord.Data);
			////if (!(recordList is INotifyCollectionChanged))
			//{
			//    if (!object.Equals(obj2, this.Records[index]))
			//    {
			//        index = this.Records.IndexOfRecord(newRecord.Data);
			//    }

			//    this.BeginAddNew(obj2, index);
			//}
#if !SILVERLIGHT
			var initialize = obj2 as ISupportInitialize;
			if (initialize != null)
			{
				initialize.BeginInit();
			}
#endif
			var obj3 = obj2 as IEditableObject;
			if (obj3 != null)
			{
				obj3.BeginEdit();
			}

			this.newItem = obj2;

			return obj2;
		}

	    private void EnsureItemConstructor()
		{
			if (!this.isItemConstructorValid)
			{
			    Type itemType = EnumerableExtensions.GetItemType(this.SourceCollection, true);
				if (itemType != null)
				{
					this.itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
					this.isItemConstructorValid = true;
				}
			}
		}

        public List<RecordEntry> FilteredRecord
        {
            get;
            set;
        }

		/// <summary>
		/// This property used to find out the paging is enabled
		/// </summary>
		public bool EnablePaging
		{
			get;
			set;
		}
		/// <summary>
		/// This property used to find out the Paging is ViewLevel or sourcelevel
		/// </summary>
		public bool IsViewLevelPaging
		{
			get;
			set;
		}
		/// <summary>
		/// This property hold the PagedCollectionView object
		/// </summary>
		public PagedCollectionView PagedSource
		{
			get;
			set;
		}



		/// <summary>
		/// Gets a value that indicates whether a new item can be added to the collection.
		/// </summary>
		/// <value></value>
		/// <returns>true if a new item can be added to the collection; otherwise, false.
		/// </returns>
		public bool CanAddNew
		{
			get
			{
				if (this.GetSourceListCollection() == null)
				{
					return false;
				}

				if (!this.isItemConstructorValid)
				{
					this.EnsureItemConstructor();
				}

				return this.isItemConstructorValid;
			}
		}

		private bool currentElementWasRemoved;

		private int newItemIndex = 0;

		private void BeginAddNew(object newItem, int index)
		{
			this.newItem = newItem;
			this.newItemIndex = index;
			int adjustedNewIndex = -1;
			switch (this.NewItemPlaceholderPosition)
			{
				case NewItemPlaceholderPosition.None:
					adjustedNewIndex = this.UsesLocalArray ? (this.Records.Count - 1) : this.newItemIndex;
					break;

				case NewItemPlaceholderPosition.AtBeginning:
					adjustedNewIndex = 1;
					break;

				case NewItemPlaceholderPosition.AtEnd:
					adjustedNewIndex = this.Records.Count - 2;
					break;
			}

			if (!this.IsGrouping)
			{
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, adjustedNewIndex);
				this.ProcessCollectionChangedWithAdjustedIndex(args, null, -1, adjustedNewIndex);
                var args1 = this.ProcessCollectionChangedAction(args, -1, adjustedNewIndex);
                this.UpdateSourceList(args1, -1, adjustedNewIndex);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:CollectionChanged"/> event.
		/// </summary>
		/// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		public void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
#if !SILVERLIGHT
            if (DispatchOwner != null)
            {
                if (DispatchOwner.Thread != Thread.CurrentThread)
                {
                    try
                    {
                        DispatchOwner.Invoke(
                            new Action(() =>
                            {
                                this.OnCollectionChanged(args);
                            }));
                    }
                    catch
                    {
                    }
                    return;
                }
            }
#endif
			if (this.deferRefreshCount > -1)
			{
				return;
			}

			if (this.CollectionChanged != null)
			{
				if (args.Action != NotifyCollectionChangedAction.Replace)
						this.UpdateTableSummary();		

				if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems[0].Equals(this.CurrentEditItem))
					this.ResetEditItemCache();
					
				if (this.CollectionChanged != null)
					this.CollectionChanged(this, args);
			}

			if ((args.Action != NotifyCollectionChangedAction.Replace)
#if !SILVERLIGHT
 && (args.Action != NotifyCollectionChangedAction.Move)
#endif
)
			{
				this.OnPropertyChanged("Count");
			}

			bool isEmpty = this.IsEmpty;
			if (isEmpty != this.CheckFlag(CollectionViewFlags.CachedIsEmpty))
			{
				this.SetFlag(CollectionViewFlags.CachedIsEmpty, isEmpty);
				this.OnPropertyChanged("IsEmpty");
			}
		}

		#endregion

		/// <summary>
		/// Gets a value that indicates whether the editing of an item can be canceled.
		/// </summary>
		/// <value></value>
		/// <returns>true if editing an item can be canceled; otherwise, false.
		/// </returns>
		public bool CanCancelEdit
		{
			get
			{
				return this.editItem is IEditableObject;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether an item can be removed from the collection.
		/// </summary>
		/// <value></value>
		/// <returns>true if an item can be removed from the collection; otherwise, false.
		/// </returns>
		public bool CanRemove
		{
			get
			{
				return this.GetSourceListCollection() != null;
			}
		}

		private void ImplicitlyCancelEdit()
		{
			IEditableObject obj2 = this.editItem as IEditableObject;
			this.editItem = null;
			if (obj2 != null)
			{
				obj2.CancelEdit();
			}
		}

		/// <summary>
		/// Ends the edit transaction and discards any pending changes to the item.
		/// </summary>
		public void CancelEdit()
		{
			this.VerifyRefreshNotDeferred();
			if (this.editItem != null)
			{
				IEditableObject obj2 = this.editItem as IEditableObject;

#if !SILVERLIGHT
			    var datarow = this.editItem as DataRowView;
			    if (datarow != null)
			    {
			        if (datarow.Row.RowState != DataRowState.Deleted && datarow.Row.RowState != DataRowState.Detached)
                        this.editItemState.Restore(this.editItem);
			    }
			    else
#endif
			        this.editItemState.Restore(this.editItem);
			    this.editItemState = null;
				this.editItem = null;
				//if (obj2 == null)
				//{
				//    throw new InvalidOperationException("CancelEditNotSupported");
				//}
				if (obj2 != null)
				{
					obj2.CancelEdit();
				}
			}
		}

		/// <summary>
		/// Ends the edit transaction and discards the editing item changes
		/// </summary>
		internal void ResetEditItemCache()
		{
			this.editItemState = null;
			this.editItem = null;
		}

		/// <summary>
		/// Ends the add transaction and discards the pending new item.
		/// </summary>
		public void CancelNew()
		{
			if (this.IsEditingItem)
			{
				throw new InvalidOperationException("MemberNotAllowedDuringTransaction");
			}

			if (!this.CanRemove)
			{
				return;
			}

			var sourceList = this.GetSourceListCollection();
			this.VerifyRefreshNotDeferred();
			this.MoveCurrentToFirst();
			if (this.newItem != null)
			{
				this.Records.RemoveAt(this.newItemIndex);
				if (this.newItem != null)
				{
					int adjustedOldIndex = this.AdjustBefore(NotifyCollectionChangedAction.Remove, this.newItem, this.newItemIndex);
					object changedItem = this.EndAddNew(true);
					this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItem, adjustedOldIndex), null, adjustedOldIndex, -1);
				}
			}
		}
		//Merged from 8.2

	    /// <summary>
	    /// Cancels the new item in view.
	    /// </summary>
	    /// <param name="cancelNewItem"></param>
	    /// <param name="allowCancel"></param>
	    internal void CancelNew(bool cancelNewItem, bool allowCancel)
		{
			if (cancelNewItem)
			{
				this.newItem = null;
			}

			if (allowCancel)
			{
				this.CancelNew();
			}
		}

		public void CommitEdit()
		{
			CommitEdit(string.Empty);
		}

		/// <summary>
		/// Ends the edit transaction and saves the pending changes.
		/// </summary>
		public void CommitEdit(string ColumnName)
		{
			if (this.IsInSuspend || this.IsInCommitEdit)
			{
				return;
			}
			this.IsInCommitEdit = true;
			if (this.IsAddingNew)
			{
				throw new InvalidOperationException("MemberNotAllowedDuringTransaction");
			}
			this.VerifyRefreshNotDeferred();
			if (this.editItem != null)
			{
				object item = this.editItem;
                this.currentEditItemState = new ObjectState<object>(this.editItem, this.itemProperties, this.GetMappingNames(), propertyAccessProvider);
				if (this.editItemState.ValidateEdit(item))
				{
					if (this.editItemIndex == -1)
						return;

					var obj3 = this.editItem as IEditableObject;
					if (obj3 != null)
					{
						obj3.EndEdit();
					}

					int num = (this.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;

					int adjustedNewIndex = this.editItemIndex;

					if (this.IsGrouping)
					{
						var record = this.Records[adjustedNewIndex];
						var parentGroup = record.Parent as Group;
						if (parentGroup != null)
						{
							adjustedNewIndex = parentGroup.Records.IndexOf(record);
						}

                        var sortDescription = this.SortDescriptions.FirstOrDefault(s => s.PropertyName == ColumnName);
                        if (this.SortDescriptions.Count > 0 && sortDescription.PropertyName != null)
                        {
                            var resolveIndex = this.editItemIndex - adjustedNewIndex;
                            var newIndex = GetComparerIndexInsideGroup(this.editItem, adjustedNewIndex, ColumnName, parentGroup);
                            newIndex += resolveIndex;
                            if (adjustedNewIndex != newIndex)
                            {
                                if (newIndex >= this.Records.Count)
                                    adjustedNewIndex = this.Records.Count - 1;
                                else if (newIndex < 0)
                                    adjustedNewIndex = 0;
                                else
                                    adjustedNewIndex = newIndex;
                            }
                        }
                        else
                        {
                            adjustedNewIndex =this.editItemIndex;                                                  
                        }
					}

					if (!this.IsGrouping && this.SortDescriptions.Count > 0)
					{
						var newIndex = GetComparerIndex(this.editItem, this.editItemIndex,ColumnName);
					    var hasSort = this.SortDescriptions.Count > 0 &&
					                  this.SortDescriptions.FirstOrDefault(g => g.PropertyName == ColumnName) !=
					                  default(SortDescription);
                        if (!hasSort)
                            newIndex = editItemIndex;
                        if (this.editItemIndex != newIndex)
                        {
                            if (newIndex >= this.Records.Count)
                                adjustedNewIndex = this.Records.Count - 1;
                            else if (newIndex < 0)
                                adjustedNewIndex = 0;
                            else
                                adjustedNewIndex = newIndex;
                        }
					}

				    if (!this.PassesFilter(item))
				    {
				        this.ProcessCollectionChangedWithAdjustedIndex(
				            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, this.editItemIndex),
				            null, this.editItemIndex, -1);
				        this.Refresh();
				    }
				    else if (this.IsGrouping)
				    {
				        var hasGroup = this.GroupDescriptions != null && this.GroupDescriptions.Count > 0 &&
				                        this.GroupDescriptions.OfType<PropertyGroupDescription>()
				                            .FirstOrDefault(g => g.PropertyName == ColumnName) != null;
				        var hasSort = this.SortDescriptions != null && this.SortDescriptions.Count > 0 &&
				                       this.SortDescriptions.FirstOrDefault(s => s.PropertyName.Equals(ColumnName)).PropertyName !=
				                       null;
				        bool AllowChangePosition = hasGroup || hasSort;
				        if ((this.SortingOptions & SortingOptions.DisableSortingOnEdit) == 0 &&
				            (this.SortingOptions & SortingOptions.DisableSortingOnPropertyChange) == 0 && AllowChangePosition)
				        {
				            this.ProcessCollectionChangedWithAdjustedIndex(
				                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, this.editItemIndex),
				                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, adjustedNewIndex),
				                this.editItemIndex, adjustedNewIndex);
				        }

				        //This is for paging support
				        //After Commit the value we have to refresh the pagedsource
				        if (this.PagedSource != null && this.EnablePaging)
				        {
				            this.PagedSource.Refresh();
				        }
				    }
#if !SILVERLIGHT
				    else if (!IsLegacyDataTable)
#else
                    else
#endif
				    {
				        if ((this.SortingOptions & SortingOptions.DisableSortingOnEdit) == 0 &&
				            (this.SortingOptions & SortingOptions.DisableSortingOnPropertyChange) == 0)
				            this.ProcessCollectionChangedWithAdjustedIndex(
				                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, this.editItemIndex),
				                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, adjustedNewIndex),
				                this.editItemIndex, adjustedNewIndex);
				    }

				    this.editItem = null;
					if (this.editItemState != null)
						this.editItemState.Dispose();
					this.editItemState = null;
					if (!this.isEditItemCalled)
					{
						this.MoveCurrentTo(item);
					}
				}
				else
				{
					this.CancelEdit();
				}
			}
			this.IsInCommitEdit = false;
		}

		/// <summary>
		/// Ends the add transaction and saves the pending new item.
		/// </summary>
		public void CommitNew()
		{
			this.IsInSuspend = true;
			if (!this.CanAddNew)
			{
				return;
			}

			var sourceList = this.GetSourceListCollection();
			this.VerifyRefreshNotDeferred();
			if (this.newItem != null)
			{
				if (this.IsGrouping)
				{
					this.CommitNewForGrouping();
				}
				else
				{
					object item = this.EndAddNew(false);
					int newIndex = this.newItemIndex = this.Records.Count;
					if (this.sortDescriptions.Count > 0)
					{
						newIndex = this.GetComparerIndex(item, 0);
						this.newItemIndex = newIndex;
					}
                    if (PassesFilter(item))
                        this.Records.Insert(this.newItemIndex, this.Records.CreateRecordEntry(item));
					sourceList.Insert(this.newItemIndex, item);
					int oldRecordIndex = this.Records.IndexOfRecord(item);
					if (!this.IsGrouping && PassesFilter(item))
					{
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.newItemIndex));
					}
					//if (oldRecordIndex != newIndex && this.sortDescriptions.Count > 0)
					//{
					//    this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, oldRecordIndex), new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.newItemIndex), oldRecordIndex, this.newItemIndex);
					//}
					this.SetCurrent(item, newItemIndex);
				}
			}

			this.IsInSuspend = false;
		}

        private void CommitNewForGrouping()
        {
            int index = this.newItemIndex;
            object item = this.EndAddNew(false);
            this.GroupList.Add(item, true);
            this.GetSourceListCollection().Insert(index, item);
            // this.ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

		/// <summary>
		/// Gets the item that is being added during the current add transaction.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The item that is being added if <see cref="P:System.ComponentModel.IEditableCollectionView.IsAddingNew"/> is true; otherwise, null.
		/// </returns>
		public object CurrentAddItem
		{
			get
			{
				return this.newItem;
			}
            set
            {
                this.newItem = value;
            }
		}

		/// <summary>
		/// Gets the item in the collection that is being edited.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The item in the collection that is being edited if <see cref="P:System.ComponentModel.IEditableCollectionView.IsEditingItem"/> is true; otherwise, null.
		/// </returns>
		public object CurrentEditItem
		{
			get
			{
				return this.editItem;
			}
		}

		private object editItem = null;

		private int editItemIndex = -1;

		ObjectState<object> editItemState;
        ObjectState<object> currentEditItemState;
        public IEditItemState CurrentEditItemState
        {
            get
            {
                return this.currentEditItemState;
            }
        }
		/// <summary>
		/// Gets the state of the edit item.
		/// </summary>
		/// <value>The state of the edit item.</value>
		public IEditItemState EditItemState
		{
			get
			{
				return this.editItemState;
			}
		}

		private bool isEditItemCalled = false;
		/// <summary>
		/// Begins an edit transaction of the specified item.
		/// </summary>
		/// <param name="item">The item to edit.</param>
		public void EditItem(object item)
		{
			this.isEditItemCalled = true;
			this.VerifyRefreshNotDeferred();
			if (item == this.NewItemPlaceholder)
			{
				throw new ArgumentException("CannotEditPlaceholder");
			}
			//Commented due to V8.2

			//if (this.IsAddingNew)
			//{
			//    if (object.Equals(item, this.newItem))
			//    {
			//        return;
			//    }

			//    this.CommitNew();
			//}
			//-------



			if (editItemIndex > 0)
				this.CommitEdit();

			this.editItem = item;

			this.editItemState = new ObjectState<object>(item, this.itemProperties, this.GetMappingNames(), propertyAccessProvider);
			//this.editItemState = new ObjectState<object>(item, this.itemProperties, this.GetMappingNames());


			this.editItemIndex = this.Records.IndexOfRecord(item);
			IEditableObject obj2 = item as IEditableObject;
			if (obj2 != null)
			{
				obj2.BeginEdit();
			}
			this.isEditItemCalled = false;
		  
		}

		protected virtual IEnumerable<String> GetMappingNames()
		{
			return null;
		}

		internal IEnumerable<string> MappingNames
		{
			get { return this.GetMappingNames(); }
		}

		/// <summary>
		/// Gets a value that indicates whether an add transaction is in progress.
		/// </summary>
		/// <value></value>
		/// <returns>true if an add transaction is in progress; otherwise, false.
		/// </returns>
		public bool IsAddingNew
		{
			get
			{
				return this.newItem != null;
			}
		}

		internal bool IsInCommitEdit
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or Sets when Items property changed
		/// </summary>
		internal bool IsInPropertyChange
		{
			get;
			set;
		}
		private SortingOptions sortingOptions;
		/// <summary>
		/// Gets or Sets when Sorting is Enabled or Disabled. 
		/// </summary>
		internal SortingOptions SortingOptions
		{
			get
			{
				return this.sortingOptions;
			}
			set
			{
				this.sortingOptions = value;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether an edit transaction is in progress.
		/// </summary>
		/// <value></value>
		/// <returns>true if an edit transaction is in progress; otherwise, false.
		/// </returns>
		public bool IsEditingItem
		{
			get
			{
				return this.editItem != null;
			}
		}

		private NewItemPlaceholderPosition newItemPlaceholder;

		/// <summary>
		/// Gets or sets the position of the new item placeholder in the collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// One of the enumeration values that specifies the position of the new item placeholder in the collection.
		/// </returns>
		public NewItemPlaceholderPosition NewItemPlaceholderPosition
		{
			get
			{
				return this.newItemPlaceholder;
			}

			set
			{
				this.newItemPlaceholder = value;
			}
		}

		/// <summary>
		/// Removes the specified item from the collection.
		/// </summary>
        /// <param name="record">The record to remove.</param>
        public void Remove(object record)
        {
            var sourceList = this.GetSourceListCollection();
            var index = sourceList.IndexOf(record);
            if (index >= 0)
                this.RemoveAt(index);
        }

		/// <summary>
		/// Removes the item at the specified position from the collection.
		/// </summary>
		/// <param name="index">The position of the item to remove.</param>
		public void RemoveAt(int index)
		{
			this.VerifyRefreshNotDeferred();
            var sourceList = this.GetSourceListCollection();
            if (sourceList[index] == this.NewItemPlaceholder)
			{
				throw new InvalidOperationException("RemovingPlaceholder");
			}
#if !SILVERLIGHT
            if (!(this.SourceCollection is INotifyCollectionChanged) && !(this.SourceCollection is IBindingList))
#else
            if (!(this.SourceCollection is INotifyCollectionChanged))
#endif
            {
                IsInSourceCollectionChange = true;
                if (IsGrouping)
                {
                    GroupList.Remove(sourceList[index], true);
                }
                else
                {
                    var itemAt = Records.IndexOfRecord(sourceList[index]);
                    Records.RemoveAt(itemAt);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                                                                  sourceList[index], itemAt));
                }
                IsInSourceCollectionChange = false;
            }
            sourceList.RemoveAt(index);
		}

		#endregion

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Raises the <see cref="E:PropertyChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
#if !SILVERLIGHT
                if (DispatchOwner != null)
                {
                    if (DispatchOwner.Thread != Thread.CurrentThread)
                    {
                        //DispatchOwner.BeginInvoke(new PropertyChangedEventHandler(OnPropertyChanged), e);
                        DispatchOwner.Invoke(
                            new Action(() =>
                            {
                                OnPropertyChanged(e);
                            }));
                        return;
                    }
                }
#endif
                this.PropertyChanged(this, e);

            }
		}

		#endregion

#if !SILVERLIGHT
		internal void UpdateTableSummary(object record, string propertyName)
		{
			if (IsSuspend)
			{
				return;
			}
			if (this.TableSummaryRows.Count == 0)
			{
				return;
			}

			if (_changedInfo.Count == 0)
				return;

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
							if (aggregator is ISummaryAdjustible)
							{
								var adjustible = aggregator as ISummaryAdjustible;
								if (summaryValue!=null)                                foreach (var key in summaryValue.AggregateValues.Keys.ToArray())
								{
									var value = summaryValue.AggregateValues[key];
									adjustible.SetCurrentValue(value, key);
									// old value
									object oldValue = null;
									this._changedInfo.TryGetValue(propertyName, out oldValue);
									adjustible.AdjustForOldContribution(oldValue, key);
									// new value
									var newValue = this.propertyAccessProvider.GetValue(record, propertyName);
									adjustible.AdjustForNewContribution(newValue, key);

									// get the current value
									var aggregateValue = adjustible.GetCurrentValue(key);
									summaryValue.AggregateValues[key] = aggregateValue;
									this._changedInfo.Remove(key);
								}
							}
							//Custom summaries will be refreshed here
							else if (aggregator is ISummaryAggregate)
							{
								Dictionary<string, object> summaryItems = new Dictionary<string, object>();
                                IEnumerable items = this.Records.Select(o => o.Data).ToArray().OfQueryable(this.SourceType);
								SummaryCreator.RaiseQuerySummaryAggregate(items, summaryColumn, summaryItems, this);
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
					}
				}
                counter0++;
			}
		}

		internal void UpdateSummaries(Group group, object record, string propertyName)
		{
			if (IsSuspend)
			{
				return;
			}
			if (this.SummaryRows.Count == 0)
			{
				return;
			}

            if (_changedSummaryInfo.Count == 0)
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
				    var summaryRecordEntry = groupRecordEntry.Summaries[counter0];
				    foreach (var summaryColumn in summaryRow.SummaryColumns)
					{
                        if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName) && summaryColumn.MappingName.Equals(propertyName))
                        {
                            var summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                            var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
                            if (aggregator is ISummaryAdjustible)
                            {
                                var adjustible = aggregator as ISummaryAdjustible;
                                if (summaryValue != null)
                                    foreach (var key in summaryValue.AggregateValues.Keys.ToArray())
                                    {
                                        var value = summaryValue.AggregateValues[key];
                                        adjustible.SetCurrentValue(value, key);
                                        // old value
                                        object oldValue = null;
                                        this._changedSummaryInfo.TryGetValue(propertyName, out oldValue);
                                        adjustible.AdjustForOldContribution(oldValue, key);
                                        // new value
                                        var newValue = this.propertyAccessProvider.GetValue(record, propertyName);
                                        adjustible.AdjustForNewContribution(newValue, key);

                                        // get the current value
                                        var aggregateValue = adjustible.GetCurrentValue(key);
                                        summaryValue.AggregateValues[key] = aggregateValue;
                                        this._changedSummaryInfo.Remove(key);
                                    }
                            }
                        }
					}
				}
			    counter0++;
			}
		}

		internal void UpdateCaptionSummaries(Group group, object record, string propertyName)
		{
			if (IsSuspend)
				return;
			if (_changedCaptionSummaryInfo.Count == 0)
				return;
			if (this.CaptionSummaryRow == null)
				return;
			
			if (group.IsBottomLevel)
			{
				do
				{
					var summaryRecordEntry = group.SummaryDetails;//new SummaryRecordEntry(group, group.Level);
					//summaryRecordEntry.SummaryRow = summaryRow;
					foreach (var summaryColumn in captionSummaryRow.SummaryColumns)
					{
                        if (!string.IsNullOrEmpty(summaryColumn.Name) && !string.IsNullOrEmpty(summaryColumn.MappingName) && summaryColumn.MappingName.Equals(propertyName))
						{
							SummaryValue summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
							var aggregator = SummaryCreator.GetSummaryAggregate(summaryColumn, this);
							if (aggregator is ISummaryAdjustible)
							{
								var adjustible = aggregator as ISummaryAdjustible;
								foreach (var key in summaryValue.AggregateValues.Keys.ToArray())
								{
									var value = summaryValue.AggregateValues[key];
									adjustible.SetCurrentValue(value, key);
									// old value
									object oldValue = null;
									this._changedCaptionSummaryInfo.TryGetValue(propertyName, out oldValue);
									adjustible.AdjustForOldContribution(oldValue, key);
									// new value
									var newValue = this.propertyAccessProvider.GetValue(record, propertyName);
									adjustible.AdjustForNewContribution(newValue, key);

									// get the current value
									var aggregateValue = adjustible.GetCurrentValue(key);
									summaryValue.AggregateValues[key] = aggregateValue;
									//this.changedCaptionSummaryInfo.Remove(key);
								}
							}
						}
					}
					group.SummaryDetails = summaryRecordEntry;
					group = group.Parent;
				} while (group.Parent != null);
			}
		}

#endif

		internal void UpdateTableSummary()
		{
			if (IsSuspend)
			{
				return;
			}
			if (this.TableSummaryRows == null || this.TableSummaryRows.Count == 0)
			{
				return;
			}

			var counter0 = 0;
			foreach (var summaryRow in this.TableSummaryRows)
			{
				SummaryRecordEntry summaryRecordEntry = null;
				if (counter0 < this.Records.TableSummaries.Count)
				{
					summaryRecordEntry = this.Records.TableSummaries[counter0];
				}
				else
				{
					summaryRecordEntry = new SummaryRecordEntry(null, -2) { SummaryRow = summaryRow };
					this.Records.TableSummaries.Add(summaryRecordEntry);
				}

                IEnumerable items = this.Records.Select(o => o.Data).ToArray().OfQueryable(this.SourceType);
				foreach (var summaryColumn in summaryRow.SummaryColumns)
				{
					if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
					{
						Dictionary<string, object> summaryItems = new Dictionary<string, object>();
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
				counter0++;
			}
		}

		internal bool IsSuspend = false;

		public void Suspend()
		{
			IsSuspend = true;
		}

		public void Resume()
		{
			if (IsSuspend)
			{
				IsSuspend = false;
				if (this.Records.Count > 0)
				{
					UpdateTableSummary();
					if (this.TopLevelGroup != null)
					{
						foreach (var group in this.TopLevelGroup.Groups)
						{
							this.TopLevelGroup.UpdateSummaries(group);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether [uses local array].
		/// </summary>
		/// <value><c>true</c> if [uses local array]; otherwise, <c>false</c>.</value>
		protected bool UsesLocalArray
		{
			get
			{
				var activeComparer = this.GetActiveComparer();
				if (activeComparer == null)
				{
                    return this.CanFilter;
				}

				return true;
			}
		}

		private bool persistGroupsExpandedState = false;
		/// <summary>
		/// Gets / sets
		/// </summary>
		public bool PersistGroupsExpandedState
		{
			get { return this.persistGroupsExpandedState; }
			set
			{
				this.persistGroupsExpandedState = value;
			}
		}

        private bool isGroupsExpanded = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is groups expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is groups expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsGroupsExpanded
        {
            get { return isGroupsExpanded; }
            set { this.isGroupsExpanded = value; }
        }

		private IComparer<object> activeComparer;
		protected IComparer<object> GetActiveComparer()
		{
			return this.activeComparer;
		}

		#region ICollectionViewAdv Members

		private ISummaryRow captionSummaryRow;

		/// <summary>
		/// Gets or sets the caption summary row.
		/// </summary>
		/// <value>The caption summary row.</value>
		public ISummaryRow CaptionSummaryRow
		{
			get
			{
				return this.captionSummaryRow;
			}

			set
			{
				this.captionSummaryRow = value;
			}
		}

		private ObservableCollection<ISummaryRow> tableSummaryRows;

		/// <summary>
		/// Gets the table summary rows.
		/// </summary>
		/// <value>The table summary rows.</value>
		public ObservableCollection<ISummaryRow> TableSummaryRows
		{
			get
			{
				return this.tableSummaryRows;
			}
		}

		private ObservableCollection<ISummaryRow> summaryRows;

		/// <summary>
		/// Gets the summary rows.
		/// </summary>
		/// <value>The summary rows.</value>
		public ObservableCollection<ISummaryRow> SummaryRows
		{
			get
			{
				return this.summaryRows;
			}
		}

		public virtual Func<string, object, object> GetFunc(string propertyName)
		{
			if (this.customExpressionFunc != null)
			{
				var func = this.customExpressionFunc.GetFunc(propertyName);
				if (func != null)
				{
					return func;
				}
			}

#if !SILVERLIGHT
			if (this.isITypedListSource)
			{
				return this.GetITypedListFunc(propertyName);
			}

			if (this.HasICustomTypeDescriptor)
			{
				return this.GetCustomTypeDescriptorFunc(propertyName);
			}

			if (this.HasCustomTypeDescriptionProvider)
			{
				return this.GetCustomTypeDescriptionProviderFunc(propertyName);
			}
#endif

			return null;
		}

        public virtual Func<string, object, object> GetTypeFunc(string propertyName)
        {
            if (this.customExpressionFunc != null)
            {
                var func = this.customExpressionFunc.GetTypeFunc(propertyName);
                if (func != null)
                {
                    return func;
                }
            }

#if !SILVERLIGHT
            if (this.isITypedListSource)
            {
                return this.GetITypedListTypeFunc(propertyName);
            }

            if (this.HasICustomTypeDescriptor)
            {
                return this.GetCustomTypeDescriptorTypeFunc(propertyName);
            }

            if (this.HasCustomTypeDescriptionProvider)
            {
                return this.GetCustomTypeDescriptionProviderTypeFunc(propertyName);
            }
#endif

            return null;
        }

		/// <summary>
		/// Gets the expression func for complex data sources.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		public virtual Expression<Func<string, object, object>> GetExpressionFunc(string propertyName)
		{
			if (this.customExpressionFunc != null)
			{
				var func = this.customExpressionFunc.GetExpressionFunc(propertyName);
				if (func != null)
				{
					return func;
				}
			}

#if !SILVERLIGHT
			if (this.isITypedListSource)
			{
				return this.GetITypedListExpressionFunc(propertyName);
			}

			if (this.HasICustomTypeDescriptor)
			{
				return this.GetCustomTypeDescriptorExpressionFunc(propertyName);
			}

			if (this.HasCustomTypeDescriptionProvider)
			{
				return this.GetCustomTypeDescriptionProviderExpressionFunc(propertyName);
			}
#endif

			return null;
		}

        public virtual Expression<Func<string, object, object>> GetTypeExpressionFunc(string propertyName)
        {
            if (this.customExpressionFunc != null)
            {
                var func = this.customExpressionFunc.GetTypeExpressionFunc(propertyName);
                if (func != null)
                {
                    return func;
                }
            }

#if !SILVERLIGHT
            if (this.isITypedListSource)
            {
                return this.GetITypedListTypeExpressionFunc(propertyName);
            }

            if (this.HasICustomTypeDescriptor)
            {
                return this.GetCustomTypeDescriptorTypeExpressionFunc(propertyName);
            }

            if (this.HasCustomTypeDescriptionProvider)
            {
                return this.GetCustomTypeDescriptionProviderTypeExpressionFunc(propertyName);
            }
#endif

            return null;
        }


		private IUnboundExpressionFunc customExpressionFunc;
		public void SetCustomExpressionFunc(IUnboundExpressionFunc customExpressionFunc)
		{
			this.customExpressionFunc = customExpressionFunc;
		}

		private ObservableCollection<IFilterDefinition> filters;

		/// <summary>
		/// Gets or sets the filter predicates.
		/// </summary>
		/// <value>The filter predicates.</value>
		public ObservableCollection<IFilterDefinition> FilterPredicates
		{
			get
			{
				return this.filters;
			}
			set
			{
				if (this.filters != value)
				{
					this.filters = value;
					this.ApplyFilters();
				}
			}
		}

		private void ApplyFilters()
		{
			if (this.deferRefreshCount > -1 || this.FilterPredicates == null)
			{
				return;
			}
			this.RefreshFilters();
			this.UpdateTableSummary();
		}

		#endregion

		private ObservableCollection<IRelationDefinition> relations = null;
		/// <summary>
		/// Gets the relations.
		/// </summary>
		/// <value>The relations.</value>
		public ObservableCollection<IRelationDefinition> Relations
		{
			get { return this.relations; }
		}

#if SyncfusionFramework4_0
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
				if (!this.isDynamicSourceEvaluated)
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
						{
							this.isDynamicBound = false;
						}
					}
					this.isDynamicSourceEvaluated = true;
				}

				return this.isDynamicBound;
			}
		}

#endif

#if !SILVERLIGHT
		bool isXMLBound = false;
		public bool IsXMLBound
		{
			get
			{
				var enumerator = this.SourceCollection.GetEnumerator();
				if (!enumerator.MoveNext())
				{
					return false;
				}

				var record = enumerator.Current;
				if (record != null)
				{
					this.isXMLBound = record is XmlElement;
				}
				return isXMLBound;
			}
		}
#endif

		bool isInterfaceBound = false;
		public bool IsInterfaceBound
		{
			get
			{
				if (this.SourceType != null)
					if (SourceType.IsInterface)
						isInterfaceBound = true;
//                var enumerator = this.SourceCollection.GetEnumerator();
//                if (!enumerator.MoveNext())
//                    return false;

//                var record = enumerator.Current;
//                if (record != null)
//                {
//                    var interfaces = record.GetType().GetInterfaces();
//                    int count = 0;
//                    foreach (var interfaceName in interfaces)
//                    {
//                        if ((interfaceName.GetType() is INotifyPropertyChanged
//                            || interfaceName.GetType() is IDisposable
//#if !SILVERLIGHT
// || interfaceName.GetType() is ICustomTypeDescriptor
//#endif
//))
//                            isInterfaceBound = false;
//                        else
//                            count++;
//                    }

//                    if (count > 0)
//                        isInterfaceBound = true;
//                }
				return isInterfaceBound;
			}
		}

#if !SILVERLIGHT
		/// <summary>
		/// Gets the item properties.
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptorCollection GetItemProperties()
		{
			if (this.itemProperties == null)
			{
				this.SetItemProperties(this.SourceCollection);
			}

			return this.ItemProperties;
		}
#else
		/// <summary>
		/// Gets the item properties.
		/// </summary>
		/// <returns></returns>
		public virtual PropertyInfoCollection GetItemProperties()
		{
			return this.ItemProperties;
		}
#endif

		#region ISupportInitialize Members

		private IDisposable endDeferDisposable;

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
			this.endDeferDisposable = this.DeferRefresh();
		}

		/// <summary>
		/// Signals the object that initialization is complete.
		/// </summary>
		public void EndInit()
		{
			if (this.endDeferDisposable != null)
			{
				this.endDeferDisposable.Dispose();
			}
		}

		#endregion
#if SILVERLIGHT
		class ObjectComparer : IComparer<object>
		{
		#region IComparer<object> Members

			public int Compare(object x, object y)
			{
				int cmp = 0;
				bool xIsNull = (x == null);
				bool yIsNull = (y == null);

				if (yIsNull && xIsNull)
				{
					cmp = 0;
				}
				else if (xIsNull)
				{
					cmp = -1;
				}
				else if (yIsNull)
				{
					cmp = 1;
				}
				else
				{
					if (x is IComparable && y is IComparable)
					{
						cmp = ((IComparable)x).CompareTo(y);
					}
					else
					{
						cmp = x.Equals(y) == true ? 0 : 1;
					}
				}

				return cmp;
			}

		#endregion
		}
#endif

		internal IPropertyAccessProvider propertyAccessProvider;
		/// <summary>
		/// Gets the property access provider.
		/// </summary>
		/// <returns></returns>
		public IPropertyAccessProvider GetPropertyAccessProvider()
		{
			return this.propertyAccessProvider;
		}

		/// <summary>
		/// Creates the record entry.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public virtual RecordEntry CreateRecordEntry(object data)
		{
			return null;
		}

		/// <summary>
		/// Defines the custom group comparer. This will work only when the grouped column is also sorted.
		/// </summary>
		/// <value></value>
		public IComparer<Group> GroupComparer
		{
			get;
			set;
		}

		//void IDisposable.Dispose()
		//{
		//    Dispose();
		//}
	}

	/// <summary>
	/// Specifies a state while editing items
	/// </summary>
	public interface IEditItemState
	{
		object this[string property] { get; }
	}

	internal class ObjectState<T> : IDisposable, IEditItemState
	{
		private Dictionary<string, object> storedValues;

		private IEnumerable<String> mappingNames;


#if !SILVERLIGHT
		private PropertyDescriptorCollection itemProperties;
#else
		private PropertyInfoCollection itemProperties;
#endif
		private IPropertyAccessProvider propertyAccessProvider;

#if !SILVERLIGHT
		public ObjectState(T target, PropertyDescriptorCollection itemProperties, IEnumerable<string> MappingNames, IPropertyAccessProvider propertyAccessProvider)
#else
		public ObjectState(T target, PropertyInfoCollection itemProperties, IEnumerable<string> MappingNames, IPropertyAccessProvider propertyAccessProvider)
#endif
		{
			this.propertyAccessProvider = propertyAccessProvider;
			this.Target = target;
			this.storedValues = new Dictionary<string, object>();
			this.itemProperties = itemProperties;
			this.mappingNames = MappingNames;
			this.storedValues = this.BuildUp(this.itemProperties, this.Target);
		}

		public T Target
		{
			get;
			private set;
		}

		public object this[string property]
		{
			get
			{
				if (this.storedValues.ContainsKey(property))
				{
					return this.storedValues[property];
				}

				return null;
			}
		}


#if !SILVERLIGHT
		private int SelfReferenceCurrentCount = 0;
		private const int SelfReferenceMaxCount = 7;
		private Dictionary<string, object> BuildUp(PropertyDescriptorCollection itemProperties, object target)
		{
			var dict = new Dictionary<string, object>();
			if (this.mappingNames != null)
			{
				foreach (var mappingName in this.mappingNames)
				{
					var pd = itemProperties.GetPropertyDescriptor(mappingName);

					if (pd != null)
					{
						if (!pd.IsReadOnly && !NullableHelperInternal.IsIEnumerableType(pd) && !dict.ContainsKey(mappingName.ToString()))
						{
							//dict.Add(mappingName, itemProperties.GetValue(target, mappingName));
							dict.Add(mappingName, propertyAccessProvider.GetValue(target, mappingName));
						}
					}
					else
					{
						if (!dict.ContainsKey(mappingName.ToString()))
						{
							//dict.Add(mappingName, itemProperties.GetValue(target, mappingName));
							dict.Add(mappingName, propertyAccessProvider.GetValue(target, mappingName));
						}
					}
				}
			}
			else
			{
				foreach (PropertyDescriptor pd in itemProperties)
				{
					if (!pd.IsReadOnly && !NullableHelperInternal.IsIEnumerableType(pd))
					{
						if (NullableHelperInternal.IsComplexType(pd.PropertyType))
						{
							var tempProperyDescriptor = itemProperties.GetPropertyDescriptor(pd.Name);
							if (tempProperyDescriptor != null)
							{
								object tRecord = tempProperyDescriptor.GetValue(target);
								if (this.SelfReferenceCurrentCount < SelfReferenceMaxCount)
								{
									this.SelfReferenceCurrentCount++;
									dict.Add(pd.Name, this.BuildUp(TypeDescriptor.GetProperties(pd.PropertyType), tRecord));
								}
							}

						}
						else
						{
							dict.Add(pd.Name, pd.GetValue(target));
						}
					}
				}
			}
#else
		private Dictionary<string, object> BuildUp(PropertyInfoCollection itemProperties, object target)
		{
			var dict = new Dictionary<string, object>();
			if (this.mappingNames != null)
			{
				foreach (var mappingName in this.mappingNames)
				{
					//var propList = mappingName.Split('.').ToList<string>();
					//this.PopulateProperties(propList, dict, itemProperties.GetValue(target, mappingName));
					
					var pd = itemProperties.GetPropertyDescriptor(mappingName);

					if (pd != null)
					{
						if (pd.CanWrite && !NullableHelperInternal.IsIEnumerableType(pd))
							dict.Add(mappingName, propertyAccessProvider.GetValue(target, mappingName));
					}
					else
						dict.Add(mappingName, propertyAccessProvider.GetValue(target, mappingName));
						//dict.Add(mappingName, itemProperties.GetValue(target, mappingName));
				}
			}
			else
			{
				foreach (var pInfo in itemProperties)
				{
					if (target == null)
					{
						dict.Add(pInfo.Value.Name, null);
						continue;
					}

					if (pInfo.Value.CanWrite && !NullableHelperInternal.IsIEnumerableType(pInfo.Value))
					{
						if (NullableHelperInternal.IsComplexType(pInfo.Value.PropertyType))
						{
							var tempProperyDescriptor = itemProperties.Find(pInfo.Value.Name, true);
							if (tempProperyDescriptor != null)
							{
								//object tRecord = tempProperyDescriptor.GetValue(target);
								object tRecord = propertyAccessProvider.GetValue(target,pInfo.Value.Name);
								var infoCollection = new PropertyInfoCollection(pInfo.Value.PropertyType);
								dict.Add(pInfo.Value.Name, this.BuildUp(infoCollection, tRecord));
							}
						}
						else
						{
							//dict.Add(pInfo.Value.Name, pInfo.Value.GetValue(target));
							dict.Add(pInfo.Value.Name, propertyAccessProvider.GetValue(target,pInfo.Value.Name));
						}

						//var arg2 = pInfo.Value.GetValue(target);
						//this.storedValues.Add(pInfo.Key.ToString(), arg2);
					}
				}
			}
#endif
			return dict;
		}

		public void Restore(T Originator)
		{
			if (this.mappingNames == null)
			{
				this.Restore(Originator, this.itemProperties, this.storedValues);
			}
			else
			{
				this.Restore(Originator, this.mappingNames);
			}
		}

#if !SILVERLIGHT
		private void Restore(T orginator, PropertyDescriptorCollection pdc, Dictionary<string, object> dict)
		{
			foreach (PropertyDescriptor pd in pdc)
			{
				if (dict.ContainsKey(pd.Name))
				{
					var value = dict[pd.Name];
					if (value is IDictionary)
					{
						var tempPropertyDescriptor = pdc.Find(pd.Name, true);
						object tRecord = tempPropertyDescriptor.GetValue(orginator);
						this.Restore((T)tRecord,
							TypeDescriptor.GetProperties(tRecord),
							(Dictionary<string, object>)value);
					}
					else
					{
						pd.SetValue(orginator, value);
					}
				}
			}
		}
#else
		private void Restore(T orginator, PropertyInfoCollection pdc, Dictionary<string, object> dict)
		{
			foreach (var pInfo in pdc)
			{
				if (dict.ContainsKey(pInfo.Value.Name))
				{
					var value = dict[pInfo.Value.Name];
					if (value is IDictionary)
					{
						var tempPropertyDescriptor = pdc.Find(pInfo.Value.Name, true);
						object tRecord = tempPropertyDescriptor.GetValue(orginator);
						this.Restore((T)tRecord,
							new PropertyInfoCollection(tRecord.GetType()),
							(Dictionary<string, object>)value);
					}
					else
					{
						pInfo.Value.SetValue(orginator, value);
					}
				}
			}
		}
#endif

		private void Restore(T Originator, IEnumerable<string> mappingNames)
		{
			foreach (var mappingName in this.mappingNames)
			{
				if (this.storedValues.ContainsKey(mappingName))
				{

#if !SILVERLIGHT
					propertyAccessProvider.SetValue(Originator, mappingName, this.storedValues[mappingName]);
#else
					itemProperties.SetValue(Originator, this.storedValues[mappingName], mappingName);
#endif
				}
			}
		}



		public bool ValidateEdit(object editItem)
		{
			if (editItem == null)
			{
				return false;
			}

			if (this.mappingNames != null)
			{
				foreach (var mappingName in this.mappingNames)
				{
					var pd = itemProperties.GetPropertyDescriptor(mappingName);

					if (pd != null)
					{
						if (NullableHelperInternal.IsIEnumerableType(pd))
							continue;
					}
#if !SILVERLIGHT
					var arg1 = propertyAccessProvider.GetValue(editItem, mappingName);
#else
					var arg1 = itemProperties.GetValue(editItem, mappingName);
#endif
					var arg2 = this[mappingName];

					bool arg3;
					if (pd != null)
						arg3 = NullableHelperInternal.IsNullableType(pd.PropertyType);
					else
						arg3 = false;
					if (this.Check(arg1, arg2, arg3))
					{
						return true;
					}
				}

				return false;
			}


#if !SILVERLIGHT
			foreach (PropertyDescriptor pd in itemProperties)
			{
				if (NullableHelperInternal.IsIEnumerableType(pd))
				{
					continue;
				}

				if (this.Check(pd.GetValue(editItem),
					this[pd.Name],
					NullableHelperInternal.IsNullableType(pd.PropertyType)))
				{
					return true;
				}
			}
#else
			foreach (KeyValuePair<string, PropertyInfo> pd in itemProperties)
			{
				if (NullableHelperInternal.IsIEnumerableType(pd.Value))
				{
					continue;
				}

				if (this.Check(pd.Value.GetValue(editItem),
					this[pd.Value.Name],
					NullableHelperInternal.IsNullableType(pd.Value.PropertyType)))
				{
					return true;
				}
			}
#endif

			return false;
		}



		private bool Check(object value, object editItemValue, bool isNullable)
		{
			if (editItemValue is IDictionary && value != null)
			{
#if !SILVERLIGHT
				foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(value))
				{

					if (NullableHelperInternal.IsIEnumerableType(pd))
					{
						continue;
					}

					var arg1 = pd.GetValue(value);
					var arg2 = ((IDictionary)editItemValue)[pd.Name];
					var arg3 = NullableHelperInternal.IsNullableType(pd.PropertyType);

					if (this.Check(arg1, arg2, arg3))
					{
						return true;
					}

				}
#else
				foreach (var pInfo in new PropertyInfoCollection(value.GetType()))
				{
					if (NullableHelperInternal.IsIEnumerableType(pInfo.Value))
					{
						continue;
					}

					var arg1 = pInfo.Value.GetValue(value);
					var arg2 = ((IDictionary)editItemValue)[pInfo.Value.Name];
					var arg3 = NullableHelperInternal.IsNullableType(pInfo.Value.PropertyType);

					if (this.Check(arg1, arg2, arg3))
					{
						return true;
					}
				}
#endif
			}

			if (value != null && editItemValue != null)
			{
				//if (string.Compare(value.ToString(), editItemValue.ToString()) != 0)
				if(!value.Equals(editItemValue))//This should compare object value also.
				{
					// value has been edited
					return true;
				}
			}
			else if (value != null && editItemValue == null)
			{
				return true;
			}

			else if (isNullable)
			{
				if (value == null && editItemValue != null)
				{
					return true;
				}
				else if (value != null && editItemValue == null)
				{
					return true;
				}
			}

			return false;
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.storedValues.Clear();
			this.mappingNames = null;
			this.Target = default(T);
		}

		#endregion
	}

    [Flags]
    public enum SortingOptions
	{
		/// <summary>
		/// Sorting Enabled always
		/// </summary>
		Default = 0x00,
		/// <summary>
		/// Sorting Disabled when cell value is edited
		/// </summary>
		DisableSortingOnEdit = 0x01,
		/// <summary>
		/// Sorting Disabled when cell property value changed
		/// </summary>
		DisableSortingOnPropertyChange = 0x02
	}

#if !SILVERLIGHT
	public class XMLAttributesProvider : IPropertyAccessProvider
	{
		private ICollectionViewAdv view;

		public XMLAttributesProvider(ICollectionViewAdv view)
		{
			this.view = view;
		}

		public virtual object GetValue(object record, string propName)
		{
			object result = null;
			if (record is XmlElement)
			{
				result = ((XmlElement)record).GetAttribute(propName);
			}

			return result;
		}


		public virtual bool SetValue(object record, string propName, object value)
		{
			if (record is XmlElement)
			{
				((XmlElement)record).SetAttribute(propName, value.ToString());
				return true;
			}

			return false;
		}

	}
#endif
}
