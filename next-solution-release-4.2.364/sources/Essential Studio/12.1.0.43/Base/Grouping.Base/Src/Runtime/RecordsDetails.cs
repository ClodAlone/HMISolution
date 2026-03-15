//-------------------------------------------------------------------------------------------------
// <copyright file="RecordsDetails.cs" company="syncfusion">
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
    /// The details section of a group with records.
    /// </summary>
    public class RecordsDetails : DetailsSection, IContainerElement
    {
        internal SortedRecordsTreeTable recordsTree;
        RecordsInDetailsCollection _sortedRecords;
        FilteredRecordsInDetailsCollection _filteredRecords;
        internal SortColumnDescriptorCollection sortedColumns;

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (recordsTree != null)
                {
                    recordsTree.Dispose();
                }

                _sortedRecords = null;
                recordsTree = null;
                _filteredRecords = null;
                sortedColumns = null;
            }

            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public RecordsDetails(Group parent)
            : base(parent)
        {
            recordsTree = new SortedRecordsTreeTable(this);
            if (parent.ParentTable.WithoutCounter && !(this is AddNewRecordSection))
            {
                recordsTree.WithoutCounter = true;
            }
        }

        internal void InitializeComparer(SortColumnDescriptorCollection sortedColumns)
        {
            this.sortedColumns = sortedColumns;
        }

        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            return RecordTreeEntries;
        }

        /// <summary>
        /// Determines whether the TableDescriptor.SortedColumns collection was
        /// modified since the group was sorted.
        /// </summary>
        /// <returns>True if sorted columns are modified; False otherwise.</returns>
        public bool CompareSortedColumns()
        {
            Table table = ParentTable;
            if (table != null && sortedColumns.Version != this.ParentTableDescriptor.SortedColumns.Version)
            {
                if (sortedColumns.Equals(ParentTableDescriptor.SortedColumns))
                {
                    sortedColumns.version = ParentTableDescriptor.SortedColumns.Version;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the TableDescriptor.SortedColumns collection was
        /// modified since the group was sorted and if this is the case the records
        /// in the group will be resorted.
        /// </summary>
        /// <returns>true if records needed sorting; false otherwise.</returns>
        public bool UpdateSortedColumns()
        {
            bool sort = CompareSortedColumns();
            if (!sort)
            {
                return false;
            }

            Table table = ParentTable;
            SortColumnDescriptorCollection compareColumnDescriptors = null;
            compareColumnDescriptors = ParentTableDescriptor.SortedColumns.GetShadowedCopy();

            if (recordsTree.Count == 0 || recordsTree.VirtualMode)
            {
                this.sortedColumns = compareColumnDescriptors;
                return false;
            }
            else if (recordsTree.Count == 1)
            {
                SortColumnDescriptor[] arrayOfColumnDescriptors;
                PropertyDescriptor[] arrayOfPropertyDescriptor;
                bool isSorted;
                this.ParentTableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);

                Record r = Records[0];
                r.UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);

                if (this is AddNewRecordSection)
                {
                    this.sortedColumns = compareColumnDescriptors;
                    return false;
                }
            }

            if (sort)
            {
                ParentTable.RaiseSortingItemsInGroup(ParentGroup);
                Record[] array = new Record[Records.Count];
                int len = Records.CopyTo(array, 0);

                try
                {
                    recordsTree.Clear();
                    recordsTree.BeginInit();

                    //// can we just reverse the existing sorting?

                    int sortedColumnsCount = this.sortedColumns.Count;
                    bool reverseOnly = compareColumnDescriptors.Count == sortedColumnsCount;
                    for (int n = 0; reverseOnly && n < compareColumnDescriptors.Count; n++)
                    {
                        SortColumnDescriptor cd1 = compareColumnDescriptors[n];
                        SortColumnDescriptor cd2 = this.sortedColumns[n];

                        reverseOnly &= cd1.Comparer == cd2.Comparer
                            && cd1.Name == cd2.Name
                            && cd1.SortDirection != cd2.SortDirection;
                    }

                    if (reverseOnly)
                    {
                        for (int n = array.Length - 1; n >= 0; n--)
                        {
                            SortedRecordsTreeTableEntry sortedEntry = new SortedRecordsTreeTableEntry();
                            sortedEntry.Tree = RecordTreeEntries.TreeTable;
                            sortedEntry.Element = array[n];
                            array[n].SortedEntry = sortedEntry;
                            array[n].ParentElement = this;
                            recordsTree.Add(sortedEntry);
                        }
                    }
                    else
                    {
                        SortColumnDescriptor[] arrayOfColumnDescriptors;
                        PropertyDescriptor[] arrayOfPropertyDescriptor;
                        bool isSorted;
                        this.ParentTableDescriptor.GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);

                        for (int n = 0; n < array.Length; n++)
                        {
                            array[n].UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
                        }

                        Array.Sort(array, 0, len, new RecordsDetailsSortColumnComparer(table, table.TableDescriptor.SortedColumns));

                        for (int n = 0; n < array.Length; n++)
                        {
                            SortedRecordsTreeTableEntry sortedEntry = new SortedRecordsTreeTableEntry();
                            sortedEntry.Tree = RecordTreeEntries.TreeTable;
                            sortedEntry.Element = array[n];
                            array[n].SortedEntry = sortedEntry;
                            array[n].ParentElement = this;
                            recordsTree.Add(sortedEntry);
                        }
                    }
                }
                finally
                {
                    this.sortedColumns = compareColumnDescriptors;
                    recordsTree.EndInit();
                    ParentTable.RaiseSortedItemsInGroup(ParentGroup);
                }

                return true;
            }

            return false;
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
            if (this.ParentTable == null || this.ParentTable.inItemDeleted)
            {
                return false;
            }

            return UpdateSortedColumns();
        }

        /// <override/>
        internal override SortedRecordsTreeTable RecordTreeEntries
        {
            get
            {
                return recordsTree;
            }
        }

        /// <summary>
        /// Returns the collection of all records in this group. Records that do not meet
        /// filter criteria are included.
        /// </summary>
        public RecordsInDetailsCollection Records
        {
            get
            {
                if (_sortedRecords == null)
                {
                    _sortedRecords = new RecordsInDetailsCollection(this);
                }

                return _sortedRecords;
            }
        }

        /// <summary>
        /// Returns the collection of records in this group that meet
        /// filter criteria.
        /// </summary>
        public FilteredRecordsInDetailsCollection FilteredRecords
        {
            get
            {
                if (_filteredRecords == null)
                {
                    _filteredRecords = new FilteredRecordsInDetailsCollection(this);
                }

                return _filteredRecords;
            }
        }
    }
}
