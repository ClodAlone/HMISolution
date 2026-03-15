#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.XmlReaders;
#if ( WINRT )
#if WP
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
#endif

#if  SILVERLIGHT
using FontStyleEnum = Syncfusion.XlsIO.Implementation.Silverlight.FontStyle;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using FontStyleEnum = Syncfusion.XlsIO.Implementation.WP.FontStyle;
using Syncfusion.XlsIO.Implementation.WP;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using FontStyleEnum = System.Drawing.FontStyle;
#endif

using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Contains the font attributes (font name, font size,
  /// color, and so on) for an object.
  /// </summary>
  public class FontImpl
    : CommonObject
    , IFont
    , ICloneable
    , IComparable
    , IInternalFont
    , ICloneParent, IDisposable
  {
    #region Class constants
    /// <summary>
    /// Weight of the bold font.
    /// </summary>
    internal const ushort FONTBOLD = 700;
    /// <summary>
    /// Weight of the normal font.
    /// </summary>
    internal const ushort FONTNORMAL = 400;

#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// 
    /// </summary>
    private static readonly CharacterRange[] characterRanges =
    {
      new CharacterRange( 0, 2 ),
      new CharacterRange( 1, 1 )
    };
#endif
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_INCORRECT_INDEX = -1;
    /// <summary>
    /// Font index which is not present in file.
    /// </summary>
    internal const int DEF_BAD_INDEX = 4;
    /// <summary>
    /// Multiplier for small bold font.
    /// </summary>
    private const float DEF_SMALL_BOLD_FONT_MULTIPLIER = 1.15f;
    /// <summary>
    /// Multiplier for bold font.
    /// </summary>
    private const float DEF_BOLD_FONT_MULTIPLIER = 1.07f;
    /// <summary>
    /// Represents last not default color index.
    /// </summary>
    private const int DEF_INDEX = 64;
    #endregion

    #region Class members
    /// <summary>
    /// Wrapped FontRecord.
    /// </summary>
    private FontRecord m_font;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Current position of FontImpl class in the InnerFont Collection.
    /// </summary>
    private int m_index = DEF_INCORRECT_INDEX;
    /// <summary>
    /// Font charset.
    /// </summary>
    private byte m_btCharSet = 1;
    /// <summary>
    /// Native font object.
    /// </summary>
    private Font m_fontNative;
    /// <summary>
    /// Color object.
    /// </summary>
    private ColorObject m_color;
    /// <summary>
    /// Font's language.
    /// </summary>
    private string m_strLanguage;
    private string m_scheme;
    private int m_family;
    private bool m_hasLatin = false;
    private bool m_hasComplexScripts = false;
    private bool m_hasEastAsianFont = false;
    private string m_actualFont=null;
    private string[] m_arrItalicFonts = new string[] { "Brush Script MT" };
    internal TextSettings m_textSettings = null;
    internal bool showFontName = true;
    #endregion

    #region IFont Members
    /// <summary>
    /// True if the font is bold. Read/write Boolean.
    /// </summary>
    public bool   Bold
    {
      get
      {
        return ( m_font.BoldWeight >= FONTBOLD );
      }
      set
      {
          // check whether the font supports the FontStyle 
          if (!value)
          {
              if (m_font.IsItalic)
              {
#if ( WINRT )
                  IsSupportedFontStyle(m_font.FontName, 10, FontStyle.Italic);
#elif (SILVERLIGHT)
                  Font TempFont_1 = new Silverlight.Font(m_font.FontName, 10, Silverlight.FontStyle.Italic, GraphicsUnit.Pixel, CharSet);
#elif WP
                  Font TempFont_1 = new WP.Font(m_font.FontName, 10, WP.FontStyle.Italic, GraphicsUnit.Pixel, CharSet);
#else
                  Font TempFont = new Font(m_font.FontName, 10, FontStyle.Italic);
#endif
              }
              else 
              {
#if ( WINRT )
                  IsSupportedFontStyle(m_font.FontName, 10, FontStyle.Regular); 
#elif (SILVERLIGHT)
                  Font TempFont_1 = new Silverlight.Font(m_font.FontName, 10, Silverlight.FontStyle.Regular, GraphicsUnit.Pixel, CharSet);
#elif WP
                  Font TempFont_1 = new WP.Font(m_font.FontName, 10, WP.FontStyle.Regular, GraphicsUnit.Pixel, CharSet);
#else
                  Font TempFont = new Font(m_font.FontName, 10, FontStyle.Regular);
#endif

              }
          }
        if( value != Bold )
        {
          m_font.BoldWeight = value ? FONTBOLD : FONTNORMAL;

          SetChanged();
        }
      }
    }
#if ( WINRT )
      private Font IsSupportedFontStyle(string fontName,int size,FontStyle style)
      {
           Font tempFont = new Font();
          tempFont.Name=m_font.FontName;
                  tempFont.Size = 10;
                  tempFont.Italic = true;
          return tempFont;
      }
#endif
    /// <summary>
    /// Returns or sets the primary color of the object. Read / write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return m_color.GetIndexed( m_book );
        //return ( ExcelKnownColors )m_font.PaletteColorIndex;
      }
      set
      {
        m_color.SetIndexed( value );
      }
    }
    /// <summary>
    /// Gets / sets font color. Searches for the closest color in 
    /// the workbook palette.
    /// </summary>
