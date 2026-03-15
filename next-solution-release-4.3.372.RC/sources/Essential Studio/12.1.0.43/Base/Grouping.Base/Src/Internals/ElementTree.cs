//-------------------------------------------------------------------------------------------------
// <copyright file="ElementTree.cs" company="syncfusion">
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

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;

using Syncfusion.Grouping;
using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

namespace Syncfusion.Grouping.Internals
{
    internal interface IElementTreeTableSource
    {
        ElementTreeTable GetChildElementTreeTable(bool displayOrder);
    }

    /// <summary>
    /// An entry within a tree buffer for an <see cref="Element"/> object.
    /// </summary>
    internal class ElementTreeTableEntry : TreeTableWithCounterEntry
    {
        public Element Element
        {
            get
            {
                return (Element)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// By default, elements do not get sorted.
        /// </summary>
        /// <returns>returns null</returns>
        public override object GetSortKey()
        {
            return null;
        }

        /// <summary>
        /// The tree this entry belongs to.
        /// </summary>
        public ElementTreeTable ElementTreeTable
        {
            get
            {
                TreeTable tree = this.Tree;
                if (tree != null)
                {
                    return tree.Tag as ElementTreeTable;
                }

                return null;
            }
        }

        /// <override/>
        public override void InvalidateCounterBottomUp(bool notifyParentRecordSource)
        {
            base.InvalidateCounterBottomUp(notifyParentRecordSource);
            if (Element != null)
            {
                Element.InvalidateCounter();
                Element.OnElementTreeInvalidateCounterBottomUp();
            }
        }
    }

    internal class ElementTreeTable : NonFinalizeDisposable, ITreeTableCounterSource, ITreeTable, ITreeTableSummaryArraySource
    {
        internal TreeTableWithCounter inner;
        Element _owner;
#if CACHE
        Hashtable counterPositionCache = null;
        int shadowVersion = 0;
        static bool allowCache = true;

        int cacheCount
        {
            get
            {
                return inner.ReservedShort;
            }
            set
            {
                inner.ReservedShort = value;
            }
        }
#endif
        public virtual bool WithoutCounter
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public virtual bool VirtualMode
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public virtual bool VirtualModeLockEntries
        {
            get
            {
                return false;
            }
        }

        public virtual int VirtualCount
        {
            get
            {
                return 0;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        bool allowSetItem
        {
            get
            {
                return inner.Reserved1;
            }

            set
            {
                inner.Reserved1 = value;
            }
        }
        
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
                _owner = null;
#if CACHE
                counterPositionCache = null;
                shadowVersion = 0;
#endif
            }

            base.Dispose(disposing);
        }

        public bool AllowSetItem
        {
            get
            {
                return allowSetItem;
            }

            set
            {
                allowSetItem = value;
            }
        }

        public ElementTreeTable(Element owner)
        {
            inner = new TreeTableWithCounter(owner.Engine.CounterFactory.Empty, false);
            inner.Tag = this;
            _owner = owner;
        }

        public ICounterFactory CounterFactory
        {
            get
            {
                return this._owner.Engine.CounterFactory;
            }
        }

        protected TreeTableWithCounter Inner
        {
            get { return inner; }
        }

        public TreeTable TreeTable
        {
            get
            {
                return inner;
            }
        }

#if TREECHANGED
        public event ElementsTreeChangeEventHandler TreeChanged;
#endif

        void ClearCache()
        {
#if CACHE
            this.counterPositionCache = null;
#endif
        }

#if TREECHANGED
        /// <summary>
        /// Raises the <see cref="TreeChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ElementsTreeChangeEventArgs" /> that contains the event data.</param>
        void OnTreeChanged(ElementsTreeChangeEventArgs e)
        {
            if (TreeChanged != null)
                TreeChanged(this, e);
        }
#endif

        #region ITreeTableCounterSource Private Members

        ITreeTableCounter ITreeTableCounterSource.GetCounter()
        {
            return GetCounter();
        }

        #endregion

        #region Strong Typed ITreeTableCounterSource Members

        public ITreeTableCounter GetCounter()
        {
            if (WithoutCounter)
            {
                return CounterFactory.CreateCounter(Count);
            }

            return inner.GetCounterTotal();
        }

        public void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            if (WithoutCounter)
            {
                return;
            }

            inner.InvalidateCounterTopDown(notifyCounterSource);
        }

        public void InvalidateCounterBottomUp()
        {
            ////if (!WithoutCounter)
            ////    inner.InvalidateCounterTopDown(false);
            if (this._owner != null)
            {
                ElementTreeTableEntry e = this._owner.GetElementEntry();
                if (e != null)
                {
                    e.InvalidateCounterBottomUp(true);
                }
                else
                {
                    this._owner.InvalidateCounter();
                }
            }
        }

        #endregion

        #region Strong Typed Collection Members

        public virtual ElementTreeTableEntry GetInnerItem(int index)
        {
            return (ElementTreeTableEntry)Inner[index];
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public ElementTreeTableEntry this[int index]
        {
            get
            {
                _owner.EnsureInitialized(this, false);
                return GetInnerItem(index);
            }

            set
            {
                if (!this.allowSetItem)
                {
                    throw new NotSupportedException("Only Add is allowed.");
                }

                ElementTreeTableEntry oldValue = this[index];
                if (!Object.ReferenceEquals(oldValue, value))
                {
                    value.Tree = Inner;
                    if (VirtualMode)
                    {
                        OnVirtualModeSetItem(index, value);
                    }
                    else
                    {
                        Inner[index] = value;
                    }
#if TREECHANGED
                    if (value != null)
                        OnTreeChanged(new ElementsTreeChangeEventArgs(CollectionChangeAction.Add, value));
                    if (oldValue != null)
                        OnTreeChanged(new ElementsTreeChangeEventArgs(CollectionChangeAction.Remove, oldValue));
#endif
                    ClearCache();
                }
            }
        }

        protected virtual void OnVirtualModeSetItem(int index, ElementTreeTableEntry value)
        {
        }

        protected virtual void OnVirtualModeInsert(int index, ElementTreeTableEntry value)
        {
        }

        protected virtual void OnVirtualModeRemoveAt(int index)
        {
        }

        protected virtual void OnVirtualModeClear()
        {
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, ElementTreeTableEntry value)
        {
            if (VirtualMode)
            {
                this.OnVirtualModeInsert(index, value);
                return;
            }

            if (!this.allowSetItem)
            {
                throw new NotSupportedException("Only Add is allowed");
            }

            Inner.Insert(index, value);
            value.Tree = Inner;
            ClearCache();
#if TREECHANGED
            OnTreeChanged(new ElementsTreeChangeEventArgs(CollectionChangeAction.Add, value));
#endif
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(ElementTreeTableEntry value)
        {
            if (VirtualMode)
            {
                this.OnVirtualModeRemoveAt(value.GetPosition());
                return;
            }

            Inner.Remove(value);
            ClearCache();
#if TREECHANGED
            OnTreeChanged(new ElementsTreeChangeEventArgs(CollectionChangeAction.Remove, value));
#endif
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise False.</returns>
        public bool Contains(ElementTreeTableEntry value)
        {
            if (value == null)
            {
                return false;
            }

            if (VirtualMode)
            {
                return value.Tree == this.inner;
            }

            return Inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise -1.</returns>
        public int IndexOf(ElementTreeTableEntry value)
        {
            if (VirtualMode && value != null)
            {
                return value.GetPosition();
            }

            return Inner.IndexOf(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(ElementTreeTableEntry value)
        {
            try
            {
                if (VirtualMode)
                {
                    return 0;
                }

                return Inner.Add(value);
            }
            finally
            {
                value.Tree = Inner;
                ClearCache();
#if TREECHANGED
                OnTreeChanged(new ElementsTreeChangeEventArgs(CollectionChangeAction.Add, value));
#endif
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(ElementTreeTableEntry[] array, int index)
        {
            Inner.CopyTo((ITreeTableNode[])array, index);
        }

        public ElementTreeTable SyncRoot
        {
            get
            {
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (VirtualMode)
            {
                return new VirtualElementTreeTableEnumerator(this);
            }

            return new ElementTreeTableEnumerator(Inner);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            if (VirtualMode)
            {
                this.OnVirtualModeRemoveAt(index);
                return;
            }

            Remove(this[index]);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (VirtualMode)
            {
                this.OnVirtualModeClear();
                return;
            }

            Inner.Clear();
            ClearCache();
#if TREECHANGED
            OnTreeChanged(new ElementsTreeChangeEventArgs(CollectionChangeAction.Refresh, null));
#endif
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
                this[index] = (ElementTreeTableEntry)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (ElementTreeTableEntry)value);
        }

        void IList.Remove(object value)
        {
            Remove((ElementTreeTableEntry)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((ElementTreeTableEntry)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((ElementTreeTableEntry)value);
        }

        int IList.Add(object value)
        {
            return Add((ElementTreeTableEntry)value);
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
                if (VirtualMode)
                {
                    return VirtualCount;
                }

                return Inner.GetCount();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((ElementTreeTableEntry[])array, index);
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

        #region Strong Typed ICounterElementTreeTable Members

        //// TODO: caching

        public ITreeTableCounter GetStartCounterPosition()
        {
            return inner.GetStartCounterPosition();
        }

        public ITreeTableCounter GetCounterTotal()
        {
            if (WithoutCounter)
            {
                int c = Count;
                return CounterFactory.CreateCounter(
                    c * GetCounterKindFactor(CounterKind.DisplayElementCount),
                    c * (double)GetCounterKindFactor(CounterKind.YAmountCount),
                    c,
                    c * GetCounterKindFactor(CounterKind.ElementsCount),
                    c,
                    0,
                    0);
            }

            return inner.GetCounterTotal();
        }

        internal ElementTreeTableEntry GetEntryAtVisibleIndex(int index)
        {
            if (WithoutCounter)
            {
                return this[Math.Min(Count - 1, index / GetCounterKindFactor(CounterKind.DisplayElementCount))];
            }

            return GetEntryAtCounterPosition(CounterFactory.CreateDisplayElementCounter(index), CounterKind.DisplayElementCount, false);
        }

        internal ElementTreeTableEntry GetNextVisibleEntry(ElementTreeTableEntry current)
        {
            if (WithoutCounter)
            {
                return this.GetNextEntry(current);
            }

            return GetNextCounterEntry(current, TreeTableCounterCookies.CountVisible);
        }

        public ElementTreeTableEntry GetNextEntry(ITreeTableEntry current)
        {
            _owner.EnsureInitialized(this, false);
            return (ElementTreeTableEntry)inner.GetNextEntry((ElementTreeTableEntry)current);
        }

        public ElementTreeTableEntry GetPreviousEntry(ITreeTableEntry current)
        {
            _owner.EnsureInitialized(this, false);
            return (ElementTreeTableEntry)inner.GetPreviousEntry((ElementTreeTableEntry)current);
        }

        public ElementTreeTableEntry GetNextCounterEntry(ElementTreeTableEntry current, int counterKind)
        {
            if (WithoutCounter)
            {
                return GetNextEntry(current);
            }

            _owner.EnsureInitialized(this, false);
            return (ElementTreeTableEntry)inner.GetNextNotEmptyCounterEntry((ElementTreeTableEntry)current, counterKind);
        }

        public ElementTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition)
        {
            ////_owner.EnsureInitialized(this, false);
            return GetEntryAtCounterPosition(searchPosition, false);
        }

        public ElementTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition, bool leftMost)
        {
            ////_owner.EnsureInitialized(this, false);
            return GetEntryAtCounterPosition(searchPosition, searchPosition.Kind, leftMost);
        }

        public virtual int GetCounterKindFactor(int counterKind)
        {
            return 1;
        }

        public ITreeTableCounter GetCounterPositionOf(ElementTreeTableEntry entry, int counterKind)
        {
            _owner.EnsureInitialized(this, false);
            if (WithoutCounter)
            {
                return CounterFactory.CreateCounter(this.IndexOf(entry) * GetCounterKindFactor(counterKind));
            }

            ITreeTableCounter position;
#if CACHE
            ElementTreeCacheEntry cacheEntry = null;
            if (allowCache)
            {
                cacheEntry = GetCacheEntry(counterKind);
                if (cacheEntry != null && cacheEntry.result != null && Object.ReferenceEquals(cacheEntry.result, entry))
                    return cacheEntry.searchPosition;

                position = entry.GetCounterPosition();

                if (position != null)
                    SaveCacheEntry(entry, position, counterKind, true, false, cacheEntry);
            }
            else
#endif
            position = entry.GetCounterPosition();

            return position;
        }

#if CACHE
        ElementTreeCacheEntry GetCacheEntry(int counterKind)
        {
            if (counterPositionCache != null)
            {
#if WEAKREF
                WeakReference reference = (WeakReference) counterPositionCache[counterKind];
                if (reference != null)
                    return (ElementTreeCacheEntry) reference.Target;
#else
                return (ElementTreeCacheEntry) counterPositionCache[counterKind];
#endif
            }
            return null;
        }

        void SaveCacheEntry(ElementTreeTableEntry result, Counter searchPosition, int counterKind, bool leftMost, bool forceSearchPosition, ElementTreeCacheEntry cacheEntry)
        {
            bool isNew = false;
            if (cacheEntry == null)
            {
                isNew = true;
                cacheEntry = new ElementTreeCacheEntry(this);
                this.cacheCount++;
            }

            cacheEntry.result = result;
            if (result != null)
            {
                cacheEntry.nextDelta = cacheEntry.result.GetCounterTotal().GetValue(counterKind);
                if (cacheEntry.nextDelta == 1 || forceSearchPosition)
                    cacheEntry.searchPosition = searchPosition;
                else
                    cacheEntry.searchPosition = cacheEntry.result.GetCounterPosition();
                cacheEntry.leftMost = leftMost;
            }

            if (isNew)
            {
                if (counterPositionCache == null)
                {
                    this.shadowVersion = this._owner.Engine.version;
                    counterPositionCache = new Hashtable();
                }
#if WEAKREF
                counterPositionCache[counterKind] = new WeakReference(cacheEntry);
#else
                counterPositionCache[counterKind] = cacheEntry;
#endif
            }
        }


        void SyncCache()
        {
            Engine engine = GetEngine();
            if (engine != null && engine.version != shadowVersion)
            {
                counterPositionCache = null;
            }
        }
#endif
        Engine GetEngine()
        {
            if (_owner != null)
            {
                return _owner.Engine;
            }

            return null;
        }

        public ElementTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition, int counterKind, bool leftMost)
        {
            _owner.EnsureInitialized(this, false);

            ElementTreeTableEntry result;
#if CACHE
            if (allowCache)
            {
                SyncCache();
                ElementTreeCacheEntry cacheEntry = GetCacheEntry(counterKind);
                if (cacheEntry != null && cacheEntry.result != null)
                {
                    if (cacheEntry.leftMost == leftMost)
                    {
                        double delta = searchPosition.Compare(cacheEntry.searchPosition, counterKind);
                        if (delta == 0)
                        {
                            //                        Console.WriteLine("Same");
                            return cacheEntry.result;
                        }
                        else if (delta > 0 && delta < cacheEntry.nextDelta)
                        {
                            //                        Console.WriteLine("SameInDelta");
                            return cacheEntry.result;
                        }
                        else if (delta == cacheEntry.nextDelta)
                        {
                            //                        Console.WriteLine("NetxDelta");
                            result = this.GetNextCounterEntry(cacheEntry.result, counterKind);
                            if (result != null)
                            {
                                SaveCacheEntry(result, searchPosition, counterKind, leftMost, true, cacheEntry);
                                return result;
                            }
                        }
                    }
                }
                result = (ElementTreeTableEntry) Inner.GetEntryAtCounterPosition(searchPosition, counterKind, leftMost);
                SaveCacheEntry(result, searchPosition, counterKind, leftMost, false, cacheEntry);
            }
            else
#endif
            if (WithoutCounter)
            {
                result = this.GetInnerItem(Math.Min((int)searchPosition.GetValue(counterKind) / GetCounterKindFactor(counterKind), Count - 1));
            }
            else
            {
                if (searchPosition.Compare(GetCounterTotal(), counterKind) > 0)
                {
                    return null;
                }
                result = (ElementTreeTableEntry)Inner.GetEntryAtCounterPosition(searchPosition, counterKind, leftMost);
            }

            return result;
        }

        #endregion

        #region Strong Typed ITreeTable Members

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
            get { return inner.Sorted; }
        }

        public IComparer Comparer
        {
            get
            {
                return inner.Comparer;
            }

            set
            {
                inner.Comparer = value;
            }
        }

        public void BeginInit()
        {
            inner.BeginInit();
        }

        public void EndInit()
        {
            inner.EndInit();
        }

        ElementTreeTableEntry GetNextEntry(ElementTreeTableEntry current)
        {
            if (VirtualMode)
            {
                if (IndexOf(current) + 1 < this.Count)
                {
                    return GetInnerItem(IndexOf(current) + 1);
                }

                return null;
            }

            return (ElementTreeTableEntry)inner.GetNextEntry(current);
        }

        #endregion
        
        #region ITreeTable Members

        ITreeTableEntry ITreeTable.GetNextEntry(ITreeTableEntry current)
        {
            return GetNextEntry((ElementTreeTableEntry)current);
        }

        ITreeTableEntry ITreeTable.GetPreviousEntry(ITreeTableEntry current)
        {
            return GetPreviousEntry((ElementTreeTableEntry)current);
        }

        #endregion

        #region ISummaryArraySource Members

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
                ElementTreeTableEntry e = this._owner.GetSummaryElementEntry();
                if (e != null)
                {
                    e.InvalidateSummariesBottomUp(true);
                }
            }
        }

        #endregion
        
        public int VisibleCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return Count * GetCounterKindFactor(CounterKind.DisplayElementCount);
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : (int)counter.GetVisibleCount();
            }
        }

        public int FilteredRecordCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return Count;
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : (int)counter.FilteredRecordCount;
            }
        }

        public int RecordCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return Count;
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : (int)counter.RecordCount;
            }
        }
        
