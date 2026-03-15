//-------------------------------------------------------------------------------------------------
// <copyright file="RecordRows.cs" company="syncfusion">
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
    /// A part in a record that serves as a container for <see cref="RecordRow"/>
    /// elements in a record.
    /// </summary>
    public class RecordRowsPart : RecordPart, IRowElementsContainer, IContainerElement
    {
        internal RowElementsTreeTable rowElementTable;

        RecordRowCollection _recordRows;
        RowElementCollection _rowElements;

        /// <summary>
        /// Initializes a new object with the given record as parent.
        /// </summary>
        /// <param name="parent">The parent record this object is created in.</param>
        public RecordRowsPart(Record parent)
            : base(parent)
        {
            rowElementTable = new RowElementsTreeTable(this);
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
                _recordRows = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return RecordRows.Count;
        }

        /// <summary>Returns the element height.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return rowElementTable.YAmountCount;
        }

        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }
        
        /// <summary>Walks down to the child branches and resets counters.</summary>
        /// <param name="notifyCounterSource">Indicates whether to notify the counter source.</param>
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
            AdjustRecordRowCount();
            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Walks down to the child branches and resets summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            TreeEntries.InvalidateSummariesTopDown();
        }

        /// <summary>Resets summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }
        
        /// <summary>Gets the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            AdjustRecordRowCount();
            return RecordRows.Count;
        }

        /// <summary>
        /// Returns the collection of <see cref="RecordRow"/> elements.
        /// </summary>
        public RecordRowCollection RecordRows
        {
            get
            {
                if (_recordRows == null)
                {
                    _recordRows = new RecordRowCollection(this);
                }

                return _recordRows;
            }
        }

        /// <summary>
        /// Returns the collection of <see cref="RecordPreviewRow"/> elements.
        /// </summary>
        public RowElementCollection RowElements
        {
            get
            {
                if (_rowElements == null)
                {
                    _rowElements = new RowElementCollection(this);
                }

                return _rowElements;
            }
        }

        #region IElementTreeTableSource Members

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            return this.rowElementTable;
        }

        #endregion

        internal new RowElementsTreeTable TreeEntries
        {
            get
            {
                return rowElementTable;
            }
        }

        /// <summary>
        /// Adjusts the record row count.
        /// </summary>
        /// <returns>returns boolean value to indicate AdjustRecordRowCount</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool AdjustRecordRowCount()
        {
            return this.ParentElement.AdjustRecordRowCount();
        }
    }

    /// <summary>
    /// A Record row is a place-holder element shown in the <see cref="Table.DisplayElements"/>
    /// for each row in a record. Each record can have multiple record rows. You get access to this element through the <see cref="Record.RecordRows"/>
    /// property of a <see cref="Record"/>.
    /// </summary>
    public class RecordRow : RowElement
    {
        /// <summary>
        /// Initializes a new object in the specified record part.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        public RecordRow(RecordRowsPart parent)
            : base(parent)
        {
        }

        /// <summary>Gets the kind of element.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return ParentRecord.Kind; }
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            Record record = ParentRecord;
            string s = GetType().Name;
            if (record != null)
            {
                s += " " + record.FieldsToString();
            }

            return s;
        }

        /// <summary>
        /// Returns the parent this element belongs to.
        /// </summary>
        public new RecordRowsPart ParentElement
        {
            get
            {
                return (RecordRowsPart)base.ParentElement;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        /// <returns>Number of elements.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 1;
        }

        /// <returns>returns True if changes were detected and the object was updated; False otherwise</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Number of visible elements.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Gets the element height.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultRecordRowHeight;
        }
    }

    #region RecordRowCollection
    /// <summary>
    /// A collection of <see cref="RecordRow"/> elements that are children of a <see cref="Record"/>.
    /// An instance of this collection is returned by the <see cref="Record.RecordRows"/> property
    /// of a <see cref="Record"/> object. RecordRows reserve the display area for displaying the fields
    /// of a record in a grid or report.
    /// </summary>
    public class RecordRowCollection : IList
    {
        internal RowElementCollection _inner;
        internal RecordRowsPart _parentRecord;

        internal RecordRowCollection(RecordRowsPart parentRecord)
        {
            _inner = parentRecord.RowElements;
            _parentRecord = parentRecord;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public RecordRow this[int index]
        {
            get
            {
                return (RecordRow)_inner[index];
            }

            set
            {
                _inner[index] = value;
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(RecordRow value)
        {
            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(RecordRow value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(RecordRow[] array, int index)
        {
            _inner.CopyTo(array, index);
        }

        ////        public RecordRowCollection SyncRoot
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
        public RecordRowCollectionEnumerator GetEnumerator()
        {
            return new RecordRowCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, RecordRow value)
        {
            _inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(RecordRow value)
        {
            _inner.Remove(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(RecordRow value)
        {
            return _inner.Add(value);
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
        /// Gets the number of elements contained in the collection. 
        /// </summary>
        public int Count
        {
            get
            {
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
                this[index] = (RecordRow)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (RecordRow)value);
        }

        void IList.Remove(object value)
        {
            Remove((RecordRow)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((RecordRow)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((RecordRow)value);
        }

        int IList.Add(object value)
        {
            return Add((RecordRow)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((RecordRow[])array, index);
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
    /// Enumerator class for <see cref="RecordRow"/> elements of a <see cref="RecordRowCollection"/>.
    /// </summary>
    public class RecordRowCollectionEnumerator : IEnumerator
    {
        RowElementCollectionEnumerator inner;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public RecordRowCollectionEnumerator(RecordRowCollection collection)
        {
            inner = new RowElementCollectionEnumerator(collection);
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            inner.Reset();
        }

        object IEnumerator.Current
        {
            get
            {
                return inner.Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public RecordRow Current
        {
            get
            {
                return (RecordRow)inner.Current;
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
            return inner.MoveNext();
        }
        #endregion
    }
    #endregion
}