#if ( WINRT )
        public Windows.UI.Color RGBColor
#else
    public Color  RGBColor
#endif
    {
      get
      {
        return m_color.GetRGB( m_book );
      }
      set
      {
        m_color.SetRGB( value, m_book );
        //this.Color = ParentWorkbook.GetNearestColor( value, WorkbookImpl.DEF_FIRST_USER_COLOR );
      }
    }
    /// <summary>
    /// True if the font style is italic. Read/write Boolean.
    /// </summary>
    public bool   Italic
    {
      get
      {
        return m_font.IsItalic;
      }
      set
      {
          // check whether the font supports the FontStyle
          if (!value)
          {
#if ( WINRT )
              IsSupportedFontStyle(m_font.FontName, 10, FontStyle.Bold);
#elif (SILVERLIGHT)
              Font TempFont_1 = new Silverlight.Font(m_font.FontName, 10, Silverlight.FontStyle.Bold, GraphicsUnit.Pixel, CharSet);
#elif WP
              Font TempFont_1 = new WP.Font(m_font.FontName, 10, WP.FontStyle.Bold, GraphicsUnit.Pixel, CharSet);
#else
                  Font TempFont = new Font(m_font.FontName, 10, FontStyle.Bold);
#endif
          }
          else
          {
#if ( WINRT )
              IsSupportedFontStyle(m_font.FontName, 10, FontStyle.Regular);
#elif (SILVERLIGHT)
              Silverlight.FontStyle style = GetSupportedFontStyle(m_font.FontName);
              Font TempFont_1 = new Silverlight.Font(m_font.FontName, 10, style, GraphicsUnit.Pixel, CharSet);
#elif WP
              WP.FontStyle style = GetSupportedFontStyle(m_font.FontName);
              Font TempFont_1 = new WP.Font(m_font.FontName, 10, style, GraphicsUnit.Pixel, CharSet);
#else
              FontStyle DefaultFontStyle = GetSupportedFontStyle(m_font.FontName);
              Font TempFont = new Font(m_font.FontName, 10, DefaultFontStyle);
#endif
          }              
        if( m_font.IsItalic != value )
        {
          m_font.IsItalic = value;

          SetChanged();
        }
      }
    }
    /// <summary>
    /// True if the font is an outline font. Read/write Boolean.
    /// </summary>
    public bool   MacOSOutlineFont
    {
      get
      {
        return m_font.IsMacOutline;
      }
      set
      {
        m_font.IsMacOutline = value;

        SetChanged();
      }
    }
    /// <summary>
    /// True if the font is a shadow font or if the object has
    /// a shadow. Read/write Boolean.
    /// </summary>
    public bool   MacOSShadow
    {
      get
      {
        return m_font.IsMacShadow;
      }
      set
      {
        m_font.IsMacShadow = value;

        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the size of the font in points. Read / write Variant.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When size is less than 1 or greater than 409.
    /// </exception>
    public double Size
    {
      get
      {
        return m_font.FontHeight / 20.0;
      }
      set
      {
        if( value < 1 || value > 409 )
        {
          throw new ArgumentOutOfRangeException( "Font.Size", 
            "Font.Size out of range. Size must be less then 409 and greater than 1." );
        }

        ushort newSize = ( ushort )( value * 20 );

        if( m_font.FontHeight != newSize )
        {
          m_font.FontHeight = ( ushort )( value * 20 );
          SetChanged();
        }
      }
    }
    /// <summary>
    /// True if the font is struck through with a horizontal line.
    /// Read/write Boolean.
    /// </summary>
    public bool   Strikethrough
    {
      get
      {
        return m_font.IsStrikeout;
      }
      set
      {
        m_font.IsStrikeout = value;

        SetChanged();
      }
    }
    /// <summary>
    /// True if the font is formatted as subscript.
    /// False by default. Read/write Boolean.
    /// </summary>
    public bool   Subscript
    {
      get
      {
        return ( m_font.SuperSubscript == ExcelFontVertialAlignment.Subscript );
      }
      set
      {
        if( value != Subscript )
        {
          if( value == true )
          {
            m_font.SuperSubscript = ExcelFontVertialAlignment.Subscript;
          }
          else if( m_font.SuperSubscript == ExcelFontVertialAlignment.Subscript )
          {
            m_font.SuperSubscript = ExcelFontVertialAlignment.Baseline;
          }

          SetChanged();
        }
      }
    }
    /// <summary>
    /// True if the font is formatted as superscript; False by default.
    /// Read/write Boolean.
    /// </summary>
    public bool   Superscript
    {
      get
      {
        return ( m_font.SuperSubscript == ExcelFontVertialAlignment.Superscript );
      }
      set
      {
        if( value != Superscript )
        {
          if( value == true )
          {
            m_font.SuperSubscript = ExcelFontVertialAlignment.Superscript;
          }
          else if( m_font.SuperSubscript == ExcelFontVertialAlignment.Superscript )
          {
            m_font.SuperSubscript = ExcelFontVertialAlignment.Baseline;
          }

          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the type of underline applied to the font. Can
    /// be one of the following ExcelUnderlineStyle constants.
    /// Read/write ExcelUnderline.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        return m_font.Underline;
      }
      set
      {
        m_font.Underline = value;

        SetChanged();
      }
    }
    /// <summary>
    /// Returns or sets the font name. Read/write string.
    /// </summary>
    public string FontName
    {
      get
      {
        return m_font.FontName;
      }
      set
      {
        if( value != m_font.FontName )
        {
          m_font.FontName = value;
          SetChanged();
          // find the supported font style
#if !(SILVERLIGHT || WP)
          FontStyle style = GetSupportedFontStyle(m_font.FontName);
          switch (style)
          {
              case FontStyle.Bold:
                  m_font.BoldWeight = FONTBOLD;
                  SetChanged();
                  break;
              case FontStyle.Italic:
                   m_font.IsItalic = true;
                   SetChanged();
                   break;
          }
#elif SILVERLIGHT
          Silverlight.FontStyle style = GetSupportedFontStyle(m_font.FontName);
          switch (style)
          {
              case Silverlight.FontStyle.Bold:
                  m_font.BoldWeight = FONTBOLD;
                  SetChanged();
                  break;
              case Silverlight.FontStyle.Italic:
                  m_font.IsItalic = true;
                  SetChanged();
                  break;
          }
#elif WP
          WP.FontStyle style = GetSupportedFontStyle(m_font.FontName);
          switch (style)
          {
              case WP.FontStyle.Bold:
                  m_font.BoldWeight = FONTBOLD;
                  SetChanged();
                  break;
              case WP.FontStyle.Italic:
                  m_font.IsItalic = true;
                  SetChanged();
                  break;
          }
#endif
        }
      }
    }
    /// <summary>
    /// Gets / sets font vertical alignment.
    /// </summary>
    public ExcelFontVertialAlignment VerticalAlignment
    {
      get
      {
        return m_font.SuperSubscript;
      }
      set
      {
        m_font.SuperSubscript = value;
      }
    }
    /// <summary>
    /// Indicates whether color is automatically selected. Read-only.
    /// </summary>
    public bool IsAutoColor
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Gets or sets the baseline value which indicates whether superscript or subscript
    /// </summary>    
    internal int BaseLine
    {
        get
        {
            return m_font.Baseline;
        }
        set
        {
            m_font.Baseline = value;
        }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns wrapped FontRecord. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public FontRecord Record
    {
      get
      {
        return m_font;
      }
    }
    /// <summary>
    /// Parent workbook. Read-only.
    /// </summary>
    internal WorkbookImpl ParentWorkbook
    {
      get
      {
        if( m_book == null )
        {
          m_book = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );
        }

        return m_book;
      }
    }
    /// <summary>
    /// Font index in the workbook fonts collection.
    /// </summary>
    internal int Index
    {
      get
      {
        return m_index;
      }
      set
      {
        if( m_index != value )
        {
          //ValueChangedEventArgs args = new ValueChangedEventArgs( m_index, value, "Index" );
          //RaiseIndexChangedEvent( args );
          m_index = value;
        }
      }
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Returns workbook graphics. Read-only.
    /// </summary>
    protected Graphics BookGraphics
    {
      get
      {
        return ParentWorkbook.InnerGraphics;
      }
    }
#endif
    /// <summary>
    /// Gets/sets font charset.
    /// </summary>
    public byte CharSet
    {
      get
      {
        return m_font.Charset;
      }
      set
      {
        m_font.Charset = value;
      }
    }
    /// <summary>
    /// Gets or sets the family.
    /// </summary>
    /// <value>The family.</value>
    internal byte Family
    {
        get
        {
            return m_font.Family;
        }
        set
        {
            m_font.Family = value;
        }
    }
    /// <summary>
    /// Returns color object.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// Gets or sets font's language.
    /// </summary>
    public string Language
    {
      get
      {
        return m_strLanguage;
      }
      set
      {
        m_strLanguage = value;
      }
    }
    internal bool HasLatin
    {
        get
        {
            return m_hasLatin;
        }
        set
        {
            m_hasLatin=value;
        }
    }
      internal bool HasComplexScripts
      {
          get
          {
              return m_hasComplexScripts;
          }
          set
          {
              m_hasComplexScripts=value;
          }
      }
      internal bool  HasEastAsianFont
      {
          get
          {
              return m_hasEastAsianFont;
          }
          set
          {
              m_hasEastAsianFont=value;
          }
      }
     
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Creates font and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the font.</param>
    /// <param name="parent">Parent object for the font.</param>
    public FontImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_font = ( FontRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Font );
      m_font.FontName = AppImplementation.StandardFont;
      m_font.FontHeight = (ushort) SizeInTwips( AppImplementation.StandardFontSize );
      InitializeColor();
      InitializeParent();
    }
    /// <summary>
    /// Reads font from the stream.
    /// </summary>
    /// <param name="application">Application object for the font.</param>
    /// <param name="parent">Parent object for the font.</param>
    /// <param name="reader">Reader with font data.</param>
    [ CLSCompliant( false ) ]
    public FontImpl( IApplication application, object parent, BiffReader reader )
      : this( application, parent )
    {
      Parse( reader );
    }
    /// <summary>
    /// Creates FontImpl from FontRecord.
    /// </summary>
    /// <param name="application">Application object for the font.</param>
    /// <param name="parent">Parent object for the font.</param>
    /// <param name="record">Record with font data.</param>
    [ CLSCompliant( false ) ]
    public FontImpl( IApplication application, object parent, FontRecord record )
      : this( application, parent )
    {
      m_font = record;
      UpdateColor();
    }
    /// <summary>
    /// Creates FontImpl from FontRecord.
    /// </summary>
    /// <param name="application">Application object for the font.</param>
    /// <param name="parent">Parent object for the font.</param>
    /// <param name="record">Record with font data.</param>
    [CLSCompliant(false)]
    public FontImpl(IApplication application, object parent, FontImpl font)
        : this(application, parent)
    {
        m_font = font.Record;
        if (font != null)
            m_color = font.ColorObject;
        UpdateColor();
    }
    /// <summary>
    /// Creates font using data from baseFont.
    /// </summary>
    /// <param name="baseFont">IFont that will be copied.</param>
    /// <exception cref="System.ArgumentException">
    /// When baseFont is not FontImpl and not FontImplWrapper.
    /// </exception>
    public FontImpl( IFont baseFont )
      : this( baseFont.Application, baseFont.Parent )
    {
      if( baseFont is FontImpl )
      {
        m_font = ( FontRecord )( ( FontImpl )baseFont).Record.Clone();
      }
      else if( baseFont is FontWrapper )
      {
        m_font = ( FontRecord )( ( FontWrapper )baseFont ).Wrapped.Record.Clone();
      }
      else
        throw new ArgumentException( "baseFont must be FontImpl or FontWrapper class instance" );

      UpdateColor();
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Creates font based on native font and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the font.</param>
    /// <param name="parent">Parent object for the font.</param>
    /// <param name="nativeFont">Native font to create from.</param>
    public FontImpl( IApplication application, object parent, Font nativeFont )
      : this( application, parent )
    {
      Parse( nativeFont );
    }
#endif
    /// <summary>
    /// Initializes color object.
    /// </summary>
    private void InitializeColor()
    {
      m_color = new ColorObject( ( ExcelKnownColors )m_font.PaletteColorIndex );
      m_color.AfterChange += UpdateRecord;
    }
    /// <summary>
    /// Updates font record after color change.
    /// </summary>
    private void UpdateRecord()
    {
      m_font.PaletteColorIndex = ( ushort )m_color.GetIndexed( m_book );
      SetChanged();
    }
    /// <summary>
    /// Updates color with record's data.
    /// </summary>
    private void UpdateColor()
    {
        if (m_color.ColorType == ColorType.RGB || m_color.ColorType == ColorType.Theme)
            m_color.SetRGB( m_color.GetRGB(m_book));
        else
            m_color.SetIndexed( ( ExcelKnownColors )m_font.PaletteColorIndex );
    }
    /// <summary>
    /// Initializes parent objects.
    /// </summary>
    private void InitializeParent()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentException( "Cannot find parent workbook." );
    }
    #endregion

    #region Extract Font Record from Stream
    /// <summary>
    /// Reads font from the BiffReader.
    /// </summary>
    /// <param name="reader">BiffReader that contains font data.</param>
    /// <exception cref="System.ApplicationException">
    /// When reader can't extract records anymore or 
    /// if current record in the reader is not FontRecord.
    /// </exception>
    private void Parse( BiffReader reader )
    {
      if( reader.IsEOF )
        throw new ApplicationException( "Reached end of stream. Font object cannot be initialized." );

      BiffRecordRaw raw = reader.GetRecord();

      if( raw.TypeCode != TBIFFRecord.Font )
        throw new ApplicationException( "Record extracted from stream is not a Font Record" );

      m_font = ( FontRecord )raw;
      UpdateColor();
    }
    #endregion

    #region Save Font as Biff Record
    /// <summary>
    /// Saves all range cells into OffsetArrayList.
    /// </summary>
    /// <param name="records">Array that will receive font record.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      records.Add( m_font );
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// Copies data from this instance to another.
    /// </summary>
    /// <param name="twin">Font impl that will receive data from this font.</param>
    public void CopyTo( FontImpl twin )
    {
      m_font.CopyTo( twin.m_font );
      twin.CharSet = CharSet;
    }

    /// <summary>
    /// This method should be called after any changes.
    /// Sets Saved property of the parent workbook to false.
    /// </summary>
    public void SetChanged()
    {
      ParentWorkbook.Saved = false;
      m_fontNative = null;
    }
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont()
    {
      if( m_fontNative == null )
        m_fontNative = GenerateNativeFont( ( float )Size );

      return m_fontNative;
    }
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <param name="size">Desired font size.</param>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont( float size )
    {
#if ( WINRT )
            FontStyle fontstyle = GetSupportedFontStyle(m_font.FontName);
      switch (fontstyle)
      {
          case FontStyle.Bold:
              Bold = true;
              break;
          case FontStyle.Italic:
              Italic = true;
              break;
      }

            if (Bold) fontstyle |= FontStyle.Bold;
            if (Italic) fontstyle |= FontStyle.Italic;
            if (Strikethrough) fontstyle |= FontStyle.Strikeout;
            if (Underline != ExcelUnderline.None) fontstyle |= FontStyle.Underline;

            if (Array.IndexOf(m_arrItalicFonts, FontName) >= 0)
                fontstyle |= FontStyle.Italic;

#else
        FontStyleEnum fontstyle =
        FontStyleEnum.Regular;
#if !(SILVERLIGHT || WP)
      FontStyle style = GetSupportedFontStyle(m_font.FontName);
      switch (style)
      {
          case FontStyle.Bold:
              Bold = true;
              break;
          case FontStyle.Italic:
              Italic = true;
              break;
      }
#elif SILVERLIGHT
          Silverlight.FontStyle style = GetSupportedFontStyle(m_font.FontName);
          switch (style)
          {
              case Silverlight.FontStyle.Bold:
                  Bold=true;
                  break;
              case Silverlight.FontStyle.Italic:
                  Italic = true;
                  break;
          }
#elif WP
        WP.FontStyle style = GetSupportedFontStyle(m_font.FontName);
        switch (style)
        {
            case WP.FontStyle.Bold:
                Bold = true;
                break;
            case WP.FontStyle.Italic:
                Italic = true;
                break;
        }
#endif

      if( Bold )          fontstyle |= FontStyleEnum.Bold;
      if( Italic )        fontstyle |= FontStyleEnum.Italic;
      if( Strikethrough ) fontstyle |= FontStyleEnum.Strikeout;
      if( Underline != ExcelUnderline.None ) fontstyle |= FontStyleEnum.Underline;

      if (Array.IndexOf(m_arrItalicFonts,FontName)>=0)
          fontstyle |= FontStyleEnum.Italic;
        
#endif

      Font result = new Font( FontName, size, fontstyle, GraphicsUnit.Point, m_btCharSet );
      //LOGFONT logFont = new LOGFONT();
      //result.ToLogFont( logFont, BookGraphics );
      //logFont.lfPitchAndFamily = 2;

      //result = System.Drawing.Font.FromLogFont( logFont );
      //result.ToLogFont( logFont, BookGraphics );

      //if( result.Name != FontName )
      //{
      //  string strFontName = AppImplementation.StandardFont;
      //  result = new Font( strFontName, size, fontstyle, GraphicsUnit.Point, m_btCharSet );
      //}

      return result;
    }
    /// <summary>
    /// Parses native font.
    /// </summary>
    /// <param name="nativeFont">Font to parse.</param>
    public void Parse( Font nativeFont )
    {
      if( nativeFont == null )
        throw new ArgumentNullException( "nativeFont" );

      FontName = nativeFont.Name;
      Size = ( int )nativeFont.Size;
      Strikethrough = nativeFont.Strikeout;
      Bold = nativeFont.Bold;
      Italic = nativeFont.Italic;
      Underline = nativeFont.Underline ? ExcelUnderline.Single : ExcelUnderline.None;
      UpdateColor();
    }
    /// <summary>
    /// Measures the specified string when drawn with this font.
    /// </summary>
    /// <param name="strValue">String to measure.</param>
    /// <returns>String size.</returns>
    public  SizeF MeasureString( string strValue )
    {
      //StringFormat format = StringFormat.GenericTypographic;
      //format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
      //BookGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
      //return BookGraphics.MeasureString( strValue, GenerateNativeFont(), -1, format );
      //TextRenderer renderer = new TextRenderer();

#if !SILVERLIGHT && !WINRT && !WP
      //Size result = TextRenderer.MeasureText( strValue, GenerateNativeFont(), new Size( int.MaxValue, int.MaxValue ),
      //   TextFormatFlags.NoPadding );
      Size result = AppImplementation.MeasureString( strValue, this, new SizeF( int.MaxValue, int.MaxValue ) ).ToSize();
      return new SizeF( result.Width, result.Height-1 );
#elif ( WINRT )
        Windows.Foundation.Size size = new Windows.Foundation.Size(double.MaxValue,
            double.MaxValue);
        UIDispatcher.Initialize();
        Action action=new Action(
            delegate
            {
        
                Windows.UI.Xaml.Controls.TextBlock text = new Windows.UI.Xaml.Controls.TextBlock();

                 text.Text = strValue;
                 Font font = GenerateNativeFont();
                 text.FontFamily = new FontFamily(font.Name);
                 text.FontSize = font.Size;
                 text.FontStyle = (font.Italic) ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                 text.FontWeight = (font.Bold) ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal;
                 text.Measure(size);
                 size = text.DesiredSize;
            });
        UIDispatcher.Execute(action);
       
        return new SizeF((float)size.Width, (float)size.Height);
#else
      SizeF result = SizeF.Empty;

      if( ColorExtension.IsBackgroundThread )
      {
        ManualResetEvent threadCompleteEvent = new ManualResetEvent( false );
        DispatcherOperation operation = System.Windows.Deployment.Current.Dispatcher.BeginInvoke(
          delegate
          {
            result = Measure( strValue );
            threadCompleteEvent.Set();
          } );

        threadCompleteEvent.WaitOne();
        threadCompleteEvent.Close();
      }
      else
      {
        result = Measure( strValue );
      }

      return result;
#endif
    }
#if  (SILVERLIGHT || WP)
    private SizeF Measure( string strValue )
    {
      TextBlock textBlock = new TextBlock()
      {
        Text = strValue,
        FontFamily = new FontFamily( FontName ),
        FontSize = ApplicationImpl.ConvertUnitsStatic( Size * 96, MeasureUnits.Point, MeasureUnits.Inch ),
        FontWeight = Bold ? FontWeights.Bold : FontWeights.Normal,
        FontStyle = Italic ? FontStyles.Italic : FontStyles.Normal,
        TextWrapping = TextWrapping.NoWrap,
        FontStretch = FontStretches.Expanded,
      };
      return new SizeF( ( float )textBlock.ActualWidth, ( float )textBlock.ActualHeight );
    }
#endif

    /// <summary>
    /// Measures the specified string in special way (as close as possible to MS Excel).
    /// </summary>
    /// <param name="strValue">String to measure.</param>
    /// <returns>String size.</returns>
    public SizeF MeasureStringSpecial( string strValue )
    {
#if !SILVERLIGHT && !WINRT && !WP
      StringFormat stringFormat = new StringFormat( StringFormat.GenericTypographic );
      stringFormat.Alignment = StringAlignment.Near;
      stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip;
      stringFormat.SetMeasurableCharacterRanges( characterRanges );

      lock (BookGraphics)
      {
          Graphics graphics = BookGraphics;
          TextRenderingHint oldHint = graphics.TextRenderingHint;

          graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

          double dCorrectSize = Size;

          if (Bold)
          {
              dCorrectSize *= ((Size >= 10) ? DEF_BOLD_FONT_MULTIPLIER : DEF_SMALL_BOLD_FONT_MULTIPLIER);
          }
          else if (Size <= 10)
          {
              dCorrectSize *= DEF_BOLD_FONT_MULTIPLIER;
          }

          SizeF result = graphics.MeasureString(strValue, GenerateNativeFont((float)Math.Ceiling(dCorrectSize)),
            int.MaxValue, stringFormat);

          graphics.TextRenderingHint = oldHint;
          return result;
      }
#else
      return MeasureString( strValue );
#endif
           
        return new SizeF(0, 0);

    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strValue"></param>
    /// <param name="ranges"></param>
    /// <returns></returns>
    public SizeF[] MeasureCharacterRanges( string strValue, CharacterRange[] ranges )
    {
      Font font = GenerateNativeFont();

      StringFormat stringFormat = new StringFormat();
      stringFormat.SetMeasurableCharacterRanges( ranges );
      stringFormat.FormatFlags = StringFormatFlags.NoClip;

      Region[] regions = BookGraphics.MeasureCharacterRanges( strValue, font,
        new RectangleF( 0, 0, 1000, 1000 ), stringFormat );

      SizeF[] result = new SizeF[ regions.Length ];

      for( int i = 0, len = regions.Length; i < len; i++ )
      {
        RectangleF rect = regions[ i ].GetBounds( BookGraphics );
        result[ i ] = new SizeF( rect.Width, rect.Height );
      }

      return result;
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public SizeF MeasureCharacter( char value )
    {
#if !SILVERLIGHT && !WINRT && !WP
      return MeasureCharacterRanges( new string( value, 3 ), characterRanges )[ 1 ];
#else
      throw new NotSupportedException();
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    private void RaiseIndexChangedEvent( ValueChangedEventArgs args )
    {
      if( IndexChanged != null )
      {
        IndexChanged( this, args );
      }
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of the current instance.</returns>
    public FontImpl TypedClone()
    {
      FontImpl font = this.MemberwiseClone() as FontImpl;
      font.m_font = m_font.Clone() as FontRecord;
      return font;
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of the current instance.</returns>
    public object Clone()
    {
      return TypedClone();
    }
    /// <summary>
    /// Clones FontImpl.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public FontImpl Clone( object parent )
    {
      FontImpl result = new FontImpl( Application, parent );
      result.m_bIsDisposed = m_bIsDisposed;
      result.m_index = -1;
      result.m_font = ( FontRecord )m_font.Clone();

      return result;
    }
    #endregion

    #region Static helper methods
    /// <summary>
    /// Converts size of the font to the twips.
    /// </summary>
    /// <param name="fontSize">Size of the font.</param>
    /// <returns>Size of the font in twips.</returns>
    public static int SizeInTwips( double fontSize )
    {
      return ( int )( fontSize * 20 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="twipsSize"></param>
    /// <returns></returns>
    public static double SizeInPoints( int twipsSize )
    {
      return ( double )(twipsSize / 20f);
    }
    /// <summary>
    /// Updates font indexes in different workbooks.
    /// </summary>
    /// <param name="iOldIndex">Index to update.</param>
    /// <param name="dicNewIndexes">Dictionary with new indexes.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Returns new index.</returns>
    public static int UpdateFontIndexes( int iOldIndex, Dictionary<int, int> dicNewIndexes,
      ExcelParseOptions options )
    {
      int iNewIndex = iOldIndex;

      if( dicNewIndexes != null )
      {
        dicNewIndexes.TryGetValue( iOldIndex, out iNewIndex );
      }

      return iNewIndex;
    }
    /// <summary>
    /// Find and returns the supported font font style by the respective font.
    /// </summary>
    /// <param name="FontName"></param>
    /// <returns></returns>
#if !(SILVERLIGHT || WP)
    public FontStyle GetSupportedFontStyle(string fontName)
    {
        FontStyle[] styles = { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic };
        int i;
        Font font = null;
        for (i = 0; i < styles.Length; i++)
        {
            try
            {
#if ( WINRT )
                font = IsSupportedFontStyle(FontName, 12, styles[i]);
#else
                font = font = new Font(FontName, 12, styles[i]);
#endif
                return styles[i];
            }
            catch (Exception ex)
            {

            }
        }
        return FontStyle.Regular;
    }
#endif
#if (SILVERLIGHT)
    public Silverlight.FontStyle GetSupportedFontStyle(string fontName)
    {
        
        Syncfusion.XlsIO.Implementation.Silverlight.FontStyle[] styles = { Silverlight.FontStyle.Regular, Silverlight.FontStyle.Bold, Silverlight.FontStyle.Italic };
        int i;
        Font font = null;
        for (i = 0; i < styles.Length; i++)
        {
            try
            {  
                font = font = new Font(FontName, 10, styles[i], GraphicsUnit.Pixel, 1);
                return styles[i];
            }
            catch (Exception ex)
            {

            }
        }
        return Silverlight.FontStyle.Regular;
    }
#elif WP
    public WP.FontStyle GetSupportedFontStyle(string fontName)
    {
        
        Syncfusion.XlsIO.Implementation.WP.FontStyle[] styles = { WP.FontStyle.Regular, WP.FontStyle.Bold, WP.FontStyle.Italic };
        int i;
        Font font = null;
        for (i = 0; i < styles.Length; i++)
        {
            try
            {  
                font = font = new Font(FontName, 10, styles[i], GraphicsUnit.Pixel, 1);
                return styles[i];
            }
            catch (Exception ex)
            {

            }
        }
        return WP.FontStyle.Regular;
    }
#endif
    #endregion

    #region Class events
    /// <summary>
    /// 
    /// </summary>
    internal event ValueChangedEventHandler IndexChanged;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public event EventHandler OnAfterChange;
    #endregion

    #region Class Overrides
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>
    /// True if the specified Object is equal to the current Object;
    /// otherwise, False.
    /// </returns>
    public override bool Equals( object obj )
    {
      FontImpl font = obj as FontImpl;

      if( font == null ) return false;

      if( GetHashCode() != font.GetHashCode() )
        return false;

      return font.m_font.Equals( m_font ) && m_btCharSet == font.m_btCharSet && m_color == font.m_color;
      //return ( font.m_Font == m_Font );
    }
    /// <summary>
    /// Serves as a hash function for a particular type, suitable for
    /// use in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      return m_font.GetHashCode();
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
      FontImpl font = obj as FontImpl;

      if( font == null )
        throw new ArgumentNullException( "font" );

      int iResult = m_font.CompareTo( font.m_font );

      if( iResult == 0 )
        iResult = m_btCharSet - font.m_btCharSet;

      if( iResult == 0 )
        iResult = ( m_color == font.m_color ) ? 0 : 1;

      return iResult;
    }

    #endregion

    #region IInternalFont Members
    /// <summary>
    /// Returns font index. Read-only.
    /// </summary>
    int Syncfusion.XlsIO.Interfaces.IInternalFont.Index
    {
      get
      {
        return Index;
      }
    }
    /// <summary>
    /// Returns current font. Read-only.
    /// </summary>
    public Syncfusion.XlsIO.Implementation.FontImpl Font
    {
      get
      {
        return this;
      }
    }

    internal string ActualFontName
    {
        get
        {
            return m_actualFont;
        }
        set
        {
            m_actualFont = value;
      }
    }

    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    object Syncfusion.XlsIO.Interfaces.ICloneParent.Clone(object parent)
    {
      return Clone( parent );
    }

    #endregion
      
      #region IDisposable Members
    void IDisposable.Dispose()
   {
        GC.SuppressFinalize(this);
    }

    #endregion

    internal void Clear()
    {
        this.IndexChanged = null;
       

        if(m_fontNative!=null)m_fontNative.Dispose();
        if (m_color != null) m_color.Dispose();

        m_font = null;
        m_color = null;
        m_fontNative = null;

        this.Dispose();


    }
  }

  /// <summary>
  /// Class that is created when user accesses the
  /// font in a multicell range. Redirects all calls 
  /// to the fonts of the individual cells.
  /// </summary>
  public class FontArrayWrapper
    : CommonObject
    , IFont
  {
    #region Class members
    /// <summary>
    /// Array that contains all cells of the range.
    /// </summary>
    private List<IRange> m_arrCells = new List<IRange>();
    #endregion

    #region Class constructors
    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="range">Base range.</param>
    public FontArrayWrapper( IRange range )
      : base( range.Application, range )
    {
      m_arrCells.AddRange( range.Cells );
    }
    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="range">Base range.</param>
    public FontArrayWrapper(List<IRange> lstRange,IApplication application)
        : base(application, lstRange[0])
    {
        m_arrCells = lstRange;
    }
    #endregion

    #region IFont Members
    /// <summary>
    /// True if the font is bold. Read/write Boolean.
    /// </summary>
    public bool Italic
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Italic;
            first = false;
          }
          else if( range.CellStyle.Font.Italic != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Italic = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the primary color of the object. Read / write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        ExcelKnownColors value = ExcelKnownColors.None;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Color;
            first = false;
          }
          else if( range.CellStyle.Font.Color != value )
          {
            return ExcelKnownColors.None;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Color = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets font color. If there is at least one free color, 
    /// define a new color. If not, search for the closest one in 
    /// workbook palette.
    /// </summary>
#if ( WINRT )
        public Windows.UI.Color RGBColor
#else
    public Color  RGBColor
#endif
    {
      get
      {
        ExcelKnownColors color = this.Color;
        IRange range = m_arrCells[ 0 ];
        return range.Worksheet.Workbook.GetPaletteColor( color );
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.RGBColor = value;
        }
      }
    }
    /// <summary>
    /// True if the font style is italic. Read/write Boolean.
    /// </summary>
    public bool Bold
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Bold;
            first = false;
          }
          else if( range.CellStyle.Font.Bold != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Bold = value;
        }
      }
    }

    /// <summary>
    /// True if the font is an outline font. Read/write Boolean.
    /// </summary>
    public bool MacOSOutlineFont
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.MacOSOutlineFont;
            first = false;
          }
          else if( range.CellStyle.Font.MacOSOutlineFont != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.MacOSOutlineFont = value;
        }
      }
    }

    /// <summary>
    /// True if the font is a shadow font or if the object has 
    /// a shadow. Read/write Boolean.
    /// </summary>
    public bool MacOSShadow
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.MacOSShadow;
            first = false;
          }
          else if( range.CellStyle.Font.MacOSShadow != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.MacOSShadow = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the size of the font. Read/write Variant.
    /// </summary>
    public double Size
    {
      get
      {
        double value = 0;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Size;
            first = false;
          }
          else if( range.CellStyle.Font.Size != value )
          {
            return 0;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Size = value;
        }
      }
    }

    /// <summary>
    /// True if the font is struck through with a horizontal line. 
    /// Read/write Boolean
    /// </summary>
    public bool Strikethrough
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Strikethrough;
            first = false;
          }
          else if( range.CellStyle.Font.Strikethrough != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Strikethrough = value;
        }
      }
    }

    /// <summary>
    /// True if the font is formatted as subscript. 
    /// False by default. Read/write Boolean.
    /// </summary>
    public bool Subscript
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Subscript;
            first = false;
          }
          else if( range.CellStyle.Font.Subscript != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Subscript = value;
        }
      }
    }

    /// <summary>
    /// True if the font is formatted as superscript; False by default. 
    /// Read/write Boolean.
    /// </summary>
    public bool Superscript
    {
      get
      {
        bool value = false;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Superscript;
            first = false;
          }
          else if( range.CellStyle.Font.Superscript != value )
          {
            return false;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Superscript = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the type of underline applied to the font. Can 
    /// be one of the following ExcelUnderlineStyle constants. 
    /// Read/write ExcelUnderline.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        ExcelUnderline value = ExcelUnderline.None;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.Underline;
            first = false;
          }
          else if( range.CellStyle.Font.Underline != value )
          {
            return ExcelUnderline.None;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.Underline = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the font name. Read/write string.
    /// </summary>
    public string FontName
    {
      get
      {
        string value = null;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.FontName;
            first = false;
          }
          else if( range.CellStyle.Font.FontName != value )
          {
            return null;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.FontName = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets font vertical alignment.
    /// </summary>
    public ExcelFontVertialAlignment VerticalAlignment
    {
      get
      {
        ExcelFontVertialAlignment value = ExcelFontVertialAlignment.Baseline;
        bool first = true;
        
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];

          if( first )
          {
            value = range.CellStyle.Font.VerticalAlignment;
            first = false;
          }
          else if( range.CellStyle.Font.VerticalAlignment != value )
          {
            value = ExcelFontVertialAlignment.Baseline;
            break;
          }
        }

        return value;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange range = m_arrCells[ i ];
          range.CellStyle.Font.VerticalAlignment = value;
        }
      }
    }
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont()
    {
      IRange range = m_arrCells[ 0 ];
      IStyle style = range.CellStyle;
      IFont font = style.Font;
      return font.GenerateNativeFont();
    }
    /// <summary>
    /// Indicates whether color is automatically selected. Read-only.
    /// </summary>
    public bool IsAutoColor
    {
      get
      {
        return false;
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      // TODO: implement BeginUpdate if necessary
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      // TODO: implement EndUpdate if necessary
    }
    #endregion
  }
}
