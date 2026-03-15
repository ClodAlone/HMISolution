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
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal abstract class WordSubdocumentWriter
      : WordWriterBase, IWordSubdocumentWriter
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordSubdocumentWriter(WordWriter mainWriter)
            : base(mainWriter.m_streamsManager)
        {
            m_docInfo = mainWriter.m_docInfo;
            m_styleSheet = mainWriter.StyleSheet;
            InitClass();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets type of subdocumnet writer
        /// </summary>
        public WordSubdocument Type
        {
            get
            {
                return m_type;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        public abstract void WriteDocumentEnd();

        /// <summary>
        /// 
        /// </summary>
        public virtual void WriteItemStart()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void WriteItemEnd()
        {
            WriteMarker(WordChunkType.ParagraphEnd);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitClass()
        {
            base.InitClass();
            m_iStartText = (int)m_streamsManager.MainStream.Position;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for footnote subdocument writer
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordFootnoteWriter : WordSubdocumentWriter
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Creates subdocument writer of specified type.
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordFootnoteWriter(WordWriter mainWriter)
            : base(mainWriter)
        {
            m_type = WordSubdocument.Footnote;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Writes the end of the document
        /// </summary>
        public override void WriteDocumentEnd()
        {
            m_docInfo.TablesData.Footnotes.AddTxtPosition(m_docInfo.FibData.ccpFtn);
            m_docInfo.TablesData.Footnotes.AddTxtPosition(m_docInfo.FibData.ccpFtn + 3);
            WriteChar(SpecialCharacters.ParagraphEnd);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteItemStart()
        {
            m_docInfo.TablesData.Footnotes.AddTxtPosition(m_docInfo.FibData.ccpFtn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpFtn += dataLength;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HeaderFooter subdocument writer
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordHeaderFooterWriter : WordSubdocumentWriter
    {
        #region Class constants
        //private readonly int[] DEF_HEADER_MAGIC_ARR = new int[] { 0, 3, 6, 9, 12 };
        //private const char DEF_HEADER_MAGIC_SEP1 = ( char )3;
        //private const char DEF_HEADER_MAGIC_SEP2 = ( char )4;
        private const int DEF_HEADER_INDEX = 7;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected HeaderType m_headerType = (HeaderType)0;

        /// <summary>
        /// 
        /// </summary>
        private int m_iItemIndex;

        /// <summary>
        /// 
        /// </summary>
        private int m_iSectionIndex;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates subdocument reader of specified type.
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordHeaderFooterWriter(WordWriter mainWriter)
            : base(mainWriter)
        {
            m_type = WordSubdocument.HeaderFooter;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets type of subdocumnet writer
        /// </summary>
        internal HeaderType HeaderType
        {
            get
            {
                return m_headerType;
            }

            set
            {
                if (value < m_headerType)
                {
                    throw new ArgumentOutOfRangeException(string.Format("HeaderType must be greater from {0}",
                      m_headerType));
                }

                ClosePrevHeaderTypes(value);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Writes the end of the document
        /// </summary>
        public override void WriteDocumentEnd()
        {
            ClosePrevHeaderTypes((HeaderType)6);
            WriteChar(SpecialCharacters.ParagraphEnd);
            int currTextPos = GetTextPos();
            m_docInfo.TablesData.HeaderPositions[m_iItemIndex] = currTextPos + 3;
        }

        /// <summary>
        /// Writes the end of the section
        /// </summary>
        internal void WriteSectionEnd()
        {
            HeaderType = (HeaderType)6;
            m_iSectionIndex++;
            m_iItemIndex = (m_iSectionIndex + 1) * 6 + 1;
            m_headerType = (HeaderType)0;
        }

        /// <summary>
        /// Closes the previous separator types.
        /// </summary>
        internal void ClosePrevSeparator()
        {
            int pos = GetTextPos();
            if (pos != m_docInfo.TablesData.HeaderPositions[m_iItemIndex - 1])
            {
                WriteChar(SpecialCharacters.ParagraphEnd);
                int currTextPos = GetTextPos();
                m_docInfo.TablesData.HeaderPositions[m_iItemIndex] = currTextPos;
            }
            else
            {
                m_docInfo.TablesData.HeaderPositions[m_iItemIndex] = pos;
            }
            ++m_iItemIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerType"></param>
        protected void ClosePrevHeaderTypes(HeaderType headerType)
        {
            while (m_headerType != headerType)
            {
                int pos = GetTextPos();

                if (pos != m_docInfo.TablesData.HeaderPositions[m_iItemIndex - 1])
                {
                    WriteChar(SpecialCharacters.ParagraphEnd);

                    ++m_headerType;

                    int currTextPos = GetTextPos();
                    m_docInfo.TablesData.HeaderPositions[m_iItemIndex] = currTextPos;
                }
                else
                {
                    ++m_headerType;

                    int currTextPos = GetTextPos();
                    m_docInfo.TablesData.HeaderPositions[m_iItemIndex] = currTextPos;
                }

                ++m_iItemIndex;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpHdr += dataLength;
        }

        /// <summary>
        /// Inintializes class members
        /// </summary>
        protected override void InitClass()
        {
            base.InitClass();

            int secCount = m_docInfo.FkpData.SepxAddedCount;
            m_docInfo.TablesData.HeaderPositions = new int[7 + secCount * 6 + 1];
            WriteHeaderFooterHead();
            m_headerType = (HeaderType)0;
            m_curTxbxId = 4050;
            m_curPicId = 4500;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void WriteHeaderFooterHead()
        {
            m_iItemIndex = 1;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordAnnotationWriter : WordSubdocumentWriter
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordAnnotationWriter"/> class.
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordAnnotationWriter(WordWriter mainWriter)
            : base(mainWriter)
        {
            m_type = WordSubdocument.Annotation;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Writes the end of the document
        /// </summary>
        public override void WriteDocumentEnd()
        {
            m_docInfo.TablesData.Annotations.AddTxtPosition(m_docInfo.FibData.ccpAtn);
            m_docInfo.TablesData.Annotations.AddTxtPosition(m_docInfo.FibData.ccpAtn + 3);
            WriteChar(SpecialCharacters.ParagraphEnd);
        }

        /// <summary>
        /// </summary>
        public override void WriteItemStart()
        {
            m_docInfo.TablesData.Annotations.AddTxtPosition(m_docInfo.FibData.ccpAtn);
            WriteMarker(WordChunkType.Annotation);
        }

        /// <summary>
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpAtn += dataLength;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for footnote subdocument writer
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordEndnoteWriter : WordSubdocumentWriter
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordEndnoteWriter"/> class.
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordEndnoteWriter(WordWriter mainWriter)
            : base(mainWriter)
        {
            m_type = WordSubdocument.Endnote;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// </summary>
        public override void WriteItemStart()
        {
            m_docInfo.TablesData.Endnotes.AddTxtPosition(m_docInfo.FibData.ccpEdn);
        }

        /// <summary>
        /// Writes the end of the document
        /// </summary>
        public override void WriteDocumentEnd()
        {
            m_docInfo.TablesData.Endnotes.AddTxtPosition(m_docInfo.FibData.ccpEdn);
            m_docInfo.TablesData.Endnotes.AddTxtPosition(m_docInfo.FibData.ccpEdn + 3);
            WriteChar(SpecialCharacters.ParagraphEnd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpEdn += dataLength;
        }
        #endregion
    }

    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordTextBoxWriter : WordSubdocumentWriter
    {
        #region Class constants
        /// <summary>
        /// Textbox reserved data
        /// </summary>
        protected const uint DEF_TEXTBOX_RESERVED_DATA = 4294967295;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected int m_lastTxbxPosition = 0;

        /// <summary>
        /// BreakDescriptor counter
        /// </summary>
        protected int m_txbxBkDCnt;

        /// <summary>
        /// 
        /// </summary>
        protected long m_dataPosition;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordTextBoxWriter"/> class.
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordTextBoxWriter(WordWriter mainWriter)
            : base(mainWriter)
        {
            m_type = WordSubdocument.TextBox;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Writes the end of the document
        /// </summary>
        public override void WriteDocumentEnd()
        {
            //Write paragraph ends
            for (int i = 0; i < 2; i++)
            {
                WriteChar(SpecialCharacters.ParagraphEnd);
            }

            AddNewTxbx(true, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteItemEnd()
        {
            base.WriteItemEnd();
            AddNewTxbx(false, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid">Spid of textbox.</param>
        internal void WriteTextBoxEnd(int spid)
        {
            base.WriteMarker(WordChunkType.ParagraphEnd);
            AddNewTxbx(false, spid);
        }

        /// <summary>
        /// Increase ccp on the defined length
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpTxbx += dataLength;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLast"></param>
        /// <param name="spid"></param>    
        protected virtual void AddNewTxbx(bool isLast, int spid)
        {
            TextBoxStoryDescriptor txbxStory = new TextBoxStoryDescriptor();
            BreakDescriptor bkd = new BreakDescriptor();

            txbxStory.TextBoxCnt = 1;
            if (!isLast)
            {
                bkd.Ipgd = (short)m_txbxBkDCnt;
                bkd.Options = 16;
                txbxStory.ShapeIdent = spid;
                txbxStory.Reserved = DEF_TEXTBOX_RESERVED_DATA;
            }
            else
            {
                bkd.Ipgd = -1;
                bkd.Options = 0;
                txbxStory.ShapeIdent = 0;
            }

            m_docInfo.TablesData.ArtObj.AddTxbx(WordSubdocument.Main, txbxStory, bkd, m_lastTxbxPosition);
            m_lastTxbxPosition = m_docInfo.FibData.ccpTxbx;
            m_txbxBkDCnt++;
        }
        #endregion
    }

    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordHFTextBoxWriter : WordTextBoxWriter
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Creates subdocument reader of specified type.
        /// </summary>
        /// <param name="mainWriter"></param>
        internal WordHFTextBoxWriter(WordWriter mainWriter)
            : base(mainWriter)
        {
            m_type = WordSubdocument.HeaderTextBox;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Write textbox "end"
        /// </summary>
        internal void WriteHFTextBoxEnd(int spid)
        {
            base.WriteMarker(WordChunkType.ParagraphEnd);
            AddNewTxbx(false, spid);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Increase header/footer ccp
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpHdrTxbx += dataLength;
        }

        /// <summary>
        /// Create TxbxStory and BKD and add it to artObjects
        /// </summary>
        protected override void AddNewTxbx(bool isLast, int spid)
        {
            TextBoxStoryDescriptor txbxStory = new TextBoxStoryDescriptor();
            BreakDescriptor bkd = new BreakDescriptor();

            txbxStory.TextBoxCnt = 1;
            if (!isLast)
            {
                bkd.Ipgd = (short)m_txbxBkDCnt;
                bkd.Options = 16;
                txbxStory.ShapeIdent = spid;
                txbxStory.Reserved = DEF_TEXTBOX_RESERVED_DATA;
            }
            else
            {
                bkd.Ipgd = -1;
                bkd.Options = 0;
            }

            m_docInfo.TablesData.ArtObj.AddTxbx(WordSubdocument.HeaderFooter, txbxStory, bkd, m_lastTxbxPosition);
            m_lastTxbxPosition = m_docInfo.FibData.ccpHdrTxbx;
            m_txbxBkDCnt++;
        }
        #endregion
    }
}