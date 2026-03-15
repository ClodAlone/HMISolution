//-------------------------------------------------------------------------------------------------
// <copyright file="SelectedRecords.cs" company="syncfusion">
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
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A collection of selected <see cref="Record"/> elements that are children of a <see cref="Table"/>.
    /// </summary>
    public class SelectedRecordsCollection : IList, IDisposable
    {
        internal TreeTable _inner;
        internal Table _table;
        bool allowRaiseEvents = true;

        /// <summary>Disables the events.</summary>
        /// <exclude/>
        public void DisableEvents()
        {
            allowRaiseEvents = false;
        }

        /// <summary>Enables the events.</summary>
        /// <exclude/>
        public void EnableEvents()
        {
            allowRaiseEvents = true;
        }

        internal SelectedRecordsCollection(Table table)
        {
            _inner = new TreeTable(true);
            _inner.Comparer = new SelectedRecordComparer();
            _table = table;
        }

        /// <summary>
        /// Disposes of the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            _inner.Dispose();
            _inner = null;
            _table = null;
        }

        /// <overload>
        /// Searches for the occurrence of a record and returns the zero-based index of the occurrence found or -1 if not found.
        /// </overload>
        /// <summary>
        /// Searches for the occurrence of a record and returns the zero-based index of the occurrence found or -1 if not found.
        /// </summary>
        /// <param name="record">The record to search for. </param>    
        /// <returns>Record index.</returns>
        public int FindRecord(Record record)
        {
            if (Count > 0)
            {
                int sourceIndex = _table.UnsortedRecords.IndexOf(record);
                return FindRecord(sourceIndex);
                ////object key = sortKey is DBNull ? sortKey : Convert.ChangeType(sortKey, type);
                ////return _inner.TreeTable.IndexOfKey(new object[] { sortKey });
            }

            return -1;
        }

        /// <summary>
        /// Searches for the occurrence of a record and returns the zero-based index of the occurrence found or -1 if not found.
        /// </summary>
        /// <param name="sourceIndex">The position of the record in the underlying datasource (UnsortedRecords.IndexOf). </param>    
        /// <returns>Record index.</returns>
        public int FindRecord(int sourceIndex)
        {
            if (Count > 0)
            {
                TreeTableEntry entry = (TreeTableEntry)_inner.FindKey(sourceIndex);
                if (entry != null)
                {
                    return _inner.IndexOf(entry);
                }
            }

            return -1;
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
        public SelectedRecord this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                TreeTableEntry entry = (TreeTableEntry)_inner[index];
                if (entry == null)
                {
                    return null;
                }

                return entry.Value as SelectedRecord;
            }

            set
            {
                throw new InvalidOperationException("Collection is sorted by unsorted record positon. Use Add method to add a record.");
            }
        }

        internal SelectedRecord GetInnerItem(int index)
        {
            TreeTableEntry entry = (TreeTableEntry)_inner[index];
            if (entry == null)
            {
                return null;
            }

            return entry.Value as SelectedRecord;
        }

        /// <overload>
        /// Determines if the element belongs to this collection.
        /// </overload>
        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(SelectedRecord value)
        {
            if (value == null)
            {
                return false;
            }

            _table.EnsureInitialized(this);
            return _inner.Contains(value.Entry);
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="record">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(Record record)
        {
            return FindRecord(record) != -1;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <overload>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </overload>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(SelectedRecord value)
        {
            _table.EnsureInitialized(this);
            if (value == null)
            {
                return -1;
            }

            return _inner.IndexOf(value.Entry);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(Record value)
        {
            return FindRecord(value);
        }

        /// <overload>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </overload>
        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(SelectedRecord[] array, int index)
        {
            int n = 0;
            foreach (SelectedRecord selectedRecord in this)
            {
                array[index + n] = selectedRecord;
                n++;
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(Record[] array, int index)
        {
            int n = 0;
            foreach (SelectedRecord selectedRecord in this)
            {
                array[index + n] = selectedRecord.Record;
                n++;
            }
        }

        ////        public SelectedRecordsCollection SyncRoot
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
        public SelectedRecordsCollectionEnumerator GetEnumerator()
        {
            _table.EnsureInitialized(this);
            return new SelectedRecordsCollectionEnumerator(this);
        }

        /// <overload>
        /// Addes a record to the collection of selected records and marks it as selected.
        /// </overload>
        /// <summary>
        /// Addes a record to the collection of selected records and marks it as selected.
        /// </summary>
        /// <param name="value">A SelectedRecord to add.</param>
        /// <returns>Index at which the record is added.</returns>
        public int Add(SelectedRecord value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            SelectedRecordsChangedEventArgs e = new SelectedRecordsChangedEventArgs(_table, SelectedRecordsChangedType.Added, value);
            if (allowRaiseEvents)
            {
                _table.RaiseSelectedRecordsChanging(e);
            }

            if (!e.Cancel)
            {
                try
                {
                    TreeTableEntry entry = new TreeTableEntry();
                    entry.Value = value;
                    value.Entry = entry;
                    entry.Tree = _inner;
                    return _inner.Add(value.Entry);
                }
                finally
                {
                    if (allowRaiseEvents)
                    {
                        _table.RaiseSelectedRecordsChanged(e);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Addes a record to the collection of selected records and marks it as selected.
        /// </summary>
        /// <param name="value">Record to add.</param>
        /// <returns>Index at which the record is added.</returns>
        public int Add(Record value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            SelectedRecord selectedRecord = new SelectedRecord(value);
            return Add(selectedRecord);
        }

        /// <summary>
        /// Deletes all the selected records.
        /// </summary>
        ///<remark>
        /// A call to SeltectedRecords.DeleteAll does not just unselect the selected records. This method actually removes the selected records from the Records being displayed in the GridGroupingControl..
        ///</remark>
        public void DeleteAll()
        {
            if (this.Count > 0)
            {
                foreach (SelectedRecord rec in this)
                {
                    rec.Record.Delete();
                }
            }
        }

        /// <summary>
        /// Adds the collection of records  to the Selected records collecton and marks them as selected.
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public int[] AddRange(RecordsInTableCollection records)
        {
            int[] indices = new int[records.Count];
            int i = 0;
            foreach(Record record in records)
            {
                indices[i]= this.Add(record);
                i++;
            }
            return indices;
        }

        /// <summary>
        /// Adds the collection of records  to the Selected records collecton and marks them as selected.
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public int[] AddRange(Record[] records)
        {   
            int[] indices = new int[records.Length];
            int i = 0;
            foreach (Record record in records)
            {
                indices[i] = this.Add(record);
                i++;
            }
            return indices;
        }

        /// <summary>
        /// Removes the collection of records from Selected records collecton and marks them as deselected.
        /// </summary>
        /// <param name="records"></param>
        public void RemoveRange(RecordsInTableCollection records)
        {
            foreach (Record record in records)
            {
                this.Remove(record);
            }
        }

        /// <summary>
        /// Removes the collection of records from Selected records collecton and marks them as deselected.
        /// </summary>
        /// <param name="records"></param>
        public void RemoveRange(Record[] records)
        {
            foreach (Record record in records)
            {
                this.Remove(record);
            }
        }

        /// <summary>
        /// Removes the collection of selected records and marks them as deselected.
        /// </summary>
        /// <param name="records"></param>
        public void RemoveRange(SelectedRecordsCollection records)
        {
            foreach (SelectedRecord record in records)
            {
                this.Remove(record);
            }
        }

        /// <summary>
        /// Removes the collection of selected records and marks them as deselected.
        /// </summary>
        /// <param name="records"></param>
        public void RemoveRange(SelectedRecord[] records)
        {
            foreach (SelectedRecord record in records)
            {
                this.Remove(record);
            }
        }

        /// <summary>
        /// Removes a record the collection of selected records and marks it as deselected.
        /// </summary>
        /// <overload>
        /// Removes a record the collection of selected records and marks it as deselected.
        /// </overload>
        /// <param name="value">Record to remove.</param>
        public void Remove(SelectedRecord value)
        {
            if (value == null)
            {
                return;
            }

            SelectedRecordsChangedEventArgs e = new SelectedRecordsChangedEventArgs(_table, SelectedRecordsChangedType.Removed, value);
            if (allowRaiseEvents)
            {
                _table.RaiseSelectedRecordsChanging(e);
            }

            if (!e.Cancel)
            {
                try
                {
                    _inner.Remove(value.Entry);
                }
                finally
                {
                    if (allowRaiseEvents)
                    {
                        _table.RaiseSelectedRecordsChanged(e);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a record from the collection of selected records and marks it as deselected.
        /// </summary>
        /// <param name="value">Record to remove.</param>
        public void Remove(Record value)
        {
            if (value == null)
            {
                return;
            }

            int index = this.FindRecord(value);
            if (index != -1)
            {
                SelectedRecordsChangedEventArgs e = new SelectedRecordsChangedEventArgs(_table, SelectedRecordsChangedType.Removed, this[index]);
                if (allowRaiseEvents)
                {
                    _table.RaiseSelectedRecordsChanging(e);
                }

                if (!e.Cancel)
                {
                    try
                    {
                        _inner.Remove(e.SelectedRecord.Entry);
                    }
                    finally
                    {
                        if (allowRaiseEvents)
                        {
                            _table.RaiseSelectedRecordsChanged(e);
                        }
                    }
                }
            }
        }

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public void InternalClear()
        {
            _inner.Clear();
        }

        /// <summary>
        /// Removes all records from the collection of selected records and marks them as deselected.
        /// </summary>
        public void Clear()
        {
            SelectedRecordsChangedEventArgs e = new SelectedRecordsChangedEventArgs(_table, SelectedRecordsChangedType.Reset, null);
            if (allowRaiseEvents)
            {
                _table.RaiseSelectedRecordsChanging(e);
            }

            if (!e.Cancel)
            {
                try
                {
                    _inner.Clear();
                }
                finally
                {
                    if (allowRaiseEvents)
                    {
                        _table.RaiseSelectedRecordsChanged(e);
                    }
                }
            }
        }

        /// <summary>
        /// Returns False since this collection is not read only.
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
        
        #region IList Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
            }
        }

        /// <summary>
        /// Not supported because collection is Read-only.
        /// </summary>
        /// <param name="index">Index of the reord to be removed.</param>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Remove(object value)
        {
            Remove((SelectedRecord)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((SelectedRecord)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Record)value);
        }

        int IList.Add(object value)
        {
            return Add((SelectedRecord)value);
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

                _table.EnsureInitialized(this);
                return _inner.GetCount();
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
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="SelectedRecordsCollection"/>.
    /// </summary>
    public class SelectedRecordsCollectionEnumerator : IEnumerator
    {
        TreeTableEnumerator _inner;

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
        public SelectedRecordsCollectionEnumerator(SelectedRecordsCollection collection)
        {
            _inner = new TreeTableEnumerator(collection._inner);
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
        public SelectedRecord Current
        {
            get
            {
                return _inner.Current.Value as SelectedRecord;
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

    /// <summary>
    /// A SelectedRecord class is a wrapper around a <see cref="Record"/>. It is used
    /// to add records to the <see cref="SelectedRecordsCollection"/>.
    /// </summary>
    public class SelectedRecord : ITreeTableEntrySource, IDisposable
    {
        Record record;
        TreeTableEntry selectedRecordEntry;

        /// <summary>
        /// Creates a <see cref="SelectedRecord"/> entry for a <see cref="Record"/>
        /// </summary>
        /// <param name="r">The Record.</param>
        public SelectedRecord(Record r)
        {
            this.record = r;
        }

        internal TreeTableEntry Entry
        {
            get
            {
                return selectedRecordEntry;
            }

            set
            {
                selectedRecordEntry = value;
            }
        }

        /// <summary>
        /// The <see cref="Record"/> that is reprented by this object.
        /// </summary>
        public Record Record
        {
            get
            {
                return record;
            }
        }

        internal void InternalSetRecord(Record r)
        {
            // Virtual Mode support - swap record without raising events
            record = r;
        }

        /// <summary>
        /// The position of the record in the underlying datasource (UnsortedRecords.IndexOf)
        /// </summary>
        /// <returns>Record position.</returns>
        public int GetSourcePosition()
        {
            return record.GetSourceIndex();
        }

        #region ITreeTableEntrySource Members

        ITreeTableEntry ITreeTableEntrySource.Entry
        {
            get
            {
                return Entry;
            }

            set
            {
                Entry = (TreeTableEntry)value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            record = null;
            if (selectedRecordEntry != null)
            {
                selectedRecordEntry.Tree.Remove(selectedRecordEntry);
            }

            selectedRecordEntry = null;
        }

        #endregion
    }

    internal class SelectedRecordComparer : IComparer
    {
        public SelectedRecordComparer()
        {
        }

        public int Compare(object x, object y)
        {
            SelectedRecord ry = (SelectedRecord)y;
            if (x is SelectedRecord)
            {
                SelectedRecord rx = (SelectedRecord)x;

                return rx.GetSourcePosition() - ry.GetSourcePosition();
            }
            else 
            {
                //// This branch is used when called from FindRecord.
                int sourceIndex = (int)x;
                return sourceIndex - ry.GetSourcePosition();
            }
        }
    }
}