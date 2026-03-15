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
using System.IO;
using System.Text;
using System.Collections;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Schema;

using System.ComponentModel;
using System.ComponentModel.Design;
//using System.Windows.Forms;

using System.Resources;
using System.Diagnostics;
using System.Reflection;

//using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
using Syncfusion.HTMLUI.Base.Parser.HTML;
#endregion

namespace Syncfusion.HTMLUI.Base
{
  /// <summary>
  /// This class provides HTML Parser functionality. It makes the HTML
  /// well formatted and safe for use as XML document.
  /// </summary>
  public class HTMLUIParser
  {
    #region Class constants
    /// <summary>
    /// Default size of the stream internal buffer.
    /// </summary>
    private const int DEF_BUFFER_SIZE = 8192;
    #endregion

    #region Class members
    /// <summary>
    /// Converter of HTML document to xHTML document.
    /// </summary>
    private HTMLToXML m_convertor;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns the encoding used by this parser.
    /// </summary>
    public Encoding Encoding
    {
      get
      {
        Encoding result = Encoding.Default;

        if( m_convertor != null )
        {
          result = m_convertor.Encoding;
        }

        return result;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public HTMLUIParser()
    {
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Overloaded. Converts HTML data in the stream to xHTML format.
    /// </summary>
    /// <param name="data">Stream containing HTML data.</param>
    /// <returns>Html in xHTML format.</returns>
    public virtual XmlDocument Parse( Stream data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      using( TokenStream ts = new TokenStream( data, false ) )
      {
        return Parse( ts );
      }
    }
    /// <summary>
    /// Main method. Parses, checks and converts HTML document.
    /// </summary>
    /// <param name="tokenStream">Object containing data.</param>
    /// <returns>HTML data in xHTML format.</returns>
    public virtual XmlDocument Parse( TokenStream tokenStream )
    {
      if( tokenStream == null )
        throw new ArgumentNullException( "tokenStream" );

      XmlDocument document = new XmlDocument();
      document.PreserveWhitespace = true;

#if PERFORMANCE
      DateTime now = DateTime.Now;
#endif
      
      Stream streamIn = ConvertToXHTML( tokenStream );

#if PERFORMANCE
      Debug.Indent();
      Debug.WriteLine( DateTime.Now.Subtract( now ), "ConvertToXHTML Takes" );
      Debug.Unindent();
      now = DateTime.Now;
#endif
      LoadDocument( document, streamIn );

#if PERFORMANCE
      Debug.Indent();
      Debug.WriteLine( DateTime.Now.Subtract( now ), "XMLDocument.Load Takes" );
      Debug.Unindent();
      now = DateTime.Now;
#endif

      CheckCorrectDocument( document );

#if PERFORMANCE
      Debug.Indent();
      Debug.WriteLine( DateTime.Now.Subtract( now ), "CheckCorrectDocument Takes" );
      Debug.Unindent();
      now = DateTime.Now;
#endif

      return document;
    }
    /// <summary>
    /// Builds HTML object tree from the string text.
    /// </summary>
    /// <returns>Root element.</returns>
    public virtual XmlElement ParseString( string html )
    {
      using( TokenStream ts = TokenStream.FromString( html ) )
      {
        XmlDocument doc = ConvertToXML( ts );

        return doc.DocumentElement;
      }
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// Overloaded. Converts HTML to XHTML.
    /// </summary>
    /// <returns>String of the parsed document.</returns>
    protected virtual XmlDocument ConvertToXML( string fileName )
    {
      XmlDocument document = null;

      using( TokenStream file = new TokenStream( fileName ) )
      {
        document = ConvertToXML( file );
        file.Close();
      }

      return document;
    }
    /// <summary>
    /// Converts HTML document to XHTML document and loads the result into XML document.
    /// </summary>
    /// <param name="stream">Source of data.</param>
    protected virtual XmlDocument ConvertToXML( TokenStream stream )
    {
      XmlDocument document = new XmlDocument();
      document.Load( ConvertToXHTML( stream ) );
      return document;
    }
    /// <summary>
    /// Converts HTML to XHTML.
    /// </summary>
    /// <returns>String of the parsed document.</returns>
    protected virtual Stream ConvertToXHTML( TokenStream stream )
    {
      MemoryStream ms = new MemoryStream( DEF_BUFFER_SIZE );
      XmlTextWriter output = new XmlTextWriter( ms, Encoding.Unicode );

      // NOTE: Here we put tag HTML as first, because for correct parsing of
      // xhtml document as XML in document must be only one root tag...
      output.WriteStartElement( "html" );
      m_convertor = new HTMLToXML( stream, output );
      m_convertor.Convert();
      output.WriteEndElement();
      output.Flush();
      ms.Position = 0;

#if DEBUG
      StringBuilder build = new StringBuilder( 8192 );
      build.Append( Encoding.Unicode.GetChars( ms.ToArray() ) );
      Debug.WriteLine( build.ToString(), "HTML" );
#endif

      return ms;
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Creates a tag.
    /// </summary>
    /// <param name="document">Storage of the data.</param>
    /// <param name="parentName">Name of the parent tag.</param>
    /// <param name="elementName">Name of the element.</param>
    private void CreateTag( XmlDocument document, string parentName, string elementName )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      if( parentName == null )
        throw new ArgumentNullException( "parentName" );

      if( parentName.Length == 0 )
        throw new ArgumentException( "parentName - string can not be empty" );

      if( elementName == null )
        throw new ArgumentNullException( "elementName" );

      if( elementName.Length == 0 )
        throw new ArgumentException( "elementName - string can not be empty" );

      XmlNode parentNode = document.SelectSingleNode( "/" + parentName );
      XmlElement element = document.CreateElement( elementName );
      parentNode.AppendChild( element );
    }

    /// <summary>
    /// Detaches a tag from the document.
    /// </summary>
    /// <param name="document">Storage of the data.</param>
    /// <param name="xPath">XPath pattern for detaching elements.</param>
    /// <returns>Node after detaching child nodes.</returns>
    private XmlNode DetachTag( XmlDocument document, string xPath )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      if( xPath == null )
        throw new ArgumentNullException( "xPath" );

      if( xPath.Length == 0 )
        throw new ArgumentException( "xPath - string can not be empty" );

      XmlNode output = document.SelectSingleNode( xPath );
      return output.ParentNode.RemoveChild( output );
    }
    /// <summary>
    /// Attaches node to the document.
    /// </summary>
    /// <param name="document">Storage of data.</param>
    /// <param name="parentName">Name of the parent element.</param>
    /// <param name="node">Name of the element.</param>
    private void AttachNodeToDocument( XmlDocument document, string parentName, XmlNode node )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      if( parentName == null )
        throw new ArgumentNullException( "parentName" );

      if( parentName.Length == 0 )
        throw new ArgumentException( "parentName - string can not be empty" );

      if( node == null )
        throw new ArgumentNullException( "node" );

      XmlNode parentNode = document.SelectSingleNode( "/" + parentName );
      parentNode.AppendChild( node );
    }

    /// <summary>
    /// Moves nodes from the list to the defined node.
    /// </summary>
    /// <param name="node">Owner of the nodes in the list.</param>
    /// <param name="list">Nodes which must be replaced.</param>
    private void MoveTagsToNewParent( XmlNode node, XmlNodeList list )
    {
      if( node == null )
        throw new ArgumentNullException( "node" );

      if( list == null )
        throw new ArgumentNullException( "list" );

      string nodeValue = string.Empty;
      XmlNode childNode = null;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        childNode = list[ i ];

        if( childNode is XmlText )
        {
          nodeValue = childNode.Value.Trim();
          if( nodeValue == null || nodeValue.Length == 0 ) continue;

          if( nodeValue.Length != childNode.Value.Length ) childNode.Value = nodeValue;
          node.AppendChild( childNode );
        }
        else if( childNode is XmlElement )
        {
          node.AppendChild( childNode );
        }
      }
    }
    /// <summary>
    /// Parses document and converts it to a valid HTML document.
    /// </summary>
    /// <param name="document">XML document for checking.</param>
    /// <returns>Checked document.</returns>
    private void CheckCorrectDocument( XmlDocument document )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      CheckCorrectDocumentHTMLTag( document );

      // Starting find missed nodes.
      XmlDocument configDoc = HTMLConfig.XMLConfig1;

      HTMLElement elmBody = ( HTMLElement )HTMLConfig.MainConfig.Elements[ "html:body" ];
      HTMLElement elmHead = ( HTMLElement )HTMLConfig.MainConfig.Elements[ "html:head" ];

      string xPath = "/html/head";
      string xPath2 = "/html/body";
      XmlNode nodeHead, nodeBody;

      nodeHead = document.SelectSingleNode( xPath );
      if( nodeHead == null )
      {
        CreateTag( document, "html", "head" ); // create tag in document
      }
      else
      {
        RemoveBadTags( nodeHead, xPath + "/node()", elmHead );
      }

      nodeHead = DetachTag( document, xPath );
      if( elmHead.IsChildrenUsed )
      {
        foreach( string key  in elmHead.Children.Keys )
        {
          HTMLElement elm = ( HTMLElement )elmHead.Children[ key ];

          if( elm.Expression.Length == 0 )
          {
            xPath = "//*[name()='" + elm.SubName + "']";
          }
          else
          {
            xPath = elm.Expression;
          }

          XmlNodeList list = document.SelectNodes( xPath );
          MoveTagsToNewParent( nodeHead, list );
        }
      }

      if( document.SelectSingleNode( xPath2 ) == null )
      {
        CreateTag( document, "html", "body" );
        nodeBody = DetachTag( document, xPath2 );
        MoveTagsToNewParent( nodeBody, document.SelectNodes( "/html/node()" ) );
      }
      else
      {
        nodeBody = DetachTag( document, xPath2 );
      }

        // The issue SD2883 and incident #61882
      while (nodeBody.InnerXml.Contains("<html") && nodeBody.InnerXml.Contains("<body"))
      {
          XmlNamespaceManager namespacemang = new XmlNamespaceManager(document.NameTable);
          namespacemang.AddNamespace("xmls", nodeBody.ChildNodes[0].NamespaceURI);
          nodeBody = nodeBody.SelectSingleNode("/xmls:html/xmls:body", namespacemang);
      }
      AttachNodeToDocument( document, "html", nodeHead );
      AttachNodeToDocument( document, "html", nodeBody );
    }
    /// <summary>
    /// Removes bad tags from the document.
    /// </summary>
    /// <param name="node">Current node for removing tags.</param>
    /// <param name="xPath">xPath pattern.</param>
    /// <param name="element">Element containing tags.</param>
    private void RemoveBadTags( XmlNode node, string xPath, HTMLElement element )
    {
      XmlNodeList nodeList = node.SelectNodes( xPath );
      XmlNode childNode = null;

      for( int i = 0, len = nodeList.Count; i < len; i++ )
      {
        childNode = nodeList[ i ];
        if( childNode is XmlText )
        {
          node.RemoveChild( childNode );
          continue;
        }
        if( !element.Children.Contains( "html:" + childNode.Name ) )
        {
          node.RemoveChild( childNode );
        }
      }
    }
    /// <summary>
    /// Checks if document has two root elements HTML, if yes - repairs that bug.
    /// </summary>
    /// <param name="document">XML document object for checking.</param>
    /// <returns>Checked object.</returns>
    private void CheckCorrectDocumentHTMLTag( XmlDocument document )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      string xPath = "/html/html";
      XmlNode secondRoot = document.SelectSingleNode( xPath );

