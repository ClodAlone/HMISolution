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

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;

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

#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class wraps extended format inside in order to hide from user
  /// creation of new extended formats when user changes any properties
  /// of extended format.
  /// </summary>
  public class ExtendedFormatWrapper
    : CommonWrapper
    , IInternalExtendedFormat
    , IXFIndex
    , IStyle
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Extended format with style settings.
    /// </summary>
    protected ExtendedFormatImpl m_xFormat;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    protected WorkbookImpl m_book;
    /// <summary>
    /// Font wrapper.
    /// </summary>
    protected FontWrapper m_font;
    /// <summary>
    /// Borders collection.
    /// </summary>
    private BordersCollection m_borders;
    /// <summary>
    /// Interior wrapper.
    /// </summary>
    private InteriorWrapper m_interior;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of extended format wrapper.
    /// </summary>
    public ExtendedFormatWrapper( WorkbookImpl book )
    {
      m_book = book;
    }

    /// <summary>
    /// Creates new instance of extended format wrapper.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="iXFIndex">Index of extended format to wrap.</param>
    public ExtendedFormatWrapper( WorkbookImpl book, int iXFIndex )
      : this( book )
    {
      SetFormatIndex( iXFIndex );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Sets solid fill pattern when changing 
    /// </summary>
    public void ChangeFillPattern()
    {
      ExcelPattern currentPattern = m_xFormat.FillPattern;

      if( currentPattern == ExcelPattern.None || currentPattern == ExcelPattern.Gradient )
      {
        m_xFormat.FillPattern = ExcelPattern.Solid;
        m_xFormat.Gradient = null;
      }
    }
    /// <summary>
    /// Creates inner extended format.
    /// </summary>
    /// <param name="index">Index to extended format to wrap.</param>
    public void SetFormatIndex( int index )
    {
      if( m_xFormat != null && m_xFormat.Index == index ) return;

      // Use 'as' to increase performance.
      m_xFormat = m_book.InnerExtFormats[ index ] as ExtendedFormatImpl;

      if (m_book.InnerFonts.Count <= m_xFormat.FontIndex)
          m_xFormat.FontIndex = 0;

      int iFontIndex = m_xFormat.FontIndex;

      // Use 'as' to increase performance.
      FontImpl font = m_book.InnerFonts[ iFontIndex ] as FontImpl;

      if( m_font == null )
      {
        m_font = new FontWrapper();
        m_font.AfterChangeEvent += new EventHandler( WrappedFontAfterChangeEvent );
      }

      m_font.Wrapped = font;
    }
    /// <summary>
    /// Updates inner extended format's font wrapper.
    /// </summary>
    public void UpdateFont()
    {
      m_font.Wrapped = ( FontImpl )m_xFormat.Font;
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    protected virtual void SetParents( object parent )
    {
      m_book = CommonObject.FindParent( parent, typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Workbook", "Can't find parent workbook" );
    }
    /// <summary>
    /// Sets Saved flag of the parent workbook to the False.
    /// Called when any changes occurred in the style.
    /// </summary>
    protected void SetChanged()
    {
      m_book.SetChanged();
    }
    /// <summary>
    /// Event handler for font AfterChange event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void WrappedFontAfterChangeEvent(object sender, EventArgs e)
    {
      FontIndex = m_font.FontIndex;
    }
    /// <summary>
    /// Event handler for interior AfterChange event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void WrappedInteriorAfterChangeEvent( object sender, EventArgs e )
    {
      BeginUpdate();
      m_xFormat = m_interior.Wrapped;
      EndUpdate();
    }
    /// <summary>
    /// This method is called after changes in NumberFormat.
    /// </summary>
    protected void OnNumberFormatChange()
    {
      if( NumberFormatChanged != null )
      {
        NumberFormatChanged( this, EventArgs.Empty );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public override object Clone( object parent )
    {
      ExtendedFormatWrapper result = ( ExtendedFormatWrapper )base.Clone( parent );
      result.m_book = CommonObject.FindParent( parent, typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( result.m_book == null )
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent workbook." );

      result.m_borders = null;
      result.m_xFormat = null;
      result.m_font = null;

      result.SetFormatIndex( m_xFormat.Index );

      return result;
    }
    /// <summary>
    /// This method is called before reading any value. Can be used
    /// to update wrapped object before read operation.
    /// </summary>
    protected virtual void BeforeRead()
    {
    }
    /// <summary>
    /// Gets style object either from parent or from this instance if it has no parent.
    /// </summary>
    /// <returns>Parent style object.</returns>
    private IStyle GetStyle()
    {
      int xfIndex = ( m_xFormat.HasParent ) ?
        m_xFormat.ParentIndex :
        m_xFormat.XFormatIndex;

      IStyle result = m_book.InnerStyles.GetByXFIndex( xfIndex );
      return result;
    }
    #endregion

    #region Class Protected Properties
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    #endregion

    #region IExtendedFormat Members
    /// <summary>
    /// Gets / Sets fill pattern.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        BeforeRead();
        return m_xFormat.FillPattern;
      }
      set
      {
        if( FillPattern != value )
        {
          BeginUpdate();
          m_xFormat.FillPattern = value;
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.XFormatIndex;
      }
    }
    /// <summary>
    /// Gets / Sets index of fill background color.
    /// </summary>
    public ExcelKnownColors FillBackground
    {
      get
      {
        BeforeRead();
        return m_xFormat.FillBackground;
      }
      set
      {
        if( FillBackground != value )
        {
          BeginUpdate();
          m_xFormat.FillBackground = value;

          ChangeFillPattern();

          EndUpdate();
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
        BeforeRead();
        return m_xFormat.FillBackgroundRGB;
      }
      set
      {
        if( FillBackgroundRGB != value )
        {
          BeginUpdate();
          m_xFormat.FillBackgroundRGB = value;

          ChangeFillPattern();

          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Gets / Sets index of fill foreground color.
    /// </summary>
    public ExcelKnownColors FillForeground
    {
      get
      {
        BeforeRead();
        return m_xFormat.FillForeground;
      }
      set
      {
        if( FillForeground != value )
        {
          BeginUpdate();
          m_xFormat.FillForeground = value;

          ChangeFillPattern();

          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Gets / Sets fill foreground color.
    /// </summary>
    public Color FillForegroundRGB
    {
      get
      {
        BeforeRead();
        return m_xFormat.FillForegroundRGB;
      }
      set
      {
        if( FillForegroundRGB != value )
        {
          BeginUpdate();
          m_xFormat.FillForegroundRGB = value;

          ChangeFillPattern();

          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Gets / Sets format index.
    /// </summary>
    public int NumberFormatIndex
    {
      get
      {
        BeforeRead();
        return m_xFormat.NumberFormatIndex;
      }
      set
      {
        if( NumberFormatIndex != value )
        {
          BeginUpdate();
          m_xFormat.NumberFormatIndex = value;
          EndUpdate();
          OnNumberFormatChange();
        }
      }
    }
    /// <summary>
    /// Horizontal alignment.
    /// </summary>
    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        BeforeRead();
        return m_xFormat.HorizontalAlignment;
      }
      set
      {
        if( HorizontalAlignment != value )
        {
          BeginUpdate();
          m_xFormat.HorizontalAlignment = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the style includes the AddIndent, HorizontalAlignment,
    /// VerticalAlignment, WrapText, and Orientation properties.
    /// Read / write Boolean.
    /// </summary>
    public bool IncludeAlignment
    {
      get
      {
        BeforeRead();
        return m_xFormat.IncludeAlignment;
      }
      set
      {
        if( IncludeAlignment != value )
        {
          BeginUpdate();
          m_xFormat.IncludeAlignment = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the style includes the Color, ColorIndex, LineStyle,
    /// and Weight border properties. Read / write Boolean.
    /// </summary>
    public bool IncludeBorder
    {
      get
      {
        BeforeRead();
        return m_xFormat.IncludeBorder;
      }
      set
      {
        if( IncludeBorder != value )
        {
          BeginUpdate();
          m_xFormat.IncludeBorder = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the style includes the Background, Bold, Color,
    /// ColorIndex, FontStyle, Italic, Name, OutlineFont, Shadow,
    /// Size, Strikethrough, Subscript, Superscript, and Underline
    /// font properties. Read / write Boolean.
    /// </summary>
    public bool IncludeFont
    {
      get
      {
        BeforeRead();
        return m_xFormat.IncludeFont;
      }
      set
      {
        if( IncludeFont != value )
        {
          BeginUpdate();
          m_xFormat.IncludeFont = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the style includes the NumberFormat property.
    /// Read / write Boolean.
    /// </summary>
    public bool IncludeNumberFormat
    {
      get
      {
        BeforeRead();
        return m_xFormat.IncludeNumberFormat;
      }
      set
      {
        if( IncludeNumberFormat != value )
        {
          BeginUpdate();
          m_xFormat.IncludeNumberFormat = value;
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.IncludePatterns;
      }
      set
      {
        if( IncludePatterns != value )
        {
          BeginUpdate();
          m_xFormat.IncludePatterns = value;
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.IncludeProtection;
      }
      set
      {
        if( IncludeProtection != value )
        {
          BeginUpdate();
          m_xFormat.IncludeProtection = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Indent level.
    /// </summary>
    public int IndentLevel
    {
      get
      {
        BeforeRead();
        return m_xFormat.IndentLevel;
      }
      set
      {
        if( IndentLevel != value )
        {
          BeginUpdate();
          m_xFormat.IndentLevel = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if formula is hidden.
    /// </summary>
    public bool FormulaHidden
    {
      get
      {
        BeforeRead();
        return m_xFormat.FormulaHidden;
      }
      set
      {
        if( FormulaHidden != value )
        {
          BeginUpdate();
          m_xFormat.FormulaHidden = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if cell is locked.
    /// </summary>
    public bool Locked
    {
      get
      {
        BeforeRead();
        return m_xFormat.Locked;
      }
      set
      {
        if( Locked != value )
        {
          BeginUpdate();
          m_xFormat.Locked = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// For far east languages. Supported only for format. Always 0 for US.
    /// </summary>
    public bool JustifyLast
    {
      get
      {
        BeforeRead();
        return m_xFormat.JustifyLast;
      }
      set
      {
        if( JustifyLast != value )
        {
          BeginUpdate();
          m_xFormat.JustifyLast = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the format code for the object. Read / write String.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        BeforeRead();
        return m_xFormat.NumberFormat;
      }
      set
      {
        if( NumberFormat != value )
        {
          BeginUpdate();
          m_xFormat.NumberFormat = value;
          EndUpdate();
          OnNumberFormatChange();
        }
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
        BeforeRead();
        return m_xFormat.NumberFormatLocal;
      }
      set
      {
        if( NumberFormatLocal != value )
        {
          BeginUpdate();
          m_xFormat.NumberFormatLocal = value;
          EndUpdate();
          OnNumberFormatChange();
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
        BeforeRead();
        return m_xFormat.NumberFormatSettings;
      }
    }
    /// <summary>
    /// Text direction, the reading order for far east versions.
    /// </summary>
    public Syncfusion.XlsIO.ExcelReadingOrderType ReadingOrder
    {
      get
      {
        BeforeRead();
        return m_xFormat.ReadingOrder;
      }
      set
      {
        if( ReadingOrder != value )
        {
          BeginUpdate();
          m_xFormat.ReadingOrder = value;
          EndUpdate();
        }
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
        BeforeRead();
        return m_xFormat.Rotation;
      }
      set
      {
        if( Rotation != value )
        {
          BeginUpdate();
          m_xFormat.Rotation = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True - shrink content to fit into cell.
    /// </summary>
    public bool ShrinkToFit
    {
      get
      {
        BeforeRead();
        return m_xFormat.ShrinkToFit;
      }
      set
      {
        if( ShrinkToFit != value )
        {
          BeginUpdate();
          m_xFormat.ShrinkToFit = value;
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.VerticalAlignment;
      }
      set
      {
        if( VerticalAlignment != value )
        {
          BeginUpdate();
          m_xFormat.VerticalAlignment = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True - Text is wrapped at right border.
    /// </summary>
    public bool WrapText
    {
      get
      {
        BeforeRead();
        return m_xFormat.WrapText;
      }
      set
      {
        if( WrapText != value )
        {
          BeginUpdate();
          m_xFormat.WrapText = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns font object for this extended format.
    /// </summary>
    public IFont Font
    {
      get
      {
        BeforeRead();
        return m_font;
      }
    }
    /// <summary>
    /// Returns borders object for this extended format.
    /// </summary>
    public IBorders Borders
    {
      get
      {
        BeforeRead();

        if( m_borders == null )
        {
          m_borders = new BordersCollection( Application, this, this );
        }

        return m_borders;
      }
    }
    /// <summary>
    /// If true then first symbol in cell is apostrophe.
    /// </summary>
    public bool     IsFirstSymbolApostrophe
    { 
      get
      {
        BeforeRead();
        return m_xFormat.IsFirstSymbolApostrophe;
      }
      set
      {
        if( IsFirstSymbolApostrophe != value )
        {
          if(!(this.m_book.IsLoaded || this.m_book.Loading))
          BeginUpdate();

          m_xFormat.IsFirstSymbolApostrophe = value;

          if (!(this.m_book.IsLoaded || this.m_book.Loading))
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.PatternColorIndex;
      }
      set
      {
        if( PatternColorIndex != value || FillPattern == ExcelPattern.Gradient )
        {
          BeginUpdate();
          m_xFormat.PatternColorIndex = value;
          ChangeFillPattern();
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the color of the interior pattern as an Color value.
    /// </summary>
    public Color   PatternColor
    {
      get
      {
        BeforeRead();
        return m_xFormat.PatternColor;
      }
      set
      {
        if( PatternColor != value || PatternColorIndex == ( ExcelKnownColors )
          ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX )
        {
          BeginUpdate();
          m_xFormat.PatternColor = value;

          ChangeFillPattern();

          EndUpdate();
        }
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
        BeforeRead();
        return m_xFormat.ColorIndex;
      }
      set
      {
        if( FillPattern == ExcelPattern.Gradient || ColorIndex != value )
        {
          BeginUpdate();
          m_xFormat.ColorIndex = value;

          ChangeFillPattern();

          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the cell shading color.
    /// </summary>
    public Color   Color
    {
      get
      {
        BeforeRead();
        return m_xFormat.Color;
      }
      set
      {
        if( Color != value || ColorIndex == ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX )
        {
          BeginUpdate();
          m_xFormat.Color = value;

          ChangeFillPattern();

          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns interior object for this extended format.
    /// </summary>
    public IInterior Interior
    {
      get
      {
        if( m_interior == null )
        {
          m_interior = new InteriorWrapper( m_xFormat );
          m_interior.AfterChangeEvent += new EventHandler( WrappedInteriorAfterChangeEvent );
        }

        BeforeRead();
        return m_interior;
      }
    }
    /// <summary>
    /// Gets value indicating whether format was modified, compared to parent format.
    /// </summary>
    public bool IsModified
    {
      get
      {
        BeforeRead();
        return m_xFormat.IsModified;
      }
    }
    #endregion

    #region Class Public Properties
    /// <summary>
    /// Gets / Sets font index.
    /// </summary>
    public int FontIndex
    {
      get
      {
        BeforeRead();
        return m_xFormat.FontIndex;
      }
      set
      {
        if( FontIndex != value )
        {
          BeginUpdate();
          m_xFormat.FontIndex = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns wrapped format.
    /// </summary>
    public ExtendedFormatImpl Wrapped
    {
      get
      {
        BeforeRead();
        return m_xFormat;
      }
    }
    public bool HasBorder
    {
        get
        {
            return m_xFormat.HasBorder;
        }
    }
    #endregion

    #region Border properties
    /// <summary>
    /// Get/set BottomBorder color.
    /// </summary>
    public virtual ColorObject BottomBorderColor
    {
      get
      {
        BeforeRead();
        return m_xFormat.BottomBorderColor;
      }
    }
    /// <summary>
    /// Get/set TopBorder color.
    /// </summary>
    public virtual ColorObject TopBorderColor
    {
      get
      {
        BeforeRead();
        return m_xFormat.TopBorderColor;
      }
    }
    /// <summary>
    /// Get/set LeftBorder color.
    /// </summary>
    public virtual ColorObject  LeftBorderColor
    {
      get
      {
        BeforeRead();
        return m_xFormat.LeftBorderColor;
      }
    }
    /// <summary>
    /// Get/set RightBorder color.
    /// </summary>
    public virtual ColorObject  RightBorderColor
    {
      get
      {
        BeforeRead();
        return m_xFormat.RightBorderColor;
      }
    }
    /// <summary>
    /// Get/set DiagonalUpBorder color.
    /// </summary>
    public ColorObject  DiagonalBorderColor
    {
      get
      {
        BeforeRead();
        return m_xFormat.DiagonalBorderColor;
      }
    }

    /// <summary>
    /// Gets / sets line style of the left border.
    /// </summary>
    public virtual ExcelLineStyle LeftBorderLineStyle
    {
      get
      {
        BeforeRead();
        return m_xFormat.LeftBorderLineStyle;
      }
      set
      {
        BeginUpdate();
        m_xFormat.LeftBorderLineStyle = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Gets / sets line style of the right border.
    /// </summary>
    public virtual ExcelLineStyle RightBorderLineStyle
    {
      get
      {
        BeforeRead();
        return m_xFormat.RightBorderLineStyle;
      }
      set
      {
        BeginUpdate();
        m_xFormat.RightBorderLineStyle = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Gets / sets line style of the top border.
    /// </summary>
    public virtual ExcelLineStyle TopBorderLineStyle
    {
      get
      {
        BeforeRead();
        return m_xFormat.TopBorderLineStyle;
      }
      set
      {
        BeginUpdate();
        m_xFormat.TopBorderLineStyle = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Gets / sets line style of the bottom border.
    /// </summary>
    public virtual ExcelLineStyle BottomBorderLineStyle
    {
      get
      {
        BeforeRead();
        return m_xFormat.BottomBorderLineStyle;
      }
      set
      {
        BeginUpdate();
        m_xFormat.BottomBorderLineStyle = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Gets / sets line style of the diagonal border.
    /// </summary>
    public ExcelLineStyle DiagonalUpBorderLineStyle
    {
      get
      {
        BeforeRead();
        return m_xFormat.DiagonalUpBorderLineStyle;
      }
      set
      {
        bool bChanged = false;

        if( DiagonalUpBorderLineStyle != value )
        {
          BeginUpdate();
          m_xFormat.DiagonalUpBorderLineStyle = value;
          bChanged = true;
        }

        if( !m_xFormat.DiagonalUpVisible && value != ExcelLineStyle.None )
        {
          if( !bChanged ) BeginUpdate();

          m_xFormat.DiagonalUpVisible = true;
          bChanged = true;
        }

        if( bChanged )
        {
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.DiagonalDownBorderLineStyle;
      }
      set
      {
        bool bChanged = false;

        if( DiagonalDownBorderLineStyle != value )
        {
          BeginUpdate();
          m_xFormat.DiagonalDownBorderLineStyle = value;
          bChanged = true;
        }

        if( !m_xFormat.DiagonalDownVisible )
        {
          if( !bChanged ) BeginUpdate();

          m_xFormat.DiagonalDownVisible = true;
          bChanged = true;
        }

        if( bChanged )
        {
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.DiagonalUpVisible;
      }
      set
      {
        if( DiagonalUpVisible != value )
        {
          BeginUpdate();
          m_xFormat.DiagonalUpVisible = value;
          EndUpdate();
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
        BeforeRead();
        return m_xFormat.DiagonalDownVisible;
      }
      set
      {
        if( DiagonalDownVisible != value )
        {
          BeginUpdate();
          m_xFormat.DiagonalDownVisible = value;
          EndUpdate();
        }
      }
    }

    #endregion

    #region Class events
    /// <summary>
    /// Event is raised after changes in number format.
    /// </summary>
    public event EventHandler NumberFormatChanged;
    #endregion

    #region Class Properties
    /// <summary>
    /// Reference to Application which hosts all objects. Read-only.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_xFormat.Application;
      }
    }
    /// <summary>
    /// Reference to Parent object. Read-only.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_xFormat.Parent;
      }
    }
    /// <summary>
    /// Indicates whether style is build in. Read-only.
    /// </summary>
    public bool BuiltIn
    {
      get
      {
        IStyle style = GetStyle();
        return style.BuiltIn;
        //throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Returns name of the style. Read-only.
    /// </summary>
    public string Name
    {
      get
      {
        BeforeRead();
        ExtendedFormatRecord record = m_xFormat.Record;
        int iParentIndex = record.ParentIndex;

        StyleImpl style = m_book.InnerStyles.GetByXFIndex( iParentIndex );

        //As MS Excel, If parent style not found, Then default style is parent.
        if (style == null)
        {
            style = m_book.InnerStyles[RangeImpl.DEF_DEFAULT_STYLE] as StyleImpl;
            m_xFormat.ParentIndex = (ushort)style.Index;
        }

        return style.Name;
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
        BeforeRead();
        string strNormalName = m_book.AppImplementation.DefaultStyleNames[0];
        
        return !( StylesCollection.CompareStyles( this, m_book.Styles[ strNormalName ] ) );
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      base.BeginUpdate();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
        SetChanged();
    }
    #endregion

    internal void Dispose()
    {
        this.NumberFormatChanged = null;
        m_xFormat.clearAll();

        m_font.Dispose();
        if (m_borders != null) m_borders.Clear();
        if (m_interior != null) m_interior.Dispose();
    }
  }
}
