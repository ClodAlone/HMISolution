#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Globalization;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains an IEEE floating-point number.
  /// </summary>
  [ Token ( FormulaToken.tNumber ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class DoublePtg : Ptg
  {
    #region Class members
    /// <summary>
    /// Floating-point value.
    /// </summary>
    private double m_value;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public DoublePtg()
    {
    }
    /// <summary>
    /// Constructs token by double value.
    /// </summary>
    /// <param name="value">Double value.</param>
    public DoublePtg( double value )
    {
      TokenCode = FormulaToken.tNumber;
      Value = value;
    }

    /// <summary>
    /// Constructs token by string value that represents number.
    /// </summary>
    /// <param name="value">String value that will be converted to double.</param>
    public DoublePtg( string value )
      : this( value, null )
    {
    }
    /// <summary>
    /// Constructs token by string value that represents number.
    /// </summary>
    /// <param name="value">String value that will be converted to double.</param>
    /// <param name="numberInfo">Represents culture for parsing from string.</param>
    public DoublePtg( string value, NumberFormatInfo numberInfo )
    {
      double dValue = ( numberInfo == null ) ? Double.Parse( value ) : Double.Parse( value, numberInfo );

      TokenCode = FormulaToken.tNumber;
      Value = dValue;
    }
    /// <summary>
    /// Constructs token using data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public DoublePtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets value that is represented by this token.
    /// </summary>
    public double Value
    {
      get
      {
        return m_value;
      }
      set
      {
        m_value = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 9;
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
      if( numberFormat == null )
        return m_value.ToString();

      return m_value.ToString( numberFormat );
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <param name="numberInfo">Represents current number info.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1, NumberFormatInfo numberInfo )
    {
      return ToString( formulaUtil, iRow, iColumn, bR1C1, numberInfo, false );
    }
    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      BitConverter.GetBytes( m_value ).CopyTo( result, 1 );

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
      m_value = provider.ReadDouble( offset );

      offset += 8;
    }
    #endregion
  }
}
