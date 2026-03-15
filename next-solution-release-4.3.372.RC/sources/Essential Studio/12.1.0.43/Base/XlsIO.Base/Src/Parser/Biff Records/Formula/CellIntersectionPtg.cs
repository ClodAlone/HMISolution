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
  /// Summary description for CellIntersectionPtg.
  /// </summary>
//  [ Token( FormulaToken.tCellRangeIntersection ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class CellIntersectionPtg : Ptg
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public CellIntersectionPtg()
    {
    }
    /// <summary>
    /// Intersections cell ptg.
    /// </summary>
    /// <param name="strFormula"></param>
    public CellIntersectionPtg( string strFormula )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Intersections cell ptg.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Current index.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public CellIntersectionPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    #endregion

    #region Class overrides
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
      return "CellIntersection";
    }
    /// <summary>
    /// Gets size. Override.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 1;
    }
    #endregion
  }
}
