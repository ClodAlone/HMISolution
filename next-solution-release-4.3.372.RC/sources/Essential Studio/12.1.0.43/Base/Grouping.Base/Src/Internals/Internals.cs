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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping.Internals
{

	interface IDisplayElement
	{
	}

	interface IContainerElement : IElementTreeTableSource, IElement
	{
		bool ShouldStepIntoElements();
	}

	internal class RecordDataComparer : IComparer
	{
		IComparer _inner;

		public RecordDataComparer(IComparer inner)
		{
			_inner = inner;
		}

		public int Compare(object x, object y)
		{
			Record ry = (Record) y;
			if (x is Record)
			{
				Record rx = (Record) x;

				int cmp = 0;
				if (rx.ParentTable.IsSorted)
					cmp = _inner.Compare(rx, ry);
				if (cmp != 0)
					return cmp;

				return rx.UnsortedEntry.GetPosition()-ry.UnsortedEntry.GetPosition();
			}
			else // This branch is used when called from RecordsInDetailsCollection.FindRecord.
			{
				object[] ax = (object[]) x;
				int n = 0;
				foreach (SortColumnDescriptor columnDescriptor in ry.ParentTableDescriptor.SortedColumns)
				{
					if (n >= ax.Length)
						break;
					object cx = Convert.ChangeType(ax[n], columnDescriptor.FieldDescriptor.GetPropertyType());
					object cy = ry.GetValue(columnDescriptor);
					//Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(columnDescriptor.Name, cx, cy);
					int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);
					if (cmp != 0)
						return cmp;
					n++;
				}
				return 0;				
			}
		}
	}

