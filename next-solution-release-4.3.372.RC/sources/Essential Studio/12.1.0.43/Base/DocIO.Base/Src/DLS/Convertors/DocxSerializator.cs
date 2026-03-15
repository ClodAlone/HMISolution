#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.Compression;
using Syncfusion.Compression.Zip;
using Syncfusion.DocIO.DLS.Convertors;
using Syncfusion.DocIO.ReaderWriter.Security;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#endif
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
using Windows.Storage;
using System.Reflection;
#else
#if !WP
using System.Drawing;
#endif
using System.Security.Cryptography;
using System.Reflection;
#if !WP && !SILVERLIGHT
using System.Drawing.Imaging;
#endif
#endif

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the document serializator specific for Word 2010 format
    /// </summary>
    internal class DocxSerializator
    {
        #region Constants
        internal readonly string SlashSymbol = ((char)92).ToString();
        internal readonly string InvertedCommas = ((char)34).ToString();
        internal readonly string NullSymbol = ((char)0).ToString();
        internal const char CarriageReturn = (char)13;
        internal const char NewLine = (char)10;
        #endregion Constants

        #region Private members
        private ZipArchive m_archive;
        private WordDocument m_document;
        private XmlWriter m_writer;
        private int m_relationShipID = 0;
        private int m_id = 2;
        private int m_bookmarkId = 1;
        private int m_docPrId = 0;
        private int m_shapeID = 1024;
        private int m_lstOverId;
        private bool m_hasImages;
        private bool m_hasNumbering;
        private bool m_hasFontTable;
        private Dictionary<int, Dictionary<int, string>> m_lstStyleReferences;
        private Dictionary<string, ImageRecord> m_pictureBullets;
        private bool m_hasFootnote;
        private bool m_hasEndnote;
        private bool m_hasMetafiles;
        private bool m_hasOleObject;
        private bool m_hasComment;
        private Dictionary<string, ImageRecord> m_documentImages;
        private Dictionary<string, ImageRecord> m_footnoteImages;
        private Dictionary<string, ImageRecord> m_endnoteImages;
        private Dictionary<string, ImageRecord> m_commentImages;
        private Dictionary<string, Dictionary<string, ImageRecord>> m_headerFooterImages;
        private Dictionary<string, int> m_bookmarks = new Dictionary<string, int>();
        private Dictionary<HeaderFooterType, Dictionary<string, HeaderFooter>> m_headerFooterColl;
        private List<String> m_symbolFontNames;
        private Stack<WField> m_nonSupportedFields;
        private Dictionary<String, String> m_oleIds;
        internal Dictionary<String, Stream> m_oleContainers;
        internal Dictionary<string, Dictionary<String, Stream>> m_hfOleContainers;
        private List<string> m_oleTypes;
        private Dictionary<string, DictionaryEntry> m_xmlItemsRef;
        private Dictionary<int, string> m_commentsId;
        private Dictionary<string, WComment> m_commentsCollection;
        private Dictionary<int, WFootnote> m_footnoteColl;
        private Dictionary<int, WFootnote> m_endnoteColl;
        private bool IsSectionContainsEndnotes;
        private bool IsSectionContainsFootnotes;
        /// <summary>
        /// Specifies boolean value indicating whether to Serialize "cnfStyle" element for paragraph or not.
        /// </summary>
        private bool IsParagraphContainsCnfStyle;
        private int m_footnoteId = 1;
        private int m_endnoteId = 1;
        private bool HasHyperlink;
        private Dictionary<string, string> m_hyperlinkTargets;
        private Dictionary<string, string> m_altChunkTargets;
        private Dictionary<string, string> m_altChunkContentTypes;
        private Dictionary<string, string> m_footnoteHyperlinks;
        private Dictionary<string, string> m_endnoteHyperlinks;
        private Dictionary<string, Dictionary<string, string>> m_headerFooterHyperlinks;
        private Dictionary<string, string> m_commentHyperlinks;
        private Dictionary<string, string> m_hfIncludePicFieldUrl;
        private Dictionary<string, string> m_inclPicFieldUrl;
        private Dictionary<string, Dictionary<string, string>> m_headerFooterInclPicUrls;
        private bool m_hasDiagrams;
        private Dictionary<String, Dictionary<String, DictionaryEntry>> m_hfRelations;
        private List<String> m_chartsPathNames;
        private List<String> m_cntlPathNames;
        private int m_trackChangeId;
        private string m_watermarkId = string.Empty;
        private string[] m_splittedTextNode = null;
        private WTextRange m_splittedItem = null;
        private bool m_isAlternativeTableFormat = false;
        private bool m_isAlternativeRowFormat = false;
        private bool m_isAlternativeCellFormat = false;
        private HeaderFooterType m_HeaderFooterType;
        private bool m_IsAutoshapeTextboxInHeader;
        private Stack<WField> m_fieldStack;
        private Dictionary<string, DictionaryEntry> m_settingsRelations;
        private WFieldMark m_skipFieldEnd = null;
        #endregion Private members

        #region Properties
        /// <summary>
        /// Gets a value indicating whether save as macro enabled format.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if save as macro enabled; otherwise, <c>false</c>.
        /// </value>
        private bool IsMacroEnabled
        {
            get
            {
                return (m_document.SaveFormatType.ToString().EndsWith("Docm")
                    || m_document.SaveFormatType.ToString().EndsWith("Dotm"));
            }
        }
        /// <summary>
        /// Gets a value indicating whether save as Word 2007 format.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if save as Word 2007 format; otherwise, <c>false</c>.
        /// </value>
        private bool IsWord2007
        {
            get
            {
                return m_document.SaveFormatType.ToString().Contains("2007");
            }
        }
        /// <summary>
        /// Gets a value indicating whether save as Word 2010 format.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if save as Word 2010 format; otherwise, <c>false</c>.
        /// </value>
        private bool IsWord2010
        {
            get
            {
                return m_document.SaveFormatType.ToString().Contains("2010");
            }
        }
        /// <summary>
        /// Gets a value indicating whether save as Word 2013 format.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if save as Word 2013 format; otherwise, <c>false</c>.
        /// </value>
        private bool IsWord2013
        {
            get
            {
                return m_document.SaveFormatType.ToString().Contains("2013") || m_document.SaveFormatType.ToString() == "Docx";
            }
        }
        /// <summary>
        /// Gets the settings relations.
        /// </summary>
        /// <value>The settings relations.</value>
        private Dictionary<string, DictionaryEntry> SettingsRelations
        {
            get
            {
                if (m_settingsRelations == null)
                {
                    m_settingsRelations = new Dictionary<string, DictionaryEntry>();
                }
                return m_settingsRelations;
            }
        }
        /// <summary>
        /// Gets the HeaderFooter Collection
        /// </summary>
        internal Dictionary<HeaderFooterType, Dictionary<string, HeaderFooter>> HeadersFooters
        {
            get
            {
                if (m_headerFooterColl == null)
                {
                    m_headerFooterColl = new Dictionary<HeaderFooterType, Dictionary<string, HeaderFooter>>();
                }
                return m_headerFooterColl;
            }
        }
        /// <summary>
        /// Gets the Control path names
        /// </summary>
        internal List<String> ControlsPathNames
        {
            get
            {
                if (m_cntlPathNames == null)
                {
                    m_cntlPathNames = new List<String>();
                }
                return m_cntlPathNames;
            }
        }
        /// <summary>
        /// Gets the chart path names
        /// </summary>
        internal List<String> ChartsPathNames
        {
            get
            {
                if (m_chartsPathNames == null)
                {
                    m_chartsPathNames = new List<String>();
                }

                return m_chartsPathNames;
            }
        }
        /// <summary>
        /// Gets the HeaderFooter relations
        /// </summary>
        internal Dictionary<String, Dictionary<String, DictionaryEntry>> HFRelations
        {
            get
            {
                if (m_hfRelations == null)
                    m_hfRelations = new Dictionary<String, Dictionary<String, DictionaryEntry>>();
                return m_hfRelations;
            }
        }
        /// <summary>
        /// Collection of Include Picture targets with ids inside headers or footers.
        /// </summary>
        internal Dictionary<string, Dictionary<string, string>> HeaderFooterInclPicUrls
        {
            get
            {
                if (m_headerFooterInclPicUrls == null)
                {
                    m_headerFooterInclPicUrls = new Dictionary<string, Dictionary<string, string>>();
                }

                return m_headerFooterInclPicUrls;
            }
        }
        /// <summary>
        /// Gets the Urls of the include picture fields
        /// </summary>
        internal Dictionary<string, string> InclPicFieldUrl
        {
            get
            {
                if (m_inclPicFieldUrl == null)
                {
                    m_inclPicFieldUrl = new Dictionary<string, string>();
                }
                return m_inclPicFieldUrl;
            }
        }
        /// <summary>
        /// Contains key and include picture fields link present in HeaderFooters
        /// </summary>
        internal Dictionary<string, string> HeaderFooterInclPicFieldUrl
        {
            get
            {
                if (m_hfIncludePicFieldUrl == null)
                {
                    m_hfIncludePicFieldUrl = new Dictionary<string, string>();
                }
                return m_hfIncludePicFieldUrl;
            }
        }
        /// <summary>
        /// Gets the Hyperlink details present in the comments
        /// </summary>
        internal Dictionary<string, string> CommentHyperlinks
        {
            get
            {
                if (m_commentHyperlinks == null)
                {
                    m_commentHyperlinks = new Dictionary<string, string>();
                }

                return m_commentHyperlinks;
            }
        }
        /// <summary>
        /// Collection of hyperlink targets with ids.
        /// </summary>
        internal Dictionary<string, string> HyperlinkTargets
        {
            get
            {
                if (m_hyperlinkTargets == null)
                {
                    m_hyperlinkTargets = new Dictionary<string, string>();
                }
                return m_hyperlinkTargets;
            }
        }
        /// <summary>
        /// Collection of altChunk targets with ids.
        /// </summary>
        internal Dictionary<string, string> AltChunkTargets
        {
            get
            {
                if (m_altChunkTargets == null)
                {
                    m_altChunkTargets = new Dictionary<string, string>();
                }
                return m_altChunkTargets;
            }
        }
        /// <summary>
        /// Collection of altChunk targets with ids.
        /// </summary>
        internal Dictionary<string, string> AltChunkContentTypes
        {
            get
            {
                if (m_altChunkContentTypes == null)
                {
                    m_altChunkContentTypes = new Dictionary<string, string>();
                }
                return m_altChunkContentTypes;
            }
        }
        /// <summary>
        /// Collection of hyperlink targets with ids inside footnotes.
        /// </summary>
        internal Dictionary<string, string> FootnoteHyperlinks
        {
            get
            {
                if (m_footnoteHyperlinks == null)
                {
                    m_footnoteHyperlinks = new Dictionary<string, string>();
                }

                return m_footnoteHyperlinks;
            }
        }
        /// <summary>
        /// Collection of hyperlink targets with ids inside endnotes.
        /// </summary>
        internal Dictionary<string, string> EndnoteHyperlinks
        {
            get
            {
                if (m_endnoteHyperlinks == null)
                {
                    m_endnoteHyperlinks = new Dictionary<string, string>();
                }

                return m_endnoteHyperlinks;
            }
        }
        /// <summary>
        /// Collection of hyperlink targets with ids inside headers or footers.
        /// </summary>
        internal Dictionary<string, Dictionary<string, string>> HeaderFooterHyperlinks
        {
            get
            {
                if (m_headerFooterHyperlinks == null)
                {
                    m_headerFooterHyperlinks = new Dictionary<string, Dictionary<string, string>>();
                }

                return m_headerFooterHyperlinks;
            }
        }
        /// <summary>
        /// Gets the comment collections
        /// </summary>
        internal Dictionary<string, WComment> CommentCollection
        {
            get
            {
                if (m_commentsCollection == null)
                {
                    m_commentsCollection = new Dictionary<string, WComment>();
                }
                return m_commentsCollection;
            }
        }
        /// <summary>
        /// Gets the comment ids
        /// </summary>
        private Dictionary<int, string> CommentsId
        {
            get
            {
                if (m_commentsId == null)
                {
                    m_commentsId = new Dictionary<int, string>();
                }
                return m_commentsId;
            }
        }
        /// <summary>
        /// Gets the ole containers
        /// </summary>
        internal Dictionary<String, Stream> OleContainers
        {
            get
            {
                if (m_oleContainers == null)
                    m_oleContainers = new Dictionary<string, Stream>();
                return m_oleContainers;
            }
        }
        /// <summary>
        /// Gets the ole containers present in the HeaderFooters
        /// </summary>
        internal Dictionary<string, Dictionary<String, Stream>> HFOleContainers
        {
            get
            {
                if (m_hfOleContainers == null)
                    m_hfOleContainers = new Dictionary<string, Dictionary<String, Stream>>();
                return m_hfOleContainers;
            }
        }
        /// <summary>
        /// Gets the ole object ids
        /// </summary>
        private Dictionary<String, String> OleIds
        {
            get
            {
                if (m_oleIds == null)
                    m_oleIds = new Dictionary<string, string>();
                return m_oleIds;
            }
        }
        /// <summary>
        /// Gets the collection of OleObject content types
        /// </summary>
        internal List<String> OleContentTypes
        {
            get
            {
                if (m_oleTypes == null)
                    m_oleTypes = new List<string>();
                return m_oleTypes;
            }
        }
        /// <summary>
        /// Gets the collection of Xml items relations
        /// </summary>
        internal Dictionary<string, DictionaryEntry> XmlItemsRelations
        {
            get
            {
                if (m_xmlItemsRef == null)
                {
                    m_xmlItemsRef = new Dictionary<string, DictionaryEntry>();
                }

                return m_xmlItemsRef;
            }
        }
        /// <summary>
        /// Gets the list style references.
        /// </summary>
        /// <value>The list style references.</value>
        private Dictionary<int, Dictionary<int, string>> ListStyleReferences
        {
            get
            {
                if (m_lstStyleReferences == null)
                {
                    m_lstStyleReferences = new Dictionary<int, Dictionary<int, string>>();
                }
                return m_lstStyleReferences;
            }
        }
        /// <summary>
        /// Gets the collection of picture bullets
        /// </summary>
        private Dictionary<string, ImageRecord> PictureBullets
        {
            get
            {
                if (m_pictureBullets == null)
                {
                    m_pictureBullets = new Dictionary<string, ImageRecord>();
                }
                return m_pictureBullets;
            }
        }
        /// <summary>
        /// Represents the presence of font table
        /// </summary>
        public bool HasFontTable
        {
            get
            {
                return m_hasFontTable;
            }
            set
            {
                m_hasFontTable = value;
            }
        }
        /// <summary>
        /// Represents the presence of footnote
        /// </summary>
        public bool HasFootnote
        {
            get
            {
                return m_hasFootnote;
            }
            set
            {
                m_hasFootnote = value;
            }
        }
        /// <summary>
        /// Represents the presence of endnote
        /// </summary>
        public bool HasEndnote
        {
            get
            {
                return m_hasEndnote;
            }
            set
            {
                m_hasEndnote = value;
            }
        }
        /// <summary>
        /// Represents the presents the list styles
        /// </summary>
        public bool HasNumbering
        {
            get
            {
                return m_hasNumbering;
            }
            set
            {
                m_hasNumbering = value;
            }
        }
        /// <summary>
        /// Gets the collection of images present in the document body
        /// </summary>
        private Dictionary<string, ImageRecord> DocumentImages
        {
            get
            {
                if (m_documentImages == null)
                {
                    m_documentImages = new Dictionary<string, ImageRecord>();
                }
                return m_documentImages;
            }
        }
        /// <summary>
        /// Gets the collection of images present in the footnote images
        /// </summary>
        private Dictionary<string, ImageRecord> FootnoteImages
        {
            get
            {
                if (m_footnoteImages == null)
                {
                    m_footnoteImages = new Dictionary<string, ImageRecord>();
                }
                return m_footnoteImages;
            }
        }
        /// <summary>
        /// Gets the colection of images in the endnote images
        /// </summary>
        private Dictionary<string, ImageRecord> EndnoteImages
        {
            get
            {
                if (m_endnoteImages == null)
                {
                    m_endnoteImages = new Dictionary<string, ImageRecord>();
                }
                return m_endnoteImages;
            }
        }
        /// <summary>
        /// Gets the collection of images in the comments
        /// </summary>
        private Dictionary<string, ImageRecord> CommentImages
        {
            get
            {
                if (m_commentImages == null)
                {
                    m_commentImages = new Dictionary<string, ImageRecord>();
                }
                return m_commentImages;
            }
        }
        /// <summary>
        /// Gets the footnote collection
        /// </summary>
        private Dictionary<int, WFootnote> FootnoteCollection
        {
            get
            {
                if (m_footnoteColl == null)
                {
                    m_footnoteColl = new Dictionary<int, WFootnote>();
                }
                return m_footnoteColl;
            }
        }
        /// <summary>
        /// Gets the endnote collection
        /// </summary>
        private Dictionary<int, WFootnote> EndnoteCollection
        {
            get
            {
                if (m_endnoteColl == null)
                {
                    m_endnoteColl = new Dictionary<int, WFootnote>();
                }
                return m_endnoteColl;
            }
        }
        /// <summary>
        /// Gets the collections of images present in the HeaderFooters
        /// </summary>
        internal Dictionary<string, Dictionary<string, ImageRecord>> HeaderFooterImages
        {
            get
            {
                if (m_headerFooterImages == null)
                {
                    m_headerFooterImages = new Dictionary<string, Dictionary<string, ImageRecord>>();
                }

                return m_headerFooterImages;
            }
        }
        /// <summary>
        /// Gets the collections of Non-supported fields
        /// </summary>
        private Stack<WField> NonSupportedFields
        {
            get
            {
                if (m_nonSupportedFields == null)
                {
                    m_nonSupportedFields = new Stack<WField>();
                }
                return m_nonSupportedFields;
            }
        }
        /// <summary>
        /// Gets the field stack.
        /// </summary>
        /// <value>The field stack.</value>
        private Stack<WField> FieldStack
        {
            get
            {
                if (m_fieldStack == null)
                    m_fieldStack = new Stack<WField>();
                return m_fieldStack;
            }
        }
        /// <summary>
        /// Gets the current field.
        /// </summary>
        /// <value>The current field.</value>
        private WField CurrentField
        {
            get
            {
                return (m_fieldStack != null && m_fieldStack.Count > 0) ? m_fieldStack.Peek() : null;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Word2010Serializator"/> class.
        /// </summary>
        public DocxSerializator()
        {
            m_archive = new ZipArchive();
            m_archive.DefaultCompressionLevel = CompressionLevel.Normal;
        }
        #endregion Constructors

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Saves the word document
        /// </summary>
        /// <param name="fileName">Name of the file/document</param>
        /// <param name="document">Instance of WordDocument</param>
        internal void Serialize(string fileName, WordDocument document)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create);
            Serialize(stream, document);
            stream.Close();
        }
#endif
        /// <summary>
        /// Saves the word document in the stream
        /// </summary>
        /// <param name="stream">Stream to save the document</param>
        /// <param name="document">Instance of WordDocument</param>
        internal void Serialize(Stream stream, WordDocument document)
        {
            m_document = document;
            if (document.HasMacros
                && !IsMacroEnabled)
                throw new Exception("This document contains macros (VBA project) and it cannot be saved as Macro-Free format. Please save the document as Macro-Enabled format (.Docm or .Dotm) or remove macros before saving the document using WordDocument.RemoveMacros method.");
            if (document.Footnotes.ContinuationNotice.Count > 0)
                m_footnoteId++;
            if (document.Endnotes.ContinuationNotice.Count > 0)
                m_endnoteId++;
            //document.xml
            SerializeDocument();
            //Styles.xml
            SerializeStyles();
            //numbering.xml
            SerializeNumberings();
            //settings.xml
            SerializeSettings();
            //core.xml
            SerializeCoreProperties();
            //app.xml
            SerializeAppProperties();
            SerializeFontTable();
            //custom.xml
            if (m_document.CustomDocumentProperties != null && m_document.CustomDocumentProperties.Count > 0)
            {
                SerializeCustomProperties();
            }
            if (m_document.HasMacros
                && IsMacroEnabled)
            {
                SerializeVbaProject();
                SerializeVbaData();
            }
            //Settings Relations
            SerializeSettingsRelation();

            //Numbering relation if the document has picture bullet
            if (PictureBullets.Count > 0)
            {
                SerializeNumberingsRelation();
            }

            SerializeHeaderFooters();

            //comments.xml
            if (m_hasComment)
            {
                SerializeComments();
                if (CommentImages.Count > 0 || CommentHyperlinks.Count > 0)
                {
                    SerializeCommentRelations();
                }
            }

            if (HasEndnote)
            {
                SerializeFootEndnotes(true);
                if (EndnoteImages.Count > 0 || EndnoteHyperlinks.Count > 0)
                {
                    SerializeEndnoteRelations();
                }
            }

            if (HasFootnote)
            {
                SerializeFootEndnotes(false);
                if (FootnoteImages.Count > 0 || FootnoteHyperlinks.Count > 0)
                {
                    SerializeFootnoteRelations();
                }
            }
            if (m_hasDiagrams)
            {
                AddDiagramToZip(m_document.DocxPackage);
            }
            //document relations
            SerializeDocumentRelations();

            // Add chart items to archieve.
            if (ChartsPathNames.Count > 0)
            {
                AddChartsToZip(m_document.DocxPackage);
            }
            // Add controls to archieve.
            if (ControlsPathNames.Count > 0)
            {
                AddControlsToZip(m_document.DocxPackage);
            }
            if (m_document.CustomUIPartContainer != null)
                AddPartContainerToArchive(m_document.CustomUIPartContainer);
            if (m_document.CustomXMLContainer != null)
                AddPartContainerToArchive(m_document.CustomXMLContainer);
            //general relations
            SerializeGeneralRelations();
            if (m_document.HasMacros
                && IsMacroEnabled)
                SerializeVbaProjectRelations();
            //[ContentTypes].xml
            SerializeContentTypes();

            //Save all the parts and relationship into a single file
            if (string.IsNullOrEmpty(document.Password))
                m_archive.Save(stream, false);
            else
            {
                MemoryStream streamTemp = new MemoryStream();
                m_archive.Save(streamTemp, false);
                using (ICompoundFile file = (document as WordDocument).CreateCompoundFile())
                {
                    string password = (document as WordDocument).Password;
                    if (IsWord2007)
                    {
                        // Encrypts the document stream using Standard encryption.
                        StandardEncryptor encryptor = new StandardEncryptor();
                        streamTemp.Position = 0;
                        encryptor.Encrypt(streamTemp, password, file.RootStorage);
                        file.Save(stream);
                    }
                    else
                    {
                        // Encrypts the document stream using Agile encryption.
                        AgileEncryptor encryptor;
                        if (IsWord2010)
                            encryptor = new AgileEncryptor();
                        else
                            // Initializes agile encryptor for Word 2013 format.
                            encryptor = new AgileEncryptor("SHA512", 256, 64);
                        streamTemp.Position = 0;
                        encryptor.Encrypt(streamTemp, password, file.RootStorage);
                        file.Save(stream);
                    }
                }
            }
        }
        private void AddPartContainerToArchive(PartContainer partContainer)
        {
            string itemName = partContainer.Name;
            //Relations
            string[] relationKeys = new string[partContainer.Relations.Count];
            partContainer.Relations.Keys.CopyTo(relationKeys, 0);
            for (int i = 0; i < relationKeys.Length; i++)
            {
                itemName = relationKeys[i];
                m_archive.AddItem(itemName, partContainer.Relations[relationKeys[i]].DataStream, false, FileAttributes.Archive);
            }
            itemName = partContainer.Name;
            //XMLPartContainer
            AddPartContainerXMLPartContainersToArchive(itemName, partContainer.XmlPartContainers);
            //XMLParts
            string[] xmlPartKeys = new string[partContainer.XmlParts.Count];
            partContainer.XmlParts.Keys.CopyTo(xmlPartKeys, 0);
            for (int i = 0; i < xmlPartKeys.Length; i++)
            {
                string archiveXMLPartItemStartName = itemName + xmlPartKeys[i];
                m_archive.AddItem(archiveXMLPartItemStartName, partContainer.XmlParts[xmlPartKeys[i]].DataStream, false, FileAttributes.Archive);
            }
        }
        private void AddPartContainerXMLPartContainersToArchive(string itemName, Dictionary<string, PartContainer> xmlPartContainers)
        {
            string archiveXMLPartItemStartName = itemName;
            //Relations
            string[] xmlPartContainerKeys = new string[xmlPartContainers.Count];
            xmlPartContainers.Keys.CopyTo(xmlPartContainerKeys, 0);
            for (int i = 0; i < xmlPartContainerKeys.Length; i++)
            {
                archiveXMLPartItemStartName = itemName + xmlPartContainerKeys[i];
                string[] relationKeys = new string[xmlPartContainers[xmlPartContainerKeys[i]].Relations.Count];
                xmlPartContainers[xmlPartContainerKeys[i]].Relations.Keys.CopyTo(relationKeys, 0);
                for (int j = 0; j < relationKeys.Length; j++)
                {
                    string archiveRelationItemName = archiveXMLPartItemStartName + relationKeys[j];
                    m_archive.AddItem(archiveRelationItemName, xmlPartContainers[xmlPartContainerKeys[i]].Relations[relationKeys[j]].DataStream, false, FileAttributes.Archive);
                }
            }
            //XML Parts
            for (int i = 0; i < xmlPartContainerKeys.Length; i++)
            {
                archiveXMLPartItemStartName = itemName + xmlPartContainerKeys[i];
                string[] xmlPartKeys = new string[xmlPartContainers[xmlPartContainerKeys[i]].XmlParts.Count];
                xmlPartContainers[xmlPartContainerKeys[i]].XmlParts.Keys.CopyTo(xmlPartKeys, 0);
                for (int j = 0; j < xmlPartKeys.Length; j++)
                {
                    string archiveXMLItemName = archiveXMLPartItemStartName + xmlPartKeys[j];
                    m_archive.AddItem(archiveXMLItemName, xmlPartContainers[xmlPartContainerKeys[i]].XmlParts[xmlPartKeys[j]].DataStream, false, FileAttributes.Archive);
                }
            }
            //XML PartContainers
            for (int i = 0; i < xmlPartContainerKeys.Length; i++)
            {
                archiveXMLPartItemStartName = itemName + xmlPartContainerKeys[i];
                string[] xmlPartContainerInnerKeys = new string[xmlPartContainers[xmlPartContainerKeys[i]].XmlPartContainers.Count];
                xmlPartContainers[xmlPartContainerKeys[i]].XmlPartContainers.Keys.CopyTo(xmlPartContainerInnerKeys, 0);
                for (int j = 0; j < xmlPartContainerInnerKeys.Length; j++)
                {
                    string archiveXMLPartContainerName = archiveXMLPartItemStartName + xmlPartContainerInnerKeys[j];
                    AddPartContainerXMLPartContainersToArchive(archiveXMLPartContainerName, xmlPartContainers[xmlPartContainerKeys[i]].XmlPartContainers[xmlPartContainerInnerKeys[j]].XmlPartContainers);//  xmlitem.Value.XmlPartContainers);
                }
            }
        }
        /// <summary>
        /// Serialize the Font table
        /// </summary>
        private void SerializeFontTable()
        {
            if (m_symbolFontNames == null
                && m_document.FontSubstitutionTable.Count == 0
                && !(m_document.FFNStringTable != null
                && m_document.FFNStringTable.RecordsCount != 0))
                return;

            HasFontTable = true;

            MemoryStream fontTableStream = new MemoryStream();
            m_writer = CreateWriter(fontTableStream);

            m_writer.WriteStartElement("w", "fonts", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "mc", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "w", null, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("mc", "Ignorable", null, "w14 w15");
            List<string> usedFonts = new List<string>();
            if (m_symbolFontNames != null)
            {
                foreach (string fontName in m_symbolFontNames)
                {
                    m_writer.WriteStartElement("font", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("name", DocxConstants.W_namespace, fontName);
                    if (m_document.FontSubstitutionTable.ContainsKey(fontName)
                        && m_document.FontSubstitutionTable[fontName] != string.Empty)
                    {
                        usedFonts.Add(fontName);
                        m_writer.WriteStartElement("altName", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, m_document.FontSubstitutionTable[fontName]);
                        m_writer.WriteEndElement();
                    }
                    m_writer.WriteStartElement("charset", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "02");
                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                }
            }
            if (m_document.FFNStringTable != null)
            {
                foreach (FontFamilyNameRecord ffnRecord in m_document.FFNStringTable.FontFamilyNameRecords)
                {
                    m_writer.WriteStartElement("font", DocxConstants.W_namespace);
                    string[] fontName = ffnRecord.FontName.Split('\0');
                    m_writer.WriteAttributeString("name", DocxConstants.W_namespace, fontName[0]);
                    if (ffnRecord.AlternativeFontName != string.Empty)
                    {
                        m_writer.WriteStartElement("altName", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, ffnRecord.AlternativeFontName);
                        m_writer.WriteEndElement();
                    }
                    m_writer.WriteStartElement("charset", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, ffnRecord.CharacterSetId.ToString());
                    m_writer.WriteEndElement();
                    m_writer.WriteStartElement("family", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, ((FontFamilyType)ffnRecord.FontFamilyID).ToString().ToLower());
                    m_writer.WriteEndElement();
                    m_writer.WriteStartElement("pitch", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, ((FontPitchType)ffnRecord.PitchRequest).ToString().ToLower());
                    m_writer.WriteEndElement();
                    //Serialize font signature 
                    m_writer.WriteStartElement("sig",DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("usb0", DocxConstants.W_namespace, GetFontSignature(ffnRecord.SigUsb0));
                    m_writer.WriteAttributeString("usb1", DocxConstants.W_namespace, GetFontSignature(ffnRecord.SigUsb1));
                    m_writer.WriteAttributeString("usb2", DocxConstants.W_namespace, GetFontSignature(ffnRecord.SigUsb2));
                    m_writer.WriteAttributeString("usb3", DocxConstants.W_namespace, GetFontSignature(ffnRecord.SigUsb3));
                    m_writer.WriteAttributeString("csb0", DocxConstants.W_namespace, GetFontSignature(ffnRecord.SigCsb0));
                    m_writer.WriteAttributeString("csb1", DocxConstants.W_namespace, GetFontSignature(ffnRecord.SigCsb1));
                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> keyValue in m_document.FontSubstitutionTable)
                {
                    if (!usedFonts.Contains(keyValue.Key)
                        && keyValue.Value != string.Empty)
                    {
                        m_writer.WriteStartElement("font", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("name", DocxConstants.W_namespace, keyValue.Key);
                        m_writer.WriteStartElement("altName", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, keyValue.Value);
                        m_writer.WriteEndElement();
                        m_writer.WriteEndElement();
                    }
                }
            }
            m_writer.WriteEndElement();

            m_writer.Flush();
            m_archive.AddItem(DocxConstants.FontTablePath, fontTableStream, false, FileAttributes.Archive);

        }
        /// <summary>
        /// Get the font signature
        /// </summary>
        /// <returns></returns>
        private string GetFontSignature(byte[] sig)
        {
            char[] charArray = BitConverter.ToString(sig).ToCharArray();
            Array.Reverse(charArray);
            string signature = new string(charArray).Replace("-", string.Empty);
            return signature;
        }
        /// <summary>
        /// Serializes the document elements (document.xml)
        /// </summary>
        private void SerializeDocument()
        {
            MemoryStream stream = new MemoryStream();
            m_writer = CreateWriter(stream);

            m_writer.WriteStartElement("w", "document", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "wpc", null, DocxConstants.WPC_namesapce);
            m_writer.WriteAttributeString("xmlns", "mc", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "wp14", null, DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("xmlns", "wpg", null, DocxConstants.WPG_namespace);
            m_writer.WriteAttributeString("xmlns", "wpi", null, DocxConstants.WPI_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WPI_namespace);
            m_writer.WriteAttributeString("xmlns", "wps", null, DocxConstants.WPS_namespace);
            m_writer.WriteAttributeString("mc", "Ignorable", null, "w14 w15 wp14");

            SerializeBackground(m_document.Background);
            SerializeDocumentBody();

            m_writer.WriteEndElement();//end of document tag
            m_writer.Flush();
            m_archive.AddItem(DocxConstants.DocumentPath, stream, false, FileAttributes.Archive);
        }

        #region Macros
        /// <summary>
        /// Serializes the vba project relations.
        /// </summary>
        private void SerializeVbaProjectRelations()
        {
            MemoryStream relationStream = new MemoryStream();
            m_writer = CreateWriter(relationStream);

            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            SerializeRelationShip(relationStream, GetNextRelationShipID(), DocxConstants.VbaDataRelType, DocxConstants.VbaData);

            m_writer.WriteEndElement();
            m_writer.Flush();
            m_archive.AddItem(DocxConstants.VbaProjectRelsPath, relationStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the vba project.
        /// </summary>
        private void SerializeVbaProject()
        {
            m_archive.AddItem(DocxConstants.VbaProjectPath, m_document.VbaProject, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the vba data.
        /// </summary>
        private void SerializeVbaData()
        {
            MemoryStream stream = new MemoryStream();
            m_writer = CreateWriter(stream);

            m_writer.WriteStartElement("wne", "vbaSuppData", DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "wpc", null, DocxConstants.WPC_namesapce);
            m_writer.WriteAttributeString("xmlns", "mc", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "wp14", null, DocxConstants.WP14_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "w", null, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("xmlns", "wpg", null, DocxConstants.WPG_namespace);
            m_writer.WriteAttributeString("xmlns", "wpi", null, DocxConstants.WPI_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "wps", null, DocxConstants.WPS_namespace);
            m_writer.WriteAttributeString("mc", "Ignorable", null, "w14 w15 wp14");

            if (m_document.DocEvents.Count > 0)
            {
                m_writer.WriteStartElement("wne", "docEvents", DocxConstants.WNE_namespace);
                foreach (string docEvent in m_document.DocEvents)
                {
                    m_writer.WriteStartElement("wne", docEvent, DocxConstants.WNE_namespace);
                    m_writer.WriteEndElement();
                }
                m_writer.WriteEndElement();
            }
            if (m_document.VbaData.Count > 0)
            {
                m_writer.WriteStartElement("wne", "mcds", DocxConstants.WNE_namespace);
                foreach (MacroData macro in m_document.VbaData)
                {
                    m_writer.WriteStartElement("wne", "mcd", DocxConstants.WNE_namespace);
                    m_writer.WriteAttributeString("wne", "macroName", DocxConstants.WNE_namespace, macro.Name.ToUpper());
                    m_writer.WriteAttributeString("wne", "name", DocxConstants.WNE_namespace, macro.Name);
                    m_writer.WriteAttributeString("wne", "bEncrypt", DocxConstants.WNE_namespace, macro.Encrypt);
                    m_writer.WriteAttributeString("wne", "cmg", DocxConstants.WNE_namespace, macro.Cmg);
                    m_writer.WriteEndElement();
                }
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();

            m_writer.Flush();
            m_archive.AddItem(DocxConstants.VbaDataPath, stream, false, FileAttributes.Archive);
        }
        #endregion

        #region HeaderFooters
        /// <summary>
        /// Serialize Headers and Footers
        /// </summary>
        private void SerializeHeaderFooters()
        {
            SerializeHeaderFooter(HeaderFooterType.EvenFooter, m_document);
            SerializeHeaderFooter(HeaderFooterType.EvenHeader, m_document);
            SerializeHeaderFooter(HeaderFooterType.FirstPageFooter, m_document);
            SerializeHeaderFooter(HeaderFooterType.FirstPageHeader, m_document);
            SerializeHeaderFooter(HeaderFooterType.OddFooter, m_document);
            SerializeHeaderFooter(HeaderFooterType.OddHeader, m_document);
        }
        /// <summary>
        /// Serializes the Header/Footer
        /// </summary>
        /// <param name="hfType">Type of the HeaderFooter</param>
        /// <param name="doc">Instance of WordDocument</param>
        private void SerializeHeaderFooter(HeaderFooterType hfType, WordDocument doc)
        {
            if (HeadersFooters.Count == 0)
                return;

            string headerFooterPath;
            string headerFooterRelsPath;
            MemoryStream headerFooterStream = new MemoryStream();

            if (!HeadersFooters.ContainsKey(hfType))
                return;

            Dictionary<string, HeaderFooter> hfColl = HeadersFooters[hfType];
            HeaderFooter hf = null;

            foreach (string id in hfColl.Keys)
            {
                hf = hfColl[id];

                if (hfType == HeaderFooterType.EvenHeader || hfType == HeaderFooterType.FirstPageHeader ||
                  hfType == HeaderFooterType.OddHeader)
                {
                    headerFooterPath = DocxConstants.HeaderPath + id.Replace("rId", "") + ".xml";
                    headerFooterRelsPath = DocxConstants.HeaderRelationPath + id.Replace("rId", "") + ".xml.rels";
                    SerializeHeader(hf, id, headerFooterPath, headerFooterRelsPath);
                }
                else
                {
                    headerFooterPath = DocxConstants.FooterPath + id.Replace("rId", "") + ".xml";
                    headerFooterRelsPath = DocxConstants.FooterRelationPath + id.Replace("rId", "") + ".xml.rels";
                    SerializeFooter(hf, id, headerFooterPath, headerFooterRelsPath);
                }
            }
        }
        /// <summary>
        /// Serialize the header part
        /// </summary>
        /// <param name="header">The header</param>
        /// <param name="id">The header relationship ID</param>
        /// <param name="headerFooterPath">The header path</param>
        /// <param name="headerFooterRelsPath">The header's relation path</param>
        private void SerializeHeader(HeaderFooter header, string id, string headerFooterPath, string headerFooterRelsPath)
        {
            MemoryStream headerStream = new MemoryStream();
            m_writer = CreateWriter(headerStream);

            m_writer.WriteStartElement("w", "hdr", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "ve", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "a", null, DocxConstants.A_namespace);
            m_writer.WriteAttributeString("xmlns", "pic", null, DocxConstants.PIC_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("xmlns", "wpc", null, DocxConstants.WPC_namesapce);
            m_writer.WriteAttributeString("xmlns", "wp14", null, DocxConstants.WP14_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("xmlns", "wpg", null, DocxConstants.WPG_namespace);
            m_writer.WriteAttributeString("xmlns", "wpi", null, DocxConstants.WPI_namespace);
            m_writer.WriteAttributeString("xmlns", "wps", null, DocxConstants.WPS_namespace);
            m_writer.WriteAttributeString("ve", "Ignorable", null, "w14 w15 wp14");

            if (m_document.Watermark.Type != WatermarkType.NoWatermark && header.WriteWatermark)
            {
                if (header.Paragraphs.Count == 0)
                {
                    m_writer.WriteStartElement("p", DocxConstants.W_namespace);
                    m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                    m_writer.WriteStartElement("pStyle", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "Header");
                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                    Watermark watermark = header.Document.Watermark;
                    if ((header.Type == HeaderFooterType.FirstPageHeader ||
                      header.Type == HeaderFooterType.OddHeader ||
                      header.Type == HeaderFooterType.EvenHeader) &&
                      header.WriteWatermark)
                        SerializeWatermark(header.Document.Watermark);
                    m_writer.WriteEndElement();
                    SerializeBodyItems(header.Items, true);//serialize the header body contents after serialized the watermark
                }
                else
                {
                    foreach (TextBodyItem item in header.Items)
                    {
                        if (!HasSinglePageField(item))
                            SerializeBodyItem(item, true);
                    }
                }

                if (m_document.Watermark.Type == WatermarkType.PictureWatermark)
                {
                    m_hasImages = true;
                    string headerId = string.Empty;
                    Dictionary<string, ImageRecord> headerImages;

                    foreach (HeaderFooterType hfType in m_headerFooterColl.Keys)
                    {
                        Dictionary<string, HeaderFooter> hfColl = m_headerFooterColl[hfType];

                        foreach (string key in hfColl.Keys)
                        {
                            if (hfColl[key] == header)
                                headerId = key;
                        }
                    }

                    UpdateImages((m_document.Watermark as PictureWatermark).WordPicture);
                    if (HeaderFooterImages.ContainsKey(headerId))
                    {
                        headerImages = m_headerFooterImages[headerId];
                        headerImages.Add(m_watermarkId, (m_document.Watermark as PictureWatermark).WordPicture.ImageRecord);
                    }
                    else
                    {
                        headerImages = new Dictionary<string, ImageRecord>();
                        headerImages.Add(m_watermarkId, (m_document.Watermark as PictureWatermark).WordPicture.ImageRecord);
                        HeaderFooterImages.Add(headerId, headerImages);
                    }
                }
            }
            else
            {
                SerializeBodyItems(header.Items, true);
            }

            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(headerFooterPath, headerStream, false, FileAttributes.Archive);

            SerializeHFRelations(id, headerFooterRelsPath);
        }
        /// <summary>
        /// Serailize the footer and its relations
        /// </summary>
        /// <param name="footer">The Footer</param>
        /// <param name="id">The Footer relationship ID</param>
        /// <param name="headerFooterPath">Path of the Header Footer part</param>
        /// <param name="headerFooterRelsPath">Path of the HeaderFooter relations</param>
        private void SerializeFooter(HeaderFooter footer, string id, string headerFooterPath, string headerFooterRelsPath)
        {
            MemoryStream footerStream = new MemoryStream();
            m_writer = CreateWriter(footerStream);

            m_writer.WriteStartElement("w", "ftr", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "ve", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "a", null, DocxConstants.A_namespace);
            m_writer.WriteAttributeString("xmlns", "pic", null, DocxConstants.PIC_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("xmlns", "wpc", null, DocxConstants.WPC_namesapce);
            m_writer.WriteAttributeString("xmlns", "wp14", null, DocxConstants.WP14_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("xmlns", "wpg", null, DocxConstants.WPG_namespace);
            m_writer.WriteAttributeString("xmlns", "wpi", null, DocxConstants.WPI_namespace);
            m_writer.WriteAttributeString("xmlns", "wps", null, DocxConstants.WPS_namespace);
            m_writer.WriteAttributeString("ve", "Ignorable", null, "w14 w15 wp14");

            SerializeBodyItems(footer.Items, true);

            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(headerFooterPath, footerStream, false, FileAttributes.Archive);

            SerializeHFRelations(id, headerFooterRelsPath);
        }
        /// <summary>
        /// Serializes the HeaderFooter relations
        /// </summary>
        /// <param name="hfId">The headerfooter id.</param>
        /// <param name="headerFooterRelsPath">Path of the HeaderFooter relations</param>
        private void SerializeHFRelations(string hfId, string headerFooterRelsPath)
        {
            bool hasHFImage = HeaderFooterImages.ContainsKey(hfId);
            bool hasHFHyperlinks = HeaderFooterHyperlinks.ContainsKey(hfId);
            bool hasHFInclPics = HeaderFooterInclPicUrls.ContainsKey(hfId);
            if (hasHFImage || hasHFHyperlinks)
            {
                MemoryStream stream = new MemoryStream();
                m_writer = CreateWriter(stream);


                m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);

                if (hasHFImage)
                    SerializeImagesRelations(stream, HeaderFooterImages[hfId]);

                if (hasHFHyperlinks)
                    SerializeHyperlinkRelations(stream, HeaderFooterHyperlinks[hfId]);


                if (hasHFInclPics)
                    SerializeIncludePictureUrlRelations(stream, HeaderFooterInclPicUrls[hfId]);

                if (HFOleContainers.ContainsKey(hfId))
                {
                    AddOLEToZip(HFOleContainers[hfId]);
                }

                if (HFRelations.ContainsKey(hfId))
                    SerializeHFCommonRelations(stream, HFRelations[hfId]);

                m_writer.WriteEndElement();
                m_writer.Flush();

                m_archive.AddItem(headerFooterRelsPath, stream, false, FileAttributes.Archive);
            }
            else
                return;
        }
        /// <summary>
        /// Serializes the Header/Footer common relations.
        /// </summary>
        /// <param name="stream">Stream to write the relations.</param>
        /// <param name="xmlItemsRels">The XML items relations.</param>
        private void SerializeHFCommonRelations(MemoryStream stream, Dictionary<String, DictionaryEntry> xmlItemsRels)
        {
            foreach (KeyValuePair<String, DictionaryEntry> entry in xmlItemsRels)
            {
                string id = entry.Key;
                DictionaryEntry itemRel = (DictionaryEntry)entry.Value;
                string type = (string)itemRel.Key;
                string target = (string)itemRel.Value;
                SerializeRelationShip(stream, id, type, target);
            }
        }
        /// <summary>
        /// Checks whether the TextBodyItem (paragraph) contains only a page field 
        /// </summary>
        /// <param name="item">The TextBodyItem</param>
        /// <returns>return true, if the textBodyItem has a single page field.</returns>
        private bool HasSinglePageField(TextBodyItem item)
        {
            bool hasSinglePageField = false;
            if (item is WParagraph)
            {
                WParagraph para = item as WParagraph;

                if (para.Items.Count > 0 && para.Items.Count <= 4)
                {
                    WField field = para.Items[0] as WField;
                    WFieldMark fieldMark = para.Items[para.Items.Count - 1] as WFieldMark;
                    if (field != null && fieldMark != null &&
                        field.FieldType == FieldType.FieldPage && fieldMark.Type == FieldMarkType.FieldEnd)
                    {
                        hasSinglePageField = true;
                    }
                }
            }
            return hasSinglePageField;
        }
        #endregion HeaderFooters

        #region ZipArchive
        /// <summary>
        /// Adds the charts to zip.
        /// </summary>
        /// <param name="package">The package.</param>
        private void AddChartsToZip(Package package)
        {
            AddToZip(package, DocxConstants.ChartsPath);
        }
        private void AddDiagramToZip(Package package)
        {
            AddToZip(package, DocxConstants.DiagramPath);
        }
        /// <summary>
        /// Adds the controls to zip.
        /// </summary>
        /// <param name="package">The package.</param>
        private void AddControlsToZip(Package package)
        {
            AddToZip(package, DocxConstants.ControlPath);
        }
        /// <summary>
        /// Adds the part container to zip.
        /// </summary>
        /// <param name="package">The package.</param>
        private void AddToZip(Package package, string partPath)
        {
            if (package == null)
                return;

            PartContainer chartCont = package.FindPartContainer(partPath);
            if (chartCont != null)
            {
                AddContainerToZip(chartCont, "word/");
                if (chartCont.Relations.Count > 0)
                {
                    SerializeRelItems(package, chartCont);
                }
            }
        }
        /// <summary>
        /// Serialize the related for part container items.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="chartCont">The chart container.</param>
        private void SerializeRelItems(Package package, PartContainer partContainer)
        {

            XmlReader reader = null;
            Dictionary<string, DictionaryEntry> relations = null;
            foreach (string key in partContainer.Relations.Keys)
            {
                Relations rel = partContainer.Relations[key];
                if (rel.DataStream != null && rel.DataStream.Length > 0)
                {
                    rel.DataStream.Position = 0;
                    reader = XmlReader.Create(rel.DataStream);
                    relations = new Dictionary<string, DictionaryEntry>();
                    ParseRelations(reader, relations);
                    if (relations.Count > 0)
                    {
                        AddPartsToZip(package, relations);
                    }
                }
            }
        }
        /// <summary>
        /// Adds the parts to zip.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="relations">The relations.</param>
        private void AddPartsToZip(Package package, Dictionary<string, DictionaryEntry> relations)
        {
            Part part = null;
            foreach (DictionaryEntry itemEntry in relations.Values)
            {
                string type = (string)itemEntry.Key;
                string partPath = (string)itemEntry.Value;
                if (partPath.IndexOf("file") == -1)
                {
                    partPath = partPath.Replace("..", "word");
                    part = package.FindPart(partPath);
                    if (part != null)
                    {
                        if (m_archive.Find(partPath) == -1 && part.DataStream != null)
                        {
                            m_archive.AddItem(partPath, part.DataStream as MemoryStream, false, FileAttributes.Archive);
                        }
                        //AddToZip(partPath, part.DataStream as MemoryStream);
                    }
                }
            }
        }
        /// <summary>
        /// Parses the relations.
        /// </summary>
        /// <param name="relReader">The rel reader.</param>
        /// <param name="relations">The relations collection.</param>
        private void ParseRelations(XmlReader relReader, Dictionary<string, DictionaryEntry> relations)
        {
            relReader.MoveToContent();
            if (relReader.LocalName != DocxConstants.c_relationshipsTag)
            {
                relReader.ReadInnerXml();
                return;
            }
            if (relReader.IsEmptyElement)
                return;
            DictionaryEntry itemEntry;
            string id = null;
            string target = null;
            string type = null;

            do
            {
                relReader.Read();
                id = relReader.GetAttribute(DocxConstants.c_idTag);
                target = relReader.GetAttribute(DocxConstants.c_targetTag);
                type = relReader.GetAttribute(DocxConstants.c_typeTag);

                if (id != null && target != null && type != null)
                {
                    itemEntry = new DictionaryEntry(type, target);
                    relations.Add(id, itemEntry);
                }
            }
            while (relReader.LocalName != DocxConstants.c_relationshipsTag);
        }
        /// <summary>
        /// Adds the xml items to zip.
        /// </summary>
        /// <param name="package">The package.</param>
        private void AddXmlItemsToZip(Package package)
        {
            if (package == null)
                return;

            AddContainerToZip(package, null);
        }
        /// <summary>
        /// Adds the container to zip.
        /// </summary>
        /// <param name="cont">The cont.</param>
        /// <param name="dataPath">The data path.</param>
        private void AddContainerToZip(PartContainer cont, string dataPath)
        {
            // Save parts
            dataPath += cont.Name;
            foreach (Part part in cont.XmlParts.Values)
            {
                string partPath = dataPath + part.Name;
                if (part.Name.StartsWith("activeX")
                    && !ControlsPathNames.Contains(partPath))
                    ControlsPathNames.Add(partPath);
                if (m_archive.Find(partPath) == -1 && part.DataStream != null)
                {
                    m_archive.AddItem(partPath, part.DataStream as MemoryStream, false, FileAttributes.Archive);
                }
            }

            // Save relations
            foreach (Relations rel in cont.Relations.Values)
            {
                if (m_archive.Find(rel.Name) == -1 && rel.DataStream != null && WriteRel(rel.Name))
                {
                    m_archive.AddItem(rel.Name, rel.DataStream as MemoryStream, false, FileAttributes.Archive);
                    //AddToZip(rel.Name, rel.DataStream as MemoryStream);
                }
            }

            // Save subcontainers
            foreach (PartContainer subcont in cont.XmlPartContainers.Values)
            {
                if (ChartsPathNames.Count > 0)
                {
                    if (subcont.Name == "word/" || subcont.Name == "charts/" || subcont.Name == "embeddings/")
                    {
                        AddContainerToZip(subcont, dataPath);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relPath"></param>
        /// <returns></returns>
        private bool WriteRel(string relPath)
        {
            if (relPath == "word/_rels/numbering.xml.rels")
                return false;

            return true;
        }
        #endregion ZipArchive

        #region Relationship
        /// <summary>
        /// Serializes the Endnote relations
        /// </summary>
        private void SerializeEndnoteRelations()
        {
            MemoryStream endnoteRelStream = new MemoryStream();
            m_writer = CreateWriter(endnoteRelStream);
            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            SerializeImagesRelations(endnoteRelStream, EndnoteImages);
            SerializeHyperlinkRelations(endnoteRelStream, EndnoteHyperlinks);
            m_writer.WriteEndElement();//end of relationships tag
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.EndnotesRelationPath, endnoteRelStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the Footnote relations
        /// </summary>
        private void SerializeFootnoteRelations()
        {
            MemoryStream FootnoteRelStream = new MemoryStream();
            m_writer = CreateWriter(FootnoteRelStream);
            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            SerializeImagesRelations(FootnoteRelStream, FootnoteImages);
            SerializeHyperlinkRelations(FootnoteRelStream, FootnoteHyperlinks);
            m_writer.WriteEndElement();//end of relationships tag
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.FootnotesRelationPath, FootnoteRelStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the Comment relations
        /// </summary>
        private void SerializeCommentRelations()
        {
            MemoryStream commRelStream = new MemoryStream();
            m_writer = CreateWriter(commRelStream);
            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            SerializeImagesRelations(commRelStream, CommentImages);
            SerializeHyperlinkRelations(commRelStream, CommentHyperlinks);
            m_writer.WriteEndElement();//end of relationships tag
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.CommentsRelationPath, commRelStream, false, FileAttributes.Archive);

        }
        /// <summary>
        /// Serializes the document relations.
        /// </summary>
        private void SerializeDocumentRelations()
        {
            MemoryStream docRelstream = new MemoryStream();

            m_writer = CreateWriter(docRelstream);


            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.StylesRelType, "styles.xml");
            SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.SettingsRelType, "settings.xml");

            if (HasNumbering)
            {
                SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.NumberingRelType, "numbering.xml");
            }

            if (m_hasComment)
            {
                SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.CommentsRelType, "comments.xml");
            }
            if (HasEndnote)
            {
                SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.EndnoteRelType, "endnotes.xml");
            }
            if (HasFootnote)
            {
                SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.FootnoteRelType, "footnotes.xml");
            }

            SerializeHeaderFooterRelations(docRelstream);

            if (HasFontTable)
            {
                SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.FontTableRelType, "fontTable.xml");
            }

            SerializeIncludePictureUrlRelations(docRelstream, InclPicFieldUrl);
            //// Creating relationships for every hyperlink and image containing in the document
            SerializeImagesRelations(docRelstream, DocumentImages);

            if (AltChunkTargets.Count > 0)
            {
                SerializeAltChunkRelations(docRelstream, AltChunkTargets);
            }
            if (HasHyperlink && HyperlinkTargets.Count > 0)
            {
                SerializeHyperlinkRelations(docRelstream, HyperlinkTargets);
            }
            if (m_hasOleObject)
            {
                AddOLEToZip(m_document.DocxPackage);
            }
            SerilaizeXmlItemsRelations(docRelstream, XmlItemsRelations);
            if (m_document.CustomXMLContainer != null)
            {
                string[] xmlPartKeys = new string[m_document.CustomXMLContainer.XmlParts.Count];
                m_document.CustomXMLContainer.XmlParts.Keys.CopyTo(xmlPartKeys, 0);
                for (int i = 0; i < xmlPartKeys.Length; i++)
                {
                    string targetPath = m_document.CustomXMLContainer.XmlParts[xmlPartKeys[i]].Name;
                    //"../customXml/item1.xml"
                    if (!targetPath.Contains("Props"))
                        SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.CustomXmlRelType, @"../customXml/" + targetPath);
                }
            }
            if (m_document.HasMacros
                && IsMacroEnabled)
                SerializeRelationShip(docRelstream, GetNextRelationShipID(), DocxConstants.VbaProjectRelType, DocxConstants.VbaProject);
            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.WordRelationPath, docRelstream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the numbering relations.
        /// </summary>
        private void SerializeNumberingsRelation()
        {
            MemoryStream numRelStream = new MemoryStream();
            m_writer = CreateWriter(numRelStream);

            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);

            SerializeImagesRelations(numRelStream, PictureBullets);

            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.NumberingRelationPath, numRelStream, false, FileAttributes.Archive);

        }
        /// <summary>
        /// Serializes the Alternate chunk relations
        /// </summary>
        /// <param name="stream">The memory stream</param>
        /// <param name="altChunkCollection">Collection of alternate chunk relation</param>
        private void SerializeAltChunkRelations(MemoryStream stream, Dictionary<string, string> altChunkCollection)
        {
            string targetString = string.Empty;

            foreach (string id in altChunkCollection.Keys)
            {
                targetString = "/" + altChunkCollection[id];
                SerializeRelationShip(stream, id, DocxConstants.AltChunkRelType, targetString);
            }
        }
        /// <summary>
        /// Serializes the Hyperlink relations
        /// </summary>
        /// <param name="stream">The memory stream</param>
        /// <param name="hyperlinkCollection">Collection of Hyperlink relation</param>
        private void SerializeHyperlinkRelations(MemoryStream stream, Dictionary<string, string> hyperlinkCollection)
        {
            if (hyperlinkCollection != null)
            {
                string targetString = string.Empty;

                foreach (string id in hyperlinkCollection.Keys)
                {
                    targetString = hyperlinkCollection[id];
                    SerializeRelationShip(stream, id, DocxConstants.HyperlinkRelType, targetString);
                }
            }
        }
        /// <summary>
        /// Serializes the image relations
        /// </summary>
        /// <param name="stream">The memory stream</param>
        /// <param name="imageCollection">Collection of images and its relationship id</param>
        private void SerializeImagesRelations(MemoryStream stream, Dictionary<string, ImageRecord> imageCollection)
        {
            if (imageCollection != null)
            {
                string imagePath = string.Empty;
                ImageRecord image;

                foreach (string key in imageCollection.Keys)
                {
                    image = imageCollection[key];

                    if (image == null)
                    {
                        imagePath = DocxConstants.ImagePath + "0.jpeg";
                        SerializeRelationShip(stream, key, DocxConstants.ImageRelType, imagePath.Replace("word\\", ""));
                    }
                    else
                    {
                        imagePath = DocxConstants.ImagePath + image.ImageId + (image.IsMetafile ? ".wmf" : ".jpeg");
                        SerializeRelationShip(stream, key, DocxConstants.ImageRelType, imagePath.Replace("word\\", ""));
                        if (m_archive.Find(imagePath.Replace("\\", "/")) == -1)
                        {
                            m_archive.AddItem(imagePath, new MemoryStream(image.ImageBytes), false, FileAttributes.Archive);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Serializes the HeaderFooters relations to the document relations stream
        /// </summary>
        /// <param name="docRelStream">Document relation stream (Word/.rels/document.xml.rels)</param>
        private void SerializeHeaderFooterRelations(MemoryStream docRelStream)
        {
            SerializeHFRelation(HeaderFooterType.EvenFooter, docRelStream);
            SerializeHFRelation(HeaderFooterType.EvenHeader, docRelStream);
            SerializeHFRelation(HeaderFooterType.FirstPageFooter, docRelStream);
            SerializeHFRelation(HeaderFooterType.FirstPageHeader, docRelStream);
            SerializeHFRelation(HeaderFooterType.OddFooter, docRelStream);
            SerializeHFRelation(HeaderFooterType.OddHeader, docRelStream);
        }
        /// <summary>
        /// Serializes the headers footers relations.
        /// </summary>
        /// <param name="hfType">Type of the HeaderFooter.</param>
        /// <param name="stream">The stream.</param>
        private void SerializeHFRelation(HeaderFooterType hfType, MemoryStream stream)
        {
            string headerFooterPath = string.Empty;
            string relType;

            if (!HeadersFooters.ContainsKey(hfType))
                return;

            Dictionary<string, HeaderFooter> hfColl = HeadersFooters[hfType];
            foreach (string id in hfColl.Keys)
            {
                if (hfType == HeaderFooterType.EvenHeader || hfType == HeaderFooterType.FirstPageHeader ||
                  hfType == HeaderFooterType.OddHeader)
                {
                    headerFooterPath = "header" + id.Replace("rId", "") + ".xml";
                    relType = DocxConstants.HeaderRelType;
                }
                else
                {
                    headerFooterPath = "footer" + id.Replace("rId", "") + ".xml";
                    relType = DocxConstants.FooterRelType;
                }
                SerializeRelationShip(stream, id, relType, headerFooterPath);
            }
        }
        /// <summary>
        /// Serializes the IncludePicture field relations
        /// </summary>
        /// <param name="stream">The memory Stream</param>
        /// <param name="InclPicFieldUrl">Collection of URL that represents the picture mapped through the IncludePicture Field</param>
        private void SerializeIncludePictureUrlRelations(MemoryStream stream, Dictionary<string, string> InclPicFieldUrl)
        {
            if (InclPicFieldUrl != null && InclPicFieldUrl.Count > 0)
            {
                foreach (string key in InclPicFieldUrl.Keys)
                {
                    string url = InclPicFieldUrl[key];
                    SerializeRelationShip(stream, key, DocxConstants.ImageRelType, url);
                }
            }
        }
        /// <summary>
        /// Serializes the XML Item relations
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="xmlItemsRels"></param>
        private void SerilaizeXmlItemsRelations(MemoryStream stream, Dictionary<string, DictionaryEntry> xmlItemsRels)
        {
            if (xmlItemsRels.Count == 0)
                return;

            DictionaryEntry itemRel;
            string target = string.Empty;
            string type = string.Empty;

            foreach (string id in xmlItemsRels.Keys)
            {
                itemRel = xmlItemsRels[id];
                type = (string)itemRel.Key;
                target = (string)itemRel.Value;

                SerializeRelationShip(stream, id, type, target);
            }
        }
        /// <summary>
        /// Adds the OleObject (*.Bin) into the package
        /// </summary>
        /// <param name="OleContainers">Collection of OLE Containers</param>
        private void AddOLEToZip(Dictionary<string, Stream> OleContainers)
        {
            string path = null;
            if (OleContainers.Count > 0)
            {
                foreach (KeyValuePair<String, Stream> entry in OleContainers)
                {
                    path = DocxConstants.EmbeddingPath + entry.Key;
                    m_archive.AddItem(path, entry.Value, false, FileAttributes.Archive);
                }
            }
        }
        /// <summary>
        /// Adds the OLE objects to zip.
        /// </summary>
        /// <param name="package">The package.</param>
        private void AddOLEToZip(Package package)
        {
            string path = null;
            if (OleContainers.Count > 0)
            {
                foreach (KeyValuePair<String, Stream> entry in OleContainers)
                {
                    path = DocxConstants.EmbeddingPath + entry.Key;
                    m_archive.AddItem(path, entry.Value, false, FileAttributes.Archive);
                }
            }

            // Add to zip preserved 
            AddToZip(package, DocxConstants.DefaultEmbeddingPath);
        }
        #endregion Relationship

        #region Background
        /// <summary>
        /// Serialize the document background
        /// </summary>
        /// <param name="background">The document background</param>
        private void SerializeBackground(Background background)
        {
            if (background.Type != BackgroundType.NoBackground)
            {
                m_writer.WriteStartElement("background", DocxConstants.W_namespace);

                switch (background.Type)
                {
                    case BackgroundType.Color:
                        m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(background.Color));
                        break;
                    case BackgroundType.Gradient:
                        BackgroundGradient gradient = background.Gradient;
                        m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(gradient.Color1));
                        SerializeGradient(gradient);
                        break;
                    case BackgroundType.Picture:
                    case BackgroundType.Texture:
                        if (background.ImageBytes == null)
                            break;
                        m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(background.Color));
                        m_writer.WriteStartElement("background", DocxConstants.V_namespace);
                        m_writer.WriteStartElement("fill", DocxConstants.V_namespace);

                        WPicture pic = new WPicture(background.Document);
                        pic.LoadImage(background.ImageBytes);
                        UpdateImages(pic);
                        string picId = AddImageRelation(DocumentImages, pic.ImageRecord);
                        m_writer.WriteAttributeString("id", DocxConstants.R_namespace, picId);
                        m_writer.WriteAttributeString("title", DocxConstants.O_namespace, "");
                        m_writer.WriteAttributeString("color2", "#" + GetRGBCode(background.Color));
                        m_writer.WriteAttributeString("type",
                          (background.Type == BackgroundType.Picture) ? "frame" : "tile"
                          );
                        m_writer.WriteEndElement();
                        m_writer.WriteEndElement();
                        break;
                    default:
                        break;
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the background gradient
        /// </summary>
        /// <param name="gradient">The backgroundGradient</param>
        private void SerializeGradient(BackgroundGradient gradient)
        {
            m_writer.WriteStartElement("background", DocxConstants.V_namespace);

            if (gradient.Color1 != Color.White)
            {
                m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(gradient.Color1));
            }

            m_writer.WriteStartElement("fill", DocxConstants.V_namespace);

            SerializeGradientColor(gradient);
            SerializeGradientShadings(gradient);

            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the gradient shadings
        /// </summary>
        /// <param name="gradient">The background gradients</param>
        private void SerializeGradientShadings(BackgroundGradient gradient)
        {
            if (gradient.ShadingStyle != GradientShadingStyle.Horizontal)
            {
                switch (gradient.ShadingStyle)
                {
                    case GradientShadingStyle.Vertical:
                        m_writer.WriteAttributeString("angle", "-90");
                        break;
                    case GradientShadingStyle.DiagonalUp:
                        m_writer.WriteAttributeString("angle", "-135");
                        break;
                    case GradientShadingStyle.DiagonalDown:
                    case GradientShadingStyle.FromCorner:
                    case GradientShadingStyle.FromCenter:
                        m_writer.WriteAttributeString("angle", "-45");
                        break;
                }
            }
            m_writer.WriteAttributeString("method", "linear sigma");

            if (gradient.ShadingStyle == GradientShadingStyle.FromCorner)
            {
                m_writer.WriteAttributeString("focus", "100%");
            }
            else if (gradient.ShadingVariant == GradientShadingVariant.ShadingMiddle)
            {
                m_writer.WriteAttributeString("focus", "50%");
            }
            else if (gradient.ShadingVariant == GradientShadingVariant.ShadingOut)
            {
                m_writer.WriteAttributeString("focus", "-50%");
            }
            else if (gradient.ShadingVariant == GradientShadingVariant.ShadingUp)
            {
                m_writer.WriteAttributeString("focus", "100%");
            }

            if (gradient.ShadingStyle == GradientShadingStyle.FromCenter)
            {
                m_writer.WriteAttributeString("type", "gradientRadial");
            }
            else
            {
                m_writer.WriteAttributeString("type", "gradient");
            }

            if (gradient.ShadingStyle == GradientShadingStyle.FromCorner &&
              gradient.ShadingVariant != GradientShadingVariant.ShadingUp ||
              gradient.ShadingStyle == GradientShadingStyle.FromCenter)
            {
                if (gradient.ShadingStyle == GradientShadingStyle.FromCorner)
                {
                    if (gradient.ShadingVariant == GradientShadingVariant.ShadingDown)
                    {
                        m_writer.WriteAttributeString("focusposition", "1");
                    }
                    else if (gradient.ShadingVariant == GradientShadingVariant.ShadingOut)
                    {
                        m_writer.WriteAttributeString("focusposition", ",1");
                    }
                    else if (gradient.ShadingVariant == GradientShadingVariant.ShadingMiddle)
                    {
                        m_writer.WriteAttributeString("focusposition", "1,1");
                    }
                }
                else
                {
                    m_writer.WriteAttributeString("focusposition", ".5,.5");
                }

                m_writer.WriteAttributeString("focussize", "");
            }

            if (gradient.ShadingStyle == GradientShadingStyle.FromCorner ||
              (gradient.ShadingStyle == GradientShadingStyle.FromCenter
              && gradient.ShadingVariant == GradientShadingVariant.ShadingDown))
            {
                m_writer.WriteStartElement("fill", DocxConstants.O_namespace);
                m_writer.WriteAttributeString("ext", "view");
                m_writer.WriteAttributeString("type", "gradientCenter");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the gradient color
        /// </summary>
        /// <param name="gradient">The background gradient</param>
        private void SerializeGradientColor(BackgroundGradient gradient)
        {
            if (gradient.Color2 != Color.White)
            {
                if (gradient.Color2.Name[0] == 'e')
                {
                    int number = int.Parse(gradient.Color2.Name.Substring(6), NumberStyles.HexNumber);

                    if (gradient.Color2.Name[5] == '1')
                        m_writer.WriteAttributeString("color2", "fill darken(" + number + ")");
                    else
                        m_writer.WriteAttributeString("color2", "fill lighten(" + number + ")");
                }
                else
                {
                    m_writer.WriteAttributeString("color2", "#" + GetRGBCode(gradient.Color2));
                }
            }
        }
        /// <summary>
        /// Ensure the presence of metafiles and image presence in the document
        /// </summary>
        /// <param name="pic"></param>
        private void UpdateImages(WPicture pic)
        {
            if (pic.IsMetaFile)
                m_hasMetafiles = true;
            else
                m_hasImages = true;
        }
        #endregion Background

        #region Document Body
        /// <summary>
        /// Serializes the document body
        /// </summary>
        private void SerializeDocumentBody()
        {
            if (m_document.Sections.Count == 0)
            {
                throw new Exception("There are no sections present in the document");
            }
            m_writer.WriteStartElement("body", DocxConstants.W_namespace);
            foreach (WSection section in m_document.Sections)
            {
                SerializeSection(section);
            }
            m_writer.WriteEndElement();
        }
        #region Section
        /// <summary>
        /// Serializes the Section.
        /// </summary>
        /// <param name="section">The WSection</param>
        private void SerializeSection(WSection section)
        {
            bool isLastSection = false;
            if (section.NextSibling == null)
                isLastSection = true;

            if (section.Body.ChildEntities.Count == 0 && !isLastSection)
                section.AddParagraph();

            if (section.Body.ChildEntities.LastItem is WTable && !isLastSection)
                section.AddParagraph();

            SerializeBodyItems(section.Body.Items, isLastSection);

            if (isLastSection)
                SerializeSectionProperties(section);
        }
        /// <summary>
        /// Serializes the bodyItems
        /// </summary>
        /// <param name="bodyItemCollection">Collection of Body items</param>
        /// <param name="isLastSection">True, if the body items present in the last section of the document.</param>
        private void SerializeBodyItems(BodyItemCollection bodyItemCollection, bool isLastSection)
        {
            for (int i = 0; i < bodyItemCollection.Count; i++)
            {
                SerializeBodyItem(bodyItemCollection[i], isLastSection);
            }
        }
        /// <summary>
        /// Serialize the TextBody item
        /// </summary>
        /// <param name="item">The textBody item (paragraph or table)</param>
        /// <param name="isLastSection">True, if the body items present in the last section of the document.</param>
        private void SerializeBodyItem(TextBodyItem item, bool isLastSection)
        {
            if (item == null)
                throw new ArgumentException("BodyItem should not be null");

            switch (item.EntityType)
            {
                case EntityType.Paragraph:
                    SerializeParagraph(item as WParagraph, isLastSection);
                    break;
                case EntityType.Table:
                    SerializeTable(item as WTable);
                    break;
                case EntityType.StructureDocumentTag:
                    SerializeStructureDocumentTagBlock(item as StructureDocumentTagBlock);
                    break;
                case EntityType.AlternateChunk:
                    SerializeAlternateChunk(item as AlternateChunk);
                    break;
            }
        }
        #endregion Section

        # region StructureDocumentTag
        private void SerializeStructureDocumentTagInline(StructureDocumentTagInline sdTagInline)
        {
            m_writer.WriteStartElement("sdt", DocxConstants.W_namespace);
            SerializeSDTProperties(sdTagInline.SDTProperties);

            if (sdTagInline.BreakCharacterFormat != null)
            {
                m_writer.WriteStartElement("sdtEndPr", DocxConstants.W_namespace);
                SerializeCharactetFormat(sdTagInline.BreakCharacterFormat);
                m_writer.WriteEndElement();
            }
            SerializeSDTContentInline(sdTagInline.SDTContent.ParagraphItems);

            m_writer.WriteEndElement();

        }
        /// <summary>
        /// Serialize SDT content inline
        /// </summary>
        /// <param name="body"></param>
        private void SerializeSDTContentInline(ParagraphItemCollection paraItems)
        {
            m_writer.WriteStartElement("sdtContent", DocxConstants.W_namespace);
            for (int i = 0; i < paraItems.Count; i++)
            {
                SerializeParagraphItem(paraItems[i]);
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Seralize Alternate content
        /// </summary>
        /// <param name="sdTagBlock"></param>
        private void SerializeAlternateChunk(AlternateChunk altChunk)
        {
            AltChunkTargets.Add(altChunk.TargetId, altChunk.ContentPath);
            if (!AltChunkContentTypes.ContainsKey(altChunk.ContentExtension))
                AltChunkContentTypes.Add(altChunk.ContentExtension, altChunk.ContentType);
            m_writer.WriteStartElement("w", "altChunk", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("r", "id", DocxConstants.R_namespace, altChunk.TargetId);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Seralize structure document tag block
        /// </summary>
        /// <param name="sdTagBlock"></param>
        private void SerializeStructureDocumentTagBlock(StructureDocumentTagBlock sdTagBlock)
        {
            m_writer.WriteStartElement("sdt", DocxConstants.W_namespace);
            SerializeSDTProperties(sdTagBlock.SDTProperties);

            if (sdTagBlock.BreakCharacterFormat != null)
            {
                m_writer.WriteStartElement("sdtEndPr", DocxConstants.W_namespace);
                SerializeCharactetFormat(sdTagBlock.BreakCharacterFormat);
                m_writer.WriteEndElement();
            }
            SerializeSDTContent(sdTagBlock.SDTContent.TextBody);

            m_writer.WriteEndElement();

        }
        /// <summary>
        /// Serialize structure document tag properties
        /// </summary>
        /// <param name="properties"></param>
        private void SerializeSDTProperties(SDTProperties properties)
        {
            m_writer.WriteStartElement("sdtPr", DocxConstants.W_namespace);
            if (properties.SDTType != StructureDocumentType.None)
                SerializeSDTType(properties);

            if (properties.ID != null && properties.ID != string.Empty)
            {
                m_writer.WriteStartElement("id", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, properties.ID);
                m_writer.WriteEndElement();
            }
            if (properties.CharacterFormat != null)
            {
                SerializeCharactetFormat(properties.CharacterFormat);
            }
            if (properties.Alias != null && properties.Alias != string.Empty)
            {
                m_writer.WriteStartElement("alias", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, properties.Alias);
                m_writer.WriteEndElement();
            }
            if (properties.IsShowingPlaceHolder)
            {
                m_writer.WriteStartElement("showingPlcHdr", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }

            if (properties.Bibliograph)
            {
                m_writer.WriteStartElement("bibliography", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }

            if (properties.Citation)
            {
                m_writer.WriteStartElement("citation", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }

            if (properties.LockSettings != LockSettings.UnLocked)
                SerializeSDTLockSetting(properties.LockSettings);

            if (properties.Date != null)
            {
                SerializeSDTDate(properties.Date);
            }

            if (properties.DataBinding != null && properties.SDTType != StructureDocumentType.RichText)
            {
                SerializeSDTDataBinding(properties.DataBinding);
            }
            if (properties.DocPartObj != null)
            {
                m_writer.WriteStartElement("docPartObj", DocxConstants.W_namespace);
                SerializeDocPartItem(properties.DocPartObj as DocPartItem);
                m_writer.WriteEndElement();
            }
            if (properties.DocPartList != null)
            {
                m_writer.WriteStartElement("docPartList", DocxConstants.W_namespace);
                SerializeDocPartItem(properties.DocPartList as DocPartItem);
                m_writer.WriteEndElement();
            }
            switch (properties.ContentRepeatingType)
            {
                case ContentRepeatingType.RepeatingSection:
                    m_writer.WriteStartElement("repeatingSection", DocxConstants.W15_namespace);
                    m_writer.WriteEndElement();
                    break;
                case ContentRepeatingType.RepeatingSectionItem:
                    m_writer.WriteStartElement("repeatingSectionItem", DocxConstants.W15_namespace);
                    m_writer.WriteEndElement();
                    break;
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize Doc Part obj and Doc part list Child elements
        /// </summary>
        /// <param name="docPartItem"></param>
        private void SerializeDocPartItem(DocPartItem docPartItem)
        {
            if (docPartItem.DocPartGallery != null)
            {
                m_writer.WriteStartElement("docPartGallery", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, docPartItem.DocPartGallery);
                m_writer.WriteEndElement();
            }
            if (docPartItem.DocPartCategory != null)
            {
                m_writer.WriteStartElement("docPartCategory", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, docPartItem.DocPartCategory);
                m_writer.WriteEndElement();
            }
            if (docPartItem.IsDocPartUnique)
            {
                m_writer.WriteStartElement("docPartList", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize SDT data binding
        /// </summary>
        /// <param name="dataBinding"></param>
        private void SerializeSDTDataBinding(SDTDataBinding dataBinding)
        {
            m_writer.WriteStartElement("dataBinding", DocxConstants.W_namespace);

            if (dataBinding.PrefixMapping != null && dataBinding.PrefixMapping != string.Empty)
                m_writer.WriteAttributeString("w", "prefixMappings", DocxConstants.W_namespace, dataBinding.PrefixMapping);

            if (dataBinding.XPath != null && dataBinding.XPath != string.Empty)
                m_writer.WriteAttributeString("w", "xpath", DocxConstants.W_namespace, dataBinding.XPath);

            if (dataBinding.StoreItemID != null && dataBinding.StoreItemID != string.Empty)
                m_writer.WriteAttributeString("w", "storeItemID", DocxConstants.W_namespace, dataBinding.StoreItemID);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize SDT lock settings
        /// </summary>
        /// <param name="lockSetting"></param>
        private void SerializeSDTLockSetting(LockSettings lockSetting)
        {
            m_writer.WriteStartElement("lock", DocxConstants.W_namespace);
            switch (lockSetting)
            {
                case LockSettings.ContentLocked:
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "contentLocked");
                    break;
                case LockSettings.SDTContentLocked:
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "sdtContentLocked");
                    break;
                case LockSettings.SDTLocked:
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "sdtLocked");
                    break;
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize SDT type
        /// </summary>
        /// <param name="type"></param>
        private void SerializeSDTType(SDTProperties properties)
        {
            switch (properties.SDTType)
            {
                case StructureDocumentType.Equation:
                    m_writer.WriteStartElement("equation", DocxConstants.W_namespace);
                    m_writer.WriteEndElement();
                    break;
                case StructureDocumentType.Text:
                    m_writer.WriteStartElement("text", DocxConstants.W_namespace);
                    m_writer.WriteEndElement();
                    break;
                case StructureDocumentType.Picture:
                    m_writer.WriteStartElement("picture", DocxConstants.W_namespace);
                    m_writer.WriteEndElement();
                    break;
                case StructureDocumentType.ComboBox:
                    m_writer.WriteStartElement("comboBox", DocxConstants.W_namespace);
                    SerializeSDTComboBox(properties.SDTComboBox);
                    m_writer.WriteEndElement();
                    break;
                case StructureDocumentType.DropDownList:
                    m_writer.WriteStartElement("dropDownList", DocxConstants.W_namespace);
                    SerializeSDTDropDownList(properties.SDTDropDownList);
                    m_writer.WriteEndElement();
                    break;
                case StructureDocumentType.RichText:
                    m_writer.WriteStartElement("richText", DocxConstants.W_namespace);
                    m_writer.WriteEndElement();
                    break;
                case StructureDocumentType.CheckBox:
                    m_writer.WriteStartElement("checkbox", DocxConstants.W14_namespace);
                    SerializeSDTCheckBox(properties.SDTCheckBox);
                    m_writer.WriteEndElement();
                    break;
            }
        }
        /// <summary>
        /// Serialize SDT Check box
        /// </summary>
        /// <param name="sdtCheckBox">check box</param>
        private void SerializeSDTCheckBox(SDTCheckBox sdtCheckBox)
        {
            if (sdtCheckBox != null)
            {
                m_writer.WriteStartElement("checked", DocxConstants.W14_namespace);
                if (sdtCheckBox.IsChecked)
                    m_writer.WriteAttributeString("val", DocxConstants.W14_namespace, "1");
                else
                    m_writer.WriteAttributeString("val", DocxConstants.W14_namespace, "0");
                m_writer.WriteEndElement();

                m_writer.WriteStartElement("checkedState", DocxConstants.W14_namespace);

                if (sdtCheckBox.CheckedState.Value != null)
                    m_writer.WriteAttributeString("val", DocxConstants.W14_namespace, sdtCheckBox.CheckedState.Value);
                if (sdtCheckBox.CheckedState.Font != null)
                    m_writer.WriteAttributeString("font", DocxConstants.W14_namespace, sdtCheckBox.CheckedState.Font);
                m_writer.WriteEndElement();

                m_writer.WriteStartElement("uncheckedState", DocxConstants.W14_namespace);

                if (sdtCheckBox.UncheckedState.Value != null)
                    m_writer.WriteAttributeString("val", DocxConstants.W14_namespace, sdtCheckBox.UncheckedState.Value);
                if (sdtCheckBox.UncheckedState.Font != null)
                    m_writer.WriteAttributeString("font", DocxConstants.W14_namespace, sdtCheckBox.UncheckedState.Font);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize SDTDropDownList
        /// </summary>
        /// <param name="comboBox"></param>
        private void SerializeSDTDropDownList(SDTDropDownList dropDownList)
        {
            if (dropDownList.LastValue != null && dropDownList.LastValue != string.Empty)
                m_writer.WriteAttributeString("w", "lastValue", DocxConstants.W_namespace, dropDownList.LastValue);

            foreach (ListItem listItem in dropDownList.ListItems)
            {
                m_writer.WriteStartElement("listItem", DocxConstants.W_namespace);
                if (listItem.DisplayText != null && listItem.DisplayText != string.Empty)
                    m_writer.WriteAttributeString("w", "displayText", DocxConstants.W_namespace, listItem.DisplayText);

                if (listItem.Value != null && listItem.Value != string.Empty)
                    m_writer.WriteAttributeString("w", "value", DocxConstants.W_namespace, listItem.Value);

                m_writer.WriteEndElement();
            }

        }
        /// <summary>
        /// Serialize SDTComboBox
        /// </summary>
        /// <param name="comboBox"></param>
        private void SerializeSDTComboBox(SDTComboBox comboBox)
        {
            if (comboBox.LastValue != null && comboBox.LastValue != string.Empty)
                m_writer.WriteAttributeString("w", "lastValue", DocxConstants.W_namespace, comboBox.LastValue);

            foreach (ListItem listItem in comboBox.ListItems)
            {
                m_writer.WriteStartElement("listItem", DocxConstants.W_namespace);
                if (listItem.DisplayText != null && listItem.DisplayText != string.Empty)
                    m_writer.WriteAttributeString("w", "displayText", DocxConstants.W_namespace, listItem.DisplayText);

                if (listItem.Value != null && listItem.Value != string.Empty)
                    m_writer.WriteAttributeString("w", "value", DocxConstants.W_namespace, listItem.Value);

                m_writer.WriteEndElement();
            }

        }
        /// <summary>
        /// Serialize SDT date
        /// </summary>
        /// <param name="date"></param>
        private void SerializeSDTDate(SDTDate date)
        {
            m_writer.WriteStartElement("date", DocxConstants.W_namespace);
            if (date.FullDate != null && date.FullDate != string.Empty)
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, date.FullDate);

            if (date.CalendarType != CalendarType.None)
            {
                m_writer.WriteStartElement("calendar", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetCalenderType(date.CalendarType));
                m_writer.WriteEndElement();
            }

            if (date.DateFormat != null && date.DateFormat != string.Empty)
            {
                m_writer.WriteStartElement("dateFormat", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, date.DateFormat);
                m_writer.WriteEndElement();
            }

            if (date.LID != null && date.LID != string.Empty)
            {
                m_writer.WriteStartElement("lid", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, date.LID);
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();//End element of date
        }
        /// <summary>
        /// Get Calender type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetCalenderType(CalendarType type)
        {
            switch (type)
            {
                case CalendarType.Gregorian:
                    return "gregorian";
                case CalendarType.GregorianArabic:
                    return "gregorianArabic";
                case CalendarType.GregorianMiddleEastFrench:
                    return "gregorianMeFrench";
                case CalendarType.GregorianEnglish:
                    return "gregorianUs";
                case CalendarType.GregorianTransliteratedEnglish:
                    return "gregorianXlitEnglish";
                case CalendarType.GregorianTransliteratedFrench:
                    return "gregorianXlitFrench";
                case CalendarType.Hebrew:
                    return "hebrew";
                case CalendarType.Hijri:
                    return "hijri";
                case CalendarType.Japan:
                    return "japan";
                case CalendarType.Korean:
                    return "korea";
                case CalendarType.Saka:
                    return "saka";
                case CalendarType.Taiwan:
                    return "taiwan";
                case CalendarType.Thai:
                    return "thai";
                default:
                    return null;
            }



        }
        /// <summary>
        /// Serialize SDT content
        /// </summary>
        /// <param name="body"></param>
        private void SerializeSDTContent(WTextBody body)
        {
            m_writer.WriteStartElement("sdtContent", DocxConstants.W_namespace);
            TextBodyItem bodyItem = null;
            for (int i = 0; i < body.Items.Count; i++)
            {
                bodyItem = body.Items[i];
                SerializeBodyItem(bodyItem, false);
            }
            m_writer.WriteEndElement();
        }
        # endregion

        #region Table
        /// <summary>
        /// Serialize the table
        /// </summary>
        /// <param name="table">The table</param>
        private void SerializeTable(WTable table)
        {
            if (table.Rows.Count <= 0)
                return;
            m_writer.WriteStartElement("tbl", DocxConstants.W_namespace);

            if (table.Rows.Count != 0)
            {
                RowFormat table_format = table.DocxTableFormat.HasFormat ? table.DocxTableFormat.Format : table.Rows[0].RowFormat;
                SerializeTableFormat(table_format, table);
            }
            else
            {
                SerializeTableFormat(table.DocxTableFormat.Format, table);
            }


            SerializeTableGrid(table);
            SerializeTableRows(table.Rows);

            m_writer.WriteEndElement();//end of table
        }
        /// <summary>
        /// Serialize the table rows
        /// </summary>
        /// <param name="rows">The table row collection</param>
        private void SerializeTableRows(WRowCollection rows)
        {
            if (rows.Count > 0)
            {
                bool isIns = (rows[0].OwnerTable != null && (rows[0].OwnerTable as TextBodyItem).IsInsertRevision);
                bool isDel = (rows[0].OwnerTable != null && (rows[0].OwnerTable as TextBodyItem).IsDeleteRevision);
                foreach (WTableRow row in rows)
                {
                    if (row.SDTRow != null)
                    {
                        m_writer.WriteStartElement("sdt", DocxConstants.W_namespace);
                        SerializeSDTProperties(row.SDTRow.SDTProperties);
                        if (row.SDTRow.BreakCharacterFormat != null)
                        {
                            m_writer.WriteStartElement("sdtEndPr", DocxConstants.W_namespace);
                            SerializeCharactetFormat(row.SDTRow.BreakCharacterFormat);
                            m_writer.WriteEndElement();
                        }
                        m_writer.WriteStartElement("sdtContent", DocxConstants.W_namespace);
                        SerializeRow(row, isIns, isDel);
                        m_writer.WriteEndElement();
                        m_writer.WriteEndElement();
                    }
                    else
                        SerializeRow(row, isIns, isDel);
                }
            }
        }
        /// <summary>
        /// Serialize the table row
        /// </summary>
        /// <param name="row">The table row</param>
        /// <param name="isIns"></param>
        /// <param name="isDel"></param>
        private void SerializeRow(WTableRow row, bool isIns, bool isDel)
        {
            m_writer.WriteStartElement("tr", DocxConstants.W_namespace);
            SerializeRowFormat(row);

            SerializeCells(row.Cells);

            m_writer.WriteEndElement();//end od table row "tr"
        }
        /// <summary>
        /// serialize the table cells
        /// </summary>
        /// <param name="cells">The table cell collection</param>
        private void SerializeCells(WCellCollection cells)
        {
            foreach (WTableCell cell in cells)
            {
                if (cell.SDTCell != null)
                {
                    m_writer.WriteStartElement("sdt", DocxConstants.W_namespace);
                    SerializeSDTProperties(cell.SDTCell.SDTProperties);
                    if (cell.SDTCell.BreakCharacterFormat != null)
                    {
                        m_writer.WriteStartElement("sdtEndPr", DocxConstants.W_namespace);
                        SerializeCharactetFormat(cell.SDTCell.BreakCharacterFormat);
                        m_writer.WriteEndElement();
                    }
                    m_writer.WriteStartElement("sdtContent", DocxConstants.W_namespace);
                    SerializeCell(cell);
                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                }
                else
                    SerializeCell(cell);
            }
        }
        /// <summary>
        /// Serialize the table cell
        /// </summary>
        /// <param name="cell">The table cell</param>
        private void SerializeCell(WTableCell cell)
        {
            m_writer.WriteStartElement("tc", DocxConstants.W_namespace);
            SerializeCellFormat(cell.CellFormat);
            if (cell.Items.Count > 0)
            {
                int itemIndex = 0;
                TextBodyItem item = null;
                while (itemIndex < cell.Items.Count)
                {
                    item = cell.Items[itemIndex];
                    if (item is WParagraph && itemIndex == cell.Items.Count - 1)
                    {
                        WParagraph para = item as WParagraph;
                        if (para.BreakCharacterFormat.Sprms != null)
                        {
                            MergeCharProps(para.BreakCharacterFormat, cell.CharacterFormat);
                        }
                    }
                    SerializeBodyItem(item, false);
                    itemIndex += 1;
                }

                if (cell.Items.LastItem.EntityType == EntityType.Table)
                {
                    m_writer.WriteStartElement("p", DocxConstants.W_namespace);
                    m_writer.WriteEndElement();
                }
            }
            else
            {
                m_writer.WriteStartElement("p", DocxConstants.W_namespace);
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                m_writer.WriteStartElement("pStyle", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "Normal");
                m_writer.WriteEndElement();//end of pStyle
                SerializeCharactetFormat(cell.CharacterFormat);

                m_writer.WriteEndElement();//end of pPr
                m_writer.WriteEndElement();//end of P
            }
            m_writer.WriteEndElement();//end of table cell "tc"
        }
        /// <summary>
        /// Merge the characterFormats
        /// </summary>
        /// <param name="ParaBreakCharFormat">The paragraph's break character format</param>
        /// <param name="cellCharacterFormat">The cell character format</param>
        private void MergeCharProps(WCharacterFormat ParaBreakCharFormat, WCharacterFormat cellCharacterFormat)
        {
            SinglePropertyModifierArray destSprms = ParaBreakCharFormat.Sprms;
            SinglePropertyModifierArray sourceSprms = cellCharacterFormat.Sprms;
            SinglePropertyModifierRecord sprm = null;

            if (sourceSprms == null || destSprms == null)
                return;

            for (int i = 0, cnt = sourceSprms.Count; i < cnt; i++)
            {
                sprm = sourceSprms.GetSprmByIndex(i);
                if (destSprms[sprm.TypedOptions] == null)
                {
                    destSprms.Modifiers.Add(sprm);
                }
            }
        }
        /// <summary>
        /// Serialize the cell formatting
        /// </summary>
        /// <param name="cellFormat">The cell format</param>
        private void SerializeCellFormat(CellFormat cellFormat)
        {
            List<Stream> tempDocxProps = new List<Stream>();
            for (int i = 0, cnt = cellFormat.XmlProps.Count; i < cnt; i++)
                tempDocxProps.Add(cellFormat.XmlProps[i]);
            WTableCell cell = cellFormat.OwnerBase as WTableCell;
            //Get the table fomat
            RowFormat tf = cell.OwnerRow.OwnerTable.TableFormat;
            //Get the row format
            RowFormat rf = cell.OwnerRow.RowFormat;
            m_writer.WriteStartElement("tcPr", DocxConstants.W_namespace);
            //w:cnfStyle -   Table Cell Conditional Formatting
            SerializeCnfStyleElement(cell);
            //w:tcW -    Preferred Table Cell Width
            SerializeCellWidth(cell);
            //w:gridSpan -   Grid Columns Spanned by Current Table Cell
            SerializeGridSpan(cell);
            //w:hMerge -    Horizontally Merged Cell and w:vMerge -    Vertically Merged Cell
            SerializeCellMerge(cellFormat);
            //w:tcBorders -    Table Cell Borders
            m_writer.WriteStartElement("tcBorders", DocxConstants.W_namespace);
            SerializeBorders(cellFormat.Borders, 8);
            m_writer.WriteEndElement();
            //w:shd -  Table Cell Shading
            SerializeCellShading(cell, tf, rf);
            //w:noWrap -   Don't Wrap Cell Content
            if (cellFormat.HasValue(CellFormat.TextWrapKey))
            {
                m_writer.WriteStartElement("noWrap", DocxConstants.W_namespace);
                if (cellFormat.TextWrap)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            //w:tcMar -  Single Table Cell Margins
            if (!cellFormat.SamePaddingsAsTable)
            {
                m_writer.WriteStartElement("tcMar", DocxConstants.W_namespace);
                SerializePaddings(cellFormat.Paddings);
                m_writer.WriteEndElement();
            }
            //w:textDirection -   Table Cell Text Flow Direction
            SerializeTableCellDirection(cellFormat);
            //w:tcFitText -  Fit Text Within Cell
            if (cellFormat.FitText)
            {
                m_writer.WriteStartElement("tcFitText", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "true");
                m_writer.WriteEndElement();
            }
            //w:vAlign -  Table Cell Vertical Alignment
            SerializeCellVerticalAlign(cellFormat.VerticalAlignment);
            //w:hideMark -   Ignore End Of Cell Marker In Row Height Calculation
            SerializeDocxProps(tempDocxProps, "hideMark");
            //w:cellIns -    Table Cell Insertion
            SerializeDocxProps(tempDocxProps, "cellIns");
            //w:cellDel -    Table Cell Deletion
            SerializeDocxProps(tempDocxProps, "cellDel");
            //w:cellMerge -   Vertically Merged/Split Table Cells
            SerializeDocxProps(tempDocxProps, "cellMerge");
            if (cell.m_trackCellFormat != null && !m_isAlternativeCellFormat)
            {
                m_isAlternativeCellFormat = true;
                SerializeTrackChangeProps("tcPrChange");
                SerializeCellFormat(cell.TrackCellFormat);
                m_writer.WriteEndElement();
                m_isAlternativeCellFormat = false;
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the cell vertical alignment
        /// </summary>
        /// <param name="alignment"></param>
        private void SerializeCellVerticalAlign(VerticalAlignment alignment)
        {
            if (alignment != VerticalAlignment.Top)
            {
                m_writer.WriteStartElement("vAlign", DocxConstants.W_namespace);

                switch (alignment)
                {
                    case VerticalAlignment.Top:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "top");
                        break;
                    case VerticalAlignment.Middle:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "center");
                        break;
                    case VerticalAlignment.Bottom:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "bottom");
                        break;
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the table cell direction
        /// </summary>
        /// <param name="cellFormat"></param>
        private void SerializeTableCellDirection(CellFormat cellFormat)
        {
            if (cellFormat.TextDirection != TextDirection.Horizontal)
            {
                m_writer.WriteStartElement("textDirection", DocxConstants.W_namespace);

                switch (cellFormat.TextDirection)
                {
                    case TextDirection.Horizontal:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "lrTb");
                        break;
                    case TextDirection.VerticalBottomToTop:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "btLr");
                        break;
                    case TextDirection.VerticalTopToBottom:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "tbRl");
                        break;
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the cell shading
        /// </summary>
        /// <param name="cell">The table cell</param>
        /// <param name="tf">The parent table format</param>
        /// <param name="rf">The parent row format</param>
        private void SerializeCellShading(WTableCell cell, RowFormat tf, RowFormat rf)
        {
            CellFormat cf = cell.CellFormat;
            if (cf.HasValue(CellFormat.ShadingColorKey)
                || cf.HasValue(CellFormat.ForeColorKey)
                || cf.HasValue(CellFormat.TextureStyleKey))
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                if (cf.BackColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }
                else
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(cf.BackColor));
                }

                if (cell.ForeColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "auto");
                }
                else
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(cell.ForeColor));
                }

                string val = GetTextureStyle(cell.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);
                m_writer.WriteEndElement();
            }
            else if (rf.HasValue(RowFormat.ShadingColorKey) || rf.HasValue(RowFormat.TextureStyleKey))
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                if (rf.BackColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }
                else
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(rf.BackColor));
                }

                string val = GetTextureStyle(rf.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);
                m_writer.WriteEndElement();
            }
            else if (tf.HasValue(RowFormat.ShadingColorKey) || tf.HasValue(RowFormat.TextureStyleKey))
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                if (tf.BackColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }
                else
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(tf.BackColor));
                }

                string val = GetTextureStyle(tf.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize cell merge
        /// </summary>
        /// <param name="cellFormat">The cell format</param>
        private void SerializeCellMerge(CellFormat cellFormat)
        {
            if (cellFormat.HorizontalMerge != CellMerge.None)
            {
                m_writer.WriteStartElement("hMerge", DocxConstants.W_namespace);

                switch (cellFormat.HorizontalMerge)
                {
                    case CellMerge.Start:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "restart");
                        break;
                    case CellMerge.Continue:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "continue");
                        break;
                }

                m_writer.WriteEndElement();
            }
            if (cellFormat.VerticalMerge != CellMerge.None)
            {
                m_writer.WriteStartElement("vMerge", DocxConstants.W_namespace);

                switch (cellFormat.VerticalMerge)
                {
                    case CellMerge.Start:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "restart");
                        break;
                    case CellMerge.Continue:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "continue");
                        break;
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the grid span element of cell.
        /// </summary>
        /// <param name="cell"></param>
        private void SerializeGridSpan(WTableCell cell)
        {
            int gridSpan = cell.GridSpan;
            if (gridSpan > 1)
            {
                m_writer.WriteStartElement("gridSpan", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, gridSpan.ToString());
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the cell width
        /// </summary>
        /// <param name="cell"></param>
        private void SerializeCellWidth(WTableCell cell)
        {
            CellFormat cf = m_isAlternativeCellFormat ? cell.TrackCellFormat : cell.CellFormat;
            if (cf.PreferredWidth.WidthType == FtsWidth.None)
                return;
            m_writer.WriteStartElement("tcW", DocxConstants.W_namespace);
            if (cf.PreferredWidth.WidthType == FtsWidth.Auto)
            {
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "auto");
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "0");
            }
            else if (cf.PreferredWidth.WidthType == FtsWidth.Percentage)
            {
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "pct");
                int cWidth = (int)Math.Round(cf.PreferredWidth.Width * DLSConstants.PercentageFactor);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, cWidth.ToString());
            }
            else if (cf.PreferredWidth.WidthType == FtsWidth.Point)
            {
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                int cWidth = (int)Math.Round(cf.PreferredWidth.Width * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, cWidth.ToString());
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the row format
        /// </summary>
        /// <param name="row">The table row</param>
        private void SerializeRowFormat(WTableRow row)
        {
            if (row.OwnerTable.Owner != null 
                && !m_isAlternativeRowFormat
                && (row.OwnerTable.Owner.EntityType == EntityType.Table 
                || row.OwnerTable.Owner.EntityType == EntityType.TableRow) || row.HasTblPrEx)
            {
                m_writer.WriteStartElement("tblPrEx", DocxConstants.W_namespace);
                SerializeTableFormat(row.RowFormat, null);
                m_writer.WriteEndElement();
            }
            RowFormat rowFormat = m_isAlternativeRowFormat ? row.TrackRowFormat : row.RowFormat;
            List<Stream> tempDocxProps = new List<Stream>();
            for (int i = 0, cnt = rowFormat.XmlProps.Count; i < cnt; i++)
                tempDocxProps.Add(rowFormat.XmlProps[i]);
            m_writer.WriteStartElement("trPr", DocxConstants.W_namespace);

            //Serialize "cnfStyle" element 
            SerializeCnfStyleElement(row);
            //w:divId -    Associated HTML div ID
            SerializeDocxProps(tempDocxProps, "divId");
            //Serialize "gridBefore" element
            short gridBefore = rowFormat.GridBefore;
            if (gridBefore > 0)
            {
                m_writer.WriteStartElement("gridBefore", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, gridBefore.ToString());
                m_writer.WriteEndElement();
            }
            //Serialize "gridAfter" element
            short gridAfter = rowFormat.GridAfter;
            if (gridAfter > 0)
            {
                m_writer.WriteStartElement("gridAfter", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, gridAfter.ToString());
                m_writer.WriteEndElement();
            }
            //Serialize "wBefore" element 
            if (gridBefore > 0)
            {
                m_writer.WriteStartElement("wBefore", DocxConstants.W_namespace);
                switch (rowFormat.GridBeforeWidth.WidthType)
                {
                    case FtsWidth.Percentage:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "pct");
                        m_writer.WriteAttributeString("w", DocxConstants.W_namespace, (rowFormat.GridBeforeWidth.Width * DLSConstants.PercentageFactor).ToString(CultureInfo.InvariantCulture));
                        break;
                    case FtsWidth.Point:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                        m_writer.WriteAttributeString("w", DocxConstants.W_namespace, (rowFormat.GridBeforeWidth.Width * DLSConstants.TwipsInOnePoint).ToString(CultureInfo.InvariantCulture));
                        break;
                }
                m_writer.WriteEndElement();
            }
            //Serialize "wAfter" element
            if (gridAfter > 0)
            {
                m_writer.WriteStartElement("wAfter", DocxConstants.W_namespace);
                switch (rowFormat.GridAfterWidth.WidthType)
                {
                    case FtsWidth.Percentage:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "pct");
                        m_writer.WriteAttributeString("w", DocxConstants.W_namespace, (rowFormat.GridAfterWidth.Width * DLSConstants.PercentageFactor).ToString(CultureInfo.InvariantCulture));
                        break;
                    case FtsWidth.Point:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                        m_writer.WriteAttributeString("w", DocxConstants.W_namespace, (rowFormat.GridAfterWidth.Width * DLSConstants.TwipsInOnePoint).ToString(CultureInfo.InvariantCulture));
                        break;
                }
                m_writer.WriteEndElement();
            }
            //Serialize "Hidden" element 
            if (rowFormat.HasValue(RowFormat.HiddenKey))
            {
                m_writer.WriteStartElement("hidden", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //Serialize "cantSplit" element 
            if (rowFormat.HasValue(RowFormat.IsBreakAcrossPagesKey) && !rowFormat.IsBreakAcrossPages)
            {
                m_writer.WriteStartElement("cantSplit", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //Serialize "trHeight" element 
            if (row.Height != 0)
            {
                m_writer.WriteStartElement("trHeight", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, ToString((float)Math.Abs(row.Height) * DocxConstants.TwentiethOfPoint));
                switch (row.HeightType)
                {
                    case TableRowHeightType.AtLeast:
                        m_writer.WriteAttributeString("hRule", DocxConstants.W_namespace, "atLeast");
                        break;
                    case TableRowHeightType.Exactly:
                        m_writer.WriteAttributeString("hRule", DocxConstants.W_namespace, "exact");
                        break;
                }
                m_writer.WriteEndElement();
            }
            //Serialize "tblHeader" element 
            if (row.IsHeader || (rowFormat.RowDescriptor != null && rowFormat.RowDescriptor.IsTableHeader))
            {
                m_writer.WriteStartElement("tblHeader", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //Serialize "tblCellSpacing" element 
            SerializeCellSpacing(rowFormat);
            //Serialize "jc" element 
            SerializeTableAlignment(rowFormat);
            //w:hidden -    Hidden Table Row Marker
            SerializeDocxProps(tempDocxProps, "hidden");
            //w:ins -   Inserted Table Row
            if (row.IsInsertRevision)
            {
                SerializeTrackChangeProps("ins");
                m_writer.WriteEndElement();
            }
            // w:del -  Deleted Table Row
            if (row.IsDeleteRevision)
            {
                SerializeTrackChangeProps("del");
                m_writer.WriteEndElement();
            }
            // w:trPrChange - Revision Information for Table Row Properties
            if (row.m_trackRowFormat != null && !m_isAlternativeRowFormat)
            {
                m_isAlternativeRowFormat = true;
                SerializeTrackChangeProps("trPrChange");
                SerializeRowFormat(row);
                m_writer.WriteEndElement();
                m_isAlternativeRowFormat = false;
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the conditional formatting style element for table row
        /// </summary>
        /// <param name="row">The row</param>
        private void SerializeCnfStyleElement(WTableRow row)
        {
            IStyle style = m_document.Styles.FindByName(row.OwnerTable.StyleName, StyleType.TableStyle);
            if (style != null && (style as WTableStyle).ConditionalFormattingStyles.Count > 0)
            {
                int rowIndex = row.GetRowIndex();
                WTable table = row.OwnerTable;
                string fRow = "0", lRow = "0", fCol = "0", lCol = "0", oddVBand = "0", evenVBand = "0", oddHBand = "0", evenHBand = "0", fRowFCol = "0", fRowLCol = "0", lRowFCol = "0", lRowLCol = "0";
                foreach (ConditionalFormattingCode code in (style as WTableStyle).ConditionalFormattingStyles.Keys)
                {
                    switch (code)
                    {
                        case ConditionalFormattingCode.FirstRow:
                            if (rowIndex == 0 && table.ApplyStyleForHeaderRow)
                                fRow = "1";
                            break;
                        case ConditionalFormattingCode.LastRow:
                            if (rowIndex != 0 && rowIndex == table.Rows.Count - 1 && table.ApplyStyleForLastRow)
                                lRow = "1";
                            break;
                        case ConditionalFormattingCode.OddRowBanding:
                            if (rowIndex != table.Rows.Count - 1 && rowIndex % 2 == 1 && table.ApplyStyleForBandedRows)
                                oddHBand = "1";
                            break;
                        case ConditionalFormattingCode.EvenRowBanding:
                            if (rowIndex != 0 && rowIndex != table.Rows.Count - 1 && rowIndex % 2 == 0 && table.ApplyStyleForBandedRows)
                                evenHBand = "1";
                            break;
                    }
                }
                string value = fRow + lRow + fCol + lCol + oddVBand + evenVBand + oddHBand + evenHBand + fRowLCol + fRowFCol + lRowLCol + lRowFCol;
                if (value != "000000000000")
                {
                    m_writer.WriteStartElement("cnfStyle", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, value);
                    m_writer.WriteAttributeString("w", "firstRow", DocxConstants.W_namespace, fRow);
                    m_writer.WriteAttributeString("w", "lastRow", DocxConstants.W_namespace, lRow);
                    m_writer.WriteAttributeString("w", "firstColumn", DocxConstants.W_namespace, fCol);
                    m_writer.WriteAttributeString("w", "lastColumn", DocxConstants.W_namespace, lCol);
                    m_writer.WriteAttributeString("w", "oddVBand", DocxConstants.W_namespace, oddVBand);
                    m_writer.WriteAttributeString("w", "evenVBand", DocxConstants.W_namespace, evenVBand);
                    m_writer.WriteAttributeString("w", "oddHBand", DocxConstants.W_namespace, oddHBand);
                    m_writer.WriteAttributeString("w", "evenHBand", DocxConstants.W_namespace, evenHBand);
                    m_writer.WriteAttributeString("w", "firstRowFirstColumn", DocxConstants.W_namespace, fRowFCol);
                    m_writer.WriteAttributeString("w", "firstRowLastColumn", DocxConstants.W_namespace, fRowLCol);
                    m_writer.WriteAttributeString("w", "lastRowFirstColumn", DocxConstants.W_namespace, lRowFCol);
                    m_writer.WriteAttributeString("w", "lastRowLastColumn", DocxConstants.W_namespace, lRowLCol);
                    m_writer.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// Serialize the conditional formatting style element for table cell
        /// </summary>
        /// <param name="cell">The cell</param>
        private void SerializeCnfStyleElement(WTableCell cell)
        {
            IStyle style = m_document.Styles.FindByName(cell.OwnerRow.OwnerTable.StyleName, StyleType.TableStyle);
            if (style != null && (style as WTableStyle).ConditionalFormattingStyles.Count > 0)
            {
                int cellIndex = cell.GetCellIndex();
                int rowIndex = cell.OwnerRow.GetRowIndex();
                WTable table = cell.OwnerRow.OwnerTable;
                string fRow = "0", lRow = "0", fCol = "0", lCol = "0", oddVBand = "0", evenVBand = "0", oddHBand = "0", evenHBand = "0", fRowFCol = "0", fRowLCol = "0", lRowFCol = "0", lRowLCol = "0";
                foreach (ConditionalFormattingCode code in (style as WTableStyle).ConditionalFormattingStyles.Keys)
                {
                    switch (code)
                    {
                        case ConditionalFormattingCode.FirstColumn:
                            if (cellIndex == 0 && table.ApplyStyleForFirstColumn)
                                fCol = "1";
                            break;
                        case ConditionalFormattingCode.LastColumn:
                            if (cellIndex != 0 && cellIndex == cell.OwnerRow.Cells.Count - 1 && table.ApplyStyleForLastColumn)
                                lCol = "1";
                            break;
                        case ConditionalFormattingCode.OddColumnBanding:
                            if (cellIndex != cell.OwnerRow.Cells.Count - 1 && cellIndex % 2 == 1 && table.ApplyStyleForBandedColumns)
                                oddVBand = "1";
                            break;
                        case ConditionalFormattingCode.EvenColumnBanding:
                            if (cellIndex != 0 && cellIndex != cell.OwnerRow.Cells.Count - 1 && cellIndex % 2 == 0 && table.ApplyStyleForBandedColumns)
                                evenVBand = "1";
                            break;
                        case ConditionalFormattingCode.FirstRowLastCell:
                            if (rowIndex == 0 && cellIndex != 0 && cellIndex == cell.OwnerRow.Cells.Count - 1 && table.ApplyStyleForHeaderRow && table.ApplyStyleForLastColumn)
                                fRowLCol = "1";
                            break;
                        case ConditionalFormattingCode.FirstRowFirstCell:
                            if (rowIndex == 0 && cellIndex == 0 && table.ApplyStyleForHeaderRow && table.ApplyStyleForFirstColumn)
                                fRowFCol = "1";
                            break;
                        case ConditionalFormattingCode.LastRowLastCell:
                            if (rowIndex != 0 && rowIndex == table.Rows.Count - 1 && cellIndex != 0 && cellIndex == cell.OwnerRow.Cells.Count - 1 && table.ApplyStyleForLastRow && table.ApplyStyleForLastColumn)
                                lRowLCol = "1";
                            break;
                        case ConditionalFormattingCode.LastRowFirstCell:
                            if (rowIndex != 0 && rowIndex == table.Rows.Count - 1 && cellIndex == 0 && table.ApplyStyleForLastRow && table.ApplyStyleForFirstColumn)
                                lRowFCol = "1";
                            break;
                    }
                }
                string value = fRow + lRow + fCol + lCol + oddVBand + evenVBand + oddHBand + evenHBand + fRowLCol + fRowFCol + lRowLCol + lRowFCol;
                if (value != "000000000000")
                {
                    m_writer.WriteStartElement("cnfStyle", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, value);
                    m_writer.WriteAttributeString("w", "firstRow", DocxConstants.W_namespace, fRow);
                    m_writer.WriteAttributeString("w", "lastRow", DocxConstants.W_namespace, lRow);
                    m_writer.WriteAttributeString("w", "firstColumn", DocxConstants.W_namespace, fCol);
                    m_writer.WriteAttributeString("w", "lastColumn", DocxConstants.W_namespace, lCol);
                    m_writer.WriteAttributeString("w", "oddVBand", DocxConstants.W_namespace, oddVBand);
                    m_writer.WriteAttributeString("w", "evenVBand", DocxConstants.W_namespace, evenVBand);
                    m_writer.WriteAttributeString("w", "oddHBand", DocxConstants.W_namespace, oddHBand);
                    m_writer.WriteAttributeString("w", "evenHBand", DocxConstants.W_namespace, evenHBand);
                    m_writer.WriteAttributeString("w", "firstRowFirstColumn", DocxConstants.W_namespace, fRowFCol);
                    m_writer.WriteAttributeString("w", "firstRowLastColumn", DocxConstants.W_namespace, fRowLCol);
                    m_writer.WriteAttributeString("w", "lastRowFirstColumn", DocxConstants.W_namespace, lRowFCol);
                    m_writer.WriteAttributeString("w", "lastRowLastColumn", DocxConstants.W_namespace, lRowLCol);
                    m_writer.WriteEndElement();
                }
                else
                {
                    IsParagraphContainsCnfStyle = true;
                }
            }
        }
        /// <summary>
        /// Serialize the conditional formatting style element for paragraph in table
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        private void SerializeCnfStyleElement(WParagraph paragraph)
        {
            WTableCell cell = paragraph.OwnerTextBody as WTableCell;
            IStyle style = m_document.Styles.FindByName(cell.OwnerRow.OwnerTable.StyleName, StyleType.TableStyle);
            if (style != null && (style as WTableStyle).ConditionalFormattingStyles.Count > 0)
            {
                int cellIndex = cell.GetCellIndex();
                int rowIndex = cell.OwnerRow.GetRowIndex();
                WTable table = cell.OwnerRow.OwnerTable;
                if (IsParagraphHasCnfStyle(style, rowIndex, table))
                {
                    string fRow = "0", lRow = "0", fCol = "0", lCol = "0", oddVBand = "0", evenVBand = "0", oddHBand = "0", evenHBand = "0", fRowFCol = "0", fRowLCol = "0", lRowFCol = "0", lRowLCol = "0";
                    foreach (ConditionalFormattingCode code in (style as WTableStyle).ConditionalFormattingStyles.Keys)
                    {
                        switch (code)
                        {
                            case ConditionalFormattingCode.FirstRow:
                                if (rowIndex == 0 && table.ApplyStyleForHeaderRow)
                                    fRow = "1";
                                break;
                            case ConditionalFormattingCode.LastRow:
                                if (rowIndex != 0 && rowIndex == table.Rows.Count - 1 && table.ApplyStyleForLastRow)
                                    lRow = "1";
                                break;
                            case ConditionalFormattingCode.OddRowBanding:
                                if (rowIndex != table.Rows.Count - 1 && rowIndex % 2 == 1 && table.ApplyStyleForBandedRows)
                                    oddHBand = "1";
                                break;
                            case ConditionalFormattingCode.EvenRowBanding:
                                if (rowIndex != 0 && rowIndex != table.Rows.Count - 1 && rowIndex % 2 == 0 && table.ApplyStyleForBandedRows)
                                    evenHBand = "1";
                                break;
                        }
                    }
                    string value = fRow + lRow + fCol + lCol + oddVBand + evenVBand + oddHBand + evenHBand + fRowLCol + fRowFCol + lRowLCol + lRowFCol;

                    m_writer.WriteStartElement("cnfStyle", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, value);
                    m_writer.WriteAttributeString("w", "firstRow", DocxConstants.W_namespace, fRow);
                    m_writer.WriteAttributeString("w", "lastRow", DocxConstants.W_namespace, lRow);
                    m_writer.WriteAttributeString("w", "firstColumn", DocxConstants.W_namespace, fCol);
                    m_writer.WriteAttributeString("w", "lastColumn", DocxConstants.W_namespace, lCol);
                    m_writer.WriteAttributeString("w", "oddVBand", DocxConstants.W_namespace, oddVBand);
                    m_writer.WriteAttributeString("w", "evenVBand", DocxConstants.W_namespace, evenVBand);
                    m_writer.WriteAttributeString("w", "oddHBand", DocxConstants.W_namespace, oddHBand);
                    m_writer.WriteAttributeString("w", "evenHBand", DocxConstants.W_namespace, evenHBand);
                    m_writer.WriteAttributeString("w", "firstRowFirstColumn", DocxConstants.W_namespace, fRowFCol);
                    m_writer.WriteAttributeString("w", "firstRowLastColumn", DocxConstants.W_namespace, fRowLCol);
                    m_writer.WriteAttributeString("w", "lastRowFirstColumn", DocxConstants.W_namespace, lRowFCol);
                    m_writer.WriteAttributeString("w", "lastRowLastColumn", DocxConstants.W_namespace, lRowLCol);
                    m_writer.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// Checks whether the paragraph has cnf style or not.
        /// </summary>
        /// <param name="style">style</param>
        /// <param name="rowIndex">row Index</param>
        /// <param name="table">table</param>
        /// <returns></returns>
        private bool IsParagraphHasCnfStyle(IStyle style, int rowIndex, WTable table)
        {
            return ((style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.FirstColumn) && table.ApplyStyleForFirstColumn
                    || (style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.LastColumn) && table.ApplyStyleForLastColumn
                    || (style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.OddColumnBanding) && table.ApplyStyleForBandedColumns
                    || (style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.EvenColumnBanding) && table.ApplyStyleForBandedColumns
                    || rowIndex == 0 && (style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.FirstRowFirstCell) && table.ApplyStyleForHeaderRow && table.ApplyStyleForFirstColumn
                    || rowIndex != 0 && rowIndex == table.Rows.Count - 1 && (style as WTableStyle).ConditionalFormattingStyles.ContainsKey(ConditionalFormattingCode.LastRowFirstCell) && table.ApplyStyleForLastRow && table.ApplyStyleForFirstColumn);
        }

        #region TableFormat/RowFormat
        /// <summary>
        /// Serialize the row formattings.
        /// Table parameter is passed for serializing table format and null for serializing row format.
        /// </summary>
        /// <param name="format">the row format</param>
        /// <param name="table">The table</param>
        private void SerializeTableFormat(RowFormat format, WTable table)
        {
            if (table != null)
            {
                List<Stream> tempDocxProps = new List<Stream>();
                for (int i = 0, cnt = table.DocxTableFormat.NodeArray.Count; i < cnt; i++)
                    tempDocxProps.Add(table.DocxTableFormat.NodeArray[i]);
                m_writer.WriteStartElement("tblPr", DocxConstants.W_namespace);
                SerializeTableStlye(format);
                SerializeDocxProps(tempDocxProps, "tblOverlap");
                if (format.WrapTextAround)
                    SerializeTablePositioning(format.Positioning);
                if (format.Bidi)
                {
                    m_writer.WriteStartElement("bidiVisual", DocxConstants.W_namespace);
                    m_writer.WriteEndElement();
                }
                SerializeDocxProps(tempDocxProps, "tblStyleRowBandSize");
                SerializeDocxProps(tempDocxProps, "tblStyleColBandSize");
                SerializeTableWidth(table);
                SerializeTableAlignment(format);
                SerializeCellSpacing(format);
                SerializeTableIndentation(format);
                SerializeTableBorders(format);
                SerializeTableShading(format);
                SerializeTblLayout(format);
                SerializeTableCellMargin(format);
                SerializeTableLook(table);
                if (m_document.Settings.CompatibilityMode != CompatibilityMode.Word2003)
                {
                    SerializeTableTitle(table);
                    SerializeTableDescription(table);
                }
            }
            else
            {
                SerializeCellSpacing(format);
                SerializeTableIndentation(format);
                SerializeTableBorders(format);
                SerializeTableShading(format);
                SerializeTblLayout(format);
                SerializeTableCellMargin(format);
            }
            if (format.OwnerBase != null && format.OwnerBase is WTable
              && (format.OwnerBase as WTable).m_trackTblFormat != null && !m_isAlternativeTableFormat)
            {
                m_isAlternativeTableFormat = true;
                SerializeTrackChangeProps("tblPrChange");
                SerializeTableFormat((format.OwnerBase as WTable).TrackTblFormat.Format, null);
                m_writer.WriteEndElement();
                m_isAlternativeTableFormat = false;
            }

            if (format.OwnerRow != null && format.OwnerRow.m_trackRowFormat != null)
            {
                SerializeTrackChangeProps("tblPrExChange");
                SerializeTableFormat(format.OwnerRow.TrackRowFormat, null);
                m_writer.WriteEndElement();
            }

            SerializeTblTrackChanges(format);
            if (table != null)
                m_writer.WriteEndElement();//end of tblPr
        }
        /// <summary>
        /// Serializes the table title. 
        /// Word 2010 specific property.
        /// </summary>
        /// <param name="table">The table.</param>
        private void SerializeTableTitle(WTable table)
        {
            m_writer.WriteStartElement("tblCaption", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, table.Title);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the table description. 
        /// Word 2010 specific property.
        /// </summary>
        /// <param name="table">The table.</param>
        private void SerializeTableDescription(WTable table)
        {
            m_writer.WriteStartElement("tblDescription", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, table.Description);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table track changes
        /// </summary>
        /// <param name="format">The table format</param>
        private void SerializeTblTrackChanges(RowFormat format)
        {
            if (format.OwnerRow != null && format.OwnerRow.OwnerTable is WTable)
            {
                WTable table = format.OwnerRow.OwnerTable as WTable;
                if (table.IsDeleteRevision)
                {
                    SerializeTrackChangeProps("del");
                    m_writer.WriteEndElement();
                }
                else if (table.IsInsertRevision)
                {
                    SerializeTrackChangeProps("ins");
                    m_writer.WriteEndElement();
                }
            }

            if (format.OwnerBase != null && format.OwnerBase is WTableRow)
            {
                WTableRow row = format.OwnerBase as WTableRow;
                if (row.IsDeleteRevision)
                {
                    SerializeTrackChangeProps("del");
                    m_writer.WriteEndElement();
                }
                else if (row.IsInsertRevision)
                {
                    SerializeTrackChangeProps("ins");
                    m_writer.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// Serialize the table cell margins (paddings)
        /// </summary>
        /// <param name="format">The row formattings</param>
        private void SerializeTableCellMargin(RowFormat format)
        {
            if (!format.Paddings.IsDefault || format.HasValue(RowFormat.PaddingsKey))
            {
                m_writer.WriteStartElement("tblCellMar", DocxConstants.W_namespace);
                SerializePaddings(format.Paddings);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the paddings
        /// </summary>
        /// <param name="paddings"></param>
        private void SerializePaddings(Paddings paddings)
        {
            if (paddings.Top >= 0 && paddings.HasKey(2))
            {
                m_writer.WriteStartElement("top", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(paddings.Top * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
            if (paddings.Left >= 0 && paddings.HasKey(1))
            {
                m_writer.WriteStartElement("left", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(paddings.Left * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
            if (paddings.Bottom >= 0 && paddings.HasKey(3))
            {
                m_writer.WriteStartElement("bottom", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(paddings.Bottom * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
            if (paddings.Right >= 0 && paddings.HasKey(4))
            {
                m_writer.WriteStartElement("right", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(paddings.Right * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the table layout element
        /// </summary>
        /// <param name="format">The row format</param>
        private void SerializeTblLayout(RowFormat format)
        {
            if (!format.IsAutoResized)
            {
                m_writer.WriteStartElement("tblLayout", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "fixed");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the table shading
        /// </summary>
        /// <param name="format">The row format</param>
        private void SerializeTableShading(RowFormat format)
        {
            if (format.HasValue(RowFormat.ShadingColorKey) || format.HasValue(RowFormat.TextureStyleKey))
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                if (format.BackColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }
                else
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(format.BackColor));
                }

                string val = GetTextureStyle(format.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the table borders
        /// </summary>
        /// <param name="format"></param>
        private void SerializeTableBorders(RowFormat format)
        {
            m_writer.WriteStartElement("tblBorders", DocxConstants.W_namespace);
            SerializeBorders(format.Borders, 8);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table indentation.
        /// </summary>
        /// <param name="format"></param>
        private void SerializeTableIndentation(RowFormat format)
        {
            if (format.HasValue(RowFormat.LeftIndentKey))
            {
                m_writer.WriteStartElement("tblInd", DocxConstants.W_namespace);
                int tableIndent = (int)Math.Round(format.LeftIndent * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, tableIndent.ToString());
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the cell spacing.
        /// </summary>
        /// <param name="format">The row format</param>
        private void SerializeCellSpacing(RowFormat format)
        {
            if (format.CellSpacing >= 0)
            {
                m_writer.WriteStartElement("tblCellSpacing", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(format.CellSpacing * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the table width
        /// </summary>
        /// <param name="table"></param>
        private void SerializeTableWidth(WTable table)
        {
            if (table.PreferredTableWidth.WidthType == FtsWidth.None
                || ((int)table.PreferredTableWidth.WidthType >= 2
                && table.PreferredTableWidth.Width == 0))
                return;
            m_writer.WriteStartElement("tblW", DocxConstants.W_namespace);
            if (table.PreferredTableWidth.WidthType == FtsWidth.Auto)
            {
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "0");
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "auto");
            }
            else if (table.PreferredTableWidth.WidthType == FtsWidth.Percentage)
            {
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(table.PreferredTableWidth.Width * DLSConstants.PercentageFactor));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "pct");
            }
            else if (table.PreferredTableWidth.WidthType == FtsWidth.Point)
            {
                int tableWidth = (int)Math.Round(table.PreferredTableWidth.Width * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, tableWidth.ToString());
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table alignment
        /// </summary>
        /// <param name="format"></param>
        private void SerializeTableAlignment(RowFormat format)
        {
            m_writer.WriteStartElement("jc", DocxConstants.W_namespace);

            switch (format.HorizontalAlignment)
            {
                case RowAlignment.Right:
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "right");
                    break;
                case RowAlignment.Center:
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "center");
                    break;
                default:
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "left");
                    break;
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table absolute positioning formattings.
        /// </summary>
        /// <param name="positioning"></param>
        private void SerializeTablePositioning(RowFormat.TablePositioning positioning)
        {
            m_writer.WriteStartElement("tblpPr", DocxConstants.W_namespace);
            string value = null;

            if (positioning.HorizPosition != 0)
            {
                value = ToString(positioning.HorizPosition * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("tblpX", DocxConstants.W_namespace, value);
            }
            if (positioning.VertPosition != 0)
            {
                value = ToString(positioning.VertPosition * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("tblpY", DocxConstants.W_namespace, value);
            }

            if (positioning.DistanceFromTop != 0)
            {
                value = ToString(positioning.DistanceFromTop * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("topFromText", DocxConstants.W_namespace, value);
            }
            if (positioning.DistanceFromBottom != 0)
            {
                value = ToString(positioning.DistanceFromBottom * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("bottomFromText", DocxConstants.W_namespace, value);
            }

            value = ToString(positioning.DistanceFromLeft * DLSConstants.TwipsInOnePoint);
            m_writer.WriteAttributeString("leftFromText", DocxConstants.W_namespace, value);

            value = ToString(positioning.DistanceFromRight * DLSConstants.TwipsInOnePoint);
            m_writer.WriteAttributeString("rightFromText", DocxConstants.W_namespace, value);

            SerializeTableVertRelation(positioning.VertRelationTo);
            if (positioning.HorizRelationTo != HorizontalRelation.Column)
                SerializeTableHorizRelation(positioning.HorizRelationTo);

            if (positioning.HorizPositionAbs != HorizontalPosition.Left)
                SerializeTableHorizPosition(positioning.HorizPositionAbs);
            if (positioning.VertPositionAbs != VerticalPosition.None)
                SerializeTableVertPosition(positioning.VertPositionAbs);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table's horizontal positionings.
        /// </summary>
        /// <param name="position"></param>
        private void SerializeTableHorizPosition(HorizontalPosition position)
        {
            string value = null;
            switch (position)
            {
                case HorizontalPosition.Center:
                    value = "center";
                    break;
                case HorizontalPosition.Right:
                    value = "right";
                    break;
                case HorizontalPosition.Inside:
                    value = "inside";
                    break;
                case HorizontalPosition.Outside:
                    value = "outside";
                    break;
            }

            m_writer.WriteAttributeString("tblpXSpec", DocxConstants.W_namespace, value);
        }
        /// <summary>
        /// Serialize the table's vertical position.
        /// </summary>
        /// <param name="position">The position.</param>
        private void SerializeTableVertPosition(VerticalPosition position)
        {
            string value = null;
            switch (position)
            {
                case VerticalPosition.Top:
                    value = "top";
                    break;
                case VerticalPosition.Center:
                    value = "center";
                    break;
                case VerticalPosition.Bottom:
                    value = "bottom";
                    break;
                case VerticalPosition.Inside:
                    value = "inside";
                    break;
                case VerticalPosition.Outside:
                    value = "outside";
                    break;
            }

            m_writer.WriteAttributeString("tblpYSpec", DocxConstants.W_namespace, value);
        }
        /// <summary>
        /// Serialize the table's vertical relation.
        /// </summary>
        /// <param name="relation">The relation.</param>
        private void SerializeTableVertRelation(VerticalRelation relation)
        {
            string value = null;
            switch (relation)
            {
                case VerticalRelation.Paragraph:
                    value = "text";
                    break;
                case VerticalRelation.Page:
                    value = "page";
                    break;
                case VerticalRelation.Margin:
                    value = "margin";
                    break;
            }

            m_writer.WriteAttributeString("vertAnchor", DocxConstants.W_namespace, value);
        }
        /// <summary>
        /// Serialize the table's horizontal relation.
        /// </summary>
        /// <param name="relation">The relation.</param>
        private void SerializeTableHorizRelation(HorizontalRelation relation)
        {
            string value = null;
            switch (relation)
            {
                case HorizontalRelation.Column:
                    value = "text";
                    break;
                case HorizontalRelation.Margin:
                    value = "margin";
                    break;
                case HorizontalRelation.Page:
                    value = "page";
                    break;
            }

            m_writer.WriteAttributeString("horzAnchor", DocxConstants.W_namespace, value);
        }
        /// <summary>
        /// Serialize the table style element
        /// </summary>
        /// <param name="format"></param>
        private void SerializeTableStlye(RowFormat format)
        {
            WTable ownerTable = null;
            if (format.OwnerBase != null && format.OwnerBase is WTable)
            {
                ownerTable = format.OwnerBase as WTable;
                // Check whether table inherits style properties.
                if (ownerTable.DocxTableFormat.HasFormat && !ownerTable.DocxTableFormat.Format.IsDefault &&
                  string.IsNullOrEmpty(ownerTable.DocxTableFormat.StyleName))
                    return;
            }

            string styleName = "TableGrid";
            if (ownerTable != null)
            {
                if (!string.IsNullOrEmpty(ownerTable.DocxTableFormat.StyleName))
                {
                    styleName = (format.OwnerBase as WTable).DocxTableFormat.StyleName;
                }
                if (ownerTable.StyleName != null)
                    styleName = GetStyleNameId(ownerTable.StyleName);
            }
            else if ((format.OwnerBase is WTableRow) && (format.OwnerBase as WTableRow).OwnerTable != null)
            {
                string tblStyleName = (format.OwnerBase as WTableRow).OwnerTable.DocxTableFormat.StyleName;
                if ((format.OwnerBase as WTableRow).OwnerTable.StyleName != null)
                    tblStyleName = GetStyleNameId((format.OwnerBase as WTableRow).OwnerTable.StyleName);
                if (tblStyleName != null)
                    styleName = tblStyleName;
            }
            m_writer.WriteStartElement("tblStyle", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, styleName);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Gets style name Id.
        /// </summary>
        /// <param name="styleName"></param>
        private string GetStyleNameId(string styleName)
        {
            string styleNameId = "";
            if (!m_document.StyleNameIds.ContainsValue(styleName))
                return styleName.Replace(" ", string.Empty);
            foreach (KeyValuePair<string, string> pair in m_document.StyleNameIds)
            {
                if (pair.Value == styleName)
                {
                    styleNameId = pair.Key;
                    break;
                }
            }
            return styleNameId;
        }
        /// <summary>
        /// Serialize the table look element
        /// </summary>
        /// <param name="table"></param>
        private void SerializeTableLook(WTable table)
        {
            string fRow = "0", lRow = "0", fCol = "0", lCol = "0", noHBand = "0", noVBand = "0";
            if (table.ApplyStyleForHeaderRow)
                fRow = "1";
            if (table.ApplyStyleForLastRow)
                lRow = "1";
            if (table.ApplyStyleForFirstColumn)
                fCol = "1";
            if (table.ApplyStyleForLastColumn)
                lCol = "1";
            if (!table.ApplyStyleForBandedRows)
                noHBand = "1";
            if (!table.ApplyStyleForBandedColumns)
                noVBand = "1";

            string value = noVBand + noHBand + lCol + fCol + lRow + fRow + "00000";
            //Calculates HexBinary value of the table look.
            value = string.Format("{0:X}", Convert.ToInt64(value, 2)).PadLeft(4, '0');

            m_writer.WriteStartElement("tblLook", DocxConstants.W_namespace);

            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, value);

            m_writer.WriteAttributeString("w", "firstRow", DocxConstants.W_namespace, fRow);

            m_writer.WriteAttributeString("w", "lastRow", DocxConstants.W_namespace, lRow);

            m_writer.WriteAttributeString("w", "firstColumn", DocxConstants.W_namespace, fCol);

            m_writer.WriteAttributeString("w", "lastColumn", DocxConstants.W_namespace, lCol);

            m_writer.WriteAttributeString("w", "noHBand", DocxConstants.W_namespace, noHBand);

            m_writer.WriteAttributeString("w", "noVBand", DocxConstants.W_namespace, noVBand);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table grid
        /// </summary>
        /// <param name="table">The table</param>
        private void SerializeTableGrid(WTable table)
        {
            m_writer.WriteStartElement("tblGrid", DocxConstants.W_namespace);

            if (table.TableGrid.Count != 0)
            {
                SerializeGridColumns(table.TableGrid);
            }

            if (table.m_trackTableGrid != null && table.TrackTableGrid.Count != 0)
            {
                m_writer.WriteStartElement("tblGridChange", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("id", DocxConstants.W_namespace, GetNextTChangeId());
                m_writer.WriteStartElement("tblGrid", DocxConstants.W_namespace);
                SerializeGridColumns(table.TrackTableGrid);
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table grid columns.
        /// </summary>
        /// <param name="grid">The grid values.</param>
        private void SerializeGridColumns(List<float> grid)
        {
            float prevOffset = grid[0];
            float colOffset = 0;
            for (int i = 1, count = grid.Count; i < count; i++)
            {
                colOffset = grid[i];
                SerializeGridColumn(colOffset - prevOffset);
                prevOffset = colOffset;
            }
        }
        /// <summary>
        /// Serialize grid column.
        /// </summary>
        /// <param name="colWidth">The column width</param>
        private void SerializeGridColumn(float colWidth)
        {
            int width = (int)Math.Round(colWidth);
            m_writer.WriteStartElement("gridCol", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, width.ToString());
            m_writer.WriteEndElement();
        }
        #endregion TableFormat/RowFormat

        #endregion Table

        #region Paragraph
        /// <summary>
        /// Serialize the paragraph
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        /// <param name="isLastSection"></param>
        private void SerializeParagraph(WParagraph paragraph, bool isLastSection)
        {
            if (paragraph == null)
                throw new ArgumentException("Paragraph should not be null");

            if (paragraph.Text == string.Empty && paragraph.RemoveEmpty)
                return;

            if (paragraph.ParagraphFormat.PageBreakAfter && !IsPageBreakNeedToBeSkipped(paragraph as Entity))
                paragraph.InsertBreak(BreakType.PageBreak);
            if (paragraph.ParagraphFormat.ColumnBreakAfter && !IsPageBreakNeedToBeSkipped(paragraph as Entity))
                paragraph.InsertBreak(BreakType.ColumnBreak);
            string textToDisplay = ModifyText(paragraph.Text);
            if (textToDisplay.Contains("\r"))
                paragraph.SplitTextRange();

            m_writer.WriteStartElement("w", "p", DocxConstants.W_namespace);
            m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
            SerializeParagraphFormat(paragraph.ParagraphFormat, paragraph);
            m_writer.WriteEndElement(); //end of pPr

            EnsureWatermark(paragraph);// Serialize watermark if paragraph is the first item of Header document

            SerializeParagraphItems(paragraph.Items);
            m_writer.WriteEndElement();//end of paragraph tag.
        }
        /// <summary>
        /// Serialize watermark if paragraph is the first item of Header document
        /// </summary>
        /// <param name="paragraph"></param>
        private void EnsureWatermark(WParagraph paragraph)
        {
            HeaderFooter header = paragraph.OwnerTextBody is HeaderFooter ? paragraph.OwnerTextBody as HeaderFooter :
                GetBaseEntity(paragraph) as HeaderFooter;
            if (header != null && header.Paragraphs.Count > 0 && header.Paragraphs[0] == paragraph
                && !(paragraph.OwnerTextBody.Owner.Owner != null
                && (paragraph.OwnerTextBody.Owner.Owner is StructureDocumentTagBlock))
                && paragraph.Document.Watermark.Type != WatermarkType.NoWatermark)
            {
                Watermark watermark = paragraph.Document.Watermark;
                if ((header.Type == HeaderFooterType.FirstPageHeader ||
                     header.Type == HeaderFooterType.OddHeader ||
                     header.Type == HeaderFooterType.EvenHeader) &&
                    header.WriteWatermark)
                    SerializeWatermark(header.Document.Watermark);
            }
        }
        #endregion Paragraph

        #region ParagraphItems
        /// <summary>
        /// Serialize the paragraph items
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        private void SerializeParagraphItems(ParagraphItemCollection paraItems)
        {
            for (int i = 0; i < paraItems.Count; i++)
            {
                ParagraphItem item = paraItems[i];
                if ((item is WField) && (item as WField).FieldType == FieldType.FieldNext &&
                        (item as WField).Range.Count != 0 && (item as WField).ConvertedToText)
                    m_skipFieldEnd = (item as WField).FieldEnd;

                if (m_skipFieldEnd == null)
                    SerializeParagraphItem(item);

                if ((item is WFieldMark) && (item as WFieldMark) == m_skipFieldEnd)
                    m_skipFieldEnd = null;
            }
        }
        /// <summary>
        /// Serialize the paragraph item
        /// </summary>
        /// <param name="item">The paragraph item</param>
        private void SerializeParagraphItem(ParagraphItem item)
        {
            if (SkipItem(item))
                return;

            bool HasTrackChanges = SerializeTrackChange(item);

            switch (item.EntityType)
            {
                case EntityType.Break:
                    Break brk = item as Break;
                    if (brk.BreakType == BreakType.PageBreak && IsPageBreakNeedToBeSkipped(item as Entity))
                        break;
                    if (brk.BreakType == BreakType.LineBreak && brk.TextRange.Text == "\r")
                    {
                        m_writer.WriteStartElement("r", DocxConstants.W_namespace);
                        m_writer.WriteStartElement("cr", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        m_writer.WriteEndElement();
                    }
                    else
                        SerializeBreak(brk.BreakType);
                    break;
                case EntityType.BookmarkStart:
                    SerializeBookmarkStart(item as BookmarkStart);
                    break;
                case EntityType.BookmarkEnd:
                    SerializeBookmarkEnd(item as BookmarkEnd);
                    break;
                case EntityType.Picture:
                    SerializePicture(item as WPicture);
                    break;
                case EntityType.TOC:
                    serializeTableOfContents(item as TableOfContent);
                    break;
                case EntityType.FieldMark:
                    if (!IsNestedItem(item))
                    {
                        SerializeFieldMark(item as WFieldMark);
                    }
                    break;
                case EntityType.MergeField:
                    SerializeMergeField(item as WMergeField);
                    break;
                case EntityType.Symbol:
                    SerializeSymbol(item as WSymbol);
                    break;
                case EntityType.OleObject:
                    SerializeOleObject(item as WOleObject);
                    break;
                case EntityType.TextBox:
                    SerializeTextBox(item as WTextBox);
                    break;
                case EntityType.Shape:
                    if (GetBaseEntity(item.OwnerParagraph as Entity) is HeaderFooter)
                    {
                        m_HeaderFooterType = (GetBaseEntity(item.OwnerParagraph as Entity) as HeaderFooter).Type;
                        m_IsAutoshapeTextboxInHeader = true;
                    }
                    WTextBoxCollection textboxes = (item as ShapeObject).AutoShapeTextCollection;
                    SerializeTextboxes(textboxes);
                    m_IsAutoshapeTextboxInHeader = false;
                    break;
                case EntityType.AutoShape:
                    SerializeAutoShape(item as Shape);
                    break;
                case EntityType.Comment:
                    SerializeCommentReference(item as WComment);
                    break;
                case EntityType.CommentMark:
                    SerializeCommentMark(item as WCommentMark);
                    break;
                case EntityType.DropDownFormField:
                    SerializeDropDownFormField(item as WDropDownFormField);
                    break;
                case EntityType.TextFormField:
                    SerializeTextFormField(item as WTextFormField);
                    break;
                case EntityType.CheckBox:
                    SerializeCheckBoxField(item as WCheckBox);
                    break;
                case EntityType.SeqField:
                    SerializeSeqField(item as WSeqField);
                    break;
                case EntityType.Footnote:
                    SerializeFootEndnote(item as WFootnote);
                    break;
                case EntityType.Field:
                    if (!IsNestedItem(item))
                    {
                        FieldStack.Push(item as WField);
                        SerializeField(item as WField);
                    }
                    break;
                case EntityType.XmlParaItem:
                    SerializeXmlParagraphItem(item as XmlParagraphItem);
                    break;
                case EntityType.StructureDocumentTagInline:
                    SerializeStructureDocumentTagInline(item as StructureDocumentTagInline);
                    break;
                case EntityType.AbsoluteTab:
                    SerializeAbsoluteTab(item as WAbsoluteTab);
                    break;
                default:
                    SerializeTextRange(item);
                    break;
            }
            if (HasTrackChanges)
                m_writer.WriteEndElement();
        }

        private void SerializeAutoShape(Shape shape)
        {
            if (shape.AutoShapeType == AutoShapeType.Unknown)
                return;
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(shape.ParaItemCharFormat);
            if (!shape.Is2007Shape)
            {
                m_writer.WriteStartElement("AlternateContent", DocxConstants.VE_namespace);
                m_writer.WriteStartElement("Choice", DocxConstants.VE_namespace);
                m_writer.WriteAttributeString("Requires", "wps");
                m_writer.WriteStartElement("drawing", DocxConstants.W_namespace);
                if (shape.WrapFormat.TextWrappingStyle == TextWrappingStyle.Behind)
                    shape.IsBelowText = true;
                if (shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                    SerializeAbsolutePicture(shape);
                else
                    SerializeInlinePicture(shape);

                m_writer.WriteEndElement();//drawing
                m_writer.WriteEndElement();//choice
                m_writer.WriteStartElement("Fallback", DocxConstants.VE_namespace);
            }
            SerializeFallbackShape(shape);
            if (!shape.Is2007Shape)
            {
                m_writer.WriteEndElement();//Fallback end element
                m_writer.WriteEndElement();//alternate content
            }
            m_writer.WriteEndElement();
        }
        private void SerializeFallbackShape(Shape shape)
        {
            if (shape.AutoShapeType == AutoShapeType.Unknown)
                return;

            XmlReader reader = null;           
            string shapeType = string.Empty;

            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);
            if (shape.AutoShapeType != AutoShapeType.Unknown &&
               shape.AutoShapeType != AutoShapeType.Rectangle &&
               shape.AutoShapeType != AutoShapeType.RoundedRectangle &&
               shape.AutoShapeType != AutoShapeType.Line &&
               shape.AutoShapeType != AutoShapeType.Oval)
                SerializeShapeType(m_writer, shape, ref shapeType, ref reader);

            if (shape.AutoShapeType == AutoShapeType.Rectangle)
                m_writer.WriteStartElement("rect", DocxConstants.V_namespace);
            else if (shape.AutoShapeType == AutoShapeType.RoundedRectangle)
                m_writer.WriteStartElement("roundrect", DocxConstants.V_namespace);
            else if (shape.AutoShapeType == AutoShapeType.Oval)
                m_writer.WriteStartElement("oval", DocxConstants.V_namespace);
            else if (shape.AutoShapeType == AutoShapeType.Line)
                m_writer.WriteStartElement("line", DocxConstants.V_namespace);
            else if (reader != null)
            {
                m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
                string value = reader.GetAttribute("id");
                m_writer.WriteAttributeString("id", value + " " + (m_document.AutoShapeCollection.Count + 1).ToString());
                value = reader.GetAttribute("coordsize");
                m_writer.WriteAttributeString("coordsize", value);
                value = reader.GetAttribute("spt", DocxConstants.O_namespace);
                m_writer.WriteAttributeString("spt", value);
                value = reader.GetAttribute("adj");
                m_writer.WriteAttributeString("adj", value);
                value = reader.GetAttribute("path");
                m_writer.WriteAttributeString("path", value);
            }
            else
            {
                m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
                m_writer.WriteAttributeString("type", "#" + shapeType);
            }
            if (shape.IsHorizontalRule)
                SerializeHorizontalRule(shape, shape.Docx2007Props);
            else
                SerializeTextBoxFormat(shape, shape.Docx2007Props);
            SerializeFillEffects(shape, shape.Docx2007Props);
            SerializeDocxStream(shape.Docx2007Props, "callout");
            SerializeDocxStream(shape.Docx2007Props, "shadow");
            SerializeDocxStream(shape.Docx2007Props, "extrusion");
            m_writer.WriteStartElement("textbox", DocxConstants.V_namespace);
            StringBuilder textBoxStyle = new StringBuilder();
            if (shape.TextFrame.TextDirection == TextDirection.VerticalTopToBottom)
                textBoxStyle.Append("layout-flow:vertical");
            else if (shape.TextFrame.TextDirection == TextDirection.VerticalBottomToTop)
                textBoxStyle.Append("layout-flow:vertical;mso-layout-flow-alt:bottom-to-top");

            m_writer.WriteAttributeString("style", textBoxStyle.ToString());
            m_writer.WriteStartElement("txbxContent", DocxConstants.W_namespace);

            for (int i = 0, count = shape.TextBody.Items.Count; i < count; i++)
            {
                SerializeBodyItem(shape.TextBody.Items[i], false);
            }

            m_writer.WriteEndElement();//end of txbxContent tag
            m_writer.WriteEndElement();//end of textbox tag
            m_writer.WriteEndElement();//end of shape tag
            m_writer.WriteEndElement();//end of pict tag
        }

        private void SerializeHorizontalRule(Shape shape, Dictionary<string, Stream> dictionary)
        {
            StringBuilder textBoxStyle = new StringBuilder();
            textBoxStyle.Append("width:");
            textBoxStyle.Append(shape.Width.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;height:");
            textBoxStyle.Append(shape.Height.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;");            
            if (shape.DocxStyleProps.Count>0)
            {
                foreach (string prop in shape.DocxStyleProps)
                    textBoxStyle.Append(";" + prop);
            }
            m_writer.WriteAttributeString("style", textBoxStyle.ToString());
            SerializeHorizontalRule(shape);
            if (!string.IsNullOrEmpty(shape.Adjustments))
                m_writer.WriteAttributeString("adj", shape.Adjustments);
            if (shape.FillFormat.Fill)
            {
                m_writer.WriteAttributeString("filled", "t");
                if (shape.FillFormat.FillType == FillType.FillSolid && !shape.FillFormat.Color.IsEmpty)
                    m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(shape.FillFormat.Color));
                else if (!shape.FillFormat.ForeColor.IsEmpty)
                    m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(shape.FillFormat.ForeColor));
            }
            else
                m_writer.WriteAttributeString("filled", "f");

            if (!shape.LayoutInCell)
                m_writer.WriteAttributeString("allowincell", DocxConstants.O_namespace, "f");

            if (!shape.LineFormat.Line)
                m_writer.WriteAttributeString("stroked", "f");
            else
            {
                string dashStyle = GetDashStyle(shape.LineFormat.DashStyle,true);
                string lineStyle = GetLineStyle(shape.LineFormat.Style, true);
                if (dashStyle != null || lineStyle != null)
                {
                    m_writer.WriteAttributeString("strokecolor", "#" + GetRGBCode(shape.LineFormat.Color));
                    m_writer.WriteAttributeString("strokeweight", shape.LineFormat.Weight.ToString().Replace(",", ".") + "pt");
                    m_writer.WriteStartElement("stroke", DocxConstants.V_namespace);
                    if (dashStyle != null)
                        m_writer.WriteAttributeString("dashstyle", dashStyle);
                    if (lineStyle != null)
                        m_writer.WriteAttributeString("linestyle", lineStyle);
                    m_writer.WriteAttributeString("joinstyle", GetLineJoinStyle(shape.LineFormat.LineJoin));
                    m_writer.WriteAttributeString("endcap", GetLineCapStyle(shape.LineFormat.LineCap,true));
                    m_writer.WriteEndElement();
                }
            }

            if (shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.InFrontOfText &&
              shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.Behind)
            {
                m_writer.WriteStartElement("wrap", DocxConstants.W10_namespace);
                m_writer.WriteAttributeString("type", GetTextWrappingStyleAsString(shape.WrapFormat.TextWrappingStyle));
                m_writer.WriteEndElement();
            }
        }

        private void SerializeHorizontalRule(Shape shape)
        {
            if (shape.Width > 0)
                m_writer.WriteAttributeString("hrpct", DocxConstants.O_namespace, "0");
            string value = "left";
            if (shape.HorizontalAlignment == ShapeHorizontalAlignment.Center)
                value = "center";
            else if (shape.HorizontalAlignment == ShapeHorizontalAlignment.Right)
                value = "right";
            m_writer.WriteAttributeString("hralign", DocxConstants.O_namespace, value);
            m_writer.WriteAttributeString("hr", DocxConstants.O_namespace, "t");
            if (shape.UseStandardColorHR)
                m_writer.WriteAttributeString("hrstd", DocxConstants.O_namespace, "t");
            if (shape.UseNoShadeHR)
                m_writer.WriteAttributeString("hrnoshade", DocxConstants.O_namespace, "t");
        }

        private void SerializeShapeType(XmlWriter m_writer,Shape shape, ref string shapeType, ref XmlReader reader)
        {
            //Serialize shape type from embedded resource based on autoshape type.
            Stream xmlStream;

#if WINRT
                    Assembly execAssm = typeof(Syncfusion.DocIO.DLS.Style.BuiltinStyleLoader).GetTypeInfo().Assembly;
#else
            Assembly execAssm = Assembly.GetExecutingAssembly();
#endif
            xmlStream = execAssm.GetManifestResourceStream("Syncfusion.DocIO.Resources.ShapeTypes.txt");
            StreamReader sr = new StreamReader(xmlStream);
            
            string value = sr.ReadToEnd();
            string[] shapeTypes = value.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string typeAttribute = AutoShapeHelper.GetShapeTypeIDorAttributeToCheck(shape.AutoShapeType);
            for (int i = 0; i < shapeTypes.Length; i++)
            {
                if (shapeTypes[i].Contains(typeAttribute))
                {
                    if (shapeTypes[i].StartsWith("<v:shapetype")) //if (!testarr[i].Contains("o:spt=\"100\""))
                    {
                        shapeType = shapeTypes[i].Substring(17, shapeTypes[i].IndexOfAny(new char[] { '\"' }, 17) - 17);
                        m_writer.WriteRaw(shapeTypes[i]);
                        break;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        StreamWriter sw = new StreamWriter(ms);
                        sw.WriteLine("<ShapeTypes xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\"            xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"            xmlns:o=\"urn:schemas-microsoft-com:office:office\"             xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\"             xmlns:v=\"urn:schemas-microsoft-com:vml\"             xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\"             xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\"             xmlns:w10=\"urn:schemas-microsoft-com:office:word\"             xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"             xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\"             xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\"             xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\"             xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\"             xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\"             xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\"             mc:Ignorable=\"w14 w15 wp14\">  ");
                        sw.WriteLine(shapeTypes[i]);
                        sw.WriteLine("</ShapeTypes>");
                        sw.Flush();
                        ms.Position = 0;
                        reader = CreateReader(ms);
                        ms.Dispose();
                        reader.ReadToFollowing("shape", DocxConstants.V_namespace);
                        break;
                    }
                }
            }
            
        }

       
        private MemoryStream ReadSingleNodeIntoStream(XmlReader reader)
        {
            MemoryStream result = new MemoryStream();
            XmlWriter writer = UtilityMethods.CreateWriter(result, Encoding.UTF8);
            writer.WriteNode(reader, false);
            writer.Flush();
            return result;
        }
        private void SerializeTextBoxFormat(Shape shape, Dictionary<string, Stream> docxProps)
        {
            string horiz = string.Empty;
            string horizAlign = string.Empty;
            string vert = string.Empty;
            string vertAlign = string.Empty;
            string textVertAlign = string.Empty;
            StringBuilder textBoxStyle = new StringBuilder();

            if (shape.HorizontalOrigin != HorizontalOrigin.Column)
                horiz = GetHorizOriginAsString(shape.HorizontalOrigin);

            if (shape.VerticalOrigin != VerticalOrigin.Paragraph)
                vert = GetVerticalOrginAsString (shape.VerticalOrigin);

            if (shape.HorizontalAlignment != ShapeHorizontalAlignment.None)
                horizAlign = shape.HorizontalAlignment.ToString().ToLower();

            if (shape.VerticalAlignment != ShapeVerticalAlignment.None)
                vertAlign = shape.VerticalAlignment.ToString().ToLower();

            //Serialize TextBox Vertical alignment of the Text
            textVertAlign = shape.TextFrame.TextVerticalAlignment.ToString().ToLower();

            if (shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                textBoxStyle.Append("position:absolute;");
            
            textBoxStyle.Append("margin-left:");
            textBoxStyle.Append(shape.HorizontalPosition.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;margin-top:");
            textBoxStyle.Append(shape.VerticalPosition.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;width:");
            textBoxStyle.Append(shape.Width.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;height:");
            textBoxStyle.Append(shape.Height.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;");

            if (shape.ZOrderPosition != 0)
                textBoxStyle.Append("z-index:" + shape.ZOrderPosition.ToString() + ";");
            else if (shape.IsBelowText)
                textBoxStyle.Append("z-index:-251658752;");

            if (horiz.Length != 0)
            {
                textBoxStyle.Append("mso-position-horizontal-relative:");
                textBoxStyle.Append(horiz);
            }

            if (vert.Length != 0)
            {
                textBoxStyle.Append(";mso-position-vertical-relative:");
                textBoxStyle.Append(vert);
            }

            if (horizAlign.Length != 0)
            {
                textBoxStyle.Append(";mso-position-horizontal:");
                textBoxStyle.Append(horizAlign);
            }

            if (vertAlign.Length != 0)
            {
                textBoxStyle.Append(";mso-position-vertical:");
                textBoxStyle.Append(vertAlign);
            }
            //Serialize TextBox Vertical Alignment of the Text
            if (textVertAlign.Length != 0)
            {
                textBoxStyle.Append(";v-text-anchor:");
                textBoxStyle.Append(textVertAlign);
            }
            if (shape.TextFrame.HorizontalRelativePercent != float.MinValue)
            {
                textBoxStyle.Append(";mso-left-percent:");
                textBoxStyle.Append(shape.TextFrame.HorizontalRelativePercent * 10);
            }
            if (shape.TextFrame.VerticalRelativePercent != float.MinValue)
            {
                textBoxStyle.Append(";mso-top-percent:");
                textBoxStyle.Append(shape.TextFrame.VerticalRelativePercent * 10);
            }
            textBoxStyle.Append(";mso-wrap-distance-left:");
            textBoxStyle.Append(shape.WrapFormat.DistanceLeft.ToString(CultureInfo.InvariantCulture));
            textBoxStyle.Append("pt;mso-wrap-distance-top:");
            textBoxStyle.Append(shape.WrapFormat.DistanceTop.ToString(CultureInfo.InvariantCulture));
            textBoxStyle.Append("pt;mso-wrap-distance-right:");
            textBoxStyle.Append(shape.WrapFormat.DistanceRight.ToString(CultureInfo.InvariantCulture));
            textBoxStyle.Append("pt;mso-wrap-distance-bottom:");
            textBoxStyle.Append(shape.WrapFormat.DistanceBottom.ToString(CultureInfo.InvariantCulture));
            textBoxStyle.Append("pt;");

            if (shape.DocxStyleProps.Count>0)
            {
                foreach (string prop in shape.DocxStyleProps)
                {
                    textBoxStyle.Append(";" + prop);
                }
            }

            m_writer.WriteAttributeString("style", textBoxStyle.ToString());

            if(!string.IsNullOrEmpty(shape.Adjustments))
            m_writer.WriteAttributeString("adj", shape.Adjustments);
            if (shape.ArcSize > 0)
                m_writer.WriteAttributeString("arcsize", shape.ArcSize.ToString() + "f");
            if (shape.FillFormat.Fill)
            {
                m_writer.WriteAttributeString("filled", "t");
                if (shape.FillFormat.FillType == FillType.FillSolid && !shape.FillFormat.Color.IsEmpty && shape.FillFormat.ForeColor.IsEmpty)
                    m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(shape.FillFormat.Color));
                else if (!shape.FillFormat.ForeColor.IsEmpty)
                    m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(shape.FillFormat.ForeColor));
            }
            else
                m_writer.WriteAttributeString("filled", "f");

            if (!shape.LayoutInCell)
            {
                m_writer.WriteAttributeString("allowincell", DocxConstants.O_namespace, "f");
            }

            if (!shape.LineFormat.Line)
            {
                m_writer.WriteAttributeString("stroked", "f");
            }
            else
            {
                string dashStyle = GetDashStyle(shape.LineFormat.DashStyle,true);
                string lineStyle = GetLineStyle(shape.LineFormat.Style, true);

                if (dashStyle != null || lineStyle != null)
                {
                    m_writer.WriteAttributeString("strokecolor", "#" + GetRGBCode(shape.LineFormat.Color));
                    m_writer.WriteAttributeString("strokeweight", shape.LineFormat.Weight.ToString().Replace(",", ".") + "pt");
                    m_writer.WriteStartElement("stroke", DocxConstants.V_namespace);
                    if (dashStyle != null)
                        m_writer.WriteAttributeString("dashstyle", dashStyle);
                    if (lineStyle != null)
                        m_writer.WriteAttributeString("linestyle", lineStyle);

                    m_writer.WriteAttributeString("joinstyle", GetLineJoinStyle(shape.LineFormat.LineJoin));
                    m_writer.WriteAttributeString("endcap", GetLineCapStyle(shape.LineFormat.LineCap,true));
                    SerializePatternLine(shape);
                    if(IsConnectorShape(shape.AutoShapeType))
                        SerializeConnectorLine2007Properties(shape.LineFormat);

                    m_writer.WriteEndElement();
                }
            }

            if (shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.InFrontOfText &&
              shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.Behind)
            {
                m_writer.WriteStartElement("wrap", DocxConstants.W10_namespace);
                m_writer.WriteAttributeString("type", GetTextWrappingStyleAsString(shape.WrapFormat.TextWrappingStyle));
                m_writer.WriteEndElement();
            }
        }

        private void SerializeConnectorLine2007Properties(LineFormat lineFormat)
        {
            m_writer.WriteAttributeString("startarrow", GetLineEnd(lineFormat.BeginArrowheadStyle, true));
            m_writer.WriteAttributeString("startarrowwidth", GetLineEndWidth(lineFormat.BeginArrowheadWidth, true));
            m_writer.WriteAttributeString("startarrowlength", GetLineEndLength(lineFormat.BeginArrowheadLength, true));
            m_writer.WriteAttributeString("endarrow", GetLineEnd(lineFormat.EndArrowheadStyle, true));
            m_writer.WriteAttributeString("endarrowwidth", GetLineEndWidth(lineFormat.EndArrowheadWidth, true));
            m_writer.WriteAttributeString("endarrowlength", GetLineEndLength(lineFormat.EndArrowheadLength, true));
        }

        private void SerializePatternLine(Shape shape)
        {
            byte[] imgBytes = null;
            if (shape.LineFormat.ImageRecord != null)
                imgBytes = shape.LineFormat.ImageRecord.ImageBytes;
            if (shape.LineFormat.LineFormatType == LineFormatType.Patterned
                && shape.LineFormat.Pattern != PatternType.Mixed)
                imgBytes = GetPatternImageBytes(shape.LineFormat.Pattern.ToString());

            if (imgBytes != null)
            {
                Entity baseEntity = GetBaseEntity(shape);
                string id = string.Empty;
                WPicture pic = new WPicture(m_document);

                pic.LoadImage(imgBytes);
                UpdateImages(pic);
                if (baseEntity is WSection)
                {
                    id = AddImageRelation(DocumentImages, pic.ImageRecord);
                }
                else if (baseEntity is HeaderFooter)
                {
                    id = UpdateHFImageRels(baseEntity as HeaderFooter, pic);
                }
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id.ToString());
                m_writer.WriteAttributeString("title", DocxConstants.O_namespace, string.Empty);
            }

            uint opacity = Convert.ToUInt32(65536 * (1 - (shape.LineFormat.Transparency / 100)));
            m_writer.WriteAttributeString("opacity", opacity.ToString() + "f");

            if (!shape.LineFormat.Color.IsEmpty)
                m_writer.WriteAttributeString("color2", "#" + GetRGBCode(shape.LineFormat.Color));

            if (shape.LineFormat.LineFormatType == LineFormatType.Patterned)
                m_writer.WriteAttributeString("filltype", "pattern");
        }

        private void SerializeDocxStream(Dictionary<string, Stream> docxProps, string localName)
        {
            Stream memoryStream = new MemoryStream();
            if (docxProps.TryGetValue(localName, out memoryStream))
            {
                if (memoryStream != null && memoryStream.Length > 0)
                {
                    memoryStream.Position = 0;
                    XmlReader reader = CreateReader(memoryStream);
                    m_writer.WriteNode(reader, false);
                }
                return;
            }
        }
        private void SerializeDocxStream(Dictionary<string, Stream> docxProps, string localName, Shape shape)
        {
            Stream memoryStream = new MemoryStream();
            if (docxProps.TryGetValue(localName, out memoryStream))
            {
                if (memoryStream != null && memoryStream.Length > 0)
                {
                    memoryStream.Position = 0;
                    Stream updatedStream = UpdateXMLRelation(shape, memoryStream);
                    XmlReader reader = CreateReader(updatedStream);
                    m_writer.WriteNode(reader, false);
                }
                return;
            }
        }
        private void SerializeFillEffects(Shape shape, Dictionary<string, Stream> docxProps)
        {
            if (!shape.FillFormat.Fill)
                return;
            switch (shape.FillFormat.FillType)
            {
                case FillType.FillSolid:
                    SerializeSolidFill2007(shape);
                    break;
                case FillType.FillPatterned:
                case FillType.FillPicture:
                case FillType.FillTextured:
                    SerializeBlipFill2007(shape);
                    break;
                case FillType.FillGradient:
                    SerializeGridFill2007(shape);
                    break;
                default:
                    SerializeDocxStream(docxProps, "fill", shape);
                    break;
            }
        }

        private void SerializeSolidFill2007(Shape shape)
        {
            m_writer.WriteStartElement("fill", DocxConstants.V_namespace);
            uint opacity = Convert.ToUInt32(65536 * (1 - (shape.FillFormat.Transparency / 100)));
            m_writer.WriteAttributeString("opacity", opacity.ToString() + "f");
            if (!shape.FillFormat.Color.IsEmpty)
                m_writer.WriteAttributeString("color2", "#" + GetRGBCode(shape.FillFormat.Color));
            m_writer.WriteEndElement();
        }

        private void SerializeGridFill2007(Shape shape)
        {
            m_writer.WriteStartElement("fill", DocxConstants.V_namespace);
            uint opacity = Convert.ToUInt32(65536 * (1 - (shape.FillFormat.Transparency / 100)));
            string type = "gradientRadial";
            m_writer.WriteAttributeString("opacity", opacity.ToString() + "f");
            
            m_writer.WriteAttributeString("color2", "#" + GetRGBCode(shape.FillFormat.Color != Color.Empty ? shape.FillFormat.Color :
                shape.FillFormat.GradientFill.GradientStops.Count > 0 ? shape.FillFormat.GradientFill.GradientStops[shape.FillFormat.GradientFill.GradientStops.Count - 1].Color : shape.FillFormat.Color));
            m_writer.WriteAttributeString("rotate", shape.FillFormat.GradientFill.RotateWithShape ? "t" : "f");
            if (shape.FillFormat.GradientFill.LinearGradient != null)
            {
                m_writer.WriteAttributeString("angle", GetAngle(shape.FillFormat.GradientFill.LinearGradient.Angle, shape.FillFormat.GradientFill.LinearGradient.AnglePositive));
                type = "gradient";
            }
            m_writer.WriteAttributeString("colors", GetGradientStopAsColors(shape.FillFormat.GradientFill));
            if (!string.IsNullOrEmpty(shape.FillFormat.GradientFill.Focus))
                m_writer.WriteAttributeString("focus", shape.FillFormat.GradientFill.Focus);
            m_writer.WriteAttributeString("focussize", "");
            m_writer.WriteAttributeString("type", type);

            if (shape.FillFormat.GradientFill.PathGradient != null)
            {
                string focusPosition = GetFocusPosition(shape);
                if(!string.IsNullOrEmpty(focusPosition))
                    m_writer.WriteAttributeString("focusposition", focusPosition);
                if (shape.FillFormat.GradientFill.PathGradient.PathShade == GradientShadeType.Rectangle)
                {
                    m_writer.WriteStartElement("fill", DocxConstants.O_namespace);
                    m_writer.WriteAttributeString("ext", "view");
                    m_writer.WriteAttributeString("type", "gradientCenter");
                    m_writer.WriteEndElement();
                }
            }
            m_writer.WriteEndElement();
        }

        private string GetFocusPosition(Shape shape)
        {
            GradientFill gradientFill = shape.FillFormat.GradientFill;
            string focusPos = string.Empty;
            if (gradientFill.TileRectangle.TopOffset == -100 &&
                gradientFill.TileRectangle.LeftOffset == -100 &&
                gradientFill.PathGradient.RightOffset == 100 &&
                gradientFill.PathGradient.BottomOffset == 100)
            {
                return null; 
            }
            else if (gradientFill.TileRectangle.TopOffset == -100 &&
                gradientFill.TileRectangle.LeftOffset == 100 &&
                gradientFill.PathGradient.RightOffset == -100 &&
                gradientFill.PathGradient.BottomOffset == 100)
            {
                focusPos = "1";
            }
            else if (gradientFill.TileRectangle.TopOffset == 100 &&
                gradientFill.TileRectangle.LeftOffset == 100 &&
                gradientFill.PathGradient.RightOffset == -100 &&
                gradientFill.PathGradient.BottomOffset == -100)
            {
                focusPos = "1,1";
            }
            else if (gradientFill.TileRectangle.TopOffset == 100 &&
                gradientFill.TileRectangle.LeftOffset == -100 &&
                gradientFill.PathGradient.RightOffset == 100 &&
                gradientFill.PathGradient.BottomOffset == -100)
            {
                focusPos = ",1";
            }
            else if (gradientFill.TileRectangle.TopOffset == 50 &&
                    gradientFill.TileRectangle.LeftOffset == 50 &&
                    gradientFill.PathGradient.RightOffset == 50 &&
                    gradientFill.PathGradient.BottomOffset == 50)
            {
                focusPos = ".5,.5";
            }
            return focusPos;
        }

        private string GetShadingVariant(GradientShadingVariant gradientShadingVariant)
        {
            string shadingVariant = string.Empty;
            switch (gradientShadingVariant)
            {
                case GradientShadingVariant.ShadingUp:
                    shadingVariant = "100%";
                    break;
                case GradientShadingVariant.ShadingMiddle:
                    shadingVariant = "50%";
                    break;
                case GradientShadingVariant.ShadingDown:
                    shadingVariant = "0%";
                    break;
                case GradientShadingVariant.ShadingOut:
                    shadingVariant = "25%";
                    break;
            }
            return shadingVariant;
        }

        private string GetGradientStopAsColors(GradientFill gradientFill)
        {
            string gradientStops = string.Empty;
            SortGradientStops(gradientFill);
            for (int i = 0; i < gradientFill.GradientStops.Count; i++)
            {
                gradientStops += gradientFill.GradientStops[i].Position * DLSConstants.FixedPointsUnit / 100 + "f";
                gradientStops += " #" + GetRGBCode(gradientFill.GradientStops[i].Color);
                if (i != gradientFill.GradientStops.Count - 1)
                    gradientStops += ";";
            }
            return gradientStops;
        }

        private void SortGradientStops(GradientFill gradientFill)
        {
            for (int i = 0; i < gradientFill.GradientStops.Count - 1; i++)
            {
                for (int j = i + 1; j < gradientFill.GradientStops.Count - 1; j++)
                {
                    if (gradientFill.GradientStops[i].Position > gradientFill.GradientStops[j].Position)
                    {
                        GradientStop temp = gradientFill.GradientStops[i];
                        gradientFill.GradientStops[i] = gradientFill.GradientStops[j];
                        gradientFill.GradientStops[j] = temp;
                    }
                }
            }
        }

        private string GetAngle(short angle, bool isAnglePositive)
        {
            int angle2013 = Convert.ToInt32(angle);
            int angle2007 = int.MaxValue;
            if (isAnglePositive)
            {
                if (angle2013 >= 0 && angle2013 <= 90)
                    angle2007 = 90 - angle2013;
                else if (angle2013 > 90 && angle2013 <= 180)
                    angle2007 = 270 + (180 - angle2013);
                else if (angle2013 > 180 && angle2013 <= 270)
                    angle2007 = 180 + (270 - angle2013);
                else if (angle2013 > 270 && angle2013 <= 360)
                    angle2007 = 90 + (360 - angle2013);

                angle2007 = angle2007 == 360 ? 0 : angle2007;
            }
            else
            {
                if (angle2013 >= 0 && angle2013 <= 90)
                    angle2007 = 90 + angle2013;
                else if (angle2013 > 90 && angle2013 <= 180)
                    angle2007 = 180 + (angle2013 - 90);
                else if (angle2013 > 180 && angle2013 <= 270)
                    angle2007 = 270 + (angle2013 - 180);
                else if (angle2013 > 270 && angle2013 <= 360)
                    angle2007 = 0 + (angle2013 - 270);

                angle2007 = angle2007 == 360 ? 0 : -angle2007;
            }
            return angle2007.ToString();
        }

        private void SerializeBlipFill2007(Shape shape)
        {
            m_writer.WriteStartElement("fill", DocxConstants.V_namespace);
            
            byte[] imgBytes = null;
            if (shape.FillFormat.ImageRecord != null)
                imgBytes = shape.FillFormat.ImageRecord.ImageBytes;
            if (shape.FillFormat.FillType == FillType.FillPatterned 
                && shape.FillFormat.Pattern != PatternType.Mixed)
                imgBytes = GetPatternImageBytes(shape.FillFormat.Pattern.ToString());

            if (imgBytes != null)
            {
                Entity baseEntity = GetBaseEntity(shape);
                string id = string.Empty;
                WPicture pic = new WPicture(m_document);
                pic.LoadImage(imgBytes);
                UpdateImages(pic);
                if (baseEntity is WSection)
                {
                    id = AddImageRelation(DocumentImages, pic.ImageRecord);
                }
                else if (baseEntity is HeaderFooter)
                {
                    id = UpdateHFImageRels(baseEntity as HeaderFooter, pic);
                }
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id.ToString());
            }
            
            uint opacity = Convert.ToUInt32(65536 * (1 - (shape.FillFormat.Transparency / 100)));
            m_writer.WriteAttributeString("opacity", opacity.ToString() + "f");
            
            if (!shape.FillFormat.Color.IsEmpty)
                m_writer.WriteAttributeString("color2", "#" + GetRGBCode(shape.FillFormat.Color));
            m_writer.WriteAttributeString("rotate", shape.FillFormat.RotateWithObject ? "t" : "f");
            
            if (shape.FillFormat.FillType == FillType.FillPatterned)
                m_writer.WriteAttributeString("type", "pattern");
            else if (shape.FillFormat.FillType == FillType.FillPicture)
                m_writer.WriteAttributeString("type", "frame");
            else
                m_writer.WriteAttributeString("type", "tile");
            
            m_writer.WriteEndElement();
        }

        private byte[] GetPatternImageBytes(string patternType)
        {
#if WINRT
            Assembly execAssm = typeof(DocxSerializator).GetTypeInfo().Assembly;
#else
            Assembly execAssm = Assembly.GetExecutingAssembly();
#endif
            Stream xmlStream = execAssm.GetManifestResourceStream("Syncfusion.DocIO.Resources" + ".PatternFillResources.xml");
            XmlReader reader = XmlReader.Create(xmlStream);
            byte[] imgBytes = null;
            while (!reader.EOF)
            {
                reader.ReadToFollowing("name");
                if (!reader.EOF)
                {
                    reader.Read();
                    //skip whitespaces
                    SkipWhitespaces(reader);
                    string name = reader.Value;
                    if (name == patternType)
                    {
                        reader.ReadToFollowing("patternBytes");
                        if (!reader.EOF)
                        {
                            reader.Read();
                            SkipWhitespaces(reader);
                            string patternBytes = reader.Value;
                            imgBytes = System.Convert.FromBase64String(patternBytes);
                            return imgBytes;
                        }
                    }
                }
                //skip whitespaces
                SkipWhitespaces(reader);
            }
            return imgBytes;
        }
        private void SkipWhitespaces(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
                return;

            while (reader.NodeType == XmlNodeType.Whitespace)
                reader.Read();
        }
        /// <summary>
        /// Gets the emu from point.
        /// </summary>
        /// <param name="pointValue">The point value.</param>
        /// <returns></returns>
        private string GetEmuFromPoint(decimal pointValue)
        {
            decimal value = Math.Round(pointValue * DLSConstants.EmusPerPoint);
            return value.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Serializze the absolutely positioned picture.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializeAbsolutePicture(Shape shape)
        {
            m_writer.WriteStartElement("anchor", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("distT", GetEmuFromPoint((decimal)shape.WrapFormat.DistanceTop));
            m_writer.WriteAttributeString("distB", GetEmuFromPoint((decimal)shape.WrapFormat.DistanceBottom));
            m_writer.WriteAttributeString("distL", GetEmuFromPoint((decimal)shape.WrapFormat.DistanceLeft));
            m_writer.WriteAttributeString("distR", GetEmuFromPoint((decimal)shape.WrapFormat.DistanceRight));
            m_writer.WriteAttributeString("simplePos", "0");

            m_writer.WriteAttributeString("relativeHeight", shape.ZOrderPosition.ToString());
            string isBelowText = (shape.IsBelowText) ? "1" : "0";
            m_writer.WriteAttributeString("behindDoc", isBelowText);
            string lockAnchor = (shape.LockAnchor) ? "1" : "0";
            m_writer.WriteAttributeString("locked", lockAnchor);
            if (shape.LayoutInCell)
                m_writer.WriteAttributeString("layoutInCell", "1");
            else
                m_writer.WriteAttributeString("layoutInCell", "0");
            string allowOverlap = (shape.WrapFormat.AllowOverlap) ? "1" : "0";
            m_writer.WriteAttributeString("allowOverlap", allowOverlap);

            m_writer.WriteStartElement("simplePos", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("x", "0");
            m_writer.WriteAttributeString("y", "0");
            m_writer.WriteEndElement(); //end of simplePos

            m_writer.WriteStartElement("positionH", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("relativeFrom", GetShapeHorzOrigin(shape.HorizontalOrigin));

            if (shape.HorizontalAlignment == ShapeHorizontalAlignment.None)
            {
                m_writer.WriteStartElement("posOffset", DocxConstants.WP_namespace);
                int horPos = (int)Math.Round(shape.HorizontalPosition * DLSConstants.EmusPerPoint);
                m_writer.WriteString(horPos.ToString());
                m_writer.WriteEndElement(); //end of posOffset
            }
            else
            {
                m_writer.WriteStartElement("align", DocxConstants.WP_namespace);
                string horAlig = shape.HorizontalAlignment.ToString().ToLower();
                m_writer.WriteString(horAlig);
                m_writer.WriteEndElement(); //end of align
            }
            m_writer.WriteEndElement();//end of postionH

            m_writer.WriteStartElement("positionV", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("relativeFrom", shape.VerticalOrigin.ToString().ToLower());
            if (shape.VerticalAlignment == ShapeVerticalAlignment.None)
            {
                m_writer.WriteStartElement("posOffset", DocxConstants.WP_namespace);
                int vertPos = (int)Math.Round(shape.VerticalPosition * DLSConstants.EmusPerPoint);
                m_writer.WriteString(vertPos.ToString());
                m_writer.WriteEndElement(); // end of posOffset
            }
            else
            {
                m_writer.WriteStartElement("align", DocxConstants.WP_namespace);
                string verAlig = shape.VerticalAlignment.ToString().ToLower();
                m_writer.WriteString(verAlig);
                m_writer.WriteEndElement(); //end of align
            }
            m_writer.WriteEndElement(); //end of postionV

            m_writer.WriteStartElement("extent", DocxConstants.WP_namespace);
            int cx = (int)Math.Round(shape.Width * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cx", cx.ToString());
            int cy = (int)Math.Round(shape.Height * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cy", cy.ToString());
            m_writer.WriteEndElement(); //end of extent


            switch (shape.WrapFormat.TextWrappingStyle)
            {
                case TextWrappingStyle.Square:
                    m_writer.WriteStartElement("wrapSquare", DocxConstants.WP_namespace);
                    m_writer.WriteAttributeString("wrapText", GetPictureWrappingTypeAsString(shape.WrapFormat.TextWrappingType));
                    m_writer.WriteEndElement();
                    break;
                case TextWrappingStyle.Through:
                    m_writer.WriteStartElement("wrapThrough", DocxConstants.WP_namespace);
                    m_writer.WriteAttributeString("wrapText", GetPictureWrappingTypeAsString(shape.WrapFormat.TextWrappingType));
                    SerializeWrapPolygon(shape,shape.WrapFormat.WrapPolygon);
                    m_writer.WriteEndElement();
                    break;
                case TextWrappingStyle.Tight:
                    m_writer.WriteStartElement("wrapTight", DocxConstants.WP_namespace);
                    m_writer.WriteAttributeString("wrapText", GetPictureWrappingTypeAsString(shape.WrapFormat.TextWrappingType));
                    SerializeWrapPolygon(shape,shape.WrapFormat.WrapPolygon);
                    m_writer.WriteEndElement();
                    break;
                case TextWrappingStyle.TopAndBottom:
                    m_writer.WriteStartElement("wrapTopAndBottom", DocxConstants.WP_namespace);
                    m_writer.WriteEndElement();
                    break;
                default:
                    m_writer.WriteStartElement("wrapNone", DocxConstants.WP_namespace);
                    m_writer.WriteEndElement();
                    break;
            }

            SerializeDrawingGraphics(shape);
            m_writer.WriteEndElement();
        }

        /// <summary>
        /// Get Shape Horizontal orgin
        /// </summary>
        /// <param name="horzOrigin"></param>
        /// <returns></returns>
        private string GetShapeHorzOrigin(HorizontalOrigin horzOrigin)
        {
            switch (horzOrigin)
            {
                case HorizontalOrigin.LeftMargin:
                    return "leftMargin";
                case HorizontalOrigin.RightMargin:
                    return "rightMargin";
                case HorizontalOrigin.InsideMargin:
                    return "insideMargin";
                case HorizontalOrigin.OutsideMargin:
                    return "outsideMargin";
                default:
                    return horzOrigin.ToString().ToLower();
            }
        }
        /// <summary>
        /// Get Shape Horizontal orgin of Fall back
        /// </summary>
        /// <param name="horzOrigin"></param>
        /// <returns></returns>
        private string GetHorizOriginAsString(HorizontalOrigin horzOrigin)
        {
            switch (horzOrigin)
            {
                case HorizontalOrigin.LeftMargin:
                    return "left-margin-area";
                case HorizontalOrigin.RightMargin:
                    return "right-margin-are";
                case HorizontalOrigin.InsideMargin:
                    return "inner-margin-area";
                case HorizontalOrigin.OutsideMargin:
                    return "outer-margin-area";
                default:
                    return horzOrigin.ToString().ToLower();
            }
        }
        /// <summary>
        /// Serialize the inline picture.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializeInlinePicture(Shape shape)
        {
            m_writer.WriteStartElement("inline", DocxConstants.WP_namespace);
            m_writer.WriteStartElement("extent", DocxConstants.WP_namespace);
            int cx = (int)Math.Round(((shape.Width * shape.WidthScale) / 100) * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cx", cx.ToString());
            int cy = (int)Math.Round(((shape.Height * shape.HeightScale) / 100) * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cy", cy.ToString());
            m_writer.WriteEndElement();

            SerializeDrawingGraphics(shape);
            m_writer.WriteEndElement();
        }
        private void SerializeDrawingGraphics(Shape shape)
        {

            // Processing shape
            m_writer.WriteStartElement("wp", "docPr", DocxConstants.WP_namespace);
            int id = GetNextDocPrID();//m_document.AutoShapeCollection.IndexOf(shape) + 1;
            m_writer.WriteAttributeString("id", id.ToString());
            string name = "";
            if (shape.Name != null && shape.Name.Length > 0)
                name = shape.Name;
            else
                name = shape.AutoShapeType.ToString();
            m_writer.WriteAttributeString("name", name);
            m_writer.WriteAttributeString("title", shape.Title);
            if (shape.AlternativeText != null)
                m_writer.WriteAttributeString("descr", shape.AlternativeText);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("a", "graphic", DocxConstants.A_namespace);
            m_writer.WriteStartElement("a", "graphicData", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("uri", DocxConstants.WPS_namespace);
            m_writer.WriteStartElement("wps", "wsp", DocxConstants.WPS_namespace);
            m_writer.WriteStartElement("wps", "cNvSpPr", DocxConstants.WPS_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("wps", "spPr", DocxConstants.WPS_namespace);
            m_writer.WriteStartElement("a", "xfrm", DocxConstants.A_namespace);
            if (shape.FlipHorizantal)
                m_writer.WriteAttributeString("flipH", "1");
            if (shape.FlipVertical)
                m_writer.WriteAttributeString("flipV", "1");
            m_writer.WriteStartElement("a", "off", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("x", "0");
            m_writer.WriteAttributeString("y", "0");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("a", "ext", DocxConstants.A_namespace);
            int cx = (int)Math.Round((((shape.Width * shape.WidthScale) / 100) * DLSConstants.EmusPerPoint));
            m_writer.WriteAttributeString("cx", cx.ToString());
            int cy = (int)Math.Round((((shape.Height * shape.HeightScale) / 100) * DLSConstants.EmusPerPoint));
            m_writer.WriteAttributeString("cy", cy.ToString());
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            if (!shape.DocxProps.ContainsKey("custGeom"))
            {
                m_writer.WriteStartElement("a", "prstGeom", DocxConstants.A_namespace);

                AutoShapeType type = shape.AutoShapeType;
                AutoShapeConstant constant = AutoShapeHelper.GetAutoShapeConstant(type);
                string value = AutoShapeHelper.GetAutoShapeString(constant);

                m_writer.WriteAttributeString("prst", value);
                m_writer.WriteStartElement("a", "avLst", DocxConstants.A_namespace);
                foreach (KeyValuePair<string, string> shapeGuide in shape.ShapeGuide)
                {
                    m_writer.WriteStartElement("a", "gd", DocxConstants.A_namespace);
                    m_writer.WriteAttributeString("name", shapeGuide.Key);
                    m_writer.WriteAttributeString("fmla", shapeGuide.Value);
                    m_writer.WriteEndElement();
                }
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }
            else
            {
                SerializeDocxStream(shape.DocxProps, "custGeom", shape);
            }
            if (shape.FillFormat.Fill)
            {
                switch (shape.FillFormat.FillType)
                {
                    case FillType.FillTextured:
                    case FillType.FillPicture:
                        SerializeBlipFill(shape);
                        break;
                    case FillType.FillGradient:
                        SerializeGradientFill(shape.FillFormat.GradientFill);
                        break;
                    case FillType.FillPatterned:
                        SerializePatternFill(shape.FillFormat);
                        break;
                    default:
                        SerializeSolidFill(shape.FillFormat.Color, shape.FillFormat.Transparency);
                        break;
                }
            }
            else
            {
                m_writer.WriteStartElement("noFill", DocxConstants.A_namespace);
                m_writer.WriteEndElement();
            }
            
            m_writer.WriteStartElement("ln", DocxConstants.A_namespace);
            if (shape.LineFormat.InsetPen)
                m_writer.WriteAttributeString("algn", "in");
            m_writer.WriteAttributeString("cap", GetLineCapStyle(shape.LineFormat.LineCap,false));
            m_writer.WriteAttributeString("cmpd", GetLineStyle(shape.LineFormat.Style, false));
            float width = (shape.LineFormat.Weight * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("w", width.ToString());
            if (shape.LineFormat.Line)
            {
                if (shape.LineFormat.LineFormatType== LineFormatType.Gradient)
                {
                    SerializeGradientFill(shape.LineFormat.GradientFill);
                }
                else if (shape.LineFormat.LineFormatType == LineFormatType.Patterned)
                {
                    SerializePatternFill(shape.LineFormat);
                }
                else
                {
                    if (!shape.LineFormat.Color.IsEmpty)
                        SerializeSolidFill(shape.LineFormat.Color, shape.LineFormat.Transparency);
                }
            }
            else
            {
                m_writer.WriteStartElement("noFill", DocxConstants.A_namespace);
                m_writer.WriteEndElement();
            }
            if (shape.LineFormat.DocxProps.ContainsKey("custDash"))
            {
                SerializeDocxStream(shape.LineFormat.DocxProps, "custDash");
            }
            else
            {
                m_writer.WriteStartElement("prstDash", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("val", GetDashStyle(shape.LineFormat.DashStyle,false));
                m_writer.WriteEndElement();
            }
            m_writer.WriteStartElement(GetLineJoinStyle(shape.LineFormat.LineJoin));
            m_writer.WriteEndElement();
            if(IsConnectorShape(shape.AutoShapeType))
            {
                SerializeConnectorLineProperties(shape.LineFormat);
            }
            if (shape.LineFormat.DocxProps.ContainsKey("extLst"))
            {
                SerializeDocxStream(shape.LineFormat.DocxProps, "extLst");
            }

            m_writer.WriteEndElement();

            if (shape.DocxProps.ContainsKey("effectLst"))
            {
                SerializeDocxStream(shape.DocxProps, "effectLst");
            }
            if (shape.DocxProps.ContainsKey("extLst"))
            {
                SerializeDocxStream(shape.DocxProps, "extLst");
            }
            if (shape.DocxProps.ContainsKey("scene3d"))
            {
                SerializeDocxStream(shape.DocxProps, "scene3d");
            }
            if (shape.DocxProps.ContainsKey("sp3d"))
            {
                SerializeDocxStream(shape.DocxProps, "sp3d");
            }
            m_writer.WriteEndElement();


            if (shape.DocxProps.ContainsKey("Style"))
            {
                SerializeDocxStream(shape.DocxProps, "Style");
            }
            else
                SerializeAutoShapeStyles(shape);

            if (shape.TextBody.Items.Count >= 0)
                SerializeTextBoxContent(shape);
                SerializeBodyProperties(shape);

            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }

        private void SerializeConnectorLineProperties(LineFormat lineFormat)
        {
            m_writer.WriteStartElement("headEnd", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("type", GetLineEnd(lineFormat.BeginArrowheadStyle, false));
            m_writer.WriteAttributeString("w", GetLineEndWidth(lineFormat.BeginArrowheadWidth, false));
            m_writer.WriteAttributeString("len", GetLineEndLength(lineFormat.BeginArrowheadLength, false));
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("tailEnd", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("type", GetLineEnd(lineFormat.EndArrowheadStyle, false));
            m_writer.WriteAttributeString("w", GetLineEndWidth(lineFormat.EndArrowheadWidth, false));
            m_writer.WriteAttributeString("len", GetLineEndLength(lineFormat.EndArrowheadLength, false));
            m_writer.WriteEndElement();
        }

        private void SerializePatternFill(LineFormat lineFormat)
        {
            m_writer.WriteStartElement("pattFill", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("prst", GetPatternFillType(lineFormat.Pattern));
            //forecolor
            m_writer.WriteStartElement("fgClr", DocxConstants.A_namespace);

            m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", GetRGBCode(lineFormat.ForeColor));
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();
            //backcolor
            m_writer.WriteStartElement("bgClr", DocxConstants.A_namespace);

            m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", GetRGBCode(lineFormat.Color));
            uint alpha = Convert.ToUInt32(65536 * (1 - (lineFormat.Transparency / 100)));
            alpha = (uint)Math.Round((alpha * DLSConstants.HundredthsUnit) / (double)DLSConstants.FixedPointsUnit);
            alpha = alpha * DLSConstants.ThousandthsUnit;
            m_writer.WriteStartElement("alpha", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", alpha.ToString());
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }

        private bool IsConnectorShape(AutoShapeType autoShapeType)
        {
            switch(autoShapeType)
            {
                case AutoShapeType.Line:
                case AutoShapeType.ElbowConnector:
                case AutoShapeType.CurvedConnector:
                case AutoShapeType.StraightConnector:
                case AutoShapeType.BentConnector2:
                case AutoShapeType.BentConnector4:
                case AutoShapeType.BentConnector5:
                case AutoShapeType.CurvedConnector2:
                case AutoShapeType.CurvedConnector4:
                case AutoShapeType.CurvedConnector5:
                    return true;
                default:
                    return false;
            }
            return false;
        }

        private void SerializeBlipFill(Shape shape)
        {
            FillFormat fillFormat = shape.FillFormat;
            m_writer.WriteStartElement("blipFill", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("rotWithShape", fillFormat.RotateWithObject ? "1" : "0");
            m_writer.WriteAttributeString("dpi", "0");
            m_writer.WriteStartElement("blip", DocxConstants.A_namespace);

            Entity baseEntity = GetBaseEntity(shape);
            string id = string.Empty;
            WPicture pic = new WPicture(m_document);
            if (shape.FillFormat.ImageRecord.ImageBytes != null)
                pic.LoadImage(shape.FillFormat.ImageRecord.ImageBytes);
            UpdateImages(pic);
            if (baseEntity is WSection)
            {
                id = AddImageRelation(DocumentImages, pic.ImageRecord);
            }
            else if (baseEntity is HeaderFooter)
            {
                id = UpdateHFImageRels(baseEntity as HeaderFooter, pic);
            }

            m_writer.WriteAttributeString("embed", DocxConstants.R_namespace, id);
            m_writer.WriteStartElement("alphaModFix", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("amt", (DLSConstants.ThousandthsUnit * (100 - fillFormat.Transparency)).ToString());
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("srcRect", DocxConstants.A_namespace);
            if (fillFormat.SourceRectangle != null)
                SerializeTileRectange(fillFormat.SourceRectangle);
            m_writer.WriteEndElement();

            if (fillFormat.FillRectangle != null && (fillFormat.FillRectangle.BottomOffset != 0 || fillFormat.FillRectangle.LeftOffset != 0
                || fillFormat.FillRectangle.RightOffset != 0 || fillFormat.FillRectangle.TopOffset != 0))
            {
                m_writer.WriteStartElement("stretch", DocxConstants.A_namespace);
                m_writer.WriteStartElement("fillRect", DocxConstants.A_namespace);
                SerializeTileRectange(fillFormat.FillRectangle);
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }

            if (fillFormat.TextureTile)
            {
                m_writer.WriteStartElement("tile", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("tx", (fillFormat.TextureOffsetX * DLSConstants.EmusPerPoint).ToString());
                m_writer.WriteAttributeString("ty", (fillFormat.TextureOffsetY * DLSConstants.EmusPerPoint).ToString());
                m_writer.WriteAttributeString("sx", (fillFormat.TextureHorizontalScale * DLSConstants.ThousandthsUnit).ToString());
                m_writer.WriteAttributeString("sy", (fillFormat.TextureVerticalScale * DLSConstants.ThousandthsUnit).ToString());
                m_writer.WriteAttributeString("flip", GetFlipOrientation(fillFormat.FlipOrientation));
                m_writer.WriteAttributeString("algn", GetBlipAlignment(fillFormat.TextureAlignment));
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
        }

        private void SerializeTileRectange(TileRectangle tileRectangle)
        {
            if (tileRectangle.BottomOffset != 0)
                m_writer.WriteAttributeString("b", (tileRectangle.BottomOffset * DLSConstants.ThousandthsUnit).ToString());
            if (tileRectangle.LeftOffset != 0)
                m_writer.WriteAttributeString("l", (tileRectangle.LeftOffset * DLSConstants.ThousandthsUnit).ToString());
            if (tileRectangle.RightOffset != 0)
                m_writer.WriteAttributeString("r", (tileRectangle.RightOffset * DLSConstants.ThousandthsUnit).ToString());
            if (tileRectangle.TopOffset != 0)
                m_writer.WriteAttributeString("t", (tileRectangle.TopOffset * DLSConstants.ThousandthsUnit).ToString());
        }

        private string GetBlipAlignment(TextureAlignment textureAlignment)
        {
            switch (textureAlignment)
            {
                case TextureAlignment.Bottom:
                    //b (Rectangle Alignment Enum ( Bottom )) Bottom
                    return "b";
                    break;
                //bl (Rectangle Alignment Enum ( Bottom Left )) Bottom Left
                case TextureAlignment.BottomLeft:
                    return "bl";
                    break;
                // br (Rectangle Alignment Enum ( Bottom Right )) Bottom Right
                case TextureAlignment.BottomRight:
                    return "br";
                    break;
                //ctr (Rectangle Alignment Enum ( Center )) Center
                case TextureAlignment.Center:
                    return "ctr";
                    break;
                //l (Rectangle Alignment Enum ( Left )) Left
                case TextureAlignment.Left:
                    return "l";
                    break;
                //r (Rectangle Alignment Enum ( Right )) Right
                case TextureAlignment.Right:
                    return "r";
                    break;
                //t (Rectangle Alignment Enum ( Top )) Top
                case TextureAlignment.Top:
                    return "t";
                    break;
                //tl (Rectangle Alignment Enum ( Top Left )) Top Left
                case TextureAlignment.TopLeft:
                    return "tl";
                    break;
                //tr (Rectangle Alignment Enum ( Top Right )) Top Right
                case TextureAlignment.TopRight:
                    return "tr";
                    break;
            }
            return string.Empty;
        }

        private void SerializePatternFill(FillFormat fillFormat)
        {
            m_writer.WriteStartElement("pattFill", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("prst", GetPatternFillType(fillFormat.Pattern));
            //forecolor
            m_writer.WriteStartElement("fgClr", DocxConstants.A_namespace);

            m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", GetRGBCode(fillFormat.ForeColor));
            m_writer.WriteEndElement();
            
            m_writer.WriteEndElement();
            //backcolor
            m_writer.WriteStartElement("bgClr", DocxConstants.A_namespace);

            m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", GetRGBCode(fillFormat.Color));
            uint alpha = Convert.ToUInt32(65536 * (1 - (fillFormat.Transparency / 100)));
            alpha = (uint)Math.Round((alpha * DLSConstants.HundredthsUnit) / (double)DLSConstants.FixedPointsUnit);
            alpha = alpha * DLSConstants.ThousandthsUnit;
            m_writer.WriteStartElement("alpha", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", alpha.ToString());
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }

        private string GetPatternFillType(PatternType patternType)
        {
            switch(patternType)
            {
                case PatternType.Cross:
                    return "cross";
                case PatternType.DashedDownwardDiagonal:
                    return "dashDnDiag";
                case PatternType.DashedHorizontal:
                    return "dashHorz";
                case PatternType.DashedUpwardDiagonal:
                    return "dashUpDiag";
                case PatternType.DashedVertical:
                    return "dashVert";
                case PatternType.DiagonalBrick:
                    return "diagBrick";//diagBrick (Diagonal Brick)
                case PatternType.DiagonalCross:
                    return "diagCross";//diagCross (Diagonal Cross)
                case PatternType.Divot:
                    return "divot";//divot (Divot)
                case PatternType.DarkDownwardDiagonal:
                    return "dkDnDiag";//dkDnDiag (Dark Downward Diagonal)
                case PatternType.DarkHorizontal:
                    return "dkHorz";//dkHorz (Dark Horizontal)
                case PatternType.DarkUpwardDiagonal:
                    return "dkUpDiag";//dkUpDiag (Dark Upward Diagonal)
                case PatternType.DarkVertical://dkVert (Dark Vertical)
                    return "dkVert";
                case PatternType.DownwardDiagonal:
                    return "dnDiag";//dnDiag (Downward Diagonal)
                case PatternType.DottedDiamond:
                    return "dotDmnd";//dotDmnd (Dotted Diamond)
                case PatternType.DottedGrid:
                    return "dotGrid";//dotGrid (Dotted Grid)
                case PatternType.Horizontal:
                    return "horz";//horz (Horizontal)
                case PatternType.HorizontalBrick://horzBrick (Horizontal Brick)
                    return "horzBrick";
                case PatternType.LargeCheckerBoard://lgCheck (Large Checker Board)
                    return "lgCheck";
                case PatternType.LargeConfetti://lgConfetti (Large Confetti)
                    return "lgConfetti";
                case PatternType.LargeGrid:
                    return "lgGrid";//ltDnDiag (Large Grid)
                case PatternType.LightDownwardDiagonal://ltDnDiag (Light Downward Diagonal)
                    return "ltDnDiag";
                case PatternType.LightHorizontal:
                    return "ltHorz";//ltHorz (Light Horizontal)
                case PatternType.LightUpwardDiagonal://ltUpDiag (Light Upward Diagonal)
                    return "ltUpDiag";
                case PatternType.LightVertical:
                    return "ltVert";//ltVert (Light Vertical)
                case PatternType.NarrowHorizontal:
                    return "narHorz";//narHorz (Narrow Horizontal)
                case PatternType.NarrowVertical:
                    return "narVert";//narVert (Narrow Vertical)
                case PatternType.OutlinedDiamond:
                    return "openDmnd";//openDmnd (Open Diamond)
                case PatternType.Pattern10Percent:
                    return "pct10";//pct10 (10%)
                case PatternType.Pattern20Percent:
                    return "pct20";//pct20 (20%)
                case PatternType.Pattern25Percent:
                    return "pct25";//pct25 (25%)
                case PatternType.Pattern30Percent:
                    return "pct30";//pct30 (30%)
                case PatternType.Pattern40Percent:
                    return "pct40";//pct40 (40%)
                case PatternType.Pattern5Percent:
                    return "pct5";//pct5 (5%)
                case PatternType.Pattern50Percent:
                    return "pct50";//pct50 (50%)
                case PatternType.Pattern60Percent:
                    return "pct60";//pct60 (60%)
                case PatternType.Pattern70Percent:
                    return "pct70";//pct70 (70%)
                case PatternType.Pattern75Percent:
                    return "pct75";//pct75 (75%)
                case PatternType.Pattern80Percent:
                    return "pct80";//pct80 (80%)
                case PatternType.Pattern90Percent:
                    return "pct90";//pct90 (90%)
                case PatternType.Plaid:
                    return "Plaid";//plaid (Plaid)
                case PatternType.Shingle:
                    return "shingle";//shingle (Shingle)
                case PatternType.SmallCheckerBoard:
                    return "smCheck";//smCheck (Small Checker Board)
                case PatternType.SmallConfetti:
                    return "smConfetti";//smConfetti (Small Confetti)
                case PatternType.SmallGrid:
                    return "smGrid";//smGrid (Small Grid)
                case PatternType.SolidDiamond:
                    return "solidDmnd";//solidDmnd (Solid Diamond)
                case PatternType.Sphere:
                    return "sphere";//sphere (Sphere)
                case PatternType.Trellis:
                    return "trellis";//trellis (Trellis)
                case PatternType.UpwardDiagonal:
                    return "upDiag";//upDiag (Upward Diagonal)
                case PatternType.Vertical:
                    return "vert";//vert (Vertical)
                case PatternType.Wave:
                    return "wave";//wave (Wave)
                case PatternType.WideDownwardDiagonal:
                    return "wdDnDiag";//wdDnDiag (Wide Downward Diagonal)
                case PatternType.WideUpwardDiagonal:
                    return "wdUpDiag";//wdUpDiag (Wide Upward Diagonal)
                case PatternType.Weave:
                    return "weave";//weave (Weave)
                case PatternType.ZigZag:
                    return "zigZag";//zigZag (Zig Zag)
                default:
                    return "pct5";
            }
        }

        private void SerializeSolidFill(Color fillColor, float transparency)
        {
            m_writer.WriteStartElement("solidFill", DocxConstants.A_namespace);
            m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", GetRGBCode(fillColor));
            uint alpha = Convert.ToUInt32(65536 * (1 - (transparency / 100)));
            alpha = (uint)Math.Round((alpha * DLSConstants.HundredthsUnit) / (double)DLSConstants.FixedPointsUnit);
            alpha = alpha * DLSConstants.ThousandthsUnit;
            m_writer.WriteStartElement("alpha", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", alpha.ToString());
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }

        private string GetLineStyle(LineStyle lineStyle, bool is2007)
        {
            switch (lineStyle)
            {
                case LineStyle.ThinThin:
                    return is2007 ? "thinThin" : "dbl";
                case LineStyle.ThinThick:
                    return "thinThick";
                case LineStyle.ThickThin:
                    return "thickThin";
                case LineStyle.ThickBetweenThin:
                    return is2007 ? "thickBetweenThin" : "tri";
                default:
                    return is2007 ? "single" : "sng";
            }
        }

        private void SerializeTextBoxContent(Shape shape)
        {
            m_writer.WriteStartElement("txbx", DocxConstants.WPS_namespace);
            m_writer.WriteStartElement("txbxContent", DocxConstants.W_namespace);
            for (int i = 0, count = shape.TextBody.Items.Count; i < count; i++)
            {
                SerializeBodyItem(shape.TextBody.Items[i], false);
            }
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }

        private void SerializeBodyProperties(Shape shape)
        {
            m_writer.WriteStartElement("wps", "bodyPr", DocxConstants.WPS_namespace);

            m_writer.WriteAttributeString("rot", "0");
            m_writer.WriteAttributeString("spcFirstLastPara", "0");
            m_writer.WriteAttributeString("vertOverflow", "overflow");
            m_writer.WriteAttributeString("horzOverflow", "overflow");
            m_writer.WriteAttributeString("wrap", "square");
            float LeftMargin = shape.TextFrame.InternalMargin.Left;
            if (LeftMargin != 0)
                LeftMargin = (float)Math.Round(LeftMargin, 3);

            float RightMargin = shape.TextFrame.InternalMargin.Right;
            if (RightMargin != 0)
                RightMargin = (float)Math.Round(RightMargin, 3);

            float TopMargin = shape.TextFrame.InternalMargin.Top;
            if (TopMargin != 0)
                TopMargin = (float)Math.Round(TopMargin, 3);

            float BottomMargin = shape.TextFrame.InternalMargin.Bottom;
            if (BottomMargin != 0)
                BottomMargin = (float)Math.Round(BottomMargin, 3);


            m_writer.WriteAttributeString("lIns", XmlConvert.ToString(LeftMargin) + "pt");
            m_writer.WriteAttributeString("tIns", XmlConvert.ToString(TopMargin) + "pt");
            m_writer.WriteAttributeString("rIns", XmlConvert.ToString(RightMargin) + "pt");
            m_writer.WriteAttributeString("bIns", XmlConvert.ToString(BottomMargin) + "pt");
            m_writer.WriteAttributeString("idx", "2");
            m_writer.WriteAttributeString("numCol", "1");
            m_writer.WriteAttributeString("spcCol", "0");
            m_writer.WriteAttributeString("rtlCol", "0");
            m_writer.WriteAttributeString("fromWordArt", "0");
            if (shape.TextFrame.TextVerticalAlignment == VerticalAlignment.Bottom)
                m_writer.WriteAttributeString("anchor", "b");
            else if (shape.TextFrame.TextVerticalAlignment == VerticalAlignment.Middle)
                m_writer.WriteAttributeString("anchor", "ctr");
            else
                m_writer.WriteAttributeString("anchor", "t");
            m_writer.WriteAttributeString("anchorCtr", "0");
            m_writer.WriteAttributeString("forceAA", "0");
            m_writer.WriteAttributeString("compatLnSpc", "1");

            if( shape.TextFrame.TextDirection == TextDirection.VerticalTopToBottom)
                m_writer.WriteAttributeString("vert", "vert");
            else if (shape.TextFrame.TextDirection == TextDirection.VerticalBottomToTop)
                m_writer.WriteAttributeString("vert", "vert270");

            m_writer.WriteStartElement("a", "prstTxWarp", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("prst", "textNoShape");
            m_writer.WriteStartElement("avLst", DocxConstants.WPS_namespace);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("noAutofit", DocxConstants.A_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();


        }

        private void SerializeAutoShapeStyles(Shape shape)
        {
            m_writer.WriteStartElement("wps", "style", DocxConstants.WPS_namespace);

            m_writer.WriteStartElement("a", "lnRef", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("idx", "2");
            m_writer.WriteStartElement("a", "schemeClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", "accent1");
            m_writer.WriteStartElement("a", "shade", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", "50000");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("a", "fillRef", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("idx", "1");
            m_writer.WriteStartElement("a", "schemeClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", "accent1");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("a", "effectRef", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("idx", "0");
            m_writer.WriteStartElement("a", "schemeClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", "accent1");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("a", "fontRef", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("idx", "minor");
            m_writer.WriteStartElement("a", "schemeClr", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("val", "lt1");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();


        }

        #region Watermark
        /// <summary>
        /// Serialize the watermark.
        /// </summary>
        /// <param name="watermark">The watermark.</param>
        private void SerializeWatermark(Watermark watermark)
        {
            if (watermark.Type == WatermarkType.TextWatermark)
            {
                SerializeTextWatermark(watermark as TextWatermark);
            }
            else
            {
                SerializePictureWatermark(watermark as PictureWatermark);
            }
        }
        /// <summary>
        /// Serialize the text watremark.
        /// </summary>
        /// <param name="textWatermark">The text watermark.</param>
        private void SerializeTextWatermark(TextWatermark textWatermark)
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);
            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("id", "PowerPlusWaterMarkObject31" + Guid.NewGuid().ToString());
            m_writer.WriteAttributeString("type", "#_x0000_t136");
            m_writer.WriteAttributeString("allowincell", DocxConstants.O_namespace, "f");

            string styleAttrString = SerializeTextStyleAttribute(textWatermark);
            m_writer.WriteAttributeString("style", styleAttrString);

            StringBuilder builder = new StringBuilder();

            m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(textWatermark.Color));

            m_writer.WriteAttributeString("stroked", "f");

            if (textWatermark.Semitransparent)
            {
                m_writer.WriteStartElement("fill", DocxConstants.V_namespace);
                m_writer.WriteAttributeString("opacity", ".5");
                m_writer.WriteEndElement();
            }

            m_writer.WriteStartElement("textpath", DocxConstants.V_namespace);
            builder.Remove(0, builder.Length);
            builder.Append("font-family:" + InvertedCommas);
            builder.Append(textWatermark.FontName.Replace(NullSymbol, string.Empty));
            builder.Append(InvertedCommas);

            if (textWatermark.Size == 36 || textWatermark.Size == 144)
            {
                builder.Append(";font-size:2in");
            }
            else
            {
                builder.Append(";font-size:");
                builder.Append(textWatermark.Size);
                builder.Append("pt");
            }

            m_writer.WriteAttributeString("style", builder.ToString());
            m_writer.WriteAttributeString("string", textWatermark.Text.Replace(NullSymbol, string.Empty));

            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the picture watermark
        /// </summary>
        /// <param name="pictWatermark">The picture watermark.</param>
        private void SerializePictureWatermark(PictureWatermark pictWatermark)
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);
            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("id", "WordPictureWatermark1" + Guid.NewGuid().ToString());
            m_writer.WriteAttributeString("type", "##_x0000_t75");
            m_writer.WriteAttributeString("allowincell", DocxConstants.O_namespace, "f");

            string styleAttrStr = SerializeShapePictStyle(pictWatermark.WordPicture);
            m_writer.WriteAttributeString("style", styleAttrStr);
            m_writer.WriteStartElement("imagedata", DocxConstants.V_namespace);

            if (m_watermarkId == string.Empty)
                m_watermarkId = GetNextRelationShipID();

            m_writer.WriteAttributeString("id", DocxConstants.R_namespace, m_watermarkId);

            if (pictWatermark.Washout)
            {
                m_writer.WriteAttributeString("gain", "19661f");
                m_writer.WriteAttributeString("blacklevel", "22938f");
            }

            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Prepares the text watermark "style" attribute string.
        /// </summary>
        /// <param name="textWatermark">The text watermark.</param>
        private string SerializeTextStyleAttribute(TextWatermark textWatermark)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("position:absolute;margin-left:0;margin-top:0;width:");

            if (textWatermark.ShapeWidthInPixels != -1)
            {
                float shapeWidth = (float)textWatermark.ShapeWidthInPixels / 20;
                strBuilder.Append(XmlConvert.ToString(shapeWidth));
            }
            else
            {
                strBuilder.Append(XmlConvert.ToString(textWatermark.ShapeSize.Width * 0.6934f));
            }

            strBuilder.Append("pt;height:");

            if (textWatermark.ShapeHeightInPixels != -1)
            {
                float shapeHeight = textWatermark.ShapeHeightInPixels / 20;
                strBuilder.Append(XmlConvert.ToString(shapeHeight));
            }
            else
            {
                strBuilder.Append(XmlConvert.ToString(textWatermark.ShapeSize.Height * 0.67f));
            }

            strBuilder.Append("pt;");

            if (textWatermark.Layout == WatermarkLayout.Diagonal)
            {
                strBuilder.Append("rotation:315;");
            }

            strBuilder.Append("mso-position-horizontal:center;mso-position-horizontal-relative:margin;mso-position-vertical:center;mso-position-vertical-relative:margin");

            return strBuilder.ToString();
        }

        #endregion Watermark

        #region Fields
        /// <summary>
        /// Serialize the field
        /// </summary>
        /// <param name="field">The field</param>
        private void SerializeField(WField field)
        {
            if (!IsSupportedField(field.FieldType))
            {
                NonSupportedFields.Push(field);
            }
            else if (field.FieldEnd == null && field.FieldType == FieldType.FieldUnknown)
            {
                WTextRange textRamge = new WTextRange(m_document);
                textRamge.ApplyCharacterFormat(field.CharacterFormat);
                textRamge.Text = field.FieldCode;
                SerializeTextRange(textRamge);
            }
            else
            {
                switch (field.FieldType)
                {
                    case FieldType.FieldHyperlink:
                        HasHyperlink = true;
                        SerializeHyperlink(field);
                        break;
                    case FieldType.FieldFileName:
                        SerializeFileNameField(field);
                        break;
                    case FieldType.FieldAddressBlock:
                    case FieldType.FieldAdvance:
                        SerializeAddressBlockField(field);
                        break;
                    case FieldType.FieldIncludePicture:
                        SerializeIncludePictureField(field);
                        break;
                    default:
                        if (field.FieldType == FieldType.FieldNext && field.Range.Count == 0)
                        {
                            if (!field.ConvertedToText)
                                SerializeNextField(field);
                            break;
                        }
                        InsertFieldBegin();

                        m_writer.WriteStartElement("r", DocxConstants.W_namespace);
                        SerializeCharactetFormat(field.CharacterFormat);
                        if (field.CharacterFormat.IsDeleteRevision)
                            m_writer.WriteStartElement("delInstrText", DocxConstants.W_namespace);
                        else
                            m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);

                        if (field.FieldType != FieldType.FieldIndexEntry && field.FieldType != FieldType.FieldIndex
                            && field.FieldType != FieldType.FieldUnknown && field.FieldType != FieldType.FieldPage
                            && field.FieldType != FieldType.FieldPageRef && field.FieldType != FieldType.FieldMacroButton)
                            m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");

                        if (!string.IsNullOrEmpty(field.FieldCode))
                        {
                            m_writer.WriteString(UpdateFieldCode(field.FieldCode));
                        }
                        else
                        {
                            string ftValue = GetFieldTypeAsString(field.FieldType);

                            //WriteString( ftValue );
                            m_writer.WriteString(ftValue);
                            string tfValue = field.FormattingString;
                            //WriteString( tfValue );
                            m_writer.WriteString(tfValue);

                            if (field.FieldValue == " ")
                            {
                                field.m_fieldValue = InvertedCommas;
                            }
                            else if (field.FieldValue != string.Empty && field.FieldValue.Contains(" ")
                              && field.FieldValue.Length > 1 && !field.FieldValue.StartsWith("_")
                              && !field.FieldValue.Contains(InvertedCommas) && field.FieldType != FieldType.FieldFormula
                              && field.FieldType != FieldType.FieldExpression)
                            {
                                field.m_fieldValue = InvertedCommas + field.FieldValue + InvertedCommas;
                            }

                            SerializeSafeFieldText(field.FieldValue);
                            SerializeSafeFieldText(field.m_formattingString);
                        }
                        m_writer.WriteEndElement();

                        m_writer.WriteEndElement();
                        break;
                }
            }
        }
        /// <summary>
        /// Update the field code text
        /// </summary>
        /// <param name="text">The field code value</param>
        /// <returns></returns>
        private string UpdateFieldCode(string text)
        {
            text = text.Replace(DocxConstants.TOC_SYMBOL, ' ');
            text = text.Replace(DocxConstants.PAGENUMBER_SYMBOL, ' ');

            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        private void SerializeSafeFieldText(string inputString)
        {
            if (inputString == null || inputString == String.Empty)
                return;

            int pos = inputString.IndexOf(DocxConstants.TOC_SYMBOL);
            if (pos == -1)
            {
                //WriteString( inputString );
                m_writer.WriteString(inputString);
            }
            else
            {
                //WriteString( inputString.Substring( 0, pos ) );
                m_writer.WriteString(inputString.Substring(0, pos));
            }
        }
        /// <summary>
        /// Serialize the include picture field
        /// </summary>
        /// <param name="field"></param>
        private void SerializeIncludePictureField(WField field)
        {
            string fieldCode = field.FieldCode;
            fieldCode = fieldCode.Trim();
            string[] words = fieldCode.Split(new char[] { '"' });
            string url = words[1];

            if (string.IsNullOrEmpty(url))
                return;

            InsertFieldBegin();

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
            m_writer.WriteString("INCLUDEPICTURE \"" + url + "\"");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
#if !SILVERLIGHT && !WP
            InsertFieldSeparator();

            Image image = DownloadImage(url);


            if (image == null)
            {
                InsertFieldEnd();
                return;
            }


            WPicture pic = new WPicture(field.Document);
            pic.LoadImage(image);
            pic.SetOwner(field.Owner);

            string imageId = UpdateShapeId(pic, false, null);
            string urlId = UpdateInclPicFieldUrl(url, field);
            ParagraphItem fieldContent = GetFieldContent(field);
            if (fieldContent is WPicture)
            {
                pic.Width = ((fieldContent as WPicture).Width * (fieldContent as WPicture).WidthScale) / 100;
                pic.Height = ((fieldContent as WPicture).Height * (fieldContent as WPicture).HeightScale) / 100;
            }
            string pictureStyle = "width:" + pic.Width.ToString(CultureInfo.InvariantCulture) + "pt;height:" + pic.Height.ToString(CultureInfo.InvariantCulture) + "pt";
            pictureStyle = pictureStyle.Replace(",", ".");

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);
            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("style", pictureStyle);
            m_writer.WriteStartElement("imagedata", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.R_namespace, imageId);
            m_writer.WriteAttributeString("href", DocxConstants.R_namespace, urlId);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            UpdateInclPicField(field);
#endif
            InsertFieldEnd();
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Get the image from the url
        /// </summary>
        /// <param name="url">Th url</param>
        /// <returns></returns>
        public Image DownloadImage(string url)
        {
            Image image = null;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.AllowWriteStreamBuffering = true;

                WebResponse webResponse = httpWebRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                image = Image.FromStream(webStream);

                webResponse.Close();
            }
            catch
            { }

            return image;
        }
#endif
        /// <summary>
        /// Update owner paragraph for IncludePictureField
        /// </summary>
        /// <param name="field">Include Picure Field</param>
        private void UpdateInclPicField(WField field)
        {
            ParagraphItem nextFirstItem = GetNextSibling(field as ParagraphItem);
            ParagraphItem nextThirdItem = null;
            if (nextFirstItem != null && nextFirstItem is WFieldMark
                && (nextFirstItem as WFieldMark).Type == FieldMarkType.FieldSeparator)
            {
                ParagraphItem nextSecondItem = GetNextSibling(nextFirstItem);
                if (nextSecondItem != null && nextSecondItem is WPicture)
                {
                    nextThirdItem = GetNextSibling(nextSecondItem);
                    if (nextThirdItem != null && nextThirdItem is WFieldMark
                        && (nextThirdItem as WFieldMark).Type == FieldMarkType.FieldEnd)
                    {
                        nextFirstItem.SkipDocxItem = true;
                        nextSecondItem.SkipDocxItem = true;
                        nextThirdItem.SkipDocxItem = true;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the content of the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>ParagraphItem.</returns>
        private ParagraphItem GetFieldContent(WField field)
        {
            ParagraphItem nextFirstItem = GetNextSibling(field as ParagraphItem);
            nextFirstItem = field.NextSibling as ParagraphItem;
            ParagraphItem nextSecondItem = nextFirstItem.NextSibling as ParagraphItem;
            return nextSecondItem;
        }
        /// <summary>
        /// Update the include picture field url 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private string UpdateInclPicFieldUrl(string url, WField field)
        {
            string urlId = GetNextRelationShipID();
            if (field.OwnerParagraph.OwnerTextBody is HeaderFooter)
            {
                string hfId = string.Empty;
                Dictionary<string, string> hfInclPicUrls;

                foreach (HeaderFooterType hfType in m_headerFooterColl.Keys)
                {
                    Dictionary<string, HeaderFooter> hfColl = m_headerFooterColl[hfType];

                    foreach (string key in hfColl.Keys)
                    {
                        if (hfColl[key] == (field.OwnerParagraph.OwnerTextBody as HeaderFooter))
                            hfId = key;
                    }
                }
                if (HeaderFooterInclPicUrls.ContainsKey(hfId))
                {
                    hfInclPicUrls = m_headerFooterInclPicUrls[hfId];
                    hfInclPicUrls.Add(urlId, url);
                }
                else
                {
                    hfInclPicUrls = new Dictionary<string, string>();
                    hfInclPicUrls.Add(urlId, url);
                    HeaderFooterInclPicUrls.Add(hfId, hfInclPicUrls);
                }
            }
            else
            {
                InclPicFieldUrl.Add(urlId, url);
            }
            return urlId;
        }
        /// <summary>
        /// Get the next sibling
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ParagraphItem GetNextSibling(ParagraphItem item)
        {
            if (item.NextSibling != null)
            {
                return item.NextSibling as ParagraphItem;
            }
            else
            {
                WParagraph ownerPara = item.OwnerParagraph;
                int index = ownerPara.GetIndexInOwnerCollection();
                while (index < ownerPara.OwnerTextBody.Items.Count)
                {
                    TextBodyItem textBodyItem = ownerPara.OwnerTextBody.Items[index++];
                    if (textBodyItem is WParagraph)
                    {
                        if ((textBodyItem as WParagraph).Items.Count > 0)
                            return (textBodyItem as WParagraph).Items[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Serialize the address block field
        /// </summary>
        /// <param name="field">The field</param>
        private void SerializeAddressBlockField(WField field)
        {
            string fieldCodePrefix = (field.FieldType == FieldType.FieldAddressBlock) ? " ADDRESSBLOCK " : " ADVANCE ";

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("fldCharType", DocxConstants.W_namespace, "begin");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
            m_writer.WriteString(fieldCodePrefix + field.FieldValue + field.FormattingString);
            m_writer.WriteWhitespace(" ");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the filename field
        /// </summary>
        /// <param name="field"></param>
        private void SerializeFileNameField(WField field)
        {
            m_writer.WriteStartElement("fldSimple", DocxConstants.W_namespace);
            string fieldCode = string.IsNullOrEmpty(field.FieldCode) ? FieldTypeDefiner.GetFieldCode(field.FieldType) : field.FieldCode;
            m_writer.WriteAttributeString("instr", DocxConstants.W_namespace, fieldCode);
            WTextRange textRange = field.NextSibling.NextSibling as WTextRange;
            if (textRange != null && (textRange.IsInsertRevision || textRange.IsDeleteRevision))
                SerializeTrackChange(textRange);
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            if (textRange != null)
            {
                SerializeCharactetFormat(textRange.CharacterFormat);
                SerializeText(textRange.Text, textRange.CharacterFormat.IsDeleteRevision);
            }
            if (textRange != null && (textRange.IsInsertRevision || textRange.IsDeleteRevision))
                m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the next field
        /// </summary>
        /// <param name="nextField"></param>
        private void SerializeNextField(WField nextField)
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("fldCharType", DocxConstants.W_namespace, "begin");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
            m_writer.WriteString(" NEXT " + nextField.FieldValue + nextField.FormattingString);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            InsertFieldSeparator();

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("t", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("space", DocxConstants.Xml_namespace, "preserve");
            m_writer.WriteString(nextField.Text);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();

            InsertFieldEnd();
        }
        /// <summary>
        /// Insert the field mark separator
        /// </summary>
        private void InsertFieldSeparator()
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("fldCharType", DocxConstants.W_namespace, "separate");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Insert the field mark end
        /// </summary>
        private void InsertFieldEnd()
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("fldCharType", DocxConstants.W_namespace, "end");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Inser the field mark begin
        /// </summary>
        private void InsertFieldBegin()
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            if (CurrentField.CharacterFormat != null)
                SerializeCharactetFormat(CurrentField.CharacterFormat);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("fldCharType", DocxConstants.W_namespace, "begin");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the hyperlink
        /// </summary>
        /// <param name="field">The hyperlink field</param>
        private void SerializeHyperlink(WField field)
        {
            if (!CheckHyperlink(field))
                return;
            if (field.FieldCode == field.NestedFieldCode && CheckHyperlink(field) && field.FieldCode == string.Empty)
            {
                //Serializes the simple hyperlink field.
                m_writer.WriteStartElement("hyperlink", DocxConstants.W_namespace);
                SerializeHyperlinkAttributes(field);
                m_writer.WriteAttributeString("history", DocxConstants.W_namespace, "1");
            }
            else
            {
                //Serializes the nested hyperlink field.
                InsertFieldBegin();
                m_writer.WriteStartElement("r", DocxConstants.W_namespace);
                SerializeCharactetFormat(field.CharacterFormat);
                if (field.CharacterFormat.IsDeleteRevision)
                    m_writer.WriteStartElement("delInstrText", DocxConstants.W_namespace);
                else
                    m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
                m_writer.WriteString(UpdateFieldCode(field.FieldCode));
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the hyperlink attributes
        /// </summary>
        /// <param name="field"></param>
        private void SerializeHyperlinkAttributes(WField field)
        {
            string relationId = GetNextRelationShipID();
            string targetString = field.FieldValue.Replace(InvertedCommas, string.Empty);
            string target = targetString;
            Entity entity = field.OwnerParagraph.OwnerTextBody.Owner;

            targetString = targetString.Replace(" ", "%20");

            bool isTOCLink = targetString.StartsWith("_Toc");

            if (targetString.IndexOf(@"http") == -1 && targetString.IndexOf(@"www") == -1
              && targetString.IndexOf(@"mailto") == -1 && targetString.IndexOf(@"javascript") == -1
              && targetString.IndexOf(@"ftp:") == -1)
            {
                targetString = targetString.Replace(SlashSymbol + SlashSymbol, "/");

                if (targetString.Length > 2 && targetString.Substring(0, 3) == "../")
                {
                    targetString = targetString.Remove(0, 3);
                }
                else if (field.IsLocal || field.FormattingString.IndexOf(SlashSymbol + SlashSymbol + "l") != -1)
                {
                    targetString = targetString.Insert(0, "#");
                }
            }

            if (GetBaseEntity(field.OwnerParagraph) is HeaderFooter || m_IsAutoshapeTextboxInHeader)
            {
                string hfId = string.Empty;
                Dictionary<string, string> hfHyperlinks;

                foreach (HeaderFooterType hfType in m_headerFooterColl.Keys)
                {
                    Dictionary<string, HeaderFooter> hfColl = m_headerFooterColl[hfType];

                    foreach (string key in hfColl.Keys)
                    {
                        if (hfColl[key] == (GetBaseEntity(field) as HeaderFooter) || ((hfColl[key] as HeaderFooter).Type == m_HeaderFooterType && m_IsAutoshapeTextboxInHeader))
                            hfId = key;
                    }
                }

                if (HeaderFooterHyperlinks.ContainsKey(hfId))
                {
                    hfHyperlinks = m_headerFooterHyperlinks[hfId];
                    hfHyperlinks.Add(relationId, targetString);
                }
                else
                {
                    hfHyperlinks = new Dictionary<string, string>();
                    hfHyperlinks.Add(relationId, targetString);
                    HeaderFooterHyperlinks.Add(hfId, hfHyperlinks);
                }
            }
            else if (!isTOCLink)
            {
                if (entity is WSection || entity is WTextBox || entity is WTableRow)
                {
                    HyperlinkTargets.Add(relationId, targetString);
                }
                else if (entity is WFootnote)
                {
                    if ((entity as WFootnote).FootnoteType == FootnoteType.Footnote)
                        FootnoteHyperlinks.Add(relationId, targetString);
                    else
                        EndnoteHyperlinks.Add(relationId, targetString);
                }

                if (entity is WComment)
                {
                    CommentHyperlinks.Add(relationId, targetString);
                }
            }

            if (isTOCLink)
            {
                m_writer.WriteAttributeString("anchor", DocxConstants.W_namespace, field.FieldValue.Replace(InvertedCommas, string.Empty));
            }
            else
            {
                m_writer.WriteAttributeString("r", "id", DocxConstants.R_namespace, relationId);
            }
            WriteLocalReference(field);
        }
        /// <summary>
        /// Writes the Local reference attribute
        /// </summary>
        /// <param name="field"></param>
        private void WriteLocalReference(WField field)
        {
            if (field.IsLocal && field.LocalReference != null && field.LocalReference != string.Empty)
                m_writer.WriteAttributeString("anchor", DocxConstants.W_namespace, field.LocalReference.Replace(InvertedCommas, string.Empty));
        }
        /// <summary>
        /// <summary>
        /// Check whether the hyperlink is Picture Hyperlink and Text Hyperlink
        /// </summary>
        /// <param name="field">The hyperlink field</param>
        /// <returns></returns>
        private bool CheckHyperlink(WField field)
        {
            if (field.FieldType != FieldType.FieldHyperlink)
                return false;

            IEntity ent = field;
            for (int i = 0; i < 2; i++)
            {
                ent = ent.NextSibling;
                if (ent == null)
                    break;
            }
            if (ent is WPicture
                && (ent.NextSibling is WFieldMark)
                && (ent.NextSibling as WFieldMark).Type == FieldMarkType.FieldEnd
                && !(ent as WPicture).IsShape)
                return false;

            return true;
        }
        /// <summary>
        /// Returns whether the field is supported or not.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        private bool IsSupportedField(FieldType fieldType)
        {
            if (fieldType != FieldType.FieldDatabase
                && fieldType != FieldType.FieldSkipIf
                && fieldType != FieldType.FieldShape)
            {
                return true;
            }
            return false;
        }

        #endregion Fields

        #region Footnote/Endnote
        /// <summary>
        /// Serialize the footnote and endnote.
        /// </summary>
        /// <param name="footnote"></param>
        private void SerializeFootEndnote(WFootnote footnote)
        {
            footnote.EnsureFtnMarker();
            bool isEndnote = (footnote.FootnoteType == FootnoteType.Endnote) ? true : false;
            int id = isEndnote ? m_endnoteId++ : m_footnoteId++;
            if (isEndnote)
            {
                HasEndnote = true;
                IsSectionContainsEndnotes = true;
                EndnoteCollection.Add(id, footnote);
            }
            else
            {
                HasFootnote = true;
                IsSectionContainsFootnotes = true;
                FootnoteCollection.Add(id, footnote);
            }

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);

            SerializeCharactetFormat(footnote.MarkerCharacterFormat);

            string refStr = (isEndnote) ? "endnoteReference" : "footnoteReference";
            m_writer.WriteStartElement(refStr, DocxConstants.W_namespace);

            string follow = (footnote.IsAutoNumbered) ? "0" : "1";
            m_writer.WriteAttributeString("customMarkFollows", DocxConstants.W_namespace, follow);
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, id.ToString());
            m_writer.WriteEndElement();

            if (!footnote.IsAutoNumbered)
            {
                if (footnote.SymbolCode != 0)
                {
                    m_writer.WriteStartElement("sym", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("char", DocxConstants.W_namespace, "F0" + System.Convert.ToString(footnote.SymbolCode, 16).ToUpper());
                    m_writer.WriteAttributeString("font", DocxConstants.W_namespace, footnote.SymbolFontName);
                    m_writer.WriteEndElement();
                }
                else if (footnote.CustomMarker != string.Empty && footnote.CustomMarker != "(")
                {
                    m_writer.WriteElementString("t", DocxConstants.W_namespace, footnote.CustomMarker);
                }
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the endnotes/footnotes
        /// </summary>
        /// <param name="IsEndnotes"></param>
        private void SerializeFootEndnotes(bool IsEndnotes)
        {
            MemoryStream footEndNoteStream = new MemoryStream();
            string collTagStr = string.Empty;
            string elemTagStr = string.Empty;

            m_writer = CreateWriter(footEndNoteStream);

            Dictionary<int, WFootnote> NotesCollection = null;

            if (IsEndnotes)
            {
                if (m_endnoteColl != null)
                {
                    NotesCollection = m_endnoteColl;
                }
            }
            else if (m_footnoteColl != null)
            {
                NotesCollection = m_footnoteColl;
            }

            if (NotesCollection != null)
            {
                if (IsEndnotes)
                {
                    collTagStr = "endnotes";
                    elemTagStr = "endnote";
                }
                else
                {
                    collTagStr = "footnotes";
                    elemTagStr = "footnote";
                }

                SerializeFootEndnotesStartElement(collTagStr, elemTagStr);

                foreach (int key in NotesCollection.Keys)
                {
                    SerializeFootEndnoteElement(NotesCollection[key], IsEndnotes, key);
                }
            }

            m_writer.WriteEndElement();
            m_writer.Flush();
            if (IsEndnotes)
                m_archive.AddItem(DocxConstants.EndnotesPath, footEndNoteStream, false, FileAttributes.Archive);
            else
                m_archive.AddItem(DocxConstants.FootnotesPath, footEndNoteStream, false, FileAttributes.Archive);
        }

        /// <summary>
        /// Serialize start foot/endnotes elements.
        /// </summary>
        /// <param name="collTagStr"></param>
        /// <param name="elemTagStr"></param>
        private void SerializeFootEndnotesStartElement(string collTagStr, string elemTagStr)
        {
            m_writer.WriteStartElement("w", collTagStr, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "ve", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "a", null, DocxConstants.A_namespace);
            m_writer.WriteAttributeString("xmlns", "pic", null, DocxConstants.PIC_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);

            m_writer.WriteStartElement("w", elemTagStr, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "separator");
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, "-1");
            WTextBody textBody = null;
            if (elemTagStr == "footnote")
                textBody = m_document.Footnotes.Separator;
            else
                textBody = m_document.Endnotes.Separator;
            SerializeBodyItems(textBody.Items, true);
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("w", elemTagStr, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "continuationSeparator");
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, "0");
            if (elemTagStr == "footnote")
                textBody = m_document.Footnotes.ContinuationSeparator;
            else
                textBody = m_document.Endnotes.ContinuationSeparator;
            SerializeBodyItems(textBody.Items, true);
            m_writer.WriteEndElement();
            if (elemTagStr == "footnote")
                textBody = m_document.Footnotes.ContinuationNotice;
            else
                textBody = m_document.Endnotes.ContinuationNotice;
            if (textBody.ChildEntities.Count > 0)
            {
                m_writer.WriteStartElement("w", elemTagStr, DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "continuationNotice");
                m_writer.WriteAttributeString("id", DocxConstants.W_namespace, "1");
                SerializeBodyItems(textBody.Items, true);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize footnote/endnote.
        /// </summary>
        /// <param name="footnote">The footnote.</param>
        /// <param name="isEndNote">if it is end note, set to <c>true</c>.</param>
        /// <param name="id">The id.</param>
        private void SerializeFootEndnoteElement(WFootnote footnote, bool isEndNote, int id)
        {
            if (isEndNote)
            {
                m_writer.WriteStartElement("endnote", DocxConstants.W_namespace);
            }
            else
            {
                m_writer.WriteStartElement("footnote", DocxConstants.W_namespace);
            }

            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, id.ToString());

            foreach (TextBodyItem item in footnote.TextBody.Items)
            {
                //Build body items
                SerializeBodyItem(item, false);
            }
            m_writer.WriteEndElement();
        }
        #endregion Footnote/Endnote

        #region SeqField
        /// <summary>
        /// Serialize the seq field.
        /// </summary>
        /// <param name="seqField">The Seq field</param>
        private void SerializeSeqField(WSeqField seqField)
        {
            //<w:r>
            //    <w:fldChar w:fldCharType="begin" />
            //</w:r>
            //<w:r>
            //    <w:instrText xml:space="preserve"> SEQ  Chapter \* Upper  \* ARABIC </w:instrText>
            //</w:r>
            m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("w", "fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "fldCharType", DocxConstants.W_namespace, "begin");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
            SerializeSeqFieldCode(seqField);
            m_writer.WriteEndElement();

        }

        /// <summary>
        /// Serialize the seq field code.
        /// </summary>
        /// <param name="seqField"></param>
        private void SerializeSeqFieldCode(WSeqField seqField)
        {
            StringBuilder instrCode = new StringBuilder();
            instrCode.Append(" SEQ ");
            instrCode.Append(seqField.CaptionName + " ");
            instrCode.Append(seqField.FormattingString + " ");
            m_writer.WriteStartElement("w", "instrText", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xml", "space", null, "preserve");
            m_writer.WriteString(instrCode.ToString());
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Get the field text format.
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        private string GetFieldTextFormat(TextFormat tf)
        {
            switch (tf)
            {
                case TextFormat.Uppercase:
                    return SlashSymbol + "* Upper";
                case TextFormat.Lowercase:
                    return SlashSymbol + "* Lower";
                case TextFormat.FirstCapital:
                    return SlashSymbol + "* FirstCap";
                case TextFormat.Titlecase:
                    return SlashSymbol + "* Caps";
                default:
                    return String.Empty;
            }
        }
        #endregion SeqField

        #region Form elements

        #region DropDownFormField
        /// <summary>
        /// Serialize the drop down field.
        /// </summary>
        /// <param name="dropDownFormField"></param>
        private void SerializeDropDownFormField(WDropDownFormField dropDownFormField)
        {
            FieldStack.Push(dropDownFormField);
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(dropDownFormField.CharacterFormat);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "fldCharType", DocxConstants.W_namespace, "begin");

            if (dropDownFormField.HasFFData)
            {
                m_writer.WriteStartElement("ffData", DocxConstants.W_namespace);
                SerializeFormFieldData(dropDownFormField as WFormField);

                m_writer.WriteStartElement("ddList", DocxConstants.W_namespace);

                m_writer.WriteStartElement("default", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, dropDownFormField.DefaultDropDownValue.ToString());
                m_writer.WriteEndElement();

                if (dropDownFormField.DropDownSelectedIndex > 0)
                {
                    m_writer.WriteStartElement("result", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, dropDownFormField.DropDownSelectedIndex.ToString());
                    m_writer.WriteEndElement();
                }

                foreach (WDropDownItem item in dropDownFormField.DropDownItems)
                {
                    m_writer.WriteStartElement("listEntry", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, item.Text);
                    m_writer.WriteEndElement();
                }

                m_writer.WriteEndElement();//end of ddList tag
                m_writer.WriteEndElement();//end of ffData
            }
            m_writer.WriteEndElement();//end of fldCharType - Begin
            m_writer.WriteEndElement();//end of run tag
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(dropDownFormField.CharacterFormat);
            m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
            if (dropDownFormField.FieldCode != null && dropDownFormField.FieldCode.Trim() != string.Empty && !dropDownFormField.FieldCode.Trim().ToUpper().Equals("FORMDROPDOWN"))
                m_writer.WriteString(dropDownFormField.FieldCode);
            else
                m_writer.WriteString("FORMDROPDOWN");
            m_writer.WriteEndElement();//end of instrText
            m_writer.WriteEndElement();//end of run tag
        }

        #endregion DropDownFormField

        #region CheckBox
        /// <summary>
        /// Serialize the check box field.
        /// </summary>
        /// <param name="checkBox"></param>
        private void SerializeCheckBoxField(WCheckBox checkBox)
        {
            FieldStack.Push(checkBox);
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(checkBox.CharacterFormat);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "fldCharType", DocxConstants.W_namespace, "begin");
            if (checkBox.HasFFData)
            {
                m_writer.WriteStartElement("ffData", DocxConstants.W_namespace);
                SerializeFormFieldData(checkBox as WFormField);
                m_writer.WriteEndElement();//end of ffData
            }
            m_writer.WriteEndElement();//end of fldCharType
            m_writer.WriteEndElement();//end of run tag
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(checkBox.CharacterFormat);
            m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
            if (checkBox.FieldCode != null && checkBox.FieldCode.Trim() != string.Empty && !checkBox.FieldCode.Trim().ToUpper().Equals("FORMCHECKBOX"))
                m_writer.WriteString(checkBox.FieldCode);
            else
                m_writer.WriteString("FORMCHECKBOX");
            m_writer.WriteEndElement();//instrText
            m_writer.WriteEndElement();//end of run tag
        }
        #endregion Checkbox

        #region TextFormField
        /// <summary>
        /// Serialize the text form field.
        /// </summary>
        /// <param name="textFormField">The textform field.</param>
        private void SerializeTextFormField(WTextFormField textFormField)
        {
            FieldStack.Push(textFormField);
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(textFormField.CharacterFormat);
            m_writer.WriteStartElement("fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "fldCharType", DocxConstants.W_namespace, "begin");
            if (textFormField.HasFFData)
            {
                m_writer.WriteStartElement("ffData", DocxConstants.W_namespace);
                SerializeFormFieldData(textFormField as WFormField);
                m_writer.WriteEndElement();//end of ffData
            }
            m_writer.WriteEndElement();//end of fldCharType
            m_writer.WriteEndElement();//end of run tag
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(textFormField.CharacterFormat);
            if (textFormField.CharacterFormat.IsDeleteRevision)
                //Writes delInstrText while deleted track changes
                m_writer.WriteStartElement("delInstrText", DocxConstants.W_namespace);
            else
                //Writes instrText while inserted track changes
                m_writer.WriteStartElement("instrText", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
            if (textFormField.FieldCode != null && textFormField.FieldCode.Trim() != string.Empty && !textFormField.FieldCode.Trim().ToUpper().Equals("FORMTEXT"))
                m_writer.WriteString(textFormField.FieldCode);
            else
                m_writer.WriteString(" FORMTEXT ");
            m_writer.WriteEndElement();//end of instrText
            m_writer.WriteEndElement();//end of run tag
        }
        #endregion TextFormField
        /// <summary>
        /// Serialize the form field data
        /// </summary>
        /// <param name="field"></param>
        private void SerializeFormFieldData(WFormField field)
        {
            if (field.MacroOnStart != null && field.MacroOnStart.Length > 0)
            {
                m_writer.WriteStartElement("entryMacro", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, field.MacroOnStart);
                m_writer.WriteEndElement();
            }

            if (field.MacroOnEnd != null && field.MacroOnEnd.Length > 0)
            {
                m_writer.WriteStartElement("exitMacro", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, field.MacroOnEnd);
                m_writer.WriteEndElement();
            }

            if (field.Help != "")
            {
                m_writer.WriteStartElement("helpText", DocxConstants.W_namespace);
                if ((field.Params & 0x80) == 128)
                {
                    m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "text");
                }
                else
                {
                    m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "autoText");
                }

                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, field.Help);
                m_writer.WriteEndElement();
            }

            if (field.StatusBarHelp != "")
            {
                m_writer.WriteStartElement("statusText", DocxConstants.W_namespace);

                if ((field.Params & 0x100) == 256)
                {
                    m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "text");
                }
                else
                {
                    m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "autoText");
                }

                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, field.StatusBarHelp);
                m_writer.WriteEndElement();
            }

            if (!field.Enabled)
            {
                m_writer.WriteStartElement("enabled", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }

            if (field.CalculateOnExit)
            {
                m_writer.WriteStartElement("calcOnExit", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }

            if (field.Name != "")
            {
                m_writer.WriteStartElement("name", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, field.Name);
                m_writer.WriteEndElement();
            }

            switch (field.FieldType)
            {
                case FieldType.FieldFormTextInput:
                    WTextFormField input = field as WTextFormField;
                    m_writer.WriteStartElement("textInput", DocxConstants.W_namespace);

                    if (input.MaximumLength > 0)
                    {
                        m_writer.WriteStartElement("maxLength", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, input.MaximumLength.ToString());
                        m_writer.WriteEndElement();
                    }

                    switch (input.Type)
                    {
                        case TextFormFieldType.RegularText:
                            m_writer.WriteStartElement("type", DocxConstants.W_namespace);
                            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "regular");
                            m_writer.WriteEndElement();
                            break;
                        case TextFormFieldType.NumberText:
                            m_writer.WriteStartElement("type", DocxConstants.W_namespace);
                            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "number");
                            m_writer.WriteEndElement();
                            break;
                        case TextFormFieldType.DateText:
                            m_writer.WriteStartElement("type", DocxConstants.W_namespace);
                            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "date");
                            m_writer.WriteEndElement();
                            break;
                        //Handled for TextFormField type Calculation
                        case TextFormFieldType.Calculation:
                            m_writer.WriteStartElement("type", DocxConstants.W_namespace);
                            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "calculated");
                            m_writer.WriteEndElement();
                            break;
                        default:
                            break;
                    }

                    m_writer.WriteStartElement("default", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, input.DefaultText);
                    m_writer.WriteEndElement();

                    m_writer.WriteStartElement("format", DocxConstants.W_namespace);
                    if (input.Type == TextFormFieldType.RegularText)
                    {
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, input.TextFormat.ToString());
                    }
                    else
                    {
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, input.StringFormat);
                    }
                    m_writer.WriteEndElement();


                    m_writer.WriteEndElement();
                    break;
                case FieldType.FieldFormCheckBox:
                    WCheckBox checkbox = field as WCheckBox;
                    m_writer.WriteStartElement("checkBox", DocxConstants.W_namespace);

                    if (checkbox.SizeType == CheckBoxSizeType.Auto)
                    {
                        m_writer.WriteStartElement("sizeAuto", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                    }
                    else
                    {
                        m_writer.WriteStartElement("size", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, (checkbox.CheckBoxSize * 2).ToString());
                        m_writer.WriteEndElement();
                    }
                    //if (checkbox.DefaultCheckBoxValue)
                    //{
                    m_writer.WriteStartElement("default", DocxConstants.W_namespace);
                    string defCheckBoxVal = (checkbox.DefaultCheckBoxValue) ? "true" : "false";
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, defCheckBoxVal);
                    //}
                    m_writer.WriteEndElement();

                    if (checkbox.Checked)
                    {
                        m_writer.WriteStartElement("checked", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                    }
                    else if (checkbox.DefaultCheckBoxValue && !checkbox.Checked)
                    {
                        m_writer.WriteStartElement("checked", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "0");
                        m_writer.WriteEndElement();
                    }



                    m_writer.WriteEndElement();
                    break;
            }
        }

        #endregion Form elements

        #region Textbox
        /// <summary>
        /// Serialize the textboxes
        /// </summary>
        /// <param name="textboxes">The textbox collection</param>
        private void SerializeTextboxes(WTextBoxCollection textboxes)
        {
            foreach (WTextBox textbox in textboxes)
            {
                SerializeTextBox(textbox);
            }
        }
        /// <summary>
        /// Serialize the textbox
        /// </summary>
        /// <param name="textBox">The textbox</param>
        private void SerializeTextBox(WTextBox textBox)
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(textBox.CharacterFormat);

            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);

            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("type", "#_x0000_t202");
            SerializeTextBoxFormat(textBox.TextBoxFormat);
            // Build textbox fill effects
            SerializeFillEffects(textBox);
            m_writer.WriteStartElement("textbox", DocxConstants.V_namespace);

            StringBuilder textBoxStyle = new StringBuilder();
            if (textBox.TextBoxFormat.FitTextToShape)
                textBoxStyle.Append("mso-fit-shape-to-text:t");
            //Build Text Box Text Direction
            if (textBox.TextBoxFormat.TextDirection == TextDirection.VerticalTopToBottom)
                textBoxStyle.Append("layout-flow:vertical");
            else if (textBox.TextBoxFormat.TextDirection == TextDirection.VerticalBottomToTop)
                textBoxStyle.Append("layout-flow:vertical;mso-layout-flow-alt:bottom-to-top");
            m_writer.WriteAttributeString("style", textBoxStyle.ToString());
            if (!textBox.TextBoxFormat.FitTextToShape)
                SerializeInsetAttribute(textBox);

            m_writer.WriteStartElement("txbxContent", DocxConstants.W_namespace);

            for (int i = 0, count = textBox.TextBoxBody.Items.Count; i < count; i++)
            {
                //if (i == count - 1 && textBox.TextBoxBody.Items[i] is WParagraph &&
                //  ((textBox.TextBoxBody.Items[i]) as WParagraph).Items.Count == 0)
                //    continue;

                SerializeBodyItem(textBox.TextBoxBody.Items[i], false);
            }

            m_writer.WriteEndElement();//end of txbxContent tag
            m_writer.WriteEndElement();//end of textbox tag
            m_writer.WriteEndElement();//end of shape tag
            m_writer.WriteEndElement();//end of pict tag
            m_writer.WriteEndElement();//end of run tag
        }
        /// <summary>
        /// Serialize the fill effects
        /// </summary>
        /// <param name="textBox"></param>
        private void SerializeFillEffects(WTextBox textBox)
        {
            Background background = textBox.TextBoxFormat.FillEfects;

            if (background.PatternFill != null)
            {
                bool isHeaderFooter = false;
                if (textBox.OwnerParagraph != null)
                    isHeaderFooter = (textBox.OwnerParagraph.Owner is HeaderFooter) ? true : false;
                SerializePatternFill(textBox, isHeaderFooter);
                return;
            }

            if (background.Type == BackgroundType.NoBackground || background.Type == BackgroundType.Color)
                return;

            m_writer.WriteStartElement("fill", DocxConstants.V_namespace);

            if ((background.Type == BackgroundType.Picture || background.Type == BackgroundType.Texture)
              && background.ImageBytes != null)
            {
                SerializePictureFill(textBox);
            }
            else if (background.Type == BackgroundType.Gradient)
            {
                SerializeGradientFill(textBox.TextBoxFormat.FillEfects.Gradient);
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the gradient effects.
        /// </summary>
        /// <param name="backgroundGradient"></param>
        private void SerializeGradientFill(BackgroundGradient backgroundGradient)
        {
            SerializeGradientColor(backgroundGradient);
            SerializeGradientShadings(backgroundGradient);
        }
        /// <summary>
        /// Serialize the fill effects.
        /// </summary>
        /// <param name="textBox"></param>
        private void SerializePictureFill(WTextBox textBox)
        {
            Entity baseEntity = GetBaseEntity(textBox);
            Background background = textBox.TextBoxFormat.FillEfects;
            string id = string.Empty;

            WPicture pic = new WPicture(textBox.Document);
            if (background.ImageBytes != null)
                pic.LoadImage(background.ImageBytes);
            UpdateImages(pic);

            if (baseEntity is WSection)
            {
                id = AddImageRelation(DocumentImages, pic.ImageRecord);
            }
            else if (baseEntity is HeaderFooter)
            {
                id = UpdateHFImageRels(baseEntity as HeaderFooter, pic);
            }

            m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id);

            if (background.Type == BackgroundType.Picture)
                m_writer.WriteAttributeString("type", "frame");
            else
                m_writer.WriteAttributeString("type", "tile");
        }
        /// <summary>
        /// Serialize the pattern fill.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="isHeaderFooter"></param>
        private void SerializePatternFill(WTextBox textBox, bool isHeaderFooter)
        {
            Background background = textBox.TextBoxFormat.FillEfects;
            string id = string.Empty;
            if (background.PatternImageBytes != null)
            {
                WPicture pic = new WPicture(m_document);
                pic.LoadImage(background.PatternImageBytes);
                if (isHeaderFooter)
                {
                    HeaderFooter headFoot = textBox.OwnerParagraph.Owner as HeaderFooter;
                    id = UpdateHFImageRels(headFoot, pic);
                }
                else
                {
                    id = AddImageRelation(DocumentImages, pic.ImageRecord);
                }
                m_hasImages = true;
                Stream outputStream = SetIDAttribute(background.PatternFill, id);
                XmlReader reader = CreateReader(outputStream);
                m_writer.WriteNode(reader, false);
            }
            else
            {
                XmlReader reader = CreateReader(background.PatternFill);
                m_writer.WriteNode(reader, false);
            }
        }
        /// <summary>
        /// Set Relationship ID for Stream data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationId"></param>
        private Stream SetIDAttribute(Stream inputStream, string relationId)
        {
            inputStream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(inputStream);

            MemoryStream outputStream = new MemoryStream((int)inputStream.Length);
            XmlWriter writer = UtilityMethods.CreateWriter(outputStream, Encoding.UTF8);

            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        SetRelationshipIDAttribute(reader, writer, relationId);
                        reader.MoveToElement();
                        if (reader.IsEmptyElement)
                            writer.WriteEndElement();
                        break;

                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        writer.WriteEndElement();
                        break;

                    case XmlNodeType.SignificantWhitespace:
                        writer.WriteWhitespace(reader.Value);
                        break;
                    default:
                        break;
                }
            } while (reader.Read());
            writer.Flush();
            outputStream.Flush();
            return outputStream;
        }
        /// <summary>
        /// Set Relationship ID Attribute
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationId"></param>
        private void SetRelationshipIDAttribute(XmlReader reader, XmlWriter writer, string relationId)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "id":
                    case "href":
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, relationId);
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Serialize the textbox format.
        /// </summary>
        /// <param name="textBoxFormat"></param>
        private void SerializeTextBoxFormat(WTextBoxFormat textBoxFormat)
        {
            string horiz = string.Empty;
            string horizAlign = string.Empty;
            string vert = string.Empty;
            string vertAlign = string.Empty;
            string textVertAlign = string.Empty;
            StringBuilder textBoxStyle = new StringBuilder();

            if (textBoxFormat.HorizontalOrigin != HorizontalOrigin.Column)
            {
                horiz = GetHorizOriginAsString(textBoxFormat.HorizontalOrigin);
            }

            if (textBoxFormat.VerticalOrigin != VerticalOrigin.Paragraph)
            {
                vert = GetVerticalOrginAsString(textBoxFormat.VerticalOrigin);
            }

            if (textBoxFormat.HorizontalAlignment != ShapeHorizontalAlignment.None)
            {
                horizAlign = textBoxFormat.HorizontalAlignment.ToString().ToLower();
            }

            if (textBoxFormat.VerticalAlignment != ShapeVerticalAlignment.None)
            {
                vertAlign = textBoxFormat.VerticalAlignment.ToString().ToLower();
            }
            //Serialize TextBox Vertical alignment of the Text
            textVertAlign = textBoxFormat.TextVerticalAlignment.ToString().ToLower();


            if (textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
            {
                textBoxStyle.Append("position:absolute;");
            }

            textBoxStyle.Append("margin-left:");
            textBoxStyle.Append(textBoxFormat.HorizontalPosition.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;margin-top:");
            textBoxStyle.Append(textBoxFormat.VerticalPosition.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;width:");
            textBoxStyle.Append(textBoxFormat.Width.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;height:");
            textBoxStyle.Append(textBoxFormat.Height.ToString().Replace(",", "."));
            textBoxStyle.Append("pt;");

            if (textBoxFormat.OrderIndex != int.MaxValue)
            {
                if (textBoxFormat.OrderIndex > 0 && textBoxFormat.IsBelowText)
                    textBoxStyle.Append("z-index:-" + textBoxFormat.OrderIndex.ToString() + ";");
                else
                    textBoxStyle.Append("z-index:" + textBoxFormat.OrderIndex.ToString() + ";");
            }
            else if (textBoxFormat.IsBelowText)
                textBoxStyle.Append("z-index:-251658752;");

            if (horiz.Length != 0)
            {
                textBoxStyle.Append("mso-position-horizontal-relative:");
                textBoxStyle.Append(horiz);
            }

            if (vert.Length != 0)
            {
                textBoxStyle.Append(";mso-position-vertical-relative:");
                textBoxStyle.Append(vert);
            }

            if (horizAlign.Length != 0)
            {
                textBoxStyle.Append(";mso-position-horizontal:");
                textBoxStyle.Append(horizAlign);
            }

            if (vertAlign.Length != 0)
            {
                textBoxStyle.Append(";mso-position-vertical:");
                textBoxStyle.Append(vertAlign);
            }
            if (textBoxFormat.WidthRelativePercent != 0)
            {
                textBoxStyle.Append(";mso-width-percent:" + (textBoxFormat.WidthRelativePercent * 10).ToString());
                textBoxStyle.Append(";mso-width-relative:" + GetWidthOrigin(textBoxFormat.WidthOrigin));
            }
            if (textBoxFormat.HeightRelativePercent != 0)
            {
                textBoxStyle.Append(";mso-height-percent:" + (textBoxFormat.HeightRelativePercent * 10).ToString());
                textBoxStyle.Append(";mso-height-relative:" + GetHeightOrigin(textBoxFormat.HeightOrigin));
            }
            //Serialize TextBox Vertical Alignment of the Text
            if (textVertAlign.Length != 0)
            {
                textBoxStyle.Append(";v-text-anchor:");
                textBoxStyle.Append(textVertAlign);
            }
            if (textBoxFormat.HorizontalRelativePercent != float.MinValue)
            {
                textBoxStyle.Append(";mso-left-percent:");
                textBoxStyle.Append(textBoxFormat.HorizontalRelativePercent * 10);
            }
            if (textBoxFormat.VerticalRelativePercent != float.MinValue)
            {
                textBoxStyle.Append(";mso-top-percent:");
                textBoxStyle.Append(textBoxFormat.VerticalRelativePercent * 10);
            }
            textBoxStyle.Append(";mso-wrap-distance-left:");
            textBoxStyle.Append(textBoxFormat.WrapDistanceLeft);
            textBoxStyle.Append("pt;mso-wrap-distance-top:");
            textBoxStyle.Append(textBoxFormat.WrapDistanceTop);
            textBoxStyle.Append("pt;mso-wrap-distance-right:");
            textBoxStyle.Append(textBoxFormat.WrapDistanceRight);
            textBoxStyle.Append("pt;mso-wrap-distance-bottom:");
            textBoxStyle.Append(textBoxFormat.WrapDistanceBottom);
            textBoxStyle.Append("pt;");
            if (textBoxFormat.HasDocxProps)
            {
                foreach (string prop in textBoxFormat.DocxStyleProps)
                {
                    if (!(prop.Contains("mso-wrap-distance-left") 
                        || prop.Contains("mso-wrap-distance-top") || prop.Contains("mso-wrap-distance-right") 
                        || prop.Contains("mso-wrap-distance-bottom")))
                        textBoxStyle.Append(";" + prop);
                }
            }

            m_writer.WriteAttributeString("style", textBoxStyle.ToString());
            // serialize wrap coordinates.
            if (textBoxFormat.TextWrappingStyle == TextWrappingStyle.Through || textBoxFormat.TextWrappingStyle == TextWrappingStyle.Tight)
            {
                StringBuilder wrapCoords = new StringBuilder();
                foreach (PointF point in textBoxFormat.WrapPolygon.Vertices)
                {
                    wrapCoords.Append(point.X.ToString(CultureInfo.InvariantCulture) + " ");
                    wrapCoords.Append(point.Y.ToString(CultureInfo.InvariantCulture) + " ");
                }
                m_writer.WriteAttributeString("wrapcoords", wrapCoords.ToString());
            }

            if (textBoxFormat.FillEfects.Type == BackgroundType.Color || textBoxFormat.FillEfects.Type == BackgroundType.Gradient)
            {
                Color color = Color.Empty;
                if (textBoxFormat.FillEfects.Type == BackgroundType.Gradient)
                    color = textBoxFormat.FillEfects.Gradient.Color1;
                else
                    color = textBoxFormat.FillColor;

                if (color == Color.Empty)
                {
                    m_writer.WriteAttributeString("filled", "f");
                }
                else
                {
                    m_writer.WriteAttributeString("fillcolor", "#" + GetRGBCode(color));
                }
            }

            if (textBoxFormat.LineColor != Color.Empty)
            {
                m_writer.WriteAttributeString("strokecolor", "#" + GetRGBCode(textBoxFormat.LineColor));
            }

            if (textBoxFormat.LineWidth > 0)
            {
                m_writer.WriteAttributeString("strokeweight", textBoxFormat.LineWidth.ToString().Replace(",", ".") + "pt");
            }

            if (!textBoxFormat.AllowInCell)
            {
                m_writer.WriteAttributeString("allowincell", DocxConstants.O_namespace, "f");
            }
            if (!textBoxFormat.AllowOverlap)
            {
                m_writer.WriteAttributeString("allowoverlap", DocxConstants.O_namespace, "f");
            }

            if (textBoxFormat.NoLine)
            {
                m_writer.WriteAttributeString("stroked", "f");
            }
            else
            {
                SerializeStroke(textBoxFormat);
            }

            if (textBoxFormat.TextWrappingStyle != TextWrappingStyle.InFrontOfText &&
              textBoxFormat.TextWrappingStyle != TextWrappingStyle.Behind)
            {
                m_writer.WriteStartElement("wrap", DocxConstants.W10_namespace);
                m_writer.WriteAttributeString("type", GetTextWrappingStyleAsString(textBoxFormat.TextWrappingStyle));
                if (textBoxFormat.TextWrappingType != TextWrappingType.Both)
                    m_writer.WriteAttributeString("side", GetTextWrappingTypeAsString(textBoxFormat.TextWrappingType));

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Get the vertical Origin of the height in TextBox.
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        private string GetHeightOrigin(HeightOrigin rel)
        {
            switch (rel)
            {
                case HeightOrigin.Page:
                    return "page";
                case HeightOrigin.TopMargin:
                    return "top-margin-area";
                case HeightOrigin.InsideMargin:
                    return "inner-margin-area";
                case HeightOrigin.OutsideMargin:
                    return "outer-margin-area";
                case HeightOrigin.BottomMargin:
                    return "bottom-margin-area";
                default:
                    return "margin";
            }
        }
        /// <summary>
        /// Get the horizontal Origin of the width in TextBox.
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        private string GetWidthOrigin(WidthOrigin rel)
        {
            switch (rel)
            {
                case WidthOrigin.Page:
                    return "page";
                case WidthOrigin.LeftMargin:
                    return "left-margin-area";
                case WidthOrigin.InsideMargin:
                    return "inner-margin-area";
                case WidthOrigin.OutsideMargin:
                    return "outer-margin-area";
                case WidthOrigin.RightMargin:
                    return "right-margin-area";
                default:
                    return "margin";
            }
        }
        /// <summary>
        /// Get the textWrapping Type
        /// </summary>
        /// <param name="textWrappingType"></param>
        /// <returns></returns>
        private string GetTextWrappingTypeAsString(TextWrappingType textWrappingType)
        {
            switch (textWrappingType)
            {
                case TextWrappingType.Left:
                    return "left";
                case TextWrappingType.Right:
                    return "right";
                default:
                    return "largest";
            }
        }
        /// <summary>
        /// Get the textWrapping style
        /// </summary>
        /// <param name="textWrappingStyle"></param>
        /// <returns></returns>
        private string GetTextWrappingStyleAsString(TextWrappingStyle textWrappingStyle)
        {
            switch (textWrappingStyle)
            {
                case TextWrappingStyle.Inline:
                    return "none";
                case TextWrappingStyle.Tight:
                    return "tight";
                case TextWrappingStyle.TopAndBottom:
                    return "topAndBottom";
                case TextWrappingStyle.Through:
                    return "through";
                default:
                    return "square";
            }
        }
        /// <summary>
        /// Serialize the stroke value.
        /// </summary>
        /// <param name="textBoxFormat"></param>
        private void SerializeStroke(WTextBoxFormat textBoxFormat)
        {
            string dashStyle = GetDashStyle(textBoxFormat.LineDashing, true);
            string lineStyle = GetLineStyle(textBoxFormat.LineStyle, true);

            if (dashStyle != null || lineStyle != null)
            {
                m_writer.WriteStartElement("stroke", DocxConstants.V_namespace);

                if (dashStyle != null)
                    m_writer.WriteAttributeString("dashstyle", dashStyle);
                if (lineStyle != null)
                    m_writer.WriteAttributeString("linestyle", lineStyle);

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Get the line dashing style.
        /// </summary>
        /// <param name="lineDashing"></param>
        /// <returns></returns>
        private string GetDashStyle(LineDashing lineDashing , bool is2007)
        {
            switch (lineDashing)
            {
                case LineDashing.Solid:
                    return "solid";
                case LineDashing.Dash:
                    return "sysDash";
                case LineDashing.DashDot:
                    return "sysDashDot";
                case LineDashing.DashDotDot:
                    return "sysDashDotDot";
                case LineDashing.Dot:
                    return "sysDot";
                case LineDashing.DashGEL:
                    return "dash";
                case LineDashing.DashDotGEL:
                    return "dashDot";
                case LineDashing.DotGEL:
                    return  "dot";
                case LineDashing.LongDashGEL:
                    return is2007? "longDash" : "lgDash";
                case LineDashing.LongDashDotGEL:
                    return is2007? "longDashDot":"lgDashDot";
                case LineDashing.LongDashDotDotGEL:
                    return is2007? "longDashDotDot": "lgDashDotDot";
                default:
                    return null;
            }
        }
        /// <summary>
        /// Get the line style
        /// </summary>
        /// <param name="lineStyle"></param>
        /// <returns></returns>
        private string GetLineStyle(TextBoxLineStyle lineStyle, bool is2007)
        {
            switch (lineStyle)
            {
                case TextBoxLineStyle.Double:
                    return is2007 ? "thinThin" : "dbl";
                case TextBoxLineStyle.ThinThick:
                    return "thinThick";
                case TextBoxLineStyle.ThickThin:
                    return "thickThin";
                case TextBoxLineStyle.Triple:
                    return is2007 ? "thickBetweenThin" : "tri";
                default:
                    return is2007 ? "single" : "sng";
            }
        }
        /// <summary>
        /// Serialize the inset attribute.
        /// </summary>
        /// <param name="textBox"></param>
        private void SerializeInsetAttribute(WTextBox textBox)
        {
            float LeftMargin = textBox.TextBoxFormat.InternalMargin.Left;
            if (LeftMargin != 0)
                LeftMargin = (float)Math.Round(LeftMargin, 3);

            float RightMargin = textBox.TextBoxFormat.InternalMargin.Right;
            if (RightMargin != 0)
                RightMargin = (float)Math.Round(RightMargin, 3);

            float TopMargin = textBox.TextBoxFormat.InternalMargin.Top;
            if (TopMargin != 0)
                TopMargin = (float)Math.Round(TopMargin, 3);

            float BottomMargin = textBox.TextBoxFormat.InternalMargin.Bottom;
            if (BottomMargin != 0)
                BottomMargin = (float)Math.Round(BottomMargin, 3);

            string insetValue = string.Empty;

            insetValue += (XmlConvert.ToString(LeftMargin) + "pt,");
            insetValue += (XmlConvert.ToString(TopMargin) + "pt,");
            insetValue += (XmlConvert.ToString(RightMargin) + "pt,");
            insetValue += (XmlConvert.ToString(BottomMargin) + "pt");

            m_writer.WriteAttributeString("inset", insetValue);
        }
        /// <summary>
        /// Check whether to skip the paragraph item while serializing.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool SkipItem(ParagraphItem item)
        {
            if (item is WPicture && m_nonSupportedFields != null && m_nonSupportedFields.Count > 0)
            { //To skip picture of ole object between FieldMarkSeparator and FieldMarkEnd
                WField embedField = m_nonSupportedFields.Peek() as WField;
                if (embedField != null && embedField.FieldType == FieldType.FieldEmbed)
                    return true;
            }
            if (item is WField || item is WFieldMark)
            {
                if (m_nonSupportedFields != null && m_nonSupportedFields.Count > 0)
                { //To skip the Fieldmark of OleObject
                    WField field = m_nonSupportedFields.Peek() as WField;
                    if (field != null && field.FieldType == FieldType.FieldEmbed)
                    {
                        if (item is WFieldMark
                            && (item as WFieldMark).Type == FieldMarkType.FieldEnd)
                            m_nonSupportedFields.Pop();
                        return true;
                    }
                }
                return false;
            }
            if (m_nonSupportedFields == null || m_nonSupportedFields.Count == 0)
            {
                return false;
            }


            return true;
        }

        #endregion Textbox
        /// <summary>
        /// Serialize the Xml paragraph item.
        /// </summary>
        /// <param name="item"></param>
        private void SerializeXmlParagraphItem(XmlParagraphItem item)
        {
            if (m_document.DocxPackage == null)
                return;

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(item.CharacterFormat);
            Stream stream;
            //Change doc property identifiers
            if (item.Relations.Count == 0
                && item.ImageRelations.Count == 0)
            {
                List<string> newIds = new List<string>();
                stream = ChangeIDAttribute(item.DataNode, ref newIds);
            }
            else
                // Change relation identificators.
                stream = ChangeItemRel(item);

            XmlReader xmlItemReader = CreateReader(stream);
            m_writer.WriteNode(xmlItemReader, false);
            m_writer.WriteEndElement();
        }
        private Stream UpdateXMLRelation(Shape shape, Stream stream)
        {
            Stream updatedStream = stream;
            //Change doc property identifiers
            if (shape.Relations.Count == 0
                && shape.ImageRelations.Count == 0)
            {
                List<string> newIds = new List<string>();
                updatedStream = ChangeIDAttribute(stream, ref newIds);
            }
            else
                // Change relation identificators.
                updatedStream = ChangeItemRel(shape, stream);
            return updatedStream;
        }
        private Stream ChangeItemRel(Shape shape, Stream shapestream)
        {
            List<String> oldIds;
            List<String> newIds = new List<string>();
            oldIds = FindRelationshipIds(shapestream);
            Stream stream = ChangeIDAttribute(shapestream, ref newIds);

            for (int i = 0, count = oldIds.Count; i < count; i++)
            {
                string oldID = oldIds[i];
                string newID = newIds[i];

                if (shape.ImageRelations.ContainsKey(oldID))
                {
                    ChangePicRel(shape, oldID, newID);
                }
                else
                {
                    DictionaryEntry itemRel = shape.Relations[oldID];
                    string relType = itemRel.Key.ToString();
                    UpdateItemRels(shape, newID, itemRel);

                    if (relType == DocxConstants.ChartRelType || relType == DocxConstants.ControlRelType)
                    {
                        AddXmlItemContType(relType, itemRel.Value.ToString());
                    }
                    else if (relType == DocxConstants.OleObjectRelType || relType == DocxConstants.PackageRelType)
                    {
                        m_hasOleObject = true;
                        UpdateOleContentType(shapestream);
                    }
                    else
                    {
                        m_hasDiagrams = true;
                    }
                }
            }
            return stream;
        }
        private void ChangePicRel(Shape shape, string oldID, string newID)
        {
            ImageRecord imageRecord = null;
            if (!shape.ImageRelations.ContainsKey(oldID))
                return;
            imageRecord = shape.ImageRelations[oldID];
            Entity owner = GetBaseEntity(shape);//GetXmlItemOwner(item);
            if (owner is HeaderFooter)
            {
                HeaderFooter headerFooter = owner as HeaderFooter;
                UpdateHFImageRels(newID, headerFooter, imageRecord);
            }
            else if (owner.Owner != null && owner.Owner is HeaderFooter)
            {
                HeaderFooter headerFooter = owner.Owner as HeaderFooter;
                UpdateHFImageRels(newID, headerFooter, imageRecord);
            }
            else
            {
                if (owner is WFootnote)
                {
                    if ((owner as WFootnote).FootnoteType == FootnoteType.Footnote)
                        FootnoteImages.Add(newID, imageRecord);
                    else
                        EndnoteImages.Add(newID, imageRecord);
                }
                else if (owner is WComment)
                    CommentImages.Add(newID, imageRecord);
                else
                    DocumentImages.Add(newID, imageRecord);
            }

            if (imageRecord.IsMetafile)
                m_hasMetafiles = true;
            else
                m_hasImages = true;
        }
        /// <summary>
        /// Change the relation ids in the XmlParagraph Item.
        /// </summary>
        /// <param name="item"></param>
        private Stream ChangeItemRel(XmlParagraphItem item)
        {
            List<String> oldIds;
            List<String> newIds = new List<string>();
            oldIds = FindRelationshipIds(item.DataNode);
            Stream stream = ChangeIDAttribute(item.DataNode, ref newIds);

            for (int i = 0, count = oldIds.Count; i < count; i++)
            {
                string oldID = oldIds[i];
                string newID = newIds[i];

                if (item.ImageRelations.ContainsKey(oldID))
                {
                    ChangePicRel(item, oldID, newID);
                }
                else
                {
                    DictionaryEntry itemRel = item.Relations[oldID];
                    string relType = itemRel.Key.ToString();
                    UpdateItemRels(item, newID, itemRel);

                    if (relType == DocxConstants.ChartRelType || relType == DocxConstants.ControlRelType)
                    {
                        AddXmlItemContType(relType, itemRel.Value.ToString());
                    }
                    else if (relType == DocxConstants.OleObjectRelType || relType == DocxConstants.PackageRelType)
                    {
                        m_hasOleObject = true;
                        UpdateOleContentType(item.DataNode);
                    }
                    else
                    {
                        m_hasDiagrams = true;
                    }
                }
            }
            return stream;
        }
        /// <summary>
        /// Changes the picture relations.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="oldID">The old ID.</param>
        /// <param name="newID">The new ID.</param>
        private void ChangePicRel(XmlParagraphItem item, string oldID, string newID)
        {
            ImageRecord imageRecord = null;
            if (!item.ImageRelations.ContainsKey(oldID))
                return;
            imageRecord = item.ImageRelations[oldID];
            Entity owner = GetXmlItemOwner(item);
            if (owner is HeaderFooter)
            {
                HeaderFooter headerFooter = owner as HeaderFooter;
                UpdateHFImageRels(newID, headerFooter, imageRecord);
            }
            else if (owner.Owner != null && owner.Owner is HeaderFooter)
            {
                HeaderFooter headerFooter = owner.Owner as HeaderFooter;
                UpdateHFImageRels(newID, headerFooter, imageRecord);
            }
            else
            {
                if (owner is WFootnote)
                {
                    if ((owner as WFootnote).FootnoteType == FootnoteType.Footnote)
                        FootnoteImages.Add(newID, imageRecord);
                    else
                        EndnoteImages.Add(newID, imageRecord);
                }
                else if (owner is WComment)
                    CommentImages.Add(newID, imageRecord);
                else
                    DocumentImages.Add(newID, imageRecord);
            }

            if (imageRecord.IsMetafile)
                m_hasMetafiles = true;
            else
                m_hasImages = true;
        }
        /// <summary>
        /// Get the Xml paragraph item owner.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private Entity GetXmlItemOwner(XmlParagraphItem item)
        {
            Entity owner = item.Owner;
            WParagraph ownerPara = null;

            if (item.Owner is WOleObject)
                owner = (item.Owner as WOleObject).OwnerParagraph;
            if (owner.EntityType == EntityType.SDTInlineContent)
                ownerPara = owner.Owner.Owner as WParagraph;
            else if (owner.EntityType == EntityType.Paragraph)
                ownerPara = owner as WParagraph;

            WTableCell ownerCell = ownerPara.Owner as WTableCell;
            Entity ownerEntity = ownerPara.Owner.Owner;
            HeaderFooter ownerHeaderFooter;
            Entity ownerTextBody;

            if (ownerCell != null)
                ownerTextBody = ownerCell.OwnerRow.OwnerTable.OwnerTextBody;
            else
                ownerTextBody = ownerPara.Owner;

            ownerEntity = ownerTextBody.Owner;

            if (ownerEntity is WTextBox && (ownerEntity as WTextBox).OwnerParagraph != null)
                ownerHeaderFooter = (ownerEntity as WTextBox).OwnerParagraph.OwnerTextBody as HeaderFooter;
            else
                ownerHeaderFooter = ownerTextBody as HeaderFooter;

            if (ownerHeaderFooter != null)
                return ownerHeaderFooter;
            else
                return ownerEntity;
        }
        /// <summary>
        /// Updates the relations for xml items.
        /// </summary>
        /// <param name="newID">The new ID.</param>
        /// <param name="itemRel">The relation item.</param>
        private void UpdateItemRels(ParagraphItem item, string newID, DictionaryEntry itemRel)
        {
            if ((item.OwnerBase as WParagraph).Owner is HeaderFooter)
            {
                HeaderFooter headFoot = (item.OwnerBase as WParagraph).Owner as HeaderFooter;
                UpdateHFXmlRels(newID, headFoot, itemRel);
            }
            else
            {
                XmlItemsRelations.Add(newID, itemRel);
            }
        }
        /// <summary>
        /// Updates the relations for header/footer.
        /// </summary>
        /// <param name="newId">The new id.</param>
        /// <param name="hf">The hf.</param>
        /// <param name="itemRel">The item rel.</param>
        private void UpdateHFXmlRels(string newId, HeaderFooter hf, DictionaryEntry itemRel)
        {
            string id = GetHeaderFooterId(hf);
            if (id == null)
                return;

            Dictionary<String, DictionaryEntry> relation = null;
            if (!HFRelations.ContainsKey(id))
            {
                relation = new Dictionary<String, DictionaryEntry>();
                HFRelations.Add(id, relation);
            }
            else
                relation = HFRelations[id];

            relation.Add(newId, itemRel);
        }
        /// <summary>
        /// Changes the ID attribute in XmlNode.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private Stream ChangeIDAttribute(Stream inputStream, ref List<String> relationIds)
        {
            inputStream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(inputStream);

            MemoryStream outputStream = new MemoryStream((int)inputStream.Length);
            XmlWriter writer = UtilityMethods.CreateWriter(outputStream, Encoding.UTF8);

            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        ChangeRelationshipIDs(reader, writer, ref relationIds);
                        reader.MoveToElement();
                        if (reader.IsEmptyElement)
                            writer.WriteEndElement();
                        break;

                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        writer.WriteEndElement();
                        break;

                    case XmlNodeType.SignificantWhitespace:
                        writer.WriteWhitespace(reader.Value);
                        break;
                    default:
                        break;
                }
            } while (reader.Read());
            writer.Flush();
            outputStream.Flush();
            return outputStream;
        }
        /// <summary>
        /// Changes Relationship ID's
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationIds"></param>
        private void ChangeRelationshipIDs(XmlReader reader, XmlWriter writer, ref List<string> relationIds)
        {
            if (reader.LocalName == "fill" || reader.LocalName == "chart" || reader.LocalName == "imagedata"
                        || reader.LocalName == "stroke" || reader.LocalName == "control" || reader.LocalName == "OLEObject"
                         || reader.LocalName == "hyperlink")
            {
                ChangeRelationshipIDAttribute(reader, writer, ref relationIds);
            }
            else if (reader.LocalName == "arc" || reader.LocalName == "curve" || reader.LocalName == "line"
                        || reader.LocalName == "oval" || reader.LocalName == "polyline" || reader.LocalName == "rect"
                        || reader.LocalName == "roundrect")
            {
                ChangeShapeIDAttribute(reader, writer);
            }
            else if (reader.LocalName == "blip")
            {
                ChangeBlipIDAttribute(reader, writer, ref relationIds);
            }
            else if (reader.LocalName == "relIds")
            {
                ChangeRelationIDAttribute(reader, writer, ref relationIds);
            }
            else if (reader.LocalName == "docPr")
            {
                ChangeDocPropertyIDAttribute(reader, writer);
            }
            else if (reader.LocalName == "numId")
            {
                ChangeNumId(reader, writer);
            }
            else
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                }
            }
        }
        /// <summary>
        /// Change Num Id
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        private void ChangeNumId(XmlReader reader, XmlWriter writer)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "val":
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            if (m_document.ListStyleNames.ContainsKey(reader.Value))
                            {
                                string styleName = m_document.ListStyleNames[reader.Value];
                                string id = GetListNumId(styleName);
                                writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, id);
                            }
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Get List Id
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private string GetListNumId(string styleName)
        {
            int listId = 0;
            foreach (ListStyle listStyle in m_document.ListStyles)
            {
                if (listStyle.Name == styleName)
                {
                    listId += 1;
                    break;
                }
                else
                {
                    listId += 1;
                }
            }
            return listId.ToString();
        }
        /// <summary>
        /// Changes Relationship ID
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationIds"></param>
        private void ChangeRelationshipIDAttribute(XmlReader reader, XmlWriter writer, ref List<string> relationIds)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "id":
                    case "href":
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            string id = GetNextRelationShipID();
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, id);
                            relationIds.Add(id);
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Changes Relationship ID for Blip 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationIds"></param>
        private void ChangeBlipIDAttribute(XmlReader reader, XmlWriter writer, ref List<string> relationIds)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "embed":
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            string id = GetNextRelationShipID();
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, id);
                            relationIds.Add(id);
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Changes Relationship ID for Relation 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationIds"></param>
        private void ChangeRelationIDAttribute(XmlReader reader, XmlWriter writer, ref List<string> relationIds)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "dm":
                    case "lo":
                    case "qs":
                    case "cs":
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            string id = GetNextRelationShipID();
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, id);
                            relationIds.Add(id);
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Changes Doc Property ID attribute
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        private void ChangeDocPropertyIDAttribute(XmlReader reader, XmlWriter writer)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "id":
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            string id = GetNextDocPrID().ToString();
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, id);
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
        /// <summary>
        /// Changes Shape ID
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="relationIds"></param>
        private void ChangeShapeIDAttribute(XmlReader reader, XmlWriter writer)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "id":                   
                        if (reader.Value != null && reader.Value.Length > 0)
                        {
                            string shapeId = "_x0000_i" + GetNextShapeID().ToString();
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, shapeId);
                        }
                        else
                            writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                    default:
                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                        break;
                }
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Changes the ID attribute in diagram.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="array">The array.</param>
        private void ChangeDiagID(XmlNode childNode, List<String> array)
        {
            XmlAttribute attr = null;

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        attr = childNode.Attributes["dm", DocxConstants.R_namespace];
                        break;
                    case 1:
                        attr = childNode.Attributes["lo", DocxConstants.R_namespace];
                        break;
                    case 2:
                        attr = childNode.Attributes["qs", DocxConstants.R_namespace];
                        break;
                    case 3:
                        attr = childNode.Attributes["cs", DocxConstants.R_namespace];
                        break;
                }

                if (attr != null)
                {
                    attr.Value = GetNextRelationShipID();
                    array.Add(attr.Value);
                }
            }
        }
#endif
        /// <summary>
        /// Adds the type of the XML item cont.
        /// </summary>
        /// <param name="relType">Type of the rel.</param>
        /// <param name="relTarget">The rel target.</param>
        private void AddXmlItemContType(string relType, string relTarget)
        {
            if (relType == DocxConstants.ChartRelType)
            {
                ChartsPathNames.Add(@"word/" + relTarget);
                UpdateChartInnerRelation(@"word/" + relTarget);
            }
            else if (relType == DocxConstants.ControlRelType)
            {
                ControlsPathNames.Add(@"word/" + relTarget);
            }
        }
        /// <summary>
        /// Updates the chart inner relation.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        private void UpdateChartInnerRelation(string containerName)
        {
            PartContainer container = m_document.DocxPackage.FindPartContainer(containerName);
            // Gets xml part relations key.
            string partName = containerName.Substring(containerName.LastIndexOf('/') + 1);
            string xmlPartRelationKey = container.GetXmlPartRelationKey(partName);
            if (!string.IsNullOrEmpty(xmlPartRelationKey))
            {
                Stream stream = container.Relations[xmlPartRelationKey].DataStream;
                stream.Position = 0;
                XmlReader xmlReader = UtilityMethods.CreateReader(stream);

                xmlReader.MoveToContent();
                if (xmlReader.LocalName != "Relationships")
                    xmlReader.ReadInnerXml();
                if (xmlReader.IsEmptyElement)
                    return;
                do
                {
                    xmlReader.Read();
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        string targetMode = xmlReader.GetAttribute("TargetMode");
                        bool isExternal = (targetMode == "External") ? true : false;
                        if (!isExternal)
                        {
                            string target = xmlReader.GetAttribute("Target");
                            string path = target;
                            if (target.StartsWith("../"))
                                path = m_document.DocxPackage.GetXmlPartContainerPath(container, target);
                            else
                                path = containerName.Remove(containerName.LastIndexOf('/') + 1) + target;
                            if (path.EndsWith(".xml"))
                            {
                                ChartsPathNames.Add(path);
                                UpdateChartInnerRelation(path);
                            }
                        }
                    }
                }
                while (xmlReader.LocalName != "Relationships");
#if WINRT
                xmlReader.Dispose();
#else
                xmlReader.Close();
#endif
                stream.Position = 0;
            }
        }

        #region Absolute Tab
        private void SerializeAbsoluteTab(WAbsoluteTab absoluteTab)
        {
            m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
            SerializeCharactetFormat(absoluteTab.CharacterFormat);
            m_writer.WriteStartElement("ptab", DocxConstants.W_namespace);
            switch (absoluteTab.Relation)
            {
                case AbsoluteTabRelation.Margin:
                    m_writer.WriteAttributeString("w", "relativeTo", DocxConstants.W_namespace, "margin");
                    break;
                case AbsoluteTabRelation.Indent:
                    m_writer.WriteAttributeString("w", "relativeTo", DocxConstants.W_namespace, "margin");
                    break;
            }
            switch (absoluteTab.Alignment)
            {
                case AbsoluteTabAlignment.Left:
                    m_writer.WriteAttributeString("w", "alignment", DocxConstants.W_namespace, "left");
                    break;
                case AbsoluteTabAlignment.Right:
                    m_writer.WriteAttributeString("w", "alignment", DocxConstants.W_namespace, "right");
                    break;
                case AbsoluteTabAlignment.Center:
                    m_writer.WriteAttributeString("w", "alignment", DocxConstants.W_namespace, "center");
                    break;
            }
            m_writer.WriteAttributeString("leader", DocxConstants.W_namespace, GetTabLeader(absoluteTab.TabLeader));
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        #endregion

        #region OLEObject
        /// <summary>
        /// Serialize the ole object.
        /// </summary>
        /// <param name="oleObject"></param>
        private void SerializeOleObject(WOleObject oleObject)
        {
            if (oleObject.OleXmlItem != null)
            {
                oleObject.OleXmlItem.SetOwner(oleObject.Owner);
                SerializeXmlParagraphItem(oleObject.OleXmlItem);
                return;
            }

            if (oleObject.IsEmpty
                && oleObject.LinkType == OleLinkType.Embed
                || oleObject.OlePicture == null)
                return;

            WField embedField = new WField(m_document);
            embedField.FieldType = FieldType.FieldEmbed;

            NonSupportedFields.Push(embedField);

            m_hasOleObject = true;

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(oleObject.OlePicture.PictureCharacterFormat);
            m_writer.WriteStartElement("object", DocxConstants.W_namespace);

            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
            string shapeId = "_x0000_i" + GetNextShapeID().ToString();
            m_writer.WriteAttributeString("id", shapeId);
            m_writer.WriteAttributeString("type", "#_x0000_t75");

            string style = GetOlePictureStyle(oleObject.OlePicture);
            m_writer.WriteAttributeString("style", style);
            m_writer.WriteAttributeString("ole", DocxConstants.O_namespace, "");

            SerializeOlePicture(oleObject); //write imageData
            m_writer.WriteEndElement(); //end of shape tag

            SerializeOleData(oleObject, shapeId); //write oleObject tag

            m_writer.WriteEndElement(); //end of object tag
            m_writer.WriteEndElement(); //end of run tag
        }
        /// <summary>
        /// Serialize the ole data.
        /// </summary>
        /// <param name="oleObject"></param>
        /// <param name="shapeId"></param>
        private void SerializeOleData(WOleObject oleObject, string shapeId)
        {
            m_writer.WriteStartElement("OLEObject", DocxConstants.O_namespace);

            string linkType = (oleObject.LinkType == OleLinkType.Embed) ? "Embed" : "Link";
            m_writer.WriteAttributeString("Type", linkType);
            m_writer.WriteAttributeString("ProgID", oleObject.ObjectType);
            m_writer.WriteAttributeString("ShapeID", shapeId);
            if (oleObject.DisplayAsIcon)
                m_writer.WriteAttributeString("DrawAspect", "Icon");
            else
                m_writer.WriteAttributeString("DrawAspect", "Content");

            string id = null;
            string hfID = null;
            IEntity ent = GetBaseEntity(oleObject);
            if (ent.EntityType == EntityType.HeaderFooter)
                //hfID = Null indicates that the current entity is not a Header footer child.
                hfID = GetHeaderFooterId(ent as HeaderFooter);
            //Checks whether ole object is already present in current header footer
            //If hfID == null, Checks whether ole is already added to the main document by checking the Ole storage name.
            if (!(hfID != null && !HFOleContainers.ContainsKey(hfID))
                && OleIds.ContainsKey(oleObject.OleStorageName) && oleObject.LinkType == OleLinkType.Embed)
            {
                id = OleIds[oleObject.OleStorageName];
                m_writer.WriteAttributeString("ObjectID", "_" + oleObject.OleStorageName);
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id);
            }
            else
            {
                id = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id);
                if (!OleIds.ContainsKey(oleObject.OleStorageName) && oleObject.LinkType == OleLinkType.Embed)
                    OleIds.Add(oleObject.OleStorageName, id);

                string target = null;
                if (oleObject.LinkType == OleLinkType.Embed)
                {
                    m_writer.WriteAttributeString("ObjectID", "_" + oleObject.OleStorageName);
                    string oleName = GetOleFileName(oleObject.OleObjectType);
                    if (ent.EntityType == EntityType.HeaderFooter)
                    {
                        if (HFOleContainers.ContainsKey(hfID))
                            HFOleContainers[hfID].Add(oleName, oleObject.GetOlePartStream());
                        else
                        {
                            HFOleContainers.Add(hfID, new Dictionary<string, Stream>());
                            HFOleContainers[hfID].Add(oleName, oleObject.GetOlePartStream());
                        }
                    }
                    else
                    {
                        OleContainers.Add(oleName, oleObject.GetOlePartStream());
                    }
                    target = "embeddings/" + oleName;
                }
                else
                {
                    m_writer.WriteAttributeString("UpdateMode", "Always");
                    WriteLinkOptions();
                    target = oleObject.LinkPath.Replace(" ", "%20");
                    if (!string.IsNullOrEmpty(target) && !target.Contains("file:///"))
                        target = "file:///" + target;
                }

                string relType = GetOleRelType(oleObject.OleObjectType);

                DictionaryEntry entry = new DictionaryEntry(relType, target);
                UpdateItemRelation(oleObject, id, entry);
                UpdateOleContentType(oleObject.ObjectType);
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the ole picture.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializeOlePicture(WOleObject oleObject)
        {
            WPicture picture = oleObject.OlePicture;
            string picId = UpdateShapeId(picture, true, oleObject);// GetNextRelationShipID();

            m_writer.WriteStartElement("imagedata", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.R_namespace, picId);
            m_writer.WriteAttributeString("title", DocxConstants.O_namespace, "");
            m_writer.WriteEndElement();

            if (picture.IsMetaFile)
            {
                m_hasMetafiles = true;
            }
            else
            {
                m_hasImages = true;
            }
        }
        /// <summary>
        /// Serialize the link options.
        /// </summary>
        private void WriteLinkOptions()
        {
            m_writer.WriteStartElement("LinkType", DocxConstants.O_namespace);
            m_writer.WriteString("EnhancedMetaFile");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("LockedField", DocxConstants.O_namespace);
            m_writer.WriteString("false");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("FieldCodes", DocxConstants.O_namespace);
            m_writer.WriteString(SlashSymbol + SlashSymbol + "f 0");
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        private string GetOleFileName(OleObjectType objectType)
        {
            string name = null;
            string strObjType = OleTypeConvertor.ToString(objectType, false);
            switch (strObjType)
            {
                case "Excel.Chart.8":
                case "Excel.Sheet.8":
                    name = "Microsoft_Office_Excel_97-2003_Worksheet" + GetNextID().ToString() + ".xls";
                    break;
                case "Excel.Sheet.12":
                    name = "Microsoft_Office_Excel_Worksheet" + GetNextID().ToString() + ".xlsx";
                    break;
                case "Excel.SheetMacroEnabled.12":
                    name = "Microsoft_Office_Excel_Macro-Enabled_Worksheet" + GetNextID().ToString() + ".xlsm";
                    break;
                case "Excel.SheetBinaryMacroEnabled.12":
                    name = "Microsoft_Office_Excel_Binary_Worksheet" + GetNextID().ToString() + ".xlsb";
                    break;
                case "PowerPoint.Show.8":
                    name = "Microsoft_Office_PowerPoint_97-2003_Presentation" + GetNextID().ToString() + ".ppt";
                    break;
                case "PowerPoint.Show.12":
                    name = "Microsoft_Office_PowerPoint_Presentation" + GetNextID().ToString() + ".pptx";
                    break;
                case "Word.Document.8":
                    name = "Microsoft_Office_Word_97-2003_Document" + GetNextID().ToString() + ".doc";
                    break;
                case "Word.Document.12":
                    name = "Microsoft_Office_Word_Document" + GetNextID().ToString() + ".docx";
                    break;
                case "Word.DocumentMacroEnabled.12":
                    name = "Microsoft_Office_Word_Macro-Enabled_Document" + GetNextID().ToString() + ".docm";
                    break;
                case "PowerPoint.ShowMacroEnabled.12":
                    name = "Microsoft_Office_PowerPoint_Macro-Enabled_Presentation" + GetNextID().ToString() + ".pptm";
                    break;
                case "PowerPoint.SlideMacroEnabled.12":
                    name = "Microsoft_Office_PowerPoint_Macro-Enabled_Slide" + GetNextID().ToString() + ".sldm";
                    break;
                case "PowerPoint.Slide.12":
                    name = "Microsoft_Office_PowerPoint_Slide" + GetNextID().ToString() + ".sldx";
                    break;
                default:
                    name = "oleObject" + GetNextID().ToString() + ".bin";
                    break;
            }

            return name;
        }
        /// <summary>
        /// Gets the type of the OLE relation.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        private string GetOleRelType(OleObjectType objectType)
        {
            string relType = null;
            string strObjType = OleTypeConvertor.ToString(objectType, false);
            switch (strObjType)
            {
                case "Excel.Sheet.12":
                case "Excel.SheetBinaryMacroEnabled.12":
                case "PowerPoint.Show.12":
                case "Word.Document.12":
                case "Word.DocumentMacroEnabled.12":
                case "Excel.SheetMacroEnabled.12":
                case "PowerPoint.ShowMacroEnabled.12":
                case "PowerPoint.SlideMacroEnabled.12":
                case "PowerPoint.Slide.12":
                    relType = DocxConstants.PackageRelType;
                    break;
                default:
                    relType = DocxConstants.OleObjectRelType;
                    break;
            }

            return relType;
        }
        /// <summary>
        /// Gets the ole picture style.
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private string GetOlePictureStyle(WPicture picture)
        {
            string style = string.Empty;
            float value = picture.Width * picture.WidthScale / 100;
            style = "width:" + value.ToString(CultureInfo.InvariantCulture).Replace(",", ".") + "pt";
            style += ";";
            value = picture.Height * picture.HeightScale / 100;
            style += "height:" + value.ToString(CultureInfo.InvariantCulture).Replace(",", ".") + "pt";

            return style;
        }
        /// <summary>
        /// Update the ole content type
        /// </summary>
        /// <param name="node"></param>
        private void UpdateOleContentType(Stream nodeStream)
        {
            XmlReader oleContentTypeReader = UtilityMethods.CreateReader(nodeStream);
            oleContentTypeReader.ReadToFollowing("OLEObject", DocxConstants.O_namespace);
            if (oleContentTypeReader.LocalName != "OLEObject")
                return;

            string attrValue = oleContentTypeReader.GetAttribute("ProgID");
            if (attrValue == null)
                return;

            UpdateOleContentType(attrValue);
        }
        /// <summary>
        /// Updates the content type of OLE object.
        /// </summary>
        /// <param name="type">The type.</param>
        private void UpdateOleContentType(string type)
        {
            string oleType = null;
            switch (type)
            {
                case "Excel.Chart.8":
                case "Excel.Sheet.8":
                    oleType = "application/vnd.ms-excel";
                    break;
                case "Excel.Sheet.12":
                    oleType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "Excel.SheetBinaryMacroEnabled.12":
                    oleType = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                    break;
                case "Excel.SheetMacroEnabled.12":
                    oleType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                    break;
                case "PowerPoint.Show.8":
                    oleType = "application/vnd.ms-powerpoint";
                    break;
                case "PowerPoint.Show.12":
                    oleType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case "Word.Document.8":
                    oleType = "application/msword";
                    break;
                case "Word.Document.12":
                    oleType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "Word.DocumentMacroEnabled.12":
                    oleType = "application/vnd.ms-word.document.macroEnabled.12";
                    break;
                case "PowerPoint.ShowMacroEnabled.12":
                    oleType = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                    break;
                case "PowerPoint.SlideMacroEnabled.12":
                    oleType = "application/vnd.ms-powerpoint.slide.macroEnabled.12";
                    break;
                case "PowerPoint.Slide.12":
                    oleType = "application/vnd.openxmlformats-officedocument.presentationml.slide";
                    break;
                default:
                    oleType = "application/vnd.openxmlformats-officedocument.oleObject";
                    break;
            }

            if (oleType != null)
            {
                if (!OleContentTypes.Contains(oleType))
                    OleContentTypes.Add(oleType);
            }
        }

        #endregion OLEObject

        #region Symbol
        /// <summary>
        /// Serialize the symbol.
        /// </summary>
        /// <param name="symbol"></param>
        private void SerializeSymbol(WSymbol symbol)
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(symbol.CharacterFormat);
            m_writer.WriteStartElement("sym", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("char", DocxConstants.W_namespace, "F0" + System.Convert.ToString(symbol.CharacterCode, 16).ToUpper());
            m_writer.WriteAttributeString("font", DocxConstants.W_namespace, symbol.FontName);

            if (m_symbolFontNames == null)
            {
                m_symbolFontNames = new List<String>();
            }

            if (!m_symbolFontNames.Contains(symbol.FontName))
            {
                m_symbolFontNames.Add(symbol.FontName);
            }

            m_writer.WriteEndElement();//end of sym tag
            m_writer.WriteEndElement();//end of run tag
        }
        #endregion Symbol

        #region MergeField
        /// <summary>
        /// Serialize the merge field.
        /// </summary>
        /// <param name="mergeField"></param>
        private void SerializeMergeField(WMergeField mergeField)
        {
            //<w:fldSimple w:instr=" MERGEFIELD  SecondField  \* MERGEFORMAT ">
            //    <w:r>
            //        <w:rPr>
            //            <w:noProof />
            //        </w:rPr>
            //        <w:t>«Secon</w:t>
            //    </w:r>
            //    <w:r w:rsidRPr="00A57496">
            //        <w:rPr>
            //            <w:b />
            //            <w:noProof />
            //        </w:rPr>
            //        <w:t>dFi</w:t>
            //    </w:r>
            //    <w:r>
            //        <w:rPr>
            //            <w:noProof />
            //        </w:rPr>
            //        <w:t>eld»</w:t>
            //    </w:r>
            //</w:fldSimple>
            if (mergeField.ConvertedToText && mergeField.Text != null)
            {
                SerializeMergeFieldText(mergeField);
            }
            else
            {
                m_writer.WriteStartElement("fldSimple", DocxConstants.W_namespace);
                SerializeMergeFieldCodes(mergeField);
                if (mergeField.TextItems.Count > 0)
                {
                    for (int i = 0, cnt = mergeField.TextItems.Count; i < cnt; i++)
                    {
                        SerializeTextRange(mergeField.TextItems[i]);
                    }
                }
                else
                {
                    m_writer.WriteStartElement("r", DocxConstants.W_namespace);
                    SerializeCharactetFormat(mergeField.CharacterFormat);
                    m_writer.WriteStartElement("t", DocxConstants.W_namespace);


                    m_writer.WriteString(mergeField.Text);

                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the merge field codes.
        /// </summary>
        /// <param name="mergeField"></param>
        private void SerializeMergeFieldCodes(WMergeField mergeField)
        {
            StringBuilder instr = new StringBuilder();
            instr.Append(" MERGEFIELD ");

            if (mergeField.Prefix != string.Empty)
            {
                instr.AppendFormat(mergeField.Prefix);
                instr.Append(":");
            }

            instr.Append(mergeField.FieldName);
            if (mergeField.TextBefore != string.Empty)
            {
                instr.Append(@" " + SlashSymbol + "b ");
                instr.Append(mergeField.TextBefore);
            }
            if (mergeField.TextAfter != string.Empty)
            {
                instr.Append(@" " + SlashSymbol + "f ");
                instr.Append(mergeField.TextAfter);
            }
            if (!string.IsNullOrEmpty(mergeField.DateFormat))
            {
                instr.Append(@" " + SlashSymbol + "@ ");
                instr.Append(InvertedCommas + mergeField.DateFormat + InvertedCommas);
            }
            if (!string.IsNullOrEmpty(mergeField.NumberFormat))
            {
                instr.Append(@" " + SlashSymbol + "# ");
                instr.Append(InvertedCommas + mergeField.NumberFormat + InvertedCommas);
            }
            if (mergeField.TextFormat != TextFormat.None)
            {
                instr.Append(@" " + SlashSymbol + "* ");
                instr.Append(GetTextFormat(mergeField.TextFormat));
            }
            instr.Append(mergeField.FormattingString);
            m_writer.WriteAttributeString("instr", DocxConstants.W_namespace, instr.ToString());
        }
        /// <summary>
        /// Gets the text format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private string GetTextFormat(TextFormat format)
        {
            string value = "";
            switch (format)
            {
                case TextFormat.Uppercase:
                    value = "Upper";
                    break;
                case TextFormat.Lowercase:
                    value = "Lower";
                    break;
                case TextFormat.Titlecase:
                    value = "Caps";
                    break;
                case TextFormat.FirstCapital:
                    value = "FirstCap";
                    break;
            }
            return value;
        }
        /// <summary>
        /// Serialize the merge field text.
        /// </summary>
        /// <param name="mergeField"></param>
        private void SerializeMergeFieldText(WMergeField mergeField)
        {
            if (string.IsNullOrEmpty(mergeField.Text))
                return;

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            SerializeCharactetFormat(mergeField.CharacterFormat);

            if (!string.IsNullOrEmpty(mergeField.TextBefore))
            {
                m_writer.WriteStartElement("t", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
                m_writer.WriteString(mergeField.TextBefore);
                m_writer.WriteEndElement();
            }

            try
            {
                if (mergeField.NumberFormat != string.Empty)
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
                    string formattedValue = d.ToString(numberFormat);
                    mergeField.Text = formattedValue;
                }
                else if (mergeField.DateFormat != string.Empty)
                {
                    DateTime dateTime = DateTime.Parse(mergeField.Text);
                    string value = dateTime.ToString(mergeField.DateFormat, DateTimeFormatInfo.CurrentInfo);
                    mergeField.Text = value;
                }
            }
            catch
            {
            }
            string textToDisplay = ModifyText(mergeField.Text);
            int crIndex = textToDisplay.IndexOf(ControlChar.ParagraphBreak);
            if (crIndex != -1)
            {
                //Serializes the text with carriage return characters.
                if (!string.IsNullOrEmpty(mergeField.TextAfter))
                {
                    mergeField.Text += mergeField.TextAfter;
                    mergeField.TextAfter = string.Empty;
                }
                SerializeTextRange(mergeField);
            }
            else
            {
                SerializeText(mergeField.Text, mergeField.CharacterFormat.IsDeleteRevision);

                if (!string.IsNullOrEmpty(mergeField.TextAfter))
                {
                    m_writer.WriteStartElement("t", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
                    m_writer.WriteString(mergeField.TextAfter);
                    m_writer.WriteEndElement();
                }
            }
            m_writer.WriteEndElement();
        }
        #endregion MergeField

        #region FieldMark
        /// <summary>
        /// Serialize the field mark.
        /// </summary>
        /// <param name="fieldMark"></param>
        private void SerializeFieldMark(WFieldMark fieldMark)
        {
            WField parentField = GetParentField(fieldMark) as WField;

            bool writeHyperlinkField = false;
            if (parentField != null && parentField.FieldType == FieldType.FieldHyperlink)
            {
                writeHyperlinkField = CheckHyperlink(parentField) && parentField.FieldCode != string.Empty;
            }
            if (parentField == null || (parentField != null &&
                (parentField.FieldType != FieldType.FieldDatabase &&
                parentField.FieldType != FieldType.FieldSkipIf &&
                parentField.FieldType != FieldType.FieldShape &&
                (parentField.FieldType != FieldType.FieldHyperlink
                || writeHyperlinkField) &&
                parentField.FieldType != FieldType.FieldFileName &&
                parentField.FieldType != FieldType.FieldIncludePicture)))
            {

                m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
                if (fieldMark.CharacterFormat.IsDefault && parentField != null)
                {
                    fieldMark.CharacterFormat.ImportContainer(parentField.CharacterFormat);
                    fieldMark.CharacterFormat.CopyProperties(parentField.CharacterFormat);
                }
                WCharacterFormat format = (CurrentField != null) ? CurrentField.CharacterFormat : fieldMark.CharacterFormat;
                SerializeCharactetFormat(format);
                m_writer.WriteStartElement("w", "fldChar", DocxConstants.W_namespace);
                string charType = fieldMark.Type == FieldMarkType.FieldSeparator ? "separate" : "end";
                m_writer.WriteAttributeString("w", "fldCharType", DocxConstants.W_namespace, charType);
                m_writer.WriteEndElement();//end of fldChar
                m_writer.WriteEndElement();//end of run

            }
            if (fieldMark.Type == FieldMarkType.FieldEnd && m_nonSupportedFields != null)
            {
                if (m_nonSupportedFields.Count > 0)
                    m_nonSupportedFields.Pop();
            }
            if (fieldMark.Type == FieldMarkType.FieldEnd && parentField != null
                && CheckHyperlink(parentField) && parentField.FieldCode == string.Empty)
                m_writer.WriteEndElement(); //end of Hyperlink Field
            if (fieldMark.Type == FieldMarkType.FieldEnd && CurrentField != null)
                FieldStack.Pop();
        }
        /// <summary>
        /// Check whether it is a nested item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsNestedItem(ParagraphItem item)
        {
            WField parentField = GetParentField(item) as WField;

            if (parentField != null)
            {
                if (parentField.FieldType == FieldType.FieldHyperlink)
                {
                    return (parentField.FieldCode == parentField.NestedFieldCode && !CheckHyperlink(parentField));
                }
                else
                    return IsNestedItem(parentField);
            }
            else
                return false;
        }

        #endregion FieldMark

        #region Picture
        /// <summary>
        /// Serialize the picture.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializePicture(WPicture picture)
        {
            if (picture.SkipDocxItem)
                return;

            bool isBulletPicture = IsPictureBullet(picture);

            if (!isBulletPicture && picture.Width > 0 && picture.Height > 0)
            {

                m_writer.WriteStartElement("r", DocxConstants.W_namespace);
                SerializeCharactetFormat(picture.PictureCharacterFormat);

                if (picture.IsShape)
                    SerializeShape(picture);
                else
                    SerializeDrawing(picture);

                m_writer.WriteEndElement();//end of run element

            }
        }
        /// <summary>
        /// Check whether the picture is picturebullet
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <returns></returns>
        private bool IsPictureBullet(WPicture picture)
        {
            ParagraphItemCollection paraItems = null;
            if (picture.Owner is WParagraph)
                paraItems = (picture.Owner as WParagraph).Items;
            else if (picture.Owner is SDTInlineContent)
                paraItems = (picture.Owner as SDTInlineContent).ParagraphItems;

            if (paraItems != null)
            {
                foreach (ParagraphItem parItem in paraItems)
                {
                    BookmarkStart picBulletBookmark = parItem as BookmarkStart;

                    if (picBulletBookmark != null && picBulletBookmark.Name == "_PictureBullets")
                        return true;
                }
            }
            return false;

        }
        /// <summary>
        /// Serialize the drawing element.
        /// </summary>
        /// <param name="picture">The Picture.</param>
        private void SerializeDrawing(WPicture picture)
        {
            if (picture.ImageRecord == null)
                return;

            m_writer.WriteStartElement("drawing", DocxConstants.W_namespace);

            if (picture.TextWrappingStyle == TextWrappingStyle.Behind)
                picture.IsBelowText = true;

            if (picture.TextWrappingStyle != TextWrappingStyle.Inline)
            {
                SerializeAbsolutePicture(picture);
            }
            else
            {
                SerializeInlinePicture(picture);
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the inline picture.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializeInlinePicture(WPicture picture)
        {
            m_writer.WriteStartElement("inline", DocxConstants.WP_namespace);
            m_writer.WriteStartElement("extent", DocxConstants.WP_namespace);
            int cx = (int)Math.Round(((picture.Width * picture.WidthScale) / 100) * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cx", cx.ToString());
            int cy = (int)Math.Round(((picture.Height * picture.HeightScale) / 100) * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cy", cy.ToString());
            m_writer.WriteEndElement();
            double borderWidth = (double)picture.PictureShape.PictureDescriptor.BorderLeft.LineWidth / DLSConstants.BorderLineFactor;
            if (borderWidth > 0 && picture.DocxProps.Count == 0)
            {
                long leftTop = 0, rightBottom = 0;
                picture.PictureShape.GetEffectExtent(borderWidth, ref leftTop, ref rightBottom);
                m_writer.WriteStartElement("effectExtent", DocxConstants.WP_namespace);
                m_writer.WriteAttributeString("l", leftTop.ToString());
                m_writer.WriteAttributeString("t", leftTop.ToString());
                m_writer.WriteAttributeString("r", rightBottom.ToString());
                m_writer.WriteAttributeString("b", rightBottom.ToString());
                m_writer.WriteEndElement();
            }
            SerializePicProperties(picture);

            SerializeDrawingGraphics(picture);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializze the absolutely positioned picture.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializeAbsolutePicture(WPicture picture)
        {
            m_writer.WriteStartElement("anchor", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("distT", (picture.DistanceFromTop * DLSConstants.EmusPerPoint).ToString());
            m_writer.WriteAttributeString("distB", (picture.DistanceFromBottom * DLSConstants.EmusPerPoint).ToString());
            m_writer.WriteAttributeString("distL", (picture.DistanceFromLeft * DLSConstants.EmusPerPoint).ToString());
            m_writer.WriteAttributeString("distR", (picture.DistanceFromRight * DLSConstants.EmusPerPoint).ToString());
            m_writer.WriteAttributeString("simplePos", "0");
            if (picture.OrderIndex != int.MaxValue)
                m_writer.WriteAttributeString("relativeHeight", picture.OrderIndex.ToString());
            else
                m_writer.WriteAttributeString("relativeHeight", "0");
            string isBelowText = (picture.IsBelowText) ? "1" : "0";
            m_writer.WriteAttributeString("behindDoc", isBelowText);
            m_writer.WriteAttributeString("locked", "0");
            if (picture.LayoutInCell)
                m_writer.WriteAttributeString("layoutInCell", "1");
            else
                m_writer.WriteAttributeString("layoutInCell", "0");
            if (picture.AllowOverlap)
                m_writer.WriteAttributeString("allowOverlap", "1");
            else
                m_writer.WriteAttributeString("allowOverlap", "0");

            m_writer.WriteStartElement("simplePos", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("x", "0");
            m_writer.WriteAttributeString("y", "0");
            m_writer.WriteEndElement(); //end of simplePos

            m_writer.WriteStartElement("positionH", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("relativeFrom", picture.HorizontalOrigin.ToString().ToLower());

            if (picture.HorizontalAlignment == ShapeHorizontalAlignment.None)
            {
                m_writer.WriteStartElement("posOffset", DocxConstants.WP_namespace);
                int horPos = (int)Math.Round(picture.HorizontalPosition * DLSConstants.EmusPerPoint);
                m_writer.WriteString(horPos.ToString());
                m_writer.WriteEndElement(); //end of posOffset
            }
            else
            {
                m_writer.WriteStartElement("align", DocxConstants.WP_namespace);
                string horAlig = picture.HorizontalAlignment.ToString().ToLower();
                m_writer.WriteString(horAlig);
                m_writer.WriteEndElement(); //end of align
            }
            m_writer.WriteEndElement();//end of postionH

            m_writer.WriteStartElement("positionV", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("relativeFrom", picture.VerticalOrigin.ToString().ToLower());
            if (picture.VerticalAlignment == ShapeVerticalAlignment.None)
            {
                m_writer.WriteStartElement("posOffset", DocxConstants.WP_namespace);
                int vertPos = (int)Math.Round(picture.VerticalPosition * DLSConstants.EmusPerPoint);
                m_writer.WriteString(vertPos.ToString());
                m_writer.WriteEndElement(); // end of posOffset
            }
            else
            {
                m_writer.WriteStartElement("align", DocxConstants.WP_namespace);
                string verAlig = picture.VerticalAlignment.ToString().ToLower();
                m_writer.WriteString(verAlig);
                m_writer.WriteEndElement(); //end of align
            }
            m_writer.WriteEndElement(); //end of postionV

            m_writer.WriteStartElement("extent", DocxConstants.WP_namespace);
            int cx = (int)Math.Round(((picture.Width * picture.WidthScale) / 100) * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cx", cx.ToString());
            int cy = (int)Math.Round(((picture.Height * picture.HeightScale) / 100) * DLSConstants.EmusPerPoint);
            m_writer.WriteAttributeString("cy", cy.ToString());
            m_writer.WriteEndElement(); //end of extent

            SerializePicProperties(picture);

            switch (picture.TextWrappingStyle)
            {
                case TextWrappingStyle.Square:
                    m_writer.WriteStartElement("wrapSquare", DocxConstants.WP_namespace);
                    m_writer.WriteAttributeString("wrapText", GetPictureWrappingTypeAsString(picture.TextWrappingType));
                    m_writer.WriteEndElement();
                    break;
                case TextWrappingStyle.Through:
                    m_writer.WriteStartElement("wrapThrough", DocxConstants.WP_namespace);
                    m_writer.WriteAttributeString("wrapText", GetPictureWrappingTypeAsString(picture.TextWrappingType));
                    SerializeWrapPolygon(picture, picture.WrapPolygon);
                    m_writer.WriteEndElement();
                    break;
                case TextWrappingStyle.Tight:
                    m_writer.WriteStartElement("wrapTight", DocxConstants.WP_namespace);
                    m_writer.WriteAttributeString("wrapText", GetPictureWrappingTypeAsString(picture.TextWrappingType));
                    SerializeWrapPolygon(picture, picture.WrapPolygon);
                    m_writer.WriteEndElement();
                    break;
                case TextWrappingStyle.TopAndBottom:
                    m_writer.WriteStartElement("wrapTopAndBottom", DocxConstants.WP_namespace);
                    m_writer.WriteEndElement();
                    break;
                default:
                    m_writer.WriteStartElement("wrapNone", DocxConstants.WP_namespace);
                    m_writer.WriteEndElement();
                    break;
            }

            SerializeDrawingGraphics(picture);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the picture properties
        /// </summary>
        /// <param name="picture"></param>
        private void SerializePicProperties(WPicture picture)
        {
            if (picture.DocxProps.Count > 0)
            {
                foreach (Stream stream in picture.DocxProps)
                {
                    XmlReader reader = CreateReader(stream);
                    m_writer.WriteNode(reader, false);
                }
            }
        }
        /// <summary>
        /// Serialize the graphics element for pictures.
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="multiplier"></param>
        private void SerializeDrawingGraphics(WPicture picture)
        {
            string id = string.Empty;

            id = UpdateShapeId(picture, false, null);
            picture.ShapeId = GetNextDocPrID();
            // Processing picture
            m_writer.WriteStartElement("wp", "docPr", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("id", picture.ShapeId.ToString());
            m_writer.WriteAttributeString("name", "");
            if (picture.AlternativeText != null)
                m_writer.WriteAttributeString("descr", picture.AlternativeText);
            SerializePictureHyperlink(picture);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("a", "graphic", DocxConstants.A_namespace);
            m_writer.WriteStartElement("a", "graphicData", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("uri", DocxConstants.PIC_namespace);
            m_writer.WriteStartElement("pic", "pic", DocxConstants.PIC_namespace);
            m_writer.WriteStartElement("pic", "nvPicPr", DocxConstants.PIC_namespace);
            m_writer.WriteStartElement("pic", "cNvPr", DocxConstants.PIC_namespace);
            m_writer.WriteAttributeString("id", "0");
            m_writer.WriteAttributeString("name", "");
            m_writer.WriteAttributeString("descr", "");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("pic", "cNvPicPr", DocxConstants.PIC_namespace);
            m_writer.WriteStartElement("a", "picLocks", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("noChangeAspect", "1");
            m_writer.WriteAttributeString("noChangeArrowheads", "1");
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("pic", "blipFill", DocxConstants.PIC_namespace);
            m_writer.WriteStartElement("a", "blip", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("r", "embed", DocxConstants.R_namespace, id);
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("a","srcRect", DocxConstants.A_namespace);
            if (picture.FillRectangle.LeftOffset != 0)
                m_writer.WriteAttributeString("l", (picture.FillRectangle.LeftOffset * DLSConstants.ThousandthsUnit).ToString());
            if (picture.FillRectangle.TopOffset != 0)
                m_writer.WriteAttributeString("t", (picture.FillRectangle.TopOffset * DLSConstants.ThousandthsUnit).ToString());
            if (picture.FillRectangle.RightOffset != 0)
                m_writer.WriteAttributeString("r", (picture.FillRectangle.RightOffset * DLSConstants.ThousandthsUnit).ToString());
            if (picture.FillRectangle.BottomOffset != 0)
                m_writer.WriteAttributeString("b", (picture.FillRectangle.BottomOffset * DLSConstants.ThousandthsUnit).ToString());
            m_writer.WriteEndElement();            
            m_writer.WriteStartElement("a", "stretch", DocxConstants.A_namespace);
            m_writer.WriteStartElement("a", "fillRect", DocxConstants.A_namespace);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("pic", "spPr", DocxConstants.PIC_namespace);
            m_writer.WriteAttributeString("bwMode", "auto");
            m_writer.WriteStartElement("a", "xfrm", DocxConstants.A_namespace);
            m_writer.WriteStartElement("a", "off", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("x", "0");
            m_writer.WriteAttributeString("y", "0");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("a", "ext", DocxConstants.A_namespace);
            int cx = (int)Math.Round((((picture.Width * picture.WidthScale) / 100) * DLSConstants.EmusPerPoint));
            m_writer.WriteAttributeString("cx", cx.ToString());
            int cy = (int)Math.Round((((picture.Height * picture.HeightScale) / 100) * DLSConstants.EmusPerPoint));
            m_writer.WriteAttributeString("cy", cy.ToString());
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("a", "prstGeom", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("prst", "rect");
            m_writer.WriteStartElement("a", "avLst", DocxConstants.A_namespace);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            if (picture.HasBorder)
            {
                if (picture.TextWrappingStyle == TextWrappingStyle.Inline)
                    SerializeInlineShapeLine(picture.PictureShape);
                else
                    SerializeShapeLine(picture.PictureShape);
            }
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the inline shape line.
        /// </summary>
        /// <param name="shape">The shape.</param>
        private void SerializeInlineShapeLine(InlineShapeObject shape)
        {
            BorderCode border = shape.PictureDescriptor.BorderLeft;
            TextBoxLineStyle lineStyle = TextBoxLineStyle.Simple;
            LineDashing dashing = shape.GetDashStyle((BorderStyle)border.BorderType, ref lineStyle);
            m_writer.WriteStartElement("ln", DocxConstants.A_namespace);
            if (lineStyle != TextBoxLineStyle.Simple)
                m_writer.WriteAttributeString("cmpd", GetLineStyle(lineStyle, false));
            uint width = (uint)Math.Round(((double)border.LineWidth / DLSConstants.BorderLineFactor) * DLSConstants.EmusPerPoint);
            if (width > 0)
                m_writer.WriteAttributeString("w", width.ToString());
            if (shape.LineGradient.GradientStops.Count > 0)
                SerializeGradientFill(shape.LineGradient);
            else
            {
                Color color = shape.PictureDescriptor.BorderLeft.LineColorExt;
                if (shape.ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderLeftColor))
                    color = WordColor.ConvertRGBToColor(shape.ShapeContainer.ShapePosition.GetPropertyValue((int)FOPTEGroupShape.borderLeftColor));
                if (color.IsEmpty)
                {
                    m_writer.WriteStartElement("noFill", DocxConstants.A_namespace);
                    m_writer.WriteEndElement();
                }
                else
                {
                    m_writer.WriteStartElement("solidFill", DocxConstants.A_namespace);
                    m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
                    m_writer.WriteAttributeString("val", GetRGBCode(color));
                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                }
            }
            if (border.BorderType != (byte)BorderStyle.None)
            {
                m_writer.WriteStartElement("prstDash", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("val", GetDashStyle(dashing,false));
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Gets the line cap style.
        /// </summary>
        /// <param name="lineCap">The line cap.</param>
        /// <returns></returns>
        private string GetLineCapStyle(LineCap lineCap, bool is2007)
        {
            switch (lineCap)
            {
                case LineCap.Flat:
                    return "flat";
                case LineCap.Round:
                    return is2007 ? "round" : "rnd";
                default:
                    return is2007 ? "square" : "sq";
            }
        }
        /// <summary>
        /// Gets the line join style.
        /// </summary>
        /// <param name="linejoin">The linejoin.</param>
        /// <returns></returns>
        private string GetLineJoinStyle(LineJoin linejoin)
        {
            switch (linejoin)
            {
                case LineJoin.Bevel:
                    return "bevel";
                case LineJoin.Round:
                    return "round";
                default:
                    return "miter";
            }
        }
        /// <summary>
        /// Gets the line end.
        /// </summary>
        /// <param name="lineEnd">The line end.</param>
        /// <returns></returns>
        private string GetLineEnd(LineEnd lineEnd, bool is2007)
        {
            switch (lineEnd)
            {
                case LineEnd.ArrowEnd:
                    return is2007 ? "block" : "triangle";
                case LineEnd.ArrowOpenEnd:
                    return is2007 ? "open" : "arrow";
                case LineEnd.ArrowOvalEnd:
                    return "oval";
                case LineEnd.ArrowStealthEnd:
                    return is2007 ? "classic" : "stealth";
                case LineEnd.ArrowDiamondEnd:
                    return "diamond";
                default:
                    return "none";
            }
        }
        /// <summary>
        /// Gets the end width of the line.
        /// </summary>
        /// <param name="lineEndWidth">End width of the line.</param>
        /// <returns></returns>
        private string GetLineEndWidth(LineEndWidth lineEndWidth, bool is2007)
        {
            switch (lineEndWidth)
            {
                case LineEndWidth.NarrowArrow:
                    return is2007 ? "narrow" : "sm";
                case LineEndWidth.WideArrow:
                    return is2007 ? "wide" : "lg";
                default:
                    return is2007 ? "medium" : "med";
            }
        }
        /// <summary>
        /// Gets the end length of the line.
        /// </summary>
        /// <param name="lineEndLength">End length of the line.</param>
        /// <returns></returns>
        private string GetLineEndLength(LineEndLength lineEndLength, bool is2007)
        {
            switch (lineEndLength)
            {
                case LineEndLength.ShortArrow:
                    return is2007 ? "short" : "sm";
                case LineEndLength.LongArrow:
                    return is2007 ? "long" : "lg";
                default:
                    return is2007 ? "medium" : "med";
            }
        }
        /// <summary>
        /// Serializes the shape line.
        /// </summary>
        /// <param name="shape">The shape.</param>
        private void SerializeShapeLine(InlineShapeObject shape)
        {
            m_writer.WriteStartElement("ln", DocxConstants.A_namespace);
            if (shape.ShapeContainer.ShapeOptions.LineProperties.HasDefined
                && shape.ShapeContainer.ShapeOptions.LineProperties.PenAlignInset)
                m_writer.WriteAttributeString("algn", "in");
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndCapStyle))
                m_writer.WriteAttributeString("cap", GetLineCapStyle((LineCap)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineEndCapStyle),false));
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStyle))
                m_writer.WriteAttributeString("cmpd", GetLineStyle((TextBoxLineStyle)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineStyle),false));
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineWidth))
                m_writer.WriteAttributeString("w", shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineWidth).ToString());
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineColor))
            {
                m_writer.WriteStartElement("solidFill", DocxConstants.A_namespace);
                m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
                Color color = WordColor.ConvertRGBToColor(shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineColor));
                m_writer.WriteAttributeString("val", GetRGBCode(color));
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineOpacity))
                {
                    uint alpha = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineOpacity);
                    alpha = (uint)Math.Round((alpha * DLSConstants.HundredthsUnit) / (double)DLSConstants.FixedPointsUnit);
                    alpha = alpha * DLSConstants.ThousandthsUnit;
                    m_writer.WriteStartElement("alpha", DocxConstants.A_namespace);
                    m_writer.WriteAttributeString("val", alpha.ToString());
                    m_writer.WriteEndElement();
                }
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }
            else if (shape.LineGradient.GradientStops.Count > 0)
            {
                SerializeGradientFill(shape.LineGradient);
            }
            else
            {
                m_writer.WriteStartElement("noFill", DocxConstants.A_namespace);
                m_writer.WriteEndElement();
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineDashing))
            {
                m_writer.WriteStartElement("prstDash", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("val", GetDashStyle((LineDashing)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineDashing),false));
                m_writer.WriteEndElement();
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineJoinStyle))
            {
                LineJoin lineJoin = (LineJoin)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineJoinStyle);
                m_writer.WriteStartElement(GetLineJoinStyle(lineJoin), DocxConstants.A_namespace);
                if (lineJoin == LineJoin.Miter
                    && shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineMiterLimit))
                {
                    uint miterLimit = shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineMiterLimit);
                    miterLimit = (uint)(((double)miterLimit / DLSConstants.FixedPointsUnit) * DLSConstants.HundredthsUnit * DLSConstants.ThousandthsUnit);
                    m_writer.WriteAttributeString("lim", miterLimit.ToString());
                }
                m_writer.WriteEndElement();
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowhead)
                || shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowLength)
                || shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowWidth))
            {
                m_writer.WriteStartElement("headEnd", DocxConstants.A_namespace);
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowhead))
                {
                    string lineStartArrowhead = GetLineEnd((LineEnd)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineStartArrowhead), false);
                    if (lineStartArrowhead != null)
                        m_writer.WriteAttributeString("type", lineStartArrowhead);
                }
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowWidth))
                {
                    LineEndWidth lineEndWidth = (LineEndWidth)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineStartArrowWidth);
                    m_writer.WriteAttributeString("w", GetLineEndWidth(lineEndWidth, false));
                }
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowLength))
                {
                    LineEndLength lineEndLength = (LineEndLength)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineStartArrowLength);
                    m_writer.WriteAttributeString("len", GetLineEndLength(lineEndLength, false));
                }
                m_writer.WriteEndElement();
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowhead)
                || shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowLength)
                || shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowWidth))
            {
                m_writer.WriteStartElement("tailEnd", DocxConstants.A_namespace);
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowhead))
                {
                    string lineEndArrowhead = GetLineEnd((LineEnd)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineEndArrowhead), false);
                    if (lineEndArrowhead != null)
                        m_writer.WriteAttributeString("type", lineEndArrowhead);
                }
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowWidth))
                {
                    LineEndWidth lineEndWidth = (LineEndWidth)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineEndArrowWidth);
                    m_writer.WriteAttributeString("w", GetLineEndWidth(lineEndWidth, false));
                }
                if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowLength))
                {
                    LineEndLength lineEndLength = (LineEndLength)shape.ShapeContainer.GetPropertyValue((int)FOPTELineStyle.lineEndArrowLength);
                    m_writer.WriteAttributeString("len", GetLineEndLength(lineEndLength, false));
                }
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the gradient fill.
        /// </summary>
        /// <param name="gradientFill">The gradient fill.</param>
        private void SerializeGradientFill(GradientFill gradientFill)
        {
            m_writer.WriteStartElement("gradFill", DocxConstants.A_namespace);
            if (gradientFill.Flip != FlipOrientation.None)
                m_writer.WriteAttributeString("flip", GetFlipOrientation(gradientFill.Flip));
            m_writer.WriteAttributeString("rotWithShape", (gradientFill.RotateWithShape ? "1" : "0"));

            m_writer.WriteStartElement("gsLst", DocxConstants.A_namespace);
            foreach (GradientStop gradientStop in gradientFill.GradientStops)
            {
                SerializeGradientStop(gradientStop);
            }
            m_writer.WriteEndElement();

            if (gradientFill.LinearGradient != null)
            {
                m_writer.WriteStartElement("lin", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("ang", (gradientFill.LinearGradient.Angle * DLSConstants.SixtyThousandthsUnit).ToString());
                m_writer.WriteAttributeString("scaled", (gradientFill.LinearGradient.Scaled ? "1" : "0"));
                m_writer.WriteEndElement();
            }

            if (gradientFill.PathGradient != null)
            {
                m_writer.WriteStartElement("path", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("path", GetGradientShadeType(gradientFill.PathGradient.PathShade));
                m_writer.WriteStartElement("fillToRect", DocxConstants.A_namespace);
                if (gradientFill.PathGradient.LeftOffset != 0)
                    m_writer.WriteAttributeString("l", (gradientFill.PathGradient.LeftOffset * DLSConstants.ThousandthsUnit).ToString());
                if (gradientFill.PathGradient.TopOffset != 0)
                    m_writer.WriteAttributeString("t", (gradientFill.PathGradient.TopOffset * DLSConstants.ThousandthsUnit).ToString());
                if (gradientFill.PathGradient.RightOffset != 0)
                    m_writer.WriteAttributeString("r", (gradientFill.PathGradient.RightOffset * DLSConstants.ThousandthsUnit).ToString());
                if (gradientFill.PathGradient.BottomOffset != 0)
                    m_writer.WriteAttributeString("b", (gradientFill.PathGradient.BottomOffset * DLSConstants.ThousandthsUnit).ToString());
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }

            m_writer.WriteStartElement("tileRect", DocxConstants.A_namespace);
            if (gradientFill.TileRectangle.LeftOffset != 0)
                m_writer.WriteAttributeString("l", (gradientFill.TileRectangle.LeftOffset * DLSConstants.ThousandthsUnit).ToString());
            if (gradientFill.TileRectangle.TopOffset != 0)
                m_writer.WriteAttributeString("t", (gradientFill.TileRectangle.TopOffset * DLSConstants.ThousandthsUnit).ToString());
            if (gradientFill.TileRectangle.RightOffset != 0)
                m_writer.WriteAttributeString("r", (gradientFill.TileRectangle.RightOffset * DLSConstants.ThousandthsUnit).ToString());
            if (gradientFill.TileRectangle.BottomOffset != 0)
                m_writer.WriteAttributeString("b", (gradientFill.TileRectangle.BottomOffset * DLSConstants.ThousandthsUnit).ToString());
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the gradient stop.
        /// </summary>
        /// <param name="gradientStop">The gradient stop.</param>
        private void SerializeGradientStop(GradientStop gradientStop)
        {
            m_writer.WriteStartElement("gs", DocxConstants.A_namespace);
            m_writer.WriteAttributeString("pos", (gradientStop.Position * DLSConstants.ThousandthsUnit).ToString());
            if (gradientStop.Color != null)
            {
                m_writer.WriteStartElement("srgbClr", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("val", GetRGBCode(gradientStop.Color));
                if (gradientStop.Opacity != byte.MaxValue)
                {
                    m_writer.WriteStartElement("alpha", DocxConstants.A_namespace);
                    m_writer.WriteAttributeString("val", (gradientStop.Opacity * DLSConstants.ThousandthsUnit).ToString());
                    m_writer.WriteEndElement();
                }
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Gets the flip orientation.
        /// </summary>
        /// <param name="flip">The flip.</param>
        /// <returns></returns>
        private string GetFlipOrientation(FlipOrientation flip)
        {
            switch (flip)
            {
                case FlipOrientation.Horizontal:
                    return "x";
                case FlipOrientation.Vertical:
                    return "y";
                case FlipOrientation.Both:
                    return "xy";
                default:
                    return "none";
            }
        }
        /// <summary>
        /// Gets the type of the gradient shade.
        /// </summary>
        /// <param name="shade">The shade.</param>
        /// <returns></returns>
        private string GetGradientShadeType(GradientShadeType shade)
        {
            switch (shade)
            {
                case GradientShadeType.Circle:
                    return "circle";
                case GradientShadeType.Rectangle:
                    return "rect";
                default:
                    return "shape";
            }
        }
        /// <summary>
        /// Serialize the picture hyperlink.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializePictureHyperlink(WPicture picture)
        {
            IEntity ent = picture;
            for (int i = 0; i < 2; i++)
            {
                ent = ent.PreviousSibling;
                if (ent == null)
                    break;
            }

            if (ent != null && ent is WField)
            {
                WField field = ent as WField;
                if (field.FieldType != FieldType.FieldHyperlink)
                    return;

                m_writer.WriteStartElement("hlinkClick", DocxConstants.A_namespace);
                m_writer.WriteAttributeString("xmlns", "a", null, DocxConstants.A_namespace);
                SerializeHyperlinkAttributes(field);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Update the shape id.
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        private string UpdateShapeId(WPicture picture, bool isOlePicture, WOleObject oleObject)
        {
            string id = string.Empty;
            IEntity owner;
            if (!isOlePicture)
                owner = GetPictureOwner(picture);
            else
                owner = GetOleObjectOwner(oleObject);

            // Adding picture byte data to the corresponding picture collection 
            // depending on its owner subdocument
            if (owner is HeaderFooter)
            {
                id = UpdateHFImageRels(owner as HeaderFooter, picture);
            }
            else if (owner.Owner != null && owner.Owner is HeaderFooter)
            {
                HeaderFooter headerFooter = owner.Owner as HeaderFooter;
                id = UpdateHFImageRels(headerFooter, picture);
            }
            else
            {
                UpdateImages(picture);
                if (owner is WSection || owner is WTextBox || owner is WTableRow || owner is WParagraph || owner is SDTBlockContent || owner is Shape)
                    id = AddImageRelation(DocumentImages, picture.ImageRecord);

                if (owner is WFootnote)
                {
                    if ((owner as WFootnote).FootnoteType == FootnoteType.Footnote)
                        id = AddImageRelation(FootnoteImages, picture.ImageRecord);
                    else
                        id = AddImageRelation(EndnoteImages, picture.ImageRecord);
                }

                if (owner is WComment)
                    id = AddImageRelation(CommentImages, picture.ImageRecord);
            }

            return id;
        }
        /// <summary>
        /// Adds the image relation.
        /// </summary>
        /// <param name="imageCollection">The image collection.</param>
        /// <param name="imageRecord">The image record.</param>
        /// <returns></returns>
        private string AddImageRelation(Dictionary<string, ImageRecord> imageCollection, ImageRecord imageRecord)
        {
            string relationId = string.Empty;
            if (imageCollection.ContainsValue(imageRecord))
            {
                foreach (string key in imageCollection.Keys)
                {
                    if (imageRecord == imageCollection[key])
                    {
                        relationId = key;
                        break;
                    }
                }
            }
            else
            {
                relationId = GetNextRelationShipID();
                imageCollection.Add(relationId, imageRecord);
            }
            return relationId;
        }
        /// <summary>
        /// Update the HeaderFooter image relations.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hf"></param>
        /// <param name="image"></param>
        private void UpdateHFImageRels(string id, HeaderFooter hf, ImageRecord imageRecord)
        {
            string headerId = string.Empty;
            foreach (HeaderFooterType hfType in m_headerFooterColl.Keys)
            {
                Dictionary<string, HeaderFooter> hfColl = m_headerFooterColl[hfType];

                foreach (string key in hfColl.Keys)
                {
                    if (hfColl[key] == hf)
                    {
                        headerId = key;
                        Dictionary<string, ImageRecord> headerImages = null;
                        if (HeaderFooterImages.ContainsKey(headerId))
                        {
                            headerImages = HeaderFooterImages[headerId];
                            headerImages.Add(id, imageRecord);
                        }
                        else
                        {
                            headerImages = new Dictionary<string, ImageRecord>();
                            headerImages.Add(id, imageRecord);
                            HeaderFooterImages.Add(headerId, headerImages);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Update the HeaderFooter image relations.
        /// </summary>
        /// <param name="hf"></param>
        /// <param name="image"></param>
        private string UpdateHFImageRels(HeaderFooter hf, WPicture image)
        {
            UpdateImages(image);
            string id = string.Empty;
            string headerId = string.Empty;
            foreach (HeaderFooterType hfType in m_headerFooterColl.Keys)
            {
                Dictionary<string, HeaderFooter> hfColl = m_headerFooterColl[hfType];

                foreach (string key in hfColl.Keys)
                {
                    if (hfColl[key] == hf)
                    {
                        headerId = key;
                        Dictionary<string, ImageRecord> headerImages = null;
                        if (HeaderFooterImages.ContainsKey(headerId))
                        {
                            headerImages = HeaderFooterImages[headerId];
                            id = AddImageRelation(headerImages, image.ImageRecord);
                        }
                        else
                        {
                            headerImages = new Dictionary<string, ImageRecord>();
                            id = AddImageRelation(headerImages, image.ImageRecord);
                            HeaderFooterImages.Add(headerId, headerImages);
                        }
                    }
                }
            }
            return id;
        }
        /// <summary>
        /// Get the header footer id.
        /// </summary>
        /// <param name="hf"></param>
        /// <returns></returns>
        private string GetHeaderFooterId(HeaderFooter hf)
        {
            foreach (HeaderFooterType hfType in m_headerFooterColl.Keys)
            {
                Dictionary<string, HeaderFooter> hfColl = m_headerFooterColl[hfType];

                foreach (string key in hfColl.Keys)
                {
                    if (hfColl[key] == hf)
                    {
                        return key;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Get the picture owner.
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        private IEntity GetPictureOwner(WPicture pic)
        {
            Entity picOwner = pic.Owner;
            WParagraph ownerPara = null;

            if (pic.Owner is WOleObject)
            {
                picOwner = (pic.Owner as WOleObject).OwnerParagraph;
            }

            if (picOwner.EntityType == EntityType.SDTInlineContent)
                ownerPara = picOwner.Owner.Owner as WParagraph;
            else if (picOwner.EntityType == EntityType.Paragraph)
                ownerPara = picOwner as WParagraph;

            WTableCell ownerCell = ownerPara.Owner as WTableCell;
            Entity ownerEntity = ownerPara.Owner.Owner;
            HeaderFooter ownerHeaderFooter;
            IEntity ownerTextBody;

            if (ownerCell != null)
            {
                ownerTextBody = ownerCell.OwnerRow.OwnerTable.OwnerTextBody;
            }
            else
            {
                ownerTextBody = ownerPara.Owner;
            }

            ownerEntity = ownerTextBody.Owner;

            ownerHeaderFooter = GetBaseEntity(pic) as HeaderFooter;

            if (ownerHeaderFooter != null)
                return ownerHeaderFooter;
            else
                return ownerEntity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oleObject"></param>
        /// <returns></returns>
        private IEntity GetOleObjectOwner(WOleObject oleObject)
        {
            WParagraph ownerPara = oleObject.OwnerParagraph;
            WTableCell ownerCell = ownerPara.Owner as WTableCell;
            Entity ownerEntity = ownerPara.Owner.Owner;
            HeaderFooter ownerHeaderFooter;
            IEntity ownerTextBody;

            if (ownerCell != null)
            {
                ownerTextBody = ownerCell.OwnerRow.OwnerTable.OwnerTextBody;
            }
            else
            {
                ownerTextBody = oleObject.OwnerParagraph.Owner;
            }

            ownerEntity = ownerTextBody.Owner;

            ownerHeaderFooter = GetBaseEntity(oleObject) as HeaderFooter;

            if (ownerHeaderFooter != null)
                return ownerHeaderFooter;
            else
                return ownerEntity;
        }
        /// <summary>
        /// Serialize the shape.
        /// </summary>
        /// <param name="picture"></param>
        private void SerializeShape(WPicture picture)
        {
            string id = UpdateShapeId(picture, false, null);

            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);
            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("type", "#_x0000_t75");

            string style = SerializeShapePictStyle(picture);
            m_writer.WriteAttributeString("style", style);
            if (picture.HasBorder
                && picture.TextWrappingStyle == TextWrappingStyle.Inline)
                SerializeShapeBorderColor(picture.PictureShape);
            m_writer.WriteStartElement("imagedata", DocxConstants.V_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id);
            if (picture.Title != null)
                m_writer.WriteAttributeString("title", DocxConstants.O_namespace, picture.Title);
            m_writer.WriteEndElement();

            SerializeWrapping(picture.TextWrappingStyle, picture.TextWrappingType);
            if (picture.HasBorder)
            {
                if (picture.TextWrappingStyle == TextWrappingStyle.Inline)
                    SerializeShapeBorders(picture.PictureShape.PictureDescriptor);
                else
                    SerializeStrokeProps(picture.PictureShape);
            }
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the color of the shape border.
        /// </summary>
        /// <param name="shape">The shape.</param>
        private void SerializeShapeBorderColor(InlineShapeObject shape)
        {
            Color bottomColor = shape.PictureDescriptor.BorderBottom.LineColorExt;
            Color leftColor = shape.PictureDescriptor.BorderLeft.LineColorExt;
            Color rightColor = shape.PictureDescriptor.BorderRight.LineColorExt;
            Color topColor = shape.PictureDescriptor.BorderTop.LineColorExt;
            if (shape.ShapeContainer != null
                && shape.ShapeContainer.ShapePosition != null)
            {
                if (shape.ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderLeftColor))
                    leftColor = WordColor.ConvertRGBToColor(shape.ShapeContainer.ShapePosition.GetPropertyValue((int)FOPTEGroupShape.borderLeftColor));
                if (shape.ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderRightColor))
                    rightColor = WordColor.ConvertRGBToColor(shape.ShapeContainer.ShapePosition.GetPropertyValue((int)FOPTEGroupShape.borderRightColor));
                if (shape.ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderTopColor))
                    topColor = WordColor.ConvertRGBToColor(shape.ShapeContainer.ShapePosition.GetPropertyValue((int)FOPTEGroupShape.borderTopColor));
                if (shape.ShapeContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.borderBottomColor))
                    bottomColor = WordColor.ConvertRGBToColor(shape.ShapeContainer.ShapePosition.GetPropertyValue((int)FOPTEGroupShape.borderBottomColor));
            }
            m_writer.WriteAttributeString("bordertopcolor", DocxConstants.O_namespace, "#" + GetRGBCode(topColor));
            m_writer.WriteAttributeString("borderleftcolor", DocxConstants.O_namespace, "#" + GetRGBCode(leftColor));
            m_writer.WriteAttributeString("borderbottomcolor", DocxConstants.O_namespace, "#" + GetRGBCode(bottomColor));
            m_writer.WriteAttributeString("borderrightcolor", DocxConstants.O_namespace, "#" + GetRGBCode(rightColor));
        }
        /// <summary>
        /// Serializes the stroke props.
        /// </summary>
        /// <param name="shape">The shape.</param>
        private void SerializeStrokeProps(InlineShapeObject shape)
        {
            m_writer.WriteStartElement("stroke", DocxConstants.V_namespace);

            if (shape.ShapeContainer.ShapeOptions.LineProperties.HasDefined)
            {
                if (shape.ShapeContainer.ShapeOptions.LineProperties.UsefLine
                    && shape.ShapeContainer.ShapeOptions.LineProperties.Line)
                    m_writer.WriteAttributeString("on", "t");
                else
                    m_writer.WriteAttributeString("on", "f");
                if (shape.ShapeContainer.ShapeOptions.LineProperties.PenAlignInset)
                    m_writer.WriteAttributeString("insetpen", "t");

                if (shape.ShapeContainer.ShapeOptions.LineProperties.UsefNoLineDrawDash
                    && shape.ShapeContainer.ShapeOptions.LineProperties.NoLineDrawDash)
                    m_writer.WriteAttributeString("forcedash", "t");
            }
            uint value;
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineColor))
            {
                Color color = WordColor.ConvertRGBToColor(shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineColor));
                m_writer.WriteAttributeString("color", "#" + GetRGBCode(color));
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineOpacity))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineOpacity);
                m_writer.WriteAttributeString("opacity", value.ToString() + "f");
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineDashing))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineDashing);
                m_writer.WriteAttributeString("dashstyle", GetStrokeDashStyle((LineDashing)value));
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineJoinStyle))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineJoinStyle);
                m_writer.WriteAttributeString("joinstyle", GetLineJoinStyle((LineJoin)value));
                if (value == (uint)LineJoin.Miter
                    && shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineMiterLimit))
                {
                    value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineMiterLimit);
                    m_writer.WriteAttributeString("miterlimit", value.ToString() + "f");
                }
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStyle))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineStyle);
                m_writer.WriteAttributeString("linestyle", GetStrokeLineStyle((TextBoxLineStyle)value));
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndCapStyle))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineEndCapStyle);
                m_writer.WriteAttributeString("endcap", GetLineCapStyle((LineCap)value,false));
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineWidth))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineWidth);
                double lineWidth = (double)Math.Round((double)value / DLSConstants.EmusPerPoint, 2);
                m_writer.WriteAttributeString("weight", lineWidth.ToString(CultureInfo.InvariantCulture) + "pt");
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowhead))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineStartArrowhead);
                m_writer.WriteAttributeString("startarrow", GetLineEnd((LineEnd)value, true));
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowWidth))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineStartArrowWidth);
                m_writer.WriteAttributeString("startarrowwidth", GetLineEndWidth((LineEndWidth)value, true));
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineStartArrowLength))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineStartArrowLength);
                m_writer.WriteAttributeString("startarrowlength", GetLineEndLength((LineEndLength)value, true));
            }

            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowhead))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineEndArrowhead);
                m_writer.WriteAttributeString("endarrow", GetLineEnd((LineEnd)value, true));
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowWidth))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineEndArrowWidth);
                m_writer.WriteAttributeString("endarrowwidth", GetLineEndWidth((LineEndWidth)value, true));
            }
            if (shape.ShapeContainer.ShapeOptions.Properties.ContainsKey((int)FOPTELineStyle.lineEndArrowLength))
            {
                value = shape.ShapeContainer.ShapeOptions.GetPropertyValue((int)FOPTELineStyle.lineEndArrowLength);
                m_writer.WriteAttributeString("endarrowlength", GetLineEndLength((LineEndLength)value, true));
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Gets the stroke line style.
        /// </summary>
        /// <param name="lineStyle">The line style.</param>
        /// <returns></returns>
        private string GetStrokeLineStyle(TextBoxLineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case TextBoxLineStyle.Double:
                    return "thinThin";
                case TextBoxLineStyle.ThinThick:
                    return "thinThick";
                case TextBoxLineStyle.ThickThin:
                    return "thickThin";
                case TextBoxLineStyle.Triple:
                    return "thickBetweenThin";
                default:
                    return "single";
            }
        }
        /// <summary>
        /// Gets the stroke dash style.
        /// </summary>
        /// <param name="lineDashing">The line dashing.</param>
        /// <returns></returns>
        private string GetStrokeDashStyle(LineDashing lineDashing)
        {
            switch (lineDashing)
            {
                case LineDashing.Dash:
                case LineDashing.DashGEL:
                    return "dash";
                case LineDashing.DashDotGEL:
                    return "dashDot";
                case LineDashing.Dot:
                case LineDashing.DotGEL:
                    return "1 1";
                case LineDashing.LongDashGEL:
                    return "longDash";
                case LineDashing.DashDot:
                case LineDashing.LongDashDotGEL:
                    return "longDashDot";
                case LineDashing.DashDotDot:
                case LineDashing.LongDashDotDotGEL:
                    return "longDashDotDot";
                default:
                    return "solid";
            }
        }
        /// <summary>
        /// Serialize the text wrapping style.
        /// </summary>
        /// <param name="wrapStyle"></param>
        /// <param name="wrapType"></param>
        private void SerializeWrapping(TextWrappingStyle wrapStyle, TextWrappingType wrapType)
        {
            //<w10:wrap type="tight" />
            if (wrapStyle == TextWrappingStyle.InFrontOfText)
                return;

            m_writer.WriteStartElement("wrap", DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("type", GetWrappingStyleAsString(wrapStyle));
            if (wrapType != TextWrappingType.Both)
            {
                m_writer.WriteAttributeString("side", GetWrappingTypeAsString(wrapType));
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the wrap polygon.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="wrapPolygon">The wrap polygon.</param>
        private void SerializeWrapPolygon(Entity entity,WrapPolygon wrapPolygon)
        {
            m_writer.WriteStartElement("wrapPolygon",DocxConstants.WP_namespace);
            string value = (wrapPolygon.Edited) ? "1" : "0";
            m_writer.WriteAttributeString("edited",  value);
            m_writer.WriteStartElement("start", DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("x",  wrapPolygon.Vertices[0].X.ToString(CultureInfo.InvariantCulture));
            m_writer.WriteAttributeString("y", wrapPolygon.Vertices[0].Y.ToString(CultureInfo.InvariantCulture));
            m_writer.WriteEndElement();
            for (int i = 1; i < wrapPolygon.Vertices.Count; i++)
            {
                m_writer.WriteStartElement("lineTo", DocxConstants.WP_namespace);
                m_writer.WriteAttributeString("x", wrapPolygon.Vertices[i].X.ToString(CultureInfo.InvariantCulture));
                m_writer.WriteAttributeString("y", wrapPolygon.Vertices[i].Y.ToString(CultureInfo.InvariantCulture));
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Get the text wrapping style.
        /// </summary>
        /// <param name="wrapStyle"></param>
        /// <returns></returns>
        private string GetWrappingStyleAsString(TextWrappingStyle wrapStyle)
        {
            switch (wrapStyle)
            {
                case TextWrappingStyle.Square:
                    return "square";
                    break;
                case TextWrappingStyle.Tight:
                    return "tight";
                    break;
                case TextWrappingStyle.Through:
                    return "through";
                    break;
                case TextWrappingStyle.TopAndBottom:
                    return "topAndBottom";
                    break;
                case TextWrappingStyle.Inline:
                default:
                    return "none";
            }
        }
        /// <summary>
        /// Gets the vertical orgin as string.
        /// </summary>
        /// <param name="verticalOrigin">The vertical origin.</param>
        /// <returns></returns>
        private string GetVerticalOrginAsString(VerticalOrigin verticalOrigin)
        {
            string origin = string.Empty;
            switch (verticalOrigin)
            {
                case VerticalOrigin.TopMargin:
                    origin = "top-margin-area";
                    break;
                case VerticalOrigin.BottomMargin:
                    origin = "bottom-margin-area";
                    break;
                case VerticalOrigin.InsideMargin:
                    origin = "inner-margin-area";
                    break;
                case VerticalOrigin.OutsideMargin:
                    origin = "outer-margin-area";
                    break;
                default :
                    origin = verticalOrigin.ToString().ToLower();
                    break;
            }
            return origin;
        }
        /// <summary>
        /// Get the text wrapping type
        /// </summary>
        /// <param name="wrapType"></param>
        /// <returns></returns>
        private string GetWrappingTypeAsString(TextWrappingType wrapType)
        {
            switch (wrapType)
            {
                case TextWrappingType.Left:
                    return "left";
                case TextWrappingType.Right:
                    return "right";
                case TextWrappingType.Largest:
                    return "largest";
                default:
                    return "both";
            }
        }
        /// <summary>
        /// Get the picture's wrapping type.
        /// </summary>
        /// <param name="wrapType"></param>
        /// <returns></returns>
        private string GetPictureWrappingTypeAsString(TextWrappingType wrapType)
        {
            switch (wrapType)
            {
                case TextWrappingType.Left:
                    return "left";
                case TextWrappingType.Right:
                    return "right";
                case TextWrappingType.Largest:
                    return "largest";
                case TextWrappingType.Both:
                default:
                    return "bothSides";
            }
        }
        /// <summary>
        /// Serialize the shape borders.
        /// </summary>
        /// <param name="borders"></param>
        private void SerializeShapeBorders(PICF picDescriptor)
        {
            if (picDescriptor.BorderTop.BorderType != (byte)BorderStyle.None)
                SerializeShapeBorder(picDescriptor.BorderTop, "bordertop");

            if (picDescriptor.BorderBottom.BorderType != (byte)BorderStyle.None)
                SerializeShapeBorder(picDescriptor.BorderBottom, "borderbottom");

            if (picDescriptor.BorderRight.BorderType != (byte)BorderStyle.None)
                SerializeShapeBorder(picDescriptor.BorderRight, "borderright");

            if (picDescriptor.BorderLeft.BorderType != (byte)BorderStyle.None)
                SerializeShapeBorder(picDescriptor.BorderLeft, "borderleft");
        }
        /// <summary>
        /// Serialize the shape border
        /// </summary>
        /// <param name="border"></param>
        /// <param name="localName"></param>
        private void SerializeShapeBorder(BorderCode border, string localName)
        {
            m_writer.WriteStartElement(localName, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("type", GetShapeBorderStyleAsString((BorderStyle)border.BorderType));
            m_writer.WriteAttributeString("width", border.LineWidth.ToString());
            if (border.Shadow)
                m_writer.WriteAttributeString("shadow", border.Shadow.ToString());
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Get the shape border style.
        /// </summary>
        /// <param name="borderStyle"></param>
        /// <returns></returns>
        private string GetShapeBorderStyleAsString(BorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case BorderStyle.TwistedLines1:
                    return "twistedLines1";
                case BorderStyle.Triple:
                    return "triple";
                case BorderStyle.DashSmallGap:
                    return "dashedSmall";
                case BorderStyle.Single:
                    return "single";
                case BorderStyle.Hairline:
                    return "hairline";
                case BorderStyle.Dot:
                    return "dot";
                case BorderStyle.DotDash:
                    return "dotDash";
                case BorderStyle.DashLargeGap:
                    return "dash";
                case BorderStyle.DotDotDash:
                    return "dashDotDot";
                case BorderStyle.Double:
                    return "double";
                case BorderStyle.ThinThinSmallGap:
                    return "thickThinSmall";
                case BorderStyle.ThinThickSmallGap:
                    return "thinThickSmall";
                case BorderStyle.ThinThickThinSmallGap:
                    return "thickBetweenThinSmall";
                case BorderStyle.ThickThinMediumGap:
                    return "thickThin";
                case BorderStyle.ThinThickMediumGap:
                    return "thinThick";
                case BorderStyle.ThickThickThinMediumGap:
                    return "thickBetweenThin";
                case BorderStyle.ThickThinLargeGap:
                    return "thickThinLarge";
                case BorderStyle.ThinThickLargeGap:
                    return "thinThickLarge";
                case BorderStyle.ThinThickThinLargeGap:
                    return "thickBetweenThinLarge";
                case BorderStyle.Thick:
                    return "thick";
                case BorderStyle.Wave:
                    return "wave";
                case BorderStyle.DoubleWave:
                    return "doubleWave";
                case BorderStyle.DashDotStroker:
                    return "dashDotStroked";
                case BorderStyle.Engrave3D:
                    return "threeDEngrave";
                case BorderStyle.Emboss3D:
                    return "threeDEmboss";
                case BorderStyle.Outset:
                    return "HTMLOutset";
                case BorderStyle.Inset:
                    return "HTMLInset";
                case BorderStyle.Cleared:
                    return "nil";
                case BorderStyle.None:
                default:
                    return "none";
            }
        }
        /// <summary>
        /// Serialize the shape picture style.
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private string SerializeShapePictStyle(WPicture picture)
        {
            StringBuilder style = new StringBuilder();

            if (picture.TextWrappingStyle != TextWrappingStyle.Inline)
                style.Append(@"position:absolute;");

            style.Append("margin-left:");
            style.Append(picture.HorizontalPosition.ToString().Replace(",", "."));
            style.Append("pt;margin-top:");
            style.Append(picture.VerticalPosition.ToString().Replace(",", "."));
            style.Append("pt;width:");
            int cx = (int)Math.Round(((picture.Width * picture.WidthScale) / 100));
            style.Append(cx);
            style.Append("pt;height:");
            int cy = (int)Math.Round(((picture.Height * picture.HeightScale) / 100));
            style.Append(cy);
            style.Append(@"pt");

            if (picture.HorizontalOrigin != HorizontalOrigin.Column)
            {
                style.Append(";mso-position-horizontal-relative:");
                style.Append(GetHorizOriginAsString(picture.HorizontalOrigin));
            }

            if (picture.VerticalOrigin != VerticalOrigin.Paragraph)
            {
                style.Append(";mso-position-vertical-relative:");
                style.Append(GetVerticalOrginAsString (picture.VerticalOrigin));
            }

            if (picture.HorizontalAlignment != ShapeHorizontalAlignment.None)
            {
                style.Append(";mso-position-horizontal:");
                style.Append(picture.HorizontalAlignment.ToString().ToLower());
            }

            if (picture.VerticalAlignment != ShapeVerticalAlignment.None)
            {
                style.Append(";mso-position-vertical:");
                style.Append(picture.VerticalAlignment.ToString().ToLower());
            }

            if (picture.OrderIndex != int.MaxValue)
            {
                if (picture.OrderIndex > 0 && picture.TextWrappingStyle == TextWrappingStyle.Behind)
                    style.Append(";z-index:-" + picture.OrderIndex.ToString());
                else
                    style.Append(";z-index:" + picture.OrderIndex.ToString());
            }
            else if (picture.TextWrappingStyle == TextWrappingStyle.Behind)
                style.Append(";z-index:-1");


            style.Append(";visibility:visible");
            return style.ToString();
        }

        #endregion

        #region Bookmark
        /// <summary>
        /// Serialize the bookmark end.
        /// </summary>
        /// <param name="bookmarkEnd"></param>
        private void SerializeBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            //<w:bookmarkEnd w:id="0"/>
            if (m_bookmarks.ContainsKey(bookmarkEnd.Name))
            {
                m_writer.WriteStartElement("w", "bookmarkEnd", DocxConstants.W_namespace);
                int bkmkEndId = m_bookmarks[bookmarkEnd.Name];
                m_writer.WriteAttributeString("w", "id", DocxConstants.W_namespace, bkmkEndId.ToString());
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the bookmark start.
        /// </summary>
        /// <param name="bookmarkStart"></param>
        private void SerializeBookmarkStart(BookmarkStart bookmarkStart)
        {
            //<w:bookMarkStart w:colFirst="0" w:colLast="1" w:id="0" w:name="table"/>
            string bookmarkName = bookmarkStart.Name;
            int bookmarkID = GetNextBookmarkID();
            if (m_bookmarks.ContainsKey(bookmarkName))
                return;
            m_bookmarks.Add(bookmarkName, bookmarkID);

            m_writer.WriteStartElement("w", "bookmarkStart", DocxConstants.W_namespace);

            if (bookmarkStart.ColumnFirst >= 0)
                m_writer.WriteAttributeString("w", "colFirst", DocxConstants.W_namespace, bookmarkStart.ColumnFirst.ToString());

            if (bookmarkStart.ColumnLast >= 0)
                m_writer.WriteAttributeString("w", "colLast", DocxConstants.W_namespace, bookmarkStart.ColumnLast.ToString());

            m_writer.WriteAttributeString("w", "id", DocxConstants.W_namespace, bookmarkID.ToString());
            m_writer.WriteAttributeString("w", "name", DocxConstants.W_namespace, bookmarkName);

            m_writer.WriteEndElement();

        }
        #endregion Bookmark

        #region Break
        /// <summary>
        /// Serialize the break element.
        /// </summary>
        /// <param name="breakType"></param>
        private void SerializeBreak(BreakType breakType)
        {
            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("br", DocxConstants.W_namespace);
            if (breakType == BreakType.ColumnBreak)
            {
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "column");
            }
            else if (breakType == BreakType.PageBreak)
            {
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "page");
            }
            m_writer.WriteEndElement();//end of br tag (break)
            m_writer.WriteEndElement();//end of r tag (textRange)
        }
        #endregion Break

        #region Comments
        /// <summary>
        /// Serialize the comment reference.
        /// </summary>
        /// <param name="comment"></param>
        private void SerializeCommentReference(WComment comment)
        {
            string commentId = null;
            if (m_commentsId != null && m_commentsId.ContainsKey(comment.Format.TagBkmk))
            {
                commentId = m_commentsId[comment.Format.TagBkmk];
            }
            else
            {
                commentId = GetNextID().ToString();
            }

            CommentCollection.Add(commentId, comment);
            m_hasComment = true;

            if (comment.AppendItems)
            {
                WriteCommItems(comment, commentId);
            }

            m_writer.WriteStartElement("r", DocxConstants.W_namespace);
            m_writer.WriteStartElement("commentReference", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, commentId);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the commented items.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="commentId">The comment id.</param>
        private void WriteCommItems(WComment comment, string commentId)
        {
            if (comment.CommentedBodyPart != null)
            {
                // Write paragraph end.
                m_writer.WriteEndElement();

                SerializeCommentRangeStart(commentId);
                // Build commented body items
                SerializeBodyItems(comment.CommentedBodyPart.BodyItems, false);
                SerializeCommentRangeEnd(commentId);

                // Continue breaked paragraph.
                m_writer.WriteStartElement("p", DocxConstants.W_namespace);
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);

                SerializeParagraphFormat(comment.OwnerParagraph.ParagraphFormat, comment.OwnerParagraph);

                m_writer.WriteEndElement();
            }
            else if (comment.CommentedItems.Count > 0)
            {
                SerializeCommentRangeStart(commentId);
                foreach (ParagraphItem item in comment.CommentedItems)
                {
                    SerializeParagraphItem(item);
                }
                SerializeCommentRangeEnd(commentId);
            }
        }
        /// <summary>
        /// Serialize the comment range start.
        /// </summary>
        private void SerializeCommentRangeStart(string commentId)
        {
            m_writer.WriteStartElement("commentRangeStart", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, commentId);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the comment range end.
        /// </summary>
        private void SerializeCommentRangeEnd(string commentId)
        {
            m_writer.WriteStartElement("commentRangeEnd", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, commentId);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the comments.
        /// </summary>
        /// <returns></returns>
        private void SerializeComments()
        {
            MemoryStream commentsStream = new MemoryStream();
            m_writer = CreateWriter(commentsStream);


            m_writer.WriteStartElement("w", "comments", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "ve", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "a", null, DocxConstants.A_namespace);
            m_writer.WriteAttributeString("xmlns", "pic", null, DocxConstants.PIC_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);

            WComment comment;

            foreach (string id in CommentCollection.Keys)
            {
                comment = CommentCollection[id];
                SerializeComment(comment, id);
            }

            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.CommentsPath, commentsStream, false, FileAttributes.Archive);

        }
        /// <summary>
        /// Serialize the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="id">The id.</param>
        internal void SerializeComment(WComment comment, string id)
        {
            m_writer.WriteStartElement("w", "comment", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "id", DocxConstants.W_namespace, id);
            m_writer.WriteAttributeString("w", "author", DocxConstants.W_namespace, comment.Format.User);
            m_writer.WriteAttributeString("w", "initials", DocxConstants.W_namespace, comment.Format.UserInitials);

            //foreach( TextBodyItem bodyItem in comment.TextBody.Items )
            TextBodyItem bodyItem = null;
            for (int i = 0; i < comment.TextBody.Items.Count; i++)
            {
                bodyItem = comment.TextBody.Items[i];
                SerializeBodyItem(bodyItem, true);
            }

            m_writer.WriteEndElement();
        }

        /// <summary>
        /// Serialize the comment mark.
        /// </summary>
        /// <param name="commMark">The comm mark.</param>
        private void SerializeCommentMark(WCommentMark commMark)
        {
            string id = null;
            if (commMark.Type == CommentMarkType.CommentStart)
            {
                TagIdRandomizer.NoneChangeIds.Add(commMark.CommentId);

                id = GetNextID().ToString();
                CommentsId.Add(commMark.CommentId, id);
                SerializeCommentRangeStart(id);
            }
            else if (m_commentsId != null && m_commentsId.ContainsKey(commMark.CommentId))
            {
                id = m_commentsId[commMark.CommentId];
                SerializeCommentRangeEnd(id);
            }
        }
        #endregion Comments

        #region TextRange
        /// <summary>
        /// Serialize the text range.
        /// </summary>
        /// <param name="item"></param>
        private void SerializeTextRange(ParagraphItem item)
        {
            if (item is WTextRange)
            {
                if (item.PreviousSibling != null && item.PreviousSibling.PreviousSibling != null)
                {
                    WField field = (item.PreviousSibling.PreviousSibling as WField);

                    if (field != null &&
                        (field.FieldType == FieldType.FieldFileName))
                        return;
                }

                if (!IsNestedItem(item))
                {

                    WTextRange txtRange = item as WTextRange;


                    WFootnote footnote = null;

                    if (txtRange.OwnerParagraph != null && txtRange.OwnerParagraph.OwnerTextBody != null)
                    {
                        footnote = txtRange.OwnerParagraph.OwnerTextBody.Owner as WFootnote;
                    }

                    if ((footnote != null && txtRange.Text == SpecialCharacters.FootnoteAscii.ToString()) ||
                      (footnote != null && footnote.IsAutoNumbered && txtRange.PreviousSibling == null &&
                      txtRange.OwnerParagraph.PreviousSibling == null))
                    {
                        m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
                        SerializeCharactetFormat(txtRange.CharacterFormat);
                        if (footnote.FootnoteType == FootnoteType.Endnote)
                        {
                            m_writer.WriteStartElement("endnoteRef", DocxConstants.W_namespace);
                            m_writer.WriteEndElement();
                        }
                        else
                        {
                            m_writer.WriteStartElement("footnoteRef", DocxConstants.W_namespace);
                            m_writer.WriteEndElement();
                        }
                        m_writer.WriteEndElement();
                        return;
                    }
                    m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
                    SerializeCharactetFormat(txtRange.CharacterFormat);
                    SerializeText(txtRange.Text, txtRange.CharacterFormat.IsDeleteRevision);
                    m_writer.WriteEndElement();//end of Run element

                }
            }
        }
        /// <summary>
        /// Serialize the text.
        /// </summary>
        /// <param name="textToDisplay"></param>
        /// <param name="IsDeletedText"></param>
        private void SerializeText(string textToDisplay, bool IsDeletedText)
        {
            //internal char DEF_NONBREAK_HYPHEN = (char)0x1E;
            //internal char DEF_SOFT_HYPHEN = (char)0x1F;
            char[] specialCases = new char[32];
            for (int i = 0; i <= 31; i++)
                specialCases[i] = (char)i;
            int startIndex = 0;
            int index = textToDisplay.IndexOfAny(specialCases);
            while (index != -1)
            {
                string text = textToDisplay.Substring(startIndex, index);
                char nextChar = textToDisplay[index];
                if (text != string.Empty)
                    SerializeRawText(text, IsDeletedText);

                switch (nextChar)
                {
                    case '\f':
                        m_writer.WriteStartElement("w", "br", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("w", "type", DocxConstants.W_namespace, "page");
                        m_writer.WriteEndElement();
                        break;
                    case '\t':
                        m_writer.WriteStartElement("w", "tab", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case '\v':
                        m_writer.WriteStartElement("w", "br", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)0x1E:
                        m_writer.WriteStartElement("noBreakHyphen", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)0x1F:
                        m_writer.WriteStartElement("softHyphen", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)2:
                        m_writer.WriteStartElement("footnoteRef", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)3:
                        m_writer.WriteStartElement("separator", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)4:
                        m_writer.WriteStartElement("continuationSeparator", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)5:
                        m_writer.WriteStartElement("annotationRef", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)0xA:
                        m_writer.WriteStartElement("w", "P", DocxConstants.W_namespace);
                        m_writer.WriteEndElement();
                        break;
                    case (char)0xE:
                        m_writer.WriteStartElement("w", "br", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("w", "type", DocxConstants.W_namespace, "column");
                        m_writer.WriteEndElement();
                        break;
                }
                textToDisplay = textToDisplay.Substring(index + 1);
                index = textToDisplay.IndexOfAny(specialCases);
            }
            SerializeRawText(textToDisplay, IsDeletedText);
        }
        /// <summary>
        /// Serialize the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IsDeletedText"></param>
        private void SerializeRawText(string text, bool IsDeletedText)
        {

            if (IsDeletedText)
                m_writer.WriteStartElement("delText", DocxConstants.W_namespace);
            else
                m_writer.WriteStartElement("t", DocxConstants.W_namespace);

            m_writer.WriteAttributeString("xml", "space", DocxConstants.Xml_namespace, "preserve");
            m_writer.WriteString(text);
            m_writer.WriteEndElement();//end of text "t"/"delText" tag.
        }
        #endregion TextRange

        #region TOC
        /// <summary>
        /// Serialize the table of contents.
        /// </summary>
        /// <param name="toc"></param>
        private void serializeTableOfContents(TableOfContent toc)
        {
            m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
            SerializeCharactetFormat(toc.TOCField.CharacterFormat);
            m_writer.WriteStartElement("w", "fldChar", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "fldCharType", DocxConstants.W_namespace, "begin");
            m_writer.WriteEndElement();//end of fldChar
            m_writer.WriteEndElement();//end of run

            m_writer.WriteStartElement("w", "r", DocxConstants.W_namespace);
            SerializeCharactetFormat(toc.TOCField.CharacterFormat);
            m_writer.WriteStartElement("w", "instrText", DocxConstants.W_namespace);
            m_writer.WriteString(GetFieldTypeAsString(FieldType.FieldTOC));
            toc.UpdateFormattingString();
            m_writer.WriteString(" " + toc.FormattingString);
            m_writer.WriteEndElement();//end of instrText
            m_writer.WriteEndElement();//end of run


        }
        /// <summary>
        /// Get the field type as string.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetFieldTypeAsString(FieldType type)
        {
            switch (type)
            {
                case FieldType.FieldAdvance:
                    return "ADVANCE ";
                case FieldType.FieldAuthor:
                    return "AUTHOR ";
                case FieldType.FieldAutoNum:
                    return "AUTONUM ";
                case FieldType.FieldAutoNumLegal:
                    return "AUTONUMLGL ";
                case FieldType.FieldAutoNumOutline:
                    return "AUTONUMOUT ";
                case FieldType.FieldAutoText:
                    return "AUTOTEXT ";
                case FieldType.FieldAutoTextList:
                    return "AUTOTEXTLIST ";
                case FieldType.FieldAsk:
                    return "ASK ";
                case FieldType.FieldBarCode:
                    return "BARCODE ";
                case FieldType.FieldComments:
                    return "COMMENTS ";
                case FieldType.FieldCreateDate:
                    return "CREATEDATE ";
                case FieldType.FieldDate:
                    return "DATE ";
                case FieldType.FieldDocProperty:
                    return "DOCPROPERTY ";
                case FieldType.FieldDocVariable:
                    return "DOCVARIABLE ";
                case FieldType.FieldEditTime:
                    return "EDITTIME ";
                case FieldType.FieldIf:
                    return "IF ";
                case FieldType.FieldFillIn:
                    return "FILLIN ";
                case FieldType.FieldFileName:
                    return "FILENAME ";
                case FieldType.FieldFileSize:
                    return "FILESIZE ";
                case FieldType.FieldFormCheckBox:
                    return "FORMCHECKBOX ";
                case FieldType.FieldFormDropDown:
                    return "FORMDROPDOWN ";
                case FieldType.FieldFormTextInput:
                    return "FORMTEXT ";
                case FieldType.FieldFormula:
                    return "= ";
                case FieldType.FieldGoToButton:
                    return "GOTOBUTTON ";
                case FieldType.FieldHyperlink:
                    return "HYPERLINK ";
                case FieldType.FieldIncludePicture:
                    return "INCLUDEPICTURE ";
                case FieldType.FieldIncludeText:
                    return "INCLUDETEXT ";
                case FieldType.FieldIndex:
                    return "INDEX ";
                case FieldType.FieldInfo:
                    return "INFO ";
                case FieldType.FieldKeyWord:
                    return "KEYWORDS ";
                case FieldType.FieldLastSavedBy:
                    return "LASTSAVEDBY ";
                case FieldType.FieldLink:
                    return "LINK ";
                case FieldType.FieldListNum:
                    return "LISTNUM ";
                case FieldType.FieldMacroButton:
                    return "MACROBUTTON ";
                case FieldType.FieldMergeField:
                    return "MERGEFIELD ";
                case FieldType.FieldNoteRef:
                    return "NOTEREF ";
                case FieldType.FieldNumChars:
                    return "NUMCHARS ";
                case FieldType.FieldNumPages:
                    return "NUMPAGES ";
                case FieldType.FieldNumWords:
                    return "NUMWORDS ";
                case FieldType.FieldPage:
                    return "PAGE ";
                case FieldType.FieldPageRef:
                    return "PAGEREF ";
                case FieldType.FieldPrint:
                    return "PRINT ";
                case FieldType.FieldPrintDate:
                    return "PRINTDATE ";
                case FieldType.FieldPrivate:
                    return "PRIVATE ";
                case FieldType.FieldQuote:
                    return "QUOTE ";
                case FieldType.FieldRef:
                    return "REF ";
                case FieldType.FieldRevisionNum:
                    return "REVNUM ";
                case FieldType.FieldSaveDate:
                    return "SAVEDATE ";
                case FieldType.FieldSection:
                    return "SECTION ";
                case FieldType.FieldSectionPages:
                    return "SECTIONPAGES ";
                case FieldType.FieldSequence:
                    return "SEQ ";
                case FieldType.FieldSet:
                    return "SET ";
                case FieldType.FieldStyleRef:
                    return "STYLEREF ";
                case FieldType.FieldSubject:
                    return "SUBJECT ";
                case FieldType.FieldSymbol:
                    return "SYMBOL ";
                case FieldType.FieldTemplate:
                    return "TEMPLATE ";
                case FieldType.FieldTime:
                    return "TIME ";
                case FieldType.FieldTitle:
                    return "TITLE ";
                case FieldType.FieldTOA:
                    return "TOA ";
                case FieldType.FieldTOC:
                    return "TOC ";
                case FieldType.FieldUserAddress:
                    return "USERADDRESS ";
                case FieldType.FieldUserInitials:
                    return "USERINITIALS ";
                case FieldType.FieldUserName:
                    return "USERNAME ";
                case FieldType.FieldAddin:
                    return "ADDIN ";
                case FieldType.FieldAddressBlock:
                    return "ADDRESSBLOCK";
                case FieldType.FieldBidiOutline:
                    return "BIDIOUTLINE";
                case FieldType.FieldIndexEntry:
                    return "XE ";
                default:
                    return string.Empty;
            }
        }
        #endregion TOC
        #endregion ParagraphItems

        #region SectionProperties
        /// <summary>
        /// Serialize the section properties.
        /// </summary>
        /// <param name="section"></param>
        private void SerializeSectionProperties(WSection section)
        {
            m_writer.WriteStartElement("w", "sectPr", DocxConstants.W_namespace);
            SerializeHFReference(section.HeadersFooters);
            if (IsSectionContainsFootnotes)
                SerializeFootnoteProperties(section);
            if (IsSectionContainsEndnotes)
                SerializeEndnoteProperties(section);
            SerializeSectionType(section.BreakCode);
            SerializePageSetup(section.PageSetup);
            SerializeColumns(section);
            SerializeSectionProtection(section);

            if (section.PageSetup.VerticalAlignment != PageAlignment.Top)
            {
                m_writer.WriteStartElement("vAlign", DocxConstants.W_namespace);

                switch (section.PageSetup.VerticalAlignment)
                {
                    case PageAlignment.Top:
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "top");
                        break;
                    case PageAlignment.Middle:
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "center");
                        break;
                    case PageAlignment.Justified:
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "both");
                        break;
                    case PageAlignment.Bottom:
                        m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "bottom");
                        break;
                }

                m_writer.WriteEndElement();
            }

            if (section.PageSetup.DifferentFirstPage)
            {
                m_writer.WriteStartElement("titlePg", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }

            SerializeTextDirection(section);

            if (section.PageSetup.Bidi)
            {
                m_writer.WriteStartElement("bidi", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //rtlGutter
            SerializeDocGrid(section);
            //printerSettings
            m_writer.WriteEndElement();//end of sectPr tag

        }
        /// <summary>
        /// Serialize the heeader/footer reference.
        /// </summary>
        /// <param name="headerFooters">The HeaderFooters</param>
        private void SerializeHFReference(WHeadersFooters headerFooters)
        {
            bool existsWatermark = (headerFooters.Document.Watermark != null &&
              headerFooters.Document.Watermark.Type != WatermarkType.NoWatermark);
            string hfId = string.Empty;

            if (headerFooters.FirstPageHeader.Items.Count > 0 || (existsWatermark && headerFooters.FirstPageHeader.WriteWatermark))
            {
                m_writer.WriteStartElement("headerReference", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "first");
                hfId = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, hfId);
                AddHeaderFooter(headerFooters.FirstPageHeader, HeaderFooterType.FirstPageHeader, hfId);
                m_writer.WriteEndElement();
            }

            if (headerFooters.FirstPageFooter.Items.Count > 0)
            {
                m_writer.WriteStartElement("footerReference", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "first");
                hfId = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, hfId);
                AddHeaderFooter(headerFooters.FirstPageFooter, HeaderFooterType.FirstPageFooter, hfId);
                m_writer.WriteEndElement();
            }

            if (headerFooters.EvenHeader.Items.Count > 0 || (existsWatermark && headerFooters.EvenHeader.WriteWatermark))
            {
                m_writer.WriteStartElement("headerReference", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "even");
                hfId = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, hfId);
                AddHeaderFooter(headerFooters.EvenHeader, HeaderFooterType.EvenHeader, hfId);
                m_writer.WriteEndElement();
            }

            if (headerFooters.EvenFooter.Items.Count > 0)
            {
                m_writer.WriteStartElement("footerReference", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "even");
                hfId = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, hfId);
                AddHeaderFooter(headerFooters.EvenFooter, HeaderFooterType.EvenFooter, hfId);
                m_writer.WriteEndElement();
            }

            if (headerFooters.OddHeader.Items.Count > 0 || (existsWatermark && headerFooters.OddHeader.WriteWatermark))
            {
                m_writer.WriteStartElement("headerReference", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "default");
                hfId = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, hfId);
                AddHeaderFooter(headerFooters.OddHeader, HeaderFooterType.OddHeader, hfId);
                m_writer.WriteEndElement();
            }

            if (headerFooters.OddFooter.Items.Count > 0)
            {
                m_writer.WriteStartElement("footerReference", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "default");
                hfId = GetNextRelationShipID();
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, hfId);
                AddHeaderFooter(headerFooters.OddFooter, HeaderFooterType.OddFooter, hfId);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the docGrid element
        /// </summary>
        /// <param name="section"></param>
        private void SerializeDocGrid(WSection section)
        {
            if (section.PageSetup.LinePitch > 0)
            {
                m_writer.WriteStartElement("docGrid", DocxConstants.W_namespace);
                switch (section.PageSetup.PitchType)
                {
                    case GridPitchType.LinesOnly:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "lines");
                        break;
                    case GridPitchType.CharsAndLine:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "linesAndChars");
                        break;
                    case GridPitchType.SnapToChars:
                        m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "snapToChars");
                        break;
                }

                ushort linePitch = (ushort)Math.Round(section.PageSetup.LinePitch * DLSConstants.TwipsInOnePoint);
                m_writer.WriteAttributeString("linePitch", DocxConstants.W_namespace, linePitch.ToString());
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize section protection.
        /// </summary>
        /// <param name="section"></param>
        private void SerializeSectionProtection(WSection section)
        {
            if (!section.ProtectForm)
            {
                m_writer.WriteStartElement("w", "formProt", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the text direction.
        /// </summary>
        /// <param name="section"></param>
        private void SerializeTextDirection(WSection section)
        {
            if (section.TextDirection == DocTextDirection.LeftToRight)
                return;

            m_writer.WriteStartElement("w", "textDirection", DocxConstants.W_namespace);
            switch (section.TextDirection)
            {
                case DocTextDirection.LeftToRightRotated:
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "btLr");
                    break;
                case DocTextDirection.RightToLeft:
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "lrTbV");
                    break;
                case DocTextDirection.RightToLeftRotated:
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "tbRlV");
                    break;
                case DocTextDirection.TopToBottom:
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "tbRl");
                    break;
                case DocTextDirection.TopToBottomRotated:
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "lrTbV");
                    break;
            }
            m_writer.WriteEndElement();//end of textDirection
        }
        /// <summary>
        /// Serialize the column properties of section.
        /// </summary>
        /// <param name="section"></param>
        private void SerializeColumns(WSection section)
        {
            ColumnCollection columns = section.Columns;

            m_writer.WriteStartElement("cols", DocxConstants.W_namespace);

            if (columns.Count > 0)
            {
                m_writer.WriteAttributeString("num", DocxConstants.W_namespace, columns.Count.ToString());
            }

            if (section.PageSetup.DrawLinesBetweenCols)
                m_writer.WriteAttributeString("sep", DocxConstants.W_namespace, "1");

            if (columns.OwnerSection.PageSetup.EqualColumnWidth)
            {
                m_writer.WriteAttributeString("equalWidth", DocxConstants.W_namespace, "1");
                m_writer.WriteAttributeString("space", DocxConstants.W_namespace, ToString(columns[0].Space * DocxConstants.TwentiethOfPoint));
            }
            else if (columns.Count > 0)
            {
                m_writer.WriteAttributeString("equalWidth", DocxConstants.W_namespace, "0");

                foreach (Column column in columns)
                {
                    m_writer.WriteStartElement("col", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(column.Width * DocxConstants.TwentiethOfPoint));
                    m_writer.WriteAttributeString("space", DocxConstants.W_namespace, ToString(column.Space * DocxConstants.TwentiethOfPoint));
                    m_writer.WriteEndElement();
                }
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the page setup properties.
        /// </summary>
        /// <param name="pageSetup">The page setup.</param>
        private void SerializePageSetup(WPageSetup pageSetup)
        {
            SerializePageSize(pageSetup);
            SerializePageMargins(pageSetup);
            //paperSrc
            m_writer.WriteStartElement("pgBorders", DocxConstants.W_namespace);
            //zOrder
            if (pageSetup.PageBordersApplyType == PageBordersApplyType.FirstPage)
                m_writer.WriteAttributeString("display", DocxConstants.W_namespace, "firstPage");
            else if (pageSetup.PageBordersApplyType == PageBordersApplyType.AllExceptFirstPage)
                m_writer.WriteAttributeString("display", DocxConstants.W_namespace, "notFirstPage");
            if (pageSetup.PageBorderOffsetFrom == PageBorderOffsetFrom.PageEdge)
            {
                m_writer.WriteAttributeString("offsetFrom", DocxConstants.W_namespace, "page");
            }
            //Serializing zOrder of the front page border
            if (!pageSetup.IsFrontPageBorder)
            {
                m_writer.WriteAttributeString("zOrder", DocxConstants.W_namespace, "back");
            }
            SerializePageBorders(pageSetup.Borders);
            m_writer.WriteEndElement();

            SerializeLineNumberType(pageSetup);
            SerializePageNumberType(pageSetup);

        }
        /// <summary>
        /// Serializes the page borders.
        /// </summary>
        /// <param name="borders">The borders.</param>
        private void SerializePageBorders(Borders borders)
        {
            SerializeBorder(borders.Top, "top", DocxConstants.BorderMultiplier);
            SerializeBorder(borders.Left, "left", DocxConstants.BorderMultiplier);
            SerializeBorder(borders.Bottom, "bottom", DocxConstants.BorderMultiplier);
            SerializeBorder(borders.Right, "right", DocxConstants.BorderMultiplier);
        }
        /// <summary>
        /// Serialize the borders.
        /// </summary>
        /// <param name="borders"></param>
        /// <param name="multipler"></param>
        private void SerializeBorders(Borders borders, int multipler)
        {
            SerializeBorder(borders.Top, "top", multipler);
            SerializeBorder(borders.Left, "left", multipler);
            SerializeBorder(borders.Bottom, "bottom", multipler);
            SerializeBorder(borders.Right, "right", multipler);

            SerializeBorder(borders.Horizontal, "insideH", multipler);
            SerializeBorder(borders.Vertical, "insideV", multipler);
            SerializeBorder(borders.DiagonalDown, "tl2br", multipler);
            SerializeBorder(borders.DiagonalUp, "tr2bl", multipler);
        }
        /// <summary>
        /// Serialize the border.
        /// </summary>
        /// <param name="pageSetup"></param>
        private void SerializePageMargins(WPageSetup pageSetup)
        {
            m_writer.WriteStartElement("pgMar", DocxConstants.W_namespace);
            int marginValue = (int)Math.Round(pageSetup.Margins.Top * DocxConstants.TwentiethOfPoint);
            m_writer.WriteAttributeString("top", DocxConstants.W_namespace, marginValue.ToString());
            marginValue = (int)Math.Round(pageSetup.Margins.Right * DocxConstants.TwentiethOfPoint);
            m_writer.WriteAttributeString("right", DocxConstants.W_namespace, marginValue.ToString());
            marginValue = (int)Math.Round(pageSetup.Margins.Bottom * DocxConstants.TwentiethOfPoint);
            m_writer.WriteAttributeString("bottom", DocxConstants.W_namespace, marginValue.ToString());
            marginValue = (int)Math.Round(pageSetup.Margins.Left * DocxConstants.TwentiethOfPoint);
            m_writer.WriteAttributeString("left", DocxConstants.W_namespace, marginValue.ToString());

            if (pageSetup.HeaderDistance >= 0)
                m_writer.WriteAttributeString("header", DocxConstants.W_namespace, ToString(pageSetup.HeaderDistance * DocxConstants.TwentiethOfPoint));
            if (pageSetup.FooterDistance >= 0)
                m_writer.WriteAttributeString("footer", DocxConstants.W_namespace, ToString(pageSetup.FooterDistance * DocxConstants.TwentiethOfPoint));
            marginValue = (int)Math.Round(pageSetup.Margins.Gutter * DocxConstants.TwentiethOfPoint);
            m_writer.WriteAttributeString("gutter", DocxConstants.W_namespace, marginValue.ToString());

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// serialize the page size
        /// </summary>
        /// <param name="pageSetup"></param>
        private void SerializePageSize(WPageSetup pageSetup)
        {
            m_writer.WriteStartElement("pgSz", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(pageSetup.PageSize.Width * DocxConstants.TwentiethOfPoint));
            m_writer.WriteAttributeString("h", DocxConstants.W_namespace, ToString(pageSetup.PageSize.Height * DocxConstants.TwentiethOfPoint));

            if (pageSetup.Orientation == PageOrientation.Landscape)
            {
                m_writer.WriteAttributeString("orient", DocxConstants.W_namespace, "landscape");
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the page number type.
        /// </summary>
        /// <param name="pageSetup"></param>
        private void SerializePageNumberType(WPageSetup pageSetup)
        {
            m_writer.WriteStartElement("pgNumType", DocxConstants.W_namespace);
            string type = GetPageNumType(pageSetup.PageNumberStyle);
            m_writer.WriteAttributeString("fmt", DocxConstants.W_namespace, type);
            if (pageSetup.RestartPageNumbering)
                m_writer.WriteAttributeString("start", DocxConstants.W_namespace, pageSetup.PageStartingNumber.ToString());
            if (pageSetup.PageNumbers.HeadingLevelForChapter != HeadingLevel.None)
            {
                m_writer.WriteAttributeString("chapStyle", DocxConstants.W_namespace, Convert.ToInt32(pageSetup.PageNumbers.HeadingLevelForChapter).ToString());
                switch (pageSetup.PageNumbers.ChapterPageSeparator)
                {
                    case ChapterPageSeparatorType.Colon:
                        m_writer.WriteAttributeString("chapSep", DocxConstants.W_namespace, "colon");
                        break;
                    case ChapterPageSeparatorType.Hyphen:
                        m_writer.WriteAttributeString("chapSep", DocxConstants.W_namespace, "hyphen");
                        break;
                    case ChapterPageSeparatorType.Period:
                        m_writer.WriteAttributeString("chapSep", DocxConstants.W_namespace, "period");
                        break;
                    case ChapterPageSeparatorType.EmDash:
                        m_writer.WriteAttributeString("chapSep", DocxConstants.W_namespace, "emDash");
                        break;
                    case ChapterPageSeparatorType.EnDash:
                        m_writer.WriteAttributeString("chapSep", DocxConstants.W_namespace, "enDash");
                        break;
                }
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the line number type.
        /// </summary>
        /// <param name="pageSetup"></param>
        private void SerializeLineNumberType(WPageSetup pageSetup)
        {
            if (pageSetup.HasLineNumbering || pageSetup.LineNumberingMode != LineNumberingMode.None || pageSetup.LineNumberingStartValue != 0
              && pageSetup.LineNumberingStep != 0 || pageSetup.LineNumberingDistanceFromText != 0)
            {
                m_writer.WriteStartElement("lnNumType", DocxConstants.W_namespace);
                if (pageSetup.LineNumberingStep != 0)
                    m_writer.WriteAttributeString("countBy", DocxConstants.W_namespace, pageSetup.LineNumberingStep.ToString());
                if (pageSetup.LineNumberingStartValue != 0)
                    m_writer.WriteAttributeString("start", DocxConstants.W_namespace, (pageSetup.LineNumberingStartValue - 1).ToString());
                if (pageSetup.LineNumberingDistanceFromText != 0)
                    m_writer.WriteAttributeString("distance", DocxConstants.W_namespace, ToString(pageSetup.LineNumberingDistanceFromText * DocxConstants.TwentiethOfPoint));

                switch (pageSetup.LineNumberingMode)
                {
                    case LineNumberingMode.RestartPage:
                        m_writer.WriteAttributeString("restart", DocxConstants.W_namespace, "newPage");
                        break;
                    case LineNumberingMode.RestartSection:
                        m_writer.WriteAttributeString("restart", DocxConstants.W_namespace, "newSection");
                        break;
                    case LineNumberingMode.Continuous:
                        m_writer.WriteAttributeString("restart", DocxConstants.W_namespace, "continuous");
                        break;
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the section type.
        /// </summary>
        /// <param name="sectionBreakCode"></param>
        private void SerializeSectionType(SectionBreakCode sectionBreakCode)
        {
            m_writer.WriteStartElement("w", "type", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetSectionBreakCode(sectionBreakCode));
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the section break code.
        /// </summary>
        /// <param name="sectionBreakCode"></param>
        /// <returns></returns>
        private string GetSectionBreakCode(SectionBreakCode sectionBreakCode)
        {
            switch (sectionBreakCode)
            {
                case SectionBreakCode.NewColumn:
                    return "nextColumn";
                case SectionBreakCode.NewPage:
                    return "nextPage";
                case SectionBreakCode.EvenPage:
                    return "evenPage";
                case SectionBreakCode.Oddpage:
                    return "oddPage";
                default:
                    return "continuous";
            }
        }
        /// <summary>
        /// Serialize the endnote properties.
        /// </summary>
        /// <param name="section"></param>
        private void SerializeEndnoteProperties(WSection section)
        {
            m_writer.WriteStartElement("endnotePr", DocxConstants.W_namespace);
            SerializeEndnoteFootnoteNumberFormat(section, false);
            if (section.PageSetup.InitialEndnoteNumber != 0)
            SerializeEndnoteFootnoteElement("numStart", section.PageSetup.InitialEndnoteNumber.ToString());
            if (section.PageSetup.RestartIndexForEndnote == EndnoteRestartIndex.RestartForEachSection)
            {
                SerializeEndnoteFootnoteElement("numRestart", "eachSect");
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the footnote properties.
        /// </summary>
        /// <param name="section"></param>
        private void SerializeFootnoteProperties(WSection section)
        {
            m_writer.WriteStartElement("footnotePr", DocxConstants.W_namespace);
            SerializeFootnotePosition(section);
            SerializeEndnoteFootnoteNumberFormat(section, true);
            if (section.PageSetup.InitialFootnoteNumber != 0)
                SerializeEndnoteFootnoteElement("numStart", section.PageSetup.InitialFootnoteNumber.ToString());
            if (section.PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachPage)
            {
                SerializeEndnoteFootnoteElement("numRestart", "eachPage");
            }
            else if (section.PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachSection)
            {
                SerializeEndnoteFootnoteElement("numRestart", "eachSect");
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the endnote/footnote element properties
        /// </summary>
        /// <param name="elemName"></param>
        /// <param name="elemValue"></param>
        private void SerializeEndnoteFootnoteElement(string elemName, string elemValue)
        {
            m_writer.WriteStartElement(elemName, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, elemValue);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the endnote footnote number format.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="isFootnote">if set to <c>true</c> [is footnote].</param>
        private void SerializeEndnoteFootnoteNumberFormat(WSection section, bool isFootnote)
        {
            switch ((isFootnote) ? section.PageSetup.FootnoteNumberFormat : section.PageSetup.EndnoteNumberFormat)
            {
                case FootEndNoteNumberFormat.Arabic:
                    SerializeEndnoteFootnoteElement("numFmt", "decimal");
                    break;
                case FootEndNoteNumberFormat.LowerCaseLetter:
                    SerializeEndnoteFootnoteElement("numFmt", "lowerLetter");
                    break;
                case FootEndNoteNumberFormat.UpperCaseLetter:
                    SerializeEndnoteFootnoteElement("numFmt", "upperLetter");
                    break;
                case FootEndNoteNumberFormat.LowerCaseRoman:
                    SerializeEndnoteFootnoteElement("numFmt", "lowerRoman");
                    break;
                case FootEndNoteNumberFormat.UpperCaseRoman:
                    SerializeEndnoteFootnoteElement("numFmt", "upperRoman");
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Serializes the endnote footnote number format.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="isFootnote">if set to <c>true</c> [is footnote].</param>
        private void SerializeEndnoteFootnoteNumberFormat(bool isFootnote)
        {
            switch ((isFootnote) ? m_document.FootnoteNumberFormat : m_document.EndnoteNumberFormat)
            {
                case FootEndNoteNumberFormat.Arabic:
                    SerializeEndnoteFootnoteElement("numFmt", "decimal");
                    break;
                case FootEndNoteNumberFormat.LowerCaseLetter:
                    SerializeEndnoteFootnoteElement("numFmt", "lowerLetter");
                    break;
                case FootEndNoteNumberFormat.UpperCaseLetter:
                    SerializeEndnoteFootnoteElement("numFmt", "upperLetter");
                    break;
                case FootEndNoteNumberFormat.LowerCaseRoman:
                    SerializeEndnoteFootnoteElement("numFmt", "lowerRoman");
                    break;
                case FootEndNoteNumberFormat.UpperCaseRoman:
                    SerializeEndnoteFootnoteElement("numFmt", "upperRoman");
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds the header footer details to the collection.
        /// </summary>
        /// <param name="hf"></param>
        /// <param name="hfType"></param>
        /// <param name="id"></param>
        private void AddHeaderFooter(HeaderFooter hf, HeaderFooterType hfType, string id)
        {
            if (!HeadersFooters.ContainsKey(hfType))
            {
                Dictionary<string, HeaderFooter> hfColl = new Dictionary<string, HeaderFooter>();
                HeadersFooters.Add(hfType, hfColl);
            }

            HeadersFooters[hfType].Add(id, hf);
        }
        #endregion SectionProperties

        #endregion Document Body

        #region Settings
        /// <summary>
        /// Serialize the document settings. (settings.xml)
        /// </summary>
        private void SerializeSettings()
        {
            MemoryStream settingsStream = new MemoryStream();
            m_writer = CreateWriter(settingsStream);

            m_writer.WriteStartElement("w", "settings", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "mc", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("xmlns", "sl", null, DocxConstants.SL_namespace);
            m_writer.WriteAttributeString("mc", "Ignorable", null, "w14 w15");

            //Parse Settings Relations
            ParseSettingsRelations();
            List<Stream> tempDocxProps = new List<Stream>();
            for (int i = 0, cnt = m_document.DocxProps.Count; i < cnt; i++)
                tempDocxProps.Add(m_document.DocxProps[i]);
            //w:writeProtection - Write Protection
            if (m_document.WriteProtected)
            {
                m_writer.WriteStartElement("w", "writeProtection", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("recommended", DocxConstants.W_namespace, "1");
                m_writer.WriteEndElement();
            }
            //w:view - Document View Setting
            if (m_document.ViewSetup.DocumentViewType != DocumentViewType.PrintLayout &&
              m_document.ViewSetup.DocumentViewType != DocumentViewType.NormalLayout)
            {
                m_writer.WriteStartElement("view", DocxConstants.W_namespace);
                string viewTypeStr = string.Empty;
                if (m_document.ViewSetup.DocumentViewType == DocumentViewType.OutlineLayout)
                {
                    viewTypeStr = "outline";
                }
                else if (m_document.ViewSetup.DocumentViewType == DocumentViewType.WebLayout)
                {
                    viewTypeStr = "web";
                }
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, viewTypeStr);
                m_writer.WriteEndElement();
            }
            //w:zoom - Magnification Setting
            m_writer.WriteStartElement("zoom", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("percent", DocxConstants.W_namespace, m_document.ViewSetup.ZoomPercent.ToString());
            m_writer.WriteEndElement();
            //w:removePersonalInformation - Remove Personal Information from Document Properties
            SerializeDocxProps(tempDocxProps, "removePersonalInformation");
            //w:removeDateAndTime - Remove Date and Time from Annotations
            SerializeDocxProps(tempDocxProps, "removeDateAndTime");
            //w:doNotDisplayPageBoundaries - Do Not Display Visual Boundary For Header/Footer or Between Pages
            if (m_document.DOP.Dop2000.NoMargPgvwSaved)
            {
                m_writer.WriteStartElement("doNotDisplayPageBoundaries", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:displayBackgroundShape - Display Background Objects When Displaying Document
            if (m_document.Background.Type != BackgroundType.NoBackground)
            {
                m_writer.WriteStartElement("displayBackgroundShape", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:printFractionalCharacterWidth - Print Fractional Character Widths
            SerializeDocxProps(tempDocxProps, "printFractionalCharacterWidth");
            //w:printFormsData - Only Print Form Field Content
            SerializeDocxProps(tempDocxProps, "printFormsData");
            //w:embedTrueTypeFonts - Embed TrueType Fonts
            SerializeDocxProps(tempDocxProps, "embedTrueTypeFonts");
            //w:embedSystemFonts - Embed Common System Fonts
            SerializeDocxProps(tempDocxProps, "embedSystemFonts");
            //w:saveSubsetFonts - Subset Fonts When Embedding
            SerializeDocxProps(tempDocxProps, "saveSubsetFonts");
            //w:saveFormsData - Only Save Form Field Content
            SerializeDocxProps(tempDocxProps, "saveFormsData");
            //w:mirrorMargins - Mirror Page Margins
            if (m_document.DOP.MirrorMargins)
            {
                m_writer.WriteStartElement("mirrorMargins", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:alignBordersAndEdges - Align Paragraph and Table Borders with Page Border
            SerializeDocxProps(tempDocxProps, "alignBordersAndEdges");
            //w:bordersDoNotSurroundHeader - Page Border Excludes Header
            SerializeDocxProps(tempDocxProps, "bordersDoNotSurroundHeader");
            //w:bordersDoNotSurroundFooter - Page Border Excludes Footer
            SerializeDocxProps(tempDocxProps, "bordersDoNotSurroundFooter");
            //w:gutterAtTop - Position Gutter At Top of Page
            SerializeDocxProps(tempDocxProps, "gutterAtTop");
            //w:hideSpellingErrors - Do Not Display Visual Indication of Spelling Errors
            SerializeDocxProps(tempDocxProps, "hideSpellingErrors");
            //w:hideGrammaticalErrors - Do Not Display Visual Indication of Grammatical Errors
            SerializeDocxProps(tempDocxProps, "hideGrammaticalErrors");
            //w:activeWritingStyle -    Grammar Checking Settings
            SerializeDocxProps(tempDocxProps, "activeWritingStyle");
            //w:proofState - Spelling and Grammatical Checking State
            if (m_document.GrammarSpellingData == null || (m_document.GrammarSpellingData != null &&
              m_document.GrammarSpellingData.PlcfgramData == null &&
              m_document.GrammarSpellingData.PlcfsplData == null))
            {
                m_writer.WriteStartElement("proofState", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "spelling", DocxConstants.W_namespace, "clean");
                m_writer.WriteAttributeString("w", "grammar", DocxConstants.W_namespace, "clean");
                m_writer.WriteEndElement();
            }
            //w:formsDesign - Structured Document Tag Placeholder Text Should be Resaved
            SerializeDocxProps(tempDocxProps, "formsDesign");
            //w:attachedTemplate - Attached Document Template
            if (!string.IsNullOrEmpty(m_document.AssociatedStrings.AttachedTemplate))
            {
                //Remove existing Attached Document Template relation from settings relation
                foreach (KeyValuePair<string, DictionaryEntry> keyValue in SettingsRelations)
                {
                    DictionaryEntry entry = SettingsRelations[keyValue.Key.ToString()];
                    if (Convert.ToString(entry.Key) == DocxConstants.AttachedTemplateRelType)
                    {
                        SettingsRelations.Remove(keyValue.Key);
                        break;
                    }
                }
                m_writer.WriteStartElement("attachedTemplate", DocxConstants.W_namespace);
                string id = "rId1";
                for (int i = 1; i <= SettingsRelations.Count; i++)
                {
                    if (SettingsRelations.ContainsKey(id))
                        id = string.Format("rId{0}", i+1);
                    else                       
                        break;
                    
                }
                m_writer.WriteAttributeString("r", "id", DocxConstants.R_namespace, id);
                string template = m_document.AssociatedStrings.AttachedTemplate.ToString();
                if (!template.StartsWith(@"file:///") && !template.Contains("http") && !template.Contains("www"))
                    template = @"file:///" + template;
                SettingsRelations.Add(id, new DictionaryEntry(DocxConstants.AttachedTemplateRelType, template));
                m_writer.WriteEndElement();
            }
            //w:linkStyles - Automatically Update Styles From Document Template
            if (m_document.DOP.LinkStyles)
            {
                m_writer.WriteStartElement("linkStyles", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:stylePaneFormatFilter - Suggested Filtering for List of Document Styles
            SerializeDocxProps(tempDocxProps, "stylePaneFormatFilter");
            //w:stylePaneSortMethod - Suggested Sorting for List of Document Styles
            SerializeDocxProps(tempDocxProps, "stylePaneSortMethod");
            //w:documentType - Document Classification
            SerializeDocxProps(tempDocxProps, "documentType");
            //w:mailMerge - Mail Merge Settings
            SerializeDocxProps(tempDocxProps, "mailMerge");
            //w:revisionView - Visibility of Annotation Types
            SerializeDocxProps(tempDocxProps, "revisionView");
            //w:trackRevisions - Track Revisions to Document
            if (m_document.TrackChanges)
            {
                m_writer.WriteStartElement("trackRevisions", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:doNotTrackMoves - Do Not Use Move Syntax When Tracking Revisions
            SerializeDocxProps(tempDocxProps, "doNotTrackMoves");
            //w:doNotTrackFormatting - Do Not Track Formatting Revisions When Tracking Revisions
            SerializeDocxProps(tempDocxProps, "doNotTrackFormatting");
            //w:documentProtection - Document Editing Restrictions
            if (m_document.DOP.Dop2003.EnforceDocProt)
            {
                SerializeProtect((ProtectionType)m_document.DOP.Dop2003.DocProtCur);
            }
            //w:autoFormatOverride - Allow Automatic Formatting to Override Formatting Protection Settings
            SerializeDocxProps(tempDocxProps, "autoFormatOverride");
            //w:styleLockTheme - Prevent Modification of Themes Part
            SerializeDocxProps(tempDocxProps, "styleLockTheme");
            //w:styleLockQFSet - Prevent Replacement of Styles Part
            SerializeDocxProps(tempDocxProps, "styleLockQFSet");
            //w:defaultTabStop - Distance Between Automatic Tab Stops
            m_writer.WriteStartElement("defaultTabStop", DocxConstants.W_namespace);
            int tabWidth = (int)Math.Round(m_document.DefaultTabWidth * DLSConstants.TwipsInOnePoint);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, tabWidth.ToString());
            m_writer.WriteEndElement();
            //w:autoHyphenation - Automatically Hyphenate Document Contents When Displayed
            if (m_document.DOP.AutoHyphen)
            {
                m_writer.WriteStartElement("autoHyphenation", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:consecutiveHyphenLimit - Maximum Number of Consecutively Hyphenated Lines
            if (m_document.DOP.ConsecHypLim > 0)
            {
                m_writer.WriteStartElement("consecutiveHyphenLimit", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, m_document.DOP.ConsecHypLim.ToString());
                m_writer.WriteEndElement();
            }
            //w:hyphenationZone - Hyphenation Zone
            if (m_document.DOP.DxaHotZ != 360)
            {
                m_writer.WriteStartElement("hyphenationZone", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, m_document.DOP.DxaHotZ.ToString());
                m_writer.WriteEndElement();
            }
            //w:doNotHyphenateCaps - Do Not Hyphenate Words in ALL CAPITAL LETTERS
            if (!m_document.DOP.HyphCapitals)
            {
                m_writer.WriteStartElement("doNotHyphenateCaps", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:showEnvelope - Show E-Mail Message Header
            SerializeDocxProps(tempDocxProps, "showEnvelope");
            //w:summaryLength - Percentage of Document to Use When Generating Summary
            SerializeDocxProps(tempDocxProps, "summaryLength");
            //w:clickAndTypeStyle - Paragraph Style Applied to Automatically Generated Paragraphs
            SerializeDocxProps(tempDocxProps, "clickAndTypeStyle");
            //w:defaultTableStyle - Default Table Style for Newly Inserted Tables
            SerializeDocxProps(tempDocxProps, "defaultTableStyle");
            //w:evenAndOddHeaders - Different Even/Odd Page Headers and Footers
            if (m_document.DifferentOddAndEvenPages)
            {
                m_writer.WriteStartElement("evenAndOddHeaders", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:bookFoldRevPrinting - Reverse Book Fold Printing
            SerializeDocxProps(tempDocxProps, "bookFoldRevPrinting");
            //w:bookFoldPrinting - Book Fold Printing
            SerializeDocxProps(tempDocxProps, "bookFoldPrinting");
            //w:bookFoldPrintingSheets - Number of Pages Per Booklet
            SerializeDocxProps(tempDocxProps, "bookFoldPrintingSheets");
            //w:drawingGridHorizontalSpacing - Drawing Grid Horizontal Grid Unit Size
            SerializeDocxProps(tempDocxProps, "drawingGridHorizontalSpacing");
            //w:drawingGridVerticalSpacing - Drawing Grid Vertical Grid Unit Size
            SerializeDocxProps(tempDocxProps, "drawingGridVerticalSpacing");
            //w:displayHorizontalDrawingGridEvery - Distance between Horizontal Gridlines
            SerializeDocxProps(tempDocxProps, "displayHorizontalDrawingGridEvery");
            //w:displayVerticalDrawingGridEvery - Distance between Vertical Gridlines
            SerializeDocxProps(tempDocxProps, "displayVerticalDrawingGridEvery");
            //w:doNotUseMarginsForDrawingGridOrigin - Do Not Use Margins for Drawing Grid Origin
            SerializeDocxProps(tempDocxProps, "doNotUseMarginsForDrawingGridOrigin");
            //w:drawingGridHorizontalOrigin - Drawing Grid Horizontal Origin Point
            SerializeDocxProps(tempDocxProps, "drawingGridHorizontalOrigin");
            //w:drawingGridVerticalOrigin - Drawing Grid Vertical Origin Point
            SerializeDocxProps(tempDocxProps, "drawingGridVerticalOrigin");
            //w:doNotShadeFormData - Do Not Show Visual Indicator For Form Fields
            if (!m_document.DOP.FormFieldShading)
            {
                m_writer.WriteStartElement("doNotShadeFormData", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:noPunctuationKerning - Never Kern Punctuation Characters
            SerializeDocxProps(tempDocxProps, "noPunctuationKerning");
            //w:characterSpacingControl - Character-Level Whitespace Compression
            SerializeDocxProps(tempDocxProps, "characterSpacingControl");
            //w:printTwoOnOne - Print Two Pages Per Sheet
            SerializeDocxProps(tempDocxProps, "printTwoOnOne");
            //w:strictFirstAndLastChars - Use Strict Kinsoku Rules for Japanese Text
            SerializeDocxProps(tempDocxProps, "strictFirstAndLastChars");
            //w:noLineBreaksAfter - Custom Set of Characters Which Cannot End a Line
            SerializeDocxProps(tempDocxProps, "noLineBreaksAfter");
            //w:noLineBreaksBefore - Custom Set Of Characters Which Cannot Begin A Line
            SerializeDocxProps(tempDocxProps, "noLineBreaksBefore");
            //w:savePreviewPicture - Generate Thumbnail For Document On Save
            SerializeDocxProps(tempDocxProps, "savePreviewPicture");
            //w:doNotValidateAgainstSchema - Do Not Validate Custom XML Markup Against Schemas
            SerializeDocxProps(tempDocxProps, "doNotValidateAgainstSchema");
            //w:saveInvalidXml - Allow Saving Document As XML File When Custom XML Markup Is Invalid
            SerializeDocxProps(tempDocxProps, "saveInvalidXml");
            //w:ignoreMixedContent - Ignore Mixed Content When Validating Custom XML Markup
            SerializeDocxProps(tempDocxProps, "ignoreMixedContent");
            //w:alwaysShowPlaceholderText - Use Custom XML Element Names as Default Placeholder Text
            SerializeDocxProps(tempDocxProps, "alwaysShowPlaceholderText");
            //w:doNotDemarcateInvalidXml - Do Not Show Visual Indicator For Invalid Custom XML Markup
            SerializeDocxProps(tempDocxProps, "doNotDemarcateInvalidXml");
            //w:saveXmlDataOnly - Only Save Custom XML Markup
            SerializeDocxProps(tempDocxProps, "saveXmlDataOnly");
            //w:useXSLTWhenSaving - Save Document as XML File through Custom XSL Transform
            SerializeDocxProps(tempDocxProps, "useXSLTWhenSaving");
            //w:saveThroughXslt - Custom XSL Transform To Use When Saving As XML File
            SerializeDocxProps(tempDocxProps, "saveThroughXslt");
            //w:showXMLTags - Show Visual Indicators for Custom XML Markup Start/End Locations
            SerializeDocxProps(tempDocxProps, "showXMLTags");
            //w:alwaysMergeEmptyNamespace - Do Not Mark Custom XML Elements With No Namespace As Invalid
            SerializeDocxProps(tempDocxProps, "alwaysMergeEmptyNamespace");
            //w:updateFields - Automatically Recalculate Fields on Open
            SerializeDocxProps(tempDocxProps, "updateFields");
            //w:hdrShapeDefaults - Default Properties for VML Objects in Header and Footer
            SerializeDocxProps(tempDocxProps, "hdrShapeDefaults");
            //w:footnotePr - Document-Wide Footnote Properties and w:endnotePr - Document-Wide Endnote Properties
            SerializeFootnoteSettings();
            //w:compat - Compatibility Settings
            SerializeCompatSettings();
            //w:docVars - Document Variables
            if (m_document.Variables.Count > 0)
            {
                SerializeDocVariables(m_document.Variables);
            }
            //w:rsids - Listing of All Revision Save ID Values
            SerializeDocxProps(tempDocxProps, "rsids");
            //m:mathPr - properties of math in the document
            SerializeDocxProps(tempDocxProps, "mathPr");
            //w:uiCompat97To2003 - Disable Features Incompatible With Earlier Word Processing Formats
            SerializeDocxProps(tempDocxProps, "uiCompat97To2003");
            //w:attachedSchema -   Attached Custom XML Schema
            SerializeDocxProps(tempDocxProps, "attachedSchema");
            //w:themeFontLang - Theme Font Languages
            SerializeDocxProps(tempDocxProps, "themeFontLang");
            //w:clrSchemeMapping - Theme Color Mappings
            SerializeDocxProps(tempDocxProps, "clrSchemeMapping");
            //w:doNotIncludeSubdocsInStats - Do Not Include Content in Text Boxes, Footnotes, and Endnotes in Document Statistics
            SerializeDocxProps(tempDocxProps, "doNotIncludeSubdocsInStats");
            //w:doNotAutoCompressPictures - Do Not Automatically Compress Images
            SerializeDocxProps(tempDocxProps, "doNotAutoCompressPictures");
            //w:forceUpgrade - Upgrade Document on Open
            SerializeDocxProps(tempDocxProps, "forceUpgrade");
            //w:captions - Caption Settings
            SerializeDocxProps(tempDocxProps, "captions");
            //w:readModeInkLockDown - Freeze Document Layout
            SerializeDocxProps(tempDocxProps, "readModeInkLockDown");
            //w:smartTagType -    Supplementary Smart Tag Information
            SerializeDocxProps(tempDocxProps, "smartTagType");
            //sl:schemaLibrary - Custom XML Schema List
            SerializeDocxProps(tempDocxProps, "schemaLibrary");
            //w:shapeDefaults - Default Properties for VML Objects in Main Document
            SerializeDocxProps(tempDocxProps, "shapeDefaults");
            //w:doNotEmbedSmartTags - Remove Smart Tags When Saving
            SerializeDocxProps(tempDocxProps, "doNotEmbedSmartTags");
            //w:decimalSymbol - Radix Point for Field Code Evaluation
            SerializeDocxProps(tempDocxProps, "decimalSymbol");
            //w:listSeparator - List Separator for Field Code Evaluation
            SerializeDocxProps(tempDocxProps, "listSeparator");

            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.SettingsPath, settingsStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the setting relations
        /// </summary>
        private void SerializeSettingsRelation()
        {
            if (SettingsRelations.Count == 0)
                return;

            MemoryStream stream = new MemoryStream();
            m_writer = CreateWriter(stream);
            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            foreach (KeyValuePair<string, DictionaryEntry> keyValue in SettingsRelations)
            {
                DictionaryEntry entry = SettingsRelations[keyValue.Key.ToString()];
                SerializeRelationShip(stream, keyValue.Key.ToString(), entry.Key.ToString(), entry.Value.ToString());
            }
            m_writer.WriteEndElement();
            m_writer.Flush();
            m_archive.AddItem(DocxConstants.SettingsRelationpath, stream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Parses the settings relations.
        /// </summary>
        private void ParseSettingsRelations()
        {
            if (m_document.DocxPackage!=null && m_document.DocxPackage.XmlPartContainers.ContainsKey("word/"))
            {
                PartContainer partContainer = m_document.DocxPackage.FindPartContainer("word/");
                if (partContainer != null && partContainer.Relations.ContainsKey("word/_rels/settings.xml.rels"))
                {
                    Stream settingsStream = partContainer.Relations["word/_rels/settings.xml.rels"].DataStream;
                    settingsStream.Position = 0;
                    XmlReader hfRelReader = UtilityMethods.CreateReader(settingsStream);
                    ParseRelations(hfRelReader, SettingsRelations);
                }
            }
        }
        /// <summary>
        /// Serializes the Docx unhandled properties
        /// </summary>
        private void SerializeDocxProps(List<Stream> tempDocxProps, string propertyName)
        {
            for (int i = 0, cnt = tempDocxProps.Count; i < cnt; i++)
            {
                XmlReader reader = CreateReader(tempDocxProps[i]);
                reader.MoveToContent();
                if (reader.LocalName == propertyName)
                {
                    tempDocxProps.RemoveAt(i);
                    m_writer.WriteNode(reader, false);
                    break;
                }
            }
        }
        /// <summary>
        /// Serializes the compatibility settings
        /// </summary>
        private void SerializeCompatSettings()
        {
            m_writer.WriteStartElement("compat", DocxConstants.W_namespace);
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.OrigWordTableRules])
            {
                m_writer.WriteStartElement("useSingleBorderforContiguousCells", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.WPJust])
            {
                m_writer.WriteStartElement("wpJustification", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.NoTabForInd])
            {
                m_writer.WriteStartElement("noTabHangInd", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.NoExtLeading])
            {
                m_writer.WriteStartElement("noLeading", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (!m_document.Settings.CompatibilityOptions[CompatibilityOption.DontMakeSpaceForUL])
            {
                m_writer.WriteStartElement("spaceForUL", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.NoColumnBalance])
            {
                m_writer.WriteStartElement("noColumnBalance", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (!m_document.Settings.CompatibilityOptions[CompatibilityOption.DntBlnSbDbWid])
            {
                m_writer.WriteStartElement("balanceSingleByteDoubleByteWidth", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.ExactOnTop])
            {
                m_writer.WriteStartElement("noExtraLineSpacing", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (!m_document.Settings.CompatibilityOptions[CompatibilityOption.LeaveBackslashAlone])
            {
                m_writer.WriteStartElement("doNotLeaveBackslashAlone", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (!m_document.Settings.CompatibilityOptions[CompatibilityOption.DntULTrlSpc])
            {
                m_writer.WriteStartElement("ulTrailSpace", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (!m_document.Settings.CompatibilityOptions[CompatibilityOption.ExpShRtn])
            {
                m_writer.WriteStartElement("doNotExpandShiftReturn", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.TruncDxaExpand])
            {
                m_writer.WriteStartElement("spacingInWholePoints", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.LineWrapLikeWord6])
            {
                m_writer.WriteStartElement("lineWrapLikeWord6", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.PrintBodyBeforeHdr])
            {
                m_writer.WriteStartElement("printBodyTextBeforeHeader", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.MapPrintTextColor])
            {
                m_writer.WriteStartElement("printColBlack", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.WPSpace])
            {
                m_writer.WriteStartElement("wpSpaceWidth", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.ShowBreaksInFrames])
            {
                m_writer.WriteStartElement("showBreaksInFrames", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SubOnSize])
            {
                m_writer.WriteStartElement("subFontBySize", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.ExtraAfter])
            {
                m_writer.WriteStartElement("suppressBottomSpacing", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SuppressTopSpacing])
            {
                m_writer.WriteStartElement("suppressTopSpacing", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SuppressTopSpacingMac5])
            {
                m_writer.WriteStartElement("suppressSpacingAtTopOfPage", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.F2ptExtLeadingOnly])
            {
                m_writer.WriteStartElement("suppressTopSpacingWP", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SuppressSpBfAfterPgBrk])
            {
                m_writer.WriteStartElement("suppressSpBfAfterPgBrk", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SwapBordersFacingPgs])
            {
                m_writer.WriteStartElement("swapBordersFacingPages", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.ConvMailMergeEsc])
            {
                m_writer.WriteStartElement("convMailMergeEsc", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.TruncFontHeight])
            {
                m_writer.WriteStartElement("truncateFontHeightsLikeWP6", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.MWSmallCaps])
            {
                m_writer.WriteStartElement("mwSmallCaps", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.PrintMet])
            {
                m_writer.WriteStartElement("usePrinterMetrics", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.WW6BorderRules])
            {
                m_writer.WriteStartElement("doNotSuppressParagraphBorders", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.WrapTrailSpaces])
            {
                m_writer.WriteStartElement("wrapTrailSpaces", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.FtnLayoutLikeWW8])
            {
                m_writer.WriteStartElement("footnoteLayoutLikeWW8", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SpLayoutLikeWW8])
            {
                m_writer.WriteStartElement("shapeLayoutLikeWW8", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.AlignTablesRowByRow])
            {
                m_writer.WriteStartElement("alignTablesRowByRow", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.ForgetLastTabAlign])
            {
                m_writer.WriteStartElement("forgetLastTabAlignment", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (!m_document.Settings.CompatibilityOptions[CompatibilityOption.DontAdjustLineHeightInTable])
            {
                m_writer.WriteStartElement("adjustLineHeightInTable", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.UseAutospaceForFullWidthAlpha])
            {
                m_writer.WriteStartElement("autoSpaceLikeWord95", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.NoSpaceRaiseLower])
            {
                m_writer.WriteStartElement("noSpaceRaiseLower", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontUseHTMLParagraphAutoSpacing])
            {
                m_writer.WriteStartElement("doNotUseHTMLParagraphAutoSpacing", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.LayoutRawTableWidth])
            {
                m_writer.WriteStartElement("layoutRawTableWidth", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.LayoutTableRowsApart])
            {
                m_writer.WriteStartElement("layoutTableRowsApart", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.UseWord97LineBreakingRules])
            {
                m_writer.WriteStartElement("useWord97LineBreakRules", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontBreakWrappedTables])
            {
                m_writer.WriteStartElement("doNotBreakWrappedTables", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontSnapToGridInCell])
            {
                m_writer.WriteStartElement("doNotSnapToGridInCell", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontAllowFieldEndSelect])
            {
                m_writer.WriteStartElement("selectFldWithFirstOrLastChar", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.ApplyBreakingRules])
            {
                m_writer.WriteStartElement("applyBreakingRules", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontWrapTextWithPunct])
            {
                m_writer.WriteStartElement("doNotWrapTextWithPunct", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontUseAsianBreakRules])
            {
                m_writer.WriteStartElement("doNotUseEastAsianBreakRules", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.UseWord2002TableStyleRules])
            {
                m_writer.WriteStartElement("useWord2002TableStyleRules", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.GrowAutoFit])
            {
                m_writer.WriteStartElement("growAutofit", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.UseNormalStyleForList])
            {
                m_writer.WriteStartElement("useNormalStyleForList", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontUseIndentAsNumberingTabStop])
            {
                m_writer.WriteStartElement("doNotUseIndentAsNumberingTabStop", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.FELineBreak11])
            {
                m_writer.WriteStartElement("useAltKinsokuLineBreakRules", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.AllowSpaceOfSameStyleInTable])
            {
                m_writer.WriteStartElement("allowSpaceOfSameStyleInTable", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.WW11IndentRules])
            {
                m_writer.WriteStartElement("doNotSuppressIndentation", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontAutofitConstrainedTables])
            {
                m_writer.WriteStartElement("doNotAutofitConstrainedTables", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.AutofitLikeWW11])
            {
                m_writer.WriteStartElement("autofitToFirstFixedWidthCell", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.UnderlineTabInNumList])
            {
                m_writer.WriteStartElement("underlineTabInNumList", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.HangulWidthLikeWW11])
            {
                m_writer.WriteStartElement("displayHangulFixedWidth", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.SplitPgBreakAndParaMark])
            {
                m_writer.WriteStartElement("splitPgBreakAndParaMark", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontVertAlignCellWithSp])
            {
                m_writer.WriteStartElement("doNotVertAlignCellWithSp", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontBreakConstrainedForcedTables])
            {
                m_writer.WriteStartElement("doNotBreakConstrainedForcedTable", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.DontVertAlignInTxbx])
            {
                m_writer.WriteStartElement("doNotVertAlignInTxbx", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.Word11KerningPairs])
            {
                m_writer.WriteStartElement("useAnsiKerningPairs", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            if (m_document.Settings.CompatibilityOptions[CompatibilityOption.CachedColBalance])
            {
                m_writer.WriteStartElement("cachedColBalance", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            string compatSetting = "15";
            if (m_document.Settings.CompatibilityMode == CompatibilityMode.Word2003)
                compatSetting = "11";
            if (m_document.Settings.CompatibilityMode == CompatibilityMode.Word2007)
                compatSetting = "12";
            if (m_document.Settings.CompatibilityMode == CompatibilityMode.Word2010)
                compatSetting = "14";
            if (IsWord2007)
                compatSetting = "12";
            if (IsWord2010)
                compatSetting = "14";
            m_writer.WriteStartElement("compatSetting", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "name", DocxConstants.W_namespace, "compatibilityMode");
            m_writer.WriteAttributeString("w", "uri", DocxConstants.W_namespace, "http://schemas.microsoft.com/office/word");
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, compatSetting);
            m_writer.WriteEndElement();

            if (IsWord2010 || IsWord2013)
            {
                bool writeSettings = true;
                if (m_document.Settings.CompatibilityOptions.PropertiesHash.ContainsKey(CompatibilityOption.overrideTableStyleFontSizeAndJustification))
                    writeSettings = m_document.Settings.CompatibilityOptions[CompatibilityOption.overrideTableStyleFontSizeAndJustification];
                if (writeSettings)
                {
                    m_writer.WriteStartElement("compatSetting", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "name", DocxConstants.W_namespace, "overrideTableStyleFontSizeAndJustification");
                    m_writer.WriteAttributeString("w", "uri", DocxConstants.W_namespace, "http://schemas.microsoft.com/office/word");
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "1");
                    m_writer.WriteEndElement();
                }
                writeSettings = true;
                if (m_document.Settings.CompatibilityOptions.PropertiesHash.ContainsKey(CompatibilityOption.enableOpenTypeFeatures))
                    writeSettings = m_document.Settings.CompatibilityOptions[CompatibilityOption.enableOpenTypeFeatures];
                if (writeSettings)
                {
                    m_writer.WriteStartElement("compatSetting", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "name", DocxConstants.W_namespace, "enableOpenTypeFeatures");
                    m_writer.WriteAttributeString("w", "uri", DocxConstants.W_namespace, "http://schemas.microsoft.com/office/word");
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "1");
                    m_writer.WriteEndElement();
                }
                writeSettings = true;
                if (m_document.Settings.CompatibilityOptions.PropertiesHash.ContainsKey(CompatibilityOption.doNotFlipMirrorIndents))
                    writeSettings = m_document.Settings.CompatibilityOptions[CompatibilityOption.doNotFlipMirrorIndents];
                if (writeSettings)
                {
                    m_writer.WriteStartElement("compatSetting", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("w", "name", DocxConstants.W_namespace, "doNotFlipMirrorIndents");
                    m_writer.WriteAttributeString("w", "uri", DocxConstants.W_namespace, "http://schemas.microsoft.com/office/word");
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "1");
                    m_writer.WriteEndElement();
                }
                if (IsWord2013)
                {
                    writeSettings = true;
                    if (m_document.Settings.CompatibilityOptions.PropertiesHash.ContainsKey(CompatibilityOption.doNotFlipMirrorIndents))
                        writeSettings = m_document.Settings.CompatibilityOptions[CompatibilityOption.doNotFlipMirrorIndents];
                    if (writeSettings)
                    {
                        m_writer.WriteStartElement("compatSetting", DocxConstants.W_namespace);
                        m_writer.WriteAttributeString("w", "name", DocxConstants.W_namespace, "differentiateMultirowTableHeader");
                        m_writer.WriteAttributeString("w", "uri", DocxConstants.W_namespace, "http://schemas.microsoft.com/office/word");
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "1");
                        m_writer.WriteEndElement();
                    }
                }
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the document protection type.
        /// </summary>
        /// <param name="protectionType"></param>
        private void SerializeProtect(ProtectionType protectionType)
        {
            string type = string.Empty;
            switch (protectionType)
            {
                case ProtectionType.AllowOnlyComments:
                    type = "comments";
                    break;
                case ProtectionType.AllowOnlyFormFields:
                    type = "forms";
                    break;
                case ProtectionType.AllowOnlyRevisions:
                    type = "trackedChanges";
                    break;
                case ProtectionType.AllowOnlyReading:
                    type = "readOnly";
                    break;
            }

            if (type != string.Empty)
            {
                m_writer.WriteStartElement("documentProtection", DocxConstants.W_namespace);
                //Serialize Document Protection type and Enforcement
                if (m_document.DOP.m_bLockRev)
                {
                    m_writer.WriteAttributeString("edit", DocxConstants.W_namespace, "trackedChanges");
                    m_writer.WriteAttributeString("enforcement", DocxConstants.W_namespace, "1");
                }
                else if (m_document.DOP.m_bProtEnabled)
                {
                    m_writer.WriteAttributeString("edit", DocxConstants.W_namespace, "forms");
                    m_writer.WriteAttributeString("enforcement", DocxConstants.W_namespace, "1");
                }
                else if (m_document.DOP.m_bLockAtn)
                {
                    if (m_document.DOP.Dop2003.TreatLockAtnAsReadOnly)
                    {
                        m_writer.WriteAttributeString("edit", DocxConstants.W_namespace, "readOnly");
                        m_writer.WriteAttributeString("enforcement", DocxConstants.W_namespace, "1");
                    }
                    else
                    {
                        m_writer.WriteAttributeString("edit", DocxConstants.W_namespace, "comments");
                        m_writer.WriteAttributeString("enforcement", DocxConstants.W_namespace, "1");
                    }
                }
                else
                {
                    m_writer.WriteAttributeString("edit", DocxConstants.W_namespace, type);
                    m_writer.WriteAttributeString("enforcement", DocxConstants.W_namespace, "0");
                }
                if (m_document.DOP.ProtectionKey != 0)
                {
                    DocxProtection protection = new DocxProtection();
                    m_writer.WriteAttributeString("cryptProviderType", DocxConstants.W_namespace, DocxProtection.CryptographicType);
                    m_writer.WriteAttributeString("cryptAlgorithmClass", DocxConstants.W_namespace, DocxProtection.CryptographicAlgorithmClass);
                    m_writer.WriteAttributeString("cryptAlgorithmType", DocxConstants.W_namespace, DocxProtection.CryptographicAlgorithmType);
                    m_writer.WriteAttributeString("cryptAlgorithmSid", DocxConstants.W_namespace, DocxProtection.CryptographicAlgorithmId.ToString());
                    m_writer.WriteAttributeString("cryptSpinCount", DocxConstants.W_namespace, DocxProtection.SpinCount.ToString());
                    byte[] salt = protection.CreateSalt(16);
                    byte[] hash = protection.ComputeHash(salt, m_document.DOP.ProtectionKey);
                    m_writer.WriteAttributeString("hash", DocxConstants.W_namespace, Convert.ToBase64String(hash));
                    m_writer.WriteAttributeString("salt", DocxConstants.W_namespace, Convert.ToBase64String(salt));
                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the document variables.
        /// </summary>
        /// <param name="docVariables"></param>
        private void SerializeDocVariables(DocVariables docVariables)
        {
            m_writer.WriteStartElement("docVars", DocxConstants.W_namespace);

            string value = string.Empty;
            foreach (string name in docVariables.Items.Keys)
            {
                if (docVariables.Items.ContainsKey(name))
                {
                    value = docVariables.Items[name];
                }
                m_writer.WriteStartElement("docVar", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("name", DocxConstants.W_namespace, name);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, value);
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();//end of docVars
        }
        /// <summary>
        /// Serializes the Footnote/Endnote settings
        /// </summary>
        private void SerializeFootnoteSettings()
        {
            if (HasFootnote)
            {
                m_writer.WriteStartElement("footnotePr", DocxConstants.W_namespace);

                SerializeFootnotePosition();
                SerializeEndnoteFootnoteNumberFormat(true);
                if (m_document.InitialFootnoteNumber != 0)
                    SerializeEndnoteFootnoteElement("numStart", m_document.InitialFootnoteNumber.ToString());
                if (m_document.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachPage)
                {
                    SerializeEndnoteFootnoteElement("numRestart", "eachPage");
                }
                else if (m_document.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachSection)
                {
                    SerializeEndnoteFootnoteElement("numRestart", "eachSect");
                }
                //Handled to write footnote separator id's similar to MS Word.
                WriteFootEndnoteID(true, -1);
                WriteFootEndnoteID(true, 0);
                if (m_document.Footnotes.ContinuationNotice.Count > 0)
                    WriteFootEndnoteID(true, 1);
                m_writer.WriteEndElement();
            }

            if (HasEndnote)
            {
                m_writer.WriteStartElement("endnotePr", DocxConstants.W_namespace);

                WriteEntPosition();
                SerializeEndnoteFootnoteNumberFormat(false);
                if (m_document.InitialEndnoteNumber != 0)
                    SerializeEndnoteFootnoteElement("numStart", m_document.InitialEndnoteNumber.ToString());
                if (m_document.RestartIndexForEndnote == EndnoteRestartIndex.RestartForEachSection)
                {
                    SerializeEndnoteFootnoteElement("numRestart", "eachSect");
                }
                //Handled to write endnote separator id's similar to MS Word.
                WriteFootEndnoteID(false, -1);
                WriteFootEndnoteID(false, 0);
                if (m_document.Endnotes.ContinuationNotice.Count > 0)
                    WriteFootEndnoteID(false, 1);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the Footnote position
        /// </summary>
        private void SerializeFootnotePosition()
        {
            if (m_document.FootnotePosition == FootnotePosition.PrintImmediatelyBeneathText)
            {
                m_writer.WriteStartElement("pos", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "beneathText");
                m_writer.WriteEndElement();
            }
            else if (m_document.FootnotePosition == FootnotePosition.PrintAsEndnotes)
            {
                WriteEntPosition();
            }
        }
        /// <summary>
        /// Serialize the Endnote position
        /// </summary>
        private void WriteEntPosition()
        {
            if (m_document.EndnotePosition == EndnotePosition.DisplayEndOfSection)
            {
                m_writer.WriteStartElement("pos", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "sectEnd");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the Footnote position
        /// </summary>
        private void SerializeFootnotePosition(WSection section)
        {
            if (section.PageSetup.FootnotePosition == FootnotePosition.PrintImmediatelyBeneathText)
            {
                m_writer.WriteStartElement("pos", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "beneathText");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the Footnote/Endnote ID
        /// </summary>
        /// <param name="isFootnote"></param>
        /// <param name="id"></param>
        private void WriteFootEndnoteID(bool isFootnote, int id)
        {
            string type = (isFootnote) ? "footnote" : "endnote";

            m_writer.WriteStartElement(type, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, id.ToString());
            m_writer.WriteEndElement();
        }
        #endregion Settings

        #region Numberings
        /// <summary>
        /// Serialize the list styles and numberings (numberings.xml)
        /// </summary>
        private void SerializeNumberings()
        {
            if (m_document.ListStyles.Count == 0 && m_document.ListOverrides.Count == 0)
                return;

            MemoryStream listStream = new MemoryStream();
            m_writer = CreateWriter(listStream);
            HasNumbering = true;


            m_writer.WriteStartElement("w", "numbering", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "wpc", null, DocxConstants.WPC_namesapce);
            m_writer.WriteAttributeString("xmlns", "mc", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "o", null, DocxConstants.O_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "m", null, DocxConstants.M_namespace);
            m_writer.WriteAttributeString("xmlns", "v", null, DocxConstants.V_namespace);
            m_writer.WriteAttributeString("xmlns", "wp14", null, DocxConstants.WP14_namespace);
            m_writer.WriteAttributeString("xmlns", "wp", null, DocxConstants.WP_namespace);
            m_writer.WriteAttributeString("xmlns", "w10", null, DocxConstants.W10_namespace);
            m_writer.WriteAttributeString("xmlns", "w", null, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("xmlns", "wpg", null, DocxConstants.WPG_namespace);
            m_writer.WriteAttributeString("xmlns", "wpi", null, DocxConstants.WPI_namespace);
            m_writer.WriteAttributeString("xmlns", "wne", null, DocxConstants.WNE_namespace);
            m_writer.WriteAttributeString("xmlns", "wps", null, DocxConstants.WPS_namespace);
            m_writer.WriteAttributeString("mc", "Ignorable", null, "w14 w15 wp14");

            if (m_document.ListStyles.Count > 0)
            {
                SerializePictureBullets(m_document.ListStyles);
                SerializeAbstractListStyles(m_document.ListStyles);
                SerializeListInstances(m_document.ListStyles);
                SerializeListOverrides(m_document.ListOverrides);
            }
            m_writer.WriteEndElement();
            m_writer.Flush();
            m_archive.AddItem(DocxConstants.NumberingPath, listStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the Override styles
        /// </summary>
        /// <param name="listOverrideStyles">Collection of ListOverride style</param>
        private void SerializeListOverrides(ListOverrideStyleCollection listOverrideStyles)
        {
            if (m_lstStyleReferences == null || m_lstStyleReferences.Count == 0)
                return;

            foreach (int abstractId in m_lstStyleReferences.Keys)
            {
                Dictionary<int, string> overrideStyleNames = m_lstStyleReferences[abstractId];
                //int id = 0;
                string lfoName = string.Empty;
                foreach (int id in overrideStyleNames.Keys)
                {
                    lfoName = overrideStyleNames[id];
                    m_writer.WriteStartElement("num", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("numId", DocxConstants.W_namespace, id.ToString());
                    m_writer.WriteStartElement("abstractNumId", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, abstractId.ToString());
                    m_writer.WriteEndElement();

                    ListOverrideStyle overStyle = listOverrideStyles.FindByName(lfoName);
                    if (overStyle != null)
                    {
                        SerializeOverrideStyle(overStyle);
                    }

                    m_writer.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// Serializes the Override styles
        /// </summary>
        /// <param name="listOverrideStyle">List Override style</param>
        private void SerializeOverrideStyle(ListOverrideStyle listOverrideStyle)
        {
            foreach (KeyValuePair<int, int> keyValuePair in listOverrideStyle.OverrideLevels.LevelIndex)
            {
                SerializeOverrideLevel(keyValuePair.Key, listOverrideStyle.OverrideLevels[keyValuePair.Key]);
            }
        }
        /// <summary>
        /// Serializes the level overrides
        /// </summary>
        /// <param name="levelIndex">The level Index</param>
        /// <param name="level">Override level</param>
        private void SerializeOverrideLevel(int levelIndex, OverrideLevelFormat level)
        {
            m_writer.WriteStartElement("lvlOverride", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("ilvl", DocxConstants.W_namespace, levelIndex.ToString());
            if (level.OverrideStartAtValue)
            {
                m_writer.WriteStartElement("startOverride", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, level.StartAt.ToString());
                m_writer.WriteEndElement();
            }

            if (level.OverrideFormatting)
            {
                SerializeListLevel(level.OverrideListLevel, levelIndex);
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the list styles
        /// </summary>
        /// <param name="listStyles">Collection of list styles</param>
        private void SerializeListInstances(ListStyleCollection listStyles)
        {
            for (int i = 0; i < listStyles.Count; i++)
            {
                m_writer.WriteStartElement("num", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("numId", DocxConstants.W_namespace, (i + 1).ToString());
                m_writer.WriteStartElement("abstractNumId", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, i.ToString());
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the abstract list styles
        /// </summary>
        /// <param name="listStyles">Collecgtion of list styles</param>
        private void SerializeAbstractListStyles(ListStyleCollection listStyles)
        {
            int abstractIndex = 0;
            foreach (ListStyle lstStyle in listStyles)
            {
                m_writer.WriteStartElement("abstractNum", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("abstractNumId", DocxConstants.W_namespace, abstractIndex.ToString());
                for (int ilvl = 0, cnt = lstStyle.Levels.Count; ilvl < cnt; ilvl++)
                {
                    SerializeListLevel(lstStyle.Levels[ilvl], ilvl);
                }
                m_writer.WriteEndElement();//end of abstractNum
                abstractIndex += 1;
            }
        }
        /// <summary>
        /// Serialize the list level
        /// </summary>
        /// <param name="listLevel">The List level</param>
        /// <param name="levelIndex">The level index</param>
        private void SerializeListLevel(WListLevel listLevel, int levelIndex)
        {
            m_writer.WriteStartElement("lvl", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("ilvl", DocxConstants.W_namespace, levelIndex.ToString());

            m_writer.WriteStartElement("start", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, listLevel.StartAt.ToString());
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("numFmt", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, GetPatternType(listLevel));
            m_writer.WriteEndElement();

            if (listLevel.NoRestartByHigher)
            {
                m_writer.WriteStartElement("lvlRestart", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }

            if (listLevel.ParaStyleName != null)
            {
                string name = listLevel.ParaStyleName.Substring(0, 1).ToUpper() + listLevel.ParaStyleName.Remove(0, 1);

                m_writer.WriteStartElement("pStyle", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, name);
                m_writer.WriteEndElement();
            }

            if (listLevel.IsLegalStyleNumbering)
            {
                m_writer.WriteStartElement("isLgl", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            SerializeLevelFollow(listLevel);
            SerializeLevelText(listLevel, levelIndex + 1);
            SerializeLegacyProperties(listLevel);

            if (listLevel.PicBulletId > 0)
            {
                m_writer.WriteStartElement("lvlPicBulletId", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, listLevel.PicBulletId.ToString());
                m_writer.WriteEndElement();
            }

            //lvlJc
            if (listLevel.NumberAlignment != ListNumberAlignment.Left)
            {
                m_writer.WriteStartElement("lvlJc", DocxConstants.W_namespace);
                string alignment = string.Empty;

                if (listLevel.NumberAlignment == ListNumberAlignment.Right)
                {
                    alignment = "right";
                }
                else
                {
                    alignment = "center";
                }
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, alignment);

                m_writer.WriteEndElement();
            }
            m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
            SerializeParagraphFormat(listLevel.ParagraphFormat, null);
            m_writer.WriteEndElement();//end of pPr
            SerializeCharactetFormat(listLevel.CharacterFormat);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the list level legacy properties.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        private void SerializeLegacyProperties(WListLevel listLevel)
        {
            if (listLevel.Word6Legacy)
            {
                m_writer.WriteStartElement("legacy", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("legacy", DocxConstants.W_namespace, "1");
                m_writer.WriteAttributeString("legacySpace", DocxConstants.W_namespace, listLevel.LegacySpace.ToString());
                m_writer.WriteAttributeString("legacyIndent", DocxConstants.W_namespace, listLevel.LegacyIndent.ToString());
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the level follow character
        /// </summary>
        /// <param name="listLevel">The list level</param>
        private void SerializeLevelFollow(WListLevel listLevel)
        {
            string followCharacter = string.Empty;
            if (listLevel.FollowCharacter == FollowCharacterType.Space)
                followCharacter = "space";
            else if (listLevel.FollowCharacter == FollowCharacterType.Tab)
                followCharacter = "tab";
            else
                followCharacter = "nothing";

            m_writer.WriteStartElement("suff", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, followCharacter);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// To Remove xml ilegal character Ascii 0-29 from text.
        /// </summary>
        /// <param name="listLevel">The text.</param>        
        private string RemoveIllegalXMLCharacters(string text)
        {
            char[] illegaChar = new char[30];
            for (int i = 0; i < 30; i++)
               text =  text.Replace((char)i, '\0');
                //illegaChar[i] = (char)i;
            return text == null ? text : text.Replace("\0",string.Empty);
        }
        /// <summary>
        /// Serializes the level text
        /// </summary>
        /// <param name="listLevel">The list level</param>
        /// <param name="lvlIndex">The level index</param>
        private void SerializeLevelText(WListLevel listLevel, int lvlIndex)
        {
            m_writer.WriteStartElement("lvlText", DocxConstants.W_namespace);
            bool preserveLevelText = false;
            if (listLevel.PatternType == ListPatternType.None &&
                    listLevel.BulletCharacter != null && listLevel.BulletCharacter.Length > 0 &&
                    listLevel.ParaStyleName == null)
                preserveLevelText = true;
            if (listLevel.PatternType == ListPatternType.Bullet || preserveLevelText)
            {
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, RemoveIllegalXMLCharacters(listLevel.BulletCharacter));
            }
            else
            {
                string numPrefix = string.Empty;
                if (listLevel.NumberPrefix != null && listLevel.NumberPrefix.Length > 0)
                {
                    numPrefix = UpdateNumberPrefix(listLevel.NumberPrefix);
                }

                string levelText = numPrefix;
                if (!listLevel.NoLevelText && listLevel.NumberSufix != null)
                {
                    char listSymbol = GetListSymbol(listLevel.LevelNumber);
                    listLevel.NumberSufix = listLevel.NumberSufix.Replace(listSymbol.ToString(), string.Empty);
                    char[] nullChar = new char[] { '\0' };
                    levelText += "%" + lvlIndex.ToString() + listLevel.NumberSufix.Trim(nullChar);
                }

                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, levelText);
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the picture bullets.
        /// </summary>
        /// <param name="listStyleCollection">Collection of list styles</param>
        private void SerializePictureBullets(ListStyleCollection listStyleCollection)
        {
            foreach (ListStyle listStyle in listStyleCollection)
            {
                for (int i = 0, count = listStyle.Levels.Count; i < count; i++)
                {
                    WListLevel listLevel = listStyle.Levels[i];
                    if (listLevel.PicBullet != null)
                    {
                        SerializePictureBullet(listLevel);
                    }
                }
            }
        }
        /// <summary>
        /// Serializes the picture bullet.
        /// </summary>
        /// <param name="listLevel">The list level</param>
        private void SerializePictureBullet(WListLevel listLevel)
        {
            WPicture pic = listLevel.PicBullet;
            string id = AddImageRelation(PictureBullets, pic.ImageRecord);
            m_hasImages = true;

            int name = GetNextID();
            listLevel.PicBulletId = (short)name;

            StringBuilder strBuild = new StringBuilder("width:");
            strBuild.Append(pic.Width.ToString(CultureInfo.InvariantCulture));
            strBuild.Append("pt;height:");
            strBuild.Append(pic.Height.ToString(CultureInfo.InvariantCulture));
            strBuild.Append("pt");
            strBuild.Replace(",", ".");

            m_writer.WriteStartElement("numPicBullet", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("numPicBulletId", DocxConstants.W_namespace, name.ToString());
            m_writer.WriteStartElement("pict", DocxConstants.W_namespace);
            m_writer.WriteStartElement("shape", DocxConstants.V_namespace);

            m_writer.WriteAttributeString("type", "#_x0000_t75");
            m_writer.WriteAttributeString("style", strBuild.ToString());
            m_writer.WriteAttributeString("bullet", DocxConstants.O_namespace, "t");
            if (!listLevel.IsEmptyPicture)
            {
                m_writer.WriteStartElement("imagedata", DocxConstants.V_namespace);
                m_writer.WriteAttributeString("id", DocxConstants.R_namespace, id);
                m_writer.WriteAttributeString("title", DocxConstants.O_namespace, string.Empty);
                m_writer.WriteEndElement();
            }

            m_writer.WriteEndElement();//end of shape
            m_writer.WriteEndElement();//end of pict
            m_writer.WriteEndElement();//end of numPicBullet
        }
        #endregion Numberings

        #region Styles
        /// <summary>
        /// Serialize the styles (styles.xml)
        /// </summary>
        private void SerializeStyles()
        {
            MemoryStream styleStream = new MemoryStream();
            m_writer = CreateWriter(styleStream);


            m_writer.WriteStartElement("w", "styles", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "mc", null, DocxConstants.VE_namespace);
            m_writer.WriteAttributeString("xmlns", "r", null, DocxConstants.R_namespace);
            m_writer.WriteAttributeString("xmlns", "w", null, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("xmlns", "w14", null, DocxConstants.W14_namespace);
            m_writer.WriteAttributeString("xmlns", "w15", null, DocxConstants.W15_namespace);
            m_writer.WriteAttributeString("mc", "Ignorable", null, "w14 w15");
            //writes the document defaults, latent styles and default styles.
            SerializeDefaultStyles();
            //writes the document styles
            SerializeDocumentStyles();

            m_writer.WriteEndElement();//end of styles tag
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.StylePath, styleStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the document styles
        /// </summary>
        private void SerializeDocumentStyles()
        {
            Dictionary<string, int> styleNames = new Dictionary<string, int>();

            foreach (Style style in m_document.Styles)
            {
                while (styleNames.ContainsKey(style.Name))
                {
                    style.IsCustom = true;
                    string name = style.Name;
                    style.SetStyleName(style.Name + "_" + styleNames[style.Name].ToString());
                    styleNames[name] += 1;
                }

                SerializeStyle(style, m_document);
                styleNames.Add(style.Name, 0);
            }
        }
        /// <summary>
        /// Serialize the document style
        /// </summary>
        /// <param name="style">The Style</param>
        /// <param name="document">Instance of the word document</param>
        private void SerializeStyle(Style style, WordDocument document)
        {

            string styleName = style.Name;
            string baseStyleName = string.Empty;
            string styleType = string.Empty;
            switch (style.TypeCode)
            {
                case WordStyleType.TableStyle:
                    if (styleName == "TableNormal" || styleName == "Table Normal" || styleName == "NormalTable" || styleName == "Normal Table")
                        return;
                    styleType = "table";
                    break;
                case WordStyleType.ListStyle:
                    styleType = "numbering";
                    break;
                default:
                    styleType = (style is WTableStyle) ? "table" : (style is WParagraphStyle) ? "paragraph" : "character";
                    break;
            }
            m_writer.WriteStartElement("w", "style", DocxConstants.W_namespace);

            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, styleType);

            string styleId = styleName;
            Dictionary<string, string> styleNames = document.StyleNameIds;
            if (styleNames.ContainsValue(styleName))
            {
                foreach (string key in styleNames.Keys)
                {
                    if (styleNames[key] == styleName)
                    {
                        m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, key);
                        break;
                    }
                }
            }
            else
            {
                if (styleId == "Normal Table")
                    styleId = "TableNormal";
                m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, styleId.Replace(" ", string.Empty));
            }

            if (style.BuiltinStyles.ContainsKey(styleName.ToLower()))
                style.IsCustom = false;
            else
                style.IsCustom = true;

            if (style.IsCustom)
            {
                m_writer.WriteAttributeString("customStyle", DocxConstants.W_namespace, "1");
            }
            else if (IsDefaultStyle(style))
            {
                m_writer.WriteAttributeString("default", DocxConstants.W_namespace, "1");
            }

            //w:name -   Primary Style Name
            m_writer.WriteStartElement("name", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, styleName);
            m_writer.WriteEndElement();
            //w:aliases [0..1]    Alternate Style Names
            //w:basedOn -   Parent Style ID
            if (style.BaseStyle != null)
            {
                baseStyleName = style.BaseStyle.Name;
                m_writer.WriteStartElement("basedOn", DocxConstants.W_namespace);
                Dictionary<string, string> baseStyleNames = document.StyleNameIds;
                if (styleNames.ContainsValue(baseStyleName))
                {
                    foreach (string key in baseStyleNames.Keys)
                    {
                        if (styleNames[key] == baseStyleName)
                        {
                            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, key);
                            break;
                        }
                    }
                }
                else
                {
                    if (baseStyleName == "Normal Table")
                        baseStyleName = "TableNormal";
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, baseStyleName.Replace(" ", string.Empty));
                }
                m_writer.WriteEndElement();
            }
            //w:next -    Style For Next Paragraph
            if (!string.IsNullOrEmpty(style.NextStyle))
            {
                string nextStyle = style.NextStyle.Replace(" ", string.Empty);
                m_writer.WriteStartElement("next", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, nextStyle);
                m_writer.WriteEndElement();
            }
            //w:link -   Linked Style Reference
            if (!string.IsNullOrEmpty(style.LinkStyle))
            {
                m_writer.WriteStartElement("link", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, style.LinkStyle.Replace(" ", string.Empty));
                m_writer.WriteEndElement();
            }
            //w:autoRedefine [0..1]    Automatically Merge User Formatting Into Style Definition
            //w:hidden [0..1]    Hide Style From User Interface
            //w:uiPriority [0..1]    Optional User Interface Sorting Order
            //w:semiHidden [0..1]    Hide Style From Main User Interface
            if (style.IsSemiHidden)
            {
                m_writer.WriteStartElement("semiHidden", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:unhideWhenUsed [0..1]    Remove Semi-Hidden Property When Style Is Used
            if (style.UnhideWhenUsed)
            {
                m_writer.WriteStartElement("unhideWhenUsed", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:qFormat [0..1]    Primary Style
            if (style.IsPrimaryStyle)
            {
                m_writer.WriteStartElement("qFormat", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //w:locked -   Style Cannot Be Applied
            //w:personal -    E-Mail Message Text Style
            //w:personalCompose -    E-Mail Message Composition Style
            //w:personalReply -    E-Mail Message Reply Style
            //w:rsid -    Revision Identifier for Style Definition
            //w:pPr ,w:rPr ,w:tblPr,w:trPr ,w:tcPr,w:tblStylePr
            if (style is WParagraphStyle)
            {
                WParagraphStyle paraStyle = style as WParagraphStyle;
                // Apply list style to paragraph style
                if (paraStyle.ListIndex >= 0)
                {
                    SerializeNumPr(paraStyle.ListIndex, paraStyle.ListLevel);
                }
                else if (paraStyle.ListFormat.CurrentListStyle != null || paraStyle.ListFormat.IsEmptyList)
                {
                    int listId = 0;
                    int levelNumber = -1;
                    if (!paraStyle.ListFormat.IsEmptyList)
                    {
                        listId = GetListId(paraStyle.ListFormat);
                        int outLevel = (int)paraStyle.ParagraphFormat.OutlineLevel;
                        levelNumber = paraStyle.ListFormat.ListLevelNumber;
                    }

                    SerializeNumPr(listId, levelNumber);
                }
                else if (paraStyle.ListFormat.ListLevelNumber > 0)
                {
                    SerializeNumPr(-1, paraStyle.ListFormat.ListLevelNumber);
                }
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                SerializeParagraphFormat(paraStyle.ParagraphFormat, null);
                m_writer.WriteEndElement();//end of pPr

                SerializeCharactetFormat(style.CharacterFormat);
            }
            else if (style is WTableStyle)
            {
                SerializeTableStyle(style as WTableStyle);
            }
            else
                SerializeCharactetFormat(style.CharacterFormat);



            m_writer.WriteEndElement();//end of style tag
        }
        /// <summary>
        /// Serialize the table style
        /// </summary>
        /// <param name="style">The Style</param>
        private void SerializeTableStyle(WTableStyle style)
        {
            if (!style.ParagraphFormat.IsDefault)
            {
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                // Serialize list format.
                if (style.ListFormat.CurrentListStyle != null || style.ListFormat.IsEmptyList)
                {
                    int listId = 0;
                    int levelNumber = -1;
                    if (!style.ListFormat.IsEmptyList)
                    {
                        listId = GetListId(style.ListFormat);
                        levelNumber = style.ListFormat.ListLevelNumber;
                    }
                    SerializeNumPr(listId, levelNumber);
                }
                else if (style.ListFormat.ListLevelNumber > 0)
                    SerializeNumPr(-1, style.ListFormat.ListLevelNumber);
                SerializeParagraphFormat(style.ParagraphFormat, null);
                m_writer.WriteEndElement();
            }

            if (!style.CharacterFormat.IsDefault)
                SerializeCharactetFormat(style.CharacterFormat);

            if (!style.TableProperties.IsDefault)
                SerializeTableStyleTableProperties(style.TableProperties);

            if (!style.RowProperties.IsDefault)
                SerializeTableStyleRowProperties(style.RowProperties);

            if (!style.CellProperties.IsDefault)
                SerializeTableStyleCellProperties(style.CellProperties);

            foreach (KeyValuePair<ConditionalFormattingCode, ConditionalFormattingStyle> stylePair in style.ConditionalFormattingStyles)
            {
                SerializeConditionalFormattingStyle(stylePair.Key, stylePair.Value);
            }
        }
        /// <summary>
        /// Serialize the table conditional formatting style
        /// </summary>
        /// <param name="code">The Code</param>
        /// <param name="style">The Style</param>
        private void SerializeConditionalFormattingStyle(ConditionalFormattingCode code, ConditionalFormattingStyle style)
        {
            m_writer.WriteStartElement("w", "tblStylePr", DocxConstants.W_namespace);

            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, GetConditionalStyleType(code));

            if (!style.ParagraphFormat.IsDefault)
            {
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                SerializeParagraphFormat(style.ParagraphFormat, null);
                m_writer.WriteEndElement();
            }

            if (!style.CharacterFormat.IsDefault)
                SerializeCharactetFormat(style.CharacterFormat);

            if (!style.TableProperties.IsDefault)
                SerializeTableStyleTableProperties(style.TableProperties);

            if (!style.RowProperties.IsDefault)
                SerializeTableStyleRowProperties(style.RowProperties);

            if (!style.CellProperties.IsDefault)
                SerializeTableStyleCellProperties(style.CellProperties);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Gets conditional formatting style type
        /// </summary>
        /// <param name="code">The Code</param>
        private string GetConditionalStyleType(ConditionalFormattingCode code)
        {
            string styleType = "";
            switch (code)
            {
                case ConditionalFormattingCode.FirstRow:
                    styleType = "firstRow";
                    break;
                case ConditionalFormattingCode.LastRow:
                    styleType = "lastRow";
                    break;
                case ConditionalFormattingCode.OddRowBanding:
                    styleType = "band1Horz";
                    break;
                case ConditionalFormattingCode.EvenRowBanding:
                    styleType = "band2Horz";
                    break;
                case ConditionalFormattingCode.FirstColumn:
                    styleType = "firstCol";
                    break;
                case ConditionalFormattingCode.LastColumn:
                    styleType = "lastCol";
                    break;
                case ConditionalFormattingCode.OddColumnBanding:
                    styleType = "band1Vert";
                    break;
                case ConditionalFormattingCode.EvenColumnBanding:
                    styleType = "band2Vert";
                    break;
                case ConditionalFormattingCode.FirstRowLastCell:
                    styleType = "neCell";
                    break;
                case ConditionalFormattingCode.FirstRowFirstCell:
                    styleType = "nwCell";
                    break;
                case ConditionalFormattingCode.LastRowLastCell:
                    styleType = "seCell";
                    break;
                case ConditionalFormattingCode.LastRowFirstCell:
                    styleType = "swCell";
                    break;
            }
            return styleType;
        }
        /// <summary>
        /// Serialize the table style cell properties
        /// </summary>
        /// <param name="props">The Props</param>
        private void SerializeTableStyleCellProperties(TableStyleCellProperties props)
        {
            m_writer.WriteStartElement("tcPr", DocxConstants.W_namespace);

            //Serialize Table cell borders
            if (!props.Borders.IsDefault && props.HasValue(TableStyleCellProperties.BordersKey))
            {
                m_writer.WriteStartElement("tcBorders", DocxConstants.W_namespace);
                SerializeBorders(props.Borders, 8);
                m_writer.WriteEndElement();
            }
            //Serialize cell shading
            SerializeShading(props);
            //Serialize "noWrap" element
            if (!props.TextWrap)
            {
                m_writer.WriteStartElement("noWrap", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //Serialize cell margins
            if (!props.Paddings.IsDefault && props.HasValue(TableStyleCellProperties.PaddingsKey))
            {
                m_writer.WriteStartElement("tcMar", DocxConstants.W_namespace);
                SerializePaddings(props.Paddings);
                m_writer.WriteEndElement();
            }
            //Serialize cell vertical alignment "vAlign" element
            SerializeCellVerticalAlign(props.VerticalAlignment);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table style row properties
        /// </summary>
        /// <param name="props">The Props</param>
        private void SerializeTableStyleRowProperties(TableStyleRowProperties props)
        {
            m_writer.WriteStartElement("trPr", DocxConstants.W_namespace);
            //Serialize "cantSplit" element
            if (!props.IsBreakAcrossPages)
            {
                m_writer.WriteStartElement("cantSplit", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //Serialize "tblHeader" element
            if (props.IsHeader)
            {
                m_writer.WriteStartElement("tblHeader", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            //Serialize cell spacing
            if (props.HasValue(TableStyleRowProperties.CellSpacingKey))
            {
                m_writer.WriteStartElement("tblCellSpacing", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(props.CellSpacing * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
            //Serialize row justification.
            if (props.HasValue(TableStyleRowProperties.RowAlignmentKey))
            {
                m_writer.WriteStartElement("jc", DocxConstants.W_namespace);

                switch (props.HorizontalAlignment)
                {
                    case RowAlignment.Right:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "right");
                        break;
                    case RowAlignment.Center:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "center");
                        break;
                    default:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "left");
                        break;
                }

                m_writer.WriteEndElement();
            }
            //Serialize "hidden" element
            if (props.IsHidden)
            {
                m_writer.WriteStartElement("hidden", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the table style table properties
        /// </summary>
        /// <param name="props">The Props</param>
        private void SerializeTableStyleTableProperties(TableStyleTableProperties props)
        {
            m_writer.WriteStartElement("tblPr", DocxConstants.W_namespace);
            //Serialize row band size
            if ((props.OwnerBase is WTableStyle) && props.HasValue(TableStyleTableProperties.RowStripeKey))
            {
                m_writer.WriteStartElement("tblStyleRowBandSize", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, props.RowStripe.ToString());
                m_writer.WriteEndElement();
            }
            //Serialize column band size
            if ((props.OwnerBase is WTableStyle) && props.HasValue(TableStyleTableProperties.ColumnStripeKey))
            {
                m_writer.WriteStartElement("tblStyleColBandSize", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, props.ColumnStripe.ToString());
                m_writer.WriteEndElement();
            }
            //Serialize table justification
            if (props.HasValue(TableStyleTableProperties.RowAlignmentKey))
            {
                m_writer.WriteStartElement("jc", DocxConstants.W_namespace);

                switch (props.HorizontalAlignment)
                {
                    case RowAlignment.Right:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "right");
                        break;
                    case RowAlignment.Center:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "center");
                        break;
                    default:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "left");
                        break;
                }

                m_writer.WriteEndElement();
            }
            //Serialize cell spacing
            if (props.HasValue(TableStyleTableProperties.CellSpacingKey))
            {
                m_writer.WriteStartElement("tblCellSpacing", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(props.CellSpacing * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
            //Serialize left indent
            if (props.HasValue(TableStyleTableProperties.LeftIndentKey))
            {
                m_writer.WriteStartElement("tblInd", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", DocxConstants.W_namespace, ToString(props.LeftIndent * DocxConstants.TwentiethOfPoint));
                m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
                m_writer.WriteEndElement();
            }
            //Serialize table borders
            if (!props.Borders.IsDefault && props.HasValue(TableStyleTableProperties.BordersKey))
            {
                m_writer.WriteStartElement("tblBorders", DocxConstants.W_namespace);
                SerializeBorders(props.Borders, 8);
                m_writer.WriteEndElement();
            }
            //Serialize table margins
            if (!props.Paddings.IsDefault && props.HasValue(TableStyleTableProperties.PaddingsKey))
            {
                m_writer.WriteStartElement("tblCellMar", DocxConstants.W_namespace);
                SerializePaddings(props.Paddings);
                m_writer.WriteEndElement();
            }
            //Serialize table shading
            SerializeShading(props);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the shading element in cell properties.
        /// </summary>
        /// <param name="cell">The table cell</param>
        private void SerializeShading(TableStyleCellProperties props)
        {
            if (props.HasValue(TableStyleCellProperties.ShadingColorKey) || props.HasValue(TableStyleCellProperties.ForeColorKey) || props.HasValue(TableStyleCellProperties.TextureStyleKey))
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                string val = GetTextureStyle(props.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);

                if (props.ForeColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "auto");
                }
                else if (props.ForeColor != Color.Empty)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(props.ForeColor));
                }

                if (props.BackColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }
                else if (props.BackColor != Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(props.BackColor));
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize the shading element in table properties.
        /// </summary>
        /// <param name="props">The props</param>
        private void SerializeShading(TableStyleTableProperties props)
        {
            if (props.HasValue(TableStyleTableProperties.ShadingColorKey) || props.HasValue(TableStyleTableProperties.ForeColorKey) || props.HasValue(TableStyleTableProperties.TextureStyleKey))
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                string val = GetTextureStyle(props.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);

                if (props.ForeColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "auto");
                }
                else if (props.ForeColor != Color.Empty)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(props.ForeColor));
                }

                if (props.BackColor == Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }
                else if (props.BackColor != Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(props.BackColor));
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the latent styles
        /// </summary>
        private void SerializeLatentStyles()
        {
#if !SILVERLIGHT && !WP
            if (m_document.LatentStyles != null)
            {
                m_document.LatentStyles.WriteTo(m_writer);
            }
            else if (m_document.LatentStyles2010 != null)
            {
                XmlReader reader = CreateReader(m_document.LatentStyles2010);
                m_writer.WriteNode(reader, false);
            }
#else
            if (m_document.LatentStyles2010 != null)
            {
                XmlReader reader = CreateReader(m_document.LatentStyles2010);
                m_writer.WriteNode(reader, false);
            }
#endif
        }
        /// <summary>
        /// Serializes the default styles (document default paragraph and character format)
        /// </summary>
        private void SerializeDefaultStyles()
        {
            m_writer.WriteStartElement("docDefaults", DocxConstants.W_namespace);


            //if (HasDefaultCharFormat())
            //{
            m_writer.WriteStartElement("rPrDefault", DocxConstants.W_namespace);
            if (m_document.DefCharFormat != null)
                SerializeCharactetFormat(m_document.DefCharFormat);
            else
            {
                m_writer.WriteStartElement("rPr", DocxConstants.W_namespace);
                m_writer.WriteStartElement("rFonts", DocxConstants.W_namespace);

                if (!string.IsNullOrEmpty(m_document.StandardAsciiFont))
                    m_writer.WriteAttributeString("ascii", DocxConstants.W_namespace, m_document.StandardAsciiFont);

                if (!string.IsNullOrEmpty(m_document.StandardFarEastFont))
                    m_writer.WriteAttributeString("eastAsia", DocxConstants.W_namespace, m_document.StandardFarEastFont);

                if (!string.IsNullOrEmpty(m_document.StandardNonFarEastFont))
                    m_writer.WriteAttributeString("hAnsi", DocxConstants.W_namespace, m_document.StandardNonFarEastFont);

                if (!string.IsNullOrEmpty(m_document.StandardBidiFont))
                    m_writer.WriteAttributeString("cs", DocxConstants.W_namespace, m_document.StandardBidiFont);

                m_writer.WriteEndElement();

                float fontSize = GetDefFontSize(m_document, WCharacterFormat.FontSizeKey);
                if (fontSize != 0f)
                {
                    m_writer.WriteStartElement("sz", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, (fontSize * 2).ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                fontSize = GetDefFontSize(m_document, WCharacterFormat.FontSizeBidiKey);
                if (fontSize != 0f)
                {
                    m_writer.WriteStartElement("szCs", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, (fontSize * 2).ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
            //}

            m_writer.WriteStartElement("pPrDefault", DocxConstants.W_namespace);
            if (m_document.m_defParaFormat != null)
            {
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                SerializeParagraphFormat(m_document.m_defParaFormat, null);
                m_writer.WriteEndElement();//end of pPr
            }
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();


            SerializeLatentStyles();

            //Default styles
            if (m_document.Styles.Count == 0 || m_document.Styles.FindByName("Normal") == null)
            {
                SerializeDefaultParagraphStyle();
            }
            if (!IsDocumentContainsDefaultTableStyle())
            {
                SerializeTableNormalStyle();
            }
            if (m_document.Styles.FindByName("No List") == null && m_document.Styles.FindByName("NoList") == null)
                SerializeNoListStyle();
            if (m_document.Styles.FindByName("Table Grid") == null && m_document.Styles.FindByName("TableGrid") == null)
            {
                SerializeTableGridStyle();
            }
        }
        /// <summary>
        /// Serializes the TableGrid style.
        /// </summary>
        private void SerializeTableGridStyle()
        {
            m_writer.WriteStartElement("w", "style", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "table");
            m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, "TableGrid");

            m_writer.WriteStartElement("name", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "Table Grid");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("basedOn", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "TableNormal");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("tblPr", DocxConstants.W_namespace);

            m_writer.WriteStartElement("tblInd", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("tblBorders", DocxConstants.W_namespace);

            m_writer.WriteStartElement("top", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "single");
            m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, "4");
            m_writer.WriteAttributeString("space", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "000000");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("left", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "single");
            m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, "4");
            m_writer.WriteAttributeString("space", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "000000");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("bottom", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "single");
            m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, "4");
            m_writer.WriteAttributeString("space", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "000000");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("right", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "single");
            m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, "4");
            m_writer.WriteAttributeString("space", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "000000");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("insideH", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "single");
            m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, "4");
            m_writer.WriteAttributeString("space", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "000000");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("insideV", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "single");
            m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, "4");
            m_writer.WriteAttributeString("space", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "000000");
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();//end of table borders
            m_writer.WriteEndElement();//end of table properties
            m_writer.WriteEndElement();//end of style "TableGrid"
        }
        /// <summary>
        /// Serializes "NoList" style
        /// </summary>
        private void SerializeNoListStyle()
        {
            m_writer.WriteStartElement("w", "style", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "numbering");
            m_writer.WriteAttributeString("default", DocxConstants.W_namespace, "1");
            m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, "NoList");

            m_writer.WriteStartElement("name", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "No List");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("uiPriority", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "99");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("semiHidden", DocxConstants.W_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("unhideWhenUsed", DocxConstants.W_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();//end of the style "NoList"
        }
        /// <summary>
        /// Serializes the "TableNormal" style
        /// </summary>
        private void SerializeTableNormalStyle()
        {
            m_writer.WriteStartElement("w", "style", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "table");
            m_writer.WriteAttributeString("default", DocxConstants.W_namespace, "1");
            m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, "TableNormal");

            m_writer.WriteStartElement("name", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "Normal Table");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("uiPriority", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "99");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("semiHidden", DocxConstants.W_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("unhideWhenUsed", DocxConstants.W_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("qFormat", DocxConstants.W_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("tblPr", DocxConstants.W_namespace);
            m_writer.WriteStartElement("tblInd", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("tblCellMar", DocxConstants.W_namespace);
            m_writer.WriteStartElement("top", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("left", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "108");
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("bottom", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "0");
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("right", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", DocxConstants.W_namespace, "108");
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "dxa");
            m_writer.WriteEndElement();

            m_writer.WriteEndElement();//end of table cell margins
            m_writer.WriteEndElement();//end of Table properties
            m_writer.WriteEndElement();//end of table style "TableGrid"
        }
        /// <summary>
        /// Serializes the default paragraph style
        /// </summary>
        private void SerializeDefaultParagraphStyle()
        {
            //Write default styles      
            m_writer.WriteStartElement("w", "style", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "paragraph");
            m_writer.WriteAttributeString("default", DocxConstants.W_namespace, "1");
            m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, "Normal");

            m_writer.WriteStartElement("name", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "Normal");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("qFormat", DocxConstants.W_namespace);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();//end of Normal style

            m_writer.WriteStartElement("w", "style", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("type", DocxConstants.W_namespace, "character");
            m_writer.WriteAttributeString("default", DocxConstants.W_namespace, "1");
            m_writer.WriteAttributeString("styleId", DocxConstants.W_namespace, "DefaultParagraphFont");

            m_writer.WriteStartElement("name", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "Default Paragraph Font");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("uiPriority", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "1");
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("semiHidden", DocxConstants.W_namespace);
            m_writer.WriteEndElement();

            m_writer.WriteStartElement("unhideWhenUsed", DocxConstants.W_namespace);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();//end of Default paragraph styles.
        }
        #endregion Styles

        #region Character/paragraph Formatting

        /// <summary>
        /// Serializes the Character format
        /// </summary>
        /// <param name="characterFormat"></param>
        private void SerializeCharactetFormat(WCharacterFormat characterFormat)
        {
            List<Stream> tempDocxProps = new List<Stream>();
            for (int i = 0, cnt = characterFormat.XmlProps.Count; i < cnt; i++)
                tempDocxProps.Add(characterFormat.XmlProps[i]);
            m_writer.WriteStartElement("rPr", DocxConstants.W_namespace);
            //CharacterStyle
            if (characterFormat.CharStyleName != null && characterFormat.CharStyleName.Length != 0)
            {
                string styleName = characterFormat.CharStyleName;
                Dictionary<string, string> styleNames = m_document.StyleNameIds;
                if (styleNames.ContainsValue(characterFormat.CharStyleName))
                {
                    foreach (string key in styleNames.Keys)
                    {
                        if (styleNames[key] == characterFormat.CharStyleName)
                        {
                            styleName = key;
                            break;
                        }
                    }
                }
                m_writer.WriteStartElement("rStyle", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, styleName.Replace(" ", string.Empty));
                m_writer.WriteEndElement();//end of rStyle
            }
            if (HasFont(characterFormat))
            {
                m_writer.WriteStartElement("rFonts", DocxConstants.W_namespace);
                if (characterFormat.HasValue(WCharacterFormat.FontNameAsciiKey))
                    m_writer.WriteAttributeString("ascii", DocxConstants.W_namespace, characterFormat.FontNameAscii);
                if (characterFormat.HasValue(WCharacterFormat.FontNameNonFarEastKey))
                    m_writer.WriteAttributeString("hAnsi", DocxConstants.W_namespace, characterFormat.FontNameNonFarEast);
                if (characterFormat.HasValue(WCharacterFormat.FontNameFarEastKey))
                    m_writer.WriteAttributeString("eastAsia", DocxConstants.W_namespace, characterFormat.FontNameFarEast);
                if (characterFormat.HasValue(WCharacterFormat.FontNameBidiKey))
                    m_writer.WriteAttributeString("cs", DocxConstants.W_namespace, characterFormat.FontNameBidi);
                if(characterFormat.HasValue(WCharacterFormat.IdctHintKey))
                    m_writer.WriteAttributeString("hint", DocxConstants.W_namespace, characterFormat.GetFontHint());
                m_writer.WriteEndElement();//end 
            }
            if (characterFormat.HasValue(WCharacterFormat.BoldKey))
            {
                SerializeBoolProperty(WCharacterFormat.BoldKey, "b", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.BoldBidiKey))
            {
                SerializeBoolProperty(WCharacterFormat.BoldBidiKey, "bCs", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.ItalicKey))
            {
                SerializeBoolProperty(WCharacterFormat.ItalicKey, "i", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.ItalicBidiKey))
            {
                SerializeBoolProperty(WCharacterFormat.ItalicBidiKey, "iCs", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.AllCapsKey))
            {
                SerializeBoolProperty(WCharacterFormat.AllCapsKey, "caps", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.SmallCapsKey))
            {
                SerializeBoolProperty(WCharacterFormat.SmallCapsKey, "smallCaps", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.StrikeKey))
            {
                SerializeBoolProperty(WCharacterFormat.StrikeKey, "strike", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.DoubleStrikeKey))
            {
                SerializeBoolProperty(WCharacterFormat.DoubleStrikeKey, "dstrike", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.OutlineKey))
            {
                SerializeBoolProperty(WCharacterFormat.OutlineKey, "outline", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.ShadowKey))
            {
                SerializeBoolProperty(WCharacterFormat.ShadowKey, "shadow", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.EmbossKey))
            {
                SerializeBoolProperty(WCharacterFormat.EmbossKey, "emboss", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.EngraveKey))
            {
                SerializeBoolProperty(WCharacterFormat.EngraveKey, "imprint", characterFormat);
            }
            if (characterFormat.HasValue(WCharacterFormat.NoProofKey))
            {
                SerializeBoolProperty(WCharacterFormat.NoProofKey, "noProof", characterFormat);
            }
            //snapToGrid
            SerializeDocxProps(tempDocxProps, "snapToGrid");
            SerializeDocxProps(tempDocxProps, "glow");
            SerializeDocxProps(tempDocxProps, "reflection");
            SerializeDocxProps(tempDocxProps, "textOutline");
            if (characterFormat.HasValue(WCharacterFormat.HiddenKey))
            {
                SerializeBoolProperty(WCharacterFormat.HiddenKey, "vanish", characterFormat);
            }
            //webHidden
            if (characterFormat.HasValue(WCharacterFormat.TextColorKey) || characterFormat.HasValue(WCharacterFormat.TextColorExtKey))
            {
                m_writer.WriteStartElement("color", DocxConstants.W_namespace);
                if (characterFormat.TextColor == Color.Empty)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "auto");
                else
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetRGBCode(characterFormat.TextColor));
                m_writer.WriteEndElement();
            }
            if (characterFormat.HasValue(WCharacterFormat.SpacingKey))
            {
                m_writer.WriteStartElement("spacing", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, ToString(characterFormat.CharacterSpacing * DocxConstants.TwentiethOfPoint));
                m_writer.WriteEndElement();
            }
            //w - Text scale
            SerializeDocxProps(tempDocxProps, "W");
            SerializeDocxProps(tempDocxProps, "kern");
            if (characterFormat.HasValue(WCharacterFormat.PositionKey))
            {
                m_writer.WriteStartElement("position", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, ToString(characterFormat.Position * DLSConstants.ShortSize));
                m_writer.WriteEndElement();
            }
            if (characterFormat.HasValue(WCharacterFormat.FontSizeKey))
            {
                m_writer.WriteStartElement("sz", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, ToString(characterFormat.FontSize * 2));
                m_writer.WriteEndElement();
            }
            if (characterFormat.HasValue(WCharacterFormat.FontSizeBidiKey))
            {
                m_writer.WriteStartElement("szCs", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, ToString(characterFormat.FontSizeBidi * 2));
                m_writer.WriteEndElement();
            }
            if (characterFormat.HasValue(WCharacterFormat.HighlightColorKey) && characterFormat.HighlightColor != Color.Empty)
            {
                m_writer.WriteStartElement("highlight", DocxConstants.W_namespace);
                Color color = WordColor.ColorsArray[WordColor.ConvertColorToId(characterFormat.HighlightColor)];
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetHighlightColor(color));
                m_writer.WriteEndElement();
            }
            if (characterFormat.HasValue(WCharacterFormat.UnderlineKey))
            {
                m_writer.WriteStartElement("u", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetUnderlineStyle(characterFormat.UnderlineStyle));
                m_writer.WriteEndElement();
            }
            //effect
            SerializeDocxProps(tempDocxProps, "effect");
            SerializeBorder(characterFormat.Border, "bdr", DocxConstants.BorderMultiplier);
            if (characterFormat.HasValue(WCharacterFormat.TextBkgColorKey) || characterFormat.HasValue(WCharacterFormat.TextBkgColorNewKey) || characterFormat.HasValue(WCharacterFormat.ForeColorKey) || characterFormat.HasValue(WCharacterFormat.ForeColorNewKey))
            {
                SerializeCharacterShading(characterFormat);
            }
            //fitText
            SerializeDocxProps(tempDocxProps, "fitText");
            if (characterFormat.HasValue(WCharacterFormat.SubSuperScriptKey))
            {
                m_writer.WriteStartElement("vertAlign", DocxConstants.W_namespace);
                switch (characterFormat.SubSuperScript)
                {
                    case SubSuperScript.SubScript:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "subscript");
                        break;
                    case SubSuperScript.SuperScript:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "superscript");
                        break;
                    default:
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "baseline");
                        break;
                }
                m_writer.WriteEndElement();
            }
            if (characterFormat.HasValue(WCharacterFormat.BidiKey))
            {
                SerializeBoolProperty(WCharacterFormat.BidiKey, "rtl", characterFormat);
            }
            if (characterFormat.HasKey(WCharacterFormat.ComplexScriptKey))
            {
                SerializeBoolProperty(WCharacterFormat.ComplexScriptKey, "cs", characterFormat);
            }
            //em
            SerializeDocxProps(tempDocxProps, "em");
            if (characterFormat.HasValue(WCharacterFormat.LocaleIdASCIIKey) || characterFormat.HasValue(WCharacterFormat.LocaleIdFarEastKey))
                SerializeLanguage(characterFormat);
            //eastAsianLayout
            SerializeDocxProps(tempDocxProps, "eastAsianLayout");
            //specVanish
            SerializeDocxProps(tempDocxProps, "specVanish");
            //oMath
            SerializeDocxProps(tempDocxProps, "oMath");
            SerializeLigatures(characterFormat);
            SerializeStylisticSet(characterFormat);
            SerializeNumberForm(characterFormat);
            SerializeNumberSpacing(characterFormat);
            SerializeContextualAlternates(characterFormat);
            if (characterFormat.IsChangedFormat)
            {
                SerializeChangeFormat(characterFormat);
            }


            m_writer.WriteEndElement();//end of rPr
        }
        /// <summary>
        /// Serializes the ligatures.
        /// </summary>
        /// <param name="characterFormat">The character format.</param>
        private void SerializeLigatures(WCharacterFormat characterFormat)
        {
            if (characterFormat.HasValue(WCharacterFormat.LigaturesKey))
            {
                m_writer.WriteStartElement("ligatures", DocxConstants.W14_namespace);
                switch (characterFormat.Ligatures)
                {
                    case LigatureType.None:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "none");
                        break;
                    case LigatureType.Standard:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standard");
                        break;
                    case LigatureType.Contextual:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "contextual");
                        break;
                    case LigatureType.StandardContextual:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standardContextual");
                        break;
                    case LigatureType.Historical:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "historical");
                        break;
                    case LigatureType.StandardHistorical:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standardHistorical");
                        break;
                    case LigatureType.ContextualHistorical:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "contextualHistorical");
                        break;
                    case LigatureType.StandardContextualHistorical:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standardContextualHistorical");
                        break;
                    case LigatureType.Discretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "discretional");
                        break;
                    case LigatureType.StandardDiscretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standardDiscretional");
                        break;
                    case LigatureType.ContextualDiscretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "contextualDiscretional");
                        break;
                    case LigatureType.StandardContextualDiscretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standardContextualDiscretional");
                        break;
                    case LigatureType.HistoricalDiscretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "historicalDiscretional");
                        break;
                    case LigatureType.StandardHistoricalDiscretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "standardHistoricalDiscretional");
                        break;
                    case LigatureType.ContextualHistoricalDiscretional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "contextualHistoricalDiscretional");
                        break;
                    case LigatureType.All:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "all");
                        break;
                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the number form.
        /// </summary>
        /// <param name="characterFormat">The character format.</param>
        private void SerializeNumberForm(WCharacterFormat characterFormat)
        {
            if (characterFormat.HasValue(WCharacterFormat.NumberFormKey))
            {
                m_writer.WriteStartElement("numForm", DocxConstants.W14_namespace);
                switch (characterFormat.NumberForm)
                {
                    case NumberFormType.Default:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "default");
                        break;
                    case NumberFormType.Lining:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "lining");
                        break;
                    case NumberFormType.OldStyle:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "oldStyle");
                        break;
                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the number spacing.
        /// </summary>
        /// <param name="characterFormat">The character format.</param>
        private void SerializeNumberSpacing(WCharacterFormat characterFormat)
        {
            if (characterFormat.HasValue(WCharacterFormat.NumberSpacingKey))
            {
                m_writer.WriteStartElement("numSpacing", DocxConstants.W14_namespace);
                switch (characterFormat.NumberSpacing)
                {
                    case NumberSpacingType.Default:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "default");
                        break;
                    case NumberSpacingType.Proportional:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "proportional");
                        break;
                    case NumberSpacingType.Tabular:
                        m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "tabular");
                        break;
                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the stylistic set.
        /// </summary>
        /// <param name="characterFormat">The character format.</param>
        private void SerializeStylisticSet(WCharacterFormat characterFormat)
        {
            if (characterFormat.HasValue(WCharacterFormat.StylisticSetKey))
            {
                m_writer.WriteStartElement("stylisticSets", DocxConstants.W14_namespace);
                m_writer.WriteStartElement("styleSet", DocxConstants.W14_namespace);
                switch (characterFormat.StylisticSet)
                {
                    case StylisticSetType.StylisticSetDefault:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "0");
                        break;
                    case StylisticSetType.StylisticSet01:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "1");
                        break;
                    case StylisticSetType.StylisticSet02:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "2");
                        break;
                    case StylisticSetType.StylisticSet03:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "3");
                        break;
                    case StylisticSetType.StylisticSet04:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "4");
                        break;
                    case StylisticSetType.StylisticSet05:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "5");
                        break;
                    case StylisticSetType.StylisticSet06:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "6");
                        break;
                    case StylisticSetType.StylisticSet07:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "7");
                        break;
                    case StylisticSetType.StylisticSet08:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "8");
                        break;
                    case StylisticSetType.StylisticSet09:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "9");
                        break;
                    case StylisticSetType.StylisticSet10:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "10");
                        break;
                    case StylisticSetType.StylisticSet11:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "11");
                        break;
                    case StylisticSetType.StylisticSet12:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "12");
                        break;
                    case StylisticSetType.StylisticSet13:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "13");
                        break;
                    case StylisticSetType.StylisticSet14:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "14");
                        break;
                    case StylisticSetType.StylisticSet15:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "15");
                        break;
                    case StylisticSetType.StylisticSet16:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "16");
                        break;
                    case StylisticSetType.StylisticSet17:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "17");
                        break;
                    case StylisticSetType.StylisticSet18:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "18");
                        break;
                    case StylisticSetType.StylisticSet19:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "19");
                        break;
                    case StylisticSetType.StylisticSet20:
                        m_writer.WriteAttributeString("w14", "id", DocxConstants.W14_namespace, "20");
                        break;
                }
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the contextual alternates.
        /// </summary>
        /// <param name="characterFormat">The character format.</param>
        private void SerializeContextualAlternates(WCharacterFormat characterFormat)
        {
            if (characterFormat.HasValue(WCharacterFormat.ContextualAlternatesKey))
            {
                m_writer.WriteStartElement("cntxtAlts", DocxConstants.W14_namespace);
                if (!characterFormat.UseContextualAlternates)
                    m_writer.WriteAttributeString("w14", "val", DocxConstants.W14_namespace, "false");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize character shading
        /// </summary>
        private void SerializeCharacterShading(WCharacterFormat characterFormat)
        {
            m_writer.WriteStartElement("shd", DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetTextureStyle(characterFormat.TextureStyle));
            if (characterFormat.ForeColor != Color.Empty)
            {
                m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(characterFormat.ForeColor));
            }
            else
            {
                m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "auto");
            }
            if (characterFormat.TextBackgroundColor != Color.Empty)
            {
                m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(characterFormat.TextBackgroundColor));
            }
            else
            {
                m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the paragraph format
        /// </summary>
        /// <param name="paragraphFormat"></param>
        /// <param name="paragraph"></param>
        private void SerializeParagraphFormat(WParagraphFormat paragraphFormat, WParagraph paragraph)
        {
            //m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
            IWParagraphStyle pStyle = null;
            List<Stream> tempDocxProps = new List<Stream>();
            for (int i = 0, cnt = paragraphFormat.XmlProps.Count; i < cnt; i++)
                tempDocxProps.Add(paragraphFormat.XmlProps[i]);
            if (paragraph != null)
            {
                pStyle = paragraph.GetStyle();
                if (pStyle != null && pStyle.Name != "Normal")
                {
                    m_writer.WriteStartElement("pStyle", DocxConstants.W_namespace);
                    Dictionary<string, string> styleNames = m_document.StyleNameIds;
                    if (styleNames.ContainsValue(pStyle.Name))
                    {
                        foreach (string key in styleNames.Keys)
                        {
                            if (styleNames[key] == pStyle.Name)
                            {
                                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, key);
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, pStyle.Name.Replace(" ", string.Empty));
                    }
                    m_writer.WriteEndElement();
                }
            }
            if (paragraphFormat.HasValue(WParagraphFormat.KeepFollowKey))
            {
                m_writer.WriteStartElement("keepNext", DocxConstants.W_namespace);
                if (!paragraphFormat.KeepFollow)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }
            if (paragraphFormat.HasValue(WParagraphFormat.KeepKey))
            {
                m_writer.WriteStartElement("keepLines", DocxConstants.W_namespace);
                if (!paragraphFormat.Keep)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }
            if (paragraphFormat.HasValue(WParagraphFormat.PageBreakBeforeKey))
            {
                m_writer.WriteStartElement("pageBreakBefore", DocxConstants.W_namespace);
                if (!paragraphFormat.PageBreakBefore)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }
            if (paragraphFormat.IsInFrame())
            {
                SerializeFrame(paragraphFormat);
            }
            if (paragraphFormat.HasValue(WParagraphFormat.WidowControlKey))
            {
                m_writer.WriteStartElement("widowControl", DocxConstants.W_namespace);
                if (paragraphFormat.WidowControl)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "true");
                else
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            if (paragraph != null)
                SerializeListParagraph(paragraph);
            else
                SerializeListStyle(paragraphFormat);
            //suppressLineNumbers
            SerializeDocxProps(tempDocxProps, "suppressLineNumbers");
            SerializeParagraphBorders(paragraphFormat, DocxConstants.BorderMultiplier);
            SerializeParagraphShading(paragraphFormat);
            SerializeTabs(paragraphFormat);
            SerializeSuppressAutoHyphens(paragraphFormat);
            //kinsoku
            SerializeDocxProps(tempDocxProps, "kinsoku");
            //wordWrap
            if (paragraphFormat.HasValue(WParagraphFormat.WordWrapKey))
            {
                m_writer.WriteStartElement("wordWrap", DocxConstants.W_namespace);
                if (paragraphFormat.WordWrap)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "true");
                else
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            //overflowPunct
            SerializeDocxProps(tempDocxProps, "overflowPunct");
            //topLinePunct
            SerializeDocxProps(tempDocxProps, "topLinePunct");
            if (paragraphFormat.HasValue(WParagraphFormat.AutoSpaceDEKey))
            {
                m_writer.WriteStartElement("autoSpaceDE", DocxConstants.W_namespace);
                if (paragraphFormat.AutoSpaceDE)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "true");
                else
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            if (paragraphFormat.HasValue(WParagraphFormat.AutoSpaceDNKey))
            {
                m_writer.WriteStartElement("autoSpaceDN", DocxConstants.W_namespace);
                if (paragraphFormat.AutoSpaceDN)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "true");
                else
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            if (paragraphFormat.HasValue(WParagraphFormat.BidiKey))
            {
                m_writer.WriteStartElement("bidi", DocxConstants.W_namespace);
                if (!paragraphFormat.Bidi)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, paragraphFormat.Bidi.ToString().ToLower());
                m_writer.WriteEndElement();
            }
            if (paragraphFormat.HasValue(WParagraphFormat.AdjustRightIndentKey))
            {
                m_writer.WriteStartElement("adjustRightInd", DocxConstants.W_namespace);
                if (paragraphFormat.AdjustRightIndent)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "true");
                else
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            //snapToGrid
            SerializeDocxProps(tempDocxProps, "snapToGrid");
            SerializeParagraphSpacing(paragraphFormat);
            SerializeIndentation(paragraphFormat);
            if (paragraphFormat.HasValue(WParagraphFormat.ContextualSpacingKey))
            {
                m_writer.WriteStartElement("contextualSpacing", DocxConstants.W_namespace);
                if (!paragraphFormat.ContextualSpacing)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
            // Docx specific property (both Word-2007 and Word-2010)
            SerializeMirrorIndents(paragraphFormat);
            //suppressOverlap
            SerializeDocxProps(tempDocxProps, "suppressOverlap");
            SerializeParagraphAlignment(paragraphFormat, pStyle);
            //textDirection
            SerializeDocxProps(tempDocxProps, "textDirection");
            //textAlignment
            SerializeDocxProps(tempDocxProps, "textAlignment");
            //textboxTightWrap
            SerializeDocxProps(tempDocxProps, "textboxTightWrap");
            if (paragraphFormat.HasValue(WParagraphFormat.OutlineLevelKey))
            {
                if ((byte)paragraphFormat.OutlineLevel >= 0 && (byte)paragraphFormat.OutlineLevel < 9)
                {
                    m_writer.WriteStartElement("outlineLvl", DocxConstants.W_namespace);
                    m_writer.WriteAttributeString("val", DocxConstants.W_namespace, ((byte)paragraphFormat.OutlineLevel).ToString());
                    m_writer.WriteEndElement();
                }
            }
            //divId
            SerializeDocxProps(tempDocxProps, "divId");
            if (paragraph != null && paragraph.IsInCell && IsParagraphContainsCnfStyle)
            {
                IsParagraphContainsCnfStyle = false;
                //Serialize "cnfStyle" element 
                SerializeCnfStyleElement(paragraph);
            }
            if (paragraph != null)
                SerializeCharactetFormat(paragraph.BreakCharacterFormat);

            if (paragraph != null && paragraph.NextSibling == null && paragraph.OwnerTextBody != null &&
                !(paragraph.OwnerTextBody is HeaderFooter))
            {
                WSection sec = paragraph.OwnerTextBody.Owner as WSection;
                if (sec != null && sec.NextSibling != null)
                {
                    string paraText = ModifyText(paragraph.Text);
                    if (sec != null && !paraText.Contains('\r'.ToString()))
                        SerializeSectionProperties(sec);
                }
            }

            if (paragraphFormat.IsChangedFormat)
            {
                SerializeTrackChangeProps("pPrChange");
                m_writer.WriteStartElement("pPr", DocxConstants.W_namespace);
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
            }

        }
        /// <summary>
        /// Serializes the mirror indents.
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format.</param>
        private void SerializeMirrorIndents(WParagraphFormat paragraphFormat)
        {
            if (paragraphFormat.HasValue(WParagraphFormat.MirrorIndentsKey))
            {
                m_writer.WriteStartElement("mirrorIndents", DocxConstants.W_namespace);
                if (!paragraphFormat.MirrorIndents)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the suppress automatic hyphens.
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format.</param>
        private void SerializeSuppressAutoHyphens(WParagraphFormat paragraphFormat)
        {
            if (paragraphFormat.HasValue(WParagraphFormat.SuppressAutoHyphensKey))
            {
                m_writer.WriteStartElement("suppressAutoHyphens", DocxConstants.W_namespace);
                if (!paragraphFormat.SuppressAutoHyphens)
                    m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "false");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Gets the next track change id.
        /// </summary>
        /// <returns></returns>
        private string GetNextTChangeId()
        {
            return (m_trackChangeId++).ToString();
        }
        /// <summary>
        /// Serializes the track change.
        /// </summary>
        /// <param name="item">The item.</param>
        private bool SerializeTrackChange(ParagraphItem item)
        {
            if (item.IsInsertRevision)
            {
                SerializeTrackChangeProps("ins");
                return true;
            }
            else if (item.IsDeleteRevision)
            {
                SerializeTrackChangeProps("del");
                return true;
            }
            return false;
        }
        /// <summary>
        /// Serializes the track changes property
        /// </summary>
        /// <param name="type"></param>
        private void SerializeTrackChangeProps(string type)
        {
            m_writer.WriteStartElement(type, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("id", DocxConstants.W_namespace, GetNextTChangeId());
            string author = (m_document.BuiltinDocumentProperties != null) ? m_document.BuiltinDocumentProperties.Author : string.Empty;
            m_writer.WriteAttributeString("author", DocxConstants.W_namespace, author);
        }
        /// <summary>
        /// Serializes the row change format.
        /// </summary>
        private void SerializeChangeFormat(WCharacterFormat characterFormat)
        {
            SerializeTrackChangeProps("rPrChange");
            m_writer.WriteStartElement("rPr", DocxConstants.W_namespace);
            if (characterFormat.HasValue(WCharacterFormat.LocaleIdASCIIKey) || characterFormat.HasValue(WCharacterFormat.LocaleIdFarEastKey))
                SerializeLanguage(characterFormat);
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Serializes unparsed docx elements to document.
        /// </summary>
        /// <param name="format">The format.</param>
        private void SerializeXmlElements(FormatBase format)
        {
            if (format.HasXmlProps())
            {
                for (int i = 0, cnt = format.XmlProps.Count; i < cnt; i++)
                {
                    XmlReader reader = CreateReader(format.XmlProps[i]);
                    reader.MoveToContent();
                    m_writer.WriteNode(reader, false);
                }
            }
        }
#endif
        /// <summary>
        /// Serializes the paragraph alignment
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format</param>
        /// <param name="pStyle">The paragraph style</param>
        private void SerializeParagraphAlignment(WParagraphFormat paragraphFormat, IWParagraphStyle pStyle)
        {
            if (paragraphFormat.HasValue(WParagraphFormat.HrAlignmentKey))
            {
                m_writer.WriteStartElement("jc", DocxConstants.W_namespace);

                if (!IsBidiPara(pStyle, paragraphFormat))
                {
                    if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Center)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "center");
                    else if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Right)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "right");
                    else if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Justify)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "both");
                    else
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "left");
                }
                else
                {
                    if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Left)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "right");
                    else if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Center)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "center");
                    else if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Right)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "left");
                    else if (paragraphFormat.HorizontalAlignment == HorizontalAlignment.Justify)
                        m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "both");
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the paragraph indentation
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format</param>
        private void SerializeIndentation(WParagraphFormat paragraphFormat)
        {
            if (paragraphFormat.HasValue(WParagraphFormat.LeftIndentKey)
                || paragraphFormat.HasValue(WParagraphFormat.RightIndentKey)
                || paragraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey)
                || paragraphFormat.HasValue(WParagraphFormat.LeftIndentCharsKey)
                || paragraphFormat.HasValue(WParagraphFormat.RightIndentCharsKey)
                || paragraphFormat.HasValue(WParagraphFormat.FirstLineIndentCharsKey))
            {
                m_writer.WriteStartElement("ind", DocxConstants.W_namespace);

                if (paragraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                    m_writer.WriteAttributeString("left", DocxConstants.W_namespace, ToString(paragraphFormat.LeftIndent * DLSConstants.TwipsInOnePoint));
                if (paragraphFormat.HasValue(WParagraphFormat.LeftIndentCharsKey))
                    m_writer.WriteAttributeString("leftChars", DocxConstants.W_namespace, ToString(paragraphFormat.LeftIndentChars * DLSConstants.HundredthsUnit));

                if (paragraphFormat.HasValue(WParagraphFormat.RightIndentKey))
                    m_writer.WriteAttributeString("right", DocxConstants.W_namespace, ToString(paragraphFormat.RightIndent * DLSConstants.TwipsInOnePoint));
                if (paragraphFormat.HasValue(WParagraphFormat.RightIndentCharsKey))
                    m_writer.WriteAttributeString("rightChars", DocxConstants.W_namespace, ToString(paragraphFormat.RightIndentChars * DLSConstants.HundredthsUnit));

                if (paragraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                {
                    if (paragraphFormat.FirstLineIndent < 0)
                        m_writer.WriteAttributeString("hanging", DocxConstants.W_namespace, ToString(-1 * paragraphFormat.FirstLineIndent * DLSConstants.TwipsInOnePoint));
                    else
                        m_writer.WriteAttributeString("firstLine", DocxConstants.W_namespace, ToString(paragraphFormat.FirstLineIndent * DLSConstants.TwipsInOnePoint));
                }
                if (paragraphFormat.HasValue(WParagraphFormat.FirstLineIndentCharsKey))
                {
                    if (paragraphFormat.FirstLineIndentChars < 0)
                        m_writer.WriteAttributeString("hangingChars", DocxConstants.W_namespace, ToString(-1 * paragraphFormat.FirstLineIndentChars * DLSConstants.HundredthsUnit));
                    else
                        m_writer.WriteAttributeString("firstLineChars", DocxConstants.W_namespace, ToString(paragraphFormat.FirstLineIndentChars * DLSConstants.HundredthsUnit));
                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the paragraph spacings
        /// </summary>
        /// <param name="paragraphFormat"></param>
        private void SerializeParagraphSpacing(WParagraphFormat paragraphFormat)
        {
            m_writer.WriteStartElement("spacing", DocxConstants.W_namespace);

            if (paragraphFormat.HasValue(WParagraphFormat.BeforeSpacingKey))
            {
                m_writer.WriteAttributeString("before", DocxConstants.W_namespace, ToString(paragraphFormat.BeforeSpacing * DocxConstants.TwentiethOfPoint));
            }

            if (paragraphFormat.HasValue(WParagraphFormat.SpacingBeforeAutoKey))
            {
                if (paragraphFormat.SpaceBeforeAuto)
                {
                    m_writer.WriteAttributeString("beforeAutospacing", DocxConstants.W_namespace, "1");
                }
                else
                {
                    m_writer.WriteAttributeString("beforeAutospacing", DocxConstants.W_namespace, "0");
                }
            }

            if (paragraphFormat.HasValue(WParagraphFormat.AfterSpacingKey))
            {
                m_writer.WriteAttributeString("after", DocxConstants.W_namespace, ToString(paragraphFormat.AfterSpacing * DocxConstants.TwentiethOfPoint));
            }

            if (paragraphFormat.HasValue(WParagraphFormat.SpacingAfterAutoKey))
            {
                if (paragraphFormat.SpaceAfterAuto)
                {
                    m_writer.WriteAttributeString("afterAutospacing", DocxConstants.W_namespace, "1");
                }
                else
                {
                    m_writer.WriteAttributeString("afterAutospacing", DocxConstants.W_namespace, "0");
                }
            }

            if (paragraphFormat.HasValue(WParagraphFormat.LineSpacingKey))
            {

                m_writer.WriteAttributeString("line", DocxConstants.W_namespace, ToString((float)Math.Abs(paragraphFormat.LineSpacing) * DocxConstants.TwentiethOfPoint));
                switch (paragraphFormat.LineSpacingRule)
                {
                    case LineSpacingRule.AtLeast:
                        m_writer.WriteAttributeString("lineRule", DocxConstants.W_namespace, "atLeast");
                        break;
                    case LineSpacingRule.Exactly:
                        m_writer.WriteAttributeString("lineRule", DocxConstants.W_namespace, "exact");
                        break;
                    default:
                        m_writer.WriteAttributeString("lineRule", DocxConstants.W_namespace, "auto");
                        break;
                }
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the tabs
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format</param>
        private void SerializeTabs(WParagraphFormat paragraphFormat)
        {
            if (paragraphFormat.Tabs.Count > 0)
            {
                m_writer.WriteStartElement("tabs", DocxConstants.W_namespace);

                foreach (Tab tab in paragraphFormat.Tabs)
                {
                    SerializeTab(tab);
                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the tab
        /// </summary>
        /// <param name="tab">The tab</param>
        private void SerializeTab(Tab tab)
        {
            int pos = 0;
            m_writer.WriteStartElement("tab", DocxConstants.W_namespace);
            if (tab.Position == 0 && tab.DeletePosition != 0)
            {
                pos = (int)Math.Round(tab.DeletePosition);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "clear");
            }
            else
            {
                pos = (int)Math.Round(tab.Position * DocxConstants.TwentiethOfPoint);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetTabJustification(tab.Justification));
            }
            if (tab.TabLeader != TabLeader.NoLeader)
                m_writer.WriteAttributeString("leader", DocxConstants.W_namespace, GetTabLeader(tab.TabLeader));
            m_writer.WriteAttributeString("pos", DocxConstants.W_namespace, pos.ToString());
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the paragraph shadings
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format</param>
        private void SerializeParagraphShading(WParagraphFormat paragraphFormat)
        {
            if (paragraphFormat.HasShading())
            {
                m_writer.WriteStartElement("shd", DocxConstants.W_namespace);

                string val = GetTextureStyle(paragraphFormat.TextureStyle);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, val);
                if (paragraphFormat.ForeColor != Color.Empty)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(paragraphFormat.ForeColor));
                }
                else
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "auto");
                }
                if (paragraphFormat.BackColor != Color.Empty)
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, GetRGBCode(paragraphFormat.BackColor));
                }
                else
                {
                    m_writer.WriteAttributeString("fill", DocxConstants.W_namespace, "auto");
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serializes the paragraph borders
        /// </summary>
        /// <param name="paragraphFormat">The Paragraph format</param>
        /// <param name="multiplier"></param>
        private void SerializeParagraphBorders(WParagraphFormat paragraphFormat, int multiplier)
        {
            if (!paragraphFormat.Borders.IsDefault)
            {
                Borders borders = paragraphFormat.Borders;

                m_writer.WriteStartElement("pBdr", DocxConstants.W_namespace);

                if (paragraphFormat.HasValue(WParagraphFormat.TopBorderKey) ||
                  paragraphFormat.HasValue(WParagraphFormat.TopBorderNewKey))
                {
                    SerializeBorder(borders.Top, "top", multiplier);
                }

                if (paragraphFormat.HasValue(WParagraphFormat.LeftBorderKey) ||
                  paragraphFormat.HasValue(WParagraphFormat.LeftBorderNewKey))
                {
                    SerializeBorder(borders.Left, "left", multiplier);
                }

                if (paragraphFormat.HasValue(WParagraphFormat.BottomBorderKey) ||
                  paragraphFormat.HasValue(WParagraphFormat.BottomBorderNewKey))
                {
                    SerializeBorder(borders.Bottom, "bottom", multiplier);
                }

                if (paragraphFormat.HasValue(WParagraphFormat.RightBorderKey) ||
                  paragraphFormat.HasValue(WParagraphFormat.RightBorderNewKey))
                {
                    SerializeBorder(borders.Right, "right", multiplier);
                }

                if (paragraphFormat.HasValue(WParagraphFormat.BetweenBorderKey))
                {
                    SerializeBorder(borders.Horizontal, "between", multiplier);
                }

                if (paragraphFormat.HasValue(WParagraphFormat.BarBorderKey))
                {
                    SerializeBorder(borders.Vertical, "bar", multiplier);
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Seraializes the pargraph list format
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        private void SerializeListParagraph(WParagraph paragraph)
        {
            if (paragraph == null)
                return;

            if (paragraph.ListFormat.ListType != ListType.NoList)
            {
                SerializeListFormat(paragraph.ListFormat);
            }
            else if (paragraph.ListFormat.IsEmptyList)
            {
                SerializeNumPr(0, 0);
            }
        }
        /// <summary>
        /// Serializes the style list format
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        private void SerializeListStyle(WParagraphFormat paragraphFormat)
        {
            if (paragraphFormat.OwnerBase is WParagraphStyle)
            {
                WParagraphStyle paraStyle = (paragraphFormat.OwnerBase as WParagraphStyle);
                if (paraStyle.ListIndex >= 0)
                {
                    SerializeNumPr(paraStyle.ListIndex, paraStyle.ListLevel);
                }
                else if (paraStyle.ListFormat.CurrentListStyle != null || paraStyle.ListFormat.IsEmptyList)
                {
                    int listId = 0;
                    int levelNumber = -1;
                    if (!paraStyle.ListFormat.IsEmptyList)
                    {
                        listId = GetListId(paraStyle.ListFormat);
                        int outLevel = (int)paraStyle.ParagraphFormat.OutlineLevel;
                        levelNumber = paraStyle.ListFormat.ListLevelNumber;
                    }

                    SerializeNumPr(listId, levelNumber);
                }
                else if (paraStyle.ListFormat.ListLevelNumber > 0)
                {
                    SerializeNumPr(-1, paraStyle.ListFormat.ListLevelNumber);
                }
            }
        }
        /// <summary>
        /// Serialize the list format
        /// </summary>
        /// <param name="lf">The list format</param>
        private void SerializeListFormat(WListFormat lf)
        {
            string pStyleName = null;

            if (lf.CurrentListStyle.IsBuiltInStyle && lf.OwnerParagraph != null)
            {
                pStyleName = lf.OwnerParagraph.StyleName;
            }

            int listId = GetListId(lf);

            if (pStyleName != null && !lf.OwnerParagraph.ParagraphFormat.HasListReference)
            {
                WordDocument doc = lf.OwnerParagraph.Document;
                WParagraphStyle style = doc.Styles.FindByName(pStyleName, StyleType.ParagraphStyle) as WParagraphStyle;

                if (style.ListIndex == -1)
                {
                    ListStyle lstStyle = lf.OwnerParagraph.Document.ListStyles.FindByName(lf.CustomStyleName);
                    style.ListIndex = listId;

                    if (lstStyle.Levels.Count > 1)
                    {
                        style.ListLevel = lf.ListLevelNumber;
                    }

                    pStyleName = pStyleName.Replace(" ", "");
                    lstStyle.Levels[lf.ListLevelNumber].ParaStyleName = pStyleName;
                }
            }
            else
            {
                SerializeNumPr(listId, lf.ListLevelNumber);
            }
        }
        /// <summary>
        /// Serializes the paragraph frame.
        /// </summary>
        /// <param name="paragraphFormat"></param>
        private void SerializeFrame(WParagraphFormat paragraphFormat)
        {
            m_writer.WriteStartElement("framePr", DocxConstants.W_namespace);

            if (paragraphFormat.HasValue(WParagraphFormat.FrameWidthKey))
                m_writer.WriteAttributeString("w", "w", DocxConstants.W_namespace, ToString((short)Math.Round(paragraphFormat.FrameWidth * DLSConstants.TwipsInOnePoint)));
            bool isAtleastHeight = true;
            if (paragraphFormat.HasValue(WParagraphFormat.FrameHeightKey)
                && paragraphFormat.FrameHeight != 0)
            {
                // Retrieves the frame height based on the Binary format sprm - sprmPWHeightAbs
                ushort heightValue = (ushort)Math.Round(paragraphFormat.FrameHeight * DLSConstants.TwipsInOnePoint);
                isAtleastHeight = (heightValue & (1 << 15)) != 0;
                float frameHeight = (heightValue & ((1 << 15) - 1));
                m_writer.WriteAttributeString("w", "h", DocxConstants.W_namespace, ToString(frameHeight));
            }
            if (paragraphFormat.HasValue(WParagraphFormat.FrameHorizontalDistanceFromTextKey))
                m_writer.WriteAttributeString("w", "hSpace", DocxConstants.W_namespace, ToString((short)Math.Round(paragraphFormat.FrameHorizontalDistanceFromText * DLSConstants.TwipsInOnePoint)));
            if (paragraphFormat.HasValue(WParagraphFormat.FrameVerticalDistanceFromTextKey))
                m_writer.WriteAttributeString("w", "vSpace", DocxConstants.W_namespace, ToString((short)Math.Round(paragraphFormat.FrameVerticalDistanceFromText * DLSConstants.TwipsInOnePoint)));

            SerializeFrameWrapMode(paragraphFormat.WrapFrameAround);

            if (paragraphFormat.HasValue(WParagraphFormat.FramePosKey))
            {
                String hAnchor = null;

                FrameHorzAnchor frHorAnch = (FrameHorzAnchor)paragraphFormat.FrameHorizontalPos;
                hAnchor = frHorAnch.ToString().ToLower();

                m_writer.WriteAttributeString("w", "hAnchor", DocxConstants.W_namespace, hAnchor);
                FrameVertAnchor frVerAnch = (FrameVertAnchor)paragraphFormat.FrameVerticalPos;
                m_writer.WriteAttributeString("w", "vAnchor", DocxConstants.W_namespace, frVerAnch.ToString().ToLower());
            }
            if (paragraphFormat.HasValue(WParagraphFormat.FrameXKey))
            {
                if (paragraphFormat.IsFrameXAlign((short)paragraphFormat.FrameX))
                {
                    PageNumberAlignment pageNumAl = (PageNumberAlignment)paragraphFormat.FrameX;
                    m_writer.WriteAttributeString("w", "xAlign", DocxConstants.W_namespace, pageNumAl.ToString().ToLower());
                }
                else
                {
                    m_writer.WriteAttributeString("w", "x", DocxConstants.W_namespace, ToString((short)Math.Round(paragraphFormat.FrameX * DLSConstants.TwipsInOnePoint)));
                }
            }
            if (paragraphFormat.HasValue(WParagraphFormat.FrameYKey))
            {
                if (paragraphFormat.IsFrameYAlign((short)paragraphFormat.FrameY))
                {
                    FrameVerticalPosition frameVertPos = (FrameVerticalPosition)paragraphFormat.FrameY;
                    m_writer.WriteAttributeString("w", "yAlign", DocxConstants.W_namespace, frameVertPos.ToString().ToLower());
                }
                else
                    m_writer.WriteAttributeString("w", "y", DocxConstants.W_namespace, ToString((short)Math.Round(paragraphFormat.FrameY * DLSConstants.TwipsInOnePoint)));
            }
            if (!isAtleastHeight)
                m_writer.WriteAttributeString("w", "hRule", DocxConstants.W_namespace, "exact");
            else if (paragraphFormat.HasValue(WParagraphFormat.FrameHeightKey) && paragraphFormat.FrameHeight == 0)
                m_writer.WriteAttributeString("w", "hRule", DocxConstants.W_namespace, "auto");
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Writes the frame's wrapping mode.
        /// </summary>
        /// <param name="relation">The relation.</param>
        private void SerializeFrameWrapMode(FrameWrapMode wrapFrameAround)
        {
            string value = null;
            switch (wrapFrameAround)
            {
                case FrameWrapMode.NotBeside:
                    value = "notBeside";
                    break;
                case FrameWrapMode.Around:
                    value = "around";
                    break;
                case FrameWrapMode.None:
                    value = "none";
                    break;
                case FrameWrapMode.Tight:
                    value = "tight";
                    break;
                case FrameWrapMode.Through:
                    value = "through";
                    break;
                case FrameWrapMode.Auto:
                    value = "auto";
                    break;
            }
            m_writer.WriteAttributeString("w", "wrap", DocxConstants.W_namespace, value);
        }
        /// <summary>
        /// Serializes the Border
        /// </summary>
        /// <param name="border">The Border</param>
        /// <param name="tagName">The tag name</param>
        /// <param name="multiplier"></param>
        private void SerializeBorder(Border border, string tagName, int multiplier)
        {
            BorderStyle borderStyle = border.BorderType;
            int sz = (int)((float)(border.LineWidth * multiplier));
            float space = border.Space;

            if ((borderStyle == BorderStyle.None && !border.HasNoneStyle)
                || (borderStyle == BorderStyle.Cleared && border.HasNoneStyle))
            {
                return;
            }
            else if (borderStyle == BorderStyle.Cleared)
            {
                m_writer.WriteStartElement(tagName, DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, "nil");
                m_writer.WriteEndElement();
                return;
            }

            m_writer.WriteStartElement(tagName, DocxConstants.W_namespace);
            m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, GetBorderStyle(borderStyle));
            if (borderStyle != BorderStyle.Cleared)
            {
                if (border.Color.IsEmpty || border.Color == Color.Black)
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, "auto");
                }
                else
                {
                    m_writer.WriteAttributeString("color", DocxConstants.W_namespace, GetRGBCode(border.Color));
                }
                m_writer.WriteAttributeString("sz", DocxConstants.W_namespace, sz.ToString());
                m_writer.WriteAttributeString("space", DocxConstants.W_namespace, space.ToString(CultureInfo.InvariantCulture));
                if (border.Shadow)
                {
                    m_writer.WriteAttributeString("shadow", DocxConstants.W_namespace, "on");
                }
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the language formats
        /// </summary>
        /// <param name="characterFormat"></param>
        private void SerializeLanguage(WCharacterFormat characterFormat)
        {
            m_writer.WriteStartElement("lang", DocxConstants.W_namespace);
            if (characterFormat.HasValue(WCharacterFormat.LocaleIdASCIIKey))
            {
                string langASCII = ((LocaleIDs)characterFormat.LocaleIdASCII).ToString().Replace('_', '-');
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, langASCII);
            }
            if (characterFormat.HasValue(WCharacterFormat.LocaleIdFarEastKey))
            {
                string langFarEast = ((LocaleIDs)characterFormat.LocaleIdFarEast).ToString().Replace('_', '-');
                m_writer.WriteAttributeString("w", "eastAsia", DocxConstants.W_namespace, langFarEast);
            }
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the bool character format property
        /// </summary>
        /// <param name="key">The key of the property</param>
        /// <param name="tag">Tag name</param>
        /// <param name="characterFormat">The character format</param>
        private void SerializeBoolProperty(short key, string tag, WCharacterFormat characterFormat)
        {
            bool complexProp = false;

            complexProp = characterFormat.GetComplexBoolValue(key);

            if (complexProp)
            {
                m_writer.WriteStartElement(tag, DocxConstants.W_namespace);
                m_writer.WriteEndElement();
            }
            else if (!complexProp && characterFormat.IsComplex(key))
            {
                m_writer.WriteStartElement(tag, DocxConstants.W_namespace);
                m_writer.WriteAttributeString("val", DocxConstants.W_namespace, "0");
                m_writer.WriteEndElement();
            }
        }

        #endregion Character/paragraph Formatting

        #region ContentTypes
        /// <summary>
        /// Serializes the content types [Content-Types].xml
        /// </summary>
        private void SerializeContentTypes()
        {
            MemoryStream contentStream = new MemoryStream();
            m_writer = CreateWriter(contentStream);


            m_writer.WriteStartElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");
            //if (m_hasOleObject)
            //{
            //    //<Default Extension="bin" ContentType="application/vnd.openxmlformats-officedocument.oleObject"/>
            //    SerializeDefaultContentType(contentStream, "bin", "application/vnd.openxmlformats-officedocument.oleObject");
            //}
            SerializeDefaultContentType(contentStream, "rels", DocxConstants.RelationContentType);
            SerializeDefaultContentType(contentStream, "xml", DocxConstants.XmlContentType);

            if (m_hasImages || PictureBullets.Count > 0 || m_hasOleObject || m_hasMetafiles)
            {
                SerializeDefaultContentType(contentStream, "gif", "image/gif");
                SerializeDefaultContentType(contentStream, "jpeg", "image/jpeg");
                SerializeDefaultContentType(contentStream, "wmf", "image/x-wmf");
                SerializeDefaultContentType(contentStream, "png", "image/.png");
            }
            if (m_document.HasMacros
                && IsMacroEnabled)
                SerializeDefaultContentType(contentStream, "bin", DocxConstants.VbaProjectContentType);
            if (m_hasOleObject)
            {
                SerializeOleContentType(contentStream);
            }

            //Serialize Alternate chunk content types
            if (AltChunkContentTypes.Count > 0)
            {
                string contentType = string.Empty;
                foreach (string extension in AltChunkContentTypes.Keys)
                {
                    contentType = AltChunkContentTypes[extension];
                    SerializeDefaultContentType(contentStream, extension, contentType);
                }
            }

            //document.xml
            if (IsMacroEnabled)
            {
                if (m_document.SaveFormatType.ToString().EndsWith("Dotm"))
                    SerializeOverrideContentType(contentStream, DocxConstants.DocumentPath, DocxConstants.MacroTemplateContentType);
                else
                    SerializeOverrideContentType(contentStream, DocxConstants.DocumentPath, DocxConstants.MacroDocumentContentType);
                if (m_document.HasMacros)
                    SerializeOverrideContentType(contentStream, DocxConstants.VbaDataPath, DocxConstants.VbaDataContentType);
            }
            else
            {
                if (m_document.SaveFormatType.ToString().EndsWith("Dotx"))
                    SerializeOverrideContentType(contentStream, DocxConstants.DocumentPath, DocxConstants.TemplateContentType);
                else
                    SerializeOverrideContentType(contentStream, DocxConstants.DocumentPath, DocxConstants.DocumentContentType);
            }

            if (m_document.CustomXMLContainer != null)
            {
                foreach (Part part in m_document.CustomXMLContainer.XmlParts.Values)
                {
                    if (part.Name.StartsWith("itemProps"))
                    {
                        string partPath = DocxConstants.CustomXMLPath + part.Name;
                        string contentType = GetContentType(part.Name);
                        SerializeOverrideContentType(contentStream, partPath, contentType);
                    }
                }
            }

            //<Override PartName="/word/numbering.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml"/>
            if (HasNumbering)
            {
                SerializeOverrideContentType(contentStream, DocxConstants.NumberingPath, DocxConstants.NumberingContentType);
            }
            if (m_hasComment)
            {
                //<Override PartName="/word/comments.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.comments+xml"/>
                SerializeOverrideContentType(contentStream, DocxConstants.CommentsPath, DocxConstants.CommentsContentType);
            }

            if (HasEndnote)
            {
                //<Override PartName="/word/endnotes.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml" />
                SerializeOverrideContentType(contentStream, DocxConstants.EndnotesPath, DocxConstants.EndnoteContentType);
            }
            if (HasFootnote)
            {
                SerializeOverrideContentType(contentStream, DocxConstants.FootnotesPath, DocxConstants.FootnoteContentType);
            }
            if (HasFontTable)
            {
                SerializeOverrideContentType(contentStream, DocxConstants.FontTablePath, DocxConstants.FontTableContentType);
            }
            if (m_hasDiagrams)
            {
                SerializeDiagramType(m_document.DocxPackage, contentStream);
            }
            //Add alternate chunks to zip
            if (AltChunkTargets.Count > 0)
            {
                foreach (string id in AltChunkTargets.Keys)
                {
                    Part chunkPart = m_document.DocxPackage.FindPart(AltChunkTargets[id]);
                    m_archive.AddItem(AltChunkTargets[id], chunkPart.DataStream, false, FileAttributes.Archive);
                }
            }
            //styles.xml
            SerializeOverrideContentType(contentStream, DocxConstants.StylePath, DocxConstants.StylesContentType);

            SerializeOverrideContentType(contentStream, DocxConstants.SettingsPath, DocxConstants.SettingsContentType);
            //core.xml
            SerializeOverrideContentType(contentStream, DocxConstants.CorePath, DocxConstants.CoreContentType);
            //app.xml
            SerializeOverrideContentType(contentStream, DocxConstants.AppPath, DocxConstants.AppContentType);
            //custom.xml
            if (m_document.CustomDocumentProperties != null && m_document.CustomDocumentProperties.Count > 0)
                SerializeOverrideContentType(contentStream, DocxConstants.CustomPath, DocxConstants.CustomContentType);

            SerializeHFContentTypes(contentStream);
            WriteXmlItemsContentTypes(contentStream);

            //End of Types tag
            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.ContentTypesPath, contentStream, false, FileAttributes.Archive);

        }
        /// <summary>
        /// Writes the type of the diagrams.
        /// </summary>
        private void SerializeDiagramType(Package package, MemoryStream stream)
        {
            if (m_hasDiagrams)
            {
                PartContainer diagramCont = package.FindPartContainer(DocxConstants.DiagramPath);
                if (diagramCont != null)
                {
                    foreach (Part part in diagramCont.XmlParts.Values)
                    {
                        string partPath = DocxConstants.DiagramPath + part.Name;
                        string contentType = GetContentType(part.Name);
                        SerializeOverrideContentType(stream, partPath, contentType);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <param name="partName">Name of the part.</param>
        /// <returns></returns>
        private string GetContentType(string partName)
        {
            if (partName.StartsWith("data"))
                return DocxConstants.DiagramData;
            else if (partName.StartsWith("colors"))
                return DocxConstants.DiagramColor;
            else if (partName.StartsWith("quickStyle"))
                return DocxConstants.DiagramColor;
            else if (partName.StartsWith("item"))
                return DocxConstants.CustomXmlContentType;
            else
                return DocxConstants.DiagramLayout;
        }
        private void SerializeOleContentType(MemoryStream contentStream)
        {
            foreach (string type in OleContentTypes)
            {
                switch (type)
                {
                    case "application/vnd.ms-excel":
                        SerializeDefaultContentType(contentStream, "xls", type);
                        break;
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        SerializeDefaultContentType(contentStream, "xlsx", type);
                        break;
                    case "application/vnd.ms-powerpoint":
                        SerializeDefaultContentType(contentStream, "ppt", type);
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                        SerializeDefaultContentType(contentStream, "pptx", type);
                        break;
                    case "application/msword":
                        SerializeDefaultContentType(contentStream, "doc", type);
                        break;
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        SerializeDefaultContentType(contentStream, "docx", type);
                        break;
                    case "application/vnd.ms-word.document.macroEnabled.12":
                        SerializeDefaultContentType(contentStream, "docm", type);
                        break;
                    case "application/vnd.ms-excel.sheet.binary.macroEnabled.12":
                        SerializeDefaultContentType(contentStream, "xlsb", type);
                        break;
                    case "application/vnd.ms-excel.sheet.macroEnabled.12":
                        SerializeDefaultContentType(contentStream, "xlsm", type);
                        break;
                    case "application/vnd.ms-powerpoint.presentation.macroEnabled.12":
                        SerializeDefaultContentType(contentStream, "pptm", type);
                        break;
                    case "application/vnd.ms-powerpoint.slide.macroEnabled.12":
                        SerializeDefaultContentType(contentStream, "sldm", type);
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.slide":
                        SerializeDefaultContentType(contentStream, "sldx", type);
                        break;
                    default:
                        SerializeDefaultContentType(contentStream, "bin", type);
                        break;
                }
            }
        }
        /// <summary>
        /// Serializes the HeaderFooter content types
        /// </summary>
        /// <param name="stream"></param>
        private void SerializeHFContentTypes(MemoryStream stream)
        {
            SerializeHeaderFootersContentType(HeaderFooterType.EvenFooter, stream);
            SerializeHeaderFootersContentType(HeaderFooterType.EvenHeader, stream);
            SerializeHeaderFootersContentType(HeaderFooterType.FirstPageFooter, stream);
            SerializeHeaderFootersContentType(HeaderFooterType.FirstPageHeader, stream);
            SerializeHeaderFootersContentType(HeaderFooterType.OddFooter, stream);
            SerializeHeaderFootersContentType(HeaderFooterType.OddHeader, stream);
        }
        /// <summary>
        /// Serializes the HeaderFooter content types.
        /// </summary>
        /// <param name="hfType">Type of the HeaderFooter.</param>
        /// <param name="stream">The stream.</param>
        private void SerializeHeaderFootersContentType(HeaderFooterType hfType, MemoryStream stream)
        {
            string contentType;
            string partName;

            if (!HeadersFooters.ContainsKey(hfType))
                return;

            Dictionary<string, HeaderFooter> hfColl = HeadersFooters[hfType];
            foreach (string id in hfColl.Keys)
            {
                if (hfType == HeaderFooterType.EvenHeader || hfType == HeaderFooterType.FirstPageHeader ||
                  hfType == HeaderFooterType.OddHeader)
                {
                    partName = DocxConstants.HeaderPath + id.Replace("rId", "") + ".xml";
                    contentType = DocxConstants.HeaderContentType;
                }
                else
                {
                    partName = DocxConstants.FooterPath + id.Replace("rId", "") + ".xml";
                    contentType = DocxConstants.FooterContentType;
                }
                SerializeOverrideContentType(stream, partName, contentType);
            }
        }
        /// <summary>
        /// Serialize the Xml type contents.
        /// </summary>
        /// <param name="stream"></param>
        private void WriteXmlItemsContentTypes(MemoryStream stream)
        {
            if (ChartsPathNames.Count > 0)
            {
                //WriteDefaultType(stream, "xlsx", DocxConstants.c_xlsxContentType);
                if (!(m_hasOleObject && OleContentTypes.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")))
                {
                    SerializeDefaultContentType(stream, "xlsx", DocxConstants.XlsxContentType);
                }
                int nameCnt = ChartsPathNames.Count;
                for (int i = 0; i < nameCnt; i++)
                {
                    string partName = ChartsPathNames[i].Replace(Path.GetExtension(ChartsPathNames[i]), "");
                    partName = partName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                    string contentType = DocxConstants.ChartsContentType;
                    if (partName.EndsWith("colors"))
                        contentType = DocxConstants.ChartColorStyleContentType;
                    else if (partName.EndsWith("style"))
                        contentType = DocxConstants.ChartStyleContentType;
                    else if (partName.EndsWith("drawing"))
                        contentType = DocxConstants.ChartDrawingContentType;
                    SerializeOverrideContentType(stream, ChartsPathNames[i], contentType);
                }
            }

            if (ControlsPathNames.Count > 0)
            {
                if (!m_document.HasMacros)
                    SerializeDefaultContentType(stream, "bin", DocxConstants.ActiveXBinContentType);
                for (int i = 0, cnt = ControlsPathNames.Count; i < cnt; i++)
                {
                    if (ControlsPathNames[i].EndsWith("bin"))
                        SerializeOverrideContentType(stream, ControlsPathNames[i], DocxConstants.ActiveXBinContentType);
                    else
                        SerializeOverrideContentType(stream, ControlsPathNames[i], DocxConstants.ActiveXContentType);
                }
            }
        }
        /// <summary>
        /// Serializes the default content type
        /// </summary>
        /// <param name="stream">Content type stream</param>
        /// <param name="extension">The extenstion</param>
        /// <param name="contentType">The content type</param>
        private void SerializeDefaultContentType(MemoryStream stream, string extension, string contentType)
        {
            m_writer.WriteStartElement("Default");
            m_writer.WriteAttributeString("Extension", extension);
            m_writer.WriteAttributeString("ContentType", contentType);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the Override content type.
        /// </summary>
        /// <param name="stream">The content type stream</param>
        /// <param name="partName">The name of the part</param>
        /// <param name="contentType">Content type</param>
        private void SerializeOverrideContentType(MemoryStream stream, string partName, string contentType)
        {
            m_writer.WriteStartElement("Override");
            m_writer.WriteAttributeString("PartName", "/" + partName.Replace("\\", "/"));
            m_writer.WriteAttributeString("ContentType", contentType);
            m_writer.WriteEndElement();
        }
        #endregion ContentTypes

        #region General Relations
        /// <summary>
        /// Serialize the general relations
        /// </summary>
        private void SerializeGeneralRelations()
        {
            MemoryStream relationStream = new MemoryStream();
            m_writer = CreateWriter(relationStream);
            ResetRelationShipID();

            m_writer.WriteStartElement("Relationships", DocxConstants.RP_namespace);
            SerializeRelationShip(relationStream, GetNextRelationShipID(), DocxConstants.DocumentRelType, DocxConstants.DocumentPath);
            SerializeRelationShip(relationStream, GetNextRelationShipID(), DocxConstants.AppRelType, DocxConstants.AppPath);
            SerializeRelationShip(relationStream, GetNextRelationShipID(), DocxConstants.CoreRelType, DocxConstants.CorePath);
            if (m_document.CustomDocumentProperties != null && m_document.CustomDocumentProperties.Count > 0)
            {
                SerializeRelationShip(relationStream, GetNextRelationShipID(), DocxConstants.CustomRelType, DocxConstants.CustomPath);
            }

            if (m_document.CustomUIPartContainer != null)
                SerializeRelationShip(relationStream, GetNextRelationShipID(), DocxConstants.CustomUIRelType, "customUI/customUI.xml");

            //End of Relationships tag
            m_writer.WriteEndElement();
            m_writer.Flush();

            m_archive.AddItem(DocxConstants.GeneralRelationPath, relationStream, false, FileAttributes.Archive);
        }
        /// <summary>
        /// Serializes the relationship
        /// </summary>
        /// <param name="stream">The memory stream</param>
        /// <param name="relationshipID">The relationship id</param>
        /// <param name="relationshipType">The relationship type</param>
        /// <param name="targetPath">The part of the target item</param>
        private void SerializeRelationShip(Stream stream, string relationshipID, string relationshipType, string targetPath)
        {
            m_writer.WriteStartElement("Relationship");
            m_writer.WriteAttributeString("Id", relationshipID);
            m_writer.WriteAttributeString("Type", relationshipType);

            if (relationshipType == DocxConstants.HyperlinkRelType || targetPath.StartsWith("http://") || targetPath.StartsWith("file:///"))
            {
                Uri targetUri;
                if (Uri.TryCreate(targetPath, UriKind.Absolute, out targetUri))
                {
                    //Handled using Try catch to avoid exception if the Host name type is None because in Silverlight "HostNameType" property is not available.
                    try
                    {
                        m_writer.WriteAttributeString("Target", targetUri.AbsoluteUri);
                    }
                    catch
                    {
                        m_writer.WriteAttributeString("Target", targetPath.Replace("\\", "/").Replace('\v'.ToString(), string.Empty));
                    }
                }
                else
                {
                    m_writer.WriteAttributeString("Target", targetPath.Replace("\\", "/").Replace('\v'.ToString(), string.Empty));
                }
                m_writer.WriteAttributeString("TargetMode", "External");
            }
            else
            {
                m_writer.WriteAttributeString("Target", targetPath.Replace("\\", "/").Replace('\v'.ToString(), string.Empty));
            }

            m_writer.WriteEndElement();
        }
        #endregion General Relations

        #region DocumentProperties
        /// <summary>
        /// Serializes the core properties
        /// </summary>
        private void SerializeCoreProperties()
        {
            MemoryStream coreStream = new MemoryStream();
            m_writer = CreateWriter(coreStream);
            BuiltinDocumentProperties properties = m_document.BuiltinDocumentProperties;


            m_writer.WriteStartElement("cp", "coreProperties", DocxConstants.CP_namespace);
            m_writer.WriteAttributeString("xmlns", "cp", null, DocxConstants.CP_namespace);
            m_writer.WriteAttributeString("xmlns", "dc", null, DocxConstants.DC_namespace);
            m_writer.WriteAttributeString("xmlns", "dcterms", null, DocxConstants.DCTERMS_namespace);
            m_writer.WriteAttributeString("xmlns", "dcmitype", null, DocxConstants.DCMI_namespace);
            m_writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");

            if (properties != null)
            {
                if (properties.Title != null)
                {
                    m_writer.WriteStartElement("dc", "title", DocxConstants.DC_namespace);
                    m_writer.WriteString(properties.Title);
                    m_writer.WriteEndElement();
                }
                if (properties.Subject != null)
                {
                    m_writer.WriteStartElement("dc", "subject", DocxConstants.DC_namespace);
                    m_writer.WriteString(properties.Subject);
                    m_writer.WriteEndElement();
                }

                if (properties.Author != null)
                {
                    m_writer.WriteStartElement("dc", "creator", DocxConstants.DC_namespace);
                    m_writer.WriteString(properties.Author);
                    m_writer.WriteEndElement();
                }

                if (properties.Keywords != null)
                {
                    m_writer.WriteStartElement("keywords", DocxConstants.CP_namespace);
                    m_writer.WriteString(properties.Keywords);
                    m_writer.WriteEndElement();
                }

                if (properties.Comments != null)
                {
                    m_writer.WriteStartElement("dc", "description", DocxConstants.DC_namespace);
                    m_writer.WriteString(properties.Comments);
                    m_writer.WriteEndElement();
                }

                if (properties.LastAuthor != null)
                {
                    m_writer.WriteStartElement("lastModifiedBy", DocxConstants.CP_namespace);
                    m_writer.WriteString(properties.LastAuthor);
                    m_writer.WriteEndElement();
                }
                if (properties.LastPrinted != null && properties.LastPrinted != DateTime.MinValue)
                {
                    m_writer.WriteStartElement("lastPrinted", DocxConstants.CP_namespace);
#if !WINRT
                    string value = XmlConvert.ToString(properties.LastPrinted, XmlDateTimeSerializationMode.Utc);
#else
                    string value = XmlConvert.ToString(properties.LastPrinted.ToUniversalTime());//, XmlDateTimeSerializationMode.Utc);
#endif
                    m_writer.WriteString(value);
                    m_writer.WriteEndElement();
                }
                if (properties.RevisionNumber != null)
                {
                    m_writer.WriteStartElement("revision", DocxConstants.CP_namespace);

                    int revisionNum = 1;

                    if (properties.RevisionNumber != "NaN")
                    {
                        try
                        {
                            revisionNum = int.Parse(properties.RevisionNumber);
                        }
                        catch
                        {
                        }
                    }

                    m_writer.WriteString(revisionNum.ToString());
                    m_writer.WriteEndElement();
                }

                string time = null;

                if (properties.CreateDate != DateTime.Now)
                {
                    m_writer.WriteStartElement("dcterms", "created", DocxConstants.DCTERMS_namespace);
                    m_writer.WriteAttributeString("xsi", "type", DocxConstants.XSI_namespace, "dcterms:W3CDTF");

                    time = null;
#if !WINRT
                    time = XmlConvert.ToString(properties.CreateDate, XmlDateTimeSerializationMode.Utc);
#else
                    time= XmlConvert.ToString(properties.CreateDate.ToUniversalTime());
#endif
                    m_writer.WriteString(time);
                    m_writer.WriteEndElement();
                }
                if (properties.LastSaveDate != DateTime.Now)
                {
                    m_writer.WriteStartElement("dcterms", "modified", DocxConstants.DCTERMS_namespace);
                    m_writer.WriteAttributeString("xsi", "type", DocxConstants.XSI_namespace, "dcterms:W3CDTF");
#if !WINRT
                    time = XmlConvert.ToString(properties.LastSaveDate, XmlDateTimeSerializationMode.Utc);
#else
                    time = XmlConvert.ToString(properties.LastSaveDate.ToUniversalTime());//, XmlDateTimeSerializationMode.Utc);
#endif
                    m_writer.WriteString(time);
                    m_writer.WriteEndElement();
                }
                if (properties.Category != null)
                {
                    m_writer.WriteStartElement("category", DocxConstants.CP_namespace);
                    m_writer.WriteString(properties.Category);
                    m_writer.WriteEndElement();
                }
            }

            m_writer.WriteEndElement();
            m_writer.Flush();

            //Add the stream into the archive (core.xml)
            m_archive.AddItem(DocxConstants.CorePath, coreStream, false, FileAttributes.Archive);

        }
        /// <summary>
        /// Serializes the custom properties.
        /// </summary>
        private void SerializeCustomProperties()
        {
            MemoryStream customStream = new MemoryStream();
            m_writer = CreateWriter(customStream);
            CustomDocumentProperties properties = m_document.CustomDocumentProperties;

            m_writer.WriteStartElement("Properties", DocxConstants.CustomProps_namespace);
            m_writer.WriteAttributeString("xmlns", "vt", null, DocxConstants.VT_namespace);

            int index = 1;
            string entryKey = string.Empty;

            if (properties != null)
            {
                foreach (string key in properties.CustomHash.Keys)
                {
                    entryKey = key;
                    if (entryKey == "_PID_LINKBASE" || entryKey == "_PID_HLINKS")
                    {
                        continue;
                    }

                    m_writer.WriteStartElement("property");
                    m_writer.WriteAttributeString("fmtid", "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}");

                    string name = string.Empty;
                    name = key;
                    m_writer.WriteAttributeString("name", name);
                    m_writer.WriteAttributeString("pid", (++index).ToString());
                    DocumentProperty property = properties.CustomHash[key];

                    switch (property.PropertyType)
                    {
                        case Syncfusion.CompoundFile.DocIO.PropertyType.String:
                            m_writer.WriteStartElement("lpwstr", DocxConstants.VT_namespace);
                            m_writer.WriteString(UpdateText(property.Text));
                            m_writer.WriteEndElement();
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.AsciiString:
                            m_writer.WriteStartElement("lpstr", DocxConstants.VT_namespace);
                            m_writer.WriteString(UpdateText(property.Text));
                            m_writer.WriteEndElement();
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Int:
                            m_writer.WriteStartElement("i4", DocxConstants.VT_namespace);
                            m_writer.WriteString(XmlConvert.ToString(property.Integer));
                            m_writer.WriteEndElement();
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Int32:
                            m_writer.WriteStartElement("i4", DocxConstants.VT_namespace);
                            m_writer.WriteString(XmlConvert.ToString(property.Int32));
                            m_writer.WriteEndElement();
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Double:
                            m_writer.WriteStartElement("r8", DocxConstants.VT_namespace);
                            m_writer.WriteString(XmlConvert.ToString(property.Double));
                            m_writer.WriteEndElement();
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.DateTime:
                            m_writer.WriteStartElement("filetime", DocxConstants.VT_namespace);
                            string time = string.Empty;
#if !WINRT
                            time = XmlConvert.ToString(property.ToDateTime(), XmlDateTimeSerializationMode.Utc);
#else
                            time = XmlConvert.ToString(property.ToDateTime().ToUniversalTime());//.ToUniversalTime());
#endif
                            m_writer.WriteString(time);
                            m_writer.WriteEndElement();
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Bool:
                            m_writer.WriteStartElement("bool", DocxConstants.VT_namespace);
                            m_writer.WriteString(XmlConvert.ToString(property.Boolean));
                            m_writer.WriteEndElement();
                            break;
                    }

                    m_writer.WriteEndElement();
                }
            }

            //End of Properties tag
            m_writer.WriteEndElement();
            m_writer.Flush();

            //Add the stream into the archive (custom.xml)
            m_archive.AddItem(DocxConstants.CustomPath, customStream, true, FileAttributes.Archive);
        }
        /// <summary>
        /// Updates the text, that cannot be represented in Xml as defined by XML 1.0 specification.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string UpdateText(string text)
        {
            //Converts the text to valid Xml text. (_xHHHH_ escape character format).
            text = XmlConvert.EncodeName(text);
            //Replace xml character to white space.
            text = text.Replace("_x0020_", " ");
            return text;
        }
        /// <summary>
        /// Serializes the app properties (app.xml)
        /// </summary>
        private void SerializeAppProperties()
        {
            MemoryStream appStream = new MemoryStream();
            BuiltinDocumentProperties properties = m_document.BuiltinDocumentProperties;
            m_writer = CreateWriter(appStream);

            m_writer.WriteStartElement("Properties", DocxConstants.docProps_namespace);
            m_writer.WriteAttributeString("xmlns", "vt", null, DocxConstants.VT_namespace);

            if (properties != null)
            {
                if (properties.Template != null)
                {
                    m_writer.WriteStartElement("Template");
                    m_writer.WriteString(properties.Template);
                    m_writer.WriteEndElement();
                }
                if (properties.TotalEditingTime != TimeSpan.MinValue)
                {
                    m_writer.WriteStartElement("TotalTime");
                    m_writer.WriteString(properties.TotalEditingTime.TotalMinutes.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                if (properties.PageCount != int.MinValue)
                {
                    m_writer.WriteStartElement("Pages");
                    m_writer.WriteString(properties.PageCount.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }
                if (properties.WordCount != int.MinValue)
                {
                    m_writer.WriteStartElement("Words");
                    m_writer.WriteString(properties.WordCount.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                if (properties.CharCount != int.MinValue)
                {
                    m_writer.WriteStartElement("Characters");
                    m_writer.WriteString(properties.CharCount.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                if (properties.ApplicationName != null)
                {
                    m_writer.WriteStartElement("Application");
                    m_writer.WriteString(properties.ApplicationName);
                    m_writer.WriteEndElement();
                }

                if (properties.DocSecurity != int.MinValue)
                {
                    m_writer.WriteStartElement("DocSecurity");
                    m_writer.WriteString(properties.DocSecurity.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                if (properties.LinesCount != int.MinValue)
                {
                    m_writer.WriteStartElement("Lines");
                    m_writer.WriteString(properties.LinesCount.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                if (properties.ParagraphCount != int.MinValue)
                {
                    m_writer.WriteStartElement("Paragraphs");
                    m_writer.WriteString(properties.ParagraphCount.ToString(CultureInfo.InvariantCulture));
                    m_writer.WriteEndElement();
                }

                if (properties.Manager != null)
                {
                    m_writer.WriteStartElement("Manager");
                    m_writer.WriteString(properties.Manager);
                    m_writer.WriteEndElement();
                }

                if (properties.Company != null)
                {
                    m_writer.WriteStartElement("Company");
                    m_writer.WriteString(properties.Company);
                    m_writer.WriteEndElement();
                }
                m_writer.WriteStartElement("AppVersion");
                if (IsWord2007)
                    m_writer.WriteString("12.0000");
                else if (IsWord2010)
                    m_writer.WriteString("14.0000");
                else
                    m_writer.WriteString("15.0000");
                m_writer.WriteEndElement();
            }
            //end element for Properties tag
            m_writer.WriteEndElement();
            m_writer.Flush();

            //Add the stream into the archive (app.xml)
            m_archive.AddItem(DocxConstants.AppPath, appStream, true, FileAttributes.Archive);

        }
        #endregion DocumentProperties

        #region Helper methods
        /// <summary>
        /// Checks whether the style is 
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private bool IsDefaultStyle(Style style)
        {
            string styleName = style.Name;
            if ((styleName == "Normal" && style.StyleType == StyleType.ParagraphStyle)
                || (styleName == "Default Paragraph Font" && style.StyleType == StyleType.CharacterStyle)
                || (styleName == "Table Normal" || styleName == "Normal Table") && style.StyleType == StyleType.TableStyle
                || (styleName == "No List" && style.TypeCode == WordStyleType.ListStyle))
            {
                return true;
            }

            if (style.StyleId == 0)
                return true;

            return false;
        }
        /// <summary>
        /// Get the tab leader type as string
        /// </summary>
        /// <param name="tabLeader">The tab leader</param>
        /// <returns>returns the tab leader type as string</returns>
        private string GetTabLeader(TabLeader tabLeader)
        {
            switch (tabLeader)
            {
                case TabLeader.Dotted:
                    return "dot";
                case TabLeader.Hyphenated:
                    return "hyphen";
                case TabLeader.Single:
                    return "underscore";
                case TabLeader.Heavy:
                    return "heavy";
                default:
                    return "none";

            }
        }
        /// <summary>
        /// Gets the tab justication type as string
        /// </summary>
        /// <param name="tabJustification">The tab justification</param>
        /// <returns>returns the tab justication type as string</returns>
        private string GetTabJustification(TabJustification tabJustification)
        {
            switch (tabJustification)
            {
                case TabJustification.Left:
                    return "left";
                case TabJustification.Centered:
                    return "center";
                case TabJustification.Right:
                    return "right";
                case TabJustification.Decimal:
                    return "decimal";
                case TabJustification.Bar:
                    return "bar";
                case TabJustification.List:
                    return "num";
                default:
                    return "clear";
            }
        }
        /// <summary>
        /// Get the list ID
        /// </summary>
        /// <param name="lf">The list format</param>
        /// <returns>returns the list id</returns>
        private int GetListId(WListFormat lf)
        {
            int listId = 0;

            if (lf.LFOStyleName == string.Empty || lf.CurrentListStyle.Name != string.Empty)
            {
                foreach (ListStyle listStyle in lf.Document.ListStyles)
                {
                    if (listStyle.Name == lf.CustomStyleName)
                    {
                        break;
                    }
                    else
                    {
                        listId += 1;
                    }
                }
            }

            //Implementation of list overrides
            if (lf.LFOStyleName != null)
            {
                if (m_lstOverId == 0)
                {
                    //Init start id for list overrides
                    m_lstOverId = lf.Document.ListStyles.Count + 1;
                }

                if (!ListStyleReferences.ContainsKey(listId))
                {
                    listId = AddListOverride(listId, lf.LFOStyleName);
                }
                else
                {
                    Dictionary<int, string> overrideStyleNames = ListStyleReferences[listId];
                    bool findStyle = false;
                    foreach (int key in overrideStyleNames.Keys)
                    {
                        if (overrideStyleNames[key] == lf.LFOStyleName)
                        {
                            listId = key;
                            findStyle = true;
                            break;
                        }
                    }

                    if (!findStyle)
                    {
                        listId = AddListOverride(listId, lf.LFOStyleName);
                    }
                }
            }
            else
            {
                listId += 1;
            }

            return listId;
        }
        /// <summary>
        /// Add the list override to ListStyleReferences collection
        /// </summary>
        /// <param name="listId">The list id</param>
        /// <param name="lfoStyleName">The LFO Style name</param>
        /// <returns>returns the list id from the list style reference collection</returns>
        private int AddListOverride(int listId, string lfoStyleName)
        {
            if (!ListStyleReferences.ContainsKey(listId))
            {
                ListStyleReferences.Add(listId, new Dictionary<int, string>());
            }
            ListStyleReferences[listId].Add(m_lstOverId, lfoStyleName);
            int retListId = m_lstOverId;
            m_lstOverId += 1;

            return retListId;
        }
        /// <summary>
        /// Serializes the numbering properties to the paragraph
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="listLevel"></param>
        private void SerializeNumPr(int listId, int listLevel)
        {
            m_writer.WriteStartElement("numPr", DocxConstants.W_namespace);

            if (listLevel != -1)
            {
                m_writer.WriteStartElement("ilvl", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, listLevel.ToString());
                m_writer.WriteEndElement();
            }

            if (listId != -1)
            {
                m_writer.WriteStartElement("numId", DocxConstants.W_namespace);
                m_writer.WriteAttributeString("w", "val", DocxConstants.W_namespace, listId.ToString());
                m_writer.WriteEndElement();
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Check whether the character format has font property
        /// </summary>
        /// <param name="characterFormat"></param>
        /// <returns></returns>
        private bool HasFont(WCharacterFormat characterFormat)
        {
            if (characterFormat.HasValue(WCharacterFormat.FontNameKey)
                || characterFormat.HasValue(WCharacterFormat.FontNameFarEastKey)
                || characterFormat.HasValue(WCharacterFormat.FontNameNonFarEastKey)
                || characterFormat.HasValue(WCharacterFormat.FontNameBidiKey)
                || characterFormat.HasValue(WCharacterFormat.IdctHintKey))
                return true;

            if (characterFormat.Document != null && characterFormat.Document.GrammarSpellingData == null &&
              characterFormat.HasKey(WCharacterFormat.FontKey))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Create xml writer
        /// </summary>
        /// <param name="data">The stream</param>
        /// <returns>returns the xml writer</returns>
        private XmlWriter CreateWriter(Stream data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(data, settings);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"");
            return writer;
        }
        /// <summary>
        /// Create xml reader
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>returns xml reader</returns>
        private XmlReader CreateReader(Stream stream)
        {
            stream.Position = 0;
            XmlReader reader = XmlReader.Create(stream);
            reader.Read();

            while (reader.NodeType == XmlNodeType.XmlDeclaration)
                reader.Read();

            return reader;
        }
        /// <summary>
        /// Reset the relationship id counter
        /// </summary>
        private void ResetRelationShipID()
        {
            m_relationShipID = 0;
        }
        /// <summary>
        /// Get the next relationship ID
        /// </summary>
        /// <returns>returns the next relationship ID</returns>
        private string GetNextRelationShipID()
        {
            return string.Format("rId{0}", ++m_relationShipID);
        }
        /// <summary>
        /// Get the next ID
        /// </summary>
        /// <returns></returns>
        private int GetNextID()
        {
            return ++m_id;
        }
        /// <summary>
        /// Get the TextureStyle as string
        /// </summary>
        /// <param name="texttureStyle"></param>
        /// <returns></returns>
        private string GetTextureStyle(TextureStyle texttureStyle)
        {
            switch (texttureStyle)
            {
                case TextureStyle.Texture5Percent:
                case TextureStyle.Texture2Pt5Percent:
                case TextureStyle.Texture7Pt5Percent:
                    return "pct5";
                case TextureStyle.Texture10Percent:
                    return "pct10";
                case TextureStyle.Texture12Pt5Percent:
                    return "pct12";
                case TextureStyle.Texture15Percent:
                    return "pct15";
                case TextureStyle.Texture17Pt5Percent:
                    return "pct15";
                case TextureStyle.Texture20Percent:
                    return "pct20";
                case TextureStyle.Texture25Percent:
                case TextureStyle.Texture27Pt5Percent:
                    return "pct25";
                case TextureStyle.Texture30Percent:
                case TextureStyle.Texture32Pt5Percent:
                    return "pct30";
                case TextureStyle.Texture35Percent:
                    return "pct35";
                case TextureStyle.Texture37Pt5Percent:
                    return "pct37";
                case TextureStyle.Texture40Percent:
                case TextureStyle.Texture42Pt5Percent:
                    return "pct40";
                case TextureStyle.Texture45Percent:
                case TextureStyle.Texture47Pt5Percent:
                    return "pct45";
                case TextureStyle.Texture50Percent:
                case TextureStyle.Texture52Pt5Percent:
                    return "pct50";
                case TextureStyle.Texture55Percent:
                case TextureStyle.Texture57Pt5Percent:
                    return "pct55";
                case TextureStyle.Texture60Percent:
                    return "pct60";
                case TextureStyle.Texture62Pt5Percent:
                    return "pct62";
                case TextureStyle.Texture65Percent:
                case TextureStyle.Texture67Pt5Percent:
                    return "pct65";
                case TextureStyle.Texture70Percent:
                case TextureStyle.Texture72Pt5Percent:
                    return "pct70";
                case TextureStyle.Texture75Percent:
                case TextureStyle.Texture77Pt5Percent:
                    return "pct75";
                case TextureStyle.Texture80Percent:
                case TextureStyle.Texture82Pt5Percent:
                    return "pct80";
                case TextureStyle.Texture85Percent:
                    return "pct85";
                case TextureStyle.Texture87Pt5Percent:
                    return "pct87";
                case TextureStyle.Texture90Percent:
                case TextureStyle.Texture92Pt5Percent:
                    return "pct90";
                case TextureStyle.Texture95Percent:
                case TextureStyle.Texture97Pt5Percent:
                    return "pct95";
                case TextureStyle.TextureCross:
                    return "thinHorzCross";
                case TextureStyle.TextureDarkCross:
                    return "horzCross";
                case TextureStyle.TextureDarkDiagonalCross:
                    return "diagCross";
                case TextureStyle.TextureDarkDiagonalDown:
                    return "reverseDiagStripe";
                case TextureStyle.TextureDarkDiagonalUp:
                    return "diagStripe";
                case TextureStyle.TextureDarkHorizontal:
                    return "horzStripe";
                case TextureStyle.TextureDarkVertical:
                    return "vertStripe";
                case TextureStyle.TextureDiagonalCross:
                    return "thinDiagCross";
                case TextureStyle.TextureDiagonalDown:
                    return "thinReverseDiagStripe";
                case TextureStyle.TextureDiagonalUp:
                    return "thinDiagStripe";
                case TextureStyle.TextureHorizontal:
                    return "thinHorzStripe";
                case TextureStyle.TextureSolid:
                    return "solid";
                case TextureStyle.TextureVertical:
                    return "thinVertStripe";
                default:
                    return "clear";
            }
        }
        /// <summary>
        /// Get the border style as string
        /// </summary>
        /// <param name="borderStyle"></param>
        /// <returns></returns>
        private string GetBorderStyle(BorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case BorderStyle.TwistedLines1:
                    return "twistedLines1";
                case BorderStyle.Triple:
                    return "triple";
                case BorderStyle.DashSmallGap:
                    return "dashSmallGap";
                case BorderStyle.Single:
                case BorderStyle.Hairline:
                    return "single";
                case BorderStyle.Dot:
                    return "dotted";
                case BorderStyle.DotDash:
                    return "dotDash";
                case BorderStyle.DashLargeGap:
                    return "dashed";
                case BorderStyle.DotDotDash:
                    return "dotDotDash";
                case BorderStyle.Double:
                    return "double";
                case BorderStyle.ThinThinSmallGap:
                    return "thickThinSmallGap";
                case BorderStyle.ThinThickSmallGap:
                    return "thinThickSmallGap";
                case BorderStyle.ThinThickThinSmallGap:
                    return "thinThickThinSmallGap";
                case BorderStyle.ThickThinMediumGap:
                    return "thickThinMediumGap";
                case BorderStyle.ThinThickMediumGap:
                    return "thinThickMediumGap";
                case BorderStyle.ThickThickThinMediumGap:
                    return "thinThickThinMediumGap";
                case BorderStyle.ThickThinLargeGap:
                    return "thickThinLargeGap";
                case BorderStyle.ThinThickLargeGap:
                    return "thinThickLargeGap";
                case BorderStyle.ThinThickThinLargeGap:
                    return "thinThickThinLargeGap";
                case BorderStyle.Thick:
                    return "thick";
                case BorderStyle.Wave:
                    return "wave";
                case BorderStyle.DoubleWave:
                    return "doubleWave";
                case BorderStyle.DashDotStroker:
                    return "dashDotStroked";
                case BorderStyle.Engrave3D:
                    return "threeDEngrave";
                case BorderStyle.Emboss3D:
                    return "threeDEmboss";
                case BorderStyle.Outset:
                    return "outset";
                case BorderStyle.Inset:
                    return "inset";
                case BorderStyle.Cleared:
                    return "nil";
                case BorderStyle.None:
                    return "none";
                default:
                    return "single";
            }
        }
        /// <summary>
        /// Get the underline style as string
        /// </summary>
        /// <param name="underlineStyle"></param>
        /// <returns></returns>
        private string GetUnderlineStyle(UnderlineStyle underlineStyle)
        {
            switch (underlineStyle)
            {
                case UnderlineStyle.Dash:
                    return "dash";
                case UnderlineStyle.DotDotDashHeavy:
                    return "dashDotDotHeavy";
                case UnderlineStyle.DotDashHeavy:
                    return "dashDotHeavy";
                case UnderlineStyle.DashHeavy:
                    return "dashedHeavy";
                case UnderlineStyle.DashLong:
                    return "dashLong";
                case UnderlineStyle.DashLongHeavy:
                    return "dashLongHeavy";
                case UnderlineStyle.DotDash:
                    return "dotDash";
                case UnderlineStyle.DotDotDash:
                    return "dotDotDash";
                case UnderlineStyle.Dotted:
                    return "dotted";
                case UnderlineStyle.DottedHeavy:
                    return "dottedHeavy";
                case UnderlineStyle.Double:
                    return "double";
                case UnderlineStyle.Single:
                    return "single";
                case UnderlineStyle.Thick:
                    return "thick";
                case UnderlineStyle.Wavy:
                    return "wave";
                case UnderlineStyle.WavyDouble:
                    return "wavyDouble";
                case UnderlineStyle.WavyHeavy:
                    return "wavyHeavy";
                case UnderlineStyle.Words:
                    return "words";
                default:
                    return "none";
            }
        }
        /// <summary>
        /// Get the highlight color as string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private string GetHighlightColor(Color color)
        {
            if (color.ToArgb() == Color.Black.ToArgb())
                return "black";
            else if (color.ToArgb() == Color.Blue.ToArgb())
                return "blue";
            else if (color.ToArgb() == Color.Cyan.ToArgb())
                return "cyan";
            else if (color.ToArgb() == Color.DarkBlue.ToArgb())
                return "darkBlue";
            else if (color.ToArgb() == Color.DarkCyan.ToArgb())
                return "darkCyan";
            else if (color.ToArgb() == Color.DarkGray.ToArgb() || color == Color.FromArgb(0, 128, 128, 128))
                return "darkGray";
            else if (color.ToArgb() == Color.DarkGreen.ToArgb())
                return "darkGreen";
            else if (color.ToArgb() == Color.DarkMagenta.ToArgb())
                return "darkMagenta";
            else if (color.ToArgb() == Color.DarkRed.ToArgb())
                return "darkRed";
            else if (color.ToArgb() == Color.Green.ToArgb())
                return "green";
            else if (color.ToArgb() == Color.Gold.ToArgb())
                return "darkYellow";
            else if (color.ToArgb() == Color.LightGray.ToArgb())
                return "lightGray";
            else if (color.ToArgb() == Color.Magenta.ToArgb())
                return "magenta";
            else if (color.ToArgb() == Color.Red.ToArgb())
                return "red";
            else if (color.ToArgb() == Color.White.ToArgb())
                return "white";
            else if (color.ToArgb() == Color.Yellow.ToArgb())
                return "yellow";
            else
                return "none";
        }
        /// <summary>
        /// Convert the float value to string.
        /// </summary>
        /// <param name="value">float value</param>
        /// <returns></returns>
        public string ToString(float value)
        {
            int val = (int)Math.Round(value);
            return val.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Get the RGB color code
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns></returns>
        private string GetRGBCode(Color color)
        {
            return color.R.ToString("X2") +
              color.G.ToString("X2") +
              color.B.ToString("X2");
        }
        /// <summary>
        /// Check whether the paragraph is rtl
        /// </summary>
        /// <param name="pStyle">The paragraph style</param>
        /// <param name="pFormat">The paragraph format</param>
        /// <returns></returns>
        private bool IsBidiPara(IWParagraphStyle pStyle, WParagraphFormat pFormat)
        {
            if (pFormat.HasValue(WParagraphFormat.BidiKey))
            {
                return (pFormat.Bidi) ? true : false;
            }
            if (pStyle == null)
                return false;

            if (pStyle != null && (pStyle.ParagraphFormat.HasValue(WParagraphFormat.BidiKey)))
            {
                return (pStyle.ParagraphFormat.Bidi) ? true : false;
            }
            WParagraphFormat format = pStyle.ParagraphFormat.BaseFormat as WParagraphFormat;

            while (format != null)
            {
                if (format.HasValue(WParagraphFormat.BidiKey))
                    return (format.Bidi) ? true : false;

                format = format.BaseFormat as WParagraphFormat;
            }

            return false;
        }
        /// <summary>
        /// Get the default font size
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private float GetDefFontSize(WordDocument doc, short key)
        {
            Style defStyle = doc.Styles.FindByName("Default Paragraph Font") as Style;
            Style normalStyle = doc.Styles.FindByName("Normal") as Style;
            float fontSize = 0;

            if (defStyle != null && defStyle.CharacterFormat.HasValue(key))
                fontSize = defStyle.CharacterFormat.FontSize;

            return fontSize;
        }
        /// <summary>
        /// Get the list pattern type as string.
        /// </summary>
        /// <param name="listLevel"></param>
        /// <returns></returns>
        private string GetPatternType(WListLevel listLevel)
        {
            string patternType = string.Empty;
            switch (listLevel.PatternType)
            {
                case ListPatternType.Arabic:
                    patternType = "decimal";
                    break;
                case ListPatternType.UpRoman:
                    patternType = "upperRoman";
                    break;
                case ListPatternType.LowRoman:
                    patternType = "lowerRoman";
                    break;
                case ListPatternType.UpLetter:
                    patternType = "upperLetter";
                    break;
                case ListPatternType.LowLetter:
                    patternType = "lowerLetter";
                    break;
                case ListPatternType.Ordinal:
                    patternType = "ordinal";
                    break;
                case ListPatternType.OrdinalText:
                    patternType = "ordinalText";
                    break;
                case ListPatternType.LeadingZero:
                    patternType = "decimalZero";
                    break;
                case ListPatternType.Bullet:
                    patternType = "bullet";
                    break;
                case ListPatternType.Number:
                    patternType = "cardinalText";
                    break;
                case ListPatternType.FarEast:
                    patternType = "aiueoFullWidth";
                    break;
                case ListPatternType.Special:
                    patternType = "russianLower";
                    break;
                default:
                    patternType = "none";
                    break;
            }
            return patternType;
        }
        /// <summary>
        /// Get the list symbol
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private char GetListSymbol(int level)
        {
            switch (level)
            {
                case 1:
                    return (char)0x00;
                case 2:
                    return (char)0x01;
                case 3:
                    return (char)0x02;
                case 4:
                    return (char)0x03;
                case 5:
                    return (char)0x04;
                case 6:
                    return (char)0x05;
                case 7:
                    return (char)0x06;
                case 8:
                    return (char)0x07;
                default:
                    return (char)0x08;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private string UpdateNumberPrefix(string prefix)
        {
            string numberPrefix = prefix;

            numberPrefix = numberPrefix.Replace(WListLevel.Level1Str, "%1");
            numberPrefix = numberPrefix.Replace(WListLevel.Level2Str, "%2");
            numberPrefix = numberPrefix.Replace(WListLevel.Level3Str, "%3");
            numberPrefix = numberPrefix.Replace(WListLevel.Level4Str, "%4");
            numberPrefix = numberPrefix.Replace(WListLevel.Level5Str, "%5");
            numberPrefix = numberPrefix.Replace(WListLevel.Level6Str, "%6");
            numberPrefix = numberPrefix.Replace(WListLevel.Level7Str, "%7");
            numberPrefix = numberPrefix.Replace(WListLevel.Level8Str, "%8");
            numberPrefix = numberPrefix.Replace(WListLevel.Level9Str, "%9");

            return numberPrefix;
        }
        /// <summary>
        /// Get the page number type as string
        /// </summary>
        /// <param name="pageNumberStyle">The page number style</param>
        /// <returns></returns>
        private string GetPageNumType(PageNumberStyle pageNumberStyle)
        {
            switch (pageNumberStyle)
            {
                case PageNumberStyle.RomanLower:
                    return "lowerRoman";
                case PageNumberStyle.RomanUpper:
                    return "upperRoman";
                case PageNumberStyle.LetterLower:
                    return "lowerLetter";
                case PageNumberStyle.LetterUpper:
                    return "upperLetter";
                default:
                    return "decimal";
            }
        }
        /// <summary>
        /// Get the next bookmark ID
        /// </summary>
        /// <returns></returns>
        private int GetNextBookmarkID()
        {
            return ++m_bookmarkId;
        }
        /// <summary>
        /// Get the docPr id (used for pictues)
        /// </summary>
        /// <returns></returns>
        private int GetNextDocPrID()
        {
            return ++m_docPrId;
        }
        /// <summary>
        /// Get the next shape ID
        /// </summary>
        /// <returns></returns>
        private int GetNextShapeID()
        {
            return ++m_shapeID;
        }
        /// <summary>
        /// Update the text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ModifyText(string text)
        {
            text = text.Replace(Environment.NewLine, CarriageReturn.ToString());
            text = text.Replace(NewLine, CarriageReturn);
            text = text.Replace('\a'.ToString(), string.Empty);
            text = text.Replace('\b'.ToString(), string.Empty);
            return text;
        }
        /// <summary>
        /// Get the base entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Entity GetBaseEntity(Entity entity)
        {
            Entity baseEntity = entity;
            do
            {
                if (baseEntity.Owner == null)
                    return baseEntity;
                baseEntity = baseEntity.Owner;
            }
            while (!(baseEntity is WSection || baseEntity is HeaderFooter));

            return baseEntity;
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
        /// <summary>
        /// Update item relations
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newID"></param>
        /// <param name="itemRel"></param>
        private void UpdateItemRelation(ParagraphItem item, string newID, DictionaryEntry itemRel)
        {
            if (GetBaseEntity(item) is HeaderFooter)
            {
                HeaderFooter headFoot = GetBaseEntity(item) as HeaderFooter;
                UpdateHFXmlRels(newID, headFoot, itemRel);
            }
            else
            {
                XmlItemsRelations.Add(newID, itemRel);
            }
        }
        /// <summary>
        /// Get the parent field of the paragraph item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ParagraphItem GetParentField(ParagraphItem item)
        {
            int fieldEndCount = 0;
            IEntity currentItem = item.PreviousSibling;

            while (currentItem != null)
            {
                WField curField = currentItem as WField;
                TableOfContent tocField = currentItem as TableOfContent;
                WFieldMark curFieldMark = currentItem as WFieldMark;

                if (curField != null || tocField != null)
                {
                    if (fieldEndCount == 0)
                    {
                        if (tocField != null)
                            return tocField.TOCField;
                        else
                            return curField;
                    }
                    else
                    {
                        fieldEndCount--;
                    }
                }

                if (curFieldMark != null && curFieldMark.Type == FieldMarkType.FieldEnd)
                    fieldEndCount++;

                currentItem = currentItem.PreviousSibling;
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="XmlParaItemStream"></param>
        /// <returns></returns>
        private List<string> FindRelationshipIds(Stream XmlParaItemStream)
        {
            XmlParaItemStream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(XmlParaItemStream);

            List<string> relationIds = new List<string>();
            bool skip = false;
            do
            {
                skip = false;
                string id = string.Empty;
                string href = string.Empty;

                switch (reader.LocalName)
                {
                    case "fill":
                    case "chart":
                    case "imagedata":
                    case "stroke":
                    case "control":
                    case "OLEObject":
                    case "hyperlink":
                        id = reader.GetAttribute("id", DocxConstants.R_namespace);
                        href = reader.GetAttribute("href", DocxConstants.R_namespace);
                        break;
                    case "blip":
                        id = reader.GetAttribute("embed", DocxConstants.R_namespace);
                        break;
                    case "relIds":
                        id = reader.GetAttribute("dm", DocxConstants.R_namespace);
                        if (!string.IsNullOrEmpty(id))
                            relationIds.Add(id);

                        id = reader.GetAttribute("lo", DocxConstants.R_namespace);
                        if (!string.IsNullOrEmpty(id))
                            relationIds.Add(id);

                        id = reader.GetAttribute("qs", DocxConstants.R_namespace);
                        if (!string.IsNullOrEmpty(id))
                            relationIds.Add(id);

                        id = reader.GetAttribute("cs", DocxConstants.R_namespace);
                        if (!string.IsNullOrEmpty(id))
                            relationIds.Add(id);
                        skip = true;
                        break;
                }

                if (id != null && id != string.Empty && !skip)
                {
                    relationIds.Add(id);
                }

                if (href != null && href != string.Empty)
                {
                    relationIds.Add(href);
                }

                reader.Read();
            }
            while (!reader.EOF);

            return relationIds;
        }
        /// <summary>
        /// Determines whether document style collection has Default Table style
        /// </summary>
        /// <returns></returns>
        private bool IsDocumentContainsDefaultTableStyle()
        {
            Style tableNormalStyle;
            if (((tableNormalStyle = m_document.Styles.FindByName("Normal Table") as Style) == null || (tableNormalStyle != null && tableNormalStyle.StyleType != StyleType.TableStyle)) &&
                ((tableNormalStyle = m_document.Styles.FindByName("NormalTable") as Style) == null || (tableNormalStyle != null && tableNormalStyle.StyleType != StyleType.TableStyle)) &&
                ((tableNormalStyle = m_document.Styles.FindByName("Table Normal") as Style) == null || (tableNormalStyle != null && tableNormalStyle.StyleType != StyleType.TableStyle)) &&
                ((tableNormalStyle = m_document.Styles.FindByName("TableNormal") as Style) == null || (tableNormalStyle != null && tableNormalStyle.StyleType != StyleType.TableStyle)))
                return false;
            return true;
        }
        #endregion Helper methods
    }
}