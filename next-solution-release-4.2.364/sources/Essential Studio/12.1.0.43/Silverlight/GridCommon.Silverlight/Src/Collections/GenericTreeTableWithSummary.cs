#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#if !WinRT

namespace Syncfusion.Windows.Collections.Generic
#else

namespace Syncfusion.WinRT.Collections.Generic
#endif
{

    /* Sample:
    public class Record : ITreeTableEntryHost
    {
        #region IGenericTreeTableValue Members

        ITreeTableEntry thisEntry;

        ITreeTableEntry ITreeTableEntryHost.GetTreeTableEntry(int kind)
        {
            return thisEntry;
        }

        void ITreeTableEntryHost.SetTreeTableEntry(int kind, ITreeTableEntry entry)
        {
            thisEntry = entry;
        }

        #endregion
    }
     */


    /// <summary>
    /// A tree leaf with value and summary information.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GenericTreeTableWithSummaryEntry<V> : TreeTableWithSummaryEntry
    {
        public GenericTreeTableWithSummaryEntry()
        {
        }

        public GenericTreeTableWithSummaryEntry(TreeTable tree, V value)
        {
            base.Value = value;
            Tree = tree;
        }

        public GenericTreeTableWithSummaryEntry(GenericTreeTableWithSummary<V> tree, V value)
        {
            base.Value = value;
            Tree = tree.InternalTree;
        }

        /// <summary>
        /// Gets or sets the value attached to this leaf.
        /// </summary>
        public new V Value
        {
            get
            {
                return (V) base.Value;
            }
            set
            {
                base.Value = value;
            }
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GenericTreeTableWithSummary<V> : ITreeTable, IList<GenericTreeTableWithSummaryEntry<V>>, ISupportInitialize
    {
        TreeTableWithSummary thisTree;

        #region Properties

        public object Tag
        {
            get { return thisTree.Tag; }
            set { thisTree.Tag = value; }
        }
        
        //public GenericBinaryTreeWithSummaryCollection<V> BinaryTreeCollection { get; set; }
        public int Identifier { get; set; }

        /// <summary>
        /// The internal thisTree table.
        /// </summary>
        public TreeTableWithSummary InternalTree
        {
            get { return thisTree; }
        }

        #endregion

        #region Ctor, Dispose
        /// <summary>
        /// Initializes a new <see cref="GenericTreeTableWithSummary{V}"/>.
        /// </summary>
        /// <param name="sorted"></param>
        public GenericTreeTableWithSummary(bool sorted)
        {
            thisTree = new TreeTableWithSummary(sorted);
            thisTree.Tag = this;
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            thisTree.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Root, Sort
        /// <summary>
        /// Gets the root node.
        /// </summary>
        public ITreeTableNode Root
        {
            get { return thisTree.Root; }
        }

        /// <summary>
        /// Indicates whether thisTree is sorted.
        /// </summary>
        public bool Sorted
        {
            get { return thisTree.Sorted; }
        }

        /// <summary>
        /// Gets or sets the comparer used by sorted trees.
        /// </summary>
        public IComparer Comparer
        {
            get
            {
                return thisTree.Comparer;
            }
            set
            {
                thisTree.Comparer = value;
            }
        }

        #endregion

        #region ISupportInitialize
        /// <summary>
        /// Indicates whether BeginInit was called.
        /// </summary>
        public bool IsInitializing
        {
            get { return thisTree.IsInitializing; }
        }

        /// <summary>
        /// Optimizes insertion of many items when thisTree is initialized for the first time.
        /// </summary>
        public void BeginInit()
        {
            thisTree.BeginInit();
        }

        /// <summary>
        /// Ends optimization of insertion of items when thisTree is initialized for the first time.
        /// </summary>
        public void EndInit()
        {
            thisTree.EndInit();
        }

        #endregion

        #region GetNext, GetPrevious

        /// <summary>
        /// Optimized access to a subsequent entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public GenericTreeTableWithSummaryEntry<V> GetNextEntry(GenericTreeTableWithSummaryEntry<V> current)
        {
            return (GenericTreeTableWithSummaryEntry<V>) thisTree.GetNextEntry(current);
        }

        ITreeTableEntry ITreeTable.GetNextEntry(ITreeTableEntry current)
        {
            return GetNextEntry((GenericTreeTableWithSummaryEntry<V>) current);
        }

        /// <summary>
        /// Optimized access to the previous entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public GenericTreeTableWithSummaryEntry<V> GetPreviousEntry(GenericTreeTableWithSummaryEntry<V> current)
        {
            return (GenericTreeTableWithSummaryEntry<V>) thisTree.GetPreviousEntry(current);
        }

        ITreeTableEntry ITreeTable.GetPreviousEntry(ITreeTableEntry current)
        {
            return GetPreviousEntry((GenericTreeTableWithSummaryEntry<V>) current);
        }

        #endregion

        #region Search Key, AddIfNotExists

        public GenericTreeTableWithSummaryEntry<V> AddIfNotExists(object key, GenericTreeTableWithSummaryEntry<V> entry)
        {
            return (GenericTreeTableWithSummaryEntry<V>) thisTree.AddIfNotExists(key, entry);
        }

        public int IndexOfKey(object key)
        {
            return thisTree.IndexOfKey(key);
        }

        public GenericTreeTableWithSummaryEntry<V> FindKey(object key)
        {
            return (GenericTreeTableWithSummaryEntry<V>) thisTree.FindKey(key);
        }

        public GenericTreeTableWithSummaryEntry<V> FindHighestSmallerOrEqualKey(object key)
        {
            return (GenericTreeTableWithSummaryEntry<V>) thisTree.FindHighestSmallerOrEqualKey(key);
        }

        #endregion

        #region IList<GenericTreeTableWithSummaryEntry<V>> Members

        /// <summary>
        /// Gets or sets the item at the zero-based index.
        /// </summary>
        public GenericTreeTableWithSummaryEntry<V> this[int index]
        {
            get
            {
                return (GenericTreeTableWithSummaryEntry<V>) thisTree[index];
            }
            set
            {
                thisTree[index] = value;
            }
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted.</param>
        /// <param name="item">The item to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GenericTreeTableWithSummaryEntry<V> item)
        {
            thisTree.Insert(index, item);
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove from the collection. If the value is NULL or the item is not contained
        /// in the collection, the method will do nothing.</param>
        public bool Remove(GenericTreeTableWithSummaryEntry<V> item)
        {
            return thisTree.Remove(item);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove. </param>
        public void RemoveAt(int index)
        {
            thisTree.RemoveAt(index);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the item in the collection.
        /// </summary>
        /// <param name="value">The item to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the item within the entire collection, if found; otherwise -1.</returns>
        public int IndexOf(GenericTreeTableWithSummaryEntry<V> item)
        {
            return thisTree.IndexOf(item);
        }

        #endregion

        #region ICollection<GenericTreeTableWithSummaryEntry<V>> Members

        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return thisTree.GetCount();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the collection is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return thisTree.IsReadOnly;
            }
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The item to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GenericTreeTableWithSummaryEntry<V> item)
        {
            return thisTree.Add(item);
        }

        void ICollection<GenericTreeTableWithSummaryEntry<V>>.Add(GenericTreeTableWithSummaryEntry<V> item)
        {
            Add(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            thisTree.Clear();
        }

        /// <summary>
        /// Determines if the item belongs to this collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise False.</returns>
        public bool Contains(GenericTreeTableWithSummaryEntry<V> item)
        {
            if (item == null)
                return false;

            return thisTree.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the items copied from the  ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GenericTreeTableWithSummaryEntry<V>[] array, int index)
        {
            thisTree.CopyTo((ITreeTableNode[]) array, index);
        }

        #endregion

        #region IEnumerable<GenericTreeTableWithSummaryEntry<V,C>> Members

        public IEnumerator<GenericTreeTableWithSummaryEntry<V>> GetEnumerator()
        {
            return new GenericTreeTableWithSummaryEnumerator<V>(thisTree);
        }

        #endregion

        #region IList Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (GenericTreeTableWithSummaryEntry<V>) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GenericTreeTableWithSummaryEntry<V>) value);
        }

        void IList.Remove(object value)
        {
            Remove((GenericTreeTableWithSummaryEntry<V>) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GenericTreeTableWithSummaryEntry<V>) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GenericTreeTableWithSummaryEntry<V>) value);
        }

        int IList.Add(object value)
        {
            return Add((GenericTreeTableWithSummaryEntry<V>) value);
        }

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the collection has a fixed size; otherwise, false.
        /// </returns>
        public bool IsFixedSize
        {
            get
            {
                return thisTree.IsFixedSize;
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

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GenericTreeTableWithSummaryEntry<V>[]) array, index);
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

        #region Summaries

        /// <summary>
        /// Marks all summaries dirty.
        /// </summary>
        /// <param name="notifySummariesSource">if set to <c>true</c> notify summaries source.</param>
        public void InvalidateSummariesTopDown(bool notifySummariesSource)
        {
            thisTree.InvalidateSummariesTopDown(notifySummariesSource);
        }

        /// <summary>
        /// Indicates whether the tree has summaries.
        /// </summary>
        public bool HasSummaries
        {
            get
            {
                return thisTree.HasSummaries;
            }
        }

        /// <summary>
        /// Returns an array of summary objects.
        /// </summary>
        public ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
        {
            return thisTree.GetSummaries(emptySummaries);
        }

        #endregion
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GenericTreeTableWithSummaryEnumerator<V> : TreeTableEnumerator, IEnumerator<GenericTreeTableWithSummaryEntry<V>>
    {
        public GenericTreeTableWithSummaryEnumerator(ITreeTable tree)
            : base(tree)
        {
        }

        public new GenericTreeTableWithSummaryEntry<V> Current
        {
            get
            {
                return (GenericTreeTableWithSummaryEntry<V>) base.Current;
            }
        }
    }


    // Preparing for a "VirtualSortedRecordsCollection" implementation in grouping
    // where yet another wrapper can then switch between VirtualSortedRecordsCollection
    // or GenericBinaryTreeWithSummaryCollection for SortedRecords inside TopLevelGroup.

    public interface IGenericBinaryTreeWithSummaryCollection<V> : IGenericBinaryTreeCollection<V>
    {
        ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries);
        bool HasSummaries { get; }
        void InvalidateSummariesTopDown(bool notifySummariesSource);
    }

    /// <summary>
    /// A collection of items maintained in a binary tree
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GenericBinaryTreeWithSummaryCollection<V> : IList<V>, IList, IDisposable, ISupportInitialize, IGenericBinaryTreeWithSummaryCollection<V>
        where V : ITreeTableEntryHost
    {
        private GenericTreeTableWithSummary<V> thisGenericTree;

        #region Properties

        public GenericTreeTableWithSummary<V> InternalTable
        {
            get { return thisGenericTree; }
        }

        public object Tag
        {
            get { return thisGenericTree.Tag; }
            set { thisGenericTree.Tag = value; }
        }

        public int Identifier
        {
            get { return thisGenericTree.Identifier; }
            set { thisGenericTree.Identifier = value; }
        }

        #endregion

        #region Ctor, Dispose

        public GenericBinaryTreeWithSummaryCollection(GenericTreeTableWithSummary<V> genericTree)
        {
            thisGenericTree = genericTree;
            //thisGenericTree.BinaryTreeCollection = this;
            thisGenericTree.Tag = this;
        }

        public GenericBinaryTreeWithSummaryCollection(bool sorted)
        {
            thisGenericTree = new GenericTreeTableWithSummary<V>(sorted);
            //thisGenericTree.BinaryTreeCollection = this;
            thisGenericTree.Tag = this;
        }

        /// <summary>
        /// Disposes of the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            thisGenericTree = null;
        }

        #endregion

        #region GetPrevious, GetNext

        /// <summary>
        /// Returns the next item in the collection.
        /// </summary>
        /// <returns></returns>
        public V GetNext(V current)
        {
            var entry = GetTreeTableEntry(current);
            if (entry != null)
            {
                var next = thisGenericTree.GetNextEntry(entry);
                if (next != null)
                    return next.Value;
            }

            return default(V);
        }

        /// <summary>
        /// Returns the previous item in the collection.
        /// </summary>
        /// <returns></returns>
        public V GetPrevious(V current)
        {
            var entry = GetTreeTableEntry(current);
            if (entry != null)
            {
                var prev = thisGenericTree.GetPreviousEntry(entry);
                if (prev != null)
                    return prev.Value;
            }

            return default(V);
        }

        #endregion

        #region Search Key, AddIfNotExists

        public V AddIfNotExists(object key, V item)
        {
            // Previously created a value V with CreateItem.
            // Set sort keys in V.
            // Now call AddIfNotExists.
            //
            // If exists then method will discard "item" and return found value.
            // If it does not exist then method will add item to collection and return "item"
            RaiseEnsureInitialized("AddIfNotExists");
            var entry = GetTreeTableEntry(item);
            entry = thisGenericTree.AddIfNotExists(key, entry);
            return entry.Value;
        }

        public int IndexOfKey(object key)
        {
            RaiseEnsureInitialized("IndexOfKey");
            return thisGenericTree.IndexOfKey(key);
        }

        public V FindKey(object key)
        {
            RaiseEnsureInitialized("FindKey");
            var entry = thisGenericTree.FindKey(key);
            if (entry != null)
                return entry.Value;
            return default(V);
        }

        public V FindHighestSmallerOrEqualKey(object key)
        {
            RaiseEnsureInitialized("FindHighestSmallerOrEqualKey");
            var entry = thisGenericTree.FindHighestSmallerOrEqualKey(key);
            if (entry != null)
                return entry.Value;
            return default(V);
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
            thisGenericTree.BeginInit();
        }

        public void EndInit()
        {
            thisGenericTree.EndInit();
        }

        /// <summary>
        /// Indicates whether BeginInit was called.
        /// </summary>
        public bool IsInitializing
        {
            get { return thisGenericTree.IsInitializing; }
        }

        #endregion

        #region EnsureInitialized Event

        public void RaiseEnsureInitialized(string member)
        {
            if (EnsureInitialized != null)
                EnsureInitialized(this, new GenericTreeTableEnsureInitializedEventArgs(member));
        }

        public event EventHandler<GenericTreeTableEnsureInitializedEventArgs> EnsureInitialized;

        #endregion

        #region IList<V> Members

        public int IndexOf(V item)
        {
            RaiseEnsureInitialized("IndexOf");
            var entry = GetTreeTableEntry(item);
            return this.thisGenericTree.IndexOf(entry);
        }

        public void Insert(int index, V item)
        {
            RaiseEnsureInitialized("Insert");
            var entry = this.LookupOrCreateEntry(item);
            this.thisGenericTree.Insert(index, entry);
        }

        public void RemoveAt(int index)
        {
            RaiseEnsureInitialized("RemoveAt");
            this.Remove(this[index]);
        }

        public V this[int index]
        {
            get
            {
                RaiseEnsureInitialized("getItem");
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var entry = this.thisGenericTree[index];
                if (entry == null)
                    return default(V);
                return entry.Value;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("V");
                }

                RaiseEnsureInitialized("setItem");
                var entry = this.LookupOrCreateEntry(value);
                this.thisGenericTree[index] = entry;
            }
        }

        #endregion

        #region ICollection<V> Members

        public int Add(V item)
        {
            RaiseEnsureInitialized("Add");
            var entry = this.LookupOrCreateEntry(item);
            return this.thisGenericTree.Add(entry);
        }

        void ICollection<V>.Add(V item)
        {
            Add(item);
        }

        public void Clear()
        {
            RaiseEnsureInitialized("Clear");
            this.thisGenericTree.Clear();
        }

        public bool Contains(V item)
        {
            if (item == null)
            {
                return false;
            }

            RaiseEnsureInitialized("Contains");
            var entry = GetTreeTableEntry(item);
            return this.thisGenericTree.Contains(entry);
        }

        public void CopyTo(V[] array, int arrayIndex)
        {
            int n = 0;
            foreach (V record in this)
            {
                array[arrayIndex + n] = record;
                n++;
            }
        }

        public int Count
        {
            get
            {
                RaiseEnsureInitialized("Count");
                return this.thisGenericTree.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return this.thisGenericTree.IsReadOnly; }
        }

        public bool Remove(V item)
        {
            RaiseEnsureInitialized("Remove");
            var entry = GetTreeTableEntry(item);
            return this.thisGenericTree.Remove(entry);
        }

        #endregion

        #region IEnumerable<V> Members

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public IEnumerator<V> GetEnumerator()
        {
            return new GenericBinaryTreeWithSummaryCollectionEnumerator<V>(this);
        }


        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList Members

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        int IList.Add(object value)
        {
            return Add((V) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((V) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((V) value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (V) value);
        }

        void IList.Remove(object value)
        {
            Remove((V) value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (V) value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((V[]) array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region CreateItem, TreeTableEntry

        private GenericTreeTableWithSummaryEntry<V> GetTreeTableEntry(V item)
        {
            return (GenericTreeTableWithSummaryEntry<V>) item.GetTreeTableEntry(Identifier);
        }

        GenericTreeTableWithSummaryEntry<V> LookupOrCreateEntry(V item)
        {
            var entry = GetTreeTableEntry(item);
            if (entry == null || entry.Tree == null || entry.Tree != thisGenericTree.InternalTree)
            {
                entry = new GenericTreeTableWithSummaryEntry<V>(thisGenericTree, item);
                item.SetTreeTableEntry(thisGenericTree.Identifier, entry);
            }
            return entry;
        }

        //public V CreateItem()
        //{
        //    var item = new V();
        //    var entry = new GenericTreeTableWithSummaryEntry<V>(thisGenericTree, item);
        //    item.SetTreeTableEntry(Identifier, entry);
        //    return item;
        //}

        #endregion

        #region Summaries

        /// <summary>
        /// Marks all summaries dirty.
        /// </summary>
        /// <param name="notifySummariesSource">if set to <c>true</c> notify summaries source.</param>
        public void InvalidateSummariesTopDown(bool notifySummariesSource)
        {
            thisGenericTree.InvalidateSummariesTopDown(notifySummariesSource);
        }

        /// <summary>
        /// Indicates whether the tree has summaries.
        /// </summary>
        public bool HasSummaries
        {
            get
            {
                return thisGenericTree.HasSummaries;
            }
        }

        /// <summary>
        /// Returns an array of summary objects.
        /// </summary>
        public ITreeTableSummary[] GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries)
        {
            return thisGenericTree.GetSummaries(emptySummaries);
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for items of a <see cref="GenericBinaryTreeWithSummaryCollection{V}"/>.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GenericBinaryTreeWithSummaryCollectionEnumerator<V> : IEnumerator<V>
        where V : ITreeTableEntryHost
    {
        GenericTreeTableWithSummaryEnumerator<V> treeEnumerator;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GenericBinaryTreeWithSummaryCollectionEnumerator(GenericBinaryTreeWithSummaryCollection<V> collection)
        {
            treeEnumerator = new GenericTreeTableWithSummaryEnumerator<V>(collection.InternalTable);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first item in the collection.
        /// </summary>
        public virtual void Reset()
        {
            treeEnumerator.Reset();
        }

        object IEnumerator.Current
        {
            get
            {
                return (V) Current;
            }
        }

        /// <summary>
        /// Gets the current item in the collection.
        /// </summary>
        public V Current
        {
            get
            {
                return treeEnumerator.Current.Value;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next item of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next item; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            return treeEnumerator.MoveNext();
        }

        public void Dispose()
        {
            treeEnumerator.Dispose();
        }


    }
}