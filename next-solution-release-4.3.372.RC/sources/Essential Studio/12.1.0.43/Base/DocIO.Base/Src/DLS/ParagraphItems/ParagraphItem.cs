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
using Syncfusion.DocIO.DLS;

#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Layouting;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a paragraph item.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class ParagraphItem
      : WidgetBase,
        IParagraphItem
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private int m_startIndex = 0;
        internal bool Cloned = false;
        protected WCharacterFormat m_charFormat;
        private bool m_skipDocxItem;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        internal bool SkipDocxItem
        {
            get
            {
                return m_skipDocxItem;
            }
            set
            {
                m_skipDocxItem = value;
            }
        }
        /// <summary>
        /// Gets owner paragraph.
        /// </summary>
        /// <value></value>
        public WParagraph OwnerParagraph
        {
            get
            {
                return Owner as WParagraph;
            }
        }
        /// <summary>
        /// Get Owner Paragraph of the Item is in SDTInlineContent
        /// </summary>
        /// <returns></returns>
        internal WParagraph GetOwnerParagraph()
        {
            Entity ent = Owner;
            while (!(ent is WParagraph) && ent != null)
            {
                ent = ent.Owner;
            }
            return ent as WParagraph;
        }
        /// <summary>
        /// Gets a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <value>
        /// 	if this instance was inserted, set to <c>true</c>.
        /// </value>
        public bool IsInsertRevision
        {
            get
            {
                if (m_charFormat != null)
                {
                    return m_charFormat.IsInsertRevision;
                }
                return false;
            }
        }
        /// <summary>
        /// Sets the insert revision.
        /// </summary>
        /// <param name="value">if it specifies insert revision, set to <c>true</c>.</param>
        internal void SetInsertRev(bool value)
        {
            ParaItemCharFormat.IsInsertRevision = value;
        }
        /// <summary>
        /// Gets or set a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <value>
        /// 	if this instance is delete revision, set to <c>true</c>.
        /// </value>
        public bool IsDeleteRevision
        {
            get
            {
                if (m_charFormat != null)
                {
                    return m_charFormat.IsDeleteRevision;
                }
                return false;
            }
        }
        /// <summary>
        /// Sets the delete revision.
        /// </summary>
        /// <param name="value">if it specifies delete revision, set to <c>true</c>.</param>
        internal void SetDeleteRev(bool value)
        {
            ParaItemCharFormat.IsDeleteRevision = value;
        }
        /// <summary>
        /// Gets or set a value indicating whether this instance has changed format.
        /// </summary>
        /// <value>
        /// 	if this instance has changed format, set to <c>true</c>.
        /// </value>
        internal bool IsChangedCFormat
        {
            get
            {
                if (m_charFormat != null)
                {
                    return m_charFormat.IsChangedFormat;
                }
                return false;
            }
            set
            {
                if (m_charFormat != null)
                {
                    m_charFormat.IsChangedFormat = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the start pos of paragraph item.
        /// </summary>
        /// <value>The start pos.</value>
        internal int StartPos
        {
            get
            {
                return m_startIndex;
            }
            set
            {
                //if( m_detached )
                //  throw new InvalidOperationException();

                m_startIndex = value;
            }
        }
        /// <summary>
        /// Gets the end pos of paragraph item.
        /// </summary>
        /// <value>The end pos.</value>
        internal virtual int EndPos
        {
            get
            {
                return StartPos;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="ParagraphItem"/> is detached.
        /// </summary>
        /// <value>if detached, set to <c>true</c>.</value>
        internal bool ItemDetached
        {
            get
            {
                return (OwnerBase == null);
            }
        }
        /// <summary>
        /// Sets the paragraph item character format.
        /// </summary>
        /// <value>The paragraph item char format.</value>
        internal WCharacterFormat ParaItemCharFormat
        {
            get
            {
                if (m_charFormat == null)
                {
                    if (Owner is SDTInlineContent)
                        m_charFormat = GetOwnerParagraph().BreakCharacterFormat;
                    else
                        m_charFormat = OwnerParagraph.BreakCharacterFormat;
                }
                return m_charFormat;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphItem"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        protected ParagraphItem(WordDocument doc)
            : base(doc, null)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        internal virtual void Attach(WParagraph owner, int itemPos)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (owner != OwnerParagraph)
                throw new InvalidOperationException();

            if (ItemDetached)
                throw new InvalidOperationException();
            if (this.OwnerParagraph.BreakCharacterFormat.BaseFormat != null)
                this.ParaItemCharFormat.ApplyBase(this.OwnerParagraph.BreakCharacterFormat.BaseFormat);
            StartPos = itemPos;
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal virtual void Detach()
        {
            if (ItemDetached && !Document.IsClosing)
                throw new InvalidOperationException();
        }
        /// <summary>
        /// Accepts the changes.
        /// </summary>
        internal void AcceptChanges()
        {
            if (m_charFormat != null)
            {
                m_charFormat.AcceptChanges();
            }
        }
        /// <summary>
        /// Removes the format changes.
        /// </summary>
        internal void RemoveChanges()
        {
            if (m_charFormat != null)
            {
                m_charFormat.RemoveChanges();
            }
        }
        /// <summary>
        /// Determines whether has tracked changes.
        /// </summary>
        /// <returns>
        /// 	if has tracked changes, set to <c>true</c>.
        /// </returns>
        internal bool HasTrackedChanges()
        {
            if (IsInsertRevision || IsDeleteRevision || IsChangedCFormat)
                return true;

            return false;
        }
        /// <summary>
        /// Gets the char format.
        /// </summary>
        /// <returns></returns>
        internal WCharacterFormat GetCharFormat()
        {
            return m_charFormat;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal virtual void Close()
        {
            if (m_charFormat != null)
            {
                m_charFormat.Close();
                m_charFormat = null;
            }
        }
        #endregion

        #region Implementation / overrides
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            ParagraphItem item = (ParagraphItem)base.CloneImpl();
            if (m_charFormat != null)
            {
                item.m_charFormat = new WCharacterFormat(Document);
                item.m_charFormat.ImportContainer(m_charFormat);
                item.m_charFormat.CopyProperties(m_charFormat);
                item.m_charFormat.SetOwner(item);
            }

            return item;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);
            if (doc.ImportOption != ImportOptions.UseDestinationStyles)
                UpdateFormatting(doc);
            if (m_charFormat == null || string.IsNullOrEmpty(m_charFormat.CharStyleName))
                return;
            if (doc.ImportOption == ImportOptions.UseDestinationStyles
                && doc.ImportStyles)
            {
                CharacterStyle cs = Document.Styles.FindByName(m_charFormat.CharStyleName, StyleType.CharacterStyle) as CharacterStyle;

                if (cs != null)
                {
                    IStyle foundStyle = doc.Styles.FindByName(cs.Name, StyleType.CharacterStyle);

                    if (foundStyle == null)
                    {
                        cs.ImportStyleTo(doc);
                    }
                    else
                    {
                        if (doc.CurClonedSection != null)
                        {
                            cs = (CharacterStyle)(cs as Style).ApplyOrImportStyleTo(doc, foundStyle);
                            m_charFormat.CharStyleName = cs.Name;
                        }
                    }
                }
            }
            if (doc != this.Document)
            {
                // Updates the document default formattings if the destination and source document is different.
                this.m_charFormat.UpdateDefaultFormats();
            }
        }
        /// <summary>
        /// Updates the formatting.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void UpdateFormatting(WordDocument doc)
        {
            if (doc.ImportOption == ImportOptions.MergeFormatting)
            {
                //Merges the source and destination formatting.
                WParagraph lastParagraph = doc.LastParagraph;
                if (lastParagraph == null)
                    lastParagraph = new WParagraph(doc);
                m_charFormat.MergeFormat(lastParagraph.BreakCharacterFormat);
            }
            else
            {
                //Updates the source formatting.
                WParagraphStyle style = doc.Styles.FindByName("Normal", StyleType.ParagraphStyle) as WParagraphStyle;
                if (doc.ImportOption == ImportOptions.KeepSourceFormatting)
                    m_charFormat.UpdateSourceFormat(style.CharacterFormat);
            }
        }
        #endregion
    }
}
