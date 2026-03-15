#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Compression.Zip;
using System.IO;
using System.Xml;
using Syncfusion.CompoundFile.DocIO.Native;
using System.Globalization;
using Syncfusion.Layouting;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Collections;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject;
#if !SILVERLIGHT && !WP
using System.Drawing.Imaging;
#else 
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#endif
using System.Net;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.DocIO.ReaderWriter.Security;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#if WINRT
using Syncfusion.DocIO.WinrtHelper;
using System.Threading.Tasks;
using Windows.Storage;
#elif !WP
using System.Drawing;
#endif

namespace Syncfusion.DocIO.DLS.Convertors
{
    /// <summary>
    /// Represents the parser for MS Word 2010 generated document.
    /// </summary>
    class DocxParser
    {
        #region Constants
        private const char NONBREAK_HYPHEN = (char)0x1E;
        private const char SOFT_HYPHEN = (char)0x1F;
        #endregion Constants

        #region Private members
        private ZipArchive m_docArchive;
        private XmlReader m_reader;
        private WordDocument m_doc;
        private string m_majorFontName;
        private string m_minorFontName;
        private bool IsRowChangeFormat;
        private bool IsCellChangeFormat;
        private bool IsTableChangeFormat;
        private bool m_isInHeyperlinkField;
        private float m_gutter;
        private string m_currentFile = string.Empty;
        private Dictionary<string, string> m_linkStyleNames;
        private Dictionary<string, string> m_baseStyleNames;
        private Dictionary<string, DictionaryEntry> m_docRelations;
        private Dictionary<string, DictionaryEntry> m_settingsRelations;
        private Dictionary<string, bool> m_isExternalHyperlink;
        private List<BookmarkInfo> m_bookmarkNames;
        private Dictionary<string, Dictionary<string, DictionaryEntry>> m_hfRelations;
        private Dictionary<string, WPicture> m_pictureBullet;
        private Dictionary<string, string> m_abstractListStyles;
        private Dictionary<string, string> m_overListStyles;
        private WCharacterFormat m_currentRunFormat;
        private bool m_isFootnote;
        private List<DictionaryEntry> m_footnote;
        private List<DictionaryEntry> m_endnote;
        private bool m_isPrevItemFieldStart;
        private FieldCharType m_currentFldCharType;
        private StringBuilder m_fieldInstrText = new StringBuilder();
        private Stack<WField> m_fieldStack;
        private TrackChangeType m_trackChangeType = TrackChangeType.None;
        private Entity m_postBkmk;
        private WCommentMark m_postCommMark;
        private Dictionary<string, WComment> m_comments;
        private Stack<WComment> m_commStack;
        private int m_currentTableCellIndex;
        private int m_gridCount;
        private short m_gridSpan;
        private Dictionary<string, int> m_imageIds;
        private string m_appVersion = string.Empty;
        private string m_documentPath;
        private List<FontFamilyNameRecord> m_fontFamilyRecords = new List<FontFamilyNameRecord>();
        private struct BookmarkInfo
        {
            /// <summary>
            /// Bookmark Id
            /// </summary>
            internal string bookmarkId;
            /// <summary>
            /// Bookmark name
            /// </summary>
            internal string bookmarName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the image ids.
        /// </summary>
        /// <value>The image ids.</value>
        private Dictionary<string, int> ImageIds
        {
            get
            {
                if (m_imageIds == null)
                    m_imageIds = new Dictionary<string, int>();
                return m_imageIds;
            }
        }
        /// <summary>
        /// Gets the comment collection.
        /// </summary>
        /// <value>The comment stack.</value>
        private Dictionary<string, WComment> Comments
        {
            get
            {
                if (m_comments == null)
                {
                    //Parse the comments part from the zip archive.
                    ParseComments();
                }
                return m_comments;
            }
        }
        /// <summary>
        /// Gets the comments stack.
        /// </summary>
        /// <value>The comments stack.</value>
        private Stack<WComment> CommentsStack
        {
            get
            {
                if (m_commStack == null)
                {
                    m_commStack = new Stack<WComment>();
                }
                return m_commStack;
            }
        }
        /// <summary>
        /// Gets the current comment.
        /// </summary>
        /// <value>The current comment.</value>
        private WComment CurrentComment
        {
            get
            {
                if (m_commStack != null && m_commStack.Count > 0)
                {
                    return m_commStack.Peek();
                }
                return null;
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
                {
                    m_fieldStack = new Stack<WField>();
                }
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
                if (m_fieldStack != null && m_fieldStack.Count > 0)
                {
                    return m_fieldStack.Peek();
                }
                return null;
            }
        }
        /// <summary>
        /// Gets the footnote by id.
        /// </summary>
        /// <value>The footnote.</value>
        private List<DictionaryEntry> Footnote
        {
            get
            {
                if (m_footnote == null || m_footnote.Count == 0)
                {
                    m_footnote = new List<DictionaryEntry>();
                }
                return m_footnote;
            }
        }


        /// <summary>
        /// Gets the endnote by id.
        /// </summary>
        /// <value>The endnote.</value>
        private List<DictionaryEntry> Endnote
        {
            get
            {
                if (m_endnote == null)
                {
                    m_endnote = new List<DictionaryEntry>();
                }
                return m_endnote;
            }
        }
        /// <summary>
        /// Gets the collection of override list style names.
        /// </summary>
        /// <value>The over list style names.</value>
        private Dictionary<string, string> OverListStyleNames
        {
            get
            {
                if (m_overListStyles == null)
                {
                    m_overListStyles = new Dictionary<string, string>();
                }

                return m_overListStyles;
            }
        }
        /// <summary>
        /// Gets the abstract list styles.
        /// </summary>
        /// <value>The abstract list styles.</value>
        private Dictionary<string, string> AbstractListStyleNames
        {
            get
            {
                if (m_abstractListStyles == null)
                {
                    m_abstractListStyles = new Dictionary<string, string>();
                }
                return m_abstractListStyles;
            }
        }
        /// <summary>
        /// Gets the collection of picture bullet.
        /// </summary>
        /// <value>The picture bullet.</value>
        private Dictionary<string, WPicture> PictureBullet
        {
            get
            {
                if (m_pictureBullet == null)
                {
                    m_pictureBullet = new Dictionary<string, WPicture>();
                }
                return m_pictureBullet;
            }
        }
        /// <summary>
        /// Gets the collection of document relations.
        /// </summary>
        /// <value>The document relations.</value>
        private Dictionary<string, DictionaryEntry> DocumentRelations
        {
            get
            {
                if (m_docRelations == null)
                {
                    m_docRelations = new Dictionary<string, DictionaryEntry>();
                }
                return m_docRelations;
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
        /// Represents the external hyperlink
        /// </summary>
        /// <value></value>
        private Dictionary<string, bool> IsExternalHyperlink
        {
            get
            {
                if (m_isExternalHyperlink == null)
                {
                    m_isExternalHyperlink = new Dictionary<string, bool>();
                }
                return m_isExternalHyperlink;
            }
        }
        /// <summary>
        /// Gets the bookmark names collection.
        /// </summary>
        /// <value>The name of the bookmark.</value>
        private List<BookmarkInfo> BookmarkNames
        {
            get
            {
                if (m_bookmarkNames == null)
                {
                    m_bookmarkNames = new List<BookmarkInfo>();
                }
                return m_bookmarkNames;
            }
        }
        /// <summary>
        /// Represents the HeaderFooter relations.
        /// </summary>
        /// <value>The headers footers rel.</value>
        private Dictionary<string, Dictionary<string, DictionaryEntry>> HFRelations
        {
            get
            {
                if (m_hfRelations == null)
                {
                    m_hfRelations = new Dictionary<string, Dictionary<string, DictionaryEntry>>();
                }
                return m_hfRelations;
            }
        }
        /// <summary>
        /// Gets the collection of base style names.
        /// </summary>
        /// <value>The base style names.</value>
        private Dictionary<string, string> BaseStyleNames
        {
            get
            {
                if (m_baseStyleNames == null)
                {
                    m_baseStyleNames = new Dictionary<string, string>();
                }
                return m_baseStyleNames;
            }
        }
        /// <summary>
        /// Gets the collection of style name and id.
        /// </summary>
        /// <value>The style name id.</value>
        private Dictionary<string, string> StyleNameId
        {
            get
            {
                return m_doc.StyleNameIds;
            }
        }
        /// <summary>
        /// Gets the link style names.
        /// </summary>
        /// <value>The link style names.</value>
        private Dictionary<string, string> LinkStyleNames
        {
            get
            {
                if (m_linkStyleNames == null)
                {
                    m_linkStyleNames = new Dictionary<string, string>();
                }
                return m_linkStyleNames;
            }
        }
        /// <summary>
        /// Gets the AppVersion.
        /// </summary>
        /// <value>The AppVersion.</value>
        private string AppVersion
        {
            get
            {
                if (m_appVersion.StartsWith("12"))
                    return "Word2007";
                else if (m_appVersion.StartsWith("14"))
                    return "Word2010";
                else if (m_appVersion.StartsWith("15"))
                    return "Word2013";
                else
                    return "Docx";
            }
            set
            {
                m_appVersion = value;
            }
        }
        #endregion

        #region General
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Reads the specified document path.
        /// </summary>
        /// <param name="documentPath">The document path.</param>
        /// <param name="doc">The Word document.</param>
        /// <returns></returns>
        internal WordDocument Read(string fileName, WordDocument document)
        {
            m_docArchive = new ZipArchive();
            if (fileName == null || fileName.Length == 0)
                throw new ArgumentOutOfRangeException("inputFileName");

            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (document.CheckForEncryption(stream))
                {
                    Stream decryptedStream = DecryptDocumentStream(stream, document);
                    m_docArchive.Open(decryptedStream, false);
                }
                else
                {
                    m_docArchive.Open(stream, false);
                }
                //Specific to preserve document as non encrypted. Removes the password which is passed as parameter in open method.
                document.Password = null;
            }
            m_doc = document;
            Read(document);

            return document;
        }
#endif
        /// <summary>
        /// Reads the specified data stream.
        /// </summary>
        /// <param name="dataStream">The document stream.</param>
        /// <param name="doc">Instance of Word document.</param>
        /// <returns></returns>
        internal WordDocument Read(Stream docStream, WordDocument document)
        {
            m_docArchive = new ZipArchive();
            if (document.CheckForEncryption(docStream))
            {
                Stream decryptedStream = DecryptDocumentStream(docStream, document);
                m_docArchive.Open(decryptedStream, false);
            }
            else
            {
#if WINRT
                m_docArchive.Open(docStream);
#else
                m_docArchive.Open(docStream, false);
#endif
            }
            //Specific to preserve document as non encrypted. Removes the password which is passed as parameter in open method.
            document.Password = null;
            m_doc = document;
            Read(document);

            return document;
        }
        /// <summary>
        /// Gets the decrypted document stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private Stream DecryptDocumentStream(Stream stream, WordDocument doc)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Stream result = new MemoryStream();
            bool bEncrypted = false;

            using (ICompoundFile file = doc.CreateCompoundFile(stream))
            {
                ICompoundStorage storage = file.RootStorage;
                SecurityHelper securityHelper = new SecurityHelper();
                SecurityHelper.EncrytionType encryptionType = securityHelper.GetEncryptionType(storage);
                if (encryptionType != SecurityHelper.EncrytionType.None)
                {
                    if (doc.Password == null)
                        throw new ArgumentException("Document is encrypted, password is needed to open the document");
                    switch (encryptionType)
                    {
                        case SecurityHelper.EncrytionType.Standard:
                            {
                                bEncrypted = true;
                                // Decrypt the document encrypted using Standard Encryption.
                                StandardDecryptor decryptor = new StandardDecryptor();
                                decryptor.Initialize(storage);

                                if (!decryptor.CheckPassword(doc.Password))
                                    throw new Exception("Specified password \"" + doc.Password + "\" is incorrect!");

                                result = decryptor.Decrypt();
                            }
                            break;
                        case SecurityHelper.EncrytionType.Agile:
                            {
                                bEncrypted = true;
                                // Decrypt the document encrypted using Agile Encryption.
                                AgileDecryptor decryptor = new AgileDecryptor();
                                decryptor.Initialize(storage);

                                if (!decryptor.CheckPassword(doc.Password))
                                    throw new Exception("Specified password \"" + doc.Password + "\" is incorrect!");

                                result = decryptor.Decrypt();
                            }
                            break;
                    }
                }
            }
            if (!bEncrypted)
#if WINRT
                throw new Exception("Wrong Word version");
#else
                throw new ApplicationException("Wrong Word version");
#endif
            return result;
        }
        /// <summary>
        /// Reads the word document
        /// </summary>
        /// <param name="document">The Word Document</param>
        private void Read(WordDocument document)
        {
            document.DocxPackage = new Package();
            document.DocxPackage.Load(m_docArchive);
#if WINRT
            m_docArchive.Dispose();
#else
            m_docArchive.Close();
#endif
            m_docArchive = null;
            UpdatePath(document.DocxPackage);
            ParseDocumentProperties(document.DocxPackage);
            UpdateFormatType(document);
            ParseDocument(document.DocxPackage);
        }
        /// <summary>
        /// Update the main document path
        /// </summary>
        private void UpdatePath(Package package)
        {
            //Update the main document part path
            m_documentPath = "document.xml";
            string path = GetPathByContentType(DocxConstants.DocumentContentType, package);
            if (path != null)
                m_documentPath = path;
            path = GetPathByContentType(DocxConstants.MacroDocumentContentType, package);
            if (path != null)
                m_documentPath = path;
            path = GetPathByContentType(DocxConstants.MacroTemplateContentType, package);
            if (path != null)
                m_documentPath = path;
            path = GetPathByContentType(DocxConstants.TemplateContentType, package);
            if (path != null)
                m_documentPath = path;
        }
        /// <summary>
        /// Get the path by its content type
        /// </summary>
        /// <param name="document">The extension.</param>
        private string GetPathByContentType(string contentType, Package package)
        {
            //Set the Xml stream postion to zero to read from begining
            package.XmlParts[DocxConstants.ContentTypesPath].DataStream.Position = 0;
            XmlReader reader = XmlReader.Create(m_doc.DocxPackage.XmlParts[DocxConstants.ContentTypesPath].DataStream);
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "Types")
                throw new XmlException("Expected xml tag \"Types\"");

            reader.Read();
            while (reader.LocalName != "Types")
            {
                if (reader.LocalName == "Override"
                    && reader.HasAttributes)
                {
                    string type = reader.GetAttribute("ContentType");
                    if (contentType == type)
                    {
                        string partName = reader.GetAttribute("PartName");
                        if (partName != null && partName != string.Empty)
                        {
                            string[] stringArr = partName.Split('/');
                            return stringArr[stringArr.Length - 1];
                        }
                    }
                }
                reader.Read();
            }
            //Set the Xml stream postion to zero to read from begining
            package.XmlParts[DocxConstants.ContentTypesPath].DataStream.Position = 0;
            return null;
        }
        /// <summary>
        /// Get the content type of extension
        /// </summary>
        /// <param name="document">The extension.</param>
        private string GetExtensionContentType(string extension)
        {
            //Set the Xml stream postion to zero to read from begining
            m_doc.DocxPackage.XmlParts[DocxConstants.ContentTypesPath].DataStream.Position = 0;
            XmlReader reader = XmlReader.Create(m_doc.DocxPackage.XmlParts[DocxConstants.ContentTypesPath].DataStream);
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "Types")
                throw new XmlException("Expected xml tag \"Types\"");

            reader.Read();
            while (reader.LocalName != "Types")
            {
                if (reader.LocalName == "Default"
                    && reader.HasAttributes)
                {
                    string partName = reader.GetAttribute("Extension");
                    if (extension == partName)
                    {
                        return reader.GetAttribute("ContentType");
                    }
                }
                reader.Read();
            }

            return null;
        }
        /// <summary>
        /// Get Format Type based AppVersion.
        /// </summary>
        /// <param name="document">The document.</param>
        private FormatType GetFormatType(string type)
        {
            switch (AppVersion)
            {
                case "Word2007":
                    if (type == "DOCM")
                        return FormatType.Word2007Docm;
                    else if (type == "DOTM")
                        return FormatType.Word2007Dotm;
                    else if (type == "DOTX")
                        return FormatType.Word2007Dotx;
                    else
                        return FormatType.Word2007;
                    break;
                case "Word2010":
                    if (type == "DOCM")
                        return FormatType.Word2010Docm;
                    else if (type == "DOTM")
                        return FormatType.Word2010Dotm;
                    else if (type == "DOTX")
                        return FormatType.Word2010Dotx;
                    else
                        return FormatType.Word2010;
                    break;
                case "Word2013":
                    if (type == "DOCM")
                        return FormatType.Word2013Docm;
                    else if (type == "DOTM")
                        return FormatType.Word2013Dotm;
                    else if (type == "DOTX")
                        return FormatType.Word2013Dotx;
                    else
                        return FormatType.Word2013;
                    break;
            }
            return FormatType.Docx;
        }
        /// <summary>
        /// Updates the document format type.
        /// </summary>
        /// <param name="document">The document.</param>
        private void UpdateFormatType(WordDocument document)
        {
            //Set the Xml stream postion to zero to read from begining
            document.DocxPackage.XmlParts[DocxConstants.ContentTypesPath].DataStream.Position = 0;
            document.ActualFormatType = GetFormatType("DOCX");
            XmlReader reader = XmlReader.Create(document.DocxPackage.XmlParts[DocxConstants.ContentTypesPath].DataStream);
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "Types")
                throw new XmlException("Expected xml tag \"Types\"");

            reader.Read();
            while (reader.LocalName != "Types")
            {
                if (reader.LocalName == "Override"
                    && reader.HasAttributes)
                {
                    string partName = reader.GetAttribute("PartName");
                    if ("/word/" + m_documentPath == partName)
                    {
                        if (reader.GetAttribute("ContentType") == DocxConstants.TemplateContentType)
                        {
                            document.ActualFormatType = GetFormatType("DOTX");
                            break;
                        }
                        else if (reader.GetAttribute("ContentType") == DocxConstants.MacroTemplateContentType)
                        {
                            document.ActualFormatType = GetFormatType("DOTM");
                            break;
                        }
                        else if (reader.GetAttribute("ContentType") == DocxConstants.MacroDocumentContentType)
                        {
                            document.ActualFormatType = GetFormatType("DOCM");
                            break;
                        }
                    }
                }
                reader.Read();
            }
            document.DocxPackage.XmlParts[DocxConstants.ContentTypesPath].DataStream.Position = 0;
        }
        /// <summary>
        /// Parse the document and its relations
        /// </summary>
        /// <param name="package"></param>
        private void ParseDocument(Package package)
        {
            PartContainer partContainer = package.FindPartContainer("word/theme/");
            // Parse theme
            if (partContainer.XmlParts.ContainsKey("theme1.xml"))
            {
                ParseTheme(partContainer.XmlParts["theme1.xml"].DataStream);
            }

            partContainer = package.FindPartContainer("word/");
            // Parse document relations

            if (partContainer.Relations.ContainsKey("word/_rels/" + m_documentPath + ".rels"))
            {
                Relations rels = partContainer.Relations["word/_rels/" + m_documentPath + ".rels"];
                ParseDocumentRelations(rels.DataStream);
            }

            partContainer = package.FindPartContainer("word/");

            if (partContainer.XmlParts.ContainsKey("numbering.xml"))
            {
                Part numberingPart = partContainer.XmlParts["numbering.xml"];
                if (numberingPart != null && numberingPart.DataStream != null && numberingPart.DataStream.Length > 0)
                {
                    m_reader = UtilityMethods.CreateReader(numberingPart.DataStream);
                    ParseNumberings(m_reader);
#if WINRT
                    m_reader.Dispose();
#else
                    m_reader.Close();
#endif
                }
            }

            if (partContainer.XmlParts.ContainsKey("styles.xml"))
            {
                m_reader = UtilityMethods.CreateReader(partContainer.XmlParts["styles.xml"].DataStream);
                ParseStyles(m_reader);
#if WINRT
                m_reader.Dispose();
#else
                m_reader.Close();
#endif
            }

            //Parse the footnotes.xml part
            if (partContainer.XmlParts.ContainsKey("footnotes.xml"))
                ParseFootnotePart(true);
            //Parse the endnotes.xml part 
            if (partContainer.XmlParts.ContainsKey("endnotes.xml"))
                ParseFootnotePart(false);

            foreach (string partName in partContainer.XmlParts.Keys)
            {
                if (partName == m_documentPath)
                {
                    ParseDocument(partContainer.XmlParts[m_documentPath].DataStream);
                    break;
                }
            }
            // Parse Settings relations
            if (partContainer.Relations.ContainsKey("word/_rels/settings.xml.rels"))
                ParseSettingsRelations(partContainer.Relations["word/_rels/settings.xml.rels"].DataStream);
            if (partContainer.XmlParts.ContainsKey("settings.xml"))
                ParseSettings(partContainer.XmlParts["settings.xml"].DataStream);

            if (partContainer.XmlParts.ContainsKey("fontTable.xml"))
                ParseFontTable(partContainer.XmlParts["fontTable.xml"].DataStream);
            // Parse macros.
            if (partContainer.XmlParts.ContainsKey(DocxConstants.VbaProject))
                ParseVbaProject(partContainer.XmlParts[DocxConstants.VbaProject].DataStream);
            if (partContainer.XmlParts.ContainsKey(DocxConstants.VbaData))
                ParseVbaData(partContainer.XmlParts[DocxConstants.VbaData].DataStream);
            //Custom UI
            partContainer = m_doc.DocxPackage.FindPartContainer("customUI/");
            if (partContainer.XmlParts.ContainsKey("customUI.xml"))
                m_doc.CustomUIPartContainer = partContainer;
            //Custom XML
            partContainer = m_doc.DocxPackage.FindPartContainer("customXml/");
            if (partContainer.Name == "customXml/")
                m_doc.CustomXMLContainer = partContainer;
        }
        /// <summary>
        /// Clears the parsed image from package.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="containerName">Name of the container.</param>
        private void ClearParsedImage(string imageName, string containerName)
        {
            PartContainer container = m_doc.DocxPackage.FindPartContainer(containerName);
            if (container.XmlParts.ContainsKey(imageName))
            {
                Part part = container.XmlParts[imageName];
                container.XmlParts.Remove(imageName);
#if WINRT
                part.DataStream.Dispose();
#else
                part.DataStream.Close();
#endif
                part = null;
            }
        }
        /// <summary>
        /// Parse the theme xml part
        /// </summary>
        /// <param name="stream">Theme part stream</param>
        private void ParseTheme(Stream stream)
        {
            m_majorFontName = GetBaseFontName(stream, true);
            m_minorFontName = GetBaseFontName(stream, false);
            ParseThemeColor(stream);
            return;
        }

        private void ParseThemeColor(Stream stream)
        {
            string tagName = "clrScheme";
            stream.Position = 0;
            XmlReader reader = XmlReader.Create(stream);
            reader.ReadToFollowing(tagName, DocxConstants.A_namespace); //"http://schemas.openxmlformats.org/drawingml/2006/main");
            if (reader.EOF)
                return;

            while (reader.Read() && reader.LocalName != tagName)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string value;
                    string key;
                    switch (reader.LocalName)
                    {
                        case "dk1":
                        case "lt1":
                            key = reader.LocalName;
                            reader.Read();
                            SkipWhitespaces(reader);
                            value = reader.GetAttribute("lastClr");
                            m_doc.SchemeColor.Add(key, GetColorValue(value));
                            reader.Read();
                            break;
                        default:
                            key = reader.LocalName;
                            reader.Read();
                            SkipWhitespaces(reader);
                            value = reader.GetAttribute("val");
                            m_doc.SchemeColor.Add(key, GetColorValue(value));
                            reader.Read();
                            break;

                    }
                }
            }
        }
        /// <summary>
        /// Gets the name of the base font.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isMajor">if it is major, set to <c>true</c>.</param>
        /// <returns></returns>
        private string GetBaseFontName(Stream stream, bool isMajor)
        {
            string tagName = isMajor ? "majorFont" : "minorFont";
            stream.Position = 0;
            XmlReader reader = XmlReader.Create(stream);
            reader.ReadToFollowing(tagName, "http://schemas.openxmlformats.org/drawingml/2006/main");
            //MoveToTag(reader, tagName);
            if (reader.EOF)
                return null;

            while (reader.Read())
            {
                if (reader.LocalName == "latin")
                {
                    string value = reader.GetAttribute("typeface");
                    if (value != null)
                    {
                        stream.Position = 0;
                        return value;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Extract the DocProperties part
        /// </summary>
        /// <param name="package">Docx package</param>
        private void ParseDocumentProperties(Package package)
        {
            PartContainer partContainer = package.FindPartContainer("docProps/");
            foreach (string partName in partContainer.XmlParts.Keys)
            {
                Stream partStream = partContainer.XmlParts[partName].DataStream;

                switch (partName)
                {
                    case "app.xml":
                        m_reader = UtilityMethods.CreateReader(partStream);
                        ParseAppProperties(m_reader);
#if WINRT
                        m_reader.Dispose();
#else
                        m_reader.Close();
#endif
                        break;
                    case "core.xml":
                        m_reader = UtilityMethods.CreateReader(partStream);
                        ParseCoreProperties(m_reader);
#if WINRT
                        m_reader.Dispose();
#else
                        m_reader.Close();
#endif
                        break;
                    case "custom.xml":
                        m_reader = UtilityMethods.CreateReader(partStream);
                        ParseCustomProperties(m_reader);
#if WINRT
                        m_reader.Dispose();
#else
                        m_reader.Close();
#endif
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion General

        #region Implementation / Macros
        /// <summary>
        /// Parses the vba project.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void ParseVbaProject(Stream stream)
        {
            m_doc.VbaProject = stream;
        }
        /// <summary>
        /// Parses the vba data.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void ParseVbaData(Stream stream)
        {
            XmlReader reader = UtilityMethods.CreateReader(stream);

            if (reader == null)
                throw new Exception("reader is null");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "vbaSuppData")
                throw new XmlException("Expected xml tag \"vbaSuppData\"");

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != "vbaSuppData")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "mcds":
                            reader.Read();
                            ParseMacroData(reader);
                            break;
                        case "docEvents":
                            reader.Read();
                            ParseDocEvents(reader);
                            break;
                    }
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the macro data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseMacroData(XmlReader reader)
        {
            if (reader == null)
                throw new Exception("reader is null");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "mcd")
                throw new XmlException("Expected xml tag \"mcd\"");

            SkipWhitespaces(reader);
            while (reader.LocalName != "mcds")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "mcd":
                            MacroData macro = new MacroData();
                            macro.Name = reader.GetAttribute("name", DocxConstants.WNE_namespace);
                            macro.Encrypt = reader.GetAttribute("bEncrypt", DocxConstants.WNE_namespace);
                            macro.Cmg = reader.GetAttribute("cmg", DocxConstants.WNE_namespace);
                            m_doc.VbaData.Add(macro);
                            break;
                    }
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the doc events.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseDocEvents(XmlReader reader)
        {
            if (reader == null)
                throw new Exception("reader is null");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            SkipWhitespaces(reader);
            while (reader.LocalName != "docEvents")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    m_doc.DocEvents.Add(reader.LocalName);
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }
        #endregion

        #region Parse FontTable
        /// <summary>
        /// Parses the font table.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void ParseFontTable(Stream stream)
        {
            XmlReader reader = UtilityMethods.CreateReader(stream);

            if (reader == null)
                throw new Exception("reader is null");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "fonts")
                throw new XmlException("Expected xml tag \"fonts\"");

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != "fonts")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "font":
                            string fontName = reader.GetAttribute("name", DocxConstants.W_namespace);
                            ParseFontDetails(reader, fontName);
                            break;
                    }
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
            //Update Font Table
            UpdateFontTable();
        }
        /// <summary>
        /// Update Font Table
        /// </summary>
        private void UpdateFontTable()
        {
            m_doc.FFNStringTable = new FontFamilyNameStringTable();
            m_doc.FFNStringTable.RecordsCount = m_fontFamilyRecords.Count;
            int index = 0;
            foreach (FontFamilyNameRecord ffnRecord in m_fontFamilyRecords)
            {
                m_doc.FFNStringTable.FontFamilyNameRecords[index] = ffnRecord;
                index++;
            }
        }
        /// <summary>
        /// Parses the font details.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fontName">The fontName.</param>
        private void ParseFontDetails(XmlReader reader, string fontName)
        {
            if (reader == null)
                throw new Exception("reader is null");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "font")
                throw new XmlException("Expected xml tag \"font\"");

            if (reader.IsEmptyElement)
                return;
            FontFamilyNameRecord ffnRecord = new FontFamilyNameRecord();
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != "font")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "altName":
                            string altFont = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (m_doc.FontSubstitutionTable.ContainsKey(fontName))
                                m_doc.FontSubstitutionTable[fontName] = altFont;
                            else
                                m_doc.FontSubstitutionTable.Add(fontName, altFont);
                            ffnRecord.AlternativeFontName = altFont;
                            break;
                        case "charset":
                            string charset = reader.GetAttribute("val", DocxConstants.W_namespace);
                            try
                            {
                                ffnRecord.CharacterSetId = Convert.ToByte(charset);
                            }
                            catch
                            {
                                //Default char set
                                ffnRecord.CharacterSetId = 1;
                            }
                            break;
                        case "family":
                            string fontFamily = reader.GetAttribute("val", DocxConstants.W_namespace);
                            switch (fontFamily)
                            {
                                case "auto":
                                    ffnRecord.FontFamilyID = 0;
                                    break;
                                case "roman":
                                    ffnRecord.FontFamilyID = 1;
                                    break;
                                case "swiss":
                                    ffnRecord.FontFamilyID = 2;
                                    break;
                                case "modern":
                                    ffnRecord.FontFamilyID = 3;
                                    break;
                                case "script":
                                    ffnRecord.FontFamilyID = 4;
                                    break;
                                case "decorative":
                                    ffnRecord.FontFamilyID = 5;
                                    break;
                            }
                            break;
                        case "pitch":
                            string pitchRequest = reader.GetAttribute("val", DocxConstants.W_namespace);
                            switch (pitchRequest)
                            {
                                case "default":
                                    ffnRecord.PitchRequest = 0;
                                    break;
                                case "fixed":
                                    ffnRecord.PitchRequest = 1;
                                    break;
                                case "variable":
                                    ffnRecord.PitchRequest = 2;
                                    break;
                            }
                            break;
                        //Parse Signature
                        case "sig":
                            string sig = reader.GetAttribute("usb0", DocxConstants.W_namespace);
                            if (sig != null && sig != string.Empty)
                                ffnRecord.SigUsb0 = GetBytes(sig);
                            sig = reader.GetAttribute("usb1", DocxConstants.W_namespace);
                            if (sig != null && sig != string.Empty)
                                ffnRecord.SigUsb1 = GetBytes(sig);
                            sig = reader.GetAttribute("usb2", DocxConstants.W_namespace);
                            if (sig != null && sig != string.Empty)
                                ffnRecord.SigUsb2 = GetBytes(sig);
                            sig = reader.GetAttribute("usb3", DocxConstants.W_namespace);
                            if (sig != null && sig != string.Empty)
                                ffnRecord.SigUsb3 = GetBytes(sig);
                            sig = reader.GetAttribute("csb0", DocxConstants.W_namespace);
                            if (sig != null && sig != string.Empty)
                                ffnRecord.SigCsb0 = GetBytes(sig);
                            sig = reader.GetAttribute("csb1", DocxConstants.W_namespace);
                            if (sig != null && sig != string.Empty)
                                ffnRecord.SigCsb1 = GetBytes(sig);
                            break;
                    }
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
            ffnRecord.FontName = fontName;
            m_fontFamilyRecords.Add(ffnRecord);
        }
        /// <summary>
        /// Get Bytes from string
        /// </summary>
        /// <returns></returns>
        private byte[] GetBytes(string signature)
        {
            byte[] sig = new byte[4];
            if (signature.Length == 8)
            {
                //Reverse the signature string
                char[] charArray = signature.ToCharArray();
                Array.Reverse(charArray);
                signature = new string(charArray);
                int startIndex = 0;
                for (int i = 0; i < 4; i++)
                {
                    sig[i] = Convert.ToByte(signature.Substring(startIndex, 2), 16);
                    startIndex += 2;
                }
            }
            return sig;
        }
        #endregion

        #region Document elements
        /// <summary>
        /// Parse the document
        /// </summary>
        /// <param name="stream">The document.xml stream</param>
        private void ParseDocument(Stream stream)
        {
            XmlReader reader = UtilityMethods.CreateReader(stream);

            if (reader == null)
                throw new Exception("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "document")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != "document")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "background":
                            //Parse the document background.
                            ParseDocumentBackground(reader);
                            skip = true;
                            break;
                        case "body":
                            //Parse the document body
                            m_doc.AddSection();
                            ParseBody(reader, null);
                            break;
                        default:
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse the document body
        /// </summary>
        /// <param name="reader">The Xml reader</param>
        /// <param name="entity">The entity</param>
        private void ParseBody(XmlReader reader, IEntity entity)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            string endNode = reader.LocalName;

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "p":
                            //Parse the paragraph
                            IWParagraph paragraph = AddParagraph(entity);
                            AddPostElements(paragraph);
                            ParseParagraphItems(reader, paragraph.Items);

                            if (!String.IsNullOrEmpty(paragraph.StyleName))
                                paragraph.ApplyStyle(paragraph.StyleName);
                            break;
                        case "tbl":
                            //Parse the table.
                            IWTable table = AddTable(entity);
                            //Set IsAutoResized property as true to layout the table with Auto width (Default in DocX format document)
                            table.TableFormat.IsAutoResized = true;
                            ParseTable(reader, table as WTable);
                            break;
                        case "sectPr":
                            //Parse the final section properties
                            ParseSectionProperties(reader, m_doc.LastSection);
                            break;
                        case "bookmarkStart":
                            //Parse the bookmark Start element
                            ParseBookmarkStart(reader, null);
                            break;
                        case "bookmarkEnd":
                            //Parse the bookmark End element
                            ParseBookmarkEnd(reader, entity);
                            break;
                        case "comment":
                            ParseComment(reader);
                            break;
                        case "commentRangeStart":
                            ParseCommentRangeStart(reader, null);
                            break;
                        case "commentRangeEnd":
                            ParseCommentRangeEnd(reader, null);
                            break;
                        case "sdt":
                            IStructureDocumentTagBlock sdTagBlock = AddStructureDocumentTagBlock(entity);
                            ParseStructureDocumentTagBlock(reader, sdTagBlock as StructureDocumentTagBlock);
                            break;
                        case "altChunk":
                            AlternateChunk altChuk = AddAlternateChunk(entity);
                            ParseAlternateChunk(reader, altChuk as AlternateChunk);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }

        #region Paragraph
        /// <summary>
        /// Add the paragraph to the corresponding textbody
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IWParagraph AddParagraph(IEntity entity)
        {
            IWParagraph paragraph = null;

            if (entity is HeaderFooter)
            {
                paragraph = (entity as HeaderFooter).AddParagraph();
            }
            else if (entity is WFootnote)
            {
                paragraph = (entity as WFootnote).TextBody.AddParagraph();
            }
            else if (entity is WComment)
            {
                paragraph = (entity as WComment).TextBody.AddParagraph();
            }
            else if (entity is StructureDocumentTagBlock)
            {
                paragraph = (entity as StructureDocumentTagBlock).SDTContent.TextBody.AddParagraph();
            }
            else if (entity is WTextBody)
            {
                paragraph = (entity as WTextBody).AddParagraph();
            }
            else
            {
                paragraph = m_doc.LastSection.AddParagraph();
            }

            return paragraph;
        }
        /// <summary>
        /// Add the post elements to the paragraph
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        private void AddPostElements(IWParagraph paragraph)
        {
            if (m_postBkmk != null)
            {
                paragraph.Items.Add(m_postBkmk);
                m_postBkmk = null;
            }

            if (m_postCommMark != null)
            {
                paragraph.Items.Add(m_postCommMark);
                m_postCommMark = null;
            }
        }
        /// <summary>
        /// Parse the pargraph element
        /// </summary>
        /// <param name="reader">Xml reader</param>
        /// <param name="paragraph">The paragraph object</param>
        private void ParseParagraphItems(XmlReader reader, ParagraphItemCollection paraItems)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();
            MemoryStream drawingStream = null;
            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case DocxConstants.c_paragraphFormatTag:
                            WParagraph paragraph = paraItems.OwnerBase as WParagraph;
                            if (paraItems.Owner is SDTInlineContent)
                                paragraph = paraItems.Owner.Owner.Owner as WParagraph;
                            if (paragraph != null)
                            {
                                ParseParagraphFormat(reader, paragraph.ParagraphFormat);
                                if (paragraph.StyleName == null || paragraph.StyleName == string.Empty)
                                {
                                    if (m_doc.Styles.FindByName("Normal") != null)
                                        paragraph.ApplyStyle("Normal");
                                }
                            }
                            break;
                        case "r":
                            ParseRun(reader, paraItems);
                            m_currentRunFormat = null;
                            break;
                        case "bookmarkStart":
                            ParseBookmarkStart(reader, paraItems);
                            break;
                        case "bookmarkEnd":
                            ParseBookmarkEnd(reader, paraItems);
                            break;
                        case "commentRangeStart":
                            ParseCommentRangeStart(reader, paraItems);
                            break;
                        case "commentRangeEnd":
                            ParseCommentRangeEnd(reader, paraItems);
                            break;
                        case "del":
                        case "moveFrom":
                            m_trackChangeType = TrackChangeType.IsDelete;
                            ParseParagraphItems(reader, paraItems);
                            m_trackChangeType = TrackChangeType.None;
                            break;
                        case "ins":
                        case "moveTo":
                            m_trackChangeType = TrackChangeType.IsInsert;
                            ParseParagraphItems(reader, paraItems);
                            m_trackChangeType = TrackChangeType.None;
                            break;
                        case "hyperlink":
                            ParseHyperlink(reader, paraItems);
                            break;
                        case "fldSimple":
                            skip = ParseFieldSimple(reader, paraItems);
                            break;
                        case "drawing":
                            ParagraphItem item = ParseDrawing(reader, paraItems, ref drawingStream);
                            paraItems.Add(item);
                            m_currentRunFormat = null;
                            skip = true;
                            break;
                        case "sdt":
                            IStructureDocumentTagInline sdTagInline = new StructureDocumentTagInline(m_doc);
                            AddItem(sdTagInline as ParagraphItem, paraItems);
                            if (paraItems.OwnerBase is WParagraph)
                                (paraItems.OwnerBase as WParagraph).m_bHasSDTInlineItem = true;
                            ParseStructureDocumentTagInline(reader, sdTagInline as StructureDocumentTagInline);
                            break;
                        default:
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        #endregion Paragraph

        #region Comments
        /// <summary>
        /// Parses the comment.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseComment(XmlReader reader)
        {
            if (m_comments == null)
            {
                m_comments = new Dictionary<string, WComment>();
            }

            WComment comment = new WComment(m_doc);
            string id = reader.GetAttribute("id", DocxConstants.W_namespace);
            comment.Format.TagBkmk = Int32.Parse(reader.GetAttribute("id", DocxConstants.W_namespace));

            string author = reader.GetAttribute("author", DocxConstants.W_namespace);
            if (author != null)
            {
                comment.Format.User = author;
            }

            string userInits = reader.GetAttribute("initials", DocxConstants.W_namespace);
            if (userInits != null)
            {
                comment.Format.UserInitials = userInits;
            }
            m_comments.Add(id, comment);

            ParseBody(reader, comment);
        }
        /// <summary>
        /// Parse the comments part (comments.xml)
        /// </summary>
        private void ParseComments()
        {

            Part commPart = FindPart("word/", "comments.xml");

            if (commPart == null || commPart.DataStream == null || commPart.DataStream.Length == 0)
                return;

            commPart.DataStream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(commPart.DataStream);
            m_currentFile = "comments.xml";
            ParseBody(reader, null);
            m_currentFile = string.Empty;
        }
        /// <summary>
        /// Parses the comment start.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="para">The paragraph.</param>
        private void ParseCommentRangeStart(XmlReader reader, ParagraphItemCollection paraItems)
        {
            string id = reader.GetAttribute("id", DocxConstants.W_namespace);

            WCommentMark commMark = new WCommentMark(m_doc, Int32.Parse(id));
            if (paraItems != null && paraItems.OwnerBase != null)
                paraItems.Add(commMark);
            else
                m_postCommMark = commMark;

            if (Comments != null && Comments.ContainsKey(id))
                CommentsStack.Push(Comments[id]);
        }
        /// <summary>
        /// Parses the comment end.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="para">The paragraph.</param>
        private void ParseCommentRangeEnd(XmlReader reader, ParagraphItemCollection paraItems)
        {
            int id = Int32.Parse(reader.GetAttribute("id", DocxConstants.W_namespace));

            WCommentMark commMark = new WCommentMark(m_doc, id, CommentMarkType.CommentEnd);
            if (paraItems != null && paraItems.OwnerBase != null)
                paraItems.Add(commMark);
            else
                m_postCommMark = commMark;

            if (m_commStack != null && m_commStack.Count > 0)
                m_commStack.Pop();
        }
        /// <summary>
        /// Updates the commented items.
        /// </summary>
        /// <param name="item">The item.</param>
        private void UpdateCommentItems(ParagraphItem item)
        {
            if (CurrentComment != null)
            {
                CurrentComment.CommentedItems.Add(item);
            }
        }
        #endregion Comments

        #region Table
        /// <summary>
        /// Parse the table
        /// </summary>
        /// <param name="reader">The XmlReader</param>
        /// <param name="table">The Table</param>
        private void ParseTable(XmlReader reader, WTable table)
        {
            if (reader.LocalName != "tbl")
                throw new XmlException("table element");

            if (reader.IsEmptyElement)
                return;

            if (table == null)
                throw new ArgumentException("table");

            bool skip = false;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != "tbl")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "tblPr":
                            ParseTableProperties(reader, table);
                            skip = true;
                            UpdateTableBorders(table.DocxTableFormat);
                            break;
                        case "tblGrid":
                            if (reader.IsEmptyElement)
                                break;
                            // skip to handle the second time defined grid for the same table.
                            if (table.TableGrid.Count > 0)
                                break;
                            table.TableGrid.Add(0f);
                            ParseTableGrid(reader, table, false);
                            break;
                        case "tr":
                            WTableRow tableRow = table.AddRow(false, false);
                            ApplyTableProperties(tableRow, table);
                            ParseTableRow(reader, tableRow);
                            break;
                        case "sdt":
                            StructureDocumentTagRow sdtRow = new StructureDocumentTagRow(table.Document);
                            ParseStructureDocumentTagRow(reader, sdtRow, table);
                            break;
                        case "bookmarkStart":
                            ParseBookmarkStart(reader, null);
                            break;
                        case "bookmarkEnd":
                            ParseBookmarkEnd(reader, table);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
            }
        }
        /// <summary>
        /// Parse the table row
        /// </summary>
        /// <param name="reader">The Xml reader</param>
        /// <param name="tableRow">The table row</param>
        private void ParseTableRow(XmlReader reader, WTableRow tableRow)
        {
            if (reader.LocalName != "tr")
                throw new XmlException("table row element");

            if (reader.IsEmptyElement)
                return;

            if (tableRow == null)
                throw new ArgumentException("table row");

            bool skip = false;
            m_gridCount = 0;
            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != "tr")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "trPr":
                            ParseTableRowProperties(reader, tableRow);
                            break;
                        case "tblPrEx":
                            tableRow.HasTblPrEx = true;
                            ParseTableProperties(reader, tableRow);
                            skip = true;
                            break;
                        case "sdt":
                            StructureDocumentTagCell sdtCell = new StructureDocumentTagCell(tableRow.Document);
                            ParseStructureDocumentTagCell(reader, sdtCell, tableRow);
                            break;
                        case "tc":
                            WTableCell tableCell = tableRow.AddCell(false);
                            ParseTableCell(reader, tableCell);
                            break;
                        case "bookmarkStart":
                            ParseBookmarkStart(reader, null);
                            break;
                        case "bookmarkEnd":
                            ParseBookmarkEnd(reader, tableRow);
                            break;
                        default:
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse Structure document tag cell
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdtCell"></param>
        /// <param name="tableRow"></param>
        private void ParseStructureDocumentTagCell(XmlReader reader, StructureDocumentTagCell sdtCell, WTableRow tableRow)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "sdtPr":
                            ParseSDTProperties(reader, sdtCell.SDTProperties);
                            break;
                        case "sdtContent":
                            ParseSDTCellContent(reader, sdtCell, tableRow);
                            break;
                        case "sdtEndPr":
                            ParseSDTEndCharacterFormat(reader, sdtCell.BreakCharacterFormat);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }

            }
        }
        /// <summary>
        /// Parse structure document tag cell content
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdtCell"></param>
        /// <param name="tableRow"></param>
        private void ParseSDTCellContent(XmlReader reader, StructureDocumentTagCell sdtCell, WTableRow tableRow)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            string endNode = reader.LocalName;

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "tc":
                            WTableCell tableCell = tableRow.AddCell(false);
                            ParseTableCell(reader, tableCell);
                            tableCell.SDTCell = sdtCell;
                            break;
                        case "sdt":
                            StructureDocumentTagCell sdtCell1 = new StructureDocumentTagCell(m_doc);
                            ParseStructureDocumentTagCell(reader, sdtCell1 as StructureDocumentTagCell, tableRow);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        private void ParseStructureDocumentTagRow(XmlReader reader, StructureDocumentTagRow sdtRow, WTable table)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            bool isSDTcontentFirst = true;
            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "sdtPr":
                            ParseSDTProperties(reader, sdtRow.SDTProperties);
                            break;
                        case "sdtContent":
                            ParseSDTRowContent(reader, sdtRow, table, ref isSDTcontentFirst);
                            break;
                        case "sdtEndPr":
                            ParseSDTEndCharacterFormat(reader, sdtRow.BreakCharacterFormat);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }

            }
        }
        /// <summary>
        /// Parses Structure document tag row content
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="sdtRow">StructureDocumentTagRow</param>
        /// <param name="table">WTable</param>
        /// <param name="isSDTcontentFirst"></param>
        private void ParseSDTRowContent(XmlReader reader, StructureDocumentTagRow sdtRow, WTable table, ref bool isSDTcontentFirst)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            string endNode = reader.LocalName;

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "tr":
                            WTableRow tableRow = table.AddRow(false, false);
                            ApplyTableProperties(tableRow, table);
                            ParseTableRow(reader, tableRow);
                            if (isSDTcontentFirst)
                            {
                                tableRow.SDTRow = sdtRow;
                                isSDTcontentFirst = false;
                            }
                            break;
                        case "sdt":
                            StructureDocumentTagRow sdtrow1 = new StructureDocumentTagRow(m_doc);
                            ParseStructureDocumentTagRow(reader, sdtrow1 as StructureDocumentTagRow, table);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse the table cell
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="tableCell">The table cell</param>
        private void ParseTableCell(XmlReader reader, WTableCell tableCell)
        {
            if (reader.LocalName != "tc")
                throw new XmlException("table cell element");

            if (reader.IsEmptyElement)
                return;

            if (tableCell == null)
                throw new ArgumentException("table cell");

            bool skip = false;
            reader.Read();
            SkipWhitespaces(reader);
            //Updates cell width from table grid.
            UpdateCellWidth(tableCell, 1);
            while (reader.LocalName != "tc")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "tcPr":
                            if (reader.IsEmptyElement)
                                break;
                            tableCell.CellFormat.Borders.IsDefault = false;
                            m_gridSpan = 1;
                            ParseCellProperties(reader, tableCell);
                            UpdateCellWidth(tableCell, m_gridSpan);
                            if (m_gridSpan > 1)
                            {
                                m_gridCount += m_gridSpan - 1;
                                m_gridSpan = 1;
                            }
                            break;
                        case "p":
                            IWParagraph paragraph = tableCell.AddParagraph();
                            AddPostElements(paragraph);
                            ParseParagraphItems(reader, paragraph.Items);
                            if (!String.IsNullOrEmpty(paragraph.StyleName))
                                paragraph.ApplyStyle(paragraph.StyleName);
                            break;
                        case "tbl":
                            WTable table = tableCell.AddTable() as WTable;
                            //Set IsAutoResized property as true to layout the table with Auto width (Default in DocX format document)
                            table.TableFormat.IsAutoResized = true;
                            int prevGridCount = m_gridCount;
                            ParseTable(reader, table);
                            m_gridCount = prevGridCount;
                            break;
                        case "bookmarkStart":
                            ParseBookmarkStart(reader, null);
                            break;
                        case "bookmarkEnd":
                            ParseBookmarkEnd(reader, tableCell);
                            break;
                        default:
                            break;

                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
            m_gridCount++;
        }
        /// <summary>
        /// Parse the table cell properties (CellFormat)
        /// </summary>
        /// <param name="reader">The Xml reader</param>
        /// <param name="tableCell">The table cell</param>
        private void ParseCellProperties(XmlReader reader, WTableCell tableCell)
        {
            if (reader.LocalName != "tcPr")
                throw new XmlException("table cell properties element");

            if (reader.IsEmptyElement)
                return;

            if (tableCell == null)
                throw new ArgumentException("table cell");

            bool skip = false;

            reader.Read();
            SkipWhitespaces(reader);

            CellFormat cellFormat = IsCellChangeFormat ? tableCell.TrackCellFormat : tableCell.CellFormat;

            while (reader.LocalName != "tcPr")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "noWrap":
                            cellFormat.TextWrap = !GetBooleanValue(reader);
                            break;
                        case "tcFitText":
                            cellFormat.FitText = GetBooleanValue(reader);
                            break;
                        case "tcW":
                            ParseCellWidth(reader, tableCell);
                            break;
                        case "textDirection":
                            ParseCellDirection(reader, cellFormat);
                            break;
                        case "vAlign":
                            cellFormat.VerticalAlignment = ParseCellVerticalAlignment(reader);
                            break;
                        case "vMerge":
                            ParseCellVerticalMerge(reader, cellFormat);
                            break;
                        case "hMerge":
                            ParseCellHorizontalMerge(reader, cellFormat);
                            break;
                        case "tcMar":
                            cellFormat.SamePaddingsAsTable = false;
                            ParseTableMargins(reader, tableCell);
                            break;
                        case "tcBorders":
                            ParseBorders(reader, tableCell);
                            break;
                        case "shd":
                            ParseCellShading(reader, tableCell);
                            break;
                        case "gridSpan":
                            if (!IsCellChangeFormat)
                                m_gridSpan = short.Parse(reader.GetAttribute("val", DocxConstants.W_namespace));
                            break;
                        case "tcPrChange":
                            IsCellChangeFormat = true;
                            reader.Read();
                            SkipWhitespaces(reader);
                            ParseCellProperties(reader, tableCell);
                            IsCellChangeFormat = false;
                            break;
                        case "cnfStyle":
                            break;
                        default:
                            cellFormat.XmlProps.Add(ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse the cell shadings
        /// </summary>
        /// <param name="reader">The xmlreader</param>
        /// <param name="cell">The table cell</param>
        private void ParseCellShading(XmlReader reader, WTableCell cell)
        {
            string value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (IsCellChangeFormat)
                cell.TrackCellFormat.TextureStyle = ParseTexture(value);
            else
                cell.CellFormat.TextureStyle = ParseTexture(value);

            string fill = reader.GetAttribute("fill", DocxConstants.W_namespace);
            Color backColor = Color.Empty;
            if (fill != "auto")
                backColor = GetColorValue(fill);

            if (IsCellChangeFormat)
                cell.TrackCellFormat.BackColor = backColor;
            else
                cell.CellFormat.BackColor = backColor;
            string color = reader.GetAttribute("color", DocxConstants.W_namespace);
            Color foreColor;
            if (color == "auto")
                foreColor = Color.Empty;
            else
                foreColor = GetColorValue(color);

            if (IsCellChangeFormat)
                cell.TrackCellFormat.ForeColor = foreColor;
            else
                cell.CellFormat.ForeColor = foreColor;
        }
        /// <summary>
        /// Parse the cell vertical Merge
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cellFormat"></param>
        private void ParseCellVerticalMerge(XmlReader reader, CellFormat cellFormat)
        {
            string merge = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (merge == "restart")
            {
                cellFormat.VerticalMerge = CellMerge.Start;
            }
            else
            {
                cellFormat.VerticalMerge = CellMerge.Continue;
            }
        }
        /// <summary>
        /// Parse the cell horizontal Merge
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cellFormat"></param>
        private void ParseCellHorizontalMerge(XmlReader reader, CellFormat cellFormat)
        {
            string merge = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (merge == "restart")
            {
                cellFormat.HorizontalMerge = CellMerge.Start;
            }
            else
            {
                cellFormat.HorizontalMerge = CellMerge.Continue;
            }
        }
        /// <summary>
        /// Parse the cell vertical alignment
        /// </summary>
        /// <param name="reader">The Xmlreader</param>
        /// <returns></returns>
        private VerticalAlignment ParseCellVerticalAlignment(XmlReader reader)
        {
            VerticalAlignment vAlign = VerticalAlignment.Top;
            string align = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (align != null)
            {
                switch (align)
                {
                    case "top":
                        vAlign = VerticalAlignment.Top;
                        break;
                    case "bottom":
                        vAlign = VerticalAlignment.Bottom;
                        break;
                    case "center":
                        vAlign = VerticalAlignment.Middle;
                        break;
                }
            }
            return vAlign;
        }
        /// <summary>
        /// Parse the text direction of the cell.
        /// </summary>
        /// <param name="reader">The xmlreader</param>
        /// <param name="cellFormat">The CellFormat</param>
        private void ParseCellDirection(XmlReader reader, CellFormat cellFormat)
        {
            string direction = reader.GetAttribute("val", DocxConstants.W_namespace);

            switch (direction)
            {
                case "tbRl":
                    cellFormat.TextDirection = TextDirection.VerticalTopToBottom;
                    break;
                case "btLr":
                    cellFormat.TextDirection = TextDirection.VerticalBottomToTop;
                    break;
            }
        }
        /// <summary>
        /// Parse the cell Width
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cell"></param>
        private void ParseCellWidth(XmlReader reader, WTableCell cell)
        {
            string type = reader.GetAttribute("type", DocxConstants.W_namespace);
            if (type == null)
                return;
            CellFormat cellFormat = IsCellChangeFormat ? cell.TrackCellFormat : cell.CellFormat;
            if (type == "auto")
            {
                cellFormat.PreferredWidth.WidthType = FtsWidth.Auto;
                UpdateCellWidth(cell);
            }
            else
            {
                string value = reader.GetAttribute("w", DocxConstants.W_namespace);

                if (type == "pct")
                {
                    cellFormat.PreferredWidth.WidthType = FtsWidth.Percentage;
                    cellFormat.PreferredWidth.Width = (float)ParseIntegerValue(value) / DLSConstants.PercentageFactor;
                }
                else if (type == "dxa")
                {
                    cellFormat.PreferredWidth.WidthType = FtsWidth.Point;
                    cellFormat.PreferredWidth.Width = (float)ParseIntegerValue(value) / DLSConstants.TwipsInOnePoint;
                }
            }
        }
        /// <summary>
        /// Updates the width of the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private void UpdateCellWidth(WTableCell cell)
        {
            if (cell.OwnerRow == null || cell.OwnerRow.OwnerTable == null)
                return;

            List<float> tableGrid = cell.OwnerRow.OwnerTable.TableGrid;
            float cellWidth = 0;

            if (tableGrid == null || tableGrid.Count == 0)
            {
                float width = cell.OwnerRow.OwnerTable.Width;
                if (width != 0)
                    cellWidth = width / cell.OwnerRow.Cells.Count;
            }
            else if (m_gridCount + 1 < tableGrid.Count)
            {
                cellWidth = tableGrid[m_gridCount + 1] - tableGrid[m_gridCount];
            }

            if (IsCellChangeFormat)
                cell.TrackCellFormat.CellWidth = cellWidth / DocxConstants.TwentiethOfPoint;
            else
                cell.CellFormat.CellWidth = cellWidth / DocxConstants.TwentiethOfPoint;
        }
        /// <summary>
        /// Updates the width of the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="gridSpan">The grid span.</param>
        private void UpdateCellWidth(WTableCell cell, short gridSpan)
        {
            if (cell.OwnerRow == null
                || cell.OwnerRow.OwnerTable == null
                || cell.OwnerRow.OwnerTable.TableGrid == null
                || cell.OwnerRow.OwnerTable.TableGrid.Count == 0)
                return;

            List<float> tableGrid = cell.OwnerRow.OwnerTable.TableGrid;
            float cellWidth = 0;
            if (m_gridCount + gridSpan < tableGrid.Count)
                cellWidth = tableGrid[m_gridCount + gridSpan] - tableGrid[m_gridCount];

            if (IsCellChangeFormat)
                cell.TrackCellFormat.CellWidth = cellWidth / DLSConstants.TwipsInOnePoint;
            else
                cell.CellFormat.CellWidth = cellWidth / DLSConstants.TwipsInOnePoint;
        }
        /// <summary>
        /// Parse the Table row properties
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="tableRow">The table row.</param>
        private void ParseTableRowProperties(XmlReader reader, WTableRow tableRow)
        {
            if (reader.LocalName != "trPr")
                throw new XmlException("table row element");

            if (reader.IsEmptyElement)
                return;

            if (tableRow == null)
                throw new ArgumentException("table row");

            bool skip = false;

            reader.Read();
            SkipWhitespaces(reader);
            RowFormat rowFormat = GetRowFormat(tableRow);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "trPr")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "trHeight":
                            ParseRowHeight(reader, tableRow);
                            break;
                        case "tblCellSpacing":
                            float spacing = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            string type = reader.GetAttribute("type", DocxConstants.W_namespace);
                            if (spacing != float.MaxValue && !(type != null && type == "nil"))
                                rowFormat.CellSpacing = spacing;
                            break;
                        case "tblHeader":
                            tableRow.IsHeader = true;
                            break;
                        case "trPrChange":
                            IsRowChangeFormat = true;
                            reader.Read();
                            SkipWhitespaces(reader);
                            ParseTableRowProperties(reader, tableRow);
                            IsRowChangeFormat = false;
                            break;
                        case "del":
                        case "moveFrom":
                            tableRow.IsDeleteRevision = true;
                            break;
                        case "ins":
                        case "moveTo":
                            tableRow.IsInsertRevision = true;
                            break;
                        case "cantSplit":
                            rowFormat.IsBreakAcrossPages = false;
                            break;
                        case "cnfStyle":
                            break;
                        case "gridBefore":
                            short value = short.Parse(GetStringVal(reader, "val", DocxConstants.W_namespace));
                            if (value > 0 && !IsRowChangeFormat)
                                m_gridCount = value;
                            break;
                        case "gridAfter":
                            //Grid after count will be preserved based on the grid after width property and hence skipped parsing of gridAfter.
                            break;
                        case "wBefore":
                            switch (reader.GetAttribute("type", DocxConstants.W_namespace))
                            {
                                case "pct":
                                    rowFormat.GridBeforeWidth.WidthType = FtsWidth.Percentage;
                                    rowFormat.GridBeforeWidth.Width = Int32.Parse(reader.GetAttribute("w", DocxConstants.W_namespace)) / DLSConstants.PercentageFactor;
                                    break;
                                case "dxa":
                                    rowFormat.GridBeforeWidth.WidthType = FtsWidth.Point;
                                    rowFormat.GridBeforeWidth.Width = Int32.Parse(reader.GetAttribute("w", DocxConstants.W_namespace)) / DLSConstants.TwipsInOnePoint;
                                    break;
                            }
                            break;
                        case "wAfter":
                            switch (reader.GetAttribute("type", DocxConstants.W_namespace))
                            {
                                case "pct":
                                    rowFormat.GridAfterWidth.WidthType = FtsWidth.Percentage;
                                    rowFormat.GridAfterWidth.Width = Int32.Parse(reader.GetAttribute("w", DocxConstants.W_namespace)) / DLSConstants.PercentageFactor;
                                    break;
                                case "dxa":
                                    rowFormat.GridAfterWidth.WidthType = FtsWidth.Point;
                                    rowFormat.GridAfterWidth.Width = Int32.Parse(reader.GetAttribute("w", DocxConstants.W_namespace)) / DLSConstants.TwipsInOnePoint;
                                    break;
                            }
                            break;
                        case "hidden":
                            rowFormat.Hidden = true;
                            break;
                        default:
                            if (reader.LocalName != string.Empty)
                            {
                                rowFormat.XmlProps.Add(ReadSingleNodeIntoStream(reader));
                                skip = true;
                            }
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <param name="attrNS">The attribute namespace.</param>
        /// <returns></returns>
        private string GetStringVal(XmlReader reader, string attrName, string attrNS)
        {
            if (reader.AttributeCount == 0)
                return null;

            if (attrNS == null)
            {
                return reader.GetAttribute(attrName);
            }
            else
            {
                return reader.GetAttribute(attrName, attrNS);
            }
        }
        /// <summary>
        /// Parse the table row height
        /// </summary>
        /// <param name="reader">The xmlreader</param>
        /// <param name="tableRow">The table row</param>
        private void ParseRowHeight(XmlReader reader, WTableRow tableRow)
        {
            float height = GetFloatValue(reader, "val", DocxConstants.W_namespace);
            if (height != float.MaxValue)
            {
                tableRow.Height = height;
            }
            string heightType = reader.GetAttribute("hRule", DocxConstants.W_namespace);
            if (heightType != null && heightType == "exact")
            {
                tableRow.HeightType = TableRowHeightType.Exactly;
            }
        }

        /// <summary>
        /// Applies table properties on table row.
        /// </summary>
        /// <param name="tblRow">The table row</param>
        /// <param name="tbl">The table</param>
        private void ApplyTableProperties(WTableRow tblRow, WTable table)
        {
            tblRow.RowFormat.ImportContainer(table.DocxTableFormat.Format);
            tblRow.RowFormat.IsAutoResized = table.DocxTableFormat.Format.IsAutoResized;
            if (table.DocxTableFormat.StyleName == null || table.DocxTableFormat.StyleName.Length == 0)
            {
                Borders borders = table.DocxTableFormat.Format.Borders;
                Borders rowBorders = tblRow.RowFormat.Borders;
                if (borders.Bottom.BorderType == BorderStyle.None)
                    rowBorders.Bottom.HasNoneStyle = true;
                if (borders.Left.BorderType == BorderStyle.None)
                    rowBorders.Left.HasNoneStyle = true;
                if (borders.Right.BorderType == BorderStyle.None)
                    rowBorders.Right.HasNoneStyle = true;
                if (borders.Top.BorderType == BorderStyle.None)
                    rowBorders.Top.HasNoneStyle = true;
                if (borders.Horizontal.BorderType == BorderStyle.None)
                    rowBorders.Horizontal.HasNoneStyle = true;
                if (borders.Vertical.BorderType == BorderStyle.None)
                    rowBorders.Vertical.HasNoneStyle = true;
            }
        }
        /// <summary>
        /// Updates the table borders.
        /// </summary>
        /// <param name="xmlFormat">The XML table format.</param>
        private void UpdateTableBorders(XmlTableFormat xmlFormat)
        {
            if (!string.IsNullOrEmpty(xmlFormat.StyleName))
                return;

            if (xmlFormat.Format.Borders.IsDefault)
            {
                xmlFormat.Format.Borders.BorderType = BorderStyle.None;
            }
        }
        /// <summary>
        /// Parse the table properties
        /// </summary>
        /// <param name="reader">The xmlreader</param>
        /// <param name="entity">The entity</param>
        private void ParseTableProperties(XmlReader reader, IEntity entity)
        {
            if (reader.IsEmptyElement)
                return;
            Stream stream = ReadSingleNodeIntoStream(reader);
            bool isStyleDefined = HasNode(stream, "tblStyle");// to handle the reading of table properties starting from style, check whether the style is defined for the table. 
            XmlReader tblprReader = UtilityMethods.CreateReader(stream);
            string endNode = tblprReader.LocalName;
            if (endNode == "tblPrEx")//parse the table propertis as it is, if the properties are defined in table property exchange element.
                isStyleDefined = false;
            RowFormat format = GetRowFormat(entity);
            bool skip = false;
            tblprReader.Read();
            SkipWhitespaces(tblprReader);
            WTable table = entity as WTable;
            if (entity is WTableRow)
            {
                table = (entity as WTableRow).OwnerTable;
            }
            while (tblprReader.NodeType != XmlNodeType.EndElement && tblprReader.LocalName != endNode)
            {
                skip = false;
                if (tblprReader.NodeType == XmlNodeType.Element)
                {
                    switch (tblprReader.LocalName)
                    {
                        case "tblStyle":
                            ParseTableStyle(tblprReader, table);
                            isStyleDefined = false;
                            break;
                        case "tblW":
                            string widthType = tblprReader.GetAttribute("type", DocxConstants.W_namespace);
                            if (widthType == null)
                                return;
                            if (isStyleDefined)
                                break;
                            if (widthType == "auto")
                                table.PreferredTableWidth.WidthType = FtsWidth.Auto;
                            else
                            {
                                string value = tblprReader.GetAttribute("w", DocxConstants.W_namespace);
                                if (widthType == "pct")
                                {
                                    table.PreferredTableWidth.WidthType = FtsWidth.Percentage;
                                    table.PreferredTableWidth.Width = (float)ParseIntegerValue(value) / DLSConstants.PercentageFactor;
                                }
                                else if (widthType == "dxa")
                                {
                                    table.PreferredTableWidth.WidthType = FtsWidth.Point;
                                    table.PreferredTableWidth.Width = (float)ParseIntegerValue(value) / DLSConstants.TwipsInOnePoint;
                                }
                            }
                            break;
                        case "tblBorders":
                            if (isStyleDefined)
                            {
                                tblprReader.Skip();
                                break;
                            }
                            ParseBorders(tblprReader, entity);
                            break;
                        case "tblCellSpacing":
                            if (isStyleDefined)
                                break;
                            float spacingValue = GetFloatValue(tblprReader, "w", DocxConstants.W_namespace);
                            string type = tblprReader.GetAttribute("type", DocxConstants.W_namespace);
                            if (spacingValue != float.MaxValue && !(type != null && type == "nil"))
                                format.CellSpacing = spacingValue;
                            break;
                        case "jc":
                            if (isStyleDefined)
                                break;
                            format.HorizontalAlignment = ParseTableJustification(tblprReader);
                            break;
                        case "tblCellMar":
                            if (isStyleDefined)
                            {
                                tblprReader.Skip();
                                break;
                            }
                            ParseTableMargins(tblprReader, entity);
                            break;
                        case "tblInd":
                            if (isStyleDefined)
                                break;
                            string indentValue = tblprReader.GetAttribute("w", DocxConstants.W_namespace);
                            format.LeftIndent = float.Parse(indentValue, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
                            if (endNode == "tblPrEx" && entity is WTableRow && (entity as WTableRow).GetRowIndex() == 0)
                                format.OwnerRow.OwnerTable.TableFormat.LeftIndent = format.LeftIndent;
                            break;
                        case "tblLayout":
                            if (isStyleDefined)
                                break;
                            ParseTableLayout(tblprReader, format);
                            break;
                        case "tblPrChange":
                        case "tblPrExChange":
                            if (isStyleDefined)
                            {
                                tblprReader.Skip();
                                break;
                            }
                            IsTableChangeFormat = true;
                            tblprReader.Read();
                            SkipWhitespaces(tblprReader);
                            ParseTableProperties(tblprReader, table);
                            skip = true;
                            IsTableChangeFormat = false;
                            break;
                        case "del":
                        case "moveFrom":
                            table.SetDeleteRev(true);
                            break;
                        case "ins":
                        case "moveTo":
                            table.SetInsertRev(true);
                            break;
                        case "tblpPr":
                            if (isStyleDefined)
                                break;
                            ParseTablePositioning(tblprReader, table);
                            break;
                        case "shd":
                            if (isStyleDefined)
                                break;
                            ParseTableShading(tblprReader, format);
                            break;
                        case "tblLook":
                            ParseTableLook(tblprReader, table);
                            break;
                        case "tblCaption":
                            ParseTableTitle(tblprReader, table);
                            break;
                        case "tblDescription":
                            ParseTableDescription(tblprReader, table);
                            break;
                        default:
                            if (tblprReader.LocalName != string.Empty && !(entity is WTableRow))
                            {
                                if (IsTableChangeFormat)
                                    table.TrackTblFormat.NodeArray.Add(ReadSingleNodeIntoStream(tblprReader));
                                else
                                    table.DocxTableFormat.NodeArray.Add(ReadSingleNodeIntoStream(tblprReader));
                                skip = true;
                            }
                            break;
                    }
                    if (!skip)
                        tblprReader.Read();
                }
                else
                    tblprReader.Read();
            }
        }
        /// <summary>
        /// Converts the string to its corresponding Integer value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int ParseIntegerValue(string value)
        {
            int intValue = 0;
            double result = 0.0;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                if (value.Contains("."))
                {
                    int index = value.IndexOf('.');
                    if (index > 0)
                        value = value.Substring(0, index);
                    else if (index == 0)
                        value = "0";
                }
                intValue = int.Parse(value, CultureInfo.InvariantCulture);
            }
            return intValue;
        }
        /// <summary>
        /// Parses the table title.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="table">The table.</param>
        private void ParseTableTitle(XmlReader reader, WTable table)
        {
            string value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (value != null)
                table.Title = value;
        }
        /// <summary>
        /// Parses the table description.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="table">The table.</param>
        private void ParseTableDescription(XmlReader reader, WTable table)
        {
            string value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (value != null)
                table.Description = value;
        }
        /// <summary>
        /// Parses the table look.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="table">The table.</param>
        private void ParseTableLook(XmlReader reader, WTable table)
        {
            string value = GetTableLookValue(reader, "firstRow");
            if (value != null)
                table.ApplyStyleForHeaderRow = GetBoolValue(value);

            value = GetTableLookValue(reader, "lastRow");
            if (value != null)
                table.ApplyStyleForLastRow = GetBoolValue(value);

            value = GetTableLookValue(reader, "firstColumn");
            if (value != null)
                table.ApplyStyleForFirstColumn = GetBoolValue(value);

            value = GetTableLookValue(reader, "lastColumn");
            if (value != null)
                table.ApplyStyleForLastColumn = GetBoolValue(value);

            value = GetTableLookValue(reader, "noHBand");
            if (value != null)
                table.ApplyStyleForBandedRows = !GetBoolValue(value);

            value = GetTableLookValue(reader, "noVBand");
            if (value != null)
                table.ApplyStyleForBandedColumns = !GetBoolValue(value);
        }
        /// <summary>
        /// Gets the bool value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool GetBoolValue(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "f" || value == "0" || value == "off" || value == "false")
                return false;
            else
                return true;
        }
        /// <summary>
        /// Gets the table look value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="name">The name.</param>
        private string GetTableLookValue(XmlReader reader, string name)
        {
            string value = reader.GetAttribute(name, DocxConstants.W_namespace);
            if (value == null)
            {
                string val = reader.GetAttribute("val", DocxConstants.W_namespace);
                if (val != null)
                {
                    string hexBinary = Convert.ToString(Convert.ToInt32(val, 16), 2).PadLeft(11, '0');
                    switch (name)
                    {
                        case "firstRow":
                            value = hexBinary.Substring(5, 1);
                            break;
                        case "lastRow":
                            value = hexBinary.Substring(4, 1);
                            break;
                        case "firstColumn":
                            value = hexBinary.Substring(3, 1);
                            break;
                        case "lastColumn":
                            value = hexBinary.Substring(2, 1);
                            break;
                        case "noHBand":
                            value = hexBinary.Substring(1, 1);
                            break;
                        case "noVBand":
                            value = hexBinary.Substring(0, 1);
                            break;
                    }
                }
            }
            return value;
        }
        /// <summary>
        /// Parse the table margins
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="paddings"></param>
        private void ParseTableMargins(XmlReader reader, Paddings paddings)
        {
            float val = float.MaxValue;

            if (reader.LocalName != "tblCellMar" && reader.LocalName != "tcMar")
                throw new XmlException("Table margins");

            string endElement = reader.LocalName;

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "top":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Top = val;
                            break;
                        case "left":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Left = val;
                            break;
                        case "bottom":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Bottom = val;
                            break;
                        case "right":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Right = val;
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse the table margins
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        private void ParseTableMargins(XmlReader reader, IEntity entity)
        {
            float val = float.MaxValue;
            Paddings paddings = GetPaddings(entity);

            if (reader.LocalName != "tblCellMar" && reader.LocalName != "tcMar")
                throw new XmlException("Table margins");

            string endElement = reader.LocalName;

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "top":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Top = val;
                            break;
                        case "left":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Left = val;
                            break;
                        case "bottom":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Bottom = val;
                            break;
                        case "right":
                            val = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            if (val != float.MaxValue) paddings.Right = val;
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Get the corresponding paddings wrt the entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The paddings</returns>
        private Paddings GetPaddings(IEntity entity)
        {
            Paddings paddings = null;
            if (entity is WTable)
            {
                if (IsTableChangeFormat)
                    paddings = (entity as WTable).TrackTblFormat.Format.Paddings;
                else
                    paddings = (entity as WTable).DocxTableFormat.Format.Paddings;
            }
            else if (entity is WTableRow)
            {
                if (IsRowChangeFormat)
                    paddings = (entity as WTableRow).TrackRowFormat.Paddings;
                else
                    paddings = (entity as WTableRow).RowFormat.Paddings;
            }
            else if (entity is WTableCell)
            {
                if (IsCellChangeFormat)
                    paddings = (entity as WTableCell).TrackCellFormat.Paddings;
                else
                    paddings = (entity as WTableCell).CellFormat.Paddings;
            }
            return paddings;
        }
        /// <summary>
        /// Parse the table justification
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private RowAlignment ParseTableJustification(XmlReader reader)
        {
            RowAlignment justification = RowAlignment.Left;
            string jc = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (jc != null)
            {
                switch (jc)
                {
                    case "left":
                        justification = RowAlignment.Left;
                        break;
                    case "center":
                        justification = RowAlignment.Center;
                        break;
                    case "right":
                        justification = RowAlignment.Right;
                        break;
                }
            }
            return justification;
        }
        /// <summary>
        /// Parses the table absolute positioning.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="table">The table.</param>
        private void ParseTablePositioning(XmlReader reader, WTable table)
        {
            RowFormat.TablePositioning positioning = table.DocxTableFormat.Format.Positioning;

            String value = reader.GetAttribute("leftFromText", DocxConstants.W_namespace);
            if (value != null)
            {
                positioning.DistanceFromLeft = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }

            value = reader.GetAttribute("rightFromText", DocxConstants.W_namespace);
            if (value != null)
            {
                positioning.DistanceFromRight = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }

            value = reader.GetAttribute("topFromText", DocxConstants.W_namespace);
            if (value != null)
            {
                positioning.DistanceFromTop = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }

            value = reader.GetAttribute("bottomFromText", DocxConstants.W_namespace);
            if (value != null)
            {
                positioning.DistanceFromBottom = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }

            value = reader.GetAttribute("vertAnchor", DocxConstants.W_namespace);
            if (value != null)
            {
                ParseTableVerticalRelation(positioning, value);
            }

            value = reader.GetAttribute("horzAnchor", DocxConstants.W_namespace);
            if (value != null)
            {
                ParseTableHorizontalRelation(positioning, value);
            }

            value = reader.GetAttribute("tblpXSpec", DocxConstants.W_namespace);
            if (value != null)
            {
                ParseTableHorizontalPosition(positioning, value);
            }

            value = reader.GetAttribute("tblpYSpec", DocxConstants.W_namespace);
            if (value != null)
            {
                ParseTableVerticalPosition(positioning, value);
            }

            value = reader.GetAttribute("tblpX", DocxConstants.W_namespace);
            if (value != null)
            {
                positioning.HorizPosition = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }

            value = reader.GetAttribute("tblpY", DocxConstants.W_namespace);
            if (value != null)
            {
                positioning.VertPosition = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }
        }
        /// <summary>
        /// Parses the tables' vertical relation.
        /// </summary>
        /// <param name="positioning">The positioning.</param>
        /// <param name="position">The position.</param>
        private void ParseTableVerticalRelation(RowFormat.TablePositioning positioning, string position)
        {
            switch (position)
            {
                case "page":
                    positioning.VertRelationTo = VerticalRelation.Page;
                    break;
                case "text":
                    positioning.VertRelationTo = VerticalRelation.Paragraph;
                    break;
                default:
                    positioning.VertRelationTo = VerticalRelation.Margin;
                    break;
            }
        }
        /// <summary>
        /// Parses the table vertical relation.
        /// </summary>
        /// <param name="positioning">The positioning.</param>
        /// <param name="position">The position.</param>
        private void ParseTableHorizontalRelation(RowFormat.TablePositioning positioning, string position)
        {
            switch (position)
            {
                case "page":
                    positioning.HorizRelationTo = HorizontalRelation.Page;
                    break;
                case "margin":
                    positioning.HorizRelationTo = HorizontalRelation.Margin;
                    break;
                default:
                    positioning.HorizRelationTo = HorizontalRelation.Column;
                    break;
            }
        }
        /// <summary>
        /// Parses the table absolute horizontal positioning.
        /// </summary>
        /// <param name="positioning">The positioning.</param>
        /// <param name="position">The position.</param>
        private void ParseTableHorizontalPosition(RowFormat.TablePositioning positioning, string position)
        {
            switch (position)
            {
                case "inside":
                    positioning.HorizPositionAbs = HorizontalPosition.Inside;
                    break;
                case "outside":
                    positioning.HorizPositionAbs = HorizontalPosition.Outside;
                    break;
                case "center":
                    positioning.HorizPositionAbs = HorizontalPosition.Center;
                    break;
                case "right":
                    positioning.HorizPositionAbs = HorizontalPosition.Right;
                    break;
                default:
                    positioning.HorizPositionAbs = HorizontalPosition.Left;
                    break;
            }
        }
        /// <summary>
        /// Parses the tables' absolute horizontal positioning.
        /// </summary>
        /// <param name="positioning">The positioning.</param>
        /// <param name="position">The position.</param>
        private void ParseTableVerticalPosition(RowFormat.TablePositioning positioning, string position)
        {
            switch (position)
            {
                case "inside":
                    positioning.VertPositionAbs = VerticalPosition.Inside;
                    break;
                case "outside":
                    positioning.VertPositionAbs = VerticalPosition.Outside;
                    break;
                case "center":
                    positioning.VertPositionAbs = VerticalPosition.Center;
                    break;
                case "bottom":
                    positioning.VertPositionAbs = VerticalPosition.Bottom;
                    break;
                case "top":
                    positioning.VertPositionAbs = VerticalPosition.Top;
                    break;
                default:
                    positioning.VertPositionAbs = VerticalPosition.None;
                    break;
            }
        }
        /// <summary>
        /// Parse the table shadings
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="format">The row format</param>
        private void ParseTableShading(XmlReader reader, RowFormat format)
        {
            string fill = reader.GetAttribute("fill", DocxConstants.W_namespace);
            string textureStyle = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (textureStyle != null)
            {
                format.TextureStyle = ParseTexture(textureStyle);
            }
            if (fill != null)
            {
                if (fill == "auto")
                    format.BackColor = Color.Empty;
                else
                    format.BackColor = GetColorValue(fill);
            }
        }
        /// <summary>
        /// Parse the table layout element
        /// </summary>
        /// <param name="reader">The xmlreader</param>
        /// <param name="format">The rowformat</param>
        private void ParseTableLayout(XmlReader reader, RowFormat format)
        {
            string layout = reader.GetAttribute("type", DocxConstants.W_namespace);
            if (layout == "fixed")
            {
                format.IsAutoResized = false;
            }
        }
        /// <summary>
        /// Parse the table style element
        /// </summary>
        /// <param name="reader">The xmlreader</param>
        /// <param name="table">The table</param>
        private void ParseTableStyle(XmlReader reader, WTable table)
        {
            string styleNameId = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (!string.IsNullOrEmpty(styleNameId)
                && StyleNameId.ContainsKey(styleNameId)
                && m_doc.Styles.FindByName(StyleNameId[styleNameId], StyleType.TableStyle) != null)
                table.ApplyStyle(StyleNameId[styleNameId]);
            if (IsTableChangeFormat)
                table.TrackTblFormat.StyleName = styleNameId;
            else
                table.DocxTableFormat.StyleName = styleNameId;
        }
        /// <summary>
        /// Get the corresponding row format based on the entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns></returns>
        private RowFormat GetRowFormat(IEntity entity)
        {
            RowFormat format = null;
            WTable table = null;
            if (entity is WTable)
            {
                table = entity as WTable;
                if (IsTableChangeFormat)
                {
                    format = table.TrackTblFormat.Format;
                }
                else
                {
                    format = table.DocxTableFormat.Format;
                    if (table.DocxTableFormat.StyleName == null)
                        table.DocxTableFormat.StyleName = string.Empty;
                }
            }
            else if (entity is WTableRow)
            {
                table = (entity as WTableRow).OwnerTable;
                if (IsRowChangeFormat)
                    format = (entity as WTableRow).TrackRowFormat;
                else
                    format = (entity as WTableRow).RowFormat;
            }
            return format;
        }
        /// <summary>
        /// Parse the table grid element
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="table">The table</param>
        /// <param name="isTableGridChange"></param>
        private void ParseTableGrid(XmlReader reader, WTable table, bool isTableGridChange)
        {
            if (!(reader.LocalName == "tblGrid"
                || reader.LocalName == "tblGridChange"))
                throw new XmlException("table grid");
            string endElement = reader.LocalName;
            bool skip = false;
            List<float> grid = (isTableGridChange == true) ? table.TrackTableGrid : table.TableGrid;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endElement)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "gridCol":
                            string width = reader.GetAttribute("w", DocxConstants.W_namespace);
                            if (string.IsNullOrEmpty(width))
                            {
                                table.m_isTableGridCorrupted = true;
                                break;
                            }
                            float lastWidth = grid[grid.Count - 1];
                            grid.Add(float.Parse(width, CultureInfo.InvariantCulture) + lastWidth);
                            break;
                        case "tblGridChange":
                            table.TrackTableGrid.Add(0f);
                            ParseTableGrid(reader, table, true);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Add a table to the corresponding the textbody
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IWTable AddTable(IEntity entity)
        {
            IWTable table = null;

            if (entity is HeaderFooter)
            {
                table = (entity as HeaderFooter).AddTable();
            }
            else if (entity is WFootnote)
            {
                table = (entity as WFootnote).TextBody.AddTable();
            }
            else if (entity is WComment)
            {
                table = (entity as WComment).TextBody.AddTable();
            }
            else if (entity is StructureDocumentTagBlock)
            {
                table = (entity as StructureDocumentTagBlock).SDTContent.TextBody.AddTable();
            }
            else if (entity is WTextBody)
            {
                table = (entity as WTextBody).AddTable();
            }
            else
            {
                table = m_doc.LastSection.AddTable();
            }

            return table;
        }
        #endregion Table

        #region Hyperlink
        /// <summary>
        /// Parse the Hyperlink
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="para">The paragraph element</param>
        private void ParseHyperlink(XmlReader reader, ParagraphItemCollection paraItems)
        {
            string id = reader.GetAttribute("id", DocxConstants.R_namespace);
            string anchor = reader.GetAttribute("anchor", DocxConstants.W_namespace);

            if (id == null && anchor == null)
                return;

            WField field = new WField(m_doc);
            field.FieldType = FieldType.FieldHyperlink;
            Hyperlink hyperlink = new Hyperlink(field);

            if (id == null)
            {
                hyperlink.Type = HyperlinkType.Bookmark;
                hyperlink.BookmarkName = anchor;
                AddToParagraph(field, paraItems);
            }
            else
            {
                DictionaryEntry entry = GetHyperlinkRelation(id);

                if (entry.Key == null || entry.Key.ToString() != DocxConstants.HyperlinkRelType)
                    return;

                AddToParagraph(field, paraItems);
                string targetString = (string)entry.Value;

                if (targetString.StartsWith("#"))
                {
                    hyperlink.Type = HyperlinkType.Bookmark;
                    hyperlink.BookmarkName = targetString.Replace("#", string.Empty);
                }
                else
                {
                    field.m_fieldValue = "\"" + targetString.Replace(@"\", @"\\") + "\"";
                    if (m_isExternalHyperlink != null && m_docRelations.ContainsKey(id))
                    {
                        bool isNotLocal = IsExternalHyperlink[id];

                        if (isNotLocal)
                        {
                            if (anchor != null)
                            {
                                field.IsLocal = true;
                                field.LocalReference = anchor;
                            }
                        }
                        else
                            field.IsLocal = true;
                    }
                }
            }

            WFieldMark separ = new WFieldMark(m_doc, FieldMarkType.FieldSeparator);
            AddToParagraph(separ, paraItems);
            field.FieldSeparator = separ;

            if (reader.LocalName.ToLower() == "hyperlink")
            {
                ParseHyperlinkText(reader, paraItems);
                WFieldMark fieldEnd = new WFieldMark(m_doc, FieldMarkType.FieldEnd);
                AddToParagraph(fieldEnd, paraItems);
                field.FieldEnd = fieldEnd;
            }
        }
        /// <summary>
        /// Get the hyperlink relation
        /// </summary>
        /// <param name="id">The relation id</param>
        /// <returns></returns>
        private DictionaryEntry GetHyperlinkRelation(string id)
        {
            DictionaryEntry entry = new DictionaryEntry();

            if (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer") || m_currentFile.StartsWith("comments")
                || m_currentFile.StartsWith("footnotes") || m_currentFile.StartsWith("endnotes"))
            {
                string fileName = string.Empty;
                if (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer"))
                    fileName = m_currentFile;
                else if (m_currentFile.StartsWith("comments"))
                    fileName = "comments.xml.rels";
                else if (m_currentFile.StartsWith("footnotes"))
                    fileName = "footnotes.xml.rels";
                else if (m_currentFile.StartsWith("endnotes"))
                    fileName = "endnotes.xml.rels";

                Dictionary<string, DictionaryEntry> rel = GetFileRelations(fileName);

                if (rel != null && rel.ContainsKey(id))
                    entry = (DictionaryEntry)rel[id];
            }
            else if (m_docRelations.ContainsKey(id))
            {
                entry = m_docRelations[id];
            }
            return entry;
        }
        /// <summary>
        /// Parses the hyperlink text.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="paragraph">The paragraph</param>
        private void ParseHyperlinkText(XmlReader reader, ParagraphItemCollection paraItems)
        {
            reader.MoveToElement();

            if (reader.LocalName != "hyperlink")
                throw new XmlException("hyperlink");

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);
            m_isInHeyperlinkField = true;
            while (reader.LocalName != "hyperlink")
            {
                ParseRun(reader, paraItems);
                reader.Read();
                SkipWhitespaces(reader);
            }
            m_currentRunFormat = null;
            m_isInHeyperlinkField = false;
        }
        #endregion Hyperlink

        #region FieldSimple
        /// <summary>
        /// Parse the Field Simple
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="paragraph">The paragraph</param>
        /// <returns></returns>
        private bool ParseFieldSimple(XmlReader reader, ParagraphItemCollection paraItems)
        {
            string instr = reader.GetAttribute("instr", DocxConstants.W_namespace);

            if (string.IsNullOrEmpty(instr))
                instr = ModifyText(reader.ReadInnerXml());

            WField field = GetFieldType(instr);
            field.ParseFieldCode(field.FieldCode);
            if (m_currentRunFormat != null)
            {
                field.ApplyCharacterFormat(m_currentRunFormat);
            }
            bool isTOC = field.FieldType == FieldType.FieldTOC && m_doc.TOC == null;
            if (isTOC)
            {
                TableOfContent toc = new TableOfContent(m_doc, field.FormattingString);
                m_doc.TOC = toc;
                toc.FormattingString = field.FormattingString;
                AddItem(toc, paraItems);
            }
            else
                AddItem(field, paraItems);
            if (reader.IsEmptyElement)
            {
                WFieldMark fieldMark = new WFieldMark(m_doc, FieldMarkType.FieldSeparator);
                AddItem(fieldMark, paraItems);
                fieldMark = new WFieldMark(m_doc, FieldMarkType.FieldEnd);
                AddItem(fieldMark, paraItems);
                field.FieldEnd = fieldMark;
                return false;
            }

            if (field.FieldType == FieldType.FieldMergeField)
            {
                FieldStack.Push(field);
                m_currentFldCharType = FieldCharType.SimpleField;
                ParseParagraphItems(reader, paraItems);
                FieldStack.Pop();
                m_currentFldCharType = FieldCharType.Unknown;
                return true;
            }
            else if (field.FieldType == FieldType.FieldNext)
            {
                WParagraph text = new WParagraph(m_doc);
                FieldStack.Push(field);
                m_currentFldCharType = FieldCharType.SimpleField;
                ParseParagraphItems(reader, paraItems);
                field.m_formattingString = text.Text;
                field.Text = text.Text;
                FieldStack.Pop();
                m_currentFldCharType = FieldCharType.Unknown;
                return true;
            }
            else
            {
                WFieldMark fieldMark = new WFieldMark(m_doc, FieldMarkType.FieldSeparator);
                AddItem(fieldMark, paraItems);
                if (isTOC)
                    m_doc.TOC.TOCField.FieldSeparator = fieldMark;
                else
                    field.FieldSeparator = fieldMark;
                ParseParagraphItems(reader, paraItems);
                fieldMark = new WFieldMark(m_doc, FieldMarkType.FieldEnd);
                AddItem(fieldMark, paraItems);
                if (isTOC)
                    m_doc.TOC.TOCField.FieldEnd = fieldMark;
                else
                    field.FieldEnd = fieldMark;
                return true;
            }
            return false;
        }
        #endregion FieldSimple

        #region Bookmark
        /// <summary>
        /// Get the particular bookmark name by its ID from BookmarkInfo collection
        /// </summary>
        /// <param name="reader">The bookmark id.</param>
        /// <returns>The bookmark name</returns>
        private string GetBookmarkName(string bookmarkId)
        {
            for (int i = 0; i < BookmarkNames.Count; i++)
            {
                if (BookmarkNames[i].bookmarkId.Equals(bookmarkId))
                    return BookmarkNames[i].bookmarName;
            }
            return null;
        }
        /// <summary>
        /// Removes the particular bookmark name by its ID from BookmarkInfo collection
        /// </summary>
        /// <param name="reader">The bookmark id.</param>     
        private bool RemoveBookmarkName(string bookmarkId)
        {
            for (int i = 0; i < BookmarkNames.Count; i++)
            {
                if (BookmarkNames[i].bookmarkId.Equals(bookmarkId))
                {
                    BookmarkNames.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the bookmark end.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="paraItems">The para items.</param>
        private void ParseBookmarkEnd(XmlReader reader, ParagraphItemCollection paraItems)
        {
            string bookmarkID = reader.GetAttribute("id", DocxConstants.W_namespace);
            string bookmarkName = GetBookmarkName(bookmarkID);
            if (bookmarkName != null)
            {
                BookmarkEnd bkmk = new BookmarkEnd(m_doc, bookmarkName);
                paraItems.Add(bkmk);
                //Remove the bookmark name from bookmark name collection after inserting the bookmark end
                RemoveBookmarkName(bookmarkID);
            }
        }
        /// <summary>
        /// Parse the bookmark end element
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="ent">The entity</param>
        private void ParseBookmarkEnd(XmlReader reader, IEntity ent)
        {
            string bookmarkID = reader.GetAttribute("id", DocxConstants.W_namespace);
            string bookmarkName = GetBookmarkName(bookmarkID);
            if (bookmarkName != null)
            {
                WParagraph para = null;

                if (ent is WParagraph)
                {
                    para = ent as WParagraph;
                }
                else if (ent == null)
                {
                    para = m_doc.LastParagraph;
                }
                else if (ent is WTable && (ent as WTable).OwnerTextBody != null)
                {
                    WTableRow row = (ent as WTable).LastRow;
                    if (row != null)
                        para = GetBookmarkParagraph(row);
                    else
                        para = (ent as WTable).OwnerTextBody.AddParagraph() as WParagraph;
                }
                else if (ent is WTableCell)
                {
                    para = GetBookmarkParagraph(ent as WTableCell);
                }
                else if (ent is WTableRow)
                {
                    para = GetBookmarkParagraph(ent as WTableRow);
                }

                if (para != null)
                {
                    para.AppendBookmarkEnd(bookmarkName);
                    //Remove the bookmark name from bookmark name collection after inserting the bookmark end
                    RemoveBookmarkName(bookmarkID);
                }
                else
                {
                    BookmarkEnd bookmarkEnd = new BookmarkEnd(m_doc, bookmarkName);
                    m_postBkmk = bookmarkEnd;
                }
            }
        }
        /// <summary>
        /// Gets the paragraph which contains bookmark end.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        private WParagraph GetBookmarkParagraph(WTableCell cell)
        {
            WParagraph para = null;

            if (cell.Items != null && cell.Items.Count > 0)
                para = cell.Items[cell.Items.Count - 1] as WParagraph;

            return para;
        }
        /// <summary>
        /// Gets the paragraph which contains bookmark end.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        private WParagraph GetBookmarkParagraph(WTableRow row)
        {
            WParagraph para = null;
            if (row.Cells.Count > 0)
            {
                para = GetBookmarkParagraph(row.Cells[row.Cells.Count - 1]);
            }

            return para;
        }
        /// <summary>
        /// Parse the bookmark start element
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="paragraph">The paragraph</param>
        private void ParseBookmarkStart(XmlReader reader, ParagraphItemCollection paraItems)
        {
            string name = reader.GetAttribute("name", DocxConstants.W_namespace);
            string id = reader.GetAttribute("id", DocxConstants.W_namespace);
            BookmarkStart bookmarkStart = new BookmarkStart(m_doc, name);
            // Check column start index.
            string colIndex = reader.GetAttribute("colFirst", DocxConstants.W_namespace);
            if (!string.IsNullOrEmpty(colIndex))
                bookmarkStart.ColumnFirst = Int32.Parse(colIndex);

            // Check column end index.
            colIndex = reader.GetAttribute("colLast", DocxConstants.W_namespace);
            if (!string.IsNullOrEmpty(colIndex))
                bookmarkStart.ColumnLast = Int32.Parse(colIndex);

            BookmarkInfo bookmarkInfo = new BookmarkInfo();
            bookmarkInfo.bookmarName = name;
            bookmarkInfo.bookmarkId = id;
            BookmarkNames.Add(bookmarkInfo);

            if (paraItems == null)
            {
                m_postBkmk = bookmarkStart;
            }
            else
            {
                paraItems.Add(bookmarkStart);
            }
        }
        #endregion Bookmark

        #region ParagraphItems
        /// <summary>
        /// Parse the run element
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="paragraph">The paragraph</param>
        private void ParseRun(XmlReader reader, ParagraphItemCollection paraItems)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            bool isAlternateContent = false;
            bool isChoice = false;
            bool isFallback = false;
            ParagraphItem choiceItem = null;

            reader.Read();
            MemoryStream drawingStream = null;
            SkipWhitespaces(reader);
            while (!(reader.LocalName == "r" && reader.NodeType == XmlNodeType.EndElement))
            {
                ParagraphItem item = null;
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "r":
                            WCharacterFormat prevRunFormat = m_currentRunFormat;
                            ParseRun(reader, paraItems);
                            m_currentRunFormat = prevRunFormat;
                            break;
                        case DocxConstants.c_characterFormatTag:
                            WParagraph para = paraItems.OwnerBase as WParagraph;
                            if (paraItems.Owner is SDTInlineContent)
                                para = paraItems.Owner.Owner.Owner as WParagraph;
                            m_currentRunFormat = new WCharacterFormat(m_doc);
                            if (para != null
                                && para.ParaStyle != null)
                                m_currentRunFormat.ApplyBase(para.ParaStyle.CharacterFormat);
                            ParseCharacterFormat(reader, m_currentRunFormat);
                            break;
                        case "delText":
                        case "t":
                            item = ParseText(reader, paraItems);
                            break;
                        case "drawing":
                            if (isAlternateContent && isChoice) //Parse choice 
                            {
                                choiceItem = ParseDrawing(reader, paraItems, ref drawingStream);
                                item = choiceItem;
                                if (!(item is Shape) || (item is Shape &&
                                    (item as Shape).AutoShapeType == AutoShapeType.Rectangle
                                    && (item as Shape).TextBody.Count > 0))
                                {
                                    item = null;
                                    choiceItem = null;
                                }
                            }
                            else
                                item = ParseDrawing(reader, paraItems, ref drawingStream);
                            skip = true;
                            break;
                        case "object":
                            item = ParseObject(reader);
                            skip = true;
                            break;
                        case "pict":
                            MemoryStream shapeStream = ReadSingleNodeIntoStream(reader);
                            item = ParseShape(reader, paraItems, drawingStream, shapeStream);
                            
                            if (isAlternateContent && isChoice && (item is WTextBox) 
                                && (choiceItem is XmlParagraphItem))
                            {
                               ParseTextBoxGraphics((item as WTextBox),(choiceItem as XmlParagraphItem));
                            }

                            if (isFallback && choiceItem != null && (choiceItem is Shape))
                            {
                                ImportDocxPropsAndXMLRelation(choiceItem, item, shapeStream);
                                item = null;
                            }
                            skip = true;
                            drawingStream = null;
                            break;
                        case "br":
                        case "cr":
                            ParseBreak(reader, paraItems);
                            break;
                        case "fldChar":
                            ParseFieldMark(reader, paraItems);
                            skip = true;
                            break;
                        case "delInstrText":
                        case "instrText":
                            ParseFieldValue(reader, paraItems);
                            m_isPrevItemFieldStart = false;
#if SILVERLIGHT || WP
                            skip = true;
#endif
                            break;
                        case "footnoteReference":
                        case "endnoteReference":
                            WCharacterFormat charFormat = null;
                            if (m_currentRunFormat != null)
                                charFormat = m_currentRunFormat;
                            item = ParseFootnote(reader);
                            if (charFormat != null)
                                (item as WFootnote).MarkerCharacterFormat.ImportContainer(charFormat);
                            break;
                        case "footnoteRef":
                        case "endnoteRef":
                            item = ParseFootnoteMarker();
                            break;
                        case "tab":
                            item = new WTextRange(m_doc);
                            string tabCharacter = '\t'.ToString();
                            UpdateTextRange(item as WTextRange, tabCharacter, m_currentRunFormat);
                            break;
                        case "sym":
                            item = ParseSymbol(reader, paraItems);
                            break;
                        case "noBreakHyphen":
                            item = new WTextRange(m_doc);
                            UpdateTextRange(item as WTextRange, NONBREAK_HYPHEN.ToString(), m_currentRunFormat);
                            break;
                        case "softHyphen":
                            item = new WTextRange(m_doc);
                            UpdateTextRange(item as WTextRange, SOFT_HYPHEN.ToString(), m_currentRunFormat);
                            break;
                        case "commentReference":
                            string id = reader.GetAttribute("id", DocxConstants.W_namespace);
                            if (Comments.ContainsKey(id))
                                item = Comments[id];
                            break;
                        case "AlternateContent":
                            isAlternateContent = true;
                            break;
                        case "Choice":
                            if (isAlternateContent)
                                isChoice = true;
                            break;
                        case "Fallback":
                            if (isAlternateContent)
                                isFallback = true;
                            break;
                        case "ptab":
                            item = ParseAbsoluteTab(reader);
                            break;
                        case "separator":
                            item = new WTextRange(m_doc);
                            UpdateTextRange(item as WTextRange, SpecialCharacters.Separator.ToString(), m_currentRunFormat);
                            (item as WTextRange).CharacterFormat.Special = true;
                            break;
                        case "continuationSeparator":
                            item = new WTextRange(m_doc);
                            UpdateTextRange(item as WTextRange, SpecialCharacters.ContinuationSeparator.ToString(), m_currentRunFormat);
                            (item as WTextRange).CharacterFormat.Special = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else if (reader.LocalName == "AlternateContent" && reader.NodeType == XmlNodeType.EndElement)
                {
                    isAlternateContent = false;
                    isChoice = false;
                    choiceItem = null;
                    isFallback = false;
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }

                SkipWhitespaces(reader);

                if (item != null)
                {
                    AddToParagraph(item, paraItems);
                    if (item is WPicture)
                        AppendFieldEndToImageHyperlink(item, paraItems);
                    CheckTrackChange(item);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choiceItem"></param>
        /// <param name="item"></param>
        /// <param name="shapeStream"></param>
        private void ImportDocxPropsAndXMLRelation(ParagraphItem choiceItem, ParagraphItem item, Stream shapeStream)
        {
            Shape choice = choiceItem as Shape;
            choice.DocxProps.Add("pict", shapeStream);
            if (item is Shape)
            {
                Shape fallback = item as Shape;
                choice.Docx2007Props = fallback.Docx2007Props;
                //In some cases line and fill format color is defined in the style reference of the choice content. 
                //Since we preserve style tag directly, we are not able to retreive the color value for the line and fill format
                //As a work around, we retreiving the color value from fallback content
                if (choice.LineFormat.Color.IsEmpty && !fallback.LineFormat.Color.IsEmpty)
                    choice.LineFormat.Color = fallback.LineFormat.Color;
                if (choice.FillFormat.Color.IsEmpty && !fallback.FillFormat.Color.IsEmpty)
                    choice.FillFormat.Color = fallback.FillFormat.Color;
                if (choice.FillFormat.ForeColor.IsEmpty && !fallback.FillFormat.ForeColor.IsEmpty)
                    choice.FillFormat.ForeColor = fallback.FillFormat.ForeColor;
                string[] keys = new string[fallback.ImageRelations.Count];

                (item as Shape).ImageRelations.Keys.CopyTo(keys, 0);
                for (int i = 0; i < keys.Length; i++)
                    choice.ImageRelations.Add(keys[i], fallback.ImageRelations[keys[i]]);

                keys = new string[fallback.Relations.Count];

                fallback.Relations.Keys.CopyTo(keys, 0);
                for (int i = 0; i < keys.Length; i++)
                    choice.Relations.Add(keys[i], fallback.Relations[keys[i]]);
            }
        }
        #endregion ParagraphItems

        #region Absolute Tab
        private ParagraphItem ParseAbsoluteTab(XmlReader reader)
        {
            string relativeValue = reader.GetAttribute("relativeTo", DocxConstants.W_namespace);
            string alignmentValue = reader.GetAttribute("alignment", DocxConstants.W_namespace);
            string leaderValue = reader.GetAttribute("leader", DocxConstants.W_namespace);
            WAbsoluteTab absoluteTab = new WAbsoluteTab(m_doc);
            switch (relativeValue)
            {
                case "margin":
                    absoluteTab.Relation = AbsoluteTabRelation.Margin;
                    break;
                case "indent":
                    absoluteTab.Relation = AbsoluteTabRelation.Indent;
                    break;
            }
            switch (alignmentValue)
            {
                case "left":
                    absoluteTab.Alignment = AbsoluteTabAlignment.Left;
                    break;
                case "right":
                    absoluteTab.Alignment = AbsoluteTabAlignment.Right;
                    break;
                case "center":
                    absoluteTab.Alignment = AbsoluteTabAlignment.Center;
                    break;
            }
            absoluteTab.TabLeader = GetTabLeader(leaderValue);
            absoluteTab.CharacterFormat = m_currentRunFormat;
            return absoluteTab;
        }
        #endregion

        #region Fields
        /// <summary>
        /// Parse the field value
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="paragraph">The paragraph</param>
        private void ParseFieldValue(XmlReader reader, ParagraphItemCollection paraItems)
        {
#if !SILVERLIGHT && !WP
            string fieldValue = reader.ReadString();
#else
            string fieldValue = reader.ReadInnerXml();
#endif

            if (fieldValue == null || fieldValue == string.Empty)
                return;

            WField field = GetFieldType(fieldValue);

            if (!m_isPrevItemFieldStart
                && CurrentField != null
                && (!IsFormField()
                || field != CurrentField))
            {
                ParagraphItem lastItem = paraItems.LastItem as ParagraphItem;
                int index = -1;
                if (CurrentField != null
                    && CurrentField.NextSibling == null)
                {
                    lastItem = CurrentField as ParagraphItem;
                    index = paraItems.IndexOf(CurrentField);
                }
                if ((m_currentFldCharType == FieldCharType.Seperate
                    || m_currentFldCharType == FieldCharType.SimpleField)
                    && lastItem is WMergeField)
                {
                    CurrentField.Text += fieldValue;
                    if (m_currentRunFormat != null)
                        CurrentField.ApplyCharacterFormat(m_currentRunFormat);
                }
                else if (CurrentField != null
                    && (index >= 0
                    && index == paraItems.Count - 1
                    || IsFormField()
                    || CurrentField is WMergeField
                    || CurrentField.FieldType == FieldType.FieldDate)
                    && m_currentFldCharType != FieldCharType.Seperate)
                {
                    CurrentField.FieldCode += fieldValue;
                    if (CurrentField.FieldType == FieldType.FieldUnknown)
                    {
                        WField curField = FieldStack.Pop();
                        paraItems.Remove(curField);
                        curField = GetFieldType(curField.FieldCode);
                        if (m_currentRunFormat != null)
                            curField.ApplyCharacterFormat(m_currentRunFormat);
                        paraItems.Add(curField);
                        if (CurrentField != curField)
                            FieldStack.Push(curField);
                    }
                    else if (IsFormField())
                        paraItems.Add(CurrentField);
                }
                else
                {
                    WTextRange item = new WTextRange(m_doc);
                    if (m_currentRunFormat != null)
                        item.ApplyCharacterFormat(m_currentRunFormat);
                    item.Text = fieldValue;
                    AddToParagraph(item, paraItems);
                }
                if (field.FieldType == FieldType.FieldHyperlink)
                    m_isInHeyperlinkField = true;
                return;
            }

            if (field.FieldType == FieldType.FieldMergeField)
            {
                m_fieldInstrText.Remove(0, m_fieldInstrText.Length);
                if (m_currentRunFormat != null)
                    field.ApplyCharacterFormat(m_currentRunFormat);
                AddToParagraph(field, paraItems);
                FieldStack.Push(field);
                return;
            }
            else
            {
                field.FieldCode = fieldValue;
            }



            m_fieldInstrText.Remove(0, m_fieldInstrText.Length);

            if (m_currentRunFormat != null)
            {
                field.ApplyCharacterFormat(m_currentRunFormat);
            }

            if (field.FieldType == FieldType.FieldHyperlink)
                m_isInHeyperlinkField = true;
            if (!m_isPrevItemFieldStart && !IsFormField())
            {
                WTextRange textRange = new WTextRange(m_doc);
                textRange.ApplyCharacterFormat(field.CharacterFormat);
                textRange.Text = fieldValue;
                AddToParagraph(textRange, paraItems);
            }
            else
            {
                AddToParagraph(field, paraItems);
                CheckTrackChange(field);
                if (!(field is WFormField))
                    FieldStack.Push(field);
            }
        }
        /// <summary>
        /// Parse the field mark element
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="entity">The entity</param>
        private void ParseFieldMark(XmlReader reader, ParagraphItemCollection paraItems)
        {
            string type = reader.GetAttribute("fldCharType", DocxConstants.W_namespace);
            if (type == null)
                return;

            Stream fieldNode = ReadSingleNodeIntoStream(reader);

            if (HasFFData(fieldNode))
            {
                XmlReader fldReader = UtilityMethods.CreateReader(fieldNode);
                ParseFieldData(fldReader, paraItems);
            }
            else
            {
                switch (type)
                {
                    case "begin":
                        InitFieldMarkBegin();
                        break;
                    case "separate":
                        ProcessFieldMarkSeperator(paraItems);
                        break;
                    case "end":
                        ProcessFieldMarkEnd(paraItems);
                        break;
                }
            }
        }
        /// <summary>
        /// Check whether ffData element is present as field mark child elements
        /// </summary>
        /// <param name="fieldNode"></param>
        /// <returns></returns>
        private bool HasFFData(Stream fieldNode)
        {
            XmlReader reader = UtilityMethods.CreateReader(fieldNode);
            while (reader.Read())
            {
                if (reader.LocalName == "ffData")
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Process the Field mark seperator
        /// </summary>
        /// <param name="para">The paragraph</param>
        private void ProcessFieldMarkSeperator(ParagraphItemCollection paraItems)
        {
            m_isPrevItemFieldStart = false;
            m_currentFldCharType = FieldCharType.Seperate;
            ParagraphItem lastItem = paraItems.LastItem as ParagraphItem;
            if (CurrentField != null)
                lastItem = CurrentField as ParagraphItem;
            if ((lastItem is WField
                || lastItem is WTextRange)
                && !(lastItem is WMergeField))
            {
                bool isTOC = (lastItem is WField) ? (lastItem as WField).FieldType == FieldType.FieldTOC : false;
                bool isTextRange = (lastItem is WTextRange) ? true : false;
                string fieldInstrText = m_fieldInstrText.ToString();

                if (isTOC && m_doc.TOC == null)
                {
                    if (lastItem.Owner == null)
                        return;
                    (lastItem as WField).ParseFieldCode((lastItem as WField).FieldCode);
                    TableOfContent toc = new TableOfContent(m_doc, CurrentField.FormattingString);
                    m_doc.TOC = toc;
                    toc.FormattingString = CurrentField.FormattingString;
                    paraItems.Remove(lastItem);
                    AddToParagraph(toc, paraItems);
                }
                else if (!isTextRange)
                {
                    WField field = lastItem as WField;
                    if (field != null && field.FormattingString.Contains(fieldInstrText))
                        field.m_formattingString += m_fieldInstrText;
                }
                WFieldMark separator = new WFieldMark(m_doc, FieldMarkType.FieldSeparator);
                AddItem(separator, paraItems);
                if (lastItem is WField)
                    (lastItem as WField).FieldSeparator = separator;
            }
            else if (lastItem is WMergeField && m_fieldInstrText.Length > 0)
            {
                WMergeField mergeField = lastItem as WMergeField;
                string fldName = mergeField.FieldName;
                mergeField.ParseFieldCode(mergeField.FieldValue + m_fieldInstrText.ToString());
                if (!string.IsNullOrEmpty(fldName))
                    mergeField.FieldName = fldName;
                m_fieldInstrText.Remove(0, m_fieldInstrText.Length);
            }
        }
        /// <summary>
        /// Process the field mark end
        /// </summary>
        /// <param name="para"></param>
        private void ProcessFieldMarkEnd(ParagraphItemCollection paraItems)
        {
            m_isPrevItemFieldStart = false;
            m_currentFldCharType = FieldCharType.End;
            ParagraphItem lastItem = paraItems.LastItem as ParagraphItem;
            if (CurrentField != null)
                lastItem = CurrentField as ParagraphItem;
            if (!(lastItem is WMergeField))
            {
                WFieldMark endField = new WFieldMark(m_doc, FieldMarkType.FieldEnd);
                AddItem(endField, paraItems);
                if (lastItem is WField)
                {
                    (lastItem as WField).FieldEnd = endField;
                    if ((lastItem as WField).FieldType == FieldType.FieldHyperlink)
                        m_isInHeyperlinkField = false;
                }
                m_currentRunFormat = null;
            }

            if (m_fieldStack != null && m_fieldStack.Count > 0)
            {
                WField field = m_fieldStack.Pop();
                field.ParseFieldCode(field.FieldCode);

                if (field.FieldType == FieldType.FieldDate
                    || field.FieldType == FieldType.FieldTime)
                    field.Update();
            }
            m_currentFldCharType = FieldCharType.Unknown;
        }
        /// <summary>
        /// Insert the begin - field mark 
        /// </summary>
        private void InitFieldMarkBegin()
        {
            m_isPrevItemFieldStart = true;
            m_currentFldCharType = FieldCharType.Begin;
        }
        /// <summary>
        /// Gets the field code.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        /// <returns></returns>
        private WField GetFieldType(string fieldCode)
        {
            WField field = null;
            string typeCode = fieldCode.Trim();
            FieldType fieldType = FieldTypeDefiner.GetFieldType(typeCode);

            switch (fieldType)
            {
                case FieldType.FieldMergeField:
                    field = new WMergeField(m_doc);
                    break;
                case FieldType.FieldFormCheckBox:
                case FieldType.FieldFormTextInput:
                case FieldType.FieldFormDropDown:
                    if (CurrentField == null)
                    {
                        if (typeCode.ToUpper().StartsWith("TEXTINPUT") || typeCode.ToUpper().StartsWith("FORMTEXT"))
                            field = new WTextFormField(m_doc);
                        else if (typeCode.ToUpper().StartsWith("DDLIST") || typeCode.ToUpper().StartsWith("FORMDROPDOWN"))
                            field = new WDropDownFormField(m_doc);
                        else if (typeCode.ToUpper().StartsWith("CHECKBOX") || typeCode.ToUpper().StartsWith("FORMCHECKBOX"))
                            field = new WCheckBox(m_doc);

                        (field as WFormField).HasFFData = false;
                    }
                    else
                        field = CurrentField;
                    break;
                case FieldType.FieldIf:
                    field = new WIfField(m_doc);
                    break;
                default:
                    field = new WField(m_doc);
                    break;
            }

            field.FieldCode += fieldCode;
            if (!(field is WFormField))
            {
                field.FieldType = fieldType;
            }

            return field;
        }

        /// <summary>
        /// Append Field End to Image if it has hyperlink
        /// </summary>
        /// <param name="item">Paragraph item (WPicture)</param>
        /// <param name="para">Paragraph</param>
        private void AppendFieldEndToImageHyperlink(ParagraphItem item, ParagraphItemCollection paraItems)
        {
            int itemCount = paraItems.Count;
            if ((item.PreviousSibling is WFieldMark) && (item.PreviousSibling as WFieldMark).Type == FieldMarkType.FieldSeparator && !m_isInHeyperlinkField)
            {
                WFieldMark fieldEnd = new WFieldMark(m_doc, FieldMarkType.FieldEnd);
                AddToParagraph(fieldEnd, paraItems);
                (paraItems[itemCount - 3] as WField).FieldEnd = fieldEnd;
            }
        }
        #endregion Fields

        #region Formfields
        /// <summary>
        /// Parse the form field's data
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="entity">The entity</param>
        private void ParseFieldData(XmlReader reader, ParagraphItemCollection paraItems)
        {
            if (reader.IsEmptyElement)
                return;

            reader.MoveToContent();

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ffData":
                            if (!reader.IsEmptyElement)
                            {
                                Stream ffDataStream = ReadSingleNodeIntoStream(reader);
                                WFormField formField = GetFormField(ffDataStream);
                                if (formField != null)
                                {
                                    FieldStack.Push(formField);
                                    ffDataStream.Position = 0;
                                    XmlReader ffReader = UtilityMethods.CreateReader(ffDataStream);
                                    ParseFormField(ffReader, formField);
                                    if (m_currentRunFormat != null)
                                        formField.ApplyCharacterFormat(m_currentRunFormat);
                                    m_currentRunFormat = null;
                                    if (paraItems.OwnerBase is WParagraph)
                                        AddToParagraph(formField, paraItems);
                                }
                            }
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse the form fields
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="formField">The Form field.</param>
        private void ParseFormField(XmlReader reader, WFormField formField)
        {
            if (reader.IsEmptyElement)
                return;

            reader.MoveToContent();

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "checkBox":
                            ParseCheckBox(reader, formField as WCheckBox);
                            break;
                        case "textInput":
                            ParseTextInput(reader, formField as WTextFormField);
                            break;
                        case "ddList":
                            ParseDropDown(reader, formField as WDropDownFormField);
                            break;
                        case "name":
                            formField.Name = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "enabled":
                            formField.Enabled = GetBooleanValue(reader);
                            break;
                        case "calcOnExit":
                            formField.CalculateOnExit = GetBooleanValue(reader);
                            break;
                        case "helpText":
                            string helpText = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (helpText != null)
                                formField.Help = helpText;
                            break;
                        case "statusText":
                            string statusText = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (statusText != null)
                                formField.StatusBarHelp = statusText;
                            break;
                        case "entryMacro":
                            formField.MacroOnStart = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "exitMacro":
                            formField.MacroOnEnd = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the drop down form field.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        /// <param name="ent">The dropdown Form field</param>
        private void ParseDropDown(XmlReader reader, WDropDownFormField dropDownFormField)
        {
            if (reader.IsEmptyElement)
                return;

            reader.MoveToContent();

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {

                    switch (reader.LocalName)
                    {
                        case "listEntry":
                            string value = reader.GetAttribute("val", DocxConstants.W_namespace);
                            dropDownFormField.DropDownItems.Add(value);
                            break;
                        case "result":
                            int selIndex = Int32.Parse(reader.GetAttribute("val", DocxConstants.W_namespace));
                            dropDownFormField.DropDownSelectedIndex = selIndex;
                            break;
                        case "default":
                            int defIndex = Int32.Parse(reader.GetAttribute("val", DocxConstants.W_namespace));
                            dropDownFormField.DefaultDropDownValue = defIndex;
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the textform field.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        /// <param name="textFormField">The textForm field</param>
        private void ParseTextInput(XmlReader reader, WTextFormField textFormField)
        {
            if (reader.IsEmptyElement)
                return;

            reader.MoveToContent();

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "type":
                            string type = reader.GetAttribute("val", DocxConstants.W_namespace);
                            textFormField.Type = GetTextFieldType(type);
                            break;
                        case "default":
                            textFormField.DefaultText = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "maxLength":
                            textFormField.MaximumLength = Int32.Parse(reader.GetAttribute("val", DocxConstants.W_namespace));
                            break;
                        case "format":
                            string format = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (textFormField.Type == TextFormFieldType.RegularText)
                                textFormField.TextFormat = GetTextFormat(format);
                            else
                                textFormField.StringFormat = format;
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the text format.
        /// </summary>
        /// <param name="format">The format.</param>
        private TextFormat GetTextFormat(string format)
        {
            switch (format.ToUpper())
            {
                case "UPPERCASE":
                case "UPPER CASE":
                    {
                       return TextFormat.Uppercase;
                    }
                case "LOWERCASE":
                case "LOWER CASE":
                    {
                        return TextFormat.Lowercase;
                    }
                case "FIRSTCAPITAL":
                case "FIRST CAPITAL":
                    {
                        return TextFormat.FirstCapital;
                    }
                case "TITLECASE":
                case "TITLE CASE":
                    {
                        return TextFormat.Titlecase;                        
                    }
                default:
                    return TextFormat.None;
            }
        }
        /// <summary>
        /// Parses the type of the text formfield.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private TextFormFieldType GetTextFieldType(string type)
        {
            switch (type)
            {
                case "number":
                    return TextFormFieldType.NumberText;
                case "currentDate":
                case "currentTime":
                case "date":
                    return TextFormFieldType.DateText;
                //Handled for TextFormFieldType "calculation"
                case "calculated":
                    return TextFormFieldType.Calculation;
                default:
                    return TextFormFieldType.RegularText;
            }
        }
        /// <summary>
        /// Parses the check box.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        /// <param name="checkBox">The Checkbox.</param>
        private void ParseCheckBox(XmlReader reader, WCheckBox checkBox)
        {
            if (reader.IsEmptyElement)
                return;

            reader.MoveToContent();

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "sizeAuto":
                            bool isAutoSize = GetBooleanValue(reader);
                            checkBox.SizeType = (isAutoSize) ? CheckBoxSizeType.Auto : CheckBoxSizeType.Exactly;
                            break;
                        case "default":
                            checkBox.DefaultCheckBoxValue = GetBooleanValue(reader);
                            break;
                        case "checked":
                            checkBox.Checked = GetBooleanValue(reader);
                            break;
                        case "size":
                            checkBox.CheckBoxSize = Int32.Parse(reader.GetAttribute("val", DocxConstants.W_namespace)) / 2;
                            checkBox.SizeType = CheckBoxSizeType.Exactly;
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets the form field object.
        /// </summary>
        /// <param name="node">The node.</param>
        private WFormField GetFormField(Stream ffDataStream)
        {
            ffDataStream.Position = 0;
            XmlReader ffdataReader = UtilityMethods.CreateReader(ffDataStream);

            while (ffdataReader.Read())
            {
                switch (ffdataReader.LocalName)
                {
                    case "checkBox":
                        return new WCheckBox(m_doc);
                    case "ddList":
                        return new WDropDownFormField(m_doc);
                    case "textInput":
                        return new WTextFormField(m_doc);
                }
            }
            return null;
        }
        /// <summary>
        /// Determines whether the field is form field.
        /// </summary>
        private bool IsFormField()
        {
            if (CurrentField != null)
            {
                FieldType fldType = CurrentField.FieldType;
                if (fldType == FieldType.FieldFormCheckBox || fldType == FieldType.FieldFormDropDown ||
                  fldType == FieldType.FieldFormTextInput)
                    return true;
            }
            return false;
        }
        #endregion Formfields

        #region Footnote / Endnote
        private void ParseFootnotePart(bool isFootnote)
        {
            string partName = string.Empty;
            partName = (isFootnote) ? "footnotes.xml" : "endnotes.xml";
            m_currentFile = partName + ".rels";
            Part part = FindPart("word/", partName);
            if (part == null)
                return;

            XmlReader reader = UtilityMethods.CreateReader(part.DataStream);
            reader.MoveToContent();
            string id = string.Empty;
            string type = string.Empty;
            bool skip = false;
            do
            {
                if (!skip)
                    reader.Read();

                id = reader.GetAttribute("id", DocxConstants.W_namespace);
                type = reader.GetAttribute("type", DocxConstants.W_namespace);

                if (id != null && id != string.Empty)
                {
                    if (type != null && (type != string.Empty || type != "normal"))
                    {
                        skip = false;
                        if (isFootnote)
                        {
                            if (type == "separator")
                            {
                                m_doc.Footnotes.Separator = new WTextBody(m_doc, null);
                                ParseBody(reader, m_doc.Footnotes.Separator);
                            }
                            else if (type == "continuationSeparator")
                            {
                                m_doc.Footnotes.ContinuationSeparator = new WTextBody(m_doc, null);
                                ParseBody(reader, m_doc.Footnotes.ContinuationSeparator);
                            }
                            else if (type == "continuationNotice")
                            {
                                m_doc.Footnotes.ContinuationNotice = new WTextBody(m_doc, null);
                                ParseBody(reader, m_doc.Footnotes.ContinuationNotice);
                            }
                        }
                        else
                        {
                            if (type == "separator")
                            {
                                m_doc.Endnotes.Separator = new WTextBody(m_doc, null);
                                ParseBody(reader, m_doc.Endnotes.Separator);
                            }
                            else if (type == "continuationSeparator")
                            {
                                m_doc.Endnotes.ContinuationSeparator = new WTextBody(m_doc, null);
                                ParseBody(reader, m_doc.Endnotes.ContinuationSeparator);
                            }
                            else if (type == "continuationNotice")
                            {
                                m_doc.Endnotes.ContinuationNotice = new WTextBody(m_doc, null);
                                ParseBody(reader, m_doc.Endnotes.ContinuationNotice);
                            }
                        }
                        continue;
                    }

                    WFootnote footnote = new WFootnote(m_doc);
                    ParseBody(reader, footnote);
                    DictionaryEntry dictEntry = new DictionaryEntry(id, footnote);

                    if (isFootnote)
                    {
                        Footnote.Add(dictEntry);
                    }
                    else
                    {
                        footnote.FootnoteType = FootnoteType.Endnote;
                        Endnote.Add(dictEntry);
                    }
                    skip = false;
                }
            }
            while (!reader.EOF);
            m_currentFile = "";
        }
        /// <summary>
        /// Parses the footnote endnote.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private WFootnote ParseFootnote(XmlReader reader)
        {
            string id = reader.GetAttribute("id", DocxConstants.W_namespace);
            string customMarkFollows = reader.GetAttribute("customMarkFollows", DocxConstants.W_namespace);
            bool isAutoNumbered = true;
            string type = reader.LocalName;
            bool isFootnote = (type.StartsWith("footnote")) ? true : false;
            m_isFootnote = isFootnote;

            if (id == null)
                return null;

            if (customMarkFollows != null)
            {
                isAutoNumbered = (customMarkFollows == "0") ? true : false;
            }

            WFootnote footnote = GetFootnote(isFootnote, id);

            if (!isAutoNumbered)
            {
                ParseFootnoteSymbol(reader, footnote);
            }
            return footnote;
        }
        /// <summary>
        /// Gets the footnote by ID.
        /// </summary>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private WFootnote GetFootnote(bool isFootnote, string id)
        {
            WFootnote footnote = null;

            if (isFootnote)
            {
                for (int i = 0, leng = Footnote.Count; i < leng; i++)
                {
                    if (Footnote[i].Key.ToString() == id)
                    {
                        footnote = (WFootnote)Footnote[i].Value;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0, leng = Endnote.Count; i < leng; i++)
                {
                    if (Endnote[i].Key.ToString() == id)
                    {
                        footnote = (WFootnote)Endnote[i].Value;
                        break;
                    }
                }
            }

            return footnote;
        }
        /// <summary>
        /// Parses the footnote entnote symbol.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="footnote">The footnote.</param>
        private void ParseFootnoteSymbol(XmlReader reader, WFootnote footnote)
        {
            footnote.IsAutoNumbered = false;
            SkipWhitespaces(reader);
            MoveToNextLine(reader);
            string font = reader.GetAttribute("font", DocxConstants.W_namespace);
            string symbolStr = reader.GetAttribute("char", DocxConstants.W_namespace);
#if !SILVERLIGHT && !WP
            string text = reader.ReadString();
#else 
            string text = reader.ReadInnerXml();
#endif

            if (font != null && symbolStr != null)
            {
                footnote.SymbolFontName = font;
                symbolStr = symbolStr.Replace("F0", string.Empty);
                byte s = byte.Parse(symbolStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                footnote.SymbolCode = s;
            }
            else if (text != null)
            {
                footnote.CustomMarker = text;
            }
        }
        /// <summary>
        /// Moves to next line.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void MoveToNextLine(XmlReader reader)
        {
            do
            {
                reader.Read();
            }
            while (reader.LocalName == string.Empty || reader.NodeType == XmlNodeType.Whitespace);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private WTextRange ParseFootnoteMarker()
        {
            WTextRange tr = new WTextRange(m_doc);
            tr.Text = SpecialCharacters.FootnoteAscii.ToString();
            if (m_currentRunFormat != null)
            {
                tr.ApplyCharacterFormat(m_currentRunFormat);
            }

            return tr;
        }

        #endregion Footnote / Endnote

        #region OleObject
        /// <summary>
        /// Parse the objects
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private ParagraphItem ParseObject(XmlReader reader)
        {
            Stream objectStream = ReadSingleNodeIntoStream(reader);
            bool isControl = HasNode(objectStream, "control");
            bool isOleObject = HasNode(objectStream, "OLEObject");

            if (isOleObject)
            {
                return ParseOleObject(objectStream);
            }
            else if (isControl)
            {
                objectStream.Position = 0;
                return ParseXmlParaItem(objectStream);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Parse the ole object
        /// </summary>
        /// <param name="objectStream"></param>
        /// <returns></returns>
        private ParagraphItem ParseOleObject(Stream objectStream)
        {
            WOleObject oleObject = new WOleObject(m_doc);

            XmlReader reader = UtilityMethods.CreateReader(objectStream);

            if (reader.IsEmptyElement)
                return null;

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "shape":
                            ParseOlePicture(reader, oleObject);
                            break;
                        case "OLEObject":
                            ParseOleData(reader, oleObject);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
            return oleObject;
        }
        /// <summary>
        /// Parse the ole picture.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="oleObject"></param>
        private void ParseOlePicture(XmlReader reader, WOleObject oleObject)
        {
            if (reader.LocalName != "shape")
                throw new XmlException("OLE image data");

            if (reader.IsEmptyElement)
                return;

            bool isHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;

            string picStyle = reader.GetAttribute("style");

            reader.ReadToFollowing("imagedata", DocxConstants.V_namespace);

            if (reader.LocalName != "imagedata")
                return;

            string id = reader.GetAttribute("id", DocxConstants.R_namespace);
            WPicture pic = new WPicture(m_doc);
            if (id != null)
            {
                pic.SetOwner(oleObject);
                LoadImage(pic, id, isHeaderFooter, false);
                oleObject.SetOlePicture(pic);
                if (m_currentRunFormat != null)
                    pic.PictureCharacterFormat.ImportContainer(m_currentRunFormat);
            }
            if (picStyle != null)
            {
                ProcessPictureStyle(pic, picStyle);
                pic.IsShape = true;
            }

        }
        /// <summary>
        /// Parse the ole data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="oleObject"></param>
        private void ParseOleData(XmlReader reader, WOleObject oleObject)
        {
            if (reader.LocalName != "OLEObject")
                throw new XmlException("OLE Object data");

            string type = reader.GetAttribute("Type");

            if (type != null)
            {
                if (type == "Link")
                    oleObject.SetLinkType(OleLinkType.Link);
                else
                    oleObject.SetLinkType(OleLinkType.Embed);
            }

            string progID = reader.GetAttribute("ProgID");
            if (progID != null)
                oleObject.ObjectType = progID;

            string objectID = reader.GetAttribute("ObjectID");
            if (objectID != null)
                oleObject.OleStorageName = objectID.Replace("_", string.Empty);
            //Get the DrawAspect
            string drawAspect = reader.GetAttribute("DrawAspect");
            if (drawAspect != null && drawAspect == "Content")
                oleObject.DisplayAsIcon = false;
            string id = reader.GetAttribute("id", DocxConstants.R_namespace);

            if (oleObject.LinkType == OleLinkType.Embed)
            {
                Part olePart = GetOlePart(id);
                if (olePart == null)
                    return;

                olePart.DataStream.Position = 0;
                oleObject.ParseOlePartStream(olePart.DataStream);
                ClearParsedImage(olePart.Name, "word/embeddings/");
            }
            else
            {
                oleObject.LinkPath = GetOleLinkPath(id);
            }
        }
        /// <summary>
        /// Gets the OLE part.
        /// </summary>
        /// <param name="oleId">The OLE id.</param>
        /// <returns></returns>
        private Part GetOlePart(string oleId)
        {
            bool isHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;

            string fileName = null;

            if (isHeaderFooter)
            {
                Dictionary<string, DictionaryEntry> entryHF = GetFileRelations(m_currentFile);
                DictionaryEntry typeAndTarget = entryHF[oleId];
                fileName = (string)typeAndTarget.Value;
            }
            else
            {
                DictionaryEntry entry = m_docRelations[oleId];
                fileName = entry.Value.ToString();
            }

            if (fileName == null)
                return null;

            fileName = fileName.Replace("embeddings/", null);
            return FindPart("word/embeddings/", fileName);
        }
        /// <summary>
        /// Gets the OLE link path.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private string GetOleLinkPath(string id)
        {
            bool isHeaderFooter = false;// (m_currentFile.StartsWith("header") ||
            //m_currentFile.StartsWith("footer")) ? true : false;

            string fileName = null;
            if (isHeaderFooter)
            {
                //Dictionary<string, DictionaryEntry> entryHF = GetFileRelations(m_currentFile);
                //DictionaryEntry typeAndTarget = entryHF[id];
                //fileName = (string)typeAndTarget.Value;
            }
            else
            {
                DictionaryEntry entry = m_docRelations[id];
                fileName = entry.Value.ToString();
            }

            if (fileName == null)
                return null;
            else
                return fileName.Replace("file:///", string.Empty);
        }
        /// <summary>
        /// Determines whether the container is the native data.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the container is the native data; otherwise, <c>false</c>.
        /// </returns>
        private Boolean IsNativeDataInside(OleObjectType type)
        {
            bool isNative = false;
            if (type == OleObjectType.Excel_97_2003_Worksheet ||
                type == OleObjectType.ExcelBinaryWorksheet ||
                type == OleObjectType.ExcelChart ||
                type == OleObjectType.ExcelMacroWorksheet ||
                type == OleObjectType.ExcelWorksheet ||
                type == OleObjectType.PowerPoint_97_2003_Presentation ||
                type == OleObjectType.PowerPoint_97_2003_Slide ||
                type == OleObjectType.PowerPointMacroPresentation ||
                type == OleObjectType.PowerPointMacroSlide ||
                type == OleObjectType.PowerPointPresentation ||
                type == OleObjectType.PowerPointSlide ||
                type == OleObjectType.VisioDrawing ||
                type == OleObjectType.Word_97_2003_Document ||
                type == OleObjectType.WordDocument ||
                type == OleObjectType.WordMacroDocument)
            {
                isNative = true;
            }

            return isNative;
        }
        /// <summary>
        /// Check whether the mentioned node exists or not
        /// </summary>
        /// <param name="objectStream"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private bool HasNode(Stream objectStream, string elementName)
        {
            objectStream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(objectStream);
            while (reader.Read())
            {
                if (reader.LocalName == elementName)
                {
                    objectStream.Position = 0;
                    return true;
                }
            }
            objectStream.Position = 0;
            return false;
        }
        #endregion OleObject

        #region Shape/Textbox
        private ParagraphItem ParseShape(XmlReader reader, ParagraphItemCollection paraItems, 
            MemoryStream drawingStream, MemoryStream shapeStream)
        {
            shapeStream.Position = 0;
            AutoShapeType autoShapeType = AutoShapeType.Unknown;            
            Dictionary<string, Stream> docxProps = new Dictionary<string, Stream>();
            string shapeTypeID = null;
            
            ShapeType shapeType = DetectShapeType(shapeStream, ref autoShapeType, ref docxProps, ref shapeTypeID);
            
            shapeStream.Position = 0;
            XmlReader shapeReader = UtilityMethods.CreateReader(shapeStream);
            //Handled to skip the auto shape parsing, if the auto shape type is rectangle and if the previous implementation
            //recognizes text box shape to avoid break in current behavior
            if (autoShapeType != AutoShapeType.Unknown && 
                !(autoShapeType == AutoShapeType.Rectangle && shapeType == ShapeType.TextboxShape))
            {
                Shape shape = ParseShape(shapeStream, drawingStream);
                shape.Docx2007Props = docxProps;
                shape.ShapeTypeID = shapeTypeID;
                ParseXMLRelations(shape, shapeStream);                
                shape.DocxProps.Add("pict", shapeStream);
                if (drawingStream == null)
                    shape.Is2007Shape = true;

                shape.AutoShapeType = autoShapeType;
                return shape;
            }
            else
            {
                switch (shapeType)
                {
                    case ShapeType.TextboxShape:
                        return ParseTextboxShape(shapeStream, drawingStream);
                    case ShapeType.PictureShape:
                        return ParsePictureShape(shapeStream);
                    case ShapeType.WatermarkShape:
                        ParseWatermark(shapeStream, paraItems);
                        return null;
                    case ShapeType.GroupedShape:
                    default:
                        shapeStream.Position = 0;
                        return ParseXmlParaItem(shapeStream);
                }
            }
        }

        private void ParseXMLRelations(Shape shape, MemoryStream shapeStream)
        {
            List<string> relationshipIds = FindRelationshipIds(shapeStream);

            if (relationshipIds.Count > 0 && m_docRelations != null)
            {
                for (int i = 0, count = relationshipIds.Count; i < count; i++)
                {
                    string id = relationshipIds[i];
                    bool isImageRelation = ParseImageRelation(shape, id);
                    if (!isImageRelation)
                    {
                        DictionaryEntry entry = new DictionaryEntry();
                        if (m_currentFile != null && m_currentFile != string.Empty)
                        {
                            Dictionary<string, DictionaryEntry> rels = GetFileRelations(m_currentFile);
                            if (rels != null)
                                entry = rels[id];
                        }
                        else if (m_docRelations.ContainsKey(id))
                            entry = m_docRelations[id];
                        if (!shape.Relations.ContainsKey(id))
                            shape.Relations.Add(id, entry);
                    }
                }
            }
        }

        private Shape ParseShape(MemoryStream shapeStream, MemoryStream drawingStream)
        {
            shapeStream.Position = 0;
            XmlReader shapeReader = UtilityMethods.CreateReader(shapeStream);
            Shape shape = new Shape(m_doc);
            shape.FillFormat.Color = Color.Empty;
            shape.LineFormat.Color = Color.Empty;
            shape.ApplyCharacterFormat(m_currentRunFormat);
            if (drawingStream != null)
            {
                //Read DrawingML properties first
                drawingStream.Position = 0;
                XmlReader reader = UtilityMethods.CreateReader(drawingStream);
                ParseDrawingProperties(reader, shape);
            }
            Parse2007Shape(shapeReader, shape);
            return shape;
        }

        private void Parse2007Shape(XmlReader reader, Shape shape)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (!reader.EOF && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "shape":
                        case "rect":
                        case "oval":
                        case "line":
                        case "roundrect":
                            ParseShape2007Properties(reader, shape);
                            ParseTextbox(reader, shape);
                            //ParseTextBoxWrapStyle(reader, shape);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        
        }

        private void ParseTextBoxWrapStyle(XmlReader reader, Shape shape)
        {
            if (reader.IsEmptyElement)
                return;
            string endNode = reader.LocalName;
            reader.Read();
            bool skip = false;
            while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "wrap":
                            string wrapStyle = reader.GetAttribute("type");
                            if (wrapStyle != null)
                                shape.WrapFormat.TextWrappingStyle = GetWrapStyle(wrapStyle);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
       
        }
        private void ParseTextbox(XmlReader reader, Shape shape)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            bool skip = false;
            while (!reader.EOF && !(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "stroke":
                            ParseStroke(reader, shape);
                            break;
                        case "textbox":
                            ParseTextboxIntMargins(reader, shape);
                            ParseTextBoxStyle(reader, shape);
                            m_currentRunFormat = null;
                            Parse2007TextboxContent(reader, shape);
                            break;
                        case "fill":
                            skip = ParseFillEffects(reader, shape);
                            break;
                        case "wrap":
                            string wrapStyle = reader.GetAttribute("type");
                            if (wrapStyle != null)
                                shape.WrapFormat.TextWrappingStyle = GetWrapStyle(wrapStyle);
                            break;// Parse Textwrap Type
                            string wrapType = reader.GetAttribute("side");
                            if (wrapType != null)
                                shape.WrapFormat.TextWrappingType = GetTextWrapType(wrapType);

                        default:
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }

        private bool ParseFillEffects(XmlReader reader, Shape shape)
        {
            bool skip = false;
            string opacity = reader.GetAttribute("opacity");
            if (!String.IsNullOrEmpty(opacity))
                shape.FillFormat.Transparency = (float)Math.Round((1 - (float.Parse(opacity.Replace("f", "")) / 65536)), 2) * 100;

            string color2 = reader.GetAttribute("color2");
            if (color2 != null)
            {
                if (!shape.FillFormat.Color.IsEmpty)
                {
                    shape.FillFormat.ForeColor = shape.FillFormat.Color;//In 2007 format, color2 defines back color
                    shape.FillFormat.Color = GetColorValue(color2);
                }
            }

            string rotate = reader.GetAttribute("rotate");
            if (!string.IsNullOrEmpty(rotate))
                shape.FillFormat.RotateWithObject = GetBoolValue(rotate);

            string fillType = reader.GetAttribute("type");
            if (fillType != null)
            {
                switch (fillType)
                {
                    case "tile":
                    case "frame":
                    case "pattern":
                        {
                            switch (fillType)
                            {
                                case "pattern":
                                    shape.FillFormat.FillType = FillType.FillPatterned;
                                    break;
                                case "frame":
                                    shape.FillFormat.FillType = FillType.FillPicture;
                                    break;
                                case "tile":
                                    shape.FillFormat.FillType = FillType.FillTextured;
                                    break;
                            }
                            string imageId = reader.GetAttribute("id", DocxConstants.R_namespace);
                            if (!string.IsNullOrEmpty(imageId))
                                shape.FillFormat.ImageRecord = GetImageRecord(imageId);
                        }
                        break;
                    case "gradient":
                    case "gradientRadial":
                        color2 = reader.GetAttribute("color2");
                        shape.FillFormat.FillType = FillType.FillGradient;
                        if (fillType == "gradient")
                        {
                            shape.FillFormat.GradientFill.LinearGradient = new LinearGradient();
                            string angle = reader.GetAttribute("angle");
                            if (!string.IsNullOrEmpty(angle))
                            {
                                int value = GetAngle(angle);
                                if (angle.StartsWith("-"))
                                    shape.FillFormat.GradientFill.LinearGradient.AnglePositive = false;
                                if (value != int.MaxValue)
                                    shape.FillFormat.GradientFill.LinearGradient.Angle = Convert.ToInt16(value);
                            }
                        }
                        else
                        {
                            shape.FillFormat.GradientFill.PathGradient = new PathGradient();
                            shape.FillFormat.GradientFill.PathGradient.PathShade = GradientShadeType.Shape;
                        }
                        Parse2007GradientFill(reader, shape.FillFormat.GradientFill);
                        break;
                }
            }
            return skip;
        }

        private int GetAngle(string angle)
        {
            int angle2007 = int.Parse(angle);
            int angle2013 = int.MaxValue;
            if (angle2007 < 0)
            {
                angle2007 = -angle2007;
                if (angle2007 >= 0 && angle2007 <= 90)
                    angle2013 = 270 + angle2007;
                else if (angle2007 > 90 && angle2007 <= 180)
                    angle2013 = 0 + (angle2007 - 90);
                else if (angle2007 > 180 && angle2007 <= 270)
                    angle2013 = 90 + (angle2007 - 180);
                else if (angle2007 > 270 && angle2007 <= 360)
                    angle2013 = 180 + (angle2007 - 270);
                angle2013 = angle2013 == 360 ? 0 : angle2013;
            }
            else
            {
                if (angle2007 >= 0 && angle2007 <= 90)
                    angle2013 = 90 - angle2007;
                else if (angle2007 > 90 && angle2007 <= 180)
                    angle2013 = 270 + (180 - angle2007);
                else if (angle2007 > 180 && angle2007 <= 270)
                    angle2013 = 180 + (270 - angle2007);
                else if (angle2007 > 270 && angle2007 <= 360)
                    angle2013 = 90 + (360 - angle2007);

                angle2013 = angle2013 == 360 ? 0 : angle2013;
            }
            return angle2013;
        }

        private void Parse2007GradientFill(XmlReader reader, GradientFill gradientFill)
        {
            string rotate = reader.GetAttribute("rotate");
            if (!string.IsNullOrEmpty(rotate))
                gradientFill.RotateWithShape = rotate == "t" ? true : false;
            ParseFocus(reader, gradientFill);
            ParseGradientStops(reader, gradientFill);
            if(gradientFill.PathGradient != null) //Focus position will be preserved only for path gradient
                ParseFocusPosition(reader, gradientFill);
        }

        private void ParseFocusPosition(XmlReader reader, GradientFill gradientFill)
        {
            string focusPos = reader.GetAttribute("focusposition");
            if (focusPos == null)
            {
                gradientFill.TileRectangle.TopOffset = -100;
                gradientFill.TileRectangle.LeftOffset = -100;
                gradientFill.PathGradient.RightOffset = 100;
                gradientFill.PathGradient.BottomOffset = 100;
            }
            else if (focusPos == "1")
            {
                gradientFill.TileRectangle.TopOffset = -100;
                gradientFill.TileRectangle.LeftOffset = 100;
                gradientFill.PathGradient.RightOffset = -100;
                gradientFill.PathGradient.BottomOffset = 100;
            }
            else if (focusPos == "1,1")
            {
                gradientFill.TileRectangle.TopOffset = 100;
                gradientFill.TileRectangle.LeftOffset = 100;
                gradientFill.PathGradient.RightOffset = -100;
                gradientFill.PathGradient.BottomOffset = -100;
            }
            else if (focusPos == ",1")
            {
                gradientFill.TileRectangle.TopOffset = 100;
                gradientFill.TileRectangle.LeftOffset = -100;
                gradientFill.PathGradient.RightOffset = 100;
                gradientFill.PathGradient.BottomOffset = -100;
            }
            else if(focusPos == ".5,.5")
            {
                gradientFill.TileRectangle.TopOffset = 
                    gradientFill.TileRectangle.LeftOffset =
                    gradientFill.PathGradient.RightOffset =
                    gradientFill.PathGradient.BottomOffset = 50;
            }
        }

        private void ParseGradientStops(XmlReader reader, GradientFill gradientFill)
        {
            string colors = reader.GetAttribute("colors");
            if (!string.IsNullOrEmpty(colors))
            {
                string[] gradientStops = colors.Split(new char[] { ';' });
                for (int i = 0; i < gradientStops.Length; i++)
                {
                    string[] value = gradientStops[i].Split(new char[] { ' ' });
                    GradientStop gradientStop = new GradientStop();
                    gradientStop.Position = value[0].Contains("f") ?
                        Convert.ToByte(double.Parse(value[0].Replace("f", string.Empty)) * 100 / DLSConstants.FixedPointsUnit) :
                        Convert.ToByte(double.Parse(value[0]) * 100);
                    gradientStop.Color = GetColorValue(value[1]);
                    gradientFill.GradientStops.Add(gradientStop);
                }
            }
        }

        private void ParseFocus(XmlReader reader, GradientFill gradientFill)
        {
            gradientFill.Focus = reader.GetAttribute("focus");

            //if (focus == null || focus == "0%")
            //    gradientFill.ShadingVariant = GradientShadingVariant.ShadingDown;
            //else if (focus == "100%")
            //    gradientFill.ShadingVariant = GradientShadingVariant.ShadingUp;
            //else if (focus == "50%")
            //    gradientFill.ShadingVariant = GradientShadingVariant.ShadingMiddle;
            //else
            //    gradientFill.ShadingVariant = GradientShadingVariant.ShadingOut;
        }

        private ImageRecord GetImageRecord(string imageID)
        {
            bool isHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;
            byte[] imgBytes = null;
            string imageName = GetImageName(imageID, isHeaderFooter, false);
            
            if (ImageIds.ContainsKey(imageName))
                return m_doc.Images[ImageIds[imageName]];
            else
            {
                imgBytes = GetImageBytes(imageName);
                if (imgBytes != null)
                    return new ImageRecord(m_doc, imgBytes);
            }
            
            return null;
        }

        private void Parse2007TextboxContent(XmlReader reader, Shape shape)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;

            reader.Read();

            if (reader.IsEmptyElement)
                return;

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "p":
                            IWParagraph para = shape.TextBody.AddParagraph();
                            ParseParagraphItems(reader, para.Items);
                            break;
                        case "tbl":
                            IWTable table = shape.TextBody.AddTable();
                            //Set IsAutoResized property as true to layout the table with Auto width (Default in DocX format document)
                            table.TableFormat.IsAutoResized = true;
                            int prevGridCount = m_gridCount;
                            ParseTable(reader, table as WTable);
                            m_gridCount = prevGridCount;
                            break;
                        case "sdt":
                            IStructureDocumentTagBlock sdTagBlock = shape.TextBody.AddStructureDocumentTag();
                            ParseStructureDocumentTagBlock(reader, sdTagBlock as StructureDocumentTagBlock);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }

        }

        private void ParseTextBoxStyle(XmlReader reader, Shape shape)
        {
            string style = reader.GetAttribute("style");
            if (style == null)
                return;
            string[] styleProperties = style.Split(';');
            for (int i = 0; i < styleProperties.Length; i++)
            {
                //if (styleProperties[i] == DocxConstants.DEF_FIT_TEXT_TO_SHAPE)
                //    textbox.TextBoxFormat.FitTextToShape = true;
                //else 
                if (styleProperties[i] == "mso-layout-flow-alt:bottom-to-top")
                    shape.TextFrame.TextDirection = TextDirection.VerticalBottomToTop;
                else if (styleProperties[i] == "layout-flow:vertical" || styleProperties[i] == "layout-flow:vertical-ideographic")
                    shape.TextFrame.TextDirection = TextDirection.VerticalTopToBottom;
            }
        }

        private void ParseTextboxIntMargins(XmlReader reader, Shape shape)
        {
            //string inset = reader.GetAttribute("inset");
            //if (inset == null)
            //    return;

            //inset = inset.Replace("mm", string.Empty);
            //string[] insetParts = inset.Split(new char[1] { ',' });
            //for (int i = 0, cnt = insetParts.Length; i < cnt; i++)
            //{
            //    if (insetParts[i] == string.Empty)
            //        continue;

            //    float margin = GetTextboxMargin(insetParts[i]);
            //    if (i == 0) 
            //        textbox.TextBoxFormat.InternalMargin.Left = margin;
            //    else if (i == 1)
            //        textbox.TextBoxFormat.InternalMargin.Top = margin;
            //    else if (i == 2)
            //        textbox.TextBoxFormat.InternalMargin.Right = margin;
            //    else if (i == 3)
            //        textbox.TextBoxFormat.InternalMargin.Bottom = margin;
            //}
        
        }

        private void ParseStroke(XmlReader reader, Shape shape)
        {
            string value = reader.GetAttribute("opacity");
            if (!String.IsNullOrEmpty(value))
                shape.LineFormat.Transparency = (float)Math.Round((1 - (float.Parse(value.Replace("f", "")) / 65536)), 2) * 100;

            value = reader.GetAttribute("color2");
            if (value != null && !shape.LineFormat.Color.IsEmpty)
            {
                shape.LineFormat.ForeColor = shape.LineFormat.Color;//In 2007 format, color2 defines back color
                shape.LineFormat.Color = GetColorValue(value);
            }
            value = reader.GetAttribute("filltype");
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "tile":
                    case "frame":
                    case "pattern":
                        {
                            shape.LineFormat.LineFormatType = LineFormatType.Patterned;
                            value = reader.GetAttribute("id", DocxConstants.R_namespace);
                            if (!string.IsNullOrEmpty(value))
                                shape.LineFormat.ImageRecord = GetImageRecord(value);
                        }
                        break;
                }
            }
            value = reader.GetAttribute("dashstyle");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.DashStyle = GetDashStyle(value);
            value = reader.GetAttribute("linestyle");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.Style = GetShapeLineStyle(value);            
            value = reader.GetAttribute("joinstyle");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.LineJoin = GetLineJoinStyle(value);
            value = reader.GetAttribute("endcap");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.LineCap = GetLineCapStyle(value);
            value = reader.GetAttribute("startarrow");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.BeginArrowheadStyle = GetLineEnd(value);
            value = reader.GetAttribute("startarrowwidth");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.BeginArrowheadWidth = GetLineEndWidth(value);
            value = reader.GetAttribute("startarrowlength");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.BeginArrowheadLength = GetLineEndLength(value);
            value = reader.GetAttribute("endarrow");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.EndArrowheadStyle = GetLineEnd(value);
            value = reader.GetAttribute("endarrowwidth");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.EndArrowheadWidth = GetLineEndWidth(value);
            value = reader.GetAttribute("endarrowlength");
            if (!string.IsNullOrEmpty(value))
                shape.LineFormat.EndArrowheadLength = GetLineEndLength(value);
        }
        private LineStyle GetShapeLineStyle(string lineStyle)
        {
            switch (lineStyle)
            {
                // dbl (Double Lines) Double lines of equal width
                case "dbl":
                case "thinThin":
                    return LineStyle.ThinThin; //TextBoxLineStyle.Double
                    break;
                //thinThick (Thin Thick Double Lines) Double lines: one thin, one thick
                case "thinThick":
                    return LineStyle.ThinThick; //TextBoxLineStyle.ThinThick;
                    break;
                //thickThin (Thick Thin Double Lines) Double lines: one thick, one thin
                case "thickThin":
                    return LineStyle.ThickThin;//TextBoxLineStyle.ThickThin;
                    break;
                //tri (Thin Thick Thin Triple Lines) Three lines: thin, thick, thin
                case "thickBetweenThin":
                case "tri":
                    return LineStyle.ThickBetweenThin;
                    break;
                //sng (Single Line) Single line: one normal width
                default:
                    return LineStyle.Single;
                    break;
            }
            return LineStyle.Single;
        }
        private void ParseShape2007Properties(XmlReader reader, Shape shape)
        {
            string style = reader.GetAttribute("style");
            if (style == null)
                return;

            style = style.Trim();
            char[] delimeter = new char[1] { ';' };

            string[] styleParts = style.Split(delimeter);
            string[] propertyValues = new string[2];

            for (int i = 0, cnt = styleParts.Length; i < cnt; i++)
            {
                propertyValues = GetPropertyValues(styleParts[i]);

                if (propertyValues == null)
                    continue;
                else
                    Apply2007ShapeProperties(shape, propertyValues[0], propertyValues[1]);
            }
            shape.Adjustments = reader.GetAttribute("adj");
            ParseHorizontalRule(reader, shape);
            string value = reader.GetAttribute("arcsize");
            if (!string.IsNullOrEmpty(value))
                shape.ArcSize = double.Parse(value.Replace("f", string.Empty));
            ParseLineShapeProperties(reader, shape);
            Parse2007ShapeEffects(reader, shape);
        }

        private void ParseLineShapeProperties(XmlReader reader, Shape shape)
        {
            if (reader.LocalName == "line")
            {
                //from="0,4.5pt" to="153pt,18.75pt"
                string from = reader.GetAttribute("from");
                string to = reader.GetAttribute("to");
                if ((!string.IsNullOrEmpty(from)) && (!string.IsNullOrEmpty(to)))
                {
                    float x = 0, y = 0, x1 = 0, y1 = 0, height = 0, width = 0;
                    //From
                    string[] fromValues = from.Split(new char[] { ',' });
                    x = GetPointValue(fromValues[0]);
                    y = GetPointValue(fromValues[1]);
                    shape.HorizontalPosition = x;
                    shape.VerticalPosition = y;
                    //To
                    string[] toValues = to.Split(new char[] { ',' });
                    x1 = GetPointValue(toValues[0]);
                    y1 = GetPointValue(toValues[1]);
                    shape.Height = (height = y1 - y) > 0 ? height : -height;
                    shape.Width = (width = x1 - x) > 0 ? width : -width;
                }
            }
        }

        private void ParseHorizontalRule(XmlReader reader, Shape shape)
        {
            string value = reader.GetAttribute("hr", DocxConstants.O_namespace);
            if (!string.IsNullOrEmpty(value))
                shape.IsHorizontalRule = GetBoolValue(value);
            value = reader.GetAttribute("hrstd", DocxConstants.O_namespace);
            if (!string.IsNullOrEmpty(value))
                shape.UseStandardColorHR = GetBoolValue(value);
            value = reader.GetAttribute("hrnoshade", DocxConstants.O_namespace);
            if (!string.IsNullOrEmpty(value))
                shape.UseNoShadeHR = GetBoolValue(value);
            value = reader.GetAttribute("hralign", DocxConstants.O_namespace);
            if (!string.IsNullOrEmpty(value))
                shape.HorizontalAlignment = GetHorizAlign(value);
        }
        private void Parse2007ShapeEffects(XmlReader reader, Shape shape)
        {
            // Parse line effects
            string strokeColor = reader.GetAttribute("strokecolor");
            if (strokeColor != null)
                shape.LineFormat.Color = GetColorValue(strokeColor);

            string strokeweight = reader.GetAttribute("strokeweight");
            if (strokeweight != null)
            {
                shape.LineFormat.Weight = GetPointValue(strokeweight);
            }
            else
                shape.LineFormat.Weight = 0.75f; //Default value of stroke weight in Fallback (2007) format
            // Parse fill effects
            string filled = reader.GetAttribute("filled");
            if (filled != null && filled == "f")
            {
                shape.FillFormat.Fill = false;
            }
            else
            {
                string fillColor = reader.GetAttribute("fillcolor");
                if (fillColor != null)
                {
                    shape.FillFormat.Color = GetColorValue(fillColor);
                    shape.FillFormat.FillType = FillType.FillSolid;
                }
            }

            string stroked = reader.GetAttribute("stroked");
            if (stroked != null)
            {
                shape.LineFormat.Line = (stroked == "f") ? false : true;
            }

            string allowInCell = reader.GetAttribute("allowincell", DocxConstants.O_namespace);
            if (allowInCell != null)
            {
                shape.LayoutInCell = (allowInCell == "f") ? false : true;
            }
        }
        
        private void Apply2007ShapeProperties(Shape shape, string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case "margin-left":
                    shape.HorizontalPosition = GetPointValue(propertyValue);
                    break;
                case "margin-top":
                    shape.VerticalPosition = GetPointValue(propertyValue);
                    break;
                case "width":
                    shape.Width = GetPointValue(propertyValue);
                    break;
                case "height":
                    shape.Height = GetPointValue(propertyValue);
                    break;
                case "z-index":
                    int val = int.Parse(propertyValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    shape.ZOrderPosition = val;

                    shape.IsBelowText = (val > 0) ? false : true;
                    if (shape.IsBelowText)
                        shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.Behind;
                    else
                        shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                    break;
                case "mso-position-horizontal":
                    shape.HorizontalAlignment = GetHorizAlign(propertyValue);
                    break;
                case "mso-position-vertical":
                    shape.VerticalAlignment = GetVertAlign(propertyValue);
                    break;
                case "v-text-anchor":
                    shape.TextFrame.TextVerticalAlignment = GetTextVertAlign(propertyValue);
                    break;
                case "mso-position-vertical-relative":
                    shape.VerticalOrigin = GetVertOrigin(propertyValue);
                    break;
                case "mso-position-horizontal-relative":
                    shape.HorizontalOrigin = GetHorizOrigin(propertyValue);
                    break;
                case "mso-left-percent":
                    shape.TextFrame.HorizontalRelativePercent = ParseFloatVal(propertyValue) / 10;
                    break;
                case "mso-top-percent":
                    shape.TextFrame.VerticalRelativePercent = ParseFloatVal(propertyValue) / 10;
                    break;
                case "mso-wrap-distance-left":
                    shape.WrapFormat.DistanceLeft = GetPointValue(propertyValue);
                    break;
                case "mso-wrap-distance-top":
                    shape.WrapFormat.DistanceTop = GetPointValue(propertyValue);
                    break;
                case "mso-wrap-distance-right":
                    shape.WrapFormat.DistanceRight = GetPointValue(propertyValue);
                    break;
                case "mso-wrap-distance-bottom":
                    shape.WrapFormat.DistanceBottom = GetPointValue(propertyValue);
                    break;
                default:
                    shape.DocxStyleProps.Add(propertyName + ":" + propertyValue);
                    break;
            }
        }

        private void ParseDrawingProperties(XmlReader reader, Shape shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "drawing")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            string value;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "drawing")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "wrapSquare":
                            shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.Square;
                            break;
                        case "wrapTight":
                            shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                            ParseWrapPolygon(reader, shape);
                            break;
                        case "wrapThrough":
                            shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.Through;
                            ParseWrapPolygon(reader, shape);
                            break;
                        case "wrapTopAndBottom":
                            shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.TopAndBottom;
                            break;
                        case "wrapNone":
                            if (shape.IsBelowText)
                                shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.Behind;
                            else
                                shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                            break;
                        case "anchor":
                            if (reader.AttributeCount == 0)
                                break;
                            value = reader.GetAttribute("behindDoc");
                            shape.IsBelowText = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("allowOverlap");
                            if (value != null)
                                shape.WrapFormat.AllowOverlap = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("relativeHeight");
                            shape.ZOrderPosition = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case "inline":
                            shape.WrapFormat.TextWrappingStyle = TextWrappingStyle.Inline;
                            break;
                        case "bodypr":
                            if (reader.AttributeCount == 0)
                                break;
                            value = reader.GetAttribute("vert");
                            if (value == "vert")
                                shape.TextFrame.TextDirection = TextDirection.VerticalTopToBottom;
                            else if (value == "vert270")
                                shape.TextFrame.TextDirection = TextDirection.VerticalBottomToTop;

                            value = reader.GetAttribute("anchor");
                            shape.TextFrame.TextVerticalAlignment = GetTextVertAlign(value);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
      
        }

        private void ParseWatermark(MemoryStream shapeStream, ParagraphItemCollection paraItems)
        {
            if (m_doc.Watermark.Type == WatermarkType.NoWatermark)
            {
                string value = FindAttributeValue(shapeStream, "shape", "id", null);
                if (value.StartsWith("PowerPlusWaterMarkObject"))
                {
                    ParseTextWatermark(shapeStream);
                }
                else if (value.StartsWith("WordPictureWatermark"))
                {
                    ParsePictureWatermark(shapeStream);
                }
            }
            Entity baseEntity = GetBaseEntity(paraItems.OwnerBase as Entity);

            if (baseEntity is HeaderFooter)
            {
                HeaderFooter hf = baseEntity as HeaderFooter;
                if (hf != null)
                    hf.WriteWatermark = true;
            }
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
        private void ParsePictureWatermark(MemoryStream shapeStream)
        {
            m_doc.InsertWatermark(WatermarkType.PictureWatermark);
            PictureWatermark watermark = m_doc.Watermark as PictureWatermark;

            string imagedataId = FindAttributeValue(shapeStream, "imagedata", "id", DocxConstants.R_namespace);

            //if (attr.ParentNode != null && attr.ParentNode.Attributes.Count > 0)
            //    attr.ParentNode.Attributes.RemoveNamedItem("href", DocxConstants.R_namespace);

            WPicture pic = new WPicture(m_doc);
            LoadImage(pic, imagedataId, true, false);
            watermark.WordPicture = pic;

            string imagedataGainValue = FindAttributeValue(shapeStream, "imagedata", "gain", null);
            string imagedataBlackLevel = FindAttributeValue(shapeStream, "imagedata", "blacklevel", null);

            if (imagedataGainValue == null && imagedataBlackLevel == null)
            {
                watermark.Washout = false;
            }
            shapeStream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(shapeStream);
            ParsePictureWatermarkProperties(reader, watermark);
        }

        private void ParsePictureWatermarkProperties(XmlReader reader, PictureWatermark watermark)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "shape":
                            ParsePictureShapeProperties(reader, watermark.WordPicture);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parses the text watermark.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="ent">The ent.</param>
        private void ParseTextWatermark(Stream stream)
        {
            m_doc.InsertWatermark(WatermarkType.TextWatermark);
            TextWatermark watermark = m_doc.Watermark as TextWatermark;
            string attrValue = FindAttributeValue(stream, "fill", "opacity", null);
            if (attrValue == null)
            {
                watermark.Semitransparent = false;
            }
            XmlReader reader = UtilityMethods.CreateReader(stream);
            ParseTextWatermarkProperties(reader, watermark);
        }

        private void ParseTextWatermarkProperties(XmlReader reader, TextWatermark watermark)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "textpath":
                            if (reader.GetAttribute("string") != null)
                            {
                                watermark.Text = reader.GetAttribute("string");
                            }
                            if (reader.GetAttribute("style") != null)
                            {
                                int shapeH = watermark.ShapeHeightInPixels;
                                int shapeW = watermark.ShapeWidthInPixels;
                                ParseTextWatermarkStyleAndSize(reader, watermark, shapeH, shapeW);
                            }
                            break;
                        case "shape":
                            string color = reader.GetAttribute("fillcolor");
                            if (color == null)
                                return;

                            if (color == "auto")
                                watermark.Color = Color.Empty;
                            else
                                watermark.Color = GetColorValue(color);

                            ParseTextWatermarkPosition(reader, watermark);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();
            }
        }
        /// <summary>
        /// Parses the watermark position.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="water">The water.</param>
        private void ParseTextWatermarkPosition(XmlReader reader, TextWatermark watermark)
        {
            string properties = reader.GetAttribute("style");

            if (properties.Length == 0)
                return;

            string[] propParts = properties.Split(new char[1] { ';' });
            string partString = null;
            bool isHorizontal = true;
            for (int i = 0, cnt = propParts.Length; i < cnt; i++)
            {
                partString = propParts[i];
                if (partString.StartsWith("width:"))
                {
                    partString = partString.Replace("width:", string.Empty);
                    watermark.ShapeWidthInPixels = (int)(ParseSize(partString) * 20);
                }
                else if (partString.StartsWith("height:"))
                {
                    partString = partString.Replace("height:", string.Empty);
                    watermark.ShapeHeightInPixels = (int)(ParseSize(partString) * 20);
                }
                else if (partString.StartsWith("rotation"))
                {
                    isHorizontal = false;
                }
            }

            if (isHorizontal)
            {
                watermark.Layout = WatermarkLayout.Horizontal;
            }
        }
        /// <summary>
        /// Parses the size of the font name and.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ent">The ent.</param>
        /// <param name="shapeH">The shape H.</param>
        /// <param name="shapeW">The shape W.</param>
        private void ParseTextWatermarkStyleAndSize(XmlReader reader, TextWatermark watermark, int shapeH, int shapeW)
        {
            string font = reader.GetAttribute("style");
            if (font.Length == 0)
                return;

            string[] fontProp = font.Split(new char[1] { ';' });

            string fontName = fontProp[0];
            fontName = fontName.Replace(@"""", string.Empty);
            watermark.FontName = fontName.Replace("font-family:", string.Empty);

            string fontSize = (fontProp.Length == 2) ? fontProp[1] : string.Empty;
            if (fontSize != string.Empty)
            {
                if (fontSize == "font-size:2in")
                {
                    watermark.Size = 144;
                }
                else
                {
                    fontSize = fontSize.Replace("font-size:", string.Empty);
                    watermark.Size = ParseSize(fontSize);
                }

                watermark.ShapeHeightInPixels = shapeH;
                watermark.ShapeWidthInPixels = shapeW;
            }
        }
        private string FindAttributeValue(Stream stream, string elementName, string attributeName, string namspace)
        {
            stream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(stream);
            while (reader.Read())
            {
                if (reader.LocalName == elementName)
                {
                    string attrValue = reader.GetAttribute(attributeName, namspace);
                    if (attrValue != null)
                    {
                        stream.Position = 0;
                        return attrValue;
                    }
                }
            }
            stream.Position = 0;
            return null;
        }
        private WPicture ParsePictureShape(MemoryStream shapeStream)
        {
            XmlReader reader = UtilityMethods.CreateReader(shapeStream);
            WPicture picture = new WPicture(m_doc);
            if (m_currentRunFormat != null)
                picture.PictureCharacterFormat.ImportContainer(m_currentRunFormat);
            m_currentRunFormat = null;
            picture.PictureShape.ShapeContainer = new MsofbtSpContainer(m_doc);
            picture.PictureShape.ShapeContainer.Children.Add(new MsofbtOPT(m_doc));
            picture.PictureShape.ShapeContainer.Children.Add(new MsofbtTertiaryFOPT(m_doc));
            ParsePictureShape(reader, picture);
            return picture;
        }
        /// <summary>
        /// Parses the picture shape.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ent">The ent.</param>
        private void ParsePictureShape(XmlReader reader, IEntity entity)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;

            reader.Read();

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "shape":
                            ParsePictureShapeProperties(reader, entity);
                            WPicture pic = entity as WPicture;
                            pic.IsShape = true;
                            ParsePictureShapeImage(reader, entity);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }

        private void ParsePictureShapeImage(XmlReader reader, IEntity ent)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;

            reader.Read();
            WPicture pict = ent as WPicture;

            while (reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "imagedata":
                            string id = reader.GetAttribute("id", DocxConstants.R_namespace);
                            if (!string.IsNullOrEmpty(id))
                            {
                                bool isHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;
                                float width = pict.Width;
                                float height = pict.Height;
                                LoadImage(pict, id, isHeaderFooter, false);
                                pict.Width = width;
                                pict.Height = height;
                            }
                            string title = reader.GetAttribute("title", DocxConstants.O_namespace);
                            if (title != null)
                                pict.Title = title;
                            break;
                        case "wrap":
                            string wrapStyle = reader.GetAttribute("type");
                            if (wrapStyle != null && pict.Position != ShapePosition.Static && wrapStyle != "none")
                                pict.TextWrappingStyle = GetWrapStyle(wrapStyle);

                            string wrapType = reader.GetAttribute("side");
                            if (wrapType != null)
                                pict.TextWrappingType = GetWrapType(wrapType);
                            break;
                        case "bordertop":
                            ParseShapeBorder(reader, pict.PictureShape.PictureDescriptor.BorderTop);
                            break;
                        case "borderleft":
                            ParseShapeBorder(reader, pict.PictureShape.PictureDescriptor.BorderLeft);
                            break;
                        case "borderbottom":
                            ParseShapeBorder(reader, pict.PictureShape.PictureDescriptor.BorderBottom);
                            break;
                        case "borderright":
                            ParseShapeBorder(reader, pict.PictureShape.PictureDescriptor.BorderRight);
                            break;
                        case "stroke":
                            ParseStrokeProps(reader, pict.PictureShape);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();
            }
        }
        /// <summary>
        /// Parses the stroke props.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="shape">The shape.</param>
        private void ParseStrokeProps(XmlReader reader, InlineShapeObject shape)
        {
            if (!shape.ShapeContainer.ShapeOptions.LineProperties.HasDefined)
            {
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties, LineStyleBooleanProperties.DefaultValue);
                shape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = true;
                shape.ShapeContainer.ShapeOptions.LineProperties.Line = true;
            }
            string value = reader.GetAttribute("on");
            if (value != null)
            {
                bool val = GetBoolValue(value);
                shape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = val;
                shape.ShapeContainer.ShapeOptions.LineProperties.Line = val;
            }
            value = reader.GetAttribute("color");
            if (value != null)
            {
                Color color = GetHexColor(value);
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineColor, WordColor.ConvertColorToRGB(color));
            }
            value = reader.GetAttribute("dashstyle");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineDashing, (uint)GetDashStyle(value));

            value = reader.GetAttribute("endarrow");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndArrowhead, (uint)GetLineEnd(value));

            value = reader.GetAttribute("endarrowlength");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndArrowLength, (uint)GetLineEndLength(value));

            value = reader.GetAttribute("endarrowwidth");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndArrowWidth, (uint)GetLineEndWidth(value));

            value = reader.GetAttribute("endcap");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndCapStyle, (uint)GetLineCapStyle(value));

            value = reader.GetAttribute("forcedash");
            if (value != null)
            {
                bool val = GetBoolValue(value);
                shape.ShapeContainer.ShapeOptions.LineProperties.UsefNoLineDrawDash = val;
                shape.ShapeContainer.ShapeOptions.LineProperties.NoLineDrawDash = val;
            }

            value = reader.GetAttribute("insetpen");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.LineProperties.PenAlignInset = GetBoolValue(value);

            value = reader.GetAttribute("joinstyle");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineJoinStyle, (uint)GetLineJoinStyle(value));

            value = reader.GetAttribute("linestyle");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyle, (uint)GetLineStyle(value));

            value = reader.GetAttribute("miterlimit");
            if (value != null)
            {
                uint miterLimit;
                if (value.EndsWith("f"))
                {
                    value = value.Replace("f", "");
                    miterLimit = uint.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
                    shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineMiterLimit, miterLimit);
                }
                else if (value.EndsWith("%"))
                {
                    miterLimit = (uint)((GetPercentage(value) * DLSConstants.FixedPointsUnit) / DLSConstants.HundredthsUnit);
                    shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineMiterLimit, miterLimit);
                }
            }

            value = reader.GetAttribute("opacity");
            if (value != null)
            {
                uint opacity;
                if (value.EndsWith("f"))
                {
                    opacity = uint.Parse(value.Replace("f", ""), NumberStyles.Number, CultureInfo.InvariantCulture);
                    //65536 represents 0% transparency (100% opaque), in Docx suffixed with 65536f.
                    shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineOpacity, opacity);
                }
                else if (value.EndsWith("%"))
                {
                    double dOpacity = GetPercentage(value);
                    opacity = (uint)((dOpacity * DLSConstants.FixedPointsUnit) / DLSConstants.HundredthsUnit);
                    //65536 represents 0% transparency (100% opaque), in Docx suffixed with 65536f.
                    shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineOpacity, opacity);
                }
            }

            value = reader.GetAttribute("startarrow");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStartArrowhead, (uint)GetLineEnd(value));

            value = reader.GetAttribute("startarrowlength");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStartArrowLength, (uint)GetLineEndLength(value));

            value = reader.GetAttribute("startarrowwidth");
            if (value != null)
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStartArrowWidth, (uint)GetLineEndWidth(value));

            value = reader.GetAttribute("weight");
            if (value != null)
            {
                uint lineWidth = (uint)(GetPointValue(value) * DLSConstants.EmusPerPoint);
                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineWidth, lineWidth);
            }
        }
        /// <summary>
        /// Gets the line join style.
        /// </summary>
        /// <param name="lineJoinStyle">The line join style.</param>
        /// <returns></returns>
        private LineJoin GetLineJoinStyle(string lineJoinStyle)
        {
            switch (lineJoinStyle)
            {
                case "bevel":
                    return LineJoin.Bevel;
                case "round":
                    return LineJoin.Round;
                default:
                    return LineJoin.Miter;
            }
        }
        /// <summary>
        /// Parses the shape border.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="border">The border.</param>
        private void ParseShapeBorder(XmlReader reader, BorderCode brc)
        {
            string attrVal = reader.GetAttribute("type");
            if (attrVal != null)
            {
                BorderStyle borderStyle = GetShapeBorderStyle(attrVal);
                brc.BorderType = (byte)borderStyle;
            }

            attrVal = reader.GetAttribute("width");
            if (attrVal != null)
            {
                int lineWidth = int.Parse(attrVal, NumberStyles.Number, CultureInfo.InvariantCulture);
                brc.LineWidth = (byte)lineWidth;
            }

            attrVal = reader.GetAttribute("shadow");
            if (attrVal != null && (attrVal == "on" || attrVal == "1" || attrVal == "true"))
                brc.Shadow = true;
        }
        /// <summary>
        /// Gets the shape border style.
        /// </summary>
        /// <param name="boderStyle">The boder style.</param>
        /// <returns></returns>
        private BorderStyle GetShapeBorderStyle(string boderStyle)
        {
            BorderStyle style = BorderStyle.None;
            switch (boderStyle)
            {
                case "single":
                    style = BorderStyle.Single;
                    break;
                case "thick":
                    style = BorderStyle.Thick;
                    break;
                case "double":
                    style = BorderStyle.Double;
                    break;
                case "hairline":
                    style = BorderStyle.Hairline;
                    break;
                case "dot":
                    style = BorderStyle.Dot;
                    break;
                case "dash":
                    style = BorderStyle.DashLargeGap;
                    break;
                case "dotDash":
                    style = BorderStyle.DotDash;
                    break;
                case "dashDotDot":
                    style = BorderStyle.DotDotDash;
                    break;
                case "triple":
                    style = BorderStyle.Triple;
                    break;
                case "thinThickSmall":
                    style = BorderStyle.ThinThickSmallGap;
                    break;
                case "thickThinSmall":
                    style = BorderStyle.ThinThinSmallGap;
                    break;
                case "thickBetweenThinSmall":
                    style = BorderStyle.ThinThickThinSmallGap;
                    break;
                case "thinThick":
                    style = BorderStyle.ThinThickMediumGap;
                    break;
                case "thickThin":
                    style = BorderStyle.ThickThinMediumGap;
                    break;
                case "thickBetweenThin":
                    style = BorderStyle.ThickThickThinMediumGap;
                    break;
                case "thinThickLarge":
                    style = BorderStyle.ThinThickLargeGap;
                    break;
                case "thickThinLarge":
                    style = BorderStyle.ThickThinLargeGap;
                    break;
                case "thickBetweenThinLarge":
                    style = BorderStyle.ThinThickThinLargeGap;
                    break;
                case "wave":
                    style = BorderStyle.Wave;
                    break;
                case "doubleWave":
                    style = BorderStyle.DoubleWave;
                    break;
                case "dashedSmall":
                    style = BorderStyle.DashSmallGap;
                    break;
                case "dashDotStroked":
                    style = BorderStyle.DashDotStroker;
                    break;
                case "threeDEmboss":
                    style = BorderStyle.Emboss3D;
                    break;
                case "threeDEngrave":
                    style = BorderStyle.Engrave3D;
                    break;
                case "HTMLOutset":
                    style = BorderStyle.Outset;
                    break;
                case "HTMLInset":
                    style = BorderStyle.Inset;
                    break;
            }
            return style;
        }
        /// <summary>
        /// Parses the pic shape props.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ent">The ent.</param>
        private void ParsePictureShapeProperties(XmlReader reader, IEntity ent)
        {
            WPicture pic = ent as WPicture;
            string style = reader.GetAttribute("style");
            if (style != null)
            {
                style = style.Trim();
                char[] splitter = new char[1] { ';' };

                string[] styleParts = style.Split(splitter);
                string[] propVals = new string[2];

                for (int i = 0, cnt = styleParts.Length; i < cnt; i++)
                {
                    propVals = GetPropertyValues(styleParts[i]);
                    if (propVals == null)
                        continue;
                    else
                        ParsePictureShapeProperties(pic, propVals[0], propVals[1]);
                }
            }
            string value = reader.GetAttribute("bordertopcolor", DocxConstants.O_namespace);
            if (value != null)
            {
                Color color = GetHexColor(value);
                int id = WordColor.ConvertColorToId(color);
                pic.PictureShape.PictureDescriptor.BorderTop.LineColor = (byte)id;
                pic.PictureShape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderTopColor, WordColor.ConvertColorToRGB(color));
            }
            value = reader.GetAttribute("borderleftcolor", DocxConstants.O_namespace);
            if (value != null)
            {
                Color color = GetHexColor(value);
                int id = WordColor.ConvertColorToId(color);
                pic.PictureShape.PictureDescriptor.BorderLeft.LineColor = (byte)id;
                pic.PictureShape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderLeftColor, WordColor.ConvertColorToRGB(color));
            }
            value = reader.GetAttribute("borderbottomcolor", DocxConstants.O_namespace);
            if (value != null)
            {
                Color color = GetHexColor(value);
                int id = WordColor.ConvertColorToId(color);
                pic.PictureShape.PictureDescriptor.BorderBottom.LineColor = (byte)id;
                pic.PictureShape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderBottomColor, WordColor.ConvertColorToRGB(color));
            }
            value = reader.GetAttribute("borderrightcolor", DocxConstants.O_namespace);
            if (value != null)
            {
                Color color = GetHexColor(value);
                int id = WordColor.ConvertColorToId(color);
                pic.PictureShape.PictureDescriptor.BorderRight.LineColor = (byte)id;
                pic.PictureShape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderRightColor, WordColor.ConvertColorToRGB(color));
            }
            value = reader.GetAttribute("stroked");
            if (value != null)
            {
                pic.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties, LineStyleBooleanProperties.DefaultValue);
                bool val = GetBoolValue(value);
                if (val)
                {
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = true;
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.Line = true;
                }
            }
            value = reader.GetAttribute("strokecolor");
            if (value != null)
            {
                if (!pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.HasDefined)
                {
                    pic.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties, LineStyleBooleanProperties.DefaultValue);
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = true;
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.Line = true;
                }
                Color color = GetHexColor(value);
                pic.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineColor, WordColor.ConvertColorToRGB(color));
            }
            value = reader.GetAttribute("strokeweight");
            if (value != null)
            {
                if (!pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.HasDefined)
                {
                    pic.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties, LineStyleBooleanProperties.DefaultValue);
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = true;
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.Line = true;
                }
                uint lineWidth = (uint)(GetPointValue(value) * DLSConstants.EmusPerPoint);
                pic.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineWidth, lineWidth);
            }
            value = reader.GetAttribute("insetpen");
            if (value != null)
            {
                if (!pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.HasDefined)
                {
                    pic.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties, LineStyleBooleanProperties.DefaultValue);
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = true;
                    pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.Line = true;
                }
                pic.PictureShape.ShapeContainer.ShapeOptions.LineProperties.PenAlignInset = GetBoolValue(value);
            }
            pic.AlternativeText = reader.GetAttribute("alt");
        }
        /// <summary>
        /// Parses the pic shape props.
        /// </summary>
        /// <param name="pic">The pic.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="propVal">The prop val.</param>
        private void ParsePictureShapeProperties(WPicture pic, string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case "margin-left":
                    pic.HorizontalPosition = GetPointValue(propertyValue);
                    break;
                case "margin-top":
                    pic.VerticalPosition = GetPointValue(propertyValue);
                    break;
                case "width":
                    pic.Width = GetPointValue(propertyValue);
                    break;
                case "height":
                    pic.Height = GetPointValue(propertyValue);
                    break;
                case "z-index":
                    int val = int.Parse(propertyValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    pic.OrderIndex = val;

                    pic.IsBelowText = (val > 0) ? false : true;
                    if (pic.Position != ShapePosition.Static)
                    {
                        if (pic.IsBelowText)
                            pic.TextWrappingStyle = TextWrappingStyle.Behind;
                        else
                            pic.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                    }
                    break;
                case "mso-position-horizontal":
                    pic.HorizontalAlignment = GetHorizAlign(propertyValue);
                    break;
                case "mso-position-vertical":
                    pic.VerticalAlignment = GetVertAlign(propertyValue);
                    break;
                case "mso-position-vertical-relative":
                    pic.VerticalOrigin = GetVertOrigin(propertyValue);
                    break;
                case "mso-position-horizontal-relative":
                    pic.HorizontalOrigin = GetHorizOrigin(propertyValue);
                    break;
                case "position":
                    if (!string.IsNullOrEmpty(propertyValue) && propertyValue == "absolute")
                    {
                        pic.HorizontalOrigin = HorizontalOrigin.Column;
                        pic.VerticalOrigin = VerticalOrigin.Paragraph;
                    }
                    switch (propertyValue)
                    {
                        case "absolute":
                            pic.Position = ShapePosition.Absolute;
                            break;
                        case "relative":
                            pic.Position = ShapePosition.Relative;
                            break;
                    }
                    if (pic.Position != ShapePosition.Static)
                        pic.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                    break;
                default:
                    break;
            }
        }
        private WTextBox ParseTextboxShape(MemoryStream shapeStream, MemoryStream drawingStream)
        {
            shapeStream.Position = 0;
            XmlReader shapeReader = UtilityMethods.CreateReader(shapeStream);
            WTextBox textbox = new WTextBox(m_doc);
            if (drawingStream != null)
            {
                drawingStream.Position = 0;
                XmlReader reader = UtilityMethods.CreateReader(drawingStream);
                ParseTextboxProperties(reader, textbox);
            }
            ParseTextboxShape(shapeReader, textbox);
            return textbox;
        }
        /// <summary>
        /// Parse Text Box Properties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="textbox"></param>
        private void ParseTextboxProperties(XmlReader reader, WTextBox textbox)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "drawing")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            string value;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "drawing")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "wrapSquare":
                            textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.Square;
                            break;
                        case "wrapTight":
                            textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                            ParseWrapPolygon(reader, textbox);
                            break;
                        case "wrapThrough":
                            textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.Through;
                            ParseWrapPolygon(reader, textbox);
                            break;
                        case "wrapTopAndBottom":
                            textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.TopAndBottom;
                            break;
                        case "wrapNone":
                            if (textbox.TextBoxFormat.IsBelowText)
                                textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.Behind;
                            else
                                textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                            break;
                        case "anchor":
                            if (reader.AttributeCount == 0)
                                break;
                            value = reader.GetAttribute("behindDoc");
                            textbox.TextBoxFormat.IsBelowText = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("allowOverlap");
                            if (value != null)
                                textbox.TextBoxFormat.AllowOverlap = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("relativeHeight");
                            textbox.TextBoxFormat.OrderIndex = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case "inline":
                            textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.Inline;
                            break;
                        case "bodypr":
                            if (reader.AttributeCount == 0)
                                break;
                            value = reader.GetAttribute("vert");
                            if (value == "vert")
                                textbox.TextBoxFormat.TextDirection = TextDirection.VerticalTopToBottom;
                            else if (value == "vert270")
                                textbox.TextBoxFormat.TextDirection = TextDirection.VerticalBottomToTop;
                            value = reader.GetAttribute("anchor");
                            textbox.TextBoxFormat.TextVerticalAlignment = GetTextVertAlign(value);

                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        private void ParseTextboxShape(XmlReader reader, IEntity entity)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;

            reader.Read();
            SkipWhitespaces(reader);

            while (!reader.EOF && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "shape":
                        case "rect":
                        case "roundrect":
                            ParseShapeProperties(reader, entity);
                            ParseTextbox(reader, entity);
                            //ParseTextBoxWrappingStyle(reader, entity);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse the Text Box Wrapping Style
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        private void ParseTextBoxWrappingStyle(XmlReader reader, IEntity entity)
        {
            if (reader.IsEmptyElement)
                return;
            string endNode = reader.LocalName;
            reader.Read();
            bool skip = false;
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "wrap":
                            string wrapStyle = reader.GetAttribute("type");
                            if (wrapStyle != null)
                                (entity as WTextBox).TextBoxFormat.TextWrappingStyle = GetWrapStyle(wrapStyle);
                            // Parse Textwrap Type
                            string wrapType = reader.GetAttribute("side");
                            if (wrapType != null)
                                (entity as WTextBox).TextBoxFormat.TextWrappingType = GetTextWrapType(wrapType);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        private void ParseTextbox(XmlReader reader, IEntity entity)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            bool skip = false;
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "stroke":
                            ParseStroke(reader, entity as WTextBox);
                            break;
                        case "textbox":
                            ParseTextboxIntMargins(reader, entity as WTextBox);
                            ParseTextBoxStyle(reader, entity as WTextBox);
                            m_currentRunFormat = null;
                            ParseTextboxContent(reader, entity);
                            break;
                        case "fill":
                            skip = ParseFillEffects(reader, entity as WTextBox);
                            break;
                        case "wrap":
                            string wrapStyle = reader.GetAttribute("type");
                            if (wrapStyle != null)
                                (entity as WTextBox).TextBoxFormat.TextWrappingStyle = GetWrapStyle(wrapStyle);
                            // Parse Textwrap Type
                            string wrapType = reader.GetAttribute("side");
                            if (wrapType != null)
                                (entity as WTextBox).TextBoxFormat.TextWrappingType = GetTextWrapType(wrapType);

                            break;
                        default:
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parses the fill effect.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="txbx">The Textbox.</param>
        /// <returns></returns>
        private bool ParseFillEffects(XmlReader reader, WTextBox textbox)
        {
            bool skip = false;
            string fillType = reader.GetAttribute("type");
            if (fillType != null)
            {
                switch (fillType)
                {
                    case "tile":
                    case "frame":
                        ParsePictureFill(reader, textbox, fillType);
                        break;
                    case "gradient":
                    case "gradientRadial":
                        ParseGradientFill(reader, textbox.TextBoxFormat.FillEfects);
                        break;
                    case "pattern":
                        ParsePatternFill(reader, textbox.TextBoxFormat.FillEfects);
                        skip = true;
                        break;
                }
            }

            string fillOpacity = reader.GetAttribute("opacity");
            if (fillOpacity == "0")
                textbox.TextBoxFormat.FillColor = Color.Empty;

            return skip;
        }
        /// <summary>
        /// Parses the gradient fill.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="background">The background.</param>
        private void ParseGradientFill(XmlReader reader, Background background)
        {
            string type = reader.GetAttribute("type");
            background.Type = BackgroundType.Gradient;
            ParseGradientColor(reader, background);

            if (type == "gradientRadial")
                ParseRadialGradient(reader, background.Gradient);
            else
                ParseGradient(reader, background.Gradient);
        }
        /// <summary>
        /// Parses the color of the gradient.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="background">The background.</param>
        private void ParseGradientColor(XmlReader reader, Background background)
        {
            BackgroundGradient gradient = background.Gradient;

            if (background.Color == Color.Empty)
                gradient.Color1 = Color.White;
            else
                gradient.Color1 = background.Color;


            string color2 = reader.GetAttribute("color2");
            if (color2 == null)
                gradient.Color2 = Color.Black;
            else
                gradient.Color2 = GetColorValue(color2);
        }
        /// <summary>
        /// Parses the gradient style.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="gradient">The gradient.</param>
        private void ParseGradient(XmlReader reader, BackgroundGradient gradient)
        {
            string focus = reader.GetAttribute("focus");
            string angle = reader.GetAttribute("angle");

            if (angle == null)
                gradient.ShadingStyle = GradientShadingStyle.Horizontal;
            else if (angle == "-90")
                gradient.ShadingStyle = GradientShadingStyle.Vertical;
            else if (angle == "-135")
                gradient.ShadingStyle = GradientShadingStyle.DiagonalUp;
            else if (angle == "-45")
                gradient.ShadingStyle = GradientShadingStyle.DiagonalDown;

            gradient.ShadingVariant = ParseShadingVariant(focus);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="gradient"></param>
        private void ParseRadialGradient(XmlReader reader, BackgroundGradient gradient)
        {
            string focusPos = reader.GetAttribute("focusposition");
            string focus = reader.GetAttribute("focus");
            string innerXml = reader.ReadInnerXml();

            if (innerXml != string.Empty)
            {
                gradient.ShadingStyle = GradientShadingStyle.FromCorner;

                if (focusPos == null)
                    gradient.ShadingVariant = GradientShadingVariant.ShadingUp;
                else if (focusPos == "1")
                    gradient.ShadingVariant = GradientShadingVariant.ShadingDown;
                else if (focusPos == "1,1")
                    gradient.ShadingVariant = GradientShadingVariant.ShadingMiddle;
                else if (focusPos == ",1")
                    gradient.ShadingVariant = GradientShadingVariant.ShadingOut;
            }
            else
            {
                gradient.ShadingStyle = GradientShadingStyle.FromCenter;
                gradient.ShadingVariant = ParseShadingVariant(focus);
            }
        }
        /// <summary>
        /// Parses the shading variant.
        /// </summary>
        /// <param name="focus">The focus.</param>
        /// <returns></returns>
        private GradientShadingVariant ParseShadingVariant(string focus)
        {
            if (focus == null)
                return GradientShadingVariant.ShadingDown;
            else if (focus == "100%")
                return GradientShadingVariant.ShadingUp;
            else if (focus == "50%")
                return GradientShadingVariant.ShadingMiddle;
            else
                return GradientShadingVariant.ShadingOut;
        }
        /// <summary>
        /// Parses the pattern fill.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="background">The background.</param>
        private void ParsePatternFill(XmlReader reader, Background background)
        {
            string imageId = reader.GetAttribute("id", DocxConstants.R_namespace);
            background.PatternFill = ReadSingleNodeIntoStream(reader);

            if (imageId != null)
            {
                bool isHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;
                string imageName = GetImageName(imageId, isHeaderFooter, false);
                background.PatternImageBytes = GetImageBytes(imageName);
            }
        }
        private void ParsePatternFill(XmlReader reader, FillFormat fillFormat)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            uint opacity = uint.MaxValue;
            SkipWhitespaces(reader);
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "fgClr":
                            fillFormat.ForeColor = ParseColor(reader, "fgClr", ref opacity);
                            opacity = uint.MaxValue;
                            break;
                        case "bgClr":
                            fillFormat.Color = ParseColor(reader, "bgClr", ref opacity);
                            if (opacity != uint.MaxValue)
                                fillFormat.Transparency = (float)Math.Round((1 - ((float)opacity / 65536)), 2) * 100;
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the picture fill.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="txbx">The textbox.</param>
        /// <param name="fillType">Type of the fill.</param>
        private void ParsePictureFill(XmlReader reader, WTextBox textbox, string fillType)
        {
            string fillId = reader.GetAttribute("id", DocxConstants.R_namespace);

            if (fillId == null)
                return;

            bool isHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;

            if (fillType == "frame")
                textbox.TextBoxFormat.FillEfects.Type = BackgroundType.Picture;
            else
                textbox.TextBoxFormat.FillEfects.Type = BackgroundType.Texture;
            string imageName = GetImageName(fillId, isHeaderFooter, false);
            if (ImageIds.ContainsKey(imageName))
                textbox.TextBoxFormat.FillEfects.ImageRecord = m_doc.Images[ImageIds[imageName]];
            else
            {
                textbox.TextBoxFormat.FillEfects.ImageBytes = GetImageBytes(imageName);
                ImageIds.Add(imageName, textbox.TextBoxFormat.FillEfects.ImageRecord.ImageId);
            }
        }
       /// <summary>
       /// Parses the wrap Type.
       /// </summary>
       /// <param name="wrapType">The Wrap type</param>
       /// <returns></returns>
        private TextWrappingType GetTextWrapType(string wrapType)
        {
            switch (wrapType)
            {
                case "left":
                    return TextWrappingType.Left;
                case "right":
                    return TextWrappingType.Right;
                case "largest":
                    return TextWrappingType.Largest;
                default:
                    return TextWrappingType.Both;
            }
        }
        /// <summary>
        /// Parses the wrap style.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="txbx">The textbox.</param>
        private TextWrappingStyle GetWrapStyle(string wrapStyle)
        {
            switch (wrapStyle)
            {
                case "square":
                    return TextWrappingStyle.Square;
                case "tight":
                    return TextWrappingStyle.Tight;
                case "through":
                    return TextWrappingStyle.Through;
                case "topAndBottom":
                    return TextWrappingStyle.TopAndBottom;
                case "none":
                    return TextWrappingStyle.Inline;
                default:
                    return TextWrappingStyle.InFrontOfText;
            }
        }
        /// <summary>
        /// Parses the type of the wrap.
        /// </summary>
        /// <param name="wrapType">Type of the wrap.</param>
        /// <returns></returns>
        private TextWrappingType GetWrapType(string wrapType)
        {
            switch (wrapType)
            {
                case "left":
                    return TextWrappingType.Left;
                case "right":
                    return TextWrappingType.Right;
                default:
                    return TextWrappingType.Both;
            }
        }
        /// <summary>
        /// Parses the content of the textbox.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ent">The entity.</param>
        private void ParseTextboxContent(XmlReader reader, IEntity entity)
        {
            WTextBox textbox = entity as WTextBox;
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;

            reader.Read();


            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "p":
                            IWParagraph para = textbox.TextBoxBody.AddParagraph();
                            ParseParagraphItems(reader, para.Items);
                            break;
                        case "tbl":
                            IWTable table = textbox.TextBoxBody.AddTable();
                            //Set IsAutoResized property as true to layout the table with Auto width (Default in DocX format document)
                            table.TableFormat.IsAutoResized = true;
                            int prevGridCount = m_gridCount;
                            ParseTable(reader, table as WTable);
                            m_gridCount = prevGridCount;
                            break;
                        case "sdt":
                            IStructureDocumentTagBlock sdTagBlock = textbox.TextBoxBody.AddStructureDocumentTag();
                            ParseStructureDocumentTagBlock(reader, sdTagBlock as StructureDocumentTagBlock);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }

        /// <summary>
        /// Parses the text box style.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="textbox">The textbox.</param>
        private void ParseTextBoxStyle(XmlReader reader, WTextBox textbox)
        {
            string style = reader.GetAttribute("style");
            if (style == null)
                return;
            string[] styleProperties = style.Split(';');
            for (int i = 0; i < styleProperties.Length; i++)
            {
                if (styleProperties[i] == DocxConstants.DEF_FIT_TEXT_TO_SHAPE)
                    textbox.TextBoxFormat.FitTextToShape = true;
                else if (styleProperties[i] == "mso-layout-flow-alt:bottom-to-top")
                    textbox.TextBoxFormat.TextDirection = TextDirection.VerticalBottomToTop;
                else if (styleProperties[i] == "layout-flow:vertical" || styleProperties[i] == "layout-flow:vertical-ideographic")
                    textbox.TextBoxFormat.TextDirection = TextDirection.VerticalTopToBottom;
            }
        }
        /// <summary>
        /// Parses the internal margins of textbox.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="txbx">The textbox.</param>
        private void ParseTextboxIntMargins(XmlReader reader, WTextBox textbox)
        {
            string inset = reader.GetAttribute("inset");
            if (inset == null)
                return;

            inset = inset.Replace("mm", string.Empty);
            string[] insetParts = inset.Split(new char[1] { ',' });
            for (int i = 0, cnt = insetParts.Length; i < cnt; i++)
            {
                if (insetParts[i] == string.Empty)
                    continue;

                float margin = GetTextboxMargin(insetParts[i]);
                if (i == 0)
                    textbox.TextBoxFormat.InternalMargin.Left = margin;
                else if (i == 1)
                    textbox.TextBoxFormat.InternalMargin.Top = margin;
                else if (i == 2)
                    textbox.TextBoxFormat.InternalMargin.Right = margin;
                else if (i == 3)
                    textbox.TextBoxFormat.InternalMargin.Bottom = margin;
            }
        }
        /// <summary>
        /// Gets the textbox margin.
        /// </summary>
        /// <param name="margin">The margin.</param>
        /// <returns></returns>
        private float GetTextboxMargin(string margin)
        {
            if (margin == string.Empty)
                return 0;

            float ptMargin = float.MaxValue;

            if (margin.EndsWith("pt"))
            {
                margin = margin.Replace("pt", string.Empty);
                ptMargin = float.Parse(margin, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            else if (margin.EndsWith("in"))
            {
                margin = margin.Replace("in", string.Empty);
                ptMargin = (float)UnitsConvertor.Instance.ConvertUnits(Convert.ToDouble(margin), PrintUnits.Inch, PrintUnits.Point);
            }
            else if (margin.EndsWith("emu"))
            {
                float mmMargin = float.Parse(margin.Replace("emu", string.Empty), CultureInfo.InvariantCulture);
                ptMargin = (float)UnitsConvertor.Instance.ConvertUnits(Convert.ToDouble(mmMargin), PrintUnits.EMU,
                 PrintUnits.Point);
            }
            else
            {
                float mmMargin = float.Parse(margin, NumberStyles.Float, CultureInfo.InvariantCulture);
                ptMargin = (float)UnitsConvertor.Instance.ConvertUnits(mmMargin, PrintUnits.Millimeter,
                  PrintUnits.Point);
            }

            return ptMargin;
        }
        /// <summary>
        /// Gets the textbox margin.
        /// </summary>
        /// <param name="margin">The margin.</param>
        /// <returns></returns>
        private float GetShapeInternalMargin(string margin)
        {
            if (margin == string.Empty)
                return 0;

            float ptMargin = float.MaxValue;

            if (margin.EndsWith("pt"))
            {
                margin = margin.Replace("pt", string.Empty);
                ptMargin = float.Parse(margin, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            else if (margin.EndsWith("in"))
            {
                margin = margin.Replace("in", string.Empty);
                ptMargin = (float)UnitsConvertor.Instance.ConvertUnits(Convert.ToDouble(margin), PrintUnits.Inch, PrintUnits.Point);
            }
            else if (margin.EndsWith("mm"))
            {
                float mmMargin = float.Parse(margin, NumberStyles.Float, CultureInfo.InvariantCulture);
                ptMargin = (float)UnitsConvertor.Instance.ConvertUnits(mmMargin, PrintUnits.Millimeter,
                  PrintUnits.Point);
            }
            else
            {
                float mmMargin = float.Parse(margin.Replace("emu", string.Empty), CultureInfo.InvariantCulture);
                ptMargin = (float)UnitsConvertor.Instance.ConvertUnits(Convert.ToDouble(mmMargin), PrintUnits.EMU,
                 PrintUnits.Point);
            }

            return ptMargin;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="textBox"></param>
        private void ParseStroke(XmlReader reader, WTextBox textBox)
        {
            string dashstyle = reader.GetAttribute("dashstyle");
            if (dashstyle != null)
            {
                textBox.TextBoxFormat.LineDashing = GetDashStyle(dashstyle);
            }

            string lineStyle = reader.GetAttribute("linestyle");
            if (lineStyle != null)
            {
                textBox.TextBoxFormat.LineStyle = GetLineStyle(lineStyle);
            }
        }
        /// <summary>
        /// Parses the dash style.
        /// </summary>
        /// <param name="dashstyle">The dash style.</param>
        /// <returns></returns>
        private LineDashing GetDashStyle(string dashstyle)
        {
            switch (dashstyle)
            {
                case "sysDash":
                    return LineDashing.Dash;
                case "sysDashDot":
                    return LineDashing.DashDot;
                case "sysDashDotDot":
                    return LineDashing.DashDotDot;
                case "sysDot":
                    return LineDashing.Dot;
                case "dash":
                    return LineDashing.DashGEL;
                case "dashDot":
                    return LineDashing.DashDotGEL;
                case "1 1":
                case "dot":
                    return LineDashing.DotGEL;
                case "lgDash":
                case "longDash":
                    return LineDashing.LongDashGEL;
                case "lgDashDot":
                case "longDashDot":
                    return LineDashing.LongDashDotGEL;
                case "lgDashDotDot":
                case "longDashDotDot":
                    return LineDashing.LongDashDotDotGEL;
                default:
                    return LineDashing.Solid;
            }
        }
        /// <summary>
        /// Parses the line style.
        /// </summary>
        /// <param name="lineStyle">The line style.</param>
        /// <returns></returns>
        private TextBoxLineStyle GetLineStyle(string lineStyle)
        {
            switch (lineStyle)
            {
                case "dbl":
                case "thinThin":
                    return TextBoxLineStyle.Double;
                case "thinThick":
                    return TextBoxLineStyle.ThinThick;
                case "thickThin":
                    return TextBoxLineStyle.ThickThin;
                case "thickBetweenThin":
                case "tri":
                    return TextBoxLineStyle.Triple;
                default:
                    return TextBoxLineStyle.Simple;
            }
        }
        private void ParseShapeProperties(XmlReader reader, IEntity entity)
        {
            WTextBox textbox = entity as WTextBox;
            string style = reader.GetAttribute("style");
            if (style == null)
                return;

            style = style.Trim();
            char[] delimeter = new char[1] { ';' };

            string[] styleParts = style.Split(delimeter);
            string[] propertyValues = new string[2];

            for (int i = 0, cnt = styleParts.Length; i < cnt; i++)
            {
                propertyValues = GetPropertyValues(styleParts[i]);

                if (propertyValues == null)
                    continue;
                else
                    ApplyShapeProperties(textbox, propertyValues[0], propertyValues[1]);
            }

            ParseTextboxEffects(reader, textbox);
        }

        private void ParseTextboxEffects(XmlReader reader, WTextBox textbox)
        {
            //parse wrap polygon coordinates
            string wrapCoords = reader.GetAttribute("wrapcoords");
            if (wrapCoords != null)
            {
                textbox.TextBoxFormat.WrapPolygon = new WrapPolygon();
                char[] partSplit = new char[1] { ' ' };
                string[] Coords=wrapCoords.Split(partSplit);
                for (int i = 0; i < Coords.Length-1; i=i+2)
                {
                    float x=float.Parse( Coords[i],CultureInfo.InvariantCulture);
                    float y=float.Parse( Coords[i+1],CultureInfo.InvariantCulture);
                    textbox.TextBoxFormat.WrapPolygon.Vertices.Add(new PointF(x, y));
                }

            }
            // Parse line effects
            string strokeColor = reader.GetAttribute("strokecolor");
            if (strokeColor != null)
                textbox.TextBoxFormat.LineColor = GetColorValue(strokeColor);

            string strokeweight = reader.GetAttribute("strokeweight");
            if (strokeweight != null)
            {
                textbox.TextBoxFormat.LineWidth = GetPointValue(strokeweight);
            }
            // Parse fill effects
            string filled = reader.GetAttribute("filled");
            if (filled != null && filled == "f")
            {
                textbox.TextBoxFormat.FillColor = Color.Empty;
            }
            else
            {
                string fillColor = reader.GetAttribute("fillcolor");
                if (fillColor != null)
                {
                    textbox.TextBoxFormat.FillEfects.Color = GetColorValue(fillColor);
                    textbox.TextBoxFormat.FillEfects.Type = BackgroundType.Color;
                }
            }

            string stroked = reader.GetAttribute("stroked");
            if (stroked != null)
            {
                textbox.TextBoxFormat.NoLine = (stroked == "f") ? true : false;
            }

            string allowInCell = reader.GetAttribute("allowincell", DocxConstants.O_namespace);
            if (allowInCell != null)
            {
                textbox.TextBoxFormat.AllowInCell = (allowInCell == "f") ? false : true;
            }

            string allowOverlap = reader.GetAttribute("allowoverlap", DocxConstants.O_namespace);
            if (allowOverlap != null)
            {
                textbox.TextBoxFormat.AllowOverlap = (allowOverlap == "f")||(allowOverlap=="false") ? false : true;
            }
        }
        /// <summary>
        /// Gets the point value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetPointValue(string value)
        {
            if (value.StartsWith("."))
                value = "0" + value;
            double doubleValue;
            if (value.EndsWith("pt"))
            {
                value = value.Replace("pt", string.Empty);
                doubleValue = Math.Round(double.Parse(value, CultureInfo.InvariantCulture), 2);
            }
            else if (value.EndsWith("mm"))
            {
                value = value.Replace("mm", string.Empty);
                doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                //Converts the millimeter to point.
                doubleValue = Math.Round((doubleValue / 25.4f) * 72, 2);
            }
            else if (value.EndsWith("px"))
            {
                value = value.Replace("px", string.Empty);
                doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                //Converts the pixel to point.
                doubleValue = Math.Round((doubleValue / 4) * 3, 2);
            }
            else if (value.EndsWith("in"))
            {
                value = value.Replace("in", string.Empty);
                doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                //Converts the inch to point.
                doubleValue = Math.Round(doubleValue * 72, 2);
            }
            else if (value.EndsWith("cm"))
            {
                value = value.Replace("cm", string.Empty);
                doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                //Converts the centimeter to point.
                doubleValue = Math.Round((doubleValue / 2.54f) * 72, 2);
            }
            else
            {
                doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                //Converts the emu to point.
                doubleValue = Math.Round(doubleValue / DLSConstants.EmusPerPoint, 2);
            }
            return (float)doubleValue;
        }
        private void ApplyShapeProperties(WTextBox textbox, string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case "margin-left":
                    textbox.TextBoxFormat.HorizontalPosition = GetPointValue(propertyValue);
                    break;
                case "margin-top":
                    textbox.TextBoxFormat.VerticalPosition = GetPointValue(propertyValue);
                    break;
                case "width":
                    textbox.TextBoxFormat.Width = GetPointValue(propertyValue);
                    break;
                case "height":
                    textbox.TextBoxFormat.Height = GetPointValue(propertyValue);
                    break;
                case "z-index":
                    int val = int.Parse(propertyValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    textbox.TextBoxFormat.OrderIndex = val;

                    textbox.TextBoxFormat.IsBelowText = (val > 0) ? false : true;
                    if (textbox.TextBoxFormat.IsBelowText)
                        textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.Behind;
                    else
                        textbox.TextBoxFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                    break;
                case "mso-position-horizontal":
                    textbox.TextBoxFormat.HorizontalAlignment = GetHorizAlign(propertyValue);
                    break;
                case "mso-width-percent":
                    textbox.TextBoxFormat.WidthRelativePercent = ParseFloatVal(propertyValue) / 10;
                    break;
                case "mso-height-percent":
                    textbox.TextBoxFormat.HeightRelativePercent = ParseFloatVal(propertyValue) / 10;
                    break;
                case "mso-position-vertical":
                    textbox.TextBoxFormat.VerticalAlignment = GetVertAlign(propertyValue);
                    break;
                case "v-text-anchor":
                    textbox.TextBoxFormat.TextVerticalAlignment = GetTextVertAlign(propertyValue);
                    break;
                case "mso-height-relative":
                    textbox.TextBoxFormat.HeightOrigin = GetHeightOrigin(propertyValue);
                    break;
                case "mso-width-relative":
                    textbox.TextBoxFormat.WidthOrigin = GetWidthOrigin(propertyValue);
                    break;
                case "mso-position-vertical-relative":
                    textbox.TextBoxFormat.VerticalOrigin = GetVertOrigin(propertyValue);
                    break;
                case "mso-position-horizontal-relative":
                    textbox.TextBoxFormat.HorizontalOrigin = GetHorizOrigin(propertyValue);
                    break;
                case "mso-left-percent":
                    textbox.TextBoxFormat.HorizontalRelativePercent = ParseFloatVal(propertyValue) / 10;
                    break;
                case "mso-top-percent":
                    textbox.TextBoxFormat.VerticalRelativePercent = ParseFloatVal(propertyValue) / 10;
                    break;
                case "mso-wrap-distance-left":
                    textbox.TextBoxFormat.WrapDistanceLeft = GetPointValue(propertyValue);
                    break;
                case "mso-wrap-distance-top":
                    textbox.TextBoxFormat.WrapDistanceTop = GetPointValue(propertyValue);
                    break;
                case "mso-wrap-distance-right":
                    textbox.TextBoxFormat.WrapDistanceRight = GetPointValue(propertyValue);
                    break;
                case "mso-wrap-distance-bottom":
                    textbox.TextBoxFormat.WrapDistanceBottom = GetPointValue(propertyValue);
                    break;
                default:
                    textbox.TextBoxFormat.DocxStyleProps.Add(propertyName + ":" + propertyValue);
                    break;
            }
        }

        private string[] GetPropertyValues(string value)
        {
            char[] partSplit = new char[1] { ':' };
            string[] parts = value.Split(partSplit);
            if (parts.Length == 2)
            {
                return parts;
            }

            return null;
        }

        private ShapeType DetectShapeType(MemoryStream shapeStream, ref AutoShapeType autoShapeType, 
            ref Dictionary<string, Stream> docxProps,ref string shapeTypeId)
        {
            XmlReader reader = UtilityMethods.CreateReader(shapeStream);

            if (reader.LocalName != "pict")
                throw new XmlException("picture shape element");

            string shapeId = null;
            string shapeType = null;
            string imageDataId = null;
            string imageDataHref = null;
            bool hasImageData = false;
            bool hasTextbox = false;
            bool hasTextboxContent = false;

            bool isSkip = false;
            reader.Read();
           
            while (reader.LocalName != "pict" && !reader.EOF)
            {
                isSkip = false;

                SkipWhitespaces(reader);
                if (reader.NodeType == XmlNodeType.Element)
                {

                    switch (reader.LocalName)
                    {
                        case "group":
                            return ShapeType.GroupedShape;
                        case "OLEObject":
                            return ShapeType.OleObject;
                        case "shape":
                            shapeType = reader.GetAttribute("type");
                            shapeId = reader.GetAttribute("id");    
                            break;
                        case "textbox":
                        case "rect":
                            hasTextbox = true;
                            if (reader.LocalName == "rect")
                                autoShapeType = AutoShapeType.Rectangle;
                            break;
                        case "txbxContent":
                            hasTextboxContent = true;
                            break;
                        case "shapetype":
                            shapeTypeId = reader.GetAttribute("id");
                            ReadSingleNodeIntoStream(reader);
                            isSkip = true;
                            break;
                        case "imagedata":
                            hasImageData = true;
                            imageDataId = reader.GetAttribute("id", DocxConstants.R_namespace);
                            imageDataHref = reader.GetAttribute("href", DocxConstants.R_namespace);
                            break;
                        case "roundrect":
                            autoShapeType = AutoShapeType.RoundedRectangle;
                            break;
                        case "oval":
                            autoShapeType = AutoShapeType.Oval;
                            break;
                        case "line":
                            autoShapeType = AutoShapeType.Line;
                            break;
                        case "callout":
                        case "stroke":
                        case "fill":
                        case "shadow":
                        case "extrusion":
                            if (!docxProps.ContainsKey(reader.LocalName))
                                docxProps.Add(reader.LocalName, ReadSingleNodeIntoStream(reader));
                            isSkip = true;
                            break;
                        default:
                            break;
                    }
                }
                if (!isSkip)
                    reader.Read();
            }

            if (shapeId != null && (shapeId.StartsWith("PowerPlusWaterMarkObject") || shapeId.StartsWith("WordPictureWatermark")))
                return ShapeType.WatermarkShape;

            if ((hasTextbox && shapeType == "#_x0000_t202") || (hasTextbox && hasTextboxContent && (shapeType == null || imageDataId != null)))
                return ShapeType.TextboxShape;

            if (shapeTypeId != null && shapeType != null && shapeTypeId == "_x0000_t75" && shapeType == "#_x0000_t75"
                && hasImageData && !hasTextbox)
                return ShapeType.PictureShape;

            if (shapeTypeId == null && shapeType == "#_x0000_t75" && !hasTextbox)
                return ShapeType.PictureShape;

            else if ((imageDataHref != null && imageDataHref != string.Empty) || (imageDataId != null && m_docRelations.ContainsKey(imageDataId)))
                return ShapeType.PictureShape;

            if (!string.IsNullOrEmpty(shapeType))
                autoShapeType = AutoShapeHelper.GetAutoShapeType(shapeType.Replace("#", "").Replace("_x0000_t", ""));
            
            return ShapeType.XmlParagraphItem;
        }
        /// <summary>
        /// Parse TextBox Graphics data
        /// </summary>
        /// <param name="textbox"></param>
        /// <param name="choiceItem"></param>
        private void ParseTextBoxGraphics(WTextBox textbox, XmlParagraphItem choiceItem)
        {
            XmlReader graphicsReader = UtilityMethods.CreateReader(choiceItem.DataNode);
            graphicsReader.ReadToFollowing("fontRef", DocxConstants.A_namespace);
            if (graphicsReader.NodeType != XmlNodeType.None)
            {
                graphicsReader.Read();
                if (graphicsReader.LocalName == "schemeClr")
                {
                    textbox.TextBoxFormat.TextThemeColor = GetSchemeColor(graphicsReader);
                }
            }
        }


        #endregion Shape/Textbox
        private enum GraphicDataContentType
        {
            None,
            Shape,
            Picture,
            Chart,
            Group
        }
        #region Drawings
        /// <summary>
        /// Parse the drawing object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        private ParagraphItem ParseDrawing(XmlReader reader, ParagraphItemCollection paraItems, ref MemoryStream drawingStream)
        {
            if (reader.LocalName != "drawing")
                throw new XmlException("Drawing element");

            drawingStream = ReadSingleNodeIntoStream(reader);

            XmlReader picReader = UtilityMethods.CreateReader(drawingStream);

            while (picReader.NodeType != XmlNodeType.Element)
                picReader.Read();
            GraphicDataContentType type = CheckPicture(picReader);
            if (type == GraphicDataContentType.Chart ||
                type == GraphicDataContentType.Group ||
                type == GraphicDataContentType.None)
            {
                drawingStream.Position = 0;
                return ParseXmlParaItem(drawingStream);
            }

            if (type == GraphicDataContentType.Picture)
            {
                //Reset xml reader to read image hyperlink
                picReader = UtilityMethods.CreateReader(drawingStream);
                ParseImageHyperlink(picReader, paraItems);
            }

            picReader = UtilityMethods.CreateReader(drawingStream);
            while (picReader.NodeType != XmlNodeType.Element)
                picReader.Read();

            if (type == GraphicDataContentType.Picture)
            {
                WPicture picture = new WPicture(m_doc);
                picReader.ReadToFollowing("blip", DocxConstants.A_namespace);
                string embedValue = picReader.GetAttribute("embed", DocxConstants.R_namespace);
                string linkValue = picReader.GetAttribute("link", DocxConstants.R_namespace);
                bool isInHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;
                if (embedValue != null)
                {
                    LoadImage(picture, embedValue, isInHeaderFooter, false);
                }
#if !SILVERLIGHT && !WP
                else if (linkValue != null)
                {
                    Image image = GetLinkedImageBytes(linkValue, isInHeaderFooter, false);
                    if (image != null)
                        picture.LoadImage(image);
                    else
                        return null;
                }
#endif
                else
                    return null;
#if WINRT
                picReader.Dispose();
#else
                picReader.Close();
#endif
                picReader = UtilityMethods.CreateReader(drawingStream);
                return ParsePicture(picReader, picture);
            }
            else
            {
                return ParseShape(picReader, paraItems, drawingStream);
            }


        }

        private ParagraphItem ParseShape(XmlReader picReader, ParagraphItemCollection paraItems, MemoryStream drawingStream)
        {
           if (picReader.ReadToFollowing("custGeom", DocxConstants.A_namespace))
            {
                return ParseXmlParaItem(drawingStream);
            }
            else
            {
                picReader = UtilityMethods.CreateReader(drawingStream);
                if (picReader.ReadToFollowing("prstGeom", DocxConstants.A_namespace))
                {
                    string value = picReader.GetAttribute("prst");
                    if (value != null && value.Length > 0)
                    {
                        AutoShapeConstant autoConst = AutoShapeHelper.GetAutoShapeConstant(value);
                        AutoShapeType type = AutoShapeHelper.GetAutoShapeType(autoConst);
                        if (type == AutoShapeType.Unknown)
                        {
                            return ParseXmlParaItem(drawingStream);
                        }
                    }
                }
            }

            Shape shape = new Shape(m_doc);
            shape.FillFormat.Color = Color.Empty;
            shape.LineFormat.Color = Color.Empty;
            shape.ApplyCharacterFormat(m_currentRunFormat);
            ParseXMLRelations(shape, drawingStream);
            picReader = UtilityMethods.CreateReader(drawingStream);
            picReader.Read();
            WrapFormat wrapFormat = shape.WrapFormat;
            while (!picReader.EOF && picReader.LocalName != "drawing")
            {
                bool skip = false;
                string value;
                SkipWhitespaces(picReader);
                if (picReader.NodeType == XmlNodeType.Element)
                {
                    switch (picReader.LocalName)
                    {
                        case "effectExtent":
                            ReadSingleNodeIntoStream(picReader);
                            skip = true;
                            break;
                        case "extent":
                            value = picReader.GetAttribute("cx");
                            if (value != null)
                                shape.Width = float.Parse(value, CultureInfo.InvariantCulture) / DLSConstants.EmusPerPoint;
                            value = picReader.GetAttribute("cy");
                            if (value != null)
                                shape.Height = float.Parse(value, CultureInfo.InvariantCulture) / DLSConstants.EmusPerPoint;
                            break;
                        case "positionH":
                            ParsePictureHorizontalPosition(picReader, shape);
                            break;
                        case "positionV":
                            ParsePictureVerticalPosition(picReader, shape);
                            break;
                        case "docPr":
                            shape.Title = picReader.GetAttribute("title");
                            shape.AlternativeText = picReader.GetAttribute("descr");
                            shape.ID = XmlConvert.ToInt64(picReader.GetAttribute("id"));
                            shape.Name = picReader.GetAttribute("name");
                            break;
                        case "wrapSquare":
                            wrapFormat.TextWrappingStyle = TextWrappingStyle.Square;
                            ParseWrappingType(picReader, wrapFormat);
                            break;
                        case "wrapTight":
                            wrapFormat.TextWrappingStyle = TextWrappingStyle.Tight;
                            ParseWrappingType(picReader, wrapFormat);
                            ParseWrapPolygon(picReader, shape);
                            break;
                        case "wrapThrough":
                            wrapFormat.TextWrappingStyle = TextWrappingStyle.Through;
                            ParseWrappingType(picReader, wrapFormat);
                            ParseWrapPolygon(picReader, shape);
                            break;
                        case "wrapTopAndBottom":
                            wrapFormat.TextWrappingStyle = TextWrappingStyle.TopAndBottom;
                            break;
                        case "wrapNone":
                            if (shape.IsBelowText)
                                wrapFormat.TextWrappingStyle = TextWrappingStyle.Behind;
                            else
                                wrapFormat.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                            break;
                        case "anchor":
                            if (picReader.AttributeCount == 0)
                                break;
                            if (picReader.MoveToAttribute("distT"))
                                wrapFormat.DistanceTop = GetPointValue(picReader.Value);
                            if (picReader.MoveToAttribute("distB"))
                                wrapFormat.DistanceBottom = GetPointValue(picReader.Value);
                            if (picReader.MoveToAttribute("distL"))
                                wrapFormat.DistanceLeft = GetPointValue(picReader.Value);
                            if (picReader.MoveToAttribute("distR"))
                                wrapFormat.DistanceRight = GetPointValue(picReader.Value);
                            //if (picReader.MoveToAttribute("simplePos"))
                            //    shape.IsSimplePos = XmlConvert.ToBoolean(picReader.Value);
                            if (picReader.MoveToAttribute("relativeHeight"))
                                shape.ZOrderPosition = int.Parse(picReader.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            if (picReader.MoveToAttribute("behindDoc"))
                                shape.IsBelowText = XmlConvert.ToBoolean(picReader.Value);
                            if (picReader.MoveToAttribute("locked"))
                                shape.LockAnchor = XmlConvert.ToBoolean(picReader.Value);
                            if (picReader.MoveToAttribute("layoutInCell"))
                                shape.LayoutInCell = XmlConvert.ToBoolean(picReader.Value);
                            //if (picReader.MoveToAttribute("hidden"))
                            //    shape.Visible = XmlConvert.ToBoolean(picReader.Value);
                            if (picReader.MoveToAttribute("allowOverlap"))
                                wrapFormat.AllowOverlap = XmlConvert.ToBoolean(picReader.Value);
                                break;
                        case "graphic":
                            picReader.Read();
                            ParseGraphicData(picReader, shape);
                            if(shape.AutoShapeType == AutoShapeType.Unknown)
                                return ParseXmlParaItem(drawingStream);
                            break;
                        case "inline":
                            wrapFormat.TextWrappingStyle = TextWrappingStyle.Inline;
                            break;
                        default:
                            //if (picReader.LocalName == "sizeRelV" ||
                            //    picReader.LocalName == "sizeRelH")
                            //    System.Diagnostics.Debugger.Break();

                            ReadSingleNodeIntoStream(picReader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        picReader.Read();
                }
                else
                {
                    picReader.Read();
                }
            }

            if (shape.AutoShapeType != AutoShapeType.Unknown)
                return shape;
            else
                return ParseXmlParaItem(drawingStream);
        }
        /// <summary>
        /// Parse the horizontal position properties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="shape"></param>
        private void ParsePictureHorizontalPosition(XmlReader reader, Shape shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "positionH")
                throw new XmlException("positionH");
            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            string value;
            string relationFromValue = reader.GetAttribute("relativeFrom");
            if (relationFromValue != null)
                shape.HorizontalOrigin = GetHorizOrigin(relationFromValue);
            reader.Read();

            SkipWhitespaces(reader);

            while ( reader.LocalName != "positionH")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "align":
#if !SILVERLIGHT && !WP
                            value = reader.ReadString();
#else 
                            value = reader.ReadInnerXml();
                            skip = true;
#endif
                            if (value != null)
                                shape.HorizontalAlignment = GetHorizAlign(value);
                            break;
                        case "posOffset":
                            float position = float.MaxValue;
#if !SILVERLIGHT && !WP
                            position = float.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
#else 
                            position = float.Parse(reader.ReadInnerXml(), CultureInfo.InvariantCulture);
                            skip = true;
#endif
                            if (position != float.MaxValue)
                                shape.HorizontalPosition = (float)Math.Round(position / DLSConstants.EmusPerPoint, 2);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// parse the vertical position element (positionV)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="shape"></param>
        private void ParsePictureVerticalPosition(XmlReader reader, Shape shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "positionV")
                throw new XmlException("PositionV");
            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            string value;
            string relationFromValue = reader.GetAttribute("relativeFrom");
            shape.VerticalOrigin = GetVertOrigin(relationFromValue);
            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "positionV")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "align":
#if !SILVERLIGHT && !WP
                            value = reader.ReadString();
#else
                            value = reader.ReadInnerXml();
                            skip = true;
#endif
                            if (value != null)
                                shape.VerticalAlignment = GetVertAlign(value);
                            break;
                        case "posOffset":
                            float position = float.MaxValue;
#if !SILVERLIGHT && !WP
                            position = float.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
#else
                            position = float.Parse(reader.ReadInnerXml(), CultureInfo.InvariantCulture);
                            skip = true;
#endif
                            if (position != float.MaxValue)
                                shape.VerticalPosition = (float)Math.Round(position / DLSConstants.EmusPerPoint, 2);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse the wrapping type
        /// </summary>
        /// <param name="wrapFormat"></param>
        /// <param name="reader"></param>
        private void ParseWrappingType(XmlReader reader, WrapFormat wrapFormat)
        {
            string wrappingType = reader.GetAttribute("wrapText");
            if (wrappingType == null)
                return;

            switch (wrappingType)
            {
                case "bothSides":
                    wrapFormat.TextWrappingType = TextWrappingType.Both;
                    break;
                case "left":
                    wrapFormat.TextWrappingType = TextWrappingType.Left;
                    break;
                case "right":
                    wrapFormat.TextWrappingType = TextWrappingType.Right;
                    break;
                case "largest":
                    wrapFormat.TextWrappingType = TextWrappingType.Largest;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Parses the wrap polygon.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="entiry">The entiry.</param>
        /// <exception cref="System.Xml.XmlException">Unexpected xml tag  + reader.LocalName</exception>
        private void ParseWrapPolygon(XmlReader reader, IEntity entiry)
        {
            reader.Read();
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "wrapPolygon")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;
            WrapPolygon wrapPolygon = null;
            if (entiry is WPicture)
            {
                (entiry as WPicture).WrapPolygon = new WrapPolygon();
                wrapPolygon = (entiry as WPicture).WrapPolygon;
            }
            else if (entiry is Shape)
            {
                (entiry as Shape).WrapFormat.WrapPolygon = new WrapPolygon();
                wrapPolygon = (entiry as Shape).WrapFormat.WrapPolygon;
            }
            else if (entiry is WTextBox)
            {
                (entiry as WTextBox).TextBoxFormat.WrapPolygon = new WrapPolygon();
                wrapPolygon = (entiry as WTextBox).TextBoxFormat.WrapPolygon;
            }
            string value = reader.GetAttribute("edited");
            if (value != null)
                wrapPolygon.Edited = (value == "0" ||value == "false") ? false : true;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != "wrapPolygon")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "start":
                        case "lineTo":
                            float x = float.Parse(reader.GetAttribute("x"),CultureInfo.InvariantCulture);
                            float y = float.Parse(reader.GetAttribute("y"),CultureInfo.InvariantCulture);
                            wrapPolygon.Vertices.Add(new PointF(x, y));
                            break;
                        default:
                            break;
                    }
                     reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
            reader.Read();
        }
        /// <summary>
        /// Parses the graphic data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="picture">The picture.</param>
        private void ParseGraphicData(XmlReader reader, Shape shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "graphicData")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != "graphicData")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "xfrm":
                            string value = reader.GetAttribute("flipH");
                            if (value != null)
                                shape.FlipHorizantal = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("flipV");
                            if (value != null)
                                shape.FlipVertical = (value == "1" || value == "true") ? true : false;
                            break;
                        case "prstGeom":
                            value = reader.GetAttribute("prst");
                            if (value != null && value.Length > 0)
                            {
                                AutoShapeConstant autoConst = AutoShapeHelper.GetAutoShapeConstant(value);
                                AutoShapeType type = AutoShapeHelper.GetAutoShapeType(autoConst);
                                shape.AutoShapeType = type;
                            }
                            break;
                        case "avLst":
                            ParseShapeAdjustValues(reader, shape);
                            break;
                        case "custGeom":
                            shape.DocxProps.Add("custGeom", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "ln":
                            ParseLineFormat(reader, shape);
                            // skip = true;
                            break;
                        case "pattFill":
                            shape.FillFormat.Fill = true;
                            shape.FillFormat.FillType = FillType.FillPatterned;
                            value = reader.GetAttribute("prst");
                            shape.FillFormat.Pattern = GetPatternType(value);
                            ParsePatternFill(reader, shape.FillFormat);
                            break;
                        case "gradFill":
                            shape.FillFormat.Fill = true;
                            shape.FillFormat.FillType = FillType.FillGradient;
                            ParseGradientFill(reader, shape.FillFormat.GradientFill);
                            break;
                        case "blipFill":
                            shape.FillFormat.Fill = true;
                            shape.FillFormat.FillType = FillType.FillPicture;
                            ParseBlipFill(reader, shape.FillFormat);
                            break;
                        case "solidFill":
                            shape.FillFormat.Fill = true;
                            shape.FillFormat.FillType = FillType.FillSolid;
                            uint opacity = uint.MaxValue;
                            Color color = ParseColor(reader, "solidFill", ref opacity);
                            shape.FillFormat.Color = color;
                            if(opacity != uint.MaxValue)
                                shape.FillFormat.Transparency = (float)Math.Round((1 - ((float)opacity / 65536)), 2) * 100;
                            break;
                        case "noFill":
                            shape.FillFormat.Fill = false;
                            break;
                        case "effectLst":
                            shape.DocxProps.Add("effectLst", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "scene3d":
                            shape.DocxProps.Add("scene3d", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "sp3d":
                            shape.DocxProps.Add("sp3d", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "style":
                            shape.DocxProps.Add("Style", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "bodyPr":
                            if (reader.AttributeCount == 0)
                                break;
                            value = reader.GetAttribute("vert");
                            shape.TextFrame.TextDirection = GetTextDirection(value);

                            value = reader.GetAttribute("anchor");
                            shape.TextFrame.TextVerticalAlignment = GetTextVertAlign(value);
                            value = reader.GetAttribute("lIns");
                            if (value != null)
                                shape.TextFrame.InternalMargin.Left = GetShapeInternalMargin(value);
                            value = reader.GetAttribute("tIns");
                            if (value != null)
                                shape.TextFrame.InternalMargin.Top = GetShapeInternalMargin(value);
                            value = reader.GetAttribute("rIns");
                            if (value != null)
                                shape.TextFrame.InternalMargin.Right = GetShapeInternalMargin(value);
                            value = reader.GetAttribute("bIns");
                            if (value != null)
                                shape.TextFrame.InternalMargin.Bottom = GetShapeInternalMargin(value);
                            shape.DocxProps.Add("BodyPr", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "txbx":
                            reader.Read();
                            SkipWhitespaces(reader);
                            WTextBox txtBox = new WTextBox(m_doc);
                            m_currentRunFormat = null;
                            ParseTextboxContent(reader, txtBox);
                            shape.TextBody = txtBox.TextBoxBody;
                            shape.TextBody.SetOwner(shape);
                            reader.Read();
                            break;
                        case "wgp":
                        case "grpSp":
                            shape.AutoShapeType = AutoShapeType.Unknown;
                            return;
                        case "extLst":
                            shape.DocxProps.Add("extLst", ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the graphic data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="picture">The picture.</param>
        private void ParseShapeAdjustValues(XmlReader reader, Shape shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "avLst")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != "avLst")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "gd":
                            string guideName = reader.GetAttribute("name");
                            string guideValue = reader.GetAttribute("fmla");
                            if (!string.IsNullOrEmpty(guideName) && !string.IsNullOrEmpty(guideValue))
                                shape.ShapeGuide.Add(guideName, guideValue);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        private void ParseBlipFill(XmlReader reader, FillFormat fillFormat)
        {
            if (reader.IsEmptyElement)
                return;

            fillFormat.RotateWithObject = GetBoolValue(reader.GetAttribute("rotWithShape"));
            //fillFormat.DPI
            string endNode = reader.LocalName;
            string value = string.Empty;
            reader.Read();
            uint opacity = uint.MaxValue;
            SkipWhitespaces(reader);
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "blip":
                            {
                                ParseBlipImage(reader, fillFormat);
                            }
                            break;
                        case "srcRect":
                            // attributes - b (Bottom Offset), l (Left Offset), r (Right Offset), t (Top Offset)
                            value = reader.GetAttribute("b");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.SourceRectangle.BottomOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("l");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.SourceRectangle.LeftOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("r");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.SourceRectangle.RightOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("t");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.SourceRectangle.TopOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            break;
                        case "stretch":
                            ParseFillRectangle(reader, fillFormat);
                            break;
                        case "tile":
                            //<a:tile tx="425450" ty="50800" sx="62000" sy="77000" flip="y" algn="tr" />
                            fillFormat.TextureTile = true;
                            fillFormat.TextureAlignment = GetTextureAlignment(reader.GetAttribute("algn"));
                            double tx = 0, ty = 0, sx = 0, sy = 0;
                            value = reader.GetAttribute("tx");
                            if (!string.IsNullOrEmpty(value))
                            {
                                tx = double.Parse(value, CultureInfo.InvariantCulture);
                                fillFormat.TextureOffsetX = tx / DLSConstants.EmusPerPoint;
                            }
                            value = reader.GetAttribute("ty");
                            if (!string.IsNullOrEmpty(value))
                            {
                                ty = double.Parse(reader.GetAttribute("ty"), CultureInfo.InvariantCulture);
                                fillFormat.TextureOffsetY = ty / DLSConstants.EmusPerPoint;
                            }
                            value = reader.GetAttribute("sx");
                            if (!string.IsNullOrEmpty(value))
                            {
                                sx = double.Parse(reader.GetAttribute("sx"), CultureInfo.InvariantCulture);
                                fillFormat.TextureHorizontalScale = sx / DLSConstants.ThousandthsUnit;
                            }
                            value = reader.GetAttribute("sy");
                            if (!string.IsNullOrEmpty(value))
                            {
                                sy = double.Parse(value, CultureInfo.InvariantCulture);
                                fillFormat.TextureVerticalScale = sy / DLSConstants.ThousandthsUnit;
                            }
                            fillFormat.FlipOrientation = GetFlipOrientation(reader.GetAttribute("flip"));

                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }

        private void ParseBlipImage(XmlReader reader, FillFormat fillFormat)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            string value = string.Empty;
            //
            string embedValue = reader.GetAttribute("embed", DocxConstants.R_namespace);
            string linkValue = reader.GetAttribute("link", DocxConstants.R_namespace);
            bool isInHeaderFooter = (m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer")) ? true : false;
            if (embedValue != null)
            {
                string imageName = GetImageName(embedValue, isInHeaderFooter, false);
                if (ImageIds.ContainsKey(imageName))
                {
                    fillFormat.ImageRecord = new ImageRecord(m_doc, m_doc.Images[ImageIds[imageName]]);
                    fillFormat.ImageRecord.OccurenceCount++;
                }
                else
                {
                    byte[] imageBytes = GetImageBytes(imageName);
                    fillFormat.ImageRecord = new ImageRecord(m_doc, imageBytes);
                }
            }
#if !SILVERLIGHT && !WP
            else if (linkValue != null)
            {
                Image image = GetLinkedImageBytes(linkValue, isInHeaderFooter, false);
                MemoryStream ms = new MemoryStream();
                image.Save(ms, image.RawFormat);
                byte[] imageArray = ms.ToArray();
                fillFormat.ImageRecord = new ImageRecord(m_doc, imageArray);
            }
#endif
            //
            reader.Read();
            uint opacity = uint.MaxValue;
            SkipWhitespaces(reader);
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "alphaModFix":
                            {
                                value = reader.GetAttribute("amt");
                                if (!string.IsNullOrEmpty(value))
                                    fillFormat.Transparency = (float)Math.Round((100 - (float.Parse(value) / DLSConstants.ThousandthsUnit)));
                            }
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }

        private void ParseFillRectangle(XmlReader reader, FillFormat fillFormat)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            string value = string.Empty;
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {

                        case "fillRect":
                            // attributes - b (Bottom Offset), l (Left Offset), r (Right Offset), t (Top Offset)
                            value = reader.GetAttribute("b");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.FillRectangle.BottomOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("l");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.FillRectangle.LeftOffset = int.Parse(reader.GetAttribute("l")) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("r");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.FillRectangle.RightOffset = int.Parse(reader.GetAttribute("r")) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("t");
                            if (!string.IsNullOrEmpty(value))
                                fillFormat.FillRectangle.TopOffset = int.Parse(reader.GetAttribute("t")) / DLSConstants.ThousandthsUnit;
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }

        private TextureAlignment GetTextureAlignment(string textureAlign)
        {
            switch (textureAlign)
            {
                case "b":
                    //b (Rectangle Alignment Enum ( Bottom )) Bottom
                    return TextureAlignment.Bottom;
                    break;

                //bl (Rectangle Alignment Enum ( Bottom Left )) Bottom Left
                case "bl":
                    return TextureAlignment.BottomLeft;
                    break;

                // br (Rectangle Alignment Enum ( Bottom Right )) Bottom Right
                case "br":
                    return TextureAlignment.BottomRight;
                    break;
                //ctr (Rectangle Alignment Enum ( Center )) Center
                case "ctr":
                    return TextureAlignment.Center;
                    break;
                //l (Rectangle Alignment Enum ( Left )) Left
                case "l":
                    return TextureAlignment.Left;
                    break;
                //r (Rectangle Alignment Enum ( Right )) Right
                case "r":
                    return TextureAlignment.Right;
                    break;
                //t (Rectangle Alignment Enum ( Top )) Top
                case "t":
                    return TextureAlignment.Top;
                    break;
                //tl (Rectangle Alignment Enum ( Top Left )) Top Left
                case "tl":
                    return TextureAlignment.TopLeft;
                    break;
                //tr (Rectangle Alignment Enum ( Top Right )) Top Right
                case "tr":
                    return TextureAlignment.TopRight;
                    break;
            }


            return TextureAlignment.AlignmentMixed;
        }

        private PatternType GetPatternType(string value)
        {
            switch (value)
            {
                case "cross":
                    return PatternType.Cross;//cross (Cross)
                    break;
                case "dashDnDiag":
                    return PatternType.DashedDownwardDiagonal;//dashDnDiag (Dashed Downward Diagonal)
                    break;
                case "dashHorz":
                    return PatternType.DashedHorizontal;//dashHorz (Dashed Horizontal)
                    break;
                case "dashUpDiag":
                    return PatternType.DashedUpwardDiagonal;//dashUpDiag (Dashed Upward DIagonal)
                    break;
                case "dashVert":
                    return PatternType.DashedVertical;//dashVert (Dashed Vertical)
                    break;
                case "diagBrick":
                    return PatternType.DiagonalBrick;//diagBrick (Diagonal Brick)
                    break;
                case "diagCross":
                    return PatternType.DiagonalCross;//diagCross (Diagonal Cross)
                    break;
                case "divot":
                    return PatternType.Divot;//divot (Divot)
                    break;
                case "dkDnDiag":
                    return PatternType.DarkDownwardDiagonal;//dkDnDiag (Dark Downward Diagonal)
                    break;
                case "dkHorz":
                    return PatternType.DarkHorizontal;//dkHorz (Dark Horizontal)
                    break;
                case "dkUpDiag":
                    return PatternType.DarkUpwardDiagonal;//dkUpDiag (Dark Upward Diagonal)
                    break;
                case "dkVert":
                    return PatternType.DarkVertical;//dkVert (Dark Vertical)
                    break;
                case "dnDiag":
                    return PatternType.DownwardDiagonal;//dnDiag (Downward Diagonal)
                    break;
                case "dotDmnd":
                    return PatternType.DottedDiamond;//dotDmnd (Dotted Diamond)
                    break;
                case "dotGrid":
                    return PatternType.DottedGrid;//dotGrid (Dotted Grid)
                    break;
                case "horz":
                    return PatternType.Horizontal;//horz (Horizontal)
                    break;
                case "horzBrick":
                    return PatternType.HorizontalBrick;//horzBrick (Horizontal Brick)
                    break;
                case "lgCheck":
                    return PatternType.LargeCheckerBoard;//lgCheck (Large Checker Board)
                    break;
                case "lgConfetti":
                    return PatternType.LargeConfetti;//lgConfetti (Large Confetti)
                    break;
                case "lgGrid":
                    return PatternType.LargeGrid;//lgGrid (Large Grid)
                    break;
                case "ltDnDiag":
                    return PatternType.LightDownwardDiagonal;//ltDnDiag (Light Downward Diagonal)
                    break;
                case "ltHorz":
                    return PatternType.LightHorizontal;//ltHorz (Light Horizontal)
                    break;
                case "ltUpDiag":
                    return PatternType.LightUpwardDiagonal;//ltUpDiag (Light Upward Diagonal)
                    break;
                case "ltVert":
                    return PatternType.LightVertical;//ltVert (Light Vertical)
                    break;
                case "narHorz":
                    return PatternType.NarrowHorizontal;//narHorz (Narrow Horizontal)
                    break;
                case "narVert":
                    return PatternType.NarrowVertical;//narVert (Narrow Vertical)
                    break;
                case "openDmnd":
                    return PatternType.OutlinedDiamond;//openDmnd (Open Diamond)
                    break;
                case "pct10":
                    return PatternType.Pattern10Percent;//pct10 (10%)
                    break;
                case "pct20":
                    return PatternType.Pattern20Percent;//pct20 (20%)
                    break;
                case "pct25":
                    return PatternType.Pattern25Percent;//pct25 (25%)
                    break;
                case "pct30":
                    return PatternType.Pattern30Percent;//pct30 (30%)
                    break;
                case "pct40":
                    return PatternType.Pattern40Percent;//pct40 (40%)
                    break;
                case "pct5":
                    return PatternType.Pattern5Percent;//pct5 (5%)
                    break;
                case "pct50":
                    return PatternType.Pattern50Percent;//pct50 (50%)
                    break;
                case "pct60":
                    return PatternType.Pattern60Percent;//pct60 (60%)
                    break;
                case "pct70":
                    return PatternType.Pattern70Percent;//pct70 (70%)
                    break;
                case "pct75":
                    return PatternType.Pattern75Percent;//pct75 (75%)
                    break;
                case "pct80":
                    return PatternType.Pattern80Percent;//pct80 (80%)
                    break;
                case "pct90":
                    return PatternType.Pattern90Percent;//pct90 (90%)
                    break;
                case "Plaid":
                    return PatternType.Plaid;//plaid (Plaid)
                    break;
                case "shingle":
                    return PatternType.Shingle;//shingle (Shingle)
                case "smCheck":
                    return PatternType.SmallCheckerBoard;//smCheck (Small Checker Board)
                    break;
                case "smConfetti":
                    return PatternType.SmallConfetti;//smConfetti (Small Confetti)
                    break;
                case "smGrid":
                    return PatternType.SmallGrid;//smGrid (Small Grid)
                    break;
                case "solidDmnd":
                    return PatternType.SolidDiamond;//solidDmnd (Solid Diamond)
                    break;
                case "sphere":
                    return PatternType.Sphere;//sphere (Sphere)
                    break;
                case "trellis":
                    return PatternType.Trellis;//trellis (Trellis)
                    break;
                case "upDiag":
                    return PatternType.UpwardDiagonal;//upDiag (Upward Diagonal)
                    break;
                case "vert":
                    return PatternType.Vertical;//vert (Vertical)
                    break;
                case "wave":
                    return PatternType.Wave;//wave (Wave)
                    break;
                case "wdDnDiag":
                    return PatternType.WideDownwardDiagonal;//wdDnDiag (Wide Downward Diagonal)
                    break;
                case "wdUpDiag":
                    return PatternType.WideUpwardDiagonal;//wdUpDiag (Wide Upward Diagonal)
                    break;
                case "weave":
                    return PatternType.Weave;//weave (Weave)
                    break;
                case "zigZag":
                    return PatternType.ZigZag;//zigZag (Zig Zag)
                    break;
                default:
                    return PatternType.Pattern5Percent;
                    break;
            }
            return PatternType.Pattern5Percent;
        }
        

        private TextDirection GetTextDirection(string value)
        {
            if (value == "vert")
                return TextDirection.VerticalTopToBottom;
            else if (value == "vert270")
                return TextDirection.VerticalBottomToTop;
            else
                return TextDirection.Horizontal;
        }

        private void ParseLineFormat(XmlReader reader, Shape shape)
        {
            string value = reader.GetAttribute("w");
            if (value != null)
            {
                float width = float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
                width = width / DLSConstants.EmusPerPoint;
                shape.LineFormat.Weight = width;
            }
            value = reader.GetAttribute("cmpd");
            if (value != null)
            {
                shape.LineFormat.Style = GetShapeOutLineStyle(value);
            }
            value = reader.GetAttribute("algn");
            if (value == "in")
                shape.LineFormat.InsetPen = true;
            value = reader.GetAttribute("cap");
            if (value != null)
                shape.LineFormat.LineCap = GetLineCapStyle(value);
            ParseLineProps(reader, shape);
        }
        private LineStyle GetShapeOutLineStyle(string value)
        {
            switch (value)
            {
                // dbl (Double Lines) Double lines of equal width
                case "dbl":
                case "thinThin":
                    return LineStyle.ThinThin; //TextBoxLineStyle.Double
                    break;
                //thinThick (Thin Thick Double Lines) Double lines: one thin, one thick
                case "thinThick":
                    return LineStyle.ThinThick; //TextBoxLineStyle.ThinThick;
                    break;
                //thickThin (Thick Thin Double Lines) Double lines: one thick, one thin
                case "thickThin":
                    return LineStyle.ThickThin;//TextBoxLineStyle.ThickThin;
                    break;
                //tri (Thin Thick Thin Triple Lines) Three lines: thin, thick, thin
                case "thickBetweenThin":
                case "tri":
                    return LineStyle.ThickBetweenThin;
                    break;
                //sng (Single Line) Single line: one normal width
                default:
                    return LineStyle.Single;
                    break;
            }
        }
        /// <summary>
        /// Parse the Image Hyperlink
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="para">Paragraph</param>
        private void ParseImageHyperlink(XmlReader reader, ParagraphItemCollection paraItems)
        {
            reader.ReadToFollowing("hlinkClick", DocxConstants.A_namespace);
            if (reader.LocalName == "hlinkClick" && !m_isInHeyperlinkField)
                ParseHyperlink(reader, paraItems);
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Download image from url
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Image</returns>
        private Image DownloadImage(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            Image image = null;

            try
            {
                if (url.StartsWith("http") || url.StartsWith("www"))
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    httpWebRequest.AllowWriteStreamBuffering = true;

                    WebResponse webResponse = httpWebRequest.GetResponse();
                    Stream webStream = webResponse.GetResponseStream();
                    image = Image.FromStream(webStream);

                    webResponse.Close();
                }
                else
                {
                    if (url.StartsWith("file:///"))
                        url = url.Replace("file:///", string.Empty);
                    image = Image.FromFile(url);
                }
            }
            catch
            {
                new FileLoadException("Can't load image, on this url: " + url);
            }

            return image;
        }
#endif
        /// <summary>
        /// Parse Xml paragraph item
        /// </summary>
        /// <param name="picReader"></param>
        /// <returns></returns>
        private XmlParagraphItem ParseXmlParaItem(Stream XmlParaItemStream)
        {
            XmlParagraphItem xmlParaItem = new XmlParagraphItem(XmlParaItemStream, m_doc);
            xmlParaItem.ApplyCharacterFormat(m_currentRunFormat);

            List<string> relationshipIds = FindRelationshipIds(XmlParaItemStream);

            if (relationshipIds.Count > 0 && m_docRelations != null)
            {
                ParseShapeRelationId(xmlParaItem, relationshipIds);

                for (int i = 0, count = relationshipIds.Count; i < count; i++)
                {
                    string id = relationshipIds[i];
                    bool isImageRelation = ParseImageRelation(xmlParaItem, id);
                    if (!isImageRelation)
                    {
                        DictionaryEntry entry = new DictionaryEntry();
                        if (m_currentFile != null && m_currentFile != string.Empty)
                        {
                            Dictionary<string, DictionaryEntry> rels = GetFileRelations(m_currentFile);
                            if (rels != null)
                                entry = rels[id];
                        }
                        else if (m_docRelations.ContainsKey(id))
                            entry = m_docRelations[id];
                        if (!xmlParaItem.Relations.ContainsKey(id))
                            xmlParaItem.Relations.Add(id, entry);
                    }
                }
            }

            return xmlParaItem;
        }
       
        private bool ParseImageRelation(Shape shape, string id)
        {
            ImageRecord imageRecord = null;
            bool isHeaderFooter = m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer");
            string imageName = GetImageName(id, isHeaderFooter, false);
            if (ImageIds.ContainsKey(imageName))
            {
                imageRecord = m_doc.Images[ImageIds[imageName]];
                imageRecord.OccurenceCount++;
            }
            else
            {
                byte[] imageBytes = GetImageBytes(imageName);
                if (imageBytes != null
                    && imageBytes.Length > 0)
                {
                    imageRecord = m_doc.Images.LoadXmlItemImage(imageBytes);
                    ImageIds.Add(imageName, imageRecord.ImageId);
                }
            }
            if (imageRecord != null && !shape.ImageRelations.ContainsKey(id))
                shape.ImageRelations.Add(id, imageRecord);
            return imageRecord != null;
        }
        /// <summary>
        /// Parses the image relation.
        /// </summary>
        /// <param name="xmlParaItem">The XML para item.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private bool ParseImageRelation(XmlParagraphItem xmlParaItem, string id)
        {
            ImageRecord imageRecord = null;
            bool isHeaderFooter = m_currentFile.StartsWith("header") || m_currentFile.StartsWith("footer");
            string imageName = GetImageName(id, isHeaderFooter, false);
            if (ImageIds.ContainsKey(imageName))
            {
                imageRecord = m_doc.Images[ImageIds[imageName]];
                imageRecord.OccurenceCount++;
            }
            else
            {
                byte[] imageBytes = GetImageBytes(imageName);
                if (imageBytes != null
                    && imageBytes.Length > 0)
                {
                    imageRecord = m_doc.Images.LoadXmlItemImage(imageBytes);
                    ImageIds.Add(imageName, imageRecord.ImageId);
                }
            }
            if (imageRecord != null && !xmlParaItem.ImageRelations.ContainsKey(id))
                xmlParaItem.ImageRelations.Add(id, imageRecord);
            return imageRecord != null;
        }
        /// <summary>
        /// Get the relationship ids present within the XmlparaItem stream
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
                reader.Read();
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
            }
            while (!reader.EOF);

            return relationIds;
        }
        /// <summary>
        /// Parses the shape hyperlinks id.
        /// </summary>
        private void ParseShapeRelationId(XmlParagraphItem xmlItem, List<String> relationIds)
        {
            for (int i = 0, count = relationIds.Count; i < count; i++)
            {
                string id = relationIds[i];
                if (IsExternalHyperlink.ContainsKey(id))
                    xmlItem.m_shapeHyperlink = id;
            }
        }
        /// <summary>
        /// Parse the picture
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        private ParagraphItem ParsePicture(XmlReader reader, WPicture picture)
        {
            if (m_currentRunFormat != null)
                picture.PictureCharacterFormat.ImportContainer(m_currentRunFormat);
            m_currentRunFormat = null;

            ParsePictureProperties(reader, picture);

            return picture;
        }
        /// <summary>
        /// Parse the picture properties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="picture"></param>
        private void ParsePictureProperties(XmlReader reader, WPicture picture)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "drawing")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            string value;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != "drawing")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "effectExtent":
                            picture.DocxProps.Add(ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        case "extent":
                            value = reader.GetAttribute("cx");
                            if (value != null)
                            {
                                if (picture.Width == float.MinValue)
                                    picture.Width = float.Parse(value, CultureInfo.InvariantCulture) / DLSConstants.EmusPerPoint;
                                else
                                    picture.WidthScale = float.Parse(value, CultureInfo.InvariantCulture) / DLSConstants.EmusPerPoint * 100 / picture.Width;
                            }
                            value = reader.GetAttribute("cy");
                            if (value != null)
                            {
                                if (picture.Height == float.MinValue)
                                    picture.Height = float.Parse(value, CultureInfo.InvariantCulture) / DLSConstants.EmusPerPoint;
                                else
                                    picture.HeightScale = float.Parse(value, CultureInfo.InvariantCulture) / DLSConstants.EmusPerPoint * 100 / picture.Height;
                            }
                            break;
                        case "positionH":
                            ParsePictureHorizontalPosition(reader, picture);
                            break;
                        case "positionV":
                            ParsePictureVerticalPosition(reader, picture);
                            break;
                        case "docPr":
                            picture.Title = reader.GetAttribute("title");
                            picture.AlternativeText = reader.GetAttribute("descr");
                            break;
                        case "wrapSquare":
                            picture.TextWrappingStyle = TextWrappingStyle.Square;
                            ParseWrappingType(reader, picture);
                            break;
                        case "wrapTight":
                            picture.TextWrappingStyle = TextWrappingStyle.Tight;
                            ParseWrappingType(reader, picture);
                            ParseWrapPolygon(reader, picture);
                            break;
                        case "wrapThrough":
                            picture.TextWrappingStyle = TextWrappingStyle.Through;
                            ParseWrappingType(reader, picture);
                            ParseWrapPolygon(reader, picture);
                            break;
                        case "wrapTopAndBottom":
                            picture.TextWrappingStyle = TextWrappingStyle.TopAndBottom;
                            break;
                        case "wrapNone":
                            if (picture.IsBelowText)
                                picture.TextWrappingStyle = TextWrappingStyle.Behind;
                            else
                                picture.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                            break;
                        case "anchor":
                            if (reader.AttributeCount == 0)
                                break;
                            value = reader.GetAttribute("behindDoc");
                            picture.IsBelowText = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("layoutInCell");
                            picture.LayoutInCell = (value == "1" || value == "true") ? true : false;
                            value = reader.GetAttribute("relativeHeight");
                            picture.OrderIndex = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            //parse the distance from left right top and bottom values of picture.
                            value = reader.GetAttribute("distT");
                            if (value != null)
                                picture.DistanceFromTop = GetPointValue(value);
                            value = reader.GetAttribute("distB");
                            if (value != null)
                                picture.DistanceFromBottom = GetPointValue(value);
                            value = reader.GetAttribute("distL");
                            if (value != null)
                                picture.DistanceFromLeft = GetPointValue(value);
                            value = reader.GetAttribute("distR");
                            if (value != null)
                                picture.DistanceFromRight = GetPointValue(value);
                            value = reader.GetAttribute("allowOverlap");
                            if (value != null)
                                picture.AllowOverlap = (value=="1"||value == "true") ? true : false;
                            break;
                        case "graphic":
                            reader.Read();
                            ParseGraphicData(reader, picture);
                            break;
                        case "inline":
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the graphic data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="picture">The picture.</param>
        private void ParseGraphicData(XmlReader reader, WPicture picture)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "graphicData")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "graphicData")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "pic":
                            ParsePictureData(reader, picture);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the picture data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="picture">The picture.</param>
        private void ParsePictureData(XmlReader reader, WPicture picture)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "pic")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "pic")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "spPr":
                            ParseVisualShapeProps(reader, picture);
                            break;
                        case "blipFill":
                            ParsePictureBlipFill(reader, picture);
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the visual shape props.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="picture">The picture.</param>
        /// <exception cref="System.Xml.XmlException">Unexpected xml tag  + reader.LocalName</exception>
        private void ParsePictureBlipFill(XmlReader reader, WPicture picture)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "blipFill")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "blipFill")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "srcRect":
                           string value = reader.GetAttribute("b");
                            if (!string.IsNullOrEmpty(value))
                                picture.FillRectangle.BottomOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("l");
                            if (!string.IsNullOrEmpty(value))
                                picture.FillRectangle.LeftOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("r");
                            if (!string.IsNullOrEmpty(value))
                                picture.FillRectangle.RightOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            value = reader.GetAttribute("t");
                            if (!string.IsNullOrEmpty(value))
                                picture.FillRectangle.TopOffset = int.Parse(value) / DLSConstants.ThousandthsUnit;
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the visual shape props.
        /// </summary>
        /// <param name="reader">The reader.</param>        /// <param name="picture">The picture.</param>
        private void ParseVisualShapeProps(XmlReader reader, WPicture picture)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "spPr")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "spPr")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ln":
                            picture.PictureShape.ShapeContainer = new MsofbtSpContainer(m_doc);
                            picture.PictureShape.ShapeContainer.Children.Add(new MsofbtOPT(m_doc));
                            picture.PictureShape.ShapeContainer.Children.Add(new MsofbtTertiaryFOPT(m_doc));
                            string value;
                            if (picture.TextWrappingStyle == TextWrappingStyle.Inline)
                            {
                                value = reader.GetAttribute("w");
                                if (value != null)
                                {
                                    int width = int.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
                                    width = (int)Math.Round(((double)width / DLSConstants.EmusPerPoint) * DLSConstants.BorderLineFactor);
                                    picture.PictureShape.PictureDescriptor.BorderLeft.LineWidth = (byte)width;
                                    picture.PictureShape.PictureDescriptor.BorderTop.LineWidth = (byte)width;
                                    picture.PictureShape.PictureDescriptor.BorderRight.LineWidth = (byte)width;
                                    picture.PictureShape.PictureDescriptor.BorderBottom.LineWidth = (byte)width;
                                }
                                value = reader.GetAttribute("cmpd");
                                BorderStyle borderStyle = BorderStyle.None;
                                if (value != null)
                                {
                                    TextBoxLineStyle linestyle = GetLineStyle(value);
                                    borderStyle = picture.PictureShape.GetBorderStyle(LineDashing.Solid, linestyle);
                                    if (linestyle == TextBoxLineStyle.Simple)
                                        borderStyle = BorderStyle.Single;
                                }
                                ParseInLineProps(reader, picture.PictureShape, ref borderStyle);
                                picture.PictureShape.PictureDescriptor.BorderLeft.BorderType = (byte)borderStyle;
                                picture.PictureShape.PictureDescriptor.BorderTop.BorderType = (byte)borderStyle;
                                picture.PictureShape.PictureDescriptor.BorderRight.BorderType = (byte)borderStyle;
                                picture.PictureShape.PictureDescriptor.BorderBottom.BorderType = (byte)borderStyle;
                            }
                            else
                            {
                                picture.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties, LineStyleBooleanProperties.DefaultValue);
                                picture.PictureShape.ShapeContainer.ShapeOptions.LineProperties.UsefLine = true;
                                picture.PictureShape.ShapeContainer.ShapeOptions.LineProperties.Line = true;
                                value = reader.GetAttribute("algn");
                                if (value == "in")
                                    picture.PictureShape.ShapeContainer.ShapeOptions.LineProperties.PenAlignInset = true;
                                value = reader.GetAttribute("cap");
                                if (value != null)
                                    picture.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndCapStyle, (uint)GetLineCapStyle(value));
                                value = reader.GetAttribute("cmpd");
                                if (value != null)
                                    picture.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStyle, (uint)GetLineStyle(value));
                                value = reader.GetAttribute("w");
                                if (value != null)
                                {
                                    uint width = uint.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
                                    picture.PictureShape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineWidth, width);
                                }
                                ParseLineProps(reader, picture.PictureShape);
                            }
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets the line cap style.
        /// </summary>
        /// <param name="lineCap">The line cap.</param>
        /// <returns></returns>
        private LineCap GetLineCapStyle(string lineCap)
        {
            switch (lineCap)
            {
                case "flat":
                    return LineCap.Flat;
                case "rnd":
                case "round":
                    return LineCap.Round;
                default:
                    return LineCap.Square;
            }
        }
        /// <summary>
        /// Parses the in line props.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="borderStyle">The border style.</param>
        private void ParseInLineProps(XmlReader reader, InlineShapeObject shape, ref BorderStyle borderStyle)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "ln")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            //Default line dashing denotes solid.
            LineDashing dashStyle = LineDashing.Solid;
            if (reader.IsEmptyElement)
                return;
            bool skip = false, noFill = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "ln")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "gradFill":
                            ParseGradientFill(reader, shape.LineGradient);
                            break;
                        case "prstDash":
                            string dash = reader.GetAttribute("val");
                            if (dash != null)
                            {
                                dashStyle = GetDashStyle(dash);
                                borderStyle = shape.GetBorderStyle(dashStyle, TextBoxLineStyle.Simple);
                            }
                            break;
                        case "solidFill":
                            //By default color is completely opaque (ie., 100%)
                            uint opacity = uint.MaxValue;
                            Color color = ParseColor(reader, "solidFill", ref opacity);
                            int id = WordColor.ConvertColorToId(color);
                            shape.PictureDescriptor.BorderLeft.LineColor = (byte)id;
                            shape.PictureDescriptor.BorderTop.LineColor = (byte)id;
                            shape.PictureDescriptor.BorderRight.LineColor = (byte)id;
                            shape.PictureDescriptor.BorderBottom.LineColor = (byte)id;
                            uint rgb = WordColor.ConvertColorToRGB(color);
                            shape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderTopColor, rgb);
                            shape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderLeftColor, rgb);
                            shape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderBottomColor, rgb);
                            shape.ShapeContainer.ShapePosition.SetPropertyValue((int)FOPTEGroupShape.borderRightColor, rgb);
                            break;
                        case "noFill":
                            noFill = true;
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
            if (dashStyle == LineDashing.Solid && borderStyle == BorderStyle.None && !noFill)
                borderStyle = BorderStyle.Single;
        }
        /// <summary>
        /// Parses the line props.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="shape">The shape.</param>
        private void ParseLineProps(XmlReader reader, InlineShapeObject shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "ln")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "ln")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "bevel":
                            shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineJoinStyle, (uint)LineJoin.Bevel);
                            break;
                        case "gradFill":
                            ParseGradientFill(reader, shape.LineGradient);
                            break;
                        case "headEnd":
                            string headEnd = reader.GetAttribute("type");
                            if (headEnd != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStartArrowhead, (uint)GetLineEnd(headEnd));
                            headEnd = reader.GetAttribute("w");
                            if (headEnd != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStartArrowWidth, (uint)GetLineEndWidth(headEnd));
                            headEnd = reader.GetAttribute("len");
                            if (headEnd != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineStartArrowLength, (uint)GetLineEndLength(headEnd));
                            break;
                        case "miter":
                            shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineJoinStyle, (uint)LineJoin.Miter);
                            string lim = reader.GetAttribute("lim");
                            if (lim != null)
                            {
                                uint miterLimit = (uint)((GetPercentage(lim) * DLSConstants.FixedPointsUnit) / DLSConstants.HundredthsUnit);
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineMiterLimit, miterLimit);
                            }
                            break;
                        case "noFill":
                            shape.ShapeContainer.ShapeOptions.LineProperties.Line = false;
                            break;
                        case "prstDash":
                            string dash = reader.GetAttribute("val");
                            if (dash != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineDashing, (uint)GetDashStyle(dash));
                            break;
                        case "round":
                            shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineJoinStyle, (uint)LineJoin.Round);
                            break;
                        case "solidFill":
                            //By default color is completely opaque (ie., 100%)
                            uint opacity = uint.MaxValue;
                            Color color = ParseColor(reader, "solidFill", ref opacity);
                            if (opacity != uint.MaxValue)
                            {
                                //65536 represents 0% transparency (100% opaque), in Docx suffixed with 65536f.
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineOpacity, opacity);
                            }
                            shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineColor, (uint)WordColor.ConvertColorToRGB(color));
                            break;
                        case "tailEnd":
                            string tailEnd = reader.GetAttribute("type");
                            if (tailEnd != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndArrowhead, (uint)GetLineEnd(tailEnd));
                            tailEnd = reader.GetAttribute("w");
                            if (tailEnd != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndArrowWidth, (uint)GetLineEndWidth(tailEnd));
                            tailEnd = reader.GetAttribute("len");
                            if (tailEnd != null)
                                shape.ShapeContainer.ShapeOptions.SetPropertyValue((int)FOPTELineStyle.lineEndArrowLength, (uint)GetLineEndLength(tailEnd));
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        private void ParseLineProps(XmlReader reader, Shape shape)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            if (reader.LocalName != "ln")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            if (reader.IsEmptyElement)
                return;
            bool skip = false;
            reader.Read();
            SkipWhitespaces(reader);
            string value = string.Empty;
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "ln")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "prstDash":
                            string dash = reader.GetAttribute("val");
                            if (dash != null)
                                shape.LineFormat.DashStyle = GetDashStyle(dash);
                            break;
                        case "solidFill":
                            shape.LineFormat.LineFormatType = LineFormatType.Solid;
                            //By default color is completely opaque (ie., 100%)
                            uint opacity = uint.MaxValue;
                            Color color = ParseColor(reader, "solidFill", ref opacity);
                            shape.LineFormat.Line = true;
                            shape.LineFormat.Color = color;
                            if(opacity != uint.MaxValue)
                                shape.LineFormat.Transparency = (float)Math.Round((1 - ((float)opacity / 65536)), 2);
                            break;
                        case "noFill":
                            shape.LineFormat.Line = false;
                            shape.LineFormat.LineFormatType = LineFormatType.None;
                            break;
                        case "gradFill":
                            shape.LineFormat.Line = true;
                            shape.LineFormat.LineFormatType = LineFormatType.Gradient;
                            ParseGradientFill(reader, shape.LineFormat.GradientFill);
                            break;
                        case "pattFill":
                            shape.LineFormat.Line = true;
                            shape.LineFormat.LineFormatType = LineFormatType.Patterned;
                            value = reader.GetAttribute("prst");
                            shape.LineFormat.Pattern = GetPatternType(value);
                            ParsePatternFill(reader, shape.LineFormat);
                            break;
                        case "bevel":
                            shape.LineFormat.LineJoin = LineJoin.Bevel;
                            break;
                        case "miter":
                            shape.LineFormat.LineJoin = LineJoin.Miter;
                            break;
                        case "round":
                            shape.LineFormat.LineJoin = LineJoin.Round;
                            break;
                        case "headEnd":
                            ParseHeadEnd(reader, shape.LineFormat);
                            break;
                        case "tailEnd":
                            ParseTailEnd(reader, shape.LineFormat);
                            break;
                        case "custDash":
                        case "extLst":
                            shape.LineFormat.DocxProps.Add(reader.LocalName, ReadSingleNodeIntoStream(reader));
                            skip = true;
                            break;
                        default:
                            ReadSingleNodeIntoStream(reader);
                            skip = true;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }

        private void ParseTailEnd(XmlReader reader, LineFormat lineFormat)
        {
            string value = reader.GetAttribute("type");
            if (!string.IsNullOrEmpty(value))
                lineFormat.EndArrowheadStyle = GetLineEnd(value);
            value = reader.GetAttribute("w");
            if (!string.IsNullOrEmpty(value))
                lineFormat.EndArrowheadWidth = GetLineEndWidth(value);
            value = reader.GetAttribute("len");
            if (!string.IsNullOrEmpty(value))
                lineFormat.EndArrowheadLength = GetLineEndLength(value);
        }

        private void ParseHeadEnd(XmlReader reader, LineFormat lineFormat)
        {
            string value = reader.GetAttribute("type");
            if (!string.IsNullOrEmpty(value))
                lineFormat.BeginArrowheadStyle = GetLineEnd(value);
            value = reader.GetAttribute("w");
            if (!string.IsNullOrEmpty(value))
                lineFormat.BeginArrowheadWidth = GetLineEndWidth(value);
            value = reader.GetAttribute("len");
            if (!string.IsNullOrEmpty(value))
                lineFormat.BeginArrowheadLength = GetLineEndLength(value);
        }

        private void ParsePatternFill(XmlReader reader, LineFormat lineFormat)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            uint opacity = uint.MaxValue;
            SkipWhitespaces(reader);
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == endNode))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "fgClr":
                            lineFormat.ForeColor = ParseColor(reader, "fgClr", ref opacity);
                            opacity = uint.MaxValue;
                            break;
                        case "bgClr":
                            lineFormat.Color = ParseColor(reader, "bgClr", ref opacity);
                            if (opacity != uint.MaxValue)
                                lineFormat.Transparency = (float)Math.Round((1 - ((float)opacity / 65536)), 2) * 100;
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                    reader.Read();

                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets the line end.
        /// </summary>
        /// <param name="lineEnd">The line end.</param>
        /// <returns></returns>
        private LineEnd GetLineEnd(string lineEnd)
        {
            switch (lineEnd)
            {
                case "triangle":
                case "block":
                    return LineEnd.ArrowEnd;
                case "arrow":
                case "open":
                    return LineEnd.ArrowOpenEnd;
                case "oval":
                    return LineEnd.ArrowOvalEnd;
                case "stealth":
                case "classic":
                    return LineEnd.ArrowStealthEnd;
                case "diamond":
                    return LineEnd.ArrowDiamondEnd;
                default:
                    return LineEnd.NoEnd;
            }
        }
        /// <summary>
        /// Gets the end width of the line.
        /// </summary>
        /// <param name="lineEndWidth">End width of the line.</param>
        /// <returns></returns>
        private LineEndWidth GetLineEndWidth(string lineEndWidth)
        {
            switch (lineEndWidth)
            {
                case "sm":
                case "narrow":
                    return LineEndWidth.NarrowArrow;
                case "lg":
                case "wide":
                    return LineEndWidth.WideArrow;
                default:
                    return LineEndWidth.MediumWidthArrow;
            }
        }
        /// <summary>
        /// Gets the end length of the line.
        /// </summary>
        /// <param name="lineEndLength">End length of the line.</param>
        /// <returns></returns>
        private LineEndLength GetLineEndLength(string lineEndLength)
        {
            switch (lineEndLength)
            {
                case "sm":
                case "short":
                    return LineEndLength.ShortArrow;
                case "lg":
                case "long":
                    return LineEndLength.LongArrow;
                default:
                    return LineEndLength.MediumLenArrow;
            }
        }
        /// <summary>
        /// Gets the flip orientation.
        /// </summary>
        /// <param name="flip">The flip.</param>
        /// <returns></returns>
        private FlipOrientation GetFlipOrientation(string flip)
        {
            switch (flip)
            {
                case "x":
                    return FlipOrientation.Horizontal;
                case "y":
                    return FlipOrientation.Vertical;
                case "xy":
                    return FlipOrientation.Both;
                default:
                    return FlipOrientation.None;
            }
        }
        /// <summary>
        /// Gets the type of the gradient shade.
        /// </summary>
        /// <param name="shade">The shade.</param>
        /// <returns></returns>
        private GradientShadeType GetGradientShadeType(string shade)
        {
            switch (shade)
            {
                case "circle":
                    return GradientShadeType.Circle;
                case "rect":
                    return GradientShadeType.Rectangle;
                default:
                    return GradientShadeType.Shape;
            }
        }
        /// <summary>
        /// Parses the gradient fill.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="gradientFill">The gradient fill.</param>
        private void ParseGradientFill(XmlReader reader, GradientFill gradientFill)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "gradFill")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;
            //Retreive Flip orientation
            string value = reader.GetAttribute("flip");
            if (!string.IsNullOrEmpty(value))
                gradientFill.Flip = GetFlipOrientation(value);
            //Rotate with shape
            value = reader.GetAttribute("rotWithShape");
            if (value != null)
                gradientFill.RotateWithShape = GetBoolValue(value);
            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "gradFill")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "gsLst":
                            ParseGradientStop(reader, gradientFill);
                            break;
                        case "lin":
                            ParseLinearGradient(reader, gradientFill);
                            break;
                        case "path":
                            gradientFill.PathGradient = new PathGradient();
                            value = reader.GetAttribute("path");
                            gradientFill.PathGradient.PathShade = GetGradientShadeType(value);
                            if (!reader.IsEmptyElement)
                                ParsePathGradient(reader, gradientFill.PathGradient);
                            break;
                        case "tileRect":
                            ParseTileRectangle(reader, gradientFill);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }

        private void ParseTileRectangle(XmlReader reader, GradientFill gradientFill)
        {
            int intValue;
            string value = reader.GetAttribute("b");
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                gradientFill.TileRectangle.BottomOffset = intValue / DLSConstants.ThousandthsUnit;
            }
            value = reader.GetAttribute("l");
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                gradientFill.TileRectangle.LeftOffset = intValue / DLSConstants.ThousandthsUnit;
            }
            value = reader.GetAttribute("r");
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                gradientFill.TileRectangle.RightOffset = intValue / DLSConstants.ThousandthsUnit;
            }
            value = reader.GetAttribute("t");
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                gradientFill.TileRectangle.TopOffset = intValue / DLSConstants.ThousandthsUnit;
            }
        }

        private void ParseLinearGradient(XmlReader reader, GradientFill gradientFill)
        {
            gradientFill.LinearGradient = new LinearGradient();
            string value = reader.GetAttribute("ang");
            if (value != null)
            {
                uint angle;
                uint.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out angle);
                gradientFill.LinearGradient.Angle = (short)Math.Round((double)angle / DLSConstants.SixtyThousandthsUnit);
            }
            value = reader.GetAttribute("scaled");
            if (value != null)
                gradientFill.LinearGradient.Scaled = GetBoolValue(value);
        }
        /// <summary>
        /// Parses the gradient stop.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="gradientFill">The gradient fill.</param>
        private void ParseGradientStop(XmlReader reader, GradientFill gradientFill)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "gsLst")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "gsLst")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "gs":
                            string value = reader.GetAttribute("pos");
                            GradientStop gStop = new GradientStop();
                            if (value != null)
                            {
                                double dValue;
                                double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out dValue);
                                gStop.Position = (byte)Math.Round(dValue / DLSConstants.ThousandthsUnit);
                            }
                            //By default color is completely opaque (ie., 100%)
                            uint opacity = uint.MaxValue;
                            gStop.Color = ParseColor(reader, "gs", ref opacity);
                            if (opacity != uint.MaxValue)
                            {
                                //65536 represents 0% transparency (100% opaque), in Docx suffixed with 65536f.
                                gStop.Opacity = (byte)Math.Round(((double)opacity / (double)DLSConstants.FixedPointsUnit) * DLSConstants.HundredthsUnit);
                            }
                            gradientFill.GradientStops.Add(gStop);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the path gradient.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="pathGradient">The path gradient.</param>
        private void ParsePathGradient(XmlReader reader, PathGradient pathGradient)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "path")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "path")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "fillToRect":
                            int intValue;
                            string value = reader.GetAttribute("b");
                            if (value != null)
                            {
                                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                                pathGradient.BottomOffset = intValue / DLSConstants.ThousandthsUnit;
                            }
                            value = reader.GetAttribute("l");
                            if (value != null)
                            {
                                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                                pathGradient.LeftOffset = intValue / DLSConstants.ThousandthsUnit;
                            }
                            value = reader.GetAttribute("r");
                            if (value != null)
                            {
                                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                                pathGradient.RightOffset = intValue / DLSConstants.ThousandthsUnit;
                            }
                            value = reader.GetAttribute("t");
                            if (value != null)
                            {
                                int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue);
                                pathGradient.TopOffset = intValue / DLSConstants.ThousandthsUnit;
                            }
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the color.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="opacity">The opacity.</param>
        /// <returns></returns>
        private Color ParseColor(XmlReader reader, string parentElement, ref uint opacity)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != parentElement)
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            Color color = Color.Empty;
            if (reader.IsEmptyElement)
                return color;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != parentElement)
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "srgbClr":
                            string hexValue = reader.GetAttribute("val");
                            color = GetHexColor(hexValue);
                            ParseColorTransform(reader, "srgbClr", ref color, ref opacity);
                            break;
                        case "scrgbClr":
                            string percent = reader.GetAttribute("r");
                            double red = GetPercentage(percent);
                            red = Math.Round(WordColor.MaxRGB * WordColor.ConvertsLinearRGBtoRGB(red / DLSConstants.HundredthsUnit));
                            percent = reader.GetAttribute("g");
                            double green = GetPercentage(percent);
                            green = Math.Round(WordColor.MaxRGB * WordColor.ConvertsLinearRGBtoRGB(green / DLSConstants.HundredthsUnit));
                            percent = reader.GetAttribute("b");
                            double blue = GetPercentage(percent);
                            blue = Math.Round(WordColor.MaxRGB * WordColor.ConvertsLinearRGBtoRGB(blue / DLSConstants.HundredthsUnit));
                            color = Color.FromArgb(WordColor.MaxRGB, (byte)red, (byte)green, (byte)blue);
                            ParseColorTransform(reader, "scrgbClr", ref color, ref opacity);
                            break;
                        case "prstClr":
                            string prstClr = reader.GetAttribute("val");
#if SILVERLIGHT || WP
                            color = GetKnownColor(prstClr);
#else
                            color = Color.FromName(prstClr);
#endif
                            ParseColorTransform(reader, "prstClr", ref color, ref opacity);
                            break;
                        case "hslClr":
                            double hue = 0, lum = 0, sat = 0;
                            string value = reader.GetAttribute("hue");
                            double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out hue);
                            hue /= DLSConstants.SixtyThousandthsUnit;
                            hue /= WordColor.MaxHue;
                            value = reader.GetAttribute("lum");
                            lum = GetPercentage(value) / DLSConstants.HundredthsUnit;
                            value = reader.GetAttribute("sat");
                            sat = GetPercentage(value) / DLSConstants.HundredthsUnit;
                            color = WordColor.ConvertHSLToColor(hue, sat, lum);
                            ParseColorTransform(reader, "hslClr", ref color, ref opacity);
                            break;
                        case "sysClr":
                            color = GetSystemColor(reader);
                            ParseColorTransform(reader, "sysClr", ref color, ref opacity);
                            break;
                        case "schemeClr":
                            color = GetSchemeColor(reader);
                            ParseColorTransform(reader, "schemeClr", ref color, ref opacity);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
            return color;
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// Gets the color of the known.
        /// </summary>
        /// <param name="colorName">Name of the color.</param>
        /// <returns></returns>
        private Color GetKnownColor(string colorName)
        {
            Color color = Color.Empty;
            try
            {
#if WINRT && !WP
                Action action = new Action(
                    delegate
                    {
                        Windows.UI.Color col = (Windows.UI.Color)Windows.UI.Xaml.Markup.XamlReader.Load(@"<Color xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" + colorName + "</Color>");
                        color = Color.FromArgb(col.A, col.R, col.G, col.B);
                    });
                UIDispatcher.Execute(action);
#else
                System.Windows.Media.Color col = (System.Windows.Media.Color)System.Windows.Markup.XamlReader.Load(@"<Color xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" + colorName + "</Color>");
                color = Color.FromArgb(col.A, col.R, col.G, col.B);
#endif
            }
            catch
            {
                color = Color.Black;
            }
            return color;
        }
#endif
        /// <summary>
        /// Gets the color of the system.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private Color GetSystemColor(XmlReader reader)
        {
            Color sysColor = Color.Empty;
            string lastClr = reader.GetAttribute("lastClr");
            if (lastClr != null)
                sysColor = GetHexColor(lastClr);
            string value = reader.GetAttribute("val");
            if (value != null)
            {
                KnownColor knownColor = KnownColor.Black;
                switch (value)
                {
                    case "3dDkShadow":
                        knownColor = KnownColor.ControlDarkDark;
                        break;
                    case "3dLight":
                        knownColor = KnownColor.ControlLight;
                        break;
                    case "activeBorder":
                        knownColor = KnownColor.ActiveBorder;
                        break;
                    case "activeCaption":
                        knownColor = KnownColor.ActiveCaption;
                        break;
                    case "appWorkspace":
                        knownColor = KnownColor.AppWorkspace;
                        break;
                    case "background":
                        knownColor = KnownColor.Desktop;
                        break;
                    case "btnFace":
                        knownColor = KnownColor.ButtonFace;
                        break;
                    case "btnHighlight":
                        knownColor = KnownColor.ButtonHighlight;
                        break;
                    case "btnShadow":
                        knownColor = KnownColor.ButtonShadow;
                        break;
                    case "btnText":
                        knownColor = KnownColor.ControlText;
                        break;
                    case "captionText":
                        knownColor = KnownColor.ActiveCaptionText;
                        break;
                    case "gradientActiveCaption":
                        knownColor = KnownColor.GradientActiveCaption;
                        break;
                    case "gradientInactiveCaption":
                        knownColor = KnownColor.GradientInactiveCaption;
                        break;
                    case "grayText":
                        knownColor = KnownColor.GrayText;
                        break;
                    case "highlight":
                        knownColor = KnownColor.Highlight;
                        break;
                    case "highlightText":
                        knownColor = KnownColor.HighlightText;
                        break;
                    case "hotLight":
                        knownColor = KnownColor.HotTrack;
                        break;
                    case "inactiveBorder":
                        knownColor = KnownColor.InactiveBorder;
                        break;
                    case "inactiveCaption":
                        knownColor = KnownColor.InactiveCaption;
                        break;
                    case "inactiveCaptionText":
                        knownColor = KnownColor.InactiveCaptionText;
                        break;
                    case "infoBk":
                        knownColor = KnownColor.Info;
                        break;
                    case "infoText":
                        knownColor = KnownColor.InfoText;
                        break;
                    case "menu":
                        knownColor = KnownColor.Menu;
                        break;
                    case "menuBar":
                        knownColor = KnownColor.MenuBar;
                        break;
                    case "menuHighlight":
                        knownColor = KnownColor.MenuHighlight;
                        break;
                    case "menuText":
                        knownColor = KnownColor.MenuText;
                        break;
                    case "scrollBar":
                        knownColor = KnownColor.ScrollBar;
                        break;
                    case "window":
                        knownColor = KnownColor.Window;
                        break;
                    case "windowFrame":
                        knownColor = KnownColor.WindowFrame;
                        break;
                    case "windowText":
                        knownColor = KnownColor.WindowText;
                        break;
                }
#if SILVERLIGHT || WP
                sysColor = GetKnownColor(Enum.GetName(typeof(KnownColor), knownColor));
#else
                sysColor = Color.FromKnownColor(knownColor);
#endif
            }
            return sysColor;
        }
        /// <summary>
        /// Gets the color of the scheme.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private Color GetSchemeColor(XmlReader reader)
        {
            Color themeColor = Color.Empty;
            string value = reader.GetAttribute("val");
            if (value != null)
            {
                if (m_doc.SchemeColor.ContainsKey(value))
                    return m_doc.SchemeColor[value];
                else
                {
                    switch (value)
                    {
                        case "accent1":
                            themeColor = Color.FromArgb(0xFF, 0x4F, 0x81, 0xBD);
                            break;
                        case "accent2":
                            themeColor = Color.FromArgb(0xFF, 0xC0, 0x50, 0x4D);
                            break;
                        case "accent3":
                            themeColor = Color.FromArgb(0xFF, 0x9B, 0xBB, 0x59);
                            break;
                        case "accent4":
                            themeColor = Color.FromArgb(0xFF, 0x80, 0x64, 0xA2);
                            break;
                        case "accent5":
                            themeColor = Color.FromArgb(0xFF, 0x4B, 0xAC, 0xC6);
                            break;
                        case "accent6":
                            themeColor = Color.FromArgb(0xFF, 0xF7, 0x96, 0x46);
                            break;
                        case "dk1":
                        case "tx1":
                        case "phClr":
                            themeColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
                            break;
                        case "dk2":
                        case "tx2":
                            themeColor = Color.FromArgb(0xFF, 0x1F, 0x49, 0x7D);
                            break;
                        case "folHlink":
                            themeColor = Color.FromArgb(0xFF, 0x80, 0x00, 0x80);
                            break;
                        case "hlink":
                            themeColor = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
                            break;
                        case "bg1":
                        case "lt1":
                            themeColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                            break;
                        case "bg2":
                        case "lt2":
                            themeColor = Color.FromArgb(0xFF, 0xEE, 0xEC, 0xE1);
                            break;
                    }
                }
            }
            return themeColor;
        }
        /// <summary>
        /// Parses the color transform.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="themeColor">Color of the theme.</param>
        /// <param name="opacity">The opacity.</param>
        private void ParseColorTransform(XmlReader reader, string parentElement, ref Color themeColor, ref uint opacity)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != parentElement)
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != parentElement)
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "alpha":
                            string alpha = reader.GetAttribute("val");
                            if (alpha != null)
                            {
                                double dOpacity = GetPercentage(alpha);
                                opacity = (uint)((dOpacity * DLSConstants.FixedPointsUnit) / DLSConstants.HundredthsUnit);
                            }
                            break;
                        case "alphaMod":
                            string alphaMod = reader.GetAttribute("val");
                            if (alphaMod != null)
                            {
                                double percent = GetPercentage(alphaMod);
                                byte a = WordColor.ConvertbyModulation(themeColor.A, percent);
                                if (a < WordColor.MaxRGB)
                                    opacity = (uint)((a / WordColor.MaxRGB) * DLSConstants.FixedPointsUnit);
                            }
                            break;
                        case "alphaOff":
                            string alphaOff = reader.GetAttribute("val");
                            if (alphaOff != null)
                            {
                                double percent = GetPercentage(alphaOff);
                                byte a = WordColor.ConvertbyOffset(themeColor.A, percent);
                                if (a < WordColor.MaxRGB)
                                    opacity = (uint)((a / WordColor.MaxRGB) * DLSConstants.FixedPointsUnit);
                            }
                            break;
                        case "blue":
                            string blue = reader.GetAttribute("val");
                            if (blue != null)
                            {
                                double percent = GetPercentage(blue);
                                themeColor = Color.FromArgb(themeColor.A, themeColor.R, themeColor.G, WordColor.ConvertbyOffset(0, percent));
                            }
                            break;
                        case "blueMod":
                            string blueMod = reader.GetAttribute("val");
                            if (blueMod != null)
                            {
                                double percent = GetPercentage(blueMod);
                                byte b = WordColor.ConvertbyModulation(themeColor.B, percent);
                                if (b < WordColor.MaxRGB)
                                    themeColor = Color.FromArgb(themeColor.A, themeColor.R, themeColor.G, b);
                            }
                            break;
                        case "blueOff":
                            string blueOff = reader.GetAttribute("val");
                            if (blueOff != null)
                            {
                                double percent = GetPercentage(blueOff);
                                byte b = WordColor.ConvertbyOffset(themeColor.B, percent);
                                if (b < WordColor.MaxRGB)
                                    themeColor = Color.FromArgb(themeColor.A, themeColor.R, themeColor.G, b);
                            }
                            break;
                        case "green":
                            string green = reader.GetAttribute("val");
                            if (green != null)
                            {
                                double percent = GetPercentage(green);
                                themeColor = Color.FromArgb(themeColor.A, themeColor.R, WordColor.ConvertbyOffset(0, percent), themeColor.B);
                            }
                            break;
                        case "greenMod":
                            string greenMod = reader.GetAttribute("val");
                            if (greenMod != null)
                            {
                                double percent = GetPercentage(greenMod);
                                byte g = WordColor.ConvertbyModulation(themeColor.G, percent);
                                if (g < WordColor.MaxRGB)
                                    themeColor = Color.FromArgb(themeColor.A, themeColor.R, g, themeColor.B);
                            }
                            break;
                        case "greenOff":
                            string greenOff = reader.GetAttribute("val");
                            if (greenOff != null)
                            {
                                double percent = GetPercentage(greenOff);
                                byte g = WordColor.ConvertbyOffset(themeColor.G, percent);
                                if (g < WordColor.MaxRGB)
                                    themeColor = Color.FromArgb(themeColor.A, themeColor.R, g, themeColor.B);
                            }
                            break;
                        case "red":
                            string red = reader.GetAttribute("val");
                            if (red != null)
                            {
                                double percent = GetPercentage(red);
                                themeColor = Color.FromArgb(themeColor.A, WordColor.ConvertbyOffset(0, percent), themeColor.G, themeColor.B);
                            }
                            break;
                        case "redMod":
                            string redMod = reader.GetAttribute("val");
                            if (redMod != null)
                            {
                                double percent = GetPercentage(redMod);
                                byte r = WordColor.ConvertbyModulation(themeColor.R, percent);
                                if (r < WordColor.MaxRGB)
                                    themeColor = Color.FromArgb(themeColor.A, r, themeColor.G, themeColor.B);
                            }
                            break;
                        case "redOff":
                            string redOff = reader.GetAttribute("val");
                            if (redOff != null)
                            {
                                double percent = GetPercentage(redOff);
                                byte r = WordColor.ConvertbyOffset(themeColor.R, percent);
                                if (r < WordColor.MaxRGB)
                                    themeColor = Color.FromArgb(themeColor.A, r, themeColor.G, themeColor.B);
                            }
                            break;
                        case "hue":
                            string hue = reader.GetAttribute("val");
                            if (hue != null)
                            {
                                double angle;
                                double.TryParse(hue, NumberStyles.Number, CultureInfo.InvariantCulture, out angle);
                                angle /= DLSConstants.SixtyThousandthsUnit;
                                WordColor.ConvertbyHue(ref themeColor, angle);
                            }
                            break;
                        case "hueMod":
                            string hueMod = reader.GetAttribute("val");
                            if (hueMod != null)
                            {
                                double ratio;
                                if (hueMod.EndsWith("%"))
                                {
                                    double.TryParse(hueMod, NumberStyles.Number, CultureInfo.InvariantCulture, out ratio);
                                    ratio /= DLSConstants.HundredthsUnit;
                                }
                                else
                                {
                                    double.TryParse(hueMod, NumberStyles.Number, CultureInfo.InvariantCulture, out ratio);
                                    ratio /= DLSConstants.SixtyThousandthsUnit;
                                    ratio /= WordColor.MaxHue;
                                }
                                WordColor.ConvertbyHueMod(ref themeColor, ratio);
                            }
                            break;
                        case "hueOff":
                            string hueOff = reader.GetAttribute("val");
                            if (hueOff != null)
                            {
                                double angle;
                                double.TryParse(hueOff, NumberStyles.Number, CultureInfo.InvariantCulture, out angle);
                                angle /= DLSConstants.SixtyThousandthsUnit;
                                WordColor.ConvertbyHueOffset(ref themeColor, angle);
                            }
                            break;
                        case "sat":
                            string sat = reader.GetAttribute("val");
                            if (sat != null)
                            {
                                double percent = GetPercentage(sat);
                                WordColor.ConvertbySat(ref themeColor, percent);
                            }
                            break;
                        case "satMod":
                            string satMod = reader.GetAttribute("val");
                            if (satMod != null)
                            {
                                double percent = GetPercentage(satMod);
                                WordColor.ConvertbySatMod(ref themeColor, percent);
                            }
                            break;
                        case "satOff":
                            string satOff = reader.GetAttribute("val");
                            if (satOff != null)
                            {
                                double percent = GetPercentage(satOff);
                                WordColor.ConvertbySatOffset(ref themeColor, percent);
                            }
                            break;
                        case "lum":
                            string lum = reader.GetAttribute("val");
                            if (lum != null)
                            {
                                double percent = GetPercentage(lum);
                                WordColor.ConvertbyLum(ref themeColor, percent);
                            }
                            break;
                        case "lumMod":
                            string lumMod = reader.GetAttribute("val");
                            if (lumMod != null)
                            {
                                double percent = GetPercentage(lumMod);
                                WordColor.ConvertbyLumMod(ref themeColor, percent);
                            }
                            break;
                        case "lumOff":
                            string lumOff = reader.GetAttribute("val");
                            if (lumOff != null)
                            {
                                double percent = GetPercentage(lumOff);
                                WordColor.ConvertbyLumOffset(ref themeColor, percent);
                            }
                            break;
                        case "comp":
                            themeColor = WordColor.ComplementColor(themeColor);
                            break;
                        case "gamma":
                            themeColor = WordColor.GammaColor(themeColor);
                            break;
                        case "gray":
                            themeColor = WordColor.GrayColor(themeColor);
                            break;
                        case "invGamma":
                            themeColor = WordColor.InverseGammaColor(themeColor);
                            break;
                        case "inv":
                            themeColor = WordColor.InverseColor(themeColor);
                            break;
                        case "tint":
                            string tint = reader.GetAttribute("val");
                            if (tint != null)
                            {
                                double percent = GetPercentage(tint) / DLSConstants.HundredthsUnit;
                                themeColor = WordColor.ConvertColorByTint(themeColor, percent);
                            }
                            break;
                        case "shade":
                            string shade = reader.GetAttribute("val");
                            if (shade != null)
                            {
                                double percent = GetPercentage(shade) / DLSConstants.HundredthsUnit;
                                themeColor = WordColor.ConvertColorByShade(themeColor, percent);
                            }
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double GetPercentage(string value)
        {
            double percent;
            if (value.EndsWith("%"))
                percent = double.Parse(value.Replace("%", ""), NumberStyles.Number, CultureInfo.InvariantCulture);
            else
                percent = double.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.ThousandthsUnit;
            return percent;
        }
        /// <summary>
        /// Parse the wrapping type
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="reader"></param>
        private void ParseWrappingType(XmlReader reader, WPicture picture)
        {
            string wrappingType = reader.GetAttribute("wrapText");

            if (wrappingType == null)
                return;

            switch (wrappingType)
            {
                case "bothSides":
                    picture.TextWrappingType = TextWrappingType.Both;
                    break;
                case "left":
                    picture.TextWrappingType = TextWrappingType.Left;
                    break;
                case "right":
                    picture.TextWrappingType = TextWrappingType.Right;
                    break;
                case "largest":
                    picture.TextWrappingType = TextWrappingType.Largest;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Parse the horizontal position properties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="picture"></param>
        private void ParsePictureHorizontalPosition(XmlReader reader, WPicture picture)
        {
            if (reader.LocalName != "positionH")
                throw new XmlException("positionH");

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            string value;

            string relationFromValue = reader.GetAttribute("relativeFrom");
            if (relationFromValue != null)
                picture.HorizontalOrigin = GetHorizOrigin(relationFromValue);

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "positionH")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "align":
#if !SILVERLIGHT && !WP
                            value = reader.ReadString();
#else 
                            value = reader.ReadInnerXml();
                            skip = true;
#endif
                            if (value != null)
                                picture.HorizontalAlignment = GetHorizAlign(value);
                            break;
                        case "posOffset":
                            float position = float.MaxValue;
#if !SILVERLIGHT && !WP
                            position = float.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
#else 
                            position = float.Parse(reader.ReadInnerXml(), CultureInfo.InvariantCulture);
                            skip = true;
#endif
                            if (position != float.MaxValue)
                                picture.HorizontalPosition = (float)Math.Round(position / DLSConstants.EmusPerPoint, 2);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        /// <param name="align">The align.</param>
        /// <returns></returns>
        private ShapeHorizontalAlignment GetHorizAlign(string align)
        {
            switch (align)
            {
                case "center":
                    return ShapeHorizontalAlignment.Center;
                case "left":
                    return ShapeHorizontalAlignment.Left;
                case "right":
                    return ShapeHorizontalAlignment.Right;
                case "inside":
                    return ShapeHorizontalAlignment.Inside;
                case "outside":
                    return ShapeHorizontalAlignment.Outside;
                default:
                    return ShapeHorizontalAlignment.None;
            }
        }
        /// <summary>
        /// Gets the horizontal origin.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        private HorizontalOrigin GetHorizOrigin(string origin)
        {
            switch (origin)
            {
                case "page":
                    return HorizontalOrigin.Page;
                case "text":
                case "column":
                    return HorizontalOrigin.Column;
                case "char":
                case "character":
                    return HorizontalOrigin.Character;
                case "left-margin-area":
                case "leftMargin":
                    return HorizontalOrigin.LeftMargin;
                case "right-margin-area":
                case "rightMargin":
                    return HorizontalOrigin.RightMargin;
                case "inner-margin-area":
                case "insideMargin":
                    return HorizontalOrigin.InsideMargin;
                case "outer-margin-area":
                case "outsideMargin":
                    return HorizontalOrigin.OutsideMargin;
                default:
                    return HorizontalOrigin.Margin;
            }
        }
        /// <summary>
        /// parse the vertical position element (positionV)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="picture"></param>
        private void ParsePictureVerticalPosition(XmlReader reader, WPicture picture)
        {
            if (reader.LocalName != "positionV")
                throw new XmlException("PositionV");

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            string value;

            string relationFromValue = reader.GetAttribute("relativeFrom");
            picture.VerticalOrigin = GetVertOrigin(relationFromValue);

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "positionV")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "align":
#if !SILVERLIGHT && !WP
                            value = reader.ReadString();
#else
                            value = reader.ReadInnerXml();
                            skip = true;
#endif
                            if (value != null)
                                picture.VerticalAlignment = GetVertAlign(value);
                            break;
                        case "posOffset":
                            float position = float.MaxValue;
#if !SILVERLIGHT && !WP
                            position = float.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
#else
                            position = float.Parse(reader.ReadInnerXml(), CultureInfo.InvariantCulture);
                            skip = true;
#endif
                            if (position != float.MaxValue)
                                picture.VerticalPosition = (float)Math.Round(position / DLSConstants.EmusPerPoint, 2);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Get the vertical alignment.
        /// </summary>
        /// <param name="align">The alignment.</param>
        /// <returns></returns>
        private ShapeVerticalAlignment GetVertAlign(string align)
        {
            switch (align)
            {
                case "top":
                    return ShapeVerticalAlignment.Top;
                case "bottom":
                    return ShapeVerticalAlignment.Bottom;
                case "center":
                    return ShapeVerticalAlignment.Center;
                case "inside":
                    return ShapeVerticalAlignment.Inside;
                case "inline":
                    return ShapeVerticalAlignment.Inline;
                case "outside":
                    return ShapeVerticalAlignment.Outside;
                default:
                    return ShapeVerticalAlignment.None;
            }
        }
        /// <summary>
        /// Get the vertical alignment of the Text in TextBox.
        /// </summary>
        /// <param name="align">The alignment.</param>
        /// <returns></returns>
        private VerticalAlignment GetTextVertAlign(string align)
        {
            switch (align)
            {
                case "b":
                case "bottom":
                    return VerticalAlignment.Bottom;
                case "ctr":
                case "middle":
                    return VerticalAlignment.Middle;
                default:
                    return VerticalAlignment.Top;
            }
        }
        /// <summary>
        /// Get the vertical Origin of the height in TextBox.
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        private HeightOrigin GetHeightOrigin(string rel)
        {
            switch (rel)
            {
                case "page":
                    return HeightOrigin.Page;
                case "top-margin-area":
                    return HeightOrigin.TopMargin;
                case "inner-margin-area":
                    return HeightOrigin.InsideMargin;
                case "outer-margin-area":
                    return HeightOrigin.OutsideMargin;
                case "bottom-margin-area":
                    return HeightOrigin.BottomMargin;
                default:
                    return HeightOrigin.Margin;
            }
        }
        /// <summary>
        /// Get the horizontal Origin of the width in TextBox.
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        private WidthOrigin GetWidthOrigin(string rel)
        {
            switch (rel)
            {
                case "page":
                    return WidthOrigin.Page;
                case "left-margin-area":
                    return WidthOrigin.LeftMargin;
                case "inner-margin-area":
                    return WidthOrigin.InsideMargin;
                case "outer-margin-area":
                    return WidthOrigin.OutsideMargin;
                case "right-margin-area":
                    return WidthOrigin.RightMargin;
                default:
                    return WidthOrigin.Margin;
            }
        }
        /// <summary>
        /// Get the vertical origin.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        private VerticalOrigin GetVertOrigin(string origin)
        {
            switch (origin.ToLower ())
            {
                case "page":
                    return VerticalOrigin.Page;
                case "paragraph":
                case "text":
                    return VerticalOrigin.Paragraph;
                case "line":
                    return VerticalOrigin.Line;
                case "topmargin":
                case "top-margin-area":
                    return VerticalOrigin.TopMargin;
                case "bottommargin":
                case "bottom-margin-area":
                    return VerticalOrigin.BottomMargin;
                case "innermargin":
                case "inner-margin-area":
                    return VerticalOrigin.InsideMargin;
                case "outermargin":
                case "outer-margin-area":
                    return VerticalOrigin.OutsideMargin;
                default:
                    return VerticalOrigin.Margin;
            }
        }
        /// <summary>
        /// Check whether the element is Picture or unsupported element
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private GraphicDataContentType CheckPicture(XmlReader reader)
        {
            reader.ReadToFollowing("graphicData", DocxConstants.A_namespace);

            if (reader.LocalName != "graphicData")
                return GraphicDataContentType.None;

            string uriValue = reader.GetAttribute("uri");

            //To Do - need to check this: WPI_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingInk";

            if (uriValue == null)
                return GraphicDataContentType.None;
            else if (uriValue == DocxConstants.PIC_namespace)
                return GraphicDataContentType.Picture;
            else if (uriValue == DocxConstants.CHART_namespace) //CHART_namespace = @"http://schemas.openxmlformats.org/drawingml/2006/chart";
                return GraphicDataContentType.Chart;
            else if (uriValue == DocxConstants.WP14_namespace || // WP14_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing";
                uriValue == DocxConstants.WPS_namespace) //WPS_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingShape";
                return GraphicDataContentType.Shape;
            else if (uriValue == DocxConstants.WPG_namespace) //WPG_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup";
                return GraphicDataContentType.Group;
            else
                return GraphicDataContentType.None;

        }
        #endregion Drawings

        #region Break
        /// <summary>
        /// Parse the break item
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private void ParseBreak(XmlReader reader, ParagraphItemCollection paraItems)
        {
            Break item = null;

            if (reader.LocalName != "br" && reader.LocalName != "cr")
                throw new XmlException("break item");

            string type = reader.GetAttribute("type", DocxConstants.W_namespace);

            if (type == "column")
                item = new Break(m_doc, BreakType.ColumnBreak);
            else if (type == "page")
                item = new Break(m_doc, BreakType.PageBreak);
            else
            {
                item = new Break(m_doc, BreakType.LineBreak);
                if (reader.LocalName == "cr")
                    item.TextRange.Text = "\r";
                else
                    item.TextRange.Text = "\v";
                if (m_currentRunFormat != null)
                    item.TextRange.ApplyCharacterFormat(m_currentRunFormat);
            }

            AddItem(item, paraItems);
        }
        #endregion Break

        #region Symbol
        /// <summary>
        /// Parse the symbol
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="paragraph">The paragraph</param>
        private ParagraphItem ParseSymbol(XmlReader reader, ParagraphItemCollection paraItems)
        {
            if (reader.LocalName != "sym")
                throw new XmlException("Excepting Symbol element");

            if (reader.AttributeCount != 2)
                return null;

            string fontValue = reader.GetAttribute("font", DocxConstants.W_namespace);
            string charValue = reader.GetAttribute("char", DocxConstants.W_namespace);

            if (fontValue == null || charValue == null)
                return null;
            int value = Int32.Parse(charValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            //Retrieves the actual character value from the Unicode character value created by adding F000.
            if (charValue.StartsWith("F0"))
                value = (int)(value - 0xF000);
            if (value <= 255)
            {
                WSymbol symbol = new WSymbol(m_doc);
                symbol.CharacterCode = (byte)value;
                symbol.FontName = fontValue;
                if (m_currentRunFormat != null)
                    symbol.CharacterFormat.ImportContainer(m_currentRunFormat);
                return symbol;
            }
            else
            {
                //Directly preserve symbol's Unicode character value from the font glyph.
                WTextRange txtRange = new WTextRange(m_doc);
                if (m_currentRunFormat != null)
                    txtRange.CharacterFormat.ImportContainer(m_currentRunFormat);
                txtRange.CharacterFormat.FontName = fontValue;
                txtRange.Text = Convert.ToString((char)value);
                return txtRange;
            }
        }
        #endregion Symbol

        #region TextRange
        /// <summary>
        /// Create a text range object
        /// </summary>
        /// <param name="textRange"></param>
        /// <param name="text"></param>
        /// <param name="runFormat"></param>
        private void UpdateTextRange(WTextRange textRange, string text, WCharacterFormat runFormat)
        {
            textRange.ApplyCharacterFormat(runFormat);
            textRange.Text = text;
        }
        /// <summary>
        /// Parse the run text
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private WTextRange ParseText(XmlReader reader, ParagraphItemCollection paraItems)
        {
            WTextRange txtRange = null;
            if (CurrentField is WMergeField)
            {
                txtRange = CurrentField as WTextRange;
                string text = GetNestedText(reader);
                if (m_currentRunFormat != null && m_currentRunFormat.Bidi && text != null)
                {
                    text = RotateText(text);
                    m_currentRunFormat.Bidi = false;
                }

                if (txtRange.Text == string.Empty)
                {
                    txtRange.Text = text;
                    txtRange.ApplyCharacterFormat(m_currentRunFormat);
                }
                else
                {
                    txtRange.Text += text;
                }

                return null;
            }
            else if (m_isPrevItemFieldStart)
            {
                ParseFieldValue(reader, paraItems);
                m_isPrevItemFieldStart = false;
                return txtRange;
            }
            else
            {
                txtRange = new WTextRange(m_doc);
                if (m_currentRunFormat != null)
                    txtRange.ApplyCharacterFormat(m_currentRunFormat);
                txtRange.Text = GetNestedText(reader);
                return txtRange;
            }
        }
        /// <summary>
        /// Gets the nested text.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private string GetNestedText(XmlReader reader)
        {
            string text = string.Empty;
            if (reader.IsEmptyElement)
                return text;
            int level = 0;
            reader.Read();
            SkipWhitespaces(reader);

            while (level > 0 || reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Text
                    || reader.NodeType == XmlNodeType.SignificantWhitespace)
                    text += reader.Value;
                else if (reader.NodeType == XmlNodeType.Element
                    && !reader.IsEmptyElement)
                {
                    level++;
                    string txt = GetNestedText(reader);
                    //Handled to skip the carriage return character within nested level text element.
                    txt = txt.Replace(ControlChar.CrLf, " ");
                    txt = txt.Replace(ControlChar.ParagraphBreak, " ");
                    txt = txt.Replace(ControlChar.LineFeedChar, ' ');
                    text += txt;
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (level == 0)
                        break;
                    level--;
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
            return text;
        }
        /// <summary>
        /// Rotates the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private String RotateText(string text)
        {
            char[] charText = text.ToCharArray();
            string rotatedText = string.Empty;
            for (int i = charText.Length - 1; i >= 0; i--)
                rotatedText += charText[i].ToString();

            return rotatedText;
        }
        /// <summary>
        /// Modify the text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ModifyText(string text)
        {
            text = text.Replace("&amp;", "&");
            text = text.Replace("&lt;", "<");
            text = text.Replace("&gt;", ">");
            return text;
        }
        #endregion TextRange

        #region Background
        /// <summary>
        /// Parse document background
        /// </summary>
        /// <param name="reader"></param>
        private void ParseDocumentBackground(XmlReader reader)
        {
            if (reader.LocalName != "background")
                throw new XmlException("background");

            Stream vmlBackgroundStream = ReadSingleNodeIntoStream(reader);
            XmlReader backgroundReader = UtilityMethods.CreateReader(vmlBackgroundStream);

            Color color = GetColorValue(backgroundReader.GetAttribute("color", DocxConstants.W_namespace));
            m_doc.Background.Color = color;

            m_doc.Background.Type = BackgroundType.Color;

            while (backgroundReader.Read())
            {
                if (backgroundReader.NodeType == XmlNodeType.Element)
                {
                    switch (backgroundReader.LocalName)
                    {
                        case "background":
                            color = GetColorValue(backgroundReader.GetAttribute("fillcolor"));
                            m_doc.Background.Color = color;
                            break;
                        case "fill":
                            string type = backgroundReader.GetAttribute("type");
                            if (type.StartsWith("gradient"))
                            {
                                ParseGradientFill(backgroundReader, m_doc.Background);
                            }
                            else if (type == "frame" || type == "tile")
                            {
                                ParseBackgroundPicture(backgroundReader, m_doc.Background);
                            }
                            break;
                    }
                }
            }
        }

        private void ParseBackgroundPicture(XmlReader reader, Background background)
        {
            string type = reader.GetAttribute("type");
            string id = reader.GetAttribute("id", DocxConstants.R_namespace);

            if (type == null || id == null)
                return;

            background.Type = (type == "tile") ? BackgroundType.Texture : BackgroundType.Picture;
            string imageName = GetImageName(id, false, false);
            if (ImageIds.ContainsKey(imageName))
                background.ImageRecord = m_doc.Images[ImageIds[imageName]];
            else
            {
                background.ImageBytes = GetImageBytes(imageName);
                ImageIds.Add(imageName, background.ImageRecord.ImageId);
            }
        }
        #endregion Background

        #region AlternateChunk
        /// <summary>
        /// Add the AlternateChunk to textbody.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private AlternateChunk AddAlternateChunk(IEntity entity)
        {
            AlternateChunk altChunk = null;

            if (entity is HeaderFooter)
            {
                altChunk = (entity as HeaderFooter).AddAlternateChunk();
            }
            else if (entity is WFootnote)
            {
                altChunk = (entity as WFootnote).TextBody.AddAlternateChunk();
            }
            else if (entity is WComment)
            {
                altChunk = (entity as WComment).TextBody.AddAlternateChunk();
            }
            else if (entity is StructureDocumentTagBlock)
            {
                altChunk = (entity as StructureDocumentTagBlock).SDTContent.TextBody.AddAlternateChunk();
            }
            else
            {
                altChunk = m_doc.LastSection.AddAlternateChunk();
            }

            return altChunk;
        }
        /// <summary>
        /// Parses the AlternateChunk.
        /// </summary>
        /// <param name="reader">The entity.</param>
        /// <param name="altChunk">The AlternateChunk.</param>
        private void ParseAlternateChunk(XmlReader reader, AlternateChunk altChunk)
        {
            string altChunkId = reader.GetAttribute("id", DocxConstants.R_namespace);
            altChunk.TargetId = "AltChunkId" + m_doc.AlternateChunkCount.ToString();
            altChunk.ContentPath = (string)DocumentRelations[altChunkId].Value;
            altChunk.ContentType = GetExtensionContentType(altChunk.ContentExtension);
        }
        #endregion

        #region StructureDocumentTag
        private IStructureDocumentTagBlock AddStructureDocumentTagBlock(IEntity entity)
        {
            IStructureDocumentTagBlock sdTagBlock = null;

            if (entity is HeaderFooter)
            {
                sdTagBlock = (entity as HeaderFooter).AddStructureDocumentTag();
            }
            else if (entity is WFootnote)
            {
                sdTagBlock = (entity as WFootnote).TextBody.AddStructureDocumentTag();
            }
            else if (entity is WComment)
            {
                sdTagBlock = (entity as WComment).TextBody.AddStructureDocumentTag();
            }
            else if (entity is StructureDocumentTagBlock)
            {
                sdTagBlock = (entity as StructureDocumentTagBlock).SDTContent.TextBody.AddStructureDocumentTag();
            }
            else
            {
                sdTagBlock = m_doc.LastSection.AddStructureDocumentTag();
            }

            return sdTagBlock;
        }
        /// <summary>
        /// Parse structure document tag block
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdTagBlock"></param>
        private void ParseStructureDocumentTagInline(XmlReader reader, StructureDocumentTagInline sdTagInline)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "sdtPr":
                            ParseSDTProperties(reader, sdTagInline.SDTProperties);
                            break;
                        case "sdtContent":
                            ParseSDTContentInline(reader, sdTagInline.SDTContent);
                            break;
                        case "sdtEndPr":
                            ParseSDTEndCharacterFormat(reader, sdTagInline.BreakCharacterFormat);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }

            }
        }
        /// <summary>
        /// Parse SDT content
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdTagBlock"></param>
        private void ParseSDTContentInline(XmlReader reader, SDTInlineContent sdtInlineContent)
        {
            ParseParagraphItems(reader, sdtInlineContent.ParagraphItems);
        }
        /// <summary>
        /// Parse structure document tag block
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdTagBlock"></param>
        private void ParseStructureDocumentTagBlock(XmlReader reader, StructureDocumentTagBlock sdTagBlock)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "sdtPr":
                            ParseSDTProperties(reader, sdTagBlock.SDTProperties);
                            break;
                        case "sdtContent":
                            ParseSDTContent(reader, sdTagBlock);
                            break;
                        case "sdtEndPr":
                            ParseSDTEndCharacterFormat(reader, sdTagBlock.BreakCharacterFormat);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }

            }
        }
        /// <summary>
        /// Parse SDT end character format
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdTagBlock"></param>
        private void ParseSDTEndCharacterFormat(XmlReader reader, WCharacterFormat charFormat)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "rPr":
                            ParseCharacterFormat(reader, charFormat);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse SDT content
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdTagBlock"></param>
        private void ParseSDTContent(XmlReader reader, StructureDocumentTagBlock sdTagBlock)
        {
            ParseBody(reader, sdTagBlock);
        }
        /// <summary>
        /// Parse SDT properties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdTagBlock"></param>
        private void ParseSDTProperties(XmlReader reader, SDTProperties properties)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "rPr":
                            ParseCharacterFormat(reader, properties.CharacterFormat);
                            break;
                        case "id":
                            properties.ID = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "showingPlcHdr":
                            properties.IsShowingPlaceHolder = true;
                            break;
                        case "alias":
                            properties.Alias = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "bibliography":
                            properties.Bibliograph = true;
                            break;
                        case "citation":
                            properties.Citation = true;
                            break;
                        case "temporary":
                            properties.IsTemporary = true;
                            break;
                        case "equation":
                            properties.SDTType = StructureDocumentType.Equation;
                            break;
                        case "picture":
                            properties.SDTType = StructureDocumentType.Picture;
                            break;
                        case "text":
                            properties.SDTType = StructureDocumentType.Text;
                            break;
                        case "richText":
                            properties.SDTType = StructureDocumentType.RichText;
                            break;
                        case "comboBox":
                            properties.SDTType = StructureDocumentType.ComboBox;
                            properties.SDTComboBox = new SDTComboBox();
                            properties.SDTComboBox.LastValue = reader.GetAttribute("lastValue", DocxConstants.W_namespace);
                            ParseSDTComboBox(reader, properties.SDTComboBox);
                            break;
                        case "dropDownList":
                            properties.SDTType = StructureDocumentType.DropDownList;
                            properties.SDTDropDownList = new SDTDropDownList();
                            properties.SDTDropDownList.LastValue = reader.GetAttribute("lastValue", DocxConstants.W_namespace);
                            ParseSDTDropDownList(reader, properties.SDTDropDownList);
                            break;
                        case "lock":
                            switch (reader.GetAttribute("val", DocxConstants.W_namespace))
                            {
                                case "sdtLocked":
                                    properties.LockSettings = LockSettings.SDTLocked;
                                    break;
                                case "sdtContentLocked":
                                    properties.LockSettings = LockSettings.SDTContentLocked;
                                    break;
                                case "contentLocked":
                                    properties.LockSettings = LockSettings.ContentLocked;
                                    break;
                                case "unlocked":
                                    properties.LockSettings = LockSettings.UnLocked;
                                    break;
                            }
                            break;
                        case "date":
                            properties.Date = new SDTDate();
                            properties.SDTType = StructureDocumentType.DatePicker;
                            string value = reader.GetAttribute("fullDate", DocxConstants.W_namespace);
                            if (value != string.Empty)
                                properties.Date.FullDate = value;
                            ParseSDTDate(reader, properties.Date);
                            break;
                        case "dataBinding":
                            properties.DataBinding = new SDTDataBinding();
                            properties.DataBinding.XPath = reader.GetAttribute("xpath", DocxConstants.W_namespace);
                            properties.DataBinding.StoreItemID = reader.GetAttribute("storeItemID", DocxConstants.W_namespace);
                            properties.DataBinding.PrefixMapping = reader.GetAttribute("prefixMappings", DocxConstants.W_namespace);
                            break;
                        case "checkbox":
                            properties.SDTCheckBox = new SDTCheckBox();
                            properties.SDTType = StructureDocumentType.CheckBox;
                            parseSDTCheckBox(reader, properties.SDTCheckBox);
                            break;
                        case "docPartObj":
                            properties.DocPartObj = new DocPartObj();
                            ParseDocPartObj(reader, properties.DocPartObj);
                            break;
                        case "docPartList":
                            properties.DocPartList = new DocPartList();
                            ParseDocPartList(reader, properties.DocPartList);
                            break;
                        case "repeatingSectionItem":
                            properties.ContentRepeatingType = ContentRepeatingType.RepeatingSectionItem;
                            break;
                        case "repeatingSection":
                            properties.ContentRepeatingType = ContentRepeatingType.RepeatingSection;
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }

        private void ParseDocPartList(XmlReader reader, DocPartList docPartList)
        {
            ParseDocPartItem(reader, docPartList as DocPartItem);
        }

        private void ParseDocPartItem(XmlReader reader, DocPartItem docPartItem)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "docPartUnique":
                            docPartItem.IsDocPartUnique = true;
                            break;
                        case "docPartGallery":
                            docPartItem.DocPartGallery = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "docPartCategory":
                            docPartItem.DocPartCategory = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse Doc Part Obj
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="docPartobj"></param>
        private void ParseDocPartObj(XmlReader reader, DocPartObj docPartobj)
        {
            ParseDocPartItem(reader, docPartobj as DocPartItem);
        }
        /// <summary>
        /// Parse Doc part obj and Doc part List child entities.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sdtCheckBox"></param>
        private void parseSDTCheckBox(XmlReader reader, SDTCheckBox sdtCheckBox)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "checked":
                            sdtCheckBox.IsChecked = GetBooleanValue(reader, DocxConstants.W14_namespace);
                            break;
                        case "checkedState":
                            sdtCheckBox.CheckedState.Value = reader.GetAttribute("val", DocxConstants.W14_namespace);
                            sdtCheckBox.CheckedState.Font = reader.GetAttribute("font", DocxConstants.W14_namespace);
                            break;
                        case "uncheckedState":
                            sdtCheckBox.UncheckedState.Font = reader.GetAttribute("font", DocxConstants.W14_namespace);
                            sdtCheckBox.UncheckedState.Value = reader.GetAttribute("val", DocxConstants.W14_namespace);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }

        }
        /// <summary>
        /// Parse SDT Dropdown list
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="comboBox"></param>
        private void ParseSDTDropDownList(XmlReader reader, SDTDropDownList dropDownList)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "listItem":
                            ListItem listItem = new ListItem();
                            listItem.DisplayText = reader.GetAttribute("displayText", DocxConstants.W_namespace);
                            listItem.Value = reader.GetAttribute("value", DocxConstants.W_namespace);
                            dropDownList.ListItems.Add(listItem);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }

        /// <summary>
        /// Parse SDT combo box
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="comboBox"></param>
        private void ParseSDTComboBox(XmlReader reader, SDTComboBox comboBox)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "listItem":
                            ListItem listItem = new ListItem();
                            listItem.DisplayText = reader.GetAttribute("displayText", DocxConstants.W_namespace);
                            listItem.Value = reader.GetAttribute("value", DocxConstants.W_namespace);
                            comboBox.ListItems.Add(listItem);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse SDT Date
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="date"></param>
        private void ParseSDTDate(XmlReader reader, SDTDate date)
        {
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;
            bool skip = false;

            string endnode = reader.LocalName;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != endnode)
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "dateFormat":
                            date.DateFormat = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "lid":
                            date.LID = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "calendar":
                            date.CalendarType = GetCalendarType(reader.GetAttribute("val", DocxConstants.W_namespace));
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Get calender type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private CalendarType GetCalendarType(string type)
        {
            switch (type)
            {
                case "gregorian":
                    return CalendarType.Gregorian;
                case "gregorianArabic":
                    return CalendarType.GregorianArabic;
                case "gregorianMeFrench":
                    return CalendarType.GregorianMiddleEastFrench;
                case "gregorianUs":
                    return CalendarType.GregorianEnglish;
                case "gregorianXlitEnglish":
                    return CalendarType.GregorianTransliteratedEnglish;
                case "gregorianXlitFrench":
                    return CalendarType.GregorianTransliteratedFrench;
                case "hebrew":
                    return CalendarType.Hebrew;
                case "hijri":
                    return CalendarType.Hijri;
                case "japan":
                    return CalendarType.Japan;
                case "korea":
                    return CalendarType.Korean;
                case "saka":
                    return CalendarType.Saka;
                case "taiwan":
                    return CalendarType.Taiwan;
                case "thai":
                    return CalendarType.Thai;
                default:
                    return CalendarType.None;
            }
        }
        # endregion
        #endregion Document elements

        #region Relations
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void ParseDocumentRelations(Stream stream)
        {
            XmlReader reader = UtilityMethods.CreateReader(stream);
            ParseRelations(reader, DocumentRelations);

        }
        /// <summary>
        /// Parses the relations.
        /// </summary>
        /// <param name="relReader">The xml reader.</param>
        /// <param name="relations">The relations collection.</param>
        private void ParseRelations(XmlReader relReader, Dictionary<string, DictionaryEntry> relations)
        {
            relReader.MoveToContent();
            if (relReader.LocalName != "Relationships")
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
            string targetMode = null;
            do
            {
                relReader.Read();
                id = relReader.GetAttribute("Id");
                target = relReader.GetAttribute("Target");
                type = relReader.GetAttribute("Type");

                if (id != null && target != null && type != null)
                {
                    if (target.StartsWith("/"))
                        target = target.Remove(0, 1);
                    //Trim the target path with "word/ if it is exist 
                    if (target.StartsWith("word/"))
                        target = target.Remove(0, 5);
                    itemEntry = new DictionaryEntry(type, target);
                    relations.Add(id, itemEntry);

                    targetMode = relReader.GetAttribute("TargetMode");
                    bool isNotLocal = (targetMode == "External") ? true : false;
                    if (isNotLocal && !IsExternalHyperlink.ContainsKey(id))
                        IsExternalHyperlink.Add(id, isNotLocal);
                }
            }
            while (relReader.LocalName != "Relationships");
        }
        #endregion Relations

        #region CustomProperties
        /// <summary>
        /// Parse the custom document properties
        /// </summary>
        /// <param name="reader">XmlReader for custom.xml</param>
        private void ParseCustomProperties(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "Properties")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            CustomDocumentProperties customProperties = (CustomDocumentProperties)m_doc.CustomDocumentProperties;

            reader.Read();

            if (!reader.EOF)
            {
                do
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName == "property")
                        {
                            ParseCustomProperty(reader, customProperties);
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }
                    reader.Read();

                } while (!reader.EOF);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="customProperties"></param>
        private void ParseCustomProperty(XmlReader reader, CustomDocumentProperties customProperties)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (customProperties == null)
                throw new ArgumentNullException("customProperties");

            if (reader.LocalName != "property")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            DocumentProperty customProperty = null;

            string propertyName;
            object propertyValue;

            propertyName = reader.GetAttribute("name");

            reader.MoveToElement();

            DocumentProperty docProp = null;
            string name = null;
            int id = 0;
            if (reader.NodeType != XmlNodeType.EndElement && reader.LocalName == "property")
            {
                do
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "property")
                    {
                        if (reader.MoveToAttribute(CustomDocumentProperties.NameAttribute))
                        {
                            name = reader.Value;
                        }

                        if (reader.MoveToAttribute(CustomDocumentProperties.PIDAttribute))
                        {
                            id = int.Parse(reader.Value, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Text && name != null)
                    {
                        docProp = new DocumentProperty(name, UpdateText(reader.Value));
                        m_doc.CustomDocumentProperties.CustomHash.Add(docProp.Name, docProp);
                    }
                    reader.Read();

                } while (!reader.EOF && reader.LocalName != "property");
            }
        }
        /// <summary>
        /// Updates the text, that cannot be represented in Xml as defined by XML 1.0 specification.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string UpdateText(string text)
        {
            //Converts the Xml text (_xHHHH_ escape character format) to its correcponding string value.
            text = XmlConvert.DecodeName(text);
            return text;
        }
        #endregion CustomProperties

        #region CoreProperties
        /// <summary>
        /// Parse the core properties
        /// </summary>
        /// <param name="reader">XmlReader for core.xml</param>
        private void ParseCoreProperties(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "coreProperties")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            BuiltinDocumentProperties properties = m_doc.BuiltinDocumentProperties;

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "category":
                            properties.Category = GetReaderElementValue(reader);
                            break;

                        case "created":
                            properties.CreateDate = DateTime.Parse(GetReaderElementValue(reader));
                            break;

                        case "creator":
                            properties.Author = GetReaderElementValue(reader);
                            break;

                        case "description":
                            properties.Comments = GetReaderElementValue(reader);
                            break;

                        case "keywords":
                            properties.Keywords = GetReaderElementValue(reader);
                            break;

                        case "lastModifiedBy":
                            properties.LastAuthor = GetReaderElementValue(reader);
                            break;

                        case "lastPrinted":
                            properties.LastPrinted = DateTime.Parse(GetReaderElementValue(reader));
                            break;

                        case "modified":
                            properties.LastSaveDate = DateTime.Parse(GetReaderElementValue(reader));
                            break;

                        case "subject":
                            properties.Subject = GetReaderElementValue(reader);
                            break;

                        case "title":
                            properties.Title = GetReaderElementValue(reader);
                            break;

                        case "revision":
                            properties.RevisionNumber = GetReaderElementValue(reader);
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
        }
        #endregion CoreProperties

        #region AppProperties
        /// <summary>
        /// Parse the extended properties
        /// </summary>
        /// <param name="reader">XmlReader for app.xml</param>
        private void ParseAppProperties(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "Properties")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            BuiltinDocumentProperties properties = m_doc.BuiltinDocumentProperties;

            reader.Read();
            string value;
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    value = null;
                    switch (reader.LocalName)
                    {
                        case "Template":
                            properties.Template = GetReaderElementValue(reader);
                            break;

                        case "TotalTime":
                            value = GetReaderElementValue(reader);
                            //retrieve the XML value based on the invariant cultureinfo
                            double d = Math.Round(double.Parse(value, CultureInfo.InvariantCulture));
                            properties.TotalEditingTime = TimeSpan.FromMinutes(d);
                            break;
                        case "Pages":
                            value = GetReaderElementValue(reader);
                            properties.SetPropertyValue(PIDSI.Pagecount, Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
                            break;

                        case "Words":
                            value = GetReaderElementValue(reader);
                            properties.SetPropertyValue(PIDSI.Wordcount, Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
                            break;

                        case "Characters":
                            value = GetReaderElementValue(reader);
                            properties.SetPropertyValue(PIDSI.Charcount, Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
                            break;

                        case "Application":
                            properties.ApplicationName = GetReaderElementValue(reader);
                            break;

                        case "DocSecurity":
                            value = GetReaderElementValue(reader);
                            properties.DocSecurity = Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;

                        case "Lines":
                            value = GetReaderElementValue(reader);
                            properties.SetPropertyValue(PIDDSI.LineCount, Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
                            break;

                        case "Paragraphs":
                            value = GetReaderElementValue(reader);
                            properties.SetPropertyValue(PIDDSI.ParCount, Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
                            break;

                        case "Company":
                            properties.Company = GetReaderElementValue(reader);
                            break;

                        case "Manager":
                            properties.Manager = GetReaderElementValue(reader);
                            break;

                        case "AppVersion":
                            AppVersion = GetReaderElementValue(reader);
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
        }
        #endregion AppProperties

        #region Numberings
        /// <summary>
        /// Parses the list.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="reader">The reader.</param>
        private void ParseList(XmlReader reader, FormatBase format)
        {
            WParagraphFormat prFormat = format as WParagraphFormat;
            if (prFormat.OwnerBase == null)
                return;

            bool hasStyleRef = false;

            WListFormat listFormat = null;
            if (prFormat.OwnerBase is WParagraph)
            {
                listFormat = (prFormat.OwnerBase as WParagraph).ListFormat;
            }
            else if (prFormat.OwnerBase is WParagraphStyle)
            {
                listFormat = (prFormat.OwnerBase as WParagraphStyle).ListFormat;
                hasStyleRef = true;
            }
            else if (prFormat.OwnerBase is WTableStyle)
                listFormat = (prFormat.OwnerBase as WTableStyle).ListFormat;
            else if (prFormat.OwnerBase is WNumberingStyle)
                listFormat = (prFormat.OwnerBase as WNumberingStyle).ListFormat;
            ParseListFormat(reader, listFormat);

            SinglePropertyModifierRecord sprmPIlvl = new SinglePropertyModifierRecord(WordSprmOptions.sprmPIlvl);
            if (listFormat.ListLevelNumber != -1)
                sprmPIlvl.IntValue = listFormat.ListLevelNumber;

            if (prFormat.Sprms == null)
            {
                SinglePropertyModifierArray sprmArray = new SinglePropertyModifierArray();
                prFormat.Sprms = sprmArray;
            }
            prFormat.Sprms.Add(sprmPIlvl);

            if (hasStyleRef)
            {
                string paraName = null;
                if (listFormat.CurrentListLevel != null)
                {
                    foreach (KeyValuePair<string, string> stylenameid in StyleNameId)
                        if (stylenameid.Value == (prFormat.OwnerBase as WParagraphStyle).Name)
                        {
                            paraName = stylenameid.Key;
                            break;
                        }
                    //paraName = (prFormat.OwnerBase as WParagraphStyle).Name.Replace(" ", string.Empty);
                    if (listFormat.CurrentListLevel.ParaStyleName == null)
                        listFormat.CurrentListLevel.ParaStyleName = paraName;
                }
                else if ((prFormat.OwnerBase is WParagraphStyle) && listFormat.ListLevelNumber > 0)
                {
                    ListStyle listStyle = FindListStyle(prFormat.OwnerBase as WParagraphStyle);
                    if (listStyle != null && listStyle.Levels.Count > listFormat.ListLevelNumber)
                    {
                        paraName = (prFormat.OwnerBase as WParagraphStyle).Name.Replace(" ", string.Empty);
                        listStyle.Levels[listFormat.ListLevelNumber].ParaStyleName = paraName;
                    }
                }
            }
        }
        /// <summary>
        /// Finds the list style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        private ListStyle FindListStyle(WParagraphStyle style)
        {
            WParagraphStyle baseStyle = style.BaseStyle;
            if (baseStyle == null)
                return null;

            while (baseStyle != null)
            {
                if (baseStyle.ListFormat.CurrentListStyle != null)
                    return baseStyle.ListFormat.CurrentListStyle;

                baseStyle = baseStyle.BaseStyle;
            }

            return null;
        }
        /// <summary>
        /// Parses the list format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        private void ParseListFormat(XmlReader reader, WListFormat listFormat)
        {
            string localName = reader.LocalName;
            reader.Read();

            while (reader.LocalName != localName)
            {
                switch (reader.LocalName)
                {
                    case "ilvl":
                        int level = Int32.Parse(reader.GetAttribute("val", DocxConstants.W_namespace));
                        if (level > 8)
                        {
                            level = 8;
                        }
                        listFormat.ListLevelNumber = level;
                        break;
                    case "numId":
                        string id = reader.GetAttribute("val", DocxConstants.W_namespace);
                        if (id == "0")
                        {
                            listFormat.IsEmptyList = true;
                        }
                        else if (m_doc.ListStyleNames.ContainsKey(id))
                        {
                            string styleName = m_doc.ListStyleNames[id];

                            ListStyle listStyle = m_doc.ListStyles.FindByName(styleName);
                            //Get Base style name of the list format
                            GetListFormatBaseStyleName(listStyle, ref styleName);

                            if (styleName != null)
                            {
                                listFormat.ApplyStyle(styleName);
                            }

                            if (OverListStyleNames.ContainsKey(id))
                            {
                                listFormat.LFOStyleName = OverListStyleNames[id];
                            }
                        }
                        break;
                }
                reader.Read();
            }
        }
        /// <summary>
        /// Get Base style name of the list format
        /// </summary>
        /// <returns></returns>
        private void GetListFormatBaseStyleName(ListStyle listStyle, ref string styleName)
        {
            if (listStyle != null && listStyle.BaseListStyleName != null
                && listStyle.BaseListStyleName != string.Empty)
            {
                if (m_doc.ListStyleNames.ContainsKey(listStyle.BaseListStyleName))
                {
                    string refId = m_doc.ListStyleNames[listStyle.BaseListStyleName];
                    if (m_doc.ListStyleNames.ContainsKey(refId))
                        styleName = m_doc.ListStyleNames[refId];
                }
                else if (StyleNameId.ContainsKey(listStyle.BaseListStyleName))
                {
                    //Get Numbering Style
                    IStyle baseStyle = m_doc.Styles.FindByName(StyleNameId[listStyle.BaseListStyleName]);
                    //Get style name of the list format from the NumberingStyle
                    if (baseStyle != null && baseStyle.StyleType == StyleType.NumberingStyle
                        && (baseStyle as WNumberingStyle).ListFormat != null
                        && (baseStyle as WNumberingStyle).ListFormat.CurrentListStyle != null)
                    {
                        styleName = (baseStyle as WNumberingStyle).ListFormat.CurrentListStyle.Name;
                    }
                }
            }
        }
        /// <summary>
        /// Parse the numberings
        /// </summary>
        /// <param name="reader">The xml reader</param>
        private void ParseNumberings(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsEmptyElement)
                return;

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "numbering")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "numbering")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "numPicBullet":
                            ParsePictureBullet(reader);
                            break;
                        case "abstractNum":
                            ParseAbstractNum(reader);
                            break;
                        case "num":
                            ParseListNum(reader);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse the Num element of the numberings
        /// </summary>
        /// <param name="reader">The xml reader</param>
        private void ParseListNum(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentException("reader");

            if (reader.LocalName != "num")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            string numId = reader.GetAttribute("numId", DocxConstants.W_namespace);

            reader.Read();

            SkipWhitespaces(reader);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "abstractNum")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "abstractNumId":
                            string abstractNumId = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (AbstractListStyleNames.ContainsKey(abstractNumId))
                                m_doc.ListStyleNames.Add(numId, AbstractListStyleNames[abstractNumId]);
                            break;
                        case "lvlOverride":
                            ParseLevelOverride(reader, numId);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse the level override attrbutes
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numId"></param>
        private void ParseLevelOverride(XmlReader reader, string numId)
        {
            if (reader == null)
                throw new ArgumentException("reader");

            if (reader.LocalName != "lvlOverride")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            ListOverrideStyle listOverrideStyle = null;

            if (!OverListStyleNames.ContainsKey(numId))
            {
                listOverrideStyle = new ListOverrideStyle(m_doc);
                listOverrideStyle.Name = "LfoStyle_" + Guid.NewGuid();
                OverListStyleNames.Add(numId, listOverrideStyle.Name);
                m_doc.ListOverrides.Add(listOverrideStyle);
            }
            else
            {
                listOverrideStyle = m_doc.ListOverrides.FindByName(OverListStyleNames[numId]);
            }

            string levelNumber = reader.GetAttribute("ilvl", DocxConstants.W_namespace);
            OverrideLevelFormat levelFormat = new OverrideLevelFormat(m_doc);

            if (levelNumber != null)
            {
                int levelIndex = Int32.Parse(levelNumber);
                listOverrideStyle.OverrideLevels.Add(levelIndex, levelFormat);

                if (reader.LocalName == "lvlOverride")
                    ParseLevelOverride(reader, levelFormat);
            }
        }
        /// <summary>
        /// Parse the level override
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="levelFormat"></param>
        private void ParseLevelOverride(XmlReader reader, OverrideLevelFormat levelFormat)
        {
            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "lvlOverride")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "startOverride":
                            string startOverrideValue = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (startOverrideValue != null)
                                levelFormat.StartAt = Int32.Parse(startOverrideValue);
                            levelFormat.OverrideStartAtValue = true;
                            break;
                        case "lvl":
                            levelFormat.OverrideFormatting = true;
                            ParseListLevel(reader, levelFormat.OverrideListLevel);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse the abstract numbering element
        /// </summary>
        /// <param name="reader"></param>
        private void ParseAbstractNum(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "abstractNum")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            string abstractNumId = reader.GetAttribute("abstractNumId", DocxConstants.W_namespace);
            ListStyle listStyle = new ListStyle(m_doc);
            ParseListStyle(reader, listStyle);
            m_doc.ListStyles.Add(listStyle);
            UpdateListType(listStyle);
            UpdateStyleName(listStyle);
            listStyle.IsSimple = (listStyle.Levels.Count == 1) ? true : false;
            AbstractListStyleNames.Add(abstractNumId, listStyle.Name);
        }
        /// <summary>
        /// Parse the abstrat list styles
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="listStyle">list style</param>
        private void ParseListStyle(XmlReader reader, ListStyle listStyle)
        {
            if (listStyle == null)
                throw new ArgumentException("list style");

            if (reader.LocalName != "abstractNum")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);
            //Add Empty list levels
            listStyle.CreateEmptyListLevels(false);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "abstractNum")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "multiLevelType":
                            if (reader.GetAttribute("val", DocxConstants.W_namespace) == "hybridMultilevel")
                                listStyle.IsHybrid = true;
                            break;
                        case "numStyleLink":
                            listStyle.BaseListStyleName = reader.GetAttribute("val", DocxConstants.W_namespace);
                            break;
                        case "lvl":
                            string lvl =reader.GetAttribute("ilvl", DocxConstants.W_namespace);
                            WListLevel level = listStyle.Levels[0];
                            if (lvl != null && lvl != string.Empty)
                            {
                                int lvlId = Int32.Parse(lvl);
                                level = listStyle.Levels[lvlId];
                            }
                            ParseListLevel(reader, level);
                            break;
                    }
                    if (!skip)
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }

        }
        /// <summary>
        /// Parse the list level
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="level">The list level</param>
        private void ParseListLevel(XmlReader reader, WListLevel level)
        {
            if (level == null)
                throw new ArgumentException("list level");

            if (reader.LocalName != "lvl")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "lvl")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "start":
                            level.StartAt = Int32.Parse(reader.GetAttribute("val", DocxConstants.W_namespace));
                            break;
                        case "pPr":
                            ParseParagraphFormat(reader, level.ParagraphFormat);
                            break;
                        case "rPr":
                            ParseCharacterFormat(reader, level.CharacterFormat);
                            break;
                        case "isLgl":
                            level.IsLegalStyleNumbering = true;
                            break;
                        case "lvlRestart":
                            string isLvlRestart = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (isLvlRestart == "0")
                                level.NoRestartByHigher = true;
                            break;
                        case "pStyle":
                            string styleId = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (StyleNameId.ContainsKey(styleId))
                                level.ParaStyleName = StyleNameId[styleId];
                            break;
                        case "lvlPicBulletId":
                            string picBulletId = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (PictureBullet.ContainsKey(picBulletId))
                                level.PicBullet = PictureBullet[picBulletId];
                            break;
                        case "numFmt":
                            string pattern = reader.GetAttribute("val", DocxConstants.W_namespace);
                            level.PatternType = GetLevelNumberFormat(pattern);
                            break;
                        case "lvlText":
                            string levelText = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (levelText != null)
                                ParseLevelText(level, levelText);
                            break;
                        case "lvlJc":
                            string justification = reader.GetAttribute("val", DocxConstants.W_namespace);
                            level.NumberAlignment = ParseLevelJc(justification);
                            break;
                        case "suff":
                            string levelFollow = reader.GetAttribute("val", DocxConstants.W_namespace);
                            level.FollowCharacter = GetFollowChar(levelFollow);
                            break;
                        case "legacy":
                            ParseLegacyProperties(reader, level);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the legacy properties.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="level">The level.</param>
        private void ParseLegacyProperties(XmlReader reader, WListLevel level)
        {
            string value = reader.GetAttribute("legacy", DocxConstants.W_namespace);
            if (value == "0" || value == "false" || value == "off")
                level.Word6Legacy = false;
            else
                level.Word6Legacy = true;
            value = reader.GetAttribute("legacySpace", DocxConstants.W_namespace);
            level.LegacySpace = (int)(float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture));
            value = reader.GetAttribute("legacyIndent", DocxConstants.W_namespace);
            level.LegacyIndent = (int)(float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// Get the corresponding the follow char
        /// </summary>
        /// <param name="levelFollow">follow character as string</param>
        /// <returns>returns the level follow character</returns>
        private FollowCharacterType GetFollowChar(string levelFollow)
        {
            switch (levelFollow)
            {
                case "tab":
                    return FollowCharacterType.Tab;
                case "space":
                    return FollowCharacterType.Space;
                default:
                    return FollowCharacterType.Nothing;
            }
        }
        /// <summary>
        /// Get the justification value
        /// </summary>
        /// <param name="justification">Justification value of type string </param>
        /// <returns>returns the justification value</returns>
        private ListNumberAlignment ParseLevelJc(string justification)
        {
            switch (justification)
            {
                case "center":
                    return ListNumberAlignment.Center;
                case "right":
                    return ListNumberAlignment.Right;
                default:
                    return ListNumberAlignment.Left;
            }
        }
        /// <summary>
        /// Parse the list level text
        /// </summary>
        /// <param name="level"></param>
        /// <param name="levelText"></param>
        private void ParseLevelText(WListLevel level, string levelText)
        {
            int lvlNumber = level.LevelNumber + 1;
            string curLvlText = "%" + lvlNumber.ToString();
            int lvlTextPos = levelText.IndexOf(curLvlText);
            string prefix = null;
            string sufix = null;
            if (lvlTextPos != -1)
            {
                prefix = levelText.Substring(0, lvlTextPos);
                if (level.PatternType != ListPatternType.Bullet)
                    prefix = CheckNumberPrefix(prefix);
                int sufStartPos = lvlTextPos + 2;
                sufix = levelText.Substring(sufStartPos, levelText.Length - sufStartPos);
            }
            if (level.PatternType == ListPatternType.Bullet)
            {
                if (lvlTextPos != -1)
                {
                    level.BulletCharacter = prefix;
                    level.BulletCharacter += sufix;
                }
                else
                    level.BulletCharacter = levelText;
            }
            else
            {
                level.NumberPrefix = prefix;
                level.NumberSufix = sufix;
                //To preserve level text for pattern type None
                if (level.PatternType == ListPatternType.None)
                {
                    if (lvlTextPos != -1)
                    {
                        level.BulletCharacter = prefix;
                        level.BulletCharacter += sufix;
                    }
                    else
                        level.BulletCharacter = levelText;
                }
            }
        }
        /// <summary>
        /// Checks the number prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        private string CheckNumberPrefix(string prefix)
        {
            if (prefix == null || prefix == string.Empty)
                return prefix;

            string numberPrefix = prefix;
            numberPrefix = numberPrefix.Replace("%1", WListLevel.Level1Str);
            numberPrefix = numberPrefix.Replace("%2", WListLevel.Level2Str);
            numberPrefix = numberPrefix.Replace("%3", WListLevel.Level3Str);
            numberPrefix = numberPrefix.Replace("%4", WListLevel.Level4Str);
            numberPrefix = numberPrefix.Replace("%5", WListLevel.Level5Str);
            numberPrefix = numberPrefix.Replace("%6", WListLevel.Level6Str);
            numberPrefix = numberPrefix.Replace("%7", WListLevel.Level7Str);
            numberPrefix = numberPrefix.Replace("%8", WListLevel.Level8Str);
            numberPrefix = numberPrefix.Replace("%9", WListLevel.Level9Str);

            return numberPrefix;
        }
        /// <summary>
        /// Gets the corresponding list pattern for the string value
        /// </summary>
        /// <param name="pattern">The list pattern value</param>
        /// <returns>returns the corresponding list pattern</returns>
        private ListPatternType GetLevelNumberFormat(string pattern)
        {
            switch (pattern)
            {
                case "none":
                    return ListPatternType.None;
                case "decimal":
                    return ListPatternType.Arabic;
                case "upperRoman":
                    return ListPatternType.UpRoman;
                case "lowerRoman":
                    return ListPatternType.LowRoman;
                case "upperLetter":
                    return ListPatternType.UpLetter;
                case "lowerLetter":
                    return ListPatternType.LowLetter;
                case "ordinal":
                    return ListPatternType.Ordinal;
                case "ordinalText":
                    return ListPatternType.OrdinalText;
                case "decimalZero":
                    return ListPatternType.LeadingZero;
                case "cardinalText":
                    return ListPatternType.Number;
                case "aiueoFullWidth":
                    return ListPatternType.FarEast;
                case "russianLower":
                    return ListPatternType.Special;
                default:
                    return ListPatternType.Bullet;
            }
        }
        /// <summary>
        /// Updates the type of the list style.
        /// </summary>
        /// <param name="listStyle">The list style.</param>
        private void UpdateListType(ListStyle listStyle)
        {
            listStyle.ListType = ListType.Bulleted;
            foreach (WListLevel level in listStyle.Levels)
            {
                if (level.PatternType != ListPatternType.Bullet)
                {
                    listStyle.ListType = ListType.Numbered;
                    break;
                }
            }
        }
        /// <summary>
        /// Updates the name of the list style.
        /// </summary>
        /// <param name="listStyle">The list style.</param>
        private void UpdateStyleName(ListStyle listStyle)
        {
            if (listStyle.ListType == ListType.Numbered)
                listStyle.Name = "Numbered_" + Guid.NewGuid().ToString();
            else
                listStyle.Name = "Bulleted_" + Guid.NewGuid().ToString();
        }

        #region Picture Bullets
        /// <summary>
        /// Parses the picture bullet
        /// </summary>
        /// <param name="reader">The reader</param>
        private void ParsePictureBullet(XmlReader reader)
        {
            string picBulletId = reader.GetAttribute("numPicBulletId", DocxConstants.W_namespace);
            while (reader.LocalName != "shape")
                reader.Read();

            //get the picture size details from the shape tag.
            string picStyle = ParsePictureBulletStyle(reader);

            if (!reader.IsEmptyElement)
            {
                while (reader.LocalName != "imagedata" && !(reader.LocalName == "shape" && reader.NodeType == XmlNodeType.EndElement))
                    reader.Read();

                if (reader.LocalName == "imagedata")
                {
                    //get the relationship id from the imagedata tag.
                    string pictureId = ParsePictureId(reader);

                    if (pictureId == null || pictureId.Length == 0)
                        return;

                    WPicture picture = new WPicture(m_doc);
                    //load the image from the parts
                    LoadImage(picture, pictureId, false, true);
                    //apply the picture style to the picture
                    ProcessPictureStyle(picture, picStyle);

                    if (picBulletId == string.Empty || picture.ImageRecord == null)
                        return;

                    PictureBullet.Add(picBulletId, picture);
                }
            }

            while (reader.LocalName != "numPicBullet")
                reader.Read();

        }
        /// <summary>
        /// Process the picture style
        /// </summary>
        /// <param name="picture">The picture</param>
        /// <param name="size">Picture size</param>
        private void ProcessPictureStyle(WPicture picture, string size)
        {
            string[] propParts = size.Split(new char[1] { ';' });
            for (int i = 0, cnt = propParts.Length; i < cnt; i++)
            {
                string partString = propParts[i];
                if (partString.StartsWith("width:"))
                {
                    partString = partString.Replace("width:", string.Empty);
                    picture.Width = GetPointValue(partString);
                }
                else if (partString.StartsWith("height:"))
                {
                    partString = partString.Replace("height:", string.Empty);
                    picture.Height = GetPointValue(partString);
                }
            }
        }
        /// <summary>
        /// Parses the size.
        /// </summary>
        /// <param name="partString">The part string.</param>
        /// <returns></returns>
        private float ParseSize(string partString)
        {
            if (partString.EndsWith("in"))
            {
                int inch = int.Parse(partString.Replace("in", string.Empty));
                return (float)UnitsConvertor.Instance.ConvertUnits(inch, PrintUnits.Inch, PrintUnits.Point);
            }

            partString = partString.Replace("pt", string.Empty);
            return ParseFloatVal(partString);
        }
        /// <summary>
        /// Parse/Get the picture relationship id
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string ParsePictureId(XmlReader reader)
        {
            if (reader.LocalName != "imagedata")
                throw new XmlException("imagedata - relationship id of shape");

            string picId = reader.GetAttribute("id", DocxConstants.R_namespace);

            return picId;
        }
        /// <summary>
        /// parse and return the pitcure bullet style.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string ParsePictureBulletStyle(XmlReader reader)
        {
            if (reader.LocalName != "shape")
                throw new XmlException("shape - Picture bullet");

            string style = reader.GetAttribute("style");

            return style;
        }
        #endregion Picture Bullets

        #endregion Numberings

        #region Styles
        /// <summary>
        /// Parse the style part
        /// </summary>
        /// <param name="reader"></param>
        private void ParseStyles(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "styles")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "styles")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "docDefaults":
                            ParseDocDefaults(reader);
                            break;
                        case "latentStyles":
                            ParseLatentStyles(reader);
                            skip = true;
                            break;
                        case "style":
                            skip = ParseStyle(reader);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Skip();
                }

                SkipWhitespaces(reader);

            }
            UpdateBaseStyles();
            UpdateLinkName();
            UpdateListInStyles();
        }
        /// <summary>
        /// Updates the base styles.
        /// </summary>
        private void UpdateBaseStyles()
        {
            if (m_baseStyleNames == null)
                return;

            foreach (string key in m_baseStyleNames.Keys)
            {
                Style style = m_doc.Styles.FindByName(key) as Style;
                if (style != null)
                {
                    if (StyleNameId.ContainsKey(m_baseStyleNames[key]))
                        style.ApplyBaseStyle(StyleNameId[m_baseStyleNames[key]]);
                }
            }
        }
        /// <summary>
        /// Updates the list in styles.
        /// </summary>
        private void UpdateListInStyles()
        {
            foreach (IStyle style in m_doc.Styles)
            {
                if (style is WParagraphStyle)
                {
                    string styleName = null;
                    ListStyle listStyle = (style as WParagraphStyle).ListFormat.CurrentListStyle;
                    //Get Base style name of the list format
                    GetListFormatBaseStyleName(listStyle, ref styleName);
                    if (styleName != null)
                        (style as WParagraphStyle).ListFormat.ApplyStyle(styleName);
                }
            }
        }
        /// <summary>
        /// Updates the name of the link style.
        /// </summary>
        private void UpdateLinkName()
        {
            if (m_linkStyleNames == null)
                return;

            foreach (string key in m_linkStyleNames.Keys)
            {
                Style style = m_doc.Styles.FindByName(key) as Style;
                string linkStyleId = m_linkStyleNames[key];
                if (StyleNameId.ContainsKey(linkStyleId))
                    style.LinkStyle = StyleNameId[linkStyleId];
            }
        }
        /// <summary>
        /// Parse the document style
        /// </summary>
        /// <param name="reader"></param>
        private bool ParseStyle(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "style")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return false;

            if (!reader.HasAttributes)
            {
                reader.Read();
                return false;
            }

            string styleType = reader.GetAttribute("type", DocxConstants.W_namespace).ToLower();
            Style docStyle = CreateStyle(styleType);

            docStyle.Name = reader.GetAttribute("styleId", DocxConstants.W_namespace);
            string isCustom = reader.GetAttribute("customStyle", DocxConstants.W_namespace);
            if (isCustom != null)
                docStyle.IsCustom = XmlConvert.ToBoolean(isCustom);
            reader.MoveToElement();
            ParseStyleProperties(reader, docStyle);
            m_doc.Styles.Add(docStyle);
            return false;

        }
        /// <summary>
        /// Parse the style properties
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="style">The style object</param>
        private void ParseStyleProperties(XmlReader reader, Style style)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (style == null)
                throw new ArgumentException("style");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "style")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            bool baseStyleDefined = false;
            IStyle baseStyle;
            string styleName = style.Name;
            bool isStyleNameNeedToBeUpdate = true;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "style")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case DocxConstants.c_characterFormatTag:
                            WCharacterFormat charFormat = GetCharacterFormat(style);
                            ParseCharacterFormat(reader, charFormat);
                            break;
                        case DocxConstants.c_paragraphFormatTag:
                            WParagraphFormat paragraphFormat = GetParagraphFormat(style);
                            ParseParagraphFormat(reader, paragraphFormat);
                            break;
                        case DocxConstants.c_tableFormatTag:
                            ParseTableStyleTableProperties(reader, (style as WTableStyle).TableProperties);
                            break;
                        case DocxConstants.c_rowFormatTag:
                            ParseTableStyleRowProperties(reader, (style as WTableStyle).RowProperties);
                            break;
                        case DocxConstants.c_cellFormatTag:
                            ParseTableStyleCellProperties(reader, (style as WTableStyle).CellProperties);
                            break;
                        case DocxConstants.c_conditionalTableStyleTag:
                            ParseConditionalFormattingStyleProperties(reader, style);
                            break;
                        case "semiHidden":
                            style.IsSemiHidden = GetBooleanValue(reader);
                            break;
                        case "unhideWhenUsed":
                            style.UnhideWhenUsed = GetBooleanValue(reader);
                            break;
                        case "qFormat":
                            style.IsPrimaryStyle = GetBooleanValue(reader);
                            break;
                        case "link":
                            string styleId = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (StyleNameId.ContainsKey(styleId))
                            {
                                style.LinkStyle = StyleNameId[styleId];
                            }
                            else
                            {
                                if (LinkStyleNames.ContainsKey(style.Name))
                                    LinkStyleNames.Add(style.Name, styleId);
                            }
                            break;
                        case "next":
                            string nextstyleId = reader.GetAttribute("val", DocxConstants.W_namespace);
                            style.NextStyle = nextstyleId;
                            break;
                        case "name":
                            isStyleNameNeedToBeUpdate = false;
                            styleName = reader.GetAttribute("val", DocxConstants.W_namespace);
                            //Add StyleNameID into the StyleNameId collection
                            AddStyleNameID(style, styleName);
                            break;
                        case "basedOn":
                            string baseStyleName = reader.GetAttribute("val", DocxConstants.W_namespace);
                            baseStyleDefined = true;
                            if (!StyleNameId.ContainsKey(baseStyleName))
                            {
                                BaseStyleNames.Add(style.Name, baseStyleName);
                            }
                            else
                            {
                                baseStyle = m_doc.Styles.FindByName(StyleNameId[baseStyleName]);
                                if (baseStyle == null)
                                    BaseStyleNames.Add(style.Name, baseStyleName);
                                else
                                    style.ApplyBaseStyle(StyleNameId[baseStyleName]);
                            }
                            break;
                        case "numPr":
                            WParagraphFormat prFormat = null;
                            if (style is WParagraphStyle)
                                prFormat = (style as WParagraphStyle).ParagraphFormat;
                            else if (style is WTableStyle)
                                prFormat = (style as WTableStyle).ParagraphFormat;
                            ParseList(reader, prFormat);
                            break;
                        default:
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
            //Add StyleNameID into the StyleNameId collection
            if (isStyleNameNeedToBeUpdate)
                AddStyleNameID(style, styleName);
            if (!baseStyleDefined && style.BaseStyle != null && style is WParagraphStyle)
                style.RemoveBaseStyle();
        }
        /// <summary>
        /// Add StyleNameID into the StyleNameId collection
        /// </summary>
        /// <param name="style"></param>
        /// <param name="styleName"></param>
        private void AddStyleNameID(Style style, string styleName)
        {
            if (!style.IsCustom)
            {
                if (style.BuiltinStyles.ContainsKey(styleName.ToLower()))
                    styleName = style.BuiltinStyles[styleName.ToLower()];
            }
            if (!string.IsNullOrEmpty(style.Name) && !StyleNameId.ContainsKey(style.Name))
                StyleNameId.Add(style.Name, styleName);
            else if (!string.IsNullOrEmpty(style.Name) && StyleNameId.ContainsKey(style.Name))
                StyleNameId[style.Name] = styleName;
            style.Name = styleName;
        }
        /// <summary>
        /// Creates the style.
        /// </summary>
        /// <param name="styleType">Type of the style.</param>
        /// <returns></returns>
        private Style CreateStyle(string styleType)
        {
            Style docStyle = null;
            switch (styleType)
            {
                case "character":
                    docStyle = new CharacterStyle(m_doc);
                    break;
                case "paragraph":
                    docStyle = new WParagraphStyle(m_doc);
                    break;
                case "table":
                    docStyle = new WTableStyle(m_doc);
                    break;
                case "numbering":
                    docStyle = new WNumberingStyle(m_doc);
                    break;
            }
            return docStyle;
        }
        /// <summary>
        /// Parse the latent styles
        /// </summary>
        /// <param name="reader"></param>
        private void ParseLatentStyles(XmlReader reader)
        {
            m_doc.LatentStyles2010 = ReadSingleNodeIntoStream(reader);
        }
        /// <summary>
        /// Parse the document defaults formattings
        /// </summary>
        /// <param name="reader"></param>
        private void ParseDocDefaults(XmlReader reader)
        {
            if (reader.LocalName != "docDefaults")
                throw new XmlException("docDefaults");

            if (reader.IsEmptyElement)
                return;

            reader.Read();

            SkipWhitespaces(reader);
            while (reader.LocalName != "docDefaults")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "rPrDefault":
                            if (reader.IsEmptyElement)
                                break;
                            //Move the reader from "rPrDefault - element" to "rPr - element"
                            reader.Read();
                            SkipWhitespaces(reader);
                            m_doc.DefCharFormat = new WCharacterFormat(m_doc);
                            ParseCharacterFormat(reader, m_doc.DefCharFormat);
                            //Move the reader from "rPr - end element" to "rPrDefault - end element"
                            reader.Read();
                            break;
                        case "pPrDefault":
                            m_doc.m_defParaFormat = new WParagraphFormat(m_doc);
                            if (reader.IsEmptyElement)
                                break;
                            reader.Read();
                            //Move the reader from "pPrDefault - element" to "pPr - element"
                            SkipWhitespaces(reader);
                            ParseParagraphFormat(reader, m_doc.m_defParaFormat);
                            //Move the reader from "pPr - end element" to "pPrDefault - end element"
                            reader.Read();
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        #endregion

        #region Parser - Table Style Properties
        /// <summary>
        /// Gets the character format of style.
        /// </summary>
        /// <param name="style">The style</param>
        /// <returns></returns>
        private WCharacterFormat GetCharacterFormat(Style style)
        {
            return (style.StyleType == StyleType.TableStyle) ? (style as WTableStyle).CharacterFormat :
                            (style.StyleType == StyleType.CharacterStyle) ? (style as CharacterStyle).CharacterFormat :
                            (style.StyleType == StyleType.NumberingStyle) ? (style as WNumberingStyle).CharacterFormat :
                            (style as WParagraphStyle).CharacterFormat;
        }
        /// <summary>
        /// Gets the paragraph format of style.
        /// </summary>
        /// <param name="style">The style</param>
        /// <returns></returns>
        private WParagraphFormat GetParagraphFormat(Style style)
        {
            return (style.StyleType == StyleType.TableStyle) ? (style as WTableStyle).ParagraphFormat
                : (style.StyleType == StyleType.NumberingStyle) ? (style as WNumberingStyle).ParagraphFormat
                : (style as WParagraphStyle).ParagraphFormat;
        }
        /// <summary>
        /// Parses the conditional formatting style properties.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="style">The style</param>
        private void ParseConditionalFormattingStyleProperties(XmlReader reader, Style style)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (style == null)
                throw new ArgumentException("style");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.IsEmptyElement)
                return;

            string styleType = reader.GetAttribute("type", DocxConstants.W_namespace);
            ConditionalFormattingCode conditionCode = GetConditionalFormattingCode(styleType);
            //Creates new Conditional Formatting Style
            ConditionalFormattingStyle cnfStyle = (style as WTableStyle).ConditionalFormat(conditionCode);

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != DocxConstants.c_conditionalTableStyleTag)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case DocxConstants.c_characterFormatTag:
                            ParseCharacterFormat(reader, cnfStyle.CharacterFormat);
                            break;
                        case DocxConstants.c_paragraphFormatTag:
                            ParseParagraphFormat(reader, cnfStyle.ParagraphFormat);
                            break;
                        case DocxConstants.c_tableFormatTag:
                            ParseTableStyleTableProperties(reader, cnfStyle.TableProperties);
                            break;
                        case DocxConstants.c_rowFormatTag:
                            ParseTableStyleRowProperties(reader, cnfStyle.RowProperties);
                            break;
                        case DocxConstants.c_cellFormatTag:
                            ParseTableStyleCellProperties(reader, cnfStyle.CellProperties);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets conditional formatting code.
        /// </summary>
        /// <param name="styleType">The styleType</param>
        /// <returns></returns>
        private ConditionalFormattingCode GetConditionalFormattingCode(string styleType)
        {
            ConditionalFormattingCode code = ConditionalFormattingCode.FirstRow;
            switch (styleType)
            {
                case "firstRow":
                    code = ConditionalFormattingCode.FirstRow;
                    break;
                case "lastRow":
                    code = ConditionalFormattingCode.LastRow;
                    break;
                case "band1Horz":
                    code = ConditionalFormattingCode.OddRowBanding;
                    break;
                case "band2Horz":
                    code = ConditionalFormattingCode.EvenRowBanding;
                    break;
                case "firstCol":
                    code = ConditionalFormattingCode.FirstColumn;
                    break;
                case "lastCol":
                    code = ConditionalFormattingCode.LastColumn;
                    break;
                case "band1Vert":
                    code = ConditionalFormattingCode.OddColumnBanding;
                    break;
                case "band2Vert":
                    code = ConditionalFormattingCode.EvenColumnBanding;
                    break;
                case "neCell":
                    code = ConditionalFormattingCode.FirstRowLastCell;
                    break;
                case "nwCell":
                    code = ConditionalFormattingCode.FirstRowFirstCell;
                    break;
                case "seCell":
                    code = ConditionalFormattingCode.LastRowLastCell;
                    break;
                case "swCell":
                    code = ConditionalFormattingCode.LastRowFirstCell;
                    break;
            }
            return code;
        }
        /// <summary>
        /// Parses the table style table properties.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="props">The props</param>
        private void ParseTableStyleTableProperties(XmlReader reader, TableStyleTableProperties props)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "tblStyleRowBandSize":
                            string rowStripe = reader.GetAttribute("val", DocxConstants.W_namespace);
                            props.RowStripe = long.Parse(rowStripe, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case "tblStyleColBandSize":
                            string columnStripe = reader.GetAttribute("val", DocxConstants.W_namespace);
                            props.ColumnStripe = long.Parse(columnStripe, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case "tblCellSpacing":
                            float spacingValue = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            string type = reader.GetAttribute("type", DocxConstants.W_namespace);
                            if (spacingValue != float.MaxValue && !(type != null && type == "nil"))
                                props.CellSpacing = spacingValue;
                            break;
                        case "tblInd":
                            string indentValue = reader.GetAttribute("w", DocxConstants.W_namespace);
                            props.LeftIndent = float.Parse(indentValue, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
                            break;
                        case "jc":
                            props.HorizontalAlignment = ParseTableJustification(reader);
                            break;
                        case "tblCellMar":
                            ParseTableMargins(reader, props.Paddings);
                            break;
                        case "tblBorders":
                            ParseBorder(reader, props.Borders);
                            break;
                        case "shd":
                            ParseShading(reader, props);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the table style row properties.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="props">The props</param>
        private void ParseTableStyleRowProperties(XmlReader reader, TableStyleRowProperties props)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "hidden":
                            props.IsHidden = GetBooleanValue(reader);
                            break;
                        case "tblHeader":
                            props.IsHeader = GetBooleanValue(reader);
                            break;
                        case "cantSplit":
                            props.IsBreakAcrossPages = GetBooleanValue(reader);
                            break;
                        case "tblCellSpacing":
                            float spacingValue = GetFloatValue(reader, "w", DocxConstants.W_namespace);
                            string type = reader.GetAttribute("type", DocxConstants.W_namespace);
                            if (spacingValue != float.MaxValue && !(type != null && type == "nil"))
                                props.CellSpacing = spacingValue;
                            break;
                        case "jc":
                            props.HorizontalAlignment = ParseTableJustification(reader);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the table style cell properties.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="props">The props</param>
        private void ParseTableStyleCellProperties(XmlReader reader, TableStyleCellProperties props)
        {
            if (reader.IsEmptyElement)
                return;

            string endNode = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "noWrap":
                            props.TextWrap = !GetBooleanValue(reader);
                            break;
                        case "vAlign":
                            props.VerticalAlignment = ParseCellVerticalAlignment(reader);
                            break;
                        case "tcMar":
                            ParseTableMargins(reader, props.Paddings);
                            break;
                        case "tcBorders":
                            ParseBorder(reader, props.Borders);
                            break;
                        case "shd":
                            ParseShading(reader, props);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the shading of table style table properties.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="props">The props</param>
        private void ParseShading(XmlReader reader, TableStyleTableProperties props)
        {
            string value = reader.GetAttribute("fill", DocxConstants.W_namespace);
            if (value != null)
            {
                if (value == "auto")
                    props.BackColor = Color.Empty;
                else
                    props.BackColor = GetColorValue(value);
            }

            value = reader.GetAttribute("color", DocxConstants.W_namespace);
            if (value != null)
            {
                if (value == "auto")
                    props.ForeColor = Color.Empty;
                else
                    props.ForeColor = GetColorValue(value);
            }

            value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (value != null)
            {
                props.TextureStyle = ParseTexture(value);
            }
        }
        /// <summary>
        /// Parses the shading of table style cell properties.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="props">The props</param>
        private void ParseShading(XmlReader reader, TableStyleCellProperties props)
        {
            string value = reader.GetAttribute("fill", DocxConstants.W_namespace);
            if (value != null)
            {
                if (value == "auto")
                    props.BackColor = Color.Empty;
                else
                    props.BackColor = GetColorValue(value);
            }

            value = reader.GetAttribute("color", DocxConstants.W_namespace);
            if (value != null)
            {
                if (value == "auto")
                    props.ForeColor = Color.Empty;
                else
                    props.ForeColor = GetColorValue(value);
            }

            value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (value != null)
            {
                props.TextureStyle = ParseTexture(value);
            }
        }
        #endregion Styles

        #region Parser - Character Formats
        /// <summary>
        /// Parse the run properties/Character formattings
        /// </summary>
        /// <param name="reader"></param>
        private void ParseCharacterFormat(XmlReader reader, WCharacterFormat charFormat)
        {
            bool skip = false;

            if (reader.LocalName != DocxConstants.c_characterFormatTag)
                throw new XmlException("Run properties");

            if (charFormat == null)
                throw new ArgumentException("Character Format");

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);

            while (reader.LocalName != DocxConstants.c_characterFormatTag)
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "rFonts":
                            ParseFonts(reader, charFormat);
                            break;
                        case "cs":
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                charFormat.ComplexScript = GetBooleanValue(reader);
                                charFormat.UpdateComplexProperty(WCharacterFormat.ComplexScriptKey, charFormat.ComplexScript);
                            }
                            break;
                        case "sz":
                            string size = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (size != null)
                                charFormat.FontSize = float.Parse(size, NumberStyles.Number, CultureInfo.InvariantCulture) / 2;
                            break;
                        case "szCs":
                            string sizeBidi = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (sizeBidi != null)
                                charFormat.FontSizeBidi = float.Parse(sizeBidi, NumberStyles.Number, CultureInfo.InvariantCulture) / 2;
                            break;
                        case "lang":
                            ParseLanguage(reader, charFormat);
                            break;
                        case "u":
                            ParseUnderline(reader, charFormat);
                            break;
                        case "vertAlign":
                            ParseVertAlign(reader, charFormat);
                            break;
                        case "color":
                            string colorValue = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (colorValue == "auto")
                                charFormat.TextColor = Color.Empty;
                            else if (colorValue != null)
                                charFormat.TextColor = GetColorValue(colorValue);
                            break;
                        case "highlight":
                            ParseHighlight(reader, charFormat);
                            break;
                        case "outline":
                            charFormat.OutLine = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.OutlineKey, charFormat.OutLine);
                            break;
                        case "position":
                            string pos = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (pos != null)
                            {
                                charFormat.Position = float.Parse(pos, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.ShortSize;
                            }
                            break;
                        case "spacing":
                            float spacingValue = GetFloatValue(reader, "val", DocxConstants.W_namespace);
                            if (spacingValue != float.MaxValue)
                            {
                                charFormat.CharacterSpacing = spacingValue;
                            }
                            break;
                        case "rStyle":
                            string styleId = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (StyleNameId.ContainsKey(styleId))
                                charFormat.CharStyleName = StyleNameId[styleId];
                            break;
                        case "shd":
                            ParseRunShading(reader, charFormat);
                            break;
                        case "bdr":
                            ParseBorder(reader, charFormat.Border);
                            break;
                        case "rPrChange":
                            ParseChangeCharacterFormat(reader, charFormat);
                            break;
                        case "b":
                            charFormat.Bold = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.BoldKey, charFormat.Bold);
                            break;
                        case "bCs":
                            charFormat.BoldBidi = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.BoldBidiKey, charFormat.BoldBidi);
                            break;
                        case "caps":
                            charFormat.AllCaps = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.AllCapsKey, charFormat.AllCaps);
                            break;
                        case "dstrike":
                            charFormat.DoubleStrike = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.DoubleStrikeKey, charFormat.DoubleStrike);
                            break;
                        case "vanish":
                            charFormat.Hidden = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.HiddenKey, charFormat.Hidden);
                            break;
                        case "smallCaps":
                            charFormat.SmallCaps = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.SmallCapsKey, charFormat.SmallCaps);
                            break;
                        case "imprint":
                            charFormat.Engrave = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.EngraveKey, charFormat.Engrave);
                            break;
                        case "emboss":
                            charFormat.Emboss = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.EmbossKey, charFormat.Emboss);
                            break;
                        case "i":
                            charFormat.Italic = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.ItalicKey, charFormat.Italic);
                            break;
                        case "iCs":
                            charFormat.ItalicBidi = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.ItalicBidiKey, charFormat.ItalicBidi);
                            break;
                        case "strike":
                            charFormat.Strikeout = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.StrikeKey, charFormat.Strikeout);
                            break;
                        case "shadow":
                            charFormat.Shadow = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.ShadowKey, charFormat.Shadow);
                            break;
                        case "noProof":
                            charFormat.NoProof = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.NoProofKey, charFormat.NoProof);
                            break;
                        case "rtl":
                            charFormat.Bidi = GetBooleanValue(reader);
                            charFormat.UpdateComplexProperty(WCharacterFormat.BidiKey, charFormat.Bidi);
                            break;
                        case "del":
                        case "moveFrom":
                            charFormat.IsDeleteRevision = true;
                            break;
                        case "ins":
                        case "moveTo":
                            charFormat.IsInsertRevision = true;
                            break;
                        case "cntxtAlts":
                            charFormat.UseContextualAlternates = GetBooleanValue(reader, DocxConstants.W14_namespace);
                            break;
                        case "ligatures":
                            ParseLigatures(reader, charFormat);
                            break;
                        case "numForm":
                            ParseNumberForm(reader, charFormat);
                            break;
                        case "numSpacing":
                            ParseNumberSpacing(reader, charFormat);
                            break;
                        case "stylisticSets":
                            ParseStylisticSet(reader, charFormat);
                            break;
                        default:
                            if (reader.LocalName != string.Empty && reader.LocalName != "rPr")
                            {
                                charFormat.XmlProps.Add(ReadSingleNodeIntoStream(reader));
                                skip = true;
                            }
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
#if !SILVERLIGHT && !WP
            UpdateUsedFontsCollection(charFormat);
#endif
        }
        /// <summary>
        /// Parses the ligatures.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="charFormat">The char format.</param>
        private void ParseLigatures(XmlReader reader, WCharacterFormat charFormat)
        {
            string value = reader.GetAttribute("val", DocxConstants.W14_namespace);
            switch (value)
            {
                case "none":
                    charFormat.Ligatures = LigatureType.None;
                    break;
                case "standard":
                    charFormat.Ligatures = LigatureType.Standard;
                    break;
                case "contextual":
                    charFormat.Ligatures = LigatureType.Contextual;
                    break;
                case "standardContextual":
                    charFormat.Ligatures = LigatureType.StandardContextual;
                    break;
                case "historical":
                    charFormat.Ligatures = LigatureType.Historical;
                    break;
                case "standardHistorical":
                    charFormat.Ligatures = LigatureType.StandardHistorical;
                    break;
                case "contextualHistorical":
                    charFormat.Ligatures = LigatureType.ContextualHistorical;
                    break;
                case "standardContextualHistorical":
                    charFormat.Ligatures = LigatureType.StandardContextualHistorical;
                    break;
                case "discretional":
                    charFormat.Ligatures = LigatureType.Discretional;
                    break;
                case "standardDiscretional":
                    charFormat.Ligatures = LigatureType.StandardDiscretional;
                    break;
                case "contextualDiscretional":
                    charFormat.Ligatures = LigatureType.ContextualDiscretional;
                    break;
                case "standardContextualDiscretional":
                    charFormat.Ligatures = LigatureType.StandardContextualDiscretional;
                    break;
                case "historicalDiscretional":
                    charFormat.Ligatures = LigatureType.HistoricalDiscretional;
                    break;
                case "standardHistoricalDiscretional":
                    charFormat.Ligatures = LigatureType.StandardHistoricalDiscretional;
                    break;
                case "contextualHistoricalDiscretional":
                    charFormat.Ligatures = LigatureType.ContextualHistoricalDiscretional;
                    break;
                case "all":
                    charFormat.Ligatures = LigatureType.All;
                    break;
            }
        }
        /// <summary>
        /// Parses the number form.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="charFormat">The char format.</param>
        private void ParseNumberForm(XmlReader reader, WCharacterFormat charFormat)
        {
            string value = reader.GetAttribute("val", DocxConstants.W14_namespace);

            switch (value)
            {
                case "default":
                    charFormat.NumberForm = NumberFormType.Default;
                    break;
                case "lining":
                    charFormat.NumberForm = NumberFormType.Lining;
                    break;
                case "oldStyle":
                    charFormat.NumberForm = NumberFormType.OldStyle;
                    break;
            }
        }
        /// <summary>
        /// Parses the number spacing.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="charFormat">The char format.</param>
        private void ParseNumberSpacing(XmlReader reader, WCharacterFormat charFormat)
        {
            string value = reader.GetAttribute("val", DocxConstants.W14_namespace);

            switch (value)
            {
                case "default":
                    charFormat.NumberSpacing = NumberSpacingType.Default;
                    break;
                case "proportional":
                    charFormat.NumberSpacing = NumberSpacingType.Proportional;
                    break;
                case "tabular":
                    charFormat.NumberSpacing = NumberSpacingType.Tabular;
                    break;
            }
        }
        /// <summary>
        /// Parses the stylistic set.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="charFormat">The char format.</param>
        private void ParseStylisticSet(XmlReader reader, WCharacterFormat charFormat)
        {
            if (reader == null)
                throw new Exception("reader is null");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "stylisticSets")
                throw new XmlException("Expected xml tag \"stylisticSets\"");

            if (reader.IsEmptyElement)
                return;

            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != "stylisticSets")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "styleSet":
                            string value = reader.GetAttribute("id", DocxConstants.W14_namespace);
                            charFormat.StylisticSet = GetStylisticSet(value);
                            break;
                    }
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Gets the stylistic set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private StylisticSetType GetStylisticSet(string value)
        {
            StylisticSetType styleSet = StylisticSetType.StylisticSetDefault;
            switch (value)
            {
                case "0":
                    styleSet = StylisticSetType.StylisticSetDefault;
                    break;
                case "1":
                    styleSet = StylisticSetType.StylisticSet01;
                    break;
                case "2":
                    styleSet = StylisticSetType.StylisticSet02;
                    break;
                case "3":
                    styleSet = StylisticSetType.StylisticSet03;
                    break;
                case "4":
                    styleSet = StylisticSetType.StylisticSet04;
                    break;
                case "5":
                    styleSet = StylisticSetType.StylisticSet05;
                    break;
                case "6":
                    styleSet = StylisticSetType.StylisticSet06;
                    break;
                case "7":
                    styleSet = StylisticSetType.StylisticSet07;
                    break;
                case "8":
                    styleSet = StylisticSetType.StylisticSet08;
                    break;
                case "9":
                    styleSet = StylisticSetType.StylisticSet09;
                    break;
                case "10":
                    styleSet = StylisticSetType.StylisticSet10;
                    break;
                case "11":
                    styleSet = StylisticSetType.StylisticSet11;
                    break;
                case "12":
                    styleSet = StylisticSetType.StylisticSet12;
                    break;
                case "13":
                    styleSet = StylisticSetType.StylisticSet13;
                    break;
                case "14":
                    styleSet = StylisticSetType.StylisticSet14;
                    break;
                case "15":
                    styleSet = StylisticSetType.StylisticSet15;
                    break;
                case "16":
                    styleSet = StylisticSetType.StylisticSet16;
                    break;
                case "17":
                    styleSet = StylisticSetType.StylisticSet17;
                    break;
                case "18":
                    styleSet = StylisticSetType.StylisticSet18;
                    break;
                case "19":
                    styleSet = StylisticSetType.StylisticSet19;
                    break;
                case "20":
                    styleSet = StylisticSetType.StylisticSet20;
                    break;
            }
            return styleSet;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="charFormat"></param>
        private void UpdateUsedFontsCollection(WCharacterFormat charFormat)
        {
            string fontName = charFormat.HasValue(WCharacterFormat.FontNameAsciiKey) ? charFormat.GetFontName(WCharacterFormat.FontNameAsciiKey) : null;

            if (string.IsNullOrEmpty(fontName))
                return;


            FontStyle fontStyle = new FontStyle();
            if (charFormat.HasValue(WCharacterFormat.BoldKey) && charFormat.Bold)
            {
                fontStyle |= FontStyle.Bold;
            }
            if (charFormat.HasValue(WCharacterFormat.ItalicKey) && charFormat.Italic)
            {
                fontStyle |= FontStyle.Italic;
            }
            if (charFormat.HasValue(WCharacterFormat.UnderlineKey) && charFormat.UnderlineStyle != UnderlineStyle.None)
            {
                fontStyle |= FontStyle.Underline;
            }
            if (charFormat.HasValue(WCharacterFormat.StrikeKey) && charFormat.Strikeout)
            {
                fontStyle |= FontStyle.Strikeout;
            }
            Font font = null;
            try
            {
                font = new Font(fontName, 11, fontStyle);
            }
            catch (Exception ex)
            {
                FontFamily fontFamily = new FontFamily(fontName);
                if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                    fontStyle |= FontStyle.Bold;
                if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                    fontStyle |= FontStyle.Italic;
                if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                    fontStyle |= FontStyle.Underline;
                if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                    fontStyle |= FontStyle.Strikeout;
                font = new Font(fontName, 11, fontStyle);
            }
            if (!m_doc.UsedFontNames.Contains(font))
                m_doc.UsedFontNames.Add(font);
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charFormat"></param>
        private void ParseChangeCharacterFormat(XmlReader reader, WCharacterFormat charFormat)
        {
            charFormat.IsChangedFormat = true;
            WCharacterFormat oldFormat = new WCharacterFormat(m_doc);
            //If empty element return
            if (reader.IsEmptyElement)
                return;
            reader.Read();
            SkipWhitespaces(reader);
            //Parse Old character format and ignore the same.
            ParseCharacterFormat(reader, oldFormat);
            //Add sprmCWall with boolean value set to true indicating that the character format is changed.
            SinglePropertyModifierRecord wallSprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmCWall);
            wallSprm.BoolValue = true;
            charFormat.Sprms.Add(wallSprm);
            //Reverse sprm modifiers list
            charFormat.Sprms.Modifiers.Reverse();
        }
        /// <summary>
        /// Parse the border
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="border"></param>
        private void ParseBorder(XmlReader reader, Border border)
        {
            border.IsRead = true;
            // Get line size
            string attrVal = reader.GetAttribute("sz", DocxConstants.W_namespace);
            if (attrVal != null)
            {
                border.LineWidth = float.Parse(attrVal, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.BorderLineFactor;
            }

            border.BorderType = GetBorderStyle(reader.GetAttribute("val", DocxConstants.W_namespace), border);

            // Get border spacing
            attrVal = reader.GetAttribute("space", DocxConstants.W_namespace);
            if (attrVal != null)
            {
                border.Space = float.Parse(attrVal, NumberStyles.Number, CultureInfo.InvariantCulture);
            }
            // Get border color
            string color = reader.GetAttribute("color", DocxConstants.W_namespace);

            if (color != null)
                border.Color = GetColorValue(color);


            attrVal = reader.GetAttribute("shadow", DocxConstants.W_namespace);
            if (attrVal != null && (attrVal == "on" || attrVal == "1" || attrVal == "true"))
            {
                border.Shadow = true;
            }

            // Set default value
            if (border.BorderType == BorderStyle.None && !border.HasNoneStyle &&
              border.LineWidth == 0 && border.Color == Color.Black)
            {
                border.BorderType = BorderStyle.Single;
                border.LineWidth = 0.5F;
            }

            border.IsRead = false;
        }
        /// <summary>
        /// Get the border style for the corresponding string value
        /// </summary>
        /// <param name="strStyle">String value that represents the border style</param>
        /// <param name="border">The border</param>
        /// <returns></returns>
        private BorderStyle GetBorderStyle(string boderStyle, Border border)
        {
            BorderStyle style = BorderStyle.None;
            switch (boderStyle)
            {
                case "twistedLines1":
                    style = BorderStyle.TwistedLines1;
                    break;
                case "triple":
                    style = BorderStyle.Triple;
                    break;
                case "dashSmallGap":
                    style = BorderStyle.DashSmallGap;
                    break;
                case "single":
                    style = BorderStyle.Single;
                    break;
                case "dotted":
                    style = BorderStyle.Dot;
                    break;
                case "dotDash":
                    style = BorderStyle.DotDash;
                    break;
                case "dotDotDash":
                    style = BorderStyle.DotDotDash;
                    break;
                case "dashed":
                    style = BorderStyle.DashLargeGap;
                    break;
                case "double":
                    style = BorderStyle.Double;
                    break;
                case "thickThinSmallGap":
                    style = BorderStyle.ThinThinSmallGap;
                    break;
                case "thinThickSmallGap":
                    style = BorderStyle.ThinThickSmallGap;
                    break;
                case "thinThickThinSmallGap":
                    style = BorderStyle.ThinThickThinSmallGap;
                    break;
                case "thinThickThinMediumGap":
                    style = BorderStyle.ThickThickThinMediumGap;
                    break;
                case "thickThinMediumGap":
                    style = BorderStyle.ThickThinMediumGap;
                    break;
                case "thinThickMediumGap":
                    style = BorderStyle.ThinThickMediumGap;
                    break;
                case "thickThinLargeGap":
                    style = BorderStyle.ThickThinLargeGap;
                    break;
                case "thinThickLargeGap":
                    style = BorderStyle.ThinThickLargeGap;
                    break;
                case "thinThickThinLargeGap":
                    style = BorderStyle.ThinThickThinLargeGap;
                    break;
                case "thick":
                    style = BorderStyle.Thick;
                    break;
                case "wave":
                    style = BorderStyle.Wave;
                    break;
                case "doubleWave":
                    style = BorderStyle.DoubleWave;
                    break;
                case "dashDotStroked":
                    style = BorderStyle.DashDotStroker;
                    break;
                case "threeDEngrave":
                    style = BorderStyle.Engrave3D;
                    break;
                case "threeDEmboss":
                    style = BorderStyle.Emboss3D;
                    break;
                case "outset":
                    style = BorderStyle.Outset;
                    break;
                case "inset":
                    style = BorderStyle.Inset;
                    break;
                case "nil":
                    style = BorderStyle.Cleared;
                    break;
                case "none":
                    style = BorderStyle.None;
                    border.HasNoneStyle = true;
                    break;
            }

            return style;
        }
        /// <summary>
        /// Get the multiplier to find the border line width
        /// </summary>
        /// <param name="border"></param>
        /// <returns></returns>
        private int GetBorderMultiplier(Border border)
        {
            FormatBase format = border.ParentFormat;
            if (format is WCharacterFormat)
                return DLSConstants.TwipsInOnePoint;
            else
            {
                if (format is Borders)
                {
                    format = format.ParentFormat;
                }
                // Investigate when format is null
                if (format == null || format is WParagraphFormat)
                    return DLSConstants.TwipsInOnePoint;
                else
                    return DocxConstants.BorderMultiplier;
            }
        }
        /// <summary>
        /// Parse the shading elements of run properties
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charFormat"></param>
        private void ParseRunShading(XmlReader reader, WCharacterFormat charFormat)
        {
            string fill = reader.GetAttribute("fill", DocxConstants.W_namespace);
            string textureStyle = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (textureStyle != null)
            {
                charFormat.TextureStyle = ParseTexture(textureStyle);
            }
            if (fill != null)
            {
                if (fill == "auto")
                    charFormat.TextBackgroundColor = Color.Empty;
                else
                    charFormat.TextBackgroundColor = GetColorValue(fill);
            }
        }
        /// <summary>
        /// Returns the textureStyle for the corresponding string value
        /// </summary>
        /// <param name="textureStyle"></param>
        /// <returns></returns>
        private TextureStyle ParseTexture(string textureStyle)
        {
            switch (textureStyle)
            {
                case "pct5":
                    return TextureStyle.Texture5Percent;
                case "pct10":
                    return TextureStyle.Texture10Percent;
                case "pct12":
                    return TextureStyle.Texture12Pt5Percent;
                case "pct15":
                    return TextureStyle.Texture15Percent;
                case "pct20":
                    return TextureStyle.Texture20Percent;
                case "pct25":
                    return TextureStyle.Texture25Percent;
                case "pct30":
                    return TextureStyle.Texture30Percent;
                case "pct35":
                    return TextureStyle.Texture35Percent;
                case "pct37":
                    return TextureStyle.Texture37Pt5Percent;
                case "pct40":
                    return TextureStyle.Texture40Percent;
                case "pct45":
                    return TextureStyle.Texture45Percent;
                case "pct50":
                    return TextureStyle.Texture50Percent;
                case "pct55":
                    return TextureStyle.Texture55Percent;
                case "pct60":
                    return TextureStyle.Texture60Percent;
                case "pct62":
                    return TextureStyle.Texture62Pt5Percent;
                case "pct65":
                    return TextureStyle.Texture65Percent;
                case "pct70":
                    return TextureStyle.Texture70Percent;
                case "pct75":
                    return TextureStyle.Texture75Percent;
                case "pct80":
                    return TextureStyle.Texture80Percent;
                case "pct85":
                    return TextureStyle.Texture85Percent;
                case "pct87":
                    return TextureStyle.Texture87Pt5Percent;
                case "pct90":
                    return TextureStyle.Texture90Percent;
                case "pct95":
                    return TextureStyle.Texture95Percent;
                case "thinHorzCross":
                    return TextureStyle.TextureCross;
                case "horzCross":
                    return TextureStyle.TextureDarkCross;
                case "diagCross":
                    return TextureStyle.TextureDarkDiagonalCross;
                case "reverseDiagStripe":
                    return TextureStyle.TextureDarkDiagonalDown;
                case "diagStripe":
                    return TextureStyle.TextureDarkDiagonalUp;
                case "horzStripe":
                    return TextureStyle.TextureDarkHorizontal;
                case "vertStripe":
                    return TextureStyle.TextureDarkVertical;
                case "thinDiagCross":
                    return TextureStyle.TextureDiagonalCross;
                case "thinReverseDiagStripe":
                    return TextureStyle.TextureDiagonalDown;
                case "thinDiagStripe":
                    return TextureStyle.TextureDiagonalUp;
                case "thinHorzStripe":
                    return TextureStyle.TextureHorizontal;
                case "solid":
                    return TextureStyle.TextureSolid;
                case "thinVertStripe":
                    return TextureStyle.TextureVertical;
                default:
                    return TextureStyle.TextureNone;
            }
        }
        /// <summary>
        /// Parse the vertAlign property of run properties (superscript or subscript)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charFormat"></param>
        private void ParseVertAlign(XmlReader reader, WCharacterFormat charFormat)
        {
            string vertAlign = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (vertAlign == null) return;

            if (vertAlign == "subscript")
                charFormat.SubSuperScript = SubSuperScript.SubScript;
            else if (vertAlign == "superscript")
                charFormat.SubSuperScript = SubSuperScript.SuperScript;
            else if (vertAlign == "baseline")
                charFormat.SubSuperScript = SubSuperScript.None;
        }
        /// <summary>
        /// Parse the underline format of run properties.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charFormat"></param>
        private void ParseUnderline(XmlReader reader, WCharacterFormat charFormat)
        {
            string style = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (style == null)
                return;

            switch (style)
            {
                case "single":
                    charFormat.UnderlineStyle = UnderlineStyle.Single;
                    break;
                case "words":
                    charFormat.UnderlineStyle = UnderlineStyle.Words;
                    break;
                case "double":
                    charFormat.UnderlineStyle = UnderlineStyle.Double;
                    break;
                case "dotted":
                    charFormat.UnderlineStyle = UnderlineStyle.Dotted;
                    break;
                case "thick":
                    charFormat.UnderlineStyle = UnderlineStyle.Thick;
                    break;
                case "dash":
                    charFormat.UnderlineStyle = UnderlineStyle.Dash;
                    break;
                case "dotDash":
                    charFormat.UnderlineStyle = UnderlineStyle.DotDash;
                    break;
                case "dotDotDash":
                    charFormat.UnderlineStyle = UnderlineStyle.DotDotDash;
                    break;
                case "wave":
                    charFormat.UnderlineStyle = UnderlineStyle.Wavy;
                    break;
                case "dashLong":
                    charFormat.UnderlineStyle = UnderlineStyle.DashLong;
                    break;
                case "dottedHeavy":
                    charFormat.UnderlineStyle = UnderlineStyle.DottedHeavy;
                    break;
                case "dashedHeavy":
                    charFormat.UnderlineStyle = UnderlineStyle.DashHeavy;
                    break;
                case "dashLongHeavy":
                    charFormat.UnderlineStyle = UnderlineStyle.DashLongHeavy;
                    break;
                case "dashDotHeavy":
                    charFormat.UnderlineStyle = UnderlineStyle.DotDashHeavy;
                    break;
                case "dashDotDotHeavy":
                    charFormat.UnderlineStyle = UnderlineStyle.DotDotDashHeavy;
                    break;
                case "wavyHeavy":
                    charFormat.UnderlineStyle = UnderlineStyle.WavyHeavy;
                    break;
                case "wavyDouble":
                    charFormat.UnderlineStyle = UnderlineStyle.WavyDouble;
                    break;
                default:
                    charFormat.UnderlineStyle = UnderlineStyle.None;
                    break;
            }
        }
        /// <summary>
        /// Parse thr fonts specified on the run properties.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charFormat"></param>
        private void ParseFonts(XmlReader reader, WCharacterFormat charFormat)
        {
            for (int i = 0, cnt = reader.AttributeCount; i < cnt; i++)
            {
                reader.MoveToAttribute(i);
                switch (reader.LocalName)
                {
                    case "ascii":
                        string fontName = reader.GetAttribute(i);
                        charFormat.FontNameAscii = fontName;
                        charFormat.FontName = fontName;
                        break;
                    case "hAnsi":
                        charFormat.FontNameNonFarEast = reader.GetAttribute(i);
                        break;
                    case "eastAsia":
                        charFormat.FontNameFarEast = reader.GetAttribute(i);
                        break;
                    case "hint":
                        string hint = reader.GetAttribute(i);
                        switch (hint.ToLower())
                        {
                            case "cs":
                                charFormat.IdctHint = FontHintType.CS;
                                break;
                            case "eastasia":
                                charFormat.IdctHint = FontHintType.EastAsia;
                                break;
                            default:
                                charFormat.IdctHint = FontHintType.Default;
                                break;
                        }
                        break;
                    case "cs":
                        charFormat.FontNameBidi = reader.GetAttribute(i);
                        break;
                    case "asciiTheme":
                        string theme = reader.GetAttribute(i);

                        if (theme == "minorHAnsi")
                        {
                            if (m_minorFontName == null)
                                charFormat.FontName = "Calibri";
                            else
                                charFormat.FontName = m_minorFontName;
                        }
                        else if (theme == "majorHAnsi")
                        {
                            if (m_majorFontName == null)
                                charFormat.FontName = "Cambria";
                            else
                                charFormat.FontName = m_majorFontName;
                        }
                        break;
                }
            }

        }

        private void ParseLanguage(XmlReader reader, WCharacterFormat charFormat)
        {
            string value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (!string.IsNullOrEmpty(value) && Enum.IsDefined(typeof(LocaleIDs), value.Replace('-', '_')))
            {
#if !SILVERLIGHT && !WP
                charFormat.LocaleIdASCII = (short)((int)(LocaleIDs)Enum.Parse(typeof(LocaleIDs), value.Replace('-', '_')));
#else 
                charFormat.LocaleIdASCII = (short)((int)(LocaleIDs)Enum.Parse(typeof(LocaleIDs), value.Replace('-', '_'), true));
#endif
            }

            value = reader.GetAttribute("eastAsia", DocxConstants.W_namespace);
            string bidiValue = reader.GetAttribute("bidi", DocxConstants.W_namespace);

            if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(bidiValue) && Enum.IsDefined(typeof(LocaleIDs), value.Replace('-', '_')))
            {
#if !SILVERLIGHT && !WP
                charFormat.LocaleIdFarEast = (short)((int)(LocaleIDs)Enum.Parse(typeof(LocaleIDs), value.Replace('-', '_')));
#else
                charFormat.LocaleIdFarEast = (short)((int)(LocaleIDs)Enum.Parse(typeof(LocaleIDs), value.Replace('-', '_'), true));
#endif
            }
            else if (!string.IsNullOrEmpty(bidiValue) && Enum.IsDefined(typeof(LocaleIDs), bidiValue.Replace('-', '_')))
            {
#if !SILVERLIGHT && !WP
                charFormat.LocaleIdFarEast = (short)((int)(LocaleIDs)Enum.Parse(typeof(LocaleIDs), bidiValue.Replace('-', '_')));
#else 
                charFormat.LocaleIdFarEast = (short)((int)(LocaleIDs)Enum.Parse(typeof(LocaleIDs), bidiValue.Replace('-', '_'), true));
#endif
            }

        }
        /// <summary>
        /// Parse the highlight of the character format
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charFormat"></param>
        private void ParseHighlight(XmlReader reader, WCharacterFormat charFormat)
        {
            string highlightValue = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (highlightValue == null)
                return;
            if (highlightValue.ToLower() == "darkyellow")
            {
                charFormat.HighlightColor = Color.Gold;
                return;
            }
            charFormat.HighlightColor = GetColorValue(highlightValue);
        }
        #endregion Parser - Character Formats

        #region Parser - Paragraph formats
        /// <summary>
        /// Parse the default paragraph properties
        /// </summary>
        /// <param name="reader"></param>
        private void ParseParagraphFormat(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            if (reader.LocalName != DocxConstants.c_paragraphFormatTag && reader.LocalName != "pPrChange")
                throw new XmlException("Paragraph properties");

            if (paragraphFormat == null)
                throw new ArgumentException("Paragraph format should not be null");

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != DocxConstants.c_paragraphFormatTag)
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "sectPr":
                            ParseSectionProperties(reader, m_doc.LastSection);
                            m_doc.AddSection();
                            break;
                        case "pStyle":
                            string pStyleName = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (!string.IsNullOrEmpty(pStyleName))
                            {
                                pStyleName = pStyleName.Trim();
                                if ((paragraphFormat.OwnerBase as WParagraph) != null)
                                {
                                    if (StyleNameId.ContainsKey(pStyleName))
                                    {
                                        IWParagraphStyle style = m_doc.Styles.FindByName(StyleNameId[pStyleName], StyleType.ParagraphStyle) as IWParagraphStyle;
                                        if (style != null)
                                            (paragraphFormat.OwnerBase as WParagraph).ApplyStyle(style);
                                    }
                                }
                            }
                            break;
                        case "framePr":
                            ParseFrameProperties(reader, paragraphFormat);
                            break;
                        case "rPr":
                            if (paragraphFormat.OwnerBase is WParagraph)
                            {
                                WCharacterFormat breakCharFormat = (paragraphFormat.OwnerBase as WParagraph).BreakCharacterFormat;
                                ParseCharacterFormat(reader, breakCharFormat);
                            }
                            break;
                        case "tabs":
                            ParseTabs(reader, paragraphFormat);
                            break;
                        case "pageBreakBefore":
                            paragraphFormat.PageBreakBefore = GetBooleanValue(reader);
                            break;
                        case "keepLines":
                            paragraphFormat.Keep = GetBooleanValue(reader);
                            break;
                        case "outlineLvl":
                            string level = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (level != null)
                            {
                                int outlineLevel = Int32.Parse(level, NumberStyles.Integer, CultureInfo.InvariantCulture);
                                paragraphFormat.OutlineLevel = (outlineLevel >= 0 && outlineLevel <= 9) ?
                                            (OutlineLevel)Enum.ToObject(typeof(OutlineLevel), outlineLevel) :
                                             OutlineLevel.BodyText;
                            }
                            break;
                        case "keepNext":
                            paragraphFormat.KeepFollow = GetBooleanValue(reader);
                            break;
                        case "jc":
                            ParseParagraphJustification(reader, paragraphFormat);
                            break;
                        case "ind":
                            ParseIndentation(reader, paragraphFormat);
                            break;
                        case "spacing":
                            ParseSpacing(reader, paragraphFormat);
                            break;
                        case "shd":
                            ParseShading(reader, paragraphFormat);
                            break;
                        case "bidi":
                            paragraphFormat.Bidi = GetBooleanValue(reader);
                            break;
                        case "widowControl":
                            paragraphFormat.WidowControl = GetBooleanValue(reader);
                            break;
                        case "wordWrap":
                            paragraphFormat.WordWrap = GetBooleanValue(reader);
                            break;
                        case "autoSpaceDE":
                            paragraphFormat.AutoSpaceDE = GetBooleanValue(reader);
                            break;
                        case "autoSpaceDN":
                            paragraphFormat.AutoSpaceDN = GetBooleanValue(reader);
                            break;
                        case "adjustRightInd":
                            paragraphFormat.AdjustRightIndent = GetBooleanValue(reader);
                            break;
                        case "pBdr":
                            if (paragraphFormat.OwnerBase is IStyle && !reader.IsEmptyElement)
                            {
                                WParagraphStyle pStyle = paragraphFormat.OwnerBase as WParagraphStyle;
                                if (pStyle == null)
                                    return;

                                Borders borders = pStyle.ParagraphFormat.Borders;
                                if (borders != null)
                                {
                                    ParseBorder(reader, borders);
                                }
                            }
                            else
                                ParseBorders(reader, paragraphFormat.OwnerBase as IEntity);
                            break;
                        case "numPr":
                            ParseList(reader, paragraphFormat);
                            break;
                        case "contextualSpacing":
                            paragraphFormat.ContextualSpacing = GetBooleanValue(reader);
                            break;
                        case "pPrChange":
                            ParseChangeParagraphFormat(reader, paragraphFormat);
                            break;
                        case "cnfStyle":
                            break;
                        case "mirrorIndents":
                            paragraphFormat.MirrorIndents = GetBooleanValue(reader);
                            break;
                        case "suppressAutoHyphens":
                            paragraphFormat.SuppressAutoHyphens = GetBooleanValue(reader);
                            break;
                        default:
                            if (reader.LocalName != string.Empty && reader.LocalName != "pPr")
                            {
                                paragraphFormat.XmlProps.Add(ReadSingleNodeIntoStream(reader));
                                skip = true;
                            }
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }

                SkipWhitespaces(reader);
            }
        }


        private void ParseFrameProperties(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            ParseFramePos(reader, paragraphFormat);
            ParseFrameAnchor(reader, paragraphFormat);
            ParseFrameSize(reader, paragraphFormat);
        }
        /// <summary>
        /// Parses the size of the frame.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="paraFormat">The para format.</param>
        private void ParseFrameSize(XmlReader reader, WParagraphFormat paraFormat)
        {
            // Reads the frame width.
            string width = reader.GetAttribute("w", DocxConstants.W_namespace);
            if (width != null && width != string.Empty)
                paraFormat.FrameWidth = float.Parse(width, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;

            // Reads the frame height.
            string height = reader.GetAttribute("h", DocxConstants.W_namespace);
            float value = 0;
            if (height != null && height != string.Empty)
                value = float.Parse(height, NumberStyles.Number, CultureInfo.InvariantCulture);

            // Reads the frame height type.
            // Updates the frame height based on the Binary format sprm - sprmPWHeightAbs
            string heightType = reader.GetAttribute("hRule", DocxConstants.W_namespace);
            if (heightType != null && heightType == "exact")
            {
                paraFormat.FrameHeight = value / DLSConstants.TwipsInOnePoint;
            }
            else if (value != 0)
            {
                paraFormat.FrameHeight = (float)((short)value | (1 << 15)) / DLSConstants.TwipsInOnePoint;
            }
            else if (heightType == "auto")
                paraFormat.FrameHeight = 0;
        }
        /// <summary>
        /// Parses the frame anchor.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="paraFormat">The paragraph format.</param>
        private void ParseFrameAnchor(XmlReader reader, WParagraphFormat paraFormat)
        {
            string hAnchor = reader.GetAttribute("hAnchor", DocxConstants.W_namespace);
            if (hAnchor != null && hAnchor != string.Empty)
            {
                switch (hAnchor)
                {
                    case "margin":
                        paraFormat.FrameHorizontalPos = (byte)FrameHorzAnchor.Margin;
                        break;
                    case "page":
                    case "column":
                        paraFormat.FrameHorizontalPos = (byte)FrameHorzAnchor.Page;
                        break;
                    case "text":
                        paraFormat.FrameHorizontalPos = (byte)FrameHorzAnchor.Text;
                        break;
                }
            }

            string vAnchor = reader.GetAttribute("vAnchor", DocxConstants.W_namespace);
            if (vAnchor != null && vAnchor != string.Empty)
            {
                switch (vAnchor)
                {
                    case "margin":
                        paraFormat.FrameVerticalPos = (byte)FrameVertAnchor.Margin;
                        break;
                    case "page":
                        paraFormat.FrameVerticalPos = (byte)FrameVertAnchor.Page;
                        break;
                    case "text":
                        paraFormat.FrameVerticalPos = (byte)FrameVertAnchor.Text;
                        break;
                }
            }
        }
        /// <summary>
        /// Parses the frame position.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="paraFormat">The paragraph format.</param>
        private void ParseFramePos(XmlReader reader, WParagraphFormat paraFormat)
        {
            string xAlign = reader.GetAttribute("xAlign", DocxConstants.W_namespace);

            if (xAlign != null && xAlign != string.Empty)
            {
                switch (xAlign)
                {
                    case "right":
                        paraFormat.FrameX = (short)PageNumberAlignment.Right;
                        break;
                    case "center":
                        paraFormat.FrameX = (short)PageNumberAlignment.Center;
                        break;
                    case "inside":
                        paraFormat.FrameX = (short)PageNumberAlignment.Inside;
                        break;
                    case "outside":
                        paraFormat.FrameX = (short)PageNumberAlignment.Outside;
                        break;
                    case"left":
                        paraFormat.FrameX = (short)PageNumberAlignment.Left;
                        break;
                }
            }
            string xPosition = reader.GetAttribute("x", DocxConstants.W_namespace);
            if (xPosition != null && xPosition != string.Empty)
                paraFormat.FrameX = float.Parse(xPosition, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;

            string yAlign = reader.GetAttribute("yAlign", DocxConstants.W_namespace);

            if (yAlign != null && yAlign != string.Empty)
            {
                switch (yAlign)
                {
                    case "top":
                        paraFormat.FrameY = (short)FrameVerticalPosition.Top;
                        break;
                    case "bottom":
                        paraFormat.FrameY = (short)FrameVerticalPosition.Bottom;
                        break;
                    case "center":
                        paraFormat.FrameY = (short)FrameVerticalPosition.Center;
                        break;
                    case "inside":
                        paraFormat.FrameY = (short)FrameVerticalPosition.Inside;
                        break;
                    case "outside":
                        paraFormat.FrameY = (short)FrameVerticalPosition.Outside;
                        break;
                    case "inline":
                        paraFormat.FrameY = (short)FrameVerticalPosition.Inline;
                        break;
                }
            }
            string yPosition = reader.GetAttribute("y", DocxConstants.W_namespace);
            if (yPosition != null && yPosition != string.Empty)
                paraFormat.FrameY = float.Parse(yPosition, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;

            string wrap = reader.GetAttribute("wrap", DocxConstants.W_namespace);
            if (wrap != null && wrap != string.Empty)
            {
                switch (wrap)
                {
                    case "notBeside":
                        paraFormat.WrapFrameAround = FrameWrapMode.NotBeside;
                        break;
                    case "around":
                        paraFormat.WrapFrameAround = FrameWrapMode.Around;
                        break;
                    case "none":
                        paraFormat.WrapFrameAround = FrameWrapMode.None;
                        break;
                    case "tight":
                        paraFormat.WrapFrameAround = FrameWrapMode.Tight;
                        break;
                    case "through":
                        paraFormat.WrapFrameAround = FrameWrapMode.Through;
                        break;
                    default:
                        paraFormat.WrapFrameAround = FrameWrapMode.Auto;
                        break;
                }
            }

            string hSpace = reader.GetAttribute("hSpace", DocxConstants.W_namespace);
            if (!string.IsNullOrEmpty(hSpace))
                paraFormat.FrameHorizontalDistanceFromText = float.Parse(hSpace, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;

            string vSpace = reader.GetAttribute("vSpace", DocxConstants.W_namespace);
            if (!string.IsNullOrEmpty(vSpace))
                paraFormat.FrameVerticalDistanceFromText = float.Parse(vSpace, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pFormat"></param>
        private void ParseTabs(XmlReader reader, WParagraphFormat paragraphFormat)
        {

            if (reader.LocalName != "tabs")
                throw new XmlException("Tab collection");

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != "tabs")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "tab":
                            Tab tab = paragraphFormat.Tabs.AddTab();

                            string tabValue = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (tabValue != null && tabValue != "clear")
                            {
                                tab.Justification = GetTabAlign(tabValue);
                            }
                            //Get the TabStop position
                            float position = GetFloatValue(reader, "pos", DocxConstants.W_namespace);
                            if (position != float.MaxValue)
                            {
                                if (tabValue == "clear")
                                    tab.DeletePosition = position * DLSConstants.TwipsInOnePoint;
                                else
                                    tab.Position = position;
                            }
                            //Get the tab leader
                            string leaderValue = reader.GetAttribute("leader", DocxConstants.W_namespace);
                            if (leaderValue != null)
                            {
                                tab.TabLeader = GetTabLeader(leaderValue);
                            }
                            break;

                    }
                    if (!skip)
                        reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the tab justification.
        /// </summary>
        /// <param name="align">The alignment.</param>
        /// <returns></returns>
        private TabJustification GetTabAlign(string align)
        {
            switch (align)
            {
                case "center":
                    return TabJustification.Centered;
                case "right":
                    return TabJustification.Right;
                case "decimal":
                    return TabJustification.Decimal;
                case "bar":
                    return TabJustification.Bar;
                case "num":
                    return TabJustification.List;
                default:
                    return TabJustification.Left;
            }
        }
        /// <summary>
        /// Parses the tab leader.
        /// </summary>
        /// <param name="leader">The leader.</param>
        /// <returns></returns>
        private TabLeader GetTabLeader(string leader)
        {
            switch (leader)
            {
                case "dot":
                    return TabLeader.Dotted;
                case "hyphen":
                    return TabLeader.Hyphenated;
                case "underscore":
                    return TabLeader.Single;
                case "heavy":
                    return TabLeader.Heavy;
                default:
                    return TabLeader.NoLeader;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="paragraphFormat"></param>
        private void ParseChangeParagraphFormat(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            paragraphFormat.IsChangedFormat = true;
            WParagraphFormat oldFormat = new WParagraphFormat(m_doc);
            //If empty return
            if (reader.IsEmptyElement)
                return;
            reader.Read();
            SkipWhitespaces(reader);
            //Parse Old paragraph format and ignore the same
            ParseParagraphFormat(reader, oldFormat);
            //Add sprmPWall with boolean value set to true indicating that the paragraph format is changed.
            SinglePropertyModifierRecord wallSprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmPWall);
            wallSprm.BoolValue = true;
            paragraphFormat.Sprms.Add(wallSprm);
            //Reverse sprm modifiers collection
            paragraphFormat.Sprms.Modifiers.Reverse();
        }
        /// <summary>
        /// Checks the track change.
        /// </summary>
        /// <param name="item">The item.</param>
        private void CheckTrackChange(ParagraphItem item)
        {
            if (m_trackChangeType == TrackChangeType.IsDelete)
                item.SetDeleteRev(true);
            else if (m_trackChangeType == TrackChangeType.IsInsert)
                item.SetInsertRev(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ent"></param>
        private void ParseBorders(XmlReader reader, IEntity ent)
        {
            Borders borders = null;

            if (ent is WTable)
            {
                if (IsTableChangeFormat)
                    borders = (ent as WTable).TrackTblFormat.Format.Borders;
                else
                    borders = (ent as WTable).DocxTableFormat.Format.Borders;
            }
            else if (ent is WTableRow)
            {
                if (IsRowChangeFormat)
                    borders = (ent as WTableRow).TrackRowFormat.Borders;
                else
                    borders = (ent as WTableRow).RowFormat.Borders;
            }
            else if (ent is WTableCell)
            {
                if (IsCellChangeFormat)
                    borders = (ent as WTableCell).TrackCellFormat.Borders;
                else
                    borders = (ent as WTableCell).CellFormat.Borders;
            }
            else if (ent is WSection)
            {
                borders = (ent as WSection).PageSetup.Borders;
            }
            else if (ent is WParagraph)
            {
                borders = (ent as WParagraph).ParagraphFormat.Borders;
            }

            if (borders != null)
            {
                ParseBorder(reader, borders);
            }
        }
        /// <summary>
        /// Parses the border.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="borders">The borders.</param>
        private void ParseBorder(XmlReader reader, Borders borders)
        {
            //if (reader.LocalName != "pBdr")
            //    throw new XmlException("Paragraph Borders");

            if (borders == null)
                throw new ArgumentException("Borders should not be null");

            if (reader.IsEmptyElement)
                return;
            string endNode = reader.LocalName;
            reader.Read();

            SkipWhitespaces(reader);

            while (reader.NodeType != XmlNodeType.EndElement && reader.LocalName != endNode)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "top":
                            ParseBorder(reader, borders.Top);
                            break;
                        case "left":
                            ParseBorder(reader, borders.Left);
                            break;
                        case "bottom":
                            ParseBorder(reader, borders.Bottom);
                            break;
                        case "right":
                            ParseBorder(reader, borders.Right);
                            break;
                        case "between":
                        case "insideH":
                            ParseBorder(reader, borders.Horizontal);
                            break;
                        case "bar":
                        case "insideV":
                            ParseBorder(reader, borders.Vertical);
                            break;
                        case "tl2br":
                            ParseBorder(reader, borders.DiagonalDown);
                            break;
                        case "tr2bl":
                            ParseBorder(reader, borders.DiagonalUp);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parse the paragraph spacings
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="paragraphFormat"></param>
        private void ParseSpacing(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            if (reader.AttributeCount == 0)
                return;

            // Parse before and afetr spacing.
            float value = GetFloatValue(reader, "before", DocxConstants.W_namespace);
            if (value != float.MaxValue)
                paragraphFormat.BeforeSpacing = value;

            value = GetFloatValue(reader, "after", DocxConstants.W_namespace);
            if (value != float.MaxValue)
                paragraphFormat.AfterSpacing = value;

            string strVal = reader.GetAttribute("beforeAutospacing", DocxConstants.W_namespace);
            if (strVal != null)
            {
                paragraphFormat.SpaceBeforeAuto = (strVal == "1") ? true : false;
            }

            strVal = reader.GetAttribute("afterAutospacing", DocxConstants.W_namespace);
            if (strVal != null)
            {
                paragraphFormat.SpaceAfterAuto = (strVal == "1") ? true : false;
            }

            // Parse line spacing
            ParseLineSpacing(reader, paragraphFormat);
        }
        /// <summary>
        /// Parse the line spacing of paragraph format
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="paragraphFormat"></param>
        private void ParseLineSpacing(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            float value = GetFloatValue(reader, "line", DocxConstants.W_namespace);
            if (value != float.MaxValue)
            {
                paragraphFormat.LineSpacing = 0;
                paragraphFormat.LineSpacing = value;
                string strVal = reader.GetAttribute("lineRule", DocxConstants.W_namespace);
                if (strVal != null)
                {
                    switch (strVal)
                    {
                        case "atLeast":
                            paragraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                            break;
                        case "exact":
                            paragraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                            break;
                        default:
                            paragraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Parse the paragraph indentation.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="paragraphFormat">The paragraph format</param>
        private void ParseIndentation(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            if (reader.AttributeCount == 0)
                return;

            // Read left indent
            float value = GetFloatValue(reader, "left", DocxConstants.W_namespace);
            if (value != float.MaxValue)
            {
                paragraphFormat.LeftIndent = value;
            }
            // Read right indent
            value = GetFloatValue(reader, "right", DocxConstants.W_namespace);
            if (value != float.MaxValue)
            {
                paragraphFormat.RightIndent = value;
            }
            // Read first line indent
            value = GetFloatValue(reader, "firstLine", DocxConstants.W_namespace);
            if (value != float.MaxValue)
            {
                paragraphFormat.FirstLineIndent = value;
            }
            //The firstLine and hanging attributes are mutually exclusive, if both are specified, then the firstLine value is ignored.
            // Read first line indent (hanging)
            value = GetFloatValue(reader, "hanging", DocxConstants.W_namespace);
            if (value != float.MaxValue)
            {
                paragraphFormat.FirstLineIndent = -value;
            }
            // Read left indent in character units
            value = GetFloatValue(reader, "leftChars", DocxConstants.W_namespace);
            if (value != float.MaxValue)
                paragraphFormat.LeftIndentChars = value * DLSConstants.TwipsInOnePoint / DLSConstants.HundredthsUnit;
            // Read right indent in character units
            value = GetFloatValue(reader, "rightChars", DocxConstants.W_namespace);
            if (value != float.MaxValue)
                paragraphFormat.RightIndentChars = value * DLSConstants.TwipsInOnePoint / DLSConstants.HundredthsUnit;
            // Read first line indent in character units
            value = GetFloatValue(reader, "firstLineChars", DocxConstants.W_namespace);
            if (value != float.MaxValue)
                paragraphFormat.FirstLineIndentChars = value * DLSConstants.TwipsInOnePoint / DLSConstants.HundredthsUnit;
            //The firstLine and hanging attributes are mutually exclusive, if both are specified, then the firstLine value is ignored.
            // Read first line indent (hanging) in character units
            value = GetFloatValue(reader, "hangingChars", DocxConstants.W_namespace);
            if (value != float.MaxValue)
                paragraphFormat.FirstLineIndentChars = -(value * DLSConstants.TwipsInOnePoint / DLSConstants.HundredthsUnit);
        }
        /// <summary>
        /// Parse the justification
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="paragraphFormat"></param>
        private void ParseParagraphJustification(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            string alignment = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (alignment != null)
            {
                switch (alignment)
                {
                    case "center":
                        paragraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "right":
                    case "lowKashida":
                        paragraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "both":
                        paragraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                        break;
                    default:
                        paragraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                }
            }
        }
        /// <summary>
        /// Parse the shading.
        /// </summary>
        /// <param name="pFormat"></param>
        /// <param name="reader"></param>
        private void ParseShading(XmlReader reader, WParagraphFormat paragraphFormat)
        {
            string value = reader.GetAttribute("fill", DocxConstants.W_namespace);
            if (value != null)
            {
                if (value == "auto")
                    paragraphFormat.BackColor = Color.Empty;
                else
                    paragraphFormat.BackColor = GetColorValue(value);
            }

            value = reader.GetAttribute("color", DocxConstants.W_namespace);
            if (value != null)
            {
                if (value == "auto")
                    paragraphFormat.ForeColor = Color.Empty;
                else
                    paragraphFormat.ForeColor = GetColorValue(value);
            }

            value = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (value != null)
            {
                paragraphFormat.TextureStyle = ParseTexture(value);
            }
        }
        #endregion Parser - Paragraph formats

        #region Section Properties
        /// <summary>
        /// Parse the section properties
        /// </summary>
        /// <param name="reader">The XmlReader</param>
        /// <param name="section">The section</param>
        private void ParseSectionProperties(XmlReader reader, IEntity entity)
        {
            if (reader.LocalName != "sectPr")
                throw new XmlException("Section properties");

            if (entity == null)
                throw new ArgumentException("Section should not be null");

            if (reader.IsEmptyElement)
                return;

            bool skip = false;

            reader.Read();

            SkipWhitespaces(reader);

            while (reader.LocalName != "sectPr")
            {
                skip = false;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "footerReference":
                        case "headerReference":
                            DictionaryEntry entry = m_docRelations[reader.GetAttribute("r:id")];
                            string type = reader.GetAttribute("type", DocxConstants.W_namespace);
                            bool isHeader = ((string)entry.Key == DocxConstants.HeaderRelType) ? true : false;
                            Part part = FindPart("word/", entry.Value.ToString());
                            m_currentFile = entry.Value.ToString() + ".rels";
                            ParseHeaderFooter(m_doc.LastSection.HeadersFooters, part, type, isHeader);
                            m_currentFile = "";
                            break;
                        case "type":
                            string secType = reader.GetAttribute("val", DocxConstants.W_namespace);
                            ParseSectionBreakType((entity as WSection), secType);
                            break;
                        case "pgSz":
                            ParsePageSize(reader, (entity as WSection));
                            break;
                        case "pgMar":
                            ParsePageMargins(reader, (entity as WSection));
                            break;
                        case "cols":
                            ParseColumns(reader, (entity as WSection));
                            break;
                        case "titlePg":
                            (entity as WSection).PageSetup.DifferentFirstPage = true;
                            break;
                        case "pgBorders":
                            ParsePageBorders(reader, (entity as WSection));
                            break;
                        case "docGrid":
                            ParseGrid(reader, (entity as WSection));
                            break;
                        case "vAlign":
                            ParseVertAlign(reader, (entity as WSection));
                            break;
                        case "lnNumType":
                            ParseLineNumbering(reader, (entity as WSection));
                            break;
                        case "footnotePr":
                            ParseFootnoteProp(reader, true, (entity as WSection));
                            break;
                        case "endnotePr":
                            ParseFootnoteProp(reader, false, (entity as WSection));
                            break;
                        case "textDirection":
                            ParseTextDirection(reader, (entity as WSection));
                            break;
                        case "rtlGutter":
                            if (GetBooleanValue(reader))
                                (entity as WSection).PageSetup.Margins.Right += m_gutter;
                            break;
                        case "pgNumType":
                            ParsePageNumberType(reader, (entity as WSection));
                            break;
                        case "bidi":
                            (entity as WSection).PageSetup.Bidi = GetBooleanValue(reader);
                            break;
                        case "formProt":
                            (entity as WSection).ProtectForm = GetBooleanValue(reader);
                            break;
                        default:
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }

        /// <summary>
        /// Parses the header footer.
        /// </summary>
        /// <param name="hf">The hf.</param>
        /// <param name="part">The part.</param>
        /// <param name="type">The type.</param>
        /// <param name="isHeader">if it is header, set to <c>true</c>.</param>
        private void ParseHeaderFooter(WHeadersFooters headerfooter, Part part, string type, bool isHeader)
        {
            IEntity entity = null;
            XmlReader reader = UtilityMethods.CreateReader(part.DataStream);
            switch (type)
            {
                case "default":
                    if (isHeader)
                        entity = headerfooter.OddHeader;
                    else
                        entity = headerfooter.OddFooter;
                    break;
                case "first":
                    if (isHeader)
                        entity = headerfooter.FirstPageHeader;
                    else
                        entity = headerfooter.FirstPageFooter;
                    break;
                case "even":
                    if (isHeader)
                        entity = headerfooter.EvenHeader;
                    else
                        entity = headerfooter.EvenFooter;
                    break;
                default:
                    break;
            }

            if (entity != null)
            {
                reader.MoveToContent();
                ParseBody(reader, entity);
            }
        }
        /// <summary>
        /// Parses the footnote and endnote property.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">Boolean value specifies whether the element is Footnote or Endnote.</param>
        private void ParseFootnoteProp(XmlReader reader, bool isFootnote, WSection section)
        {
            string end = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != end)
            {
                switch (reader.LocalName)
                {
                    case "pos":
                        string pos = reader.GetAttribute("val", DocxConstants.W_namespace);
                        if (pos != null && pos == "beneathText")
                        {
                            section.PageSetup.FootnotePosition = FootnotePosition.PrintImmediatelyBeneathText;
                        }
                        break;
                    case "numFmt":
                        ParseFootnoteNumFormat(reader, isFootnote, section);
                        break;
                    case "numRestart":
                        ParseFootnoteNumRestart(reader, isFootnote, section);
                        break;
                    case "numStart":
                        ParseFootnoteNumberStart(reader, isFootnote, section);
                        break;
                    default:
                        break;
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the footnote endnote number start.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        private void ParseFootnoteNumberStart(XmlReader reader, bool isFootnote, WSection section)
        {
            string numStart = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (numStart == null)
                return;

            if (isFootnote)
            {
                section.PageSetup.InitialFootnoteNumber = int.Parse(numStart);
            }
            else
            {
                section.PageSetup.InitialEndnoteNumber = int.Parse(numStart);
            }
        }
        /// <summary>
        /// Parses the footnote endnote number format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        private void ParseFootnoteNumFormat(XmlReader reader, bool isFootnote, WSection section)
        {
            string numFmt = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (numFmt == null)
                return;

            switch (numFmt)
            {
                case "decimal":
                    if (isFootnote)
                        section.PageSetup.FootnoteNumberFormat = FootEndNoteNumberFormat.Arabic;
                    else
                        section.PageSetup.EndnoteNumberFormat = FootEndNoteNumberFormat.Arabic;
                    break;
                case "lowerLetter":
                    if (isFootnote)
                        section.PageSetup.FootnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseLetter;
                    else
                        section.PageSetup.EndnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseLetter;
                    break;
                case "upperLetter":
                    if (isFootnote)
                        section.PageSetup.FootnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseLetter;
                    else
                        section.PageSetup.EndnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseLetter;
                    break;
                case "lowerRoman":
                    if (isFootnote)
                        section.PageSetup.FootnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseRoman;
                    else
                        section.PageSetup.EndnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseRoman;
                    break;
                case "upperRoman":
                    if (isFootnote)
                        section.PageSetup.FootnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseRoman;
                    else
                        section.PageSetup.EndnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseRoman;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Parses the footnote endnote number restart.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        private void ParseFootnoteNumRestart(XmlReader reader, bool isFootnote, WSection section)
        {
            string numRestart = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (numRestart == null)
                return;

            if (numRestart == "eachPage")
            {
                section.PageSetup.RestartIndexForFootnotes = FootnoteRestartIndex.RestartForEachPage;
            }
            else if (numRestart == "eachSect")
            {
                if (isFootnote)
                    section.PageSetup.RestartIndexForFootnotes = FootnoteRestartIndex.RestartForEachSection;
                else
                    section.PageSetup.RestartIndexForEndnote = EndnoteRestartIndex.RestartForEachSection;
            }
        }
        /// <summary>
        /// Parses the footnote and endnote property.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">Boolean value specifies whether the element is Footnote or Endnote.</param>
        private void ParseFootnoteProp(XmlReader reader, bool isFootnote)
        {
            string end = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != end)
            {
                switch (reader.LocalName)
                {
                    case "pos":
                        string pos = reader.GetAttribute("val", DocxConstants.W_namespace);
                        if (pos != null && pos == "beneathText")
                        {
                            m_doc.FootnotePosition = FootnotePosition.PrintImmediatelyBeneathText;
                        }
                        else if (!isFootnote && pos != null && pos == "sectEnd")
                        {
                            m_doc.EndnotePosition = EndnotePosition.DisplayEndOfSection;
                        }
                        break;
                    case "numFmt":
                        ParseFootnoteNumFormat(reader, isFootnote);
                        break;
                    case "numRestart":
                        ParseFootnoteNumRestart(reader, isFootnote);
                        break;
                    case "numStart":
                        ParseFootnoteNumberStart(reader, isFootnote);
                        break;
                    default:
                        break;
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Parses the footnote endnote number start.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        private void ParseFootnoteNumberStart(XmlReader reader, bool isFootnote)
        {
            string numStart = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (numStart == null)
                return;

            if (isFootnote)
            {
                m_doc.InitialFootnoteNumber = int.Parse(numStart);
            }
            else
            {
                m_doc.InitialEndnoteNumber = int.Parse(numStart);
            }
        }
        /// <summary>
        /// Parses the footnote endnote number format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        private void ParseFootnoteNumFormat(XmlReader reader, bool isFootnote)
        {
            string numFmt = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (numFmt == null)
                return;

            switch (numFmt)
            {
                case "decimal":
                    if (isFootnote)
                        m_doc.FootnoteNumberFormat = FootEndNoteNumberFormat.Arabic;
                    else
                        m_doc.EndnoteNumberFormat = FootEndNoteNumberFormat.Arabic;
                    break;
                case "lowerLetter":
                    if (isFootnote)
                        m_doc.FootnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseLetter;
                    else
                        m_doc.EndnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseLetter;
                    break;
                case "upperLetter":
                    if (isFootnote)
                        m_doc.FootnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseLetter;
                    else
                        m_doc.EndnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseLetter;
                    break;
                case "lowerRoman":
                    if (isFootnote)
                        m_doc.FootnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseRoman;
                    else
                        m_doc.EndnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseRoman;
                    break;
                case "upperRoman":
                    if (isFootnote)
                        m_doc.FootnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseRoman;
                    else
                        m_doc.EndnoteNumberFormat = FootEndNoteNumberFormat.UpperCaseRoman;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Parses the footnote endnote number restart.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="isFootnote">if it is footnote, set to <c>true</c>.</param>
        private void ParseFootnoteNumRestart(XmlReader reader, bool isFootnote)
        {
            string numRestart = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (numRestart == null)
                return;

            if (numRestart == "eachPage")
            {
                m_doc.RestartIndexForFootnotes = FootnoteRestartIndex.RestartForEachPage;
            }
            else if (numRestart == "eachSect")
            {
                if (isFootnote)
                    m_doc.RestartIndexForFootnotes = FootnoteRestartIndex.RestartForEachSection;
                else
                    m_doc.RestartIndexForEndnote = EndnoteRestartIndex.RestartForEachSection;
            }
        }
        /// <summary>
        /// Parses the type and number start of the page.
        /// </summary>
        /// <param name="sec">The sec.</param>
        /// <param name="reader">The reader.</param>
        private void ParsePageNumberType(XmlReader reader, WSection section)
        {
            string numberType = reader.GetAttribute("fmt", DocxConstants.W_namespace);
            if (numberType != null & numberType != string.Empty)
                switch (numberType)
                {
                    case "decimal":
                        section.PageSetup.PageNumberStyle = PageNumberStyle.Arabic;
                        break;
                    case "lowerRoman":
                        section.PageSetup.PageNumberStyle = PageNumberStyle.RomanLower;
                        break;
                    case "upperRoman":
                        section.PageSetup.PageNumberStyle = PageNumberStyle.RomanUpper;
                        break;
                    case "lowerLetter":
                        section.PageSetup.PageNumberStyle = PageNumberStyle.LetterLower;
                        break;
                    case "upperLetter":
                        section.PageSetup.PageNumberStyle = PageNumberStyle.LetterUpper;
                        break;
                }

            string startingNumber = reader.GetAttribute("start", DocxConstants.W_namespace);
            if (startingNumber != null && startingNumber != string.Empty)
            {
                section.PageSetup.RestartPageNumbering = true;
                section.PageSetup.PageStartingNumber = Int32.Parse(startingNumber);
            }
            string chapterNumberStyle = reader.GetAttribute("chapStyle", DocxConstants.W_namespace);
            if (chapterNumberStyle != null && chapterNumberStyle != string.Empty)
            {
                section.PageSetup.PageNumbers.HeadingLevelForChapter = (HeadingLevel)Int32.Parse(chapterNumberStyle);
            }
            string chapterNumberSeprator = reader.GetAttribute("chapSep", DocxConstants.W_namespace);
            if (chapterNumberSeprator != null & chapterNumberSeprator != string.Empty)
                switch (chapterNumberSeprator)
                {
                    case "colon":
                        section.PageSetup.PageNumbers.ChapterPageSeparator = ChapterPageSeparatorType.Colon;
                        break;
                    case "hyphen":
                        section.PageSetup.PageNumbers.ChapterPageSeparator = ChapterPageSeparatorType.Hyphen;
                        break;
                    case "period":
                        section.PageSetup.PageNumbers.ChapterPageSeparator = ChapterPageSeparatorType.Period;
                        break;
                    case "emDash":
                        section.PageSetup.PageNumbers.ChapterPageSeparator = ChapterPageSeparatorType.EmDash;
                        break;
                    case "enDash":
                        section.PageSetup.PageNumbers.ChapterPageSeparator = ChapterPageSeparatorType.EnDash;
                        break;
                }


        }
        /// <summary>
        /// Parses the text direction.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ent">The ent.</param>
        private void ParseTextDirection(XmlReader reader, IEntity ent)
        {
            string direction = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (direction == null)
                return;

            switch (direction)
            {
                case "tb":
                case "lrTb":
                    (ent as WSection).TextDirection = DocTextDirection.LeftToRight;
                    break;
                case "rl":
                case "tbRl":
                    (ent as WSection).TextDirection = DocTextDirection.TopToBottom;
                    break;
                //case "lrTbV":
                //    (ent as WSection).TextDirection = DocTextDirection.TopToBottomRotated;
                //    break;
                case "lr":
                case "btLr":
                    (ent as WSection).TextDirection = DocTextDirection.LeftToRightRotated;
                    break;
                case "tbV":
                case "lrTbV":
                    (ent as WSection).TextDirection = DocTextDirection.RightToLeft;
                    break;
                case "rlV":
                case "tbRlV":
                    (ent as WSection).TextDirection = DocTextDirection.RightToLeftRotated;
                    break;
            }
        }
        /// <summary>
        /// Parses the line numbering.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The sec.</param>
        private void ParseLineNumbering(XmlReader reader, WSection section)
        {
            WPageSetup pageSetup = section.PageSetup;
            pageSetup.HasLineNumbering = true;

            string strVal = reader.GetAttribute("countBy", DocxConstants.W_namespace);
            if (strVal != null)
                pageSetup.LineNumberingStep = Int32.Parse(strVal, NumberStyles.Integer, CultureInfo.InvariantCulture);

            strVal = reader.GetAttribute("start", DocxConstants.W_namespace);
            if (strVal != null)
                pageSetup.LineNumberingStartValue = Int32.Parse(strVal, NumberStyles.Integer, CultureInfo.InvariantCulture) + 1;

            float floatVal = GetFloatValue(reader, "distance", DocxConstants.W_namespace);
            if (floatVal != float.MaxValue)
                pageSetup.LineNumberingDistanceFromText = floatVal;

            strVal = reader.GetAttribute("restart", DocxConstants.W_namespace);
            if (strVal != null)
            {
                switch (strVal)
                {
                    case "newPage":
                        pageSetup.LineNumberingMode = LineNumberingMode.RestartPage;
                        break;
                    case "newSection":
                        pageSetup.LineNumberingMode = LineNumberingMode.RestartSection;
                        break;
                    case "continuous":
                        pageSetup.LineNumberingMode = LineNumberingMode.Continuous;
                        break;
                }
            }
        }
        /// <summary>
        /// Parse the vertical alignment of the page.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="sec">The section</param>
        private void ParseVertAlign(XmlReader reader, WSection section)
        {
            string align = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (align != null)
            {
                switch (align)
                {
                    case "top":
                        section.PageSetup.VerticalAlignment = PageAlignment.Top;
                        break;
                    case "center":
                        section.PageSetup.VerticalAlignment = PageAlignment.Middle;
                        break;
                    case "both":
                        section.PageSetup.VerticalAlignment = PageAlignment.Justified;
                        break;
                    case "bottom":
                        section.PageSetup.VerticalAlignment = PageAlignment.Bottom;
                        break;
                }
            }
        }

        /// <summary>
        /// Parses the grid.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The section.</param>
        private void ParseGrid(XmlReader reader, WSection section)
        {
            float pitch = GetFloatValue(reader, "linePitch", DocxConstants.W_namespace);
            if (pitch == float.MaxValue)
                return;

            section.PageSetup.LinePitch = pitch;
            string type = reader.GetAttribute("type", DocxConstants.W_namespace);
            if (type != null)
            {
                switch (type)
                {
                    case "lines":
                        section.PageSetup.PitchType = GridPitchType.LinesOnly;
                        break;
                    case "linesAndChars":
                        section.PageSetup.PitchType = GridPitchType.CharsAndLine;
                        break;
                    case "snapToChars":
                        section.PageSetup.PitchType = GridPitchType.SnapToChars;
                        break;
                }
            }
        }

        /// <summary>
        /// Parses the page borders.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The section.</param>
        private void ParsePageBorders(XmlReader reader, WSection section)
        {
            string offsetFrom = reader.GetAttribute("offsetFrom", DocxConstants.W_namespace);
            if (offsetFrom != null)
            {
                section.PageSetup.PageBorderOffsetFrom = (offsetFrom == "page") ?
                  PageBorderOffsetFrom.PageEdge : PageBorderOffsetFrom.Text;
            }

            //zOrder of page border
            string zOrder = reader.GetAttribute("zOrder", DocxConstants.W_namespace);
            if (zOrder == "back")
            {
                section.PageSetup.IsFrontPageBorder = false;
            }
            string applyType = reader.GetAttribute("display", DocxConstants.W_namespace);
            if (applyType != null)
            {
                if (applyType == "firstPage")
                    section.PageSetup.PageBordersApplyType = PageBordersApplyType.FirstPage;
                else if (applyType == "notFirstPage")
                    section.PageSetup.PageBordersApplyType = PageBordersApplyType.AllExceptFirstPage;
            }

            ParseBorders(reader, section);
        }

        /// <summary>
        /// Parses the columns.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The sec.</param>
        private void ParseColumns(XmlReader reader, WSection section)
        {
            string drawLinesBetween = reader.GetAttribute("sep", DocxConstants.W_namespace);
            if (drawLinesBetween != null && drawLinesBetween == "1")
                section.PageSetup.DrawLinesBetweenCols = true;

            if (!CheckEqualColumn(reader))
            {
                section.Columns.OwnerSection.PageSetup.EqualColumnWidth = false;
                ParseColumn(reader, section);
            }
            else
            {
                section.Columns.OwnerSection.PageSetup.EqualColumnWidth = true;
                ParseEqualColumns(reader, section);
            }
        }
        /// <summary>
        /// Checks the equal column.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private bool CheckEqualColumn(XmlReader reader)
        {
            if (reader.GetAttribute("equalWidth", DocxConstants.W_namespace) == "0")
                return false;
            else
                return true;
        }
        /// <summary>
        /// Parses the equal column.
        /// </summary>
        /// <param name="sec">The sec.</param>
        /// <param name="reader">The reader.</param>
        private void ParseEqualColumns(XmlReader reader, WSection section)
        {
            float pageWidht = section.PageSetup.PageSize.Width * DocxConstants.TwentiethOfPoint;
            float left = section.PageSetup.Margins.Left * DocxConstants.TwentiethOfPoint;
            float right = section.PageSetup.Margins.Right * DocxConstants.TwentiethOfPoint;
            float space = 0;
            float width = 0;
            int num = 1;

            if (reader.GetAttribute("num", DocxConstants.W_namespace) != null)
            {
                num = int.Parse(reader.GetAttribute("num", DocxConstants.W_namespace));
            }
            if (reader.GetAttribute("space", DocxConstants.W_namespace) != null)
            {
                space = float.Parse(reader.GetAttribute("space", DocxConstants.W_namespace), CultureInfo.InvariantCulture);
            }
            width = (pageWidht - left - right - space * num - 1) / num;

            for (int i = 0; i < num; i++)
            {
                Column col = new Column(m_doc);
                col.Space = space / DocxConstants.TwentiethOfPoint;
                col.Width = width / DocxConstants.TwentiethOfPoint;
                section.Columns.Add(col);
            }
        }
        /// <summary>
        /// Parses the column.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ent">The entity.</param>
        private void ParseColumn(XmlReader reader, WSection section)
        {
            string end = reader.LocalName;
            reader.Read();
            SkipWhitespaces(reader);
            while (reader.LocalName != end)
            {
                switch (reader.LocalName)
                {
                    case "col":
                        float space = float.MaxValue;
                        float width = float.MaxValue;

                        if (reader.GetAttribute("space", DocxConstants.W_namespace) != null)
                        {
                            space = int.Parse(reader.GetAttribute("space", DocxConstants.W_namespace));
                        }
                        if (reader.GetAttribute("w", DocxConstants.W_namespace) != null)
                        {
                            width = int.Parse(reader.GetAttribute("w", DocxConstants.W_namespace));
                        }

                        Column col = new Column(m_doc);
                        if (space != float.MaxValue)
                            col.Space = space / DocxConstants.TwentiethOfPoint;

                        if (width != float.MaxValue)
                        {
                            col.Width = width / DocxConstants.TwentiethOfPoint;
                        }

                        section.Columns.Add(col);
                        break;
                }
                reader.Read();
                SkipWhitespaces(reader);
            }
        }

        /// <summary>
        /// Parses the page margins.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The section.</param>
        private void ParsePageMargins(XmlReader reader, WSection section)
        {
            float top = GetMarginValue(reader, "top");
            float right = GetMarginValue(reader, "right");
            float bottom = GetMarginValue(reader, "bottom");
            float left = GetMarginValue(reader, "left");
            float footer = GetMarginValue(reader, "footer");
            float header = GetMarginValue(reader, "header");
            float gutter = GetMarginValue(reader, "gutter");

            section.PageSetup.Margins = new MarginsF(left, top, right, bottom);
            section.PageSetup.Margins.Gutter = gutter;

            if (footer != -1)
                section.PageSetup.FooterDistance = footer;

            if (header != -1)
                section.PageSetup.HeaderDistance = header;

            m_gutter = gutter;
        }
        /// <summary>
        /// Gets the margin value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns></returns>
        private float GetMarginValue(XmlReader reader, string attributeName)
        {
            float value = (attributeName == "footer" || attributeName == "header") ? -1 : 0;

            string margVal = reader.GetAttribute(attributeName, DocxConstants.W_namespace);
            if (margVal != null)
                value = float.Parse(margVal, CultureInfo.InvariantCulture) / DocxConstants.TwentiethOfPoint;

            return value;
        }
        /// <summary>
        /// Parses the size of the page.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The section.</param>
        private void ParsePageSize(XmlReader reader, WSection section)
        {
            float pageHeight = float.Parse(reader.GetAttribute("h", DocxConstants.W_namespace), CultureInfo.InvariantCulture) / DocxConstants.TwentiethOfPoint;
            float pageWidth = float.Parse(reader.GetAttribute("w", DocxConstants.W_namespace), CultureInfo.InvariantCulture) / DocxConstants.TwentiethOfPoint;
            section.PageSetup.PageSize = new SizeF(pageWidth, pageHeight);

            // Parse page orientation
            string pageOrient = reader.GetAttribute("orient", DocxConstants.W_namespace);
            section.PageSetup.Orientation = (pageOrient == "landscape") ? PageOrientation.Landscape : PageOrientation.Portrait;
        }

        /// <summary>
        /// Parses the type of the section.
        /// </summary>
        /// <param name="sec">The sec.</param>
        /// <param name="type">The type.</param>
        private void ParseSectionBreakType(WSection section, string sectionBreakType)
        {
            switch (sectionBreakType)
            {
                case "nextColumn":
                    section.BreakCode = SectionBreakCode.NewColumn;
                    break;
                case "nextPage":
                    section.BreakCode = SectionBreakCode.NewPage;
                    break;
                case "evenPage":
                    section.BreakCode = SectionBreakCode.EvenPage;
                    break;
                case "oddPage":
                    section.BreakCode = SectionBreakCode.Oddpage;
                    break;
                default:
                    section.BreakCode = SectionBreakCode.NoBreak;
                    break;
            }
        }
        #endregion Section Properties

        #region Settings
        /// <summary>
        /// Parses the settings relations.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void ParseSettingsRelations(Stream stream)
        {
            stream.Position = 0;
            XmlReader hfRelReader = UtilityMethods.CreateReader(stream);
            ParseRelations(hfRelReader, SettingsRelations);
        }
        /// <summary>
        /// Initialize word 2010 specific Compatiblity Settings
        /// </summary>
        private void InitializeCompactSettings()
        {
            if (AppVersion == "Word2010" || AppVersion == "Word2013")
            {
                //Initialize word 2010 specific Compatibility options as false in order to avoid default value while serialization
                m_doc.Settings.CompatibilityOptions[CompatibilityOption.overrideTableStyleFontSizeAndJustification] = false;
                m_doc.Settings.CompatibilityOptions[CompatibilityOption.enableOpenTypeFeatures] = false;
                m_doc.Settings.CompatibilityOptions[CompatibilityOption.doNotFlipMirrorIndents] = false;
            }
            if (AppVersion == "Word2010" || AppVersion == "Word2013")
            {
                m_doc.Settings.CompatibilityOptions[CompatibilityOption.differentiateMultirowTableHeader] = false;
            }
        }
        private void ParseSettings(Stream stream)
        {
            // Initialize word 2010 or word 2013 specific Compatiblity Settings
            InitializeCompactSettings();
            XmlReader reader = UtilityMethods.CreateReader(stream);

            if (reader == null)
                throw new Exception("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "settings")
                throw new XmlException("Unexpected xml tag " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            bool skip = false;
            bool displayBackground = false;

            reader.Read();

            while (reader.LocalName != "settings")
            {
                skip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "zoom":
                            string percent = reader.GetAttribute("percent", DocxConstants.W_namespace);
                            if (percent != null)
                            {
                                int value = Int32.Parse(percent, NumberStyles.Integer, CultureInfo.InvariantCulture);
                                m_doc.ViewSetup.SetZoomPercent(value);
                            }
                            break;
                        case "defaultTabStop":
                            m_doc.DefaultTabWidth = GetFloatValue(reader, "val", DocxConstants.W_namespace);
                            break;
                        case "mirrorMargins":
                            m_doc.DOP.MirrorMargins = true;
                            break;
                        case "displayBackgroundShape":
                            displayBackground = true;
                            break;
                        case "doNotDisplayPageBoundaries":
                            m_doc.DOP.Dop2000.NoMargPgvwSaved = true;
                            break;
                        case "trackRevisions":
                            m_doc.TrackChanges = true;
                            break;
                        case "uiCompat97To2003":
                            m_doc.Settings.CompatibilityMode = CompatibilityMode.Word2003;
                            break;
                        case "hdrShapeDefaults":
                        case "rsids":
                        case "mathPr":
                        case "themeFontLang":
                        case "clrSchemeMapping":
                        case "shapeDefaults":
                            reader.Skip();
                            skip = true;
                            break;
                        case "footnotePr":
                            ParseFootnoteProp(reader, true);
                            break;
                        case "endnotePr":
                            ParseFootnoteProp(reader, false);
                            break;
                        case "evenAndOddHeaders":
                            m_doc.DifferentOddAndEvenPages = true;
                            break;
                        case "docVars":
                            ParseVariables(reader);
                            break;
                        case "view":
                            ParseViewType(reader);
                            break;
                        case "documentProtection":
                            ParseProtectType(reader);
                            break;
                        case "compat":
                            ParseCompatNode(reader);
                            break;
                        case "compatSetting":
                            ParseCompatSettingNode(reader);
                            break;
                        case "autoHyphenation":
                            m_doc.DOP.AutoHyphen = GetBooleanValue(reader);
                            break;
                        case "consecutiveHyphenLimit":
                            string hyphenLimit = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (hyphenLimit != null)
                                m_doc.DOP.ConsecHypLim = Int32.Parse(hyphenLimit, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case "hyphenationZone":
                            string hyphenationZone = reader.GetAttribute("val", DocxConstants.W_namespace);
                            if (hyphenationZone != null)
                                m_doc.DOP.DxaHotZ = Int32.Parse(hyphenationZone, NumberStyles.Integer, CultureInfo.InvariantCulture);
                            break;
                        case "doNotHyphenateCaps":
                            m_doc.DOP.HyphCapitals = !GetBooleanValue(reader);
                            break;
                        case "attachedTemplate":
                            string id = reader.GetAttribute("id", DocxConstants.R_namespace);
                            if (SettingsRelations.ContainsKey(id))
                                m_doc.AssociatedStrings.AttachedTemplate = SettingsRelations[id].Value.ToString().Replace("file:///", string.Empty);
                            break;
                        case "linkStyles":
                            m_doc.DOP.LinkStyles = GetBooleanValue(reader);
                            break;
                        default:
                            skip = ParseCompatibiltyOption(reader);
                            break;
                    }
                    if (!skip)
                        reader.Read();
                }
                else
                    reader.Read();
            }
            if (!displayBackground)
                m_doc.Background.Type = BackgroundType.NoBackground;
        }
        /// <summary>
        /// Parses the Compatibilty Option.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private bool ParseCompatibiltyOption(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                case "useSingleBorderforContiguousCells":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.OrigWordTableRules] = GetBooleanValue(reader);
                    break;
                case "wpJustification":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.WPJust] = GetBooleanValue(reader);
                    break;
                case "noTabHangInd":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.NoTabForInd] = GetBooleanValue(reader);
                    break;
                case "noLeading":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.NoExtLeading] = GetBooleanValue(reader);
                    break;
                case "spaceForUL":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontMakeSpaceForUL] = !GetBooleanValue(reader);
                    break;
                case "noColumnBalance":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.NoColumnBalance] = GetBooleanValue(reader);
                    break;
                case "balanceSingleByteDoubleByteWidth":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DntBlnSbDbWid] = !GetBooleanValue(reader);
                    break;
                case "noExtraLineSpacing":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ExactOnTop] = GetBooleanValue(reader);
                    break;
                case "doNotLeaveBackslashAlone":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.LeaveBackslashAlone] = !GetBooleanValue(reader);
                    break;
                case "ulTrailSpace":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DntULTrlSpc] = !GetBooleanValue(reader);
                    break;
                case "doNotExpandShiftReturn":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ExpShRtn] = !GetBooleanValue(reader);
                    break;
                case "spacingInWholePoints":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.TruncDxaExpand] = GetBooleanValue(reader);
                    break;
                case "lineWrapLikeWord6":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.LineWrapLikeWord6] = GetBooleanValue(reader);
                    break;
                case "printBodyTextBeforeHeader":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.PrintBodyBeforeHdr] = GetBooleanValue(reader);
                    break;
                case "printColBlack":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.MapPrintTextColor] = GetBooleanValue(reader);
                    break;
                case "wpSpaceWidth":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.WPSpace] = GetBooleanValue(reader);
                    break;
                case "showBreaksInFrames":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ShowBreaksInFrames] = GetBooleanValue(reader);
                    break;
                case "subFontBySize":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SubOnSize] = GetBooleanValue(reader);
                    break;
                case "suppressBottomSpacing":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ExtraAfter] = GetBooleanValue(reader);
                    break;
                case "suppressTopSpacing":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SuppressTopSpacing] = GetBooleanValue(reader);
                    break;
                case "suppressSpacingAtTopOfPage":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SuppressTopSpacingMac5] = GetBooleanValue(reader);
                    break;
                case "suppressTopSpacingWP":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.F2ptExtLeadingOnly] = GetBooleanValue(reader);
                    break;
                case "suppressSpBfAfterPgBrk":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SuppressSpBfAfterPgBrk] = GetBooleanValue(reader);
                    break;
                case "swapBordersFacingPages":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SwapBordersFacingPgs] = GetBooleanValue(reader);
                    break;
                case "convMailMergeEsc":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ConvMailMergeEsc] = GetBooleanValue(reader);
                    break;
                case "truncateFontHeightsLikeWP6":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.TruncFontHeight] = GetBooleanValue(reader);
                    break;
                case "mwSmallCaps":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.MWSmallCaps] = GetBooleanValue(reader);
                    break;
                case "usePrinterMetrics":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.PrintMet] = GetBooleanValue(reader);
                    break;
                case "doNotSuppressParagraphBorders":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.WW6BorderRules] = GetBooleanValue(reader);
                    break;
                case "wrapTrailSpaces":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.WrapTrailSpaces] = GetBooleanValue(reader);
                    break;
                case "footnoteLayoutLikeWW8":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.FtnLayoutLikeWW8] = GetBooleanValue(reader);
                    break;
                case "shapeLayoutLikeWW8":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SpLayoutLikeWW8] = GetBooleanValue(reader);
                    break;
                case "alignTablesRowByRow":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.AlignTablesRowByRow] = GetBooleanValue(reader);
                    break;
                case "forgetLastTabAlignment":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ForgetLastTabAlign] = GetBooleanValue(reader);
                    break;
                case "adjustLineHeightInTable":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontAdjustLineHeightInTable] = !GetBooleanValue(reader);
                    break;
                case "autoSpaceLikeWord95":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.UseAutospaceForFullWidthAlpha] = GetBooleanValue(reader);
                    break;
                case "noSpaceRaiseLower":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.NoSpaceRaiseLower] = GetBooleanValue(reader);
                    break;
                case "doNotUseHTMLParagraphAutoSpacing":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontUseHTMLParagraphAutoSpacing] = GetBooleanValue(reader);
                    break;
                case "layoutRawTableWidth":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.LayoutRawTableWidth] = GetBooleanValue(reader);
                    break;
                case "layoutTableRowsApart":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.LayoutTableRowsApart] = GetBooleanValue(reader);
                    break;
                case "useWord97LineBreakRules":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.UseWord97LineBreakingRules] = GetBooleanValue(reader);
                    break;
                case "doNotBreakWrappedTables":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontBreakWrappedTables] = GetBooleanValue(reader);
                    break;
                case "doNotSnapToGridInCell":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontSnapToGridInCell] = GetBooleanValue(reader);
                    break;
                case "selectFldWithFirstOrLastChar":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontAllowFieldEndSelect] = GetBooleanValue(reader);
                    break;
                case "applyBreakingRules":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.ApplyBreakingRules] = GetBooleanValue(reader);
                    break;
                case "doNotWrapTextWithPunct":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontWrapTextWithPunct] = GetBooleanValue(reader);
                    break;
                case "doNotUseEastAsianBreakRules":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontUseAsianBreakRules] = GetBooleanValue(reader);
                    break;
                case "useWord2002TableStyleRules":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.UseWord2002TableStyleRules] = GetBooleanValue(reader);
                    break;
                case "growAutofit":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.GrowAutoFit] = GetBooleanValue(reader);
                    break;
                case "useNormalStyleForList":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.UseNormalStyleForList] = GetBooleanValue(reader);
                    break;
                case "doNotUseIndentAsNumberingTabStop":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontUseIndentAsNumberingTabStop] = GetBooleanValue(reader);
                    break;
                case "useAltKinsokuLineBreakRules":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.FELineBreak11] = GetBooleanValue(reader);
                    break;
                case "allowSpaceOfSameStyleInTable":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.AllowSpaceOfSameStyleInTable] = GetBooleanValue(reader);
                    break;
                case "doNotSuppressIndentation":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.WW11IndentRules] = GetBooleanValue(reader);
                    break;
                case "doNotAutofitConstrainedTables":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontAutofitConstrainedTables] = GetBooleanValue(reader);
                    break;
                case "autofitToFirstFixedWidthCell":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.AutofitLikeWW11] = GetBooleanValue(reader);
                    break;
                case "underlineTabInNumList":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.UnderlineTabInNumList] = GetBooleanValue(reader);
                    break;
                case "displayHangulFixedWidth":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.HangulWidthLikeWW11] = GetBooleanValue(reader);
                    break;
                case "splitPgBreakAndParaMark":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.SplitPgBreakAndParaMark] = GetBooleanValue(reader);
                    break;
                case "doNotVertAlignCellWithSp":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontVertAlignCellWithSp] = GetBooleanValue(reader);
                    break;
                case "doNotBreakConstrainedForcedTable":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontBreakConstrainedForcedTables] = GetBooleanValue(reader);
                    break;
                case "doNotVertAlignInTxbx":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.DontVertAlignInTxbx] = GetBooleanValue(reader);
                    break;
                case "useAnsiKerningPairs":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.Word11KerningPairs] = GetBooleanValue(reader);
                    break;
                case "cachedColBalance":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.CachedColBalance] = GetBooleanValue(reader);
                    break;
                case "compatSetting":
                    ParseCompatSettingNode(reader);
                    break;
                default:
                    m_doc.DocxProps.Add(ReadSingleNodeIntoStream(reader));
                    return true;
                    break;
            }
            return false;
        }
        /// <summary>
        /// Parses the compat Node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseCompatNode(XmlReader reader)
        {
            if (reader == null)
                throw new Exception("reader");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            if (reader.LocalName != "compat")
                throw new XmlException("Expected xml tag \"compat\"");

            if (reader.IsEmptyElement)
                return;

            reader.Read();

            while (reader.LocalName != "compat")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!ParseCompatibiltyOption(reader))
                        reader.Read();
                }
                else
                    reader.Read();
            }
        }
        /// <summary>
        /// Parses the CompatSetting Node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseCompatSettingNode(XmlReader reader)
        {
            string option = reader.GetAttribute("name", DocxConstants.W_namespace);
            switch (option)
            {
                case "compatibilityMode":
                    m_doc.Settings.CompatibilityMode = GetCompatibilityMode(reader);
                    break;
                case "overrideTableStyleFontSizeAndJustification":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.overrideTableStyleFontSizeAndJustification] = GetBooleanValue(reader);
                    break;
                case "enableOpenTypeFeatures":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.enableOpenTypeFeatures] = GetBooleanValue(reader);
                    break;
                case "doNotFlipMirrorIndents":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.doNotFlipMirrorIndents] = GetBooleanValue(reader);
                    break;
                case "differentiateMultirowTableHeader":
                    m_doc.Settings.CompatibilityOptions[CompatibilityOption.differentiateMultirowTableHeader] = GetBooleanValue(reader);
                    break;
            }
        }
        /// <summary>
        /// Parses the document variables.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseVariables(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            reader.Read();
            do
            {
                string key = reader.GetAttribute("name", DocxConstants.W_namespace);
                string value = reader.GetAttribute("val", DocxConstants.W_namespace);

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    m_doc.Variables.Items.Add(key, ParseString(value));

                reader.Read();
            }
            while (reader.LocalName != "docVars");
        }
        /// <summary>
        /// replaces hexadecimal values with equivalent string
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns></returns>
        private string ParseString(string text)
        {
            const int HexaLength = 4;
            const string HexaStart = "_x";
            const string HexaEnd = "_";
            const int PrefixStartLen = 2;
            StringBuilder sb = new StringBuilder(text);

            for (int i = 0; i < text.Length; )
            {
                int startIndex = text.IndexOf(HexaStart, i);

                if (startIndex == -1)
                    break;

                startIndex += PrefixStartLen;
                int endIndex = text.IndexOf(HexaEnd, startIndex);

                if (endIndex == -1)
                    break;

                int length = endIndex - startIndex;

                if (length == HexaLength)
                {
                    string value = text.Substring(startIndex, 4);
                    int result;
                    if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                    {
#if !SILVERLIGHT && !WP
                        sb.Replace(HexaStart + value + HexaEnd, char.ConvertFromUtf32(result));
#else
                        sb.Replace(HexaStart + value + HexaEnd, Convert.ToString((char)result));
#endif
                    }
                }
                i = endIndex;
            }
            return sb.ToString();
        }
        /// <summary>
        /// Parses the type of the protect.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseProtectType(XmlReader reader)
        {
            string type = reader.GetAttribute("edit", DocxConstants.W_namespace);
            if (type == null || type == string.Empty)
                return;

            switch (type)
            {
                case "comments":
                    m_doc.ProtectionType = ProtectionType.AllowOnlyComments;
                    break;
                case "forms":
                    m_doc.ProtectionType = ProtectionType.AllowOnlyFormFields;
                    break;
                case "trackedChanges":
                    m_doc.ProtectionType = ProtectionType.AllowOnlyRevisions;
                    break;
                case "readOnly":
                    m_doc.ProtectionType = ProtectionType.AllowOnlyReading;
                    break;
                default:
                    break;
            }
            //Parse Document protection Enforcement
            string enforcement = reader.GetAttribute("enforcement", DocxConstants.W_namespace);
            if (enforcement != null || enforcement != string.Empty)
            {
                bool isEnforcement = GetBoolValue(enforcement);
                if (!isEnforcement)
                {
                    switch (m_doc.ProtectionType)
                    {
                        case ProtectionType.AllowOnlyComments:
                            m_doc.DOP.m_bLockAtn = false;
                            break;
                        case ProtectionType.AllowOnlyFormFields:
                            m_doc.DOP.m_bProtEnabled = false;
                            break;
                        case ProtectionType.AllowOnlyRevisions:
                            m_doc.DOP.m_bLockRev = false;
                            break;
                        case ProtectionType.AllowOnlyReading:
                            m_doc.DOP.m_bLockAtn = false;
                            m_doc.DOP.Dop2003.TreatLockAtnAsReadOnly = false;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Parses the view type.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ParseViewType(XmlReader reader)
        {
            string view = reader.GetAttribute("val", DocxConstants.W_namespace);
            if (view != null)
            {
                switch (view)
                {
                    case "web":
                        m_doc.ViewSetup.DocumentViewType = DocumentViewType.WebLayout;
                        break;
                    case "outline":
                        m_doc.ViewSetup.DocumentViewType = DocumentViewType.OutlineLayout;
                        break;
                }
            }
        }
        #endregion Settings

        #region Helper Methods
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="para">The para.</param>
        private void AddItem(ParagraphItem item, ParagraphItemCollection paragraphItems)
        {
            paragraphItems.Add(item);
        }
        /// <summary>
        /// Adds to paragraph.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="para">The para.</param>
        private void AddToParagraph(ParagraphItem item, ParagraphItemCollection paragraphItems)
        {
            AddItem(item, paragraphItems);
            UpdateCommentItems(item);
            if (item is WOleObject)
                AddOleObject(item as WOleObject, paragraphItems);

        }
        /// <summary>
        /// Adds the OLE object.
        /// </summary>
        /// <param name="oleObject">The OLE object.</param>
        /// <param name="para">The para.</param>
        private void AddOleObject(WOleObject oleObject, ParagraphItemCollection paragraphItems)
        {
            // Add field separator
            WFieldMark separator = new WFieldMark(m_doc);
            separator.Type = FieldMarkType.FieldSeparator;
            AddToParagraph(separator, paragraphItems);

            // Add ole picture
            WPicture olePicture = oleObject.OlePicture;
            AddToParagraph(olePicture, paragraphItems);

            // Add field end
            WFieldMark fldMark = new WFieldMark(m_doc);
            fldMark.Type = FieldMarkType.FieldEnd;
            AddToParagraph(fldMark, paragraphItems);
        }
        /// <summary>
        /// Gets the image id.
        /// </summary>
        /// <param name="imageId">The image id.</param>
        /// <param name="isHeaderFooter">if set to <c>true</c> [is header footer].</param>
        /// <param name="isPicBullet">if set to <c>true</c> [is pic bullet].</param>
        /// <returns></returns>
        private string GetImageName(string imageId, bool isHeaderFooter, bool isPicBullet)
        {
            string imageName;
            if (isHeaderFooter)
            {
                Dictionary<string, DictionaryEntry> entryHF = GetFileRelations(m_currentFile);
                DictionaryEntry typeAndTarget = entryHF[imageId];
                imageName = (string)typeAndTarget.Value;
            }
            else if (isPicBullet)
            {
                Dictionary<string, DictionaryEntry> rels = GetFileRelations("numbering.xml.rels");
                DictionaryEntry entry = rels[imageId];
                imageName = entry.Value.ToString();
            }
            else if (m_currentFile.StartsWith("comments"))
            {
                Dictionary<string, DictionaryEntry> rels = GetFileRelations("comments.xml.rels");
                DictionaryEntry entry = rels[imageId];
                imageName = entry.Value.ToString();
            }
            else if (!string.IsNullOrEmpty(m_currentFile))
            {
                Dictionary<string, DictionaryEntry> rels = GetFileRelations(m_currentFile);
                DictionaryEntry entry = rels[imageId];
                imageName = entry.Value.ToString();
            }
            else
            {
                DictionaryEntry entry = m_docRelations[imageId];
                imageName = entry.Value.ToString();
            }
            return imageName;
        }
        /// <summary>
        /// Loads the image data.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="imageRelId">The image relation id.</param>
        /// <param name="isHeaderFooter">if set to <c>true</c> [is header footer].</param>
        /// <param name="isPicBullet">if set to <c>true</c> [is picture bullet].</param>
        private void LoadImage(WPicture picture, string id, bool isHeaderFooter, bool isPicBullet)
        {
            string imageName = GetImageName(id, isHeaderFooter, isPicBullet);
            if (ImageIds.ContainsKey(imageName))
            {
                picture.LoadImage(m_doc.Images[ImageIds[imageName]]);
                picture.ImageRecord.OccurenceCount++;
            }
            else
            {
                byte[] imageBytes = GetImageBytes(imageName);
                if (imageBytes != null
                    && imageBytes.Length > 0)
                {
                    picture.LoadImage(imageBytes);
                    ImageIds.Add(imageName, picture.ImageRecord.ImageId);
                }
            }
        }
        /// <summary>
        /// Gets the image bytes.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        private byte[] GetImageBytes(string imageName)
        {
            int index = imageName.LastIndexOf('/');
            if (index > 0 && (imageName.Substring(index + 1, imageName.Length - (index + 1))).Length > 0) 
                imageName = imageName.Replace(imageName.Substring(0, index + 1), null);//get only the name of the image from path
            Part part = FindPart("word/media/", imageName);
            byte[] imageBytes = null;
            if (part == null)
            {
                part = FindPart("media/", imageName);
                if (part != null)
                {
                    imageBytes = GetBytesFrom(part);
                    part = null;
                    ClearParsedImage(imageName, "media/");
                }
            }
            else
            {
                imageBytes = GetBytesFrom(part);
                part = null;
                ClearParsedImage(imageName, "word/media/");
            }
            return imageBytes;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageId">The image id.</param>
        /// <param name="isHeaderFooter">if it is a header/footer, set to <c>true</c>.</param>
        /// <returns></returns>
        private Image GetLinkedImageBytes(string imageId, bool isHeaderFooter, bool isPicBullet)
        {
            string fileName = null;
            if (isHeaderFooter)
            {
                Dictionary<string, DictionaryEntry> entryHF = GetFileRelations(m_currentFile);
                DictionaryEntry typeAndTarget = entryHF[imageId];
                fileName = (string)typeAndTarget.Value;
            }
            else if (isPicBullet)
            {
                Dictionary<string, DictionaryEntry> rels = GetFileRelations("numbering.xml.rels");
                DictionaryEntry entry = rels[imageId];
                fileName = entry.Value.ToString();
            }
            else
            {
                DictionaryEntry entry = m_docRelations[imageId];
                fileName = entry.Value.ToString();
            }
            Image image = DownloadImage(fileName);
            if (image != null)
                return image;
            else
                return null;
        }
#endif

        /// <summary>
        /// Gets the bytes from.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        private byte[] GetBytesFrom(Part part)
        {
            if (part != null)
            {
                int dataLen = (int)part.DataStream.Length;
                byte[] bytes = new byte[dataLen];
                part.DataStream.Position = 0;
                part.DataStream.Read(bytes, 0, dataLen);
                return bytes;
            }
            return null;
        }
        /// <summary>
        /// Finds the part.
        /// </summary>
        /// <param name="partContainer">The part container.</param>
        /// <param name="partName">Name of the part.</param>
        /// <returns></returns>
        private Part FindPart(string partContainer, string partName)
        {
            partName = partName.Replace(partContainer, string.Empty);
            PartContainer container = m_doc.DocxPackage.FindPartContainer(partContainer);
            if (container.XmlParts.ContainsKey(partName))
            {
                return container.XmlParts[partName];
            }
            return null;
        }
        /// <summary>
        /// Gets the HF relation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private Dictionary<string, DictionaryEntry> GetFileRelations(string name)
        {
            if (!HFRelations.ContainsKey(name))
            {
                PartContainer wordContainer = m_doc.DocxPackage.FindPartContainer("word/");

                if (!wordContainer.Relations.ContainsKey("word/_rels/" + name)
                    || wordContainer.Relations["word/_rels/" + name].DataStream == null)
                    return null;

                Relations hfRels = wordContainer.Relations["word/_rels/" + name];
                hfRels.DataStream.Position = 0;

                XmlReader hfRelReader = UtilityMethods.CreateReader(hfRels.DataStream);
                Dictionary<string, DictionaryEntry> hfRelation = new Dictionary<string, DictionaryEntry>();
                ParseRelations(hfRelReader, hfRelation);
                HFRelations.Add(name, hfRelation);
                return HFRelations[name];
            }
            else
            {
                return HFRelations[name];
            }
        }
        /// <summary>
        /// Parses the float val.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private float ParseFloatVal(string value)
        {
            if (value.StartsWith("."))
            {
                value = "0" + value;
            }

            //value = value.Replace(".", ",");
            if (value.EndsWith("in"))
            {
                float inchVal = float.Parse(value.Replace("in", string.Empty), CultureInfo.InvariantCulture);
                return (float)UnitsConvertor.Instance.ConvertUnits(inchVal, PrintUnits.Inch, PrintUnits.Point);
            }
            else
            {
                try
                {
                    return Convert.ToSingle(value, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                }
                return float.Parse(value, CultureInfo.InvariantCulture);
            }
        }
        /// <summary>
        /// Returns the boolean value from the current node
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private bool GetBooleanValue(XmlReader reader)
        {
            bool value = true;

            if (reader.AttributeCount > 0)
            {
                string val = reader.GetAttribute("val", DocxConstants.W_namespace);
                if (val == "0" || val == "false" || val == "off")
                {
                    value = false;
                }
            }

            return value;
        }
        /// <summary>
        /// Returns the boolean value from the current node
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private CompatibilityMode GetCompatibilityMode(XmlReader reader)
        {
            CompatibilityMode value = CompatibilityMode.Word2013;

            if (reader.AttributeCount > 0)
            {
                string val = reader.GetAttribute("val", DocxConstants.W_namespace);
                if (val == "11")
                {
                    value = CompatibilityMode.Word2003;
                }
                else if (val == "12")
                {
                    value = CompatibilityMode.Word2007;
                }
                else if (val == "14")
                {
                    value = CompatibilityMode.Word2010;
                }
                else if (val == "15")
                {
                    value = CompatibilityMode.Word2013;
                }
            }

            return value;
        }
        /// <summary>
        /// Gets the boolean value from the current node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <returns></returns>
        private bool GetBooleanValue(XmlReader reader, string nameSpace)
        {
            bool value = true;

            if (reader.AttributeCount > 0)
            {
                string val = reader.GetAttribute("val", nameSpace);
                if (val == "0" || val == "false" || val == "off")
                {
                    value = false;
                }
            }

            return value;
        }
        /// <summary>
        /// Returns xml element value.
        /// </summary>
        /// <param name="reader">XmlReader to get value from.</param>
        /// <returns>Xml element value.</returns>
        private string GetReaderElementValue(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return string.Empty;
            }

            string strResult;

            reader.Read();

            if (reader.NodeType != XmlNodeType.EndElement)
            {
                strResult = reader.Value;
                reader.Skip();
            }
            else
            {
                strResult = string.Empty;
            }

            reader.Skip();

            return strResult;
        }
        /// <summary>
        /// Parses the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color GetColorValue(string color)
        {
            if (color == null || color == "auto")
                return Color.Empty;

            if (color.StartsWith("fill darken") || color.StartsWith("fill lighten"))
            {
                return GetGradientColor(color);
            }
            else
            {
                Color colorVal = GetHexColor(color);
                if (colorVal == Color.Empty)
                {
                    colorVal = GetHtmlColor(color);
                }
                return colorVal;
            }
        }
        /// <summary>
        /// Gets the color of the gradient.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color GetGradientColor(string color)
        {
            int startIndex = color.IndexOf("(") + 1;
            int endIndex = color.IndexOf(")");

            if (startIndex != -1 && endIndex != -1)
            {
                int colVal = int.Parse(color.Substring(startIndex, endIndex - startIndex));
                int r = 255, g = 255, b = 255;
                if (color.StartsWith("fill darken"))
                {
                    //green = blue = n, where red = 0 when fill darken(n)
                    g = b = colVal;
                    r = 0;
                }
                else
                {
                    //red = 255 - n, where green = blue = 255 when fill lighten(n)
                    g = b = 255;
                    r = 255 - colVal;
                }
                return Color.FromArgb(r, g, b);
            }

            return Color.White;
        }
        /// <summary>
        /// Gets the hexadecimal color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color GetHexColor(string color)
        {
            // Parse hexadecimal color
            color = color.Replace("#", string.Empty);
            string htmlColor = color;

            if (color.Length > 6)
            {
                color = color.Substring(0, 6);
            }
            else if (color.Length < 6 && color.Length != 3)
            {
                int symbToAdd = 6 - color.Length;
                for (int i = 0; i < symbToAdd; i++)
                    color = color.Insert(0, "0");
            }
            else if (color.Length == 3)
            {
                color = color.Insert(0, color[0].ToString());
                color = color.Insert(2, color[2].ToString());
                color = color.Insert(4, color[4].ToString());
            }

            try
            {
                string strR = color.Substring(0, 2);
                string strG = color.Substring(2, 2);
                string strB = color.Substring(4, 2);

                int r = Int32.Parse(strR, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int g = Int32.Parse(strG, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int b = Int32.Parse(strB, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return Color.FromArgb(r, g, b);
            }
            catch
            { }

            return Color.Empty;
        }
        /// <summary>
        /// Gets the HTML color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color GetHtmlColor(string color)
        {
            // Parse html color.
            int bracketIndex = color.IndexOf("[");
            if (bracketIndex != -1)
            {
                color = color.Remove(bracketIndex, color.Length - bracketIndex);
                color = color.Trim();
            }

            try
            {
#if !SILVERLIGHT && !WP
                return ColorTranslator.FromHtml(color);
#else
                return GetHighLightColor(color);
#endif
            }
            catch
            { };

            return Color.Empty;
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// Gets the HighLight color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color GetHighLightColor(string color)
        {
            Color highLightColor = Color.Empty;
            switch (color)
            {
                case "empty":
                    highLightColor = WordColor.ColorsArray[0];
                    break;
                case "black":
                    highLightColor = WordColor.ColorsArray[1];
                    break;
                case "blue":
                    highLightColor = WordColor.ColorsArray[2];
                    break;
                case "cyan":
                    highLightColor = WordColor.ColorsArray[3];
                    break;
                case "green":
                    highLightColor = WordColor.ColorsArray[4];
                    break;
                case "magenta":
                    highLightColor = WordColor.ColorsArray[5];
                    break;
                case "red":
                    highLightColor = WordColor.ColorsArray[6];
                    break;
                case "yellow":
                    highLightColor = WordColor.ColorsArray[7];
                    break;
                case "white":
                    highLightColor = WordColor.ColorsArray[8];
                    break;
                case "darkBlue":
                    highLightColor = WordColor.ColorsArray[9];
                    break;
                case "darkCyan":
                    highLightColor = WordColor.ColorsArray[10];
                    break;
                case "darkGreen":
                    highLightColor = WordColor.ColorsArray[11];
                    break;
                case "darkMagenta":
                    highLightColor = WordColor.ColorsArray[12];
                    break;
                case "darkRed":
                    highLightColor = WordColor.ColorsArray[13];
                    break;
                case "darkYellow":
                    highLightColor = WordColor.ColorsArray[14];
                    break;
                case "darkGray":
                    highLightColor = WordColor.ColorsArray[15];
                    break;
                case "lightGray":
                    highLightColor = WordColor.ColorsArray[16];
                    break;
            }
            return highLightColor;
        }
#endif
        /// <summary>
        /// Gets the float value from element attribute.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attrName">Name of the attr.</param>
        /// <param name="attrNS">The attr NS.</param>
        /// <returns></returns>
        private float GetFloatValue(XmlReader reader, string attrName, string attrNS)
        {
            if (attrName == null || attrName.Length == 0)
                throw new ArgumentException("Attribute name must not be null or empty");

            string value;

            if (attrNS == null)
                value = reader.GetAttribute(attrName);
            else
                value = reader.GetAttribute(attrName, attrNS);

            if (value != null)
            {
                return float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture) / DLSConstants.TwipsInOnePoint;
            }
            return float.MaxValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private MemoryStream ReadSingleNodeIntoStream(XmlReader reader)
        {
            MemoryStream result = new MemoryStream();
            XmlWriter writer = UtilityMethods.CreateWriter(result, Encoding.UTF8);
            writer.WriteNode(reader, false);
            writer.Flush();
            return result;
        }
        /// <summary>
        /// Skip whitespaces and moves the reader to the next node.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        private void SkipWhitespaces(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
                return;

            while (reader.NodeType == XmlNodeType.Whitespace)
                reader.Read();
        }
        #endregion Helper Methods
    }

    #region Helper Classes

    #region UtilityMethods
    /// <summary>
    /// This class has few utility methods
    /// </summary>
    internal class UtilityMethods
    {
        /// <summary>
        /// Converts DateTime into number.
        /// </summary>
        /// <param name="dateTime">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static double ConvertDateTimeToNumber(DateTime dateTime)
        {
            double dNumber = dateTime.ToOADate();

            if (dNumber < 61)
            {
                // We are decreasing one day because OADate starts from 31 December 1899,
                // but MS Excel date from 1 January 1900.
                dNumber--;
            }

            return dNumber;
        }
        /// <summary>
        /// Converts number into DateTime.
        /// </summary>
        /// <param name="dNumber">Number to convert.</param>
        /// <returns>Converted value.</returns>
        public static DateTime ConvertNumberToDateTime(double dNumber)
        {
            if (dNumber < 61)
            {
                // We are adding one day, because FromOADate starts from 31 December 1899
                // and Excel date starts from 1 January 1900
                dNumber++;
            }

#if WINRT
            return DateTimeFromOADate(dNumber);
#else
            return DateTime.FromOADate(dNumber);
#endif
        }
#if WINRT
        private static DateTime DateTimeFromOADate(double dNumber)
        {
            throw new NotImplementedException();
        }
#endif
        /// <summary>
        /// Copies one stream into another.
        /// </summary>
        /// <param name="source">Source stream to copy from.</param>
        /// <param name="destination">Destination stream to copy into.</param>
        public static void CopyStreamTo(Stream source, Stream destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (destination == null)
                throw new ArgumentNullException("destination");

            const int BufferSize = 32768;
            int iReadCount;
            byte[] arrBuffer = new byte[BufferSize];

            while ((iReadCount = source.Read(arrBuffer, 0, BufferSize)) > 0)
            {
                destination.Write(arrBuffer, 0, iReadCount);
            }
        }
        /// <summary>
        /// Creates copy of the MemoryStream.
        /// </summary>
        /// <param name="source">Source stream to copy.</param>
        /// <returns>A copy of the original MemoryStream.</returns>
        public static Stream CloneStream(Stream source)
        {
            Stream result = new MemoryStream((int)source.Length);
            long lStartPosition = source.Position;
            source.Position = 0;
            CopyStreamTo(source, result);
            result.Position = source.Position = lStartPosition;

            return result;
        }
        /// <summary>
        /// Creates xml reader to read data from the stream.
        /// </summary>
        /// <param name="data">Data to read.</param>
        /// <returns>Created xml reader.</returns>
        public static XmlReader CreateReader(Stream data, bool skipToElement)
        {
            XmlReader result;
            data.Position = 0;
            result = XmlReader.Create(data);

            if (skipToElement)
            {
                while (result.NodeType != XmlNodeType.Element)
                    result.Read();
            }

            return result;
        }
        /// <summary>
        /// Creates xml reader to read data from the stream.
        /// </summary>
        /// <param name="data">Data to read.</param>
        /// <returns>Created xml reader.</returns>
        public static XmlReader CreateReader(Stream data)
        {
            return CreateReader(data, true);
        }
        /// <summary>
        /// Creates xml writer to read data from the stream.
        /// </summary>
        /// <param name="data">Data to read.</param>
        /// <returns>Created xml writer.</returns>
        public static XmlWriter CreateWriter(Stream data, Encoding encoding)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = encoding;
            return XmlWriter.Create(data, settings);
        }
        /// <summary>
        /// Creates xml writer to read data from the stream.
        /// </summary>
        /// <param name="data">Data to read.</param>
        /// <returns>Created xml writer.</returns>
        public static XmlWriter CreateWriter(TextWriter data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            return XmlWriter.Create(data, settings);
        }
        /// <summary>
        /// Creates xml writer to read data from the stream.
        /// </summary>
        /// <param name="data">Data to read.</param>
        /// <returns>Created xml writer.</returns>
        public static XmlWriter CreateWriter(TextWriter data, bool indent)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = indent;
            return XmlWriter.Create(data, settings);
        }
    }
    #endregion UtilityMethods

    #endregion Helper Classes

    #region Enums
    /// <summary>
    /// Represents the ShapeType
    /// </summary>
    internal enum ShapeType
    {
        TextboxShape = 0,
        GroupedShape = 1,
        PictureShape = 2,
        WatermarkShape = 3,
        OleObject = 4,
        XmlParagraphItem = 5
    }
    /// <summary>
    /// Represents the Track change type
    /// </summary>
    internal enum TrackChangeType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        IsDelete,
        /// <summary>
        /// 
        /// </summary>
        IsInsert
    }
    #endregion Enums
}