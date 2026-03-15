//-------------------------------------------------------------------------------------------------
// <copyright file="GroupsDetails.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// The details section of a group with nested child groups.
    /// </summary>
    public class GroupsDetails : DetailsSection, IContainerElement
    {
        internal GroupCategoryTreeTable categoryTreeTable;
        GroupsInDetailsCollection _groups;
        internal GroupSortOrderTreeTable sortOrderTreeTable;
        internal SortColumnDescriptorCollection columnDescriptors;

        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GroupsDetails(Group parent)
            : base(parent)
        {
            categoryTreeTable = new GroupCategoryTreeTable(this);
            ////sortOrderTreeTable = new GroupSortOrderTreeTable(this);
            GroupSortOrderDirty = true;
            HasGroupSortOrder = false;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (categoryTreeTable != null)
                {
                    categoryTreeTable.Dispose();
                }

                categoryTreeTable = null;

                if (sortOrderTreeTable != null)
                {
                    sortOrderTreeTable.Dispose();
                }

                sortOrderTreeTable = null;

                _groups = null;
                columnDescriptors = null;
            }

            base.Dispose(disposing);
        }

        internal void InitializeComparer(SortColumnDescriptorCollection columnDescriptors)
        {
            if (columnDescriptors == null)
            {
                throw new ArgumentNullException("columnDescriptors");
            }

            this.columnDescriptors = columnDescriptors;
            categoryTreeTable.Comparer = columnDescriptors.GetGroupedColumnsComparer();
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            if (displayOrder && sortOrderTreeTable != null)
            {
                return sortOrderTreeTable;
            }

            return categoryTreeTable;
        }

        /// <summary>
        /// Set this true when records in this group were modified. Will be reset in OnEnsureInitialized.
        /// When set GroupSortOrderTreeTable will be recreated.
        /// </summary>
        /// <internalonly/>
        public bool GroupSortOrderDirty
        {
            get
            {
                return this.Reserved11;
            }

            set
            {
                ////                if (Reserved11 != value)
                {
                    Reserved11 = value;
                    ////                    if (value && this.HasGroupSortOrder)
                    ////                    {
                    ////                        this.ParentGroup.InvalidateCounterBottomUp();
                    ////                        this.ParentTable.DisplayElements.ClearCache();
                    ////                    }
                }
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool HasGroupSortOrder
        {
            get
            {
                return Reserved2;
            }

            set
            {
                Reserved2 = value;
            }
        }

        void SortCategories(SortColumnDescriptorCollection compareColumnDescriptors, SortColumnDescriptorCollection columnDescriptors, Group[] array)
        {
            bool reverseOnly = compareColumnDescriptors.Count == this.columnDescriptors.Count;
            bool isEqual = reverseOnly;   // isEqual = true when GroupSortOrderDirty was set (SortOrder might change when SortOrderComparer was set)

            for (int n = 0; n < compareColumnDescriptors.Count; n++)
            {
                SortColumnDescriptor cd1 = compareColumnDescriptors[n];
                SortColumnDescriptor cd2 = this.columnDescriptors[n];

                bool b1 = cd1.Comparer == cd2.Comparer && cd1.Name == cd2.Name;
                reverseOnly &= b1 && cd1.SortDirection != cd2.SortDirection;
                isEqual &= b1 && cd1.SortDirection == cd2.SortDirection;
            }

            this.columnDescriptors = compareColumnDescriptors;
            categoryTreeTable.Comparer = compareColumnDescriptors.GetGroupedColumnsComparer();

            if (reverseOnly)
            {
                // Just reverse the existing sorting.
                categoryTreeTable.Clear();

                categoryTreeTable.BeginInit();

                for (int n = array.Length - 1; n >= 0; n--)
                {
                    GroupCategoryTreeTableEntry groupCategoryEntry = new GroupCategoryTreeTableEntry();
                    groupCategoryEntry.Tree = categoryTreeTable.TreeTable;
                    groupCategoryEntry.Element = array[n];
                    array[n].GroupCategoryEntry = groupCategoryEntry;
                    array[n].ParentElement = this;
                    categoryTreeTable.Add(groupCategoryEntry);
                }

                categoryTreeTable.EndInit();
            }
            else if (!isEqual)
            {
                // Resort the whole array.
                Array.Sort(array, new GroupsDetailsSortColumnsComparer(this.columnDescriptors));

                categoryTreeTable.Clear();

                categoryTreeTable.BeginInit();

                for (int n = 0; n < array.Length; n++)
                {
                    GroupCategoryTreeTableEntry groupCategoryEntry = new GroupCategoryTreeTableEntry();
                    groupCategoryEntry.Tree = categoryTreeTable.TreeTable;
                    groupCategoryEntry.Element = array[n];
                    array[n].GroupCategoryEntry = groupCategoryEntry;
                    array[n].ParentElement = this;
                    categoryTreeTable.Add(groupCategoryEntry);
                }

                categoryTreeTable.EndInit();
            }
        }

        Group[] CreateGroupArray()
        {
            Group[] array = new Group[categoryTreeTable.Count];
            int n = 0;
            foreach (GroupCategoryTreeTableEntry groupCategoryEntry in categoryTreeTable)
            {
                array[n] = groupCategoryEntry.Element;
                n++;
            }

            return array;
        }

        /// <summary>
        /// This virtual method is called from <see cref="OnEnsureInitialized"/> and
        /// lets derived elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the <see cref="EnsureInitialized"/> call.</param>
        /// <returns>
        /// True if changes were detected and the object was updated; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            bool retVal = false;

            // on demand sorting of groups when they are touched.
            if (this.columnDescriptors != null)
            {
                bool shouldSort = false;
                SortColumnDescriptorCollection compareColumnDescriptors;
                Group pg = this.ParentGroup;
                if (pg.IsMainGroup && ParentTableDescriptor.RelationChildColumns.Count > 0)
                {
                    compareColumnDescriptors = this.ParentTableDescriptor.RelationChildColumns.GetShadowedCopy();
                }
                else if (this.ParentTableDescriptor.GroupedColumns.Count > GroupLevel)
                {
                    compareColumnDescriptors = this.ParentTableDescriptor.GroupedColumns[GroupLevel].GetShadowedCopy();
                }
                else
                {
                    compareColumnDescriptors = SortColumnDescriptorCollection.Empty;
                }

                shouldSort = !this.columnDescriptors.Equals(compareColumnDescriptors);

                HasGroupSortOrder = compareColumnDescriptors.Count == 1 && compareColumnDescriptors[0].GroupSortOrderComparer != null;

                Group[] array = null;

                if (HasGroupSortOrder)
                {
                    bool notified = false;

                    // Fix Categories
                    if (shouldSort)
                    {
                        if (categoryTreeTable.TreeTable.GetCount() <= 1)
                        {
                            this.columnDescriptors = compareColumnDescriptors;
                        }
                        else
                        {
                            notified = true;
                            ParentTable.RaiseSortingItemsInGroup(ParentGroup);
                            array = CreateGroupArray();
                            SortCategories(compareColumnDescriptors, columnDescriptors, array);

                            retVal = true;
                        }
                    }

                    //// Fix Group Sort Order

                    if (shouldSort || this.GroupSortOrderDirty)
                    {
                        if (!notified)
                        {
                            ParentTable.RaiseSortingItemsInGroup(ParentGroup);
                            array = CreateGroupArray();
                            this.columnDescriptors = compareColumnDescriptors;
                        }

                        notified = true;

                        if (sortOrderTreeTable == null)
                        {
                            sortOrderTreeTable = new GroupSortOrderTreeTable(this);
                        }

                        sortOrderTreeTable.Comparer = compareColumnDescriptors[0].GroupSortOrderComparer;

                        // Resort the whole array.
                        Array.Sort(array, sortOrderTreeTable.Comparer);

                        sortOrderTreeTable.Clear();

                        sortOrderTreeTable.BeginInit();

                        for (int n = 0; n < array.Length; n++)
                        {
                            GroupSortOrderTreeTableEntry groupSortOrderEntry = new GroupSortOrderTreeTableEntry();
                            groupSortOrderEntry.Tree = sortOrderTreeTable.TreeTable;
                            groupSortOrderEntry.Element = array[n];
                            array[n].GroupSortOrderEntry = groupSortOrderEntry;
                            array[n].ParentElement = this;
                            sortOrderTreeTable.Add(groupSortOrderEntry);
                        }

                        sortOrderTreeTable.EndInit();
                    }

                    this.GroupSortOrderDirty = false;

                    ParentTable.ClearCollectionCaches();

                    if (notified)
                    {
                        ParentTable.RaiseSortedItemsInGroup(ParentGroup);
                    }

                    retVal = true;
                }
                else
                {
                    sortOrderTreeTable = null;

                    if (shouldSort)
                    {
                        if (categoryTreeTable.TreeTable.GetCount() <= 1)
                        {
                            this.columnDescriptors = compareColumnDescriptors;
                            return false;
                        }

                        ParentTable.RaiseSortingItemsInGroup(ParentGroup);

                        array = CreateGroupArray();

                        SortCategories(compareColumnDescriptors, columnDescriptors, array);

                        ParentTable.RaiseSortedItemsInGroup(ParentGroup);

                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        /// <summary>Resets the counter.</summary>
        /// <override/>
        public override void InvalidateCounter()
        {
            ////if (sortOrderTreeTable != null)
            ////    sortOrderTreeTable.InvalidateCounterTopDown(false);

            base.InvalidateCounter();
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <param name="notifySource">Indicates whether to notify the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifySource)
        {
            if (sortOrderTreeTable != null)
            {
                sortOrderTreeTable.InvalidateCounterTopDown(false);
            }

            base.InvalidateCounterTopDown(notifySource);
        }

        /// <override/>
        internal override ElementTreeTable GroupDisplayEntries
        {
            get
            {
                if (sortOrderTreeTable != null)
                {
                    return sortOrderTreeTable;
                }

                return categoryTreeTable;
            }
        }

        /// <override/>
        internal override GroupSortOrderTreeTable GroupSortOrderTreeTable
        {
            get
            {
                return sortOrderTreeTable;
            }
        }

        /// <override/>
        internal override GroupCategoryTreeTable GroupCategoryTreeTable
        {
            get
            {
                return categoryTreeTable;
            }
        }

        /// <summary>
        /// Returns the collection of child groups.
        /// </summary>
        public GroupsInDetailsCollection Groups
        {
            get
            {
                if (_groups == null)
                {
                    _groups = new GroupsInDetailsCollection(this);
                }

                return _groups;
            }
        }
    }
}
