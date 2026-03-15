#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Drawing;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents a style description. The Style object contains
  /// all style attributes (font, number format, alignment, and so on) as
  /// properties. There are several built-in styles, including Normal,
  /// Currency, and Percent. Using the Style object is a fast and efficient
  /// way to change several cell-formatting properties on multiple cells at
  /// the same time.
  /// For the Workbook object, the Style object is a member of the Styles
  /// collection. The Styles collection contains all the defined styles for
  /// the workbook.
  /// </summary>
  public class StyleImpl
    : ExtendedFormatWrapper
    , IStyle
    , IComparable
    , INamedObject
  {
    #region Subtypes
    /// <summary>
    /// This class stores default predefined styles settings.
    /// </summary>
    internal class StyleSettings
    {
      #region Members
      /// <summary>
      /// Fill settings.
      /// </summary>
      public FillImpl Fill;
      /// <summary>
      /// Font settings.
      /// </summary>
      public FontSettings Font;
      /// <summary>
      /// Borders information.
      /// </summary>
      public BorderSettings Borders;
      #endregion

      #region Methods
      /// <summary>
      /// Initializes new instance of the class.
      /// </summary>
      /// <param name="fill">Fill object.</param>
      /// <param name="font">Font object.</param>
      public StyleSettings( FillImpl fill, FontSettings font )
        : this( fill, font, null )
      {
      }
      /// <summary>
      /// Initializes new instance of the class.
      /// </summary>
      /// <param name="fill">Fill object.</param>
      /// <param name="font">Font object.</param>
      /// <param name="borders">Borders object.</param>
      public StyleSettings( FillImpl fill, FontSettings font, BorderSettings borders )
      {
        Fill = fill;
        Font = font;
        Borders = borders;
      }
      #endregion

      internal void Clear()
      {
          if (this.Fill != null) this.Fill.Dispose();
          if (this.Font != null) this.Font.Dispose();
          if (this.Borders != null) this.Borders.Dispose();

          this.Fill = null;
          this.Font = null;
          this.Borders = null;
      }
    }
    internal class FontSettings
    {
      public ColorObject Color;
      public int Size;
      public bool Bold;
      public bool Italic;
      public string Name;

      public FontSettings( ColorObject color )
        : this( color, 11 )
      {
      }
      public FontSettings( ColorObject color, int size )
        : this( color, size, FontStyle.Regular )
      {
      }
      public FontSettings( ColorObject color, FontStyle fontStyle )
        : this( color, 11, fontStyle )
      {
      }
      public FontSettings( ColorObject color, int size, FontStyle fontStyle )
        : this( color, size, fontStyle, null )
      {
      }
      public FontSettings( ColorObject color, int size, FontStyle fontStyle, string name )
      {
        Color = color;
        Size = size;
        Bold = ( fontStyle & FontStyle.Bold ) != 0;
        Italic = ( fontStyle & FontStyle.Italic ) != 0;
        Name = name;
      }

      internal void Dispose()
      {
          this.Color.Dispose();
          this.Color = null;
      }
    }
    internal class BorderSettings
    {
      public ColorObject BorderColor;
      public ExcelLineStyle Left;
      public ExcelLineStyle Right;
      public ExcelLineStyle Top;
      public ExcelLineStyle Bottom;
      public BorderSettings( ColorObject color, ExcelLineStyle lineStyle )
      {
        BorderColor = color;
        Left = Right = Top = Bottom = lineStyle;
      }
      public BorderSettings( ColorObject color, ExcelLineStyle left, ExcelLineStyle right,
        ExcelLineStyle top, ExcelLineStyle bottom )
      {
        BorderColor = color;
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
      }

      internal void Dispose()
      {
          this.BorderColor.Dispose();
          this.BorderColor = null;
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

    #region Class constants
    /// <summary>
    /// First excel 2007 specific style in the styles array.
    /// </summary>
    private const int Excel2007StylesStart = 10;
    

    
    /// <summary>
    /// Style options.
    /// </summary>
    [ Flags ]
    private enum StyleOptions
    {
      None          = 0,
      UpdateStyleXF = 1,
      Temporary     = 2,
    }
    /// <summary>
    /// Constant indicating that one object is less than another. Used by CompareTo method.
    /// </summary>
    public const int DEF_LESS = -1;
    /// <summary>
    /// Constant indicating that one object is equal to another. Used by CompareTo method.
    /// </summary>
    public const int DEF_EQUAL = 0;
    /// <summary>
    /// Constant indicating that one object is larger than another. Used by CompareTo method.
    /// </summary>
    public const int DEF_LARGER = 1;
    /// <summary>
    /// Build in style index for RowLevel_n styles.
    /// </summary>
    private const int RowLevelStyleIndex = 1;
    /// <summary>
    /// Build in style index for ColLevel_n styles.
    /// </summary>
    private const int ColumnLevelStyleIndex = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Style record that describes style.
    /// </summary>
    private StyleRecord m_style;
    /// <summary>
    /// Indicates whether do not to compare name during CompareTo operation.
    /// </summary>
    private bool m_bNotCompareName;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the style.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    public StyleImpl( WorkbookImpl book )
      : base( book )
    {
      m_style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      SetFormatIndex( m_style.ExtendedFormatIndex );
    }

    /// <summary>
    /// Initializes new instance of the style.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="style">Style record to parse.</param>
    [ CLSCompliant( false ) ]
    public StyleImpl( WorkbookImpl book, StyleRecord style )
      : base( book )
    {
      m_style = style;
      SetFormatIndex( m_style.ExtendedFormatIndex );

      if( style.IsBuildInStyle && style.BuildInOrNameLen == 0 )
      {
        m_font.IsDirectAccess = true;
      }
    }
    /// <summary>
    /// Initializes new instance of the style.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="strName">Name of the style to create.</param>
    public StyleImpl( WorkbookImpl book, string strName )
      : this( book, strName, null )
    {
    }
    /// <summary>
    /// Initializes new instance of the style.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="strName">Name of the style to create.</param>
    /// <param name="baseStyle">Base style.</param>
    public StyleImpl( WorkbookImpl book, string strName, StyleImpl baseStyle )
      : this( book, strName, baseStyle, false )
    {
    }
    /// <summary>
    /// Initializes new instance of the style.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="strName">Name of the style to create.</param>
    /// <param name="baseStyle">Base style.</param>
    /// <param name="bIsBuiltIn">Indicates whether created style is built in.</param>
    public StyleImpl( WorkbookImpl book, string strName,
      StyleImpl baseStyle, bool bIsBuiltIn )
      : this( book )
    {
      if( baseStyle != null )
      {
        StyleRecord parentRecord = baseStyle.m_style;
        m_style = parentRecord.Clone() as StyleRecord;
      }
      else
      {
        m_style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      }

      int index = -1;

      if( bIsBuiltIn )
      {
        index = Array.IndexOf( DefaultStyleNames, strName );

        if( index < 0 )
          throw new ArgumentOutOfRangeException( "name", "Can't find built in name" );

        m_style.BuildInOrNameLen = ( byte )index;
      }
      else
      {
        m_style.StyleName = strName;
      }

      m_style.IsBuildInStyle = bIsBuiltIn;
      ExtendedFormatImpl format;

      if( baseStyle == null )
      {
        format = ( ExtendedFormatImpl )m_book.CreateExtFormat( true );
      }
      else
      {
        format = ( ExtendedFormatImpl )m_book.CreateExtFormat( baseStyle.Wrapped, true );
      }

      format.ParentIndex = m_book.MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
      format.XFType = ExtendedFormatRecord.TXFType.XF_CELL;

      m_style.ExtendedFormatIndex = ( ushort )format.Index;
      m_style.IsBuildInStyle = bIsBuiltIn;
      SetFormatIndex( format.Index );

      if( bIsBuiltIn && 
        ( m_book.Version !=ExcelVersion.Excel97to2003 ) &&
        index >= Excel2007StylesStart )
      {
        CopyDefaultStyleSettings( index );
        //throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Copies default style settings into internal extended format from built-in style.
    /// </summary>
    /// <param name="index">Built-in style index.</param>
    private void CopyDefaultStyleSettings( int index )
    {
      StyleSettings settings = (this.Application as ApplicationImpl).BuiltInStyleInfo[ index ];
      FillImpl fill = settings.Fill;
      // We can do direct copy because we are creating new style object which always
      // has separate extended format for its purposes regardless of settings.
      if( fill != null )
        Excel2007Parser.CopyFillSettings( fill, m_xFormat );

      FontSettings font = settings.Font;

      if( font != null )
        CopyFontSettings( font, m_font );

      BorderSettings borders = settings.Borders;

      if( borders != null )
        CopyBordersSettings( borders, m_xFormat );
    }

    private void CopyBordersSettings( BorderSettings borders, ExtendedFormatImpl m_xFormat )
    {
      ColorObject color = borders.BorderColor;

      if( borders.Left != ExcelLineStyle.None )
      {
        m_xFormat.LeftBorderLineStyle = borders.Left;

        if( color != null )
          m_xFormat.LeftBorderColor.CopyFrom( color, true );
      }

      if( borders.Right != ExcelLineStyle.None )
      {
        m_xFormat.RightBorderLineStyle = borders.Right;

        if( color != null )
          m_xFormat.RightBorderColor.CopyFrom( color, true );
      }

      if( borders.Top != ExcelLineStyle.None )
      {
        m_xFormat.TopBorderLineStyle = borders.Top;

        if( color != null )
          m_xFormat.TopBorderColor.CopyFrom( color, true );
      }

      if( borders.Bottom != ExcelLineStyle.None )
      {
        m_xFormat.BottomBorderLineStyle = borders.Bottom;

        if( color != null )
          m_xFormat.BottomBorderColor.CopyFrom( color, true );
      }
    }

    private void CopyFontSettings( FontSettings font, FontWrapper m_font )
    {
      ColorObject fontColor = font.Color;

      m_font.BeginUpdate();

      if( fontColor != null )
        m_font.ColorObject.CopyFrom( fontColor, true );

      m_font.Size = font.Size;
      m_font.Italic = font.Italic;
      m_font.Bold = font.Bold;

      string fontName = font.Name;

      if( fontName != null )
        m_font.FontName = fontName;

      m_font.EndUpdate();
    }
    #endregion

    #region Class Public Properites
    private string[] DefaultStyleNames
    {
        get
        {
            return m_book.AppImplementation.DefaultStyleNames;
        }
    }
    /// <summary>
    /// Indicates whether style is build in. Read-only.
    /// </summary>
    new public bool BuiltIn
    {
      get
      {
        return m_style.IsBuildInStyle;
      }
    }
    /// <summary>
    /// Returns name of the style. Read-only.
    /// </summary>
    new public string Name
    {
      get
      {
        string strResult = null;
        bool bUseStyleName = !BuiltIn;

        if( !bUseStyleName )
        {
          int iBuildInIndex = m_style.BuildInOrNameLen;
          strResult = DefaultStyleNames[ iBuildInIndex ];

          if( iBuildInIndex == RowLevelStyleIndex || iBuildInIndex == ColumnLevelStyleIndex )
            strResult += ( m_style.OutlineStyleLevel + 1 ).ToString();

          bUseStyleName = ( strResult == null || strResult.Length == 0 );
        }

        if( bUseStyleName )
        {
          string strStyleName = m_style.StyleName;

          if( strStyleName != null )
            strResult = m_style.StyleName;
        }

        return strResult;
      }
    }

    /// <summary>
    /// Indicates whether style is initialized (differs from Normal style).
    /// Read-only.
    /// </summary>
    new public bool IsInitialized
    {
      get
      {
        string strNormalName = DefaultStyleNames[ 0 ];
        
        return !( Name == strNormalName || StylesCollection.CompareStyles(
          this, m_book.Styles[ strNormalName ] ) );
      }
    }
    /// <summary>
    /// Returns index of the style's extended format.
    /// </summary>
    public int Index
    {
      get
      {
        return m_xFormat.Index;
      }
    }
    /// <summary>
    /// Indicates whether do not to compare name during CompareTo operation.
    /// </summary>
    public bool NotCompareNames
    {
      get
      {
        return m_bNotCompareName;
      }
      set
      {
        m_bNotCompareName = value;
      }
    }
    /// <summary>
    /// Returns style record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public StyleRecord Record
    {
      get
      {
        UpdateStyleRecord();
        return m_style;
      }
    }
    public bool IsBuiltInCustomized
    {
        get
        {
            return this.Record.IsBuiltIncustomized;
        }
        set
        {
            this.Record.IsBuiltIncustomized = value;
        }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Saves style into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList to save style into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_style );
    }
    /// <summary>
    /// Updates style record according to the xf indexes.
    /// </summary>
    public void UpdateStyleRecord()
    {
      m_style.ExtendedFormatIndex = ( ushort )m_xFormat.Index;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// This method is called after any changes are made in styles.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        if( AfterChange != null )
        {
          AfterChange( this, EventArgs.Empty );
        }
      }

      // 1. Find all children
      // 2. copy changes inside each of them if necessary
      List<int> arrChildXFs = FindChildXFs();
      ExtendedFormatsCollection arrXFormats = m_book.InnerExtFormats;

      for( int i = 0, len = arrChildXFs.Count; i < len; i++ )
      {
        int iChildXFIndex = arrChildXFs[ i ];
        ExtendedFormatImpl format = arrXFormats[ iChildXFIndex ];
        format.SynchronizeWithParent();
      }
    }
    /// <summary>
    /// This method is called before any changed are made in styles.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {
        if( BeforeChange != null )
        {
          BeforeChange( this, EventArgs.Empty );
        }
      }

      base.BeginUpdate ();
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>Copy of the current instance.</returns>
    public override object Clone( object parent )
    {
      //throw new NotImplementedException();
      StyleImpl result = ( StyleImpl )base.Clone( parent );
      result.m_style = ( StyleRecord )CloneUtils.CloneCloneable( m_style );

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private List<int> FindChildXFs()
    {
      List<int> arrResult = new List<int>();
      ExtendedFormatsCollection arrXfs = m_book.InnerExtFormats;
      int iXFIndex = Index;

      for( int i = 0, len = arrXfs.Count; i < len; i++ )
      {
        ExtendedFormatImpl format = arrXfs[ i ];

        if( format.ParentIndex == iXFIndex )
        {
          arrResult.Add( i );
        }
      }

      return arrResult;
    }
    #endregion

    #region Class events
    /// <summary>
    /// This event is raised before any changes are made in styles.
    /// </summary>
    public event EventHandler BeforeChange;
    /// <summary>
    /// This event is raised after any changes are made in styles.
    /// </summary>
    public event EventHandler AfterChange;
    #endregion

    #region IComparable Members
    /// <summary>
    /// Compares the current instance with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the comparands.
    /// The return value has these meanings:
    /// Value                Meaning
    /// Less than zero     - This instance is less than obj. 
    /// Zero               - This instance is equal to obj. 
    /// Greater than zero  - This instance is greater than obj. 
    /// </returns>
    public int CompareTo( object obj )
    {
      StyleImpl style = obj as StyleImpl;

      if( style == null ) return DEF_LARGER;

      int result = m_font.Wrapped.CompareTo( style.m_font.Wrapped );

      if( result != DEF_EQUAL )
      {
        //        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Not equal", "Comparison" );
        return result;
      }


      result = m_xFormat.CompareToWithoutIndex( style.m_xFormat );

      if( result != DEF_EQUAL ) return result;

      if( !( m_bNotCompareName || style.m_bNotCompareName ) )
      {
        result = Name.CompareTo ( style.Name );
        if( result != DEF_EQUAL ) return result;
      }
      //      if( result == DEF_EQUAL )
      //        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Equal", "Comparison" );

      return result;
    }
    #endregion

    internal void Dispose()
    {
        this.AfterChange = null;
        this.BeforeChange = null;

        m_style = null;

        base.Dispose();

    }
  }
}