      // document has
      if( secondRoot != null )
      {
        // move node by xpath "/html/node()[name()!='html']"
        XmlNodeList nodeList = document.SelectNodes( "/html/node()[name()!='html']" );
        XmlNode body = document.SelectSingleNode( xPath + "/body" );

        if( body != null )
        {
          XmlNode first = body.FirstChild;
          XmlNode nd = null;

          for( int i = 0, len = nodeList.Count; i < len; i++ )
          {
            nd = nodeList[ i ];
            if( first != null )
            {
              body.InsertBefore( nd, first );
            }
            else
            {
              body.AppendChild( nd );
            }
          }
        }
        else
        {
          XmlNode nd = null;

          for( int i = 0, len = nodeList.Count; i < len; i++ )
          {
            nd = nodeList[ i ];
            secondRoot.AppendChild( nd );
          }
        }

        document.ReplaceChild( secondRoot, document.DocumentElement );
      }
    }
    /// <summary>
    /// Loads parsed data into XML document object.
    /// </summary>
    /// <param name="document">XML Document object containing output data.</param>
    /// <param name="stream">Stream with parsed data.</param>
    private void LoadDocument( XmlDocument document, Stream stream )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( m_convertor == null ) return;

      // Convert prefixes of the document.
      if( m_convertor.Prefixes == null || m_convertor.Prefixes.Count == 0 )
      {
        document.Load( stream );
      }
      else
      {
        XmlNamespaceManager xmlnsManager = new XmlNamespaceManager( document.NameTable );
        InfillPrefixes( xmlnsManager, m_convertor.Prefixes );
        XmlParserContext context = new XmlParserContext( null, xmlnsManager,
          null, XmlSpace.None );
        XmlTextReader reader = new XmlTextReader( stream, XmlNodeType.Document,
          context );

        document.Load( reader );

        reader.Close();
      }
    }
    /// <summary>
    /// Infills namespaces into namespace manager.
    /// </summary>
    /// <param name="manager">Namespace manager.</param>
    /// <param name="prefixes">Array of prefixes.</param>
    private void InfillPrefixes( XmlNamespaceManager manager, ArrayList prefixes )
    {
      if( manager == null )
        throw new ArgumentNullException( "manager" );

      if( prefixes == null || prefixes.Count == 0 ) return;

      string url = string.Empty;
      for( int i = 0, len = prefixes.Count; i < len; i++ )
      {
        string pref = ( string )prefixes[ i ];
        manager.AddNamespace( pref, url );
      }
    }
    #endregion
  }
}
