//-------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElements.cs" company="syncfusion">
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
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A Read-only collection of <see cref="Element"/> objects in a <see cref="Table"/> which represent a flattened
    /// view of grouped and hierarchical records that are visible and meet filter criteria in the datasource.
    /// An instance of this collection is
    /// returned by the <see cref="Table.DisplayElements"/> property of a <see cref="Table"/> object.
    /// <para/>
    /// This collection does not step into child elements of a <see cref="NestedTable"/>.
    /// </summary>
    public class DisplayElementsInTableCollection : RuntimeElementsInTableCollection
    {
        internal DisplayElementsInTableCollection(Table table)
            : base(table)
        {
            _table = table;
            _childTable = null;
        }

        internal DisplayElementsInTableCollection(Table table, bool stepInNestedTables)
            : base(table, stepInNestedTables)
        {
            _table = table;
            _childTable = null;
            _stepInNestedTables = stepInNestedTables;
        }

        internal DisplayElementsInTableCollection(ChildTable childTable)
            : base(childTable)
        {
            _table = childTable.ParentTable;
            _childTable = childTable;
        }
    }

    /// <summary>
    /// A Read-only collection <see cref="Element"/> objects in a <see cref="Table"/> which represent a flattened
    /// view of grouped and hierarchical records. All records are returned no matter if they meet filter criteria or not.
    /// An instance of this collection is
    /// returned by the <see cref="Table.Elements"/> property of a <see cref="Table"/> object.
    /// <para/>
    /// This collection does not step into child elements of a <see cref="NestedTable"/>.
    /// </summary>
    public class ElementsInTableCollection : RuntimeElementsInTableCollection
    {
        internal ElementsInTableCollection(Table table)
            : base(table)
        {
            _table = table;
            _childTable = null;
            counterKind = CounterKind.ElementsCount;
        }

        internal ElementsInTableCollection(Table table, bool stepInNestedTables)
            : base(table, stepInNestedTables)
        {
            _table = table;
            _childTable = null;
            _stepInNestedTables = stepInNestedTables;
            counterKind = CounterKind.ElementsCount;
        }

        internal ElementsInTableCollection(ChildTable childTable)
            : base(childTable)
        {
            _table = childTable.ParentTable;
            _childTable = childTable;
            counterKind = CounterKind.ElementsCount;
        }
    }
    
    /// <summary>
    /// This is a Read-only collection base class for <see cref="DisplayElementsInTableCollection"/> and <see cref="ElementsInTableCollection"/>.
    /// </summary>
    public class RuntimeElementsInTableCollection : IList, IDisposable
    {
        internal Table _table;
        internal int cachedAdjustedTableIndex;
        internal int version;
        int cachedElementCount;
        internal ChildTable _childTable;
        internal bool _stepInNestedTables;
        internal ChildTable cachedFilteredChildTable;
        internal int cachedFilteredChildTablePos;
        internal int counterKind = CounterKind.DisplayElementCount;
        internal static bool trace = false;
        static internal bool allowCache = true;
        const int cparam1 = 5;
        const int cparam2 = 8;
        Hashtable countCache = new Hashtable();
        bool isDisposed = false;

        /// <summary>
        /// Returns true when collection has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }
        
        #region IDisposable Members

        /// <summary>
        /// Disposes of the object and clears the cache.
        /// </summary>
        public void Dispose()
        {
            cachedElement = null;
            for (int n = 0; n < cacheList.Count; n++)
            {
                IDisposable obj = cacheList[n] as IDisposable;
                if (obj != null)
                {
                    obj.Dispose();
                }
            }

            cacheList.Clear();
            cacheTree.Dispose();
            cacheElementIdTree.Dispose();
            for (int n = 0; n < yAmountCacheList.Count; n++)
            {
                IDisposable obj = yAmountCacheList[n] as IDisposable;
                if (obj != null)
                {
                    obj.Dispose();
                }
            }

            yAmountCacheList.Clear();
            yAmountCacheTree.Dispose();
            yAmountCacheElementIdTree.Dispose();
            cachedFilteredChildTable = null;
            countCache.Clear();

            _table = null;
            this.isDisposed = true;
        }

        /// <summary>
        /// Clears the cache of recently accessed elements.
        /// </summary>
        public void ClearCache()
        {
            cachedElement = null;
            cacheList.Clear();
            cacheTree.Clear();
            cacheElementIdTree.Clear();
            yAmountCacheList.Clear();
            yAmountCacheTree.Clear();
            yAmountCacheElementIdTree.Clear();
            countCache.Clear();
        }

        #endregion

        static RuntimeElementsInTableCollection()
        {
#if TRACE
            trace = Switches.RuntimeElementsInTableCollection.TraceVerbose;
#endif
        }

#if WEAKREF
        internal WeakReference _cachedElement;
        internal Element cachedElement
        {
            get
            {
                if (_cachedElement != null)
                    return (Element) _cachedElement.Target;
                return null;
            }
            set
            {
                _cachedElement = new WeakReference(value);
            }
        }
#else
        internal Element cachedElement;
        internal ArrayList cacheList = new ArrayList(cacheSpan);
        internal int cacheListPointer = 0;
        const int cacheSpan = 1024;
        TreeTable cacheTree = new TreeTable(true);
        TreeTable cacheElementIdTree = new TreeTable(true);

        internal ArrayList yAmountCacheList = new ArrayList(cacheSpan);
        TreeTable yAmountCacheTree = new TreeTable(true);
        TreeTable yAmountCacheElementIdTree = new TreeTable(true);
        internal int yAmountCacheListPointer = 0;

#endif
        internal RuntimeElementsInTableCollection(Table table)
        {
            _table = table;
            _childTable = null;
        }

        internal RuntimeElementsInTableCollection(Table table, bool stepInNestedTables)
        {
            _table = table;
            _childTable = null;
            _stepInNestedTables = stepInNestedTables;
        }

        internal RuntimeElementsInTableCollection(ChildTable childTable)
        {
            _table = childTable.ParentTable;
            _childTable = childTable;
        }

        internal DetailsSection GetRootDetails()
        {
            return _table.TopLevelGroup.Details;
        }

#if true || DIAGNOSING
        static int countItemCacheHit;
        static int countItemNonCacheHit;
#endif

        int cachedFilteredChildTableVersion = -1;

        int GetFilterChildTablePosition(ChildTable childTable)
        {
            if (cachedFilteredChildTable != null && version == _table.EngineVersion
                && cachedFilteredChildTableVersion == _table.EngineVersion
                && cachedFilteredChildTable == childTable)
            {
                return this.cachedFilteredChildTablePos;
            }

            cachedFilteredChildTable = childTable;
            cachedFilteredChildTablePos = (int)ElementHelper.GetCumulatedPosition(childTable, counterKind);
            cachedFilteredChildTableVersion = _table.EngineVersion;
            return cachedFilteredChildTablePos;
        }

        /// <summary>
        /// Gets (and caches) the element at the zero-based index.
        /// Setting is not supported and will throw an exception since the collection is readonly.
        /// </summary>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Element this[int index]
        {
            get
            {
                int count = Count;
                if (index < 0 || index >= count)
                {
                    return null;
                }
                ////    throw new ArgumentOutOfRangeException();

                int adjustedTableIndex = index;
                int filterChildTablePosition = 0;

                ChildTable childTable = this.GetFilterChildTable();
                if (childTable != null)
                {
                    filterChildTablePosition = this.GetFilterChildTablePosition(childTable);
                    adjustedTableIndex += filterChildTablePosition;
                }

                if (trace)
                {
                    Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(index, adjustedTableIndex);
                }

                bool cacheIt = allowCache; ////&& _table.RelatedTables.Count > 0;
                //// && cachedElement != null && cachedAdjustedTableIndex > 0)
                if (cacheIt && version == _table.EngineVersion) 
                {
                    if (trace)
                    {
                        Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(cachedAdjustedTableIndex, cachedElement, cachedElementCount, _table.FilteredChildTable);
                    }
                    
                    bool found = false;

#if LOOPING
                    for (int n = 0; n < cacheList.Count; n++)
                    {
                        CacheState cs = this.cacheList[n] as CacheState;
                        if (cs != null)
                        {
                            cachedAdjustedTableIndex = this.GetCachedIndexOf(cs);
                            cachedElement = cs.cachedElement;
                            cachedElementCount = cs.cachedElementCount;

                            if (adjustedTableIndex == cachedAdjustedTableIndex)
                            {
                                if (trace)
                                    Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("Same", index, adjustedTableIndex);
                                //return cachedElement;
                                found = true;
                                break;
                            }
                            else
                            {
                                // Is this a child element of a nested table? (see discussion below)
                                if (adjustedTableIndex > cachedAdjustedTableIndex && adjustedTableIndex < cachedAdjustedTableIndex+cachedElementCount)
                                {
                                    if (trace)
                                        Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("Same nested", index, adjustedTableIndex);
                                    //return cachedElement;
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        for (int n = 0; n < cacheList.Count; n++)
                        {
                            CacheState cs = this.cacheList[n] as CacheState;
                            if (cs != null)
                            {
                                cachedAdjustedTableIndex = this.GetCachedIndexOf(cs);
                                cachedElement = cs.cachedElement;
                                cachedElementCount = cs.cachedElementCount;


                                // Is this the subsequent element?

                                if (/*_table.FilteredChildTable == null && */adjustedTableIndex == cachedAdjustedTableIndex+cachedElementCount)
                                {
                                    // TODO: caching gets messed up when _table.FilteredChildTable is not null. GetNextElementStepIn
                                    // will step out of table, e.g. if filter is set on products table, GetNextElementStepIn might
                                    // setep into categories parent table.

                                    cachedAdjustedTableIndex += cachedElementCount;
                                    Element elx = cachedElement;
                                    cachedElement = ElementHelper.GetNextElementStepIn(cachedElement, counterKind, null, this._stepInNestedTables, this._table);

                                    if (this._stepInNestedTables)
                                    {
                                        cachedElementCount = 1;
                                        //if (cachedElement != null && cachedElement.GetVisibleCount() != 1)
                                        //    Debugger.Break();
                                    }
                                    else
                                    {
                                        // No need to call cachedElement.GetDisplayPosition();
                                        // cachedAdjustedTableIndex is already pointing to the first display element of the nested table.
                                        if (cachedElement != null)
                                            cachedElementCount = (int) cachedElement.GetCounter().GetValue(counterKind);
                                        //Debug.Assert(cachedElement.ParentTable == _table);
                                    }

                                    this.CacheElement(cachedElement, cachedAdjustedTableIndex, cachedElementCount);
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

#else
                    CacheState cs = null;

                    // Check recent added caches.
                    int to = Math.Min(this.cacheListPointer + 1, cacheList.Count);
                    for (int n = Math.Max(this.cacheListPointer - 4, 0); n < to; n++)
                    {
                        CacheState es = this.cacheList[n] as CacheState;
                        if (es != null
                            && adjustedTableIndex >= es.cachedAdjustedTableIndex
                            && adjustedTableIndex < es.cachedAdjustedTableIndex + es.cachedElementCount
                            && es.cachedAdjustedTableIndex >= 0)
                        {
                            cs = es;
                            ////lastcacheindex = Math.Max(n-cparam1, 0);//Math.Min(n+1, cacheList.Count);
                            break;
                        }
                    }

                    // Check recent hits.
                    if (cs == null)
                    {
                        to = Math.Min(lastcacheindex + cparam2, cacheList.Count);
                        for (int n = lastcacheindex; n < to; n++)
                        {
                            CacheState es = this.cacheList[n] as CacheState;
                            if (es != null
                                && adjustedTableIndex >= es.cachedAdjustedTableIndex
                                && adjustedTableIndex < es.cachedAdjustedTableIndex + es.cachedElementCount
                                && es.cachedAdjustedTableIndex >= 0)
                            {
                                cs = es;
                                lastcacheindex = Math.Max(n - cparam1, 0); ////Math.Min(n+1, cacheList.Count);
                                break;
                            }
                        }
                    }

                    // Check sorted tree table.
                    if (cs == null)
                    {
                        CacheTreeEntry cte = (CacheTreeEntry)this.cacheTree.FindHighestSmallerOrEqualKey(adjustedTableIndex);
                        if (cte != null)
                        {
                            cs = cte.cs;
                        }
                    }

                    if (cs != null && cs.cachedElement != null)
                    {
                        cachedAdjustedTableIndex = cs.cachedAdjustedTableIndex;
                        cachedElement = cs.cachedElement;
                        cachedElementCount = cs.cachedElementCount;
                        int nextCount = cs.cachedAdjustedTableIndex + cs.cachedElementCount;

                        //// Same element or child element of a nested table
                        if (adjustedTableIndex >= cs.cachedAdjustedTableIndex && adjustedTableIndex < nextCount)
                        {
                            ////if (!_stepInNestedTables && cachedElement.ParentTable != this._table)
                            ////    Debugger.Break();
                            lastcacheindex = Math.Max(cs.cacheIndex - cparam1, 0);
                            found = true;
                        }                       
                        else if (/*_table.FilteredChildTable == null && */adjustedTableIndex == nextCount)
                        { 
                            //// Subsequent element.
                            //// Careful: caching gets messed up when _table.FilteredChildTable is not null. GetNextElementStepIn
                            //// will step out of table, e.g. if filter is set on products table, GetNextElementStepIn might
                            //// setep into categories parent table.

                            cachedAdjustedTableIndex += cachedElementCount;
                            Element elx = cachedElement;
                            cachedElement = ElementHelper.GetNextElementStepIn(cachedElement, counterKind, null, this._stepInNestedTables, this._table);

                            if (this._stepInNestedTables)
                            {
                                cachedElementCount = 1;
                            }
                            else
                            {
                                // No need to call cachedElement.GetDisplayPosition();
                                // cachedAdjustedTableIndex is already pointing to the first display element of the nested table.
                                if (cachedElement != null)
                                {
                                    cachedElementCount = (int)cachedElement.GetCounter().GetValue(counterKind);
                                }
                            }

                            ////if (!_stepInNestedTables && cachedElement.ParentTable != this._table)
                            ////    Debugger.Break();
                            this.CacheElement(cachedElement, cachedAdjustedTableIndex, cachedElementCount);
                            lastcacheindex = Math.Max(cs.cacheIndex - cparam1, 0); ////Math.Min(n+1, cacheList.Count);
                            found = true;
                        }
                    }
#endif
                    if (found && cachedElement != null)
                    {
                        ////#if DIAGNOSING
                        ////                        if (true)
                        ////                        {
                        ////                            bool shouldCheckVisible = (counterKind == CounterKind.DisplayElementCount);
                        ////                            ////if (index == 141)
                        ////                            ////    Console.WriteLine(index);
                        ////                            int testIndexOf = IndexOfInternal(cachedElement, shouldCheckVisible, true);
                        ////                            ////int cachedElementCount = cachedElement.GetCounter().GetValue(counterKind);
                        ////
                        ////                            if (index < testIndexOf || index >=  testIndexOf+cachedElementCount)
                        ////                            {
                        ////                                TraceUtil.TraceCurrentMethodInfo(index);
                        ////                                TraceUtil.TraceCalledFrom(20);
                        ////                                testIndexOf = IndexOfInternal(cachedElement, shouldCheckVisible, true);
                        ////                                ////Debugger.Break();
                        ////                            }
                        ////
                        ////                            ////                                        Console.WriteLine(testIndexOf);
                        ////                        }
                        ////
                        countItemCacheHit++;
                        if (countItemCacheHit % 1000 == 0)
                        {
                            ////                            Console.WriteLine("Cache ration {0} Hits, {1} Missed, {2:0.00%} Ratio", countItemCacheHit, countItemNonCacheHit,
                            ////                                countItemCacheHit * 1f / (countItemCacheHit + countItemNonCacheHit));
                        }
                        ////#endif
                        if (childTable == null || cachedElement.ParentChildTable == childTable)
                        {
                            return cachedElement;
                        }
                        
                        //// TODO: this should not happen ... but preventing a bug from escalating
                        //// by simply ignoring the cached value and clearing the cache.
                        cacheList.Clear();
                        cacheTree = new TreeTable(true);
                        cacheElementIdTree = new TreeTable(true);
                    }
                }
                else
                {
                    cacheList.Clear();
                    cacheTree = new TreeTable(true);
                    cacheElementIdTree = new TreeTable(true);
                }

                // Look up the element and cache it together with adjusted table index.
                Element result;
                try
                {
                    result = ElementHelper.FindElement(_table, CounterFactory.CreateCounter(adjustedTableIndex, counterKind), _stepInNestedTables);
                    ////if (!_stepInNestedTables && result.ParentTable != this._table)
                    ////    Debugger.Break();
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    this._table.InvalidateCounterTopDown(true);
                    result = ElementHelper.FindElement(_table, CounterFactory.CreateCounter(adjustedTableIndex, counterKind), _stepInNestedTables);
                }

                if (cacheIt)
                {
#if DEBUG
                    if (result == null)
                    {
                        Debug.Assert(false); // Step into the next method to find out what happened.
                        result = ElementHelper.FindElement(_table, CounterFactory.CreateCounter(adjustedTableIndex, counterKind), _stepInNestedTables);
                    }
#endif
                    CacheState cs = CacheElement(result, adjustedTableIndex, -1);
                    if (cs != null)
                    {
                        lastcacheindex = Math.Max(cs.cacheIndex - cparam1, 0); ////Math.Min(n+1, cacheList.Count);
                    }
#if true || DIAGNOSING
                    countItemNonCacheHit++;
#endif
                }

                version = _table.EngineVersion;
                return result;
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

        CacheState CacheElement(Element el, int index, int count)
        {
            if (this.IsDisposed)
            {
                return null;
            }

            cachedElement = el;

            if (_stepInNestedTables && el is NestedTable)
            {
                return null;
            }

            ////if (!_stepInNestedTables && cachedElement.ParentTable != this._table)
            ////    Debugger.Break();
            if (count == -1)
            {
                if (_stepInNestedTables || !(el is NestedTable))
                {
                    cachedElementCount = 1;
                }
                else
                {
                    cachedElementCount = (int)cachedElement.GetCounter().GetValue(counterKind);
                }

                if (cachedElementCount == 1)
                {
                    cachedAdjustedTableIndex = index;
                }
                else
                { 
                    ////cachedAdjustedTableIndex = -1;
                    cachedAdjustedTableIndex = this.IndexOfInternal(cachedElement, false, false);
                }
            }
            else
            {
                cachedElementCount = count;
                cachedAdjustedTableIndex = index;
            }

            CacheState cs = new CacheState();

            cs.cachedAdjustedTableIndex = this.cachedAdjustedTableIndex;
            cs.cachedElement = this.cachedElement;
            cs.cachedElementCount = this.cachedElementCount;
            if (cacheListPointer < this.cacheList.Count)
            {
                CacheState oldState = this.cacheList[cacheListPointer] as CacheState;
                if (oldState != null)
                {
                    oldState.Detach();
                }

                this.cacheList[cacheListPointer] = cs;
                cs.cacheIndex = cacheListPointer;
            }
            else
            {
                this.cacheList.Add(cs);
                cs.cacheIndex = cacheListPointer;
                cacheListPointer = this.cacheList.Count;
            }

            if (cacheListPointer == cacheSpan)
            {
                cacheListPointer = 0;
            }

            this.cacheTree.AddSorted(new CacheTreeEntry(cs, cacheTree));
            if (cs.cachedElement != null && cs.cachedElement.SupportsId())
            {
                this.cacheElementIdTree.AddSorted(new CacheElementIdTreeEntry(cs, cacheElementIdTree));
            }

            return cs;
        }

        /// <summary>
        /// Checks if the group belongs to the details section and is visible.
        /// </summary>
        /// <param name="value">Group element.</param>
        /// <returns>True if the group belongs to the details section and is visible; False otherwise.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(Element value)
        {
            if (value == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);

            bool shouldCheckVisible = counterKind == CounterKind.DisplayElementCount;

            // First check if any ParentTable matches this table
            // and save the parent element that is at the same level as this table
            // (in case this of a child table the NestedTable element will be at the same level as _table).
            Element childElement = value;
            Element parentElement = value;
            bool tableFound = false;
            while (parentElement != null)
            {
                if (parentElement.IsDisposed)
                {
                    return false;
                }

                if (shouldCheckVisible)
                {
                    if (parentElement != childElement && !parentElement.IsChildVisible(childElement))
                    {
                        return false;
                    }
                }

                if (parentElement.ParentTable == this._table)
                {
                    tableFound = true;
                    break;
                }

                // Get ParentElement or childTable.ParentNestedTable.
                childElement = parentElement;
                parentElement = parentElement.ParentDisplayElement;
            }

            if (!tableFound)
            {
                return false;
            }

            // Check if any ChildTable matches the filtered child table.
            ChildTable filteredChildTable = this.GetFilterChildTable();
            if (filteredChildTable == null)
            {
                return true;
            }

            ChildTable childTable = parentElement is ChildTable ? (ChildTable)parentElement : parentElement.ParentChildTable;
            return childTable == filteredChildTable;
        }

        int lastcacheindex = 0;

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
        public int IndexOf(Element value)
        {
            if (value == null)
            {
                return -1;
            }

            bool cacheIt = allowCache; ////&& _table.RelatedTables.Count > 0;

            if (!_stepInNestedTables)
            {
                //// If this element belongs to a nested table we need to
                //// get the NestedTable element that belongs to this table.
                while (value.ParentTable != this._table)
                {
                    ChildTable ct = value as ChildTable;
                    if (ct != null)
                    {
                        ct = ct.ParentChildTable;
                    }

                    if (ct == null)
                    {
                        return -1;
                    }

                    value = ct.ParentNestedTable;
                    if (value == null)
                    {
                        return -1;
                    }
                }
            }
            // Use cache if the same as last element returned from this[int index] operation.
            if (cacheIt)
            {
                if (version == _table.EngineVersion)
                {
                    int to = Math.Min(lastcacheindex + cparam2, cacheList.Count);
                    for (int n = lastcacheindex; n < to; n++)
                    {
                        CacheState cs = this.cacheList[n] as CacheState;
                        if (cs != null && cs.cachedAdjustedTableIndex >= 0)
                        {
                            if (Object.ReferenceEquals(cs.cachedElement, value))
                            {
                                lastcacheindex = Math.Max(n - cparam1, 0); ////Math.Min(n+1, cacheList.Count);
                                int index = cs.cachedAdjustedTableIndex;

                                ChildTable filteredChildTable = this.GetFilterChildTable();
                                if (filteredChildTable != null)
                                {
                                    if (value is NestedTable && Object.ReferenceEquals(((NestedTable)value).ChildTable, value))
                                    {
                                        return 0;
                                    }

                                    index -= this.GetFilterChildTablePosition(filteredChildTable);
                                }

                                return index;
                            }
                        }
                    }

                    if (value.SupportsId())
                    {
                        CacheElementIdTreeEntry ctel = (CacheElementIdTreeEntry)this.cacheElementIdTree.FindKey(value.Id);
                        if (ctel != null)
                        {
                            CacheState cs = ctel.cs;
                            if (cs != null && cs.cachedAdjustedTableIndex >= 0)
                            {
                                if (Object.ReferenceEquals(cs.cachedElement, value))
                                {
                                    lastcacheindex = Math.Max(cs.cacheIndex - cparam1, 0); ////Math.Min(n+1, cacheList.Count);
                                    int index = cs.cachedAdjustedTableIndex;

                                    ChildTable filteredChildTable = this.GetFilterChildTable();
                                    if (filteredChildTable != null)
                                    {
                                        index -= this.GetFilterChildTablePosition(filteredChildTable);
                                    }

                                    return index;
                                }
                            }
                        }
                    }
#if false

                for (int n = lastcacheindex; n < cacheList.Count; n++)
                {
                    CacheState cs = this.cacheList[n] as CacheState;
                    if (cs != null && cs.cachedAdjustedTableIndex >= 0)
                    {
                        if (Object.ReferenceEquals(cs.cachedElement, value))
                        {
                            lastcacheindex = Math.Max(n-5, 0);//Math.Min(n+1, cacheList.Count);
                            int index = this.GetCachedIndexOf(cs);

                            ChildTable filteredChildTable = this.GetFilterChildTable();
                            if (filteredChildTable != null)
                                index -= this.GetFilterChildTablePosition(filteredChildTable);
                            return index;
                        }
                    }
                }
                for (int n = Math.Min(cacheList.Count, lastcacheindex)-1; n >= 0; n--)
                {
                    CacheState cs = this.cacheList[n] as CacheState;
                    if (cs != null && cs.cachedAdjustedTableIndex >= 0)
                    {
                        if (Object.ReferenceEquals(cs.cachedElement, value))
                        {
                            lastcacheindex = Math.Max(n-5, 0);//0;//n;
                            int index = this.GetCachedIndexOf(cs);

                            ChildTable filteredChildTable = this.GetFilterChildTable();
                            if (filteredChildTable != null)
                                index -= this.GetFilterChildTablePosition(filteredChildTable);
                            return index;
                        }
                    }
                }
#endif
                }
                else
                {
                    cacheList.Clear();
                    // TODO: cacheTree.Clear();
                    cacheTree = new TreeTable(true);
                    cacheElementIdTree = new TreeTable(true);
                }
            }

            value.EnsureInitialized(this, true);

            bool shouldCheckVisible = counterKind == CounterKind.DisplayElementCount;

            int indexOf = IndexOfInternal(value, shouldCheckVisible, true);

            // Only cache value if sure that this is not a conatiner element (e.g. a GridRecord).
            if (cacheIt && !(value is IContainerElement && ((IContainerElement)value).ShouldStepIntoElements())
                && value.GetVisibleCount() > 0 && value.GetVisibleInHierarchy())
            {
                int count;
                if (_stepInNestedTables || !(value is NestedTable))
                {
                    count = 1;
                }
                else
                {
                    count = (int)value.GetCounter().GetValue(counterKind);
                }

                int adjustedIndex = indexOf;
                ChildTable filteredChildTable = this.GetFilterChildTable();
                if (filteredChildTable != null)
                {
                    adjustedIndex += this.GetFilterChildTablePosition(filteredChildTable);
                }

                this.CacheElement(value, adjustedIndex, count);
            }

            return indexOf;
        }
        
        int IndexOfInternal(Element value, bool shouldCheckVisible, bool adjustIndex)
        {
#if DEBUG
            if (!adjustIndex)
            {
                Debug.Assert(!shouldCheckVisible);
            }
#endif

            if (value == null || value.IsDisposed)
            {
                return -1;
            }

            // First check if any ParentTable matches this table
            // and save the parent element that is at the same level as this table
            // (in case this of a child table the NestedTable element will be at the same level as _table).
            Element childElement = value;
            Element parentElement = value;
            bool tableFound = false;
            while (parentElement != null)
            {
                if (shouldCheckVisible)
                {
                    if (parentElement != childElement && !parentElement.IsChildVisible(childElement))
                    {
                        return -1;
                    }
                }

                if (parentElement.ParentTable == this._table)
                {
                    tableFound = true;
                    break;
                }

                // Get ParentElement or childTable.ParentNestedTable.
                childElement = parentElement;
                parentElement = parentElement.ParentDisplayElement;
            }

            if (!tableFound)
            {
                return -1;
            }

            int filterChildTablePosition = 0;

            ChildTable childTable = parentElement is ChildTable ? (ChildTable)parentElement : parentElement.ParentChildTable;

            // Check if any ChildTable matches the filtered child table.
            ChildTable filteredChildTable = this.GetFilterChildTable();
            if (filteredChildTable != null && adjustIndex)
            {
                if (childTable != filteredChildTable)
                {
                    return -1;
                }

                ////filterChildTablePosition = ElementHelper.GetCumulatedPosition(childTable, counterKind);
                filterChildTablePosition = this.GetFilterChildTablePosition(childTable);
            }

            // parentElement is the Element ittself or the ParentNestedTable that is direct child of this table.
            int parentElementPosition = (int)ElementHelper.GetCumulatedPosition(parentElement, counterKind);
            int indexOfParentElementInTable = parentElementPosition - filterChildTablePosition;

            // childElement is either a ChildTable if the parent element is a NestedTable. Otherwise, it
            // is just a direct child of this table and we don't have to do ChildTable adjustments.
            if (this._stepInNestedTables && childElement != value && childElement is ChildTable)
            {
                ChildTable childTableInRelatedTable = (ChildTable)childElement;
                int indexOfElementInChildTableInRelatedTable;
                if (counterKind == CounterKind.DisplayElementCount)
                {
                    indexOfElementInChildTableInRelatedTable = childTableInRelatedTable.NestedDisplayElements.IndexOfInternal(value, false, true);
                }
                else
                {
                    indexOfElementInChildTableInRelatedTable = childTableInRelatedTable.NestedElements.IndexOfInternal(value, false, true);
                }

                return indexOfElementInChildTableInRelatedTable + indexOfParentElementInTable;
            }
            else
            {
                return indexOfParentElementInTable;
            }
        }

        internal ChildTable GetFilterChildTable()
        {
            if (_childTable != null)
            {
                return _childTable;
            }
            else if (_table.FilteredChildTable != null)
            {
                return _table.FilteredChildTable;
            }

            return null;
        }

        YamountCacheState CacheYAmount(Element el, double index, double count)
        {
            if (this.IsDisposed)
            {
                return null;
            }

            if (el is NestedTable && this._stepInNestedTables)
            {
                return null;
            }

            YamountCacheState cs = new YamountCacheState();
            cs.cachedYamountPosition = index;
            ////if (count > 100)
            ////    Debugger.Break();
            cs.cachedYamountCount = count;
            cs.cachedElement = el;
            if (yAmountCacheListPointer < this.yAmountCacheList.Count)
            {
                YamountCacheState oldState = this.yAmountCacheList[yAmountCacheListPointer] as YamountCacheState;
                if (oldState != null)
                {
                    oldState.Detach();
                }

                this.yAmountCacheList[yAmountCacheListPointer] = cs;
                cs.cacheIndex = yAmountCacheListPointer;
            }
            else
            {
                this.yAmountCacheList.Add(cs);
                cs.cacheIndex = yAmountCacheListPointer;
                yAmountCacheListPointer = this.yAmountCacheList.Count;
            }

            if (yAmountCacheListPointer == cacheSpan)
            {
                yAmountCacheListPointer = 0;
            }

            this.yAmountCacheTree.AddSorted(new YamountCacheTreeEntry(cs, this.yAmountCacheTree));
            if (cs.cachedElement != null && cs.cachedElement.SupportsId())
            {
                this.yAmountCacheElementIdTree.AddSorted(new YamountCacheElementIdTreeEntry(cs, yAmountCacheElementIdTree));
            }

            return cs;
        }

        /// <summary>
        /// Gets (and caches) the element at the zero-based YAmount. With a grouping grid, YAmount represents the
        /// vertical pixel scroll position of the grid.
        /// Setting a value is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        /// <param name="index">The Index.</param>
        /// <returns>returns Element.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Element GetItemAtYAmount(double index)
        {
            int count = Count;
            double adjustedIndex = index;

            ////if (!_stepInNestedTables)
            {
                if (_childTable != null)
                {
                    adjustedIndex += _childTable.GetYAmountPosition();
                }
                else if (_table.FilteredChildTable != null)
                {
                    adjustedIndex += _table.FilteredChildTable.GetYAmountPosition();
                }
            }

            bool cacheIt = allowCache; ////&& _table.RelatedTables.Count > 0;

            adjustedIndex = Math.Min(adjustedIndex, _table.GetYAmountCount());
#if true
            if (cacheIt)
            {
                if (yAmountVersion == _table.EngineVersion)
                {
                    bool found = false;

                    YamountCacheState cs = null;

                    int to = Math.Min(this.yAmountCacheListPointer + 1, yAmountCacheList.Count);
                    for (int n = Math.Max(this.yAmountCacheListPointer - 4, 0); n < to; n++)
                    {
                        YamountCacheState es = this.yAmountCacheList[n] as YamountCacheState;
                        if (es != null
                            && adjustedIndex >= es.cachedYamountPosition
                            && adjustedIndex < es.cachedYamountPosition + es.cachedYamountCount
                            && es.cachedYamountPosition >= 0)
                        {
                            cs = es;
                            ////lastyAmountCacheindex = Math.Max(n-cparam1, 0);//Math.Min(n+1, yAmountCacheList.Count);
                            break;
                        }
                    }
                    
                    ////                if (cs == null)
                    ////                {
                    ////                    to = Math.Min(lastyAmountCacheindex+cparam2, yAmountCacheList.Count);
                    ////                    for (int n = lastyAmountCacheindex; n < to; n++)
                    ////                    {
                    ////                        YamountCacheState es = this.yAmountCacheList[n] as YamountCacheState;
                    ////                        if (es != null
                    ////                            && adjustedIndex >= es.cachedYamountPosition
                    ////                            && adjustedIndex < es.cachedYamountPosition+es.cachedYamountCount
                    ////                            && es.cachedYamountPosition >= 0)
                    ////                        {
                    ////                            cs = es;
                    ////                            lastyAmountCacheindex = Math.Max(n-cparam1, 0);////Math.Min(n+1, yAmountCacheList.Count);
                    ////                            break;
                    ////                        }
                    ////                    }
                    ////                }

                    if (cs == null)
                    {
                        YamountCacheTreeEntry cte = (YamountCacheTreeEntry)this.yAmountCacheTree.FindHighestSmallerOrEqualKey(adjustedIndex);
                        if (cte != null)
                        {
                            cs = cte.cs;
                        }
                    }

                    if (cs != null && cs.cachedYamountCount > 0)
                    {
                        double nextIndex = cs.cachedYamountPosition + cs.cachedYamountCount;
                        if (adjustedIndex >= cs.cachedYamountPosition && adjustedIndex < nextIndex)
                        {
                            found = true;
                        }                           
                        else if (/*_table.FilteredChildTable == null && */adjustedIndex == nextIndex)
                        { 
                            // subsequent element?
                            // Careful: caching gets messed up when _table.FilteredChildTable is not null. GetNextElementStepIn
                            // will step out of table, e.g. if filter is set on products table, GetNextElementStepIn might
                            // setep into categories parent table.

                            ////Element next = ElementHelper.FindElement(_table, Counter.CreateYAmountCounter(adjustedIndex), _stepInNestedTables);
                            Element next = ElementHelper.GetNextElementStepIn(cs.cachedElement, CounterKind.YAmountCount, null, this._stepInNestedTables, this._table);
                            ////                            if (!Object.ReferenceEquals(next, elx))
                            ////                                Console.WriteLine(elx.Info);
                            if (next != null)
                            {
                                cs = this.CacheYAmount(next, adjustedIndex, next.GetYAmountCount());
                                found = true;
                            }
                        }
                    }
                    
                    if (found)
                    {
                        countItemyAmountCacheHit++;
                        ////                        if (countItemyAmountCacheHit % 1000 == 0)
                        ////                        {
                        ////                            Console.WriteLine("yAmountCache ration {0} Hits, {1} Missed, {2:0.00%} Ratio", countItemyAmountCacheHit, countItemyAmountNonCacheHit,
                        ////                                countItemyAmountCacheHit * 1f / (countItemyAmountCacheHit + countItemyAmountNonCacheHit));
                        ////                        }

                        ////allowCache = false;

                        ////Element testEl = this.GetItemAtYAmount(index);
                        ////Debug.Assert(cs.cachedElement == testEl);

                        ////allowCache = true;

                        ////#endif
                        return cs.cachedElement;
                    }
                }
                else
                {
                    yAmountCacheList.Clear();
                    yAmountCacheTree = new TreeTable(true);
                    yAmountCacheElementIdTree = new TreeTable(true);
                }
            }
#endif
            // Look up the element and cache it together with adjusted table index.
            Element result;
            try
            {
                result = ElementHelper.FindElement(_table, CounterFactory.CreateYAmountCounter(adjustedIndex), _stepInNestedTables);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                this._table.InvalidateCounterTopDown(true);
                result = ElementHelper.FindElement(_table, CounterFactory.CreateYAmountCounter(adjustedIndex), _stepInNestedTables);
            }

            if (allowCache && false)
            {
                YamountCacheState cs = CacheYAmount(result, adjustedIndex, result.GetYAmountCount());
                ////if (cs != null)
                ////    lastcacheindex = Math.Max(cs.cacheIndex-cparam1, 0);//Math.Min(n+1, cacheList.Count);
#if true || DIAGNOSING
                countItemyAmountNonCacheHit++;
#endif
            }

            yAmountVersion = _table.EngineVersion;

            return result;
        }

        int yAmountVersion = -1;
        static int countItemyAmountCacheHit = 0;
        static int countItemyAmountNonCacheHit = 0;
        int lastyAmountCacheindex;

        /// <summary>
        /// Returns the YAmount position of the element in the collection. With a grouping grid, YAmount represents the
        /// vertical pixel scroll position of the grid.
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
        public double GetYAmountPositionOf(Element value)
        {
            double index = 0f;

            bool cacheIt = allowCache; ////&& _table.RelatedTables.Count > 0;

#if true
            if (cacheIt)
            {
                // use yAmountCache if the same as last element returned from this[int index] operation.
                if (yAmountVersion == _table.EngineVersion)
                {
                    int to = Math.Min(lastyAmountCacheindex + cparam2, yAmountCacheList.Count);
                    for (int n = lastyAmountCacheindex; n < to; n++)
                    {
                        YamountCacheState cs = this.yAmountCacheList[n] as YamountCacheState;
                        if (cs != null && cs.cachedYamountPosition >= 0)
                        {
                            if (Object.ReferenceEquals(cs.cachedElement, value))
                            {
                                lastyAmountCacheindex = Math.Max(n - cparam1, 0); ////Math.Min(n+1, yAmountCacheList.Count);
                                index = cs.cachedYamountPosition;
                                return index; ////found = true;
                            }
                        }
                    }

                    if (value.SupportsId())
                    {
                        YamountCacheElementIdTreeEntry ctel = (YamountCacheElementIdTreeEntry)this.yAmountCacheElementIdTree.FindKey(value.Id);
                        if (ctel != null)
                        {
                            YamountCacheState cs = ctel.cs;
                            if (cs != null && cs.cachedYamountPosition >= 0)
                            {
                                if (Object.ReferenceEquals(cs.cachedElement, value))
                                {
                                    lastyAmountCacheindex = Math.Max(cs.cacheIndex - cparam1, 0); ////Math.Min(n+1, yAmountCacheList.Count);
                                    index = cs.cachedYamountPosition;
                                    return index; ////found = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    yAmountCacheList.Clear();
                    yAmountCacheTree = new TreeTable(true);
                    yAmountCacheElementIdTree = new TreeTable(true);
                }
            }
#endif

            if (!Contains(value) || value.IsDisposed)
            {
                return -1;
            }
            else
            {
                index = value.GetYAmountPosition();

                if (!_stepInNestedTables)
                {
                    if (_childTable != null)
                    {
                        index -= _childTable.GetYAmountPosition();
                    }
                    else if (_table.FilteredChildTable != null)
                    {
                        index -= _table.FilteredChildTable.GetYAmountPosition();
                    }
                }
                else
                {
                    ChildTable childTable = value.ParentChildTable;
                    while (childTable != null)
                    {
                        index -= childTable.GetYAmountPosition();
                        NestedTable nestedTable = childTable.ParentNestedTable;
                        if (nestedTable != null)
                        {
                            index += nestedTable.GetYAmountPosition();
                            childTable = nestedTable.ParentChildTable;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (cacheIt && value.SupportsId())
            {
                // Check for special case where GetYAmountPositionOf is called for Table.CurrentElement
                // in which case Record could be expanded and have many nested elements.
                if (!(value is IContainerElement && ((IContainerElement)value).ShouldStepIntoElements()))
                {
                    this.CacheYAmount(value, index, value.GetYAmountCount());
                }
            }

            return index;
        }
        
        /// <summary>
        /// Gets the element at the zero-based CustomCount position.
        /// </summary>
        /// <param name="index">Custom count position.</param>
        /// <returns>Element at the given position.</returns>
        public Element GetItemAtCustomCount(double index)
        {
            int count = Count;
            double adjustedIndex = index;

            if (_childTable != null)
            {
                adjustedIndex += _childTable.GetCustomPosition();
            }
            else if (_table.FilteredChildTable != null)
            {
                adjustedIndex += _table.FilteredChildTable.GetCustomPosition();
            }

            adjustedIndex = Math.Min(adjustedIndex, _table.GetCustomCount());

            // Look up the element and cache it together with adjusted table index.
            Element result;
            try
            {
                result = ElementHelper.FindElement(_table, CounterFactory.CreateCustomCounter(adjustedIndex), _stepInNestedTables);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                this._table.InvalidateCounterTopDown(true);
                result = ElementHelper.FindElement(_table, CounterFactory.CreateCustomCounter(adjustedIndex), _stepInNestedTables);
            }

            return result;
        }
        
        /// <summary>
        /// Returns the CustomCount position of the element in the collection.
        /// </summary>
        /// <param name="value">The Element.</param>
        /// <returns>Custom count position.</returns>
        public double GetCustomCountPositionOf(Element value)
        {
            double index = 0f;

            if (!Contains(value) || value.IsDisposed)
            {
                return -1;
            }
            else
            {
                index = value.GetCustomPosition();

                if (!_stepInNestedTables)
                {
                    if (_childTable != null)
                    {
                        index -= _childTable.GetCustomPosition();
                    }
                    else if (_table.FilteredChildTable != null)
                    {
                        index -= _table.FilteredChildTable.GetCustomPosition();
                    }
                }
                else
                {
                    ChildTable childTable = value.ParentChildTable;
                    while (childTable != null)
                    {
                        index -= childTable.GetCustomPosition();
                        NestedTable nestedTable = childTable.ParentNestedTable;
                        if (nestedTable != null)
                        {
                            index += nestedTable.GetCustomPosition();
                            childTable = nestedTable.ParentChildTable;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return index;
        }
        
        /// <summary>
        /// Gets the element at the zero-based CustomCount position.
        /// </summary>
        /// <param name="index">Custom count position.</param>
        /// <returns>Element at the given position.</returns>
        public Element GetItemAtVisibleCustomCount(double index)
        {
            int count = Count;
            double adjustedIndex = index;

            if (_childTable != null)
            {
                adjustedIndex += _childTable.GetCustomPosition();
            }
            else if (_table.FilteredChildTable != null)
            {
                adjustedIndex += _table.FilteredChildTable.GetVisibleCustomPosition();
            }

            adjustedIndex = Math.Min(adjustedIndex, _table.GetVisibleCustomPosition());

            // Look up the element and cache it together with adjusted table index.
            Element result;
            try
            {
                result = ElementHelper.FindElement(_table, CounterFactory.CreateVisibleCustomCounter(adjustedIndex), _stepInNestedTables);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                this._table.InvalidateCounterTopDown(true);
                result = ElementHelper.FindElement(_table, CounterFactory.CreateVisibleCustomCounter(adjustedIndex), _stepInNestedTables);
            }

            return result;
        }

        /// <summary>
        /// Returns the CustomCount position of the element in the collection.
        /// </summary>
        /// <param name="value">The Element.</param>
        /// <returns>Custom count position of the element.</returns>
        public double GetVisibleCustomCountPositionOf(Element value)
        {
            double index = 0f;

            if (!Contains(value) || value.IsDisposed)
            {
                return -1;
            }
            else
            {
                index = value.GetVisibleCustomPosition();

                if (!_stepInNestedTables)
                {
                    if (_childTable != null)
                    {
                        index -= _childTable.GetVisibleCustomPosition();
                    }
                    else if (_table.FilteredChildTable != null)
                    {
                        index -= _table.FilteredChildTable.GetVisibleCustomPosition();
                    }
                }
                else
                {
                    ChildTable childTable = value.ParentChildTable;
                    while (childTable != null)
                    {
                        index -= childTable.GetVisibleCustomPosition();
                        NestedTable nestedTable = childTable.ParentNestedTable;
                        if (nestedTable != null)
                        {
                            index += nestedTable.GetVisibleCustomPosition();
                            childTable = nestedTable.ParentChildTable;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void CopyTo(Element[] array, int index)
        {
            int n = 0;
            foreach (Element element in this)
            {
                array[index + n] = element;
                n++;
            }
        }

        ////        public RuntimeElementsInTableCollection SyncRoot
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
        public RuntimeElementsInTableCollectionEnumerator GetEnumerator()
        {
            return new RuntimeElementsInTableCollectionEnumerator(this);
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
        /// Not supported because collection is readonly.
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
            return Contains((Element)value);
        }

        void IList.Clear()
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Element)value);
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

#if PERF
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int TicksCount = 0;
#endif

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
                try
                {
                    if (_table == null)
                    {
                        return 0;
                    }

                    _table.EnsureInitialized(this, true);
                    if (_childTable != null)
                    {
                        return (int)_childTable.GetCounter().GetValue(counterKind);
                    }
                    else if (_table.FilteredChildTable != null)
                    {
                        return (int)_table.FilteredChildTable.GetCounter().GetValue(counterKind);
                    }

                    int count;
                    try
                    {
#if PERF
                        int ticks = Environment.TickCount;
#endif
                        count = (int)_table.GetCounter().GetValue(counterKind);
#if PERF
                        int _ticksCount = Environment.TickCount - ticks;
                        if (_ticksCount > 100)
                            TicksCount = _ticksCount;
#endif

                        countCache[counterKind] = count;
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (countCache.ContainsKey(counterKind))
                        {
                            return (int)countCache[counterKind];
                        }

                        return 0;
                    }

                    return count;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the YAmount count for the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// The method calls <see cref="Table.EnsureInitialized"/>.
        /// </remarks>
        public double YAmountCount
        {
            get
            {
                try
                {
                    int counterKind = CounterKind.YAmountCount;
                    if (_table == null)
                    {
                        return 0;
                    }

                    _table.EnsureInitialized(this);
                    if (_childTable != null)
                    {
                        return (int)_childTable.GetCounter().GetValue(counterKind);
                    }
                    else if (_table.FilteredChildTable != null)
                    {
                        return (int)_table.FilteredChildTable.GetCounter().GetValue(counterKind);
                    }

                    double count;
                    try
                    {
#if PERF
                        int ticks = Environment.TickCount;
#endif
                        count = (double)_table.GetCounter().GetValue(counterKind);
#if PERF
                        int _ticksCount = Environment.TickCount - ticks;
                        if (_ticksCount > 100)
                            TicksCount = _ticksCount;
#endif

                        countCache[counterKind] = count;
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (countCache.ContainsKey(counterKind))
                        {
                            return (double)countCache[counterKind];
                        }

                        return 0;
                    }

                    return count;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Element[])array, index);
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
    /// Enumerator class for <see cref="Element"/> objects of a <see cref="DisplayElementsInTableCollection"/> or <see cref="ElementsInTableCollection"/> .
    /// </summary>
    public class RuntimeElementsInTableCollectionEnumerator : IEnumerator
    {
        Element _cursor, _next;
        RuntimeElementsInTableCollection _coll;
        ChildTable filteredChildTable;
        int skipElements = 0;
        int nextIndex = -1;  // just for debugging purpose ...

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
        public RuntimeElementsInTableCollectionEnumerator(RuntimeElementsInTableCollection collection)
        {
            _coll = collection;
            Reset();
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            filteredChildTable = _coll._childTable;
            if (filteredChildTable == null)
            {
                filteredChildTable = _coll._table.FilteredChildTable;
            }

            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0];
                nextIndex = 0;
                if (!_coll._stepInNestedTables)
                {
                    skipElements = (int)_next.GetCounter().GetValue(_coll.counterKind) - 1;
                }
            }
            else
            {
                _next = null;
                nextIndex = -1;
                skipElements = 0;
            }
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
        public Element Current
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
            nextIndex++;

            if (_coll._stepInNestedTables || --skipElements < 0)
            {
                _next = ElementHelper.GetNextElementStepIn(_next, _coll.counterKind, null, _coll._stepInNestedTables, _coll._table);

                if (!_coll._stepInNestedTables)
                {
                    skipElements = 0;
                    if (_next != null)
                    {
                        if (_coll.counterKind == CounterKind.ElementsCount)
                        {
                            skipElements = _next.GetElementCount() - 1;
                        }
                        else
                        {
                            skipElements = _next.GetVisibleCount() - 1;
                        }
                    }
                }
                // don't step out/iterate into display elements of parents table.
                if (_next != null)
                {
                    if (filteredChildTable != null && _next.ParentChildTable != filteredChildTable)
                    {
                        _next = null;
                    }
                }
            }

            return _cursor != null;
        }
        #endregion
    }
}