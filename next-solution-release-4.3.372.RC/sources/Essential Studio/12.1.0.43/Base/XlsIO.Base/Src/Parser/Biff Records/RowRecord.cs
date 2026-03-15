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

using System;
using System.IO;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Stores the row information for the sheet.
  /// </summary>
  [ Biff( TBIFFRecord.Row ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class RowRecord
    : BiffRecordRaw
    , IOutline
  {
    #region Class constants
    /// <summary>
    /// Bit mask for outline level.
    /// </summary>
    public const ushort DEF_OUTLINE_LEVEL_MASK = 0x0007;
    /// <summary>
    /// Maximum row height in points.
    /// </summary>
    public const double DEF_MAX_HEIGHT = 409.5;
    /// <summary>
    /// 
    /// </summary>
    internal const int DEF_RECORD_SIZE = 16;
//    /// <summary>
//    /// Default style.
//    /// </summary>
//    internal const int DEF_DEFAULT_STYLE = 15;
    /// <summary>
    /// Possible option flags.
    /// </summary>
    internal enum OptionFlags
    {
      /// <summary>
      /// Whether or not to collapse this row.
      /// </summary>
      Colapsed = 16,
      /// <summary>
      /// Whether or not to display this row with 0 height.
      /// </summary>
      ZeroHeight = 32,
      /// <summary>
      /// Whether the font and row height are not compatible.
      /// True if they aren't compatible.
      /// </summary>
      BadFontHeight = 64,
      /// <summary>
      /// Whether the row has been formatted (even if its got all blank cells) or
      /// row has explicit default format.
      /// </summary>
      Formatted = 128,
      /// <summary>
      /// If this value is set to False, then Excel will not show row outline
      /// groups.
      /// </summary>
      ShowOutlineGroups = 256,
      /// <summary>
      /// Additional space above the row.
      /// </summary>
      SpaceAbove = 0x10000000,
      /// <summary>
      /// Additional space below the row.
      /// </summary>
      SpaceBelow = 0x20000000,
    }
    #endregion

    #region Class members

    /// <summary>
    /// Index of this row.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRowNumber;

    /// <summary>
    /// Index to column of the first cell which is described by a cell record.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usFirstCol;

    /// <summary>
    /// Index to column of the last cell which is described by a cell record,
    /// increased by 1.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usLastCol;

    /// <summary>
    /// Height of the row, in twips = 1/20 of a point.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usHeigth;

    /// <summary>
    ///  Not used.
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iReserved;

    /// <summary>
    /// Options flag.
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private OptionFlags m_optionFlags = OptionFlags.ShowOutlineGroups;

    /// <summary>
    /// Worksheet object.
    /// </summary>
    private WorksheetImpl m_sheet;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int Options
    {
      get
      {
        return ( int )m_optionFlags;
      }
      set
      {
        m_optionFlags = ( OptionFlags )value;
      }
    }
    /// <summary>
    /// Index of this row.
    /// </summary>
    public ushort RowNumber
    {
      get
      {
        return m_usRowNumber;
      }
      set
      {
        m_usRowNumber = value;
      }
    }

    /// <summary>
    /// Index to column of the first cell which is described by a cell record.
    /// </summary>
    public ushort FirstColumn
    {
      get
      {
        return m_usFirstCol;
      }
      set
      {
        m_usFirstCol = value;
      }
    }

    /// <summary>
    /// Index to column of the last cell which is described by a cell record,
    /// increased by 1.
    /// </summary>
    public ushort LastColumn
    {
      get
      {
        return m_usLastCol;
      }
      set
      {
        m_usLastCol = value;
      }
    }

    /// <summary>
    /// Height of the row, in twips = 1/20 of a point.
    /// </summary>
    public ushort Height
    {
      get
      {
        return m_usHeigth;
      }
      set
      {
        //if( value > DEF_MAX_HEIGHT*20 )
        //  throw new ArgumentOutOfRangeException( "Height" );

        m_usHeigth = value;
        //IsBadFontHeight = true;
      }
    }

    /// <summary>
    /// If the row is formatted, then this is the index to
    /// the extended format record.
    /// </summary>
    public ushort ExtendedFormatIndex
    {
      get
      {
        return ( ushort )( ( ( int )m_optionFlags & 0xfff0000 ) >> 16 );
      }
      set
      {
        int iOptionFlags = ( int )m_optionFlags;
        iOptionFlags &= ( ~0xfff0000 );

        iOptionFlags |= ( ( value << 16 ) & 0xfff0000 );

        if( value != Syncfusion.XlsIO.Implementation.RangeImpl.DEF_NORMAL_STYLE_INDEX )
        {
          IsFormatted = true;
        }

        m_optionFlags = ( OptionFlags )iOptionFlags;
      }
    }

    /// <summary>
    /// The outline level of this row.
    /// Changes some bits of m_usOptionFlags private member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When value is more than 7.
    /// </exception>
    public ushort OutlineLevel
    {
      get
      {
        return ( ushort )( ( int )m_optionFlags & DEF_OUTLINE_LEVEL_MASK );
      }
      set
      {
        if( value > DEF_OUTLINE_LEVEL_MASK )
          throw new ArgumentOutOfRangeException();

        int iOptionFlags = ( int )m_optionFlags;

        iOptionFlags &= ( ~DEF_OUTLINE_LEVEL_MASK );
        iOptionFlags |= ( value & DEF_OUTLINE_LEVEL_MASK );

        m_optionFlags = ( OptionFlags )iOptionFlags;
      }
    }

    /// <summary>
    /// Whether or not to collapse this row.
    /// </summary>
    public bool   IsCollapsed
    {
      get
      {
        return ( m_optionFlags & OptionFlags.Colapsed ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.Colapsed;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.Colapsed;
        }
      }
    }

    /// <summary>
    /// Whether or not to display this row with 0 height.
    /// </summary>
    public bool   IsHidden
    {
      get
      {
        return ( m_optionFlags & OptionFlags.ZeroHeight ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.ZeroHeight;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.ZeroHeight;
        }
      }
    }

    /// <summary>
    /// Whether the font and row height are not compatible.
    /// True if they aren't compatible.
    /// </summary>
    public bool   IsBadFontHeight
    {
      get
      {
        return ( m_optionFlags & OptionFlags.BadFontHeight ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.BadFontHeight;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.BadFontHeight;
        }
      }
    }

    /// <summary>
    /// Whether the row has been formatted (even if it has all blank cells).
    /// </summary>
    public bool   IsFormatted
    {
      get
      {
        return ( m_optionFlags & OptionFlags.Formatted ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.Formatted;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.Formatted;
        }
      }
    }
    /// <summary>
    /// True if there is additional space above the row.
    /// </summary>
    public bool   IsSpaceAboveRow
    {
      get
      {
        return ( m_optionFlags & OptionFlags.SpaceAbove ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.SpaceAbove;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.SpaceAbove;
        }
      }
    }
    /// <summary>
    /// True if there is additional space below the row.
    /// </summary>
    public bool   IsSpaceBelowRow
    {
      get
      {
        return ( m_optionFlags & OptionFlags.SpaceBelow ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.SpaceBelow;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.SpaceBelow;
        }
      }
    }
    /// <summary>
    /// Undocumented bit flag. If it is set to False, then Excel will
    /// not show row groups. Default value is True.
    /// </summary>
    public bool   IsGroupShown
    {
      get
      {
        return ( m_optionFlags & OptionFlags.ShowOutlineGroups ) != 0;
      }
      set
      {
        if( value )
        {
          m_optionFlags |= OptionFlags.ShowOutlineGroups;
        }
        else
        {
          m_optionFlags &= ~OptionFlags.ShowOutlineGroups;
        }
      }
    }
    /// <summary>
    /// Read-only. Not used.
    /// </summary>
    public int Reserved
    {
      get
      {
        return m_iReserved;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public override int MaximumMemorySize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Row or column index.
    /// </summary>
    ushort  IOutline.Index
    {
      get
      {
        return RowNumber;
      }
      set
      {
        RowNumber = value;
      }
    }
    /// <summary>
    /// Gets or sets the worksheet.
    /// </summary>
    /// <value>The worksheet.</value>
    internal WorksheetImpl Worksheet
    {
        get
        {
            return m_sheet;
        }
        set
        {
            m_sheet = value;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  RowRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    public  RowRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    public  RowRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
//      uint uiValue = provider.ReadUInt32( iOffset );
//      iOffset += 4;
//
//      const uint DEF_TWOBYTE_MASK = 0xFFFF;
//      const int BitsInShort = 16;
//      m_usRowNumber = ( ushort )( uiValue & DEF_TWOBYTE_MASK );//provider.ReadUInt16( iOffset );
//      m_usFirstCol = ( ushort )( uiValue >> BitsInShort );//provider.ReadUInt16( iOffset );
//
//      uiValue = provider.ReadUInt32( iOffset );
//      iOffset += 4;
//
//      m_usLastCol = ( ushort )( uiValue & DEF_TWOBYTE_MASK );//provider.ReadUInt16( iOffset );
//      m_usHeigth = ( ushort )( uiValue >> BitsInShort );//provider.ReadUInt16( iOffset );
//
//      uiValue = provider.ReadUInt32( iOffset );
//      iOffset += 4;
//
//      m_usOptimize = ( ushort )( uiValue & DEF_TWOBYTE_MASK );//provider.ReadUInt16( iOffset );
//      m_usReserved = ( ushort )( uiValue >> BitsInShort );//provider.ReadUInt16( iOffset );
      m_usRowNumber = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usFirstCol = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usLastCol = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usHeigth = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_iReserved = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_optionFlags = ( OptionFlags )provider.ReadInt32( iOffset );
      //iOffset += 4;
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      IsFormatted = ExtendedFormatIndex != Syncfusion.XlsIO.Implementation.RangeImpl.DEF_NORMAL_STYLE_INDEX;

      provider.WriteUInt16( iOffset, m_usRowNumber );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usFirstCol );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usLastCol );
      iOffset += 2;

      if (Worksheet != null)
      {
          ushort height = IsBadFontHeight ? m_usHeigth : (ushort)(Worksheet.PageSetup as PageSetupImpl).DefaultRowHeight;
          provider.WriteUInt16(iOffset, height);
      }
      else
          provider.WriteUInt16(iOffset, m_usHeigth);
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iReserved );
      iOffset += 4;

      provider.WriteInt32( iOffset, ( int )m_optionFlags );
      //iOffset += 4;
    }
    #endregion
  }
}
