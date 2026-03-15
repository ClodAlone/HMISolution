//-------------------------------------------------------------------------------------------------
// <copyright file="CaptionSection.cs" company="syncfusion">
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
    /// The section with the CaptionRow:
    /// </summary>
    /// <remarks>
    /// This is the first section within a group which provides the caption bar above the
    /// column headers. CaptionSection is a display element and will be an item returned by
    /// the Table.DisplayElements and Table.GroupedElements collection.<para/>
    /// <para/>
    /// CaptionSection is also a section that can be accessed through the Group.Sections
    /// collection of its parent group.
    ///  <para/>
    /// If a caption should be displayed at the beginning of each group, then CaptionSection
    /// (s) are created when the grouping for a table is initialized. The grouping engine
    /// loops through all sorted records and categorizes them. For each new group, the virtual
    /// TableDescriptor.CreateGroup factory method is called. The TableDescriptor.CreateGroup method instantiates a CaptionSection by calling the
    /// virtual TableDescriptor.CreateCaptionSection factory method. <para/>
    /// </remarks>
    /// <example>The following example creates a string based on the CaptionSection's parent
    /// Group number of direct child elements (as usually shown in the caption element for a
    ///  group in GroupingGrid). Use the ParentGroup property to get access to the parent
    ///  Group of this CaptionSection:
    /// <code lang="C#">
    ///             CaptionSection cs;
    ///             object cat = cs.ParentGroup.Category;
    ///             if (cat == null)
    ///                 cat = "(null)";
    ///             string caption = "{2} Item(s)";
    ///             string groupName = cs.ParentGroup.Name;
    ///             if (groupName != "")
    ///                 caption = "{0}: {1} - " + caption;
    ///             return string.Format(caption, groupName, cat, cs.ParentGroup.GetChildCount());
    /// </code>
    /// <para/>
    /// See also:<para/>
    /// TableDescriptor.CreateGroup, TableDescriptor.CreateCaptionSection, Group.Caption,
    /// Group.Sections, IDisplayElement
    /// </example>
    public class CaptionSection : RowElementsSection
    {
        CaptionRowCollection _captionRowRows;

        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public CaptionSection(Group parent)
            : base(parent)
        {
        }

        #region SupportsId

        /// <summary>Determines if this object can be uniquely identified using its Id.</summary>
        /// <returns>True if Id is supported; False otherwise.</returns>
        /// <override/>
        public override bool SupportsId()
        {
            return true;
        }

        int id;

        /// <summary>Gets or sets the id used to lookup objects (used by internal caches).</summary>
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
        #endregion

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            _captionRowRows = null;
            base.Dispose(disposing);
        }

        /// <summary>Gets the display element kind.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Caption; }
        }

        /// <summary>Returns the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.VisibleCount;
            }

            return 1;
        }

        /// <summary>Returns the height of the caption.</summary>
        /// <returns>Caption height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.YAmountCount;
            }

            return ParentTable.DefaultCaptionRowHeight;
        }

        /// <summary>Returns the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            if (this.rowElementTable != null)
            {
                return rowElementTable.ElementCount;
            }

            return 1;
        }

        /// <summary>Returns the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <summary>Returns the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 0;
        }

        internal sealed override void OnCreatedTreeTable(RowElementsTreeTable treeEntries)
        {
            //// Add one AddNewRecord into element tree.
            CaptionRow rowElement = Engine.CreateCaptionRow(this);
            RowElementsTreeTableEntry rowElementEntry = new RowElementsTreeTableEntry();
            rowElementEntry.Element = rowElement;
            rowElementEntry.Tree = treeEntries.TreeTable;
            rowElement.ParentElement = this;
            rowElement.RowElementEntry = rowElementEntry;
            treeEntries.Add(rowElementEntry);
            ////CaptionRows.Add(row);
            ////base.OnCreatedTreeTable ();
        }

        /// <summary>
        /// The collection of <see cref="CaptionRow"/> elements.
        /// </summary>
        public CaptionRowCollection CaptionRows
        {
            get
            {
                if (_captionRowRows == null)
                {
                    _captionRowRows = new CaptionRowCollection(this);
                }

                return _captionRowRows;
            }
        }
    }

    /// <summary>
    /// Represents the row for the caption bar of a group.
    /// </summary>
    public class CaptionRow : RowElement
    {
        /// <summary>
        /// Initializes the new element in the specified CaptionSection.
        /// </summary>
        /// <param name="parent">The parent that this element belongs to.</param>
        public CaptionRow(CaptionSection parent)
            : base(parent)
        {
        }

        /// <summary>Gets the display element kind.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Caption; }
        }

        /// <summary>
        /// Gets / sets the CaptionSection this elements belongs to.
        /// </summary>
        public new CaptionSection ParentElement
        {
            get
            {
                return (CaptionSection)base.ParentElement;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>Returns 1 (element count).</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 1;
        }

        /// <summary>Returns 1(visible elements count).</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Gets the height for the element.</summary>
        /// <returns>Returns height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultCaptionRowHeight;
        }

        /// <summary>Returns the string representation of CaptionRow object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            CaptionSection cs = this.ParentSection as CaptionSection;
            string s = GetType().Name;
            if (cs != null)
            {
                s += " " + cs.ParentGroup.CategoriesToString();
            }

            return s;
        }
    }

    #region CaptionRowCollection

    /// <summary>
    /// A collection of <see cref="CaptionRow"/> elements that are children of a <see cref="CaptionSection"/> in a <see cref="Group"/>.
    /// An instance of this collection is returned by the <see cref="CaptionSection.CaptionRows"/> property
    /// of a <see cref="CaptionSection"/> object.
    /// </summary>
    public class CaptionRowCollection : IList
    {
        internal RowElementCollection _inner;
        internal CaptionSection _parentCaptionSection;

        internal CaptionRowCollection(CaptionSection parentCaptionSection)
        {
            _inner = parentCaptionSection.RowElements;
            _parentCaptionSection = parentCaptionSection;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        /// <param name="index">Key to identify the requested element.</param>
        public CaptionRow this[int index]
        {
            get
            {
                return (CaptionRow)_inner[index];
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
        public bool Contains(CaptionRow value)
        {
            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(CaptionRow value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(CaptionRow[] array, int index)
        {
            _inner.CopyTo(array, index);
        }

        ////        public CaptionRowCollection SyncRoot
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
        public CaptionRowCollectionEnumerator GetEnumerator()
        {
            return new CaptionRowCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, CaptionRow value)
        {
            _inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(CaptionRow value)
        {
            _inner.Remove(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(CaptionRow value)
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
                this[index] = (CaptionRow)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (CaptionRow)value);
        }

        void IList.Remove(object value)
        {
            Remove((CaptionRow)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((CaptionRow)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((CaptionRow)value);
        }

        int IList.Add(object value)
        {
            return Add((CaptionRow)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((CaptionRow[])array, index);
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
    /// Enumerator class for <see cref="CaptionRow"/> elements of a <see cref="CaptionRowCollection"/>.
    /// </summary>
    public class CaptionRowCollectionEnumerator : IEnumerator
    {
        RowElementCollectionEnumerator inner;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public CaptionRowCollectionEnumerator(CaptionRowCollection collection)
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
        public CaptionRow Current
        {
            get
            {
                return (CaptionRow)inner.Current;
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