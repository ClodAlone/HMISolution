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

using Syncfusion.DLS.Collections;

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes table functionality
  /// </summary>
  public interface ITable : IParagraphItem
  {
    /// <summary>
    /// Gets table format.
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    TableFormat TableFormat{ get; }
    /// <summary>
    /// Gets / sets table width.
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    float Width{ get; set; }
    /// <summary>
    /// Gets row collection.
    /// </summary>
    RowCollection Rows { get; }
    /// <summary>
    /// Adds new row to table.
    /// </summary>
    /// <returns></returns>
    TableRow AddRow();
    /// <summary>
    /// Adds new row to table.
    /// </summary>
    /// <returns></returns>
    TableRow AddRow( bool isCopyFormat );
    /// <summary>
    /// Adds new column to table.
    /// </summary>
    void AddColumn();
    /// <summary>
    /// Adds several columns to table.
    /// </summary>
    void AddColumn( int columnsCount );
    /// <summary>
    /// Resets row/column dimensions.
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    void ResetCells( int rows, int columns );
    /// <summary>
    /// Gets last cell in last row.
    /// </summary>
    TableCell LastCell { get; }
    /// <summary>
    /// Gets last row in table.
    /// </summary>
    TableRow LastRow { get; }
    /// <summary>
    /// Gets table columns.
    /// </summary>
    TableColumnCollection Columns{ get; }
    /// <summary>
    /// Gets cell by row/column indexes.
    /// </summary>
    TableCell this[ int row, int column ]{ get; }
    
  }
}