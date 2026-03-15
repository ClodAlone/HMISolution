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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using Syncfusion.Diagnostics;

namespace Syncfusion.Collections
{
	/// <summary>
	/// Implements a two-dimensional table that holds an <see cref="SFArrayList" /> of rows. Each row
	/// is an <see cref="SFArrayList" /> of objects.
	/// </summary>
	/// <remarks>
	/// <p>This is a memory efficient way to represent a table where values can remain empty. Only rows
	/// that actually contain data will allocate an <see cref="SFArrayList" /> and the array only holds
	/// as many objects as the specific row contains columns.</p>
	/// <p>When you access data that are out of range, an empty () object will be returned.
	/// If you set data that are out of range, an exception will be thrown. If you set data for
	/// a row that is empty, the row will be allocated before the value is stored.</p>
	/// <p>SFTable provides methods that let you insert, remove or rearrange columns or rows
	/// in the table.</p>
	/// </remarks>
	/// <seealso cref="SFArrayList"/>
	[Serializable]
	public class SFTable : ICloneable, ISerializable
	{
		private int rowCount = 0;
		private int colCount = 0;
		private SFArrayList rows = null;
		private int cellCount = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="SFTable"/> class from the specified instances
		/// of the <see cref="SerializationInfo"/> and <see cref="StreamingContext"/> classes.
		/// </summary>
		/// <param name="info">An instance of the <see cref="System.Runtime.Serialization.SerializationInfo"/> class containing the information required to serialize the new <see cref="SFTable"/> instance.</param>
		/// <param name="context">An instance of the <see cref="System.Runtime.Serialization.StreamingContext"/> class containing the source of the serialized stream associated with the new <see cref="SFTable"/> instance. </param>
		/// <remarks>This constructor implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface for the <see cref="SFTable"/> class.</remarks>
		protected SFTable(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#endif

			rowCount = info.GetInt32("RowCount");
			colCount = info.GetInt32("ColCount");
			rows = (SFArrayList) info.GetValue("Rows", typeof(SFArrayList));
			cellCount = info.GetInt32("CellCount");
		}

		/// <summary>
		/// Implements the ISerializable interface and returns the data needed to serialize the <see cref="SFTable"/>.
		/// </summary>
		/// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
		/// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		[SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter=true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#endif


			info.AddValue("RowCount", rowCount); // int
			info.AddValue("ColCount", colCount); // int
			info.AddValue("Rows", rows); // SFArrayList
			info.AddValue("CellCount", cellCount); // int
		}

		/// <overload>
		/// Initializes a new instance of the <see cref="SFTable" />
		/// class.
		/// </overload>
		/// <summary>
		/// Initializes a new instance of the <see cref="SFTable" />
		/// class that is empty.
		/// </summary>
		public SFTable()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SFTable" />
		/// class and optional copies of data from an existing table.
		/// </summary>
		protected SFTable(SFTable data, bool clone)
		{
			rowCount = data.rowCount;
			colCount = data.colCount;
			if (data.rows != null && clone)
				rows = (SFArrayList) data.rows.Clone();
		}

		/// <summary>
		///   <para>Creates a deep copy of the <see cref="SFTable" />.</para>
		/// </summary>
		/// <returns>
		///   <para>A deep copy of the <see cref="SFTable" />.</para>
		/// </returns>
		public virtual object Clone()
		{
			return new SFTable(this, true);
		}

		/// <summary>
		///   <para>Removes all elements from the <see cref="SFTable" />.</para>
		/// </summary>
		public void Clear()
		{
			rowCount = 0;
			colCount = 0;
			rows = null;
		}

        /// <summary>
        /// Returns the <see cref="SFArrayList" /> from all rows.
        /// </summary>
		public SFArrayList Rows
		{
			get
			{
				if (rows == null)
					rows = CreateRowsCollection();
				return rows;
			}
		}

		/// <summary>
        /// Creates and returns a new <see cref="SFArrayList" />.
		/// </summary>
        /// <returns>A new <see cref="SFArrayList" /></returns>
        SFArrayList CreateRowsCollection()
		{
			return new SFArrayList();
		}

		/// <summary>
		/// Creates a collection of cells for a row.
		/// </summary>
		/// <returns>An <see cref="SFArrayList"/> or derived object for the cell collection.</returns>
		public virtual SFArrayList CreateCellCollection()
		{
			return new SFArrayList();
		}

