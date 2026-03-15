//-------------------------------------------------------------------------------------------------
// <copyright file="TableCollection.cs" company="syncfusion">
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
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A collection of <see cref="Table"/> elements that are children of a parent <see cref="Table"/>.
    /// An instance of this collection is returned by the <see cref="Table.RelatedTables"/> property
    /// of a <see cref="Table"/> object. The collection is internally populated from criteria specified
    /// with the <see cref="TableDescriptor.Relations"/> collection.
    /// </summary>
    [ListBindableAttribute(false)]
    public class TableCollection : IList, IDisposable
    {
        ArrayList inner;
        Table parentTable;
        internal int relationDescriptorVersion = -1;
        internal int tableSourceListVersion = -1;
        SortedList _sortNestedNames;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly TableCollection Empty = new TableCollection(null);

        #region IDisposable Members

        /// <summary>
        /// Disposes of the object and resets references to objects.
        /// </summary>
        public void Dispose()
        {
            if (inner != null)
            {
                inner.Clear();
            }

            parentTable = null;
        }

        #endregion

        /// <summary>
        /// Initializes the collection that belongs to the specified parent table.
        /// </summary>
        /// <param name="parentTable">The table this collection belongs to.</param>
        public TableCollection(Table parentTable)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            inner = new ArrayList();
            this.parentTable = parentTable;
        }

        /// <summary>
        /// Initializes the collection that belongs to the specified parent table.
        /// </summary>
        /// <param name="parentTable">The table this collection belongs to.</param>
        /// <param name="inner">The inner list that should be attached to this collection.</param>
        internal TableCollection(Table parentTable, ArrayList inner)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            this.inner = inner;
            this.parentTable = parentTable;
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="tables">The array whose elements should be added to the end of the collection. 
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic). 
        /// </param>
        public void AddRange(Table[] tables)
        {
            _sortNestedNames = null;

            ////TraceUtil.TraceCurrentMethodInfo();
            this.inner.AddRange(tables);
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public Table this[int index]
        {
            get
            {
                this.SynchronizeWithRelationDescriptor();
                Table table = (Table)inner[index];
                return table;
            }

            set
            {
                ////TraceUtil.TraceCurrentMethodInfo();
                inner[index] = value;
                _sortNestedNames = null;
            }
        }

        int FindName(string name)
        {
            return this.parentTable.ParentTableDescriptor.Relations.IndexOf(name);
        }

        int FindNestedName(string name)
        {
            if (_sortNestedNames == null)
            {
                _sortNestedNames = new SortedList();
                int n = 0;
                for (int i = 0; i < inner.Count; i++)
                {
                    Table table = (Table)inner[i];
                    RelationDescriptor rd = table.TableDescriptor.ParentRelation;
                    if (rd.RelationKind == RelationKind.RelatedMasterDetails || rd.RelationKind == RelationKind.UniformChildList)
                    {
                        _sortNestedNames.Add(table.TableDescriptor.Name, n++);
                    }
                }
            }

            if(_sortNestedNames.Contains(name))
                return (int)_sortNestedNames[name];
            return -1;
        }

        /// <summary>
        /// Gets the table with the specified name.
        /// </summary>
        public Table this[string name]
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
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOfNestedTable(string name)
        {
            return FindNestedName(name);
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(Table value)
        {
            if (value == null)
            {
                return false;
            }

            return inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(Table value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(Table[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        TableCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public TableCollectionEnumerator GetEnumerator()
        {
            return new TableCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, Table value)
        {
            _sortNestedNames = null;
            ////TraceUtil.TraceCurrentMethodInfo();
            inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(Table value)
        {
            _sortNestedNames = null;
            inner.Remove(value);
        }

        /// <summary>
        /// Adds a table to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(Table value)
        {
            _sortNestedNames = null;
            ////TraceUtil.TraceCurrentMethodInfo();
            return inner.Add(value);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            _sortNestedNames = null;
            inner.RemoveAt(index);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            _sortNestedNames = null;
            ////TraceUtil.TraceCurrentMethodInfo();
            inner.Clear();
        }

        /// <summary>
        /// Returns True because this collection is always Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
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
        /// Adds or removes tables from this collection if the table descriptor or relation
        /// descriptor is changed.
        /// </summary>
        public void SynchronizeWithRelationDescriptor()
        {
            parentTable.EnsureSourceList();
            if (relationDescriptorVersion != parentTable.TableDescriptor.Relations.Version
                || tableSourceListVersion != parentTable.SourceListVersion)
            {
                ////                relationDescriptorVersion = parentTable.TableDescriptor.Relations.Version;
                ////                tableSourceListVersion = parentTable.sourceListVersion;
                ////
                parentTable.SynchronizeRelatedTables();
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
                SynchronizeWithRelationDescriptor();
                return inner.Count;
            }
        }

        //// TODO: NestedTables Count

        #region IList Private Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (Table)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (Table)value);
        }

        void IList.Remove(object value)
        {
            Remove((Table)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((Table)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Table)value);
        }

        int IList.Add(object value)
        {
            return Add((Table)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Table[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Private Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="Table"/> items of a <see cref="TableCollection"/>.
    /// </summary>
    public class TableCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        TableCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public TableCollectionEnumerator(TableCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
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
        public Table Current
        {
            get
            {
                return _coll[_cursor];
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
            if (_next == -1)
            {
                return false;
            }

            _cursor = _next;

            _next++;
            if (_next >= _coll.Count)
            {
                _next = -1;
            }

            return _cursor != -1;
        }
        #endregion
    }
}
