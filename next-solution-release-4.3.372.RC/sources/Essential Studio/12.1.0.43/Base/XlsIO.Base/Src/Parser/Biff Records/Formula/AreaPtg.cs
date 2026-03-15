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

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains the reference to a cell range in the same sheet.
  /// </summary>
  [ Token ( FormulaToken.tArea1 ) ]
  [ Token ( FormulaToken.tArea2 ) ]
  [ Token ( FormulaToken.tArea3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class AreaPtg
    : Ptg
    , IRangeGetterToken
    , IToken3D
    , IRectGetter
  {
    #region Class members
    /// <summary>
    /// Index of first row (0..65535) or row offset (-32768..32767).
    /// </summary>
    private int m_iFirstRow;
    /// <summary>
    /// Index of last row (0..65535) or row offset (-32768..32767).
    /// </summary>
    private int m_iLastRow;
    /// <summary>
    /// Index of first column (0..255) or column offset (-128..127).
    /// </summary>
    private int m_iFirstColumn;
    /// <summary>
    /// Option flags of first row and first column.
    /// </summary>
    private byte m_firstOptions;
    /// <summary>
    /// Index of last column (0..255) or column offset (-128..127).
    /// </summary>
    private int m_iLastColumn;
    /// <summary>
    /// Option flags of last row and last column.
    /// </summary>
    private byte m_lastOptions;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public AreaPtg()
    {
    }
    ///// <summary>
    ///// Creates token by its string representation.
    ///// </summary>
    ///// <param name="strFormula">String representation of the formula.</param>
    ///// <exception cref="System.ArgumentException">
    ///// When specified string is not valid token string.
    ///// </exception>
    //public AreaPtg( string strFormula )
    //{
    //  Match m = FormulaUtil.CellRangeRegex.Match( strFormula );
    //  string col1 = m.Groups[ "Column1" ].Value;
    //  string row1 = m.Groups[ "Row1" ].Value;
    //  string col2 = m.Groups[ "Column2" ].Value;
    //  string row2 = m.Groups[ "Row2" ].Value;

    //  if( !m.Success )
    //    throw new ArgumentException();

    //  SetArea( 0, 0, row1, col1, row2, col2, false );
    //}
    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the formula.</param>
    /// <param name="book">Parent workbook.</param>
    /// <exception cref="System.ArgumentException">
    /// When specified string is not valid token string.
    /// </exception>
    public AreaPtg( string strFormula, IWorkbook book )
    {
      Match m = FormulaUtil.CellRangeRegex.Match( strFormula );
      string col1 = m.Groups[ "Column1" ].Value;
      string row1 = m.Groups[ "Row1" ].Value;
      string col2 = m.Groups[ "Column2" ].Value;
      string row2 = m.Groups[ "Row2" ].Value;

      if( !m.Success )
        throw new ArgumentException();

      SetArea( 0, 0, row1, col1, row2, col2, false, book );
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public AreaPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Creates copy of the token.
    /// </summary>
    /// <param name="ptg">Token to clone.</param>
    public AreaPtg( AreaPtg ptg )
    {
      m_iFirstRow = ptg.m_iFirstRow;
      m_iLastRow = ptg.m_iLastRow;
      m_iFirstColumn = ptg.m_iFirstColumn;
      m_firstOptions = ptg.m_firstOptions;
      m_iLastColumn = ptg.m_iLastColumn;
      m_lastOptions = ptg.m_lastOptions;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iFirstRow"></param>
    /// <param name="iFirstCol"></param>
    /// <param name="iLastRow"></param>
    /// <param name="iLastCol"></param>
    /// <param name="firstOptions"></param>
    /// <param name="lastOptions"></param>
    public AreaPtg( int iFirstRow, int iFirstCol, int iLastRow, int iLastCol,
      byte firstOptions, byte lastOptions )
    {
      m_iFirstRow = iFirstRow;
      m_iLastRow = iLastRow;
      m_iFirstColumn = iFirstCol;
      m_iLastColumn = iLastCol;
      m_firstOptions = firstOptions;
      m_lastOptions = lastOptions;
    }
    /// <summary>
    /// Creates token using strings that represents cell addresses.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains this token.</param>
    /// <param name="iCellColumn">Column index of the cell that contains this token.</param>
    /// <param name="strFirstRow">String representation of the first row.</param>
    /// <param name="strFirstColumn">String representation of the first column.</param>
    /// <param name="strLastRow">String representation of the last row.</param>
    /// <param name="strLastColumn">String representation of the last column.</param>
    /// <param name="bR1C1"></param>
    /// <param name="book">Parent workbook.</param>
    public AreaPtg( int iCellRow, int iCellColumn, string strFirstRow,
      string strFirstColumn, string strLastRow, string strLastColumn, bool bR1C1,
      IWorkbook book )
    {
//      if( strFirstRow == null )
//        throw new ArgumentNullException( "strFirstRow" );
//
//      if( strFirstRow.Length == 0 )
//        throw new ArgumentException( "strFirstRow - string cannot be empty" );
//
//      if( strFirstColumn == null )
//        throw new ArgumentNullException( "strFirstColumn" );
//
//      if( strFirstColumn.Length == 0 )
//        throw new ArgumentException( "strFirstColumn - string cannot be empty" );
//
//      if( strLastRow == null )
//        throw new ArgumentNullException( "strLastRow" );
//
//      if( strLastRow.Length == 0 )
//        throw new ArgumentException( "strLastRow - string cannot be empty" );
//
//      if( strLastColumn == null )
//        throw new ArgumentNullException( "strLastColumn" );
//
//      if( strLastColumn.Length == 0 )
//        throw new ArgumentException( "strLastColumn - string cannot be empty" );

      SetArea( iCellRow, iCellColumn, strFirstRow, strFirstColumn, strLastRow,
        strLastColumn, bR1C1, book );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Index of the first row (0..65535) or row offset (-32768..32767).
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }
    /// <summary>
    /// True if the first row index is relative.
    /// </summary>
    public bool IsFirstRowRelative
    {
      get
      {
        return RefPtg.IsRelative( m_firstOptions, RefPtg.RowBitMask );
      }
      set
      {
        m_firstOptions = RefPtg.SetRelative( m_firstOptions, RefPtg.RowBitMask, value );
      }
    }
    /// <summary>
    /// True if the first column index is relative.
    /// </summary>
    public bool IsFirstColumnRelative
    {
      get
      {
        return RefPtg.IsRelative( m_firstOptions, RefPtg.ColumnBitMask );
      }
      set
      {
        m_firstOptions = RefPtg.SetRelative( m_firstOptions, RefPtg.ColumnBitMask, value );
      }
    }
    /// <summary>
    /// Index to column (0..255) or column offset (-128..127).
    /// </summary>
    public int FirstColumn
    {
      get
      {
        return m_iFirstColumn;
      }
      set
      {
        m_iFirstColumn = value;
      }
    }
    /// <summary>
    /// Index of the last row (0..65535) or row offset (-32768..32767).
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
      set
      {
        m_iLastRow = value;
      }
    }
    /// <summary>
    /// True if the last row index is relative.
    /// </summary>
    public bool IsLastRowRelative
    {
      get
      {
        return RefPtg.IsRelative( m_lastOptions, RefPtg.RowBitMask );
      }
      set
      {
        m_lastOptions = RefPtg.SetRelative( m_lastOptions, RefPtg.RowBitMask, value );
      }
    }
    /// <summary>
    /// True if the last column index is relative.
    /// </summary>
    public bool IsLastColumnRelative
    {
      get
      {
        return RefPtg.IsRelative( m_lastOptions, RefPtg.ColumnBitMask );
      }
      set
      {
        m_lastOptions = RefPtg.SetRelative( m_lastOptions, RefPtg.ColumnBitMask, value );
      }
    }
    /// <summary>
    /// Index to column (0..255) or column offset (-128..127).
    /// </summary>
    public int LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
      set
      {
        m_iLastColumn = value;
      }
    }
    /// <summary>
    /// Options for the top left cell of the range.
    /// </summary>
    protected byte FirstOptions
    {
      get
      {
        return m_firstOptions;
      }
      set
      {
        m_firstOptions = value;
      }
    }
    /// <summary>
    /// Options for the bottom right cell of the range.
    /// </summary>
    protected byte LastOptions
    {
      get
      {
        return m_lastOptions;
      }
      set
      {
        m_lastOptions = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Parses specified column and row values and fills token
    /// fields with appropriate values.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains this token.</param>
    /// <param name="iCellColumn">Column index of the cell that contains this token.</param>
    /// <param name="column1">String representing left column of the area.</param>
    /// <param name="row1">String representing top row of the area.</param>
    /// <param name="column2">String representing right column of the area.</param>
    /// <param name="row2">String representing bottom row of the area.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <param name="book">Parent workbook.</param>
    protected void SetArea( int iCellRow, int iCellColumn, string row1, string column1,
      string row2, string column2, bool bR1C1, IWorkbook book )
    {
      bool bRelative;

      int firstColumn = RefPtg.GetColumnIndex( iCellColumn, column1, bR1C1, out bRelative );
      IsFirstColumnRelative = bRelative;

      int firstRow = RefPtg.GetRowIndex( iCellRow, row1, bR1C1, out bRelative );
      IsFirstRowRelative = bRelative;

      int lastColumn = RefPtg.GetColumnIndex( iCellColumn, column2, bR1C1, out bRelative );
      IsLastColumnRelative = bRelative;

      int lastRow = RefPtg.GetRowIndex( iCellRow, row2, bR1C1, out bRelative );
      IsLastRowRelative = bRelative;

      if( firstRow == -1 && lastRow == -1 )
      {
        firstRow = 0;
        lastRow = book.MaxRowCount - 1;
      }
      else if( firstColumn == -1 && lastColumn == -1 )
      {
        firstColumn = 0;
        lastColumn = book.MaxColumnCount - 1;
      }

      m_iFirstRow = firstRow;
      m_iLastRow = lastRow;
      m_iFirstColumn = firstColumn;
      m_iLastColumn = lastColumn;
    }

    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <returns>Reference index.</returns>
    public virtual int CodeToIndex()
    {
      return CodeToIndex( TokenCode );
    }
    /// <summary>
    /// Gets corresponding error code.
    /// </summary>
    /// <returns>Corresponding error code.</returns>
    public virtual FormulaToken GetCorrespondingErrorCode()
    {
      int index = CodeToIndex();
      return AreaErrorPtg.IndexToCode( index );
    }
    /// <summary>
    /// Indicates whether area covers whole single row. Read-only.
    /// </summary>
    protected bool IsWholeRow( IWorkbook book )
    {
      return FirstRow == LastRow && IsWholeRows( book );
    }
    /// <summary>
    /// Indicates whether area covers whole rows. Read-only.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    protected bool IsWholeRows( IWorkbook book )
    {
      return ( book != null ) ?
        IsFirstRowRelative == IsLastRowRelative && FirstColumn == 0 && LastColumn == book.MaxColumnCount - 1 :
        false;
    }
    /// <summary>
    /// Indicates whether area covers whole columns. Read-only.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    protected bool IsWholeColumns( IWorkbook book )
    {
      return ( book != null ) ?
        IsFirstColumnRelative == IsLastColumnRelative && FirstRow == 0 && LastRow == book.MaxRowCount - 1 :
        false;
    }
    /// <summary>
    /// Indicates whether area covers whole single column. Read-only.
    /// </summary>
    protected bool IsWholeColumn( IWorkbook book )
    {
      return FirstColumn == LastColumn && IsWholeColumns( book );
    }
    /// <summary>
    /// Converts incorrect area range to corresponding error ptg.
    /// </summary>
    public virtual AreaPtg ConvertToErrorPtg()
    {
      return new AreaErrorPtg( this );
    }
    #endregion

    #region Class infill methods
    /// <summary>
    /// Infill PTG structure.
    /// </summary>
    /// <param name="provider">Represents storage.</param>
    /// <param name="offset">Offset in storage.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public override void InfillPTG( DataProvider provider, ref int offset, ExcelVersion version )
    {
      if( version == ExcelVersion.Excel97to2003 )
      {
        m_iFirstRow = provider.ReadUInt16( offset );
        offset += 2;
        m_iLastRow = provider.ReadUInt16( offset );
        offset += 2;
        m_iFirstColumn = provider.ReadByte( offset++ );
        m_firstOptions = provider.ReadByte(  offset++ );
        m_iLastColumn = provider.ReadByte(  offset++ );
        m_lastOptions = provider.ReadByte(  offset++ );
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        m_iFirstRow = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;

        m_iLastRow = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;

        m_iFirstColumn = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;

        m_firstOptions = provider.ReadByte(  offset++ );

        m_iLastColumn = provider.ReadInt32(  offset );
        offset += ExcelConstants.IntSize;

        m_lastOptions = provider.ReadByte(  offset++ );
      }
      else
      {
        throw new NotImplementedException();
      }
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts specified index to token code.
    /// </summary>
    /// <param name="index">Index of the needed token.</param>
    /// <returns>Token that corresponds to the index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than one or greater than 3.
    /// </exception>
    public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tArea1;
        case 2: return FormulaToken.tArea2;
        case 3: return FormulaToken.tArea3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts specified token code to index.
    /// </summary>
    /// <param name="code">Token code for which index is required.</param>
    /// <returns>Index that corresponds to the code.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is not one of tArea1, tArea2, tArea3.
    /// </exception>
    public static int CodeToIndex( FormulaToken code )
    {
      switch( code )
      {
        case FormulaToken.tArea1: return 1;
        case FormulaToken.tArea2: return 2;
        case FormulaToken.tArea3: return 3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// Read-only. Size of the record.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          return 9;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          return 19;

        default:
          throw new ArgumentOutOfRangeException( "version" );
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
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization )
    {
      WorkbookImpl book = ( formulaUtil != null ) ?
        ( WorkbookImpl )formulaUtil.ParentWorkbook :
        null;

      bool bIsIsWholeRows = IsWholeRows( book );
      bool bIsWholeColumns = IsWholeColumns( book );

      if( bIsIsWholeRows && bR1C1 )
      {
        return RefPtg.GetR1C1Name( iRow, RefPtg.DEF_R1C1_ROW, FirstRow, IsFirstRowRelative ) + ":"
          + RefPtg.GetR1C1Name( iRow, RefPtg.DEF_R1C1_ROW, LastRow, IsFirstRowRelative );
      }
      else if( bIsWholeColumns && bR1C1 )
      {
        return RefPtg.GetR1C1Name( iColumn, RefPtg.DEF_R1C1_COLUMN, FirstColumn, IsFirstColumnRelative ) + ":"
          + RefPtg.GetR1C1Name( iColumn, RefPtg.DEF_R1C1_COLUMN, LastColumn, IsFirstColumnRelative );
      }

      if( bIsIsWholeRows )
      {
        string strRow = ( FirstRow + 1 ).ToString();
        return "$" + ( FirstRow + 1 ).ToString() + ":$" + ( LastRow + 1 ).ToString();
      }

      if( bIsWholeColumns )
      {
        string strColumn = RangeImpl.GetColumnName( FirstColumn + 1 );
        return "$" + RangeImpl.GetColumnName( FirstColumn + 1 ) + ":$" + RangeImpl.GetColumnName( LastColumn + 1 );
      }

      return
        RefPtg.GetCellName( iRow, iColumn, FirstRow, FirstColumn, IsFirstRowRelative, 
        IsFirstColumnRelative, bR1C1 ) + ":" +
        RefPtg.GetCellName( iRow, iColumn, LastRow, LastColumn, IsLastRowRelative, 
        IsLastColumnRelative, bR1C1 );
    }

    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );

      int iOffset = 1;

      if( version == ExcelVersion.Excel97to2003 )
      {
        if( m_iFirstRow > ushort.MaxValue || m_iLastRow > ushort.MaxValue
          || m_iFirstColumn > byte.MaxValue || m_iLastColumn > byte.MaxValue )
        {
          FormulaToken tokenCode = GetCorrespondingErrorCode();
          result[ 0 ] = ( byte )tokenCode;
        }

        BitConverter.GetBytes( ( ushort )m_iFirstRow ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.ShortSize;

        BitConverter.GetBytes( ( ushort )m_iLastRow ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.ShortSize;

        result[ iOffset++ ] = ( byte )m_iFirstColumn;
        result[ iOffset++ ] = m_firstOptions;
        result[ iOffset++ ] = ( byte )m_iLastColumn;
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        BitConverter.GetBytes( m_iFirstRow ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.IntSize;

        BitConverter.GetBytes( m_iLastRow ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.IntSize;

        BitConverter.GetBytes( m_iFirstColumn ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.IntSize;

        result[ iOffset++ ] = m_firstOptions;

        BitConverter.GetBytes( m_iLastColumn ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.IntSize;
      }

      result[ iOffset ] = m_lastOptions;
      return result;
    }

    /// <summary>
    /// Moves row by iRowOffset, iColumnOffset.
    /// </summary>
    /// <param name="iRowOffset">Row offset.</param>
    /// <param name="iColumnOffset">Column offset.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Updated token.</returns>
    public override Ptg Offset( int iRowOffset, int iColumnOffset, WorkbookImpl book )
    {
      AreaPtg result = ( AreaPtg )base.Offset( iRowOffset, iColumnOffset, book );
      
      int iRowIndex = IsFirstRowRelative ? FirstRow + iRowOffset : FirstRow;
      int iColIndex = IsFirstColumnRelative ? FirstColumn + iColumnOffset : FirstColumn;
      int iLastRow = IsLastRowRelative ? LastRow + iRowOffset : LastRow;
      int iLastCol = IsLastColumnRelative ? LastColumn + iColumnOffset : LastColumn;
      
      if( iRowIndex < 0 || iRowIndex > book.MaxRowCount - 1
        || iColIndex < 0 || iColIndex > book.MaxColumnCount - 1
        || iLastRow < 0 || iLastRow > book.MaxRowCount - 1
        || iLastCol < 0 || iLastCol > book.MaxColumnCount - 1 )
      {
        // This is wrong row or column index so we have to create RefErr.
        FormulaToken tokenCode = GetCorrespondingErrorCode();
        return FormulaUtil.CreatePtg( tokenCode, this );
      }

      result.FirstRow     = iRowIndex;
      result.FirstColumn  = iColIndex;
      result.LastRow      = iLastRow;
      result.LastColumn   = iLastCol;

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCurSheetIndex"></param>
    /// <param name="iTokenRow"></param>
    /// <param name="iTokenColumn"></param>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="rectSource"></param>
    /// <param name="iDestSheetIndex"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    public override Ptg Offset( int iCurSheetIndex, int iTokenRow, int iTokenColumn,
      int iSourceSheetIndex, Rectangle rectSource, int iDestSheetIndex,
      Rectangle rectDest, out bool bChanged, WorkbookImpl book )
    {
      AreaPtg result = ( AreaPtg )base.Offset( iCurSheetIndex, iTokenRow, iTokenColumn,
        iSourceSheetIndex, rectSource, iDestSheetIndex, rectDest, out bChanged, book );

      // If referenced area was moved...
      if( iCurSheetIndex == iSourceSheetIndex )
      {
        return MoveReferencedArea( iSourceSheetIndex, rectSource,
          iDestSheetIndex, rectDest, ref bChanged, book );
      }
      else
      {
        // If formula was moved into different worksheet...
        if( iCurSheetIndex == iDestSheetIndex && iSourceSheetIndex != iDestSheetIndex
          && RectangleContains( rectDest, iTokenRow, iTokenColumn ) )
        {
          int iRowOffset = rectDest.Top - rectSource.Top;
          int iColOffset = rectDest.Left - rectSource.Left;

          bChanged = true;
          return result.MoveIntoDifferentSheet( result, iSourceSheetIndex, rectSource, 
            iDestSheetIndex, iRowOffset, iColOffset );
        }
      }

      return result;

    }
    /// <summary>
    /// Converts tokens from regular formula into tokens from shared formula.
    /// </summary>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="iRow">Represents first row from cells range of shared formula.Zero-base.</param>
    /// <param name="iColumn">Represents first column from cells range of shared formula.Zero-based.</param>
    /// <returns>New token for shared formula.</returns>
    public override Ptg ConvertPtgToNPtg( IWorkbook parent, int iRow, int iColumn )
    {
      FormulaToken token = AreaNPtg.IndexToCode( CodeToIndex() );
      AreaNPtg result = ( AreaNPtg )FormulaUtil.CreatePtg( token );
      bool bWholeRow = IsWholeRows( parent );
      bool bWholeColumn = IsWholeColumns( parent );

      int iFirstRow = IsFirstRowRelative && !bWholeColumn ?
        ( FirstRow - iRow ) :
        FirstRow;

      short sFirstColumn = IsFirstColumnRelative && !bWholeRow ?
        ( short )( FirstColumn - iColumn ) :
        ( short )FirstColumn;

      int iLastRow = IsLastRowRelative && !bWholeColumn ?
        ( LastRow - iRow ) :
        LastRow;

      short sLastColumn = IsLastColumnRelative && !bWholeRow ?
        ( short )( LastColumn - iColumn ) :
        ( short )LastColumn;

      result.FirstRow = iFirstRow;
      result.FirstColumn = sFirstColumn;
      result.LastRow = iLastRow;
      result.LastColumn = sLastColumn;
      result.FirstOptions = FirstOptions;
      result.LastOptions = LastOptions;

      return result;
    }
    /// <summary>
    /// Converts full row or column ptg from Excel2007 to Excel97to03 version and vice versa.
    /// </summary>
    /// <returns>Returns converted Ptg.</returns>
    /// <param name="bFromExcel07To97">Defines what conversion must be applied.</param>
    public AreaPtg ConvertFullRowColumnAreaPtgs( bool bFromExcel07To97 )
    {
      int iExcel07MaxRowCount;
      int iExcel07ColumnCount;
      int iExcel97to03MaxRowCount;
      int iExcel97to03ColumnCount;

      UtilityMethods.GetMaxRowColumnCount( out iExcel07MaxRowCount, out iExcel07ColumnCount, ExcelVersion.Excel2007 );
      UtilityMethods.GetMaxRowColumnCount( out iExcel97to03MaxRowCount, out iExcel97to03ColumnCount, ExcelVersion.Excel97to2003 );

      if( bFromExcel07To97 )
      {
        if( FirstColumn == 0 && LastColumn == iExcel07ColumnCount - 1 )
        {
          LastColumn = iExcel97to03ColumnCount - 1;
        }
        else if( FirstRow == 0 && LastRow == iExcel07MaxRowCount - 1 )
        {
          LastRow = iExcel97to03MaxRowCount - 1;
        }
        else if( FirstColumn > iExcel97to03ColumnCount || LastColumn > iExcel97to03ColumnCount ||
          FirstRow > iExcel97to03MaxRowCount || LastRow > iExcel97to03MaxRowCount )
        {
          return ConvertToErrorPtg();
        }
      }
      else
      {
        if( FirstColumn == 0 && LastColumn == iExcel97to03ColumnCount - 1 )
        {
          LastColumn = iExcel07ColumnCount - 1;
        }
        else if( FirstRow == 0 && LastRow == iExcel97to03MaxRowCount - 1 )
        {
          LastRow = iExcel07MaxRowCount - 1;
        }
      }

      return this;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="rectSource"></param>
    /// <param name="iDestSheetIndex"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="iColOffset"></param>
    /// <returns></returns>
    private Ptg MoveIntoDifferentSheet( AreaPtg result,  int iSourceSheetIndex, 
      Rectangle rectSource, int iDestSheetIndex, int iRowOffset, int iColOffset )
    {
      int iSheetIndex = iSourceSheetIndex;
      bool bMoved = ReferencedAreaMoved( rectSource );

      int iFirstRow = result.FirstRow;
      int iFirstCol = result.FirstColumn;
      int iLastRow  = result.LastRow;
      int iLastCol  = result.LastColumn;

      if( bMoved )
      {
        iSheetIndex = iDestSheetIndex;
        iFirstRow += iRowOffset;
        iFirstCol += iColOffset;
        iLastRow += iRowOffset;
        iLastCol += iColOffset;
      }

      FormulaToken token = Area3DPtg.IndexToCode( CodeToIndex() );
      return FormulaUtil.CreatePtg( token, iSheetIndex, iFirstRow, iFirstCol,
        iLastRow, iLastCol, m_firstOptions, m_lastOptions );

      //      return result.UpdateReferencedCell( iSheetIndex, iSheetIndex, iRowIndex, iColIndex,
      //        ref bMoved );
    }

    /// <summary>
    /// Returns True if referenced cell was moved.
    /// </summary>
    /// <param name="rectSource"></param>
    /// <returns></returns>
    private bool ReferencedAreaMoved( Rectangle rectSource )
    {
      return RectangleContains( rectSource, FirstRow, FirstColumn ) && 
        RectangleContains( rectSource, LastRow, LastColumn );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCurSheetIndex"></param>
    /// <param name="iDestSheetIndex"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="iColOffset"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg UpdateReferencedArea( int iCurSheetIndex, int iDestSheetIndex,
      int iRowOffset, int iColOffset, ref bool bChanged, WorkbookImpl book )
    {
      if( m_iLastRow+ iRowOffset < 0 || m_iFirstColumn + iColOffset < 0
        || m_iLastRow + iRowOffset > book.MaxRowCount - 1
        || m_iLastColumn + iColOffset > book.MaxColumnCount - 1 )
      {
        // This is wrong row or column index so we have to create AreaErr.
        FormulaToken tokenCode = GetCorrespondingErrorCode();
        return FormulaUtil.CreatePtg( tokenCode, ToString( book.FormulaUtil ), book );
      }

      if( iCurSheetIndex == iDestSheetIndex )
      {
        //result = ( RefPtg )result.Clone();
        m_iFirstRow  = m_iFirstRow + iRowOffset;
        m_iLastRow   = m_iLastRow + iRowOffset;
        m_iFirstColumn = m_iFirstColumn + iColOffset;
        m_iLastColumn  = m_iLastColumn + iColOffset;
        bChanged = true;
      }
      else
      {
        bChanged = true;
        FormulaToken token = Area3DPtg.IndexToCode( CodeToIndex() );
        return FormulaUtil.CreatePtg( token, iDestSheetIndex,
          m_iFirstRow + iRowOffset, m_iFirstColumn + iColOffset,
          m_iLastRow + iRowOffset, m_iLastColumn + iColOffset,
          m_firstOptions, m_lastOptions );
        //new Ref3DPtg( iDestSheetIndex, iRowIndex, iColIndex, CodeToIndex() );
      }

      return this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCurSheetIndex"></param>
    /// <param name="iDestSheetIndex"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="iColOffset"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg UpdateFirstCell( int iCurSheetIndex, int iDestSheetIndex,
      int iRowOffset, int iColOffset, ref bool bChanged, IWorkbook book )
    {
      int iFirstRow  = m_iFirstRow + iRowOffset;
      int iFirstColumn = m_iFirstColumn + iColOffset;

      if( iFirstRow < 0 || iFirstColumn < 0 
        || iFirstRow > book.MaxRowCount - 1
        || iFirstColumn> book.MaxColumnCount - 1 )
      {
        // This is wrong row or column index so we have to create AreaErr.
        FormulaToken tokenCode = GetCorrespondingErrorCode();
        return FormulaUtil.CreatePtg( tokenCode, ToString() );
      }

      if( iFirstRow > m_iLastRow || iFirstColumn > m_iLastColumn )
        return this;

      if( iCurSheetIndex == iDestSheetIndex )
      {
        //result = ( RefPtg )result.Clone();

        m_iFirstRow = iFirstRow;
        m_iFirstColumn = iFirstColumn;
        bChanged = true;
      }
      else
      {
        bChanged = true;
        FormulaToken token = Area3DPtg.IndexToCode( CodeToIndex() );

        return FormulaUtil.CreatePtg( token, iDestSheetIndex,
          iFirstRow, iFirstColumn, m_iLastRow, m_iLastColumn,
          m_firstOptions, m_lastOptions );
        //new Ref3DPtg( iDestSheetIndex, iRowIndex, iColIndex, CodeToIndex() );
      }

      return this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCurSheetIndex"></param>
    /// <param name="iDestSheetIndex"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="iColOffset"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg UpdateLastCell( int iCurSheetIndex, int iDestSheetIndex,
      int iRowOffset, int iColOffset, ref bool bChanged, IWorkbook book )
    {
      int iLastRow  = m_iLastRow + iRowOffset;
      int iLastColumn = m_iLastColumn + iColOffset;

      if( iLastRow < 0 || iLastColumn < 0 
        || iLastRow > book.MaxRowCount - 1
        || iLastColumn> book.MaxColumnCount - 1 )
      {
        // This is wrong row or column index so we have to create AreaErr.
        FormulaToken tokenCode = GetCorrespondingErrorCode();
        return FormulaUtil.CreatePtg( tokenCode, ToString() );
      }

      if( iLastRow < m_iFirstRow || iLastColumn < m_iFirstColumn )
        return this;

      if( iCurSheetIndex == iDestSheetIndex )
      {
        //result = ( RefPtg )result.Clone();

        m_iLastRow = iLastRow;
        m_iLastColumn = iLastColumn;
        bChanged = true;
      }
      else
      {
        bChanged = true;
        FormulaToken token = Area3DPtg.IndexToCode( CodeToIndex() );

        return FormulaUtil.CreatePtg( token, iDestSheetIndex,
          m_iFirstRow, m_iFirstColumn, iLastRow, iLastColumn,
          m_firstOptions, m_lastOptions );
        //new Ref3DPtg( iDestSheetIndex, iRowIndex, iColIndex, CodeToIndex() );
      }

      return this;
    }
    /// <summary>
    /// Checks if while moving rectSource full top row was moved.
    /// </summary>
    /// <param name="rectSource">Rectangle that was moved.</param>
    /// <returns>True if whole top row was moved.</returns>
    private bool FullFirstRowMove( Rectangle rectSource )
    {
      return RectangleContains( rectSource, FirstRow, FirstColumn )
        && RectangleContains( rectSource, FirstRow, LastColumn );
    }
    /// <summary>
    /// Checks if while moving rectSource full bottom row was moved.
    /// </summary>
    /// <param name="rectSource">Rectangle that was moved.</param>
    /// <returns>True if whole bottom row was moved.</returns>
    private bool FullLastRowMove( Rectangle rectSource )
    {
      return RectangleContains( rectSource, LastRow, FirstColumn )
        && RectangleContains( rectSource, LastRow, LastColumn );
    }
    /// <summary>
    /// Checks if while moving rectSource full left row was moved.
    /// </summary>
    /// <param name="rectSource">Rectangle that was moved.</param>
    /// <returns>True if whole left row was moved.</returns>
    private bool FullFirstColMove( Rectangle rectSource )
    {
      return RectangleContains( rectSource, FirstRow, FirstColumn )
        && RectangleContains( rectSource, LastRow, FirstColumn );
    }
    /// <summary>
    /// Checks if while moving rectSource full right row was moved.
    /// </summary>
    /// <param name="rectSource">Rectangle that was moved.</param>
    /// <returns>True if whole right row was moved.</returns>
    private bool FullLastColMove( Rectangle rectSource )
    {
      return RectangleContains( rectSource, FirstRow, LastColumn )
        && RectangleContains( rectSource, LastRow, LastColumn );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="rectSource"></param>
    /// <param name="iDestSheetIndex"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg MoveReferencedArea( int iSourceSheetIndex, Rectangle rectSource,
      int iDestSheetIndex, Rectangle rectDest, ref bool bChanged, WorkbookImpl book )
    {
      int iRowOffset = rectDest.Top - rectSource.Top;
      int iRowSourceOffset = rectSource.Top - rectDest.Top;
      int iColOffset = rectDest.Left - rectSource.Left;

      if( iRowOffset == 0 && iColOffset == 0 && iSourceSheetIndex == iDestSheetIndex )
        return this;

      rectSource = Rectangle.FromLTRB(rectSource.Left, rectSource.Top,
        m_iLastColumn, m_iLastRow);

      rectDest = Rectangle.FromLTRB(rectDest.Left, rectDest.Top,
       m_iLastColumn, m_iLastRow + 1);


      bool bFirstCell = iRowOffset <=0 ?
                        RefPtg.RectangleContains( rectDest, FirstRow, FirstColumn )
                        : RefPtg.RectangleContains(rectSource, FirstRow, FirstColumn);
      bool bLastCell = iRowSourceOffset <= 0?
                        RefPtg.RectangleContains(rectSource, LastRow, LastColumn)
                        : RefPtg.RectangleContains(rectDest, LastRow, LastColumn);

      if (m_iLastRow >= book.MaxRowCount-1 || m_iLastColumn>=book.MaxColumnCount-1)
      {
          bFirstCell = false;
          bLastCell = false;
      }
                        
                        

      if( !bFirstCell && !bLastCell )
      {
          //Zero base index.
          if (book.MaxRowCount-1 == LastRow && FirstRow == 0)
              return this;
          Rectangle formulaRectangle = Rectangle.FromLTRB(FirstColumn, FirstRow, LastColumn, LastRow);
          bool bRangeFirstCell = RefPtg.RectangleContains(formulaRectangle, rectDest.Top, formulaRectangle.Left);

          bool bRangeLastCell = RefPtg.RectangleContains(formulaRectangle, rectDest.Bottom, formulaRectangle.Right);
                            
          if (!(bRangeFirstCell && bRangeLastCell))
            return this;

      }
      if( iColOffset == 0 && iSourceSheetIndex == iDestSheetIndex )
      {
        return VerticalMove( iSourceSheetIndex, rectSource, iRowOffset,
          rectDest, ref bChanged, book );
      }
      else if ( iRowOffset == 0 && iSourceSheetIndex == iDestSheetIndex )
      {
        return HorizontalMove( iSourceSheetIndex, rectSource, iColOffset,
          rectDest, ref bChanged, book );
      }
      else if( bFirstCell || bLastCell )
      {
        return UpdateReferencedArea( iSourceSheetIndex, iDestSheetIndex,
          iRowOffset, iColOffset, ref bChanged, book );
      }

      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="rectSource"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg VerticalMove( int iSourceSheetIndex, Rectangle rectSource,
      int iRowOffset, Rectangle rectDest, ref bool bChanged, WorkbookImpl book )
    {
      bool bFirstRow = iRowOffset<0?
                        FullFirstRowMove( rectSource )
                        :FullFirstRowMove(rectDest);

      if (rectSource.X <= rectDest.X && iRowOffset >0 && bFirstRow != true)
          bFirstRow = !FullFirstRowMove(rectDest);     

      if (m_iFirstRow >= rectSource.Y)
          bFirstRow = true;
      else
          bFirstRow = false;
		  
      bool bLastRow = iRowOffset<0?
                        FullLastRowMove( rectDest )
                        :FullLastRowMove(rectSource);
      Rectangle rectArea = Rectangle.FromLTRB( FirstColumn, FirstRow, LastColumn, LastRow );

      if( bFirstRow && bLastRow )
      {
        return UpdateReferencedArea( iSourceSheetIndex, iSourceSheetIndex,
          iRowOffset, 0, ref bChanged, book );
      }
      else if( bFirstRow )
      {
        return FirstRowVerticalMove( iSourceSheetIndex, iRowOffset, rectSource,
          rectDest, ref bChanged, book );
      }
      else if( bLastRow )
      {
        return LastRowVerticalMove( iSourceSheetIndex, iRowOffset, rectSource,
          rectDest, ref bChanged, book );
      }
      else if( !Rectangle.Intersect( rectArea, rectDest ).IsEmpty )
      {
        if( LastRow == rectDest.Bottom || FirstRow == rectDest.Top ) return this;

        if( iRowOffset < 0 )
        {
          LastRow = ( ushort )( rectDest.Top - 1 );
        }
        else
        {
          FirstRow = ( ushort )( rectDest.Bottom + 1 );
        }
      }

      return this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="rectSource"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg FirstRowVerticalMove( int iSourceSheetIndex, int iRowOffset,
      Rectangle rectSource, Rectangle rectDest, ref bool bChanged, IWorkbook book )
    {
      if( iRowOffset < 0 )
      {
        return UpdateFirstCell( iSourceSheetIndex, iSourceSheetIndex,
          iRowOffset, 0, ref bChanged, book );
      }
      else if( FirstRow + iRowOffset <= LastRow )
      {
        if( rectDest.Top <= rectSource.Bottom )
        {
          FirstRow = ( ushort )( rectSource.Top + iRowOffset );
        }
        else
        {
          FirstRow = ( ushort )( FirstRow + iRowOffset );
        }

        return this;
      }

      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="iRowOffset"></param>
    /// <param name="rectSource"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg LastRowVerticalMove( int iSourceSheetIndex, int iRowOffset,
      Rectangle rectSource, Rectangle rectDest, ref bool bChanged, IWorkbook book )
    {
      if( iRowOffset > 0 )
      {
        return UpdateLastCell( iSourceSheetIndex, iSourceSheetIndex, iRowOffset,
          0, ref bChanged, book );
      }
      else if( LastRow + iRowOffset >= FirstRow )
      {
        //if( rectDest.Bottom <= rectSource.Top )
        //{
        //  LastRow = ( ushort )( rectDest.Bottom - 1 );
        //}
        //else
        {
          LastRow = ( ushort )( LastRow + iRowOffset );
        }
      }

      return this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="rectSource"></param>
    /// <param name="iColOffset"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg HorizontalMove( int iSourceSheetIndex, Rectangle rectSource,
      int iColOffset, Rectangle rectDest, ref bool bChanged, WorkbookImpl book )
    {
      bool bFirstCol = FullFirstColMove( rectSource );
      bool bLastCol = FullLastColMove( rectSource );
      Rectangle rectArea = Rectangle.FromLTRB( FirstColumn, FirstRow, LastColumn, LastRow );

      if( bFirstCol && bLastCol )
      {
        return UpdateReferencedArea( iSourceSheetIndex, iSourceSheetIndex, 0,
          iColOffset, ref bChanged, book );
      }
      else if( bFirstCol )
      {
        return FirstColumnHorizontalMove( iSourceSheetIndex, iColOffset, rectSource,
          rectDest, ref bChanged, book );
      }
      else if( bLastCol )
      {
        return LastColumnHorizontalMove( iSourceSheetIndex, iColOffset, rectSource,
          rectDest, ref bChanged, book );
      }
      else if (!bFirstCol && !bLastCol)
      {
          return this;
      }
      else if (!Rectangle.Intersect(rectArea, rectDest).IsEmpty)
      {
          // if rectSource doesn't contain rectArea &&
          // rectDest contains it fully then it should be cleared -> changed to area error.

          if (InsideRectangle(rectDest, rectArea) && OutsideRectangle(rectSource, rectArea))
              return ConvertToErrorPtg();//new AreaErrorPtg( this );

          if (LastColumn == rectDest.Right || FirstColumn == rectDest.Left)
              return this;

          if (iColOffset < 0)
          {
              LastColumn = (byte)(rectDest.Left - 1);
          }
          else
          {
              FirstColumn = (byte)(rectDest.Right + 1);
          }
      }

      return this;
    }
    /// <summary>
    /// Checks whether one rectangle is outside of another one.
    /// </summary>
    /// <param name="owner">First rectangle to check.</param>
    /// <param name="toCheck">Second rectangle to check.</param>
    /// <returns>True if toCheck rectangle is outside of owner rectangle.</returns>
    private bool OutsideRectangle( Rectangle owner, Rectangle toCheck )
    {
      return owner.Top > toCheck.Bottom || owner.Bottom < toCheck.Top ||
        owner.Left > toCheck.Right || owner.Right < toCheck.Left;
    }
    /// <summary>
    /// Checks whether one rectangle is inside of another one.
    /// </summary>
    /// <param name="owner">Rectangle that could contain another one.</param>
    /// <param name="toCheck">Rectangle that could be contained by another one.</param>
    /// <returns>True if toCheck rectangle is contained by owner rectangle.</returns>
    private bool InsideRectangle( Rectangle owner, Rectangle toCheck )
    {
      return owner.Left <= toCheck.Left && owner.Right >= toCheck.Right &&
          owner.Top <= toCheck.Top && owner.Bottom >= toCheck.Bottom;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="iColOffset"></param>
    /// <param name="rectSource"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg FirstColumnHorizontalMove( int iSourceSheetIndex, int iColOffset,
      Rectangle rectSource, Rectangle rectDest, ref bool bChanged, IWorkbook book )
    {
      if( iColOffset < 0 )
      {
        return UpdateFirstCell( iSourceSheetIndex, iSourceSheetIndex,
          0, iColOffset, ref bChanged, book );
      }
      else if( FirstColumn + iColOffset <= LastColumn )
      {
        if( rectDest.Left <= rectSource.Right )
        {
          FirstColumn = ( byte )( rectSource.Right + 1 );
        }
        else
        {
          FirstColumn = ( byte )( FirstColumn + iColOffset );
        }

        return this;
      }

      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceSheetIndex"></param>
    /// <param name="iColOffset"></param>
    /// <param name="rectSource"></param>
    /// <param name="rectDest"></param>
    /// <param name="bChanged"></param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    private Ptg LastColumnHorizontalMove( int iSourceSheetIndex, int iColOffset,
      Rectangle rectSource, Rectangle rectDest, ref bool bChanged, IWorkbook book )
    {
      if( iColOffset > 0 )
      {
        return UpdateLastCell( iSourceSheetIndex, iSourceSheetIndex, 0,
          iColOffset, ref bChanged, book );
      }
      else if( LastColumn + iColOffset >= FirstColumn )
      {
        if( rectDest.Right <= rectSource.Left )
        {
          LastColumn = ( byte )( rectSource.Left + 1 );
        }
        else
        {
          LastColumn = ( byte )( LastColumn + iColOffset );
        }
      }

      return this;
    }
    #endregion

    #region IRangeGetter Members
    /// <summary>
    /// Returns range represented by the token that implements this interface.
    /// </summary>
    /// <param name="book">Workbook that contains range.</param>
    /// <param name="sheet">Worksheet that contains range.</param>
    /// <returns>Range represented by the token.</returns>
    public IRange GetRange( IWorkbook book, IWorksheet sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if (FirstColumn > LastColumn)
      {
          int tmp = LastColumn;
          LastColumn = FirstColumn;
          FirstColumn = tmp;
      }
      return sheet[FirstRow + 1, FirstColumn + 1, LastRow + 1, LastColumn + 1];
      //return sheet[FirstRow + 1, firstcolumn + 1, LastRow + 1, lastcolumn + 1];
    }
    /// <summary>
    /// Returns rectangle represented by the token that implements this interface.
    /// All coordinates are zero-based.
    /// </summary>
    /// <returns>Rectangle represented by the token.</returns>
    public Rectangle GetRectangle()
    {
      return Rectangle.FromLTRB( FirstColumn, FirstRow, LastColumn, LastRow );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public Ptg UpdateRectangle( Rectangle rectangle )
    {
      AreaPtg result = ( AreaPtg )Clone();
      result.FirstColumn = rectangle.Left;
      result.LastColumn = rectangle.Right;
      result.FirstRow = rectangle.Top;
      result.LastRow = rectangle.Bottom;

      return result;
    }
    /// <summary>
    /// Converts current token into error token.
    /// </summary>
    /// <returns>Created token.</returns>
    public virtual Ptg ConvertToError()
    {
      FormulaToken tokenCode = GetCorrespondingErrorCode();
      return FormulaUtil.CreatePtg( tokenCode, this );
    }
    #endregion

    #region IToken3D Members

    /// <summary>
    /// Converts current token to the 3D token.
    /// </summary>
    /// <param name="iSheetReference">Reference to the worksheet.</param>
    /// <returns>Created token.</returns>
    public Ptg Get3DToken( int iSheetReference )
    {
      int iIndex = CodeToIndex( TokenCode );
      FormulaToken tokenCode = Area3DPtg.IndexToCode( iIndex );

      Ptg result = new Area3DPtg( iSheetReference, FirstRow, FirstColumn, LastRow,
        LastColumn, FirstOptions, LastOptions );

      result.TokenCode = tokenCode;
      return result;
    }

    #endregion
  }
}
