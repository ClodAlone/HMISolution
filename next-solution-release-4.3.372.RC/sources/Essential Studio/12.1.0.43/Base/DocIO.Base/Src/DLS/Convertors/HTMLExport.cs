#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !(SILVERLIGHT || WP) || WINRT

#region File using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Syncfusion.Layouting;
using System.Collections.Generic;
#if WINRT
using Syncfusion.DocIO.DLS.Entities;
using XmlTextWriter = System.Xml.XmlWriter;
#else
using System.Drawing;
using System.Drawing.Imaging;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The class HTMLExport allows to convert the word document as HTML file.
    /// </summary>
    public class HTMLExport
    {
        #region Constants
        private const string DEF_HYPHEN = "-";
        private const char DEF_FIELD_START = (char)171;
        private const char DEF_FIELD_END = (char)187;
        private const char DEF_NONBREAK_HYPHEN = (char)0x1E;
        private const char DEF_SOFT_HYPHEN = (char)0x1F;
        private const string c_slashRSymbol = "\r";
        private const string c_slashNSymbol = "\n";
        #endregion

        #region Fields
        private XmlTextWriter m_writer;
        private string m_fileNameWithoutExt;
        private string m_destFolder;
        private string m_imagesFolder;
        private int m_imgCounter = 0;
        private int m_currListLevel = -1;
        private bool isKeepValue = false;
        private bool m_bImagesFolderCreated = false;
        private bool m_bFieldOpened = false;
        private Dictionary<int, WFootnote> m_footnotes;
        private Dictionary<int, WFootnote> m_endnotes;
        private int m_footnoteSecIndex;
        private Dictionary<string, Dictionary<int, int>> m_lists;
        private string m_ftntAttrStr;
        private string m_ftntString;
        private bool m_bUseAbsolutePath = false;
        private bool m_bHyperLinkOpened = false;
        private WParagraph m_currPara = null;
        private bool m_bIsFirstSection = true;
        private Dictionary<string, string> m_stylesColl;
        private WordDocument m_document;
        private string m_prefixedValue ;
        private bool m_bIsPrefixedList;
        private bool m_bIsParaWithinDivision;
        private bool m_bIsPreserveListAsPara;
        private WParagraphStyle m_normalStyle;
        private bool m_bIsBookmarkStart;
        /// <summary>
        /// Cache files as stream
        /// </summary>
        private bool m_cacheFilesInternally = false;
        /// <summary>
        /// Has navigation id
        /// </summary>
        private bool m_hasNavigationId = false;
        /// <summary>
        /// OEB header and footer
        /// </summary>
        private bool m_hasOEBHeaderFooter = false;
        /// <summary>
        /// Auto incrementing pointer for navigation point
        /// </summary>
        private int m_nameID;
        /// <summary>
        /// Holds heading styles
        /// </summary>
        private string[] m_headingStyles;
        /// <summary>
        /// Holds the name and data of stylesheets
        /// </summary>
        private MemoryStream m_styleSheet;
        /// <summary>
        /// Holds the name and data of image files
        /// </summary>
        private Dictionary<string, Stream> m_imageFiles;
        /// <summary>
        /// Bookmark collection for TOC
        /// </summary>
        private Dictionary<string, string> m_bookmarks;
        #endregion

        #region Properties
        /// <summary>
        /// When true, specifies that Absolute path should be used.
        /// </summary>
        public bool UseAbsolutePath
        {
            get
            {
                return m_bUseAbsolutePath;
            }
            set
            {
                m_bUseAbsolutePath = Convert.ToBoolean(value);
            }
        }
        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <value>The lists.</value>
        private Dictionary<string, Dictionary<int, int>> Lists
        {
            get
            {
                if (m_lists == null)
                {
                    m_lists = new Dictionary<string, Dictionary<int, int>>();
                }
                return m_lists;
            }
        }
        /// <summary>
        /// Gets the footnotes.
        /// </summary>
        /// <value>The footnotes.</value>
        private Dictionary<int, WFootnote> Footnotes
        {
            get
            {
                if (m_footnotes == null)
                {
                    m_footnotes = new Dictionary<int, WFootnote>();
                }
                return m_footnotes;
            }
        }
        /// <summary>
        /// Gets the endnotes.
        /// </summary>
        /// <value>The endnotes.</value>
        private Dictionary<int, WFootnote> Endnotes
        {
            get
            {
                if (m_endnotes == null)
                {
                    m_endnotes = new Dictionary<int, WFootnote>();
                }
                return m_endnotes;
            }
        }
        /// <summary>
        /// Gets or sets whether to cache files as stream.
        /// </summary>
        internal bool CacheFilesInternally
        {
            get
            {
                return m_cacheFilesInternally;
            }
            set
            {
                m_cacheFilesInternally = value;
            }
        }
        /// <summary>
        /// Gets or sets whether to create navigation point
        /// </summary>
        internal bool HasNavigationId
        {
            get
            {
                return m_hasNavigationId;
            }
            set
            {
                m_hasNavigationId = value;
            }
        }
        /// <summary>
        /// Gets or sets if the OEB header / footer is created
        /// </summary>
        internal bool HasOEBHeaderFooter
        {
            get
            {
                return m_hasOEBHeaderFooter;
            }
            set
            {
                m_hasOEBHeaderFooter = value;
            }
        }
        /// <summary>
        /// Gets the stylesheets to be embedded in document
        /// </summary>
        internal Stream EmbeddedStyleSheet
        {
            get
            {
                return m_styleSheet;
            }
        }
        /// <summary>
        /// Gets the images to be embedded in document
        /// </summary>
        internal Dictionary<string, Stream> EmbeddedImages
        {
            get
            {
                return m_imageFiles;
            }
        }
        /// <summary>
        /// Returns the bookmarks used to create TOC
        /// </summary>
        internal Dictionary<string, string> Bookmarks
        {
            get
            {
                return m_bookmarks;
            }
        }
        #endregion

        #region Public methods
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Saves as XHTML.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="fileName">Name of the file.</param>
        public void SaveAsXhtml(WordDocument doc, string fileName)
        {
            doc.CheckEvalExpired();
            m_document = doc;
            m_fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            m_destFolder = Path.GetDirectoryName(fileName) + "\\";

            if (m_destFolder == "\\")
                m_destFolder = System.Environment.CurrentDirectory + "\\";

            m_imagesFolder = m_destFolder;
            m_stylesColl = new Dictionary<string, string>();
            string cssFileName = m_fileNameWithoutExt + "_styles.css";

#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      if( doc.SaveOptions.HtmlExportCssStyleSheetFileName != null && 
        doc.SaveOptions.HtmlExportCssStyleSheetFileName != string.Empty )
#else
            if (!string.IsNullOrEmpty(doc.SaveOptions.HtmlExportCssStyleSheetFileName))
#endif
            {
                cssFileName = doc.SaveOptions.HtmlExportCssStyleSheetFileName;
            }

#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      if( doc.SaveOptions.HtmlExportImagesFolder != null && 
        doc.SaveOptions.HtmlExportImagesFolder != string.Empty )
#else
            if (!string.IsNullOrEmpty(doc.SaveOptions.HtmlExportImagesFolder))
