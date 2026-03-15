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

#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class SDTContent
    {
        #region fields
        /// <summary>
        /// TODO: Need to investigate and handle
        /// </summary>
        // m_BookmarkEnd;
        // m_BookmarkStart;
        // m_CommentRangeEnd;
        // m_CommentRangeStart;
        // m_CustomXML;
        // m_CustomXMLDelRangeEnd;
        // m_CustomXMLDelRangeStart;
        // m_CustomXMLInsRangeStart;
        // m_CustomXMLInsRangeEnd;
        // m_CustomXMLMoveToRangeEnd;
        // m_CustomXMLMoveToRangeStart;
        // m_del;
        // m_ins;
        // m_MoveFromRangeEnd;
        // m_MoveFromRangeStart;
        // m_MoveTo;
        // m_MoveToRangeEnd;
        // m_MoveToRangeStart;
        // m_OMath;
        // m_OMathPara;
        // m_PermStart;
        // m_PermEnd;
        // m_ProogErr;
        #endregion
    }
    internal class SDTBlockContent : WidgetBase
    {
        #region Fields
        private WParagraphCollection m_Paragraphs;
        private WTableCollection m_Tables;
        private WTextBody m_TextBody;
        private StructureDocumentTagBlock m_SDTBlock;
        private EntityCollection m_childEntities;
        #endregion 

        #region properties
        internal WParagraphCollection Paragraphs
        {
            get { return m_Paragraphs; }
            set { m_Paragraphs = value; }
        }
        internal WTableCollection Tables
        {
            get { return m_Tables; }
            set { m_Tables = value; }
        }
        internal WTextBody TextBody
        {
            get { return m_TextBody; }
            set { m_TextBody = value; }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.SDTBlockContent;
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
        #endregion

        #region Constructor
        internal SDTBlockContent(WordDocument doc, StructureDocumentTagBlock sdtBlock)
            : base((WordDocument)doc,sdtBlock)
        {
            m_TextBody = new WTextBody(doc ,this);
            m_Paragraphs = new WParagraphCollection(m_TextBody.Items);
            m_Tables = new WTableCollection(m_TextBody.Items);
            m_childEntities = new BodyItemCollection(doc);
        }
        #endregion

        # region Implementation
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            m_TextBody.AddSelf();
        }
        internal SDTBlockContent Clone()
        {
            return (SDTBlockContent)CloneImpl();
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            SDTBlockContent sdtBlockContent = (SDTBlockContent)base.CloneImpl();
            return sdtBlockContent;
        }
#if !SILVERLIGHT && !WP
        protected override void CreateLayoutInfo()
        {
            //TODO: Need to be implemented
        }
#endif
        # endregion
    }
    internal class SDTRowContent
    {
        internal SDTRowContent()
        {
        }
    }
    internal class SDTCellContent 
    {
        internal SDTCellContent()
        {
        }
    }
    internal class SDTInlineContent : WidgetBase
    {
        #region Fields
        private ParagraphItemCollection m_paragraphItemCollection;
        private StructureDocumentTagInline m_SDTInline;
        private EntityCollection m_childEntities;
        #endregion

        #region Properties
        internal ParagraphItemCollection ParagraphItems
        {
            get
            {
                return m_paragraphItemCollection;
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
                return EntityType.SDTInlineContent;
            }
        }
        #endregion

        #region Constructor
        internal SDTInlineContent(WordDocument doc,StructureDocumentTagInline sdtInline)
            : base((WordDocument)doc,sdtInline)
        {
            m_paragraphItemCollection = new ParagraphItemCollection(doc);
            m_paragraphItemCollection.SetOwner(this);
        }
        #endregion

        # region Implementation
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            foreach (ParagraphItem item in m_paragraphItemCollection)
            {
                item.AddSelf();
            }
        }
        internal SDTInlineContent Clone()
        {
            return (SDTInlineContent)CloneImpl();
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            SDTInlineContent sdtInlineContent = (SDTInlineContent)base.CloneImpl();
            return sdtInlineContent;
        }
#if !SILVERLIGHT && !WP
        protected override void CreateLayoutInfo()
        {
            //TODO: Need to be implemented
        }
#endif
        # endregion
    }
}
