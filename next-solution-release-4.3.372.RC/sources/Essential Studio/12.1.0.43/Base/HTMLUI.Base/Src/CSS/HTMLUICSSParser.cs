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
using System.Xml;
using System.Text;
using System.Diagnostics;

using Syncfusion.HTMLUI.Base.Parser.CSS;
#endregion

namespace Syncfusion.HTMLUI.Base
{
  /// <summary>
  /// This class is responsible for CSS document parsing.
  /// </summary>
  public class HTMLUICSSParser
  {
    #region Class constants
    private const int DEF_BUFFER_SIZE = 8192;
    #endregion

    #region Class members
    /// <summary>
    /// Converts CSS document into an XML document.
    /// </summary>
    private CSSParser   m_parser;
    /// <summary>
    /// Holds XML document after converting.
    /// </summary>
    private XmlDocument m_document;
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets the XML document.
    /// </summary>
    public XmlDocument Document
    {
      get
      {
        return m_document;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded Constructor. Initializes new object.
    /// </summary>
    public HTMLUICSSParser()
    {
      m_parser = new CSSParser();
    }
    /// <summary>
    /// Initializes new object.
    /// </summary>
    /// <param name="fileName">Path to file for parsing.</param>
    public HTMLUICSSParser( string fileName ) : this()
    {
      m_document = Parse( fileName );
    }
    /// <summary>
    /// Initializes new object from the stream.
    /// </summary>
    /// <param name="stream">Token stream object.</param>
    public HTMLUICSSParser( TokenStream stream ) : this()
    {
      m_document = Parse( stream );
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Overloaded Utility method. Converts file to XML.
    /// </summary>
    /// <param name="fileName">Path to CSS document.</param>
    public XmlDocument Parse( string fileName )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName - string can not be empty" );

      XmlDocument document;

      using( TokenStream file = new TokenStream( fileName ) )
      {
        document = this.Parse( file );
        file.Close();
      }

      return document;
    }
    /// <summary>
    /// Utility method. Converts file to XML.
    /// </summary>
    /// <param name="stream">Token stream object.</param>
    public XmlDocument Parse( TokenStream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      XmlDocument document = new XmlDocument();

      using( Stream streamOut = ConvertCSSToXML( stream ) )
      {
        document.Load( streamOut );
      }

      return document;
    }
    /// <summary>
    /// Converts CSS string which contains style declaration to XML document.
    /// </summary>
    /// <param name="styleDeclaration">Declaration of CSS style(s).</param>
    /// <returns>Document which contains an XML interpretation of the CSS document.</returns>
    public XmlDocument ParseString( string styleDeclaration )
    {
      if( styleDeclaration == null )
        throw new ArgumentNullException( "styleDeclaration" );

      if( styleDeclaration.Length == 0 )
        throw new ArgumentException( "styleDeclaration - string can not be empty" );

      XmlDocument document = new XmlDocument();

      using( TokenStream tokens = TokenStream.FromString( styleDeclaration ) )
      {
        return Parse( tokens );
      }
    }

    #endregion

    #region Class utility methods
    /// <summary>
    /// Converts to XML.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <returns>The base stream infilled by parser.</returns>
    protected virtual Stream ConvertCSSToXML( TokenStream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      MemoryStream ms = new MemoryStream( DEF_BUFFER_SIZE );
      XmlTextWriter output = new XmlTextWriter( ms, Encoding.Unicode );

      try
      {
        m_parser.ConfigureParser( stream, output );
        m_parser.Process();
      }
      catch( Exception e )
      {
        Debug.WriteLine( e.Message +
          Environment.NewLine + e.StackTrace, "Parse Exception" );

        // Close output stream for parser on exception - free memory.
        ms.Capacity = 0;
        ms.SetLength( 0 );
        output.Close();

        throw new ParseException( e );
      }

      output.Flush();

      ms.Position = 0;
      return ms;
    }
    #endregion
  }
}