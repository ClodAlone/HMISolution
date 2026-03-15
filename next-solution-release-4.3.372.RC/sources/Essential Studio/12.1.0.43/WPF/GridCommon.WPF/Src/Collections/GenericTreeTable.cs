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
using System.Collections.Specialized;

namespace Syncfusion.Windows.Collections.Generic
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


    public interface ITreeTableEntryHost
    {
        ITreeTableEntry GetTreeTableEntry(int kind);
        void SetTreeTableEntry(int kind, ITreeTableEntry entry);
    }

    /// <summary>
    /// A leaf in the tree with value and optional sort key.
    /// </summary>
    public class GenericTreeTableEntry<V> : TreeTableNode, ITreeTableEntry
    {
        V thisValue;

        public GenericTreeTableEntry()
        {
        }

        public GenericTreeTableEntry(TreeTable tree, V value)
        {
            thisValue = value;
            Tree = tree;
        }

        public GenericTreeTableEntry(GenericTreeTable<V> tree, V value)
        {
            thisValue = value;
            Tree = tree.InternalTree;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (thisValue is IDisposable)
                    ((IDisposable)thisValue).Dispose();
                thisValue = default(V);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the value attached to this leaf.
        /// </summary>
        public virtual V Value
        {
            get
            {
                return thisValue;
            }
            set
            {
                thisValue = value;
            }
        }

        /// <summary>
        /// Returns the sort key of this leaf.
        /// </summary>
        public virtual V GetSortKey()
        {
            return Value;
        }

        /// <summary>
        /// Returns the minimum value (of the most-left leaf) of the branch in a sorted tree.
        /// </summary>
        /// <returns></returns>
        public override object GetMinimum()
        {
            return GetSortKey();
        }

        /// <summary>
        /// Returns the number of child nodes (+1 for the current node).
        /// </summary>
        /// <returns></returns>
        public override int GetCount()
        {
            return 1;
        }

        /// <summary>
        /// Creates a branch that can hold this entry when new leaves are inserted into the tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public virtual ITreeTableBranch CreateBranch(TreeTable tree)
        {
            return new TreeTableBranch(tree);
        }

        /// <summary>
        /// Returns the Debug / text information about the node.
        /// </summary>
        /// <returns></returns>
        public override string GetNodeInfo()
        {
            return base.GetNodeInfo() + ", " + (Value != null ? Value.ToString() : "null");
        }

        /// <summary>
        /// Indicates whether this is a leaf.
        /// </summary>
        /// <returns></returns>
        public override bool IsEntry()
        {
            return true;
        }

        #region Explicit ITreeTableEntry Members

        object ITreeTableEntry.GetSortKey()
        {
            return GetSortKey();
        }

        object ITreeTableEntry.Value
        {
            get
            {
                return this.Value;
            }
            set
            {
                Value = (V)value;
            }
        }

        #endregion

    }

    /// <summary>
    /// A tree table.
    /// </summary>
    public class GenericTreeTable<V> : ITreeTable, IList<GenericTreeTableEntry<V>>, ISupportInitialize
    {
        TreeTable thisTree;

        #region Properties


        public object Tag
        {
            get { return thisTree.Tag; }
            set { thisTree.Tag = value; }
        }

        //public GenericBinaryTreeCollection<V> BinaryTreeCollection { get; set; }
        public int Identifier { get; set; }

        /// <summary>
        /// The non-generic tree table with actual implementation.
        /// </summary>
        public TreeTable InternalTree
        {
            get { return thisTree; }
        }

        #endregion

        #region Ctor, Dispose

        /// <summary>
        /// Initializes a new <see cref="GenericTreeTable{V}"/>.
        /// </summary>
        /// <param name="sorted"></param>
        public GenericTreeTable(bool sorted)
        {
            thisTree = new TreeTable(sorted);
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
        public GenericTreeTableEntry<V> GetNextEntry(GenericTreeTableEntry<V> current)
        {
            return (GenericTreeTableEntry<V>)thisTree.GetNextEntry(current);
        }

        ITreeTableEntry ITreeTable.GetNextEntry(ITreeTableEntry current)
        {
            return GetNextEntry((GenericTreeTableEntry<V>)current);
        }

        /// <summary>
        /// Optimized access to the previous entry.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public GenericTreeTableEntry<V> GetPreviousEntry(GenericTreeTableEntry<V> current)
        {
            return (GenericTreeTableEntry<V>)thisTree.GetPreviousEntry(current);
        }

        ITreeTableEntry ITreeTable.GetPreviousEntry(ITreeTableEntry current)
        {
            return GetPreviousEntry((GenericTreeTableEntry<V>)current);
        }

        #endregion

        #region Search Key, AddIfNotExists

        public GenericTreeTableEntry<V> AddIfNotExists(object key, GenericTreeTableEntry<V> entry)
        {
            return (GenericTreeTableEntry<V>)thisTree.AddIfNotExists(key, entry);
        }

        public int IndexOfKey(object key)
        {
            return thisTree.IndexOfKey(key);
        }

        public GenericTreeTableEntry<V> FindKey(object key)
        {
            return (GenericTreeTableEntry<V>)thisTree.FindKey(key);
        }

        public GenericTreeTableEntry<V> FindHighestSmallerOrEqualKey(object key)
        {
            return (GenericTreeTableEntry<V>)thisTree.FindHighestSmallerOrEqualKey(key);
        }

        #endregion

        #region IList<GenericTreeTableEntry<V>>

        /// <summary>
        /// Gets or sets the item at the zero-based index.
        /// </summary>
        public GenericTreeTableEntry<V> this[int index]
        {
            get
            {
                return (GenericTreeTableEntry<V>)thisTree[index];
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
        public void Insert(int index, GenericTreeTableEntry<V> item)
        {
            thisTree.Insert(index, item);
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove from the collection. If the value is NULL or the item is not contained
        /// in the collection, the method will do nothing.</param>
        public bool Remove(GenericTreeTableEntry<V> item)
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
        public int IndexOf(GenericTreeTableEntry<V> item)
        {
            return thisTree.IndexOf(item);
        }


        #endregion

        #region ICollection<GenericTreeTableEntry<V>> Members

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
        public int Add(GenericTreeTableEntry<V> item)
        {
            return thisTree.Add(item);
        }

        void ICollection<GenericTreeTableEntry<V>>.Add(GenericTreeTableEntry<V> item)
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
        public bool Contains(GenericTreeTableEntry<V> item)
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
        public void CopyTo(GenericTreeTableEntry<V>[] array, int index)
        {
            thisTree.CopyTo((ITreeTableNode[])array, index);
        }

        #endregion

        #region IEnumerable<GenericTreeTableEntry<V>> Members

        public IEnumerator<GenericTreeTableEntry<V>> GetEnumerator()
        {
            return new GenericTreeTableEnumerator<V>(thisTree);
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
                this[index] = (GenericTreeTableEntry<V>)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GenericTreeTableEntry<V>)value);
        }

        void IList.Remove(object value)
        {
            Remove((GenericTreeTableEntry<V>)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GenericTreeTableEntry<V>)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GenericTreeTableEntry<V>)value);
        }

        int IList.Add(object value)
        {
            return Add((GenericTreeTableEntry<V>)value);
        }

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the collection has a fixed size; otherwise, false.
        /// </returns>
        bool IList.IsFixedSize
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
            CopyTo((GenericTreeTableEntry<V>[])array, index);
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

    public class GenericTreeTableEnumerator<V> : TreeTableEnumerator, IEnumerator<GenericTreeTableEntry<V>>
    {
        public GenericTreeTableEnumerator(ITreeTable tree)
            : base(tree)
        {
        }

        public new GenericTreeTableEntry<V> Current
        {
            get
            {
                return (GenericTreeTableEntry<V>)base.Current;
            }
        }

    }

    // Preparing for a "VirtualUnsortedRecordsCollection" implementation in grouping
    // where yet another wrapper can then switch between VirtualUnsortedRecordsCollection
    // or GenericBinaryTreeCollection for UnsortedRecords.
    public interface IGenericBinaryTreeCollection<V>
    {
        int Add(V item);
        void BeginInit();
        void Clear();
        bool Contains(V item);
        void CopyTo(V[] array, int arrayIndex);
        int Count { get; }
        //V CreateItem();
        void Dispose();
        void EndInit();
        IEnumerator<V> GetEnumerator();
        V GetNext(V current);
        V GetPrevious(V current);
        int IndexOf(V item);
        void Insert(int index, V item);
        bool IsInitializing { get; }
        bool IsReadOnly { get; }
        bool Remove(V item);
        void RemoveAt(int index);
        V this[int index] { get; set; }

        V AddIfNotExists(object key, V item);
        int IndexOfKey(object key);
        V FindKey(object key);
        V FindHighestSmallerOrEqualKey(object key);
    }

    public class GenericBinaryTreeCollection<V> : IList<V>, IList, IDisposable, ISupportInitialize, IGenericBinaryTreeCollection<V>, INotifyCollectionChanged
        where V : ITreeTableEntryHost
    {
        private GenericTreeTable<V> thisGenericTree;

        #region Properties

        public GenericTreeTable<V> InternalTable
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

        public GenericBinaryTreeCollection(GenericTreeTable<V> genericTree)
        {
            this.thisGenericTree = genericTree;
            this.thisGenericTree.Tag = this;
        }

        public GenericBinaryTreeCollection(bool sorted)
        {
            this.thisGenericTree = new GenericTreeTable<V>(sorted);
            this.thisGenericTree.Tag = this;
        }

        public virtual void Dispose()
        {
            this.thisGenericTree = null;
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
            if (!this.IsInitializing) RaiseEnsureInitialized("Add");
            var entry = this.LookupOrCreateEntry(item);
            var insertAt = this.thisGenericTree.Add(entry);
            if (!this.IsInSuspend)
            {
                var notifyChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, entry.Value, insertAt);
                this.RaiseCollectionChanged(notifyChangedArgs);
            }
            return insertAt;
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
            //int n = 0;
            for (int i = arrayIndex; i < array.Length; i++)
            {
                array[arrayIndex + i] = this[i];
            }
            //foreach (V record in this)
            //{
            //    array[arrayIndex + n] = record;
            //    n++;
            //}
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
            var removeAt = this.thisGenericTree.Remove(entry);

            if (!this.IsInSuspend)
            {
                var notifyChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, entry.Value, -1);
                this.RaiseCollectionChanged(notifyChangedArgs);
            }

            return removeAt;
        }

        #endregion

        #region IEnumerable<V> Members

        public IEnumerator<V> GetEnumerator()
        {
            return new GenericBinaryTreeCollectionEnumerator<V>(this);
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
            return Add((V)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((V)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((V)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (V)value);
        }

        void IList.Remove(object value)
        {
            Remove((V)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (V)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((V[])array, index);
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

        private GenericTreeTableEntry<V> GetTreeTableEntry(V item)
        {
            return (GenericTreeTableEntry<V>)item.GetTreeTableEntry(Identifier);
        }

        protected GenericTreeTableEntry<V> LookupOrCreateEntry(V item)
        {
            var entry = (GenericTreeTableEntry<V>)item.GetTreeTableEntry(Identifier);
            if (entry == null || entry.Tree == null || entry.Tree != this.thisGenericTree.InternalTree)
            {
                entry = new GenericTreeTableEntry<V>(thisGenericTree, item);
                item.SetTreeTableEntry(Identifier, entry);
            }
            return entry;
        }

        //public V CreateItem()
        //{
        //    var item = new V();
        //    var entry = new GenericTreeTableEntry<V>(thisGenericTree, item);
        //    item.SetTreeTableEntry(Identifier, entry);
        //    return item;
        //}

        #endregion


        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool IsInSuspend
        {
            get;
            private set;
        }

        public void SuspendRaiseEvents()
        {
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
            }
        }

        public void ResumeRaiseEvents()
        {
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
            }
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (!this.IsInSuspend && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for items of a <see cref="GenericBinaryTreeCollection{V}"/>.
    /// </summary>
    public class GenericBinaryTreeCollectionEnumerator<V> : IEnumerator<V>
          where V : ITreeTableEntryHost
    {
        GenericTreeTableEnumerator<V> treeEnumerator;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GenericBinaryTreeCollectionEnumerator(GenericBinaryTreeCollection<V> collection)
        {
            treeEnumerator = new GenericTreeTableEnumerator<V>(collection.InternalTable);
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
                return (V)Current;
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

        #region IDisposable Members

        public void Dispose()
        {
            treeEnumerator.Dispose();
        }

        #endregion
    }


    // Summary:
    //     Provides data for the System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    //     event.
    public class GenericTreeTableEnsureInitializedEventArgs : EventArgs
    {
        string thisMemberName;

        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyChangedEventArgs
        //     class.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property that changed.
        public GenericTreeTableEnsureInitializedEventArgs(string memberName)
        {
            thisMemberName = memberName;
        }


        // Summary:
        //     Gets the name of the property that changed.
        //
        // Returns:
        //     The name of the property that changed.
        public virtual string MemberName
        {
            get { return thisMemberName; }
        }
    }


}
