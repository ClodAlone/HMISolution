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

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT) 
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for FontWrapper.
	/// </summary>
	public class FontWrapper
//    : CommonObject
//    , IFont
    : CommonWrapper
    , IFont
    , IInternalFont
	{
    #region Class members
    /// <summary>
    /// Wrapped font.
    /// </summary>
    private FontImpl m_font;
    /// <summary>
    /// Indicates whether font is read-only.
    /// </summary>
    private bool m_bReadOnly;
    /// <summary>
    /// Indicates whether raise events.
    /// </summary>
    private bool m_bRaiseEvents = true;
    /// <summary>
    /// Indicates whether wrapped font is accessed directly (without creating
    /// new font in OnBeforeChange, OnAfterChange methods).
    /// </summary>
    private bool m_bDirectAccess;
    /// <summary>
    /// Font color object
    /// </summary>
    private ColorObject m_fontColor;
    /// <summary>
    /// Represents the baseline settings for superscript and subscript
    /// </summary>
    private int m_Baseline;
    /// <summary>
    /// Represents the color format
    /// </summary>
    private bool m_bIsAutoColor = true;
#endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new font wrapper.
    /// </summary>
    public FontWrapper()// WorkbookImpl book )
      //: base( application, parent )
    {
      //m_book = book;
      //SetParents( parent );
      m_fontColor = new ColorObject( ColorExtension.Black );
      m_fontColor.AfterChange += new ColorObject.AfterChangeHandler( ColorObjectUpdate );
    }
    /// <summary>
    /// Creates new font wrapper.
    /// </summary>
    /// <param name="font">Font to wrap.</param>
    public FontWrapper( FontImpl font )
      : this()
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      m_font = font;
      m_fontColor.CopyFrom( font.ColorObject, false );
    }
    /// <summary>
    /// Creates new font wrapper.
    /// </summary>
    /// <param name="font">Font to wrap.</param>
    /// <param name="bReadOnly">Indicates whether wrapper should be read-only.</param>
    /// <param name="bRaiseEvents">
    /// Indicates whether to call OnBeforeChange and OnAfterChange when any property changes.
    /// </param>
    public FontWrapper( FontImpl font,
      bool bReadOnly, bool bRaiseEvents )
      : this( font )
    {
      m_bReadOnly = bReadOnly;
      m_bRaiseEvents = bRaiseEvents;
    }
    #endregion

    #region IFont Members
    /// <summary>
    /// True if the font is bold. Read / write Boolean.
    /// </summary>
    public bool Bold
    {
      get
      {
        return m_font.Bold;
      }
      set
      {
        if( value != Bold )
        {
          BeginUpdate();
          m_font.Bold = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read / write Integer.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return m_font.Color;
      }
      set
      {
        if( value != Color )
        {
          BeginUpdate();
          m_fontColor.SetIndexed( value );
          m_bIsAutoColor = false;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Gets / sets font color. Searches for the closest color in 
    /// the workbook palette.
    /// </summary>
    public Color RGBColor
    {
      get
      {
        return m_font.RGBColor;
      }
      set
      {
        if( value != RGBColor )
        {
          BeginUpdate();
          m_fontColor.SetRGB( value );
          m_bIsAutoColor = false;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the font style is italic. Read / write Boolean.
    /// </summary>
    public bool Italic
    {
      get
      {
        return m_font.Italic;
      }
      set
      {
        if( value != Italic )
        {
          BeginUpdate();
          m_font.Italic = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the font is an outline font. Read / write Boolean.
    /// </summary>
    public bool MacOSOutlineFont
    {
      get
      {
        return m_font.MacOSOutlineFont;
      }
      set
      {
        if( value != MacOSOutlineFont )
        {
          BeginUpdate();
          m_font.MacOSOutlineFont = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the font is a shadow font or if the object has
    /// a shadow. Read / write Boolean.
    /// </summary>
    public bool MacOSShadow
    {
      get
      {
        return m_font.MacOSShadow;
      }
      set
      {
        if( value != MacOSShadow )
        {
          BeginUpdate();
          m_font.MacOSShadow = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the size of the font. Read / write Variant.
    /// </summary>
    public double Size
    {
      get
      {
        return m_font.Size;
      }
      set
      {
        if( value != Size )
        {
          BeginUpdate();
          m_font.Size = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the font is struck through with a horizontal line.
    /// Read / write Boolean
    /// </summary>
    public bool Strikethrough
    {
      get
      {
        return m_font.Strikethrough;
      }
      set
      {
        if( value != Strikethrough )
        {
          BeginUpdate();
          m_font.Strikethrough = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Gets or sets the offset value of superscript and subscript
    /// </summary>
    public int Baseline
    {
        get
        {
            return m_Baseline;
        }
        set
        {
            m_Baseline = value;
        }
    }
    /// <summary>
    /// True if the font is formatted as subscript.
    /// False by default. Read / write Boolean.
    /// </summary>
    public bool Subscript
    {
      get
      {
        return m_font.Subscript;
      }
      set
      {
        if( value != Subscript )
        {
          BeginUpdate();
          m_font.Subscript = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// True if the font is formatted as superscript. False by default.
    /// Read/write Boolean
    /// </summary>
    public bool Superscript
    {
      get
      {
        return m_font.Superscript;
      }
      set
      {
        if( value != Superscript )
        {
          BeginUpdate();
          m_font.Superscript = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the type of underline applied to the font. Can
    /// be one of the following ExcelUnderlineStyle constants.
    /// Read / write ExcelUnderline.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        return m_font.Underline;
      }
      set
      {
        if( value != Underline )
        {
          BeginUpdate();
          m_font.Underline = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Returns or sets the font name. Read / write string.
    /// </summary>
    public string FontName
    {
      get
      {
        return m_font.FontName;
      }
      set
      {
        if( value != FontName )
        {
          BeginUpdate();
          m_font.FontName = value;
          EndUpdate();
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
        return m_font.VerticalAlignment;
      }
      set
      {
        if( value != VerticalAlignment )
        {
          BeginUpdate();
          m_font.VerticalAlignment = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont()
    {
      return m_font.GenerateNativeFont();
    }
    /// <summary>
    /// Indicates whether color is automatically selected. Read-only.
    /// </summary>
    public bool IsAutoColor
    {
        get
        {
            return m_bIsAutoColor ;
        }
        set
        {
            m_bIsAutoColor = value ;
        }
    }
    #endregion

    #region IParentApplication Members
    /// <summary>
    /// Application object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_font.Application;
      }
    }

    /// <summary>
    /// Parent object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_font.Parent;
      }
    }

    #endregion

    #region Class helper methods
    public void ColorObjectUpdate()
    {
      BeginUpdate();
      m_font.ColorObject.CopyFrom( m_fontColor, true );
      EndUpdate();
    }
    /// <summary>
    /// Returns copy of current object.
    /// </summary>
    /// <param name="book">Parent Workbook.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new indexes.</param>
    /// <returns>Clone of FontWraper.</returns>
    public FontWrapper Clone( WorkbookImpl book, object parent, IDictionary dicFontIndexes )
    {
      FontWrapper result = new FontWrapper();
      int iIndex = m_font.Index;

      if( dicFontIndexes != null )
      {
        iIndex = ( int )dicFontIndexes[ iIndex ];
      }

      result.m_bReadOnly = m_bReadOnly;
      result.m_font = ( FontImpl )book.InnerFonts[ iIndex ];
      //result.AddWrapper();

      return result;
    }
//    /// <summary>
//    /// Adds this wrapper to the workbook's collection if necessary.
//    /// </summary>
//    public void AddWrapper()
//    {
//      WorkbookImpl book = m_font.ParentWorkbook;
//    
//      if( book.ContainsFont( m_font ) )
//      {
//        /*m_iWrapperIndex =*/ book.AddWrapper( this );
//      }
//    }
    #endregion

    #region Class events
    /// <summary>
    /// Event raised after wrapped font changed.
    /// </summary>
    public event EventHandler AfterChangeEvent;
    #endregion

    #region Class Public Properties
    /// <summary>
    /// Returns font index. Read-only.
    /// </summary>
    public int FontIndex
    {
      get
      {
        return m_font.Index;
      }
    }
    /// <summary>
    /// Returns wrapped font. Read-only.
    /// </summary>
    public FontImpl Wrapped
    {
      get
      {
        return m_font;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_font = value;
      }
    }
    /// <summary>
    /// Indicates whether font is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return m_bReadOnly;
      }
      set
      {
        if( !value && m_bReadOnly )
          throw new ArgumentOutOfRangeException( "Can't change this property for read-only fonts" );

        m_bReadOnly = value;
      }
    }
    /// <summary>
    /// Returns parent workbook.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_font.ParentWorkbook;
      }
    }
    /// <summary>
    /// Indicates whether wrapped font is accessed directly (without creating
    /// new font in OnBeforeChange, OnAfterChange methods).
    /// </summary>
    public bool IsDirectAccess
    {
      get
      {
        return m_bDirectAccess;
      }
      set
      {
        m_bDirectAccess = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        // TODO: update events.
        return m_fontColor;
      }
    }
    #endregion

    #region IInternalFont Members
    /// <summary>
    /// Returns index of the wrapped font. Read-only.
    /// </summary>
    public int Index
    {
      get
      {
        return m_font.Index;
      }
    }
    /// <summary>
    /// Returns wrapped font. Read-only.
    /// </summary>
    public FontImpl Font
    {
      get
      {
        return m_font;
      }
    }

    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {
        if( m_bReadOnly )
          throw new ReadOnlyException();

        if( !m_bRaiseEvents )
          return;

        if( !m_bDirectAccess )
        {
          m_font = ( FontImpl )Workbook.CreateFont( m_font, false );
        }
      }

      base.BeginUpdate();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        if( !m_bRaiseEvents )
          return;

        WorkbookImpl book = Workbook;

        if( !m_bDirectAccess )
        {
          m_font = ( FontImpl )book.AddFont( m_font );
        }

        book.SetChanged();

        if( AfterChangeEvent != null )
        {
          AfterChangeEvent( this, EventArgs.Empty );
        }
      }
    }
    /// <summary>
    /// Invokes after change event.
    /// </summary>
    internal void InvokeAfterChange()
    {
      if( AfterChangeEvent != null )
        AfterChangeEvent( this, EventArgs.Empty );
    }
    #endregion


    internal void Dispose()
    {
        this.AfterChangeEvent = null;
        m_fontColor.Dispose();


        m_font.Clear();
        m_font = null;
        m_fontColor = null;
    }
    }
}
