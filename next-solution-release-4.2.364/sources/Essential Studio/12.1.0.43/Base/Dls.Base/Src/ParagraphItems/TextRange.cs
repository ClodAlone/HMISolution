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
using System.Drawing;
using System.Text.RegularExpressions;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents minimal text run that has same formatting.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class TextRange
    : ParagraphItem,
    ITextRange,
    IStringWidget
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private int m_iLength = 0;
    /// <summary>
    /// Hyperlink object.
    /// </summary>
    private Hyperlink m_hyperlink;
    /// <summary>
    /// 
    /// </summary>
    protected CharacterFormat m_format = null;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bDetached = true;
    /// <summary>
    /// 
    /// </summary>
    protected string m_strDetachedText = string.Empty;
    #endregion

    #region Properties
    /// <summary>
    /// Gets hyperlink object.
    /// </summary>
    public Hyperlink Hyperlink
    {
      get
      {
        if( m_hyperlink == null )
        {
          m_hyperlink = new Hyperlink( Document );
        }

        return m_hyperlink;
      }
    }
    /// <summary>
    /// Gets / sets text.
    /// </summary>
    public string Text
    {
      get
      {
        if( m_bDetached )
        {
          return m_strDetachedText;
        }
        else
        {
          return OwnerParagraphEx.GetText( m_iStartIndex, m_iLength );
        }
      }
      set
      {
        if( m_bDetached )
        {
          m_strDetachedText = value;
          m_iLength = value.Length;
        }
        else if( value != Text )
        {
          OwnerParagraphEx.UpdateText( this, m_iLength, value );
          m_iLength = value.Length;
        }
      }
    }
    /// <summary>
    /// Gets  character format( font properties ).
    /// </summary>
    public CharacterFormat CharacterFormat
    {
      get
      {
        return m_format;
      }
    }
    /// <summary>
    /// Gets length of the text.
    /// </summary>
    protected internal int TextLength
    {
      get
      {
        return m_iLength;
      }
      set
      {
        m_iLength = value;
      }
    }
    /// <summary>
    /// Gets owner paragraph.
    /// </summary>
    protected Paragraph OwnerParagraphEx
    {
      get
      {
        return OwnerParagraph as Paragraph;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializing constructor
    /// </summary>
    public TextRange( IDocument doc )
      : base( doc )
    {
      m_format = DocumentEx.CreateCharacterFormatImpl();
      m_hyperlink = new Hyperlink( doc );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="txtRange"></param>
    /// <param name="paragraph"></param>
    protected internal TextRange( ITextRange txtRange, IParagraph paragraph )
      : this( (paragraph as Paragraph ).Document )
    {
      base.SetOwnerParagraph( ( paragraph as Paragraph ), ( txtRange as TextRange ).StartIndex );
      Text = txtRange.Text;
      m_format.ImportContainer( txtRange.CharacterFormat );
      //CharacterFormat.ApplyBase( paragraph.CharacterFormat );
    }
    #endregion

    #region Internal methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    internal int Replace( Regex pattern, string replace )
    {
      int changesMade = 0;
      string s = pattern.Replace(Text, replace);
      if( s != Text )
      {
        changesMade = pattern.Matches(Text).Count;;
        Text = s;
      }
      return changesMade;
    }
    #endregion

    #region Overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <param name="startIndex"></param>
    protected internal override void SetOwnerParagraph( IParagraph paragraph, int startIndex )
    {
      base.SetOwnerParagraph( paragraph, startIndex );
      IParagraphStyle style = paragraph.GetStyle();
      if( style != null )
      {
        m_format.ApplyBase( style.CharacterFormat );
      }
      
      if( m_bDetached )
      {
        m_bDetached = false;
        
        if( m_strDetachedText != null && m_strDetachedText.Length != 0 )
        {
          m_iLength = 0;
          Text = m_strDetachedText;
          m_strDetachedText = null;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      return new TextRange( this, paragraph );
    }
    #endregion

    #region XDLSSerializableBase overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void InitXDLSHolder()
    {
      XDLSHolder.SkipID = true;
      XDLSHolder.AddElement( XDLSConstants.CharacterFormatTag, m_format );
      XDLSHolder.AddElement( XDLSConstants.HyperlinkType, m_hyperlink );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      base.WriteXmlContent( writer );
      writer.WriteChildStringElement( XDLSConstants.TextTag, Text );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
      if( reader.TagName == XDLSConstants.TextTag )
      {
        m_iStartIndex = OwnerParagraph.Text.Length;
        Text = reader.ReadChildStringContent();
        if( Text == "" ) 
          reader.InnerReader.Read();
        return true;
      }

      return false;
    }
    #endregion
    
    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = ( Text == "\t" )
        ? new LayoutTextRangeInfo( this )
        : new LayoutInfo( ChildrenLayoutDirection.Horizontal );
      
      m_layoutInfo.IsLineBreak = CharacterFormat.LineBreak;
      
      if( CharacterFormat.Position > 0 )
      {
        m_layoutInfo.Margins.Bottom = CharacterFormat.Position;
      }

      if( CharacterFormat.Position < 0 )
      {
        m_layoutInfo.Margins.Top = -CharacterFormat.Position;
      }
    }
    #endregion

    #region IStringWidget implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      string text = ltWidget.TextTag != null ? ltWidget.TextTag : Text;
      ( this as IStringWidget ).Draw( cg, ltWidget, text );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <returns></returns>
    public SizeF Measure( CustomGraphics cg )
    {
      //string text = ltWidget.TextTag != null ? ltWidget.TextTag : Text;
      return ( this as IStringWidget ).Measure( cg, Text );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    /// <param name="text"></param>
    void IStringWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget, string text )
    {
      ( cg as DLSGraphics ).DrawTextRange( this, ltWidget, text );
      DrawImpl( cg, ltWidget );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="offset"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    int IStringWidget.OffsetToIndex( CustomGraphics cg, double offset, string text )
    {
      LayoutParagraphInfo paragraphInfo = m_layoutInfo as LayoutParagraphInfo;
      bool textWrap = ( paragraphInfo != null ) ? paragraphInfo.TextWrap : true;
      return cg.GetSplitIndexByOffset( text, this, offset, !textWrap );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    SizeF ITextMeasurable.Measure( CustomGraphics cg, string text )
    {
      return ( cg as DLSGraphics ).MeasureTextRange( this, text );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <returns></returns>
    public double GetTextAscent( CustomGraphics cg )
    {
      return ( cg as DLSGraphics ).GetAscentTextRange( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    ISplitLeafWidget[] ISplitLeafWidget.SplitByOffset( CustomGraphics cg, SizeF offset )
    {
      return SplitStringWidget.SplitByOffset( cg, offset.Width, this, null );
    }
    #endregion
    
    #region Internal declarations
    /// <summary>
    /// /
    /// </summary>
    internal class LayoutTextRangeInfo
      : LayoutTabsInfo
    {
      #region Constructors
      /// <summary>
      /// Initializes a new instance of the <see cref="LayoutTextRangeInfo"/> class.
      /// </summary>
      /// <param name="textRange">The text range.</param>
      public LayoutTextRangeInfo( TextRange textRange )
        : base( Layouting.ChildrenLayoutDirection.Horizontal )
      {
        textRange.Text = string.Empty;
        float pageMarginLeft = textRange.Document.LastSection.PageSetup.Margins.Left;
        pageMarginLeft = ( pageMarginLeft != -0.05f ) ? pageMarginLeft : 0f;
        m_defaultTabWidth = textRange.Owner.Document.LastSection.PageSetup.DefaultTabWidth;
        m_pageMarginLeft = pageMarginLeft;
        Paragraph paragrath = textRange.OwnerParagraph as Paragraph;

        ParagraphFormat pFormat = paragrath.ParagraphFormat;

        

        if( pFormat.Tabs.Count == 0 && paragrath.Style.ParagraphFormat.Tabs.Count > 0 )
        {
          pFormat = paragrath.Style.ParagraphFormat;
        }

        // fill tabs
        for( int i = 0, size = pFormat.Tabs.Count; i < size; i++ )
        {
          Tab tab = pFormat.Tabs[ i ];
          AddTab(
            tab.Position + pageMarginLeft,
            ( Layouting.TabJustification )tab.Justification,
            ( Layouting.TabLeader )tab.TabLeader );
        }
      }
      #endregion
    }
    #endregion
  }
}
