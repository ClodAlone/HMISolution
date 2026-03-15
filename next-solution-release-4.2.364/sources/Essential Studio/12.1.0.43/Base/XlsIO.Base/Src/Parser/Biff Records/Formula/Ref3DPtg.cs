#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Text.RegularExpressions;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
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
  /// This token contains a 3D reference or an external reference to a cell.
  /// </summary>
  [ Token ( FormulaToken.tRef3d1 ) ]
  [ Token ( FormulaToken.tRef3d2 ) ]
  [ Token ( FormulaToken.tRef3d3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class Ref3DPtg
    : RefPtg
    , IRangeGetter
    , ISheetReference
  {
    #region Class members
    /// <summary>
    /// Index to a REF entry in an EXTERNSHEET record in the Link Table.
    /// </summary>
    private ushort m_usRefIndex = 0;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public Ref3DPtg()
    {
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public Ref3DPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the token.</param>
    /// <param name="parent">Workbook that contains this reference.</param>
    public Ref3DPtg( string strFormula, IWorkbook parent )
    {
      Match m = FormulaUtil.Cell3DRegex.Match( strFormula );

      if( !m.Success ) throw new ArgumentException( "strFormula" );

      string sheetName = m.Groups[ FormulaUtil.DEF_SHEETNAME_GROUP ].Value;
      string row       = m.Groups[ FormulaUtil.DEF_ROW_GROUP ].Value;
      string column    = m.Groups[ FormulaUtil.DEF_COLUMN_GROUP ].Value;

      SetCellA1( column, row );
      SetSheetIndex( sheetName, parent );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCellRow"></param>
    /// <param name="iCellColumn"></param>
    /// <param name="iSheetIndex"></param>
    /// <param name="strRow"></param>
    /// <param name="strColumn"></param>
    /// <param name="bR1C1"></param>
    public Ref3DPtg( int iCellRow, int iCellColumn, int iSheetIndex,
      string strRow, string strColumn, bool bR1C1 )
      : base( iCellRow, iCellColumn, strRow, strColumn, bR1C1 )
    {
      m_usRefIndex = ( ushort )iSheetIndex;
    }
    /// <summary>
    /// Creates token.
    /// </summary>
    /// <param name="iSheetIndex">Sheet reference.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iColIndex">Column index.</param>
    /// <param name="options">Options.</param>
    public Ref3DPtg( int iSheetIndex, int iRowIndex, int iColIndex, byte options )
      : base( iRowIndex, iColIndex, options )
    {
      m_usRefIndex = ( ushort )iSheetIndex;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="twin"></param>
    public Ref3DPtg( Ref3DPtg twin )
      : base( twin )
    {
      m_usRefIndex = twin.m_usRefIndex;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public ushort RefIndex
    {
      get
      {
        return m_usRefIndex;
      }
      set
      {
        m_usRefIndex = value;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the record.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return base.GetSize( version ) + 2;
    }

    /// <summary>
    /// Converts token to the string.
    /// </summary>
    /// <returns>String representation of the token.</returns>
    public override string ToString()
    {
      return "[RefIndex=" + m_usRefIndex.ToString() + " " + base.ToString() + "]";
    }
    /// <summary>
    /// Converts token to the array of bytes.
    /// </summary>
    /// <returns>Array of bytes that represents this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      int iLength = result.Length;

      Buffer.BlockCopy( result, 1, result, 3, iLength - 3 );
      BitConverter.GetBytes( m_usRefIndex ).CopyTo( result, 1 );

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
      if( formulaUtil == null )
      {
        return ToString();
      }
      else
      {
        string strSheetName = GetSheetName( formulaUtil.ParentWorkbook, m_usRefIndex );

        if (strSheetName != null)
        {
            if (Area3DPtg.ValidateSheetName(strSheetName))
                strSheetName = "'" + strSheetName + "'!";
            else
                strSheetName = strSheetName + "!";
        }
        else
            strSheetName = string.Empty;
        

        return strSheetName + base.ToString( formulaUtil, iRow, iColumn, bR1C1, numberFormat, isForSerialization );
      }
    }
    /// <summary>
    /// Calls ToString method of the base (not 3d) class.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public string BaseToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1 )
    {
      return base.ToString( formulaUtil, iRow, iColumn, bR1C1 );
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
    public override Ptg Offset(int iCurSheetIndex, int iTokenRow, int iTokenColumn,
      int iSourceSheetIndex, Rectangle rectSource, int iDestSheetIndex,
      Rectangle rectDest, out bool bChanged, WorkbookImpl book )
    {
      bChanged = false;
      if( m_usRefIndex == ( ushort )iSourceSheetIndex )
      {
        Ref3DPtg result = ( Ref3DPtg )base.Offset( iDestSheetIndex, iTokenRow, iTokenColumn,
          iDestSheetIndex, rectSource, iDestSheetIndex, rectDest, out bChanged, book );

        if( bChanged )
        {
          result.m_usRefIndex = ( ushort )iDestSheetIndex;
        }

        return result;
      }
      else
      {
        return ( Ptg )Clone();
      }
    }

    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <returns>Reference index.</returns>
    public override int CodeToIndex()
    {
      return CodeToIndex( TokenCode );
    }
    /// <summary>
    /// Gets corresponding error code.
    /// </summary>
    /// <returns>Corresponding error code.</returns>
    public override FormulaToken GetCorrespondingErrorCode()
    {
      int index = CodeToIndex();
      return RefError3dPtg.IndexToCode( index );
    }
    /// <summary>
    /// Returns referenced worksheet name.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="refIndex">Sheet reference index</param>
    /// <returns>Referenced worksheet name.</returns>
    public static string GetSheetName( IWorkbook book, int refIndex )
    {
      if( book == null )
        return null;

      string strSheetName = ( ( WorkbookImpl )book ).GetSheetNameByReference( refIndex, false );

      return ( strSheetName != null ) ?
        strSheetName.Replace( "'", "''" ) :
        null;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets index of the sheet in correct value
    /// by getting the information from specified workbook.
    /// </summary>
    /// <param name="sheetName">Name of the worksheet that is referenced.</param>
    /// <param name="parent">
    /// Workbook that contains this record. It must contain specified worksheet.
    /// </param>
    protected void SetSheetIndex( string sheetName, IWorkbook parent )
    {
      WorkbookImpl book = (WorkbookImpl) parent;

      if( sheetName[ 0 ] == '\'' && sheetName[ sheetName.Length - 1 ] == '\'' )
        sheetName = sheetName.Substring( 1, sheetName.Length - 2 );

      try
      {
        m_usRefIndex = (ushort) book.AddSheetReference( sheetName );
      }
      catch( ArgumentException )
      {
        throw new ParseException();
      }
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Returns token code by index.
    /// </summary>
    /// <param name="index">Index of the token code.</param>
    /// <returns>Required token code.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or greater than 3.
    /// </exception>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tRef3d1;
        case 2: return FormulaToken.tRef3d2;
        case 3: return FormulaToken.tRef3d3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <param name="token">Token code (should be one of tRef1, tRef2, tRef3).</param>
    /// <returns>Reference index.</returns>
    new public static int CodeToIndex( FormulaToken token )
    {
      switch( token )
      {
        case FormulaToken.tRef3d1: return 1;
        case FormulaToken.tRef3d2: return 2;
        case FormulaToken.tRef3d3: return 3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region IRangeGetter Members
    /// <summary>
    /// Returns range represented by the token that implements this interface.
    /// </summary>
    /// <param name="book">Workbook that contains range.</param>
    /// <param name="sheet">Worksheet that contains range.</param>
    /// <returns>Range represented by the token.</returns>
    new public IRange GetRange( IWorkbook book, IWorksheet sheet )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      WorkbookImpl bookImpl = ( WorkbookImpl )book;
      IRange result = null;

      if( !bookImpl.IsExternalReference( m_usRefIndex ) )
      {
        sheet = bookImpl.GetSheetByReference( m_usRefIndex, false );

        if( sheet != null )
          result = sheet[ RowIndex + 1, ColumnIndex + 1 ];
      }
      else
      {
        ExternWorksheetImpl externSheet = bookImpl.GetExternSheet( m_usRefIndex );
        result = new ExternalRange( externSheet, RowIndex + 1, ColumnIndex + 1 );
      }

      return result;
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
      m_usRefIndex = provider.ReadUInt16( offset );
      offset += 2;

      base.InfillPTG( provider, ref offset, version );
    }
    #endregion
  }
}
