//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellPos.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Holds the coordinates for a cell. Is used by GridVolatileData to look up cell information.
    /// </summary>
    public struct GridCellPos
    {
        // Fields
        internal int _rowNumber;
        internal int _columnNumber;

        // Constructors

        /// <summary>
        /// Initializes a new <see cref="GridCellPos"/> with row and column coordinates.
        /// </summary>
        /// <param name="r">The row index.</param>
        /// <param name="c">The column index.</param>
        public GridCellPos(int r, int c)
        {
            this._rowNumber = r;
            this._columnNumber = c;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <override/>
        public override int GetHashCode()
        {
            return unchecked((int)((this._rowNumber * 2654435761u) + this._columnNumber));
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                new string[] 
                {
                    "GridCellPos {RowNumber = ",
                    this.RowNumber.ToString(),
                    ", ColumnNumber = ",
                    this.ColumnNumber.ToString(),
                    "}"
                });
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int ColumnNumber
        {
            get
            {
                return this._columnNumber;
            }

            set
            {
                this._columnNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        public int RowNumber
        {
            get
            {
                return this._rowNumber;
            }

            set
            {
                this._rowNumber = value;
            }
        }

        /// <summary>
        /// Gets results of ToString method.
        /// </summary>
        public string Info
        {
            get
            {
                return ToString();
            }
        }
    }
}
