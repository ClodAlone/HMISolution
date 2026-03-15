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
using System.Text;
using System.Xml;
using System.Collections;
using System.Globalization;
using Syncfusion.Layouting;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Net;
#if WINRT
using Syncfusion.DocIO.DLS.Convertors;
using Syncfusion.DocIO.DLS;
using Font = Syncfusion.DocIO.DLS.Font;
using XmlNode = Syncfusion.DocIO.DLS.XNode;
using Windows.Storage;
using XmlAttribute = Syncfusion.DocIO.DLS.XAttribute;
using XmlDocument = Syncfusion.DocIO.DLS.XDocument;
using System.Threading.Tasks;
#else
using System.Drawing;
using Font = System.Drawing.Font;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The default implementation of IHtmlConverter.
    /// </summary>
    internal class HTMLConverterImpl : IHtmlConverter
    {
        #region Constants
        private const string c_Xhtml1StrictSchema = "Syncfusion.DocIO.Resources.xhtml1-strict.xsd";
        private const string c_Xhtml1TransitionalSchema = "Syncfusion.DocIO.Resources.xhtml1-transitional.xsd";
        private const string DEF_WHITESPACE = " ";
        private const string DEF_IMAGENOTFOUND = "Syncfusion.DocIO.Resources.ImageNotFound.jpg";
        private const float DEF_LH_INDENT = 35f;
        private const float DEF_MEDIUMVALUE = 3f;
        private const float DEF_THICKVALUE = 4.5f;
        private const float DEF_THINVALUE = 0.75f;
        private const float DEF_INDENT = 36;
        private const string c_Xhtml1ScrictDocType = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n";
        private const string c_Xhtml1TRansitionalDocType = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n";
        #endregion

        #region Fields
        static readonly Regex m_removeSpaces = new Regex(@"\s+");
        const float c_DefCellWidth = 3f;
        /// <summary>
        /// 
        /// </summary>
        private XmlDocument m_xmlDoc;
        private Stack<TextFormat> m_styleStack = new Stack<TextFormat>();
        private BodyItemCollection m_bodyItems;
        WTextBody m_textBody;
        private Stack<BodyItemCollection> m_nestedBodyItems = new Stack<BodyItemCollection>();
        private Stack<WTable> m_nestedTable = new Stack<WTable>();
        private WParagraph m_currParagraph = null;
        private WTable m_currTable = null;
        private string m_basePath = null;
        private int m_curListLevel = -1;
        private List<int> m_listLevelNo = new List<int>();
        private bool checkFirstElement = false;
        private Stack<ListStyle> m_listStack;
        private Stack<string> m_lfoStack;
        [ThreadStatic]
        static private TextFormat s_defFormat;
        internal float childTableWidth = 0.0f;
        private HorizontalAlignment m_horizontalAlignmentDefinedInCellNode = HorizontalAlignment.Left;
        private HorizontalAlignment m_horizontalAlignmentDefinedInRowNode = HorizontalAlignment.Left;
        private bool m_bBorderStyle = false;
        private bool m_bIsCellStyle = false;
        private int m_currTableFooterRowIndex = -1;
        private TextFormat currDivFormat = null;
        private bool m_bIsInDiv = false;
        private Stack<bool> m_stackCellStyle = new Stack<bool>();
        public IWParagraphStyle m_userStyle = null;
        private TableGrid tableGrid;
        private bool m_bIsInBlockquote;
        private int m_blockquoteLevel = 0;
        private ListStyle m_userListStyle = null;
        private int m_divCount=0;
        private bool m_bIsAlignAttrDefinedInRowNode;
        private bool m_bIsAlignAttriDefinedInCellNode;
        private bool m_bIsVAlignAttriDefinedInRowNode;
        private VerticalAlignment m_verticalAlignmentDefinedInRowNode = VerticalAlignment.Middle;
        private bool m_bIsBorderCollapse;
        private Stack<float> m_listLeftIndentStack = new Stack<float>();
        private bool m_bIsWithinList;
        private Color m_hyperlinkcolor = Color.Empty;
        internal WSection m_currentSection;
        internal HTMLImportSettings HtmlImportSettings;
        #endregion

        #region Properties
        /// <summary>
        /// Get client width of the html textbody
        /// </summary>
        internal float ClientWidth
        {
            get
            {
                float clientWidth = m_textBody.Document.LastSection.PageSetup.ClientWidth;
                if (m_textBody is WTableCell)
                    clientWidth = (m_textBody as WTableCell).Width;
                else if (m_textBody is WTextBox)
                    clientWidth = (m_textBody as IWTextBox).TextBoxFormat.Width;
                else if (m_currentSection != null)
                    clientWidth = m_currentSection.PageSetup.ClientWidth;
                return clientWidth;
            }
        }
        /// <summary>
        /// Get & set the base path
        /// </summary>
        protected string BasePath
        {
            get
            {
                return m_basePath;
            }
            set
            {
                m_basePath = value;
            }
        }
        /// <summary>
        /// Gets the current format.
        /// </summary>
        /// <value>The current format.</value>
        protected TextFormat CurrentFormat
        {
            get
            {
                if (m_styleStack.Count > 0)
                {
                    return (TextFormat)m_styleStack.Peek();
                }

                if (s_defFormat == null)
                {
                    s_defFormat = new TextFormat();
                }

                return s_defFormat;
            }
        }
        /// <summary>
        /// Gets the current para.
        /// </summary>
        /// <value>The current para.</value>
        protected WParagraph CurrentPara
        {
            get
            {
                if (m_currParagraph == null)
                {
                    m_currParagraph = new WParagraph(m_bodyItems.Document);
                    m_bodyItems.Add(m_currParagraph);
                    if (m_userStyle != null)
                        m_currParagraph.ApplyStyle(m_userStyle);
                }
                return m_currParagraph;
            }
        }
        /// <summary>
        /// Gets the lfo stack.
        /// </summary>
        /// <value> lfoName .</value>
        private Stack<string> LfoStack
        {
            get
            {
                if (m_lfoStack == null)
                    m_lfoStack = new Stack<string>();
                return m_lfoStack;
            }
        } 
        /// <summary> 
        /// Gets the list stack.
        /// </summary>
        /// <value>The list stack.</value>
        private Stack<ListStyle> ListStack
        {
            get
            {
                if (m_listStack == null)
                    m_listStack = new Stack<ListStyle>();
                return m_listStack;
            }
        }
        /// <summary>
        /// Gets the current list style.
        /// </summary>
        /// <value>The current list style.</value>
        private ListStyle CurrentListStyle
        {
            get
            {
                return (m_listStack != null && m_listStack.Count != 0) ? m_listStack.Peek() : null;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Appends to text body with style
        /// </summary>
        /// <param name="dlsTextBody">The DLS text body.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="paragraphIndex">Index of the paragraph.</param>
        /// <param name="paragraphItemIndex">Index of the paragraph item.</param>
        public void AppendToTextBody(ITextBody textBody, string html, int paragraphIndex, int paragraphItemIndex, IWParagraphStyle style, ListStyle listStyle)
        {
            if (style != null)
                m_userStyle = style;
            if (listStyle != null)
                m_userListStyle = listStyle;
            AppendToTextBody(textBody, html, paragraphIndex, paragraphItemIndex);
            m_userStyle = null;
            m_userListStyle = null;
        }
        /// <summary>
        /// Appends to text body without style.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="paragraphIndex">Index of the paragraph.</param>
        /// <param name="paragraphItemIndex">Index of the paragraph item.</param>
        public void AppendToTextBody(ITextBody textBody, string html, int paragraphIndex, int paragraphItemIndex)
        {
            textBody.Document.IsOpening = true;
            Init();
            m_basePath = textBody.Document.HtmlBaseUrl;
            m_textBody = textBody as WTextBody;
            m_currentSection = textBody.Owner as WSection;
            TextBodyPart bodyPart = new TextBodyPart(textBody.Document);
            m_bodyItems = bodyPart.BodyItems;
            m_currParagraph = null;
            tableGrid = new TableGrid();
#if WINRT
            XmlReader reader = LoadXhtml(html);
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(reader);
            XmlNode body = xDoc.RootNode;
            XmlNode head = xDoc.RootNode;
#else
            LoadXhtml(html);
            XmlNode body = m_xmlDoc.DocumentElement;
            XmlNode head = m_xmlDoc.DocumentElement;
#endif
            // If root node is "html" and contains child node "body" - take that.
            if (body.LocalName.ToLower() == "html")
            {
                foreach (XmlNode tmpNode in body.ChildNodes)
                {
                    if (tmpNode.LocalName.ToLower() == "head" && tmpNode.NodeType == XmlNodeType.Element)
                    {
                        head = tmpNode;
                        foreach (XmlNode child in head.ChildNodes)
                        {
                            if (child.LocalName.ToLower() == "base" && child.NodeType == XmlNodeType.Element)
                            {
                                BasePath = GetAttributeValue(child, "href");
                                break;
                            }
                        }
                    }
                    if (tmpNode.LocalName.ToLower() == "body")
                    {
                        ParseBodyAttributes(tmpNode);
                        body = tmpNode;
                        break;
                    }
                }
            }
			//Parse Body style
            bool stylePresent = ParseBodyStyle(body, textBody, paragraphIndex);

            TraverseChildNodes(body.ChildNodes);

            LeaveStyle(stylePresent);
            if (m_currParagraph != null)
                ApplyTextFormatting(m_currParagraph.BreakCharacterFormat);
            RemoveLastLineBreakFromParagraph(bodyPart.BodyItems);
            textBody.Document.IsOpening = false;
            bodyPart.PasteAt(textBody, paragraphIndex, paragraphItemIndex);

            //Below case is handled to preserve the style of the firt paragraph within body part after pasting into the document body
            if (bodyPart.BodyItems.Count > 0 && bodyPart.BodyItems[0].EntityType == EntityType.Paragraph)
            {
                WParagraph firstPara = bodyPart.BodyItems[0] as WParagraph;
                WParagraphFormat format = firstPara.ParagraphFormat;
                (textBody.ChildEntities[paragraphIndex] as WParagraph).ParagraphFormat.ImportContainer(format);
                if (!string.IsNullOrEmpty(firstPara.StyleName))
                    (textBody.ChildEntities[paragraphIndex] as WParagraph).ApplyStyle(firstPara.StyleName);
                (textBody.ChildEntities[paragraphIndex] as WParagraph).BreakCharacterFormat.ImportContainer(firstPara.BreakCharacterFormat);
            }
            //Below case is handled to preserve the style of the last paragraph within body part after pasting into the document body
            if (bodyPart.BodyItems.Count > 1 && bodyPart.BodyItems[bodyPart.BodyItems.Count - 1].EntityType == EntityType.Paragraph)
            {
                WParagraph lastPara = (bodyPart.BodyItems[bodyPart.BodyItems.Count - 1] as WParagraph);
                int lastParaIndex = paragraphIndex + bodyPart.BodyItems.Count - 1;
                //Increment the last para index count for the additional empty paragraph that is created after pasting the first bodypart item[table]
                if (bodyPart.BodyItems.Count > 0 && bodyPart.BodyItems[0].EntityType == EntityType.Table)
                    lastParaIndex = lastParaIndex + 1;
                if (textBody.ChildEntities[lastParaIndex].EntityType == EntityType.Paragraph && lastPara .StyleName  != string.Empty)
                {
                    WParagraph para = textBody.ChildEntities[lastParaIndex] as WParagraph;
                    para.ApplyStyle(lastPara .StyleName );
                }
            }
            SetNextStyleForParagraphStyle(m_bodyItems.Document);
        }
        /// <summary>
        /// Parses the Body style.
        /// </summary>
        /// <param name="node">The node.</param>
        private bool ParseBodyStyle(XmlNode node, ITextBody textBody, int paragraphIndex)
        {
            string style = GetAttributeValue(node, "style");
            string[] borderStyle = { "dashed", "dotted", "double", "groove", "inset", "outset", "ridge", "solid", "hidden" };
            if (style.Length != 0)
            {
                // Clones last format
                TextFormat format = AddStyle();
                format.Borders = new TableBorders(null);
                // Splits by tokens
                string[] styleParams = style.Split(';', ':');

                for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
                {
                    char[] trimChar = new char[] { '\'', '\"' };
                    string paramName = styleParams[i].ToLower().Trim();
                    string paramValue = styleParams[i + 1].ToLower().Trim();
                    paramValue = paramValue.Trim(trimChar);
                    GetFormat(format, paramName, paramValue, node);
                }
                if (textBody.ChildEntities.Count == 1 && textBody.ChildEntities[0] is WParagraph && paragraphIndex == 0)
                {
                    //Apply page borders
                    ApplyPageBorder(format);
                    //Apply page margins and background color
                    ApplyPageFormat(format);
                }
                //Reset the Background color
                if (format.HasKey(TextFormat.BackColorKey))
                    format.BackColor = Color.Empty;
                //Reset the page margins
                if (format.HasKey(TextFormat.BottomMarginKey))
                    format.BottomMargin = 0;
                if (format.HasKey(TextFormat.TopMarginKey))
                    format.TopMargin = 0;
                if (format.HasKey(TextFormat.LeftMarginKey))
                    format.LeftMargin = 0;
                if (format.HasKey(TextFormat.RightMarginKey))
                    format.RightMargin = 0;
                //Reset page borders
                format.Borders = new TableBorders(null);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Apply Page margins and background color
        /// </summary>
        /// <param name="format"></param>
        private void ApplyPageFormat(TextFormat format)
        {
            //Apply Page margin
            m_currentSection.PageSetup.Margins.Left += format.LeftMargin;
            m_currentSection.PageSetup.Margins.Right += format.RightMargin;
            m_currentSection.PageSetup.Margins.Top += format.TopMargin;
            m_currentSection.PageSetup.Margins.Bottom += format.BottomMargin;
            //Apply page background color
            if (format.HasKey(TextFormat.BackColorKey))
            {
                m_bodyItems.Document.Background.Type = BackgroundType.Color;
                m_bodyItems.Document.Background.Color = format.BackColor;
            }
        }
        /// <summary>
        /// Apply Page border
        /// </summary>
        /// <param name="pformat"></param>
        /// <param name="format"></param>
        private void ApplyPageBorder(TextFormat format)
        {
            if (format.Borders.AllStyle != BorderStyle.None)
            {
                m_currentSection.PageSetup.Borders.Bottom.BorderType = m_currentSection.PageSetup.Borders.Top.BorderType = 
                m_currentSection.PageSetup.Borders.Left.BorderType = m_currentSection.PageSetup.Borders.Right.BorderType = format.Borders.AllStyle;

                if (format.Borders.AllColor != Color.Empty)
                {
                    m_currentSection.PageSetup.Borders.Bottom.Color = m_currentSection.PageSetup.Borders.Top.Color = 
                    m_currentSection.PageSetup.Borders.Left.Color = m_currentSection.PageSetup.Borders.Right.Color = format.Borders.AllColor;
                }
                if (format.Borders.AllWidth != -1.0f)
                {
                    m_currentSection.PageSetup.Borders.Bottom.LineWidth = m_currentSection.PageSetup.Borders.Top.LineWidth =
                    m_currentSection.PageSetup.Borders.Left.LineWidth = m_currentSection.PageSetup.Borders.Right.LineWidth = format.Borders.AllWidth;
                }
            }
            if (format.Borders.BottomStyle != BorderStyle.None)
            {
                m_currentSection.PageSetup.Borders.Bottom.BorderType = format.Borders.BottomStyle;
                m_currentSection.PageSetup.Borders.Bottom.LineWidth = format.Borders.BottomWidth;
                m_currentSection.PageSetup.Borders.Bottom.Color = format.Borders.BottomColor;
            }
            if (format.Borders.TopStyle != BorderStyle.None)
            {
                m_currentSection.PageSetup.Borders.Top.BorderType = format.Borders.TopStyle;
                m_currentSection.PageSetup.Borders.Top.LineWidth = format.Borders.TopWidth;
                m_currentSection.PageSetup.Borders.Top.Color = format.Borders.TopColor;
            }
            if (format.Borders.LeftStyle != BorderStyle.None)
            {
                m_currentSection.PageSetup.Borders.Left.BorderType = format.Borders.LeftStyle;
                m_currentSection.PageSetup.Borders.Left.LineWidth = format.Borders.LeftWidth;
                m_currentSection.PageSetup.Borders.Left.Color = format.Borders.LeftColor;
            }
            if (format.Borders.RightStyle != BorderStyle.None)
            {
                m_currentSection.PageSetup.Borders.Right.BorderType = format.Borders.RightStyle;
                m_currentSection.PageSetup.Borders.Right.LineWidth = format.Borders.RightWidth;
                m_currentSection.PageSetup.Borders.Right.Color = format.Borders.RightColor;
            }
            if (format.Borders.TopWidth > 0)
                m_currentSection.PageSetup.Borders.Top.LineWidth = format.Borders.TopWidth;
            if (format.Borders.RightWidth > 0)
                m_currentSection.PageSetup.Borders.Right.LineWidth = format.Borders.RightWidth;
            if (format.Borders.LeftWidth > 0)
                m_currentSection.PageSetup.Borders.Left.LineWidth = format.Borders.LeftWidth;
            if (format.Borders.BottomWidth > 0)
                m_currentSection.PageSetup.Borders.Bottom.LineWidth = format.Borders.BottomWidth;

        }
        /// <summary>
        /// Set Next style for the pararaph style
        /// </summary>
        /// <param name="document"></param>
        private void SetNextStyleForParagraphStyle(WordDocument document)
        {
            foreach(Style style in document .Styles)
            {
                if (style.StyleType == StyleType.ParagraphStyle)
                    style.NextStyle = style.Name.Replace (" ",string .Empty); 
            }
        }
        /// <summary>
        /// Parse body attributes
        /// </summary>
        /// <param name="node"></param>
        private void ParseBodyAttributes(XmlNode node)
        {
            string color = string.Empty;
            foreach (XmlAttribute attr in node.Attributes)
            {
                switch (attr.Name.ToLower())
                {
                    case "link":
                        color = GetAttributeValue(node, "link");
                        if (color != string.Empty)
                        {
                            IStyle style = m_bodyItems.Document.Styles.FindByName(BuiltinStyle.Hyperlink.ToString());
                            if (style == null)
                            {
                                style = Style.CreateBuiltinStyle(BuiltinStyle.Hyperlink, StyleType.CharacterStyle, m_bodyItems.Document);
                                m_bodyItems.Document.Styles.Add(style);
                            }
                            (style as Style).CharacterFormat.TextColor = GetColor(color);
                            m_hyperlinkcolor = GetColor(color);
                        }
                        break;
                    case "vlink":
                        color = GetAttributeValue(node, "vlink");
                        if (color != string.Empty)
                        {
                            IStyle style = m_bodyItems.Document.Styles.FindByName(BuiltinStyle.FollowedHyperlink.ToString());
                            if (style == null)
                            {
                                style = Style.CreateBuiltinStyle(BuiltinStyle.FollowedHyperlink, StyleType.CharacterStyle, m_bodyItems.Document);
                                m_bodyItems.Document.Styles.Add(style);
                            }
                            (style as Style).CharacterFormat.TextColor = GetColor(color);
                        }
                        break;
                }
            }
        }
       /// <summary>
       /// Parse child entities of text body and removes last line break from the paragraph
       /// </summary>
       /// <param name="entities"></param>
        private void RemoveLastLineBreakFromParagraph(BodyItemCollection itemCollection)
        {
            foreach (TextBodyItem item in itemCollection )
            {
                if (item.EntityType == EntityType.Paragraph)
                {
                    WParagraph para = item as WParagraph;
                    //Check whether the last 2 item in the paragraph is line break. If so remove the line break
                    if (para.Items.Count > 0 && para.Items[para.Items.Count - 1].EntityType == EntityType.Break 
                        && (para.Items[para.Items.Count - 1] as Break).BreakType == BreakType.LineBreak && 
                         ((para.Items [para .Items .Count -1] as Break ).HtmlToDocLayoutInfo.RemoveLineBreak  ))
                        para.Items.RemoveAt(para.Items.Count - 1);
                    if (para.Items.Count > 0 && para.Items[para.Items.Count - 1].EntityType == EntityType.Break
                        && (para.Items[para.Items.Count - 1] as Break).BreakType == BreakType.LineBreak && 
                        ((para.Items[para.Items.Count - 1] as Break).HtmlToDocLayoutInfo.RemoveLineBreak   ))
                        para.Items.RemoveAt(para.Items.Count - 1);                   
                }
                else if (item.EntityType == EntityType.Table)
                {
                    WTable table = item as WTable;
                    foreach (WTableRow row in table.Rows)
                    {
                        foreach (WTableCell cell in row.Cells)
                        {
                            RemoveLastLineBreakFromParagraph(cell.Items);
                        }
                    }
                }
            }
        }
#if!SILVERLIGHT && !WP
        /// <summary>
        /// Validates the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	If the specified HTML is valid, set to <c>true</c>.
        /// </returns>
        public bool IsValid(string html, XHTMLValidationType type)
        {
            string error;
            return IsValid(html, type, out error);
        }
        /// <summary>
        /// Determines whether the specified HTML is valid.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="type">The type.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>
        /// 	If the specified HTML is valid, set to <c>true</c>.
        /// </returns>
        public bool IsValid(string html, XHTMLValidationType type, out string exceptionMessage)
        {
            exceptionMessage = string.Empty;
            Assembly execAssm = Assembly.GetExecutingAssembly();
            XmlSchema schema = null;
            html = ReplaceHtmlConstantByUnicodeChar(html);
            switch (type)
            {
                case XHTMLValidationType.Strict:
                    Stream stream1 = execAssm.GetManifestResourceStream(c_Xhtml1StrictSchema);
                    schema = XmlSchema.Read(stream1, new ValidationEventHandler(OnValidation));
                    break;
                case XHTMLValidationType.Transitional:
                    Stream stream2 = execAssm.GetManifestResourceStream(c_Xhtml1TransitionalSchema);
                    schema = XmlSchema.Read(stream2, new ValidationEventHandler(OnValidation));
                    break;
                case XHTMLValidationType.None:
                    return true;
                default:
                    break;
            }
            m_xmlDoc = new XmlDocument();
            m_xmlDoc.PreserveWhitespace = true;
            html = PrepareHtml(html, schema);

            try
            {
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.Schemas.Add(schema);
                XmlReader reader = XmlReader.Create(new StringReader(html), readerSettings);
                m_xmlDoc.Load(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                return false;
            }

            return true;
        }
#endif
        /// <summary>
        /// Replace constants by Unicode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlConstantByUnicodeChar(string html)
        {
            html = ReplaceHtmlSpecialCharacters(html);
            html = ReplaceHtmlSymbols(html);
            html = ReplaceHtmlCharacters(html);
            html = ReplaceHtmlMathSymbols(html);
            html = ReplaceHtmlGreekLetters(html);
            html = ReplaceHtmlOtherEntities(html);
            return html;
        }
        /// <summary>
        /// Replace HtmlSpecialCharacter's EntityNames by EntityNumbers
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlSpecialCharacters(string html)
        {
            //quotation mark
            html = html.Replace("&quot;", "&#34;");
            //apostrophe 
            html = html.Replace("&apos;", "&#39;");
            //ampersand
            html = html.Replace("&amp;", "&#38;");
            //less-than
            html = html.Replace("&lt;", "&#60;");
            //greater-than
            html = html.Replace("&gt;", "&#62;");
            return html;
        }
        /// <summary>
        /// Replace HtmlSymbol's EntityNames by EntityNumbers
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlSymbols(string html)
        {
            //non-breaking space
            html = html.Replace("&nbsp;", "&#160;");
            //inverted exclamation mark
            html = html.Replace("&iexcl;", "&#161;");
            //cent
            html = html.Replace("&cent;", "&#162;");
            //pound
            html = html.Replace("&pound;", "&#163;");
            //currency
            html = html.Replace("&curren;", "&#164;");
            //yen
            html = html.Replace("&yen;", "&#165;");
            //broken vertical bar
            html = html.Replace("&brvbar;", "&#166;");
            //section
            html = html.Replace("&sect;", "&#167;");
            //spacing diaeresis
            html = html.Replace("&uml;", "&#168;");
            //copyright
            html = html.Replace("&copy;", "&#169;");
            //feminine ordinal indicator
            html = html.Replace("&ordf;", "&#170;");
            //angle quotation mark (left)
            html = html.Replace("&laquo;", "&#171;");
            //negation
            html = html.Replace("&not;", "&#172;");
            //soft hyphen
            html = html.Replace("&shy;", "&#173;");
            //registered trademark
            html = html.Replace("&reg;", "&#174;");
            //spacing macron
            html = html.Replace("&macr;", "&#175;");
            //degree
            html = html.Replace("&deg;", "&#176;");
            //plus-or-minus 
            html = html.Replace("&plusmn;", "&#177;");
            //superscript 2
            html = html.Replace("&sup2;", "&#178;");
            //superscript 3
            html = html.Replace("&sup3;", "&#179;");
            //spacing acute
            html = html.Replace("&acute;", "&#180;");
            //micro
            html = html.Replace("&micro;", "&#181;");
            //paragraph
            html = html.Replace("&para;", "&#182;");
            //middle dot
            html = html.Replace("&middot;", "&#183;");
            //spacing cedilla
            html = html.Replace("&cedil;", "&#184;");
            //superscript 1
            html = html.Replace("&sup1;", "&#185;");
            //masculine ordinal indicator
            html = html.Replace("&ordm;", "&#186;");
            //angle quotation mark (right)
            html = html.Replace("&raquo;", "&#187;");
            //fraction 1/4
            html = html.Replace("&frac14;", "&#188;");
            //fraction 1/2
            html = html.Replace("&frac12;", "&#189;");
            //fraction 3/4
            html = html.Replace("&frac34;", "&#190;");
            //inverted question mark
            html = html.Replace("&iquest;", "&#191;");
            //multiplication
            html = html.Replace("&times;", "&#215;");
            //division
            html = html.Replace("&divide;", "&#247;");
            return html;
        }
        /// <summary>
        /// Replace HtmlCharacter's EntityNames by EntityNumbers
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlCharacters(string html)
        {
            //capital a, grave accent
            html = html.Replace("&Agrave;", "&#192;");
            //capital a, acute accent
            html = html.Replace("&Aacute;", "&#193;");
            //capital a, circumflex accent
            html = html.Replace("&Acirc;", "&#194;");
            //capital a, tilde
            html = html.Replace("&Atilde;", "&#195;");
            //capital a, umlaut mark
            html = html.Replace("&Auml;", "&#196;");
            //capital a, ring
            html = html.Replace("&Aring;", "&#197;");
            //capital ae
            html = html.Replace("&AElig;", "&#198;");
            //capital c, cedilla
            html = html.Replace("&Ccedil;", "&#199;");
            //capital e, grave accent
            html = html.Replace("&Egrave;", "&#200;");
            //capital e, acute accent
            html = html.Replace("&Eacute;", "&#201;");
            //capital e, circumflex accent
            html = html.Replace("&Ecirc;", "&#202;");
            //capital e, umlaut mark
            html = html.Replace("&Euml;", "&#203;");
            //capital i, grave accent
            html = html.Replace("&Igrave;", "&#204;");
            //capital i, acute accent
            html = html.Replace("&Iacute;", "&#205;");
            //capital i, circumflex accent
            html = html.Replace("&Icirc;", "&#206;");
            //capital i, umlaut mark
            html = html.Replace("&Iuml;", "&#207;");
            //capital eth, Icelandic
            html = html.Replace("&ETH;", "&#208;");
            //capital n, tilde
            html = html.Replace("&Ntilde;", "&#209;");
            //capital o, grave accent
            html = html.Replace("&Ograve;", "&#210;");
            //capital o, acute accent
            html = html.Replace("&Oacute;", "&#211;");
            //capital o, circumflex accent
            html = html.Replace("&Ocirc;", "&#212;");
            //capital o, tilde
            html = html.Replace("&Otilde;", "&#213;");
            //capital o, umlaut mark
            html = html.Replace("&Ouml;", "&#214;");
            //capital o, slash
            html = html.Replace("&Oslash;", "&#216;");
            //capital u, grave accent
            html = html.Replace("&Ugrave;", "&#217;");
            //capital u, acute accent
            html = html.Replace("&Uacute;", "&#218;");
            //capital u, circumflex accent
            html = html.Replace("&Ucirc;", "&#219;");
            //capital u, umlaut mark
            html = html.Replace("&Uuml;", "&#220;");
            //capital y, acute accent
            html = html.Replace("&Yacute;", "&#221;");
            //capital THORN, Icelandic
            html = html.Replace("&THORN;", "&#222;");
            //small sharp s, German
            html = html.Replace("&szlig;", "&#223;");
            //small a, grave accent
            html = html.Replace("&agrave;", "&#224;");
            //small a, acute accent
            html = html.Replace("&aacute;", "&#225;");
            //small a, circumflex accent
            html = html.Replace("&acirc;", "&#226;");
            //small a, tilde
            html = html.Replace("&atilde;", "&#227;");
            //small a, umlaut mark
            html = html.Replace("&auml;", "&#228;");
            //small a, ring
            html = html.Replace("&aring;", "&#229;");
            //small ae
            html = html.Replace("&aelig;", "&#230;");
            //small c, cedilla
            html = html.Replace("&ccedil;", "&#231;");
            //small e, grave accent
            html = html.Replace("&egrave;", "&#232;");
            //small e, acute accent
            html = html.Replace("&eacute;", "&#233;");
            //small e, circumflex accent
            html = html.Replace("&ecirc;", "&#234;");
            //small e, umlaut mark
            html = html.Replace("&euml;", "&#235;");
            //small i, grave accent
            html = html.Replace("&igrave;", "&#236;");
            //small i, acute accent
            html = html.Replace("&iacute;", "&#237;");
            //small i, circumflex accent
            html = html.Replace("&icirc;", "&#238;");
            //small i, umlaut mark
            html = html.Replace("&iuml;", "&#239;");
            //small eth, Icelandic
            html = html.Replace("&eth;", "&#240;");
            //small n, tilde
            html = html.Replace("&ntilde;", "&#241;");
            //small o, grave accent
            html = html.Replace("&ograve;", "&#242;");
            //small o, acute accent
            html = html.Replace("&oacute;", "&#243;");
            //small o, circumflex accent
            html = html.Replace("&ocirc;", "&#244;");
            //small o, tilde
            html = html.Replace("&otilde;", "&#245;");
            //small o, umlaut mark
            html = html.Replace("&ouml;", "&#246;");
            //small o, slash
            html = html.Replace("&oslash;", "&#248;");
            //small u, grave accent
            html = html.Replace("&ugrave;", "&#249;");
            //small u, acute accent
            html = html.Replace("&uacute;", "&#250;");
            //small u, circumflex accent
            html = html.Replace("&ucirc;", "&#251;");
            //small u, umlaut mark
            html = html.Replace("&uuml;", "&#252;");
            //small y, acute accent
            html = html.Replace("&yacute;", "&#253;");
            //small thorn, Icelandic
            html = html.Replace("&thorn;", "&#254;");
            //small y, umlaut mark
            html = html.Replace("&yuml;", "&#255;");
            return html;
        }
        /// <summary>
        /// Replace HtmlMathSymbol's EntityNames by EntityNumbers
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlMathSymbols(string html)
        {
            //for all
            html = html.Replace("&forall;", "&#8704;");
            //part
            html = html.Replace("&part;", "&#8706;");
            //exists
            html = html.Replace("&exist;", "&#8707;");
            //empty
            html = html.Replace("&empty;", "&#8709;");
            //nabla
            html = html.Replace("&nabla;", "&#8711;");
            //isin
            html = html.Replace("&isin;", "&#8712;");
            //notin
            html = html.Replace("&notin;", "&#8713;");
            //ni
            html = html.Replace("&ni;", "&#8715;");
            //prod
            html = html.Replace("&prod;", "&#8719;");
            //sum
            html = html.Replace("&sum;", "&#8721;");
            //minus
            html = html.Replace("&minus;", "&#8722;");
            //lowast
            html = html.Replace("&lowast;", "&#8727;");
            //square root
            html = html.Replace("&radic;", "&#8730;");
            //proportional to
            html = html.Replace("&prop;", "&#8733;");
            //infinity
            html = html.Replace("&infin;", "&#8734;");
            //angle
            html = html.Replace("&ang;", "&#8736;");
            //and
            html = html.Replace("&and;", "&#8743;");
            //or
            html = html.Replace("&or;", "&#8744;");
            //cap
            html = html.Replace("&cap;", "&#8745;");
            //cup
            html = html.Replace("&cup;", "&#8746;");
            //integral
            html = html.Replace("&int;", "&#8747;");
            //therefore
            html = html.Replace("&there4;", "&#8756;");
            //similar to
            html = html.Replace("&sim;", "&#8764;");
            //congruent to
            html = html.Replace("&cong;", "&#8773;");
            //almost equal
            html = html.Replace("&asymp;", "&#8776;");
            //not equal
            html = html.Replace("&ne;", "&#8800;");
            //equivalent
            html = html.Replace("&equiv;", "&#8801;");
            //less or equal
            html = html.Replace("&le;", "&#8804;");
            //greater or equal
            html = html.Replace("&ge;", "&#8805;");
            //subset of
            html = html.Replace("&sub;", "&#8834;");
            //superset of
            html = html.Replace("&sup;", "&#8835;");
            //not subset of
            html = html.Replace("&nsub;", "&#8836;");
            //subset or equal
            html = html.Replace("&sube;", "&#8838;");
            //superset or equal
            html = html.Replace("&supe;", "&#8839;");
            //circled plus
            html = html.Replace("&oplus;", "&#8853;");
            //cirled times
            html = html.Replace("&otimes;", "&#8855;");
            //perpendicular
            html = html.Replace("&perp;", "&#8869;");
            //dot operator
            html = html.Replace("&sdot;", "&#8901;");
            //frasl character
            html = html.Replace("&frasl;", "&#8260;");
            return html;
        }
        /// <summary>
        /// Replace HtmlGreekLetter's EntityNames by EntityNumbers
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlGreekLetters(string html)
        {
            //Alpha
            html = html.Replace("&Alpha;", "&#913;");
            //Beta
            html = html.Replace("&Beta;", "&#914;");
            //Gamma
            html = html.Replace("&Gamma;", "&#915;");
            //Delta
            html = html.Replace("&Delta;", "&#916;");
            //Epsilon
            html = html.Replace("&Epsilon;", "&#917;");
            //Zeta
            html = html.Replace("&Zeta;", "&#918;");
            //Eta
            html = html.Replace("&Eta;", "&#919;");
            //Theta
            html = html.Replace("&Theta;", "&#920;");
            //Iota
            html = html.Replace("&Iota;", "&#921;");
            //Kappa
            html = html.Replace("&Kappa;", "&#922;");
            //Lambda
            html = html.Replace("&Lambda;", "&#923;");
            //Mu
            html = html.Replace("&Mu;", "&#924;");
            //Nu
            html = html.Replace("&Nu;", "&#925;");
            //Xi
            html = html.Replace("&Xi;", "&#926;");
            //Omicron
            html = html.Replace("&Omicron;", "&#927;");
            //Pi
            html = html.Replace("&Pi;", "&#928;");
            //Rho
            html = html.Replace("&Rho;", "&#929;");
            //Sigma
            html = html.Replace("&Sigma;", "&#931;");
            //Tau
            html = html.Replace("&Tau;", "&#932;");
            //Upsilon
            html = html.Replace("&Upsilon;", "&#933;");
            //Phi
            html = html.Replace("&Phi;", "&#934;");
            //Chi
            html = html.Replace("&Chi;", "&#935;");
            //Psi
            html = html.Replace("&Psi;", "&#936;");
            //Omega
            html = html.Replace("&Omega;", "&#937;");
            //alpha
            html = html.Replace("&alpha;", "&#945;");
            //beta
            html = html.Replace("&beta;", "&#946;");
            //gamma
            html = html.Replace("&gamma;", "&#947;");
            //delta
            html = html.Replace("&delta;", "&#948;");
            //epsilon
            html = html.Replace("&epsilon;", "&#949;");
            //zeta
            html = html.Replace("&zeta;", "&#950;");
            //eta
            html = html.Replace("&eta;", "&#951;");
            //theta
            html = html.Replace("&theta;", "&#952;");
            //iota
            html = html.Replace("&iota;", "&#953;");
            //kappa
            html = html.Replace("&kappa;", "&#954;");
            //lambda
            html = html.Replace("&lambda;", "&#955;");
            //mu
            html = html.Replace("&mu;", "&#956;");
            //nu
            html = html.Replace("&nu;", "&#957;");
            //xi
            html = html.Replace("&xi;", "&#958;");
            //omicron
            html = html.Replace("&omicron;", "&#959;");
            //pi
            html = html.Replace("&pi;", "&#960;");
            //rho
            html = html.Replace("&rho;", "&#961;");
            //sigmaf
            html = html.Replace("&sigmaf;", "&#962;");
            //sigma
            html = html.Replace("&sigma;", "&#963;");
            //tau
            html = html.Replace("&tau;", "&#964;");
            //upsilon
            html = html.Replace("&upsilon;", "&#965;");
            //phi
            html = html.Replace("&phi;", "&#966;");
            //chi
            html = html.Replace("&chi;", "&#967;");
            //psi
            html = html.Replace("&psi;", "&#968;");
            //omega
            html = html.Replace("&omega;", "&#969;");
            //theta symbol
            html = html.Replace("&thetasym;", "&#977;");
            //upsilon symbol
            html = html.Replace("&upsih;", "&#978;");
            //pi symbol
            html = html.Replace("&piv;", "&#982;");
            return html;
        }
        /// <summary>
        /// Replace Html Other EntityNames by EntityNumbers
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlOtherEntities(string html)
        {
            //capital ligature OE
            html = html.Replace("&OElig;", "&#338;");
            //small ligature oe
            html = html.Replace("&oelig;", "&#339;");
            //capital S with caron
            html = html.Replace("&Scaron;", "&#352;");
            //small S with caron
            html = html.Replace("&scaron;", "&#353;");
            //capital Y with diaeres
            html = html.Replace("&Yuml;", "&#376;");
            //f with hook
            html = html.Replace("&fnof;", "&#402;");
            //modifier letter circumflex accent
            html = html.Replace("&circ;", "&#710;");
            //small tilde
            html = html.Replace("&tilde;", "&#732;");
            //en space
            html = html.Replace("&ensp;", "&#8194;");
            //em space
            html = html.Replace("&emsp;", "&#8195;");
            //thin space
            html = html.Replace("&thinsp;", "&#8201;");
            //zero width non-joiner
            html = html.Replace("&zwnj;", "&#8204;");
            //zero width joiner
            html = html.Replace("&zwj;", "&#8205;");
            //left-to-right mark
            html = html.Replace("&lrm;", "&#8206;");
            //right-to-left mark
            html = html.Replace("&rlm;", "&#8207;");
            //en dash
            html = html.Replace("&ndash;", "&#8211;");
            //em dash
            html = html.Replace("&mdash;", "&#8212;");
            //left single quotation mark
            html = html.Replace("&lsquo;", "&#8216;");
            //right single quotation mark
            html = html.Replace("&rsquo;", "&#8217;");
            //single low-9 quotation mark
            html = html.Replace("&sbquo;", "&#8218;");
            //left double quotation mark
            html = html.Replace("&ldquo;", "&#8220;");
            //right double quotation mark
            html = html.Replace("&rdquo;", "&#8221;");
            //double low-9 quotation mark
            html = html.Replace("&bdquo;", "&#8222;");
            //dagger
            html = html.Replace("&dagger;", "&#8224;");
            //double dagger
            html = html.Replace("&Dagger;", "&#8225;");
            //bullet
            html = html.Replace("&bull;", "&#8226;");
            //horizontal ellipsis
            html = html.Replace("&hellip;", "&#8230;");
            //per mille 
            html = html.Replace("&permil;", "&#8240;");
            //minutes
            html = html.Replace("&prime;", "&#8242;");
            //seconds
            html = html.Replace("&Prime;", "&#8243;");
            //single left angle quotation
            html = html.Replace("&lsaquo;", "&#8249;");
            //single right angle quotation
            html = html.Replace("&rsaquo;", "&#8250;");
            //overline
            html = html.Replace("&oline;", "&#8254;");
            //euro
            html = html.Replace("&euro;", "&#8364;");
            //trademark
            html = html.Replace("&trade;", "&#8482;");
            //left arrow
            html = html.Replace("&larr;", "&#8592;");
            //up arrow
            html = html.Replace("&uarr;", "&#8593;");
            //right arrow
            html = html.Replace("&rarr;", "&#8594;");
            //down arrow
            html = html.Replace("&darr;", "&#8595;");
            //left right arrow
            html = html.Replace("&harr;", "&#8596;");
            //carriage return arrow
            html = html.Replace("&crarr;", "&#8629;");
            //left double arrow
            html = html.Replace("&lArr;", "&#8656;");
            //up double arrow
            html = html.Replace("&uArr;", "&#8657;");
            //right double arrow
            html = html.Replace("&rArr;", "&#8658;");
            //down double arrow
            html = html.Replace("&dArr;", "&#8659;");
            //left right double arrow
            html = html.Replace("&hArr;", "&#8660;");
            //left ceiling
            html = html.Replace("&lceil;", "&#8968;");
            //right ceiling
            html = html.Replace("&rceil;", "&#8969;");
            //left floor
            html = html.Replace("&lfloor;", "&#8970;");
            //right floor
            html = html.Replace("&rfloor;", "&#8971;");
            //lozenge
            html = html.Replace("&loz;", "&#9674;");
            //spade
            html = html.Replace("&spades;", "&#9824;");
            //club
            html = html.Replace("&clubs;", "&#9827;");
            //heart
            html = html.Replace("&hearts;", "&#9829;");
            //diamond
            html = html.Replace("&diams;", "&#9830;");
            //left-pointing angle bracket
            html = html.Replace("&lang;", "&#9001;");
            //right-pointing angle bracket
            html = html.Replace("&rang;", "&#9002;");
            return html;
        }
        #endregion

        #region Implementation
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Loads the XHTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        private void LoadXhtml(string html)
        {
            Assembly execAssm = Assembly.GetExecutingAssembly();
            XmlSchema schema = null;
            html = html.Replace("&nbsp;", "nbsp;");

            switch (m_bodyItems.Document.XHTMLValidateOption)
            {
                case XHTMLValidationType.Strict:
                    Stream stream1 = execAssm.GetManifestResourceStream(c_Xhtml1StrictSchema);
                    schema = XmlSchema.Read(stream1, new ValidationEventHandler(OnValidation));
                    break;
                case XHTMLValidationType.Transitional:
                    Stream stream2 = execAssm.GetManifestResourceStream(c_Xhtml1TransitionalSchema);
                    schema = XmlSchema.Read(stream2, new ValidationEventHandler(OnValidation));
                    break;
                case XHTMLValidationType.None:
                    break;
                default:
                    break;
            }

            try
            {
                m_xmlDoc = new XmlDocument();
                m_xmlDoc.PreserveWhitespace = true;
                LoadXhtml(html, schema);
            }
            catch (XmlException ex)
            {
                throw new NotSupportedException("DocIO support only welformatted xhtml \nDetails:\n" + ex.Message, ex);
            }
        }
        /// <summary>
        /// Load XHTML
        /// </summary>
        /// <param name="html"></param>
        /// <param name="schema"></param>
        private void LoadXhtml(string html, XmlSchema schema)
        {
            html = PrepareHtml(html, schema);
            if (schema != null)
            {
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.Schemas.Add(schema);
                readerSettings.ValidationEventHandler += readerSettings_ValidationEventHandler;
                XmlReader reader = XmlReader.Create(new StringReader(html), readerSettings);
                m_xmlDoc.Load(reader);
                reader.Close();
            }
            else
            {
                m_xmlDoc.LoadXml(html);
            }
        }

        void readerSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw new NotSupportedException("DocIO support only welformatted xhtml \nDetails:\n" + e.Exception.Message, e.Exception);
        }
#endif
#if WINRT
        /// <summary>
        /// Load XHTML
        /// </summary>
        /// <param name="html"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private XmlReader LoadXhtml(string html)
        {
            try
            {
                html = PrepareHtml(html, null);
                MemoryStream htmlstream = new MemoryStream();
                StreamWriter writer = new StreamWriter(htmlstream);
                writer.Write(html);
                writer.Flush();
                XmlReader reader = UtilityMethods.CreateReader(htmlstream);
                return reader;
            }
            catch (XmlException ex)
            {
                throw new NotSupportedException("DocIO support only welformatted xhtml \nDetails:\n" + ex.Message, ex);
            }
        }
#endif
        /// <summary>
        /// Prepares the HTML string.
        /// </summary>
        /// <param name="html">The HTML string.</param>
        /// <param name="schema">The schema.</param>
        private string PrepareHtml(string html, XmlSchema schema)
        {          
            html = ReplaceHtmlConstantByUnicodeChar(html);
            html = RemoveXmlAndDocTypeElement(html, schema);
            html = InsertHtmlElement(html, schema);
            return html;
        }
        /// <summary>
        /// Remove Xml and DocType element from html start
        /// </summary>
        /// <param name="html"></param>
        /// <param name="schema"></param>
        /// <param name="htmlBegin"></param>
        /// <returns></returns>
        private string RemoveXmlAndDocTypeElement(string html, XmlSchema schema)
        {
            int startIndex = 0;
            int endIndex = 0;
            while (true)
            {
				//Remove Empty spaces
                html = html.TrimStart();
                if (html.ToLower().StartsWith("<?xml version") || html.ToLower().StartsWith("<?xmlversion") || html.ToLower().StartsWith("<xml"))
                {
                    endIndex = html.IndexOf(">");
                    html = html.Remove(0, endIndex + 1);
                }
                else if (html.ToLower().StartsWith("<!doctype"))
                {
                    endIndex = html.IndexOf(">");
                    html = html.Remove(0, endIndex + 1);
                }
                else if (html.StartsWith("\r") || html.StartsWith("\n"))
                {
                    html = html.Remove(0, 1);
                }              
                else
                    break;
            }
            return html;
        }
        /// <summary>
        /// Insert DocType elemnet and html start element based on XHTMLValidationType
        /// </summary>
        /// <param name="html"></param>
        /// <param name="schema"></param>
        /// <param name="htmlBegin"></param>
        /// <returns></returns>
        private string InsertHtmlElement(string html, XmlSchema schema)
        {
            string htmlBegin = "<html>";
#if !WINRT
            if (schema != null)
                htmlBegin = "<html xmlns=\"" + schema.TargetNamespace + "\">";
#endif

            if (html.ToLower().StartsWith("<html"))
            {
                if (schema != null)
                {
                    int endIndex = html.IndexOf("<body");
                    html = htmlBegin + "<head><title></title></head>" + html.Remove(0, endIndex);
                }
            }
            else if (html.ToLower().StartsWith("<head"))
            {
                html = htmlBegin + html + "</html>";
            }
            else if (html.ToLower().StartsWith("<body"))
            {
                html = htmlBegin + "<head><title></title></head>" + html + "</html>";
            }
            else
            {
                html = htmlBegin + "<head><title></title></head><body>" + html + "</body></html>";
            }
            return html;
        }
        /// <summary>
        /// Traverses the child nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
#if WINRT
         private void TraverseChildNodes(List<XmlNode> nodes)
#else
         private void TraverseChildNodes(XmlNodeList nodes)
#endif 
        {
            XmlNode prevNode = null;
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Text)
                {
                    TraverseTextWithinTag(node, prevNode);
                }
                else if (node.NodeType == XmlNodeType.Element)
                {
                    ParseTags(node);
                }
                else if (node.NodeType == XmlNodeType.Whitespace)
                {
                    if (node.Value.StartsWith(" ") && m_currParagraph != null)
                    {
                        TraverseTextWithinTag(node, prevNode);
                    }
                }
                prevNode = node;
            }
        }
        /// <summary>
        /// Traverse text within the tag
        /// </summary>
        /// <param name="node"></param>
        /// <param name="prevNode"></param>
        private void TraverseTextWithinTag(XmlNode node, XmlNode prevNode)
        {
            if (IsPargraphNeedToBeAdded(prevNode))
            {
                AddNewParagraph(node);
                if (m_bIsInBlockquote)
                    CurrentPara.ParagraphFormat.LeftIndent = CurrentPara.ParagraphFormat.LeftIndent + (m_blockquoteLevel * 36);
            }
            else if (m_currParagraph == null)
            {
                if ((node.ParentNode.LocalName.ToLower() == "div" && IsFirstNode(node))
                    || (node.ParentNode.LocalName.ToLower() == "span")
                    || node.ParentNode.LocalName.ToLower() == "blockquote")
                {
                    AddNewParagraph(node);
                    if (m_bIsInBlockquote)
                        CurrentPara.ParagraphFormat.LeftIndent = CurrentPara.ParagraphFormat.LeftIndent + (m_blockquoteLevel * 36);
                }
            }
           
            
              
            string bookmarkname = GetAttributeValue(node.ParentNode, "id");
            if (bookmarkname != string.Empty)
            {
                CurrentPara.AppendBookmarkStart(bookmarkname);
            }

            string text = node.InnerText.Replace('\n', ' ').Replace('\r', ' ');
            if (text != " " && !(m_styleStack.Count > 0 && m_styleStack.Peek().IsPreserveWhiteSpace))
            {
                text = m_removeSpaces.Replace(text, " ");
            }
            text = text.Replace("nbsp;", ((char)160).ToString());
            if (!(m_styleStack.Count > 0 && m_styleStack.Peek().IsPreserveWhiteSpace))
                text = RemoveWhiteSpacesAtParagraphBegin(text, CurrentPara);
            if (node.ParentNode.LocalName.ToLower() == "title")
                m_bodyItems.Document.BuiltinDocumentProperties.Title = text;
            else  if (!(node.ParentNode.LocalName == "body" && node.PreviousSibling == null))
            {
                if (node.ParentNode.LocalName == "p" && checkFirstElement == true)
                {
                    AddNewParagraph(node);
                    checkFirstElement = true;
                }
                if (node.ParentNode.LocalName == "body")
                {
                    ApplyParagraphStyle();
                }
                if (text != string.Empty && !(text == " " && node.ParentNode.LocalName == "br"))
                {
                    IWTextRange tr = CurrentPara.AppendText(text);
                    ApplyTextFormatting(tr.CharacterFormat);
                }
                if (node.ParentNode.LocalName == "span")
                    ApplySpanParagraphFormat();
            }
            else
            {
                IWTextRange tr = CurrentPara.AppendText(text);
                ApplyTextFormatting(tr.CharacterFormat );
            }
            if (bookmarkname != string.Empty)
            {
                CurrentPara.AppendBookmarkEnd(bookmarkname);
            }
        }

        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        private void ApplySpanParagraphFormat()
        {
            if (CurrentPara != null)
            {
                WParagraphFormat paraformat = CurrentPara.ParagraphFormat;
                TextFormat format = CurrentFormat;

                if (format.HasValue(TextFormat.TextAlignKey))
                    paraformat.HorizontalAlignment = format.TextAlign;
                if (format.HasValue(TextFormat.LineHeightKey))
                {
                    paraformat.LineSpacingRule = format.LineSpacingRule;
                    paraformat.LineSpacing = format.LineHeight;
                }
                if (format.IsLineHeightNormal)
                {
                    paraformat.SpaceAfterAuto = true;
                    paraformat.SpaceBeforeAuto = true;
                }
                if (m_bIsWithinList && !(m_currParagraph.IsInCell && m_currParagraph.ListFormat.CurrentListStyle == null))
                {
                    //Need to adjust the left indent for all the paragraph within <li> tag
                    if (m_listLeftIndentStack.Count == 0)
                        paraformat.LeftIndent = format.LeftMargin + (DEF_INDENT * (m_curListLevel + 1));
                    else
                        paraformat.LeftIndent = format.LeftMargin + m_listLeftIndentStack.Peek();
                }
                else if (format.HasValue(TextFormat.LeftMarginKey))
                    paraformat.LeftIndent = format.LeftMargin;
                if (format.HasValue(TextFormat.TextIndentKey))
                    paraformat.FirstLineIndent = format.TextIndent;
                if (format.HasValue(TextFormat.RightMarginKey))
                    paraformat.RightIndent = format.RightMargin;
                if (format.HasValue(TextFormat.PageBreakBeforeKey))
                {
                    paraformat.PageBreakBefore = format.PageBreakBefore;
                    //TODO: The following fix has been handled as a work around for the preservation page-break-before in heading tag along with horizontal alignment
                    paraformat.ParaProps.ParagraphPropertyException.PageBreakBefore = true;
                }
                if (format.HasValue(TextFormat.PageBreakAfterKey))
                    paraformat.PageBreakAfter = format.PageBreakAfter;
                if (format.HasValue(TextFormat.WordWrapKey))
                    paraformat.WordWrap = format.WordWrap;
                if (format.HasValue(TextFormat.BackColorKey))
                    paraformat.BackColor = format.BackColor;
            }
        }
        /// <summary>
        /// Checks whether the paragraph need to be added
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsPargraphNeedToBeAdded(XmlNode node)
        {
            if (node != null &&
             ((node.LocalName.ToLower() == "dt" || node.LocalName.ToLower() == "dd") ||
             (node.LocalName.ToLower() == "h1" || node.LocalName.ToLower() == "h2" ||
             node.LocalName.ToLower() == "h3" || node.LocalName.ToLower() == "h4" ||
             node.LocalName.ToLower() == "h5" || node.LocalName.ToLower() == "h6") ||
             node.LocalName.ToLower() == "div" || node.LocalName.ToLower() == "p" ||
             node.LocalName.ToLower() == "ul" || node.LocalName.ToLower() == "ol" || node.LocalName.ToLower() == "table"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Remove white spaces at the begining of paragraph
        /// </summary>
        /// <param name="text"></param>
        /// <param name="CurrentPara"></param>
        /// <returns></returns>
        private string RemoveWhiteSpacesAtParagraphBegin(string text, WParagraph CurrentPara)
        {
            if (text.StartsWith(" "))
            {
                if (CurrentPara.ChildEntities.LastItem != null && CurrentPara.ChildEntities.LastItem.EntityType == EntityType.Break)
                {
                    text = text.TrimStart();
                }
                else if (CurrentPara.Text == "" || CurrentPara.Text == null)
                {
                    text = text.TrimStart();
                }
            }
            return text;
        }
        /// <summary>
        /// Adds the new paragraph.
        /// </summary>
        private void AddNewParagraph(XmlNode node)
        {
            m_currParagraph = new WParagraph(m_bodyItems.Document);
            m_bodyItems.Add(m_currParagraph);
            m_currParagraph.ParagraphFormat.BeforeSpacing = 0f;
            if (currDivFormat != null && (!(m_bIsInDiv && m_currParagraph.IsInCell) || NodeIsInDiv(node)))
            {
                ApplyDivParagraphFormat(node);
            }
        }
        /// <summary>
        /// Determine whether the Node is in Div
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool NodeIsInDiv(XmlNode node)
        {
            XmlNode parentNode = node.ParentNode;
            while (parentNode != null && parentNode.LocalName.ToLower() != "td" && parentNode.LocalName.ToLower() != "th")
            {
                if (parentNode.LocalName.ToLower() == "div")
                    return true;
                else
                    parentNode = parentNode.ParentNode;
            }
            return false;
        }
        ///<summary>
        ///Parses paragraph tags
        ///</summary>
        private void TraverseParagraphTag(XmlNode node)
        {
            ApplyParagraphFormat(node);
            TraverseChildNodes(node.ChildNodes);
            ApplyParagraphStyle();

            //if (IsListNodeEnd(node) && (CurrentFormat.NumBulleted || CurrentFormat.DefBulleted))
           // {
           //     m_curListLevel--;
           //     ListStack.Pop();
          //  }

        }
        /// <summary>
        /// Checks whether this is a first sibling of the parent node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsFirstNode(XmlNode node)
        {
            while (true)
            {
                if (node.PreviousSibling == null)
                    return true;
                else if (node.PreviousSibling.Name == @"#whitespace"
                    || node.PreviousSibling.Name == "#significant-whitespace")
                    node = node.PreviousSibling;
                else
                {
                    if (node .PreviousSibling.NodeType != XmlNodeType.Text &&  IsEmptyNode(node.PreviousSibling))
                        node = node.PreviousSibling;
                    else
                        return false;
                }
            }
        }
        /// <summary>
        /// Parses the tags.
        /// </summary>
        /// <param name="node">The node.</param>
        private void ParseTags(XmlNode node)
        {
            string tagName = node.Name.ToLower();
            TextFormat tf;

            switch (tagName)
            {
                case "dir":
                case "body":
                    TraverseChildNodes(node.ChildNodes);
                    break;
                case "p":
                    ParseParagraphTag(node);          
                    break;
                case "li":
                case "dt":
                case "dd":
                case "lh":
                    if (tagName == "li")
                        m_bIsWithinList = true;
                    bool pres1 = ParseStyle(node);
                    WriteParagraph(node);
                    LeaveStyle(pres1);
                    if (tagName == "li")
                        m_bIsWithinList = false;
                    m_currParagraph = null;
                    break;
                case "div":
                    OnDivBegin(node);
                    TraverseChildNodes(node.ChildNodes);
                    OnDivEnd();
                    break;
                case "h1":                 
                case "h2":                  
                case "h3":                   
                case "h4":                   
                case "h5":                   
                case "h6":
                case "h7":
                    if (!IsEmptyNode(node))
                    {
                        tf = EnsureStyle(node);
                        ParseHeadingTag(tf,node);
                    }
                    break;
                case "table":
                    OnTableBegin();
                    ParseTable(node);
                    OnTableEnd();
                    break;
                case "img":
                    tf = EnsureStyle(node);
                    WriteImage(node);
                    LeaveStyle(true);
                    if (m_currParagraph != null)
                        ApplyTextFormatting(m_currParagraph.BreakCharacterFormat);                      
                    break;
                case "a":
                    string bkName = string.Empty;
                    tf = EnsureStyle(node);
                    if (IsDefinedInline(node, "id"))
                        bkName = GetAttributeValue(node, "id");
                    else if (IsDefinedInline(node, "name"))
                        bkName = GetAttributeValue(node, "name");
                    if (GetAttributeValue(node, "href") == string.Empty && GetAttributeValue(node, "target") == string.Empty && bkName != string .Empty )
                    {
                        CurrentPara.AppendBookmarkStart(bkName);
                        TraverseChildNodes(node.ChildNodes);
                        CurrentPara.AppendBookmarkEnd(bkName);                       
                    }
                    else
                    WriteHyperlink(node);
                    LeaveStyle(true);
                    if (m_currParagraph != null)
                        ApplyTextFormatting(m_currParagraph.BreakCharacterFormat);                      
                    break;
                case "br":
                  bool isStylePresent= ParseStyle(node);
                  if (!IsDefinedInline(node, "page-break-before") && !IsDefinedInline(node, "page-break-after") && !IsDefinedInline (node,"page-break-inside"))
                  {
                      Break brk = CurrentPara.AppendBreak(BreakType.LineBreak);
                      TraverseChildNodes(node.ChildNodes);
                      string attrValue = GetAttributeValue(node, "clear");
                      if (attrValue == "all" || attrValue == "left" || attrValue == "right")
                          brk.HtmlToDocLayoutInfo .RemoveLineBreak  = false;
                  }
                  LeaveStyle(isStylePresent);
                  break;
                case "blockquote":                  
                    OnBlockquoteBegin(node );          
                    TraverseChildNodes(node.ChildNodes);                  
                    OnBlockquoteEnd();
                    break;
                case "title":
                    TraverseChildNodes(node.ChildNodes);
                    break;
                case "form":
                case "script":
                    break;
                default:
                    ParseFormattingTags(node);
                    break;
            }
        }
        private void ParseParagraphTag(XmlNode node)
        {
            if (!IsEmptyNode(node))
            {
                if (!((node.ParentNode.LocalName.ToLower() == "li" && IsFirstNode(node)) || (node.ParentNode.LocalName.ToLower() == "td" && IsFirstNode(node))))
                    AddNewParagraph(node);
                bool pres = ParseStyle(node);
                TraverseParagraphTag(node);
                if (m_currParagraph != null)
                    ApplyTextFormatting(m_currParagraph.BreakCharacterFormat);
                LeaveStyle(pres);
                m_currParagraph = null;
            }

        }
        /// <summary>
        /// Check whether the tag is empty tag
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsEmptyNode(XmlNode node)
        {
            bool IsEmptyNode=true ;

            if (node.LocalName.ToLower() == "img" || node.LocalName.ToLower() == "br" || node.LocalName.ToLower() == "a")
                return false;
            foreach (XmlNode childNode in node.ChildNodes)           
            {
                if (childNode.NodeType == XmlNodeType.Whitespace || childNode.NodeType == XmlNodeType.SignificantWhitespace)
                    continue;
                else
                {
                    IsEmptyNode = false;
                    break;
                }
            }
                return IsEmptyNode ;
        }
        /// <summary>
        /// Parse heading tag
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="fontSize"></param>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <param name="isBold"></param>
        /// <param name="isItalic"></param>

        private void ParseHeadingTag(TextFormat tf,XmlNode node)
        {
            switch (node.LocalName.ToLower())
            {
                case "h1":
                    tf.Style = BuiltinStyle.Heading1;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 24;
                    if (!tf.HasKey(TextFormat.FontFamilyKey))
                        tf.FontFamily = "Times New Roman";
                    break;
                case "h2":
                    tf.Style = BuiltinStyle.Heading2;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 18;
                    if (!tf.HasKey(TextFormat.FontFamilyKey))
                        tf.FontFamily = "Times New Roman";
                    break;
                case "h3":
                    tf.Style = BuiltinStyle.Heading3 ;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 13;
                    if (!tf.HasKey(TextFormat.FontFamilyKey))
                        tf.FontFamily = "Times New Roman";
                    break;
                case "h4":
                    tf.Style = BuiltinStyle.Heading4 ;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 12;
                    break;
                case "h5":
                    tf.Style = BuiltinStyle.Heading5 ;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 10;
                    break;
                case "h6":
                    tf.Style = BuiltinStyle.Heading6 ;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 7;
                    break;
                case "h7":
                    tf.Style = BuiltinStyle.Heading7 ;
                    if (!tf.HasKey(TextFormat.FontSizeKey))
                        tf.FontSize = 12;
                    break;
            }

            if (!tf.HasKey(TextFormat.BoldKey))
            {
                if (node.LocalName.ToLower() == "h7")
                    tf.Bold = false;
                else
                    tf.Bold = true;
            }
            if (!tf.HasKey(TextFormat.ItalicKey))
                tf.Italic = false;            
          
            WriteParagraph(node);
            LeaveStyle(true);
            m_currParagraph = null;
        }
        /// <summary>
        /// Specifies the process when a blockquote tag starts
        /// </summary>
        private void OnBlockquoteBegin(XmlNode node)
        {
            m_currParagraph = null;
            m_bIsInBlockquote = true;
            m_blockquoteLevel++;
            OnDivBegin(node);
        }
        /// <summary>
        /// Specifies the process when a blockquote tag ends
        /// </summary>
        private void OnBlockquoteEnd()
        {
            m_blockquoteLevel--;
            if (m_blockquoteLevel == 0)
                m_bIsInBlockquote = false;
            OnDivEnd();
            m_currParagraph = null;
        }
        /// <summary>
        /// Specifies the process when a div tag starts
        /// </summary>
        /// <param name="node">Div node</param>
        private void OnDivBegin(XmlNode node)
        {
            currDivFormat = null;
            m_divCount++;
            if (m_bIsInDiv)
            {
                m_currParagraph = null;
            }

            m_bIsInDiv = true;

            HorizontalAlignment align = GetHorizontalAlignment ((GetAttributeValue(node, "align")));
            bool pres1 = false;
            pres1 = ParseStyle(node);
            if (pres1)
                currDivFormat = m_styleStack.Peek();
            else if (m_bIsInDiv && m_styleStack.Count > 0)
            {
                TextFormat format = m_styleStack.Peek().Clone();
                m_styleStack.Push(format);
                currDivFormat = m_styleStack.Peek();
            }

            if (currDivFormat != null && !currDivFormat.HasKey(TextFormat.TextAlignKey))
                currDivFormat.TextAlign = align;
            else if (currDivFormat == null && align != HorizontalAlignment.Left)
            {
                currDivFormat = new TextFormat();
                if (!currDivFormat.HasKey(TextFormat.TextAlignKey))
                    currDivFormat.TextAlign = align;
            }

        }
        /// <summary>
        /// Specifies the process when a div tag ends
        /// </summary>
        private void OnDivEnd()
        {
            m_divCount--;
            m_bIsInDiv = m_divCount == 0 ? false : true;
            if (m_styleStack.Count > 0)
                m_styleStack.Pop();
            if (m_styleStack.Count > 0 && m_bIsInDiv)
                currDivFormat = m_styleStack.Peek();
            else
                currDivFormat = null;
            m_currParagraph = null;
        }
        /// <summary>
        /// Specifies the process when a table tag ends
        /// </summary>
        private void OnTableEnd()
        {
            m_currTable = m_nestedTable.Pop();
            m_bodyItems = m_nestedBodyItems.Pop();
            m_bIsCellStyle = m_stackCellStyle.Pop();
            m_currParagraph = null;
        }
        /// <summary>
        /// Specifies the process when a table tag starts
        /// </summary>
        private void OnTableBegin()
        {
            m_nestedBodyItems.Push(m_bodyItems);
            m_nestedTable.Push(m_currTable);
            m_stackCellStyle.Push(m_bIsCellStyle);
        }
        /// <summary>
        /// COnverting Hyperlink in html to Doc
        /// </summary>
        /// <param name="node">Hyperlink node</param>
        private void WriteHyperlink(XmlNode node)
        {
            bool IsImageLink = false;
            HyperlinkType type = HyperlinkType.None;
            string src = null;
            string link = GetAttributeValue(node, "href");
            //Traverse child nodes of the HyperLink
            IWField field = TraverseHyperlinkField(node);
            if (link.StartsWith("#"))
            {
                type = HyperlinkType.Bookmark;
                link = link.Replace("#", string.Empty);
            }
            else if (link.StartsWith("mailto:"))
            {
                type = HyperlinkType.EMailLink;
            }
            else if (link.StartsWith("http") || link.StartsWith("www"))
            {
                type = HyperlinkType.WebLink;
            }
            else
            {
                type = HyperlinkType.FileLink;
            }
            //Set Hyperlink field path.
            Hyperlink hl = new Hyperlink(field as WField);
            hl.Type = type;
            if (type == HyperlinkType.WebLink || type == HyperlinkType.EMailLink)
                hl.Uri = link;
            else if (hl.Type == HyperlinkType.Bookmark)
                hl.BookmarkName = link;
            else if (hl.Type == HyperlinkType.FileLink)
                hl.FilePath = link;
            ApplyHyperlinkStyle(field as WField);
        }
        /// <summary>
        /// Traverse child nodes of the Hyperlink Field
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IWField TraverseHyperlinkField(XmlNode node)
        {
            WField fieldStart = new WField(CurrentPara.Document);
            fieldStart.FieldType = FieldType.FieldHyperlink;
            CurrentPara.Items.Add(fieldStart);
            CurrentPara.AppendFieldMark(FieldMarkType.FieldSeparator);
            fieldStart.FieldSeparator = CurrentPara.LastItem as WFieldMark;
            //Traverse child nodes
            TraverseChildNodes(node.ChildNodes);
            WFieldMark end = new WFieldMark(CurrentPara.Document, FieldMarkType.FieldEnd);
            CurrentPara.Items.Add(end);
            fieldStart.FieldEnd = end;
            return fieldStart;
        }
        /// <summary>
        /// Apply Hyperlink style
        /// </summary>
        /// <param name="field"></param>
        private void ApplyHyperlinkStyle(WField field)
        {
            WParagraph ownerPara = field.OwnerParagraph;
            int fieldIndex = ownerPara.Items.IndexOf(field);
            bool isTextRangeStart = false;

            for (int i = fieldIndex; i < ownerPara.Items.Count; i++)
            {
                ParagraphItem item = ownerPara.Items[i];
                if (item.EntityType == EntityType.FieldMark && (item as WFieldMark).Type == FieldMarkType.FieldSeparator)
                    isTextRangeStart = true;
                else if (item.EntityType == EntityType.TextRange && isTextRangeStart)
                {
                    if (m_hyperlinkcolor != Color.Empty)
                        (item as WTextRange).CharacterFormat.TextColor = m_hyperlinkcolor;
                    if (m_bodyItems.Document.Styles.FindByName(BuiltinStyle.Hyperlink.ToString()) == null)
                    {
                        IStyle style = Style.CreateBuiltinStyle(BuiltinStyle.Hyperlink, StyleType.CharacterStyle, m_bodyItems.Document);
                        m_bodyItems.Document.Styles.Add(style);
                    }
                    (item as WTextRange).CharacterFormat.CharStyleName = (BuiltinStyle.Hyperlink.ToString());

                    ApplyTextFormatting((item as WTextRange).CharacterFormat);
                }
                else if (item.EntityType == EntityType.FieldMark && (item as WFieldMark).Type == FieldMarkType.FieldEnd)
                {
                    isTextRangeStart = false;
                    break;
                }
            }
        }

        ///<summary>
        ///Parses image attribute
        ///</summary>
        private void ParseImageAttribute(XmlNode node, IWPicture pic)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                switch (attr.Name.ToLower())
                {
                    case "height":
                        pic.Height = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        break;
                    case "width":
                        pic.Width = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        break;
                    case "style":
                        ParseImageStyleAttribute(attr, pic);
                        break;
                    case "align":
                       
                        switch (attr.Value)
                        {
                            case "top":
                                pic.VerticalAlignment = ShapeVerticalAlignment.Top;
                                break;
                            case "bottom":
                                pic.VerticalAlignment = ShapeVerticalAlignment.Bottom;
                                break;
                            case "middle":
                                pic.HorizontalAlignment = ShapeHorizontalAlignment.Center;
                                break;
                            case "left":
                                pic.HorizontalAlignment = ShapeHorizontalAlignment.Left;
                                break;
                            case "right":
                                pic.HorizontalAlignment = ShapeHorizontalAlignment.Right;
                                pic.TextWrappingStyle = TextWrappingStyle.Square;
                                break;
                        }
                        break;
                    case "alt":
                        if (attr.Value != null && attr.Value != string.Empty)
                            pic.AlternativeText = attr.Value.Trim();
                        break;
                }
            }
        }
        /// <summary>
        /// Parses the Image style.
        /// </summary>
        /// <param name="attr">The attr.</param>
        /// <param name="pic">The Picture.</param>
        private void ParseImageStyleAttribute(XmlAttribute attr, IWPicture pic)
        {
            if (attr.Name.ToLower() != "style")
                return;

            string[] styleParams = attr.Value.Split(';', ':');

            for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
            {
                string paramName = styleParams[i].ToLower().Trim();
                string paramValue = styleParams[i + 1].ToLower().Trim();
                try
                {
                    switch (paramName)
                    {
                        case "height":
                            pic.Height = Convert.ToSingle(ExtractValue(paramValue));
                            break;
                        case "width":
                            pic.Width = Convert.ToSingle(ExtractValue(paramValue));
                            break;
                    }
                }
                //To avoid the FormatException, we need to continue the process while the style Attribute is have Unknown attribute value
                catch
                {
                    continue;
                }

            }
        }

        /// <summary>
        /// Writes the image.
        /// </summary>
        /// <param name="node">The node.</param>
        private void WriteImage(XmlNode node)
        {
            //Add new paragraph if the current paragraph is null and the node is within division
            if (m_currParagraph == null && m_bIsInDiv)
            {
                if ((node.ParentNode.LocalName.ToLower() == "div" && IsFirstNode(node)) 
                    || (node.ParentNode.LocalName.ToLower() == "span"))
                {
                    AddNewParagraph(node);
                }
            }
           
            string src = GetAttributeValue(node, "src");

            IWPicture pic = new WPicture(m_bodyItems.Document);
#if WINRT
            GetImage(src, pic);
            if (pic.ImageBytes != null)
            {
                CurrentPara.Items.Add(pic);
                ParseImageAttribute(node, pic);
            }
#else
            Image img = GetImage(src);
            pic = CurrentPara.AppendPicture(img);
            //Set Image size
            pic.Width = (float)(pic.Image.Width * 0.75);
            pic.Height = (float)(pic.Image.Height * 0.75);

            ParseImageAttribute(node, pic);
#endif
            //Set Hidden property
            ApplyTextFormatting((pic as ParagraphItem).ParaItemCharFormat);
        }
        
#if !WINRT
            /// <summary>
        /// Get Image
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Image GetImage(string src)
        {
            Image img = null;
            try
            {
                if (src.StartsWith("data:image/"))
                {
                    int startIndex = src.IndexOf(",");
                    src = src.Substring(startIndex + 1);
                    img = Image.FromStream(new MemoryStream(System.Convert.FromBase64String(src)));
                }
                else if (src.StartsWith("http") || src.StartsWith("ftp"))
                {
                    WebRequest request = WebRequest.Create(src);
                    request.Method = "GET";

                    Stream st = request.GetResponse().GetResponseStream();
                    img = Image.FromStream(st);
                }
                else
                {
                    if (File.Exists(src))
                    {
                        img = Image.FromFile(src);
                    }
                    else if (File.Exists(BasePath + @"\" + src))
                    {
                        img = Image.FromFile(BasePath + @"\" + src);
                    }
                }
            }
            catch
            {
                if (src.StartsWith("http") || src.StartsWith("ftp"))
                {
                    ImageDownloadingFailedEventArgs args = null;
                    if (HtmlImportSettings != null)
                    {
                        //Execute Image downloading failed event to get the credentials
                        args = HtmlImportSettings.ExecuteImageDownloadingFailedEvent(src);
                        if (!string.IsNullOrEmpty(args.UserName))
                            return TryDownloadingFailedImage(src, args);
                    }
                }
                Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_IMAGENOTFOUND);
                img = Image.FromStream(imgStream);
            }
            if (img == null)
            {
                Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_IMAGENOTFOUND);
                img = Image.FromStream(imgStream);
            }
            return img;
        }
        /// <summary>
        /// Try downloading the image again with the given credentials
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Image TryDownloadingFailedImage(string src, ImageDownloadingFailedEventArgs args)
        {
            Image img = null;
            try
            {
                WebRequest request = WebRequest.Create(src);
                request.Method = "GET";

                request.Credentials = new NetworkCredential(args.UserName, args.Password);

                Stream st = request.GetResponse().GetResponseStream();
                img = Image.FromStream(st);
            }
            catch
            {
                Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_IMAGENOTFOUND);
                img = Image.FromStream(imgStream);
            }
            return img;
        }
#else
        /// <summary>
        /// Get Image
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private async void GetImage(string src, IWPicture pic)
        {
            byte[] img = null;
            try
            {
                if (src.StartsWith("data:image/"))
                {
                    int startIndex = src.IndexOf(",");
                    src = src.Substring(startIndex + 1);
                    img = System.Convert.FromBase64String(src);
                    pic.LoadImage(img);
                }
                else if (src.StartsWith("http") || src.StartsWith("ftp"))
                {
                    WebRequest request = WebRequest.Create(src);
                    request.Method = "GET";
                    Task<WebResponse> task = request.GetResponseAsync();
                    task.Wait();
                    WebResponse response = task.Result;
                    Stream imageStream = response.GetResponseStream();
                    using (var result = new MemoryStream())
                    {
                        imageStream.CopyTo(result);
                        pic.LoadImage(result);
                    }
                    imageStream.Dispose();
                    response.Dispose();
                }
                else
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(src);
                    if (file == null)
                        file = await StorageFile.GetFileFromPathAsync(BasePath + @"\" + src);
                    if (file != null)
                    {
                        Stream fs = await file.OpenStreamForReadAsync();
                        pic.LoadImage(fs);
                        fs.Dispose();
                    }
                }
            }
            catch
            {
                Stream imgStream = typeof(HTMLConverterImpl).GetTypeInfo().Assembly.GetManifestResourceStream(DEF_IMAGENOTFOUND);
                pic.LoadImage(imgStream);
                imgStream.Dispose();
            }
            if (pic.ImageBytes == null)
            {
                Stream imgStream = typeof(HTMLConverterImpl).GetTypeInfo().Assembly.GetManifestResourceStream(DEF_IMAGENOTFOUND);
                pic.LoadImage(imgStream);
                imgStream.Dispose();
            }
        }
#endif
        /// <summary>
        /// Appends the HTML text.
        /// </summary>
        /// <param name="para">The para.</param>
        /// <param name="textNode">The text node.</param>
        private void ParseFormattingTags(XmlNode tag)
        {
            if (m_curListLevel < 0)
                m_listLeftIndentStack.Clear();
            TextFormat format = EnsureStyle(tag);

            switch (tag.Name.ToLower())
            {
                case "b":
                case "strong":
                    format.Bold = true;
                    break;
                case "i":
                case "em":
                case "cite":
                case "dfn":
                case "var":
                    format.Italic = true;
                    break;
                case "u":
                    format.Underline = true;
                    break;
                case "s":
                case "strike":
                    format.Strike = true;
                    break;
                case "small":
                    if (format.FontSize < 0)
                        format.FontSize = 10;

                    format.FontSize -= 2;
                    break;
                case "big":
                    format.FontSize += 2;
                    break;
                case "code":
                case "tt":
                case "pre":
                case "samp":
                    format.FontFamily = "Courier New";
                    format.FontSize = 10;
                    break;
                case "font":
                    string color = GetAttributeValue(tag, "color");
                    string face = GetAttributeValue(tag, "face");
                    if (color.Length > 0)
                        format.FontColor = GetColor (color);
                    if (face.Length > 0)
                        format.FontFamily = face;
                    string value = GetAttributeValue(tag, "size");
                    if (value.Length > 0)                  
                        ApplyFontSize(value, format);                   
                    break;
                case "ul":
                    m_curListLevel++;
                    SetListMode( true, tag, format );
                    if (!IsDefinedInline(tag, "margin-left") && !IsDefinedInline (tag,"margin") )
                        UpdateListLeftIndentStack(0, false);
                    if (m_curListLevel == 0)
                        CreateListStyle(tag);
                    break;
                case "ol":
                    m_curListLevel++;
                    SetListMode( false, tag, format );                   
                    if (!IsDefinedInline(tag, "margin-left") && !IsDefinedInline(tag, "margin"))
                        UpdateListLeftIndentStack(0, false);
                    if (m_curListLevel == 0)
                        CreateListStyle(tag);
                    break;
                case "a":
                    format.FontColor = Color.Blue;
                    format.Underline = true;
                    break;
                case "sup":
                    format.SubSuperScript = SubSuperScript.SuperScript;
                    break;
                case "sub":
                    format.SubSuperScript = SubSuperScript.SubScript;
                    break;
                default:
                    //throw new NotSupportedException( "DocIO do not support html tag: " + tag.Name );
                    break;
            }

            TraverseChildNodes(tag.ChildNodes);
            //Apply Div format to the paragraph
            if (currDivFormat != null && tag.LocalName.ToLower() == "label" && m_currParagraph != null
               && (!(m_bIsInDiv && m_currParagraph.IsInCell) || NodeIsInDiv(tag)))
            {
                ApplyDivParagraphFormat(tag);
            }
            if (tag.LocalName.ToLower() == "ol" || tag.LocalName.ToLower() == "ul")
            {
                m_curListLevel--;
                if (m_listLeftIndentStack.Count > 0)
                {
                    m_listLeftIndentStack.Pop();

                }
                if (m_curListLevel < 0 && ListStack .Count >0)
                    ListStack.Pop();
                if (LfoStack.Count > 0)
                    LfoStack.Pop();
                if (m_curListLevel < 0)
                    m_listLevelNo.Clear();
               
            }
            LeaveStyle(true);
        }
        /// <summary>
        /// Update List left indent stack
        /// </summary>
        /// <param name="leftIndent"></param>
        /// <param name="isInlineLeftIndent"></param>
        private void UpdateListLeftIndentStack(float leftIndent,bool isInlineLeftIndent)
        {           
            if (m_listLeftIndentStack.Count > 0)
            {
                if (isInlineLeftIndent)
                    m_listLeftIndentStack.Push(m_listLeftIndentStack.Peek() + leftIndent);                
                else
                    m_listLeftIndentStack.Push(m_listLeftIndentStack.Peek() + DEF_INDENT);
            }
            else
            {
                if(isInlineLeftIndent)
                    m_listLeftIndentStack.Push(leftIndent);
                else
                    m_listLeftIndentStack.Push(DEF_INDENT);
            }
        }
        /// <summary>
        /// Apply Font size specified in font tag
        /// </summary>
        /// <param name="fontSize"></param>
        /// <param name="format"></param>
        private void ApplyFontSize(string value, TextFormat format)
        {
            bool isIncrement = false;
            bool isDecrement = false;
            char ch;
            bool isSuffixed = false;
            if (value.StartsWith("+"))
            {
                isIncrement = true;
                value = value.Substring(1, value.Length - 1);
            }
            else if (value.StartsWith("-"))
            {
                isDecrement = true;
                value = value.Substring(1, value.Length - 1);
            }
           
            for (int i = 0; i < value.Length; i++)
            {
                ch = value[i];
                if (Char.IsDigit(ch))
                    isSuffixed = false;
                else
                {
                    isSuffixed = true;
                    break;
                }
            }

            if (isSuffixed)
                format.FontSize = 12f;
            else
            {
                int sizeCount = Convert.ToInt32(value);
                if (isIncrement)
                    sizeCount = 3 + sizeCount;
                else if (isDecrement)
                    sizeCount = 3 - sizeCount;
                if (sizeCount <= 1)
                    format.FontSize = 7.5f;
                else if (sizeCount >= 7)
                    format.FontSize = 36f;
                else
                {
                    switch (sizeCount)
                    {
                        case 2:
                            format.FontSize = 10f;
                            break;
                        case 3:
                            format.FontSize = 12f;
                            break;
                        case 4:
                            format.FontSize = 13.5f;
                            break;
                        case 5:
                            format.FontSize = 18f;
                            break;
                        case 6:
                            format.FontSize = 24f;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the list mode.
        /// </summary>
        /// <param name="isBulleted">if set to <c>true</c> is bulleted.</param>
        /// <param name="node">The node.</param>
        private void SetListMode( bool isBulleted, XmlNode node, TextFormat format )
        {
            if( isBulleted )
                format.DefBulleted = true;
            else
                format.NumBulleted = true;
        }
        /// <summary>
        /// Writes the paragraph.
        /// </summary>
        private void WriteParagraph(XmlNode node)
        {
            if (node.ParentNode .LocalName !="li" && !(node .ParentNode .LocalName .ToLower ()=="td" && IsFirstNode(node)))
                AddNewParagraph(node); 
            if (m_currParagraph != null)
               ApplyTextFormatting(m_currParagraph.BreakCharacterFormat);
            ApplyParagraphFormat(node);
            TraverseChildNodes(node.ChildNodes);
            ApplyParagraphStyle();

        }
        /// <summary>
        /// Applies the paragraph style.
        /// </summary>
        private void ApplyParagraphStyle()
        {
            TextFormat format = CurrentFormat;
            if (m_currParagraph == null)
                return;

            if (format.Style != BuiltinStyle.Normal)
            {
                m_currParagraph.ApplyStyle(format.Style);
                if (m_currParagraph.ParaStyle.ParagraphFormat.KeepFollow)
                {
                    m_currParagraph.ParaStyle.ParagraphFormat.KeepFollow = false;
                }
            }
            else if (m_userStyle != null)
            {
                m_currParagraph.ApplyStyle(m_userStyle);
            }
            else
            {
                m_currParagraph.ApplyStyle(BuiltinStyle.NormalWeb);
            }
        }
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        private void ApplyParagraphFormat(XmlNode node)
        {
            if (m_currParagraph != null)
            {
                if (!(m_bIsInDiv && m_currParagraph.IsInCell))
                    ApplyDivParagraphFormat(node);
                WParagraphFormat pformat = m_currParagraph.ParagraphFormat;
                TextFormat format = CurrentFormat;
                ApplyParagraphStyle();
                if (node.Name.ToLower() != "th" && node.Name.ToLower() != "td")
                    ApplyParagraphBorder(pformat, format);
                
               
                if (node.LocalName.ToLower() == "dd")
                {
                    m_currParagraph.ParagraphFormat.LeftIndent = 36f;
                    m_currParagraph.ParagraphFormat.LeftIndentBi = 36f;
                }

                if (format.HasValue(TextFormat.TextAlignKey))
                    pformat.HorizontalAlignment = format.TextAlign;

                if (m_currParagraph.IsInCell)
                {
                    if (format.HasValue(TextFormat.TextAlignKey))
                        m_currParagraph.ParagraphFormat.HorizontalAlignment = format.TextAlign;
                }

                ApplyListFormatting(pformat, format, node);

                if (format.HasValue(TextFormat.LineHeightKey))
                {
                    pformat.LineSpacingRule = format.LineSpacingRule;
                    pformat.LineSpacing = format.LineHeight;
                }

                if (format.IsLineHeightNormal)
                {
                    pformat.SpaceAfterAuto = true;
                    pformat.SpaceBeforeAuto = true;
                }
                if (m_bIsWithinList)
                {
                    //Need to adjust the left indent for all the paragraph within <li> tag
                    if (!(m_currParagraph.IsInCell && m_currParagraph.ListFormat.CurrentListStyle == null))
                        pformat.LeftIndent = AdjustLeftIndentForList(node, format);
                }
                else if (format.HasValue(TextFormat.LeftMarginKey) && format.LeftMargin > 0)
                {
                    pformat.LeftIndent = format.LeftMargin;
                }
                if (format.HasValue(TextFormat.TextIndentKey))
                {
                    pformat.FirstLineIndent = format.TextIndent;
                }
                if (format.HasValue(TextFormat.RightMarginKey))
                {
                    pformat.RightIndent = format.RightMargin;
                }
                if (IsBottomMarginNeedToBePreserved(node, format))
                {
                    pformat.AfterSpacing = format.BottomMargin;
                    pformat.SpaceAfterAuto = false;
                }
                else if(node .LocalName .ToLower ()!= "td")
                {
                    pformat.SpaceAfterAuto = true;
                }
               
                if (IsTopMarginNeedToBePreserved (node ,format))
                {
                    pformat.BeforeSpacing = format.TopMargin;
                    pformat.SpaceBeforeAuto = false;
                }
                else if (node.LocalName.ToLower() != "td")
                {                 
                    pformat.SpaceBeforeAuto = true;
                }
              


                if (format.HasValue(TextFormat.PageBreakBeforeKey))
                {
                    pformat.PageBreakBefore = format.PageBreakBefore;
                    //TODO: The following fix has been handled as a work around for the preservation page-break-before in heading tag along with horizontal alignment
                    pformat.ParaProps.ParagraphPropertyException.PageBreakBefore = true;
                }
                if (format.HasValue(TextFormat.PageBreakAfterKey))
                {
                    pformat.PageBreakAfter = format.PageBreakAfter;
                }
                if (format.HasValue(TextFormat.WordWrapKey))
                    pformat.WordWrap = format.WordWrap;
                if (format.BackColor != null)
                    pformat.BackColor = format.BackColor;
               
                UpdateParaFormat(node,pformat );

                if (m_bIsInBlockquote && node .LocalName .ToLower ()!="li" )
                {
                    pformat.LeftIndent = CurrentPara.ParagraphFormat.LeftIndent + (m_blockquoteLevel * DEF_INDENT);
                }
            }
        }
       /// <summary>
        /// Adjust left indent value for list
       /// </summary>
       /// <param name="node"></param>
       /// <param name="format"></param>
       /// <returns></returns>
        private float  AdjustLeftIndentForList(XmlNode node, TextFormat format)
        {
            float leftIndent;
            if (!IsDefinedInline(node, "margin-left"))
                format.LeftMargin = 0;
            XmlNode parentNode = node.ParentNode;
            if (m_listLeftIndentStack.Count == 0)
                leftIndent = format.LeftMargin + (DEF_INDENT * (m_curListLevel + 1));
            else
                leftIndent  = format.LeftMargin + m_listLeftIndentStack.Peek();
            return leftIndent;
        }
        /// <summary>
        /// Check whether the bottom margin need to be preserved
        /// </summary>
        /// <param name="node"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool IsBottomMarginNeedToBePreserved(XmlNode node, TextFormat format)
        {
            if (format.HasKey(TextFormat.BottomMarginKey))
            {
                if (IsDefinedInline(node, "margin-bottom") || IsDefinedInline(node, "margin")
                    || IsDefinedInline(node, "padding") || IsDefinedInline(node, "padding-bottom"))
                    return true ;
                else if(node .ParentNode .LocalName .ToLower () == "div" && IsLastNode(node))
                {
                    XmlNode parentNode = node.ParentNode;
                    while (parentNode !=null && parentNode.LocalName.ToLower() == "div")
                    {
                        if (IsDefinedInline(parentNode, "margin-bottom") || IsDefinedInline(parentNode, "margin") 
                            || IsDefinedInline(parentNode, "padding") || IsDefinedInline(parentNode, "padding-bottom"))
                            return true;
                        else if (parentNode.ParentNode .LocalName .ToLower ()== "div" && IsLastNode(parentNode))
                            parentNode = parentNode.ParentNode;
                        else
                            return false;
                    }
                }
                else
                    return false ;
            }

            return false;
        }
        /// <summary>
        /// Check whether the Top margin need to be preserved
        /// </summary>
        /// <param name="node"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool IsTopMarginNeedToBePreserved(XmlNode node, TextFormat format)
        {
            if (format.HasKey(TextFormat.TopMarginKey))
            {
                if (IsDefinedInline(node, "margin-top") || IsDefinedInline(node, "margin") 
                    || IsDefinedInline(node, "padding")|| IsDefinedInline (node ,"padding-top"))
                    return true;
                else if (node.ParentNode.LocalName.ToLower() == "div" && IsFirstNode(node))
                {
                    XmlNode parentNode = node.ParentNode;
                    while (parentNode != null && parentNode.LocalName.ToLower() == "div")
                    {
                        if (IsDefinedInline(parentNode, "margin-top") || IsDefinedInline(parentNode, "margin")
                            || IsDefinedInline(parentNode, "padding") || IsDefinedInline(parentNode, "padding-top"))
                            return true;
                        else if (parentNode .ParentNode .LocalName .ToLower ()=="div" && IsFirstNode(parentNode))
                            parentNode = parentNode.ParentNode;
                        else
                            return false;

                    }
                }
                else
                    return false;
            }

            return false;
        }
        /// <summary>
        /// Checks whether the node is last node within division
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsLastNode(XmlNode node)
        {
            while (true)
            {
                if (node.NextSibling == null)
                    return true;
                else if (node.NextSibling.Name == @"#whitespace" 
                    || node.NextSibling.Name == "#significant-whitespace")
                    node = node.NextSibling;
                else
                {
                    if (node.NextSibling.NodeType != XmlNodeType.Text && IsEmptyNode(node.NextSibling))
                        node = node.NextSibling;
                    else
                        return false;
                }
            }
        }
        /// <summary>
        /// Check whether the specified attribute defined inline
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attName"></param>
        /// <returns></returns>
        private bool IsDefinedInline(XmlNode node, string attName)
        {
            string style = string .Empty ;

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.LocalName.ToLower() == "style")
                    {
                        style = GetAttributeValue(node, "style");
                        string[] styleParams = style.Split(';', ':');
                        for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
                        {
                            string paramName = styleParams[i].ToLower().Trim();
                            if (paramName == attName)
                                return true;                      
                        }
                       
                    }
                    else if (attr.LocalName.ToLower() == attName)
                        return true;
                }
            }
            return false;           
        }
        /// <summary>
        /// Apply list formatting
        /// </summary>
        /// <param name="pformat"></param>
        /// <param name="format"></param>
        /// <param name="node"></param>
        private void ApplyListFormatting(WParagraphFormat pformat, TextFormat format, XmlNode node)
        {
            if (format.NumBulleted && node.Name.ToUpper() != "LH" && node.Name.ToLower() == "li" && node.ParentNode.Name.ToLower() == "ol")
            {
                bool styleApplied = false;
                foreach (XmlAttribute attr in node.ParentNode.Attributes)
                {
                    if (attr.Name.ToLower() == "type")
                    {
                        BuildListStyle(GetListPatternType(attr.Value), node);
                        styleApplied = true;
                    }
                    else if (attr.Name.ToLower() == "style" && IsDefinedInline(node.ParentNode, "list-style-type"))
                    {
                        string paramValue = GetStyleAttributeValue(attr.Value.ToLower(), "list-style-type");
                        BuildListStyle(GetListPatternType(paramValue), node);
                        styleApplied = true;
                    }
                }

                if (!styleApplied)
                {
                    // default list style
                    BuildListStyle(ListPatternType.Arabic, node);
                }
            }
            else if (node.ParentNode.Name.ToLower() == "ul" && (format.DefBulleted) && ((node.LocalName.ToLower() == "li") && (node.LocalName.ToLower() != "lh")))
            {
                BuildListStyle(ListPatternType.Bullet, node);
                foreach (XmlAttribute attr in node.ParentNode.Attributes)
                {
                    if (attr.Name.ToLower() == "style" && IsDefinedInline(node.ParentNode, "list-style-image"))
                    {
                        string paramValue = GetStyleAttributeValue(attr.Value.ToLower(), "list-style-image");
                        string imgSrc = paramValue.Replace("url('", string.Empty).Replace("')", string.Empty);
                        WPicture pic = new WPicture(m_bodyItems.Document);
#if !SILVERLIGHT && !WP
                        Image img = GetImage(imgSrc);
                        pic.LoadImage(img);
#endif
#if WINRT
                        GetImage(imgSrc, pic);
#endif
                        m_currParagraph.ListFormat.CurrentListLevel.PicBullet = pic;

                    }

                }
            }
            else if (node.Name.ToUpper() == "LH")
            {
                pformat.LeftIndent = DEF_LH_INDENT;
            }
        }
        /// <summary>
        /// Get List Pattrn type
        /// </summary>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        private ListPatternType GetListPatternType(string attrValue)
        {
            switch (attrValue)
            {
                case "lower-alpha":
                case "a":
                    return ListPatternType.LowLetter;
                    break;
                case "upper-alpha":
                case "A":
                    return ListPatternType.UpLetter;
                    break;
                case "lower-roman":
                case "i":
                    return ListPatternType.LowRoman;
                    break;
                case "upper-roman":
                case "I":
                    return ListPatternType.UpRoman;
                    break;
                case "decimal-leading-zero":
                case "decimal":
                    return ListPatternType.Arabic;
                    break;
                case "none":
                    return ListPatternType.None;
                    break;
                default:
                    return ListPatternType.Arabic;
                    break;

            }

        }
        /// <summary>
        /// Apply Paragraph border
        /// </summary>
        /// <param name="pformat"></param>
        /// <param name="format"></param>
        private void ApplyParagraphBorder(WParagraphFormat pformat,TextFormat format)
        {
            if (format.Borders.AllStyle != BorderStyle.None)
            {
                pformat.Borders.Bottom.BorderType = pformat.Borders.Top.BorderType =
                pformat.Borders.Left.BorderType = pformat.Borders.Right.BorderType = format.Borders.AllStyle;

                if (format.Borders.AllColor != Color.Empty)
                {
                    pformat.Borders.Bottom.Color = pformat.Borders.Top.Color =
                    pformat.Borders.Left.Color = pformat.Borders.Right.Color = format.Borders.AllColor;
                }
                if (format.Borders.AllWidth != -1.0f)
                {
                    pformat.Borders.Bottom.LineWidth = pformat.Borders.Top.LineWidth =
                    pformat.Borders.Left.LineWidth = pformat.Borders.Right.LineWidth = format.Borders.AllWidth;
                }
            }
            if (format.Borders.BottomStyle != BorderStyle.None)
            {
                pformat.Borders.Bottom.BorderType = format.Borders.BottomStyle;
                pformat.Borders.Bottom.LineWidth = format.Borders.BottomWidth;
                pformat.Borders.Bottom.Color = format.Borders.BottomColor;
            }
            if (format.Borders.TopStyle != BorderStyle.None)
            {
                pformat.Borders.Top.BorderType = format.Borders.TopStyle;
                pformat.Borders.Top.LineWidth = format.Borders.TopWidth;
                pformat.Borders.Top.Color = format.Borders.TopColor;
            }
            if (format.Borders.LeftStyle != BorderStyle.None)
            {
                pformat.Borders.Left.BorderType = format.Borders.LeftStyle;
                pformat.Borders.Left.LineWidth = format.Borders.LeftWidth;
                pformat.Borders.Left.Color = format.Borders.LeftColor;
            }
            if (format.Borders.RightStyle != BorderStyle.None)
            {
                pformat.Borders.Right.BorderType = format.Borders.RightStyle;
                pformat.Borders.Right.LineWidth = format.Borders.RightWidth;
                pformat.Borders.Right.Color = format.Borders.RightColor;
            }
            if (format.Borders.TopWidth > 0)
                pformat.Borders.Top.LineWidth = format.Borders.TopWidth;
            if (format.Borders.RightWidth > 0)
                pformat.Borders.Right.LineWidth = format.Borders.RightWidth;
            if (format.Borders.LeftWidth > 0)
                pformat.Borders.Left.LineWidth = format.Borders.LeftWidth;
            if (format.Borders.BottomWidth > 0)
                pformat.Borders.Bottom.LineWidth = format.Borders.BottomWidth;

        }
        /// <summary>
        /// Apply the current div formt to the paragraph.
        /// </summary>
        private void ApplyDivParagraphFormat(XmlNode node)
        {
            if (currDivFormat != null)
            {
                WParagraphFormat pFormat = m_currParagraph.ParagraphFormat;
                if (currDivFormat.HasValue(TextFormat.BackColorKey))
                    pFormat.BackColor = currDivFormat.BackColor;
                if (currDivFormat.HasValue(TextFormat.LeftMarginKey))
                    pFormat.LeftIndent = currDivFormat.LeftMargin;
                if (currDivFormat.HasValue(TextFormat.RightMarginKey))
                    pFormat.RightIndent = currDivFormat.RightMargin;
                if (currDivFormat.HasValue(TextFormat.TextAlignKey))
                    pFormat.HorizontalAlignment = currDivFormat.TextAlign;


                if (IsBottomMarginNeedToBePreserved (node,currDivFormat))
                    pFormat.AfterSpacing = currDivFormat.BottomMargin;
               

                if (IsTopMarginNeedToBePreserved (node ,currDivFormat))
                    pFormat.BeforeSpacing = currDivFormat.TopMargin;

                if (currDivFormat.HasValue(TextFormat.LineHeightKey))
                {
                    pFormat.LineSpacingRule = currDivFormat.LineSpacingRule;
                    pFormat.LineSpacing = currDivFormat.LineHeight;
                }
                if (currDivFormat.IsLineHeightNormal)
                {
                    pFormat.SpaceAfterAuto = true;
                    pFormat.SpaceBeforeAuto = true;
                }
                if (currDivFormat.Borders.AllStyle != BorderStyle.None)
                {
                    pFormat.Borders.Bottom.BorderType = pFormat.Borders.Top.BorderType =
                    pFormat.Borders.Left.BorderType = pFormat.Borders.Right.BorderType = currDivFormat.Borders.AllStyle;

                    if (currDivFormat.Borders.AllColor != Color.Empty)
                    {
                        pFormat.Borders.Bottom.Color = pFormat.Borders.Top.Color =
                        pFormat.Borders.Left.Color = pFormat.Borders.Right.Color = currDivFormat.Borders.AllColor;
                    }
                    if (currDivFormat.Borders.AllWidth != -1.0f)
                    {
                        pFormat.Borders.Bottom.LineWidth = pFormat.Borders.Top.LineWidth =
                        pFormat.Borders.Left.LineWidth = pFormat.Borders.Right.LineWidth = currDivFormat.Borders.AllWidth;
                    }
                }


                if (currDivFormat.Borders.BottomStyle != BorderStyle.None)
                {
                    pFormat.Borders.Bottom.BorderType = currDivFormat.Borders.BottomStyle;
                    pFormat.Borders.Bottom.LineWidth = currDivFormat.Borders.BottomWidth;
                    pFormat.Borders.Bottom.Color = currDivFormat.Borders.BottomColor;
                }
                if (currDivFormat.Borders.TopStyle != BorderStyle.None)
                {
                    pFormat.Borders.Top.BorderType = currDivFormat.Borders.TopStyle;
                    pFormat.Borders.Top.LineWidth = currDivFormat.Borders.TopWidth;
                    pFormat.Borders.Top.Color = currDivFormat.Borders.TopColor;
                }
            }
        }
        /// <summary>
        /// Applies the formatting.
        /// </summary>
        /// <param name="tr">The tr.</param>
        private void ApplyTextFormatting(WCharacterFormat charFormat)
        {
            if (!(m_bIsInDiv && m_currParagraph.IsInCell))
                ApplyDivCharacterFormat(charFormat);

            TextFormat format = CurrentFormat;
            if(!(m_userStyle !=null &&  m_userStyle .CharacterFormat .HasKey (WCharacterFormat .BoldKey )))
            if (format.HasValue(TextFormat.BoldKey))
                charFormat.Bold = format.Bold;

            if (format.HasValue(TextFormat.ItalicKey))
                charFormat.Italic = (format.Italic == true);

            if (format.HasValue(TextFormat.UnderlineKey) && format.Underline)
                charFormat.UnderlineStyle = UnderlineStyle.Single;


            if (format.HasValue(TextFormat.StrikeKey) && format.Strike)
                charFormat.Strikeout = true;

            if (format.HasValue(TextFormat.FontColorKey) && format.FontColor != Color.Empty)
                charFormat.TextColor = format.FontColor;

            if (format.HasValue(TextFormat.FontFamilyKey) && format.FontFamily.Length > 0)
            {
                char[] singleQuotes = new char[1] { '\'' };
                charFormat.FontName = format.FontFamily.Trim(singleQuotes);
            }
            if (format.HasValue(TextFormat.FontSizeKey))
                charFormat.FontSize  = format.FontSize;
            else if (CurrentPara.ParaStyle != null && CurrentPara.ParaStyle.CharacterFormat.HasValue(WCharacterFormat.FontSizeKey))
            {
                charFormat.FontSize = CurrentPara.ParaStyle.CharacterFormat.FontSize;
            }
            else if (!charFormat.HasValue(WCharacterFormat.FontSizeKey))
            {
                WParagraphStyle styleNormalWeb = m_bodyItems.Document.Styles.FindByName("Normal (Web)") as WParagraphStyle;
                if (styleNormalWeb != null)
                {
                    charFormat.FontSize = (styleNormalWeb.CharacterFormat.FontSize != 12f) ? styleNormalWeb.CharacterFormat.FontSize : 12f;
                }
            }


            if (format.HasValue(TextFormat.BackColorKey) && format.BackColor != Color.Empty)
                charFormat.TextBackgroundColor = format.BackColor;

            if (format.SubSuperScript != SubSuperScript.None)
                charFormat.SubSuperScript = format.SubSuperScript;

            if (format.HasValue(TextFormat.CharacterSpacingKey))
                charFormat.CharacterSpacing = format.CharacterSpacing;

            if (format.HasValue(TextFormat.AllCapsKey))
                charFormat.AllCaps = format.AllCaps;
            //Apply Hidden property
            if (format.HasValue(TextFormat.HiddenKey))
                charFormat.Hidden = format.Hidden;
            else if (CurrentPara.IsInCell)
            {
                if ((CurrentPara.OwnerTextBody as WTableCell).CellFormat.Hidden)
                    charFormat.Hidden = true;
                else if ((CurrentPara.OwnerTextBody as WTableCell).OwnerRow.RowFormat.HasValue(RowFormat.HiddenKey))
                    charFormat.Hidden = (CurrentPara.OwnerTextBody as WTableCell).OwnerRow.RowFormat.Hidden;
                else if ((CurrentPara.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.TableFormat.HasValue(RowFormat.HiddenKey))
                    charFormat.Hidden = (CurrentPara.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.TableFormat.Hidden;
            }
        }
        /// <summary>
        /// Apply the current div format to the character format of the textrange
        /// </summary>
        /// <param name="tr"></param>
        private void ApplyDivCharacterFormat(WCharacterFormat charFormat)
        {
            if (currDivFormat == null)
                return;

            if (currDivFormat.FontSize > 0)
                charFormat.FontSize = currDivFormat.FontSize;
            
            if (currDivFormat.FontFamily.Length > 0)
                charFormat.FontName = currDivFormat.FontFamily;

            if (currDivFormat.HasValue(TextFormat.FontColorKey) && currDivFormat.FontColor != Color.Empty)
                charFormat.ForeColor = currDivFormat.FontColor;

            if (currDivFormat.HasValue(TextFormat.BoldKey))
                charFormat.Bold = currDivFormat.Bold;

            if (currDivFormat.HasValue(TextFormat.UnderlineKey))
                charFormat.UnderlineStyle = (currDivFormat.Underline) ? UnderlineStyle.Single : UnderlineStyle.None;

            if (currDivFormat.HasValue(TextFormat.StrikeKey))
                charFormat.Strikeout = currDivFormat.Strike;

            if (currDivFormat.HasValue(TextFormat.ItalicKey))
                charFormat.Italic = currDivFormat.Italic;

            if (currDivFormat.HasValue(TextFormat.SubSuperScriptKey))
                charFormat.SubSuperScript = currDivFormat.SubSuperScript;
        }
        /// <summary>
        /// Ensures the style.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private TextFormat EnsureStyle(XmlNode node)
        {
            bool present = ParseStyle(node);

            if (!present)
            {
                return AddStyle();
            }

            return CurrentFormat;
        }
        /// <summary>
        /// Extract the value alone without units
        /// </summary>
        /// <param name="value">The Value</param>
        /// <returns></returns>
        internal string ExtractValue(string value)
        {
            float result = float.MinValue;
            if (value.EndsWith("pt"))
                return value.Replace("pt", string.Empty);
            else if (value.EndsWith("%"))
            {
                float tempValue = Convert.ToSingle(value.Replace("%", string.Empty), CultureInfo.InvariantCulture);
                float width;
                if (m_currParagraph != null)
                    width = m_currParagraph.Document.Sections[0].PageSetup.ClientWidth;
                else if (m_currTable != null)
                    width = m_currTable.Document.Sections[0].PageSetup.ClientWidth;
                else
                    width = 0;
                result = (tempValue / 100) * width;
                return result.ToString(CultureInfo.InvariantCulture);
            }
            else if (value.EndsWith("em"))
            {
                float tempValue = Convert.ToSingle(value.Replace("em", string.Empty), CultureInfo.InvariantCulture);
                result = (float) (tempValue * 12);
            }
            else if (value.EndsWith("in"))
            {
                float tempValue = Convert.ToSingle(value.Replace("in", string.Empty), CultureInfo.InvariantCulture);
                result = PointsConverter.FromInch(tempValue);
            }
            else if (value.EndsWith("cm"))
            {
                float tempValue = Convert.ToSingle(value.Replace("cm", string.Empty), CultureInfo.InvariantCulture);
                result = PointsConverter.FromCm(tempValue);
            }
            else if (value.EndsWith("pc"))
            {
                float tempValue = Convert.ToSingle(value.Replace("pc", string.Empty), CultureInfo.InvariantCulture);
                result = (float)(tempValue * 12);
            }
            else if (value.EndsWith("mm"))
            {
                float tempValue = Convert.ToSingle(value.Replace("mm", string.Empty), CultureInfo.InvariantCulture);
                result = (float)UnitsConvertor.Instance.ConvertUnits(tempValue, PrintUnits.Millimeter, PrintUnits.Point);
            }
            else
            {
                float tempValue = Convert.ToSingle(value.Replace("px", string.Empty), CultureInfo.InvariantCulture);
                result = (float)(tempValue * 0.75);
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Parses the style.
        /// </summary>
        /// <param name="node">The node.</param>
        private bool ParseStyle(XmlNode node)
        {
            string style = GetAttributeValue(node, "style");
            string[] borderStyle = { "dashed", "dotted", "double", "groove", "inset", "outset", "ridge", "solid", "hidden" };
            if (style.Length != 0)
            {
                // Clones last format
                TextFormat format = AddStyle();
                format.Borders = new TableBorders(null);
                // Splits by tokens
                string[] styleParams = style.Split(';', ':');

                for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
                {
                    char[] trimChar = new char [] { '\'','\"' };
                    string paramName = styleParams[i].ToLower().Trim();
                    string paramValue = styleParams[i + 1].ToLower().Trim();
                    paramValue = paramValue.Trim(trimChar);
                    GetFormat(format, paramName, paramValue,node);
                }
                return true;
            }

            return false;
        }
        /// <summary>
        /// Get foramt
        /// </summary>
        /// <param name="format"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        private void GetFormat(TextFormat format, string paramName, string paramValue,XmlNode node)
        {
            char[] space = new char[] { ' ' };
            string[] value;
            int red, green, blue;
            if (paramValue.ToLower().Contains("inherit") && paramName.ToLower() != "page-break-before" && paramName.ToLower() != "page-break-after" && paramName != "page-break-inside")
                return;
            try
            {
                switch (paramName)
                {
                    case "display":
                        if (node.LocalName != "label")
                        {
                            if (paramValue == "none")
                                format.Hidden = true;
                            else
                                format.Hidden = false;
                        }
                        break;
                    case "white-space":
                        if (paramValue == "pre")
                            format.IsPreserveWhiteSpace = true;
                        else
                            format.IsPreserveWhiteSpace = false;
                        break;
                    case "text-transform":
                        switch (paramValue)
                        {
                            case "uppercase":
                                format.AllCaps = true;
                                break;
                            case "none":
                                format.AllCaps = false;
                                break;
                        }
                        break;
                    case "letter-spacing":
                        if (paramValue == "normal")
                            format.CharacterSpacing = 0;
                        else
                            format.CharacterSpacing = Convert.ToSingle(ExtractValue(paramValue));
                        break;
                    case "font-family":
                        format.FontFamily = GetFontName(paramValue);
                        break;
                    case "font-style":
                        if (paramValue == "italic" || paramValue == "oblique")
                            format.Italic = true;
                        if (paramValue == "strike")
                            format.Strike = true;
                        if (paramValue == "normal")
                            format.Italic = false;
                        break;
                    case "font-weight":
                        if (paramValue == "normal")
                            format.Bold = false;
                        else
                            format.Bold = true;
                        break;
                    case "font-size":
                        if (paramValue == "smaller")
                            format.FontSize = 10f;
                        else
                            format.FontSize = (float)ConvertSize(paramValue, format.FontSize);
                        break;
                    case "height":
                    case "line-height":
                        if (paramValue != "normal")
                        {
                            if (paramValue.EndsWith("pt") || paramValue.EndsWith("px") || paramValue.EndsWith("em") || paramValue.EndsWith("cm") || paramValue.EndsWith("pc"))
                            {
                                format.LineSpacingRule = LineSpacingRule.AtLeast;
                                format.LineHeight = (float)Single.Parse(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            }
                            else if (paramValue.EndsWith("%"))
                            {
                                paramValue = paramValue.Replace("%", string.Empty);
                                format.LineSpacingRule = LineSpacingRule.Multiple;
                                format.LineHeight = (float)(Convert.ToSingle(paramValue) / 100) * 12;
                            }
                            else
                            {
                                format.LineSpacingRule = LineSpacingRule.Multiple;
                                format.LineHeight = Convert.ToInt32(paramValue) * 12;
                            }

                        }
                        else
                            format.IsLineHeightNormal = true;
                        break;
                    case "text-align":
                        format.TextAlign = GetHorizontalAlignment(paramValue);
                        break;
                    case "text-decoration":
                        if (paramValue == "underline")
                            format.Underline = true;
                        if (paramValue == "line-through")
                            format.Strike = true;
                        if (paramValue == "none")
                        {
                            format.Underline = false;
                            format.Strike = false;
                        }
                        break;
                    case "color":
                        format.FontColor = GetColor(paramValue);
                        break;
                    case "background":
                    case "background-color":
                        string tagName = node.Name.ToLower();
                        if (tagName != "table" && tagName != "th" && tagName != "td")
                        {
                            format.BackColor = GetColor(paramValue);
                        }
                        if (paramValue == "transparent")
                            format.BackColor = Color.Empty;
                        break;
                    case "margin-left":
                        if (paramValue.ToLower() != "auto")
                            format.LeftMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        if (node.LocalName.ToLower() == "ul" || node.LocalName.ToLower() == "ol")
                            UpdateListLeftIndentStack(format.LeftMargin, true);
                        break;
                    case "text-indent":
                        if (node.LocalName.ToLower() != "ul" && node.LocalName.ToLower() != "ol")
                            format.TextIndent = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        break;
                    case "margin-right":
                        if (paramValue.ToLower() != "auto")
                            format.RightMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        break;
                    case "margin-top":
                        if (paramValue.ToLower() != "auto")
                            format.TopMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        else
                            format.TopMargin = -1.0f;
                        break;
                    case "margin-bottom":
                        if (paramValue.ToLower() != "auto")
                            format.BottomMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        else
                            format.BottomMargin = -1.0f;
                        break;
                    case "margin":
                        value = paramValue.Split(space);
                        int count = value.Length;
                        switch (count)
                        {
                            case 1:
                                if (value[0] != "auto")
                                {
                                    float margin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                    format.TopMargin = margin;
                                    format.RightMargin = margin;
                                    format.BottomMargin = margin;
                                    format.LeftMargin = margin;
                                }
                                if (node.LocalName.ToLower() == "ul" || node.LocalName.ToLower() == "ol")
                                    UpdateListLeftIndentStack(format.LeftMargin, true);
                                break;
                            case 2:
                                if (value[0] != "auto")
                                    format.TopMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                if (value[1] != "auto")
                                    format.RightMargin = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                if ((node.LocalName.ToLower() == "ol" || node.LocalName.ToLower() == "ul"))
                                    UpdateListLeftIndentStack(format.LeftMargin, false);
                                break;
                            case 3:
                                if (value[0] != "auto")
                                    format.TopMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                if (value[1] != "auto")
                                    format.RightMargin = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                if (value[2] != "auto")
                                    format.BottomMargin = Convert.ToSingle(ExtractValue(value[2]), CultureInfo.InvariantCulture);
                                if ((node.LocalName.ToLower() == "ol" || node.LocalName.ToLower() == "ul"))
                                    UpdateListLeftIndentStack(format.LeftMargin, false);
                                break;
                            case 4:
                                if (value[0] != "auto")
                                    format.TopMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                if (value[1] != "auto")
                                    format.RightMargin = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                if (value[2] != "auto")
                                    format.BottomMargin = Convert.ToSingle(ExtractValue(value[2]), CultureInfo.InvariantCulture);
                                if (value[3] != "auto")
                                    format.LeftMargin = Convert.ToSingle(ExtractValue(value[3]), CultureInfo.InvariantCulture);
                                if (node.LocalName.ToLower() == "ul" || node.LocalName.ToLower() == "ol")
                                    UpdateListLeftIndentStack(format.LeftMargin, true);
                                break;
                        }

                        break;
                    case "border-bottom":
                        ParseBorder(paramValue, format.Borders.BottomColor, format.Borders.BottomWidth, format.Borders.BottomStyle);
                        break;
                    case "border-top":
                        ParseBorder(paramValue, format.Borders.TopColor, format.Borders.TopWidth, format.Borders.TopStyle);
                        break;
                    case "border-left":
                        ParseBorder(paramValue, format.Borders.LeftColor, format.Borders.LeftWidth, format.Borders.LeftStyle);
                        break;
                    case "border-right":
                        ParseBorder(paramValue, format.Borders.RightColor, format.Borders.RightWidth, format.Borders.RightStyle);
                        break;
                    case "outline-color":
                    case "border-color":
                        format.Borders.AllColor = GetColor(paramValue);
                        break;
                    case "border-left-color":
                        format.Borders.LeftColor = GetColor(paramValue);
                        break;
                    case "border-right-color":
                        format.Borders.RightColor = GetColor(paramValue);
                        break;
                    case "border-top-color":
                        format.Borders.TopColor = GetColor(paramValue);
                        break;
                    case "border-bottom-color":
                        format.Borders.BottomColor = GetColor(paramValue);
                        break;
                    case "outline-width":
                    case "border-width":
                        format.Borders.AllWidth = CalculateBorderWidth(paramValue);
                        break;
                    case "border-left-width":
                        format.Borders.LeftWidth = CalculateBorderWidth(paramValue);
                        break;
                    case "border-right-width":
                        format.Borders.RightWidth = CalculateBorderWidth(paramValue);
                        break;
                    case "border-top-width":
                        format.Borders.TopWidth = CalculateBorderWidth(paramValue);
                        break;
                    case "border-bottom-width":
                        format.Borders.BottomWidth = CalculateBorderWidth(paramValue);
                        break;
                    case "outline-style":
                    case "border-style":
                        format.Borders.AllStyle = ToBorderType(paramValue);
                        break;
                    case "border-left-style":
                        format.Borders.LeftStyle = ToBorderType(paramValue);
                        break;
                    case "border-right-style":
                        format.Borders.RightStyle = ToBorderType(paramValue);
                        break;
                    case "border-top-style":
                        format.Borders.TopStyle = ToBorderType(paramValue);
                        break;
                    case "border-bottom-style":
                        format.Borders.BottomStyle = ToBorderType(paramValue);
                        break;
                    case "border":
                        ParseBorder(paramValue, format.Borders.RightColor, format.Borders.RightWidth, format.Borders.RightStyle);
                        ParseBorder(paramValue, format.Borders.LeftColor, format.Borders.LeftWidth, format.Borders.LeftStyle);
                        ParseBorder(paramValue, format.Borders.BottomColor, format.Borders.BottomWidth, format.Borders.BottomStyle);
                        ParseBorder(paramValue, format.Borders.TopColor, format.Borders.TopWidth, format.Borders.TopStyle);
                        break;
                    case "page-break-before":
                        if (node.LocalName.ToLower() == "br")
                        {
                            Break brk = null;
                            if (paramValue == "always")
                                brk = CurrentPara.AppendBreak(BreakType.PageBreak);
                            else
                            {
                                brk = CurrentPara.AppendBreak(BreakType.LineBreak);
                                if (paramValue == "avoid" || paramValue == "inherit")
                                {
                                    string attrValue = GetAttributeValue(node, "clear");
                                    if (attrValue == "all" || attrValue == "left" || attrValue == "right")
                                        brk.HtmlToDocLayoutInfo.RemoveLineBreak = false;
                                }
                            }
                        }
                        else if (paramValue == "always")
                            format.PageBreakBefore = true;
                        else if (paramValue == "auto")
                            format.PageBreakBefore = false;
                        break;
                    case "page-break-after":

                        if (node.LocalName.ToLower() == "br")
                        {
                            Break brk = null;
                            if (paramValue == "always")
                                brk = CurrentPara.AppendBreak(BreakType.PageBreak);
                            else
                            {
                                brk = CurrentPara.AppendBreak(BreakType.LineBreak);
                                if (paramValue == "avoid" || paramValue == "inherit")
                                {
                                    string attrValue = GetAttributeValue(node, "clear");
                                    if (attrValue == "all" || attrValue == "left" || attrValue == "right")
                                        brk.HtmlToDocLayoutInfo.RemoveLineBreak = false;
                                }
                            }
                        }
                        else if (paramValue == "always")
                            format.PageBreakAfter = true;
                        else if (paramValue == "auto")
                            format.PageBreakAfter = false;
                        break;
                    case "page-break-inside":
                        if (node.LocalName.ToLower() == "br")
                        {
                            Break brk = CurrentPara.AppendBreak(BreakType.LineBreak);
                            string attrValue = GetAttributeValue(node, "clear");
                            if (attrValue == "all" || attrValue == "left" || attrValue == "right")
                                brk.HtmlToDocLayoutInfo.RemoveLineBreak = false;
                        }
                        break;
                    case "padding":
                        value = paramValue.Split(space);
                        int counts = value.Length;
                        switch (counts)
                        {
                            case 1:
                                format.LeftMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                break;
                            case 2:
                                format.LeftMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                format.TopMargin = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                break;
                            case 3:
                                format.LeftMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                format.TopMargin = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                format.RightMargin = Convert.ToSingle(ExtractValue(value[2]), CultureInfo.InvariantCulture);
                                break;
                            case 4:
                                format.LeftMargin = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                format.TopMargin = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                format.RightMargin = Convert.ToSingle(ExtractValue(value[2]), CultureInfo.InvariantCulture);
                                format.BottomMargin = Convert.ToSingle(ExtractValue(value[3]), CultureInfo.InvariantCulture);
                                break;
                        }
                        break;
                    case "padding-left":
                        format.LeftMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        break;
                    case "padding-top":
                        format.TopMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        break;
                    case "padding-right":
                        format.RightMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        break;
                    case "padding-bottom":
                        format.BottomMargin = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                        break;
                    case "word-break":
                        if (paramValue == "break-all")
                            format.WordWrap = false;
                        break;
                }
            }
            //To avoid the FormatException, we need to ignore the style Attribute which have Unknown attribute value
            catch
            {
                return;
            }
        }
        /// <summary>
        /// Get Color value
        /// </summary>
        /// <param name="attValue"></param>
        /// <returns></returns>
        private Color GetColor(string attValue)
        {          
            if (attValue.StartsWith("rgb"))
            {
                string rgb = attValue.Replace("rgb", string.Empty).Trim(new char[] { '(', ')', ' ' });
                int r, g, b;
                string[] splitRGBValues = rgb.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitRGBValues.Length == 3)
                {
                    int.TryParse(splitRGBValues[0], out r);
                    int.TryParse(splitRGBValues[1], out g);
                    int.TryParse(splitRGBValues[2], out b);

                    return Color.FromArgb(r, g, b);
                }
                else
                    return Color.Empty;
            }
            else
            {
#if WINRT
				return FromHtml(attValue);
#else
                return (ColorTranslator .FromHtml(attValue));
#endif
            }
        }
#if WINRT
        /// <summary>
        /// Get HTML color
        /// </summary>
        /// <param name="htmlColor"></param>
        /// <returns></returns>
        private Color FromHtml(string htmlColor)
        {
            Color empty = Color.Empty;
            if ((htmlColor != null) && (htmlColor.Length != 0))
            {
                if ((htmlColor[0] == '#') && ((htmlColor.Length == 7) || (htmlColor.Length == 4)))
                {
                    if (htmlColor.Length == 7)
                    {
                        empty = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 0x10), Convert.ToInt32(htmlColor.Substring(3, 2), 0x10), Convert.ToInt32(htmlColor.Substring(5, 2), 0x10));
                    }
                    else
                    {
                        string str = char.ToString(htmlColor[1]);
                        string str2 = char.ToString(htmlColor[2]);
                        string str3 = char.ToString(htmlColor[3]);
                        empty = Color.FromArgb(Convert.ToInt32(str + str, 0x10), Convert.ToInt32(str2 + str2, 0x10), Convert.ToInt32(str3 + str3, 0x10));
                    }
                }
                if (empty.IsEmpty && string.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
                {
                    empty = Color.LightGray;
                }
            }
            return empty;
        }
#endif
        /// <summary>
        /// Get Font Name
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private string GetFontName(string paramValue)
        {
            string fontName = paramValue;
            if (paramValue.Trim().Contains(","))
            {
                int index = paramValue.Trim().IndexOf(',');
                fontName = paramValue.Trim().Substring(0, index);
            }
            return fontName;
        }
        /// <summary>
        /// Parse Border
        /// </summary>
        /// <param name="paramValue"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderWidth"></param>
        /// <param name="style"></param>
        private void ParseBorder(string paramValue,Color borderColor,float borderWidth, BorderStyle style)
        {
            string[] borderStyle = { "dashed", "dotted", "double", "groove", "inset", "outset", "ridge", "solid", "hidden" };
            string[] value = paramValue.Split(' ');
            int red, green, blue;
            if (paramValue == "none" || paramValue =="medium none")
            {
                borderColor = Color.Empty;
                style = BorderStyle.None;
                borderWidth = 0;
                return;
            }
            for (int j = 0; j < value.Length; j++)
            {
                if (value[j].StartsWith("#"))
                {
                    value[j] = value[j].Replace("#", string.Empty);
                    red = int.Parse(value[j].Substring(0, 2), NumberStyles.AllowHexSpecifier);
                    green = int.Parse(value[j].Substring(2, 2), NumberStyles.AllowHexSpecifier);
                    blue = int.Parse(value[j].Substring(4, 2), NumberStyles.AllowHexSpecifier);
                    borderColor = Color.FromArgb(red, green, blue);
                }
                else if (IsBorderWidth (value [j]))
                {
                    borderWidth = CalculateBorderWidth(value[j]);
                }
                else
                {
                    foreach (string styles in borderStyle)
                    {
                        if (value[j] == styles)
                            m_bBorderStyle = true;
                    }
                    if (m_bBorderStyle)
                    {
                        style = ToBorderType(value[j]);
                        m_bBorderStyle = false;
                    }
                    else
                        borderColor = GetColor(value[j]);
                }                   
                
            }
        }
        /// <summary>
        /// Checks whether the value is a border width
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsBorderWidth(string value)
        {
            if (value.EndsWith("pt") || value.EndsWith("px") || value.EndsWith("in") || value.EndsWith("em") || value.EndsWith("cm") || value.EndsWith("pc"))
                return true;
            else if (value == "medium" || value == "thick" || value == "thin")
                return true;

            return false;
              
        }
        /// <summary>
        /// Calculate border width
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float CalculateBorderWidth(string value)
        {
            float width=0;

            if (value.EndsWith("pt") || value.EndsWith("px") || value.EndsWith("em") || value.EndsWith("cm") || value.EndsWith("pc"))
            {
                width=Convert.ToSingle(ExtractValue(value));
            }
            else if(value.EndsWith ("in"))
            {
                string[] seperatedValue = SeperateParamValue(value);
                  if (seperatedValue[1] == null)
                    width = DEF_THINVALUE;
                  else
                    width = Convert.ToSingle(ExtractValue(value));
            }
            
            else
            {
                 if (value == "medium")
                  {
                     width = DEF_MEDIUMVALUE;
                  }
                 else if (value == "thick")
                  {
                     width = DEF_THICKVALUE;
                  }
            }

            return width ;

        }
        /// <summary>
        /// Seperate param value with its unit
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private string[] SeperateParamValue(string paramValue)
        {
            string[] value = new string[2];
            char ch;
            for (int i = 0; i < paramValue.Length; i++)
            {
                ch = paramValue[i];
                if (Char.IsDigit(ch))
                    value[1] += ch;
                else
                    value[0] += ch;
            }
            return value;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private BorderStyle ToBorderType(string type)
        {
            switch (type.ToLower())
            {
                case "dashed":
                    return BorderStyle.DashLargeGap;
                case "dotted":
                    return BorderStyle.Dot;
                case "double":
                    return BorderStyle.Double;
                case "groove":
                    return BorderStyle.Engrave3D;
                case "inset":
                    return BorderStyle.Inset;
                case "outset":
                    return BorderStyle.Outset;
                case "ridge":
                    return BorderStyle.Emboss3D;
                case "solid":
                    return BorderStyle.Single;
                case "none":
                case "hidden":
                    return BorderStyle.None;
            }

            return BorderStyle.None;
        }
        /// <summary>
        /// Leaves the style.
        /// </summary>
        /// <param name="stylePresent">if the style is present, set to <c>true</c>.</param>
        private void LeaveStyle(bool stylePresent)
        {
            if (stylePresent)
            {
                m_styleStack.Pop();
            }
        }
        /// <summary>
        /// Updates the paragraph's format.
        /// </summary>
        /// <param name="node">The node.</param>
        private void UpdateParaFormat(XmlNode node,WParagraphFormat pformat )
        {
            if (m_currParagraph == null)
                return;

            string align = GetAttributeValue(node, "align");
            string style = GetAttributeValue(node, "style");

            if (style.Length != 0)
            {
                string[] styleParams = style.Split(';', ':');
                for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
                {
                    char[] trimChar = new char[] { '\'', '\"' };
                    string paramName = styleParams[i].ToLower().Trim();
                    string paramValue = styleParams[i + 1].ToLower().Trim();
                    paramValue = paramValue.Trim(trimChar);
                    if (paramName == "text-align")
                        align = paramValue;
                }
            }
       

            if (align != string.Empty && node.Name.ToLower()!="table")
            {
                pformat.HorizontalAlignment = GetHorizontalAlignment(align);
            }
            else if (m_currParagraph.IsInCell)
            {
                if (m_bIsAlignAttrDefinedInRowNode)
                    pformat.HorizontalAlignment = m_horizontalAlignmentDefinedInRowNode;                  
                if (m_bIsAlignAttriDefinedInCellNode)
                    pformat.HorizontalAlignment = m_horizontalAlignmentDefinedInCellNode;
            }
        }
        /// <summary>
        /// Adds the style.
        /// </summary>
        /// <returns></returns>
        private TextFormat AddStyle()
        {
            // Clones last format
            TextFormat format = (m_styleStack.Count > 0) ?
              (m_styleStack.Peek() as TextFormat).Clone() :
              new TextFormat();

            m_styleStack.Push(format);
            return format;
        }
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrName">Name of the attr.</param>
        /// <returns></returns>
        private string GetAttributeValue(XmlNode node, string attrName)
        {
            attrName = attrName.ToLower();
            XmlAttribute attr = null;
            for (int i = 0; i < node.Attributes.Count; i++)
            {
                attr = node.Attributes[i];
                if (attr.LocalName.ToLower() == attrName)
                    return attr.Value;
            }

            return string.Empty;
        }
        /// <summary>
        /// Gets the style attribute value
        /// </summary>
        /// <param name="styleAttr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetStyleAttributeValue(string styleAttr, string styleAttrName)
        {
            string[] styleParams = styleAttr.Split(';', ':');

            for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
            {
                char[] trimChar = new char[] { '\'', '\"' };
                string paramName = styleParams[i].ToLower().Trim();
                string paramValue = styleParams[i + 1].ToLower().Trim();
                paramValue = paramValue.Trim(trimChar);

                if (paramName == styleAttrName)
                {
                    return paramValue;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Converts the size.
        /// </summary>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        private double ConvertSize(string paramValue, float baseSize)
        {
            if (baseSize < 0)
                baseSize = 3;

            switch (paramValue)
            {
                case "xx-small":
                    return 7.5;
                case "x-small":
                    return 10;
                case "small":
                    return 12;
                case "medium":
                    return 13.5;
                case "large":
                    return 18;
                case "x-large":
                    return 24;
                case "xx-large":
                    return 36;
                case "smaller":
                    return 10;
                case "bigger":
                    return 12;
                case "larger":
                    return 13.5;
                default:
					Font convertSize;

                    if (paramValue.EndsWith("%"))
                    {
                        return baseSize * GetNumberBefore(paramValue, "%") / 100;
                    }
                    else if (paramValue.EndsWith("em"))
                    {
                        return baseSize * GetNumberBefore(paramValue, "em");
                    }
                    else if (paramValue.EndsWith("ex"))
                    {
                        return baseSize / 2 * GetNumberBefore(paramValue, "ex");
                    }
                    else if (paramValue.EndsWith("pt"))
                    {
                        return GetNumberBefore(paramValue, "pt");
                    }
                    else if (paramValue.EndsWith("in"))
                    {
                        float tempValue = Convert.ToSingle(paramValue.Replace("in", string.Empty), CultureInfo.InvariantCulture);
                        return PointsConverter.FromInch(tempValue);
                    }
                    else if (paramValue.EndsWith("cm"))
                    {
                        float tempValue = Convert.ToSingle(paramValue.Replace("cm", string.Empty), CultureInfo.InvariantCulture);
                        return PointsConverter.FromCm(tempValue);
                    }
                    else if (paramValue.EndsWith("mm"))
                    {
                        float tempValue = Convert.ToSingle(paramValue.Replace("mm", string.Empty), CultureInfo.InvariantCulture);
                        return (float)UnitsConvertor.Instance.ConvertUnits(tempValue, PrintUnits.Millimeter, PrintUnits.Point);
                    }
                    else if (paramValue.EndsWith("pc"))
                    {
                        float tempValue = Convert.ToSingle(paramValue.Replace("pc", string.Empty), CultureInfo.InvariantCulture);
                        return (float)(tempValue * 12);
                    }
                    else if (paramValue.EndsWith("px"))
                    {
                        float tempValue = Convert.ToSingle(paramValue.Replace("px", string.Empty), CultureInfo.InvariantCulture);
                        return (float)(tempValue / 1.33);
                    }
                    else
                    {
                        float tempValue = Single.Parse(paramValue, CultureInfo.InvariantCulture);
                        return (float)(tempValue / 1.33);
                    }
                    break;
            }

            return 0;
        }
        /// <summary>
        /// Gets the number before.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        private float GetNumberBefore(string val, string end)
        {
            val = val.Substring(0, val.IndexOf(end));
            return Single.Parse(val, CultureInfo.InvariantCulture);
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Called when [validation].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnValidation(object sender, ValidationEventArgs args)
        {
            throw new NotSupportedException(args.Message, args.Exception);
        }
#endif
        #endregion

        #region Implementation / list
        /// <summary>
        /// Builds the list style.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="node">The xml node.</param>
        private void BuildListStyle(ListPatternType type, XmlNode node)
        {
            ListStyle style = null;
            if (ListStack.Count > 0)
                style = ListStack.Peek();
            else
            {
                CreateListStyle(node);
                style = ListStack.Peek();
            }
            if ( IsListNodeStart(node))
                CreateListLevel(style, type, node);
            m_currParagraph.ListFormat.ApplyStyle(style.Name);

            if (LfoStack.Count > 0)
                m_currParagraph.ListFormat.LFOStyleName = LfoStack.Peek();
            m_currParagraph.ListFormat.ListLevelNumber = m_curListLevel;
        }
        /// <summary>
        /// Creates a list level
        /// </summary>
        /// <param name="style"></param>
        /// <param name="type"></param>
        public void CreateListLevel(ListStyle style,ListPatternType type,XmlNode node)
        {
            WListLevel listLevel = style.Levels[m_curListLevel];
            if (m_listLevelNo.Contains(m_curListLevel) && listLevel.PatternType != type)
                listLevel = CreateListOverrideStyle(m_curListLevel, node);
            if (!m_listLevelNo.Contains(m_curListLevel))
                m_listLevelNo.Add(m_curListLevel);
            listLevel.PatternType = type;
            listLevel.NumberPosition -= 18f;
            listLevel.TabSpaceAfter = 10f;
            listLevel.FollowCharacter = FollowCharacterType.Tab;
            if (type == ListPatternType.Bullet)
                UpdateBulletChar(m_curListLevel, listLevel);
            else
            {
                listLevel.NumberPrefix = string.Empty;
                listLevel.NumberSufix = ".";
                //Removes the previous initialized font name.
                listLevel.CharacterFormat.PropertiesHash.Remove(WCharacterFormat.FontNameKey);
                listLevel.CharacterFormat.PropertiesHash.Remove(WCharacterFormat.FontNameAsciiKey);
                listLevel.CharacterFormat.PropertiesHash.Remove(WCharacterFormat.FontNameBidiKey);
                listLevel.CharacterFormat.PropertiesHash.Remove(WCharacterFormat.FontNameFarEastKey);
                listLevel.CharacterFormat.PropertiesHash.Remove(WCharacterFormat.FontNameNonFarEastKey);
            }

            string value = GetAttributeValue(node, "VALUE");
            string start = GetAttributeValue(node.ParentNode, "START");

            if (!string.IsNullOrEmpty(value))
                try
                {
                    listLevel.StartAt = Convert.ToInt32(value);
                }
                catch
                {
                    listLevel.StartAt = PrepareListStart(value, GetAttributeValue(node.ParentNode, "TYPE"));
                }
            else if (!string.IsNullOrEmpty(start))
                try
                {
                    listLevel.StartAt = Convert.ToInt32(start);
                }
                catch
                {
                    listLevel.StartAt = PrepareListStart(start, GetAttributeValue(node.ParentNode, "TYPE"));
                }

        }
         /// <summary>
        /// Update bullet char for list level
        /// </summary>
        /// <param name="listLevelNo"></param>
        /// <param name="listLevel"></param>
        private void UpdateBulletChar(int listLevelNo, WListLevel listLevel)
        {
            switch (listLevelNo % 3)
            {
                case 0:
                    listLevel.BulletCharacter = ListStyle.DEF_BULLLET_FIRST;
                    listLevel.CharacterFormat.FontName = "Symbol";
                    break;
                case 1:
                    listLevel.BulletCharacter = ListStyle.DEF_BULLLET_SECOND;
                    listLevel.CharacterFormat.FontName = "Courier New";
                    break;
                default:
                    listLevel.BulletCharacter = ListStyle.DEF_BULLLET_THIRD;
                    listLevel.CharacterFormat.FontName = "Wingdings";
                    break;
            }
            //Sets the font size to 10 for all bullet list level.
            listLevel.CharacterFormat.FontSize = 10;
        }
        /// <summary>
        /// Creates the list style.
        /// </summary>
        /// <param name="node"></param>
        private void CreateListStyle(XmlNode node)
        {
            string styleName = null;
            ListStyle style = null;

            if (m_userListStyle != null)
                ListStack.Push(m_userListStyle);
            else
            {
                styleName = "ListStyle" + m_bodyItems.Document.ListStyles.Count.ToString();
                style = m_bodyItems.Document.AddListStyle(ListType.Bulleted, styleName);
                style.IsHybrid = true;
            }
            ListStack.Push(style);
        }
        /// <summary>
        /// create the list Override style.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private WListLevel CreateListOverrideStyle(int levelNumber, XmlNode node)
        {
            ListOverrideStyle listOverrideStyle = new ListOverrideStyle(m_bodyItems.Document);
            listOverrideStyle.Name = "LfoStyle_" + Guid.NewGuid();
            m_bodyItems.Document.ListOverrides.Add(listOverrideStyle);
            OverrideLevelFormat levelFormat = new OverrideLevelFormat(m_bodyItems.Document);
            listOverrideStyle.OverrideLevels.Add(levelNumber, levelFormat);
            levelFormat.OverrideFormatting = true;
            LfoStack.Push(listOverrideStyle.Name);
            return levelFormat.OverrideListLevel;
        }
        /// <summary>
        /// Prepares the list start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        private int PrepareListStart(string start, string type)
        {
            if (type == "i" || type == "I")
            {
                return RomanToArabic(start);
            }
            else
            {
                byte letterCode = (byte)(start.ToCharArray())[0];

                if (letterCode >= 65 && letterCode <= 90)
                    return letterCode - 64;
                else if (letterCode >= 97 && letterCode <= 122)
                    return letterCode - 96;
                else
                    return 1;
            }
        }
        /// <summary>
        /// Determines whether is end of level.
        /// </summary>
        /// <param name="node">The xml node.</param>
        /// <returns>
        /// 	<c>true</c> if [is end of level] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsListNodeEnd(XmlNode node)
        {
            while (true)
            {
                if (node.NextSibling == null)
                    return true;
                else if (node.NextSibling.Name == @"#whitespace")
                    node = node.NextSibling;
                else
                    return false;
            }

            return false;
        }
        /// <summary>
        /// Determines whether is start of level.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if [is start of level] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsListNodeStart(XmlNode node)
        {
            while (true)
            {
                if (node.PreviousSibling == null)
                    return true;
                else if (node.PreviousSibling.Name == @"#whitespace")
                    node = node.PreviousSibling;
                else
                    return false;
            }
        }
        /// <summary>
        /// Determines whether the specified node is inner list.
        /// </summary>
        /// <param name="node">The xml node.</param>
        /// <returns>
        /// 	<c>true</c> if [is inner list] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInnerList(XmlNode node)
        {
            if (node.ParentNode != null
              && (node.ParentNode.Name.ToUpper() == "OL" || node.ParentNode.Name.ToUpper() == "UL"))
            {
                if (node.ParentNode.ParentNode != null
                  && (node.ParentNode.ParentNode.Name.ToUpper() == "BODY" || node.ParentNode.ParentNode.Name.ToUpper() == "HTML"))
                    return false;
            }

            return true;
        }
        #endregion

        #region Implementation / table
        /// <summary>
        /// Parses the table.
        /// 
        /// supported
        /// - table\tr\td construction
        /// - table or cell must have a width in pixel
        /// </summary>
        /// <param name="node">The node.</param>
        private void ParseTable(XmlNode node)
        {
            m_bIsBorderCollapse = false;
            TableBorders tblBorders = new TableBorders(null);
            SpanHelper spanHelper = new SpanHelper();
            spanHelper.TableGridCollection = new Dictionary<int, List<object>>();
            tableGrid.TableGridStack.Push(spanHelper.TableGridCollection);
            m_currTable = new WTable(m_bodyItems.Document, false);
            //Fix to include empty paragraph between two tables
            if (m_bodyItems.Count > 0)
            {
                if (m_bodyItems.LastItem.EntityType == EntityType.Table)
                {
                    WParagraph para = new WParagraph(m_bodyItems.Document);
                    para.BreakCharacterFormat.Hidden = true;
                    m_bodyItems.Add(para);
                }
            }
            m_bodyItems.Add(m_currTable);
            m_currTable.TableFormat.IsAutoResized = true;
            BodyItemCollection currBodyItems = m_bodyItems;
            
            TextFormat format = currDivFormat;
            if (m_bIsInDiv && currDivFormat != null && (!(m_currTable.OwnerTextBody is WTableCell) || NodeIsInDiv(node)))
            {
                ApplyDivTableFormat(node);
                if (m_styleStack.Count > 0)
                {
                    format = m_styleStack.Pop();
                    m_styleStack.Push(new TextFormat());
                }
            }

            ParseTableAttrs(node, spanHelper ,tblBorders);
            ParseTableRows(node, spanHelper, tblBorders);
            
            if (m_bIsInDiv && format != null && m_styleStack.Count > 0 && (!(m_currTable.OwnerTextBody is WTableCell) || NodeIsInDiv(node)))
            {
                TextFormat temp = m_styleStack.Pop();
                m_styleStack.Push(format);
            }

            // Restore current body items
            m_bodyItems = currBodyItems;

            // Set table cells width
            if (!(m_bodyItems.Owner is WTableCell))
                spanHelper.UpdateTable(m_bodyItems.LastItem as WTable, tableGrid.TableGridStack, ClientWidth);
            else
            {
				//Update Row span to the NestedTable
                spanHelper.UpdateNestedTableRowSpan(m_currTable);
                tableGrid.TableGridStack.Pop();
            }

            if (node.ParentNode.Name == "td")
                childTableWidth = m_currTable.Width;
            else
                childTableWidth = 0.0f;

            if (m_currTableFooterRowIndex != -1)
            {
                m_currTable.Rows.Add(m_currTable.Rows[m_currTableFooterRowIndex]);
                m_currTableFooterRowIndex = -1;
            }

            if (m_bIsBorderCollapse)
                m_currTable.TableFormat.CellSpacing = -1;
        }
        /// <summary>
        /// Apply the current div formt to the Table
        /// </summary>
        private void ApplyDivTableFormat(XmlNode node)
        {
            if (currDivFormat != null)
            {
                RowFormat tableFormat = m_currTable.TableFormat;
                if (currDivFormat.HasValue(TextFormat.BackColorKey))
                    tableFormat.BackColor = currDivFormat.BackColor;
                if (currDivFormat.HasValue(TextFormat.LeftMarginKey))
                    tableFormat.LeftIndent = currDivFormat.LeftMargin;
                if (currDivFormat.HasValue(TextFormat.TextAlignKey))
                {
                    switch (currDivFormat.TextAlign)
                    {
                        case HorizontalAlignment.Center:
                            tableFormat.HorizontalAlignment = RowAlignment.Center;
                            break;
                        case HorizontalAlignment.Right:
                            tableFormat.HorizontalAlignment = RowAlignment.Right;
                            break;
                        default:
                            tableFormat.HorizontalAlignment = RowAlignment.Left;
                            break;
                    }
                }
                if (currDivFormat.Borders.AllStyle != BorderStyle.None)
                {
                    tableFormat.Borders.Bottom.BorderType = tableFormat.Borders.Top.BorderType =
                    tableFormat.Borders.Left.BorderType = tableFormat.Borders.Right.BorderType = currDivFormat.Borders.AllStyle;

                    if (currDivFormat.Borders.AllColor != Color.Empty)
                    {
                        tableFormat.Borders.Bottom.Color = tableFormat.Borders.Top.Color =
                        tableFormat.Borders.Left.Color = tableFormat.Borders.Right.Color = currDivFormat.Borders.AllColor;
                    }
                    if (currDivFormat.Borders.AllWidth != -1.0f)
                    {
                        tableFormat.Borders.Bottom.LineWidth = tableFormat.Borders.Top.LineWidth =
                        tableFormat.Borders.Left.LineWidth = tableFormat.Borders.Right.LineWidth = currDivFormat.Borders.AllWidth;
                    }
                }


                if (currDivFormat.Borders.BottomStyle != BorderStyle.None)
                {
                    tableFormat.Borders.Bottom.BorderType = currDivFormat.Borders.BottomStyle;
                    tableFormat.Borders.Bottom.LineWidth = currDivFormat.Borders.BottomWidth;
                    tableFormat.Borders.Bottom.Color = currDivFormat.Borders.BottomColor;
                }
                if (currDivFormat.Borders.TopStyle != BorderStyle.None)
                {
                    tableFormat.Borders.Top.BorderType = currDivFormat.Borders.TopStyle;
                    tableFormat.Borders.Top.LineWidth = currDivFormat.Borders.TopWidth;
                    tableFormat.Borders.Top.Color = currDivFormat.Borders.TopColor;
                }
            }
        }
        /// <summary>
        /// Parses the table rows.
        /// </summary>
        /// <param name="node">The xml node.</param>
        private void ParseTableRows(XmlNode node, SpanHelper spanHelper, TableBorders tblBorders)
        {
            bool isCellsAutoResized = false;
            foreach (XmlNode rowNode in node.ChildNodes)
            {
                spanHelper.m_tblGrid = new List<object>();
                m_bIsAlignAttrDefinedInRowNode = false;
                m_bIsVAlignAttriDefinedInRowNode = false;
                if (rowNode.NodeType == XmlNodeType.Whitespace)
                    continue;

                if (rowNode.Name == "tbody" || rowNode.Name == "thead" || rowNode.Name == "tfoot")
                {
                    ParseTableRows(rowNode, spanHelper, tblBorders);
                    continue;
                }

                if (rowNode.Name.ToLower() == "col" || rowNode.Name.ToLower() == "colgroup")
                    continue;
                if (rowNode.Name.ToLower() != "tr")
                    throw new NotSupportedException("Html contains not wellformatted table");

                //Add Empty Row to the table
                WTableRow row = m_currTable.AddRow(false, false);
                //Apply Owner Table Formattings to the RowFormat
                row.RowFormat.ImportContainer(m_currTable.TableFormat);

                if (rowNode.ParentNode.Name == "thead")
                    row.IsHeader = true;
                else if (rowNode.ParentNode.Name == "tfoot")
                    m_currTableFooterRowIndex = m_currTable.Rows.Count - 1;

                ParseRowAttrs(rowNode, row);
                // Update Hidden property of the row based on parent node
                UpdateHiddenPropertyBasedOnParentNode(rowNode, row);
                spanHelper.ResetCurrColumn();

                foreach (XmlNode cellNode in rowNode.ChildNodes)
                {
                    m_bIsAlignAttriDefinedInCellNode = false;
                    if (cellNode.NodeType == XmlNodeType.Whitespace)
                        continue;

                    if (cellNode.Name.ToLower() == "col" || cellNode.Name.ToLower() == "colgroup")
                        continue;
                    if (cellNode.Name.ToLower() != "td" && cellNode.Name.ToLower() != "th")
                        throw new NotSupportedException("Html contains not wellformatted table");

                    WTableCell cell = row.AddCell(false);
                    m_bIsCellStyle = false;
                    m_bodyItems = cell.Items;
                    SetDefaultBorder(cell, cellNode);
                    m_currParagraph = (WParagraph)cell.AddParagraph();
                   
                    if (m_userStyle != null)
                        m_currParagraph.ApplyStyle(m_userStyle);

                    bool isTh = cellNode.Name.ToLower() == "th";
                    if (isTh)
                    {
                        m_bIsAlignAttriDefinedInCellNode = true;
                        TextFormat format = AddStyle();
                        format.TextAlign = HorizontalAlignment.Center;
                        m_horizontalAlignmentDefinedInCellNode = HorizontalAlignment.Center;
                        format.Bold = true;
                    }
                    ParseCellAttrs(cellNode, cell, spanHelper, tblBorders);

                    // Parses children
                    TraverseChildNodes(cellNode.ChildNodes);
                    LeaveStyle(m_bIsCellStyle);
                    if (m_currParagraph != null)
                        ApplyTextFormatting(m_currParagraph.BreakCharacterFormat);
                    //Remove Empty Paragraph
                    if (cell.Items.Count > 1 && (cell.Items[0] is WParagraph) && (cell.Items[0] as WParagraph).Items.Count == 0)
                    {
                        cell.Items.RemoveAt(0);
                    }
                    ApplyParagraphFormat(cellNode);
                    LeaveStyle(isTh);
                    m_horizontalAlignmentDefinedInCellNode = HorizontalAlignment.Left;
                    if (cell.CellFormat.PreferredWidth.WidthType == FtsWidth.Auto)
                    {
                        isCellsAutoResized = true;
                    }
                    if (!(cell.CellFormat.HasValue(CellFormat.ShadingColorKey)
                || cell.CellFormat.HasValue(CellFormat.ForeColorKey)
                || cell.CellFormat.HasValue(CellFormat.TextureStyleKey)))
                    {
                        WTable ownerTable = cell.OwnerRow.OwnerTable;
                        WTableRow ownerRow = cell.OwnerRow;
                        if (ownerRow.RowFormat.HasValue(RowFormat.ShadingColorKey))
                            cell.CellFormat.BackColor = ownerRow.RowFormat.BackColor;
                        else if (ownerTable.TableFormat.HasValue(RowFormat.ShadingColorKey))
                            cell.CellFormat.BackColor = ownerTable.TableFormat.BackColor;
                    }
                }
                //Update table grid collection from the stack
                spanHelper.TableGridCollection = tableGrid .TableGridStack.Pop();
                if (isCellsAutoResized &&
                    spanHelper.TableGridCollection.Count >= 1)
                {
                    for (int i = 0; i < spanHelper.TableGridCollection[spanHelper.TableGridCollection.Count - 1].Count 
                        && i < spanHelper.m_tblGrid.Count; i++)
                    {
                        WTableCell cell = row.Cells.Count > i ? row.Cells[i] : null;
                        if (cell != null && cell.CellFormat.PreferredWidth.WidthType == FtsWidth.Auto)
                            spanHelper.m_tblGrid[i] = spanHelper.TableGridCollection[spanHelper.TableGridCollection.Count - 1][i];
                    }
                }
                List<object> clonedList = new List<object>();
                foreach (object item in spanHelper.m_tblGrid)
                    clonedList.Add(item);
                spanHelper.TableGridCollection.Add(spanHelper.TableGridCollection.Count, clonedList);
                tableGrid.TableGridStack.Push(spanHelper.TableGridCollection);
                m_horizontalAlignmentDefinedInRowNode = HorizontalAlignment.Left;

                if (m_bIsVAlignAttriDefinedInRowNode)
                    foreach (WTableCell cell in row.Cells)
                    {
                        if (!cell.CellFormat.HasValue(CellFormat.VrAlignmentKey))
                            cell.CellFormat.VerticalAlignment = m_verticalAlignmentDefinedInRowNode;
                    }
            }
        }
        /// <summary>
        /// Set default border for the cell
        /// </summary>
        /// <param name="cell"></param>
        private void SetDefaultBorder(WTableCell cell, XmlNode node)
        {
            XmlNode parentTableNode = GetOwnerTable(node);
            string borderValue = string.Empty;
            borderValue = GetAttributeValue(parentTableNode, "border");
            //The following code clears the border when border attribute is not specified in table tag
            if (borderValue == string.Empty || Convert.ToUInt32(borderValue) == 0)
                cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
        }
        /// <summary>
        /// Get Owner table
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private XmlNode GetOwnerTable(XmlNode node)
        {
            while (node != null)
            {
                if (node.NodeType == XmlNodeType.Element && node.LocalName.ToLower() == "table")
                    return node;
                else
                    node = node.ParentNode;
            }
            return node;
        }
        /// <summary>
        /// Parses the cell attrs.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="spanHelper">The span helper.</param>
        /// <param name="tblBrdrs">The borders.</param>
        private void ParseCellAttrs(XmlNode node, WTableCell cell, SpanHelper spanHelper, TableBorders tblBrdrs)
        {
            TableBorders cellBrdrs = new TableBorders(tblBrdrs);
            int rowspan = 1;
            CellFormat format = cell.CellFormat;
            format.VerticalAlignment = VerticalAlignment.Middle;
            List<XmlAttribute> postponedAttrs = new List<XmlAttribute>();

            foreach (XmlAttribute attr in node.Attributes)
            {
                switch (attr.Name.ToLower())
                {
                    case "width":
                        if (attr.Value.ToLower() == "auto")
                            cell.CellFormat.PreferredWidth.WidthType = FtsWidth.Auto;
                        else if (attr.Value.EndsWith("%"))
                        {
                            cell.CellFormat.PreferredWidth.WidthType = FtsWidth.Percentage;
                            cell.CellFormat.PreferredWidth.Width = Convert.ToSingle(attr.Value.Replace("%", string.Empty), CultureInfo.InvariantCulture);
                            cell.Width = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            cell.CellFormat.PreferredWidth.WidthType = FtsWidth.Point;
                            cell.Width = cell.CellFormat.PreferredWidth.Width = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        }
                        break;
                    case "border":
                        cellBrdrs.AllWidth = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        break;
                    case "bordercolor":
                        cellBrdrs.AllColor = GetColor(attr.Value);
                        break;
                    case "style":
                        m_bIsCellStyle = true;
                        ParseCellStyle(attr, cell, cellBrdrs,node);
                        break;
                    case "colspan":
                        postponedAttrs.Add(attr);
                        break;
                    case "rowspan":
                        postponedAttrs.Add(attr);
                        break;
                    case "align":
                        m_bIsAlignAttriDefinedInCellNode = true;
                        m_horizontalAlignmentDefinedInCellNode = GetHorizontalAlignment(attr.Value);
                        break;
                    case "bgcolor":
                        cell.CellFormat.BackColor = GetColor(attr.Value);
                        break;
                    case "valign":
                        cell.CellFormat.VerticalAlignment = GetVerticalAlignment(attr.Value);
                        break;
                    case "border-collapse":
                        if (attr.Value.ToLower() == "collapse")
                            m_bIsBorderCollapse = true;
                        break;
                }
            }

            cellBrdrs.Apply(format);

            foreach (XmlAttribute attr in postponedAttrs)
            {
                switch (attr.Name.ToLower())
                {
                    case "colspan":
                        int colspan = Convert.ToInt32(attr.Value);
                        if (colspan != 1)
                        {
                            cell.CellFormat.HorizontalMerge = CellMerge.Start;
                            cell.Colspan = colspan;
                        }
                        while (colspan > 1)
                        {
                            WTableCell cell2 = cell.OwnerRow.AddCell(false);
                            cell2.CellFormat.ImportContainer(cell.CellFormat);
                            cell2.CellFormat.HorizontalMerge = CellMerge.Continue;
                            spanHelper.AddColumn(cell.OwnerRow.Cells.Count);
                            colspan--;
                        }
                        break;
                    case "rowspan":
                        rowspan = Convert.ToInt32(attr.Value);
                        if (rowspan != 1)
                        {
                            cell.CellFormat.VerticalMerge = CellMerge.Start;
                            spanHelper.AddRowSpan(rowspan);
                        }
                        break;
                }
            }

            spanHelper.UpdateTableGrid(cell, ClientWidth);
            spanHelper.NextColumn();
        }
        /// <summary>
        /// Parses the cell style.
        /// </summary>
        /// <param name="attr">The attr.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="brdrs">The borders.</param>
        private void ParseCellStyle(XmlAttribute attr, WTableCell cell, TableBorders brdrs,XmlNode node)
        {
            if (attr.Name.ToLower() != "style")
                return;
            TextFormat textFormat = AddStyle();
            textFormat.Borders = new TableBorders(null);
            CellFormat format = cell.CellFormat;
            string[] styleParams = attr.Value.Split(';', ':');
            char[] space = new char[] { ' ' };

            for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
            {
                string paramName = styleParams[i].ToLower().Trim();
                string paramValue = styleParams[i + 1].ToLower().Trim();
                if (paramValue.ToLower().Contains("inherit"))
                    continue;
                try
                {
                    brdrs.Parse(paramName, paramValue);

                    switch (paramName)
                    {
                        case "background-color":
                            format.BackColor = GetColor(paramValue);
                            if (paramValue == "transparent")
                                format.BackColor = Color.Empty;
                            break;
                        case "width":
                            if (paramValue.ToLower() == "auto")
                                cell.CellFormat.PreferredWidth.WidthType = FtsWidth.Auto;
                            else if (paramValue.EndsWith("%"))
                            {
                                cell.CellFormat.PreferredWidth.WidthType = FtsWidth.Percentage;
                                cell.CellFormat.PreferredWidth.Width = Convert.ToSingle(paramValue.Replace("%", string.Empty));
                                cell.Width = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                cell.CellFormat.PreferredWidth.WidthType = FtsWidth.Point;
                                cell.Width = cell.CellFormat.PreferredWidth.Width = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            }
                            break;
                        case "valign":
                        case "vertical-align":
                            cell.CellFormat.VerticalAlignment = GetVerticalAlignment(paramValue);
                            break;
                        case "display":
                            if (paramValue.ToLower() == "none")
                                cell.CellFormat.Hidden = true;
                            else
                                cell.CellFormat.Hidden = false;
                            break;
                        case "text-align":
                            m_bIsAlignAttriDefinedInCellNode = true;
                            m_horizontalAlignmentDefinedInCellNode = GetHorizontalAlignment(paramValue);
                            break;
                        case "border-bottom":
                            ParseBorder(paramValue, format.Borders.Bottom.Color, format.Borders.Bottom.LineWidth, format.Borders.Bottom.BorderType);
                            break;
                        case "border-top":
                            ParseBorder(paramValue, format.Borders.Top.Color, format.Borders.Top.LineWidth, format.Borders.Top.BorderType);
                            break;
                        case "border-left":
                            ParseBorder(paramValue, format.Borders.Left.Color, format.Borders.Left.LineWidth, format.Borders.Left.BorderType);
                            break;
                        case "border-right":
                            ParseBorder(paramValue, format.Borders.Right.Color, format.Borders.Right.LineWidth, format.Borders.Right.BorderType);
                            break;
                        case "border-color":
                            format.Borders.Color = GetColor(paramValue);
                            break;
                        case "border-left-color":
                            format.Borders.Left.Color = GetColor(paramValue);
                            break;
                        case "border-right-color":
                            format.Borders.Right.Color = GetColor(paramValue);
                            break;
                        case "border-top-color":
                            format.Borders.Top.Color = GetColor(paramValue);
                            break;
                        case "border-bottom-color":
                            format.Borders.Bottom.Color = GetColor(paramValue);
                            break;
                        case "border-width":
                            format.Borders.LineWidth = CalculateBorderWidth(paramValue);
                            break;
                        case "border-left-width":
                            format.Borders.Left.LineWidth = CalculateBorderWidth(paramValue);
                            break;
                        case "border-right-width":
                            format.Borders.Right.LineWidth = CalculateBorderWidth(paramValue);
                            break;
                        case "border-top-width":
                            format.Borders.Top.LineWidth = CalculateBorderWidth(paramValue);
                            break;
                        case "border-bottom-width":
                            format.Borders.Bottom.LineWidth = CalculateBorderWidth(paramValue);
                            break;
                        case "border-style":
                            format.Borders.BorderType = ToBorderType(paramValue);
                            break;
                        case "border-left-style":
                            format.Borders.Left.BorderType = ToBorderType(paramValue);
                            break;
                        case "border-right-style":
                            format.Borders.Right.BorderType = ToBorderType(paramValue);
                            break;
                        case "border-top-style":
                            format.Borders.Top.BorderType = ToBorderType(paramValue);
                            break;
                        case "border-bottom-style":
                            format.Borders.Bottom.BorderType = ToBorderType(paramValue);
                            break;
                        case "border":
                            ParseBorder(paramValue, format.Borders.Bottom.Color, format.Borders.Bottom.LineWidth, format.Borders.Bottom.BorderType);
                            ParseBorder(paramValue, format.Borders.Top.Color, format.Borders.Top.LineWidth, format.Borders.Top.BorderType);
                            ParseBorder(paramValue, format.Borders.Left.Color, format.Borders.Left.LineWidth, format.Borders.Left.BorderType);
                            ParseBorder(paramValue, format.Borders.Right.Color, format.Borders.Right.LineWidth, format.Borders.Right.BorderType);
                            break;
                        case "border-collapse":
                            if (paramValue.ToLower() == "collapse")
                                m_bIsBorderCollapse = true;
                            break;
                        case "padding":
                            string[] value = paramValue.Split(space);
                            int counts = value.Length;
                            switch (counts)
                            {
                                case 1:
                                    format.Paddings.All = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                    break;
                                case 2:
                                    format.Paddings.Top = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                    format.Paddings.Right = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                    break;
                                case 3:
                                    format.Paddings.Top = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                    format.Paddings.Right = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                    format.Paddings.Bottom = Convert.ToSingle(ExtractValue(value[2]), CultureInfo.InvariantCulture);
                                    break;
                                case 4:
                                    format.Paddings.Top = Convert.ToSingle(ExtractValue(value[0]), CultureInfo.InvariantCulture);
                                    format.Paddings.Right = Convert.ToSingle(ExtractValue(value[1]), CultureInfo.InvariantCulture);
                                    format.Paddings.Bottom = Convert.ToSingle(ExtractValue(value[2]), CultureInfo.InvariantCulture);
                                    format.Paddings.Left = Convert.ToSingle(ExtractValue(value[3]), CultureInfo.InvariantCulture);
                                    break;
                            }
                            break;
                        case "padding-left":
                            format.Paddings.Left = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            break;
                        case "padding-right":
                            format.Paddings.Right = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            break;
                        case "padding-top":
                            format.Paddings.Top = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            break;
                        case "padding-bottom":
                            format.Paddings.Bottom = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            break;
                        case "height":
                            cell.OwnerRow.Height = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            break;
                        default:
                            GetFormat(textFormat, paramName, paramValue, node);
                            break;
                    }
                }
                //To avoid the FormatException, we need to continue the process while the style Attribute is have Unknown attribute value
                catch
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// Parses the row attrs.
        /// </summary>
        /// <param name="rowNode">The row node.</param>
        /// <param name="row">The row.</param>
        private void ParseRowAttrs(XmlNode rowNode, WTableRow row)
        {
            foreach (XmlAttribute attr in rowNode.Attributes)
            {
                switch (attr.Name.ToLower())
                {
                    case "height":
                        if (attr.Value != "auto")
                            row.Height = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        break;
                    case "align":
                        m_bIsAlignAttrDefinedInRowNode = true;
                        m_horizontalAlignmentDefinedInRowNode = GetHorizontalAlignment(attr .Value);
                        break;
                    case "style":
                        ParseRowStyle(attr, row);
                        break;
                    case "valign":
                    case "vertical-align":
                        m_bIsVAlignAttriDefinedInRowNode = true;
                        m_verticalAlignmentDefinedInRowNode = GetVerticalAlignment(attr.Value.ToLower());
                        break;
                    case "bgcolor":
                        row.RowFormat.BackColor = GetColor(attr.Value);
                        break;
                    case "border-collapse":
                        if (attr.Value.ToLower() == "collapse")
                            m_bIsBorderCollapse = true;
                        break;
                }
            }
            row.HeightType = TableRowHeightType.AtLeast;
        }
        /// <summary>
        /// Update Hidden property of the row based on parent node
        /// </summary>
        /// <param name="rowNode"></param>
        /// <param name="row"></param>
        private void UpdateHiddenPropertyBasedOnParentNode(XmlNode rowNode, WTableRow row)
        {
            if (!row.RowFormat.HasKey(RowFormat.HiddenKey))
            {
                WTableRow headerRow = new WTableRow(m_currTable.Document);
                XmlNode parentNode = rowNode.ParentNode;
                while (parentNode.LocalName == "thead" || parentNode.LocalName == "tbody" || parentNode.LocalName == "tfoot")
                {
                    ParseRowAttrs(parentNode, headerRow);
                    if (headerRow.RowFormat.HasKey(RowFormat.HiddenKey))
                    {
                        row.RowFormat.Hidden = headerRow.RowFormat.Hidden;
                        break;
                    }
                    else
                        parentNode = parentNode.ParentNode;
                }
            }
        }
        /// <summary>
        /// Parses the table format.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="brdrs">The borders.</param>
        private void ParseTableAttrs(XmlNode node,SpanHelper spanHelper , TableBorders brdrs)
        {
            if (m_currTable == null)
                return;
            RowFormat format = m_currTable.TableFormat;
            format.CellSpacing = PointsConverter.FromCm(0.05f) / 2;
            format.Paddings.All = PointsConverter.FromCm(0.03f);

            foreach (XmlAttribute attr in node.Attributes)
            {
                switch (attr.Name.ToLower())
                {
                    case "border":
                        brdrs.AllWidth = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        if (brdrs.AllWidth == 0.0f)
                        {
                            brdrs.AllWidth = -1;
                            brdrs.AllStyle = BorderStyle.None;
                            brdrs.AllColor = Color.Empty;
                        }
                        break;
                    case "bordercolor":
                        brdrs.AllColor = GetColor(attr.Value);
                        break;
                    case "border-collapse":
                        if (attr.Value.ToLower() == "collapse")
                            m_bIsBorderCollapse = true;
                        break;
                    case "cellpadding":
                        float padding = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        format.Paddings.All = PointsConverter.FromPixel(padding);
                        break;
                    case "cellspacing":
                        float spacing = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                        format.CellSpacing = PointsConverter.FromPixel(spacing) / 2;
                        break;
                    case "title":
                        m_currTable.Title = attr.Value;
                        break;
                     case "style":
                        ParseTableStyle(attr, brdrs, spanHelper);
                        break;
                    case "background":
                    case "background-color":
                    case "bgcolor":
                        format.BackColor = GetColor(attr.Value);
                        if (attr.Value == "transparent")
                            format.BackColor = Color.Empty;
                        break;
                    case "align":
                        switch (attr.Value)
                        {
                            case "center":
                                m_currTable.TableFormat.HorizontalAlignment = RowAlignment.Center;
                                break;
                            case "right":
                                m_currTable.TableFormat.HorizontalAlignment = RowAlignment.Right;
                                break;
                            default:
                                m_currTable.TableFormat.HorizontalAlignment = RowAlignment.Left;
                                break;
                        }
                        break;
                    case "width":
                        if (attr.Value.ToLower() == "auto")
                            m_currTable.PreferredTableWidth.WidthType = FtsWidth.Auto;
                        else if (attr.Value.EndsWith("%"))
                        {
                            m_currTable.PreferredTableWidth.WidthType = FtsWidth.Percentage;
                            m_currTable.PreferredTableWidth.Width = Convert.ToSingle(attr.Value.Replace("%", string.Empty), CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            m_currTable.PreferredTableWidth.WidthType = FtsWidth.Point;
                            spanHelper.m_tableWidth = Convert.ToSingle(ExtractValue(attr.Value), CultureInfo.InvariantCulture);
                            m_currTable.PreferredTableWidth.Width = spanHelper.m_tableWidth;
                        }
                        break;
                }
            }

            brdrs.Apply(format);
        }
        /// <summary>
        /// Parses the table style.
        /// </summary>
        /// <param name="attr">The attr.</param>
        /// <param name="brdrs">The borders.</param>
        private void ParseTableStyle(XmlAttribute attr, TableBorders brdrs, SpanHelper spanHelper)
        {
            if (attr.Name.ToLower() != "style")
                return;

            RowFormat format = m_currTable.TableFormat;
            string[] styleParams = attr.Value.Split(';', ':');

            for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
            {
                string paramName = styleParams[i].ToLower().Trim();
                string paramValue = styleParams[i + 1].ToLower().Trim();
                try
                {
                    switch (paramName)
                    {
                        case "display":
                            if (paramValue.ToLower() == "none")
                                format.Hidden = true;
                            else
                                format.Hidden = false;
                            break;
                        case "background":
                        case "background-color":
                        case "bgcolor":
                            format.BackColor = GetColor(paramValue);
                            if (paramValue == "transparent")
                                format.BackColor = Color.Empty;
                            break;
                        case "border-collapse":
                            if (paramValue.ToLower() == "collapse")
                                m_bIsBorderCollapse = true;
                            break;
                        case "width":
                            if (paramValue.ToLower() == "auto")
                                m_currTable.PreferredTableWidth.WidthType = FtsWidth.Auto;
                            else if (paramValue.EndsWith("%"))
                            {
                                m_currTable.PreferredTableWidth.WidthType = FtsWidth.Percentage;
                                m_currTable.PreferredTableWidth.Width = Convert.ToSingle(paramValue.Replace("%", string.Empty), CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                m_currTable.PreferredTableWidth.WidthType = FtsWidth.Point;
                                spanHelper.m_tableWidth = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                                m_currTable.PreferredTableWidth.Width = spanHelper.m_tableWidth;
                            }
                            break;
                        default:
                            brdrs.Parse(paramName, paramValue);
                            break;
                    }
                }
                //To avoid the FormatException, we need to continue the process while the style Attribute is have Unknown attribute value
                catch
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// Parses the Row style.
        /// </summary>
        /// <param name="attr">The attr.</param>
        /// <param name="brdrs">The borders.</param>
        private void ParseRowStyle(XmlAttribute attr, WTableRow row)
        {
            if (attr.Name.ToLower() != "style")
                return;

            RowFormat format = row.RowFormat;
            string[] styleParams = attr.Value.Split(';', ':');

            for (int i = 0, len = styleParams.Length; i < len - 1; i += 2)
            {
                string paramName = styleParams[i].ToLower().Trim();
                string paramValue = styleParams[i + 1].ToLower().Trim();
                try
                {
                    switch (paramName)
                    {
                        case "display":
                            if (paramValue.ToLower() == "none")
                                format.Hidden = true;
                            else
                                format.Hidden = false;
                            break;
                        case "height":
                            if (attr.Value != "auto")
                                row.Height = Convert.ToSingle(ExtractValue(paramValue), CultureInfo.InvariantCulture);
                            break;
                    }
                }
                //To avoid the FormatException, we need to continue the process while the style Attribute is have Unknown attribute value
                catch
                {
                    continue;
                }

            }
        }
      /// <summary>
      /// Apply table border
      /// </summary>
      /// <param name="paramName"></param>
      /// <param name="paramValue"></param>
      /// <param name="border"></param>
        private void ApplyTableBorder(string paramName,string paramValue,Border border)
        {
            string[] borderStyle = { "dashed", "dotted", "double", "groove", "inset", "outset", "ridge", "solid", "hidden", "none"};
            char[] space = new char[] { ' ' };
            string[] value;
            int red, green, blue;
            value = paramValue.Split(space);
            for (int j = 0; j < value.Length; j++)
            {
                if (value[j].StartsWith("#"))
                {
                    value[j] = value[j].Replace("#", string.Empty);
                    red = int.Parse(value[j].Substring(0, 2), NumberStyles.AllowHexSpecifier);
                    green = int.Parse(value[j].Substring(2, 2), NumberStyles.AllowHexSpecifier);
                    blue = int.Parse(value[j].Substring(4, 2), NumberStyles.AllowHexSpecifier);
                   border .Color  = Color.FromArgb(red, green, blue);
                }
                else if (IsBorderWidth (value [j]))
                {

                    border.LineWidth = CalculateBorderWidth(value [j]);
                }
                else
                {
                    foreach (string styles in borderStyle)
                    {
                        if (value[j] == styles)
                            m_bBorderStyle = true;
                    }
                    if (m_bBorderStyle)
                    {
                       border .BorderType  = ToBorderType(value[j]);
                        m_bBorderStyle = false;
                    }
                    else
                        border.Color = GetColor(value[j]);
                }
            }
        }
        /// <summary>
        /// Toes the points.
        /// </summary>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        private float ToPoints(string val)
        {
            if (val.EndsWith("px"))
            {
                val = val.Substring(0, val.Length - 2);
            }

            float px = Convert.ToSingle(val);
            return PointsConverter.FromPixel(px);
        }
        /// <summary>
        /// Gets the vertical alignment
        /// </summary>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        private VerticalAlignment GetVerticalAlignment(string val)
        {
            switch (val.ToLower())
            {
                case "top":
                    return VerticalAlignment.Top;
                case "middle":
                    return VerticalAlignment.Middle;
                case "bottom":
                    return VerticalAlignment.Bottom;
            }

            return VerticalAlignment.Top;
        }
        /// <summary>
        /// Get HorizontalAlignment
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private HorizontalAlignment GetHorizontalAlignment(string val)
        {
            switch (val.ToLower ())
            {
                case "center":
                    return HorizontalAlignment.Center;
                    break;
                case "right":
                    return HorizontalAlignment.Right;
                    break;
                case "justify":
                    return HorizontalAlignment.Justify;
                    break;
                default:
                    return HorizontalAlignment.Left;
                    break;
            }
        }
        #endregion

        #region Internal declaration
        internal  class TableGrid
        {
            private Stack<Dictionary<int, List<object>>> m_tblGridStack = new Stack<Dictionary<int, List<object>>>();
            
            ///// <summary>
            ///// Gets/Sets the stack of Table Grid collection
            ///// </summary>
            internal Stack<Dictionary<int, List<object>>> TableGridStack
            {
                get
                {
                    return m_tblGridStack;
                }
                set
                {
                    m_tblGridStack = value;
                }
            }
          

        }
        /// <summary>
        /// 
        /// </summary>
        internal class SpanHelper
        {
            #region Fields
            int m_curCol = 0;
            internal List<object> m_tblGrid = new List<object>();
            Queue<object> m_rowspans = new Queue<object>();
            internal float m_parentWidth = 0.0f;
            internal float m_tableWidth = 0.0f;
            List<object> emptyCellIndex = new List<object>();
            private Dictionary<int, List<object>> m_tblGridCollection = new Dictionary<int, List<object>>();       
           
            #endregion

            #region Properties
            /// <summary>
            /// Gets/Sets the collection of row grid for a table
            /// </summary>
            internal Dictionary<int, List<object>> TableGridCollection
            {
                get
                {
                    return m_tblGridCollection;
                }
                set
                {
                    m_tblGridCollection = value;
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Resets the curr column.
            /// </summary>
            internal void ResetCurrColumn()
            {
                m_curCol = 0;
            }
            /// <summary>
            /// Updates the table grid.
            /// </summary>
            /// <param name="cell">The cell.</param>
            internal void UpdateTableGrid(WTableCell cell,float clientWidth)
            {
                float cellWidth = cell.Width/cell.Colspan;
                float width = 0f;
                
               
                if (cellWidth == 0)
                {
                    m_tblGrid = new List<object>();
                    m_tblGrid.Clear();
                    WTableRow row = cell.OwnerRow as WTableRow;

                    float TableWidth = (m_tableWidth != 0f) ? m_tableWidth : clientWidth;
                    int count = row.Cells.Count;
                    foreach (WTableCell c in row.Cells)
                        count += c.Colspan - 1;
                    
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (row.Cells[i].Width == 0.0f)
                        {
                            width = (TableWidth / count);
                            if (i < row.Cells.Count)
                                m_tblGrid.Add(width);
                        }
                        else
                        {
                            m_tblGrid.Add(row.Cells[i].Width / row.Cells[i].Colspan);
                            TableWidth = TableWidth - (row.Cells[i].Width/row.Cells[i].Colspan);
                        }
                    }
                }
                if (cell.Width != 0.0f)
                {
                    cellWidth = cell.Width/cell.Colspan;
                }
                else
                {
                    cellWidth = width;
                }

               if (cellWidth == 0.0)
                    cellWidth = c_DefCellWidth;

                 // Update grid
                if (m_curCol + 1 > m_tblGrid.Count)
                {
                    m_tblGrid.Add(cellWidth);
                }
                else if ((float)m_tblGrid[m_curCol] < cellWidth)
                {
                    m_tblGrid[m_curCol] = cellWidth;
                }
                  
            }
            /// <summary>
            /// Nexts the column.
            /// </summary>
            internal void NextColumn()
            {
                m_curCol++;
            }
            /// <summary>
            /// Adds the rowspan.
            /// </summary>
            /// <param name="rowspan">The rowspan.</param>
            internal void AddRowSpan(int rowspan)
            {
                m_rowspans.Enqueue(rowspan);
            }
            /// <summary>
            /// Gets the table grid from table grid collection
            /// </summary>
            /// <param name="tableGridCollection"></param>
            /// <returns></returns>
            private List<object> GetTableGrid(Dictionary<int, List<object>> tableGridCollection)
            {
                int maxGridCount = 0;
                List<object> TableGrid = new List<object>();
               //Identifies the maximum Grid count
                for (int i = 0; i < TableGridCollection.Count; i++)
                {
                    if (maxGridCount < TableGridCollection[i].Count)
                        maxGridCount = TableGridCollection[i].Count;
                }

                for (int j = 0; j < maxGridCount; j++)
                {
                   float width =0;
                    for (int k = 0; k < tableGridCollection.Count; k++)
                    {
                        List<object> tempGrid = tableGridCollection[k];
                        if (j < tempGrid.Count)
                        {
                            if (width < (float)tempGrid[j])
                            {
                                width = (float)tempGrid[j];
                            }
                        }

                    }
                    TableGrid.Add(width);

                }
                return TableGrid;

           }
            /// <summary>
            /// Updates the table.
            /// </summary>
            /// <param name="table">The table.</param>
            internal void UpdateTable(WTable table, Stack<Dictionary<int, List<object>>> tableGridStack,float clientWidth)
            {
                m_tblGridCollection = tableGridStack.Pop();
                UpdateRowSpan(table);
                m_tblGrid = GetTableGrid(m_tblGridCollection);
                float tableWidth = 0.0f;

                tableWidth = (m_tableWidth != 0f) ? m_tableWidth : clientWidth;

                //int colscount;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    emptyCellIndex.Clear();
                    for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                    {
                        if (table.Rows[i].Cells[j].Width == 0.0 || table.Rows[i].Cells[j].Width == c_DefCellWidth && m_tblGrid.Count < j)
                        {
                            WTableCell tableCell = table.Rows[i].Cells[j];
                            if (IsEmptyCell(tableCell) && tableCell.OwnerRow.PreviousSibling == null && tableCell.OwnerRow.NextSibling == null)
                            {
                                emptyCellIndex.Add(j);
                            }
                            else
                                table.Rows[i].Cells[j].Width = (float)m_tblGrid[j];
                        }
                    }
                    UpdateEmptyCellWidth(emptyCellIndex, table.Rows[i], m_tblGrid);
                }
				//Update NestedTable cells width
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                    {
                        foreach (WTable childTable in table.Rows[i].Cells[j].Tables)
                        {
                            AdjustNestedTable(childTable, table.Rows[i].Cells[j].Width);
                        }

                    }
                }
                //Update table cell widths
                if (table.Rows.Count > 1)
                {
                    foreach (WTableRow row in table.Rows)
                    {
                        for (int k = 0, len = m_tblGrid.Count; k < len; k++)
                        {
                            if (row.Cells.Count < k + 1)
                            {
                                row.AddCell(false);
                                row.Cells[k].CellFormat.Borders.BorderType = BorderStyle.Cleared;    
                            }

                            WTableCell cell = row.Cells[k];

                            cell.Width = (float)m_tblGrid[k];

                            if (cell.Width == c_DefCellWidth)
                            {
                                //cell.Width = c_DefCellWidth;
                                cell.PreferredWidth.WidthType = FtsWidth.Auto;
                            }
                        }

                        //row.RowFormat.IsAutoResized = true;
                    }
                }
            }
           
            /// <summary>
            /// Update Row span
            /// </summary>
            /// <param name="table"></param>
            private void UpdateRowSpan(WTable table)
            {
                int Count = 0;
                // Update table rowspans
                foreach (WTableRow row in table.Rows)
                {
                    m_tblGrid = m_tblGridCollection[Count];
                    for (int i = 0, len = row.Cells.Count; i < len; i++)
                    {
                        WTableCell cell = row.Cells[i];

                        if (cell.CellFormat.VerticalMerge == CellMerge.Start)
                        {
                            int cellIndex = cell.GetCellIndex();
                            int rowIndex = cell.OwnerRow.GetRowIndex() + 1;
                            int rowspan = (int)m_rowspans.Dequeue();

                            for (int j = 1, lenj = rowspan; j < lenj; j++)
                            {
                                if (table.Rows.Count > rowIndex)
                                {
                                    WTableCell spanCell = (WTableCell)cell.Clone();
                                    spanCell.Items.Clear();

                                    //The following code adds additional empty cell between last cell and rowspan cell index if the row contain unordered cell count
                                    if (table.Rows[rowIndex].Cells.Count < cellIndex)
                                    {
                                        int additionalCellCount = cellIndex - table.Rows[rowIndex].Cells.Count;
                                        for (int k = 0; k < additionalCellCount; k++)
                                        {
                                            
                                            table.Rows[rowIndex].AddCell(false);
                                            table.Rows[rowIndex].Cells[table.Rows[rowIndex].Cells.Count - 1].CellFormat.Borders.BorderType = BorderStyle.Cleared;
                                            int additionalCellIndex = table.Rows[rowIndex].Cells.Count - 1;
                                            m_tblGridCollection[rowIndex].Insert(additionalCellIndex,m_tblGrid [additionalCellIndex]);
                                        }
                                    }
                                    table.Rows[rowIndex].Cells.Insert(cellIndex, spanCell);
                                    m_tblGridCollection[rowIndex].Insert(cellIndex, m_tblGrid[cellIndex]);
                                    spanCell.CellFormat.VerticalMerge = CellMerge.Continue;

                                    if (table.Rows[rowIndex].Cells.Count > m_tblGrid.Count)
                                    {
                                        m_tblGrid.Add(c_DefCellWidth);
                                    }
                                    rowIndex++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    Count++;
                }

            }
            /// <summary>
            /// Update Row span for Nested Table
            /// </summary>
            /// <param name="table"></param>
            internal void UpdateNestedTableRowSpan(WTable table)
            {
                int Count = 0;
                // Update table rowspans
                foreach (WTableRow row in table.Rows)
                {
                    for (int i = 0, len = row.Cells.Count; i < len; i++)
                    {
                        WTableCell cell = row.Cells[i];

                        if (cell.CellFormat.VerticalMerge == CellMerge.Start)
                        {
                            int cellIndex = cell.GetCellIndex();
                            int rowIndex = cell.OwnerRow.GetRowIndex() + 1;
                            int rowspan = (int)m_rowspans.Dequeue();

                            for (int j = 1, lenj = rowspan; j < lenj; j++)
                            {
                                if (table.Rows.Count > rowIndex)
                                {
                                    WTableCell spanCell = (WTableCell)cell.Clone();
                                    spanCell.Items.Clear();

                                    //The following code adds additional empty cell between last cell and rowspan cell index if the row contain unordered cell count
                                    if (table.Rows[rowIndex].Cells.Count < cellIndex)
                                    {
                                        int additionalCellCount = cellIndex - table.Rows[rowIndex].Cells.Count;
                                        for (int k = 0; k < additionalCellCount; k++)
                                        {

                                            table.Rows[rowIndex].AddCell(false);
                                            table.Rows[rowIndex].Cells[table.Rows[rowIndex].Cells.Count - 1].CellFormat.Borders.BorderType = BorderStyle.Cleared;
                                        }
                                    }
                                    table.Rows[rowIndex].Cells.Insert(cellIndex, spanCell);
                                    spanCell.CellFormat.VerticalMerge = CellMerge.Continue;
                                    rowIndex++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    Count++;
                }

            }
            /// <summary>
            /// Check whether the cell is an empty cell
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            private bool IsEmptyCell(WTableCell cell)
            {
                bool isEmpty=true;
                foreach (Entity ent in cell.ChildEntities)
                {
                    if (ent.EntityType == EntityType.Paragraph)
                    {
                        foreach (ParagraphItem item in (ent as WParagraph).Items)
                        {
                            if (item.EntityType == EntityType.TextRange)
                            {
                                if ((item as WTextRange).Text != "")
                                    return false;
                            }
                            else if (item.EntityType == EntityType.Break || item .EntityType ==EntityType.CommentMark || item.EntityType==EntityType .FieldMark   )
                                isEmpty = true;
                            else
                                isEmpty = false;                               
                        }
                    }
                    else if ((ent.EntityType == EntityType.Table))
                    {
                        return false;
                    }
                }
                return isEmpty;
            }
            /// <summary>
            /// Updating Empty cell width
            /// </summary>
            /// <param name="emptyCellIndex"></param>
            /// <param name="row"></param>
            /// <param name="m_tableGrid"></param>
            private void UpdateEmptyCellWidth(List<object> emptyCellIndex, WTableRow row, List<object> m_tableGrid)
            {
                float totalWidth = 0;
                foreach (int index in emptyCellIndex)
                {
                    totalWidth += (float)m_tblGrid[index];
                }
                int totalCell = row.Cells.Count;
                int cellWithContent = totalCell - emptyCellIndex.Count;
                float additionalWidth = totalWidth / cellWithContent;

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (!emptyCellIndex.Contains(i))
                        row.Cells[i].Width += additionalWidth;
                }

            }
            /// <summary>
            /// Adjust Nested table
            /// </summary>
            /// <param name="table"></param>
            /// <param name="width"></param>
            private void AdjustNestedTable(WTable table, float width)
            {
                int colscount = 0;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    int cellcount = table.Rows[i].Cells.Count;
                    if (colscount < cellcount)
                        colscount = cellcount;
                }
                float RevisedWidth = width / colscount;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                    {
                        if (table.Rows[i].Cells[j].Width == 0.0 || table.Rows[i].Cells[j].Width == c_DefCellWidth)
                        {
                            table.Rows[i].Cells[j].Width = (table.Rows[i].Cells[j].Colspan * RevisedWidth);
						//Updated Adjacent Rows cell width
                        if (i > 0 && table.Rows[i - 1].Cells.Count > j && table.Rows[i - 1].Cells[j].Width < table.Rows[i].Cells[j].Width)
                            table.Rows[i - 1].Cells[j].Width = table.Rows[i].Cells[j].Width;
                        }
						//Update NestedTable cells width
                        foreach (WTable childTable in table.Rows[i].Cells[j].Tables)
                        {
                            AdjustNestedTable(childTable, table.Rows[i].Cells[j].Width);
                        }
                    }
                }
                table.UpdateWidth();
            }
            /// <summary>
            /// Adds the column.
            /// </summary>
            internal void AddColumn()
            {
                m_tblGrid.Add(1f);
                NextColumn();
            }
            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="cellsCount">The cells count.</param>
            internal void AddColumn(int cellsCount)
            {
                if (m_tblGrid.Count < cellsCount)
                    AddColumn();
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class TextFormat
        {
            #region private members

            internal const short FontSizeKey = 0;
            internal const short FontFamilyKey = 1;
            internal const short BoldKey = 2;
            internal const short UnderlineKey = 3;
            internal const short ItalicKey = 4;
            internal const short StrikeKey = 5;
            internal const short FontColorKey = 6;
            internal const short BackColorKey = 7;
            internal const short LineHeightKey = 8;
            internal const short LineHeightNormalKey = 9;
            internal const short TextAlignKey = 10;
            internal const short TopMarginKey = 11;
            internal const short LeftMarginKey = 12;
            internal const short BottomMarginKey = 13;
            internal const short RightMarginKey = 14;
            internal const short TextIndentKey = 15;
            internal const short SubSuperScriptKey = 16;
            internal const short PageBreakBeforeKey = 17;
            internal const short PageBreakAfterKey = 18;
            internal const short CharacterSpacingKey = 19;
            internal const short AllCapsKey = 20;
            internal const short WhiteSpaceKey = 21;
            internal const short WordWrapKey = 22;
            internal const short HiddenKey = 23;
            private Dictionary<int, Object> m_propertiesHash;
            private LineSpacingRule m_lineSpacingRule = LineSpacingRule.AtLeast;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether to break lines on word or character level. By default line breaks on word level.
            /// </summary>
            /// <value>
            ///   <c>true</c> if line breaks on word level; otherwise, <c>false</c>.
            /// </value>
            public bool WordWrap
            {
                get
                {
                    if (HasKey(WordWrapKey))
                        return (bool)m_propertiesHash[WordWrapKey];
                    return true;
                }
                set
                {
                    SetPropertyValue(WordWrapKey, value);
                }
            }
            /// <summary>
            /// Gets/ Sets whether white space need to be preserved
            /// </summary>
            public bool IsPreserveWhiteSpace
            {
                get
                {
                    if (HasKey(WhiteSpaceKey))
                        return (bool)m_propertiesHash[WhiteSpaceKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(WhiteSpaceKey, value);
                }
            }
            /// <summary>
            /// Gets/ Sets Hidden property of text
            /// </summary>
            internal bool Hidden
            {
                get
                {
                    if (HasKey(HiddenKey))
                        return (bool)m_propertiesHash[HiddenKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(HiddenKey, value);
                }
            }
            /// <summary>
            /// Gets/ Sets All Caps
            /// </summary>
            public bool AllCaps
            {
                get
                {
                    if (HasKey(AllCapsKey))
                        return (bool)m_propertiesHash[AllCapsKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(AllCapsKey, value);
                }
            }
            /// <summary>
            /// Gets/Sets visited Link color
            /// </summary>
            public float CharacterSpacing
            {
                get
                {
                    if (HasKey(CharacterSpacingKey))
                        return (float)m_propertiesHash[CharacterSpacingKey];
                    return 0.0f;
                }
                set
                {
                    SetPropertyValue(CharacterSpacingKey, value);
                }

            }

            /// <summary>
            /// Gets/Sets PageBreakBefore
            /// </summary>
            public bool PageBreakBefore
            {
                get
                {
                    if (HasKey(PageBreakBeforeKey))
                        return (bool)m_propertiesHash[PageBreakBeforeKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(PageBreakBeforeKey, value);
                }
            }
            /// <summary>
            /// Gets/Sets PageBreakAfter
            /// </summary>
            public bool PageBreakAfter
            {
                get
                {
                    if (HasKey(PageBreakAfterKey))
                        return (bool)m_propertiesHash[PageBreakAfterKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(PageBreakAfterKey, value);
                }
            }
            /// <summary>
            /// Gets/Set Linespacing rule
            /// </summary>
            public LineSpacingRule LineSpacingRule
            {
                get
                {
                    return m_lineSpacingRule;
                }
                set
                {
                    m_lineSpacingRule = value;
                }
            }
            public bool NumBulleted = false;
            public bool DefBulleted = false;
            /// <summary>
            /// Specifies Bold format.
            /// </summary>
            public bool Bold
            {
                get
                {
                    if (HasKey(BoldKey))
                        return (bool) m_propertiesHash[BoldKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(BoldKey, value);
                }
            }
            /// <summary>
            /// Specifies Italic format.
            /// </summary>
            public bool Italic
            {
                get
                {
                    if (HasKey(ItalicKey))
                        return (bool)m_propertiesHash[ItalicKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(ItalicKey, value);
                }
            }
            /// <summary>
            /// Specifies Underline format.
            /// </summary>
            public bool Underline
            {
                get
                {
                    if (HasKey(UnderlineKey))
                        return (bool)m_propertiesHash[UnderlineKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(UnderlineKey, value);
                }
            }
            /// <summary>
            /// Specifies Strike format.
            /// </summary>
            public bool Strike
            {
                get
                {
                    if (HasKey(StrikeKey))
                        return (bool)m_propertiesHash[StrikeKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(StrikeKey, value);
                }
            }
            /// <summary>
            /// Specifies font color of the text.
            /// </summary>
            public Color FontColor
            {
                get
                {
                    if (HasKey(FontColorKey))
                        return (Color)m_propertiesHash[FontColorKey];
                    return Color.Empty;
                }
                set
                {
                    SetPropertyValue(FontColorKey, value);
                }
            }
            /// <summary>
            /// Specifies back color of the text.
            /// </summary>
            public Color BackColor
            {
                get
                {
                    if (HasKey(BackColorKey))
                        return (Color)m_propertiesHash[BackColorKey];
                    return Color.Empty;
                }
                set
                {
                    SetPropertyValue(BackColorKey, value);
                }
            }
            /// <summary>
            /// Specifies the font family.
            /// </summary>
            public string FontFamily
            {
                get
                {
                    if (HasKey(FontFamilyKey))
                        return (string)m_propertiesHash[FontFamilyKey];
                    return string.Empty;
                }
                set
                {
                    SetPropertyValue(FontFamilyKey, value);
                }
            }

            /// <summary>
            /// Specifies the font size.
            /// </summary>
            public float FontSize
            {
                get
                {
                    if (HasKey(FontSizeKey))
                        return (float)m_propertiesHash[FontSizeKey];
                    return 12f;
                }
                set
                {
                    SetPropertyValue(FontSizeKey, value);
                }
            }
            /// <summary>
            /// Specifies the line height.
            /// </summary>
            public float LineHeight
            {
                get
                {
                    if (HasKey(LineHeightKey))
                        return (float)m_propertiesHash[LineHeightKey];
                    return -1f;
                }
                set
                {
                    SetPropertyValue(LineHeightKey, value);
                }
            }
            /// <summary>
            /// Specifies whether the line height is Normal or not
            /// </summary>
            public bool IsLineHeightNormal
            {
                get
                {
                    if (HasKey(LineHeightNormalKey))
                        return (bool)m_propertiesHash[LineHeightNormalKey];
                    return false;
                }
                set
                {
                    SetPropertyValue(LineHeightNormalKey, value);
                }
            }
            /// <summary>
            /// Specifies the text alignment.
            /// </summary>
            public HorizontalAlignment TextAlign
            {
                get
                {
                    if (HasKey(TextAlignKey))
                        return (HorizontalAlignment)m_propertiesHash[TextAlignKey];
                    return HorizontalAlignment.Left;
                }
                set
                {
                    SetPropertyValue(TextAlignKey, value);
                }
            }
            /// <summary>
            /// Specifies the text style.
            /// </summary>
            public BuiltinStyle Style = BuiltinStyle.Normal;
            /// <summary>
            /// Specifies the left margin.
            /// </summary>
            public float LeftMargin
            {
                get
                {
                    if (HasKey(LeftMarginKey))
                        return (float)m_propertiesHash[LeftMarginKey];
                    return 0.0f;
                }
                set
                {
                    SetPropertyValue(LeftMarginKey, value);
                }
            }
            /// <summary>
            /// Specifies the Text indent.
            /// </summary>
            public float TextIndent
            {
                get
                {
                    if (HasKey(TextIndentKey))
                        return (float)m_propertiesHash[TextIndentKey];
                    return 0.0f;
                }
                set
                {
                    SetPropertyValue(TextIndentKey, value);
                }
            }
            /// <summary>
            /// Specifies the Right margin
            /// </summary>
            public float RightMargin
            {
                get
                {
                    if (HasKey(RightMarginKey))
                        return (float)m_propertiesHash[RightMarginKey];
                    return 0.0f;
                }
                set
                {
                    SetPropertyValue(RightMarginKey, value);
                }
            }
            /// <summary>
            /// Specifies the Top margin
            /// </summary>
            public float TopMargin
            {
                get
                {
                    if (HasKey(TopMarginKey))
                        return (float)m_propertiesHash[TopMarginKey];
                    return 0.0f;
                }
                set
                {
                    SetPropertyValue(TopMarginKey, value);
                }
            }
            /// <summary>
            /// Specifies the Bottom margin
            /// </summary>
            public float BottomMargin
            {
                get
                {
                    if (HasKey(BottomMarginKey))
                        return (float)m_propertiesHash[BottomMarginKey];
                    return -1.0f;
                }
                set
                {
                    SetPropertyValue(BottomMarginKey, value);
                }
            }
            /// <summary>
            /// Specifies the Borders
            /// </summary>
            public TableBorders Borders = new TableBorders(null);
            /// <summary>
            /// Specifies Superscript/SubScript
            /// </summary>
            public SubSuperScript SubSuperScript
            {
                get
                {
                    if (HasKey(SubSuperScriptKey))
                        return (DLS.SubSuperScript)m_propertiesHash[SubSuperScriptKey];
                    return DLS.SubSuperScript.None;
                }
                set
                {
                    SetPropertyValue(SubSuperScriptKey, value);
                }
            }
            #endregion

            /// <summary>
            /// Initialize the formatting properties
            /// </summary>
            internal TextFormat()
            {
                m_propertiesHash = new Dictionary<int, object>();
            }

            #region Methods
            /// <summary>
            /// Clones this instance.
            /// </summary>
            /// <returns></returns>
            public TextFormat Clone()
            {
                TextFormat textformat = new TextFormat();
                textformat.m_propertiesHash = new Dictionary<int, object>(m_propertiesHash);
                textformat.NumBulleted = this.NumBulleted;
                textformat.DefBulleted = this.DefBulleted;
                textformat.Borders = this.Borders;
                textformat.LineSpacingRule = this.LineSpacingRule;
                return textformat; // (TextFormat)MemberwiseClone();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Key"></param>
            /// <param name="value"></param>
            private void SetPropertyValue(int Key, bool value)
            {
                if (!m_propertiesHash.ContainsKey(Key))
                    m_propertiesHash.Add(Key, value);
                else
                    m_propertiesHash[Key] = value;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Key"></param>
            /// <returns></returns>
            internal bool HasKey(int Key)
            {
                if (m_propertiesHash.ContainsKey(Key))
                    return true;
                return false;
            }
            /// <summary>
            ///  Determines whether the specified property key has value.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            internal bool HasValue(int key)
            {
                if (m_propertiesHash.ContainsKey(key))
                    return true;
                return false;
            }
            /// <summary>
            /// Set the values for the properties
            /// </summary>
            /// <param name="Key"></param>
            /// <param name="value"></param>
            private void SetPropertyValue(int Key, object value)
            {
                if (!m_propertiesHash.ContainsKey(Key))
                    m_propertiesHash.Add(Key, value);
                else
                    m_propertiesHash[Key] = value;
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class TableBorders
        {
            #region Properties
            /// <summary>
            /// Variable specifies the color value.
            /// </summary>
            public Color AllColor = Color.Empty;
            /// <summary>
            /// Variable specifies the width value.
            /// </summary>
            public float AllWidth = -1f;
            /// <summary>
            /// Variable specifies the style value.
            /// </summary>
            public BorderStyle AllStyle = BorderStyle.None;
            /// <summary>
            /// Variable specifies the Top color value.
            /// </summary>
            public Color TopColor = Color.Empty;
            /// <summary>
            /// Variable specifies the bottom color.
            /// </summary>
            public Color BottomColor = Color.Empty;
            /// <summary>
            /// Variable specifies the left color value.
            /// </summary>
            public Color LeftColor = Color.Empty;
            /// <summary>
            /// Variable specifies the right color value.
            /// </summary>
            public Color RightColor = Color.Empty;
            /// <summary>
            /// Variable specifies the TopStyle.
            /// </summary>
            public BorderStyle TopStyle = BorderStyle.None;
            /// <summary>
            /// Variable specifies the BottomStyle.
            /// </summary>
            public BorderStyle BottomStyle = BorderStyle.None;
            /// <summary>
            ///  Variable specifies the LeftStyle.
            /// </summary>
            public BorderStyle LeftStyle = BorderStyle.None;
            /// <summary>
            /// Variable specifies the RightStyle.
            /// </summary>
            public BorderStyle RightStyle = BorderStyle.None;
            /// <summary>
            /// Variable specifies the TopWidth.
            /// </summary>
            public float TopWidth = -1f;
            /// <summary>
            /// Variable specifies BottomWidth. 
            /// </summary>
            public float BottomWidth = -1f;
            /// <summary>
            ///  Variable specifies LeftWidth. 
            /// </summary>
            public float LeftWidth = -1f;
            /// <summary>
            /// Variable specifies RightWidth.
            /// </summary>
            public float RightWidth = -1f;
            /// <summary>
            /// Variable specifies the TableBorder.
            /// </summary>
            public TableBorders ParentBorders = null;
            private bool m_bBorderStyle = false;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="TableBorders"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public TableBorders(TableBorders parent)
            {
                ParentBorders = parent;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Parses the specified param name.
            /// </summary>
            /// <param name="paramName">Name of the param.</param>
            /// <param name="paramValue">The param value.</param>
            internal void Parse(string paramName, string paramValue)
            {
                HTMLConverterImpl htmlConverter = new HTMLConverterImpl();
                switch (paramName)
                {
                    case "border-color":
                        AllColor = htmlConverter.GetColor(paramValue);
                        break;
                    case "border-left-color":
                        LeftColor = htmlConverter.GetColor(paramValue);
                        break;
                    case "border-right-color":
                        RightColor = htmlConverter.GetColor(paramValue);
                        break;
                    case "border-top-color":
                        TopColor = htmlConverter.GetColor(paramValue);
                        break;
                    case "border-bottom-color":
                        BottomColor = htmlConverter.GetColor(paramValue);
                        break;
                    case "border-width":
                        AllWidth = htmlConverter.CalculateBorderWidth(paramValue );
                        break;
                    case "border-left-width":
                        LeftWidth = htmlConverter.CalculateBorderWidth(paramValue);
                        break;
                    case "border-right-width":
                        RightWidth = htmlConverter.CalculateBorderWidth(paramValue);
                        break;
                    case "border-top-width":
                        TopWidth = htmlConverter.CalculateBorderWidth(paramValue);
                        break;
                    case "border-bottom-width":
                        BottomWidth = htmlConverter.CalculateBorderWidth(paramValue);
                        break;
                    case "border-style":
                        AllStyle = ToBorderType(paramValue);
                        break;
                    case "border-left-style":
                        LeftStyle = ToBorderType(paramValue);
                        break;
                    case "border-right-style":
                        RightStyle = ToBorderType(paramValue);
                        break;
                    case "border-top-style":
                        TopStyle = ToBorderType(paramValue);
                        break;
                    case "border-bottom-style":
                        BottomStyle = ToBorderType(paramValue);
                        break;
                    case "border-bottom":
                        ParseTableBorder(paramValue, ref BottomColor, ref BottomWidth, ref BottomStyle, htmlConverter);
                        break;
                    case "border-top":
                        ParseTableBorder(paramValue, ref TopColor, ref TopWidth, ref TopStyle, htmlConverter);
                        break;
                    case "border-left":
                        ParseTableBorder(paramValue, ref LeftColor, ref LeftWidth, ref LeftStyle, htmlConverter);
                        break;
                    case "border-right":
                        ParseTableBorder(paramValue, ref RightColor, ref RightWidth, ref RightStyle, htmlConverter);
                        break;
                    case "border":
                        ParseTableBorder(paramValue, ref BottomColor, ref BottomWidth, ref BottomStyle, htmlConverter);
                        ParseTableBorder(paramValue, ref RightColor, ref RightWidth, ref RightStyle, htmlConverter);
                        ParseTableBorder(paramValue, ref TopColor, ref TopWidth, ref TopStyle, htmlConverter);
                        ParseTableBorder(paramValue, ref LeftColor, ref LeftWidth, ref LeftStyle, htmlConverter);
                        break;
                }
            }

            /// <summary>
            /// Parse border style
            /// </summary>
            /// <param name="paramValue"></param>
            /// <param name="Color"></param>
            /// <param name="Width"></param>
            /// <param name="Style"></param>
            /// <param name="htmlConverter"></param>
            private void ParseTableBorder(string paramValue, ref Color Color, ref float Width,ref BorderStyle  Style, HTMLConverterImpl htmlConverter)
            {
                string[] borderStyle = { "dashed", "dotted", "double", "groove", "inset", "outset", "ridge", "solid", "hidden", "none" };
                string[] value = paramValue.Split(' ');
                int red, green, blue;
                if (paramValue == "none" || paramValue == "medium none")
                {
                    Style = BorderStyle.None;
                    Color = Color.Empty;
                    Width = 0;
                    return;
                }
                for (int j = 0; j < value.Length; j++)
                {
                    if (value[j].StartsWith("#"))
                    {
                        value[j] = value[j].Replace("#", string.Empty);
                        red = int.Parse(value[j].Substring(0, 2), NumberStyles.AllowHexSpecifier);
                        green = int.Parse(value[j].Substring(2, 2), NumberStyles.AllowHexSpecifier);
                        blue = int.Parse(value[j].Substring(4, 2), NumberStyles.AllowHexSpecifier);
                        Color = Color.FromArgb(red, green, blue);
                    }
                    else if (htmlConverter .IsBorderWidth (value [j]))
                    {
                        Width = htmlConverter.CalculateBorderWidth(value [j]);
                    }
                    else
                    {
                        foreach (string styles in borderStyle)
                        {
                            if (value[j] == styles)
                                m_bBorderStyle = true;
                        }
                        if (m_bBorderStyle)
                        {
                            Style = ToBorderType(value[j]);
                            m_bBorderStyle = false;
                        }
                        else
                            Color =htmlConverter.GetColor(value[j]);
                    }                                     
                   
                }
            }
            /// <summary>
            /// Applies the specified format.
            /// </summary>
            /// <param name="format">The format.</param>
            internal void Apply(RowFormat format)
            {
                // Check default width
                if (AllWidth != -1)
                {
                    ApplyWidth(format.Borders, AllWidth);
                    ApplyStyle(format.Borders, BorderStyle.Outset);

                    if (AllColor == Color.Empty)
                    {
                        ApplyColor(format.Borders, Color.Silver);
                    }
                }
                else
                {
                    format.Borders.Left.HasNoneStyle = true;
                    format.Borders.Right.HasNoneStyle = true;
                    format.Borders.Top.HasNoneStyle = true;
                    format.Borders.Bottom.HasNoneStyle = true;
                    format.Borders.Vertical.HasNoneStyle = true;
                    format.Borders.Horizontal.HasNoneStyle = true;
                }

                // Check default style
                if (AllStyle != BorderStyle.None)
                {
                    ApplyStyle(format.Borders, AllStyle);
                    ApplyColor(format.Borders, Color.Silver);

                    if (AllWidth == -1)
                    {
                        ApplyWidth(format.Borders, 1f);
                    }
                }

                // Check default color
                if (AllColor != Color.Empty)
                {
                    ApplyColor(format.Borders, AllColor);
                }

                // Check top border
                ApplyOneBorder(format.Borders.Top, TopStyle, TopWidth, TopColor);
                // Check bottom border
                ApplyOneBorder(format.Borders.Bottom, BottomStyle, BottomWidth, BottomColor);
                // Check left border
                ApplyOneBorder(format.Borders.Left, LeftStyle, LeftWidth, LeftColor);
                // Check right border
                ApplyOneBorder(format.Borders.Right, RightStyle, RightWidth, RightColor);
            }
            /// <summary>
            /// Applies the specified format.
            /// </summary>
            /// <param name="format">The format.</param>
            internal void Apply(CellFormat format)
            {
                bool emptyBorder = true;

                // Check default width
                if (AllWidth != -1)
                {
                    emptyBorder = false;
                    ApplyWidth(format.Borders, AllWidth);
                    ApplyStyle(format.Borders, BorderStyle.Outset);

                    //if (AllColor == Color.Empty)
                    //{
                    //    ApplyColor(format.Borders, Color.Silver);
                    //}
                }

                // Check default style
                if (AllStyle != BorderStyle.None)
                {
                    emptyBorder = false;
                    ApplyStyle(format.Borders, AllStyle);
                    ApplyColor(format.Borders, Color.Silver);

                    if (AllWidth == -1)
                    {
                        ApplyWidth(format.Borders, 1f);
                    }
                }

                // Check default color
                if (AllColor != Color.Empty)
                {
                    ApplyColor(format.Borders, AllColor);
                }

                // Check top border
                ApplyOneBorder(format.Borders.Top, TopStyle, TopWidth, TopColor);
                // Check bottom border
                ApplyOneBorder(format.Borders.Bottom, BottomStyle, BottomWidth, BottomColor);
                // Check left border
                ApplyOneBorder(format.Borders.Left, LeftStyle, LeftWidth, LeftColor);
                // Check right border
                ApplyOneBorder(format.Borders.Right, RightStyle, RightWidth, RightColor);

                if (emptyBorder && ParentBorders != null)
                {
                    if (TopStyle == BorderStyle.None)
                    {
                        Color tc = GetNoEmptyColor(TopColor, ParentBorders.TopColor, ParentBorders.AllColor);
                        ApplyOneBorder(format.Borders.Top, BorderStyle.None, TopWidth, tc);
                    }
                    if (BottomStyle == BorderStyle.None)
                    {
                        Color bc = GetNoEmptyColor(BottomColor, ParentBorders.BottomColor, ParentBorders.AllColor);
                        ApplyOneBorder(format.Borders.Bottom, BorderStyle.None, BottomWidth, bc);
                    }
                    if (LeftStyle == BorderStyle.None)
                    {
                        Color lc = GetNoEmptyColor(LeftColor, ParentBorders.LeftColor, ParentBorders.AllColor);
                        ApplyOneBorder(format.Borders.Left, BorderStyle.None, LeftWidth, lc);
                    }
                    if (RightStyle == BorderStyle.None)
                    {
                        Color rc = GetNoEmptyColor(RightColor, ParentBorders.RightColor, ParentBorders.AllColor);
                        ApplyOneBorder(format.Borders.Right, BorderStyle.None, RightWidth, rc);
                    }
                }
            }
            /// <summary>
            /// Gets the empty color of the no.
            /// </summary>
            /// <param name="cl1">The CL1.</param>
            /// <param name="cl2">The CL2.</param>
            /// <param name="cl3">The CL3.</param>
            /// <returns></returns>
            private Color GetNoEmptyColor(Color cl1, Color cl2, Color cl3)
            {
                if (cl1 != Color.Empty)
                {
                    return cl1;
                }
                else if (cl2 != Color.Empty)
                {
                    return cl2;
                }
                else
                {
                    return cl3;
                }
            }
            /// <summary>
            /// Applies the one border.
            /// </summary>
            /// <param name="border">The border.</param>
            /// <param name="RightStyle">The right style.</param>
            /// <param name="RightWidth">Width of the right.</param>
            /// <param name="RightColor">Color of the right.</param>
            private void ApplyOneBorder(Border border, BorderStyle style, float width, Color color)
            {
                // Check width
                if (width != -1)
                {
                    border.LineWidth = width;
                    border.BorderType = style;

                    if (color == Color.Empty)
                    {
                        border.Color = Color.Silver;
                    }
                }

                // Check style
                if (style != BorderStyle.None)
                {
                    border.BorderType = style;
                    border.Color = Color.Silver;

                    if (width == -1)
                    {
                        border.LineWidth = 1f;
                    }
                }

                // Check color
                if (color != Color.Empty)
                {
                    border.Color = color;
                }
            }
            /// <summary>
            /// Applies the width.
            /// </summary>
            /// <param name="bs">The borders.</param>
            /// <param name="width">The width.</param>
            private void ApplyWidth(Borders bs, float width)
            {
                bs.LineWidth = width;
                bs.Horizontal.LineWidth = width;
                bs.Vertical.LineWidth = width;
            }
            /// <summary>
            /// Applies the color.
            /// </summary>
            /// <param name="bs">The bs.</param>
            /// <param name="cl">The cl.</param>
            private void ApplyColor(Borders bs, Color cl)
            {
                bs.Color = cl;
                bs.Horizontal.Color = cl;
                bs.Vertical.Color = cl;
            }
            /// <summary>
            /// Applies the style.
            /// </summary>
            /// <param name="bs">The bs.</param>
            /// <param name="style">The style.</param>
            private void ApplyStyle(Borders bs, BorderStyle style)
            {
                bs.BorderType = style;
                bs.Horizontal.BorderType = style;
                bs.Vertical.BorderType = style;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="paramValue"></param>
            /// <returns></returns>
            private BorderStyle ToBorderType(string type)
            {
                switch (type.ToLower())
                {
                    case "dashed":
                        return BorderStyle.DashLargeGap;
                    case "dotted":
                        return BorderStyle.Dot;
                    case "double":
                        return BorderStyle.Double;
                    case "groove":
                        return BorderStyle.Engrave3D;
                    case "inset":
                        return BorderStyle.Inset;
                    case "outset":
                        return BorderStyle.Outset;
                    case "ridge":
                        return BorderStyle.Emboss3D;
                    case "solid":
                        return BorderStyle.Single;
                    case "none":
                    case "hidden":
                        return BorderStyle.None;
                }

                return BorderStyle.None;
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal enum ThreeState
        {
            False = 0,
            True = 1,
            Unknown = 2
        }
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// Roman number to arabic number.
        /// </summary>
        /// <param name="numberStr">The number.</param>
        /// <returns></returns>
        private int RomanToArabic(string numberStr)
        {
            numberStr = numberStr.ToUpper();
            char[] romanNumber = numberStr.ToCharArray();
            int number = 0;
            int previousSum = 0;
            int sum = 0;

            for (int i = romanNumber.Length - 1; i >= 0; i--)
            {
                if (romanNumber[i] == 'M')
                    number = 1000;
                else if (romanNumber[i] == 'D')
                    number = 500;
                else if (romanNumber[i] == 'C')
                    number = 100;
                else if (romanNumber[i] == 'L')
                    number = 50;
                else if (romanNumber[i] == 'X')
                    number = 10;
                else if (romanNumber[i] == 'V')
                    number = 5;
                else if (romanNumber[i] == 'I')
                    number = 1;
                else
                    number = 0;

                if (previousSum > number)
                    sum = previousSum - number;
                else
                    sum = sum + number;

                previousSum = number;
            }

            return sum;
        }
        /// <summary>
        /// Inits this convertor.
        /// </summary>
        private void Init()
        {
            m_curListLevel = -1;
            m_listStack = null;
            m_listLeftIndentStack.Clear();
        }
        #endregion
    }
}

#endif