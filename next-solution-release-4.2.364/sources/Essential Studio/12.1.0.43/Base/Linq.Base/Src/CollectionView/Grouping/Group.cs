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
    using Syncfusion.Linq;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
using System.Windows.Data;

    public class Group : GroupEntry, IEnumerable<NodeEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public Group(Group parent, int level)
            : base(parent, level)
        {
            this.Parent = parent;
            this.IsBottomLevel = false;
            this.isDirty = false;
            this.isSourceYAmountDirty = false;
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
                this.toplevelGroup = null;
                if (this.Details.IsGroups)
                {
                    this.Groups.ForEach<Group>(g =>
                    {
                        g.Dispose();
                    });
                    this.Groups.Clear();
                }
                else if (this.Details.IsRecords)
                {
                    this.Records.ForEach<RecordEntry>(r =>
                    {
                        r.Dispose();
                    });
                    this.Records.Clear();
                }
                if (this.Source != null)
                {
                    this.Source.Clear();
                }
                /*if (this.CollectionViewGroup != null)
                {
                    this.CollectionViewGroup.Dispose();
                }*/
                GC.SuppressFinalize(this);
            }
        }

        /*public CollectionViewGroupRoot CollectionViewGroup
        {
            get;
            set;
        }*/

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public new Group Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the key for the Group.
        /// </summary>
        /// <value>The key.</value>
        public object Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the items count.
        /// </summary>
        /// <value>The items count.</value>
        public int ItemsCount
        {
            get
            {
                if (this.Groups != null)
                {
                    //return this.Groups.Count;
                    return this.GetGroupsCount();
                }
                else
                    return this.Records.Count;
            }
        }

        private bool isExpanded = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.GetTopLevelGroup().UpdateSummaries(this);
                    this.SetDirtyOnExpand();
                    if (value)
                    {
                        this.OnGroupExpanded();
                    }
                    else
                    {
                        this.OnGroupCollapsed();
                    }
                    this.RaisePropertyChanged(new PropertyChangedEventArgs("IsExpanded"));
                }
            }
        }

        private void SetDirtyOnExpand()
        {
            Group group = this;
            while (group != null)
            {
                group.SetDirty();
                group = group.Parent;
            }
        }

        /// <summary>
        /// Called when group.IsExpanded = true;.
        /// </summary>
        protected virtual void OnGroupExpanded()
        {
        }

        /// <summary>
        /// Called when group.IsExpanded = false;.
        /// </summary>
        protected virtual void OnGroupCollapsed()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is bottom level. Bottom-Level group will have the list of records for the Group.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is bottom level; otherwise, <c>false</c>.
        /// </value>
        public bool IsBottomLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is top level group. Top-Level Group will be the first-level group.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is top level group; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsTopLevelGroup
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Populates the specified groups to populate.
        /// </summary>
        /// <param name="groupsToPopulate">The groups to populate.</param>
        /// <returns></returns>
        public virtual int Populate(IEnumerable<GroupResult> groupsToPopulate)
        {
            if (groupsToPopulate != null)
            {
                this.yAmountCache = this.Populate(groupsToPopulate, this.Groups, this, this.Level + 1);
            }
            //this.CollectionViewGroup = new CollectionViewGroupRoot(this);            
            return this.yAmountCache;
        }


        /// <summary>
        /// Populates the specified groups to populate for the paging.
        /// </summary>
        /// <param name="groupsToPopulate">The groups to populate.</param>
        /// <param name="page"></param>
        /// <param name="IsViewLevelPaging"></param>
        /// <returns></returns>
        public virtual int Populate(IEnumerable<GroupResult> groupsToPopulate,PagedCollectionView page,bool IsViewLevelPaging)
        {
            if (groupsToPopulate != null)
            {
                this.yAmountCache = this.Populate(groupsToPopulate, this.Groups, this, this.Level + 1,page,IsViewLevelPaging);
            }
            //this.CollectionViewGroup = new CollectionViewGroupRoot(this);            
            return this.yAmountCache;
        }

       

        /// <summary>
        /// Populates the specified groups to populate.
        /// </summary>
        /// <param name="groupsToPopulate">The groups to populate.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        protected virtual int Populate(IEnumerable<GroupResult> groupsToPopulate, List<Group> groups, Group parent, int level)
        {
            var parentCounter0 = 0;
            var enumerator = groupsToPopulate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var groupResult = enumerator.Current;
                var group = this.CreateNewGroup(parent, groupResult, level);
                var counter0 = 1;
                groups.Add(group);
                if (groupResult.SubGroups == null)
                {
                    // create a record details collection
                    group.CreateDetailsForRecords(group, groupResult.Items, level);
                    // add the count of records
                    counter0 += group.Records.Count;
                    group.SetDirty();
                    group.SetSourceYAmountDirty();
                    group.GetYAmountCache();
                    group.GetSourceYAmountCache();
                    parentCounter0 += counter0;
                }
                else
                {
                    group.CreateDetailsForGroups(level);
                    counter0 += this.Populate(groupResult.SubGroups, group.Groups, group, level + 1);
                    group.SetDirty();
                    group.SetSourceYAmountDirty();
                    group.GetYAmountCache();
                    group.GetSourceYAmountCache();
                    parentCounter0 += counter0;
                    counter0 = 0;
                }
            }

            return parentCounter0;
        }

        
        
        /// <summary>
        /// Populates the specified groups to populate for paging.
        /// </summary>
        /// <param name="groupsToPopulate">The groups to populate.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        /// <param name="_page">PagedSource</param>
        /// <param name="IsViewLevePaging">IsViewLevelPaging</param>
        /// <returns></returns>
     
        protected virtual int Populate(IEnumerable<GroupResult> groupsToPopulate, List<Group> groups, Group parent, int level,PagedCollectionView _page,bool IsViewLevePaging)
        {
            var parentCounter0 = 0;
            var enumerator = groupsToPopulate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var groupResult = enumerator.Current;
                var lis = groupResult.Items.ToList<object>();
                foreach (var i in lis)
                {
                    if (_page.Contains(i))
                    {
                        var group = this.CreateNewGroup(parent, groupResult, level);
                        var counter0 = 1;

                        groups.Add(group);
                        if (groupResult.SubGroups == null)
                        {
                            // create a record details collection
                            group.CreateDetailsForRecords(group, groupResult.Items, level);
                            // add the count of records
                          
                            if (IsViewLevePaging)
                            {
                                for (int j = 0; j < group.Records.Count; j++)
                                    if (!_page.Contains(group.Records[j].Data))
                                    {
                                        group.Records.Remove(group.Records[j]);
                                        j--;
                                    }
                            } 
                            counter0 += group.Records.Count;
                            group.SetDirty();
                            group.SetSourceYAmountDirty();
                            group.GetYAmountCache();
                            group.GetSourceYAmountCache();
                            parentCounter0 += counter0;
                        }
                        else
                        {
                            group.CreateDetailsForGroups(level);
                            counter0 += this.Populate(groupResult.SubGroups, group.Groups, group, level + 1,_page,IsViewLevePaging);
                            group.SetDirty();
                            group.SetSourceYAmountDirty();
                            group.GetYAmountCache();
                            group.GetSourceYAmountCache();
                            parentCounter0 += counter0;
                            counter0 = 0;
                        }
                        break;
                    }
                }

            }
            return parentCounter0;
        }


        /// <summary>
        /// Creates the new group.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="groupResult">The group result.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        protected virtual Group CreateNewGroup(Group parent, GroupResult groupResult, int level)
        {
            return new Group(parent, level) { Key = groupResult.Key, toplevelGroup = this as TopLevelGroup/*ItemsCount = groupResult.Count*/ };
        }

        private TopLevelGroup toplevelGroup;

        public string GetSummaryValue(string ColumnName)
        {
            if (this.toplevelGroup != null && !string.IsNullOrEmpty(ColumnName) && this.SummaryDetails != null)
            {
                var summaryRecordEntry = this.SummaryDetails;
                if (summaryRecordEntry != null)
                    return SummaryCreator.GetSummaryDisplayText(summaryRecordEntry, ColumnName, this.toplevelGroup.CollectionView, this);
                else
                    return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates the new group.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="Key">The key.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public virtual Group CreateNewGroup(Group parent, object Key, int level)
        {
            return new Group(parent, level) { Key = Key, /*ItemsCount = 1 */};
        }

        /// <summary>
        /// Creates the details for records.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public virtual void CreateDetailsForRecords(Group parent, int level)
        {
            this.IsBottomLevel = true;
            this.Details = new GroupRecordEntry(parent, level);
        }

        /// <summary>
        /// Creates the details for records.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        public virtual void CreateDetailsForRecords(Group parent, IEnumerable source, int level)
        {
            this.IsBottomLevel = true;
            var topLevelGroup = this.GetTopLevelGroup();
            this.Details = new GroupRecordEntry(parent, level, source, topLevelGroup.CollectionView.PassesFilter);
        }

        /// <summary>
        /// Creates the details for groups.
        /// </summary>
        /// <param name="level">The level.</param>
        public virtual void CreateDetailsForGroups(int level)
        {
            this.Details = new GroupEntry(this.Parent, level);
        }

        private bool isDirty;
        /// <summary>
        /// Sets the dirty. When this is set to true, the YAmountCache will be re-computed for the whole group structure.
        /// </summary>
        public void SetDirty()
        {
            if (!this.isDirty)
            {
                this.isDirty = true;
            }
        }

        private bool isSourceYAmountDirty;
        internal void SetSourceYAmountDirty()
        {
            this.isSourceYAmountDirty = true;
        }

        private int yAmountCache = -1;
        private int sourceYAmountCache = -1;
        /// <summary>
        /// Each group knows the exact height of its child nodes. The YAmountCache returns the y height of each group based on its inner node levels.and their expanded states
        /// </summary>
        /// <returns>Calculated YAmount</returns>
        public int GetYAmountCache()
        {
            if (this.isDirty)
            {
                this.RecalculateYAmount();
                this.isDirty = false;
            }

            return this.yAmountCache;
        }

        /// <summary>
        /// Gets the source Y amount cache.
        /// </summary>
        /// <returns></returns>
        public int GetSourceYAmountCache()
        {
            if (this.isSourceYAmountDirty)
            {
                this.RecalculateSourceYAmount();
                this.isSourceYAmountDirty = false;
            }

            return this.sourceYAmountCache;
        }

        /// <summary>
        /// Gets the parent Y amount cache.
        /// </summary>
        /// <returns></returns>
        public int GetParentYAmountCache()
        {
            int totalYAmountCache = this.GetSourceYAmountCache();
            Group group = this.Parent as Group;
            while (group != null)
            {
                totalYAmountCache += group.GetSourceYAmountCache();
                group = group.Parent as Group;
            }

            return totalYAmountCache;
        }

        protected void ResetYAmount()
        {
            this.yAmountCache = 0;
        }

        protected void ResetSourceYAmount()
        {
            this.yAmountCache = 0;
            this.sourceYAmountCache = 0;
        }

        protected void RecalculateYAmount()
        {
            if (!this.isDirty)
            {
                throw new InvalidOperationException("Should not be called if already calculated");
            }

            var cachedCounter = 1;
            this.RecalculateYAmount(this, ref cachedCounter);
            this.yAmountCache = cachedCounter;
        }

        protected void RecalculateSourceYAmount()
        {
            if (!this.isSourceYAmountDirty)
            {
                throw new InvalidOperationException("Should not be called if already calculated");
            }

            var cachedCounter = 1;
            this.RecalculateSourceYAmount(this, ref cachedCounter);
            this.sourceYAmountCache = cachedCounter;
        }

        private void RecalculateSourceYAmount(Group group, ref int counter0)
        {
            if (!group.IsBottomLevel)
            {
                counter0 += group.GetGroupsCount();
                foreach (var innergroup in group.Groups)
                    this.RecalculateSourceYAmount(innergroup, ref counter0);
            }
            else
            {
                counter0 += group.GetRecordCount();
                if (((GroupRecordEntry)group.Details).Summaries != null)
                {
                    counter0 += ((GroupRecordEntry)group.Details).Summaries.Count;
                }
            }
        }

        private void RecalculateYAmount(Group group, ref int counter0)
        {
            if (group.IsExpanded)
            {
                if (group.IsBottomLevel)
                {
                    var recordCount = group.GetRecordCount();
                    counter0 += recordCount;
                    if (((GroupRecordEntry)group.Details).Summaries != null && recordCount > 0)
                    {
                        counter0 += ((GroupRecordEntry)group.Details).Summaries.Count;
                    }
                }
                else
                {
                    counter0 += group.GetGroupsCount();
                    foreach (var innergroup in group.Groups)
                        this.RecalculateYAmount(innergroup, ref counter0);
                }
            }
        }

        /// <summary>
        /// Gets or sets the details for this group. This could be Groups / Records collection.
        /// </summary>
        /// <value>The details.</value>
        public NodeEntry Details
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the records when Group.Details is list of records.
        /// </summary>
        /// <value>The records.</value>
        public IList<RecordEntry> Records
        {
            get
            {
                if (this.Details.IsRecords)
                {
                    return ((GroupRecordEntry)this.Details).Records;
                }

                return null;
            }
        }
        public virtual void AddRecord(RecordEntry record, bool isInSourceCollectionChange)
        {
            if (!isInSourceCollectionChange)
                this.Records.Add(record);
            else
            {
                this.Records.Add(record);
                this.Source.Add(record.Data);
            }
        }

        public virtual void InsertRecord(int index, RecordEntry record, bool isInSourceCollectionChange)
        {
            if (!isInSourceCollectionChange)
                this.Records.Insert(index, record);
            else
            {
                this.Records.Insert(index, record);
                this.Source.Add(record.Data);
            }
        }

        public virtual void AddItem(object record)
        {
            this.Source.Add(record);
        }

        public virtual bool RemoveRecord(RecordEntry record, bool isInSourceCollectionChange)
        {
            if (!isInSourceCollectionChange)
                return this.Records.Remove(record);
            else
            {
                this.Source.Remove(record.Data);
                return this.Records.Remove(record);
            }
        }

        public virtual bool RemoveItem(object record)
        {
            return this.Source.Remove(record);
        }
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public List<object> Source
        {
            get
            {
                if (this.Details.IsRecords)
                {
                    if((this.Details as GroupRecordEntry) != null)
                    {
                        return ((GroupRecordEntry)this.Details).UnfilteredRecords;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Gets the groups when Group.Details is list of Groups.
        /// </summary>
        /// <value>The groups.</value>
        public new List<Group> Groups
        {
            get
            {
                if (this.Details.IsGroups)
                {
                    return ((GroupEntry)this.Details).Groups;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount()
        {
            GroupRecordEntry group1 = this.Details as GroupRecordEntry;
            return group1.GetRecordsCount();
        }

        /// <summary>
        /// Gets the groups count.
        /// </summary>
        /// <returns></returns>
        public int GetGroupsCount()
        {
            int FilteredCount = 0;
            GroupEntry group = this.Details as GroupEntry;
            if (this.Details.IsGroups)
            {
                foreach (var subgroup in this.Groups)
                {
                    bool flag1 = false;
                    this.CountOnFilter(subgroup, ref flag1);
                    if (!flag1)
                    {
                        FilteredCount++;
                    }
                }

                return group.Groups.Count - FilteredCount;
            }
            return 0;
        }

        internal void CountOnFilter(Group group, ref bool flag)
        {
            if (group.IsBottomLevel)
            {
                if (group.Records.Count != 0)
                {
                    flag = true;
                }
            }
            else
            {
                foreach (var innnergroup in group.Groups)
                {
                    this.CountOnFilter(innnergroup, ref flag);
                    if (flag)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the relations count.
        /// </summary>
        /// <returns></returns>
        public int GetRelationsCount()
        {
            GroupRecordEntry group1 = this.Details as GroupRecordEntry;
            return group1.GetRelationsCount();
        }

        #region IEnumerable<NodeEntry> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<NodeEntry> GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator() as IEnumerator<NodeEntry>;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new GroupEnumerator(this);
        }

        #endregion

        private void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.propertyChanged != null)
            {
                this.propertyChanged(this, args);
            }
        }

        private event EventHandler<PropertyChangedEventArgs> propertyChanged;
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged
        {
            add
            {
                //if (this.propertyChanged == null)
                //{
                propertyChanged += value;//value.MakeWeak<PropertyChangedEventArgs>(eh => propertyChanged -= eh);
                //}
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        /// <summary>
        /// Contains summary record for this group
        /// </summary>
        public SummaryRecordEntry SummaryDetails
        {
            get;
            set;
        }
    }

    /// <summary>
    /// IComparer implemented for <see cref="Group"/> class.
    /// </summary>
    public class GroupComparer : IComparer<Group>
    {
        public GroupComparer()
        {
            this.SortDirection = ListSortDirection.Ascending;
        }

        public ListSortDirection SortDirection
        {
            get;
            set;
        }

        #region IComparer<Group> Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(Group x, Group y)
        {
            int cmp = 0;
            if (x == null || y == null)
            {
                if (x == y)
                    cmp = 0;
                else if (x == null)
                    cmp = -1;
                else
                    cmp = 1;
            }
            else
            {
                var xIsNull = (x.Key == null) || (x.Key is DBNull) || !(x.Key is IComparable);
                var yIsNull = (y.Key == null) || (y.Key is DBNull) || !(y.Key is IComparable);
                if (xIsNull || yIsNull)
                {
                    if ((x.Key == null && y.Key == null) || (x.Key is DBNull && y.Key is DBNull) ||
                        (!(x.Key is IComparable) && !(y.Key is IComparable)))
                        cmp = 0;
                    else if (xIsNull)
                        cmp = -1;
                    else
                        cmp = 1;
                }
                else
                    cmp = ((IComparable)x.Key).CompareTo(y.Key);
            }
            if (this.SortDirection == ListSortDirection.Ascending)
                return cmp;
            return -cmp;
        }
        #endregion
    }
}