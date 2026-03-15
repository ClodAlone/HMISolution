#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    #region delegates
    /// <summary>
    /// Editor text changed event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="evtArgs">Event args</param>
    public delegate void EditorTextChangedEventHandler(object sender, EditorTextChangedEventArgs evtArgs);
    #endregion

    /// <summary>
    /// Editor text changed event args
    /// </summary>
    public class EditorTextChangedEventArgs
        : EventArgs
    {
        #region fields
        protected int m_nSelectionStart;
        protected string m_strDeletedText = string.Empty;
        protected string m_strInsertedText = string.Empty;
        protected TextFormatting m_fmtType;
        protected bool m_bMerge;
        #endregion

        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorTextChangedEventArgs"/> class.
        /// </summary>
        /// <param name="fmtType">Type of the text formatting.</param>
        /// <param name="nSelectionPosition">The selection position.</param>
        /// <param name="strDeletedText">The deleted text.</param>
        /// <param name="strInsertedText">The inserted text.</param>
        /// <param name="bMerge">if set to <c>true</c> to merger text changing command merge.</param>
        public EditorTextChangedEventArgs(TextFormatting fmtType, int nSelectionPosition, string strDeletedText, string strInsertedText, bool bMerge)
        {
            if (nSelectionPosition < 0)
                throw new ArgumentOutOfRangeException("nSelectionPosition", "nSelectionPosition can not be negative.");

            m_fmtType = fmtType;
            m_nSelectionStart = nSelectionPosition;
            m_strDeletedText = strDeletedText;
            m_strInsertedText = strInsertedText;
            m_bMerge = bMerge;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the text formatting.
        /// </summary>
        /// <value>The text formatting.</value>
        public TextFormatting TextFormatting
        {
            get { return m_fmtType; }
        }

        /// <summary>
        /// Gets the selection start.
        /// </summary>
        /// <value>The selection start.</value>
        public int SelectionStart
        {
            get { return m_nSelectionStart; }
        }

        /// <summary>
        /// Gets the deleted text.
        /// </summary>
        /// <value>The deleted text.</value>
        public string DeletedText
        {
            get { return m_strDeletedText; }
        }

        /// <summary>
        /// Gets the inserted text.
        /// </summary>
        /// <value>The inserted text.</value>
        public string InsertedText
        {
            get { return m_strInsertedText; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EditorTextChangedEventArgs"/> is merge.
        /// </summary>
        /// <value><c>true</c> if merge; otherwise, <c>false</c>.</value>
        public bool Merge
        {
            get { return m_bMerge; }
        }
        #endregion
    }
}