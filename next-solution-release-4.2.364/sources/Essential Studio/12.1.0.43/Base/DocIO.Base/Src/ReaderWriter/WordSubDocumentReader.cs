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

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Implemented IWordSubdocumentReader interface, used for reading 
    /// Word "subdocument parts" as header/footer, footnotes, endnotes, annotations, macros, etc.
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal abstract class WordSubdocumentReader
      : WordReaderBase,
      IWordSubdocumentReader
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        protected int DEF_SECTION_NUMBER = 1;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected HeaderType m_headerType = HeaderType.InvalidValue;

        /// <summary>
        /// 
        /// </summary>
        protected int m_itemIndex = -1;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsNextItemPos;

        /// <summary>
        /// 
        /// </summary>
        private WordReader m_mainReader = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates subdocument reader of specified type.
        /// </summary>
        /// <param name="mainReader"> Main document word reader. </param>
        public WordSubdocumentReader(WordReader mainReader)
            : base(mainReader.m_streamsManager)
        {
            m_mainReader = mainReader;
            m_styleSheet = mainReader.StyleSheet;
            m_currStyleIndex = mainReader.CurrentStyleIndex;
            m_mainReader.FreezeStreamPos();
            InitClass();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// The type of subdocument reader.
        /// </summary>
        public WordSubdocument Type
        {
            get
            {
                return m_type;
            }
        }

        /// <summary>
        /// Gets/Sets current type of header / footer.
        /// ( if Type != WordSubdocument.HeaderFooter, returns HeaderType.InvalidValue )
        /// </summary>
        public HeaderType HeaderType
        {
            get
            {
                return m_headerType;
            }
            set
            {
                m_headerType = value;
            }
        }

        /// <summary>
        /// Gets number of current footnote 
        /// </summary>
        public int ItemNumber
        {
            get
            {
                return m_itemIndex;
            }
        }

        ///// <summary>
        ///// Gets number of current footnote 
        ///// </summary>
        //public int AtnNumber
        //{
        //  get
        //  {
        //    return m_atnIndex;
        //  }
        //}
        ///// <summary>
        ///// Gets the text box number.
        ///// </summary>
        ///// <value>The text box number.</value>
        //public int TextBoxNumber
        //{
        //  get
        //  {
        //    return m_txtBoxIndex;
        //  }
        //}
        ///// <summary>
        ///// Gets the header/footer texbox number
        ///// </summary>
        //public int HFTextBoxNumber
        //{
        //  get
        //  {
        //    return m_hfTxbxIndex;
        //  }
        //}    
        ///// <summary>
        ///// 
        ///// </summary>
        //public int EndnoteNumber
        //{
        //  get
        //  {
        //    return m_ednIndex;
        //  }
        //}

        /// <summary>
        /// 
        /// </summary>
        public StatePositionsBase StatePositions
        {
            get
            {
                return m_statePositions;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is next item pos.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is next item pos; otherwise, <c>false</c>.
        /// </value>
        internal bool IsNextItemPos
        {
            get
            {
                return m_bIsNextItemPos;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FieldDescriptor GetFld()
        {
            int cp = CalcCP(StatePositions.StartItemPos, 1);

            return m_docInfo.TablesData.Fields.FindFld(m_type, cp);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Reset()
        {
            m_docInfo = m_mainReader.m_docInfo;

            CreateStatePositions();
            UpdateCharacterProperties();
            UpdateParagraphProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        public virtual void MoveToItem(int itemIndex)
        {
            m_itemIndex = itemIndex;
            UpdateStreamPosition();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Creates object of needed type and fills it with stream positions
        /// </summary>
        protected virtual void CreateStatePositions()
        {
            MoveToItem(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iCurrentPos"></param>
        /// <returns></returns>
        protected override long GetChunkEndPosition(long iCurrentPos)
        {
            long iMinPos = base.GetChunkEndPosition(iCurrentPos);

            return Math.Min(iMinPos, StatePositions.EndItemPos);
        }

        /// <summary>
        /// Initializes class members
        /// </summary>
        protected override void InitClass()
        {
            Reset();
        }

        /// <summary>
        /// Initializes start/end positioons of reader
        /// </summary>
        protected virtual void UpdateStreamPosition()
        {
            m_streamsManager.MainStream.Position = StatePositions.MoveToItem(m_itemIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        protected override void UpdateEndPositions(long iEndPos)
        {
            m_bIsNextItemPos = false;
            base.UpdateEndPositions(iEndPos);

            if (m_type == WordSubdocument.HeaderFooter)
            {
                if ((StatePositions as HFStatePositions).UpdateHeaderEndPos(iEndPos, m_headerType))
                {
                    // If header position updated, then increment header type.
                    m_headerType++;
                }
            }
            else
            {
                if (StatePositions.UpdateItemEndPos(iEndPos))
                {
                    m_itemIndex++;
                    m_bIsNextItemPos = true;
                }
            }

            UpdateCharacterProperties();
            UpdateParagraphProperties();
        }
        /// <summary>
        /// Unfreeze stream position
        /// </summary>
        public override void UnfreezeStreamPos()
        {
            // Before freeze main stream position.
            m_mainReader.FreezeStreamPos();

            base.UnfreezeStreamPos();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HeaderFooter subdocument reader
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordHeaderFooterReader : WordSubdocumentReader
    {
        #region Class properties
        /// <summary>
        /// Gets HFStatePositions object
        /// </summary>
        new public HFStatePositions StatePositions
        {
            get
            {
                return (HFStatePositions)m_statePositions;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordHeaderFooterReader"/> class.
        /// </summary>
        /// <param name="mainReader">Main document word reader.</param>
        public WordHeaderFooterReader(WordReader mainReader)
            : base(mainReader)
        {
            m_type = WordSubdocument.HeaderFooter;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Resets subdocument reader state, reading process restarted.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            MoveToSection(DEF_SECTION_NUMBER);
        }

        /// <summary>
        /// Move to specified section
        /// </summary>
        /// <param name="iSectionNumber"></param>
        public void MoveToSection(int iSectionNumber)
        {
            StatePositions.SectionIndex = iSectionNumber - 1;

            MoveToHeader((HeaderType)0);
            // Updates properties
            UpdateCharacterProperties();
            UpdateParagraphProperties();
        }

        /// <summary>
        /// Move to specified header in currebt section
        /// </summary>
        /// <param name="hType"></param>
        public void MoveToHeader(HeaderType hType)
        {
            m_headerType = hType;
            UpdateStreamPosition();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        public override void MoveToItem(int itemIndex)
        {
            MoveToHeader((HeaderType)itemIndex);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FileShapeAddress GetFSPA()
        {
            int cp = CalcCP(StatePositions.StartItemPos, 1);

            return m_docInfo.TablesData.FileArtObjects != null ? m_docInfo.TablesData.FileArtObjects.FindFileShape(m_type, cp) : null;
        }

        /// <summary>
        /// Reset as it is HeaderFooter reader
        /// </summary>
        /// <returns></returns>
        protected override void UpdateStreamPosition()
        {
            if (m_docInfo.TablesData.HeaderFooterCharPosTable == null)
            {
                m_chunkType = WordChunkType.DocumentEnd;
                m_headerType = HeaderType.InvalidValue;
                return;
            }
            UnfreezeStreamPos();

            m_chunkType = WordChunkType.Text;
            m_streamsManager.MainStream.Position = StatePositions.MoveToItem((int)m_headerType);
        }

        /// <summary>
        /// Creates object and fills it with stream positions.
        /// </summary>
        protected override void CreateStatePositions()
        {
            if (m_statePositions == null)
                m_statePositions = new HFStatePositions(m_docInfo.FkpData);
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HeaderFooter subdocument reader.
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordFootnoteReader : WordSubdocumentReader
    {
        #region Class members
        /// <summary>
        /// Saves previous position of main stream.
        /// </summary>
        protected int m_prevStreamPos = -1;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates subdocument reader of specified type.
        /// </summary>
        /// <param name="mainReader"> Main document word reader. </param>
        public WordFootnoteReader(WordReader mainReader)
            : base(mainReader)
        {
            Init();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets a value indicating whether this body refers to next footnote.
        /// </summary>
        /// <value>
        /// <c>true</c> if this body refers to next footnote; otherwise, <c>false</c>.
        /// </value>
        public bool IsNextItem
        {
            get
            {
                bool retVal = false;
                bool isNext = CheckPosition();
                if (isNext)
                {
                    retVal = (m_streamsManager.MainStream.Position == m_prevStreamPos) ? false : isNext;
                    m_prevStreamPos = (int)m_streamsManager.MainStream.Position;
                }

                return retVal;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Creates object and fills it with stream positions.
        /// </summary>
        protected override void CreateStatePositions()
        {
            InitStatePositions();
            base.CreateStatePositions();
        }

        /// <summary>
        /// Checks current cp position.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckPosition()
        {
            int cp = CalcCP(StatePositions.StartText, m_textChunk.Length);
            return m_docInfo.FkpData.Tables.Footnotes.HasPosition(cp) && cp != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init()
        {
            m_type = WordSubdocument.Footnote;
        }

        /// <summary>
        /// Initialize the state positions.
        /// </summary>
        protected virtual void InitStatePositions()
        {
            m_statePositions = new FootnoteStatePositions(m_docInfo.FkpData);
        }

        /// <summary>
        /// Determines the end of footnote items.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsEndOfItems()
        {
            return (m_docInfo.FkpData.Tables.Footnotes.Count == m_itemIndex + 1) ? true : false;
        }

        /// <summary>
        /// Read next elementary text string*.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// * - "elementary text string" - string, in which all symbols have the
        /// identical character/paragraph/section properties.
        /// </remarks>
        public override WordChunkType ReadChunk()
        {
            WordChunkType chunkType = base.ReadChunk();

            if (IsEndOfItems())
            {
                chunkType = WordChunkType.DocumentEnd;
            }

            return chunkType;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HeaderFooter subdocument reader.
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordAnnotationReader : WordSubdocumentReader
    {
        #region Class properties
        /// <summary>
        /// Gets a value indicating whether this instance is start annotation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is start annotation; otherwise, <c>false</c>.
        /// </value>
        public bool IsStartAnnotation
        {
            get
            {
                int cp = CalcCP(StatePositions.StartText, m_textChunk.Length);

                return m_docInfo.FkpData.Tables.Annotations.HasPosition(cp);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordAnnotationReader"/> class.
        /// </summary>
        /// <param name="mainReader">Main document word reader.</param>
        public WordAnnotationReader(WordReader mainReader)
            : base(mainReader)
        {
            m_type = WordSubdocument.Annotation;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AnnotationDescriptor Descriptor
        {
            get
            {
                return m_docInfo.TablesData.Annotations.GetDescriptor(m_itemIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string User
        {
            get
            {
                return m_docInfo.TablesData.Annotations.GetUser(m_itemIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BookmarkStartOffset
        {
            get
            {
                return m_docInfo.TablesData.Annotations.GetBookmarkStartOffset(m_itemIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BookmarkEndOffset
        {
            get
            {
                return m_docInfo.TablesData.Annotations.GetBookmarkEndOffset(m_itemIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override WordChunkType ReadChunk()
        {
            base.ReadChunk();
            if (m_docInfo.FkpData.Tables.Annotations.Count == m_itemIndex + 1)
            {
                m_chunkType = WordChunkType.DocumentEnd;
            }

            return m_chunkType;
        }


        /// <summary>
        /// Gets the position of the annotation.
        /// </summary>
        /// <value>The get position.</value>
        public int Position
        {
            get
            {
                return m_docInfo.TablesData.Annotations.GetPosition(m_itemIndex);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Creates object and fills it with stream positions.
        /// </summary>
        protected override void CreateStatePositions()
        {
            m_statePositions = new AtnStatePositions(m_docInfo.FkpData);
            base.CreateStatePositions();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for Endnote subdocument reader.
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordEndnoteReader : WordFootnoteReader
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordEndnoteReader"/> class.
        /// </summary>
        /// <param name="mainReader">Main document word reader.</param>
        public WordEndnoteReader(WordReader mainReader)
            : base(mainReader)
        { }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Checks current cp position.
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPosition()
        {
            int cp = CalcCP(StatePositions.StartText, m_textChunk.Length);
            return m_docInfo.FkpData.Tables.Endnotes.HasPosition(cp) && cp != 0;
        }

        /// <summary>
        /// Initialize reader.
        /// </summary>
        protected override void Init()
        {
            m_type = WordSubdocument.Endnote;
        }

        /// <summary>
        /// Initialize the state positions.
        /// </summary>
        protected override void InitStatePositions()
        {
            m_statePositions = new EndnoteStatePositions(m_docInfo.FkpData);
        }

        /// <summary>
        /// Determines the end of footnote items.
        /// </summary>
        /// <returns></returns>
        protected override bool IsEndOfItems()
        {
            return (m_docInfo.FkpData.Tables.Endnotes.Count == m_itemIndex + 1) ? true : false;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordTextBoxReader : WordSubdocumentReader
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordTextBoxReader"/> class.
        /// </summary>
        /// <param name="mainReader">Main document word reader.</param>
        public WordTextBoxReader(WordReader mainReader)
            : base(mainReader)
        {
            m_type = WordSubdocument.TextBox;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Creates object of needed type and fills it with stream positions
        /// </summary>
        protected override void CreateStatePositions()
        {
            m_statePositions = new TextBoxStatePositions(m_docInfo.FkpData);
            base.CreateStatePositions();
        }
        #endregion
    }

    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordHFTextBoxReader : WordSubdocumentReader
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordHFTextBoxReader"/> class.
        /// </summary>
        /// <param name="mainReader">Main document word reader.</param>
        public WordHFTextBoxReader(WordReader mainReader)
            : base(mainReader)
        {
            m_type = WordSubdocument.HeaderTextBox;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Creates object of needed type and fills it with stream positions
        /// </summary>
        protected override void CreateStatePositions()
        {
            m_statePositions = new HFTextBoxStatePositions(m_docInfo.FkpData);
            base.CreateStatePositions();
        }
        #endregion
    }
}