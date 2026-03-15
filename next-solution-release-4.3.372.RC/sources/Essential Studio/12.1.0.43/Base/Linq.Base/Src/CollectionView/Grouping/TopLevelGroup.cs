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
    using System.Text;
    using System.Windows.Data;
    using Syncfusion.Linq;
    using Syncfusion.Linq.Data;
    using System.Reflection;
#if !SILVERLIGHT
    using System.Data;
#endif

    /// <summary>
    /// TopLevelGroup is the first-level of the Groups present in <see cref="ICollectionViewAdv"/>. It maintains the data structure for Grouping with <see cref="ICollectionViewAdv"/>. Iterate the DisplayElements property to get one-to-one mapping of the index with the TopLevelGroup items. Access all the Bottom-level and other nested level groups with the Groups property.
    /// </summary>
    public class TopLevelGroup : Group, IGroupRefresh, IGroupList, INotifyCollectionChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TopLevelGroup"/> class.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        public TopLevelGroup(CollectionViewAdv collectionView)
            : base(null, 0)
        {
            this.CollectionView = collectionView;
            this.CreateDetailsForGroups(0);
            this.IsExpanded = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.displayElements != null)
                {
                    this.displayElements.Dispose();
                    this.displayElements = null;
                }

                if (this.CollectionView != null)
                {
                    this.CollectionView = null;
                }
            }
        }

        /// <summary>
        /// Invalidates the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public virtual void Invalidate(int index, int count)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is top level group. Top-Level Group will be the first-level group.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is top level group; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTopLevelGroup
        {
            get
            {
                return true;
            }
        }

        private bool resetCache = false;
        public bool ResetCache
        {
            get
            {
                return this.resetCache;
            }

            set
            {
                this.resetCache = value;
            }
        }

        private int relationsCount = 0;
        /// <summary>
        /// Gets or sets the relations count.
        /// </summary>
        /// <value>The relations count.</value>
        public virtual int RelationsCount
        {
            get
            {
                return this.relationsCount;
            }
            set
            {
                this.relationsCount = value;
            }
        }

        internal void OnCollectionChanged(NotifyCollectionChangedAction action, List<NodeEntry> changedItems, int startindex)
        {
            this.ResetCache = true;
            var arg = new NotifyCollectionChangedEventArgs(action, changedItems, startindex);
            this.CollectionView.OnCollectionChanged(arg);
        }

        #region IGroupRefresh members

        /// <summary>
        /// Refreshes the sorting order.
        /// </summary>
        void IGroupRefresh.RefreshSortingOrder()
        {
            if (this.deferRefreshCount > -1)
            {
                return;
            }

            //if (this.IsLegacyDataTable)
            //{
                // the grouping already sorts the data.
                this.RefreshSortingOrderForGroups();
                // this.RefreshSortingOrderForBottomLevel();
                this.ResetDisplayElements();
            //}
        }

        private void RefreshSortingOrderForGroups()
        {
            var sortkey = this.GetSortKey(0);
            if (sortkey.PropertyName != null)
            {
                this.RefreshSortingOrderForGroups(this, sortkey);
            }
            
            this.SortForAllGroups(this.Groups);
            //this.CollectionViewGroup = new CollectionViewGroupRoot(this);
        }

        private SortDescription GetSortKey(int level)
        {
            var group = this.CollectionView.GroupDescriptions[level] as PropertyGroupDescription;
            foreach (var sortdecription in this.CollectionView.SortDescriptions)
            {
                if (sortdecription.PropertyName == group.PropertyName)
                {
                    return sortdecription;
                }
            }
            return new SortDescription();
        }

        private void SortForAllGroups(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (!group.IsBottomLevel)
                {
                    if (this.CollectionView.SortDescriptions.Count > group.Level)
                    {
                        var sortkey = this.GetSortKey(group.Level);
                        if (sortkey.PropertyName != null)
                        {
                            this.RefreshSortingOrderForGroups(group, sortkey);
                        }
                        this.SortForAllGroups(group.Groups);
                    }
                }
            }
        }

        private void RefreshSortingOrderForGroups(Group group, SortDescription sortKey)
        {
            //var groupArray = group.Groups.ToArray();
            IComparer<Group> groupComparer = null;
            if (this.CollectionView.GroupComparer != null)
            {
                groupComparer = this.CollectionView.GroupComparer;

                if (groupComparer is ISortDirection)
                    (groupComparer as ISortDirection).SortDirection = sortKey.Direction;
            }
            else
            {
                if (this.CollectionView.SortComparers != null && this.CollectionView.SortComparers.Keys.Contains(sortKey.PropertyName))
                {
                    groupComparer = this.CollectionView.SortComparers[sortKey.PropertyName] as IComparer<Group>;
                    if (groupComparer is ISortDirection)
                        (groupComparer as ISortDirection).SortDirection = sortKey.Direction;
                }
                else
                    groupComparer = new GroupComparer() { SortDirection = sortKey.Direction };
            }

            group.Groups.Sort(groupComparer);
            //Array.Sort(groupArray, groupComparer);
            //group.Groups.Clear();
            //if (sortKey.Direction == ListSortDirection.Ascending)
            //{
            //    foreach (Group grp in groupArray)
            //    {
            //        group.Groups.Add(grp);
            //    }
            //}
            //else
            //{
            //    for (int i = groupArray.Length - 1; i >= 0; i--)
            //    {
            //        group.Groups.Add(groupArray[i]);
            //    }
            //}
        }

        /// <summary>
        /// Refreshes the filters.
        /// </summary>
        void IGroupRefresh.RefreshFilters()
        {
            if (this.deferRefreshCount > -1)
            {
                return;
            }

            if (this.CollectionView.CanFilter)
            {
                // this.SetDirty();
                this.Groups.SetDirty();
                // Filters applied to groups has to be handled at the CollectionView, Call RefreshFilters only to refresh the TopLevelGroup summaries and display elements
                //this.RefreshFilters(this.Groups);
                this.UpdateCaptionSummaries();
                this.ResetDisplayElements();
            }
        }

        /*
        /// <summary>
        /// Refreshes the filters.
        /// </summary>
        /// <param name="groups">The groups.</param>
        protected virtual void RefreshFilters(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    var groupRecordEntry = (GroupRecordEntry)group.Details;
#if !SILVERLIGHT
                    if (!this.IsLegacyDataTable)

#endif
                    {
                        if (this.CollectionView.Filter != null)
                        {
                            groupRecordEntry.PopulateRecords(groupRecordEntry.ToArray(), this.CollectionView.Filter);
                        }
                        else
                        {
                            groupRecordEntry.PopulateRecords(groupRecordEntry.UnfilteredRecords, null);
                        }
                        this.UpdateSummaries(group);
                    }
#if !SILVERLIGHT
                    else
                    {
                        this.RefreshFiltersForDataRows(groupRecordEntry);
                    }
#endif
                }
                else
                {
                    this.RefreshFilters(group.Groups);
                }
            }
        }*/

        private bool IsSuspend = false;
        /// <summary>
        /// Suspends the events.
        /// </summary>
        public void SuspendEvents()
        {
            IsSuspend = true;
        }

        /// <summary>
        /// Resumes the events.
        /// </summary>
        public void ResumeEvents()
        {
            IsSuspend = false;
        }

        /// <summary>
        /// returns an IDisposable object to specify controlled updates.
        /// </summary>
        /// <returns></returns>
        IDisposable IGroupRefresh.DeferRefresh()
        {
            this.deferRefreshCount += 1;
            return new DeferHelper(this);
        }

        private class DeferHelper : IDisposable
        {
            private TopLevelGroup topLevelGroup;
            public DeferHelper(TopLevelGroup topLevelGroup)
            {
                this.topLevelGroup = topLevelGroup;
            }

            public void Dispose()
            {
                this.topLevelGroup.EndDefer();
                this.topLevelGroup = null;
            }
        }

        private int deferRefreshCount = -1;
        internal void EndDefer()
        {
            this.deferRefreshCount -= 1;

            if (this.deferRefreshCount == -1)
            {
                var grpRefreh = this as IGroupRefresh;
                grpRefreh.RefreshSortingOrder();
                grpRefreh.RefreshFilters();

                // sorting order refresh will automatically apply filters too
                // grpRefreh.RefreshFilters();
            }
        }

        #endregion

        /// <summary>
        /// Gets the collection view.
        /// </summary>
        /// <value>The collection view.</value>
        public ICollectionViewAdv CollectionView
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the group descriptions.
        /// </summary>
        /// <value>The group descriptions.</value>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this.CollectionView.GroupDescriptions;
            }
        }

        #region IGroupList Members

        /// <summary>
        /// Adds the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Add(object record, bool isInSourceCollectionChange)
        {
            RecordEntry item = this.CollectionView.Records.CreateRecordEntry(record);
            this.DisplayElements.Add(item, isInSourceCollectionChange);
        }

        public void Insert(object record, int index, bool isInSourceCollectionChange)
        {
            RecordEntry item = null;
#if   !SILVERLIGHT          
            if (this.IsLegacyDataTable)
            {
                if (record is DataRowView && ((DataRowView)record).Row.RowState == DataRowState.Detached)
                {
                    return;
                }
                item = this.CollectionView.Records.GetRecord(record);
                if (item == null)
                {
                    item = this.CollectionView.Records.CreateRecordEntry(record);

                }
            }

            else
            {
#endif
                item = this.CollectionView.Records.CreateRecordEntry(record);
#if   !SILVERLIGHT  
            }
#endif

            this.DisplayElements.InsertRecord(item, index, isInSourceCollectionChange);
            
        }

        /// <summary>
        /// Removes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="isInSourceCollectionChange">if set to <c>true</c> [is in source collection change].</param>
        /// <returns></returns>
        public virtual int Remove(object record, bool isInSourceCollectionChange)
        {
            return this.RemoveRecord(record, isInSourceCollectionChange);
        }
        /// <summary>
        /// Resets the group.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propertyName">Name of the property.</param>
        internal virtual void ResetGroup(object record, string propertyName)
        {
            this.DisplayElements.ResetGroup(this, record, true, propertyName);
        }
        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="isInSourceCollectionChange">if set to <c>true</c> [is in source collection change].</param>
        /// <returns></returns>
        private int RemoveRecord(object record, bool isInSourceCollectionChange)
        {
            //int index = -1;   
#if SILVERLIGHT
            if (record is List<object>)
                record = ((List<object>)record)[0];
#endif

            RecordEntry item = this.CollectionView.Records.GetRecord(record); // GetItem(record, this, ref index);
            var index =-1;
            if (item != null)
                index = this.CollectionView.Records.IndexOfRecord(item);
            if (index < 0)
            {
                if (isInSourceCollectionChange)
                    this.DisplayElements.RemoveItem(this, record);
                return -1;
            }
            if (item != null)
            {
                var parentGroup = item.Parent as Group;
                var removedAt = this.DisplayElements.RemoveNode(item, isInSourceCollectionChange);
                if (parentGroup != null)
                {
                    while (parentGroup.Parent != null)
                    {
                        parentGroup = parentGroup.Parent;
                        parentGroup.SetDirty();
                    }
                    return removedAt;
                }
                return removedAt;
              
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Determines whether the record is found in the DisplayElements of the TopLevelGroup.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(object record)
        {
            if (this.IndexOf(record) == -1)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Finds the index of the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public int IndexOf(object record)
        {
            int index = -1;
            RecordEntry item = GetItem(record, this, ref index);
            if (item != null)
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        private RecordEntry GetItem(object record, Group group, ref int index)
        {
            RecordEntry item = null;
            //if (group.IsExpanded)
            {
                foreach (var innergroup in group.Groups)
                {
                    if (innergroup.ItemsCount > 0)
                    {
                        index++;
                        if (innergroup.IsExpanded)
                        {
                            if (this.CheckKey(innergroup, record))
                            {
                                if (innergroup.IsBottomLevel)
                                {
                                    foreach (var originalrecord in innergroup.Records)
                                    {
                                        index++;
                                        if (originalrecord.Data == record)
                                        {
                                            item = originalrecord;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    item = GetItem(record, innergroup, ref index);
                                    if (item != null)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                index += innergroup.GetYAmountCache() - 1;
                            }
                        }
                    }
                    if (item != null)
                    {
                        break;
                    }
                }
            }
            return item;
        }

        internal void RaiseCollectionChanged(NotifyCollectionChangedAction action, RecordEntry record)
        {
            if (this.IsSuspend)
            {
                return;
            }

            int index = 0;
            if (action != NotifyCollectionChangedAction.Remove)
            {
                GetResolvedIndex(this, record.Data, ref index);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                index = this.CollectionView.Records.IndexOfRecord(record.Data);
            }

            this.RaiseCollectionChanged(action, record, index);
        }

        internal void RaiseCollectionChanged(NotifyCollectionChangedAction action, RecordEntry record, int index)
        {
            if (this.IsSuspend)
            {
                return;
            }

            var handler = this.CollectionChanged;
            if (handler != null)
            {
                NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(action, record, index);
                handler(this, arg);
            }
        }

        private bool GetResolvedIndex(Group baseGroup, object data, ref int index)
        {
            var breakLoop = false;
            foreach (var group in baseGroup.Groups)
            {
                if (CheckKey(group, data))
                {
                    if (group.IsBottomLevel)
                    {
                        if (group.Details != null)
                        {
                            for (int idx = 0; idx < group.Records.Count; idx++)
                            {
                                var rec = group.Records[idx];
                                if (rec.Data == data)
                                {
                                    breakLoop = true;
                                    break;
                                }
                                index += 1;
                            }
                        }
                    }
                    else
                    {
                        breakLoop = GetResolvedIndex(group, data, ref index);
                    }
                }
                else
                {
                    index += GetTotalRecordCount(group);
                }

                if (breakLoop)
                {
                    break;
                }
            }
            return breakLoop;
        }

        private int GetTotalRecordCount(Group group)
        {
            var count = 0;
            if (group.IsBottomLevel)
            {
                if (group.Details != null)
                {
                    count += group.Records.Count;
                }
            }
            else
            {
                foreach (var grp in group.Groups)
                {
                    count += GetTotalRecordCount(grp);
                }
            }
            return count;
        }

        internal bool IsCorrectGroup(Group group, object record)
        {
            if (group.Parent == null)
                return true;

            if (this.CheckKey(group, record))
                return this.IsCorrectGroup(group.Parent, record);
            return false;
        }

        internal bool CheckKey(Group group, object record)
        {
            if (record == null)
            {
                return false;
            }

            var pgd = this.CollectionView.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            object value = null;
            var isDataTable = false;
#if !SILVERLIGHT
            isDataTable = this.IsLegacyDataTable;
#endif
            if (!record.Equals(this.CollectionView.CurrentEditItem) || isDataTable)
            {
                var converter = pgd.Converter;
                if (pgd.Converter != null)
                {
                    value = converter.Convert(record, group.Key != null ? group.Key.GetType() : typeof (object), null,
                                              this.CollectionView.Culture);
                }
                else
#if !SILVERLIGHT
                    if (!this.IsLegacyDataTable && !this.IsDataViewWrapper)
#endif
                    {
                        var provider = this.CollectionView.GetPropertyAccessProvider();
                        if (provider != null)
                        {
                            value = provider.GetValue(record, pgd.PropertyName);
                        }
                    }
#if !SILVERLIGHT
                    else
                    {
                        object data = null;
                        var rowView = record as DataRowView;
                        if (rowView != null && rowView.Row != null && rowView.Row.RowState != DataRowState.Detached)
                            data = rowView[pgd.PropertyName];
                        value = data;
                    }
#endif
            }
            else
            {
                var currenteditItemState = this.CollectionView.CurrentEditItemState;
                if (currenteditItemState != null)
                {
                    value = currenteditItemState[pgd.PropertyName];
                }
            }

            if ((value == null && group.Key == null) || (value is DBNull && group.Key is DBNull) ||
                (!(value is IComparable) && !(group.Key is IComparable)))
            {
                return true;
            }
            else if (value == null || group.Key == null || value is DBNull || group.Key is DBNull ||
                     !(value is IComparable) || !(group.Key is IComparable))
            {
                return false;
            }

            var c = ((IComparable)group.Key).CompareTo((IComparable)value);
            return c == 0;
        }


        #endregion

        #region Display Elements

        private GroupDisplayElements displayElements = null;
        /// <summary>
        /// Gets the display elements.
        /// </summary>
        /// <value>The display elements.</value>
        public GroupDisplayElements DisplayElements
        {
            get
            {
                if (this.displayElements == null)
                {
                    this.ResetDisplayElements();
                }

                return this.displayElements;
            }
        }

        /// <summary>
        /// Resets the display elements.
        /// </summary>
        internal void ResetDisplayElements()
        {
            if (this.displayElements != null)
            {
                this.displayElements.Dispose();
                this.displayElements = null;
                this.resetCache = true;
            }

            this.displayElements = new GroupDisplayElements(this);
        }

        /// <summary>
        /// Gets the max level.
        /// </summary>
        /// <returns></returns>
        public int GetMaxLevel()
        {
            return this.CollectionView.GroupDescriptions.Count;
        }

        #endregion
        
        #region Group Caption Text

        /// <summary>
        /// Gets the group caption text for the specified <see cref="Group"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="groupSpecifierText">The group specifier text.</param>
        /// <param name="columnHeaderName"></param>
        /// <returns></returns>
        public virtual string GetGroupCaptionText(Group group, string groupSpecifierText, string columnHeaderName)
        {
            string result = string.Empty;
            if (group.Level == 0)
                return result;
            var kvp = this.GetGroupCaptionTextList(group, groupSpecifierText, columnHeaderName);
            result = string.Format(kvp.Key, kvp.Value.ToArray());
            return result;
        }

        /// <summary>
        /// Gets the group caption text for the specified <see cref="Group"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="groupSpecifierText">The group specifier text.</param>
        /// <param name="columnHeaderName"></param>
        /// <returns></returns>
        public KeyValuePair<string, List<String>> GetGroupCaptionTextList(Group group, string groupSpecifierText, string columnHeaderName)
        {
#if !SILVERLIGHT
            var properties = this.GetCaptionProperties(group);
            PropertyDescriptor[] pds = null;
#else
            var properties = this.GetCaptionProperties(group);
            PropertyInfo[] pds = null;
#endif
            string formatString = string.Empty;
            var flag = false;

            if (group.SummaryDetails != null)
            if (/*this.TableProperties.ShowGroupSummaryInCaption && */this.CollectionView.CaptionSummaryRow != null && group.SummaryDetails.SummaryValues.Count > 0)
            {
                var captionSummaryRow = this.CollectionView.CaptionSummaryRow;
                //if (captionSummaryRow.ShowSummaryInRow)
                {
                    //Title="'{Name} Charges - {FreightSummary}$ for {OrderCount} Items'"
                    var captionText = this.CollectionView.CaptionSummaryRow.Title;
                    var summaryRecordEntry = group.SummaryDetails;
                    if (summaryRecordEntry != null)
                    {
                        var summaryItems = summaryRecordEntry.SummaryValues;
                        var summaryRow = captionSummaryRow;
                        Dictionary<string, object> rowValues = new Dictionary<string, object>();
                        summaryRow.SummaryColumns.ForEach<ISummaryColumn>(col =>
                        {
                            var item = summaryItems.FirstOrDefault(s => s.Name == col.Name);
                            var value = SummaryCreator.GetFormattedSummary(item.AggregateValues, col, this.CollectionView);
                            rowValues.Add(col.Name, value);
                        });
                        captionText.ParseFormat(false, properties, out formatString, out pds);
                        if (this.CanParseSummaryItemsForCaptionText(formatString, rowValues))
                        {
                            formatString = this.ParseFormatForCaptionText(formatString, rowValues);
                        }

                        flag = true;
                    }
                }
            }

            if (!flag)
            {
                groupSpecifierText.ParseFormat(false, properties, out formatString, out pds);
            }

            List<string> objs = new List<string>();
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null)
                {
                    var pd = pds[i];
                    object value = null;
                    if (pd.Name == "ColumnName")
                    {
                        //var visibleCol = this.TableProperties.VisibleColumns.Where(v => v.MappingName == groupedCol.PropertyName).FirstOrDefault();
                        //if (visibleCol != null)
                        //{
                        //    value = visibleCol.HeaderText != string.Empty ? visibleCol.HeaderText : visibleCol.MappingName;
                        //}
                        if (columnHeaderName != string.Empty)
                        {
                            value = columnHeaderName;
                        }
                        else
                        {
                            var groupedCol = this.CollectionView.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                            value = groupedCol.PropertyName;
                        }
                    }
                    else if (pd.Name == "Key")
                    {
                        value = pd.GetValue(group);
                    }
                    else if (pd.Name == "ItemsCount")
                    {
                        value = pd.GetValue(group);
                    }
                    objs.Insert(i, value != null ? value.ToString() : string.Empty);
                }
                else
                {
                    objs.Insert(i, string.Empty);
                }
            }

            //result = string.Format(formatString, objs.ToArray());
            var kvp = new KeyValuePair<string, List<string>>(formatString, objs);
            return kvp;
        }



        private bool CanParseSummaryItemsForCaptionText(string value, Dictionary<string, object> summaryColumns)
        {
            int n1 = value.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(value);
            }
            else
            {
                sb.Append(value.Substring(0, n1));
            }

            var flag = false;
            while (n1 != -1)
            {
                n2 = value.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = value.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = value.Substring(n1 + 1, n3 - n1 - 1);
                    var pd = summaryColumns.ContainsKey(name) ? summaryColumns[name] : null;
                    if (pd != null)
                    {
                        flag = true;
                    }
                    n1 = value.IndexOf("{", n2);
                }
            }
            return flag;
        }

        private string ParseFormatForCaptionText(string value, Dictionary<string, object> summaryColumns)
        {
            string result = string.Empty;
            int n1 = value.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(value);
            }
            else
            {
                sb.Append(value.Substring(0, n1));
            }

            while (n1 != -1)
            {
                n2 = value.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = value.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = value.Substring(n1 + 1, n3 - n1 - 1);
                    if (name == "0" || name == "1" || name == "2")
                    {
                        sb.Append("{" + name + "}");
                        n1 = value.IndexOf("{", n2);
                        if (n1 != -1)
                        {
                            sb.Append(value.Substring(n3 + 1, n1 - (n3 + 1)));
                        }
                    }
                    else
                    {
                        var pd = summaryColumns.ContainsKey(name) ? summaryColumns[name] : null;
                        if (pd != null)
                        {
                            sb.Append(pd.ToString());
                        }
                        else
                        {
                            sb.Append(value.Substring(n1, n1 + 1));
                            sb.Append(name);
                            sb.Append(value.Substring(n2, 1));
                        }
                        n1 = value.IndexOf("{", n2);
                        if (n1 == -1)
                        {
                            if (pd != null)
                            {
                                sb.Append(value.Substring(n3 + 1));
                            }
                            else
                            {
                                sb.Append(value.Substring(n3));
                            }
                        }
                        else
                        {
                            sb.Append(value.Substring(n3 + 1, n1 - (n3 + 1)));
                        }
                    }
                }
            }
            return sb.ToString();
        }

        //internal const string GroupCaptionConstant = "{ColumnName} : {Key} - {ItemCount} Items";
#if !SILVERLIGHT
        private Dictionary<string, PropertyDescriptor> GetCaptionProperties(Group group)
        {
            var properties = new Dictionary<string, PropertyDescriptor>();
            var groupProperty = TypeDescriptor.GetProperties(typeof(SortColumn)).Find("ColumnName", false);
            properties.Add(groupProperty.Name, groupProperty);
            var keyProperty = TypeDescriptor.GetProperties(typeof(Group)).Find("Key", false);
            var countProperty = TypeDescriptor.GetProperties(typeof(Group)).Find("ItemsCount", false);
            properties.Add(keyProperty.Name, keyProperty);
            properties.Add(countProperty.Name, countProperty);
            return properties;
        }
#endif
#if SILVERLIGHT
        private Dictionary<string, PropertyInfo> GetCaptionProperties(Group group)
        {
            var properties = new Dictionary<string, PropertyInfo>();
            var sortProperty = typeof(SortColumn).GetProperties();
            var groupProperty = sortProperty.FirstOrDefault(p => p.Name == "ColumnName");
            properties.Add(groupProperty.Name, groupProperty);
            var propertyInfoArray = group.GetType().GetProperties();
            var keyProperty = propertyInfoArray.FirstOrDefault(p => p.Name == "Key");
            properties.Add(keyProperty.Name, keyProperty);
            var countProperty = propertyInfoArray.FirstOrDefault(p => p.Name == "ItemsCount");
            properties.Add(countProperty.Name, countProperty);
            return properties;
        }
#endif

        #endregion

        #region Summary Display Text

        //public string GetSummaryDisplayTextForRow(SummaryRecordEntry summaryEntry)
        //{
        //    var summaryItems = summaryEntry.SummaryValues;
        //    var summaryRow = summaryEntry.SummaryRow;
        //    Dictionary<string, object> rowValues = new Dictionary<string, object>();
        //    summaryRow.SummaryColumns.ForEach<ISummaryColumn>(col =>
        //    {
        //        var item = summaryItems.FirstOrDefault(s => s.Name == col.Name);
        //        var value = SummaryCreator.GetFormattedSummary(item.AggregateValues,col,this.CollectionView);
        //        rowValues.Add(col.Name, value);
        //    });
        //    var result = summaryRow.Title.FormatByName(rowValues);
        //    return result;
        //}

        //public string GetSummaryDisplayText(SummaryRecordEntry summaryEntry, string columnName)
        //{
        //    string result = string.Empty;
        //    var summaryRow = summaryEntry.SummaryRow;
        //    var summaryCol = summaryRow.SummaryColumns.FirstOrDefault(s => s.MappingName == columnName);
        //    var summaryItems = summaryEntry.SummaryValues;
        //    if (summaryItems != null && summaryCol != null)
        //    {
        //        var item = summaryItems.FirstOrDefault(s => s.Name == summaryCol.Name);
        //        result = SummaryCreator.GetFormattedSummary(item.AggregateValues, summaryCol,this.CollectionView);
        //    }
        //    return result;
        //}

        #endregion

        #region Summaries

        #region CaptionSummary

        /// <summary>
        /// Updates the caption summaries.
        /// </summary>
        public void UpdateCaptionSummaries()
        {
            if (this.IsSuspend)
            {
                return;
            }
            if (this.CollectionView.CaptionSummaryRow == null || ((CollectionViewAdv)this.CollectionView).Count < 1/*|| !this.TableProperties.ShowGroupSummaryInCaption*/)
            {
                return;
            }

            var captionSummaryRow = this.CollectionView.CaptionSummaryRow;
            var maxLevel = this.GetMaxLevel();
            for (int i = maxLevel; i > 0; i--)
            {
                this.UpdateCaptionSummaries(this, i, captionSummaryRow);
            }
        }

        internal void UpdateCaptionSummaries(Group group)
        {
            if (this.CollectionView.CaptionSummaryRow == null)
            {
                return;
            }

            if (!group.IsBottomLevel)
            {
                this.UpdateCaptionSummaryForGroup(group, this.CollectionView.CaptionSummaryRow);
            }
            else
            {
                this.UpdateCaptionSummaryForBottomLevelGroup(group, this.CollectionView.CaptionSummaryRow);
            }
        }

        private void UpdateCaptionSummaries(Group group, int level, ISummaryRow summaryRow)
        {
            if (group.Level == level)
            {
                this.UpdateCaptionSummaryForGroup(group, summaryRow);
            }
            else
            {
                this.UpdateCaptionSummaries(group.Groups, level, summaryRow);
            }
        }

        private void UpdateCaptionSummaries(List<Group> groups, int level, ISummaryRow summaryRow)
        {
            foreach (var group in groups)
            {
                if (group.Level == level)
                {
                    if (group.IsBottomLevel)
                    {
                        this.UpdateCaptionSummaryForBottomLevelGroup(group, summaryRow);
                    }
                    else
                    {
                        this.UpdateCaptionSummaryForGroup(group, summaryRow);
                    }
                }
                else
                {
                    this.UpdateCaptionSummaries(group, level, summaryRow);
                }
            }
        }

        private void UpdateCaptionSummaryForGroup(Group group, ISummaryRow summaryRow)
        {
            var collection = this.CollectionView as CollectionViewAdv;
            var underlyingType = this.GetUnderlyingSourceType();
            foreach (var summaryColumn in summaryRow.SummaryColumns)
            {
                bool emptyconstructorflag = true;
#if !SILVERLIGHT
                if (!collection.IsLegacyDataTable)
#endif
                    emptyconstructorflag = underlyingType.GetConstructor(Type.EmptyTypes) != null;

                if (summaryColumn.SummaryType != SummaryType.Custom && emptyconstructorflag)
                {
                    var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, this.CollectionView);
                    var parsedFormat = string.Empty;
                    var pdsArray = SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat);
                    for (int i = 0; i < pdsArray.Length; i++)
                    {
                        var pd = pdsArray[i];
                        // generate records
                        var records = new List<object>();
#if !SILVERLIGHT
                        DataView clonedView = null;
                        if (this.IsLegacyDataTable)
                        {
                            clonedView = new DataView(this.GetClonedDataTable());
                        }
#endif
                        foreach (var innerGroup in group.Groups)
                        {
                            var summaryRecordEntry = innerGroup.SummaryDetails;

                            if (summaryRecordEntry == null)
                            {
                                continue;
                            }

                            object record = null;
#if !SILVERLIGHT
                            record = !this.IsLegacyDataTable ? underlyingType.CreateNew() : clonedView.AddNew();
                            var properties = TypeDescriptor.GetProperties(record);
                            var prop = properties.GetPropertyDescriptor(summaryColumn.MappingName);
#else
                            record = underlyingType.CreateNew();
                            var properties = record.GetType().GetProperties();
                            var prop = properties.FirstOrDefault(p => p.Name == summaryColumn.MappingName);
#endif

#if !SILVERLIGHT
                            if (!this.IsLegacyDataTable && prop == null)
#else
                            if (prop == null)
#endif
                            {
                                // create a dynamic class since we are dealing with unbound values
                                var source = this.CollectionView.SourceCollection;
                                var enumerator = source.GetEnumerator();
                                if (!enumerator.MoveNext())
                                {
                                    return;
                                }
                                var unboundExpressionFunc = this.CollectionView as IUnboundExpressionFunc;
                                if (unboundExpressionFunc == null)
                                {
                                    return;
                                }

                                var expressionFunc = unboundExpressionFunc.GetExpressionFunc(summaryColumn.MappingName);
                                var typeFunc = unboundExpressionFunc.GetTypeExpressionFunc(summaryColumn.MappingName);
                                var checkDelg = typeFunc.Compile();
                                var bodyType = (Type)checkDelg.DynamicInvoke(new object[] { summaryColumn.MappingName, enumerator.Current });                               
								
                                var dynProperty = new DynamicProperty(summaryColumn.MappingName, bodyType);
                                var dynClass = QueryableExtensions.CreateClass(dynProperty).CreateNew();
                                record = dynClass;

#if !SILVERLIGHT
                                properties = TypeDescriptor.GetProperties(record);
                                prop = properties.GetPropertyDescriptor(summaryColumn.MappingName);
#else

                            properties = record.GetType().GetProperties();
                            prop = properties.FirstOrDefault(p => p.Name == summaryColumn.MappingName);
#endif
                            }

                            string columnName = summaryColumn.MappingName;
                            string[] propertyNameList = columnName.Split('.');
                            int iterator, complexPropertyCount = propertyNameList.Count();
                            object tRecord = record;
                            for (iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                            {
#if !SILVERLIGHT
                                var tempProperyDescriptor = properties.Find(propertyNameList[iterator], true);
#else
                            var tempProperyDescriptor = properties.FirstOrDefault(d => d.Name == propertyNameList[iterator]);
#endif
                                if (tempProperyDescriptor != null)
                                {
                                    tRecord = tempProperyDescriptor.PropertyType.CreateNew();
#if !SILVERLIGHT
                                    properties = TypeDescriptor.GetProperties(tRecord);
#else
                                properties = tRecord.GetType().GetProperties();
#endif
                                }
                            }

                            record = tRecord;

                            var summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                            object value = null;
                            if (summaryValue != null)
                                summaryValue.AggregateValues.TryGetValue(pd.Name, out value);
                            if (value != null)
                            {
                                if (NullableHelperInternal.IsNullableType(prop.PropertyType))
                                {
                                    value = NullableHelperInternal.FixDbNUllasNull(value, prop.PropertyType);
                                }

                                value = NullableHelperInternal.ChangeType(value, prop.PropertyType);
#if !SILVERLIGHT
                                prop.SetValue(record, value);
#else
                                                    prop.SetValue(record, value, null);
#endif
                            }

                            records.Add(record);

                        }

#if !SILVERLIGHT
                        // workaround, just add an item if it is DataView. This caused a bug to exclude the last group in the subgroup.
                        if (this.IsLegacyDataTable)
                        {
                            // this would add a new item only in the view, and update the underlying cloned table with the RowState = Added, that way the summaries get the records computed properly. 
                            clonedView.AddNew();
                        }
#endif


                        // calc summaries
                        var enumerableSource1 = records.OfQueryable(collection.SourceType);
                        var actualSummaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, this.CollectionView);
                        var summaryDelg = actualSummaryAggregate.CalculateAggregateFunc();
                        summaryDelg.DynamicInvoke(new object[] { enumerableSource1, summaryColumn.MappingName, pd });
#if !SILVERLIGHT
                        var groupSummaryValue = pd.GetValue(actualSummaryAggregate);
#else
                    var groupSummaryValue = pd.GetValue(actualSummaryAggregate, null);
#endif

                        // add it to the group.SummaryDetails
                        SummaryRecordEntry recordEntry = group.SummaryDetails as SummaryRecordEntry;
                        if (group.SummaryDetails == null)
                        {
                            recordEntry = new SummaryRecordEntry(group, group.Level);
                            recordEntry.SummaryRow = summaryRow;
                            group.SummaryDetails = recordEntry;
                        }
                        // we may encounter the same summaryValue.Name with SummaryColumn.Name
                        var summaryValueEntry = recordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                        if (summaryValueEntry == null)
                        {
                            summaryValueEntry = new SummaryValue() { Name = summaryColumn.Name };
                            recordEntry.SummaryValues.Add(summaryValueEntry);
                        }
                        object summaryResult = null;
                        if (summaryValueEntry.AggregateValues.TryGetValue(pd.Name, out summaryResult))
                        {
                            summaryValueEntry.AggregateValues[pd.Name] = groupSummaryValue;
                        }
                        else
                        {
                            summaryValueEntry.AggregateValues.Add(pd.Name, groupSummaryValue);
                        }
                    }
                }
                else
                {
                    List<object> records = new List<object>();
                    GetRecordsFromGroup(group, ref records);
                    var enumerableSource = records.OfQueryable(collection.SourceType);
                    var summaryRecordEntry = new SummaryRecordEntry(group, group.Level);
                    summaryRecordEntry.SummaryRow = summaryRow;

                    foreach (var summaryColumns in summaryRow.SummaryColumns)
                    {
                        if (summaryColumns.Name != string.Empty && summaryColumns.MappingName != string.Empty)
                        {
                            Dictionary<string, object> summaryItems = new Dictionary<string, object>();
                            SummaryCreator.RaiseQuerySummaryAggregate(enumerableSource, summaryColumns, summaryItems, this.CollectionView);
                            var summaryValue = new SummaryValue() { Name = summaryColumns.Name };
                            foreach (var kvp in summaryItems)
                            {
                                summaryValue.AggregateValues.Add(kvp.Key, kvp.Value);
                            }
                            summaryRecordEntry.SummaryValues.Add(summaryValue);
                        }
                    }
                    group.SummaryDetails = summaryRecordEntry;
                }
            }
        }

        private void GetRecordsFromGroup(Group group, ref List<object> records)
        {
            foreach (var innerGroup in group.Groups)
            {
                if (innerGroup.IsBottomLevel)
                {
                    var groupRecordEntry = innerGroup.Details as GroupRecordEntry;
                    var rec = groupRecordEntry.ToArray();
                    records.AddRange(rec);
                }
                else
                {
                    this.GetRecordsFromGroup(innerGroup, ref records);
                }
            }
                            
        }

        private void UpdateCaptionSummaryForBottomLevelGroup(Group group, ISummaryRow summaryRow)
        {
            var collection = this.CollectionView as CollectionViewAdv;
            if (group.IsBottomLevel)
            {
                var groupRecordEntry = group.Details as GroupRecordEntry;
                var records = groupRecordEntry.ToArray();
                if (records.Length > 0)
                {
                    var enumerableSource = records.OfQueryable(collection.SourceType);
                    var summaryRecordEntry = new SummaryRecordEntry(group, group.Level);
                    summaryRecordEntry.SummaryRow = summaryRow;
                    foreach (var summaryColumn in summaryRow.SummaryColumns)
                    {
                        if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
                        {
                            Dictionary<string, object> summaryItems = new Dictionary<string, object>();
                            SummaryCreator.RaiseQuerySummaryAggregate(enumerableSource, summaryColumn, summaryItems, this.CollectionView);
                            var summaryValue = new SummaryValue() { Name = summaryColumn.Name };
                            foreach (var kvp in summaryItems)
                            {
                                summaryValue.AggregateValues.Add(kvp.Key, kvp.Value);
                            }
                            summaryRecordEntry.SummaryValues.Add(summaryValue);
                        }
                    }
                    group.SummaryDetails = summaryRecordEntry;
                }
            }
        }

        #endregion

        #region Row Summaries

        private Type GetUnderlyingSourceType()
        {
            var enumerator = this.CollectionView.SourceCollection.GetEnumerator();
            var hasItem = enumerator.MoveNext();
            if (hasItem)
            {
                return enumerator.Current.GetType();
            }
            else if (this.CollectionView.SourceCollection.GetType() != null)
            {
                return this.CollectionView.SourceCollection.GetType();
            }

            return null;
        }

        /// <summary>
        /// Updates the summaries for the specified <see cref="Group"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        public void UpdateSummaries(Group group)
        {
            var collection = this.CollectionView as CollectionViewAdv;
            if (collection != null && collection.IsSuspend)
            {
                return;
            }

            if (group.IsBottomLevel /*&& this.TableProperties.SummaryRows.Count > 0*/)
            {
                var visibleSummaryRows = this.CollectionView.SummaryRows.Where(s => s.IsVisible);
                var groupRecordEntry = group.Details as GroupRecordEntry;
                if (visibleSummaryRows.Any())
                {                    
                    var records = groupRecordEntry.ToArray();
                    if (records.Length > 0)
                    {
                        var enumerableSource = records.OfQueryable(collection.SourceType);
                        var counter0 = 0;
                        foreach (var summaryRow in visibleSummaryRows)
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
                                if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
                                {
                                    Dictionary<string, object> summaryItems = new Dictionary<string, object>();
                                    SummaryCreator.RaiseQuerySummaryAggregate(enumerableSource, summaryColumn, summaryItems, this.CollectionView);
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
                            counter0 += 1;
                        }
                    }
                }
                else
                {
                    groupRecordEntry.Summaries.Clear();
                }
            }
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
                return this.CollectionView.SourceCollection is DataView;
            }
        }

        public bool IsDataViewWrapper
        {
            get { return this.CollectionView.SourceCollection is DataViewWrapperList; }
        }
        
        private DataTable GetClonedDataTable()
        {
            DataTable table, returnDT;            
            if (this.CollectionView.SourceCollection is DataView)
            {
                table = ((DataView)this.CollectionView.SourceCollection).Table;
            }
            else
            {
                table = this.CollectionView.SourceCollection as DataTable;
            }
            returnDT = table.Clone();
            if (returnDT.PrimaryKey != null)
                returnDT.PrimaryKey.Select(pk => pk.AutoIncrement = true).Count();
            return returnDT;
        }
