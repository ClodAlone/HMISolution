#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a part of text body.
    /// </summary>
    public class TextBodyPart
    {
        #region Fields
        private WTextBody m_textPart;
        private WTextBody m_body;
        private int m_itemIndex;
        private int m_pItemIndex;
        private WCharacterFormat m_srcFormat;
        private bool m_saveFormatting;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the body items.
        /// </summary>
        /// <value>The body items.</value>
        public BodyItemCollection BodyItems
        {
            get
            {
                return (m_textPart != null) ? m_textPart.Items : null;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodyPart"/> class.
        /// </summary>
        public TextBodyPart()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodyPart"/> class.
        /// </summary>
        /// <param name="textBodySelection">The text body selection.</param>
        public TextBodyPart(TextBodySelection textBodySelection)
        {
            Copy(textBodySelection);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodyPart"/> class.
        /// </summary>
        /// <param name="textSelection">The text selection.</param>
        public TextBodyPart(TextSelection textSelection)
        {
            Copy(textSelection);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodyPart"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public TextBodyPart(WordDocument doc)
        {
            EnsureTextBody(doc);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            m_textPart.Items.Clear();
        }
        /// <summary>
        /// Copies text from selection.
        /// </summary>
        /// <param name="textSel">The text selection.</param>
        public void Copy(TextSelection textSel)
        {
            EnsureTextBody(textSel.OwnerParagraph.Document);
            WTextRange[] ranges = textSel.GetRanges();

            WParagraph para = new WParagraph(m_textPart.Document);
            m_textPart.Items.Add(para);

            for (int i = 0, len = ranges.Length; i < len; i++)
            {
                para.Items.Add(ranges[i].Clone());
            }
        }
        /// <summary>
        /// Copies text from selection.
        /// </summary>
        /// <param name="textSel">The text selection.</param>
        public void Copy(TextBodySelection textSel)
        {
            EnsureTextBody(textSel.TextBody.Document);
            int itemStart = textSel.ItemStartIndex;
            int itemEnd = textSel.ItemEndIndex;

            for (int i = itemStart; i <= itemEnd; i++)
            {
                TextBodyItem bodyItem = (TextBodyItem)textSel.TextBody.Items[i].Clone();

                if ((i == itemStart || i == itemEnd) &&
                  bodyItem.EntityType == EntityType.Paragraph)
                {
                    WParagraph para = bodyItem as WParagraph;

                    if (i == itemEnd)
                    {
                        int j = textSel.ParagraphItemEndIndex + 1;

                        while (j < para.Items.Count)
                        {
                            para.Items.InnerList.RemoveAt(j);
                        }
                    }

                    if (i == itemStart)
                    {
                        int j = textSel.ParagraphItemStartIndex;

                        while (j > 0)
                        {
                            para.Items.InnerList.RemoveAt(0);
                            j--;
                        }
                    }
                }

                m_textPart.Items.Add(bodyItem);
            }
        }
        /// <summary>
        /// Copies the specified body item.
        /// </summary>
        /// <param name="bodyItem">The body item.</param>
        /// <param name="clone">if it is to clone, set to <c>true</c>.</param>
        public void Copy(TextBodyItem bodyItem, bool clone)
        {
            if (clone)
                bodyItem = (TextBodyItem)bodyItem.Clone();

            EnsureTextBody(bodyItem.Document);

            m_textPart.Items.Add(bodyItem);
        }
        /// <summary>
        /// Copies the specified p item.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <param name="clone">if it is to clone, set to <c>true</c>.</param>
        public void Copy(ParagraphItem pItem, bool clone)
        {
            if (clone)
                pItem = (ParagraphItem)pItem.Clone();

            EnsureTextBody(pItem.Document);
            m_textPart.AddParagraph().Items.Add(pItem);
        }
        /// <summary>
        /// Copies the specified text body.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="clone">if it is to clone, set to <c>true</c>.</param>
        internal void Copy(WTextBody textBody, bool clone)
        {
            EnsureTextBody(textBody.Document);

            if (clone)
            {
                m_textPart = (WTextBody)textBody.Clone();
            }
            else
            {
                m_textPart = textBody;
            }
        }
        /// <summary>
        /// Pastes after specified item.
        /// </summary>
        /// <param name="bodyItem">The body item.</param>
        public void PasteAfter(TextBodyItem bodyItem)
        {
            int itemIndex = bodyItem.GetIndexInOwnerCollection();
            PasteAt(bodyItem.OwnerTextBody, itemIndex + 1);
        }
        /// <summary>
        /// Pastes after specified paragraph item.
        /// </summary>
        /// <param name="paragraphItem">The paragraph item.</param>
        public void PasteAfter(ParagraphItem paragraphItem)
        {
            TextBodyItem bodyItem = paragraphItem.Owner as TextBodyItem;
            int itemIndex = bodyItem.GetIndexInOwnerCollection();
            int pItemIndex = paragraphItem.GetIndexInOwnerCollection();

            PasteAt(bodyItem.OwnerTextBody, itemIndex, pItemIndex + 1);
        }
        /// <summary>
        /// Pastes at specified position.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="itemIndex">Index of the item.</param>
        public void PasteAt(ITextBody textBody, int itemIndex)
        {
            PasteAt(textBody, itemIndex, 0);
        }
        /// <summary>
        /// Pastes at specified position with character formatting.
        /// </summary>
        /// <param name="textBody">>The text body.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="pItemIndex">Index of the p item.</param>
        /// <param name="format">Replace character format</param>
        /// <param name="applyFormat">if it specifies to apply the format, set to <c>true</c>.</param>
        internal void PasteAt(ITextBody textBody, int itemIndex, int pItemIndex, WCharacterFormat srcFormat, bool saveFormatting)
        {
            m_srcFormat = srcFormat;
            m_saveFormatting = saveFormatting;
            PasteAt(textBody, itemIndex, pItemIndex);
        }
        /// <summary>
        /// Pastes at specified position.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="pItemIndex">Index of the p item.</param>
        public void PasteAt(ITextBody textBody, int itemIndex, int pItemIndex)
        {
            if (m_textPart.Items.Count == 0)
                return;

            m_body = textBody as WTextBody;
            m_itemIndex = itemIndex;
            m_pItemIndex = pItemIndex;

            // Validate arguments
            ValidateArgs();

            WParagraph srcFirstPara = m_textPart.Items[0] as WParagraph;
            WParagraph srcLastPara = m_textPart.Items[m_textPart.Count - 1] as WParagraph;
            WParagraph trgFirstPara = itemIndex < m_body.Items.Count ?
              m_body.Items[itemIndex] as WParagraph :
              null;
            WParagraph trgLastPara = null;
            int trgOffset = 0;
            int srcStartIndex = 0;
            int srcEndIndex = m_textPart.Items.Count - 1;
            bool multiLine = (srcEndIndex > 0) || (srcFirstPara == null);

            // 1. Splits start paragraph:
            //   1.a Inserts "srcLastPara" as "trgLastPara" (or create "trgLastPara");
            //   1.b Moves p. items from "trgFirstPara" to end of "trgLastPara";
            //if( multiLine && trgFirstPara != null && m_pItemIndex > 0 )
            if (multiLine && trgFirstPara != null && m_pItemIndex >= 0)
            {
                trgLastPara = SplitParagraph(trgFirstPara, srcLastPara);
                trgOffset = 1;

                if (srcLastPara != null)
                    srcEndIndex -= 1;
            }
            if (srcFirstPara!= null && trgFirstPara != null && !m_saveFormatting)
            {
                trgFirstPara.ParagraphFormat.ClearFormatting();
                trgFirstPara.ParagraphFormat.ImportContainer(srcFirstPara.ParagraphFormat);
            }
            if (srcFirstPara != null && trgFirstPara != null && srcFirstPara.ListFormat.ListType == ListType.NoList)
            {
                int startoff = trgFirstPara.Items.Count - m_pItemIndex;
                int ioff = trgFirstPara.Items.Count - startoff;
                // 2. Inserts p. items from "srcFirstPara" to "trgFirstPara".
                for (int i = 0, len = srcFirstPara.Items.Count; i < len; i++)
                {
                    Entity pItem = srcFirstPara.Items[i].Clone();
                    if (pItem is WTextRange && m_saveFormatting && m_srcFormat != null)
                        (pItem as WTextRange).CharacterFormat.ImportContainer(m_srcFormat);

                    trgFirstPara.Items.Insert(ioff, pItem);
                    ioff = trgFirstPara.Items.Count - startoff;
                }

                if (srcFirstPara.Items.Count == 1 && !m_saveFormatting && !string.IsNullOrEmpty(srcFirstPara.StyleName)) 
                {
                    trgFirstPara.ApplyStyle(srcFirstPara.ParaStyle);
                    trgFirstPara.BreakCharacterFormat.ImportContainer(srcFirstPara.BreakCharacterFormat);
                }

                //if (srcFirstPara.ListFormat.ListType != ListType.NoList)
                //    trgFirstPara.ListFormat.ImportContainer(srcFirstPara.ListFormat);

                srcStartIndex = 1;
                trgOffset = 1;
            }
            else if (srcFirstPara != null && trgFirstPara != null && srcFirstPara.ListFormat.ListType != ListType.NoList)
            {
                WParagraph trgParaClone = trgFirstPara.Clone() as WParagraph;
                trgFirstPara = srcFirstPara.Clone() as WParagraph;                
                int ioff = 0;
                if (srcFirstPara.Items.Count == 1 && !m_saveFormatting && !string.IsNullOrEmpty(srcFirstPara.StyleName))
                {
                    trgFirstPara.ApplyStyle(srcFirstPara.ParaStyle);
                    trgFirstPara.BreakCharacterFormat.ImportContainer(srcFirstPara.BreakCharacterFormat);
                }
                m_body.Items.RemoveAt(itemIndex);
                ApplySrcFormat(trgFirstPara as WParagraph);
                // 2. Inserts p. items from "trgParaClone" to "trgFirstPara".
                for (int i = 0, len = trgParaClone.Items.Count; i < len; i++)
                {
                    Entity pItem = trgParaClone.Items[i].Clone();

                    trgFirstPara.Items.Insert(ioff, pItem);
                    ioff += 1;
                }
                m_body.Items.Insert(itemIndex, trgFirstPara);
                srcStartIndex = 1;
                trgOffset = 1;
            }
            // 3. Inserts paragraphs of TextBodyPart ( from "srcStartIndex" to 
            //    "srcEndIndex" ) at "itemIndex + trgStartOffset".
            itemIndex += (trgOffset - srcStartIndex);

            for (int i = srcStartIndex, len = srcEndIndex; i <= len; i++)
            {
                Entity bItem = m_textPart.Items[i].Clone();
                if (bItem is WParagraph)
                    ApplySrcFormat(bItem as WParagraph);
                else if (bItem is WTable)
                    ApplySrcFormat(bItem as WTable);
                m_body.Items.Insert(itemIndex + i, bItem);
            }

            // If the table is inserted into empty paragraph then insert bookmark start into the 
            //    first table cell and remove previous empty paragraph
            if (itemIndex > 0 && m_body.Items[itemIndex - 1] is WParagraph
              && (m_body.Items[itemIndex - 1] as WParagraph).Items.Count == 1
              && (m_body.Items[itemIndex - 1] as WParagraph).Items[0] is BookmarkStart
              && m_textPart.Items[0] is WTable)
            {
                WParagraph prevPara = m_body.Items[itemIndex - 1] as WParagraph;
                WTable table = (m_body.Items[itemIndex] as WTable);
                WParagraph nextPara;
                string bkmkName = (prevPara.Items[0].Clone() as BookmarkStart).Name;
                WordDocument doc = m_body.Document;

                doc.Bookmarks.Remove(doc.Bookmarks[bkmkName]);
                prevPara.RemoveSelf();

                // Append bookmark start to the paragraph
                if (table.FirstRow != null && table.FirstRow.Cells.Count > 0)
                {
                    if (table.FirstRow.Cells[0].Items.Count == 0)
                    {
                        table.FirstRow.Cells[0].Items.Add(new WParagraph(doc));
                    }
                    if (table.FirstRow.Cells[0].Items[0] is WParagraph)
                    {
                        (table.FirstRow.Cells[0].Items[0] as WParagraph).Items.Insert(0, new BookmarkStart(doc, bkmkName));
                    }
                }

                if (m_textPart.Items.Count == 1)
                {
                    nextPara = m_body.Items[itemIndex] as WParagraph;
                    nextPara.Items.Insert(0, new BookmarkEnd(doc, bkmkName));
                }
                else
                {
                    nextPara = m_body.Items[itemIndex + m_textPart.Items.Count - 2] as WParagraph;

                    if (nextPara == null)
                    {
                        nextPara = new WParagraph(doc);
                        m_body.Items.Add(nextPara);
                    }

                    nextPara.Items.Add(new BookmarkEnd(doc, bkmkName));
                }
            }
        }
        /// <summary>
        /// Pastes at end of textbody.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        public void PasteAtEnd(ITextBody textBody)
        {
            PasteAt(textBody, ((WTextBody)textBody).Items.Count);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Splits the paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="nextpItemIndex">Index of the next paragraph item.</param>
        /// <param name="paraToInsert">The paragraph to insert.</param>
        internal static void SplitParagraph(WParagraph para, int nextpItemIndex, WParagraph paraToInsert)
        {
            int paraIndex = para.GetIndexInOwnerCollection();
            para.OwnerTextBody.Items.Insert(paraIndex + 1, paraToInsert);
            //Copy formattings from source paragraph
            paraToInsert.ParagraphFormat.ImportContainer(para.ParagraphFormat);
            paraToInsert.BreakCharacterFormat.ImportContainer(para.BreakCharacterFormat);
            while (para.Items.Count > nextpItemIndex)
            {
                paraToInsert.Items.Add(para.Items[nextpItemIndex]);
            }
        }
        /// <summary>
        /// Validates the args.
        /// </summary>
        private void ValidateArgs()
        {
            if (m_body == null)
                throw new ArgumentNullException("textBody");

            if (m_itemIndex < 0 || m_itemIndex > m_body.Items.Count)
                throw new ArgumentOutOfRangeException("itemIndex", "itemIndex is less than 0 or greater than " + m_body.Items.Count);

            TextBodyItem item = (m_body.Items.Count > m_itemIndex) ? m_body.Items[m_itemIndex] : null;
            WParagraph para = item as WParagraph;

            if (para != null)
            {
                if (m_pItemIndex < 0 || m_pItemIndex > para.Items.Count)
                    throw new ArgumentOutOfRangeException("pItemIndex", "pItemIndex is less than 0 or greater than  " + para.Items.Count);
            }
        }
        /// <summary>
        /// Splits the paragraph.
        /// </summary>
        /// <param name="trgFirstPara">The TRG first paragraph.</param>
        /// <param name="srcLastPara">The SRC last paragraph.</param>
        /// <returns></returns>
        private WParagraph SplitParagraph(WParagraph trgFirstPara, WParagraph srcLastPara)
        {
            WParagraph trgLastPara;

            // 0. Check "srcLastPara" and create "trgLastPara".
            if (srcLastPara != null)
            {
                trgLastPara = (WParagraph)srcLastPara.Clone();
                ApplySrcFormat(trgLastPara);
                if (trgLastPara.ParaStyle != null && trgFirstPara.ParaStyle != null)
                    (trgLastPara.ParaStyle as Style).SetStyleName(trgFirstPara.ParaStyle.Name);
            }
            else
            {
                trgLastPara = new WParagraph(m_body.Document);
            }

            // 1. Inserts "trgLastPara" after "trgFirstPara".
            if (trgFirstPara.Items.Count > m_pItemIndex || srcLastPara != null)
                m_body.Items.Insert(m_itemIndex + 1, trgLastPara);

            // 2. Moves second part of p. items from "trgFirstPara" to "trgLastPara".
            while (trgFirstPara.Items.Count > m_pItemIndex)
            {
                trgLastPara.Items.Add(trgFirstPara.Items[m_pItemIndex]);
            }

            return trgLastPara;
        }
        /// <summary>
        /// Ensures the text body.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void EnsureTextBody(WordDocument doc)
        {
            if (m_textPart != null && m_textPart.Document == doc)
            {
                Clear();
            }
            else
            {
                m_textPart = new WTextBody(doc, null);
            }
        }
        /// <summary>
        /// Applies the source format.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void ApplySrcFormat(WParagraph para)
        {
            if (!m_saveFormatting || m_srcFormat == null || para == null)
                return;

            foreach (ParagraphItem item in para.Items)
                if (item is WTextRange)
                {
                    item.ParaItemCharFormat.ImportContainer(m_srcFormat);
                    item.ParaItemCharFormat.CopyProperties(m_srcFormat);
                }
        }
        /// <summary>
        /// Applies the source format.
        /// </summary>
        /// <param name="table">The Table.</param>
        private void ApplySrcFormat(WTable table)
        {
            if (!m_saveFormatting || m_srcFormat == null || table == null)
                return;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                {
                    for (int k = 0; k < table.Rows[i].Cells[j].ChildEntities.Count; k++)
                    {
                        if (table.Rows[i].Cells[j].ChildEntities[k] is WParagraph)
                            ApplySrcFormat(table.Rows[i].Cells[j].ChildEntities[k] as WParagraph);
                        else if (table.Rows[i].Cells[j].ChildEntities[k] is WTable)
                            ApplySrcFormat(table.Rows[i].Cells[j].ChildEntities[k] as WTable);
                    }
                }
            }
        }
        #endregion
    }
}
