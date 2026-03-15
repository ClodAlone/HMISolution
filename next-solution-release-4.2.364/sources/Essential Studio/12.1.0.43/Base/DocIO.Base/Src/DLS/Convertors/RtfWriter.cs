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

#region File using directives

using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Specialized;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
using ImageFormat = Syncfusion.DocIO.DLS.Entities.ImageFormat;
#else
using Syncfusion.DocIO.Rendering;
using Image = System.Drawing.Image;
using System.Drawing;
using System.Drawing.Imaging;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Converts dls document into rtf format
    /// </summary>
    internal class RtfWriter : RtfNavigator
    {
        #region Constants
        private const char DEF_FOOTNOTE_SYMBOL = (char)0x02;
        private const string DEF_FONT_NAME = "Times New Roman";
        private const int MM_ANISOTROPIC = 8;
        // symbols
        private readonly string c_transfer = ((char)160).ToString();
        private readonly string c_formFieldSymbol = ((char)8194).ToString();
        private readonly string c_symbol92 = ((char)92).ToString();
        private readonly string c_symbol31 = ((char)31).ToString();
        private readonly string c_symbol61553 = ((char)61553).ToString();
        private readonly string c_symbol61549 = ((char)61549).ToString();
        private readonly string c_symbol123 = ((char)123).ToString();
        private readonly string c_symbol125 = ((char)125).ToString();
        private readonly string c_slashSymbol = ((char)92).ToString();
        private readonly string c_symbol8226 = ((char)8226).ToString();
        #endregion

        #region Fields
        private WordDocument m_doc;
        private Stream m_stream;

        private Encoding m_encoding;

        byte[] m_defStyleBytes;
        byte[] m_listTableBytes;
        byte[] m_listOverrideTableBytes;
        byte[] m_styleBytes;
        byte[] m_colorBytes;
        byte[] m_fontBytes;

        List<byte[]> m_mainBodyBytesList;

        private int m_fontId = 0;
        private int m_uniqueId = 1;
        private int m_cellEndPos;
        private int m_tableNestedLevel = 0;
        private int m_colorId = 1;

        private Dictionary<string, string> m_styles;
        // style Name and Id
        private Dictionary<string, string> m_stylesNumb;
        // list Name and StartValue
        private Dictionary<string, Dictionary<int, int>> m_listStart;
        // list Name and Id
        private Dictionary<string, int> m_listsIds;
        /// <summary>
        /// Collection of font table entries - to avoid serialization of duplicate font entries in fonttbl
        /// </summary>
        private Dictionary<string, string> m_fontEntries;

        private Dictionary<string, string> m_associatedFontEntries;

        private bool m_hasFootnote = false;
        private bool m_hasEndnote = false;
        private bool m_isCyrillicText = false;

        private Dictionary<int, string> m_listOverride;
        private Dictionary<int, int> m_commentIds;
        private Stack<Object> m_currentField;
        private Dictionary<Color, int> m_colorTable;
        #endregion

        #region Properties
        private Dictionary<string, string> FontEntries
        {
            get
            {
                if (m_fontEntries == null)
                    m_fontEntries = new Dictionary<string, string>();

                return m_fontEntries;
            }
        }
        private Dictionary<string, string> AssociatedFontEntries
        {
            get
            {
                if (m_associatedFontEntries == null)
                    m_associatedFontEntries = new Dictionary<string, string>();

                return m_associatedFontEntries;
            }
        }
        /// <summary>
        /// Gets the document lists ids.
        /// </summary>
        /// <value>The lists ids.</value>
        private Dictionary<string, int> ListsIds
        {
            get
            {
                if (m_listsIds == null)
                {
                    m_listsIds = new Dictionary<string, int>();
                }
                return m_listsIds;
            }
        }
        /// <summary>
        /// Gets the list override array.
        /// </summary>
        /// <value>The list override ar.</value>
        private Dictionary<int, string> ListOverrideAr
        {
            get
            {
                if (m_listOverride == null)
                {
                    m_listOverride = new Dictionary<int, string>();
                }
                return m_listOverride;
            }
        }
        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        private Dictionary<string, string> Styles
        {
            get
            {
                if (m_styles == null)
                {
                    m_styles = new Dictionary<string, string>();
                }
                return m_styles;
            }
        }
        /// <summary>
        /// Gets the style numb.
        /// </summary>
        /// <value>The style numb.</value>
        private Dictionary<string, string> StyleNumb
        {
            get
            {
                if (m_stylesNumb == null)
                {
                    m_stylesNumb = new Dictionary<string, string>();
                }
                return m_stylesNumb;
            }
        }
        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <value>The lists.</value>
        private Dictionary<string,Dictionary<int,int>> ListStart
        {
            get
            {
                if (m_listStart == null)
                {
                    m_listStart = new Dictionary<string, Dictionary<int, int>>();
                }
                return m_listStart;
            }
        }
        /// <summary>
        /// Gets the comment ids.
        /// </summary>
        /// <value>The comment ids.</value>
        private Dictionary<int, int> CommentIds
        {
            get
            {
                if (m_commentIds == null)
                {
                    m_commentIds = new Dictionary<int, int>();
                }
                return m_commentIds;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Stack<Object> CurrentField
        {
            get
            {
                if (m_currentField == null)
                    m_currentField = new Stack<Object>();
                return m_currentField;
            }
        }

        /// <summary>
        /// Gets the color table.
        /// </summary>
        /// <value>The color table.</value>
        private Dictionary<Color, int> ColorTable
        {
            get
            {
                if (m_colorTable == null)
                    m_colorTable = new Dictionary<Color, int>();
                
                return m_colorTable;
            }
        }
        #endregion

        #region Enums
        /// <summary>
        /// Row borders
        /// </summary>
        private enum BorderType
        {
            /// <summary>
            /// Right border
            /// </summary>
            Right,
            /// <summary>
            /// Left border
            /// </summary>
            Left,
            /// <summary>
            /// Top border
            /// </summary>
            Top,
            /// <summary>
            /// Bottom border
            /// </summary>
            Bottom
        }
        #endregion

        #region constructor
        public RtfWriter()
        {
#if (SILVERLIGHT || WP) && !WINRT
            m_encoding = new Syncfusion.DocIO.DLS.Convertors.ASCIIEncoding();
#else
            m_encoding = Encoding.GetEncoding("ASCII");
#endif
            m_styleBytes = m_encoding.GetBytes(@"{" + c_slashSymbol + "stylesheet");
            m_fontBytes = m_encoding.GetBytes(@"{" + c_slashSymbol + "fonttbl");
            m_colorBytes = m_encoding.GetBytes(@"{" + c_slashSymbol + "colortbl;");

        }
        #endregion

        #region Internal methods
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Writes document to the file specified by filename
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="document">The document.</param>
        internal void Write(string fileName, IWordDocument document)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create);
            Write(stream, document);
            stream.Dispose();
        }
#endif
        /// <summary>
        /// Writers WordDocument to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="document">The document.</param>
        internal void Write(Stream stream, IWordDocument document)
        {
            m_doc = document as WordDocument;
            m_stream = stream;

            BuildDefaultStyles();
            AppendListStyles();
            BuildStyleSheet();
            BuildSections();
            AppendOverrideList();

            byte[] byteArr;
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "rtf1" + c_slashSymbol + "ansi");
            m_stream.Write(byteArr, 0, byteArr.Length);

            WriteBody();
            byteArr = m_encoding.GetBytes("}");
            m_stream.Write(byteArr, 0, byteArr.Length);
        }
        /// <summary>
        /// Gets the RTF text.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        internal string GetRtfText(IWordDocument document)
        {
            string rtfText = "";
            m_doc = document as WordDocument;

            m_stream = new MemoryStream();
            Write(m_stream, m_doc);
            m_stream.Position = 0;
            byte[] data = new byte[m_stream.Length];
            m_stream.Read(data, 0, data.Length);
            m_stream.Dispose();
            rtfText = m_encoding.GetString(data, 0, data.Length);
           
            return rtfText;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Writes the body of document.
        /// </summary>
        private void WriteBody()
        {
            m_stream.Write(m_fontBytes, 0, m_fontBytes.Length);
            //Appended end symbol for fonttbl
            m_stream.WriteByte((byte)'}');
            m_stream.WriteByte((byte)'\r');
            m_stream.WriteByte((byte)'\n');
            m_fontBytes = null;            
            m_stream.Write(m_colorBytes, 0, m_colorBytes.Length);
            //Appended end symbol for colortbl
            m_stream.WriteByte((byte)'}');
            m_stream.WriteByte((byte)'\r');
            m_stream.WriteByte((byte)'\n');
            m_colorBytes = null;
            m_stream.Write(m_defStyleBytes, 0, m_defStyleBytes.Length);
            m_defStyleBytes = null;
            m_stream.Write(m_styleBytes, 0, m_styleBytes.Length);
            //Appended end symbol for stylesheet
            m_stream.WriteByte((byte)'}');
            m_stream.WriteByte((byte)'\r');
            m_stream.WriteByte((byte)'\n');
            m_styleBytes = null;
            m_stream.Write(m_listTableBytes, 0, m_listTableBytes.Length);
            m_listTableBytes = null;
            m_stream.Write(m_listOverrideTableBytes, 0, m_listOverrideTableBytes.Length);
            m_listOverrideTableBytes = null;
            int count = m_mainBodyBytesList.Count;
            for (int i = 0; i < count; i++)
            {
                m_stream.Write(m_mainBodyBytesList[0], 0, m_mainBodyBytesList[0].Length);
                m_mainBodyBytesList.RemoveAt(0);
            }
            m_mainBodyBytesList = null;
        }
        /// <summary>
        /// Builds the default styles.
        /// </summary>
        /// <returns></returns>
        private void BuildDefaultStyles()
        {
            WParagraphStyle normalStyle = m_doc.Styles.FindByName("Normal") as WParagraphStyle;

            MemoryStream defStyleStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "defchp");
            defStyleStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildCharacterFormat((normalStyle.CharacterFormat)));
            defStyleStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            defStyleStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            defStyleStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "defpap");
            defStyleStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildParagraphFormat(normalStyle.ParagraphFormat, null));
            defStyleStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            defStyleStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            defStyleStream.Write(byteArr, 0, byteArr.Length);

            m_defStyleBytes = defStyleStream.ToArray();
        }
        /// <summary>
        /// Builds the sections.
        /// </summary>
        private void BuildSections()
        {
            m_mainBodyBytesList = new List<byte[]>();

            BuildBackground();
            //MainBody.Append( BuildWatermark() );

            for (int i = 0, count = m_doc.Sections.Count; i < count; i++)
            {
                m_hasFootnote = m_hasEndnote = false;
                WSection section = m_doc.Sections[i] as WSection;
                BuildSectionProp(section);
                CheckFootEndnote();
                BuildSection(section);
            }
        }
        /// <summary>
        /// Builds the background.
        /// </summary>
        private void BuildBackground()
        {
            if (m_doc.Background.Type != BackgroundType.NoBackground)
            {
                MemoryStream memStream = new MemoryStream();
                byte[] byteArr;

                //Background shapes will show in page layout view.
                byteArr = m_encoding.GetBytes(c_slashSymbol + "viewbksp1");
                memStream.Write(byteArr, 0, byteArr.Length);

                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "background");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "shp");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "shpinst");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(BuildShapeFill(m_doc.Background, true));
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}}}");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(Environment.NewLine);
                memStream.Write(byteArr, 0, byteArr.Length);

                m_mainBodyBytesList.Add(memStream.ToArray());
            }
        }
        /// <summary>
        /// Builds the section.
        /// </summary>
        /// <param name="section">The section.</param>
        private void BuildSection(WSection section)
        {
            bool lastSection = (section.NextSibling == null);
            BuildHeadersFooters(section.HeadersFooters);
            BuildSectionBodyItems(section.Body.Items);

            if (!lastSection)
            {
                m_mainBodyBytesList.Add(m_encoding.GetBytes(c_slashSymbol + "sect"));
            }
            m_mainBodyBytesList.Add(m_encoding.GetBytes(Environment.NewLine));            
        }
        /// <summary>
        /// Builds the section body items.
        /// </summary>
        /// <param name="collect">The section body item collection.</param>
        private void BuildSectionBodyItems(BodyItemCollection collect)
        {
            foreach (Entity ent in collect)
            {
                switch (ent.EntityType)
                {
                    case EntityType.Paragraph:
                        m_mainBodyBytesList.Add(BuildParagraph(ent as WParagraph));
                        break;
                    case EntityType.Table:
                        m_mainBodyBytesList.Add(BuildTable(ent as WTable));
                        break;
                    case EntityType.StructureDocumentTag:
                        m_mainBodyBytesList.Add(BuildBodyItems((ent as StructureDocumentTagBlock).SDTContent.TextBody.Items));
                        break;
                }
            }
            if (collect.LastItem is WTable)
            {
               m_mainBodyBytesList.Add(BuildParagraph(new WParagraph(collect.Document)));
            }
        }
        /// <summary>
        /// Builds the body items.
        /// </summary>
        /// <param name="collect">The body item collection.</param>
        private byte[] BuildBodyItems(BodyItemCollection collect)
        {
            MemoryStream bodyItemStream = new MemoryStream();
            byte[] byteArr;
            foreach (Entity ent in collect)
            {
                switch (ent.EntityType)
                {
                    case EntityType.Paragraph:
                        byteArr = BuildParagraph(ent as WParagraph);
                        bodyItemStream.Write(byteArr, 0, byteArr.Length);
                        break;
                    case EntityType.Table:
                        byteArr = BuildTable(ent as WTable);
                        bodyItemStream.Write(byteArr, 0, byteArr.Length);
                        break;
                    case EntityType.StructureDocumentTag:
                        byteArr = BuildBodyItems((ent as StructureDocumentTagBlock).SDTContent.TextBody.Items);
                        bodyItemStream.Write(byteArr, 0, byteArr.Length);
                        break;
                }
            }
            if (collect.LastItem is WTable)
            {
                byteArr = BuildParagraph(new WParagraph(collect.Document));
                bodyItemStream.Write(byteArr, 0, byteArr.Length);
            }
            return bodyItemStream.ToArray();
        }
        /// <summary>
        /// Builds the headers and footers.
        /// </summary>
        /// <param name="headerFooters">The headers footers.</param>
        private void BuildHeadersFooters(WHeadersFooters headerFooters)
        {
            if (headerFooters == null && m_doc.Watermark == null)
                return;

            string waterMarkStr = string.Empty;
            Watermark waterMark = m_doc.Watermark;
            if (waterMark != null && waterMark.Type != WatermarkType.NoWatermark)
            {
                if (waterMark.Type == WatermarkType.TextWatermark)
                    waterMarkStr = BuildTextWtrmarkBody(waterMark as TextWatermark);
                else
                    waterMarkStr = BuildPictWtrmarkBody(waterMark as PictureWatermark);
            }

            if (headerFooters.EvenHeader.Items.Count > 0
              || (waterMark.Type != WatermarkType.NoWatermark && headerFooters.EvenHeader.WriteWatermark))
                BuildHeaderFooter(@"{" + c_slashSymbol + "headerl", headerFooters.EvenHeader.Items, waterMarkStr, headerFooters.EvenHeader.WriteWatermark);

            if (headerFooters.OddHeader.Items.Count > 0
              || (waterMark.Type != WatermarkType.NoWatermark && headerFooters.OddHeader.WriteWatermark))
                BuildHeaderFooter(@"{" + c_slashSymbol + "headerr", headerFooters.OddHeader.Items, waterMarkStr, headerFooters.OddHeader.WriteWatermark);

            if (headerFooters.EvenFooter.Items.Count > 0)
                BuildHeaderFooter(@"{" + c_slashSymbol + "footerl", headerFooters.EvenFooter.Items, string.Empty, false);

            if (headerFooters.OddFooter.Items.Count > 0)
                BuildHeaderFooter(@"{" + c_slashSymbol + "footerr", headerFooters.OddFooter.Items, string.Empty, false);

            if (headerFooters.FirstPageHeader.Items.Count > 0
              || (waterMark.Type != WatermarkType.NoWatermark && headerFooters.FirstPageHeader.WriteWatermark))
                BuildHeaderFooter(@"{" + c_slashSymbol + "headerf", headerFooters.FirstPageHeader.Items, waterMarkStr, headerFooters.FirstPageHeader.WriteWatermark);

            if (headerFooters.FirstPageFooter.Items.Count > 0)
                BuildHeaderFooter(@"{" + c_slashSymbol + "footerf", headerFooters.FirstPageFooter.Items, string.Empty, false);
        }
        /// <summary>
        /// Builds the header footer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="collect">The body item collection.</param>
        private void BuildHeaderFooter(string name, BodyItemCollection collect, string watermarkStr, bool writeWaterMark)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(name);
            memStream.Write(byteArr, 0, byteArr.Length);

            if (writeWaterMark)
            {
                byteArr = m_encoding.GetBytes(watermarkStr);
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = BuildBodyItems(collect);
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            m_mainBodyBytesList.Add(memStream.ToArray());
        }
        /// <summary>
        /// Builds the paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private byte[] BuildParagraph(WParagraph para)
        {
            MemoryStream paraStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(BuildListText(para.ListFormat));
            paraStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildParagraphFormat(para.ParagraphFormat, para));
            paraStream.Write(byteArr, 0, byteArr.Length);
            //SectionBody.Append(BuildListText(para.ListFormat));
            //SectionBody.Append(BuildParagraphFormat(para.ParagraphFormat, para));

            ParagraphItem item = null;
            for (int i = 0, count = para.Items.Count; i < count; i++)
            {
                item = para.Items[i];
                byteArr = BuildParagraphItem(item);
                paraStream.Write(byteArr, 0, byteArr.Length);
            }

            if (HasParaEnd(para))
            {
                byteArr = BuildParagraphEnd(para);
                paraStream.Write(byteArr, 0, byteArr.Length);
            }
            else
            {
                byteArr = m_encoding.GetBytes(BuildCharacterFormat(para.BreakCharacterFormat));
                paraStream.Write(byteArr, 0, byteArr.Length);
                //SectionBody.Append(BuildCharacterFormat(para.BreakCharacterFormat));
            }
            return paraStream.ToArray();
        }
        /// <summary>
        /// Builds the paragraph end.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private byte[] BuildParagraphEnd(WParagraph para)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildCharacterFormat(para.BreakCharacterFormat));
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "par");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);

            return memStream.ToArray();
        }
        #endregion

        #region Implementation / formatting
        /// <summary>
        /// Builds the character format.
        /// </summary>
        /// <param name="cFormat">The character format.</param>
        /// <returns></returns>
        private string BuildCharacterFormat(WCharacterFormat cFormat)
        {
            if (cFormat == null)
                return string.Empty;

            WCharacterFormat styleCF = null;
            if (!string.IsNullOrEmpty(cFormat.CharStyleName))
            {
                Style style = m_doc.Styles.FindByName(cFormat.CharStyleName) as Style;
                if (style != null && style.CharacterFormat != null)
                    styleCF = style.CharacterFormat;
            }

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(c_slashSymbol + "rtlch" + c_slashSymbol + "fcs0" + c_slashSymbol + "lang1033");
            strBuilder.Append(WriteFontNameBidi(cFormat));

            // Wtite Font size bidi for paragraph
            if (cFormat.OwnerBase is WParagraphStyle)
            {
                strBuilder.Append( c_slashSymbol + "afs");
                strBuilder.Append((short)Math.Round(cFormat.FontSizeBidi * RtfNavigator.c_two));
            }
            // Write Font size bidi for paragraph item
            else if (cFormat.HasValue(WCharacterFormat.FontSizeKey))
            {
                strBuilder.Append( c_slashSymbol + "afs");
                strBuilder.Append((short)Math.Round(cFormat.FontSizeBidi * RtfNavigator.c_two));
            }

            strBuilder.Append(c_slashSymbol + "ltrch" + c_slashSymbol + "fcs0" + c_slashSymbol + "lang1033");
            strBuilder.Append(WriteFontName(cFormat));

            // Wtite Font size for paragraph
            if (cFormat.OwnerBase is WParagraphStyle)
            {
                strBuilder.Append( c_slashSymbol + "fs");
                strBuilder.Append((short)Math.Round(cFormat.FontSize * RtfNavigator.c_two));
            }
            // Write Font size for paragraph item
            else if (cFormat.HasValue(WCharacterFormat.FontSizeKey))
            {
                strBuilder.Append( c_slashSymbol + "fs");
                strBuilder.Append((short)Math.Round(cFormat.FontSize * RtfNavigator.c_two));
            }
            //Write Local Language ASCII ID
            if (cFormat.HasValue(WCharacterFormat.LocaleIdASCIIKey))
                strBuilder.Append(c_slashSymbol + "lang" + cFormat.LocaleIdASCII);
            if (cFormat.HasValue(WCharacterFormat.LocaleIdFarEastKey))
                strBuilder.Append(c_slashSymbol + "langfe" + cFormat.LocaleIdFarEast);
            // Write Vertical Position for paragraph item
            if (cFormat.HasValue(WCharacterFormat.PositionKey))
            {
                if (cFormat.Position > 0)
                {
                    strBuilder.Append(c_slashSymbol + "up");
                    strBuilder.Append(cFormat.Position * RtfNavigator.c_two);
                }
                if (cFormat.Position < 0)
                {
                    strBuilder.Append(c_slashSymbol + "dn");
                    strBuilder.Append(-(cFormat.Position) * RtfNavigator.c_two);
                }
            }
            if (!string.IsNullOrEmpty(cFormat.CharStyleName))
            {
                if (Styles.ContainsKey(cFormat.CharStyleName))
                {
                    strBuilder.Append(Styles[cFormat.CharStyleName]);
                }
                //string styleId = Styles[cFormat.CharStyleName];
                //if (!string.IsNullOrEmpty(styleId))
                //    strBuilder.Append(styleId);
            }

            BuildBoolProp(WCharacterFormat.BoldKey, c_slashSymbol + "b", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.ItalicKey, c_slashSymbol + "i", cFormat, strBuilder);

            if (cFormat.HasValue(WCharacterFormat.UnderlineKey))
                BuildUnderLineStyle(cFormat.UnderlineStyle, strBuilder);
            else if (styleCF != null && styleCF.HasValue(WCharacterFormat.UnderlineKey))
                BuildUnderLineStyle(styleCF.UnderlineStyle, strBuilder);

            BuildBoolProp(WCharacterFormat.StrikeKey, c_slashSymbol + "strike", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.DoubleStrikeKey, c_slashSymbol + "striked1", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.ShadowKey, c_slashSymbol + "shad", cFormat, strBuilder);

            if (cFormat.HasValue(WCharacterFormat.SubSuperScriptKey))
            {
                if (cFormat.SubSuperScript == SubSuperScript.SuperScript)
                    strBuilder.Append( c_slashSymbol + "super");
                else if (cFormat.SubSuperScript == SubSuperScript.SubScript)
                    strBuilder.Append( c_slashSymbol + "sub");
                else if (cFormat.SubSuperScript == SubSuperScript.None)
                    strBuilder.Append( c_slashSymbol + "nosupersub");
            }
            else if (styleCF != null && styleCF.HasValue(WCharacterFormat.SubSuperScriptKey))
            {
                if (styleCF.SubSuperScript == SubSuperScript.SuperScript)
                    strBuilder.Append( c_slashSymbol + "super");
                else if (styleCF.SubSuperScript == SubSuperScript.SubScript)
                    strBuilder.Append( c_slashSymbol + "sub");
                else if (styleCF.SubSuperScript == SubSuperScript.None)
                    strBuilder.Append( c_slashSymbol + "nosupersub");
            }

            Color col = Color.Empty;
            if (styleCF != null)
                col = styleCF.TextColor;
            strBuilder.Append(BuildColorValue(cFormat, cFormat.TextColor, styleCF, col,
              WCharacterFormat.TextColorKey, c_slashSymbol + "cf"));

            if (styleCF != null)
                col = styleCF.TextBackgroundColor;
            strBuilder.Append(BuildColorValue(cFormat, cFormat.TextBackgroundColor, styleCF, col,
              WCharacterFormat.TextBkgColorKey, c_slashSymbol + "chcbpat"));

            if (styleCF != null)
                col = styleCF.HighlightColor;
            strBuilder.Append(BuildColorValue(cFormat, cFormat.HighlightColor, styleCF, col,
              WCharacterFormat.HighlightColorKey, c_slashSymbol + "highlight"));

            strBuilder.Append(BuildTextBorder(cFormat.Border));

            BuildBoolProp(WCharacterFormat.SmallCapsKey, c_slashSymbol + "scaps", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.HiddenKey, c_slashSymbol + "v", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.OutlineKey, c_slashSymbol + "outl", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.AllCapsKey, c_slashSymbol + "caps", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.EmbossKey, c_slashSymbol + "embo", cFormat, strBuilder);
            BuildBoolProp(WCharacterFormat.EngraveKey, c_slashSymbol + "impr", cFormat, strBuilder);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the bool prop.
        /// </summary>
        /// <param name="propKey">The property key.</param>
        /// <param name="propString">The property string.</param>
        /// <param name="chf">The character format.</param>
        private void BuildBoolProp(short propKey, string propString, WCharacterFormat chf, StringBuilder strBuilder)
        {
            bool complexProp = chf.GetComplexBoolValue(propKey);

            if (complexProp)
            {
                strBuilder.Append(propString);
            }
            else if (!complexProp && chf.IsComplex(propKey))
            {
                strBuilder.Append(propString + "0");
            }
        }
        /// <summary>
        /// Builds the paragraph format.
        /// </summary>
        /// <param name="pFormat">The p format.</param>
        /// <param name="appendStyle">if it is append style, set to <c>true</c>.</param>
        /// <returns></returns>
        private string BuildParagraphFormat(WParagraphFormat pFormat, WParagraph para)
        {
            if (pFormat == null)
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();
            string styleId = string.Empty;
            string styleName = string.Empty;

            if (para != null)
            {
                styleName = para.StyleName;
                if (string.IsNullOrEmpty(styleName))
                    styleName = "Normal";
                if (!Styles.ContainsKey(styleName))
                    BuildStyle(styleName);
                if (Styles.ContainsKey(styleName))
                    styleId = Styles[styleName];
            }

            WParagraphFormat stylePF = null;
            if (para != null && !string.IsNullOrEmpty(para.StyleName))
            {
                Style style = m_doc.Styles.FindByName(para.StyleName) as Style;
                if (style != null && (style as WParagraphStyle) != null && (style as WParagraphStyle).ParagraphFormat != null)
                {
                    stylePF = (style as WParagraphStyle).ParagraphFormat;
                }
            }

            strBuilder.Append(c_slashSymbol + "pard");
            strBuilder.Append(c_slashSymbol + "plain");
            //Applies english language to paragraph.
            strBuilder.Append(c_slashSymbol + "lang1033");
            //This is a paragraph property used to override the absence of the document-level \widowctrl
            if (pFormat.WidowControl)
                strBuilder.Append(c_slashSymbol + "widctlpar");
            else
                strBuilder.Append(c_slashSymbol + "nowidctlpar");

            strBuilder.Append(styleId);

            if (pFormat.Bidi)
                strBuilder.Append(c_slashSymbol + "rtlpar");

            if (pFormat.HorizontalAlignment == HorizontalAlignment.Left)
                strBuilder.Append(c_slashSymbol + "ql");
            else if (pFormat.HorizontalAlignment == HorizontalAlignment.Right)
                strBuilder.Append(c_slashSymbol + "qr");
            else if (pFormat.HorizontalAlignment == HorizontalAlignment.Center)
                strBuilder.Append(c_slashSymbol + "qc");
            else if (pFormat.HorizontalAlignment == HorizontalAlignment.Justify)
                strBuilder.Append(c_slashSymbol + "qj");
            else
                strBuilder.Append(c_slashSymbol + "qd");

            int firstLIndent = (int)pFormat.FirstLineIndent * RtfNavigator.c_twentiethOfPoint;
            strBuilder.Append(c_slashSymbol + "fi");
            if (firstLIndent == 0 && para != null && para.ListFormat.ListType != ListType.NoList)
            {
                firstLIndent = (int)para.ListFormat.CurrentListLevel.ParagraphFormat.FirstLineIndent * RtfNavigator.c_twentiethOfPoint;
            }
            strBuilder.Append(firstLIndent);

            int leftIndent = 0;
            strBuilder.Append(c_slashSymbol + "li");
            if (para != null && para.ListFormat.ListType != ListType.NoList && pFormat.LeftIndent == 0)
                leftIndent = (int)para.ListFormat.CurrentListLevel.ParagraphFormat.LeftIndent * RtfNavigator.c_twentiethOfPoint;
            else
                leftIndent = (int)pFormat.LeftIndent * RtfNavigator.c_twentiethOfPoint;
            strBuilder.Append(leftIndent);

            int rightIndent = (int)pFormat.RightIndent * RtfNavigator.c_twentiethOfPoint;
            strBuilder.Append(c_slashSymbol + "ri");
            if (rightIndent == 0 && para != null && para.ListFormat.ListType != ListType.NoList)
            {
                rightIndent = (int)para.ListFormat.CurrentListLevel.ParagraphFormat.RightIndent * RtfNavigator.c_twentiethOfPoint;
            }
            strBuilder.Append(rightIndent);

            if (pFormat.SuppressAutoHyphens)
                strBuilder.Append(c_slashSymbol + "hyphpar0");

            if (pFormat.MirrorIndents)
                strBuilder.Append(c_slashSymbol + "indmirror");

            strBuilder.Append(BuildFrameProps(pFormat));

            bool isInCell = (para != null && para.IsInCell) ? true : false;
            if (isInCell)
                strBuilder.Append(c_slashSymbol + "intbl");

            strBuilder.Append(BuildParaBorders(pFormat));
            strBuilder.Append(BuildParaSpacing(pFormat, isInCell));

            if (pFormat.HasBoolValueWithParent (WParagraphFormat.KeepKey))
                strBuilder.Append(c_slashSymbol + "keep");

            if (pFormat.HasBoolValueWithParent (WParagraphFormat.KeepFollowKey))
                strBuilder.Append(c_slashSymbol + "keepn");

            if (pFormat.HasBoolValueWithParent(WParagraphFormat.PageBreakBeforeKey))
                strBuilder.Append(c_slashSymbol + "pagebb");

            if (pFormat.HasValueWithParent(WParagraphFormat.OutlineLevelKey))
            {
                if ((byte)pFormat.OutlineLevel >= 0 && (byte)pFormat.OutlineLevel < 9)
                {
                    strBuilder.Append(c_slashSymbol + "outlinelevel");
                    strBuilder.Append((byte)pFormat.OutlineLevel);
                }
            }
            //else
            //  // Default value
            //  strBuilder.Append( @"\outlinelevel1" );

            if (pFormat.HasShading())
            {
                if (!pFormat.BackColor.IsEmpty)
                    strBuilder.Append(BuildColor(pFormat.BackColor, c_slashSymbol + "cbpat"));

                if (!pFormat.ForeColor.IsEmpty)
                    strBuilder.Append(BuildColor(pFormat.ForeColor, c_slashSymbol + "cfpat"));

                strBuilder.Append(BuildTextureStyle(pFormat.TextureStyle));
            }

            strBuilder.Append(BuildParaListId(para, pFormat));
            TabCollection tabs = stylePF != null ? stylePF.Tabs : null;
            tabs = pFormat.Tabs.Count > 0 ? pFormat.Tabs : tabs;
            strBuilder.Append(BuildTabs(tabs));

            if (para != null && para.ParaStyle != null)
            {
                WCharacterFormat paraCharFormat = para.ParaStyle.CharacterFormat;
                strBuilder.Append(BuildCharacterFormat(paraCharFormat));
            }
            else if (m_doc.DefCharFormat != null)
            {
                strBuilder.Append(BuildCharacterFormat(m_doc.DefCharFormat));
            }

            if (stylePF != null && stylePF.ContextualSpacing)
                strBuilder.Append(c_slashSymbol + "contextualspace");

            if (para != null && m_tableNestedLevel > 1)
            {
                para.ParagraphFormat.ParaProps.TablesNestingLevel = m_tableNestedLevel;
                strBuilder.Append(c_slashSymbol + "itap" + para.ParagraphFormat.ParaProps.TablesNestingLevel.ToString());
            }

            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the paragraph spacing.
        /// </summary>
        /// <param name="pFormat">The paragraph format.</param>
        /// <param name="stylePF">The style paragraph format.</param>
        /// <returns></returns>
        private string BuildParaSpacing(WParagraphFormat pFormat, bool isInCell)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (pFormat.ContextualSpacing)
            {
                strBuilder.Append(c_slashSymbol + "contextualspace");
            }
            else
            {
                if (pFormat.HasValueWithParent(WParagraphFormat.BeforeSpacingKey))
                    strBuilder.Append(BuildSpacing(c_slashSymbol + "sb", pFormat.BeforeSpacing));
                else if (m_doc.m_defParaFormat != null && m_doc.m_defParaFormat.HasValue(WParagraphFormat.BeforeSpacingKey) && !isInCell)
                    strBuilder.Append(BuildSpacing(c_slashSymbol + "sb", m_doc.m_defParaFormat.BeforeSpacing));

                if (pFormat.HasValueWithParent(WParagraphFormat.AfterSpacingKey))
                    strBuilder.Append(BuildSpacing(c_slashSymbol + "sa", pFormat.AfterSpacing));
                else if (m_doc.m_defParaFormat != null && m_doc.m_defParaFormat.HasValue(WParagraphFormat.AfterSpacingKey) && !isInCell)
                    strBuilder.Append(BuildSpacing(c_slashSymbol + "sa", m_doc.m_defParaFormat.AfterSpacing));

                if (pFormat.HasValueWithParent(WParagraphFormat.SpacingBeforeAutoKey))
                    strBuilder.Append(BuildAutoSpacing(c_slashSymbol + "sbauto", pFormat.SpaceBeforeAuto));

                if (pFormat.HasValueWithParent(WParagraphFormat.SpacingAfterAutoKey))
                    strBuilder.Append(BuildAutoSpacing(c_slashSymbol + "saauto", pFormat.SpaceAfterAuto));

                if (pFormat.HasValueWithParent(WParagraphFormat.LineSpacingKey))
                    strBuilder.Append(BuildLineSpacing(pFormat));
                else if (m_doc.m_defParaFormat != null && m_doc.m_defParaFormat.HasValue(WParagraphFormat.LineSpacingKey) && !isInCell)
                    strBuilder.Append(BuildLineSpacing(m_doc.m_defParaFormat));
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the spacing.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string BuildSpacing(string attribute, float value)
        {
            int spacing = (int)Math.Round(value * RtfNavigator.c_twentiethOfPoint);
            return attribute + spacing.ToString();
        }
        /// <summary>
        /// Builds the auto spacing.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hasSpacing">if it has spacing, set to <c>true</c>.</param>
        /// <returns></returns>
        private string BuildAutoSpacing(string value, bool hasSpacing)
        {
            if (hasSpacing)
                return value + "1";
            else
                return value + "0";
        }
        /// <summary>
        /// Builds the line spacing.
        /// </summary>
        /// <param name="pFormat">The paragraph format.</param>
        /// <returns></returns>
        private string BuildLineSpacing(WParagraphFormat pFormat)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(c_slashSymbol + "sl");
            int linespacing = (int)Math.Abs(Math.Round(pFormat.LineSpacing * DLSConstants.TwipsInOnePoint));
            if (pFormat.LineSpacingRule == LineSpacingRule.Exactly)
                strBuilder.Append("-" + linespacing);
            else
                strBuilder.Append(linespacing);

            switch (pFormat.LineSpacingRule)
            {
                case LineSpacingRule.AtLeast:
                case LineSpacingRule.Exactly:
                    strBuilder.Append(c_slashSymbol + "slmult0");
                    break;
                case LineSpacingRule.Multiple:
                    strBuilder.Append(c_slashSymbol + "slmult1");
                    break;
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the texture style.
        /// </summary>
        /// <param name="style">The texture style.</param>
        /// <returns></returns>
        private string BuildTextureStyle(TextureStyle style)
        {
            switch (style)
            {
                case TextureStyle.Texture5Percent:
                    return c_slashSymbol + "shading500";
                case TextureStyle.Texture2Pt5Percent:
                    return c_slashSymbol + "shading250";
                case TextureStyle.Texture7Pt5Percent:
                    return c_slashSymbol + "shading750";
                case TextureStyle.Texture10Percent:
                    return c_slashSymbol + "shading1000";
                case TextureStyle.Texture12Pt5Percent:
                    return c_slashSymbol + "shading1250";
                case TextureStyle.Texture15Percent:
                    return c_slashSymbol + "shading1500";
                case TextureStyle.Texture17Pt5Percent:
                    return c_slashSymbol + "shading1750";
                case TextureStyle.Texture20Percent:
                    return c_slashSymbol + "shading2000";
                case TextureStyle.Texture25Percent:
                    return c_slashSymbol + "shading2500";
                case TextureStyle.Texture27Pt5Percent:
                    return c_slashSymbol + "shading2750";
                case TextureStyle.Texture30Percent:
                    return c_slashSymbol + "shading3000";
                case TextureStyle.Texture32Pt5Percent:
                    return c_slashSymbol + "shading3250";
                case TextureStyle.Texture35Percent:
                    return c_slashSymbol + "shading3500";
                case TextureStyle.Texture37Pt5Percent:
                    return c_slashSymbol + "shading3750";
                case TextureStyle.Texture40Percent:
                    return c_slashSymbol + "shading4000";
                case TextureStyle.Texture42Pt5Percent:
                    return c_slashSymbol + "shading4250";
                case TextureStyle.Texture45Percent:
                    return c_slashSymbol + "shading4500";
                case TextureStyle.Texture47Pt5Percent:
                    return c_slashSymbol + "shading4750";
                case TextureStyle.Texture50Percent:
                    return c_slashSymbol + "shading5000";
                case TextureStyle.Texture52Pt5Percent:
                    return c_slashSymbol + "shading5250";
                case TextureStyle.Texture55Percent:
                    return c_slashSymbol + "shading5500";
                case TextureStyle.Texture57Pt5Percent:
                    return c_slashSymbol + "shading5750";
                case TextureStyle.Texture60Percent:
                    return c_slashSymbol + "shading6000";
                case TextureStyle.Texture62Pt5Percent:
                    return c_slashSymbol + "shading6250";
                case TextureStyle.Texture65Percent:
                    return c_slashSymbol + "shading6500";
                case TextureStyle.Texture67Pt5Percent:
                    return c_slashSymbol + "shading6750";
                case TextureStyle.Texture70Percent:
                    return c_slashSymbol + "shading7000";
                case TextureStyle.Texture72Pt5Percent:
                    return c_slashSymbol + "shading7250";
                case TextureStyle.Texture75Percent:
                    return c_slashSymbol + "shading7500";
                case TextureStyle.Texture77Pt5Percent:
                    return c_slashSymbol + "shading7750";
                case TextureStyle.Texture80Percent:
                    return c_slashSymbol + "shading8000";
                case TextureStyle.Texture82Pt5Percent:
                    return c_slashSymbol + "shading8250";
                case TextureStyle.Texture85Percent:
                    return c_slashSymbol + "shading8500";
                case TextureStyle.Texture87Pt5Percent:
                    return c_slashSymbol + "shading8750";
                case TextureStyle.Texture90Percent:
                    return c_slashSymbol + "shading9000";
                case TextureStyle.Texture92Pt5Percent:
                    return c_slashSymbol + "shading9250";
                case TextureStyle.Texture95Percent:
                    return c_slashSymbol + "shading9500";
                case TextureStyle.Texture97Pt5Percent:
                    return c_slashSymbol + "shading9750";
                case TextureStyle.TextureCross:
                    return c_slashSymbol + "bgcross";
                case TextureStyle.TextureDarkCross:
                    return c_slashSymbol + "bgdkcross";
                case TextureStyle.TextureDarkDiagonalCross:
                    return c_slashSymbol + "bgdkdcross";
                case TextureStyle.TextureDarkDiagonalDown:
                    return c_slashSymbol + "bgdkbdiag";
                case TextureStyle.TextureDarkDiagonalUp:
                    return c_slashSymbol + "bgdkfdiag";
                case TextureStyle.TextureDarkHorizontal:
                    return c_slashSymbol + "bgdkhoriz";
                case TextureStyle.TextureDarkVertical:
                    return c_slashSymbol + "bgdkvert";
                case TextureStyle.TextureDiagonalCross:
                    return c_slashSymbol + "bgdcross";
                case TextureStyle.TextureDiagonalDown:
                    return c_slashSymbol + "bgbdiag";
                case TextureStyle.TextureDiagonalUp:
                    return c_slashSymbol + "bgfdiag";
                case TextureStyle.TextureHorizontal:
                    return c_slashSymbol + "bghoriz";
                case TextureStyle.TextureVertical:
                    return c_slashSymbol + "bgvert";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the section properties.
        /// </summary>
        /// <param name="section">The section.</param>
        private void BuildSectionProp(WSection section)
        {
            MemoryStream sectionPropStream = new MemoryStream();
            byte[] byteArr;
            byteArr = m_encoding.GetBytes(c_slashSymbol + "sectd");
            sectionPropStream.Write(byteArr, 0, byteArr.Length);

            switch (section.BreakCode)
            {
                case SectionBreakCode.EvenPage:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "sbkeven");
                    sectionPropStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case SectionBreakCode.Oddpage:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "sbkodd");
                    sectionPropStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case SectionBreakCode.NewColumn:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "sbkcol");
                    sectionPropStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case SectionBreakCode.NoBreak:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "sbknone");
                    sectionPropStream.Write(byteArr, 0, byteArr.Length);
                    break;
                default:
                    break;
            }

            if (section.PageSetup.Orientation == PageOrientation.Landscape)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "lndscpsxn");
                sectionPropStream.Write(byteArr, 0, byteArr.Length);
            }
            if (section.TextDirection == DocTextDirection.LeftToRight)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "ltrsect");
                sectionPropStream.Write(byteArr, 0, byteArr.Length);
            }
            else if (section.TextDirection == DocTextDirection.RightToLeft)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "rtlsect");
                sectionPropStream.Write(byteArr, 0, byteArr.Length);
            }
            //Nead investigation. Nead for Picture in document. This propersies is only in MS Office 2007.
            //strBuilder.Append( @"\nouicompat" );

            byteArr = m_encoding.GetBytes(c_slashSymbol + "nofeaturethrottle1");
            sectionPropStream.Write(byteArr, 0, byteArr.Length);

            //This document has form field shading on.
            byteArr = m_encoding.GetBytes(c_slashSymbol + "formshade");
            sectionPropStream.Write(byteArr, 0, byteArr.Length);
            //Don't lay out AutoShapes like Word 97.
            byteArr = m_encoding.GetBytes(c_slashSymbol + "splytwnine");
            sectionPropStream.Write(byteArr, 0, byteArr.Length);

            byteArr = BuildPageSetup(section.PageSetup);
            sectionPropStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(Environment.NewLine);
            sectionPropStream.Write(byteArr, 0, byteArr.Length);

            m_mainBodyBytesList.Add(sectionPropStream.ToArray());
        }
        /// <summary>
        /// Builds the page setup.
        /// </summary>
        /// <param name="pSetup">The page setup.</param>
        /// <returns></returns>
        private byte[] BuildPageSetup(WPageSetup pSetup)
        {
            MemoryStream pageSetupStream = new MemoryStream();
            byte[] byteArr;

            if (pSetup.HeaderDistance > 0)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "headery" + pSetup.HeaderDistance * RtfNavigator.c_twentiethOfPoint);
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
            }

            if (pSetup.FooterDistance > 0)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "footery" + pSetup.FooterDistance * RtfNavigator.c_twentiethOfPoint);
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
            }

            switch (pSetup.VerticalAlignment)
            {
                case PageAlignment.Top:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "vertalt");
                    pageSetupStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case PageAlignment.Bottom:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "vertalb");
                    pageSetupStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case PageAlignment.Middle:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "vertalc");
                    pageSetupStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case PageAlignment.Justified:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "vertalj");
                    pageSetupStream.Write(byteArr, 0, byteArr.Length);
                    break;
            }

            if (pSetup.DifferentFirstPage)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "titlepg");
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
            }
            if (m_doc.DifferentOddAndEvenPages)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "facingp");
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
            }
            // Size and margin
            byteArr = m_encoding.GetBytes(c_slashSymbol + "paperw" + (int)Math.Round(pSetup.PageSize.Width * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "paperh" + (int)Math.Round(pSetup.PageSize.Height * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "margl" + (int)Math.Round(pSetup.Margins.Left * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "margr" + (int)Math.Round(pSetup.Margins.Right * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "margt" + (int)Math.Round(pSetup.Margins.Top * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "margb" + (int)Math.Round(pSetup.Margins.Bottom * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "gutter" + (int)Math.Round(pSetup.Margins.Gutter * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "deftab" + (int)Math.Round(m_doc.DefaultTabWidth * RtfNavigator.c_twentiethOfPoint));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            // Page Number
            if (pSetup.RestartPageNumbering != false)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "pgnrestart");
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(c_slashSymbol + "pgnstarts" + pSetup.PageStartingNumber);
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(BuildPageNumStyle(pSetup.PageNumberStyle));
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(c_slashSymbol + "pgncont");
            pageSetupStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(c_slashSymbol + "sectlinegrid" + pSetup.LinePitch * RtfNavigator.c_twentiethOfPoint);
            pageSetupStream.Write(byteArr, 0, byteArr.Length);
            if (pSetup.PitchType == GridPitchType.LinesOnly)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "sectspecifyl");
                pageSetupStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(BuildPageBorders(pSetup.Borders));
            pageSetupStream.Write(byteArr, 0, byteArr.Length);

            byteArr = BuildColumns((pSetup.OwnerBase as WSection).Columns);
            pageSetupStream.Write(byteArr, 0, byteArr.Length);

            return pageSetupStream.ToArray();
        }
        /// <summary>
        /// Builds the page number style.
        /// </summary>
        /// <param name="pageNumSt">The page number style.</param>
        /// <returns></returns>
        private string BuildPageNumStyle(PageNumberStyle pageNumSt)
        {
            switch (pageNumSt)
            {
                case PageNumberStyle.LetterLower:
                    return c_slashSymbol + "pgnlcltr";
                case PageNumberStyle.LetterUpper:
                    return c_slashSymbol + "pgnucltr";
                case PageNumberStyle.RomanLower:
                    return c_slashSymbol + "pgnlcrm";
                case PageNumberStyle.RomanUpper:
                    return c_slashSymbol + "pgnucrm";
                default:
                    return c_slashSymbol + "pgndec";
            }
        }
        /// <summary>
        /// Builds the columns.
        /// </summary>
        /// <param name="sols">The column collection.</param>
        /// <returns></returns>
        private byte[] BuildColumns(ColumnCollection cols)
        {
            MemoryStream colStream = new MemoryStream();
            byte[] byteArr;
            StringBuilder strBuilder = new StringBuilder();
            WPageSetup pSetup = (cols.OwnerSection as WSection).PageSetup;

            byteArr = m_encoding.GetBytes(c_slashSymbol + "cols" + cols.Count);
            colStream.Write(byteArr, 0, byteArr.Length);

            if (!pSetup.EqualColumnWidth && cols.Count != 1)
            {
                for (int i = 0, count = cols.Count; i < count; i++)
                {
                    Column col = cols[i] as Column;
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "colno" + (i+1));
                    colStream.Write(byteArr, 0, byteArr.Length);
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "colw" + col.Width * RtfNavigator.c_twentiethOfPoint);
                    colStream.Write(byteArr, 0, byteArr.Length);
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "colsr" + col.Space * RtfNavigator.c_twentiethOfPoint);
                    colStream.Write(byteArr, 0, byteArr.Length);
                }
            }

            if (pSetup.DrawLinesBetweenCols)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "linebetcol");
                colStream.Write(byteArr, 0, byteArr.Length);
            }
            return colStream.ToArray();
        }
        /// <summary>
        /// Builds the underline style.
        /// </summary>
        /// <param name="style">The under line style.</param>
        /// <param name="strBuilder">The string builder.</param>
        private void BuildUnderLineStyle(UnderlineStyle style, StringBuilder strBuilder)
        {
            switch (style)
            {
                case UnderlineStyle.Single:
                    strBuilder.Append(c_slashSymbol + "ul");
                    break;
                case UnderlineStyle.Dash:
                    strBuilder.Append(c_slashSymbol + "uldash");
                    break;
                case UnderlineStyle.Dotted:
                    strBuilder.Append(c_slashSymbol + "uld");
                    break;
                case UnderlineStyle.Double:
                    strBuilder.Append(c_slashSymbol + "uldb");
                    break;
                case UnderlineStyle.DashLong:
                    strBuilder.Append(c_slashSymbol + "ulldash");
                    break;
                case UnderlineStyle.None:
                    strBuilder.Append(c_slashSymbol + "ulnone");
                    break;
                case UnderlineStyle.Thick:
                    strBuilder.Append(c_slashSymbol + "ulth");
                    break;
                case UnderlineStyle.Wavy:
                    strBuilder.Append(c_slashSymbol + "ulwave");
                    break;
                case UnderlineStyle.WavyDouble:
                    strBuilder.Append(c_slashSymbol + "ululdbwave");
                    break;
                case UnderlineStyle.WavyHeavy:
                    strBuilder.Append(c_slashSymbol + "ulhwave");
                    break;
                case UnderlineStyle.Words:
                    strBuilder.Append(c_slashSymbol + "ulw");
                    break;
            }
        }
        /// <summary>
        /// Builds all tab from tab collection.
        /// </summary>
        /// <param name="tabs">The tab collection.</param>
        /// <returns></returns>
        private string BuildTabs(TabCollection tabs)
        {
            if (tabs == null)
                return String.Empty;
            StringBuilder strBuilder = new StringBuilder();
            if (tabs.Count > 0)
            {
                for (int i = 0, count = tabs.Count; i < count; i++)
                {
                    Tab tab = tabs[i] as Tab;

                    switch (tab.Justification)
                    {
                        case TabJustification.Centered:
                            strBuilder.Append(c_slashSymbol + "tqc");
                            break;
                        case TabJustification.Right:
                            strBuilder.Append(c_slashSymbol + "tqr");
                            break;
                        case TabJustification.Decimal:
                            strBuilder.Append(c_slashSymbol + "tqdec");
                            break;
                    }

                    if (tab.TabLeader != TabLeader.NoLeader)
                    {
                        switch (tab.TabLeader)
                        {
                            case TabLeader.Dotted:
                                strBuilder.Append(c_slashSymbol + "tldot");
                                break;
                            case TabLeader.Hyphenated:
                                strBuilder.Append(c_slashSymbol + "tlhyph");
                                break;
                            case TabLeader.Single:
                                strBuilder.Append(c_slashSymbol + "tlth");
                                break;
                            case TabLeader.Heavy:
                                strBuilder.Append(c_slashSymbol + "tleq");
                                break;
                        }
                    }
                    strBuilder.Append(c_slashSymbol + "tx");
                    strBuilder.Append(tab.Position * RtfNavigator.c_twentiethOfPoint);
                }
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the paragraph borders.
        /// </summary>
        /// <param name="pFormat">The paragraph format.</param>
        /// <returns></returns>
        private string BuildParaBorders(WParagraphFormat pFormat)
        {
            Borders borders = pFormat.Borders;
            StringBuilder strBuilder = new StringBuilder();

            if (pFormat.HasValue(WParagraphFormat.TopBorderKey) ||
              pFormat.HasValue(WParagraphFormat.TopBorderNewKey))
            {
                strBuilder.Append(c_slashSymbol + "brdrt");
                strBuilder.Append(BuildBorder(borders.Top, false));
            }

            if (pFormat.Borders.Top.Shadow)
                strBuilder.Append(c_slashSymbol + "brdrsh");

            if (pFormat.HasValue(WParagraphFormat.LeftBorderKey) ||
              pFormat.HasValue(WParagraphFormat.LeftBorderNewKey))
            {
                strBuilder.Append(c_slashSymbol + "brdrl");
                strBuilder.Append(BuildBorder(borders.Left, false));
            }

            if (pFormat.Borders.Left.Shadow)
                strBuilder.Append(c_slashSymbol + "brdrsh");

            if (pFormat.HasValue(WParagraphFormat.BottomBorderKey) ||
              pFormat.HasValue(WParagraphFormat.BottomBorderNewKey))
            {
                strBuilder.Append(c_slashSymbol + "brdrb");
                strBuilder.Append(BuildBorder(borders.Bottom, false));
            }

            if (pFormat.Borders.Bottom.Shadow)
                strBuilder.Append(c_slashSymbol + "brdrsh");

            if (pFormat.HasValue(WParagraphFormat.RightBorderKey) ||
              pFormat.HasValue(WParagraphFormat.RightBorderNewKey))
            {
                strBuilder.Append(c_slashSymbol + "brdrr");
                strBuilder.Append(BuildBorder(borders.Right, false));
            }

            if (pFormat.Borders.Right.Shadow)
                strBuilder.Append(c_slashSymbol + "brdrsh");

            //if( pFormat.HasValue( WParagraphFormat.BetweenBorderKey ) )
            //{
            //  strBuilder.Append( BuildBorder( borders.Horizontal, false ) );
            //}

            //if( pFormat.HasValue( WParagraphFormat.BarBorderKey ) )
            //{
            //  strBuilder.Append( BuildBorder( borders.Vertical, false ) );
            //}

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the page borders.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <returns></returns>
        private string BuildPageBorders(Borders borders)
        {
            if (borders.NoBorder)
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(c_slashSymbol + "pgbrdropt32");

            strBuilder.Append(c_slashSymbol + "pgbrdrt");
            strBuilder.Append(BuildBorder(borders.Top, false));
            strBuilder.Append(c_slashSymbol + "pgbrdrb");
            strBuilder.Append(BuildBorder(borders.Bottom, false));
            strBuilder.Append(c_slashSymbol + "pgbrdrl");
            strBuilder.Append(BuildBorder(borders.Left, false));
            strBuilder.Append(c_slashSymbol + "pgbrdrr");
            strBuilder.Append(BuildBorder(borders.Right, false));

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the border.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <returns></returns>
        private string BuildBorder(Border border, bool isTable)
        {
            BorderStyle borderStyle = border.BorderType;
            //int multiplier = (isTable) ? RtfNavigator.c_twentiethOfPoint : RtfNavigator.c_fiftiethOfPoint;
            int sz = (int)((float)(border.LineWidth * RtfNavigator.c_twentiethOfPoint));
            float space = border.Space * 20;

            if (borderStyle == BorderStyle.None && !border.HasNoneStyle)
                return BuildColor(Color.Black, c_slashSymbol + "brdrs" + c_slashSymbol + "brdrw10" + c_slashSymbol + "brdrcf");

            StringBuilder strBuilder = new StringBuilder();

            if (borderStyle != BorderStyle.None && borderStyle != BorderStyle.Cleared)
            {
                strBuilder.Append(BuildBorderStyle(borderStyle));
                strBuilder.Append(c_slashSymbol + "brdrw");
                strBuilder.Append(sz);
                strBuilder.Append(BuildColor(border.Color, c_slashSymbol + "brdrcf"));
            }

            if (space > 0)
            {
                strBuilder.Append(c_slashSymbol + "brsp");
                strBuilder.Append(space);
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the border style.
        /// </summary>
        /// <param name="borderStyle">The border style.</param>
        /// <returns></returns>
        private string BuildBorderStyle(BorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case BorderStyle.Double:
                    return c_slashSymbol + "brdrdb";
                case BorderStyle.Dot:
                    return c_slashSymbol + "brdrdot";
                case BorderStyle.Hairline:
                    return c_slashSymbol + "brdrhair";
                case BorderStyle.DashSmallGap:
                    return c_slashSymbol + "brdrdashsm";
                case BorderStyle.DotDash:
                    return c_slashSymbol + "brdrdashd";
                case BorderStyle.DotDotDash:
                    return c_slashSymbol + "brdrdashdd";
                case BorderStyle.Inset:
                    return c_slashSymbol + "brdrinset";
                case BorderStyle.None:
                    return c_slashSymbol + "brdrnone";
                case BorderStyle.Outset:
                    return c_slashSymbol + "brdroutset";
                case BorderStyle.Triple:
                    return c_slashSymbol + "brdrtriple";
                case BorderStyle.Wave:
                    return c_slashSymbol + "brdrwavy";
                case BorderStyle.DoubleWave:
                    return c_slashSymbol + "brdrwavydb";
                case BorderStyle.Emboss3D:
                    return c_slashSymbol + "brdremboss";
                case BorderStyle.Engrave3D:
                    return c_slashSymbol + "brdrengrave";
                case BorderStyle.ThickThinMediumGap:
                    return c_slashSymbol + "brdrtnthmg";
                case BorderStyle.ThinThickMediumGap:
                    return c_slashSymbol + "brdrthtnmg";
                case BorderStyle.ThickThinLargeGap:
                    return c_slashSymbol + "brdrtnthlg";
                case BorderStyle.ThinThickLargeGap:
                    return c_slashSymbol + "brdrthtnlg";
                case BorderStyle.ThinThickSmallGap:
                    return c_slashSymbol + "brdrthtnsg";
                case BorderStyle.ThinThickThinSmallGap:
                    return c_slashSymbol + "brdrtnthtnsg";
                case BorderStyle.ThinThickThinLargeGap:
                    return c_slashSymbol + "brdrtnthtnlg";
                case BorderStyle.DashDotStroker:
                    return c_slashSymbol + "brdrdashdotstr";
                case BorderStyle.DashLargeGap:
                    return c_slashSymbol + "brdrdash";
                case BorderStyle.Thick:
                    return c_slashSymbol + "brdrth";
                case BorderStyle.ThinThinSmallGap:
                    return c_slashSymbol + "brdrtnthsg";
                case BorderStyle.ThickThickThinMediumGap:
                    return c_slashSymbol + "brdrtnthtnmg";
                default:
                    return c_slashSymbol + "brdrs";
            }
        }
        /// <summary>
        /// Builds the style sheet.
        /// </summary>
        private void BuildStyleSheet()
        {
            // Init styles numbers
            int i = 1;
            foreach (Style st in m_doc.Styles)
            {
                if (!StyleNumb.ContainsKey(st.Name))
                {
                    StyleNumb.Add(st.Name, i.ToString());
                    i++;
                }
            }

            foreach (Style style in m_doc.Styles)
            {
                BuildStyle(style);
            }
        }
        /// <summary>
        /// Builds the style.
        /// </summary>
        /// <param name="style">The style.</param>
        private void BuildStyle(Style style)
        {
            if (Styles.ContainsKey(style.Name))
                return;

            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;
            memStream.Write(m_styleBytes, 0, m_styleBytes.Length);

            string styleId = string.Empty;
            if (style.StyleType == StyleType.ParagraphStyle)
            {
                //styleId = @"\s" + GetNextStyleId();
                if (StyleNumb.ContainsKey(style.Name))
                    styleId = c_slashSymbol + "s" + StyleNumb[style.Name];
            }
            else if (style.StyleType == StyleType.CharacterStyle)
            {
                //styleId = @"\cs" + GetNextStyleId();
                if (StyleNumb.ContainsKey(style.Name))
                    styleId = c_slashSymbol + "cs" + StyleNumb[style.Name];
            }

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(styleId);
            memStream.Write(byteArr, 0, byteArr.Length);

            if (style.StyleType == StyleType.ParagraphStyle)
            {
                WParagraphStyle pStyle = style as WParagraphStyle;
                byteArr = m_encoding.GetBytes(BuildParagraphFormat(pStyle.ParagraphFormat, null));
                memStream.Write(byteArr, 0, byteArr.Length);
            }

            if (style.StyleType == StyleType.CharacterStyle)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "additive");
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(BuildCharacterFormat(style.CharacterFormat));
            memStream.Write(byteArr, 0, byteArr.Length);

            if (style.BaseStyle != null && !string.IsNullOrEmpty(style.BaseStyle.Name)
                && StyleNumb.ContainsKey(style.BaseStyle.Name))
            {
                string baseStId = StyleNumb[style.BaseStyle.Name];
                byteArr = m_encoding.GetBytes(c_slashSymbol + "sbasedon" + baseStId);
                memStream.Write(byteArr, 0, byteArr.Length);
            }

            if (!string.IsNullOrEmpty(style.LinkStyle) && StyleNumb.ContainsKey(style.LinkStyle))
            {
                string linkSt = StyleNumb[style.LinkStyle];
                byteArr = m_encoding.GetBytes(c_slashSymbol + "slink" + linkSt);
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(c_slashSymbol + "sqformat");
            memStream.Write(byteArr, 0, byteArr.Length);

            string preparedName = PrepareText(style.Name);
            if (m_isCyrillicText)
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "falt " + preparedName + @"}");
                memStream.Write(byteArr, 0, byteArr.Length);
                m_isCyrillicText = false;
            }
            else
            {
                byteArr = m_encoding.GetBytes(@" " + preparedName);
                memStream.Write(byteArr, 0, byteArr.Length);
            }

            byteArr = m_encoding.GetBytes(@";}");
            memStream.Write(byteArr, 0, byteArr.Length);
            m_styleBytes = memStream.ToArray();

            Styles.Add(style.Name, styleId);
        }
        /// <summary>
        /// Builds the style.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        private void BuildStyle(string styleName)
        {
            Style style = m_doc.Styles.FindByName(styleName) as Style;
            if (style != null)
                BuildStyle(style);
        }
        /// <summary>
        /// Builds the text border.
        /// </summary>
        /// <param name="brd">The border.</param>
        private string BuildTextBorder(Border brd)
        {
            if (brd == null || (brd.BorderType == BorderStyle.None && !brd.HasNoneStyle))
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(c_slashSymbol + "chbrdr");
            strBuilder.Append(BuildBorder(brd, false));

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the frame property.
        /// </summary>
        /// <param name="pFormat">The paragraph format.</param>
        /// <returns></returns>
        private string BuildFrameProps(WParagraphFormat pFormat)
        {
            if (!pFormat.IsFrame)
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();

            switch (pFormat.FrameHorizontalPos)
            {
                case 0:
                    strBuilder.Append(c_slashSymbol + "phcol");
                    break;
                case 1:
                    strBuilder.Append(c_slashSymbol + "phmrg");
                    break;
                case 2:
                    strBuilder.Append(c_slashSymbol + "phpg");
                    break;
            }

            switch (pFormat.FrameVerticalPos)
            {
                case 0:
                    strBuilder.Append(c_slashSymbol + "pvmrg");
                    break;
                case 1:
                    strBuilder.Append(c_slashSymbol + "pvpg");
                    break;
                case 2:
                    strBuilder.Append(c_slashSymbol + "pvpara");
                    break;
            }

            if (pFormat.FrameX < 0)
            {
                switch ((short)pFormat.FrameX)
                {
                    case 0: //Left
                        strBuilder.Append(c_slashSymbol + "posxl");
                        break;
                    case -4://Center
                        strBuilder.Append(c_slashSymbol + "posxc");
                        break;
                    case -8://Right
                        strBuilder.Append(c_slashSymbol + "posxr");
                        break;
                    case -12://Inside
                        strBuilder.Append(c_slashSymbol + "posxi");
                        break;
                    case -16://Outside
                        strBuilder.Append(c_slashSymbol + "posxo");
                        break;
                }
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the paragraph list id.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="pFormat">The paragraph format.</param>
        /// <returns></returns>
        private string BuildParaListId(WParagraph para, WParagraphFormat pFormat)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (pFormat.OwnerBase == null)
                return string.Empty;

            WParagraphStyle style = null;
            int listId = -1;
            if (pFormat.OwnerBase is WParagraphStyle)
                style = pFormat.OwnerBase as WParagraphStyle;
            else if (pFormat.OwnerBase is WParagraph)
                style = m_doc.Styles.FindByName((pFormat.OwnerBase as WParagraph).StyleName) as WParagraphStyle;

            string listOverStyleName = string.Empty;

            if (para != null && para.ListFormat.ListType != ListType.NoList
              && !string.IsNullOrEmpty(para.ListFormat.CurrentListStyle.Name))
            {
                listId = ListsIds[para.ListFormat.CurrentListStyle.Name] + 1;
                listOverStyleName = para.ListFormat.LFOStyleName;
            }
            else if (style != null && style.ListFormat != null && style.ListFormat.CurrentListStyle != null
              && !string.IsNullOrEmpty(style.ListFormat.CurrentListStyle.Name))
            {
                listId = ListsIds[style.ListFormat.CurrentListStyle.Name] + 1;
                listOverStyleName = style.ListFormat.LFOStyleName;
            }

            if (listId != -1)
            {
                strBuilder.Append(c_slashSymbol + "ls");
                strBuilder.Append(listId);
                if (!ListOverrideAr.ContainsKey(listId))
                    ListOverrideAr.Add(listId, listOverStyleName);
                else if (string.IsNullOrEmpty(ListOverrideAr[listId]) && !string.IsNullOrEmpty(listOverStyleName))
                    ListOverrideAr[listId] = listOverStyleName;

                if (para != null)
                {
                    strBuilder.Append(c_slashSymbol + "ilvl");
                    strBuilder.Append(para.ListFormat.ListLevelNumber);
                }
            }

            return strBuilder.ToString();
        }
        #endregion

        #region Implementation / table
        /// <summary>
        /// Builds the table.
        /// </summary>
        /// <param name="table">The table.</param>
        private byte[] BuildTable(WTable table)
        {
            MemoryStream tableStream = new MemoryStream();
            m_tableNestedLevel++;
            for (int i = 0, count = table.Rows.Count; i < count; i++)
            {
                WTableRow row = table.Rows[i] as WTableRow;
                byte[] rowBytes = BuildTableRow(row);
                tableStream.Write(rowBytes, 0, rowBytes.Length);
            }
            m_tableNestedLevel--;
            return tableStream.ToArray();
        }
        /// <summary>
        /// Builds the table row.
        /// </summary>
        /// <param name="row">The table row.</param>
        private byte[] BuildTableRow(WTableRow row)
        {
            MemoryStream rowStream = new MemoryStream();
            byte[] byteArr;
            string rowFormatStr = BuildTRowFormat(row.RowFormat);
            if (m_tableNestedLevel == 1)//&& !HasNestedItems( row ) )
            {
                byteArr = m_encoding.GetBytes(rowFormatStr);
                rowStream.Write(byteArr, 0, byteArr.Length);
            }
            for (int i = 0, count = row.Cells.Count; i < count; i++)
            {
                WTableCell cell = row.Cells[i] as WTableCell;
                byteArr = BuildTableCell(cell);
                rowStream.Write(byteArr, 0, byteArr.Length);
            }

            if (m_tableNestedLevel != 1)
            {
                rowFormatStr = @"{" + c_slashSymbol + "*" + c_slashSymbol + "nesttableprops"
                    + rowFormatStr + c_slashSymbol + "nestrow}";
            }

            byteArr = m_encoding.GetBytes(rowFormatStr);
            rowStream.Write(byteArr, 0, byteArr.Length);
            //SectionBody.Append(rowFormatStr.ToString());

            if (row.OwnerTable != null && row.OwnerTable.Owner != null
              && row.OwnerTable.Owner.OwnerBase != null && row.OwnerTable.Owner.OwnerBase is WTableRow)
            {
                WTableRow ownerRow = row.OwnerTable.Owner.OwnerBase as WTableRow;
                byteArr = m_encoding.GetBytes(BuildTRowFormat(ownerRow.RowFormat));
                rowStream.Write(byteArr, 0, byteArr.Length);
            }
            else
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "row}");
                rowStream.Write(byteArr, 0, byteArr.Length);
            }
            return rowStream.ToArray();
        }
        /// <summary>
        /// Builds the Table row property.
        /// </summary>
        /// <param name="row">The table row.</param>
        private string BuildTRowFormat(RowFormat rowFormat)
        {
            StringBuilder strBuilder = new StringBuilder();
            WTableRow row = rowFormat.OwnerRow;

            strBuilder.Append(c_slashSymbol + "trowd");

            if (row != null && row.OwnerTable != null && row == row.OwnerTable.LastRow)
                strBuilder.Append(c_slashSymbol + "lastrow");

            if(row .IsHeader )
                strBuilder.Append(c_slashSymbol + "trhdr");

            if (!row.RowFormat.IsBreakAcrossPages)
                strBuilder.Append(c_slashSymbol + "trkeep");
            //Handled to preserve the first row first cell padding left value as like ms word
            float paddingLeft = 0;
            if (row.OwnerTable.FirstRow.Cells.Count > 0)
            {
                WTableCell cell = row.OwnerTable.FirstRow.Cells[0];
                paddingLeft = cell.CellFormat.Paddings.Left;
                if (cell.CellFormat.SamePaddingsAsTable)
                    paddingLeft = row.OwnerTable.TableFormat.Paddings.Left;
            }
            strBuilder.Append(c_slashSymbol + "tblind" + Math.Round(rowFormat.LeftIndent * DLSConstants.TwipsInOnePoint).ToString());
            strBuilder.Append(c_slashSymbol + "tblindtype3");
            if (rowFormat.LeftIndent != 0 && !rowFormat.WrapTextAround && rowFormat.HorizontalAlignment==RowAlignment.Left)
            {
                int tableIndent = (int)Math.Round((rowFormat.LeftIndent - paddingLeft) * DLSConstants.TwipsInOnePoint);
                tableIndent -= (paddingLeft != 0) ? 0 : 5;
                strBuilder.Append(c_slashSymbol + "trleft" + tableIndent.ToString());
                m_cellEndPos = tableIndent;
            }
            else 
            {
                float leftIndent = -paddingLeft;
                int tableIndent = (int)Math.Round(leftIndent * DLSConstants.TwipsInOnePoint);
                tableIndent = (paddingLeft != 0) ? tableIndent : -5;
                strBuilder.Append(c_slashSymbol + "trleft" + tableIndent.ToString());
                m_cellEndPos = tableIndent;
            }

            if (rowFormat.Positioning.VertRelationTo == VerticalRelation.Paragraph)
                strBuilder.Append(c_slashSymbol + "tpvpara");
            else if (rowFormat.Positioning.VertRelationTo == VerticalRelation.Page)
                strBuilder.Append(c_slashSymbol + "tpvpg");

            if (rowFormat.Positioning.HorizRelationTo == HorizontalRelation.Page)
                strBuilder.Append(c_slashSymbol + "tphpg");
            else if (rowFormat.Positioning.HorizRelationTo == HorizontalRelation.Margin)
                strBuilder.Append(c_slashSymbol + "tphmrg");


            if (rowFormat.WrapTextAround)
            {
                int tblpx = (int)Math.Round(rowFormat.Positioning.HorizPosition * DLSConstants.TwipsInOnePoint);
                strBuilder.Append(c_slashSymbol + "tposx" + tblpx.ToString());

                int tblpy = (int)Math.Round(rowFormat.Positioning.VertPosition * DLSConstants.TwipsInOnePoint);
                strBuilder.Append(c_slashSymbol + "tposy" + tblpy.ToString());

                int tdfrmtxtLeft = (int)Math.Round(rowFormat.Positioning.DistanceFromLeft * DLSConstants.TwipsInOnePoint);
                if (tdfrmtxtLeft != 0)
                    strBuilder.Append(c_slashSymbol + "tdfrmtxtLeft" + tdfrmtxtLeft.ToString());

                int tdfrmtxtRight = (int)Math.Round(rowFormat.Positioning.DistanceFromRight * DLSConstants.TwipsInOnePoint);
                if (tdfrmtxtRight != 0)
                    strBuilder.Append(c_slashSymbol + "tdfrmtxtRight" + tdfrmtxtLeft.ToString());

                int tdfrmtxtTop = (int)Math.Round(rowFormat.Positioning.DistanceFromTop * DLSConstants.TwipsInOnePoint);
                if (tdfrmtxtTop != 0)
                    strBuilder.Append(c_slashSymbol + "tdfrmtxtTop" + tdfrmtxtLeft.ToString());

                int tdfrmtxtBottom = (int)Math.Round(rowFormat.Positioning.DistanceFromBottom * DLSConstants.TwipsInOnePoint);
                if (tdfrmtxtBottom != 0)
                    strBuilder.Append(c_slashSymbol + "tdfrmtxtBottom" + tdfrmtxtLeft.ToString());
            }
            if (!rowFormat.Positioning.AllowOverlap)
            {
                strBuilder.Append(c_slashSymbol + "tabsnoovrlp" + "1");
            }

            
            if (rowFormat.CellSpacing != -1)
            {
                string cellSpacing = ((int)Math.Round(rowFormat.CellSpacing * DLSConstants.TwipsInOnePoint)).ToString();
                //strBuilder.Append( @"\trgaph" + cellSpacing );
                strBuilder.Append(c_slashSymbol + "trspdl" + cellSpacing);
                strBuilder.Append(c_slashSymbol + "trspdr" + cellSpacing);
                strBuilder.Append(c_slashSymbol + "trspdb" + cellSpacing);
                strBuilder.Append(c_slashSymbol + "trspdt" + cellSpacing);
                strBuilder.Append(c_slashSymbol + "trspdfl3" + c_slashSymbol + "trspdft3" + c_slashSymbol
                    + "trspdfb3" + c_slashSymbol + "trspdfr3");
            }
            else if (rowFormat.LeftIndent > 0)
            {
                strBuilder.Append(c_slashSymbol + "trgaph108");
            }

            if (rowFormat.Height != 0)
                strBuilder.Append(c_slashSymbol + "trrh" + ((int)Math.Round(row.Height * RtfNavigator.c_twentiethOfPoint)).ToString());

            if (row != null && row.OwnerTable != null)
            {
                int tableWidth = 0;
                string widthType = string.Empty;
                if (row.OwnerTable.TableFormat.PreferredWidth.WidthType != FtsWidth.Auto)
                {
                    if (row.OwnerTable.TableGrid.Count > 0 && row.OwnerTable.DocxTableFormat.HasFormat)
                        tableWidth = (int)(row.OwnerTable.TableGrid[row.OwnerTable.TableGrid.Count - 1]);
                    else if (row.OwnerTable.TableFormat.PreferredWidth.WidthType == FtsWidth.Percentage)
                    {
                        tableWidth = (int)Math.Round(row.OwnerTable.PreferredTableWidth.Width * DLSConstants.PercentageFactor);
                        widthType = "trftsWidth2";
                    }
                    else if (row.OwnerTable.TableFormat.PreferredWidth.WidthType == FtsWidth.Point)
                    {
                        tableWidth = (int)Math.Round(row.OwnerTable.PreferredTableWidth.Width * DLSConstants.TwipsInOnePoint);
                        widthType = "trftsWidth3";
                    }
                    strBuilder.Append(c_slashSymbol + "trwWidth" + tableWidth.ToString());
                    //Units for @"\trwWidth" - Twips
                    if (widthType != string.Empty)
                        strBuilder.Append(c_slashSymbol + widthType);
                }
            }
            if (rowFormat.IsAutoResized)
                strBuilder.Append(c_slashSymbol + "trautofit1");
            if (rowFormat.GridBeforeWidth.Width > 0 && rowFormat.GridBeforeWidth.WidthType != FtsWidth.Auto && rowFormat.GridBeforeWidth.WidthType != FtsWidth.None)
            {
                int width = 0;
                string widthType = string.Empty;
                if (rowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage)
                {
                    width = (int)Math.Round(rowFormat.GridBeforeWidth.Width * DLSConstants.PercentageFactor);
                    widthType = "trftsWidthB2";
                }
                else if (rowFormat.GridBeforeWidth.WidthType == FtsWidth.Point)
                {
                    width = (int)Math.Round(rowFormat.GridBeforeWidth.Width * DLSConstants.TwipsInOnePoint);
                    widthType = "trftsWidthB3";
                }
                strBuilder.Append(c_slashSymbol + "trftsWidthB" + width.ToString());
                //Units for @"\trwWidth" - Twips
                if (widthType != string.Empty)
                    strBuilder.Append(c_slashSymbol + widthType);
            }
            if (rowFormat.GridAfterWidth.Width > 0 && rowFormat.GridAfterWidth.WidthType != FtsWidth.Auto && rowFormat.GridAfterWidth.WidthType != FtsWidth.None)
            {
                int width = 0;
                string widthType = string.Empty;
                if (rowFormat.GridAfterWidth.WidthType == FtsWidth.Percentage)
                {
                    width = (int)Math.Round(rowFormat.GridAfterWidth.Width * DLSConstants.PercentageFactor);
                    widthType = "trftsWidthA2";
                }
                else if (rowFormat.GridAfterWidth.WidthType == FtsWidth.Point)
                {
                    width = (int)Math.Round(rowFormat.GridAfterWidth.Width * DLSConstants.TwipsInOnePoint);
                    widthType = "trftsWidthA3";
                }
                strBuilder.Append(c_slashSymbol + "trftsWidthA" + width.ToString());
                //Units for @"\trwWidth" - Twips
                if (widthType != string.Empty)
                    strBuilder.Append(c_slashSymbol + widthType);
            }
            if (row != null)
                strBuilder.Append(BuildCharacterFormat(row.CharacterFormat));

            if (rowFormat.HorizontalAlignment == RowAlignment.Right)
                strBuilder.Append(c_slashSymbol + "trqr");
            else if (rowFormat.HorizontalAlignment == RowAlignment.Center)
                strBuilder.Append(c_slashSymbol + "trqc");
            else
                strBuilder.Append(c_slashSymbol + "trql");

            strBuilder.Append(BuildTRowBorders(rowFormat.Borders));

            if (!rowFormat.Paddings.IsDefault)
                strBuilder.Append(BuildPadding(rowFormat.Paddings, true));

            WSection ownerSec = GetOwnerSection(row as Entity);

            if (row != null)
            {
                for (int i = 0, count = row.Cells.Count; i < count; i++)
                {
                    WTableCell cell = row.Cells[i] as WTableCell;
                    if (cell != null && cell.CellFormat != null)
                    {
                        strBuilder.Append(Environment.NewLine);
                        strBuilder.Append(BuildTCellFormat(cell.CellFormat));
                    }
                }
            }

            m_cellEndPos = 0;

            strBuilder.Append(Environment.NewLine);
            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the Table cell property.
        /// </summary>
        /// <param name="cFormat">The table cell format.</param>
        /// <returns></returns>
        private string BuildTCellFormat(CellFormat cFormat)
        {
            StringBuilder strBuilder = new StringBuilder();
            WTableCell cell = cFormat.OwnerBase as WTableCell;

            if (cell.CellFormat.VerticalMerge == CellMerge.Start)
            {
                strBuilder.Append(c_slashSymbol + "clvmgf");
                strBuilder.Append(BuildVertAlignment(cell.CellFormat.VerticalAlignment));
            }
            else if (cell.CellFormat.VerticalMerge == CellMerge.Continue)
            {
                strBuilder.Append(c_slashSymbol + "clvmrg");
                strBuilder.Append(BuildVertAlignment(cFormat.VerticalAlignment));
            }
            else
            {
                strBuilder.Append(BuildVertAlignment(cFormat.VerticalAlignment));
            }

            if (cFormat.Borders.NoBorder)
                strBuilder.Append(BuildTCellBorders(cell, (cell.OwnerRow as WTableRow).RowFormat.Borders, null));
            else
                strBuilder.Append(BuildTCellBorders(cell, cFormat.Borders, (cell.OwnerRow as WTableRow).RowFormat.Borders));

            if (!cFormat.BackColor.IsEmpty)
            {
                //strBuilder.Append( @"\clshdng" + ( cFormat.BackColor.A * 10 ).ToString() );
                strBuilder.Append(BuildColor(cFormat.BackColor, c_slashSymbol + "clcbpat"));
            }
            //if( cell.GetCellIndex() != 0 || cell.OwnerRow.Cells.Count == 0 )
            m_cellEndPos += (int)Math.Round(cell.Width * RtfNavigator.c_twentiethOfPoint);
            //Units for @"\clwWidth" - Twips
            strBuilder.Append(c_slashSymbol + "clftsWidth3");
            strBuilder.Append(c_slashSymbol + "clwWidth");
            strBuilder.Append((int)Math.Round(cell.Width * RtfNavigator.c_twentiethOfPoint));
            if (!cFormat.Paddings.IsDefault)
                strBuilder.Append(BuildPadding(cFormat.Paddings, false));
            strBuilder.Append(c_slashSymbol + "cellx");
            strBuilder.Append(m_cellEndPos);

            if (cFormat.FitText)
                strBuilder.Append(c_slashSymbol + "clFitText");

            if (!cFormat.TextWrap)
                strBuilder.Append(c_slashSymbol + "clNoWrap");
            //else if( cFormat.OwnerRowFormat != null && !cFormat.OwnerRowFormat.Paddings.IsDefault )
            //  BuildPadding( cFormat.OwnerRowFormat.Paddings, false );

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the Table row borders.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <returns></returns>
        private string BuildTRowBorders(Borders borders)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(c_slashSymbol + "trbrdrt");
            strBuilder.Append(BuildBorder(borders.Top, true));
            strBuilder.Append(c_slashSymbol + "trbrdrb");
            strBuilder.Append(BuildBorder(borders.Bottom, true));
            strBuilder.Append(c_slashSymbol + "trbrdrl");
            strBuilder.Append(BuildBorder(borders.Left, true));
            strBuilder.Append(c_slashSymbol + "trbrdrr");
            strBuilder.Append(BuildBorder(borders.Right, true));
            strBuilder.Append(c_slashSymbol + "trbrdrh");
            strBuilder.Append(BuildBorder(borders.Horizontal, true));
            strBuilder.Append(c_slashSymbol + "trbrdrv");
            strBuilder.Append(BuildBorder(borders.Vertical, true));

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the Table cell borders.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="borders">The cell borders.</param>
        /// <param name="rowBorders">The row borders.</param>
        /// <returns></returns>
        private string BuildTCellBorders(WTableCell cell, Borders borders, Borders rowBorders)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(c_slashSymbol + "clbrdrt");
            if (CheckCellBorders(cell, BorderType.Top) && borders.Top.HasNoneStyle)
                strBuilder.Append(BuildBorder((cell.OwnerRow as WTableRow).RowFormat.Borders.Horizontal, true));
            else if (!borders.Top.HasNoneStyle)
                strBuilder.Append(BuildBorder(borders.Top, true));
            else if (rowBorders != null)
                strBuilder.Append(BuildBorder(rowBorders.Top, true));

            strBuilder.Append(c_slashSymbol + "clbrdrb");
            if (CheckCellBorders(cell, BorderType.Bottom) && borders.Bottom.HasNoneStyle)
                strBuilder.Append(BuildBorder((cell.OwnerRow as WTableRow).RowFormat.Borders.Horizontal, true));
            else if (!borders.Bottom.HasNoneStyle)
                strBuilder.Append(BuildBorder(borders.Bottom, true));
            else if (rowBorders != null)
                strBuilder.Append(BuildBorder(rowBorders.Bottom, true));

            strBuilder.Append(c_slashSymbol + "clbrdrl");
            if (CheckCellBorders(cell, BorderType.Left) && borders.Left.HasNoneStyle)
                strBuilder.Append(BuildBorder((cell.OwnerRow as WTableRow).RowFormat.Borders.Vertical, true));
            else if (!borders.Left.HasNoneStyle)
                strBuilder.Append(BuildBorder(borders.Left, true));
            else if (rowBorders != null)
                strBuilder.Append(BuildBorder(rowBorders.Left, true));

            strBuilder.Append(c_slashSymbol + "clbrdrr");
            if (CheckCellBorders(cell, BorderType.Right) && borders.Right.HasNoneStyle)
                strBuilder.Append(BuildBorder((cell.OwnerRow as WTableRow).RowFormat.Borders.Vertical, true));
            else if (!borders.Right.HasNoneStyle)
                strBuilder.Append(BuildBorder(borders.Right, true));
            else if (rowBorders != null)
                strBuilder.Append(BuildBorder(rowBorders.Right, true));

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the table cell.
        /// </summary>
        /// <param name="cell">The table cell.</param>
        private byte[] BuildTableCell(WTableCell cell)
        {
            MemoryStream cellStream = new MemoryStream();
            byte[] byteArr;
            byteArr = BuildBodyItems(cell.Items);
            cellStream.Write(byteArr, 0, byteArr.Length);
            if (m_tableNestedLevel > 1)
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "nestcell}");
                cellStream.Write(byteArr, 0, byteArr.Length);
            }
            else
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "cell}");
                cellStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            cellStream.Write(byteArr, 0, byteArr.Length);
            return cellStream.ToArray();
        }
        /// <summary>
        /// Builds the padding.
        /// </summary>
        /// <param name="paddings">The paddings.</param>
        /// <param name="isRow">if set to <c>true</c> [is row].</param>
        /// <returns></returns>
        private string BuildPadding(Paddings paddings, bool isRow)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (isRow)
            {
                strBuilder.Append(c_slashSymbol + "trpaddl");
                strBuilder.Append((int)Math.Round(paddings.Left * RtfNavigator.c_twentiethOfPoint));

                strBuilder.Append(c_slashSymbol + "trpaddt");
                strBuilder.Append((int)Math.Round(paddings.Top * RtfNavigator.c_twentiethOfPoint));

                strBuilder.Append(c_slashSymbol + "trpaddb");
                strBuilder.Append((int)Math.Round(paddings.Bottom * RtfNavigator.c_twentiethOfPoint));

                strBuilder.Append(c_slashSymbol + "trpaddr");
                strBuilder.Append((int)Math.Round(paddings.Right * RtfNavigator.c_twentiethOfPoint));

                //Units for @"\trpaddl", @"\trpaddt", @"\trpaddb", @"\trpaddr" - Twips
                strBuilder.Append(c_slashSymbol + "trpaddfb3");
                strBuilder.Append(c_slashSymbol + "trpaddfl3");
                strBuilder.Append(c_slashSymbol + "trpaddfr3");
                strBuilder.Append(c_slashSymbol + "trpaddft3");
            }
            else
            {
                strBuilder.Append(c_slashSymbol + "clpadl");
                strBuilder.Append((int)Math.Round(paddings.Top * RtfNavigator.c_twentiethOfPoint));

                strBuilder.Append(c_slashSymbol + "clpadt");
                strBuilder.Append((int)Math.Round(paddings.Left * RtfNavigator.c_twentiethOfPoint));

                strBuilder.Append(c_slashSymbol + "clpadb");
                strBuilder.Append((int)Math.Round(paddings.Bottom * RtfNavigator.c_twentiethOfPoint));

                strBuilder.Append(c_slashSymbol + "clpadr");
                strBuilder.Append((int)Math.Round(paddings.Right * RtfNavigator.c_twentiethOfPoint));

                //Units for @"\clpadl", @"\clpadr", @"\clpadt", @"\clpadb" - Twips
                strBuilder.Append(c_slashSymbol + "clpadfl3");
                strBuilder.Append(c_slashSymbol + "clpadft3");
                strBuilder.Append(c_slashSymbol + "clpadfb3");
                strBuilder.Append(c_slashSymbol + "clpadfr3");
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the vertical alignment.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        private string BuildVertAlignment(VerticalAlignment alignment)
        {
            if (alignment == VerticalAlignment.Top)
                return c_slashSymbol + "clvertalt";
            else if (alignment == VerticalAlignment.Middle)
                return c_slashSymbol + "clvertalc";
            else if (alignment == VerticalAlignment.Bottom)
                return c_slashSymbol + "clvertalb";
            else
                return string.Empty;
        }
        /// <summary>
        /// Checks the cell borders.
        /// </summary>
        /// <param name="cellBorders">The table cell.</param>
        /// <param name="borderType">Type of the border.</param>
        /// <returns></returns>
        private bool CheckCellBorders(WTableCell cell, BorderType borderType)
        {
            WTableRow row = cell.OwnerRow as WTableRow;
            switch (borderType)
            {
                case BorderType.Top:
                    if (row.PreviousSibling is WTableRow)
                        return true;
                    break;
                case BorderType.Bottom:
                    if (row.NextSibling is WTableRow)
                        return true;
                    break;
                case BorderType.Left:
                    if (cell.PreviousSibling is WTableCell)
                        return true;
                    break;
                case BorderType.Right:
                    if (cell.NextSibling is WTableCell)
                        return true;
                    break;
            }
            return false;
        }
        #endregion

        #region Implementation / paragraph items
        /// <summary>
        /// Builds the paragraph item.
        /// </summary>
        /// <param name="item">The paragraph item.</param>
        private byte[] BuildParagraphItem(ParagraphItem item)
        {
            MemoryStream paraItemStream = new MemoryStream();
            byte[] byteArr;

            StringBuilder strBuilder = new StringBuilder();
            switch (item.EntityType)
            {
                case EntityType.TextRange:
                    byteArr = m_encoding.GetBytes(BuildTextRange(item as WTextRange));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.BookmarkStart:
                    byteArr = InsertBkmkStart(item as BookmarkStart);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.BookmarkEnd:
                    byteArr = InsertBkmkEnd(item as BookmarkEnd);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.Break:
                    byteArr = InsertLineBreak(item as Break);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.Field:
                    byteArr = m_encoding.GetBytes(BuildField(item as WField));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.FieldMark:
                    byteArr = m_encoding.GetBytes(BuildFieldMark(item as WFieldMark));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.Picture:
                    if ((item as WPicture).ImageRecord != null)
                    {
                        byteArr = m_encoding.GetBytes(BuildPicture(item as WPicture));
                        paraItemStream.Write(byteArr, 0, byteArr.Length);
                    }
                    break;
                case EntityType.TextBox:
                    byteArr = BuildTextBox(item as WTextBox);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.Footnote:
                    byteArr = BuildFootnoteEndnote(item as WFootnote);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.Symbol:
                    byteArr = BuildSymbol(item as WSymbol);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.TOC:
                    byteArr = BuildTocField(item as TableOfContent);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.CheckBox:
                    byteArr = m_encoding.GetBytes(BuildCheckBox(item as WCheckBox));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.TextFormField:
                    byteArr = m_encoding.GetBytes(BuildTextFormField(item as WTextFormField));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.DropDownFormField:
                    byteArr = m_encoding.GetBytes(BuildDropDownField(item as WDropDownFormField));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.CommentMark:
                    byteArr = BuildCommentMark(item as WCommentMark);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.Comment:
                    byteArr = BuildComment(item as WComment);
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.MergeField:
                    byteArr = m_encoding.GetBytes(BuildMergeField(item as WMergeField));
                    paraItemStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case EntityType.StructureDocumentTagInline:
                    ParagraphItemCollection paraItems = (item as StructureDocumentTagInline).SDTContent.ParagraphItems;
                    for (int i = 0; i < paraItems.Count; i++)
                    {
                        byteArr = BuildParagraphItem(paraItems[i]);
                        paraItemStream.Write(byteArr, 0, byteArr.Length);
                    }
                    break;
            }
            return paraItemStream.ToArray();
        }
        /// <summary>
        /// Builds the merge field.
        /// </summary>
        /// <param name="mField">The merge field.</param>
        private string BuildMergeField(WMergeField mField)
        {
            if (mField.ConvertedToText && mField.Text != null)
            {
                return BuildTextRange(mField as WTextRange);
            }
            else
            {
                StringBuilder strBuilder = new StringBuilder();

                strBuilder.Append(@"{" + c_slashSymbol + "field");
                strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst{");
                strBuilder.Append(BuildCharacterFormat(mField.CharacterFormat));
                strBuilder.Append(@" ");
                strBuilder.Append(BuildFieldType(mField.FieldType));

                if (mField.FieldValue != string.Empty)
                    strBuilder.Append(mField.FieldValue);
                else
                    strBuilder.Append(mField.FieldName);

                strBuilder.Append(@" " + c_slashSymbol + c_slashSymbol + "*" + mField.FormattingString);
                strBuilder.Append(@"}}");

                strBuilder.Append(@"{" + c_slashSymbol + "fldrslt{");
                string fieldText = mField.TextBefore + mField.Text + mField.TextAfter;
                strBuilder.Append(BuildTextRangeStr((mField as WTextRange).CharacterFormat, fieldText));
                strBuilder.Append(@"}}");
                strBuilder.Append(@"}");

                return strBuilder.ToString();
            }
        }
        /// <summary>
        /// Builds the symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        private byte[] BuildSymbol(WSymbol symbol)
        {
            MemoryStream symbolStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{");
            symbolStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildCharacterFormat(symbol.CharacterFormat));
            symbolStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "field{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst{ SYMBOL ");
            symbolStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(symbol.CharacterCode.ToString());
            symbolStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@" " + c_slashSymbol + c_slashSymbol + "f " + c_slashSymbol + symbol.FontName);
            symbolStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "fldrslt}");
            symbolStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}}}}");
            symbolStream.Write(byteArr, 0, byteArr.Length);

            return symbolStream.ToArray();
        }
        /// <summary>
        /// Builds the footnote/endnote.
        /// </summary>
        /// <param name="footnote">The footnote.</param>
        private byte[] BuildFootnoteEndnote(WFootnote footnote)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            if (string.IsNullOrEmpty(footnote.CustomMarker) && !footnote.CustomMarkerIsSymbol)
            {
                byteArr = m_encoding.GetBytes(BuildCharacterFormat(footnote.ParaItemCharFormat));
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(c_slashSymbol + "chftn");
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            else if (footnote.CustomMarkerIsSymbol)
            {
                byteArr = m_encoding.GetBytes(BuildCharacterFormat(footnote.MarkerCharacterFormat));
                memStream.Write(byteArr, 0, byteArr.Length);
                WField field = new WField(m_doc);
                field.FieldType = FieldType.FieldSymbol;
                field.m_fieldValue = footnote.SymbolCode.ToString();
                field.m_formattingString = c_slashSymbol + c_slashSymbol + "f Symbol";
                byteArr = m_encoding.GetBytes(BuildField(field));
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}");
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            else
            {
                byteArr = m_encoding.GetBytes(BuildCharacterFormat(footnote.MarkerCharacterFormat));
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(" " + footnote.CustomMarker);
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "footnote");
            memStream.Write(byteArr, 0, byteArr.Length);
            if (footnote.FootnoteType == FootnoteType.Endnote)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "ftnalt");
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            if (string.IsNullOrEmpty(footnote.CustomMarker) && !footnote.CustomMarkerIsSymbol)
            {
                byteArr = m_encoding.GetBytes(BuildCharacterFormat(footnote.MarkerCharacterFormat));
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(c_slashSymbol + "chftn");
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = BuildBodyItems(footnote.TextBody.Items);
            memStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(@"}}");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);

            if (footnote.FootnoteType == FootnoteType.Footnote)
            {
                byteArr = m_encoding.GetBytes(BuildFootnoteProp());
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            else
            {
                byteArr = m_encoding.GetBytes(BuildEndnoteProp());
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            return memStream.ToArray();
        }
        /// <summary>
        /// Builds the footnote property.
        /// </summary>
        /// <param name="footnote">The footnote.</param>
        private string BuildFootnoteProp()
        {
            if (m_hasFootnote)
                return string.Empty;

            m_hasFootnote = true;
            StringBuilder strBuilder = new StringBuilder();

            if (m_doc.RestartIndexForFootnotes == FootnoteRestartIndex.DoNotRestart)
                strBuilder.Append(c_slashSymbol + "sftnrstcont");
            else if (m_doc.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachPage)
                strBuilder.Append(c_slashSymbol + "sftnrstpg");
            else if (m_doc.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachSection)
                strBuilder.Append(c_slashSymbol + "sftnrestart");

            strBuilder.Append(c_slashSymbol + "sftnstart");
            strBuilder.Append(m_doc.InitialFootnoteNumber);

            switch (m_doc.FootnoteNumberFormat)
            {
                case FootEndNoteNumberFormat.Arabic:
                    strBuilder.Append(c_slashSymbol + "sftnnar");
                    break;
                case FootEndNoteNumberFormat.LowerCaseLetter:
                    strBuilder.Append(c_slashSymbol + "sftnnalc");
                    break;
                case FootEndNoteNumberFormat.LowerCaseRoman:
                    strBuilder.Append(c_slashSymbol + "sftnnrlc");
                    break;
                case FootEndNoteNumberFormat.UpperCaseLetter:
                    strBuilder.Append(c_slashSymbol + "sftnnauc");
                    break;
                case FootEndNoteNumberFormat.UpperCaseRoman:
                    strBuilder.Append(c_slashSymbol + "sftnnruc");
                    break;
                default:
                    strBuilder.Append(c_slashSymbol + "sftnnchi");
                    break;
            }

            if (m_doc.FootnotePosition == FootnotePosition.PrintAtBottomOfPage)
                strBuilder.Append(c_slashSymbol + "ftnbj");
            else if (m_doc.FootnotePosition == FootnotePosition.PrintImmediatelyBeneathText)
                strBuilder.Append(c_slashSymbol + "ftntj");
            else if (m_doc.FootnotePosition == FootnotePosition.PrintAsEndnotes)
            {
                if (m_doc.EndnotePosition == EndnotePosition.DisplayEndOfDocument)
                    strBuilder.Append(c_slashSymbol + "enddoc");
                else if (m_doc.EndnotePosition == EndnotePosition.DisplayEndOfSection)
                    strBuilder.Append(c_slashSymbol + "endnotes");
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the endnote property.
        /// </summary>
        private string BuildEndnoteProp()
        {
            if (m_hasEndnote)
                return string.Empty;

            m_hasEndnote = true;
            StringBuilder strBuilder = new StringBuilder();

            if (m_doc.RestartIndexForEndnote == EndnoteRestartIndex.DoNotRestart)
                strBuilder.Append(c_slashSymbol + "saftnrstcont");
            else if (m_doc.RestartIndexForEndnote == EndnoteRestartIndex.RestartForEachSection)
                strBuilder.Append(c_slashSymbol + "saftnrestart");

            strBuilder.Append(c_slashSymbol + "saftnstart");
            strBuilder.Append(m_doc.InitialEndnoteNumber);

            switch (m_doc.EndnoteNumberFormat)
            {
                case FootEndNoteNumberFormat.Arabic:
                    strBuilder.Append(c_slashSymbol + "saftnnar");
                    break;
                case FootEndNoteNumberFormat.LowerCaseLetter:
                    strBuilder.Append(c_slashSymbol + "saftnnalc");
                    break;
                case FootEndNoteNumberFormat.LowerCaseRoman:
                    strBuilder.Append(c_slashSymbol + "saftnnrlc");
                    break;
                case FootEndNoteNumberFormat.UpperCaseLetter:
                    strBuilder.Append(c_slashSymbol + "saftnnauc");
                    break;
                case FootEndNoteNumberFormat.UpperCaseRoman:
                    strBuilder.Append(c_slashSymbol + "saftnnruc");
                    break;
            }

            if (m_doc.EndnotePosition == EndnotePosition.DisplayEndOfDocument)
                strBuilder.Append(c_slashSymbol + "aenddoc");
            else if (m_doc.EndnotePosition == EndnotePosition.DisplayEndOfSection)
                strBuilder.Append(c_slashSymbol + "aendnotes");

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the field mark.
        /// </summary>
        /// <param name="fieldMark">The field mark.</param>
        private string BuildFieldMark(WFieldMark fieldMark)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (m_currentField == null || CurrentField.Count == 0)
                return string.Empty;

            if (fieldMark.Type == FieldMarkType.FieldSeparator)
            {
                strBuilder.Append(@"{" + c_slashSymbol + "fldrslt");
            }
            else if (!(fieldMark.PreviousSibling is WField))
            {
                strBuilder.Append(@"}}");
                CurrentField.Pop();
            }
            else if (WriteFieldEnd(fieldMark))
            {
                strBuilder.Append(@"{" + c_slashSymbol + "fldrslt}}");
                CurrentField.Pop();
            }
            else
            {
                strBuilder.Append(@"}");
                CurrentField.Pop();
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the field.
        /// </summary>
        /// <param name="field">The field.</param>
        private string BuildField(WField field)
        {
            if (field.FieldEnd == null && field.FieldType == FieldType.FieldUnknown)
            {
                WTextRange textRamge = new WTextRange(m_doc);
                textRamge.ApplyCharacterFormat(field.CharacterFormat);
                textRamge.Text = field.FieldCode;
                return BuildTextRange(textRamge);
            }
            else
            {
                CurrentField.Push(field);

                StringBuilder strBuilder = new StringBuilder();

                strBuilder.Append(@"{" + c_slashSymbol + "field");
                strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst{");
                strBuilder.Append(BuildCharacterFormat(field.CharacterFormat));
                strBuilder.Append(@" ");
                if (!string.IsNullOrEmpty(field.FieldCode))
                {
                    field.FieldCode = field.FieldCode.Replace(c_symbol92, c_symbol92 + c_symbol92);
                    strBuilder.Append(PrepareText(field.FieldCode));
                }
                else
                {
                    strBuilder.Append(BuildFieldType(field.FieldType));
                    strBuilder.Append(field.FieldValue);
                    if (field.FieldType == FieldType.FieldTime)
                        field.m_formattingString = c_slashSymbol + field.m_formattingString.Trim();
                    strBuilder.Append(field.FormattingString);
                }
                strBuilder.Append(@"}}");

                return strBuilder.ToString();
            }
        }
        /// <summary>
        /// Inserts the line break.
        /// </summary>
        /// <param name="brk">The Break.</param>
        private byte[] InsertLineBreak(Break brk)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildCharacterFormat(brk.TextRange.CharacterFormat));
            memStream.Write(byteArr, 0, byteArr.Length);
            switch (brk.BreakType)
            {
                case BreakType.LineBreak:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "line");
            memStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case BreakType.PageBreak:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "page");
            memStream.Write(byteArr, 0, byteArr.Length);
                    break;
                case BreakType.ColumnBreak:
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "column");
            memStream.Write(byteArr, 0, byteArr.Length);
                    break;
            }
            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            return memStream.ToArray();
        }
        /// <summary>
        /// Builds the text range.
        /// </summary>
        /// <param name="textRange">The text range.</param>
        private string BuildTextRange(WTextRange textRange)
        {
            textRange.Text = textRange.Text.Replace(DEF_FOOTNOTE_SYMBOL.ToString(), string.Empty);
            if (textRange.Text == string.Empty)
                return string.Empty;

            string text = UpdateText(textRange.Text);

            StringBuilder strBuilder = new StringBuilder();

            char symbol = (char)((byte)13);
            if (text.IndexOf(symbol) == -1)
            {
                strBuilder.Append(BuildTextRangeStr(textRange.CharacterFormat, PrepareText(text)));
            }
            else
            {
                WCharacterFormat breakCFormat = (textRange.OwnerParagraph == null)
                  ? m_doc.DefCharFormat : textRange.OwnerParagraph.BreakCharacterFormat;
                string textToWrite = string.Empty;
                do
                {
                    int index = text.IndexOf(symbol);
                    if (index == 0)
                    {
                        strBuilder.Append(BuildTextRangeStr(breakCFormat, c_slashSymbol + "par"));
                        text = text.Remove(0, 1);
                    }
                    else
                    {
                        if (index == -1)
                            index = text.Length;
                        textToWrite = text.Substring(0, index);
                        strBuilder.Append(BuildTextRangeStr(textRange.CharacterFormat, PrepareText(textToWrite)));
                        text = text.Remove(0, index);
                    }
                } while (text.Length != 0);
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Inserts the Bookmark end.
        /// </summary>
        /// <param name="bbkmrStart">The bookmark end.</param>
        private byte[] InsertBkmkEnd(BookmarkEnd bkmkEnd)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "*" + c_slashSymbol + "bkmkend ");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(bkmkEnd.Name);
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);

            return memStream.ToArray();
        }
        /// <summary>
        /// Inserts the Bookmark start.
        /// </summary>
        /// <param name="bbkmrStart">The bookmark start.</param>
        private byte[] InsertBkmkStart(BookmarkStart bkmkStart)
        {
            StringBuilder strBuilder = new StringBuilder();
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "*" + c_slashSymbol + "bkmkstart ");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(bkmkStart.Name);
            memStream.Write(byteArr, 0, byteArr.Length);
            if (bkmkStart.ColumnFirst >= 0)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "bkmkcolf" + bkmkStart.ColumnFirst);
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            if (bkmkStart.ColumnLast >= 0)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "bkmkcoll" + (bkmkStart.ColumnLast + 1));
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);

            return memStream.ToArray();
        }
        /// <summary>
        /// Builds the toc field.
        /// </summary>
        /// <param name="toc">The toc.</param>
        private byte[] BuildTocField(TableOfContent toc)
        {
            CurrentField.Push(toc);
            MemoryStream tocFieldStream = new MemoryStream();
            byte[] byteArr;

            byteArr = m_encoding.GetBytes(Environment.NewLine);
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "field");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            // Need, A formatting change has been made to the field result since the field was last updated
            byteArr = m_encoding.GetBytes(c_slashSymbol + "flddirty");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildCharacterFormat(toc.TOCField.CharacterFormat));
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(" TOC ");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(toc.TOCField.FieldValue);
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(toc.FormattingString.Replace(c_slashSymbol, c_slashSymbol + c_slashSymbol));
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}");
            tocFieldStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            tocFieldStream.Write(byteArr, 0, byteArr.Length);

            return tocFieldStream.ToArray();
        }
        #endregion

        #region Implementation / picture
        /// <summary>
        /// Builds the picture.
        /// </summary>
        /// <param name="pic">The picture.</param>
        private string BuildPicture(WPicture pic)
        {
            if (pic.TextWrappingStyle == TextWrappingStyle.Inline)
                return BuildInLineImage(pic);
            else
                return BuildShapeImage(pic);
        }
        /// <summary>
        /// Builds the shape image.
        /// </summary>
        /// <param name="pic">The picture.</param>
        private string BuildShapeImage(WPicture pic)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{");
            strBuilder.Append(BuildCharacterFormat(pic.PictureCharacterFormat));
            strBuilder.Append(@"{" + c_slashSymbol + "shp");
            strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "shpinst");

            int width = (int)Math.Round(pic.Width * RtfNavigator.c_twentiethOfPoint * pic.WidthScale / 100);
            int height = (int)Math.Round(pic.Height * RtfNavigator.c_twentiethOfPoint * pic.HeightScale / 100);
            strBuilder.Append(BuildShapePosition(pic.HorizontalPosition, pic.VerticalPosition, width, height));

            if (pic.IsHeaderPicture)
                strBuilder.Append(c_slashSymbol + "shpfhdr1");
            else
                strBuilder.Append(c_slashSymbol + "shpfhdr0");

            if (pic.HorizontalOrigin == HorizontalOrigin.Page)
                strBuilder.Append(c_slashSymbol + "shpbxpage");
            else if (pic.HorizontalOrigin == HorizontalOrigin.Margin || pic.HorizontalOrigin == HorizontalOrigin.LeftMargin
                || pic.HorizontalOrigin == HorizontalOrigin.RightMargin || pic.HorizontalOrigin == HorizontalOrigin.InsideMargin 
                || pic.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                strBuilder.Append(c_slashSymbol + "shpbxmargin");
            else if (pic.HorizontalOrigin == HorizontalOrigin.Column)
                strBuilder.Append(c_slashSymbol + "shpbxcolumn");
            //strBuilder.Append( @"\shpbxignore" );

            if (pic.VerticalOrigin == VerticalOrigin.Page)
                strBuilder.Append(c_slashSymbol + "shpbypage");
            else if (pic.VerticalOrigin == VerticalOrigin.Margin)
                strBuilder.Append(c_slashSymbol + "shpbymargin");
            else if (pic.VerticalOrigin == VerticalOrigin.Paragraph)
                strBuilder.Append(c_slashSymbol + "shpbypara");
            //strBuilder.Append( @"\shpbyignore" );

            strBuilder.Append(BuildWrappingStyle(pic.TextWrappingStyle, pic.TextWrappingType));
            strBuilder.Append(BuildShapeProp("shapeType", "75"));
            strBuilder.Append(BuildShapeProp("pib", BuildPictureProp(pic)));

            if (pic.PictureShape.PictureDescriptor.BorderTop.IsDefault &&
              pic.PictureShape.PictureDescriptor.BorderLeft.IsDefault &&
              pic.PictureShape.PictureDescriptor.BorderRight.IsDefault &&
              pic.PictureShape.PictureDescriptor.BorderBottom.IsDefault)
            {
                strBuilder.Append(BuildShapeProp("fLine", "0"));
            }
            else
            {
                strBuilder.Append(BuildShapeProp("fLine", "1"));
            }

            strBuilder.Append(BuildHorAlignm(pic.HorizontalAlignment));
            strBuilder.Append(BuildHorPos(pic.HorizontalOrigin));
            strBuilder.Append(BuildVertAlignm(pic.VerticalAlignment));
            strBuilder.Append(BuildVertPos(pic.VerticalOrigin));

            strBuilder.Append(@"}}}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the Inline Image.
        /// </summary>
        /// <param name="pic">The picture.</param>
        private string BuildInLineImage(WPicture pic)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{");
            strBuilder.Append(BuildCharacterFormat(pic.PictureCharacterFormat));

            if (pic.IsMetaFile && IsWmfImage(pic))
            {
                strBuilder.Append(BuildMetafileProp(pic));
            }
            else
            {
                strBuilder.Append(@"{" + c_slashSymbol + "shppict");
                strBuilder.Append(BuildPictureProp(pic));
                strBuilder.Append(@"}");

                strBuilder.Append(@"{" + c_slashSymbol + "nonshppict");
                strBuilder.Append(BuildMetafileProp(pic));
                strBuilder.Append(@"}");
            }


            strBuilder.Append(@"}");
            return strBuilder.ToString();
        }
        /// <summary>
        /// Determine whether the image is WMF format
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private bool IsWmfImage(WPicture picture)
        {
            return (picture.ImageRecord.ImageFormat == ImageFormat.Wmf
#if !SILVERLIGHT && !WP
 || picture.ImageRecord.ImageFormat.Guid.ToString() == "B96B3CAD-0728-11D3-9D7B-0000F81EF32E"
#endif
 || (picture.PictureShape != null && picture.PictureShape.ShapeContainer != null
                   && picture.PictureShape.ShapeContainer.Bse != null && picture.PictureShape.ShapeContainer.Bse.Blip != null
                   && picture.PictureShape.ShapeContainer.Bse.Blip != null && picture.PictureShape.ShapeContainer.Bse.Blip.Header != null
                   && picture.PictureShape.ShapeContainer.Bse.Blip.Header.Type == Syncfusion.DocIO.ReaderWriter.Escher.MSOFBT.msofbtBlipWMF));
        }
        /// <summary>
        /// Builds the picture property.
        /// </summary>
        /// <param name="pic">The picture.</param>
        /// <returns></returns>
        private string BuildPictureProp(WPicture pic)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"{" + c_slashSymbol + "pict");
            strBuilder.Append(c_slashSymbol + "picscalex");
            strBuilder.Append(Math.Round(pic.WidthScale));
            strBuilder.Append(c_slashSymbol + "picscaley");
            strBuilder.Append(Math.Round(pic.HeightScale));
            strBuilder.Append(c_slashSymbol + "picwgoal");
            strBuilder.Append(Math.Round(pic.Width * RtfNavigator.c_twentiethOfPoint));
            strBuilder.Append(c_slashSymbol + "pichgoal");
            strBuilder.Append(Math.Round(pic.Height * RtfNavigator.c_twentiethOfPoint));
            strBuilder.Append(c_slashSymbol + "picw");
            strBuilder.Append(Math.Round(pic.Width * RtfNavigator.c_thirtyfive));
            strBuilder.Append(c_slashSymbol + "pich");
            strBuilder.Append(Math.Round(pic.Height * RtfNavigator.c_thirtyfive));

            if (pic.IsMetaFile)
                strBuilder.Append(c_slashSymbol + "wmetafile8");
            else
                strBuilder.Append(c_slashSymbol + "jpegblip");

            strBuilder.Append(@" ");
            byte[] imageBytes = pic.ImageBytes;
            StringBuilder imageData = new StringBuilder(BitConverter.ToString(imageBytes));
            imageData.Replace("-", "");
            strBuilder.Append(imageData);
            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the metafile properties.
        /// </summary>
        /// <param name="pic">The picture.</param>
        /// <returns></returns>
        private string BuildMetafileProp(WPicture pic)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"{" + c_slashSymbol + "pict");
            strBuilder.Append(c_slashSymbol + "picscalex");
            strBuilder.Append(Math.Round(pic.WidthScale));
            strBuilder.Append(c_slashSymbol + "picscaley");
            strBuilder.Append(Math.Round(pic.HeightScale));
            strBuilder.Append(c_slashSymbol + "picwgoal");
            strBuilder.Append(Math.Round(pic.Width * RtfNavigator.c_twentiethOfPoint));
            strBuilder.Append(c_slashSymbol + "pichgoal");
            strBuilder.Append(Math.Round(pic.Height * RtfNavigator.c_twentiethOfPoint));
            strBuilder.Append(c_slashSymbol + "picw");
            strBuilder.Append(Math.Round(pic.Width * RtfNavigator.c_thirtyfive));
            strBuilder.Append(c_slashSymbol + "pich");
            strBuilder.Append(Math.Round(pic.Height * RtfNavigator.c_thirtyfive));

            strBuilder.Append(c_slashSymbol + "wmetafile8");
            strBuilder.Append(@" ");

            byte[] bytes = null;

            if (pic.IsMetaFile && IsWmfImage(pic))
            {
                bytes = pic.ImageBytes;
            }
            else
            {
#if AllowUnsafeCode
                bytes = GetRtfImage(pic.Image);
#endif
            }

            if (bytes != null)
            {
                StringBuilder imageData = new StringBuilder(BitConverter.ToString(bytes));
                imageData.Replace("-", "");
                strBuilder.Append(imageData);
            }

            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the RTF image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        private byte[] GetRtfImage(Image image)
        {
            MemoryStream stream = null;
            Graphics graphics = null;
            Metafile metaFile = null;
            IntPtr hdc;

            try
            {
                stream = new MemoryStream();
                Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

                using (graphics = Graphics.FromImage(bitmap))
                {
                    hdc = graphics.GetHdc();
                    metaFile = new Metafile(stream, hdc);
                    graphics.ReleaseHdc(hdc);
                }

                using (graphics = Graphics.FromImage(metaFile))
                    graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height));


                IntPtr hEmf = metaFile.GetHenhmetafile();
                uint _bufferSize = GdipEmfToWmfBits(hEmf, 0, null, MM_ANISOTROPIC,
                  EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                byte[] _buffer = new byte[_bufferSize];
                uint _convertedSize = GdipEmfToWmfBits(hEmf, _bufferSize, _buffer, MM_ANISOTROPIC,
                  EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

                return _buffer;
            }
            finally
            {
                if (graphics != null)
                    graphics.Dispose();
                if (metaFile != null)
                    metaFile.Dispose();
                if (stream != null)
                    stream.Close();
            }
        }
        /// <summary>
        /// Use the EmfToWmfBits function in the GDI+ specification to convert a 
        /// Enhanced Metafile to a Windows Metafile
        /// </summary>
        /// <param name="hEmf">
        /// A handle to the Enhanced Metafile to be converted
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer used to store the Windows Metafile bits returned
        /// </param>
        /// <param name="buffer">
        /// An array of bytes used to hold the Windows Metafile bits returned
        /// </param>
        /// <param name="mappingMode">
        /// The mapping mode of the image.  This control uses MM_ANISOTROPIC.
        /// </param>
        /// <param name="flags">
        /// Flags used to specify the format of the Windows Metafile returned
        /// </param>
        [DllImportAttribute("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr hEmf, uint bufferSize,
          byte[] buffer, int mappingMode, EmfToWmfBitsFlags flags);
#endif
        /// <summary>
        /// Builds the wrapping style.
        /// </summary>
        /// <param name="style">The wrapping style.</param>
        /// <param name="type">The wrapping type.</param>
        /// <returns></returns>
        private string BuildWrappingStyle(TextWrappingStyle style, TextWrappingType type)
        {
            switch (style)
            {
                case TextWrappingStyle.TopAndBottom:
                    return c_slashSymbol + "shpwr1";
                case TextWrappingStyle.Square:
                    return c_slashSymbol + "shpwr2" + BuildWrappingType(type);
                case TextWrappingStyle.Tight:
                    return c_slashSymbol + "shpwr4" + BuildWrappingType(type);
                case TextWrappingStyle.Through:
                    return c_slashSymbol + "shpwr5";
                case TextWrappingStyle.InFrontOfText:
                    return c_slashSymbol + "shpfblwtxt0";
                case TextWrappingStyle.Behind:
                    return c_slashSymbol + "shpfblwtxt1";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the wrapping type.
        /// </summary>
        /// <param name="type">The text wrapping type.</param>
        /// <returns></returns>
        private string BuildWrappingType(TextWrappingType type)
        {
            switch (type)
            {
                case TextWrappingType.Both:
                    return c_slashSymbol + "shpwrk0";
                case TextWrappingType.Left:
                    return c_slashSymbol + "shpwrk1";
                case TextWrappingType.Right:
                    return c_slashSymbol + "shpwrk2";
                case TextWrappingType.Largest:
                    return c_slashSymbol + "shpwrk3";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the shape property.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="propValue">The property value.</param>
        /// <returns></returns>
        private string BuildShapeProp(string propName, string propValue)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"{" + c_slashSymbol + "sp");
            strBuilder.Append(@"{" + c_slashSymbol + "sn ");
            strBuilder.Append(propName);
            strBuilder.Append(@"}");
            strBuilder.Append(@"{" + c_slashSymbol + "sv ");
            strBuilder.Append(propValue);
            strBuilder.Append(@"}}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the horizontal alignment.
        /// </summary>
        /// <param name="hAlignm">The shape horizontal alignm.</param>
        /// <returns></returns>
        private string BuildHorAlignm(ShapeHorizontalAlignment hAlignm)
        {
            switch (hAlignm)
            {
                case ShapeHorizontalAlignment.Left:
                    return BuildShapeProp("posh", "1");
                case ShapeHorizontalAlignment.Center:
                    return BuildShapeProp("posh", "2");
                case ShapeHorizontalAlignment.Right:
                    return BuildShapeProp("posh", "3");
                case ShapeHorizontalAlignment.Inside:
                    return BuildShapeProp("posh", "4");
                case ShapeHorizontalAlignment.Outside:
                    return BuildShapeProp("posh", "5");
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the vertical alignment.
        /// </summary>
        /// <param name="vAlignm">The shape vertical alignm.</param>
        /// <returns></returns>
        private string BuildVertAlignm(ShapeVerticalAlignment vAlignm)
        {
            switch (vAlignm)
            {
                case ShapeVerticalAlignment.Top:
                    return BuildShapeProp("posv", "1");
                case ShapeVerticalAlignment.Center:
                    return BuildShapeProp("posv", "2");
                case ShapeVerticalAlignment.Bottom:
                    return BuildShapeProp("posv", "3");
                case ShapeVerticalAlignment.Inside:
                    return BuildShapeProp("posv", "4");
                case ShapeVerticalAlignment.Outside:
                    return BuildShapeProp("posv", "5");
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the horizontal position.
        /// </summary>
        /// <param name="hPos">The horizontal position.</param>
        /// <returns></returns>
        private string BuildHorPos(HorizontalOrigin hPos)
        {
            switch (hPos)
            {
                case HorizontalOrigin.Margin:
                    return BuildShapeProp("posrelh", "0");
                case HorizontalOrigin.Page:
                    return BuildShapeProp("posrelh", "1");
                case HorizontalOrigin.Column:
                    return BuildShapeProp("posrelh", "2");
                case HorizontalOrigin.Character:
                    return BuildShapeProp("posrelh", "3");
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the vertical position.
        /// </summary>
        /// <param name="vPos">The vertical position.</param>
        /// <returns></returns>
        private string BuildVertPos(VerticalOrigin vPos)
        {
            switch (vPos)
            {
                case VerticalOrigin.Margin:
                    return BuildShapeProp("posrelv", "0");
                case VerticalOrigin.Page:
                    return BuildShapeProp("posrelv", "1");
                case VerticalOrigin.Paragraph:
                    return BuildShapeProp("posrelv", "2");
                case VerticalOrigin.Line:
                    return BuildShapeProp("posrelv", "3");
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Builds the shape position.
        /// </summary>
        /// <param name="horPos">The horizontal position.</param>
        /// <param name="vertPos">The vertical position.</param>
        /// <param name="shapeW">The shape width * shape horizontal scale.</param>
        /// <param name="shapeH">The shape height * shape vertical scale.</param>
        /// <returns></returns>
        private string BuildShapePosition(float horPos, float vertPos, int shapeW, int shapeH)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (horPos == 0 && vertPos == 0)
            {
                strBuilder.Append(c_slashSymbol + "shpright");
                strBuilder.Append(shapeW);
                strBuilder.Append(c_slashSymbol + "shpbottom");
                strBuilder.Append(shapeH);
            }
            else
            {
                strBuilder.Append(c_slashSymbol + "shpleft");
                strBuilder.Append(horPos * RtfNavigator.c_twentiethOfPoint);
                strBuilder.Append(c_slashSymbol + "shptop");
                strBuilder.Append(vertPos * RtfNavigator.c_twentiethOfPoint);
                strBuilder.Append(c_slashSymbol + "shpright");
                strBuilder.Append(horPos * RtfNavigator.c_twentiethOfPoint + shapeW);
                strBuilder.Append(c_slashSymbol + "shpbottom");
                strBuilder.Append(vertPos * RtfNavigator.c_twentiethOfPoint + shapeH);
            }

            return strBuilder.ToString();
        }
        #endregion

        #region Implementation / textbox
        /// <summary>
        /// Builds the text box.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        private byte[] BuildTextBox(WTextBox textBox)
        {
            MemoryStream textBoxStream = new MemoryStream();
            byte[] byteArr;
            StringBuilder strBuilder = new StringBuilder();

            byteArr = m_encoding.GetBytes(@"{");
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildCharacterFormat((textBox.CharacterFormat)));
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "shp");
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "shpinst");
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildShapePosition(textBox.TextBoxFormat.HorizontalPosition, textBox.TextBoxFormat.VerticalPosition,
              (int)textBox.TextBoxFormat.Width * RtfNavigator.c_twentiethOfPoint,
              (int)textBox.TextBoxFormat.Height * RtfNavigator.c_twentiethOfPoint));
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            if (textBox.TextBoxFormat.IsHeaderTextBox)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpfhdr1");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            else
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpfhdr0");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }

            if (textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.Page)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpbxpage");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            else if (textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.Margin || textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.LeftMargin
                || textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.RightMargin || textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.InsideMargin
                || textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpbxmargin");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            else if (textBox.TextBoxFormat.HorizontalOrigin == HorizontalOrigin.Column)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpbxcolumn");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            //strBuilder.Append( @"\shpbxignore" );

            if (textBox.TextBoxFormat.VerticalOrigin == VerticalOrigin.Page)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpbypage");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            else if (textBox.TextBoxFormat.VerticalOrigin == VerticalOrigin.Margin)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpbymargin");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            else if (textBox.TextBoxFormat.VerticalOrigin == VerticalOrigin.Paragraph)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "shpbypara");
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }
            //strBuilder.Append( @"\shpbyignore" );

            byteArr = m_encoding.GetBytes(BuildWrappingStyle(textBox.TextBoxFormat.TextWrappingStyle,
             textBox.TextBoxFormat.TextWrappingType));
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(BuildShapeProp("shapeType", "202"));
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            if (textBox.TextBoxFormat.NoLine)
            {
                byteArr = m_encoding.GetBytes(BuildShapeProp("fLine", "0"));
                textBoxStream.Write(byteArr, 0, byteArr.Length);
            }

            byteArr = m_encoding.GetBytes(BuildShapeLines(textBox.TextBoxFormat.LineColor,
             textBox.TextBoxFormat.LineDashing, textBox.TextBoxFormat.LineWidth));
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(BuildTextBoxLineStyle(textBox.TextBoxFormat.LineStyle));
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(BuildShapeFill(textBox.TextBoxFormat.FillEfects, false));
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "shptxt");
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = BuildBodyItems(textBox.TextBoxBody.Items);
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(@"}}}}");
            textBoxStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            textBoxStream.Write(byteArr, 0, byteArr.Length);

            return textBoxStream.ToArray();
        }
        /// <summary>
        /// Builds the text box line style.
        /// </summary>
        /// <param name="style">The line style.</param>
        /// <returns></returns>
        private string BuildTextBoxLineStyle(TextBoxLineStyle style)
        {
            switch (style)
            {
                case TextBoxLineStyle.Double:
                    return BuildShapeProp("lineStyle", "1");
                case TextBoxLineStyle.ThickThin:
                    return BuildShapeProp("lineStyle", "2");
                case TextBoxLineStyle.ThinThick:
                    return BuildShapeProp("lineStyle", "3");
                case TextBoxLineStyle.Triple:
                    return BuildShapeProp("lineStyle", "4");
                default:
                    return BuildShapeProp("lineStyle", "0");
            }
        }
        /// <summary>
        /// Builds the shape lines.
        /// </summary>
        /// <param name="col">The line color.</param>
        /// <param name="dash">The line dashing.</param>
        /// <param name="lineWidth">Width of the line.</param>
        /// <returns></returns>
        private string BuildShapeLines(Color col, LineDashing dash, float lineWidth)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (!col.IsEmpty && col.Name != Color.Black.Name)
            {
                strBuilder.Append(BuildShapeProp("lineColor", GetRtfShapeColor(col)));
            }

            switch (dash)
            {
                case LineDashing.Solid:
                    strBuilder.Append(BuildShapeProp("lineDashing", "0"));
                    break;
                case LineDashing.Dash:
                    strBuilder.Append(BuildShapeProp("lineDashing", "1"));
                    break;
                case LineDashing.Dot:
                    strBuilder.Append(BuildShapeProp("lineDashing", "2"));
                    break;
                case LineDashing.DashDot:
                    strBuilder.Append(BuildShapeProp("lineDashing", "3"));
                    break;
                case LineDashing.DashDotDot:
                    strBuilder.Append(BuildShapeProp("lineDashing", "4"));
                    break;
                case LineDashing.DotGEL:
                    strBuilder.Append(BuildShapeProp("lineDashing", "6"));
                    break;
                case LineDashing.DashGEL:
                    strBuilder.Append(BuildShapeProp("lineDashing", "7"));
                    break;
                case LineDashing.LongDashGEL:
                    strBuilder.Append(BuildShapeProp("lineDashing", "8"));
                    break;
                case LineDashing.DashDotGEL:
                    strBuilder.Append(BuildShapeProp("lineDashing", "10"));
                    break;
                case LineDashing.LongDashDotGEL:
                    strBuilder.Append(BuildShapeProp("lineDashing", "9"));
                    break;
                case LineDashing.LongDashDotDotGEL:
                    strBuilder.Append(BuildShapeProp("lineDashing", "11"));
                    break;
            }
            //Nead investigation. Why 12700 ?
            strBuilder.Append(BuildShapeProp("lineWidth", (lineWidth * 12700).ToString()));

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the shape fill.
        /// </summary>
        /// <param name="col">The color.</param>
        /// <returns></returns>
        private string BuildShapeFill(Background backgr, bool isPageBackground)
        {
            StringBuilder strBuilder = new StringBuilder();

            switch (backgr.Type)
            {
                case BackgroundType.Color:
                    if (backgr.Color.IsEmpty || backgr.Color.Name == Color.White.Name)
                        return string.Empty;
                    if (isPageBackground)
                        strBuilder.Append(BuildShapeProp("fillColor", GetRtfPageBackgroundColor(backgr.Color)));
                    else
                        strBuilder.Append(BuildShapeProp("fillColor", GetRtfShapeColor(backgr.Color)));
                    break;
                case BackgroundType.Picture:
                case BackgroundType.Texture:

                    if (backgr.Type == BackgroundType.Picture)
                        strBuilder.Append(BuildShapeProp("fillType", "3"));
                    else
                        strBuilder.Append(BuildShapeProp("fillType", "2"));

                    WPicture pic = new WPicture(m_doc);
                    pic.LoadImage(backgr.Picture);
                    strBuilder.Append(BuildShapeProp("fillBlip", BuildPicture(pic)));
                    break;
                case BackgroundType.Gradient:
                    switch (backgr.Gradient.ShadingStyle)
                    {
                        case GradientShadingStyle.FromCorner:
                            strBuilder.Append(BuildShapeProp("fillType", "5"));
                            strBuilder.Append(BuildShapeProp("fillFocus", "100"));
                            if (backgr.Gradient.ShadingVariant == GradientShadingVariant.ShadingDown)
                            {
                                strBuilder.Append(BuildShapeProp("fillToLeft", "65536"));
                                strBuilder.Append(BuildShapeProp("fillToRight", "65536"));
                            }
                            else if (backgr.Gradient.ShadingVariant == GradientShadingVariant.ShadingMiddle)
                            {
                                strBuilder.Append(BuildShapeProp("fillToLeft", "65536"));
                                strBuilder.Append(BuildShapeProp("fillToTop", "65536"));
                                strBuilder.Append(BuildShapeProp("fillToRight", "65536"));
                                strBuilder.Append(BuildShapeProp("fillToBottom", "65536"));
                            }
                            else if (backgr.Gradient.ShadingVariant == GradientShadingVariant.ShadingOut)
                            {
                                strBuilder.Append(BuildShapeProp("fillToTop", "65536"));
                                strBuilder.Append(BuildShapeProp("fillToBottom", "65536"));
                            }
                            break;
                        case GradientShadingStyle.FromCenter:
                            strBuilder.Append(BuildShapeProp("fillType", "6"));
                            if (backgr.Gradient.ShadingVariant == GradientShadingVariant.ShadingUp)
                                strBuilder.Append(BuildShapeProp("fillFocus", "100"));

                            strBuilder.Append(BuildShapeProp("fillToLeft", "32768"));
                            strBuilder.Append(BuildShapeProp("fillToRight", "32768"));
                            strBuilder.Append(BuildShapeProp("fillToTop", "32768"));
                            strBuilder.Append(BuildShapeProp("fillToBottom", "32768"));
                            break;
                        case GradientShadingStyle.Horizontal:
                            strBuilder.Append(BuildShapeProp("fillType", "7"));
                            strBuilder.Append(BuildGradientVariant(backgr.Gradient.ShadingVariant));
                            break;
                        case GradientShadingStyle.Vertical:
                            strBuilder.Append(BuildShapeProp("fillType", "7"));
                            strBuilder.Append(BuildShapeProp("fillAngle", "-5898240"));
                            strBuilder.Append(BuildGradientVariant(backgr.Gradient.ShadingVariant));
                            break;
                        case GradientShadingStyle.DiagonalDown:
                            strBuilder.Append(BuildShapeProp("fillType", "7"));
                            strBuilder.Append(BuildShapeProp("fillAngle", "-2949120"));
                            strBuilder.Append(BuildGradientVariant(backgr.Gradient.ShadingVariant));
                            break;
                        case GradientShadingStyle.DiagonalUp:
                            strBuilder.Append(BuildShapeProp("fillType", "7"));
                            strBuilder.Append(BuildShapeProp("fillAngle", "-8847360"));
                            strBuilder.Append(BuildGradientVariant(backgr.Gradient.ShadingVariant));
                            break;
                    }

                    strBuilder.Append(BuildShapeProp("fillColor", GetRtfShapeColor(backgr.Gradient.Color1)));
                    strBuilder.Append(BuildShapeProp("fillBackColor", GetRtfShapeColor(backgr.Gradient.Color2)));
                    break;
                default:
                    return string.Empty;
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the gradient variant.
        /// </summary>
        /// <param name="variant">The gradient variant.</param>
        /// <returns></returns>
        private string BuildGradientVariant(GradientShadingVariant variant)
        {
            switch (variant)
            {
                case GradientShadingVariant.ShadingUp:
                    return BuildShapeProp("fillFocus", "100");
                case GradientShadingVariant.ShadingMiddle:
                    return BuildShapeProp("fillFocus", "50");
                case GradientShadingVariant.ShadingOut:
                    return BuildShapeProp("fillFocus", "-50");
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Implementation / list
        /// <summary>
        /// Appends the list styles to list table.
        /// </summary>
        /// <param name="listStyles">The list styles.</param>
        private void AppendListStyles()
        {
            ListStyleCollection listStyles = m_doc.ListStyles;
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            if (listStyles.Count == 0)
                return;

            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "listtable");
            memStream.Write(byteArr, 0, byteArr.Length);

            for (int i = 0, count = listStyles.Count; i < count; i++)
            {
                ListStyle listStyle = listStyles[i] as ListStyle;
                byteArr = m_encoding.GetBytes(Environment.NewLine);
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "list");
                memStream.Write(byteArr, 0, byteArr.Length);

                if (listStyle.IsSimple)
                {
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "listsimple1");
                    memStream.Write(byteArr, 0, byteArr.Length);
                }
                else
                {
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "listsimple0");
                    memStream.Write(byteArr, 0, byteArr.Length);
                }
                if (listStyle.IsHybrid)
                {
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "listhybrid");
                    memStream.Write(byteArr, 0, byteArr.Length);
                }
                for (int j = 0, cnt = listStyle.Levels.Count; j < cnt; j++)
                {
                    WListLevel listLevel = listStyle.Levels[j];
                    byteArr = m_encoding.GetBytes(BuildListLevel(listLevel));
                    memStream.Write(byteArr, 0, byteArr.Length);
                }


                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "listname ");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(listStyle.Name);
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@" ;}");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(c_slashSymbol + "listid" + i.ToString());
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(Environment.NewLine);
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}");
                memStream.Write(byteArr, 0, byteArr.Length);

                ListsIds.Add(listStyle.Name, i);
            }

            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);

            m_listTableBytes = memStream.ToArray();
        }
        /// <summary>
        /// Builds the list level.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        /// <returns></returns>
        private string BuildListLevel(WListLevel listLevel)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "listlevel");

            strBuilder.Append(BuildLevelFormatting(listLevel.PatternType));

            if (listLevel.NumberAlignment == ListNumberAlignment.Left)
                strBuilder.Append(c_slashSymbol + "leveljc0" + c_slashSymbol + "leveljcn0");
            else if (listLevel.NumberAlignment == ListNumberAlignment.Center)
                strBuilder.Append(c_slashSymbol + "leveljc1" + c_slashSymbol + "leveljcn1");
            else if (listLevel.NumberAlignment == ListNumberAlignment.Right)
                strBuilder.Append(c_slashSymbol + "leveljc2" + c_slashSymbol + "leveljcn2");

            if (listLevel.FollowCharacter == FollowCharacterType.Tab)
                strBuilder.Append(c_slashSymbol + "levelfollow0");
            else if (listLevel.FollowCharacter == FollowCharacterType.Space)
                strBuilder.Append(c_slashSymbol + "levelfollow1");
            else
                strBuilder.Append(c_slashSymbol + "levelfollow2");

            strBuilder.Append(c_slashSymbol + "levelstartat");
            strBuilder.Append(listLevel.StartAt.ToString());

            strBuilder.Append(c_slashSymbol + "levelspace");
            strBuilder.Append(listLevel.LegacySpace);

            strBuilder.Append(c_slashSymbol + "levelindent");
            strBuilder.Append(listLevel.LegacyIndent);

            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(BuildLevelText(listLevel));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(BuildLevelNumbers(listLevel));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(BuildParagraphFormat(listLevel.ParagraphFormat, null));
            strBuilder.Append(BuildCharacterFormat(listLevel.CharacterFormat));

            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Updates the list numbering prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        private string UpdateNumberPrefix(string prefix)
        {
            string numberPrefix = prefix;

            numberPrefix = numberPrefix.Replace(WListLevel.Level1Str, c_slashSymbol + "'00");
            numberPrefix = numberPrefix.Replace(WListLevel.Level2Str, c_slashSymbol + "'01");
            numberPrefix = numberPrefix.Replace(WListLevel.Level3Str, c_slashSymbol + "'02");
            numberPrefix = numberPrefix.Replace(WListLevel.Level4Str, c_slashSymbol + "'03");
            numberPrefix = numberPrefix.Replace(WListLevel.Level5Str, c_slashSymbol + "'04");
            numberPrefix = numberPrefix.Replace(WListLevel.Level6Str, c_slashSymbol + "'05");
            numberPrefix = numberPrefix.Replace(WListLevel.Level7Str, c_slashSymbol + "'06");
            numberPrefix = numberPrefix.Replace(WListLevel.Level8Str, c_slashSymbol + "'07");
            numberPrefix = numberPrefix.Replace(WListLevel.Level9Str, c_slashSymbol + "'08");

            return numberPrefix;
        }
        /// <summary>
        /// Builds the level text.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        /// <returns></returns>
        private string BuildLevelText(WListLevel listLevel)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "leveltext");

            if (listLevel.PatternType == ListPatternType.Bullet)
            {
                strBuilder.Append(c_slashSymbol + "'01");
                string bulletStr = listLevel.BulletCharacter;
                if (IsChanged(ref bulletStr))
                {
                    strBuilder.Append(bulletStr);
                }
                else
                {

                    byte symbol = (byte)listLevel.BulletCharacter[0];
                    if (listLevel.CharacterFormat.FontName == "Symbol"
                      || (!(symbol > 64 && symbol < 91) && !(symbol > 96 && symbol < 123)))
                    {
                        int symbolCode = 4096 - (int)symbol;
                        strBuilder.Append(c_slashSymbol + "u-" + symbolCode.ToString() + " ?");
                    }
                    else
                    {
                        //roman alphabet
                        strBuilder.Append(listLevel.BulletCharacter[0].ToString());
                    }
                }
            }
            else
            {
                string levelText = GetLevelText(listLevel);
                int levelTextLeng = GetLevelTextLeng(levelText);
                strBuilder.Append(c_slashSymbol + "'" + levelTextLeng.ToString("X2") + levelText);
            }
            strBuilder.Append(@";}");

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the level numbers.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        /// <returns></returns>
        private string BuildLevelNumbers(WListLevel listLevel)
        {
            StringBuilder strBuilder = new StringBuilder();

            string levelText = GetLevelText(listLevel);
            strBuilder.Append(@"{" + c_slashSymbol + "levelnumbers");
            if (listLevel.PatternType != ListPatternType.Bullet && levelText != string.Empty)
            {
                levelText = levelText.Trim();
                int levelTextLeng = GetLevelTextLeng(levelText);

                if (!string.IsNullOrEmpty(listLevel.NumberPrefix) && !IsComplexList(listLevel.NumberPrefix))
                {
                    int pos = listLevel.NumberPrefix.Length + 1;
                    for (int i = pos; i <= levelTextLeng; i += 2)
                        strBuilder.Append(c_slashSymbol + "'" + i.ToString("X2"));
                }
                else
                {
                    for (int i = 1; i <= levelTextLeng; i += 2)
                        strBuilder.Append(c_slashSymbol + "'" + i.ToString("X2"));
                }
            }
            strBuilder.Append(@";}");

            return strBuilder.ToString();
            //if( levelText.IndexOf( @"(" ) != -1 && levelText.IndexOf( @")" ) != -1 )
            //{
            //  for( int i = 2; i <= levelTextLeng; i += 2 )
            //    strBuilder.Append( @"\'" + i.ToString( "X2" ) );
            //}
            //else
            //{
            //  for( int i = 1; i <= levelTextLeng; i += 2 )
            //    strBuilder.Append( @"\'" + i.ToString( "X2" ) );
            //}
        }
        /// <summary>
        /// Gets the level text length.
        /// </summary>
        /// <param name="levelText">The level text.</param>
        /// <returns></returns>
        private int GetLevelTextLeng(string levelText)
        {
            levelText = levelText.Replace(c_slashSymbol + "'0", string.Empty);
            return levelText.Length;
        }
        /// <summary>
        /// Gets the level text.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        /// <returns></returns>
        private string GetLevelText(WListLevel listLevel)
        {
            string numberPrefix = listLevel.NumberPrefix;
            if (!string.IsNullOrEmpty(numberPrefix))
            {
                numberPrefix = UpdateNumberPrefix(listLevel.NumberPrefix);
            }
            return numberPrefix + c_slashSymbol + "'0" + listLevel.LevelNumber.ToString() + listLevel.NumberSufix;
        }
        /// <summary>
        /// Builds the level formatting.
        /// </summary>
        /// <param name="type">The type of number.</param>
        /// <returns></returns>
        private string BuildLevelFormatting(ListPatternType type)
        {
            switch (type)
            {
                case ListPatternType.Arabic:
                    return c_slashSymbol + "levelnfc0" + c_slashSymbol + "levelnfcn0";
                case ListPatternType.UpRoman:
                    return c_slashSymbol + "levelnfc1" + c_slashSymbol + "levelnfcn1";
                case ListPatternType.LowRoman:
                    return c_slashSymbol + "levelnfc2" + c_slashSymbol + "levelnfcn2";
                case ListPatternType.UpLetter:
                    return c_slashSymbol + "levelnfc3" + c_slashSymbol + "levelnfcn3";
                case ListPatternType.LowLetter:
                    return c_slashSymbol + "levelnfc4" + c_slashSymbol + "levelnfcn4";
                case ListPatternType.Ordinal:
                    return c_slashSymbol + "levelnfc5" + c_slashSymbol + "levelnfcn5";
                case ListPatternType.OrdinalText:
                    return c_slashSymbol + "levelnfc7" + c_slashSymbol + "levelnfcn7";
                case ListPatternType.LeadingZero:
                    return c_slashSymbol + "levelnfc22" + c_slashSymbol + "levelnfcn22";
                case ListPatternType.Bullet:
                    return c_slashSymbol + "levelnfc23" + c_slashSymbol + "levelnfcn23";
                default:
                    return c_slashSymbol + "levelnfc255" + c_slashSymbol + "levelnfcn255";
            }
        }
        /// <summary>
        /// Appends the override list style.
        /// </summary>
        private void AppendOverrideList()
        {
            MemoryStream memStream = new MemoryStream();

            byte[] byteArr;
            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "listoverridetable");
            memStream.Write(byteArr, 0, byteArr.Length);

            foreach (int id in ListOverrideAr.Keys)
            {
                byteArr = m_encoding.GetBytes(Environment.NewLine);
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "listoverride");
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(c_slashSymbol + "listid" + (id - 1).ToString());
                memStream.Write(byteArr, 0, byteArr.Length);

                string listOverName = ListOverrideAr[id];
                if (!string.IsNullOrEmpty(listOverName))
                {
                    ListOverrideStyle listOverStyle = m_doc.ListOverrides.FindByName(listOverName);

                    byteArr = m_encoding.GetBytes(c_slashSymbol + "listoverridecount" + listOverStyle.OverrideLevels.Count);
                    memStream.Write(byteArr, 0, byteArr.Length);
                    byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "lfolevel" + c_slashSymbol + "listoverrideformat");
                    memStream.Write(byteArr, 0, byteArr.Length);
                    byteArr = m_encoding.GetBytes(Environment.NewLine);
                    memStream.Write(byteArr, 0, byteArr.Length);

                    foreach (OverrideLevelFormat overListForm in listOverStyle.OverrideLevels)
                    {
                        byteArr = m_encoding.GetBytes(BuildListLevel(overListForm.OverrideListLevel));
                        memStream.Write(byteArr, 0, byteArr.Length);
                    }
                    byteArr = m_encoding.GetBytes(@"}");
                    memStream.Write(byteArr, 0, byteArr.Length);
                }
                else
                {
                    byteArr = m_encoding.GetBytes(c_slashSymbol + "listoverridecount0");
                    memStream.Write(byteArr, 0, byteArr.Length);
                }
                byteArr = m_encoding.GetBytes(c_slashSymbol + "ls" + id.ToString());
                memStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}");
                memStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);

            m_listOverrideTableBytes = memStream.ToArray();
        }
        /// <summary>
        /// Builds the list text for paragraph.
        /// </summary>
        /// <param name="para">The para.</param>
        /// <returns></returns>
        private string BuildListText(WListFormat listFormat)
        {
            if (listFormat.ListType == ListType.NoList)
                return string.Empty;

            WListLevel listLevel = listFormat.CurrentListLevel;

            if (!string.IsNullOrEmpty(listFormat.LFOStyleName))
            {
                ListOverrideStyle overLevelStyle = m_doc.ListOverrides.FindByName(listFormat.LFOStyleName);
                if (overLevelStyle != null)
                    listLevel = overLevelStyle.OverrideLevels[listFormat.ListLevelNumber].OverrideListLevel;
            }

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "listtext");
            strBuilder.Append(BuildParagraphFormat(listLevel.ParagraphFormat, null));
            strBuilder.Append(BuildCharacterFormat(listLevel.CharacterFormat));
            strBuilder.Append(BuildListText(listLevel, listFormat));
            strBuilder.Append(c_slashSymbol + "tab}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();

            //if( listLevel.PatternType != ListPatternType.None )
            //{
            //  strBuilder.Append( BuildListText( listLevel, listFormat ) );
            //}
            //else if( !string.IsNullOrEmpty( listFormat.LFOStyleName ) )
            //{
            //  ListOverrideStyle overLevelStyle = m_doc.ListOverrides.FindByName( listFormat.LFOStyleName );
            //  if( overLevelStyle != null )
            //  {
            //    listLevel = overLevelStyle.OverrideLevels[ listFormat.ListLevelNumber ].OverrideListLevel;
            //    strBuilder.Append( BuildListText( listLevel, listFormat ) );
            //  }
            //}
        }
        /// <summary>
        /// Builds the list text for paragraph.
        /// </summary>
        /// <param name="listLevel">The list level.</param>
        /// <returns></returns>
        private string BuildListText(WListLevel listLevel, WListFormat listFormat)
        {
            if (listLevel == null && listFormat == null)
                return string.Empty;

            if (listLevel.PatternType == ListPatternType.Bullet)
            {
                byte symbol = (byte)listLevel.BulletCharacter[0];
                return c_slashSymbol + "'" + symbol.ToString("X2");
            }
            else if (listLevel.PatternType == ListPatternType.LowLetter
              || listLevel.PatternType == ListPatternType.UpLetter)
            {
                return BuildLstLetterSymbol(listFormat);
            }
            else if (listLevel.PatternType == ListPatternType.Arabic && !IsComplexList(listLevel.NumberPrefix))
            {
                return listLevel.NumberPrefix + GetLstStartVal(listFormat) + listLevel.NumberSufix;
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Gets the List start value.
        /// </summary>
        /// <param name="format">The list format.</param>
        /// <returns></returns>
        private int GetLstStartVal(WListFormat format)
        {
            if (!ListStart.ContainsKey(format.CustomStyleName))
            {
                Dictionary<int,int> startVal = new Dictionary<int,int>();
                ListStart.Add(format.CustomStyleName, startVal);
                WListLevel level = format.CurrentListStyle.Levels[format.ListLevelNumber];
                startVal.Add(format.ListLevelNumber, level.StartAt + 1);
                return level.StartAt;
            }
            else
            {
                Dictionary<int, int> lstStyle = ListStart[format.CustomStyleName];
                if (lstStyle.ContainsKey(format.ListLevelNumber))
                {
                    int startAt = lstStyle[format.ListLevelNumber];
                    lstStyle[format.ListLevelNumber] = startAt + 1;
                    return startAt;
                }
                else
                {
                    WListLevel level = format.CurrentListStyle.Levels[format.ListLevelNumber];
                    lstStyle.Add(format.ListLevelNumber, level.StartAt + 1);
                    return level.StartAt + 1;
                }
            }
        }
        /// <summary>
        /// Builds the List letter symbol.
        /// </summary>
        /// <param name="format">The list format.</param>
        /// <returns></returns>
        private string BuildLstLetterSymbol(WListFormat format)
        {
            int index = (format.CurrentListLevel.PatternType == ListPatternType.LowLetter) ? 96 : 64;

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(format.CurrentListLevel.NumberPrefix);
            int startAt = GetLstStartVal(format);
            int count = 1;
            while (startAt > 26)
            {
                startAt -= 26;
                count++;
            }
            int symbol = startAt + index;
            string letter = ((char)symbol).ToString();
            for (int i = 0; i < count; i++)
            {
                strBuilder.Append(letter);
            }
            strBuilder.Append(format.CurrentListLevel.NumberSufix);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Determines whether the specified text is changed.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is changed; otherwise, <c>false</c>.
        /// </returns>
        private bool IsChanged(ref string listLText)
        {
            string originalText = listLText;

            listLText = listLText.Replace(c_symbol8226, c_slashSymbol + "'95");

            if (originalText == listLText)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Checks the number prefix.
        /// </summary>
        /// <param name="prefix">The number prefix.</param>
        /// <returns>
        /// 	<c>true</c> if [is complex list] [the specified prefix]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsComplexList(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return true;

            if (prefix.Contains(WListLevel.Level1Str))
                return true;
            if (prefix.Contains(WListLevel.Level2Str))
                return true;
            if (prefix.Contains(WListLevel.Level3Str))
                return true;
            if (prefix.Contains(WListLevel.Level4Str))
                return true;
            if (prefix.Contains(WListLevel.Level5Str))
                return true;
            if (prefix.Contains(WListLevel.Level6Str))
                return true;
            if (prefix.Contains(WListLevel.Level7Str))
                return true;
            if (prefix.Contains(WListLevel.Level8Str))
                return true;
            if (prefix.Contains(WListLevel.Level9Str))
                return true;

            return false;
        }
        #endregion

        #region Implementation / watermark
        /// <summary>
        /// Builds the watermark.
        /// </summary>
        /// <returns></returns>
        private string BuildWatermark()
        {
            Watermark watermark = m_doc.Watermark;
            if (watermark.Type == WatermarkType.NoWatermark)
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();

            string wtrmarkBody = string.Empty;
            if (watermark.Type == WatermarkType.TextWatermark)
                wtrmarkBody = BuildTextWtrmarkBody(watermark as TextWatermark);
            else
                wtrmarkBody = BuildPictWtrmarkBody(watermark as PictureWatermark);

            strBuilder.Append(@"{" + c_slashSymbol + "headerl");
            strBuilder.Append(wtrmarkBody);
            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            strBuilder.Append(@"{" + c_slashSymbol + "headerr");
            strBuilder.Append(wtrmarkBody);
            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            strBuilder.Append(@"{" + c_slashSymbol + "headerf");
            strBuilder.Append(wtrmarkBody);
            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the picture watermark body.
        /// </summary>
        /// <param name="picWatermark">The pic watermark.</param>
        /// <returns></returns>
        private string BuildPictWtrmarkBody(PictureWatermark picWatermark)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "shp");
            strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "shpinst");
            strBuilder.Append(c_slashSymbol + "shpleft0");
            strBuilder.Append(c_slashSymbol + "shptop0");
            //Wrapping style none ( behind text )
            strBuilder.Append(c_slashSymbol + "shpwr3");
            strBuilder.Append(c_slashSymbol + "shpright");
            strBuilder.Append(picWatermark.WordPicture.Width * RtfNavigator.c_twentiethOfPoint);
            strBuilder.Append(c_slashSymbol + "shpbottom");
            strBuilder.Append(picWatermark.WordPicture.Height * RtfNavigator.c_twentiethOfPoint);
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(BuildShapeProp("shapeType", "75"));
            strBuilder.Append(BuildShapeProp("pib", BuildPictureProp(picWatermark.WordPicture)));
            strBuilder.Append(BuildDefWtrmarkProp());
            strBuilder.Append(@"}}");

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the text watermark body ( property ).
        /// </summary>
        /// <param name="textWatermark">The text watermark.</param>
        /// <returns></returns>
        private string BuildTextWtrmarkBody(TextWatermark textWatermark)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "shp");
            strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "shpinst");
            strBuilder.Append(c_slashSymbol + "shpleft0");
            strBuilder.Append(c_slashSymbol + "shptop0");
            strBuilder.Append(c_slashSymbol + "shpright");
            strBuilder.Append(textWatermark.ShapeWidthInPixels);
            strBuilder.Append(c_slashSymbol + "shpbottom");
            strBuilder.Append(textWatermark.ShapeHeightInPixels);
            // wrapping none
            strBuilder.Append(c_slashSymbol + "shpwr3");
            strBuilder.Append(Environment.NewLine);

            strBuilder.Append(BuildShapeProp("shapeType", "136"));
            if (textWatermark.Layout == WatermarkLayout.Diagonal)
                strBuilder.Append(BuildShapeProp("rotation", "20643840"));
            string text = textWatermark.Text.Replace(c_slashSymbol + "0", string.Empty);
            strBuilder.Append(BuildShapeProp("gtextUNICODE", text));
            int size = (int)Math.Round(textWatermark.Size * 65536);
            strBuilder.Append(BuildShapeProp("gtextSize", size.ToString()));
            string fontName = textWatermark.FontName.Replace(c_slashSymbol + "0", string.Empty);
            strBuilder.Append(BuildShapeProp("gtextFont", fontName));
            strBuilder.Append(BuildShapeProp("fillColor", GetRtfShapeColor(textWatermark.Color)));
            if (textWatermark.Semitransparent)
                strBuilder.Append(BuildShapeProp("fillOpacity", "32768"));
            strBuilder.Append(BuildDefWtrmarkProp());
            strBuilder.Append(@"}}");

            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the default watermark property.
        /// </summary>
        /// <returns></returns>
        private string BuildDefWtrmarkProp()
        {
            StringBuilder strBuilder = new StringBuilder();
            // Has line - FALSE
            strBuilder.Append(BuildShapeProp("fLine", "0"));
            // Horizontal alignment: Center
            strBuilder.Append(BuildShapeProp("posh", "2"));
            // Position horizontally relative to: Margin
            strBuilder.Append(BuildShapeProp("posrelh", "0"));
            // Vertical alignment: Column
            strBuilder.Append(BuildShapeProp("posv", "2"));
            // Position horizontally relative to: Margin
            strBuilder.Append(BuildShapeProp("posrelv", "0"));
            return strBuilder.ToString();
        }
        #endregion

        #region Implementation / form field
        /// <summary>
        /// Builds the text form field.
        /// </summary>
        /// <param name="textField">The text field.</param>
        /// <returns></returns>
        private string BuildTextFormField(WTextFormField textField)
        {
            CurrentField.Push(textField);
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "field");
            strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst{");
            strBuilder.Append(BuildCharacterFormat(textField.CharacterFormat));
            strBuilder.Append(@" ");
            strBuilder.Append(BuildFieldType(textField.FieldType));
            strBuilder.Append(@"}");

            strBuilder.Append(@"{" + c_slashSymbol + "formfield");
            strBuilder.Append(@"{" + c_slashSymbol + "fftype0");
            if (textField.Type == TextFormFieldType.RegularText)
                strBuilder.Append(c_slashSymbol + "fftypetxt0");
            else if (textField.Type == TextFormFieldType.NumberText)
                strBuilder.Append(c_slashSymbol + "fftypetxt1");
            else
                strBuilder.Append(c_slashSymbol + "fftypetxt2");

            if (textField.CalculateOnExit)
                strBuilder.Append(c_slashSymbol + "ffrecalc");

            if (textField.MaximumLength != 0)
                strBuilder.Append(c_slashSymbol + "ffmaxlen" + textField.MaximumLength.ToString());

            strBuilder.Append(c_slashSymbol + "ffhps20");

            if (!string.IsNullOrEmpty(textField.Name))
            {
                strBuilder.Append(@"{" + c_slashSymbol + "ffname ");
                strBuilder.Append(textField.Name + @"}");
            }

            if (!string.IsNullOrEmpty(textField.DefaultText))
            {
                strBuilder.Append(@"{" + c_slashSymbol + "ffdeftext ");
                strBuilder.Append(textField.DefaultText + @"}");
            }

            if (textField.TextFormat != TextFormat.None)
            {
                strBuilder.Append(@"{" + c_slashSymbol + "ffformat ");
                switch (textField.TextFormat)
                {
                    case TextFormat.Uppercase:
                        strBuilder.Append(@"Uppercase");
                        break;
                    case TextFormat.Lowercase:
                        strBuilder.Append(@"Lowercase");
                        break;
                    case TextFormat.Titlecase:
                        strBuilder.Append(@"Titlecase");
                        break;
                    case TextFormat.FirstCapital:
                        strBuilder.Append("FirstCapital");
                        break;
                }
                strBuilder.Append(@"}");
            }

            strBuilder.Append(@"}}}");
            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the check box.
        /// </summary>
        /// <param name="checkBox">The check box.</param>
        /// <returns></returns>
        private string BuildCheckBox(WCheckBox checkBox)
        {
            CurrentField.Push(checkBox);
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "field");
            strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst{");
            strBuilder.Append(BuildCharacterFormat(checkBox.CharacterFormat));
            strBuilder.Append(@" ");
            strBuilder.Append(BuildFieldType(checkBox.FieldType));
            strBuilder.Append(@"}");

            strBuilder.Append(@"{" + c_slashSymbol + "formfield");
            strBuilder.Append(@"{" + c_slashSymbol + "fftype1");
            // Need investigation
            strBuilder.Append(c_slashSymbol + "ffres25");
            if (checkBox.SizeType == CheckBoxSizeType.Auto)
                strBuilder.Append(c_slashSymbol + "ffsize0");
            else
                strBuilder.Append(c_slashSymbol + "ffsize1");
            strBuilder.Append(c_slashSymbol + "fftypetxt0");
            if (checkBox.CalculateOnExit)
                strBuilder.Append(c_slashSymbol + "ffrecalc");
            strBuilder.Append(c_slashSymbol + "ffhps" + (checkBox.CheckBoxSize * 2).ToString());

            if (!string.IsNullOrEmpty(checkBox.Name))
            {
                strBuilder.Append(@"{" + c_slashSymbol + "ffname ");
                strBuilder.Append(checkBox.Name + @"}");
            }

            if (checkBox.Checked)
                strBuilder.Append(c_slashSymbol + "ffdefres1");
            else
                strBuilder.Append(c_slashSymbol + "ffdefres0");

            strBuilder.Append(@"}}}");
            return strBuilder.ToString();
        }
        /// <summary>
        /// Builds the drop down field.
        /// </summary>
        /// <param name="dropDownField">The drop down field.</param>
        /// <returns></returns>
        private string BuildDropDownField(WDropDownFormField dropDownField)
        {
            CurrentField.Push(dropDownField);
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{" + c_slashSymbol + "field");
            strBuilder.Append(@"{" + c_slashSymbol + "*" + c_slashSymbol + "fldinst{");
            strBuilder.Append(BuildCharacterFormat(dropDownField.CharacterFormat));
            strBuilder.Append(@" ");
            strBuilder.Append(BuildFieldType(dropDownField.FieldType));
            strBuilder.Append(@"}");

            strBuilder.Append(@"{" + c_slashSymbol + "formfield");
            strBuilder.Append(@"{" + c_slashSymbol + "fftype2");
            // Need investigation
            strBuilder.Append(c_slashSymbol + "ffres25");
            strBuilder.Append(c_slashSymbol + "fftypetxt0");

            if (dropDownField.CalculateOnExit)
                strBuilder.Append(c_slashSymbol + "ffrecalc");

            strBuilder.Append(c_slashSymbol + "ffhaslistbox");
            strBuilder.Append(c_slashSymbol + "ffhps20");

            if (!string.IsNullOrEmpty(dropDownField.Name))
            {
                strBuilder.Append(@"{" + c_slashSymbol + "ffname ");
                strBuilder.Append(dropDownField.Name + @"}");
            }

            strBuilder.Append(@"ffdefres0");

            for (int i = 0, count = dropDownField.DropDownItems.Count; i < count; i++)
            {
                strBuilder.Append(@"{" + c_slashSymbol + "ffl ");
                strBuilder.Append((dropDownField.DropDownItems[i]).Text);
                strBuilder.Append(@"}");
            }

            strBuilder.Append(@"}}}");
            return strBuilder.ToString();
        }
        #endregion

        #region Implementation / comment
        /// <summary>
        /// Builds the comment mark.
        /// </summary>
        /// <param name="cMark">The c mark.</param>
        /// <returns></returns>
        private byte[] BuildCommentMark(WCommentMark cMark)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;
            if (cMark.Type == CommentMarkType.CommentStart)
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "atrfstart ");
                memStream.Write(byteArr, 0, byteArr.Length);
                int id = GetNextId();
                byteArr = m_encoding.GetBytes(id.ToString());
                memStream.Write(byteArr, 0, byteArr.Length);
                CommentIds.Add(cMark.CommentId, id);
            }
            else
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "atrfend ");
                memStream.Write(byteArr, 0, byteArr.Length);
                int id = CommentIds[cMark.CommentId];
                byteArr = m_encoding.GetBytes(id.ToString());
                memStream.Write(byteArr, 0, byteArr.Length);
            }

            byteArr = m_encoding.GetBytes(@"}");
            memStream.Write(byteArr, 0, byteArr.Length);
            return memStream.ToArray();
        }
        /// <summary>
        /// Builds the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        private byte[] BuildComment(WComment comment)
        {
            StringBuilder strBuilder = new StringBuilder();
            MemoryStream commentStream = new MemoryStream();
            byte[] byteArr;

            string commentId = null;
            if (m_commentIds != null && CommentIds.ContainsKey(comment.Format.TagBkmk))
                commentId = (CommentIds[comment.Format.TagBkmk]).ToString();

            if (commentId == null && comment.CommentedItems.Count != 0)
                commentId = GetNextId().ToString();

            if (comment.AppendItems)
            {
                byteArr = BuildComItems(comment, int.Parse(commentId));
                commentStream.Write(byteArr, 0, byteArr.Length);
            }

            byteArr = m_encoding.GetBytes(@"{");
            commentStream.Write(byteArr, 0, byteArr.Length);
            if (!string.IsNullOrEmpty(comment.Format.UserInitials))
            {
                byteArr = m_encoding.GetBytes(BuildCharacterFormat(comment.ParaItemCharFormat));
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "atnid ");
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(comment.Format.UserInitials);
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}");
                commentStream.Write(byteArr, 0, byteArr.Length);
            }

            if (!string.IsNullOrEmpty(comment.Format.User))
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "atnauthor ");
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(comment.Format.User);
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}");
                commentStream.Write(byteArr, 0, byteArr.Length);
            }

            byteArr = m_encoding.GetBytes(c_slashSymbol + "chatn ");
            commentStream.Write(byteArr, 0, byteArr.Length);

            byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "annotation");
            commentStream.Write(byteArr, 0, byteArr.Length);

            if (commentId != null)
            {
                byteArr = m_encoding.GetBytes(@"{" + c_slashSymbol + "*" + c_slashSymbol + "atnref ");
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(commentId);
                commentStream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(@"}");
                commentStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = BuildBodyItems(comment.TextBody.Items);
            commentStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@"}}");
            commentStream.Write(byteArr, 0, byteArr.Length);
           
            return commentStream.ToArray();
        }
        /// <summary>
        /// Builds the Commnet items.
        /// </summary>
        /// <param name="comment">The comment.</param>
        private byte[] BuildComItems(WComment comment, int id)
        {
            MemoryStream comItemsStream = new MemoryStream();
            byte[] byteArr;

            WCommentMark comStart = new WCommentMark(comment.Document, id, CommentMarkType.CommentStart);
            WCommentMark comEnd = new WCommentMark(comment.Document, id, CommentMarkType.CommentEnd);
            byteArr = BuildCommentMark(comStart);
            comItemsStream.Write(byteArr, 0, byteArr.Length);
            foreach (ParagraphItem item in comment.CommentedItems)
            {
                byteArr = BuildParagraphItem(item);
                comItemsStream.Write(byteArr, 0, byteArr.Length);
            }
            byteArr = BuildCommentMark(comEnd);
            comItemsStream.Write(byteArr, 0, byteArr.Length);
            return comItemsStream.ToArray();
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Sets the color value.
        /// </summary>
        /// <param name="cFormat">The character format.</param>
        /// <param name="cFormatColor">Color</param>
        /// <param name="baseCFormat">The base character format.</param>
        /// <param name="baseCFormatColor">Color</param>
        /// <param name="optionKey">The option key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string BuildColorValue(WCharacterFormat cFormat, Color cFormatColor, WCharacterFormat baseCFormat,
          Color baseCFormatColor, int optionKey, string value)
        {
            if (cFormat.HasValue(optionKey) && !cFormatColor.IsEmpty)
            {
                if (cFormatColor.IsNamedColor && cFormatColor.IsKnownColor)
                {
                    if (WCharacterFormat.HighlightColorKey == optionKey)
                        return BuildHighlightNamedColor(cFormatColor, value);
                    else
                        return BuildNamedColor(cFormatColor, value);
                }
                else
                    return BuildColor(cFormatColor, value);
            }
            else if (baseCFormat != null && baseCFormat.HasValue(optionKey) && !baseCFormatColor.IsEmpty)
            {
                if (baseCFormatColor.IsNamedColor && baseCFormatColor.IsKnownColor)
                {
                    if (WCharacterFormat.HighlightColorKey == optionKey)
                        return BuildHighlightNamedColor(baseCFormatColor, value);
                    else
                        return BuildNamedColor(baseCFormatColor, value);
                }
                else
                    return BuildColor(baseCFormatColor, value);
            }

            return string.Empty;
        }
        /// <summary>
        /// Builds Highlight color with name
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string BuildHighlightNamedColor(Color color, string value)
        {
            Color newColor = new Color();
            switch (color.Name)
            {
                case "Maroon":
                    newColor = Color.FromArgb(255, 0, 0);
                    break;
                case "Green":
                    newColor = Color.FromArgb(0, 255, 0);
                    break;
                case "Olive":
                    newColor = Color.FromArgb(128, 128, 0);
                    break;
                case "Navy":
                    newColor = Color.FromArgb(0, 0, 128);
                    break;
                case "Purple":
                    newColor = Color.FromArgb(255, 0, 255);
                    break;
                case "Teal":
                    newColor = Color.FromArgb(0, 255, 255);
                    break;
                case "Red":
                    newColor = Color.FromArgb(255, 0, 0);
                    break;
                case "Lime":
                    newColor = Color.FromArgb(0, 128, 0);
                    break;
                case "Yellow":
                    newColor = Color.FromArgb(255, 255, 0);
                    break;
                case "Blue":
                    newColor = Color.FromArgb(0, 0, 255);
                    break;
                case "Fuchsia":
                    newColor = Color.FromArgb(128, 0, 128);
                    break;
                case "Aqua":
                    newColor = Color.FromArgb(0, 128, 128);
                    break;
                case "Gold":
                    newColor = Color.FromArgb(128, 100, 0);
                    break;
            }

            if (!newColor.IsEmpty)
                return BuildColor(newColor, value);
            else
                return BuildColor(color, value);
        }
        /// <summary>
        /// Builds color with name
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string BuildNamedColor(Color color, string value)
        {
            Color newColor = new Color();
            switch (color.Name)
            {
                case "Maroon":
                    newColor = Color.FromArgb(128, 0, 0);
                    break;
                case "Green":
                    newColor = Color.FromArgb(0, 128, 0);
                    break;
                case "Olive":
                    newColor = Color.FromArgb(128, 128, 0);
                    break;
                case "Navy":
                    newColor = Color.FromArgb(0, 0, 128);
                    break;
                case "Purple":
                    newColor = Color.FromArgb(128, 0, 128);
                    break;
                case "Teal":
                    newColor = Color.FromArgb(0, 128, 128);
                    break;
                case "Red":
                    newColor = Color.FromArgb(255, 0, 0);
                    break;
                case "Lime":
                    newColor = Color.FromArgb(0, 255, 0);
                    break;
                case "Yellow":
                    newColor = Color.FromArgb(255, 255, 0);
                    break;
                case "Blue":
                    newColor = Color.FromArgb(0, 0, 255);
                    break;
                case "Fuchsia":
                    newColor = Color.FromArgb(255, 0, 255);
                    break;
                case "Aqua":
                    newColor = Color.FromArgb(0, 255, 255);
                    break;
                case "Gold":
                    newColor = Color.FromArgb(255, 215, 0);
                    break;
            }

            if (!newColor.IsEmpty)
                return BuildColor(newColor, value);
            else
                return BuildColor(color, value);
        }
        /// <summary>
        /// Checks the section for footnotes/endnotes.
        /// </summary>
        private void CheckFootEndnote()
        {
            byte[] byteArr;
            if (m_hasFootnote && m_hasEndnote)
                byteArr = m_encoding.GetBytes(c_slashSymbol + "fet2");
            else if (m_hasEndnote)
                byteArr = m_encoding.GetBytes(c_slashSymbol + "fet1");
            else
                byteArr = m_encoding.GetBytes(c_slashSymbol + "fet0");
            m_mainBodyBytesList.Add(byteArr);
        }
        /// <summary>
        /// Builds field type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private string BuildFieldType(FieldType type)
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
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Gets the color of the RTF format shape.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private string GetRtfShapeColor(Color color)
        {
            string red = color.R.ToString("X");
            string green = color.G.ToString("X");
            string blue = color.B.ToString("X");

            int value = Int32.Parse(blue + green + red, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            if (color != Color.Red && color != Color.Green && color != Color.Blue
              && (color.R != 128 && color.G != 128 && color.B != 128)
              && (color.R != 192 && color.G != 192 && color.B != 192))
                value *= 16;

            return value.ToString();
        }
        /// <summary>
        /// Gets the color of the RTF format shape.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private string GetRtfPageBackgroundColor(Color color)
        {
            string red = color.R.ToString("X");
            string green = color.G.ToString("X");
            string blue = color.B.ToString("X");

            int value = Int32.Parse(blue + green + red, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            return value.ToString();
        }
        /// <summary>
        /// Writes the elements.
        /// </summary>
        /// <param name="param">The param.</param>
        private void WriteElements(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                byte[] byteArr;
                byteArr = m_encoding.GetBytes("{");
                m_stream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(param);
                m_stream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes("}");
                m_stream.Write(byteArr, 0, byteArr.Length);
                byteArr = m_encoding.GetBytes(Environment.NewLine);
                m_stream.Write(byteArr, 0, byteArr.Length);
            }
        }
        /// <summary>
        /// Generate next font id.
        /// </summary>
        /// <param name="isBidi">if set to <c>true</c> [is bidi].</param>
        /// <returns></returns>
        private string GetNextFontId(bool isBidi)
        {
            if (isBidi)
                return c_slashSymbol + "af" + m_fontId++;
            else
                return c_slashSymbol + "f" + m_fontId++;
        }
        /// <summary>
        /// Gets the next id.
        /// </summary>
        /// <returns></returns>
        private int GetNextId()
        {
            return m_uniqueId++;
        }
        /// <summary>
        /// Gets the next color id.
        /// </summary>
        /// <returns></returns>
        private int GetNextColorId()
        {
            return m_colorId++;
        }
        /// <summary>
        /// Check whether the font entries exists in the font table
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        private string IsFontEntryExits(string fontName, bool IsBidi)
        {
            string tempFontEntry;
            if (m_isCyrillicText)
                tempFontEntry = "fcharset204";
            else
                tempFontEntry = "fcharset0";

            tempFontEntry += "-" + fontName;

            if (!IsBidi)
            {
                if (FontEntries.ContainsKey(tempFontEntry))
                    return FontEntries[tempFontEntry];
            }
            else
            {
                if (AssociatedFontEntries.ContainsKey(tempFontEntry))
                    return AssociatedFontEntries[tempFontEntry];
            }

            return null;
        }
        /// <summary>
        /// Appends the font to the FonrString.
        /// </summary>
        /// <param name="fontId">The font id.</param>
        /// <param name="format">The format.</param>
        private void AppendFont(string fontId, string fontName)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;

            memStream.Write(m_fontBytes, 0, m_fontBytes.Length);

            string tempFontEntry;

            byteArr = m_encoding.GetBytes(@"{");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(fontId);
            memStream.Write(byteArr, 0, byteArr.Length);

            if (m_isCyrillicText)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "fcharset204");
                memStream.Write(byteArr, 0, byteArr.Length);
                tempFontEntry = "fcharset204";
            }
            else
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "fcharset0");
                memStream.Write(byteArr, 0, byteArr.Length);
                tempFontEntry = "fcharset0";
            }

            byteArr = m_encoding.GetBytes(@" ");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(fontName);
            memStream.Write(byteArr, 0, byteArr.Length);
            tempFontEntry += "-" + fontName;
            byteArr = m_encoding.GetBytes(";}");
            memStream.Write(byteArr, 0, byteArr.Length);

            if (fontId.StartsWith(@"\a"))
                AssociatedFontEntries.Add(tempFontEntry, fontId);
            else
                FontEntries.Add(tempFontEntry, fontId);

            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);

            m_fontBytes = memStream.ToArray();
        }
        /// <summary>
        /// Appends the color to the ColorString.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="attributeStr">The attribute string.</param>
        /// <returns></returns>
        private string BuildColor(Color color, string attributeStr)
        {
            if (color.IsEmpty)
                return string.Empty;

            if (ColorTable.ContainsKey(color))
                return attributeStr + ColorTable[color].ToString();
            else
                ColorTable.Add(color, m_colorId);

            MemoryStream memStream = new MemoryStream();
            byte[] byteArr;
            memStream.Write(m_colorBytes, 0, m_colorBytes.Length);

            if (color.A == 255 && color.R == 192 && color.G == 192 && color.B == 192)
            {
                byteArr = m_encoding.GetBytes(c_slashSymbol + "red0" + c_slashSymbol + "green0" + c_slashSymbol + "blue0;" + Environment.NewLine);
                memStream.Write(byteArr, 0, byteArr.Length);
                m_colorBytes = memStream.ToArray();
                return attributeStr + GetNextColorId();
            }

            byteArr = m_encoding.GetBytes(c_slashSymbol + "red" + color.R);
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "green" + color.G);
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(c_slashSymbol + "blue" + color.B);
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(@";");
            memStream.Write(byteArr, 0, byteArr.Length);
            byteArr = m_encoding.GetBytes(Environment.NewLine);
            memStream.Write(byteArr, 0, byteArr.Length);
            m_colorBytes = memStream.ToArray();

            return attributeStr + GetNextColorId();
        }
        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string UpdateText(string text)
        {
            char symbolR = (char)((byte)13); // '\r'
            char symbolN = (char)((byte)10); // '\n'

            text = text.Replace(Environment.NewLine, symbolR.ToString());
            text = text.Replace(symbolN, symbolR);
            text = text.Replace(c_symbol92, c_symbol92 + c_symbol92);

            return text;
        }
        /// <summary>
        /// Writes the font names.
        /// </summary>
        /// <param name="cFormat">The character format.</param>
        /// <returns></returns>
        private string WriteFontName(WCharacterFormat cFormat)
        {
            string fontId = IsFontEntryExits(cFormat.FontName, false);
            if (fontId == null)
            {
                fontId = GetNextFontId(false);
                AppendFont(fontId, cFormat.FontName);
                return fontId;
            }
            else
                return fontId;
        }
        /// <summary>
        /// Writes the font name bidi.
        /// </summary>
        /// <param name="cFormat">The character format.</param>
        /// <returns></returns>
        private string WriteFontNameBidi(WCharacterFormat cFormat)
        {
            string fontId = IsFontEntryExits(cFormat.FontName, true);
            if (fontId == null)
            {
                fontId = GetNextFontId(true);
                AppendFont(fontId, cFormat.FontName);
                return fontId;
            }
            else
                return fontId;
        }
        /// <summary>
        /// Writes the paragraph end.
        /// </summary>
        /// <param name="para">The para.</param>
        /// <returns></returns>
        private bool HasParaEnd(WParagraph para)
        {
            if ((para.IsInCell && para.NextSibling == null) || para.OwnerTextBody == null)
                return false;

            if (para.OwnerTextBody.OwnerBase is WFootnote)
                return false;

            if (para.OwnerTextBody.OwnerBase is WComment && para.NextSibling == null)
                return false;

            if (para.OwnerTextBody.OwnerBase is WSection && (para.OwnerTextBody.OwnerBase as WSection).PreviousSibling == null
              && para.Items.Count == 0 && para.NextSibling == null && !(para.PreviousSibling is WTable))
                return false;

            return true;
        }
        /// <summary>
        /// Prepares the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string PrepareText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            string preparedText = string.Empty;
            char[] textCharArray = text.ToCharArray();

            foreach (char letter in textCharArray)
            {
                if (letter >= 848 && letter <= 1103)
                {
                    byte code = (byte)((int)letter - 848);
                    string preparedLetter = c_slashSymbol + "'" + code.ToString("x");
                    preparedText += preparedLetter;
                    m_isCyrillicText = true;
                }
                else
                {
                    preparedText += letter.ToString();
                }
            }

            preparedText = " " + preparedText;
            preparedText = preparedText.Replace(c_slashSymbol + "t", c_slashSymbol + "tab");
            preparedText = preparedText.Replace(c_transfer, c_slashSymbol + "~");
            preparedText = preparedText.Replace(c_formFieldSymbol, c_slashSymbol + "u8194" + c_slashSymbol + "'3f");
            preparedText = preparedText.Replace(c_symbol123, c_slashSymbol + "{");
            preparedText = preparedText.Replace(c_symbol125, c_slashSymbol + "}");
            preparedText = preparedText.Replace(c_symbol31, c_slashSymbol + "-");
            preparedText = preparedText.Replace(c_symbol61553, c_slashSymbol + "u-3983" + c_slashSymbol + "'3f");
            preparedText = preparedText.Replace(c_symbol61549, c_slashSymbol + "u-3987" + c_slashSymbol + "'3f");

            preparedText = ReplaceUnicode(preparedText);

            return preparedText;
        }
        /// <summary>
        /// Replace unicode characters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string ReplaceUnicode(string text)
        {
            char[] textCharArray = text.ToCharArray();
            foreach (char letter in textCharArray)
            {
                if (letter > 0x7f)
                {
                    string preparedLetter = "\\u" + Convert.ToUInt32(letter) + "?";
                    text = text.Replace(letter.ToString(), preparedLetter);
                }
            }
            return text;
        }
        /// <summary>
        /// Builds the text range.
        /// </summary>
        /// <param name="cFormat">The character format.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string BuildTextRangeStr(WCharacterFormat cFormat, string text)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"{");
            strBuilder.Append(text);
            strBuilder.Insert(1, BuildCharacterFormat(cFormat));
            strBuilder.Append(@"}");
            strBuilder.Append(Environment.NewLine);

            m_isCyrillicText = false;

            return strBuilder.ToString();
        }
        /// <summary>
        /// Writes the field end.
        /// </summary>
        /// <param name="mark">The field mark.</param>
        /// <returns></returns>
        private bool WriteFieldEnd(WFieldMark mark)
        {
            if (mark.PreviousSibling != null)
            {
                if (mark.PreviousSibling is WCheckBox || mark.PreviousSibling is WDropDownFormField)
                    return true;
                else if (mark.PreviousSibling is WField && (mark.PreviousSibling as WField).FieldType == FieldType.FieldGoToButton)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Gets the owner section.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private WSection GetOwnerSection(Entity entity)
        {
            while (entity != null && entity.EntityType != EntityType.Section)
                entity = entity.Owner;

            if (entity != null)
                return entity as WSection;
            else
                return null;
        }
        /// <summary>
        /// Inits the cell end pos.
        /// </summary>
        private void InitCellEndPos()
        {
        }
        #endregion
    }
}