		/// <summary>
		///   <para>Gets / sets the number of rows contained in the <see cref="SFTable" />.</para>
		/// </summary>
		/// <remarks>
		/// If you decrease the row count, the rows in the <see cref="SFTable" /> will be removed.
		/// </remarks>
		public int RowCount
		{
			get
			{
				return rowCount;
			}
			set
			{
				if (value < rowCount)
					RemoveRows(value, rowCount-value);
				else if (value > rowCount)
					InsertRows(rowCount, value-rowCount);
			}
		}

		/// <summary>
		///   <para>Gets / sets the number of columns contained in the <see cref="SFTable" />.</para>
		/// </summary>
		/// <remarks>
		/// If you decrease the column count, the last columns in each row will be removed.
		/// </remarks>
		public int ColCount
		{
			get
			{
				return colCount;
			}
			set
			{
				if (value < colCount)
					RemoveCols(value, colCount-value);
				else if (value > colCount)
					InsertCols(colCount, value-colCount);
			}
		}

		/// <summary>
		/// Inserts a specified number of empty rows in the <see cref="SFTable"/> at a given row index.
		/// </summary>
		/// <param name="rowIndex">The zero-based row index of the first row to be inserted.</param>
		/// <param name="count">The number of rows to be added.</param>
		/// <returns>not used.</returns>
		public bool InsertRows(int rowIndex, int count)
		{
			if (rowIndex > rowCount + 1)
				throw new ArgumentOutOfRangeException("rowIndex");
			Rows.InsertRange(rowIndex, count);
			rowCount = Math.Max(rowIndex, rowCount)+count;
			return true;
		}

		/// <summary>
		/// Inserts a specified number of empty columns for each row in the <see cref="SFTable"/> at a given column index.
		/// </summary>
		/// <param name="colIndex">The zero-based column index of the first column to be inserted.</param>
		/// <param name="count">The number of columns to be inserted.</param>
		/// <returns>not used.</returns>
		public bool InsertCols(int colIndex, int count)
		{
			if (colIndex > colCount + 1)
				throw new ArgumentOutOfRangeException("colIndex");
			// insert columns for each row
			foreach (object obj in Rows)
			{
				SFArrayList row = obj as SFArrayList;
				if (row != null)
					row.InsertRange(colIndex, count);
			}
			colCount = Math.Max(colIndex, colCount)+count;
			return true;
		}

		/// <summary>
		/// Removes a specified number of rows from the <see cref="SFTable"/> at a given row index.
		/// </summary>
		/// <param name="rowIndex">The zero-based row index of the first row to be removed.</param>
		/// <param name="count">The number of rows to be removed.</param>
		/// <returns>not used.</returns>
		public bool RemoveRows(int rowIndex, int count)
		{
			if (rowIndex >= rowCount || rowIndex < 0)
				throw new ArgumentOutOfRangeException("rowIndex");
			if (rowIndex+count > rowCount)
				throw new ArgumentOutOfRangeException("count");
			Rows.RemoveRange(rowIndex, count);
			rowCount -= count;
			return true;
		}

		/// <summary>
		/// Removes a specified number of columns for each row in the <see cref="SFTable"/> at a given column index.
		/// </summary>
		/// <param name="colIndex">The zero-based column index of the first column to be removed.</param>
		/// <param name="count">The number of columns to be removed.</param>
		/// <returns>not used.</returns>
		public bool RemoveCols(int colIndex, int count)
		{
			if (colIndex >= colCount || colIndex < 0)
				throw new ArgumentOutOfRangeException("colIndex");
			if (colIndex+count > colCount)
				throw new ArgumentOutOfRangeException("count");
			foreach (object obj in Rows)
			{
				SFArrayList row = obj as SFArrayList;
				if (row != null)
					row.RemoveRange(colIndex, count);
			}
			colCount -= count;
			return true;
		}

		/// <summary>
		/// Rearranges rows in the <see cref="SFTable"/>.
		/// </summary>
		/// <param name="rowIndex">The zero-based index of the first row to be moved.</param>
		/// <param name="count">The number of rows in the range to be moved.</param>
		/// <param name="target">The new starting index for the range. The zero-based index is based on the original array.</param>
		/// <example>
		/// <code>
		/// SFTable array = new SFTable();
		/// array.RowCount = 5;
		/// array.ColCount = 1;
		/// array[0,0] = 0;
		/// array[1,0] = 1;
		/// array[2,0] = 2;
		/// array[3,0] = 3;
		/// array.MoveRows(0, 2, 3);
		/// // results in new order: 2, 0, 1, 3
		/// </code>
		/// </example>
		public bool MoveRows(int rowIndex, int count, int target)
		{
			if (rowIndex >= rowCount && target >= rowCount)
				return false;
			//			if (rowIndex >= rowCount || rowIndex < 0)
//				throw new ArgumentOutOfRangeException("rowIndex");
//			if (rowIndex+count > rowCount)
//				throw new ArgumentOutOfRangeException("count");
//			if (target >= rowCount || target < 0)
//				throw new ArgumentOutOfRangeException("target");
			Rows.MoveRange(rowIndex, count, target);
			return true;
		}

