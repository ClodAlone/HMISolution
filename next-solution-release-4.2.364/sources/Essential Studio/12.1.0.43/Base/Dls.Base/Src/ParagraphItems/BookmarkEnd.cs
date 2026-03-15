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

using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents the end position of the bookmark in the document.
  /// </summary>
  public class BookmarkEnd 
    : ParagraphItem,
    ILeafWidget
  {
    #region Class members
    private string m_strName = "";
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets/sets bookmark name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    public BookmarkEnd( IDocument doc )
      : base( doc )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="bookmark"></param>
    /// <param name="paragraph"></param>    
    protected BookmarkEnd( BookmarkEnd bookmark, IParagraph paragraph ) 
      : base( bookmark.Document )
    {
      Name = bookmark.Name;
      SetOwnerParagraph( paragraph, bookmark.StartIndex );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Horizontal );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      Bookmark bookmark = Document.Bookmarks.FindByName( Name );

      BookmarkEnd end = new BookmarkEnd( this, paragraph );
      bookmark.m_bkmkEnd = end;
      
      return end;
    }
    #endregion
    
    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ParagraphItemType.BookmarkEnd );
      
      writer.WriteValue( XDLSConstants.BookMarkNameAttr, Name );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      
      Name = reader.ReadString( XDLSConstants.BookMarkNameAttr );
      // End bookmark in the collection
      Document.Bookmarks.EndBookmark( this, Name );
    }
    #endregion

    #region IWidget/ILeafWidget implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    SizeF ILeafWidget.Measure( CustomGraphics cg )
    {
      return new SizeF( 0, 0 );
    }
    #endregion
  }

}
