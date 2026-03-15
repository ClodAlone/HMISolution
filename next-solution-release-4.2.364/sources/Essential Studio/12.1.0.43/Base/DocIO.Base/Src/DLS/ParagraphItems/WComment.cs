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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a container for text of a comment. 
    /// </summary>
    public class WComment : ParagraphItem, ICompositeEntity
    {
        #region Fields
        protected WTextBody m_textBody;
        protected WCommentFormat m_format;
        private ParagraphItemCollection m_commItems;
        private TextBodyPart m_bodyPart;
        private bool m_appendItems;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_textBody.ChildEntities;
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
                return EntityType.Comment;
            }
        }
        /// <summary>
        /// Gets comment body.
        /// </summary>
        /// <value>The text body.</value>
        public WTextBody TextBody
        {
            get
            {
                return m_textBody;
            }
        }
        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public WCommentFormat Format
        {
            get
            {
                return m_format;
            }
        }
        /// <summary>
        /// Gets the range of commented items.
        /// </summary>
        /// <value>The commented range.</value>
        public ParagraphItemCollection CommentedItems
        {
            get
            {
                if (m_commItems == null)
                {
                    m_commItems = new ParagraphItemCollection(m_doc);
                }
                return m_commItems;
            }
        }
        /// <summary>
        /// Gets a value indicating whether to append commented items to the document .
        /// </summary>
        /// <value><c>true</c> if append items; otherwise, <c>false</c>.</value>
        internal bool AppendItems
        {
            get
            {
                return m_appendItems;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextBodyPart CommentedBodyPart
        {
            get
            {
                return m_bodyPart;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WComment"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WComment(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_format = new WCommentFormat();
            m_textBody = new WTextBody(Document, this);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WComment comm = (WComment)base.CloneImpl();
            comm.m_format = (WCommentFormat)Format.Clone(Document);
            comm.m_textBody = (WTextBody)TextBody.Clone();
            m_commItems = null;
            m_bodyPart = null;
            return comm;
        }
        /// <summary>
        /// Removes the commented items.
        /// </summary>
        public void RemoveCommentedItems()
        {
            if (m_commItems == null || m_commItems.Count == 0)
                return;

            if (m_appendItems)
            {
                m_commItems.Clear();
                m_bodyPart = null;
                return;
            }

            ParagraphItem firstItem = m_commItems.FirstItem as ParagraphItem;
            ParagraphItem lastItem = m_commItems.LastItem as ParagraphItem;
            RemoveItemsBetween(firstItem, lastItem);

            this.Format.BookmarkStartOffset = 0;
            this.Format.BookmarkEndOffset = 1;
            m_commItems.Clear();
            m_appendItems = false;
        }
        /// <summary>
        /// Removes the items between.
        /// </summary>
        /// <param name="firstItem">The first item.</param>
        /// <param name="lastItem">The last item.</param>
        internal void RemoveItemsBetween(ParagraphItem firstItem, ParagraphItem lastItem)
        {
            if (firstItem.PreviousSibling != null && firstItem.PreviousSibling is WCommentMark)
            {
                // Remove comment start mark.
                firstItem.OwnerParagraph.Items.Remove(firstItem.PreviousSibling);
            }

            if (lastItem.NextSibling != null && lastItem.NextSibling is WCommentMark)
            {
                // Remove comment end mark.
                lastItem.OwnerParagraph.Items.Remove(lastItem.NextSibling);
            }

            if (firstItem != lastItem)
            {
                // Remove everything between first and last paragraph item.
                if (firstItem.OwnerParagraph != lastItem.OwnerParagraph)
                {
                    while (firstItem.OwnerParagraph.NextTextBodyItem != lastItem.OwnerParagraph &&
                      firstItem.OwnerParagraph.NextTextBodyItem != null &&
                      CheckTextBody(firstItem.OwnerParagraph.NextTextBodyItem))
                    {
                        firstItem.OwnerParagraph.NextTextBodyItem.RemoveSelf();
                    }
                }
                // Remove all paragraph items after first paragraph item.
                while (firstItem.NextSibling != null && firstItem.NextSibling != lastItem &&
                  !(firstItem.NextSibling is WComment))
                {
                    firstItem.OwnerParagraph.Items.Remove(firstItem.NextSibling);
                }
                // Remove all paragraph items before last item
                while (lastItem.PreviousSibling != null && lastItem.PreviousSibling != firstItem &&
                  !(firstItem.NextSibling is WComment))
                {
                    lastItem.OwnerParagraph.Items.Remove(lastItem.PreviousSibling);
                }

                // Remove tables between first and last item.
                RemoveTables(firstItem, lastItem);
                RemoveFirstItem(firstItem, lastItem);
            }

            // Remove last item
            lastItem.RemoveSelf();
        }
        /// <summary>
        /// Replace commented items with given text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ReplaceCommentedItems(string text)
        {
            string modText = ModifyText(text);

            WTextRange textRange = new WTextRange(m_doc);
            textRange.Text = text;

            if (this.Format.TagBkmk == -1)
                this.Format.UpdateTagBkmk();

            if (modText.IndexOf("\r") != -1)
            {
                RemoveCommentedItems();
                m_appendItems = false;
                int commentId = this.Format.TagBkmk;
                int indexComment = this.GetIndexInOwnerCollection();
                WCommentMark comStart = new WCommentMark(this.Document, commentId, CommentMarkType.CommentStart);
                WCommentMark comEnd = new WCommentMark(this.Document, commentId, CommentMarkType.CommentEnd);
                this.OwnerParagraph.Items.Insert(indexComment, comEnd);
                this.OwnerParagraph.Items.Insert(indexComment, textRange);
                this.OwnerParagraph.Items.Insert(indexComment, comStart);
            }
            else
            {
                RemoveCommentedItems();
                m_appendItems = true;
                CommentedItems.Add(textRange);
            }
        }
        /// <summary>
        /// Replaces the commented items with specified TextBodyPart.
        /// </summary>
        /// <param name="textBodyPart">The text body part.</param>
        public void ReplaceCommentedItems(TextBodyPart textBodyPart)
        {
            RemoveCommentedItems();
            m_appendItems = true;
            m_bodyPart = textBodyPart;
            FillCommItems();
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        { }
        /// <summary>
        /// Attach the comment items into the CommentsCollection
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="itemPos"></param>
        internal override void Attach(WParagraph owner, int itemPos)
        {
            base.Attach(owner, itemPos);
            Document.Comments.Add(this);
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
            if (m_textBody != null)
            {
                m_textBody.Close();
                m_textBody = null;
            }

            m_format = null;
            m_bodyPart = null;
            m_commItems = null;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);
            if (m_textBody != null)
                m_textBody.CloneRelationsTo(doc, nextOwner);
        }
        /// <summary>
        /// Adds the paragraph item to the commented items.
        /// </summary>
        /// <param name="paraItem">The paragraph item.</param>
        /// <returns></returns>
        public void AddCommentedItem(IParagraphItem paraItem)
        {
            if (this.OwnerParagraph == null)
                return;

            if (m_commItems != null && m_commItems.Contains(paraItem))
                return;

            WParagraph para = this.OwnerParagraph;

            int index = this.GetIndexInOwnerCollection();

            if (m_format.TagBkmk == -1)
            {
                int id = TagIdRandomizer.Instance.Next();
                m_format.TagBkmk = id;

                WCommentMark commentStart = new WCommentMark(m_doc, id);
                commentStart.Type = CommentMarkType.CommentStart;
                WCommentMark commentEnd = new WCommentMark(m_doc, id);
                commentEnd.Type = CommentMarkType.CommentEnd;

                para.Items.Insert(index, commentEnd);
                para.Items.Insert(index, commentStart);
            }

            index = this.GetIndexInOwnerCollection();

            if (para.Items[index - 1] is WCommentMark)
            {
                int curCommentId = m_format.TagBkmk;
                if (paraItem.OwnerParagraph == null)
                {
                    InsertCommItem(para, index - 1, paraItem);
                }
                else if (para.Items.Count > index + 1 && paraItem == para.Items[index + 1])
                {
                    para.Items.RemoveAt(index + 1);
                    InsertCommItem(para, index - 1, paraItem);
                }
                else
                {
                    WCommentMark start = FindCommentStart(index, curCommentId, para.Items);
                    if (start != null && paraItem == para.Items[start.GetIndexInOwnerCollection() - 1])
                    {
                        int startIndex = start.GetIndexInOwnerCollection();
                        para.Items.RemoveAt(startIndex - 1);
                        InsertCommItem(para, startIndex, paraItem);
                    }
                    else
                    {
                        ParagraphItem item = paraItem.Clone() as ParagraphItem;
                        InsertCommItem(para, index - 1, item);
                    }
                }
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// Initialize the XDLS holder.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.TextBodyTag, m_textBody);
            XDLSHolder.AddElement(XDLSConstants.CommentFormatTag, m_format);
        }
        /// <summary>
        /// Writes the XML attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.Comment);
        }
//#endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Creates the layout info.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo();
        }
#endif
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// Inserts the commented item.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item to insert.</param>
        private void InsertCommItem(WParagraph para, int index, IParagraphItem item)
        {
            para.Items.Insert(index, item);
            (item as ParagraphItem).SetOwner(para);
            CommentedItems.Add(item);
        }
        /// <summary>
        /// Finds the comment start.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="startId">The start id.</param>
        /// <param name="itemCollection">The item collection.</param>
        /// <returns></returns>
        private WCommentMark FindCommentStart(int index, int startId, ParagraphItemCollection itemCollection)
        {
            ParagraphItem item = null;
            WCommentMark start = null;
            for (int i = index; i > 0; i--)
            {
                item = itemCollection[i];
                if (item is WCommentMark)
                {
                    WCommentMark commentMark = item as WCommentMark;
                    if (commentMark.Type == CommentMarkType.CommentStart && commentMark.CommentId == startId)
                    {
                        start = commentMark;
                        break;
                    }
                }
            }
            return start;
        }
        /// <summary>
        /// Checks the text body on the existing of WComment objects.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool CheckTextBody(TextBodyItem item)
        {
            if (item is WParagraph)
            {
                return CheckPara(item as WParagraph);
            }
            else
            {
                return CheckTable(item as WTable);
            }
        }
        /// <summary>
        /// Checks the paragraph on the existing of WComment objects.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <returns></returns>
        private bool CheckPara(WParagraph para)
        {
            foreach (ParagraphItem item in para.Items)
            {
                if (item is WComment)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Checks the table on the existing of WComment objects.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        private bool CheckTable(WTable table)
        {
            bool check = true;
            foreach (WTableRow row in table.Rows)
                foreach (WTableCell cell in row.Cells)
                    foreach (IEntity ent in cell.ChildEntities)
                    {
                        if (ent is WParagraph)
                            check = CheckPara(ent as WParagraph);
                        else
                            check = CheckTable(ent as WTable);

                        if (!check)
                            return false;
                    }
            return check;
        }
        /// <summary>
        /// Removes the tables.
        /// </summary>
        /// <param name="firstItem">The first item.</param>
        /// <param name="lastItem">The last item.</param>
        private void RemoveTables(ParagraphItem firstItem, ParagraphItem lastItem)
        {
            if (firstItem.OwnerParagraph.NextSibling == lastItem.OwnerParagraph ||
              firstItem.OwnerParagraph.NextSibling == null)
                return;

            WParagraph firstItemPara = firstItem.OwnerParagraph;
            WParagraph lastItemPara = lastItem.OwnerParagraph;
            WTable commentTable = null;
            WTable lastItemTable = null;

            if (this.OwnerParagraph.Owner is WTableCell)
            {
                commentTable = (this.OwnerParagraph.Owner as WTableCell).OwnerRow.OwnerTable;
            }

            if (lastItemPara.Owner is WTableCell)
            {
                lastItemTable = (lastItemPara.Owner as WTableCell).OwnerRow.OwnerTable;
            }

            while (firstItemPara.NextSibling != lastItem.OwnerParagraph && firstItemPara.NextSibling != null)
            {
                if (firstItem.NextSibling == commentTable || firstItem.NextSibling == lastItemTable)
                    break;

                (firstItem.NextSibling as TextBodyItem).RemoveSelf();
            }
        }
        /// <summary>
        /// Removes the first item.
        /// </summary>
        /// <param name="firstItem">The first item.</param>
        /// <param name="lastItem">The last item.</param>
        private void RemoveFirstItem(ParagraphItem firstItem, ParagraphItem lastItem)
        {
            WParagraph ownerPara = firstItem.OwnerParagraph;
            int ownerIndex = ownerPara.GetIndexInOwnerCollection();
            // If item is not the only item in the paragraph
            if (ownerIndex > 0)
            {
                firstItem.RemoveSelf();
                return;
            }

            // Define whether first item in in the table.
            WTable ownerTable = null;
            if (ownerPara.Owner is WTableCell)
            {
                ownerTable = (ownerPara.Owner as WTableCell).OwnerRow.OwnerTable;
            }

            // If first item is not in the table
            if (ownerTable == null)
            {
                if (ownerPara.Items.Count > 1)
                    firstItem.RemoveSelf();
                else
                    ownerPara.RemoveSelf();
                return;
            }

            // Define whether last paragraph is in the table.
            WParagraph lastItemPara = lastItem.OwnerParagraph;
            WTable lastItemTable = null;
            if (lastItemPara.Owner is WTableCell)
            {
                lastItemTable = (lastItemPara.Owner as WTableCell).OwnerRow.OwnerTable;
            }

            if (ownerTable != lastItemTable)
            {
                if (ownerPara.Owner == ownerTable.FirstRow.Cells[0])
                {
                    ownerTable.RemoveSelf();
                    return;
                }
            }

            firstItem.RemoveSelf();
        }
        /// <summary>
        /// Fills the commented items from TextBodyPart.
        /// </summary>
        private void FillCommItems()
        {
            foreach (TextBodyItem bodyItem in m_bodyPart.BodyItems)
            {
                if (bodyItem is WParagraph)
                    FillCommItems(bodyItem as WParagraph);
                else
                    FillCommItems(bodyItem as WTable);
            }
        }
        /// <summary>
        /// Fills the commented items from the paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void FillCommItems(WParagraph para)
        {
            foreach (ParagraphItem item in para.Items)
            {
                CommentedItems.Add(item);
            }
        }
        /// <summary>
        /// Fills the commented item from the table.
        /// </summary>
        /// <param name="table">The table.</param>
        private void FillCommItems(WTable table)
        {
            foreach (WTableRow row in table.Rows)
                foreach (WTableCell cell in row.Cells)
                    foreach (IEntity ent in cell.ChildEntities)
                    {
                        if (ent is WParagraph)
                            FillCommItems(ent as WParagraph);
                        else
                            FillCommItems(ent as WTable);
                    }
        }
        /// <summary>
        /// Modifies the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string ModifyText(string text)
        {
            text = text.Replace("\r\n", "\r");
            text = text.Replace('\n', '\r');

            return text;
        }
        #endregion
    }
    /// <summary>
    /// Class represents comment start marker
    /// </summary>
    public class WCommentMark : ParagraphItem
    {
        #region Fields
        /// <summary>
        /// Id of the comment current mark refers to
        /// </summary>
        private int m_commentId = -1;
        /// <summary>
        /// 
        /// </summary>
        private CommentMarkType m_markType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the id of the comment this mark refers to.
        /// </summary>
        /// <value>The comment id.</value>
        internal int CommentId
        {
            get
            {
                return m_commentId;
            }
            set
            {
                m_commentId = value;
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
                return EntityType.CommentMark;
            }
        }
        /// <summary>
        /// Gets or sets the type of the CommentMark.
        /// </summary>
        /// <value>The type.</value>
        public CommentMarkType Type
        {
            get
            {
                return m_markType;
            }
            set
            {
                m_markType = value;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WCommentMark"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="commentId">The comment id.</param>
        internal WCommentMark(WordDocument doc, int commentId)
            : base(doc)
        {
            m_commentId = commentId;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WCommentMark"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="commentId">The comment id.</param>
        internal WCommentMark(WordDocument doc, int commentId, CommentMarkType type)
            : this(doc, commentId)
        {
            m_markType = type;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WCommentMark commMark = (WCommentMark)base.CloneImpl();
            // Get next id based on previous.
            if (m_commentId != -1)
            {
                if (m_markType == CommentMarkType.CommentStart)
                    commMark.CommentId = TagIdRandomizer.GetMarkerId(m_commentId, true);
                else
                    commMark.CommentId = TagIdRandomizer.GetMarkerId(m_commentId, false);

            }

            return commMark;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo();
            m_layoutInfo.IsSkip = true;
        }
#endif
        #endregion
    }
}
