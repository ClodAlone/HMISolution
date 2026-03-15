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

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
  /// <summary>
  /// Summary description for FontGroup.
  /// </summary>
  public class FontGroup
    : CommonObject
    , IFont
  {
    #region Class members
    /// <summary>
    /// Parent range group.
    /// </summary>
    private StyleGroup m_styleGroup;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    public FontGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_styleGroup = FindParent( typeof( StyleGroup ) ) as StyleGroup;

      if( m_styleGroup == null )
      {
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent style group." );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the group. Read-only.
    /// </summary>
    public IFont this[ int index ]
    {
      get
      {
        return m_styleGroup[ index ].Font;
      }
    }
    /// <summary>
    /// Returns number of elements in the group. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_styleGroup.Count;
      }
    }
    #endregion

    #region IFont Properties
    /// <summary>
    /// True if the font is bold. Read / write Boolean.
    /// </summary>
    public bool Bold
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].Bold;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Bold )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Bold = value;
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
        int iCount = Count;

        if( iCount == 0 ) return ExcelKnownColors.None;

        ExcelKnownColors result = this[ 0 ].Color;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Color )
            return ExcelKnownColors.None;
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
    /// Gets / sets font color. Searches for the closest color in 
    /// the workbook palette.
    /// </summary>
    public Color RGBColor
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ColorExtension.Empty;

        Color result = this[ 0 ].RGBColor;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].RGBColor )
            return ColorExtension.Empty;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].RGBColor = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].Italic;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Italic )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Italic = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].MacOSOutlineFont;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].MacOSOutlineFont )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].MacOSOutlineFont = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].MacOSShadow;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].MacOSShadow )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].MacOSShadow = value;
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
        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        double result = this[ 0 ].Size;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Size )
            return double.MinValue;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Size = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].Strikethrough;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Strikethrough )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Strikethrough = value;
        }
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].Subscript;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Subscript )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Subscript = value;
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
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].Superscript;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Superscript )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Superscript = value;
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
        int iCount = Count;

        if( iCount == 0 ) return ExcelUnderline.None;

        ExcelUnderline result = this[ 0 ].Underline;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Underline )
            return ExcelUnderline.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Underline = value;
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
        int iCount = Count;

        if( iCount == 0 ) return null;

        string result = this[ 0 ].FontName;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].FontName )
            return null;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].FontName = value;
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
        int iCount = Count;

        if( iCount == 0 ) return ExcelFontVertialAlignment.Baseline;

        ExcelFontVertialAlignment result = this[ 0 ].VerticalAlignment;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].VerticalAlignment )
            return ExcelFontVertialAlignment.Baseline;
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

    #region IFont methods
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont()
    {
      throw new NotSupportedException();
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
