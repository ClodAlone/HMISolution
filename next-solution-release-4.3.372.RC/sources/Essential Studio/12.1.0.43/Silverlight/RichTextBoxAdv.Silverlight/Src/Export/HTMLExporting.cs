#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Xml;
using System.Reflection;

#if Silverlight
using System.Windows.Browser;
#endif

using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Globalization;


namespace Syncfusion.Windows.Tools.Controls
{
    public class HTMLExporting
    {

        #region Private Members
        internal static List<StringBuilder> SpanStyleBuilder = new List<StringBuilder>();
        internal static List<StringBuilder> ParagraphStyleBuilder = new List<StringBuilder>();
        internal static List<StringBuilder> TableStyleBuilder=new List<StringBuilder>();
        internal static List<StringBuilder> BlockStyleList;
        static Random randomNumber = new Random();
        private static HtmlAsciiCodesInfo htmlasciicode;

        #endregion

        #region Properties

        /// <summary>
        /// It stores the attributes of HTMLTag
        /// </summary>
        private static Dictionary<string, object> TagAttributes
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HTMLExporting"/> class.
        /// </summary>
        public HTMLExporting()
        {

        }

        static HTMLExporting()
        {
            TagAttributes = new Dictionary<string, object>();
            htmlasciicode = new HtmlAsciiCodesInfo();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Return the string from the DocumentAdv and Stream
        /// </summary>
        /// <param name="document"></param>
        /// <param name="htmlstream"></param>
        /// <returns></returns>
        public static string ConvertToHtml(DocumentAdv document, Stream htmlstream)
        {
            StyleCreator(document);
            return SaveAsHTML(htmlstream);
        }


        /// <summary>
        /// Return the string from the DocumentAdv
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static string ConvertToHtml(DocumentAdv document)
        {
            StyleCreator(document);
            return SaveAsHTML();
        }

        /// <summary>
        /// Processing the Paragraph Style
        /// </summary>
        /// <param name="textContent">Content of the RichTextBox</param>
        internal static void StyleCreator(DocumentAdv textContent)
        {
            bool started = false;
            SpanStyleBuilder.Clear();
            ParagraphStyleBuilder.Clear();
            foreach (SectionAdv section in textContent.Sections)
            {
                StringBuilder blockStyle = new StringBuilder();
                BlockStyleList = new List<StringBuilder>();
                foreach (BlockAdv block in section.Blocks)
                {
                    if (block.IsParagraph)
                    {
                        ParagraphStyleCreator(block as ParagraphAdv, blockStyle, ref started);
                    }
                    else if (block.IsTable)
                    {
                        TableStyleCreator(block as TableAdv, blockStyle);
                    }
                }
                BlockStyleList.Add(blockStyle);
            }
        }

        internal static void ParagraphStyleCreator(ParagraphAdv paragraph,StringBuilder blockStyle,ref bool started)
        {
            string PTagClassName = string.Empty;
            if (paragraph.ListType != ListType.None && !started)
            {
                switch (paragraph.ListType)
                {
                    case ListType.Bulleted:
                        blockStyle.AppendLine();
                        blockStyle.Append(CreateTag(HtmlConstants.ULTag));
                        break;
                    case ListType.Numbered:
                        blockStyle.AppendLine();
                        blockStyle.Append(CreateTag(HtmlConstants.OLTag));
                        break;
                    default:
                        break;
                }
                started = true;
            }
            blockStyle.AppendLine();
            if (paragraph != paragraph.PreviousBlock as ParagraphAdv)
                PTagClassName = ClassNameGenerator();
            TagAttributes.Clear();
#if (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            if (!string.IsNullOrWhiteSpace(PTagClassName))
            {
                ParagraphStyleCreator(paragraph, PTagClassName);
                TagAttributes.Add("class", PTagClassName);
            }
#else
                         if (!string.IsNullOrEmpty(PTagClassName))
                        {
                            ParagraphStyleCreator(paragraph, PTagClassName);
                            TagAttributes.Add("class", PTagClassName);
                        }
#endif
            blockStyle.Append(CreateAttributesTag(HtmlConstants.ParagraphTag, TagAttributes));
            blockStyle.AppendLine();
            if (paragraph.ListType != ListType.None)
            {
                blockStyle.Append(CreateTag(HtmlConstants.LiTag));
                blockStyle.AppendLine();
            }
            if (paragraph.Inlines.Count == 0)
                //Handled to preserve non breaking space for empty paragraphs similar to MS Word behavior.
                blockStyle.Append(HtmlConstants.NonBreakingSpace);
            else
                InlineStyleCreator(paragraph, blockStyle);

            if (paragraph.ListType != ListType.None)
                blockStyle.Append(EndTag(HtmlConstants.LiTag));
            blockStyle.AppendLine();
            blockStyle.Append(EndTag(HtmlConstants.ParagraphTag));
            if ((paragraph.NextBlock != null && paragraph.NextBlock.IsParagraph && (paragraph.NextBlock as ParagraphAdv).ListType != paragraph.ListType && started)
                || (paragraph.NextBlock == null && started))
            {
                switch (paragraph.ListType)
                {
                    case ListType.Bulleted:
                        blockStyle.AppendLine();
                        blockStyle.Append(EndTag(HtmlConstants.ULTag));
                        blockStyle.AppendLine();
                        break;
                    case ListType.Numbered:
                        blockStyle.AppendLine();
                        blockStyle.Append(EndTag(HtmlConstants.OLTag));
                        blockStyle.AppendLine();
                        break;
                    default:
                        break;
                }
                started = false;
            }
        }

        /// <summary>
        /// Creates the style for Inline items.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="blockStyle">The block style.</param>
        private static void InlineStyleCreator(ParagraphAdv paragraph, StringBuilder blockStyle)
        {
            foreach (Inline inline in paragraph.Inlines)
            {
                if (inline is SpanAdv)
                {
                    SpanAdv spanning = new SpanAdv();
                    spanning = inline as SpanAdv;
                    string UniqueClassName = ClassNameGenerator();
                    TagAttributes.Clear();
                    TagAttributes.Add("class", UniqueClassName);
                    blockStyle.Append(CreateAttributesTag(HtmlConstants.SpanTag, TagAttributes));
                    //paragraphStyle.AppendLine();
                    SpanStyleCreator(spanning, UniqueClassName, paragraph);
                    string text = DecodeHtmlNames(spanning.Text);
                    blockStyle.Append(text);
                    //paragraphStyle.AppendLine();
                    blockStyle.Append(EndTag(HtmlConstants.SpanTag));
                    //paragraphStyle.AppendLine();
                }
                else if (inline is HyperlinkAdv)
                {
                    HyperlinkAdv hyperLinkData = inline as HyperlinkAdv;
                    string hyperLinkText = hyperLinkData.Text;
                    string hyperLink = hyperLinkData.NavigationUrl;
                    string UniqueClassName = ClassNameGenerator();
                    TagAttributes.Clear();
                    TagAttributes.Add("class", UniqueClassName);
                    TagAttributes.Add("href", hyperLink);
#if Silverlight
                            TagAttributes.Add(InlineProperties.TargetType, hyperLinkData.TargetType);
#endif
                    hyperLinkText = DecodeHtmlNames(hyperLinkText);
                    blockStyle.Append(CreateAttributesTag(HtmlConstants.HyperlinkTag, TagAttributes));
                    blockStyle.Append(hyperLinkText);
                    blockStyle.AppendLine();
                    blockStyle.Append(EndTag(HtmlConstants.HyperlinkTag));
                    blockStyle.AppendLine();
                }
                else if (inline is ImageContainerAdv)
                {
                    string imagesource = string.Empty;
                    ImageContainerAdv imagecontainer = inline as ImageContainerAdv;
                    if (imagecontainer.ImageSource != null)
                        imagesource = imagecontainer.ImageSource.ToString();
                    if (imagecontainer.ImageBytes != null)
                        imagesource = "data:image/png;base64," + Convert.ToBase64String(imagecontainer.ImageBytes);
                    double width = imagecontainer.Width;
                    double height = imagecontainer.Height;
                    TagAttributes.Clear();
                    TagAttributes.Add(HtmlConstants.Width, width.ToString(CultureInfo.InvariantCulture));
                    TagAttributes.Add(HtmlConstants.Height, height.ToString(CultureInfo.InvariantCulture));
                    TagAttributes.Add(HtmlConstants.ImageSource, imagesource);
                    blockStyle.Append(CreateAttributesTag(HtmlConstants.ImageTag, TagAttributes));
                    blockStyle.AppendLine();
                    blockStyle.Append(EndTag(HtmlConstants.ImageTag));
                    blockStyle.AppendLine();
                }
            }
        }

        internal static void TableStyleCreator(TableAdv table,StringBuilder blockstyle)
        {
            blockstyle.AppendLine();
            TagAttributes.Add(CssConstants.Border, table.BorderThickness.ToString(CultureInfo.InvariantCulture));
            if (table.Background != Color.FromArgb(0, 0, 0, 0))
                TagAttributes.Add(CssConstants.BgColor, ConvertColor(table.Background));
            blockstyle.Append(CreateAttributesTag(HtmlConstants.TableTag, TagAttributes));
            blockstyle.AppendLine();
            foreach (TableRowAdv row in table.Rows)
            {
                TagAttributes.Clear();
                if (row.Background != Color.FromArgb(0, 0, 0, 0))
                    TagAttributes.Add(CssConstants.BgColor, ConvertColor(row.Background));
                TagAttributes.Add(CssConstants.Align, row.TextAlignment);
                blockstyle.Append(CreateAttributesTag(HtmlConstants.TableRowTag, TagAttributes));

                foreach (TableCellAdv cell in row.Cells)
                {
                    TagAttributes.Clear();
                    if (cell.Background != Color.FromArgb(0, 0, 0, 0))
                        TagAttributes.Add(CssConstants.BgColor, ConvertColor(cell.Background));
                    TagAttributes.Add(CssConstants.Align, cell.TextAlignment);
                    blockstyle.Append(CreateAttributesTag(HtmlConstants.TableCellDataTag, TagAttributes));
                    bool started = false;
                    foreach (BlockAdv block in cell.Blocks)
                    {
                        if (block.IsTable)
                        {
                            TableStyleCreator(block as TableAdv, blockstyle);
                        }
                        else
                        {
                            ParagraphStyleCreator(block as ParagraphAdv, blockstyle,ref started);
                        }
                    }
                    blockstyle.AppendLine();
                    blockstyle.Append(EndTag(HtmlConstants.TableCellDataTag));
                    blockstyle.AppendLine();
                }
                blockstyle.AppendLine();
                blockstyle.Append(EndTag(HtmlConstants.TableRowTag));
                blockstyle.AppendLine();
            }
            blockstyle.AppendLine();
            blockstyle.Append(EndTag(HtmlConstants.TableTag));
            blockstyle.AppendLine();
        }

        internal static void ParagraphStyleCreator(ParagraphAdv paragraph, string classname)
        {
            StringBuilder paragrpahclass = new StringBuilder();
            paragrpahclass.AppendLine();
            paragrpahclass.Append(HTMLOperators.Dot);
            paragrpahclass.Append(classname);
            paragrpahclass.Append(HTMLOperators.OpenBrace);

            TextAlignment paragraphalignment = paragraph.TextAlignment;
            paragrpahclass.Append(CssConstants.TextAlign);
            paragrpahclass.Append(HTMLOperators.Colon);
            paragrpahclass.Append(paragraphalignment.ToString());
            paragrpahclass.Append(HTMLOperators.SemiColon);

            double beforespacing = paragraph.BeforeSpacing;
            double rightindent = paragraph.RightIndent;
            double afterspacing = paragraph.AfterSpacing;
            double leftindent = paragraph.LeftIndent;

            paragrpahclass.Append(CssConstants.Margin);
            paragrpahclass.Append(HTMLOperators.Colon);
            paragrpahclass.Append(beforespacing.ToString(CultureInfo.InvariantCulture) + "px " + rightindent.ToString(CultureInfo.InvariantCulture) + "px " + afterspacing.ToString(CultureInfo.InvariantCulture) + "px " + leftindent.ToString(CultureInfo.InvariantCulture) + "px ");
            paragrpahclass.Append(HTMLOperators.SemiColon);

            paragrpahclass.Append(HTMLOperators.CloseBrace);
            paragrpahclass.AppendLine();
            ParagraphStyleBuilder.Add(paragrpahclass);

        }

        internal static void TableStyleCreator(TableAdv table,string classname)
        {
            StringBuilder tableclass = new StringBuilder();
            tableclass.Append(HTMLOperators.Dot);
            tableclass.Append(classname);
            tableclass.Append(HTMLOperators.OpenBrace);

            double borderthickness = table.BorderThickness;
            tableclass.Append(CssConstants.Border);
            tableclass.Append(HTMLOperators.Colon);
            tableclass.Append(borderthickness.ToString(CultureInfo.InvariantCulture));
            tableclass.Append(HTMLOperators.SemiColon);

            string bgcolor = ConvertColor(table.Background);
            tableclass.Append(CssConstants.BgColor);
            tableclass.Append(HTMLOperators.Colon);
            tableclass.Append(bgcolor);
            tableclass.Append(HTMLOperators.SemiColon);

            tableclass.Append(HTMLOperators.CloseBrace);
            tableclass.AppendLine();
            TableStyleBuilder.Add(tableclass);
        }

        /// <summary>
        /// Spans the style creator.
        /// </summary>
        /// <param name="spanText">Span Tag Contents</param>
        /// <param name="className">Name of the class.</param>
        internal static void SpanStyleCreator(SpanAdv spanText, string className, ParagraphAdv paragraph)
        {
            StringBuilder spanClass = new StringBuilder();
            spanClass.AppendLine();
            spanClass.Append(HTMLOperators.Dot);
            spanClass.Append(className);
            spanClass.Append(HTMLOperators.OpenBrace);
            FontStyle fontStyle = spanText.FontStyle;
            if (fontStyle != null)
            {
                spanClass.Append(CssConstants.FontStyle);
                spanClass.Append(HTMLOperators.Colon);
                if (fontStyle.ToString().ToLower() == HtmlConstants.FontStyle_Italic)
                {
                    spanClass.Append(fontStyle.ToString());
                    spanClass.Append(HTMLOperators.SemiColon);
                }
                else
                {
                    spanClass.Append("Normal");
                    spanClass.Append(HTMLOperators.SemiColon);
                }
            }

            //Text Strike
            if (spanText.StrikeThrough != StrikeThrough.None)
            {

                if (spanText.StrikeThrough == StrikeThrough.SingleStrike)
                {
                    spanClass.Append(CssConstants.TextDecoration);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(CssConstants.LineThrough);
                    spanClass.Append(HTMLOperators.SemiColon);
                }
                else if (spanText.StrikeThrough == StrikeThrough.DoubleStrike)
                {
                    spanClass.Append(CssConstants.TextDecoration);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(CssConstants.LineThrough);
                    spanClass.Append(HTMLOperators.SemiColon);
                }
            }

            //Text Baselining
            if (spanText.Baseline != Baseline.Normal)
            {
                if (spanText.Baseline == Baseline.Superscript)
                {
                    spanClass.Append(CssConstants.SubSuperScripts);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(CssConstants.SuperScript);
                    spanClass.Append(HTMLOperators.SemiColon);
                }
                else if (spanText.Baseline == Baseline.Subscript)
                {
                    spanClass.Append(CssConstants.SubSuperScripts);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(CssConstants.SubScript);
                    spanClass.Append(HTMLOperators.SemiColon);
                }
            }

            //Text Foreground and Background Color 
            if (spanText.HighlightColor != null)
            {
                string highlightColor = ConvertBackgroundColor(spanText.HighlightColor);
                if (!string.IsNullOrEmpty(highlightColor))
                {
                    spanClass.Append(CssConstants.Background);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(highlightColor);
                    spanClass.Append(HTMLOperators.SemiColon);
                }
            }

            if (spanText.Foreground != null)
            {
                string textColor = ConvertColor(spanText.Foreground);
                if (!string.IsNullOrEmpty(textColor))
                {
                    spanClass.Append(CssConstants.Color);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(textColor);
                    spanClass.Append(HTMLOperators.SemiColon);
                }
            }

            if (spanText.Underline == true)
            {
                spanClass.Append(CssConstants.TextDecoration);
                spanClass.Append(HTMLOperators.Colon);
                spanClass.Append(CssConstants.Underline);
                spanClass.Append(HTMLOperators.SemiColon);
                spanClass.AppendLine();
            }

            if (spanText.FontSize != 0)
            {
                spanClass.Append(CssConstants.FontSize);
                spanClass.Append(HTMLOperators.Colon);
                spanClass.Append(spanText.FontSize.ToString(CultureInfo.InvariantCulture));
                spanClass.Append(CssConstants.Pt);
                spanClass.Append(HTMLOperators.SemiColon);
            }
            if (spanText.FontFamily != null)
            {
                spanClass.Append(CssConstants.FontFamily);
                spanClass.Append(HTMLOperators.Colon);
                spanClass.Append(spanText.FontFamily);
                spanClass.Append(HTMLOperators.SemiColon);
            }
            if (spanText.FontWeight != null)
            {
                FontWeight fontWeight = spanText.FontWeight;
                if (fontWeight.ToString().ToLower() == HtmlConstants.FontWeight_Bold)
                {
                    spanClass.Append(CssConstants.FontWeight);
                    spanClass.Append(HTMLOperators.Colon);
                    spanClass.Append(fontWeight.ToString());
                    spanClass.Append(HTMLOperators.SemiColon);
                }
            }
            spanClass.Append(HTMLOperators.CloseBrace);
            SpanStyleBuilder.Add(spanClass);
        }

        /// <summary>
        /// Saves as HTML.
        /// </summary>
        internal static string SaveAsHTML(Stream htmlStream)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(CreateTag(HtmlConstants.HtmlTag));
            stringBuilder.AppendLine();
            stringBuilder.Append(CreateTag(HtmlConstants.HeadTag));
            stringBuilder.AppendLine();
            TagAttributes.Clear();
            TagAttributes.Add("type", "text/css");
            stringBuilder.Append(CreateAttributesTag(HtmlConstants.StyleTag, TagAttributes));
            foreach (StringBuilder styleList in SpanStyleBuilder)
            {
                stringBuilder.Append(styleList);
                stringBuilder.AppendLine();
            }
            foreach (StringBuilder item in ParagraphStyleBuilder)
            {
                int index = ParagraphStyleBuilder.IndexOf(item);
                stringBuilder.Append(item);
                stringBuilder.AppendLine();
            }
            stringBuilder.Append(EndTag(HtmlConstants.StyleTag));
            stringBuilder.AppendLine();
            stringBuilder.Append(EndTag(HtmlConstants.HeadTag));
            stringBuilder.AppendLine();
            stringBuilder.Append(CreateTag(HtmlConstants.BodyTag));
            foreach (StringBuilder paragraphStyle in BlockStyleList)
            {
                stringBuilder.Append(paragraphStyle);
                stringBuilder.AppendLine();
            }
            stringBuilder.Append(WriteEndDocument());

            //Writes the content to stream.
            StreamWriter writer = new StreamWriter(htmlStream);
            writer.Write(stringBuilder.ToString());
            writer.Flush();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Return the HTML String without any input
        /// </summary>
        /// <returns></returns>
        internal static string SaveAsHTML()
        {
            StringWriter stringwriter = new StringWriter();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(CreateTag(HtmlConstants.HtmlTag));
            stringBuilder.AppendLine();
            stringBuilder.Append(CreateTag(HtmlConstants.HeadTag));
            stringBuilder.AppendLine();
            TagAttributes.Clear();
            TagAttributes.Add("type", "text/css");
            stringBuilder.Append(CreateAttributesTag(HtmlConstants.StyleTag, TagAttributes));
            foreach (StringBuilder styleList in SpanStyleBuilder)
            {
                stringBuilder.Append(styleList);
                stringBuilder.AppendLine();
            }
            foreach (StringBuilder item in ParagraphStyleBuilder)
            {
                stringBuilder.Append(item);
                stringBuilder.AppendLine();
            }
            stringBuilder.Append(EndTag(HtmlConstants.StyleTag));
            stringBuilder.AppendLine();
            stringBuilder.Append(EndTag(HtmlConstants.HeadTag));
            stringBuilder.AppendLine();
            stringBuilder.Append(CreateTag(HtmlConstants.BodyTag));
            foreach (StringBuilder paragraphStyle in BlockStyleList)
            {
                stringBuilder.Append(paragraphStyle);
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine();
            stringBuilder.Append(WriteEndDocument());
            stringwriter.WriteLine(stringBuilder);
            stringwriter.Close();
            return stringBuilder.ToString();
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Writes the start document.
        /// </summary>
        /// <returns></returns>
        internal static StringBuilder WriteStartDocument()
        {
            StringBuilder firstTag = new StringBuilder();
            firstTag.Append(CreateTag(HtmlConstants.HtmlTag));
            firstTag.AppendLine();
            firstTag.Append(CreateTag(HtmlConstants.BodyTag));
            firstTag.AppendLine();
            return firstTag;
        }

        /// <summary>
        /// Writes the end document.
        /// </summary>
        /// <returns>End Tags</returns>
        internal static StringBuilder WriteEndDocument()
        {
            StringBuilder endTag = new StringBuilder();
            endTag.Append(EndTag(HtmlConstants.BodyTag));
            endTag.AppendLine();
            endTag.Append(EndTag(HtmlConstants.HtmlTag));
            endTag.AppendLine();
            return endTag;
        }


        private static string DecodeHtmlNames(string text)
        {
            foreach (string symbol in htmlasciicode.HtmlNameTable.Values)
            {
                if (text.Contains(symbol) && symbol != "&" )
                {
                    var key = (from K in htmlasciicode.HtmlNameTable where string.Compare(K.Value, symbol) == 0 select K.Key).FirstOrDefault();
                    text = text.Replace(symbol, key);
                }
            }
            return text;
        }

        /// <summary>
        /// Creates the tag.
        /// </summary>
        /// <param name="tagValue">The tag value.</param>
        /// <returns></returns>
        internal static StringBuilder CreateTag(string tagValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HTMLOperators.LessthanSymbol);
            sb.Append(tagValue);
            sb.Append(HTMLOperators.GreaterthanSymbol);
            return sb;
        }

        /// <summary>
        /// Creates the attribute tag.
        /// </summary>
        /// <param name="tagValue">The tag value.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns></returns>

        internal static StringBuilder CreateAttributesTag(string tagValue, Dictionary<string, object> localproperties)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HTMLOperators.LessthanSymbol);
            sb.Append(tagValue);
            foreach (string attributename in localproperties.Keys)
            {
                sb.Append("  ");
                sb.Append(attributename);
                sb.Append(HTMLOperators.Equal);
                string Value = HTMLOperators.Quotes + localproperties[attributename] + HTMLOperators.Quotes;
                sb.Append(Value);
            }
            sb.Append(HTMLOperators.GreaterthanSymbol);
            return sb;
        }
        /// <summary>
        /// Ends the tag.
        /// </summary>
        /// <param name="tagValue">The tag value.</param>
        /// <returns></returns>
        internal static StringBuilder EndTag(string tagValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HTMLOperators.LessthanSymbol);
            sb.Append(HTMLOperators.Endslash);
            sb.Append(tagValue);
            sb.Append(HTMLOperators.GreaterthanSymbol);
            return sb;
        }

        /// <summary>
        /// Classes the name generator.
        /// </summary>
        /// <returns></returns>
        internal static string ClassNameGenerator()
        {
            string className = "style";
            className = className + randomNumber.Next();
            return className;
        }

        /// <summary>
        /// Converts the color.
        /// </summary>
        /// <param name="convertingColor">Convert the Color from Brush to string</param>
        /// <returns></returns>
        internal static string ConvertColor(Color convertingColor)
        {
            Color brush = ((Color)convertingColor);
            Color Color = Color.FromArgb(0xFF, brush.R, brush.G, brush.B);
            string textcolor = Color.ToString().Remove(0, 3);
            return "#" + textcolor;
        }

        /// <summary>
        /// Converts the color.
        /// </summary>
        /// <param name="convertingColor">Convert the Color from Brush to string</param>
        /// <returns></returns>
        internal static string ConvertBackgroundColor(Color convertingColor)
        {
            Color brush = ((Color)convertingColor);
            Color Color = Color.FromArgb(brush.A, brush.R, brush.G, brush.B);
            string highlightColor = Color.ToString().Remove(0, 3);
            if (brush.A == 0)
                return "#ffffff";
            return "#" + highlightColor;
        }


        #endregion

    }
}
