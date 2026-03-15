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
using System.Collections;
using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Readonly copy for custom bookmarks collection.
	/// </summary>
	public class CustomBookmarksCollection
    : BookmarksCollection
	{
    #region Class Initialization/Finalization
    /// <summary>
    /// Creates and initalizes new bookmarks collection.
    /// </summary>
    /// <param name="baseCollection">Base Collection of bookmarks.</param>
    internal protected CustomBookmarksCollection( IList baseCollection )
      : base( baseCollection )
    {}
    #endregion
    
    #region Class Properties
    /// <summary>
    /// Gets custom bookmark by the index in collection.
    /// </summary>
    public new ICustomBookmark this[ int index ]
    {
      get
      {
        return ( ICustomBookmark )base[ index ];
      }
    }
    #endregion
  }
}
