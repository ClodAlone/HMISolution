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

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
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
  /// Summary description for AreaError3dPtg.
  /// </summary>
  [ Token( FormulaToken.tAreaErr3d1 ) ]
  [ Token( FormulaToken.tAreaErr3d2 ) ]
  [ Token( FormulaToken.tAreaErr3d3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class AreaError3DPtg
    : Area3DPtg
    , IRangeGetter
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public AreaError3DPtg()
    {
    }
    /// <summary>
    /// Creates token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public AreaError3DPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ptg"></param>
    public AreaError3DPtg( Area3DPtg ptg )
      : base( ptg )
    {
      this.TokenCode = ( FormulaToken )( ( int )ptg.TokenCode - ( int )FormulaToken.tArea3d1
        + ( int )FormulaToken.tAreaErr3d1 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="book"></param>
    public AreaError3DPtg( string value, IWorkbook book )
      : base( value, book )
    {
      TokenCode = FormulaToken.tAreaErr3d1;
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
      IWorkbook book = ( formulaUtil != null ) ?
        formulaUtil.ParentWorkbook :
        null;

      string strSheetName = Ref3DPtg.GetSheetName( book, RefIndex );
      return ( book != null ) ?
        string.Format( "'{0}'!{1}", strSheetName, RefErrorPtg.ReferenceError ) :
        RefErrorPtg.ReferenceError;
    }
    /// <summary>
    /// Converts token code to index (inverse operation to IndexToCode).
    /// </summary>
    /// <returns>Reference index.</returns>
    public override int CodeToIndex()
    {
      return CodeToIndex( TokenCode );
    }
    public override Ptg Offset( int iCurSheetIndex, int iTokenRow, int iTokenColumn, int iSourceSheetIndex,
        Rectangle rectSource, int iDestSheetIndex, Rectangle rectDest, out bool bChanged, WorkbookImpl book )
    {
      bChanged = false;
      return ( Ptg )Clone();
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts specified index to the token code.
    /// </summary>
    /// <param name="index">Function parameter index.</param>
    /// <returns>Token code that corresponds to the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or greater than 3.
    /// </exception>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tAreaErr3d1;
        case 2: return FormulaToken.tAreaErr3d2;
        case 3: return FormulaToken.tAreaErr3d3;

        default: throw new ArgumentOutOfRangeException( "index",
                   "Must be less than 4 and greater than than 0." );
      }
    }
    /// <summary>
    /// Converts specified token code to index.
    /// </summary>
    /// <param name="code">Token code for which index is required.</param>
    /// <returns>Index that corresponds to the code.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is not one of tAreaErr3d1, tAreaErr3d2, tAreaErr3d3.
    /// </exception>
    new public static int CodeToIndex( FormulaToken code )
    {
      switch( code )
      {
        case FormulaToken.tAreaErr3d1:
          return 1;
        case FormulaToken.tAreaErr3d2:
          return 2;
        case FormulaToken.tAreaErr3d3:
          return 3;

        default:
          throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region IRangeGetter methods
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
