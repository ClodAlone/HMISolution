//-------------------------------------------------------------------------------------------------
// <copyright file="ColumnHeaderSection.cs" company="syncfusion">
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
    /// The section with the ColumnHeaderRow(s):
    /// </summary>
    /// <remarks>
    /// A ColumnHeaderSection holds ColumnHeaderRow(s) that are displayed at the top of a
    /// table or group. ColumnHeaderRow is a place holder item where the grouping grid
    /// should display column headers. <para/>
    /// <para/> 
    /// The ColumnHeaderSection is a container element and has a collection of one or
    /// multiple ColumnHeaderRow elements.<para/>
    /// <para/> 
    /// If ColumnHeaderSection(s) should be displayed at the top of each group,
    /// ColumnHeaderSection(s) are created when the grouping for a table is initialized.
    ///  The grouping engine loops through all sorted records and categorizes them. For
    ///  each new group the virtual TableDescriptor.CreateGroup factory method is called.
    /// The TableDescriptor.CreateGroup method instantiates a ColumnHeaderSection by
    /// calling the virtual TableDescriptor.CreateColumnHeaderSection factory method.
    /// <para/>
    /// <para/> 
    /// One set of ColumnHeaderRows are displayed at the top of the table below the CaptionSection.
    /// The rows belong to the ColumnHeaderSection of the TopLevelGroup
    /// of a table. The TopLevelGroup is created with the virtual
    /// TableDescriptor.CreateGroup factory method. The TableDescriptor.CreateGroup
    /// method instantiates a ColumnHeaderSection by calling the virtual
    /// TableDescriptor.CreateColumnHeaderSection factory method. <para/>
    /// <para/> 
    /// Because the ColumnHeaderSection is a container element and not a display element,
    /// it will not be an item returned by the Table.DisplayElements and
    /// Table.GroupedElements collections. Instead, the ColumnHeaderSection can be
    /// accessed only through the Group.Sections collection of its parent group
    /// or TopLevelGroup.<para/>
    /// <para/> 
    /// See also:<para/>
    /// TableDescriptor.CreateGroup, TableDescriptor.CreateColumnHeaderSection,
    /// Table.TopLevelGroup, IContainerElement, Group.Sections
    /// </remarks>
    public class ColumnHeaderSection : RowElementsSection, IElementTreeTableSource
    {
        ColumnHeaderRowCollection _columnHeaderRows;

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
        /// <param name="parent">The group this section is created in.</param>
        public ColumnHeaderSection(Group parent)
            : base(parent)
        {
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
            get { return DisplayElementKind.ColumnHeader; }
        }
             
        /// <returns>returns boolean value false.</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            AdjustColumnHeaderRowCount();
            return base.OnEnsureInitialized(sender);
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return null;
            }

            return TreeEntries;
        }

        /// <summary>Forces reevaluation of all counters for all the table elements.</summary>
        /// <param name="notifyCounterSource">When True notifies counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            base.InvalidateCounterTopDown(notifyCounterSource);
            rowsChanged = true;
        }

        internal sealed override void OnCreatedTreeTable(RowElementsTreeTable treeEntries)
        {
            rowsChanged = true;
            AdjustColumnHeaderRowCount();
        }

        bool AdjustColumnHeaderRowCount()
        {
            if (!this.ParentTable.RecordsAsDisplayElements
                && rowsChanged)
            {
                // Add rows on demand - this is trigger when InvalidateCounterTopDown is called on Table
                // see Table.SummaryRows_Changed: CountersDirty = true;
                Table table = this.ParentTable;
                if (table != null)
                {
                    int count = table.TableDescriptor.RowsPerRecord;
                    if (ColumnHeaderRows.Count != count)
                    {
                        while (ColumnHeaderRows.Count > count)
                        {
                            ColumnHeaderRows.Remove(ColumnHeaderRows[ColumnHeaderRows.Count - 1]);
                        }

                        while (ColumnHeaderRows.Count < count)
                        {
                            ColumnHeaderRow row = Engine.CreateColumnHeaderRow(this);
                            ColumnHeaderRows.Add(row);
                        }

                        return true;
                    }
                }

                rowsChanged = false;
            }
            ////if (ColumnHeaderRows.Count == 0)
            ////    Debugger.Break();
            return false;
        }

        /// <summary>Returns the number of elements.</summary>
        /// <returns>Number of elements.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.ElementCount;
            }

            return ParentTable.TableDescriptor.RowsPerRecord;
        }

        /// <summary>Returns the number of visible elements.</summary>
        /// <returns>Number of visible elements.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.VisibleCount;
            }

            return ParentTable.TableDescriptor.RowsPerRecord;
        }

        /// <summary>Returns the column header section height.</summary>
        /// <returns>Returns Height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.YAmountCount;
            }

            return ParentTable.TableDescriptor.RowsPerRecord * ParentTable.DefaultColumnHeaderRowHeight;
        }

        /// <summary>
        /// The collection of <see cref="ColumnHeaderRow"/> elements.
        /// </summary>
        public ColumnHeaderRowCollection ColumnHeaderRows
        {
            get
            {
                if (_columnHeaderRows == null)
                {
                    _columnHeaderRows = new ColumnHeaderRowCollection(this);
                }

                return _columnHeaderRows;
            }
        }

        ////        public override void InvalidateSummariesTopDown()
        ////        {
        ////
        ////        }

        /// <summary>Returns the number of filtered records.</summary>
        /// <returns>Number of filtered records.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <override/>
        /// <summary>Returns the number of records.</summary>
        /// <returns>Returns 0.</returns>
        public override int GetRecordCount()
        {
            return 0;
        }

        /// <summary>
        /// Gets a concatenated string of all field names of <see cref="TableDescriptor.Fields"/> of the 
        /// <see cref="TableDescriptor"/> this section belongs to.
        /// </summary>
        /// <returns>A string with comma-separated field names.</returns>
        public string FieldsToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("({0}): ", GetType().Name);
            bool first = true;
            foreach (FieldDescriptor fd in ParentTable.TableDescriptor.Fields)
            {
                if (true)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.AppendFormat("{0}", fd.Name);
                }
            }

            return sb.ToString();
        }

        /// <summary>Returns string representation of ColumnHeaderSection.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return FieldsToString();
        }
    }

    /// <summary>
    /// Represents the row for the column header row in a group.
    /// </summary>
    /// <remarks>
    /// A ColumnHeaderRow is displayed at the top of a table or group. ColumnHeaderRow is a place holder 
    /// item where the grouping grid should display column headers. <para/>
    ///  <para/>
    /// ColumnHeaderRow is a RowElement and an element of the ColumnHeaderSection.ColumnHeaderRows 
    /// collection of a parent ColumnHeaderSection. <para/>
    ///  <para/>
    /// ColumnHeaderRows are created and removed internally within a ColumnHeaderSection when the 
    /// section is created and when TableDescriptor.RowsPerRecord property of a TableDescriptor 
    /// is changed.<para/>
    ///  <para/>
    /// See also:<para/>
    /// TableDescriptor.RowsPerRecord, IDisplayElement, ColumnHeaderSection, ColumnHeaderSection.ColumnHeaderRows, 
    /// ColumnHeaderRowCollection
    ///  <para/>
    /// </remarks>
    public class ColumnHeaderRow : RowElement
    {
        /// <summary>
        /// Initializes the new element in the specified ColumnHeaderSection.
        /// </summary>
        /// <param name="parent">The parent section that this element belongs to.</param>
        public ColumnHeaderRow(ColumnHeaderSection parent)
            : base(parent)
        {
        }

        /// <summary>Gets the kind of disply element.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.ColumnHeader; }
        }

        /// <summary>
        /// Gets / sets the ColumnHeaderSection this elements belongs to.
        /// </summary>
        public new ColumnHeaderSection ParentElement
        {
            get
            {
                return (ColumnHeaderSection)base.ParentElement;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>Returns the number of elements.</summary>
        /// <returns>Number of elements.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 1;
        }
               
        /// <returns>returns boolean value, false</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Returns the number of visible elements.</summary>
        /// <returns>Number of visible elements.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Gets the height of column header row.</summary>
        /// <returns>Row Height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultColumnHeaderRowHeight;
        }

        /// <summary>Returns string representation of the ColumnHeaderRow object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            ColumnHeaderSection parent = ParentElement;
            string s = GetType().Name;
            if (parent != null)
            {
                s += " " + parent.FieldsToString();
            }

            return s;
        }
    }

    #region ColumnHeaderRowCollection
    /// <summary>
    /// A collection of <see cref="CaptionRow"/> elements that are children of a <see cref="ColumnHeaderSection"/> in a <see cref="Group"/>.
    /// An instance of this collection is returned by the <see cref="ColumnHeaderSection.ColumnHeaderRows"/> property
    /// of a <see cref="ColumnHeaderSection"/> object.
    /// </summary>
    public class ColumnHeaderRowCollection : IList
    {
        internal RowElementCollection _inner;
        internal ColumnHeaderSection _parentColumnHeaderSection;

        internal ColumnHeaderRowCollection(ColumnHeaderSection parentColumnHeaderSection)
        {
            _inner = parentColumnHeaderSection.RowElements;
            _parentColumnHeaderSection = parentColumnHeaderSection;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public ColumnHeaderRow this[int index]
        {
            get
            {
                return (ColumnHeaderRow)_inner[index];
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
        public bool Contains(ColumnHeaderRow value)
        {
            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(ColumnHeaderRow value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(ColumnHeaderRow[] array, int index)
        {
            _inner.CopyTo(array, index);
        }

        ////        public ColumnHeaderRowCollection SyncRoot
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
        public ColumnHeaderRowCollectionEnumerator GetEnumerator()
        {
            return new ColumnHeaderRowCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, ColumnHeaderRow value)
        {
            _inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(ColumnHeaderRow value)
        {
            _inner.Remove(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(ColumnHeaderRow value)
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
                this[index] = (ColumnHeaderRow)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (ColumnHeaderRow)value);
        }

        void IList.Remove(object value)
        {
            Remove((ColumnHeaderRow)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((ColumnHeaderRow)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((ColumnHeaderRow)value);
        }

        int IList.Add(object value)
        {
            return Add((ColumnHeaderRow)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((ColumnHeaderRow[])array, index);
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
    /// Enumerator class for <see cref="ColumnHeaderRow"/> elements of a <see cref="ColumnHeaderRowCollection"/>.
    /// </summary>
    public class ColumnHeaderRowCollectionEnumerator : IEnumerator
    {
        RowElementCollectionEnumerator inner;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public ColumnHeaderRowCollectionEnumerator(ColumnHeaderRowCollection collection)
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
        public ColumnHeaderRow Current
        {
            get
            {
                return (ColumnHeaderRow)inner.Current;
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