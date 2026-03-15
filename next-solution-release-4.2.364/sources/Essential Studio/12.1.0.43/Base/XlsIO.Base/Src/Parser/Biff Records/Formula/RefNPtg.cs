#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using Syncfusion.XlsIO.Implementation;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains the reference to a cell in the same sheet. It stores relative
  /// components as signed offsets and is used in shared formulas, conditional formatting,
  /// and data validity.
  /// </summary>
  [ Token ( FormulaToken.tRefN1 ) ]
  [ Token ( FormulaToken.tRefN2 ) ]
  [ Token ( FormulaToken.tRefN3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class RefNPtg : RefPtg
  {
    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public RefNPtg()
    {
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public RefNPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Constructs reference by its string representation.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="strRow">String representation of the row.</param>
    /// <param name="strColumn">String representation of the column.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation was used.</param>
    public RefNPtg( int iCellRow, int iCellColumn, string strRow, string strColumn, bool bR1C1 )
    {
      SetCell( iCellRow, iCellColumn, strRow, strColumn, bR1C1 );

      ColumnIndex -= iCellColumn ;
      RowIndex -= iCellRow;
      base.IsRowIndexRelative = true;
      base.IsColumnIndexRelative = true;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Converts token from shared formula into token from regular formula.
    /// </summary>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    /// <returns>New token for regular formula.</returns>
    public override Ptg ConvertSharedToken( IWorkbook parent, int iRow, int iColumn )
    {
      int iColumnIndex = ( parent.Version == ExcelVersion.Excel97to2003 ) ?
        ColumnIndex :
        base.ColumnIndex;

      int iRowIndex = ( parent.Version == ExcelVersion.Excel97to2003 ) ?
        RowIndex :
        base.RowIndex;

      int iNewColumn = IsColumnIndexRelative ? iColumn + iColumnIndex : iColumnIndex;
      int iNewRow = IsRowIndexRelative ? iRow + iRowIndex : iRowIndex;

      if( parent.Version == ExcelVersion.Excel97to2003 )
      {
        iNewColumn = ( byte )iNewColumn;
        iNewRow = ( ushort )iNewRow;
      }

      Ptg result = new RefPtg( iNewRow, iNewColumn, Options );
      int index = RefNPtg.CodeToIndex( TokenCode );
      result.TokenCode = RefPtg.IndexToCode( index );

      return result;
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization )
    {
        short columnCount = 256;
        short signedRowIndex = (short)(RowIndex);
        short signedColIndex = (short)(ColumnIndex);
        if (signedColIndex >= 255)
            signedColIndex = (short)(signedColIndex - columnCount);
        int iUpdatedRow = IsRowIndexRelative ?
         Math.Abs(iRow + signedRowIndex - 1) :
         RowIndex ;

        int iUpdatedColumn = IsColumnIndexRelative ?
          Math.Abs(iColumn + signedColIndex - 1) :
          ColumnIndex;

        return GetCellName(iRow, iColumn, iUpdatedRow, iUpdatedColumn, IsRowIndexRelative,
          IsColumnIndexRelative, bR1C1);
    }
    /// <summary>
    /// Converts current token to the 3D token.
    /// </summary>
    /// <param name="iSheetReference">Reference to the worksheet.</param>
    /// <returns>Created token.</returns>
    public override Ptg Get3DToken(int iSheetReference)
    {
        int iIndex = CodeToIndex(TokenCode);
        FormulaToken tokenCode = Ref3DPtg.IndexToCode(iIndex);

        Ptg result = new Ref3DPtg(iSheetReference, RowIndex, ColumnIndex, Options);
        result.TokenCode = tokenCode;
        return result;
    }
    #endregion

    #region Class properties
    //  Index should support -65536 to 65536
    ///// <summary>
    ///// Returns relative row index (-32768..32767).
    ///// </summary>
    //new public int RowIndex
    //{
    //    get
    //    {
    //        return (short)base.RowIndex;
    //    }
    //    set
    //    {
    //        base.RowIndex = (short)value;
    //    }
    //}
    ///// <summary>
    ///// Returns relative row index (-32768..32767).
    ///// </summary>
    //new public int ColumnIndex
    //{
    //    get
    //    {
    //        return (short)base.ColumnIndex;
    //    }
    //    set
    //    {
    //        base.ColumnIndex = (short)value;
    //    }
    //}
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <param name="token">Token code (should be one of tRefN1, tRefN2, tRefN3).</param>
    /// <returns>Reference index.</returns>
    new public static int CodeToIndex( FormulaToken token )
    {
      switch( token )
      {
        case FormulaToken.tRefN1: return 1;
        case FormulaToken.tRefN2: return 2;
        case FormulaToken.tRefN3: return 3;

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
        case 1: return FormulaToken.tRefN1;
        case 2: return FormulaToken.tRefN2;
        case 3: return FormulaToken.tRefN3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion
  }
}
