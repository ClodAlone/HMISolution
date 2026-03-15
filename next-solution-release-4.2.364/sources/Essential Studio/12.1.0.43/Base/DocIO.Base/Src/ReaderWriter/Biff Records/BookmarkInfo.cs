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

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for Bookmark.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BookmarkInfo
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private string m_strName;
        /// <summary>
        /// 
        /// </summary>
        private int m_iStartPos;
        /// <summary>
        /// 
        /// </summary>
        private int m_iEndPos;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isCellGroup;
        /// <summary>
        /// 
        /// </summary>
        private int m_startCell = -1;
        /// <summary>
        /// 
        /// </summary>
        private int m_endCell = -1;
        /// <summary>
        /// 
        /// </summary>
        private int m_bookmarkIndex;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="startPos">The start pos.</param>
        /// <param name="endPos">The end pos.</param>
        /// <param name="isCellGroup">if set to <c>true</c> [is cell group].</param>
        /// <param name="startCellIndex">Start index of the cell.</param>
        /// <param name="endCellIndex">End index of the cell.</param>
        internal BookmarkInfo(string name, int startPos, int endPos,
          bool isCellGroup, int startCellIndex, int endCellIndex)
        {
            m_strName = name;
            m_iStartPos = startPos;
            m_iEndPos = endPos;
            if (isCellGroup)
            {
                m_isCellGroup = isCellGroup;
                m_startCell = startCellIndex;
                m_endCell = endCellIndex - 1;
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int EndPos
        {
            get
            {
                return m_iEndPos;
            }
            set
            {
                m_iEndPos = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int StartPos
        {
            get
            {
                return m_iStartPos;
            }
            set
            {
                m_iStartPos = value;
            }
        }
        /// <summary>
        /// Determines whether specified bookmark covers a group of table cells .
        /// </summary>
        internal bool IsCellGroupBookmark
        {
            get
            {
                return m_isCellGroup;
            }
        }
        /// <summary>
        /// Gets the start index of the cell which is covered by the bookmark.
        /// </summary>
        internal int StartCellIndex
        {
            get
            {
                return m_startCell;
            }
            set
            {
                m_startCell = value;
            }
        }
        /// <summary>
        /// Gets the end index of the cell which is covered by the bookmark.
        /// </summary>
        internal int EndCellIndex
        {
            get
            {
                return m_endCell;
            }
            set
            {
                m_endCell = value;
            }
        }
        /// <summary>
        /// Bookmark index in the collection of bookmarks.
        /// </summary>
        internal int Index
        {
            get
            {
                return m_bookmarkIndex;
            }
            set
            {
                m_bookmarkIndex = value;
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Clones current BookmarkInfo object.
        /// </summary>
        /// <returns></returns>
        internal BookmarkInfo Clone()
        {
            BookmarkInfo bkmkInfo = (BookmarkInfo)this.MemberwiseClone();
            //      BookmarkInfo bkmkInfo = new BookmarkInfo( string.Empty, 0, 0, false, 0, 0 );
            //      bkmkInfo.Name = this.Name;
            //      bkmkInfo.StartPos = this.StartPos;
            //      bkmkInfo.EndPos = this.EndPos;

            return bkmkInfo;
        }
        #endregion
    }
}