#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;

namespace Syncfusion.UI.Xaml.ScrollAxis
{
	/// <summary>
	/// Holds the coordinates for a cell. 
	/// </summary>
    [ClassReference(IsReviewed = false)]
	public struct RowColumnIndex
	{
		// Fields
        int _rowIndex;
        int _columnIndex;

		// Constructors

		/// <summary>
        /// Initializes a new <see cref="RowColumnIndex"/> with row and column coordinates.
		/// </summary>
		/// <param name="r">The row index.</param>
		/// <param name="c">The column index.</param>
        public RowColumnIndex(int r, int c)  
		{
			this._rowIndex = r;
			this._columnIndex = c;
		}

        /// <summary>
        /// Gets the empty instance with RowIndex and ColumnIndex set to int.MinValue
        /// </summary>
        /// <value>The empty.</value>
        public static RowColumnIndex Empty
        {
            get
            {
                return new RowColumnIndex(int.MinValue, int.MinValue);
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = (RowColumnIndex)obj;

            return other.RowIndex == RowIndex && other.ColumnIndex == ColumnIndex;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return _rowIndex == int.MinValue;
            }
        }


        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
		public override int GetHashCode()  
		{
			return unchecked((int) (this._rowIndex*2654435761u + this._columnIndex)); 
		}

        /// <summary>
        /// Returns the type name with state of this instance.
        /// </summary>
        /// <returns>
        /// </returns>
		public override string ToString()  
		{
			return String.Concat(
				new string[] 
				{
					"RowColumnPosition { RowIndex = ",
					this.RowIndex.ToString(),
					", ColumnIndex = ",
					this.ColumnIndex.ToString(),
					"}"
				}
				);
		}

		/// <summary>
		/// The column index.
		/// </summary>
		public int ColumnIndex
		{
			get 
			{
				return this._columnIndex;
			}
			set 
			{
				this._columnIndex = value;
			}
		}

		/// <summary>
		/// The row index.
		/// </summary>
		public int RowIndex
		{
			get 
			{
				return this._rowIndex;
			}
			set 
			{
				this._rowIndex = value;
			}
		}

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RowColumnIndex r1, RowColumnIndex r2)
        {
            return r1.Equals(r2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RowColumnIndex r1, RowColumnIndex r2)
        {
            return !r1.Equals(r2);
        }
    }
}

