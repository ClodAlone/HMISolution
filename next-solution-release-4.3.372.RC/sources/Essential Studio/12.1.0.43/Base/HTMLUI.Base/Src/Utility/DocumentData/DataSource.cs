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
using System.Text;

using Syncfusion.HTMLUI.Base;


namespace Syncfusion.HTMLUI.Base.Utility
{
	/// <summary>
	/// This class incapsulates source data of the document.
	/// </summary>
  public class DataSource
    : IResourceProvider
  {
    #region Class constants
    /// <summary>
    /// Extension of the file.
    /// </summary>
    private const string DEF_EXT_MHT = ".mht";
    #endregion

    #region Class members
    /// <summary>
    /// Cached original content of the document's data.
    /// </summary>
    private string m_cachedContent;
    /// <summary>
    /// Object providing data of the document.
    /// </summary>
    /// <remarks>Don't use variable if there is no need. Use property instead.</remarks>
    private IResourceProvider m_dataProvider;
    /// <summary>
    /// Default data provider. It may be used for resolving path by links, web forms, etc.
    /// </summary>
    private IResourceProvider m_defaultDataProvider;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns the path to the document.
    /// </summary>
    public string FileName
    {
      get
      {
        return this.DataProvider.FileName;
      }
    }
    /// <summary>
    /// Returns the Uri to the document.
    /// </summary>
    public Uri Uri
    {
      get
      {
        return this.DataProvider.Uri;
      }
    }
    /// <summary>
    /// Returns the stream containing document's data.
    /// </summary>
    public Stream Stream
    {
      get
      {
        return this.DataProvider.Stream;
      }
    }
    /// <summary>
    /// Gets or sets the current directory for this document.
    /// </summary>
    public string RootDirectory
    {
      get
      {
        return this.DataProvider.RootDirectory;
      }
      set
      {
        this.DataProvider.RootDirectory = value;
      }
    }
    /// <summary>
    /// Indicates whether input HTML document is loaded from file.
    /// </summary>
    public bool IsFileName
    {
      get
      {
        return this.DataProvider.IsFileName;
      }
    }
    /// <summary>
    /// Indicates whether input HTML document is loaded by Uri.
    /// </summary>
    public bool IsUri
    {
      get
      {
        return this.DataProvider.IsUri;
      }
    }
    /// <summary>
    /// Indicates whether input HTML document is loaded from stream.
    /// </summary>
    public bool IsStream
    {
      get
      {
        return this.DataProvider.IsStream;
      }
    }
    /// <summary>
    /// Returns the format of the document's data.
    /// </summary>
    public DataFormat Format
    {
      get
      {
        return this.DataProvider.Format;
      }
    }
    /// <summary>
    /// Returns the default data provider.
    /// </summary>
    /// <remarks>It may be used by hyperlinks for path resolving, web forms, etc.</remarks>
    public IResourceProvider DefaultDataProvider
    {
      get
      {
        return m_defaultDataProvider;
      }
    }
    /// <summary>
    /// Returns the data provider object.
    /// </summary>
    public IResourceProvider DataProvider
    {
      get
      {
        return m_dataProvider;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Creates a new datasource object.
    /// </summary>
    private DataSource()
    {
    }
    /// <summary>
    /// Creates a new object with the specified filename.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public DataSource( string fileName )
      : this()
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );
      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName - string can not be empty" );

      DataFormat dataFormat = RecognizeDataFormat( fileName );
      m_dataProvider = CreateDataProviderFromFile( fileName, dataFormat );
      SetDefaultDataProvider( m_dataProvider );
    }
    /// <summary>
    /// Creates a new object with the specified source stream.
    /// </summary> 
    /// <param name="stream">Source stream of the data.</param>
    public DataSource( Stream stream )
      : this()
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      m_dataProvider = CreateDataProviderFromStream( stream, DataFormat.Html );
      SetDefaultDataProvider( m_dataProvider );
    }
    /// <summary>
    /// Creates a new object with the specified uri.
    /// </summary>
    /// <param name="uri">Uri of the resource.</param>
    public DataSource( Uri uri )
      : this()
    {
      if( uri == null )
        throw new ArgumentNullException( "uri" );

      m_dataProvider = CreateDataProviderFromUri( uri, DataFormat.Html );
      SetDefaultDataProvider( m_dataProvider );
    }
    /// <summary>
    /// Clears all resources used by this class.
    /// </summary>
    public void Dispose()
    {
      m_cachedContent = null;

      if( this.DataProvider != null )
      {
        this.DataProvider.Dispose();
        m_dataProvider = null;

      }
      
      GC.SuppressFinalize( this );
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Checks input properties of InputHTML class and creates
    /// TokenStream which is needed for correct conversion.
    /// </summary>
    /// <param name="bClose">Indicates whether we must close stream or not.</param>
    /// <returns>Stream which provides access to HTML document.</returns>
    public TokenStream GetInputStream( out bool bClose )
    {
      TokenStream ts = GetDocumentBody();
      bClose = false;

      if( ts != null )
      {
        CacheDocContentFromStream( ts );
        bClose = !this.IsStream;
      }

      return ts;
    }
    /// <summary>
    /// Returns the original content of the document if it's possible to retrieve.
    /// </summary>
    /// <returns>Original content of the document if it's possible to retrieve.</returns>
    public string GetDocumentContent()
    {
      return m_cachedContent;
    }
    /// <summary>
    /// Searches for a body of the document and returns data of it.
    /// </summary>
    /// <returns>Data within document's body if found; Null otherwise.</returns>
    public TokenStream GetDocumentBody()
    {
      return this.DataProvider.GetDocumentBody();
    }
    /// <summary>
    /// Indicates whether resource with specified path exists. Checks as local file as remote resource.
    /// </summary>
    /// <param name="path">Path for searching.</param>
    /// <param name="fullPath">Full path to resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>True if resource is found; False otherwise.</returns>
    public bool ResourceExists( string path, out string fullPath, out ResourceType type  )
    {
      return this.DataProvider.ResourceExists( path, out fullPath, out type );
    }
    /// <summary>
    /// Overloaded. Searches for resource by it's location inside the document.
    /// </summary>
    /// <param name="path">Path to the resource.</param>
    /// <returns>Data of resource if found; Null otherwise.</returns>
    public Stream GetResource( string path )
    {
      return this.DataProvider.GetResource( path );
    }
    /// <summary>
    /// Searches for resource by it's location inside the document.
    /// </summary>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>Data of resource if found; Null otherwise.</returns>
    public Stream GetResource( string fullPath, ResourceType type )
    {
      return this.DataProvider.GetResource( fullPath, type );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns the type of document's data.
    /// </summary>
    /// <param name="fileName">Name of the data source file.</param>
    /// <returns>Format of the data.</returns>
    private DataFormat RecognizeDataFormat( string fileName )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );
      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName - string can not be empty" );

      DataFormat dataFormat = DataFormat.Html;
      string ext = Path.GetExtension( fileName );
      ext = ext.ToLower();

      switch( ext )
      {
        case DEF_EXT_MHT :
          dataFormat = DataFormat.Mht;
          break;
      }

      return dataFormat;
    }
    /// <summary>
    /// Caches data from the document.
    /// </summary>
    /// <param name="stream">Stream data.</param>
    private void CacheDocContentFromStream( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( stream.CanSeek && stream.CanRead )
      {
        long pos = stream.Position;
        m_cachedContent = Utilities.StringFromStream( stream, Encoding.Default );
        stream.Position = pos;
      }
    }
    #endregion

