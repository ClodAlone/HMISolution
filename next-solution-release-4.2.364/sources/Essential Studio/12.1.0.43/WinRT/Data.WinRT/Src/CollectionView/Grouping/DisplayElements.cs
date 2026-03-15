#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Syncfusion.Data.Extensions;
#if WinRT
using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
#endif

namespace Syncfusion.Data
{
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
        /// <param name="isInSourceCollectionChange"></param>
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
            this.TopLevelGroup = null;
        }

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

        #region Virtual Methods

        protected virtual NodeEntry GetItemAt(int index)
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

        #endregion

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
                index = GetIndex(item, true) + item.Level - 1;
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
                    index += group.Records.IndexOf(item as RecordEntry);
                else
                    index += group.Records.IndexOf(item as RecordEntry)*(group.GetRelationsCount() + 1);

                if (useGroupLevel)
                    index += group.Level;

                return (index + GetIndex(group, useGroupLevel));
            }
            else if (item is SummaryRecordEntry)
            {
                var group = item.Parent as Group;

                if (group.GetRelationsCount() == 0)
                    index += (group.Records.Count);
                else
                    index += (group.Records.Count)*(group.GetRelationsCount() + 1);

                if (useGroupLevel)
                    index += group.Level;

                return (index + GetIndex(group, useGroupLevel));
            }
            else
            {
                var parentGroup = item.Parent as Group;
                if (parentGroup == null)
                {
                    return index;
                }
                foreach (var group in parentGroup.Groups)
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
        /// Gets or sets the <see cref="Syncfusion.WinRT.Data.NodeEntry"/> at the specified index.
        /// </summary>
        /// <value></value>
        public NodeEntry this[int index]
        {
            get
            {
                return GetItemAt(index);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<NodeEntry> Members

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
            if (record == null)
            {
                throw new InvalidOperationException("Item has to be of type RecordEntry");
            }
            this.AddRecord(this.TopLevelGroup, record,isInSourceCollectionChange);
        }

        public void InsertRecord(NodeEntry item, int index, bool isInSourceCollectionChange)
        {
            var record = item as RecordEntry;
            if (record == null)
            {
                throw new InvalidOperationException("Item has to be of type RecordEntry");
            }
            var changedItems = new List<NodeEntry>();
            var startIndex = -1;
            this.InsertRecord(this.TopLevelGroup, record, index, isInSourceCollectionChange, ref startIndex,
                              ref changedItems);
            this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, startIndex, changedItems);
        }

        private bool AddRecord(Group baseGroup, RecordEntry record, bool isInSourceCollectionChange)
        {
            var changedItems = new List<NodeEntry>();
            var startIndex = -1;
            var added = this.InsertRecord(baseGroup, record, -1, isInSourceCollectionChange, ref startIndex, ref changedItems);
            this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, startIndex, changedItems);
            return added;
        }

        private bool InsertRecord(Group baseGroup, RecordEntry record, int index, bool isInSourceCollectionChange,ref int startIndex, ref List<NodeEntry> changedItems)
        {
            var added = false;
            var maxLevel = this.TopLevelGroup.GroupDescriptions.Count;
            //PropertyGroupDescription pgd = null;
            var collectionview = this.TopLevelGroup.CollectionView as CollectionViewAdv;
            var candd = true;
            if (isInSourceCollectionChange)
                candd = collectionview.FilterRecord(record.Data);
            NotifyCollectionChangedEventArgs args = null;

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

                    record.Parent = group;
                    if (collectionview.SortDescriptions.Count > 0)
                    {
                        if (index > -1 && index < group.Records.Count)
                        {
                            group.InsertRecord(index, record, isInSourceCollectionChange);
                        }
                        else
                        {
                            if (collectionview.LiveDataUpdateMode == LiveDataUpdateMode.AllowDataShaping)
                                index = this.AdjustBeforeAdd(group, record.Data);
                            if (index > -1 && index < group.Records.Count)
                            {
                                group.InsertRecord(index, record, isInSourceCollectionChange);
                            }
                            else
                            {
                                group.AddRecord(record, isInSourceCollectionChange);
                            }
                        }
                    }
                    else
                    {
                        group.AddRecord(record, isInSourceCollectionChange);
                    }

                    TopLevelGroup.SetDirty();

                    var rootGroupExpanded = IsGroupInView(baseGroup);
                    if (rootGroupExpanded && group.Records.Count == 1)
                    {
                        startIndex = this.IndexOf(group);
                        changedItems.Add(group);
                    }

                    if (group.IsExpanded)// && rootGroupExpanded)
                    {
                        if (rootGroupExpanded)
                        {
                            if (changedItems.Count == 0)
                                startIndex = IndexOf(record);
                            changedItems.Add(record);
                        }
                        group.SetDirty();
                        var groupRecordEntry = group.Details as GroupRecordEntry;
                        if (this.TopLevelGroup.CollectionView.SummaryRows.Count > 0 && collectionview.LiveDataUpdateMode != LiveDataUpdateMode.Default)
                        {
                            collectionview.UpdateSummaries(group, record.Data, NotifyCollectionChangedAction.Add);
                            if(rootGroupExpanded)
                            {
                                var summaryindex = this.IndexOf(groupRecordEntry.Summaries[0]);
                                this.TopLevelGroup.Invalidate(summaryindex, groupRecordEntry.Summaries.Count);
                            }
                        }

                        if (group.Records.Count == 1 && rootGroupExpanded)
                        {
                            foreach (var summary in groupRecordEntry.Summaries)
                                changedItems.Add(summary);
                        }
                    }

                    if (collectionview.LiveDataUpdateMode != LiveDataUpdateMode.Default)
                    {
                        Group temp = group;
                        while (temp != null && !temp.IsTopLevelGroup)
                        {
                            if (temp.Parent.IsExpanded)
                            {
                                collectionview.UpdateCaptionSummaries(temp, record.Data,
                                                                      NotifyCollectionChangedAction.Add);
                                if (this.IsGroupInView(temp))
                                    this.TopLevelGroup.Invalidate(this.IndexOf(temp), 1);
                            }
                            temp = temp.Parent;
                        }
                    }
                    this.TopLevelGroup.UpdateCollectionViewRecords(NotifyCollectionChangedAction.Add, record);
                    break;
                }
                else
                {
                    added = this.InsertRecord(group, record, index, isInSourceCollectionChange,ref startIndex,ref changedItems);
                    if (added)
                        break;
                }
            }

            if (!added)
            {
                var rootGroupExpanded = IsGroupInView(baseGroup);
                if (!rootGroupExpanded)
                    startIndex = -2;
                var newGroup = this.CreateNewGroups(baseGroup, maxLevel, record.Data, ref changedItems, ref startIndex);
                newGroup.CreateDetailsForRecords(newGroup, newGroup.Level);
                
                record.Parent = newGroup;
                if (candd)
                {
                    if (!rootGroupExpanded)
                        changedItems.Clear();

                    TopLevelGroup.SetDirty();
                    newGroup.AddRecord(record, isInSourceCollectionChange);
                    UpdateCaptionSummariesWhenAdd(newGroup);
                    if (newGroup.IsExpanded)
                    {
                        this.TopLevelGroup.UpdateSummaries(newGroup);
                        if (rootGroupExpanded)
                        {
                            var groupRecordEntry = newGroup.Details as GroupRecordEntry;
                            changedItems.Add(record);
                            foreach (var summary in groupRecordEntry.Summaries)
                                changedItems.Add(summary);
                        }
                    }
                    TopLevelGroup.SetDirty();
                    this.TopLevelGroup.UpdateCollectionViewRecords(NotifyCollectionChangedAction.Add, record);
                    added = true;
                }
                else
                {
                    changedItems.Clear();
                    newGroup.AddItem(record.Data);
                    TopLevelGroup.SetDirty();
                }
                if (isInSourceCollectionChange)
                    TopLevelGroup.ResetCache = true;
            }
            return added;
        }

        private void UpdateCaptionSummariesWhenAdd(Group group)
        {
            if (group == null || group.IsTopLevelGroup)
                return;

            this.TopLevelGroup.UpdateCaptionSummaries(group);
            if (this.IsGroupInView(group))
            {
                this.TopLevelGroup.Invalidate(this.IndexOf(group), 1);
            }
            if (group.Parent != null)
                this.UpdateCaptionSummariesWhenAdd(group.Parent as Group);
        }

        private bool IsGroupInView(Group group)
        {
            if (group.IsTopLevelGroup)
                return true;

            group = group.Parent;
            while (!group.IsTopLevelGroup)
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
                var newGroup = this.CreateNewGroups(baseGroup, maxLevel, record, ref changeditems, ref startIndex);
                newGroup.CreateDetailsForRecords(newGroup, newGroup.Level);
                newGroup.AddItem(record);
                added = true;
                //TopLevelGroup.SetDirty();
            }
            return added;
        }

        private int AdjustBeforeAdd(Group group, object record)
        {
            return this.TopLevelGroup.AdjustBeforeAdd(group, record);
        }
        
        private bool Check(int c, bool isAscending)
        {
            return isAscending ? c > 0 : c < 0;
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
            {
                var provider = this.TopLevelGroup.CollectionView.GetPropertyAccessProvider();
                if (provider != null)
                {
                    value = provider.GetValue(data, pgd.PropertyName);
                }
            }
            return value;
        }

        private Group CreateNewGroups(Group group, int maxlevel, object data, ref List<NodeEntry> changedItems, ref int startIndex)
        {
            var propertyGroupDescription = this.TopLevelGroup.GroupDescriptions[group.Level] as PropertyGroupDescription;
            var converter = propertyGroupDescription != null ? propertyGroupDescription.Converter : null;
            object key = converter == null ? this.GetPropertyValue(group.Level, data) : converter.Convert(data, null, null, this.TopLevelGroup.CollectionView.Culture.GetCulture());

            Group newGroup = group.CreateNewGroup(group, key, group.Level + 1);

            if (TopLevelGroup.CollectionView.CaptionSummaryRow != null)
            {
                newGroup.SummaryDetails = new SummaryRecordEntry(newGroup, newGroup.Level) { SummaryRow = TopLevelGroup.CollectionView.CaptionSummaryRow };
            }

            SortDescription sortDesc;
            var topLevelGroup = group.GetTopLevelGroup();
            var pgd = topLevelGroup.CollectionView.GroupDescriptions[group.Level] as PropertyGroupDescription;
            sortDesc = topLevelGroup.CollectionView.SortDescriptions.FirstOrDefault(s => s.PropertyName == pgd.PropertyName);
            var index = Array.BinarySearch(group.Groups.ToArray(), newGroup, new GroupComparer() { SortDirection = sortDesc != null ? sortDesc.Direction : ListSortDirection.Ascending });
            if (index < 0)
            {
                index = ~index;
            }

            if (group.IsExpanded && startIndex != -2)
            {
                group.SetDirty();
                if (startIndex == -1)
                    startIndex = index;

                changedItems.Add(newGroup);

                if (this.TopLevelGroup.CollectionView.AutoExpandGroups)
                    newGroup.IsExpanded = true;
            }
            
            group.Groups.Insert(index, newGroup);
            group.SetDirty();
            group.SetSourceYAmountDirty();
           
            if (newGroup.Level != maxlevel)
            {
                newGroup.CreateDetailsForGroups(newGroup.Level);
                newGroup = this.CreateNewGroups(newGroup, maxlevel, data, ref changedItems, ref startIndex);
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
            // find the index and return this
            var removedAt = this.GetIndex(item, false);
            var changedItems = new List<NodeEntry>();
            var startIndex = -1;
            this.RemoveNode(item, isInSourceCollectionChange, ref startIndex, ref changedItems);
            //if (setdirty)
            //    this.TopLevelGroup.SetDirty();
            this.TopLevelGroup.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, startIndex, changedItems);
            return removedAt;
        }

        private NodeEntry RemoveNode(NodeEntry item, bool isInSourceCollectionChange,ref int startIndex, ref List<NodeEntry> changedItems)
        {
            var parent = item.Parent as Group;
            if (parent == null)
                throw new InvalidOperationException("Parent is Null for the removing item");
            
            if (this.currentnodecache == item)
                this.currentnodecache = null;

            if (item is RecordEntry && parent.IsBottomLevel)
            {
                var recordEntry = (RecordEntry)item;
                var iscorrectGroup = this.TopLevelGroup.IsValidGroup(parent, recordEntry.Data);
                NodeEntry lastremoved = parent;
                var canUpdateSource = isInSourceCollectionChange;
                if (!isInSourceCollectionChange)
                    canUpdateSource = !iscorrectGroup;

                var rootGroupExpanded = IsGroupInView(parent);

                if (parent.Records.Count == 1 && canUpdateSource)
                {
                    var details = parent.Details as GroupRecordEntry;
                    if (details != null)
                    {
                        if (rootGroupExpanded && parent.IsExpanded)
                        {
                            foreach (var summary in details.Summaries)
                                changedItems.Add(summary);
                        }
                        details.Summaries.Clear();
                    }
                    lastremoved = this.RemoveNode(parent, canUpdateSource, ref startIndex, ref changedItems);
                }

                if (rootGroupExpanded && parent.IsExpanded)
                {
                    if (startIndex == -1)
                        startIndex = IndexOf(item);
                    changedItems.Add(item);
                }

                if (recordEntry.IsExpanded)
                {
                    if (recordEntry.ChildViews.Count > 0)
                    {
                        foreach (var view in recordEntry.ChildViews)
                        {
                            changedItems.Add(view.Value);
                        }
                    }
                }
                parent.RemoveRecord(recordEntry, canUpdateSource);
                var collectionViewAdv = this.TopLevelGroup.CollectionView as CollectionViewAdv;

                if (parent.IsExpanded && parent.Records.Count > 0)
                {
                    var groupRecordEntry = parent.Details as GroupRecordEntry;
                    if (groupRecordEntry != null)
                    {
                        if (this.TopLevelGroup.CollectionView.SummaryRows.Count > 0)
                        {
                            if (collectionViewAdv != null &&
                                collectionViewAdv.LiveDataUpdateMode != LiveDataUpdateMode.Default)
                            {
                                collectionViewAdv.UpdateSummaries(parent, recordEntry.Data,
                                                                  NotifyCollectionChangedAction.Remove);
                            }
                            if (rootGroupExpanded)
                            {
                                var summaryIndex = this.IndexOf(groupRecordEntry.Summaries[0]);
                                this.TopLevelGroup.Invalidate(summaryIndex, groupRecordEntry.Summaries.Count);
                            }
                        }
                    }
                }

                this.TopLevelGroup.SetDirty();
                if (collectionViewAdv != null && collectionViewAdv.LiveDataUpdateMode != LiveDataUpdateMode.Default)
                {
                    var temp = lastremoved as Group;
                    while (temp != null && !temp.IsTopLevelGroup)
                    {
                        collectionViewAdv.UpdateCaptionSummaries(temp, recordEntry.Data, NotifyCollectionChangedAction.Remove);
                        if (this.IsGroupInView(temp))
                            this.TopLevelGroup.Invalidate(this.IndexOf(temp), 1);
                        temp = temp.Parent;
                    }
                }

                if (!isInSourceCollectionChange && !iscorrectGroup)
                    AddItemToGroupSource(TopLevelGroup, recordEntry.Data);

                this.TopLevelGroup.UpdateCollectionViewRecords(NotifyCollectionChangedAction.Remove,
                                                            (RecordEntry)item);
                
                return item;
            }
            else
            {
                if (parent.Groups.Count == 1)
                {
                    if (parent is TopLevelGroup)
                    {
                        if (this.IsGroupInView((Group) item))
                        {
                            changedItems.Insert(0, item);
                            startIndex = IndexOf(item);
                        }
                        parent.Groups.Remove((Group) item);
                        return item;
                    }
                    if (this.IsGroupInView((Group) item))
                        changedItems.Insert(0, item);
                    return this.RemoveNode(parent, isInSourceCollectionChange, ref startIndex, ref changedItems);
                }
                else
                {
                    if (this.IsGroupInView((Group)item))
                    {
                        changedItems.Insert(0, item);
                        startIndex = IndexOf(item);
                    }
                    parent.Groups.Remove((Group) item);
                    return item;
                }
            }
        }

        internal bool ResetGroup(Group baseGroup, object record, bool isReset, string propertyName)
        {
            if (baseGroup.IsBottomLevel)
            {
                baseGroup.RemoveItem(record);
                if (baseGroup.Records.Count == 0 && baseGroup.GetSourceCount() == 0)
                {
                    var group = baseGroup;
                    var parent = group.Parent;
                    while (parent != null)
                    {
                        parent = group.Parent;
                        if (parent.Groups.Count == 1)
                            parent.Groups.Remove(group);
                        else
                        {
                            parent.Groups.Remove(group);
                            break;
                        }
                    }
                }
                if (isReset)
                    AddItemToGroupSource(TopLevelGroup, record);
                return true;
            }

            foreach (var group in baseGroup.Groups)
            {
                var pgd = this.TopLevelGroup.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                if (group.Records!=null && group.Records.Count != 0)
                    continue;
                if (pgd.PropertyName.Equals(propertyName) || string.IsNullOrEmpty(propertyName))
                {
                    if (group.IsBottomLevel)
                    {
                        if (group.Source.Contains(record))
                            return this.ResetGroup(group, record, isReset, propertyName);
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
                    return this.ResetGroup(group, record, isReset, propertyName);
                }
            }
            return false;
        }

        internal bool RemoveItem(Group baseGroup, object record)
        {
            if (baseGroup.IsBottomLevel)
            {
                var removed = baseGroup.RemoveItem(record);
                if (baseGroup.Records.Count == 0 && baseGroup.GetSourceCount() == 0)
                {
                    while (baseGroup.Parent != null)
                    {
                        if (baseGroup.Parent.Groups.Count == 1)
                        {
                            if (baseGroup.Parent is TopLevelGroup)
                            {
                                baseGroup.Parent.Groups.Remove(baseGroup);
                                break;
                            }
                            baseGroup = baseGroup.Parent;
                        }
                        else
                        {
                            baseGroup.Parent.Groups.Remove(baseGroup);
                            break;
                        }
                    }
                }
                return removed;
            }
            else
            {
                foreach (var group in baseGroup.Groups)
                {
                    if (!this.TopLevelGroup.CheckKey(group, record))
                        continue;
                    return RemoveItem(group, record);
                }
            }
            return ResetGroup(TopLevelGroup, record, false, string.Empty);
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