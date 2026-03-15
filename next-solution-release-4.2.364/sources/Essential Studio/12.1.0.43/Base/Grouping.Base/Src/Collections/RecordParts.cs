//-------------------------------------------------------------------------------------------------
// <copyright file="RecordParts.cs" company="syncfusion">
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
    /// A collection of <see cref="RecordPart"/> elements that are children of a <see cref="Record"/>.
    /// An instance of this collection is returned by the <see cref="Record.RecordParts"/> property
    /// of a <see cref="Record"/> object.
    /// </summary>
    public class RecordPartInRecordCollection : IList, IDisposable
    {
        internal RecordPartsTreeTable _inner;
        internal Record _parentRecord;

        internal RecordPartInRecordCollection(Record parentRecord)
        {
            _inner = parentRecord.RecordPartEntries;
            _parentRecord = parentRecord;
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            _inner = null;
            _parentRecord = null;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public RecordPart this[int index]
        {
            get
            {
                // --> calls EnsureInitalized
                if (index < 0 || index >= Count)  
                {
                    throw new ArgumentOutOfRangeException();
                }

                RecordPartsTreeTableEntry entry = _inner[index] as RecordPartsTreeTableEntry;
                if (entry == null)
                {
                    return null;
                }

                return entry.RecordPart;
            }

            set
            {
                RecordPart recordPart = value;
                RecordPartsTreeTableEntry recordPartEntry = new RecordPartsTreeTableEntry();
                recordPartEntry.RecordPart = recordPart;
                recordPartEntry.Tree = _inner.TreeTable;

                recordPart.ParentElement = _parentRecord;
                recordPart.RecordPartEntry = recordPartEntry;
                _inner[index] = recordPartEntry;
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(RecordPart value)
        {
            if (value == null)
            {
                return false;
            }

            ////value.EnsureInitialized(this, false);
            return value.ParentElement == _parentRecord;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(RecordPart value)
        {
            if (!Contains(value))
            {
                return -1;
            }

            return value.RecordPartEntry.GetPosition();
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void CopyTo(RecordPart[] array, int index)
        {
            int n = 0;
            foreach (RecordPart recordPart in this)
            {
                array[index + n] = recordPart;
                n++;
            }
        }

        ////        public RecordPartInRecordCollection SyncRoot
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
        public RecordPartInRecordCollectionEnumerator GetEnumerator()
        {
            return new RecordPartInRecordCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="recordPart">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void Insert(int index, RecordPart recordPart)
        {
            // Count calls EnsureInitialized
            if (index < 0 || index >= Count) 
            {
                throw new ArgumentOutOfRangeException();
            }

            RecordPartsTreeTableEntry recordPartEntry = new RecordPartsTreeTableEntry();
            recordPartEntry.RecordPart = recordPart;
            recordPartEntry.Tree = _inner.TreeTable;

            recordPart.ParentElement = _parentRecord;
            recordPart.RecordPartEntry = recordPartEntry;

            _inner.Insert(index, recordPartEntry);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(RecordPart value)
        {
            int index = IndexOf(value); // (does not call EnsureInitialized)
            if (index != -1)
            {
                _inner.RemoveAt(index);
            }
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="recordPart">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(RecordPart recordPart)
        {
            RecordPartsTreeTableEntry recordPartEntry = new RecordPartsTreeTableEntry();
            recordPartEntry.RecordPart = recordPart;
            recordPartEntry.Tree = _inner.TreeTable;

            recordPart.ParentElement = _parentRecord;
            recordPart.RecordPartEntry = recordPartEntry;

            return _inner.Add(recordPartEntry);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        void IList.Clear()
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
        /// table if changes have been made to the Table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// The method calls <see cref="Element.EnsureInitialized"/> of the <see cref="Record"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_parentRecord == null)
                {
                    return 0;
                }

                _parentRecord.EnsureInitialized(this);
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
                this[index] = (RecordPart)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (RecordPart)value);
        }

        void IList.Remove(object value)
        {
            Remove((RecordPart)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((RecordPart)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((RecordPart)value);
        }

        int IList.Add(object value)
        {
            return Add((RecordPart)value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((RecordPart[])array, index);
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
    /// Enumerator class for <see cref="RecordPart"/> elements of a <see cref="RecordPartInRecordCollection"/>.
    /// </summary>
    public class RecordPartInRecordCollectionEnumerator : IEnumerator
    {
        RecordPart _cursor, _next;
        IList _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public RecordPartInRecordCollectionEnumerator(RecordPartInRecordCollection collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0] as RecordPart;
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0] as RecordPart;
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
        public RecordPart Current
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

            _next = (RecordPart)ElementHelper.GetNextSibling(_next); ////_next.GetNextSibling();

            return _cursor != null;
        }
        #endregion
    }
}

