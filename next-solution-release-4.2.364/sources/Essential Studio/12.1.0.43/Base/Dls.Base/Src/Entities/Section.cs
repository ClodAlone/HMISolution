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
using System.Diagnostics;
using System;
using System.Drawing;
using System.Collections;

using Syncfusion.DLS.Collections;
using Syncfusion.Layouting;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a document section
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class Section
    : TextBody,
      ISection,
      IWidgetContainer
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const float DEF_DISTANCE_BETWEEN_COLUMNS = 36;
    #endregion

    #region Class members
    /// <summary>
    /// The section page Setup.
    /// </summary>
    protected PageSetup m_pageSetup;
    /// <summary>
    /// The columns collection.
    /// </summary>
    private ColumnCollection m_columns;
    /// <summary>
    /// The headers / footers, related with current section.
    /// </summary>
    private HeadersFooters m_headersFooters;
    /// <summary>
    /// The break code.
    /// </summary>
    private SectionBreakCode m_breakCode = SectionBreakCode.NewPage;
    /// <summary>
    /// Collection that holds names of old styles and their new names.
    /// </summary>
    private Hashtable m_oldParaStylesHolder = new Hashtable();
    #endregion

    #region Class properties
    /// <summary>
    /// Gets headers/footers of current section.
    /// </summary>
    public HeadersFooters HeadersFooters
    {
      get
      {
        return m_headersFooters;
      }
    }
    /// <summary>
    /// Gets page Setup of current section.
    /// </summary>
    public PageSetup PageSetup
    {
      get
      {
        return m_pageSetup;
      }
    }
    /// <summary>
    /// Get collection of columns which logically divide page on many.
    /// printing/publishing areas
    /// </summary>
    public ColumnCollection Columns
    {
      get
      {
        return m_columns;
      }
    }
    /// <summary>
    /// Gets / sets break code.
    /// </summary>
    public SectionBreakCode BreakCode
    {
      get
      {
        return m_breakCode;
      }
      set
      {
        m_breakCode = value;
      }
    }
    /// <summary>
    /// Gets the collection that holds names of old styles and their new names.
    /// </summary>
    internal Hashtable OldParaStylesHolder
    {
      get
      {
        return m_oldParaStylesHolder;
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="doc"></param>
    public Section( IDocument doc )
      : base( doc )
    {
      m_columns = DocumentEx.CreateColumnCollectionImpl( this );
      m_pageSetup = DocumentEx.CreatePageSetupImpl();
      m_paragraphs = DocumentEx.CreateParagraphCollectionImpl( this );
      m_headersFooters = DocumentEx.CreateHeadersFootersImpl();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="section"></param>
    /// <param name="doc"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal Section( ISection section, IDocument doc ) 
     : this( doc )
    {
      ( doc as Document ).CurClonedSection = section as Section;
      m_pageSetup = section.PageSetup.Clone( doc );
 
      foreach( Column column in section.Columns )
      {
        m_columns.Add( column.Clone() );
      }
      
      foreach( Paragraph paragraph in section.Paragraphs )
      {
        m_paragraphs.Add( paragraph.Clone( doc ) );
      }
      
      // Executes for headers/footers
      for( int i = 0; i < 6; i++ )
      {
        IParagraphCollection paragraphs = section.HeadersFooters[ i ].Paragraphs;
          
        foreach( Paragraph paragraph in paragraphs )
        {
          this.HeadersFooters[ i ].Paragraphs.Add( paragraph.Clone( doc ) );
        }
      }
      
      BreakCode = section.BreakCode;
      ( doc as Document ).CurClonedSection.OldParaStylesHolder.Clear();
      ( doc as Document ).CurClonedSection = null;

    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds new column to the section.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="spacing"></param>
    /// <returns></returns>
    public Column AddColumn( float width, float spacing )
    {
      Column column = DocumentEx.CreateColumnImpl();
      column.Width = width;
      column.Space = spacing;
      Columns.Add( column );
      
      return column;
    }
    /// <summary>
    /// Clones section and sets new owner document.
    /// </summary>
    /// <param name="document">New owner document</param>
    /// <returns></returns>
    public ISection Clone( IDocument document )
    {
      return CloneImpl( document ); 
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected virtual ISection CloneImpl( IDocument document )
    {
      return new Section( this, document );
    }
    /// <summary>
    /// Makes all columns in current section to be of equal width.
    /// </summary>
    public void MakeColumnsEqual()
    {
      if( Columns.Count > 0 )
      {
        float usingWidth = PageSetup.PageSize.Width
          - ( ( PageSetup.Margins.Left != -0.05f ) ? PageSetup.Margins.Left : 0f )
          - ( ( PageSetup.Margins.Right != -0.05f ) ? PageSetup.Margins.Right : 0f );

        float colWidth = ( usingWidth - ( Columns.Count - 1 ) * DEF_DISTANCE_BETWEEN_COLUMNS ) / Columns.Count;
        
        foreach(Column column in Columns )
        {
          column.Width = colWidth;
          column.Space = DEF_DISTANCE_BETWEEN_COLUMNS;
        }
      }
    }
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.PageSetupTag, PageSetup );
      XDLSHolder.AddElement( XDLSConstants.ColumnsTag, Columns );
      XDLSHolder.AddElement( XDLSConstants.ParagraphsTag, Paragraphs );
      XDLSHolder.AddElement( XDLSConstants.HeadersFootersTag, HeadersFooters );
      XDLSHolder.SkipID = true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.SectionBreakCodeAttr, BreakCode );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );

      if( reader.HasAttribute( XDLSConstants.SectionBreakCodeAttr ) )
      {
        BreakCode = ( SectionBreakCode )reader.ReadEnum( XDLSConstants.SectionBreakCodeAttr, typeof( SectionBreakCode ) );
      }

    }
    #endregion

    #region WidgetContainer overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Vertical );
    }
    #endregion
  }
}