        public int ElementCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return Count * GetCounterKindFactor(CounterKind.ElementsCount);
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : (int)counter.ElementCount;
            }
        }

        public double YAmountCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return (double)Count * (double)GetCounterKindFactor(CounterKind.YAmountCount);
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : counter.YAmountCount;
            }
        }

        public double VisibleCustomCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return Count;
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : (int)counter.VisibleCustomCount;
            }
        }
        
        public double CustomCount
        {
            get
            {
                if (inner.Root == null)
                {
                    return 0;
                }

                if (WithoutCounter)
                {
                    return Count;
                }

                FilteredRecordCounter counter = (FilteredRecordCounter)inner.GetCounterTotal();
                return counter == null ? 0 : (int)counter.CustomCount;
            }
        }
    }

    /// <summary>
    /// Strongly typed enumerator for the ElementTreeTable.
    /// </summary>
    internal class ElementTreeTableEnumerator : TreeTableEnumerator
    {
        public ElementTreeTableEnumerator(TreeTable tree)
            : base(tree)
        {
        }

        public new ElementTreeTableEntry Current
        {
            get
            {
                return (ElementTreeTableEntry)base.Current;
            }
        }
    }

#if CACHE
    internal class ElementTreeCacheEntry
    {
        public ElementTreeCacheEntry(ElementTreeTable owner)
        {
            this.owner = owner;
        }

#if WEAKREF
            ~ElementTreeCacheEntry()
            {
                if (--this.owner.cacheCount == 0)
                    this.owner.counterPositionCache = null;
            }
#endif

        ElementTreeTable owner;
        public Counter searchPosition;
        public ElementTreeTableEntry result;
        public double nextDelta;
        public bool leftMost;
    }
#endif

    /// <summary>
    /// Enumerator class for <see cref="ElementTreeTableEntry"/> elements of a <see cref="ElementTreeTable"/>.
    /// </summary>
    internal class VirtualElementTreeTableEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        ElementTreeTable _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public VirtualElementTreeTableEnumerator(ElementTreeTable collection)
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
        public ElementTreeTableEntry Current
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
            if (_next == -1 || _next >= _coll.Count)
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
