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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Root Box
    /// </summary>
    public class RootBox :ParagraphBox
    {

        #region Private Members

        HtmlAsciiCodesInfo htmlAsciicode;

        #endregion

        #region Properties
            
        /// <summary>
        /// It creates the paragarphs from the this RootBox tree
        /// </summary>
        internal List<ParagraphBox> Paragraphs
        {
            get;
            set;
        }

        /// <summary>
        /// It stores the CssProperties boxes from the Style tag
        /// </summary>
        internal Dictionary<string, CssPropertiesBox> StyleBoxes
        {
            get;
            set;
        }

        private List<ParagraphBox> TreeBoxes
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public RootBox(string htmlstring)
            :this()
        {
            htmlAsciicode = new HtmlAsciiCodesInfo();
            ParseTags(htmlstring);
            BoxesCorrection(this);
            FormattingBoxes(this);
            CascadeParagraphs();
        }

        public RootBox()
        {
            Paragraphs = new List<ParagraphBox>();
            StyleBoxes = new Dictionary<string, CssPropertiesBox>();
            TreeBoxes = new List<ParagraphBox>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create Boxes from the HTML string
        /// </summary>
        /// <param name="htmlstring"></param>
        internal void ParseTags(string htmlstring)
        {
            ParagraphBox root = this;
            ParagraphBox currentbox = root;
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
                            ParagraphTextBox abox = new ParagraphTextBox(currentbox, currentbox.HTMLTag);
                            abox.Text = text;
                            TreeBoxes.Add(abox);
                            int index = TreeBoxes.IndexOf(abox) - 1;
                            if (index != -1)
                                abox.PreviousBox = TreeBoxes[index];
                        }
                    }
                    if (Htmltag.IsCloseTag)
                        currentbox = FindParent(Htmltag.TagName, currentbox);
                    else if (Htmltag.IsSingle)
                    {
                        ParagraphBox singletagbox = new ParagraphBox(currentbox, Htmltag);
                        TreeBoxes.Add(singletagbox);
                    }
                    else
                    {
                        currentbox = new ParagraphBox(currentbox, Htmltag);
                        TreeBoxes.Add(currentbox);
                        int index = TreeBoxes.IndexOf(currentbox) - 1;
                        if (index != -1)
                            currentbox.PreviousBox = TreeBoxes[index];
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
        /// Create Paragraphs from the RootBox
        /// </summary>
        /// <param name="root">The root.</param>
        private void CascadeParagraphs()
        {
            ParagraphBox prghbox = null;
            for (int i = 0; i < TreeBoxes.Count; i++)
            {
                ParagraphBox box = TreeBoxes[i];

                if (box == null)
                    continue;

                if (prghbox == null)
                    prghbox = new ParagraphBox();

                if (box.HTMLTag != null && (AreContainerTags(box.HTMLTag.TagName) || AreListTags(box.HTMLTag.TagName)
                    || AreTableTags(box.HTMLTag.TagName) || AreSpanTags(box.HTMLTag.TagName)))
                {
                    i = CascadeBoxes(i, prghbox);
                    Paragraphs.Add(prghbox);
                    prghbox = null;
                }
                else if (box.GetType() == typeof(ParagraphBox) && box.HTMLTag != null && AreHeadingTags(box.HTMLTag.TagName))
                {
                    i = CascadeHeadingBoxes(i, prghbox);
                    Paragraphs.Add(prghbox);
                    prghbox = null;
                }
            }
        }


        private int CascadeBoxes(int index,ParagraphBox paragraphbox)
        {
            int boxindex = 0;
            for (int id = index; id < TreeBoxes.Count; id++)
            {
                ParagraphBox child = TreeBoxes[id];
                boxindex = id;
                if (child.NextBox != null && child.NextBox.HTMLTag!=null && (AreContainerTags(child.NextBox.HTMLTag.TagName) || AreHeadingTags(child.NextBox.HTMLTag.TagName)
                    || AreListTags(child.NextBox.HTMLTag.TagName) || AreTableTags(child.NextBox.HTMLTag.TagName)) && child.NextBox.GetType() != typeof(ParagraphTextBox))
                {
                    if(child.HTMLTag!=null) paragraphbox.Boxes.Add(child);
                    return id;
                }
                else
                    if (child.HTMLTag != null) paragraphbox.Boxes.Add(child);
            }
            return boxindex;
        }

        private int CascadeHeadingBoxes(int index, ParagraphBox headingbox)
        {
            ParagraphBox currentbox=TreeBoxes[index];

            foreach (ParagraphBox b in currentbox.Boxes)
            {
                if ((b.HTMLTag != null && b.GetType() == typeof(ParagraphTextBox) || b.HTMLTag.IsSingle))
                    headingbox.Boxes.Add(b);
                index++;
                CascadeHeadingBoxes(index, headingbox);
            }
            return index;
        }

        /// <summary>
        /// Apply the Inline style , External styles and Inherit the styles from the ParentBoxes
        /// </summary>
        /// <param name="root"></param>

        private void FormattingBoxes(ParagraphBox root)
        {
            foreach (ParagraphBox box in root.Boxes)
            {
                box.InheritParentStyle();

                if (box.HTMLTag!=null)
                {
                    if (StyleBoxes.ContainsKey(box.HTMLTag.TagName))
                        StyleBoxes[box.HTMLTag.TagName].AssignProperties(box);

                    if (box.HTMLTag.HasAttribute("class") && StyleBoxes.ContainsKey("."+ box.HTMLTag.Attributes["class"]))
                        StyleBoxes["." + box.HTMLTag.Attributes["class"]].AssignProperties(box);

                    if (box.HTMLTag.HasAttribute("id") && StyleBoxes.ContainsKey("#"+ box.HTMLTag.Attributes["id"]))
                        StyleBoxes["#" + box.HTMLTag.Attributes["id"]].AssignProperties(box);
                    
                    box.HTMLTag.SetAttributes(box);

                    if (box.HTMLTag.HasAttribute("style"))
                    {
                        CssPropertiesBox propertiesbox = new CssPropertiesBox(box.HTMLTag.Attributes["style"].ToString());
                        propertiesbox.AssignProperties(box);
                    }

                    if (box.HTMLTag.TagName=="style")
                        if(box.Text!=null)
                            GenerateCssPropertiesBoxes(box.Text);                  
                }
                FormattingBoxes(box);   
            }
        }

        /// <summary>
        /// Mapping the values from box to Paragraph 
        /// </summary>
        /// <param name="startBox"></param>
        /// <returns></returns>

        internal DocumentAdv AssignTo(ParagraphBox startBox)
        {
            DocumentAdv document = new DocumentAdv();
            SectionAdv section = new SectionAdv();
            ParagraphAdv paragraph = null;
            ParagraphBox tempbox = null;
            for (int i = 0; i < Paragraphs.Count; i++)
            {
                paragraph = new ParagraphAdv();
                for (int j = 0; j < Paragraphs[i].Boxes.Count; j++)
                {
                    tempbox = Paragraphs[i].Boxes[j];
                    if (tempbox is ParagraphTextBox || tempbox.HTMLTag.TagName == HtmlConstants.ImageTag)
                    {
                        if (paragraph == null)
                        {
                            paragraph = new ParagraphAdv();
                        }
                        paragraph.TextAlignment = StringToAlignment(tempbox.TextAlign);
                        paragraph.LeftIndent = tempbox.LeftIndent;
                        paragraph.ListType = tempbox.ListType;

                        if (AreSpanTags(tempbox.HTMLTag.TagName.ToLower())
                            || AreTableTags(tempbox.HTMLTag.TagName.ToLower()))
                        {
                            if (tempbox.Hyperlink == null)
                                paragraph.Inlines.Add(GetSpanAdv(tempbox));
                            else
                                paragraph.Inlines.Add(GetHyperlinkAdv(tempbox));
                        }
                        else if (AreContainerTags(tempbox.HTMLTag.TagName.ToLower()))
                        {
                            if (tempbox.Boxes.Count == 0 && tempbox is ParagraphTextBox)
                            {
                                paragraph.Inlines.Add(GetSpanAdv(tempbox));
                            }
                        }
                        else if(AreListTags(tempbox.HTMLTag.TagName.ToLower()))
                        {
                            paragraph.Inlines.Add(GetSpanAdv(tempbox));
                        }
                        else if (AreHeadingTags(tempbox.HTMLTag.TagName.ToLower()))
                        {
                            if (tempbox.ParentBox.Boxes.Count == 1)
                            {
                                paragraph = new ParagraphAdv();
                                paragraph.LeftIndent = tempbox.LeftIndent;
                            }
                            paragraph.TextAlignment = StringToAlignment(tempbox.TextAlign);
                            paragraph.Inlines.Add(GetSpanAdv(tempbox));
                            if (paragraph.Inlines.Count > 0) section.Blocks.Add(paragraph);
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
                }
                if (!(section.Blocks.Contains(paragraph)) && paragraph.Inlines.Count>0)
                {
                    section.Blocks.Add(paragraph);
                }
            }
            document.Sections.Add(section);
            return document;
        }

        /// <summary>
        /// It corrects the boxes based upon the Tag information
        /// </summary>
        /// <param name="home"></param>
        internal void BoxesCorrection(ParagraphBox home)
        {
            foreach (ParagraphBox box in home.Boxes)
            {
                if (box.HTMLTag != null)
                {
                    switch (box.HTMLTag.TagName.ToLower())
                    {
                        case HtmlConstants.BoldTag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            break;
                        case HtmlConstants.ItalicTag:
                            box.FontSyle = HtmlConstants.FontStyle_Italic;
                            break;
                        case HtmlConstants.VarTag:
                            box.FontSyle = HtmlConstants.FontStyle_Italic;
                            break;
                        case HtmlConstants.UnderlineTag:
                            box.Underline = true;
                            break;
                        case HtmlConstants.CodeTag:
                            box.FontFamily = "Courier New";
                            box.FontWeight = HtmlConstants.FontWeight_SemiBold;
                            break;
                        case HtmlConstants.OLTag:
                            box.ListType = ListType.Numbered;
                            break;
                        case HtmlConstants.ULTag:
                            box.ListType = ListType.Bulleted;
                            break;
                        case HtmlConstants.SubTag:
                            box.BaseLine = Baseline.Subscript;
                            break;
                        case HtmlConstants.H1Tag:
                            box.FontWeight = HtmlConstants.FontWeight_ExtraBold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "24.0";
                            break;
                        case HtmlConstants.H2Tag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "20.0";
                            break;
                        case HtmlConstants.H3Tag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "16.0";
                            break;
                        case HtmlConstants.H4Tag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "12.0";
                            break;
                        case HtmlConstants.H5Tag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "10.0";
                            break;
                        case HtmlConstants.H6Tag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "8.0";
                            break;
                        case HtmlConstants.H7Tag:
                            box.FontWeight = HtmlConstants.FontWeight_Bold;
                            box.FontSyle = HtmlConstants.FontStyle_Normal;
                            if (!HasFontSize(box))
                                box.FontSize = "6.0";
                            break;
                        case HtmlConstants.SupTag:
                            box.BaseLine = Baseline.Superscript;
                            break;
                        case HtmlConstants.StrongTag:
                            box.FontWeight = HtmlConstants.FontWeight_ExtraBold;
                            break;
                        case HtmlConstants.SmallTag:
                            if (double.Parse(box.FontSize) < double.Parse("0.0"))
                                box.FontSize = "11.0";
                            box.FontSize = (double.Parse(box.FontSize) - 2).ToString();
                            break;
                        case HtmlConstants.BigTag:
                            box.FontSize += 2;
                            break;
                        case HtmlConstants.SampTag:
                            box.FontSize = "11.0";
                            box.FontFamily = "Courier New";
                            break;
                        case HtmlConstants.CenterTag:
                            box.TextAlign = HtmlConstants.Center;
                            FormattingBoxes(box);
                            break;
                        case HtmlConstants.LeftTag:
                            box.TextAlign = HtmlConstants.Left;
                            FormattingBoxes(box);
                            break;
                        default:
                            break;
                    }
                    BoxesCorrection(box);
                }
            }
           
        }

        /// <summary>
        /// Generate the spandadv from the particular box
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal SpanAdv GetSpanAdv(ParagraphBox box)
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
            spanadv.FontSize = double.Parse(box.FontSize);
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
        internal HyperlinkAdv GetHyperlinkAdv(ParagraphBox box)
        {
            HyperlinkAdv hyperlinkadv = null;
            hyperlinkadv = new HyperlinkAdv();
            if(box.Hyperlink==null)box.Hyperlink = new HyperlinkAdv();
            hyperlinkadv.Text = box.Text;
            hyperlinkadv.Text = Regex.Replace(hyperlinkadv.Text, HtmlTagsParser.WhiteSpaces, " ");
            hyperlinkadv.Text = EncodeHtmlNames(hyperlinkadv.Text);
            if (hyperlinkadv.Text.Contains("\n"))
                hyperlinkadv.Text = hyperlinkadv.Text.Replace("\n", " ");
            hyperlinkadv.NavigationUrl = box.Hyperlink.NavigationUrl;
            hyperlinkadv.FontFamily = new System.Windows.Media.FontFamily(box.FontFamily);
            hyperlinkadv.FontSize = double.Parse(box.FontSize);
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
        internal ImageContainerAdv GetImageContainerAdv(ParagraphBox imagebox)
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
                if (c == ';')
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

        private bool HasFontSize(ParagraphBox box)
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
        internal ParagraphBox FindParent(string name,ParagraphBox b)
        {
            if (b == null)
            {
                return this;
            }
            else if (b.HTMLTag != null && b.HTMLTag.TagName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                return b.ParentBox == null ? this : b.ParentBox;
            }
            else
            {
                return FindParent(name, b.ParentBox);
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
