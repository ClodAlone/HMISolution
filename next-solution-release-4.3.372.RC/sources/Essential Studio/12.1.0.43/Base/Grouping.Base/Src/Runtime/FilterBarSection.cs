//-------------------------------------------------------------------------------------------------
// <copyright file="FilterBarSection.cs" company="syncfusion">
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
    ///  A place holder section with 1 visible element for showing a filter bar.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class FilterBarSection : RowElementsSection
    {
        FilterBarRowCollection _columnHeaderRows;
        ////bool rowsChanged = true;
        bool rowsChanged
        {
            get
            {
                return Reserved1;
            }

            set
            {
                Reserved1 = value;
            }
        }

        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        public FilterBarSection(Group parent)
            : base(parent)
        {
            rowsChanged = true;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            _columnHeaderRows = null;
            base.Dispose(disposing);
        }

        /// <summary>Gets the kind of display element.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.FilterBar; }
        }
          
        /// <returns>return boolean value</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            AdjustFilterBarRowCount();
            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Forces reevaluation of the counters of all elements.</summary>
        /// <param name="notifyCounterSource">When true notifies the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            base.InvalidateCounterTopDown(notifyCounterSource);
            rowsChanged = true;
        }

        internal sealed override void OnCreatedTreeTable(RowElementsTreeTable treeEntries)
        {
            rowsChanged = true;
            AdjustFilterBarRowCount();
        }

        bool AdjustFilterBarRowCount()
        {
            if (!this.ParentTable.RecordsAsDisplayElements
                && rowsChanged)
            {
                if (this.ParentTable.TableDescriptor.Fields.Count == 0 && FilterBarRows.Count > 0)
                {
                    this.InvalidateCounterBottomUp();
                    FilterBarRows.Clear();
                }
                else
                {
                    // Add rows on demand - this is triggered when InvalidateCounterTopDown is called on table.
                    // see Table.SummaryRows_Changed: CountersDirty = true;
                    Table table = this.ParentTable;
                    if (table != null)
                    {
                        int count = table.TableDescriptor.RowsPerRecord;
                        if (FilterBarRows.Count != count)
                        {
                            while (FilterBarRows.Count > count)
                            {
                                FilterBarRows.Remove(FilterBarRows[FilterBarRows.Count - 1]);
                            }

                            while (FilterBarRows.Count < count)
                            {
                                FilterBarRow row = Engine.CreateFilterBarRow(this);
                                FilterBarRows.Add(row);
                            }

                            return true;
                        }
                    }
                }

                rowsChanged = false;
            }

            return false;
        }

        /// <summary>Gets number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.ElementCount;
            }

            return ParentTable.TableDescriptor.RowsPerRecord;
        }

        /// <summary>Gets number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.VisibleCount;
            }

            return ParentTable.TableDescriptor.RowsPerRecord;
        }

        /// <summary>Gets the height of the element.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.YAmountCount;
            }

            return ParentTable.TableDescriptor.RowsPerRecord * ParentTable.DefaultFilterBarRowHeight;
        }
        
        /// <summary>
        /// Returns the collection of <see cref="FilterBarRow"/> elements.
        /// </summary>
        public FilterBarRowCollection FilterBarRows
        {
            get
            {
                if (_columnHeaderRows == null)
                {
                    _columnHeaderRows = new FilterBarRowCollection(this);
                }

                return _columnHeaderRows;
            }
        }

        ////        public override void InvalidateSummariesTopDown()
        ////        {
        ////
        ////        }

        /// <summary>Gets number of filtered records.</summary>
        /// <returns>Fitlered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <summary>Gets number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 0;
        }
    }

    /// <summary>Creates a filter bar row. Only for internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class FilterBarRow : RowElement
    {
        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        public FilterBarRow(FilterBarSection parent)
            : base(parent)
        {
        }

        /// <summary>Only for internal use.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.FilterBar; }
        }

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        public new FilterBarSection ParentElement
        {
            get
            {
                return (FilterBarSection)base.ParentElement;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>Gets number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 1;
        }

         /// <returns>returns boolean value</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Gets number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Gets the height for the element.</summary>
        /// <returns>Returns Height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultFilterBarRowHeight;
        }
    }
    
    #region FilterBarRowCollection
    /// <summary>
    /// A collection of <see cref="FilterBarRow"/> elements that are children of a <see cref="FilterBarSection"/> in a <see cref="Group"/>.
    /// An instance of this collection is returned by the <see cref="FilterBarSection.FilterBarRows"/> property
    /// of a <see cref="FilterBarSection"/> object. (NOTE: FilterBars are not supported in Version 2.0).
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class FilterBarRowCollection : IList
    {
        internal RowElementCollection _inner;
        internal FilterBarSection _parentFilterBarSection;

        internal FilterBarRowCollection(FilterBarSection parentFilterBarSection)
        {
            _inner = parentFilterBarSection.RowElements;
            _parentFilterBarSection = parentFilterBarSection;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public FilterBarRow this[int index]
        {
            get
            {
                return (FilterBarRow)_inner[index];
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
        public bool Contains(FilterBarRow value)
        {
            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(FilterBarRow value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(FilterBarRow[] array, int index)
        {
            _inner.CopyTo(array, index);
        }

        ////        public FilterBarRowCollection SyncRoot
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
        public FilterBarRowCollectionEnumerator GetEnumerator()
        {
            return new FilterBarRowCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, FilterBarRow value)
        {
            _inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(FilterBarRow value)
        {
            _inner.Remove(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(FilterBarRow value)
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
                this[index] = (FilterBarRow)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (FilterBarRow)value);
        }

        void IList.Remove(object value)
        {
            Remove((FilterBarRow)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((FilterBarRow)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((FilterBarRow)value);
        }

        int IList.Add(object value)
        {
            return Add((FilterBarRow)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((FilterBarRow[])array, index);
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
    /// Enumerator class for <see cref="FilterBarRow"/> elements of a <see cref="FilterBarRowCollection"/>.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class FilterBarRowCollectionEnumerator : IEnumerator
    {
        RowElementCollectionEnumerator inner;

        /// <summary>
        /// Initializes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public FilterBarRowCollectionEnumerator(FilterBarRowCollection collection)
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
        public FilterBarRow Current
        {
            get
            {
                return (FilterBarRow)inner.Current;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator is successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            return inner.MoveNext();
        }
        #endregion
    }
    #endregion
}