#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Text;

using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This class is the base class for each token of a formula.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public abstract class Ptg : ICloneable
  {
    #region Class constants
    private const int DEF_PTG_INDEX_DELTA = 0x20;
    #endregion

    #region Class members
    /// <summary>
    /// Code of the token.
    /// </summary>
    private FormulaToken m_Code;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Ptg()
    {
    }

    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    protected Ptg( DataProvider provider, int offset, ExcelVersion version )
    {
      m_Code = ( FormulaToken )provider.ReadByte( offset );
      offset++;
      InfillPTG( provider, ref offset, version );
    }
    #endregion

    #region Class properties
//    /// <summary>
//    /// Read-only. Size of the ptg token.
//    /// </summary>
//    abstract public int Size{ get; }

    /// <summary>
    /// Read-only. True if this ptg represents operation ptg.
    /// </summary>
    public virtual bool IsOperation
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets / sets. Code of the token.
    /// </summary>
    public virtual FormulaToken TokenCode
    {
      get
      {
        return m_Code;
      }
      set
      {
        m_Code = value;
      }
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Gets string from byte array, length of string is set in 16 bit value.
    /// </summary>
    /// <param name="data">Data array which contains string.</param>
    /// <param name="offset">Offset to the string data.</param>
    /// <returns>Parsed string.</returns>
    public static string GetString16Bit( byte[] data, int offset )
    {
      int iFullLength;
      return GetString16Bit( data, offset, out iFullLength );
    }

    /// <summary>
    /// Gets string from byte array and returns it's length in iFullLength parameter.
    /// </summary>
    /// <param name="data">Data array which contains string.</param>
    /// <param name="offset">Offset to the string data.</param>
    /// <param name="iFullLength">Length of the string in bytes.</param>
    /// <returns>Parsed string.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data array is smaller than the string that should be in it.
    /// </exception>
    public static string GetString16Bit( byte[] data, int offset, out int iFullLength )
    {
      if( offset + 3 >= data.Length )
        throw new ArgumentOutOfRangeException( "GetString16Bit: data array too small." );

      ushort strLen = BitConverter.ToUInt16( data, offset );
      offset += 2;
      bool Is16Bit = BitConverter.ToBoolean( data, offset );
      offset++;
      iFullLength = Is16Bit ? 3 + strLen * 2 : 3 + strLen;

      if( iFullLength >= data.Length )
        throw new ArgumentOutOfRangeException( "GetString16Bit: data array too small." );

      return Is16Bit ? Encoding.Unicode.GetString( data, offset, strLen * 2 )
        : BiffRecordRawWithArray.LatinEncoding.GetString( data, offset, strLen );
    }
    #endregion

    #region Class infill methods
    /// <summary>
    /// Infill PTG structure.
    /// </summary>
    /// <param name="provider">Represents storage.</param>
    /// <param name="offset">Offset in storage.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public virtual void InfillPTG( DataProvider provider, ref int offset, ExcelVersion version )
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns size of the tokens array.
    /// </summary>
    /// <param name="version">Excel version - defines resulting size.</param>
    /// <returns>Size of the tokens array.</returns>
    public abstract int GetSize( ExcelVersion version );
    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <param name="version">Excel version - defines resulting array format and size.</param>
    /// <returns>Array of bytes representing this token.</returns>
    public virtual byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = new byte[ GetSize( version ) ];
      result[ 0 ] = ( byte )TokenCode;

      return result;
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <returns>String representation of the token.</returns>
    public override string ToString()
    {
      return ToString( null, 0, 0, false );
    }

    /// <summary>
    /// Converts token to the string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <returns>String representation of the token.</returns>
    public virtual string ToString( FormulaUtil formulaUtil )
    {
      return ToString( formulaUtil, 0, 0, false );
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public virtual string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1 )
    {
      return ToString( formulaUtil, iRow, iColumn, bR1C1, null, false );
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="row">Zero-based row index of the cell that contains this token.</param>
    /// <param name="col">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns></returns>
    public virtual string ToString( int row, int col, bool bR1C1 )
    {
      return ToString( null, row, col, bR1C1 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="numberFormat"></param>
    /// <returns></returns>
    public virtual string ToString( FormulaUtil formulaUtil, int row, int col, bool bR1C1,
      NumberFormatInfo numberFormat )
    {
      return ToString( formulaUtil, row, col, bR1C1, numberFormat, false );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="numberFormat"></param>
    /// <returns></returns>
    public virtual string ToString( FormulaUtil formulaUtil, int row, int col, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization )
    {
      return base.ToString();
    }
    /// <summary>
    /// Moves token by iRowOffset to the right and iColumnOffset to the left.
    /// (Updates formula token after copy operation.)
    /// </summary>
    /// <param name="iRowOffset">Row offset.</param>
    /// <param name="iColumnOffset">Column offset.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Updated token.</returns>
    public virtual Ptg Offset( int iRowOffset, int iColumnOffset, WorkbookImpl book )
    {
      return ( Ptg )Clone();
    }
    /// <summary>
    /// Adjusts the location of the token by the specified amount.
    /// Returns adjusted token. (Updates formula token after move operation.)
    /// </summary>
    /// <param name="iCurSheetIndex">Index of the sheet where formula is located.</param>
    /// <param name="iTokenRow">Zero-based row index which this token is located.</param>
    /// <param name="iTokenColumn">Zero-based column index where this token is located.</param>
    /// <param name="iSourceSheetIndex">Index of the source worksheet in move range operation.</param>
    /// <param name="iDestSheetIndex">Index of the destination worksheet in move range operation.</param>
    /// <param name="rectSource">Rectangle that was moved.</param>
    /// <param name="rectDest">Location were range was moved.</param>
    /// <param name="bChanged">Indicates whether token was changed during move operation.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Adjusted token.</returns>
    public virtual Ptg Offset( int iCurSheetIndex, int iTokenRow, int iTokenColumn,
      int iSourceSheetIndex, Rectangle rectSource, int iDestSheetIndex,
      Rectangle rectDest, out bool bChanged, WorkbookImpl book )
    {
      bChanged = false;
      return ( Ptg )Clone();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="iRow"></param>
    /// <param name="iColumn"></param>
    /// <returns></returns>
    public static bool RectangleContains( Rectangle rect, int iRow, int iColumn )
    {
      if( rect.Left <= iColumn && rect.Right >= iColumn
        && rect.Top <= iRow && rect.Bottom >= iRow )
        return true;

      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="iRow"></param>
    /// <param name="iColumn"></param>
    /// <returns></returns>
    public virtual Ptg ConvertSharedToken( IWorkbook parent, int iRow, int iColumn )
    {
      return ( Ptg )Clone();
    }
    /// <summary>
    /// Converts tokens from regular formula into tokens from shared formula.
    /// </summary>
    /// <param name="parent">Represents parent workbook.</param>
    /// <param name="iRow">Represents row index.</param>
    /// <param name="iColumn">Represents column index.</param>
    /// <returns>Formula token.</returns>
    public virtual Ptg ConvertPtgToNPtg( IWorkbook parent, int iRow, int iColumn )
    {
      return ( Ptg )this;
    }
    /// <summary>
    /// Compares this token to the specified one.
    /// </summary>
    /// <param name="token">Token to compare with.</param>
    /// <returns>0 if tokens are equal.</returns>
    public int CompareTo( Ptg token )
    {
      if( token == null ) return 1;

      int iResult = ( int )TokenCode - ( int )token.TokenCode;

      if( iResult == 0 )
      {
        iResult = GetSize( ExcelVersion.Excel2007 )- token.GetSize( ExcelVersion.Excel2007 );
      }

      if( iResult == 0 ) iResult = CompareContent( token );

      return iResult;
    }
    /// <summary>
    /// Compares tokens content.
    /// </summary>
    /// <param name="token">Token to compare with this one.</param>
    /// <returns>0 if tokens are equal.</returns>
    protected int CompareContent( Ptg token )
    {
      if( token == null )
        throw new ArgumentNullException( "token" );

      // TODO: optimize this compare - probably should be overridden in the child classes for better performance.
      byte[] arrData = ToByteArray( ExcelVersion.Excel2007 );
      byte[] arrData2 = token.ToByteArray( ExcelVersion.Excel2007 );

      return BiffRecordRaw.CompareArrays( arrData, arrData2 )
        ? 0
        : 1;
    }
    /// <summary>
    /// Compares two token arrays.
    /// </summary>
    /// <param name="arrTokens1">The first array to compare.</param>
    /// <param name="arrTokens2">The second array to compare.</param>
    /// <returns>True if arrays are equal; otherwise false.</returns>
    public static bool CompareArrays( Ptg[] arrTokens1, Ptg[] arrTokens2 )
    {
      if( arrTokens1 == null && arrTokens2 == null ) return true;

      if( arrTokens1 == null || arrTokens2 == null ) return false;

      int iLength = arrTokens1.Length;

      if( iLength != arrTokens2.Length ) return false;

      for( int i = 0; i < iLength; i++ )
      {
        Ptg token1 = arrTokens1[ i ];
        Ptg token2 = arrTokens2[ i ];

        if( token1.CompareTo( token2 ) != 0 )
          return false;
      }

      return true;
    }
    public virtual string ToString( FormulaUtil formulaUtil, int row, int col, bool bR1C1, NumberFormatInfo numberInfo,
      bool isForSerialization, IWorksheet sheet )
    {
      return ToString( formulaUtil, row, col, bR1C1, numberInfo, isForSerialization );
    }
    #endregion

    #region ICloneable Members

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
      return MemberwiseClone();
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts index to token code.
    /// </summary>
    /// <param name="baseToken"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static FormulaToken IndexToCode( FormulaToken baseToken, int index )
    {
      if( index < 1 || index > 3 )
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 1 or greater than 3." );

      int iTokenCode = ( int )baseToken + ( index - 1 ) * DEF_PTG_INDEX_DELTA;
      return ( FormulaToken )iTokenCode;
    }
    #endregion
  }
}