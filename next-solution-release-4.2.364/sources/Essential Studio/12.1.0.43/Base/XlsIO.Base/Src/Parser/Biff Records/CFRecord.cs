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
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The begin record defines the start of a block of records for a (Graphing)
  /// data object. This record is matched with a corresponding EndRecord.
  /// </summary>
  [ Biff( TBIFFRecord.CF ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class CFRecord
    : BiffRecordRaw
    , ICloneable
  {
    #region Class constants
    /// <summary>
    /// Minimum size of the record.
    /// </summary>
    private const ushort DEF_MINIMUM_RECORD_SIZE = 12;
    /// <summary>
    /// Size of the first block of reserved bytes in the font block.
    /// </summary>
    private const int    DEF_FONT_FIRST_RESERVED_SIZE = 64;
    /// <summary>
    /// Size of the second block of reserved bytes in the font block.
    /// </summary>
    private const int    DEF_FONT_SECOND_RESERVED_SIZE = 3;
    /// <summary>
    /// Size of the third block of reserved bytes in the font block.
    /// </summary>
    private const int    DEF_FONT_THIRD_RESERVED_SIZE = 16;
    /// <summary>
    /// Mask for font posture bit.
    /// </summary>
    private const uint   DEF_FONT_POSTURE_MASK = 0x2;
    /// <summary>
    /// Mask for font cancellation (strikethrough) bit.
    /// </summary>
    private const uint   DEF_FONT_CANCELLATION_MASK = 0x80;
    /// <summary>
    /// Mask for font style modification bit.
    /// </summary>
    private const uint   DEF_FONT_STYLE_MODIFIED_MASK = 0x02;
    /// <summary>
    /// Mask for font cancellation modification bit.
    /// </summary>
    private const uint   DEF_FONT_CANCELLATION_MODIFIED_MASK = 0x80;

    /// <summary>
    /// Mask for left border line style bits.
    /// </summary>
    private const ushort DEF_BORDER_LEFT_MASK   = 0x000F;
    /// <summary>
    /// Mask for right border line style bits.
    /// </summary>
    private const ushort DEF_BORDER_RIGHT_MASK  = 0x00F0;
    /// <summary>
    /// Mask for top border line style bits.
    /// </summary>
    private const ushort DEF_BORDER_TOP_MASK    = 0x0F00;
    /// <summary>
    /// Mask for bottom border line style bits.
    /// </summary>
    private const ushort DEF_BORDER_BOTTOM_MASK = 0xF000;

    /// <summary>
    /// Mask for left border color bits.
    /// </summary>
    private const uint   DEF_BORDER_LEFT_COLOR_MASK   = 0x0000007F;
    /// <summary>
    /// Mask for right border color bits.
    /// </summary>
    private const uint   DEF_BORDER_RIGHT_COLOR_MASK  = 0x00003F80;
    /// <summary>
    /// Mask for top border color bits.
    /// </summary>
    private const uint   DEF_BORDER_TOP_COLOR_MASK    = 0x007F0000;
    /// <summary>
    /// Mask for bottom border color bits.
    /// </summary>
    private const uint   DEF_BORDER_BOTTOM_COLOR_MASK = 0x3F800000;

    /// <summary>
    /// Start bit of left border color bits.
    /// </summary>
    private const int    DEF_BORDER_LEFT_COLOR_START    = 0;
    /// <summary>
    /// Start bit of right border color bits.
    /// </summary>
    private const int    DEF_BORDER_RIGHT_COLOR_START   = 7;
    /// <summary>
    /// Start bit of top border color bits.
    /// </summary>
    private const int    DEF_BORDER_TOP_COLOR_START     = 16;
    /// <summary>
    /// Start bit of bottom border color bits.
    /// </summary>
    private const int    DEF_BORDER_BOTTOM_COLOR_START  = 23;

    /// <summary>
    /// Mask for fill pattern bits.
    /// </summary>
    private const ushort DEF_PATTERN_MASK = 0xFC00;
    /// <summary>
    /// Mask for pattern color bits.
    /// </summary>
    private const ushort DEF_PATTERN_COLOR_MASK = 0x007F;
    /// <summary>
    /// Mask for pattern backcolor bits.
    /// </summary>
    private const ushort DEF_PATTERN_BACKCOLOR_MASK = 0x3F80;

    /// <summary>
    /// Start bit of fill pattern bits.
    /// </summary>
    private const int    DEF_PATTERN_START = 10;
    /// <summary>
    /// Start bit of fill pattern back color bits.
    /// </summary>
    private const int    DEF_PATTERN_BACKCOLOR_START = 7;

    /// <summary>
    /// Size of the font block.
    /// </summary>
    private const int DEF_FONT_BLOCK_SIZE = DEF_FONT_FIRST_RESERVED_SIZE + 13
      + DEF_FONT_SECOND_RESERVED_SIZE + 20 + DEF_FONT_THIRD_RESERVED_SIZE + 2;
    /// <summary>
    /// Size of the border block.
    /// </summary>
    private const int DEF_BORDER_BLOCK_SIZE = 8;
    /// <summary>
    /// Size of the pattern block.
    /// </summary>
    private const int DEF_PATTERN_BLOCK_SIZE = 4;
    /// <summary>
    /// Size of the number format block.
    /// </summary>
    private const int DEF_NUMBER_FORMAT_BLOCK_SIZE = 2;
    /// <summary>
    /// Default color index.
    /// </summary>
    public const uint DefaultColorIndex = 0xFFFFFFFF;
    #endregion

    #region Class members
    /// <summary>
    /// Type of the conditional formatting: 
    /// 
    /// 01H = Compare with current cell value 
    /// (the comparison specified below is used)
    /// 
    /// 02H = Evaluate a formula (condition is met 
    /// if formula evaluates to a value not equal to 0)
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_formatingType = 1;

    /// <summary>
    /// Comparison operator: 
    /// 00H = No comparison (only valid for formula type, see above)
    /// 01H = Between
    /// 02H = Not between
    /// 03H = Equal
    /// 04H = Not equal
    /// 05H = Greater than
    /// 06H = Less than
    /// 07H = Greater or equal
    /// 08H = Less or equal
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_compareOperator = 1;

    /// <summary>
    /// Size of the formula data for first value or formula.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usFirstFormulaSize;

    /// <summary>
    /// Size of the formula data for second value or formula
    /// (sz2, used for second part of �Between� and �Not between�
    /// comparison, this field is 0 for other comparisons).
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usSecondFormulaSize;

    /// <summary>
    /// Option flags
    /// </summary>
    [ BiffRecordPos( 6, 4 ) ]
    private uint m_uiOptions;

    /// <summary>
    /// Not used
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usReserved;

    /// <summary>
    /// False if left border style and color are modified.
    /// </summary>
    [ BiffRecordPos( 7, 2, TFieldType.Bit ) ]
    private bool m_bLeftBorder = true;

    /// <summary>
    /// False if right border style and color are modified.
    /// </summary>
    [ BiffRecordPos( 7, 3, TFieldType.Bit ) ]
    private bool m_bRightBorder = true;

    /// <summary>
    /// False if top border style and color are modified.
    /// </summary>
    [ BiffRecordPos( 7, 4, TFieldType.Bit ) ]
    private bool m_bTopBorder = true;

    /// <summary>
    /// False if bottom border style and color are modified.
    /// </summary>
    [ BiffRecordPos( 7, 5, TFieldType.Bit ) ]
    private bool m_bBottomBorder = true;

    /// <summary>
    /// False if pattern style is modified.
    /// </summary>
    [ BiffRecordPos( 8, 0, TFieldType.Bit ) ]
    private bool m_bPatternStyle = true;

    /// <summary>
    /// False if pattern color is modified.
    /// </summary>
    [ BiffRecordPos( 8, 1, TFieldType.Bit ) ]
    private bool m_bPatternColor = true;

    /// <summary>
    /// False if pattern background color is modified.
    /// </summary>
    [ BiffRecordPos( 8, 2, TFieldType.Bit ) ]
    private bool m_bPatternBackColor = true;

    /// <summary>
    /// False if the number format is modified.
    /// </summary>
    [BiffRecordPos(8, 3, TFieldType.Bit)]
    private bool m_bNumberFormatModified = true;

    /// <summary>
    /// True if record contains number format.
    /// </summary>
    [BiffRecordPos(9, 1, TFieldType.Bit)]
    private bool m_bNumberFormatPresent = false;

    /// <summary>
    /// True if record contains font formatting block.
    /// </summary>
    [ BiffRecordPos( 9, 2, TFieldType.Bit ) ]
    private bool m_bFontFormat = false;

    /// <summary>
    /// True if record contains border formatting block.
    /// </summary>
    [ BiffRecordPos( 9, 4, TFieldType.Bit ) ]
    private bool m_bBorderFormat = false;

    /// <summary>
    /// True if record contains pattern formatting block.
    /// </summary>
    [ BiffRecordPos( 9, 5, TFieldType.Bit ) ]
    private bool m_bPatternFormat = false;

    /// <summary>
    /// True if record contains the user defined number format.
    /// </summary>
    [BiffRecordPos(10, 0, TFieldType.Bit)]
    private bool m_numberFormatIsUserDefined = false;

    #region Font Formatting Block
    /// <summary>
    /// Font height.
    /// </summary>
    private uint m_uiFontHeight = 0xFFFFFFFF;
    /// <summary>
    /// Font options.
    /// </summary>
    private uint m_uiFontOptions;
    /// <summary>
    /// Font weight (100-1000, only if font - style = 0).
    /// Standard values are 0190H (400) for normal text
    /// and 02BCH (700) for bold text.
    /// </summary>
    private ushort m_usFontWeight = 400;
    /// <summary>
    /// Escapement type (only if font - esc = 0):
    /// 0000H = None; 0001H = Superscript; 0002H = Subscript
    /// </summary>
    private ushort m_usEscapmentType;
    /// <summary>
    /// Underline type (only if font - underl = 0):
    /// 00H = None
    /// 01H = Single
    /// 02H = Double
    /// 21H = Single accounting
    /// 22H = Double accounting
    /// </summary>
    private byte m_Underline;
    /// <summary>
    /// Font color index or FFFFFFFFH to preserve the cell font color:
    /// </summary>
    private uint m_uiFontColorIndex = DefaultColorIndex;
    /// <summary>
    /// Option flags for modified font attributes:
    /// </summary>
    private uint m_uiModifiedFlags = 0x0000000F;
    /// <summary>
    /// 0 = Escapement type modified
    /// </summary>
    private uint m_uiEscapmentModified = 1;
    /// <summary>
    /// 0 = Underline type modified:
    /// </summary>
    private uint m_uiUnderlineModified = 1;
    #endregion

    #region Border Formatting Block
    /// <summary>
    /// Border line styles:
    /// </summary>
    private ushort m_usBorderLineStyles;
    /// <summary>
    /// Border line colour indexes:
    /// </summary>
    private uint m_uiBorderColors;
    #endregion

    #region Pattern Formatting Block
    /// <summary>
    /// Fill pattern style:
    /// </summary>
    private ushort m_usPatternStyle;
    /// <summary>
    /// Fill pattern color indexes:
    /// </summary>
    private ushort m_usPatternColors;
    #endregion

    #region Number Formatting Block
    /// <summary>
    /// Unused
    /// </summary>
    private ushort m_unUsed=0;
    /// <summary>
    /// Unsigned integer that specifies the identifier of the number format.
    /// </summary>
    private ushort m_numFormatIndex;
    #endregion

    /// <summary>
    /// <summary>
    /// Formula data for first value or formula (RPN token array without size field):
    /// </summary>
    private byte[] m_arrFirstFormula = new byte[ 0 ];
    /// <summary>
    /// Formula data for second value or formula (RPN token array without size field):
    /// </summary>
    private byte[] m_arrSecondFormula = new byte[ 0 ];

    /// <summary>
    /// 
    /// </summary>
    private Ptg[] m_arrFirstFormulaParsed;
    /// <summary>
    /// 
    /// </summary>
    private Ptg[] m_arrSecondFormulaParsed;
    #endregion

    #region Class properties
    /// <summary>
    /// Type of the conditional formatting: 
    /// 
    /// 01H = Compare with current cell value 
    /// (the comparison specified below is used)
    /// 
    /// 02H = Evaluate a formula (condition is met 
    /// if formula evaluates to a value not equal to 0)
    /// </summary>
    public ExcelCFType FormatType
    {
      get
      {
        return ( ExcelCFType ) m_formatingType;
      }
      set
      {
        m_formatingType = ( byte ) value;
      }
    }

    /// <summary>
    /// Comparison operator: 
    /// 00H = No comparison (only valid for formula type, see above)
    /// 01H = Between
    /// 02H = Not between
    /// 04H = Not equal
    /// 05H = Greater than
    /// 06H = Less than
    /// 03H = Equal
    /// 07H = Greater or equal
    /// 08H = Less or equal
    /// </summary>
    public ExcelComparisonOperator ComparisonOperator
    {
      get
      {
        return ( ExcelComparisonOperator ) m_compareOperator;
      }
      set
      {
        m_compareOperator = ( byte ) value;
      }
    }

    /// <summary>
    /// Size of the formula data for first value or formula. Read-only.
    /// </summary>
    public ushort FirstFormulaSize
    {
      get
      {
        return m_usFirstFormulaSize;
      }
    }

    /// <summary>
    /// Size of the formula data for second value or formula
    /// (sz2, used for second part of "Between" and "Not between"
    /// comparison, this field is 0 for other comparisons). Read-only.
    /// </summary>
    public ushort SecondFormulaSize
    {
      get
      {
        return m_usSecondFormulaSize;
      }
    }

    /// <summary>
    /// Option flags.
    /// </summary>
    public uint Options
    {
      get
      {
        return m_uiOptions;
      }
      internal set
      {
        m_uiOptions = value;
      }
    }

    /// <summary>
    /// Not used.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
      internal set
      {
        m_usReserved = value;
      }
    }

    /// <summary>
    /// True if left border style and color are modified.
    /// </summary>
    public bool IsLeftBorderModified
    {
      get
      {
        return !m_bLeftBorder;
      }
      set
      {
        m_bLeftBorder = !value;
      }
    }

    /// <summary>
    /// True if right border style and color modified.
    /// </summary>
    public bool IsRightBorderModified
    {
      get
      {
        return !m_bRightBorder;
      }
      set
      {
        m_bRightBorder = !value;
      }
    }

    /// <summary>
    /// True if top border style and color are modified.
    /// </summary>
    public bool IsTopBorderModified
    {
      get
      {
        return !m_bTopBorder;
      }
      set
      {
        m_bTopBorder = !value;
      }
    }

    /// <summary>
    /// True if bottom border style and color are modified.
    /// </summary>
    public bool IsBottomBorderModified
    {
      get
      {
        return !m_bBottomBorder;
      }
      set
      {
        m_bBottomBorder = !value;
      }
    }

    /// <summary>
    /// True if pattern style is modified.
    /// </summary>
    public bool IsPatternStyleModified
    {
      get
      {
        return !m_bPatternStyle;
      }
      set
      {
        m_bPatternStyle = !value;
      }
    }

    /// <summary>
    /// True if pattern color is modified.
    /// </summary>
    public bool IsPatternColorModified
    {
      get
      {
        return !m_bPatternColor;
      }
      set
      {
        m_bPatternColor = !value;
      }
    }

    /// <summary>
    /// False if pattern background color is modified.
    /// </summary>
    public bool IsPatternBackColorModified
    {
      get
      {
        return !m_bPatternBackColor;
      }
      set
      {
        m_bPatternBackColor = !value;
      }
    }

    /// <summary>
    /// False if Number format is modified.
    /// </summary>
    public bool IsNumberFormatModified
    {
        get
        {
            return !m_bNumberFormatModified;
        }
        set
        {
            m_bNumberFormatModified = !value;
        }
    }

    /// <summary>
    /// True if record contains font formatting block.
    /// </summary>
    public bool IsFontFormatPresent
    {
      get
      {
        return m_bFontFormat;
      }
      set
      {
        m_bFontFormat = value;
      }
    }

    /// <summary>
    /// True if record contains border formatting block.
    /// </summary>
    public bool IsBorderFormatPresent
    {
      get
      {
        return m_bBorderFormat;
      }
      set
      {
        m_bBorderFormat = value;
      }
    }

    /// <summary>
    /// True if record contains pattern formatting block.
    /// </summary>
    public bool IsPatternFormatPresent
    {
      get
      {
        return m_bPatternFormat;
      }
      set
      {
        m_bPatternFormat = value;
      }
    }

    /// <summary>
    /// True if record contains Number format.
    /// </summary>
    public bool IsNumberFormatPresent
    {
        get
        {
            return m_bNumberFormatPresent;
        }
        set
        {
            m_bNumberFormatPresent = value;
        }
    }

    /// <summary>
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_MINIMUM_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Font height (in twips = 1/20 of a point);
    /// or FFFFFFFFH to preserve the cell font height.
    /// </summary>
    public uint FontHeight
    {
      get
      {
        return m_uiFontHeight;
      }
      set
      {
        if( m_uiFontHeight != value )
        {
          // TODO: Set flags that font block is present
          // and height was modified.
          m_uiFontHeight = value;
        }
      }
    }
    /// <summary>
    /// Posture: 0 = Normal; 1 = Italic
    /// </summary>
    public bool FontPosture
    {
      get
      {
        return ( ( m_uiFontOptions & DEF_FONT_POSTURE_MASK ) != 0 );
      }
      set
      {
        if( value != FontPosture )
        {
          m_uiFontOptions &= ~DEF_FONT_POSTURE_MASK;
          
          if( value ) m_uiFontOptions += DEF_FONT_POSTURE_MASK;
        }
      }
    }
    /// <summary>
    /// Cancellation: 0 = Off; 1 = On
    /// </summary>
    public bool FontCancellation
    {
      get
      {
        return ( ( m_uiFontOptions & DEF_FONT_CANCELLATION_MASK ) != 0 );
      }
      set
      {
        if( value != FontCancellation )
        {
          m_uiFontOptions &= ~DEF_FONT_CANCELLATION_MASK;
          
          if( value ) m_uiFontOptions += DEF_FONT_CANCELLATION_MASK;
        }
      }
    }
    /// <summary>
    /// Font weight (400 - Normal text, 700 - Bold text).
    /// </summary>
    public ushort FontWeight
    {
      get
      {
        return m_usFontWeight;
      }
      set
      {
        if( m_usFontWeight != value )
        {
          m_usFontWeight = value;
          // TODO: set some flags
        }
      }
    }

    /// <summary>
    /// Escapement type.
    /// </summary>
    public ExcelFontVertialAlignment FontEscapment
    {
      get
      {
        return ( ExcelFontVertialAlignment )m_usEscapmentType;
      }
      set
      {
        if( m_usEscapmentType != ( ushort ) value )
        {
          m_usEscapmentType = ( ushort ) value;
          // TODO: set some flags
        }
      }
    }
    /// <summary>
    /// Underline type.
    /// </summary>
    public ExcelUnderline FontUnderline
    {
      get
      {
        return ( ExcelUnderline ) m_Underline;
      }
      set
      {
        if( m_Underline != ( byte ) value )
        {
          m_Underline = ( byte ) value;
          // TODO: set some flags
        }
      }
    }
    /// <summary>
    /// Font color index; or FFFFFFFFH to preserve the cell font color.
    /// </summary>
    public uint FontColorIndex
    {
      get
      {
        return m_uiFontColorIndex;
      }
      set
      {
        if( m_uiFontColorIndex != value )
        {
          m_uiFontColorIndex = value;
          // TODO: set some flags
        }
      }
    }
    /// <summary>
    /// Indicates whether font style (posture or boldness) was modified.
    /// </summary>
    public bool IsFontStyleModified
    {
      get
      {
        return ( ( m_uiModifiedFlags & DEF_FONT_STYLE_MODIFIED_MASK ) == 0 );
      }
      set
      {
        if( value != IsFontStyleModified )
        {
          m_uiModifiedFlags &= ~DEF_FONT_STYLE_MODIFIED_MASK;
          
          if( !value ) m_uiModifiedFlags += DEF_FONT_STYLE_MODIFIED_MASK;
        }
      }
    }
    /// <summary>
    /// Indicates whether font cancellation was modified.
    /// </summary>
    public bool IsFontCancellationModified
    {
      get
      {
        return ( ( m_uiModifiedFlags & DEF_FONT_CANCELLATION_MODIFIED_MASK ) == 0 );
      }
      set
      {
        if( value != IsFontCancellationModified )
        {
          m_uiModifiedFlags &= ~DEF_FONT_CANCELLATION_MODIFIED_MASK;
          
          if( !value ) m_uiModifiedFlags += DEF_FONT_CANCELLATION_MODIFIED_MASK;
        }
      }
    }
    /// <summary>
    /// Indicates whether font escapment was modified.
    /// </summary>
    public bool IsFontEscapmentModified
    {
      get
      {
        return ( m_uiEscapmentModified == 0 );
      }
      set
      {
        m_uiEscapmentModified = (uint) ( value ? 0 : 1 );
      }
    }
    /// <summary>
    /// Indicates whether font underline was modified.
    /// </summary>
    public bool IsFontUnderlineModified
    {
      get
      {
        return ( m_uiUnderlineModified == 0 );
      }
      set
      {
        m_uiUnderlineModified = ( uint ) ( value ? 0 : 1 );
      }
    }
    /// <summary>
    /// True if number format is user defined.
    /// </summary>
    public bool IsNumberFormatUserDefined
    {
        get
        {
            return m_numberFormatIsUserDefined;
        }
        set
        {
            m_numberFormatIsUserDefined = value;
        }
    }

    /// <summary>
    /// <summary>
    /// Number format.
    /// </summary>
    public ushort NumberFormatIndex
    {
        get
        {
            return m_numFormatIndex;
        }
        set
        {
            m_numFormatIndex = value;
        }
    }

    /// <summary>
    /// Left border line style.
    /// </summary>
    public ExcelLineStyle LeftBorderStyle
    {
      get
      {
        return ( ExcelLineStyle ) GetUInt16BitsByMask( 
          m_usBorderLineStyles, DEF_BORDER_LEFT_MASK );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usBorderLineStyles, DEF_BORDER_LEFT_MASK
          , ( ushort ) value );
      }
    }
    /// <summary>
    /// Right border line style.
    /// </summary>
    public ExcelLineStyle RightBorderStyle
    {
      get
      {
        return ( ExcelLineStyle ) ( GetUInt16BitsByMask(
          m_usBorderLineStyles, DEF_BORDER_RIGHT_MASK ) >> 4 );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usBorderLineStyles, DEF_BORDER_RIGHT_MASK
          , ( ushort ) ( ( int ) value << 4 ) );
      }
    }
    /// <summary>
    /// Top border line style.
    /// </summary>
    public ExcelLineStyle TopBorderStyle
    {
      get
      {
        return ( ExcelLineStyle ) ( GetUInt16BitsByMask(
          m_usBorderLineStyles, DEF_BORDER_TOP_MASK ) >> 8 );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usBorderLineStyles, DEF_BORDER_TOP_MASK
          , ( ushort ) ( ( int ) value << 8 ) );
      }
    }
    /// <summary>
    /// Bottom border line style.
    /// </summary>
    public ExcelLineStyle BottomBorderStyle
    {
      get
      {
        return ( ExcelLineStyle ) ( GetUInt16BitsByMask(
          m_usBorderLineStyles, DEF_BORDER_BOTTOM_MASK ) >> 12 );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usBorderLineStyles, DEF_BORDER_BOTTOM_MASK
          , ( ushort ) ( ( int ) value << 12 ) );
      }
    }

    /// <summary>
    /// Color index for left line.
    /// </summary>
    public uint LeftBorderColorIndex
    {
      get
      {
        return GetUInt32BitsByMask( m_uiBorderColors
          , DEF_BORDER_LEFT_COLOR_MASK );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiBorderColors, DEF_BORDER_LEFT_COLOR_MASK
          , value );
      }
    }
    /// <summary>
    /// Color index for right line.
    /// </summary>
    public uint RightBorderColorIndex
    {
      get
      {
        return ( uint ) ( GetUInt32BitsByMask( m_uiBorderColors
          , DEF_BORDER_RIGHT_COLOR_MASK ) >> DEF_BORDER_RIGHT_COLOR_START );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiBorderColors, DEF_BORDER_RIGHT_COLOR_MASK
          , value << DEF_BORDER_RIGHT_COLOR_START );
      }
    }
    /// <summary>
    /// Color index for top line.
    /// </summary>
    public uint TopBorderColorIndex
    {
      get
      {
        return ( uint ) ( GetUInt32BitsByMask( m_uiBorderColors
          , DEF_BORDER_TOP_COLOR_MASK ) >> DEF_BORDER_TOP_COLOR_START );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiBorderColors, DEF_BORDER_TOP_COLOR_MASK
          , value << DEF_BORDER_TOP_COLOR_START );
      }
    }
    /// <summary>
    /// Color index for bottom line.
    /// </summary>
    public uint BottomBorderColorIndex
    {
      get
      {
        return ( uint ) ( GetUInt32BitsByMask( m_uiBorderColors
          , DEF_BORDER_BOTTOM_COLOR_MASK ) >> DEF_BORDER_BOTTOM_COLOR_START );
      }
      set
      {
        SetUInt32BitsByMask( ref m_uiBorderColors, DEF_BORDER_BOTTOM_COLOR_MASK
          , value << DEF_BORDER_BOTTOM_COLOR_START );
      }
    }

    /// <summary>
    /// Fill pattern style.
    /// </summary>
    public ExcelPattern PatternStyle
    {
      get
      {
        return ( ExcelPattern ) ( GetUInt16BitsByMask( m_usPatternStyle
          , DEF_PATTERN_MASK ) >> DEF_PATTERN_START );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usPatternStyle, DEF_PATTERN_MASK
          , ( ushort ) ( ( int ) value << DEF_PATTERN_START ) );
      }
    }
    /// <summary>
    /// Color index for pattern.
    /// </summary>
    public ushort PatternColorIndex
    {
      get
      {
        return GetUInt16BitsByMask( m_usPatternColors
          , DEF_PATTERN_COLOR_MASK );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usPatternColors, DEF_PATTERN_COLOR_MASK
          , value );
      }
    }
    /// <summary>
    /// Color index for pattern background.
    /// </summary>
    public ushort PatternBackColor
    {
      get
      {
        return ( ushort ) ( GetUInt16BitsByMask( m_usPatternColors
          , DEF_PATTERN_BACKCOLOR_MASK ) >> DEF_PATTERN_BACKCOLOR_START );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usPatternColors, DEF_PATTERN_BACKCOLOR_MASK
          , ( ushort ) ( value << DEF_PATTERN_BACKCOLOR_START ) );
      }
    }

    /// <summary>
    /// Parsed first formula string.
    /// </summary>
    public Ptg[] FirstFormulaPtgs
    {
      get
      {
//        if( m_arrFirstFormulaParsed == null && m_arrFirstFormula != null )
//        {
//          int iLength = m_arrFirstFormula.Length;
//          m_arrFirstFormulaParsed = FormulaUtil.ParseExpression( 
//            new ByteArrayDataProvider( m_arrFirstFormula ), iLength, m_version );
//        }

        return m_arrFirstFormulaParsed;
      }
      set
      {
        m_arrFirstFormula = FormulaUtil.PtgArrayToByteArray( value, ExcelVersion.Excel2007 );
        m_arrFirstFormulaParsed = value;
        m_usFirstFormulaSize = ( ushort ) m_arrFirstFormula.Length;
      }
    }

    /// <summary>
    /// Parsed second formula string.
    /// </summary>
    public Ptg[] SecondFormulaPtgs
    {
      get
      {
//        if( m_arrSecondFormulaParsed == null && m_arrSecondFormula != null )
//        {
//          int iLength = m_arrSecondFormula.Length;
//          m_arrSecondFormulaParsed = FormulaUtil.ParseExpression( 
//            new ByteArrayDataProvider( m_arrSecondFormula ), iLength, m_version );
//        }

        return m_arrSecondFormulaParsed;
      }
      set
      {
        m_arrSecondFormula = FormulaUtil.PtgArrayToByteArray( value, ExcelVersion.Excel2007 );
        m_arrSecondFormulaParsed = value;
        m_usSecondFormulaSize = ( ushort ) m_arrSecondFormula.Length;
      }
    }
    /// <summary>
    ///  Returns bytes of the first formula. Read-only.
    /// </summary>
    public byte[] FirstFormulaBytes
    {
      get
      {
        return m_arrFirstFormula;
      }
    }
    /// <summary>
    ///  Returns bytes of the second formula. Read-only.
    /// </summary>
    public byte[] SecondFormulaBytes
    {
      get
      {
        return m_arrSecondFormula;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  CFRecord()
      : base()
    {
      m_uiOptions |= 0x38C3FF;
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  CFRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserves for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CFRecord( int iReserve )
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
      m_formatingType = provider.ReadByte( iOffset );
      iOffset++;

      m_compareOperator = provider.ReadByte( iOffset );
      iOffset++;

      m_usFirstFormulaSize = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usSecondFormulaSize = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_uiOptions = provider.ReadUInt32( iOffset );
      iOffset++;

      m_bLeftBorder = provider.ReadBit( iOffset, 2 );
      m_bRightBorder = provider.ReadBit( iOffset, 3 );
      m_bTopBorder = provider.ReadBit( iOffset, 4 );
      m_bBottomBorder = provider.ReadBit( iOffset, 5 );
      iOffset++;

      m_bPatternStyle = provider.ReadBit( iOffset, 0 );
      m_bPatternColor = provider.ReadBit( iOffset, 1 );
      m_bPatternBackColor = provider.ReadBit( iOffset, 2 );
      m_bNumberFormatModified = provider.ReadBit(iOffset, 3);
      iOffset++;

      m_bNumberFormatPresent = provider.ReadBit(iOffset, 1);
      m_bFontFormat = provider.ReadBit( iOffset, 2 );
      m_bBorderFormat = provider.ReadBit( iOffset, 4 );
      m_bPatternFormat = provider.ReadBit( iOffset, 5 );
      iOffset++;

      //iOffset += 4;

      m_numberFormatIsUserDefined = provider.ReadBit(iOffset, 0);
      iOffset++;

      m_usReserved = provider.ReadUInt16( iOffset );
      iOffset += 1;

      if (!m_numberFormatIsUserDefined)
      {
          ParseNumberFormatBlock(provider, ref iOffset);
      }

      ParseFontBlock( provider, ref iOffset );
      ParseBorderBlock( provider, ref iOffset );
      ParsePatternBlock( provider, ref iOffset );

      m_arrFirstFormula = new byte[ m_usFirstFormulaSize ];
      provider.ReadArray( iOffset, m_arrFirstFormula );
      iOffset += m_usFirstFormulaSize;

      m_arrSecondFormula = new byte[ m_usSecondFormulaSize ];
      provider.ReadArray( iOffset, m_arrSecondFormula );

      m_arrFirstFormulaParsed = FormulaUtil.ParseExpression( 
        new ByteArrayDataProvider( m_arrFirstFormula ), m_usFirstFormulaSize, version );

      m_arrSecondFormulaParsed = FormulaUtil.ParseExpression( 
        new ByteArrayDataProvider( m_arrSecondFormula ), m_usSecondFormulaSize, version );

      if( version != ExcelVersion.Excel2007 )
      {
        // To compare records correctly regardless original version.
        if( m_usFirstFormulaSize > 0 )
        {
          m_arrFirstFormula = FormulaUtil.PtgArrayToByteArray( m_arrFirstFormulaParsed, ExcelVersion.Excel2007 );
          m_usFirstFormulaSize = ( ushort )m_arrFirstFormula.Length;
        }

        if( m_usSecondFormulaSize > 0 )
        {
          m_arrSecondFormula = FormulaUtil.PtgArrayToByteArray( m_arrSecondFormulaParsed, ExcelVersion.Excel2007 );
          m_usSecondFormulaSize = ( ushort )m_arrSecondFormula.Length;
        }
      }
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
      m_iLength = GetStoreSize( version );

      if( m_arrFirstFormulaParsed != null && m_arrFirstFormulaParsed.Length > 0 )
      {
        m_arrFirstFormula = FormulaUtil.PtgArrayToByteArray( m_arrFirstFormulaParsed, version );
        m_usFirstFormulaSize = ( ushort )m_arrFirstFormula.Length;
      }
      else
      {
        m_arrFirstFormula = null;
        m_usFirstFormulaSize = 0;
      }

      if( m_arrSecondFormulaParsed != null && m_arrSecondFormulaParsed.Length > 0 )
      {
        m_arrSecondFormula = FormulaUtil.PtgArrayToByteArray( m_arrSecondFormulaParsed, version );
        m_usSecondFormulaSize = ( ushort )m_arrSecondFormula.Length;
      }
      else
      {
        m_arrSecondFormula = null;
        m_usSecondFormulaSize = 0;
      }
      byte fomateType = m_formatingType == (byte)1 ? (byte)1 : (byte)2;
      provider.WriteByte(iOffset, fomateType);
      iOffset++;

      provider.WriteByte( iOffset, m_compareOperator );
      iOffset++;

      provider.WriteUInt16( iOffset, m_usFirstFormulaSize );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usSecondFormulaSize );
      iOffset += 2;

      provider.WriteUInt32( iOffset, m_uiOptions );
      iOffset++;

      provider.WriteBit( iOffset, m_bLeftBorder, 2 );
      provider.WriteBit( iOffset, m_bRightBorder, 3 );
      provider.WriteBit( iOffset, m_bTopBorder, 4 );
      provider.WriteBit( iOffset, m_bBottomBorder, 5 );
      iOffset++;

      provider.WriteBit( iOffset, m_bPatternStyle, 0 );
      provider.WriteBit( iOffset, m_bPatternColor, 1 );
      provider.WriteBit( iOffset, m_bPatternBackColor, 2 );
      provider.WriteBit(iOffset, m_bNumberFormatModified, 3);
      iOffset++;

      provider.WriteBit(iOffset, m_bNumberFormatPresent, 1);
      provider.WriteBit( iOffset, m_bFontFormat, 2 );
      provider.WriteBit( iOffset, m_bBorderFormat, 4 );
      provider.WriteBit( iOffset, m_bPatternFormat, 5 );
      iOffset++;

      provider.WriteBit(iOffset, m_numberFormatIsUserDefined,0);
      iOffset++;
      //iOffset += 4;

      provider.WriteUInt16( iOffset, m_usReserved );
      iOffset ++;

      if (!m_numberFormatIsUserDefined)
      {
          SerializeNumberFormatBlock(provider, ref iOffset);
      }

      SerializeFontBlock( provider, ref iOffset );
      SerializeBorderBlock( provider, ref iOffset );
      SerializePatternBlock( provider, ref iOffset );
      
      //provider.WriteBytes( arrBuffer, iOffset, m_arrFirstFormula );
      int iFormulaLen = m_arrFirstFormula.Length;
      provider.WriteBytes( iOffset, m_arrFirstFormula, 0, m_usFirstFormulaSize );
      iOffset += m_usFirstFormulaSize;

      provider.WriteBytes( iOffset, m_arrSecondFormula, 0, m_usSecondFormulaSize );
      iOffset += m_usSecondFormulaSize;
    }

    /// <summary>
    /// Parses font block if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset to the font block data in the internal data array.
    /// </param>
    public int ParseFontBlock( DataProvider provider, ref int iOffset )
    {
      if( !IsFontFormatPresent ) return iOffset;

      iOffset += DEF_FONT_FIRST_RESERVED_SIZE;

      m_uiFontHeight = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiFontOptions = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_usFontWeight = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usEscapmentType = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_Underline = provider.ReadByte( iOffset );
      iOffset += 1;

      iOffset += DEF_FONT_SECOND_RESERVED_SIZE;

      m_uiFontColorIndex = provider.ReadUInt32( iOffset );
      iOffset += 4;

      iOffset += 4;

      m_uiModifiedFlags = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiEscapmentModified = provider.ReadUInt32( iOffset );
      iOffset += 4;

      m_uiUnderlineModified = provider.ReadUInt32( iOffset );
      iOffset += 4;

//      SetByte( iOffset, 0, DEF_FONT_THIRD_RESERVED_SIZE );
      iOffset += DEF_FONT_THIRD_RESERVED_SIZE;

//      SetUInt16( iOffset, 0x0001 );
      iOffset += 2;

      return iOffset;
    }
    /// <summary>
    /// Parses border block if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset to the border block data in the internal data array.
    /// </param>
    public int ParseBorderBlock( DataProvider provider, ref int iOffset )
    {
      if( !IsBorderFormatPresent ) return iOffset;

      m_usBorderLineStyles = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_uiBorderColors = provider.ReadUInt32( iOffset );
      iOffset += 4;

      iOffset += 2;

      return iOffset;
    }
    /// <summary>
    /// Parses pattern block if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset to the pattern block data in the internal data array.
    /// </param>
    public int ParsePatternBlock(DataProvider provider, ref int iOffset)
    {
      if( !IsPatternFormatPresent ) return iOffset;

      m_usPatternStyle = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usPatternColors = provider.ReadUInt16( iOffset );
      iOffset += 2;

      return iOffset;
    }
    /// <summary>
    /// Parses number format block if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset to the number format block data in the internal data array.
    /// </param>
    public int ParseNumberFormatBlock(DataProvider provider, ref int iOffset)
    {
        if (!IsNumberFormatPresent) return iOffset;

        m_unUsed = provider.ReadByte(iOffset);
        iOffset++;

        m_numFormatIndex = provider.ReadByte(iOffset);
        iOffset++;

        return iOffset;
    }
    /// <summary>
    /// Writes font block into internal data array
    /// if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset where font block should be written.
    /// </param>
    public int SerializeFontBlock(DataProvider provider, ref int iOffset)
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      if (!IsFontFormatPresent) return iOffset;

      //SetByte( arrBuffer, iOffset, 0, DEF_FONT_FIRST_RESERVED_SIZE );
      for( int i = 0; i < DEF_FONT_FIRST_RESERVED_SIZE; i++, iOffset++ )
      {
        provider.WriteByte( iOffset, 0 );
      }

      //iOffset += DEF_FONT_FIRST_RESERVED_SIZE;

      provider.WriteUInt32( iOffset, m_uiFontHeight );
      iOffset += 4;

      provider.WriteUInt32( iOffset, m_uiFontOptions );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usFontWeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usEscapmentType );
      iOffset += 2;

      provider.WriteByte( iOffset, m_Underline );
      iOffset += 1;

      //provider.WriteByte( arrBuffer, iOffset, 0, DEF_FONT_SECOND_RESERVED_SIZE );
      for( int i = 0; i < DEF_FONT_SECOND_RESERVED_SIZE; i++, iOffset++ )
      {
        provider.WriteByte( iOffset, 0 );
      }
      //iOffset += DEF_FONT_SECOND_RESERVED_SIZE;

      provider.WriteUInt32( iOffset, m_uiFontColorIndex );
      iOffset += 4;

      provider.WriteUInt32( iOffset, 0 );
      iOffset += 4;

      provider.WriteUInt32( iOffset, m_uiModifiedFlags );
      iOffset += 4;

      provider.WriteUInt32( iOffset, m_uiEscapmentModified );
      iOffset += 4;

      provider.WriteUInt32( iOffset, m_uiUnderlineModified );
      iOffset += 4;

      for( int i = 0; i < DEF_FONT_THIRD_RESERVED_SIZE; i++, iOffset++ )
      {
        provider.WriteByte( iOffset, 0 );
      }
