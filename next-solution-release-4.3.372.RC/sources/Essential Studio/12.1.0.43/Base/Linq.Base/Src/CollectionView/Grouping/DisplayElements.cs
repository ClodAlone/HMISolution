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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using Syncfusion.Linq;
#if !SILVERLIGHT
    using System.Data;
#endif

    /// <summary>
    /// Interface that exposes method to the TopLevelGroup for Add / Remove / Contains /
    /// IndexOf methods with the underlying bound object.
    /// </summary>
    /// <remarks>
    /// This interface will only interact with the underlying bound object that can be
    /// found in RecordEntry.Data.
    /// </remarks>
    public interface IGroupList
    {
        /// <summary>
        /// Adds the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="isInSourceCollectionChange"></param>
        void Add(object record, bool isInSourceCollectionChange);


        /// <summary>
        /// Insert the record in the specified position.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="index"></param>
        /// <param name="isInSourceCollectionChange"></param>
        void Insert(object record, int index, bool isInSourceCollectionChange);


        /// <summary>
        /// Removes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="isInSourceCollectionChange">if set to <c>true</c> [is in source collection change].</param>
        /// <returns></returns>
        int Remove(object record, bool isInSourceCollectionChange);

        /// <summary>
        /// Determines whether the record is found in the DisplayElements of the TopLevelGroup.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(object record);

        /// <summary>
        /// Finds the index of the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        int IndexOf(object record);
    }

    public class GroupDisplayElements : IDisposable, IList<NodeEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDisplayElements"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        public GroupDisplayElements(TopLevelGroup group)
        {
            this.TopLevelGroup = group;
            this.helper = new TraversalHelper();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // unwire all subgroups
            // this.UnwireRecursively(this.TopLevelGroup);
            // unwire toplevel group
            // this.UnwireGroup(this.TopLevelGroup);
            this.TopLevelGroup = null;
        }

        /*private void UnwireRecursively(Group group)
        {
            if (group.IsBottomLevel)
            {
                this.UnwireGroup(group);
            }
            else
            {
                foreach (var grp in group.Groups)
                {
                    this.UnwireRecursively(grp);
                }
                this.UnwireGroup(group);
            }
        }

        #region IsExpanded state

        private void WireGroup(Group groupKey)
        {
            groupKey.PropertyChanged += this.OnGroupPropertyChanged;
        }

        private void UnwireGroup(Group groupKey)
        {
            groupKey.PropertyChanged -= this.OnGroupPropertyChanged;
        }

        private void OnGroupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var groupKey = sender as Group;
            if (e.PropertyName == "IsExpanded")
            {
                var value = groupKey.IsExpanded;

                var groupIndex = this.IndexOf(groupKey);
                if (value)
                {
                    if (groupKey.Details.IsGroups)
                    {
                        ////var groupArray = groupKey.Groups.ToArray();
                        ////groupArray.ForEach<Group>(g => this.WireGroup(g));
                        ////this.innerList.InsertRange(groupIndex + 1, groupArray);
                        this.InsertRecursive(groupKey.Groups, groupIndex + 1);

                    }
                    else if (groupKey.Details.IsRecords)
                    {
                        this.InsertRecursiveRecords(groupKey, groupIndex + 1);

                    }
                }
                else
                {
                    if (groupKey.Details.IsGroups)
                    {
                        ////groupKey.Groups.ForEach<Group>(g => this.UnwireGroup(g));
                        ////this.innerList.RemoveRange(groupIndex + 1, groupKey.Groups.Count);
                        this.RemoveRecursive(groupKey.Groups, groupIndex + 1);
                    }
                    else if (groupKey.Details.IsRecords)
                    {
                        this.RemoveRecursiveRecords(groupKey, groupIndex + 1);
                    }
                }
            }
        }

        protected virtual int InsertRecursive(IList<Group> groups, int insertAt)
        {
            foreach (var group in groups)
            {
                ////this.innerList.Insert(insertAt, group);
                this.WireGroup(group);
                //// if already expanded, we simply insert the records
                var flag = false;
                if (group.IsExpanded && group.IsBottomLevel)
                {
                    insertAt = this.InsertRecursiveRecords(group, insertAt + 1);
                    flag = true;
                }
                else if (group.IsExpanded)
                {
                    insertAt = this.InsertRecursive(group.Groups, insertAt + 1);
                    flag = true;
                }

                if (!flag)
                {
                    insertAt += 1;
                }
            }

            return insertAt;
        }

        protected virtual int InsertRecursiveRecords(Group group, int insertAt)
        {
            int rCount = group.GetRelationsCount();
            var index = insertAt;
            foreach (var record in group.Records)
            {
                //// this.innerList.Insert(index, record);
                this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, record, index);
                index += 1;
                // //for (int i = 1; i <= rcount; i++)
                // //{
                // //    this.innerList.Insert(index + i, new NestedRecordEntry(record, record.Level, null) { NestedLevel = i });
                // //}
                //index += rCount + 1;
            }

            var totalCount = rCount + index;
            ////var summaries = ((GroupRecordEntry)group.Details).Summaries.ToArray();
            ////if (summaries.Length > 0)
            ////{
            ////    this.innerList.InsertRange(index, summaries);
            ////}
            ////totalCount += summaries.Length;
            return totalCount;
        }

        private void RemoveRecursiveRecords(Group groupKey, int removeAt)
        {
            var index = removeAt;
            foreach (var record in groupKey.Records)
            {
                //// this.innerList.Insert(index, record);
                this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, record, index);
                // //for (int i = 1; i <= rcount; i++)
                // //{
                // //    this.innerList.Insert(index + i, new NestedRecordEntry(record, record.Level, null) { NestedLevel = i });
                // //}
                index += 1;
            }

            var count = groupKey.Records.Count + ((GroupRecordEntry)groupKey.Details).Summaries.Count + groupKey.GetRelationsCount();
        }

        protected virtual void RemoveRecursive(IList<Group> groups, int removeAt)
        {
            foreach (var group in groups)
            {
                if (group.IsExpanded && group.IsBottomLevel)
                {
                    this.RemoveRecursiveRecords(group, removeAt + 1);
                }
                else if (group.IsExpanded)
                {
                    this.RemoveRecursive(group.Groups, removeAt + 1);
                }

                this.UnwireGroup(group);
            }
        }

        #endregion
        */

        /// <summary>
        /// Gets or sets the top level group.
        /// </summary>
        /// <value>The top level group.</value>
        public TopLevelGroup TopLevelGroup
        {
            get;
            private set;
        }

        private TraversalHelper helper;
        private int currentindex;
        private NodeEntry currentnodecache = null;

        #region IList<NodeEntry> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(NodeEntry item)
        {
            var index = 0;// this.GetIndex(item);
            if (item is Group)
            {
                index = GetIndex(item, true) + ((Group)item).Level - 1;
            }
            else
            {
                index = GetIndex(item, true);
            }

            if (index > this.Count - 1)
            {
                return -1;
            }

            return index;
        }

        private int GetIndex(NodeEntry item, bool useGroupLevel)
        {
            int index = 0;
            if (item == null)
            {
                return index;
            }
            if (item is RecordEntry)
            {
                var group = item.Parent as Group;
                if (!group.IsExpanded && useGroupLevel)
                {
                    return -1;
                }
                if (group.GetRelationsCount() == 0)
                {
                    index += group.Records.IndexOf(item as RecordEntry);
                    if (useGroupLevel)
                    {
                        index += group.Level;
                    }
                }
                else
                {
                    index += group.Records.IndexOf(item as RecordEntry) * (group.GetRelationsCount() + 1);
                    if (useGroupLevel)
                    {
                        index += group.Level;
                    }
                }

                return (index + GetIndex(group, useGroupLevel));
            }
            else if (item is SummaryRecordEntry)
            {
                var group = item.Parent as Group;

                if (group.GetRelationsCount() == 0)
                {
                    index += (group.Records.Count);
                    if (useGroupLevel)
                    {
                        index += group.Level;
                    }
                }
                else
                {
                    index += (group.Records.Count) * (group.GetRelationsCount() + 1);
                    if (useGroupLevel)
                    {
                        index += group.Level;
                    }
                }

                return (index + GetIndex(group, useGroupLevel));
            }
            else
            {
                var parentGroup = item.Parent as Group;
                if (parentGroup == null)
                {
                    return index;
                }
                foreach (Group group in parentGroup.Groups)
                {
                    // allow groups that don't have filters applied
                    bool flag1 = false;
                    parentGroup.CountOnFilter(group, ref flag1);
                    if (flag1)
                    {
                        if (group == item)
                        {
                            return (index + GetIndex(parentGroup, true));
                        }
                        else
                        {
                            if (group.GetYAmountCache() < 0)
                            {
                                group.SetDirty();
                            }

                            index += group.GetYAmountCache();
                        }
                    }
                }

                return index;
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, NodeEntry item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Data.NodeEntry"/> at the specified index.
        /// </summary>
        /// <value></value>
        public NodeEntry this[int index]
        {
            get
            {
                NodeEntry nodeEntry = null;
                if (index >= 0 && index < this.Count)
                {
                    if (this.TopLevelGroup.ResetCache)
                    {
                        this.currentnodecache = null;
                        this.currentindex = -2;
                        this.TopLevelGroup.ResetCache = false;
                    }
                    if (this.currentnodecache != null)
                    {
                        if (index == this.currentindex)
                        {
                            nodeEntry = this.currentnodecache;
                        }
                        else if (index == this.currentindex - 1)
                        {
                            nodeEntry = this.helper.GetPrevious(this.currentnodecache);
                        }
                        else if (index == this.currentindex + 1)
                        {
                            nodeEntry = this.helper.GetNext(this.currentnodecache);
                        }
                    }
                    if (nodeEntry == null)
                    {
                        nodeEntry = this.helper.GetGroup(this.TopLevelGroup, index);
                    }

                    this.currentnodecache = nodeEntry;

                    this.currentindex = index;
                }
                return nodeEntry;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<NodeEntry> Members

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(NodeEntry item)
        {
            this.Add(item, true);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(NodeEntry item, bool isInSourceCollectionChange)
        {
            var record = item as RecordEntry;
            var changedItems = new List<NodeEntry>();
            var startIndex = -1;
            if (record == null)
            {
                throw new InvalidOperationException("Item has to be of type RecordEntry");
            }
            this.AddRecord(this.TopLevelGroup, record,ref changedItems, ref startIndex, isInSourceCollectionChange);
            this.TopLevelGroup.SetDirty();
            if(startIndex != -1)
                this.TopLevelGroup.OnCollectionChanged(NotifyCollectionChangedAction.Add, changedItems, startIndex);
        }

        private bool AddRecord(Group baseGroup, RecordEntry record, ref List<NodeEntry> changedItems, ref int startIndex, bool isInSourceCollectionChange)
        {
            return this.InsertRecord(baseGroup, record, -1, ref changedItems, ref startIndex, isInSourceCollectionChange);
        }

        public void InsertRecord(NodeEntry item, int index, bool isInSourceCollectionChange)
        {
            var record = item as RecordEntry;
            var changedItems = new List<NodeEntry>();
            int startIndex = -1;
            if (record == null)
            {
                throw new InvalidOperationException("Item has to be of type RecordEntry");
            }

            this.InsertRecord(this.TopLevelGroup, record, index, ref changedItems, ref startIndex, isInSourceCollectionChange);
            this.TopLevelGroup.SetDirty();
            foreach(var grp in this.TopLevelGroup.Groups)
                this.TopLevelGroup.Invalidate(this.IndexOf(grp),1);
            if (startIndex != -1)
                this.TopLevelGroup.OnCollectionChanged(NotifyCollectionChangedAction.Add, changedItems, startIndex);
        }

        private bool InsertRecord(Group baseGroup, RecordEntry record, int index,ref List<NodeEntry> changedItems,ref int startIndex,  bool isInSourceCollectionChange)
        {
            var added = false;
            var maxLevel = this.TopLevelGroup.GroupDescriptions.Count;
            var collectionview = this.TopLevelGroup.CollectionView;
            var candd = true;
            if (isInSourceCollectionChange)
                candd = collectionview.PassesFilter(record.Data);

            foreach (var group in baseGroup.Groups)
            {
                if (!this.TopLevelGroup.CheckKey(group, record.Data))
                    continue;

                if (group.IsBottomLevel)
                {
                    added = true;
                    if (!candd)
                    {
                        group.AddItem(record.Data);
                        break;
                    }
                    int _startIndex = 0;
                    record.Parent = group;

                    var isexpanded = IsGroupInView(baseGroup);

                    var currentgroup = group;
                    while (isexpanded && currentgroup.Parent != null && currentgroup.ItemsCount == 0)
                    {
                        changedItems.Add(currentgroup);
                        currentgroup = currentgroup.Parent;
                    }

                    if (collectionview.SortDescriptions.Count > 0)
                        index = this.GetComparerIndex(group, record.Data);

                    if (index > -1 && index < group.Records.Count)
                    {
                        group.InsertRecord(index, record, isInSourceCollectionChange);
                        _startIndex = this.IndexOf(group) + index + 1;
                        this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, record);
                    }
                    else
                    {
                        group.AddRecord(record, isInSourceCollectionChange);
                        _startIndex = this.IndexOf(group) + group.GetRecordCount() + 1;
                        this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, record);
                    }

                    TopLevelGroup.SetDirty();
                    startIndex = _startIndex;
                    if (group.IsExpanded)
                        group.SetDirty();

                    if (group.IsExpanded && isexpanded)
                    {
                        changedItems.Add(record);
                        var groupRecordEntry = group.Details as GroupRecordEntry;

                        if (collectionview.SummaryRows.Count > 0)
                        {
                            this.TopLevelGroup.UpdateSummaries(group);
                            var summaryindex = this.IndexOf(groupRecordEntry.Summaries[0]);
                            this.TopLevelGroup.Invalidate(summaryindex, groupRecordEntry.Summaries.Count);
                        }

                        if (group.Records.Count == 1)
                            foreach (var summary in groupRecordEntry.Summaries)
                                changedItems.Add(summary);
                    }

                    this.TopLevelGroup.UpdateCaptionSummaries();
                    var temp = group;
                    while (temp.Parent != null)
                    {
                        this.TopLevelGroup.Invalidate(this.IndexOf(temp), 1);
                        temp = temp.Parent;
                    }
                    break;
                }
                else
                {
                    added = this.InsertRecord(group, record, index, ref changedItems, ref startIndex, isInSourceCollectionChange);
                    if (added)
                        break;
                }
            }

            if (!added)
            {
                var isexpanded = IsGroupInView(baseGroup);
                var newGroup = this.CreateNewGroups(baseGroup, maxLevel, record.Data, ref changedItems, ref startIndex, isexpanded);
                newGroup.CreateDetailsForRecords(newGroup, newGroup.Level);
                record.Parent = newGroup;
                if (candd)
                {
                    TopLevelGroup.SetDirty();
                    newGroup.AddRecord(record, isInSourceCollectionChange);
                    this.TopLevelGroup.UpdateCaptionSummaries(newGroup);
                    this.TopLevelGroup.Invalidate(this.IndexOf(newGroup), 1);

                    if (isexpanded && newGroup.IsExpanded)
                    {
                        this.TopLevelGroup.UpdateSummaries(newGroup);
                        var details = newGroup.Details as GroupRecordEntry;
                        changedItems.Add(record);
                        foreach (var summary in details.Summaries)
                            changedItems.Add(summary);
                    }
                    this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, record);
                    added = true;
                }
                else
                {
                    startIndex = -1;
                    changedItems.Clear();
                    newGroup.AddItem(record.Data);
                    TopLevelGroup.SetDirty();
                }
            }
            return added;
        }

        private bool IsGroupInView(Group group)
        {
            while (group.Parent != null)
            {
                if (group.IsExpanded)
                {
                    group = group.Parent;
                    continue;
                }
                return false;
            }
            return true;
        }

        private bool AddItemToGroupSource(Group baseGroup, object record)
        {
            var added = false;
            var maxLevel = this.TopLevelGroup.GroupDescriptions.Count;

            foreach (var group in baseGroup.Groups)
            {
                if (!this.TopLevelGroup.CheckKey(group, record))
                    continue;

                if (group.IsBottomLevel)
                {
                    added = true;
                    group.AddItem(record);
                    break;
                }
                else
                {
                    added = this.AddItemToGroupSource(group, record);
                    if (added)
                        break;
                }
            }

            if (!added)
            {
                var changeditems = new List<NodeEntry>();
                int startIndex = -1;
                var newGroup = this.CreateNewGroups(baseGroup, maxLevel, record, ref changeditems, ref startIndex, false);
                newGroup.CreateDetailsForRecords(newGroup, newGroup.Level);
                newGroup.AddItem(record);
                this.TopLevelGroup.Invalidate(this.IndexOf(newGroup), 1);
                added = true;
            }
            return added;
        }

        private int GetComparerIndex(Group group, object record)
        {
            var view = this.TopLevelGroup.CollectionView;
            var comparer = new SortFieldComparer(view.SortDescriptions, view.SortComparers, view.Culture, (rec, propName) =>
            {
                var provider = view.GetPropertyAccessProvider();
                if (provider != null)
                {
                    return provider.GetValue(rec, propName);
                }

                return null;
            });
            var comparerIndex = Array.BinarySearch<object>(group.Records.ToArray(), record, comparer);
            if (comparerIndex < 0)
            {
                comparerIndex = ~comparerIndex;
            }
            return comparerIndex;
        }

        private int FindIndex(object data, Group group, SortDescription property)
        {
            int index = 0;
            bool isAscending = property.Direction == ListSortDirection.Ascending;
            //var itemProperties = this.TopLevelGroup.CollectionView.GetItemProperties();
            var view = this.TopLevelGroup.CollectionView;
            var provider = view.GetPropertyAccessProvider();
#if !SILVERLIGHT
            if (!this.TopLevelGroup.IsLegacyDataTable)
#endif
            {
                if (provider != null)
                {
                    var newValue = provider.GetValue(data, property.PropertyName);
                    if (!(newValue is DBNull) && newValue != null)
                    {
                        foreach (var record in group.Records)
                        {
                            var recordValue = provider.GetValue(record.Data, property.PropertyName);
                            if (!(recordValue is DBNull) && recordValue != null)
                            {
                                int c = ((IComparable)recordValue).CompareTo((IComparable)newValue);
                                if (Check(c, isAscending))
                                {
                                    index = group.Records.IndexOf(record);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        index = 0;
                    }
                }
            }
#if !SILVERLIGHT
            else
            {
                index = FindIndexForDataTable(data as DataRowView, isAscending, property.PropertyName, group);
            }
#endif
            return index;
        }

#if !SILVERLIGHT
        private int FindIndexForDataTable(DataRowView newRow, bool isAscending, string propertyName, Group group)
        {
            int index = -1;
            var newValue = newRow[propertyName];
            if (newValue != null)
            {
                foreach (var item in group.Records)
                {
                    var rowView = ((RecordEntry)item).Data as DataRowView;
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
                return c > 0;
            else
                return c < 0;
        }

        private object GetPropertyValue(int level, object data)
        {
            var enumerator = this.TopLevelGroup.GroupDescriptions.OfType<PropertyGroupDescription>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (level <= 0)
                {
                    break;
                }

                level--;
            }
            object value = null;
            var pgd = enumerator.Current as PropertyGroupDescription;
#if !SILVERLIGHT
            if (!this.TopLevelGroup.IsLegacyDataTable)
#endif
            {
                var provider = this.TopLevelGroup.CollectionView.GetPropertyAccessProvider();
                if (provider != null)
                {
                    value = provider.GetValue(data, pgd.PropertyName);
                }
            }
#if !SILVERLIGHT
            else
            {
                var rowView = data as DataRowView;
                value = rowView[pgd.PropertyName];
            }
#endif
            return value;
        }

        private Group CreateNewGroups(Group group, int maxlevel, object data, ref List<NodeEntry> changedItems, ref int startIndex, bool isexpanded)
        {
            var propertyGroupDescription = this.TopLevelGroup.GroupDescriptions[group.Level] as PropertyGroupDescription;
            var converter = propertyGroupDescription != null ? propertyGroupDescription.Converter : null;
            object key = converter == null ? this.GetPropertyValue(group.Level, data) : converter.Convert(data, null, null, this.TopLevelGroup.CollectionView.Culture);
            var newGroup = group.CreateNewGroup(group, key, group.Level + 1);
            SortDescription sortDesc;
            var topLevelGroup = group.GetTopLevelGroup();
            var pgd = topLevelGroup.CollectionView.GroupDescriptions[group.Level] as PropertyGroupDescription;
            sortDesc = topLevelGroup.CollectionView.SortDescriptions.FirstOrDefault(s => s.PropertyName == pgd.PropertyName);
            var index = Array.BinarySearch<Group>(group.Groups.ToArray(), newGroup, new GroupComparer() { SortDirection = sortDesc != null ? sortDesc.Direction : ListSortDirection.Ascending });
            if (index < 0)
            {
                index = ~index;
            }

            var newPositionIndex = 0;
            if (group.IsExpanded)
            {
                for (int i = index - 1; i < group.Groups.Count && i >= 0; i--)
                {
                    var grp = group.Groups[i];
                    if (grp.IsExpanded)
                    {
                        newPositionIndex += grp.GetYAmountCache();
                    }
                    else
                    {
                        // including the group row
                        newPositionIndex += 1;
                    }
                }
                group.SetDirty();
                startIndex = newPositionIndex;
                if (isexpanded)
                    changedItems.Add(newGroup);
            }

            group.Groups.Insert(index, newGroup);
            //group.CollectionViewGroup.InsertNewGroup(index,newGroup);
            // this.WireGroup(newGroup);
            group.SetDirty();
            group.SetSourceYAmountDirty();
            if (this.TopLevelGroup.CollectionView.IsGroupsExpanded)
                newGroup.IsExpanded = true;
            if (newGroup.Level != maxlevel)
            {
                newGroup.CreateDetailsForGroups(newGroup.Level);
                newGroup = this.CreateNewGroups(newGroup, maxlevel, data, ref changedItems, ref startIndex, isexpanded);
            }
            return (Group)newGroup;
        }

        void ICollection<NodeEntry>.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(NodeEntry item)
        {
            return ((Group)item.Parent).Records.Contains((RecordEntry)item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(NodeEntry[] array, int arrayIndex)
        {
            var enumerator = this.GetEnumerator();
            this.SetIndex(this.helper.GetGroup(this.TopLevelGroup, arrayIndex), enumerator);
            while (enumerator.MoveNext())
            {
                array[arrayIndex] = enumerator.Current;
                arrayIndex++;
            }
        }

        private void SetIndex(NodeEntry node, IEnumerator<NodeEntry> enumerator)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == node)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return this.TopLevelGroup.GetYAmountCache() - 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(NodeEntry item)
        {
            var removedAt = this.RemoveNode(item, true);
            return removedAt > -1;
        }

        public int RemoveNode(NodeEntry item, bool isInSourceCollectionChange)
        {
            var changedItems = new List<NodeEntry>();
            int startIndex = -1;
            // find the index and return this
            var removedAt = this.GetIndex(item, false);
            var lastRemoved = this.RemoveNode(item, ref changedItems, ref startIndex, isInSourceCollectionChange);
            this.TopLevelGroup.SetDirty();
            Group group = null;
            if (changedItems.Count != 0)
            {
                if (changedItems.Count == 1)
                {
                    group = (Group)changedItems[0].Parent;
                }
                else
                {
                    group = (Group)changedItems[changedItems.Count - 1].Parent;
                }

                if (startIndex != -1)
                {
                    this.TopLevelGroup.OnCollectionChanged(NotifyCollectionChangedAction.Remove, changedItems, startIndex);
                }

                var groupRecordEntry = group.Details as GroupRecordEntry;
                if (groupRecordEntry != null)
                {
                    if (groupRecordEntry.Summaries.Count > 0)
                    {
                        this.TopLevelGroup.UpdateSummaries(group);
                        var summaryIndex = this.IndexOf(groupRecordEntry.Summaries[0]);
                        this.TopLevelGroup.Invalidate(summaryIndex, groupRecordEntry.Summaries.Count);
                    }
                }

                // If we delete all records in a group then TopLevelGroup becomes null and Raises the Exception. So here i added a condition Check.
                if (this.TopLevelGroup != null)
                    foreach (var gp in this.TopLevelGroup.Groups)
                        if (gp.Records != null && gp.Records.Count > 0)
                            this.TopLevelGroup.UpdateCaptionSummaries(gp);
            }

            while (lastRemoved.Parent != null)
            {
                lastRemoved = lastRemoved.Parent;

                if (this.TopLevelGroup != null)
                    this.TopLevelGroup.Invalidate(this.IndexOf(lastRemoved), 1);
            }

            foreach (var grp in this.TopLevelGroup.Groups)
                this.TopLevelGroup.Invalidate(this.IndexOf(grp), 1);
            return removedAt;
        }

        private NodeEntry RemoveNode(NodeEntry item, ref List<NodeEntry> changedItems, ref int startIndex,
                                     bool isInSourceCollectionChange)
        {
            var lastRemoved = item;
            var parent = item.Parent as Group;
            if (parent == null)
                throw new InvalidOperationException("Parent is Null for the removing item");

            startIndex = this.IndexOf(item);
            if (this.currentnodecache == item)
                this.currentnodecache = null;
            
            if (parent.IsExpanded)
                changedItems.Add(item);
            else
                changedItems.Clear();

            if (item is RecordEntry && parent.IsBottomLevel)
            {
                var recordEntry = (RecordEntry) item;
                var iscorrectGroup = false;

                var canUpdateSource = isInSourceCollectionChange;
                if (!isInSourceCollectionChange)
                {
                    iscorrectGroup = this.TopLevelGroup.IsCorrectGroup(parent, recordEntry.Data); //CheckKey(parent, recordEntry.Data);
                    canUpdateSource = !iscorrectGroup;
                }

                if (canUpdateSource)
                {
                    if (parent.Records.Count == 1)
                    {
                        var details = parent.Details as GroupRecordEntry;
                        if (parent.IsExpanded)
                        {
                            foreach (var summary in details.Summaries)
                            {
                                changedItems.Add(summary);
                            }
                        }
                        details.Summaries.Clear();
                        lastRemoved = this.RemoveNode(parent, ref changedItems, ref startIndex,
                                                      isInSourceCollectionChange);
                    }
                    else
                        parent.SetDirty();
                }
                else
                {
                    parent.SetDirty();
                    lastRemoved = item;
                }
                parent.RemoveRecord(recordEntry, canUpdateSource);
                this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, recordEntry);
                if (!isInSourceCollectionChange && !iscorrectGroup)
                    AddItemToGroupSource(TopLevelGroup, recordEntry.Data);
            }
            else
            {
                if (parent.Groups.Count == 1)
                {
                    if (parent is TopLevelGroup)
                    {
                        parent.Groups.Remove((Group) item);
                    }
                    lastRemoved = this.RemoveNode(parent, ref changedItems, ref startIndex, isInSourceCollectionChange);
                }
                else
                {
                    parent.SetDirty();
                    lastRemoved = item;
                    parent.Groups.Remove((Group) item);
                }
            }
            return lastRemoved;
        }

        internal bool ResetGroup(Group baseGroup, object record, bool isReset, string propertyName)
        {
            Group Group1 = null;
            foreach (var group in baseGroup.Groups)
            {
                var pgd = this.TopLevelGroup.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                if (group.Records.Count != 0)
                    continue;
                if (pgd.PropertyName.Equals(propertyName) || string.IsNullOrEmpty(propertyName))
                {
                    if (group.IsBottomLevel)
                    {
                        if (!group.Source.Contains(record))
                            continue;
                    }
                    else
                    {
                        if (this.ResetGroup(group, record, isReset, propertyName))
                            return true;
                        continue;
                    }
                }
                else
                {
                    if (!this.TopLevelGroup.CheckKey(group, record))
                        continue;
                }
                if (group.IsBottomLevel)
                    Group1 = group;
                break;
            }

            if (Group1 != null)
            {
                Group1.RemoveItem(record);
                if (Group1.GetRecordCount() == 0 && Group1.Source.Count == 0)
                {
                    while (Group1.Parent != null)
                    {
                        if (Group1.Parent.Groups.Count == 1)
                        {
                            if (Group1.Parent is TopLevelGroup)
                            {
                                Group1.Parent.Groups.Remove(Group1);
                                break;
                            }
                            Group1 = Group1.Parent;
                        }
                        else
                        {
                            Group1.Parent.Groups.Remove(Group1);
                            break;
                        }
                    }
                }
                if (isReset)
                    AddItemToGroupSource(TopLevelGroup, record);
            }
            return true;
        }

        internal bool RemoveItem(Group baseGroup, object record)
        {
            Group Group1 = null;
            foreach (var group in baseGroup.Groups)
            {
                if (!this.TopLevelGroup.CheckKey(group, record))
                    continue;
                if (group.IsBottomLevel)
                    Group1 = group;
                break;
            }

            if (Group1 != null)
            {
                var removed = Group1.RemoveItem(record);
                if (Group1.GetRecordCount() == 0 && Group1.Source.Count == 0)
                {
                    while (Group1.Parent != null)
                    {
                        if (Group1.Parent.Groups.Count == 1)
                        {
                            if (Group1.Parent is TopLevelGroup)
                            {
                                Group1.Parent.Groups.Remove(Group1);
                                break;
                            }
                            Group1 = Group1.Parent;
                        }
                        else
                        {
                            Group1.Parent.Groups.Remove(Group1);
                            break;
                        }
                    }
                }
                return removed;
            }
            else
            {
                ResetGroup(TopLevelGroup, record, false, string.Empty);
            }
            return false;
        }

        #endregion

        #region IEnumerable<NodeEntry> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<NodeEntry> GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this).GetEnumerator() as IEnumerator<NodeEntry>;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new DisplayElementEnumerator(this.TopLevelGroup);
        }

        #endregion
    }
}