#endif

        #endregion

        #endregion

        #region Details View Check

        /// <summary>
        /// Determines whether [has details view].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has details view]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasDetailsView()
        {
            return false;
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

    }

    internal static class SummaryCreator
    {
        #region Add on functions

        public static string ColumnName = "ColumnName";

        public static string Key = "Key";

        public static string ItemsCount = "ItemsCount";

        public static string GetSummaryDisplayTextForRow(SummaryRecordEntry summaryEntry, ICollectionViewAdv collectionView)
        {
            var summaryItems = summaryEntry.SummaryValues;
            var summaryRow = summaryEntry.SummaryRow;
            Dictionary<string, object> rowValues = new Dictionary<string, object>();
            summaryRow.SummaryColumns.ForEach<ISummaryColumn>(col =>
            {
                var item = summaryItems.FirstOrDefault(s => s.Name == col.Name);
                var value = SummaryCreator.GetFormattedSummary(item.AggregateValues, col, collectionView);
                rowValues.Add(col.Name, value);
            });
            var result = summaryRow.Title.FormatByName(rowValues);
            return result;
        }

        //public static string Get(SummaryRecordEntry summaryRecordEntry,ISummaryRow summaryRow,ICollectionViewAdv collectionView)
        //{
        //    string formatString = string.Empty;
        //    var summaryItems = summaryRecordEntry.SummaryValues;
        //    var captionText = summaryRow.Title;
        //    Dictionary<string, object> rowValues = new Dictionary<string, object>();
        //    summaryRow.SummaryColumns.ForEach<ISummaryColumn>(col =>
        //    {
        //        var item = summaryItems.FirstOrDefault(s => s.Name == col.Name);
        //        var value = SummaryCreator.GetFormattedSummary(item.AggregateValues, col,collectionView);
        //        rowValues.Add(col.Name, value);
        //    });
        //    captionText.ParseFormat(false, properties, out formatString, out pds);
        //    if (this.CanParseSummaryItemsForCaptionText(formatString, rowValues))
        //    {
        //        formatString = this.ParseFormatForCaptionText(formatString, rowValues);
        //    }
        //    return formatString;
        //}

        public static string GetSummaryDisplayText(SummaryRecordEntry summaryEntry, string columnName, ICollectionViewAdv collectionView)
        {
            string result = string.Empty;
            var summaryRow = summaryEntry.SummaryRow;
            var summaryCol = summaryRow.SummaryColumns.FirstOrDefault(s => s.MappingName == columnName);
            var summaryItems = summaryEntry.SummaryValues;
            if (summaryItems != null && summaryCol != null)
            {
                var item = summaryItems.FirstOrDefault(s => s.Name == summaryCol.Name);
                result = SummaryCreator.GetFormattedSummary(item.AggregateValues, summaryCol, collectionView);
            }
            return result;
        }

        public static string GetSummaryDisplayText(SummaryRecordEntry summaryEntry, string columnName, ICollectionViewAdv collectionView, Group group)
        {
            string result = string.Empty;
            var summaryRow = summaryEntry.SummaryRow;
            var summaryCol = summaryRow.SummaryColumns.FirstOrDefault(s => s.MappingName == columnName);
            var summaryItems = summaryEntry.SummaryValues;
            if (summaryItems != null && summaryCol != null)
            {
                var item = summaryItems.FirstOrDefault(s => s.Name == summaryCol.Name);
                result = SummaryCreator.GetFormattedSummary(item.AggregateValues, summaryCol, collectionView, group);
            }
            return result;
        }

        public static object RaiseQuerySummaryAggregate(IEnumerable items, ISummaryColumn summaryColumn, Dictionary<string, object> summaries, ICollectionViewAdv collectionView)
        {
            var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, collectionView);
            //var pd = this.Table.ItemProperties.Find(summaryColumn.MappingName, false);
            //if (pd != null)
            //{
            var result = SummaryCreator.GetFormattedSummary(items, summaryAggregate, summaries, summaryColumn, collectionView);
            //}

            return result;
        }

        internal static object GetFormattedSummary(IEnumerable items, ISummaryAggregate summaryAggregate, Dictionary<string, object> summaries, ISummaryColumn summaryColumn, ICollectionViewAdv collectionView)
        {
            string parsedFormat = string.Empty;

            object[] objs = SummaryCreator.GetSummary(items, summaryAggregate, summaries, summaryColumn, out parsedFormat, collectionView) as object[];
            if (objs != null)
            {
                return string.Format(parsedFormat, objs);
            }
            return string.Empty;
        }

        internal static string GetFormattedSummary(Dictionary<string, object> summaryItems, ISummaryColumn summaryColumn, ICollectionViewAdv collectionView)
        {
            var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, collectionView);
            if (summaryAggregate != null && summaryItems.Count > 0)
            {
                string parsedFormat = string.Empty;
                SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat);
                var objs = summaryItems.Values.ToArray();
                return string.Format(parsedFormat, objs);
            }

            return string.Empty;
        }

        internal static string GetFormattedSummary(Dictionary<string, object> summaryItems, ISummaryColumn summaryColumn, ICollectionViewAdv collectionView, Group group)
        {
            var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, collectionView);
            if (summaryAggregate != null)
            {
                string parsedFormat = string.Empty;
                SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat, collectionView, group);
                var objs = summaryItems.Values.ToArray();
                return string.Format(parsedFormat, objs);
            }

            return string.Empty;
        }


