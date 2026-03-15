//-------------------------------------------------------------------------------------------------
// <copyright file="ElementHelper.cs" company="syncfusion">
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
using System.Text;

using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping.Internals
{
    internal interface IElement
    {
        bool EnsureInitialized(object sender, bool notifyParent);
    }

    interface IDisplayElement
    {
    }

    interface IContainerElement : IElementTreeTableSource, IElement
    {
        bool ShouldStepIntoElements();
    }

    internal class ElementHelper
    {
        public static ElementTreeTable GetTreeEntries(Element el, bool displayOrder)
        {
            if (el is IElementTreeTableSource)
            {
                return ((IElementTreeTableSource)el).GetChildElementTreeTable(displayOrder);
            }

            return null;
        }

        public static Element GetElementAtCounterPosition(Element el, ITreeTableCounter counter)
        {
            ElementTreeTableEntry entry = GetEntryAtCounterPosition(el, counter);
            if (entry != null)
            {
                return entry.Element;
            }

            return null;
        }

        public static ElementTreeTableEntry GetEntryAtCounterPosition(Element el, ITreeTableCounter counter)
        {
            IElementTreeTableSource cs = el as IElementTreeTableSource;
            if (cs != null)
            {
                return GetEntryAtCounterPosition(cs.GetChildElementTreeTable(true), counter);
            }

            return null;
        }

        public static ElementTreeTableEntry GetEntryAtCounterPosition(ElementTreeTable table, ITreeTableCounter counter)
        {
            return table.GetEntryAtCounterPosition(counter);
        }

        public static double GetInnerPosition(Element el, int cookie)
        {
            ElementTreeTableEntry entry = el.GetElementEntry();
            if (entry != null)
            {
                ITreeTableCounter pos = entry.GetCounterPosition();
                if (pos != null)
                {
                    return pos.GetValue(cookie);
                }
            }

            return 0;
        }

        public static double GetParentPosition(Element el, int cookie)
        {
            Element parent = el.ParentElement;
            if (parent != null)
            {
                return GetCumulatedPosition(parent, cookie);
            }

            return 0;
        }

        public static double GetCumulatedPosition(Element el, int cookie)
        {
            double n1 = GetInnerPosition(el, cookie);
            double n2 = GetParentPosition(el, cookie);
            if (n1 < 0)
#if DIAGNOSING
            {
                Debugger.Break();
                n1 = GetInnerPosition(el, cookie);
            }
#else
                n1 = 0;
#endif
            return n1 + n2;
            ////return GetInnerPosition(el, cookie) + GetParentPosition(el, cookie);
        }

        public static Element FindElement(Table _table, ITreeTableCounter index)
        {
            Element el2 = __FindElement(_table, index, null, false);
            return el2;
        }

        public static Element FindElement(Table _table, ITreeTableCounter index, bool stepInNestedTables)
        {
            Element el2 = __FindElement(_table, index, null, stepInNestedTables);
            return el2;
        }

        static ICounterFactory GetCounterFactory(Element el)
        {
            return el.Engine.CounterFactory;
        }

        public static Element __FindElement(Group _group, ITreeTableCounter index, Type t, bool stepInNestedTables)
        {
            int pos = (int)GetCumulatedPosition(_group, index.Kind);
            ITreeTableCounter findPosition = GetCounterFactory(_group).CreateCounter(pos + (int)index.GetValue(index.Kind), index.Kind);
            return __FindElement(_group.ParentTable, findPosition, t, stepInNestedTables);
        }

        public static Element __FindElement(Table _table, ITreeTableCounter index, Type t, bool stepInNestedTables)
        {
            Element g = _table.TopLevelGroup;
            int findPos = (int)index.GetValue(index.Kind);

            while (g != null)
            {
                int pos = (int)GetInnerPosition(g, index.Kind);
                ////int pos = g.VisiblePositionInSection;
                //// Groups is only an abstract element - only the caption, its records etc are visible elements.

                findPos = Math.Max(0, findPos - pos);
                ////Section s = g.VisibleSections[findPos];
                g.EnsureInitialized(g);
                Element s = GetElementAtCounterPosition(g, GetCounterFactory(_table).CreateCounter(findPos, index.Kind));

                if (s == null)
                {
                    return null;
                }

                int count = (int)s.GetCounter().GetValue(index.Kind);
                if (count == 0)
                {
                    return g;
                }

                if (ElementHelper.GetTreeEntries(s, true) == null)
                {
                    return s;
                }

                ////pos = s.VisiblePositionInSection;
                pos = (int)GetInnerPosition(s, index.Kind);
                //// DetailsSection is an abstract element - only its members are visible.
                //// Caption, footer, header are considered visible elements. 

                // !(s is DetailsSection))
                if (pos == findPos && t != null && t.IsAssignableFrom(s.GetType()))
                {
                    return s;
                }
                // !(s is DetailsSection))
                if (pos >= findPos && !(s is IContainerElement))
                {
                    return s;
                }

                findPos -= pos;
                s.EnsureInitialized(s);
                Element el = GetElementAtCounterPosition(s, GetCounterFactory(_table).CreateCounter(findPos, index.Kind));
                // !(s is DetailsSection))
                if (t != null && t.IsAssignableFrom(el.GetType()))
                {
                    return el;
                }
                else if ((el is NestedTable && stepInNestedTables)
                    || (el is IContainerElement && ((IContainerElement)el).ShouldStepIntoElements()))
                {
                    g = (Element)el;
                }
                else
                {
                    return el;
                }
            }

            return null;
        }

        public static Element GetNextSiblingElement(Element el, int cookie)
        {
            ElementTreeTableEntry current = el.GetElementEntry();
            if (current == null)
            {
                return null;
            }

            ElementTreeTable tree = GetTreeEntries(el.ParentElement, true);
            if (tree == null)
            {
                return null;
            }

            ElementTreeTableEntry next = tree.GetNextCounterEntry(current, cookie);

            while (next != null)
            {
                if (!CounterKind.IsVisibleCounter(cookie) || next.Element.ParentElement == null || next.Element.ParentElement.IsChildVisible(next.Element))
                {
                    break;
                }

                next = tree.GetNextCounterEntry(next, cookie);
            }

            if (next == null)
            {
                return null;
            }

            return next.Element;
        }

        public static Element GetNextElementStepIn(Element el, int cookie)
        {
            return GetNextElementStepIn(el, cookie, null);
        }

        public static Element GetNextElementStepIn(Element el, int cookie, Type finalType)
        {
            return GetNextElementStepIn(el, cookie, finalType, false, null);
        }

        public static Element GetNextElementStepIn(Element el, int cookie, Type finalType, bool stepInNestedTables, Table outerTable)
        {
            Element next = el;

            ElementTreeTable treeEntries = GetTreeEntries(el, true);

            if (treeEntries != null
                && (finalType == null || !finalType.IsAssignableFrom(el.GetType()))
                && ((el is NestedTable && stepInNestedTables) || !(el is IContainerElement) || ((IContainerElement)el).ShouldStepIntoElements())
                && (treeEntries.GetCounterTotal().GetValue(cookie) > 0))
            {
                {
                    ElementTreeTableEntry entry = treeEntries.GetEntryAtCounterPosition(treeEntries.GetStartCounterPosition(), cookie, false);
                    if (entry != null)
                    {
                        next = entry.Element;
                    }
                }
            }
            else
            {
                next = ElementHelper.GetNextSiblingElement(next, cookie);
            }

            while (next != null)
            {
                if (!CounterKind.IsVisibleCounter(cookie) || next.ParentElement.IsChildVisible(next))
                {
                    break;
                }

                next = ElementHelper.GetNextSiblingElement(next, cookie);
            }

            if (next != null)
            {
                next.EnsureInitialized(next);
                if (!((next is NestedTable && stepInNestedTables)
                    || (next is IContainerElement && ((IContainerElement)next).ShouldStepIntoElements())))
                {
                    return next;
                }

                if ((finalType == null || !finalType.IsAssignableFrom(el.GetType()))
                    && next is IElementTreeTableSource)
                {
                    treeEntries = GetTreeEntries(next, true);
                    if (treeEntries == null || treeEntries.GetCounterTotal().GetValue(cookie) == 0)
                    {
                        return next;
                    }

                    return ElementHelper.GetNextElementStepIn(next, cookie, finalType, stepInNestedTables, outerTable);
                }
                else
                {
                    return next;
                }
            }

            Element parent = GetParentElement(el, stepInNestedTables, outerTable);
            while (parent != null)
            {
                next = ElementHelper.GetNextSiblingElement(parent, cookie);
                if (next != null)
                {
                    if (!CounterKind.IsVisibleCounter(cookie) || next.ParentElement.IsChildVisible(next))
                    {
                        if (next is IElementTreeTableSource)
                        {
                            treeEntries = GetTreeEntries(next, true);
                            if (treeEntries == null || treeEntries.GetCounterTotal().GetValue(cookie) == 0)
                            {
                                return next;
                            }

                            return ElementHelper.GetNextElementStepIn(next, cookie, finalType, stepInNestedTables, outerTable);
                        }
                        else
                        {
                            return next;
                        }
                    }
                }

                parent = GetParentElement(parent, stepInNestedTables, outerTable);
            }

            return null;
        }

        public static Element GetParentElement(Element el, bool stepInNestedTables, Table outerTable)
        {
            if (stepInNestedTables)
            {
                if (el is ChildTable && !Object.ReferenceEquals(el.ParentTable, outerTable))
                {
                    el = ((ChildTable)el).ParentNestedTable;
                }
            }

            if (el != null)
            {
                el = el.ParentElement;
            }

            // Skip also child table if it was parent ... otherwise we get the sibbling in the related table.
            if (stepInNestedTables)
            {
                if (el is ChildTable && !Object.ReferenceEquals(el.ParentTable, outerTable))
                {
                    el = ((ChildTable)el).ParentNestedTable;
                }
            }

            return el;
        }

        public static Element GetNextSibling(Element el)
        {
            ITreeTableEntry current = el.GetElementEntry() as ITreeTableEntry;
            if (current == null)
            {
                return null;
            }

            ITreeTable tree = el.GetElementEntryTable();
            if (tree == null)
            {
                return null;
            }

            ElementTreeTableEntry next = tree.GetNextEntry(current) as ElementTreeTableEntry;
            if (next == null)
            {
                return null;
            }

            return next.Element;
        }

        public static Element GetPreviousSibling(Element el)
        {
            ITreeTableEntry current = el.GetElementEntry() as ITreeTableEntry;
            if (current == null)
            {
                return null;
            }

            ITreeTable tree = el.GetElementEntryTable();
            if (tree == null)
            {
                return null;
            }

            ElementTreeTableEntry next = tree.GetPreviousEntry(current) as ElementTreeTableEntry;
            if (next == null)
            {
                return null;
            }

            return next.Element;
        }

        public static Element GetNextStepIn(Element el)
        {
            Element next = el;
            ElementTreeTable treeEntries = ElementHelper.GetTreeEntries(next, true);
            if (treeEntries != null && treeEntries.Count > 0)
            {
                ////ElementTreeTableEntry entry = (ElementTreeTableEntry) treeEntries.GetEntryAtCounterPosition(counter, CounterKind.ElementsCount);
                ElementTreeTableEntry entry = (ElementTreeTableEntry)treeEntries[0];
                if (entry != null)
                {
                    next = entry.Element;
                }
            }
            else
            {
                next = ElementHelper.GetNextSibling(next); ////next.GetNextSibling();
            }

            if (next != null)
            {
                if (ElementHelper.GetTreeEntries(next, true) != null)
                {
                    return ElementHelper.GetNextStepIn(next); ////next.GetNextStepIn();
                }
                else
                {
                    return next;
                }
            }

            Element parent = el.ParentElement;
            while (parent != null)
            {
                next = ElementHelper.GetNextSibling(parent); ////parent.GetNextSibling();
                if (next != null)
                {
                    if (ElementHelper.GetTreeEntries(next, true) != null)
                    {
                        return ElementHelper.GetNextStepIn(next); ////next.GetNextStepIn();
                    }
                    else
                    {
                        return next;
                    }
                }

                parent = parent.ParentElement;
            }

            return null;
        }
    }

    class CacheTreeEntry : TreeTableEntry
    {
        public CacheTreeEntry(CacheState cs, TreeTable tree)
        {
            this.cs = cs;
            cs.cacheTreeEntry = this;
            this.Tree = tree;
        }

        public override object GetSortKey()
        {
            return cs.cachedAdjustedTableIndex;
        }

        protected override void Dispose(bool disposing)
        {
            cs = null;
            base.Dispose(disposing);
        }

        internal CacheState cs;
    }

    class CacheElementIdTreeEntry : TreeTableEntry
    {
        public CacheElementIdTreeEntry(CacheState cs, TreeTable tree)
        {
            this.cs = cs;
            cs.cacheElementIdTreeEntry = this;
            this.Tree = tree;
        }

        public override object GetSortKey()
        {
            return cs.cachedElement.Id;
        }

        protected override void Dispose(bool disposing)
        {
            cs = null;
            base.Dispose(disposing);
        }

        internal CacheState cs;
    }

    class CacheState : IDisposable
    {
        internal int cachedAdjustedTableIndex;
        internal int cachedElementCount;
        internal Element cachedElement;
        internal CacheTreeEntry cacheTreeEntry;
        internal CacheElementIdTreeEntry cacheElementIdTreeEntry;
        internal int cacheIndex;

        public void Detach()
        {
            if (cacheTreeEntry != null)
            {
                cacheTreeEntry.Tree.Remove(cacheTreeEntry);
            }

            if (cacheElementIdTreeEntry != null)
            {
                cacheElementIdTreeEntry.Tree.Remove(cacheElementIdTreeEntry);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            cachedElement = null;
            cacheTreeEntry = null;
            cacheElementIdTreeEntry = null;
        }

        #endregion
    }

    class YamountCacheTreeEntry : TreeTableEntry
    {
        public YamountCacheTreeEntry(YamountCacheState cs, TreeTable tree)
        {
            this.cs = cs;
            cs.cacheTreeEntry = this;
            this.Tree = tree;
        }

        public override object GetSortKey()
        {
            return cs.cachedYamountPosition;
        }

        protected override void Dispose(bool disposing)
        {
            cs = null;
            base.Dispose(disposing);
        }

        internal YamountCacheState cs;
    }

    class YamountCacheElementIdTreeEntry : TreeTableEntry
    {
        public YamountCacheElementIdTreeEntry(YamountCacheState cs, TreeTable tree)
        {
            this.cs = cs;
            cs.cacheElementIdTreeEntry = this;
            this.Tree = tree;
        }

        public override object GetSortKey()
        {
            return cs.cachedElement.Id;
        }

        protected override void Dispose(bool disposing)
        {
            cs = null;
            base.Dispose(disposing);
        }

        internal YamountCacheState cs;
    }

    class YamountCacheState : IDisposable
    {
        internal double cachedYamountPosition;
        internal double cachedYamountCount;
        internal Element cachedElement;
        internal YamountCacheTreeEntry cacheTreeEntry;
        internal YamountCacheElementIdTreeEntry cacheElementIdTreeEntry;
        internal int cacheIndex;

        public void Detach()
        {
            if (cacheTreeEntry != null)
            {
                cacheTreeEntry.Tree.Remove(cacheTreeEntry);
            }

            if (cacheElementIdTreeEntry != null)
            {
                cacheElementIdTreeEntry.Tree.Remove(cacheElementIdTreeEntry);
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            cachedElement = null;
            cacheTreeEntry = null;
            cacheElementIdTreeEntry = null;
        }
        #endregion
    }
}
