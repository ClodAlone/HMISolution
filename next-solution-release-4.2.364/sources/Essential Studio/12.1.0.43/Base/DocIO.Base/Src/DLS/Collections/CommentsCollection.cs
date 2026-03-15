#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.DocIO.DLS.CommentsCollection"/> objects that 
    /// represent the comments in the document.
    /// </summary>
    public class CommentsCollection : CollectionImpl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public CommentsCollection(WordDocument doc)
            : base(doc, doc)
        { }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the comment at specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public WComment this[int index]
        {
            get
            {
                return InnerList[index] as WComment;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        public int Counts()
        {
            return InnerList.Count;
        }
        /// <summary>
        /// Remove a Comment at specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            WComment comment = InnerList[index] as WComment;
            InnerList.Remove(comment);
            if (comment.OwnerParagraph != null)
                comment.OwnerParagraph.Items.Remove(comment);
        }
        /// <summary>
        /// Remove all the Comment from the document.
        /// </summary>
        public void Clear()
        {
            while (InnerList.Count > 0)
            {
                int lastIndex = InnerList.Count - 1;
                RemoveAt(lastIndex);
            }
        }
        /// <summary>
        /// Adds the specified comment.
        /// </summary>
        /// <param name="comment"></param>
        internal void Add(WComment comment)
        {
            InnerList.Add(comment);
        }
        /// <summary>
        /// Removes the specified Comment.
        /// </summary>
        /// <param name="comment"></param>
        public void Remove(WComment comment)
        {
            InnerList.Remove(comment);
            comment.OwnerParagraph.Items.Remove(comment);
        }

        #endregion
    }
}
