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
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.Documentation;
using Syncfusion.Layouting;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// 
    /// </summary>
    [DocumentationExclude()]
    internal sealed class DocWriterAdapter
    {
        #region  Constants
        private const String LINK_STRING = "OLE_LINK";
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private int m_listID = WListFormat.DEF_START_LISTID;
        private int m_oleLinkObjCnt = 1;
        private int m_secNumber = 0;
        private int m_tableNestingLevel = 0;
        private string m_prevStyleName = string.Empty;
        private WParagraph m_lastParagarph;
        private WFieldMark m_skipFieldEnd = null;

        /// <summary>
        /// 
        /// </summary>
        private WordWriter m_mainWriter;
        private IWordWriterBase m_currWriter = null;

        /// <summary>
        /// 
        /// </summary>
        private WordDocument m_document = null;
        private IWSection m_currSection;

        /// <summary>
        /// Collection of main textboxes
        /// </summary>
        private WTextBoxCollection m_txbxItems = null;

        /// <summary>
        /// Collection of header/footer textboxes
        /// </summary>
        private WTextBoxCollection m_hfTxbxItems = null;

        /// <summary>
        /// 
        /// </summary>
        private List<WComment> m_commentCollection = null;

        /// <summary>
        /// 
        /// </summary>
        private List<WFootnote> m_footnoteCollection = null;

        /// <summary>
        /// 
        /// </summary>
        private List<WFootnote> m_endnoteCollection = null;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, int> m_charStylesHash = new Dictionary<string, int>();

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, ListData> m_listData = new Dictionary<string,ListData>();

        /// <summary>
        /// 
        /// </summary>
        private List<String> m_bookmarksAfterCell = new List<String>();

        /// <summary>
        /// Field stack.
        /// </summary>
        private Stack<WField> m_fieldStack = new Stack<WField>();

        /// <summary>
        /// Stores bookmark offsets for each comment 
        /// </summary>
        private Dictionary<int, DictionaryEntry> m_commOffsets;

        /// <summary>
        /// 
        /// </summary>
        private List<WPicture> m_listPicture;
        private List<WOleObject> m_oleObjects;
        private List<OLEObject> m_OLEObjects;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the OLE objects.
        /// </summary>
        /// <value>The OLE objects.</value>
        private List<WOleObject> OleObjects
        {
            get
            {
                if (m_oleObjects == null)
                    m_oleObjects = new List<WOleObject>();
                return m_oleObjects;
            }
        }
        /// <summary>
        /// Gets the OLE objects in Control fields.
        /// </summary>
        /// <value>The OLE objects.</value>
        private List<OLEObject> OLEObjects
        {
            get
            {
                if (m_OLEObjects == null)
                    m_OLEObjects = new List<OLEObject>();
                return m_OLEObjects;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private IWordWriterBase CurrentWriter
        {
            get
            {
                return m_currWriter;
            }
            set
            {
                m_currWriter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private WField CurrentField
        {
            get
            {
                return (m_fieldStack.Count > 0) ? m_fieldStack.Peek() : null;
            }
        }

        /// <summary>
        /// Gets the list picture.
        /// </summary>
        /// <value>The list picture.</value>
        private List<WPicture> ListPicture
        {
            get
            {
                if (m_listPicture == null)
                {
                    m_listPicture = new List<WPicture>();
                }
                return m_listPicture;
            }
        }
        /// <summary>
        /// Gets the last paragraph of the document 
        /// </summary>
        /// <value>The last paragraph.</value>
        private WParagraph LastParagraph
        {
            get
            {
                return m_lastParagarph;
            }
        }
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets the comment collection.
        /// </summary>
        /// <value>The comment collection.</value>
        private List<WComment> CommentCollection
        {
            get
            {
                if (m_commentCollection == null)
                {
                    m_commentCollection = new List<WComment>();
                }
                return m_commentCollection;
            }
        }

        /// <summary>
        /// Gets the footnote collection.
        /// </summary>
        /// <value>The footnote collection.</value>
        private List<WFootnote> FootnoteCollection
        {
            get
            {
                if (m_footnoteCollection == null)
                {
                    m_footnoteCollection = new List<WFootnote>();
                }
                return m_footnoteCollection;
            }
        }

        /// <summary>
        /// Gets the endnote collection.
        /// </summary>
        /// <value>The endnote collection.</value>
        private List<WFootnote> EndnoteCollection
        {
            get
            {
                if (m_endnoteCollection == null)
                {
                    m_endnoteCollection = new List<WFootnote>();
                }
                return m_endnoteCollection;
            }
        }

        /// <summary>
        /// Gets the header/footer text box collection.
        /// </summary>
        /// <value>The HF text box collection.</value>
        private WTextBoxCollection HFTextBoxCollection
        {
            get
            {
                if (m_hfTxbxItems == null)
                {
                    m_hfTxbxItems = new WTextBoxCollection(m_document);
                }
                return m_hfTxbxItems;
            }
        }

        /// <summary>
        /// Gets the text box collection.
        /// </summary>
        /// <value>The text box collection.</value>
        private WTextBoxCollection TextBoxCollection
        {
            get
            {
                if (m_txbxItems == null)
                {
                    m_txbxItems = new WTextBoxCollection(m_document);
                }
                return m_txbxItems;
            }
        }

        /// <summary>
        /// Gets the comment offsets.
        /// </summary>
        /// <value>The comment offsets.</value>
        private Dictionary<int,DictionaryEntry> CommentOffsets
        {
            get
            {
                if (m_commOffsets == null)
                {
                    m_commOffsets = new Dictionary<int, DictionaryEntry>();
                }
                return m_commOffsets;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        internal DocWriterAdapter()
        {
            AdapterListIDHolder.Instance.ListStyleIDtoName.Clear();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// By means of WordWriter writes WordDocument to doc
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="document">The document.</param>
        public void Write(WordWriter writer, WordDocument document)
        {
            m_document = document;

            Init(writer);
            m_mainWriter.WriteDocumentHeader();
            WriteBody();
            m_mainWriter.WriteDocumentEnd(m_document.Password);
        }
        #endregion

        #region Implementation / common
        /// <summary>
        /// Must be called before doc building
        /// </summary>
        private void Init(WordWriter writer)
        {
            ResetLists();
            m_secNumber = 0;

            CurrentWriter = writer;
            m_mainWriter = writer;
            writer.StyleSheet.FontSubstitutionTable = m_document.FontSubstitutionTable;
            writer.m_docInfo.TablesData.FFNStringTable = m_document.FFNStringTable;
            m_lastParagarph = m_document.LastParagraph;

            m_mainWriter.CharacterProperties.StickProperties = false;
            m_mainWriter.ParagraphProperties.StickProperties = false;
            m_mainWriter.SectionProperties.StickProperties = false;

            WriteStyleSheet(writer);
            // Fill the collection of list pictures.
            AddListPictures();
            SectionPropertiesConverter.Import(writer.SectionProperties, m_document.Sections[0] as WSection);
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteBody()
        {
            m_secNumber = 0;
            WriteBackground();
            WriteDocumentEscher();

            WriteMainBody();
            WriteFootnotesBody();
            WriteHFBody();
            WriteAnnotationsBody();
            WriteEndnotesBody();
            WriteTextBoxes();
            WriteDocumentProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteMainBody()
        {
            WSection section = null;
            for (int i = 0, cnt = m_document.Sections.Count; i < cnt; i++)
            {
                section = m_document.Sections[i];
                WriteSectionEnd(section);

                if (section.Body.Items.Count > 0)
                {
                    WriteParagraphs(section.Body.Items, false);
                }
                else
                {
                    m_mainWriter.ParagraphProperties.Sprms.Clear();
                    m_mainWriter.CurrentStyleIndex = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteHFBody()
        {
            bool bNextSection = false;
            bool bEmpty = true;
            if (m_document.Watermark.Type != WatermarkType.NoWatermark)
            {
                //If document has watermark -> write empty paragraphs if header/footer
                //doesn't contain any paragraphs  
                WriteWatermarkParagraphs();
            }

            WSection section = null;
            for (int i = 0, cnt = m_document.Sections.Count; i < cnt; i++)
            {
                section = m_document.Sections[i];
                if (!section.HeadersFooters.IsEmpty)
                {
                    bEmpty = false;
                    break;
                }
            }

            if (FootnoteCollection.Count > 0 || EndnoteCollection.Count > 0 || !bEmpty)
            {
                WordHeaderFooterWriter hfWriter =
                  m_mainWriter.GetSubdocumentWriter(WordSubdocument.HeaderFooter) as WordHeaderFooterWriter;
                hfWriter.CharacterProperties.StickProperties = false;
                CurrentWriter = hfWriter;
                hfWriter.ParagraphProperties.StickProperties = false;
                //Writes footnote and endnote separator stories.
                WriteSeparatorStories();
                for (int index = 0, counter = m_document.Sections.Count; index < counter; index++)
                {
                    section = m_document.Sections[index];
                    if (bNextSection)
                    {
                        hfWriter.WriteSectionEnd();
                    }
                    // Executes for headers/footers
                    for (int i = 0; i < 6; i++)
                    {
                        if (m_document.Watermark.Type != WatermarkType.NoWatermark &&
                          section.HeadersFooters[i].WriteWatermark)
                        {
                            InsertWatermark(section.HeadersFooters[i], (HeaderType)i);
                        }
                        WriteHeaderFooter(hfWriter, (section.HeadersFooters[i].ChildEntities as BodyItemCollection), (HeaderType)i);
                    }

                    bNextSection = true;
                }

                if (hfWriter != null)
                {
                    hfWriter.WriteDocumentEnd();
                }
            }
        }

        /// <summary>
        /// Writes the Footnote/Endnote separator stories.
        /// </summary>
        private void WriteSeparatorStories()
        {
            //Writes Footnote separator
            WriteSeparatorStory(m_document.Footnotes.Separator);
            //Writes Footnote ContinuationSeparator
            WriteSeparatorStory(m_document.Footnotes.ContinuationSeparator);
            //Writes Footnote Continuation Notice
            WriteSeparatorStory(m_document.Footnotes.ContinuationNotice);
            //Writes Endnote Separator
            WriteSeparatorStory(m_document.Endnotes.Separator);
            //Writes Endnote Continuation Separator
            WriteSeparatorStory(m_document.Endnotes.ContinuationSeparator);            
            //Writes Endnote continuation Notice
            WriteSeparatorStory(m_document.Endnotes.ContinuationNotice);
        }
        /// <summary>
        /// Writes the separator story body items
        /// </summary>
        /// <param name="section">The section.</param>
        private void WriteSeparatorStory(WTextBody body)
        {
            WriteParagraphs((BodyItemCollection)body.ChildEntities, false);
            if (body.ChildEntities.Count >= 1 && !(body.ChildEntities[body.ChildEntities.Count - 1] is WTable))
            {
                (CurrentWriter as WordHeaderFooterWriter).WriteMarker(WordChunkType.ParagraphEnd);
            }
            (CurrentWriter as WordHeaderFooterWriter).ClosePrevSeparator();
        }
        /// <summary>
        /// Inserts the watermark.
        /// </summary>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <param name="headerType">Type of the header.</param>
        private void InsertWatermark(WTextBody textBody, HeaderType headerType)
        {
            if (headerType == HeaderType.EvenHeader ||
              headerType == HeaderType.OddHeader ||
              headerType == HeaderType.FirstPageHeader)
            {
                WParagraph para = GetFirstPara(textBody);

                if (para == null)
                {
                    WSection curSection = textBody.OwnerBase as WSection;
                    para = new WParagraph(curSection.Document);
                    curSection.HeadersFooters[(int)headerType].Items.Insert(0, para);
                }
                para.Items.Insert(0, m_document.Watermark);
            }
        }
        /// <summary>
        /// Returns First Paragraph in the given text body
        /// </summary>
        /// <param name="textBody"></param>
        /// <returns></returns>
        private WParagraph GetFirstPara(WTextBody textBody)
        {

            if (textBody != null &&
                (textBody.Items[0] is WParagraph))
                return textBody.Items[0] as WParagraph;
            else if (textBody != null &&
                (textBody.Items[0] is WTable))
                return GetFirstTblPara(textBody.Items[0] as WTable);
            else if (textBody != null &&
                (textBody.Items[0] is StructureDocumentTagBlock))
                return GetFirstPara((textBody.Items[0] as StructureDocumentTagBlock).SDTContent.TextBody);

            return null;
        }

        /// <summary>
        /// Gets the first TBL paragraph.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        private WParagraph GetFirstTblPara(WTable table)
        {
            foreach (WTableRow row in table.Rows)
                foreach (WTableCell cell in row.Cells)
                    foreach (TextBodyItem item in cell.Items)
                        if (item is WParagraph)
                            return item as WParagraph;
            if (table.Rows.Count > 0)
                return table.Rows[0].Cells[0].AddParagraph() as WParagraph;

            return null;
        }

        /// <summary>
        ///  Write textbox body to document
        /// </summary>
        /// <param name="txbxCollection">Collection of document's textboxes</param>
        /// <param name="txBxType">Textbox type ( main or header/footer)</param>
        private void WriteTextBoxBody(WTextBoxCollection txbxCollection, WordSubdocument txBxType)
        {
            if ((txbxCollection != null) && (txbxCollection.Count > 0))
            {
                IWordSubdocumentWriter txBxWriter = m_mainWriter.GetSubdocumentWriter(txBxType);
                txBxWriter.CharacterProperties.StickProperties = false;
                txBxWriter.ParagraphProperties.StickProperties = false;
                CurrentWriter = txBxWriter;

                int cnt = txbxCollection.Count;
                for (int i = 0; i < cnt; i++)
                {
                    WriteTextBoxText(txBxWriter, txbxCollection[i] as WTextBox);
                }
                txBxWriter.WriteDocumentEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txbxWriter"></param>
        /// <param name="textBox"></param>
        private void WriteTextBoxText(IWordSubdocumentWriter txbxWriter, WTextBox textBox)
        {
            WriteParagraphs((BodyItemCollection)textBox.TextBoxBody.ChildEntities, false);
            if (txbxWriter is WordHFTextBoxWriter)
            {
                ((WordHFTextBoxWriter)txbxWriter).WriteHFTextBoxEnd(textBox.TextBoxSpid);
            }
            else
            {
                ((WordTextBoxWriter)txbxWriter).WriteTextBoxEnd(textBox.TextBoxSpid);
            }
        }

        /// <summary>
        /// Writes the footnotes body.
        /// </summary>
        private void WriteFootnotesBody()
        {
            if (m_footnoteCollection != null && m_footnoteCollection.Count > 0)
            {
                int cnt = m_footnoteCollection.Count;
                IWordSubdocumentWriter writer =
                  m_mainWriter.GetSubdocumentWriter(WordSubdocument.Footnote);

                writer.CharacterProperties.StickProperties = false;
                writer.ParagraphProperties.StickProperties = false;
                CurrentWriter = writer;

                for (int i = 0; i < cnt; i++)
                {
                    WFootnote footnote = m_footnoteCollection[i];
                    WriteSubDocumentText(writer, footnote.TextBody);
                }

                writer.WriteDocumentEnd();
            }
        }

        /// <summary>
        /// Writes the annotations body.
        /// </summary>
        private void WriteAnnotationsBody()
        {
            if (m_commentCollection != null && m_commentCollection.Count > 0)
            {
                int cnt = m_commentCollection.Count;
                IWordSubdocumentWriter writer =
                  m_mainWriter.GetSubdocumentWriter(WordSubdocument.Annotation);

                writer.CharacterProperties.StickProperties = false;
                writer.ParagraphProperties.StickProperties = false;
                CurrentWriter = writer;

                for (int i = 0; i < cnt; i++)
                {
                    WComment comment = m_commentCollection[i];

                    WriteSubDocumentText(writer, comment.TextBody);
                }

                writer.WriteDocumentEnd();
            }
        }

        /// <summary>
        /// Writes the endnotes body.
        /// </summary>
        private void WriteEndnotesBody()
        {
            if (m_endnoteCollection != null && m_endnoteCollection.Count > 0)
            {
                int cnt = m_endnoteCollection.Count;
                IWordSubdocumentWriter writer =
                  m_mainWriter.GetSubdocumentWriter(WordSubdocument.Endnote);

                writer.CharacterProperties.StickProperties = false;
                writer.ParagraphProperties.StickProperties = false;
                CurrentWriter = writer;

                for (int i = 0; i < cnt; i++)
                {
                    WFootnote endnote = m_endnoteCollection[i];
                    WriteSubDocumentText(writer, endnote.TextBody);
                }

                writer.WriteDocumentEnd();
            }
        }

        /// <summary>
        /// Write textbody for header/footer and main textboxes
        /// </summary>
        private void WriteTextBoxes()
        {
            WriteTextBoxBody(m_txbxItems, WordSubdocument.TextBox);
            WriteTextBoxBody(m_hfTxbxItems, WordSubdocument.HeaderTextBox);
        }

        /// <summary>
        /// Writes header/footer to wordwriter
        /// </summary>
        /// <param name="hfWriter"></param>
        /// <param name="collection"></param>
        /// <param name="hType"></param>
        private void WriteHeaderFooter(WordHeaderFooterWriter hfWriter, BodyItemCollection collection,
          HeaderType hType)
        {
            hfWriter.HeaderType = hType;
            WriteParagraphs(collection, false);

            if (collection.Count >= 1 && !(collection[collection.Count - 1] is WTable))
            {
                hfWriter.WriteMarker(WordChunkType.ParagraphEnd);
            }
        }

        /// <summary>
        /// Writes the sub document text.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="body">The body.</param>
        private void WriteSubDocumentText(IWordSubdocumentWriter writer, WTextBody body)
        {
            writer.WriteItemStart();
            WriteParagraphs((BodyItemCollection)body.ChildEntities, false);
            writer.WriteItemEnd();
        }
        /// <summary>
        /// Update the text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ModifyText(string text)
        {
            text = text.Replace(Environment.NewLine, DocxSerializator.CarriageReturn.ToString());
            text = text.Replace(DocxSerializator.NewLine, DocxSerializator.CarriageReturn);
            text = text.Replace('\a'.ToString(), string.Empty);
            text = text.Replace('\b'.ToString(), string.Empty);
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <param name="isTableBody"></param>
        private void WriteParagraphs(BodyItemCollection paragraphs, bool isTableBody)
        {
            IEntity item = null;
            IWordWriterBase writer = CurrentWriter;

            for (int i = 0, length = paragraphs.Count; i < paragraphs.Count; i++)
            {
                //if( CheckCurItemInTable( isTableBody, paragraphs, i )) continue;
                if (i != 0)
                {
                    if (isTableBody)
                    {
                        CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                    }
                    if (item is IWParagraph)
                    {
                        WParagraph para = item as WParagraph;
                        if (para.RemoveEmpty && para.Text == string.Empty)
                        { }
                        else
                        {
                            writer.WriteMarker(WordChunkType.ParagraphEnd);
                        }
                    }
                }
                if (isTableBody)
                {
                    writer.ParagraphProperties.IsCellMark = true;
                    CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                }

                item = paragraphs[i];
                if (item is IWParagraph)
                {
                    WParagraph paragraph = item as WParagraph;

                    if (paragraph.RemoveEmpty && paragraph.Text == string.Empty)
                        continue;
                    if (paragraph.ParagraphFormat.PageBreakAfter && !IsPageBreakNeedToBeSkipped(paragraph as Entity))
                        paragraph.InsertBreak(BreakType.PageBreak);
                    if (paragraph.ParagraphFormat.ColumnBreakAfter && !IsPageBreakNeedToBeSkipped(paragraph as Entity))
                        paragraph.InsertBreak(BreakType.ColumnBreak);
                    WriteParagraph(item as IWParagraph);
                }
                else if (item is IWTable)
                {
                    WriteTable(item as IWTable);
                }
                else if (item is StructureDocumentTagBlock)
                {
                    bool writeParaEnd = WriteSDTBlock((item as StructureDocumentTagBlock), isTableBody);
                    if (item.NextSibling != null && writeParaEnd)
                        writer.WriteMarker(WordChunkType.ParagraphEnd);
                }
            }
        }
        /// <summary>
        /// Writes the SDT block.
        /// </summary>
        /// <param name="sdtBlock">The SDT block.</param>
        /// <param name="isTableBody">if set to <c>true</c> [is table body].</param>
        /// <returns></returns>
        private bool WriteSDTBlock(StructureDocumentTagBlock sdtBlock, bool isTableBody)
        {
            BodyItemCollection items = sdtBlock.SDTContent.TextBody.Items;
            bool writeParaEnd = (items.LastItem is WParagraph);
            IEntity item = null;
            IWordWriterBase writer = CurrentWriter;
            for (int i = 0; i < items.Count; i++)
            {
                if (i != 0)
                {
                    if (isTableBody)
                    {
                        CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                    }
                    if (item is IWParagraph)
                    {
                        WParagraph para = item as WParagraph;
                        if (para.RemoveEmpty && para.Text == string.Empty)
                        { }
                        else
                        {
                            writer.WriteMarker(WordChunkType.ParagraphEnd);
                        }
                    }
                }
                if (isTableBody)
                {
                    writer.ParagraphProperties.IsCellMark = true;
                    CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                }

                item = items[i];
                if (item is IWParagraph)
                {
                    WParagraph paragraph = item as WParagraph;

                    if (paragraph.RemoveEmpty && paragraph.Text == string.Empty)
                        continue;
                    if (paragraph.ParagraphFormat.PageBreakAfter && !IsPageBreakNeedToBeSkipped(paragraph as Entity))
                        paragraph.InsertBreak(BreakType.PageBreak);
                    if (paragraph.ParagraphFormat.ColumnBreakAfter && !IsPageBreakNeedToBeSkipped(paragraph as Entity))
                        paragraph.InsertBreak(BreakType.ColumnBreak);

                    WriteParagraph(item as IWParagraph);
                }
                else if (item is IWTable)
                {
                    WriteTable(item as IWTable);
                }
                else if (item is StructureDocumentTagBlock)
                {
                    writeParaEnd = WriteSDTBlock((item as StructureDocumentTagBlock), isTableBody);
                    if (item.NextSibling != null
                        && writeParaEnd)
                    {
                        writer.WriteMarker(WordChunkType.ParagraphEnd);
                        writeParaEnd = false;
                    }
                }
            }
            return writeParaEnd;
        }

        /// <summary>
        /// Checks the next item in table.
        /// </summary>
        /// <param name="isTableBody">if it is table body, set to <c>true</c>.</param>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns></returns>
        private bool CheckCurItemInTable(bool isTableBody, BodyItemCollection paragraphs, int itemIndex)
        {
            bool checkResult = false;
            if (isTableBody)
            {
                IEntity item = paragraphs[itemIndex];

                if (item is WParagraph)
                {
                    if ((item as WParagraph).Items.Count == 1)
                    {
                        WParagraph para = item as WParagraph;
                        if (para.Items[0] is BookmarkEnd)
                        {
                            if ((para.Items[0] as BookmarkEnd).IsCellGroupBkmk)
                            {
                                m_bookmarksAfterCell.Add((para.Items[0] as BookmarkEnd).Name);
                                checkResult = true;
                            }
                        }
                    }
                }
            }
            return checkResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraph"></param>
        private void WriteParagraph(IWParagraph paragraph)
        {
            WParagraph para = null;
            string textToDisplay = ModifyText((paragraph as WParagraph).Text);
            if (textToDisplay.Contains("\r"))
                (paragraph as WParagraph).SplitTextRange();
            //bool isLastPara = (paragraph == m_document.LastParagraph) ? true : false;
            bool isLastPara = (paragraph == this.LastParagraph) ? true : false;

            //AddPictures( paragraph.ListFormat );
            WriteParagraphProperties(paragraph);

            for (int i = 0, len = paragraph.Items.Count; i < len; i++)
            {
                ParagraphItem item = paragraph[i] as ParagraphItem;
                if ((item is WField) && (item as WField).FieldType == FieldType.FieldNext &&
                        (item as WField).Range.Count != 0 && (item as WField).ConvertedToText)
                    m_skipFieldEnd = (item as WField).FieldEnd;

                if (m_skipFieldEnd == null)
                    WriteParaItem(item, paragraph);

                if ((item is WFieldMark) && (item as WFieldMark) == m_skipFieldEnd)
                    m_skipFieldEnd = null;              
            }

            if (isLastPara && ListPicture.Count > 0)
            {
                WriteBookmarkStart(new BookmarkStart(m_document, "_PictureBullets"));
                WriteListPictures();
                WriteBookmarkEnd(new BookmarkEnd(m_document, "_PictureBullets"));
            }

        }

        /// <summary>
        /// Writes the list pictures.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="item">The item.</param>
        private void WriteListPictures()
        {
            for (int j = 0, count = ListPicture.Count; j < count; j++)
            {
                WPicture pic = ListPicture[j];
                pic.PictureCharacterFormat.Hidden = true;
                WriteImage(pic);
            }
        }

        /// <summary>
        /// Writes the paragraph item.
        /// </summary>
        /// <param name="item">The paragraph item.</param>
        /// <param name="paragraph">The paragraph.</param>
        private void WriteParaItem(ParagraphItem item, IWParagraph paragraph)
        {
            WTextRange text = item as WTextRange;

            if (item is WFormField)
            {
                WriteFormField(item as WFormField);
            }
            // For IWMergeField items
            else if (item is IWField && !(item as WField).ConvertedToText)
            {
                WriteField(item as WField);
            }
            // For IWTextRange items
            else if (text != null)
            {
                WriteText(text);
            }
            // For IPicture items
            else if (item is WPicture
                && (item as WPicture).ImageRecord != null)
            {
                WriteImage(item as WPicture);
            }
            // For Bookmark start and Bookmark end
            else if (item is BookmarkStart)
            {
                WriteBookmarkStart(item as BookmarkStart);
            }
            else if (item is BookmarkEnd)
            {
                WriteBookmarkEnd(item as BookmarkEnd);
            }
            else if (item is WSymbol)
            {
                WriteSymbol(item as WSymbol);
            }
            // For textbox items
            else if (item is IWTextBox)
            {
                WriteTextBoxShape(item as WTextBox);
            }
            else if (item is ShapeObject)
            {
                WriteShapeObject(item as ShapeObject);
            }
            else if (item is WFieldMark)
            {
                WriteFieldMarkAndText(item as WFieldMark);
            }
            else if (item is WComment)
            {
                WriteComment(item as WComment);
            }
            else if (item is WFootnote)
            {
                WriteFootnote(item as WFootnote);
            }
            else if (item is Break)
            {
                WriteBreak(item as Break, (WParagraph)paragraph);
            }
            else if (item is Watermark)
            {
                WriteWatermark(item as Watermark);
            }
            else if (item is TableOfContent)
            {
                WriteTOC(item as TableOfContent);
            }
            else if (item is WCommentMark)
            {
                WriteCommMark(item as WCommentMark);
            }
            else if (item is WOleObject)
            {
                WriteOleObject(item as WOleObject);
            }
            else if (item is WAbsoluteTab)
            {
                WriteAbsoluteTab(item as WAbsoluteTab);
            }
            else if (item is StructureDocumentTagInline)
            {
                ParagraphItemCollection paraItems = (item as StructureDocumentTagInline).SDTContent.ParagraphItems;
                for (int i = 0; i < paraItems.Count; i++)
                {
                    WriteParaItem(paraItems[i], paragraph);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteDocumentEscher()
        {
            EscherClass escher = m_document.Escher;
            if (escher != null)
            {
                (m_currWriter as WordWriterBase).Escher = escher;
            }
        }

        /// <summary>
        /// Write Absolute tab
        /// </summary>
        private void WriteAbsoluteTab(WAbsoluteTab absoluteTab)
        {
            WTextRange textRange = new WTextRange(m_document);
            textRange.Text = absoluteTab.Text;
            textRange.ApplyCharacterFormat(absoluteTab.CharacterFormat);
            WriteText(textRange);
        }

        /// <summary>
        /// Write empty paragraphs (needed for watermark).
        /// </summary>
        private void WriteWatermarkParagraphs()
        {
            if (m_document.Sections.Count == 0)
            {
                m_document.AddSection();
            }
            //WSection section = m_document.Sections[ 0 ] as WSection;
            foreach (WSection section in m_document.Sections)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (section.HeadersFooters[i].ChildEntities.Count == 0 && section.HeadersFooters[i].WriteWatermark)
                    {
                        WParagraph watermarkParagraph = new WParagraph(m_document);
                        section.HeadersFooters[i].ChildEntities.Add(watermarkParagraph);
                    }
                }
            }
        }
        #endregion

        #region Implementation / paragraph items
        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void WriteText(WTextRange text)
        {
            UpdateCharStyleIndex(text.CharacterFormat.CharStyleName, false);
            CharacterPropertiesConverter.FormatToCHP(text.CharacterFormat, CurrentWriter.CharacterProperties);

            // If it is a mergeField and it needs to be converted into text check if 
            // it has any text 
            if (text is WMergeField && (text as WMergeField).ConvertedToText && text.Text != "")
            {
                WriteConvertedMergeField(text);
            }
            else
            {
                if (text.Text != SpecialCharacters.FootnoteAsciiStr)
                {
                    //Writes the Text value
                    string textValue = text.Text;
                    if (CurrentField is WTextFormField)
                        textValue = FormFieldPropertiesConverter.FormatText(CurrentField.TextFormat, textValue);
                    WriteTextChunks(textValue, text.SafeText);
                }
                else
                {
                    CurrentWriter.WriteMarker(WordChunkType.Footnote);
                }
            }
            CurrentWriter.CharacterProperties.Sprms.Clear();
        }
        /// <summary>
        /// Writes the text chunks.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="safeText">if it is a safe text, set to <c>true</c>.</param>
        private void WriteTextChunks(string text, bool safeText)
        {
            if (safeText)
            {
                CurrentWriter.WriteSafeChunk(text);
            }
            else
            {
                CurrentWriter.WriteChunk(text);
            }
        }

        /// <summary>
        /// Builds WordWriter doc from WordDocument
        /// </summary>
        private void WriteField(WField field)
        {
            if (field.FieldEnd == null && field.FieldType == FieldType.FieldUnknown)
            {
                WTextRange textRange = new WTextRange(m_document);
                textRange.ApplyCharacterFormat(field.CharacterFormat);
                textRange.Text = field.FieldCode;
                WriteText(textRange);
            }
            else
            {
                m_fieldStack.Push(field);
                if (field is WControlField && (field as WControlField).StoragePicLocation > 0)
                    OLEObjects.Add((field as WControlField).OleObject);
                if (field is WIfField)
                {
                    (field as WIfField).UpdateExpString();
                }

                UpdateCharStyleIndex(field.CharacterFormat.CharStyleName, false);
                CharacterPropertiesConverter.FormatToCHP(field.CharacterFormat, CurrentWriter.CharacterProperties);

                string fieldSwitches = field.FormattingString;
                string fieldName = UpdateFieldName(field);
                string fieldValue = UpdateFieldValue(field, fieldName);

                string fieldCode = string.Empty;
                if (field.FieldType == FieldType.FieldHyperlink)
                {
                    fieldCode = UpdateFieldCode(field, fieldName, fieldValue, fieldSwitches);
                }
                else
                {
                    fieldCode = string.IsNullOrEmpty(field.FieldCode) ?
                                 UpdateFieldCode(field, fieldName, fieldValue, fieldSwitches) : field.FieldCode;
                }

                if (field.FieldType == FieldType.FieldIndexEntry)
                {
                    CurrentWriter.InsertFieldIndexEntry(fieldCode);
                }
                else if (field.FieldType == FieldType.FieldTOCEntry)
                {
                    WriteFldTocEntry(fieldCode);
                }
                else
                {
                    bool hasSeparator = false;
                    if (field.FieldType == FieldType.FieldMergeField || field.FieldType == FieldType.FieldNext)
                    {
                        hasSeparator = true;
                    }
                    CurrentWriter.InsertStartField(fieldCode, field, hasSeparator);

                    if (field.FieldType == FieldType.FieldMergeField || (field.FieldType == FieldType.FieldNext && field.Range.Count == 0))
                    {
                        if (field is WMergeField && (field as WMergeField).TextItems.Count > 0)
                        {
                            WMergeField mergeField = field as WMergeField;
                            WTextRange textRange = null;
                            for (int i = 0, cnt = mergeField.TextItems.Count; i < cnt; i++)
                            {
                                textRange = mergeField.TextItems[i] as WTextRange;
                                CharacterPropertiesConverter.FormatToCHP(textRange.CharacterFormat, CurrentWriter.CharacterProperties);
                                CurrentWriter.WriteChunk(textRange.Text);
                            }
                        }
                        else
                        {
                            CharacterPropertiesConverter.FormatToCHP(field.CharacterFormat, CurrentWriter.CharacterProperties);
                            CurrentWriter.WriteSafeChunk(fieldValue);
                        }

                        CurrentWriter.InsertEndField();
                        if (m_fieldStack.Count > 0)
                            m_fieldStack.Pop();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="embedField"></param>
        private void WriteOleObjectCharProps(WField field)
        {
            if (field.Owner is WOleObject)
            {
                WOleObject oleObject = (field.Owner as WOleObject);
                int picLocation = 0;
                if( int.TryParse(oleObject.OleStorageName, out picLocation ) )
                    CurrentWriter.CharacterProperties.PicLocation = picLocation;
                CurrentWriter.CharacterProperties.IsOle2 = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        private void WriteConvertedMergeField(IWTextRange text)
        {
            bool bStick = CurrentWriter.CharacterProperties.StickProperties;
            CurrentWriter.CharacterProperties.StickProperties = true;

            WMergeField mergeField = text as WMergeField;
            // Apply general switches for the text of the converted to text field
            //      if( mergeField.UpperCase )
            if (mergeField.TextFormat == TextFormat.Uppercase)
            {
                mergeField.Text = mergeField.Text.ToUpper();
            }
            //      else if( mergeField.LowerCase )
            else if (mergeField.TextFormat == TextFormat.Lowercase)
            {
                mergeField.Text = mergeField.Text.ToLower();
            }
            //      else if( mergeField.FirstCapital )
            else if (mergeField.TextFormat == TextFormat.FirstCapital)
            {
                string s = mergeField.Text;
                string cap = s[0].ToString().ToUpper();
                mergeField.Text = cap + s.Remove(0, 1);
            }
            else if (mergeField.TextFormat == TextFormat.Titlecase)
            //      else if( mergeField.TitleCase )
            {
                string[] words = mergeField.Text.Split(new char[] { ' ' });
                for (int i = 0; i < words.Length; i++)
                {
                    string s = words[i];
                    if (s != string.Empty)
                    {
                        string cap = s[0].ToString().ToUpper();
                        s = s.Remove(0, 1);
                        words[i] = cap + s;
                    }
                }
                mergeField.Text = "";
                int wordsCount = words.Length;
                for (int i = 0; i < wordsCount; i++)
                {
                    mergeField.Text += words[i];
                    if (i < wordsCount - 1)
                    {
                        mergeField.Text += " ";
                    }
                }
            }
            if (mergeField.TextBefore != null && mergeField.TextBefore != "")
            {
                CurrentWriter.WriteChunk(mergeField.TextBefore);
            }

            try
            {
                if (mergeField.NumberFormat != "")
                {
                    double d = double.Parse(mergeField.Text, CultureInfo.InvariantCulture);
                    if (mergeField.NumberFormat.Contains("%"))
                        d = d / 100;
                    string numberFormat = mergeField.NumberFormat;
                    //If the decimal separator of current culture and number format is same, 
                    //then update the decimal separator of invariant culture in the number format.
                    if (CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator == ","
                        && mergeField.NumberFormat.Contains(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator))
                        numberFormat = numberFormat.Replace(',', '.');
                    string s = d.ToString(numberFormat);
                    CurrentWriter.WriteChunk(s);
                }
                else if (mergeField.DateFormat != "")
                {
                    DateTime dateTime = DateTime.Parse(mergeField.Text);
                    string s = dateTime.ToString(mergeField.DateFormat, DateTimeFormatInfo.CurrentInfo);
                    CurrentWriter.WriteChunk(s);
                }
                else
                {
                    CurrentWriter.WriteChunk(mergeField.Text);
                }
            }
            catch
            {
                CurrentWriter.WriteChunk(mergeField.Text);
            }

            if (mergeField.TextAfter != null && mergeField.TextAfter != "")
            {
                CurrentWriter.WriteChunk(mergeField.TextAfter);
            }

            CurrentWriter.CharacterProperties.StickProperties = bStick;
        }

        /// <summary>
        /// Writes the form field.
        /// </summary>
        /// <param name="field">The field.</param>
        private void WriteFormField(WFormField field)
        {
            m_fieldStack.Push(field);
            FormField frmField = null;
            if (field.HasFFData)
            {
                frmField = new FormField(field.FieldType);
                FormFieldPropertiesConverter.WriteFormFieldProperties(frmField, field);
            }
            string fieldCode = FieldTypeDefiner.GetFieldCode(field.FieldType);
            if (field.FieldCode != null && field.FieldCode.Trim() != string.Empty && !field.FieldCode.Trim().ToUpper().Equals(fieldCode.ToUpper()))
                fieldCode = field.FieldCode;
            CharacterPropertiesConverter.FormatToCHP(field.CharacterFormat, CurrentWriter.CharacterProperties);
            string fieldSwitches = field.FormattingString;

            // Adds uri if field is hyperlink
            if (field.FieldValue != null && field.FieldValue.Length != 0)
            {
                fieldCode += " " + field.FieldValue + " " + fieldSwitches;
            }
            fieldCode = " " + fieldCode + " ";
            CurrentWriter.InsertFormField(fieldCode, frmField);

            if (frmField != null
                && field.FormFieldType == FormFieldType.TextInput)
            {
                if ((field as WTextFormField).TextRange.Text.Length == 0)
                {
                    if (frmField.DefaultTextInputValue.Length > 0)
                    {
                        (field as WTextFormField).TextRange.Text = frmField.DefaultTextInputValue;
                    }
                }
            }
        }

        /// <summary>
        /// Builds WordWriter doc from WordDocument
        /// </summary>
        private void WriteTable(IWTable table)
        {
            //CurrentWriter.ParagraphProperties.ClonePapx();
            CurrentWriter.ParagraphProperties.Sprms.Clear();

            if (table != null)
            {
                for (int i = 0, rowsCount = table.Rows.Count; i < rowsCount; i++)
                {
                    m_tableNestingLevel++;
                    WTableRow row = table.Rows[i];

                    int cellsCount = row.Cells.Count;

                    for (int j = 0; j < cellsCount; j++)
                    {
                        WTableCell cell = row.Cells[j];
                        CurrentWriter.CharacterProperties.Sprms.Clear();
                        CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                        WriteParagraphs((BodyItemCollection)cell.ChildEntities, true);
#if WINRT || (!SILVERLIGHT && !WP)
                        if (cell.Items.Count > 0 && cell.Items.LastItem is WParagraph && m_document.ActualFormatType == FormatType.Html)
                            cell.CharacterFormat.ImportContainer(cell.LastParagraph.BreakCharacterFormat);
#endif
                        CharacterPropertiesConverter.FormatToCHP(cell.CharacterFormat, CurrentWriter.CharacterProperties);
                        UpdateCharStyleIndex(cell.CharacterFormat.CharStyleName, false);
                        CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                        CurrentWriter.WriteCellMark(m_tableNestingLevel);
                        for (int k = 0, cnt = m_bookmarksAfterCell.Count; k < cnt; k++)
                        {
                            CurrentWriter.InsertBookmarkEnd(m_bookmarksAfterCell[k]);
                        }
                        m_bookmarksAfterCell.Clear();
                    }
                    if (m_tableNestingLevel == 1)
                    {
                        CurrentWriter.ParagraphProperties.IsCellMark = true;
                        CurrentWriter.ParagraphProperties.IsRowMark = true;
                        CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                    }
                    else
                    {
                        CurrentWriter.ParagraphProperties.IsCellMark = true;
                        CurrentWriter.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
                        CurrentWriter.ParagraphProperties.IsSubCell = true;
                        CurrentWriter.ParagraphProperties.IsSubRow = true;
                    }
                    WriteTableProps(CurrentWriter, row, table);
                    CurrentWriter.WriteRowMark(m_tableNestingLevel, cellsCount);
                    m_tableNestingLevel--;
                }
            }
            CurrentWriter.ParagraphProperties.Sprms.Clear();
        }

        /// <summary>
        /// Writes the table props.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">The row.</param>
        /// <param name="table">The table.</param>
        private void WriteTableProps(IWordWriterBase writer, WTableRow row, IWTable table)
        {
            if (row.RowFormat.HasInvalidSprms || row.RowFormat.Sprms == null)
            {
                bool hasRowDesc = (row.RowFormat.RowDescriptor == null) ? false : true;
                WriteTableProperties(CurrentWriter, row, table, hasRowDesc);
                writer.ParagraphProperties.TablesNestingLevel = m_tableNestingLevel;
            }
            else
            {
                TablePropertiesConverter.Import(CurrentWriter.ParagraphProperties, row.RowFormat);
                CharacterPropertiesConverter.FormatToCHP(row.CharacterFormat, writer.CharacterProperties);
            }
        }

        /// <summary>
        /// Builds WordWriter doc from WordDocument
        /// </summary>
        private void WriteImage(IWPicture picture)
        {
            WPicture wPict = picture as WPicture;
            wPict.IsHeaderPicture = (CurrentWriter is WordHeaderFooterWriter || CurrentWriter is WordHFTextBoxWriter) ? true : false;
            int height = (int)Math.Round(picture.Height * DLSConstants.TwipsInOnePoint);
            int width = (int)Math.Round(picture.Width * DLSConstants.TwipsInOnePoint);

            if (wPict.TextWrappingStyle == TextWrappingStyle.Inline)
            {
                WriteInlinePicture(wPict, height, width);
            }
            else
            {
                WritePictureShape(wPict, height, width);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wPict"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="divider"></param>
        private void WritePictureShape(WPicture wPict, int height, int width)
        {
            CheckShapeForCloning(wPict);
            int vertPos = (int)Math.Round(wPict.VerticalPosition * DLSConstants.TwipsInOnePoint);
            int horizPos = (int)Math.Round(wPict.HorizontalPosition * DLSConstants.TwipsInOnePoint);

            PictureShapeProps pictProps = new PictureShapeProps();

            if (wPict.HorizontalOrigin == HorizontalOrigin.LeftMargin || wPict.HorizontalOrigin == HorizontalOrigin.RightMargin
                || wPict.HorizontalOrigin == HorizontalOrigin.InsideMargin || wPict.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                pictProps.RelHrzPos = HorizontalOrigin.Margin;
            else
                pictProps.RelHrzPos = wPict.HorizontalOrigin;
            pictProps.RelVrtPos = wPict.VerticalOrigin;
            pictProps.XaLeft = horizPos;
            pictProps.YaTop = vertPos;
            pictProps.Width = (int)(((float)width / 100) * wPict.WidthScale);
            pictProps.Height = (int)(((float)height / 100) * wPict.HeightScale);            
            pictProps.TextWrappingType = wPict.TextWrappingType;            
            
            if (wPict.TextWrappingStyle == TextWrappingStyle.Behind)
            {
                pictProps.IsBelowText = true;
            }
            else
            {
                pictProps.IsBelowText = wPict.IsBelowText;
            }
            pictProps.TextWrappingStyle = wPict.TextWrappingStyle;
            pictProps.HorizontalAlignment = wPict.HorizontalAlignment;
            pictProps.VerticalAlignment = wPict.VerticalAlignment;
            pictProps.Spid = wPict.ShapeId;
            pictProps.TxbxCount = 0;
            pictProps.AlternativeText = wPict.AlternativeText;

            CurrentWriter.InsertShape(wPict, pictProps);
            if (wPict.EmbedBody != null)
                WriteEmbedBody(wPict.EmbedBody, pictProps.Spid);
        }

        /// <summary>
        /// Writes the embed body.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="shapeId">The shape id.</param>
        private void WriteEmbedBody(WTextBody text, int shapeId)
        {
            WTextBox newTextBox = new WTextBox(m_document);
            newTextBox.SetTextBody(text);

            for (int i = 0, iCount = text.Items.Count; i < iCount; i++ )
            {
                TextBodyItem item = text.Items[i] as TextBodyItem;
                if (item is WParagraph)
                {
                    WParagraph para = item as WParagraph;
                    for (int j = 0, pCount = para.Items.Count; i < pCount; i++)
                    {
                        ParagraphItem pItem = para.Items[j];
                        if ( ( pItem is BookmarkStart && !( pItem as BookmarkStart ).Name.StartsWith( "OLE_LINK" ) ) ||
                            ( pItem is BookmarkEnd && !( pItem as BookmarkEnd ).Name.StartsWith( "OLE_LINK" ) ) )
                        {
                            para.Items.Remove(pItem);
                        }                       
                    }
                }
            }

            newTextBox.TextBoxSpid = shapeId;

            newTextBox.TextBoxFormat.TextBoxIdentificator = (m_currWriter as WordWriterBase).NextTextId;            
            PrepareTextBoxColl(newTextBox);
            MsofbtSpContainer shape = m_document.Escher.FindContainerBySpid(shapeId) as MsofbtSpContainer;
            if (shape == null)
                return;

            FOPTEBid option = shape.ShapeOptions.Properties[(int)FOPTEBlip.pictureId] as FOPTEBid;
            if (option != null)
            {
                option.Value = (uint)newTextBox.TextBoxFormat.TextBoxIdentificator;
            }
            else
            {
                shape.ShapeOptions.Properties.Add( new FOPTEBid((int)FOPTEBlip.pictureId, false, (uint)newTextBox.TextBoxFormat.TextBoxIdentificator) );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wPict"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        private void WriteInlinePicture(WPicture wPict, int height, int width)
        {
            CheckShapeForCloning(wPict);
            CharacterPropertiesConverter.FormatToCHP(wPict.PictureCharacterFormat, CurrentWriter.CharacterProperties);
            if (wPict.PictureShape.ShapeContainer == null || wPict.PictureShape.ShapeContainer.Shape == null ||
              (wPict.PictureShape.ShapeContainer != null && wPict.PictureShape.ShapeContainer.Bse.Blip.IsDib) ||
              (wPict.IsMetaFile && wPict.PictureShape.ShapeContainer.Bse.Blip is MsofbtImage) || wPict.Document.m_isReadOnly)
            {
                CurrentWriter.InsertImage(wPict, height, width);
            }
            else
            {
                // When preserving
                wPict.PictureShape.ShapeContainer.CheckOptContainer();
                PictureShapeProps pictProps = new PictureShapeProps();
                pictProps.AlternativeText = wPict.AlternativeText;
                wPict.PictureShape.ShapeContainer.WritePictureOptions(pictProps,wPict);

                wPict.PictureShape.PictureDescriptor.SetBasePictureOptions(height, width,
                  wPict.HeightScale, wPict.WidthScale);
                (m_currWriter as WordWriterBase).InsertInlineShapeObject(wPict.PictureShape);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeObject"></param>
        private void WriteShapeObject(ShapeObject shapeObject)
        {
            CheckShapeForCloning(shapeObject);
            CharacterPropertiesConverter.FormatToCHP(shapeObject.CharacterFormat, CurrentWriter.CharacterProperties);
            if (shapeObject is InlineShapeObject)
            {
                (m_currWriter as WordWriterBase).InsertInlineShapeObject(shapeObject as InlineShapeObject);
            }
            else
            {
                (m_currWriter as WordWriterBase).InsertShapeObject(shapeObject);
                WriteShapeObjTextBody(shapeObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeObject"></param>
        private void WriteShapeObjTextBody(ShapeObject shapeObject)
        {
            if (m_currWriter is WordHeaderFooterWriter)
            {
                for (int i = 0, cnt = shapeObject.AutoShapeTextCollection.Count; i < cnt; i++)
                {
                    HFTextBoxCollection.Add(shapeObject.AutoShapeTextCollection[i] as WTextBox);
                }
            }
            else
            {
                for (int i = 0, cnt = shapeObject.AutoShapeTextCollection.Count; i < cnt; i++)
                {
                    TextBoxCollection.Add(shapeObject.AutoShapeTextCollection[i] as WTextBox);
                }
            }
        }

        /// <summary>
        /// Builds WordWriter doc from WordDocument
        /// </summary>
        private void WriteSectionEnd(IWSection section)
        {
            if (CurrentWriter is WordWriter)
            {
                if (m_secNumber != 0)
                {
                    SectionPropertiesConverter.Import(m_mainWriter.SectionProperties, section as WSection);
                    m_mainWriter.WriteMarker(WordChunkType.SectionEnd);
                }

                m_currSection = section;
                m_secNumber++;
            }
        }

        /// <summary>
        /// Builds WordWriter doc from WordDocument
        /// </summary>
        private void WriteBookmarkStart(BookmarkStart start)
        {
            //if (start.Name.StartsWith(LINK_STRING))
            //{
            //    start.SetName(LINK_STRING + m_oleLinkObjCnt);
            //}
            //      if( CurrentWriter is WordWriter )
            {
                m_mainWriter.InsertBookmarkStart(start.Name, start);
            }
        }

        /// <summary>
        /// Builds WordWriter doc from WordDocument
        /// </summary>
        private void WriteBookmarkEnd(BookmarkEnd end)
        {
            //if (end.Name.StartsWith(LINK_STRING))
            //{
            //    end.SetName(LINK_STRING + m_oleLinkObjCnt);
            //    m_oleLinkObjCnt += 1;
            //}
            //      if( CurrentWriter is WordWriter )
            {
                m_mainWriter.InsertBookmarkEnd(end.Name);
            }
        }

        /// <summary>
        /// Writes the break.
        /// </summary>
        /// <param name="docBreak">The doc break.</param>
        /// <param name="paragraph">The paragraph.</param>
        private void WriteBreak(Break docBreak, WParagraph paragraph)
        {
            CharacterPropertiesConverter.FormatToCHP(docBreak.TextRange.CharacterFormat, CurrentWriter.CharacterProperties);

            if (docBreak.BreakType == BreakType.ColumnBreak)
            {
                m_mainWriter.WriteMarker(WordChunkType.ColumnBreak);
            }
            else if (docBreak.BreakType == BreakType.PageBreak)
            {
                if (CurrentWriter is WordWriter)
                {
                    ParagraphPropertiesConverter.Import(CurrentWriter.ParagraphProperties, paragraph.ParagraphFormat, paragraph);
                    (CurrentWriter as WordWriter).InsertPageBreak();
                }
            }
            else
            {
                if (docBreak.BreakType == BreakType.LineBreak && docBreak.TextRange.Text == "\r")
                    WriteText(docBreak.TextRange);
                else
                    CurrentWriter.WriteMarker(WordChunkType.LineBreak);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        private void WriteSymbol(WSymbol symbol)
        {
            if (CurrentWriter.CharacterProperties.StyleSheet.FontNameToIndex(symbol.FontName) == -1)
                CurrentWriter.CharacterProperties.StyleSheet.UpdateFontName(symbol.FontName);
            UpdateCharStyleIndex(symbol.CharacterFormat.CharStyleName, false);
            CharacterPropertiesConverter.FormatToCHP(symbol.CharacterFormat, CurrentWriter.CharacterProperties);
            //      string charStyleName = symbol.CharacterFormat.CharStyleName;
            //      if( charStyleName != null )
            //      {
            //        object styleId = m_charStylesHash[ charStyleName ];
            //        if( styleId != null )
            //        {
            //          CurrentWriter.CharacterProperties.CharacterStyleId = ( ushort )( ( int )styleId );
            //        }
            //      }
            //      CharacterPropertiesConverter.FormatToCHP(symbol.CharacterFormat, CurrentWriter.CharacterProperties, m_parseAllData );

            SymbolDescriptor symbolDescriptor = new SymbolDescriptor();
            symbolDescriptor.CharCode = symbol.CharacterCode;
            symbolDescriptor.CharCodeExt = symbol.CharCodeExt;
            symbolDescriptor.FontCode = (short)CurrentWriter.StyleSheet.FontNameToIndex(symbol.FontName);

            CurrentWriter.CharacterProperties.Symbol = symbolDescriptor;
            CurrentWriter.WriteMarker(WordChunkType.Symbol);
        }

        /// <summary>
        /// Write textbox item to document;
        /// </summary>
        /// <param name="textBoxItem"></param>
        private void WriteTextBoxShape(WTextBox textBoxItem)
        {
            textBoxItem.TextBoxFormat.IsHeaderTextBox = (CurrentWriter is WordHeaderFooterWriter) ? true : false;
            CheckShapeForCloning(textBoxItem);
            TextBoxProps textBoxProps = new TextBoxProps();
            (textBoxItem as WTextBox).TextBoxSpid = CurrentWriter.InsertTextBox(textBoxItem.TextBoxFormat);
            PrepareTextBoxColl(textBoxItem);
        }

        /// <summary>
        /// Prepares the text box collection.
        /// </summary>
        /// <param name="textBoxItem">The text box item.</param>
        private void PrepareTextBoxColl(WTextBox textBoxItem)
        {
            if (m_currWriter is WordHeaderFooterWriter)
            {
                HFTextBoxCollection.Add(textBoxItem);
            }
            else
            {
                TextBoxCollection.Add(textBoxItem);
            }
        }

        /// <summary>
        /// Checks the shape for cloning. Clones shape container if escher doesn't
        /// have container for current shape item.
        /// </summary>
        /// <param name="shapeItem">The shape item.</param>
        private void CheckShapeForCloning(ParagraphItem shapeItem)
        {
            if (shapeItem.Cloned)
            {
                m_document.CloneShapeEscher(m_document, shapeItem);
            }
        }

        /// <summary>
        /// Writes the field mark and field text.
        /// </summary>
        /// <param name="fldMark">The field mark.</param>
        private void WriteFieldMarkAndText(WFieldMark fldMark)
        {
            if (CurrentField != null 
                && (CurrentField.FieldType == FieldType.FieldIndexEntry
                || CurrentField.FieldType == FieldType.FieldTOCEntry))
            {
                m_fieldStack.Pop();
                return;
            }

            if (fldMark.Type == FieldMarkType.FieldEnd)
            {
                if (CurrentField != null && CurrentField.FieldType == FieldType.FieldDocVariable)
                {
                    if (fldMark.PreviousSibling is WField && m_document.UpdateFields)
                    {
                        bool stickBefore = CurrentWriter.CharacterProperties.StickProperties;
                        CurrentWriter.CharacterProperties.StickProperties = true;
                        WriteFieldSeparator();
                        CurrentWriter.CharacterProperties.StickProperties = stickBefore;
                    }
                }
            }

            WCharacterFormat charFormat = (CurrentField is WFormField) ? CurrentField.CharacterFormat : fldMark.CharacterFormat;
            CharacterPropertiesConverter.FormatToCHP(charFormat, CurrentWriter.CharacterProperties);

            if (fldMark.Type == FieldMarkType.FieldSeparator)
            {
                WriteFieldSeparator();
            }
            else
            {
                WriteFieldEnd();
            }
        }

        /// <summary>
        /// Writes the field separator.
        /// </summary>
        private void WriteFieldSeparator()
        {
            if (CurrentField != null)
            {
                if (CurrentField.FieldType == FieldType.FieldEmbed || CurrentField.FieldType == FieldType.FieldLink)
                {
                	WriteOleObjectCharProps(CurrentField);
                }
                else if (CurrentField.FieldType == FieldType.FieldOCX)
                {
                    CurrentWriter.CharacterProperties.PicLocation = (CurrentField as WControlField).StoragePicLocation;
                }
            }
            CurrentWriter.InsertFieldSeparator();
        }

        /// <summary>
        /// Writes the field end.
        /// </summary>
        private void WriteFieldEnd()
        {
            CurrentWriter.InsertEndField();
            if (m_fieldStack.Count > 0)
            {
                m_fieldStack.Pop();
            }
            //      else
            //      {
            //        Debug.WriteLine( "Number of field start markers and field end markers is not equal.");
            //      }
        }


        /// <summary>
        /// Writes the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        private void WriteComment(WComment comment)
        {
            if (comment.AppendItems)
            {
                WriteCommItems(comment);
            }
            else
            {
                CountCommOffset(comment);
            }

            CommentCollection.Add(comment);
            WCommentFormat format = comment.Format;
            (CurrentWriter as WordWriter).InsertComment(format);
        }

        /// <summary>
        /// Writes the footnote.
        /// </summary>
        /// <param name="footnote">The footnote.</param>
        private void WriteFootnote(WFootnote footnote)
        {
            footnote.EnsureFtnMarker();
            if (footnote.FootnoteType == FootnoteType.Footnote)
            {
                FootnoteCollection.Add(footnote);
            }
            else
            {
                EndnoteCollection.Add(footnote);
            }

            UpdateCharStyleIndex(footnote.MarkerCharacterFormat.CharStyleName, false);
            CharacterPropertiesConverter.FormatToCHP(footnote.MarkerCharacterFormat, CurrentWriter.CharacterProperties);

            (CurrentWriter as WordWriter).InsertFootnote(footnote);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="watermark"></param>
        private void WriteWatermark(Watermark watermark)
        {
            SizeF pageSize = m_document.LastSection.PageSetup.PageSize;
            MarginsF margins = m_document.LastSection.PageSetup.Margins;
            float maxPictureWidth = pageSize.Width - margins.Left - margins.Right;
            CurrentWriter.InsertWatermark(watermark, UnitsConvertor.Instance, maxPictureWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toc"></param>
        private void WriteTOC(TableOfContent toc)
        {
            toc.UpdateFormattingString();
            WriteField(toc.TOCField);
        }

        /// <summary>
        /// Writes the FieldToc entry.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        private void WriteFldTocEntry(string fieldCode)
        {
            CurrentWriter.CharacterProperties.StickProperties = true;
            if (!CurrentWriter.CharacterProperties.HasOptions(WordSprmOptions.sprmCFFldVanish))
                CurrentWriter.CharacterProperties.FldVanish = true;
            CurrentWriter.WriteMarker(WordChunkType.FieldBeginMark);
            CurrentWriter.WriteSafeChunk(fieldCode);
            CurrentWriter.CharacterProperties.StickProperties = false;
            CurrentWriter.WriteMarker(WordChunkType.FieldEndMark);
        }

        /// <summary>
        /// Writes the comment mark.
        /// </summary>
        /// <param name="commMark">The comment mark.</param>
        private void WriteCommMark(WCommentMark commMark)
        {
            DictionaryEntry entry;
            if (commMark.Type == CommentMarkType.CommentStart)
            {
                entry = new DictionaryEntry((CurrentWriter as WordWriter).GetTextPos(), 0);
                if(!CommentOffsets.ContainsKey(commMark.CommentId))
                    CommentOffsets.Add(commMark.CommentId, entry);
            }
            else
            {
                if (CommentOffsets.ContainsKey(commMark.CommentId))
                {
                    entry = CommentOffsets[commMark.CommentId];
                    entry.Value = (CurrentWriter as WordWriter).GetTextPos();
                    CommentOffsets[commMark.CommentId] = entry;
                }
            }
        }
        /// <summary>
        /// Writes the OLE object.
        /// </summary>
        /// <param name="oleObject">The OLE object.</param>
        private void WriteOleObject(WOleObject oleObject)
        {
            WField oleField = oleObject.Field;
            OleObjects.Add(oleObject);
            oleField.m_fieldValue = string.Empty;
            if (!string.IsNullOrEmpty(oleObject.ObjectType))
                oleField.m_fieldValue += oleObject.ObjectType;

            if (oleField.FieldType == FieldType.FieldLink)
                oleField.m_fieldValue += " \"" + oleObject.LinkPath.Replace(@"\", @"\\") + "\"";
            WriteField(oleField);

            //if (oleObject.Cloned)
            //{
            //    // Add ole container to the object pool
            //    int storPicLocation = int.Parse(oleObject.OleStorageName);
            //    StreamsManager.AppendToObjectPool(oleObject.Container, storPicLocation, m_document);
            //    oleObject.Cloned = false;
            //}
        }
        /// <summary>
        /// Writes the object pool.
        /// </summary>
        private void WriteObjectPool()
        {
            Syncfusion.CompoundFile.DocIO.Net.CompoundFile cmpFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile();
            Syncfusion.CompoundFile.DocIO.ICompoundStorage storage = cmpFile.RootStorage.CreateStorage("ObjectPool");
            int index = 2;
            foreach (WOleObject oleObject in OleObjects)
            {
                if (Array.IndexOf(storage.Storages, "_" + oleObject.OleStorageName) == -1)
                {
                    Syncfusion.CompoundFile.DocIO.ICompoundStorage oleStorage = storage.CreateStorage("_" + oleObject.OleStorageName);
                    oleObject.UpdateGuid(cmpFile, index);
                    index++;
                    oleObject.WriteToStorage(oleStorage);
                }
            }
            OleObjects.Clear();
            foreach (OLEObject oleObject in OLEObjects)
            {
                if (Array.IndexOf(storage.Storages, oleObject.Storage.StorageName) == -1)
                {
                    Syncfusion.CompoundFile.DocIO.ICompoundStorage oleStorage = storage.CreateStorage(oleObject.Storage.StorageName);
                    oleObject.UpdateGuid(cmpFile, index);
                    index++;
                    oleObject.Storage.WriteToStorage(oleStorage);
                }
            }
            OLEObjects.Clear();
            cmpFile.Flush();
            m_mainWriter.ObjectPoolStream = new MemoryStream((cmpFile.BaseStream as MemoryStream).ToArray());
            cmpFile.Dispose();
        }

        #endregion

        #region Implementation / properties, helper methods
        /// <summary>
        /// Adds the list pictures.
        /// </summary>
        private void AddListPictures()
        {
            foreach (ListStyle listStyle in m_document.ListStyles)
            {
                if (listStyle == null || listStyle.Levels == null)
                    return;

                for (int i = 0, count = listStyle.Levels.Count; i < count; i++)
                {
                    WListLevel listLevel = listStyle.Levels[i];
                    if (listLevel != null && listLevel.PicBullet != null)
                    {
                        ListPicture.Add(listLevel.PicBullet);
                        int index = ListPicture.Count - 1;

                        listLevel.CharacterFormat.ListPictureIndex = (int)index;
                        listLevel.CharacterFormat.ListHasPicture = true;
                    }
                }
            }
        }

        /// <summary>
        /// Addings the pictures to ListPictures.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        private void AddPictures(WListFormat listFormat)
        {
            return;

            if (m_listData.ContainsKey(listFormat.CustomStyleName))
                return;

            ListStyle listStyle = listFormat.CurrentListStyle;

            if (listStyle == null || listStyle.Levels == null)
                return;

            for (int i = 0, count = listStyle.Levels.Count; i < count; i++)
            {
                WListLevel listLevel = listStyle.Levels[i];
                if (listLevel != null && listLevel.PicBullet != null)
                {
                    ListPicture.Add(listLevel.PicBullet);
                    int index = ListPicture.Count - 1;

                    listLevel.CharacterFormat.ListPictureIndex = (int)index;
                    listLevel.CharacterFormat.ListHasPicture = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraph"></param>
        private void WriteParagraphProperties(IWParagraph paragraph)
        {
            // Import paragraph properties
            WriteListProperties(paragraph.ListFormat, CurrentWriter as WordWriterBase);
            ParagraphPropertiesConverter.Import(CurrentWriter.ParagraphProperties, paragraph.ParagraphFormat, paragraph as WParagraph);
            //Sort Sprms
            CurrentWriter.ParagraphProperties.Sprms.SortSprms();
            // Update character properties for paragraph end symbol.
            CurrentWriter.BreakCharProperties.Sprms.Clear();
            UpdateCharStyleIndex(paragraph.BreakCharacterFormat.CharStyleName, true);
            CharacterPropertiesConverter.FormatToCHP(paragraph.BreakCharacterFormat, CurrentWriter.BreakCharProperties);
            WriteParagraphStyle(CurrentWriter, paragraph);

        }

        /// <summary>
        /// Writes style of paragraph to WordWriter
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="paragraph"></param>
        private void WriteParagraphStyle(IWordWriterBase writer, IWParagraph paragraph)
        {
            string styleName = paragraph.StyleName;

            if (styleName != null)
            {
                int styleIndex = writer.StyleSheet.StyleNameToIndex(styleName, false);
                if (styleIndex > -1)
                {
                    writer.CurrentStyleIndex = styleIndex;
                }
            }
            else
            {
                writer.CurrentStyleIndex = 0;
            }
        }

        /// <summary>
        /// Writes the table properties.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">The row.</param>
        /// <param name="table">The table.</param>
        /// <param name="hasTableDesc">if it has a table descriptor, set to <c>true</c>.</param>
        private void WriteTableProperties(IWordWriterBase writer, WTableRow row, IWTable table, bool hasTableDesc)
        {
            TableRowDescriptor rowDesc = null;
            if (hasTableDesc)
            {
                rowDesc = row.RowFormat.RowDescriptor;
            }
            else
            {
                row.RowFormat.CheckDefPadding();
                rowDesc = (writer as WordWriterBase).CreateTableRowDescriptor(row.Cells.Count);
                rowDesc.Table = table as WTable;
                TablePropertiesConverter.Import(rowDesc, row);
                TablePropertiesConverter.ImportCellsWidth(rowDesc, row, m_currSection);
            }
            TableBorders tableBorders = (writer as WordWriterBase).CreateTableBorders();
            CopyRowProperties(writer.ParagraphProperties, row.RowFormat);

            TablePropertiesConverter.Import(tableBorders, row.RowFormat);
            //Imports GridBefore and GridAfter properties of the table row
            ParagraphPropertiesConverter.ImportGridBeforeAfter(writer.ParagraphProperties, row);
            writer.ParagraphProperties.TableBorders = tableBorders;
            writer.ParagraphProperties.TableBordersNew = tableBorders;
            writer.ParagraphProperties.TableRowProperties = rowDesc;

            if (row.IsDeleteRevision)
                row.CharacterFormat.IsDeleteRevision = true;
            if (row.IsInsertRevision)
                row.CharacterFormat.IsInsertRevision = true;

            // Import some other table properties
            ParagraphPropertiesConverter.Import(writer.ParagraphProperties, row);
            CharacterPropertiesConverter.FormatToCHP(row.CharacterFormat, writer.CharacterProperties);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraphProperties"></param>
        /// <param name="rowFormat"></param>
        private void CopyRowProperties(ParagraphProperties paragraphProperties, RowFormat rowFormat)
        {
            //Copying the sprms to avoid table splitting problem while updating cell format
            if (rowFormat.Sprms != null && rowFormat.Sprms.Contain(0x7469))
                paragraphProperties.Sprms.Add(rowFormat.Sprms[0x7469]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void WriteStyleSheet(IWordWriter writer)
        {
            WordStyleSheet styleSheet = writer.StyleSheet;
            UpdateDefFormat();
            List<String> styleNames = new List<String>();
            List<int> FixedStyleIndex = new List<int>();
            
            Style style = null;
            for (int i = 0, cnt = m_document.Styles.Count; i < cnt; i++)
            {
                style = m_document.Styles[i] as Style;
                WParagraphStyle paraStyle = style as WParagraphStyle;
                CharacterStyle charStyle = style as CharacterStyle;

                bool isCharacter = (style.StyleType == StyleType.CharacterStyle);

                int styleIndex = 0;
                if (!styleNames.Contains(style.Name))
                    styleIndex = styleSheet.StyleNameToIndex(style.Name, isCharacter);
                else
                    styleIndex = -1;

                styleNames.Add(style.Name);
                WordStyle wordStyle;
                if (((style.StyleId > 0 && style.StyleId < 10) || style.StyleId == 105 || style.StyleId == 107)
                    && !FixedStyleIndex.Contains(style.StyleId))
                {
                    // Write heading styles
                    wordStyle = new WordStyle(styleSheet, style.Name);
                    wordStyle.ID = style.StyleId;
                    if ((style.StyleId > 0 && style.StyleId < 10) && !FixedStyleIndex.Contains(style.StyleId)) //Heading style
                    {
                        styleSheet.RemoveStyleByIndex(wordStyle.ID);
                        styleSheet.InsertStyle(wordStyle.ID, wordStyle);
                    }
                    // Write Table Normal style
                    else if (style.StyleId == 105 && !FixedStyleIndex.Contains(style.StyleId)) //TableNormal Style
                    {
                        styleSheet.RemoveStyleByIndex(11);
                        styleSheet.InsertStyle(11, wordStyle);
                    }
                    // Write No List style
                    else if (style.StyleId == 107 && !FixedStyleIndex.Contains(style.StyleId)) //NoList style
                    {
                        styleSheet.RemoveStyleByIndex(12);
                        styleSheet.InsertStyle(12, wordStyle);
                    }
                    FixedStyleIndex.Add(style.StyleId);
                }
                else if (styleIndex < 0)
                {
                    styleIndex = styleSheet.StylesCount;
                    if (styleIndex == 13)
                    {
                        // Writes the reserved style at the fixed index 13.
                        if (m_document.Styles.FixedIndex13HasStyle && (m_document.Styles.FixedIndex13StyleName != null && m_document.Styles.FixedIndex13StyleName != string.Empty))
                        {
                            wordStyle = styleSheet.CreateStyle(m_document.Styles.FixedIndex13StyleName, isCharacter);
                            wordStyle.ID = style.StyleId;
                        }
                        else
                        {
                            //If fixed index 13 has empty style
                            wordStyle = WordStyle.Empty;
                            styleSheet.InsertStyle(13, wordStyle);
                            styleIndex = styleSheet.StylesCount;
                            if (!(m_document.Styles.FixedIndex14HasStyle && (m_document.Styles.FixedIndex14StyleName != null && m_document.Styles.FixedIndex14StyleName != string.Empty)))
                            {
                                // Writes the reserved style at the fixed index 14, if reserved style (at index 14) is empty.
                                wordStyle = WordStyle.Empty;
                                styleSheet.InsertStyle(14, wordStyle);
                                styleIndex = styleSheet.StylesCount;
                                wordStyle = styleSheet.CreateStyle(style.Name, isCharacter);
                                wordStyle.ID = style.StyleId;
                            }
                            else
                                continue;
                        }
                    }
                    else if (styleIndex == 14)
                    {
                        // Writes the reserved style at the fixed index 14.
                        if (m_document.Styles.FixedIndex14HasStyle && (m_document.Styles.FixedIndex14StyleName != null && m_document.Styles.FixedIndex14StyleName != string.Empty))
                        {
                            wordStyle = styleSheet.CreateStyle(m_document.Styles.FixedIndex14StyleName, isCharacter);
                            wordStyle.ID = style.StyleId;
                            if (m_document.Styles.FixedIndex14StyleName != style.Name)
                                i--;
                        }
                        else
                        {
                            wordStyle = WordStyle.Empty;
                            styleSheet.InsertStyle(14, wordStyle);
                            styleIndex = styleSheet.StylesCount;
                            wordStyle = styleSheet.CreateStyle(style.Name, isCharacter);
                            wordStyle.ID = style.StyleId;
                        }
                    }
                    else
                    {
                        styleIndex = styleSheet.StylesCount;
                        wordStyle = styleSheet.CreateStyle(style.Name, isCharacter);
                        wordStyle.ID = style.StyleId;

                    }
                }
                else
                {
                    wordStyle = styleSheet.GetStyleByIndex(styleIndex);
                }

                if (paraStyle != null)
                {
                    UpdateListInStyle(writer, paraStyle);
                    ParagraphPropertiesConverter.Import(wordStyle.ParagraphProperties, paraStyle.ParagraphFormat, null);
                }

                // NAZAR: Fixed problem with non preserving sprmCRgFtc0 for Normal style.
                if (style.CharacterFormat.HasKey(WCharacterFormat.FontNameAsciiKey) ||
                  (style.CharacterFormat.Sprms != null && style.CharacterFormat.Sprms[WordSprmOptions.sprmCRgFtc0] != null))
                {
                    wordStyle.CharacterProperties.Sprms.RemoveValue(WordSprmOptions.sprmCRgFtc0);
                }
                CharacterPropertiesConverter.FormatToCHP(style.CharacterFormat, wordStyle.CharacterProperties);
                wordStyle.IsPrimary = style.IsPrimaryStyle;
                wordStyle.IsSemiHidden = style.IsSemiHidden;
                wordStyle.UnhideWhenUsed = style.UnhideWhenUsed;
                wordStyle.TypeCode = style.TypeCode;
                if (style.TableStyleData != null && style.TypeCode == WordStyleType.TableStyle)
                {
                    wordStyle.TableStyleData = new byte[style.TableStyleData.Length];
                    Buffer.BlockCopy(style.TableStyleData, 0, wordStyle.TableStyleData, 0, style.TableStyleData.Length);
                }
                if (charStyle != null)
                {
                    try
                    {
                        m_charStylesHash.Add(style.Name, styleIndex);
                    }
                    catch
                    {
                        //              Debug.WriteLine( "Character style with name " + style.Name + " already exists in stylesheet." );
                    }
                }
            }

            for (int index = 0, counter = m_document.Styles.Count; index < counter; index++)
            {
                style = m_document.Styles[index] as Style;

                if (string.IsNullOrEmpty(style.Name))
                    continue;

                bool isCharacter = (style.StyleType == StyleType.CharacterStyle);
                int styleIndex = styleSheet.StyleNameToIndex(style.Name, isCharacter);

                if (style.BaseStyle != null)
                {
                    int baseStyleIndex = styleSheet.StyleNameToIndex(style.BaseStyle.Name, isCharacter);
                    styleSheet.GetStyleByIndex(styleIndex).BaseStyleIndex = baseStyleIndex;
                }

                if (style.NextStyle != null)
                {
                    int nextStyleIndex = styleSheet.StyleNameToIndex(style.NextStyle, isCharacter);
                    styleSheet.GetStyleByIndex(styleIndex).NextStyleIndex = nextStyleIndex;
                }

                if (!string.IsNullOrEmpty(style.LinkStyle))
                {
                    int linkStyleIndex = styleSheet.StyleNameToIndex(style.LinkStyle);
                    styleSheet.GetStyleByIndex(styleIndex).LinkStyleIndex = linkStyleIndex;
                }
            }
        }

        /// <summary>
        /// Updates the default format.
        /// </summary>
        private void UpdateDefFormat()
        {
            WParagraphStyle style = m_document.Styles.FindByName("Normal") as WParagraphStyle;
            if (style == null)
                return;

            if (m_document.m_defParaFormat != null)
            {
                if ((style.ParagraphFormat.Sprms == null || style.ParagraphFormat.Sprms.Count == 0) && style.ParagraphFormat.IsDefault)
                {
                    style.ParagraphFormat.ImportContainer(m_document.m_defParaFormat);
                }
                else if (m_document.m_defParaFormat.Sprms != null)
                {
                    foreach (SinglePropertyModifierRecord modifier in m_document.m_defParaFormat.Sprms)
                    {
                        if (style.ParagraphFormat.Sprms[modifier.Options] == null)
                            style.ParagraphFormat.Sprms.Add(modifier);
                    }
                }
            }
            if (m_document.DefCharFormat != null && m_document.DefCharFormat.Sprms != null)
            {
                if ((style.CharacterFormat.Sprms == null || style.CharacterFormat.Sprms.Count == 0) && style.CharacterFormat.IsDefault)
                {
                    style.CharacterFormat.ImportContainer(m_document.DefCharFormat);
                }
                else
                {
                    foreach (SinglePropertyModifierRecord modifier in m_document.DefCharFormat.Sprms)
                    {
                        if (style.CharacterFormat.Sprms[modifier.Options] == null)
                            style.CharacterFormat.Sprms.Add(modifier);
                    }

                    if (style.CharacterFormat.CharStyleName == null)
                        style.CharacterFormat.CharStyleName = m_document.DefCharFormat.CharStyleName;
                    if (!style.CharacterFormat.HasKey(WCharacterFormat.FontNameAsciiKey))
                        style.CharacterFormat.FontNameAscii = m_document.DefCharFormat.FontNameAscii;
                    if (!style.CharacterFormat.HasKey(WCharacterFormat.FontNameBidiKey))
                        style.CharacterFormat.FontNameBidi = m_document.DefCharFormat.FontNameBidi;
                    if (!style.CharacterFormat.HasKey(WCharacterFormat.FontNameFarEastKey))
                        style.CharacterFormat.FontNameFarEast = m_document.DefCharFormat.FontNameFarEast;
                    if (!style.CharacterFormat.HasKey(WCharacterFormat.FontNameNonFarEastKey))
                        style.CharacterFormat.FontNameNonFarEast = m_document.DefCharFormat.FontNameNonFarEast;
                }
            }
        }

        /// <summary>
        /// Writes the document properties.
        /// </summary>
        private void WriteDocumentProperties()
        {
            if (m_document.BuiltinDocumentProperties != null)
                m_mainWriter.BuiltinDocumentProperties = m_document.BuiltinDocumentProperties.Clone();

            if (m_document.CustomDocumentProperties != null)
                m_mainWriter.CustomDocumentProperties = m_document.CustomDocumentProperties.Clone();

            m_mainWriter.WriteProtected = m_document.WriteProtected;
            m_mainWriter.HasPicture = m_document.HasPicture;

            if (m_document.MacrosData != null)
            {
                m_mainWriter.MacrosStream = new MemoryStream(m_document.MacrosData);
            }
            if (m_document.MacroCommands != null)
            {
                m_mainWriter.MacroCommands = m_document.MacroCommands;
            }
            if (OleObjects.Count > 0 || OLEObjects.Count > 0)
                WriteObjectPool();
            if (m_document.GrammarSpellingData != null)
            {
                m_mainWriter.GrammarSpellingData = m_document.GrammarSpellingData;
            }
            if (m_document.DOP != null)
            {
                m_mainWriter.DOP = m_document.DOP;
                m_mainWriter.DOP.OddAndEvenPagesHeaderFooter = m_document.DifferentOddAndEvenPages;
                m_mainWriter.DOP.DefaultTabWidth = (ushort)Math.Round(m_document.DefaultTabWidth * DLSConstants.TwipsInOnePoint);
                m_mainWriter.DOP.UpdateDateTime(m_mainWriter.BuiltinDocumentProperties);
            }
            if (m_document.AssociatedStrings != null)
            {
                m_mainWriter.AssociatedStrings = m_document.AssociatedStrings.GetAssociatedStrings();
            }
            
            m_mainWriter.StandardAsciiFont = m_document.StandardAsciiFont;
            m_mainWriter.StandardFarEastFont = m_document.StandardFarEastFont;
            m_mainWriter.StandardNonFarEastFont = m_document.StandardNonFarEastFont;
            m_mainWriter.StandardBidiFont = m_document.StandardBidiFont;
            //if (m_document.ViewSetup.DocumentViewType != DocumentViewType.PrintLayout)
            {
                m_mainWriter.DOP.ViewType = (byte)m_document.ViewSetup.DocumentViewType;
            }
            if (m_document.ViewSetup.ZoomType != ZoomType.None)
            {
                m_mainWriter.DOP.ZoomType = (byte)m_document.ViewSetup.ZoomType;
            }
            if (m_document.ViewSetup.ZoomPercent != ViewSetup.DEF_ZOOMING)
            {
                m_mainWriter.DOP.ZoomPercent = (ushort)m_document.ViewSetup.ZoomPercent;
            }
            if (m_document.Variables.Count > 0)
            {
                m_mainWriter.Variables = m_document.Variables.ToByteArray();
            }
        }

        /// <summary>
        /// Write background effect to document.
        /// </summary>
        private void WriteBackground()
        {
            Background background = m_document.Background;
            if (background.Type != BackgroundType.NoBackground)
            {
                CheckEscher();
                EscherClass escher = m_document.Escher;
                m_document.DOP.Dop2003.DispBkSpSaved = true;

                MsofbtSpContainer backContainer = new MsofbtSpContainer(m_document);
                backContainer.CreateRectangleContainer();
                MsofbtSpContainer oldBackContainer = (MsofbtSpContainer)escher.GetBackgroundContainer();
                int backgroundSpid = oldBackContainer.Shape.ShapeId;
                backContainer.UpdateBackground(m_document, background);

                //        switch( background.Type )
                //        {
                //          case BackgroundType.Texture:
                //          case BackgroundType.Picture:
                //            if( background.Picture != null )
                //            {
                //              WritePictureBackground( backContainer, oldBackContainer, background, escher );
                //            }
                //            break;
                //          case BackgroundType.Color:
                //            backContainer.UpdateFillColor( background.Color );
                //            break;
                //          case BackgroundType.Gradient:
                //            backContainer.UpdateFillGradient( background.Gradient );
                //            break;
                //        }
                //        backContainer.UpdateFillProps();

                MsofbtDgContainer dgContainer = escher.FindDgContainerForSubDocType(ShapeDocType.Main);
                dgContainer.Children.Remove(oldBackContainer);
                escher.Containers.Remove(backgroundSpid);

                dgContainer.Children.Add(backContainer);
                escher.Containers.Add(backgroundSpid, backContainer);
            }
        }

        /// <summary>
        /// Writes the picture background.
        /// </summary>
        /// <param name="backContainer">The back container.</param>
        /// <param name="oldBackContainer">The old back container.</param>
        /// <param name="background">The background.</param>
        /// <param name="escher">The escher.</param>
        private void WritePictureBackground(MsofbtSpContainer backContainer, MsofbtSpContainer oldBackContainer,
          Background background, EscherClass escher)
        {
            MsofbtBSE backgroundBse = new MsofbtBSE(m_document);
            backgroundBse.Initialize(background.ImageRecord);
            backContainer.Bse = backgroundBse;
            uint imageIndex = backContainer.GetPropertyValue((int)FOPTEFillStyle.fillBlip);
            if (imageIndex != uint.MaxValue)
            {
                escher.ModifyBStoreByPid((int)imageIndex, backgroundBse);
            }
            else
            {
                escher.m_msofbtDggContainer.BstoreContainer.Children.Add(backgroundBse);
                imageIndex = (uint)escher.m_msofbtDggContainer.BstoreContainer.Children.Count;
            }
            backContainer.UpdateFillPicture(background, (int)imageIndex);
        }

        /// <summary>
        /// Noes the background.
        /// </summary>
        /// <returns></returns>
        private void CheckEscher()
        {
            EscherClass escher = m_document.Escher;
            if ((escher != null && escher.m_dgContainers.Count == 0) || escher == null)
            {
                escher = new EscherClass(m_document);
                escher.CreateDgForSubDocuments();
                m_document.Escher = escher;
            }
            else if (escher.m_msofbtDggContainer.BstoreContainer == null)
            {
                escher.m_msofbtDggContainer.Children.Add(new MsofbtBstoreContainer(m_document));
            }
        }

        /// <summary>
        /// Writes the commented items.
        /// </summary>
        /// <param name="comment">The comment.</param>
        private void WriteCommItems(WComment comment)
        {
            int beforeTextPos = 0;

            if (comment.CommentedBodyPart != null)
            {
                // End current paragraph
                WriteParagraphProperties(comment.OwnerParagraph);
                CurrentWriter.WriteMarker(WordChunkType.ParagraphEnd);

                beforeTextPos = (CurrentWriter as WordWriter).GetTextPos();
                WriteParagraphs(comment.CommentedBodyPart.BodyItems, false);
            }
            else if (comment.CommentedItems.Count > 0)
            {
                beforeTextPos = (CurrentWriter as WordWriter).GetTextPos();
                foreach (ParagraphItem item in comment.CommentedItems)
                {
                    WriteParaItem(item, comment.OwnerParagraph);
                }
            }

            int afterTextPos = (CurrentWriter as WordWriter).GetTextPos();
            comment.Format.BookmarkStartOffset = afterTextPos - beforeTextPos;
            comment.Format.BookmarkEndOffset = 0;
        }

        /// <summary>
        /// Counts the comment offset.
        /// </summary>
        /// <param name="comment">The comment.</param>
        private void CountCommOffset(WComment comment)
        {
            if (CommentOffsets.ContainsKey(comment.Format.TagBkmk))
            {
                DictionaryEntry commOffset = CommentOffsets[comment.Format.TagBkmk];
                int start = (int)(commOffset.Key);
                int end = (int)(commOffset.Value);
                if (end != 0)
                {
                    comment.Format.BookmarkStartOffset = end - start;
                    if (comment.Format.BookmarkStartOffset == 0)
                        comment.Format.BookmarkEndOffset = 1;
                    else
                        comment.Format.BookmarkEndOffset = 0;
                }
            }
        }
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// 
        /// </summary>    
        internal void Close()
        {
            m_document = null;

            if (m_txbxItems != null)
                m_txbxItems = null;


            if (m_hfTxbxItems != null)
                m_hfTxbxItems = null;

            if (m_commentCollection != null)
                m_commentCollection = null;

            if (m_footnoteCollection != null)
                m_footnoteCollection = null;

            if (m_endnoteCollection != null)
                m_endnoteCollection = null;

            if (m_charStylesHash != null)
            {
                m_charStylesHash.Clear();
                m_charStylesHash = null;
            }

            if (m_listData != null)
            {
                m_listData.Clear();
                m_listData = null;
            }

            if (m_bookmarksAfterCell != null)
            {
                m_bookmarksAfterCell.Clear();
                m_bookmarksAfterCell = null;
            }

            if (m_fieldStack != null)
            {
                m_fieldStack.Clear();
                m_fieldStack = null;
            }

            if (m_commOffsets != null)
            {
                m_commOffsets.Clear();
                m_commOffsets = null;
            }

            if (m_listPicture != null)
            {
                m_listPicture.Clear();
                m_listPicture = null;
            }
        }

        /// <summary>
        /// Gets character style index by character style name.
        /// </summary>
        /// <param name="charStyleName">Name of the char style.</param>
        /// <returns></returns>
        private void UpdateCharStyleIndex(string charStyleName, bool isParaBreak)
        {
            if (charStyleName != null)
            {
                if (m_charStylesHash.ContainsKey(charStyleName))
                {
                    CharacterProperties charProps = null;

                    if (isParaBreak)
                        charProps = CurrentWriter.BreakCharProperties;
                    else
                        charProps = CurrentWriter.CharacterProperties;

                    charProps.CharacterStyleId = (ushort)m_charStylesHash[charStyleName];
                }
            }
        }

        /// <summary>
        /// Updates the name of the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private string UpdateFieldName(WField field)
        {
            string fieldName = string.Empty;

            if (field is WMergeField)
            {
                WMergeField mergeField = field as WMergeField;

                if (mergeField.FieldName != null)
                {
                    fieldName = mergeField.FieldName;
                }
                if (mergeField.Prefix != null && mergeField.Prefix != "")
                {
                    fieldName = mergeField.Prefix + ":" + mergeField.FieldName;
                }
            }
            return fieldName;
        }

        /// <summary>
        /// Updates the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private string UpdateFieldValue(WField field, string fieldName)
        {
            string fieldValue = string.Empty;

            if (field.FieldType != FieldType.FieldLink && field.FieldType != FieldType.FieldEmbed)
            {
                fieldValue = field.Text;
            }

            return fieldValue;
        }

        /// <summary>
        /// Updates the field code.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="fieldSwitches">The field switches.</param>
        /// <returns></returns>
        private string UpdateFieldCode(WField field, string fieldName, string fieldValue, string fieldSwitches)
        {
            string fieldCode = " " + FieldTypeDefiner.GetFieldCode(field.FieldType) + " ";

            if (field.FieldType == FieldType.FieldExpression)
            {
                fieldCode = "";
            }

            // Update hyperlink field code
            if (field.FieldType == FieldType.FieldHyperlink && fieldSwitches.Length != 0)
            {
                if (field.LocalReference != null && field.LocalReference != string.Empty)
                {
                    fieldCode += " " + field.FieldValue;
                    fieldCode += " " + fieldSwitches;
                    fieldCode += " " + field.LocalReference;
                    //field.m_fieldValue = null;
                }
                else
                {
                    fieldCode += " " + fieldSwitches;
                }
            }

            if (fieldName.Length != 0)
            {
                fieldCode += " " + fieldName;
            }

            // Adds uri if field is hyperlink
            if (field.FieldValue != null && field.FieldValue.Length != 0
                && fieldName != field.FieldValue
                && field.FieldType != FieldType.FieldMergeField)
            {
                fieldCode += " " + field.FieldValue;
            }

            // Adds swithces if exists
            if (field.FieldType != FieldType.FieldHyperlink && fieldSwitches.Length != 0)
            {
                fieldCode += " " + fieldSwitches;
            }

            if (fieldCode == null)
            {
                fieldCode = " ";
            }

            return fieldCode;
        }

        /// <summary>
        /// Writes the page break after.
        /// </summary>
        /// <param name="curPara">The current paragraph.</param>
        /// <param name="type">The break type.</param>
        private void WriteBreakAfter(WParagraph curPara, BreakType type)
        {
            WParagraph nextPara = curPara.NextSibling as WParagraph;
            if (nextPara != null)
            {
                ParagraphPropertiesConverter.Import(CurrentWriter.ParagraphProperties, nextPara.ParagraphFormat, curPara);
            }
            if (type == BreakType.PageBreak)
            {
                m_mainWriter.WriteMarker(WordChunkType.PageBreak);
            }
            else if (type == BreakType.ColumnBreak)
            {
                m_mainWriter.WriteMarker(WordChunkType.ColumnBreak);
            }
        }
        /// <summary>
        /// Determines whether the page break need to be skipped based on given entity owner.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>true, if present inside TextBox/FootNote/EndNote/Header/Footer</returns>
        private bool IsPageBreakNeedToBeSkipped(Entity entity)
        {
            Entity baseEntity = entity;
            do
            {
                if (baseEntity.Owner == null)
                    return false;
                baseEntity = baseEntity.Owner;
            }
            while (!(baseEntity is WTextBox) && !(baseEntity is WFootnote) && !(baseEntity is HeaderFooter));

            return true;
        }
        #endregion

        #region Implementation / lists
        /// <summary>
        /// Writes the list properties.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="writer">The writer.</param>
        private void WriteListProperties(WListFormat listFormat, IWordWriterBase writer)
        {
            ListType listType = listFormat.ListType;

            if (listType == ListType.NoList)
            {
                ProcessEmptyList(listFormat, writer);
            }
            else if (listFormat.CustomStyleName != string.Empty)
            {
                ProcessList(listFormat, writer);
            }
        }

        /// <summary>
        /// Processes the empty list.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="writer">The writer.</param>
        private void ProcessEmptyList(WListFormat listFormat, IWordWriterBase writer)
        {
            if (listFormat.IsListRemoved)
                writer.ParagraphProperties.RemoveListSprms();
            else if (listFormat.IsEmptyList)
                writer.ParagraphProperties.WriteEmptyList();
        }

        /// <summary>
        /// Processes the list.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="writer">The writer.</param>
        private void ProcessList(WListFormat listFormat, IWordWriterBase writer)
        {
            bool useBaseStyle = listFormat.CurrentListStyle.IsBuiltInStyle;
            if (useBaseStyle)
            {
                writer.ParagraphProperties.RemoveListSprms();
                return;
            }

            if ((m_prevStyleName != listFormat.CustomStyleName) || (listFormat.RestartNumbering))
            {
                string currStyleName = listFormat.CurrentListStyle.Name;

                if (!listFormat.RestartNumbering && AdapterListIDHolder.Instance.ContainsListName(currStyleName))
                {
                    ContinueCurrentList(writer, listFormat);
                }
                else
                {
                    ApplyStyle(writer, listFormat, useBaseStyle);
                }
            }
            else
            {
                ContinueCurrentList(writer, listFormat);
                m_prevStyleName = listFormat.CustomStyleName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="listFormat"></param>
        /// <param name="useBaseStyle"></param>
        private void ContinueCurrentList(IWordWriterBase writer, WListFormat listFormat)
        {
            ListData curListData = m_listData[listFormat.CustomStyleName];
            writer.ListProperties.ContinueCurrentList(curListData, listFormat, writer.StyleSheet);
            m_prevStyleName = listFormat.CustomStyleName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="listFormat"></param>
        /// <param name="useBaseStyle"></param>
        private void ApplyStyle(IWordWriterBase writer, WListFormat listFormat, bool useBaseStyle)
        {
            ListStyle curListStyle = m_document.ListStyles.FindByName(listFormat.CustomStyleName);
            if (curListStyle != null)
            {
                ListData curListData = CreateListData(curListStyle, writer.StyleSheet, listFormat);
                if (!m_listData.ContainsKey(listFormat.CustomStyleName))
                {
                    m_listData.Add(listFormat.CustomStyleName, curListData);
                }
                else
                {
                    //Change listData in the collection on RestartNumbering.
                    m_listData[listFormat.CustomStyleName] = curListData;
                }

                if (useBaseStyle)
                {
                    int listFormatIndex = 0;
                    if (writer.ListProperties.StyleListIndexes[listFormat.CustomStyleName] == null)
                        listFormatIndex = writer.ListProperties.ApplyBaseStyleList(curListData, listFormat, writer.StyleSheet);
                    else
                        listFormatIndex = (short)writer.ListProperties.StyleListIndexes[listFormat.CustomStyleName];

                    ModifyBaseStyles(listFormatIndex, writer);
                }

                writer.ListProperties.ApplyList(curListData, listFormat, writer.StyleSheet, true);
                m_prevStyleName = listFormat.CustomStyleName;
                listFormat.RestartNumbering = false;
            }
        }

        /// <summary>
        /// Set sprmPIlfo value in collection of  word styles
        /// </summary>
        /// <param name="listFormatIndex">Index of the list format.</param>
        /// <param name="listLevelNumber">The list level number.</param>
        /// <param name="writer">The writer.</param>
        private void ModifyBaseStyles(int listFormatIndex, IWordWriterBase writer)
        {
            int styleIndex = writer.CurrentStyleIndex;
            WordStyle curStyle = writer.StyleSheet.GetStyleByIndex(styleIndex);
            short prevLfoIndex = -1;
            prevLfoIndex = curStyle.ParagraphProperties.ListFormatIndex;
            if (prevLfoIndex != listFormatIndex && prevLfoIndex != -1)
            {
                curStyle.ParagraphProperties.ListFormatIndex = (short)listFormatIndex;
            }
        }

        /// <summary>
        /// Get list format from ListStyleCollection by lisStyleName.
        /// </summary>
        /// <param name="listStyle">ListStyleName</param>
        /// <param name="styleSheet">StyleSheet</param>
        /// <param name="lstFormat">List format</param>
        private ListData CreateListData(ListStyle listStyle, WordStyleSheet styleSheet, WListFormat lstFormat)
        {
            ListData listData = new ListData(m_listID, listStyle.IsHybrid, listStyle.IsSimple);

            AdapterListIDHolder.Instance.ListStyleIDtoName.Add(m_listID, listStyle.Name);

            ListPropertiesConverter.Import(listStyle, listData, styleSheet);
            m_listID++;
            return listData;
        }

        /// <summary>
        /// Updates the list in style.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="style">The style.</param>
        private void UpdateListInStyle(IWordWriterBase writer, WParagraphStyle style)
        {
            if (style.ListFormat.ListType == ListType.NoList || style.ListFormat.CurrentListStyle == null)
                return;

            string name = style.ListFormat.CurrentListStyle.Name;
            if (AdapterListIDHolder.Instance.ContainsListName(name))
            {
                if (writer.ListProperties.StyleListIndexes[name] != null)
                {
                    short listIndex = (short)writer.ListProperties.StyleListIndexes[name];

                    if (style.ParagraphFormat.Sprms[WordSprmOptions.sprmPIlfo] == null)
                        style.ParagraphFormat.Sprms.Add(new SinglePropertyModifierRecord(WordSprmOptions.sprmPIlfo));

                    style.ParagraphFormat.Sprms[WordSprmOptions.sprmPIlfo].ShortValue = listIndex;
                }
            }
            else
            {
                ListStyle curListStyle = style.ListFormat.CurrentListStyle;
                WListFormat listFormat = style.ListFormat;

                ListData curListData = CreateListData(curListStyle, writer.StyleSheet, listFormat);
                m_listData.Add(listFormat.CustomStyleName, curListData);
                int listIndex = writer.ListProperties.ApplyList(curListData, listFormat, writer.StyleSheet, false);

                if (style.ParagraphFormat.Sprms[WordSprmOptions.sprmPIlfo] == null)
                {
                    style.ParagraphFormat.Sprms.Add(new SinglePropertyModifierRecord(WordSprmOptions.sprmPIlfo));
                }
                style.ParagraphFormat.Sprms[WordSprmOptions.sprmPIlfo].ShortValue = (short)listIndex;
            }
        }

        /// <summary>
        /// Resets the lists.
        /// </summary>
        private void ResetLists()
        {
            m_prevStyleName = null;
            AdapterListIDHolder.Instance.ListStyleIDtoName.Clear();

            if (m_listData != null && m_listData.Count > 0)
                m_listData.Clear();
        }
        #endregion
    }
}
