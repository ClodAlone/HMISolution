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
  /// This class represents an integer token in formula.
  /// </summary>
  [ Token ( FormulaToken.tInteger ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class IntegerPtg : Ptg
  {
    #region Class members
    /// <summary>
    /// Contains an unsigned 16-bit integer value in the range from 0 to 65535.
    /// </summary>
    public ushort m_usValue;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public IntegerPtg()
    {
    }
    /// <summary>
    /// Constructs token by integer value.
    /// </summary>
    /// <param name="value">Integer value that will be placed into token.</param>
    public IntegerPtg( ushort value ): base()
    {
      this.TokenCode = FormulaToken.tInteger;
      this.Value = value;
    }

    /// <summary>
    /// Constructs token by string value.
    /// </summary>
    /// <param name="value">String value that will be parsed into unsigned short.</param>
    public IntegerPtg( string value )
      : this( ushort.Parse( value ) )
    {
    }

    /// <summary>
    /// Constructs token using data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public IntegerPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Get / sets value contained by this token.
    /// </summary>
    public ushort Value
    {
      get
      {
        return m_usValue;
      }
      set
      {
        m_usValue = value;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 3;
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
      return m_usValue.ToString ();
    }
    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes corresponding to the token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      BitConverter.GetBytes( m_usValue ).CopyTo( result, 1 );
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
      m_usValue = provider.ReadUInt16( offset );

      offset += 2;
    }
    #endregion
  }
}
