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
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Controls.Cells;
using System.Security;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    ///    Defines a range of cells in the grid. Possible range types are: Row(s), Column(s), Cell(s), Table or Empty.
    ///    GridRangeInfo is immutable.
    /// </summary>
	[TypeConverter(typeof(Syncfusion.Windows.Controls.Grid.GridRangeInfoConverter))]
    [Serializable]
    [ImmutableObject(true)]

#if ENABLE_PARTIAL_TRUST
    public class GridRangeInfo: IFormattable, ICloneable, IDisposable
#else
    public class GridRangeInfo: IFormattable, ICloneable,ISerializable, IDisposable
#endif
    {
        // Fields.
        internal int _top;
        internal int _left;
        internal int _bottom;
        internal int _right;
        internal GridRangeInfoType _rangeType;

		/// <summary>
		///   <para>Represents a <see cref="GridRangeInfo"/> with its properties left uninitialized and range type set to GridRangeInfoType.Empty.</para>
		/// </summary>
		/// <remarks>
		///   <para>A range is defined by its coordinates and range type. If uninitialized, the range type is GridRangeInfoType.Empty</para>
		/// </remarks>
        public readonly static GridRangeInfo Empty = new GridRangeInfo();
		//public readonly static GridRangeInfo All = new GridRangeInfo(0, 0, int.MaxValue, int.MaxValue);

		/// <overload>
		///   <para>Initializes a <see cref="GridRangeInfo"/> object.</para>
		/// </overload>
		/// <summary>
		///   <para>Initializes an empty <see cref="GridRangeInfo"/> object.</para>
		/// </summary>
		/// <remarks>
		///   <para>This constructor initializes a new <see cref="GridRangeInfo"/> object with empty range type.</para>
		/// </remarks>
        [DebuggerStepThrough()] public GridRangeInfo()
        {
            _top = _left = _bottom = _right = 0;
            _rangeType = GridRangeInfoType.Empty;
        }

		/// <summary>
		/// Initializes a new <see cref="GridRangeInfo"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		public GridRangeInfo(SerializationInfo info, StreamingContext context)
		{
			_top = info.GetInt32("Top");
			_left = info.GetInt32("Left");
			_bottom = info.GetInt32("Bottom");
			_right = info.GetInt32("Right");
			_rangeType = (GridRangeInfoType) info.GetValue("RangeType", typeof(GridRangeInfoType));
		}

		/// <summary>
		/// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridRangeInfo"/>.
		/// </summary>
		/// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
		/// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
#if !SyncfusionFramework4_0
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter=true)]
#else
        [SecurityCritical]
#endif

#if !ENABLE_PARTIAL_TRUST
        [DebuggerStepThrough()] public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Top", _top); // Int32
            info.AddValue("Left", _left); // Int32
            info.AddValue("Bottom", _bottom); // Int32
            info.AddValue("Right", _right); // Int32
            info.AddValue("RangeType", _rangeType); // GridRangeInfoType
        }
