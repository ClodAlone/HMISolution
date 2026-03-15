#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

using Syncfusion.Pdf.Native;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Internal class which is used converts the html string in to Rich Text.
    /// </summary>
    [ToolboxItem(false)]
    internal class RichTextBoxExt : RichTextBox
    {

#region Fields
        /// <summary>
        /// Internal variable to store the status of the RichTextBox control.
        /// </summary>
        private int updating = 0;

        /// <summary>
        /// Internal variable.
        /// </summary>
        private int oldEventMask = 0;

        /// <summary>
        /// Internal variable used while parsing the html text.
        /// </summary>
        private string lastTag = " ";

        /// <summary>
        /// Internal variable to check for nested tag.
        /// </summary>
        private bool isNested;

        /// <summary>
        /// Dictionary containg list of support html tags.
        /// </summary>
        Dictionary<string, int> m_htmlDictionary;

        /// <summary>
        /// Internal variable to specify font.
        /// </summary>
        private Font m_Font = null;

        /// <summary>
        /// Internal varible to specify the color.
        /// </summary>
        private PdfColor m_Color;

        /// <summary>
        /// Internal variable to store copy of m_color.
        /// </summary>
        private PdfColor colors;

        /// <summary>
        /// Internal variable to store the PDF font size;
        /// </summary>
        private int m_pdfFontHeight;

        /// <summary>
        /// Internal variable to store the Html font size.
        /// </summary>
        private int m_htmlFontHeight;
        #endregion

#region Internal Properties
        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public new TextAlign SelectionAlignment
        {
            get
            {
                PARAFORMAT fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                             RtfApi.EM_GETPARAFORMAT,
                             RtfApi.SCF_SELECTION, ref fmt);

                if ((fmt.dwMask & RtfApi.PFM_ALIGNMENT) == 0)
                {
                    return TextAlign.Left;
                }

                return (TextAlign)fmt.wAlignment;
            }

            set
            {
                PARAFORMAT fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = RtfApi.PFM_ALIGNMENT;
                fmt.wAlignment = (short)value;

                RtfApi.SendMessage(new HandleRef(this, Handle),
                             RtfApi.EM_SETPARAFORMAT,
                             RtfApi.SCF_SELECTION, ref fmt);
            }
        }

        /// <summary>
        /// Gets or sets the paragraph format.
        /// </summary>
        public PARAFORMAT ParaFormat
        {
            get
            {
                PARAFORMAT pf = new PARAFORMAT();
                pf.cbSize = Marshal.SizeOf(pf);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_GETPARAFORMAT,
                    RtfApi.SCF_SELECTION, ref pf);

                return pf;
            }

            set
            {
                PARAFORMAT pf = value;
                pf.cbSize = Marshal.SizeOf(pf);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_SETPARAFORMAT,
                    RtfApi.SCF_SELECTION, ref pf);
            }
        }

        /// <summary>
        /// Gets or sets the default paragraph format.
        /// </summary>
        public PARAFORMAT DefaultParaFormat
        {
            get
            {
                PARAFORMAT pf = new PARAFORMAT();
                pf.cbSize = Marshal.SizeOf(pf);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_GETPARAFORMAT,
                    RtfApi.SCF_ALL, ref pf);

                return pf;
            }

            set
            {
                PARAFORMAT pf = value;
                pf.cbSize = Marshal.SizeOf(pf);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_SETPARAFORMAT,
                    RtfApi.SCF_ALL, ref pf);
            }
        }

        /// <summary>
        /// Gets or sets the character format.
        /// </summary>
        public CHARFORMAT CharFormat
        {
            get
            {
                CHARFORMAT cf = new CHARFORMAT();
                cf.cbSize = Marshal.SizeOf(cf);

                // Get the alignment.
                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_GETCHARFORMAT,
                    RtfApi.SCF_SELECTION, ref cf);

                return cf;
            }

            set
            {
                CHARFORMAT cf = value;
                cf.cbSize = Marshal.SizeOf(cf);

                // Set the alignment.
                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_SETCHARFORMAT,
                    RtfApi.SCF_SELECTION, ref cf);
            }
        }

        /// <summary>
        /// Gets or sets the default character format.
        /// </summary>
        public CHARFORMAT DefaultCharFormat
        {
            get
            {
                CHARFORMAT cf = new CHARFORMAT();
                cf.cbSize = Marshal.SizeOf(cf);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_GETCHARFORMAT,
                    RtfApi.SCF_ALL, ref cf);

                return cf;
            }

            set
            {
                CHARFORMAT cf = value;
                cf.cbSize = Marshal.SizeOf(cf);

                RtfApi.SendMessage(new HandleRef(this, Handle),
                    RtfApi.EM_SETCHARFORMAT,
                    RtfApi.SCF_ALL, ref cf);
            }
        }
        #endregion

