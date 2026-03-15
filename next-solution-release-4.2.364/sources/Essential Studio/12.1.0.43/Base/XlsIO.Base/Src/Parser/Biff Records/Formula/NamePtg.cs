#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Globalization;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
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
  /// This token contains the one-based index to a NAME record.
  /// </summary>
  [ Token ( FormulaToken.tName1 ) ]
  [ Token ( FormulaToken.tName2 ) ]
  [ Token ( FormulaToken.tName3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class NamePtg
    : Ptg
    , IRangeGetter
  {
    #region Class members
    /// <summary>
    /// One-based Index of ExternNameRecord.
    /// </summary>
    private ushort m_usIndex = 0;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public NamePtg()
    {
    }
    /// <summary>
    /// Constructs token using data from a byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public NamePtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the token.</param>
    /// <param name="parent">Workbook that contains this reference.</param>
    public NamePtg( string strFormula, IWorkbook parent )
    {
      IName name = parent.Names[ strFormula ];
      
      if( name == null )
        throw new ArgumentNullException( "Extern name " + strFormula 
          + " does not exist" );

      // index is one-based
      m_usIndex = ( ushort )( name.Index + 1 );
    }
    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the token.</param>
    /// <param name="book">Workbook that contains this reference.</param>
    /// <param name="sheet">Worksheet that contains this reference.</param>
    public NamePtg( string strFormula, IWorkbook book, IWorksheet sheet )
    {
      IName name;

      if( sheet.Names.Contains( strFormula ) )
      {
        name = sheet.Names[ strFormula ];
      }
      else if( book.Names.Contains( strFormula ) )
      {
        name = book.Names[ strFormula ];
      }
      else
      {
        throw new ArgumentException( "Unknown name", strFormula );
      }

      m_usIndex = ( ushort )( name.Index + 1 );
    }
    /// <summary>
    /// Creates token by name index.
    /// </summary>
    /// <param name="iNameIndex">Name index.</param>
    public NamePtg( int iNameIndex )
    {
      m_usIndex = ( ushort )( iNameIndex + 1 );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 5;
    }

    /// <summary>
    /// Gets / sets one-based index of ExternNameRecord.
    /// </summary>
    public ushort ExternNameIndex
    {
      get
      {
        return m_usIndex;
      }
      set
      {
        m_usIndex = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Converts token to string.
    /// </summary>
    /// <returns>String representation of the token.</returns>
    public override string ToString()
    {
      return "( NameIndex = " + m_usIndex.ToString () + " )";
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
      if( formulaUtil == null )
        return ToString();

      WorkbookNamesCollection names;
      names = formulaUtil.ParentWorkbook.Names as WorkbookNamesCollection;
        
      if( names.Count <= m_usIndex - 1 || m_usIndex < 1 )
        throw new ParseException();

      return names[m_usIndex - 1].Name;
    }
    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      BitConverter.GetBytes( this.m_usIndex ).CopyTo( result, 1 );

      return result;
    }

    #endregion

    #region Class static methods
    /// <summary>
    /// Returns token code by index.
    /// </summary>
    /// <param name="index">Index of the token code.</param>
    /// <returns>Required token code.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than 1 or greater than 3.
    /// </exception>
    public static FormulaToken IndexToCode( int index )
    {
      switch( index )
      {
        case 1: return FormulaToken.tName1;
        case 2: return FormulaToken.tName2;
        case 3: return FormulaToken.tName3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region IRangeGetter methods
    /// <summary>
    /// Returns range represented by the token that implements this interface.
    /// </summary>
    /// <param name="book">Workbook that contains range.</param>
    /// <param name="sheet">Parent sheet.</param>
    /// <returns>Range represented by the token.</returns>
    public IRange GetRange( IWorkbook book, IWorksheet sheet )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      IName name = book.Names[ ExternNameIndex - 1 ];

      return name as IRange;
    }

    /// <summary>
    /// Returns rectangle represented by the token that implements this interface.
    /// </summary>
    /// <returns>Rectangle represented by the token.</returns>
    public Rectangle GetRectangle()
    {
      throw new NotSupportedException();
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
      m_usIndex = provider.ReadUInt16( offset );
      offset += GetSize( version ) - 1;
    }
    #endregion
  }
}
