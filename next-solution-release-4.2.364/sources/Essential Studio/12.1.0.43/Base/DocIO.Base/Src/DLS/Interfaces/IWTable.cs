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

#region file using directives
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a table in a document. 
    /// </summary>
    public interface IWTable : ICompositeEntity
    {
        /// <summary>
        /// Gets row collection.
        /// </summary>
        WRowCollection Rows
        {
            get;
        }
        /// <summary>
        /// Gets the table format.
        /// </summary>
        /// <value>The table format.</value>
        RowFormat TableFormat
        {
            get;
        }
        /// <summary>
        /// Gets the last cell in last row.
        /// </summary>
        WTableCell LastCell
        {
            get;
        }
        /// <summary>
        /// Gets the last row in table.
        /// </summary>
        WTableRow FirstRow
        {
            get;
        }
        /// <summary>
        /// Gets the last row in table.
        /// </summary>
        WTableRow LastRow
        {
            get;
        }
        /// <summary>
        /// Gets cell by row/column indexes.
        /// </summary>
        WTableCell this[int row, int column]
        {
            get;
        }
        /// <summary>
        /// Gets / sets table width.
        /// </summary>
        float Width
        {
            get;
        }
        /// <summary>
        /// Gets or sets the table title.
        /// </summary>
        /// <value>The title.</value>
        string Title
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the table description.
        /// </summary>
        /// <value>The description.</value>
        string Description
        {
            get;
            set;
        }
        /// <summary>
        /// Adds new row to table.
        /// </summary>
        /// <returns></returns>
        WTableRow AddRow();
        /// <summary>
        /// Adds new row to table.
        /// </summary>
        /// <returns></returns>
        WTableRow AddRow(bool isCopyFormat);
        /// <summary>
        /// Adds a row to table with copy format option
        /// </summary>
        /// <param name="isCopyFormat">Indicates whether copy format from previous row or not</param>
        /// <param name="autoPopulateCells">if it specifies auto populate cells, set to <c>true</c>.</param>
        /// <returns></returns>
        WTableRow AddRow(bool isCopyFormat, bool autoPopulateCells);
        /// <summary>
        /// Resets rows / columns numbers.
        /// </summary>
        /// <param name="rowsNum">The rows number.</param>
        /// <param name="columnsNum">The columns number.</param>
        void ResetCells(int rowsNum, int columnsNum);
        /// <summary>
        /// Resets rows / columns numbers.
        /// </summary>
        /// <param name="rowsNum">The rows num.</param>
        /// <param name="columnsNum">The columns num.</param>
        /// <param name="format">The format.</param>
        /// <param name="cellWidth">Width of the cell.</param>
        void ResetCells(int rowsNum, int columnsNum, RowFormat format, float cellWidth);
        /// <summary>
        /// Applies the vertical merge for table cells.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="endRowIndex">End index of the row.</param>
        void ApplyVerticalMerge(int columnIndex, int startRowIndex, int endRowIndex);
        /// <summary>
        /// Applies horizontal merging for cells of table row.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startCellIndex">Start index of the cell.</param>
        /// <param name="endCellIndex">End index of the cell.</param>
        void ApplyHorizontalMerge(int rowIndex, int startCellIndex, int endCellIndex);
        /// <summary>
        /// Gets/sets indent from left for the table.
        /// </summary>
        float IndentFromLeft
        {
            get;
            set;
        }
        /// <summary>
        /// Removes the absolute position data. If table has absolute position in the document,
        /// all position data will be erased.  
        /// </summary>
        void RemoveAbsPosition();
        /// <summary>
        /// Applies the built-in table style.
        /// </summary>
        /// <param name="builtinStyle">The built-in table style.</param>
        void ApplyStyle(BuiltinTableStyle builtinTableStyle);
    }
}