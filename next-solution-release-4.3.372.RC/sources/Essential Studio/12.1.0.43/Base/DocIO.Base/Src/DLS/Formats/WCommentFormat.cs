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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a comment properties.
    /// </summary>
    public class WCommentFormat
      : XDLSSerializableBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private string m_strUser = "";
        /// <summary>
        /// 
        /// </summary>
        private string m_strUserInitials = "";
        /// <summary>
        /// 
        /// </summary>
        private int m_iBookmarkStartOffset = -1;
        /// <summary>
        /// 
        /// </summary>
        private int m_iBookmarkEndOffset = -1;
        /// <summary>
        /// 
        /// </summary>
        private int m_iTagBkmk = -1;
        /// <summary>
        /// Text position of the comment.
        /// </summary>
        private int m_iPosition;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the user initials.
        /// </summary>
        /// <value>The user initials.</value>
        public string UserInitials
        {
            get
            {
                return m_strUserInitials;
            }
            set
            {
                if (value.Length > 9)
                {
                    throw new ArgumentOutOfRangeException("UserInitials", "Users initials length must be less than 10 symbols.");
                }
                m_strUserInitials = value;
            }
        }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public string User
        {
            get
            {
                return m_strUser;
            }
            set
            {
                m_strUser = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int BookmarkStartOffset
        {
            get
            {
                return m_iBookmarkStartOffset;
            }
            set
            {
                m_iBookmarkStartOffset = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int BookmarkEndOffset
        {
            get
            {
                return m_iBookmarkEndOffset;
            }
            set
            {
                m_iBookmarkEndOffset = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int TagBkmk
        {
            get
            {
                return m_iTagBkmk;
            }
            set
            {
                m_iTagBkmk = value;
            }
        }
        /// <summary>
        /// Gets/sets the text position of the comment.
        /// </summary>
        /// <value>The position.</value>
        internal int Position
        {
            get
            {
                return m_iPosition;
            }
            set
            {
                m_iPosition = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int StartTextPos
        {
            get
            {
                return m_iPosition - m_iBookmarkStartOffset;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WCommentFormat"/> class.
        /// </summary>
        public WCommentFormat()
            : base(null, null)
        { }
        #endregion

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// Write comment's XML attributes
        /// </summary>
        /// <param name="writer"> XMLWriter</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (m_strUserInitials != "")
                writer.WriteValue(XDLSConstants.CommentFormatUserInitialsAttr, m_strUserInitials);

            if (m_strUser != "")
                writer.WriteValue(XDLSConstants.CommentFormatUserAttr, m_strUser);

            if (m_iBookmarkStartOffset != -1)
                writer.WriteValue(XDLSConstants.CommentBookmarkStartAttr, m_iBookmarkStartOffset);

            if (m_iBookmarkEndOffset != -1)
                writer.WriteValue(XDLSConstants.CommentBookmarkEndAttr, m_iBookmarkEndOffset);

            if (m_iTagBkmk != -1)
                writer.WriteValue(XDLSConstants.CommentTagBkmkAttr, m_iTagBkmk);
        }
        /// <summary>
        /// Read comment's XML attributes
        /// </summary>
        /// <param name="reader"> XMLReader</param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.CommentFormatUserAttr))
                m_strUser = reader.ReadString(XDLSConstants.CommentFormatUserAttr);

            if (reader.HasAttribute(XDLSConstants.CommentFormatUserInitialsAttr))
                m_strUserInitials = reader.ReadString(XDLSConstants.CommentFormatUserInitialsAttr);

            if (reader.HasAttribute(XDLSConstants.CommentBookmarkStartAttr))
                m_iBookmarkStartOffset = reader.ReadInt(XDLSConstants.CommentBookmarkStartAttr);

            if (reader.HasAttribute(XDLSConstants.CommentBookmarkEndAttr))
                m_iBookmarkEndOffset = reader.ReadInt(XDLSConstants.CommentBookmarkEndAttr);

            if (reader.HasAttribute(XDLSConstants.CommentTagBkmkAttr))
                m_iTagBkmk = reader.ReadInt(XDLSConstants.CommentTagBkmkAttr);
        }
//#endif
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public WCommentFormat Clone(IWordDocument doc)
        {
            WCommentFormat format = new WCommentFormat();
            format.m_strUserInitials = m_strUserInitials;
            format.m_strUser = m_strUser;
            format.m_iBookmarkEndOffset = m_iBookmarkEndOffset;
            format.m_iBookmarkStartOffset = m_iBookmarkStartOffset;

            if (doc == Document || FindTagBkmk(doc, m_iTagBkmk))
            {
                format.m_iTagBkmk = TagIdRandomizer.GetId(m_iTagBkmk);
                //format.m_iTagBkmk = ( new Random( m_iTagBkmk ) ).Next();
            }
            else
            {
                format.m_iTagBkmk = m_iTagBkmk;
            }

            return format;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tagBkmk"></param>
        /// <returns></returns>
        private bool FindTagBkmk(IWordDocument doc, int tagBkmk)
        {
            foreach (WSection sec in doc.Sections)
            {
                foreach (WParagraph paragraph in sec.Body.Paragraphs)
                {
                    foreach (IParagraphItem item in paragraph.Items)
                    {
                        WComment comment = item as WComment;

                        if (comment != null && comment.Format.TagBkmk == tagBkmk)
                        {
                            return true;
                        }
                    }
                }
            }

            if (TagIdRandomizer.ChangedIds.ContainsKey(tagBkmk))
                return true;

            return false;
        }

        #endregion

        #region Helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal void UpdateTagBkmk()
        {
            m_iTagBkmk = TagIdRandomizer.Instance.Next();
        }
        #endregion
    }
    /// <summary>
    /// Class which provides WCommentFormat class with random tag bookmark id.
    /// </summary>
    internal class TagIdRandomizer
    {
        #region Fields
        [ThreadStatic]
        private static Random m_instance;
        /// <summary>
        /// Contains changed IDs and values they where change on.
        /// </summary>
        [ThreadStatic]
        private static List<int> m_ids;
        /// <summary>
        /// Contains the comment ids, which can't be changed
        /// </summary>
        [ThreadStatic]
        private static List<int> m_noneChangeIds;
        /// <summary>
        /// Contains the old id(s) and the new id(s)
        /// </summary>
        [ThreadStatic]
        private static Dictionary<int, int> m_changedIds;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance of Random class.
        /// </summary>
        /// <value>The instance.</value>
        internal static Random Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new Random(1000);
                }
                return m_instance;
            }
        }
        /// <summary>
        /// Gets the collection of ids and new ids they were changed on.
        /// </summary>
        internal static Dictionary<int, int> ChangedIds
        {
            get
            {
                if (m_changedIds == null)
                {
                    m_changedIds = new Dictionary<int, int>();
                }
                return m_changedIds;
            }
        }
        /// <summary>
        /// Gets the collection of ids and new ids they were changed on.
        /// </summary>
        internal static List<int> Identificators
        {
            get
            {
                if (m_ids == null)
                {
                    m_ids = new List<int>();
                }
                return m_ids;
            }
        }
        /// <summary>
        /// Gets the collection of ids and new ids they were changed on.
        /// </summary>
        internal static List<int> NoneChangeIds
        {
            get
            {
                if (m_noneChangeIds == null)
                {
                    m_noneChangeIds = new List<int>();
                }
                return m_noneChangeIds;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the id for comment marker.
        /// </summary>
        /// <param name="currentId">The current id.</param>
        /// <param name="newId">if set to <c>true</c> create new id.</param>
        /// <returns></returns>
        internal static int GetId(int currentId)
        {
            if (NoneChangeIds.Contains(currentId))
                return currentId;

            int nextId = -1;
            if (!ChangedIds.ContainsKey(currentId))
            {
                nextId = Instance.Next();
                ChangedIds.Add(currentId, nextId);
                Identificators.Add(nextId);
            }
            else
            {
                nextId = (int)ChangedIds[currentId];
                if (IsValidId(nextId))
                {
                    Identificators.Add(nextId);
                }
                else
                {
                    nextId = Instance.Next();
                    Identificators.Add(nextId);
                }
            }

            return nextId;
        }
        /// <summary>
        /// Determines whether the new id is valid.
        /// </summary>
        /// <param name="newId">The new id.</param>
        /// <returns>
        /// 	if it specifies a valid id, set to <c>true</c>.
        /// </returns>
        private static bool IsValidId(int newId)
        {
            bool isValid = true;
            if (m_ids != null && m_ids.Count > 0)
            {
                foreach (int id in m_ids)
                {
                    if (id == newId)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }
        /// <summary>
        /// Gets the marker id.
        /// </summary>
        /// <param name="currentId">The current id.</param>
        /// <param name="newId">if it creates new id, set to <c>true</c>.</param>
        /// <returns></returns>
        internal static int GetMarkerId(int currentId, bool newId)
        {
            if (NoneChangeIds.Contains(currentId))
                return currentId;

            if (!ChangedIds.ContainsKey(currentId) || newId)
            {
                int nextId = Instance.Next();
                if (!ChangedIds.ContainsKey(currentId))
                    ChangedIds.Add(currentId, nextId);
                else
                    ChangedIds[currentId] = nextId;

                return nextId;
            }
            else
            {
                return (int)ChangedIds[currentId];
            }
        }
        #endregion
    }
}
