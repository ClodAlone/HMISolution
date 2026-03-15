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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
using Syncfusion.DLS;

#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a DLS document. 
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class Document
    : WidgetContainer,
      IDocument,
      IXmlSerializable,
      IWidgetContainer
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_NORMAL_STYLE = "Normal";
    /// <summary>
    /// 
    /// </summary>
    protected internal const string DEF_BULLETS_STYLE = "Bulleted";
    /// <summary>
    /// 
    /// </summary>
    protected internal const string DEF_NUMBERING_STYLE = "Numbered";
    //    private const string DEF_WHOLE_WORD_BEFORE = @"(?<=^|\W)";
    //    private const string DEF_WHOLE_WORD_AFTER = @"(?=$|\W)";
    #endregion

    #region Class members
    /// <summary>
    /// Image that represents the background image of a document
    /// </summary>
    private Image m_backgrndImage = null;
    /// <summary>
    /// Collection of document sections
    /// </summary>
    protected ISectionCollection m_sections;
    /// <summary>
    /// Collection of document styles
    /// </summary>
    protected IStyleCollection m_styles;
    /// <summary>
    /// Collection of list styles
    /// </summary>
    protected ListStyleCollection m_listStyles;
    /// <summary>
    /// 
    /// </summary>
    private BookmarkCollection m_bookmarks = null;
    /// <summary>
    /// Collection of textboxes
    /// </summary>
    private TextBoxCollection m_txbxItems = null;
    /// <summary>
    /// 
    /// </summary>
    internal Hashtable m_stylesHistory = null;
    internal bool m_bIsImportingSection = false;
    internal Section CurClonedSection;
    internal int m_iStyleChangeIndex = 1;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets image that represents background the image of a document.
    /// </summary>
    public Image BackgroundImage
    {
      get
      {
        if(m_backgrndImage == null )
        {
          m_backgrndImage = GetBackGndImage();
        }
        return m_backgrndImage;
      }
      set
      {
        OnSetBackgroundImage();
        m_backgrndImage = value;
      }
    }
    /// <summary>
    /// Gets document sections.
    /// </summary>
    public ISectionCollection Sections
    {
      get
      {
        return m_sections;
      }
    }
    /// <summary>
    /// Gets document styles.
    /// </summary>
    public IStyleCollection Styles
    {
      get
      {
        return m_styles;
      }
    }
    /// <summary>
    /// Gets document list styles.
    /// </summary>
    public ListStyleCollection ListStyles
    {
      get
      {
        return m_listStyles;
      }
    }
    /// <summary>
    /// Gets document bookmarks.
    /// </summary>
    public BookmarkCollection Bookmarks
    {
      get
      {
        if( m_bookmarks == null )
        {
          m_bookmarks = new BookmarkCollection( this );
        }
        return m_bookmarks;
      }
    }
    /// <summary>
    /// Gets last section of the document.
    /// </summary>
    public ISection LastSection
    {
      get
      {
        if( Sections.Count > 0 )
        {
          return Sections[ Sections.Count - 1 ];
        }

        return null;
      }
    }
    /// <summary>
    /// Gets last paragraph of last section.
    /// </summary>
    public IParagraph LastParagraph
    {
      get
      {
        if( LastSection != null )
        {
          int count = LastSection.Paragraphs.Count;

          if( count > 0 )
          {
            return LastSection.Paragraphs[ count - 1 ];
          }
        }
        return null;
      }
    }
    /// <summary>
    /// Get/set textbox items of main document
    /// </summary>
    public TextBoxCollection TextBoxCollection
    {
      get
      {
        return m_txbxItems;
      }
      set
      {
        m_txbxItems = value;
      }
    }
    #endregion

    #region Class initialize / finalize methods
    /// <summary>
    /// Creates document instance.
    /// </summary>
    public Document()
    {
      DocumentEx = this;
      m_sections = CreateSectionCollectionImpl();
      m_styles = CreateStyleCollectionImpl();
      m_listStyles = new ListStyleCollection( this );
      EnsureListStyles();
      m_txbxItems = CreateTextBoxCollectionImpl();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Document"/> class.
    /// </summary>
    /// <param name="ensureMinimal">if set to <c>true</c> [ensure minimal].</param>
    public Document( bool ensureMinimal )
      : this()
    {
      if( ensureMinimal )
        EnsureMinimal();
    }
    /// <summary>
    /// Make deep copy data from specified document.
    /// </summary>
    /// <param name="doc"></param>
    protected Document( IDocument doc )
      : this()
    {
      ImportContent( doc );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds one section and one paragraph to the document.
    /// </summary>
    public void EnsureMinimal()
    {
      if( Sections.Count == 0 )
      {
        AddSection().AddParagraph();
      }
    }
    /// <summary>
    /// Adds new section to document.
    /// </summary>
    /// <returns></returns>
    public ISection AddSection()
    {
      ISection sec = CreateSectionImpl();
      m_sections.Add( sec );
      return sec;
    }
    /// <summary>
    /// Adds new paragraph style to the document.
    /// </summary>
    /// <param name="styleName">Paragraph style name</param>
    /// <returns></returns>
    public IParagraphStyle AddParagraphStyle( string styleName )
    {
      return AddStyle( DLS.StyleType.ParagraphStyle, styleName ) as IParagraphStyle;
    }
    /// <summary>
    /// Adds new list style to document.
    /// </summary>
    /// <param name="listType">List type</param>
    /// <param name="styleName">Paragraph style name</param>
    /// <returns></returns>
    public ListStyle AddListStyle( ListType listType, string styleName )
    {
      ListStyle style = new ListStyle( this, listType );
      ListStyles.Add( style );
      style.Name = styleName;
      return style;
    }
    /// <summary>
    /// Adds new style to document.
    /// </summary>
    /// <param name="styleType">Style type</param>
    /// <param name="styleName">Style name</param>
    /// <returns></returns>
    public IStyle AddStyle( StyleType styleType, string styleName )
    {
      IStyle style = CreateStyleImpl( styleType );

      if( style != null )
      {
        style.Name = styleName;
        m_styles.Add( style );
      }

      return style;
    }
    /// <summary>
    /// Opens the document from xml format file.
    /// </summary>
    /// <param name="fileName">The name of file</param>
    public void OpenXml( string fileName )
    {
      using( FileStream stream = new FileStream( fileName, FileMode.Open ) )
      {
        OpenXml( stream );
      }
    }
    /// <summary>
    /// Opens the XML document from stream.
    /// </summary>
    /// <param name="stream">The stream object</param>
    public void OpenXml( Stream stream )
    {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      XmlTextReader reader = new XmlTextReader( stream );
#elif SyncfusionFramework2_0
      // Set the validation settings.
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.ValidationType = ValidationType.Schema;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
      settings.ValidationEventHandler += new ValidationEventHandler( ValidationCallBack );
      settings.CheckCharacters = false;
      settings.Schemas.Add( GetSchema() );

       //Create the XmlReader object.
      XmlReader reader = XmlReader.Create( stream, settings );
 
#endif
      ReadXml( reader );
    }
    // Display any warnings or errors.
    private static void ValidationCallBack( object sender, ValidationEventArgs args )
    {
      if( args.Severity == XmlSeverityType.Warning )
        Debug.WriteLine( "\tWarning: Matching schema not found.  No validation occurred." + args.Message );
      else
      {
        string errmess = "\tValidation error: " + args.Message;
        Debug.WriteLine( errmess );
        throw new XDLSException( errmess );
      }

    }
    /// <summary>
    /// Saves the document in xml format.
    /// </summary>
    /// <param name="fileName">The name of target file</param>
    public void SaveXml( string fileName )
    {
      XmlTextWriter writer = new XmlTextWriter( fileName, Encoding.Unicode );
      writer.Formatting = Formatting.Indented;
      WriteXml( writer );
      writer.Close();
    }
    /// <summary>
    /// Saves the document in xml format.
    /// </summary>
    /// <param name="stream">The target stream</param>
    public void SaveXml( Stream stream )
    {
      XmlTextWriter writer = new XmlTextWriter( stream, Encoding.Unicode );

      try
      {
        writer.Formatting = Formatting.Indented;
        WriteXml( writer );
      }
      finally
      {
        writer.Flush();
      }
    }
    /// <summary>
    /// Saves the document in text format.
    /// </summary>
    /// <param name="fileName">The name of target file</param>
    public void SaveTxt( string fileName )
    {
      StreamWriter writer = new StreamWriter( fileName );
      TextConverter textConverter = new TextConverter();
      textConverter.Write( writer, this );
      writer.Close();
    }
    /// <summary>
    /// Saves the document in text format.
    /// </summary>
    /// <param name="stream">The name of target file</param>
    public void SaveTxt( Stream stream )
    {
      StreamWriter writer = new StreamWriter( stream );
      try
      {
        TextConverter textConverter = new TextConverter();
        textConverter.Write( writer, this );
      }
      finally
      {
        writer.Flush();
      }
    }
    /// <summary>
    /// Gets the document's text.
    /// </summary>
    public string GetText()
    {
      TextConverter textConverter = new TextConverter();
      return textConverter.GetText( this );
    }
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <returns></returns>
    public IDocument Clone()
    {
      return CloneImpl();
    }
    /// <summary>
    /// Imports section into document.
    /// </summary>
    /// <param name="section"></param>
    public void ImportSection( ISection section )
    {
      m_stylesHistory = new Hashtable();
      m_bIsImportingSection = true;

      ISection clonedSection = section.Clone( this );
      Sections.Add( clonedSection );

      m_stylesHistory = null;
      m_bIsImportingSection = false;
    }
    #endregion

    #region Class public methods / factory
    /// <summary>
    /// Creates new pargraph instance.
    /// </summary>
    /// <returns></returns>
    public IParagraph CreateParagraph()
    {
      return CreateParagraphImpl();
    }
    /// <summary>
    /// Creates new paragraph item instance.
    /// </summary>
    /// <param name="itemType">Paragraph item type</param>
    /// <returns></returns>
    public IParagraphItem CreateParagraphItem( ParagraphItemType itemType )
    {
      return CreateParagraphItemImpl( itemType );
    }
    /// <summary>
    /// Creates new paragraph item instance.
    /// </summary>
    /// <param name="itemType">Paragraph item type</param>
    /// <returns></returns>
    protected internal virtual IParagraphItem CreateParagraphItem( object itemType )
    {
      return CreateParagraphItemImpl( ( ParagraphItemType )itemType );
    }
    /// <summary>
    /// Creates new shape instance.
    /// </summary>
    /// <param name="shapeType"></param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public Shape CreateShape( ShapeType shapeType, Canvas canvas )
    {
      return CreateShapeImpl( shapeType, canvas );
    }
    #endregion

    #region Class public methods / text find, replace
    /// <summary>
    /// Replaces all entries of given regular expression with replace string.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    public int Replace( Regex pattern, string replace )
    {
      int changesMade = 0;

      foreach( Section section in Sections )
      {
        foreach( Paragraph paragraph in section.Paragraphs )
        {
          changesMade += paragraph.Replace( pattern, replace );
        }
        // Executes for headers/footers
        for( int i = 0; i < 6; i++ )
        {
          IParagraphCollection paragraphs = section.HeadersFooters[ i ].Paragraphs;

          foreach( Paragraph paragraph in paragraphs )
          {
            changesMade += paragraph.Replace( pattern, replace );
          }
        }
      }

      return changesMade;
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
      given = Regex.Escape( given );

      if( wholeWord )
      {
        given = Paragraph.DEF_WHOLE_WORD_BEFORE + given + Paragraph.DEF_WHOLE_WORD_AFTER;
      }

      Regex pattern = new Regex( given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );

      return Replace( pattern, replace );
    }
    /// <summary>
    /// Returns first entry of given regex.
    /// </summary>
    /// <param name="pattern"></param>
    public TextRangesHolder Find( Regex pattern )
    {
      foreach( Section section in Sections )
      {
        foreach( Paragraph paragraph in section.Paragraphs )
        {
          TextRangesHolder rangesHolder = paragraph.Find( pattern );
          if( rangesHolder != null && rangesHolder.Count > 0 )
          {
            return rangesHolder;
          }
        }
        // Executes for headers/footers
        for( int i = 0; i < 6; i++ )
        {
          IParagraphCollection paragraphs = section.HeadersFooters[ i ].Paragraphs;

          foreach( Paragraph paragraph in paragraphs )
          {
            TextRangesHolder rangesHolder = paragraph.Find( pattern );
            if( rangesHolder != null && rangesHolder.Count > 0 )
            {
              return rangesHolder;
            }
          }
        }
      }
      return new TextRangesHolder();
    }
    /// <summary>
    /// Returns first entry of given string, taking into consideration caseSensitive
    /// and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    /// <returns></returns>
    /// <remarks>
    /// Result TextRangesHolder is expected to use as parameter for Replace method.
    /// Each text ranges from TextRangesHolder is only reference to original textrange
    /// in document.
    /// </remarks>
    public TextRangesHolder Find( string given, bool caseSensitive, bool wholeWord )
    {
      given = Regex.Escape( given );

      if( wholeWord )
      {
        given = Paragraph.DEF_WHOLE_WORD_BEFORE + given + Paragraph.DEF_WHOLE_WORD_AFTER;
      }

      Regex pattern = new Regex( given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );

      return Find( pattern );
    }
    /// <summary>
    /// Returns all entries of given regex.
    /// </summary>
    /// <param name="pattern"></param>
    public TextRangesHolder[] FindAll( Regex pattern )
    {
      ArrayList holdersArray = new ArrayList();
      foreach( Section section in Sections )
      {
        foreach( Paragraph paragraph in section.Paragraphs )
        {
          ArrayList rangesHolders = paragraph.FindAll( pattern );
          if( rangesHolders != null && rangesHolders.Count > 0 )
          {
            holdersArray.AddRange( rangesHolders );
          }
        }
        // Executes for headers/footers
        for( int i = 0; i < 6; i++ )
        {
          IParagraphCollection paragraphs = section.HeadersFooters[ i ].Paragraphs as IParagraphCollection;

          foreach( Paragraph paragraph in paragraphs )
          {
            ArrayList rangesHolders = paragraph.FindAll( pattern );
            if( rangesHolders != null && rangesHolders.Count > 0 )
            {
              holdersArray.AddRange( rangesHolders );
            }
          }
        }
      }
      return ( TextRangesHolder[] )holdersArray.ToArray( typeof( TextRangesHolder ) );
    }
    /// <summary>
    /// Returns all entries of given string, taking into consideration caseSensitive
    /// and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    /// <returns></returns>
    public TextRangesHolder[] FindAll( string given, bool caseSensitive, bool wholeWord )
    {
      given = Regex.Escape( given );

      if( wholeWord )
      {
        given = Paragraph.DEF_WHOLE_WORD_BEFORE + given + Paragraph.DEF_WHOLE_WORD_AFTER;
      }

      Regex pattern = new Regex( given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );

      return FindAll( pattern );
    }
    /// <summary>
    /// Replaces all entries of given regular expression with TextRangesHolder.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="textRangesHolder"></param>
    public void Replace( Regex pattern, TextRangesHolder textRangesHolder )
    {
      foreach( Section section in Sections )
      {
        foreach( Paragraph paragraph in section.Paragraphs )
        {
          paragraph.Replace( pattern, textRangesHolder );
        }
        // Executes for headers/footers
        for( int i = 0; i < 6; i++ )
        {
          IParagraphCollection paragraphs = section.HeadersFooters[ i ].Paragraphs ;

          foreach( Paragraph paragraph in paragraphs )
          {
            paragraph.Replace( pattern, textRangesHolder );
          }
        }
      }
    }
    /// <summary>
    /// Replaces all entries of given string with TextRangesHolder, taking into
    /// consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="textRangesHolder"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    public void Replace( string given, TextRangesHolder textRangesHolder, bool caseSensitive, bool wholeWord )
    {
      given = Regex.Escape( given );

      if( wholeWord )
      {
        given = Paragraph.DEF_WHOLE_WORD_BEFORE + given + Paragraph.DEF_WHOLE_WORD_AFTER;
      }

      Regex pattern = new Regex( given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );

      Replace( pattern, textRangesHolder );
    }
    #endregion

    #region Class factory methods
    /// <summary>
    /// Implementation of paragraph creation.
    /// </summary>
    /// <returns></returns>
    protected virtual IParagraph CreateParagraphImpl()
    {
      return new Paragraph( this );
    }
    /// <summary>
    /// Implementation of section creating
    /// </summary>
    /// <returns></returns>
    protected internal virtual ISection CreateSectionImpl()
    {
      return new Section( this );
    }
    /// <summary>
    /// Implementation of style creation.
    /// </summary>
    /// <returns></returns>
    internal protected virtual Column CreateColumnImpl()
    {
      return new Column( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual TableRow CreateTableRowImpl( Table table )
    {
      return new TableRow( table );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual TableCell CreateTableCellImpl( TableRow row )
    {
      return new TableCell( row );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual TableColumn CreateTableColumnImpl( ITable table )
    {
      return new TableColumn( table );
    }
    /// <summary>
    /// Implementation of character foprmat creating
    /// </summary>
    /// <returns></returns>
    protected internal virtual TextBody CreateTextBodyImpl()
    {
      return new TextBody( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected internal virtual HeadersFooters CreateHeadersFootersImpl()
    {
      return new HeadersFooters( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected internal virtual PageSetup CreatePageSetupImpl()
    {
      return new PageSetup( this );
    }
    #endregion

    #region Class factory methods / collections
    /// <summary>
    /// Implementation of section collection creating
    /// </summary>
    /// <returns></returns>
    internal protected virtual ColumnCollection CreateColumnCollectionImpl( ISection owner )
    {
      return new ColumnCollection( owner );
    }
    /// <summary>
    /// Implementation of section collection creating
    /// </summary>
    /// <returns></returns>
    protected virtual ISectionCollection CreateSectionCollectionImpl()
    {
      return new SectionCollection( this );
    }
    /// <summary>
    /// Implementation of paragraph collection creating
    /// </summary>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual IParagraphCollection CreateParagraphCollectionImpl()
    {
      return new ParagraphCollection( this );
    }
    /// <summary>
    /// Implementation of paragraph collection creating
    /// </summary>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual IParagraphCollection CreateParagraphCollectionImpl( IEntityBase owner )
    {
      return new ParagraphCollection( owner );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual RowCollection CreateTableRowCollectionImpl( Table table )
    {
      return new RowCollection( table );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual CellCollection CreateTableCellCollectionImpl( TableRow row )
    {
      return new CellCollection( row );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal virtual TableColumnCollection CreateTableColumnCollectionImpl( Table table )
    {
      return new TableColumnCollection( table );
    }
    /// <summary>
    /// Creates the tab collection impl.
    /// </summary>
    /// <param name="paragraph">The paragraph.</param>
    /// <returns></returns>
    protected internal virtual TabCollection CreateTabCollectionImpl( ParagraphFormat paragraph )
    {
      return new TabCollection( Document );
    }
    #endregion

    #region Class factory methods / styles
    /// <summary>
    /// Implementation of style collection creating
    /// </summary>
    /// <returns></returns>
    protected virtual IStyleCollection CreateStyleCollectionImpl()
    {
      return new StyleCollection( this );
    }
    /// <summary>
    /// Implementation of style creating
    /// </summary>
    /// <returns></returns>
    protected internal virtual IStyle CreateStyleImpl( StyleType styleType )
    {
      IStyle style = null;

      switch( styleType )
      {
        case DLS.StyleType.ParagraphStyle:
          style = new ParagraphStyle( this );
          break;
        case DLS.StyleType.ImageStyle:
          style = new ImageStyle( this );
          break;
        case DLS.StyleType.ShapeStyle:
          style = new ShapeStyle( this );
          break;
      }

      return style;
    }
    /// <summary>
    /// Implementation of character foprmat creating
    /// </summary>
    /// <returns>Character format object.</returns>
    protected internal virtual CharacterFormat CreateCharacterFormatImpl()
    {
      return new CharacterFormat( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected internal virtual ListStyle CreateListStyleImpl()
    {
      return new ListStyle( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected internal virtual ListLevel CreateListLevelImpl( ListStyle style )
    {
      return new ListLevel( style );
    }
    /// <summary>
    /// Implementation of paragraph foprmat creating
    /// </summary>
    /// <returns>Paragraph format object.</returns>
    protected internal virtual ParagraphFormat CreateParagraphFormatImpl()
    {
      return new ParagraphFormat( this );
    }
    /// <summary>
    /// Implementation of table format creating
    /// </summary>
    /// <returns>Table format instance</returns>
    protected internal virtual TableFormat CreateTableFormatImpl()
    {
      return new TableFormat();
    }
    /// <summary>
    /// Implementation of cell format creating
    /// </summary>
    /// <returns></returns>
    protected internal virtual CellFormat CreateCellFormatImpl()
    {
      return new CellFormat();
    }
    /// <summary>
    /// Implementation of shape format creating
    /// </summary>
    /// <returns>Shape format object.</returns>
    protected internal virtual ShapeFormat CreateShapeFormatImpl()
    {
      return new ShapeFormat();
    }
    /// <summary>
    /// Implementation of textbox format creating
    /// </summary>
    /// <returns>textbox format object.</returns>
    protected internal virtual TextBoxFormat CreateTextboxFormatImpl()
    {
      return new TextBoxFormat();
    }
    /// <summary>
    /// Text
    /// </summary>
    /// <returns></returns>
    protected virtual TextBoxCollection CreateTextBoxCollectionImpl()
    {
      return new TextBoxCollection( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    protected internal virtual ListFormat CreateListFormatImpl( IParagraph owner )
    {
      return new ListFormat( owner );
    }   
    #endregion

    #region Class factory methods / paragraph items
    /// <summary>
    /// Implementation of paragraph item creating
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    protected internal virtual IParagraphItem CreateParagraphItemImpl( ParagraphItemType itemType )
    {
      switch( itemType )
      {
        case ParagraphItemType.TextRange:
          return new TextRange( this );
        case ParagraphItemType.Picture:
          return new Picture( this );
        case ParagraphItemType.Canvas:
          return new Canvas( this );
        case ParagraphItemType.Table:
          return new Table( this );
        case ParagraphItemType.BookmarkStart:
          return new BookmarkStart( this );
        case ParagraphItemType.BookmarkEnd:
          return new BookmarkEnd( this );
        case ParagraphItemType.Field:
          return new Field( this );
        case ParagraphItemType.TextBox:
          return new TextBox( this );
        default:
          throw new ArgumentException( "Ivalid type of paragraph item" );
      }
    }
    /// <summary>
    /// Creates the paragraph item collection.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <returns></returns>
    protected internal virtual IParagraphItemCollection CreateParagraphItemCollectionImpl( IParagraph owner )
    {
      return new ParagraphItemCollection( owner );
    }
    /// <summary>
    /// Implementation of shape creating
    /// </summary>
    /// <param name="shapeType">Shape type</param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    protected virtual Shape CreateShapeImpl( ShapeType shapeType, Canvas canvas )
    {
      switch( shapeType )
      {
        case ShapeType.Text:
          return new TextShape( canvas );
        case ShapeType.Rectangle:
          return new RectangleShape( canvas );
        case ShapeType.Image:
          return new ImageShape( canvas );
        case ShapeType.Arc:
          return new ArcShape( canvas );
        case ShapeType.Bezier:
          return new BezierShape( canvas );
        case ShapeType.Ellipse:
          return new EllipseShape( canvas );
        case ShapeType.Line:
          return new LineShape( canvas );
        case ShapeType.Path:
          return new PathShape( canvas );
        case ShapeType.Pie:
          return new PieShape( canvas );
        case ShapeType.Polygon:
          return new PolygonShape( canvas );
        default:
          throw new ArgumentException( "Invalid shape type" );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    public void ImportContent( IDocument doc )
    {
      // Clones sections items
      foreach( ISection section in doc.Sections )
      {
        Sections.Add( section.Clone( this ) );
      }

      // Clones styles items
      foreach( IStyle style in doc.Styles )
      {
        IStyle foundStyle = Styles.FindByName( style.Name );
        if( foundStyle == null )
        {
          Styles.Add( style.Clone( this ) );
        }
        else if( foundStyle != style )
        {
          IStyle newStyle = style.Clone( this );
          newStyle.Name += Guid.NewGuid().ToString();
          Styles.Add( newStyle );
        }
      }

      //Clone list styles
      foreach( ListStyle listStyle in doc.ListStyles )
      {
        if( ListStyles.FindByName( listStyle.Name ) == null )
        {
          ListStyles.Add( listStyle.Clone( this ));
        }
      }
    }
    /// <summary>
    /// Create default liststyles and add them to ListStyleCollection 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal void EnsureListStyles()
    {
      if( ListStyles.Count == 0 )
      {
        //Create Numbered Style
        ListStyle numStyle = new ListStyle( this, ListType.Numbered );
        numStyle.Name = "Numbered";
        numStyle.ListType = ListType.Numbered;
        ListStyles.Add( numStyle );

        //Create Bulleted Style
        ListStyle bulletStyle = new ListStyle( this, ListType.Bulleted );
        bulletStyle.Name = "Bulleted";
        bulletStyle.ListType = ListType.Bulleted;
        ListStyles.Add( bulletStyle );
      }
//      if( ListStyles.FindByName( DEF_BULLETS_STYLE ) == null )
//      {
//        ListStyle bullListStyle = new ListStyle( this );
//        bullListStyle.Name = DEF_BULLETS_STYLE;
//        bullListStyle.ListType = ListType.Bulleted;
//        ListStyles.Add( bullListStyle );
//      }
//
//      if( ListStyles.FindByName( DEF_NUMBERING_STYLE ) == null )
//      {
//        ListStyle numListStyle = new ListStyle( this );
//        numListStyle.Name = DEF_NUMBERING_STYLE;
//        numListStyle.ListType = ListType.Numbered;
//        ListStyles.Add( numListStyle );
//      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected internal void EnsureParagraphStyle( IParagraph paragraph )
    {
      if( paragraph.StyleName == null )
      {
        if( Styles.FindByName( DEF_NORMAL_STYLE ) == null )
        {
          AddStyle( DLS.StyleType.ParagraphStyle, DEF_NORMAL_STYLE );
        }

        paragraph.ApplyStyle( DEF_NORMAL_STYLE );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected virtual IDocument CloneImpl()
    {
      return new Document( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    private void WriteXml( XmlWriter writer )
    {
      XDLSWriter xdlsWriter = new XDLSWriter( writer );
      xdlsWriter.Serialize( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [Syncfusion.Documentation.DocumentationExclude()]
    private void ReadXml( XmlReader reader )
    {
      XDLSReader dlsXmlReader = new XDLSReader( reader );
      dlsXmlReader.Deserialize( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected internal virtual Image GetBackGndImage()
    {
      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual void OnSetBackgroundImage()
    {}
    #endregion

    #region IXmlSerializable implement
    /// <summary>
    /// Gets xml schema
    /// </summary>
    /// <returns></returns>
    XmlSchema IXmlSerializable.GetSchema()
    {
      return GetSchema();
    }
    /// <summary>
    /// Reads document using XmlReader
    /// </summary>
    /// <param name="reader"></param>
    void IXmlSerializable.ReadXml( XmlReader reader )
    {
      ReadXml( reader );
    }
    /// <summary>
    /// Writes document using XmlWriter
    /// </summary>
    /// <param name="writer"></param>
    void IXmlSerializable.WriteXml( XmlWriter writer )
    {
      WriteXml( writer );
    }
    #endregion

    #region XDLSSerializableBase overrides
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual XmlSchema GetSchema()
    {
      return XsdGenerator.GetDLSLocalSchema();
    }
    /// <summary>
    /// Adds serializable elements to XDLS holder.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void InitXDLSHolder()
    {
      
      XDLSHolder.AddElement( XDLSConstants.StylesTag, Styles );
      XDLSHolder.AddElement( XDLSConstants.ListStylesTag, ListStyles ); 
      XDLSHolder.AddElement( XDLSConstants.SectionsTag, Sections );
    }
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="writer"></param>
//    [Syncfusion.Documentation.DocumentationExclude()]
//    protected override void WriteXmlContent( IXDLSContentWriter writer )
//    {
//      base.WriteXmlContent( writer );
//      writer.WriteImage( m_backgrndImage );
//    }
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="reader"></param>
//    [Syncfusion.Documentation.DocumentationExclude()]
//    protected override bool ReadXmlContent( IXDLSContentReader reader )
//    {
//      if( reader.TagName == XDLSConstants.ImageTag )
//      {
//        m_backgrndImage = reader.ReadImage();
//      }
//      return base.ReadXmlContent( reader );
//    }
    #endregion

    #region WidgetContainer overrides
    /// <summary>
    /// Creates and initializes layouting data
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Vertical );
    }
    /// <summary>
    /// Gets subwidgets
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    protected override ICollectionBase WidgetCollection
    {
      get
      {
        return Sections;
      }
    }
    #endregion
  }
}