#endif
            {
                m_bUseAbsolutePath = true;
                if (!doc.SaveOptions.HtmlExportImagesFolder.EndsWith(@"\"))
                    doc.SaveOptions.HtmlExportImagesFolder += "\\";

                m_imagesFolder = doc.SaveOptions.HtmlExportImagesFolder;
            }
            else
                m_bUseAbsolutePath = false;

            if (doc.SaveOptions.HtmlExportCssStyleSheetType == CssStyleSheetType.External)
            {
                using (StreamWriter sw = File.CreateText(m_destFolder + cssFileName))
                {
                    sw.Write(GetStyleSheet(doc));
                }
            }
            m_normalStyle = doc.Styles.FindByName("Normal") as WParagraphStyle;
            if (m_normalStyle == null)
                m_normalStyle = doc.Styles.FindByName("normal") as WParagraphStyle;
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      m_writer = new XmlTextWriter( fileName, Encoding.UTF8 );
#else
            using (m_writer = new XmlTextWriter(fileName, Encoding.UTF8))
#endif
            {
                WriteXhtml(doc, cssFileName);
            }

#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      m_writer.Close();
#endif

        }
        /// <summary>
        /// Saves as XHTML.
        /// </summary>
        /// <param name="doc">Word document to convert</param>
        /// <param name="stream">Stream to save</param>
        /// <param name="EPub">True if XHTML is created for EPub format</param>
        internal void SaveAsXhtml(WordDocument doc, Stream stream, bool EPub)
        {
            if (EPub)
                m_bIsPreserveListAsPara = true;
            doc.CheckEvalExpired();
            m_document = doc;
            m_nameID = 1;
            m_imageFiles = new Dictionary<string, Stream>();

            int headinglevels = m_document.SaveOptions.EPubHeadingLevels;
            m_headingStyles = new string[headinglevels];
            for (int i = 0; i < headinglevels; i++)
                m_headingStyles[i] = string.Concat("heading", (i + 1));

            m_bookmarks = new Dictionary<string, string>();
            SaveAsXhtml(doc, stream);
        }
#endif
        /// <summary>
        /// Saves as XHTML.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="stream">The stream.</param>
        public void SaveAsXhtml(WordDocument doc, Stream stream)
        {
            doc.CheckEvalExpired();
            m_document = doc;
            if (!string.IsNullOrEmpty(doc.SaveOptions.HtmlExportImagesFolder))
                m_bUseAbsolutePath = true;
            else
                m_bUseAbsolutePath = false;
            if (!doc.SaveOptions.HtmlExportImagesFolder.EndsWith(@"\"))
                doc.SaveOptions.HtmlExportImagesFolder += "\\";

            m_destFolder = doc.SaveOptions.HtmlExportImagesFolder;
            m_imagesFolder = m_destFolder;
            
            m_stylesColl = new Dictionary<string, string>();
#if WINRT
            m_writer = CreateWriter(stream);
#else
            m_writer = new XmlTextWriter(stream, Encoding.UTF8);
#endif
            if (!m_cacheFilesInternally)
                WriteXhtml(doc, string.Empty);
            else
                WriteXhtml(doc, doc.SaveOptions.HtmlExportCssStyleSheetFileName);
            m_writer.Flush();
        }
#if WINRT
        /// <summary>
        /// Create xml writer
        /// </summary>
        /// <param name="data">The stream</param>
        /// <returns>returns the xml writer</returns>
        private XmlWriter CreateWriter(Stream data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(data, settings);
            return writer;
        }
#endif

        #endregion

        #region Implementation
        /// <summary>
        /// Writes the XHTML.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="cssFileName">Name of the CSS file.</param>
        private void WriteXhtml(WordDocument doc, string cssFileName)
        {
            WriteHead(doc, cssFileName);

            WriteBody(doc);
           
            m_writer.WriteEndElement(); // Close HTML
        }

        private void WriteHead(WordDocument doc, string cssFileName)
        {
            m_writer.WriteStartDocument();
            m_writer.WriteDocType("html", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd", null);
            m_writer.WriteStartElement("html", "http://www.w3.org/1999/xhtml");
            m_writer.WriteStartElement("head");
            m_writer.WriteRaw("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\" />");
#if AllowUnsafeCode
            m_writer.WriteRaw(
              string.Format("<title>{0}</title>", doc.BuiltinDocumentProperties.Title)
              );
#else
      m_writer.WriteRaw(
        string.Format( "<title>{0}</title>", m_fileNameWithoutExt )
        );
#endif

#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      if(
        doc.SaveOptions.HtmlExportCssStyleSheetType == CssStyleSheetType.Internal ||
        cssFileName == null || cssFileName == string.Empty
        )
#else
            if (
              doc.SaveOptions.HtmlExportCssStyleSheetType == CssStyleSheetType.Internal ||
              string.IsNullOrEmpty(cssFileName)
              )
#endif
            {
                m_writer.WriteStartElement("style");
                m_writer.WriteAttributeString("type", "text/css");
                m_writer.WriteRaw(GetStyleSheet(doc));
                m_writer.WriteEndElement();
            }
            else
            {
                if (m_cacheFilesInternally)
                {
                    m_styleSheet = new MemoryStream();
                    StreamWriter writer = new StreamWriter(m_styleSheet);
                    writer.Write(GetStyleSheet(doc));
                    writer.Flush();
                }
                m_writer.WriteRaw(
                  string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\"/>", cssFileName)
                  );
            }
            
            m_writer.WriteEndElement();
        }

        /// <summary>
        /// Writes body
        /// </summary>
        /// <param name="doc"></param>
        private void WriteBody(WordDocument doc)
        {
            m_writer.WriteStartElement("body");
            m_bIsFirstSection = true;
            foreach (WSection sec in doc.Sections)
            {
                WriteSection(sec);
                WriteFootnotes(FootnoteType.Footnote);
                m_footnoteSecIndex += 1;
            }
            m_bIsFirstSection = true;
            // Writes endnotes
            WriteFootnotes(FootnoteType.Endnote);

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Writes the style sheet.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="cssFileName">Name of the CSS file.</param>
        private string GetStyleSheet(WordDocument doc)
        {
            StringBuilder sb = new StringBuilder();

            if (doc.SaveOptions.EPubExportFont && doc.SaveOptions.FontFiles != null)
            {
                foreach (string str in doc.SaveOptions.FontFiles)
                    AppendLine(sb, str);
            }

            AppendLine(sb, "body{ font-family:'Times New Roman'; font-size:1em; }");
            //AppendLine( sb, "table td{ border: 1px solid black }" );
            AppendLine(sb, "ul, ol{ margin-top: 0; margin-bottom: 0; }");
            foreach (Style style in doc.Styles)
            {
                switch (EncodeName(style.Name).ToLower())
                {
                    case "heading1":
                    case "heading-1":
                    case "heading 1":
                        sb.Append("h1");
                        AppendStyleSheet(style, sb);
                        break;
                    case "heading2":
                    case "heading-2":
                    case "heading 2":
                        sb.Append("h2");
                        AppendStyleSheet(style, sb);
                        break;
                    case "heading3":
                    case "heading-3":
                    case "heading 3":
                        sb.Append("h3");
                        AppendStyleSheet(style, sb);
                        break;
                    case "heading4":
                    case "heading-4":
                    case "heading 4":
                        sb.Append("h4");
                        AppendStyleSheet(style, sb);
                        break;
                    case "heading5":
                    case "heading-5":
                    case "heading 5":
                        sb.Append("h5");
                        AppendStyleSheet(style, sb);
                        break;
                    case "heading6":
                    case "heading-6":
                    case "heading 6":
                        sb.Append("h6");
                        AppendStyleSheet(style, sb);
                        break;
               }
                sb.Append(".");
                sb.Append(EncodeName(style.Name));
                AppendStyleSheet(style, sb);              
            }
            return sb.ToString();
        }
        /// <summary>
        /// Append style sheet
        /// </summary>
        /// <param name="style"></param>
        /// <param name="sb"></param>
        private void AppendStyleSheet(Style style,StringBuilder sb)
        {
            string innerStyle = string.Empty;
            sb.Append("{");

            switch (style.StyleType)
            {
                case StyleType.ParagraphStyle:
                    WParagraphStyle pStyle = style as WParagraphStyle;
                    innerStyle = GetStyle(pStyle.ParagraphFormat, false, m_bIsPreserveListAsPara, null);
                    innerStyle += GetStyle(pStyle.CharacterFormat);
                    break;
                case StyleType.CharacterStyle:
                    CharacterStyle cStyle = style as CharacterStyle;
                    innerStyle = GetStyle(cStyle.CharacterFormat);
                    break;
                case StyleType.OtherStyle:
                    break;
                default:
                    break;
            }

            sb.Append(innerStyle);
            if (!m_stylesColl.ContainsKey(style.Name))
                m_stylesColl.Add(style.Name, innerStyle);
            AppendLine(sb, "}");
        }
        /// <summary>
        /// Appends the line.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="textline">The textline.</param>
        private void AppendLine(StringBuilder sb, string textline)
        {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      sb.Append( textline + System.Environment.NewLine );
#else
            sb.AppendLine(textline);
#endif
        }
        /// <summary>
        /// Writes the section.
        /// </summary>
        /// <param name="sec">The sec.</param>
        private void WriteSection(WSection sec)
        {
            bool writeHF = sec.Document.SaveOptions.HtmlExportHeadersFooters;
            m_writer.WriteStartElement("div");
            m_writer.WriteAttributeString("class", "Section" + sec.GetIndexInOwnerCollection().ToString());

            if (sec.PreviousSibling != null && sec.BreakCode == SectionBreakCode.NewPage)
                m_writer.WriteAttributeString("style", "clear: both; page-break-before: always");

            if (writeHF)
            {
                if (m_bIsFirstSection)
                {
                    if (m_hasOEBHeaderFooter)
                    {
                        m_writer.WriteStartElement("div");
                        m_writer.WriteAttributeString("style", "display: oeb-page-head");
                    }
                    if (sec.PageSetup.DifferentFirstPage)
                    {
                        foreach (TextBodyItem bodyItem in sec.HeadersFooters.FirstPageHeader.Items)
                        {
                            WriteBodyItem(bodyItem);
                        }
                    }
                    else
                    {
                        foreach (TextBodyItem bodyItem in sec.HeadersFooters.OddHeader.Items)
                        {
                            WriteBodyItem(bodyItem);
                        }
                    }
                    if (m_hasOEBHeaderFooter)
                        m_writer.WriteEndElement();
                }
            }
            foreach (TextBodyItem bodyItem in sec.Body.Items)
            {
                WriteBodyItem(bodyItem);
            }

            if (writeHF && sec.NextSibling == null)
            {
                sec = sec.Document.Sections[0];
                if (m_hasOEBHeaderFooter)
                {
                    m_writer.WriteStartElement("div");
                    m_writer.WriteAttributeString("style", "display: oeb-page-foot");
                }
                // Writes footer
                foreach (TextBodyItem bodyItem in sec.HeadersFooters.Footer.Items)
                {
                    WriteBodyItem(bodyItem);
                }
                if (m_hasOEBHeaderFooter)
                    m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();
            m_bIsFirstSection = false;
        }
        /// <summary>
        /// Writes the footnotes.
        /// </summary>
        /// <param name="sec">The sec.</param>
        private void WriteFootnotes(FootnoteType ftnType)
        {
            Dictionary<int,WFootnote> footnotes = (ftnType == FootnoteType.Footnote) ? m_footnotes : m_endnotes;

            if (footnotes != null && footnotes.Count > 0)
            {
                m_writer.WriteElementString("hr", "");

                string ftnIndex = null;
                WFootnote footnote = null;
                foreach (int index in footnotes.Keys)
                {
                    ftnIndex = (index + 1).ToString();
                    footnote = footnotes[index];

                    if (ftnType == FootnoteType.Footnote)
                        m_ftntAttrStr = "_ftnref" + m_footnoteSecIndex + "_" + ftnIndex;
                    else
                        m_ftntAttrStr = "_ednref" + ftnIndex;

                    m_ftntString = "[" + ftnIndex + "] ";

                    foreach (TextBodyItem bodyItem in footnote.TextBody.Items)
                    {
                        WriteBodyItem(bodyItem);
                    }
                }

                if (ftnType == FootnoteType.Footnote)
                    m_footnotes.Clear();
            }
        }
        /// <summary>
        /// Writes the body item.
        /// </summary>
        /// <param name="bodyItem">The body item.</param>
        private void WriteBodyItem(TextBodyItem bodyItem)
        {
            switch (bodyItem.EntityType)
            {
                case EntityType.Paragraph:
                    m_currPara = bodyItem as WParagraph;
                    WriteParagraph(bodyItem as WParagraph);
                    break;
                case EntityType.Table:
                    WriteTable(bodyItem as WTable, false);
                    break;
                case EntityType.StructureDocumentTag:
                    for (int i = 0; i < (bodyItem as StructureDocumentTagBlock).SDTContent.TextBody.Items.Count; i++)
                    {
                        WriteBodyItem((bodyItem as StructureDocumentTagBlock).SDTContent.TextBody.Items[i]);
                    }
                    break;
            }
        }
        /// <summary>
        /// Writes the paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void WriteParagraph(WParagraph para)
        {
            if (para.Items.Count > 0 && para.Items[0].EntityType == EntityType.Break)
            {
                WriteBreak(para.Items[0]);
            }
            WriteParagraphOrList(para);
            WriteFtntAttributes();

            if (para.Items.Count == 0)
            {
                WriteEmptyPara(para.BreakCharacterFormat);
            }
            WriteParagraphItems(para.Items);
            m_writer.WriteEndElement();
            m_writer.WriteRaw("\r\n");
        }
        /// <summary>
        /// Writes the paragraph items.
        /// </summary>
        /// <param name="paraItems">The para items.</param>
        private void WriteParagraphItems(ParagraphItemCollection paraItems)
        {
            for (int i = 0; i < paraItems.Count; i++)
            {
                ParagraphItem item = paraItems[i];
                // Skips field content 
                if (m_bFieldOpened)
                {
                    if ((item.EntityType == EntityType.FieldMark &&
                      (item as WFieldMark).Type == FieldMarkType.FieldEnd) || (item.EntityType == EntityType.FieldMark &&
                      (item as WFieldMark).Type == FieldMarkType.FieldSeparator))
                    {
                        m_bFieldOpened = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (m_bHyperLinkOpened)
                {
                    if ((item.EntityType == EntityType.FieldMark &&
                             (item as WFieldMark).Type == FieldMarkType.FieldEnd))
                    {
                        m_writer.WriteEndElement();
                        m_bHyperLinkOpened = false;
                    }
                    else if (item.EntityType == EntityType.TextRange)
                    {
                        WriteTextRange(item as WTextRange);
                        continue;
                    }
                    else if (item.EntityType == EntityType.Picture)
                    {
                        WriteImage(item as WPicture);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                // Processes paragraph items
                switch (item.EntityType)
                {
                    case EntityType.TextRange:
                        WriteTextRange(item as WTextRange);
                        break;
                    case EntityType.Picture:
                        if ((item as WPicture).ImageRecord != null)
                            WriteImage(item as WPicture);
                        break;
                    case EntityType.Field:
                        WriteField(item as WField);
                        break;
                    case EntityType.FieldMark:
                        break;
                    case EntityType.MergeField:
                        WriteMergeField(item as WMergeField);
                        break;
                    case EntityType.SeqField:
                        break;
                    case EntityType.EmbededField:
                        break;
                    case EntityType.TextFormField:
                    case EntityType.DropDownFormField:
                    case EntityType.CheckBox:
                        WriteFormField(item as WFormField);
                        i += (item as WFormField).Range.Count;
                        break;
                    case EntityType.BookmarkStart:
                        WriteBookmark(item as BookmarkStart);
                        break;
                    case EntityType.BookmarkEnd:
                        if (m_bIsBookmarkStart)
                        {
                            m_writer.WriteEndElement();
                            m_bIsBookmarkStart = false;
                        }
                        break;
                    case EntityType.Shape:
                        break;
                    case EntityType.Comment:
                        break;
                    case EntityType.Footnote:
                        WriteFootnote(item as WFootnote);
                        break;
                    case EntityType.TextBox:
                        WriteTextBox(item as WTextBox);
                        break;
                    case EntityType.Break:
                        ParagraphItemCollection items = paraItems;
                        if (item.Owner is SDTInlineContent)
                            items = (item.Owner.Owner.Owner as WParagraph).GetParagraphItems();

                        if (items.IndexOf(item) > 0)
                        {
                            WriteBreak(item);
                        }
                        break;
                    case EntityType.Symbol:
                        byte code = (item as WSymbol).CharacterCode;
                        string symbol = "&#" + code.ToString() + ";";
                        string fontname = (item as WSymbol).FontName;
                        m_writer.WriteStartElement("font");
                        m_writer.WriteAttributeString("face", fontname);
                        m_writer.WriteRaw(symbol);
                        m_writer.WriteEndElement();
                        break;
                    case EntityType.TOC:
                        break;
                    case EntityType.StructureDocumentTagInline:
                        WriteParagraphItems((item as StructureDocumentTagInline).SDTContent.ParagraphItems);
                        break;
                    //case EntityType.XmlParaItem:
                    //  break;
                    //case EntityType.Undefined:
                    //  break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Write break
        /// </summary>
        /// <param name="item"></param>
        private void WriteBreak(ParagraphItem item)
        {
            if ((item as Break).BreakType == BreakType.LineBreak)
            {
                m_writer.WriteRaw("<br/>");
            }
            else if ((item as Break).BreakType == BreakType.PageBreak)
            {
                m_writer.WriteRaw("<br style='clear:both;page-break-before:always'/>");
            }
        }
        /// <summary>
        /// Writes the text box.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        private void WriteTextBox(WTextBox textBox)
        {
            //Gets the text box as a table
            WTable table = textBox.GetAsTable(0);
            //TODO: GetAsTable() method for the text box need to preserve the same padding as table for the cell. 
            //Currently SamePaddingAsTable property does not resets to false on setting padding value for cell. GetPadding() method currently retrieves the padding of the cell.It cannot be handled based on "SamePaddingAsTable" property.
            //The below code is the workaround to explicitly set the cell padding value from textbox format
            table.Rows[0].Cells[0].CellFormat.Paddings.Left = textBox.TextBoxFormat.InternalMargin.Left;
            table.Rows[0].Cells[0].CellFormat.Paddings.Right  = textBox.TextBoxFormat.InternalMargin.Right ;
            table.Rows[0].Cells[0].CellFormat.Paddings.Bottom  = textBox.TextBoxFormat.InternalMargin.Bottom ;
            table.Rows[0].Cells[0].CellFormat.Paddings.Top  = textBox.TextBoxFormat.InternalMargin.Top ;
            WriteTable(table,true);
        }
        /// <summary>
        /// Writes the footnote.
        /// </summary>
        /// <param name="wFootnote">The w footnote.</param>
        private void WriteFootnote(WFootnote footnote)
        {
            if (footnote.FootnoteType == FootnoteType.Footnote)
            {
                Footnotes.Add(Footnotes.Count, footnote);

                m_writer.WriteStartElement("a");
                m_writer.WriteAttributeString("href", "#" + "_ftnref" + m_footnoteSecIndex + "_" + Footnotes.Count);
                m_writer.WriteAttributeString("class", "Footnote");
                m_writer.WriteRaw(m_footnotes.Count.ToString());
                m_writer.WriteEndElement();
            }
            else
            {
                Endnotes.Add(Endnotes.Count, footnote);

                m_writer.WriteStartElement("a");
                m_writer.WriteAttributeString("href", "#" + "_ednref" + Endnotes.Count);
                m_writer.WriteAttributeString("class", "Endnote");
                m_writer.WriteRaw(Endnotes.Count.ToString());
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Writes the footnote attributes.
        /// </summary>
        private void WriteFtntAttributes()
        {
            if (m_ftntString != null && m_ftntAttrStr != null)
            {
                m_writer.WriteStartElement("a");
                m_writer.WriteAttributeString("id", m_ftntAttrStr);
                m_writer.WriteString(m_ftntString);
                m_writer.WriteEndElement();

                m_ftntAttrStr = null;
                m_ftntString = null;
            }
        }
        /// <summary>
        /// Writes the form field.
        /// </summary>
        /// <param name="field">The field.</param>
        private void WriteFormField(WFormField field)
        {
            switch (field.FieldType)
            {
                case FieldType.FieldFormCheckBox:
                    WCheckBox checkBox = field as WCheckBox;
                    m_writer.WriteStartElement("a");
                    m_writer.WriteAttributeString("name", checkBox.Name);
                    m_writer.WriteEndElement();

                    m_writer.WriteStartElement("input");
                    m_writer.WriteAttributeString("type", "checkbox");
                    m_writer.WriteAttributeString("name", checkBox.Name);
                    if (checkBox.Checked)
                        m_writer.WriteAttributeString("checked", "checked");
                    m_writer.WriteEndElement();
                    break;
                case FieldType.FieldFormDropDown:
                    WDropDownFormField dropDownField = field as WDropDownFormField;
                    m_writer.WriteStartElement("a");
                    m_writer.WriteAttributeString("name", dropDownField.Name);
                    m_writer.WriteEndElement();

                    m_writer.WriteStartElement("select");
                    m_writer.WriteAttributeString("type", "text");
                    m_writer.WriteAttributeString("name", dropDownField.Name);

                    foreach (WDropDownItem item in dropDownField.DropDownItems)
                    {
                        m_writer.WriteStartElement("option");
                        if (dropDownField.DropDownValue == item.Text)
                            m_writer.WriteAttributeString("selected", "selected");
                        m_writer.WriteRaw(item.Text);
                        m_writer.WriteEndElement();
                    }
                    m_writer.WriteEndElement();
                    break;
                case FieldType.FieldFormTextInput:
                    WTextFormField textInput = field as WTextFormField;
                    m_writer.WriteStartElement("a");
                    m_writer.WriteAttributeString("name", textInput.Name);
                    m_writer.WriteEndElement();

                    if (field.Document.SaveOptions.HtmlExportTextInputFormFieldAsText)
                    {
                        m_writer.WriteElementString("span", textInput.Text);
                    }
                    else
                    {
                        m_writer.WriteStartElement("input");
                        m_writer.WriteAttributeString("type", "text");
                        m_writer.WriteAttributeString("name", textInput.Name);
                        m_writer.WriteAttributeString("value", textInput.Text);
                        m_writer.WriteEndElement();
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Writes the paragraph or list.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void WriteParagraphOrList(WParagraph para)
        {
            WListFormat listFormat = GetListFormat(para);
            // close nested list nodes
            if (!m_bIsPreserveListAsPara)
                CloseNestedList(GetLevelNumer(listFormat));
            m_prefixedValue = string.Empty;
            m_bIsPrefixedList = false;
            m_bIsParaWithinDivision = false;
            string style = string.Empty;
            m_currListLevel = GetLevelNumer(listFormat);
            //Ensure whether the paragraph is written within division tag
            if (!para.IsInCell)
                EnsureWithinDivision(para);
            //Write Paragraph 
            if (listFormat.ListType == ListType.NoList)
            {
                if (para.ParaStyle != null)
                {
                    switch (EncodeName(para.ParaStyle.Name).ToLower())
                    {
                        case "heading1":
                        case "heading-1":
                        case "heading 1":
                            m_writer.WriteStartElement("h1");
                            break;
                        case "heading2":
                        case "heading-2":
                        case "heading 2":
                            m_writer.WriteStartElement("h2");
                            break;
                        case "heading3":
                        case "heading-3":
                        case "heading 3":
                            m_writer.WriteStartElement("h3");
                            break;
                        case "heading4":
                        case "heading-4":
                        case "heading 4":
                            m_writer.WriteStartElement("h4");
                            break;
                        case "heading5":
                        case "heading-5":
                        case "heading 5":
                            m_writer.WriteStartElement("h5");
                            break;
                        case "heading6":
                        case "heading-6":
                        case "heading 6":
                            m_writer.WriteStartElement("h6");
                            break;
                        default:
                            m_writer.WriteStartElement("p");
                            break;
                    }
                }
                else
                    m_writer.WriteStartElement("p");
                if (isKeepValue == false)
                    style = GetStyle(para.ParagraphFormat, false, m_bIsPreserveListAsPara, null);
                else
                {
                    if (!m_bIsParaWithinDivision)
                        CreateNavigationPoint(para);
                    style = ValidateStyle(para.StyleName, style);
                }
                WriteParaStyle(para, style,listFormat);
            }
            else
            {
                //Write list
                if (m_bIsPreserveListAsPara)
                {
                    m_currListLevel = -1;
                    PreserveListAsPara(listFormat, para,style);
                }
                else
                {
                   style= WriteList(listFormat ,para,style );
                }
            }

            if (!m_bIsPreserveListAsPara && listFormat.ListType != ListType.NoList)
            {
                WriteParaStyle(para, style,listFormat );
                if (m_bIsPrefixedList)
                {
                    m_currListLevel = -1;
                    m_writer.WriteStartElement("span");
                    m_writer.WriteRaw(m_prefixedValue);
                    m_writer.WriteEndElement();
                    m_writer.WriteStartElement("span");
                    m_writer.WriteRaw("&#xa0;&#xa0;&#xa0;&#xa0;&#xa0;&#xa0;");
                    m_writer.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// Ensure within Division
        /// </summary>
        /// <param name="para"></param>
        private void EnsureWithinDivision(WParagraph para)
        {
            if (para.ParagraphFormat.Keep == true && isKeepValue == false)
            {
                m_writer.WriteStartElement("div");
                string keepStyle = GetStyle(para.ParagraphFormat, false, m_bIsPreserveListAsPara, null);
                Style pLineStyle = para.Document.Styles.FindByName(para.StyleName) as Style;
                if (pLineStyle != null && !IsHeadingStyleNeedToPreserveAsElementSelector(pLineStyle.Name))
                {
                    string classLineAttr = EncodeName(pLineStyle.Name);
                    m_writer.WriteAttributeString("class", classLineAttr);
                }
                if (!string.IsNullOrEmpty(para.StyleName))
                    CreateNavigationPoint(para);
                if (!string.IsNullOrEmpty(keepStyle))
                {
                    keepStyle = ValidateStyle(para.StyleName, keepStyle);
                    if (keepStyle != string .Empty)
                        m_writer.WriteAttributeString("style", keepStyle);
                }
                isKeepValue = true;
                m_bIsParaWithinDivision = true;
            }
            if (para.ParagraphFormat.Keep == false && isKeepValue == true)
            {
                m_writer.WriteEndElement();
                isKeepValue = false;
            }
        }

        # region List Implementation
        /// <summary>
        /// Write style attribute for paragraph
        /// </summary>
        /// <param name="para"></param>
        /// <param name="style"></param>
        private void WriteParaStyle(WParagraph para, string style,WListFormat listFormat)
        {
            if (para.StyleName != null && para.StyleName.Length > 0 && para.StyleName != "Normal" && isKeepValue == false)
            {
                Style pStyle = para.Document.Styles.FindByName(para.StyleName) as Style;

                if ((pStyle != null && !IsHeadingStyleNeedToPreserveAsElementSelector(para.StyleName)) || (listFormat != null && listFormat.ListType != ListType.NoList ))
                {
                    string classAttr = EncodeName(pStyle.Name);
                    m_writer.WriteAttributeString("class", classAttr);
                }
                CreateNavigationPoint(para);
                style = ValidateStyle(para.StyleName, style);
            }
            if (style.Length > 0)
            {
                m_writer.WriteAttributeString("style", style);
            }
        }
            /// <summary>
        /// Check whether the style is heading style
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private bool IsHeadingStyleNeedToPreserveAsElementSelector(string styleName)
        {
            switch (EncodeName(styleName).ToLower())
            {
                case "heading1":
                case "heading-1":
                case "heading 1":
                case "heading2":
                case "heading-2":
                case "heading 2":
                case "heading3":
                case "heading-3":
                case "heading 3":
                case "heading4":
                case "heading-4":
                case "heading 4":
                case "heading5":
                case "heading-5":
                case "heading 5":
                case "heading6":
                case "heading-6":
                case "heading 6":
                    return true;
                default:
                    return false;
            }
        }
    /// <summary>
    /// Write List 
    /// </summary>
    /// <param name="listFormat"></param>
    /// <param name="para"></param>
    /// <param name="style"></param>
    /// <returns></returns>
        private string WriteList(WListFormat listFormat, WParagraph para, string style)
        {
            string listFormatStyle=null ;
            int startAt = 0;
            if (listFormat.CurrentListLevel.PatternType != ListPatternType.Bullet)
            {
                startAt = GetStartValue(listFormat);
            }
            WriteListStartTag(listFormat, startAt);            
            
            if (!m_bIsPrefixedList)
                    m_writer.WriteStartElement("li");
            else
                m_writer.WriteStartElement("p");
            style = GetStyle(para.ParagraphFormat, true, m_bIsPreserveListAsPara, listFormat);
            

            if(listFormat !=null && listFormat.CurrentListLevel !=null )
            {
                WCharacterFormat charFormat = GetCharacterFormatOfList(para);
                //Gets the actual list level character formatting
                if (charFormat != null)
                    listFormatStyle = GetStyle(charFormat);
                else
                {
                    if (para.BreakCharacterFormat != null)
                        style += GetStyle(para.BreakCharacterFormat);
                    listFormatStyle = GetStyle(listFormat.CurrentListLevel.CharacterFormat);
                }

                listFormatStyle = listFormatStyle.Replace("font-family:'Wingdings';", string.Empty);
            }

            if (listFormatStyle != null)
            {
                style = EnsureStyle(listFormatStyle, style);
            }
            return style;

        }
        /// <summary>
        /// Gets the CharacterFormat of the list
        /// </summary>
        private WCharacterFormat GetCharacterFormatOfList(WParagraph paragraph)
        {
            if (paragraph.ListFormat.IsEmptyList
                || paragraph.SectionEndMark)
                return null;
            WCharacterFormat characterFormat = null;
            WListFormat listFormat = null;
            WParagraphStyle pStyle = paragraph.ParaStyle as WParagraphStyle;
            if (paragraph.ListFormat.ListType != ListType.NoList)
                listFormat = paragraph.ListFormat;
            else if (pStyle.ListFormat.ListType != ListType.NoList)
                listFormat = pStyle.ListFormat;

            if (listFormat != null
                && listFormat.CurrentListStyle != null)
            {
                ListStyle listStyle = listFormat.CurrentListStyle;
                int levelNumber = 0;
                if (paragraph.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                    levelNumber = paragraph.ListFormat.ListLevelNumber;
                else if (pStyle.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                    levelNumber = pStyle.ListFormat.ListLevelNumber;

                // Updates current list level.
                WListLevel level = listStyle.GetNearLevel(levelNumber);

                ListOverrideStyle listOverrideStyle = null;
                if (listFormat.LFOStyleName != null
                    && listFormat.LFOStyleName.Length > 0)
                    listOverrideStyle = (m_document as WordDocument).ListOverrides.FindByName(listFormat.LFOStyleName);
                if (listOverrideStyle != null
                    && listOverrideStyle.OverrideLevels.HasOverrideLevel(levelNumber)
                    && listOverrideStyle.OverrideLevels[levelNumber].OverrideFormatting)
                    level = listOverrideStyle.OverrideLevels[levelNumber].OverrideListLevel;
                // Updates character format for the list.
                characterFormat = new WCharacterFormat(m_document);
                characterFormat.ImportContainer(paragraph.BreakCharacterFormat);
                characterFormat.CopyProperties(paragraph.BreakCharacterFormat);
                characterFormat.ApplyBase(paragraph.BreakCharacterFormat.BaseFormat);
                if (characterFormat.PropertiesHash.ContainsKey(WCharacterFormat.UnderlineKey))
                {
                    characterFormat.UnderlineStyle = UnderlineStyle.None;
                    characterFormat.PropertiesHash.Remove(WCharacterFormat.UnderlineKey);
                }

                //Copy character fomatting from list format
                CopyCharacterFormatting(level.CharacterFormat, characterFormat);
            }
            return characterFormat;
        }
        /// <summary>
        /// Copys Character formatting
        /// </summary>
        /// <param name="destFormat"></param>
        /// <param name="sourceFormat"></param>
        private void CopyCharacterFormatting(WCharacterFormat sourceFormat, WCharacterFormat destFormat)
        {
            if (sourceFormat.HasValue(WCharacterFormat.FontSizeKey))
                destFormat.FontSize = sourceFormat.FontSize;
            if (sourceFormat.HasValue(WCharacterFormat.TextColorKey))
                destFormat.TextColor = sourceFormat.TextColor;
            if (sourceFormat.HasValue(WCharacterFormat.FontNameKey))
                destFormat.FontName = sourceFormat.FontName;
            if (sourceFormat.HasValue(WCharacterFormat.BoldKey))
                destFormat.Bold = sourceFormat.Bold;
            if (sourceFormat.HasValue(WCharacterFormat.ItalicKey))
                destFormat.Italic = sourceFormat.Italic;
            if (sourceFormat.HasValue(WCharacterFormat.UnderlineKey))
                destFormat.UnderlineStyle = sourceFormat.UnderlineStyle;
            if (sourceFormat.HasValue(WCharacterFormat.HighlightColorKey))
                destFormat.HighlightColor = sourceFormat.HighlightColor;
            if (sourceFormat.HasValue(WCharacterFormat.ShadowKey))
                destFormat.Shadow = sourceFormat.Shadow;
            if (sourceFormat.HasValue(WCharacterFormat.SpacingKey))
                destFormat.CharacterSpacing = sourceFormat.CharacterSpacing;
            if (sourceFormat.HasValue(WCharacterFormat.DoubleStrikeKey))
                destFormat.DoubleStrike = sourceFormat.DoubleStrike;
            if (sourceFormat.HasValue(WCharacterFormat.EmbossKey))
                destFormat.Emboss = sourceFormat.Emboss;
            if (sourceFormat.HasValue(WCharacterFormat.EngraveKey))
                destFormat.Engrave = sourceFormat.Engrave;
            if (sourceFormat.HasValue(WCharacterFormat.SubSuperScriptKey))
                destFormat.SubSuperScript = sourceFormat.SubSuperScript;
            destFormat.TextBackgroundColor = sourceFormat.TextBackgroundColor;
            if (sourceFormat.HasValue(WCharacterFormat.AllCapsKey))
                destFormat.AllCaps = sourceFormat.AllCaps;
            if (sourceFormat.Bidi)
            {
                destFormat.Bidi = true;
                destFormat.FontNameBidi = sourceFormat.FontNameBidi;
                destFormat.FontSizeBidi = sourceFormat.FontSizeBidi;
            }
            if (sourceFormat.HasValue(WCharacterFormat.BoldBidiKey))
                destFormat.BoldBidi = sourceFormat.BoldBidi;
            if (sourceFormat.HasValue(WCharacterFormat.FieldVanishKey))
                destFormat.FieldVanish = sourceFormat.FieldVanish;
            if (sourceFormat.HasValue(WCharacterFormat.HiddenKey))
                destFormat.Hidden = sourceFormat.Hidden;
            if (sourceFormat.HasValue(WCharacterFormat.SmallCapsKey))
                destFormat.SmallCaps = sourceFormat.SmallCaps;
        }
        /// <summary>
        /// Ensure whether the style is already defined and appends the style accordingly
        /// </summary>
        /// <param name="currentStyle"></param>
        /// <param name="existingStyle"></param>
        /// <returns></returns>
        private string EnsureStyle(string currentStyle, string existingStyle)
        {
            bool isAleadySpecified = false;
            string[] existingStyleAttribute = existingStyle.Split(new char[] { ';' });
            string[] currentStyleAttribute = currentStyle.Split(new char[] { ';' });

            foreach (string text in currentStyleAttribute)
            {
                isAleadySpecified = false;
                if (text.Length > 0)
                {
                    int index = text.IndexOf(":");
                    string attName = text.Substring(0, index);
                    string attValue = text.Substring(index + 1);

                    foreach (string existingText in existingStyleAttribute)
                    {
                        if (existingText.Contains(attName ))
                        {
                            isAleadySpecified =true ;
                            int exitingTextIndex=existingText .IndexOf (":");
                            string extAttValue = existingText.Substring(exitingTextIndex + 1);
                            string overwrittenText= existingText.Replace(extAttValue, attValue);
                            existingStyle = existingStyle.Replace(existingText, overwrittenText);
                        }
                    }
                    if (!isAleadySpecified)
                    {                       
                        existingStyle += attName + ":" + attValue + ";";
                    }
                    
                }
            }
            return existingStyle;
        }
        /// <summary>
        /// Removes duplicate style attribute entry
        /// </summary>
        /// <param name="p">Name of the style</param>
        /// <param name="style">Current style</param>
        /// <returns>validated style</returns>
        private string ValidateStyle(string p, string style)
        {
            // Removes duplicate style attribute entry
            if (!string.IsNullOrEmpty(p) &&  m_stylesColl.ContainsKey(p))
            {
                string[] styleAtt = m_stylesColl[p].Split(new char[] { ';' });

                foreach (string text in styleAtt)
                {
                    if (text.Length > 0 && style.Length > 0)
                        style = style.Replace(text + ";", string.Empty);
                }
            }

            return style;
        }
     /// <summary>
     /// Write list start tag
     /// </summary>
     /// <param name="listFormat"></param>
     /// <param name="startAt"></param>
        private void WriteListStartTag(WListFormat listFormat, int startAt)
        {
            if (m_currListLevel >= 0)
            {
                if (listFormat.CurrentListLevel.PatternType == ListPatternType.Bullet)
                {
                    m_writer.WriteStartElement("ul");
                    WriteListType(listFormat.CurrentListLevel.PatternType, listFormat);
                    m_writer.WriteRaw("\r\n");
                }
                else if (listFormat.CurrentListLevel.NumberPrefix != null && !listFormat.CurrentListLevel.NumberPrefix.StartsWith("\0."))
                {
                    m_writer.WriteStartElement("ol");
                    WriteListType(listFormat.CurrentListLevel.PatternType, listFormat);
                    if (startAt >= 0)
                        m_writer.WriteAttributeString("start", startAt.ToString());
                    m_writer.WriteRaw("\r\n");
                }
                else
                {
                    m_bIsPrefixedList = true;
                    m_prefixedValue = GetPrefixValue(listFormat, startAt);

                }
            }
        }

      /// <summary>
      /// Gets StartAt Value for current list level
      /// </summary>
      /// <param name="listFormat"></param>
      /// <returns></returns>
        private int GetStartValue(WListFormat listFormat)
        {
            int startAt = 0;
            if (listFormat.RestartNumbering)
                EnsureLvlRestart(listFormat, true);
            else if (listFormat.ListLevelNumber == 0)
                EnsureLvlRestart(listFormat, false);
            startAt = GetLstStartVal(listFormat);

            return startAt;

        }
        
      /// <summary>
      /// Preserve List as paragraph tag
      /// </summary>
      /// <param name="listFormat"></param>
      /// <param name="para"></param>
      /// <param name="style"></param>
        private void PreserveListAsPara(WListFormat listFormat, WParagraph para, string style)
        {
                int startAt = 0;                      
                startAt = GetStartValue(listFormat);          
                m_writer.WriteStartElement("p");
                style = GetStyle(para.ParagraphFormat, true,m_bIsPreserveListAsPara,listFormat );
                if (listFormat.CurrentListLevel.NumberPrefix != null && listFormat.CurrentListLevel.NumberPrefix.StartsWith("\0."))
                    {
                        m_bIsPrefixedList = true;
                        m_prefixedValue = GetPrefixValue(listFormat, startAt);
                    }
                WriteParaStyle(para, style,listFormat);
                //Preserve bullet and numbering as a text
                PreserveBulletsAndNumberingAsText(listFormat, startAt);
                   
        }
        /// <summary>
        /// Write prefix value for List
        /// </summary>
        /// <param name="listFormat"></param>
        private void PreserveBulletsAndNumberingAsText(WListFormat listFormat, int startAt)
        {
            string fontStyle = GetStyle(listFormat.CurrentListLevel.CharacterFormat);
            m_writer.WriteStartElement("span");
            m_writer.WriteAttributeString("style", fontStyle);
            if (listFormat.CurrentListLevel.PatternType == ListPatternType.Bullet)
            {
                if (listFormat.CurrentListLevel.CharacterFormat.FontName.ToLower () == "symbol" || listFormat.CurrentListLevel.CharacterFormat.FontName.ToLower () == "wingdings")
                {
                    byte symbol = (byte)listFormat.CurrentListLevel.BulletCharacter[0];
                    m_writer.WriteRaw("&#" + symbol.ToString() + ";");
                }
                else
                   m_writer.WriteRaw(listFormat.CurrentListLevel.BulletCharacter);
            }
            else if (m_bIsPrefixedList)
            {
                m_writer.WriteRaw(m_prefixedValue);
            }
            else
            {
                string listChar = GetNumberingsAsText(listFormat.CurrentListLevel.PatternType, startAt);
                m_writer.WriteRaw(listChar);
            }
            if (!m_bIsPrefixedList && listFormat.CurrentListLevel.NumberSufix != null)
                m_writer.WriteRaw(listFormat.CurrentListLevel.NumberSufix.ToString());
            m_writer.WriteEndElement();
            WriteTabSpace(listFormat);
        }
        /// <summary>
        /// Write tab space for list
        /// </summary>
        /// <param name="listFormat"></param>
        private void WriteTabSpace(WListFormat listFormat)
        {
            int tabCount=0;
            StringBuilder sb = new StringBuilder();
            float textPosition = listFormat.CurrentListLevel.TextPosition;
            float bulletPosition = listFormat.CurrentListLevel.TextPosition + listFormat.CurrentListLevel.NumberPosition;
            float tabStopPosition = listFormat.CurrentListLevel.TabSpaceAfter;
            if (listFormat.CurrentListLevel.TabSpaceAfter <= 0)
                tabCount = (int)Math.Round((textPosition - bulletPosition) / 36.0);
            else
                tabCount = (int)Math.Round((listFormat.CurrentListLevel.TabSpaceAfter - bulletPosition) / 36.0);
            m_writer.WriteStartElement("span");
            sb.Append("font-size:" + XmlConvert.ToString(Math.Round(7f / 12, 2)) + "em;");
            sb.Append("font-family:'Times New Roman';");
            m_writer.WriteAttributeString("style", sb.ToString());
            //Creates tab spaces between bullets/Numbering and Text
            if (tabCount > 0)
            {
                for (int i = 0; i < tabCount; i++)
                {
                    for (int j = 0; j < 22; j++)
                    {
                        m_writer.WriteRaw("&#xa0;");
                    }
                }
            }
            else
            {
                m_writer.WriteRaw("&#xa0;&#xa0;&#xa0;&#xa0;&#xa0;&#xa0;&#xa0;");
            }
            m_writer.WriteEndElement();
        }
   
        /// <summary>
        /// Get list character for Epub
        /// </summary>
        /// <param name="listFormat"></param>
        /// <param name="startAt"></param>
        /// <returns></returns>
        private string GetNumberingsAsText(ListPatternType type, int startAt)
        {
            int value = 0;
            string listChar = String.Empty;
            switch (type)
            {
                case ListPatternType .LowLetter :
                    value = 97 + (startAt - 1);
                    listChar = Char.ConvertFromUtf32(value);
                    break;
                case ListPatternType .UpLetter :
                    value = 65 + (startAt - 1);
                    listChar = Char.ConvertFromUtf32(value);
                    break;
                case ListPatternType .LowRoman :
                    listChar = ConvertArabicToRoman(startAt).ToLower();
                    break;
                case ListPatternType .UpRoman :
                    listChar = ConvertArabicToRoman(startAt).ToUpper();
                    break;
                default:
                   listChar = startAt.ToString(); 
                    break ;
            }
            return listChar;
        }
        /// <summary>
        /// Convert Arabic to Roman
        /// </summary>
        /// <param name="arabic"></param>
        /// <returns></returns>
        private string ConvertArabicToRoman(int arabic)
        {
            string result = "";
            for (int i = 0; i < arabic; i++)
            {
                while (arabic >= 1000)
                {//check for thousands place

                    result = result + "M";
                    arabic = arabic - 1000;
                }
                while (arabic >= 900)
                {
                    //check for nine hundred place
                    result = result + "CM";
                    arabic = arabic - 900;
                }
                while (arabic >= 500)
                {
                    //check for five hundred place
                    result = result + "D";
                    arabic = arabic - 500;
                }
                while (arabic >= 400)
                {
                    //check for four hundred place
                    result = result + "CD";
                    arabic = arabic - 400;
                }
                while (arabic >= 100)
                {
                    //check for one hundred place
                    result = result + "C";
                    arabic = arabic - 100;
                }
                while (arabic >= 90)
                {
                    //check for ninety place
                    result = result + "XC";
                    arabic = arabic - 90;
                }
                while (arabic >= 50)
                {
                    //check for fifty place
                    result = result + "L";
                    arabic = arabic - 50;
                }
                while (arabic >= 40)
                {
                    // check for forty place
                    result = result + "XL";
                    arabic = arabic - 40;
                }

                while (arabic >= 10)
                {
                    // check for tenth place
                    result = result + "X";
                    arabic = arabic - 10;
                }
                while (arabic >= 9)
                {
                    //check for nineth place
                    result = result + "IX";
                    arabic = arabic - 9;
                }
                while (arabic >= 5)
                {
                    //check for fifth place
                    result = result + "V";
                    arabic = arabic - 5;
                }
                while (arabic >= 4)
                {
                    //check for fourth place
                    result = result + "IV";
                    arabic = arabic - 4;
                }
                while (arabic >= 1)
                {
                    //check for first place
                    result = result + "I";
                    arabic = arabic - 1;
                }
            }
            return result;

        }
     /// <summary>
     /// Get Prefix value
     /// </summary>
     /// <param name="listFormat"></param>
     /// <param name="startAt"></param>
     /// <returns></returns>
        private string  GetPrefixValue(WListFormat listFormat, int startAt)
        {
            string m_prefixedValue = string.Empty;
            int levelNo = listFormat.CurrentListLevel.LevelNumber;
            string styleName = listFormat.CustomStyleName;
            string prefix = listFormat.CurrentListLevel.NumberPrefix;
            if (Lists.ContainsKey(listFormat.CustomStyleName))
            {
                string value = string.Empty;
                Dictionary<int, int> levels = Lists[listFormat.CustomStyleName];
                for (int i = 0; i < levelNo; i++)
                {
                    if (levels.ContainsKey(i))
                    {
                        value += Convert.ToString(Convert.ToInt32(levels[i]) - 1) + ".";                      
                    }
                }             
                value += startAt.ToString();
                m_prefixedValue = value + listFormat.CurrentListLevel.NumberSufix;              
            }
            return m_prefixedValue;
        }

        # endregion


        /// <summary>
        /// Writes the bookmark.
        /// </summary>
        /// <param name="bookmarkStart">The bookmark start.</param>
        private void WriteBookmark(BookmarkStart bookmark)
        {
            m_bIsBookmarkStart = true;
            m_writer.WriteStartElement("a");
            m_writer.WriteAttributeString("id", bookmark.Name);
            m_writer.WriteRaw(string .Empty);
        }
        /// <summary>
        /// Writes the field.
        /// </summary>
        /// <param name="wField">The w field.</param>
        private void WriteField(WField field)
        {
            if (field.FieldEnd == null && field.FieldType == FieldType.FieldUnknown)
            {
                WTextRange textRamge = new WTextRange(m_document);
                textRamge.ApplyCharacterFormat(field.CharacterFormat);
                textRamge.Text = field.FieldCode;
                WriteTextRange(textRamge);
            }
            else
            {
                switch (field.FieldType)
                {
                    case FieldType.FieldHyperlink:
                        Hyperlink hyperlink = new Hyperlink(field);
                        WriteHyperlink(hyperlink);
                        m_bHyperLinkOpened = true;
                        break;
                    case FieldType.FieldMergeField:
                        WriteMergeField(field as WMergeField);
                        break;
                    case FieldType.FieldIf:
                        m_bFieldOpened = true;
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Writes the hyperlink.
        /// </summary>
        /// <param name="hyperlink">The hyperlink.</param>
        private void WriteHyperlink(Hyperlink hyperlink)
        {
            m_writer.WriteStartElement("a");

            string hyperlinkStyle = hyperlink.Field.CharacterFormat.CharStyleName;

            if (hyperlinkStyle != null && hyperlinkStyle.Length > 0)
            {
                Style charStyle = hyperlink.Field.Document.Styles.FindByName(hyperlinkStyle) as Style;
                string classAttr = GetClassAttr(charStyle, hyperlink.Field.Document);
                m_writer.WriteAttributeString("class", classAttr);
            }
            WField field = new WField(hyperlink.Field.Document);
            WTextRange tr = new WTextRange(hyperlink.Field.Document);
            ParagraphItem item;
            field = hyperlink.Field as WField;
            item = field.NextSibling as ParagraphItem;
            if (item.NextSibling is WTextRange)
            {
                tr = item.NextSibling as WTextRange;
            }
            string style = GetStyle(tr.CharacterFormat);
            if(!string.IsNullOrEmpty(hyperlinkStyle))
                style = ValidateStyle(hyperlinkStyle,style);

            if (style.Length > 0)
            {
                m_writer.WriteAttributeString("style", style);
            }

            switch (hyperlink.Type)
            {
                case HyperlinkType.None:
                    break;
                case HyperlinkType.FileLink:
                    m_writer.WriteAttributeString("href", hyperlink.FilePath);
                    break;
                case HyperlinkType.WebLink:
                    m_writer.WriteAttributeString("href", hyperlink.Uri);
                    break;
                case HyperlinkType.EMailLink:
                    m_writer.WriteAttributeString("href", hyperlink.Uri);
                    break;
                case HyperlinkType.Bookmark:
                    m_writer.WriteAttributeString("href", "#" + hyperlink.BookmarkName);
                    break;
                default:
                    break;
            }
           // m_writer.WriteString(hyperlink.TextToDisplay);
            
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Writes the image.
        /// </summary>
        /// <param name="pic">The pic.</param>
        private void WriteImage(WPicture pic)
        {
            //string extension = (pic.IsMetaFile) ? ".png" : ".jpeg";
            Image image = pic.Image;
            ImageFormat format = GetImageFormat(image.RawFormat.Guid.ToString());
            string extension = "." + format.ToString().ToLower();
            string imgPath = GetImagePath() + extension;
            EnsureImagesFolder();

            int width = (int)Math.Round(UnitsConvertor.Instance.ConvertToPixels(
              (pic.Width * pic.WidthScale / 100), PrintUnits.Point));
            int height = (int)Math.Round(UnitsConvertor.Instance.ConvertToPixels(
              (pic.Height * pic.HeightScale / 100), PrintUnits.Point));

            if (width < 0 || height < 0)
                return;

            ProcessImage(image, imgPath, width, height, pic.IsMetaFile, format);

            //Wrapping styles InFrontOfText and Behind are handled here with the help of span tag
            if (pic.TextWrappingStyle == TextWrappingStyle.InFrontOfText || pic.TextWrappingStyle == TextWrappingStyle.Behind)
            {
                m_writer.WriteStartElement("span");
                if (pic.VerticalAlignment == ShapeVerticalAlignment.Center && pic.HorizontalAlignment == ShapeHorizontalAlignment.Center)
                {
                    m_writer.WriteAttributeString("style", "position:" + ShapePosition.Absolute + "; width:" + width.ToString() + "px; height:" + height.ToString() + "px; left:0px; margin-left:0px; margin-top:0px;");
                }

                else if (pic.HorizontalAlignment == ShapeHorizontalAlignment.Right)
                {
                    //As per MS Word behavior, the addition of image width and margin left value written in the span tag must be equal to 1024 when right alignment is set.
                    int marginLeft = 1024 - width;
                    m_writer.WriteAttributeString("style", "position:" + ShapePosition.Absolute + "; width:" + width.ToString() + "px; height:" + height.ToString() + "px; left:0px; margin-left:" + marginLeft + "px; margin-top:0px;");
                }
                else
                {
                    m_writer.WriteAttributeString("style", "position:" + ShapePosition.Absolute + "; width:" + width.ToString() + "px; height:" + height.ToString() + "px; left:0px; margin-left:" +
                        Math.Round(UnitsConvertor.Instance.ConvertToPixels(pic.HorizontalPosition, PrintUnits.Point)).ToString()
                    + "px; margin-top:" + Math.Round(UnitsConvertor.Instance.ConvertToPixels(pic.VerticalPosition, PrintUnits.Point)).ToString() + "px;");
                }
            }

            m_writer.WriteStartElement("img");
            if (m_imagesFolder == "\\")
            {
                //Embed base64 string representation of the image when the HTML document is saved as stream
                string data = "data:image/" + format.ToString().ToLower() + "";
                imgPath = Convert.ToBase64String(pic.ImageBytes);
                m_writer.WriteAttributeString("src", data + ";base64," + imgPath);
            }
            else
            {
                m_writer.WriteAttributeString("src", (UseAbsolutePath ? m_imagesFolder : string.Empty) + imgPath);
            }
            m_writer.WriteAttributeString("width", width.ToString());
            m_writer.WriteAttributeString("height", height.ToString());

            //Alignment of the Image is handled for the wrapping styles Square, Tight and Through
            if (pic.TextWrappingStyle == TextWrappingStyle.Square || pic.TextWrappingStyle == TextWrappingStyle.Tight || pic.TextWrappingStyle == TextWrappingStyle.Through)
            {
                if (pic.HorizontalAlignment == ShapeHorizontalAlignment.Right)
                {                   
                    m_writer.WriteAttributeString("align", "right");
                }
                else
                {
                    //Alignment left is set for the cases left, center and None
                    m_writer.WriteAttributeString("align", "left");
                }
                //TODO: In our code base Distance from text property is not handled for Pictures. Once it is handled, hspace and vspace attributes of img tag have to be included here.
                // Eg : m_writer.WriteAttributeString("hspace", "value");
            }
            // Write alternative text
            if (!string.IsNullOrEmpty(pic.AlternativeText))
                m_writer.WriteAttributeString("alt", pic.AlternativeText);

            m_writer.WriteEndElement();

            //Wrapping style TopAndBottom is handled here
            if (pic.TextWrappingStyle == TextWrappingStyle.TopAndBottom)
            {
                m_writer.WriteStartElement("br");
                m_writer.WriteAttributeString("clear", "ALL");
            }

            //Writing the end tag of span
            if (pic.TextWrappingStyle == TextWrappingStyle.InFrontOfText || pic.TextWrappingStyle == TextWrappingStyle.Behind)
            {
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Gets image format
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private ImageFormat GetImageFormat(string value)
        {
            ImageFormat format = ImageFormat.Jpeg;

            if (value == ImageFormat.Emf.Guid.ToString() || value == ImageFormat.Wmf.Guid.ToString())
            {
                format = ImageFormat.Png;
            }
            else if (value == ImageFormat.Png.Guid.ToString())
            {
                format = ImageFormat.Png;
            }
            else
                format = ImageFormat.Jpeg; 
          
            return format;
        }
#endif
#if WINRT
        /// <summary>
        /// Writes the image.
        /// </summary>
        /// <param name="pic">The pic.</param>
        private void WriteImage(WPicture pic)
        {
            Image image = pic.Image;
            ImageFormat format = pic.Image.Format;
            string extension = "." + format.ToString().ToLower();

            int width = (int)Math.Round(UnitsConvertor.Instance.ConvertToPixels(
              (pic.Width * pic.WidthScale / 100), PrintUnits.Point));
            int height = (int)Math.Round(UnitsConvertor.Instance.ConvertToPixels(
              (pic.Height * pic.HeightScale / 100), PrintUnits.Point));

            if (width < 0 || height < 0)
                return;

            m_writer.WriteStartElement("img");
            //Embed base64 string representation of the image when the HTML document is saved as stream
            string data = "data:image/" + extension + "";
            string imgPath = Convert.ToBase64String(pic.ImageBytes);
            m_writer.WriteAttributeString("src", data + ";base64," + imgPath);
            m_writer.WriteAttributeString("width", width.ToString());
            m_writer.WriteAttributeString("height", height.ToString());

            // Write alternative text
            if (!string.IsNullOrEmpty(pic.AlternativeText))
                m_writer.WriteAttributeString("alt", pic.AlternativeText);

            m_writer.WriteEndElement();

            // break should be added if text wrapping is not inline
            if (pic.TextWrappingStyle == TextWrappingStyle.Inline)
                m_writer.WriteRaw("<br/>");
        }
#endif
        /// <summary>
        /// Writes the text range.
        /// </summary>
        /// <param name="wTextRange">The w text range.</param>
        private void WriteTextRange(WTextRange tr)
        {
            if (tr.Text.Length > 0 && tr.Text[0] != (char)2)
            {
                m_writer.WriteStartElement("span");

                string style = GetStyle(tr.CharacterFormat);

                if (tr.CharacterFormat.CharStyleName != null && tr.CharacterFormat.CharStyleName.Length > 0)
                {
                    Style charStyle = tr.Document.Styles.FindByName(tr.CharacterFormat.CharStyleName) as Style;
                    string classAttr = GetClassAttr(charStyle, tr.Document);
                    m_writer.WriteAttributeString("class", classAttr);

                    if (!string.IsNullOrEmpty(tr.CharacterFormat.CharStyleName))
                        style = ValidateStyle(tr.CharacterFormat.CharStyleName, style);
                }

                if (style.Length > 0)
                {
                    m_writer.WriteAttributeString("style", style);
                }

                if (tr.Text == string.Empty)
                {
                    m_writer.WriteRaw("������������� ");
                }
                else
                {
                    string text=tr.Text;
                    if (tr.OwnerParagraph != null && tr.OwnerParagraph.Items.FirstItem == tr)
                    {
                        text = ReplaceEmptySpace(text);
                    }
                    WriteText(text);
                }

                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Writes the merge field.
        /// </summary>
        /// <param name="mField">The merge field.</param>
        private void WriteMergeField(WMergeField mField)
        {
            WTextRange text = new WTextRange(mField.Document);

            if ((mField as WField).FieldValue != string.Empty)
                text.Text = mField.FieldValue;
            else
                text.Text = mField.UpdateMergeFieldText(mField);
            text.ApplyCharacterFormat(mField.CharacterFormat);
            WriteTextRange(text);

        }
        #endregion

        #region Implementation / table
        /// <summary>
        /// Writes the table.
        /// </summary>
        /// <param name="wTable">The w table.</param>
        private void WriteTable(WTable table, bool isTableCreatedFromTextBox)
        {
            ApplyTableGridStyle(table, isTableCreatedFromTextBox);
            //Updates the actual formatting from table styles
            table.ApplyBaseStyleFormats();
            if (table.Rows.Count == 0)
                return;
            CloseNestedList(-1);
            List<float> colOffsets = CalculateOffsets(table);

            // Write table width
            m_writer.WriteStartElement("div");
            m_writer.WriteStartElement("table");

            WriteTableAttributes(table);

            WTableRow row = null;
            WTableCell cell = null;

            for (int i = 0, cnt = table.Rows.Count; i < cnt; i++)
            {
                row = table.Rows[i];
                m_writer.WriteStartElement("tr");
                float currOffset = 0f;
                WriteRowAttributes(row);
                short gridCount = row.RowFormat.GridBefore;
                if (gridCount > 0)
                {
                    WriteGridCell(gridCount, row.RowFormat.GridBeforeWidth);
                }
                for (int j = 0, cellCnt = row.Cells.Count; j < cellCnt; j++)
                {
                    cell = row.Cells[j];
                    if (cell.CellFormat.VerticalMerge == CellMerge.Continue ||
                      cell.CellFormat.HorizontalMerge == CellMerge.Continue)
                    {
                        currOffset += (float)Math.Round(cell.Width, 2);
                        continue; // Skip for cells with "Continue" option
                    }
                    if ((row.RowFormat.RowDescriptor != null && row.RowFormat.RowDescriptor.IsTableHeader) || row.IsHeader)
                        m_writer.WriteStartElement("th");
                    else
                        m_writer.WriteStartElement("td");
                    float cellWidth = WriteCellAttributes(cell);
                    string tdStyle = GetStyle(cell.CellFormat);
                    if (cell.CellFormat.SamePaddingsAsTable)
                        tdStyle += GetPaddings(cell);
                    else
                        tdStyle += GetPaddings(cell.CellFormat.Paddings);                   
                    if (cellWidth > 0)
                        tdStyle += "width:" + cellWidth.ToString(CultureInfo.InvariantCulture) + "px;";
                    if (tdStyle.Length > 0)
                    {
                        m_writer.WriteAttributeString("style", tdStyle);
                    }

                    WriteSpanAttributes(colOffsets, currOffset, cell);

                    if (cell.Items.Count == 0)
                    {
                        m_writer.WriteStartElement("p");
                        WriteEmptyPara(cell.CharacterFormat);
                        m_writer.WriteEndElement();
                    }

                    foreach (TextBodyItem bodyItem in cell.Items)
                    {
                        WriteBodyItem(bodyItem);
                    }
                    CloseNestedList(-1);

                    m_writer.WriteEndElement();
                    currOffset += (float)Math.Round(cell.Width, 2);
                }
                gridCount = row.RowFormat.GridAfter;
                if (gridCount > 0)
                {
                    WriteGridCell(gridCount, row.RowFormat.GridAfterWidth);
                }
                m_writer.WriteEndElement();
            }
            // Writes row with offsets.
            WriteOffsetsRow(colOffsets);

            m_writer.WriteEndElement();

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Write grid before cell
        /// </summary>
        /// <param name="row"></param>
        private void WriteGridCell(int cellCount, PreferredWidthInfo gridWidth)
        {
            for (int i = 0; i < cellCount; i++)
            {
                m_writer.WriteStartElement("td");
                m_writer.WriteAttributeString("style", "border:none;");
                if (gridWidth.WidthType == FtsWidth.Percentage)
                    m_writer.WriteAttributeString("width", XmlConvert.ToString(gridWidth.Width) + "%");
                else if (gridWidth.WidthType == FtsWidth.Point)
                    m_writer.WriteAttributeString("width", XmlConvert.ToString(gridWidth.Width));
                m_writer.WriteRaw("&nbsp;");
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Apply default table grid style for the table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="isTableCreatedFromTextBox"></param>
        private void ApplyTableGridStyle(WTable table, bool isTableCreatedFromTextBox)
        {
            if (!isTableCreatedFromTextBox && (table.StyleName == null || table.StyleName == string.Empty))
            {
                switch (table.Document.ActualFormatType)
                {
                    case FormatType.Doc:
                        if (table.TableFormat.Sprms == null)
                            table.ApplyStyle(BuiltinTableStyle.TableGrid);
                        break;
                    case FormatType.Docx:
                    case FormatType.Word2007:
                    case FormatType.Word2010:
                        if (!table.DocxTableFormat.HasFormat)
                            table.ApplyStyle(BuiltinTableStyle.TableGrid);
                        break;
                }
            }
        }
        /// <summary>
        /// Get the bottom border for the Vertically merged cell
        /// </summary>
        /// <param name="cell"></param>
        private Border GetBottomBorderOfVerticallyMergedCell(WTableCell cell)
        {
            Border bottomBorder = cell.CellFormat.Borders.Bottom;
            WTableRow ownerRow = cell.OwnerRow;
            WTable ownerTable = ownerRow.OwnerTable;

            int rowIndex = ownerTable.Rows.IndexOf(ownerRow as Entity);
            int cellIndex = ownerRow.Cells.IndexOf(cell as Entity);

            for (int i = rowIndex; i < ownerTable.Rows.Count; i++)
            {
                if (cellIndex < ownerTable.Rows[i].Cells.Count)
                {
                    if (ownerTable.Rows[i].Cells[cellIndex].CellFormat.VerticalMerge == CellMerge.Continue)
                    {
                        bottomBorder = ownerTable.Rows[i].Cells[cellIndex].CellFormat.Borders.Bottom;
                    }
                }
            }
            return bottomBorder;
        }
       /// <summary>
       /// Get the right border for horizontally merged cell
       /// </summary>
       /// <param name="cell"></param>
        private Border GetRightBorderOfHorizontallyMergedCell(WTableCell cell)
        {
            Border rightBorder = cell.CellFormat.Borders.Right;
            WTableRow ownerRow = cell.OwnerRow;     
            int cellIndex = ownerRow.Cells.IndexOf(cell as Entity);

            for (int i = cellIndex ; i < ownerRow .Cells .Count; i++)
            {
                if (ownerRow.Cells[i].CellFormat.HorizontalMerge  == CellMerge.Continue)
                {
                    rightBorder =ownerRow .Cells [i].CellFormat .Borders .Right ;
                }
            }
            return rightBorder ;
        }
        /// <summary>
        /// Writes the row of offsets.
        /// </summary>
        /// <param name="offsets">The offsets.</param>
        private void WriteOffsetsRow(List<float> offsets)
        {
            if (offsets.Count == 0)
                return;

            float width = 0;
            m_writer.WriteStartElement("tr");
            m_writer.WriteAttributeString("style", string.Format("{0}:{1}px;", "height", "0"));

            for (int i = 0, cnt = offsets.Count; i < cnt; i++)
            {
                m_writer.WriteStartElement("td");

                if (i == 0)
                    width = offsets[i];
                else
                    width = offsets[i] - offsets[i - 1];
                //The Offset row is created to render the misaligned cells correctly. Offset row cell need not to be visible and hence setting the border as none and padding as 0
                string styleAttr = string.Format("{0}:{1}px;", "width", width) + "border:none;" + "padding:0pt;";
                m_writer.WriteAttributeString("style", styleAttr);
                //m_writer.WriteAttributeString( "style", "border: none" );

                m_writer.WriteEndElement();
            }

            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Writes the span attributes.
        /// </summary>
        /// <param name="colOffsets">The col offsets.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="cell">The cell.</param>
        private void WriteSpanAttributes(List<float> colOffsets, float rowOffset, WTableCell cell)
        {
            int colspan = 1;
            int rowspan = 1;

            if (cell.CellFormat.HorizontalMerge == CellMerge.Start)
            {
                colspan = GetColspan(cell);
            }
            else
            {
                rowOffset = (float)Math.Round(rowOffset, 2);
                colspan = GetColspan(colOffsets, rowOffset, cell.Width);
            }
            if (colspan > 1)
            {
                m_writer.WriteAttributeString("colspan", colspan.ToString());
                colspan = 1;
            }

            if (cell.CellFormat.VerticalMerge == CellMerge.Start)
            {
                rowspan = GetRowspan(cell, rowOffset);
                m_writer.WriteAttributeString("rowspan", rowspan.ToString());
            }
        }
        /// <summary>
        /// Gets the rowspan.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        private int GetRowspan(WTableCell cell, float rowOffset)
        {
            int rowspan = 1;
            WTableRow row = cell.OwnerRow;
            WTable table = row.OwnerTable;
            int cellIndex = cell.GetCellIndex();
            int rowIndex = row.GetRowIndex();
            short gridCount = row.RowFormat.GridBefore;
            if (gridCount > 0)
            {
                //Reset the offset value based on Grid before width
                if (row.RowFormat.GridBeforeWidth.WidthType == FtsWidth.Point)
                    rowOffset = rowOffset + row.RowFormat.GridBeforeWidth.Width;
                else if (row.RowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage)
                {
                    //TODO: The following offset value is calculated based on table layout specification. 
                    //The offset calculated is wrong. Need to analyze more on this offset preservation
                    float value = (cell.OwnerRow.OwnerTable.OwnerTextBody.Owner as WSection).PageSetup.ClientWidth * (row.RowFormat.GridBeforeWidth.Width / 100);
                    rowOffset = rowOffset + value;
                }
            }
            for (int i = rowIndex + 1, len = table.Rows.Count; i < len; i++)
            {
                WTableCell cell2 = GetCellByOffset(table.Rows[i], rowOffset);
                if (cell2 == null || (cell2 != null && cell.Width != cell2.Width))
                    break;

                if (cell2.CellFormat.VerticalMerge == CellMerge.Continue)
                    rowspan++;
                else
                    break;
            }

            return rowspan;
        }
        /// <summary>
        /// Gets the cell by offset.
        /// </summary>
        /// <param name="wTableRow">The w table row.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <returns></returns>
        private WTableCell GetCellByOffset(WTableRow row, float rowOffset)
        {
            float offset = 0f;
            if (row.RowFormat.GridBefore > 0)
            {
                //Resets the offset value if the current row has grid before width
                if (row.RowFormat.GridBeforeWidth.WidthType == FtsWidth.Point)
                    offset = (float)row.RowFormat.GridBeforeWidth.Width;
                else if (row.RowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage)
                {
                    //TODO: The following offset value is calculated based on table layout specification. 
                    //The offset calculated is wrong. Need to analyze more on this offset preservation
                    float width = (row.OwnerTable.OwnerTextBody.Owner as WSection).PageSetup.ClientWidth;
                    offset = width * (float)((float)row.RowFormat.GridBeforeWidth.Width / 100);
                }
            }
            for (int i = 0, len = row.Cells.Count; i < len; i++)
            {
                if ((float)Math.Round(offset, 2) == rowOffset)
                {
                    return row.Cells[i];
                }

                offset += (float)Math.Round(row.Cells[i].Width, 2);
            }

            return null;
            //throw new ArgumentException( "Invalid rowOffset" );
        }
        /// <summary>
        /// Calculates the columns.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        private List<float> CalculateOffsets(WTable table)
        {
            List<float> offsets = new List<float>();
            List<float> tableGrid = table.TableGrid;
            for (int i = 0, len = table.Rows.Count; i < len; i++)
            {
                WTableRow row = table.Rows[i];
                float rowOffset = 0;

                //Debug.WriteLine( "=table=" );
                //Debug.Write( "[" );

                for (int j = 0, lenj = row.Cells.Count; j < lenj; j++)
                {
                    WTableCell cell = row.Cells[j];
                    rowOffset = (float)Math.Round(rowOffset + cell.Width, 2);

                    //Debug.Write( rowOffset.ToString( "F4" ) +  " | " );

                    if (!offsets.Contains(rowOffset))
                    {
                        offsets.Add(rowOffset);
                    }
                }

                //Debug.WriteLine(" ]");
            }

            offsets.Sort();

            //Debug.WriteLine( "=offsets=" );
            foreach (float of in offsets)
            {
                //Debug.Write(of.ToString("F4",CultureInfo.InvariantCulture) + " , ");
            }

            return offsets;
        }
        /// <summary>
        /// Gets the colspan.
        /// </summary>
        /// <param name="colOffsets">The col offsets.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private int GetColspan(List<float> colOffsets, float startOffset, float colWidth)
        {
            //int startIndex = colOffsets.BinarySearch( startOffset, FloatApproxComparer.Instance );
            int startIndex = colOffsets.IndexOf(startOffset);
            if (startIndex < 0 && startOffset > 0)
                throw new InvalidOperationException();
            int colspan = 1;
            float endOffset = startOffset + colWidth;

            if (colOffsets.Count > startIndex + colspan)
            {
                while (endOffset - colOffsets[startIndex + colspan] > 0.01f)
                {
                    colspan++;
                }
            }

            return colspan;
        }
        /// <summary>
        /// Gets the colspan.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        private int GetColspan(WTableCell cell)
        {
            int colspan = 1;
            int cellIndex = cell.GetIndexInOwnerCollection();
            WTableRow row = cell.OwnerRow;

            for (int i = cellIndex + 1, len = row.Cells.Count; i < len; i++)
            {
                if (row.Cells[i].CellFormat.HorizontalMerge == CellMerge.Continue)
                {
                    colspan++;
                }
            }

            return colspan;
        }
        /// <summary>
        /// Writes the cell attributes.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private float WriteCellAttributes(WTableCell cell)
        {
            WTableRow row = cell.OwnerRow;
            int index = row.Cells.IndexOf(cell);
            float width = cell.Width;
            for (int i = index + 1; i < row.Cells.Count; i++)
            {
                if (row.Cells[i].CellFormat.HorizontalMerge == CellMerge.Continue)
                    width += row.Cells[i].Width;
                else
                    break;
            }
            if (width > 0)
            {
                width = UnitsConvertor.Instance.ConvertToPixels(width, PrintUnits.Point);
                //m_writer.WriteAttributeString("width", XmlConvert.ToString(pixelsWidth));

            }
            return width;
        }
        /// <summary>
        /// Writes the table attributes.
        /// </summary>
        /// <param name="table">The table.</param>
        private void WriteTableAttributes(WTable table)
        {
            StringBuilder sb = new StringBuilder();
            float value;
            if (table.TableFormat.CellSpacing >= 0)
            {
                string borderStyle = GetBordersStyle(table.TableFormat.Borders, sb, true);
                if (IsBorderAttributeNeedToPreserve (table ,borderStyle))
                    m_writer.WriteAttributeString("border", "1");
            }

            if (!string.IsNullOrEmpty(table.Title))
                m_writer.WriteAttributeString("title", table.Title);
            if (table.TableFormat.HasValue(RowFormat.ShadingColorKey) && table .TableFormat .BackColor != Color .Empty)
                sb.Append("background-color:" + GetColor(table.TableFormat.BackColor) + ";");

            
            if (table.IndentFromLeft != 0)
                sb.Append("margin-left:" + Convert.ToString(table.IndentFromLeft) + "pt;");

            sb.Append(WriteTableWidth(table));
            WriteTableAlignment(table);
            sb.Append(WriteTableCellSpacing(table));
            if (table.TableFormat.CellSpacing >= 0)
              WriteTableBorder(table,sb);

            if (sb.ToString() != string.Empty)
            {
                m_writer.WriteAttributeString("style", sb.ToString());
            }
        }
        /// <summary>
        /// Check whether the border attribute need to preserve
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private bool IsBorderAttributeNeedToPreserve(WTable table, string borderStyle)
        {
            if (borderStyle == string.Empty
                && !table.TableFormat.Borders.Bottom.HasNoneStyle
                && !table.TableFormat.Borders.Right.HasNoneStyle
                && !table.TableFormat.Borders.Left.HasNoneStyle
                && !table.TableFormat.Borders.Top.HasNoneStyle
                && table.TableFormat.Borders.Top.BorderType != BorderStyle.Cleared
                && table.TableFormat.Borders.Left.BorderType != BorderStyle.Cleared
                && table.TableFormat.Borders.Bottom.BorderType != BorderStyle.Cleared
                && table.TableFormat.Borders.Right.BorderType != BorderStyle.Cleared)
                return true;
            else
                return false;

        }
        /// <summary>
        /// Write table border
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private void WriteTableBorder(WTable table, StringBuilder sb)
        {
            GetTableborder(table.TableFormat.Borders.Bottom, "bottom", sb);
            GetTableborder(table.TableFormat.Borders.Top, "bottom", sb);
            GetTableborder(table.TableFormat.Borders.Left, "bottom", sb);
            GetTableborder(table.TableFormat.Borders.Right, "bottom", sb);
        }
        /// <summary>
        /// Get table border style
        /// </summary>
        /// <param name="border"></param>
        /// <param name="suffix"></param>
        /// <param name="sb"></param>
        private void GetTableborder(Border border, string suffix, StringBuilder sb)
        {
            if (border.BorderType == BorderStyle.Cleared)
                return;
            if (border.BorderType != BorderStyle.None)
                sb.Append("border-" + suffix + "-style:" + ToBorderStyle(border.BorderType) + ";");

            if (border.LineWidth > 0)
                sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(border.LineWidth) + "pt;");

            if (border.Color != Color.Empty)
                sb.Append("border-" + suffix + "-color" + GetColor(border.Color) + ";");
            else if(border .BorderType != BorderStyle .None )
                sb.Append("border-" + suffix + "-color:" + "#000000" + ";");


        }
        /// <summary>
        /// Write table cell spacing
        /// </summary>
        /// <param name="table"></param>
        private string WriteTableCellSpacing(WTable table)
        {
            StringBuilder sb = new StringBuilder();
            if (table.TableFormat.CellSpacing > 0)
            {
                // Cell spacing must be in pixels
                m_writer.WriteAttributeString("cellspacing", XmlConvert.ToString(UnitsConvertor.Instance.ConvertToPixels(table.TableFormat.CellSpacing * 2, PrintUnits.Point)));// + "pt");
            }
            else
            {
                m_writer.WriteAttributeString("cellspacing", "0");
            }

            if (table.TableFormat.CellSpacing <= 0)
            {
                sb.Append("border-collapse: collapse; ");
            }
            return sb.ToString();

        }
        /// <summary>
        /// Write table width
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private string WriteTableWidth(WTable table)
        {
            float value;
            StringBuilder sb = new StringBuilder();
            switch (table.PreferredTableWidth.WidthType)
            {
                case FtsWidth.Percentage:
                    sb.Append("width: " + Convert.ToString(table.PreferredTableWidth.Width) + "%; ");
                    break;
                case FtsWidth.Point:
                    sb.Append("width: " + Convert.ToString(table.PreferredTableWidth.Width) + "pt; ");
                    break;
                case FtsWidth.Auto:
                    sb.Append("width: auto; ");
                    break;
            }            
            return sb.ToString();
        }
        /// <summary>
        /// Write table alignment
        /// </summary>
        /// <param name="table"></param>
        private void WriteTableAlignment(WTable table)
        {
            switch (table.TableFormat.HorizontalAlignment)
            {
                case RowAlignment.Center:
                    m_writer.WriteAttributeString("align", "center");
                    break;
                case RowAlignment.Right:
                    m_writer.WriteAttributeString("align", "right");
                    break;
            }
        }
        /// <summary>
        /// Writes the row attributes.
        /// </summary>
        /// <param name="row">The row.</param>
        private void WriteRowAttributes(WTableRow row)
        {
            string height = null;
            row.Height = Math.Abs(row.Height);
            if (row.Height > 0)
                height = XmlConvert.ToString(UnitsConvertor.Instance.ConvertToPixels(row.Height, PrintUnits.Point)) + "px";
            else
                height = "2px";

            m_writer.WriteAttributeString("style", "height: " + height);
            if (row.RowFormat.HasValue(RowFormat.HiddenKey))
                m_writer.WriteAttributeString("style", "display:", "none");

        }
        #endregion

        #region Implementation / helper
        /// <summary>
        /// Gets the vertical alignment.
        /// </summary>
        /// <param name="shapeAlign">The shape align.</param>
        /// <returns></returns>
        private VerticalAlignment GetVerAlign(ShapeVerticalAlignment shapeAlign)
        {
            switch (shapeAlign)
            {
                case ShapeVerticalAlignment.Center:
                    return VerticalAlignment.Middle;
                case ShapeVerticalAlignment.Bottom:
                    return VerticalAlignment.Bottom;
                default:
                    return VerticalAlignment.Top;
            }
        }
        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        /// <param name="shapeAlign">The shape align.</param>
        /// <returns></returns>
        private RowAlignment GetHorAlign(ShapeHorizontalAlignment shapeAlign)
        {
            switch (shapeAlign)
            {
                case ShapeHorizontalAlignment.Center:
                    return RowAlignment.Center;
                case ShapeHorizontalAlignment.Right:
                    return RowAlignment.Right;
                default:
                    return RowAlignment.Left;
            }
        }
        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <param name="lineStyle">The line style.</param>
        /// <returns></returns>
        private BorderStyle GetBordersStyle(TextBoxLineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case TextBoxLineStyle.Simple:
                    return BorderStyle.Single;
                case TextBoxLineStyle.Double:
                    return BorderStyle.Double;
                case TextBoxLineStyle.ThickThin:
                    return BorderStyle.ThickThinMediumGap;
                case TextBoxLineStyle.ThinThick:
                    return BorderStyle.ThinThickMediumGap;
                case TextBoxLineStyle.Triple:
                    return BorderStyle.Triple;
                default:
                    return BorderStyle.None;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Ensures the images folder.
        /// </summary>
        private void EnsureImagesFolder()
        {
            if (!m_bImagesFolderCreated && !m_cacheFilesInternally)
            {
                Directory.CreateDirectory(m_imagesFolder + m_fileNameWithoutExt + "_images\\");
                m_bImagesFolderCreated = true;
            }
        }
#endif
        /// <summary>
        /// Gets the name of the image file.
        /// </summary>
        /// <returns></returns>
        private string GetImagePath()
        {
            m_imgCounter++;
            string imageName;
            if (!m_cacheFilesInternally)
                imageName = m_fileNameWithoutExt + "_images\\" + m_fileNameWithoutExt + "_img" + m_imgCounter.ToString();
            else
                imageName = "images/img" + m_imgCounter.ToString();
            return imageName;
        }
        /// <summary>
        /// Gets the paddings.
        /// </summary>
        /// <param name="paddings">The paddings.</param>
        /// <returns></returns>
        private string GetPaddings(Paddings paddings)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("padding-left:" + XmlConvert.ToString(paddings.Left) + "pt;");
            sb.Append("padding-right:" + XmlConvert.ToString(paddings.Right) + "pt;");
            sb.Append("padding-top:" + XmlConvert.ToString(paddings.Top) + "pt;");
            sb.Append("padding-bottom:" + XmlConvert.ToString(paddings.Bottom) + "pt;");

            return sb.ToString();
        }
        /// <summary>
        /// Gets the paddings.
        /// </summary>
        /// <param name="paddings">The paddings.</param>
        /// <returns></returns>
        private string GetPaddings(WTableCell cell)
        {
            StringBuilder sb = new StringBuilder();

            Paddings paddings = GetCellPaddingBasedOnTable(cell);

            sb.Append("padding-left:" + XmlConvert.ToString(paddings.Left) + "pt;");
            sb.Append("padding-right:" + XmlConvert.ToString(paddings.Right) + "pt;");
            sb.Append("padding-top:" + XmlConvert.ToString(paddings.Top) + "pt;");
            sb.Append("padding-bottom:" + XmlConvert.ToString(paddings.Bottom) + "pt;");

            return sb.ToString();
        }
        /// <summary>
        /// Get cell padding from table
        /// </summary>
        /// <param name="cell"></param>
        private Paddings GetCellPaddingBasedOnTable(WTableCell cell)
        {
            Paddings paddings = new Paddings();
            //Left
            if (cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
            {
                paddings.Left = cell.OwnerRow.RowFormat.Paddings.Left;
            }
            else if (cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.LeftKey))
            {
                paddings.Left = cell.OwnerRow.OwnerTable.TableFormat.Paddings.Left;
            }
            //Doc format document can have default cell margin value as zero.
            else if (cell.Document.ActualFormatType == FormatType.Doc)
                paddings.Left = 0.0f;
            else
                paddings.Left = 5.4f;
            //Right
            if (cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
            {
                paddings.Right = cell.OwnerRow.RowFormat.Paddings.Right;
            }
            else if (cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.RightKey))
            {
                paddings.Right = cell.OwnerRow.OwnerTable.TableFormat.Paddings.Right;
            }
            //Doc format document can have default cell margin value as zero.
            else if (cell.Document.ActualFormatType == FormatType.Doc)
                paddings.Right = 0.0f;
            else
                paddings.Right = 5.4f;
            //Top
            if (cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
            {
                paddings.Top = cell.OwnerRow.RowFormat.Paddings.Top;
            }
            else if (cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.TopKey))
            {
                paddings.Top = cell.OwnerRow.OwnerTable.TableFormat.Paddings.Top;
            }
            else
                paddings.Top = 0.0f;
            //Bottom
            if (cell.OwnerRow.RowFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
            {
                paddings.Bottom = cell.OwnerRow.RowFormat.Paddings.Bottom;
            }
            else if (cell.OwnerRow.OwnerTable.TableFormat.Paddings.HasKey(Syncfusion.DocIO.DLS.Paddings.BottomKey))
            {
                paddings.Bottom = cell.OwnerRow.OwnerTable.TableFormat.Paddings.Bottom;
            }
            else
                paddings.Bottom = 0.0f;

            return paddings;
        }
        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private string GetStyle(RowFormat format, WTable table)
        {
            StringBuilder sb = new StringBuilder();

            if (table.IndentFromLeft > 0)
            {
                sb.Append("margin-left:" + XmlConvert.ToString(table.IndentFromLeft) + "pt;");
            }

            if (table.TableFormat.CellSpacing < 0)
            {
                sb.Append("border-collapse: collapse;");
            }

            if (format.HasValue(CellFormat.ShadingColorKey) && format.BackColor != Color .Empty)
            {
                sb.Append("background-color: " + GetColor(format.BackColor) + ";");
            }

            GetBordersStyle(format.Borders, sb, true);
            SetDefBorders(format.Borders, format.Borders, sb);
            return sb.ToString();
        }
        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private string GetStyle(CellFormat format)
        {
            WTableCell cell = format.OwnerBase as WTableCell;
            StringBuilder sb = new StringBuilder();

            //if( format.VerticalAlignment != VerticalAlignment.Top )
            {
                sb.Append("vertical-align:" + format.VerticalAlignment.ToString().ToLower() + ";");
            }

            if (format.ForeColor != Color .Empty  || format.TextureStyle != TextureStyle .TextureNone  || format.BackColor != Color .Empty )
                sb.Append("background-color:" + GetCellBackground(format) + ";");

            GetBordersStyle(format.Borders, format.OwnerRowFormat.Borders, sb, cell);

            //GetBordersStyle( format.Borders, sb, true );
            //SetDefBorders( format.Borders, format.OwnerRowFormat.Borders, sb );
            return sb.ToString();
        }
        /// <summary>
        /// Checks with Texture and returns the Background color of the cell
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string GetCellBackground(CellFormat format)
        {
            float percent = Build_TextureStyle(format.TextureStyle);
            int r = 0, g = 0, b = 0;
            r = GetColorValue(format.ForeColor.R, format.BackColor.R, percent,format .ForeColor .IsEmpty ,format .BackColor .IsEmpty);
            g = GetColorValue(format.ForeColor.G, format.BackColor.G, percent, format.ForeColor.IsEmpty, format.BackColor.IsEmpty);
            b = GetColorValue(format.ForeColor.B, format.BackColor.B, percent, format.ForeColor.IsEmpty, format.BackColor.IsEmpty);
            Color backgroundColor = Color.FromArgb(r, g, b);
            string s = GetColor(backgroundColor);
            return s;
           
        }
        /// <summary>
        /// Gets the color value.
        /// </summary>
        /// <param name="foreColorValue">The fore color value.</param>
        /// <param name="backColorValue">The back color value.</param>
        /// <param name="percent">The percent.</param>
        /// <returns></returns>
        private int GetColorValue(int foreColorValue, int backColorValue, float percent, bool isForeColorEmpty, bool isBackColorEmpty)
        {
            int colorValue = 0;

            if (percent == 100)
            {
                colorValue = foreColorValue;
            }
            else
            {
                if (isForeColorEmpty)
                {
                    if (isBackColorEmpty)
                        colorValue = (int)Math.Round(255 * (1 - percent / 100));
                    else
                        colorValue = (int)Math.Round(backColorValue * (1 - percent / 100));
                }
                else
                {
                    if (isBackColorEmpty)
                        colorValue = (int)Math.Round(foreColorValue * (percent / 100));
                    else
                        colorValue = backColorValue + (int)Math.Round(foreColorValue * (percent / 100)) - (int)Math.Round(backColorValue * (percent / 100));
                }
            }
            return colorValue;
        }
        /// <summary>
        /// Gets the borders style.
        /// </summary>
        /// <param name="cellBorders">The cell borders.</param>
        /// <param name="rowBorders">The row borders.</param>
        /// <param name="sb">The string builder.</param>
        /// <param name="ownerCell">The owner cell.</param>
        private void GetBordersStyle(Borders cellBorders, Borders rowBorders, StringBuilder sb, WTableCell ownerCell)
        {
            Border rowBorder = GetRowBorder(rowBorders, ownerCell, "top" );
            GetBorderStyle(cellBorders.Top, rowBorder, sb, "top", ownerCell);

            rowBorder = GetRowBorder(rowBorders, ownerCell, "left");
            GetBorderStyle(cellBorders.Left, rowBorder, sb, "left", ownerCell);

            rowBorder = GetRowBorder(rowBorders, ownerCell, "right");
            if (ownerCell.CellFormat.HorizontalMerge == CellMerge.Start)
                GetBorderStyle(GetRightBorderOfHorizontallyMergedCell(ownerCell), rowBorder, sb, "right",ownerCell );
            else
                GetBorderStyle(cellBorders.Right, rowBorder, sb, "right", ownerCell);

            rowBorder = GetRowBorder(rowBorders, ownerCell, "bottom");
            if (ownerCell.CellFormat.VerticalMerge == CellMerge.Start)
                GetBorderStyle(GetBottomBorderOfVerticallyMergedCell(ownerCell), rowBorder, sb, "bottom", ownerCell);
            else
                GetBorderStyle(cellBorders.Bottom, rowBorder, sb, "bottom", ownerCell);
        }
        /// <summary>
        /// Gets the row border.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        private Border GetRowBorder(Borders borders, WTableCell cell, string side)
        {
            WTableRow row = null;
            switch (side)
            {
                case "top":
                    row = cell.OwnerRow;
                    if (row != null && row.GetIndexInOwnerCollection() > 0)
                        return borders.Horizontal;
                    else
                        return borders.Top;
                case "left":
                    if (cell.GetIndexInOwnerCollection() > 0)
                        return borders.Vertical;
                    else
                        return borders.Left;
                case "right":
                    row = cell.OwnerRow;
                    if (row != null && cell.GetIndexInOwnerCollection() == row.Cells.Count - 1)
                        return borders.Right;
                    else
                        return borders.Vertical;
                case "bottom":
                    row = cell.OwnerRow;
                    WTable table = row.OwnerTable;
                    if (cell != null && table != null && row.GetIndexInOwnerCollection() == table.Rows.Count - 1)
                        return borders.Bottom;
                    else
                        return borders.Horizontal;

            }

            return null;
        }
        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <param name="cellBorder">The cell border.</param>
        /// <param name="rowBorder">The row border.</param>
        /// <param name="sb">The string builder.</param>
        /// <param name="side">The side.</param>
        private void GetBorderStyle(Border cellBorder, Border rowBorder, StringBuilder sb, string suffix,WTableCell cell)
        {
            if (cellBorder.BorderType == BorderStyle.Cleared)
            {
                sb.Append("border-" + suffix + ":none;");
                return;
            }

            // Write border type
            BorderStyle style = (cellBorder.BorderType != BorderStyle.None && cellBorder.BorderType != BorderStyle.Cleared) ?
              cellBorder.BorderType : rowBorder.BorderType;

            if (style != BorderStyle.None && style != BorderStyle.Cleared)
                sb.Append("border-" + suffix + "-style:" + ToBorderStyle(style) + ";");
            else if (!cellBorder.HasNoneStyle)
            {
                GetCellborderStyleBasedOnTableBorder(cell, suffix, sb);
            }

            // Write border color
            Color color = (cellBorder.Color != Color.Empty) ? cellBorder.Color : rowBorder.Color;
            if (color != Color.Empty)
                sb.Append("border-" + suffix + "-color:" + GetColor(color) + ";");
            else if (!cellBorder.HasNoneStyle)
            {
                GetCellBorderColorBasedOnTableBorder(cell, suffix, sb);
            }
            else if (!rowBorder.HasNoneStyle)
            {
                if (rowBorder.BorderType != BorderStyle.None)
                    sb.Append("border-" + suffix + "-color:" + "#000000" + ";");
            }


            float lineWidth = 0;
            if (cellBorder.LineWidth > 0)
                lineWidth = GetLineWidthBasedOnBorderStyle(cellBorder);
            else if (rowBorder.LineWidth > 0)
                lineWidth = GetLineWidthBasedOnBorderStyle(rowBorder);
            if (lineWidth > 0)
            {
                sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(lineWidth) + "pt;");
            }
            else if (!cellBorder.HasNoneStyle )
            {
                GetCellborderWidthBasedOnTableBorder(cell, suffix, sb);
            }
        }
        /// <summary>
        /// Get table border style based on table border
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="suffix"></param>
        /// <param name="sb"></param>
        private void GetCellborderStyleBasedOnTableBorder(WTableCell cell, string suffix, StringBuilder sb)
        {

            WTableRow ownerRow = cell.OwnerRow;
            WTable ownerTable = ownerRow.OwnerTable;
            int cellIndex = cell.GetIndexInOwnerCollection();
            int rowIndex = ownerRow.GetIndexInOwnerCollection();

            switch (suffix)
            {
                case "top":
                    if (rowIndex == 0 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if (ownerTable.TableFormat.Borders.Top.BorderType != BorderStyle.None)
                            sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Top.BorderType) + ";");
                    }
                    else if (ownerTable.TableFormat.Borders.Horizontal.BorderType != BorderStyle.None)
                        sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Horizontal.BorderType) + ";");
                   
                    break;
                case "bottom":
                    if (rowIndex == ownerTable.Rows.Count - 1 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if (ownerTable.TableFormat.Borders.Bottom.BorderType != BorderStyle.None)
                            sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Bottom.BorderType) + ";");
                    }
                    else if (ownerTable.TableFormat.Borders.Horizontal.BorderType != BorderStyle.None)
                        sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Horizontal.BorderType) + ";");
                    break;
                case "left":
                    if (cellIndex == 0 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if ( ownerTable.TableFormat.Borders.Left.BorderType != BorderStyle.None)
                            sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Left.BorderType) + ";");
                    }
                    else if (ownerTable.TableFormat.Borders.Vertical.BorderType != BorderStyle.None)
                        sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Vertical.BorderType) + ";");
                    break;
                case "right":
                    if (cellIndex == ownerRow.Cells.Count - 1 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if (ownerTable.TableFormat.Borders.Right.BorderType != BorderStyle.None)
                            sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Right.BorderType) + ";");
                    }
                    else if (ownerTable.TableFormat.Borders.Vertical.BorderType != BorderStyle.None)
                        sb.Append("border-" + suffix + "-style:" + ToBorderStyle(ownerTable.TableFormat.Borders.Vertical.BorderType) + ";");
                    break;
            }
        }
        /// <summary>
        /// Get Cell border color based on table border
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="suffix"></param>
        /// <param name="sb"></param>
        private void GetCellBorderColorBasedOnTableBorder(WTableCell cell, string suffix, StringBuilder sb)
        {
            WTableRow ownerRow = cell.OwnerRow;
            WTable ownerTable = ownerRow.OwnerTable;
            int cellIndex = cell.GetIndexInOwnerCollection();
            int rowIndex = ownerRow.GetIndexInOwnerCollection();

            switch (suffix)
            {
                case "top":
                    if (rowIndex == 0 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                         GetBorderColor(ownerTable.TableFormat.Borders.Top, sb, suffix);
                    }
                    else
                        GetBorderColor(ownerTable.TableFormat.Borders.Horizontal, sb, suffix);
                    break;
                case "bottom":
                    if (rowIndex == ownerTable.Rows.Count - 1 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        GetBorderColor(ownerTable.TableFormat.Borders.Bottom, sb, suffix);                       
                    }
                    else
                        GetBorderColor(ownerTable.TableFormat.Borders.Horizontal, sb, suffix);
                    break;
                case "left":
                    if (cellIndex == 0 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        GetBorderColor(ownerTable.TableFormat.Borders.Left, sb, suffix);
                    }
                    else
                        GetBorderColor(ownerTable.TableFormat.Borders.Vertical, sb, suffix);
                    break;
                case "right":
                    if (cellIndex == ownerRow.Cells.Count - 1 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        GetBorderColor(ownerTable.TableFormat.Borders.Right, sb, suffix);
                    }
                    else
                        GetBorderColor(ownerTable.TableFormat.Borders.Vertical, sb, suffix);
                    break;
            }
        }
        /// <summary>s
        /// Get border color
        /// </summary>
        /// <param name="border"></param>
        /// <param name="sb"></param>
        /// <param name="suffix"></param>
        private void GetBorderColor(Border border,StringBuilder  sb, string suffix)
        {
            if (border.Color != Color.Empty)
                sb.Append("border-" + suffix + "-color:" + GetColor(border.Color) + ";");
            else if (border.BorderType != BorderStyle.None)
                sb.Append("border-" + suffix + "-color:" + "#000000" + ";");
        }
        /// <summary>
        /// Get Cell border width based on table border
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="suffix"></param>
        /// <param name="sb"></param>
        private void GetCellborderWidthBasedOnTableBorder(WTableCell cell, string suffix, StringBuilder sb)
        {

            WTableRow ownerRow = cell.OwnerRow;
            WTable ownerTable = ownerRow.OwnerTable;
            int cellIndex = cell.GetIndexInOwnerCollection();
            int rowIndex = ownerRow.GetIndexInOwnerCollection();

            switch (suffix)
            {
                case "top":
                    if (rowIndex == 0 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if ( ownerTable.TableFormat.Borders.Top.LineWidth > 0)
                            sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Top)) + "pt;");
                    }
                    else if (ownerTable.TableFormat.Borders.Horizontal.LineWidth > 0)
                        sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Horizontal)) + "pt;");
                    break;
                case "bottom":
                    if (rowIndex == ownerTable.Rows.Count - 1 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if ( ownerTable.TableFormat.Borders.Bottom.LineWidth > 0)
                            sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Bottom)) + "pt;");
                    }
                    else if (ownerTable.TableFormat.Borders.Horizontal.LineWidth > 0)
                        sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Horizontal)) + "pt;");
                    break;
                case "left":
                    if (cellIndex == 0 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if (ownerTable.TableFormat.Borders.Left.LineWidth > 0)
                            sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Left)) + "pt;");
                    }
                    else if (ownerTable.TableFormat.Borders.Vertical.LineWidth > 0)
                        sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Vertical)) + "pt;");
                    break;
                case "right":
                    if (cellIndex == ownerRow.Cells.Count - 1 && ownerTable.TableFormat.CellSpacing <= 0)
                    {
                        if (ownerTable.TableFormat.Borders.Right.LineWidth > 0)
                            sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Right)) + "pt;");
                    }
                    else if (ownerTable.TableFormat.Borders.Vertical.LineWidth > 0)
                        sb.Append("border-" + suffix + "-width:" + XmlConvert.ToString(GetLineWidthBasedOnBorderStyle(ownerTable.TableFormat.Borders.Vertical)) + "pt;");
                    break;
            }
        }
        /// <summary>
        /// Get line width based on border style
        /// </summary>
        /// <param name="border"></param>
        /// <returns></returns>
        private float GetLineWidthBasedOnBorderStyle(Border border)
        {  
            //TODO: The following line width calculation is made based on behavioral analysis of Word automation Doc to Html conversion
            switch (border.BorderType)
            {
                case BorderStyle.Triple:
                    return (border.LineWidth * 5);
                case BorderStyle.ThinThickSmallGap:
                case BorderStyle.ThinThinSmallGap:
                    return (float)(border.LineWidth + 1.5);
                case BorderStyle.ThinThickThinSmallGap:
                    return (float)(border.LineWidth + 3);
                case BorderStyle.ThinThickMediumGap:
                case BorderStyle.ThickThinMediumGap:
                    return (border.LineWidth * 2);
                case BorderStyle.ThickThickThinMediumGap:
                    return (border.LineWidth * 3);
                case BorderStyle.ThinThickLargeGap:
                    return (float)(border.LineWidth + 2.25);
                case BorderStyle.ThickThinLargeGap:
                case BorderStyle.ThinThickThinLargeGap:
                    return (float)((border.LineWidth * 2) + 3);
                case BorderStyle.DoubleWave:
                case BorderStyle.Double:
                    if (border.LineWidth <= 0.5f)
                        return 1.5f;
                    else
                        return (border.LineWidth * 3);
                    break;
                default :
                    if (border.LineWidth < 1)
                        return 1f;
                    else
                        return border.LineWidth;
                    break;
            }
        }
        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private string GetStyle(WParagraphFormat format, bool isListLevel, bool isListAsPara, WListFormat listFormat)
        {
            StringBuilder sb = new StringBuilder();
          
            sb.Append("text-align:" + format.HorizontalAlignment.ToString().ToLower() + ";");

            if (format.Keep)
                sb.Append("page-break-inside:avoid;");
            else
                sb.Append("page-break-inside:auto;");

            if (format.KeepFollow)
                sb.Append("page-break-after:avoid;");
            else
                sb.Append("page-break-after:auto;");

            if (format.PageBreakBefore)
                sb.Append("page-break-before:always;");
            else
                sb.Append("page-break-before:auto;");

            if (format.BackColor != Color.Empty)
                sb.Append("background-color:" + GetColor(format.BackColor) + ";");

            if (Math.Abs(format.LineSpacing) > 0 && CheckParentFormat(format))
            {
                //Write Default linespacing
                if (Math.Abs(format.LineSpacing) == 12 && format.LineSpacingRule == LineSpacingRule.Multiple)
                    sb.Append("line-height:" + "normal;");
                else if (format.LineSpacingRule == LineSpacingRule.Multiple)
                    sb.Append("line-height:" + XmlConvert.ToString((Math.Abs(format.LineSpacing) / 12) * 100) + "%;");
                else
                    sb.Append("line-height:" + XmlConvert.ToString(Math.Abs(format.LineSpacing)) + "pt;");
            }

            sb.Append("margin-top:" + XmlConvert.ToString(format.BeforeSpacing) + "pt;");
            sb.Append("margin-bottom:" + XmlConvert.ToString(format.AfterSpacing) + "pt;");

            if (!format.WordWrap)
                sb.Append("word-break:break-all;");
            // Write left indent
            if (isListLevel && !isListAsPara)
            {
                float levelIndent = format.LeftIndent + format.FirstLineIndent;
                float textIndent=0;
                WListLevel level=null;

                if (levelIndent > 0)
                {
                    if (format.Tabs.Count > 0 && format.Tabs[0].Justification == TabJustification.List)
                        levelIndent = format.Tabs[0].Position - levelIndent;
                }
                if (format.OwnerBase is WParagraph)
                {
                    if (listFormat != null)
                        level = listFormat.CurrentListLevel;
                    if ((level != null && level.TextPosition + level.NumberPosition == 0) || level == null)
                        levelIndent = 0;
                    else if (level != null && (format.OwnerBase as WParagraph).ListFormat.ListType == ListType.Numbered)
                        levelIndent = GetLevelIndent(level, (format.OwnerBase as WParagraph));
                    else if (level != null && (format.OwnerBase as WParagraph).ListFormat.ListType == ListType.Bulleted)
                        levelIndent = Math.Abs(levelIndent - level.TextPosition);
                    else if (level != null)
                        levelIndent = GetLevelIndent(level, (format.OwnerBase as WParagraph));
                }

                sb.Append("margin-left:" + XmlConvert.ToString(levelIndent) + "pt;");
                sb.Append("text-indent:" + "0pt;");
              
            }
            else if (isListLevel && isListAsPara)
            {
                float levelIndent = format.LeftIndent + format.FirstLineIndent;
                float textIndent = 0;
                if (format.OwnerBase is WParagraph)
                {
                    WListLevel level = (format.OwnerBase as WParagraph).ListFormat.CurrentListLevel;
                    levelIndent = level.TextPosition;
                    textIndent = level.NumberPosition;
                }
                sb.Append("margin-left:" + XmlConvert.ToString(levelIndent) + "pt;");
                sb.Append("text-indent:" + XmlConvert.ToString(textIndent) + "pt;");
            }
            else
            {
                if (format.HasValue(WParagraphFormat.LeftIndentKey))
                {
                    sb.Append("margin-left:" + XmlConvert.ToString(format.LeftIndent) + "pt;");
                }
                if (format.HasValue(WParagraphFormat.FirstLineIndentKey))
                {
                    sb.Append("text-indent:" + XmlConvert.ToString(format.FirstLineIndent) + "pt;");
                }
            }

            if (format.HasValue(WParagraphFormat.RightIndentKey))
            {
                sb.Append("margin-right:" + XmlConvert.ToString(format.RightIndent) + "pt;");
            }
            return GetBordersStyle(format.Borders, sb, false);
        }
        /// <summary>
        /// Get level indent
        /// </summary>
        /// <param name="level"></param>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private float GetLevelIndent(WListLevel level, WParagraph paragraph)
        {
            float leftIndent = 0;
            float firstLineIndent = 0;
            WParagraphStyle pStyle = paragraph.ParaStyle as WParagraphStyle;
            // Updates Left indent
            if (level.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                leftIndent = level.ParagraphFormat.LeftIndent;
            if (paragraph.ListFormat.ListType == ListType.NoList && pStyle != null
                && pStyle.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                leftIndent = pStyle.ParagraphFormat.LeftIndent;
            if (paragraph.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                leftIndent = paragraph.ParagraphFormat.LeftIndent;

            // Updates FirstLine indent
            if (level.ParagraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                firstLineIndent = level.ParagraphFormat.FirstLineIndent;
            if (paragraph.ListFormat.ListType == ListType.NoList && pStyle != null
                && pStyle.ParagraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                firstLineIndent = pStyle.ParagraphFormat.FirstLineIndent;
            if (paragraph.ParagraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                firstLineIndent = paragraph.ParagraphFormat.FirstLineIndent;

            if (firstLineIndent < 0
                && leftIndent == 0
                && !paragraph.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                leftIndent = Math.Abs(firstLineIndent);
            return leftIndent + firstLineIndent;
        }
        /// <summary>
        /// Get character format style
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string GetStyle(WCharacterFormat format)
        {
            StringBuilder sb = new StringBuilder();

            if (format.CharacterSpacing > 0)
                sb.Append("letter-spacing:" + format.CharacterSpacing + "pt;");
            if (format.TextColor != Color.Empty)
                sb.Append("color:" + GetColor(format.TextColor) + ";");
            sb.Append("font-family:" + format.FontName + ";");
            if (format.FontSize > 0.0)
                sb.Append("font-size:" + format.FontSize.ToString(CultureInfo.InvariantCulture) + "pt;");
            if (!format.HighlightColor.IsEmpty)
                sb.Append("background-color:" + GetColor(format.HighlightColor) + ";");
            else if (!format.TextBackgroundColor.IsEmpty)
                sb.Append("background-color" + GetColor(format.TextBackgroundColor) + ";");

            if (format.AllCaps)
                sb.Append("text-transform:uppercase;");
            else
                sb.Append("text-transform:none;");

            if (format.Bold || format.Emboss || format.OutLine )
                sb.Append("font-weight:bold;");
            else
                sb.Append("font-weight:normal;");

            if (format.Hidden)
                sb.Append("display:none;");

            if (format.Italic)
                sb.Append("font-style:italic;");
            else
                sb.Append("font-style:normal;");

            if (format.SmallCaps)
                sb.Append("font-variant:small-caps;");
            else
                sb.Append("font-variant:normal;");


            if (format.SubSuperScript == SubSuperScript.SubScript)
            {
                sb.Append("vertical-align:sub;");
                sb.Append("font-size:" + XmlConvert.ToString(Math.Round(GetScriptFontSize(format) / 12, 2)) + "em;");
            }
            else if (format.SubSuperScript == SubSuperScript.SuperScript)
            {
                sb.Append("vertical-align:super;");
                sb.Append("font-size:" + XmlConvert.ToString(Math.Round(GetScriptFontSize(format) / 12, 2)) + "em;");
            }

            if ((m_currPara == null) || (m_currPara != null && m_currPara.Text != ""))
            {
                if (format.UnderlineStyle != UnderlineStyle.None)
                    sb.Append("text-decoration: underline;");
                if (format.DoubleStrike || format.Strikeout )
                    sb.Append("text-decoration: line-through;");
            }

            return sb.ToString();
        }
        /// <summary>
        /// Gets the color in Hex format
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Hex value</returns>
        private string GetColor(Color color)
        {            
            return string.Concat("#", (color.ToArgb() & 0x00FFFFFF).ToString("X6"));
        }        
        /// <summary>
        /// Builds the boolean character property.
        /// </summary>
        /// <param name="propKey">The property key.</param>
        /// <param name="trueStr">The "true" string value.</param>
        /// <param name="falseStr">The "false" string value.</param>
        /// <param name="chf">The character format.</param>
        /// <param name="sb">The string builder.</param>
        private void BuildBoolProp(short propKey, string trueStr, string falseStr,
          WCharacterFormat chf, StringBuilder sb)
        {
            bool complexProp = chf.GetComplexBoolValue(propKey);

            if (complexProp)
            {
                sb.Append(trueStr);
            }
            else if (!complexProp && chf.IsComplex(propKey))
            {
                sb.Append(falseStr);
            }
        }
        /// <summary>
        /// Gets the size of the script font.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private float GetScriptFontSize(WCharacterFormat format)
        {
            if (format.FontSize > 1f)
            {
                return (format.FontSize / 2);
            }

            return 8;
        }
        /// <summary>
        /// Gets the borders style.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <returns></returns>
        private string GetBordersStyle(Borders borders, StringBuilder sb, bool isTableBorder)
        {
            if (!(borders.ParentFormat != null
                && borders.ParentFormat is WParagraphFormat
                && (borders.ParentFormat as WParagraphFormat).OwnerBase is WParagraph
                && (borders.ParentFormat.OwnerBase as WParagraph).PreviousSibling != null
                && (borders.ParentFormat.OwnerBase as WParagraph).PreviousSibling is WParagraph
                && ((borders.ParentFormat.OwnerBase as WParagraph).PreviousSibling as WParagraph).ParagraphFormat.Borders.Top.BorderType == borders.Top.BorderType
                && ((borders.ParentFormat.OwnerBase as WParagraph).PreviousSibling as WParagraph).ParagraphFormat.Borders.Top.LineWidth == borders.Top.LineWidth))
            {
                GetBorderStyle("top", borders.Top, sb, isTableBorder);
            }
            GetBorderStyle("left", borders.Left, sb, isTableBorder);
            GetBorderStyle("right", borders.Right, sb, isTableBorder);
            if (!(borders.ParentFormat != null
                 && borders.ParentFormat is WParagraphFormat
                && (borders.ParentFormat as WParagraphFormat).OwnerBase is WParagraph
                && ((borders.ParentFormat as WParagraphFormat).OwnerBase as WParagraph).NextSibling != null
                && ((borders.ParentFormat as WParagraphFormat).OwnerBase as WParagraph).NextSibling is WParagraph
                && (((borders.ParentFormat as WParagraphFormat).OwnerBase as WParagraph).NextSibling as WParagraph).ParagraphFormat.Borders.Bottom.BorderType == borders.Bottom.BorderType
                && (((borders.ParentFormat as WParagraphFormat).OwnerBase as WParagraph).NextSibling as WParagraph).ParagraphFormat.Borders.Bottom.LineWidth == borders.Bottom.LineWidth))
            {
                GetBorderStyle("bottom", borders.Bottom, sb, isTableBorder);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="border">The border.</param>
        /// <param name="sb">The sb.</param>
        private void GetBorderStyle(string suffix, Border border, StringBuilder sb, bool isTableBorder)
        {
            if (border.BorderType == BorderStyle.Cleared)
                return;

            if (border.Color != Color.Empty)
                sb.Append("border-" + suffix + "-color:" + GetColor(border.Color) + ";");
            else if (border.BorderType != BorderStyle.None)
                sb.Append("border-" + suffix + "-color:" + "#000000" + ";");

            if (border.BorderType != BorderStyle.None)
                sb.Append("border-" + suffix + "-style:" + ToBorderStyle(border.BorderType) + ";");

            if (border.LineWidth > 0)
            {
                string width = (isTableBorder) ?
                  XmlConvert.ToString(border.LineWidth) :
                  XmlConvert.ToString((int)(border.LineWidth * 25) / 10f);

                sb.Append("border-" + suffix + "-width:" + width + "pt;");
            }

            if (border.Space > 0)
                sb.Append("padding-" + suffix + ":" + XmlConvert.ToString(border.Space) + "pt;");
        }
        /// <summary>
        /// Sets the default borders.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <param name="sb">The sb.</param>
        private void SetDefBorders(Borders borders, Borders parentBorders, StringBuilder sb)
        {
            SetDefBorderStyle("top", borders.Top, parentBorders.Top, sb);
            SetDefBorderStyle("left", borders.Left, parentBorders.Left, sb);
            SetDefBorderStyle("right", borders.Right, parentBorders.Right, sb);
            SetDefBorderStyle("bottom", borders.Bottom, parentBorders.Bottom, sb);
        }
        /// <summary>
        /// Sets the default border style.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <param name="border">The border.</param>
        /// <param name="sb">The sb.</param>
        private void SetDefBorderStyle(string suffix, Border border, Border pBorder, StringBuilder sb)
        {
            if (pBorder != border)
            {
                if (WriteBaseBdrStyle(pBorder, border))
                    GetBorderStyle(suffix, pBorder, sb, true);
            }
            else
            {
                if (border.BorderType == BorderStyle.None && !border.HasNoneStyle)
                    sb.Append("border-" + suffix + "-style: solid;");

                if (border.LineWidth == 0)
                    sb.Append("border-" + suffix + "-width: 1px;");

                if (border.Color == Color.Empty)
                    sb.Append("border-" + suffix + "-color: black;");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseBorder"></param>
        /// <param name="border"></param>
        /// <returns></returns>
        private bool WriteBaseBdrStyle(Border baseBorder, Border border)
        {
            bool writeBdr = false;

            if (baseBorder.BorderType == BorderStyle.None && baseBorder.HasNoneStyle)
                return writeBdr;

            if (border.BorderType != BorderStyle.None && border.BorderType != BorderStyle.Single)
                return writeBdr;

            if (baseBorder.BorderType != BorderStyle.None)
            {
                if (border.BorderType != baseBorder.BorderType ||
                  border.LineWidth != baseBorder.LineWidth)
                    writeBdr = true;
            }

            return writeBdr;
        }
        /// <summary>
        /// Converts border style to html border style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        private string ToBorderStyle(BorderStyle style)
        {
            switch (style)
            {
                case BorderStyle.None:
                    return "hidden";
                case BorderStyle.DotDash:
                    return "dashed";
                case BorderStyle.DotDotDash:
                    return "dashed";
                case BorderStyle.Triple:
                    return "double";
                case BorderStyle.ThinThickSmallGap:
                    return "double";
                case BorderStyle.ThinThinSmallGap:
                    return "double";
                case BorderStyle.ThinThickThinSmallGap:
                    return "double";
                case BorderStyle.ThinThickMediumGap:
                    return "double";
                case BorderStyle.ThickThinMediumGap:
                    return "double";
                case BorderStyle.ThickThickThinMediumGap:
                    return "double";
                case BorderStyle.ThinThickLargeGap:
                    return "double;";
                case BorderStyle.ThickThinLargeGap:
                    return "double";
                case BorderStyle.ThinThickThinLargeGap:
                    return "double";         
                case BorderStyle.DoubleWave:
                    return "double";
                case BorderStyle.DashSmallGap:
                    return "dashed";
                case BorderStyle.DashDotStroker:
                case BorderStyle.Thick:
                case BorderStyle.Hairline:
                case BorderStyle.Wave:
                case BorderStyle.Single:
                    return "solid";
                case BorderStyle.DashLargeGap:
                    return "dashed";
                case BorderStyle.Dot:
                    return "dotted";
                case BorderStyle.Double:
                    return "double";
                case BorderStyle.Engrave3D:
                    return "groove";
                case BorderStyle.Inset:
                    return "inset";
                case BorderStyle.Outset:
                    return "outset";
                case BorderStyle.Emboss3D:
                    return "ridge";
                case BorderStyle.Cleared:
                    return "none";
                default:
                    return "solid";
            }
        }
        /// <summary>
        /// Encodes the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string EncodeName(string name)
        {
            name = name.Trim();
            name = CheckValidSymbols(name);
            if (name.StartsWith(DEF_HYPHEN))
                name = name.Remove(0, 1);

            if (char.IsDigit(name[0]))
                name = "Style_" + name;
            return name;
        }
        /// <summary>
        /// Checks the valid symbols.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string CheckValidSymbols(string name)
        {
            for (int i = 0, cnt = name.Length; i < cnt; i++)
            {
                char ch = name[i];
                if (!Char.IsLetterOrDigit(ch) && name[i] != '-')
                {
                    name = name.Replace(name[i], '-');
                }
            }
            return name;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="pic">The picture.</param>
        /// <param name="imgPath">The image path.</param>
        private void ProcessImage(Image srcImage, string imgPath, int width, int height, bool isMetafile, ImageFormat format)
        {
            if (srcImage == null)
                return;

            Image destImage = (Image)srcImage.Clone();
            Bitmap bitmap = null;

            if (isMetafile)
            {
                Metafile metaFile = srcImage as Metafile;
                GraphicsUnit gUnit = GraphicsUnit.Display;
                System.Drawing.Size size = System.Drawing.Size.Ceiling(metaFile.GetBounds(ref gUnit).Size);
                bitmap = new Bitmap(size.Width, size.Height);
                bitmap.SetResolution(metaFile.HorizontalResolution, metaFile.VerticalResolution);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImageUnscaled(metaFile, System.Drawing.Point.Empty);
                    g.Dispose();
                }

                if (!m_cacheFilesInternally)
                    bitmap.Save(m_imagesFolder + imgPath, format);
            }
            else
            {
                bitmap = new Bitmap(destImage, width, height);
                if (!m_cacheFilesInternally)
                    bitmap.Save(m_imagesFolder + imgPath, format);
            }

            if (m_cacheFilesInternally)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, format);
                m_imageFiles.Add(imgPath, stream);
            }
        }
#endif
        /// <summary>
        /// Writes the empty paragraph.
        /// </summary>
        /// <param name="chFormat">The characterformat format.</param>
        private void WriteEmptyPara(WCharacterFormat chFormat)
        {
            m_writer.WriteStartElement("span");

            string style = GetStyle(chFormat);
            if (style.Length > 0)
            {
                m_writer.WriteAttributeString("style", style);
            }

            m_writer.WriteRaw("&#xa0;");
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void WriteText(string text)
        {
            text = text.Replace("&", "&amp;");
            if (text.Contains("\t"))
            {
                string temp = string.Empty;
                for (int i = 0; i < 15; i++)
                {
                    temp += ControlChar.NonBreakingSpace;
                }
                text = text.Replace("\t", temp + ControlChar.Space);
            }
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");
            text = text.Replace(DEF_NONBREAK_HYPHEN.ToString(), "&#8209;");
            text = text.Replace(DEF_SOFT_HYPHEN.ToString(), "&#xad;");
            text = text.Replace(Environment.NewLine, "</br>");
            text = text.Replace(c_slashRSymbol, "</br>");
            text = text.Replace(c_slashNSymbol, "</br>");
            m_writer.WriteRaw(text);
        }
        /// <summary>
        /// Replace empty space with non-breaking space
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ReplaceEmptySpace(string text)
        {
            char emptySpaceChar = (char)32;
            char nonBreakingSpaceChar = (char)160;
            if (text.StartsWith(" "))
            {
                string remainingText = text.TrimStart(' ');
                int startIndex = text.IndexOf(remainingText);
                string emptySpace = text.Substring(0, startIndex);
                if (remainingText.Length == 0)
                {
                    text = text.Replace(emptySpaceChar,  nonBreakingSpaceChar);
                }
                else
                {
                    emptySpace = emptySpace.Replace(emptySpaceChar, nonBreakingSpaceChar);
                    text = emptySpace + remainingText;
                }
            }
            return text;
        }
        /// <summary>
        /// Replaces the spaces with &nbsp;
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string WriteSpaces(string text)
        {
            int count = 0;
            int index = 0;
            char current, result;
            char prev = new char();
            char[] characters = new char[text.Length];
            characters = text.ToCharArray();
            for (count = 0; count < characters.Length; count++)
            {
                if (count >= 1)
                {
                    prev = characters[count - 1];
                }
                current = characters[count];
                if (((prev == ' ') && (current == ' ')))
                {
                    text = text.Remove(index, 1);
                    text = text.Insert(index, "&nbsp;");
                    index += 6;
                }
                else
                {
                    index++;
                }
            }
            return text;
        }
        /// <summary>
        /// Returns the Tab Space string
        /// </summary>
        /// <returns></returns>
        private string WriteTabSpace()
        {
            string tabs = string.Empty;
            for (int i = 0; i < 14; i++)
            {
                tabs += "&nbsp;";
            }
            return tabs;
        }
        /// <summary>
        /// Returns the string with style names (according to their hierarchy)
        /// which are applied on paragraph.
        /// </summary>
        /// <returns></returns>
        private string GetClassAttr(Style style, WordDocument doc)
        {
            List<String> styleHierarchy = new List<String>();
            string classAttr = string.Empty;
            if (style != null)
            {
                styleHierarchy.Add(style.Name);
                UpdateStyleHierarchy(style.BaseStyle as Style, styleHierarchy, doc);


                for (int i = styleHierarchy.Count - 1; i >= 0; i--)
                {
                    classAttr += EncodeName(styleHierarchy[i]);
                    if (i > 0)
                    {
                        classAttr += " ";
                    }
                }
            }
            return classAttr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="styleHirarchy"></param>
        private void UpdateStyleHierarchy(Style style, List<String> styleHirarchy, WordDocument doc)
        {
            if (style != null && style.StyleId != 0 && !style.Name.StartsWith("Normal"))
            {
                styleHirarchy.Add(style.Name);
                if (style.BaseStyle != null)
                    UpdateStyleHierarchy(style.BaseStyle as Style, styleHirarchy, doc);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        private bool IsSpacingNeeded(WParagraphFormat format, bool spacing)
        {
            return !(format.OwnerBase != null && format.OwnerBase.OwnerBase is WTableCell && spacing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool CheckParentFormat(WParagraphFormat format)
        {
            float rowheight;
            if (format.OwnerBase != null)
            {
                if (format.OwnerBase.OwnerBase is WTableCell && format.OwnerBase is WParagraph)
                {
                    WTableCell cell = format.OwnerBase.OwnerBase as WTableCell;
                    WParagraph para = format.OwnerBase as WParagraph;
                    WTextRange tr = new WTextRange(para.Document);
                    rowheight = cell.OwnerRow.Height;
                    if (para.Items.Count > 0 && para.Items.FirstItem is WTextRange)
                    {
                        tr = para.Items.FirstItem as WTextRange;
                    }
                    if (rowheight <= tr.CharacterFormat.FontSize)
                        return false;
                }
                else if (format.OwnerBase.OwnerBase is WTextBody && format.OwnerBase is WParagraph)
                {
                    WParagraph para = format.OwnerBase as WParagraph;
                    WTextRange tr;
                    IEntity ent = null;
                    if (para.Items.Count > 0 && para.ListFormat.ListType == ListType.NoList)
                    {
                        ent = para.Items.FirstItem as Entity;
                        while (ent.NextSibling != null && ent.EntityType != EntityType.TextRange)
                        {
                            ent = ent.NextSibling as Entity;
                            if (ent is WTextRange)
                                break;
                        }
                        if (ent is WTextRange)
                        {
                            tr = ent as WTextRange;
                            if (Math.Abs(format.LineSpacing) <= tr.CharacterFormat.CharacterProps.FontSize)
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        #region Implementation / lists
        /// <summary>
        /// Closes the nested list.
        /// </summary>
        private void CloseNestedList(int paraLevelNum)
        {
            if (m_currListLevel >= 0)
            {
                m_writer.WriteEndElement();
                m_writer.WriteRaw("\r\n");
            }
            m_currListLevel = -1;
        }
        /// <summary>
        /// Writes the type of the list.
        /// </summary>
        /// <param name="type">The type.</param>
        private void WriteListType(ListPatternType type, WListFormat listFormat)
        {
            string strType = null;
            if (type != ListPatternType.Bullet)
            {
                switch (type)
                {
                    case ListPatternType.LowLetter:
                        strType = "a";
                        break;
                    case ListPatternType.UpLetter:
                        strType = "A";
                        break;
                    case ListPatternType.LowRoman:
                        strType = "i";
                        break;
                    case ListPatternType.UpRoman:
                        strType = "I";
                        break;
                    default:
                        strType = "1";
                        break;
                }
            }
            else
            {
                switch (listFormat.CurrentListLevel.LevelNumber)
                {
                    case 0:
                        strType = "disc";
                        break;
                    case 1:
                        strType = "circle";
                        break;
                    case 2:
                        strType = "square";
                        break;
                    case 3:
                        strType = "disc";
                        break;
                    case 4:
                        strType = "circle";
                        break;
                    case 5:
                        strType = "square";
                        break;
                    default:
                        strType = "disc";
                        break;
                }

            }
            m_writer.WriteAttributeString("type", strType);
        }
        /// <summary>
        /// Gets the level numer.
        /// </summary>
        /// <param name="para">The list format.</param>
        /// <returns></returns>
        private int GetLevelNumer(WListFormat listFormat)
        {
            if (listFormat.ListType != ListType.NoList)
            {
                return listFormat.ListLevelNumber;
            }

            return -1;
        }
        /// <summary>
        /// Gets the list start value.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private int GetLstStartVal(WListFormat format)
        {
            if (format.CurrentListLevel.PatternType == ListPatternType.Bullet)
                return 1;

            if (!Lists.ContainsKey(format.CustomStyleName))
            {
                Dictionary<int, int> startVal = new Dictionary<int, int>();
                Lists.Add(format.CustomStyleName, startVal);
                WListLevel level = format.CurrentListStyle.Levels[format.ListLevelNumber];
                for (int i = 0; i <= level.LevelNumber; i++)
                {
                    startVal.Add(i, format.CurrentListStyle.Levels[i].StartAt + 1);
                }
                return level.StartAt;
            }
            else
            {
                Dictionary<int, int> lstStyle = Lists[format.CustomStyleName];
                if (lstStyle.ContainsKey(format.ListLevelNumber))
                {
                    int startAt = lstStyle[format.ListLevelNumber];
                    lstStyle[format.ListLevelNumber] = startAt + 1;
                    int levelno = format.ListLevelNumber;
                    while (lstStyle.ContainsKey(levelno + 1))
                    {
                        lstStyle[levelno + 1] = 1;
                        levelno++;
                    }
                    return startAt;
                }
                else
                {
                    WListLevel level = format.CurrentListStyle.Levels[format.ListLevelNumber];
                    lstStyle.Add(format.ListLevelNumber, level.StartAt + 1);
                    return level.StartAt;
                }
            }
        }
        /// <summary>
        /// Ensures the level restart.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="fullRestart">if set to <c>true</c> full restart is performed.</param>
        private void EnsureLvlRestart(WListFormat format, bool fullRestart)
        {
            if (m_lists == null)
                return;

            if (!Lists.ContainsKey(format.CustomStyleName))
                return;

            Dictionary<int, int> levels = Lists[format.CustomStyleName];
            ICollection keys = levels.Keys;
            IEnumerator enumerator = keys.GetEnumerator();
            int count = keys.Count;
            int[] levelNums = new int[count];
            int index = 0;

            while (enumerator.MoveNext())
            {
                levelNums[index] = (int)enumerator.Current;
                index++;
            }

            for (int i = 0; i < count; i++)
            {
                if (!fullRestart)
                {
                    if (levelNums[i] == 0 || format.CurrentListStyle.Levels[levelNums[i]].NoRestartByHigher)
                        continue;
                }

                levels[levelNums[i]] = format.CurrentListStyle.Levels[levelNums[i]].StartAt;
            }
        }
        /// <summary>
        /// Returns the Percentage of Color for the TextureStyle
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private float Build_TextureStyle(TextureStyle ts)
        {
            switch (ts)
            {
                case TextureStyle.Texture5Percent:
                case TextureStyle.Texture2Pt5Percent:
                case TextureStyle.Texture7Pt5Percent:
                    return 5.0f;
                case TextureStyle.Texture10Percent:
                    return 10.0f;
                case TextureStyle.Texture12Pt5Percent:
                    return 12.5f;
                case TextureStyle.Texture15Percent:
                    return 15f;
                case TextureStyle.Texture17Pt5Percent:
                    return 17.5f;
                case TextureStyle.Texture20Percent:
                    return 20.0f;
                case TextureStyle.Texture25Percent:
                case TextureStyle.Texture27Pt5Percent:
                    return 27.5f;
                case TextureStyle.Texture30Percent:
                case TextureStyle.Texture32Pt5Percent:
                    return 32.5f;
                case TextureStyle.Texture35Percent:
                    return 35f;
                case TextureStyle.Texture37Pt5Percent:
                    return 37.5f;
                case TextureStyle.Texture40Percent:
                case TextureStyle.Texture42Pt5Percent:
                    return 40.0f;
                case TextureStyle.Texture45Percent:
                case TextureStyle.Texture47Pt5Percent:
                    return 45.0f;
                case TextureStyle.Texture50Percent:
                case TextureStyle.Texture52Pt5Percent:
                    return 50.0f;
                case TextureStyle.Texture55Percent:
                case TextureStyle.Texture57Pt5Percent:
                    return 55.0f;
                case TextureStyle.Texture60Percent:
                    return 60.0f;
                case TextureStyle.Texture62Pt5Percent:
                    return 62.5f;
                case TextureStyle.Texture65Percent:
                case TextureStyle.Texture67Pt5Percent:
                    return 65.0f;
                case TextureStyle.Texture70Percent:
                case TextureStyle.Texture72Pt5Percent:
                    return 70.0f;
                case TextureStyle.Texture75Percent:
                case TextureStyle.Texture77Pt5Percent:
                    return 75.0f;
                case TextureStyle.Texture80Percent:
                case TextureStyle.Texture82Pt5Percent:
                    return 80.0f;
                case TextureStyle.Texture85Percent:
                    return 85.0f;
                case TextureStyle.Texture87Pt5Percent:
                    return 87.5f;
                case TextureStyle.Texture90Percent:
                case TextureStyle.Texture92Pt5Percent:
                    return 90.0f;
                case TextureStyle.Texture95Percent:
                case TextureStyle.Texture97Pt5Percent:
                    return 95.0f;
                case TextureStyle.TextureSolid:
                    return 100.0f;
                case TextureStyle.TextureCross:
                case TextureStyle.TextureDarkCross:
                case TextureStyle.TextureDarkDiagonalCross:
                case TextureStyle.TextureDarkDiagonalDown:
                case TextureStyle.TextureDarkDiagonalUp:
                case TextureStyle.TextureDarkHorizontal:
                case TextureStyle.TextureDarkVertical:
                case TextureStyle.TextureDiagonalCross:
                case TextureStyle.TextureDiagonalDown:
                case TextureStyle.TextureDiagonalUp:
                case TextureStyle.TextureHorizontal:
                case TextureStyle.TextureVertical:
                default:
                    return 0.0f;
            }
        }
        /// <summary>
        /// Gets the list format for paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <returns></returns>
        private WListFormat GetListFormat(WParagraph para)
        {
            if (string.IsNullOrEmpty(para.StyleName) || para.StyleName == "Normal")
                return para.ListFormat;

            if (para.ListFormat.CurrentListLevel != null && para.ListFormat.CurrentListStyle != null)
            {
                return para.ListFormat;
            }
            else if (!para.ListFormat.IsEmptyList)
            {
                WListFormat listFormat = new WListFormat(para);
                WParagraphStyle pStyle = null;
                while (listFormat.CurrentListLevel == null && listFormat.CurrentListStyle == null)
                {
                    pStyle = (pStyle == null) ? para.Document.Styles.FindByName(para.StyleName) as WParagraphStyle : pStyle.BaseStyle;
                    if (pStyle != null && pStyle.ListFormat != null)
                    {
                        if (listFormat.CurrentListLevel == null && listFormat.ListLevelNumber == 0 &&
                          pStyle.ListFormat.CurrentListLevel != null && pStyle.ListFormat.CurrentListLevel.LevelNumber >= 0 && pStyle.ListFormat.CurrentListLevel.LevelNumber <= 8)
                        {
                            listFormat.ListLevelNumber = pStyle.ListFormat.ListLevelNumber;
                        }
                        else
                        {
                            WParagraphStyle paraStyle = para.ParaStyle as WParagraphStyle;
                            int outLineLevelNumber;
                            while (paraStyle != null)
                            {
                                outLineLevelNumber = GetOutLineLevel(paraStyle.ParagraphFormat);
                                if (outLineLevelNumber != -1)
                                {
                                    listFormat.ListLevelNumber = outLineLevelNumber;
                                    break;
                                }
                                else
                                    paraStyle = paraStyle.BaseStyle;
                            }
                        }
                        if (listFormat.CurrentListStyle == null && !string.IsNullOrEmpty(pStyle.ListFormat.CustomStyleName))
                            listFormat.ApplyStyle(pStyle.ListFormat.CustomStyleName);
                    }
                    else
                        break;
                }
                return listFormat;
            }
            else
            {
                return para.ListFormat;
            }
        }
        /// <summary>
        /// Gets the outline level 
        /// </summary>
        /// <param name="paraFormat"></param>
        /// <returns></returns>
        private int GetOutLineLevel(WParagraphFormat paraFormat)
        {
            switch (paraFormat.OutlineLevel)
            {
                case OutlineLevel .Level1 :
                    return 0;
                case OutlineLevel .Level2 :
                    return 1;
                case OutlineLevel .Level3 :
                    return 2;
                case OutlineLevel.Level4 :
                    return 3;
                case OutlineLevel.Level5 :
                    return 4;
                case OutlineLevel.Level6 :
                    return 5;
                case OutlineLevel.Level7 :
                    return 6;
                case OutlineLevel.Level8 :
                    return 7;
                case OutlineLevel.Level9 :
                    return 8;
                default :
                    return -1;
            }
        }
        #endregion

        #region Helpher methods
        /// <summary>
        /// Creates Navigation point
        /// </summary>
        /// <param name="para">The paragraph</param>
        private void CreateNavigationPoint(WParagraph para)
        {
            if (m_hasNavigationId && !String.IsNullOrEmpty(para.Text))
            {
                // Checks for TOC custom styles
                if (m_document.HasTOC && !m_document.TOC.UseHeadingStyles)
                {
                    foreach (int key in m_document.TOC.TOCStyles.Keys)
                    {
                        foreach (WParagraphStyle toc_Style in m_document.TOC.TOCStyles[key])
                        {
                            if ((toc_Style.Name == para.StyleName) && key <= m_document.SaveOptions.EPubHeadingLevels)
                            {
                                string navigationPt = GetNavigationPoint();
                                m_writer.WriteAttributeString("id", navigationPt);
                                m_bookmarks.Add(string.Format("{0};{1}", key, navigationPt), GetParagraphText(para.Text));
                                break;
                            }
                        }
                    }
                }
                // TOC default heading styles
                else if (CheckHeadingStyle(para.StyleName))
                {
                    int level = GetHeadingLevel(para.StyleName);

                    if (level <= m_document.SaveOptions.EPubHeadingLevels)
                    {
                        string navigationPt = GetNavigationPoint();
                        m_writer.WriteAttributeString("id", navigationPt);
                        m_bookmarks.Add(string.Format("{0};{1}", level.ToString(), navigationPt), GetParagraphText(para.Text));
                    }
                }
            }
        }
        /// <summary>
        /// Checks if heading style is present in the document
        /// </summary>
        /// <param name="styleName">Style name</param>
        /// <returns>True if the style is heading style; false otherwise</returns>
        private bool CheckHeadingStyle(string styleName)
        {
            bool isHeadingStylePresent = false;
            foreach (string heading in m_headingStyles)
            {
                string paraHeading = styleName.ToLower().Replace(" ", string.Empty);
                if (paraHeading.Contains(heading))
                {
                    isHeadingStylePresent = true;
                    break;
                }
            }
            return isHeadingStylePresent;
        }
        /// <summary>
        /// Replaces special character
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns>Replaced text</returns>
        private string GetParagraphText(string text)
        {
            text = text.Replace("\v", string.Empty);
            return text;
        }
        /// <summary>
        /// Returns the heading level of the style
        /// </summary>
        /// <param name="p">Style Name</param>
        /// <returns>Heading level</returns>
        private int GetHeadingLevel(string p)
        {
            if (p.Contains(","))
                p = p.Split(new char[] { ',' })[0];
            if (p.Contains("+"))
                p = p.Split(new char[] { '+' })[0];
            char[] num = p.ToCharArray();
            string number = ""; int result;

            foreach (char ch in num)
            {
                if (ch == '_')
                    break;
                if (int.TryParse(ch.ToString(), out result))
                    number = number + result.ToString();
            }

            return int.Parse(number);
        }
        /// <summary>
        /// Gets new navigation point id
        /// </summary>
        /// <returns>Id</returns>
        internal string GetNavigationPoint()
        {
            return string.Concat("nav_Point", m_nameID++);
        }
        # endregion
    }

    internal class FloatApproxComparer : IComparer
    {
        /// <summary>
        /// Instance of FloatApproxComparer class.
        /// </summary>
        public FloatApproxComparer Instance = new FloatApproxComparer();

        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other. </exception>
        public int Compare(object x, object y)
        {
            if ((float)x - (float)y < 0.0001f)
                return 0;
            else if ((float)x > (float)y)
                return 1;
            else
                return -1;
        }
        #endregion
    }
}
#endif
