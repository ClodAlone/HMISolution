//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellContextValue.cs" company="syncfusion">
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
    /// Provides storage for a value that can only be associated with one specific cell. Optimal
    /// for shared cell objects where only one cell will create events at a time. Other cells will
    /// have a default value instead.
    /// </summary>
    /// <remarks>
    /// <see cref="GridCellButton"/> does for example use this storage to save and query for
    /// hovering and mousedown state. Only one cell can be in hovering or mouse down mode.
    /// </remarks>
    public class GridCellContextValue
    {
        // Fields
        int rowIndex = -1;
        int colIndex = -1;
        object value;
        object defaultValue;
        
        // ctor

        /// <overload>
        /// Initializes <see cref="GridCellContextValue"/>.
        /// </overload>
        /// <summary>
        /// Initializes <see cref="GridCellContextValue"/> with a default value.
        /// </summary>
        /// <param name="defaultValue">The default value to be used for all queries where the row and
        /// column index do not match the current settings.</param>
        public GridCellContextValue(object defaultValue)
        {
            this.value = defaultValue;
            this.defaultValue = defaultValue;
        }
        
        /// <summary>
        /// Initializes <see cref="GridCellContextValue"/> with row, column index, and associated value and a default value.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="value">The value that is specific to row and column index.</param>
        /// <param name="defaultValue">The default value to be used for all queries where the row and
        /// column index do not match the current settings.</param>
        public GridCellContextValue(int rowIndex, int colIndex, object value, object defaultValue)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.value = value;
            this.defaultValue = defaultValue;
        }

        // Methods

        /// <summary>
        /// Gets the value for the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>
        /// If the row and column index do match the current settings, the saved value is returned;
        /// otherwise the default value is returned.
        /// </returns>
        public object GetValue(int rowIndex, int colIndex)
        {
            if (rowIndex == this.rowIndex && colIndex == this.colIndex)
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Sets the value for the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="value">The value that is specific to row and column index.</param>
        /// <returns>True if the value was modified; False otherwise.</returns>
        public bool SetValue(int rowIndex, int colIndex, object value)
        {
            bool changed = this.rowIndex != rowIndex || this.colIndex != colIndex
                || ((this.value != null && !this.value.Equals(value)) 
                || (this.value == null && value != null));
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.value = value;
            return changed;
        }

        /// <summary>
        /// Resets current information. The value will be reset to default value.
        /// </summary>
        public void ResetValue()
        {
            this.rowIndex = -1;
            this.colIndex = -1;
            this.value = defaultValue;
        }

        /// <override/>
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see
        /// cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see
        /// cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return String.Concat("rowIndex = ", rowIndex.ToString(), ", colIndex = ", colIndex.ToString(), ", value = ", value);
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

