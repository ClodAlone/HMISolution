//-------------------------------------------------------------------------------------------------
// <copyright file="SortedRecordsTree.cs" company="syncfusion">
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

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;

using Syncfusion.Grouping;

namespace Syncfusion.Grouping.Internals
{
    internal class VirtualSortedRecordsTreeTableEntry : SortedRecordsTreeTableEntry
    {
        int position;
        internal Hashtable virtualEntryCache;
        ElementTreeTable t;

        ~VirtualSortedRecordsTreeTableEntry()
        {
            if (t != null && !t.VirtualModeLockEntries)
            {
                if (virtualEntryCache != null && virtualEntryCache.Contains(this.position))
                    virtualEntryCache.Remove(this.position);
            }
        }

        public VirtualSortedRecordsTreeTableEntry(ElementTreeTable t)
        {
            this.t = t;
        }

        public int VPosition
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public override int GetPosition()
        {
            return position;
        }

        public override int GetCount()
        {
            return 1;
        }

        public override ITreeTableCounter GetCounterPosition()
        {
            ////return CounterFactory.CreateCounter(1);
            if (t == null)
                return null;
            int c = position;
            ////ElementTreeTable t = (ElementTreeTable) this.Tree.Tag;
            return t.CounterFactory.CreateCounter(
                c * t.GetCounterKindFactor(CounterKind.DisplayElementCount),
                c * (double)t.GetCounterKindFactor(CounterKind.YAmountCount),
                c,
                c * t.GetCounterKindFactor(CounterKind.ElementsCount),
                c,
                0,
                0);
        }
    }

    internal class SortedRecordsTreeTableEntry : ElementTreeTableEntry
    {
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Avoid calling dispose on element, only UnsortedRecordsTree should dispose record.
                if (!(Value is AddNewRecord))
                {
                    Value = null;
                }
            }

