//-------------------------------------------------------------------------------------------------
// <copyright file="UnsortedRecordsTree.cs" company="syncfusion">
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
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

namespace Syncfusion.Grouping.Internals
{
    internal class UnsortedRecordsTreeEntry
        ////UFD: TreeTableWithSummaryEntry
        : TreeTableEntry
    {
        public Record Record
        {
            get
            {
                return (Record)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// Returns the sort key of this leaf.
        /// </summary>
        /// <returns>returns null</returns>
        /// <override/>
        public override object GetSortKey()
        {
            return null; ////Record.GetData();
        }

        public UnsortedRecordsTree UnsortedRecordsTree
        {
            get
            {
                TreeTable tree = this.Tree;
                if (tree != null)
                {
                    return tree.Tag as UnsortedRecordsTree;
                }

                return null;
            }
        }

        public Table Table
        {
            get
            {
                UnsortedRecordsTree table = this.UnsortedRecordsTree;
                if (table != null)
                {
                    return table.Table as Table;
                }

                return null;
            }
        }

        /*UFD:public override ITreeTableSummary[] OnGetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
        {
            return this.Table.TableDescriptor.UnfilteredSummaries.CreateSummaries(Record);
        }*/
    }

    internal class UnsortedRecordsTree : ITreeTable ////UFD:, ITreeTableSummaryArraySource
    {
        ////UFD:TreeTableWithSummary inner = new TreeTableWithSummary(false);
        TreeTable inner = new TreeTable(false);
        Table _owner = null;

        public UnsortedRecordsTree(Table owner)
        {
            _owner = owner;
            inner.Tag = this;
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            inner.Dispose();
            inner = null;
            _owner = null;
            GC.SuppressFinalize(this);
        }

        ////UFD:public TreeTableWithSummary TreeTable
        public TreeTable TreeTable
        {
            get
            {
                return inner;
            }
        }

        public Table Table
        {
            get
            {
                return _owner;
            }
        }

        public ITreeTableNode Root
        {
            get { return inner.Root; }
        }

        public bool IsInitializing
        {
            get { return inner.IsInitializing; }
        }

        public bool Sorted
        {
            get { return false; }
        }

        public void BeginInit()
        {
            inner.BeginInit();
        }

        public void EndInit()
        {
            inner.EndInit();
        }

        public IComparer Comparer
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotSupportedException(string.Empty);
            }
        }

        public UnsortedRecordsTreeEntry GetNextEntry(UnsortedRecordsTreeEntry current)
        {
            return (UnsortedRecordsTreeEntry)inner.GetNextEntry(current);
        }

        ITreeTableEntry ITreeTable.GetNextEntry(ITreeTableEntry current)
        {
            return GetNextEntry((UnsortedRecordsTreeEntry)current);
        }

        public UnsortedRecordsTreeEntry GetPreviousEntry(UnsortedRecordsTreeEntry current)
        {
            return (UnsortedRecordsTreeEntry)inner.GetPreviousEntry(current);
        }

        ITreeTableEntry ITreeTable.GetPreviousEntry(ITreeTableEntry current)
        {
            return GetPreviousEntry((UnsortedRecordsTreeEntry)current);
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public UnsortedRecordsTreeEntry this[int index]
        {
            get
            {
                return (UnsortedRecordsTreeEntry)inner[index];
            }

            set
            {
                inner[index] = value;
            }
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, UnsortedRecordsTreeEntry value)
        {
            if (!this._owner.VirtualMode)
            {
                inner.Insert(index, value);
            }
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(UnsortedRecordsTreeEntry value)
        {
            if (!this._owner.VirtualMode)
            {
                inner.Remove(value);
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise False.</returns>
        public bool Contains(UnsortedRecordsTreeEntry value)
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
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise -1.</returns>
        public int IndexOf(UnsortedRecordsTreeEntry value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(UnsortedRecordsTreeEntry value)
        {
            if (this._owner.VirtualMode)
            {
                return -1;
            }

            return inner.Add(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the  ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(UnsortedRecordsTreeEntry[] array, int index)
        {
            inner.CopyTo((ITreeTableNode[])array, index);
        }

        public UnsortedRecordsTree SyncRoot
        {
            get
            {
                return null;
            }
        }

        public UnsortedRecordsTreeEnumerator GetEnumerator()
        {
            return new UnsortedRecordsTreeEnumerator(inner);
        }

        #region ISummaryArraySource Members
        /*UFD:
                public ISummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries, out bool summaryChanged)
                {
                    summaryChanged = !inner.HasSummaries;
                    return inner.GetSummaries(emptySummaries);
                }

                public void InvalidateSummariesTopDown()
                {
                    inner.InvalidateSummariesTopDown(true);
                }

                public void InvalidateSummariesTopDown(bool notifySource)
                {
                    inner.InvalidateSummariesTopDown(notifySource);
                }

                public void InvalidateSummary()
                {
                    if (this._owner != null)
                    {
                        this._owner.InvalidateSummary();
                    }
                }

                public void InvalidateSummariesBottomUp()
                {
                    if (this._owner != null)
                    {
                        ElementTreeTableEntry e = this._owner.GetElementEntry();
                        if (e != null)
                            e.InvalidateSummariesBottomUp(true);
                    }
                }
        */
        #endregion

        #region IList Members

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

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (UnsortedRecordsTreeEntry)value;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            inner.RemoveAt(index);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (UnsortedRecordsTreeEntry)value);
        }

        void IList.Remove(object value)
        {
            Remove((UnsortedRecordsTreeEntry)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((UnsortedRecordsTreeEntry)value);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            inner.Clear();
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((UnsortedRecordsTreeEntry)value);
        }

        int IList.Add(object value)
        {
            return Add((UnsortedRecordsTreeEntry)value);
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
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return inner.GetCount();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((UnsortedRecordsTreeEntry[])array, index);
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

    internal class UnsortedRecordsTreeEnumerator : TreeTableEnumerator
    {
        public UnsortedRecordsTreeEnumerator(ITreeTable tree)
            : base(tree)
        {
        }

        public new UnsortedRecordsTreeEntry Current
        {
            get
            {
                return (UnsortedRecordsTreeEntry)base.Current;
            }
        }
    }
}

