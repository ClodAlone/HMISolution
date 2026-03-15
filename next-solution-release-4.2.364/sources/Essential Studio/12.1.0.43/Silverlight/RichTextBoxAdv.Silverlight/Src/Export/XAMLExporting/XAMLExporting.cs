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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    public class XAMLExporting
    {
        #region Private Members

        static string nameSpaceValue = "RichText";

        #endregion

        #region Constructor

        public XAMLExporting()
        {

        }
        #endregion

        #region Implementation

        /// <summary>
        /// Returns the XAML string from the DocumentAdv and Stream
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xamlstream"></param>
        /// <returns></returns>
        public static string ConvertToXAML(DocumentAdv document, Stream xamlstream)
        {
            return SaveAsXAML(document, xamlstream);
        }

        /// <summary>
        /// Returns the XAML string from the DocumentAdv
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static string ConvertToXAML(DocumentAdv document)
        {
            return SaveAsXAML(document);
        }

        /// <summary>
        /// Process the DoucmentAdv 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static StringBuilder XamlProcessor(DocumentAdv document)
        {
            StringBuilder blockbuilder = new StringBuilder();
            blockbuilder.Append(CreateNamedSpaces());
            blockbuilder.AppendLine();
            foreach (SectionAdv section in document.Sections)
            {
                blockbuilder.Append(CreateSection());
                foreach (BlockAdv block in section.Blocks)
                {
                    if (block.IsParagraph)
                    {
                        CreateParagraphBlock(block, ref blockbuilder);
                    }
                    else if (block.IsTable)
                    {
                        IterateBlocks(block, ref blockbuilder);
                    }
                }
                blockbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.Section));
            }
            blockbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.Document));
            return blockbuilder;
        }

        internal static void IterateBlocks(BlockAdv tempblock, ref StringBuilder tempbuilder)
        {
            tempbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.TableAdv, new StringBuilder("")));
            tempbuilder.AppendLine();
            foreach (TableRowAdv r in (tempblock as TableAdv).Rows)
            {
                tempbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.TableRowAdv, new StringBuilder("")));
                tempbuilder.AppendLine();
                foreach (TableCellAdv c in r.Cells)
                {
                    StringBuilder cellbuilder = new StringBuilder();
                    cellbuilder.Append(CreateAttribute(CellProperties.DesiredWidth, c.DesiredWidth.ToString(CultureInfo.InvariantCulture)));
                    cellbuilder.Append(CreateAttribute(CellProperties.RowSpan, c.RowSpan.ToString()));
                    cellbuilder.Append(CreateAttribute(CellProperties.ColumnSpan, c.ColumnSpan.ToString()));
                    cellbuilder.Append(CreateAttribute(CellProperties.Background, c.Background.ToString()));
                    tempbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.TableCellAdv, cellbuilder));
                    foreach (BlockAdv b in c.Blocks)
                    {
                        if (b.IsParagraph)
                        {
                            CreateParagraphBlock(b, ref tempbuilder);
                        }
                        else if (b.IsTable)
                        {
                            IterateBlocks(b, ref tempbuilder);
                        }
                    }
                    tempbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.TableCellAdv));
                    tempbuilder.AppendLine();
                }
                tempbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.TableRowAdv));
                tempbuilder.AppendLine();
            }
            tempbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.TableAdv));
            tempbuilder.AppendLine();
        }

        internal static void CreateParagraphBlock(BlockAdv b, ref StringBuilder paragraphbuilder)
        {
            StringBuilder paragraphAttributes = new StringBuilder();
            paragraphAttributes = CreateParagraphAttributes(b as ParagraphAdv);
            paragraphbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.Paragraph, paragraphAttributes));
            paragraphbuilder.AppendLine();
            foreach (Inline inlineContent in b.Inlines)
            {
                if (inlineContent is SpanAdv)
                {
                    StringBuilder spanAttributes = new StringBuilder();
                    SpanAdv spanXamlContent = inlineContent as SpanAdv;
                    spanAttributes = CreateSpanAttribute(spanXamlContent);
                    paragraphbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.Span, spanAttributes));
                    paragraphbuilder.AppendLine();
                    paragraphbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.Span));
                    paragraphbuilder.AppendLine();
                }
                else if (inlineContent is HyperlinkAdv)
                {
                    StringBuilder hyperLinkAttributes = new StringBuilder();
                    HyperlinkAdv hyperlinkContent = inlineContent as HyperlinkAdv;
                    if (hyperlinkContent.NavigationUrl != null)
                    {
                        hyperLinkAttributes.Append(CreateAttribute(InlineProperties.NavigationUrl, hyperlinkContent.NavigationUrl));
                    }
                    if (hyperlinkContent.FontFamily != null)
                    {
                        hyperLinkAttributes.Append((CreateAttribute(InlineProperties.FontFamily, hyperlinkContent.FontFamily.ToString())));
                    }
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.FontSize, hyperlinkContent.FontSize.ToString(CultureInfo.InvariantCulture))));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.Foreground, hyperlinkContent.Foreground.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.FontWeight, hyperlinkContent.FontWeight.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.FontStyle, hyperlinkContent.FontStyle.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.HighlightColor, hyperlinkContent.HighlightColor.ToString())));

                    if (hyperlinkContent.Text != null)
                    {
                        hyperLinkAttributes.Append(CreateAttribute(InlineProperties.NavigationText, hyperlinkContent.Text.Trim()));
                    }
