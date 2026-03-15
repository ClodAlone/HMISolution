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
using System.Text.RegularExpressions;

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a Text Body.
  /// </summary>
  public class TextBody
    : WidgetContainer,
      ITextBody,
      IWidgetContainer
  {
    #region Class members
    /// <summary>
    /// The section paragraphs
    /// </summary>
    protected IParagraphCollection m_paragraphs;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets inner paragraphs
    /// </summary>
    public IParagraphCollection Paragraphs
    {
      get
      {
        return m_paragraphs;
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="doc"></param>
    public TextBody( IDocument doc )
      : base( doc )
    {
      m_paragraphs = DocumentEx.CreateParagraphCollectionImpl();
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds paragraph at end of section.
    /// </summary>
    /// <returns></returns>
    public IParagraph AddParagraph()
    {
      int i = m_paragraphs.Add( Document.CreateParagraph() );
      return m_paragraphs[ i ];
    }
    /// <summary>
    /// Inserts html at end of text body.
    /// </summary>
    public void InsertHTML( string html )
    {
      InsertHTML( html, Paragraphs.Count );
    }
    /// <summary>
    /// Inserts html begins from paragraph spesified by paragraphIndex
    /// </summary>
    public void InsertHTML( string html, int paragraphIndex )
    {
      Paragraphs.Insert( paragraphIndex, Document.CreateParagraph() );
      InsertHTML(html, paragraphIndex, 0);
    }
    /// <summary>
    /// Inserts html begins from paragraph spesified by paragraphIndex, 
    /// and after paragraph item by paragraphItemIndex
    /// </summary>
    public void InsertHTML( string html, int paragraphIndex, int paragraphItemIndex )
    {
      IHtmlConverter htmlConverter = HtmlConverterFactory.GetInstance();
      
      htmlConverter.AppendToTextBody( this, html, paragraphIndex, paragraphItemIndex);
    }
    #endregion
    
    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.ParagraphsTag, Paragraphs );
      XDLSHolder.SkipID = true;
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
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override ICollectionBase WidgetCollection
    {
      get
      {
        return Paragraphs;
      }
    }
    #endregion
  }
}
