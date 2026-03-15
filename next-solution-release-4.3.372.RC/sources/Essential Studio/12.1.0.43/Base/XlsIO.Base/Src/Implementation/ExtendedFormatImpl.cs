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
using System.Reflection;
using System.Diagnostics;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Contains the font attributes (font name, font size,
  /// color, and so on) for an object.
  /// </summary>
  public class ExtendedFormatImpl
    : CommonObject
    , IInternalExtendedFormat
    , IComparable
    , ICloneable
    , IXFIndex
    , ICloneParent,IDisposable
  {
    #region Class constants
    /// <summary>
    /// Weight of the bold font.
    /// </summary>
    internal ushort FONTBOLD = 700;
    /// <summary>
    /// Weight of the normal font.
    /// </summary>
    internal ushort FONTNORMAL = 400;
    /// <summary>
    /// Parent index that indicates that extended format doesn't have any parent format.
    /// </summary>
    public const int DEF_NO_PARENT_INDEX = 4095;
    /// <summary>
    /// Indicates that text is drawn from top to bottom.
    /// </summary>
    public const int TopToBottomRotation = 255;
    /// <summary>
    /// Maximum tint value.
    /// </summary>
    public const int MaxTintValue = 32767;
    #endregion

    #region Class members
    /// <summary>
    /// ExtendedFormatRecord that contains information about font and format indexes,
    /// borders, colors, etc.
    /// </summary>
    private ExtendedFormatRecord  m_extFormat;
    /// Extended X Format
    /// </summary>
    private ExtendedXFRecord m_xfExt;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl          m_book;
    /// <summary>
    /// Position of this format in m_book.InnerExtFormats.
    /// </summary>
    private int                m_iXFIndex;
    /// <summary>
    /// ShapeFill implementation that contains gradient fill effects.
    /// </summary>
    private ShapeFillImpl         m_gradient;
    /// <summary>
    /// 
    /// </summary>
    private ColorObject m_color;
    /// <summary>
    /// 
    /// </summary>
    private ColorObject m_patternColor;
    private ColorObject m_topBorderColor;
    private ColorObject m_bottomBorderColor;
    private ColorObject m_leftBorderColor;
    private ColorObject m_rightBorderColor;
    private ColorObject m_diagonalBorderColor;
    /// <summary>
    /// Represent the cell-border
    /// </summary>
    private bool m_hasBorder;    
    #endregion

    #region IExtendedFormat members
    /// <summary>
    /// Get / set font index.
    /// </summary>
    public int      FontIndex
    {
      get
      {
        return IncludeFont
          ? m_extFormat.FontIndex
          : ParentRecord.FontIndex;
      }
      set
      {
        if( FontIndex != value )
        {
          IncludeFont = true;
          m_extFormat.FontIndex = ( ushort )value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets format index in m_book.InnerFormats.
    /// </summary>
    public int      XFormatIndex
    {
      get
      {
        return Index;
      }
    }
      
    /// <summary>
    /// Get / set Format index.
    /// </summary>
    public int      NumberFormatIndex
    {
      get
      {
        return IncludeNumberFormat
          ? m_extFormat.FormatIndex
          : ParentRecord.FormatIndex;
      }
      set
      {
        if( !m_book.InnerFormats.Contains( value ) )
        {
          throw new ArgumentOutOfRangeException( "Unknown format index" );
        }

        if( NumberFormatIndex != value )
        {
          IncludeNumberFormat = true;
          m_extFormat.FormatIndex = ( ushort )value;
        }

        SetChanged();
      }
    }
    /// <summary>
    /// Get / set fill pattern.
    /// </summary>
    public ExcelPattern      FillPattern
    {
      get
      {
        return ( ExcelPattern )( IncludePatterns
          ? m_extFormat.AdtlFillPattern
          : ParentRecord.AdtlFillPattern );
      }
      set
      {
        if( FillPattern != value )
        {
          IncludePatterns = true;

          if( value == ExcelPattern.None )
          {
            ColorIndex = ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX;
            PatternColorIndex = ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX;
          }

          m_extFormat.AdtlFillPattern = ( ushort )value;

          SetChanged();
        }
      }
    }
    /// <summary>
    /// Get / set fill background color.
    /// </summary>
    public ExcelKnownColors FillBackground
    {
      get
      {
        return ColorIndex;
      }
      set
      {
        ColorIndex = value;
      }
    }
    /// <summary>
    /// Get / set fill background color.
    /// </summary>
    public Color FillBackgroundRGB
    {
      get
      {
        return Color;
      }
      set
      {
        Color = value;
      }
    }
    /// <summary>
    /// Get / set fill foreground color.
    /// </summary>
    public ExcelKnownColors FillForeground
    {
      get
      {
        return PatternColorIndex;
      }
      set
      {
        PatternColorIndex = value;
      }
    }
    /// <summary>
    /// Get / set fill foreground color.
    /// </summary>
    public Color FillForegroundRGB
    {
      get
      {
        return PatternColor;
      }
      set
      {
        PatternColor = value;
      }
    }
    /// <summary>
    /// Horizontal alignment.
    /// </summary>
    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        ExtendedFormatRecord record = IncludeAlignment
          ? m_extFormat
          : ParentRecord;

        return record.HAlignmentType;
      }
      set
      {
        if( HorizontalAlignment != value )
        {
          IncludeAlignment = true;
          m_extFormat.HAlignmentType = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Indent level.
    /// </summary>
    public int      IndentLevel
    {
      get
      {
        return m_extFormat.Indent;
      }
      set
      {
        if( IndentLevel != value )
        {
          if( value > m_book.MaxIndent )
            throw new ArgumentOutOfRangeException( "IndentLevel" );

          IncludeAlignment = true;
          m_extFormat.Indent = ( byte )value;

          if (HorizontalAlignment == ExcelHAlign.HAlignGeneral)
              HorizontalAlignment = ExcelHAlign.HAlignLeft;

          if( value != 0 )
            m_extFormat.Rotation = 0;

          SetChanged();
        }
      }
    }
    /// <summary>
    /// Indicates whether formula is hidden.
    /// </summary>
    public bool     FormulaHidden
    {
      get
      {
        return m_extFormat.IsHidden;
      }
      set
      {
        m_extFormat.IsHidden = value;

        SetChanged();
      }
    }
    /// <summary>
    /// Indicates whether cell with this XF is locked.
    /// </summary>
    public bool     Locked
    {
      get
      {
        return m_extFormat.IsLocked;
      }
      set
      {
        if( Locked != value )
        {
          IncludeProtection = true;
          m_extFormat.IsLocked = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// For far east languages. Supported only for format. Always 0 for US.
    /// </summary>
    public bool     JustifyLast
    {
      get
      {
        return m_extFormat.JustifyLast;
      }
      set
      {
        m_extFormat.JustifyLast = value;

        SetChanged();
      }
    }

    /// <summary>
    /// Returns or sets the format code for the object. Read/write String.
    /// </summary>
    public string   NumberFormat
    {
      get
      {
        return ( ( FormatImpl )m_book.InnerFormats[ NumberFormatIndex ] ).FormatString;
      }
      set
      {
        NumberFormatIndex = ( ushort )m_book.InnerFormats.FindOrCreateFormat( value );
        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the format code for the object as a string in the
    /// language of the user. Read/write String.
    /// </summary>
    public string   NumberFormatLocal
    {
      get
      {
        return NumberFormat;
      }
      set
      {
        NumberFormat = value;
      }
    }
    /// <summary>
    /// Returns object that describes number format. Read-only.
    /// </summary>
    public INumberFormat NumberFormatSettings
    {
      get
      {
        return ( INumberFormat )m_book.InnerFormats[ NumberFormatIndex ];
      }
    }
    /// <summary>
    /// True- Indicates that the contents are shrunk to fit into cell.
    /// </summary>
    public bool     ShrinkToFit
    {
      get
      {
        return m_extFormat.ShrinkToFit;
      }
      set
      {
        if( value != ShrinkToFit )
        {
          IncludeAlignment = true;

          m_extFormat.ShrinkToFit = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// True - Indicates  that text is wrapped at right border.
    /// </summary>
    public bool     WrapText
    {
      get
      {
        return m_extFormat.WrapText;
      }
      set
      {
        if( WrapText != value )
        {
          IncludeAlignment = true;
          m_extFormat.WrapText = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Vertical alignment.
    /// </summary>
    public ExcelVAlign VerticalAlignment
    {
      get
      {
        ExtendedFormatRecord record = IncludeAlignment
          ? m_extFormat
          : ParentRecord;

        return record.VAlignmentType;
      }
      set
      {
        if( VerticalAlignment != value )
        {
          IncludeAlignment = true;
          m_extFormat.VAlignmentType = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Flag for horizontal and vertical alignment, text wrap, indentation,
    /// orientation, rotation, and text direction. If False, the attribute
    /// of parent style is used.
    /// </summary>
    public bool     IncludeAlignment
    {
      get
      {
        bool bResult = m_extFormat.IsNotParentAlignment;

        return HasParent ? bResult : !bResult;
      }
      set
      {
        if( HasParent )
        {
          if( IncludeAlignment != value )
          {
            if( value && !m_book.Loading )
            {
              ExtendedFormatRecord parentRecord = ParentRecord;
              m_extFormat.CopyAlignment( parentRecord );
            }

            m_extFormat.IsNotParentAlignment = value;
            SetChanged();
          }
        }
        else
        {
          m_extFormat.IsNotParentAlignment = !value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Flag for border lines.
    /// If False, the attribute of parent style is used.
    /// </summary>
    public bool     IncludeBorder
    {
      get
      {
        bool bResult = m_extFormat.IsNotParentBorder;
        return HasParent ? bResult : !bResult;
      }
      set
      {
        if( HasParent )
        {
          if( IncludeBorder != value )
          {
            if( value && !m_book.Loading )
            {
              ExtendedFormatImpl parentFormat = ParentFormat;
              CopyBorders( parentFormat );
            }

            m_extFormat.IsNotParentBorder = value;
            SetChanged();
          }
        }
        else
        {
          m_extFormat.IsNotParentBorder = !value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Flag for font.
    /// If False, the attribute of parent style is used.
    /// </summary>
    public bool     IncludeFont
    {
      get
      {
        bool bResult = m_extFormat.IsNotParentFont;
        return HasParent ? bResult : !bResult;
      }
      set
      {
        if( HasParent )
        {
          if( IncludeFont != value )
          {
            if( value && !m_book.Loading )
            {
              // Copy font setting from parent.
              m_extFormat.FontIndex = ParentRecord.FontIndex;
            }

            m_extFormat.IsNotParentFont = value;
            SetChanged();
          }
        }
        else
        {
          m_extFormat.IsNotParentFont = !value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Flag for number format.
    /// If False, the attribute of parent style is used.
    /// </summary>
    public bool     IncludeNumberFormat
    {
      get
      {
        bool bResult = m_extFormat.IsNotParentFormat;
        return HasParent ? bResult : !bResult;
      }
      set
      {
        if( HasParent )
        {
          if( IncludeNumberFormat != value )
          {
            if( value && !m_book.Loading )
            {
              // Copy format setting from parent.
              m_extFormat.FormatIndex = ParentRecord.FormatIndex;
            }

            m_extFormat.IsNotParentFormat = value;
            SetChanged();
          }
        }
        else
        {
          m_extFormat.IsNotParentFormat = !value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Flag for background area style.
    /// If False, the attribute of parent style is used.
    /// </summary>
    public bool     IncludePatterns
    {
      get
      {
        bool bResult = m_extFormat.IsNotParentPattern;
        return HasParent ? bResult : !bResult;
      }
      set
      {
        if( HasParent )
        {
          if( IncludePatterns != value )
          {
            if( value && !m_book.Loading )
            {
              ExtendedFormatImpl parentFormat = ParentFormat;
              CopyPatterns( parentFormat );
            }

            m_extFormat.IsNotParentPattern = value;
            SetChanged();
          }
        }
        else
        {
          m_extFormat.IsNotParentPattern = !value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Flag for cell protection (cell locked and formula hidden).
    /// If False, the attribute of parent style is used.
    /// </summary>
    public bool     IncludeProtection
    {
      get
      {
        bool bResult = m_extFormat.IsNotParentCellOptions;
        return HasParent ? bResult : !bResult;
      }
      set
      {
        if( HasParent )
        {
          if( IncludeProtection != value )
          {
            if( value && !m_book.Loading )
            {
              ExtendedFormatRecord parentRecord = ParentRecord;
              m_extFormat.CopyProtection( parentRecord );
            }

            m_extFormat.IsNotParentCellOptions = value;
            SetChanged();
          }
        }
        else
        {
          m_extFormat.IsNotParentCellOptions = !value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual IFont Font
    {
      get
      {
        return m_book.InnerFonts[ FontIndex ];
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IBorders Borders
    {
      get
      {
        return new BordersCollection( Application, m_book, this );
        //throw new NotImplementedException();
      }
    }

    /// <summary>
    /// If true then first symbol in cell is apostrophe.
    /// </summary>
    public bool     IsFirstSymbolApostrophe
    { 
      get
      {
        return m_extFormat._123Prefix;
      }
      set
      {
        m_extFormat._123Prefix = value;

        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an index into the current color palette.
    /// </summary>
    public ExcelKnownColors PatternColorIndex
    {
      get
      {
        return PatternColorObject.GetIndexed( m_book );
      }
      set
      {
        if( value != PatternColorIndex )
        {
          IncludePatterns = true;
          PatternColorObject.SetIndexed( value );
          m_extFormat.AdtlFillPattern |= 0x1;
          //m_extFormat.FillForeground = ( ushort )value;

          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    public Color PatternColor
    {
      get
      {
        return PatternColorObject.GetRGB( m_book );//m_book.GetPaletteColor( PatternColorIndex );
      }
      set
      {
        IncludePatterns = true;
        PatternColorObject.SetRGB( value, m_book );
          if (m_extFormat.AdtlFillPattern == 0)
              m_extFormat.AdtlFillPattern |= 0x1;
        SetChanged();
        //PatternColorIndex = m_book.GetNearestColor( value );
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    public ColorObject PatternColorObject
    {
      get
      {
        ExtendedFormatImpl format = IncludePatterns
          ? this
          : ParentFormat;

        return format.InnerPatternColor;
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior. The color is specified as
    /// an index value into the current color palette.
    /// </summary>
    public ExcelKnownColors ColorIndex
    {
      get
      {
        return ColorObject.GetIndexed( m_book );
      }
      set
      {
        if( value != ColorIndex )
        {
          IncludePatterns = true;
      //    m_extFormat.FillBackground = ( ushort )value;   
            
            ColorObject.SetIndexed( value,true,m_book );

          if( m_extFormat.AdtlFillPattern == 0 )
            m_extFormat.AdtlFillPattern = 1;

          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    public Color Color
    {
      get
      {
        return ColorObject.GetRGB( m_book );//m_book.GetPaletteColor( ColorIndex );
      }
      set
      {
        IncludePatterns = true;

        ColorObject.SetRGB( value, m_book );

        if( m_extFormat.AdtlFillPattern == 0 )
          m_extFormat.AdtlFillPattern = 1;

        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        ExtendedFormatImpl format = IncludePatterns
          ? this
          : ParentFormat;

        return format.InnerColor;
      }
    }
    /// <summary>
    /// Gets value indicating whether format was modified, compared to parent format.
    /// </summary>
    public bool IsModified
    {
      get
      {
        bool result = false;

        if( HasParent )
        {
          result = IncludeAlignment ||
            IncludeBorder ||
            IncludeFont ||
            IncludeNumberFormat ||
            IncludePatterns ||
            IncludeProtection;
          //ExtendedFormatImpl parent = ParentFormat;
          //result = CompareProperties( parent );
        }

        return result;
      }
    }

    private bool CompareProperties( ExtendedFormatImpl parent )
    {
      /*
    public int      FontIndex;
    public int      XFormatIndex;
    public int      NumberFormatIndex;
    public ExcelPattern      FillPattern;
    public ExcelKnownColors FillBackground;
    public Color FillBackgroundRGB;
    public ExcelKnownColors FillForeground;
    public Color FillForegroundRGB;
    public ExcelHAlign HorizontalAlignment;
    public int      IndentLevel;
    public bool     FormulaHidden;
    public bool     Locked;
    public bool     JustifyLast;
    public string   NumberFormat;
    public string   NumberFormatLocal;
    public INumberFormat NumberFormatSettings;
    public bool     ShrinkToFit;
    public bool     WrapText;
    public ExcelVAlign VerticalAlignment;
    public bool     IncludeAlignment;
    public bool     IncludeBorder;
    public bool     IncludeFont;
    public bool     IncludeNumberFormat;
    public bool     IncludePatterns;
    public bool     IncludeProtection;
    public virtual IFont Font;
    public IBorders Borders;
    public bool     IsFirstSymbolApostrophe;
    public ExcelKnownColors PatternColorIndex;
    public Color PatternColor;
    public ColorObject PatternColorObject;
    public ExcelKnownColors ColorIndex;
    public Color Color;
    public ColorObject ColorObject;*/
      throw new NotImplementedException();
    }
    /// <summary>
    /// List of extended property.
    /// </summary>
    public List<ExtendedProperty> Properties
    {
        get
        {
            return m_xfExt.Properties;
        }
        set
        {
            m_xfExt.Properties = value;
            m_xfExt.PropertyCount = (ushort)m_xfExt.Properties.Count;
        }
    }
    #endregion

    #region Properties from ExtendedFormatRecord
    /// <summary>
    /// Text direction, the reading order for far east versions.
    /// </summary>        
    public ExcelReadingOrderType ReadingOrder
    {
      get
      {
        return ( ExcelReadingOrderType )m_extFormat.ReadingOrder;
      }
      set
      {
        m_extFormat.ReadingOrder = ( ushort )value;

        SetChanged();
      }
    }

    /// <summary>
    /// Text rotation angle:
    /// 0 Not rotated
    /// 1-90 1 to 90 degrees counterclockwise
    /// 91-180 1 to 90 degrees clockwise
    /// 255 Letters are stacked top-to-bottom, but not rotated.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when value is more than 0xFF.
    /// </exception>
    public int     Rotation
    {
      get
      {
        return m_extFormat.Rotation;
      }
      set
      {
        if( value != Rotation )
        {
          IncludeAlignment = true;
          m_extFormat.Rotation = ( ushort )value;

          if( value != 0 )
            m_extFormat.Indent = 0;

          SetChanged();
        }
      }
    }

    /// <summary>
    /// Type of the extended format.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ExtendedFormatRecord.TXFType XFType
    {
      get
      {
        return m_extFormat.XFType;
      }
      set
      {
        m_extFormat.XFType = value;

        SetChanged();
      }
    }
    #endregion

    #region Implementation properties
    /// <summary>
    /// Gets / sets gradient object.
    /// </summary>
    public IGradient Gradient
    {
      get
      {
        return m_gradient;
      }
      set
      {
        m_gradient = ( ShapeFillImpl )value;
      }
    }
    /// <summary>
    /// Get / set index for this record in Workbook.InnerExtFormats collection.
    /// </summary>
    internal int Index
    {
      get
      {
        return m_iXFIndex;
      }
      set
      {
        m_iXFIndex = value;
      }
    }
    /// <summary>
    /// Read-only. Returns ExtendedFormatRecord with 
    /// information about this format.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ExtendedFormatRecord Record
    {
      get
      {
        return m_extFormat;
      }
      protected set
      {
        if( value == null )
          throw new ArgumentNullException();

        m_extFormat = value;
      }
    }
    /// <summary>
    /// Read-only. Returns ExtendedXFormat with information about this format.
    /// </summary>
    [CLSCompliant(false)]
    public ExtendedXFRecord XFRecord
    {
        get
        {
            return m_xfExt;
        }
        protected set
        {
            if (value == null)
                throw new ArgumentNullException();

            m_xfExt = value;
        }
    }
    /// <summary>
    /// Index of the parent extended format.
    /// </summary>
    internal int ParentIndex
    {
      get
      {
        return m_extFormat.ParentIndex;
      }
      set
      {
        m_extFormat.ParentIndex = ( ushort )value;
      }
    }
    /// <summary>
    /// Returns parent workbook.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Returns parent collection. Read-only.
    /// </summary>
    internal protected ExtendedFormatsCollection ParentCollection
    {
      get
      {
        return m_book.InnerExtFormats;
      }
    }
    /// <summary>
    /// Get/set BottomBorder color.
    /// </summary>
    public ColorObject  BottomBorderColor
    {
      get
      {
        ExtendedFormatImpl format = IncludeBorder
          ? this
          : ParentFormat;

        return format.InnerBottomBorderColor;
      }
    }
    /// <summary>
    /// Get/set TopBorder color.
    /// </summary>
    public ColorObject TopBorderColor
    {
      get
      {
        ExtendedFormatImpl format = IncludeBorder
          ? this
          : ParentFormat;

        return format.InnerTopBorderColor;
      }
    }
    /// <summary>
    /// Get/set LeftBorder color.
    /// </summary>
    public ColorObject LeftBorderColor
    {
      get
      {
        ExtendedFormatImpl format = IncludeBorder
          ? this
          : ParentFormat;

        return format.InnerLeftBorderColor;
      }
    }
    /// <summary>
    /// Get/set RightBorder color.
    /// </summary>
    public ColorObject RightBorderColor
    {
      get
      {
        ExtendedFormatImpl format = IncludeBorder
          ? this
          : ParentFormat;

        return format.InnerRightBorderColor;
      }
    }
    /// <summary>
    /// Get/set DiagonalUpBorder color.
    /// </summary>
    public ColorObject DiagonalBorderColor
    {
      get
      {
        ExtendedFormatImpl format = IncludeBorder
          ? this
          : ParentFormat;

        return format.InnerDiagonalBorderColor;
      }
    }

    /// <summary>
    /// Gets / sets line style of the left border.
    /// </summary>
    public ExcelLineStyle LeftBorderLineStyle
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return record.BorderLeft;
      }
      set
      {
        if( LeftBorderLineStyle != value )
        {
          IncludeBorder = true;
          m_extFormat.SetWorkbook(m_book);
          m_extFormat.BorderLeft = value;
          if(value == ExcelLineStyle.None)
              LeftBorderColor.SetIndexed((ExcelKnownColors)BorderImpl.DEF_BADCOLOR_INCREMENT, false);
          LeftBorderColor.Normalize();
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets / sets line style of the right border.
    /// </summary>
    public ExcelLineStyle RightBorderLineStyle
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return record.BorderRight;
      }
      set
      {
        if( RightBorderLineStyle != value )
        {
          IncludeBorder = true;
          m_extFormat.SetWorkbook(m_book);
          m_extFormat.BorderRight = value;
          if (value == ExcelLineStyle.None)
              RightBorderColor.SetIndexed((ExcelKnownColors)BorderImpl.DEF_BADCOLOR_INCREMENT, false);       
          RightBorderColor.Normalize();
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets / sets line style of the top border.
    /// </summary>
    public ExcelLineStyle TopBorderLineStyle
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return record.BorderTop;
      }
      set
      {
        if( TopBorderLineStyle != value )
        {
          IncludeBorder = true;
          m_extFormat.SetWorkbook(m_book);
          m_extFormat.BorderTop = value;
          if (value == ExcelLineStyle.None)
              TopBorderColor.SetIndexed((ExcelKnownColors)BorderImpl.DEF_BADCOLOR_INCREMENT, false);       
          TopBorderColor.Normalize();
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets / sets line style of the bottom border.
    /// </summary>
    public ExcelLineStyle BottomBorderLineStyle
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return record.BorderBottom;
      }
      set
      {
        if( BottomBorderLineStyle != value )
        {
          IncludeBorder = true;
          m_extFormat.SetWorkbook(m_book);
          m_extFormat.BorderBottom = value;
          if (value == ExcelLineStyle.None)
              BottomBorderColor.SetIndexed((ExcelKnownColors)BorderImpl.DEF_BADCOLOR_INCREMENT, false);       
          BottomBorderColor.Normalize();
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets / sets line style of the diagonal border.
    /// </summary>
    public ExcelLineStyle DiagonalUpBorderLineStyle
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return ( ExcelLineStyle )record.DiagonalLineStyle;
      }
      set
      {
        if( DiagonalUpBorderLineStyle != value )
        {
          IncludeBorder = true;
          m_extFormat.DiagonalLineStyle = ( ushort )value;
          if (value == ExcelLineStyle.None)
              DiagonalBorderColor.SetIndexed((ExcelKnownColors)BorderImpl.DEF_BADCOLOR_INCREMENT, true);       
          DiagonalBorderColor.Normalize();
          SetChanged();
        }

        if( !m_extFormat.DiagonalFromBottomLeft )
        {
          IncludeBorder = true;
          m_extFormat.DiagonalFromBottomLeft = true;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Gets / sets line style of the diagonal border.
    /// </summary>
    public ExcelLineStyle DiagonalDownBorderLineStyle
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return ( ExcelLineStyle )m_extFormat.DiagonalLineStyle;
      }
      set
      {
        if( DiagonalDownBorderLineStyle != value )
        {
          IncludeBorder = true;
          m_extFormat.DiagonalLineStyle = ( ushort )value;
          if (value == ExcelLineStyle.None)
              DiagonalBorderColor.SetIndexed((ExcelKnownColors)BorderImpl.DEF_BADCOLOR_INCREMENT, true);       
          DiagonalBorderColor.Normalize();
          SetChanged();
        }

        if( !m_extFormat.DiagonalFromTopLeft )
        {
          IncludeBorder = true;
          m_extFormat.DiagonalFromTopLeft = true;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Indicates whether DiagonalUp line is visible.
    /// </summary>
    public bool DiagonalUpVisible
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return record.DiagonalFromBottomLeft;
      }
      set
      {
        if( DiagonalUpVisible != value )
        {
          IncludeBorder = true;
          m_extFormat.DiagonalFromBottomLeft = value;
          SetChanged();
        }
      }
    }

    /// <summary>
    /// Indicates whether DiagonalDown line is visible.
    /// </summary>
    public bool DiagonalDownVisible
    {
      get
      {
        ExtendedFormatRecord record = IncludeBorder
          ? m_extFormat
          : ParentRecord;

        return record.DiagonalFromTopLeft;
      }
      set
      {
        if( DiagonalDownVisible != value )
        {
          IncludeBorder = true;
          m_extFormat.DiagonalFromTopLeft = value;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Indicates whether this format is child format.
    /// </summary>
    public bool HasParent
    {
      get
      {
        return ParentIndex != m_book.MaxXFCount;//DEF_NO_PARENT_INDEX;
      }
    }
    /// <summary>
    /// Indicates whether color is default. Read-only.
    /// </summary>
    public bool IsDefaultColor
    {
      get
      {
        return ( int )ColorIndex == ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX;
      }
    }
    /// <summary>
    /// Indicates whether pattern color is default. Read-only.
    /// </summary>
    public bool IsDefaultPatternColor
    {
      get
      {
        return ( int )PatternColorIndex == ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX;
      }
    }
    /// <summary>
    /// Returns record from parent format if possible, otherwise just returns format record. Read-only.
    /// </summary>
    private ExtendedFormatRecord ParentRecord
    {
      get
      {
        return HasParent
          ? ( ( ExtendedFormatImpl )m_book.GetExtFormat( ParentIndex ) ).Record
          : m_extFormat;
      }
    }
    /// <summary>
    /// Returns parent format if possible, otherwise just returns this format. Read-only.
    /// </summary>
    private ExtendedFormatImpl ParentFormat
    {
      get
      {
        return HasParent
          ? ( ( ExtendedFormatImpl )m_book.GetExtFormat( ParentIndex ) )
          : this;
      }
    }
    /// <summary>
    /// Gets number format object.
    /// </summary>
    public FormatImpl NumberFormatObject
    {
      get
      {
        int iIndex = NumberFormatIndex;
        return m_book.InnerFormats[ iIndex ];
      }
    }
    public bool HasBorder
    {
        get
        {
            return m_hasBorder;
        }
        set
        {
            m_hasBorder = value;
        }
    }
    #endregion

    #region Color objects
    /// <summary>
    /// 
    /// </summary>
    protected ColorObject InnerColor
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected ColorObject InnerPatternColor
    {
      get
      {
        return m_patternColor;
      }
    }
    /// <summary>
    /// Returns color of the top border. Read-only.
    /// </summary>
    protected ColorObject InnerTopBorderColor
    {
      get
      {
        return m_topBorderColor;
      }
    }
    /// <summary>
    /// Returns color of the bottom border. Read-only.
    /// </summary>
    protected ColorObject InnerBottomBorderColor
    {
      get
      {
        return m_bottomBorderColor;
      }
    }
    /// <summary>
    /// Returns color of the left border. Read-only.
    /// </summary>
    protected ColorObject InnerLeftBorderColor
    {
      get
      {
        return m_leftBorderColor;
      }
    }
    /// <summary>
    /// Returns color of the right border. Read-only.
    /// </summary>
    protected ColorObject InnerRightBorderColor
    {
      get
      {
        return m_rightBorderColor;
      }
    }
    /// <summary>
    /// Returns color of the diagonal border. Read-only.
    /// </summary>
    protected ColorObject InnerDiagonalBorderColor
    {
      get
      {
        return m_diagonalBorderColor;
      }
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// This method should be called after any changes.
    /// Sets Saved property of the parent workbook to False.
    /// </summary>
    internal void SetChanged()
    {
      m_book.Saved = false;
    }
    /// <summary>
    /// Copies all data from this XF into another.
    /// </summary>
    /// <param name="twin">Format to copy into.</param>
    public void CopyTo( ExtendedFormatImpl twin )
    {
      if( twin == null )
        throw new ArgumentNullException( "twin" );

      twin.m_book = m_book;
      m_extFormat.CopyTo( twin.m_extFormat );
      m_xfExt.CopyTo(twin.m_xfExt);
    }
    /// <summary>
    /// Creates child format if this format is for named style and registers it in workbook.
    /// </summary>
    /// <returns>Child format.</returns>
    public ExtendedFormatImpl CreateChildFormat()
    {
      return CreateChildFormat( true );
    }
    /// <summary>
    /// Creates child format if this format is for named style.
    /// </summary>
    /// <param name="bRegister">Defines whether to register format in workbook.</param>
    /// <returns>Child format.</returns>
    public ExtendedFormatImpl CreateChildFormat( bool bRegister )
    {
      ExtendedFormatImpl format = this;

      if( format.Record.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
      {
        format = m_book.CreateExtFormatWithoutRegister( format );
        format.Record.XFType = ExtendedFormatRecord.TXFType.XF_STYLE;
        format.ParentIndex = Index;

        if( bRegister )
          format = m_book.RegisterExtFormat( format );
      }

      return format;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldFormat"></param>
    /// <returns></returns>
    public ExtendedFormatImpl CreateChildFormat( ExtendedFormatImpl oldFormat )
    {
      ExtendedFormatRecord.TXFType xfType = Record.XFType;
      ExtendedFormatImpl format = CreateChildFormat();

      if( xfType == ExtendedFormatRecord.TXFType.XF_CELL )
      {
        // Here we have to check whether parent style contains some settings disabled.
        ExtendedFormatImpl format2 = ( ExtendedFormatImpl )format.Clone();
        bool bUseSecond = false;

        if( !IncludeAlignment )
        {
          // Here we have to copy alignment settings from old format.
          CopyAlignment( oldFormat, format2, false );
          format2.IncludeAlignment = true;
          bUseSecond = true;
        }

        if( !IncludeBorder )
        {
          // Here we have to copy border settings from old format.
          CopyBorders( oldFormat, format2, false );
          format2.IncludeBorder = true;
          bUseSecond = true;
        }

        if( !IncludeFont )
        {
          // Here we have to copy font settings from old format.
          CopyFont( oldFormat, format2, false );
          format2.IncludeFont = true;
          bUseSecond = true;
        }

        if( !IncludeNumberFormat )
        {
          // Here we have to copy number format settings from old format.
          CopyFormat( oldFormat, format2, false );
          format2.IncludeNumberFormat = true;
          bUseSecond = true;
        }

        if( !IncludePatterns )
        {
          // Here we have to copy pattern settings from old format.
          CopyPatterns( oldFormat, format2, false );
          format2.IncludePatterns = true;
          bUseSecond = true;
        }

        if( !IncludeProtection )
        {
          // Here we have to copy protection settings from old format.
          CopyProtection( oldFormat, format2, false );
          format2.IncludeProtection = true;
          bUseSecond = true;
        }

        if( bUseSecond )
        {
          format = m_book.InnerExtFormats.Add( format2 );
        }
      }

      return format;
    }
    /// <summary>
    /// Synchronizes properties with parent record.
    /// </summary>
    public void SynchronizeWithParent()
    {
      ExtendedFormatRecord parentRecord = ParentRecord;

      if( !IncludeAlignment )
      {
        m_extFormat.CopyAlignment( parentRecord );
      }

      if( !IncludeBorder )
      {
        CopyBorders( ParentFormat );
      }

      if( !IncludeFont )
      {
        m_extFormat.FontIndex = parentRecord.FontIndex;
      }

      if( !IncludeNumberFormat )
      {
        m_extFormat.FormatIndex = parentRecord.FormatIndex;
      }

      if( !IncludePatterns )
      {
        CopyPatterns( ParentFormat );
      }

      if( !IncludeProtection )
      {
        m_extFormat.CopyProtection( parentRecord );
      }
    }
    /// <summary>
    /// Copies borders from source format.
    /// </summary>
    /// <param name="source">Format to copy border settings from.</param>
    private void CopyBorders( ExtendedFormatImpl source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      m_extFormat.CopyBorders( source.m_extFormat );
      m_topBorderColor.CopyFrom( source.m_topBorderColor, false );
      m_bottomBorderColor.CopyFrom( source.m_bottomBorderColor, false );
      m_leftBorderColor.CopyFrom( source.m_leftBorderColor, false );
      m_rightBorderColor.CopyFrom( source.m_rightBorderColor, false );
      m_diagonalBorderColor.CopyFrom( source.m_diagonalBorderColor, false );
    }
    /// <summary>
    /// Copies patterns from source format.
    /// </summary>
    /// <param name="source">Format to copy pattern settings from.</param>
    private void CopyPatterns( ExtendedFormatImpl source )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      m_extFormat.CopyPatterns( m_extFormat );

      if( m_color == null && m_book.Loading )
        return;

      m_color.CopyFrom( source.m_color, false );
      m_patternColor.CopyFrom( source.m_patternColor, false );
    }
    /// <summary>
    /// Starts updating process.
    /// </summary>
    public void BeginUpdate()
    {
    }
    /// <summary>
    /// Ends updating process.
    /// </summary>
    public void EndUpdate()
    {
    }
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Sets application and parent fields.
    /// </summary>
    /// <param name="application">Application object for the format.</param>
    /// <param name="parent">Parent object for the format.</param>
    public ExtendedFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
      m_extFormat = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
      m_xfExt = (ExtendedXFRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ExtendedXFRecord);
      Parse( m_extFormat,m_xfExt );
    }
    /// <summary>
    /// Recovers ExtendedFormatImpl from the stream.
    /// </summary>
    /// <param name="application">Application object for the format.</param>
    /// <param name="parent">Parent object for the format.</param>
    /// <param name="reader">
    /// Reader that contains ExtendedFormatRecord for
    /// the new extended format.
    /// </param>
    private ExtendedFormatImpl( IApplication application, object parent
      , BiffReader reader )
      : this( application, parent )
    {
      Parse( reader );
    }
    /// <summary>
    /// Recovers ExtendedFormatImpl records from Biff Records array and position
    /// of ExtendedFormatRecord in it.
    /// </summary>
    /// <param name="application">Application object for the XF.</param>
    /// <param name="parent">Parent object for the XF.</param>
    /// <param name="data">Array of Biff Records.</param>
    /// <param name="position">Position of ExtendedFormatRecord.</param>
    [ CLSCompliant( false ) ]
    public ExtendedFormatImpl( IApplication application, object parent
      , BiffRecordRaw[] data, int position )
      : this( application, parent )
    {
      Parse( data, position );
    }
    /// <summary>
    /// Recovers ExtendedFormatImpl records from Biff Records 
    /// List and position of ExtendedFormatRecord in it.
    /// </summary>
    /// <param name="application">Application object for the XF.</param>
    /// <param name="parent">Parent object for the XF.</param>
    /// <param name="data">List of Biff Records.</param>
    /// <param name="position">Position of ExtendedFormatRecord in List.</param>
    public ExtendedFormatImpl( IApplication application, object parent, List<BiffRecordRaw> data
      , int position )
      : this( application, parent )
    {
      Parse( data, position );
    }
    /// <summary>
    /// Gets information about extended format from specified format.
    /// </summary>
    /// <param name="application">Application object for the XF.</param>
    /// <param name="parent">Parent object for the XF.</param>
    /// <param name="format">Format that contains all needed information.</param>
    [CLSCompliant( false )]
    public ExtendedFormatImpl( IApplication application, object parent
      , ExtendedFormatRecord format, ExtendedXFRecord xfExt)
        : this(application, parent, format, xfExt, true)
    {
    }
    /// <summary>
    /// Gets information about extended format from specified format.
    /// </summary>
    /// <param name="application">Application object for the XF.</param>
    /// <param name="parent">Parent object for the XF.</param>
    /// <param name="format">Format that contains all needed information.</param>
    [ CLSCompliant( false ) ]
    public ExtendedFormatImpl( IApplication application, object parent
      , ExtendedFormatRecord format, ExtendedXFRecord xfext, bool bInitializeColors)
      : base( application, parent )
    {
      FindParents();
      Parse(format, xfext, bInitializeColors);
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    #endregion

    #region Class reading methods
    /// <summary>
    /// Recover from the stream.
    /// </summary>
    /// <param name="reader">Reader with XF data.</param>
    [ CLSCompliant( false ) ]
    protected void Parse( BiffReader reader )
    {
    }
    /// <summary>
    /// Recovers ExtendedFormat from array of Biff Records and position of
    /// ExtendedFormatRecord in it.
    /// </summary>
    /// <param name="data">Array of Biff Records.</param>
    /// <param name="position">Position of ExtendedFormatRecord in the array.</param>
    [ CLSCompliant( false ) ]
    protected void Parse( IList<BiffRecordRaw> data, int position )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( position < 0 || position > data.Count - 1 )
        throw new ArgumentOutOfRangeException( "position", "Value cannot be less than 0 and greater than data.Length - 1." );

      Parse((ExtendedFormatRecord)data[position], (ExtendedXFRecord)data[position]);
   }
    /// <summary>
    /// Parses ExtendedFormatRecord
    /// </summary>
    /// <param name="format">Record to parse.</param>
    /// <exception cref="System.ApplicationException">
    /// When font index in format record is larger than size 
    /// of the inner font collection of the parent workbook.
    /// </exception>
    [CLSCompliant( false )]
    protected void Parse(ExtendedFormatRecord format, ExtendedXFRecord xfExt)
    {
        Parse(format, xfExt, true);
    }
    /// <summary>
    /// Parses ExtendedFormatRecord
    /// </summary>
    /// <param name="format">Record to parse.</param>
    /// <exception cref="System.ApplicationException">
    /// When font index in format record is larger than size 
    /// of the inner font collection of the parent workbook.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected void Parse(ExtendedFormatRecord format, ExtendedXFRecord xfExt, bool isInitializeColors)
    {
      m_extFormat = format;
      m_xfExt = xfExt;

      if( m_extFormat.FontIndex > m_book.InnerFonts.Count )
        throw new ApplicationException( "Extended Format record FontIndex field has wrong value" );

      this.Index = ( ushort )m_book.InnerExtFormats.Count;

      //if( !m_book.Loading || !( m_book.Version == ExcelVersion.Excel97to2003 && !IncludePatterns && HasParent ) )
      if( isInitializeColors ||
        !( m_book.Loading && m_book.Version == ExcelVersion.Excel97to2003 && !IncludePatterns && HasParent ) )
      {
        InitializeColors();
        CopyColors(xfExt);
      }
    }
    /// <summary>
    /// Updates values from parent format and initializes colors if necessary.
    /// </summary>
    public void UpdateFromParent()
    {
      ExtendedFormatImpl parent = ParentFormat;

      if( m_book.Version == ExcelVersion.Excel97to2003 && !IncludePatterns && HasParent )
      {
        if( ( ushort )parent.FillBackground != m_extFormat.FillBackground ||
          ( ushort )parent.FillForeground != m_extFormat.FillForeground )
        {
          ushort fore = m_extFormat.FillForeground;
          ushort back = m_extFormat.FillBackground;
          IncludePatterns = true;
          m_extFormat.FillForeground = fore;
          m_extFormat.FillBackground = back;
        }

        InitializeColors();
      }
    }
    /// <summary>
    /// Updates values from current format.
    /// </summary>
    public void UpdateFromCurrentExtendedFormat(ExtendedFormatImpl CurrXF)
    {
        ExtendedFormatImpl extFormat;
        ExtendedFormatRecord rc;
        ExtendedXFRecord xfExt;
        ShapeFillImpl gradient = null;

        if (CurrXF == null)
            throw new ArgumentNullException("CurrentXF");

        extFormat = (ExtendedFormatImpl)CurrXF;
        rc = (ExtendedFormatRecord)extFormat.Record.Clone();
        xfExt = (ExtendedXFRecord)extFormat.XFRecord.CloneObject();

        ExtendedFormatImpl baseFormatImpl = CurrXF;
        CurrXF = new ExtendedFormatImpl(Application, this, rc, xfExt);

        CurrXF.ColorObject.CopyFrom(baseFormatImpl.ColorObject, false);
        CurrXF.PatternColorObject.CopyFrom(baseFormatImpl.PatternColorObject, false);
        CurrXF.Gradient = gradient;
        CurrXF.BottomBorderColor.CopyFrom(baseFormatImpl.BottomBorderColor, false);
        CurrXF.TopBorderColor.CopyFrom(baseFormatImpl.TopBorderColor, false);
        CurrXF.LeftBorderColor.CopyFrom(baseFormatImpl.LeftBorderColor, false);
        CurrXF.RightBorderColor.CopyFrom(baseFormatImpl.RightBorderColor, false);
        CurrXF.DiagonalBorderColor.CopyFrom(baseFormatImpl.DiagonalBorderColor, false);        
        CurrXF.Font.RGBColor = baseFormatImpl.Font.RGBColor;
        CurrXF.IndentLevel = baseFormatImpl.IndentLevel;        
    }
    /// <summary>
    /// Initializes color objects.
    /// </summary>
    protected void InitializeColors()
    {
      m_color = new ColorObject( ( ExcelKnownColors )m_extFormat.FillBackground );
      m_color.AfterChange += UpdateColor;

      m_patternColor = new ColorObject( ( ExcelKnownColors )m_extFormat.FillForeground );
      m_patternColor.AfterChange += UpdatePatternColor;

      m_topBorderColor = new ColorObject( ( ExcelKnownColors )m_extFormat.TopBorderPaletteIndex );
      m_topBorderColor.AfterChange += UpdateTopBorderColor;

      m_bottomBorderColor = new ColorObject( ( ExcelKnownColors )m_extFormat.BottomBorderPaletteIndex );
      m_bottomBorderColor.AfterChange += UpdateBottomBorderColor;

      m_leftBorderColor = new ColorObject( ( ExcelKnownColors )m_extFormat.LeftBorderPaletteIndex );
      m_leftBorderColor.AfterChange += UpdateLeftBorderColor;

      m_rightBorderColor = new ColorObject( ( ExcelKnownColors )m_extFormat.RightBorderPaletteIndex );
      m_rightBorderColor.AfterChange += UpdateRightBorderColor;

      m_diagonalBorderColor = new ColorObject( ( ExcelKnownColors )m_extFormat.DiagonalLineColor );
      m_diagonalBorderColor.AfterChange += UpdateDiagonalBorderColor;
    }
    /// <summary>
    /// Updates color field in the record.
    /// </summary>
    private void UpdateColor()
    {
      m_extFormat.FillBackground = ( ushort )m_color.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates pattern color in the record.
    /// </summary>
    private void UpdatePatternColor()
    {
      m_extFormat.FillForeground = ( ushort )m_patternColor.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates top border color in the record.
    /// </summary>
    private void UpdateTopBorderColor()
    {
      m_extFormat.TopBorderPaletteIndex = ( ushort )m_topBorderColor.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates bottom border color in the record.
    /// </summary>
    private void UpdateBottomBorderColor()
    {
      m_extFormat.BottomBorderPaletteIndex = ( ushort )m_bottomBorderColor.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates left border color in the record.
    /// </summary>
    private void UpdateLeftBorderColor()
    {
      m_extFormat.LeftBorderPaletteIndex = ( ushort )m_leftBorderColor.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates right border color in the record.
    /// </summary>
    private void UpdateRightBorderColor()
    {
      m_extFormat.RightBorderPaletteIndex = ( ushort )m_rightBorderColor.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates diagonal border color in the record.
    /// </summary>
    private void UpdateDiagonalBorderColor()
    {
      m_extFormat.DiagonalLineColor = ( ushort )m_diagonalBorderColor.GetIndexed( m_book );
    }
    #endregion

    #region Save Extended Format as Biff Record
    /// <summary>
    /// Adds format records into records array.
    /// </summary>
    /// <param name="records">Array that will receive format information.</param>
    [ CLSCompliant( false ) ]
    public void Serialize(OffsetArrayList records, uint[] crcCache)
    {
      ExtendedFormatRecord xf = ( ExtendedFormatRecord )m_extFormat.Clone();
      xf.FillBackground = ( ushort )ColorIndex;
      xf.FillForeground = ( ushort )PatternColorIndex;

      CheckAndCorrectFormatRecord( xf );
      records.Add( xf );

      byte[] data = xf.Data;
      m_book.crcValue = m_book.CalculateCRC(m_book.crcValue, data, crcCache);
    }
    /// <summary>
    /// Checks the record to see if everything is proper and corrects
    /// it if necessary.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected void CheckAndCorrectFormatRecord( ExtendedFormatRecord record )
    {
        ExtendedFormatImpl parent = (ExtendedFormatImpl)m_book.GetExtFormat(0);
        ExtendedFormatRecord xf = parent.m_extFormat;
      if( ParentIndex == 0 )
      {
        
        //ExtendedFormatRecord own = m_extFormat;

        if( !record.IsNotParentAlignment )   record.CopyAlignment( xf );
        if( !record.IsNotParentBorder )      record.CopyBorders( xf );
        if( !record.IsNotParentCellOptions ) record.CopyProtection( xf );
        if( !record.IsNotParentFont )        record.FontIndex = xf.FontIndex;
        if( !record.IsNotParentFormat )      record.FormatIndex = xf.FormatIndex;
        if( !record.IsNotParentPattern )     record.CopyPatterns( xf );

//        own.IsNotParentAlignment = ( xf.AlignmentOptions != own.AlignmentOptions );
//        own.IsNotParentBorder = ( xf.BorderOptions != own.BorderOptions );
//        own.IsNotParentCellOptions = ( xf.CellOptions != own.CellOptions );
//        own.IsNotParentFont = ( xf.FontIndex != own.FontIndex );
//        own.IsNotParentFormat = ( xf.FormatIndex != own.FormatIndex );
//        own.IsNotParentPattern = ( xf.AdtlFillPattern != own.AdtlFillPattern );
      }
        // For formats with parent set to 4095, all Includes must be set to False value.
      else if( !HasParent )
      {
        if( m_iXFIndex > 20 ) // Adding default styles
        {
            if (!record.IsNotParentAlignment) record.CopyAlignment(xf);
            if (!record.IsNotParentBorder)
                record.CopyBorders(xf);
            if (!record.IsNotParentCellOptions) record.CopyProtection(xf);
            if (!record.IsNotParentFont) record.FontIndex = xf.FontIndex;
            if (!record.IsNotParentFormat) record.FormatIndex = xf.FormatIndex;
            if (!record.IsNotParentPattern) record.CopyPatterns(xf);
        }
      }
    }
    #endregion

    #region Save Extended XF Format as Biff Record
    /// <summary>
    /// Adds Extended XF format records into records array.
    /// </summary>
    /// <param name="records">Array that will receive format information.</param>
    [CLSCompliant(false)]
    public void SerializeXFormat(OffsetArrayList records)
    {
        if (m_xfExt != null)
        {
            ExtendedXFRecord xfExt = (ExtendedXFRecord)m_xfExt.CloneObject();
            records.Add(xfExt);
        }
    }

    /// <summary>
    /// Copy Colors.
    /// </summary>
    private void CopyColors(ExtendedXFRecord xfExt)
    {
        if (xfExt.Properties.Count > 0)
        {
            for (int i = 0; i < xfExt.Properties.Count; i++)
            {

                ExtendedProperty property = xfExt.Properties[i];

#if (SILVERLIGHT || WP)
                System.Windows.Media.Color ColorValue = m_book.ConvertRGBAToARGB(m_book.UIntToColor(property.ColorValue));
#elif ( WINRT )
                Color ColorValue = m_book.ConvertRGBAToARGB(m_book.UIntToColor(property.ColorValue));
#else
                System.Drawing.Color ColorValue = m_book.ConvertRGBAToARGB(m_book.UIntToColor(property.ColorValue));
#endif
                ColorType TypeOfColor = property.ColorType;

             if(TypeOfColor == ColorType.RGB || TypeOfColor == ColorType.Theme || property.Indent > 15)
             {
                 switch (property.Type)
                 {
                     case CellPropertyExtensionType.BackColor:
                         if (TypeOfColor == ColorType.Theme)
                         {
                             property.Tint = property.Tint / MaxTintValue;
                             ColorObject.SetTheme((int)property.ColorValue, Workbook, property.Tint);
                         }
                         else if (FillPattern == ExcelPattern.Solid)
                         {
                             PatternColorObject.ColorType = property.ColorType;
                             PatternColor = ColorValue;
                         }
                         else
                         {
                             ColorObject.ColorType = property.ColorType;
                             Color = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.ForeColor:
                         if (TypeOfColor == ColorType.Theme)
                         {
                             property.Tint = property.Tint / MaxTintValue;
                             ColorObject.SetTheme((int)property.ColorValue, Workbook, property.Tint);
                         }
                         else if (FillPattern == ExcelPattern.Solid)
                         {
                             ColorObject.ColorType = property.ColorType;
                             Color = ColorValue;
                         }
                         else
                         {
                             PatternColorObject.ColorType = property.ColorType;
                             PatternColor = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.TopBorderColor:
                         if (TypeOfColor == ColorType.RGB)
                         {
                             Borders[ExcelBordersIndex.EdgeTop].ColorObject.ColorType = property.ColorType;
                             Borders[ExcelBordersIndex.EdgeTop].ColorRGB = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.BottomBorderColor:
                         if (TypeOfColor == ColorType.RGB)
                         {
                             Borders[ExcelBordersIndex.EdgeBottom].ColorObject.ColorType = property.ColorType;
                             Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.LeftBorderColor:
                         if (TypeOfColor == ColorType.RGB)
                         {
                             Borders[ExcelBordersIndex.EdgeLeft].ColorObject.ColorType = property.ColorType;
                             Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.RightBorderColor:
                         if (TypeOfColor == ColorType.RGB)
                         {
                             Borders[ExcelBordersIndex.EdgeRight].ColorObject.ColorType = property.ColorType;
                             Borders[ExcelBordersIndex.EdgeRight].ColorRGB = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.DiagonalCellBorder:
                         if (TypeOfColor == ColorType.RGB)
                         {
                             Borders[ExcelBordersIndex.DiagonalUp].ColorObject.ColorType = property.ColorType;
                             Borders[ExcelBordersIndex.DiagonalUp].ColorRGB = ColorValue;
                             Borders[ExcelBordersIndex.DiagonalDown].ColorObject.ColorType = property.ColorType;
                             Borders[ExcelBordersIndex.DiagonalDown].ColorRGB = ColorValue;
                         }
                         break;

                     case CellPropertyExtensionType.TextColor:
                         if (TypeOfColor == ColorType.RGB)
                             Font.RGBColor = ColorValue;
                         break;

                     case CellPropertyExtensionType.FontScheme:
                         break;

                     case CellPropertyExtensionType.TextIndentationLevel:
                         if (TypeOfColor != ColorType.Theme)
                             IndentLevel = property.Indent;
                         break;

                     default:
                         break;
                 }
              }
           }
        }
    }
     
    #endregion

    #region IComparable Members
    /// <summary>
    /// Compares the current instance with another object of the same type.
    /// </summary>
    /// <param name="obj">Object to compare with this instance.</param>
    /// <returns>
    /// Less than zero    - This instance is less than obj. 
    /// Zero              - This instance is equal to obj. 
    /// Greater than zero - This instance is greater than obj. 
    ///</returns>
    public int CompareTo( object obj )
    {
      if( obj is ExtendedFormatImpl == false )
        throw new ArgumentException( "Can only compare types with the same type", "obj" );

      ExtendedFormatImpl twin = ( ExtendedFormatImpl )obj;
      return CompareTo( twin );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="twin"></param>
    /// <returns></returns>
    public int CompareTo( ExtendedFormatImpl twin )
    {
      // Update formats to valid state.
      CheckAndCorrectFormatRecord( m_extFormat );
      twin.CheckAndCorrectFormatRecord( twin.m_extFormat );

      byte[] data1 = m_extFormat.Data;
      byte[] data2 = twin.m_extFormat.Data;

      int retValue;

      for( int i = 0, len = Math.Min( data1.Length, data2.Length ); i < len; i++ )
      {
        if( (retValue = data1[ i ].CompareTo( data2[ i ] )) != 0 ) return retValue;
      }

      return data1.Length - data2.Length;
    }
    /// <summary>
    /// Compares formats without comparing indexes.
    /// </summary>
    /// <param name="twin">Format to compare.</param>
    /// <returns>0 if formats are equal.</returns>
    public int CompareToWithoutIndex( ExtendedFormatImpl twin )
    {
      int iCompare = 1;

      if( m_gradient != null )
      {
        iCompare = m_gradient.CompareTo( twin.Gradient );
      }
      else
      {
        iCompare = ( twin.Gradient == null ) ? 0 : 1;
      }

      if( iCompare == 0 )
      {
        iCompare = ( m_color == twin.m_color && m_patternColor == twin.m_patternColor ) ?
          0 :
          1;
      }

      return ( iCompare == 0 && m_extFormat.CompareTo( twin.m_extFormat ) == 0 ) ? 0 : 1;
    }

    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use
    /// in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      return ( m_gradient != null ) ? m_extFormat.GetHashCode() ^ m_gradient.GetHashCode() : m_extFormat.GetHashCode();
    }

    /// <summary>
    /// Determines whether two Object instances are equal.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>
    /// True if the specified Object is equal to the current Object; otherwise, False.
    /// </returns>
    public override bool Equals(object obj)
    {
      ExtendedFormatImpl xFormat = obj as ExtendedFormatImpl;

      if( xFormat == null )
        return false;

      return this.CompareToWithoutIndex( xFormat ) == 0;
    }

    #endregion

    #region Class Copy methods
    /// <summary>
    /// Copies one extended format into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static public void CopyFromTo( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      /*if( childFormat.IncludeAlignment )*/ CopyAlignment( childFormat, parentFormat, bSetFlag );
      /*if( childFormat.IncludeBorder )*/    CopyBorders( childFormat, parentFormat, bSetFlag );
      /*if( childFormat.IncludeFont )*/      CopyFont( childFormat, parentFormat, bSetFlag );
      /*if( childFormat.IncludeNumberFormat )*/    CopyFormat( childFormat, parentFormat, bSetFlag );
      /*if( childFormat.IncludePatterns )*/  CopyPatterns( childFormat, parentFormat, bSetFlag );
      /*if( childFormat.IncludeProtection)*/ CopyProtection( childFormat, parentFormat, bSetFlag );

    }
    /// <summary>
    /// Copies alignment of one XFormat into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static private void CopyAlignment( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      parentFormat.VerticalAlignment = childFormat.VerticalAlignment;
      parentFormat.HorizontalAlignment = childFormat.HorizontalAlignment;
      parentFormat.WrapText = childFormat.WrapText;
      parentFormat.IndentLevel = childFormat.IndentLevel;
      parentFormat.Rotation = childFormat.Rotation;
      parentFormat.ReadingOrder = childFormat.ReadingOrder;

      if( bSetFlag )
        childFormat.IncludeAlignment = false;
    }
    /// <summary>
    /// Copies borders of one XFormat into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static private void CopyBorders( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      parentFormat.Record.BorderBottom  = childFormat.Record.BorderBottom;
      parentFormat.Record.BorderLeft    = childFormat.Record.BorderLeft;
      parentFormat.Record.BorderRight   = childFormat.Record.BorderRight;
      parentFormat.Record.BorderTop     = childFormat.Record.BorderTop;
      
      parentFormat.Record.DiagonalFromBottomLeft = childFormat.Record.DiagonalFromBottomLeft;
      parentFormat.Record.DiagonalFromTopLeft = childFormat.Record.DiagonalFromTopLeft;
      parentFormat.Record.DiagonalLineStyle = childFormat.Record.DiagonalLineStyle;

      parentFormat.TopBorderColor.CopyFrom( childFormat.TopBorderColor, true );
      parentFormat.BottomBorderColor.CopyFrom( childFormat.BottomBorderColor, true );
      parentFormat.LeftBorderColor.CopyFrom( childFormat.LeftBorderColor, true );
      parentFormat.RightBorderColor.CopyFrom( childFormat.RightBorderColor, true );
      parentFormat.DiagonalBorderColor.CopyFrom( childFormat.DiagonalBorderColor, true );

      if( bSetFlag )
        childFormat.IncludeBorder = false;
    }
    /// <summary>
    /// Copies font of one XFormat into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static private void CopyFont( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      parentFormat.FontIndex = childFormat.FontIndex;

      if( bSetFlag )
        childFormat.IncludeFont = false;
    }
    /// <summary>
    /// Copies number format of one XFormat into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static private void CopyFormat( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      parentFormat.NumberFormat = childFormat.NumberFormat;

      if( bSetFlag )
        childFormat.IncludeNumberFormat = false;
    }
    /// <summary>
    /// Copies patterns of one XFormat into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static private void CopyPatterns( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      parentFormat.ColorObject.CopyFrom( childFormat.ColorObject, true );
      parentFormat.PatternColorObject.CopyFrom( childFormat.PatternColorObject, true );
      parentFormat.FillPattern     = childFormat.FillPattern;

      if( bSetFlag )
        childFormat.IncludePatterns = false;
    }
    /// <summary>
    /// Copies protection of one XFormat into another.
    /// </summary>
    /// <param name="childFormat">Destination format.</param>
    /// <param name="parentFormat">Source format.</param>
    /// <param name="bSetFlag">Indicates whether we should change corresponding
    /// Include... flag(s) after copying data.</param>
    static private void CopyProtection( ExtendedFormatImpl childFormat,
      ExtendedFormatImpl parentFormat, bool bSetFlag )
    {
      parentFormat.FormulaHidden = childFormat.FormulaHidden;
      parentFormat.Locked = childFormat.Locked;

      if( bSetFlag )
        childFormat.IncludeProtection = false;
    }
    /// <summary>
    /// Copies color settings from specified format object.
    /// </summary>
    /// <param name="format"></param>
    protected internal void CopyColorsFrom( ExtendedFormatImpl format )
    {
      m_color.CopyFrom( format.m_color, false );
      m_patternColor.CopyFrom( format.m_patternColor, false );
      m_topBorderColor.CopyFrom( format.m_topBorderColor, false );
      m_bottomBorderColor.CopyFrom( format.m_bottomBorderColor, false );
      m_leftBorderColor.CopyFrom( format.m_leftBorderColor, false );
      m_rightBorderColor.CopyFrom( format.m_rightBorderColor, false );
      m_diagonalBorderColor.CopyFrom( format.m_diagonalBorderColor, false );
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of the current instance.</returns>
    public object Clone()
    {
      return TypedClone( this );
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of the current instance.</returns>
    public ExtendedFormatImpl TypedClone( object parent )
    {
      ExtendedFormatImpl format = this.MemberwiseClone() as ExtendedFormatImpl;
      format.m_extFormat = m_extFormat.Clone() as ExtendedFormatRecord;
      format.m_xfExt = m_xfExt.CloneObject() as ExtendedXFRecord;

      format.Index = ushort.MaxValue;

      if( parent != format.Parent )
      {
        format.SetParent( parent );
        format.FindParents();
      }

      if( format.m_gradient != null )
      {
        format.m_gradient = m_gradient.Clone( format ) as ShapeFillImpl;
      }

      if( ParentIndex == m_book.MaxXFCount )
      {
        format.ParentIndex = format.m_book.MaxXFCount;
      }

      format.InitializeColors();
      format.m_color.CopyFrom( m_color, false );
      format.m_patternColor.CopyFrom( m_patternColor, false );
      format.m_topBorderColor.CopyFrom( m_topBorderColor, false );
      format.m_bottomBorderColor.CopyFrom( m_bottomBorderColor, false );
      format.m_leftBorderColor.CopyFrom( m_leftBorderColor, false );
      format.m_rightBorderColor.CopyFrom( m_rightBorderColor, false );
      format.m_diagonalBorderColor.CopyFrom( m_diagonalBorderColor, false );

      return format;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    object Syncfusion.XlsIO.Interfaces.ICloneParent.Clone( object parent )
    {
      return TypedClone( parent );
    }

    #endregion
      
    public void Clear()
    {
        m_gradient = null;
        m_xfExt = null;
        m_extFormat = null;
        m_color = null;
        m_patternColor = null;
        m_topBorderColor = null;
        m_bottomBorderColor = null;
        m_leftBorderColor = null;
        m_rightBorderColor = null;
        m_diagonalBorderColor = null;
        Dispose();
   }

    #region IDisposable Members

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion

    internal void clearAll()
    {
       
        if(m_gradient!=null)m_gradient.Clear();

        if (m_color != null) m_color.Dispose();
        if (m_patternColor != null) m_patternColor.Dispose();
        if (m_topBorderColor != null) m_topBorderColor.Dispose();
        if (m_bottomBorderColor != null) m_bottomBorderColor.Dispose();
        if (m_leftBorderColor != null) m_leftBorderColor.Dispose();
        if (m_rightBorderColor != null) m_rightBorderColor.Dispose();
        if (m_diagonalBorderColor != null) m_diagonalBorderColor.Dispose();

        Clear();

    }
  }
}
