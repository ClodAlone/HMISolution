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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;
using Syncfusion.DocIO.ReaderWriter.Security;
using Syncfusion.Documentation;
using Syncfusion.Layouting;
using Syncfusion.DocIO.DLS.Entities;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Syncfusion.CompoundFile.DocIO.Net;
using Syncfusion.CompoundFile.DocIO;
#else
using Image = System.Drawing.Image;
using Syncfusion.CompoundFile.DocIO.Native;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.CompoundFile.DocIO.Net;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif

#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for WordWriter.
    /// </summary>
    [CLSCompliant(false)]
    [DocumentationExclude()]
    internal class WordWriter
      : WordWriterBase,
        IWordWriter,
        IDisposable
    {
        #region Class members
        private bool m_bHeaderWritten = false;

        /// <summary>
        /// Is Last ParagraphEnd for the subdocument
        /// </summary>
        private bool m_bLastParagrapfEnd = false;

        /// <summary>
        /// 
        /// </summary>
        //private bool m_bDisposed = false;
        //private bool m_bDestroyStream = false;
        private SectionProperties m_secProperties = null;

        ///// <summary>
        ///// 
        ///// </summary>
        //private IWordSubdocumentWriter m_ftnWriter;
        ///// <summary>
        ///// 
        ///// </summary>
        //private IWordSubdocumentWriter m_hfWriter;
        ///// <summary>
        ///// 
        ///// </summary>
        //private IWordSubdocumentWriter m_atnWriter;
        ///// <summary>
        ///// TextBox writer
        ///// </summary>
        //private IWordSubdocumentWriter m_txBxWriter;
        ///// <summary>
        ///// Header/footer textbox wtiter
        ///// </summary>
        //private IWordSubdocumentWriter m_hfTxBxWriter;
        ///// <summary>
        ///// 
        ///// </summary>
        //private IWordSubdocumentWriter m_ednWriter;
        /// <summary>
        /// 
        /// </summary>
        private IWordSubdocumentWriter m_lastWriter;
        /// <summary>
        /// Document summary properties members
        /// </summary>
        private BuiltinDocumentProperties m_builtinProp = new BuiltinDocumentProperties();

        /// <summary>
        /// 
        /// </summary>
        private CustomDocumentProperties m_customProp = new CustomDocumentProperties();
        /// <summary>
        /// 
        /// </summary>
        private bool m_isTemplate;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsWriteProtected;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bHasPicture;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets section properties 
        /// </summary>
        public SectionProperties SectionProperties
        {
            get
            {
                return m_secProperties;
            }
        }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //public MainStatePositions StatePositions
        //{
        //  get
        //  {
        //    return (MainStatePositions)m_statePositions;
        //  }
        //}
        /// <summary>
        /// Get/sets name of the author of the application.
        /// </summary>
        public BuiltinDocumentProperties BuiltinDocumentProperties
        {
            get
            {
                return m_builtinProp;
            }

            set
            {
                m_builtinProp = value;
            }
        }

        /// <summary>
        /// Get/sets name of the author of the application.
        /// </summary>
        public CustomDocumentProperties CustomDocumentProperties
        {
            get
            {
                return m_customProp;
            }

            set
            {
                m_customProp = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DOPDescriptor DOP
        {
            get
            {
                return m_docInfo.TablesData.DOP;
            }

            set
            {
                m_docInfo.TablesData.DOP = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GrammarSpelling GrammarSpellingData
        {
            get
            {
                return m_docInfo.TablesData.GrammarSpellingData;
            }

            set
            {
                m_docInfo.TablesData.GrammarSpellingData = value;
            }
        }

        /// <summary>
        /// Get/set document Macros
        /// </summary>
        public MemoryStream MacrosStream
        {
            get
            {
                return m_streamsManager.MacrosStream;
            }

            set
            {
                m_streamsManager.MacrosStream = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MemoryStream ObjectPoolStream
        {
            get
            {
                return m_streamsManager.ObjectPoolStream;
            }

            set
            {
                m_streamsManager.ObjectPoolStream = value;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public MemoryStream MsoDataStore
        //{
        //    get
        //    {
        //        return m_streamsManager.MsoDataStore;
        //    }
        //    set
        //    {
        //        m_streamsManager.MsoDataStore = value;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public byte[] MacroCommands
        {
            get
            {
                return m_docInfo.TablesData.MacroCommands;
            }

            set
            {
                m_docInfo.TablesData.MacroCommands = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Variables
        {
            get
            {
                return m_docInfo.TablesData.Variables;
            }

            set
            {
                m_docInfo.TablesData.Variables = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StandardAsciiFont
        {
            get
            {
                return m_docInfo.TablesData.StandardAsciiFont;
            }

            set
            {
                m_docInfo.TablesData.StandardAsciiFont = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StandardFarEastFont
        {
            get
            {
                return m_docInfo.TablesData.StandardFarEastFont;
            }

            set
            {
                m_docInfo.TablesData.StandardFarEastFont = value;
            }
        }
        /// <summary>
        /// Gets and sets the standard/default bidi font
        /// </summary>
        public string StandardBidiFont
        {
            get
            {
                return m_docInfo.TablesData.StandardBidiFont;
            }

            set
            {
                m_docInfo.TablesData.StandardBidiFont = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string StandardNonFarEastFont
        {
            get
            {
                return m_docInfo.TablesData.StandardNonFarEastFont;
            }

            set
            {
                m_docInfo.TablesData.StandardNonFarEastFont = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the document is template.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is template; otherwise, <c>false</c>.
        /// </value>
        internal bool IsTemplate
        {
            get
            {
                return m_isTemplate;
            }
            set
            {
                m_isTemplate = value;
            }
        }
        /// <summary>
        /// Gets or sets the associated strings.
        /// </summary>
        /// <value>The associated strings.</value>
        public byte[] AssociatedStrings
        {
            get
            {
                return m_docInfo.TablesData.AsociatedStrings;
            }

            set
            {
                m_docInfo.TablesData.AsociatedStrings = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the document is write protected.
        /// </summary>
        /// <value><c>true</c> if [write protected]; otherwise, <c>false</c>.</value>
        internal bool WriteProtected
        {
            get
            {
                return m_bIsWriteProtected;
            }
            set
            {
                m_bIsWriteProtected = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool HasPicture
        {
            get
            {
                return m_bHasPicture;
            }
            set
            {
                m_bHasPicture = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates instance from specified stream
        /// </summary>
        public WordWriter(Stream stream)
        {
            //m_memStream = stream;
            //m_stgStream = StgStream.CreateStorageOnILockBytes();
            m_streamsManager = new StreamsManager(stream, true);
            InitClass();
        }
        /// <summary>
        /// Creates instance from specified file name
        /// </summary>
        public WordWriter(string fileName)
        {
            //m_stgStream = StgStream.CreateStorage( fileName );
            m_streamsManager = new StreamsManager(fileName, true);
            InitClass();
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Writes header of the document to word file
        /// </summary>
        public void WriteDocumentHeader()
        {
            m_bHeaderWritten = true;
            AddSepxProperties();
        }

        /// <summary>
        /// Gets the subdocument writer of the specified type
        /// </summary>
        /// <returns></returns>
        public IWordSubdocumentWriter GetSubdocumentWriter(WordSubdocument subDocumentType)
        {
            if (!m_bHeaderWritten)
            {
                throw new InvalidOperationException("Call WriteDocumentHeader before this method");
            }

            if (!m_bLastParagrapfEnd)
            {
                // Complete document
                WriteMarker(WordChunkType.ParagraphEnd);
                m_bLastParagrapfEnd = true;
            }

            switch (subDocumentType)
            {
                case WordSubdocument.Footnote:
                    return m_lastWriter = new WordFootnoteWriter(this);
                case WordSubdocument.HeaderFooter:
                    return m_lastWriter = new WordHeaderFooterWriter(this);
                case WordSubdocument.Annotation:
                    return m_lastWriter = new WordAnnotationWriter(this);
                case WordSubdocument.Endnote:
                    return m_lastWriter = new WordEndnoteWriter(this);
                case WordSubdocument.TextBox:
                    return m_lastWriter = new WordTextBoxWriter(this);
                case WordSubdocument.HeaderTextBox:
                    return m_lastWriter = new WordHFTextBoxWriter(this);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override void WriteChunk(string textChunk)
        {
            if (!m_bHeaderWritten)
            {
                throw new InvalidOperationException("Call WriteDocumentHeader before this method");
            }

            base.WriteChunk(textChunk);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override void WriteSafeChunk(string textChunk)
        {
            if (!m_bHeaderWritten)
            {
                throw new InvalidOperationException("Call WriteDocumentHeader before this method");
            }

            base.WriteSafeChunk(textChunk);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkType"></param>
        public override void WriteMarker(WordChunkType chunkType)
        {
            if (!m_bHeaderWritten)
            {
                throw new InvalidOperationException("Call WriteDocumentHeader before this method");
            }

            base.WriteMarker(chunkType);

            switch (chunkType)
            {
                case WordChunkType.SectionEnd:
                    WriteChar(SpecialCharacters.PageBreak);
                    AddSepxProperties();
                    break;
                case WordChunkType.ColumnBreak:
                    WriteChar(SpecialCharacters.ColumnBreak);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteDocumentEnd(string password)
        {
            m_docInfo.FibData.IsDocumentTemplate = m_isTemplate;
            m_docInfo.FibData.fReadOnlyRecommended = WriteProtected;
            m_docInfo.FibData.fHasPic = this.HasPicture;
            CompleteMainStream();

            if (!string.IsNullOrEmpty(password))
            {
                byte[] zeroBuff = new byte[Constants.BytesInInt + 16 + 16 + 16];
                m_streamsManager.TableStream.Position = 0;
                m_streamsManager.TableStream.Write(zeroBuff, 0, zeroBuff.Length);
                m_docInfo.FibData.fEncrypted = true;
            }

            WriteTables();
            WriteFib(); // must be written after Main & Table stream

            WriteSummary();

            if (!string.IsNullOrEmpty(password))
            {
                WordDecryptor wDecryptor = new WordDecryptor(m_streamsManager.TableStream, (MemoryStream)m_streamsManager.MainStream, m_streamsManager.DataStream, m_docInfo.FibData);
                wDecryptor.Encrypt(password);
                m_streamsManager.UpdateStreams(wDecryptor.MainStream, wDecryptor.TableStream, wDecryptor.DataStream);
            }

            m_streamsManager.SaveStg();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertComment(WCommentFormat format)
        {
            AnnotationDescriptor antDesc = new AnnotationDescriptor();
            int index = m_docInfo.TablesData.Annotations.AddGXAO(format.User);
            int pos = GetTextPos();
            antDesc.UserInitials = format.UserInitials;
            antDesc.IndexToGrpOwner = (short)index;
            antDesc.TagBkmk = format.TagBkmk;
            m_docInfo.TablesData.Annotations.AddDescriptor(antDesc, pos,
              pos - format.BookmarkStartOffset,
              pos + format.BookmarkEndOffset);
            WriteMarker(WordChunkType.Annotation);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertFootnote(WFootnote footnote)
        {
            int pos = GetTextPos();
            if (footnote.FootnoteType == FootnoteType.Footnote)
                m_docInfo.TablesData.Footnotes.AddReferense(pos, footnote.IsAutoNumbered);
            else
                m_docInfo.TablesData.Endnotes.AddReferense(pos, footnote.IsAutoNumbered);

            //      CharacterPropertiesConverter.FormatToCHP(footnote.MarkerCharacterFormat, m_charProps);

            if (footnote.IsAutoNumbered)
            {
                WriteMarker(WordChunkType.Footnote);
            }
            else if (footnote.CustomMarkerIsSymbol)
            {
                //CharacterProperties.Special = true;
                CharacterProperties.FontNameAscii = footnote.SymbolFontName;
                SymbolDescriptor symbolDescriptor = new SymbolDescriptor();
                symbolDescriptor.CharCode = footnote.SymbolCode;
                symbolDescriptor.FontCode = (short)StyleSheet.FontNameToIndex(footnote.SymbolFontName);

                CharacterProperties.Symbol = symbolDescriptor;
                WriteMarker(WordChunkType.Symbol);
                //CharacterProperties.Special = false;
            }
            else
            {
                WriteString(footnote.CustomMarker);
            }
        }

        /// <summary>
        /// Writes marker to word file
        /// </summary>
        public void InsertPageBreak()
        {
            Stream stream = m_streamsManager.MainStream;
            byte[] arrBuffer = m_docInfo.FibData.Encoding.GetBytes(SpecialCharacters.PageBreakStr);
            stream.Write(arrBuffer, 0, arrBuffer.Length);

            //CharacterPropertyException chpx = m_charProps.CloneChpx();
            //m_docInfo.FkpData.AddChpxProperties((uint)stream.Position, chpx);
            AddChpxProperties(false);

            IncreaseCcp(1);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void CompleteMainStream()
        {
            // Close last paragraph for subdocument
            if (m_lastWriter != null)
            {
                m_lastWriter.WriteMarker(WordChunkType.ParagraphEnd);
            }
            // ------------------------------------

            // Close last paragraph for main document
            if (!m_bLastParagrapfEnd)
            {
                WriteMarker(WordChunkType.ParagraphEnd);
                m_bLastParagrapfEnd = true;
            }
            // ------------------------------------

            // Calculate fcMac
            m_docInfo.FibData.UpdateFcMac();

            while (m_streamsManager.MainStream.Position < m_docInfo.FibData.fcMac)
            {
                m_streamsManager.MainWriter.Write((byte)0);
            }

            // ------------------------------------

            // Writes FKPs (rewrite last Papx for document)
            uint pos = (uint)(m_iStartText + m_docInfo.FibData.ccpText * m_docInfo.FibData.EncodingCharSize);
            m_docInfo.FkpData.CloneAndAddLastPapx(pos);
            m_docInfo.FkpData.CloneAndAddLastChpx(pos);
            m_docInfo.FkpData.Write(m_streamsManager.MainStream);

            // Write Esher
            if (Escher != null)
            {
                Escher.WriteContainersData(m_streamsManager.MainStream);
            }

            m_docInfo.FibData.cbMac = (int)m_streamsManager.MainStream.Position;
        }

        /// <summary>
        /// Writes the zero block.
        /// </summary>
        /// <param name="size">The size.</param>
        private void WriteZeroBlock(int size)
        {
            byte[] zeroBuf = new byte[size];

            for (int i = 0; i < zeroBuf.Length; i++)
            {
                zeroBuf[i] = 0;
            }

            m_streamsManager.MainWriter.Write(zeroBuf, 0, zeroBuf.Length);
        }

        /// <summary>
        /// Writes the tables.
        /// </summary>
        private void WriteTables()
        {
            m_docInfo.TablesData.AddStyleSheetTable(StyleSheet);
            m_docInfo.TablesData.Write(m_streamsManager.TableStream, m_lastWriter != null);
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteFib()
        {
            m_streamsManager.MainStream.Position = 0;
            m_docInfo.FibData.Write(m_streamsManager.MainStream);
        }

        /// <summary>
        /// Create Summary information for document.
        /// </summary>
        private void WriteSummary()
        {
            Guid s_guidSummary = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");
            Guid s_guidDocument = new Guid("D5CDD502-2E9C-101B-9397-08002B2CF9AE");
            Guid s_guidCustom = new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

            DocumentPropertyCollection summary = new DocumentPropertyCollection();
            PropertySection summarySection = new PropertySection(s_guidSummary, -1);
            summary.Sections.Add(summarySection);

            DocumentPropertyCollection documentSummary = new DocumentPropertyCollection();
            PropertySection customSection = new PropertySection(s_guidCustom, -1);
            PropertySection documentSummarySection = new PropertySection(s_guidDocument, -1);
            documentSummary.Sections.Add(documentSummarySection);
            documentSummary.Sections.Add(customSection);

            WriteProps(summarySection, m_builtinProp.SummaryHash.Values);
            WriteProps(documentSummarySection, m_builtinProp.DocumentHash.Values);
            WriteProps(customSection, m_customProp.CustomHash.Values);

            summary.Serialize(StreamsManager.SummaryInfoStream);
            documentSummary.Serialize(StreamsManager.DocumentSummaryInfoStream);

        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="section"></param>
       /// <param name="values"></param>
        private void WriteProps(PropertySection section, ICollection values)
        {
            int iPropertyId = 2;

            foreach (DocumentProperty property in values)
            {
                PropertyData data = ConvertToPropertyData(property, iPropertyId);
                section.Properties.Add(data);
                iPropertyId++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="iPropertyId"></param>
        /// <returns></returns>
        private PropertyData ConvertToPropertyData(DocumentProperty property, int iPropertyId)
        {
            PropertyData result = new PropertyData();
            property.FillPropVariant(result, iPropertyId);
#if !SILVERLIGHT && !WP && AllowUnsafeCode
            if (property.Value != null && property.PropertyType == Syncfusion.CompoundFile.DocIO.PropertyType.Empty)
            {
                if (property.Value is string)
                    result.Type = VarEnum.VT_LPWSTR;
            }
#endif
            if (property.InternalName != null)
                result.Name = property.InternalName;

            return result;
        }
        /// <summary>
        /// Adds the sepx properties.
        /// </summary>
        private void AddSepxProperties()
        {
            int pos = GetTextPos();
            SectionPropertyException sepx = SectionProperties.CloneSepx();
            m_docInfo.FkpData.AddSepxProperties(pos, sepx);
            m_secProperties = new SectionProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Close()
        {
            m_builtinProp = null;
            m_customProp = null;
            m_secProperties = null;
            m_docInfo.Close();
            m_docInfo = null;
            m_styleSheet = null;
            m_charProps = null;
            m_paragProps = null;
            m_breakCharProps = null;
            m_listProperties = null;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        protected override void InitClass()
        {
            base.InitClass();
            m_type = WordSubdocument.Main;
            m_secProperties = new SectionProperties();
            m_docInfo = new DocInfo(m_streamsManager);
            m_streamsManager.MainStream.Seek(m_docInfo.FibData.fcMin, SeekOrigin.Begin);
            m_iStartText = (int)m_streamsManager.MainStream.Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLength"></param>
        protected override void IncreaseCcp(int dataLength)
        {
            m_docInfo.FibData.ccpText += dataLength;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            //WordWriter.Dispose implementation
            //if(m_bDisposed) return;

            //m_bDisposed = true;

            //if(m_bDestroyStream)
            //{
            //  m_streamsManager.Storage.Dispose();
            //}

            //m_stgStream = null;
        }
        #endregion

    }

    /// <summary>
    /// Summary description for WordWriterBase.
    /// </summary>
    [CLSCompliant(false)]
    [DocumentationExclude()]
    internal abstract class WordWriterBase : IWordWriterBase
    {
        #region Class constants
        /// <summary>
        /// Fieldshape type value. 
        /// </summary>
        private const int DEF_FIELDSHAPETYPE_VAL = 2;
        #endregion

        #region Class members
        public StreamsManager m_streamsManager;
        public DocInfo m_docInfo;
        protected WordStyleSheet m_styleSheet;
        protected int m_nextPicLocation = 0;
        protected CharacterProperties m_charProps = null;
        protected ParagraphProperties m_paragProps = null;
        protected CharacterProperties m_breakCharProps = null;
        protected ListProperties m_listProperties = null;
        private Stack<FieldDescriptor> m_endStack = new Stack<FieldDescriptor>();
        private int m_iCountCell = 0;
        private int m_currStyleIndex = 0;
        protected int m_curTxbxId;
        protected int m_curPicId;
        private int m_curTxid;
        protected int m_textColIndex;
        protected int m_iStartText;
        protected BinaryWriter m_textWriter;

        /// <summary>
        /// 
        /// </summary>
        protected WordSubdocument m_type;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets stylesheet
        /// </summary>
        public WordStyleSheet StyleSheet
        {
            get
            {
                return m_styleSheet;
            }
        }

        /// <summary>
        /// Gets/sets current style index
        /// </summary>
        public int CurrentStyleIndex
        {
            get
            {
                return m_currStyleIndex;
            }

            set
            {
                if (m_currStyleIndex != value)
                {
                    if (value < 0 || value > m_styleSheet.StylesCount - 1)
                    {
                        throw
                          new ArgumentOutOfRangeException(
                            "CurrentStyleIndex",
                            string.Format("value must be between 0 and {0}", m_styleSheet.StylesCount - 1));
                    }

                    m_currStyleIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets character properties
        /// </summary>
        public CharacterProperties CharacterProperties
        {
            get
            {
                return m_charProps;
            }

            set
            {
                m_charProps = value;
            }
        }

        /// <summary>
        /// Gets/sets paragraph properties
        /// </summary>
        public ParagraphProperties ParagraphProperties
        {
            get
            {
                return m_paragProps;
            }

            set
            {
                m_paragProps = value;
            }
        }

        /// <summary>
        /// Gets/sets character properties for paragraph end symbol.
        /// </summary>
        /// <value></value>
        public CharacterProperties BreakCharProperties
        {
            get
            {
                return m_breakCharProps;
            }

            set
            {
                m_breakCharProps = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ListProperties ListProperties
        {
            get
            {
                if (m_listProperties == null)
                {
                    m_listProperties = new ListProperties(m_docInfo.TablesData.ListInfo, m_paragProps);
                }
                return m_listProperties;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EscherClass Escher
        {
            get
            {
                return m_docInfo.TablesData.Escher;
            }

            set
            {
                m_docInfo.TablesData.Escher = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StreamsManager StreamsManager
        {
            get
            {
                return m_streamsManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BinaryWriter MainWriter
        {
            get
            {
                if (m_textWriter == null)
                {
                    m_textWriter = new BinaryWriter(m_streamsManager.MainStream, m_docInfo.FibData.Encoding);
                }
                return m_textWriter;
            }
        }

        /// <summary>
        /// Gets the get next text id.
        /// </summary>
        /// <value>The get next text id.</value>
        internal int NextTextId
        {
            get
            {
                m_curTxid += MsofbtSpContainer.DEF_TXID_INCREMENT;
                return m_curTxid;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Writes text chunk to word file 
        /// </summary>
        /// <returns></returns>
        public virtual void WriteChunk(string textChunk)
        {
            bool bCharStick = CharacterProperties.StickProperties;
            bool bParStick = ParagraphProperties.StickProperties;
            bool bBreakCharStick = BreakCharProperties.StickProperties;
            bool bInTable = ParagraphProperties.IsCellMark;
            bool bIsSubCell = ParagraphProperties.IsSubCell;
            bool bIsSubRow = ParagraphProperties.IsSubRow;

            textChunk = textChunk.Replace("\r\n", "\r");
            textChunk = textChunk.Replace('\n', '\r');
            string[] textChunks = textChunk.Split("\r".ToCharArray());

            int count = textChunks.Length;
            if (count > 1)
            {
                CharacterProperties.StickProperties = true;
                ParagraphProperties.StickProperties = true;
                BreakCharProperties.StickProperties = true;

                if (bIsSubCell)
                    ParagraphProperties.Sprms.RemoveValue(WordSprmOptions.sprmPSubTableCellEnd);
                if (bIsSubRow)
                    ParagraphProperties.Sprms.RemoveValue(WordSprmOptions.sprmPSubTableRowEnd);
            }

            for (int i = 0; i < count; i++)
            {
                WriteString(textChunks[i]);

                if (i < count - 1)
                {
                    WriteChar(SpecialCharacters.ParagraphEnd);
                }
            }

            if (count > 1)
            {
                if (!bCharStick)
                    CharacterProperties.Sprms.Clear();
                //if(!bParStick)        
                //  ParagraphProperties.Sprms.Clear();

                if (bIsSubCell)
                    ParagraphProperties.IsSubCell = true;
                if (bIsSubRow)
                    ParagraphProperties.IsSubRow = true;
            }

            CharacterProperties.StickProperties = bCharStick;
            ParagraphProperties.StickProperties = bParStick;
            BreakCharProperties.StickProperties = bBreakCharStick;
            if (bInTable)
            {
                ParagraphProperties.IsCellMark = bInTable;
            }
            //      bool bCharStick = CharacterProperties.StickProperties;
            //      bool bParStick = ParagraphProperties.StickProperties;
            //      bool bInTable = ParagraphProperties.IsCellMark;
            //
            //      textChunk = textChunk.Replace( "\r\n", "\n" );
            //      char[] splitter = new char[ 1 ] { '\n' };
            //      string[] chunks = textChunk.Split( splitter );
            //      int cnt = chunks.Length - 1;
            //
            ////      int cnt = 0;
            ////      char[] symbol = new char[ 1 ]{ '\n'};
            ////      string[] chunks = new string[ 1 ]{ textChunk };
            ////      if( textChunk.IndexOfAny( symbol ) != -1 )
            ////      {
            ////        char[] splitter = new char[ 1 ] { '\n' };
            ////        chunks = textChunk.Split( splitter );
            ////        cnt = chunks.Length - 1;
            ////      }
            //
            //      if ( cnt > 0 )
            //      {
            //        CharacterProperties.StickProperties = true;
            //        ParagraphProperties.StickProperties = true;
            //      }
            //
            //      for( int i = 0; i < cnt; i++ )
            //      {
            //        WriteString( chunks[ i ] );
            //        if( i < cnt - 1 )
            //        {
            //          WriteChar( SpecialCharacters.ParagraphEnd );
            //        }        
            //      }
            //      
            //      WriteString( chunks[ cnt ] );
            //
            //      if( !bCharStick )
            //      {
            //        CharacterProperties.Sprms.Clear();
            //      }
            //
            //      if( cnt > 1 )
            //      {
            //        CharacterProperties.StickProperties = bCharStick;
            //        ParagraphProperties.StickProperties = bParStick;
            //      }
            //
            //      if( bInTable )
            //      {
            //        ParagraphProperties.IsCellMark = bInTable;
            //      }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textChunk"></param>
        public virtual void WriteSafeChunk(string textChunk)
        {
            bool bInTable = ParagraphProperties.IsCellMark;

            WriteString(textChunk);

            if (!CharacterProperties.StickProperties)
            {
                CharacterProperties.Sprms.Clear();
            }

            if (bInTable)
            {
                ParagraphProperties.IsCellMark = bInTable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nestingLevel"></param>
        public void WriteCellMark(int nestingLevel)
        {
            if (nestingLevel == 1)
            {
                WriteMarker(WordChunkType.TableCell);
            }
            else
            {
                ParagraphProperties.IsCellMark = true;
                ParagraphProperties.TablesNestingLevel = nestingLevel;
                //Add the sprmPSubTableCellEnd at the beging of SPRM collection to avoid table corruption issue.
                //Need to further analyze on the exact cause of table corruption issue.
                if (ParagraphProperties.Sprms.HasSprm(WordSprmOptions.sprmPSubTableCellEnd))
                    ParagraphProperties.Sprms.RemoveValue(WordSprmOptions.sprmPSubTableCellEnd);
                SinglePropertyModifierRecord subCellEndSprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmPSubTableCellEnd);
                subCellEndSprm.BoolValue = true;
                ParagraphProperties.Sprms.InsertAt(subCellEndSprm, 0);
                //WriteChar(SpecialCharacters.ParagraphEnd);
                WriteNestedMark();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nestingLevel"></param>
        /// <param name="cellCount"></param>
        public void WriteRowMark(int nestingLevel, int cellCount)
        {
            if (nestingLevel == 1)
            {
                m_iCountCell = cellCount;
                WriteMarker(WordChunkType.TableRow);
            }
            else
            {
                if (!ParagraphProperties.PresentRowDescriptor)
                {
                    ParagraphProperties.TableRowProperties = CreateTableRowDescriptor(cellCount);
                }

                if (!ParagraphProperties.PresentTableBorders)
                {
                    ParagraphProperties.TableBorders = CreateTableBorders();
                }

                ParagraphProperties.IsCellMark = true;
                ParagraphProperties.TablesNestingLevel = nestingLevel;
                ParagraphProperties.IsSubCell = true;
                ParagraphProperties.IsSubRow = true;
                //WriteChar(SpecialCharacters.ParagraphEnd);
                WriteNestedMark();
                ParagraphProperties.IsCellMark = false;
            }
        }

        /// <summary>
        /// Writes marker to word file
        /// </summary>
        /// <param name="chunkType"></param>
        public virtual void WriteMarker(WordChunkType chunkType)
        {
            switch (chunkType)
            {
                case WordChunkType.Text:
                    break;
                case WordChunkType.DocumentEnd:
                    break;
                case WordChunkType.ParagraphEnd:
                    WriteChar(SpecialCharacters.ParagraphEnd);
                    break;
                case WordChunkType.PageBreak:
                    WriteChar(SpecialCharacters.PageBreak);
                    break;
                case WordChunkType.Table:
                    WriteChar(SpecialCharacters.TableAscii);
                    break;
                case WordChunkType.TableCell:
                    ParagraphProperties.IsCellMark = true;
                    WriteChar(SpecialCharacters.TableAscii);
                    ++m_iCountCell;
                    break;
                case WordChunkType.TableRow:
                    if (!ParagraphProperties.PresentRowDescriptor)
                    {
                        ParagraphProperties.TableRowProperties = CreateTableRowDescriptor();
                    }

                    if (!ParagraphProperties.PresentTableBorders)
                    {
                        ParagraphProperties.TableBorders = CreateTableBorders();
                    }

                    ParagraphProperties.IsCellMark = true;
                    ParagraphProperties.IsRowMark = true;
                    WriteChar(SpecialCharacters.TableAscii);
                    ParagraphProperties.IsCellMark = false;
                    m_iCountCell = 0;
                    break;
                case WordChunkType.FieldBeginMark:
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.FieldBeginMark);
                    CharacterProperties.Special = false;
                    break;
                case WordChunkType.FieldSeparator:
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.FieldSeparator);
                    CharacterProperties.Special = false;
                    break;
                case WordChunkType.FieldEndMark:
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.FieldEndMark);
                    CharacterProperties.Special = false;
                    break;
                case WordChunkType.Tab:
                    WriteChar(SpecialCharacters.TabAscii);
                    break;
                case WordChunkType.Annotation:
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.AnnotationAscii);
                    CharacterProperties.Special = false;
                    break;
                case WordChunkType.LineBreak:
                    WriteChar(SpecialCharacters.LineBreakAscii);
                    break;
                case WordChunkType.Image:
                    WriteChar(SpecialCharacters.ImageAscii);
                    break;
                case WordChunkType.Shape:
                    CharacterProperties.PicLocation = 0;
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.ShapeAscii);
                    CharacterProperties.Special = false;
                    break;
                case WordChunkType.Symbol:
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.SymbolAscii);
                    CharacterProperties.Special = false;
                    break;
                case WordChunkType.Footnote:
                    CharacterProperties.Special = true;
                    WriteChar(SpecialCharacters.FootnoteAscii);
                    CharacterProperties.Special = false;
                    break;
            }
        }

        /// <summary>
        /// Creates instance of TableRowDescriptor for current number of cells
        /// </summary>
        /// <returns></returns>
        public TableRowDescriptor CreateTableRowDescriptor()
        {
            return new TableRowDescriptor(m_iCountCell);
        }

        /// <summary>
        /// Creates instance of TableRowDescriptor for current number of cells
        /// </summary>
        /// <returns></returns>
        public TableRowDescriptor CreateTableRowDescriptor(int cellCount)
        {
            return new TableRowDescriptor(cellCount);
        }

        /// <summary>
        /// Creates instance of TableBorders
        /// </summary>
        /// <returns></returns>
        public TableBorders CreateTableBorders()
        {
            return new TableBorders();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldcode"></param>
        /// <param name="hasSeparator"></param>
        public void InsertStartField(string fieldcode, bool hasSeparator)
        {
            if ((fieldcode == null) || (fieldcode.Length == 0))
            {
                throw new ArgumentException("fieldcode must be present.");
            }
            bool bStick = CharacterProperties.StickProperties;
            CharacterProperties.StickProperties = true;


            int pos = GetTextPos();
            FieldDescriptor fld = WriteFieldStart(FieldTypeDefiner.GetFieldType(fieldcode));
            AddFieldDescriptor(fld, pos);
            // Write field code
            WriteSafeChunk(fieldcode);

            CharacterProperties.StickProperties = bStick;
            if (hasSeparator)
            {
                pos = GetTextPos();
                FieldDescriptor separatorFld = WriteFieldSeparator();
                AddFieldDescriptor(separatorFld, pos);
            }

            // Write end field descriptor to stack
            FieldDescriptor fldNew = new FieldDescriptor();
            fldNew.HasSeparator = hasSeparator;
            m_endStack.Push(fldNew);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldcode"></param>
        /// <param name="fieldType"></param>
        /// <param name="hasSeparator"></param>
        public void InsertStartField(string fieldcode, WField field, bool hasSeparator)
        {
            if ((fieldcode == null) || (fieldcode.Length == 0))
            {
                throw new ArgumentException("fieldcode must be present.");
            }
            bool bStick = CharacterProperties.StickProperties;
            CharacterProperties.StickProperties = true;

            int pos = GetTextPos();
            FieldDescriptor fld = WriteFieldStart(field.FieldType);
            if (field.FieldType == FieldType.FieldUnknown)
                fld.Type = (FieldType)field.SourceFieldType;
            AddFieldDescriptor(fld, pos);

            // Write field code
            if (!fieldcode.EndsWith("\"") && (field.NextSibling as WMergeField) == null &&
              string.IsNullOrEmpty(field.FieldCode))
                fieldcode += " ";
            WriteSafeChunk(fieldcode);
            //REF - HFD, PAGEREF-HFD, NOTEREF-HFD
            if (field.FieldType == FieldType.FieldRef || 
                field.FieldType==FieldType.FieldPageRef ||
                field.FieldType==FieldType.FieldNoteRef)
            {
                WriteNilPICFAndBinData(field);
            }

            CharacterProperties.StickProperties = bStick;
            if (hasSeparator)
            {
                pos = GetTextPos();
                FieldDescriptor separatorFld = WriteFieldSeparator();
                AddFieldDescriptor(separatorFld, pos);
            }

            // Write end field descriptor to stack
            FieldDescriptor fldNew = new FieldDescriptor();
            fldNew.HasSeparator = true; //hasSeparator;
            m_endStack.Push(fldNew);
        }
        /// <summary>
        /// Insert field separator.
        /// </summary>
        public void InsertFieldSeparator()
        {
            int pos = GetTextPos();
            FieldDescriptor separatorFld = WriteFieldSeparator();
            AddFieldDescriptor(separatorFld, pos);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertEndField()
        {
            if (m_endStack.Count > 0)
            {
                FieldDescriptor fld = m_endStack.Pop();
                fld.FieldBoundary = (byte)SpecialCharacters.FieldEndMark;
                fld.IsNested = (m_endStack.Count != 0);

                int pos = GetTextPos();
                AddFieldDescriptor(fld, pos);
                WriteFieldEnd();
            }
        }

        /// <summary>
        /// Inserts the field index entry.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        public void InsertFieldIndexEntry(string fieldCode)
        {
            bool bCharStick = CharacterProperties.StickProperties;
            CharacterProperties.StickProperties = true;
            WriteMarker(WordChunkType.FieldBeginMark);
            WriteSafeChunk(fieldCode);
            WriteMarker(WordChunkType.FieldEndMark);
            CharacterProperties.Sprms.Clear();
            CharacterProperties.StickProperties = bCharStick;
        }

        /// <summary>
        /// Insert field to the document 
        /// </summary>
        /// <param name="fieldcode"></param>
        /// <param name="value"></param>
        public void InsertField(string fieldcode, string value)
        {
            if (value != null)
            {
                InsertStartField(fieldcode, true);
            }
            else
            {
                InsertStartField(fieldcode, false);
            }
            WriteChunk(value);
            InsertEndField();
        }

        /// <summary>
        /// Insert field to the document 
        /// </summary>
        /// <param name="fieldcode"></param>
        /// <param name="formField"></param>
        public void InsertFormField(string fieldcode, FormField formField)
        {
            if ((fieldcode == null) || (fieldcode.Length == 0))
            {
                throw new ArgumentException("fieldcode must be present.");
            }

            bool bStick = CharacterProperties.StickProperties;
            CharacterProperties.StickProperties = true;

            bool hasSeparator = false;
            if (formField != null)
            {
                hasSeparator = formField.FormFieldType == FormFieldType.TextInput;

                if (formField.Params == 1)
                {
                    Debug.WriteLine("Parameters of form field is equal to 1 and formfield was forcibly set to have separator.");
                    hasSeparator = true;
                }
            }

            int pos = GetTextPos();
            FieldDescriptor fld = WriteFieldStart(FieldTypeDefiner.GetFieldType(fieldcode));
            AddFieldDescriptor(fld, pos);
            //Write field code
            WriteSafeChunk(fieldcode);

            CharacterProperties.StickProperties = false;
            int dataPos = (int)m_streamsManager.DataStream.Position;
            if (formField != null)
            {
                formField.Write(m_streamsManager.DataStream);
                CharacterProperties.FldVanish = true;
                CharacterProperties.PicLocation = dataPos;
                CharacterProperties.IsData = true;
                CharacterProperties.Special = true;
                WriteMarker(WordChunkType.Image);
                CharacterProperties.StickProperties = true;
            }
            CharacterProperties.StickProperties = bStick;

            //Write end field descriptor to stack
            FieldDescriptor fldNew = new FieldDescriptor();
            fldNew.HasSeparator = hasSeparator;
            m_endStack.Push(fldNew);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayText"></param>
        /// <param name="url"></param>
        /// <param name="isLocalUrl"></param>
        public void InsertHyperlink(string displayText, string url, bool isLocalUrl)
        {
            InsertStartField(string.Format("HYPERLINK {0}\"{1}\"", !isLocalUrl ? @"\l " : "", url), true);
            WriteChunk(displayText);
            InsertEndField();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="picture"></param>
        public void InsertImage(WPicture picture)
        {
            if (picture.ImageRecord != null)
            {
                Size imageSize = picture.ImageRecord.Size;
                InsertImage(picture, imageSize.Height, imageSize.Width);
            }
        }

        /// <summary>
        /// Insert image
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <param name="height">Picture height</param>
        /// <param name="width">Picture width</param>
        public void InsertImage(WPicture picture, int height, int width)
        {
            if (picture.ImageRecord != null)
            {
                m_nextPicLocation = (int)m_streamsManager.DataStream.Position;
                CharacterProperties.PicLocation = m_nextPicLocation;
                CharacterProperties.Special = true;
                m_docInfo.ImageWriter.WriteImage(picture, height, width);
                WriteMarker(WordChunkType.Image);
                CharacterProperties.Special = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pict"></param>
        /// <param name="pictProps"></param>
        public void InsertShape(WPicture pict, PictureShapeProps pictProps)
        {
            bool isPictureAdded=false;
            int pos = GetTextPos();
            WriteMarker(WordChunkType.Shape);
            MsofbtSpContainer spContainer = null;
            if (Escher.Containers.ContainsKey(pictProps.Spid))
            {
                spContainer = Escher.Containers[pictProps.Spid] as MsofbtSpContainer;
            }
            int[] escherKeys = new int[Escher.Containers.Count];
            Escher.Containers.Keys.CopyTo(escherKeys, 0);
            if (pict.Cloned)
                for (int i = 0; i < escherKeys.Length; i++)
                {
                    if (Escher.Containers[escherKeys[i]] is MsofbtSpContainer)
                    {
                        MsofbtSpContainer shapeContainer = Escher.Containers[escherKeys[i]] as MsofbtSpContainer;
                        if (shapeContainer.Bse != null && shapeContainer.Bse.Blip.ImageRecord == pict.ImageRecord)
                        {
                            pictProps.Spid = escherKeys[i];
                            m_docInfo.TablesData.ArtObj.AddFSPA(pictProps, m_type, pos);
                            isPictureAdded = true;
                            break;
                        }
                    }
                    else
                    {
                        isPictureAdded = false;
                        break;
                    }
                }
            if (!isPictureAdded)
            {
                SetFSPASpid(pictProps);
                AddPictContainer(pict, spContainer, pictProps);
                m_docInfo.TablesData.ArtObj.AddFSPA(pictProps, m_type, pos);
            }
        }

        /// <summary>
        /// Insert texbox (shape) to the document
        /// </summary>
        /// <param name="txbxFormat">Textbox format</param>
        /// <returns></returns>
        public int InsertTextBox(WTextBoxFormat txbxFormat)
        {
            MsofbtSpContainer spContainer = null;
            if (Escher.Containers.ContainsKey(txbxFormat.TextBoxShapeID))
            {
                spContainer = Escher.Containers[txbxFormat.TextBoxShapeID] as MsofbtSpContainer;
            }

            if (spContainer == null && txbxFormat.TextWrappingStyle == TextWrappingStyle.Inline)
            {
                return InsertInlineTextBox(txbxFormat);
            }
            else
            {
                if (spContainer == null || spContainer.ShapeOptions.Txid == null)
                {
                    AddTxBxContainer(txbxFormat);
                }
                else
                {
                    SyncTxBxContainer(spContainer, txbxFormat);
                }
                int pos = GetTextPos();
                FileShapeAddress fspa = new FileShapeAddress();
                TextBoxPropertiesConverter.Import(fspa, txbxFormat);
                m_docInfo.TablesData.ArtObj.AddFSPA(fspa, m_type, pos);
                WriteMarker(WordChunkType.Shape);

                return txbxFormat.TextBoxShapeID;
            }
        }

        /// <summary>
        /// Inserts the inline text box.
        /// </summary>
        /// <param name="txbxFormat">The textbox format.</param>
        /// <returns></returns>
        public int InsertInlineTextBox(WTextBoxFormat txbxFormat)
        {
            //SetFSPASpid(txbxProps);
            txbxFormat.TextBoxShapeID = m_curTxbxId;

            FileShapeAddress fspa = new FileShapeAddress();
            TextBoxPropertiesConverter.Import(fspa, txbxFormat);

            AddTxBxContainer(txbxFormat);
            InsertStartField(" SHAPE \\*MERGEFORMAT ", true);

            int pos = GetTextPos();
            fspa.TxbxCount = 0;
            fspa.IsAnchorLock = true;
            fspa.TextWrappingStyle = TextWrappingStyle.InFrontOfText;

            m_docInfo.TablesData.ArtObj.AddFSPA(fspa, m_type, pos);
            WriteMarker(WordChunkType.Shape);

            m_nextPicLocation = (int)m_streamsManager.DataStream.Position;
            CharacterProperties.PicLocation = m_nextPicLocation;
            CharacterProperties.Special = true;
            m_nextPicLocation = m_docInfo.ImageWriter.WriteInlineTxBxPicture(txbxFormat);
            WriteMarker(WordChunkType.Image);
            CharacterProperties.Special = false;
            InsertEndField();

            return txbxFormat.TextBoxShapeID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeObj"></param>
        public void InsertShapeObject(ShapeObject shapeObj)
        {
            int newSpid = -1;
            
            if (Escher.Containers.ContainsKey(shapeObj.FSPA.Spid))
            {
                BaseContainer spContainer = Escher.Containers[shapeObj.FSPA.Spid];
                WTextBoxCollection autoShapeCollection = shapeObj.AutoShapeTextCollection;
                m_textColIndex = 0;
                spContainer.SynchronizeIdent(autoShapeCollection, ref m_curTxbxId, ref m_curPicId,
                  ref m_curTxid, ref m_textColIndex);
                newSpid = spContainer.GetSpid();
                shapeObj.FSPA.Spid = newSpid;
                Escher.FillCollectionForSearch(spContainer);
            }

            int pos = GetTextPos();
            m_docInfo.TablesData.ArtObj.AddFSPA(shapeObj.FSPA, m_type, pos);
            WriteMarker(WordChunkType.Shape);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeObj"></param>
        public void InsertInlineShapeObject(InlineShapeObject shapeObj)
        {
            if (shapeObj.IsOLE && shapeObj.OLEContainerId != -1)
            {
                CharacterProperties.PicLocation = shapeObj.OLEContainerId;
                CharacterProperties.IsOle2 = true;
            }
            else
            {
                int startPos = (int)m_streamsManager.DataStream.Position;
                m_nextPicLocation = startPos;
                CharacterProperties.PicLocation = m_nextPicLocation;
                m_nextPicLocation = m_docInfo.ImageWriter.WriteInlineShapeObject(shapeObj);

                if (startPos == m_nextPicLocation)
                    m_streamsManager.DataStream.Position++;
            }

            CharacterProperties.Special = true;
            WriteMarker(WordChunkType.Image);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void InsertBookmarkStart(string name, BookmarkStart start)
        {
            m_docInfo.TablesData.BookmarkStrings.Add(name);
            int pos = GetTextPos();
            m_docInfo.TablesData.BookmarkDescriptor.Add(pos);

            int index = m_docInfo.TablesData.BookmarkStrings.Find(name);

            if (start.ColumnLast > -1 && index != -1)
            {
                BookmarkDescriptor bookmarkDes = m_docInfo.TablesData.BookmarkDescriptor;
                bookmarkDes.SetCellGroup(index, true);
                bookmarkDes.SetStartCellIndex(index, start.ColumnFirst);
                bookmarkDes.SetEndCellIndex(index, start.ColumnLast + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertBookmarkEnd(string name)
        {
            int index = m_docInfo.TablesData.BookmarkStrings.Find(name);
            if (index == -1)
            {
                //throw new ArgumentException("No such bookmark with name \"" + name + "\" found");
                return;
            }
            int pos = GetTextPos();
            m_docInfo.TablesData.BookmarkDescriptor.SetEndPos(index, pos);
        }

        /// <summary>
        /// Insert watermark.
        /// </summary>
        /// <param name="watermark">The watermark.</param>
        /// <param name="unitsConvertor">The units converter.</param>
        /// <param name="maxWidth">Maximak width.</param>
        public void InsertWatermark(Watermark watermark, UnitsConvertor unitsConvertor, float maxWidth)
        {
            FileShapeAddress fspa = CreateWatermarkFSPA();
            MsofbtSpContainer waterMarkCont = new MsofbtSpContainer(watermark.Document);
            if (watermark.Type == WatermarkType.TextWatermark)
            {
                waterMarkCont = InsertTextWatermark(watermark, fspa, unitsConvertor);
            }
            else
            {
                waterMarkCont = InsertPictureWatermark(watermark, fspa, unitsConvertor, maxWidth);
            }
            Escher.AddContainerForSubDocument(WordSubdocument.HeaderFooter, waterMarkCont);
            int pos = GetTextPos();
            m_docInfo.TablesData.ArtObj.AddFSPA(fspa, m_type, pos);
            WriteMarker(WordChunkType.Shape);
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordWriterBase"/> class.
        /// </summary>
        /// <param name="streamsManager">The streams manager.</param>
        public WordWriterBase(StreamsManager streamsManager)
        {
            //m_mainStream = stream as MemoryStream;
            m_streamsManager = streamsManager;
        }
        /// <summary>
        /// Default protected constructor.
        /// </summary>
        protected WordWriterBase()
        {
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLength"></param>
        protected abstract void IncreaseCcp(int dataLength);

        /// <summary>
        /// Initialize class members with default values
        /// </summary>
        protected virtual void InitClass()
        {
            m_curTxbxId = 3026;
            //m_curPicId = 3500;
            m_curPicId = 17000;

            if (m_styleSheet == null)
            {
                m_styleSheet = new WordStyleSheet(true);
            }

            m_charProps = new CharacterProperties(m_styleSheet);
            m_breakCharProps = new CharacterProperties(m_styleSheet);
            m_paragProps = new ParagraphProperties();
            m_currStyleIndex = m_styleSheet.DefaultStyleIndex;
        }

        /// <summary>
        /// Writes string to word file 
        /// </summary>
        /// <param name="text"></param>
        protected void WriteString(string text)
        {
            if (text != null && text != string.Empty)
            {
                byte[] arrBuffer = m_docInfo.FibData.Encoding.GetBytes(text);
                m_streamsManager.MainStream.Write(arrBuffer, 0, arrBuffer.Length);

                AddChpxProperties(false);
                IncreaseCcp(text.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void AddChpxProperties(bool isParaBreak)
        {
            CharacterPropertyException chpx = null;

            if (isParaBreak)
                chpx = m_breakCharProps.CloneChpx();
            else
                chpx = m_charProps.CloneChpx();

            m_docInfo.FkpData.AddChpxProperties((uint)m_streamsManager.MainStream.Position, chpx);
        }

        /// <summary>
        /// Adds the papx properties.
        /// </summary>
        protected void AddPapxProperties()
        {
            MemoryStream stream = m_streamsManager.MainStream;
            ParagraphPropertyException paraPropertyException = m_paragProps.ParagraphPropertyException;
            ParagraphExceptionInDiskPage papx = new ParagraphExceptionInDiskPage(m_paragProps.ClonePapx());
            if (paraPropertyException.PropertyModifiers[WordSprmOptions.sprmPIstd] != null)
            {
                //To handle the case paragraph style id not present in the style sheet
                WordStyle SprmStyle = StyleSheet.GetStyleByIndex(paraPropertyException.PropertyModifiers[WordSprmOptions.sprmPIstd].ShortValue);
                if (SprmStyle.ID > 0 || CurrentStyleIndex != 0)
                    papx.ParagraphStyleId = (ushort)CurrentStyleIndex;
            }
            papx.StyleIndex = (ushort)CurrentStyleIndex;
            m_docInfo.FkpData.AddPapxProperties((uint)stream.Position, papx, m_streamsManager.DataStream);
        }

        /// <summary>
        /// Writes the symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        protected void WriteSymbol(char symbol)
        {
            MemoryStream stream = m_streamsManager.MainStream;
            byte[] arrBuffer = m_docInfo.FibData.Encoding.GetBytes(symbol.ToString());
            stream.Write(arrBuffer, 0, arrBuffer.Length);
        }

        /// <summary>
        /// Writes character to word file
        /// </summary>
        /// <param name="symbol"></param>
        protected void WriteChar(char symbol)
        {
            WriteSymbol(symbol);

            //CharacterPropertyException chpx = m_charProps.CloneChpx();
            //m_docInfo.FkpData.AddChpxProperties((uint)stream.Position, chpx);
            AddChpxProperties(symbol == SpecialCharacters.ParagraphEnd);

            if (symbol == SpecialCharacters.ParagraphEnd || symbol == SpecialCharacters.PageBreak || symbol == SpecialCharacters.TableAscii)
            {
                AddPapxProperties();
            }
            IncreaseCcp(1);
        }

        /// <summary>
        /// Writes the nested cell/row mark.
        /// </summary>
        protected void WriteNestedMark()
        {
            WriteSymbol(SpecialCharacters.ParagraphEnd);
            AddChpxProperties(false);
            AddPapxProperties();
            IncreaseCcp(1);
        }
        /// <summary>
        /// Gets position in text
        /// </summary>
        /// <returns></returns>
        internal virtual int GetTextPos()
        {
            int pos = (int)(m_streamsManager.MainStream.Position - m_iStartText);
            return pos / m_docInfo.FibData.EncodingCharSize;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        private void SetFSPASpid(BaseProps props)
        {
            if (props is PictureShapeProps)
            {
                props.Spid = m_curPicId;
            }
            else
            {
                props.Spid = m_curTxbxId;
            }
        }
        /// <summary>
        /// Writes the field start.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns></returns>
        protected FieldDescriptor WriteFieldStart(FieldType fieldType)
        {
            FieldDescriptor fld = new FieldDescriptor();
            fld.FieldBoundary = (byte)SpecialCharacters.FieldBeginMark;

            //Walkaround for Word 2000 
            if (fieldType == FieldType.FieldShape)
            {
                fld.Type = (FieldType)DEF_FIELDSHAPETYPE_VAL;
            }
            else
            {
                fld.Type = fieldType;
            }
            WriteMarker(WordChunkType.FieldBeginMark);
            return fld;
        }

        /// <summary>
        /// Writes the field separator.
        /// </summary>
        /// <returns></returns>
        protected FieldDescriptor WriteFieldSeparator()
        {
            FieldDescriptor fld = new FieldDescriptor();
            fld.FieldBoundary = (byte)SpecialCharacters.FieldSeparator;
            fld.IsNested = (m_endStack.Count > 1);
            WriteMarker(WordChunkType.FieldSeparator);

            return fld;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected void WriteFieldEnd()
        {
            WriteMarker(WordChunkType.FieldEndMark);
        }
        /// <summary>
        /// Write the NilPICFAndBinData
        /// Writes Binary data for reference and hyperlink field
        /// </summary>
        /// <param name="field">Field</param>
        protected void WriteNilPICFAndBinData(WField field)
        {
            CharacterProperties.StickProperties = false;
            int dataPos = (int)m_streamsManager.DataStream.Position;
            //Get writer for Data stream
            BinaryWriter writer = new BinaryWriter(m_streamsManager.DataStream);
            //Create Picture descriptor object
            PICF picf = new PICF();
            //PICF.lcb - structure size
            //29 = 1 (bits) + 16 (clsID) + hyperlink (variable size)
            //hyperlink = 4 (stream version) + 4 (bit information) + 4 (display name length) + Display name = ((field.FieldValue.Length + 1) * 2) // 1 denotes null terminated string
            picf.lcb = 29 + ((field.FieldValue.Length + 1) * 2) + picf.cbHeader;
            picf.Write(m_streamsManager.DataStream);
            byte[] binData = new byte[picf.lcb - picf.cbHeader];
            //bits - 0000 1000 // Tool tip enabled
            binData[0] = Convert.ToByte(8);
            //ClsID - GUID specifies COM component used to create Hyperlink - Here MS Word
            string clsID = "D0 C9 EA 79 F9 BA CE 11 8C 82 00 AA 00 4B A9 0B";
            string[] split = clsID.Split(' ');
            byte[] clsIDByteArray = new byte[split.Length];
            for (int i = 0; i < clsIDByteArray.Length; i++)
            {
                clsIDByteArray[i] = Convert.ToByte(Int32.Parse(split[i], System.Globalization.NumberStyles.HexNumber).ToString());
            }
            clsIDByteArray.CopyTo(binData, 1);
            //Stream version - must be 2
            binData[17] = Convert.ToByte((uint)2);
            //bits  0000 1000 //Display name enabled
            binData[21] = Convert.ToByte(8);
            //Remove the formatting strings from display name(FieldValue)            
            string displayName = RemoveFormattingString(field.FieldValue);
            //Length of the Display name
            binData[25] = Convert.ToByte(displayName.Length + 1);
            //Display name - writes reference bookmark name
            byte[] displayNameByteArray = Encoding.Unicode.GetBytes((displayName + "\0").ToCharArray());
            displayNameByteArray.CopyTo(binData, 29);

            writer.Write(binData);

            CharacterProperties.FldVanish = true;
            CharacterProperties.PicLocation = dataPos;
            CharacterProperties.IsData = true;
            CharacterProperties.Special = true;
            //Write image mark
            WriteMarker(WordChunkType.Image);
            CharacterProperties.StickProperties = true;
        }
        /// <summary>
        /// Removes the formatting strings
        /// </summary>
        /// <param name="value"></param>
        private string RemoveFormattingString(string value)
        {
            Regex regex = new Regex(@"([\\+].)+");
            Match match = regex.Match(value);
            foreach (Group group in match.Groups)
            {
                if(group.Value!=string.Empty)
                    value = value.Replace(group.Value, string.Empty);
            }
            return value.Trim();
        }
        /// <summary>
        /// Write start, end, or separator field descriptor to table stream
        /// </summary>
        /// <param name="fld"></param>
        /// <param name="pos"></param>
        protected void AddFieldDescriptor(FieldDescriptor fld, int pos)
        {
            m_docInfo.TablesData.Fields.AddField(m_type, fld, pos);
        }
        /// <summary>
        /// Adds the textbox container.
        /// </summary>
        /// <param name="txbxFormat">The textbox format.</param>
        public void AddTxBxContainer(WTextBoxFormat txbxFormat)
        {
            txbxFormat.TextBoxShapeID = m_curTxbxId;
            txbxFormat.TextBoxIdentificator = NextTextId;
            MsofbtSpContainer spContainer = new MsofbtSpContainer(txbxFormat.Document);
            spContainer.CreateTextBoxContainer(txbxFormat);
            Escher.AddContainerForSubDocument(m_type, spContainer);
            m_curTxbxId++;
        }
        /// <summary>
        /// Syncronize the textbox container.
        /// </summary>
        /// <param name="spContainer">The shape container.</param>
        /// <param name="txbxFormat">The textbox format.</param>
        internal void SyncTxBxContainer(MsofbtSpContainer spContainer, WTextBoxFormat txbxFormat)
        {
            m_textColIndex = 0;

            if (spContainer.ShapeOptions != null)
            {
                UpdateContainers(spContainer.Shape.ShapeId, m_curTxbxId, spContainer);
                uint txId = spContainer.ShapeOptions.Txid.Value;
                txbxFormat.TextBoxShapeID = m_curTxbxId;
                spContainer.SynchronizeIdent(null, ref m_curTxbxId, ref m_curPicId, ref m_curTxid,
                  ref m_textColIndex);
                txbxFormat.TextBoxIdentificator = m_curTxid;
                spContainer.WriteTextBoxOptions(txbxFormat);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pict"></param>
        /// <param name="spContainer"></param>
        /// <param name="pictProps"></param>
        internal void AddPictContainer(WPicture pict, MsofbtSpContainer spContainer, PictureShapeProps pictProps)
        {
            if (spContainer == null || spContainer.Bse == null ||
              (pict.IsMetaFile && spContainer.Bse.Blip is MsofbtImage) ||
              (spContainer != null && spContainer.Bse.Blip.IsDib) || pict.Document.m_isReadOnly)
            {
                spContainer = new MsofbtSpContainer(pict.Document);
                spContainer.CreateImageContainer(pict, pictProps);
                m_curPicId += 1;
                Escher.AddContainerForSubDocument(m_type, spContainer);
            }
            else
            {
                Escher.ModifyBStoreByPid((int)spContainer.ShapeOptions.Pib.Value, spContainer.Bse);
                SyncPictContainer(spContainer, pictProps,pict);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        /// <param name="pictProps"></param>
        internal void SyncPictContainer(MsofbtSpContainer spContainer, PictureShapeProps pictProps, WPicture pic)
        {
            UpdateContainers(spContainer.Shape.ShapeId, m_curPicId, spContainer);
            spContainer.Shape.HasAnchor = true;
            spContainer.Shape.ShapeId = m_curPicId;
            m_curPicId++;
            spContainer.WritePictureOptions(pictProps, pic);
        }
        /// <summary>
        /// Updates the containers.
        /// </summary>
        /// <param name="oldId">The old id.</param>
        /// <param name="newId">The new id.</param>
        /// <param name="spContainer">The shape container.</param>
        private void UpdateContainers(int oldId, int newId, MsofbtSpContainer spContainer)
        {
            //Escher.Containers.Remove(oldId);
            //Escher.Containers.Remove(newId);  // Need investigation
            try
            {
                Escher.Containers.Add(newId, spContainer);
            }
            catch
            { }  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private FileShapeAddress CreateWatermarkFSPA()
        {
            FileShapeAddress fspa = new FileShapeAddress();
            fspa.Height = 2000;
            fspa.Width = 2000;
            fspa.RelHrzPos = HorizontalOrigin.Column;
            fspa.RelVrtPos = VerticalOrigin.Paragraph;
            fspa.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
            fspa.TextWrappingType = TextWrappingType.Both;

            return fspa;
        }

        /// <summary>
        /// Inserts the text watermark.
        /// </summary>
        /// <param name="watermark">The watermark.</param>
        /// <param name="fspa">The File Shape Address.</param>
        /// <param name="unitsConvertor">The units converter.</param>
        /// <returns></returns>
        private MsofbtSpContainer InsertTextWatermark(Watermark watermark, FileShapeAddress fspa,
          UnitsConvertor unitsConvertor)
        {
            TextWatermark textWatermark = watermark as TextWatermark;
            MsofbtSpContainer textCont = new MsofbtSpContainer(watermark.Document);
            textCont.CreateTextWatermarkContainer(GetWatermarkNumber(), textWatermark);

            if (textWatermark.ShapeHeightInPixels != -1)
            {
                fspa.Height = textWatermark.ShapeHeightInPixels;
                fspa.Width = textWatermark.ShapeWidthInPixels;
            }
            else
            {
                fspa.Height = (int)(textWatermark.ShapeSize.Height * 13.43f);
                fspa.Width = (int)(textWatermark.ShapeSize.Width * 13.88f);
            }
            fspa.Spid = m_curTxbxId;
            textCont.Shape.ShapeId = m_curTxbxId;
            m_curTxbxId += 1;
            textCont.IsWatermark = true;

            return textCont;
        }

        /// <summary>
        /// Inserts the picture watermark.
        /// </summary>
        /// <param name="watermark">The watermark.</param>
        /// <param name="fspa">The File Shape Address.</param>
        /// <param name="unitsConvertor">The units converter.</param>
        /// <param name="maxWidth">Max width.</param>
        /// <returns></returns>
        private MsofbtSpContainer InsertPictureWatermark(Watermark watermark, FileShapeAddress fspa,
          UnitsConvertor unitsConvertor, float maxWidth)
        {
            MsofbtSpContainer pictCont = new MsofbtSpContainer(watermark.Document);
            PictureWatermark picWatermark = watermark as PictureWatermark;
            SizeF pictureSize = FitPictureToPage(picWatermark, maxWidth, unitsConvertor);
            CreatePictureWatermarkCont(picWatermark, pictCont);

            float realHeight = pictureSize.Height * DLSConstants.TwipsInOnePoint;
            float realWidth = pictureSize.Width * DLSConstants.TwipsInOnePoint;
            fspa.Width = (int)((realWidth / 100) * picWatermark.Scaling);
            fspa.Height = (int)((realHeight / 100) * picWatermark.Scaling);
            //Apply Picture properties
            fspa.YaTop = (int)Math.Round(picWatermark.WordPicture.VerticalPosition * DLSConstants.TwipsInOnePoint);
            fspa.XaLeft = (int)Math.Round(picWatermark.WordPicture.HorizontalPosition * DLSConstants.TwipsInOnePoint);
            fspa.RelVrtPos = picWatermark.WordPicture.VerticalOrigin;
            if (picWatermark.WordPicture.HorizontalOrigin == HorizontalOrigin.LeftMargin || picWatermark.WordPicture.HorizontalOrigin == HorizontalOrigin.RightMargin
                || picWatermark.WordPicture.HorizontalOrigin == HorizontalOrigin.InsideMargin || picWatermark.WordPicture.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                fspa.RelHrzPos = HorizontalOrigin.Margin;
            else
                fspa.RelHrzPos = picWatermark.WordPicture.HorizontalOrigin;
            fspa.TextWrappingStyle = picWatermark.WordPicture.TextWrappingStyle;
            fspa.TextWrappingType = picWatermark.WordPicture.TextWrappingType;
            fspa.IsBelowText = picWatermark.WordPicture.IsBelowText;
            fspa.Spid = m_curPicId;

            pictCont.Shape.ShapeId = m_curPicId;
            m_curPicId += 1;
            pictCont.IsWatermark = true;

            return pictCont;
        }

        /// <summary>
        /// Fits the watermark picture to page.
        /// </summary>
        /// <param name="picWatermark">The pic watermark.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="unitsConvertor">The units converter.</param>
        /// <returns>Picture size</returns>
        private SizeF FitPictureToPage(PictureWatermark picWatermark, float maxWidth, UnitsConvertor unitsConvertor)
        {
            float pictureHeight = picWatermark.WordPicture.Height;
            float pictureWidth = picWatermark.WordPicture.Width;
            SizeF pictureSize = new SizeF(pictureWidth, pictureHeight);
#if !SILVERLIGHT && !WP
            if (pictureWidth > maxWidth && picWatermark.Picture is Metafile)
            {
                float proporsion = pictureHeight / pictureWidth;
                float width = maxWidth;
                float height = width * proporsion;
                float pixelsWidth = unitsConvertor.ConvertToPixels(width, PrintUnits.Point);
                float pixelsHeight = unitsConvertor.ConvertToPixels(height, PrintUnits.Point);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(picWatermark.Picture, (int)pixelsWidth, (int)pixelsHeight);
                picWatermark.Picture = bitmap;
                pictureSize = new SizeF(width, height);
            }
#endif

            return pictureSize;
        }

        /// <summary>
        /// Creates the picture watermark container.
        /// </summary>
        /// <param name="pictWatermark">The picture watermark.</param>
        /// <param name="pictContainer">The picture container.</param>
        private void CreatePictureWatermarkCont(PictureWatermark pictWatermark, MsofbtSpContainer pictContainer)
        {
            if (pictWatermark.OriginalPib != -1)
            {
                bool checkResult = this.Escher.CheckBStoreContByPid(pictWatermark.OriginalPib);
                pictWatermark.OriginalPib = (checkResult) ? pictWatermark.OriginalPib : -1;
            }
            pictContainer.CreatePictWatermarkContainer(GetWatermarkNumber(), pictWatermark);
            pictContainer.Pib = pictWatermark.OriginalPib;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetWatermarkNumber()
        {
            int watermarkNumber = 0;
            switch ((this as WordHeaderFooterWriter).HeaderType)
            {
                case HeaderType.FirstPageHeader:
                    watermarkNumber = 1;
                    break;
                case HeaderType.OddHeader:
                    watermarkNumber = 2;
                    break;
                default:
                    watermarkNumber = 3;
                    break;
            }
            return watermarkNumber;
        }
        /// <summary>
        /// Determines whether the image are equal
        /// </summary>
        /// <param name="imageBytes1">Hash value of first image</param>
        /// <param name="imageBytes2">Hash value of second image</param>
        /// <returns>
        /// returns true if image hash are equal.
        /// </returns>
        private bool IsImageEqual(byte[] imageHash1, byte[] imageHash2)
        {
            bool isImageEqual = true;
            //Compare the hash values
            for (int j = 0; j < imageHash1.Length && j < imageHash2.Length; j++)
            {
                if (imageHash1[j] != imageHash2[j])
                {
                    isImageEqual = false;
                    break;
                }
            }
            return isImageEqual;
        }
        #endregion
    }
}
