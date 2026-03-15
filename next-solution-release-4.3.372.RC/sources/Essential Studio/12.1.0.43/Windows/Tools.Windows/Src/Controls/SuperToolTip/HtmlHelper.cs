#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
   
    internal struct Constants
    {

        #region Html Constants

        internal const string A = "A";
        internal const string ABBR = "ABBR";
        internal const string ACRONYM = "ACRONYM";
        internal const string ADDRESS = "ADDRESS";
        internal const string APPLET = "APPLET";
        internal const string AREA = "AREA";
        internal const string B = "B";
        internal const string BASE = "BASE";
        internal const string BASEFONT = "BASEFONT";
        internal const string BDO = "BDO";
        internal const string BIG = "BIG";
        internal const string BLOCKQUOTE = "BLOCKQUOTE";
        internal const string BODY = "BODY";
        internal const string BR = "BR";
        internal const string BUTTON = "BUTTON";
        internal const string CAPTION = "CAPTION";
        internal const string CENTER = "CENTER";
        internal const string CITE = "CITE";
        internal const string CODE = "CODE";
        internal const string COL = "COL";
        internal const string COLGROUP = "COLGROUP";
        internal const string DD = "DD";
        internal const string DEL = "DEL";
        internal const string DFN = "DFN";
        internal const string DIR = "DIR";
        internal const string DIV = "DIV";
        internal const string DL = "DL";
        internal const string DT = "DT";
        internal const string EM = "EM";
        internal const string FIELDSET = "FIELDSET";
        internal const string FONT = "FONT";
        internal const string FORM = "FORM";
        internal const string FRAME = "FRAME";
        internal const string FRAMESET = "FRAMESET";
        internal const string H1 = "H1";
        internal const string H2 = "H2";
        internal const string H3 = "H3";
        internal const string H4 = "H4";
        internal const string H5 = "H5";
        internal const string H6 = "H6";
        internal const string HEAD = "HEAD";
        internal const string HR = "HR";
        internal const string HTML = "HTML";
        internal const string I = "I";
        internal const string IFRAME = "IFRAME";
        internal const string IMG = "IMG";
        internal const string INPUT = "INPUT";
        internal const string INS = "INS";
        internal const string ISINDEX = "ISINDEX";
        internal const string KBD = "KBD";
        internal const string LABEL = "LABEL";
        internal const string LEGEND = "LEGEND";
        internal const string LI = "LI";
        internal const string LINK = "LINK";
        internal const string MAP = "MAP";
        internal const string MENU = "MENU";
        internal const string META = "META";
        internal const string NOFRAMES = "NOFRAMES";
        internal const string NOSCRIPT = "NOSCRIPT";
        internal const string OBJECT = "OBJECT";
        internal const string OL = "OL";
        internal const string OPTGROUP = "OPTGROUP";
        internal const string OPTION = "OPTION";
        internal const string P = "P";
        internal const string PARAM = "PARAM";
        internal const string PRE = "PRE";
        internal const string Q = "Q";
        internal const string S = "S";
        internal const string SAMP = "SAMP";
        internal const string SCRIPT = "SCRIPT";
        internal const string SELECT = "SELECT";
        internal const string SMALL = "SMALL";
        internal const string SPAN = "SPAN";
        internal const string STRIKE = "STRIKE";
        internal const string STRONG = "STRONG";
        internal const string STYLE = "STYLE";
        internal const string SUB = "SUB";
        internal const string SUP = "SUP";
        internal const string TABLE = "TABLE";
        internal const string TBODY = "TBODY";
        internal const string TD = "TD";
        internal const string TEXTAREA = "TEXTAREA";
        internal const string TFOOT = "TFOOT";
        internal const string TH = "TH";
        internal const string THEAD = "THEAD";
        internal const string TITLE = "TITLE";
        internal const string TR = "TR";
        internal const string TT = "TT";
        internal const string U = "U";
        internal const string UL = "UL";
        internal const string VAR = "VAR";

        #endregion

        #region Html Object Constants

        internal const string abbr = "abbr";
        internal const string accept = "accept";
        internal const string accesskey = "accesskey";
        internal const string action = "action";
        internal const string align = "align";
        internal const string alink = "alink";
        internal const string alt = "alt";
        internal const string archive = "archive";
        internal const string axis = "axis";
        internal const string background = "background";
        internal const string bgcolor = "bgcolor";
        internal const string border = "border";
        internal const string bordercolor = "bordercolor";
        internal const string cellpadding = "cellpadding";
        internal const string cellspacing = "cellspacing";
        internal const string char_ = "char";
        internal const string charoff = "charoff";
        internal const string charset = "charset";
        internal const string checked_ = "checked";
        internal const string cite = "cite";
        internal const string class_ = "class";
        internal const string classid = "classid";
        internal const string clear = "clear";
        internal const string code = "code";
        internal const string codebase = "codebase";
        internal const string codetype = "codetype";
        internal const string color = "color";
        internal const string cols = "cols";
        internal const string colspan = "colspan";
        internal const string compact = "compact";
        internal const string content = "content";
        internal const string coords = "coords";
        internal const string data = "data";
        internal const string datetime = "datetime";
        internal const string declare = "declare";
        internal const string defer = "defer";
        internal const string dir = "dir";
        internal const string disabled = "disabled";
        internal const string enctype = "enctype";
        internal const string face = "face";
        internal const string for_ = "for";
        internal const string frame = "frame";
        internal const string frameborder = "frameborder";
        internal const string headers = "headers";
        internal const string height = "height";
        internal const string href = "href";
        internal const string hreflang = "hreflang";
        internal const string hspace = "hspace";
        internal const string http_equiv = "http-equiv";
        internal const string id = "id";
        internal const string ismap = "ismap";
        internal const string label = "label";
        internal const string lang = "lang";
        internal const string language = "language";
        internal const string link = "link";
        internal const string longdesc = "longdesc";
        internal const string marginheight = "marginheight";
        internal const string marginwidth = "marginwidth";
        internal const string maxlength = "maxlength";
        internal const string media = "media";
        internal const string method = "method";
        internal const string multiple = "multiple";
        internal const string name = "name";
        internal const string nohref = "nohref";
        internal const string noresize = "noresize";
        internal const string noshade = "noshade";
        internal const string nowrap = "nowrap";
        internal const string object_ = "object";
        internal const string onblur = "onblur";
        internal const string onchange = "onchange";
        internal const string onclick = "onclick";
        internal const string ondblclick = "ondblclick";
        internal const string onfocus = "onfocus";
        internal const string onkeydown = "onkeydown";
        internal const string onkeypress = "onkeypress";
        internal const string onkeyup = "onkeyup";
        internal const string onload = "onload";
        internal const string onmousedown = "onmousedown";
        internal const string onmousemove = "onmousemove";
        internal const string onmouseout = "onmouseout";
        internal const string onmouseover = "onmouseover";
        internal const string onmouseup = "onmouseup";
        internal const string onreset = "onreset";
        internal const string onselect = "onselect";
        internal const string onsubmit = "onsubmit";
        internal const string onunload = "onunload";
        internal const string profile = "profile";
        internal const string prompt = "prompt";
        internal const string readonly_ = "readonly";
        internal const string rel = "rel";
        internal const string rev = "rev";
        internal const string rows = "rows";
        internal const string rowspan = "rowspan";
        internal const string rules = "rules";
        internal const string scheme = "scheme";
        internal const string scope = "scope";
        internal const string scrolling = "scrolling";
        internal const string selected = "selected";
        internal const string shape = "shape";
        internal const string size = "size";
        internal const string span = "span";
        internal const string src = "src";
        internal const string standby = "standby";
        internal const string start = "start";
        internal const string style = "style";
        internal const string summary = "summary";
        internal const string tabindex = "tabindex";
        internal const string target = "target";
        internal const string text = "text";
        internal const string title = "title";
        internal const string type = "type";
        internal const string usemap = "usemap";
        internal const string valign = "valign";
        internal const string value = "value";
        internal const string valuetype = "valuetype";
        internal const string version = "version";
        internal const string vlink = "vlink";
        internal const string vspace = "vspace";
        internal const string width = "width";

        #endregion

        #region Html Alignment Constants

        internal const string left = "left";
        internal const string right = "right";
        internal const string top = "top";
        internal const string center = "center";
        internal const string middle = "middle";
        internal const string bottom = "bottom";
        internal const string justify = "justify";

        #endregion

        #region Css Constants

        internal const string Absolute = "absolute";
        internal const string Auto = "auto";
        internal const string Baseline = "baseline";
        internal const string Blink = "blink";
        internal const string Block = "block";
        internal const string Bold = "bold";
        internal const string Bolder = "bolder";
        internal const string Bottom = "bottom";
        internal const string Center = "center";
        internal const string Collapse = "collapse";
        internal const string Cursive = "cursive";
        internal const string Decimal = "decimal";
        internal const string Fantasy = "fantasy";
        internal const string Hide = "hide";
        internal const string Inherit = "inherit";
        internal const string Inline = "inline";
        internal const string InlineTable = "inline-table";
        internal const string Inset = "inset";
        internal const string Italic = "italic";
        internal const string Justify = "justify";
        internal const string Large = "large";
        internal const string Larger = "larger";
        internal const string Left = "left";
        internal const string Lighter = "lighter";
        internal const string LineThrough = "line-through";
        internal const string ListItem = "list-item";
        internal const string Ltr = "ltr";
        internal const string Medium = "medium";
        internal const string Middle = "middle";
        internal const string Monospace = "monospace";
        internal const string None = "none";
        internal const string Normal = "normal";
        internal const string Nowrap = "nowrap";
        internal const string Oblique = "oblique";
        internal const string Outset = "outset";
        internal const string Overline = "overline";
        internal const string Pre = "pre";
        internal const string PreWrap = "pre-wrap";
        internal const string PreLine = "pre-line";
        internal const string Right = "right";
        internal const string Rtl = "rtl";
        internal const string SansSerif = "sans-serif";
        internal const string Serif = "serif";
        internal const string Show = "show";
        internal const string Small = "small";
        internal const string Smaller = "smaller";
        internal const string Solid = "solid";
        internal const string Sub = "sub";
        internal const string Super = "super";
        internal const string Table = "table";
        internal const string TableRow = "table-row";
        internal const string TableRowGroup = "table-row-group";
        internal const string TableHeaderGroup = "table-header-group";
        internal const string TableFooterGroup = "table-footer-group";
        internal const string TableColumn = "table-column";
        internal const string TableColumnGroup = "table-column-group";
        internal const string TableCell = "table-cell";
        internal const string TableCaption = "table-caption";
        internal const string TextBottom = "text-bottom";
        internal const string TextTop = "text-top";
        internal const string Thin = "thin";
        internal const string Thick = "thick";
        internal const string Top = "top";
        internal const string Underline = "underline";
        internal const string XLarge = "x-large";
        internal const string XSmall = "x-small";
        internal const string XXLarge = "xx-large";
        internal const string XXSmall = "xx-small";
        
        #endregion

        #region Units

        internal const string Cm = "cm";
        internal const string Mm = "mm";
        internal const string Px = "px";
        internal const string In = "in";
        internal const string Em = "em";
        internal const string Ex = "ex";
        internal const string Pt = "pt";
        internal const string Pc = "pc";

        #endregion

        #region Color Constants

        internal const string Maroon = "maroon";
        internal const string Red = "red";
        internal const string Orange = "orange";
        internal const string Yellow = "yellow";
        internal const string Olive = "olive";
        internal const string Fuchsia = "fuchsia";
        internal const string Lime = "lime";
        internal const string White = "white";
        internal const string Green = "green";
        internal const string Navy = "navy";
        internal const string Blue = "blue";
        internal const string Aqua = "aqua";
        internal const string Teal = "teal";
        internal const string Black = "black";
        internal const string Silver = "silver";
        internal const string Gray = "gray";
        internal const string Purple = "purple";

        #endregion

        #region Css Search Constants

        internal const string CssProperties = @";?[^;\s]*:[^\{\}:;]*(\}|;)?";
        internal const string CssComments = @"/\*[^*/]*\*/";
        internal const string HtmlBlocks = @"[^\{\}]*\{[^\{\}]*\}";
        internal const string CssNumber = @"{[0-9]+|[0-9]*\.[0-9]+}";
        internal const string CssPercentage = @"([0-9]+|[0-9]*\.[0-9]+)\%"; 
        internal const string HtmlLength = @"([0-9]+|[0-9]*\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)";
        internal const string CssColors = @"(#\S{6}|#\S{3}|rgb\(\s*[0-9]{1,3}\%?\s*\,\s*[0-9]{1,3}\%?\s*\,\s*[0-9]{1,3}\%?\s*\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)";
        internal const string CssLineHeight = "(normal|" + CssNumber + "|" + HtmlLength + "|" + CssPercentage + ")";
        internal const string CssBorderStyle = @"(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)";
        internal const string CssBorderWidth = "(" + HtmlLength + "|thin|medium|thick)";
        internal const string CssFontFamily = "(\"[^\"]*\"|'[^']*'|\\S+\\s*)(\\s*\\,\\s*(\"[^\"]*\"|'[^']*'|\\S+))*";
        internal const string CssFontStyle = "(normal|italic|oblique)";
        internal const string CssFontVariant = "(normal|small-caps)";
        internal const string CssFontWeight = "(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)";
        internal const string CssFontSize = "(" + HtmlLength + "|" + CssPercentage + "|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)";
        internal const string CssFontSizeAndLineHeight = CssFontSize + @"(\/" + CssLineHeight + @")?(\s|$)";
        internal const string HtmlTag = @"<[^<>]*>";
        internal const string HmlTagAttributes = "[^\\s]*\\s*=\\s*(\"[^\"]*\"|[^\\s]*)";

        #endregion

        #region Css Default Constants

        internal const string DefaultStyleSheet = @"
        html, address,
        blockquote,
        body, dd, div,
        dl, dt, fieldset, form,
        frame, frameset,
        h1, h2, h3, h4,
        h5, h6, noframes,
        ol, p, ul, center,
        dir, hr, menu, pre   { display: block }
        li              { display: list-item }
        head            { display: none }
        table           { display: table }
        tr              { display: table-row }
        thead           { display: table-header-group }
        tbody           { display: table-row-group }
        tfoot           { display: table-footer-group }
        col             { display: table-column }
        colgroup        { display: table-column-group }
        td, th          { display: table-cell }
        caption         { display: table-caption }
        th              { font-weight: bolder; text-align: center }
        caption         { text-align: center }
        body            { margin: 8px }
        h1              { font-size: 2em; margin: .67em 0 }
        h2              { font-size: 1.5em; margin: .75em 0 }
        h3              { font-size: 1.17em; margin: .83em 0 }
        h4, p,
        blockquote, ul,
        fieldset, form,
        ol, dl, dir,
        menu            { margin: 1.12em 0 }
        h5              { font-size: .83em; margin: 1.5em 0 }
        h6              { font-size: .75em; margin: 1.67em 0 }
        h1, h2, h3, h4,
        h5, h6, b,
        strong          { font-weight: bolder; }
        blockquote      { margin-left: 40px; margin-right: 40px }
        i, cite, em,
        var, address    { font-style: italic }
        pre, tt, code,
        kbd, samp       { font-family: monospace }
        pre             { white-space: pre }
        button, textarea,
        input, select   { display: inline-block }
        big             { font-size: 1.17em }
        small, sub, sup { font-size: .83em }
        sub             { vertical-align: sub }
        sup             { vertical-align: super }
        table           { border-spacing: 2px; }
        thead, tbody,
        tfoot           { vertical-align: middle }
        td, th          { vertical-align: inherit }
        s, strike, del  { text-decoration: line-through }
        hr              { border: 1px inset }
        ol, ul, dir,
        menu, dd        { margin-left: 40px }
        ol              { list-style-type: decimal }
        ol ul, ul ol,
        ul ul, ol ol    { margin-top: 0; margin-bottom: 0 }
        u, ins          { text-decoration: underline }
        br:before       { content: ""\A"" }
        :before, :after { white-space: pre-line }
        center          { text-align: center }
        :link, :visited { text-decoration: underline }
        :focus          { outline: thin dotted invert }
        a               { color:blue; text-decoration:underline }
        table           { border-color:#dfdfdf; border-style:outset; }
        td, th          { border-color:#dfdfdf; border-style:inset; }
        style, title,
        script, link,
        meta, area,
        base, param     { display:none }
        hr              { border-color: #ccc }  
        pre             { font-size:10pt }
        ";

        internal static float FontSize = 12f;
        internal static string FontSerif = System.Drawing.FontFamily.GenericSerif.Name;
        internal static string FontSansSerif = System.Drawing.FontFamily.GenericSansSerif.Name;
        internal static string FontCursive = "Monotype Corsiva";
        internal static string FontFantasy = "Comic Sans MS";
        internal static string FontMonospace = System.Drawing.FontFamily.GenericMonospace.Name;

        #endregion
    }
    
    internal struct Do
    {

        /// <summary>
        /// Matches the pattern.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal static MatchCollection MatchPattern(string regex, string source)
        {
            Regex regExp = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return regExp.Matches(source);
        }

        /// <summary>
        /// Searches the pattern.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal static string SearchPattern(string regex, string source)
        {
            int position;
            return SearchPattern(regex, source, out position);
        }

        /// <summary>
        /// Searches the pattern.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="source">The source.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        internal static string SearchPattern(string regex, string source, out int position)
        {
            MatchCollection matches = MatchPattern(regex, source);

            if (matches.Count > 0)
            {
                position = matches[0].Index;
                return matches[0].Value;
            }
            else
            {
                position = -1;
            }

            return null;
        }

        /// <summary>
        /// Parses the length.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="hundredPercent">The hundred percent.</param>
        /// <returns></returns>
        internal static float ParseLength(string number, float hundredPercent)
        {
            if (string.IsNullOrEmpty(number))
            {
                return 0f;
            }

            string toParse = number;
            bool isPercent = number.EndsWith("%");
            float result = 0f;

            if (isPercent) toParse = number.Substring(0, number.Length - 1);

            if (!float.TryParse(toParse, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out result))
            {
                return 0f;
            }

            if (isPercent)
            {
                result = (result / 100f) * hundredPercent;
            }

            return result;
        }

        /// <summary>
        /// Parses the length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="hundredPercent">The hundred percent.</param>
        /// <param name="box">The box.</param>
        /// <returns></returns>
        internal static float ParseLength(string length, float hundredPercent, HtmlBox box)
        {
            return ParseLength(length, hundredPercent, box, box.GetEmHeight(), false);
        }

        /// <summary>
        /// Parses the length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="hundredPercent">The hundred percent.</param>
        /// <param name="box">The box.</param>
        /// <param name="emFactor">The em factor.</param>
        /// <param name="returnPoints">if set to <c>true</c> [return points].</param>
        /// <returns></returns>
        internal static float ParseLength(string length, float hundredPercent, HtmlBox box, float emFactor, bool returnPoints)
        {
            if (string.IsNullOrEmpty(length) || length == "0") return 0f;
            if (length.EndsWith("%")) return ParseLength(length, hundredPercent);
            if (length.Length < 3) return 0f;
            string unit = length.Substring(length.Length - 2, 2);
            float factor = 1f;
            string number = length.Substring(0, length.Length - 2);
            switch (unit)
            {
                case Constants.Em:
                    factor = emFactor;
                    break;
                case Constants.Px:
                    factor = 1f;
                    break;
                case Constants.Mm:
                    factor = 3f; 
                    break;
                case Constants.Cm:
                    factor = 37f; 
                    break;
                case Constants.In:
                    factor = 96f; 
                    break;
                case Constants.Pt:
                    factor = 96f / 72f; 

                    if (returnPoints)
                    {
                        return ParseLength(number, hundredPercent);
                    }

                    break;
                case Constants.Pc:
                    factor = 96f / 72f * 12f;
                    break;
                default:
                    factor = 0f;
                    break;
            }

            

            return factor * ParseLength(number, hundredPercent);
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="colorValue">The color value.</param>
        /// <returns></returns>
        internal static Color GetColor(string colorValue)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            Color onError = Color.Empty;

            if (string.IsNullOrEmpty(colorValue)) return onError;

            colorValue = colorValue.ToLower().Trim();

            if (colorValue.StartsWith("#"))
            {
                string hex = colorValue.Substring(1);

                if (hex.Length == 6)
                {
                    r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                }
                else if (hex.Length == 3)
                {
                    r = int.Parse(new String(hex.Substring(0, 1)[0], 2), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(new String(hex.Substring(1, 1)[0], 2), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(new String(hex.Substring(2, 1)[0], 2), System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    return onError;
                } 
            }
            else if (colorValue.StartsWith("rgb(") && colorValue.EndsWith(")"))
            {

                string rgb = colorValue.Substring(4, colorValue.Length - 5);
                string[] chunks = rgb.Split(',');

                if (chunks.Length == 3)
                {
                    unchecked
                    {
                        r = Convert.ToInt32(ParseLength(chunks[0].Trim(), 255f));
                        g = Convert.ToInt32(ParseLength(chunks[1].Trim(), 255f));
                        b = Convert.ToInt32(ParseLength(chunks[2].Trim(), 255f)); 
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
                    case Constants.Maroon:
                        hex = "#800000"; break;
                    case Constants.Red:
                        hex = "#ff0000"; break;
                    case Constants.Orange:
                        hex = "#ffA500"; break;
                    case Constants.Olive:
                        hex = "#808000"; break;
                    case Constants.Purple:
                        hex = "#800080"; break;
                    case Constants.Fuchsia:
                        hex = "#ff00ff"; break;
                    case Constants.White:
                        hex = "#ffffff"; break;
                    case Constants.Lime:
                        hex = "#00ff00"; break;
                    case Constants.Green:
                        hex = "#008000"; break;
                    case Constants.Navy:
                        hex = "#000080"; break;
                    case Constants.Blue:
                        hex = "#0000ff"; break;
                    case Constants.Aqua:
                        hex = "#00ffff"; break;
                    case Constants.Teal:
                        hex = "#008080"; break;
                    case Constants.Black:
                        hex = "#000000"; break;
                    case Constants.Silver:
                        hex = "#c0c0c0"; break;
                    case Constants.Gray:
                        hex = "#808080"; break;
                    case Constants.Yellow:
                        hex = "#FFFF00"; break;
                }

                if (string.IsNullOrEmpty(hex))
                {
                    return onError;
                }
                else
                {
                    Color c = GetColor(hex);
                    r = c.R;
                    g = c.G;
                    b = c.B;
                }
            }

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Gets the width of the border.
        /// </summary>
        /// <param name="borderValue">The border value.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        internal static float GetBorderWidth(string borderValue, HtmlBox b)
        {
            if (string.IsNullOrEmpty(borderValue))
            {
                return GetBorderWidth(Constants.Medium, b);
            }

            switch (borderValue)
            {
                case Constants.Thin:
                    return 1f;
                case Constants.Medium:
                    return 2f;
                case Constants.Thick:
                    return 4f;
                default:
                    return Math.Abs(ParseLength(borderValue, 1, b));
            }
        }

        /// <summary>
        /// Splits the values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static string[] SplitValues(string value)
        {
            return SplitValues(value, ' ');
        }

        /// <summary>
        /// Splits the values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        internal static string[] SplitValues(string value, char separator)
        {
            if (string.IsNullOrEmpty(value)) return new string[] { };

            string[] values = value.Split(separator);
            List<string> result = new List<string>();

            for (int i = 0; i < values.Length; i++)
            {
                string val = values[i].Trim();

                if (!string.IsNullOrEmpty(val))
                {
                    result.Add(val);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="moreInfo">The more info.</param>
        /// <returns></returns>
        private static Type GetTypeInfo(string path, ref string moreInfo)
        {
            int lastDot = path.LastIndexOf('.');

            if (lastDot < 0)
                return null;

            string type = path.Substring(0, lastDot);
            moreInfo = path.Substring(lastDot + 1);
            if (moreInfo.IndexOf("(") > 0)
                moreInfo = moreInfo.Substring(0, moreInfo.IndexOf("(") + 1);
            moreInfo = moreInfo.Replace("(", string.Empty).Replace(")", string.Empty);
            
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = a.GetType(type, false, true);

                if (t != null) 
                    return t;
            }

            return null;
        }

        /// <summary>
        /// Finds the style sheet source.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private static object FindStyleSheetSource(string path)
        {
            if (path.StartsWith("class-method:", StringComparison.CurrentCultureIgnoreCase))
            {
                string methodName = string.Empty;
                Type t = GetTypeInfo(path.Substring(13), ref methodName); 

                if (t == null) 
                    return null;

                MethodInfo method = t.GetMethod(methodName);

                if (!method.IsStatic)
                {
                    return null;
                }

                return method;
            }
            else if (path.StartsWith("class-property:", StringComparison.CurrentCultureIgnoreCase))
            {
                string propName = string.Empty;
                Type t = GetTypeInfo(path.Substring(15), ref propName); 

                if (t == null) 
                    return null;

                PropertyInfo prop = t.GetProperty(propName);

                return prop;
            }
            else if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            {
                return new Uri(path);
            }
            else
            {
                return new FileInfo(path);
            }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static Image GetImage(string path)
        {
            object source = FindStyleSheetSource(path);

            FileInfo finfo = source as FileInfo;
            PropertyInfo propertyInfo = source as PropertyInfo;
            MethodInfo method = source as MethodInfo;

            try
            {
                if (finfo != null)
                {
                    if (!finfo.Exists) return null;

                    return Image.FromFile(finfo.FullName);

                }
                else if (propertyInfo != null)
                {
                    if (!propertyInfo.PropertyType.Equals(typeof(Image))) 
                        return null;
                    
                    return propertyInfo.GetValue(null, null) as Image;
                }
                else if (method != null)
                {
                    if (!method.ReturnType.Equals(typeof(Image))) 
                        return null;

                    if (method.GetParameters().Length > 0)
                    {
                        Object[] parameters = new Object[method.GetParameters().Length];
                        string value = path;
                        value = value.Substring(value.IndexOf("(") + 1, value.IndexOf(")") - value.IndexOf("(") - 1);
                        string[] values = value.Split(',');
                        for (int i = 0; i < method.GetParameters().Length; i++)
                        {
                            parameters.SetValue(values[i], i);
                        }
                        return method.Invoke(null, parameters) as Image;
                    }
                    else
                        return method.Invoke(null, null) as Image;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return new Bitmap(50, 50); 
            }
        }

        /// <summary>
        /// Gets the style sheet.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static string GetStyleSheet(string path)
        {
            object source = FindStyleSheetSource(path);

            FileInfo finfo = source as FileInfo;
            PropertyInfo propertyInfo = source as PropertyInfo;
            MethodInfo method = source as MethodInfo;

            try
            {
                if (finfo != null)
                {
                    if (!finfo.Exists) return null;

                    StreamReader streamReader = new StreamReader(finfo.FullName);
                    string result = streamReader.ReadToEnd();
                    streamReader.Dispose();

                    return result;
                }
                else if (propertyInfo != null)
                {
                    if (!propertyInfo.PropertyType.Equals(typeof(string))) return null;

                    return propertyInfo.GetValue(null, null) as string;
                }
                else if (method != null)
                {
                    if (!method.ReturnType.Equals(typeof(string))) return null;

                    return method.Invoke(null, null) as string;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Removes the comments.
        /// </summary>
        /// <param name="StyleSheetToProcess">The style sheet to process.</param>
        internal static void RemoveComments ( ref string StyleSheetToProcess)
        {
            for (MatchCollection comments = Do.MatchPattern(Constants.CssComments, StyleSheetToProcess); comments.Count > 0; comments = Do.MatchPattern(Constants.CssComments, StyleSheetToProcess))
            {
                StyleSheetToProcess = StyleSheetToProcess.Remove(comments[0].Index, comments[0].Length);
            }
        }

        /// <summary>
        /// Finds the parent box.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="b">The b.</param>
        /// <param name="InitContainer">The init container.</param>
        /// <returns></returns>
        internal static HtmlBox FindParentBox(string tagName, HtmlBox box, HtmlRootBox rootBox)
        {
            if (box == null)
            {
                return rootBox;
            }
            else if (box.HtmlTag != null && box.HtmlTag.TagName.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
            {
                return box.ParentBox == null ? rootBox : box.ParentBox;
            }
            else
            {
                return FindParentBox(tagName, box.ParentBox, rootBox);
            }
        }

        /// <summary>
        /// Darks the color.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        internal static Color DarkTheColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.R / 2, c.G / 2, c.B / 2);
        }

        internal enum Border
        {
            Top, Right, Bottom, Left
        }

        /// <summary>
        /// Gets the border path.
        /// </summary>
        /// <param name="border">The border.</param>
        /// <param name="b">The b.</param>
        /// <param name="r">The r.</param>
        /// <param name="isLineStart">if set to <c>true</c> [is line start].</param>
        /// <param name="isLineEnd">if set to <c>true</c> [is line end].</param>
        /// <returns></returns>
        internal static GraphicsPath GetBorderPath(Border border, HtmlBox b, RectangleF r, bool isLineStart, bool isLineEnd)
        {
            PointF[] pts = new PointF[4];
            float bwidth = 0;

            switch (border)
            {
                case Border.Top:
                    bwidth = b.ParseFloat(b.Border_Top_Width,"BORDER");
                    pts[0] = new PointF(r.Left, r.Top);
                    pts[1] = new PointF(r.Right, r.Top);
                    pts[2] = new PointF(r.Right, r.Top + bwidth);
                    pts[3] = new PointF(r.Left, r.Top + bwidth);
                    if (isLineEnd) pts[2].X -= b.ParseFloat(b.Border_Right_Width, "BORDER");
                    if (isLineStart) pts[3].X += b.ParseFloat(b.Border_Left_Width, "BORDER");
                    break;
                case Border.Right:
                    bwidth = b.ParseFloat(b.Border_Right_Width, "BORDER");
                    pts[0] = new PointF(r.Right - bwidth, r.Top);
                    pts[1] = new PointF(r.Right, r.Top);
                    pts[2] = new PointF(r.Right, r.Bottom);
                    pts[3] = new PointF(r.Right - bwidth, r.Bottom);
                    pts[0].Y += b.ParseFloat(b.Border_Top_Width, "BORDER");
                    pts[3].Y -= b.ParseFloat(b.Bottom_Border_Width, "BORDER");
                    break;
                case Border.Bottom:
                    bwidth = b.ParseFloat(b.Bottom_Border_Width, "BORDER");
                    pts[0] = new PointF(r.Left, r.Bottom - bwidth);
                    pts[1] = new PointF(r.Right, r.Bottom - bwidth);
                    pts[2] = new PointF(r.Right, r.Bottom);
                    pts[3] = new PointF(r.Left, r.Bottom);
                    if (isLineStart) pts[0].X += b.ParseFloat(b.Border_Left_Width, "BORDER");
                    if (isLineEnd) pts[1].X -= b.ParseFloat(b.Border_Right_Width, "BORDER");
                    break;
                case Border.Left:
                    bwidth = b.ParseFloat(b.Border_Left_Width, "BORDER");
                    pts[0] = new PointF(r.Left, r.Top);
                    pts[1] = new PointF(r.Left + bwidth, r.Top);
                    pts[2] = new PointF(r.Left + bwidth, r.Bottom);
                    pts[3] = new PointF(r.Left, r.Bottom);

                    pts[1].Y += b.ParseFloat(b.Border_Top_Width, "BORDER");
                    pts[2].Y -= b.ParseFloat(b.Bottom_Border_Width, "BORDER");
                    break;
            }

            GraphicsPath path = new GraphicsPath(pts, new byte[] { (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });
            return path;
        }

        /// <summary>
        /// Constructs the line boxes.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="blockBox">The block box.</param>
        internal static void ConstructLineBoxes(Graphics g, HtmlBox blockBox)
        {

            blockBox.LineBoxes.Clear();

            float maxRight = blockBox.ActualRight - blockBox.ParseFloat(blockBox.Padding_Right, "FLOAT") - blockBox.ParseFloat(blockBox.Border_Right_Width, "BORDER");

            float startx = blockBox.Location.X + blockBox.ParseFloat(blockBox.Padding_Left, "FLOAT") - 0 + blockBox.ParseFloat(blockBox.Border_Left_Width, "BORDER");
            float starty = blockBox.Location.Y + blockBox.ParseFloat(blockBox.Padding_Top, "FLOAT") - 0 + blockBox.ParseFloat(blockBox.Border_Top_Width, "BORDER");
            float curx = startx + blockBox.ParseFloat(blockBox.Text_Indent, "FLOAT");
            float cury = starty;

            float maxBottom = starty;

            float lineSpacing = 0f;

            HtmlLineBox line = new HtmlLineBox(blockBox);

            CalcTopBottomSpacing(g, blockBox, blockBox, maxRight, lineSpacing, startx, ref line, ref curx, ref cury, ref maxBottom);

            foreach (HtmlLineBox linebox in blockBox.LineBoxes)
            {
                BubbleRectangles(blockBox, linebox);
                linebox.AssignRectanglesToBoxes();
                Align(g, linebox);
                if (blockBox.Direction == Constants.Rtl) AlignRightToLeft(linebox);
            }

            blockBox.ActualBottom = maxBottom + blockBox.ParseFloat(blockBox.Padding_Bottom, "FLOAT") + blockBox.ParseFloat(blockBox.Bottom_Border_Width, "BORDER");
        }

        private static void CalcTopBottomSpacing(Graphics g, HtmlBox blockbox, HtmlBox box, float maxright, float linespacing, float startx, ref HtmlLineBox line, ref float curx, ref float cury, ref float maxbottom)
        {
            box.FirstHostingLineBox = line;

            foreach (HtmlBox htBox in box.Boxes)
            {

                float leftspacing = htBox.ParseFloat(htBox.Margin_Left, "FLOAT") + htBox.ParseFloat(htBox.Border_Left_Width, "BORDER") + htBox.ParseFloat(htBox.Padding_Left, "FLOAT");
                float rightspacing = htBox.ParseFloat(htBox.Margin_Right, "FLOAT") + htBox.ParseFloat(htBox.Border_Right_Width, "BORDER") + htBox.ParseFloat(htBox.Padding_Right, "FLOAT");
                float topspacing = htBox.ParseFloat(htBox.Border_Top_Width, "BORDER") + htBox.ParseFloat(htBox.Padding_Top, "FLOAT");
                float bottomspacing = htBox.ParseFloat(htBox.Bottom_Border_Width, "BORDER") + htBox.ParseFloat(htBox.Padding_Bottom, "FLOAT");

                htBox.ClearRectangles();
                htBox.CalculateWordsSize(g);

                curx += leftspacing;

                if (htBox.HtmlTag != null)
                {
                    if (htBox.HtmlTag.TagName.ToLower() == "br")
                    {
                        curx = startx;
                        cury = maxbottom + 1f;
                        line = new HtmlLineBox(blockbox);
                    }
                }

                if (htBox.Words.Count > 0)
                {

                    foreach (HtmlWordBox word in htBox.Words)
                    {
                        if ((htBox.White_Space != Constants.Nowrap && curx + word.Width + rightspacing > maxright) ||
                            word.IsLineBreak)
                        {

                            curx = startx;
                            cury = maxbottom + linespacing;

                            line = new HtmlLineBox(blockbox);

                            if (word.IsImage || word.Equals(htBox.Words[0]))
                            {
                                curx += leftspacing;
                            }

                        }

                        line.AddWordBox(word);

                        word.Left = curx;
                        word.Top = cury;

                        curx = word.Right;
                        maxbottom = Math.Max(maxbottom, word.Bottom);

                    }

                }
                else
                {
                    CalcTopBottomSpacing(g, blockbox, htBox, maxright, linespacing, startx, ref line, ref curx, ref cury, ref maxbottom);
                }

                curx += rightspacing;
            }

            box.LastHostingLineBox = line;
        }

        private static void BubbleRectangles(HtmlBox box, HtmlLineBox line)
        {
            if (box.Words.Count > 0)
            {
                float x = float.MaxValue, y = float.MaxValue, r = float.MinValue, b = float.MinValue;
                List<HtmlWordBox> words = line.GetWordBoxes(box);

                if (words.Count > 0)
                {
                    foreach (HtmlWordBox word in words)
                    {
                        x = Math.Min(x, word.Left);
                        r = Math.Max(r, word.Right);
                        y = Math.Min(y, word.Top);
                        b = Math.Max(b, word.Bottom);
                    }
                    line.ChangeRectangle(box, x, y, r, b);
                }
            }
            else
            {
                foreach (HtmlBox htmlBox in box.Boxes)
                {
                    BubbleRectangles(htmlBox, line);
                }
            }
        }

        internal static float WhiteSpace(Graphics g, HtmlBox box)
        {
            string space = " .";
            float width = 0f;
            float onError = 5f;

            StringFormat stringFormat = new StringFormat();
            stringFormat.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, 1) });
            Region[] regions = g.MeasureCharacterRanges(space, box.GetFont(), new RectangleF(0, 0, float.MaxValue, float.MaxValue), stringFormat);

            if (regions == null || regions.Length == 0) return onError;

            width = regions[0].GetBounds(g).Width;

            if (!(string.IsNullOrEmpty(box.Word_Spacing) || box.Word_Spacing == Constants.Normal))
            {
                width += Do.ParseLength(box.Word_Spacing, 0, box);
            }
            return width;
        }

        /// <summary>
        /// Does the alignment.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="lineBox">The line box.</param>
        private static void Align(Graphics g, HtmlLineBox lineBox)
        {

            #region Horizontal alignment

            switch (lineBox.OwnerBox.Text_Align)
            {
                case Constants.Right:
                    RightAlign(g, lineBox);
                    break;
                case Constants.Center:
                    CenterAlign(g, lineBox);
                    break;
                case Constants.Justify:
                    JustifyAlign(g, lineBox);
                    break;
                default:
                    break;
            }

            #endregion

            VerticalAlign(g, lineBox);
        }

        /// <summary>
        /// Aligns righttoleft.
        /// </summary>
        /// <param name="line">The line.</param>
        private static void AlignRightToLeft(HtmlLineBox line)
        {
            float left = line.OwnerBox.ClientLeft;
            float right = line.OwnerBox.ClientRight;

            foreach (HtmlWordBox word in line.Words)
            {
                float diff = word.Left - left;
                float wright = right - diff;
                word.Left = wright - word.Width;
            }
        }

        /// <summary>
        /// Gets the ascent.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        internal static float GetAscent(Font f)
        {
            float mainAscent = f.Size * f.FontFamily.GetCellAscent(f.Style) / f.FontFamily.GetEmHeight(f.Style);
            return mainAscent;
        }

        /// <summary>
        /// Gets the descent.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        internal static float GetDescent(Font font)
        {
            float mainDescent = font.Size * font.FontFamily.GetCellDescent(font.Style) / font.FontFamily.GetEmHeight(font.Style);
            return mainDescent;
        }

        /// <summary>
        /// Gets the line spacing.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        internal static float GetLineSpacing(Font font)
        {
            float s = font.Size * font.FontFamily.GetLineSpacing(font.Style) / font.FontFamily.GetEmHeight(font.Style);
            return s;
        }

        /// <summary>
        /// Vertical Alignment
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="lineBox">The line box.</param>
        private static void VerticalAlign(Graphics g, HtmlLineBox lineBox)
        {

            bool isTableCell = lineBox.OwnerBox.Display == Constants.TableCell;
            float baseline = lineBox.GetMaxWordBottom() - GetDescent(lineBox.OwnerBox.GetFont()) - 2;
            List<HtmlBox> boxes = new List<HtmlBox>(lineBox.Rectangles.Keys);

            foreach (HtmlBox box in boxes)
            {
                float ascent = GetAscent(box.GetFont());
                float descent = GetDescent(box.GetFont());

                switch (box.VerticalAlign)
                {
                    case Constants.Sub:
                        lineBox.SetBaseLine(g, box, baseline + lineBox.Rectangles[box].Height * .2f);
                        break;
                    case Constants.Super:
                        lineBox.SetBaseLine(g, box, baseline - lineBox.Rectangles[box].Height * .2f);
                        break;
                    case Constants.TextTop:

                        break;
                    case Constants.TextBottom:

                        break;
                    case Constants.Top:

                        break;
                    case Constants.Bottom:

                        break;
                    case Constants.Middle:

                        break;
                    default:
                        lineBox.SetBaseLine(g, box, baseline);
                        break;
                }

            }


        }

        /// <summary>
        /// vertical cell alignment.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="cell">The cell.</param>
        internal static void CellVerticalAlign(Graphics g, HtmlBox cell)
        {
            if (cell.VerticalAlign == Constants.Top || cell.VerticalAlign == Constants.Baseline) return;

            float celltop = cell.ClientTop;
            float cellbot = cell.ClientBottom;
            float bottom = cell.GetMaxBottom(cell, 0f);
            float dist = 0f;

            if (cell.VerticalAlign == Constants.Bottom)
            {
                dist = cellbot - bottom;
            }
            else if (cell.VerticalAlign == Constants.Middle)
            {
                dist = (cellbot - bottom) / 2;
            }

            foreach (HtmlBox box in cell.Boxes)
            {
                box.OffsetTop(dist);
            }
        }

        /// <summary>
        /// Justify Alignment.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="lineBox">The line box.</param>
        private static void JustifyAlign(Graphics g, HtmlLineBox lineBox)
        {
            if (lineBox.Equals(lineBox.OwnerBox.LineBoxes[lineBox.OwnerBox.LineBoxes.Count - 1])) return;

            float indent = lineBox.Equals(lineBox.OwnerBox.LineBoxes[0]) ? lineBox.OwnerBox.ParseFloat(lineBox.OwnerBox.Text_Indent,"FLOAT") : 0f;
            float textSum = 0f;
            float words = 0f;
            float availWidth = lineBox.OwnerBox.ClientRectangle.Width - indent;

            #region Gather text sum
            foreach (HtmlWordBox word in lineBox.Words)
            {
                textSum += word.Width;
                words += 1f;
            }
            #endregion

            if (words <= 0f) return; 
            float spacing = (availWidth - textSum) / words; 
            float curx = lineBox.OwnerBox.ClientLeft + indent;

            foreach (HtmlWordBox word in lineBox.Words)
            {
                word.Left = curx;
                curx = word.Right + spacing;

                if (word == lineBox.Words[lineBox.Words.Count - 1])
                {
                    word.Left = lineBox.OwnerBox.ClientRight - word.Width;
                }
            }
        }

        /// <summary>
        /// Center Alignment.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="line">The line.</param>
        private static void CenterAlign(Graphics g, HtmlLineBox line)
        {
            if (line.Words.Count == 0) return;

            HtmlWordBox lastWord = line.Words[line.Words.Count - 1];
            float right = line.OwnerBox.ActualRight - line.OwnerBox.ParseFloat(line.OwnerBox.Padding_Right,"FLOAT") - line.OwnerBox.ParseFloat(line.OwnerBox.Border_Right_Width,"BORDER");
            float diff = right - lastWord.Right - lastWord.LastMeasureOffset.X - lastWord.OwnerBox.ParseFloat(lastWord.OwnerBox.Border_Right_Width, "BORDER") - lastWord.OwnerBox.ParseFloat(lastWord.OwnerBox.Padding_Right, "FLOAT");
            diff /= 2;

            if (diff <= 0) return;

            foreach (HtmlWordBox word in line.Words)
            {
                word.Left += diff;
            }

            foreach (HtmlBox htmlBox in line.Rectangles.Keys)
            {
                RectangleF rectangleF = htmlBox.Rectangles[line];
                htmlBox.Rectangles[line] = new RectangleF(rectangleF.X + diff, rectangleF.Y, rectangleF.Width, rectangleF.Height);
            }
        }

        /// <summary>
        /// Right Alignment.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="line">The line.</param>
        private static void RightAlign(Graphics g, HtmlLineBox line)
        {
            if (line.Words.Count == 0) return;


            HtmlWordBox lastWord = line.Words[line.Words.Count - 1];
            float right = line.OwnerBox.ActualRight - line.OwnerBox.ParseFloat(line.OwnerBox.Padding_Right, "FLOAT") - line.OwnerBox.ParseFloat(line.OwnerBox.Border_Right_Width, "BORDER");
            float diff = right - lastWord.Right - lastWord.LastMeasureOffset.X - lastWord.OwnerBox.ParseFloat(lastWord.OwnerBox.Border_Right_Width, "BORDER") - lastWord.OwnerBox.ParseFloat(lastWord.OwnerBox.Padding_Right, "FLOAT");


            if (diff <= 0) return;

            foreach (HtmlWordBox word in line.Words)
            {
                word.Left += diff;
            }

            foreach (HtmlBox box in line.Rectangles.Keys)
            {
                RectangleF rectangle = box.Rectangles[line];
                box.Rectangles[line] = new RectangleF(rectangle.X + diff, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        /// <summary>
        /// Splits the words.
        /// </summary>
        /// <param name="Box">The box.</param>
        /// <param name="WordText">The word text.</param>
        /// <param name="Words">The words.</param>
        internal static void SplitWords(HtmlBox Box, string WordText, List<HtmlWordBox> Words)
        {
            HtmlBox box = Box;
            string text = WordText.Replace("\r", string.Empty);
            List<HtmlWordBox> words = new List<HtmlWordBox>();
            HtmlWordBox curword = null;
            Words.Clear();

            if (string.IsNullOrEmpty(text)) return;

            curword = new HtmlWordBox(box);

            bool onspace = (text[0] == ' ' || text[0] == '\t' || text[0] == '\n');

            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i] == ' ' || text[i] == '\t' || text[i] == '\n'))
                {
                    if (!onspace)
                    {
                        if (curword.Text.Length > 0)
                            words.Add(curword);
                        curword = new HtmlWordBox(Box);
                    }

                    if (text[i] == '\n' || text[i] == '\a')
                    {
                        curword.AppendChar('\n');
                        if (curword.Text.Length > 0)
                            words.Add(curword);
                        curword = new HtmlWordBox(Box);
                    }
                    else if (text[i] == '\t')
                    {
                        curword.AppendChar('\t');
                        if (curword.Text.Length > 0)
                            words.Add(curword);
                        curword = new HtmlWordBox(Box);
                    }
                    else
                    {
                        curword.AppendChar(' ');
                    }

                    onspace = true;
                }
                else
                {
                    if (onspace)
                    {
                        if (curword.Text.Length > 0)
                            words.Add(curword);
                        curword = new HtmlWordBox(Box);
                    }
                    curword.AppendChar(text[i]);

                    onspace = false;
                }
            }

            if (curword.Text.Length > 0)
                words.Add(curword);
            curword = new HtmlWordBox(Box);

            Words.AddRange(words);
        }

        internal enum Unit
        {
            None,

            Ems,

            Pixels,

            Ex,

            Inches,

            Centimeters,

            Milimeters,

            Points,

            Picas
        }

        /// <summary>
        /// Gets the property default value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        internal static string GetPropertyDefaultValue(string Value)
        {
            string returnValue;
            switch (Value.ToLower().Replace("_", "-"))
            {
                case "border-bottom-width":
                case "border-left-width":
                case "border-right-width":
                case "border-top-width":
                case "font-size":
                    returnValue = "medium";
                    break;
                case "border-bottom-style":
                case "border-left-style":
                case "float":
                case "border-right-style":
                case "border-top-style":
                case "background-image":
                    returnValue = "none";
                    break;
                case "line-height":
                case "white-space":
                case "word-spacing":
                case "font-style":
                case "font-variant":
                case "font-weight":
                    returnValue = "normal";
                    break;
                case "color":
                case "border-color":
                case "border-bottom-color":
                case "border-right-color":
                case "border-top-color":
                case "border-left-color":
                    returnValue = "black";
                    break;
                case "border-spacing":
                case "margin-bottom":
                case "margin-left":
                case "margin-top":
                case "margin-right":
                case "padding-bottom":
                case "padding-left":
                case "padding-right":
                case "padding-top":
                case "text-indent":
                    returnValue = "0";
                    break;
                case "width":
                case "height":
                    returnValue = "auto";
                    break;
                case "border-collapse":
                    returnValue = "separate";
                    break;
                case "background-color":
                    returnValue = "transparent";
                    break;
                case "background-repeat":
                    returnValue = "repeat";
                    break;
                case "display":
                    returnValue = "inline";
                    break;
                case "direction":
                    returnValue = "ltr";
                    break;
                case "empty-cells":
                    returnValue = "show";
                    break;
                case "position":
                    returnValue = "static";
                    break;
                case "vertical-align":
                    returnValue = "baseline";
                    break;
                case "font-family":
                    returnValue = "serif";
                    break;
                case "list-style-position":
                    returnValue = "outside";
                    break;
                case "list-style-type":
                    returnValue = "disc";
                    break;
                default:
                    returnValue = "";
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// Determines whether the specified property is valid property.
        /// </summary>
        /// <param name="Property">The property.</param>
        /// <param name="sType">Type of the s.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid property] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsValidProperty(string Property, string sType)
        {
            bool IsTrue;
            IsTrue = false;
            if (sType == "inherited")
                switch (Property.ToLower().Replace("_", "-"))
                {
                    case "border-spacing":case "border-collapse":case "color":
                    case "empty-cells":case "vertical-align":case "text-indent":
                    case "text-align":case "white-space":case "font":
                    case "font-style":case "font-family":case "font-variant":
                    case "font-size":case "font-weight":case "list-style":
                    case "list-style-position":case "list-style-image":case "list-style-type":
                        IsTrue = true;
                        break;
                }
            if (sType != "inherited")
                switch (Property.ToLower().Replace("_", "-"))
                {
                    case "border-bottom-width":case "border-left-width":case "border-right-width":
                    case "border-top-width":case "border-width":case "border-bottom-style":
                    case "border-left-style":case "border-right-style":case "border-style":
                    case "border-top-style":case "border-color":case "border-bottom-color":
                    case "border-left-color":case "border-right-color":case "border-top-color":
                    case "border":case "border-bottom":case "border-left":case "border-right":
                    case "border-top":case "border-spacing":case "border-collapse":case "margin":
                    case "margin-bottom":case "margin-left":case "margin-right":case "margin-top":
                    case "padding":case "padding-bottom":case "padding-left":case "padding-right":
                    case "padding-top":case "width":case "height":case "background-color":case "background-image":
                    case "background-repeat":case "color":case "display":case "direction":
                    case "empty-cells":case "float":case "position":case "line-height":
                    case "vertical-align":case "text-indent":case "text-align":
                    case "text-decoration":case "white-space":case "word-spacing":case "font":
                    case "font-style":case "font-family":case "font-variant":case "font-size":
                    case "font-weight":case "list-style":case "list-style-position":
                    case "list-style-image":case "list-style-type":
                        IsTrue = true;
                        break;
                }
            return IsTrue;
        }

        /// <summary>
        /// Gets the unit.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        internal static Do.Unit GetUnit(string length)
        {
            Do.Unit unit;
            unit = Do.Unit.None;
            if (string.IsNullOrEmpty(length) || length == "0") return unit;

            if (length.EndsWith("%"))
            {
                return unit;
            }

            if (length.Length < 3)
            {
                return unit;
            }
            string u = length.Substring(length.Length - 2, 2);
            switch (u)
            {
                case Constants.Em:
                    unit = Do.Unit.Ems;
                    break;
                case Constants.Ex:
                    unit = Do.Unit.Ex;
                    break;
                case Constants.Px:
                    unit = Do.Unit.Pixels;
                    break;
                case Constants.Mm:
                    unit = Do.Unit.Milimeters;
                    break;
                case Constants.Cm:
                    unit = Do.Unit.Centimeters;
                    break;
                case Constants.In:
                    unit = Do.Unit.Inches;
                    break;
                case Constants.Pt:
                    unit = Do.Unit.Points;
                    break;
                case Constants.Pc:
                    unit = Do.Unit.Picas;
                    break;
                default:
                    unit = Do.Unit.None;
                    break; 
            }
            return unit;
        }

        /// <summary>
        /// Gets the HTML length number.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        internal static float GetHtmlLengthNumber(string length)
        {
            float lfnumber;
            lfnumber = 0f;
            if (length.EndsWith("%"))
            {
                lfnumber = Do.ParseLength(length, 1);
                return lfnumber;
            }

            if (length.Length < 3)
            {
                float.TryParse(length, out lfnumber);
                return lfnumber;
            }
            string number = length.Substring(0, length.Length - 2);
            float.TryParse(number, System.Globalization.NumberStyles.Number, NumberFormatInfo.InvariantInfo, out lfnumber);
            return lfnumber;
        }

        /// <summary>
        /// Determines whether the specified length has error.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// 	<c>true</c> if [is HTML length has error] [the specified length]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsHtmlLengthHasError(string length)
        {
            float lfnumber;
            bool hasError;
            lfnumber = 0f;
            hasError = false;
            if (length.EndsWith("%"))
            {
                lfnumber = Do.ParseLength(length, 1);
                return hasError;
            }

            if (length.Length < 3)
            {
                float.TryParse(length, out lfnumber);
                hasError = true;
                return hasError;
            }
            string number = length.Substring(0, length.Length - 2);
            if (!float.TryParse(number, System.Globalization.NumberStyles.Number, NumberFormatInfo.InvariantInfo, out lfnumber))
            {
                hasError = true;
            }
            return hasError;
        }

        /// <summary>
        /// If length is in Ems, returns its value in pixels
        /// </summary>
        internal static string ConvertEmToPixels(float pixelFactor,string length, bool point)
        {
            float lfnumber;
            string pixels;
            lfnumber = Do.GetHtmlLengthNumber(length);
            if (length.EndsWith("%")) return "";
            if (Do.GetUnit(length) != Do.Unit.Ems) return string.Format(NumberFormatInfo.InvariantInfo, "{0}%", lfnumber);
            if (point == true)
                pixels = Do.GetLengthString(string.Format("{0}pt", Convert.ToSingle(lfnumber * pixelFactor).ToString("0.0", NumberFormatInfo.InvariantInfo)));
            else
                pixels = Do.GetLengthString(string.Format("{0}px", Convert.ToSingle(lfnumber * pixelFactor).ToString("0.0", NumberFormatInfo.InvariantInfo)));
            return pixels;
             
        }

        /// <summary>
        /// Gets the length string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        internal static string GetLengthString(string length)
        {
                string unit = string.Empty;
                float lfnumber;
                lfnumber = Do.GetHtmlLengthNumber(length);
                if (length.EndsWith("%")) return "";
                switch (Do.GetUnit(length))
                {
                    case Do.Unit.None:
                        break;
                    case Do.Unit.Ems:
                        unit = "em";
                        break;
                    case Do.Unit.Pixels:
                        unit = "px";
                        break;
                    case Do.Unit.Ex:
                        unit = "ex";
                        break;
                    case Do.Unit.Inches:
                        unit = "in";
                        break;
                    case Do.Unit.Centimeters:
                        unit = "cm";
                        break;
                    case Do.Unit.Milimeters:
                        unit = "mm";
                        break;
                    case Do.Unit.Points:
                        unit = "pt";
                        break;
                    case Do.Unit.Picas:
                        unit = "pc";
                        break;
                }

                return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}", lfnumber, unit);
            }
        }
}
