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
using System.IO;
using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// One of the most complex records. There are two types:
  /// Style and Cell. It should be noted that fields in the extended format record are
  /// somewhat arbitrary.  Almost all of the fields are bit-level, but
  /// we name them as best as possible by functional group.  In some
  /// places, this is more conducive than others.
  /// </summary>
  [ Biff( TBIFFRecord.ExtendedFormat ) ]
  [ CLSCompliant( false ) ]
  public class ExtendedFormatRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Type of extended format.
    /// </summary>
    public enum TXFType : int
    {
      /// <summary>
      /// Represents the XF_CELL extended format type.
      /// </summary>
      XF_CELL  = 0,
      /// <summary>
      /// Represents the XF_STYLE extended format type.
      /// </summary>
      XF_STYLE = 1
    }

    /// <summary>
    /// Indent bit mask:
    /// </summary>
    private const ushort DEF_INDENT_MASK = 0x0F;
    /// <summary>
    /// Read order bit mask:
    /// </summary>
    private const ushort DEF_READ_ORDER_MASK = 0x00C0;
    /// <summary>
    /// Start bit for read order value.
    /// </summary>
    private const ushort DEF_READ_ORDER_START_BIT = 6;
    /// <summary>
    /// Parent index bit mask:
    /// </summary>
    private const ushort DEF_PARENT_INDEX_MASK = 0xFFF0;
    /// <summary>
    /// Rotation bit mask:
    /// </summary>
    private const ushort DEF_ROTATION_MASK = 0xFF00;
    /// <summary>
    /// Top border palette bit mask:
    /// </summary>
    private const uint   DEF_TOP_BORDER_PALLETE_MASK = 0x0000007F;
    /// <summary>
    /// Bottom border palette bit mask:
    /// </summary>
    private const uint   DEF_BOTTOM_BORDER_PALLETE_MASK = 0x00003F80;
    /// <summary>
    /// Diagonal bit mask:
    /// </summary>
    private const uint   DEF_DIAGONAL_MASK = 0x001FC000;
    /// <summary>
    /// Diagonal line bit mask:
    /// </summary>
    private const uint   DEF_DIAGONAL_LINE_MASK = 0x01E00000;
    /// <summary>
    /// Fill pattern bit mask:
    /// </summary>
    private const uint   DEF_FILL_PATTERN_MASK = 0xFC000000;
    /// <summary>
    /// Left border bit mask:
    /// </summary>
    private const ushort DEF_BORDER_LEFT_MASK  = 0x000F;
    /// <summary>
    /// Right border bit mask:
    /// </summary>
    private const ushort DEF_BORDER_RIGTH_MASK = 0x00F0;
    /// <summary>
    /// Top border bit mask:
    /// </summary>
    private const ushort DEF_BORDER_TOP_MASK   = 0x0F00;
    /// <summary>
    /// Bottom border bit mask:
    /// </summary>
    private const ushort DEF_BORDER_BOTTOM_MASK= 0xF000;
    /// <summary>
    /// Horizontal alignment bit mask:
    /// </summary>
    private const ushort DEF_HOR_ALIGNMENT_MASK = 0x0007;
    /// <summary>
    /// Vertical alignment bit mask:
    /// </summary>
    private const ushort DEF_VER_ALIGNMENT_MASK = 0x0070;
    /// <summary>
    /// Background bit mask:
    /// </summary>
    private const ushort DEF_BACKGROUND_MASK = 0x007F;
    /// <summary>
    /// Foreground border bit mask:
    /// </summary>
    private const ushort DEF_FOREGROUND_MASK = 0x3F80;
    /// <summary>
    /// Left border palette bit mask:
    /// </summary>
    private const ushort DEF_LEFT_BORDER_PALLETE_MASK = 0x007F;
    /// <summary>
    /// Right border palette bit mask:
    /// </summary>
    private const ushort DEF_RIGHT_BORDER_PALLETE_MASK = 0x3F80;
    /// <summary>
    /// Start bit for the right border bit mask:
    /// </summary>
    private const int    DEF_RIGHT_BORDER_START_MASK = 0x07;
    /// <summary>
    /// 
    /// </summary>
    private const int    DEF_RECORD_SIZE = 20;
    /// <summary>
    /// Mask for FillForeground property.
    /// </summary>
    private const int    DEF_FILL_FOREGROUND_MASK = 0x3F80;
    /// <summary>
    /// Default color index.
    /// </summary>
    public const int DEF_DEFAULT_COLOR_INDEX = 65;
    /// <summary>
    /// Default pattern color index.
    /// </summary>
    public const int DEF_DEFAULT_PATTERN_COLOR_INDEX = 64;
    /// <summary>
    /// Maximum possible index in the extended format, means that there is no parent.
    /// </summary>
    private const int DEF_XF_MAX_INDEX = 4095;//0x0FFF

    #endregion

    #region Class members
    /// <summary>
    /// Index to font record.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFontIndex = 0;
    /// <summary>
    /// Index to FORMAT record.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usFormatIndex = 0;
    /// <summary>
    /// Cell options.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usCellOptions = 0;

    #region CellOptions bit fields
    /// <summary>
    /// True if cell is locked.
    /// </summary>
    [ BiffRecordPos( 4, 0, TFieldType.Bit ) ]
    private bool m_bLocked = true;
    /// <summary>
    /// True if formula is hidden.
    /// </summary>
    [ BiffRecordPos( 4, 1, TFieldType.Bit ) ]
    private bool m_bHidden = false;
    /// <summary>
    /// Type of extended format record, False = Cell XF; True = Style XF.
    /// </summary>
    [ BiffRecordPos( 4, 2, TFieldType.Bit ) ]
    private bool m_xfType = false;
    /// <summary>
    ///
    /// </summary>
    [ BiffRecordPos( 4, 3, TFieldType.Bit ) ]
    private bool m_b123Prefix = false;
    #endregion

    /// <summary>
    /// Alignment options of the extended format.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usAlignmentOptions = 32;

    #region AlignmentOptions bit fields
    /// <summary>
    /// True indicates that text is wrapped at right border.
    /// </summary>
    [ BiffRecordPos( 6, 3, TFieldType.Bit ) ]
    private bool m_bWrapText = false;
    /// <summary>
    /// For far east languages. Supported only for format, always 0 for US.
    /// </summary>
    [ BiffRecordPos( 6, 7, TFieldType.Bit ) ]
    private bool m_bJustifyLast = false;
    #endregion

    /// <summary>
    /// Indent options of the extended format.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usIndentOptions = 0; // or zero

    #region IndentOptions bit fields
    /// <summary>
    /// True indicates shrinking content to fit into cell.
    /// </summary>
    [ BiffRecordPos( 8, 4, TFieldType.Bit ) ]
    private bool m_bShrinkToFit = false;
    /// <summary>
    /// True indicates that XF contains merged cells.
    /// </summary>
    [ BiffRecordPos( 8, 5, TFieldType.Bit ) ]
    private bool m_bMergeCells = false;
    /// <summary>
    /// Flag for number format, if False, then an attribute of the parent style is used.
    /// </summary>
    [ BiffRecordPos( 9, 2, TFieldType.Bit ) ]
    private bool m_bIndentNotParentFormat = false;
    /// <summary>
    /// Flag for font, if False, then an attribute of the parent style is used.
    /// </summary>
    [ BiffRecordPos( 9, 3, TFieldType.Bit ) ]
    private bool m_bIndentNotParentFont = false;
    /// <summary>
    /// Flag for horizontal and vertical alignment, text wrap, indentation,
    /// orientation, rotation, and text direction. If False, then an attribute
    /// of the parent style is used.
    /// </summary>
    [ BiffRecordPos( 9, 4, TFieldType.Bit ) ]
    private bool m_bIndentNotParentAlignment = false;
    /// <summary>
    /// Flag for border lines.
    /// If False, then an attribute of the parent style is used.
    /// </summary>
    [ BiffRecordPos( 9, 5, TFieldType.Bit ) ]
    private bool m_bIndentNotParentBorder = false;
    /// <summary>
    /// Flag for background area style.
    /// If False, then an attribute of the parent style is used.
    /// </summary>
    [ BiffRecordPos( 9, 6, TFieldType.Bit ) ]
    private bool m_bIndentNotParentPattern = false;
    /// <summary>
    /// Flag for cell protection (cell locked and formula hidden).
    /// If False, then an attribute of the parent style is used.
    /// </summary>
    [ BiffRecordPos( 9, 7, TFieldType.Bit ) ]
    private bool m_bIndentNotParentCellOptions = false;
    #endregion
    /// <summary>
    /// Indent value.
    /// </summary>
    private byte m_btIndent = 0;

    /// <summary>
    /// Border options:
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usBorderOptions = 0;
    /// <summary>
    /// Palette options:
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usPaletteOptions = 0;

    #region Diagonal Line bits
    /// <summary>
    /// True if diagonal line runs from top left to right bottom.
    /// </summary>
    [ BiffRecordPos( 13, 6, TFieldType.Bit) ]
    private bool m_bDiagnalFromTopLeft;
    /// <summary>
    /// True if diagonal line runs from bottom left to right top.
    /// </summary>
    [ BiffRecordPos( 13, 7, TFieldType.Bit) ]
    private bool m_bDiagnalFromBottomLeft;
    #endregion

    /// <summary>
    /// Additional palette options:
    /// </summary>
    [ BiffRecordPos( 14, 4 ) ]
    private uint   m_uiAddPaletteOptions = 0;
    /// <summary>
    /// Fill options:
    /// </summary>
    [BiffRecordPos( 18, 2 )]
    private ushort m_usFillPaletteOptions = 0x2041;//8384;
    /// <summary>
    /// Indicates whether hash is valid.
    /// </summary>
    private bool m_bHashValid = false;
    /// <summary>
    /// Current hash value.
    /// </summary>
    private int m_iHash = 0;
    /// <summary>
    /// Index to the parent extended format.
    /// </summary>
    private ushort m_usParentXFIndex;
    /// <summary>
    /// Extended format fill pattern type.
    /// </summary>
    private ushort m_usFillPattern;
    private WorkbookImpl m_book;
    /// <summary>
    /// Index of Fill.
    /// </summary>
    private ushort m_fillIndex;
    /// <summary>
    /// Index of Border.
    /// </summary>
    private ushort m_borderIndex;
    #endregion

    #region Class properties
    /// <summary>
    /// Cell options bits in one location. Read-only.
    /// </summary>
    public int  CellOptions
    {
      get
      {
        int retVal = 0;
        retVal |= m_bLocked ? 1 : 0; retVal <<= 1;
        retVal |= m_bHidden ? 1 : 0; retVal <<= 1;
        retVal |= m_bMergeCells ? 1 : 0; retVal <<= 1;
        retVal |= m_bShrinkToFit ? 1 : 0; retVal <<= 1;

        return retVal;
      }
    }
    /// <summary>
    /// Border options bits in one location. Read-only.
    /// </summary>
    public int  BorderOptions
    {
      get
      {
        int retVal = 0;
        retVal |= m_bDiagnalFromTopLeft ? 1 : 0; retVal <<= 1;
        retVal |= m_bDiagnalFromBottomLeft ? 1 : 0; retVal <<= 15;
        retVal |= ( int )m_usBorderOptions;

        return retVal;
      }
    }
    /// <summary>
    /// Alignment options bits in one location. Read-only.
    /// </summary>
    public int  AlignmentOptions
    {
      get
      {
        return (int)m_usAlignmentOptions;
      }
    }
    /// <summary>
    /// Index to font record.
    /// </summary>
    public ushort  FontIndex
    {
      get
      {
        return m_usFontIndex;
      }
      set
      {
        // NOTE: The font with index 4 is omitted in all BIFF versions. This means
        // the first four fonts have zero-based indexes and the fifth font and all
        // following fonts are referenced with one-based indexes.
        if( value == Syncfusion.XlsIO.Implementation.FontImpl.DEF_BAD_INDEX )
          throw new ArgumentException( "FontIndex must be less than or higher than 4 for ExtendedFormatRecords." );

        m_bHashValid = false;
        m_usFontIndex = value;
      }
    }
    /// <summary>
    /// Represent the fill index
    /// </summary>
    internal ushort FillIndex
    {
        get
        {
            return m_fillIndex;
        }
        set
        {
            m_fillIndex = value;
        }
    }
    /// <summary>
    /// Represent the Border Index
    /// </summary>
    internal ushort BorderIndex
    {
        get
        {
            return m_borderIndex;
        }
        set
        {
            m_borderIndex = value;
        }
    }
    /// <summary>
    /// Index to FORMAT record:
    /// </summary>
    public ushort  FormatIndex
    {
      get
      {
        return m_usFormatIndex;
      }
      set
      {
        m_bHashValid = false;
        m_usFormatIndex = value;
      }
    }
    /// <summary>
    /// True if cell is locked.
    /// </summary>
    public bool    IsLocked
    {
      get
      {
        return m_bLocked;
      }
      set
      {
        m_bHashValid = false;
        m_bLocked = value;
      }
    }
    /// <summary>
    /// True if formula is hidden.
    /// </summary>
    public bool    IsHidden
    {
      get
      {
        return m_bHidden;
      }
      set
      {
        m_bHashValid = false;
        m_bHidden = value;
      }
    }
    /// <summary>
    /// Type of extended format record.
    /// </summary>
    public TXFType XFType
    {
      get
      {
        return ( m_xfType ) ? TXFType.XF_CELL : TXFType.XF_STYLE;
      }
      set
      {
        m_bHashValid = false;
        m_xfType = ( value == TXFType.XF_CELL ) ? true : false;
      }
    }
    /// <summary>
    ///
    /// </summary>
    public bool    _123Prefix
    {
      get
      {
        return m_b123Prefix;
      }
      set
      {
        m_bHashValid = false;
        m_b123Prefix = value;
      }
    }
    /// <summary>
    /// Gets /sets index to parent style XF (always FFFH in style XFs in Excel 97).
    /// </summary>
    public ushort  ParentIndex
    {
      get
      {
        //return (ushort)( GetUInt16BitsByMask( m_usCellOptions, 0xFFF0 ) >> 4 );
        return m_usParentXFIndex;
      }
      set
      {
        m_usParentXFIndex = value;
//        if( value > 0x0FFF )
//          throw new ArgumentOutOfRangeException( "ParentIndex", value, "Can't be larger than 0x0FFF." );
//
//        m_bHashValid = false;
//        SetUInt16BitsByMask( ref m_usCellOptions, DEF_PARENT_INDEX_MASK, ( ushort ) (value << 4 ) );
      }
    }
    /// <summary>
    /// True means that text is wrapped at right border.
    /// </summary>
    public bool    WrapText
    {
      get
      {
        return m_bWrapText;
      }
      set
      {
        if( m_bWrapText != value )
        {
          m_bHashValid = false;
          m_bWrapText = value;
          SetBitInVar( ref m_usAlignmentOptions, m_bWrapText, 3 );
        }
      }
    }
    /// <summary>
    /// For far east languages supported only for format, always use False for US.
    /// </summary>
    public bool    JustifyLast
    {
      get
      {
        return m_bJustifyLast;
      }
      set
      {
        if( m_bJustifyLast != value )
        {
          m_bHashValid = false;
          m_bJustifyLast = value;
          SetBitInVar( ref m_usAlignmentOptions, m_bJustifyLast, 7 );
        }
      }
    }
    /// <summary>
    /// Gets or sets indent level.
    /// </summary>
    public byte  Indent
    {
      get
      {
        return m_btIndent;//GetUInt16BitsByMask( m_usIndentOptions, DEF_INDENT_MASK );
      }
      set
      {
        //if( value > 0x0F )
        //  throw new ArgumentOutOfRangeException("Indent");

        m_bHashValid = false;
        m_btIndent = value;
        //SetUInt16BitsByMask( ref m_usIndentOptions, DEF_INDENT_MASK, value );
      }
    }
    /// <summary>
    /// True means to shrink content to fit into cell.
    /// </summary>
    public bool    ShrinkToFit
    {
      get
      {
        return m_bShrinkToFit;
      }
      set
      {
        m_bHashValid = false;
        m_bShrinkToFit = value;
      }
    }
    /// <summary>
    /// True if extended format contains merged cells.
    /// </summary>
    public bool    MergeCells
    {
      get
      {
        return m_bMergeCells;
      }
      set
      {
        m_bHashValid = false;
        m_bMergeCells = value;
      }
    }
    /// <summary>
    /// Text direction, the reading order for far east versions.
    /// </summary>
    public ushort  ReadingOrder
    {
      get
      {
        return ( ushort ) ( GetUInt16BitsByMask( m_usIndentOptions, DEF_READ_ORDER_MASK )
          >> DEF_READ_ORDER_START_BIT );
      }
      set
      {
        if( value > 0x03 )
          throw new ArgumentOutOfRangeException( "Reading Order" );

        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usIndentOptions, DEF_READ_ORDER_MASK,
          ( ushort )( value << DEF_READ_ORDER_START_BIT ) );
      }
    }
    /// <summary>
    /// Text rotation angle:
    /// 0- Not rotated
    /// 1-90- 1 to 90 degrees counterclockwise
    /// 91-180- 1 to 90 degrees clockwise
    /// 255- Letters are stacked top-to-bottom, but not rotated.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0xFF.
    /// </exception>
    public ushort  Rotation
    {
      get
      {
        return ( ushort )( GetUInt16BitsByMask( m_usAlignmentOptions, DEF_ROTATION_MASK ) >> 8 );
      }
      set
      {
        if( value > 0xFF )
          throw new ArgumentOutOfRangeException( "Rotation" );

        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usAlignmentOptions, DEF_ROTATION_MASK, ( ushort )( value << 8 ) );
      }
    }
    /// <summary>
    /// Flag for number format, if False, then an attribute of the parent style is used.
    /// </summary>
    public bool    IsNotParentFormat
    {
      get
      {
        return m_bIndentNotParentFormat;
      }
      set
      {
        m_bHashValid = false;
        m_bIndentNotParentFormat = value;
      }
    }
    /// <summary>
    /// Flag for font, if False, then an attribute of the parent style is used.
    /// </summary>
    public bool    IsNotParentFont
    {
      get
      {
        return m_bIndentNotParentFont;
      }
      set
      {
        m_bHashValid = false;
        m_bIndentNotParentFont = value;
      }
    }
    /// <summary>
    /// Flag for horizontal and vertical alignment, text wrap, indentation,
    /// orientation, rotation, and text direction. If False, then an attribute
    /// of the parent style is used.
    /// </summary>
    public bool    IsNotParentAlignment
    {
      get
      {
        return m_bIndentNotParentAlignment;
      }
      set
      {
        m_bHashValid = false;
        m_bIndentNotParentAlignment = value;
      }
    }
    /// <summary>
    /// Flag for border lines.
    /// If False, then an attribute of the parent style is used.
    /// </summary>
    public bool    IsNotParentBorder
    {
      get
      {
        return m_bIndentNotParentBorder;
      }
      set
      {
        m_bHashValid = false;
        m_bIndentNotParentBorder = value;
      }
    }
    /// <summary>
    /// Flag for background area style.
    /// If False, then an attribute of the parent style is used.
    /// </summary>
    public bool    IsNotParentPattern
    {
      get
      {
        return m_bIndentNotParentPattern;
      }
      set
      {
        m_bHashValid = false;
        m_bIndentNotParentPattern = value;
      }
    }
    /// <summary>
    /// Flag for cell protection (cell locked and formula hidden).
    /// If False, then attributes of the parent style is used.
    /// </summary>
    public bool    IsNotParentCellOptions
    {
      get
      {
        return m_bIndentNotParentCellOptions;
      }
      set
      {
        m_bHashValid = false;
        m_bIndentNotParentCellOptions = value;
      }
    }

    /// <summary>
    /// Color index for top line color.
    /// This property changes bits of the m_uiAddPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort  TopBorderPaletteIndex
    {
      get
      {
        return ( ushort )( m_uiAddPaletteOptions & DEF_TOP_BORDER_PALLETE_MASK );
      }
      set
      {
        if( value > 0x7F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        m_uiAddPaletteOptions &= ~DEF_TOP_BORDER_PALLETE_MASK;
        m_uiAddPaletteOptions += value;
      }
    }
    /// <summary>
    /// Color index for bottom line color.
    /// This property changes bits of the m_uiAddPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort  BottomBorderPaletteIndex
    {
      get
      {
        return ( ushort ) ( ( m_uiAddPaletteOptions & DEF_BOTTOM_BORDER_PALLETE_MASK ) >> 7 );
      }
      set
      {
        if( value > 0x7F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        m_uiAddPaletteOptions &= ~DEF_BOTTOM_BORDER_PALLETE_MASK;
        m_uiAddPaletteOptions += ( uint )( value << 7 );
      }
    }
    /// <summary>
    /// Color index for left line color
    /// This property changes bits of the m_usPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort  LeftBorderPaletteIndex
    {
      get
      {
        return ( ushort )( m_usPaletteOptions & DEF_LEFT_BORDER_PALLETE_MASK );
      }
      set
      {
        if( value > 0x7F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usPaletteOptions, DEF_LEFT_BORDER_PALLETE_MASK, value );
      }
    }
    /// <summary>
    /// Color index for right line color.
    /// This property changes bits of the m_usPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort  RightBorderPaletteIndex
    {
      get
      {
        return ( ushort )( GetUInt16BitsByMask( m_usPaletteOptions, DEF_RIGHT_BORDER_PALLETE_MASK )
          >> DEF_RIGHT_BORDER_START_MASK );
      }
      set
      {
        if( value > 0x7F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usPaletteOptions, DEF_RIGHT_BORDER_PALLETE_MASK,
          (ushort) ( value << DEF_RIGHT_BORDER_START_MASK ) );
      }
    }
    /// <summary>
    /// Color index for diagonal line color.
    /// This property changes bits of the m_uiAddPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort  DiagonalLineColor
    {
      get
      {
        return ( ushort ) ( ( m_uiAddPaletteOptions & DEF_DIAGONAL_MASK ) >> 14 );
      }
      set
      {
        if( value > 0x7F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        m_uiAddPaletteOptions &= ~DEF_DIAGONAL_MASK;
        m_uiAddPaletteOptions |= ( uint ) ( value << 14 );
      }
    }
    /// <summary>
    /// Diagonal line style.
    /// This property changes bits of the m_uiAddPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x0F.
    /// </exception>
    public ushort  DiagonalLineStyle
    {
      get
      {
        return ( ushort ) ( ( m_uiAddPaletteOptions &  DEF_DIAGONAL_LINE_MASK ) >> 21 );
      }
      set
      {
        if( value > 0x0F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        m_uiAddPaletteOptions &= ~DEF_DIAGONAL_LINE_MASK;
        m_uiAddPaletteOptions |= ( uint ) ( value << 21 );
      }
    }
    /// <summary>
    /// Diagonal line is drawing from top left to bottom right corner of cell.
    /// </summary>
    public bool    DiagonalFromTopLeft
    {
      get
      {
        return m_bDiagnalFromTopLeft;
      }
      set
      {
        if( m_bDiagnalFromTopLeft != value )
        {
          m_bHashValid = false;
          m_bDiagnalFromTopLeft = value;
          SetBitInVar( ref m_usPaletteOptions, m_bDiagnalFromTopLeft, 6 + 8 );
        }
      }
    }
    /// <summary>
    /// Diagonal line is drawing from bottom left to top right corner of cell.
    /// </summary>
    public bool    DiagonalFromBottomLeft
    {
      get
      {
        return m_bDiagnalFromBottomLeft;
      }
      set
      {
        if( m_bDiagnalFromBottomLeft != value )
        {
          m_bHashValid = false;
          m_bDiagnalFromBottomLeft = value;
          SetBitInVar( ref m_usPaletteOptions, m_bDiagnalFromBottomLeft, 7 + 8 );
        }
      }
    }
    /// <summary>
    /// Fill pattern:
    /// This property changes bits of the m_uiAddPaletteOptions class member.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x3F.
    /// </exception>
    public ushort  AdtlFillPattern
    {
      get
      {
        return m_usFillPattern;
        //return ( ushort ) ( ( m_uiAddPaletteOptions & DEF_FILL_PATTERN_MASK ) >> 26 );
      }
      set
      {
        m_usFillPattern = value;
        //if( value > 0x3F )
        //  throw new ArgumentOutOfRangeException();

        //m_bHashValid = false;
        //m_uiAddPaletteOptions &= ~DEF_FILL_PATTERN_MASK;
        //m_uiAddPaletteOptions += ( uint )( value << 26 );
      }
    }

    /// <summary>
    /// Left line style:
    /// This property changes bits of the m_usBorderOptions class member.
    /// </summary>
    public ExcelLineStyle BorderLeft
    {
      get
      {
        return ( ExcelLineStyle ) GetUInt16BitsByMask( m_usBorderOptions, DEF_BORDER_LEFT_MASK );
      }
      set
      {
        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usBorderOptions, DEF_BORDER_LEFT_MASK, ( ushort )value );

        if ((value != ExcelLineStyle.None && LeftBorderPaletteIndex == 0))
            LeftBorderPaletteIndex = DEF_DEFAULT_PATTERN_COLOR_INDEX;
        else if (value == ExcelLineStyle.None)
            LeftBorderPaletteIndex = 0;
      }
    }
    /// <summary>
    /// Right line style:
    /// This property changes bits of the m_usBorderOptions class member.
    /// </summary>
    public ExcelLineStyle BorderRight
    {
      get
      {
        return ( ExcelLineStyle )
          ( GetUInt16BitsByMask( m_usBorderOptions, DEF_BORDER_RIGTH_MASK ) >> 4 );
      }
      set
      {
        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usBorderOptions, DEF_BORDER_RIGTH_MASK,
          (ushort) ( (ushort) value << 4 ) );

        if ((value != ExcelLineStyle.None && RightBorderPaletteIndex == 0))
            RightBorderPaletteIndex = DEF_DEFAULT_PATTERN_COLOR_INDEX;
        else if (value == ExcelLineStyle.None)
            RightBorderPaletteIndex = 0;
      }
    }
    /// <summary>
    /// Top line style:
    /// This property changes bits of the m_usBorderOptions class member.
    /// </summary>
    public ExcelLineStyle BorderTop
    {
      get
      {
        return ( ExcelLineStyle )
          ( GetUInt16BitsByMask( m_usBorderOptions, DEF_BORDER_TOP_MASK ) >> 8 );
      }
      set
      {
        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usBorderOptions, DEF_BORDER_TOP_MASK,
          (ushort) ( (ushort) value << 8 ) );

        if ((value != ExcelLineStyle.None && TopBorderPaletteIndex == 0))
            TopBorderPaletteIndex = DEF_DEFAULT_PATTERN_COLOR_INDEX;
        else if (value == ExcelLineStyle.None)
            TopBorderPaletteIndex = 0;
      }
    }
    /// <summary>
    /// Bottom line style:
    /// This property changes bits of the m_usBorderOptions class member.
    /// </summary>
    public ExcelLineStyle BorderBottom
    {
      get
      {
        return ( ExcelLineStyle )
          ( GetUInt16BitsByMask( m_usBorderOptions, DEF_BORDER_BOTTOM_MASK ) >> 12 );
      }
      set
      {
        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usBorderOptions, DEF_BORDER_BOTTOM_MASK,
          ( ushort ) ( ( ushort )value << 12 ) );

        if ((value != ExcelLineStyle.None && BottomBorderPaletteIndex == 0))
            BottomBorderPaletteIndex = DEF_DEFAULT_PATTERN_COLOR_INDEX;
        else if (value == ExcelLineStyle.None)
            BottomBorderPaletteIndex = 0;
      }
    }

    /// <summary>
    /// Horizontal alignment.
    /// </summary>
    public ExcelHAlign    HAlignmentType
    {
      get
      {
        return ( ExcelHAlign )GetUInt16BitsByMask( m_usAlignmentOptions,
          DEF_HOR_ALIGNMENT_MASK );
      }
      set
      {
        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usAlignmentOptions, DEF_HOR_ALIGNMENT_MASK,
          ( ushort ) value );
      }
    }
    /// <summary>
    /// Vertical alignment.
    /// </summary>
    public ExcelVAlign    VAlignmentType
    {
      get
      {
        return ( ExcelVAlign )
          ( GetUInt16BitsByMask( m_usAlignmentOptions, DEF_VER_ALIGNMENT_MASK ) >> 4 );
      }
      set
      {
        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usAlignmentOptions, DEF_VER_ALIGNMENT_MASK,
          (ushort)( (ushort)value << 4 ) );
      }
    }

    /// <summary>
    /// Color index for pattern color
    /// This property changes bits of m_usFillPaletteOptions.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort FillBackground
    {
      get
      {
        return GetUInt16BitsByMask( m_usFillPaletteOptions, DEF_BACKGROUND_MASK );
      }
      set
      {
        if( value > 0x07F )
          throw new ArgumentOutOfRangeException();

        m_bHashValid = false;
        SetUInt16BitsByMask( ref m_usFillPaletteOptions, DEF_BACKGROUND_MASK, value );
      }
    }
    /// <summary>
    /// Color index for pattern background.
    /// This property changes bits of m_usFillPaletteOptions.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0x7F.
    /// </exception>
    public ushort FillForeground
    {
      get
      {
        return ( ushort ) ( ( m_usFillPaletteOptions & DEF_FILL_FOREGROUND_MASK )>> 7 );
      }
      set
      {
        if( value > 0x07F )
          throw new ArgumentOutOfRangeException( "FillForeground", "Argument is too large" );

        m_bHashValid = false;
        unchecked{ m_usFillPaletteOptions &= ( ushort )( ~DEF_FOREGROUND_MASK ); }
        m_usFillPaletteOptions |= ( ushort )( value << 7 );
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
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  ExtendedFormatRecord()
      : base()
    {
//      this.m_iCode = ( int ) TBIFFRecord.ExtendedFormat;
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  ExtendedFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for the record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  ExtendedFormatRecord( int iReserve )
      : base( iReserve )
    {
      this.m_iCode = ( int ) TBIFFRecord.ExtendedFormat;
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
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usFontIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usFormatIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usCellOptions = provider.ReadUInt16( iOffset );

      m_xfType = provider.ReadBit( iOffset, 2 );
      m_b123Prefix = provider.ReadBit( iOffset, 3 );
      m_bLocked = provider.ReadBit( iOffset, 0 );
      m_bHidden = provider.ReadBit( iOffset, 1 );
      iOffset += 2;

      m_usAlignmentOptions = provider.ReadUInt16( iOffset);

      m_bJustifyLast = provider.ReadBit( iOffset, 7 );
      m_bWrapText = provider.ReadBit( iOffset, 3 );
      iOffset += 2;

      m_usIndentOptions = provider.ReadUInt16( iOffset  );
      m_bMergeCells = provider.ReadBit( iOffset, 5 );
      m_bShrinkToFit = provider.ReadBit( iOffset, 4 );
      m_btIndent = ( byte )GetUInt16BitsByMask( m_usIndentOptions, DEF_INDENT_MASK );
      m_usIndentOptions = ( ushort )( m_usIndentOptions & ~DEF_INDENT_MASK );
      iOffset++;

      m_bIndentNotParentBorder = provider.ReadBit( iOffset, 5 );
      m_bIndentNotParentPattern = provider.ReadBit( iOffset, 6 );
      m_bIndentNotParentCellOptions = provider.ReadBit( iOffset, 7 );
      m_bIndentNotParentFormat = provider.ReadBit( iOffset, 2 );
      m_bIndentNotParentFont = provider.ReadBit( iOffset, 3 );
      m_bIndentNotParentAlignment = provider.ReadBit( iOffset, 4 );
      iOffset++;

      m_usBorderOptions = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usPaletteOptions = provider.ReadUInt16( iOffset );
      iOffset++;

      m_bDiagnalFromBottomLeft = provider.ReadBit( iOffset, 7 );
      m_bDiagnalFromTopLeft = provider.ReadBit( iOffset, 6 );
      iOffset++;

      m_uiAddPaletteOptions = provider.ReadUInt32( iOffset );
      m_usFillPattern = ( ushort )( ( m_uiAddPaletteOptions & DEF_FILL_PATTERN_MASK ) >> 26 );
      iOffset += 4;

      m_usFillPaletteOptions = provider.ReadUInt16( iOffset );
      m_iLength = DEF_RECORD_SIZE;

      SwapColors();

      m_usParentXFIndex = ( ushort )( GetUInt16BitsByMask( m_usCellOptions, 0xFFF0 ) >> 4 );
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
      //if( ParentIndex == DEF_XF_MAX_INDEX && FontIndex == 0 && FormatIndex == 0 )
      //{
      //  IsNotParentAlignment    = false;
      //  IsNotParentBorder       = false;
      //  IsNotParentCellOptions  = false;
      //  IsNotParentFont         = false;
      //  IsNotParentFormat       = false;
      //  IsNotParentPattern      = false;
      //}

      if( m_usParentXFIndex > 0x0FFF )
      {
        m_usParentXFIndex = 0x0FFF;
        //throw new ArgumentOutOfRangeException( "ParentIndex", "Can't be larger than 0x0FFF." );
      }
      
      m_bHashValid = false;

      SetUInt16BitsByMask( ref m_usCellOptions, DEF_PARENT_INDEX_MASK,
        ( ushort )( m_usParentXFIndex << 4 ) );

      SwapColors();

      provider.WriteUInt16( iOffset, m_usFontIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usFormatIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usCellOptions );
      provider.WriteBit( iOffset, m_xfType, 2 );
      provider.WriteBit( iOffset, m_b123Prefix, 3 );
      provider.WriteBit( iOffset, m_bLocked, 0 );
      provider.WriteBit( iOffset, m_bHidden, 1 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usAlignmentOptions );
      iOffset += 2;

      SetUInt16BitsByMask( ref m_usIndentOptions, DEF_INDENT_MASK, m_btIndent );
      provider.WriteUInt16( iOffset, m_usIndentOptions );
      provider.WriteBit( iOffset, m_bMergeCells, 5 );
      provider.WriteBit( iOffset, m_bShrinkToFit, 4 );
      iOffset++;

      provider.WriteBit( iOffset, m_bIndentNotParentBorder, 5 );
      provider.WriteBit( iOffset, m_bIndentNotParentPattern, 6 );
      provider.WriteBit( iOffset, m_bIndentNotParentCellOptions, 7 );
      provider.WriteBit( iOffset, m_bIndentNotParentFormat, 2 );
      provider.WriteBit( iOffset, m_bIndentNotParentFont, 3 );
      provider.WriteBit( iOffset, m_bIndentNotParentAlignment, 4 );
      iOffset++;

      provider.WriteUInt16( iOffset, m_usBorderOptions );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usPaletteOptions );
      iOffset++;

      provider.WriteBit( iOffset, m_bDiagnalFromBottomLeft, 7 );
      provider.WriteBit( iOffset, m_bDiagnalFromTopLeft, 6 );
      iOffset++;

      m_usFillPattern = ( m_usFillPattern == ( ushort )ExcelPattern.Gradient ) ? ( ushort )ExcelPattern.Solid : m_usFillPattern;

      if( m_usFillPattern > 0x3F )
        throw new ArgumentOutOfRangeException();

      m_bHashValid = false;
      m_uiAddPaletteOptions &= ~DEF_FILL_PATTERN_MASK;
      m_uiAddPaletteOptions += ( uint )( m_usFillPattern << 26 );

      provider.WriteInt32( iOffset, ( int )m_uiAddPaletteOptions );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usFillPaletteOptions );

      m_iLength = DEF_RECORD_SIZE;
      SwapColors();
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Compares with Extended format record.
    /// </summary>
    /// <param name="twin">Param to compare.</param>
    /// <returns>Returns compare results.</returns>
    public int CompareTo( ExtendedFormatRecord twin )
    {
      if( twin == null )
        throw new ArgumentNullException( "twin" );

      // NOTE: in order to increase performance we use same code multiple times (no function calls).
      const byte btOne = 1;
      const byte btZero = 0;

      byte iBool1 = m_bLocked ? btOne : btZero;
      byte iBool2 = twin.m_bLocked ? btOne : btZero;

      int result = iBool1 - iBool2;//m_bLocked.CompareTo( twin.m_bLocked );
      if( result != 0 ) return result;

      iBool1 = m_bHidden ? btOne : btZero;
      iBool2 = twin.m_bHidden ? btOne : btZero;

      result = iBool1 - iBool2;//m_bHidden.CompareTo( twin.m_bHidden );
      if( result != 0 ) return result;

      result = m_xfType.CompareTo( twin.m_xfType );
      if( result != 0 ) return result;

      result = m_usAlignmentOptions - twin.m_usAlignmentOptions;
      if( result != 0 ) return result;

      result = Indent - twin.Indent;
      if( result != 0 ) return result;

      result = ReadingOrder - twin.ReadingOrder;
      if( result != 0 ) return result;

      iBool1 = m_bShrinkToFit ? btOne : btZero;
      iBool2 = twin.m_bShrinkToFit ? btOne : btZero;

      result = iBool1 - iBool2;//m_bShrinkToFit.CompareTo( twin.m_bShrinkToFit ) ;
      if( result != 0 ) return result;

      iBool1 = m_bMergeCells ? btOne : btZero;
      iBool2 = twin.m_bMergeCells ? btOne : btZero;

      result = iBool1 - iBool2;//m_bMergeCells.CompareTo( twin.m_bMergeCells );
      if( result != 0 ) return result;

      iBool1 = m_bIndentNotParentFormat ? btOne : btZero;
      iBool2 = twin.m_bIndentNotParentFormat ? btOne : btZero;

      result = iBool1 - iBool2;
      //result = m_bIndentNotParentFormat.CompareTo( twin.m_bIndentNotParentFormat );
      if( result != 0 ) return result;

      iBool1 = m_bIndentNotParentFont ? btOne : btZero;
      iBool2 = twin.m_bIndentNotParentFont ? btOne : btZero;

      result = iBool1 - iBool2;
      //result = m_bIndentNotParentFont.CompareTo( twin.m_bIndentNotParentFont );
      if( result != 0 ) return result;

      iBool1 = m_bIndentNotParentAlignment ? btOne : btZero;
      iBool2 = twin.m_bIndentNotParentAlignment ? btOne : btZero;

      result = iBool1 - iBool2;
      //result = m_bIndentNotParentAlignment.CompareTo( twin.m_bIndentNotParentAlignment );
      if( result != 0 ) return result;

      iBool1 = m_bIndentNotParentBorder ? btOne : btZero;
      iBool2 = twin.m_bIndentNotParentBorder ? btOne : btZero;

      result = iBool1 - iBool2;
      //result = m_bIndentNotParentBorder.CompareTo( twin.m_bIndentNotParentBorder );
      if( result != 0 ) return result;

      iBool1 = m_bIndentNotParentPattern ? btOne : btZero;
      iBool2 = twin.m_bIndentNotParentPattern ? btOne : btZero;

      result = iBool1 - iBool2;
      //result = m_bIndentNotParentPattern.CompareTo( twin.m_bIndentNotParentPattern );
      if( result != 0 ) return result;

      iBool1 = m_bIndentNotParentCellOptions ? btOne : btZero;
      iBool2 = twin.m_bIndentNotParentCellOptions ? btOne : btZero;

      result = iBool1 - iBool2;
      //result = m_bIndentNotParentCellOptions.CompareTo( twin.m_bIndentNotParentCellOptions );
      if( result != 0 ) return result;

      result = m_usBorderOptions - twin.m_usBorderOptions;
      if( result != 0 ) return result;

      result = m_usPaletteOptions - twin.m_usPaletteOptions;
      if( result != 0 ) return result;

      long lResult = ( long )m_uiAddPaletteOptions - ( long )twin.m_uiAddPaletteOptions;
      if( lResult != 0 ) return lResult > 0 ? 1 : -1;

      result = m_usFillPaletteOptions - twin.m_usFillPaletteOptions;
      if( result != 0 )return result;


      result = m_usFillPattern - twin.m_usFillPattern;
      if( result != 0 ) return result;


      iBool1 = m_b123Prefix ? btOne : btZero;
      iBool2 = twin.m_b123Prefix ? btOne : btZero;

      result = iBool1 - iBool2;//m_b123Prefix.CompareTo( twin.m_b123Prefix );
      if( result != 0 ) return result;

      result = m_usFormatIndex - twin.m_usFormatIndex;
      if( result != 0 ) return result;

      result = m_usFontIndex - twin.m_usFontIndex;
      if( result != 0 ) return result;

      result = m_usParentXFIndex - twin.m_usParentXFIndex;

      return result;
    }
    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use
    /// in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      if( !m_bHashValid )
      {
        m_iHash = m_usFontIndex.GetHashCode()
          ^ m_usFormatIndex.GetHashCode()
          ^ ( m_usCellOptions & DEF_PARENT_INDEX_MASK ).GetHashCode()

          ^ m_bLocked.GetHashCode()
          ^ m_bHidden.GetHashCode()
          ^ m_xfType.GetHashCode()
          ^ m_b123Prefix.GetHashCode()

          ^ m_usAlignmentOptions.GetHashCode()

          ^ m_bWrapText.GetHashCode()
          ^ m_bJustifyLast.GetHashCode()

          ^ m_usIndentOptions.GetHashCode()

          ^ m_bShrinkToFit.GetHashCode()
          ^ m_bMergeCells.GetHashCode()
          ^ m_bIndentNotParentFormat.GetHashCode()
          ^ m_bIndentNotParentFont.GetHashCode()
          ^ m_bIndentNotParentAlignment.GetHashCode()
          ^ m_bIndentNotParentBorder.GetHashCode()
          ^ m_bIndentNotParentPattern.GetHashCode()
          ^ m_bIndentNotParentCellOptions.GetHashCode()

          ^ m_usBorderOptions.GetHashCode()
          ^ m_usPaletteOptions.GetHashCode()

          ^ m_bDiagnalFromTopLeft.GetHashCode()
          ^ m_bDiagnalFromBottomLeft.GetHashCode()

          ^ m_uiAddPaletteOptions.GetHashCode()
          ^ m_usFillPattern.GetHashCode()
          ^ m_usFillPaletteOptions.GetHashCode();

        m_bHashValid = true;
      }

      return m_iHash;
    }
    /// <summary>
    /// Swaps colors if necessary.
    /// </summary>
    private void SwapColors()
    {
      ushort usPattern = AdtlFillPattern;
      if( usPattern != ( ushort )ExcelPattern.Solid /*&& usPattern != ( ushort )ExcelPattern.None*/ )
      {
        // Here we should swap colors, because MS Excel does the same,
        // for solid pattern it stores colors swapped.
        ushort usBack = FillBackground;

        //if( usBack == DEF_DEFAULT_COLOR_INDEX )
        //{
        //  usBack = ( ushort )DEF_DEFAULT_PATTERN_COLOR_INDEX;
        //}
        //else if( usBack == DEF_DEFAULT_PATTERN_COLOR_INDEX )
        //{
        //  usBack = ( ushort )DEF_DEFAULT_COLOR_INDEX;
        //}

        ushort usFore = FillForeground;

        //if( usFore == DEF_DEFAULT_COLOR_INDEX )
        //{
        //  usFore = ( ushort )DEF_DEFAULT_PATTERN_COLOR_INDEX;
        //}
        //else if( usFore == DEF_DEFAULT_PATTERN_COLOR_INDEX )
        //{
        //  usFore = ( ushort )DEF_DEFAULT_COLOR_INDEX;
        //}

        FillBackground = usFore;
        FillForeground = usBack;
      }
    }
    /// <summary>
    /// Copies border settings from another extended format record.
    /// </summary>
    /// <param name="source">Source record to copy data from.</param>
    public void CopyBorders( ExtendedFormatRecord source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      m_usBorderOptions = source.m_usBorderOptions;

      BorderBottom = source.BorderBottom;
      BorderLeft = source.BorderLeft;
      BorderRight = source.BorderRight;
      BorderTop = source.BorderTop;

      BottomBorderPaletteIndex = source.BottomBorderPaletteIndex;
      LeftBorderPaletteIndex = source.LeftBorderPaletteIndex;
      RightBorderPaletteIndex = source.RightBorderPaletteIndex;
      TopBorderPaletteIndex = source.TopBorderPaletteIndex;

      DiagonalFromBottomLeft = source.DiagonalFromBottomLeft;
      DiagonalFromTopLeft = source.DiagonalFromTopLeft;
      DiagonalLineColor = source.DiagonalLineColor;
      DiagonalLineStyle = source.DiagonalLineStyle;
    }
    /// <summary>
    /// Copies alignment settings from another extended format record.
    /// </summary>
    /// <param name="source">Source record to copy data from.</param>
    public void CopyAlignment( ExtendedFormatRecord source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      m_usAlignmentOptions = source.m_usAlignmentOptions;
      MergeCells = source.MergeCells;
      Rotation = source.Rotation;
      ShrinkToFit = source.ShrinkToFit;
      Indent = source.Indent;
    }
    /// <summary>
    /// Copies pattern settings from another extended format record.
    /// </summary>
    /// <param name="source">Source record to copy data from.</param>
    public void CopyPatterns( ExtendedFormatRecord source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      AdtlFillPattern = source.AdtlFillPattern;
      FillBackground = source.FillBackground;
      FillForeground = source.FillForeground;
    }
    /// <summary>
    /// Copies protection / cells settings from another extended format record.
    /// </summary>
    /// <param name="source">Source record to copy data from.</param>
    public void CopyProtection( ExtendedFormatRecord source )
    {
      if( source == null )
        throw new ArgumentNullException( "m_extFormat" );

      IsLocked = source.IsLocked;
      IsHidden = source.IsHidden;
    }
    /// <summary>
    /// Copies data from the current ExtendedFormat record to the specified
    /// ExtendedFormat record.
    /// </summary>
    /// <param name="twin">ExtendedFormat record that will receive data from
    /// the current record.</param>
    public void CopyTo( ExtendedFormatRecord twin )
    {
      twin.m_b123Prefix = m_b123Prefix;
      twin.m_bDiagnalFromBottomLeft = m_bDiagnalFromBottomLeft;
      twin.m_bDiagnalFromTopLeft = m_bDiagnalFromTopLeft;
      twin.m_bHashValid = m_bHashValid;
      twin.m_bHidden = m_bHidden;
      twin.m_bIndentNotParentAlignment = m_bIndentNotParentAlignment;
      twin.m_bIndentNotParentBorder = m_bIndentNotParentBorder;
      twin.m_bIndentNotParentCellOptions = m_bIndentNotParentCellOptions;
      twin.m_bIndentNotParentFont = m_bIndentNotParentFont;
      twin.m_bIndentNotParentFormat = m_bIndentNotParentFormat;
      twin.m_bIndentNotParentPattern = m_bIndentNotParentPattern;
      twin.m_bJustifyLast = m_bJustifyLast;
      twin.m_bLocked = m_bLocked;
      twin.m_bMergeCells = m_bMergeCells;
      twin.m_bShrinkToFit = m_bShrinkToFit;
      twin.m_bWrapText = m_bWrapText;
      twin.m_iCode = m_iCode;
      twin.m_iHash = m_iHash;
      twin.m_iLength = m_iLength;
      twin.m_uiAddPaletteOptions = m_uiAddPaletteOptions;
      twin.m_usAlignmentOptions = m_usAlignmentOptions;
      twin.m_usBorderOptions = m_usBorderOptions;
      twin.m_usCellOptions = m_usCellOptions;
      twin.m_usFillPaletteOptions = m_usFillPaletteOptions;
      twin.m_usFontIndex = m_usFontIndex;
      twin.m_usFormatIndex = m_usFormatIndex;
      twin.m_usIndentOptions = m_usIndentOptions;
      twin.m_usPaletteOptions = m_usPaletteOptions;
      twin.m_xfType = m_xfType;
      twin.m_usParentXFIndex = m_usParentXFIndex;
      twin.m_usFillPattern = m_usFillPattern;
    }
    internal void SetWorkbook(WorkbookImpl book)
    {
        m_book = book;
    }
    /// <summary>
    /// Copies data from the current Biff record to the specified Biff record.
    /// </summary>
    /// <param name="raw">Biff record that will receive data from the current record.</param>
    public override void CopyTo( BiffRecordRaw raw )
    {
      if ( raw == null )
        throw new ArgumentNullException( "raw" );

      ExtendedFormatRecord formatRecord = raw as ExtendedFormatRecord;
      
      if ( formatRecord != null )
      {
        CopyTo( formatRecord );
      }
      else
      {
        throw new ArgumentException ( "raw" );
      }
    }
    #endregion
  }
}
