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
  /// This token contains a 3D reference or an external reference to a cell range.
  /// </summary>
  [ Token( FormulaToken.tArea3d1 ) ]
  [ Token( FormulaToken.tArea3d2 ) ]
  [ Token( FormulaToken.tArea3d3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class Area3DPtg
    : AreaPtg
    , ISheetReference
    , IRangeGetter
  {
    #region Class members
    /// <summary>
    /// Index to REF entry in EXTERNSHEET record.
    /// </summary>
    private ushort m_usRefIndex = 0;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public Area3DPtg()
    {
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public Area3DPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }

    /// <summary>
    /// Constructs token by string value and parent workbook.
    /// </summary>
    /// <param name="strFormula">String representation of the token.</param>
    /// <param name="parent">Workbook containing the token.</param>
    /// <exception cref="System.ArgumentException">
    /// When specified formula string is not valid Area3D string.
    /// </exception>
    public Area3DPtg( string strFormula, IWorkbook parent )
    {
      Match m = FormulaUtil.CellRange3DRegex.Match( strFormula );

      if( !( m.Success && m.Value == strFormula ) )
      {
        m = FormulaUtil.CellRange3DRegex2.Match( strFormula );

        if( !( m.Success && m.Value == strFormula &&
          m.Groups[ "SheetName" ].Value == m.Groups[ "SheetName2" ].Value ) )
          throw new ArgumentException( "Not valid area 3D string." );

      }

      SetValues( m, parent );
    }

    /// <summary>
    /// Creates new token based on another Area3D token.
    /// </summary>
    /// <param name="ptg">Token to copy.</param>
    public Area3DPtg( Area3DPtg ptg )
      : base( ptg )
    {
      m_usRefIndex = ptg.m_usRefIndex;
    }
    /// <summary>
    /// Initializes new token.
    /// </summary>
    /// <param name="iSheetIndex">Worksheet reference index.</param>
    /// <param name="iFirstRow">Zero-based first row index.</param>
    /// <param name="iFirstCol">Zero-based first column index.</param>
    /// <param name="iLastRow">Zero-based last row index.</param>
    /// <param name="iLastCol">Zero-based last column index.</param>
    /// <param name="firstOptions">First cell options.</param>
    /// <param name="lastOptions">Second cell options.</param>
    public Area3DPtg( int iSheetIndex, int iFirstRow, int iFirstCol,
      int iLastRow, int iLastCol, byte firstOptions, byte lastOptions )
      : base( iFirstRow, iFirstCol, iLastRow, iLastCol, firstOptions, lastOptions )
    {
      m_usRefIndex = ( ushort )iSheetIndex;
    }
    /// <summary>
    /// Initializes new token.
    /// </summary>
    /// <param name="iCellRow">Zero-based row index of the cell that will contain new token.</param>
    /// <param name="iCellColumn">Zero-based column index of the cell that will contain new token.</param>
    /// <param name="iRefIndex">Worksheet reference index.</param>
    /// <param name="strFirstRow">String representation of the first row of the area.</param>
    /// <param name="strFirstColumn">String representation of the first column of the area.</param>
    /// <param name="strLastRow">String representation of the last row of the area.</param>
    /// <param name="strLastColumn">String representation of the last column of the area.</param>
    /// <param name="bR1C1">Indicates whether strings are in R1C1 notation.</param>
    /// <param name="book">Parent workbook.</param>
    public Area3DPtg( int iCellRow, int iCellColumn, int iRefIndex,
      string strFirstRow, string strFirstColumn, string strLastRow,
      string strLastColumn, bool bR1C1, IWorkbook book )
      : base( iCellRow, iCellColumn, strFirstRow, strFirstColumn, strLastRow,
      strLastColumn, bR1C1, book )
    {
      m_usRefIndex = ( ushort )iRefIndex;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Reference to the worksheet. Read-only.
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
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return base.GetSize( version ) + 2;
    }
    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes that represents this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] arrResult = base.ToByteArray( version );
      //byte[] Result = new byte[ GetSize( version ) ];

      //Result[ 0 ] = oldResult[ 0 ];
      Buffer.BlockCopy( arrResult, 1, arrResult, 3, arrResult.Length - 3 );
      BitConverter.GetBytes( m_usRefIndex ).CopyTo( arrResult, 1 );

      return arrResult;
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
      string strBase = base.ToString( formulaUtil, iRow, iColumn, bR1C1, numberFormat, isForSerialization );

      string strStart;

      if( formulaUtil == null )
      {
        strStart = "[ReferenceIndex = " + m_usRefIndex + " ] ";
      }
      else
      {
        strStart = ( ( WorkbookImpl )formulaUtil.ParentWorkbook ).GetSheetNameByReference( m_usRefIndex, false );

        if( strStart != null )
        {
            strStart = strStart.Replace("'", "''");
            if (ValidateSheetName (strStart))
                strStart = "'" + strStart + "'!";
            else
                strStart = strStart + "!";
        }
        else
        {
          strStart = string.Empty;
        }
      }

      return strStart + strBase;
    }
      /// <summary>
      /// To check the sheet name it's having any special charecter or not
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
    public static bool ValidateSheetName(string value)
    {
        char[] Symbols = { '!', '@', '#', '$', '%', '^', '&', '(', ')', '-', '\'', ';', ' ','"', '[',']','~', '{', '}', '+','|', ',', '=', '<', '>', '`' };
        int[] Max_column = { 88, 70, 68 };
        int count = value.IndexOfAny("123456789".ToCharArray(), 0);
        char Row = 'R', Column = 'C';
        value = value.ToUpper();
        char[] To_Char = value.ToCharArray();
        for (int i = 0; i < Symbols.Length - 1; i++)
        {
            if (value.Contains(Symbols[i].ToString()))
                return true;
        }
        if (char.IsDigit(To_Char[0]) || (To_Char.Length == 1 && (To_Char[0] == Column || To_Char[0] == Row)))
            return true;
        else if ((To_Char[0] == Column || To_Char[0] == Row) && (To_Char[1] != '0' && char.IsDigit(To_Char[1])))
            return true;
        if (count < 4 && count != -1)
        {
            for (int i = count; i < To_Char.Length; i++)
            {
                if (char.IsLetter(To_Char[i]) || i > count + 5)
                {
                    return false;
                }
            }
            if (count == 3)
            {
                for (int j = 0; j < count; j++)
                {
                    if (To_Char[j] < Max_column[j])
                        return true;
                    else if (To_Char[j] == Max_column[j])
                        continue;
                    else
                        return false;
                }
            }

            return true;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int CodeToIndex()
    {
      return CodeToIndex( TokenCode );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override FormulaToken GetCorrespondingErrorCode()
    {
      int index = CodeToIndex();
      return AreaError3DPtg.IndexToCode( index );
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
        Area3DPtg result = ( Area3DPtg )base.Offset( iDestSheetIndex, iTokenRow, iTokenColumn,
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
    /// Converts incorrect area range to corresponding error ptg.
    /// </summary>
    public override AreaPtg ConvertToErrorPtg()
    {
      return new AreaError3DPtg( this );
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
    #endregion

    #region Class methods
    /// <summary>
    /// Sets index of the sheet in the correct value,
    /// taking information from the specified workbook.
    /// </summary>
    /// <param name="sheetName">Name of the worksheet that is referenced.</param>
    /// <param name="parent">
    /// Workbook that contains this record and must contain specified worksheet.
    /// </param>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.ParseException">
    /// Unable to find specified worksheet in the workbook,
    /// possibly because wasn't loaded yet.
    /// </exception>
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
    /// <summary>
    /// Set area values such as reference indexes, first and last columns, and rows.
    /// </summary>
    /// <param name="m">Match that contains such groups SheetName, Column1, Column2, Row1, Row2.</param>
    /// <param name="parent">Workbook that contains this token.</param>
    protected void SetValues( Match m, IWorkbook parent )
    {
      string sheetName = m.Groups[ "SheetName" ].Value;
      string col1      = m.Groups[ "Column1"   ].Value;
      string row1      = m.Groups[ "Row1"      ].Value;
      string col2      = m.Groups[ "Column2"   ].Value;
      string row2      = m.Groups[ "Row2"      ].Value;

      SetArea( 0, 0, row1, col1, row2, col2, false, parent );
      SetSheetIndex( sheetName, parent );
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

//      FirstRow = provider.ReadUInt16( offset );
//      offset += 2;
//      LastRow = provider.ReadUInt16( offset );
//      offset += 2;
//      FirstColumn = provider.ReadByte( offset++ );
//      FirstOptions = provider.ReadByte(  offset++ );
//      LastColumn = provider.ReadByte(  offset++ );
//      LastOptions = provider.ReadByte(  offset++ );
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts specified index to the token code.
    /// </summary>
    /// <param name="index">Function parameter index.</param>
    /// <returns>Token code that corresponds to the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or greater than 3.
    /// </exception>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tArea3d1;
        case 2: return FormulaToken.tArea3d2;
        case 3: return FormulaToken.tArea3d3;

        default: throw new ArgumentOutOfRangeException( "index",
          "Must be less than 4 and greater than than 0." );
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
    new public static int CodeToIndex( FormulaToken code )
    {
      switch( code )
      {
        case FormulaToken.tArea3d1: return 1;
        case FormulaToken.tArea3d2: return 2;
        case FormulaToken.tArea3d3: return 3;

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
          result = sheet[ FirstRow + 1, FirstColumn + 1, LastRow + 1, LastColumn + 1 ];
      }
      else
      {
        ExternWorksheetImpl externSheet = bookImpl.GetExternSheet( m_usRefIndex );
        result = new ExternalRange( externSheet, FirstRow + 1, FirstColumn + 1, LastRow + 1, LastColumn + 1 );
      }

      return result;
    }
    #endregion
  }
}