#if !SILVERLIGHT
        internal static PropertyDescriptor[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat, ICollectionView collectionView, Group group)
#else
        internal static PropertyInfo[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat, ICollectionView collectionView, Group group)
#endif
        {
            parsedFormat = string.Empty;
#if !SILVERLIGHT
            var pdc = TypeDescriptor.GetProperties(summaryAggregate);
            var groupProperty = TypeDescriptor.GetProperties(typeof(SortColumn)).Find(ColumnName, false);
            var keyProperty = TypeDescriptor.GetProperties(typeof(Group)).Find(Key, false);
            var countProperty = TypeDescriptor.GetProperties(typeof(Group)).Find(ItemsCount, false);
            List<PropertyDescriptor> al = new List<PropertyDescriptor>();
#else
            var pdc = summaryAggregate.GetType().GetProperties();
            var sortProperty = typeof(SortColumn).GetProperties();
            var groupProperty = sortProperty.FirstOrDefault(p => p.Name == "ColumnName");
            var propertyInfoArray = group.GetType().GetProperties();
            var keyProperty = propertyInfoArray.FirstOrDefault(p => p.Name == "Key");
            var countProperty = propertyInfoArray.FirstOrDefault(p => p.Name == "ItemsCount");
            var al = new List<PropertyInfo>();
#endif
            int n1 = SummaryColumn.Format.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(SummaryColumn.Format);
            }
            else
            {
                sb.Append(SummaryColumn.Format.Substring(0, n1 + 1));
            }

            int count = 0;

            while (n1 != -1)
            {

                n2 = SummaryColumn.Format.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = SummaryColumn.Format.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = SummaryColumn.Format.Substring(n1 + 1, n3 - n1 - 1);
#if !SILVERLIGHT
                    var pd = pdc[name];
#else
                    var pd = pdc.FirstOrDefault(p => p.Name == name);
#endif
                    if (pd != null)
                    {
                        sb.Append((al.Count - count).ToString());
                        al.Add(pd);
                    }
                    else
                    {
                        if (raiseException)
                            throw new FormatException("FilterPredicates not found: " + name);
                        else
                        {
                            //                            SummaryColumn.Format = string.Empty;
                            //                            // this.pds = new PropertyDescriptor[0];
                            //#if !SILVERLIGHT
                            //                            return new PropertyDescriptor[0];
                            //#else
                            //                            return new PropertyInfo[0];
                            //#endif

#if !SILVERLIGHT

                            if (name == ColumnName)
                            {
                                al.Add(groupProperty);
                                count++;
                            }
                            else if (name == Key)
                            {
                                al.Add(keyProperty);
                                count++;
                            }
                            else if (name == ItemsCount)
                            {
                                al.Add(countProperty);
                                count++;
                            }

#else
                            SummaryColumn.Format = string.Empty;
                            return new PropertyInfo[0];
#endif
                        }
                    }
                    n1 = SummaryColumn.Format.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(SummaryColumn.Format.Substring(n3));
                    }
                    else
                    {
                        sb.Append(SummaryColumn.Format.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        SummaryColumn.Format = string.Empty;
                        // this.pds = new PropertyDescriptor[0];
#if !SILVERLIGHT
                        return new PropertyDescriptor[0];
#else
                        return new PropertyInfo[0];
#endif
                    }
                    //					sb.Append(Format.Substring(n1));
                    //					n1 = -1;
                }

            }
            parsedFormat = sb.ToString();

            foreach (var pd in al)
            {
                if (pd.Name == ColumnName)
                {
                    int index = al.IndexOf(pd);
                    var groupedCol = collectionView.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                    var value = groupedCol.PropertyName;
                    var rindex = parsedFormat.IndexOf("{}");
                    parsedFormat = parsedFormat.Remove(rindex, 2);
                    if (rindex != 0)
                    {
                        parsedFormat = parsedFormat.Insert(rindex - 1, value);
                    }
                    else
                    {
                        parsedFormat = parsedFormat.Insert(rindex, value);
                    }
                }

                if (pd.Name == Key)
                {
                    int index = al.IndexOf(pd);
                    var value = pd.GetValue(group);
                    var rindex = parsedFormat.IndexOf("{}");
                    parsedFormat = parsedFormat.Remove(rindex, 2);
                    parsedFormat = parsedFormat.Insert(rindex - 1, value.ToString());
                }

                if (pd.Name == ItemsCount)
                {
                    int index = al.IndexOf(pd);
                    var value = pd.GetValue(group);
                    parsedFormat = parsedFormat.Replace("{}", value.ToString());
                }
            }
            al.Remove(groupProperty);
            al.Remove(keyProperty);
            al.Remove(countProperty);

            return al.ToArray();
        }