#endif

		/// <summary>
		/// Creates an exact copy of this <see cref="GridRangeInfo"/> object.
		/// </summary>
		/// <returns>The <see cref="GridRangeInfo"/> object that this method creates.</returns>
		[DebuggerStepThrough()] public virtual object Clone()
        {
			GridRangeInfo rg = new GridRangeInfo();
			CopyAllMembers(rg);
            return rg;
        }

		/// <internalonly/>
		protected void CopyAllMembers(GridRangeInfo rg)
		{
			rg._bottom = _bottom;
			rg._left = _left;
			rg._rangeType = _rangeType;
			rg._right = _right;
			rg._top = _top;
		}

        internal GridRangeInfo(int row, int col)
            : this(row, col, row, col)
        {
        }

        internal GridRangeInfo(int top, int left, int bottom, int right)
        {
            this._top = Math.Min(top, bottom);
            this._left = Math.Min(left, right);
            this._bottom = Math.Max(top, bottom);
            this._right = Math.Max(left, right);
            this._rangeType = GridRangeInfoType.Cells;
            if (this._bottom == int.MaxValue || this._right == int.MaxValue
                || this._left < 0 || this._top < 0)
                throw new ArgumentException();
        }

        internal GridRangeInfo(GridRangeInfoType rt, int from, int to)
        {
            switch (rt)
            {
            case GridRangeInfoType.Cols:
                _left = Math.Min(from, to);
                _right = Math.Max(from, to);
                _top = 0;
                _bottom = int.MaxValue;
                _rangeType = GridRangeInfoType.Cols;
                if (this._right == int.MaxValue || this._left < 0)
                    throw new ArgumentException();
                break;

            case GridRangeInfoType.Rows:
                _top = Math.Min(from, to);
                _bottom = Math.Max(from, to);
                _left = 0;
                _right = int.MaxValue;
                _rangeType = GridRangeInfoType.Rows;
                if (this._bottom == int.MaxValue || this._top < 0)
                    throw new ArgumentException();
                break;

            default:
                throw new ArgumentException("Invalid RangeType", "rt");
            }
        }

        internal GridRangeInfo(GridRangeInfoType rt, int rowOrCol)
            : this(rt, rowOrCol, rowOrCol)
        {
        }

        internal GridRangeInfo(GridRangeInfoType rt) 
        {
            if (rt != GridRangeInfoType.Table)
                throw new ArgumentException("Invalid RangeType", "rt");

            _left = _top = 0;
            _right = _bottom = int.MaxValue;
            _rangeType = GridRangeInfoType.Table;
        }

        internal GridRangeInfo(GridRangeInfoType rangeType, int top, int left, int bottom, int right)
        {
            this._rangeType = rangeType;
            this._top = Math.Min(top, bottom);
            this._left = Math.Min(left, right);
            this._bottom = Math.Max(top, bottom);
            this._right = Math.Max(left, right);
            if (this._left < 0 || this._top < 0)
                throw new ArgumentException("Negative row or column numbers invalid", "rt");
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object with the specified bounds.
		/// </summary>
		/// <param name="top">
		///   The row index of the upper-left corner of the cell range.
		/// </param>
		/// <param name="left">
		///   The column index of the upper-left corner of the cell range.
		/// </param>
		/// <param name="height">
		///   Number of rows to span. 
		/// </param>
		/// <param name="width">
		///   Number of columns to span.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo FromTlhw(int top, int left, int height, int width)  
        {
            if (height <= 0 || width <= 0)
                return GridRangeInfo.Empty;

            return new GridRangeInfo(top, left, top+height-1, left+width-1);
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the specified row and column index.
		/// </summary>
		/// <param name="row">
		///   The row index.
		/// </param>
		/// <param name="col">
		///   The column index.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Cell(int row, int col) 
        {
			if (row < 0 || col < 0)
				return GridRangeInfo.Empty;
            return GridRangeInfo.Cells(row, col, row, col);
        }
        
		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object with the specified bounds.
		/// </summary>
		/// <param name="top">
		///   The row index of the upper-left corner of the cell range.
		/// </param>
		/// <param name="left">
		///   The column index of the upper-left corner of the cell range.
		/// </param>
		/// <param name="bottom">
		///   The row index of the bottom-right corner of the cell range.
		/// </param>
		/// <param name="right">
		///   The column index of the bottom-right corner of the cell range.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
        [DebuggerStepThrough()] public static GridRangeInfo Cells(int top, int left, int bottom, int right)
        {
			if (top < 0 || left < 0)
				return GridRangeInfo.Empty;
			return new GridRangeInfo(top, left, bottom, right);
        }
        
        internal static GridRangeInfo InternalCells(int top, int left, int bottom, int right)
        {
            if (bottom < top || right < left || top < 0 || left < 0)
                return GridRangeInfo.Empty;

            return new GridRangeInfo(top, left, bottom, right);
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the specified row index.
		/// </summary>
		/// <param name="rowIndex">
		///   The row index of the cell range.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Row(int rowIndex)
        {
            return GridRangeInfo.Rows(rowIndex, rowIndex);
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the specified rows.
		/// </summary>
		/// <param name="top">
		///   The top row index of the cell range.
		/// </param>
		/// <param name="bottom">
		///   The botom row index of the cell range.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Rows(int top, int bottom)
        {
			if (top < 0 || bottom < 0)
				return GridRangeInfo.Empty;
			return new GridRangeInfo(GridRangeInfoType.Rows, top, bottom);
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the specified column.
		/// </summary>
		/// <param name="colIndex">
		///   The column index of the cell range.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Col(int colIndex)
        {
            return GridRangeInfo.Cols(colIndex, colIndex);
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the specified columns.
		/// </summary>
		/// <param name="left">
		///   The left column index of the cell range.
		/// </param>
		/// <param name="right">
		///   The right column index of the cell range.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Cols(int left, int right)
        {
			if (right < 0 || left < 0)
				return GridRangeInfo.Empty;
			return new GridRangeInfo(GridRangeInfoType.Cols, left, right);
        }

		/// <summary>
		/// Returns <see cref="GridRangeInfo.Empty"/> that is an empty range.
		/// </summary>
		/// <returns>An empty range object.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo EmptyRange()
		{
			return Empty;
		}

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the whole table.
		/// </summary>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Table()
        {
            return new GridRangeInfo(GridRangeInfoType.Table);
        }

		/// <summary>
		/// Creates a new <see cref="GridRangeInfo"/> object for the specified row and column.
		/// </summary>
		/// <param name="rowIndex">
		/// The row index. -1 to create a range of columns or whole table.
		/// </param>
		/// <param name="colIndex">
		/// The column index. -1 to create a range of rows or whole table.
		/// </param>
		/// <remarks>
		/// If both row and column index are less than zero, a table will be created.
		/// If row index is less than zero and column index greater or equal to zero, a column range will be created.
		/// If row index is greater or equal to zero and column index less than zero, a row range will be created.
		/// Otherwise a cell range is created.
		/// </remarks>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo Auto(int rowIndex, int colIndex)
		{
			GridRangeInfo range;

			if (rowIndex > -1 && colIndex > -1)
				range = GridRangeInfo.Cell(rowIndex, colIndex);
			else if (colIndex > -1)
				range = GridRangeInfo.Col(colIndex);
			else if (rowIndex > -1)
				range = GridRangeInfo.Row(rowIndex);
			else
				range = GridRangeInfo.Table();

			return range;
		}

        /// <summary>
        /// Creates a new <see cref="GridRangeInfo"/> object for the specified rows and columns.
        /// </summary>
        /// <param name="top">
        /// The row index. -1 to create a range of columns or whole table.
        /// </param>
        /// <param name="left">
        /// The column index. -1 to create a range of rows or whole table.
        /// </param>
        /// <param name="bottom">
        /// Bottom row index. 
        /// </param>
        /// <param name="right">
        /// Right column index. 
        /// </param>
        /// <remarks>
        /// If both row and column index are less than zero, a table will be created.
        /// If row index is less than zero and column index greater or equal to zero, a column range will be created.
        /// If row index is greater or equal to zero and column index less than zero, a row range will be created.
        /// Otherwise a cell range is created.
        /// </remarks>
        /// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
        [DebuggerStepThrough()]
        public static GridRangeInfo Auto(int top, int left, int bottom, int right)
        {
            GridRangeInfo range;

            if (top > -1 && left > -1)
                range = GridRangeInfo.Cells(top, left, bottom, right);
            else if (left > -1)
                range = GridRangeInfo.Cols(left, right);
            else if (top > -1)
                range = GridRangeInfo.Rows(top, bottom);
            else
                range = GridRangeInfo.Table();

            return range;
        }

        internal CellSpanInfoBase ToCellSpan()
        {
            return new CellSpanInfoBase(Top, Left, Bottom, Right);
        }

        /// <summary>
        /// Returns the spanned range for the given grid model.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <returns>Spanned range.</returns>
        public CellSpanInfoBase ToCellSpan(GridModel gridModel)
        {
            return ExpandRange(-1, -1, gridModel.RowCount, gridModel.ColumnCount).ToCellSpan();
        }

        /// <summary>
		/// Enumerate through all cells in range object. 
		/// </summary>
		/// <param name="top">
		/// The row index of the upper-left corner of the cell range.
		/// </param>
		/// <param name="left">
		/// The column index of the upper-left corner of the cell range.
		/// </param>
		/// <returns>This method returns the first cell in the range.</returns>
        [DebuggerStepThrough()] public bool GetFirstCell(out int top, out int left)
        {
            top = this._top;
            left = this._left;
            return Valid;
        }

		/// <overload>
		/// This method returns the adjacent cell in the range for the given row and column index.
		/// </overload>
		/// <summary>
		/// This method returns the adjacent cell in the range for the given row and column index.
		/// </summary>
		/// <param name="nRow">
		///   The row index.
		/// </param>
		/// <param name="nCol">
		///   The column index.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if an adjacent cell in this <see cref="GridRangeInfo"/> object coud be found; otherwise, <see langword="false"/>.
		/// </returns>
		[DebuggerStepThrough()] public bool GetNextCell(ref int nRow, ref int nCol) 
        {
            // Sort By Row.
            return GetNextCell(ref nRow, ref nCol, true);
        }

		/// <summary>
		/// This method returns the adjacent cell in the range for the given row and column index.
		/// </summary>
		/// <param name="nRow">
		///   The row index.
		/// </param>
		/// <param name="nCol">
		///   The column index.
		/// </param>
		/// <param name="bSortByRow">
		/// <see langword="true"/> if the range should be traversed by row; <see langword="false"/> if the range should be traversed by column.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if an adjacent cell in this <see cref="GridRangeInfo"/> object could be found; otherwise, <see langword="false"/>.
		/// </returns>
        [DebuggerStepThrough()] public bool GetNextCell(ref int nRow, ref int nCol, bool bSortByRow)
        {
            if (!Valid)
                return false;

            if (bSortByRow && ++nCol > _right)
            {
                if (++nRow > _bottom)
                    nRow = nCol = 0;
                else
                    nCol = _left;
            }
            else if (!bSortByRow && ++nRow > _bottom)
            {
                if (++nCol > _right)
                    nRow = nCol = 0;
                else
                    nRow = _top;
            }

            return nRow > 0 || nCol > 0;
        }

		/// <summary>
		/// Convert column and row ranges into cell ranges with the specified bounds.
		/// (Could also be done through IntersectRange ... if nFirstRow >= nFirstCol >= 0). 
		/// </summary>
		/// /<remarks>
		/// Column ranges will be converted to cell ranges using nFirstRow and nRowCount. 
		/// Row ranges will be converted to cell ranges using nFirstCol and nColCount. 
		/// Column ranges will be converted to cell ranges using all input parameters. 
		/// </remarks>
		/// <param name="nFirstRow">Row index for the first non-label cell in grid area.</param>
		/// <param name="nFirstCol">Column index for the first non-label cell in grid area.</param>
		/// <param name="nRowCount">Last row in the grid.</param>
		/// <param name="nColCount">Last column in the grid</param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public GridRangeInfo ExpandRange(int nFirstRow, int nFirstCol, int nRowCount, int nColCount)
        {
            GridRangeInfo r = (GridRangeInfo) this.Clone();
            switch (this._rangeType)
            {
            case GridRangeInfoType.Cols:
                r._top = nFirstRow;
                r._bottom = nRowCount - 1;
                break;
            case GridRangeInfoType.Rows:
                r._left = nFirstCol;
                r._right = nColCount - 1;
                break;
            case GridRangeInfoType.Table:
                r._top = nFirstRow;
                r._bottom = nRowCount -1;
                r._left = nFirstCol;
                r._right = nColCount - 1;
                break;
            }
            r._rangeType = GridRangeInfoType.Cells;
            return r;
        }

		/// <summary>
		/// Adjusts the location of this range by the specified amount.
		/// </summary>
		/// <param name="rowOffset">Amount of rows to offset the location.</param>
		/// <param name="colOffset">Amount of columns to offset the location.</param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public GridRangeInfo OffsetRange(int rowOffset, int colOffset)
        {
            GridRangeInfo r = (GridRangeInfo) this.Clone();
            switch (this._rangeType)
            {
            case GridRangeInfoType.Rows:
                r._top += rowOffset;
                r._bottom += rowOffset;
                break;
            case GridRangeInfoType.Cols:
                r._left += colOffset;
                r._right += colOffset;
                break;
            case GridRangeInfoType.Cells:
                r._top += rowOffset;
                r._bottom += rowOffset;
                r._left += colOffset;
                r._right += colOffset;
                break;
            }
            return r;
        }

        // see also ExpandRange(-1, -1, -1, -1)
        internal GridRangeInfo InternalRange()
        {
            GridRangeInfo r = (GridRangeInfo) this.Clone();
            switch (this._rangeType)
            {
            case GridRangeInfoType.Cols:
                r._top = -1;
                r._bottom = -1;
                break;
            case GridRangeInfoType.Rows:
                r._left = -1;
                r._right = -1;
                break;
            case GridRangeInfoType.Table:
                r._top = -1;
                r._bottom = -1;
                r._left = -1;
                r._right = -1;
                break;
            }
            r._rangeType = GridRangeInfoType.Cells;
            return r;
        }



        // Range operations.
		/// <summary>
		///   <para>Determines if this range intersects with <paramref name="range"/>.</para>
		/// </summary>
		/// <param name="range">
		///   The range to test.
		/// </param>
		/// <example>
		///   <para>The following example creates two cell ranges and tests whether they intersect:</para>
		///   <code lang="C#">
		///   GridRangeInfo firstRange = new GridRangeInfo(1, 1, 100, 100);
		///   ...
		///   </code>
		/// </example>
		/// <returns>
		///   <para><see langword="true"/> if there is any intersection.</para>
		/// </returns>
        [DebuggerStepThrough()] public bool IntersectsWith(GridRangeInfo range)  
        {
            return this.IntersectRange(range).Valid;
        }

		/// <summary>
		/// <para>Determines if this range fully contains all of <paramref name="range"/>.</para>
		/// </summary>
		/// <param name="range">
		///   The range to test.
		/// </param>
		/// <returns>
		///   <para><see langword="true"/> if range is a subset of this range.</para>
		/// </returns>
		[DebuggerStepThrough()] public bool Contains(GridRangeInfo range)  
        {
            return !IsEmpty && this.IntersectRange(range).Equals(range);
        }

		/// <summary>
		///   <para>Creates a new <see cref="GridRangeInfo"/> with the intersection of itself and the specified <see cref="GridRangeInfo"/>.</para>
		/// </summary>
		/// <overload>
		///   <para>Creates a new <see cref="GridRangeInfo"/> with the intersection of itself and the specified <see cref="GridRangeInfo"/>.</para>
		/// </overload>
		/// <param name="range">
		///   The <see cref="GridRangeInfo"/> with which to intersect.
		/// </param>
		/// <example>
		///   <para>The following example creates two <see cref="GridRangeInfo"/> objects and creates a <see cref="GridRangeInfo"/> with their intersection:</para>
		///   <code lang="C#">GridRangeInfo firstRange = new GridRangeInfo(0, 0, 100, 100);
		///   </code>
		/// </example>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public GridRangeInfo IntersectRange(GridRangeInfo range)
        {
            return GridRangeInfo.IntersectRange(this, range);
        }

		/// <summary>
		///   <para>Creates a new <see cref="GridRangeInfo"/> with the intersection of two <see cref="GridRangeInfo"/> parameters.</para>
		/// </summary>
		/// <param name="r1">
		///   The first <see cref="GridRangeInfo"/> with which to intersect.
		/// </param>
		/// <param name="r2">
		///   The second <see cref="GridRangeInfo"/> with which to intersect.
		/// </param>
		/// <example>
		///   <para>The following example creates two <see cref="GridRangeInfo"/> objects and creates a <see cref="GridRangeInfo"/> with their intersection:</para>
		///   <code lang="C#">GridRangeInfo firstRange = new GridRangeInfo(0, 0, 100, 100);
		///   </code>
		/// </example>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo IntersectRange(GridRangeInfo r1, GridRangeInfo r2)
        {
            GridRangeInfo r = new GridRangeInfo();
            r._left = Math.Max(r1._left, r2._left);
            r._right = Math.Min(r1._right, r2._right);
            r._top = Math.Max(r1._top, r2._top);
            r._bottom = Math.Min(r1._bottom, r2._bottom);
            if (r._left <= r._right && r._top <= r._bottom)
                r.EnsureRangeType();
            else
                r = GridRangeInfo.Empty;

            return r;
        }

		/// <overload>
		/// Creates a <see cref="GridRangeInfo"/> that represents the union of itself and another range.
		/// </overload>
		/// <summary>
		/// Creates a <see cref="GridRangeInfo"/> that represents the union of itself and another range.
		/// </summary>
		/// <param name="range">
		/// A range to union.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public GridRangeInfo UnionRange(GridRangeInfo range)
        {
            return GridRangeInfo.UnionRange(this, range);
        }

		/// <summary>
		/// Creates a <see cref="GridRangeInfo"/> that represents the union of two ranges.
		/// </summary>
		/// <param name="r1">
		/// A range to union.
		/// </param>
		/// <param name="r2">
		/// A range to union.
		/// </param>
		/// <returns>The new <see cref="GridRangeInfo"/> that this method creates.</returns>
		[DebuggerStepThrough()] public static GridRangeInfo UnionRange(GridRangeInfo r1, GridRangeInfo r2)
        {
            if (r1.IsEmpty && r2.IsEmpty)
                return GridRangeInfo.Empty;
            else if (r1.IsEmpty)
                return r2;
            else if (r2.IsEmpty)
                return r1;
            else
            {
                GridRangeInfo r = new GridRangeInfo();
                r._left = Math.Min(r1._left, r2._left);
                r._right = Math.Max(r1._right, r2._right);
                r._top = Math.Min(r1._top, r2._top);
                r._bottom = Math.Max(r1._bottom, r2._bottom);
                if (r._left <= r._right && r._top <= r._bottom)
                    r.EnsureRangeType();
                else
                    r = GridRangeInfo.Empty;
                return r;
            }
        }

        private void EnsureRangeType()
        {
            if (this._left == 0 && this._right == int.MaxValue)
            {
                if (this._top == 0 && this._bottom == int.MaxValue)
                    this._rangeType = GridRangeInfoType.Table;
                else
                    this._rangeType = GridRangeInfoType.Rows;
            }
            else if (this._top == 0 && this._bottom == int.MaxValue)
                this._rangeType = GridRangeInfoType.Cols;
            else
                this._rangeType = GridRangeInfoType.Cells;
        }                    
            
        internal bool InsertRows(int row, int count)
        {
            if (Valid && _bottom != int.MaxValue && _bottom >= row)
            {
                if (_top >= row)
                    _top += count;
                _bottom += count;
                return true;
            }
            return false;
        }
        
        internal bool InsertCols(int col, int count)
        {
            if (Valid && _right != int.MaxValue && _right >= col)
            {
	    	    if (_left >= col)
	    	    	_left += count;
	    	    _right += count;
                return true;
            }
            return false;
	    }

        internal bool RemoveRows(int from, int last, int maxrow)
        {
            bool affected = false;
            if (Valid && _bottom != int.MaxValue)
            {
                int count = last-from+1;
	
                if (_top >= from && _bottom <= last)
                {
                    _rangeType = GridRangeInfoType.Empty;
                    affected = true;
                }
                else
                {
                    if (_top >= from)
                    {
                        _top = Math.Max(from, Math.Max(last, _top)-count);
                        affected = true;
                    }
                    if (_bottom >= from)
                    {
                        _bottom = Math.Max(last, _bottom)-count;
                        affected = true;
                    }
                    if (_bottom < _top)
                        _rangeType = GridRangeInfoType.Empty;
                }

                if (EnsureRowLimits(maxrow))
                    affected = true;
            }
            return affected;
        }

        internal bool EnsureRowLimits(int maxrow)
        {
            if (!Valid)
                return false;
            else if (_top > maxrow)
            {
                _rangeType = GridRangeInfoType.Empty;
                return true;
            } 
            else if (_bottom > maxrow)
            {
                // Don't adjust when GridRangeInfoType.Cols or GridRangeInfoType.Table
                if (_bottom != int.MaxValue)
                {
                    _bottom = maxrow;
                    return true;
                }
            }
            return false;
        }
            
        internal bool RemoveCols(int from, int last, int maxcol)
        {
            bool affected = false;
            if (Valid && _right != int.MaxValue)
            {
                int count = last-from+1;
	
                if (_left >= from && _right <= last)
                {
                    _rangeType = GridRangeInfoType.Empty;
                    affected = true;
                }
                else
                {
                    if (_left >= from)
                    {
                        _left = Math.Max(from, Math.Max(last, _left)-count);
                        affected = true;
                    }
                    if (_right >= from)
                    {
                        _right = Math.Max(last, _right)-count;
                        affected = true;
                    }
                    if (_right < _left)
                        _rangeType = GridRangeInfoType.Empty;
                }

                if (EnsureColLimits(maxcol))
                    affected = true;
            }
            return affected;
        }

        internal bool EnsureColLimits(int maxcol)
        {
            if (!Valid)
                return false;
            else if (_left > maxcol)
            {
                _rangeType = GridRangeInfoType.Empty;
                return true;
            } 
            else if (_right > maxcol)
            {
                // Don't adjust when GridRangeInfoType.Rows or GridRangeInfoType.Table
                if (_right != int.MaxValue)
                {
                    _right = maxcol;
                    return true;
                }
            }
            return false;
        }
            
        internal bool MoveRows(int nFromRow, int nToRow, int nDestRow, int maxrow)
        {
            if (!Valid || _bottom == int.MaxValue)
                return false;

            bool affected = false;
            int nCount = nToRow-nFromRow+1;
            if (nDestRow > nFromRow)
                nDestRow += nCount;

            if (_top >= nFromRow && _top <= nToRow && nToRow < nDestRow)
            {
                // from _top to dest
                _top = nDestRow - nCount + (_top-nFromRow);
                _bottom = nDestRow - nCount + (_bottom-nFromRow);
                affected = true;
            }
            else if (_top >= nFromRow && _top <= nToRow && nDestRow < nFromRow)
            {
                // dest from _top to
                _top = nDestRow + (_top-nFromRow);
                _bottom = nDestRow + (_bottom-nFromRow);
                affected = true;
            }
            else if (_top > nToRow && _top < nDestRow)
            {
                // from to _top dest
                _top  = _top - nCount;
                _bottom = _bottom - nCount;
                affected = true;
            }
            else if (_top >= nDestRow && _top < nFromRow)
            {
                // dest _top from to
                _top = _top + nCount;
                _bottom = _bottom + nCount;
                affected = true;
            }

            if (EnsureRowLimits(maxrow))
                affected = true;

            return affected;
        }

        internal bool MoveCols(int nFromCol, int nToCol, int nDestCol, int maxcol)
        {
            if (!Valid || _right == int.MaxValue)
                return false;

            bool affected = false;
            int nCount = nToCol-nFromCol+1;
            if (nDestCol > nFromCol)
                nDestCol += nCount;

            if (_left >= nFromCol && _left <= nToCol && nToCol < nDestCol)
            {
                // from _left to dest
                _left = nDestCol - nCount + (_left-nFromCol);
                _right = nDestCol - nCount + (_right-nFromCol);
                affected = true;
            }
            else if (_left >= nFromCol && _left <= nToCol && nDestCol < nFromCol)
            {
                // dest from _left to
                _left = nDestCol + (_left-nFromCol);
                _right = nDestCol + (_right-nFromCol);
                affected = true;
            }
            else if (_left > nToCol && _left < nDestCol)
            {
                // from to _left dest
                _left  = _left - nCount;
                _right = _right - nCount;
                affected = true;
            }
            else if (_left >= nDestCol && _left < nFromCol)
            {
                // dest _left from to
                _left = _left + nCount;
                _right = _right + nCount;
                affected = true;
            }

            if (EnsureColLimits(maxcol))
                affected = true;

            return affected;
        }

        // Object overrides
		
		/// <override/>
        /// <summary>Specifies whether the current object is equivalent to the given object.</summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if they are equal; False otherwise.</returns>
		[DebuggerStepThrough()] public override bool Equals(object obj)  
        {
            if (obj == null || !(obj is GridRangeInfo)) 
                return false;

            GridRangeInfo r = obj as GridRangeInfo;
            return !(this.Valid || r.Valid) ||
                (this.Valid && r.Valid
                && this._rangeType == r._rangeType
                && this._top == r._top
                && this._left == r._left
                && this._bottom == r._bottom
                && this._right == r._right);
        }

		/// <override/>
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
		[DebuggerStepThrough()] public override int GetHashCode()  
        {
            return (((this._left^((this._top << 13) | (this._left >> 19)))^((this._right << 26) | (this._right >> 6)))^((this._bottom << 7) | (this._bottom >> 25)));
        }

		/// <override/>        
        /// <summary>
        ///		Compares two range objects and returns if they are both equal. 
        /// </summary>
        /// <param name="r1">The left-hand side of the operator.</param>
        /// <param name="r2">The right-hand side of the operator.</param>
        /// <returns>
        ///		True if both are equal; False otherwise.
        ///	</returns>
        [DebuggerStepThrough()] public static bool operator==(GridRangeInfo r1, GridRangeInfo r2)  
        {
            object obj1 = r1;
            object obj2 = r2;
            return obj1 == obj2 || obj1 != null && r1.Equals(r2);
        }

		/// <override/>
        /// <summary>
		///		Compares two range objects and returns if they are both unequal. 
		/// </summary>
		/// <param name="r1">The left-hand side of the operator.</param>
		/// <param name="r2">The right-hand side of the operator.</param>
		/// <returns>
        ///		True if both are unequal; False otherwise.
		///	</returns>
		[DebuggerStepThrough()] public static bool operator!=(GridRangeInfo r1, GridRangeInfo r2)
        {
			object obj1 = r1;
            object obj2 = r2;
            return !(obj1 == obj2 || obj1 != null && r1.Equals(r2));
        }

		/// <summary cref="Equals">
		///		Compares two range objects and returns if they are both equal. 
		/// </summary>
		/// <param name="r1">The left-hand side of the operator.</param>
		/// <param name="r2">The right-hand side of the operator.</param>
		/// <returns>
		///		bool
		///	</returns>
		public static bool Compare( GridRangeInfo r1, GridRangeInfo r2)  
		{
			object obj1 = r1;
			object obj2 = r2;
			return obj1 == obj2 || obj1 != null && r1.Equals(r2);
		}


		/// <summary>
		/// Returns a string in the format "A, B, C, ... AA, AB ..." to be used for column labels.
		/// </summary>
		/// <param name="nCol">The column index.</param>
		/// <returns>
		///   <para>A string that contains the column label for the column index.</para>
		/// </returns>
        [DebuggerStepThrough()] public static string GetAlphaLabel(int nCol) 
        {
            char[] cols = new char[10];
            int n = 0;
			while (nCol > 0 && n < 9)
			{
				nCol--;
				cols[n] = (char) (nCol%26 + 'A');
                nCol = nCol/26;
                n++;
			}
			
			char[] chs = new char[n];
			for (int i = 0; i < n; i++)
				chs[n-i-1] = cols[i];
			
            return new String(chs);
        }
            
		/// <summary>
		/// Returns a numeric string in the format to be used for row labels.
		/// </summary>
		/// <param name="nRow">The row index.</param>
		/// <returns>
		///   <para>A string that contains the row label for the row index.</para>
		/// </returns>
		[DebuggerStepThrough()] public static string GetNumericLabel(int nRow) 
        {
            return nRow.ToString();
        }
            

        // Properties

		/// <summary>
		/// The <see cref="GridRangeInfoType"/> of this <see cref="GridRangeInfo"/> object.
		/// </summary>
        public GridRangeInfoType RangeType 
        {
			[DebuggerStepThrough()] 
            get 
            {
                return _rangeType; 
            }
        }

		/// <summary>
		///   <para><see langword="true"/> if this represents a table.</para>
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTable
        {
			[DebuggerStepThrough()] 
            get
            {
                return _rangeType == GridRangeInfoType.Table;
            }
        }

		/// <summary>
		///   <para><see langword="true"/> if this represents a range of rows.</para>
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRows
        {
			[DebuggerStepThrough()] 
            get
            {
                return _rangeType == GridRangeInfoType.Rows;
            }
        }

		/// <summary>
		///   <para><see langword="true"/> if this represents a range of columns.</para>
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCols
        {
			[DebuggerStepThrough()] 
            get
            {
                return _rangeType == GridRangeInfoType.Cols;
            }
        }

		/// <summary>
		///   <para><see langword="true"/> if this represents a range of individual cells.</para>
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCells
        {
			[DebuggerStepThrough()] 
            get
            {
                return _rangeType == GridRangeInfoType.Cells;
            }
        }

		/// <summary>
		///   The row index of the upper-left corner of the cell range.
		/// </summary>
		public int Top 
        {
			[DebuggerStepThrough()] 
            get 
            {
                if ((_rangeType & GridRangeInfoType.Cols) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _top; 
            }
        }

		/// <summary>
		///   The column index of the upper-left corner of the cell range.
		/// </summary>
        public int Left 
        {
			[DebuggerStepThrough()] 
            get 
            {
                if ((_rangeType & GridRangeInfoType.Rows) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _left; 
            }
        }

		/// <summary>
		///   The row index of the bottom-right corner of the cell range.
		/// </summary>
        public int Bottom 
        {
			[DebuggerStepThrough()] 
            get
            {
                if ((_rangeType & GridRangeInfoType.Cols) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _bottom; 
            }
        }

		/// <summary>
		///   The column index of the bottom-right corner of the cell range.
		/// </summary>
        public int Right 
        {
			[DebuggerStepThrough()] 
            get 
            {
                if ((_rangeType & GridRangeInfoType.Rows) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                  return _right; 
            }
        }

        private bool Valid 
        {
            get 
            {
                return _rangeType != GridRangeInfoType.Empty; 
            }
        }

		/// <summary>
		///   <para>Tests whether this <see cref="GridRangeInfo"/> has a <see cref="GridRangeInfoType"/> of Empty.</para>
		/// </summary>
		/// <value>
		///   <para><see langword="true"/> if <see cref="GridRangeInfoType"/> is Empty; otherwise, <see langword="false"/>.</para>
		/// </value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmpty
        {
			[DebuggerStepThrough()] 
            get 
            {
                return ((object) this) == null || _rangeType == GridRangeInfoType.Empty; 
            }
        }

		/// <summary>
		///   <para>Gets the width of the range defined by this <see cref="GridRangeInfo"/>.</para>
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Width
        {
			[DebuggerStepThrough()] 
            get
            {
                return _right-_left+1;
            }
        }

		/// <summary>
		///   <para>Gets the height of the range defined by this <see cref="GridRangeInfo"/>.</para>
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Height
        {
			[DebuggerStepThrough()] 
            get
            {
                return _bottom-_top+1;
            }
        }

		/// <summary>
		/// Results of ToString method.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Info
		{
			get
			{
				return ToString();
			}
		}

		/// <overload>
		///   <para>Converts the attributes of this <see cref="GridRangeInfo"/> to a human-readable string.</para>
		/// </overload>
		/// <summary>
		///   <para>Converts the attributes of this <see cref="GridRangeInfo"/> to a human-readable string.</para>
		/// </summary>
		/// <remarks>
		/// The generate string will be in the format R#C#, R#, C#, T.<para/>
		/// An example for a range of cells is "R1C1:R10C20" or "R5C5".<para/>
		/// An example for a range of rows is "R1:R10" or "R6".<para/>
		/// An example for a range of columns is "C1:C10" or "C7".<para/>
		/// An example for a table range is "T".<para/>
		/// </remarks>
		/// <returns>
		///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
		/// </returns>
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		/// <summary>
		///   <para>Converts the attributes of this <see cref="GridRangeInfo"/> to a human-readable string.</para>
		/// </summary>
		/// <param name="provider">The <see cref="IFormatProvider"/> to use to format the value. Is ignored.</param>
		/// <returns>
		///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
		/// </returns>
		/// <returns>
		///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
		/// </returns>
		public string ToString(IFormatProvider provider)
		{
			return this.ToString(null, provider);
		}
		
		/// <summary>
		///   <para>Converts the attributes of this <see cref="GridRangeInfo"/> to a human-readable string.</para>
		/// </summary>
		/// <param name="format">Ignored.</param>
		/// <returns>
		///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
		/// </returns>
		/// <returns>
		///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
		/// </returns>
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		/// <summary>
		///   <para>Converts the attributes of this <see cref="GridRangeInfo"/> to a human-readable string.</para>
		/// </summary>
		/// <param name="formatProvider">The <see cref="IFormatProvider"/> to use to format the value. Is ignored.</param>
		/// <param name="format">Ignored.</param>
		/// <returns>
		///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
		/// </returns>
		/// <genoverload/>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (IsEmpty)
				return "";

			GridRangeInfo range = (GridRangeInfo) this;

			string[] parts = null;

            if (format == null || format == "" || format == "G")
			{
				switch (range.RangeType)
				{
					case GridRangeInfoType.Table:
						parts = new string[] 
					{
						"T"
					};
						break;

					case GridRangeInfoType.Cols:
						if (range.Width > 1 || range.Height > 1)
						{
							parts = new string[] 
						{
							"C", range.Left.ToString(),
							":",
							"C", range.Right.ToString()
						};
						}
						else
						{
							parts = new string[] 
						{
							"C", range.Left.ToString(),
							};
						}
						break;

					case GridRangeInfoType.Rows:
						if (range.Width > 1 || range.Height > 1)
						{
							parts = new string[] 
						{
							"R", range.Top.ToString(),
							":",
							"R", range.Bottom.ToString(),
							};
						}
						else
						{
							parts = new string[] 
						{
							"R", range.Top.ToString(),
							};
						}
						break;

					case GridRangeInfoType.Cells:
						if (range.Width > 1 || range.Height > 1)
						{
							parts = new string[] 
						{
							"R", range.Top.ToString(),
							"C", range.Left.ToString(),
							":",
							"R", range.Bottom.ToString(),
							"C", range.Right.ToString()
						};
						}
						else
						{
							parts = new string[] 
						{
							"R", range.Top.ToString(),
							"C", range.Left.ToString(),
							};
						}
						break;
				}
			}
			else
				throw new ArgumentException("format");

			return String.Concat(parts);
		}
		
		private static GridRangeInfo ParsePart(string parseText) 
        {
            if (parseText == "T")
            {
                return GridRangeInfo.Table();
            }
            else
            {
                if (parseText.StartsWith("R"))
                {
                    int ci = parseText.IndexOf("C");
                    if (ci == -1)
                        return GridRangeInfo.Row(Int32.Parse(parseText.Substring(1)));
                    else
                        return GridRangeInfo.Cell(Int32.Parse(parseText.Substring(1, ci-1)), Int32.Parse(parseText.Substring(ci+1)));
                }
                else if (parseText.StartsWith("C"))
                    return GridRangeInfo.Col(Int32.Parse(parseText.Substring(1)));
            }

            throw new FormatException("Invalid Range: " + parseText);
        }

		/// <summary>
		/// Creates a range object from a string.
		/// </summary>
		/// <param name="parseText">The text with text representation of the range.</param>
		/// <returns>The <see cref="GridRangeInfo"/> with coordinates specifed in the string.</returns>
		/// <remarks>
		/// The method parses a string that was previously created with <see cref="GridRangeInfo.ToString"/>.
		/// <para/>
		/// The string should be in the format R#C#, R#, C#, T.<para/>
		/// An example for a range of cells is "R1C1:R10C20" or "R5C5".<para/>
		/// An example for a range of rows is "R1:R10" or "R6".<para/>
		/// An example for a range of columns is "C1:C10" or "C7".<para/>
		/// An example for a table range is "T".<para/>
		/// </remarks>
		public static GridRangeInfo Parse(string parseText)
        {
            parseText = parseText.Trim();
            if (parseText == "")
                return GridRangeInfo.Empty;

            try
            {
                if (parseText.IndexOf("..") != -1)
                    parseText = new StringBuilder(parseText).Replace("..", ":").ToString();
        
                parseText = parseText.ToUpper(CultureInfo.InvariantCulture);

                string[] parts = parseText.Split(new char[] { ':' });
                if (parts.Length == 1)
                    return ParsePart(parts[0]);
                else if (parts.Length == 2)
                {
                    GridRangeInfo r1 = ParsePart(parts[0]);
                    GridRangeInfo r2 = ParsePart(parts[1]);

                    if (r1.RangeType == r2.RangeType)
                        return GridRangeInfo.UnionRange(r1, r2);
                }
            }
            catch (FormatException ex)
            {
				TraceUtil.TraceExceptionCatched(ex);
				//ExceptionManager.RaiseExceptionCatched(null, ex);
			}
            catch (ArgumentException ex)
            {
				TraceUtil.TraceExceptionCatched(ex);
				//ExceptionManager.RaiseExceptionCatched(null, ex);
			}

            throw new FormatException("Invalid Range: " + parseText);
        }


		internal class RowComparer: IComparer 
        {
            public int Compare(object a, object b)  
            {
                if (a == null || b == null)
                    return System.Collections.Comparer.Default.Compare(a,b);

                GridRangeInfo r0 = (GridRangeInfo) a;
                GridRangeInfo r1 = (GridRangeInfo) b;

                if (r0 == r1)
                    return 0;
                else 
                {
                    if (r0.RangeType == GridRangeInfoType.Table)
                        return -1;
                    else if (r1.RangeType == GridRangeInfoType.Table)
                        return 1;
                    else if (r0.RangeType == GridRangeInfoType.Cols)
                        return -1;
                    else if (r1.RangeType == GridRangeInfoType.Cols)
                        return 1;
                    else if (r0.Top != r1.Top)
                        return r0.Top - r1.Top;
                    else if (r0.Bottom != r1.Bottom)
                        return r0.Bottom - r1.Bottom;
                    else if (r0.Left != r1.Left)
                        return r0.Left - r1.Left;
                    else
                        return r0.Right - r1.Right;
                }
            }
        }

		internal class ColComparer: IComparer 
        {
            public int Compare(object a, object b)  
            {
                if (a == null || b == null)
                    return System.Collections.Comparer.Default.Compare(a,b);

                GridRangeInfo r0 = (GridRangeInfo) a;
                GridRangeInfo r1 = (GridRangeInfo) b;

                if (r0 == r1)
                    return 0;
                else 
                {
                    if (r0.RangeType == GridRangeInfoType.Table)
                        return -1;
                    else if (r1.RangeType == GridRangeInfoType.Table)
                        return 1;
                    else if (r0.RangeType == GridRangeInfoType.Rows)
                        return -1;
                    else if (r1.RangeType == GridRangeInfoType.Rows)
                        return 1;
                    else if (r0.Left != r1.Left)
                        return r0.Left - r1.Left;
                    else if (r0.Right != r1.Right)
                        return r0.Right - r1.Right;
                    else if (r0.Top != r1.Top)
                        return r0.Top - r1.Top;
                    else 
                        return r0.Bottom - r1.Bottom;
                }
            }
        }

        public void Dispose()
        {
        
        }
    }
}
