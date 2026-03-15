#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.GridCommon;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{

    /// <summary>
    /// This a base class for spanned ranges such as covered cells and
    /// cell spanned backgrounds. It contains Top, Left, Bottom and
    /// Right row and column index for the spanned range.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CellSpanInfoBase : IComparable<CellSpanInfoBase>, IComparable
    {
        int top;
        int left;
        int bottom;
        int right;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanInfoBase"/> class.
        /// </summary>
        public CellSpanInfoBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanInfoBase"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        public CellSpanInfoBase(int top, int left, int bottom, int right)
        {
            this.top = Math.Min(top, bottom);
            this.left = Math.Min(left, right);
            this.bottom = Math.Max(top, bottom);
            this.right = Math.Max(left, right);
        }

        /// <summary>
        /// Gets the top row index.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get
            {
                return top;
            }
            //set
            //{
            //    top = value;
            //}
        }

        /// <summary>
        /// Gets the left column index.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get
            {
                return left;
            }
            //set
            //{
            //    left = value;
            //}
        }

        /// <summary>
        /// Gets the bottom row index.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom
        {
            get
            {
                return bottom;
            }
            //set
            //{
            //    bottom = value;
            //}
        }

        /// <summary>
        /// Gets the right column index.
        /// </summary>
        /// <value>The right.</value>
        public int Right
        {
            get
            {
                return right;
            }
            //set
            //{
            //    right = value;
            //}
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return Right - Left + 1;
            }
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return Bottom - Top + 1;
            }
        }


        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (((this.left ^ ((this.top << 13) | (this.left >> 19))) ^ ((this.right << 26) | (this.right >> 6))) ^ ((this.bottom << 7) | (this.bottom >> 25)));
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            CellSpanInfoBase other = obj as CellSpanInfoBase;
            if (other == null)
                return false;

            return left == other.left
                && right == other.right
                && top == other.top
                && bottom == other.bottom;
        }

        /// <summary>
        /// Determines whether the specified cell is inside the span.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cell</param>
        /// <returns>
        /// true if cell is inside the span; false otherwise.
        /// </returns>
        public bool Contains(RowColumnIndex cellRowColumnIndex)
        {
            return cellRowColumnIndex.RowIndex >= top && cellRowColumnIndex.RowIndex <= bottom
                && cellRowColumnIndex.ColumnIndex >= left && cellRowColumnIndex.ColumnIndex <= right;
        }


        /// <summary>
        /// Determines whether the specified row is inside the span.
        /// </summary>
        /// <param name="rowIndex">The row index</param>
        /// <returns>
        /// true if row is inside the span; false otherwise.
        /// </returns>
        public bool ContainsRow(int rowIndex)
        {
            return rowIndex >= top && rowIndex <= bottom;
        }


        /// <summary>
        /// Determines whether the specified column is inside the span.
        /// </summary>
        /// <param name="columnIndex">The column index</param>
        /// <returns>
        /// true if column is inside the span; false otherwise.
        /// </returns>
        public bool ContainsColumn(int columnIndex)
        {
            return columnIndex >= left && columnIndex <= right;
        }
        /// <summary>
        /// Determines whether the specified cell is inside the span.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>
        /// true if cell is inside the span; false otherwise.
        /// </returns>
        public bool Contains(int rowIndex, int columnIndex)
        {
            return rowIndex >= top && rowIndex <= bottom
                && columnIndex >= left && columnIndex <= right;
        }

        #region IComparable<CellSpanInfoBase> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(CellSpanInfoBase other)
        {
            int cmp = other.top - top;
            if (cmp == 0)
            {
                cmp = other.left - left;
                if (cmp == 0)
                {
                    cmp = other.bottom - bottom;
                    if (cmp == 0)
                        cmp = other.right - right;
                }
            }
            return cmp;
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return CompareTo((CellSpanInfoBase)obj);
        }

        #endregion

        /// <summary>
        /// Returns a string describing the state of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} ( Top = {1} Left = {2} Bottom = {3} Right = {4} )",
               GetType().Name, Top, Left, Bottom, Right);
        }


        /// <summary>
        /// Offsets the span with the specified row and column count.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public void Offset(int rowCount, int columnCount)
        {
            this.top += rowCount;
            this.bottom += rowCount;
            this.left += columnCount;
            this.right += columnCount;
        }

    }

}
