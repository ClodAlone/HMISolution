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
	/// Readonly copy for bookmarks collection.
	/// </summary>
	public class BookmarksCollection
    : ICollection
	{
    #region Class Internal Members
    /// <summary>
    /// Internal list exposed with the collection.
    /// </summary>
    private IList m_list;
    #endregion

    #region Class Initialization/Finalization
    /// <summary>
    /// Creates and initalizes new bookmarks collection.
    /// </summary>
    /// <param name="baseCollection">Base Collection of bookmarks.</param>
		internal protected BookmarksCollection( IList baseCollection )
		{
			m_list = new ArrayList( baseCollection );  
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets bookmark by the index in collection.
    /// </summary>
    public IBookmark this[ int index ]
    {
      get
      {
        return ( IBookmark )m_list[ index ];
      }
    }
    /// <summary>
    /// Gets value indication whether collection is synchronized. Always returns false.
    /// </summary>
    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Gets count of the bookmarks.
    /// </summary>
    public int Count
    {
      get
      {
        return m_list.Count;
      }
    }

    /// <summary>
    /// Gets syncroot of the collection.
    /// </summary>
    object ICollection.SyncRoot
    {
      get
      {
        return m_list.SyncRoot;
      }
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Copies items to the specified array.
    /// </summary>
    /// <param name="array">Array that will contain copy of the collection.</param>
    /// <param name="index">Item of the first item in array.</param>
    public void CopyTo(Array array, int index)
    {
      m_list.CopyTo( array, index );
    }
    /// <summary>
    /// Gets enumerator for the collection items.
    /// </summary>
		/// <returns>IEnumerator.</returns>
    public IEnumerator GetEnumerator()
    {
      return m_list.GetEnumerator();
    }
    #endregion
  }
}
