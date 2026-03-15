//-------------------------------------------------------------------------------------------------
// <copyright file="NestedTable.cs" company="syncfusion">
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
    /// The part of a record with nested tables. You can access nested tables in a record with the <see cref="Record.NestedTables"/>
    /// property of a <see cref="Record"/>.
    /// </summary>
    public class RecordNestedTablesPart : RecordPart, IContainerElement
    {
        internal NestedTablesTreeTable nestedTablesTreeTable;

        WeakReference __nestedTables;

        NestedTablesCollection _nestedTables
        {
            get
            {
                if (__nestedTables != null)
                {
                    return (NestedTablesCollection)__nestedTables.Target;
                }

                return null;
            }

            set
            {
                __nestedTables = new WeakReference(value);
            }
        }

        /// <summary>
        /// Initializes a new object with the given record as parent.
        /// </summary>
        /// <param name="parent">The parent record this object is created in.</param>
        public RecordNestedTablesPart(Record parent)
            : base(parent)
        {
            nestedTablesTreeTable = new NestedTablesTreeTable(this);
        }

        /// <summary>
        /// Gets the number of visible elements.
        /// </summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return nestedTablesTreeTable.VisibleCount;
        }

        /// <summary>
        /// Gets the height for the element.
        /// </summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return nestedTablesTreeTable.YAmountCount;
        }
        
        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <param name="notifyCounterSource">Indicates whether to nofity the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            TreeEntries.InvalidateCounterTopDown(notifyCounterSource);
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
            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Resets the summary for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            TreeEntries.InvalidateSummariesTopDown();
        }

        /// <summary>Resets the summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return nestedTablesTreeTable.ElementCount;
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public NestedTablesCollection NestedTables
        {
            get
            {
                if (_nestedTables == null)
                {
                    _nestedTables = new NestedTablesCollection(this);
                }

                return _nestedTables;
            }
        }

        #region IElementTreeTableSource Members

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            return this.nestedTablesTreeTable;
        }

        #endregion

        internal new NestedTablesTreeTable TreeEntries
        {
            get
            {
                return nestedTablesTreeTable;
            }
        }
    }

    /// <summary>
    /// A nested table inside a record. You can access nested tables in a record with the <see cref="Record.NestedTables"/>
    /// property of a <see cref="Record"/>. Each nested table has a <see cref="ChildTable"/> that provides a link to the
    /// to the child records in a related table.
    /// </summary>
    public class NestedTable : Element, IContainerElement
    {
        NestedTablesTreeTableEntry nestedTableEntry;
        ChildTable childTable;
        internal bool childTableDisposed = false;

        /// <summary>
        /// Initializes a new object with the specified parent.
        /// </summary>
        /// <param name="parent">The parent element this object is created in.</param>
        public NestedTable(RecordNestedTablesPart parent)
            : base(parent)
        {
        }

        /// <summary>Gets the element kind.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.NestedTable; }
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            if (childTable == null)
            {
                return null;
            }

            return ((IElementTreeTableSource)childTable).GetChildElementTreeTable(displayOrder);
        }

        bool IContainerElement.ShouldStepIntoElements()
        {
            return false;
        }

        /// <summary>Resets the counter.</summary>
        /// <override/>
        public override void InvalidateCounter()
        {
            base.InvalidateCounter();
        }

        /// <summary>
        /// <see cref="ChildTable"/> provides a link to the
        /// to the child records in a related table.
        /// </summary>
        public ChildTable ChildTable
        {
            get
            {
                return childTable;
            }

            set
            {
                childTable = value;
                if (childTable != null)
                {
                    childTable.NestedTableEntry = this.NestedTableEntry;
                }
            }
        }

        /// <summary>Gets the number of childs.</summary>
        /// <returns>Child count.</returns>
        /// <override/>
        public override int GetChildCount()
        {
            if (childTable == null)
            {
                return 0;
            }

            return 1;
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            if (childTable == null)
            {
                return 0;
            }

            return childTable.GetVisibleCount();
        }

        /// <summary>Returns height for the element.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            if (childTable == null)
            {
                return 0;
            }

            return childTable.GetYAmountCount();
        }

        /// <summary>Gets the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            if (childTable == null)
            {
                return 0;
            }

            return childTable.GetElementCount();
        }

        /// <summary>Gets the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <summary>Gets the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 0;
        }

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetElementEntry() 
        { 
            return NestedTableEntry; 
        }

        internal NestedTablesTreeTableEntry NestedTableEntry
        {
            get
            {
                return nestedTableEntry;
            }

            set
            {
                if (childTable != null)
                {
                    childTable.NestedTableEntry = value;
                }

                nestedTableEntry = value;
            }
        }

        /// <summary>
        /// Determines if the child table with child records is expanded or collapsed.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return childTable != null && childTable.IsExpanded;
            }

            set
            {
                if (childTable == null)
                {
                    return;
                }

                childTable.IsExpanded = value;
            }
        }

        /// <summary>
        /// Returns string representation of the object.
        /// </summary>
        /// <returns>
        /// String representation of the current object.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            ChildTable childTable = this.ChildTable;
            if (childTable != null)
            {
                return GetType().Name + " " + childTable.CategoriesToString();
            }
            else
            {
                return GetType().Name;
            }
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <override/>
        public override void InvalidateCounterBottomUp()
        {
            ParentElement.InvalidateCounterBottomUp();
        }

        /// <summary>Resets the summary for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            if (childTable == null)
            {
                return;
            }

            childTable.InvalidateSummariesTopDown();
        }

        /// <summary>Resets the summaries.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <param name="notifyCounterSource">When true notifies the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            if (childTable == null)
            {
                return;
            }

            childTable.InvalidateCounterTopDown(notifyCounterSource);
        }

        /// <summary>
        /// ShortCut for ((RecordsDetails) Details).Records. This returns all records in the group including records that
        /// do not meet filter criteria.
        /// </summary>
        public RecordsInDetailsCollection Records
        {
            get
            {
                if (childTable == null)
                {
                    return RecordsInDetailsCollection.Empty;
                }

                return childTable.Records;
            }
        }

        /// <summary>
        /// ShortCut for ((RecordsDetails) Details).FilteredRecords. This returns only records in the group that
        /// meet filter criteria.
        /// </summary>
        public FilteredRecordsInDetailsCollection FilteredRecords
        {
            get
            {
                if (childTable == null)
                {
                    return FilteredRecordsInDetailsCollection.Empty;
                }

                return childTable.FilteredRecords;
            }
        }

        /// <summary>
        /// ShortCut for ((GroupsDetails) Details).Groups. A group can either be a final node with records or it can be a node with nested groups. If a
        /// group has records, its <see cref="Groups"/> collection will be empty and the <see cref="Records"/>
        /// collection will contain all records. If a group has nested groups, its <see cref="Groups"/>
        /// collection will have the nested groups and the <see cref="Records"/> collection will be empty.
        /// </summary>
        public GroupsInDetailsCollection Groups
        {
            get
            {
                if (childTable == null)
                {
                    return GroupsInDetailsCollection.Empty;
                }

                return childTable.Groups;
            }
        }

        #region CurrentRecordManager Members

        /// <summary>Returns null.</summary>
        /// <returns>Returns Null value.</returns>
        /// <override/>
        public override object GetData()
        {
            return null;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.BeginEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnBeginEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnBeginEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EndEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnEndEditCalled()
        {
            if (ChildTable.ContainsCurrentRecordOrChildTable())
            {
                ChildTable.LeaveRecord(false);
            }

            return !ChildTable.ContainsCurrentRecordOrChildTable();
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnEndEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.CancelEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnCancelEditCalled()
        {
            if (ChildTable.ContainsCurrentRecordOrChildTable())
            {
                ChildTable.LeaveRecord(false);
            }

            return !ChildTable.ContainsCurrentRecordOrChildTable();
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnCancelEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.LeaveRecord"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.LeaveRecord"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnLeaveRecordCalled()
        {
            // If child table has current record or child notification will bubble down to grandchildren.
            if (ChildTable == null || ChildTable.ParentTable.CurrentRecordManager.InLeaveRecord)
            {
                return true;
            }

            ChildTable.LeaveRecord(false);
            return !ChildTable.ContainsCurrentRecordOrChildTable();
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.LeaveRecord"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnLeaveRecordComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EnterRecord"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EnterRecord"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnEnterRecordCalled()
        {
            ////            // If parent's current child table is not this table, then notification will bubble up to grandparent table.
            ////            if (!ChildTable.Table.CurrentRecordManager.InEnterRecord)
            ////            {
            ////                Table parent = ParentTable;
            ////                if (parent != null)
            ////                {
            ////                    if (!parent.HasCurrentNestedTable || parent.CurrentNestedTable != this)
            ////                        return ParentTable.CurrentRecordManager.NavigateTo(this) == this;
            ////                }
            ////            }
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EnterRecord"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnEnterRecordComplete(bool success)
        {
        }

        #endregion
    }

    #region NestedTablesCollection
    /// <summary>
    /// A collection of <see cref="NestedTable"/> elements that are children of a <see cref="Record"/>.
    /// An instance of this collection is returned by the <see cref="Record.NestedTables"/> property
    /// of a <see cref="Record"/> object. Each NestedTable has a <see cref="NestedTable.ChildTable"/>
    /// that links the record in a parent table to the child records in a related table.
    /// </summary>
    public class NestedTablesCollection : IList
    {
        internal NestedTablesTreeTable _inner;
        internal IContainerElement _parentElementContainer; // RecordNestedTablesPart

        internal NestedTablesCollection(IContainerElement parentElementContainer)
        {
            _inner = (NestedTablesTreeTable)parentElementContainer.GetChildElementTreeTable(false);
            _parentElementContainer = parentElementContainer;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public NestedTable this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                NestedTablesTreeTableEntry entry = _inner[index] as NestedTablesTreeTableEntry;
                if (entry == null)
                {
                    return null;
                }

                return entry.Element;
            }

            set
            {
                NestedTable nestedTable = value;
                NestedTablesTreeTableEntry sectionEntry = new NestedTablesTreeTableEntry();
                sectionEntry.Element = nestedTable;
                sectionEntry.Tree = _inner.TreeTable;

                // ParentElement should be the Table with all the child tables ..., not this records
                // Paremt!
                nestedTable.ParentElement = (Element)_parentElementContainer;
                nestedTable.NestedTableEntry = sectionEntry;
                _inner[index] = sectionEntry;
            }
        }

        int FindName(string name)
        {
            Table table = ((Element)_parentElementContainer).ParentTable;
            int index = table.RelatedTables.IndexOfNestedTable(name);
            return index;
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        public NestedTable this[string name]
        {
            get
            {
                int index = FindName(name);
                if (index == -1)
                {
                    throw new ArgumentException(name + " not found in Relations collection.");
                }

                return this[index];
            }

            set
            {
                int index = FindName(name);
                if (index == -1)
                {
                    throw new ArgumentException(name + " not found in Relations collection.");
                }

                this[index] = value;
            }
        }

        /// <summary>
        /// Determines if the element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            return FindName(name) != -1;
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            return FindName(name);
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(NestedTable value)
        {
            if (value == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);
            return value.ParentElement == _parentElementContainer;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(NestedTable value)
        {
            if (!Contains(value))
            {
                return -1;
            }

            return value.NestedTableEntry.GetPosition();
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(NestedTable[] array, int index)
        {
            int n = 0;
            foreach (NestedTable nestedTable in this)
            {
                array[index + n] = nestedTable;
                n++;
            }
        }

        ////        public NestedTablesCollection SyncRoot
        ////        {
        ////            get
        ////            {
        ////                return null;
        ////            }
        ////        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public NestedTablesCollectionEnumerator GetEnumerator()
        {
            return new NestedTablesCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="nestedTable">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void Insert(int index, NestedTable nestedTable)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            NestedTablesTreeTableEntry nestedTableEntry = new NestedTablesTreeTableEntry();
            nestedTableEntry.Element = nestedTable;
            nestedTableEntry.Tree = _inner.TreeTable;

            // ParentElement should be the Table with all the child tables ..., not this records
            // Paremt!
            nestedTable.ParentElement = (Element)_parentElementContainer;
            nestedTable.NestedTableEntry = nestedTableEntry;

            _inner.Insert(index, nestedTableEntry);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(NestedTable value)
        {
            int index = IndexOf(value);
            if (index != -1)
            {
                _inner.RemoveAt(index);
            }
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="nestedTable">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(NestedTable nestedTable)
        {
            NestedTablesTreeTableEntry nestedTableEntry = new NestedTablesTreeTableEntry();
            nestedTableEntry.Element = nestedTable;
            nestedTableEntry.Tree = _inner.TreeTable;

            // ParentElement should be the Table with all the child tables ..., not this records
            // Paremt!
            nestedTable.ParentElement = (Element)_parentElementContainer;
            nestedTable.NestedTableEntry = nestedTableEntry;

            return _inner.Add(nestedTableEntry);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            _inner.Clear();
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// If changes in the TableDescriptor are detected, the
        /// method will reinitialize the collection before returning the count.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_parentElementContainer == null)
                {
                    return 0;
                }

                _parentElementContainer.EnsureInitialized(this, false);
                return _inner.Count;
            }
        }
        
        #region IList Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (NestedTable)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (NestedTable)value);
        }

        void IList.Remove(object value)
        {
            Remove((NestedTable)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((NestedTable)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((NestedTable)value);
        }

        int IList.Add(object value)
        {
            return Add((NestedTable)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((NestedTable[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="NestedTable"/> elements of a <see cref="NestedTablesCollection"/>.
    /// </summary>
    public class NestedTablesCollectionEnumerator : IEnumerator
    {
        NestedTable _cursor, _next;
        IList _coll;

        /// <summary>
        /// Initializes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public NestedTablesCollectionEnumerator(NestedTablesCollection collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0] as NestedTable;
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0] as NestedTable;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public NestedTable Current
        {
            get
            {
                return _cursor;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if it passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == null)
            {
                return false;
            }

            _cursor = _next;

            _next = (NestedTable)ElementHelper.GetNextSibling(_next);

            return _cursor != null;
        }
        #endregion
    }
    #endregion
}