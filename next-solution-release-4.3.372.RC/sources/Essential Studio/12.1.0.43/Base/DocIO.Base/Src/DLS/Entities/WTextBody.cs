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
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a text body.
    /// </summary>
    public class WTextBody
      : WidgetContainer
      , ITextBody
      , IWidgetContainer
    {
        #region Class members
        /// <summary>
        /// The section paragraphs
        /// </summary>
        protected BodyItemCollection m_bodyItems;
        private WParagraphCollection m_paragraphs = null;
        private WTableCollection m_tables = null;
        private FormFieldCollection m_formFields = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.TextBody;
            }
        }
        /// <summary>
        /// Gets inner paragraphs
        /// </summary>
        public IWParagraphCollection Paragraphs
        {
            get
            {
                return m_paragraphs;
            }
        }
        /// <summary>
        /// Gets inner tables
        /// </summary>
        public IWTableCollection Tables
        {
            get
            {
                return m_tables;
            }
        }
        /// <summary>
        /// Gets the form fields.
        /// </summary>
        /// <value>The form fields.</value>
        public FormFieldCollection FormFields
        {
            get
            {
                if (m_formFields == null)
                {
                    m_formFields = new FormFieldCollection(this);
                }

                return m_formFields;
            }
        }
        /// <summary>
        /// Gets the last paragraph.
        /// </summary>
        /// <value>The last paragraph.</value>
        public IWParagraph LastParagraph
        {
            get
            {
                return (Paragraphs.Count > 0) ? Paragraphs[Paragraphs.Count - 1] : null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether [form fields created].
        /// </summary>
        /// <value>if the form fields created, set to <c>true</c>.</value>
        internal bool IsFormFieldsCreated
        {
            get
            {
                return m_formFields != null;
            }
        }
        /// <summary>
        /// Gets the body items.
        /// </summary>
        /// <value>The body items.</value>
        internal BodyItemCollection Items
        {
            get
            {
                return m_bodyItems;
            }
        }
        #endregion

        #region Class properties / composite
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_bodyItems;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WTextBody"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="owner">The owner.</param>
        internal WTextBody(WordDocument doc, Entity owner)
            : base(doc, owner)
        {
            m_bodyItems = new BodyItemCollection(this);
            m_paragraphs = new WParagraphCollection(m_bodyItems);
            m_tables = new WTableCollection(m_bodyItems);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WTextBody"/> class.
        /// </summary>
        /// <param name="sec">The sec.</param>
        internal WTextBody(WSection sec)
            : this(sec.Document, sec)
        {
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds paragraph at end of section.
        /// </summary>
        /// <returns></returns>
        public IWParagraph AddParagraph()
        {
            WParagraph p = new WParagraph(Document);
            int i = m_bodyItems.Add(p);
            return m_bodyItems[i] as IWParagraph;
        }
        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <returns></returns>
        public IWTable AddTable()
        {
            IWTable table = new WTable(Document);
            m_bodyItems.Add(table);
            return table;
        }
        /// <summary>
        /// Add Structure document tag block
        /// </summary>
        /// <returns></returns>
        internal IStructureDocumentTagBlock AddStructureDocumentTag()
        {
            IStructureDocumentTagBlock sdTag = new StructureDocumentTagBlock(m_doc);
            m_bodyItems.Add(sdTag);
            return sdTag;
            
        }
        /// <summary>
        /// Add Altername Chunk
        /// </summary>
        /// <returns></returns>
        internal AlternateChunk AddAlternateChunk()
        {
            AlternateChunk altChunk = new AlternateChunk(m_doc);            
            m_bodyItems.Add(altChunk);
            return altChunk;

        }

#if (!SILVERLIGHT && !WP) || WINRT
        /// <summary>
        /// Inserts html at end of text body.
        /// </summary>
        public void InsertXHTML(string html)
        {
            InsertXHTML(html, Paragraphs.Count);
        }
        /// <summary>
        /// Inserts html. Inserting begins from paragraph specified by paragraphIndex
        /// </summary>
        public void InsertXHTML(string html, int paragraphIndex)
        {
            Paragraphs.Insert(paragraphIndex, new WParagraph(Document));
            InsertXHTML(html, paragraphIndex, 0);
        }
        /// <summary>
        /// Inserts html. Inserting begins from paragraph specified by paragraphIndex, 
        /// and paragraph item specified by paragraphItemIndex
        /// </summary>
        public void InsertXHTML(string html, int paragraphIndex, int paragraphItemIndex)
        {
            IHtmlConverter htmlConverter = HtmlConverterFactory.GetInstance();
            (htmlConverter as HTMLConverterImpl).HtmlImportSettings = this.Document.HTMLImportSettings;
            htmlConverter.AppendToTextBody(this, html, paragraphIndex, paragraphItemIndex);
        }
#endif
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Validates the XHTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="type">The validation type.</param>
        /// <returns>
        /// 	if it is valid XHTML, set to <c>true</c>.
        /// </returns>
        public bool IsValidXHTML(string html, XHTMLValidationType type)
        {
            IHtmlConverter htmlConverter = HtmlConverterFactory.GetInstance();
            (htmlConverter as HTMLConverterImpl).HtmlImportSettings = this.Document.HTMLImportSettings;
            return htmlConverter.IsValid(html, type);
        }
        /// <summary>
        /// Validates the XHTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="type">The validation type.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>
        /// 	if it is valid XHTML, set to <c>true</c>.
        /// </returns>
        public bool IsValidXHTML(string html, XHTMLValidationType type, out string exceptionMessage)
        {
            IHtmlConverter htmlConverter = HtmlConverterFactory.GetInstance();
            (htmlConverter as HTMLConverterImpl).HtmlImportSettings = this.Document.HTMLImportSettings;
            return htmlConverter.IsValid(html, type, out exceptionMessage);
        }
#endif
        /// <summary>
        /// If the text body has no paragraphs, creates and appends one WParagraph.
        /// </summary>
        public void EnsureMinimum()
        {
            if (Paragraphs.Count == 0)
            {
                AddParagraph();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Finds the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal TextSelection Find(Regex pattern)
        {
            foreach (TextBodyItem bodyItem in Items)
            {
                TextSelection textSel = bodyItem.Find(pattern);

                if (textSel != null && textSel.Count > 0)
                    return textSel;
            }

            return null;
        }
        /// <summary>
        /// Finds the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal TextSelectionList FindAll(Regex pattern)
        {
            TextSelectionList allSelections = null;

            foreach (TextBodyItem bodyItem in Items)
            {
                TextSelectionList selections = bodyItem.FindAll(pattern);

                if (selections != null && selections.Count > 0)
                {
                    if (allSelections == null)
                    {
                        allSelections = new TextSelectionList();
                    }

                    allSelections.AddRange(selections);
                }
            }

            return allSelections;
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        internal int Replace(Regex pattern, string replace)
        {
            int count = 0;

            foreach (TextBodyItem bodyItem in Items)
            {
                count += bodyItem.Replace(pattern, replace);

                if (Document.ReplaceFirst && count > 0)
                {
                    return count;
                }
            }

            return count;
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if it specifies the save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        internal int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting)
        {
            int count = 0;

            foreach (TextBodyItem bodyItem in Items)
            {
                count += bodyItem.Replace(pattern, textSelection, saveFormatting);

                if (Document.ReplaceFirst && count > 0)
                {
                    return count;
                }
            }

            return count;
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textPart">The text part.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c> .</param>
        /// <returns></returns>
        internal int Replace(Regex pattern, TextBodyPart textPart, bool saveFormatting)
        {
            if (FindUtils.IsPatternEmpty(pattern))
                throw new ArgumentException("Search string cannot be empty");

            TextSelectionList selections = FindAll(pattern);

            if (selections != null)
            {
                foreach (TextSelection sel in selections)
                {
                    WCharacterFormat srcFormat = null;
                    if (saveFormatting)
                        srcFormat = sel.StartTextRange.CharacterFormat;
                    int selIndex = sel.SplitAndErase();
                    WParagraph para = sel.OwnerParagraph;

                    // Inserts textPart
                    textPart.PasteAt(
                      para.OwnerTextBody,
                      para.GetIndexInOwnerCollection(),
                      selIndex, srcFormat, saveFormatting);

                    if (Document.ReplaceFirst)
                        break;
                }

                return selections.Count;
            }

            return 0;
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replaceDoc">The replace doc.</param>
        /// <returns></returns>
        internal int Replace(Regex pattern, IWordDocument replaceDoc, bool saveFormatting)
        {
            if (FindUtils.IsPatternEmpty(pattern))
                throw new ArgumentException("Search string cannot be empty");

            WCharacterFormat srcFormat = null;
            TextSelectionList selections = FindAll(pattern);

            if (selections != null)
            {
                foreach (TextSelection sel in selections)
                {
                    if (saveFormatting)
                        srcFormat = sel.StartTextRange.CharacterFormat;

                    int selIndex = sel.SplitAndErase();
                    WParagraph para = sel.OwnerParagraph;

                    for (int i = replaceDoc.Sections.Count - 1; i >= 0; i--)
                    {
                        IWSection repSection = replaceDoc.Sections[i];
                        if (!saveFormatting)
                            Document.CurClonedSection = repSection as WSection;
                        TextBodyPart textPart = new TextBodyPart(Document);
                        textPart.Copy(repSection.Body, false);

                        // Inserts textPart
                        textPart.PasteAt(
                          para.OwnerTextBody,
                          para.GetIndexInOwnerCollection(),
                          selIndex, srcFormat, saveFormatting
                        );
                    }

                    if (Document.ReplaceFirst)
                        break;
                }

                return selections.Count;
            }

            return 0;
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            foreach (TextBodyItem item in ChildEntities)
            {
                item.AddSelf();
            }
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            WTextBody tb = (WTextBody)base.CloneImpl();
            tb.m_bodyItems = new BodyItemCollection(tb);
            ChildEntities.CloneTo(tb.m_bodyItems);
            tb.m_paragraphs = new WParagraphCollection(tb.m_bodyItems);
            tb.m_tables = new WTableCollection(tb.m_bodyItems);
            return tb;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner )
        {
            Entity ent = null;
            for (int i = 0, cnt = ChildEntities.Count; i < cnt; i++)
            {
                ent = ChildEntities[i];
                ent.CloneRelationsTo(doc, nextOwner);
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_bodyItems != null && m_bodyItems.Count > 0)
            {
                TextBodyItem tbItem = null;

                for (int i = 0; i < m_bodyItems.Count; i++)
                {
                    tbItem = m_bodyItems[i];
                    tbItem.Close();
                    tbItem = null;
                }
                m_bodyItems.Clear();
                m_bodyItems = null;
            }

            if (m_paragraphs != null)
            {
                m_paragraphs.Clear();
                m_paragraphs = null;
            }

            if (m_tables != null)
            {
                m_tables.Clear();
                m_tables = null;
            }

            if (m_formFields != null)
            {
                m_formFields.InnerList.Clear();
                m_formFields = null;
            }
        }
        #endregion

        #region Implementation / track changes
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        /// <param name="acceptChanges">if it is accept the changes, set to <c>true</c> </param>
        internal void MakeChanges(bool acceptChanges)
        {
            TextBodyItem item = null;
            for (int i = 0; i < m_bodyItems.Count; i++)
            {
                item = m_bodyItems[i];
                // Removes item if item was added/deleted to document in case of reject/accept changes.
                if (!RemoveChangedItem(item, acceptChanges, ref i))
                {
                    bool moveToNextPara = CheckMoveToNext(item, acceptChanges);
                    if (item is WTable)
                    {
                        WTable table = item as WTable;
                        if (acceptChanges)
                        {
#if !SILVERLIGHT && !WP
                            table.m_trackTblFormat = null;
#endif
                            table.m_trackTableGrid = null;
                        }
#if !SILVERLIGHT && !WP
                        else if (table.m_trackTblFormat != null)
                        {
                            table.DocxTableFormat.Format.ClearFormatting();
                            table.DocxTableFormat = table.TrackTblFormat.Clone(table);
                            table.FirstRow.RowFormat.ClearFormatting();
                            table.FirstRow.RowFormat.ImportContainer(table.TrackTblFormat.Format);
                            table.m_trackTblFormat = null;
                        }
#endif

                        if (!acceptChanges && table.m_trackTableGrid != null)
                        {
                            table.TableGrid.Clear();
                            foreach (float grid in table.TrackTableGrid)
                            {
                                table.TableGrid.Add(grid);
                            }
                            table.m_trackTableGrid = null;
                        }
                    }

                    if (!acceptChanges)
                    {
                        RemoveChangedFormat(item);
                    }
                    // Removes sprms responsible for marking changes from character format
                    if (item.IsInsertRevision || item.IsDeleteRevision || item.IsChangedCFormat)
                    {
                        item.AcceptCChanges();
                    }
                    // Removes sprms responsible for marking changes from paragraph/table format
                    if (item.IsChangedPFormat)
                    {
                        item.AcceptPChanges();
                    }
                    // Make changes for all items in the collection of current textbodyitem.
                    item.MakeChanges(acceptChanges);

                    // Moves paragraph items to the next paragraph.
                    if (moveToNextPara)
                    {
                        if (MoveToNextPara(item))
                        {
                            ChildEntities.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Removes the changed item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="acceptChanges">if it accepts the changes, set to <c>true</c>.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns></returns>
        private bool RemoveChangedItem(TextBodyItem item, bool acceptChanges, ref int itemIndex)
        {
            if ((item.IsInsertRevision && !acceptChanges) || (item.IsDeleteRevision && acceptChanges))
            {
                // "complRemoval" defines whether to remove item completely.
                bool complRemoval = true;
                if (item is WTable)
                {
                    WTable table = item as WTable;
                    complRemoval = table.RemoveChangedTable();
                }
                else if (item is WParagraph)
                {
                    complRemoval = (item as WParagraph).CheckOnRemove();
                }
                if (complRemoval)
                {
                    ChildEntities.RemoveAt(itemIndex);
                    itemIndex -= 1;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether text body has tracked changes.
        /// </summary>
        /// <returns>
        /// 	If has tracked changes, set to <c>true</c>.
        /// </returns>
        internal bool HasTrackedChanges()
        {
            foreach (TextBodyItem item in m_bodyItems)
            {
                if (item.HasTrackedChanges())
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Removes the changed formatting.
        /// </summary>
        /// <param name="item">The item.</param>
        private void RemoveChangedFormat(TextBodyItem item)
        {
            if (item.IsChangedCFormat)
            {
                // Removes changes in character format
                item.RemoveCFormatChanges();
            }
            if (item.IsChangedPFormat)
            {
                // Removes changes in paragraph/table format
                item.RemovePFormatChanges();
            }
        }
        /// <summary>
        /// Checks the whether item's content has to be moved to next paragraph.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="acceptChanges">if it is accept the changes, set to <c>true</c> .</param>
        /// <returns></returns>
        private bool CheckMoveToNext(TextBodyItem item, bool acceptChanges)
        {
            bool moveToNextPara = false;
            if (item is WParagraph && item.NextSibling is WParagraph)
            {
                if ((item.IsInsertRevision && !acceptChanges) || (item.IsDeleteRevision && acceptChanges))
                {
                    moveToNextPara = true;
                }
            }
            return moveToNextPara;
        }
        /// <summary>
        /// Moves item's content to the next paragraph.
        /// </summary>
        /// <param name="item">The item.</param>
        private bool MoveToNextPara(TextBodyItem item)
        {
            WParagraph curPara = item as WParagraph;
            int lastIndex = curPara.Items.Count - 1;
            WParagraph nextPara = item.NextSibling as WParagraph;

            if (nextPara == null)
                return false;

            for (int j = lastIndex; j >= 0; j--)
            {
                nextPara.Items.Insert(0, curPara.Items[j] as IEntity);
            }
            return true;
        }
        #endregion

        #region IXDLSSerializable implement
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(Syncfusion.DocIO.DLS.XML.XDLSConstants.ParagraphsTag, m_bodyItems);
        }
//#endif
        #endregion

        #region WidgetContainer overrides
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Vertical);
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override IEntityCollectionBase WidgetCollection
        {
            get
            {
                return m_bodyItems;
            }
        }
        #endregion
    }
}
