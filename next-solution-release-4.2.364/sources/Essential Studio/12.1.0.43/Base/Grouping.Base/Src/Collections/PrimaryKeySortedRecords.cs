//-------------------------------------------------------------------------------------------------
// <copyright file="PrimaryKeySortedRecords.cs" company="syncfusion">
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
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.ComponentModel;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A collection of primaryKeySorted <see cref="Record"/> elements that are children of a <see cref="Table"/> and
    /// represent the original records in the same order as the underlying data source. The collection
    /// provides support for determining a record's underlying position in the datasource using the <see cref="IndexOf"/>
    /// method.<para/>
    /// An instance of this collection is returned by the <see cref="Table.PrimaryKeySortedRecords"/> property
    /// of a <see cref="Table"/> object.
    /// </summary>
    public class PrimaryKeySortedRecordsCollection : IList, IDisposable
    {
        internal PrimaryKeySortedRecordsTree _inner;
        internal Table _table;

        internal PrimaryKeySortedRecordsCollection(PrimaryKeySortedRecordsTree inner, Table table)
        {
            _inner = inner;
            _table = table;
        }

        /// <summary>
        /// Disposes the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            _inner = null;
            _table = null;
        }

        /// <summary>
        /// Searches for the first occurrence of a record that matches the specified sortkey and returns the zero-based index of the first occurrence found or -1 if not found.
        /// </summary>
        /// <param name="sortKey">The sort key to search for. The object will be compared to the sorted fields in the table as specified with <see cref="TableDescriptor.SortedColumns"/>.</param>
        /// <returns>Record index.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int FindRecord(object sortKey)
        {
            if (Count > 0 && _table.TableDescriptor.PrimaryKeyColumns.Count > 0)
            {
                Type type = this._table.TableDescriptor.PrimaryKeyColumns[0].FieldDescriptor.GetPropertyType();
                object key = sortKey is DBNull ? sortKey : NullableHelper.ChangeType(sortKey, type);
                return _inner.TreeTable.IndexOfKey(new object[] { NullableHelper.FixDbNUllasNull(sortKey, type) });
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
        /// product21.ParentTable.TableDescriptor.PrimaryKeyColumns.Add("ProductName");
        /// product21.ParentTable.TableDescriptor.PrimaryKeyColumns.Add("SupplierID");
        /// int sp = product21.PrimaryKeySortedRecords.FindRecord("Spegesild", "21");
        /// product21.Records["Spegesild"].SetCurrent();
        /// </code>
        /// <code lang="VB">
        /// product21.ParentTable.TableDescriptor.PrimaryKeyColumns.Add("ProductName")
        /// product21.ParentTable.TableDescriptor.PrimaryKeyColumns.Add("SupplierID")
        /// Dim sp As Integer = product21.PrimaryKeySortedRecords.FindRecord("Spegesild", "21")
        /// product21.Records("Spegesild").SetCurrent()
        /// </code>
        /// </example>
        public int FindRecord(params object[] sortKeys)
        {
            if (sortKeys == null)
            {
                sortKeys = new object[] { null };
            }

            if (Count > 0 && _table.TableDescriptor.PrimaryKeyColumns.Count > 0)
            {
                object[] ax = sortKeys;
                int n = 0;
                try
                {
                    foreach (SortColumnDescriptor columnDescriptor in this._table.TableDescriptor.PrimaryKeyColumns)
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
                catch (NullReferenceException e)
                {
                    TraceUtil.TraceExceptionCatched(e);
                    throw;
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    return -1;
                }

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
                    throw new ArgumentException(sortKey.ToString() + " not found in " + this._table.TableDescriptor.PrimaryKeyColumns[0].Name);
                }

                return this[index];
            }
        }

        /// <summary>
        /// Determines if a record exists in the collection that matches the sortkey.
        /// </summary>
        /// <param name="sortKey">The sort key to search for. The sortKey will be compared to the sorted field in the table as specified with <see cref="TableDescriptor.SortedColumns"/>.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified sort key]; otherwise, <c>false</c>.
        /// </returns>
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
        /// <returns>returns first occurrence of a record.</returns>
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
        /// Gets the element at the zero-based index.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        /// <param name="index">The Index.</param>
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

                PrimaryKeySortedRecordsTreeEntry entry = _inner[index];
                if (entry == null)
                {
                    return null;
                }

                return entry.Record;
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
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

            _table.EnsureInitialized(this);
            return _inner.Contains(value.PrimaryKeySortedEntry);
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
            _table.EnsureInitialized(this);
            if (value == null)
            {
                return -1;
            }

            return _inner.IndexOf(value.PrimaryKeySortedEntry);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing. </param>
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

        ////        public PrimaryKeySortedRecordsCollection SyncRoot
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
        public PrimaryKeySortedRecordsCollectionEnumerator GetEnumerator()
        {
            _table.EnsureInitialized(this);
            return new PrimaryKeySortedRecordsCollectionEnumerator(this);
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
                throw new InvalidOperationException("Collection is read only");
            }
        }

        /// <summary>
        /// Not supported because collection is Read-only.
        /// </summary>
        /// <param name="index">The index in the list</param>
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

        /// <summary>
        /// Returns the next record in the collection.
        /// </summary>
        /// <param name="r">The Record.</param>
        /// <returns>Next record.</returns>
        public Record GetNext(Record r)
        {
            if (r.PrimaryKeySortedEntry == null)
            {
                return null;
            }

            PrimaryKeySortedRecordsTreeEntry next = (PrimaryKeySortedRecordsTreeEntry)r.PrimaryKeySortedEntry.Tree.GetNextEntry(r.PrimaryKeySortedEntry);
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
            if (r.PrimaryKeySortedEntry == null)
            {
                return null;
            }

            PrimaryKeySortedRecordsTreeEntry prev = (PrimaryKeySortedRecordsTreeEntry)r.PrimaryKeySortedEntry.Tree.GetPreviousEntry(r.PrimaryKeySortedEntry);
            if (prev != null)
            {
                return prev.Record;
            }

            return null;
        }

        /// <summary>
        /// Fixes the primary key sort position of the record after the value was changed.
        /// </summary>
        /// <param name="r">The Record.</param>
        public void FixPrimaryKeyPosition(Record r)
        {
            if (r.PrimaryKeySortedEntry == null)
            {
                return;
            }

            ITreeTable tree = r.PrimaryKeySortedEntry.Tree;

            // Remove it from the tree
            tree.Remove(r.PrimaryKeySortedEntry);
            // Insert it back in at the right sort position.
            tree.Add(r.PrimaryKeySortedEntry);
        }

        void EnsurePrimaryKeySortedEntry(Record record)
        {
            if (record.PrimaryKeySortedEntry == null)
            {
                PrimaryKeySortedRecordsTreeEntry pkEntry = new PrimaryKeySortedRecordsTreeEntry();
                record.PrimaryKeySortedEntry = pkEntry;
                pkEntry.Record = record;
                pkEntry.Tree = this._inner.TreeTable;
            }
        }

        /// <summary>
        /// Adds the given record into the collection.
        /// </summary>
        /// <param name="record">Record to add.</param>
        /// <returns>returns Index.</returns>
        public int Add(Record record)
        {
            EnsurePrimaryKeySortedEntry(record);
            int index = this._inner.Add(record.PrimaryKeySortedEntry);
            return index;
        }

        /// <summary>
        /// Removes the record from primary keys tree.
        /// </summary>
        /// <param name="r">Record to remove.</param>
        public void Remove(Record r)
        {
            if (r.PrimaryKeySortedEntry == null)
            {
                return;
            }

            ITreeTable tree = r.PrimaryKeySortedEntry.Tree;

            // Remove it from the tree
            tree.Remove(r.PrimaryKeySortedEntry);
        }
        ////public void Add(Record r)
        ////{
        ////}
    }

    /// <summary>
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="PrimaryKeySortedRecordsCollection"/>.
    /// </summary>
    public class PrimaryKeySortedRecordsCollectionEnumerator : IEnumerator
    {
        PrimaryKeySortedRecordsTreeEnumerator _inner;

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
        public PrimaryKeySortedRecordsCollectionEnumerator(PrimaryKeySortedRecordsCollection collection)
        {
            _inner = new PrimaryKeySortedRecordsTreeEnumerator(collection._inner);
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