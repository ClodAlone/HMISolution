#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Data;
    using Syncfusion.Linq;
    using Syncfusion.Linq.Data;
    using System.Data;

    /// <summary>
    /// Extended CollectionViewAdv to handle DataTable / DataView sources.
    /// </summary>
    public class DataTableCollectionView : CollectionViewAdv
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableCollectionView"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public DataTableCollectionView(IEnumerable source)
            : base(source)
        {
            this.SetSourceList(source);
            this.EnsureSourceList();
        }

        public override bool PassesFilter(object record)
        {
            return true;
        }
        private void SetSourceList(IEnumerable source)
        {
            DataView dv = null;
            if (source is DataTable)
            {
                var table = source as DataTable;
                dv = table.AsDataView();
            }
            else if (source is DataView)
            {
                dv = source as DataView;
            }
            else
            {
                throw new InvalidOperationException("Cannot find DataTable / DataView source list");
            }
            // dv.ListChanged += new ListChangedEventHandler(OnListChanged);
            this.ViewSource = dv;
        }

        /*void OnListChanged(object sender, ListChangedEventArgs e)
        {
            if (this.IsInSuspend)
            {
                return;
            }

            if (!this.IsGrouping)
            {
                //NotifyCollectionChangedEventArgs arg = null;
                //DataView source = null;
                //int newIdx = e.NewIndex;
                //DataRowView newItem = null;
                //if (e.ListChangedType != ListChangedType.Reset && e.ListChangedType != ListChangedType.PropertyDescriptorAdded && e.ListChangedType != ListChangedType.PropertyDescriptorChanged && e.ListChangedType != ListChangedType.PropertyDescriptorDeleted)
                //{
                //    source = sender as DataView;
                //    if (e.ListChangedType != ListChangedType.ItemDeleted)
                //    {
                //        newItem = source[e.NewIndex] as DataRowView;
                //    }
                //}

                NotifyCollectionChangedEventArgs arg = null;
                List<object> newItems = new List<object>();
                IList source = null;
                if (e.ListChangedType != ListChangedType.Reset && e.ListChangedType != ListChangedType.PropertyDescriptorAdded && e.ListChangedType != ListChangedType.PropertyDescriptorChanged && e.ListChangedType != ListChangedType.PropertyDescriptorDeleted)
                {
                    source = sender as IList;
                }

                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        newItems.Add(source[e.NewIndex]);
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewIndex);
                        break;

                    case ListChangedType.ItemDeleted:
                        //var removedItem = source[e.NewIndex];
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems, e.NewIndex);
                        break;

                    case ListChangedType.ItemChanged:
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, source[e.NewIndex], source[e.NewIndex], e.NewIndex);
                        break;

                    case ListChangedType.ItemMoved:
                        newItems.Add(source[e.NewIndex]);
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItems, e.NewIndex, e.OldIndex);
                        break;

                    case ListChangedType.Reset:
                        arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                        break;
                }

                if (arg != null)
                {
                    this.UpdateCollectionView(this, arg);
                    this.OnCollectionChanged(arg);
                }
            }
        }*/

        /// <summary>
        /// Updates the collection view by handling the NotifyCollectionChangedEventArgs if there is no grouping.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected override void UpdateCollectionView(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecordEntry record = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    record = this.Records.CreateRecordEntry(e.NewItems[0]);
                    var newIdx = e.NewStartingIndex;
                    if (newIdx >= this.Count)
                    {
                        this.Records.Add(record);
                    }
                    else
                    {
                        this.Records.Insert(newIdx, record);
                    }
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, record.Data, newIdx));
                    // this.SetCurrent(record.Data, newIdx);
                    break;

#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    record = this.Records.CreateRecordEntry(e.NewItems[0]);
                    this.Records.RemoveAt(e.OldStartingIndex);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, record.Data, e.OldStartingIndex));
                    this.Records.Insert(e.NewStartingIndex, record);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, record.Data, e.NewStartingIndex));
                    break;
