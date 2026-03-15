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
using System.IO;
using System.Collections;
using System.Collections.Specialized;

using Syncfusion.HTMLUI.Base;

using Syncfusion.MIME;


namespace Syncfusion.HTMLUI.Base.Utility
{
	/// <summary>
	/// Class working with documents in MHT format. It searches resources inside this document
	///  and returnes it on demand.
	/// </summary>
  internal class MhtDataProvider
    : DataProvider
  {
    #region Class constants
    /// <summary>
    /// Key by which we will separate parts.
    /// </summary>
    private const string DEF_PART_KEY = "Content-Location";
    /// <summary>
    /// Key for document part.
    /// </summary>
    private const string DEF_BODY_KEY = "Content-Type";
    /// <summary>
    /// Value of the body's key.
    /// </summary>
    private const string DEF_BODY_VALUE = "text/html";
    #endregion

    #region Class members
    /// <summary>
    /// Document controlling MIME document.
    /// </summary>
    private IMIMEDocument m_mimeDocument;
    /// <summary>
    /// When document is created from uri we must get data from Uri and then close stream.
    /// </summary>
    private Stream m_streamFromUri;
    /// <summary>
    /// Dictionary of parts inside Mime document.
    /// </summary>
    private Hashtable m_parts;
    /// <summary>
    /// Data of the HTML.
    /// </summary>
    private IMIMEPart m_bodyPart;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets object containing document in MHT format.
    /// </summary>
    private IMIMEDocument MimeDocument
    {
      get
      {
        if( m_mimeDocument == null )
        {
          m_mimeDocument = new MIMEDocument();
        }

        return m_mimeDocument;
      }
    }
    /// <summary>
    /// Gets parts of Mime document.
    /// </summary>
    private Hashtable Parts
    {
      get
      {
        return m_parts;
      }
    }
    /// <summary>
    /// Gets Format of resource provider.
    /// </summary>
    public override DataFormat Format
    {
      get
      {
        return DataFormat.Mht;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="fileName">Path to MHT document.</param>
    public MhtDataProvider( string fileName )
      : base( fileName )
    {
      bool bRaiseException = true;
      string fullPath;

      if( Utilities.IsFileExists( this.RootDirectory, this.FileName, out fullPath ) )
      {
        // User set path as relative.
        if( fullPath != this.FileName )
        {
          SetFileName( fullPath );
        }
        
        this.MimeDocument.Load( this.FileName );
        bRaiseException = false;
        InitializeData( this.MimeDocument );
      }

      if( bRaiseException )
      {
        throw new ArgumentException( "Can't load data from " + this.FileName );
      }
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="data">Data of MHT document.</param>
    public MhtDataProvider( Stream data )
      : base( data )
    {
      this.MimeDocument.Load( this.Stream );
      InitializeData( this.MimeDocument );
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="uri">Path to data of MHT document.</param>
    public MhtDataProvider( Uri uri )
      : base( uri )
    {
      bool bRaiseException = true;
      Uri fullUri;
      
      if( Utilities.IsUriExists( this.RootDirectory, uri.AbsoluteUri, out fullUri ) )
      {
        // User set not absolute Uri.
        if( this.Uri.AbsoluteUri != fullUri.AbsoluteUri )
        {
          SetUri( fullUri );
        }
        
        m_streamFromUri = Utilities.StreamFromUrl( this.Uri );

        if( m_streamFromUri != null )
        {
          this.MimeDocument.Load( m_streamFromUri );
          bRaiseException = false;
          InitializeData( this.MimeDocument );
        }
      }
      
      if( bRaiseException )
      {
        throw new ArgumentException( "Can't load data from " + uri.ToString() );
      }
    }
    /// <summary>
    /// Disposes object.
    /// </summary>
    public override void Dispose()
    {
      base.Dispose();

      if( m_streamFromUri != null )
      {
        m_streamFromUri.Close();
        m_streamFromUri = null;
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Searches for a body of the document and returns data of it.
    /// </summary>
    /// <returns>Data of document's body if found, Null otherwise.</returns>
    public override TokenStream GetDocumentBody()
    {
      TokenStream ts = null;

      if( m_bodyPart != null )
      {
        byte[] buff = ReadData( m_bodyPart );
        MemoryStream ms = new MemoryStream( buff );
        ts = new TokenStream( ms );
      }

      return ts;
    }
    /// <summary>
    /// Checks if such resource exists. Checks as local file as remote resource.
    /// </summary>
    /// <param name="path">Path for searching.</param>
    /// <param name="fullPath">Full path to resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>True - if resource forund, False otherwise.</returns>
    public override bool ResourceExists( string path, out string fullPath, out ResourceType type  )
    {
      fullPath = string.Empty;
      type = ResourceType.Unknown;
      bool bExists = false;

      if( this.Parts.ContainsKey( path ) )
      {
        fullPath = path;
        type = ResourceType.LocalResource;
        bExists = true;
      }

      return bExists;
    }
    /// <summary>
    /// Searches for resource by it's location inside of document.
    /// </summary>
    /// <param name="path">Path to the resource.</param>
    /// <returns>Data of resource if found, Null otherwise.</returns>
    public override Stream GetResource( string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );
      if( path.Length == 0 )
        throw new ArgumentException( "path - string can not be empty" );

      Stream data = null;
      string fullPath;
      ResourceType type;

      if( ResourceExists( path, out fullPath, out type ) )
      {
        data = GetResource( fullPath, type );
      }

      return data;
    }
    /// <summary>
    /// Searches for resource by it's location inside of document.
    /// </summary>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>Data of resource if found, Null otherwise.</returns>
    public override Stream GetResource( string fullPath, ResourceType type )
    {
      Stream data = null;

      IMIMEPart part = this.Parts[ fullPath ] as IMIMEPart;

      if( part != null )
      {
        byte[] buff = ReadData( part );
        data = new MemoryStream( buff );
      }

      return data;
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Make all arrangement before using Mime.
    /// </summary>
    /// <param name="mimeDocument">Document containing Mime data.</param>
    private void InitializeData( IMIMEDocument mimeDocument )
    {
      if( mimeDocument == null )
        throw new ArgumentNullException( "mimeDocument" );

      if( m_parts == null )
      {
        m_parts = CollectionsUtil.CreateCaseInsensitiveHashtable();
      }

      InfillParts( this.Parts, mimeDocument );
    }
    /// <summary>
    /// Infills parts dictionary.
    /// </summary>
    /// <param name="parts">Dictionary of parts.</param>
    /// <param name="mimeDocument">Document containing Mime data.</param>
    private void InfillParts( Hashtable parts, IMIMEDocument mimeDocument )
    {
      if( parts == null )
        throw new ArgumentNullException( "parts" );
      if( mimeDocument == null )
        throw new ArgumentNullException( "mimeDocument" );

      if( mimeDocument.Parts.Count > 0 )
      {
        for( int i = 0, len = mimeDocument.Parts.Count; i < len; i++ )
        {
          IMIMEPart part = mimeDocument.Parts[ i ];
          ProcessPart( parts, part );
        }
      }
      else if( mimeDocument.MessagePart != null )
      {
        ProcessPart( parts, mimeDocument.MessagePart );
      }
    }
    /// <summary>
    /// Returns part by its path if exists.
    /// </summary>
    /// <param name="path">Path to resource.</param>
    /// <returns>Part by its path if exists, Null otherwise.</returns>
    private IMIMEPart GetPart( string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );

      return ( this.Parts[ path ] as IMIMEPart );
    }
    /// <summary>
    /// Reads data from the part.
    /// </summary>
    /// <param name="part">Part from which data would be extracted.</param>
    /// <returns>Array of data in the part.</returns>
    private byte[] ReadData( IMIMEPart part )
    {
      if( part == null )
        throw new ArgumentNullException( "part" );

      byte[] buff = new byte[ part.Data.Length ];
      part.Data.Position = 0;

      int length = part.Data.Read( buff, 0, buff.Length );
      
      byte[] realBuff = new byte[ length ];
      Array.Copy( buff, 0, realBuff, 0, length );

      return realBuff;
    }
    /// <summary>
    /// Processes part.
    /// </summary>
    /// <param name="parts">Dictionary of parts.</param>
    /// <param name="part">Part object.</param>
    private void ProcessPart( Hashtable parts, IMIMEPart part )
    {
      if( parts == null )
        throw new ArgumentNullException( "parts" );
      
      if( part != null )
      {
        IMIMEHeader header = part.Header;
        string key = header[ DEF_PART_KEY ];

        if( key != null && key.Length > 0 )
        {
          this.Parts[ key ] = part;
        }

        // Try to find main part with HTML data.
        string bodyKey = header[ DEF_BODY_KEY ];
        if( bodyKey != null && bodyKey.Length > 0 )
        {
          bodyKey = bodyKey.Trim();

          // We found main part.
          if( Utilities.StrEquals( bodyKey, DEF_BODY_VALUE ) )
          {
            m_bodyPart = part;
          }
        }
      }
    }
    #endregion
  }
}
