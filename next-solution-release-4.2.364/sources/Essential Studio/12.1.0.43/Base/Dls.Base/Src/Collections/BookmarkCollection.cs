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
  /// Represents a Bookmarks collection in the document.
  /// </summary>
  public class BookmarkCollection : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Gets Bookmark object by Name
    /// </summary>
    public Bookmark this[ string name ]
    {
      get
      {
        return FindByName( name );
      }
    }
    /// <summary>
    /// Gets Bookmark object by Index
    /// </summary>
    public Bookmark this[ int index ]
    {
      get
      {
        return List[ index ] as Bookmark;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates a new Bookmark object for the specified document
    /// </summary>
    /// <param name="doc">The document.</param>
    public BookmarkCollection( IDocument doc )
      : base( doc )
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Finds Bookmark object by specified Name
    /// </summary>
    /// <param name="name">The bookmark name</param>
    /// <returns></returns>
    public Bookmark FindByName( string name )
    {
      for( int i = 0; i < List.Count; i++ )
      {
        Bookmark bookmark = List[ i ] as Bookmark;

        if( bookmark.Name == name )
        {
          return bookmark;
        }
      }

      return null;
    }
    /// <summary>
    /// Adds Bookmark object to the collection
    /// </summary>
    /// <param name="bookmark"></param>
    public void Add( Bookmark bookmark )
    {
      List.Add( bookmark );
    }
    /// <summary>
    /// Sets BookmarkEnd for the bookmark specified by Name
    /// </summary>
    /// <param name="bookmarkEnd"></param>
    /// <param name="name"></param>
    public void EndBookmark( BookmarkEnd bookmarkEnd, string name )
    {
      Bookmark bookmark = this[ name ];

      if( bookmark != null )
      {
        // If we have old bookmark - remove it. 
        BookmarkEnd oldBookmarkEnd = bookmark.BookmarkEnd;

        if( oldBookmarkEnd != null )
        {
          bookmark.BookmarkEnd.OwnerParagraph.RemoveItem( oldBookmarkEnd );
        }

        // Assign new end bookmark.
        bookmark.BookmarkEnd = bookmarkEnd;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    new public void RemoveAt( int index )
    {
      Bookmark bookmark = List[ index ] as Bookmark;
      BookmarkStart start = bookmark.BookmarkStart;
      start.OwnerParagraph.RemoveItem( start );
      
      BookmarkEnd end = bookmark.BookmarkEnd;
      if( end != null )
        end.OwnerParagraph.RemoveItem( end );
      
      List.RemoveAt( index );
    }
    /// <summary>
    /// 
    /// </summary>
    new public void Clear()
    {
      while ( List.Count > 0 )
      {
        Bookmark bookmark = List[ List.Count - 1 ] as Bookmark;
        RemoveAt( List.IndexOf( bookmark ) );
      }
    }
    #endregion
  }
}
