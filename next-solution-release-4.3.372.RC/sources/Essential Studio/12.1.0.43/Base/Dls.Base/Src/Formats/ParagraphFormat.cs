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
using System.Drawing;

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents paragraph formatting.
  /// </summary>
  public class ParagraphFormat : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int HrAlignmentKey = 0;
    public const int LeftIndentKey = 2;
    public const int RightIndentKey = 3;
    public const int FirstLineIndentKey = 5;
    public const int KeepKey = 6;
    public const int BeforeSpacingKey = 8;
    public const int AfterSpacingKey = 9;
    public const int KeepFollowKey = 10;
    public const int WidowControlKey = 11;
    public const int PageBreakBeforeKey = 12;
    public const int PageBreakAfterKey = 13;
    public const int BordersKey = 20;
    public const int BackColorKey = 21;
    public const int ColumnBreakAfterKey = 22;
    public const int TabsKey = 30;
    public const int BidiKey = 31;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private TabCollection m_TabCollection;
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets / sets right-to-left property of the paragraph.
    /// </summary>
    public bool Bidi
    {
      get
      {
        return ( bool )this[ BidiKey ];
      }
      set
      {
        this[ BidiKey ] = value;
      }
    }
    /// <summary>
    /// Gets the tabs info.
    /// </summary>
    /// <value>The tabs info.</value>
    public TabCollection Tabs
    {
      get
      {
        if( m_TabCollection == null )
          m_TabCollection = new TabCollection( Document );
        
        return m_TabCollection;
      }
    }
    /// <summary>
    /// True if all lines in the paragraph are to remain on the same page. 
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool Keep
    {
      get
      {
        return ( bool )this[ KeepKey ];
      }
      set
      {
        this[ KeepKey ] = value;
      }
    }
    /// <summary>
    /// True if the paragraph is to remains on the same page as the 
    /// paragraph that follows it. 
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool KeepFollow
    {
      get
      {
        return ( bool )this[ KeepFollowKey ];
      }
      set
      {
        this[ KeepFollowKey ] = value;
      }
    }
    /// <summary>
    /// True if a page break is forced before the paragraph
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool PageBreakBefore
    {
      get
      {
        return ( bool )this[ PageBreakBeforeKey ];
      }
      set
      {
        this[ PageBreakBeforeKey ] = value;
      }
    }
    /// <summary>
    /// True if a page break is forced after the paragraph
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool PageBreakAfter
    {
      get
      {
        if( this[ PageBreakAfterKey ] == null )
        {
          return false;
        }
        return ( bool )this[ PageBreakAfterKey ];
      }
      set
      {
        this[ PageBreakAfterKey ] = value;
      }
    }
    /// <summary>
    /// True if the first and last lines in the paragraph 
    /// are to remain on the same page as the rest of the paragraph. 
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool WidowControl
    {
      get
      {
        return ( bool )this[ WidowControlKey ];
      }
      set
      {
        this[ WidowControlKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets horizontal alignment for the paragraph. 
    /// </summary>
    public HorizontalAlignment HorizontalAlignment
    {
      get
      {
        return ( HorizontalAlignment )this[ HrAlignmentKey ];
      }
      set
      {
        this[ HrAlignmentKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets the value that represents the left indent for paragraph. 
    /// </summary>
    public float LeftIndent
    {
      get
      {
        return ( float )this[ LeftIndentKey ];
      }
      set
      {
        this[ LeftIndentKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets the value that represents the right indent for paragraph.
    /// </summary>
    public float RightIndent
    {
      get
      {
        return ( float )this[ RightIndentKey ];
      }
      set
      {
        this[ RightIndentKey ] = value;
      }
    }
    /// <summary>
    /// Get / set first paragraph line indent
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public float FirstLineIndent
    {
      get
      {
        return ( float )this[ FirstLineIndentKey ];
      }
      set
      {
        this[ FirstLineIndentKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets the spacing (in points) before the paragraph. 
    /// </summary>
    public float BeforeSpacing
    {
      get
      {
        return ( float )this[ BeforeSpacingKey ];
      }
      set
      {
        this[ BeforeSpacingKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets the spacing (in points) after the paragraph.
    /// </summary>
    public float AfterSpacing
    {
      get
      {
        return ( float )this[ AfterSpacingKey ];
      }
      set
      {
        this[ AfterSpacingKey ] = value;
      }
    }
    /// <summary>
    /// Gets collection of borders in the paragraph
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public Borders Borders
    {
      get
      {
        return this[ BordersKey ] as Borders;
      }
    }
    /// <summary>
    /// Gets/sets background color of the paragraph 
    /// </summary>
    public Color BackColor
    {
      get
      {
        return ( Color )this[ BackColorKey ];
      }
      set
      {
        this[ BackColorKey ] = value;
      }
    }
    /// <summary>
    /// True if a column break is forced after the paragraph
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool ColumnBreakAfter
    {
      get
      {
        if( this[ ColumnBreakAfterKey ] == null )
        {
          return false;
        }
        return ( bool )this[ ColumnBreakAfterKey ];
      }
      set
      {
        this[ ColumnBreakAfterKey ] = value;
      }
    }
    #endregion

    #region Class initialize / finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public ParagraphFormat()
    {}
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public ParagraphFormat( IDocument document )
    {
      DocumentEx = ( Document )document;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    protected override void ImportMembers(FormatBase format)
    {
      base.ImportMembers (format);
      ParagraphFormat paraFormat = format as ParagraphFormat;

      if( paraFormat != null )
      {
        Borders.ImportContainer( paraFormat.Borders );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal override void EnsureComposites()
    {
      if( HasKey( BordersKey ))
      {
        EnsureComposites( BordersKey );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object GetDefValue( int key )
    {
      switch( key )
      {
        case HrAlignmentKey:
          return HorizontalAlignment.Left;
          //        case VrAlignmentKey:
          //          return VerticalAlignment.Bottom;
        case LeftIndentKey:
        case RightIndentKey:
          //case BorderColorKey:
          //  return Color.Empty;
        case FirstLineIndentKey:
          return 0f;
        case KeepKey:
        case PageBreakAfterKey:
        case ColumnBreakAfterKey:
        case PageBreakBeforeKey:
        case WidowControlKey:
        case KeepFollowKey:
        case BidiKey:
          return false;
        case AfterSpacingKey:
        case BeforeSpacingKey:
          return 0f;
        case BackColorKey:
          return Color.Empty;
      }

      throw new ArgumentException( "key has invalid value" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override FormatBase GetDefComposite( int key )
    {
      switch( key )
      {
        case BordersKey:
          return GetDefComposite( BordersKey, new Borders( this, BordersKey ) );
      }
      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );

      if( HasKey( BidiKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphBidiAttr, Bidi );
      }
      if( HasKey( HrAlignmentKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphHrAlignmentAttr, HorizontalAlignment );
      }
      //      if( HasKey( VrAlignmentKey ) )
      //      {
      //        writer.WriteValue( XDLSConstants.ParagraphVrAlignmentAttr, VerticalAlignment );
      //      }
      if( HasKey( LeftIndentKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphLeftIndentAttr, LeftIndent );
      }
      if( HasKey( RightIndentKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphRightIndentAttr, RightIndent );
      }
      if( HasKey( FirstLineIndentKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphFirstLineIndentAttr, FirstLineIndent );
      }
      if( HasKey( KeepKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphKeepAttr, Keep );
      }
      if( HasKey( BeforeSpacingKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphBeforeSpacingAttr, BeforeSpacing );
      }
      if( HasKey( AfterSpacingKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphAfterSpacingAttr, AfterSpacing );
      }
      if( HasKey( KeepFollowKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphKeepFollowAttr, KeepFollow );
      }
      if( HasKey( WidowControlKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphWidowControlAttr, WidowControl );
      }
      if( HasKey( PageBreakBeforeKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphPageBreakBeforeAttr, PageBreakBefore );
      }
      if( HasKey( PageBreakAfterKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphPageBreakAfterAttr, PageBreakAfter );
      }
      if( HasKey( ColumnBreakAfterKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphColumnBreakAfterAttr, ColumnBreakAfter );
      }
      if( HasKey( BackColorKey ) )
      {
        writer.WriteValue( XDLSConstants.ParagraphBackColorAttr, BackColor );
      }
      //      if( HasKey( BordersKey ))
      //      {
      //        writer.WriteValue( "Borders",  );
      //      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      if( reader.HasAttribute( XDLSConstants.ParagraphBidiAttr ) )
      {
        Bidi = reader.ReadBoolean( XDLSConstants.ParagraphBidiAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphHrAlignmentAttr ) )
      {
        HorizontalAlignment =
          ( HorizontalAlignment )
          reader.ReadEnum( XDLSConstants.ParagraphHrAlignmentAttr, typeof( HorizontalAlignment ) );
      }
      //      if( reader.HasAttribute( XDLSConstants.ParagraphVrAlignmentAttr ) )
      //      {
      //        VerticalAlignment =
      //          ( VerticalAlignment )reader.ReadEnum( XDLSConstants.ParagraphVrAlignmentAttr, typeof( VerticalAlignment ) );
      //      }
      if( reader.HasAttribute( XDLSConstants.ParagraphLeftIndentAttr ) )
      {
        LeftIndent = reader.ReadFloat( XDLSConstants.ParagraphLeftIndentAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphRightIndentAttr ) )
      {
        RightIndent = reader.ReadFloat( XDLSConstants.ParagraphRightIndentAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphFirstLineIndentAttr ) )
      {
        FirstLineIndent = reader.ReadFloat( XDLSConstants.ParagraphFirstLineIndentAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphKeepAttr ) )
      {
        Keep = reader.ReadBoolean( XDLSConstants.ParagraphKeepAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphBeforeSpacingAttr ) )
      {
        BeforeSpacing = reader.ReadFloat( XDLSConstants.ParagraphBeforeSpacingAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphAfterSpacingAttr ) )
      {
        AfterSpacing = reader.ReadFloat( XDLSConstants.ParagraphAfterSpacingAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphKeepFollowAttr ) )
      {
        KeepFollow = reader.ReadBoolean( XDLSConstants.ParagraphKeepFollowAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphWidowControlAttr ) )
      {
        WidowControl = reader.ReadBoolean( XDLSConstants.ParagraphWidowControlAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphPageBreakBeforeAttr ) )
      {
        PageBreakBefore = reader.ReadBoolean( XDLSConstants.ParagraphPageBreakBeforeAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphPageBreakAfterAttr ) )
      {
        PageBreakAfter = reader.ReadBoolean( XDLSConstants.ParagraphPageBreakAfterAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphBackColorAttr ) )
      {
        BackColor = reader.ReadColor( XDLSConstants.ParagraphBackColorAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ParagraphColumnBreakAfterAttr ) )
      {
        ColumnBreakAfter = reader.ReadBoolean( XDLSConstants.ParagraphColumnBreakAfterAttr );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.BordersItemTag, Borders );
      XDLSHolder.AddElement( XDLSConstants.ParagraphTabsAttr, Tabs );
    }
    #endregion
  }
}