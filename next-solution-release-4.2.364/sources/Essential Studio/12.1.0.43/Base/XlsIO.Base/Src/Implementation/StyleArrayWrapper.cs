#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Style wrapper for multicell range. Redirects all 
  /// properties and method calls to the range styles.
  /// </summary>
  public class StyleArrayWrapper
    : CommonObject
    , IExtendedFormat
    , IStyle
    , IXFIndex
  {
    #region Class members
    /// <summary>
    /// Array that contains all cells.
    /// </summary>
    private List<IRange> m_arrRanges = new List<IRange>();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Application.
    /// </summary>
    private IApplication m_application;
    #endregion

    #region Class constructor
    /// <summary>
    /// Creates object that will contain all styles that are used by range.
    /// </summary>
    /// <param name="range">Range from which styles must be taken.</param>
    public StyleArrayWrapper( IRange range )
      : base( range.Application, range )
    {
      m_arrRanges.AddRange( range.Cells );

      IWorksheet sheet = range.Worksheet;
      m_book = sheet.Workbook as WorkbookImpl;
      m_application = range.Application;
    }
    /// <summary>
    /// Creates object that will contain all styles that are used by range.
    /// </summary>
    /// <param name="range">Range from which styles must be taken.</param>
    public StyleArrayWrapper(IApplication application,List<IRange> LstRange,IWorksheet worksheet)
        : base(application, LstRange[0])
    {
        m_arrRanges = LstRange;
        IWorksheet sheet = worksheet;
        m_book = sheet.Workbook as WorkbookImpl;
        m_application = application;
    }
    #endregion

    #region IExtendedFormat Members
    /// <summary>
    /// For far east languages. Supported only for format. Always 0 for US.
    /// </summary>
    public bool JustifyLast
    {
      get
      {
        throw new NotImplementedException( "Not implemented property." );
      }
      set
      {
        throw new NotImplementedException( "Not implemented property." );
      }
    }
    /// <summary>
    /// Returns or sets the format code for the object as a string in the
    /// language of the user. Read / write String.
    /// </summary>
    public string NumberFormatLocal
    {
      get
      {
        throw new NotImplementedException( "Not implemented property." );
      }
      set
      {
        throw new NotImplementedException( "Not implemented property." );
      }
    }
    /// <summary>
    /// Gets format index in m_book.InnerFormats.
    /// </summary>
    public int XFormatIndex
    {
      get
      {
        int len = m_arrRanges.Count;

        if( len > 0 )
        {
          IXFIndex xFormat = ( IXFIndex )m_arrRanges[ 0 ].CellStyle;
          int result = xFormat.XFormatIndex;

          for( int i = 1; i < len; i++ )
          {
            xFormat = ( IXFIndex )m_arrRanges[ i ].CellStyle;

            if( result != xFormat.XFormatIndex )
            {
              return int.MinValue;
            }
          }

          return result;
        }
        else
        {
          return int.MinValue;
        }
      }
    }
    public bool HasBorder
    {
        get
        {
            throw new ArgumentException("No need to implement");
        }
    }
    #endregion

    #region IStyle Members
    /// <summary>
    /// Returns a  Borders collection that represents the borders of a
    /// style or a range of cells (including a range defined as part of
    /// a conditional format).
    /// </summary>
    public IBorders Borders
    {
      get
      {
          RangeImpl impl = (IRange)Parent as RangeImpl;
          if (impl.IsEntireRow || impl.IsEntireColumn)
              return new BordersCollectionArrayWrapper(m_arrRanges,m_application);
          else
              return new BordersCollectionArrayWrapper((IRange)Parent);
      }
    }
    /// <summary>
    /// True if the style is a built-in style. Read-only Boolean.
    /// </summary>
    public bool BuiltIn
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.BuiltIn;
            first = false;
          }
          else if( range.CellStyle.BuiltIn != value )
          {
            return false;
          }
        }

        return value;
      }
    }
    /// <summary>
    /// Gets / sets fill pattern.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        ExcelPattern pattern = ExcelPattern.None;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            pattern = range.CellStyle.FillPattern;
            first = false;
          }
          else if( range.CellStyle.FillPattern != pattern )
          {
            return ExcelPattern.None;
          }
        }

        return pattern;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.FillPattern = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets index of fill background color.
    /// </summary>
    public ExcelKnownColors FillBackground
    {
      get
      {
        ExcelKnownColors color = ExcelKnownColors.None;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            color = range.CellStyle.FillBackground;
            first = false;
          }
          else if( range.CellStyle.FillBackground != color )
          {
            return ExcelKnownColors.None;
          }
        }

        return color;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.FillBackground = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets fill background color.
    /// </summary>
    public Color FillBackgroundRGB
    {
      get
      {
        return m_book.GetPaletteColor( FillBackground );
      }
      set
      {
        FillBackground = m_book.GetNearestColor( value );
      }
    }
    /// <summary>
    /// Gets / sets index of fill foreground color.
    /// </summary>
    public ExcelKnownColors FillForeground
    {
      get
      {
        ExcelKnownColors color = ExcelKnownColors.None;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            color = range.CellStyle.FillForeground;
            first = false;
          }
          else if( range.CellStyle.FillForeground != color )
          {
            return ExcelKnownColors.None;
          }
        }

        return color;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.FillForeground = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets fill foreground color.
    /// </summary>
    public Color FillForegroundRGB
    {
      get
      {
        return m_book.GetPaletteColor( FillForeground );
      }
      set
      {
        FillForeground = m_book.GetNearestColor( value );
      }
    }

    /// <summary>
    /// Returns a Font object that represents the font of the specified 
    /// object. Read-only.
    /// </summary>
    public IFont Font
    {
      get
      {
        IFont font = null;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          if( first )
          {
            font = range.CellStyle.Font;
            first = false;
          }
          else if( range.CellStyle.Font != font )
          {
              RangeImpl impl = (IRange)Parent as RangeImpl;

              if (impl != null && (impl.IsEntireRow || impl.IsEntireColumn))
                  return new FontArrayWrapper(m_arrRanges, m_application);
              else
                  return new FontArrayWrapper((IRange)Parent);
          }
        }

        return font;
      }
    }
    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected. 
    /// Read/write Boolean.
    /// </summary>
    public bool FormulaHidden
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.FormulaHidden;
            first = false;
          }
          else if( range.CellStyle.FormulaHidden != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.FormulaHidden = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object. 
    /// For all objects, this can be one of the following ExcelHAlign constants. 
    /// Read/write ExcelHAlign.
    /// </summary>
    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        ExcelHAlign align = ExcelHAlign.HAlignGeneral;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            align = range.CellStyle.HorizontalAlignment;
            first = false;
          }
          else if( range.CellStyle.HorizontalAlignment != align )
          {
            return ExcelHAlign.HAlignGeneral;
          }
        }

        return align;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.HorizontalAlignment = value;
        }
      }
    }
    /// <summary>
    /// True if the style includes the AddIndent, HorizontalAlignment, 
    /// VerticalAlignment, WrapText, and Orientation properties. 
    /// Read/write Boolean.
    /// </summary>
    public bool IncludeAlignment
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.IncludeAlignment;
            first = false;
          }
          else if( range.CellStyle.IncludeAlignment != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IncludeAlignment = value;
        }
      }
    }
    /// <summary>
    /// True if the style includes the Color, ColorIndex, LineStyle, 
    /// and Weight border properties. Read/write Boolean.
    /// </summary>
    public bool IncludeBorder
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.IncludeBorder;
            first = false;
          }
          else if( range.CellStyle.IncludeBorder != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IncludeBorder = value;
        }
      }
    }
    /// <summary>
    /// True if the style includes the Background, Bold, Color,
    /// ColorIndex, FontStyle, Italic, Name, OutlineFont, Shadow,
    /// Size, Strikethrough, Subscript, Superscript, and Underline
    /// font properties. Read/write Boolean.
    /// </summary>
    public bool IncludeFont
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.IncludeFont;
            first = false;
          }
          else if( range.CellStyle.IncludeFont != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IncludeFont = value;
        }
      }
    }
    /// <summary>
    /// True if the style includes the NumberFormat property. 
    /// Read/write Boolean. 
    /// </summary>
    public bool IncludeNumberFormat
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          if( first )
          {
            value = range.CellStyle.IncludeNumberFormat;
            first = false;
          }
          else if( range.CellStyle.IncludeNumberFormat != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IncludeNumberFormat = value;
        }
      }
    }
    /// <summary>
    /// True if the style includes the Color, ColorIndex, 
    /// InvertIfNegative, Pattern, PatternColor, and PatternColorIndex 
    /// interior properties. Read / write Boolean.
    /// </summary>
    public bool IncludePatterns
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          if( first )
          {
            value = range.CellStyle.IncludePatterns;
            first = false;
          }
          else if( range.CellStyle.IncludePatterns != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IncludePatterns = value;
        }
      }
    }
    /// <summary>
    /// True if the style includes the FormulaHidden and Locked protection 
    /// properties. Read / write Boolean.
    /// </summary>
    public bool IncludeProtection
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.IncludeProtection;
            first = false;
          }
          else if( range.CellStyle.IncludeProtection != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IncludeProtection = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the indent level for the style. Read/write.
    /// </summary>
    public int IndentLevel
    {
      get
      {
        int indent = 0;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            indent = range.CellStyle.IndentLevel;
            first = false;
          }
          else if( range.CellStyle.IndentLevel != indent )
          {
            return 0;
          }
        }

        return indent;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.IndentLevel = value;
        }
      }
    }
    /// <summary>
    /// Indicates whether style is initialized (differs from Normal style).
    /// Read-only.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        bool value = ( ( RangeImpl )m_arrRanges[ 0 ] ).HasStyle;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( range.HasStyle != value )
          {
            return false;
          }
        }

        return value;
      }
    }
    /// <summary>
    /// True if the object is locked, False if the object can be 
    /// modified when the sheet is protected. Read/write Boolean.
    /// </summary>
    public bool Locked
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.Locked;
            first = false;
          }
          else if( range.CellStyle.Locked != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.Locked = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the name of the object. Read-only String.
    /// </summary>
    public string Name
    {
      get
      {
        string name = null;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            name = range.CellStyle.Name;
            first = false;
          }
          else if( range.CellStyle.Name != name )
          {
            return null;
          }
        }

        return name;
      }
    }
    /// <summary>
    /// Returns or sets the format code for the object. Read / write String.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        string format = null;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            format = range.CellStyle.NumberFormat;
            first = false;
          }
          else if( range.CellStyle.NumberFormat != format )
          {
            return null;
          }
        }

        return format;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.NumberFormat = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets index of the number format.
    /// </summary>
    public int NumberFormatIndex
    {
      get
      {
        int format = int.MinValue;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            format = range.CellStyle.NumberFormatIndex;
            first = false;
          }
          else if( range.CellStyle.NumberFormatIndex != format )
          {
            return int.MinValue;
          }
        }

        return format;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.NumberFormatIndex = value;
        }
      }
    }
    /// <summary>
    /// Returns object that describes number format. Read-only.
    /// </summary>
    public INumberFormat NumberFormatSettings
    {
      get
      {
        int iNumberFormat = NumberFormatIndex;

        return ( iNumberFormat > 0 )
          ? m_arrRanges[ 0 ].CellStyle.NumberFormatSettings
          : null;
      }
    }
    /// <summary>
    /// Text rotation angle:
    /// 0 Not rotated
    /// 1-90 1 to 90 degrees counterclockwise
    /// 91-180 1 to 90 degrees clockwise
    /// 255 Letters are stacked top-to-bottom, but not rotated.
    /// int.MinValue - different rotation angle for different ranges.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when value is more than 0xFF.</exception>
    public int Rotation
    {
      get
      {
        if( m_arrRanges.Count == 0 )
          return 0;

        int iRotation = m_arrRanges[ 0 ].CellStyle.Rotation;

        for( int index = 1, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( range.CellStyle.Rotation != iRotation )
          {
            return int.MinValue;
          }
        }

        return iRotation;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.Rotation = value;
        }
      }
    }
    /// <summary>
    /// True if text automatically shrinks to fit in the available 
    /// column width. Read/write Boolean.
    /// </summary>
    public bool ShrinkToFit
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.ShrinkToFit;
            first = false;
          }
          else if( range.CellStyle.ShrinkToFit != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.ShrinkToFit = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the vertical alignment of the specified object. 
    /// Read/write ExcelVAlign.
    /// </summary>
    public ExcelVAlign VerticalAlignment
    {
      get
      {
        ExcelVAlign align = ExcelVAlign.VAlignBottom;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            align = range.CellStyle.VerticalAlignment;
            first = false;
          }
          else if( range.CellStyle.VerticalAlignment != align )
          {
            return ExcelVAlign.VAlignBottom;
          }
        }

        return align;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.VerticalAlignment = value;
        }
      }
    }

    /// <summary>
    /// True if Microsoft Excel wraps the text in the object. 
    /// Read/write Boolean.
    /// </summary>
    public bool WrapText
    {
      get
      {
        bool value = false;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            value = range.CellStyle.WrapText;
            first = false;
          }
          else if( range.CellStyle.WrapText != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        List<int> existedrow = new List<int>();
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.WrapText = value;
          int toAutofit = range.Row;
          IWorksheet sheet = range.Worksheet;
          if (!existedrow.Contains(toAutofit))
          {
              sheet.AutofitRow(toAutofit);
              RowStorage row = WorksheetHelper.GetOrCreateRow(sheet as IInternalWorksheet, toAutofit - 1, false);
              if (!row.IsBadFontHeight && !(sheet.Workbook as WorkbookImpl).Loading)
                  sheet.AutofitRow(toAutofit);
              existedrow.Add(toAutofit);
          }
        }
      }
    }

    /// <summary>
    /// Text direction, the reading order for far east versions.
    /// </summary>
    public ExcelReadingOrderType ReadingOrder
    {
      get
      {
        if( m_arrRanges == null )
          throw new ApplicationException( "Blank collection" );

        List<IRange> cells = m_arrRanges;

        ExcelReadingOrderType order = cells[ 0 ].CellStyle.ReadingOrder;

        if( order == ExcelReadingOrderType.Context )
          return ExcelReadingOrderType.Context;

        for( int i = 1, cellCount = cells.Count; i < cellCount; i++ )
        {
          IRange cell = cells[ i ];

          if( order != cell.CellStyle.ReadingOrder )
          {
            return ExcelReadingOrderType.Context;
          }
        }

        return order;
      }
      set
      {
        if( m_arrRanges == null )
          throw new ApplicationException( "Blank collection" );

        for( int i = 0, cellCount = m_arrRanges.Count; i < cellCount; i++ )
        {
          IRange cell = m_arrRanges[ i ];
          cell.CellStyle.ReadingOrder = value;
        }
      }
    }
    /// <summary>
    /// If true then first symbol in cell is apostrophe.
    /// </summary>
    public bool IsFirstSymbolApostrophe
    {
      get
      {
        bool result = true;

        for( int i = 0, len = m_arrRanges.Count; i < len && result == false; i++ )
        {
          result = m_arrRanges[ i ].CellStyle.IsFirstSymbolApostrophe;
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_arrRanges.Count; i < len; i++ )
        {
          m_arrRanges[ i ].CellStyle.IsFirstSymbolApostrophe = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an index into the current color palette.
    /// </summary>
    public ExcelKnownColors PatternColorIndex
    {
      get
      {
        ExcelKnownColors color = ExcelKnownColors.None;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            color = range.CellStyle.PatternColorIndex;
            first = false;
          }
          else if( range.CellStyle.PatternColorIndex != color )
          {
            return ExcelKnownColors.None;
          }
        }

        return color;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.PatternColorIndex = value;
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
        Color color = ColorExtension.Empty;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            color = range.CellStyle.PatternColor;
            first = false;
          }
          else if( range.CellStyle.PatternColor.ToArgb() != color.ToArgb() )
          {
            return ColorExtension.Empty;
          }
        }

        return color;
        //return m_book.GetPaletteColor( PatternColorIndex );
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.PatternColor = value;
        }
        //PatternColorIndex = m_book.GetNearestColor( value );
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
        ExcelKnownColors color = ExcelKnownColors.None;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            color = range.CellStyle.ColorIndex;
            first = false;
          }
          else if( range.CellStyle.ColorIndex != color )
          {
            return ExcelKnownColors.None;
          }
        }

        return color;
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.ColorIndex = value;
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
        Color color = ColorExtension.Empty;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];

          if( first )
          {
            color = range.CellStyle.Color;
            first = false;
          }
          else if( range.CellStyle.Color.ToArgb() != color.ToArgb() )
          {
            return ColorExtension.Empty;
          }
        }

        return color;
        //return m_book.GetPaletteColor( ColorIndex );
      }
      set
      {
        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          range.CellStyle.Color = value;
        }
        //ColorIndex = m_book.GetNearestColor( value );
      }
    }
    /// <summary>
    /// Returns Interior object that represents the interior of the specified 
    /// object. Read-only.
    /// </summary>
    public IInterior Interior
    {
      get
      {
        IInterior interior = null;
        bool first = true;

        for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
        {
          IRange range = m_arrRanges[ index ];
          if( first )
          {
            interior = range.CellStyle.Interior;
            first = false;
          }
          else if( range.CellStyle.Interior != interior )
          {
            return new InteriorArrayWrapper( ( IRange )Parent );
          }
        }

        return interior;
      }
    }
    /// <summary>
    /// Gets value indicating whether format was modified, compared to parent format.
    /// </summary>
    public bool IsModified
    {
      get
      {
        bool result = true;

        for( int i = 0, len = m_arrRanges.Count; i < len && result == false; i++ )
        {
          result = m_arrRanges[ i ].CellStyle.IsModified;
        }

        return result;
      }
    }
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public virtual void BeginUpdate()
    {
      for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
      {
        IRange range = m_arrRanges[ index ];
        range.CellStyle.BeginUpdate();
      }
    }

    /// <summary>
    /// This method should be called after several updates to the object.
    /// </summary>
    public virtual void EndUpdate()
    {
      for( int index = 0, last = m_arrRanges.Count; index < last; index++ )
      {
        IRange range = m_arrRanges[ index ];
        range.CellStyle.EndUpdate();
      }
    }
    #endregion
  }
}
