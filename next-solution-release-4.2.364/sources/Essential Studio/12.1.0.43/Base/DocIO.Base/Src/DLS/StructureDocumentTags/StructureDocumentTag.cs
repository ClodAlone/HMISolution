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
using System.Text.RegularExpressions;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.Rendering;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class StructureDocumentTag
    {
        #region fields
        private SDTContent m_SDTContent;
        private SDTProperties m_SDTProperties;
        private WCharacterFormat m_BreakCharacterFormat;
        #endregion

        # region Properties
        public SDTProperties SDTProperties
        {
            get
            {
                return m_SDTProperties;
            }
        }

        public SDTContent SDTContent
        {
            get
            {
                return m_SDTContent;
            }
        }
        #endregion
    }
    internal class StructureDocumentTagBlock :
        TextBodyItem,
#if !SILVERLIGHT && !WP
        IWidgetContainer,
        IWidget,
#endif
        IStructureDocumentTagBlock
    {
        # region Fields
        private SDTBlockContent m_SDTContent;
        private SDTProperties m_SDTProperties;
        private WCharacterFormat m_BreakCharacterFormat;
        private EntityCollection m_childEntities;
        # endregion

        # region Properties
        public SDTBlockContent SDTContent
        {
            get
            {
                return m_SDTContent;
            }
        }
        public SDTProperties SDTProperties
        {
            get
            {
                return m_SDTProperties;
            }
        }
        public WCharacterFormat BreakCharacterFormat
        {
            get
            {
                return m_BreakCharacterFormat;
            }
        }
        # endregion

        # region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTable"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal StructureDocumentTagBlock(WordDocument doc)
            : base((WordDocument)doc)
        {
            m_SDTProperties = new SDTProperties(doc);
            m_SDTContent = new SDTBlockContent(doc,this);
            m_BreakCharacterFormat = new WCharacterFormat(doc);
            m_childEntities = m_SDTContent.ChildEntities;
        }
        # endregion

        # region Implementation
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            m_SDTContent.AddSelf();
        }
        internal StructureDocumentTagBlock Clone()
        {
            return (StructureDocumentTagBlock)CloneImpl();
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            StructureDocumentTagBlock sdTagBlock = (StructureDocumentTagBlock)base.CloneImpl();
            return sdTagBlock;
        }
        /// <summary>
        /// Gets Next the text body item in the document.
        /// </summary>
        /// <returns></returns>
        internal override TextBodyItem GetNextTextBodyItem()
        {
            if (this.NextSibling != null)
                return this.NextSibling as TextBodyItem;

            if (this.Owner is WTableCell)
            {
                return (this.Owner as WTableCell).GetNextTextBodyItem();
            }
            else if (this.Owner is WTextBody)
            {
                if (this.OwnerTextBody.Owner is WTextBox)
                    return (this.OwnerTextBody.Owner as WTextBox).GetNextTextBodyItem();
                else if (this.OwnerTextBody.Owner is WSection)
                    return GetNextInSection(this.OwnerTextBody.Owner as WSection);
            }

            return null;
        }
        /// <summary>
        /// Checks a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        /// <value>
        /// 	if this instance is delete revision, set to <c>true</c>.
        /// </value>
        internal override bool CheckDeleteRev()
        {
            if (m_BreakCharacterFormat != null)
            {
                return m_BreakCharacterFormat.IsDeleteRevision;
            }

            return false;

        }
        /// <summary>
        /// Sets the changed Paragraph format for table.
        /// </summary>
        /// <param name="check">if it specifies the format to be changed, set to <c>true</c>.</param>
        internal override void SetChangedPFormat(bool check)
        {
            //TODO: Need to implement
        }
        /// <summary>
        /// Sets the changed C format.
        /// </summary>
        /// <param name="check">if it specifies formatting, set to <c>true</c>.</param>
        internal override void SetChangedCFormat(bool check)
        {
            //TODO: Need to be implemented
        }

        /// <summary>
        /// Sets the delete rev.
        /// </summary>
        /// <param name="check">if specifies delete revision, set to <c>true</c>.</param>
        internal override void SetDeleteRev(bool check)
        {
            if (m_BreakCharacterFormat != null)
            {
                m_BreakCharacterFormat.IsDeleteRevision = check;
            }
        }
        /// <summary>
        /// Sets the insert rev.
        /// </summary>
        /// <param name="check">if it specifies insert revision, set to <c>true</c>.</param>
        internal override void SetInsertRev(bool check)
        {
            if (m_BreakCharacterFormat != null)
            {
                m_BreakCharacterFormat.IsInsertRevision = check;
            }
        }
        /// <summary>
        /// Determines whether item has tracked changes.
        /// </summary>
        /// <returns>
        /// 	if has tracked changes, set to <c>true</c>.
        /// </returns>
        internal override bool HasTrackedChanges()
        {
            //TODO: Need to implement
            return false;
        }

        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, string replace)
        {
            //TODO: Need to be implemented
            return 1;
        }
        /// <summary>
        /// Replaces all entries of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given text to replace.</param>
        /// <param name="replace">The replace text .</param>
        /// <param name="caseSensitive">if specifies case sensitive, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemented
            return 0;

        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to check whole word, set to <c>true</c> .</param>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c> .</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord, bool saveFormatting)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces first entry of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The string to replace</param>
        /// <param name="replace">Replace string</param>
        /// <param name="caseSensitive">Is case sensitive replace?</param>
        /// <param name="wholeWord">Search for whole word?</param>
        /// <returns></returns>
        internal int ReplaceFirst(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        internal int ReplaceFirst(Regex pattern, string replace)
        {
            //TODO: Need to be implemented
            return 0;
        }
 #if !SILVERLIGHT && !WP
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Vertical);
        }
