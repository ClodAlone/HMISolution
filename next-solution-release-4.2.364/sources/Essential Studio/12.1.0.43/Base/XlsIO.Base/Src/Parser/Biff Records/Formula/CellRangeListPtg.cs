#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
	/// <summary>
	/// Represents CellRangeList formula token.
	/// </summary>
  [ Token( FormulaToken.tCellRangeList,          "," ) ]
  public class CellRangeListPtg : BinaryOperationPtg
	{
    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public CellRangeListPtg()
    {
    }
    /// <summary>
    /// Constructs BinaryOperation using string that contains the sign of operation.
    /// </summary>
    /// <param name="operation">String representation of the operation.</param>
    public CellRangeListPtg( string operation )
    {
      if( operation == null )
        throw new ArgumentNullException( "operation" );

      if( operation.Length == 0 )
        throw new ArgumentException( "operation - string cannot be empty" );

      OperationSymbol = operation;
      TokenCode = FormulaToken.tCellRangeList;
    }
    /// <summary>
    /// Constructs token using array with data and offset.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public CellRangeListPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil , int iRow, int iColumn, bool bR1C1)
    {
      return GetOperandsSeparator( formulaUtil );
    }

    #endregion
  }
}
