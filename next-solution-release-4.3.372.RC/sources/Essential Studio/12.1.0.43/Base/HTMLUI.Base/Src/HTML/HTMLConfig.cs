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
using System.Xml;
using System.IO;

using System.Diagnostics;
using System.Reflection;

using Syncfusion.HTMLUI.Base.Utility;
#endregion

namespace Syncfusion.HTMLUI.Base.Parser.HTML
{
  /// <summary>
  /// This class is used by pre-parser as configuration object.
  /// </summary>
  internal class HTMLConfig
  {
    #region Class constants
    /// <summary>
    /// Resource path for first config file.
    /// </summary>
    internal  const string DEF_CONFIG1 = "Syncfusion.HTMLUI.Base.Configuration.config1.xml";
    /// <summary>
    /// Resource path for second config file.
    /// </summary>
    internal  const string DEF_CONFIG2 = "Syncfusion.HTMLUI.Base.Configuration.config2.xml";
    #endregion

    #region Class static members
    /// <summary>
    /// Member holds default config based on config files from resources.
    /// </summary>
    private static HTMLConfig _config = null;
    /// <summary>
    /// First configuration XML document.
    /// </summary>
    private static XmlDocument _config1 = null;
    /// <summary>
    /// Second configuration XML document.
    /// </summary>
    private static XmlDocument _config2 = null;
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary holds elements known to pre-parser.
    /// </summary>
    private IDictionary m_elements = new SortedListEx();
    /// <summary>
    /// Dictionary holds entities known to pre-parser.
    /// </summary>
    private IDictionary m_entities = new SortedListEx();
    /// <summary>
    ///
    /// </summary>
    private IDictionary m_cache = new Hashtable();
    #endregion