            base.Dispose(disposing);
        }

        public SortedRecordsTreeTable SortedRecordsTreeTable
        {
            get
            {
                TreeTable tree = this.Tree;
                if (tree != null)
                {
                    return tree.Tag as SortedRecordsTreeTable;
                }

                return null;
            }
        }

        /// <summary>
        /// The cumulative position of this node.
        /// </summary>
        /// <returns>returns cumulative position</returns>
        public override ITreeTableCounter GetCounterPosition()
        {
            SortedRecordsTreeTable tree = SortedRecordsTreeTable;
            if (tree != null && tree.WithoutCounter)
            {
                int c = GetPosition();
                ////return CounterFactory.CreateCounter(c);
                ElementTreeTable t = SortedRecordsTreeTable;
                return t.CounterFactory.CreateCounter(
                    c * t.GetCounterKindFactor(CounterKind.DisplayElementCount),
                    c * (double)t.GetCounterKindFactor(CounterKind.YAmountCount),
                    c,
                    c * t.GetCounterKindFactor(CounterKind.ElementsCount),
                    c,
                    0,
                    0);
            }

            if (Parent == null)
            {
                if (TreeTableWithCounter == null)
                {
                    return null;
                }

                return TreeTableWithCounter.GetStartCounterPosition();
            }

            return Parent.GetCounterPositionOfChild(this);
        }

        public new Record Element
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
        /// By default, elements do not get sorted.
        /// </summary>
        /// <returns>returns the Element</returns>
        /// <override/>
        public override object GetSortKey()
        {            
            return Element;
        }
    }

    internal class SortedRecordsTreeTable : ElementTreeTable
    {
        RecordsDetails _owner = null;
        Hashtable virtualEntryCache;

        bool withoutCounter = false;
        bool virtualMode = false;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (virtualEntryCache != null)
                {
                    foreach (WeakReference wr in this.virtualEntryCache.Values)
                    {
                        VirtualSortedRecordsTreeTableEntry entry = wr.Target as VirtualSortedRecordsTreeTableEntry;
                        if (entry != null)
                        {
                            entry.Dispose();
                        }
                    }
                }

                virtualEntryCache = null;
            }

            base.Dispose(disposing);
        }
        
        public override bool WithoutCounter
        {
            get
            {
                return withoutCounter;
            }

            set
            {
                if (withoutCounter != value)
                {
                    virtualEntryCache = null;
                    withoutCounter = value;
                }
            }
        }

        public override bool VirtualMode
        {
            get
            {
                return virtualMode;
            }

            set
            {
                if (virtualMode != value)
                {
                    virtualEntryCache = null;
                    virtualMode = value;
                }
            }
        }

        public override int VirtualCount
        {
            get
            {
                IList list = this._owner.ParentTable.SourceList;
                return list != null ? list.Count : 0;
            }

            set
            {
            }
        }

        public SortedRecordsTreeTable(RecordsDetails owner)
            : base(owner)
        {
            _owner = owner;
            this.AllowSetItem = false;
        }

        protected override void OnVirtualModeSetItem(int index, ElementTreeTableEntry value)
        {
            if (virtualEntryCache == null)
            {
                virtualEntryCache = new Hashtable();
            }

            Record r = (Record)value.Element;
            virtualEntryCache[r.sourceIndex] = new WeakReference(value);
        }

        bool virtualModeLockEntries = false;

        protected override void OnVirtualModeInsert(int index, ElementTreeTableEntry value)
        {
            if (virtualEntryCache == null)
            {
                return;
            }

            virtualModeLockEntries = true;
            Hashtable ht = new Hashtable();
            foreach (WeakReference wr in this.virtualEntryCache.Values)
            {
                VirtualSortedRecordsTreeTableEntry entry = wr.Target as VirtualSortedRecordsTreeTableEntry;
                if (entry == null)
                {
                    continue;
                }
                else if (entry.VPosition >= index)
                {
                    entry.VPosition++;
                    ((Record)entry.Value).sourceIndex = entry.VPosition;
                }

                ht.Add(entry.VPosition, new WeakReference(entry));
            }

            VirtualSortedRecordsTreeTableEntry e = value as VirtualSortedRecordsTreeTableEntry;
            e.VPosition = index;
            ((Record)value.Element).sourceIndex = index;
            ht.Add(value.GetPosition(), new WeakReference(value));
            this.virtualEntryCache = ht;
            virtualModeLockEntries = false;
        }

        public override bool VirtualModeLockEntries
        {
            get
            {
                return virtualModeLockEntries;
            }
        }

        protected override void OnVirtualModeRemoveAt(int index)
        {
            if (virtualEntryCache == null)
            {
                return;
            }

            virtualModeLockEntries = true;
            Hashtable ht = new Hashtable();
            foreach (WeakReference wr in this.virtualEntryCache.Values)
            {
                VirtualSortedRecordsTreeTableEntry entry = wr.Target as VirtualSortedRecordsTreeTableEntry;
                if (entry == null || entry.VPosition == index)
                {
                    continue;
                }
                else if (entry.VPosition > index)
                {
                    entry.VPosition--;
                    ((Record)entry.Value).sourceIndex = entry.VPosition;
                }

                ht.Add(entry.VPosition, new WeakReference(entry));
            }

            this.virtualEntryCache = ht;
            virtualModeLockEntries = false;
        }

        protected override void OnVirtualModeClear()
        {
            if (virtualEntryCache == null)
            {
                return;
            }

            this.virtualEntryCache.Clear();
        }

        public override int GetCounterKindFactor(int counterKind)
        {
            if (this.WithoutCounter)
            {
                if (!_owner.ParentTable.RecordsAsDisplayElements)
                {
                    if (counterKind == CounterKind.DisplayElementCount || counterKind == CounterKind.ElementsCount)
                    {
                        return this._owner.ParentTableDescriptor.RowsPerRecord + this._owner.ParentTableDescriptor.PreviewRowsPerRecord;
                    }
                    else if (counterKind == CounterKind.YAmountCount)
                    {
                        return (_owner.ParentTable.DefaultRecordRowHeight * this._owner.ParentTableDescriptor.RowsPerRecord)
                              + (_owner.ParentTable.DefaultRecordPreviewRowHeight * this._owner.ParentTableDescriptor.PreviewRowsPerRecord);
                    }
                }
                else
                {
                    if (counterKind == CounterKind.YAmountCount)
                    {
                        return _owner.ParentTable.DefaultRecordRowHeight;
                    }
                }
            }

            return 1;
        }

        public override ElementTreeTableEntry GetInnerItem(int index)
        {
            if (this.VirtualMode)
            {
                if (virtualEntryCache == null)
                {
                    virtualEntryCache = new Hashtable();
                }
                else
                {
                    WeakReference wr = (WeakReference)virtualEntryCache[index];
                    if (wr != null)
                    {
                        ElementTreeTableEntry e = wr.Target as ElementTreeTableEntry;
                        if (e != null)
                        {
                            return e;
                        }
                    }
                }

                Record r = _owner.Engine.CreateRecord(_owner.ParentTable);
                return EnsureVirtualSortedRecordsTreeTableEntry(r, index);
            }

            return base.GetInnerItem(index);
        }

        public VirtualSortedRecordsTreeTableEntry EnsureVirtualSortedRecordsTreeTableEntry(Record r, int index)
        {
            VirtualSortedRecordsTreeTableEntry sortedEntry = new VirtualSortedRecordsTreeTableEntry(this);
            sortedEntry.VPosition = index;
            ////sortedEntry.Parent = (TreeTableWithCounterBranch) Root;
            sortedEntry.Tree = TreeTable;
            sortedEntry.Element = r;
            r.SortedEntry = sortedEntry;
            r.ParentElement = _owner;
            r.sourceIndex = index;
            r.sourceListVersion = _owner.ParentTable.SourceListVersion;
            sortedEntry.virtualEntryCache = virtualEntryCache;
            if (virtualEntryCache == null)
            {
                virtualEntryCache = new Hashtable();
            }

            virtualEntryCache[r.sourceIndex] = new WeakReference(sortedEntry);
            return sortedEntry;
        }

        public RecordsDetails Owner
        {
            get
            {
                return _owner;
            }
        }

        public new SortedRecordsTreeTableEntry this[int index]
        {
            get
            {
                return (SortedRecordsTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}