//	internal interface IGroupByCategorizer
//	{
//		object GetGroupByCategory(Group groupByNode, Record record);
//		int CompareCategory(Group groupByNode, Record record);
//	}

	internal interface IGroupByCategorizer
	{
		object GetGroupByCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, Record record);
		int CompareCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, object category, Record record);
	}

	internal class GroupedColumnCategorizer : IGroupByCategorizer
	{
		SortColumnDescriptorCollection groupedColumns;

		public GroupedColumnCategorizer(SortColumnDescriptorCollection groupedColumns)
		{
			this.groupedColumns = groupedColumns;
		}

		public object GetGroupByCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey,  Record record)
		{
			//SortColumnDescriptor columnDescriptor = groupedColumns[groupByNode.GroupLevel];
			if (columnDescriptor.Categorizer != null)
				return columnDescriptor.Categorizer.GetGroupByCategoryKey(columnDescriptor, isForeignKey, record);

			return record.GetValue(columnDescriptor);
		}

		public int CompareCategoryKey(SortColumnDescriptor columnDescriptor, bool isForeignKey, object category, Record record)
		{
			//SortColumnDescriptor columnDescriptor = groupedColumns[groupByNode.GroupLevel];
			if (columnDescriptor.Categorizer != null)
				return columnDescriptor.Categorizer.CompareCategoryKey(columnDescriptor, isForeignKey, category, record);

			//object x = groupByNode.Category;
			object x = category;
			object y = record.GetValue(columnDescriptor);

			return SortColumnComparer._Compare(columnDescriptor, x, y);
		}
	}

	internal class GroupedColumnsComparer : IComparer
	{
		SortColumnDescriptorCollection columnDescriptors;

		public GroupedColumnsComparer(SortColumnDescriptorCollection columnDescriptors)
		{
			this.columnDescriptors = columnDescriptors;
		}

		public int Compare(object x, object y)
		{
			for (int n = 0; n < columnDescriptors.Count; n++)
			{
				SortColumnDescriptor columnDescriptor = columnDescriptors[n];
				object cx = ((object[]) x)[n];
				object cy = ((object[]) y)[n];

				int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

				if (cmp != 0)
					return cmp;
			}
			return 0;
		}
	}

	internal class SortColumnComparer : IComparer
	{
		TableDescriptor table;

		public SortColumnComparer(TableDescriptor table)
		{
			this.table = table;
		}

		public int Compare(object x, object y)
		{
			int ret = _Compare(table.RelationChildColumns, x, y);
			if (ret == 0)
				ret = _Compare(table.GroupedColumns, x, y);
			if (ret == 0)
				ret = _Compare(table.SortedColumns, x, y);
			return ret;
		}

		internal static int _Compare(SortColumnDescriptorCollection columns, object x, object y)
		{
			foreach (SortColumnDescriptor columnDescriptor in columns)
			{
				object cx, cy;
				if (x is Record)
				{
					cx = ((Record) x).GetValue(columnDescriptor);
					cy = ((Record) y).GetValue(columnDescriptor);
				}
				else 
				{
					FieldDescriptor pd = columnDescriptor.FieldDescriptor;
					if (pd != null)
					{
						cx = pd.GetValue((Record) x);
						cy = pd.GetValue((Record) y);
					}
					else
					{
						throw new NotImplementedException();
						////object cx = ((object[]) x)[n];
						////object cy = ((object[]) y)[n];
						//cx = x;
						//cy = y;
					}
				}

				int cmp = SortColumnComparer._Compare(columnDescriptor, cx, cy);

				if (cmp != 0)
					return cmp;
			}
			return 0;
		}

		internal static int _Compare(SortColumnDescriptor columnDescriptor, object x, object y)
		{
			int cmp;
			if (columnDescriptor.Comparer != null)
				cmp = columnDescriptor.Comparer.Compare(x, y);
			else
				cmp = _Compare(x, y);
			if (columnDescriptor.SortDirection == ListSortDirection.Descending)
				return -cmp;
			return cmp;
		}

		internal static int _Compare(object x, object y)
		{
			int cmp;
			bool xIsNull = (x == null || x is DBNull);
			bool yIsNull = (y == null || y is DBNull);

			if (yIsNull && xIsNull)
				cmp = 0;
			else if (xIsNull)
				cmp = -1;
			else if (yIsNull)
				cmp = 1;
			else 
				cmp = ((IComparable) x).CompareTo(y);

			return cmp;
		}

	}

	internal class ElementHelper
	{
		public static ElementTreeTable GetTreeEntries(Element el)
		{
			if (el is IElementTreeTableSource)
				return ((IElementTreeTableSource) el).GetChildElementTreeTable();
			return null; 
		}

		public static Element GetElementAtCounterPosition(Element el, Counter counter)
		{
			ElementTreeTableEntry entry = GetEntryAtCounterPosition(el, counter);
			if (entry != null)
				return entry.Element;
			return null;
		}

		public static ElementTreeTableEntry GetEntryAtCounterPosition(Element el, Counter counter)
		{
			IElementTreeTableSource cs = el as IElementTreeTableSource;
			if (cs != null)
				return GetEntryAtCounterPosition(cs.GetChildElementTreeTable(), counter);
			return null;
		}

		public static ElementTreeTableEntry GetEntryAtCounterPosition(ElementTreeTable table, Counter counter)
		{
			return table.GetEntryAtCounterPosition(counter);
		}

		public static int GetInnerPosition(Element el, int cookie)
		{
			ElementTreeTableEntry entry = el.GetElementEntry();
			if (entry != null && entry.GetCounterPosition() != null)
				return (int) entry.GetCounterPosition().GetValue(cookie);
			return 0;
		}

		public static int GetParentPosition(Element el, int cookie)
		{
			Element parent = el.ParentElement;
			if (parent != null)
				return (int) GetCumulatedPosition(parent, cookie);
			return 0;
		}

		public static int GetCumulatedPosition(Element el, int cookie)
		{
			int n1 = GetInnerPosition(el, cookie);
			int n2 = GetParentPosition(el, cookie);
			return n1 + n2;
			//return GetInnerPosition(el, cookie) + GetParentPosition(el, cookie);
		}


		public static Element FindElement(Table _table, Counter index)
		{
			//Element el1 = _FindElement(_table, index);
			Element el2 = __FindElement(_table, index, null, false);
			//System.Diagnostics.Debug.Assert(el1 == el2);
			return el2;
		}

		public static Element FindElement(Table _table, Counter index, bool stepInNestedTables)
		{
			//Element el1 = _FindElement(_table, index);
			Element el2 = __FindElement(_table, index, null, stepInNestedTables);
			//System.Diagnostics.Debug.Assert(el1 == el2);
			return el2;
		}

		public static Element __FindElement(Table _table, Counter index, Type t, bool stepInNestedTables)
		{
			//GroupsDetails gd;
			Element g = _table.TopLevelGroup;
			int findPos = (int) index.GetValue(index.Kind);
//			if (_table.RecordPartEntry != null)
//				findPos += _table.RecordPartEntry.CounterPosition.GetValue(index.Kind);
			while (g != null)
			{
				int pos = GetInnerPosition(g, index.Kind);
				//int pos = g.VisiblePositionInSection;
				// Groups is only an abstract element - only the caption, its records etc are visible elements.

				findPos -= pos;
				//Section s = g.VisibleSections[findPos];
				//Section s = (Section) FindElement(g, Counter.CreateCounter(findPos, index.Kind));
				g.EnsureInitialized(g);
				Element s = GetElementAtCounterPosition(g, Counter.CreateCounter(findPos, index.Kind));
				if (s == null)
					return null;

				int count = (int) s.GetCounter().GetValue(index.Kind);
				if (count == 0)
					return g;

				if (s.TreeEntries == null)
					return s;

				//pos = s.VisiblePositionInSection;
				pos = GetInnerPosition(s, index.Kind);
				// DetailsSection is an abstract element - only its members are visible.
				// Caption, footer, header are considered visible elements.
				if (pos == findPos && t != null && t.IsAssignableFrom(s.GetType()))  // !(s is DetailsSection))
					return s;

				if (pos >= findPos && !(s is IContainerElement))  // !(s is DetailsSection))
					return s;

				findPos -= pos;
				s.EnsureInitialized(s);
				Element el = GetElementAtCounterPosition(s, Counter.CreateCounter(findPos, index.Kind));
				if (t != null && t.IsAssignableFrom(el.GetType()))  // !(s is DetailsSection))
					return el;
				else if (
					(el is NestedTable && stepInNestedTables)
					|| (el is IContainerElement && ((IContainerElement) el).ShouldStepIntoElements())
				   )
					g = (Element) el;
				else
					return el;
			}
			return null;
		}

		public static Element GetNextSiblingElement(Element el, int cookie)
		{ 
			ElementTreeTableEntry current = el.GetElementEntry();
			if (current == null)
				return null;

			ElementTreeTable tree = GetTreeEntries(el.ParentElement);
			if (tree == null)
				return null;

			ElementTreeTableEntry next = tree.GetNextCounterEntry(current, cookie);
			if (next == null)
				return null;
				
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
			
			ElementTreeTable treeEntries = GetTreeEntries(el);
			
			if (treeEntries != null 
				&& (finalType == null || !finalType.IsAssignableFrom(el.GetType()))
				&& (el is NestedTable && stepInNestedTables || !(el is IContainerElement) || ((IContainerElement) el).ShouldStepIntoElements())
				&& (treeEntries.GetCounterTotal().GetValue(cookie) > 0)
				)
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

			if (next != null)
			{
				next.EnsureInitialized(next);
				if (!
					( (next is NestedTable && stepInNestedTables) 
						|| next is IContainerElement && ((IContainerElement) next).ShouldStepIntoElements() )
					)
					return next;

				if ((finalType == null || !finalType.IsAssignableFrom(el.GetType()))
					&& next is IElementTreeTableSource)
				{
					treeEntries = GetTreeEntries(next);
					if (treeEntries == null || treeEntries.GetCounterTotal().GetValue(cookie) == 0)
						return next;
					return ElementHelper.GetNextElementStepIn(next, cookie, finalType, stepInNestedTables, outerTable); 
				}
				else
					return next;
			}

			Element parent = GetParentElement(el, stepInNestedTables, outerTable);
			while (parent != null)
			{
				next = ElementHelper.GetNextSiblingElement(parent, cookie);
				if (next != null)
				{
					if (next is IElementTreeTableSource)
						return ElementHelper.GetNextElementStepIn(next, cookie, finalType, stepInNestedTables, outerTable);
					else
						return next;
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
					el = ((ChildTable) el).ParentNestedTable;
			}

			if (el != null)
				el = el.ParentElement;
	
				// Skip also child table if it was parent ... otherwise we get the sibling in the related table.
			if (stepInNestedTables)
			{
				if (el is ChildTable && !Object.ReferenceEquals(el.ParentTable, outerTable))
					el = ((ChildTable) el).ParentNestedTable;
			}

			return el;
		}


		public static Element GetNextSibling(Element el) 
		{ 
			ITreeTableEntry current = el.GetElementEntry() as ITreeTableEntry;
			if (current == null)
				return null;

			ITreeTable tree = el.GetElementEntryTable();
			if (tree == null)
				return null;

			// ElementTreeTableEntry next = ((ElementTreeTable) tree).GetNextCounterEntry((ElementTreeTableEntry) current, CounterKind.ElementsCount) as ElementTreeTableEntry;
			ElementTreeTableEntry next = tree.GetNextEntry(current) as ElementTreeTableEntry;
			if (next == null)
				return null;
				
			return next.Element;
		}

		public static Element GetNextStepIn(Element el)
		{ 
			Element next = el;
			ElementTreeTable treeEntries = next.TreeEntries;
			if (treeEntries != null && treeEntries.Count > 0)
			{
				//ElementTreeTableEntry entry = (ElementTreeTableEntry) treeEntries.GetEntryAtCounterPosition(counter, CounterKind.ElementsCount);
				ElementTreeTableEntry entry = (ElementTreeTableEntry) treeEntries[0];
				if (entry != null)
				{
					next = entry.Element;
				}
			}
			else
			{
				next = ElementHelper.GetNextSibling(next);//next.GetNextSibling();
			}

			if (next != null)
			{
				if (next.TreeEntries != null)
					return ElementHelper.GetNextStepIn(next); //next.GetNextStepIn();
				else
					return next;
			}

			Element parent = el.ParentElement;
			while (parent != null)
			{
				next = ElementHelper.GetNextSibling(parent);//parent.GetNextSibling();
				if (next != null)
				{
					if (next.TreeEntries != null)
						return ElementHelper.GetNextStepIn(next); //next.GetNextStepIn();
					else
						return next;
				}
				parent = parent.ParentElement;
			}
			return null;
		}


	}


}