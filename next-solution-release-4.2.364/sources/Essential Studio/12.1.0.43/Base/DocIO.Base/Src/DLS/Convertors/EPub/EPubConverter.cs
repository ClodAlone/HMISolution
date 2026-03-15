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
using System.IO;
using System.Xml;

using Syncfusion.Compression.Zip;
using System.Drawing;

#if !SILVERLIGHT

namespace Syncfusion.DocIO.DLS
{
    internal class EPubConverter
    {
        # region Fields
        /// <summary>
        /// Zip archive used to compress the EPub file
        /// </summary>
        private ZipArchive m_archieve;
        /// <summary>
        /// Word document to be converted
        /// </summary>
        private WordDocument m_document;
        /// <summary>
        /// Holds the files to add their names in package file and stream in archive
        /// </summary>
        private Dictionary<string, Stream> m_embeddedFiles;
        /// <summary>
        /// Holds the navigation point and reference for EPub TOC
        /// </summary>
        private Dictionary<string, string> m_bookmarks;
        /// <summary>
        /// Name of the XHTML file
        /// </summary>
        private string m_fileName;
        /// <summary>
        /// Holds the unique identifier (GUID)
        /// </summary>
        private string m_uid;
        /// <summary>
        /// Refers to the title of the document
        /// </summary>
        private string m_title;
        /// <summary>
        /// Refers to the author of the document
        /// </summary>
        private string m_author;
        /// <summary>
        /// Holds the last navigation level
        /// </summary>
        private int m_previous;
        /// <summary>
        /// Refers to the playOrder in the NCX file.
        /// </summary>
        private int m_playOrder = 1;
        /// <summary>
        /// Refers to the cover image
        /// </summary>
        private WPicture m_coverImage;
        # endregion
        /// <summary>
        /// Gets or sets the cover image
        /// </summary>
        internal WPicture CoverImage
        {
            get
            {
                return m_coverImage;
            }
            set
            {
                m_coverImage = value;
            }
        }
        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        internal string FileName
        {
            get
            {
                return m_fileName;
            }
            set
            {
                m_fileName = value;
            }
        }
        # region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public EPubConverter()
        {
            m_uid = Guid.NewGuid().ToString();
            m_previous = 0;
            m_archieve = new ZipArchive();
            m_embeddedFiles = new Dictionary<string, Stream>();
        }
        #endregion

        # region Implementation
        /// <summary>
        /// Converts Word to EPub and saves it in disk
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="document">Input word document</param>
        public void ConvertToEPub(string fileName, WordDocument document)
        {
            m_document = document;
            ConvertToEPub();
            Save(fileName);
        }

        /// <summary>
        /// Converts Word to EPub and saves it as stream
        /// </summary>
        /// <param name="stream">File stream to save</param>
        /// <param name="document">Input word document</param>
        public void ConvertToEPub(Stream stream, WordDocument document)
        {
            m_document = document;
            ConvertToEPub();
            Save(stream);
        }

        /// <summary>
        /// Converts word document.
        /// </summary>
        /// <param name="document">Input word document</param>
        private void ConvertToEPub()
        {
            WriteMIME();

            GenerateOPS();

            GenerateOPF();

            GenerateNCX();

            GenerateContainer();
        }

        /// <summary>
        /// Saves the EPub in disk
        /// </summary>
        /// <param name="fileName">File name to save</param>
        private void Save(string fileName)
        {
            m_archieve.Save(fileName);
            m_archieve.Close();
            m_archieve.Dispose();
        }

        /// <summary>
        /// Saves the EPub as stream
        /// </summary>
        /// <param name="stream">File stream to save</param>
        private void Save(Stream stream)
        {
            m_archieve.Save(stream, false);
            m_archieve.Close();
            m_archieve.Dispose();
        }

        /// <summary>
        /// Writes Container.xml
        /// </summary>
        private void GenerateContainer()
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);

