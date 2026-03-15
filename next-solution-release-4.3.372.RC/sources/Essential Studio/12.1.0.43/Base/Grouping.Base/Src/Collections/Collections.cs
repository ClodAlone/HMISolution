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
using System.Diagnostics;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping
{
#if false
	#region DisplayElements
	/// <summary>
	/// A Read-only list of visible groups within a details section (A group can be hidden if 
	/// a filter is selected and no records fit its criteria).
	/// </summary>
	public class DisplayElementsInTableCollection : IList
	{
		internal Table _table;
		internal Element cachedElement;
		internal int cachedAdjustedTableIndex;
		internal int version;
		int cachedElementCount;
		internal ChildTable _childTable;
		internal bool _stepInNestedTables;
		internal ChildTable cachedFilteredChildTable;
		internal int cachedFilteredChildTablePos;

		internal DisplayElementsInTableCollection(Table table)
		{
			_table = table;
			_childTable = null;
		}

		internal DisplayElementsInTableCollection(Table table, bool stepInNestedTables)
		{
			_table = table;
			_childTable = null;
			_stepInNestedTables = stepInNestedTables;
		}

		internal DisplayElementsInTableCollection(ChildTable childTable)
		{
			_table = childTable.ParentTable;
			_childTable = childTable;
		}

		internal DetailsSection GetRootDetails()
		{
			return _table.TopLevelGroup.Details;
		}

		int GetFilterChildTableDisplayPosition(ChildTable childTable)
		{
			if (cachedFilteredChildTable != null && version == _table.version && cachedFilteredChildTable == childTable)
				return this.cachedFilteredChildTablePos;

			cachedFilteredChildTable = childTable;
			cachedFilteredChildTablePos = childTable.GetDisplayPosition();
			return cachedFilteredChildTablePos;
		}

		public static bool trace = false;

		public Element this[int index]
		{
			get
			{
				int count = Count;
				if (index < 0 || index >= count)
					throw new ArgumentOutOfRangeException();

				//bool trace = false;//index > 42 && _table.FilteredChildTable  != null && this._childTable == null;

				bool allowCache = false;

				int adjustedTableIndex = index;
				if (_childTable != null)
					adjustedTableIndex += GetFilterChildTableDisplayPosition(_childTable);
				else if (_table.FilteredChildTable != null)
					adjustedTableIndex += GetFilterChildTableDisplayPosition(_table.FilteredChildTable);

				if (trace)
					Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(index, adjustedTableIndex);

				if (allowCache && cachedElement != null && version == _table.version)
				{
					if (trace)
						Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(cachedAdjustedTableIndex, cachedElement, cachedElementCount, _table.FilteredChildTable);

					if (adjustedTableIndex == cachedAdjustedTableIndex)
					{
						if (trace)
							Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("Same", index, adjustedTableIndex);
						return cachedElement;
					}
					else  
					{
						// Is this a child element of a nested table? (see discussion below)
						if (adjustedTableIndex > cachedAdjustedTableIndex && adjustedTableIndex < cachedAdjustedTableIndex+cachedElementCount)
						{
							if (trace)
								Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("Same nested", index, adjustedTableIndex);
							return cachedElement;
						}
						// Is this the subsequent element? 
						else if (/*_table.FilteredChildTable == null && */adjustedTableIndex == cachedAdjustedTableIndex+cachedElementCount)
						{
							// TODO: caching gets messed up when _table.FilteredChildTable is not null. GetNextElementStepIn
							// will step out of table, e.g. if filter is set on products table, GetNextElementStepIn might
							// setep into categories parent table.

							cachedAdjustedTableIndex += cachedElementCount;
							Element elx = cachedElement;
							cachedElement = ElementHelper.GetNextElementStepIn(cachedElement, CounterKind.DisplayElementCount, null, this._stepInNestedTables, this._table);

							if (this._stepInNestedTables)
							{
								cachedElementCount = 1;
								//if (cachedElement != null && cachedElement.GetVisibleCount() != 1)
								//	Debugger.Break();
							}
							else
							{
								// No need to call cachedElement.GetDisplayPosition();
								// cachedAdjustedTableIndex is already pointing to the first display element of the nested table.
								if (cachedElement != null)
									cachedElementCount = cachedElement.GetVisibleCount();
								Debug.Assert(cachedElement.ParentTable == _table);
							}

#if later
							if (false)
							{
								int testIndexOf = IndexOf(cachedElement);

								if (testIndexOf == -1 || testIndexOf != index)
								{
									testIndexOf = IndexOf(cachedElement);
									cachedElement = ElementHelper.GetNextElementStepIn(elx, CounterKind.DisplayElementCount, null, this._stepInNestedTables, this._table);
								}

								int v = version;
								this.version = -1;
								//int testDisplayPosition = cachedElement.GetDisplayPosition();
								testIndexOf = IndexOf(cachedElement);

								if (testIndexOf == -1 || testIndexOf != index)
								{
									testIndexOf = IndexOf(cachedElement);
									cachedElement = ElementHelper.GetNextElementStepIn(elx, CounterKind.DisplayElementCount, null, this._stepInNestedTables, this._table);
								}


								if (trace)
								{
									Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("next", index, adjustedTableIndex, testIndexOf);
									//if (testDisplayPosition != cachedAdjustedTableIndex)
									//	Debugger.Break();
									if (testIndexOf != index)
										Debugger.Break();
								}
								this.version = v;
							}
#endif

							return cachedElement;
						}
					}
				}

				// Look up the element and cache it together with adjusted table index.
				Element result = ElementHelper.FindElement(_table, Counter.CreateDisplayElementCounter(adjustedTableIndex), _stepInNestedTables);
#if caching
				if (false)
				{
					if (cachedElement != null)
					{
						//lastCount = count;
						//lastFilteredChildTable = _table.FilteredChildTable;
						if (_stepInNestedTables)
						{
							cachedAdjustedTableIndex = adjustedTableIndex; 
							cachedElementCount = 1;
						}
						else if (cachedElement is NestedTable)
						{
							cachedElementCount = cachedElement.GetVisibleCount();
							if (cachedElementCount > 1)
							{	//cachedAdjustedTableIndex = ((NestedTable) cachedElement).ChildTable.GetDisplayPosition();//adjustedTableIndex - ((NestedTable) cachedElement).ChildTable.DisplayElements.IndexOf(cachedElement);
								cachedAdjustedTableIndex = IndexOfHelper(cachedElement, cachedElement.GetDisplayPosition());//adjustedTableIndex;// - ((NestedTable) cachedElement).ChildTable.DisplayElements.IndexOf(cachedElement);
							}
							else
								cachedAdjustedTableIndex = adjustedTableIndex;

							Debug.Assert(cachedElement.ParentTable == _table);
						}
						else
						{
							cachedAdjustedTableIndex = adjustedTableIndex; 
							cachedElementCount = 1;
						}

					if (false)
						{
							int testIndexOf = IndexOf(cachedElement);

							if (testIndexOf == -1 || index < testIndexOf || index >= testIndexOf + cachedElementCount)
							{
								testIndexOf = IndexOf(cachedElement);
								Debugger.Break();
							}

							int v = version;
							this.version = -1;
							//int testDisplayPosition = cachedElement.GetDisplayPosition();
							testIndexOf = IndexOf(cachedElement);

							if (testIndexOf == -1 || index < testIndexOf || index >= testIndexOf + cachedElementCount)
							{
								Debugger.Break();
							}


							if (trace)
							{
								Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("next", index, adjustedTableIndex, testIndexOf);
								//if (testDisplayPosition != cachedAdjustedTableIndex)
								//	Debugger.Break();
								if (testIndexOf != index)
									Debugger.Break();

							}
							testIndexOf = IndexOf(cachedElement);

							this.version = v;
						}

					}
				}
#endif
				version = _table.version;
				return result;
			}
			set
			{
				throw new InvalidOperationException("Collection is read only");
			}
		}

		/// <summary>
		/// Checks if the group belongs to the details section and is visible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(Element value)
		{
			if (value == null)
				return false;

			value.EnsureInitialized(this, true);

			// Check if same child table and all parent objects are expanded.
			if (_stepInNestedTables)
				return CheckVisible(value);
			else if (_childTable != null)
			{
				if (value == _childTable)
					return true;

				if (value.ParentChildTable == _childTable)
				{
					return CheckVisible(value);
				}
			}
			else if (value == _table.FilteredChildTable)
				return true;
			else if (_table.FilteredChildTable == null
				|| value.ParentChildTable == _table.FilteredChildTable)
			{
				if (value.ParentTable == _table)
				{
					return CheckVisible(value);
				}
			}

			return false;
			
		}

		bool CheckVisible(Element value)
		{
			return value.GetVisibleInHierarchy();

//			Element el = value;
//			if (value is RecordRow)
//				el = value.ParentRecord.ParentElement;
//			else if (value is CaptionSection)
//			{
//				el = value.ParentGroup;
//			}
//			else if (value is EmptySection)
//			{
//				return value.IsVisible;
//			}
//
//			if (el is ChildTable)
//			{
//				if (this._childTable != null)
//				{
//					//if (el.ParentTable == _childTable.ParentTable)
//						return el == this._childTable;
//				}
//				else if (_table.FilteredChildTable != null)
//				{
//					//if (el.ParentTable == _table.FilteredChildTable.ParentTable)
//						return el == _table.FilteredChildTable;
//				}
//			}
//			
//			if (el is Record || el is Group)
//				el = el.ParentElement;
//
//			while (el != null)
//			{
//				if (!_stepInNestedTables)
//				{
//					if (_childTable != null)
//					{
//						if (el == _childTable)
//							return true;
//					}
//					else if (_table.FilteredChildTable != null)
//					{
//						if (el == _table.FilteredChildTable)
//							return true;
//					}
//				}
//				if (el is RecordRow)
//					el = el.ParentRecord;
//
//				else if (el is CaptionSection)
//					el = el.ParentElement;
//
//				else if (el is Group && !((Group) el).IsExpanded)
//					return false;
//
//				else if (el is Record && !((Record) el).IsExpanded)
//					return false;
//
//				if (el != null)
//					el = el.ParentElement;
//			}
//
//			return true;
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

		/// <summary>
		/// Gets the visible position within the details section.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(Element value)
		{
			// Check if I can reuse object from last this[index] operation.
			int index;
			if (cachedElement != null && version == _table.version && cachedElement == value && this.GetFilterChildTable() == null)
			{
				index = cachedAdjustedTableIndex;

				//if (_childTable != null)
				//	index -= GetFilterChildTableDisplayPosition(_childTable);
				//else if (_table.FilteredChildTable != null)
				//	index -= GetFilterChildTableDisplayPosition(_table.FilteredChildTable);

				//return index;

			}
			else
			{
				if (!Contains(value))
					return -1;
				index = value.GetDisplayPosition();
			}
			return IndexOfHelper(value, index);
		}


		int IndexOfHelper(Element value, int index)
		{

			if (!_stepInNestedTables)
			{
				// GetDisplayPosition is relative to child table.
				if (GetFilterChildTable() != null)
					index -= GetFilterChildTable().GetDisplayPosition();
			}
			else
			{
				// GetDisplayPosition is relative to child table.
				// The child table is referenced by the NestedTable.ChildTable property which gives the 
				// display position in the parent table.

				ChildTable childTable = value.ParentChildTable;
				while (childTable != null)
				{
					index -= childTable.GetDisplayPosition();

					if (childTable == GetFilterChildTable())
						break;

					NestedTable nestedTable = childTable.ParentNestedTable;
					if (nestedTable != null)
					{
						index += nestedTable.GetDisplayPosition();
						childTable = nestedTable.ParentChildTable;
					}
					else
						break;
				}
			}

			if (index < 0)
			{
				//Diagnostics.IterateThroughDisplayElement(this._table);
				int i = value.GetDisplayPosition();
				index = 0;
				throw new InvalidOperationException();
			}

//			if (_childTable != null)
//				index -= GetFilterChildTableDisplayPosition(_childTable);
//			else if (_table.FilteredChildTable != null)
//				index -= GetFilterChildTableDisplayPosition(_table.FilteredChildTable);

//			if (index > 4000)
//			{
//				Debugger.Break();
//				return IndexOf(value);
//			}
			return index;
		}

		public Element GetItemAtYAmount(double index)
		{
			int count = Count;
			double index2 = index;
			if (_childTable != null)
				index2 += _childTable.GetYAmountPosition();
			else if (_table.FilteredChildTable != null)
				index2 += _table.FilteredChildTable.GetYAmountPosition();

			index2 = Math.Min(index2, _table.GetYAmountCount());
			return ElementHelper.FindElement(_table, Counter.CreateYAmountCounter(index2), _stepInNestedTables);
		}

		public double GetYAmountPositionOf(Element value)
		{
//			if (value is NestedTable)
//			{
//				if (this._stepInNestedTables)
//				{
//					value.EnsureInitialized(this, false);
//					if (value.GetVisibleCount() == 0)
//						return -1;
//					else
//						value = ((NestedTable) value).ChildTable.DisplayElements[0];
//				}
//			}
//			else if (value is IContainerElement && ((IContainerElement) value).ShouldStepIntoElements())
//				value = ElementHelper.GetNextElementStepIn(value, CounterKind.DisplayElementCount, null, _stepInNestedTables);

			if (!Contains(value))
				return -1;

			double index = value.GetYAmountPosition();
			if (!_stepInNestedTables)
			{
				if (_childTable != null)
					index -= _childTable.GetYAmountPosition();
				else if (_table.FilteredChildTable != null)
					index -= _table.FilteredChildTable.GetYAmountPosition();
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
						break;
				}
			}

			return index;
		}


		public void CopyTo(Element[] array, int index)
		{
			int n = 0;
			foreach (Element element in this)
			{
				array[index+n] = element;
				n++;
			}
		}

		//		public DisplayElementsInTableCollection SyncRoot
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		public DisplayElementsInTableCollectionEnumerator GetEnumerator()
		{
			return new DisplayElementsInTableCollectionEnumerator(this);
		}
		
		#region IList Members

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

		public void RemoveAt(int index)
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
			return Contains((Element) value);
		}

		public void Clear()
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((Element) value);
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				if (_table == null)
					return 0;
				_table.EnsureInitialized(this);
				if (_childTable != null)
					return _childTable.GetVisibleCount();
				else if (_table.FilteredChildTable != null)
					return _table.FilteredChildTable.GetVisibleCount();
				return _table.GetVisibleCount();
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((Element[]) array, index);
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
	/// Enumerates through visible groups within a details section.
	/// </summary>
	public class DisplayElementsInTableCollectionEnumerator : IEnumerator 
	{
		Element _cursor, _next;
		DisplayElementsInTableCollection _coll;
		ChildTable filteredChildTable;
		int skipElements = 0;
		int nextIndex = -1;  // just for debugging purpose ...

		public DisplayElementsInTableCollectionEnumerator(DisplayElementsInTableCollection collection)
		{
			_coll = collection;
			Reset();
		}

		#region IEnumerator Members

		public virtual void Reset()
		{
			filteredChildTable = _coll._childTable;
			if (filteredChildTable == null)
				filteredChildTable = _coll._table.FilteredChildTable;
			_cursor = null;
			if (_coll.Count > 0)
			{
				_next = _coll[0];
				nextIndex = 0;
				if (!_coll._stepInNestedTables)
					skipElements = _next.GetVisibleCount()-1;
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

		public Element Current
		{
			get
			{
				return _cursor;
			}
		}

		public bool MoveNext()
		{
			if (_next == null)
				return false;

			_cursor = _next;
			nextIndex++;

			if (_coll._stepInNestedTables || --skipElements < 0)
			{
				_next = ElementHelper.GetNextElementStepIn(_next, CounterKind.DisplayElementCount, null, _coll._stepInNestedTables, _coll._table);

				if (!_coll._stepInNestedTables)
					skipElements = _next != null ? _next.GetVisibleCount()-1 : 0;

				// don't step out/iterate into display elements of parents table.
				if (_next != null && _coll._stepInNestedTables)
				{
					if (filteredChildTable != null && _next.ParentChildTable != filteredChildTable)
						_next = null;
				}
			}

			return _cursor != null;
		}
		#endregion

	}
	#endregion
	#region GroupedElements
	public class ElementsInTableCollection : IList
	{
		internal Table _table;
		internal Element cachedElement;
		internal int cachedAdjustedTableIndex;
		internal int version;
		int cachedElementCount;
		internal ChildTable _childTable;
		internal bool _stepInNestedTables;
		internal ChildTable cachedFilteredChildTable;
		internal int cachedFilteredChildTablePos;

		internal ElementsInTableCollection(Table table)
		{
			_table = table;
			_childTable = null;
		}
		
		internal ElementsInTableCollection(Table table, bool stepInNestedTables)
		{
			_table = table;
			_childTable = null;
			_stepInNestedTables = stepInNestedTables;
		}

		internal ElementsInTableCollection(ChildTable childTable)
		{
			_table = childTable.ParentTable;
			_childTable = childTable;
		}

		internal DetailsSection GetRootDetails()
		{
			return _table.TopLevelGroup.Details;
		}

		int GetFilterChildTableElementPosition(ChildTable childTable)
		{
			if (cachedFilteredChildTable != null && version == _table.version && cachedFilteredChildTable == childTable)
				return this.cachedFilteredChildTablePos;

			cachedFilteredChildTable = childTable;
			cachedFilteredChildTablePos = childTable.GetElementPosition();
			return cachedFilteredChildTablePos;
		}

		public Element this[int index]
		{
			get
			{
				int count = Count;
				if (index < 0 || index >= count)
					throw new ArgumentOutOfRangeException();

				bool trace = false;//index > 42 && _table.FilteredChildTable  != null && this._childTable == null;

				int adjustedTableIndex = index;
				if (_childTable != null)
					adjustedTableIndex += GetFilterChildTableElementPosition(_childTable);
				else if (_table.FilteredChildTable != null)
					adjustedTableIndex += GetFilterChildTableElementPosition(_table.FilteredChildTable);

				if (trace)
					Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(index, adjustedTableIndex);

				if (cachedElement != null && version == _table.version)
				{
					if (trace)
						Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(cachedAdjustedTableIndex, cachedElement, cachedElementCount, _table.FilteredChildTable);

					if (adjustedTableIndex == cachedAdjustedTableIndex)
					{
						if (trace)
							Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("Same", index, adjustedTableIndex);
						return cachedElement;
					}
					else 
					{
						// Is this a child element of a nested table? (see discussion below)
						if (adjustedTableIndex > cachedAdjustedTableIndex && adjustedTableIndex < cachedAdjustedTableIndex+cachedElementCount)
						{
							if (trace)
								Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("Same nested", index, adjustedTableIndex);
							return cachedElement;
						}
							// Is this the subsequent element? 
						else if (_table.FilteredChildTable == null && adjustedTableIndex == cachedAdjustedTableIndex+cachedElementCount)
						{
							cachedAdjustedTableIndex += cachedElementCount;
							cachedElement = ElementHelper.GetNextElementStepIn(cachedElement, CounterKind.ElementsCount, null, this._stepInNestedTables, this._table);

							if (this._stepInNestedTables)
							{
								cachedElementCount = 1;
							}
							else
							{
								// No need to call cachedElement.GetElementPosition();
								// cachedAdjustedTableIndex is already pointing to the first Element element of the nested table.
								if (cachedElement != null)
									cachedElementCount = cachedElement.GetElementCount();
								Debug.Assert(cachedElement.ParentTable == _table);
							}

							if (trace)
							{
								int testElementPosition = cachedElement.GetElementPosition();
								int testIndexOf = IndexOf(cachedElement);

								Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("next", index, adjustedTableIndex, testElementPosition, testIndexOf);
								if (testElementPosition != cachedAdjustedTableIndex)
									Debugger.Break();
								if (testIndexOf != index)
									Debugger.Break();

							}

							return cachedElement;
						}
					}
				}

				// Look up the element and cache it together with adjusted table index.
				Element result = ElementHelper.FindElement(_table, Counter.CreateElementCounter(adjustedTableIndex), _stepInNestedTables);
				if (cachedElement != null)
				{
					//lastCount = count;
					//lastFilteredChildTable = _table.FilteredChildTable;
					if (_stepInNestedTables)
					{
						cachedAdjustedTableIndex = adjustedTableIndex; 
						cachedElementCount = 1;
					}
					else if (cachedElement is NestedTable)
					{
						cachedElementCount = cachedElement.GetElementCount();
						if (cachedElementCount > 1)
							//cachedAdjustedTableIndex = ((NestedTable) cachedElement).ChildTable.GetElementPosition();//adjustedTableIndex - ((NestedTable) cachedElement).ChildTable.ElementElements.IndexOf(cachedElement);
							cachedAdjustedTableIndex = cachedElement.GetElementPosition();//adjustedTableIndex - ((NestedTable) cachedElement).ChildTable.ElementElements.IndexOf(cachedElement);
						else
							cachedAdjustedTableIndex = adjustedTableIndex;

						Debug.Assert(cachedElement.ParentTable == _table);
					}
					else
					{
						cachedAdjustedTableIndex = adjustedTableIndex; 
						cachedElementCount = 1;
					}

					if (trace)
					{
						int testElementPosition = cachedElement.GetElementPosition();
						int testIndexOf = IndexOf(cachedElement);

						Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo("new lookup", index, adjustedTableIndex, testElementPosition, testIndexOf);
						if (testElementPosition != cachedAdjustedTableIndex)
							Debugger.Break();
						if (testIndexOf != index)
							Debugger.Break();

					}
				}
				version = _table.version;
				return result;
			}
			set
			{
				throw new InvalidOperationException("Collection is read only");
			}
		}

		/// <summary>
		/// Checks if the group belongs to the details section and is Element.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(Element value)
		{
			if (value == null)
				return false;

			value.EnsureInitialized(this, true);

			// Check if same child table and all parent objects are expanded.
			if (_stepInNestedTables)
				return CheckElement(value);
			else if (_childTable != null)
			{
				if (value == _childTable)
					return true;

				if (value.ParentChildTable == _childTable)
				{
					return CheckElement(value);
				}
			}
			else if (value == _table.FilteredChildTable)
				return true;
			else if (_table.FilteredChildTable == null
				|| value.ParentChildTable == _table.FilteredChildTable)
			{
				if (value.ParentTable == _table)
				{
					return CheckElement(value);
				}
			}

			return false;
			
		}

		bool CheckElement(Element value)
		{
			Element el = value;
			if (value is RecordRow)
				el = value.ParentRecord.ParentElement;
			else if (value is CaptionSection)
				return CheckElement(value.ParentGroup);
			else if (value is EmptySection)
			{
				return true;
			}

			if (el is ChildTable)
			{
				if (this._childTable != null)
					return el == this._childTable;
				else if (_table.FilteredChildTable != null)
					return el == _table.FilteredChildTable;
			}
			else if (el is Record || el is Group)
				el = el.ParentElement;

			while (el != null)
			{
				if (!_stepInNestedTables)
				{
					if (_childTable != null)
					{
						if (el == _childTable)
							return true;
					}
					else if (_table.FilteredChildTable != null)
					{
						if (el == _table.FilteredChildTable)
							return true;
					}
				}
				if (el is RecordRow)
					el = el.ParentRecord;

				else if (el is CaptionSection)
					el = el.ParentElement;

				if (el != null)
					el = el.ParentElement;
			}

			return true;
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

		/// <summary>
		/// Gets the Element position within the details section.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(Element value)
		{
			// Check if I can reuse object from last this[index] operation.
			int index;
			if (cachedElement != null && version == _table.version && cachedElement == value)
			{
				index = cachedAdjustedTableIndex;
			}
			else
			{
				if (!Contains(value))
					return -1;
				index = value.GetElementPosition();
			}
			return IndexOfHelper(value, index);
		}


		int IndexOfHelper(Element value, int index)
		{
			if (!_stepInNestedTables)
			{
				// GetElementPosition is relative to child table.
				if (GetFilterChildTable() != null)
					index -= GetFilterChildTable().GetElementPosition();
			}
			else
			{
				// GetElementPosition is relative to child table.
				// The child table is referenced by the NestedTable.ChildTable property which gives the 
				// Element position in the parent table.

				ChildTable childTable = value.ParentChildTable;
				while (childTable != null)
				{
					index -= childTable.GetElementPosition();

					if (childTable == GetFilterChildTable())
						break;

					NestedTable nestedTable = childTable.ParentNestedTable;
					if (nestedTable != null)
					{
						index += nestedTable.GetElementPosition();
						childTable = nestedTable.ParentChildTable;
					}
					else
						break;
				}
			}

			if (index < 0)
			{
				//Diagnostics.IterateThroughElementElement(this._table);
				int i = value.GetElementPosition();
				index = 0;
				throw new InvalidOperationException();
			}

			return index;
		}

		public void CopyTo(Element[] array, int index)
		{
			int n = 0;
			foreach (Element element in this)
			{
				array[index+n] = element;
				n++;
			}
		}

		//		public ElementsInTableCollection SyncRoot
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		public ElementsInTableCollectionEnumerator GetEnumerator()
		{
			_table.EnsureInitialized(this);
			return new ElementsInTableCollectionEnumerator(this);
		}
		
		#region IList Members

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

		public void RemoveAt(int index)
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
			return Contains((Element) value);
		}

		public void Clear()
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((Element) value);
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				if (_table == null)
					return 0;
				_table.EnsureInitialized(this);
				if (_childTable != null)
					return _childTable.GetVisibleCount();
				else if (_table.FilteredChildTable != null)
					return _table.FilteredChildTable.GetElementCount();
				return _table.GetElementCount();
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((Element[]) array, index);
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
	/// Enumerates through visible groups within a details section.
	/// </summary>
	public class ElementsInTableCollectionEnumerator : IEnumerator 
	{
		Element _cursor, _next;
		ElementsInTableCollection _coll;
		ChildTable filteredChildTable;
		int skipElements = 0;
		int nextIndex = -1;  // just for debugging purpose ...

		public ElementsInTableCollectionEnumerator(ElementsInTableCollection collection)
		{
			_coll = collection;
			Reset();
		}

		#region IEnumerator Members

		public virtual void Reset()
		{
			_cursor = null;
			filteredChildTable = _coll._childTable;
			if (filteredChildTable == null)
				filteredChildTable = _coll._table.FilteredChildTable;
			if (_coll.Count > 0)
			{
				_next = _coll[0];
				nextIndex = 0;
				if (!_coll._stepInNestedTables)
					skipElements = _next.GetElementCount()-1;
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

		public Element Current
		{
			get
			{
				return _cursor;
			}
		}

		public bool MoveNext()
		{
			if (_next == null)
				return false;

			_cursor = _next;
			nextIndex++;

			if (_coll._stepInNestedTables || --skipElements < 0)
			{
				_next = ElementHelper.GetNextElementStepIn(_next, CounterKind.ElementsCount, null, _coll._stepInNestedTables, _coll._table);
				
				if (!_coll._stepInNestedTables)
					skipElements = _next != null ? _next.GetElementCount()-1 : 0;

				// don't step out/iterate into display elements of parents table.
				if (_next != null && _coll._stepInNestedTables)
				{
					if (filteredChildTable != null && _next.ParentChildTable != filteredChildTable)
						_next = null;
				}
			}			

			return _cursor != null;
		}
		#endregion

	}
	#endregion
#endif
#if false
	#region FilteredRecords
	/// <summary>
	/// A Read-only list of visible groups within a details section. (A group can be hidden if 
	/// a filter is selected and no records fit its criteria.)
	/// </summary>
	public class FilteredRecordsInTableCollection : IList
	{
		internal Table _table;
		internal Record lastElement;
		internal int lastIndex;
		internal int version;
		internal int lastCount;

		internal FilteredRecordsInTableCollection(Table table)
		{
			_table = table;
		}

		internal DetailsSection GetRootDetails()
		{
			return _table.TopLevelGroup.Details;
		}

		/// <summary>
		/// A group.
		/// </summary>
		public Record this[int index]
		{
			get
			{
				int count = Count;
				if (index < 0 || index >= count)
					throw new ArgumentOutOfRangeException();

				if (lastIndex != -1 && lastCount == count && version == _table.version)
				{
					if (index == lastIndex)
					{
						return lastElement;
					}
					else if (index == lastIndex+1)
					{
						lastIndex++;
						lastElement = (Record) ElementHelper.GetNextElementStepIn(lastElement, CounterKind.FilteredRecordsCount, typeof(Record));
						Debug.Assert(lastElement == ElementHelper.__FindElement(_table, Counter.CreateFilteredRecordCounter(index), typeof(Record), false));
						return lastElement;
					}
				}

				lastElement = (Record) ElementHelper.__FindElement(_table, Counter.CreateFilteredRecordCounter(index), typeof(Record), false);
				lastCount = count;
				lastIndex = index;
				version = _table.version;
				return lastElement;
			}

			set
			{
				throw new InvalidOperationException("Collection is read only");
			}
		}

		/// <summary>
		/// Checks if the group belongs to the details section and is visible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(Element value)
		{
			if (!(value is Record))
				return false;
			value.EnsureInitialized(this, true);
			return value is Record && value.GetFilteredRecordCount() > 0 && value.ParentTable == _table;
		}

		/// <summary>
		/// Gets the visible position within the details section.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(Element value)
		{
			if (!Contains(value))
				return -1;
			return value.GetFilteredRecordPosition();
		}

		public void CopyTo(Element[] array, int index)
		{
			int n = 0;
			foreach (Element element in this)
			{
				array[index+n] = element;
				n++;
			}
		}

		//		public FilteredRecordsInTableCollection SyncRoot
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		public FilteredRecordsInTableCollectionEnumerator GetEnumerator()
		{
			_table.EnsureInitialized(this);
			return new FilteredRecordsInTableCollectionEnumerator(this);
		}
		
		#region IList Members

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

		public void RemoveAt(int index)
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
			return Contains((Element) value);
		}

		public void Clear()
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((Element) value);
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				if (_table == null)
					return 0;
				_table.EnsureInitialized(this);
				return _table.GetFilteredRecordCount();
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((Element[]) array, index);
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
	/// Enumerates through visible groups within a details section.
	/// </summary>
	public class FilteredRecordsInTableCollectionEnumerator : IEnumerator 
	{
		Record _cursor, _next;
		FilteredRecordsInTableCollection _coll;

		public FilteredRecordsInTableCollectionEnumerator(FilteredRecordsInTableCollection collection)
		{
			_coll = collection;
			_cursor = null;
			if (_coll.Count > 0)
				_next = _coll[0];
		}

		#region IEnumerator Members

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

		public Record Current
		{
			get
			{
				return _cursor;
			}
		}

		public bool MoveNext()
		{
			if (_next == null)
				return false;

			_cursor = _next;

			_next = (Record) ElementHelper.GetNextElementStepIn(_next, CounterKind.FilteredRecordsCount, typeof(Record));

			if (_next != null && _next.ParentTable != _coll._table)
				_next = null;

			return _cursor != null;
		}
		#endregion

	}
	#endregion
	#region SortedRecords
	
	/// <summary>
	/// A Read-only list of visible groups within a details section. (A group can be hidden if 
	/// a filter is selected and no records fit its criteria.)
	/// </summary>
	public class RecordsInTableCollection : IList
	{
		internal Table _table;
		internal Record lastElement;
		internal int lastIndex;
		internal int version;
		internal int lastCount;

		internal RecordsInTableCollection(Table table)
		{
			_table = table;
		}

		internal DetailsSection GetRootDetails()
		{
			return _table.TopLevelGroup.Details;
		}

		/// <summary>
		/// A group.
		/// </summary>
		public Record this[int index]
		{
			get
			{
				int count = Count;
				if (index < 0 || index >= count)
					throw new ArgumentOutOfRangeException();

				EnsureSortOrder();

				if (lastIndex != -1 && lastCount == count && version == _table.version)
				{
					if (index == lastIndex)
					{
						return lastElement;
					}
					else if (index == lastIndex+1)
					{
						lastIndex++;
#if DEBUG
						Element prev = lastElement;
						lastElement = (Record) ElementHelper.GetNextElementStepIn(lastElement, CounterKind.RecordsCount, typeof(Record));
						Element el = ElementHelper.__FindElement(_table, Counter.CreateRecordCounter(index), typeof(Record), false);
						if (lastElement != el)
						{
							Trace.WriteLine(prev.GetSortedRecordPosition());
							Trace.WriteLine(prev.GetElementEntry());
							Trace.WriteLine(lastElement.GetSortedRecordPosition());
							Trace.WriteLine(lastElement.GetElementEntry());
							if (el != null)
							{
								Trace.WriteLine(el.GetSortedRecordPosition());
								Trace.WriteLine(el.GetElementEntry());
							}
							el = ElementHelper.__FindElement(_table, Counter.CreateRecordCounter(index), typeof(Record), false);
							el = (Record) ElementHelper.GetNextElementStepIn(prev, CounterKind.RecordsCount, typeof(Record));
							Trace.WriteLine(el.GetSortedRecordPosition());
							Debug.Assert(false);
						}
#else
						lastElement = (Record) ElementHelper.GetNextElementStepIn(lastElement, CounterKind.RecordsCount, typeof(Record));
						#endif
						return lastElement;
					}
				}

				lastElement = (Record) ElementHelper.__FindElement(_table, Counter.CreateRecordCounter(index), typeof(Record), false);
				lastCount = count;
				lastIndex = index;
				version = _table.version;
				return lastElement;
			}
			set
			{
				throw new InvalidOperationException("Collection is read only");
			}
		}

		/// <summary>
		/// Checks if the group belongs to the details section and is visible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(Record value)
		{
			if (value == null)
				return false;

			if (_table == null)
				return false;
			value.EnsureInitialized(this, true);
			return value != null && value.ParentTable == _table;
		}

		/// <summary>
		/// Gets the visible position within the details section.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(Record value)
		{
			if (!Contains(value))
				return -1;
			return value.GetSortedRecordPosition();
		}

		public void CopyTo(Record[] array, int index)
		{
			int n = 0;
			foreach (Record record in this)
			{
				array[index+n] = record;
				n++;
			}
		}

		//		public RecordsInTableCollection SyncRoot
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		public RecordsInTableCollectionEnumerator GetEnumerator()
		{
			return new RecordsInTableCollectionEnumerator(this);
		}
		
		#region IList Members

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

		public void RemoveAt(int index)
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
			return Contains((Record) value);
		}

		public void Clear()
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((Record) value);
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		void EnsureSortOrder()
		{
			// TODO: compare sorted columsn, resort collection
		}

		public int Count
		{
			get
			{
				if (_table == null)
					return 0;
				_table.EnsureInitialized(this);
				return _table.GetRecordCount();
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((Record[]) array, index);
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
	/// Enumerates through visible groups within a details section.
	/// </summary>
	public class RecordsInTableCollectionEnumerator : IEnumerator 
	{
		Record _cursor, _next;
		RecordsInTableCollection _coll;

		public RecordsInTableCollectionEnumerator(RecordsInTableCollection collection)
		{
			_coll = collection;
			_cursor = null;
			if (_coll.Count > 0)
				_next = _coll[0];
		}

		#region IEnumerator Members

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

		public Record Current
		{
			get
			{
				return _cursor;
			}
		}

		public bool MoveNext()
		{
			if (_next == null)
				return false;

			_cursor = _next;

			_next = (Record) ElementHelper.GetNextElementStepIn(_next, CounterKind.RecordsCount, typeof(Record));

			if (_next != null && _next.ParentTable != _coll._table)
				_next = null;

			return _cursor != null;
		}
		#endregion

	}
	#endregion
#endif
	#region SortedRecordsInDetails
	public class RecordsInDetailsCollection : IList
	{
		internal SortedRecordsTreeTable _inner;
		internal RecordsDetails _groupWithRecords;

		public static RecordsInDetailsCollection Empty = new RecordsInDetailsCollection(null);

		internal RecordsInDetailsCollection(RecordsDetails groupWithRecords)
		{
			_groupWithRecords = groupWithRecords;
			if (_groupWithRecords != null)
				_inner = groupWithRecords.RecordTreeEntries;
		}

		public int FindRecord(object sortKey)
		{
			if (_groupWithRecords != null && _groupWithRecords.ParentGroup != null)
			{
				_groupWithRecords.EnsureInitialized(this);
				int c = this._groupWithRecords.ParentTable.GetVisibleCount();
				//Console.WriteLine(this._groupWithRecords.ParentTable);
				Type type = this._groupWithRecords.ParentGroup.SortColumns[0].FieldDescriptor.GetPropertyType();
				object key = Convert.ChangeType(sortKey, type);
				return _inner.TreeTable.IndexOfKey(new object[] { sortKey });
			}
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sortKeys"></param>
		/// <returns></returns>
		public int FindRecord(params object[] sortKeys)
		{
//			product21.ParentTable.TableDescriptor.SortedColumns.Add("ProductName");
//			product21.ParentTable.TableDescriptor.SortedColumns.Add("SupplierID");
//			int sp = product21.Records.FindRecord("Spegesild", "21");
//			product21.Records["Spegesild"].SetCurrent();

			if (_groupWithRecords != null && _groupWithRecords.ParentGroup != null)
			{
				_groupWithRecords.EnsureInitialized(this);
				int c = this._groupWithRecords.ParentTable.GetVisibleCount();
				//Console.WriteLine(this._groupWithRecords.ParentTable);
				return _inner.TreeTable.IndexOfKey(sortKeys);
			}
			return -1;
		}

		public Record this[string sortKey]
		{
			get
			{
				if (sortKey == null)
					throw new ArgumentNullException("sortKey");
				int index = FindRecord(sortKey);
				if (index == -1)
					throw new ArgumentException(sortKey.ToString() + " not found in " + this._groupWithRecords.ParentGroup.SortColumns[0].Name);
				return this[index];
			}
		}

		public bool Contains(string sortKey)
		{
			return FindRecord(sortKey) != -1;
		}
		
		public int IndexOf(string sortKey)
		{
			return FindRecord(sortKey);
		}


		/// <summary>
		/// Gets the element at the zero-based index.
		/// Setting is not supported and will throw an exception since the collection is Read-only.
		/// </summary>
		public Record this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException();

				SortedRecordsTreeTableEntry entry = _inner[index];
				if (entry == null)
					return null;
				return entry.Element;
			}
			set
			{
				throw new InvalidOperationException("Collection is read only");
			}
		}

		public bool Contains(Record value)
		{
			if (value == null)
				return false;

			if (_groupWithRecords == null)
				return false;
			value.EnsureInitialized(this, true);
			return value.ParentElement == _groupWithRecords;
		}

		public int IndexOf(Record value)
		{
			if (!Contains(value))
				return -1;
			return value.SortedEntry.GetPosition();//InnerSortedRecordPosition;
		}

		public void CopyTo(Record[] array, int index)
		{
			int n = 0;
			foreach (Record record in this)
			{
				array[index+n] = record;
				n++;
			}
		}

		//		public RecordsInDetailsCollection SyncRoot
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		public RecordsInDetailsCollectionEnumerator GetEnumerator()
		{
			if (_groupWithRecords != null)
				_groupWithRecords.EnsureInitialized(this);
			return new RecordsInDetailsCollectionEnumerator(this);
		}
		
		#region IList Members

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

		public void RemoveAt(int index)
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
			return Contains((Record) value);
		}

		public void Clear()
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((Record) value);
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				if (_groupWithRecords == null)
					return 0;
				_groupWithRecords.EnsureInitialized(this);
				return _inner.Count;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((Record[]) array, index);
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

	public class RecordsInDetailsCollectionEnumerator : IEnumerator 
	{
		Record _cursor, _next;
		RecordsInDetailsCollection _coll;

		public RecordsInDetailsCollectionEnumerator(RecordsInDetailsCollection collection)
		{
			_coll = collection;
			_cursor = null;
			if (_coll.Count > 0)
				_next = _coll[0];
		}

		#region IEnumerator Members

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

		public Record Current
		{
			get
			{
				return _cursor;
			}
		}

		public bool MoveNext()
		{
			if (_next == null)
				return false;

			_cursor = _next;

			_next = (Record) ElementHelper.GetNextSibling(_next);
			
			return _cursor != null;
		}
		#endregion

	}
	#endregion
	#region FilteredRecordsInDetails
	public class FilteredRecordsInDetailsCollection : IList
	{
		internal SortedRecordsTreeTable _inner;
		internal RecordsDetails _groupWithRecords;
		internal Record lastElement;
		internal int lastIndex;
		internal int lastCount;

		public static FilteredRecordsInDetailsCollection Empty = new FilteredRecordsInDetailsCollection(null);

		internal FilteredRecordsInDetailsCollection(RecordsDetails groupWithRecords)
		{
			if (groupWithRecords != null)
				_inner = groupWithRecords.RecordTreeEntries;
			_groupWithRecords = groupWithRecords;
		}

		/// <summary>
		/// Gets (and caches) the element at the zero-based index.
		/// Setting is not supported and will throw an exception since the collection is Read-only.
		/// </summary>
		public Record this[int index]
		{
			get
			{
				int count = Count;
				if (index < 0 || index >= count)
					throw new ArgumentOutOfRangeException();

				if (lastIndex != -1 && lastCount == count)
				{
					if (index == lastIndex)
					{
						return lastElement;
					}
					else if (index == lastIndex+1)
					{
						lastIndex++;
						lastElement = (Record) ElementHelper.GetNextSiblingElement(lastElement, CounterKind.FilteredRecordsCount);
						Debug.Assert(lastElement == ((SortedRecordsTreeTableEntry) _inner.GetEntryAtCounterPosition(Counter.CreateFilteredRecordCounter(index))).Element);
						return lastElement;
					}
				}

				SortedRecordsTreeTableEntry entry = (SortedRecordsTreeTableEntry) _inner.GetEntryAtCounterPosition(Counter.CreateFilteredRecordCounter(index));
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

		public bool Contains(Record value)
		{
			if (value == null)
				return false;

			if (_groupWithRecords == null)
				return false;
			_groupWithRecords.EnsureInitialized(this, true);
			return value.GetFilteredRecordCount() > 0 && value.ParentElement == _groupWithRecords;
		}

		public int IndexOf(Record value)
		{
			if (!Contains(value))
				return -1;
			return ((Counter) value.SortedEntry.GetCounterPosition()).FilteredRecordCount;
		}

		public void CopyTo(Record[] array, int index)
		{
			int n = 0;
			foreach (Record record in this)
			{
				array[index+n] = record;
				n++;
			}
		}

		//		public FilteredRecordsInDetailsCollection SyncRoot
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		public FilteredRecordsInDetailsCollectionEnumerator GetEnumerator()
		{
			if (_groupWithRecords != null)
				_groupWithRecords.EnsureInitialized(this);
			return new FilteredRecordsInDetailsCollectionEnumerator(this);
		}
		
		#region IList Members

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

		public void RemoveAt(int index)
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
			return Contains((Record) value);
		}

		public void Clear()
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((Record) value);
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Collection is Read-only.");
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				if (_groupWithRecords == null)
					return 0;
				_groupWithRecords.EnsureInitialized(this);
				return _inner.Count;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((Record[]) array, index);
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

	public class FilteredRecordsInDetailsCollectionEnumerator : IEnumerator 
	{
		Record _cursor, _next;
		FilteredRecordsInDetailsCollection _coll;

		public FilteredRecordsInDetailsCollectionEnumerator(FilteredRecordsInDetailsCollection collection)
		{
			_coll = collection;
			_cursor = null;
			if (_coll.Count > 0)
				_next = _coll[0];
		}

		#region IEnumerator Members

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

		public Record Current
		{
			get
			{
				return _cursor;
			}
		}

		public bool MoveNext()
		{
			if (_next == null)
				return false;

			_cursor = _next;

			_next = (Record) ElementHelper.GetNextSiblingElement(_next, CounterKind.RecordsCount);

			return _cursor != null;
		}
		#endregion

	}
	#endregion
}
