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
using System.Text;
using Syncfusion.XlsIO.Implementation;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains a string constant.
  /// The maximum length of the string is 255 characters in BIFF8.
  /// </summary>
  [ Token ( FormulaToken.tStringConstant ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class StringConstantPtg : Ptg
  {
    #region Class members
    /// <summary>
    /// String value.
    /// </summary>
    public string m_strValue = string.Empty;
    /// <summary>
    /// 1 - 2 bytes per character, 0 - 1 byte per character (compressed unicode).
    /// </summary>
    public byte m_compressed = 1;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public StringConstantPtg()
    {
    }
    /// <summary>
    /// Constructs token by string value.
    /// </summary>
    /// <param name="value">Value that will contain this token.</param>
    public StringConstantPtg( string value )
    {
      TokenCode = FormulaToken.tStringConstant;
      Value = value;
    }

    /// <summary>
    /// Creates token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public StringConstantPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets value of this token (its string constant).
    /// </summary>
    public string Value
    {
      get
      {
        return m_strValue;
      }
      set
      {
        if( value.Length > byte.MaxValue )
          throw new ArgumentOutOfRangeException( "value", "String is too long." );

        m_strValue = value;
        m_compressed = 1;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return ( m_compressed == 1 ) ? m_strValue.Length * 2 + 3 : m_strValue.Length + 3;
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
      string result = m_strValue;

      if( result != null )
        result = result.Replace( "\"", "\"\"" );

      return "\"" + result + "\"";
    }
    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes that correspond to the token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] str;

      if( m_compressed == 1 )
      {
        str = Encoding.Unicode.GetBytes( m_strValue );
      }
      else
      {
        str = BiffRecordRaw.LatinEncoding.GetBytes( m_strValue );
      }

      byte[] result = new byte[ str.Length + 3 ];
      result[ 0 ] = ( byte )this.TokenCode;
      result[ 1 ] = ( byte )m_strValue.Length;
      result[ 2 ] = m_compressed;
      str.CopyTo( result, 3 );

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
      int iFullLength;
      m_compressed = provider.ReadByte( offset + 1 );
      m_strValue = provider.ReadString8Bit( offset, out iFullLength );
      offset += iFullLength;
    }
    #endregion
  }
}
