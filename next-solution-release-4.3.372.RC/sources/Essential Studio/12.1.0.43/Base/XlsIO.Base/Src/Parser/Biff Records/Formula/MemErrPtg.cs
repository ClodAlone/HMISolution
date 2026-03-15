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
  /// Not fully implemented because of lack of documentation.
  /// </summary>
  [ Token( FormulaToken.tMemErr1 ) ]
  [ Token( FormulaToken.tMemErr2 ) ]
  [ Token( FormulaToken.tMemErr3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class MemErrPtg : Ptg
  {
    #region Class constants
    /// <summary>
    /// Size of the token.
    /// </summary>
    private const int SIZE = 7;
    #endregion

    #region Class members
    /// <summary>
    /// Token data.
    /// </summary>
    private byte[] m_arrData = new byte[ SIZE - 1 ];
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public MemErrPtg()
    {
    }
    /// <summary>
    /// Creates token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data in the data array.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public MemErrPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class methods
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
      return "(MemErr not implemented.)";
    }

    /// <summary>
    /// Converts token to the array of bytes.
    /// </summary>
    /// <returns></returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      m_arrData.CopyTo( result, 1 );
      return result;
    }
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 7;
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
      provider.CopyTo( offset, m_arrData, 0, m_arrData.Length );
      offset += m_arrData.Length;
    }
    #endregion
  }
}
