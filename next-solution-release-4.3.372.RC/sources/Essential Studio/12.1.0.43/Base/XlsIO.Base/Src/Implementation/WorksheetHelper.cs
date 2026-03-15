#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation
{
  public class WorksheetHelper
  {
    #region Methods
    /// <summary>
    /// Indicates if there is formula record.
    /// </summary>
    /// <param name="sheet">Worksheet to get data for.</param>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Indicates whether cell contains formula record.</returns>
    public static bool HasFormulaRecord( IInternalWorksheet sheet, int row, int column )
    {
      CellRecordCollection cells = sheet.CellRecords;
      return cells.Table.HasFormulaRecord( row, column );
    }
    /// <summary>
    /// Returns row from the collection or creates one if necessary.
    /// </summary>
    /// <param name="sheet">Worksheet to get row from.</param>
    /// <param name="rowIndex">Zero-based row index.</param>
    /// <param name="bCreate">Indicates whether to create row if it doesn't exist.</param>
    /// <returns>Desired row object.</returns>
    public static RowStorage GetOrCreateRow( IInternalWorksheet sheet, int rowIndex, bool bCreate )
    {
      CellRecordCollection cells = sheet.CellRecords;

      if( cells == null && !bCreate )
        return null;

      ExcelVersion version = sheet.Workbook.Version;
      int iHeight = ( sheet.Application as ApplicationImpl ).StandardHeightInRowUnits;

      return cells.Table.GetOrCreateRow( rowIndex, iHeight, bCreate, version );
    }
    /// <summary>
    /// Gets row information.
    /// </summary>
    /// <param name="iRowIndex">One-based row index.</param>
    /// <returns>Row information.</returns>
    [CLSCompliant( false )]
    public static IOutline GetRowOutline( IInternalWorksheet sheet, int iRowIndex )
    {
      RowStorage storage = WorksheetHelper.GetOrCreateRow( sheet, iRowIndex - 1, false );
      return storage;//.RowInformation;
    }
    /// <summary>
    /// Updates FirstColumn and LastColumn indexes.
    /// </summary>
    /// <param name="iColumnIndex">Column that was accessed.</param>
    public static void AccessColumn( IInternalWorksheet sheet, int iColumnIndex )
    {
      int iFirstColumn = sheet.FirstColumn;
      int iLastColumn = sheet.LastColumn;

      if( iFirstColumn > iColumnIndex || iFirstColumn == WorksheetImpl.DEF_MIN_COLUMN_INDEX )
        sheet.FirstColumn = ( ushort )iColumnIndex;

      if( iLastColumn < iColumnIndex || iLastColumn == WorksheetImpl.DEF_MIN_COLUMN_INDEX )
        sheet.LastColumn = ( ushort )iColumnIndex;
    }
    /// <summary>
    /// Updates FirstRow and LastRow indexes.
    /// </summary>
    /// <param name="iRowIndex">Row to access.</param>
    public static void AccessRow( IInternalWorksheet sheet, int iRowIndex )
    {
      int iFirstRow = sheet.FirstRow;
      int iLastRow = sheet.LastRow;

      if( iFirstRow > iRowIndex || iFirstRow < 0 )
        sheet.FirstRow = iRowIndex;

      if( iLastRow < iRowIndex || iLastRow < 0 )
        sheet.LastRow = iRowIndex;
    }
    #endregion
  }
}