		/// <summary>
		/// Rearranges columns in the <see cref="SFTable"/>.
		/// </summary>
		/// <param name="colIndex">The zero-based index of the first column to be moved.</param>
		/// <param name="count">The number of columns in the range to be moved.</param>
		/// <param name="target">The new starting index for the range. The zero-based index is based on the original array.</param>
		/// <example>
		/// <code>
		/// SFTable array = new SFTable();
		/// array.ColCount = 5;
		/// array.RowCount = 1;
		/// array[0,0] = 0;
		/// array[0,1] = 1;
		/// array[0,2] = 2;
		/// array[0,3] = 3;
		/// array.MoveCols(0, 2, 3);
		/// // results in new order: 2, 0, 1, 3
		/// </code>
		/// </example>
		public bool MoveCols(int colIndex, int count, int target)
		{
			if (colIndex >= colCount && target >= colCount)
				return false;
//			if (colIndex >= colCount || colIndex < 0)
//				throw new ArgumentOutOfRangeException("colIndex");
//			if (colIndex+count > colCount)
//				throw new ArgumentOutOfRangeException("count");
//			if (target >= colCount || target < 0)
//				throw new ArgumentOutOfRangeException("target");

			foreach (object obj in Rows)
			{
				SFArrayList row = obj as SFArrayList;
				if (row != null)
					row.MoveRange(colIndex, count, target);
			}
			return true;
		}


		/// <summary>
		///   <para>Indicates whether an element is at the specified coordinates in the <see cref="SFTable" />.</para>
		/// </summary>
		/// <param name="rowIndex">The zero-based row index.</param>
		/// <param name="colIndex">The zero-based column index.</param>
		/// <returns>
		///   <para>
		///     <see langword="true" /> if an element exists at the specified coordinates in the <see cref="SFTable" />;
        ///  <see langword="false" /> otherwise.</para>
		/// </returns>
		public bool Contains(int rowIndex, int colIndex)
		{
			SFArrayList row = Rows[rowIndex] as SFArrayList;
			if (row == null)
				return false;
			else
				return row[colIndex] != null;
		}

