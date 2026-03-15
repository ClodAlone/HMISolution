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
  /// A missing argument in a function argument list that is stored as a tMissArg token.
  /// This token does not contain any additional data.
  /// </summary>
  [ Token ( FormulaToken.tMissingArgument ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class MissingArgumentPtg : Ptg
  {
    #region Class constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public MissingArgumentPtg()
    {
      TokenCode = FormulaToken.tMissingArgument;
    }
    /// <summary>
    /// Constructs token using data from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public MissingArgumentPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Almost the same as default constructor.
    /// </summary>
    /// <param name="strFormula">Must be equal to string.Empty.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified string is not empty.
    /// </exception>
    public MissingArgumentPtg( string strFormula )
    {
      if( strFormula != string.Empty )
        throw new ArgumentOutOfRangeException( "strFormula", "should be empty string" );

      TokenCode = FormulaToken.tMissingArgument;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 1;
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
      return "";
    }
    #endregion
  }
}
