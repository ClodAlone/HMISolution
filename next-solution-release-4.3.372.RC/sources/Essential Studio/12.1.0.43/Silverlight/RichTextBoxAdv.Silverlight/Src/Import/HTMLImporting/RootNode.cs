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
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

#if !WPF
using System.Windows.Browser;
#endif

using System.Text;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Root Box
    /// </summary>
    public class RootNode :HtmlNode
    {

        #region Private Members

        HtmlAsciiCodesInfo htmlAsciicode;

        #endregion

        #region Properties
            
        /// <summary>
        /// It stores the CssProperties boxes from the Style tag
        /// </summary>
        internal Dictionary<string, CssPropertiesBox> StyleBoxes
        {
            get;
            set;
        }

        private List<HtmlNode> TreeNodes
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public RootNode(string htmlstring)
            :this()
        {
            htmlAsciicode = new HtmlAsciiCodesInfo();
            ParseTags(htmlstring);
            BoxesCorrection(this);
            FormattingBoxes(this);
        }

        public RootNode()
        {
            StyleBoxes = new Dictionary<string, CssPropertiesBox>();
            TreeNodes = new List<HtmlNode>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create Boxes from the HTML string
        /// </summary>
        /// <param name="htmlstring"></param>
        internal void ParseTags(string htmlstring)
        {
            HtmlNode currentbox = this;
            int intend = -1;
            MatchCollection matches = HtmlTagsParser.Match(HtmlTagsParser.HtmlTag, htmlstring);

            if (matches.Count > 0)
            {
                string text = string.Empty;

                foreach (Match matchedtag in matches)
                {
                    if (matchedtag.Index > 0)
                        text = htmlstring.Substring(intend + 1, matchedtag.Index - intend - 1);
                    else
                        text = string.Empty;

                    HTMLTagInfo Htmltag = new HTMLTagInfo(matchedtag.Value);

                    if (!string.IsNullOrEmpty(text.Trim()))
                    {
                        if (!ContainsUnSupportedTags(Htmltag.TagName))
                        {
                            HtmlTextNode abox = new HtmlTextNode(currentbox,currentbox.HTMLTag);
                            abox.Text = text;
                        }
                    }
                    if (Htmltag.IsSingle)
                    {
                        HtmlNode singletagbox;
                        //Skips the addition of new HtmlNode for single node close tags like "</img>".
                        if (!Htmltag.IsCloseTag)
                            singletagbox = new HtmlNode(currentbox, Htmltag);
                    }
                    else
                    {
                        if (Htmltag.IsCloseTag)
                            //Updates the end of current HtmlNode and sets it parent node as current HtmlNode.
                            currentbox = FindParent(Htmltag.TagName, currentbox);
                        else
                            //Creates new HtmlNode and sets it as current HtmlNode.
                            currentbox = new HtmlNode(currentbox, Htmltag);
                    }
                    intend = matchedtag.Index + matchedtag.Length - 1;
                }
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("<span>");
                builder.Append(htmlstring);
                builder.Append("</span>");
                ParseTags(builder.ToString());
            }
        }

        /// <summary>
        /// Apply the Inline style , External styles and Inherit the styles from the ParentBoxes
        /// </summary>
        /// <param name="root"></param>

        private void FormattingBoxes(HtmlNode root)
        {
            foreach (HtmlNode node in root.Nodes)
            {
                node.InheritParentStyle();

                if (node.HTMLTag!=null)
                {
                    if (StyleBoxes.ContainsKey(node.HTMLTag.TagName))
                        StyleBoxes[node.HTMLTag.TagName].AssignProperties(node);

                    if (node.HTMLTag.HasAttribute("class") && StyleBoxes.ContainsKey("."+ node.HTMLTag.Attributes["class"]))
                        StyleBoxes["." + node.HTMLTag.Attributes["class"]].AssignProperties(node);

                    if (node.HTMLTag.HasAttribute("id") && StyleBoxes.ContainsKey("#"+ node.HTMLTag.Attributes["id"]))
                        StyleBoxes["#" + node.HTMLTag.Attributes["id"]].AssignProperties(node);

                    if (node.HTMLTag.HasAttribute("class") && StyleBoxes.ContainsKey(node.HTMLTag.TagName + "." + node.HTMLTag.Attributes["class"]))
                        StyleBoxes[node.HTMLTag.TagName + "." + node.HTMLTag.Attributes["class"]].AssignProperties(node);

                    if (node.HTMLTag.TagName == HtmlConstants.TableCellDataTag ||
                        node.HTMLTag.TagName == HtmlConstants.TableCellHeaderTag)
                    {
                        HtmlNode tablenode=node.RootTableNode();
                        if (tablenode.HTMLTag.HasAttribute("class") && StyleBoxes.ContainsKey(tablenode.HTMLTag.TagName + "." + tablenode.HTMLTag.Attributes["class"] + " " + node.HTMLTag.TagName))
                        {
                            StyleBoxes[tablenode.HTMLTag.TagName + "." + tablenode.HTMLTag.Attributes["class"] + " " + node.HTMLTag.TagName].AssignProperties(node);
                        }
                    }
                    
                    node.HTMLTag.SetAttributes(node);

                    if (node.HTMLTag.HasAttribute("style"))
                    {
                        CssPropertiesBox propertiesbox = new CssPropertiesBox(node.HTMLTag.Attributes["style"].ToString());
                        propertiesbox.AssignProperties(node);
                    }

                    if (node.HTMLTag.TagName=="style")
                        if(node.Text!=null)
                            GenerateCssPropertiesBoxes(node.Text);                  
                }
                FormattingBoxes(node);   
            }
        }

        /// <summary>
        /// Mapping the values from box to Paragraph 
        /// </summary>
        /// <param name="startBox"></param>
        /// <returns></returns>

        internal DocumentAdv AssignTo(HtmlNode startBox)
        {
            DocumentAdv document = new DocumentAdv();
            SectionAdv section = new SectionAdv();
            AssignNodes(this,ref section);
            document.Sections.Add(section);
            return document;
        }

        internal void AssignNodes(HtmlNode htmlnode,ref SectionAdv section)
        {
            BlockAdv block;
            for (int i = 0; i < htmlnode.Nodes.Count; i++)
            {
                block=null;
                HtmlNode node = htmlnode.Nodes[i];
                if (node.HTMLTag.TagName == HtmlConstants.TableTag)
                {
                    block = AssignTableNodes(node);
                }
                else
                {
                    block = AssignParagraphNodes(node);
                }
                if (block !=null && !(section.Blocks.Contains(block)) && !block.IsEmpty &&
                    !block.IsInsideTable)
                {
                    section.Blocks.Add(block);
                }
                else if (node.HTMLTag.TagName != HtmlConstants.TableTag)
                {
                    AssignNodes(node, ref section);
                }
            }
        }
        /// <summary>
        /// Assign Paragraph nodes
        /// </summary>
        /// <param name="paragraphnode"></param>
        /// <returns></returns>
        internal BlockAdv AssignParagraphNodes(HtmlNode paragraphnode)
        {
            ParagraphAdv paragraph = null;
            HtmlNode tempbox = null;
            tempbox = paragraphnode;
            paragraph = new ParagraphAdv();
            paragraph.TextAlignment = StringToAlignment(tempbox.TextAlign);
            paragraph.LeftIndent = tempbox.LeftIndent;
            paragraph.RightIndent = tempbox.RightIndent;
            paragraph.AfterSpacing = tempbox.AfterSpacing;
            paragraph.BeforeSpacing = tempbox.BeforeSpacing;
            paragraph.ListType = tempbox.ListType;
            if (tempbox is HtmlTextNode)
            {
                if (AreSpanTags(tempbox.HTMLTag.TagName.ToLower()) ||
                    tempbox.HTMLTag.TagName == HtmlConstants.TableCellDataTag ||
                    tempbox.HTMLTag.TagName == HtmlConstants.TableCellHeaderTag)
                {
                    if (tempbox.HTMLTag.TagName == HtmlConstants.TableCellHeaderTag)
                    {
                        tempbox.FontWeight = "Bold";
                        tempbox.FontSize = "16";
                    }
                    if (tempbox.Hyperlink == null)
                        paragraph.Inlines.Add(GetSpanAdv(tempbox));
                    else
                        paragraph.Inlines.Add(GetHyperlinkAdv(tempbox));
                }
                else if (AreContainerTags(tempbox.HTMLTag.TagName.ToLower()))
                {
                    if (tempbox.Nodes.Count == 0 && tempbox is HtmlTextNode)
                    {
                        paragraph.Inlines.Add(GetSpanAdv(tempbox));
                    }
                }
                else if (AreListTags(tempbox.HTMLTag.TagName.ToLower()))
                {
                    paragraph.Inlines.Add(GetSpanAdv(tempbox));
                }
                else if (AreHeadingTags(tempbox.HTMLTag.TagName.ToLower()))
                {
                    if (tempbox.ParentNode.Nodes.Count == 1)
                    {
                        paragraph = new ParagraphAdv();
                        paragraph.LeftIndent = tempbox.LeftIndent;
                    }
                    paragraph.TextAlignment = StringToAlignment(tempbox.TextAlign);
                    paragraph.Inlines.Add(GetSpanAdv(tempbox));
                    return paragraph;
                }
                else if (tempbox.HTMLTag.TagName == HtmlConstants.ImageTag)
                {
                    paragraph.Inlines.Add(GetImageContainerAdv(tempbox));
                }
                else if (tempbox.HTMLTag.TagName == HtmlConstants.HyperlinkTag)
                {
                    paragraph.Inlines.Add(GetHyperlinkAdv(tempbox));
                }
            }
            else if (tempbox.HTMLTag.TagName == HtmlConstants.ParagraphTag)
            {
                paragraph = (ParagraphAdv)AssignSpanNodes(tempbox, paragraph);
            }

            return paragraph;
        }
        /// <summary>
        /// Assign span nodes to the paragraph block
        /// </summary>
        /// <param name="paragraphnode"></param>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal BlockAdv AssignSpanNodes(HtmlNode paragraphnode, ParagraphAdv paragraph)
        {
            HtmlNode tempbox = null;
            for (int j = 0; j < paragraphnode.Nodes.Count; j++)
            {
                tempbox = paragraphnode.Nodes[j];
                if (tempbox is HtmlTextNode)
                {
                    if (AreSpanTags(tempbox.HTMLTag.TagName.ToLower()) ||
                        tempbox.HTMLTag.TagName == HtmlConstants.TableCellDataTag ||
                        tempbox.HTMLTag.TagName == HtmlConstants.TableCellHeaderTag)
                    {
                        if (tempbox.HTMLTag.TagName == HtmlConstants.TableCellHeaderTag)
                        {
                            tempbox.FontWeight = "Bold";
                            tempbox.FontSize = "16";
                        }
                        if (tempbox.Hyperlink == null)
                            paragraph.Inlines.Add(GetSpanAdv(tempbox));
                        else
                            paragraph.Inlines.Add(GetHyperlinkAdv(tempbox));
                    }
                    else if (AreContainerTags(tempbox.HTMLTag.TagName.ToLower()))
                    {
                        if (tempbox.Nodes.Count == 0 && tempbox is HtmlTextNode)
                        {
                            paragraph.Inlines.Add(GetSpanAdv(tempbox));
                        }
                    }
                    else if (AreListTags(tempbox.HTMLTag.TagName.ToLower()))
                    {
                        paragraph.Inlines.Add(GetSpanAdv(tempbox));
                    }
                    else if (AreHeadingTags(tempbox.HTMLTag.TagName.ToLower()))
                    {
                        if (tempbox.ParentNode.Nodes.Count == 1)
                        {
                            paragraph = new ParagraphAdv();
                            paragraph.LeftIndent = tempbox.LeftIndent;
                        }
                        paragraph.TextAlignment = StringToAlignment(tempbox.TextAlign);
                        paragraph.Inlines.Add(GetSpanAdv(tempbox));
                        return paragraph;
                    }
                    else if (tempbox.HTMLTag.TagName == HtmlConstants.ImageTag)
                    {
                        paragraph.Inlines.Add(GetImageContainerAdv(tempbox));
                    }
                    else if (tempbox.HTMLTag.TagName == HtmlConstants.HyperlinkTag)
                    {
                        paragraph.Inlines.Add(GetHyperlinkAdv(tempbox));
                    }
                }
                else if (tempbox.HTMLTag.TagName == HtmlConstants.ImageTag)
                {
                    paragraph.Inlines.Add(GetImageContainerAdv(tempbox));
                }
                else
                {
                    paragraph = (ParagraphAdv)AssignSpanNodes(tempbox, paragraph);
                }
            }
            return paragraph;
        }

        internal void AssignTableCellNodes(HtmlNode cellnode,ref TableCellAdv cell)
        {
            BlockAdv block = null;
            for (int i = 0; i < cellnode.Nodes.Count; i++)
            {
                block = null;
                HtmlNode node = cellnode.Nodes[i];
                if (node.HTMLTag.TagName == HtmlConstants.TableTag)
                {
                    block = AssignTableNodes(node);
                }
                else
                {
                    block = AssignParagraphNodes(node);
                }
                if (block != null && !(cell.Blocks.Contains(block)) && !block.IsEmpty)
                {
                    cell.Blocks.Add(block);
                }
                else if (node.HTMLTag.TagName != HtmlConstants.TableTag)
                {
                    AssignTableCellNodes(node, ref cell);
                }
            }
        }

        internal BlockAdv AssignTableNodes(HtmlNode tablenode)
        {
            TableAdv table = null;
            if (tablenode.HTMLTag.TagName == HtmlConstants.TableTag)
            {
                table = new TableAdv();
            }
            table.BorderThickness = tablenode.BorderThickness;
            table.Background = tablenode.ActualBackgroundColor;
            foreach (HtmlNode node in tablenode.Nodes)
            {
                if (node.HTMLTag.TagName == HtmlConstants.TableRowTag)
                {
                    AssignTableRowNodes(node, table);
                }
                else if (node.HTMLTag.TagName == HtmlConstants.TableHeaderTag ||
                    node.HTMLTag.TagName == HtmlConstants.TableBodyTag ||
                    node.HTMLTag.TagName == HtmlConstants.TableFooterTag)
                {
                    foreach (HtmlNode node3 in node.Nodes)
                    {
                        if (node3.HTMLTag.TagName == HtmlConstants.TableRowTag)
                        {
                            AssignTableRowNodes(node3, table);
                        }
                    }
                }
            }
            SetColGroupValues(table, tablenode);
            return table;
        }

        internal void AssignTableRowNodes(HtmlNode node,TableAdv table)
        {
            TableRowAdv row = new TableRowAdv();
            row.Background = node.ActualBackgroundColor;
            row.TextAlignment = StringToAlignment(node.TextAlign);
            foreach (HtmlNode node2 in node.Nodes)
            {
                TableCellAdv cell = null;
                if (node2.HTMLTag.TagName == HtmlConstants.TableCellHeaderTag ||
                    node2.HTMLTag.TagName == HtmlConstants.TableCellDataTag)
                {
                    cell = new TableCellAdv();
                    AssignTableCellNodes(node2, ref cell);
                }
                if (cell != null)
                {
                    cell.ColumnSpan = node2.ColumnSpan;
                    cell.RowSpan = node2.RowSpan;
                    cell.Background = node2.ActualBackgroundColor;
                    cell.TextAlignment = StringToAlignment(node2.TextAlign);
                    row.Cells.Add(cell);
                }
            }
            table.Rows.Add(row);
        }

        internal void SetColGroupValues(TableAdv table,HtmlNode tablenode)
        {
            List<HtmlNode> columnNodes = new List<HtmlNode>();
            //table.MeasureElements();
            foreach (HtmlNode node in tablenode.Nodes)
            {
                if (node.HTMLTag.TagName == HtmlConstants.ColumnTag)
                {
                    columnNodes.Add(node);
                }
                else if (node.HTMLTag.TagName == HtmlConstants.ColumnGroupTag)
                {
                    if (node.Nodes.Count > 0)
                    {
                        foreach (HtmlNode node2 in node.Nodes)
                        {
                            if (node2.HTMLTag.TagName == HtmlConstants.ColumnTag)
                            {
                                columnNodes.Add(node2);
                            }
                        }
                    }
                    else
                    {
                        columnNodes.Add(node);
                    }
                }
            }
            table.SetCellColumnIndexs();
            int i = 0;
            foreach (HtmlNode node3 in columnNodes)
            {
                int value = i + node3.ColGroupSpan;
                while (i < value)
                {
                    foreach (TableRowAdv row in table.Rows)
                    {
                        foreach (TableCellAdv cell in row.Cells)
                        {
                            cell.OwnerRow = row;
                            if (cell.ColumnIndex == i)
                            {
                                cell.ColumnBackground = node3.ActualBackgroundColor;
                                cell.ColumnAlignment = StringToAlignment(node3.TextAlign);
                            }
                        }
                    }
                    i++;
                }
            }
        }

        private bool IsEmpty(Color color)
        {
            return Math.Abs(color.R) == 0 && Math.Abs(color.G) == 0 && Math.Abs(color.B) == 0
                && Math.Abs(color.A) == 0;
        }
        
        /// <summary>
        /// It corrects the boxes based upon the Tag information
        /// </summary>
        /// <param name="home"></param>
        internal void BoxesCorrection(HtmlNode home)
        {
            foreach (HtmlNode node in home.Nodes)
            {
                if (node.HTMLTag != null)
                {
                    switch (node.HTMLTag.TagName.ToLower())
                    {
                        case HtmlConstants.BoldTag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            break;
                        case HtmlConstants.ItalicTag:
                            node.FontSyle = HtmlConstants.FontStyle_Italic;
                            break;
                        case HtmlConstants.VarTag:
                            node.FontSyle = HtmlConstants.FontStyle_Italic;
                            break;
                        case HtmlConstants.UnderlineTag:
                            node.Underline = true;
                            break;
                        case HtmlConstants.CodeTag:
                            node.FontFamily = "Courier New";
                            node.FontWeight = HtmlConstants.FontWeight_SemiBold;
                            break;
                        case HtmlConstants.OLTag:
                            node.ListType = ListType.Numbered;
                            break;
                        case HtmlConstants.ULTag:
                            node.ListType = ListType.Bulleted;
                            break;
                        case HtmlConstants.SubTag:
                            node.BaseLine = Baseline.Subscript;
                            break;
                        case HtmlConstants.H1Tag:
                            node.FontWeight = HtmlConstants.FontWeight_ExtraBold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "24";
                            break;
                        case HtmlConstants.H2Tag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "20";
                            break;
                        case HtmlConstants.H3Tag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "16";
                            break;
                        case HtmlConstants.H4Tag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "12";
                            break;
                        case HtmlConstants.H5Tag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "10";
                            break;
                        case HtmlConstants.H6Tag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "8";
                            break;
                        case HtmlConstants.H7Tag:
                            node.FontWeight = HtmlConstants.FontWeight_Bold;
                            node.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(node))
                                node.FontSize = "6";
                            break;
                        case HtmlConstants.SupTag:
                            node.BaseLine = Baseline.Superscript;
                            break;
                        case HtmlConstants.StrongTag:
                            node.FontWeight = HtmlConstants.FontWeight_ExtraBold;
                            break;
                        case HtmlConstants.SmallTag:
                            if (double.Parse(node.FontSize, CultureInfo.InvariantCulture) < double.Parse("0"))
                                node.FontSize = "11";
                            node.FontSize = (double.Parse(node.FontSize, CultureInfo.InvariantCulture) - 2).ToString(CultureInfo.InvariantCulture);
                            break;
                        case HtmlConstants.BigTag:
                            node.FontSize += 2;
                            break;
                        case HtmlConstants.SampTag:
                            node.FontSize = "11";
                            node.FontFamily = "Courier New";
                            break;
                        case HtmlConstants.CenterTag:
                            node.TextAlign = HtmlConstants.Center;
                            FormattingBoxes(node);
                            break;
                        case HtmlConstants.LeftTag:
                            node.TextAlign = HtmlConstants.Left;
                            FormattingBoxes(node);
                            break;
                        default:
                            break;
                    }
                    BoxesCorrection(node);
                }
            }
           
        }

        /// <summary>
        /// Generate the spandadv from the particular box
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal SpanAdv GetSpanAdv(HtmlNode box)
        {
            SpanAdv spanadv = null;
            spanadv = new SpanAdv();
            if (box.Text == null)
                box.Text = string.Empty;
            spanadv.Text = box.Text;
            spanadv.Text = Regex.Replace(spanadv.Text, HtmlTagsParser.WhiteSpaces," ");
            spanadv.Text = EncodeHtmlNames(spanadv.Text);
            spanadv.FontWeight = box.ActualFontWeight;
            spanadv.FontStyle = box.StringToFontStyle(box.FontSyle);
            spanadv.FontSize = double.Parse(box.FontSize, CultureInfo.InvariantCulture);
            spanadv.Foreground = box.Foreground;
            spanadv.FontFamily = new System.Windows.Media.FontFamily(box.FontFamily);
            spanadv.HighlightColor = box.ActualBackgroundColor;
            spanadv.StrikeThrough = box.StrikeThrough;
            spanadv.Underline = box.Underline;
            spanadv.Baseline = box.BaseLine;
            if (box.FontVariant == HtmlConstants.SmallCaps)
                spanadv.Text = spanadv.Text.ToUpper();
            spanadv.Text=EncodeAsciiSymbol(spanadv.Text);
            return spanadv;
        }

        /// <summary>
        /// Generate the HyperlinkAdv from the particular box
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal HyperlinkAdv GetHyperlinkAdv(HtmlNode box)
        {
            HyperlinkAdv hyperlinkadv = null;
            hyperlinkadv = new HyperlinkAdv();
            if(box.Hyperlink==null)box.Hyperlink = new HyperlinkAdv();
            hyperlinkadv.Text = box.Text == null ? string.Empty : box.Text;
            if (hyperlinkadv.Text != null)
            {
                hyperlinkadv.Text = Regex.Replace(hyperlinkadv.Text, HtmlTagsParser.WhiteSpaces, " ");
                hyperlinkadv.Text = EncodeHtmlNames(hyperlinkadv.Text);
                if (hyperlinkadv.Text.Contains("\n"))
                    hyperlinkadv.Text = hyperlinkadv.Text.Replace("\n", " ");
            }
            hyperlinkadv.NavigationUrl = box.Hyperlink.NavigationUrl;
            hyperlinkadv.FontFamily = new System.Windows.Media.FontFamily(box.FontFamily);
            hyperlinkadv.FontSize = double.Parse(box.FontSize, CultureInfo.InvariantCulture);
            hyperlinkadv.FontStyle = box.StringToFontStyle(box.FontSyle);
            hyperlinkadv.FontWeight = box.ActualFontWeight;
            hyperlinkadv.Foreground = box.HyperLinkColor;
            hyperlinkadv.Baseline = box.BaseLine;
            hyperlinkadv.Text = EncodeAsciiSymbol(hyperlinkadv.Text);
            return hyperlinkadv;
        }

        /// <summary>
        /// Generate the ImageContainerAdv from the particular box.
        /// </summary>
        /// <param name="imagebox"></param>
        /// <returns></returns>
        internal ImageContainerAdv GetImageContainerAdv(HtmlNode imagebox)
        {
            ImageContainerAdv image = null;
            image = new ImageContainerAdv();
            if (imagebox.Image == null)
                imagebox.Image = new ImageContainerAdv();

            if ((image.ImageSource != null && (image.ImageSource as BitmapImage).UriSource.OriginalString.Contains("http://")) || image.ImageSource == null)
            {
#if !WPF
                StreamResourceInfo stream = Application.GetResourceStream(new Uri(@"Syncfusion.RichTextBoxAdv.Silverlight;component/Images/Default.png", UriKind.Relative));
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.SetSource(stream.Stream);
                image.ImageSource = bitmapimage;
#endif
                string imageSource = imagebox.HTMLTag.Attributes["src"].ToString();

                if (imageSource.Contains("base64"))
                {
                    imageSource = imageSource.Replace("data:image/png;base64,", "");
                    if (imageSource.EndsWith(">"))
                        imageSource = imageSource.Substring(1, imageSource.Length - 3);
                    byte[] btyeArr = Convert.FromBase64String(imageSource);

                    image.ImageBytes=btyeArr;

                    var bmp = new BitmapImage();
#if WPF
                    bmp.BeginInit();
#endif
                    bmp.SetSource(new MemoryStream(btyeArr));
#if WPF
                    bmp.EndInit();
#endif
                    image.ImageSource = bmp;
                    image.Width = bmp.PixelWidth;
                    image.Height = bmp.PixelHeight;
                }

                else if (Uri.IsWellFormedUriString(imageSource, UriKind.RelativeOrAbsolute))
                {
                    image.ImageSource = new BitmapImage(new Uri(imageSource, UriKind.RelativeOrAbsolute));
                    image.Height = imagebox.ImageHeight;
                    image.Width = imagebox.ImageWidth;
                }
            }

            return image;
        }

        /// <summary>
        /// Insert the Ascii symbol based on the HtmlCodeTable
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string EncodeAsciiSymbol(string text)
        {
            if (text == null)
                return null;
            List<string> htmlnumbers = new List<string>();
            bool start = false;
            char[] cArry = new char[7];
            StringBuilder builder = new StringBuilder();
            int i = 0;
            int j = 0;
            foreach (char c in text)
            {
                if (c == '&')
                    if (text[text.IndexOf(c) + 1] == '#')
                        start = true;
                
                if (start)
                {
                    //if(i < cArry.Length)
                    //    cArry[i] = c;
                    builder.Append(c);
                    i++;
                }
                if (c == ';' && start == true)
                {
                    start = false;
                    i = 0;
                    htmlnumbers.Add(builder.ToString());
                    j++;
                }
            }

            foreach (string htmlno in htmlnumbers)
            {
                if(htmlAsciicode.HtmlAsciiCodeTable.Keys.Contains(htmlno))
                    text = text.Replace(htmlno, htmlAsciicode.HtmlAsciiCodeTable[htmlno]);
                else
                    text = text.Replace(htmlno, " ");
            }

            return text;
        }

        private string EncodeHtmlNames(string nametext)
        {
            foreach (string name in htmlAsciicode.HtmlNameTable.Keys)
            {
                if (nametext.Contains(name))
                    nametext = nametext.Replace(name, htmlAsciicode.HtmlNameTable[name]);
            }
            return nametext;
        }

        #region Boolean Methods

        /// <summary>
        /// Check whether the tags are ListTags
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool AreListTags(string name)
        {
            return name ==HtmlConstants.OLTag || name==HtmlConstants.LiTag || name == HtmlConstants.ULTag || name == HtmlConstants.DLTag || name == HtmlConstants.DDTag || name == HtmlConstants.DTTag;
        }

        private bool ContainsUnSupportedTags(string name)
        {
            return name.ToLower().Contains(HtmlConstants.LeftCommentSymbol) || name.ToLower().Contains(HtmlConstants.RightCommentSymbol) || name.ToLower().Contains(HtmlConstants.DocTypeComment);
        }

        private bool HasFontSize(HtmlNode box)
        {
            if(StyleBoxes.Count != 0)
            {
                if(StyleBoxes.ContainsKey(box.HTMLTag.TagName))
                    if (StyleBoxes[box.HTMLTag.TagName].Properties.ContainsKey("font-size"))
                        return true;
                else if (box.HTMLTag.HasAttribute("class"))
                    if (StyleBoxes["." + box.HTMLTag.Attributes["class"]].Properties.ContainsKey("font-size"))
                        return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Find the parent for the Box when we find ClosingTag
        /// </summary>
        /// <param name="name"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal HtmlNode FindParent(string name,HtmlNode b)
        {
            if (b == null)
            {
                return this;
            }
            else if (b.HTMLTag != null && b.HTMLTag.TagName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                return b.ParentNode == null ? this : b.ParentNode;
            }
            else
            {
                return FindParent(name, b.ParentNode);
            }   
        }

        /// <summary>
        /// Generate CssPropertiesBox from the Style Tag's string.
        /// </summary>
        /// <param name="stylestring"></param>
        internal void GenerateCssPropertiesBoxes(string stylestring)
        {
            MatchCollection matches = HtmlTagsParser.Match(HtmlTagsParser.CssPropertyBox, stylestring);
            foreach (Match match in matches)
            {
                if (match!=null)
                {
                    FeedPropertiesBox(match.Value);                    
                }
            }
        }

        /// <summary>
        /// Stores the Properties in the CssPropertiesBoxes
        /// </summary>
        /// <param name="boxstring"></param>
        internal void FeedPropertiesBox(string boxstring)
        {
            int bracketIndex = boxstring.IndexOf("{");
            string boxSource = boxstring.Substring(bracketIndex).Replace("{", string.Empty).Replace("}", string.Empty);

            if (bracketIndex < 0) return;

            string[] classes = boxstring.Substring(0, bracketIndex).Split(',');

            for (int i = 0; i < classes.Length; i++)
            {
                string className = classes[i].Trim().ToLower(); if (string.IsNullOrEmpty(className)) continue;

                CssPropertiesBox newbox = new CssPropertiesBox(boxSource);

                if(!(StyleBoxes.ContainsKey(className)))
                    StyleBoxes.Add(className, newbox);
                else
                {
                    CssPropertiesBox ancient =StyleBoxes[className];

                    foreach (string property in newbox.Properties.Keys)
                    {
                        if (ancient.Properties.ContainsKey(property))                      
                            ancient.Properties[property] = newbox.Properties[property];                   
                        else
                            ancient.Properties.Add(property, newbox.Properties[property]);
                    }

                    ancient.UpdatePropertyValues();
                }
            }
        }
        #endregion

    }
}
