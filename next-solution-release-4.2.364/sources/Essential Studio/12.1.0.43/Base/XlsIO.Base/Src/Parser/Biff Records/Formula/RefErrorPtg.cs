#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This token contains the last reference to a deleted cell in the same sheet.
  /// </summary>
  [ ErrorCode( RefErrorPtg.ReferenceError, 23 ) ]
  [ Token( FormulaToken.tRefErr1 ) ]
  [ Token( FormulaToken.tRefErr2 ) ]
  [ Token( FormulaToken.tRefErr3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class RefErrorPtg
    : RefPtg
    , IRangeGetter
  {
    #region Constants
    /// <summary>
    /// 
    /// </summary>
    public const string ReferenceError = "#REF!";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static RefErrorPtg()
    {
    }
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public RefErrorPtg()
    {
    }
    /// <summary>
    /// Creates token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public RefErrorPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorName"></param>
    public RefErrorPtg( string errorName ) : base( "A1" )
    {
      TokenCode = FormulaToken.tRefErr2;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorName"></param>
    /// <param name="book"></param>
    public RefErrorPtg( string errorName, IWorkbook book )
      : this( errorName )
    {
    }
    /// <summary>
    /// Creates error token based on specified ref token.
    /// </summary>
    /// <param name="dataHolder">Token to take data from.</param>
    public RefErrorPtg( RefPtg dataHolder )
      : base( dataHolder )
    {
      int index = RefPtg.CodeToIndex( dataHolder.TokenCode );
      TokenCode = RefErrorPtg.IndexToCode( index );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <returns>String representation of this token.</returns>
    public override string ToString()
    {
      return "RefErr (" + base.ToString() + ")";
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
      return ReferenceError;
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
    /// Converts reference index to token code.
    /// </summary>
    /// <param name="index">Reference index.</param>
    /// <returns>Token code.</returns>
    new public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tRefErr1;
        case 2: return FormulaToken.tRefErr2;
        case 3: return FormulaToken.tRefErr3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    /// <summary>
    /// Converts tokens from regular formula into tokens from shared formula.
    /// </summary>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="iRow">Represents first row from cells range of shared formula.Zero-base.</param>
    /// <param name="iColumn">Represents first column from cells range of shared formula.Zero-based.</param>
    /// <returns>New token for shared formula.</returns>
    public override Ptg ConvertPtgToNPtg(IWorkbook parent, int iRow, int iColumn)
    {
        int iNewColumn = IsColumnIndexRelative ? ColumnIndex - iColumn : ColumnIndex;
        int iNewRow = IsRowIndexRelative ? RowIndex - iRow : RowIndex;

        FormulaToken token = RefErrorPtg.IndexToCode(this.CodeToIndex());

        RefErrorPtg result = (RefErrorPtg)FormulaUtil.CreatePtg(token);
         
       
            if (parent.Version == ExcelVersion.Excel97to2003)
            {
                result.RowIndex = iNewRow;
                result.ColumnIndex = iNewColumn;
            }
            else
            {
                RefPtg reference = (RefPtg)result;
                reference.RowIndex = iNewRow;
                reference.ColumnIndex = iNewColumn;
            }

            result.Options = Options;


            return result;
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
