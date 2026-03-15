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
using Syncfusion.XlsIO.Interfaces;


#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if (SILVERLIGHT)
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
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for StyleGroup.
	/// </summary>
	public class StyleGroup
    : CommonObject
    , IStyle
    , IXFIndex
	{
    #region Class members
    /// <summary>
    /// Parent range group.
    /// </summary>
    private RangeGroup m_rangeGroup;
    /// <summary>
    /// Font group.
    /// </summary>
    private FontGroup m_font;
    /// <summary>
    /// Borders group.
    /// </summary>
    private BordersGroup m_borders;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    public StyleGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_rangeGroup = FindParent( typeof( RangeGroup ) ) as RangeGroup;

      if( m_rangeGroup == null )
      {
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent range group." );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the group. Read-only.
    /// </summary>
    public IStyle this[ int index ]
    {
      get
      {
        return m_rangeGroup[ index ].CellStyle;
      }
    }
    /// <summary>
    /// Returns number of elements in the group. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_rangeGroup.Count;
      }
    }
    /// <summary>
    /// Returns parent workbook object. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_rangeGroup.Workbook;
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
        if( m_borders == null )
        {
          m_borders = new BordersGroup( Application, this );
        }

        return m_borders;
      }
    }

    /// <summary>
    /// True if the style is a built-in style. Read-only Boolean.
    /// </summary>
    public bool BuiltIn
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].BuiltIn;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].BuiltIn )
            return false;
        }

        return result;
      }
    }

    /// <summary>
    /// Gets / sets fill pattern.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 )
          return ExcelPattern.None;

        ExcelPattern result = this[ 0 ].FillPattern;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FillPattern )
            return ExcelPattern.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FillPattern = value;
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
        int iCount = Count;

        if( iCount == 0 )
          return ExcelKnownColors.None;

        ExcelKnownColors result = this[ 0 ].FillBackground;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FillBackground )
            return ExcelKnownColors.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FillBackground = value;
        }
      }
    }
    /// <summary>
    /// Gets / Sets fill background color.
    /// </summary>
    public Color FillBackgroundRGB
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 )
          return ColorExtension.Empty;

        Color result = this[ 0 ].FillBackgroundRGB;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FillBackgroundRGB )
            return ColorExtension.Empty;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FillBackgroundRGB = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets index of fill foreground color.
    /// </summary>
    public ExcelKnownColors FillForeground
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 )
          return ExcelKnownColors.None;

        ExcelKnownColors result = this[ 0 ].FillForeground;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FillForeground )
            return ExcelKnownColors.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FillForeground = value;
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
        int iCount = Count;

        if( iCount == 0 )
          return ColorExtension.Empty;

        Color result = this[ 0 ].FillForegroundRGB;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FillForegroundRGB )
            return ColorExtension.Empty;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FillForegroundRGB = value;
        }
      }
    }

    /// <summary>
    /// Returns a Font object that represents the font of the specified
    /// object.
    /// </summary>
    public IFont Font
    {
      get
      {
        if( m_font == null )
        {
          m_font = new FontGroup( Application, this );
        }

        return m_font;
      }
    }

    /// <summary>
    /// Returns Interior object that represents interior of the specified object.
    /// </summary>
    public IInterior Interior
    {
      get
      {
        throw new NotImplementedException();
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].FormulaHidden;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FormulaHidden )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FormulaHidden = value;
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
        int iCount = Count;

        if( iCount == 0 ) return ExcelHAlign.HAlignGeneral;

        ExcelHAlign result = this[ 0 ].HorizontalAlignment;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].HorizontalAlignment )
            return ExcelHAlign.HAlignGeneral;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].HorizontalAlignment = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IncludeAlignment;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IncludeAlignment )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IncludeAlignment = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IncludeBorder;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IncludeBorder )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IncludeBorder = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IncludeFont;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IncludeFont )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IncludeFont = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IncludeNumberFormat;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IncludeNumberFormat )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IncludeNumberFormat = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IncludePatterns;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IncludePatterns )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IncludePatterns = value;
        }
      }
    }

    /// <summary>
    /// True if the style includes the FormulaHidden and Locked protection
    /// properties. Read/write Boolean.
    /// </summary>
    public bool IncludeProtection
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IncludeProtection;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IncludeProtection )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IncludeProtection = value;
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
        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        int result = this[ 0 ].IndentLevel;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IndentLevel )
            return int.MinValue;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IndentLevel = value;
        }
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].Locked;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Locked )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Locked = value;
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
        int iCount = Count;

        if( iCount == 0 ) return null;

        string result = this[ 0 ].Name;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Name )
            return null;
        }

        return result;
      }
    }

    /// <summary>
    /// Returns or sets the format code for the object. Read/write String.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return null;

        string result = this[ 0 ].NumberFormat;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].NumberFormat )
            return null;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].NumberFormat = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the format code for the object. Read/write String.
    /// </summary>
    public string NumberFormatLocal
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return null;

        string result = this[ 0 ].NumberFormatLocal;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].NumberFormatLocal )
            return null;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].NumberFormatLocal = value;
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
        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        int result = this[ 0 ].NumberFormatIndex;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].NumberFormatIndex )
            return int.MinValue;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].NumberFormatIndex = value;
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
        int iNumberFormatIndex = NumberFormatIndex;

        if( iNumberFormatIndex < 0 ) return null;

        return ( INumberFormat )Workbook.InnerFormats[ NumberFormatIndex ];
      }
    }
    /// <summary>
    /// Text rotation angle:
    /// 0 Not rotated
    /// 1-90 1 to 90 degrees counterclockwise
    /// 91-180 1 to 90 degrees clockwise
    /// 255 Letters are stacked top-to-bottom, but not rotated.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when value is more than 0xFF.</exception>
    public int Rotation
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        int result = this[ 0 ].Rotation;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Rotation )
            return int.MinValue;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Rotation = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].ShrinkToFit;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].ShrinkToFit )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].ShrinkToFit = value;
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
        int iCount = Count;

        if( iCount == 0 ) return ExcelVAlign.VAlignTop;

        ExcelVAlign result = this[ 0 ].VerticalAlignment;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].VerticalAlignment )
            return ExcelVAlign.VAlignTop;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].VerticalAlignment = value;
        }
      }
    }

    /// <summary>
    /// True if Excel wraps the text in the object.
    /// Read/write Boolean.
    /// </summary>
    public bool WrapText
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].WrapText;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].WrapText )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].WrapText = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IsInitialized;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IsInitialized )
            return false;
        }

        return result;
      }
    }

    /// <summary>
    /// Text direction, the reading order for far east versions.
    /// </summary>
    public ExcelReadingOrderType ReadingOrder
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ExcelReadingOrderType.Context;

        ExcelReadingOrderType result = this[ 0 ].ReadingOrder;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].ReadingOrder )
            return ExcelReadingOrderType.Context;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].ReadingOrder = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IsFirstSymbolApostrophe;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IsFirstSymbolApostrophe )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].IsFirstSymbolApostrophe = value;
        }
      }
    }

    /// <summary>
    /// For far east languages. Supported only for format. Always False for US.
    /// </summary>
    public bool    JustifyLast
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].JustifyLast;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].JustifyLast )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].JustifyLast = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets index of fill background color.
    /// </summary>
    public ExcelKnownColors PatternColorIndex
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ExcelKnownColors.None;

        ExcelKnownColors result = this[ 0 ].PatternColorIndex;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].PatternColorIndex )
            return ExcelKnownColors.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].PatternColorIndex = value;
        }
      }
    }

    /// <summary>
    /// Gets / Sets fill background color.
    /// </summary>
    public Color PatternColor
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ColorExtension.Empty;

        Color result = this[ 0 ].PatternColor;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].PatternColor )
            return ColorExtension.Empty;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].PatternColor = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets index of fill foreground color.
    /// </summary>
    public ExcelKnownColors ColorIndex
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ExcelKnownColors.None;

        ExcelKnownColors result = this[ 0 ].ColorIndex;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].ColorIndex )
            return ExcelKnownColors.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].ColorIndex = value;
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
        int iCount = Count;

        if( iCount == 0 ) return ColorExtension.Empty;

        Color result = this[ 0 ].Color;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Color )
            return ColorExtension.Empty;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Color = value;
        }
      }
    }

    /// <summary>
    /// Gets value indicating whether format was modified, compared to parent format.
    /// </summary>
    public bool IsModified
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].IsModified;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].IsModified )
            return false;
        }

        return result;
      }
    }

    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public virtual void BeginUpdate()
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        this[ i ].BeginUpdate();
      }
    }

    /// <summary>
    /// This method should be called after several updates to the object.
    /// </summary>
    public virtual void EndUpdate()
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        this[ i ].EndUpdate();
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

    #region IXFIndex Members
    /// <summary>
    /// Gets format index in m_book.InnerFormats.
    /// </summary>
    public int XFormatIndex
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        int result = ( ( IXFIndex )this[ 0 ] ).XFormatIndex;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != ( ( IXFIndex )this[ i ] ).XFormatIndex )
            return int.MinValue;
        }

        return result;
      }
    }

    #endregion
  }
}
