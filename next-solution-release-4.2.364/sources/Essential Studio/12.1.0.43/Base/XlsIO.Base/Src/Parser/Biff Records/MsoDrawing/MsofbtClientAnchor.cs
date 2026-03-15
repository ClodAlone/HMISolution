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
using System.Collections;
using System.IO;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  // TODO: Find out meaning of the data array.
  /// <summary>
  /// Summary description for MsofbtClientAnchor.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtClientAnchor ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtClientAnchor : MsoBase
  {
    #region Class constants
    /// <summary>
    /// Mask for column / row index.
    /// </summary>
    private const uint DEF_CELL_MASK = 0xFFFF;
    /// <summary>
    /// Mask for offset index.
    /// </summary>
    private const uint DEF_OFFSET_MASK = 0xFFFF0000;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_OFFSET_START_BIT = 16;
    /// <summary>
    /// Length for short data.
    /// </summary>
    private const int DEF_SHORT_LENGTH = 8;
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 18;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bNotMoveWithCell;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bNotSizeWithCell;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 2, 4 ) ]
    private uint m_uiLeft;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 10, 4 ) ]
    private uint m_uiRight;
    /// <summary>
    /// Indicates whether it is short version of client anchor. Used in headers/footers.
    /// </summary>
    private bool m_bShortVersion;
    /// <summary>
    /// Zero-based top row index.
    /// </summary>
    private int m_iTopRow;
    /// <summary>
    /// Top row offset.
    /// </summary>
    private int m_iTopOffset;
    /// <summary>
    /// Zero-based bottom row index.
    /// </summary>
    private int m_iBottomRow;
    /// <summary>
    /// Bottom row offset.
    /// </summary>
    private int m_iBottomOffset;
    /// <summary>
    /// Indicates whether this anchor is oneCellAnchor (used in Excel 2007 format).
    /// </summary>
    private bool m_bOneCellAnchor;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
      set
      {
        m_usOptions = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSizeWithCell
    {
      get
      {
        return !m_bNotSizeWithCell;
      }
      set
      {
        m_bNotSizeWithCell = !value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsMoveWithCell
    {
      get
      {
        return !m_bNotMoveWithCell;
      }
      set
      {
        m_bNotMoveWithCell = !value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LeftColumn
    {
      get
      {
        return ( int )GetUInt32BitsByMask( m_uiLeft, DEF_CELL_MASK );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiLeft, DEF_CELL_MASK, ( uint )value );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int RightColumn
    {
      get
      {
        return ( int )GetUInt32BitsByMask( m_uiRight, DEF_CELL_MASK );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiRight, DEF_CELL_MASK, ( uint )value );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int TopRow
    {
      get
      {
        return m_iTopRow;
        //return ( int )GetUInt32BitsByMask( m_uiTop, DEF_CELL_MASK );
      }
      set
      {
        m_iTopRow = value;
        //SetUInt32BitsByMask( ref m_uiTop, DEF_CELL_MASK, ( uint )value );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int BottomRow
    {
      get
      {
        return m_iBottomRow;
        //return ( int )GetUInt32BitsByMask( m_uiBottom, DEF_CELL_MASK );
      }
      set
      {
        m_iBottomRow = value;
        //SetUInt32BitsByMask( ref m_uiBottom, DEF_CELL_MASK, ( uint )value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LeftOffset
    {
      get
      {
        return ( int )( GetUInt32BitsByMask( m_uiLeft, DEF_OFFSET_MASK )
          >> DEF_OFFSET_START_BIT );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiLeft, DEF_OFFSET_MASK,
          ( uint )( value << DEF_OFFSET_START_BIT ) );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int TopOffset
    {
      get
      {
        return m_iTopOffset;
        //return ( int )( GetUInt32BitsByMask( m_uiTop, DEF_OFFSET_MASK )
        //  >> DEF_OFFSET_START_BIT );
      }
      set
      {
        m_iTopOffset = value;
        //SetUInt32BitsByMask( ref m_uiTop, DEF_OFFSET_MASK,
        //  ( uint )( value << DEF_OFFSET_START_BIT ) );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int RightOffset
    {
      get
      {
        return ( int )( GetUInt32BitsByMask( m_uiRight, DEF_OFFSET_MASK )
          >> DEF_OFFSET_START_BIT );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiRight, DEF_OFFSET_MASK, 
          ( uint )( value << DEF_OFFSET_START_BIT ) );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int BottomOffset
    {
      get
      {
        return m_iBottomOffset;
        //return ( int )( GetUInt32BitsByMask( m_uiBottom, DEF_OFFSET_MASK )
        //  >> DEF_OFFSET_START_BIT );
      }
      set
      {
        m_iBottomOffset = value;
        //SetUInt32BitsByMask( ref m_uiBottom, DEF_OFFSET_MASK,
        //  ( uint )( value << DEF_OFFSET_START_BIT ) );
      }
    }
    /// <summary>
    /// Indicates whether it is short version of client anchor. Used in headers/footers.
    /// </summary>
    public bool IsShortVersion
    {
      get
      {
        return m_bShortVersion;
      }
      set
      {
        m_bShortVersion = value;
      }
    }
    /// <summary>
    /// Indicates whether parent shape should be stored as oneCellAnchor or not.
    /// Default value false.
    /// </summary>
    public bool OneCellAnchor
    {
      get
      {
        return m_bOneCellAnchor;
      }
      set
      {
        m_bOneCellAnchor = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public MsofbtClientAnchor( MsoBase parent )
      : base( parent )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="data"></param>
    /// <param name="iOffset"></param>
    public MsofbtClientAnchor( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      uint uiTop = 0;
      uint uiBottom = 0;

      SetUInt32BitsByMask( ref uiTop, DEF_CELL_MASK, ( uint )m_iTopRow );
      SetUInt32BitsByMask( ref uiTop, DEF_OFFSET_MASK,
        ( uint )( m_iTopOffset << DEF_OFFSET_START_BIT ) );

      SetUInt32BitsByMask( ref uiBottom, DEF_CELL_MASK, ( uint )m_iBottomRow );
      SetUInt32BitsByMask( ref uiBottom, DEF_OFFSET_MASK,
        ( uint )( m_iBottomOffset << DEF_OFFSET_START_BIT ) );

      if( !m_bShortVersion )
      {
        m_iLength = DEF_RECORD_SIZE;

        SetBitInVar( ref m_usOptions, m_bNotMoveWithCell, 0 );
        SetBitInVar( ref m_usOptions, m_bNotSizeWithCell, 1 );
        WriteUInt16( stream, m_usOptions );

        WriteUInt32( stream, m_uiLeft );
        WriteUInt32( stream, uiTop );
        WriteUInt32( stream, m_uiRight );
        WriteUInt32( stream, uiBottom );
      }
      else
      {
        m_iLength = DEF_SHORT_LENGTH;
        WriteUInt32( stream, m_uiLeft );
        WriteUInt32( stream, uiTop );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      m_bShortVersion = ( m_iLength == DEF_SHORT_LENGTH );
      uint uiTop;
      uint uiBottom = 0;

      if( !m_bShortVersion )
      {
        //AutoExtractFields();
        m_usOptions = ReadUInt16( stream );
        m_bNotMoveWithCell = GetBitFromVar( m_usOptions, 0 );
        m_bNotSizeWithCell = GetBitFromVar( m_usOptions, 1 );
        m_uiLeft = ReadUInt32( stream );
        uiTop = ReadUInt32( stream );
        m_uiRight = ReadUInt32( stream );
        uiBottom = ReadUInt32( stream );
      }
      else
      {
        m_uiLeft = ReadUInt32( stream );
        uiTop = ReadUInt32( stream );
      }

      m_iTopRow = ( int )GetUInt32BitsByMask( uiTop, DEF_CELL_MASK );
      m_iTopOffset = ( int )( GetUInt32BitsByMask( uiTop, DEF_OFFSET_MASK )
        >> DEF_OFFSET_START_BIT );

      m_iBottomRow = ( int )GetUInt32BitsByMask( uiBottom, DEF_CELL_MASK );
      m_iBottomOffset = ( int )( GetUInt32BitsByMask( uiBottom, DEF_OFFSET_MASK )
        >> DEF_OFFSET_START_BIT );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_bShortVersion
        ? DEF_SHORT_LENGTH
        : DEF_RECORD_SIZE;
    }
    #endregion
  }
}
