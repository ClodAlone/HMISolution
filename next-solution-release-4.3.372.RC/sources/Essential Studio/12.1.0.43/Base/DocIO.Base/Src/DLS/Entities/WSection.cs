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
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Layouting;
using System.Collections.Generic;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// WSection class is used for representation of a section of the document. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WSection
      : WidgetContainer
      , IWSection
      , IWidgetContainer
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const float DEF_DISTANCE_BETWEEN_COLUMNS = 36;
        #endregion

        #region Class members
        private WTextBody m_body;
        /// <summary>
        /// The section page Setup.
        /// </summary>
        private WPageSetup m_pageSetup;
        /// <summary>
        /// The columns collection.
        /// </summary>
        private ColumnCollection m_columns;
        /// <summary>
        /// The headers / footers, related with current section.
        /// </summary>
        internal WHeadersFooters m_headersFooters;
        /// <summary>
        /// The break code.
        /// </summary>
        private SectionBreakCode m_breakCode = SectionBreakCode.NewPage;
        private EntityCollection m_childEntities;
        /// <summary>
        /// 
        /// </summary>
        internal protected byte[] m_internalData = null;
        /// <summary>
        /// Collection that holds names of old styles and their new names.
        /// </summary>
        private Dictionary<string, string> m_oldParaStylesHolder;
        private Dictionary<string, string> m_oldCharStylesHolder;
        /// <summary>
        /// 
        /// </summary>
        internal DocTextDirection m_textDirection = DocTextDirection.LeftToRight;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bProtectForm = true;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the section body.
        /// </summary>
        /// <value>The body.</value>
        public WTextBody Body
        {
            get
            {
                return m_body;
            }
        }
        /// <summary>
        /// Gets headers/footers of current section.
        /// </summary>
        public WHeadersFooters HeadersFooters
        {
            get
            {
                return m_headersFooters;
            }
        }
        /// <summary>
        /// Gets page Setup of current section.
        /// </summary>
        public WPageSetup PageSetup
        {
            get
            {
                return m_pageSetup;
            }
        }
        /// <summary>
        /// Get collection of columns which logically divide page on many.
        /// printing/publishing areas
        /// </summary>
        public ColumnCollection Columns
        {
            get
            {
                return m_columns;
            }
        }
        /// <summary>
        /// Gets / sets break code.
        /// </summary>
        public SectionBreakCode BreakCode
        {
            get
            {
                return m_breakCode;
            }
            set
            {
                m_breakCode = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] DataArray
        {
            get
            {
                return m_internalData;
            }
            set
            {
                m_internalData = value;
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
                return EntityType.Section;
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
                if (m_childEntities == null)
                {
                    m_childEntities = new SectionChildEntities();
                    m_childEntities.InnerList.Add(m_body);
                    m_body.SetOwner(this);

                    for (int i = 0; i < 6; i++)
                    {
                        m_childEntities.InnerList.Add(m_headersFooters[i]);
                        m_headersFooters[i].SetOwner(this);
                    }
                }

                return m_childEntities;
            }
        }
        /// <summary>
        /// Gets the paragraphs.
        /// </summary>
        /// <value>The paragraphs.</value>
        public IWParagraphCollection Paragraphs
        {
            get
            {
                return Body.Paragraphs;
            }
        }
        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <value>The tables.</value>
        public IWTableCollection Tables
        {
            get
            {
                return Body.Tables;
            }
        }
        /// <summary>
        /// Gets the collection of data pairs "Old paragraph style name"/"New style name".
        /// </summary>
        internal Dictionary<string, string> OldParaStylesHolder
        {
            get
            {
                if (m_oldParaStylesHolder == null)
                {
                    m_oldParaStylesHolder = new Dictionary<string, string>();
                }
                return m_oldParaStylesHolder;
            }
        }
        /// <summary>
        /// Gets the collection of data pairs "Old character style name"/"New style name".
        /// </summary>
        internal Dictionary<string, string> OldCharStylesHolder
        {
            get
            {
                if (m_oldCharStylesHolder == null)
                {
                    m_oldCharStylesHolder = new Dictionary<string, string>();
                }
                return m_oldCharStylesHolder;
            }
        }
        /// <summary>
        /// Gets or sets the text direction.
        /// </summary>
        /// <value>The text direction.</value>
        internal DocTextDirection TextDirection
        {
            get
            {
                return m_textDirection;
            }
            set
            {
                m_textDirection = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [protect form].
        /// </summary>
        /// <value><c>true</c> if [protect form]; otherwise, <c>false</c>.</value>
        public bool ProtectForm
        {
            get
            {
                return m_bProtectForm;
            }
            set
            {
                m_bProtectForm = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WSection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WSection(IWordDocument doc)
            : base((WordDocument)doc, null)
        {
            m_body = new WTextBody(this);
            m_columns = new ColumnCollection(this);
            m_headersFooters = new WHeadersFooters(this);
            m_headersFooters.SetOwner(this);
            m_pageSetup = new WPageSetup(this);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds new column to the section.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="spacing">The spacing.</param>
        /// <returns></returns>
        public Column AddColumn(float width, float spacing)
        {
            Column column = new Column(Document);
            column.Width = width;
            column.Space = spacing;
            Columns.Add(column);

            return column;
        }
        /// <summary>
        /// Makes all columns in current section to be of equal width.
        /// </summary>
        public void MakeColumnsEqual()
        {
            if (Columns.Count > 0)
            {
                float usingWidth = PageSetup.PageSize.Width
                  - ((PageSetup.Margins.Left != -0.05f) ? PageSetup.Margins.Left : 0f)
                  - ((PageSetup.Margins.Right != -0.05f) ? PageSetup.Margins.Right : 0f);

                float colWidth = (usingWidth - (Columns.Count - 1) * DEF_DISTANCE_BETWEEN_COLUMNS) / Columns.Count;

                foreach (Column column in Columns)
                {
                    column.Width = colWidth;
                    column.Space = DEF_DISTANCE_BETWEEN_COLUMNS;
                }
            }
        }
        /// <summary>
        /// Clones it self.
        /// </summary>
        /// <returns></returns>
        new public WSection Clone()
        {
            return (WSection)base.Clone();
        }
        /// <summary>
        /// Adds the paragraph.
        /// </summary>
        /// <returns></returns>
        public IWParagraph AddParagraph()
        {
            return Body.AddParagraph();
        }
        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <returns></returns>
        public IWTable AddTable()
        {
            return Body.AddTable();
        }
        internal IStructureDocumentTagBlock AddStructureDocumentTag()
        {
            return  Body.AddStructureDocumentTag();
        }
        /// <summary>
        /// Adds the alternate chunk.
        /// </summary>
        /// <returns></returns>
        internal AlternateChunk AddAlternateChunk()
        {
            return Body.AddAlternateChunk();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            Body.AddSelf();
            for (int i = 0; i < 6; i++)
            {
                HeadersFooters[i].AddSelf();
            }
        }
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns></returns>
        internal string GetText()
        {
            string text = string.Empty;
            for (int i = 0; i < Body.ChildEntities.Count; i++)
            {
                Entity entity = Body.ChildEntities[i];
                if (entity is WParagraph)
                    text += (entity as WParagraph).GetParagraphText();
                else if(entity is WTable)
                    text += (entity as WTable).GetTableText();
                if (Document.m_prevClonedEntity != null)
                {
                    i = Document.m_prevClonedEntity.GetIndexInOwnerCollection();
                    Document.m_prevClonedEntity = null;
                }
            }
            return text;
        }
        /// <summary>
        /// Clones the without body items.
        /// Creates new section by cloning the page setup, column and header footers of the current section.
        /// </summary>
        /// <returns></returns>
        internal WSection CloneWithoutBodyItems()
        {
            Boolean prevStatus = Document.IsCloning;
            Document.IsCloning = true;

            //Creates new section.
            WSection section = new WSection(Document);

            //Clones the page setup of the section.
            section.m_pageSetup = PageSetup.Clone();
            section.m_pageSetup.SetOwner(section);

            //Clones the columns of the section.
            section.m_columns = new ColumnCollection(section);
            m_columns.CloneTo(section.m_columns);

            //Clones the header footers of the section.
            section.m_headersFooters = m_headersFooters.Clone();
            section.m_headersFooters.SetOwner(section);
            for (int i = 0; i < 6; i++)
            {
                section.m_headersFooters[i].SetOwner(section);
            }

            Document.IsCloning = prevStatus;
            return section;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            doc.CurClonedSection = this;
            base.CloneRelationsTo(doc, nextOwner);
            Body.CloneRelationsTo(doc, nextOwner);
            ImportOptions importOption = doc.ImportOption;
            bool importStyles = doc.ImportStyles;
            //Handled for preserving the Header footer contents with destination styles.
            if (doc.ImportOption != ImportOptions.UseDestinationStyles)
            {
                doc.ImportOption = ImportOptions.UseDestinationStyles;
                doc.ImportStyles = false;
            }
            for (int i = 0; i <= 5; i++)
            {
                m_headersFooters[i].CloneRelationsTo(doc, nextOwner );
            }
            if (doc.ImportOption != importOption)
            {
                doc.ImportOption = importOption;
                doc.ImportStyles = importStyles;
            }
            doc.CurClonedSection.OldCharStylesHolder.Clear();
            doc.CurClonedSection.OldParaStylesHolder.Clear();
            doc.CurClonedSection = null;
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            Boolean prevStatus = Document.IsCloning;
            Document.IsCloning = true;

            WSection sec = (WSection)base.CloneImpl();
            sec.m_childEntities = null;

            sec.m_body = (WTextBody)m_body.Clone();
            sec.m_body.SetOwner(sec);

            sec.m_pageSetup = PageSetup.Clone();
            sec.m_pageSetup.SetOwner(sec);

            sec.m_columns = new ColumnCollection(sec);

            m_columns.CloneTo(sec.m_columns);

            sec.m_headersFooters = m_headersFooters.Clone();
            sec.m_headersFooters.SetOwner(sec);

            for (int i = 0; i < 6; i++)
            {
                sec.m_headersFooters[i].SetOwner(sec);
            }

            Document.IsCloning = prevStatus;
            return sec;
        }
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        internal void MakeChanges(bool acceptChanges)
        {
            // Make changes for TextBodyItems
            m_body.MakeChanges(acceptChanges);
            if (m_internalData != null && m_internalData.Length < 300 && m_internalData.Length > 0)
            {
                //Create SPRM Array collection from Data Array
                SinglePropertyModifierArray modifierArray = new SinglePropertyModifierArray(m_internalData, 0);
                //Removes the duplicate sprms previous to revision SPRM - sprmSWall - 0x3239
                SinglePropertyModifierRecord sprm = modifierArray.TryGetSprm(WordSprmOptions.sprmSWall);
                if (sprm != null)
                {
                    int startIndex = modifierArray.Modifiers.IndexOf(sprm) + 1;
                    List<SinglePropertyModifierRecord> sprms = null;
                    if (startIndex < modifierArray.Count)
                    {
                        sprms = new List<SinglePropertyModifierRecord>();
                        for (int i = startIndex, cnt = modifierArray.Count; i < cnt; i++)
                            sprms.Add(modifierArray.GetSprmByIndex(i));

                        foreach (SinglePropertyModifierRecord modifier in sprms)
                        {
                            modifierArray.RemoveValue(modifier.Options);
                            modifierArray.Add(modifier);
                        }
                    }
                    modifierArray.RemoveValue(WordSprmOptions.sprmSWall);
                }
                //Reset Data array
                m_internalData = new byte[m_internalData.Length];
                //Save bytes
                modifierArray.Save(m_internalData, 0);
            }
            // MakeChanges for headers/footers
            for (int i = 0; i < 6; i++)
            {
                HeadersFooters[i].MakeChanges(acceptChanges);
            }
        }
        /// <summary>
        /// Determines whether section has tracked changes.
        /// </summary>
        /// <returns>
        /// 	if has tracked changes, set to <c>true</c>.
        /// </returns>
        internal bool HasTrackedChanges()
        {
            if (m_body.HasTrackedChanges())
                return true;

            for (int i = 0; i < 6; i++)
            {
                if (HeadersFooters[i].HasTrackedChanges())
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        internal void Close()
        {
            if (m_body != null)
            {
                m_body.Close();
                m_body = null;
            }

            if (m_headersFooters != null)
            {
                m_headersFooters.Close();
                m_headersFooters = null;
            }

            if (m_oldCharStylesHolder != null)
            {
                m_oldCharStylesHolder.Clear();
                m_oldCharStylesHolder = null;
            }

            if (m_oldParaStylesHolder != null)
            {
                m_oldParaStylesHolder.Clear();
                m_oldParaStylesHolder = null;
            }

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
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.TextBodyTag, m_body);
            XDLSHolder.AddElement(XDLSConstants.PageSetupTag, m_pageSetup);
            XDLSHolder.AddElement(XDLSConstants.ColumnsTag, m_columns);
            XDLSHolder.AddElement(XDLSConstants.HeadersFootersTag, m_headersFooters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.SectionBreakCodeAttr, BreakCode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.SectionBreakCodeAttr))
            {
                BreakCode = (SectionBreakCode)reader.ReadEnum(XDLSConstants.SectionBreakCodeAttr, typeof(SectionBreakCode));
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            if (DataArray != null)
            {
                writer.WriteChildBinaryElement(XDLSConstants.InternalDataTag, DataArray);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            bool retValue = base.ReadXmlContent(reader);

            if (reader.TagName == XDLSConstants.InternalDataTag)
            {
                DataArray = reader.ReadChildBinaryElement();
                retValue = true;
            }

            return retValue;
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
            //To break infinite looping of Layouting for the section with no child items
            if (this.Body.Items.Count == 0)
                m_layoutInfo.IsSkip = true;
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
                //To remove the paragraph which have section break
                if (m_body.Items.Count > 0
                    && m_body.Items.LastItem is WParagraph
                    && (m_body.Items.LastItem as WParagraph).SectionEndMark
                    && (m_body.Items.LastItem as WParagraph).PreviousSibling is WParagraph
                    && ((m_body.Items.LastItem as WParagraph).PreviousSibling as WParagraph).ChildEntities.Count > 0
                    && ((m_body.Items.LastItem.PreviousSibling as WParagraph).LastItem is Break)
                    && ((m_body.Items.LastItem.PreviousSibling as WParagraph).LastItem as Break).BreakType == BreakType.PageBreak
                    && !Document.DOP.Dop2000.Copts.SplitPgBreakAndParaMark)
                {
                    BodyItemCollection bodyItems = new BodyItemCollection(this.m_body);
                    for (int i = 0; i < m_body.Items.Count; i++)
                    {
                        bodyItems.InnerList.Add(m_body.Items[i]);
                    }
                    bodyItems.InnerList.RemoveAt(bodyItems.IndexOf(bodyItems.LastItem));
                    return bodyItems;
                }
                return m_body.Items;
            }
        }
        #endregion

        #region Internal declarations
        internal class SectionChildEntities : EntityCollection
        {
            internal SectionChildEntities()
                : base(null)
            {
            }
#if !SILVERLIGHT && !WP
            protected override string GetTagItemName()
            {
                throw new Exception();
            }
            protected override OwnerHolder CreateItem(IXDLSContentReader reader)
            {
                throw new Exception();
            }
#endif
            /// <summary>
            /// 
            /// </summary>
            protected override System.Type[] TypesOfElement
            {
                get
                {
                    throw new Exception("Cannot insert an object to SectionChildEntities collection.");
                }
            }
        }

        #endregion
    }
}