#endif
        /// <summary>
        /// Removes the character format changes.
        /// </summary>
        internal override void RemoveCFormatChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Removes the paragraph/table format changes.
        /// </summary>
        internal override void RemovePFormatChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Accepts the changes for character format.
        /// </summary>
        internal override void AcceptCChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Accepts changes in paragraph/table format.
        /// </summary>
        internal override void AcceptPChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Defines whether format was changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedCFormat()
        {
            //TODO: Need to be implemented

            return false;
        }
        /// <summary>
        /// Checks a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        /// <value>
        /// 	if this instance was inserted, set to <c>true</c>.
        /// </value>
        internal override bool CheckInsertRev()
        {
            //TODO: Need to be implemented
            return false;
        }
        /// <summary>
        /// Returns first entry of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public override TextSelection Find(Regex pattern)
        {
            //TODO: Need to be implemented
            return null;
        }
        /// <summary>
        /// Returns first entry of given string, taking into consideration caseSensitive
        /// and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public TextSelection Find(string given, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemeneted
            return null;
        }
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        /// <param name="acceptChanges">if it accepts changes, set to <c>true</c>.</param>
        internal override void MakeChanges(bool acceptChanges)
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Returns all entries of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal override TextSelectionList FindAll(Regex pattern)
        {
            //TODO: Need to be implemented
            return null;
        }
        /// <summary>
        /// Closes the item.
        /// </summary>
        internal override void Close()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Defines whether paragraph format is changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedPFormat()
        {
            //TODO: Need to be implemented
            return false;
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.StructureDocumentTag;
            }
        }
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_childEntities;
            }
        }
        # endregion

        #region Doc to PDF conversion
