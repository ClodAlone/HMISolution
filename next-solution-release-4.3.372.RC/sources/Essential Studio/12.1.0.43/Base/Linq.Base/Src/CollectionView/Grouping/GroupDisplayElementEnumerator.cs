#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System.Collections;
    using System.Collections.Generic;

    public class DisplayElementEnumerator : IEnumerator<NodeEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayElementEnumerator"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        public DisplayElementEnumerator(Group group)
        {
            this.Group = group;
            this.Helper = new TraversalHelper();
            if (group.GetGroupsCount() == 0)
            {
                this.next = null;
            }
            else
            {
                var firstGroup = group.Groups[0];
                int count = firstGroup.IsBottomLevel ? firstGroup.GetRecordCount() : firstGroup.GetGroupsCount();
                if (count == 0)
                {
                    this.next = this.Helper.GetNext(firstGroup);
                }
                else
                {
                    this.next = firstGroup;
                }
            }
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            private set;
        }

        private TraversalHelper Helper;

        private NodeEntry current;
        private NodeEntry next;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public NodeEntry Current
        {
            get
            {
                return ((IEnumerator)this).Current as NodeEntry;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.current = null;
            this.Group = null;
        }

        #endregion

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public bool MoveNext()
        {
            if (this.next == null)
            {
                return false;
            }
            this.current = this.next;
            this.next = Helper.GetNext(this.next);
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public void Reset()
        {
            this.next = this.Group.Groups[0];
            this.current = null;
        }
    }

    internal class TraversalHelper
    {
        public TraversalHelper() { this.ExpandAll = false; }

        public TraversalHelper(bool ExpandAll) { this.ExpandAll = ExpandAll; }

        private bool ExpandAll = false;


        #region GetNext

        public NodeEntry GetNext(NodeEntry entry)
        {
            NodeEntry nodeEntry = null;
            if (entry is Group)
            {
                var group = entry as Group;
                nodeEntry = this.GetNextItem(group);
            }
            else if (entry is RecordEntry)
            {
                var record = entry as RecordEntry;
                var group = record.Parent as Group;
                if (entry is SummaryRecordEntry)
                {
                    var summary = entry as SummaryRecordEntry;
                    var details = group.Details as GroupRecordEntry;
                    if (summary == details.Summaries[details.Summaries.Count - 1])
                    {
                        nodeEntry = this.GetNextGroup(group);
                    }
                    else
                    {
                        nodeEntry = details.Summaries[details.Summaries.IndexOf(summary) + 1];
                    }
                }
                else if (entry is NestedRecordEntry)
                {
                    var group1 = entry.Parent.Parent as Group;
                    int rcount = group1.GetRelationsCount();
                    if (((NestedRecordEntry)entry).NestedLevel == rcount)
                    {
                        int index = group1.Records.IndexOf((RecordEntry)entry.Parent);
                        int count = group1.Records.Count;
                        if (index == count - 1)
                        {
                            if (((GroupRecordEntry)group1.Details).Summaries.Count != 0)
                            {
                                nodeEntry = ((GroupRecordEntry)group1.Details).Summaries[0];
                            }
                            else
                            {
                                nodeEntry = this.GetNextGroup(group1);
                            }
                        }
                        else
                        {
                            nodeEntry = group1.Records[index + 1];
                        }
                    }
                    else
                    {
                        var nestedentry = entry as NestedRecordEntry;
                        if (record.IsExpanded)
                        {
                            bool flag = false;
                            foreach (var kvp in record.ChildViews)
                            {
                                if (kvp.Value == nestedentry)
                                {
                                    flag = true;
                                }

                                if (flag)
                                {
                                    nodeEntry = kvp.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    nodeEntry = GetNextRecord(group, record);
                }
            }
            else if (entry is SummaryRecordEntry)
            {
                var summary = entry as SummaryRecordEntry;
                var group=summary.Parent as Group;
                var details = group.Details as GroupRecordEntry;
                if (summary == details.Summaries[details.Summaries.Count - 1])
                {
                    nodeEntry = this.GetNextGroup(group);
                }
                else
                {
                    nodeEntry = details.Summaries[details.Summaries.IndexOf(summary) + 1];
                }
            }
            return nodeEntry;

        }

        private NodeEntry GetNextItem(Group group)
        {
            NodeEntry entry = null;
            if (group.IsExpanded || this.ExpandAll)
            {
                if (!group.IsBottomLevel)
                {
                    if (group.GetGroupsCount() == 0)
                    {
                        entry = GetNextGroup(group.Parent);
                    }
                    else
                    {
                        int count = group.Groups[0].IsBottomLevel ? group.Groups[0].GetRecordCount() : group.Groups[0].GetGroupsCount();
                        if (count == 0)
                        {
                            entry = this.GetNextGroup(group.Groups[0]);
                        }
                        else
                        {
                            entry = group.Groups[0];
                        }
                    }
                }
                else
                {
                    if (group.GetRecordCount() == 0)
                    {
                        entry = GetNextGroup(group);
                    }
                    else
                    {
                        entry = group.Records[0];
                    }
                }
            }

            if (!group.IsExpanded || this.ExpandAll)
            {
                if (entry == null)
                {
                    entry = this.GetNextGroup(group);
                }
            }

            return entry;
        }

        private NodeEntry GetNextGroup(Group group)
        {
            NodeEntry entry = null;
            Group parent = group.Parent;

            bool flag = false;
            while (group != null && parent != null)
            {
                if (parent.Groups.IndexOf(group) == parent.Groups.Count - 1)
                {
                    parent = parent.Parent;
                    group = group.Parent;
                }
                else
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                var index = parent.Groups.IndexOf(group) + 1;
                entry = parent.Groups[index];
                var nextgroup = parent.Groups[index];
                if (!nextgroup.IsBottomLevel)
                {
                    if (nextgroup.GetGroupsCount() == 0)
                    {
                        entry = GetNextGroup(nextgroup);
                    }
                }
                else
                {
                    if (nextgroup.GetRecordCount() == 0)
                    {
                        entry = GetNextGroup(nextgroup);
                    }
                }

            }


            return entry;
        }

        private NodeEntry GetNextRecord(Group group, RecordEntry record)
        {
            NodeEntry nodeEntry = null;
            var top = group.GetTopLevelGroup();
            // Details view check is applied to avoid returning the first child of the record for 
            // details veiw row
            if ((record.IsExpanded && !top.HasDetailsView()) || this.ExpandAll)
            {
                if (record.ChildViews != null)
                {
                    foreach (var kvp in record.ChildViews)
                    {
                        nodeEntry = kvp.Value;
                        break;
                    }
                }
            }

            if (!record.IsExpanded || this.ExpandAll)
            {
                int index = group.Records.IndexOf(record);
                if (index == group.Records.Count - 1)
                {
                    if (((GroupRecordEntry)group.Details).Summaries.Count != 0)
                    {
                        nodeEntry = ((GroupRecordEntry)group.Details).Summaries[0];
                    }
                    else
                    {
                        nodeEntry = this.GetNextGroup(group);
                    }
                }
                else
                {
                    nodeEntry = group.Records[index + 1];
                }
            }

            return nodeEntry;
        }

        #endregion

        #region Get Previous

        public NodeEntry GetPrevious(NodeEntry entry)
        {
            NodeEntry nodeEntry = null;

            if (entry is Group)
            {
                var group = entry as Group;
                if (group.Parent != null)
                {
                    var index = group.Parent.Groups.IndexOf(group);
                    if (index == 0)
                    {
                        nodeEntry = group.Parent;
                    }
                    else
                    {
                        nodeEntry = this.GetPreviousItem(group.Parent.Groups[index - 1]);
                    }
                }
            }
            else if (entry is RecordEntry)
            {
                if (entry is SummaryRecordEntry)
                {
                    Group recordparent = entry.Parent as Group;
                    int rcount = recordparent.GetRelationsCount();
                    GroupRecordEntry groupentry = recordparent.Details as GroupRecordEntry;
                    int index = groupentry.Summaries.IndexOf((SummaryRecordEntry)entry);
                    if (index == 0)
                    {
                        RecordEntry lastRecord = recordparent.Records[recordparent.Records.Count - 1];

                        if (lastRecord.IsExpanded)
                        {
                            foreach (var kvp in lastRecord.ChildViews)
                            {
                                nodeEntry = kvp.Value;
                            }
                        }

                        else
                        {
                            nodeEntry = lastRecord;
                        }
                    }
                    else
                    {
                        nodeEntry = groupentry.Summaries[index - 1];
                    }
                }
                else if (entry is NestedRecordEntry)
                {
                    var NestedEntry = entry as NestedRecordEntry;
                    if (NestedEntry.NestedLevel == 1)
                    {
                        nodeEntry = NestedEntry.Parent;
                    }
                    else
                    {
                        var record = NestedEntry.Parent as RecordEntry;
                        if (record.IsExpanded)
                        {
                            foreach (var kvp in ((RecordEntry)NestedEntry.Parent).ChildViews)
                            {
                                if (kvp.Value == NestedEntry)
                                {
                                    break;
                                }
                                nodeEntry = kvp.Value;
                            }
                        }
                    }
                }
                else
                {
                    var record = entry as RecordEntry;
                    var recordparent = record.Parent as Group;
                    var index = recordparent.Records.IndexOf(record);
                    if (index == 0)
                    {
                        nodeEntry = record.Parent;
                    }
                    else
                    {
                        var previousrecord = recordparent.Records[index - 1];
                        var rcount = recordparent.GetRelationsCount();
                        if (previousrecord.IsExpanded)
                        {
                            foreach (var kvp in previousrecord.ChildViews)
                            {
                                nodeEntry = kvp.Value;
                            }
                        }
                        else
                        {
                            nodeEntry = previousrecord;
                        }
                    }
                }
            }

            return nodeEntry;
        }

        public NodeEntry GetPreviousItem(Group previousgroup)
        {
            NodeEntry nodeEntry = null;
            if (previousgroup.IsExpanded)
            {
                if (previousgroup.IsBottomLevel)
                {
                    if (previousgroup.GetRecordCount() == 0)
                    {
                        nodeEntry = this.GetPrevious(previousgroup);
                    }
                    else
                    {
                        RecordEntry lastRecord = previousgroup.Records[previousgroup.Records.Count - 1];
                        GroupRecordEntry recordentry = previousgroup.Details as GroupRecordEntry;
                        int summaryCount = recordentry.Summaries.Count;
                        int rcount = previousgroup.GetRelationsCount();
                        if (summaryCount != 0)
                        {
                            nodeEntry = recordentry.Summaries[summaryCount - 1];
                        }
                        else if (lastRecord.IsExpanded)
                        {
                            foreach (var kvp in lastRecord.ChildViews)
                            {
                                nodeEntry = kvp.Value;
                            }
                        }
                        else
                        {
                            nodeEntry = lastRecord;
                        }
                    }
                }
                else
                {
                    int groupCount = previousgroup.GetGroupsCount();
                    if (groupCount == 0)
                    {
                        nodeEntry = this.GetPrevious(previousgroup);
                    }
                    else
                    {
                        nodeEntry = this.GetPreviousItem(previousgroup.Groups[groupCount - 1]);
                    }
                }
            }
            else
            {
                int Count = previousgroup.IsBottomLevel ? previousgroup.GetRecordCount() : previousgroup.GetGroupsCount();
                if (Count == 0)
                {
                    nodeEntry = this.GetPrevious(previousgroup);
                }
                else
                {
                    nodeEntry = previousgroup;
                }
            }

            return nodeEntry;
        }

        #endregion

        #region GetGroup
        public NodeEntry GetGroup(Group group, int index)
        {
          
            NodeEntry value = null;
            foreach (Group innergroup in group.Groups)
            {
                int count = innergroup.IsBottomLevel ? innergroup.GetRecordCount() : innergroup.GetGroupsCount();
                if (count != 0)
                {
                    if (index <= 0)
                    {
                        value = (NodeEntry)innergroup;
                        break;
                    }
                    else
                    {
                        if (innergroup.IsExpanded)
                        {
                            int yamount = innergroup.GetYAmountCache();
                            if (index <= yamount - 1)
                            {
                                index--;
                                if (innergroup.IsBottomLevel)
                                    value = GetRecord(innergroup, index);
                                else
                                    value = GetGroup(innergroup, index);
                                break;
                            }
                            else
                            {
                                index -= yamount - 1;
                            }
                        }
                        index--;
                    }
                }
            }

            return value;
        }

        private NodeEntry GetRecord(Group group, int index)
        {
            NodeEntry value = null;
            foreach (var record in group.Records)
            {
                if (index == 0)
                {
                    value = (NodeEntry)record;
                    break;
                }
                else
                {
                    index--;
                    int rcount = group.GetRelationsCount();
                    if (rcount != 0)
                    {
                        if (record.IsExpanded)
                        {
                            // temp variable to detect the existance of details view
                            bool hasDetails = group.GetTopLevelGroup().HasDetailsView();

                            // On Details view the index will alwasy have one additional value, to reduce that following condition is added
                            if (hasDetails && index > 0)
                                index--;

                            // On Details view when the index is 0 and record is in expand state, then the record is requested for the details row,
                            // hence break the loop to return null
                            else if (hasDetails && index == 0)
                                break;

                            foreach (var kvp in record.ChildViews)
                            {
                                if(index <= 0)
                                {
                                    value = kvp.Value;
                                    break;
                                }
                                index--;
                            }
                        }
                        else
                        {
                            index -= rcount;
                        }
                    }

                    if (value == null)
                    {
                        int recordIndex = group.Records.IndexOf(record);
                        if (recordIndex == group.Records.Count - 1)
                        {
                            if (((GroupRecordEntry)group.Details).Summaries != null)
                            {
                                foreach (var summary in ((GroupRecordEntry)group.Details).Summaries)
                                {
                                    if (index <= 0)
                                    {
                                        value = summary;
                                        break;
                                    }
                                    index--;
                                }
                            }
                        }
                    }
                    if (value != null)
                        break;
                }

            }
            return value;
        }
        #endregion
    }
}
