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
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
#if WPF
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Text;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class XAMLExporting
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
                    if (block is ParagraphAdv)
                    {
                        CreateParagraphBlock(block, ref blockbuilder);
                    }
                    else if (block is TableAdv)
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
                    cellbuilder.Append(CreateAttribute(CellProperties.DesiredWidth, c.CellFormat.CellWidth.ToString()));
                    cellbuilder.Append(CreateAttribute(CellProperties.RowSpan, c.CellFormat.RowSpan.ToString()));
                    cellbuilder.Append(CreateAttribute(CellProperties.ColumnSpan, c.CellFormat.ColumnSpan.ToString()));
                    cellbuilder.Append(CreateAttribute(CellProperties.Background, c.CellFormat.Background.ToString()));
                    tempbuilder.Append(CreateAttributeTag(nameSpaceValue, RichTextBoxConstants.TableCellAdv, cellbuilder));
                    foreach (BlockAdv b in c.Blocks)
                    {
                        if (b is ParagraphAdv)
                        {
                            CreateParagraphBlock(b, ref tempbuilder);
                        }
                        else if (b is TableAdv)
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
            foreach (Inline inlineContent in (b as ParagraphAdv).Inlines)
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
                else if (inlineContent is FieldBeginAdv)
                {
                    StringBuilder hyperLinkAttributes = new StringBuilder();
                    FieldBeginAdv hyperlinkContent = inlineContent as FieldBeginAdv;
                    //if (hyperlinkContent.NavigationUrl != null)
                    //{
                    //    hyperLinkAttributes.Append(CreateAttribute(InlineProperties.NavigationUrl, hyperlinkContent.NavigationUrl));
                    //}
                    if (hyperlinkContent.CharacterFormat.FontFamily != null)
                    {
                        hyperLinkAttributes.Append((CreateAttribute(InlineProperties.FontFamily, hyperlinkContent.CharacterFormat.FontFamily.ToString())));
                    }
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.FontSize, hyperlinkContent.CharacterFormat.FontSize.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.Foreground, hyperlinkContent.CharacterFormat.FontColor.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.BoldStyle, hyperlinkContent.CharacterFormat.Bold.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.ItalicStyle, hyperlinkContent.CharacterFormat.Italic.ToString())));
                    hyperLinkAttributes.Append((CreateAttribute(InlineProperties.HighlightColor, hyperlinkContent.CharacterFormat.HighlightColor.ToString())));

                    //if (hyperlinkContent.Text != null)
                    //{
                    //    hyperLinkAttributes.Append(CreateAttribute(InlineProperties.NavigationText, hyperlinkContent.Text.Trim()));
                    //}
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

                    if (image.ImageBytes != null)
                    {
                        imagesource = Convert.ToBase64String(image.ImageBytes);
                        Imagecontainerattributes.Append(CreateAttribute(InlineProperties.ImageString, imagesource));
                    }

                    else if (image.ImageSource != null)
                    {
                        imagesource = image.ImageSource.ToString();
                        Imagecontainerattributes.Append(CreateAttribute(InlineProperties.ImageSource, imagesource));
                    }                   

                    
                    Imagecontainerattributes.Append(CreateAttribute(InlineProperties.Width, image.Width.ToString()));
                    Imagecontainerattributes.Append(CreateAttribute(InlineProperties.Height, image.Height.ToString()));
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
                    UiElementattribute.Append(CreateAttribute(InlineProperties.Width, UiContainer.Width.ToString()));
                    UiElementattribute.Append(CreateAttribute(InlineProperties.Height, UiContainer.Height.ToString()));
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
            if (spanContent.Text != null)
            {
                if (spanContent.Text.Contains("&"))
                    spanContent.Text = spanContent.Text.Replace("&", "&amp;");
                if (spanContent.Text.Contains("\""))
                    spanContent.Text = spanContent.Text.Replace("\"", "&quot;");
                if (spanContent.Text.Contains("<") || spanContent.Text.Contains(">"))
                    spanContent.Text = spanContent.Text.Replace("<", "&lt;").Replace(">", "&gt;");
                if (spanContent.Text.StartsWith("{"))
                    spanContent.Text = spanContent.Text.Replace("{", "{}{");

                spanClass.Append(CreateAttribute(InlineProperties.Text, spanContent.Text));
            }

            //Text Strike
            if (spanContent.CharacterFormat.StrikeThrough != StrikeThrough.None)
            {
                if (spanContent.CharacterFormat.StrikeThrough == StrikeThrough.SingleStrike)
                {
                    spanClass.Append(CreateAttribute(InlineProperties.StrikeThrough, "SingleStrike"));
                }
                else if (spanContent.CharacterFormat.StrikeThrough == StrikeThrough.DoubleStrike)
                {
                    spanClass.Append(CreateAttribute(InlineProperties.StrikeThrough, "DoubleStrike"));
                }
            }

            if (spanContent.CharacterFormat.BaselineAlignment == BaselineAlignment.Superscript)
            {
                spanClass.Append(CreateAttribute(InlineProperties.BaseLineAlignment, "Superscript"));
            }
            else if (spanContent.CharacterFormat.BaselineAlignment == BaselineAlignment.Subscript)
            {
                spanClass.Append(CreateAttribute(InlineProperties.BaseLineAlignment, "Subscript"));
            }
            else if (spanContent.CharacterFormat.BaselineAlignment == BaselineAlignment.Normal)
            {
                spanClass.Append(CreateAttribute(InlineProperties.BaseLineAlignment, "Normal"));
            }

            //Text Foreground and Background Color 
            if (spanContent.CharacterFormat.HighlightColor != HighlightColor.NoColor)
            {
                string highlightColor = spanContent.CharacterFormat.HighlightColor.ToString();
                spanClass.Append(CreateAttribute(InlineProperties.HighlightColor, highlightColor));
            }

            if (spanContent.CharacterFormat.FontColor != null)
            {
                string textColor = ConvertColor(spanContent.CharacterFormat.FontColor);
                spanClass.Append(CreateAttribute(InlineProperties.Foreground, textColor));
            }

            if (spanContent.CharacterFormat.Underline != Underline.None)
            {
                spanClass.Append(CreateAttribute(InlineProperties.UnderLine, "true"));
            }

            if (spanContent.CharacterFormat.FontSize != 0.0)
            {
                spanClass.Append(CreateAttribute(InlineProperties.FontSize, spanContent.CharacterFormat.FontSize.ToString()));
            }
            if (spanContent.CharacterFormat.FontFamily != null)
            {
                spanClass.Append(CreateAttribute(InlineProperties.FontFamily, spanContent.CharacterFormat.FontFamily.ToString()));
            }
            spanClass.Append(CreateAttribute(InlineProperties.BoldStyle, spanContent.CharacterFormat.Bold.ToString()));
            spanClass.Append(CreateAttribute(InlineProperties.ItalicStyle, spanContent.CharacterFormat.Italic.ToString()));
            
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
            paragraphClass.Append(CreateAttribute(ParagraphProperties.TextAlignment, paragraphText.ParagraphFormat.TextAlignment.ToString()));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.LeftIndent, paragraphText.ParagraphFormat.LeftIndent.ToString()));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.RightIndent, paragraphText.ParagraphFormat.RightIndent.ToString()));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.BeforeSpacing, paragraphText.ParagraphFormat.BeforeSpacing.ToString()));
            paragraphClass.Append(CreateAttribute(ParagraphProperties.AfterSpacing, paragraphText.ParagraphFormat.AfterSpacing.ToString()));
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
            writer.Dispose();
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
            sb.Append(OperatorConstants.LessthanSymbol);
            sb.Append(nameSpaceValue);
            sb.Append(OperatorConstants.Colon);
            sb.Append(attributeName);
            sb.Append(" ");
            if (attributeValue != null)
            {
                sb.Append(attributeValue);
            }
            sb.Append(OperatorConstants.GreaterthanSymbol);
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
            attribBuilder.Append(OperatorConstants.Equal);
            attribBuilder.Append(OperatorConstants.Quotes);
            attribBuilder.Append(attributeData);
            attribBuilder.Append(OperatorConstants.Quotes);
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
            endTag.Append(OperatorConstants.LessthanSymbol);
            endTag.Append(OperatorConstants.Endslash);
            endTag.Append(nameSpaceValue);
            endTag.Append(OperatorConstants.Colon);
            endTag.Append(attributeName);
            endTag.Append(OperatorConstants.GreaterthanSymbol);
            return endTag;
        }

        /// <summary>
        /// It creates the namespace for the style
        /// </summary>
        /// <returns></returns>
        internal static StringBuilder CreateNamedSpaces()
        {
            StringBuilder nameSpace = new StringBuilder();
            nameSpace.Append(OperatorConstants.LessthanSymbol);
            nameSpace.Append(nameSpaceValue);
            nameSpace.Append(OperatorConstants.Colon);
            nameSpace.Append(RichTextBoxConstants.Document);
            nameSpace.Append(" ");
            nameSpace.Append(CreateAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
#if !WPF
            nameSpace.Append(CreateAttribute("xmlns:RichText", "clr-namespace:Syncfusion.UI.Xaml.RichTextBoxAdv;assembly=Syncfusion.RichTextBoxAdv.Silverlight"));
#endif
#if WPF
            nameSpace.Append(CreateAttribute("xmlns:RichText", "clr-namespace:Syncfusion.UI.Xaml.RichTextBoxAdv;assembly=Syncfusion.RichTextBoxAdv.WPF"));
#endif
            nameSpace.Append(OperatorConstants.GreaterthanSymbol);
            return nameSpace;
        }

        /// <summary>
        /// It creates the SectionAdv
        /// </summary>
        /// <returns></returns>
        internal static StringBuilder CreateSection()
        {
            StringBuilder sectionAdv = new StringBuilder();
            sectionAdv.Append(OperatorConstants.LessthanSymbol);
            sectionAdv.Append(nameSpaceValue);
            sectionAdv.Append(OperatorConstants.Colon);
            sectionAdv.Append(RichTextBoxConstants.Section);
            sectionAdv.Append(OperatorConstants.GreaterthanSymbol);
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
