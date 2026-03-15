//-------------------------------------------------------------------------------------------------
// <copyright file="RecordsInDetails.cs" company="syncfusion">
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

using Syncfusion.Diagnostics;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Grouping;
using Syncfusion.ComponentModel;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A Read-only collection of <see cref="Record"/> elements that are children of a <see cref="RecordsDetails"/> section.
    /// An instance of this collection is returned by the <see cref="RecordsDetails.Records"/> property
    /// of a <see cref="RecordsDetails"/> object. The <see cref="Group.Records"/> property of a <see cref="Group"/> also
    /// returns an instance of this collection if the group's details section contains records (and not groups). Otherwise an
    /// empty collection is returned.
    /// </summary>
    public class RecordsInDetailsCollection : IList, IDisposable
    {
        internal SortedRecordsTreeTable _inner;
        internal RecordsDetails _groupWithRecords;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static RecordsInDetailsCollection Empty = new RecordsInDetailsCollection(null);

        #region IDisposable Members

        /// <summary>
        /// Disposes the object and resets reference to objects.
        /// </summary>
        public void Dispose()
        {
            _inner = null;
            _groupWithRecords = null;
        }

        #endregion

        internal RecordsInDetailsCollection(RecordsDetails groupWithRecords)
        {
            _groupWithRecords = groupWithRecords;
            if (_groupWithRecords != null)
            {
                _inner = groupWithRecords.RecordTreeEntries;
            }
        }

        /// <summary>
        /// Searches for the first occurrence of a record that matches the specified sortkey and returns the zero-based index of the first occurrence found or -1 if not found.
        /// </summary>
        /// <param name="sortKey">The sort key to search for. The object will be compared to the sorted fields in the table as specified with <see cref="TableDescriptor.SortedColumns"/>.</param>
        /// <returns>Index of the record.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int FindRecord(object sortKey)
        {
            if (_groupWithRecords != null && _groupWithRecords.ParentGroup != null)
            {
                _groupWithRecords.EnsureInitialized(this);
                int c = this._groupWithRecords.ParentTable.GetVisibleCount();
                ////Console.WriteLine(this._groupWithRecords.ParentTable);
                if (this._groupWithRecords.sortedColumns.Count == 0)
                {
                    return c > 0 ? 0 : -1;
                }

                Type type = this._groupWithRecords.sortedColumns[0].FieldDescriptor.GetPropertyType();
                object key = NullableHelper.ChangeType(sortKey, type);
                return _inner.TreeTable.IndexOfKey(new object[] { sortKey });
            }

            return -1;
        }

        /// <summary>
        /// Searches for the first occurrence of a record that matches the specified sortkeys and returns the zero-based index of the first occurrence found or -1 if not found.
        /// </summary>
        /// <param name="sortKeys">The sort keys to search for. The object will be compared to the sorted fields in the table as specified with <see cref="TableDescriptor.SortedColumns"/>. The sortKeys
        /// must have the same order of fields as the <see cref="TableDescriptor.SortedColumns"/> collection. </param>
        /// <returns>Record index.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// product21.ParentTable.TableDescriptor.SortedColumns.Add("ProductName");
        /// product21.ParentTable.TableDescriptor.SortedColumns.Add("SupplierID");
        /// int sp = product21.Records.FindRecord("Spegesild", "21");
        /// product21.Records["Spegesild"].SetCurrent();
        /// </code>
        /// <code lang="VB">
        /// product21.ParentTable.TableDescriptor.SortedColumns.Add("ProductName")
        /// product21.ParentTable.TableDescriptor.SortedColumns.Add("SupplierID")
        /// Dim sp As Integer = product21.Records.FindRecord("Spegesild", "21")
        /// product21.Records("Spegesild").SetCurrent()
        /// </code>
        /// </example>
        public int FindRecord(params object[] sortKeys)
        {
            if (_groupWithRecords != null && _groupWithRecords.ParentGroup != null)
            {
                object[] ax = sortKeys;
                int n = 0;
                try
                {
                    foreach (SortColumnDescriptor columnDescriptor in _groupWithRecords.ParentTableDescriptor.SortedColumns)
                    {
                        if (n >= ax.Length)
                        {
                            break;
                        }

                        if (sortKeys[n] == null || sortKeys[n] is DBNull)
                        {
                            ax[n] = null;
                        }
                        else
                        {
                            ax[n] = NullableHelper.ChangeType(sortKeys[n], columnDescriptor.FieldDescriptor.GetPropertyType());
                        }

                            n++;
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    return -1;
                }

                _groupWithRecords.EnsureInitialized(this);
                int c = this._groupWithRecords.ParentTable.GetVisibleCount();
                ////Console.WriteLine(this._groupWithRecords.ParentTable);
                return _inner.TreeTable.IndexOfKey(ax);
            }

            return -1;
        }

        /// <summary>
        /// Searches for the first occurrence of a record that matches the sortkey and returns the record found or NULL if not found.
        /// </summary>
        /// <param name="sortKey">The sort key to search for. The sortKey will be compared to the sorted field in the table as specified with <see cref="TableDescriptor.SortedColumns"/>.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Record this[string sortKey]
        {
            get
            {
                if (sortKey == null)
                {
                    throw new ArgumentNullException("sortKey");
                }

                int index = FindRecord(sortKey);
                if (index == -1)
                {
                    throw new ArgumentException(sortKey.ToString() + " not found in " + this._groupWithRecords.sortedColumns[0].Name);
                }
                   
                return this[index];
            }
        }

        /// <summary>
        /// Determines if a record exists in the collection that matches the sortkey.
        /// </summary>
        /// <param name="sortKey">The sort key to search for. The sortKey will be compared to the sorted field in the table as specified with <see cref="TableDescriptor.SortedColumns"/>.</param>
        /// <returns>True if the record exists; False otherwise.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(string sortKey)
        {
            return FindRecord(sortKey) != -1;
        }

        /// <summary>
        /// Searches for the first occurrence of a record that matches the sortkey and returns the zero-based index of the first occurrence found or -1 if not found.
        /// </summary>
        /// <param name="sortKey">The sort key to search for. The sortKey will be compared to the sorted field in the table as specified with <see cref="TableDescriptor.SortedColumns"/>.</param>
        /// <returns>Record index.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int IndexOf(string sortKey)
        {
            return FindRecord(sortKey);
        }

        /// <summary>
        /// Returns the next record in the collection.
        /// </summary>
        /// <param name="r">The Record.</param>
        /// <returns>Next record.</returns>
        public Record GetNext(Record r)
        {
            if (r.SortedEntry == null)
            {
                return null;
            }

            SortedRecordsTreeTableEntry next = (SortedRecordsTreeTableEntry)r.SortedEntry.Tree.GetNextEntry(r.SortedEntry);
            if (next != null)
            {
                return next.Element;
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
            if (r.SortedEntry == null)
            {
                return null;
            }

            SortedRecordsTreeTableEntry prev = (SortedRecordsTreeTableEntry)r.SortedEntry.Tree.GetPreviousEntry(r.SortedEntry);
            if (prev != null)
            {
                return prev.Element;
            }

            return null;
        }

        /// <summary>
        /// Gets the element at the zero-based index.
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
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SortedRecordsTreeTableEntry entry = _inner[index];
                if (entry == null)
                {
                    return null;
                }

                return entry.Element;
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
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

            if (_groupWithRecords == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);
            return value.ParentElement == _groupWithRecords;
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

            return value.SortedEntry.GetPosition(); ////InnerSortedRecordPosition;
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <returns>returns record index</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int CopyTo(Record[] array, int index)
        {
            int count = -1;
            int n = 0;
            foreach (Record record in this)
            {
                if (record == null)
                {
                    count = n;
                    break;
                }

                array[index + n] = record;
                n++;
            }

            return n;
        }

        ////        public RecordsInDetailsCollection SyncRoot
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
        /// <remarks>Enumerators only allow reading the of data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public RecordsInDetailsCollectionEnumerator GetEnumerator()
        {
            if (_groupWithRecords != null)
            {
                _groupWithRecords.EnsureInitialized(this);
            }

            return new RecordsInDetailsCollectionEnumerator(this);
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
        /// <param name="index">The list index</param>
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
        /// The method calls <see cref="Element.EnsureInitialized"/> of the <see cref="Group"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_groupWithRecords == null)
                {
                    return 0;
                }

                _groupWithRecords.EnsureInitialized(this);
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
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="RecordsInDetailsCollection"/>.
    /// </summary>
    public class RecordsInDetailsCollectionEnumerator : IEnumerator
    {
        Record _cursor, _next;
        RecordsInDetailsCollection _coll;

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
        public RecordsInDetailsCollectionEnumerator(RecordsInDetailsCollection collection)
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

            _next = (Record)ElementHelper.GetNextSibling(_next);

            return _cursor != null;
        }
        #endregion
    }

    /// <summary>
    /// A Read-only collection of <see cref="Record"/> elements that meet filter criteria and 
    /// are children of a <see cref="RecordsDetails"/> section. See <see cref="RecordFilterDescriptorCollection"/>
    /// or <see cref="TableDescriptor.RecordFilters"/> for filter criteria.
    /// An instance of this collection is returned by the <see cref="Group.FilteredRecords"/> property
    /// of a <see cref="Group"/> object.
    /// </summary>
    public class FilteredRecordsInDetailsCollection : IList
    {
        internal SortedRecordsTreeTable _inner;
        internal RecordsDetails _groupWithRecords;
        internal Record lastElement;
        internal int lastIndex;
        internal int lastCount;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static FilteredRecordsInDetailsCollection Empty = new FilteredRecordsInDetailsCollection(null);

        internal FilteredRecordsInDetailsCollection(RecordsDetails groupWithRecords)
        {
            if (groupWithRecords != null)
            {
                _inner = groupWithRecords.RecordTreeEntries;
            }

            _groupWithRecords = groupWithRecords;
        }

        /// <summary>
        /// Gets the element at the zero-based index.
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

                ////                if (lastIndex != -1 && lastCount == count)
                ////                {
                ////                    if (index == lastIndex)
                ////                    {
                ////                        return lastElement;
                ////                    }
                ////                    else if (index == lastIndex+1)
                ////                    {
                ////                        lastIndex++;
                ////                        lastElement = (Record) ElementHelper.GetNextSiblingElement(lastElement, CounterKind.FilteredRecordsCount);
                ////                        Debug.Assert(lastElement == ((SortedRecordsTreeTableEntry) _inner.GetEntryAtCounterPosition(Counter.CreateFilteredRecordCounter(index))).Element);
                ////                        return lastElement;
                ////                    }
                ////                }

                SortedRecordsTreeTableEntry entry = (SortedRecordsTreeTableEntry)_inner.GetEntryAtCounterPosition(CounterFactory.CreateFilteredRecordCounter(index));
                lastElement = entry.Element;
                lastCount = count;
                lastIndex = index;
                return lastElement;
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
            }
        }

        ICounterFactory CounterFactory
        {
            get
            {
                return this._groupWithRecords.Engine.CounterFactory;
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

            if (_groupWithRecords == null)
            {
                return false;
            }

            _groupWithRecords.EnsureInitialized(this, true);
            return value.GetFilteredRecordCount() > 0 && value.ParentElement == _groupWithRecords;
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

            return (int)value.SortedEntry.GetCounterPosition().GetValue(CounterKind.FilteredRecordsCount);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing. </param>
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
            foreach (Record record in this)
            {
                array[index + n] = record;
                n++;
            }
        }

        ////        public FilteredRecordsInDetailsCollection SyncRoot
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
        /// Enumerators cannot be used to modify the underlying collection.
        /// <para/>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public FilteredRecordsInDetailsCollectionEnumerator GetEnumerator()
        {
            if (_groupWithRecords != null)
            {
                _groupWithRecords.EnsureInitialized(this);
            }

            return new FilteredRecordsInDetailsCollectionEnumerator(this);
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
                throw new InvalidOperationException("Collection is Read-only");
            }
        }

        /// <summary>
        /// Not supported because collection is Read-only.
        /// </summary>
        /// <param name="index">The list index</param>
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
        /// The method calls <see cref="Element.EnsureInitialized"/> of the <see cref="Group"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_groupWithRecords == null)
                {
                    return 0;
                }

                _groupWithRecords.EnsureInitialized(this);
                return (int)_inner.GetCounter().GetValue(CounterKind.FilteredRecordsCount);
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
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="FilteredRecordsInDetailsCollection"/>.
    /// </summary>
    public class FilteredRecordsInDetailsCollectionEnumerator : IEnumerator
    {
        Record _cursor, _next;
        FilteredRecordsInDetailsCollection _coll;

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
        public FilteredRecordsInDetailsCollectionEnumerator(FilteredRecordsInDetailsCollection collection)
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

            _next = (Record)ElementHelper.GetNextSiblingElement(_next, CounterKind.FilteredRecordsCount);

            return _cursor != null;
        }
        #endregion
    }
}

