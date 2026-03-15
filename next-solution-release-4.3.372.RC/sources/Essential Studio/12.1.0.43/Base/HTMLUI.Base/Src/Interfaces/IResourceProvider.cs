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

using Syncfusion.HTMLUI.Base.Utility;


namespace Syncfusion.HTMLUI.Base
{
	/// <summary>
	/// Interface that declares the functionality for objects
	/// exposing data of the document.
	/// </summary>
	public interface IResourceProvider : IDisposable
	{
    /// <summary>
    /// Indicates whether input HTML document is loaded from file.
    /// </summary>
    bool IsFileName{ get; }
    /// <summary>
    /// Indicates whether input HTML document is loaded by Uri.
    /// </summary>
    bool IsUri{ get; }
    /// <summary>
    /// Indicates whether input HTML document is loaded from stream.
    /// </summary>
    bool IsStream{ get; }
    /// <summary>
    /// Returns the source for parser is file, otherwise must be null.
    /// Property has higher priority
    /// </summary>
    string FileName{ get; }
    /// <summary>
    /// Returns the source for parser specified by URI, otherwise must be null.
    /// Second property by priority for source checks.
    /// </summary>
    Uri Uri{ get; }
    /// <summary>
    /// Returns the source specified as stream, otherwise must be null.
    /// Has lowest priority.
    /// </summary>
    Stream Stream{ get; }
    /// <summary>
    /// Gets or sets root directory for the document.
    /// </summary>
    string RootDirectory{ get; set; }
    /// <summary>
    /// Returns the format of resource provider.
    /// </summary>
    DataFormat Format{ get; }
    /// <summary>
    /// Searches for a body of the document and returns data contained in it.
    /// </summary>
    /// <returns>Data of document's body if found; Null otherwise.</returns>
    TokenStream GetDocumentBody();
    /// <summary>
    /// Indicates whether such resource exists. Checks as local file as remote resource.
    /// </summary>
    /// <param name="path">Path for searching.</param>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>True if resource found; False otherwise.</returns>
    bool ResourceExists( string path, out string fullPath, out ResourceType type  );
    /// <summary>
    /// Overloaded. Searches for resource by it's location inside of document.
    /// </summary>
    /// <param name="path">Path to the resource.</param>
    /// <returns>Data of resource if found; Null otherwise.</returns>
    Stream GetResource( string path );
    /// <summary>
    /// Searches for resource by it's location inside of document.
    /// </summary>
    /// <param name="fullPath">Full path to the resource.</param>
    /// <param name="type">Type of resource.</param>
    /// <returns>Data of resource if found; Null otherwise.</returns>
    Stream GetResource( string fullPath, ResourceType type );
	}
}
