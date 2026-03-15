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
  /// This token contains the last reference to a deleted cell range in the same sheet.
  /// </summary>
  [ Token( FormulaToken.tAreaErr1 ) ]
  [ Token( FormulaToken.tAreaErr2 ) ]
  [ Token( FormulaToken.tAreaErr3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class AreaErrorPtg
    : AreaPtg
    , IRangeGetter
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public AreaErrorPtg()
    {
    }
    /// <summary>
    /// Creates token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public AreaErrorPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="area"></param>
    public AreaErrorPtg( AreaPtg area )
      : base( area )
    {
      this.TokenCode = ( FormulaToken )( ( int )area.TokenCode - ( int )FormulaToken.tArea1
        + ( int )FormulaToken.tAreaErr1 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="book">Parent workbook.</param>
    public AreaErrorPtg( string value, IWorkbook book )
      : base( value, book )
    {
      TokenCode = FormulaToken.tAreaErr1;
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
      return RefErrorPtg.ReferenceError;
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
    /// Converts specified index to token code.
    /// </summary>
    /// <param name="index">Index of the needed token.</param>
    /// <returns>Token that corresponds to the index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than one or greater than 3.
    /// </exception>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tAreaErr1;
        case 2: return FormulaToken.tAreaErr2;
        case 3: return FormulaToken.tAreaErr3;

        default:
          throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts specified token code to index.
    /// </summary>
    /// <param name="code">Token code for which index is required.</param>
    /// <returns>Index that corresponds to the code.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is not one of tAreaErr1, tAreaErr2, tAreaErr3.
    /// </exception>
    new public static int CodeToIndex( FormulaToken code )
    {
      switch( code )
      {
        case FormulaToken.tAreaErr1:
          return 1;
        case FormulaToken.tAreaErr2:
          return 2;
        case FormulaToken.tAreaErr3:
          return 3;

        default:
          throw new ArgumentOutOfRangeException( "index" );
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