    #region Class Utility methods 
    /// <summary>
    /// Creates data provider depending on the format of the data.
    /// </summary>
    /// <param name="fileName">Path to the file.</param>
    /// <param name="dataFormat">Current data format.</param>
    /// <returns>Data provider depending on the format of the data.</returns>
    private IResourceProvider CreateDataProviderFromFile( string fileName, DataFormat dataFormat )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );
      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName - string can not be empty" );

      IResourceProvider dataProvider = null;

      switch( dataFormat )
      {
        case DataFormat.Html :
          dataProvider = new HtmlDataProvider( fileName );
          break;
        case DataFormat.Mht :
          dataProvider = new MhtDataProvider( fileName );
          break;
      }

      return dataProvider;
    }
    /// <summary>
    /// Creates data provider depending on the format of the data.
    /// </summary>
    /// <param name="data">Data of the document.</param>
    /// <param name="dataFormat">Current data format.</param>
    /// <returns>Data provider depending on the format of the data.</returns>
    private IResourceProvider CreateDataProviderFromStream( Stream data, DataFormat dataFormat )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      IResourceProvider dataProvider = null;

      switch( dataFormat )
      {
        case DataFormat.Html :
          dataProvider = new HtmlDataProvider( data );
          break;
        case DataFormat.Mht :
          dataProvider = new MhtDataProvider( data );
          break;
      }

      return dataProvider;
    }
    /// <summary>
    /// Creates data provider depending on the format of the data.
    /// </summary>
    /// <param name="uri">Path to the data of the document.</param>
    /// <param name="dataFormat">Current data format</param>
    /// <returns>Data provider depending on the format of the data.</returns>
    private IResourceProvider CreateDataProviderFromUri( Uri uri, DataFormat dataFormat )
    {
      if( uri == null )
        throw new ArgumentNullException( "uri" );

      IResourceProvider dataProvider = null;

      switch( dataFormat )
      {
        case DataFormat.Html :
          dataProvider = new HtmlDataProvider( uri );
          break;
        case DataFormat.Mht :
          dataProvider = new MhtDataProvider( uri );
          break;
      }

      return dataProvider;
    }
    /// <summary>
    /// Sets the default data provider.
    /// </summary>
    /// <param name="provider">Current data provider.</param>
    private void SetDefaultDataProvider( IResourceProvider provider )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      switch( provider.Format )
      {
        case DataFormat.Html :
          m_defaultDataProvider = provider;
          break;
        case DataFormat.Mht :
          m_defaultDataProvider = CreateDefaultDataProvider( provider );
          break;
      }
    }
    /// <summary>
    /// Creates an HTML provider from the current data provider.
    /// </summary>
    /// <param name="curProvider">Current provider.</param>
    /// <returns>New default data provider.</returns>
    private IResourceProvider CreateDefaultDataProvider( IResourceProvider curProvider )
    {
      if( curProvider == null )
        throw new ArgumentNullException( "curProvider" );

      IResourceProvider defProvider = null;

      if( curProvider.IsFileName )
      {
        defProvider = new HtmlDataProvider( curProvider.FileName );
      }
      else if( curProvider.IsStream )
      {
        defProvider = new HtmlDataProvider( curProvider.Stream );
      }
      else if( curProvider.IsUri )
      {
        defProvider = new HtmlDataProvider( curProvider.Uri );
      }

      return defProvider;
    }
    #endregion
  }
}
