#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Syncfusion.HTMLUI.Base.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// A sealed class Attribute Parser
    /// </summary>
    internal sealed class AttributeParser
    {
        #region Class constants
        /// <summary>
        /// Options for regular expressions.
        /// </summary>
        private const RegexOptions DEF_REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// Suffixes in values.
        /// </summary>
        private const string DEF_SUFFIXES = "em|ex|px|mm|cm|in|pt|pc|deg|rad|grad|ms|s|hz|khz|%";

        /// <summary>
        /// Pattern for integer.
        /// </summary>
        private const string DEF_INTEGER = @"^([0-9]+)(" + DEF_SUFFIXES + ")*$";

        /// <summary>
        /// Pattern for float.
        /// </summary>
        private const string DEF_FLOAT = @"^([0-9]+)([.]{1})([0-9]+)(" + DEF_SUFFIXES + ")*$";

        /// <summary>
        /// Pattern for percent.
        /// </summary>
        private const string DEF_PERCENT = @"^([0-9]+)([.]*)([0-9]*)([%]{1})$";

        /// <summary>
        /// Pattern for color in hash format.
        /// </summary>
        private const string DEF_COLOR_HEX = @"^#{1}[0-9a-fA-F]{3,6}$";

        /// <summary>
        /// Pattern for color in RGB format.
        /// </summary>
        private const string DEF_COLOR_RGB = @"^rgb\([0-9]{1,3},[ \t]*[0-9]{1,3},[ \t]*[0-9]{1,3}\)";

        /// <summary>
        /// Pattern for URL.
        /// </summary>
        private const string DEF_COLOR_URL = @"^url\({1}[a-z0-9.:/_ ]+\){1}$";

        /// <summary>
        /// Pattern for WhiteSpace.
        /// </summary>
        private const string DEF_WHITESPACE_PATTERN = @"[\s]+";

        /// <summary>
        /// Pattern for nonwhitespace data.
        /// </summary>
        private const string DEF_NONWHITESPACE = @"[\S]+";

        /// <summary>
        /// Pattern for new lines.
        /// </summary>
        private const string DEF_NEW_LINE = @"[\r\n]+";

        /// <summary>
        /// Pattern for delimiters.
        /// </summary>
        private const string DEF_DELIMITERS = @"(,){1}$";

        /// <summary>
        /// Pattern for delimiters.
        /// </summary>
        private const string DEF_URL = @"url\([\'" + "\"" + @"\s]*([^" + "\"" + @"\']+)[\'" + "\"" + @"\s]*\)"; // @"url\([\'" + "\"" + @"\s]*([\w\.\/]+)[\'" + "\"" + @"\s]*\)"

        /// <summary>
        /// Delimiter between font families.
        /// </summary>
        private const char DEF_FONT_DELIMITER = ',';

        /// <summary>
        /// Whitespace string.
        /// </summary>
        private const string DEF_WHITESPACE = " ";
        #endregion

        #region Class static constants
        /// <summary>
        /// For checking integer value.
        /// </summary>
        private static Regex m_regInt = new Regex(DEF_INTEGER, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For checking float value.
        /// </summary>
        private static Regex m_regFloat = new Regex(DEF_FLOAT, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For checking percentage value.
        /// </summary>
        private static Regex m_regPercent = new Regex(DEF_PERCENT, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For checking color in hash format.
        /// </summary>
        private static Regex m_regColorHex = new Regex(DEF_COLOR_HEX, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For checking color in RGB format.
        /// </summary>
        private static Regex m_regColorRGB = new Regex(DEF_COLOR_RGB, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For checking color URL.
        /// </summary>
        private static Regex m_regColorURL = new Regex(DEF_COLOR_URL, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For deleting multiple WhiteSpaces.
        /// </summary>
        private static Regex m_regDelWhiteSpaces = new Regex(DEF_WHITESPACE_PATTERN, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For deleting multiple NonWhiteSpaces.
        /// </summary>
        private static Regex m_regNonWhiteSpaces = new Regex(DEF_NONWHITESPACE, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For deleting possible commas after values.
        /// </summary>
        private static Regex m_regDelCommas = new Regex(DEF_DELIMITERS, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For deleting multiple NewLines.
        /// </summary>
        private static Regex m_regDelNewLines = new Regex(DEF_NEW_LINE, DEF_REGEX_OPTIONS);

        /// <summary>
        /// For checking URL.
        /// </summary>
        private static Regex m_regUrl = new Regex(DEF_URL, DEF_REGEX_OPTIONS);
        #endregion

        #region Class members
        /// <summary>
        /// Holds all names of colors.
        /// </summary>
        private static Hashtable m_hashColors;

        /// <summary>
        /// Holds all names of border styles.
        /// </summary>
        private static Hashtable m_hashBorderStyle;

        /// <summary>
        /// Holds all names of font families.
        /// </summary>
        private static Hashtable m_hashFontFamily;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the AttributeParser class
        /// </summary>
        static AttributeParser()
        {
            // Fill colors.
            m_hashColors = CollectionsUtil.CreateCaseInsensitiveHashtable();
            foreach (string ColorName in Enum.GetNames(typeof(KnownColor)))
            {
                m_hashColors.Add(ColorName, null);
            }

            // Fill border styles.
            m_hashBorderStyle = CollectionsUtil.CreateCaseInsensitiveHashtable();
            foreach (string borderstyle in Enum.GetNames(typeof(BordersStyle)))
            {
                m_hashBorderStyle.Add(borderstyle, Enum.Parse(typeof(BordersStyle), borderstyle));
            }

            // Fill font families.
            m_hashFontFamily = CollectionsUtil.CreateCaseInsensitiveHashtable();
            foreach (FontFamily ff in FontFamily.Families)
            {
                m_hashFontFamily.Add(ff.Name, ff);
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Overloaded. Converts the string parameter value to integer.
        /// </summary>
        /// <param name="val">String representation of the integer.</param>
        /// <returns>Integer number.</returns>
        public static int GetInteger(string val)
        {
            string output;
            return GetInteger(val, out output);
        }

        /// <summary>
        /// Converts the string parameter value to integer.
        /// </summary>
        /// <param name="val">String representation of the integer.</param>
        /// <param name="suffix">String which indicates the type of measurement.</param>
        /// <returns>Integer value from the string parameter value.</returns>
        public static int GetInteger(string val, out string suffix)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();
            if (IsFloat(val))
                val = val.Substring(0, val.IndexOf('.', 0));

            if (!IsInteger(val))
                throw new ArgumentException("Can't convert value '" + val + "' to int");

            Match m = m_regInt.Match(val);
            string IntValue = m.Groups[1].Value;
            suffix = m.Groups[2].Value;

            double result;

            if (Double.TryParse(IntValue, NumberStyles.Integer, null, out result))
            {
                return (int)Math.Abs(result);
            }

            return 0;
        }

        /// <summary>
        /// Overloaded. Converts the string parameter value to float.
        /// </summary>
        /// <param name="val">String representation of the float number.</param>
        /// <returns>Float value from the string parameter value.</returns>
        public static float GetFloat(string val)
        {
            string suffix;
            return GetFloat(val, out suffix);
        }

        /// <summary>
        /// Converts the string parameter value to float.
        /// </summary>
        /// <param name="val">String value of float.</param>
        /// <param name="suffix">Type of number measurement.</param>
        /// <returns>Float value from the string parameter value.</returns>
        public static float GetFloat(string val, out string suffix)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsFloat(val))
                throw new ArgumentException("Can't convert value '" + val + "' to float");

            string FloatValue;
            Match m = m_regFloat.Match(val);
            FloatValue = m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value;
            suffix = m.Groups[4].Value;

            double value;
            if (double.TryParse(FloatValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return (float)Math.Abs(value);
            }

            return 0f;
        }

        /// <summary>
        /// Returns boolean value from the string value.
        /// </summary>
        /// <param name="val">String representation of the bool.</param>
        /// <returns>Boolean value.</returns>
        public static bool GetBool(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsBool(val))
                throw new ArgumentException("Can't convert value '" + val + "' to BOOL");

            if (val.IndexOf("true") >= 0)
                return true;
            else if (val.IndexOf("false") >= 0)
                return false;

            throw new ArgumentException("Can't convert value '" + val + "' to BOOL");
        }

        /// <summary>
        /// Overloaded. Converts the string value to point.
        /// </summary>
        /// <param name="val">String representation of the point.</param>
        /// <returns>Point value.</returns>
        public static Point GetPoint(string val)
        {
            string output;
            return GetPoint(val, out output);
        }

        /// <summary>
        /// Converts the string value to point.
        /// </summary>
        /// <param name="val">String representation of point.</param>
        /// <param name="suffix">Type of number measurement.</param>
        /// <returns>Point value.</returns>
        public static Point GetPoint(string val, out string suffix)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsPoint(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Point");

            string[] PointArray = GetTokensValues(val, ' ');
            if (PointArray.Length != 2)
                throw new ArgumentException("Can't convert value '" + val + "' to Point");

            int x = GetInteger(PointArray[0]);
            int y = GetInteger(PointArray[1], out suffix);
            Point p = new Point(x, y);
            return p;
        }

        /// <summary>
        /// Overloaded. Converts the string value to triangle.
        /// </summary>
        /// <param name="val">String representation of the three numbers.</param>
        /// <returns>Rectangle with three values.</returns>
        public static Rectangle GetTriangle(string val)
        {
            string output;
            return GetTriangle(val, out output);
        }

        /// <summary>
        /// Converts the string value to triangle.
        /// </summary>
        /// <param name="val">String representation of three numbers.</param>
        /// <param name="suffix">Type of measurement.</param>
        /// <returns>Three numbers.</returns>
        public static Rectangle GetTriangle(string val, out string suffix)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (IsSimpleValue(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Triangle");

            if (!IsTriangle(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Triangle");

            string[] PointArray = GetTokensValues(val, ' ');

            int f = GetInteger(PointArray[0]);
            int s = GetInteger(PointArray[1]);
            int t = GetInteger(PointArray[2], out suffix);          
            return new Rectangle(f, s, t, 0);
        }

        /// <summary>
        /// Overloaded. Converts the string value to rectangle.
        /// </summary>
        /// <param name="val">String representation of the rectangle.</param>
        /// <returns>Rectangle object.</returns>
        public static Rectangle GetRectangle(string val)
        {
            string output;
            return GetRectangle(val, out output);
        }

        /// <summary>
        /// Converts the string value to rectangle.
        /// </summary>
        /// <param name="val">String representation of four numbers.</param>
        /// <param name="suffix">Type of measurement.</param>
        /// <returns>Four numbers from the string.</returns>
        public static Rectangle GetRectangle(string val, out string suffix)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (IsSimpleValue(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Rectangle");

            if (!IsRectangle(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Rectangle");

            string[] PointArray = GetTokensValues(val, ' ');

            int top = GetInteger(PointArray[0]);
            int right = GetInteger(PointArray[1]);
            int bottom = GetInteger(PointArray[2]);
            int left = GetInteger(PointArray[3], out suffix);

            Rectangle r = new Rectangle(left, top, right, bottom);
            return r;
        }

        /// <summary>
        /// Converts the string to color structure.
        /// </summary>
        /// <param name="val">String representation of the color.</param>
        /// <returns>Color object.</returns>
        public static Color GetColor(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsColor(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Color");

            Color c = new Color();
            /*
            int r = 0;
            int g = 0;
            int b = 0;
            */
            string numbers = string.Empty;
            AttributeToken token = DetectType(val);
            switch (token)
            {
                case AttributeToken.Color_Hex:
                    /*
                    numbers = val.Substring( 1 );
                    int NumberLength = ( int )numbers.Length /3;
                    r = System.Convert.ToInt32( "0x" + numbers.Substring( 0, NumberLength ) , 16 );
                    g = System.Convert.ToInt32( "0x" + numbers.Substring( NumberLength , NumberLength ) , 16 );
                    b = System.Convert.ToInt32( "0x" + numbers.Substring( NumberLength*2 , NumberLength ), 16 );
                    c = Color.FromArgb( r, g, b );
                    */
                    c = ColorTranslator.FromHtml(val);
                    break;

                case AttributeToken.Color_rgb:
                    numbers = val.Substring(4, val.Length - 5);
                  
                    string[] colors = GetTokensValues(numbers, ',');
                    int r = GetInteger(colors[0]);
                    int g = GetInteger(colors[1]);
                    int b = GetInteger(colors[2]);
                    c = Color.FromArgb(r, g, b);
                    
                    //// c = ColorTranslator.FromHtml( numbers );
                    break;

                case AttributeToken.Color_Word:
                    //// c = Color.FromName( val );
                    c = ColorTranslator.FromHtml(val);
                    break;
            }          
            return c;
        }

        /// <summary>
        /// Returns the border style for the format.
        /// </summary>
        /// <param name="val">String representation of the border style.</param>
        /// <returns>BorderStyle object.</returns>
        public static BordersStyle GetBorderStyle(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsBorderStyle(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Border style");

            return (BordersStyle)m_hashBorderStyle[val];
        }

        /// <summary>
        /// Returns the font family for the format.
        /// </summary>
        /// <param name="val">String representation of the font family.</param>
        /// <returns>String family name.</returns>
        public static string GetFontFamily(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsFontFamily(val))
                throw new ArgumentException("Can't convert value '" + val + "' to FontFamily");

            string[] names = GetTokensValues(val, DEF_FONT_DELIMITER);
            foreach (string name in names)
            {
                if (m_hashFontFamily.ContainsKey(name))
                {
                    //// return ( FontFamily ) m_hashFontFamily[ name ];
                    return name;
                }
            }

            return SystemInformation.MenuFont.FontFamily.Name;
        }

        /// <summary>
        /// Returns the font style for the format.
        /// </summary>
        /// <param name="val">String representation of the font style.</param>
        /// <returns>FontStyle object.</returns>
        public static FontStyle GetFontStyle(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsFontStyle(val))
                throw new ArgumentException("Can't convert value '" + val + "' to FontStyle");

            switch (val)
            {
                case "normal": return FontStyle.Regular;
                case "italic": return FontStyle.Italic;
                case "oblique": return FontStyle.Italic;
                default: return FontStyle.Regular;
            }
        }

        /// <summary>
        /// Returns the TextDecoration for the format.
        /// </summary>
        /// <param name="val">String representation of the text decoration.</param>
        /// <returns>FontStyle object.</returns>
        public static FontStyle GetTextDecoration(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsTextDecoration(val))
                throw new ArgumentException("Can't convert value '" + val + "' to TextDecoration");

            switch (val)
            {
                case "none":
                case "blink":
                case "overline": goto default;
                case "underline": return FontStyle.Underline;
                case "line-through": return FontStyle.Strikeout;
                default: return FontStyle.Regular;
            }
        }

        /// <summary>
        /// Returns the font weight for the format.
        /// </summary>
        /// <param name="val">String representation of the font weight.</param>
        /// <returns>Font style object.</returns>
        public static FontStyle GetFontWeight(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            if (!IsFontWeight(val))
                throw new ArgumentException("Can't convert value '" + val + "' to FontWeight");

            switch (val)
            {
                case "bold":
                case "bolder": return FontStyle.Bold;
                case "lighter":
                case "normal":
                case "100":
                case "200":
                case "300":
                case "400":
                case "500": goto default;
                case "600":
                case "700":
                case "800":
                case "900": return FontStyle.Bold;
                default: return FontStyle.Regular;
            }
        }

        /// <summary>
        /// Returns the size of the font for the format.
        /// </summary>
        /// <param name="val">String representation of the font.</param>
        /// <param name="font">Font object.</param>
        /// <param name="outUnit">Type of measurement.</param>
        /// <returns>Size of the font.</returns>
        public static float GetFontFromSize(string val, Font font, out GraphicsUnit outUnit)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            if (font == null)
                throw new ArgumentNullException("font");

            val = val.Trim().ToLower();

            if (!IsFontSize(val))
                throw new ArgumentException("Can't convert value '" + val + "' to Fontsize");

            GraphicsUnit unit = GraphicsUnit.Point;
            float size = font.Size;

            // if in string format
            switch (val)
            {
                case "small": 
                    size *= 1f; 
                    break;
                case "smaller":
                    size = font.SizeInPoints - size * 0.2f; 
                    break;
                case "medium":
                    size *= 1.1f;
                    break;
                case "large": 
                    size *= 1.5f;
                    break;
                case "larger": 
                    size = font.SizeInPoints + size * 0.1f;
                    break;
            }

            // if in number format
            string suffix = string.Empty;
            float newValue = size;
            if (IsInteger(val))
            {
                newValue = GetInteger(val, out suffix);
            }
            else if (IsFloat(val) || IsPercent(val))
            {
                newValue = GetFloat(val, out suffix);
            }
            if (Utilities.StrEquals(suffix, "ex"))
            {
                newValue = newValue * size / 2;
            }
            else if (Utilities.StrEquals(suffix, "%"))
            {
                newValue = size * (newValue / 100);
                unit = GraphicsUnit.Point;
            }
            else if (Utilities.StrEquals(suffix, "em"))
            {
                unit = GraphicsUnit.Point;
                newValue = newValue * size;
            }
            else if (Utilities.StrEquals(suffix, "pt"))
            {
                unit = GraphicsUnit.Pixel;
                newValue = 100 * newValue / 72;
            }
            else if (Utilities.StrEquals(suffix, "px") || suffix.Length == 0)
            {
                unit = GraphicsUnit.Pixel;
            }

            outUnit = unit;
            return newValue;
        }

        /// <summary>
        /// Returns the string path from url("path") pattern.
        /// </summary>
        /// <param name="val">String representation of the URL.</param>
        /// <returns>Returns string path from url("path") pattern.</returns>
        public static string GetUri(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            val = val.Trim().ToLower();

            //// if( !IsUrl( val ) && !ISstr )
            ////  throw new ArgumentException( "Can't extract path from '" + val + "'" );

            Match m = null;

            if ((m = m_regUrl.Match(val)).Success)
            {
                if (m.Groups.Count == 2)
                {
                    return m.Groups[1].Value;
                }
            }

            return val;
            //// throw new ArgumentException( "Can't extract path from '" + val + "'" );
        }

        /// <summary>
        /// Returns the marker style of the list item by it's type.
        /// </summary>
        /// <param name="val">Type of the marker.</param>
        /// <returns>Marker style of the list item by it's type.</returns>
        public static ListItemType GetListItemType(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            ListItemType type = ListItemType._1;
            val = val.Trim();

            switch (val)
            {
                case "a": type = ListItemType.a;
                    break;
                case "A": type = ListItemType.A;
                    break;
                case "i": type = ListItemType.i;
                    break;
                case "I": type = ListItemType.I;
                    break;
                case "circle": type = ListItemType.circle;
                    break;
                case "square": type = ListItemType.square;
                    break;
                case "disc": type = ListItemType.disc;
                    break;
            }

            return type;
        }

        /// <summary>
        /// Recognizes the type of the specified value.
        /// </summary>
        /// <param name="val">String value.</param>
        /// <returns>Type of value.</returns>
        public static AttributeToken DetectType(string val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            val = val.Trim().ToLower();
            AttributeToken tokenType = AttributeToken.Empty;

            if (!IsSimpleValue(val))
            {
                if (IsPoint(val))
                {
                    tokenType = AttributeToken.Point;
                }
                else if (IsTriangle(val))
                {
                    tokenType = AttributeToken.Triangle;
                }
                else if (IsRectangle(val))
                {
                    tokenType = AttributeToken.Rectangle;
                }
                else if (IsFontFamily(val))
                {
                    tokenType = AttributeToken.FontFamily;
                }
                else
                {
                    tokenType = AttributeToken.Complex;
                }

                return tokenType;
            }

            if (IsPercent(val))
            {
                tokenType = AttributeToken.Percent;
            }
            else if (IsInteger(val))
            {
                tokenType = AttributeToken.Integer;
            }
            else if (IsFloat(val))
            {
                tokenType = AttributeToken.Float;
            }
            else if (IsColorHex(val))
            {
                tokenType = AttributeToken.Color_Hex;
            }
            else if (IsColorRGB(val))
            {
                tokenType = AttributeToken.Color_rgb;
            }
            else if (IsColorWord(val))
            {
                tokenType = AttributeToken.Color_Word;
            }
            else if (IsBorderStyle(val))
            {
                tokenType = AttributeToken.BorderStyle;
            }
            else if (IsBool(val))
            {
                tokenType = AttributeToken.Bool;
            }
            else if (IsUrl(val))
            {
                tokenType = AttributeToken.Uri;
            }
            else if (IsFontFamily(val))
            {
                tokenType = AttributeToken.FontFamily;
            }
            else if (IsFontStyle(val))
            {
                tokenType = AttributeToken.FontStyle;
            }
            else if (IsFontWeight(val))
            {
                tokenType = AttributeToken.FontWeight;
            }
            else if (IsFontSize(val))
            {
                tokenType = AttributeToken.FontSize;
            }
            else if (IsTextDecoration(val))
            {
                tokenType = AttributeToken.TextDecoration;
            }
            else
            {
                tokenType = AttributeToken.String;
            }

            return tokenType;
        }

        /// <summary>
        /// Trims big whitespaces to single, skip tabs, new lines, etc.
        /// </summary>
        /// <param name="str">String value.</param>
        /// <returns>String after deleting whitespaces.</returns>
        public static string DeleteWhiteSpace(string str)
        {
            if (str == null || str.Length == 0) return str;

            str = m_regDelWhiteSpaces.Replace(str, DEF_WHITESPACE);

            return str;
        }

        /// <summary>
        /// Converts the string value to the specified object.
        /// </summary>
        /// <param name="val">String value of the object.</param>
        /// <param name="type">Type of objects for converting.</param>
        /// <returns>Converted object or NULL.</returns>
        public static object GetSimpleValue(string val, AttributeToken type)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            object result = null;

            if (IsSimple(type))
            {
                switch (type)
                {
                    case AttributeToken.Bool:
                        result = GetBool(val); 
                        break;
                    case AttributeToken.Color_Hex:
                    case AttributeToken.Color_rgb:
                    case AttributeToken.Color_Word: 
                    result = GetColor(val); 
                    break;
                    case AttributeToken.Float: 
                        result = GetFloat(val);
                        break;
                    case AttributeToken.Integer:
                        result = GetInteger(val); 
                        break;
                   default: 
                        result = val;
                        break;
                }
            }
            else if (type == AttributeToken.Complex)
            {
                result = val;
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the value has whitespace data only.
        /// </summary>
        /// <param name="val">String value.</param>
        /// <returns>True if value has whitespace data only; False otherwise.</returns>
        public static bool IsWhitespace(string val)
        {
            bool result = false;

            if (val != null && val.Length > 0)
            {
                result = !m_regNonWhiteSpaces.Match(val).Success;
            }

            return result;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Indicates whether the value is of simple construction.
        /// </summary>
        /// <param name="val">String value.</param>
        /// <returns>True if value has simple syntax.</returns>
        private static bool IsSimpleValue(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            val = val.Trim();
            if ((val.IndexOf(DEF_WHITESPACE) > 0) || (val.IndexOf("\t") > 0)) return false;

            return true;
        }

        /// <summary>
        /// Returns the array of simple values from the complex value.
        /// </summary>
        /// <param name="val">String value.</param>
        /// <param name="delimiter">Delimiter symbol.</param>
        /// <returns>Array of tokens.</returns>
        internal static string[] GetTokensValues(string val, char delimiter)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (val.Length == 0)
                throw new ArgumentException("val - string cannot be empty");

            RemoveBigSpaces(ref val);

            string[] output = val.Split(delimiter);

            // delete symbol: ',' after values
            for (int i = 0; i < output.Length; i++)
            {
                string token = output[i].Trim();
                output[i] = m_regDelCommas.Replace(token, string.Empty);
            }

            return output;
        }

        /// <summary>
        /// Removes all whitespaces except simple space.
        /// </summary>
        /// <param name="val">String value.</param>
        private static void RemoveBigSpaces(ref string val)
        {
            if (val != null && val.Length > 0)
            {
                val = m_regDelWhiteSpaces.Replace(val, DEF_WHITESPACE);
            }
        }

        /// <summary>
        /// Indicates whether "val" is an integer value.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to integer.</returns>
        private static bool IsInteger(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            return m_regInt.Match(val).Success;
        }

        /// <summary>
        /// Indicates whether "val" is a float value.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to float.</returns>
        private static bool IsFloat(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            return m_regFloat.Match(val).Success;
        }

        /// <summary>
        /// Indicates whether "val" is a percentage value.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to percentage.</returns>
        private static bool IsPercent(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            return m_regPercent.Match(val).Success;
        }

        /// <summary>
        /// Indicates whether "val" is a color in hash form.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to color.</returns>
        private static bool IsColorHex(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            if (m_regColorHex.Match(val).Success)
            {
                int l = val.Substring(1).Length;
                if (l == 3 || l == 6) 
                {
                    // Color
                    return true;
                }
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Indicates whether "val" is a color in RGB form.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to color.</returns>
        private static bool IsColorRGB(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            RemoveBigSpaces(ref val);
            return m_regColorRGB.Match(val).Success;
        }

        /// <summary>
        /// Indicates whether "val" is a color in word form.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to color.</returns>
        private static bool IsColorWord(string val)
        {
            if (val == null || val.Length == 0) return false;

            return m_hashColors.ContainsKey(val);
        }

        /// <summary>
        /// Indicates whether "val" is a color.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to color.</returns>
        private static bool IsColor(string val)
        {
            return IsColorHex(val) || IsColorRGB(val) || IsColorWord(val);
        }

        /// <summary>
        /// Indicates whether "val" is a border style.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to border style.</returns>
        private static bool IsBorderStyle(string val)
        {
            if (val == null || val.Length == 0) return false;

            return m_hashBorderStyle.ContainsKey(val);
        }

        /// <summary>
        /// Indicates whether "val" is a URL.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to URI.</returns>
        private static bool IsUrl(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            return m_regUrl.Match(val).Success;
        }

        /// <summary>
        /// Indicates whether "val" is a Bool.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to bool.</returns>
        private static bool IsBool(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            if (Utilities.StrEquals(val, "true") ||
                 Utilities.StrEquals(val, "false"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether "val" is a point.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to point.</returns>
        private static bool IsPoint(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            string[] content = GetTokensValues(val, ' ');

            if (content.Length == 2 &&
              DetectType(content[0]) == AttributeToken.Integer &&
              DetectType(content[1]) == AttributeToken.Integer
              )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether "val" is a triangle.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to three numbers.</returns>
        private static bool IsTriangle(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            string[] content = GetTokensValues(val, ' ');
            if (content.Length == 3 &&
              DetectType(content[0]) == AttributeToken.Integer &&
              DetectType(content[1]) == AttributeToken.Integer &&
              DetectType(content[2]) == AttributeToken.Integer
              )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether "val" is a rectangle.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to four numbers.</returns>
        private static bool IsRectangle(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            string[] content = GetTokensValues(val, ' ');
            if (content.Length == 4 &&
              DetectType(content[0]) == AttributeToken.Integer &&
              DetectType(content[1]) == AttributeToken.Integer &&
              DetectType(content[2]) == AttributeToken.Integer &&
              DetectType(content[3]) == AttributeToken.Integer
              )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether "val" is a font family name.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to font family.</returns>
        private static bool IsFontFamily(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            string[] names = GetTokensValues(val, DEF_FONT_DELIMITER);
            bool found = false;

            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                if (m_hashFontFamily.ContainsKey(name))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Indicates whether "val" is a font style name.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to font style.</returns>
        private static bool IsFontStyle(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            switch (val)
            {
                case "normal":
                case "italic":
                case "oblique": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Indicates whether "val" is a font weight name.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to font weight.</returns>
        private static bool IsFontWeight(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            switch (val)
            {
                case "normal":
                case "bold":
                case "bolder":
                case "lighter":
                case "100":
                case "200":
                case "300":
                case "400":
                case "500":
                case "600":
                case "700":
                case "800":
                case "900": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Indicates whether "val" is a font size value.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to size.</returns>
        private static bool IsFontSize(string val)
        {
            if (val == null)
                return false;
            if (val.Length == 0)
                return false;
            
            switch (val)
            {
                case "small":
                case "smaller":
                case "medium":
                case "large":
                case "larger": 
                return true;
            }
            if (!IsInteger(val) && !IsFloat(val) && !IsPercent(val))
                return false;

            string suffix = string.Empty;
            if (IsPercent(val))
            {
                return true;
            }
            else if (IsInteger(val))
            {
                GetInteger(val, out suffix);
            }
            else if (IsFloat(val))
            {
                GetFloat(val, out suffix);
            }
            if (Utilities.StrEquals(suffix, "em") || Utilities.StrEquals(suffix, "px") || Utilities.StrEquals(suffix, "ex") || Utilities.StrEquals(suffix, "pt") || suffix.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether "val" is a TextDecoration value.
        /// </summary>
        /// <param name="val">String token.</param>
        /// <returns>True if string can be converted to text decoration.</returns>
        private static bool IsTextDecoration(string val)
        {
            if (val == null) return false;
            if (val.Length == 0) return false;

            switch (val)
            {
                case "none":
                case "underline":
                case "overline":
                case "line-through":
                case "blink": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Indicates whether the type is simple.
        /// This method is used for the custom control's property settings.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <returns>True if type is simple; false otherwise.</returns>
        private static bool IsSimple(AttributeToken type)
        {
            bool bSimple = type == AttributeToken.Bool || type == AttributeToken.Color_Hex || type == AttributeToken.Color_rgb || type == AttributeToken.Color_Word || type == AttributeToken.Float || type == AttributeToken.Integer || type == AttributeToken.String;

            return bSimple;
        }
        #endregion
    }
}