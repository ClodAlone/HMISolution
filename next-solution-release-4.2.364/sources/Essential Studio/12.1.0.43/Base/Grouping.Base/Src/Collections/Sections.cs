//-------------------------------------------------------------------------------------------------
// <copyright file="Sections.cs" company="syncfusion">
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
using System.Diagnostics;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A collection of <see cref="Section"/> elements that are children of a <see cref="Group"/>.
    /// An instance of this collection is returned by the <see cref="Group.Sections"/> property
    /// of a <see cref="Group"/> object.
    /// </summary>
    public class SectionInGroupCollection : IList, IDisposable
    {
        internal SectionsTreeTable _inner;
        internal Group _parentGroup;

        internal SectionInGroupCollection(Group parentGroup)
        {
            _inner = parentGroup.SectionEntries;
            _parentGroup = parentGroup;
        }

        /// <summary>
        /// Disposes the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            _inner = null;
            _parentGroup = null;
        }
        
        /// <summary>
        /// Gets / sets the element at the zero-based index without <see cref="Element.EnsureInitialized"/> being called.
        /// </summary>
        /// <param name="index">Element index.</param>
        /// <returns>Element at the given index.</returns>
        public Section GetInnerItem(int index)
        {
            SectionsTreeTableEntry entry = _inner.GetInnerItem(index) as SectionsTreeTableEntry;
            if (entry == null)
            {
                return null;
            }

            return entry.Section;
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
        public Section this[int index]
        {
            get
            {
                if (index < 0 || index >= InnerCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SectionsTreeTableEntry entry = _inner[index] as SectionsTreeTableEntry;
                if (entry == null)
                {
                    return null;
                }

                return entry.Section;
            }

            set
            {
                Section section = value;
                SectionsTreeTableEntry sectionEntry = new SectionsTreeTableEntry();
                sectionEntry.Section = section;
                sectionEntry.Tree = _inner.TreeTable;

                section.ParentElement = _parentGroup;
                section.SectionEntry = sectionEntry;
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
        public bool Contains(Section value)
        {
            if (value == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);
            return value.ParentGroup == _parentGroup;
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
        public int IndexOf(Section value)
        {
            // calls EnsureInitialized
            if (!Contains(value)) 
            {
                return -1;
            }

            return value.SectionEntry.GetPosition();
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void CopyTo(Section[] array, int index)
        {
            int n = 0;
            foreach (Section section in this)
            {
                array[index + n] = section;
                n++;
            }
        }

        ////        public SectionInGroupCollection SyncRoot
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
        public SectionInGroupCollectionEnumerator GetEnumerator()
        {
            return new SectionInGroupCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="section">The element to insert. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void Insert(int index, Section section)
        {
            // Count calls EnsureInitialized
            if (index < 0 || index >= Count) 
            {
                throw new ArgumentOutOfRangeException();
            }

            SectionsTreeTableEntry sectionEntry = new SectionsTreeTableEntry();
            sectionEntry.Section = section;
            sectionEntry.Tree = _inner.TreeTable;

            section.ParentElement = _parentGroup;
            section.SectionEntry = sectionEntry;

            _inner.Insert(index, sectionEntry);
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
        public void Remove(Section value)
        {
            int index = IndexOf(value); // IndexOf calls EnsureInitialized
            if (index != -1)
            {
                _inner.RemoveAt(index);
            }
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="section">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(Section section)
        {
            SectionsTreeTableEntry sectionEntry = new SectionsTreeTableEntry();
            sectionEntry.Section = section;
            sectionEntry.Tree = _inner.TreeTable;

            section.ParentElement = _parentGroup;
            section.SectionEntry = sectionEntry;

            return _inner.Add(sectionEntry);
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
        /// The method calls the <see cref="Element.EnsureInitialized"/> of the <see cref="Group"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_parentGroup == null)
                {
                    return 0;
                }

                _parentGroup.EnsureInitialized(this);
                return _inner.Count;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection without calling the <see cref="Element.EnsureInitialized"/> of the <see cref="Group"/>.
        /// </summary>
        public int InnerCount
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
                this[index] = (Section)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (Section)value);
        }

        void IList.Remove(object value)
        {
            Remove((Section)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((Section)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Section)value);
        }

        int IList.Add(object value)
        {
            return Add((Section)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Section[])array, index);
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
    /// Enumerator class for <see cref="Section"/> elements of a <see cref="SectionInGroupCollection"/>.
    /// </summary>
    public class SectionInGroupCollectionEnumerator : IEnumerator
    {
        Section _cursor, _next;
        IList _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public SectionInGroupCollectionEnumerator(SectionInGroupCollection collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0] as Section;
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0] as Section;
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
        public Section Current
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
        /// True if the enumerator is successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == null)
            {
                return false;
            }

            _cursor = _next;

            _next = (Section)ElementHelper.GetNextSibling(_next);

            return _cursor != null;
        }
        #endregion
    }
}