    #region Class static properties
    /// <summary>
    /// Extracts main config from assembly. Property returns configuration
    /// based on config files from assembly resources.
    /// </summary>
    public static HTMLConfig MainConfig
    {
      get
      {
        if( _config == null )
        {
          Assembly  asm = Assembly.GetExecutingAssembly();
          Stream sr1 = asm.GetManifestResourceStream( DEF_CONFIG1 );
          Stream sr2 = asm.GetManifestResourceStream( DEF_CONFIG2 );
          _config = new HTMLConfig( sr1, sr2 );
        }

        return _config;
      }
    }
    /// <summary>
    /// Returns the first config stream from resources.
    /// </summary>
    public static Stream Config1
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_CONFIG1 );
      }
    }
    /// <summary>
    /// Returns the second config stream from resources.
    /// </summary>
    public static Stream Config2
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_CONFIG2 );
      }
    }
    /// <summary>
    /// Returns the first default configuration file as XML document.
    /// </summary>
    public static XmlDocument XMLConfig1
    {
      get
      {
        if( _config1 == null )
        {
          _config1 = new XmlDocument();
          _config1.Load( HTMLConfig.Config1 );
        }

        return _config1;
      }
    }
    /// <summary>
    /// Returns the second default configuration file as XML document.
    /// </summary>
    public static XmlDocument XMLConfig2
    {
      get
      {
        if( _config2 == null )
        {
          _config2 = new XmlDocument();
          _config2.Load( HTMLConfig.Config2 );
        }

        return _config2;
      }
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Returns the elements as Read-only.
    /// </summary>
    public IDictionary Elements
    {
      get
      {
        return m_elements;
      }
    }

    /// <summary>
    /// Returns the entities as Read-only.
    /// </summary>
    public IDictionary Entities
    {
      get
      {
        return m_entities;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Hides default constructor to prevent construction without properties.
    /// </summary>
    private HTMLConfig()
    {
    }
    /// <summary>
    /// Main constructor of configuration object.
    /// </summary>
    /// <param name="stream1">First config file which holds pre-parser elements rules.</param>
    /// <param name="stream2">Second config files which holds entities information.</param>
    public HTMLConfig( Stream stream1, Stream stream2 )
    {
      if( stream1 == null )
        throw new ArgumentNullException( "stream1" );

      if( stream2 == null )
        throw new ArgumentNullException( "stream2" );

      XmlDocument xmlDoc1 = new XmlDocument();
      XmlDocument xmlDoc2 = new XmlDocument();

      xmlDoc1.Load( stream1 );
      xmlDoc2.Load( stream2 );

      XmlNamespaceManager nsm = new XmlNamespaceManager( new NameTable() );
      nsm.AddNamespace( "xs", "http://www.w3.org/1999/XMLSchema" );

      ExtractElements( xmlDoc1, xmlDoc2, nsm );
      ExtractEntities( xmlDoc2 );
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Extracts from config files pre-parser rules and configuration information.
    /// </summary>
    /// <param name="document">Config file with pre-parser rules.</param>
    /// <param name="document2">Config file with entities rules.</param>
    /// <param name="nsm">namespaces</param>
    private void ExtractElements( XmlDocument document, XmlDocument document2, XmlNamespaceManager nsm )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      if( document2 == null )
        throw new ArgumentNullException( "document2" );

      if( nsm == null )
        throw new ArgumentNullException( "nsm" );

      XmlNodeList nodeList = document.SelectNodes( "/xs:root/xs:element", nsm );
      XmlNode node = null;

      for( int i = 0, len = nodeList.Count; i < len; i++ )
      {
        node = nodeList[ i ];
        string elmName = node.Attributes[ "name" ].Value;
        string name = "/root/el[ @name='" + elmName + "' ]";

        HTMLElement e = new HTMLElement( "html:" + elmName );

        XmlAttribute expression = node.Attributes[ "expression" ];
        if( expression != null )
        {
          e.Expression = expression.Value;
        }

        foreach( XmlElement element in document2.SelectNodes( name ) )
        {
          XmlAttribute startTag = element.Attributes[ "starttag" ];
          if( startTag != null ) e.TagStart = startTag.Value;

          XmlAttribute endTag = element.Attributes[ "endtag" ];
          if( endTag != null ) e.TagEnd = endTag.Value;

          XmlAttribute cdata = element.Attributes[ "cdata" ];
          if( cdata != null ) e.IsCDATA = ( Utilities.StrEquals( cdata.Value, "yes" ) );

          XmlAttribute preserve = element.Attributes[ "trim-whitespace" ];
          if( preserve != null ) e.PreserveWhiteSpaces = ( Utilities.StrEquals( preserve.Value, "no" ) );
        }

        m_elements.Add( e.Name, e );
      }

      foreach( HTMLElement elm in m_elements.Values )
      {
        ExtractElementInfo( document, elm, nsm );
      }
    }

    /// <summary>
    /// Extracts from config file element with additional information.
    /// </summary>
    /// <param name="document">Config file with pre-parser rules.</param>
    /// <param name="element">Element with additional info that must be extracted.</param>
    /// <param name="nsm">Namespaces.</param>
    private bool ExtractElementInfo( XmlDocument document, HTMLElement element, XmlNamespaceManager nsm )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      if( element == null )
        throw new ArgumentNullException( "element" );

      if( nsm == null )
        throw new ArgumentNullException( "nsm" );
      bool result = false;

      if( element.Name.Length > 5 )
      {
        string name = "/xs:root/xs:element[@name='" + element.SubName + "']/xs:e";

        XmlNodeList list = null;
        if( m_cache.Contains( name ) )
        {
          list = m_cache[ name ] as XmlNodeList;
        }
        else
        {
          list = document.SelectNodes( name, nsm );
          m_cache[ name ] = list;
        }

        foreach( XmlNode node in list )
        {
          string subName = node.Attributes[ "name" ].Value;
          HTMLElement sub = m_elements[ subName ] as HTMLElement;

          if( sub != null )
          {
            if( Utilities.StrEquals( sub.TagStart, "o" ) )
            {
              element.Optional.Add( sub.Name, sub );
            }

            element.Children.Add( sub.Name, sub );
            result = true;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Extracts entities from config files.
    /// </summary>
    /// <param name="document2">Config file with entities rules.</param>
    private void ExtractEntities( XmlDocument document2 )
    {
      if( document2 == null )
        throw new ArgumentNullException( "document2" );

      foreach( XmlNode element in document2.SelectNodes( "/root/entities/entity" ) )
      {
        string entityName = element.Attributes[ "name" ].Value;
        string entityValue = element.Attributes[ "value" ].Value;

        try
        {
          m_entities.Add( entityName, int.Parse( entityValue ) );
        }
        catch( Exception ex )
        {
          // NOTE: Skip entity if its value attribute can not be converted to int.
          Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );
          Debug.WriteLine( "value is == '" + entityValue + "'", "Wrong entity" );
          throw;
        }
      }
    }
    #endregion
  }
}