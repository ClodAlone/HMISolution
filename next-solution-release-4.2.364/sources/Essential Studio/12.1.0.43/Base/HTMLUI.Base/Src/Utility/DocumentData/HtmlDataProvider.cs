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
using System.Diagnostics;

using Syncfusion.HTMLUI.Base;


namespace Syncfusion.HTMLUI.Base.Utility
{
  /// <summary>
  /// Thsi class works with documents in MHT format. It searches resources inside this document
  /// and returns it on demand.
  /// </summary>
  internal class HtmlDataProvider
    : DataProvider
  {
    #region Class properties
    /// <summary>
    /// Returns the format of the resource provider.
    /// </summary>
    public override DataFormat Format
    {
      get
      {
        return DataFormat.Html;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Creates a new object.
    /// </summary>
    /// <param name="fileName">Path to the HTML document.</param>
    public HtmlDataProvider( string fileName )
      : base( fileName )
    {
    }
    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="data">Data of the HTML document.</param>
    public HtmlDataProvider( Stream data )
      : base( data )
    {
    }
    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="uri">Path to the data of the HTML document.</param>
    public HtmlDataProvider( Uri uri )
      : base( uri )
    {
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Searches for a body of the document and returns data in it.
    /// </summary>
    /// <returns>Data of the document's body if found; Null otherwise.</returns>
    public override TokenStream GetDocumentBody()
    {
      TokenStream ts = null;

      if( this.IsFileName )
      {
        string filePath = Utilities.RemoveBookmark( this.FileName );
        ts = new TokenStream( filePath );
      }
      else if( this.IsUri )
      {
        // Get a stream object containing the HTML file.
        Stream s = Utilities.StreamFromUrl( this.Uri );
        ts = new TokenStream( s );
      }
      else if( this.IsStream )
      {
        if( this.Stream is TokenStream )
        {
          ts = ( TokenStream )this.Stream;
        }
        else
        {
          ts = new TokenStream( this.Stream );
        }
      }
      else
      {
        throw new ArgumentNullException( "input", "InputHTML class does not contains any reference on input document." );
      }

      return ts;
    }
    /// <summary>
    /// Indicates whether a resource with specified path exists. Checks as local file as remote resource.
    /// </summary>
    /// <param name="path">Path for searching.</param>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>True if resource is found; False otherwise.</returns>
    public override bool ResourceExists( string path, out string fullPath, out ResourceType type  )
    {
      type = ResourceType.Unknown;
      bool bExists = false;
      Uri uri;

      // Remove bookmark.
      string tmpPath = Utilities.RemoveBookmark( path );

      if( Utilities.IsFileExists( this.RootDirectory, tmpPath, out fullPath ) )
      {
        bExists = true;
        type = ResourceType.LocalResource;
      }
      else if( Utilities.IsUriExists( this.RootDirectory, tmpPath, out uri ) )
      {
        bExists = true;
        type = ResourceType.RemoteResource;
        fullPath = uri.AbsoluteUri;
      }

      // Add bookmark.
      fullPath += Utilities.GetBookmark( path );

      return bExists;
    }
    /// <summary>
    /// Searches for the resource by it's location inside the document.
    /// </summary>
    /// <param name="path">Path to the resource.</param>
    /// <returns>Data of the resource if found; Null otherwise.</returns>
    public override Stream GetResource( string path )
    {
      if( path == null )
        throw new ArgumentNullException( "path" );
      if( path.Length == 0 )
        throw new ArgumentException( "path - string can not be empty" );

      Stream result = null;
      string fullPath;
      ResourceType type;

      if( ResourceExists( path, out fullPath, out type ) )
      {
        result = GetResource( fullPath, type );
      }

      return result;
    }
    /// <summary>
    /// Searches for the resource by it's location inside the document.
    /// </summary>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>Data of the resource if found; Null otherwise.</returns>
    public override Stream GetResource( string fullPath, ResourceType type )
    {
      // Remove bookmark.
      fullPath = Utilities.RemoveBookmark( fullPath );
      Stream result = null;
      
      if( type == ResourceType.LocalResource )
      {
        result = Utilities.StreamFromFile( this.RootDirectory, fullPath );
      }
      else if( type == ResourceType.RemoteResource )
      {
        try
        {
          Uri uri = new Uri( fullPath );
          result  = Utilities.StreamFromUrl( uri );
        }
        catch( UriFormatException ex )
        {
          Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );
        }
      }

      return result;
    }
    #endregion
  }
}
