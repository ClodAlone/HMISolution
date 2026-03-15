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

using System;

using Syncfusion.DLS.XML;

namespace Syncfusion.DLS 
{
	/// <summary>
	/// Represents a hyperlink in a document.
	/// </summary>
  public class Hyperlink : XDLSSerializableBase
	{
    #region Class members
    /// <summary>
    /// File path.
    /// </summary>
    private string m_filePath;
    /// <summary>
    /// Url link.
    /// </summary>
    private string m_uriPath;
    /// <summary>
    /// Bookmark.
    /// </summary>
    private Bookmark m_bookmark;
    /// <summary>
    /// Object that indicates the link type.
    /// </summary>
    private HyperlinkType m_hyperlinkType;
    /// <summary>
    /// Bookmark's name.
    /// </summary>
    private string m_tempBookmarkName;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets file path.
    /// </summary>
    public string FilePath
    {
      get
      {
        return m_filePath;
      }
      set
      {
        if( m_filePath != value )
        {
          m_filePath = value;
          m_hyperlinkType = HyperlinkType.FileLink;
        }
      }
    }
    /// <summary>
    /// Gets / sets url link. 
    /// </summary>
    public string Uri
    {
      get
      {
        return m_uriPath;
      }
      set
      {
        if( m_uriPath != value )
        {
          m_uriPath = value;
          m_hyperlinkType = HyperlinkType.UriLink;
        }
      }
    }
    /// <summary>
    /// Gets / sets bookmark.
    /// </summary>
    public Bookmark Bookmark
    {
      get
      {
        return m_bookmark;
      }
      set
      {
        if( m_bookmark != value )
        {
          m_bookmark = value;
          m_hyperlinkType = HyperlinkType.Bookmark;
        }
      }
    }
    /// <summary>
    /// Gets / sets a HyperlinkType object that indicates the link type. 
    /// </summary>
    public HyperlinkType HyperlinkType
    {
      get
      {
        return m_hyperlinkType;
      }
      set
      {
        if( m_hyperlinkType != value )
        {
          m_hyperlinkType = value;
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="doc"></param>
    internal Hyperlink( IDocument doc )
      : base( doc )
    {
    }
    #endregion

    #region IXDLSSerializable implement
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddRefElement( XDLSConstants.Bookmark, Bookmark );
    }
    /// <summary>
    /// Restores object references after deserialization.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void RestoreReference( string name, int index )
    {
      //base.RestoreReference (name, index);
      if( name == XDLSConstants.Bookmark && m_tempBookmarkName != null && m_tempBookmarkName.Length != 0 )
      {
        Bookmark = Document.Bookmarks[ m_tempBookmarkName ];
      }
    }
    /// <summary>
    /// Overloaded. Write attributes to xml.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      if( m_filePath != "" && m_filePath != null )
      {
        writer.WriteValue( XDLSConstants.FilePath, m_filePath );
      }
      if( m_uriPath != "" && m_uriPath != null )
      {
        writer.WriteValue( XDLSConstants.Uri, m_uriPath );
      }
      if( m_bookmark != null )
      {
        writer.WriteValue( XDLSConstants.BookMarkNameAttr, m_bookmark.Name );
      }
      if( m_hyperlinkType != HyperlinkType.None )
      {
        writer.WriteValue( XDLSConstants.HyperlinkType, m_hyperlinkType );
      }
    }
    /// <summary>
    /// Overloaded. Reads XML attributes.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader.HasAttribute( XDLSConstants.FilePath ) )
      {
        m_filePath = reader.ReadString( XDLSConstants.FilePath );
      }
      if( reader.HasAttribute( XDLSConstants.Uri ) )
      {
        m_uriPath  = reader.ReadString( XDLSConstants.Uri );
      }
      if( reader.HasAttribute( XDLSConstants.BookMarkNameAttr ) )
      {
        m_tempBookmarkName = reader.ReadString( XDLSConstants.BookMarkNameAttr );
      }
      if( reader.HasAttribute( XDLSConstants.HyperlinkType ) )
      {
        m_hyperlinkType = ( HyperlinkType )reader.ReadEnum
          ( XDLSConstants.HyperlinkType, typeof( HyperlinkType ) );
      }
    }
    #endregion
	}
}
