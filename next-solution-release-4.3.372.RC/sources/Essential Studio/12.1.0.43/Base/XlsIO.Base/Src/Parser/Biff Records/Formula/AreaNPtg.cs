#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.Text.RegularExpressions;

using Syncfusion.XlsIO.Implementation;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains the reference to a cell range in the same sheet.
  /// </summary>
  [ Token ( FormulaToken.tAreaN1 ) ]
  [ Token ( FormulaToken.tAreaN2 ) ]
  [ Token ( FormulaToken.tAreaN3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class AreaNPtg : AreaPtg
  {
    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating tokens without
    /// parameters and to allow descendants.
    /// </summary>
    public AreaNPtg() : base()
    {
    }
    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the formula.</param>
    /// <param name="book">Parent workbook.</param>
    /// <exception cref="System.ArgumentException">
    /// When specified string is not a valid token string.
    /// </exception>
    public AreaNPtg( string strFormula, IWorkbook book ) : base( strFormula, book )
    {
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public AreaNPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// First column offset (-128..127).
    /// </summary>
    new public short FirstColumn
    {
      get
      {
        return ( short )( ushort )base.FirstColumn;
      }
      set
      {
        base.FirstColumn = ( ushort )value;
      }
    }
    /// <summary>
    /// Column offset (-128..127).
    /// </summary>
    new public short LastColumn
    {
      get
      {
        return ( short )( ushort )base.LastColumn;
      }
      set
      {
        base.LastColumn = ( ushort )value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Converts token from shared formula into token from regular formula.
    /// </summary>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    /// <returns>New token for regular formula.</returns>
    public override Ptg ConvertSharedToken( IWorkbook parent, int iRow, int iColumn )
    {
      bool bWholeRows = IsWholeRows( parent );
      bool bWholeColumns = IsWholeColumns( parent );

      int iNewFirstColumn = IsFirstColumnRelative && !bWholeRows ?
        iColumn + FirstColumn:
        FirstColumn;
      
      int iNewFirstRow = IsFirstRowRelative && !bWholeColumns ?
        iRow + FirstRow :
        FirstRow;

      int iNewLastColumn = IsLastColumnRelative && !bWholeRows ?
        iColumn + LastColumn :
        LastColumn;

      int iNewLastRow = IsLastRowRelative && !bWholeColumns?
        iRow + LastRow :
        LastRow;

      if( parent.Version == ExcelVersion.Excel97to2003 )
      {
        iNewFirstColumn = ( byte )iNewFirstColumn;
        iNewLastColumn = ( byte )iNewLastColumn;
        iNewFirstRow = ( ushort )iNewFirstRow;
        iNewLastRow = ( ushort )iNewLastRow;
      }

      Ptg result = new AreaPtg( iNewFirstRow, iNewFirstColumn, iNewLastRow, iNewLastColumn,
        FirstOptions, LastOptions );

      int index = CodeToIndex( TokenCode );
      result.TokenCode = AreaPtg.IndexToCode( index );

      return result;
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <param name="token">Token code (should be one of tAreaN1, tAreaN2, tAreaN3).</param>
    /// <returns>Reference index.</returns>
    new public static int CodeToIndex( FormulaToken token )
    {
      switch( token )
      {
        case FormulaToken.tAreaN1: return 1;
        case FormulaToken.tAreaN2: return 2;
        case FormulaToken.tAreaN3: return 3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts reference index to token code.
    /// </summary>
    /// <param name="index">Reference index.</param>
    /// <returns>Token code.</returns>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1:
          return FormulaToken.tAreaN1;
        case 2:
          return FormulaToken.tAreaN2;
        case 3:
          return FormulaToken.tAreaN3;

        default:
          throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString(FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization)
    {
        WorkbookImpl book = (formulaUtil != null) ?
          (WorkbookImpl)formulaUtil.ParentWorkbook :
          null;

        bool bIsIsWholeRows = IsWholeRows(book);
        bool bIsWholeColumns = IsWholeColumns(book);

        if (bIsIsWholeRows && bR1C1)
        {
            return RefPtg.GetR1C1Name(iRow, RefPtg.DEF_R1C1_ROW, FirstRow, IsFirstRowRelative) + ":"
              + RefPtg.GetR1C1Name(iRow, RefPtg.DEF_R1C1_ROW, LastRow, IsFirstRowRelative);
        }
        else if (bIsWholeColumns && bR1C1)
        {
            return RefPtg.GetR1C1Name(iColumn, RefPtg.DEF_R1C1_COLUMN, FirstColumn, IsFirstColumnRelative) + ":"
              + RefPtg.GetR1C1Name(iColumn, RefPtg.DEF_R1C1_COLUMN, LastColumn, IsFirstColumnRelative);
        }

        if (bIsIsWholeRows)
        {
            string strRow = (FirstRow + 1).ToString();
            return "$" + (FirstRow + 1).ToString() + ":$" + (LastRow + 1).ToString();
        }

        if (bIsWholeColumns)
        {
            string strColumn = RangeImpl.GetColumnName(FirstColumn + 1);
            return "$" + RangeImpl.GetColumnName(FirstColumn + 1) + ":$" + RangeImpl.GetColumnName(LastColumn + 1);
        }
        int firstRow = IsFirstRowRelative ?
                       GetUpdatedRowIndex(iRow, FirstRow,true)
                       : FirstRow;
        int lastRow = IsLastRowRelative ?
                      GetUpdatedRowIndex(iRow, LastRow,false)
                      : LastRow;
        int firstColumn = IsFirstColumnRelative ?
                          GetUpdatedColumnIndex(iColumn, FirstColumn,true)
                          : FirstColumn;
        int lastColumn = IsLastColumnRelative ?
                         GetUpdatedColumnIndex(iColumn, LastColumn,false)
                        : LastColumn;

        if (FirstColumn == LastColumn)
            lastColumn = firstColumn;

        if (FirstRow == LastRow)
            lastRow = firstRow;

        return
          RefNPtg.GetCellName(iRow,iColumn,firstRow,firstColumn, IsFirstRowRelative,
          IsFirstColumnRelative, bR1C1) + ":" +
          RefNPtg.GetCellName(iRow, iColumn, lastRow,lastColumn, IsLastRowRelative,
          IsLastColumnRelative, bR1C1);
    }
    /// <summary>
    /// Gets the index of the updated row.
    /// </summary>
    /// <param name="iRow">The i row.</param>
    /// <param name="row">The row.</param>
    /// <param name="isFirst">if set to <c>true</c> [is first].</param>
    /// <returns></returns>
    private int GetUpdatedRowIndex(int iRow, int row,bool isFirst)
    {
        int updateRowIndex=0;
        if (iRow == 0 && row == 0)
            return updateRowIndex;
        bool check = isFirst ?
            row <= iRow || iRow>= row || iRow==0
            : iRow <= row || row >= iRow || row == 0;  

            if (check)
                updateRowIndex = (row + iRow) - 1;
            else
                updateRowIndex = iRow - (65536 - row);
       

        return updateRowIndex;        
    }
    /// <summary>
    /// Gets the index of the updated column.
    /// </summary>
    /// <param name="iColumn">The i column.</param>
    /// <param name="column">The column.</param>
    /// <param name="isFirst">if set to <c>true</c> [is first].</param>
    /// <returns></returns>
    private int GetUpdatedColumnIndex(int iColumn, int column,bool isFirst)
    {
        int updatedColumnIndex = 0;
        if (iColumn == 0 && column == 0)
            return updatedColumnIndex;
        bool check= isFirst ?
            column <= iColumn || iColumn>=column || iColumn==0
            :( iColumn <= column || column >= iColumn )&& column==0;

        if (check)
            updatedColumnIndex = (column + iColumn) - 1;
        else
            updatedColumnIndex = ((column + iColumn) - 256) - 1;

        return updatedColumnIndex;
    }
    #endregion
  }
}
