//-------------------------------------------------------------------------------------------------
// <copyright file="UnsortedRecords.cs" company="syncfusion">
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
    /// A collection of unsorted <see cref="Record"/> elements that are children of a <see cref="Table"/> and
    /// represent the original records in the same order as the underlying data source. The collection
    /// provides support for determining a record's underlying position in the datasource using the <see cref="IndexOf"/>
    /// method.<para/>
    /// An instance of this collection is returned by the <see cref="Table.UnsortedRecords"/> property
    /// of a <see cref="Table"/> object.
    /// </summary>
    public class UnsortedRecordsCollection : IList, IDisposable
    {
        internal UnsortedRecordsTree _inner;
        internal Table _table;

        internal UnsortedRecordsCollection(UnsortedRecordsTree inner, Table table)
        {
            // Make sure inner is updated whenever
            // Table.unsortedRecordsTree is changed.
            _inner = inner;
            _table = table;
        }

        /// <summary>
        /// Disposes of the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            _inner = null;
            _table = null;
        }

        /// <summary>
        /// Gets the element at the zero-based index.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Record this[int index]
        {
            get
            {
                if (_table.VirtualMode)
                {
                    return _table.Records[index];
                }

                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                UnsortedRecordsTreeEntry entry = _inner[index];
                if (entry == null)
                {
                    return null;
                }

                return entry.Record;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Record");
                }

                if (_table.VirtualMode)
                {
                    _table.TopLevelGroup.Records._inner.EnsureVirtualSortedRecordsTreeTableEntry(value, index);
                    _table.TopLevelGroup.Records._inner[index] = value.SortedEntry;
                    return;
                }

                this.EnsureUnsortedEntry(value);
                _inner[index] = value.UnsortedEntry;
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
        public bool Contains(Record value)
        {
            if (_table.VirtualMode)
            {
                return _table.Records.Contains(value);
            }

            if (value == null)
            {
                return false;
            }

            _table.EnsureInitialized(this);
            return _inner.Contains(value.UnsortedEntry);
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
        public int IndexOf(Record value)
        {
            if (_table.VirtualMode)
            {
                return _table.Records.IndexOf(value);
            }

            _table.EnsureInitialized(this);
            if (value == null)
            {
                return -1;
            }

            return _inner.IndexOf(value.UnsortedEntry);
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
        public void CopyTo(Record[] array, int index)
        {
            int n = 0;
            foreach (Record record in this)
            {
                array[index + n] = record;
                n++;
            }
        }

        /// <summary>
        /// Removes the record from unsorted records tree.
        /// </summary>
        /// <param name="r">Record to remove.</param>
        public void Remove(Record r)
        {
            if (_table.VirtualMode)
            {
                int n = r.sourceIndex;
                _table.TopLevelGroup.Records._inner.RemoveAt(n);
            }

            if (r.UnsortedEntry == null)
            {
                return;
            }

            ITreeTable tree = r.UnsortedEntry.Tree;

            // Remove it from the tree
            tree.Remove(r.UnsortedEntry);
        }
        
        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="pos">The zero-based index at which the element should be inserted.</param>
        /// <param name="r">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int pos, Record r)
        {
            if (_table.VirtualMode)
            {
                if (r.SortedEntry == null)
                {
                    this._table.TopLevelGroup.Records._inner.EnsureVirtualSortedRecordsTreeTableEntry(r, pos);
                }

                _table.TopLevelGroup.Records._inner.Insert(pos, r.SortedEntry);
                return;
            }

            EnsureUnsortedEntry(r);
            ITreeTable tree = r.UnsortedEntry.Tree;
            tree.Insert(pos, r.UnsortedEntry);
        }

        /// <summary>
        /// Returns the next record in the collection.
        /// </summary>
        /// <param name="r">The Record.</param>
        /// <returns>Next record.</returns>
        public Record GetNext(Record r)
        {
            if (r.UnsortedEntry == null)
            {
                return null;
            }

            UnsortedRecordsTreeEntry next = (UnsortedRecordsTreeEntry)r.UnsortedEntry.Tree.GetNextEntry(r.UnsortedEntry);
            if (next != null)
            {
                return next.Record;
            }

            return null;
        }

        /// <summary>
        /// Returns the previous record in the collection.
        /// </summary>
        /// <param name="r">The Record.</param>
        /// <returns>Previous record.</returns>
        public Record GetPrevious(Record r)
        {
            if (r.UnsortedEntry == null)
            {
                return null;
            }

            UnsortedRecordsTreeEntry prev = (UnsortedRecordsTreeEntry)r.UnsortedEntry.Tree.GetPreviousEntry(r.UnsortedEntry);
            if (prev != null)
            {
                return prev.Record;
            }

            return null;
        }

        void EnsureUnsortedEntry(Record record)
        {
            if (record.UnsortedEntry == null || record.UnsortedEntry.Tree != this._inner.TreeTable)
            {
                UnsortedRecordsTreeEntry unsortedEntry = new UnsortedRecordsTreeEntry();
                record.UnsortedEntry = unsortedEntry;
                unsortedEntry.Record = record;
                unsortedEntry.Tree = this._inner.TreeTable;
            }
        }

        /// <summary>
        /// Adds the given record into the collection.
        /// </summary>
        /// <param name="record">The Record.</param>
        /// <returns>Index at which the record is added.</returns>
        public int Add(Record record)
        {
            if (_table.VirtualMode)
            {
                // No need to add anything ...
                this._table.TopLevelGroup.Records._inner.EnsureVirtualSortedRecordsTreeTableEntry(record, Count - 1);
                return Count;
            }

            EnsureUnsortedEntry(record);
            int index = this._inner.Add(record.UnsortedEntry);
            record.sourceIndex = index;
            return index;
        }
        ////        public UnsortedRecordsCollection SyncRoot
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
        public IEnumerator GetEnumerator()
        {
            if (_table.VirtualMode)
            {
                return _table.Records.GetEnumerator();
            }

            _table.EnsureInitialized(this);
            return new UnsortedRecordsCollectionEnumerator(this);
        }

        #region IList Members
        
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

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        /// <summary>
        /// Not supported because collection is Read-only.
        /// </summary>
        /// <param name="index">Record index.</param>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (Record)value);
        }

        void IList.Remove(object value)
        {
            Remove((Record)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((Record)value);
        }

        void IList.Clear()
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Record)value);
        }

        int IList.Add(object value)
        {
            return Add((Record)value);
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

        #endregion

        #region ICollection Members

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
        /// The method calls <see cref="Table.EnsureInitialized"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_table == null)
                {
                    return 0;
                }

                if (_table.VirtualMode)
                {
                    return _table.Records.Count;
                }

                _table.EnsureInitialized(this);
                return _inner.Count;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Record[])array, index);
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
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="UnsortedRecordsCollection"/>.
    /// </summary>
    public class UnsortedRecordsCollectionEnumerator : IEnumerator
    {
        UnsortedRecordsTreeEnumerator _inner;

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
        public UnsortedRecordsCollectionEnumerator(UnsortedRecordsCollection collection)
        {
            _inner = new UnsortedRecordsTreeEnumerator(collection._inner);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _inner.Reset();
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
        public Record Current
        {
            get
            {
                return _inner.Current.Record;
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
            return _inner.MoveNext();
        }
    }
}