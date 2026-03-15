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
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.IO;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    public class HTMLTagInfo
    {
        #region Private member

        private bool _isclosing = false;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the HTML tag.
        /// </summary>
        internal string TagName
        {
            get;
            set;
        }

        /// <summary>
        /// Attributes for the particular tag
        /// </summary>
        internal Dictionary<string, object> Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Whether it is a closing tag
        /// </summary>
        public bool IsCloseTag
        {
            get
            {
                return _isclosing;
            }
            set
            {
                _isclosing = true;
            }
        }

        /// <summary>
        /// Whether it is a single tag
        /// </summary>
        public bool IsSingle
        {
            get
            {
                return TagName.StartsWith("!")
                    || (new List<string>(
                            new string[]{
                             "area", "base", "basefont", "br", "col",
                             "frame", "hr", "img", "input", "isindex",
                             "link", "meta", "param"
                            }
                        )).Contains(TagName)
                    ;
            }
        }

        #endregion

        #region Constrctors

        /// <summary>
        /// Assign the Attributes list
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="attlist"></param>
        public HTMLTagInfo(string _name, Dictionary<string, object> attlist)
            : this(_name)
        {
            Attributes = attlist;
        }

        /// <summary>
        /// Process HtmlTag
        /// </summary>
        /// <param name="tag"></param>
        public HTMLTagInfo(string tag)
            : this()
        {
            ProcessTagName(tag);
            GatherAttributes(tag);
        }

        
        public HTMLTagInfo()
        {
            Attributes = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set attributes for the ParagaraphBox
        /// </summary>
        /// <param name="node"></param>
        internal void SetAttributes(HtmlNode node)
        {
            foreach (string att in Attributes.Keys)
            {
                object value = Attributes[att];
                try
                {
                    switch (att)
                    {
                        case HtmlConstants.Align:
                            node.TextAlign = value.ToString().Trim();
                            break;
                        case HtmlConstants.Background:
                            if (this.TagName == HtmlConstants.BodyTag)
                                node.BackgroundColor = value.ToString();
                            break;
                        case HtmlConstants.NavigationURL:
                            if (this.TagName == HtmlConstants.HyperlinkTag)
                            {
                                if (node.Hyperlink == null) node.Hyperlink = new HyperlinkAdv();
                                node.Hyperlink.NavigationUrl = value.ToString();
                            }
                            break;
                        case HtmlConstants.Target:
                            if (this.TagName == HtmlConstants.HyperlinkTag)
                            {
                                if (node.Hyperlink == null)
                                    node.Hyperlink = new HyperlinkAdv();
#if !WPF
                            if (value.ToString().ToLower() == "_blank")
                                node.Hyperlink.TargetType = HyperlinkTargetType.Blank;
                            else
                                node.Hyperlink.TargetType = HyperlinkTargetType.Self;
#endif
                            }
                            break;
                        case HtmlConstants.BgColor:
                            node.BackgroundColor = value.ToString();
                            break;
                        case HtmlConstants.Text:
                            if (this.TagName == HtmlConstants.BodyTag)
                                node.Color = value.ToString();
                            break;
                        case HtmlConstants.ImageSource:
                            if (this.TagName == HtmlConstants.ImageTag)
                            {
                                BitmapImage bitmap;
                                node.Image = new ImageContainerAdv();
                                if (value.ToString().Contains("base64,"))
                                {
                                    string base64str = value.ToString().Remove(0, value.ToString().IndexOf("base64,"));
                                    base64str = base64str.Replace("base64,", "");
                                   
                                    byte[] byteArray = Convert.FromBase64String(base64str);
                                    bitmap = new BitmapImage();
                                    MemoryStream ms = new MemoryStream(byteArray, 0, byteArray.Length);
                                    ms.Write(byteArray, 0, byteArray.Length);
#if !WPF
                                    bitmap.SetSource(ms);
#else
                                    bitmap.BeginInit();
                                    bitmap.StreamSource = ms;
                                    bitmap.EndInit();
#endif
                                }
                                else
                                {
                                    bitmap = new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute));
                                    node.Image.ImageSource = bitmap;
                                }
                            }
                            break;
                        case HtmlConstants.Color:
                            if (this.TagName == HtmlConstants.FontTag)
                                node.Color = value.ToString();
                            break;
                        case HtmlConstants.Fontfamily:
                            if (this.TagName == HtmlConstants.FontTag)
                                node.FontFamily = value.ToString();
                            break;
                        case HtmlConstants.Height:
                            if (this.TagName == HtmlConstants.ImageTag)
                                node.ImageHeight = node.ConvertSize(value.ToString(), double.Parse(node.ParentNode.FontSize, CultureInfo.InvariantCulture));
                            break;
                        case HtmlConstants.Size:
                            if (this.TagName == HtmlConstants.Size)
                                node.FontSize = value.ToString();
                            break;
                        case HtmlConstants.Width:
                            if (this.TagName == HtmlConstants.ImageTag)
                                node.ImageWidth = node.ConvertSize(value.ToString(), double.Parse(node.ParentNode.FontSize, CultureInfo.InvariantCulture));
                            break;
                        case HtmlConstants.RowSpanTag:
                            node.RowSpan = int.Parse(value.ToString());
                            break;
                        case HtmlConstants.ColumnSpanTag:
                            node.ColumnSpan = int.Parse(value.ToString());
                            break;
                        case HtmlConstants.BorderTag:
                            node.BorderThickness = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
                            break;
                        case HtmlConstants.ColumnGroupSpanTag:
                            node.ColGroupSpan = int.Parse(value.ToString());
                            break;
                        default:
                            break;

                    }
                }
                catch { }
            }
        }

        
        /// <summary>
        /// Assign the TagName information
        /// </summary>
        /// <param name="htmltag"></param>
        private void ProcessTagName(string htmltag)
        {
            htmltag = htmltag.Substring(1, htmltag.Length - 2);

            int spaceIndex = htmltag.IndexOf(" ");

            if (spaceIndex < 0)
            {
                TagName = htmltag;
            }
            else
            {
                TagName = htmltag.Substring(0, spaceIndex);
            }

            if (TagName.StartsWith("/"))
            {
                IsCloseTag = true;
                TagName = TagName.Substring(1);
            }

            TagName = TagName.ToLower();
        }

        /// <summary>
        /// Collect attributes from the htmlstring
        /// </summary>
        /// <param name="htmltag"></param>
        private void GatherAttributes(string htmltag)
        {
            MatchCollection atts = HtmlTagsParser.Match(HtmlTagsParser.HmlTagAttributes,
                (htmltag.StartsWith("<") && htmltag.EndsWith(">") && htmltag.Length > 2) ? 
                    htmltag.Substring(1, htmltag.Length - 2) : htmltag);

            foreach (Match att in atts)
            {
                int index = att.Value.IndexOf('=');
                string[] chunks = new string[2];
                chunks[0] = att.Value.Substring(0, index).Trim();
                chunks[1] = att.Value.Substring(index + 1).Trim();

                if (chunks.Length == 1)
                {
                    if (!Attributes.ContainsKey(chunks[0]))
                        Attributes.Add(chunks[0].ToLower(), string.Empty);
                }
                else if (chunks.Length >= 2)
                {
                    string attname = chunks[0].Trim().ToLower();
                    string attvalue = chunks[1];

                    if (attvalue.StartsWith("\"") && attvalue.EndsWith("\"") && attvalue.Length > 2)
                        attvalue = attvalue.Substring(1, attvalue.Length - 2);

                    if (!Attributes.ContainsKey(attname.ToLower()))
                        Attributes.Add(attname, attvalue);
                }
            }
        }

        /// <summary>
        /// Check whether Tag has Attributes
        /// </summary>
        internal bool HasAttribute(string attrname)
        {
             return  this.Attributes.ContainsKey(attrname);
        }
        #endregion
    }
}
        
