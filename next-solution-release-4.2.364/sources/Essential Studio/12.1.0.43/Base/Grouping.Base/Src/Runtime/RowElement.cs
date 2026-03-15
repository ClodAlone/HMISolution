//-------------------------------------------------------------------------------------------------
// <copyright file="RowElement.cs" company="syncfusion">
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
using System.ComponentModel.Design;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;

using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Provides a <see cref="RowElements"/> property.
    /// </summary>
    public interface IRowElementsContainer
    {
        /// <summary>
        /// Returns the collection with row elements.
        /// </summary>
        RowElementCollection RowElements { get; }
    }

    /// <summary>
    /// A base class for sections that can contain one or multiple row elements.
    /// </summary>
    public class RowElementsSection : Section, IRowElementsContainer, IContainerElement
    {
        internal RowElementsTreeTable rowElementTable;
        RowElementCollection _rowElements;

        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public RowElementsSection(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (rowElementTable != null)
                {
                    rowElementTable.Dispose();
                }

                rowElementTable = null;
                _rowElements = null;
            }

            base.Dispose(disposing);
        }

        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            if (!ShouldCreateChildElementTreeTable())
            {
                return null;
            }

            return TreeEntries;
        }

        /// <summary>
        /// Shoulds the create child element tree table.
        /// </summary>
        /// <returns>returns boolean value to indicate to create child element tree table.</returns>
        /// <exclude/>
        protected virtual bool ShouldCreateChildElementTreeTable()
        {
            return !this.ParentTable.RecordsAsDisplayElements;
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
            return false;
        }

        internal virtual void OnCreatedTreeTable(RowElementsTreeTable treeEntries)
        {
        }

        internal new RowElementsTreeTable TreeEntries
        {
            get
            {
                if (rowElementTable == null)
                {
                    rowElementTable = new RowElementsTreeTable(this);
                    OnCreatedTreeTable(rowElementTable);
                }

                return rowElementTable;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets summary information for the elements. 
        /// </summary>
        /// <param name="parentTable">A reference to parent table.</param>
        /// <param name="summaryChanged">Returns true if changes were detected.</param>
        /// <returns>Returns summary information.</returns>
        public override ITreeTableSummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            if (rowElementTable == null)
            {
                summaryChanged = false;
                return parentTable.GetEmptySummaries();
            }

            return base.GetSummaries(parentTable, out summaryChanged);
        }

        /// <summary>
        /// Returns the collection of <see cref="RowElement"/> elements.
        /// </summary>
        public RowElementCollection RowElements
        {
            get
            {
                ////return new RowElementCollection(this);
                if (_rowElements == null)
                {
                    _rowElements = new RowElementCollection(this);
                }

                return _rowElements;
            }
        }

        /// <override/>
        /// <summary>Resets the summary for all elements.</summary>
        public override void InvalidateSummariesBottomUp()
        {
            if (this.SectionEntry != null)
            {
                this.SectionEntry.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>Resets counter for all elements.</summary>
        /// <param name="notifyCounterSource">Indicates if the counter source should be notified.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            ////rowElementTable = null;
            if (this.rowElementTable != null)
            {
                rowElementTable.InvalidateCounterTopDown(notifyCounterSource);
            }
        }

        /// <summary>Resets summary for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            if (this.rowElementTable != null)
            {
                rowElementTable.InvalidateSummariesTopDown();
            }
        }

        /// <summary>Resets the summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.VisibleCount;
        }

        /// <summary>Gets the element height.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.YAmountCount;
        }

        /// <summary>Gets the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.ElementCount;
        }

        /// <summary>Gets the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.FilteredRecordCount;
        }

        /// <summary>Gets the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            EnsureInitialized(this, false);
            return TreeEntries.RecordCount;
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return base.ToString();
        }
    }

    /// <summary>
    /// A base class for elements that can be added to a <see cref="RowElementCollection"/> in a <see cref="RowElementsSection"/>.
    /// </summary>
    public abstract class RowElement : Element, IDisplayElement
    {
        RowElementsTreeTableEntry rowElementEntry;

        /// <summary>
        /// Initializes an object with the specified parent element.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public RowElement(Element parent)
            : base(parent)
        {
        }

        int id;

        /// <summary>Determines whether this object can be uniquely identified using the Id.</summary>
        /// <returns>True if it supports id.</returns>
        /// <override/>
        public override bool SupportsId()
        {
            return true;
        }

        /// <summary>Gets or sets the key to identify the current object.</summary>
        /// <override/>
        public override int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        /// <summary>Gets the element count.</summary>
        /// <returns>Number of elements.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 0;
        }

        /// <summary>Gets the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
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

        /// <summary>Gets the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 0;
        }
        
        /// <override/>
        /// <summary>Walks up to the parent branches and resets the counters.</summary>
        public override void InvalidateCounterBottomUp()
        {
            base.InvalidateCounterBottomUp();
        }

        /// <summary>Walks down to the child branches and resets the counters.</summary>
        /// <param name="notifyCounterSource">Indicates if the counter source should be notified.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
        }

        /// <summary>Walks up to the parent branches and resets the summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
            base.InvalidateSummariesBottomUp();
        }

        /// <summary>Walks down to the child branches and resets the summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
        }

        /// <summary>Resets the summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetElementEntry()
        {
            return rowElementEntry;
        }

        internal RowElementsTreeTableEntry RowElementEntry
        {
            get
            {
                return rowElementEntry;
            }

            set
            {
                rowElementEntry = value;
            }
        }
    }

    #region RowElementCollection
    /// <summary>
    /// A collection of <see cref="RowElement"/> elements that are children of an <see cref="IRowElementsContainer"/>.
    /// This is a base class for <see cref="RecordRowCollection"/>, <see cref="RecordPreviewRowCollection"/>, and
    /// other row collections.
    /// An instance of this collection is returned by the RowElements property
    /// of an <see cref="IRowElementsContainer"/> object.
    /// </summary>
    public class RowElementCollection : IList
    {
        internal RowElementsTreeTable _inner;
        internal IContainerElement _parentElementContainer;

        internal RowElementCollection(IContainerElement parentElementContainer)
        {
            _inner = (RowElementsTreeTable)parentElementContainer.GetChildElementTreeTable(false);
            _parentElementContainer = parentElementContainer;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public RowElement this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                RowElementsTreeTableEntry entry = _inner[index] as RowElementsTreeTableEntry;
                if (entry == null)
                {
                    return null;
                }

                return entry.Element;
            }

            set
            {
                RowElement rowElement = value;
                RowElementsTreeTableEntry sectionEntry = new RowElementsTreeTableEntry();
                sectionEntry.Element = rowElement;
                sectionEntry.Tree = _inner.TreeTable;

                rowElement.ParentElement = (Element)_parentElementContainer;
                rowElement.RowElementEntry = sectionEntry;
                _inner[index] = sectionEntry;
            }
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
        public bool Contains(RowElement value)
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
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int IndexOf(RowElement value)
        {
            if (!Contains(value))
            {
                return -1;
            }

            return value.RowElementEntry.GetPosition();
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(RowElement[] array, int index)
        {
            int n = 0;
            foreach (RowElement rowElement in this)
            {
                array[index + n] = rowElement;
                n++;
            }
        }

        ////        public RowElementCollection SyncRoot
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
        /// <remarks>Enumerators only allow reading of the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public RowElementCollectionEnumerator GetEnumerator()
        {
            return new RowElementCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="rowElement">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>
        /// </remarks>
        public void Insert(int index, RowElement rowElement)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            RowElementsTreeTableEntry rowElementEntry = new RowElementsTreeTableEntry();
            rowElementEntry.Element = rowElement;
            rowElementEntry.Tree = _inner.TreeTable;

            rowElement.ParentElement = (Element)_parentElementContainer;
            rowElement.RowElementEntry = rowElementEntry;

            _inner.Insert(index, rowElementEntry);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void Remove(RowElement value)
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
        /// <param name="rowElement">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(RowElement rowElement)
        {
            RowElementsTreeTableEntry rowElementEntry = new RowElementsTreeTableEntry();
            rowElementEntry.Element = rowElement;
            rowElementEntry.Tree = _inner.TreeTable;

            rowElement.ParentElement = (Element)_parentElementContainer;
            rowElement.RowElementEntry = rowElementEntry;

            return _inner.Add(rowElementEntry);
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
                this[index] = (RowElement)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (RowElement)value);
        }

        void IList.Remove(object value)
        {
            Remove((RowElement)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((RowElement)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((RowElement)value);
        }

        int IList.Add(object value)
        {
            return Add((RowElement)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((RowElement[])array, index);
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
    /// Enumerator class for <see cref="RowElement"/> elements of a <see cref="RowElementCollection"/>.
    /// </summary>
    public class RowElementCollectionEnumerator : IEnumerator
    {
        RowElement _cursor, _next;
        IList _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public RowElementCollectionEnumerator(IList collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0] as RowElement;
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0] as RowElement;
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
        public RowElement Current
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
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == null)
            {
                return false;
            }

            _cursor = _next;

            _next = (RowElement)ElementHelper.GetNextSibling(_next);

            return _cursor != null;
        }
        #endregion
    }
    #endregion
}