#region Implementation

        /// <summary>
        /// Method for maintaining the performance of RTF control while
        /// updating.
        /// </summary>
        public void BeginUpdate()
        {
            ++updating;

            if (updating > 1)
            {
                return;
            }

            oldEventMask = RtfApi.SendMessage(new HandleRef(this, Handle),
                RtfApi.EM_SETEVENTMASK, 0, 0);

            RtfApi.SendMessage(new HandleRef(this, Handle),
                RtfApi.WM_SETREDRAW, 0, 0);
        }

        /// <summary>
        /// Method invoked once the RTF control is updated.
        /// </summary>
        public void EndUpdate()
        {
            --updating;

            if (updating > 0)
            {
                return;
            }

            RtfApi.SendMessage(new HandleRef(this, Handle),
               RtfApi.WM_SETREDRAW, 1, 0);

            RtfApi.SendMessage(new HandleRef(this, Handle),
               RtfApi.EM_SETEVENTMASK, 0, oldEventMask);
        }

        /// <summary>
        /// Gets a value indicating whether [internal updating].
        /// </summary>
        /// <value><c>true</c> if [internal updating]; otherwise, <c>false</c>.</value>
        public bool InternalUpdating
        {
            get
            {
                return (updating != 0);
            }
        }

        /// <summary>
        /// Invoked once the handle has been created.
        /// </summary>
        /// <param name="e">The Eventargs.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Enable support for justification.
            RtfApi.SendMessage(new HandleRef(this, Handle),
                         RtfApi.EM_SETTYPOGRAPHYOPTIONS,
                         RtfApi.TO_ADVANCEDTYPOGRAPHY,
                         RtfApi.TO_ADVANCEDTYPOGRAPHY);
        }


        /// <summary>
        /// Converts the the given colorref to Color
        /// </summary>
        /// <param name="crColor">The Color.</param>
        /// <returns>The RGB coded color.</returns>
        private Color GetColor(int crColor)
        {
            byte r = (byte)(crColor);
            byte g = (byte)(crColor >> 8);
            byte b = (byte)(crColor >> 16);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Initializes the dictionary.
        /// </summary>
        private void Initialize()
        {
            m_htmlDictionary = new Dictionary<string, int>(10);
            m_htmlDictionary.Add("font", 0);
            m_htmlDictionary.Add("b", 1);
            m_htmlDictionary.Add("i", 2);
            m_htmlDictionary.Add("u", 3);
            m_htmlDictionary.Add("st", 4);
            m_htmlDictionary.Add("sup", 6);
            m_htmlDictionary.Add("sub", 7);
            m_htmlDictionary.Add("p", 8);
            m_htmlDictionary.Add("li", 9);
        }


        /// <summary>
        ///  Converts the the given color to colorref
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>The ColorRef equivalent for color.</returns>
        private int GetCOLORREF(int r, int g, int b)
        {
            int r2 = r;
            int g2 = (g << 8);
            int b2 = (b << 16);

            int result = r2 | g2 | b2;

            return result;
        }

        /// <summary>
        /// Converts the the given color to colorref
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The ColorRef</returns>
        private int GetCOLORREF(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            return GetCOLORREF(r, g, b);
        }

        /// <summary>
        /// Renders the given html text in to the RTF Control.
        /// </summary>
        /// <param name="strHTML">strHTML</param>
        /// <param name="font">font</param>
        /// <param name="color">color</param>
        public void RenderHTML(string strHTML, PdfFont font, PdfBrush color)
        {
            string fontName = font.Name;
            if (font is PdfStandardFont)
            {
                if (font.Name == "TimesRoman")
                    fontName = "Times New Roman";
                else if (font.Name == "Courier")
                    fontName = "Courier New";
            }
            m_Font = new Font(fontName, font.Size, (FontStyle)font.Style);
            PdfSolidBrush brush = color as PdfSolidBrush;
            m_Color = brush.Color;
            colors = brush.Color;
            ParseHtml(strHTML);
            return;
        }

        /// <summary>
        /// Parses the HtmlString.
        /// </summary>
        /// <param name="strHTML">The HtmlString.</param>
        public void ParseHtml(string strHTML)
        {
            CHARFORMAT cf;
            PARAFORMAT pf;

            cf = this.DefaultCharFormat;
            pf = this.DefaultParaFormat;

            if (m_pdfFontHeight == 0)
            {
                m_pdfFontHeight = (int)(cf.yHeight * m_Font.Size / 8);
            }
            //Apply base font and Color
            string strFont = new string(cf.szFaceName);
            int crFont = cf.crTextColor;
            int yHeight = m_pdfFontHeight;
            strFont = m_Font.Name;
            crFont = GetCOLORREF((int)m_Color.Red, (int)m_Color.Green, (int)m_Color.Blue);
            yHeight = m_pdfFontHeight;
            cf.szFaceName = new char[RtfApi.LF_FACESIZE];
            strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
            cf.crTextColor = crFont;
            cf.yHeight = yHeight;

            cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
            cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;

            this.HideSelection = true;
            this.BeginUpdate();

            //Much simpler way to handle page-breaks.
            strHTML = strHTML.Replace("<br/>", "\r\n");
            strHTML = strHTML.Replace("<BR/>", "\r\n");
            strHTML = strHTML.Replace("&nbsp;", " ");
            strHTML = strHTML.Replace("&", "&amp;");

            string xml = "<html>" + strHTML + "</html>";
            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = true;
            document.LoadXml(xml);
            foreach (XmlNode node in document.ChildNodes[0].ChildNodes)
            {
                ParseXmlNode(node, ref cf, ref pf);
                isNested = false;
                crFont = GetCOLORREF(colors);
                cf.crTextColor = crFont;
            }

            // reposition to final
            this.SelectionStart = this.TextLength + 1;
            this.SelectionLength = 0;

            this.EndUpdate();
            this.HideSelection = false;
        }

        /// <summary>
        /// Parses the each Html Elements (Xml node) and apply the formatting.
        /// </summary>
        /// <param name="node">The htmltag</param>
        /// <param name="cf">The Character format.</param>
        /// <param name="pf">The Paragraph format.</param>
        internal void ParseXmlNode(XmlNode node, ref CHARFORMAT cf, ref PARAFORMAT pf)
        {
            if (m_htmlDictionary == null)
            {
                Initialize();
            }
            
            if (node.PreviousSibling != null)
            {
                if (node.PreviousSibling.Name == "font")
                {
                    m_htmlFontHeight = m_pdfFontHeight;
                    cf.yHeight = m_htmlFontHeight;
                }
            }
            else if (node.ParentNode != null)
            {
                if (node.ParentNode.Name == "p")
                {
                    m_htmlFontHeight = m_pdfFontHeight;
                    cf.yHeight = m_htmlFontHeight;
                }
                else if (node.ParentNode.Name == "div")
                {
                    m_htmlFontHeight = m_pdfFontHeight;
                    cf.yHeight = m_htmlFontHeight;
                }
            }
            string nodeName = node.Name.ToLower();
            char[] trimChar = { ' ', '\x0000' };

            this.HideSelection = true;
            this.BeginUpdate();

            int outVal;
            if (nodeName != null)
            {
                if (m_htmlDictionary.TryGetValue(nodeName, out outVal))
                {
                    switch (outVal)
                    {
                        case 0:

                            if (node.Attributes != null)
                            {

                                string strFont = new string(cf.szFaceName);
                                int crFont = cf.crTextColor;
                                int yHeight = cf.yHeight;
                                foreach (XmlAttribute attribute in node.Attributes)
                                {
                                    string attributeName = attribute.Name.ToLower();

                                    strFont = strFont.Trim(trimChar);

                                    if (attributeName != null)
                                    {
                                        if (attributeName == "color")
                                        {
                                            if (attribute.Value[0] != '#')
                                            {
                                                Color color = Color.FromName(attribute.Value);
                                                crFont = GetCOLORREF(color);
                                                m_Color = color;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    Color color = Color.FromArgb((byte)int.Parse(attribute.Value.Substring(1, 2), NumberStyles.HexNumber), (byte)int.Parse(attribute.Value.Substring(3, 2), NumberStyles.HexNumber), (byte)int.Parse(attribute.Value.Substring(5, 2), NumberStyles.HexNumber));
                                                    crFont = GetCOLORREF(color);
                                                    m_Color = color;
                                                }
                                                catch
                                                { }
                                            }
                                        }
                                        else if (attributeName == "size")
                                        {
                                            yHeight = (int)(float.Parse(attribute.Value) * 20 * 5);
                                            m_htmlFontHeight = yHeight;
                                        }

                                        else if (attributeName == "face")
                                            strFont = attribute.Value;
                                    }
                                }

                                cf.szFaceName = new char[RtfApi.LF_FACESIZE];
                                strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
                                cf.crTextColor = crFont;
                                if (m_htmlFontHeight == 0)
                                {
                                    cf.yHeight = yHeight;
                                }
                                else
                                {
                                    cf.yHeight = m_htmlFontHeight;
                                }
                                cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
                                cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;
                            }

                            lastTag += "font";
                            break;


                        case 1:
                            cf.dwMask |= RtfApi.CFM_WEIGHT | RtfApi.CFM_BOLD;
                            cf.dwEffects |= RtfApi.CFE_BOLD;
                            cf.wWeight = RtfApi.FW_BOLD;
                            lastTag += "b";
                            break;

                        case 2:
                            cf.dwMask |= RtfApi.CFM_ITALIC;
                            cf.dwEffects |= RtfApi.CFE_ITALIC;
                            lastTag += "i";
                            break;

                        case 3:
                            cf.dwMask |= RtfApi.CFM_UNDERLINE | RtfApi.CFM_UNDERLINETYPE;
                            cf.dwEffects |= RtfApi.CFE_UNDERLINE;
                            cf.bUnderlineType = RtfApi.CFU_UNDERLINE;
                            lastTag += "u";
                            break;

                        case 4:
                            cf.dwMask |= RtfApi.CFM_STRIKEOUT;
                            cf.dwEffects |= RtfApi.CFE_STRIKEOUT;
                            lastTag += "s";
                            break;

                        case 5:
                            pf.dwMask = RtfApi.PFM_ALIGNMENT | RtfApi.PFM_NUMBERING;
                            pf.wAlignment = (short)RtfApi.PFA_LEFT;
                            pf.wNumbering = 0;

                            break;

                        case 6:
                            cf.dwMask |= RtfApi.CFM_SUPERSCRIPT;
                            cf.dwEffects |= RtfApi.CFE_SUPERSCRIPT;
                            lastTag += "sup";
                            break;

                        case 7:
                            cf.dwMask |= RtfApi.CFM_SUBSCRIPT;
                            cf.dwEffects |= RtfApi.CFE_SUBSCRIPT;
                            lastTag += "sub";
                            break;

                        case 8:
                            if (node.Attributes != null)
                            {
                                foreach (XmlAttribute attribute in node.Attributes)
                                {
                                    string attributeName = attribute.Name.ToLower();
                                    if (attributeName != null)
                                    {
                                        if (attributeName == "align")
                                        {
                                            if (attribute.Value.IndexOf("left") > 0)
                                            {
                                                pf.dwMask |= RtfApi.PFM_ALIGNMENT;
                                                pf.wAlignment = (short)RtfApi.PFA_LEFT;
                                            }
                                            else if (attribute.Value.IndexOf("right") > 0)
                                            {
                                                pf.dwMask |= RtfApi.PFM_ALIGNMENT;
                                                pf.wAlignment = (short)RtfApi.PFA_RIGHT;
                                            }
                                            else if (attribute.Value.IndexOf("center") > 0)
                                            {
                                                pf.dwMask |= RtfApi.PFM_ALIGNMENT;
                                                pf.wAlignment = (short)RtfApi.PFA_CENTER;
                                            }
                                        }
                                    }
                                }
                            }
                            lastTag += "p";
                            break;

                        case 9:
                            if (pf.wNumbering != RtfApi.PFN_BULLET)
                            {
                                pf.dwMask |= RtfApi.PFM_NUMBERING;
                                pf.wNumbering = (short)RtfApi.PFN_BULLET;
                            }
                            lastTag += "li";
                            break;
                    }
                }
            }

            if (node.Name == "#text")
            {
                int nLen = node.OuterXml.Length;
                int nStartCache = this.SelectionStart;
                this.SelectedText = GetSafeText(node.OuterXml);
                this.SelectionStart = nStartCache;
                this.SelectionLength = node.OuterXml.Length;

                // apply format
                this.ParaFormat = pf;
                this.CharFormat = cf;

                this.SelectionStart = this.TextLength + 1;
                this.SelectionLength = 0;

                if (lastTag.IndexOf("b") >= 0)
                {
                    cf.dwEffects &= ~RtfApi.CFE_BOLD;
                    cf.wWeight = RtfApi.FW_NORMAL;
                }
                if (lastTag.IndexOf("i") >= 0)
                {
                    cf.dwEffects &= ~RtfApi.CFE_ITALIC;
                }
                if (lastTag.IndexOf("u") >= 0)
                {
                    cf.dwEffects &= ~RtfApi.CFE_UNDERLINE;
                }
                if (lastTag.IndexOf("s") >= 0)
                {
                    cf.dwEffects &= ~RtfApi.CFM_STRIKEOUT;
                }
                if (lastTag.IndexOf("sup") >= 0)
                {
                    cf.dwEffects &= ~RtfApi.CFE_SUPERSCRIPT;
                }
                if (lastTag.IndexOf("sub") >= 0)
                {
                    cf.dwEffects &= ~RtfApi.CFE_SUBSCRIPT;
                }
                if (lastTag.IndexOf("font") >= 0)
                {
                    string strFont = m_Font.Name;
                    cf.szFaceName = new char[RtfApi.LF_FACESIZE];
                    strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
                    cf.crTextColor = GetCOLORREF(m_Color);
                    int yHeight = m_pdfFontHeight;
                   // cf.yHeight = yHeight;

                    cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
                    cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;
                }
                lastTag = "";
            }
            else if (node.Name == "#whitespace")
            {
                int nStartCache = this.SelectionStart;
                this.SelectedText = node.OuterXml;
                this.SelectionStart = nStartCache;
                this.SelectionLength = node.OuterXml.Length;

                this.SelectionStart = this.TextLength + 1;
                this.SelectionLength = 0;
            }
            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.ParentNode.Name == "p" && lastTag == "")
                    {
                        string strFont = m_Font.Name;
                        cf.szFaceName = new char[RtfApi.LF_FACESIZE];
                        strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
                        cf.crTextColor = GetCOLORREF(colors);
                        int yHeight = m_pdfFontHeight;
                        // cf.yHeight = yHeight;
                        cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
                        cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;
                    }
                    else if (childNode.ParentNode.Name == "b" && lastTag == "")
                    {
                        string strFont = m_Font.Name;
                        cf.szFaceName = new char[RtfApi.LF_FACESIZE];
                        strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
                        cf.crTextColor = GetCOLORREF(colors);
                        int yHeight = m_pdfFontHeight;
                        // cf.yHeight = yHeight;
                        cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
                        cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;
                    }
                    else if (childNode.ParentNode.Name == "i" && lastTag == "")
                    {
                        string strFont = m_Font.Name;
                        cf.szFaceName = new char[RtfApi.LF_FACESIZE];
                        strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
                        cf.crTextColor = GetCOLORREF(colors);
                        int yHeight = m_pdfFontHeight;
                        // cf.yHeight = yHeight;
                        cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
                        cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;
                    }
                    else if (childNode.ParentNode.Name == "u" && lastTag == "")
                    {
                        string strFont = m_Font.Name;
                        cf.szFaceName = new char[RtfApi.LF_FACESIZE];
                        strFont.CopyTo(0, cf.szFaceName, 0, Math.Min(RtfApi.LF_FACESIZE - 1, strFont.Length));
                        cf.crTextColor = GetCOLORREF(colors);
                        int yHeight = m_pdfFontHeight;
                        // cf.yHeight = yHeight;
                        cf.dwMask |= RtfApi.CFM_COLOR | RtfApi.CFM_SIZE | RtfApi.CFM_FACE;
                        cf.dwEffects &= ~RtfApi.CFE_AUTOCOLOR;
                    }

                    isNested = true;
                    lastTag += childNode.Name;
                    this.ParseXmlNode(childNode, ref cf, ref pf);
                }
            }
        }
        /// <summary>
        /// Converts the the given text to safe text
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The safe text</returns>
        private string GetSafeText(string text)
        {
            text = text.Replace("&amp;", "&");
            text = text.Replace("&lt;", "<");
            text = text.Replace("&gt;", ">");
            return text;
        }
        #endregion
    }
}
#endif