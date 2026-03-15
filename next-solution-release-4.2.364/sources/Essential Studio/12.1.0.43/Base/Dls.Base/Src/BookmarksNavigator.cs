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
using System.Text;
using Syncfusion.DLS.Collections;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Helper class for navigations in document bookmarks and editing bookmarks content.
  /// </summary>
  public class BookmarksNavigator
  {
    #region Class constants
    private const string c_DocumentPropertyNotInitialized = "You can not use DocumentNavigator without initializing Document property";
    private const string c_NotEqualDocumentProperty = " Document property must be equal this Document property";
    private const string c_CurrBookmarkNull = "Current Bookmark didn't select";
    private const string c_NotSupportGettingContent = "Not supported getting content between bookmarks in different paragraphs";
    private const string c_NotSupportDeletingContent = "Not supported deleting content between bookmarks in different paragraphs";
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private IDocument m_document;
    private int m_currParagraphItemIndex = 0;
    private IParagraph m_currParagraph = null;
    private Bookmark m_currBookmark = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets Document that this object is attached to.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get
      {
        return m_document;
      }
      set
      {
        m_document = value;
      }
    }
    /// <summary>
    /// Gets the current bookmark.
    /// </summary>
    /// <value>The current bookmark.</value>
    public Bookmark CurrentBookmark
    {
      get
      {
        return m_currBookmark;
      }
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// 
    /// </summary>
    private IParagraphItem CurrentParagraphItem
    {
      get
      {
        if( m_currParagraph == null || m_currParagraphItemIndex < 0 || m_currParagraphItemIndex > ( m_currParagraph.ItemsCount - 1 ) )
        {
          return null;
        }

        return m_currParagraph[ m_currParagraphItemIndex ];
      }
    }
    #endregion

    #region Class initialize / finalize
    /// <summary>
    /// Initializes a new instance of the <see cref="T:BookmarksNavigator"/> class.
    /// </summary>
    /// <param name="doc">The doc.</param>
    public BookmarksNavigator( IDocument doc )
    {
      m_document = doc;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Moves the cursor to specified bookmark.
    /// </summary>
    /// <param name="bookmarkName">Name of the bookmark.</param>
    public void MoveToBookmark( string bookmarkName )
    {
      MoveToBookmark( bookmarkName, false, false );
    }
    /// <summary>
    /// Moves the cursor to specified bookmark.
    /// </summary>
    /// <param name="bookmarkName">Name of the bookmark.</param>
    /// <param name="isStart">When true, moves the cursor to the beginning of the bookmark. When false, moves the cursor to the end of the bookmark</param>
    /// <param name="isAfter">When true, moves the cursor to be after the bookmark start or end position. When false, moves the cursor to be before the bookmark start or end position. </param>
    public void MoveToBookmark( string bookmarkName, bool isStart, bool isAfter )
    {
      if( m_document == null )
        throw new InvalidOperationException( c_DocumentPropertyNotInitialized );

      m_currBookmark = m_document.Bookmarks.FindByName( bookmarkName );
      if( m_currBookmark != null )
      {
        IParagraphItem bkmkItem = ( isStart )
          ? ( IParagraphItem )m_currBookmark.BookmarkStart
          : ( IParagraphItem )m_currBookmark.BookmarkEnd;

        m_currParagraph = bkmkItem.OwnerParagraph;

        if( isAfter )
        {
          m_currParagraphItemIndex = m_currParagraph.IndexOfItem( bkmkItem ) + 1;
        }
        else
        {
          m_currParagraphItemIndex = m_currParagraph.IndexOfItem( bkmkItem );
        }
      }
      else
      {
        m_currParagraphItemIndex = -1;
        m_currParagraph = null;
      }
    }
    /// <summary>
    /// Inserts the text range to current position.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public ITextRange InsertText( string text )
    {
      CheckCurrentState();
      ITextRange txtRange = InsertParagraphItem( ParagraphItemType.TextRange ) as ITextRange;
      txtRange.Text = text;

      return txtRange;
    }
    /// <summary>
    /// Inserts the paragraph item to current position.
    /// </summary>
    /// <param name="itemType">Type of the item.</param>
    /// <returns></returns>
    public IParagraphItem InsertParagraphItem( ParagraphItemType itemType )
    {
      IParagraphItem item = m_document.CreateParagraphItem( itemType );
      m_currParagraph.InsertItem( m_currParagraphItemIndex, item );
      return item;
    }
    /// <summary>
    /// Gets the content inside of the bookmark range.
    /// <remarks>Works only iside one paragraph.</remarks>
    /// </summary>
    /// <returns>The collection of text ranges</returns>
    public TextRangesHolder GetBookmarkContent()
    {
      CheckCurrentState();

      BookmarkStart bkmkStart = m_currBookmark.BookmarkStart;
      BookmarkEnd bkmkEnd = m_currBookmark.BookmarkEnd;
      TextRangesHolder textRanges = new TextRangesHolder();
      IParagraph ownerParagraph = bkmkStart.OwnerParagraph;

      if( ownerParagraph != bkmkEnd.OwnerParagraph )
        throw new NotSupportedException( c_NotSupportGettingContent );

      int indexBkmkStart = bkmkStart.OwnerParagraph.IndexOfItem( bkmkStart );
      int i = indexBkmkStart + 1;

      while( !( ownerParagraph[ i ] is BookmarkEnd ) )
      {
        TextRange textRange = ownerParagraph[ i ] as TextRange;
        if( textRange != null )
        {
          if( textRanges.Count == 0 )
          {
            textRanges.StartCut = 0;//textRange.TextLength;
          }
          textRanges.Add( textRange );
        }
        i++;
      }

      return textRanges;
    }
    /// <summary>
    /// Deletes the content of the bookmark.
    /// <remarks>Works only iside one paragraph.</remarks>
    /// </summary>
    public void DeleteBookmarkContent()
    {
      if( CurrentBookmark == null )
        throw new InvalidOperationException();

      BookmarkStart bkmkStart = CurrentBookmark.BookmarkStart;
      BookmarkEnd bkmkEnd = CurrentBookmark.BookmarkEnd;
      if( bkmkEnd != null )
      {
        IParagraph startOwnerParagraph = bkmkStart.OwnerParagraph;
        IParagraph endOwnerParagraph = bkmkEnd.OwnerParagraph;
        int indexNextPI = 0;

        if( startOwnerParagraph == endOwnerParagraph )
        {
          indexNextPI = bkmkStart.OwnerParagraph.IndexOfItem( bkmkStart ) + 1;
          m_currParagraphItemIndex = indexNextPI;

          while( !( startOwnerParagraph[ indexNextPI ] is BookmarkEnd ) )
          {
            startOwnerParagraph.RemoveItemAt( indexNextPI );
          }
        }
        else
        {
          int startParIndex = ( startOwnerParagraph.Owner as Section ).Paragraphs.IndexOf( startOwnerParagraph );
          int endParIndex = ( endOwnerParagraph.Owner as Section ).Paragraphs.IndexOf( endOwnerParagraph );
          if( endParIndex - startParIndex <= 0 )
          {
            throw new NotSupportedException( c_NotSupportDeletingContent );
          }
          else
          {
            indexNextPI = startOwnerParagraph.IndexOfItem( bkmkStart ) + 1;
            for( int i = indexNextPI, cnt = startOwnerParagraph.ItemsCount; i < cnt; i++ )
            {
              startOwnerParagraph.RemoveItemAt( indexNextPI );
            }
            int removeIndex = startParIndex + 1;
            for( int j = removeIndex; j < endParIndex; j++ )
            {
              ( startOwnerParagraph.Owner as Section ).Paragraphs.RemoveAt( removeIndex );
            }
            int indexLastPI = endOwnerParagraph.IndexOfItem( bkmkEnd );          
            for( int i = 0; i < indexLastPI; i++ )
            {
              endOwnerParagraph.RemoveItemAt( i );
              --indexLastPI;
            }
          }
        }
      }
    }
    /// <summary>
    /// Replaces the content inside of the bookmark.
    /// </summary>
    /// <param name="textRanges">The collection of text ranges.</param>
    /// <remarks>Works only iside one paragraph.</remarks>
    public void ReplaceBookmarkContent( TextRangesHolder textRanges )
    {
      DeleteBookmarkContent();
      int indexBkmkStart = m_currParagraph.IndexOfItem( m_currBookmark.BookmarkStart ) + 1;
      textRanges.CopyTo( ( Paragraph )m_currParagraph, indexBkmkStart );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Checks the current state of navigator.
    /// </summary>
    private void CheckCurrentState()
    {
      if( m_document == null )
        throw new InvalidOperationException( c_DocumentPropertyNotInitialized );

      if( m_currBookmark == null || m_currParagraph == null || m_currParagraphItemIndex < 0 )
        throw new InvalidOperationException( c_CurrBookmarkNull );
    }
    #endregion
  }
}
