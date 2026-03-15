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

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif (WP)
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
  /// This token contains the last 3D reference or external reference to a cell in a
  /// deleted row or column.
  /// </summary>
  [ Token( FormulaToken.tRefErr3d1 ) ]
  [ Token( FormulaToken.tRefErr3d2 ) ]
  [ Token( FormulaToken.tRefErr3d3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class RefError3dPtg
    : Ref3DPtg
    , ISheetReference
    , IRangeGetter
  {
    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public RefError3dPtg()
    {
    }
    /// <summary>
    /// Creates token using data from an array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public RefError3dPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the token.</param>
    /// <param name="parent">Workbook that contains this reference.</param>
    public RefError3dPtg( string strFormula, IWorkbook parent )
      : base( strFormula, parent )
    {
      TokenCode = FormulaToken.tRefErr3d1;
    }
    /// <summary>
    /// Creates error token based on Ref3D token.
    /// </summary>
    /// <param name="dataHolder">Token to get data from.</param>
    public RefError3dPtg( Ref3DPtg dataHolder )
      : base( dataHolder )
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Converts token to string.
    /// </summary>
    /// <returns>String representation of this token.</returns>
    public override string ToString()
    {
      return "RefErr3d (" + RefIndex.ToString() + base.ToString ()  + ")";
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
      string sheetName = GetSheetName( formulaUtil.ParentWorkbook, RefIndex );
      return ( formulaUtil != null && sheetName != null )?
        string.Format( "'{0}'!{1}", sheetName, RefErrorPtg.ReferenceError ) :
        RefErrorPtg.ReferenceError;
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
      return this;
    }

    /// <summary>
    /// Returns token code by index.
    /// </summary>
    /// <param name="index">Index of the token code.</param>
    /// <returns>Required token code.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or greater than 3.
    /// </exception>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tRefErr3d1;
        case 2: return FormulaToken.tRefErr3d2;
        case 3: return FormulaToken.tRefErr3d3;

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
      return null;
    }
    #endregion
  }
}
