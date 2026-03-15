//-------------------------------------------------------------------------------------------------
// <copyright file="RecordsInTable.cs" company="syncfusion">
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
    /// A Read-only collection of sorted <see cref="Record"/> elements that meet filter criteria and are children of a <see cref="Table"/>.
    /// See <see cref="RecordFilterDescriptorCollection"/> or <see cref="TableDescriptor.RecordFilters"/> for filter criteria.
    /// An instance of this collection is returned by the <see cref="Table.FilteredRecords"/> property
    /// of a <see cref="Table"/> object. <para/>
    /// The collection
    /// provides support for determining a record's position in the grouped table using the <see cref="RecordsInTableCollectionBase.IndexOf"/>
    /// method.
    /// </summary>
    public class FilteredRecordsInTableCollection : RecordsInTableCollectionBase
    {
        internal FilteredRecordsInTableCollection(Table table)
            : base(table)
        {
            counterKind = CounterKind.FilteredRecordsCount;
        }
    }

    /// <summary>
    /// A Read-only collection of sorted <see cref="Record"/> elements that are children of a <see cref="Table"/>.
    /// An instance of this collection is returned by the <see cref="Table.Records"/> property
    /// of a <see cref="Table"/> object. This collection contains all records, it is not filtered. <para/>
    /// The collection
    /// provides support for determining a record's position in the grouped table using the <see cref="RecordsInTableCollectionBase.IndexOf"/>
    /// method.
    /// </summary>
    public class RecordsInTableCollection : RecordsInTableCollectionBase
    {
        Table table;

        internal RecordsInTableCollection(Table table)
            : base(table)
        {
            this.table = table;
            counterKind = CounterKind.RecordsCount;
        }

        /// <summary>
        /// Deletes All the records from the table.
        /// </summary>
        public void DeleteAll()
        {
            foreach (Record rec in this.table.Records)
            {
                rec.Delete();
            }
        }

        /// <summary>
        /// Deletes the given collection of records from the table.
        /// </summary>
        /// <param name="records"></param>
        public void DeleteRecords(RecordsInTableCollection records)
        {
            foreach (Record rec in records)
            {
                rec.Delete();
            }
        }

        /// <summary>
        /// Deletes the given collection of records from the table.
        /// </summary>
        /// <param name="records"></param>
        public void DeleteRecords(Record[] records)
        {
            foreach (Record rec in records)
            {
                rec.Delete();
            }
        }

        /// <summary>
        /// Deletes the given collection of selected records from the table.
        /// </summary>
        /// <param name="selectedRecords"></param>
        public void DeleteRecords(SelectedRecordsCollection selectedRecords)
        {
            foreach (SelectedRecord rec in selectedRecords)
            {
                rec.Record.Delete();
            }
        }

        /// <summary>
        /// Selects all the records from the table.
        /// </summary>
        public void SelectAll()
        {
            this.table.SelectedRecords.AddRange(this.table.Records);
        }

        /// <summary>
        /// Deletes the given record from the table.
        /// </summary>
        /// <param name="record"></param>
        public void Delete(Record record)
        {
            if(record != null)
                record.Delete();
        }
    }

    /// <summary>
    /// A Read-only collection base class for <see cref="RecordsInTableCollection"/> 
    /// and <see cref="FilteredRecordsInTableCollection"/>.
    /// </summary>
    public class RecordsInTableCollectionBase : IList, IDisposable
    {
        internal Table _table;
        internal Record lastElement;
        internal int lastIndex;
        internal int version;
        internal int lastCount;
        internal int counterKind = CounterKind.RecordsCount;

        #region IDisposable Members

        /// <summary>
        /// Disposes of the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            _table = null;
            lastElement = null;
        }

        #endregion

        internal RecordsInTableCollectionBase(Table table)
        {
            _table = table;
        }

        internal DetailsSection GetRootDetails()
        {
            return _table.TopLevelGroup.Details;
        }

        /// <summary>
        /// Gets (and caches) the element at the zero-based index.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Record this[int index]
        {
            get
            {
                int count = Count;
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                Element el;
                if (lastIndex != -1 && lastCount == count && version == _table.EngineVersion)
                {
                    if (index == lastIndex)
                    {
                        return lastElement;
                    }
                    else if (index == lastIndex + 1)
                    {
                        lastIndex++;
                        el = lastElement;
                        lastElement = (Record)ElementHelper.GetNextElementStepIn(el, counterKind, typeof(Record));
                        ////                        if (lastElement == null)
                        ////                            lastElement = (Record) ElementHelper.GetNextElementStepIn(el, counterKind, typeof(Record));
                        Debug.Assert(lastElement == ElementHelper.__FindElement(_table, CounterFactory.CreateCounter(index, counterKind), typeof(Record), false));
                        return lastElement;
                    }
                }

                el = ElementHelper.__FindElement(_table, CounterFactory.CreateCounter(index, counterKind), typeof(Record), false);
                lastElement = (Record)Record.GetParentRecord(el);
                lastCount = count;
                lastIndex = index;
                version = _table.EngineVersion;
                return lastElement;
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        ICounterFactory CounterFactory
        {
            get
            {
                return this._table.Engine.CounterFactory;
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
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(Record value)
        {
            if (value == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);
            if (value.ParentTable != _table)
            {
                return false;
            }

            if (counterKind == CounterKind.FilteredRecordsCount)
            {
                return value.GetFilteredRecordCount() > 0;
            }
            else
            {
                return value.GetRecordCount() > 0;
            }
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int IndexOf(Record value)
        {
            if (!Contains(value))
            {
                return -1;
            }

            return (int)ElementHelper.GetCumulatedPosition(value, counterKind);
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
        public void CopyTo(Record[] array, int index)
        {
            int n = 0;
            foreach (Record element in this)
            {
                array[index + n] = element;
                n++;
            }
        }

        ////        public FilteredRecordsInTableCollection SyncRoot
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
        public RecordsInTableCollectionEnumerator GetEnumerator()
        {
            _table.EnsureInitialized(this);
            return new RecordsInTableCollectionEnumerator(this);
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
        /// <param name="index">The List index</param>
        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
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
            throw new InvalidOperationException("Collection is Read-only.");
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
        /// table if changes have been made to the Table or the TableDescriptor.
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

                _table.EnsureInitialized(this);
                if (_table.VirtualMode)
                {
                    return _table.TopLevelGroup.Records.Count;
                }

                return (int)_table.GetCounter().GetValue(counterKind);
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
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="RecordsInTableCollectionBase"/>.
    /// </summary>
    public class RecordsInTableCollectionEnumerator : IEnumerator
    {
        Record _cursor, _next;
        RecordsInTableCollectionBase _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public RecordsInTableCollectionEnumerator(RecordsInTableCollectionBase collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0];
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0];
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

            _next = (Record)ElementHelper.GetNextElementStepIn(_next, _coll.counterKind, typeof(Record));

            if (_next != null && _next.ParentTable != _coll._table)
            {
                _next = null;
            }

            return _cursor != null;
        }
        #endregion
    }
}