            writer.WriteStartDocument();
            writer.WriteStartElement(EPubConstants.Container, EPubConstants.ContainerNamespace);
            writer.WriteAttributeString(EPubConstants.Version, "1.0");
            writer.WriteStartElement(EPubConstants.Rootfiles);
            writer.WriteStartElement(EPubConstants.Rootfile);
            writer.WriteAttributeString(EPubConstants.FullPath, EPubConstants.PackageFileName);
            writer.WriteAttributeString(EPubConstants.MediaType, EPubConstants.OEBPSContentType);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.Flush();
            m_archieve.AddItem(EPubConstants.ContainerFileName, stream, true, FileAttributes.Archive);
        }

        /// <summary>
        /// Writes EPub document organization as navigation file
        /// </summary>
        private void GenerateNCX()
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);

            writer.WriteStartDocument();
            writer.WriteDocType(EPubConstants.NavigationPrefix, EPubConstants.NavigationPubid, EPubConstants.NavigationSysid, null);
            writer.WriteStartElement(EPubConstants.NavigationPrefix, EPubConstants.NavigationNamespace);
            writer.WriteAttributeString(EPubConstants.Version, "2005-1");
            writer.WriteStartElement(EPubConstants.HeadElement);
            writer.WriteStartElement(EPubConstants.MetaElement);
            writer.WriteAttributeString(EPubConstants.NameAttribute, string.Format("{0}:{1}", EPubConstants.DTBookPrefix, "uid"));
            writer.WriteAttributeString(EPubConstants.ContentAttribute, m_uid);
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement(EPubConstants.DocTitleElement);
            writer.WriteStartElement(EPubConstants.Text);
            writer.WriteValue(m_title);
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement(EPubConstants.DocAuthorElement);
            writer.WriteStartElement(EPubConstants.Text);
            writer.WriteValue(m_author);
            writer.WriteEndElement();
            writer.WriteEndElement();

            // File navigation
            writer.WriteStartElement(EPubConstants.NavigationMapElement);
            writer.WriteStartElement(EPubConstants.NavigationPointElement);
            writer.WriteAttributeString(EPubConstants.IdAttribute, Path.GetFileNameWithoutExtension(m_fileName));
            writer.WriteAttributeString(EPubConstants.PlayOrder, (m_playOrder++).ToString());
            writer.WriteStartElement(EPubConstants.NavigationLabelElement);
            writer.WriteStartElement(EPubConstants.Text);
            writer.WriteValue(m_title);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement(EPubConstants.ContentAttribute);
            writer.WriteAttributeString(EPubConstants.SourceAttribute, m_fileName);
            writer.WriteEndElement();
            writer.WriteEndElement();

            if (m_bookmarks != null && m_bookmarks.Count > 0)
            {
                int current = 0;
                foreach (string key in m_bookmarks.Keys)
                {
                    string[] level = key.Split(new char[] { ';' });
                    current = int.Parse(level[0]);
                    if (current >= m_previous)
                    {
                        if (current == m_previous)
                            writer.WriteEndElement();
                        while (current > ++m_previous)
                            WriteBookmark(string.Concat(m_previous, ";", level[1]), "", writer);
                        WriteBookmark(key, m_bookmarks[key], writer);
                    }
                    else
                    {
                        while (current <= m_previous--)
                            writer.WriteEndElement();
                        WriteBookmark(key, m_bookmarks[key], writer);
                    }
                    m_previous = current;
                }
                while (current-- != 0)
                    writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.Flush();
            m_archieve.AddItem(EPubConstants.NavigationFileName, stream, true, FileAttributes.Archive);
            GenerateTOCNavigationalHTML();
        }
        /// <summary>
        /// Generates TOC (Table of Contents) HTML file for navigation in Kindle
        /// </summary>
        private void GenerateTOCNavigationalHTML()
        {
            MemoryStream tocstream = new MemoryStream();
            XmlTextWriter m_writer = new XmlTextWriter(tocstream, Encoding.UTF8);
            m_writer.WriteStartDocument();
            m_writer.WriteDocType("html", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd", null);
            m_writer.WriteStartElement("html", "http://www.w3.org/1999/xhtml");
            m_writer.WriteStartElement("head");
            m_writer.WriteRaw("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\" />");
            m_writer.WriteRaw(string.Format("<title>{0}</title>", "TOC"));
            m_writer.WriteRaw("<style type=\"text/css\"></style>");
            m_writer.WriteEndElement();
            m_writer.WriteStartElement("body");
            m_writer.WriteRaw("<p class=\"title\">Table of contents</p>");
            if (m_bookmarks != null && m_bookmarks.Count > 0)
            {
                int current = 0;
                foreach (string key in m_bookmarks.Keys)
                {
                    string[] level = key.Split(new char[] { ';' });
                    current = int.Parse(level[0]);
                    m_writer.WriteStartElement("p");
                    m_writer.WriteStartElement("a");
                    m_writer.WriteAttributeString("href", m_fileName + "#" + level[1]);
                    m_writer.WriteValue(m_bookmarks[key]);
                    m_writer.WriteEndElement();
                    m_writer.WriteEndElement();
                }
            }
            m_writer.WriteEndElement();
            m_writer.WriteEndElement();
            m_writer.Flush();
            m_archieve.AddItem("toc.html", tocstream, true, FileAttributes.Archive);
        }
        /// <summary>
        /// Writes bookmark to .NCX (navigation file) to enable TOC in EPub
        /// </summary>
        /// <param name="key">Heading level and id</param>
        /// <param name="value">Navigation text</param>
        /// <param name="writer">XmlWriter</param>
        private void WriteBookmark(string key, string value, XmlWriter writer)
        {
            string[] level = key.Split(new char[] { ';' });
            string navPointId  = string.Empty;
            if (string.IsNullOrEmpty(value))
                navPointId = string.Format("level{0}_{1}", level[0], "unknownLevel");
            else
                navPointId = string.Format("level{0}_{1}", level[0], level[1]);
            writer.WriteStartElement(EPubConstants.NavigationPointElement);
            writer.WriteAttributeString(EPubConstants.IdAttribute, navPointId);
            writer.WriteAttributeString(EPubConstants.PlayOrder, (m_playOrder++).ToString());
            writer.WriteStartElement(EPubConstants.NavigationLabelElement);
            writer.WriteStartElement(EPubConstants.Text);
            if (string.IsNullOrEmpty(value))
            {
                value = "***";
                m_playOrder--;
            }
            writer.WriteValue(value);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement(EPubConstants.ContentAttribute);
            writer.WriteAttributeString(EPubConstants.SourceAttribute, m_fileName + "#" + level[1]);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes EPub document information as package file
        /// </summary>
        private void GenerateOPF()
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);

            writer.WriteStartDocument();
            writer.WriteStartElement(EPubConstants.PackageElement, EPubConstants.IDPFNamespace);
            writer.WriteAttributeString(EPubConstants.Version, "2.0");
            writer.WriteAttributeString(EPubConstants.UID, EPubConstants.GUID);
            writer.WriteAttributeString(EPubConstants.XmlPrefix, EPubConstants.XSIPrefix, null, EPubConstants.XSIPartType);

            writer.WriteStartElement(EPubConstants.MetadataElement);
            writer.WriteAttributeString(EPubConstants.XmlPrefix, EPubConstants.DublinCorePrefix, null, EPubConstants.DublinCorePartType);
            writer.WriteAttributeString(EPubConstants.XmlPrefix, EPubConstants.PackagePrefix, null, EPubConstants.IDPFNamespace);

            writer.WriteStartElement(string.Format("{0}:{1}", EPubConstants.DublinCorePrefix, EPubConstants.TitleElement));
            writer.WriteValue(m_title);
            writer.WriteEndElement();

            writer.WriteStartElement(string.Format("{0}:{1}", EPubConstants.DublinCorePrefix, EPubConstants.CreatorElement));
            m_author = string.IsNullOrEmpty(m_document.BuiltinDocumentProperties.Author) ? "Administrator" : m_document.BuiltinDocumentProperties.Author;
            writer.WriteValue(m_author);
            writer.WriteEndElement();

            writer.WriteStartElement(string.Format("{0}:{1}", EPubConstants.DublinCorePrefix, EPubConstants.IdentifierElement));
            writer.WriteAttributeString(EPubConstants.IdAttribute, EPubConstants.GUID);
            writer.WriteValue(m_uid);
            writer.WriteEndElement();

            writer.WriteStartElement(string.Format("{0}:{1}", EPubConstants.DublinCorePrefix, EPubConstants.LanguageElement));
            writer.WriteAttributeString(EPubConstants.XSIPrefix, EPubConstants.Type, null, string.Format("{0}:{1}", EPubConstants.DublinCoreTermsPrefix, EPubConstants.LanguageTag));
            writer.WriteValue("en-US");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement(EPubConstants.ManifestElement);

            // default item for NCX
            writer.WriteStartElement(EPubConstants.ItemElement);
            writer.WriteAttributeString(EPubConstants.IdAttribute, EPubConstants.NavigationPrefix);
            writer.WriteAttributeString(EPubConstants.HyperlinkAttribute, EPubConstants.NavigationFileName);
            writer.WriteAttributeString(EPubConstants.MediaType, EPubConstants.PackageContentType);
            writer.WriteEndElement();
            // Writes Cover page file
            if (CoverImage != null)
            {
                writer.WriteStartElement(EPubConstants.ItemElement);
                writer.WriteAttributeString(EPubConstants.IdAttribute, "cover.html");
                writer.WriteAttributeString(EPubConstants.HyperlinkAttribute, "cover.html");
                writer.WriteAttributeString(EPubConstants.MediaType, EPubConstants.XHTMLContentType);
                writer.WriteEndElement();
            }
            // Writes main XHTML file
            writer.WriteStartElement(EPubConstants.ItemElement);
            writer.WriteAttributeString(EPubConstants.IdAttribute, (Path.GetFileNameWithoutExtension(m_fileName).Replace(' ','_')));
            writer.WriteAttributeString(EPubConstants.HyperlinkAttribute, m_fileName);
            writer.WriteAttributeString(EPubConstants.MediaType, EPubConstants.XHTMLContentType);
            writer.WriteEndElement();
            // Writes TOC HTML file
            writer.WriteStartElement(EPubConstants.ItemElement);
            writer.WriteAttributeString(EPubConstants.IdAttribute, "toc.html");
            writer.WriteAttributeString(EPubConstants.HyperlinkAttribute, "toc.html");
            writer.WriteAttributeString(EPubConstants.MediaType, EPubConstants.XHTMLContentType);
            writer.WriteEndElement();
            
            WriteEmbeddedFiles(writer);

            writer.WriteEndElement();

            writer.WriteStartElement(EPubConstants.SpineElement);
            writer.WriteAttributeString(Path.GetFileNameWithoutExtension(EPubConstants.NavigationFileName), EPubConstants.NavigationPrefix);

            writer.WriteStartElement(EPubConstants.ItemReferenceElement);
            writer.WriteAttributeString(EPubConstants.IdReferenceAttribute, (Path.GetFileNameWithoutExtension(m_fileName).Replace(' ', '_')));
            writer.WriteEndElement();

            foreach (string id in m_embeddedFiles.Keys)
            {
                if (Path.GetExtension(id) == ".html")
                {
                    writer.WriteStartElement(EPubConstants.ItemReferenceElement);
                    writer.WriteAttributeString(EPubConstants.IdReferenceAttribute, Path.GetFileNameWithoutExtension(id));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.Flush();
            m_archieve.AddItem(EPubConstants.PackageFileName, stream, true, FileAttributes.Archive);
        }

        /// <summary>
        /// Writes the details of all embedded files.
        /// </summary>
        /// <param name="writer">XmlTextWriter to write</param>
        /// <param name="fileName">Name of the embedded file</param>
        private void WriteEmbeddedFiles(XmlTextWriter writer)
        {
            // Writes embedded file
            if (m_embeddedFiles != null && m_embeddedFiles.Count > 0)
            {
                foreach (string id in m_embeddedFiles.Keys)
                {
                    writer.WriteStartElement(EPubConstants.ItemElement);
                    writer.WriteAttributeString(EPubConstants.IdAttribute, Path.GetFileNameWithoutExtension(id));
                    writer.WriteAttributeString(EPubConstants.HyperlinkAttribute, id.Replace('\\', '/'));
                    writer.WriteAttributeString(EPubConstants.MediaType, GetFormat(Path.GetExtension(id)));
                    writer.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// Generates XHTML and adds it to archive
        /// </summary>
        private void GenerateOPS()
        {
            HTMLExport export = new HTMLExport();

            export.CacheFilesInternally = true;
            export.HasNavigationId = true;
            export.HasOEBHeaderFooter = true;

            m_document.SaveOptions.HtmlExportCssStyleSheetType = CssStyleSheetType.External;
            m_document.SaveOptions.HtmlExportCssStyleSheetFileName = "styles.css";
            m_document.SaveOptions.HtmlExportTextInputFormFieldAsText = true;

            if (m_document.SaveOptions.EPubExportFont)
            {
                try
                {
                    EmbedFontFiles();
                }
                catch (System.Security.SecurityException)
                {
                    throw new NotSupportedException("Embedding font files is not supported in medium trust");
                }
            }

            UpdateTitle();
            m_fileName += ".html";

            // Converts the document as XHTML stream and adds to archive
            MemoryStream opsData = new MemoryStream();
            export.SaveAsXhtml(m_document, opsData, true);
            opsData.Flush();
            GenerateCoverPage();
            m_archieve.AddItem(m_fileName, opsData, true, FileAttributes.Archive);

            // Gets embedded stylesheets
            if (export.EmbeddedStyleSheet != null)
                m_embeddedFiles.Add(m_document.SaveOptions.HtmlExportCssStyleSheetFileName, export.EmbeddedStyleSheet);

            // Gets embedded images
            if (export.EmbeddedImages != null && export.EmbeddedImages.Count > 0)
            {
                foreach (string key in export.EmbeddedImages.Keys)
                    m_embeddedFiles.Add(key, export.EmbeddedImages[key]);
            }
            if (CoverImage != null)
                m_embeddedFiles.Add("images/cover.png", new MemoryStream(CoverImage.ImageBytes));
            // Adds all embedded files like images, stylesheet, fonts to archive
            foreach (string key in m_embeddedFiles.Keys)
                m_archieve.AddItem(key, m_embeddedFiles[key], true, FileAttributes.Archive);

            if (export.Bookmarks != null && export.Bookmarks.Count > 0)
                m_bookmarks = export.Bookmarks;
        }
        /// <summary>
        /// Generates Cover page html if any cover page image is provided. 
        /// </summary>
        private void GenerateCoverPage()
        {
            //Add Cover page HTML
            XmlTextWriter m_writer;
            if (CoverImage != null)
            {
                MemoryStream coverPageStream = new MemoryStream();
                m_writer = new XmlTextWriter(coverPageStream, Encoding.UTF8);
                m_writer.WriteStartDocument();
                m_writer.WriteDocType("html", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd", null);
                m_writer.WriteStartElement("html", "http://www.w3.org/1999/xhtml");
                m_writer.WriteStartElement("head");
                m_writer.WriteRaw("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\" />");
                m_writer.WriteRaw(string.Format("<title>{0}</title>", "Cover"));
                m_writer.WriteRaw("<style type=\"text/css\"></style>");
                m_writer.WriteEndElement();
                m_writer.WriteStartElement("body");
                m_writer.WriteStartElement("p");
                m_writer.WriteRaw("<img height=\"" + CoverImage.Height * 1.33 + "\" width=\""
                    + CoverImage.Width * 1.33 + "\" src=\"../Images/cover.png\" />");
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
                m_writer.WriteEndElement();
                m_writer.Flush();
                m_archieve.AddItem("cover.html", coverPageStream, true, FileAttributes.Archive);
            }
        }

        /// <summary>
        /// Saves the used fonts in the document
        /// </summary>
        private void EmbedFontFiles()
        {
            List<Font> fontnames = m_document.UsedFontNames;
            // Sorting is necessary to follow fall back of font files in CSS
            SortedDictionary<string, string> listnames = new SortedDictionary<string, string>();

            foreach (Font key in fontnames)
            {
                if (key.Name == "Times New Roman")
                    continue;
                bool bold = key.Bold;
                bool italic = key.Italic;

                string nameAdd = (bold ? "b" : "") + (italic ? "a" : "");
                string name = key.Name.ToLower().Replace(" ", string.Empty) + nameAdd + ".ttf";

                if (!m_embeddedFiles.ContainsKey(name))
                {
                    IntPtr hDC = GdiApi.CreateDC("DISPLAY", null, null, IntPtr.Zero);
                    IntPtr hFont = key.ToHfont();
                    IntPtr oldObj = GdiApi.SelectObject(hDC, hFont);
                    uint numBytes = GdiApi.GetFontData(hDC, 0, 0, null, 0);

                    if (numBytes == 0xFFFFFFFF)
                    {
                        System.Diagnostics.Debug.WriteLine("Can't create font");
                    }

                    byte[] buff = new byte[numBytes];
                    numBytes = GdiApi.GetFontData(hDC, 0, 0, buff, numBytes);

                    if (numBytes == 0xFFFFFFFF)
                    {
                        System.Diagnostics.Debug.WriteLine("Can't create font");
                    }
                    GdiApi.SelectObject(hDC, oldObj);
                    GdiApi.DeleteObject(hFont);
                    GdiApi.DeleteDC(hDC);

                    MemoryStream stream = new MemoryStream(buff, 0, buff.Length, false);
                    m_embeddedFiles.Add(name, stream);

                    string style = (italic ? "font-style:italic; " : "") + (bold ? "font-weight:bold; " : "");
                    listnames.Add(name, "@font-face { font-family:'" + key.Name + "'; " + style + "src:url('" + name + "') }");
                }
            }

            if (listnames.Count > 0)
            {
                m_document.SaveOptions.FontFiles = new string[listnames.Count];
                listnames.Values.CopyTo(m_document.SaveOptions.FontFiles, 0);
            }
        }

        /// <summary>
        /// Gets the title of the document
        /// </summary>
        private void UpdateTitle()
        {
            string temp = string.Empty;
            m_title = string.IsNullOrEmpty(m_document.BuiltinDocumentProperties.Title) ? "untitled" : m_document.BuiltinDocumentProperties.Title;

            m_fileName = string.IsNullOrEmpty(m_fileName) ? m_title : m_fileName;

            if (m_fileName == "untitled")
                return;

            Encoding encoding = Encoding.ASCII;
            Byte[] bytes = encoding.GetBytes(m_fileName);
            m_fileName = encoding.GetString(bytes);

            temp = m_fileName.Trim('?', ' ');
            temp = temp.Replace("?", string.Empty);

            if (temp == string.Empty)
            {
                m_fileName = "untitled";
                return;
            }

            m_fileName = temp;
            temp = string.Empty;

            foreach (char ch in m_fileName.ToCharArray())
            {
                if (char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch) || ch == (char)95 || ch == (char)45 || ch == (char)38)
                    temp = string.Concat(temp, ch);
            }

            m_fileName = temp;
        }

        /// <summary>
        /// Writes MIME type of EPub
        /// </summary>
        private void WriteMIME()
        {
            ZipArchiveItem item = new ZipArchiveItem(m_archieve, "mimetype", null, true, FileAttributes.Normal);
            item.CompressionMethod = CompressionMethod.Stored;

            m_archieve.AddItem(item);
            m_archieve.UpdateItem("mimetype", System.Text.Encoding.Default.GetBytes(EPubConstants.EPubContentType));
        }

        /// <summary>
        /// Returns the MIMETYPE for embedded files
        /// </summary>
        /// <param name="extension">File extenstion</param>
        /// <returns>MIMETYPE of the file</returns>
        private string GetFormat(string extension)
        {
            string mimetype = string.Empty;

            switch (extension)
            {
                case ".png":
                    mimetype = "image/png";
                    break;

                case ".jpeg":
                case ".jpg":
                    mimetype = "image/jpeg";
                    break;

                case ".css":
                    mimetype = "text/css";
                    break;

                case ".ttf":
                    mimetype = "application/octet-stream";
                    break;
            }

            return mimetype;
        }

        # endregion
    }
}

#endif