#if !SILVERLIGHT
        internal static PropertyDescriptor[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat)
#else
        internal static PropertyInfo[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat)
#endif
        {
            parsedFormat = string.Empty;
#if !SILVERLIGHT
            var pdc = TypeDescriptor.GetProperties(summaryAggregate);
            List<PropertyDescriptor> al = new List<PropertyDescriptor>();
#else
            var pdc = summaryAggregate.GetType().GetProperties();
            var al = new List<PropertyInfo>();
#endif

            string SummaryColumnFormat = SummaryColumn.Format;
            SummaryColumnFormat = SummaryColumnFormat.Replace("{ColumnName}", "");
            SummaryColumnFormat = SummaryColumnFormat.Replace("{Key}", "");
            SummaryColumnFormat = SummaryColumnFormat.Replace("{ItemsCount}", "");

            int n1 = SummaryColumnFormat.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(SummaryColumnFormat);
            }
            else
            {
                sb.Append(SummaryColumnFormat.Substring(0, n1 + 1));
            }

            while (n1 != -1)
            {
                n2 = SummaryColumnFormat.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = SummaryColumnFormat.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = SummaryColumnFormat.Substring(n1 + 1, n3 - n1 - 1);
#if !SILVERLIGHT
                    var pd = pdc[name];
#else
                    var pd = pdc.FirstOrDefault(p => p.Name == name);
#endif
                    if (pd != null)
                    {
                        sb.Append(al.Count.ToString());
                        al.Add(pd);
                    }
                    else
                    {
                        if (raiseException)
                            throw new FormatException("FilterPredicates not found: " + name);
                        else
                        {
                            // SummaryColumn.Format = string.Empty;
                            // this.pds = new PropertyDescriptor[0];

#if !SILVERLIGHT
                            return new PropertyDescriptor[0];
#else
                            return new PropertyInfo[0];
#endif
                        }
                    }
                    n1 = SummaryColumnFormat.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(SummaryColumnFormat.Substring(n3));
                    }
                    else
                    {
                        sb.Append(SummaryColumnFormat.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        SummaryColumn.Format = string.Empty;
                        // this.pds = new PropertyDescriptor[0];
#if !SILVERLIGHT
                        return new PropertyDescriptor[0];
#else
                        return new PropertyInfo[0];
#endif
                    }
                    //					sb.Append(Format.Substring(n1));
                    //					n1 = -1;
                }
            }
            parsedFormat = sb.ToString();
            // this.pds = al.ToArray();
            return al.ToArray();
        }


        internal static object GetSummary(IEnumerable items, ISummaryAggregate summaryAggregate, Dictionary<string, object> result, ISummaryColumn summaryColumn, out string parsedFormat, ICollectionViewAdv collectionView)
        {
            parsedFormat = string.Empty;
            if (summaryAggregate == null)
            {
                return null;
            }

            var enumerator = items.GetEnumerator();
            var hasValue = enumerator.MoveNext();
            var unboundExpressionFunc = collectionView as IUnboundExpressionFunc;
            var pds = SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat);
            Delegate summaryDelg = null;
            var isSummaryExp = (summaryAggregate is ISummaryExpressionAggregate) && (unboundExpressionFunc != null && unboundExpressionFunc.GetExpressionFunc(summaryColumn.MappingName) != null);