#if !WPF
                    if (hyperlinkContent.TargetType == HyperlinkTargetType.Blank)
                    {
                        hyperLinkAttributes.Append(CreateAttribute(InlineProperties.TargetType, "Blank"));
                    }
                    else if (hyperlinkContent.TargetType == HyperlinkTargetType.Self)
                    {
                        hyperLinkAttributes.Append(CreateAttribute(InlineProperties.TargetType, "Self"));
                    }
#endif
                    paragraphbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.HyperLink, hyperLinkAttributes));
                    paragraphbuilder.AppendLine();
                    paragraphbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.HyperLink));
                    paragraphbuilder.AppendLine();
                }
                else if (inlineContent is ImageContainerAdv)
                {
                    StringBuilder Imagecontainerattributes = new StringBuilder();
                    ImageContainerAdv image = inlineContent as ImageContainerAdv;
                    string imagesource = string.Empty;

                    if (image.ImageSource != null)
                    {
                        if (image.ImageSource is BitmapImage && (image.ImageSource as BitmapImage).UriSource != null
                            && !string.IsNullOrEmpty((image.ImageSource as BitmapImage).UriSource.OriginalString)
                            && Uri.IsWellFormedUriString((image.ImageSource as BitmapImage).UriSource.OriginalString, UriKind.RelativeOrAbsolute))
                        {
                            imagesource = (image.ImageSource as BitmapImage).UriSource.OriginalString;
                            Imagecontainerattributes.Append(CreateAttribute(InlineProperties.ImageSource, imagesource));
                        }
                        else
                        {
                            if (image.ImageBytes != null)
                                imagesource = Convert.ToBase64String(image.ImageBytes);
                            else
                            {
#if WPF
                                BitmapEncoder encoder = new BmpBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.ImageSource));
                                MemoryStream ms = new MemoryStream();
                                encoder.Save(ms);
                                imagesource = Convert.ToBase64String(ms.GetBuffer());
#else
                                imagesource = image.ImageSource.ToString();
#endif
                            }
                            Imagecontainerattributes.Append(CreateAttribute(InlineProperties.ImageString, imagesource));
                        }
                    }
                    else if (image.ImageBytes != null)
                    {
                        imagesource = Convert.ToBase64String(image.ImageBytes);
                        Imagecontainerattributes.Append(CreateAttribute(InlineProperties.ImageString, imagesource));
                    }


                    Imagecontainerattributes.Append(CreateAttribute(InlineProperties.Width, image.Width.ToString(CultureInfo.InvariantCulture)));
                    Imagecontainerattributes.Append(CreateAttribute(InlineProperties.Height, image.Height.ToString(CultureInfo.InvariantCulture)));
                    paragraphbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.ImageContainer, Imagecontainerattributes));
                    paragraphbuilder.AppendLine();
                    paragraphbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.ImageContainer));
                    paragraphbuilder.AppendLine();
                }
                else if (inlineContent is UIContainerAdv)
                {
                    StringBuilder UiElementattribute = new StringBuilder();
                    UIContainerAdv UiContainer = inlineContent as UIContainerAdv;
                    string uielementstring = XamlWriter.Write(UiContainer.UIElement);
                    UiElementattribute.Append(CreateAttribute(InlineProperties.Width, UiContainer.Width.ToString(CultureInfo.InvariantCulture)));
                    UiElementattribute.Append(CreateAttribute(InlineProperties.Height, UiContainer.Height.ToString(CultureInfo.InvariantCulture)));
                    paragraphbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.UiContainerAdv, UiElementattribute));
                    paragraphbuilder.AppendLine();
                    paragraphbuilder.Append(uielementstring);
                    paragraphbuilder.AppendLine();
                    paragraphbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.UiContainerAdv));
                    paragraphbuilder.AppendLine();
                }
            }
            paragraphbuilder.Append(CreateEndTag(nameSpaceValue, RichTextBoxConstants.Paragraph));
            paragraphbuilder.AppendLine();
        }

        /// <summary>
        /// It creates the attribute for Span Tag
        /// </summary>
        /// <param name="spanContent"></param>
        /// <returns></returns>
        internal static StringBuilder CreateSpanAttribute(SpanAdv spanContent)
        {
            StringBuilder spanClass = new StringBuilder();
            string text = spanContent.Text;
            //XAML uses character entities defined in XML for special characters. And escape sequence ({}) for some characters.
            if (text != null)
            {
                if (text.Contains("&"))
                    text = text.Replace("&", "&amp;");
                if (text.Contains("\""))
                    text = text.Replace("\"", "&quot;");
                if (text.Contains("<"))
                    text = text.Replace("<", "&lt;");
                if (text.Contains(">"))
                    text = text.Replace(">", "&gt;");
                if (text.StartsWith("{"))
                    text = "{}" + text;
                spanClass.Append(CreateAttribute(InlineProperties.Text, text));
            }

            FontStyle fontStyle = spanContent.FontStyle;
            if (fontStyle != null)
            {
                if (fontStyle.ToString() == "Italic")
                {
                    spanClass.Append(CreateAttribute(InlineProperties.FontStyle, fontStyle.ToString()));
                }
                else
                {
                    spanClass.Append(CreateAttribute(InlineProperties.FontStyle, "Normal"));
                }
            }

            //Text Strike
            if (spanContent.StrikeThrough != StrikeThrough.None)
            {
                if (spanContent.StrikeThrough == StrikeThrough.SingleStrike)
                {
                    spanClass.Append(CreateAttribute(InlineProperties.StrikeThrough, "SingleStrike"));
                }
                else if (spanContent.StrikeThrough == StrikeThrough.DoubleStrike)
                {
                    spanClass.Append(CreateAttribute(InlineProperties.StrikeThrough, "DoubleStrike"));
                }
            }

            if (spanContent.Baseline == Baseline.Superscript)
            {
                spanClass.Append(CreateAttribute(InlineProperties.BaseLine, "Superscript"));
            }
            else if (spanContent.Baseline == Baseline.Subscript)
            {
                spanClass.Append(CreateAttribute(InlineProperties.BaseLine, "Subscript"));
            }
            else if (spanContent.Baseline == Baseline.Normal)
            {
                spanClass.Append(CreateAttribute(InlineProperties.BaseLine, "Normal"));
            }

            //Text Foreground and Background Color 
            if (spanContent.HighlightColor != null)
            {
                string highlightColor = ConvertColor(spanContent.HighlightColor);
                spanClass.Append(CreateAttribute(InlineProperties.HighlightColor, highlightColor));
            }

            if (spanContent.Foreground != null)
            {
                string textColor = ConvertColor(spanContent.Foreground);
                spanClass.Append(CreateAttribute(InlineProperties.Foreground, textColor));
            }

            if (spanContent.Underline == true)
            {
                spanClass.Append(CreateAttribute(InlineProperties.UnderLine, "true"));
            }

            if (spanContent.FontSize != 0.0)
            {
                spanClass.Append(CreateAttribute(InlineProperties.FontSize, spanContent.FontSize.ToString(CultureInfo.InvariantCulture)));
            }
            if (spanContent.FontFamily != null)
            {
                spanClass.Append(CreateAttribute(InlineProperties.FontFamily, spanContent.FontFamily.ToString()));
            }
            if (spanContent.FontWeight != null)
            {
                FontWeight fontWeight = spanContent.FontWeight;
                if (fontWeight.ToString() == "Bold")
                {
                    spanClass.Append(CreateAttribute(InlineProperties.FontWeight, fontWeight.ToString()));
                }
                else
                {
                    spanClass.Append(CreateAttribute(InlineProperties.FontWeight, "Normal"));
                }
            }

            return spanClass;
        }

        /// <summary>
        /// It creates attributes for the Paragraph Tag
        /// </summary>
        /// <param name="paragraphText"></param>
        /// <returns></returns>
        internal static StringBuilder CreateParagraphAttributes(ParagraphAdv paragraphText)
        {
            StringBuilder paragraphClass = new StringBuilder();
            paragraphClass.Append(CreateAttribute(ParagraphProperties.ListType, paragraphText.ListType.ToString()));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.TextAlignment, paragraphText.TextAlignment.ToString()));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.LeftIndent, paragraphText.LeftIndent.ToString(CultureInfo.InvariantCulture)));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.RightIndent, paragraphText.RightIndent.ToString(CultureInfo.InvariantCulture)));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.BeforeSpacing, paragraphText.BeforeSpacing.ToString(CultureInfo.InvariantCulture)));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.AfterSpacing, paragraphText.AfterSpacing.ToString(CultureInfo.InvariantCulture)));
            return paragraphClass;
        }


        /// <summary>
        /// Save as XAML based upon the input DocumentAdv and stream
        /// </summary>
        /// <param name="documentadv"></param>
        /// <param name="xamlStream"></param>
        /// <returns></returns>
        internal static string SaveAsXAML(DocumentAdv documentadv, Stream xamlStream)
        {
            StringBuilder textBuilder = new StringBuilder();
            textBuilder = XamlProcessor(documentadv);

            //Writes the content to stream.
            StreamWriter writer = new StreamWriter(xamlStream);
            writer.Write(textBuilder.ToString());
            writer.Flush();
            return textBuilder.ToString();
        }

        /// <summary>
        /// Save as XAML based upon the input DocumentAdv
        /// </summary>
        /// <param name="documentadv"></param>
        /// <returns></returns>
        internal static string SaveAsXAML(DocumentAdv documentadv)
        {
            StringWriter writer = new StringWriter();
            StringBuilder textBuilder = new StringBuilder();
            textBuilder = XamlProcessor(documentadv);
            writer.WriteLine(textBuilder);
            writer.Close();
            return textBuilder.ToString();
        }

        /// <summary>
        /// It creates thee attribute tag 
        /// </summary>
        /// <param name="nameSpaceValue"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        internal static StringBuilder CreateAttributeTag(string nameSpaceValue, string attributeName, StringBuilder attributeValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HTMLOperators.LessthanSymbol);
            sb.Append(nameSpaceValue);
            sb.Append(HTMLOperators.Colon);
            sb.Append(attributeName);
            sb.Append(" ");
            if (attributeValue != null)
            {
                sb.Append(attributeValue);
            }
            sb.Append(HTMLOperators.GreaterthanSymbol);
            return sb;
        }

        /// <summary>
        /// It creates the attribute
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="attributeData"></param>
        /// <returns></returns>
        internal static StringBuilder CreateAttribute(string keyWord, string attributeData)
        {
            StringBuilder attribBuilder = new StringBuilder();
            attribBuilder.Append(keyWord);
            attribBuilder.Append(HTMLOperators.Equal);
            attribBuilder.Append(HTMLOperators.Quotes);
            attribBuilder.Append(attributeData);
            attribBuilder.Append(HTMLOperators.Quotes);
            attribBuilder.Append(" ");
            return attribBuilder;
        }

        /// <summary>
        /// It creates the End tag.
        /// </summary>
        /// <param name="nameSpaceValue"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        internal static StringBuilder CreateEndTag(string nameSpaceValue, string attributeName)
        {
            StringBuilder endTag = new StringBuilder();
            endTag.Append(HTMLOperators.LessthanSymbol);
            endTag.Append(HTMLOperators.Endslash);
            endTag.Append(nameSpaceValue);
            endTag.Append(HTMLOperators.Colon);
            endTag.Append(attributeName);
            endTag.Append(HTMLOperators.GreaterthanSymbol);
            return endTag;
        }

        /// <summary>
        /// It creates the namespace for the style
        /// </summary>
        /// <returns></returns>
        internal static StringBuilder CreateNamedSpaces()
        {
            StringBuilder nameSpace = new StringBuilder();
            nameSpace.Append(HTMLOperators.LessthanSymbol);
            nameSpace.Append(nameSpaceValue);
            nameSpace.Append(HTMLOperators.Colon);
            nameSpace.Append(RichTextBoxConstants.Document);
            nameSpace.Append(" ");
            nameSpace.Append(CreateAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
#if !WPF
            nameSpace.Append(CreateAttribute("xmlns:RichText", "clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.RichTextBoxAdv.Silverlight"));
#endif
#if WPF
            nameSpace.Append(CreateAttribute("xmlns:RichText", "clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.RichTextBoxAdv.WPF"));
#endif
            nameSpace.Append(HTMLOperators.GreaterthanSymbol);
            return nameSpace;
        }

        /// <summary>
        /// It creates the SectionAdv
        /// </summary>
        /// <returns></returns>
        internal static StringBuilder CreateSection()
        {
            StringBuilder sectionAdv = new StringBuilder();
            sectionAdv.Append(HTMLOperators.LessthanSymbol);
            sectionAdv.Append(nameSpaceValue);
            sectionAdv.Append(HTMLOperators.Colon);
            sectionAdv.Append(RichTextBoxConstants.Section);
            sectionAdv.Append(HTMLOperators.GreaterthanSymbol);
            return sectionAdv;
        }

        /// <summary>
        /// It converts the string into correct format.
        /// </summary>
        /// <param name="convertingColor"></param>
        /// <returns></returns>
        internal static string ConvertColor(Color convertingColor)
        {
            Color brush = ((Color)convertingColor);

            Color highlightColor = Color.FromArgb(brush.A, brush.R, brush.G, brush.B);
            if (highlightColor.A == 0 && highlightColor.B == 0 && highlightColor.G == 0 && highlightColor.R == 0)
                return convertingColor.ToString();
            return highlightColor.ToString();
        }

        #endregion
    }
}