//      provider.WriteByte( arrBuffer, iOffset, 0, DEF_FONT_THIRD_RESERVED_SIZE );
      //iOffset += DEF_FONT_THIRD_RESERVED_SIZE;

      provider.WriteUInt16( iOffset, 0x0001 );
      iOffset += 2;

      return iOffset;
    }
    /// <summary>
    /// Writes border block into internal data array
    /// if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset where border block should be written.
    /// </param>
    public int SerializeBorderBlock(DataProvider provider, ref int iOffset)
    {
        if (!IsBorderFormatPresent) return iOffset;

      if( provider == null )
        throw new ArgumentNullException( "provider" );

      provider.WriteUInt16( iOffset, m_usBorderLineStyles );
      iOffset += 2;

      provider.WriteUInt32( iOffset, m_uiBorderColors );
      iOffset += 4;

      provider.WriteUInt16( iOffset, 0 );
      iOffset += 2;

      return iOffset;
    }
    /// <summary>
    /// Writes pattern block into internal data array
    /// if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset where pattern block should be written.
    /// </param>
    public int SerializePatternBlock(DataProvider provider, ref int iOffset)
    {
      if( !IsPatternFormatPresent ) return iOffset;

      if( provider == null )
        throw new ArgumentNullException( "provider" );

      provider.WriteUInt16( iOffset, m_usPatternStyle );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usPatternColors );
      iOffset += 2;

      return iOffset;
    }
    /// <summary>
    /// Writes number format block into internal data array
    /// if it is present in the conditional format.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">
    /// Offset where number format block should be written.
    /// </param>
    public int SerializeNumberFormatBlock(DataProvider provider, ref int iOffset)
    {
        if (!IsNumberFormatPresent) return iOffset;

        if (provider == null)
            throw new ArgumentNullException("provider");

        provider.WriteUInt16(iOffset, m_unUsed);
        iOffset++;

        provider.WriteUInt16(iOffset, m_numFormatIndex);
        iOffset++;
        
        return iOffset;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iSize = DEF_MINIMUM_RECORD_SIZE;

      if( IsFontFormatPresent )
      {
        iSize += DEF_FONT_BLOCK_SIZE;
      }

      if( IsBorderFormatPresent )
      {
        iSize += DEF_BORDER_BLOCK_SIZE;
      }

      if( IsPatternFormatPresent )
      {
        iSize += DEF_PATTERN_BLOCK_SIZE;
      }

      if (IsNumberFormatPresent)
      {
          iSize += DEF_NUMBER_FORMAT_BLOCK_SIZE;
      }

      iSize += DVRecord.GetFormulaSize( m_arrFirstFormulaParsed, version, true );
      iSize += DVRecord.GetFormulaSize( m_arrSecondFormulaParsed, version, true );

      return iSize;
    }
    #endregion

    #region ICloneable method
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <returns>Returns new instance.</returns>
    public override object Clone()
    {
      CFRecord result = ( CFRecord )base.Clone();

      result.m_arrFirstFormula = CloneUtils.CloneByteArray( m_arrFirstFormula );
      result.m_arrSecondFormula = CloneUtils.CloneByteArray( m_arrSecondFormula );
      result.m_arrFirstFormulaParsed = CloneUtils.ClonePtgArray( m_arrFirstFormulaParsed );
      result.m_arrSecondFormulaParsed = CloneUtils.ClonePtgArray( m_arrSecondFormulaParsed );

      return result;
    }

    #endregion

    #region Class overrides
    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use in
    /// hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      // TODO: here we can optimize a little bit by caching hash code.
      int iHashCode = m_formatingType.GetHashCode()
        ^ m_compareOperator.GetHashCode()
        ^ m_usFirstFormulaSize.GetHashCode()
        ^ m_usSecondFormulaSize.GetHashCode()
        ^ m_uiOptions.GetHashCode()
        ^ m_usReserved.GetHashCode()
        ^ m_bLeftBorder.GetHashCode()
        ^ m_bRightBorder.GetHashCode()
        ^ m_bTopBorder.GetHashCode()
        ^ m_bBottomBorder.GetHashCode()
        ^ m_bPatternStyle.GetHashCode()
        ^ m_bPatternColor.GetHashCode()
        ^ m_bPatternBackColor.GetHashCode()
        ^ m_bNumberFormatModified.GetHashCode()
        ^ m_bNumberFormatPresent.GetHashCode()
        ^ m_bFontFormat.GetHashCode()
        ^ m_bBorderFormat.GetHashCode()
        ^ m_bPatternFormat.GetHashCode()

        ^ m_uiFontHeight.GetHashCode()
        ^ m_uiFontOptions.GetHashCode()
        ^ m_usFontWeight.GetHashCode()
        ^ m_usEscapmentType.GetHashCode()
        ^ m_Underline.GetHashCode()
        ^ m_uiFontColorIndex.GetHashCode()
        ^ m_uiModifiedFlags.GetHashCode()
        ^ m_uiEscapmentModified.GetHashCode()
        ^ m_uiUnderlineModified.GetHashCode()

        ^ m_usBorderLineStyles.GetHashCode()
        ^ m_uiBorderColors.GetHashCode()

        ^ m_usPatternStyle.GetHashCode()
        ^ m_usPatternColors.GetHashCode();

      return iHashCode;

      // NOTE: here we can also add formula into hash code evaluation.