		/// <summary>
		///   <para>Gets / sets an element at the specified coordinates in the <see cref="SFTable" />.</para>
		/// </summary>
		/// <param name="rowIndex">The zero-based row index.</param>
		/// <param name="colIndex">The zero-based column index.</param>
		/// <remarks>
		/// If you query for an element and the coordinates are out of range, an empty (<see langword="null" />) object will be returned.<para/>
		/// If you set an element and the the coordinates are out of range, an exception is thrown.
		/// </remarks>
		public object this[int rowIndex, int colIndex]
		{
			get
			{
				if (rowIndex >= rowCount || rowIndex < 0
					|| colIndex >= colCount || colIndex < 0)
					return null;

				SFArrayList row = null;
				if (rowIndex < Rows.Count)
					row = Rows[rowIndex] as SFArrayList;
				if (row == null)
					return null;
				else
					return row[colIndex];
			}
			set
			{
				if (rowIndex >= rowCount || rowIndex < 0)
					throw new ArgumentOutOfRangeException("rowIndex");
				if (colIndex >= colCount || colIndex < 0)
					throw new ArgumentOutOfRangeException("colIndex");
				SFArrayList row = Rows[rowIndex] as SFArrayList;
				if (row == null)
				{
					if (value == null)
						return;
					else
						Rows[rowIndex] = row = CreateCellCollection();
				}
				object savedValue = row[colIndex];
				if (savedValue != null)
				{
					if (value == null)
						cellCount--;
					else
						cellCount++;
				}
				else if (value != null)
					cellCount++;

				row[colIndex] = value;
			}
		}
	}

	/// <summary>
	///    Extends ArrayList with MoveRange, InsertRange and RemoveRange methods. The Item property
	///    will grow the array on demand or return NULL if an index is out of range.
	/// </summary>
	[Serializable]
	public class SFArrayList: ArrayList
	{
		/// <summary>
		/// Overloaded. Initializes a new instance of the <see cref="SFArrayList" />
		/// class that is empty and has the default initial capacity.
		/// </summary>
		public SFArrayList()
			: base() {}

		/// <summary>
		///   <para>Initializes a new instance of the <see cref="SFArrayList" /> class that contains elements copied from the specified
		/// collection and has the same initial capacity as the number of elements copied.</para>
		/// </summary>
		/// <param name="c">The <see cref="System.Collections.ICollection" /> whose elements are copied to the new list.</param>
		public SFArrayList(ICollection c)
			: base(c) {}


		/// <summary>
		///   <para>Overridden. Creates a deep copy of the <see cref="SFArrayList" />.</para>
		/// </summary>
		/// <returns>
		///   <para>A deep copy of the <see cref="SFArrayList" />.</para>
		/// </returns>
		public override object Clone()
		{
			SFArrayList al = new SFArrayList();
			foreach (object o in this)
			{
				if (o is ICloneable)
					al.Add(((ICloneable) o).Clone());
				else
					al.Add(o);
			}
			return al;
		}

		/// <summary>
		/// Rearranges the values in the <see cref="SFArrayList"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the first value to be moved.</param>
		/// <param name="count">The number of values in the range to be moved.</param>
		/// <param name="dest">The new starting index for the range. The zero-based index is based on the original array.</param>
		/// <example>
		/// <code>
		/// SFArrayList array = new SFArrayList();
		/// array[0] = 0;
		/// array[1] = 1;
		/// array[2] = 2;
		/// array[3] = 3;
		/// array.MoveRange(0, 2, 3);
		/// // results in new order: 2, 0, 1, 3
		/// </code>
		/// </example>
		public void MoveRange(int index, int count, int dest)
		{
			if (dest == index || count == 0
				|| this.Count < index && this.Count < dest)
				return;
			else
			{
				object[] objects = new object[count];
				for (int n = 0; n < count && n+index < this.Count; n++)
					objects[n] = this[n+index];

				if (dest < index)
					index += count;
				else if (dest > index)
					dest += count;

				EnsureCount(dest);
				this.InsertRange(dest, objects);

				if (index < this.Count)
					this.RemoveRange(index, Math.Min(count, this.Count-index));
			}
		}

		/// <summary>
		/// Enlarges the array if needed.
		/// </summary>
		/// <param name="value">The size to be checked. If the array has less elements, empty (<see langword="null"/>) objects will be appended
		/// at the end of the array.</param>
		public void EnsureCount(int value)
		{
			if (this.Count < value)
				this.AddRange(new object[value-Count]);
		}

		/*public override ArrayList GetRange(int index, int count)
			{
				ArrayList al = new ArrayList();
				if (this.Count > index)
					this.CopyTo(index, al, 0, Math.Min(count, this.Count-index));
				return al;
			}*/

		/// <summary>
		/// Removes a range of values from the <see cref="SFArrayList"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the first value to be removed.</param>
		/// <param name="count">The number of values in the range to be removed.</param>
		/// <example>
		/// <code>
		/// SFArrayList array = new SFArrayList();
		/// array[0] = 0;
		/// array[1] = 1;
		/// array[2] = 2;
		/// array[3] = 3;
		/// array.RemoveRange(1, 2);
		/// // results in new order: 0, 3
		/// </code>
		/// </example>
		public override void RemoveRange(int index, int count)
		{
			if (index < this.Count)
				base.RemoveRange(index, Math.Min(count, this.Count-index));
		}

		/// <summary>
		/// Inserts a specified number of (<see langword="null" />) values in the <see cref="SFArrayList"/> at a given index.
		/// </summary>
		/// <param name="index">The zero-based index of the first value to be inserted.</param>
		/// <param name="count">The number of values in the range to be added.</param>
		/// <example>
		/// <code>
		/// SFArrayList array = new SFArrayList();
		/// array[0] = 0;
		/// array[1] = 1;
		/// array[2] = 2;
		/// array[3] = 3;
		/// array.InsertRange(1, 2);
		/// // results in new order: 0, null, null, 2, 3
		/// </code>
		/// </example>
		public void InsertRange(int index, int count)
		{
			if (index < this.Count)
				this.InsertRange(index, new object[count]);
		}

		/// <summary>
		/// Gets / sets the element at the specified index.
		/// In C#, this property is the indexer for the <see cref="SFArrayList"/> class.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get / set.</param>
		/// <value>
		/// The element at the specified index.
		/// When querying the value and the index is out of range, an empty (<see langword="null" />) object will be returned.
		/// When setting the value and the index is out of range the array will be enlarged. See <see cref="SFArrayList.EnsureCount"/>
		/// </value>
		public override /*IList.*/ object this[int index]
		{
			get
			{
				if (index >= this.Count)
					return null;
				return base[index];
			}
			set
			{
				this.EnsureCount(index+1);
				base[index] = value;
			}
		}

	}
}
