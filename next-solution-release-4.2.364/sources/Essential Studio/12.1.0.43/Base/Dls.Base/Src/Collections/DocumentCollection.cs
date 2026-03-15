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
#endregion

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents a document collection.
  /// </summary>
  public class DocumentCollection
    : EntityCollectionBase,
      IDocumentCollection
  {
    #region Class properties
    /// <summary>
    /// Get document by index
    /// </summary>
    public IDocument this[ int index ]
    {
      get
      {
        return ( IDocument )List[ index ];
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    public DocumentCollection()
      : base( null )
    {}
    #endregion
  }
}