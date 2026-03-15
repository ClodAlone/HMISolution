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
#endregion

using System;
namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.DocIO.DLS.Bookmark"/> objects that 
    /// represent the bookmarks in the document.
    /// </summary>
    public class BookmarkCollection : CollectionImpl
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Bookmark"/> with the specified name.
        /// </summary>
        /// <value></value>
        public Bookmark this[string name]
        {
            get
            {
                return FindByName(name);
            }
        }
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Bookmark"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Bookmark this[int index]
        {
            get
            {
                return InnerList[index] as Bookmark;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkCollection"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        internal BookmarkCollection(WordDocument doc)
            : base(doc, doc)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Finds <see cref="Syncfusion.DocIO.DLS.Bookmark"/> object by specified name
        /// </summary>
        /// <param name="name">The bookmark name</param>
        /// <returns></returns>
        public Bookmark FindByName(string name)
        {
            String name1 = name.Replace('-', '_');
            for (int i = 0; i < InnerList.Count; i++)
            {
                Bookmark bookmark = InnerList[i] as Bookmark;

#if SyncfusionFramework2_0

                if (bookmark.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return bookmark;
                }
#else
        if( bookmark.Name.ToUpper() == name1.ToUpper() )
        {
          return bookmark;
        }
#endif
            }

            return null;
        }
        /// <summary>
        /// Removes a bookmark at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            Bookmark bookmark = InnerList[index] as Bookmark;
            Remove(bookmark);
        }
        /// <summary>
        /// Removes the specified bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        public void Remove(Bookmark bookmark)
        {
            InnerList.Remove(bookmark);

            BookmarkStart start = bookmark.BookmarkStart;
            BookmarkEnd end = bookmark.BookmarkEnd;

            if (start != null)
            {
                start.RemoveSelf();
            }

            if (end != null)
            {
                end.RemoveSelf();
            }
        }
        /// <summary>
        /// Removes all bookmarks from the document. 
        /// </summary>
        public void Clear()
        {
            while (InnerList.Count > 0)
            {
                int lastIndex = InnerList.Count - 1;
                RemoveAt(lastIndex);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds Bookmark object to the collection.
        /// </summary>
        /// <param name="bookmark"></param>
        internal void Add(Bookmark bookmark)
        {
            InnerList.Add(bookmark);
        }
        /// <summary>
        /// Attaches the bookmark start.
        /// </summary>
        /// <param name="bookmarkStart">The bookmark start.</param>
        internal void AttachBookmarkStart(BookmarkStart bookmarkStart)
        {
            Bookmark bookmark = this[bookmarkStart.Name];

            if (bookmark != null)
            {
                //bookmark.SetStart( bookmarkStart );
                bookmarkStart.SetName(bookmarkStart.Name + Guid.NewGuid().ToString());
                bookmarkStart.RemoveSelf();
            }
            else
            {
                bookmark = new Bookmark(bookmarkStart);
                Add(bookmark);
            }
        }
        /// <summary>
        /// Sets BookmarkEnd for the bookmark specified by Name
        /// </summary>
        /// <param name="bookmarkEnd"></param>
        /// 
        internal void AttachBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            Bookmark bookmark = this[bookmarkEnd.Name];

            //if( bookmark == null )
            //  throw new InvalidOperationException( "You can not add bookmark end without corresponding bookmark start" );

            if (bookmark != null)
            {
                // If we have old bookmark - remove self. 
                BookmarkEnd oldBookmarkEnd = bookmark.BookmarkEnd;

                if (oldBookmarkEnd != null)
                {
                    bookmarkEnd.RemoveSelf();
                }
                else
                {
                    bookmark.SetEnd(bookmarkEnd);
                }
            }
        }
        #endregion
    }
}
