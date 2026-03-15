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

using Syncfusion.HTMLUI.Base;

namespace Syncfusion.HTMLUI.Base.Utility
{
	/// <summary>
	/// Base class for all data provider classes.
	/// </summary>
	internal abstract class DataProvider : IResourceProvider
	{
    #region Class members
    /// <summary>
    /// Name of the source file.
    /// </summary>
    private string m_fileName;
    /// <summary>
    /// Source data stream.
    /// </summary>
    private Stream m_stream;
    /// <summary>
    /// Uri of the resource.
    /// </summary>
    private Uri m_uri;
    /// <summary>
    /// Root directory for the document.
    /// </summary>
    private string m_rootDir = string.Empty;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns the path to the document.
    /// </summary>
    public string FileName
    {
      get
      {
        return m_fileName;
      }
    }
    /// <summary>
    /// Returns the Uri to the document.
    /// </summary>
    public Uri Uri
    {
      get
      {
        return m_uri;
      }
    }
    /// <summary>
    /// Returns the stream containing the document's data.
    /// </summary>
    public Stream Stream
    {
      get
      {
        return m_stream;
      }
    }
    /// <summary>
    /// Indicates whether input HTML document is loaded from file.
    /// </summary>
    public bool IsFileName
    {
      get
      {
        return ( m_fileName != null );
      }
    }
    /// <summary>
    /// Indicates whether input HTML document is loaded by Uri.
    /// </summary>
    public bool IsUri
    {
      get
      {
        return ( m_uri != null );
      }
    }
    /// <summary>
    /// Indicates whether input HTML document is loaded from stream.
    /// </summary>
    public bool IsStream
    {
      get
      {
        return ( m_stream != null );
      }
    }
    /// <summary>
    /// Gets or sets root directory for the document.
    /// </summary>
    public virtual string RootDirectory
    {
      get
      {
        return m_rootDir;
      }
      set
      {
        if( m_rootDir != value )
        {
          m_rootDir = value;
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Creates a new object.
    /// </summary>
    private DataProvider()
    {
    }
    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="fileName">Path to the HTML document.</param>
    public DataProvider( string fileName )
      : this()
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );
      
      SetFileName( fileName );
    }
    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="data">Data of the HTML document.</param>
    public DataProvider( Stream data )
      : this()
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      m_stream = data;
    }
    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="uri">Path to the data of the HTML document.</param>
    public DataProvider( Uri uri )
      : this()
    {
      if( uri == null )
        throw new ArgumentNullException( "uri" );

      SetUri( uri );
    }
    /// <summary>
    /// Clears all objects.
    /// </summary>
    public virtual void Dispose()
    {
      m_fileName = null;
      m_stream = null;
      m_uri = null;
    }
    #endregion

    #region Class abstract methods
    /// <summary>
    /// Returns the format of the resource provider.
    /// </summary>
    public abstract DataFormat Format{ get; }
    /// <summary>
    /// Searches for a body of the document and returns the data in it.
    /// </summary>
    /// <returns>Data of the document's body if found; Null otherwise.</returns>
    public abstract TokenStream GetDocumentBody();
    /// <summary>
    ///Indicates whether a resource with specified path exists. Checks as local file as remote resource.
    /// </summary>
    /// <param name="path">Path for searching.</param>
    /// <param name="fullPath">Full path to resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>True - if resource forund, False otherwise.</returns>
    public abstract bool ResourceExists( string path, out string fullPath, out ResourceType type  );
    /// <summary>
    /// Overloaded. Searches for resource by it's location inside the document.
    /// </summary>
    /// <param name="path">Path to the resource.</param>
    /// <returns>Data of resource if found, Null otherwise.</returns>
    public abstract Stream GetResource( string path );
    /// <summary>
    /// Searches for resource by it's location inside the document.
    /// </summary>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>Data of resource if found, Null otherwise.</returns>
    public abstract Stream GetResource( string fullPath, ResourceType type );
    #endregion

    #region Class protected methods
    /// <summary>
    /// Sets the FileName property.
    /// </summary>
    /// <param name="fileName">New value of FileName property.</param>
    protected void SetFileName( string fileName )
    {
      m_fileName = fileName;

      SetDocDirFromFile( fileName );
    }
    /// <summary>
    /// Sets the Stream property.
    /// </summary>
    /// <param name="stream">New value of Stream property.</param>
    protected void SetStream( Stream stream )
    {
      m_stream = stream;
    }
    /// <summary>
    /// Sets the Uri property.
    /// </summary>
    /// <param name="uri">New value of Uri property.</param>
    protected void SetUri( Uri uri )
    {
      m_uri = uri;

      SetDocDirFromUri( uri );
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Sets the root current directory to the document.
    /// </summary>
    /// <param name="fileName">Path to the file.</param>
    private void SetDocDirFromFile( string fileName )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );
      if( fileName.Length == 0 )
        throw new ArgumentException( "fileName - string can not be empty" );

      this.RootDirectory = Path.GetDirectoryName( fileName );
    }
    /// <summary>
    /// Sets the root current directory to the document.
    /// </summary>
    /// <param name="uri">Path to the file.</param>
    private void SetDocDirFromUri( Uri uri )
    {
      if( uri == null )
        throw new ArgumentNullException( "uri" );

      int index = uri.AbsoluteUri.LastIndexOf( "/" );

      if( index != -1 )
      {
        string strUri = uri.AbsoluteUri.Substring( 0, index );
        this.RootDirectory = strUri;
      }
    }
    #endregion
	}
}