#if !SILVERLIGHT && SyncfusionFramework4_0
            var parallizableSummary = summaryAggregate as ISummaryParallelizable;
            var parallizableView = collectionView as IParallelizableView;
            if (parallizableSummary != null && parallizableView != null)
            {
                parallizableSummary.CanParallelize = parallizableView.UsePLINQ;
            }
#endif
            if (isSummaryExp)
            {
                summaryDelg = ((ISummaryExpressionAggregate)summaryAggregate).CalculateAggregateExpressionFunc();
            }
            else
            {
                summaryDelg = summaryAggregate.CalculateAggregateFunc();
            }
            object[] objs = new object[pds.Length];
            try
            {
                for (int i = 0; i < pds.Length; i++)
                {
                    if (pds[i] != null)
                    {
                        if (hasValue)
                        {
                            if (!isSummaryExp)
                            {
                                // calculates the summaries and updates the properties
                                summaryDelg.DynamicInvoke(new object[] { items, summaryColumn.MappingName, pds[i] });
                            }
                            else
                            {
                                summaryDelg.DynamicInvoke(new object[] { items, summaryColumn.MappingName, unboundExpressionFunc.GetExpressionFunc(summaryColumn.MappingName), unboundExpressionFunc.GetTypeExpressionFunc(summaryColumn.MappingName), pds[i] });
                            }
#if !SILVERLIGHT
                            objs[i] = pds[i].GetValue(summaryAggregate);
#else
                            objs[i] = pds[i].GetValue(summaryAggregate, null);
#endif
                            if (result != null)
                            {
                                result.Add(pds[i].Name, objs[i]);
                            }
                        }
                        else
                        {
                            if (result != null)
                            {
                                if (summaryColumn.SummaryType == SummaryType.Custom &&summaryColumn.CustomAggregate != null)
                                {
                                    summaryDelg.DynamicInvoke(new object[] { items, summaryColumn.MappingName, pds[i] });
                                }
                                result.Add(pds[i].Name, pds[i].GetValue(summaryAggregate)); 
                            }
                        }
                    }
                    else
                    {
                        objs[i] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return objs;
        }

        public static ISummaryAggregate GetSummaryAggregate(ISummaryColumn summaryColumn, ICollectionViewAdv collectionView)
        {
            //var args = new QuerySummaryEventArgs() { Items = items, SummaryColumn = summaryColumn, SummaryRowName = summaryRowName };
            ISummaryAggregate summaryAggregate = null;
            // this.OnQuerySummaryCustomAggregate(args);
            //if (args.Handled && args.CustomSummaryAggregate != null)
            //{
            //    summaryAggregate = args.CustomSummaryAggregate;
            //}
            //if (collectionView.Records.Count == 0)
            //    return null;
            if (summaryColumn.SummaryType == SummaryType.Custom && summaryColumn.CustomAggregate != null)
            {
                summaryAggregate = summaryColumn.CustomAggregate;
            }
            else
            {
                summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn.SummaryType, collectionView);
            }

            return summaryAggregate;
        }

        private static ISummaryAggregate GetSummaryAggregate(SummaryType summaryType, ICollectionViewAdv collectionView)
        {
#if !SILVERLIGHT
            bool isLegacyDataTable = false;
            if (collectionView.SourceCollection is DataTable || collectionView.SourceCollection is DataView)
            {
                isLegacyDataTable = true;
            }

            if (!isLegacyDataTable)
            {
#endif
            switch (summaryType)
            {
                case SummaryType.CountAggregate:
                    return new CountAggregate();
                case SummaryType.DoubleAggregate:
                    return new DoubleAggregate();
                case SummaryType.Int32Aggregate:
                    return new Int32Aggregate();
            }
#if !SILVERLIGHT
            }
            else
            {
                var table = (collectionView.SourceCollection as DataView).Table;
                switch (summaryType)
                {
                    case SummaryType.CountAggregate:
                        return new DataTableCountAggregate(table);
                    case SummaryType.Int32Aggregate:
                        return new DataTableInt32Aggregate(table);
                    case SummaryType.DoubleAggregate:
                        return new DataTableDoubleAggregate(table);
                }
            }
#endif
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Implement this interface to control refresh done with the <see cref="TopLevelGroup"/> class.
    /// </summary>
    public interface IGroupRefresh
    {
        /// <summary>
        /// Refreshes the sorting order.
        /// </summary>
        void RefreshSortingOrder();

        /// <summary>
        /// Refreshes the filters.
        /// </summary>
        void RefreshFilters();

        /// <summary>
        /// returns an IDisposable object to specify controlled updates.
        /// </summary>
        /// <returns></returns>
        IDisposable DeferRefresh();
    }
}
