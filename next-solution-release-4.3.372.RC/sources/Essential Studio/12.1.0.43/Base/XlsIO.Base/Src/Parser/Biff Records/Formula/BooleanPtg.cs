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
  /// This token represents a boolean operand in a formula.
  /// </summary>
  [ Token ( FormulaToken.tBoolean ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class BooleanPtg : Ptg
  {
    #region Class members
    /// <summary>
    /// Boolean value.
    /// </summary>
    private bool m_bValue = false;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public BooleanPtg()
    {
    }
    /// <summary>
    /// Constructs token by boolean value.
    /// </summary>
    /// <param name="value">Boolean value that will be placed into this token.</param>
    public BooleanPtg( bool value )
    {
      TokenCode = FormulaToken.tBoolean;
      Value = value;
    }
    /// <summary>
    /// Constructs token by string value.
    /// </summary>
    /// <param name="value">String value, should be valid boolean string.</param>
    public BooleanPtg( string value )
      : this( bool.Parse( value ) )
    {
    }
    /// <summary>
    /// Constructs token using data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public BooleanPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets boolean value that is represented by this class.
    /// </summary>
    public bool Value
    {
      get
      {
        return m_bValue;
      }
      set
      {
        m_bValue = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of this token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 2;
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
      return m_bValue.ToString();
    }
    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      BitConverter.GetBytes( m_bValue ).CopyTo( result, 1 );
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
      m_bValue = provider.ReadBoolean( offset++ );
    }
    #endregion
  }
}

