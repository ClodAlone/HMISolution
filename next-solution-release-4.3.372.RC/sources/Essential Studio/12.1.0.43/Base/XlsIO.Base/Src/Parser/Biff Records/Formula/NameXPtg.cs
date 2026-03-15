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
  /// This token contains the index to a NAME or EXTERNNAME record. It occurs by using
  /// internal or external names, add-in functions, DDE links, or linked OLE objects.
  /// </summary>
  [ Token ( FormulaToken.tNameX1 ) ]
  [ Token ( FormulaToken.tNameX2 ) ]
  [ Token ( FormulaToken.tNameX3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class NameXPtg
    : Ptg
    , ISheetReference
    , IRangeGetter
  {
    #region Class members
    /// <summary>
    /// Index to a REF entry in an EXTERNSHEET record in the Link Table.
    /// </summary>
    private ushort m_usRefIndex = 0;
    /// <summary>
    /// One-based index to a NAME record or EXTERNNAME record.
    /// </summary>
    private ushort m_usNameIndex = 0;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public NameXPtg()
    {
    }
    /// <summary>
    /// Constructs token using data from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public NameXPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }

    /// <summary>
    /// Creates token by its string representation.
    /// </summary>
    /// <param name="strFormula">String representation of the token.</param>
    /// <param name="parent">Workbook that contains this reference.</param>
    public NameXPtg( string strFormula, IWorkbook parent )
    {
      IName name = parent.Names[ strFormula ];
      
      if( name == null )
        throw new ArgumentNullException( "Extern name " + strFormula 
          + " does not exist" );

      // index is one-based
      m_usNameIndex = (ushort) ( name.Index +1 );
      Ptg ptg = ((NameImpl) name).Record.FormulaTokens[ 0 ];
      
      if( ptg is Area3DPtg )
      {
        m_usRefIndex = ((Area3DPtg) ptg).RefIndex;
      }
      else if( ptg is Ref3DPtg )
      {
        m_usRefIndex = ((Ref3DPtg) ptg).RefIndex;
      }
    }
    /// <summary>
    /// Creates new instance of NameX token by extern workbook index and name index in the workbook.
    /// </summary>
    /// <param name="iBookIndex">Zero-based book index.</param>
    /// <param name="iNameIndex">Zero-based name index.</param>
    public NameXPtg( int iBookIndex, int iNameIndex )
    {
      m_usRefIndex = ( ushort )iBookIndex;
      m_usNameIndex = ( ushort )( iNameIndex + 1 );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets one-based index of ExternNameRecord or NameRecord.
    /// </summary>
    public ushort NameIndex
    {
      get
      {
        return m_usNameIndex;
      }
      set
      {
        m_usNameIndex = value;
      }
    }
    /// <summary>
    /// Gets / sets index to REF entry in EXTERNSHEET record in the Link Table.
    /// </summary>
    public ushort RefIndex
    {
      get
      {
        return m_usRefIndex;
      }
      set
      {
        m_usRefIndex = value;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the record.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 7;
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
      return ToString( formulaUtil, iRow, iColumn, bR1C1, numberFormat, isForSerialization, null );
    }
    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int row, int col, bool bR1C1,
      NumberFormatInfo numberInfo, bool isForSerialization, IWorksheet sheet )
    {
      if( formulaUtil == null )
        return string.Format( "( ExternNameIndex = {0}, RefIndex = {1} )", m_usNameIndex, m_usRefIndex );

      WorkbookImpl book = ( WorkbookImpl )formulaUtil.ParentWorkbook;

      if( book.IsLocalReference( m_usRefIndex ) )
      {
        if( (formulaUtil.ParentWorkbook as WorkbookImpl).InnerNamesColection.Count <= m_usNameIndex - 1 || m_usNameIndex < 1 )
          throw new ParseException();

        IName name = (formulaUtil.ParentWorkbook as WorkbookImpl).InnerNamesColection[m_usNameIndex - 1];
        IWorksheet parentSheet = name.Worksheet;

        return ( parentSheet == sheet || parentSheet == null ) ?
          name.Name :
          string.Format("'{0}'!{1}", parentSheet.Name, name.Name);

        //return formulaUtil.ParentWorkbook.Names[ m_usNameIndex - 1 ].Name;
      }
      else
      {
        int iBookIndex = book.GetBookIndex( m_usRefIndex );
        ExternWorkbookImpl externBook = book.ExternWorkbooks[ iBookIndex ];
        ExternNameImpl externName = ( ExternNameImpl )externBook.ExternNames[ m_usNameIndex - 1 ];
        if (book.Version == ExcelVersion.Excel97to2003 || externBook.URL == null || !isForSerialization)
        {
            if (Area3DPtg.ValidateSheetName(externName.Name))
                return string.Format("'{0}'!{1}", externBook.URL, externName.Name);
            else if(externBook.URL==null)
                return externName.Name;
            else
                return string.Format("'{0}'!{1}", externBook.URL, externName.Name);
        }
        else if (externBook.IsAddInFunctions && externBook.Worksheets.Count == 0)
        {
            return string.Format("'{0}'!'{1}'", externBook.URL, externName.Name);
        }
        else
            return string.Format("{0}!{1}", "[" + (iBookIndex + 1) + "]", externName.Name);
      }
    }
    /// <summary>
    /// Calls ToString method of the base (not 3d) class.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public string BaseToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1 )
    {
      return ToString( formulaUtil, iRow, iColumn, bR1C1 );
    }
    /// <summary>
    /// Converts token to array of bytes.
    /// </summary>
    /// <returns>Array of bytes corresponding to the token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      BitConverter.GetBytes( m_usRefIndex ).CopyTo( result, 1 );
      BitConverter.GetBytes( m_usNameIndex ).CopyTo( result, 3 );

      return result;
    }

    #endregion

    #region Class static methods
    /// <summary>
    /// Returns token code by index
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
        case 1: return FormulaToken.tNameX1;
        case 2: return FormulaToken.tNameX2;
        case 3: return FormulaToken.tNameX3;

        default: throw new ArgumentOutOfRangeException( "index" );
      }
    }
    #endregion

    #region IRangeGetter methods
    /// <summary>
    /// Returns range represented by the token that implements this interface.
    /// </summary>
    /// <param name="book">Workbook that contains range.</param>
    /// <param name="sheet">Parent worksheet</param>
    /// <returns>Range represented by the token.</returns>
    public IRange GetRange( IWorkbook book, IWorksheet sheet )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      WorkbookImpl bookImpl = ( WorkbookImpl )book;

      bookImpl.CheckForInternalReference( RefIndex );
      NameImpl name = ( NameImpl )bookImpl.Names[ NameIndex - 1 ];

      return name;
    }

    /// <summary>
    /// Returns rectangle represented by the token that implements this interface.
    /// </summary>
    /// <returns>Rectangle represented by the token.</returns>
    public Rectangle GetRectangle()
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public Ptg UpdateRectangle( Rectangle rectangle )
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
      m_usRefIndex = provider.ReadUInt16( offset );
      offset += 2;
      m_usNameIndex = provider.ReadUInt16( offset );
      offset += GetSize( version ) - 3;
    }
    #endregion
  }
}