#if !SILVERLIGHT && !WP
        #region WidgetContainer overrides
        /// <summary>
        /// Gets count of child items.
        /// </summary>
        int IWidgetContainer.Count
        {
            get
            {
                return WidgetCollection.Count;
            }
        }
        /// <summary>
        /// Gets child item by index.
        /// </summary>
        IWidget IWidgetContainer.this[int index]
        {
            get
            {
                return WidgetCollection[index] as IWidget;
            }
        }
        /// <summary>
        /// Gets the child items
        /// </summary>
        protected IEntityCollectionBase WidgetCollection
        {
            get
            {
                return SDTContent.TextBody.Items;
            }
        }
        /// <summary>
        /// Gets the child items
        /// </summary>
        public EntityCollection WidgetInnerCollection
        {
            get
            {
                return WidgetCollection as EntityCollection;
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
        }
#endif
        #endregion
    }
    internal class StructureDocumentTagRow:IStructureDocumentTagRow
    {
        #region fields
        private SDTRowContent m_SDTContent;
        private SDTProperties m_SDTProperties;
        private WCharacterFormat m_BreakCharacterFormat;
        #endregion
        #region Properties
        public SDTRowContent SDTContent
        {
            get
            {
                return m_SDTContent;
            }
            set
            {
                m_SDTContent = value;
            }
        }
        public SDTProperties SDTProperties
        {
            get
            {
                return m_SDTProperties;
            }
            set
            {
                m_SDTProperties = value;
            }
        }
        public WCharacterFormat BreakCharacterFormat
        {
            get
            {
                return m_BreakCharacterFormat;
            }
            set
            {
                m_BreakCharacterFormat = value;
            }
        }
        #endregion
        #region Constructor
        internal StructureDocumentTagRow(WordDocument document)
        {
            m_SDTProperties = new SDTProperties(document);
            m_SDTContent = new SDTRowContent();
            m_BreakCharacterFormat = new WCharacterFormat(document);
        }
        #endregion
    }
    internal class StructureDocumentTagCell:IStructureDocumentTagCell
    {
        private SDTCellContent m_SDTContent;
        private SDTProperties m_SDTProperties;
        private WCharacterFormat m_BreakCharacterFormat;
        public SDTCellContent SDTContent
        {
            get
            {
                return m_SDTContent;
            }
            set
            {
                m_SDTContent = value;
            }
        }
        public SDTProperties SDTProperties
        {
            get
            {
                return m_SDTProperties;
            }
            set
            {
                m_SDTProperties = value;
            }
        }
        public WCharacterFormat BreakCharacterFormat
        {
            get
            {
                return m_BreakCharacterFormat;
            }
            set
            {
                m_BreakCharacterFormat = value;
            }
        }
        internal StructureDocumentTagCell(WordDocument document)
        {
            m_SDTProperties = new SDTProperties(document);
            m_SDTContent = new SDTCellContent();
            m_BreakCharacterFormat = new WCharacterFormat(document);
        }
    }
    internal class StructureDocumentTagInline: ParagraphItem , IStructureDocumentTagInline 
    {
        # region Fields
        private SDTInlineContent m_SDTContent;
        private SDTProperties m_SDTProperties;
        # endregion

        # region Properties
        public  SDTInlineContent SDTContent
        {
            get
            {
                return m_SDTContent;
            }
            set
            {
                m_SDTContent = value;
            }
        }
        public SDTProperties SDTProperties
        {
            get
            {
                return m_SDTProperties;
            }
            set
            {
                m_SDTProperties = value;
            }
        }
        public WCharacterFormat BreakCharacterFormat
        {
            get
            {
                return ParaItemCharFormat;
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
               return  EntityType.StructureDocumentTagInline; 
                
            }
        }
        # endregion

        # region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTable"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal StructureDocumentTagInline(WordDocument doc)
            : base((WordDocument)doc)
        {
            m_SDTProperties = new SDTProperties(doc);
            m_SDTContent = new SDTInlineContent(doc,this);
            m_SDTContent.SetOwner(this);
            m_charFormat = new WCharacterFormat(doc);
        }
        
        #endregion

        # region Implementation
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            //TODO: Need to be implemented
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal StructureDocumentTagInline Clone()
        {
            return (StructureDocumentTagInline)CloneImpl();
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            m_SDTContent.AddSelf();
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            StructureDocumentTagInline sdTagInline = (StructureDocumentTagInline)base.CloneImpl();
            return sdTagInline;
        }
        /// <summary>
        /// Applies the base format.
        /// </summary>
        internal void ApplyBaseFormat()
        {
            IWParagraphStyle style = OwnerParagraph.GetStyle();
            if (style != null)
            {
                ParaItemCharFormat.ApplyBase(style.CharacterFormat);
                for (int i = 0; i < SDTContent.ParagraphItems.Count; i++)
                {
                    SDTContent.ParagraphItems[i].ParaItemCharFormat.ApplyBase(style.CharacterFormat);
                }
            }
        }
        # endregion

        #region Doc to PDF conversion
        /// <summary>
        /// Copies the items to.
        /// </summary>
        /// <param name="paraItems">The para items.</param>
        internal void CopyItemsTo(ParagraphItemCollection paraItems)
        {
            for (int i = 0; i < SDTContent.ParagraphItems.Count; i++)
            {
                if (SDTContent.ParagraphItems[i] is StructureDocumentTagInline)
                    (SDTContent.ParagraphItems[i] as StructureDocumentTagInline).CopyItemsTo(paraItems);
                else
                    paraItems.InnerList.Add(SDTContent.ParagraphItems[i]);
            }
        }
        #endregion
    }
}
