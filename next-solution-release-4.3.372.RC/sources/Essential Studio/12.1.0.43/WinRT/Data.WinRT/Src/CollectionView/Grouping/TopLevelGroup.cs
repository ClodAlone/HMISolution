#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Reflection;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using Syncfusion.Data.Extensions;
    using Syncfusion.Data.Helper;
#if WinRT
    using Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

#if WPF
using System.Data;
#endif

namespace Syncfusion.Data
{
    public class SummaryWrapperModel
    {
        public int Count { get; set; }
        public double Sum { get; set; }
        public double Average { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
    }
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
                this.CollectionView = null;
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
            get { return this.resetCache; }
            set { this.resetCache = value; }
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

            this.RefreshSortingOrderForGroups();
            this.ResetDisplayElements();
        }

        private void RefreshSortingOrderForGroups()
        {
            var sortkey = this.GetSortKey(0);
            if (sortkey.PropertyName != null)
            {
                this.RefreshSortingOrderForGroups(this, sortkey);
            }
            this.SortForAllGroups(this.Groups);
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

        private void SortForAllGroups(IEnumerable<Group> groups)
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
            IComparer<Group> groupComparer = null;
            if (this.CollectionView.GroupComparer != null)
            {
                groupComparer = this.CollectionView.GroupComparer;
                if (groupComparer is ISortDirection)
                    (groupComparer as ISortDirection).SortDirection = sortKey.Direction;
            }
            else
            {
                if (this.CollectionView.SortComparers != null &&
                    this.CollectionView.SortComparers[sortKey.PropertyName] != null)
                {
                    groupComparer = this.CollectionView.SortComparers[sortKey.PropertyName] as IComparer<Group>;
                    if (groupComparer is ISortDirection)
                        (groupComparer as ISortDirection).SortDirection = sortKey.Direction;
                }
                else
                    groupComparer = new GroupComparer() {SortDirection = sortKey.Direction};
            }

            group.Groups.Sort(groupComparer);
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
                this.Groups.SetDirty();
                this.UpdateCaptionSummaries();
                this.ResetDisplayElements();
            }
        }

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
            var item = this.CollectionView.Records.CreateRecordEntry(record);
            this.DisplayElements.Add(item, isInSourceCollectionChange);
        }

        public void Insert(object record, int index, bool isInSourceCollectionChange)
        {
#if WPF      
            if (this.CollectionView.IsLegacyDataTable)
            {
                if (record is DataRowView && ((DataRowView)record).Row.RowState == DataRowState.Detached)
                {
                    return;
                }
            }
#endif
            var item = this.CollectionView.Records.CreateRecordEntry(record);
            this.DisplayElements.InsertRecord(item, index, isInSourceCollectionChange);
            //var parentGroup = item.Parent as Group;
            //if (parentGroup != null)
            //{
            //    while (parentGroup.Parent != null)
            //    {
            //        parentGroup = parentGroup.Parent;
            //        parentGroup.SetDirty();
            //    }
            //}
        }

        /// <summary>
        /// Removes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public virtual int Remove(object record, bool isInSourceCollectionChange)
        {
            return this.RemoveRecord(record, isInSourceCollectionChange);
        }

        internal virtual void ResetGroup(object record, string propertyName)
        {
            this.DisplayElements.ResetGroup(this, record, true, propertyName);
        }

        private int RemoveRecord(object record, bool isInSourceCollectionChange)
        {
            var index = (this.CollectionView as CollectionViewAdv).IndexOf(record);
            if (index < 0)
            {
                if (isInSourceCollectionChange)
                    this.DisplayElements.RemoveItem(this, record);
                return -1;
            }

            var item = this.CollectionView.GetRecordAt(index);
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
            return -1;
        }

        internal int AdjustBeforeAdd(Group group, object record)
        {
            var collectionview = this.CollectionView as CollectionViewAdv;
            var comparerIndex = collectionview.GetComparerIndex(group.Records, record, group.Records.Count, false);
            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }
            return comparerIndex;    
        }

        internal int GetComparerIndex(Group group, object record, int index)
        {
            var collectionview = this.CollectionView as CollectionViewAdv;
            var comparerIndex = collectionview.GetComparerIndex(group.Records, record, index, true);
            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }
            return comparerIndex;
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
            return this.IndexOf(record) != -1;
        }

        /// <summary>
        /// Finds the index of the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public virtual int IndexOf(object record)
        {
            int index = -1;
            var item = GetItem(record, this, ref index);
            return item != null ? index : -1;
        }

        private RecordEntry GetItem(object record, Group group, ref int index)
        {
            RecordEntry item = null;
            foreach (var innergroup in group.Groups)
            {
                index++;
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
                if (item != null)
                {
                    break;
                }
            }
            return item;
        }

        internal void RaiseCollectionChanged(NotifyCollectionChangedAction action, int startIndex, List<NodeEntry> changeditems)
        {
            if (this.IsSuspend)
            {
                return;
            }

            if (startIndex < 0)
                return;

            var arg = new NotifyCollectionChangedEventArgs(action, changeditems, startIndex);
            (this.CollectionView as CollectionViewAdv).RaiseGroupCollectionChanged(arg);
        }

        internal void UpdateCollectionViewRecords(NotifyCollectionChangedAction action, RecordEntry record)
        {
            var collectionview = this.CollectionView as CollectionViewAdv;
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    collectionview.Records.Add(record);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    collectionview.Records.Remove(record);
                    break;
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
                            for (int idx = 0; idx < group.GetRecordCount(); idx++)
                            {
                                var rec = group.GetRecordAt(idx);
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
                    count += group.GetRecordCount();
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

        internal bool CheckKey(Group group, object record)
        {
            if (record == null)
            {
                return false;
            }

            var pgd = this.CollectionView.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            object value = null;

            var converter = pgd.Converter;
            var provider = this.CollectionView.GetPropertyAccessProvider();

            value = converter == null
                        ? provider.GetValue(record, pgd.PropertyName)
                        : converter.Convert(record, group.Key != null ? group.Key.GetType() : typeof (object), null,
                                            CultureInfo.CurrentCulture.GetCulture());

#if WinRT
            if ((value == null && group.Key == null) || (value is Nullable && group.Key is Nullable) || (!(value is IComparable) && !(group.Key is IComparable)))
#else
            if ((value == null && group.Key == null) || (value is DBNull && group.Key is DBNull) || (!(value is IComparable) && !(group.Key is IComparable)))
#endif
            {
                return true;
            }
#if WinRT
            else if (value == null || group.Key == null || value is Nullable || group.Key is Nullable || !(value is IComparable) || !(group.Key is IComparable))
#else
            else if (value == null || group.Key == null || value is DBNull || group.Key is DBNull || !(value is IComparable) || !(group.Key is IComparable))
#endif
            {
                return false;
            }
            var c = ((IComparable)group.Key).CompareTo(value);
            return c == 0;
        }

        internal bool IsValidGroup(Group group, object record)
        {
            if (record == null)
                return false;

            var pgd = this.CollectionView.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            object value = null;

            var converter = pgd.Converter;
            var provider = this.CollectionView.GetPropertyAccessProvider();

            value = converter == null
                        ? provider.GetValue(record, pgd.PropertyName)
                        : converter.Convert(record, group.Key != null ? group.Key.GetType() : typeof(object), null,
                                            CultureInfo.CurrentCulture.GetCulture());

#if WinRT
            if ((value == null && group.Key == null) || (value is Nullable && group.Key is Nullable) || (!(value is IComparable) && !(group.Key is IComparable)))
#else
            if ((value == null && group.Key == null) || (value is DBNull && group.Key is DBNull) || (!(value is IComparable) && !(group.Key is IComparable)))
#endif
            {
                return true;
            }
#if WinRT
            else if (value == null || group.Key == null || value is Nullable || group.Key is Nullable || !(value is IComparable) || !(group.Key is IComparable))
#else
            else if (value == null || group.Key == null || value is DBNull || group.Key is DBNull || !(value is IComparable) || !(group.Key is IComparable))
#endif
            {
                return false;
            }
            var c = ((IComparable)group.Key).CompareTo(value);
            if (c == 0 && !(group.Parent is TopLevelGroup))
                return this.IsValidGroup(group.Parent, record);
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
        /// <returns></returns>
        public KeyValuePair<string, List<String>> GetGroupCaptionTextList(Group group, string groupSpecifierText, string columnHeaderName)
        {
            var properties = this.GetCaptionProperties(group);
#if WPF
            PropertyDescriptor[] pds = null;
#else
            PropertyInfo[] pds = null;
#endif
            string formatString = string.Empty;
            var flag = false;

            if (group.SummaryDetails != null)
            if (this.CollectionView.CaptionSummaryRow != null && group.SummaryDetails.SummaryValues.Count > 0)
            {
                var captionSummaryRow = this.CollectionView.CaptionSummaryRow;
                {
                    var captionText = this.CollectionView.CaptionSummaryRow.Title;
                    var summaryRecordEntry = group.SummaryDetails;
                    if (summaryRecordEntry != null)
                    {
                        var summaryItems = summaryRecordEntry.SummaryValues;
                        var summaryRow = captionSummaryRow;
                        var rowValues = new Dictionary<string, object>();
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

            var objs = new List<string>();
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null)
                {
                    var pd = pds[i];
                    object value = null;
                    if (pd.Name == "ColumnName")
                    {
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

            var kvp = new KeyValuePair<string, List<string>>(formatString, objs);
            return kvp;
        }

        private bool CanParseSummaryItemsForCaptionText(string value, Dictionary<string, object> summaryColumns)
        {
            int n1 = value.IndexOf("{");
            var sb = new StringBuilder();
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

#if WPF
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
#else
        private Dictionary<string, PropertyInfo> GetCaptionProperties(Group group)
        {
            var properties = new Dictionary<string, PropertyInfo>();
            var sortProperty = typeof (SortColumn).GetProperties();
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
            for (int i = maxLevel; i >= 0; i--)
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

        /// <summary>
        /// Update Caption summary from given group to first parent group.
        /// </summary>
        /// <param name="group"></param>
        /// <remarks></remarks>
        internal void UpdateCaptionSummariestoTopLevelGroup(Group group)
        {
            while (group != null && !group.IsTopLevelGroup)
            {
                this.UpdateCaptionSummaries(group);
                group = group.Parent;
            }
        }

        private void UpdateCaptionSummaries(IEnumerable<Group> groups, int level, ISummaryRow summaryRow)
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
#if !WinRT
                if (summaryColumn.SummaryType != SummaryType.Custom)
                {
                    var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, this.CollectionView);
                    string parsedFormat;
                    var property = string.Empty;
                    var pdsArray = SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat);
                    for (int i = 0; i < pdsArray.Length; i++)
                    {
                        var pd = pdsArray[i];
                        // generate records
                        var records = new List<object>();
#if WPF
                        DataView clonedView = null;
                        if (this.CollectionView.IsLegacyDataTable)
                        {
                            //clonedView = new DataView(this.GetClonedDataTable());
                            // Creating Temp DataStructure for storing summary values of bottom level group
                            var Dt = new DataTable();
                            Dt.TableName = "SummaryWrapperModel";
                            Dt.Columns.Add("Count", typeof(int));
                            Dt.Columns.Add("Sum", typeof(double));
                            Dt.Columns.Add("Average", typeof(double));
                            Dt.Columns.Add("Max", typeof(double));
                            Dt.Columns.Add("Min", typeof(double));
                            clonedView = new DataView(Dt);
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
#if WPF
                            if (!this.CollectionView.IsLegacyDataTable)
                            {
#endif
                                //record = underlyingType.CreateNew();
                                // Creating Temp DataStructure for storing summary values of bottom level group
                                record = new SummaryWrapperModel();
#if WPF
                            }
                            else
                                record = clonedView.AddNew();

                            var properties = TypeDescriptor.GetProperties(record);
                            var prop = properties.GetPropertyDescriptor("Count");
                            if (this.CollectionView.IsLegacyDataTable)
                                prop = properties.GetPropertyDescriptor(summaryColumn.MappingName);
#else
                            var properties = record.GetType().GetProperties();
                            var prop = properties.FirstOrDefault(p => p.Name == "Count");
#endif
                            var summaryValue = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                            object value = null;
                            if (summaryValue != null)
                            {
                                summaryValue.AggregateValues.TryGetValue(pd.Name, out value);
                                property = summaryValue.AggregateValues.FirstOrDefault().Key;
#if WPF
                                properties = TypeDescriptor.GetProperties(record);
                                prop = properties.GetPropertyDescriptor(property);
#else
                                properties = record.GetType().GetProperties();
                                prop = properties.FirstOrDefault(p => p.Name == property);
#endif
                                
                            }
                            if (value != null)
                            {
                                if (NullableHelperInternal.IsNullableType(prop.PropertyType))
                                {
                                    value = NullableHelperInternal.FixDbNUllasNull(value, prop.PropertyType);
                                }

                                value = NullableHelperInternal.ChangeType(value, prop.PropertyType);
#if WPF
                                prop.SetValue(record, value);
#else
                                prop.SetValue(record, value, null);
#endif
                            }
                            records.Add(record);
                        }
#if WPF
                        // workaround, just add an item if it is DataView. This caused a bug to exclude the last group in the subgroup.
                        if (this.CollectionView.IsLegacyDataTable)
                        {
                            // this would add a new item only in the view, and update the underlying cloned table with the RowState = Added, that way the summaries get the records computed properly. 
                            clonedView.AddNew();
                        }
#endif
                        // calc summaries
                        var enumerableSource = records.OfQueryable(typeof(SummaryWrapperModel));
#if WPF
                        if(this.CollectionView.IsLegacyDataTable)
                            enumerableSource = records.AsQueryable();
#endif
                        var actualSummaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, this.CollectionView);
                        var summaryDelg = (actualSummaryAggregate as ISummaryAggregateForGroup).CalculateAggregateFuncForGroup();
                        summaryDelg.DynamicInvoke(new object[] { enumerableSource, property, pd });
                        records.Clear();
#if WPF
                        var groupSummaryValue = pd.GetValue(actualSummaryAggregate);
#else
                        var groupSummaryValue = pd.GetValue(actualSummaryAggregate, null);
#endif
                        // add it to the group.SummaryDetails
                        var recordEntry = group.SummaryDetails as SummaryRecordEntry;
                        if (group.SummaryDetails == null)
                        {
                            recordEntry = new SummaryRecordEntry(group, group.Level) { SummaryRow = summaryRow };
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
#endif
                {
                    var records = new List<object>();
                    GetRecordsFromGroup(group, ref records);
                    var enumerableSource = records.OfQueryable(collection.SourceType);
                    var summaryRecordEntry = new SummaryRecordEntry(group, group.Level) {SummaryRow = summaryRow};

                    foreach (var summaryColumns in summaryRow.SummaryColumns)
                    {
                        if (summaryColumns.Name != string.Empty && summaryColumns.MappingName != string.Empty)
                        {
                            var summaryItems = new Dictionary<string, object>();
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
                    var rec = innerGroup.GetGroupItems();
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
                var records = group.GetGroupItems();
                if (records.Length > 0)
                {
                    var enumerableSource = records.OfQueryable(collection.SourceType);
                    var summaryRecordEntry = new SummaryRecordEntry(group, group.Level);
                    summaryRecordEntry.SummaryRow = summaryRow;
                    foreach (var summaryColumn in summaryRow.SummaryColumns)
                    {
                        if (summaryColumn.Name != string.Empty && summaryColumn.MappingName != string.Empty)
                        {
                            var summaryItems = new Dictionary<string, object>();
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

        protected virtual Type GetUnderlyingSourceType()
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
            if (this.IsSuspend)
            {
                return;
            }

            var collection = this.CollectionView as CollectionViewAdv;
            if (collection == null)
            {
                return;
            }

            if (group.IsBottomLevel)
            {
                var visibleSummaryRows = this.CollectionView.SummaryRows; //.Where(s => s.IsVisible);
                var groupRecordEntry = group.Details as GroupRecordEntry;
                if (visibleSummaryRows.Any())
                {                    
                    var records = group.GetGroupItems();
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
                                    var summaryItems = new Dictionary<string, object>();
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
#if WPF
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

        #region Details View Method

        public virtual int ExpandGroup(Group group)
        {
            this.ResetCache = true;
            int itemcount = 0;
            if (!group.IsExpanded)
            {
                group.IsExpanded = true;
                ComputeCount(group, ref itemcount);
            }

            return itemcount;
        }

        public virtual int CollapseGroup(Group group)
        {
            this.ResetCache = true;
            int itemcount = 0;
            if (group.IsExpanded)
            {
                ComputeCount(group, ref itemcount);
                group.IsExpanded = false;
            }
            return itemcount;
        }

        public void ComputeCount(Group group, ref int itemcount)
        {
            if (group.IsExpanded)
            {
                if (!group.IsBottomLevel)
                {
                    itemcount += group.GetGroupsCount();
                    foreach (var childGroup in group.Groups)
                        ComputeCount(childGroup, ref itemcount);
                }
                else
                {
                    //Calculating the row count of a expanded group
                    if (this.RelationsCount > 0)
                    {
                        var rCount = group.GetRelationsCount();
                        itemcount += (group.Records.Count * (rCount + 1)) + ((GroupRecordEntry)group.Details).Summaries.Count;
                    }
                    else
                    {
                        itemcount += group.Records.Count + ((GroupRecordEntry)group.Details).Summaries.Count;
                    }
                }
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

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