//    private byte[] m_arrFirstFormula = new byte[ 0 ];
//    private byte[] m_arrSecondFormula = new byte[ 0 ];
//
//    private Ptg[] m_arrFirstFormulaParsed;
//    private Ptg[] m_arrSecondFormulaParsed;
    }
    /// <summary>
    /// A hash code for the current Object without taking cell list into account.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {
      CFRecord toCompare = obj as CFRecord;

      if( toCompare == null ) return false;

      bool bResult = m_formatingType == toCompare.m_formatingType
        && m_compareOperator == toCompare.m_compareOperator
        && m_usFirstFormulaSize == toCompare.m_usFirstFormulaSize
        && m_usSecondFormulaSize == toCompare.m_usSecondFormulaSize
        && m_uiOptions == toCompare.m_uiOptions
        && m_bLeftBorder == toCompare.m_bLeftBorder
        && m_bRightBorder == toCompare.m_bRightBorder
        && m_bTopBorder == toCompare.m_bTopBorder
        && m_bBottomBorder == toCompare.m_bBottomBorder
        && m_bPatternStyle == toCompare.m_bPatternStyle
        && m_bPatternColor == toCompare.m_bPatternColor
        && m_bPatternBackColor == toCompare.m_bPatternBackColor
        && m_bNumberFormatModified == toCompare.m_bNumberFormatModified
        && m_bNumberFormatPresent==toCompare.m_bNumberFormatPresent
        && m_bFontFormat == toCompare.m_bFontFormat
        && m_bBorderFormat == toCompare.m_bBorderFormat
        && m_bPatternFormat == toCompare.m_bPatternFormat

        && m_uiFontHeight == toCompare.m_uiFontHeight
        && m_uiFontOptions == toCompare.m_uiFontOptions
        && m_usFontWeight == toCompare.m_usFontWeight
        && m_usEscapmentType == toCompare.m_usEscapmentType
        && m_Underline == toCompare.m_Underline
        && m_uiFontColorIndex == toCompare.m_uiFontColorIndex
        && m_uiModifiedFlags == toCompare.m_uiModifiedFlags
        && m_uiEscapmentModified == toCompare.m_uiEscapmentModified
        && m_uiUnderlineModified == toCompare.m_uiUnderlineModified

        && m_usBorderLineStyles == toCompare.m_usBorderLineStyles
        && m_uiBorderColors == toCompare.m_uiBorderColors

        && m_usPatternStyle == toCompare.m_usPatternStyle
        && m_usPatternColors == toCompare.m_usPatternColors;

      // Compare formula if necessary.
      if( bResult )
      {
        bResult = BiffRecordRaw.CompareArrays( m_arrFirstFormula, toCompare.m_arrFirstFormula )
          && BiffRecordRaw.CompareArrays( m_arrSecondFormula, toCompare.m_arrSecondFormula );
      }

      return bResult;
    }

    #endregion
  }
}
