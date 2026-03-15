#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents base class for paragraphs and tables.
    /// </summary>
    public abstract class TextBodyItem
      : WidgetBase,
      ITextBodyItem
    {
        #region Properties
        /// <summary>
        /// Gets the owner text body.
        /// </summary>
        /// <value>The owner text body.</value>
        public WTextBody OwnerTextBody
        {
            get
            {
                return Owner as WTextBody;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <value>
        /// 	If this instance was inserted, set to <c>true</c>.
        /// </value>
        public bool IsInsertRevision
        {
            get
            {
                return CheckInsertRev();
            }
        }
        /// <summary>
        /// Gets a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <value>
        /// 	If this instance is delete revision, set to <c>true</c>.
        /// </value>
        public bool IsDeleteRevision
        {
            get
            {
                return CheckDeleteRev();
            }
        }
        /// <summary>
        /// Defines whether format was changed.
        /// </summary>
        /// <value>The is changed format.</value>
        internal bool IsChangedCFormat
        {
            get
            {
                return CheckChangedCFormat();
            }
        }
        /// <summary>
        /// Gets a value indicating whether paragraph/table format is changed.
        /// </summary>
        /// <value>
        /// 	If paragraph/table format is changed, set to <c>true</c>.
        /// </value>
        internal bool IsChangedPFormat
        {
            get
            {
                return CheckChangedPFormat();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextBodyItem NextTextBodyItem
        {
            get
            {
                return GetNextTextBodyItem();
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodyItem"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public TextBodyItem(WordDocument doc)
            : base(doc, null)
        {
        }
        #endregion

        #region Implementation / abstract
        /// <summary>
        /// Finds text by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public abstract TextSelection Find(Regex pattern);
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern">Pattern</param>
        /// <param name="replace">Replace text</param>
        public abstract int Replace(Regex pattern, string replace);
        /// <summary>
        /// Replaces by specified given string.
        /// </summary>
        /// <param name="given">The given text.</param>
        /// <param name="replace">The replace text.</param>
        /// <param name="caseSensitive">if set to <c>true</c> case sensitive replace.</param>
        /// <param name="wholeWord">if it replaces only whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public abstract int Replace(string given, string replace, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        public abstract int Replace(Regex pattern, TextSelection textSelection);
        /// <summary>
        /// Replaces by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public abstract int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting);
        /// <summary>
        /// Returns all entries of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal abstract TextSelectionList FindAll(Regex pattern);
        /// <summary>
        /// Gets the next TextBodyItem in the document.
        /// </summary>
        /// <returns></returns>
        internal abstract TextBodyItem GetNextTextBodyItem();
        /// <summary>
        /// Closes the item.
        /// </summary>
        internal abstract void Close();
        #endregion

        #region Implementation / track changes
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        /// <param name="acceptChanges">if it accepts changes, set to <c>true</c>.</param>
        internal abstract void MakeChanges(bool acceptChanges);
        /// <summary>
        /// Checks a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        internal abstract bool CheckInsertRev();
        /// <summary>
        /// Checks a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>   
        internal abstract bool CheckDeleteRev();
        /// <summary>
        /// Defines whether format was changed.
        /// </summary>
        /// <returns></returns>
        internal abstract bool CheckChangedCFormat();
        /// <summary>
        /// Defines whether paragraph/table format is changed. 
        /// </summary>
        /// <returns></returns>
        internal abstract bool CheckChangedPFormat();
        /// <summary>
        /// Removes the changes from character format.
        /// </summary>
        internal abstract void AcceptCChanges();
        /// <summary>
        /// Accept changes from paragraph/table.
        /// </summary>
        internal abstract void AcceptPChanges();
        /// <summary>
        /// Removes the character format changes.
        /// </summary>
        internal abstract void RemoveCFormatChanges();
        /// <summary>
        /// Removes the paragraph format changes.
        /// </summary>
        internal abstract void RemovePFormatChanges();
        /// <summary>
        /// Determines whether item has tracked changes.
        /// </summary>
        /// <returns>
        /// 	If it has tracked changes, set to <c>true</c>.
        /// </returns>
        internal abstract bool HasTrackedChanges();
        /// <summary>
        /// Sets the changed Character format.
        /// </summary>
        /// <param name="check">if it is check, set to <c>true</c>.</param>
        internal abstract void SetChangedCFormat(bool check);
        /// <summary>
        /// Sets the changed Paragraph format.
        /// </summary>
        /// <param name="check">if it sets the Format, set to <c>true</c>.</param>
        internal abstract void SetChangedPFormat(bool check);
        /// <summary>
        /// Sets the delete revision.
        /// </summary>
        /// <param name="check">if it sets delete revision, set to <c>true</c>.</param>
        internal abstract void SetDeleteRev(bool check);
        /// <summary>
        /// Sets the insert revision.
        /// </summary>
        /// <param name="check">if it sets insert revision, set to <c>true</c>.</param>
        internal abstract void SetInsertRev(bool check);
        #endregion

        #region Implementation / NextTextBodyItem
        /// <summary>
        /// Gets the next
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        protected TextBodyItem GetNextInSection(WSection section)
        {
            if (section == null)
                return null;

            WSection nextSection = section.NextSibling as WSection;
            if (nextSection != null && nextSection.Body.Items.Count > 0)
            {
                return nextSection.Body.Items[0];
            }

            return null;
        }
        #endregion
    }
}
