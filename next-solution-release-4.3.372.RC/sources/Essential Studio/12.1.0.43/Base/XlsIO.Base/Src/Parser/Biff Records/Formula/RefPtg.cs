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

using System;
using Syncfusion.XlsIO.Implementation;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif (WP)
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
  /// This token contains an unsigned 16-bit integer value in the range from 0 to 65535.
  /// </summary>
  [ Token ( FormulaToken.tRef1 ) ]
  [ Token ( FormulaToken.tRef2 ) ]
  [ Token ( FormulaToken.tRef3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class RefPtg
    : Ptg
    , IRangeGetterToken
    , IToken3D
    , IRectGetter
  {
    #region Class constants
    /// <summary>
    /// Bit mask for row options.
    /// </summary>
    public const byte RowBitMask = 0x80;
    /// <summary>
    /// Bit mask for column options.
    /// </summary>
    public const byte ColumnBitMask = 0x40;
    /// <summary>
    /// Opening bracket for relative cell coordinates in R1C1 notation.
    /// </summary>
    private const char DEF_R1C1_OPEN_BRACKET = '[';
    /// <summary>
    /// Closing bracket for relative cell coordinates in R1C1 notation.
    /// </summary>
    private const char DEF_R1C1_CLOSE_BRACKET = ']';
    /// <summary>
    /// Starting row character in R1C1 notation.
    /// </summary>
    public const string DEF_R1C1_ROW = "R";
    /// <summary>
    /// Starting column character in R1C1 notation.
    /// </summary>
    public const string DEF_R1C1_COLUMN = "C";
    /// <summary>
    /// Opening bracket for relative indexes in R1C1 notation.
    /// </summary>
    public const char DEF_OPEN_BRACKET = '[';
    /// <summary>
    /// Closing bracket for relative indexes in R1C1 notation.
    /// </summary>
    public const char DEF_CLOSE_BRACKET = ']';
    #endregion

    #region Class members
    /// <summary>
    /// Index to row (0..65535) or row offset (-32768..32767).
    /// </summary>
    private int m_iRowIndex = 0;
    /// <summary>
    /// Index to column (0..255) or column offset (-128..127).
    /// </summary>
    private int m_iColumnIndex = 0;
    /// <summary>
    /// Option flags.
    /// </summary>
    private byte m_options = 0;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor:
    /// To prevent user from creating tokens without arguments
    /// and to allow descendants to do this.
    /// </summary>
    public RefPtg()
    {
    }
    /// <summary>
    /// Constructs reference by its string representation.
    /// </summary>
    /// <param name="strCell">String representation of the reference.</param>
    public RefPtg( string strCell )
    {
      Match m = FormulaUtil.CellRegex.Match( strCell );
      string col = m.Groups[ "Column1" ].Value;
      string row = m.Groups[ "Row1" ].Value;

      SetCellA1( col, row );
      TokenCode = FormulaToken.tRef2;
    }
    /// <summary>
    /// Creates token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public RefPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
//      m_iRowIndex = provider.ReadUInt16( offset + 1 );
//      m_iColumnIndex = provider.ReadByte( offset + 3 );
//      m_options = provider.ReadByte( offset + 4 );
    }
    /// <summary>
    /// Creates token by coordinates and options.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iColIndex">Column index.</param>
    /// <param name="options">Options.</param>
    public RefPtg( int iRowIndex, int iColIndex, byte options )
    {
      m_iRowIndex = iRowIndex;
      m_iColumnIndex = iColIndex;
      m_options = options;
    }
    /// <summary>
    /// Constructs reference by its string representation.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="strRow">String representation of the row.</param>
    /// <param name="strColumn">String representation of the column.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation was used.</param>
    public RefPtg( int iCellRow, int iCellColumn, string strRow, string strColumn, bool bR1C1 )
    {
      SetCell( iCellRow, iCellColumn, strRow, strColumn, bR1C1 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="twin"></param>
    public RefPtg( RefPtg twin )
    {
      m_iRowIndex = twin.RowIndex;
      m_iColumnIndex = twin.m_iColumnIndex;
      m_options = twin.m_options;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets. Index to row (0..65535) or row offset (-32768..32767).
    /// </summary>
    [ CLSCompliant( false ) ]
    public virtual int RowIndex
    {
      get
      {
        return m_iRowIndex;
      }
      set
      {
        m_iRowIndex = value;
      }
    }
    /// <summary>
    /// Gets / sets True if row index is relative.
    /// </summary>
    virtual public bool IsRowIndexRelative
    {
      get
      {
        return IsRelative ( m_options, RowBitMask );
      }
      set
      {
        m_options = SetRelative( m_options, RowBitMask, value );
      }
    }
    /// <summary>
    /// Gets / sets True if column index is relative.
    /// </summary>
    virtual public bool IsColumnIndexRelative
    {
      get
      {
        return IsRelative( m_options, ColumnBitMask );
      }
      set
      {
        m_options = SetRelative(  m_options, ColumnBitMask, value );
      }
    }
    /// <summary>
    /// Gets / sets index to column (0..255) or column offset (-128..127).
    /// </summary>
    virtual public int ColumnIndex
    {
      get
      {
        return m_iColumnIndex;
      }
      set
      {
        m_iColumnIndex = value;
      }
    }
    /// <summary>
    /// Options.
    /// </summary>
    protected byte Options
    {
      get
      {
        return m_options;
      }
      set
      {
        m_options = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of the record.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          return 5;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          return 10;

        default:
          throw new ArgumentOutOfRangeException( "version" );
      }
    }

    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <returns>String representation of the token.</returns>
    public override string ToString()
    {
      return GetCellName( 0, 0, m_iRowIndex, m_iColumnIndex, IsRowIndexRelative,
        IsColumnIndexRelative, false );
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
      return GetCellName( iRow, iColumn, RowIndex, ColumnIndex, IsRowIndexRelative,
        IsColumnIndexRelative, bR1C1 );
    }

    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes that represents the token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      // TODO: check version & size

      byte[] result = base.ToByteArray( version );
      int iOffset = 1;

      if( version == ExcelVersion.Excel97to2003 )
      {
        if( m_iRowIndex > ushort.MaxValue || m_iColumnIndex > byte.MaxValue )
        {
          // Here we have to convert into RefErr token.
          FormulaToken tokenCode = GetCorrespondingErrorCode();
          result[ 0 ] = ( byte )tokenCode;
        }

        BitConverter.GetBytes( ( ushort )m_iRowIndex ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.ShortSize;

        result[ iOffset ] = ( byte )m_iColumnIndex;
        iOffset++;
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        BitConverter.GetBytes( m_iRowIndex ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.IntSize;

        BitConverter.GetBytes( m_iColumnIndex ).CopyTo( result, iOffset );
        iOffset += ExcelConstants.IntSize;
      }
      else
      {
        throw new ArgumentOutOfRangeException( "version" );
      }

      result[ iOffset ] = m_options;
      return result;
    }

    /// <summary>
    /// Sets internal fields appropriate to specified row and column.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains this token.</param>
    /// <param name="iCellColumn">Column index of the cell that contains this token.</param>
    /// <param name="strColumn">String that contains column name.</param>
    /// <param name="strRow">String that contains row name.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    protected void SetCell( int iCellRow, int iCellColumn, string strRow,
      string strColumn, bool bR1C1 )
    {
      if( bR1C1 )
      {
        SetCellR1C1( iCellRow, iCellColumn, strColumn, strRow );
      }
      else
      {
        SetCellA1( strColumn, strRow );
      }
    }
    /// <summary>
    /// Sets internal fields appropriate to specified row and column.
    /// </summary>
    /// <param name="strColumn">String that contains column name.</param>
    /// <param name="strRow">String that contains row name.</param>
    protected void SetCellA1( string strColumn, string strRow )
    {
      if( strRow == null )
        throw new ArgumentNullException( "strRow" );

      if( strRow.Length == 0 )
        throw new ArgumentException( "strRow - string cannot be empty" );

      if( strColumn == null )
        throw new ArgumentNullException( "strColumn" );

      if( strColumn.Length == 0 )
        throw new ArgumentException( "strColumn - string cannot be empty" );

      bool bRelative;

      m_iColumnIndex = GetColumnIndex( 0, strColumn, false, out bRelative );
      IsColumnIndexRelative = bRelative;

      m_iRowIndex = GetRowIndex( 0, strRow, false, out bRelative );
      IsRowIndexRelative = bRelative;
    }
    /// <summary>
    /// Sets internal fields appropriate to specified row and column.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains this token.</param>
    /// <param name="iCellColumn">Column index of the cell that contains this token.</param>
    /// <param name="column">String that contains column name.</param>
    /// <param name="row">String that contains row name.</param>
    protected void SetCellR1C1( int iCellRow, int iCellColumn, string column, string row )
    {
      bool bRelative;

      m_iColumnIndex = GetR1C1Index( iCellColumn, column, out bRelative );
      IsColumnIndexRelative = bRelative;

      m_iRowIndex = GetR1C1Index( iCellRow, row, out bRelative );
      IsRowIndexRelative = bRelative;
    }
    /// <summary>
    /// Parses row or column index in R1C1 notation.
    /// </summary>
    /// <param name="iIndex">Row or column index of the cell that contains reference.</param>
    /// <param name="strValue">String to parse.</param>
    /// <param name="bRelative">Indicates whether index is relative.</param>
    /// <returns></returns>
    public static int GetR1C1Index( int iIndex, string strValue, out bool bRelative )
    {
      bRelative = false;
      if( strValue == null ) return -1;

      int iLength = strValue.Length;

      if( iLength < 2 )
      {
        bRelative = true;
        return iIndex;
      }

      strValue = strValue.Substring( 1 );
      iLength--;

      if( strValue[ 0 ] == DEF_OPEN_BRACKET && strValue[ iLength - 1 ] == DEF_CLOSE_BRACKET )
      {
        iLength -= 2;
        strValue = strValue.Substring( 1, iLength );
        bRelative = true;
      }

      int iParsedIndex = int.Parse( strValue );
      return bRelative ? iIndex + iParsedIndex : iParsedIndex - 1;
    }
    /// <summary>
    /// Creates token and modifies it row and column indexes.
    /// </summary>
    /// <param name="iRowOffset">Row offset.</param>
    /// <param name="iColumnOffset">Column offset.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Modified token.</returns>
    public override Ptg Offset( int iRowOffset, int iColumnOffset, WorkbookImpl book )
    {
      RefPtg result = ( RefPtg )base.Offset( iRowOffset, iColumnOffset, book );

      int iRowIndex = IsRowIndexRelative ? RowIndex + iRowOffset : RowIndex;
      int iColIndex = IsColumnIndexRelative ? ColumnIndex + iColumnOffset : ColumnIndex;
      
      if( iRowIndex < 0 || iRowIndex > book.MaxRowCount - 1 ||
        iColIndex < 0 || iColIndex > book.MaxColumnCount - 1 )
      {
        // This is wrong row or column index so we have to create RefErr.
        FormulaToken tokenCode = GetCorrespondingErrorCode();
        return FormulaUtil.CreatePtg( tokenCode, ToString( book.FormulaUtil ), book );
      }

      result.RowIndex = iRowIndex;
      result.ColumnIndex = iColIndex;

      return result;
    }
    /// <summary>
    /// Returns modified token after move operation.
    /// </summary>
    /// <param name="iCurSheetIndex">Current sheet index.</param>
    /// <param name="iTokenRow">Parent cell row index.</param>
    /// <param name="iTokenColumn">Parent cell column index.</param>
    /// <param name="iSourceSheetIndex">Source sheet index.</param>
    /// <param name="rectSource">Source rectangle.</param>
    /// <param name="iDestSheetIndex">Destination sheet index.</param>
    /// <param name="rectDest">Destination rectangle.</param>
    /// <param name="bChanged">Indicates whether token was changed.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    public override Ptg Offset( int iCurSheetIndex, int iTokenRow, int iTokenColumn,
      int iSourceSheetIndex, Rectangle rectSource, int iDestSheetIndex,
      Rectangle rectDest, out bool bChanged, WorkbookImpl book )
    {
      bChanged = false;
      RefPtg result = ( RefPtg )base.Offset( iCurSheetIndex, iTokenRow, iTokenColumn,
        iSourceSheetIndex, rectSource, iDestSheetIndex, rectDest, out bChanged, book );

      int iRowOffset = rectDest.Top - rectSource.Top;
      int iColOffset = rectDest.Left - rectSource.Left;

      int iRowIndex = RowIndex;
      int iColIndex = ColumnIndex;

      // If referenced cell was moved...
      if( iCurSheetIndex == iSourceSheetIndex )
      {
        if( RectangleContains( rectSource, iRowIndex, iColIndex ) )
        {
          iRowIndex += iRowOffset;
          iColIndex += iColOffset;

          return result.UpdateReferencedCell( iCurSheetIndex, iDestSheetIndex, iRowIndex,
            iColIndex, ref bChanged, book );
        }
        else if( RectangleContains( rectDest, iRowIndex, iColIndex ) )
        {
          return ConvertToError();//new RefErrorPtg( this );
        }
      }
      else
      {
        // If formula was moved into different worksheet...
        if( iCurSheetIndex == iDestSheetIndex && iSourceSheetIndex != iDestSheetIndex
          && RectangleContains( rectDest, iTokenRow, iTokenColumn ) )
        {
          bChanged = true;
          return MoveIntoDifferentSheet( result, iSourceSheetIndex, rectSource,
            iDestSheetIndex, iRowOffset, iColOffset, book );
        }
      }

      return result;
    }

    /// <summary>
    /// Moves token into different worksheet.
    /// </summary>
    /// <param name="result">Token to move</param>
    /// <param name="iSourceSheetIndex">Source sheet index.</param>
    /// <param name="rectSource">Source rectangle.</param>
    /// <param name="iDestSheetIndex">Destination sheet index.</param>
    /// <param name="iRowOffset">Row offset.</param>
    /// <param name="iColOffset">Column offset.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Modified token.</returns>
    protected virtual Ptg MoveIntoDifferentSheet( RefPtg result,  int iSourceSheetIndex, 
      Rectangle rectSource, int iDestSheetIndex, int iRowOffset, int iColOffset, WorkbookImpl book )
    {
      int iSheetIndex = iSourceSheetIndex;
      bool bMoved = ReferencedCellMoved( rectSource );

      int iRowIndex = result.RowIndex;
      int iColIndex = result.ColumnIndex;

      if( bMoved )
      {
        iSheetIndex = iDestSheetIndex;
        iRowIndex += iRowOffset;
        iColIndex += iColOffset;
      }

      FormulaToken token = Ref3DPtg.IndexToCode( CodeToIndex() );
      return result = ( RefPtg )FormulaUtil.CreatePtg( token, iSheetIndex, iRowIndex,
        iColIndex, m_options );


//      return result.UpdateReferencedCell( iSheetIndex, iSheetIndex, iRowIndex, iColIndex,
//        ref bMoved );
    }
    /// <summary>
    /// Returns True if referenced cell was moved.
    /// </summary>
    /// <param name="rectSource"></param>
    /// <returns></returns>
    private bool ReferencedCellMoved( Rectangle rectSource )
    {
      int iRowIndex = RowIndex;
      int iColIndex = ColumnIndex;
      return RectangleContains( rectSource, iRowIndex, iColIndex );
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
      return RefErrorPtg.IndexToCode( index );
    }

    /// <summary>
    /// Updates token after move operation.
    /// </summary>
    /// <param name="iCurSheetIndex">Current sheet index.</param>
    /// <param name="iDestSheetIndex">Destination sheet index.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iColIndex">Column index.</param>
    /// <param name="bChanged">Indicates whether token was changed.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Updated token.</returns>
    private Ptg UpdateReferencedCell( int iCurSheetIndex, int iDestSheetIndex,
      int iRowIndex, int iColIndex, ref bool bChanged, WorkbookImpl book )
    {
      if( iRowIndex < 0 || iRowIndex > book.MaxRowCount - 1 ||
        iColIndex < 0 || iColIndex > book.MaxColumnCount - 1 )
      {
        // This is wrong row or column index so we have to create RefErr.
        FormulaToken tokenCode = GetCorrespondingErrorCode();
        return FormulaUtil.CreatePtg( tokenCode, ToString() );
      }

      if( iCurSheetIndex == iDestSheetIndex )
      {
        //result = ( RefPtg )result.Clone();
        RowIndex = iRowIndex;
        ColumnIndex = iColIndex;
        bChanged = true;
      }
      else
      {
        bChanged = true;
        FormulaToken token = Ref3DPtg.IndexToCode( CodeToIndex() );
        return FormulaUtil.CreatePtg( token, iDestSheetIndex, iRowIndex, iColIndex, m_options );
        //new Ref3DPtg( iDestSheetIndex, iRowIndex, iColIndex, CodeToIndex() );
      }

      return this;
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
      int iNewColumn = IsColumnIndexRelative ? ColumnIndex - iColumn : ColumnIndex;
      int iNewRow = IsRowIndexRelative ? RowIndex - iRow : RowIndex;

      FormulaToken token = RefNPtg.IndexToCode( CodeToIndex() );
      RefNPtg result = ( RefNPtg )FormulaUtil.CreatePtg( token );

      if( parent.Version == ExcelVersion.Excel97to2003 )
      {
        result.RowIndex = iNewRow;
        result.ColumnIndex = iNewColumn;
      }
      else
      {
        RefPtg reference = ( RefPtg )result;
        reference.RowIndex = iNewRow;
        reference.ColumnIndex = iNewColumn;
      }

      result.Options = Options;

      return result;
    }
    #endregion

    #region Class static memthods
    /// <summary>
    /// Returns True if bit specified by mask is set to 1; otherwise False.
    /// </summary>
    /// <param name="Options">Options byte where bits will be checked.</param>
    /// <param name="mask">Bit mask that should be used for checking.</param>
    /// <returns>True if bit specified by mask is set to 1.</returns>
    public static bool IsRelative( byte Options, byte mask )
    {
      return ( ( Options & mask ) != 0 );
    }
    /// <summary>
    /// If value is True, then it sets all bits specified by mask in Options to 1;
    /// otherwise clears them, return new value.
    /// </summary>
    /// <param name="Options">Options byte where bits will be set.</param>
    /// <param name="mask">Bit mask that should be used for checking.</param>
    /// <param name="value">Flag for operation.</param>
    /// <returns>Return Options with bit specified by mask set to value.</returns>
    public static byte SetRelative( byte Options, byte mask, bool value )
    {
      Options &= ( byte ) ~mask;

      if( value ) Options += mask;

      return Options;
    }
    /// <summary>
    /// Returns string representation of the cell.
    /// </summary>
    /// <param name="iCurCellRow">Row index of the cell that contains token.</param>
    /// <param name="iCurCellColumn">Column index of the cell that contains token.</param>
    /// <param name="row">Index of cell row.</param>
    /// <param name="column">Index of cell column.</param>
    /// <param name="bRowRelative">True if row is relative.</param>
    /// <param name="bColumnRelative">True if column is relative.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used for string conversion.</param>
    /// <returns>String representation of the cell address.</returns>
    [ CLSCompliant( false ) ]
    public static string GetCellName( int iCurCellRow, int iCurCellColumn, int row, int column,
      bool bRowRelative, bool bColumnRelative, bool bR1C1 )
    {
      return bR1C1
        ? GetR1C1CellName( iCurCellRow, iCurCellColumn, row, column, bRowRelative, bColumnRelative )
        : GetA1CellName( column, row, bColumnRelative, bRowRelative );
    }
    /// <summary>
    /// Returns string representation of the cell using A1 notation.
    /// </summary>
    /// <param name="column">Index of cell column.</param>
    /// <param name="row">Index of cell row.</param>
    /// <param name="isColumnRelative">True if column is relative.</param>
    /// <param name="isRowRelative">True if row is relative.</param>
    /// <returns>String representation of the cell address.</returns>
    private static string GetA1CellName( int column, int row,
      bool isColumnRelative, bool isRowRelative )
    {
      string strColumnString = RangeImpl.GetColumnName( column + 1 );
      string strRowString = ( row + 1 ).ToString();

      if( !isRowRelative ) strRowString = "$" + strRowString;

      if( !isColumnRelative ) strColumnString = "$" + strColumnString;

      return strColumnString + strRowString;
    }
    /// <summary>
    /// Return string representation of the cell in RC format.
    /// RC format is format in which set only shifts from start row to other one.
    /// </summary>
    /// <param name="column">Index of cell column.</param>
    /// <param name="row">Index of cell row.</param>
    /// <returns>String representation of the cell address.</returns>
    public static string GetRCCellName( int column, int row )
    {
      return "R" + ( ( row == 0 ) ? "" : "[" + row + "]" ) + 
        "C" + ( ( column == 0 ) ? "" : "[" + column + "]" );
    }
    /// <summary>
    /// Returns column index of the cell.
    /// </summary>
    /// <param name="iCellColumn">Column index of the cell that contains token.</param>
    /// <param name="columnName">String representation of the column.</param>
    /// <returns>Column index.</returns>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <param name="bRelative">Indicates whether resulting column index is relative.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When parsed column index is greater than 255.
    /// </exception>
    public static int GetColumnIndex( int iCellColumn, string columnName, bool bR1C1, out bool bRelative )
    {
      return bR1C1
        ? GetR1C1Index( iCellColumn, columnName, out bRelative )
        : GetA1ColumnIndex( columnName, out bRelative );
    }
    /// <summary>
    /// Returns column index of the cell.
    /// </summary>
    /// <param name="columnName">String representation of the column.</param>
    /// <returns>Column index.</returns>
    /// <param name="bRelative">Indicates whether resulting column index is relative.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When parsed column index is greater than 255.
    /// </exception>
    public static int GetA1ColumnIndex( string columnName, out bool bRelative )
    {
      if( columnName == null )
        throw new ArgumentNullException( "columnName" );

      if( columnName.Length == 0 )
        throw new ArgumentException( "columnName - string cannot be empty." );

      bRelative = ( columnName[ 0 ] != '$' );
 
      if( !bRelative )
      {
        columnName = columnName.Substring( 1 );
      }

//      if( columnName.Length >= 3 )
//        throw new ArgumentOutOfRangeException( columnName, "Wrong cell index" );

      return RangeImpl.GetColumnIndex( columnName ) - 1;
    }

    /// <summary>
    /// Returns row index of the cell.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains token.</param>
    /// <param name="strRowName">String representation of the row.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <param name="bRelative">Indicates whether row index is relative.</param>
    /// <returns>Row index.</returns>
    public static int GetRowIndex( int iCellRow, string strRowName, bool bR1C1, out bool bRelative )
    {
      return bR1C1
        ? GetR1C1Index( iCellRow, strRowName, out bRelative )
        : GetA1RowIndex( strRowName, out bRelative );
    }
    /// <summary>
    /// Returns row index of the cell.
    /// </summary>
    /// <param name="strRowName">String representation of the row.</param>
    /// <param name="bRelative">Indicates whether row index is relative.</param>
    /// <returns>Row index.</returns>
    public static int GetA1RowIndex( string strRowName, out bool bRelative )
    {
      if( strRowName == null )
        throw new ArgumentNullException( "strRowName" );

      if( strRowName.Length == 0 )
        throw new ArgumentException( "strRowName - string cannot be empty." );

      bRelative = ( strRowName[ 0 ] != '$' );

      if( !bRelative )
      {
        strRowName = strRowName.Substring( 1 );
      }

      return int.Parse( strRowName ) - 1;
    }
    /// <summary>
    /// Converts reference index to token code.
    /// </summary>
    /// <param name="index">Reference index.</param>
    /// <returns>Token code.</returns>
    public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tRef1;
        case 2: return FormulaToken.tRef2;
        case 3: return FormulaToken.tRef3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <param name="token">Token code (should be one of tRef1, tRef2, tRef3).</param>
    /// <returns>Reference index.</returns>
    public static int CodeToIndex( FormulaToken token )
    {
      switch( token )
      {
        case FormulaToken.tRef1:
        case FormulaToken.tRefErr1: 
              return 1;
        case FormulaToken.tRef2:
        case FormulaToken.tRefErr2:       
           return 2;
        case FormulaToken.tRef3:
        case FormulaToken.tRefErr3:
           return 3;
        
        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Gets string representation of the cell in R1C1 notation.
    /// </summary>
    /// <param name="iCurRow">Row index of the cell that contains token.</param>
    /// <param name="iCurColumn">Column index of the cell that contains token.</param>
    /// <param name="row">Cell row index.</param>
    /// <param name="column">Cell column index.</param>
    /// <param name="bRowRelative">Indicates whether row is relative.</param>
    /// <param name="bColumnRelative">Indicates whether column is relative.</param>
    /// <returns>String representation of the cell in R1C1 notation.</returns>
    public static string GetR1C1CellName( int iCurRow, int iCurColumn, int row, int column,
      bool bRowRelative, bool bColumnRelative )
    {
      return GetR1C1Name( iCurRow , DEF_R1C1_ROW, row, bRowRelative )
        + GetR1C1Name( iCurColumn , DEF_R1C1_COLUMN, column, bColumnRelative );
    }
    /// <summary>
    /// Converts row or column index into string representation using R1C1 notation.
    /// </summary>
    /// <param name="iCurIndex">
    /// Row or column index of a cell that contains reference to convert.
    /// </param>
    /// <param name="strStart">Starting character.</param>
    /// <param name="iIndex">End index.</param>
    /// <param name="bIsRelative">Indicates whether index is relative.</param>
    /// <returns>String representation of row or column index using R1C1 notation.</returns>
    public static string GetR1C1Name( int iCurIndex, string strStart, int iIndex, bool bIsRelative )
    {
      if( strStart == null )
        throw new ArgumentNullException( "strStart" );

      if( bIsRelative )
      {
        iIndex -= iCurIndex;

        if( iIndex == 0 ) return strStart;
      }

      string strResult = strStart;

      if( bIsRelative )
      {
        strResult += DEF_R1C1_OPEN_BRACKET;
      }
      else
      {
        iIndex++;
      }

      strResult += iIndex.ToString();

      if( bIsRelative )strResult += DEF_R1C1_CLOSE_BRACKET;

      return strResult;
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

      return sheet.Range[ m_iRowIndex + 1, m_iColumnIndex + 1 ];
    }

    /// <summary>
    /// Returns rectangle represented by the token that implements this interface.
    /// All coordinates are zero-based.
    /// </summary>
    /// <returns>Rectangle represented by the token.</returns>
    public Rectangle GetRectangle()
    {
      return Rectangle.FromLTRB( ColumnIndex, RowIndex, ColumnIndex, RowIndex );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public Ptg UpdateRectangle( Rectangle rectangle )
    {
      RefPtg result = ( RefPtg )Clone();
      result.ColumnIndex = rectangle.Left;
      result.RowIndex = rectangle.Top;

      return result;
    }
    #endregion

    #region IToken3D Members

    /// <summary>
    /// Converts current token to the 3D token.
    /// </summary>
    /// <param name="iSheetReference">Reference to the worksheet.</param>
    /// <returns>Created token.</returns>
    public virtual Ptg Get3DToken( int iSheetReference )
    {
      int iIndex = CodeToIndex( TokenCode );
      FormulaToken tokenCode = Ref3DPtg.IndexToCode( iIndex );

      Ptg result = new Ref3DPtg( iSheetReference, RowIndex, ColumnIndex, Options );
      result.TokenCode = tokenCode;
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
        m_iRowIndex = provider.ReadUInt16( offset );
        offset += 2;
        m_iColumnIndex = provider.ReadByte( offset++ );
        m_options = provider.ReadByte( offset++ );
      }
      else if( version !=ExcelVersion.Excel97to2003)
      {
        m_iRowIndex = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;

        m_iColumnIndex = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;

        m_options = provider.ReadByte( offset++ );
      }
      else
      {
        throw new NotImplementedException();
      }
    }
    #endregion
  }
}
