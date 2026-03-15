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
using System.Drawing;
using System.Security.Policy;
using System.Text.RegularExpressions;

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
using System.Text;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Reperesents a paragraph
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class Paragraph
    : WidgetContainer,
      IWidget,
      IParagraph
  {
    #region Constants
    /// <summary>
    /// 
    /// </summary>
    internal const string DEF_WHOLE_WORD_BEFORE = @"(?<=^|\W|\t)";
    internal const string DEF_WHOLE_WORD_AFTER = @"(?=$|\W|\t)";
    #endregion

    #region Class members
    /// <summary>
    /// The paragraph style
    /// </summary>
    protected IParagraphStyle m_style;
    /// <summary>
    /// The paragraph text
    /// </summary>
    //private string m_strText = "";
    private StringBuilder m_strTextBuilder = new StringBuilder( 1 );
    /// <summary>
    /// The character format
    /// </summary>
    protected CharacterFormat m_chFormat = null;
    /// <summary>
    /// The paragraph format
    /// </summary>
    protected ParagraphFormat m_prFormat = null;
    /// <summary>
    /// The list format
    /// </summary>
    protected ListFormat m_listFormat = null;
    /// <summary>
    /// The paragraph items
    /// </summary>
    protected IParagraphItemCollection m_pItemColl = null;
    /// <summary>
    /// The paragra[h items with one empty item.
    /// </summary>
    private IParagraphItemCollection m_pEmptyItemColl = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets paragraph style name.
    /// </summary>
    public string StyleName
    {
      get
      {
        if( m_style == null )
        {
          return null;
        }
        return m_style.Name;
      }
    }
    /// <summary>
    /// Gets / sets paragraph text.
    /// </summary>
    ///<remarks>All internal formatting will be cleared when new text is set.</remarks>
    public string Text
    {
      get
      {
        return m_strTextBuilder.ToString();
      }
      set
      {
        //if( m_strText != value )
        //{
        m_pItemColl.Clear();
        AppendText( value );
        //}
      }
    }
    /// <summary>
    /// Gets paragraph item by index.
    /// </summary>
    new public IParagraphItem this[ int index ]
    {
      get
      {
        return m_pItemColl[ index ];
      }
    }
    /// <summary>
    /// Gets paragraph items count.
    /// </summary>
    public int ItemsCount
    {
      get
      {
        return m_pItemColl.Count;
      }
    }
    /// <summary>
    /// Gets paragraph format.
    /// </summary>
    public ParagraphFormat ParagraphFormat
    {
      get
      {
        return m_prFormat;
      }
    }
    /// <summary>
    /// Gets character format.
    /// </summary>
    public CharacterFormat CharacterFormat
    {
      get
      {
        return m_chFormat;
      }
    }
    /// <summary>
    /// Gets format of the list for the paragraph.
    /// </summary>
    public ListFormat ListFormat
    {
      get
      {
        return m_listFormat;
      }
    }
    /// <summary>
    /// Gets paragraph style
    /// </summary>
    internal IParagraphStyle Style
    {
      get
      {
        return m_style;
      }
    }
    /// <summary>
    /// Gets Items of the paragraph.
    /// </summary>
    protected IParagraphItemCollection Items
    {
      get
      {
        return m_pItemColl;
      }
    }

    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="doc"></param>
    public Paragraph( IDocument doc )
      : base( doc )
    {
      m_pItemColl = DocumentEx.CreateParagraphItemCollectionImpl( this );
      CreateEmptyParagraph();
      m_chFormat = DocumentEx.CreateCharacterFormatImpl();
      m_prFormat = DocumentEx.CreateParagraphFormatImpl();
      m_listFormat = DocumentEx.CreateListFormatImpl( this );
    }
    /// <summary>
    /// The copy constructor
    /// </summary>
    /// <param name="paragraph"></param>
    /// <param name="doc"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal Paragraph( IParagraph paragraph, IDocument doc )
      : this( doc )
    {
      // Copies paragraph text
      m_strTextBuilder = new StringBuilder( paragraph.Text );
      bool isStyleApplied = false;

      // Copyes paragraph items
      for( int i = 0; i < paragraph.ItemsCount; i++ )
      {
        IParagraphItem item = paragraph[ i ];
        ( m_pItemColl as ParagraphItemCollection
        ).AddWithoutUpdate( item.Clone( this ) );
      }
      
//      IParagraphStyle pStyle = paragraph.GetStyle();
//      if( pStyle != null )
//      {
//        IStyle foundStyle = doc.Styles.FindByName( pStyle.Name );
//
//
//        Document docEx = doc as Document;
//        // Export style object
//        if( foundStyle == null )
//        {
//          //          doc.Styles.Add( pStyle.Clone( doc ) );
//          CloneParagraphStyles( paragraph, doc, true );
//        }
//        else if( docEx.m_bIsImportingSection )
//        {
//          if( !CompareStyles( ( ParagraphStyle )foundStyle, ( ParagraphStyle )pStyle ) )
//          {
//            string newStyleName;
//            if( !docEx.m_stylesHistory.ContainsKey( pStyle.Name ) )
//            {
//              newStyleName = pStyle.Name + "_" + docEx.m_iStyleChangeIndex++;
//              docEx.m_stylesHistory.Add( pStyle.Name, newStyleName );
//            }
//            else
//            {
//              newStyleName = docEx.m_stylesHistory[ pStyle.Name ] as string;
//            }
//            ParagraphStyle clonedStyle = pStyle.Clone( doc ) as ParagraphStyle;
//            if( !clonedStyle.CharacterFormat.HasKey( CharacterFormat.FontSizeKey ) )
//            {
//              clonedStyle.CharacterFormat.FontSize = 10;
//            }
//            clonedStyle.Name = newStyleName;
//            doc.Styles.Add( clonedStyle );
//            ApplyStyle( newStyleName );
//            isStyleApplied = true;
//          }
//        }
//        else if( !CompareStyles( ( ParagraphStyle )foundStyle, ( ParagraphStyle )pStyle ))
//        {
//          CloneParagraphStyles( paragraph, doc, false );
//        }
//      }
      CloneParagraphStyle( paragraph as Paragraph, doc as Document );

      //Export ListStyles
      foreach( ListStyle lstStyle in paragraph.Owner.Document.ListStyles )
      {
        if( !HasStyle( lstStyle, doc ) )
        {
          doc.ListStyles.Add( lstStyle.Clone( doc ) );
        }
      }

      //Clone list override styles
      CloneParaLstOverrideStyles( paragraph.Owner.Document, doc );

      // Copyes formats
      m_chFormat.ImportContainer( paragraph.CharacterFormat );
      m_prFormat.ImportContainer( paragraph.ParagraphFormat );

      foreach( Tab tab in paragraph.ParagraphFormat.Tabs )
      {
        m_prFormat.Tabs.AddTab( ( Tab )tab.Clone() );
      }

      if( paragraph.ListFormat != null )
      {
        m_listFormat.ImportContainer( paragraph.ListFormat );
      }

      // Copyes style applying
//      if( !isStyleApplied && paragraph.StyleName != null )
//      {
//        ApplyStyle( paragraph.StyleName );
//      }
      if( !isStyleApplied && m_style != null )
      {
        ApplyBaseStyleFormats();
      }
    }
   
    #endregion

    #region Class public methods
    /// <summary>
    /// Applies the specified style.
    /// </summary>
    /// <param name="styleName">Style name</param>
    /// <remarks>Specified style must exist in Document.Styles collection</remarks>
    public void ApplyStyle( string styleName )
    {
      IParagraphStyle newStyle = Document.Styles.FindByName( styleName ) as IParagraphStyle;

      if( newStyle == null )
      {
        throw new ArgumentException(
          "Style with specified styleName does not exist or it doesn't support IParagraphStyle" );
      }

      m_style = newStyle;

      ApplyBaseStyleFormats();
    }
    /// <summary>
    /// Appends field to end of paragraph.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IField AppendField( DLSFieldType type )
    {
      IField field = AppendParagraphItem( ParagraphItemType.Field ) as IField;
      field.FieldType = type;
      //field.CharacterFormat.ApplyBase( m_chFormat );

      return field;
    }
    /// <summary>
    /// Appends text to end of document.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public ITextRange AppendText( string text )
    {
      ITextRange textRange = AppendParagraphItem( ParagraphItemType.TextRange ) as ITextRange;
      textRange.Text = text;
      //textRange.CharacterFormat.ApplyBase( m_chFormat );

      return textRange;
    }
    /// <summary>
    /// Appends image to end of paragraph.
    /// </summary>
    /// <returns></returns>
    public IPicture AppendPicture( Image image )
    {
      IPicture picture = AppendParagraphItem( ParagraphItemType.Picture ) as IPicture;
      picture.LoadImage( image );

      return picture;
    }
    /// <summary>
    /// Append Textbox to the end of the paragraph
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public ITextBox AppendTextBox( float width, float height )
    {
      ITextBox textbox = AppendParagraphItem( ParagraphItemType.TextBox ) as ITextBox;
      textbox.TextBoxFormat.Width = width;
      textbox.TextBoxFormat.Height = height;
      Document.TextBoxCollection.Add( textbox );
      return textbox;
    }
    /// <summary>
    /// Append canvas to end of paragraph.
    /// </summary>
    /// <returns></returns>
    public virtual ICanvas AppendCanvas( SizeF size )
    {
      ICanvas canvas = AppendParagraphItem( ParagraphItemType.Canvas ) as ICanvas;
      canvas.Size = size;

      return canvas;
    }
    /// <summary>
    /// Append table to end of paragraph.
    /// </summary>
    /// <returns></returns>
    public ITable AppendTable()
    {
      ITable table = AppendParagraphItem( ParagraphItemType.Table ) as ITable;
      return table;
    }
    /// <summary>
    /// Appends start of the bookmark with specified name into paragraph.
    /// </summary>
    public BookmarkStart AppendBookmarkStart( string name )
    {
      BookmarkStart bkmkStart = AppendParagraphItem( ParagraphItemType.BookmarkStart ) as BookmarkStart;
      bkmkStart.Name = name;

      // Add to bookmark collection
      Bookmark bookmark = new Bookmark( bkmkStart, bkmkStart );
      Document.Bookmarks.Add( bookmark );

      return bkmkStart;
    }
    /// <summary>
    /// Appends end of the bookmark with specified name into paragraph.
    /// </summary>
    public BookmarkEnd AppendBookmarkEnd( string name )
    {
      BookmarkEnd bkmkEnd = AppendParagraphItem( ParagraphItemType.BookmarkEnd ) as BookmarkEnd;
      bkmkEnd.Name = name;

      // Add to bookmark collection
      Document.Bookmarks.EndBookmark( bkmkEnd, name );

      return bkmkEnd;
    }
    /// <summary>
    /// Inserts paragraph item to specified position.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pItem"></param>
    public void InsertItem( int index, IParagraphItem pItem )
    {
      if( index < 0 || index > ItemsCount )
      {
        throw
          new ArgumentOutOfRangeException(
            "index", index, "Value can not be less 0 and greater " + ItemsCount );
      }

      if( pItem == null )
      {
        throw new ArgumentNullException( "item" );
      }

      m_pItemColl.Insert( index, pItem );
    }
    /// <summary>
    /// Removes specified paragraph item.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem( IParagraphItem item )
    {
      if( item == null )
      {
        throw new ArgumentNullException( "item" );
      }

      m_pItemColl.Remove( item );
    }
    /// <summary>
    /// Removes paragraph item at specified position.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveItemAt( int index )
    {
      if( index < 0 || index > ItemsCount )
      {
        throw
          new ArgumentOutOfRangeException(
            "index", index, "Value can not be less 0 and greater " + ItemsCount );
      }

      m_pItemColl.RemoveAt( index );
    }
    /// <summary>
    /// Gets index of specified paragraph item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOfItem( IParagraphItem item )
    {
      if( item == null )
      {
        throw new ArgumentNullException( "item" );
      }

      return m_pItemColl.IndexOf( item );
    }
    /// <summary>
    /// Gets related style.
    /// </summary>
    public IParagraphStyle GetStyle()
    {
      return m_style;
    }
    /// <summary>
    /// Clones self.
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public IParagraph Clone( IDocument doc )
    {
      return CloneImpl( doc );
    }
    /// <summary>
    /// Clone method implementation.
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    protected virtual IParagraph CloneImpl( IDocument doc )
    {
      return new Paragraph( this, doc );
    }
    /// <summary>
    /// Appends paragraph item to the end of paragraph.
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    protected virtual IParagraphItem AppendParagraphItem( ParagraphItemType itemType )
    {
      IParagraphItem item = Document.CreateParagraphItem( itemType );
      m_pItemColl.Add( item );
      return item;
    }
    /// <summary>
    /// Replaces all entries of given regular expression with replace string.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
    public int Replace( Regex pattern, string replace )
    {
      TextReplacer textReplacer = TextReplacer.Instance;
      return textReplacer.Replace( this, pattern, replace );
    }
    /// <summary>
    /// Replaces all entries of given string with replace string, taking into
    /// consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="replace"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    public int Replace( string given, string replace, bool caseSensitive, bool wholeWord )
    {
      TextReplacer textReplacer = TextReplacer.Instance;
      Regex pattern = StringToRegex( given, caseSensitive, wholeWord );

      return textReplacer.Replace( this, pattern, replace );
    }
    /// <summary>
    /// Returns first entry of given regex.
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public TextRangesHolder Find( Regex pattern )
    {
      TextFinder textFinder = TextFinder.Instance;
      ArrayList holders = textFinder.Find( this, pattern, true );
      return (holders != null && holders.Count > 0 ) ? ( TextRangesHolder )holders[ 0 ] : null;
    }
    /// <summary>
    /// Returns first entry of given string, taking into consideration caseSensitive
    /// and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    /// <returns></returns>
    public TextRangesHolder Find( string given, bool caseSensitive, bool wholeWord )
    {
      Regex pattern = StringToRegex( given, caseSensitive, wholeWord );

      return Find( pattern );
    }
    /// <summary>
    /// Replaces all entries of given regular expression with TextRangesHolder.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="rangesHolder"></param>
    /// <returns></returns>
    public void Replace( Regex pattern, TextRangesHolder rangesHolder )
    {
      TextHolderReplacer textReplacer = new TextHolderReplacer( this );
      textReplacer.Replace( pattern, rangesHolder );
    }
    /// <summary>
    /// Replaces all entries of given string with TextRangesHolder, taking into
    /// consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="rangesHolder"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    public void Replace( string given, TextRangesHolder rangesHolder, bool caseSensitive, bool wholeWord )
    {
      TextHolderReplacer textReplacer = new TextHolderReplacer( this );
      Regex pattern = StringToRegex( given, caseSensitive, wholeWord );
      textReplacer.Replace( pattern, rangesHolder );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <returns></returns>
    public IPicture AppendPicture( byte[] imageBytes )
    {
      IPicture picture = AppendParagraphItem( ParagraphItemType.Picture ) as IPicture;
      picture.LoadImage( imageBytes );

      return picture;
    }
    #endregion

    #region XDLSSerializableBase overrides
    /// <summary>
    /// Registers paragraph elements for xml serialization
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddRefElement( XDLSConstants.StyleItemTag, Style );
      XDLSHolder.AddElement( XDLSConstants.ParagraphFormatTag, m_prFormat );
      XDLSHolder.AddElement( XDLSConstants.CharacterFormatTag, m_chFormat );
      XDLSHolder.AddElement( XDLSConstants.ListFormatTag, ListFormat );
      XDLSHolder.AddElement( XDLSConstants.ItemsTag, m_pItemColl );
    }
    /// <summary>
    /// Restores object references after deserialization
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void RestoreReference( string name, int index )
    {
      if( name == XDLSConstants.StyleItemTag && index > -1 )
      {
        m_style = Document.Styles[ index ] as IParagraphStyle;
        //        if( m_style != null )
        //        {
        //          m_chFormat.ApplyBase( m_style.CharacterFormat );
        //          m_prFormat.ApplyBase( m_style.ParagraphFormat );
        //        }
        ApplyBaseStyleFormats();
      }
    }
#if DEBUG
    /// <summary>
    /// 
    /// </summary>
    protected override void DBG_WXC()
    {
      base.DBG_WXC();
    }
#endif
    #endregion

    #region WidgetContainer overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      base.DrawImpl( cg, ltWidget );

      bool isLineConatiner = ltWidget.ChildWidgets.Count > 0 && ltWidget.ChildWidgets[ 0 ].Widget == this;

      if( isLineConatiner )
      {
        ( cg as DLSGraphics ).DrawParagraph( this, ltWidget );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutParagraphInfoImpl( this );
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override ICollectionBase WidgetCollection
    {
      get
      {
        if( m_pItemColl.Count == 0 )
        {
          return m_pEmptyItemColl;
        }

        return m_pItemColl;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns first entry of given regex.
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    internal ArrayList FindAll( Regex pattern )
    {
      TextFinder textFinder = TextFinder.Instance;
      return textFinder.Find( this, pattern, false );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal string GetText( int startIndex, int length )
    {
      if( length != 0 )
      {
        if( startIndex < 0 || startIndex > m_strTextBuilder.Length - 1 )
        {
          throw new ArgumentOutOfRangeException( "startIndex", startIndex,
                                                 "Value can not be less 0 and greater " +
                                                 ( m_strTextBuilder.Length - 1 ) );
        }

        if( length < 0 || length > m_strTextBuilder.Length - startIndex )
        {
          throw
            new ArgumentOutOfRangeException(
              "length", length,
              "Value can not be less 0 and greater " + ( m_strTextBuilder.Length - startIndex ) );
        }
      }

      return m_strTextBuilder.ToString().Substring( startIndex, length );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pItem"></param>
    /// <param name="length"></param>
    /// <param name="newText"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal void UpdateText( ParagraphItem pItem, int length, string newText )
    {
      m_strTextBuilder.Remove( pItem.StartIndex, length );
      m_strTextBuilder.Insert( pItem.StartIndex, newText );

      // Corrects positions of next Holder Items
      int correctPos = newText.Length - length;
      int pItemIndex = m_pItemColl.IndexOf( pItem );

      if( pItemIndex < 0 )
      {
        throw new ArgumentException( "pItem haven't found in paragraph items" );
      }

      for( int i = pItemIndex + 1, len = m_pItemColl.Count; i < len; i++ )
      {
        ParagraphItem item = m_pItemColl[ i ] as ParagraphItem;

        if( item != null )
        {
          item.StartIndex += correctPos;
        }
      }
    }
    /// <summary>
    /// Replaces the substring.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="length">The length.</param>
    /// <param name="replacement">The replacement.</param>
    protected internal void ReplaceWithoutCorrection( int start, int length, string replacement )
    {
      int offset = replacement.Length - length;
      m_strTextBuilder.Remove( start, length );
      m_strTextBuilder.Insert( start, replacement );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="textRange"></param>
    /// <param name="index"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    public void UpdatePrevTextRange( ITextRange textRange, int index )
    {
      int trIndex = m_pItemColl.IndexOf( textRange );

      while( trIndex > 0 )
      {
        trIndex--;
        TextRange prevTRItem = m_pItemColl[ trIndex ] as TextRange;

        if( prevTRItem != null )
        {
          prevTRItem.TextLength = index - prevTRItem.StartIndex;
          break;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="given"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    private Regex StringToRegex( string given, bool caseSensitive, bool wholeWord )
    {
      given = Regex.Escape( given );

      if( wholeWord )
      {
        given = DEF_WHOLE_WORD_BEFORE + given + DEF_WHOLE_WORD_AFTER;
      }

      return new Regex( given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );
    }
    /// <summary>
    /// 
    /// </summary>
    protected void ApplyBaseStyleFormats()
    {
      if( m_style != null )
      {
        m_chFormat.ApplyBase( m_style.CharacterFormat );
        m_prFormat.ApplyBase( m_style.ParagraphFormat );

        foreach( ParagraphItem item in m_pItemColl )
        {
          TextRange range = item as TextRange;

          if( range != null )
          {
            range.CharacterFormat.ApplyBase( m_style.CharacterFormat );
          }
        }
      }
    }
    /// <summary>
    /// Checks if document already has current list style
    /// </summary>
    /// <param name="lstStyle"></param>
    /// <param name="doc"></param>
    /// <returns></returns>
    private bool HasStyle( ListStyle lstStyle, IDocument doc )
    {
      foreach( ListStyle listStyle in doc.ListStyles )
      {
        if( listStyle.Name == lstStyle.Name )
          return true;
      }
      return false;
    }
    /// <summary>
    /// Clones the paragraph styles.
    /// </summary>
    /// <param name="paragraph">The paragraph.</param>
    /// <param name="doc">The doc.</param>
//    private void CloneParagraphStyles( IParagraph paragraph, IDocument doc, bool isNewStyle )
//    {
//      IStyle pStyle = paragraph.GetStyle();
//      IStyle curStyle = pStyle.Clone( doc );
//      if( isNewStyle )
//      {
//        doc.Styles.Add( curStyle );          
//      }
//      else
//      {
//        curStyle.Name = curStyle.Name + "_"+ Guid.NewGuid().ToString();
//        doc.Styles.Add( curStyle );
//        ApplyStyle( curStyle.Name );
//      }
//      
//      if( ( pStyle as ParagraphStyle ).BaseStyle != null )
//      {
//        string baseStyleName = ( pStyle as ParagraphStyle ).m_baseStyleName;
//        IStyle baseStyle = paragraph.Owner.Document.Styles.FindByName( baseStyleName );
//        IStyle foundStyle = doc.Styles.FindByName( baseStyleName );
//
//        if( foundStyle == null )
//        {
//          doc.Styles.Add( baseStyle.Clone( doc ));
//        }
//        else if( !CompareStyles( (ParagraphStyle )foundStyle, ( ParagraphStyle )baseStyle ))
//        {
//          IStyle newBaseStyle = baseStyle.Clone( doc );
//          newBaseStyle.Name = newBaseStyle.Name + Guid.NewGuid().ToString(); 
//          doc.Styles.Add( newBaseStyle );
//          ( curStyle as ParagraphStyle ).m_baseStyle = newBaseStyle;
//          ( curStyle as ParagraphStyle ).m_baseStyleName = newBaseStyle.Name;
//        }
//      }
//    }
    /// <summary>
    /// Clones the paragraph style.
    /// </summary>
    /// <param name="sourcePara">The source para.</param>
    /// <param name="destDoc">The dest doc.</param>
    private void CloneParagraphStyle( Paragraph sourcePara, Document destDoc )
    {
      IParagraphStyle pStyle = sourcePara.GetStyle();
      if( sourcePara.Document.Equals( destDoc ))
      {
        m_style = pStyle;
      }
      else if( pStyle!= null )
      {
        string newStyleName = ( pStyle as ParagraphStyle ).ImportStyleTo( destDoc );
        m_style = ( IParagraphStyle )destDoc.Styles.FindByName( newStyleName );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="style1"></param>
    /// <param name="style2"></param>
    /// <returns></returns>
    private bool CompareStyles( ParagraphStyle style1, ParagraphStyle style2 )
    {
      //      if( style1.BaseStyle != style2.BaseStyle
      if( style1.CharacterFormat.PropertiesHash.Count != style2.CharacterFormat.PropertiesHash.Count
        || style1.ParagraphFormat.PropertiesHash.Count != style2.ParagraphFormat.PropertiesHash.Count )
      {
        return false;
      }
      IDictionaryEnumerator st1Enum = style1.CharacterFormat.PropertiesHash.GetEnumerator();
      while( st1Enum.MoveNext() )
      {
        object key = st1Enum.Key;
        object value = st1Enum.Value;
        if( style2.CharacterFormat.PropertiesHash[ key ] != null )
        {
          if( style2.CharacterFormat.PropertiesHash[ key ] != value )
            return false;
        }
        else
        {
          return false;
        }
      }
      st1Enum = style1.ParagraphFormat.PropertiesHash.GetEnumerator();
      while( st1Enum.MoveNext() )
      {
        object key = st1Enum.Key;
        object value = st1Enum.Value;
        if( style2.ParagraphFormat.PropertiesHash[ key ] != null )
        {
          if( style2.ParagraphFormat.PropertiesHash[ key ] != value )
            return false;
        }
        else
        {
          return false;
        }
      }
      return true;
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    private void CreateEmptyParagraph()
    {
      m_pEmptyItemColl = DocumentEx.CreateParagraphItemCollectionImpl( this );
      TextRange textRange = ( TextRange )Document.CreateParagraphItem(
        ParagraphItemType.TextRange );
      textRange.Text = " ";
      textRange.CharacterFormat.ApplyBase( m_chFormat );
      ( m_pEmptyItemColl as ParagraphItemCollection ).AddWithoutUpdate( textRange );
    }
    /// <summary>
    /// Clones the paragraph's list override styles.
    /// </summary>
    protected virtual void CloneParaLstOverrideStyles( IDocument sourceDocument, IDocument destDocument )
    {}
    #endregion

    #region Class internal declarations
    /// <summary>
    /// Class provides replacing method for the specified paragraph
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected class TextReplacer_old
    {
      #region Class members
      private Paragraph m_paragraph = null;
      private Regex m_pattern = null;
      private string m_concatString = "";
      private MatchCollection m_matches = null;
      private string m_replace = "";
      private ArrayList m_posArray = new ArrayList();
      #endregion

      #region Class initialize/finalize methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="paragraph"></param>
      public TextReplacer_old( Paragraph paragraph )
      {
        m_paragraph = paragraph;
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="pattern"></param>
      /// <param name="replace"></param>
      /// <returns></returns>
      public int Replace( Regex pattern, string replace )
      {
        m_pattern = pattern;
        m_replace = replace;

        int otherChanges = ConcatenateTextRanges();
        CorrectBoundaries();
        SplitTextRanges();

        return m_matches.Count + otherChanges;
      }
      #endregion

      #region Class helper methods
      /// <summary>
      /// 
      /// </summary>
      private void CorrectBoundaries()
      {
        //        int i;
        //        i = 0;
        //
        //        // Correct the boundaries of the substrings where needed
        //        m_matches = m_pattern.Matches( m_concatString );
        //
        //        foreach( Match match in m_matches )
        //        {
        //          int mLength = match.Value.Length;
        //          int index = match.Index;
        //
        //          bool bOnSep = false;
        //          foreach( int k in m_posArray )
        //          {
        //            if( ( index < k ) && ( k < index + mLength ) )
        //            {
        //              bOnSep = true;
        //              break;
        //            }
        //          }
        //
        //          // If we need to correct the position then do it
        //          if( bOnSep )
        //          {
        //            m_posArray[ i + 1 ] = index;
        //          }
        //          i++;
        //        }
        int i;
        i = 0;

        // Correct the boundaries of the substrings where needed
        m_matches = m_pattern.Matches( m_concatString );

        foreach( Match match in m_matches )
        {
          int mLength = match.Value.Length;
          int index = match.Index;
          int startRangeIndex = 0;
          int endRangeIndex = 0;

          bool bOnSep = false;
          //          for( int j = 0, count = m_posArray.Count; j < count; j++ )
          {
            startRangeIndex = FindTextRangeIndex( index );
            endRangeIndex = FindTextRangeIndex( index + mLength );

            if( startRangeIndex != endRangeIndex )
            {
              bOnSep = true;
              //              break;
            }
          }

          // If we need to correct the position then do it
          // Remove unneeded textranges and their positions
          if( bOnSep )
          {
            //m_posArray[ i + 1 ] = index;
            CarryPosition( index, true );
            CarryPosition( index + mLength, false );
            int diff = endRangeIndex - startRangeIndex;
            int startItem = startRangeIndex;
            if( diff > 1 )
            {
              while( diff > 1 )
              {
                if( !( m_paragraph.Items[ startItem ] is TextRange ) )
                {
                  //                  m_paragraph.Items.RemoveAt( startRangeIndex );
                  startItem++;
                  continue;
                }
                startRangeIndex++;
                startItem++;
                if( m_paragraph.Items[ startItem ] is TextRange )
                {
                  m_paragraph.Items.RemoveAt( startItem );
                  m_posArray.RemoveAt( startRangeIndex + 1 );
                  startRangeIndex--;
                  startItem--;
                }
                diff--;
              }
            }
          }
          i++;
        }
      }
      /// <summary>
      /// Concatenate strings and save their positions fot Text items 
      /// call Replace() method for tables
      /// </summary>
      /// <returns></returns>
      private int ConcatenateTextRanges()
      {
        int otherChanges = 0;
        m_posArray.Add( 0 );

        for( int j = 0, len = m_paragraph.ItemsCount; j < len; j++ )
        {
          IParagraphItem item = m_paragraph[ j ];
          TextRange text = item as TextRange;

          // For IWTextRange items
          if( item is TextRange )
          {
            m_posArray.Add( text.Text.Length + m_concatString.Length );
            m_concatString += text.Text;
          }
          else if( item is Table )
          {
            otherChanges += ( item as Table ).Replace( m_pattern, m_replace );
          }
        }
        return otherChanges;
      }
      /// <summary>
      /// Split concatenated string taking into consideration changed boundaries
      /// of the substrings
      /// </summary>
      private void SplitTextRanges()
      {
        string s;
        int i;
        i = 0;
        for( int j = 0, len = m_paragraph.ItemsCount; j < len; j++ )
        {
          ITextRange text = m_paragraph[ j ] as ITextRange;
          if( text != null )
          {
            s = m_concatString.Substring( ( int )m_posArray[ i ],
                                          ( int )m_posArray[ i + 1 ] - ( int )m_posArray[ i ] );
            text.Text = m_pattern.Replace( s, m_replace );
            i++;
          }
        }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="position"></param>
      /// <returns></returns>
      private int FindTextRangeIndex( int position )
      {
        for( int i = 0; i < m_posArray.Count - 1; i++ )
        {
          if( position >= ( int )m_posArray[ i ] && ( int )m_posArray[ i + 1 ] >= position )
          {
            return i;
          }
        }
        return -1;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="position"></param>
      /// <param name="left"></param>
      private void CarryPosition( int position, bool left )
      {
        if( left )
        {
          for( int i = 0; i < m_posArray.Count - 1; i++ )
          {
            if( ( int )m_posArray[ i ] > position )
            {
              m_posArray[ i ] = position;
              break;
            }
          }
        }
        else
        {
          for( int i = 0; i < m_posArray.Count - 1; i++ )
          {
            if( ( int )m_posArray[ i ] > position )
            {
              m_posArray[ i - 1 ] = position;
              break;
            }
          }
        }
      }
      #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected class TextReplacer
    {
      #region Properties
      [ThreadStatic]
      public static TextReplacer m_instance;
      /// <summary>
      /// Gets the instance.
      /// </summary>
      /// <value>The instance.</value>
      public static TextReplacer Instance
      {
        get
        {
          if( m_instance == null )
          {
            m_instance = new TextReplacer();
          }

          return m_instance;
        }
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Replaces the specified para.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="pattern">The pattern.</param>
      /// <param name="replacement">The replace.</param>
      /// <returns></returns>
      public int Replace( Paragraph para, Regex pattern, string replacement )
      {
        string text = para.Text;
        MatchCollection matches = pattern.Matches( text );

        if( matches.Count > 0 )
        {
          int offset = 0;
          int currOffset = 0;
          int repLength = replacement.Length;
          int mStart = 0;
          int mLength = 0;

          foreach( Match match in matches )
          {
            mStart = match.Index + offset;
            mLength = match.Length;
            currOffset = repLength - match.Length;

            para.ReplaceWithoutCorrection( mStart, mLength, replacement );
            // 1. find start range
            // 2. remove internal items
            // 3. correct last range start/length
            // 4. correct next items start
            TextRange tr;
            int startIndex = GetStartRangeIndex( para, mStart, out tr );
            int trEndIndex = tr.StartIndex + tr.TextLength;

            if( trEndIndex > mStart + mLength )
            {
              tr.TextLength += currOffset;
            }
            else
            {
              TextRange nextTr;
              RemoveInternalItems( para, mStart + mLength, startIndex + 1, out nextTr );
              int mEnd = mStart + mLength;

              if( nextTr != null )
              {
                nextTr.TextLength -= mEnd - nextTr.StartIndex;
                nextTr.StartIndex = mEnd + currOffset;
              }

              tr.TextLength = mEnd + currOffset - tr.StartIndex; 
              startIndex += 1;
            }
            
            CorrectNextItems( para, startIndex + 1, currOffset );
            offset += currOffset;
          }
        }

        return matches.Count + ReplaceInsideItems( para, pattern, replacement );
      }
      #endregion

      #region Implementation
      /// <summary>
      /// Corrects the first range.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="start"></param>
      /// <param name="tr"></param>
      /// <returns></returns>
      private int GetStartRangeIndex( Paragraph para, int start, out TextRange tr )
      {
        tr = null;
        int startIndex = 0;

        for( int i = 0, length = para.ItemsCount; i < length; i++ )
        {
          tr = para[ i ] as TextRange;

          if( tr != null && tr.StartIndex + tr.TextLength > start )
          {
            startIndex = i;
            break;
          }
        }

        return startIndex;
      }
      /// <summary>
      /// Removes the internal items.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="end"></param>
      /// <param name="startIndex"></param>
      /// <param name="nextTr"></param>
      private void RemoveInternalItems( Paragraph para, int end, int startIndex, out TextRange nextTr )
      {
        int nextTrEndIndex = 0;
        bool last = false;
        nextTr = null;

        for( int i = startIndex, length = para.ItemsCount; i < length; i++ )
        {
          nextTr = para[ i ] as TextRange;

          if( nextTr != null )
          {
            nextTrEndIndex = nextTr.StartIndex + nextTr.TextLength;

            if( nextTrEndIndex > end )
              break;
            else if( nextTrEndIndex == end )
              last = true;
          }

          ((ParagraphItemCollection)para.Items).UnsafeRemoveAt( i );

          if( last )
          {
            nextTr = ( i < para.ItemsCount ) ? para[ i ] as TextRange : null;
            break;
          }

          i--;
        }
      }
      /// <summary>
      /// Corrects the next items.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="startIndex">The start index.</param>
      /// <param name="offset">The offset.</param>
      private void CorrectNextItems( Paragraph para, int startIndex, int offset )
      {
        for( int i = startIndex, length = para.ItemsCount; i < length; i++ )
        {
          ParagraphItem item = para[ i ] as ParagraphItem;
          item.StartIndex += offset;
        }
      }
      /// <summary>
      /// Replaces the inside items.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="pattern">The pattern.</param>
      /// <param name="replacement">The replace.</param>
      private int ReplaceInsideItems( Paragraph para, Regex pattern, string replacement )
      {
        int changes = 0;

        for( int i = 0, length = para.ItemsCount; i < length; i++ )
        {
          Table table = para[ i ] as Table;

          if( table != null )
          {
            changes += table.Replace( pattern, replacement );
          }
        }

        return changes;
      }
      #endregion
    }

    /// <summary>
    /// Class provides replacing method for the specified paragraph
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    /// <summary>
    /// 
    /// </summary>
    internal class TextFinder
    {
      #region Properties
      [ThreadStatic]
      public static TextFinder m_instance;
      /// <summary>
      /// Gets the instance of TextReplacer.
      /// </summary>
      /// <value>The instance.</value>
      public static TextFinder Instance
      {
        get
        {
          if( m_instance == null )
          {
            m_instance = new TextFinder();
          }

          return m_instance;
        }
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Finds the text by specified pattern.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="pattern">The pattern.</param>
      /// <param name="onlyFirstMacth">if set to <c>true</c> [only first macth].</param>
      /// <returns></returns>
      public ArrayList Find( Paragraph para, Regex pattern, bool onlyFirstMacth )
      {
        string text = para.Text;
        MatchCollection matches = pattern.Matches( text );
        ArrayList holders = new ArrayList();

        if( matches.Count > 0 )
        {
          foreach( Match match in matches )
          {
            int mStart = match.Index;
            int mEnd = match.Index + match.Length;

            TextRange tr;
            int startIndex = GetStartRangeIndex( para, mStart, out tr );

            TextRangesHolder holder = new TextRangesHolder();
            holder.Add( tr );
            holder.StartCut = mStart - tr.StartIndex;

            while( tr.StartIndex + tr.TextLength < mEnd )
            {
              startIndex++;
              IParagraphItem pItem = para[ startIndex ];
              tr = pItem as TextRange;

              if( tr != null )
              {
                holder.Add( tr );
              }
            }

            holder.EndCut = tr.StartIndex + tr.TextLength - mEnd;
            holders.Add( holder );

            if( onlyFirstMacth )
            {
              break;
            }
          }
        }

        if( holders.Count == 0 || !onlyFirstMacth )
        {
          for (int i = 0, len = para.ItemsCount; i < len; i++)
			    {
            Table table  = para[ i ] as Table;
            if( table != null )
            {
              ArrayList list = table.FindAll( pattern );

              if( list != null && list.Count > 0 )
              {
                holders.AddRange( list );

                if( onlyFirstMacth )
                  break;
              }
            }
			    }
        }

        return holders;
      }
      /// <summary>
      /// Gets the start index of the range.
      /// </summary>
      /// <param name="para">The para.</param>
      /// <param name="start">The start.</param>
      /// <param name="tr">The tr.</param>
      /// <returns></returns>
      internal static int GetStartRangeIndex( Paragraph para, int start, out TextRange tr )
      {
        tr = null;
        int startIndex = 0;

        for( int i = 0, length = para.Items.Count; i < length; i++ )
        {
          tr = para[ i ] as TextRange;

          if( tr != null && tr.StartIndex + tr.TextLength >= start )
          {
            startIndex = i;
            break;
          }
        }

        return startIndex;
      }
      #endregion
    }
    #region /* commented */
    //    /// <summary>
    //    /// Class provides replacing method for the specified paragraph
    //    /// </summary>
    //    protected class TextFormatReplacer
    //    {
    //      #region Class constants
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      private const string DEF_WHOLE_WORD_BEFORE = @"(?<=^|\W)";
    //      private const string DEF_WHOLE_WORD_AFTER = @"(?=$|\W)";
    //      #endregion
    //      
    //      #region Class members
    //      private Paragraph m_paragraph = null;
    //      private Regex m_pattern = null;
    //      private string m_concatString = "";
    //      private MatchCollection m_matches = null;
    //      private string m_replace = "";
    //      private ArrayList m_posArray = new ArrayList();
    //      private CharacterFormat m_inputFormat = null;
    //      private CharacterFormat m_outputFormat = null;
    //      #endregion
    //
    //      #region Class initialize/finalize methods
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="paragraph"></param>
    //      public TextFormatReplacer( Paragraph paragraph )
    //      {
    //        m_paragraph = paragraph;
    //      }
    //      #endregion
    //
    //      #region Class public methods
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="pattern"></param>
    //      /// <param name="replace"></param>
    //      /// <returns></returns>
    //      public int Replace( Regex pattern, string replace, CharacterFormat inputFormat, CharacterFormat outputFormat )
    //      {
    //        m_pattern = pattern;
    //        m_replace = replace;
    //        m_inputFormat = inputFormat;
    //        m_outputFormat = outputFormat;
    //        
    //        int changes = ConcatenateTextRanges();
    //        
    //        CorrectBoundaries();
    //        
    //        SplitTextRanges();
    //        
    //        changes += MakeReplace();
    //
    //        return changes;
    //      }
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="given"></param>
    //      /// <param name="replace"></param>
    //      /// <param name="caseSensitive"></param>
    //      /// <param name="wholeWord"></param>
    //      /// <returns></returns>
    //      public int Replace( string given, string replace, bool caseSensitive, bool wholeWord, CharacterFormat inputFormat, CharacterFormat outputFormat   )
    //      {
    //        m_replace = replace;
    //        if( wholeWord )
    //        {
    //          given = DEF_WHOLE_WORD_BEFORE + Regex.Escape( given ) + DEF_WHOLE_WORD_AFTER;
    //        }
    //        Regex pattern = new Regex( given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );
    //        
    //        return Replace( pattern, replace, inputFormat, outputFormat );
    //      }
    //      #endregion
    //
    //      #region Class helper methods
    //      /// <summary>
    //      /// Concatenate strings and save their positions fot Text items 
    //      /// call Replace() method for tables
    //      /// </summary>
    //      /// <returns></returns>
    //      private int ConcatenateTextRanges()
    //      {
    //        int otherChanges = 0;
    //        m_posArray.Add( 0 );
    //
    //        for( int j = 0, len = m_paragraph.ItemsCount; j < len; j++ )
    //        {
    //          IParagraphItem item = m_paragraph[ j ];
    //          TextRange text = item as TextRange;
    //
    //          // For IWTextRange items
    //          if( item is TextRange )
    //          {
    //            m_posArray.Add( text.Text.Length + m_concatString.Length );
    //            m_concatString += text.Text;
    //          }
    //          else if( item is Table )
    //          {
    //            //otherChanges += ( item as Table ).ReplaceWithFormatting( m_pattern, m_replace, m_inputFormat, m_outputFormat );
    //          }
    //        }
    //        return otherChanges;
    //      }
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      private void CorrectBoundaries()
    //      {
    //        int i;
    //        i = 0;
    //        Hashtable hashtable = new Hashtable();
    //        // Correct the boundaries of the substrings where needed
    //        m_matches = m_pattern.Matches( m_concatString );
    //        
    //        foreach( Match match in m_matches )
    //        {
    //          int mLength = match.Value.Length;
    //          int index = match.Index;
    //        
    //          bool bOnSep = false;
    //          //foreach( int k in m_posArray )
    //          for( int j = 0, count = m_posArray.Count; j < count; j++ )
    //          {
    //            int k = ( int )m_posArray[ j ];
    //            if( ( index < k ) && ( k < index + mLength ) )
    //            {
    //              bOnSep = true;
    //              break;
    //            }
    //          }
    //        
    //          // If we need to correct the position then do it
    //          if( bOnSep )
    //          {
    //            ITextRange range1 = m_paragraph.Items[ i ] as ITextRange;
    //            ITextRange range2 = m_paragraph.Items[ i + 1 ] as ITextRange;
    //            if( range1.CharacterFormat.IsEqual( range2.CharacterFormat ) 
    //                && range1.CharacterFormat.IsEqual( m_inputFormat ))
    //            {
    //              m_posArray[ i + 1 ] = index;
    //              //m_posArray.Insert( i + 2, index + mLength );
    //              hashtable.Add( i + 2, index + mLength );
    //              ITextRange textRange = m_paragraph.Document.CreateParagraphItem( ParagraphItems.TextRange ) as ITextRange;
    //              textRange.CharacterFormat.ImportContainer( range1.CharacterFormat );
    //              m_paragraph.InsertItem( i + 1, textRange );
    //            }
    //          }
    //          i++;
    //        }
    //        foreach( DictionaryEntry entry in hashtable )
    //        {
    //          m_posArray.Insert( ( int )entry.Key, (int)entry.Value );
    //        }
    //      }
    //      /// <summary>
    //      /// Split concatenated string taking into consideration changed boundaries
    //      /// of the substrings
    //      /// </summary>
    //      private void SplitTextRanges()
    //      {
    //        // Stick strings back after the correction of boundaries
    //        string s;
    //        int i;
    //        i = 0;
    //        for( int j = 0, len = m_paragraph.ItemsCount; j < len; j++ )
    //        {
    //          ITextRange text = m_paragraph[ j ] as ITextRange;
    //          if( text != null )
    //          {
    //            s = m_concatString.Substring( ( int )m_posArray[ i ],
    //              ( int )m_posArray[ i + 1 ] - ( int )m_posArray[ i ] );
    //            text.Text = s;//m_pattern.Replace( s, m_replace );
    //            i++;
    //          }
    //        }
    //        
    //        // Split textRanges if they have any matches of the given regex
    //        Hashtable positionsHash;
    //        bool bFirstPass = true;
    //        
    //        for( int k = 0; k < m_paragraph.ItemsCount; k++ )
    //        {
    //          ITextRange textRange = m_paragraph.Items[ k ] as ITextRange;
    //          if( textRange != null )
    //          {
    //            if( m_pattern.IsMatch( textRange.Text ))
    //            {
    //              positionsHash = new Hashtable();
    //              MatchCollection matches = m_pattern.Matches( textRange.Text );
    //              if( textRange.CharacterFormat.IsEqual( m_inputFormat ))
    //              {
    //                foreach( Match match in matches )
    //                {
    //                  if( bFirstPass )
    //                  {
    //                    if( match.Index != 0 )
    //                    {
    //                      positionsHash.Add( 0, match.Index );
    //                    }
    //                    bFirstPass = false;
    //                  }
    //                  positionsHash.Add( match.Index, match.Length );
    //                }
    //                SplitTextRange( ref k, positionsHash );
    //              }
    //            }
    //          }
    //        }
    //      }
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <returns></returns>
    //      private int MakeReplace()
    //      {
    //        int changesMade = 0;
    //        
    //        foreach( ITextRange textRange in m_paragraph.Items )
    //        {
    //          if( textRange.CharacterFormat.IsEqual( m_inputFormat ))
    //          {
    //            int count = m_pattern.Matches( textRange.Text ).Count;
    //            textRange.Text = m_pattern.Replace( textRange.Text, m_replace );
    //            if( count != 0 )
    //            {
    //              textRange.CharacterFormat.ImportContainer( m_outputFormat );
    //              changesMade += count;
    //            }
    //          }
    //        }
    //        
    //        return changesMade;
    //      }
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="rangeIndex"></param>
    //      /// <param name="positions"></param>
    //      private void SplitTextRange( ref int rangeIndex, Hashtable positions )
    //      {
    //        ITextRange textRange = m_paragraph.Items[ rangeIndex ] as ITextRange;
    //        string text = textRange.Text;
    //        bool bFirstPass = true;
    //        int position = 0;
    //        int length = 0;
    //        
    //        foreach( DictionaryEntry entry in positions )
    //        {
    //          position = ( int )entry.Key;
    //          length = ( int )entry.Value;
    //          if( bFirstPass )
    //          {
    //            if( position != 0 )
    //            {
    //              textRange.Text = text.Substring( 0, position );
    //            }
    //            else
    //            {
    //              textRange.Text = text.Substring( position, length );
    //              continue;
    //            }
    //            bFirstPass = false;
    //          }
    //          if( length != text.Length )
    //          {
    //            InsertNewTextRange( text.Substring( position, length ), textRange, ref rangeIndex );
    //          }
    //        }
    //        
    //        // If there are any characters left in the input textRange
    //        string endingString = text.Substring( position + length );
    //        if( endingString != "" )
    //        {
    //          InsertNewTextRange( endingString, textRange, ref rangeIndex );
    //        }
    //      }
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="text"></param>
    //      /// <param name="textRange"></param>
    //      /// <param name="rangeIndex"></param>
    //      private void InsertNewTextRange( string text, ITextRange textRange, ref int rangeIndex )
    //      {
    //        ITextRange newTextRange;
    //        newTextRange = m_paragraph.Document.CreateParagraphItem( ParagraphItems.TextRange ) as ITextRange;
    //        newTextRange.Text = text;
    //        newTextRange.CharacterFormat.ImportContainer( textRange.CharacterFormat );
    //        m_paragraph.InsertItem( rangeIndex + 1, newTextRange );
    //        rangeIndex++;
    //      }
    //      
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="position"></param>
    //      /// <returns></returns>
    //      private int FindTextRangeIndex( int position )
    //      {
    //        for( int i = 0; i < m_posArray.Count - 1; i++ )
    //        {
    //          if( position > ( int )m_posArray[ i ] && ( int )m_posArray[ i + 1 ] > position )
    //          {
    //            return i;
    //          }
    //        }
    //        return -1;
    //      }
    //      /// <summary>
    //      /// 
    //      /// </summary>
    //      /// <param name="position"></param>
    //      /// <param name="left"></param>
    //      private void CarryPosition( int position, bool left )
    //      {
    //        if( left )
    //        {
    //          for( int i = 0; i < m_posArray.Count; i++ )
    //          {
    //            if( ( int )m_posArray[i] > position )
    //            {
    //              m_posArray[ i ] = position;
    //              break;
    //            }
    //          }
    //        }
    //        else
    //        {
    //          for( int i = 0; i < m_posArray.Count; i++ )
    //          {
    //            if( ( int )m_posArray[i] > position )
    //            {
    //              m_posArray[ i - 1 ] = position;
    //              break;
    //            }
    //          }
    //        }
    //      }
    //      
    //      #endregion
    //    }
    #endregion
    /// <summary>
    /// Class provides replacing method for the specified paragraph
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected class TextHolderReplacer
    {
      #region Class members
      private Paragraph m_paragraph = null;
      private Regex m_pattern = null;
      private string m_concatString = "";
      private MatchCollection m_matches = null;
      private TextRangesHolder m_rangesHolder = null;
      private ArrayList m_posArray = new ArrayList();
      #endregion

      #region Class initialize/finalize methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="paragraph"></param>
      public TextHolderReplacer( Paragraph paragraph )
      {
        m_paragraph = paragraph;
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="pattern"></param>
      /// <param name="rangesHolder"></param>
      /// <returns></returns>
      public int Replace( Regex pattern, TextRangesHolder rangesHolder )
      {
        m_pattern = pattern;
        m_rangesHolder = rangesHolder;

        int otherChanges = ConcatenateTextRanges();
        CorrectBoundaries();
        SplitTextRanges();
        MakeReplace();

        return m_matches.Count + otherChanges;
      }
      #endregion

      #region Class helper methods
      /// <summary>
      /// Concatenate strings and save their positions fot Text items 
      /// call Replace() method for tables
      /// </summary>
      /// <returns></returns>
      private int ConcatenateTextRanges()
      {
        int otherChanges = 0;
        m_posArray.Add( 0 );

        for( int j = 0, len = m_paragraph.ItemsCount; j < len; j++ )
        {
          IParagraphItem item = m_paragraph[ j ];
          TextRange text = item as TextRange;

          // For IWTextRange items
          if( item is TextRange )
          {
            m_posArray.Add( text.Text.Length + m_concatString.Length );
            m_concatString += text.Text;
          }
          else if( item is Table )
          {
            //otherChanges += ( item as Table ).ReplaceWithFormatting( m_pattern, m_replace, m_inputFormat, m_outputFormat );
          }
        }
        return otherChanges;
      }
      /// <summary>
      /// 
      /// </summary>
      private void CorrectBoundaries()
      {
        int i;
        i = 0;

        // Correct the boundaries of the substrings where needed
        m_matches = m_pattern.Matches( m_concatString );

        foreach( Match match in m_matches )
        {
          int mLength = match.Value.Length;
          int index = match.Index;
          int startRangeIndex = 0;
          int endRangeIndex = 0;

          bool bOnSep = false;
          startRangeIndex = FindTextRangeIndex( index );
          endRangeIndex = FindTextRangeIndex( index + mLength );

          if( startRangeIndex != endRangeIndex )
          {
            bOnSep = true;
            //              break;
          }

          // If we need to correct the position then do it
          // Remove unneeded textranges and their positions
          if( bOnSep )
          {
            //m_posArray[ i + 1 ] = index;
            CarryPosition( index, true );
            CarryPosition( index + mLength, false );
            int diff = endRangeIndex - startRangeIndex;
            if( diff > 2 )
            {
              while( diff > 2 )
              {
                startRangeIndex++;
                if( m_paragraph.Items[ startRangeIndex ] is TextRange )
                {
                  m_paragraph.Items.RemoveAt( startRangeIndex );
                  m_posArray.RemoveAt( startRangeIndex + 1 );
                }
                diff--;
              }
            }
          }
          i++;
        }
      }
      /// <summary>
      /// Split concatenated string taking into consideration changed boundaries
      /// of the substrings
      /// </summary>
      private void SplitTextRanges()
      {
        // Stick strings back after the correction of boundaries
        string s;
        int i;
        i = 0;
        for( int j = 0, len = m_paragraph.ItemsCount; j < len; j++ )
        {
          ITextRange text = m_paragraph[ j ] as ITextRange;
          if( text != null )
          {
            s = m_concatString.Substring( ( int )m_posArray[ i ],
              ( int )m_posArray[ i + 1 ] - ( int )m_posArray[ i ] );
            text.Text = s;//m_pattern.Replace( s, m_replace );
            i++;
          }
        }

        // Split textRanges if they have any matches of the given regex
        SortedList positionsHash;
        bool bFirstPass = true;

        for( int k = 0; k < m_paragraph.ItemsCount; k++ )
        {
          ITextRange textRange = m_paragraph.Items[ k ] as ITextRange;
          if( textRange != null )
          {
            if( m_pattern.IsMatch( textRange.Text ) )
            {
              positionsHash = new SortedList();
              MatchCollection matches = m_pattern.Matches( textRange.Text );

              foreach( Match match in matches )
              {
                if( bFirstPass )
                {
                  if( match.Index != 0 )
                  {
                    positionsHash.Add( 0, match.Index );
                  }
                  bFirstPass = false;
                }
                positionsHash.Add( match.Index, match.Length );
              }
              SplitTextRange( ref k, positionsHash );

            }
          }
        }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private void MakeReplace()
      {
        //        foreach( ITextRange textRange in m_paragraph.Items )
        for( int i = 0; i < m_paragraph.ItemsCount; i++ )
        {
          ITextRange textRange = m_paragraph.Items[ i ] as TextRange;
          if( textRange != null )
          {
            int count = m_pattern.Matches( textRange.Text ).Count;
            //textRange.Text = m_pattern.Replace( textRange.Text, m_replace );
            if( count != 0 )
            {
              int rangeIndex = m_paragraph.IndexOfItem( textRange );
              m_paragraph.RemoveItemAt( rangeIndex );
              m_rangesHolder.CopyTo( m_paragraph, rangeIndex );
              if( m_rangesHolder.Count > 0 )
              {
                i += m_rangesHolder.Count - 1;
              }
              continue;
            }
          }
        }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="rangeIndex"></param>
      /// <param name="positions"></param>
      private void SplitTextRange( ref int rangeIndex, SortedList positions )
      {
        ITextRange textRange = m_paragraph.Items[ rangeIndex ] as ITextRange;
        string text = textRange.Text;
        bool bFirstPass = true;
        int position = 0;
        int length = 0;

        foreach( DictionaryEntry entry in positions )
        {
          position = ( int )entry.Key;
          length = ( int )entry.Value;
          if( bFirstPass )
          {
            if( position != 0 )
            {
              textRange.Text = text.Substring( 0, position );
            }
            else
            {
              textRange.Text = text.Substring( position, length );
              continue;
            }
            bFirstPass = false;
          }
          if( length != text.Length )
          {
            InsertNewTextRange( text.Substring( position, length ), textRange, ref rangeIndex );
          }
        }

        // If there are any characters left in the input textRange
        string endingString = text.Substring( position + length );
        if( endingString != "" )
        {
          InsertNewTextRange( endingString, textRange, ref rangeIndex );
        }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="text"></param>
      /// <param name="textRange"></param>
      /// <param name="rangeIndex"></param>
      private void InsertNewTextRange( string text, ITextRange textRange, ref int rangeIndex )
      {
        ITextRange newTextRange;
        newTextRange = m_paragraph.Document.CreateParagraphItem( ParagraphItemType.TextRange ) as ITextRange;
        newTextRange.Text = text;
        newTextRange.CharacterFormat.ImportContainer( textRange.CharacterFormat );
        m_paragraph.InsertItem( rangeIndex + 1, newTextRange );
        rangeIndex++;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="position"></param>
      /// <returns></returns>
      private int FindTextRangeIndex( int position )
      {
        for( int i = 0; i < m_posArray.Count - 1; i++ )
        {
          if( position >= ( int )m_posArray[ i ] && ( int )m_posArray[ i + 1 ] >= position )
          {
            return i;
          }
        }
        return -1;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="position"></param>
      /// <param name="left"></param>
      private void CarryPosition( int position, bool left )
      {
        for( int i = 0; i < m_posArray.Count; i++ )
        {
          if( ( int )m_posArray[ i ] > position )
          {
            int index = left ? i : i - 1;
            if( ( int )m_posArray[ i ] > position )
            {
              m_posArray[ index ] = position;
            }
            break;
          }
        }
      }
      #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected class LayoutParagraphInfoImpl : LayoutParagraphInfo
    {
      #region Members
      /// <summary>
      /// 
      /// </summary>
      private Paragraph m_paragraph;
      #endregion

      #region Properties
      /// <summary>
      /// 
      /// </summary>
      protected IDocument Document
      {
        get
        {
          return m_paragraph.Document;
        }
      }
      #endregion

      #region Constructors
      /// <summary>
      /// 
      /// </summary>
      public LayoutParagraphInfoImpl( Paragraph paragraph )
        :
        base( ChildrenLayoutDirection.Horizontal )
      {
        m_bIsLineContainer = true;
        m_paragraph = paragraph;
        InitFormat();
        InitPageBreaks();
        InitListFormat();
        InitBorders();
      }
      #endregion

      #region Implementation
      /// <summary>
      /// Determines the list format.
      /// </summary>
      private void InitListFormat()
      {
        ListFormat listFormat = m_paragraph.ListFormat;
        if( listFormat.ListType != ListType.NoList )
        {
          m_strListStyleName = m_paragraph.ListFormat.CurrentListStyle.Name;
          int levelNumber = listFormat.ListLevelNumber;
          ListStyle listStyle = Document.ListStyles.FindByName( m_paragraph.ListFormat.CustomStyleName );
          ListLevel level = listStyle.GetNearLevel( levelNumber );

          if( Margins.Left == 0 )
          {
            Margins.Left = level.TextPosition;
          }

          m_levelNumber = listFormat.ListLevelNumber;

          if( listFormat.ListType == ListType.Numbered )
          {
            m_listRestart = listFormat.RestartNumbering;
          }

          ListTabs tabs = new ListTabs( m_paragraph );
          m_listType = ( byte )listFormat.ListType;
          double pos = Margins.Left + m_firstLineIndent;

          if( level.FollowCharacter == FollowCharacterType.Tab )
          {
            if( m_paragraph.ParagraphFormat.Tabs.Count > 0 )
            {
              if( m_paragraph.ParagraphFormat.Tabs[ 0 ].Justification == TabJustification.List )
              {
                m_listTab = m_paragraph.ParagraphFormat.Tabs[ 0 ].Position;
                m_listTab -= ( float )pos;
              }
            }
            else if( level.TabSpaceAfter > 0 )
            {
              m_listTab = level.TabSpaceAfter;
              m_listTab -= ( float )pos;

              if( m_listTab < 0 )
              {
                float thisTab = ( float )tabs.GetNextTabPosition( pos + m_listTab );
                m_listTab = m_listTab + thisTab;
              }
            }
            else
            {
              m_listTab = ( float )tabs.GetNextTabPosition( pos );
            }

            float max = Math.Max( Math.Abs( m_paragraph.ParagraphFormat.FirstLineIndent ),
                                  Math.Abs( level.NumberPosition ) );

            if( max == m_firstLineIndent && m_listTab == 0 )
            {
              m_listTab = ( float )tabs.GetNextTabPosition( pos );
            }
          }
        }
      }
      /// <summary>
      /// Determines the borders.
      /// </summary>
      private void InitBorders()
      {
        Borders borders = m_paragraph.ParagraphFormat.Borders;

        if( !borders.NoBorder )
        {
          Paddings.Left += borders.Left.Space;
          Paddings.Right += borders.Right.Space;
          Paddings.Top += borders.Top.Space;
          Paddings.Bottom += borders.Bottom.Space;
        }
      }
      /// <summary>
      /// Determines the page breaks.
      /// </summary>
      private void InitPageBreaks()
      {
        ISection secOwner = m_paragraph.Owner as ISection;
        bool sectionPageBreak = false;
        bool pageBreakBefore = false;

        //foreach( IParagraphItem item in m_paragraph.Items )
        //{
        //  if( ( item as PageBreak ) != null )
        //  {
        //    m_bPageBreakItem = true;
        //  }
        //  else
        //  {
        //    m_bPageBreakItem = false;
        //  }
        //}

        if( secOwner != null )
        {
          bool bLastParagraph = ( secOwner.Paragraphs.IndexOf( m_paragraph ) == secOwner.Paragraphs.Count - 1 );
          int secIndex = Document.Sections.IndexOf( secOwner );
          ISection nextSection = ( secIndex + 1 < m_paragraph.Document.Sections.Count )
            ? Document.Sections[ secIndex + 1 ]
            : null;

          if( nextSection != null )
          {
            sectionPageBreak =
              bLastParagraph &&
              nextSection.BreakCode == SectionBreakCode.NewPage;
          }

          if( !bLastParagraph )
          {
            int pIndex = secOwner.Paragraphs.IndexOf( m_paragraph );
            pageBreakBefore = secOwner.Paragraphs[ pIndex + 1 ].ParagraphFormat.PageBreakBefore;
          }
        }

        ParagraphFormat pFormat = m_paragraph.ParagraphFormat;
        m_isPageBreak = pFormat.PageBreakAfter || pageBreakBefore ||
                        pFormat.ColumnBreakAfter || sectionPageBreak;
      }
      /// <summary>
      /// Determines the format.
      /// </summary>
      private void InitFormat()
      {
        ParagraphFormat parFormat = m_paragraph.ParagraphFormat;
        Margins.Left = parFormat.LeftIndent;
        Margins.Top = parFormat.BeforeSpacing;
        Margins.Bottom = parFormat.AfterSpacing;
        Margins.Right = parFormat.RightIndent;
        m_isKeepTogether = parFormat.Keep;
        m_isKeepWithNext = parFormat.KeepFollow;
        m_firstLineIndent = parFormat.FirstLineIndent;
        m_justification =
          ( Layouting.HorizontalAlignment )parFormat.HorizontalAlignment;

        if( parFormat.Bidi )
        {
          if( m_justification == Syncfusion.Layouting.HorizontalAlignment.Left )
            m_justification = Syncfusion.Layouting.HorizontalAlignment.Right;
          else if( m_justification == Syncfusion.Layouting.HorizontalAlignment.Right )
            m_justification = Syncfusion.Layouting.HorizontalAlignment.Left;
        }
      }
      #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    internal class ListTabs
      : LayoutTabsInfo
    {
      public ListTabs( Paragraph paragrath )
        : base( ChildrenLayoutDirection.Horizontal )
      {
        //float pageMarginLeft = paragrath.Document.LastSection.PageSetup.Margins.Left;
        m_defaultTabWidth = paragrath.Document.LastSection.PageSetup.DefaultTabWidth;
        //m_pageMarginLeft = pageMarginLeft;

        for( int i = 0, size = paragrath.ParagraphFormat.Tabs.Count; i < size; i++ )
        {
          Tab tab = paragrath.ParagraphFormat.Tabs[ i ];
          AddTab(
            tab.Position,//+ pageMarginLeft,
            ( Layouting.TabJustification )tab.Justification,
            ( Layouting.TabLeader )tab.TabLeader );
        }
      }
    }
    #endregion
  }
}
