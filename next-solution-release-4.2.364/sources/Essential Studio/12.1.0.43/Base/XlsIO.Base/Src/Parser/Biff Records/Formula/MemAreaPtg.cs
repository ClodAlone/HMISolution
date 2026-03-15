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
  /// Summary description for MemAreaPtg.
  /// </summary>
  [ Token( FormulaToken.tMemArea1 ) ]
  [ Token( FormulaToken.tMemArea2 ) ]
  [ Token( FormulaToken.tMemArea3 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class MemAreaPtg
    : Ptg
    , IAdditionalData
  {
    #region Class constants
    /// <summary>
    /// Size of the one rectangle data.
    /// </summary>
    private const int DEF_RECT_SIZE = 8;
    /// <summary>
    /// Size of the header block.
    /// </summary>
    private const int DEF_HEADER_SIZE = 7;
    #endregion

    #region Class members
    /// <summary>
    /// Reserved.
    /// </summary>
    private int m_iReserved;
    /// <summary>
    /// The length of the reference subexpression.
    /// </summary>
    private ushort m_usSubExpressionLength;
    /// <summary>
    /// Subexpression.
    /// </summary>
    private Ptg[] m_arrSubexpression;
    /// <summary>
    /// 
    /// </summary>
    private Rectangle[] m_arrRects;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public MemAreaPtg()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="offset"></param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public MemAreaPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strFormula"></param>
    public MemAreaPtg( string strFormula )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns the length of the reference subexpression. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ushort SubExpressionLength
    {
      get
      {
        return m_usSubExpressionLength;
      }
    }
    /// <summary>
    /// Returns the reference subexpression. Read-only.
    /// </summary>
    public Ptg[] Subexpression
    {
      get
      {
        return m_arrSubexpression;
      }
    }
    /// <summary>
    /// Rectangles.
    /// </summary>
    public Rectangle[] Rectangles
    {
      get
      {
        return m_arrRects;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Size of the ptg token. Read-only.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return m_usSubExpressionLength + DEF_HEADER_SIZE;
    }
    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      return base.ToByteArray( version );
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
      return "MemArea";
    }
    #endregion

    #region IAdditionalData Members
    /// <summary>
    /// Extracts additional data.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the additional data.</param>
    /// <returns>Offset after extracting all required data.</returns>
    public int ReadArray( DataProvider provider, int offset )
    {
      ushort usRectCount = provider.ReadUInt16( offset );
      //usRectCount++;
      offset += 2;
      m_arrRects = new Rectangle[ usRectCount ];

      for( int i = 0; i < usRectCount; i++ )
      {
        ushort usFirstRow = provider.ReadUInt16( offset );
        offset += 2;

        ushort usLastRow = provider.ReadUInt16( offset );
        offset += 2;

        ushort usFirstColumn = provider.ReadUInt16( offset );
        offset += 2;

        ushort usLastColumn = provider.ReadUInt16( offset );
        offset += 2;

        m_arrRects[ i ] = Rectangle.FromLTRB( usFirstColumn, usFirstRow, usLastColumn, usLastRow );
      }

      return offset;
    }

    /// <summary>
    /// Returns size of the additional data. Read-only.
    /// </summary>
    public int AdditionalDataSize
    {
      get
      {
        int iRectsCount = ( m_arrRects != null ) ?
          m_arrRects.Length :
          0;

        return 2 + iRectsCount * DEF_RECT_SIZE;
      }
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
      m_iReserved = provider.ReadInt32( offset );
      offset += 4;

      m_usSubExpressionLength = provider.ReadUInt16( offset );
      offset += 2;

      int iFinalOffset;
      m_arrSubexpression = FormulaUtil.ParseExpression( provider, offset,
        m_usSubExpressionLength/* + offset*/, out iFinalOffset, version );
      offset += m_usSubExpressionLength;
    }
    #endregion
  }
}