#endif
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex == this.CurrentPosition)
                        this.UpdateCurrentItem();
                    record = this.Records[e.OldStartingIndex];
                    this.Records.RemoveAt(e.OldStartingIndex);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, record.Data, e.OldStartingIndex));
                    this.UpdateCurrentPosition();
                    break;

                case NotifyCollectionChangedAction.Replace:
                     record = this.Records[e.OldStartingIndex];
                    record.Data = e.NewItems[0];
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems[0], e.OldItems[0], e.OldStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Refresh();
                    //if (this.Records != null)
                    //{
                    //    this.Records.Clear();
                    //}
                    //this.EnsureInitialized();
                    //this.OnCollectionChanged(e);
                    break;
            }
        }

        protected override void OnTopLevelGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnTopLevelGroupCollectionChanged(sender, e);
            //this.IsInSuspend = true;
            //switch (e.Action)
            //{
            //    case NotifyCollectionChangedAction.Remove:
            //        var drv = ((RecordEntry)e.OldItems[0]).Data as DataRowView;
            //        if (drv != null)
            //        {
            //            drv.DataView.Table.Rows.Remove(drv.Row);
            //            drv.DataView.Table.AcceptChanges();
            //        }
            //        this.MoveCurrentToNext();
            //        break;
            //    case NotifyCollectionChangedAction.Add:
            //        RecordEntry record = e.NewItems[0] as RecordEntry;
            //        if (e.NewStartingIndex >= this.Count)
            //        {
            //            this.Records.Add(record);
            //        }
            //        else
            //        {
            //            this.Records.Insert(e.NewStartingIndex, record);
            //        }
            //        break;
            //    case NotifyCollectionChangedAction.Reset:
            //        this.Records.Clear();
            //        this.UnwireEvents();
            //        break;
            //}
            //this.IsInSuspend = false;



            //Console.WriteLine("Item {0}", ((RecordEntry)e.OldItems[0]).ToString());
            //Console.WriteLine("Index Value {0}", drv.Row["ID"]);

            //foreach (DataRowView dr in drv.DataView)
            //{
            //    Console.WriteLine("ID Value {0}", dr.Row["ID"]);
            //}

        }

        /// <summary>
        /// Gets or sets the view source.
        /// </summary>
        /// <value>The view source.</value>
        public DataView ViewSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the <see cref="E:SortDescriptionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnSortDescriptionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSort();
            base.OnSortDescriptionChanged(e);
        }

        /// <summary>
        /// Refreshes the sort.
        /// </summary>
        protected override void RefreshSort()
        {
            if (!this.IsGrouping)
            {
                if (this.ViewSource.Count == 0)
                {
                    return;
                }

                this.ViewSource.Sort = this.GetSortString();
                //This method invoked from RefreshSort method. To avoid no of hits commenting the invoking of this method here.
                //this.EnsureInitialized();
            }
            else
            {
                this.TopLevelGroup.SuspendEvents();
                var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                grpRefresh.RefreshSortingOrder();
                //Set sort string to Datatabe.
                this.ViewSource.Sort = this.GetSortString();
                // no need to call the below method, sorting is handled in GetGroupResult()
                //this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups, string.Empty, 0);
                this.TopLevelGroup.ResumeEvents();
            }
        }

        /// <summary>
        /// Refreshes the sorting order for bottom level.
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <param name="prevFilter"></param>
        /// <param name="level"></param>
        protected virtual void RefreshSortingOrderWithFiltersForBottomLevel(List<Group> groups, string prevFilter, int level)
        {
            var groupBy = this.GroupDescriptions.OfType<PropertyGroupDescription>().Select(g => g.PropertyName).ToArray();
            int count = groupBy.GetLength(0);
            string filter = string.Empty;
            foreach (var group in groups)
            {
                //filter = prevFilter + string.Format("[{1}] = '{0}'", group.Key, groupBy[level]);
                //filter = prevFilter + string.Format("[{1}] = '{0}'", group.Key, groupBy[level]);
                if (group.Key is DateTime)
                {
                    filter = prevFilter + string.Format("[{1}] = #{0}#", ((DateTime)group.Key).ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat), groupBy[level]);
                }
                else if (group.Key == null || group.Key is DBNull)
                {
                    filter = prevFilter + string.Format("[{1}] is null", group.Key, groupBy[level]);
                }
                else
                {
                    filter = prevFilter + string.Format("[{1}] = '{0}'", group.Key, groupBy[level]);
                }

                if (group.IsBottomLevel)
                {
                    var groupRecordsEntry = group.Details as GroupRecordEntry;

                    // var   filteredRecords = this.GetFilteredRows(groupRecordsEntry.ToArray(), this.ViewSource);
                    var filteredRecords = new DataView(this.ViewSource.Table, filter, this.ViewSource.Sort, DataViewRowState.CurrentRows);
                    if (this.SortDescriptions.Count == 0)
                    {
                        groupRecordsEntry.PopulateRecords(filteredRecords, null);
                        // return;
                    }
                    else
                    {
                        //var recordcount = groupRecordsEntry.GetRecordsCount();
                        //if (recordcount != 0)
                        //{
                        IEnumerable sortedSource = this.GetSortedSource(filteredRecords);
                        groupRecordsEntry.PopulateRecords(sortedSource, null);
                        //}
                    }
                    this.TopLevelGroup.UpdateSummaries(group);
                }
                else
                {
                    filter += " AND ";
                    this.RefreshSortingOrderWithFiltersForBottomLevel(group.Groups, filter, level + 1);
                }
            }
        }

        private IEnumerable GetSortedSource(DataView filteredView)
        {
            filteredView.Sort = this.GetSortString();
            return filteredView;
            //var sortFieldComparer = new SortFieldComparer(this.SortDescriptions, this.Culture, (record, propName) =>
            //{
            //    var provider = this.GetPropertyAccessProvider();
            //    if (provider != null)
            //    {
            //        return provider.GetValue(record, propName);
            //    }

            //    return null;
            //});
            //var array = groupRecordsEntry.ToArray();
            //Array.Sort(array, sortFieldComparer);
            //return array;
        }

        private string GetSortString()
        {
            string sortString = string.Empty;
            this.SortDescriptions.IterateIndex<SortDescription>((i, s) =>
            {
                if (s.Direction == ListSortDirection.Ascending)
                {
                    if (i == 0)
                    {
                        sortString = sortString.OrderBy(s.PropertyName);
                    }
                    else
                    {
                        sortString = sortString.ThenBy(s.PropertyName);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        sortString = sortString.OrderByDescending(s.PropertyName);
                    }
                    else
                    {
                        sortString = sortString.ThenByDescending(s.PropertyName);
                    }
                }
            });
            return sortString;
        }

        /// <summary>
        /// Creates the records.
        /// </summary>
        /// <returns></returns>
        protected override IRecordsList CreateRecords()
        {
            IEnumerable source = this.ViewSource;
            if (this.IsGrouping)
            {
                List<object> recordList = new List<object>();
                foreach (var record in this.TopLevelGroup)
                {
                    if (record is RecordEntry)
                    {
                        recordList.Add(((RecordEntry)record).Data);
                    }
                }
                source = recordList.AsQueryable();
            }
            return EnumerableRecordsWrapper.CreateNew(source, this);
        }

        private DataTable GetDataTableForGroupBy()
        {
            DataTable table;
            if (this.SourceCollection is DataView)
            {
                table = ((DataView)this.SourceCollection).ToTable();
            }
            else
            {
                table = this.SourceCollection as DataTable;
            }

            return table;
        }

        /// <summary>
        /// Gets the group result.
        /// </summary>
        /// <param name="groupBy">The group by.</param>
        /// <returns></returns>
        protected override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            //var table = this.GetDataTableForGroupBy();
            //var dv = new DataView(table);
            var dv = this.ViewSource;
            //var result = dv.GroupByMany(this.SortDescriptions.ToList(), groupBy);
            var result = dv.GroupByMany(groupBy);
            return result;
        }

        //public override bool PassesFilter(object item)
        //{
        //    var index=this.Records.IndexOfRecord(item);
        //    if (index >= 0&&index<this.Records.Count)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //protected override void EnsureInitialized()
        //{
        //    base.EnsureInitialized();
        //}

        /// <summary>
        /// Refreshes the filters.
        /// </summary>
        public override void RefreshFilters()
        {
            var filterString = GetFilterString();
            var needsRefresh = false;
            if (this.ViewSource.RowFilter != string.Empty && filterString == string.Empty)
            {
                // when the rowfilter is set as string.empty we need to manuall refresh the state
                needsRefresh = true;
            }
            this.ViewSource.RowFilter = filterString;
            if (needsRefresh)
            {
                this.EnsureInitialized();
            }
            if (this.IsGrouping && this.TopLevelGroup != null)
            {
                // no need to check for EndDefer,
                // if (!this.IsInEndDefer)
                //{
                //    this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups, string.Empty, 0);
                //}
                
                var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                grpRefresh.RefreshFilters();
            }
        }

        /*
        private void RefreshFiltersForDataRows(GroupRecordEntry groupRecordEntry)
        {
            var enumerator = groupRecordEntry.UnfilteredRecords.GetEnumerator();
            DataView dataView = this.ViewSource;
            if (enumerator.MoveNext() && enumerator.Current != null)
            {
                if (dataView.RowFilter != string.Empty)
                {
                    var filteredRecords = this.GetFilteredRows(groupRecordEntry.ToArray(), dataView);
                    groupRecordEntry.PopulateRecords(filteredRecords, null);
                }
                else
                {
                    groupRecordEntry.PopulateRecords(groupRecordEntry.ToArray(), null);
                }
            }
            else
            {
                groupRecordEntry.Records.Clear();
            }
        }*/

        private DataView GetFilteredRows(IEnumerable groupSource, DataView view)
        {
            return new DataView(view.Table, view.RowFilter, view.Sort, DataViewRowState.CurrentRows);
            //var cloneTable = view.Table.Clone();
            //foreach (DataRowView rowView in groupSource)
            //{
            //    cloneTable.ImportRow(rowView.Row);
            //}

            //cloneTable.DefaultView.RowFilter = view.RowFilter;
            //return cloneTable.DefaultView;
        }

        private string GetFilterString()
        {
            var filterString = string.Empty;
            var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();
            var firstLoop = false;
            foreach (var column in filterColumns)
            {
                for (int i = 0;i < column.Filters.Count;i++)
                {
                    var fp = column.Filters[i];
                    // blow code is for Filtering null values.
                    if (fp.FilterValue == DBNull.Value)
                    {
                        string columnName = EscapeSpecialChars(column.MappingName);
                        if (!firstLoop)
                        {
                            if (fp.FilterType == FilterType.Equals)
                                filterString = "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                            if (fp.FilterType == FilterType.NotEquals)
                                filterString = "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                            firstLoop = true;
                        }
                        else
                        {
                            if (fp.FilterType == FilterType.Equals)
                            {
                                filterString = filterString.OrPredicate();
                                filterString += "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                            }
                            else
                            {
                                filterString = filterString.AndPredicate();
                                filterString += "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                            }
                        }
                    }
                    if (fp.FilterValue != null && fp.FilterValue.ToString() != "")
                    {
                        if (!firstLoop)
                        {
                            filterString = filterString.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                            firstLoop = true;
                        }
                        else
                        {
                            if (fp.PredicateType == PredicateType.And)
                            {
                                filterString = filterString.AndPredicate();
                                filterString = filterString.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                            }
                            else if (fp.PredicateType == PredicateType.Or)
                            {
                                filterString = filterString.OrPredicate();
                                filterString = filterString.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                            }
                        }
                    }
                }
            }
            return filterString;
        }

        internal string EscapeSpecialChars(string value)
        {
            var pattern = @"([-_&\]\\*\\%\\#\\[\\\\])";
            if (System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
            {
                if ((value.Contains("]") | value.Contains(@"\")) && !value.Contains(@"\]"))
                {
                    value = value.Replace(@"\", @"\\");
                    value = value.Replace("]", @"\]");
                }
                value = "[" + value + "]";
            }
            return value;
        }

    }

    public class DataViewWrapperList : List<DataRowView>, ITypedList, INotifyCollectionChanged
    {
        #region Variable Used

        private PropertyDescriptorCollection pdc = null;
        private DataView dv = null;

        #endregion

        #region Constructor

        public DataViewWrapperList(DataView dv)
        {
            this.dv = dv;
            this.dv.ListChanged += new ListChangedEventHandler(dv_ListChanged);
            pdc = ((ITypedList)dv).GetItemProperties(null);
            this.EnsureInitialized();
        }

        public DataViewWrapperList(DataTable dataTable)
        {
            this.dv = dataTable.AsDataView();
            this.dv.ListChanged += new ListChangedEventHandler(dv_ListChanged);
            pdc = ((ITypedList)dv).GetItemProperties(null);
            this.EnsureInitialized();
        }

        #endregion

        #region Adding DataRow to List
        private void EnsureInitialized()
        {
            var enumerator = dv.GetEnumerator();
            while (enumerator.MoveNext())
                this.Add((DataRowView)enumerator.Current);
        }
        #endregion

        #region Event for DataView

        private void dv_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs arg = null;
            var newItems = new List<DataRowView>();
            IList source = null;
            if (e.ListChangedType != ListChangedType.Reset)
            {
                source = sender as IList;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (e.NewIndex > -1 && e.NewIndex < this.dv.Count)
                        newItems.Add((DataRowView)source[e.NewIndex]);
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewIndex);
                    break;
                case ListChangedType.ItemDeleted:
                    if (e.NewIndex > -1 && e.NewIndex < this.Count)
                        newItems.Add(this[e.NewIndex]);
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems,
                                                               e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
            }

            if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted ||
                e.ListChangedType == ListChangedType.Reset)
                UpdateDataViewWrapperList(sender, arg);

        }

        private void UpdateDataViewWrapperList(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.Insert(e.NewStartingIndex, (DataRowView)e.NewItems[0]);
                    this.OnCollectionChanged(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveAt(e.OldStartingIndex);
                    this.OnCollectionChanged(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Clear();
                    this.EnsureInitialized();
                    this.OnCollectionChanged(e);
                    break;
            }
        }

        #endregion

        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return pdc;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "DataViewWrapper";
        }

        #endregion

        #region INotifyCollectionChanged Members

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
#endif
