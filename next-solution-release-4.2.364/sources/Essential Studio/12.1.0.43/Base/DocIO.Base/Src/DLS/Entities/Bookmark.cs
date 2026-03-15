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

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a bookmark. Holds BookmarkStart and BookmarkEnd in the document.
    /// </summary>
    public class Bookmark
    {
        #region Fields
        private BookmarkStart m_bkmkStart = null;
        private BookmarkEnd m_bkmkEnd = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets bookmark name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_bkmkStart.Name;
            }
        }
        /// <summary>
        /// Gets the bookmark start.
        /// </summary>
        /// <value>The bookmark start.</value>
        public BookmarkStart BookmarkStart
        {
            get
            {
                return m_bkmkStart;
            }
        }
        /// <summary>
        /// Gets the bookmark end.
        /// </summary>
        /// <value>The bookmark end.</value>
        public BookmarkEnd BookmarkEnd
        {
            get
            {
                return m_bkmkEnd;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Bookmark"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        public Bookmark(BookmarkStart start)
            : this(start, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Bookmark"/> class.
        /// </summary>
        /// <param name="start">The bookmark start.</param>
        /// <param name="end">The bookmark end.</param>
        public Bookmark(BookmarkStart start, BookmarkEnd end)
        {
            m_bkmkStart = start;
            m_bkmkEnd = end;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the bookmark start.
        /// </summary>
        /// <param name="start">The bookmark start.</param>
        internal void SetStart(BookmarkStart start)
        {
            m_bkmkStart = start;
        }
        /// <summary>
        /// Sets the bookmark end.
        /// </summary>
        /// <param name="end">The end.</param>
        internal void SetEnd(BookmarkEnd end)
        {
            m_bkmkEnd = end;
        }
        #endregion
    }
}
