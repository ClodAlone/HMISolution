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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Represent the Paragraph Box
    /// </summary>

    public class ParagraphBox
    {
        #region Contstants  

        private ParagraphBox _parent;
        private List<ParagraphBox> _boxes;
        private ParagraphBox _previousbox;
        private ParagraphBox _nextbox;
        private string _fontsize = "14.0";
        private Color _foreground = Colors.Black;
        private Color _hyperlinkcolor = Colors.Blue;
        private string _color = string.Empty;
        private string _backgroundcolor = string.Empty;
        private string _background = string.Empty;
        private string _fontweight = "normal";
        private string _fontfamily = "Times New Roman";
        private string _fontstyle = HtmlConstants.FontStyle_Normal;
        private string _fontvariant = "normal";
        private StrikeThrough _strikethrough = StrikeThrough.None;
        private Baseline _baseline = Baseline.Normal;
        private string _textdecoration = string.Empty;
        private string _font = string.Empty;
        private double _imageheight ;
        private double _imagewidth ;
        private string m_TextAlign = string.Empty;
        private string _whiteSpace = string.Empty;
        private double _leftindent = 0.0;
        private ListType _listType = ListType.None;

        #endregion

        #region Constructors

        public ParagraphBox()
        {
            _boxes = new List<ParagraphBox>();
        }

        public ParagraphBox(ParagraphBox parent, HTMLTagInfo tagname)
            : this(parent)
        {
            HTMLTag = tagname;
        }

        public ParagraphBox(ParagraphBox parentbox)
            : this()
        {
            ParentBox = parentbox;
        }

        /// <summary>
        /// Collect the CssProperties from the ParagrpahBox
        /// </summary>
        static ParagraphBox()
        {
            Properties = new Dictionary<string, PropertyInfo>();
            CssProperties = new List<PropertyInfo>();
            InheritableProperties = new List<PropertyInfo>();
            NotInheritable = new Dictionary<string, PropertyInfo>();
            GatherPropertiesList();
        }

        #endregion

        #region Properties

        internal static Dictionary<string, PropertyInfo> NotInheritable
        {
            get;
            set;
        }

        /// <summary>
        /// Describes the TextAlignment of the Paragraph
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("text-align")]
        [CssPropertyInherited()]
        public string TextAlign
        {
            get
            {
                return m_TextAlign;
            }
            set
            {
                m_TextAlign = value;
            }
        }


        [CssPropertiesAttribute("white-space")]
        [CssPropertyInherited()]
        public string WhiteSpace
        {
            get { return _whiteSpace; }
            set { _whiteSpace = value; }
        }

        /// <summary>
        /// Defines the Tex which inside the tags ex: <b>I am bold</b>
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// ParagraphBoxes collection for every instance
        /// </summary>
        public List<ParagraphBox> Boxes
        {
            get
            {
                return _boxes;
            }
            set
            {
                _boxes = value;
            }

        }

        [CssPropertiesAttribute("listType")]
        [CssPropertyInherited()]
        [HTMLNotInheritable]
        public ListType ListType
        {
            get
            {
                return _listType;
            }
            set
            {
                _listType = value;
            }
        }

        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// All the properties of ParagraphBox
        /// </summary>
        public static Dictionary<string, PropertyInfo> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Only CssPropertiesAttribute
        /// </summary>
        
        public static List<PropertyInfo> CssProperties
        {
            get;
            set;
        }

        /// <summary>
        /// It collects the properties which are inheritable in the HTML architecture
        /// </summary>
        public static List<PropertyInfo> InheritableProperties
        {
            get;
            set;
        }
        
        /// <summary>
        /// Parent for the ParagraphBox
        /// </summary>
        public ParagraphBox ParentBox
        {
            get
            {
               return _parent;
            }
            set
            {
                _parent = value;
                if (_parent!=null)
                {
                    _parent.Boxes.Add(this);
                }
            }
        }

        public ParagraphBox PreviousBox
        {
            get
            {
                return _previousbox;
            }
            set
            {
                _previousbox = value;
                if(value!=null)
                    value.NextBox = this;
            }
        }


        public ParagraphBox NextBox
        {
            get
            {
                return _nextbox;
            }
            set
            {
                _nextbox = value;
            }
        }

        /// <summary>
        /// It decides whether ParagraphBox is HyperlinkAdv
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("hyperlink")]
        [CssPropertyInherited()]
        public HyperlinkAdv Hyperlink
        {
            get;
            set;
        }

        [HTMLNotInheritable()]
        [CssPropertiesAttribute("hyperlinkcolor")]
        [CssPropertyInherited()]
        public Color HyperLinkColor
        {
            get
            {
                return _hyperlinkcolor;
            }
            set
            {
                _hyperlinkcolor = value;
            }
        }
        /// <summary>
        /// It paints the forground color for the Text which inside the ParagraphBox
        /// </summary>
        
        [CssPropertiesAttribute("foreground")]
        [CssPropertyInherited()]
        public Color Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
            }
        }

        [CssPropertiesAttribute("background")]
        public string Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                ActualBackgroundColor = StringToColor(_background,false,true);
            }
        }
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("left-indent")]
        [CssPropertyInherited()]
        public double LeftIndent
        {
            get
            {
                if (this.HTMLTag != null && this.HTMLTag.TagName == HtmlConstants.BlockQuoteTag)
                    return ParentBox.LeftIndent + 35;
                return _leftindent;
            }
            set
            {
                _leftindent = value;
            }
        }


        /// <summary>
        /// It paints the background color for the ParagraphBox
        /// </summary>
        
        [CssPropertiesAttribute("background-color")]
        [CssPropertyInherited()]
        public string BackgroundColor
        {
            get
            {
                return _backgroundcolor;
            }
            set
            {
                _backgroundcolor = value;
                ActualBackgroundColor = StringToColor(_backgroundcolor,false,true);
            }
        }

        /// <summary>
        /// Describes the font [ Fontweight,FontStyle,FontVariant] for the ParagraphBox
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("font")]
        [CssPropertyInherited()]
        public string Font
        {
            get { return _font; }
            set
            {
                _font = value;

                int extractedfontpos;
                string extractedfont = HtmlTagsParser.Search(HtmlTagsParser.CssFont, value, out extractedfontpos);

                if (!string.IsNullOrEmpty(extractedfont))
                {
                    extractedfont = extractedfont.Trim();
                    //Check for style||variant||weight on the left
                    string leftSide = value.Substring(0, extractedfontpos);
                    string fontStyle = HtmlTagsParser.Search(HtmlTagsParser.FontStyle, leftSide);
                    string fontVariant = HtmlTagsParser.Search(HtmlTagsParser.FontVariant, leftSide);
                    string fontWeight = HtmlTagsParser.Search(HtmlTagsParser.FontWeight, leftSide);

                    //Check for family on the right
                    string rightSide = value.Substring(extractedfontpos + extractedfont.Length);
                    string fontFamily = rightSide.Trim(); 

                    //Check for font-size and line-height
                    string fontSize = extractedfont;
                    string lineHeight = string.Empty;

                    if (extractedfont.Contains("/") && extractedfont.Length > extractedfont.IndexOf("/") + 1)
                    {
                        int slashPos = extractedfont.IndexOf("/");
                        fontSize = extractedfont.Substring(0, slashPos);
                        lineHeight = extractedfont.Substring(slashPos + 1);
                    }

                    if (!string.IsNullOrEmpty(fontStyle)) FontSyle = fontStyle;
                    if (!string.IsNullOrEmpty(fontVariant)) FontVariant = fontVariant;
                    if (!string.IsNullOrEmpty(fontWeight)) FontWeight = fontWeight;
                    if (!string.IsNullOrEmpty(fontFamily)) FontFamily = fontFamily;
                    if (!string.IsNullOrEmpty(fontSize)) FontSize = fontSize;
                }
            }
        }

        public Color ActualBackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// String color determines the Foreground color for the Text
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("color")]
        [CssPropertyInherited()]
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                Foreground = StringToColor(_color,false,false);
                HyperLinkColor = StringToColor(_color,true,false);
            }
        }

        /// <summary>
        /// It decides whether the ParagraphBox is ImageContainerAdv
        /// </summary>
        public ImageContainerAdv Image
        {
            get;
            set;
        }

        /// <summary>
        /// It stores the corresponding HTML tag information
        /// </summary>
        public HTMLTagInfo HTMLTag
        {
            get;
            set;
        }

        /// <summary>
        /// It decides the Text's FontFamily
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("font-family")]
        [CssPropertyInherited()]
        public string FontFamily
        {
            get
            {
                return _fontfamily;
            }
            set
            {
                _fontfamily = value;
            }
        }

        /// <summary>
        /// It decides the FontSize of the Text
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("font-size")]
        [CssPropertyInherited()]
        public string FontSize
        {
            get
            {
                return _fontsize;
            }

            set
            {
                
                double temfontsize = 0.0;
                if (!(double.TryParse(value, out temfontsize)))
                {
                    switch (value)
                    {
                        case CssConstants.XXSmall:
                            temfontsize = CssDefaultValue.FontSize - 4;
                            break;
                        case CssConstants.XSmall:
                            temfontsize = CssDefaultValue.FontSize - 3;
                            break;
                        case CssConstants.Small:
                            temfontsize = CssDefaultValue.FontSize - 2;
                            break;
                        case CssConstants.Smaller:
                            temfontsize = double.Parse(this.ParentBox.FontSize) - 2;
                            break;
                        case CssConstants.Larger:
                            temfontsize = double.Parse(this.ParentBox.FontSize) + 2;
                            break;
                        case CssConstants.Large:
                            temfontsize = CssDefaultValue.FontSize + 2;
                            break;
                        case CssConstants.XLarge:
                            temfontsize = CssDefaultValue.FontSize + 3;
                            break;
                        case CssConstants.XXLarge:
                            temfontsize = CssDefaultValue.FontSize + 4;
                            break;
                        default:
                            temfontsize = ConvertSize(value, double.Parse(this.ParentBox.FontSize));
                            break;
                    }
                }
               _fontsize = temfontsize.ToString();
            }
        }

        [CssPropertiesAttribute("text-decoration")]
        public string TextDecoration
        {
            get
            {
                return _textdecoration;
            }
            set
            {
                _textdecoration = value;
                Underline = Text_DecorationToUnderline(value);
                StrikeThrough = Text_DecorationToStrikeThrogh(value);
            }
        }

        /// <summary>
        /// It decides the FontStyle of the Text.
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("font-style")]
        [CssPropertyInherited()]
        public string FontSyle
        {
            get
            {
                return _fontstyle;
            }
            set
            {
                _fontstyle = value;
            }
        }

        /// <summary>
        /// It decides the FontVariant of the Text.
        /// </summary>
        [CssPropertiesAttribute("font-variant")]
        [CssPropertyInherited()]
        public string FontVariant
        {
            get
            {
                return _fontvariant;
            }
            set
            {
                _fontvariant = value;
            }
        }

        /// <summary>
        /// It defines the SubScript and SuperScript for the Text
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("baseline")]
        [CssPropertyInherited()]
        public Baseline BaseLine
        {
            get
            {
                return _baseline;
            }
            set
            {
                _baseline = value;
            }
        }

        /// <summary>
        /// FontWeight for the Text
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("font-weight")]
        [CssPropertyInherited()]
        public string FontWeight
        {
            get
            {
                return _fontweight;
            }
            set
            {
                _fontweight = value;
                ActualFontWeight = StringToFontWeight(value);
            }
        }

        public FontWeight ActualFontWeight
        {
            get;
            set;
        }
        
        /// <summary>
        /// Whether the Text should be underlined.
        /// </summary>
        [HTMLNotInheritable()]
        [CssPropertiesAttribute("underline")]
        [CssPropertyInherited()]
        public bool Underline
        {
            get;
            set;
        }

        /// <summary>
        /// SingleStroke and DoubleStroke for the Text
        /// </summary>
        [CssPropertiesAttribute("strike-through")]
        public StrikeThrough StrikeThrough
        {
            get
            {
                return _strikethrough;
            }
            set
            {
                _strikethrough = value;
            }
        }

        /// <summary>
        /// It decides the Height of the Image
        /// </summary>
        public double ImageHeight
        {
            get
            {
                return CssDefaultValue.ImageHeight;
            }
            set
            {
                _imageheight = value;
            }
        }

        /// <summary>
        /// It decides the Width of the Image
        /// </summary>
        public double ImageWidth
        {
            get
            {
                return CssDefaultValue.ImageWidth;
            }
            set
            {
                _imagewidth = value;
            }
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Get the Actual color for the color string
        /// </summary>
        /// <param name="colorValue">String value ex" #89234 or rgb(23,32,100) or Gray</param>
        /// <returns></returns>
        private Color StringToColor(string colorValue,bool IsHyperlinkColor,bool IsBackgroundColor)
        {
            Color onError = Colors.Black;
            byte a = 0; 
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (IsHyperlinkColor)
                onError = Colors.Blue;
            else if (IsBackgroundColor)
                onError = Colors.White;
            else
                onError = Colors.Black;
            a = 255;
            
            if (string.IsNullOrEmpty(colorValue)) return onError;

            colorValue = colorValue.ToLower().Trim();

            if (colorValue.StartsWith("#"))
            {
                #region hexadecimal forms
                string hex = colorValue.Substring(1);

                if (hex.Length==8)
                {
                    a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                }

                else if (hex.Length == 6)
                {
                    r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                }
                else if (hex.Length == 3)
                {
                    r = byte.Parse(new String(hex.Substring(0, 1)[0], 2), NumberStyles.HexNumber);
                    g = byte.Parse(new String(hex.Substring(1, 1)[0], 2), NumberStyles.HexNumber);
                    b = byte.Parse(new String(hex.Substring(2, 1)[0], 2), NumberStyles.HexNumber);
                }
                else
                {
                    return onError;
                }
                #endregion
            }
            else if (colorValue.StartsWith("rgb(") && colorValue.EndsWith(")"))
            {
                string rgb = colorValue.Substring(4, colorValue.Length - 5);
                string[] chunks = rgb.Split(',');

                a = 255;
                if (chunks.Length == 4)
                {
                    a = Convert.ToByte(ParseNumber(chunks[0].Trim(), 255f));
                    r = Convert.ToByte(ParseNumber(chunks[1].Trim(), 255f));
                    g = Convert.ToByte(ParseNumber(chunks[2].Trim(), 255f));
                    b = Convert.ToByte(ParseNumber(chunks[3].Trim(), 255f));
                }
                else if (chunks.Length == 3)
                {
                    unchecked
                    {
                        r = Convert.ToByte(ParseNumber(chunks[0].Trim(), 255f));
                        g = Convert.ToByte(ParseNumber(chunks[1].Trim(), 255f));
                        b = Convert.ToByte(ParseNumber(chunks[2].Trim(), 255f));
                    }
                }
                else
                {
                    return onError;
                }

            }
            else
            {
                string hex = string.Empty;
                switch (colorValue)
                {
                    case CssConstants.Maroon:
                        hex = "#800000";
                        break;
                    case CssConstants.Red:
                        hex = "#ff0000"; 
                        break;
                    case CssConstants.Orange:
                        hex = "#ffA500"; 
                        break;
                    case CssConstants.Olive:
                        hex = "#808000"; 
                        break;
                    case CssConstants.Purple:
                        hex = "#800080"; 
                        break;
                    case CssConstants.Fuchsia:
                        hex = "#ff00ff"; 
                        break;
                    case CssConstants.White:
                        hex = "#ffffff"; 
                        break;
                    case CssConstants.Lime:
                        hex = "#00ff00"; 
                        break;
                    case CssConstants.Green:
                        hex = "#008000"; 
                        break;
                    case CssConstants.Navy:
                        hex = "#000080"; 
                        break;
                    case CssConstants.Blue:
                        hex = "#0000ff"; 
                        break;
                    case CssConstants.Aqua:
                        hex = "#00ffff"; 
                        break;
                    case CssConstants.Teal:
                        hex = "#008080"; 
                        break;
                    case CssConstants.Black:
                        hex = "#000000"; 
                        break;
                    case CssConstants.Silver:
                        hex = "#c0c0c0"; 
                        break;
                    case CssConstants.Gray:
                        hex = "#808080"; 
                        break;
                    case CssConstants.Yellow:
                        hex = "#FFFF00"; 
                        break;
                }

                if (string.IsNullOrEmpty(hex))
                {
                    return onError;
                }
                else
                {
                    Color c = StringToColor(hex,IsHyperlinkColor,IsBackgroundColor);
                    a = c.A;
                    r = c.R;
                    g = c.G;
                    b = c.B;
                }
            }

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }

        public FontWeight StringToFontWeight(string fontweight)
        {
            FontWeight font = FontWeights.Normal;
            if(!string.IsNullOrEmpty(fontweight))
            {
                switch (fontweight.ToLower())
                {
                    case "100":
                        font= FontWeights.Thin;
                        break;
                    case "200":
                        font=FontWeights.ExtraLight;
                        break;
                    case "300":
                        font= FontWeights.Light;
                        break;
                    case "400":
                        font= FontWeights.Normal;
                        break;
                    case "500":
                        font= FontWeights.Medium;
                        break;
                    case "600":
                        font= FontWeights.SemiBold;
                        break;
                    case "700":
                        font=FontWeights.Bold;
                        break;
                    case "800":
                        font=FontWeights.ExtraBold;
                        break;
                    case "900":
                        font=FontWeights.Black;
                        break;
                    case "bold":
                        font= FontWeights.Bold;
                        break;
                    case "lighter":
                        font = FontWeights.ExtraLight;
                        break;
                    case "bolder":
                        font = FontWeights.ExtraBold;
                        break;
                    default:
                        font = FontWeights.Normal;
                        break;
                }
            }
            return font;
        }

        internal TextAlignment StringToAlignment(string textalign)
        {
            TextAlignment tempalign;
            if (textalign == HtmlConstants.Left)
                tempalign = TextAlignment.Left;
            else if (textalign == HtmlConstants.Right)
                tempalign = TextAlignment.Right;
            else if (textalign == HtmlConstants.Center)
                tempalign = TextAlignment.Center;
            else if (textalign == HtmlConstants.Justify)
                tempalign = TextAlignment.Justify;
            else
                tempalign = TextAlignment.Left;
            return tempalign;
        }

        /// <summary>
        /// Converts the Percentage value to the correct float value
        /// </summary>
        private float ParseNumber(string number, float hundredPercent)
        {
            if (string.IsNullOrEmpty(number))
                return 0f;
            float result = 0;
            if (number.EndsWith(CssConstants.Percentage))
            {
                result = (result / 100f) * hundredPercent;
            }

            return result;
        }

        /// <summary>
        /// Get the number before to the units
        /// </summary>
        private float GetNumberBefore(string val, string end)
        {
            val = val.Substring(0, val.IndexOf(end));
            return Single.Parse(val, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Set the properties value from the ParentBox
        /// </summary>
        /// <param name="father"></param>
        /// <param name="everything"></param>
        private void InheritStyle(ParagraphBox father, bool everything,List<PropertyInfo> NotInherit)
        {
            if (father != null)
            {
                IEnumerable<PropertyInfo> properties = everything ? CssProperties : InheritableProperties;
                foreach (PropertyInfo property in properties)
                {
                    if(!(NotInherit.Contains(property)))
                        property.SetValue(this, property.GetValue(father, null), null);
                }
            }
        }

        private bool Text_DecorationToUnderline(string textdecoration)
        {
            if (textdecoration.ToLower()==CssConstants.Underline)
            {
                return true;
            }
            return false;
        }


        private StrikeThrough Text_DecorationToStrikeThrogh(string textdecoration)
        {
            if (textdecoration.ToLower()==CssConstants.LineThrough)
                return Controls.StrikeThrough.SingleStrike;
            return Controls.StrikeThrough.None;
        }

        private Baseline VerticalAlignToBaseLine(string verticalalign)
        {
            if (verticalalign.ToLower() == CssConstants.SubScript)
                return Baseline.Subscript;
            else if (verticalalign.ToLower() == CssConstants.SuperScript)
                return Baseline.Superscript;
            return Baseline.Normal;
        }
        #endregion

        #region InternalMethods
        /// <summary>
        /// Inherit value from the ParentBox
        /// </summary>
        internal void InheritParentStyle()
        {
            if(this.ParentBox!=null && this.HTMLTag!=null)
                InheritStyle(ParentBox, false,GetPropertiesforTag(this.HTMLTag.TagName));
        }

        internal static void GatherPropertiesList()
        {
            PropertyInfo[] properties = typeof(ParagraphBox).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                CssPropertiesAttribute cssproperty = Attribute.GetCustomAttribute(property, typeof(CssPropertiesAttribute)) as CssPropertiesAttribute;

                if (cssproperty != null)
                {
                    HTMLNotInheritable notinheritable = Attribute.GetCustomAttribute(property, typeof(HTMLNotInheritable)) as HTMLNotInheritable;
                    if (notinheritable != null)
                    {
                        NotInheritable.Add(cssproperty.PropertyName, property);
                    }
                    Properties.Add(cssproperty.PropertyName, property);
                    CssProperties.Add(property);
                    CssPropertyInherited inherited = Attribute.GetCustomAttribute(property, typeof(CssPropertyInherited)) as CssPropertyInherited;
                    if (inherited != null)
                    {
                        InheritableProperties.Add(property);
                    }
                }
            }
        }

        internal List<PropertyInfo> GetPropertiesforTag(string tagname)
        {
            List<PropertyInfo> Local = new List<PropertyInfo>();
            if (AreHeadingTags(tagname))
            {
                Local.Add(NotInheritable[CssConstants.FontWeight]);
                Local.Add(NotInheritable[CssConstants.FontSize]);
                Local.Add(NotInheritable[CssConstants.Font]);
            }
            else if (tagname == HtmlConstants.BlockQuoteTag)
                Local.Add(NotInheritable[CssConstants.LeftIndent]);
            else if (tagname == HtmlConstants.BoldTag)
                Local.Add(NotInheritable[CssConstants.FontWeight]);
            else if (tagname == HtmlConstants.ItalicTag || tagname == HtmlConstants.VarTag)
                Local.Add(NotInheritable[CssConstants.FontStyle]);
            else if (tagname == HtmlConstants.CodeTag)
            {
                Local.Add(NotInheritable[CssConstants.FontFamily]);
                Local.Add(NotInheritable[CssConstants.FontWeight]);
            }
            else if (tagname == HtmlConstants.HyperlinkTag)
                Local.Add(NotInheritable[CssConstants.HyperlinkColor]);
            else if (tagname == HtmlConstants.SubTag)
                Local.Add(NotInheritable[CssConstants.Baseline]);
            else if (tagname == HtmlConstants.UnderlineTag)
                Local.Add(NotInheritable[CssConstants.Underline]);
            else if (tagname == HtmlConstants.SupTag)
                Local.Add(NotInheritable[CssConstants.Baseline]);
            else if (tagname == HtmlConstants.StrongTag)
                Local.Add(NotInheritable[CssConstants.FontWeight]);
            else if (tagname == HtmlConstants.SmallTag)
                Local.Add(NotInheritable[CssConstants.FontSize]);
            else if (tagname == HtmlConstants.BigTag)
            {
                Local.Add(NotInheritable[CssConstants.FontSize]);
                Local.Add(NotInheritable[CssConstants.Font]);
            }
            else if (tagname == HtmlConstants.SampTag)
            {
                Local.Add(NotInheritable[CssConstants.FontSize]);
                Local.Add(NotInheritable[CssConstants.FontFamily]);
                Local.Add(NotInheritable[CssConstants.Font]);
            }
            else if (tagname == HtmlConstants.CenterTag || tagname == HtmlConstants.LeftTag)
                Local.Add(NotInheritable[CssConstants.TextAlign]);

            else if (tagname == HtmlConstants.OLTag || tagname == HtmlConstants.ULTag)
                Local.Add(NotInheritable[CssConstants.ListType]);
            
            return Local;
        }

        /// <summary>
        /// Check whether the tags are Block tags
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AreSpanTags(string name)
        {
            return name == HtmlConstants.BoldTag || name == HtmlConstants.ItalicTag || name == HtmlConstants.UnderlineTag || name == HtmlConstants.FontTag
                || name == HtmlConstants.StrongTag || name == HtmlConstants.BigTag || name == HtmlConstants.SubTag || name == HtmlConstants.SupTag
                || name == HtmlConstants.SampTag || name == HtmlConstants.SmallTag || name == HtmlConstants.SpanTag || name == HtmlConstants.CodeTag
                || name == HtmlConstants.EmTag || name == HtmlConstants.PreTag;
        }

        /// <summary>
        /// Check whether the tags are Container tags
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AreContainerTags(string name)
        {
            return name == HtmlConstants.DivTag || name == HtmlConstants.ParagraphTag || name == HtmlConstants.BlockQuoteTag
                || name == HtmlConstants.BodyTag || name == HtmlConstants.BreakTag;
        }

        /// <summary>
        /// Checks whether the tags are Table tags.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AreTableTags(string name)
        {
            return name == HtmlConstants.TableTag || name == HtmlConstants.TableRowTag || name == HtmlConstants.TableDataTag;
        }

        /// <summary>
        /// Check whether the tags are HeaderTags
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AreHeaderTags(string name)
        {
            return name == HtmlConstants.HeadTag || name == HtmlConstants.BodyTag || name == HtmlConstants.TitleTag || name == HtmlConstants.HtmlTag;
        }

        /// <summary>
        /// Check whether the tags are HeadingTags
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AreHeadingTags(string name)
        {
            return name == HtmlConstants.H1Tag || name == HtmlConstants.H2Tag || name == HtmlConstants.H3Tag || name == HtmlConstants.H4Tag
                || name == HtmlConstants.H5Tag || name == HtmlConstants.H6Tag || name == HtmlConstants.H7Tag;
        }

        /// <summary>
        /// Converts the incoming font size in points
        /// </summary>
        /// <param name="paramValue">input sting value ex: 23em or 10in or 20pc </param>
        /// <param name="baseSize">parentsize value</param>
        /// <returns></returns>
        internal double ConvertSize(string paramValue, double baseSize)
        {
            if (baseSize < 0)
                baseSize = 3;
            if (paramValue.EndsWith(CssConstants.Percentage))
                return baseSize * GetNumberBefore(paramValue, CssConstants.Percentage) / 100;
            else if (paramValue.EndsWith(CssConstants.Em))
                return baseSize * GetNumberBefore(paramValue, CssConstants.Em);
            else if (paramValue.EndsWith(CssConstants.Ex))
                return baseSize / 2 * GetNumberBefore(paramValue, CssConstants.Ex);
            else if (paramValue.EndsWith(CssConstants.Pt))
                return GetNumberBefore(paramValue,CssConstants.Pt);
            else if (paramValue.EndsWith(CssConstants.In))
                return GetNumberBefore(paramValue, CssConstants.In) * 72;
            else if (paramValue.EndsWith(CssConstants.Cm))
                return GetNumberBefore(paramValue, CssConstants.Cm) * 28;
            else if (paramValue.EndsWith(CssConstants.Mm))
                return GetNumberBefore(paramValue, CssConstants.Mm) * 3;
            else if (paramValue.EndsWith(CssConstants.Pc))
                return GetNumberBefore(paramValue, CssConstants.Pc) / 6;
            else if (paramValue.EndsWith(CssConstants.Px))
                return GetNumberBefore(paramValue, CssConstants.Px);

            return 0;
        }

        /// <summary>
        /// Converts the string font-style in FontStyle
        /// </summary>
        /// <param name="fontstyle"></param>
        /// <returns></returns>
        internal FontStyle StringToFontStyle(string fontstyle)
        {
            if (fontstyle.ToLower() == HtmlConstants.FontStyle_Italic)
                return FontStyles.Italic;
            return FontStyles.Normal;
        }

        #endregion 

    }
}
