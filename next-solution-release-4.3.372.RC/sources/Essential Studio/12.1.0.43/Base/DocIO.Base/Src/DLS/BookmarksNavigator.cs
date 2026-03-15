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
using System.Text;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Helper class for navigations in document bookmarks and editing bookmarks content.
    /// </summary>
    public class BookmarksNavigator
    {
        #region Class constants
        private const string c_DocumentPropertyNotInitialized = "You can not use DocumentNavigator without initializing Document property";
        private const string c_NotFoundSpecifiedBookmark = "Specified bookmark not found";
        private const string c_NotEqualDocumentProperty = " Document property must be equal this Document property";
        private const string c_CurrBookmarkNull = "Current Bookmark didn't select";
        private const string c_NotSupportGettingContent = "Not supported getting content between bookmarks in different paragraphs";
        private const string c_NotSupportDeletingContent = "Not supported deleting content between bookmarks in different paragraphs";
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private WordDocument m_document;
        private int m_currParagraphItemIndex = 0;
        private IWParagraph m_currParagraph = null;
        private Bookmark m_currBookmark = null;
        private bool m_isStart;
        private bool m_isAfter;
        private IParagraphItem m_currBookmarkItem = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets Document that this object is attached to.
        /// </summary>
        /// <value>The document.</value>
        public IWordDocument Document
        {
            get
            {
                return m_document;
            }
            set
            {
                m_document = (WordDocument)value;
            }
        }
        /// <summary>
        /// Gets the current bookmark.
        /// </summary>
        /// <value>The current bookmark.</value>
        public Bookmark CurrentBookmark
        {
            get
            {
                return m_currBookmark;
            }
        }
        #endregion

        #region Class helper properties
        /// <summary>
        /// Returns the current bookmarkitem
        /// </summary>
        public IParagraphItem CurrentBookmarkItem
        {
            get
            {
                m_currBookmarkItem = (m_isStart || m_currBookmark.BookmarkEnd == null)
                   ? (IParagraphItem)m_currBookmark.BookmarkStart
                   : (IParagraphItem)m_currBookmark.BookmarkEnd;
                return m_currBookmarkItem;
            }
        }
        /// <summary>
        /// Return the index to insert the item into the paragragh 
        /// </summary>
        private int CurrentParagraphItemIndex
        {
            get
            {
                if ((m_currBookmark != null) && (CurrentBookmarkItem.OwnerParagraph != null))
                {
                    if (m_isAfter)
                    {
                        m_currParagraphItemIndex = m_currBookmarkItem.OwnerParagraph.Items.IndexOf(m_currBookmarkItem) + 1;
                    }
                    else
                    {
                        m_currParagraphItemIndex = m_currBookmarkItem.OwnerParagraph.Items.IndexOf(m_currBookmarkItem);
                    }
                    return m_currParagraphItemIndex;
                }
                else
                {
                    throw new ArgumentException(c_NotFoundSpecifiedBookmark);
                }
            }
        }
        #endregion

        #region Class initialize / finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="T:BookmarksNavigator"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public BookmarksNavigator(IWordDocument doc)
        {
            m_document = (WordDocument)doc;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Moves the cursor to specified bookmark.
        /// </summary>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        public void MoveToBookmark(string bookmarkName)
        {
            MoveToBookmark(bookmarkName, false, false);
        }
        /// <summary>
        /// Moves the cursor to specified bookmark.
        /// </summary>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <param name="isStart">When true, moves the cursor to the beginning of the bookmark. When false, moves the cursor to the end of the bookmark</param>
        /// <param name="isAfter">When true, moves the cursor to be after the bookmark start or end position. When false, moves the cursor to be before the bookmark start or end position. </param>
        public void MoveToBookmark(string bookmarkName, bool isStart, bool isAfter)
        {
            m_isStart = isStart;
            m_isAfter = isAfter;
            String bookmarkName1 = bookmarkName.Replace('-', '_');
            if (m_document == null)
                throw new InvalidOperationException(c_DocumentPropertyNotInitialized);

            m_currBookmark = m_document.Bookmarks.FindByName(bookmarkName1);
            if (m_currBookmark != null)
            {
                IParagraphItem bkmkItem = (isStart || m_currBookmark.BookmarkEnd == null)
                  ? (IParagraphItem)m_currBookmark.BookmarkStart
                  : (IParagraphItem)m_currBookmark.BookmarkEnd;

                m_currParagraph = bkmkItem.OwnerParagraph;


            }
            else
            {
                //m_currParagraphItemIndex = -1;
                //m_currParagraph = null;
                throw new ArgumentException(c_NotFoundSpecifiedBookmark);
            }
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IWTextRange InsertText(string text)
        {
            return InsertText(text, true);
        }
        /// <summary>
        /// Inserts the text range to current position.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="saveFormatting">if it is save formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public IWTextRange InsertText(string text, bool saveFormatting)
        {
            return InsertText(text, saveFormatting, false);
        }
        /// <summary>
        /// Inserts the text range to current position
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="saveFormatting">if it is save formatting, set to <c>true</c>.</param>
        /// <param name="isReplaceContent">if it is Replaced content, set to <c>true</c>.</param>
        /// <returns></returns>
        private IWTextRange InsertText(string text, bool saveFormatting, bool isReplaceContent)
        {
            CheckCurrentState();
            IWTextRange txtRange;
            IWTextRange previousItem = null;
            if (saveFormatting)
            {
                if (m_isStart)
                {
                    previousItem = m_currBookmark.BookmarkStart.PreviousSibling as IWTextRange;
                }
                else
                {
                    previousItem = m_currBookmark.BookmarkEnd.PreviousSibling as IWTextRange;
                }
            }
            txtRange = InsertParagraphItem(ParagraphItemType.TextRange) as IWTextRange;
            txtRange.Text = text;
            if (saveFormatting)
            {
                WCharacterFormat charFormat = null;
                if (previousItem != null)
                {
                    charFormat = previousItem.CharacterFormat;
                    txtRange.CharacterFormat.ImportContainer(charFormat);
                    if (isReplaceContent)
                        txtRange.OwnerParagraph.ChildEntities.Remove(previousItem);
                }
                else
                {
                    ApplyParaFormatting(txtRange);
                }
            }
            return txtRange;
        }
        /// <summary>
        /// Inserts the table.
        /// </summary>
        /// <param name="table">The table.</param>
        public void InsertTable(IWTable table)
        {
            InsertBodyItem(table as TextBodyItem);
        }
        /// <summary>
        /// Inserts the paragraph item to current position.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <returns></returns>
        public IParagraphItem InsertParagraphItem(ParagraphItemType itemType)
        {
            IParagraphItem item = m_document.CreateParagraphItem(itemType);
            m_currParagraph.Items.Insert(CurrentParagraphItemIndex, item);
            return item;
        }
        /// <summary>
        /// Inserts the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        public void InsertParagraph(IWParagraph paragraph)
        {
            InsertBodyItem(paragraph as TextBodyItem);
        }
        /// <summary>
        /// Inserts the text body part.
        /// </summary>
        /// <param name="bodyPart">The text body part.</param>
        public void InsertTextBodyPart(TextBodyPart bodyPart)
        {
            if (CurrentBookmarkItem == null)
            {
                return;
            }
            TextBodyItem bodyItem = m_currBookmarkItem.Owner as TextBodyItem;
            int itemIndex = bodyItem.GetIndexInOwnerCollection();
            int pItemIndex = (m_currBookmarkItem as ParagraphItem).GetIndexInOwnerCollection();
            bodyPart.PasteAt(bodyItem.OwnerTextBody, itemIndex, CurrentParagraphItemIndex);
        }
        /// <summary>
        /// Gets the bookmark content2.
        /// </summary>
        /// <returns></returns>
        public TextBodyPart GetBookmarkContent()
        {
            CheckCurrentState();
            BookmarkStart bkmkStart = m_currBookmark.BookmarkStart;
            BookmarkEnd bkmkEnd = m_currBookmark.BookmarkEnd;

            TextBodySelection textSel = new TextBodySelection(bkmkStart, bkmkEnd);
            textSel.ParagraphItemStartIndex++;
            textSel.ParagraphItemEndIndex--;

            return new TextBodyPart(textSel);
        }
        /// <summary>
        /// Deletes the bookmark content.
        /// </summary>
        /// <param name="saveFormatting">if its save formatting, set to <c>true</c>.</param>
        public void DeleteBookmarkContent(bool saveFormatting)
        {
            m_isAfter = false;
            m_isStart = false;
            DeleteBookmarkContent(saveFormatting, false);
        }
        /// <summary>
        /// Deletes the bookmark content.
        /// </summary>
        /// <param name="saveFormatting">if it is save formatting, set to <c>true</c>.</param>
        /// <param name="removeEmptyParagraph">if it removes paragraph with 
        /// bookmark start and end if it is empty after deletion,  set to <c>true</c>.</param>
        [Obsolete("This method will be removed in future version. As a work around to remove bookmarked paragraph, utilize current bookmark property of bookmark navigator to access the current bookmarked paragraph and then remove its index from its owner (Text Body) collection.", false)]
        public void DeleteBookmarkContent(bool saveFormatting, bool removeEmptyParagraph)
        {
            if (CurrentBookmark == null)
                throw new InvalidOperationException();

            BookmarkStart bkmkStart = CurrentBookmark.BookmarkStart;
            BookmarkEnd bkmkEnd = CurrentBookmark.BookmarkEnd;

            if (bkmkEnd != null)
            {
                WParagraph paraStart = bkmkStart.OwnerParagraph;
                WParagraph paraEnd = bkmkEnd.OwnerParagraph;

                if (paraStart.Owner != paraEnd.Owner)
                {
                    throw new NotSupportedException(c_NotSupportDeletingContent);
                }
                else
                {
                    WTextBody textBody = (WTextBody)paraStart.Owner;
                    BodyItemCollection bodyItems = textBody.Items;
                    int paraNextIndex = paraStart.GetIndexInOwnerCollection() + 1;

                    if (paraStart != paraEnd)
                    {
                        // Removes internal body items.
                        while (bodyItems.Count > paraNextIndex && bodyItems[paraNextIndex] != paraEnd)
                        {
                            bodyItems.RemoveAt(paraNextIndex);
                        }
                    }

                    ParagraphItemCollection items = paraStart.Items;
                    int pItemNextIndex = bkmkStart.GetIndexInOwnerCollection() + 1;

                    // Saves one text range with formatting if need.
                    if (saveFormatting && items.Count > pItemNextIndex)
                    {
                        WTextRange tr = items[pItemNextIndex] as WTextRange;

                        if (tr != null)
                        {
                            //Add Empty TextRange to the paragraph with same formattings of the textrange which follows BookMarkStart
                            //Handled this case to apply same formattings to the replaced content.
                            WTextRange emptyTextRange = new WTextRange(Document);
                            emptyTextRange.CharacterFormat.ImportContainer(tr.CharacterFormat);
                            paraStart.ChildEntities.Insert(pItemNextIndex, emptyTextRange);
                            pItemNextIndex++;
                        }
                    }

                    // Removes paragraph. items after start bookmark.
                    while (items.Count > pItemNextIndex && items[pItemNextIndex] != bkmkEnd)
                    {
                        items.RemoveAt(pItemNextIndex);
                        if (m_currParagraphItemIndex > 0)
                            m_currParagraphItemIndex--;
                    }

                    if (paraStart != paraEnd)
                    {
                        int bkmkEndIndex = bkmkEnd.GetIndexInOwnerCollection();

                        // Adds items after end bookmark to start paragraph.
                        if (bkmkEndIndex != 0)
                        {
                            while (bkmkEndIndex < paraEnd.Items.Count)
                            {
                                Entity pItem = paraEnd.Items[bkmkEndIndex];
                                paraStart.Items.Add(pItem);
                            }

                            // Removes end paragraph.
                            paraEnd.RemoveSelf();
                        }
                        else
                        {
                            if (removeEmptyParagraph)
                            {
                                if (pItemNextIndex == 1)
                                {
                                    paraStart.RemoveSelf();
                                    paraEnd.Items.RemoveAt(bkmkEndIndex);
                                }

                                if (pItemNextIndex == 2 && items[0] is BookmarkEnd)
                                {
                                    paraEnd.Items.RemoveAt(bkmkEndIndex);
                                    BookmarkEnd tmpBkmkEndItem = items[0] as BookmarkEnd;
                                    ((WParagraph)bodyItems[paraNextIndex]).Items.Insert(0, tmpBkmkEndItem);
                                    paraStart.RemoveSelf();
                                }
                            }
                        }
                    }
                    //else if( removeEmptyParagraph)
                    //{
                    //   Bookmark start and bookmark end are in the same paragraphs          
                    //  if(  paraStart != null && paraStart.Items.Count == 2 &&
                    //    paraStart.Items[ 0 ] is BookmarkStart && paraStart.Items[ 1 ] is BookmarkEnd )
                    //  {
                    //    paraStart.RemoveSelf();
                    //  }
                    //}
                }
            }
            if (m_document.Bookmarks.InnerList.Contains(CurrentBookmark))
                MoveToBookmark(CurrentBookmark.Name, m_isStart, m_isAfter);
        }
        /// <summary>
        /// Replaces the bookmark content2.
        /// </summary>
        /// <param name="bodyPart">The body part.</param>
        public void ReplaceBookmarkContent(TextBodyPart bodyPart)
        {
            m_isStart = false;
            m_isAfter = false;
            DeleteBookmarkContent(false, false);
            InsertTextBodyPart(bodyPart);
        }
        /// <summary>
        /// Replaces the content of the bookmark.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="saveFormatting">if it is save formatting, set to <c>true</c>.</param>
        public void ReplaceBookmarkContent(string text, bool saveFormatting)
        {
            //m_isAfter and m_isStart is set to false to set the cursor in between the bookmarkstart and bookmarkend            
            m_isAfter = false;
            m_isStart = false;
            DeleteBookmarkContent(saveFormatting, false);
            InsertText(text, saveFormatting, true);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Checks the current state of navigator.
        /// </summary>
        private void CheckCurrentState()
        {
            if (m_document == null)
                throw new InvalidOperationException(c_DocumentPropertyNotInitialized);

            if (m_currBookmark == null || m_currParagraph == null || m_currParagraphItemIndex < 0)
                throw new InvalidOperationException(c_CurrBookmarkNull);
        }
        /// <summary>
        /// Applies character formatting of the paragraph on text range.
        /// </summary>
        /// <param name="textRange"></param>
        private void ApplyParaFormatting(IWTextRange textRange)
        {
            WParagraph curPara = m_currBookmark.BookmarkStart.OwnerParagraph;
            if (curPara == null)
            {
                return;
            }

            // If table cell is empty we use formationg from cell
            if (curPara.OwnerTextBody != null &&
              curPara.OwnerTextBody.EntityType == EntityType.TableCell &&
              curPara.OwnerTextBody.Paragraphs.Count == 1)
            {
                WTableCell cell = curPara.OwnerTextBody as WTableCell;
                textRange.CharacterFormat.ImportContainer(cell.CharacterFormat);
            }
            else
            {
                textRange.CharacterFormat.ImportContainer(curPara.BreakCharacterFormat);
            }
        }
        /// <summary>
        /// Inserts the body item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void InsertBodyItem(TextBodyItem item)
        {
            if (CurrentBookmarkItem == null)
                return;
            WParagraph para = m_currBookmarkItem.OwnerParagraph;
            int indexToInsert = para.GetIndexInOwnerCollection();
            WParagraph splitParagraph = new WParagraph(para.Document);
            if (CurrentParagraphItemIndex != 0)
            {
                if (m_currParagraphItemIndex < para.Items.Count)
                {
                    TextBodyPart.SplitParagraph(para, m_currParagraphItemIndex, new WParagraph(para.Document));
                }
                else
                {
                    if ((item is WTable && para.NextSibling is WParagraph) || (item is WParagraph))
                    {
                        para.OwnerTextBody.Items.Insert(indexToInsert + 1, splitParagraph);
                        //Copy formattings from source paragraph
                        splitParagraph.BreakCharacterFormat.ImportContainer(para.BreakCharacterFormat);
                        splitParagraph.ParagraphFormat.ImportContainer(para.ParagraphFormat);
                    }
                }
                indexToInsert++;
                if (item is WParagraph)
                {
                    //As per MSWord behavior, Append Paragraph items to the OwnerParagraph of the current bookmark
                    while ((item as WParagraph).Items.Count > 0)
                    {
                        para.Items.Add((item as WParagraph).Items[0]);
                    }
                    //Copy formattings from source paragraph
                    para.ParagraphFormat.ImportContainer((item as WParagraph).ParagraphFormat);
                    para.BreakCharacterFormat.ImportContainer((item as WParagraph).BreakCharacterFormat);
                }
            }
            if (item is WTable || m_currParagraphItemIndex <= 0)
                para.OwnerTextBody.Items.Insert(indexToInsert, item);
        }
        #endregion
    }
}
