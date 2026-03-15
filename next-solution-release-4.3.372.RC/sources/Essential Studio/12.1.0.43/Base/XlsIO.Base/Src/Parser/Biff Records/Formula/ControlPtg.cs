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
  /// This class represents the control token (this can be tExp or tTbl).
  /// </summary>
  [ Token( FormulaToken.tTbl ) ]
  [ Token( FormulaToken.tExp ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ControlPtg : RefPtg
  {
    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public ControlPtg()
    {
    }
    /// <summary>
    /// Constructs token from byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public ControlPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iRow"></param>
    /// <param name="iColumn"></param>
    public ControlPtg( int iRow, int iColumn )
    {
//      if( usColumn > byte.MaxValue )
//        throw new ArgumentOutOfRangeException( "usColumn" );

      RowIndex = iRow;
      ColumnIndex = iColumn;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public override bool IsColumnIndexRelative
    {
      get
      {
        return true;
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool IsRowIndexRelative
    {
      get
      {
        return true;
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          return 5;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          return 9;

        default:
          throw new ArgumentOutOfRangeException( "version" );
      }
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
      return "( ControlToken " + RangeImpl.GetCellName( ColumnIndex + 1, RowIndex + 1 ) + ")";
    }

    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes that represents this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = new byte[ GetSize( version ) ];//base.ToByteArray( version );
      result[ 0 ] = ( byte )TokenCode;

      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          BitConverter.GetBytes( ( ushort )RowIndex ).CopyTo( result, 1 );
          BitConverter.GetBytes( ( ushort )ColumnIndex ).CopyTo( result, 3 );
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          int iOffset = 1;
          BitConverter.GetBytes( RowIndex ).CopyTo( result, iOffset );
          iOffset += ExcelConstants.IntSize;

          BitConverter.GetBytes( ColumnIndex ).CopyTo( result, iOffset );
          break;

        default:
          throw new ArgumentOutOfRangeException( "version" );
      }

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    public override FormulaToken GetCorrespondingErrorCode()
    {
      return FormulaToken.tRef2;
    }
    /// <summary>
    /// Moves token into different worksheet.
    /// </summary>
    /// <param name="result">Token to move</param>
    /// <param name="iSourceSheetIndex">Source sheet index.</param>
    /// <param name="rectSource">Source rectangle.</param>
    /// <param name="iDestSheetIndex">Destination sheet index.</param>
    /// <param name="iRowOffset">Row offset.</param>
    /// <param name="iColOffset">Column offset.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Modified token.</returns>
    protected override Ptg MoveIntoDifferentSheet(RefPtg result, int iSourceSheetIndex,
      Rectangle rectSource, int iDestSheetIndex, int iRowOffset,
      int iColOffset, WorkbookImpl book )
    {
      return Offset( iRowOffset, iColOffset, book );
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
      if( version == ExcelVersion.Excel97to2003 )
      {
        RowIndex = provider.ReadUInt16( offset );
        offset += 2;
        ColumnIndex = ( byte )provider.ReadUInt16( offset );
        offset += 2;
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        RowIndex = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;

        ColumnIndex = provider.ReadInt32( offset );
        offset += ExcelConstants.IntSize;
      }
    }
    #endregion
  }
}
