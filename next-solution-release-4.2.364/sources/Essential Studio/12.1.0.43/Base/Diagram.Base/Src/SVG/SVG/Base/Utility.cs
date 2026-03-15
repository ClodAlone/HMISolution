#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for Utility.
    /// </summary>
    public class Utility
    {
        #region Members
        private static Regex m_numberRegex;
        private static Regex m_urlRegex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the float format.
        /// </summary>
        /// <value>The float format.</value>
        public static NumberFormatInfo FloatFormat
        {
            get
            {
                return CultureInfo.InvariantCulture.NumberFormat;
            }
        }

        /// <summary>
        /// Gets the number chars.
        /// </summary>
        /// <value>The number chars.</value>
        public static char[] NumberChars
        {
            get
            {
                return new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            }
        }

        /// <summary>
        /// Gets the number regex.
        /// </summary>
        /// <value>The number regex.</value>
        public static Regex NumberRegex
        {
            get
            {
                if (m_numberRegex == null)
                {
                    m_numberRegex = new Regex(
                                       @"[-]?[0-9]+([.][0-?9]+)?(\s*e[+-][0-9]+)?",
                                       RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                return m_numberRegex;
            }
        }

        /// <summary>
        /// Gets the URL regex.
        /// </summary>
        /// <value>The URL regex.</value>
        public static Regex UrlRegex
        {
            get
            {
                if (m_urlRegex == null)
                {
                    m_urlRegex = new Regex(@"url[(][#]*(?<text>\w+)[)]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                return m_urlRegex;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The float value.</returns>
        public static float GetFloat(string value)
        {
            return float.Parse(value, FloatFormat);
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The float.</returns>
        public static string GetFloat(float value)
        {
            return value.ToString(FloatFormat);
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The float value.</returns>
        public static float GetNumber(string value)
        {
            Match mch = NumberRegex.Match(value);

            return mch.Success ? GetFloat(mch.Value) : float.NaN;
        }

        /// <summary>
        /// Gets the numbers.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The float.</returns>
        public static float[] GetNumbers(string value)
        {
            MatchCollection mchs = NumberRegex.Matches(value);

            float[] res = new float[mchs.Count];

            for (int i = 0; i < mchs.Count; i++)
            {
                res[i] = GetFloat(mchs[i].Value);
            }

            return res;
        }

        /// <summary>
        /// Gets the GDI pen.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>The pen</returns>
        public static Pen GetGDIPen(IStrokeAttributes elem)
        {
            Pen res = null;

            if (!elem.Stroke.IsNone)
            {
                res = new Pen(elem.Stroke.Color, elem.StrokeWidth.Value);
                res.Color = Color.FromArgb(
                  new Opacity(elem.StrokeOpacity.Value * (elem as IOpacityAttribute).Opacity.Value).Alpha, res.Color);
                res.MiterLimit = elem.StrokeMiterlimit.Value;

                if (elem.StrokeLinejoin == EStrokeLinejoin.Bevel)
                {
                    res.LineJoin = LineJoin.Bevel;
                }
                else if (elem.StrokeLinejoin == EStrokeLinejoin.Miter)
                {
                    res.LineJoin = LineJoin.Miter;
                }
                else if (elem.StrokeLinejoin == EStrokeLinejoin.Round)
                {
                    res.LineJoin = LineJoin.Round;
                }

                if (elem.StrokeLinecap == EStrokeLinecap.Butt)
                {
                    res.EndCap = LineCap.Flat;
                }
                else if (elem.StrokeLinecap == EStrokeLinecap.Round)
                {
                    res.EndCap = LineCap.Round;
                }
                else if (elem.StrokeLinecap == EStrokeLinecap.Square)
                {
                    res.EndCap = LineCap.Square;
                }

                res.DashOffset = elem.StrokeDashoffset.Value;

                if (elem.StrokeDasharray != null)
                {
                    float[] ar = new float[elem.StrokeDasharray.Array.Length];

                    for (int i = 0, c = elem.StrokeDasharray.Array.Length; i < c; i++)
                    {
                        ar[i] = elem.StrokeDasharray.Array[i] / elem.StrokeWidth.Value;
                    }

                    res.DashPattern = ar;
                }
            }

            return res;
        }

        /// <summary>
        /// Sets the GDI pen.
        /// </summary>
        /// <param name="elem">The stroke attributes.</param>
        /// <param name="pen">The pen.</param>
        public static void SetGDIPen(IStrokeAttributes elem, Pen pen)
        {
            if (pen != null)
            {
                elem.Stroke = new NoneColor(pen.Color);
                elem.StrokeWidth = new Length(pen.Width);
                elem.StrokeOpacity = pen.Color.A;
                elem.StrokeMiterlimit = pen.MiterLimit;

                switch (pen.LineJoin)
                {
                    case LineJoin.Bevel:
                        elem.StrokeLinejoin = EStrokeLinejoin.Bevel;
                        break;
                    case LineJoin.Miter:
                        elem.StrokeLinejoin = EStrokeLinejoin.Miter;
                        break;
                    case LineJoin.Round:
                        elem.StrokeLinejoin = EStrokeLinejoin.Round;
                        break;
                }

                switch (pen.EndCap)
                {
                    case LineCap.Square:
                        elem.StrokeLinecap = EStrokeLinecap.Square;
                        break;
                    case LineCap.Round:
                        elem.StrokeLinecap = EStrokeLinecap.Round;
                        break;
                    case LineCap.Flat:
                        elem.StrokeLinecap = EStrokeLinecap.Butt;
                        break;
                }

                elem.StrokeDashoffset = pen.DashOffset;
                float w = pen.Width;

                switch (pen.DashStyle)
                {
                    case DashStyle.Dash:
                        elem.StrokeDasharray = new float[] { 3 * w, w };
                        break;
                    case DashStyle.DashDot:
                        elem.StrokeDasharray = new float[] { 3 * w, w, w, w };
                        break;
                    case DashStyle.DashDotDot:
                        elem.StrokeDasharray = new float[] { 3 * w, w, w, w, w };
                        break;
                    case DashStyle.Dot:
                        elem.StrokeDasharray = new float[] { w, w };
                        break;
                    case DashStyle.Custom:
                        elem.StrokeDasharray = pen.DashPattern;
                        break;
                }
            }
            else
            {
                elem.Stroke = new NoneColor(Color.Empty);
            }
        }

        /// <summary>
        /// Gets the GDI brush.
        /// </summary>
        /// <param name="elem">The fill attributes.</param>
        /// <param name="doc">The doc.</param>
        /// <returns>The brush</returns>
        public static Brush GetGDIBrush(IFillAttributes elem, SvgDocument doc)
        {
            Brush res = null;

            if (elem.Fill.IsNone)
            {
            }
            else if (elem.Fill.Color.IsEmpty)
            {
                string id = GetIdFromUrl(elem.Fill.Primaty);

                if (id != string.Empty)
                {
                    res = GetSpecialBrush(doc.FindElement(id));
                }
            }
            else
            {
                res = new SolidBrush(Color.FromArgb(
                  new Opacity(elem.FillOpacity.Value * (elem as IOpacityAttribute).Opacity.Value).Alpha,
                  elem.Fill.Color));
            }

            return res;
        }

        /// <summary>
        /// Sets the GDI brush.
        /// </summary>
        /// <param name="elem">The fill attribute.</param>
        /// <param name="br">The brush.</param>
        public static void SetGDIBrush(IFillAttributes elem, Brush br)
        {
            if (br != null)
            {
                if (br is SolidBrush)
                {
                    elem.Fill = new NoneColor((br as SolidBrush).Color);
                    elem.FillOpacity = new Opacity((br as SolidBrush).Color);
                }
                else if (br is LinearGradientBrush)
                {
                    LinearGradientElement lge = LinearGradientElement.FromBrush(br as LinearGradientBrush);
                    (elem as Element).OwnerDocument.Defs.AddChild(lge);
                    elem.Fill = new NoneColor(GetUrlFromId(lge.Id));
                }
                else if (br is PathGradientBrush)
                {
                    RadialGradientElement rge = RadialGradientElement.FromBrush(br as PathGradientBrush);
                    (elem as Element).OwnerDocument.Defs.AddChild(rge);
                    elem.Fill = new NoneColor(GetUrlFromId(rge.Id));
                }
                else if (br is TextureBrush)
                {
                    PatternElement pe = PatternElement.FormTextureBrush(br as TextureBrush);
                    (elem as Element).OwnerDocument.Defs.AddChild(pe);
                    elem.Fill = new NoneColor(GetUrlFromId(pe.Id));
                }
                else if (br is HatchBrush)
                {
                    PatternElement pe = PatternElement.FormHatchBrush(br as HatchBrush);
                    (elem as Element).OwnerDocument.Defs.AddChild(pe);
                    elem.Fill = new NoneColor(GetUrlFromId(pe.Id));
                }
            }
            else
            {
                elem.Fill = new NoneColor(Color.Empty);
            }
        }

        /// <summary>
        /// Gets the GDI font.
        /// </summary>
        /// <param name="elem">The font attribute.</param>
        /// <returns>The font</returns>
        public static Font GetGDIFont(IFontAttributes elem)
        {
            FontStyle style = FontStyle.Regular;

            if (elem.FontStyle != EFontStyle.Normal)
            {
                style |= FontStyle.Italic;
            }

            if ((elem.FontWeight == EFontWeight.Bold) ||
              (elem.FontWeight == EFontWeight.Bolder) ||
              (elem.FontWeight == EFontWeight.Value100) ||
              (elem.FontWeight == EFontWeight.Value200) ||
              (elem.FontWeight == EFontWeight.Value300) ||
              (elem.FontWeight == EFontWeight.Value400) ||
              (elem.FontWeight == EFontWeight.Value500))
            {
                style |= FontStyle.Bold;
            }

            return new Font(elem.FontFamily, elem.FontSize.Value, style);
        }

        /// <summary>
        /// Sets the GDI font.
        /// </summary>
        /// <param name="elem">The font attribute.</param>
        /// <param name="font">The font.</param>
        public static void SetGDIFont(IFontAttributes elem, Font font)
        {
            elem.FontFamily = font.FontFamily.Name;
            elem.FontSize = new Length(font.Size, LengthType.PT);

            if (font.Italic)
            {
                elem.FontStyle = EFontStyle.Italic;
            }

            if (font.Bold)
            {
                elem.FontWeight = EFontWeight.Bold;
            }
        }

        /// <summary>
        /// Gets the id from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The ID</returns>
        public static string GetIdFromUrl(string url)
        {
            Match mch = UrlRegex.Match(url);

            return mch.Success ? mch.Groups["text"].Value : string.Empty;
        }

        /// <summary>
        /// Gets the URL from id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The url</returns>
        public static string GetUrlFromId(string id)
        {
            return "url(#" + id + ")";
        }

        /// <summary>
        /// Gets the special brush.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>The brush</returns>
        public static Brush GetSpecialBrush(Element elem)
        {
            Brush res = null;

            switch (elem.Name)
            {
                case SVG.NAME_LINEAR_GRADIENT:
                    res = ((LinearGradientElement)elem).GetGDIBrush();
                    break;
                case SVG.NAME_PATTERN:
                    res = ((PatternElement)elem).GetGDIBrush();
                    break;
            }

            return res;
        }

        /// <summary>
        /// Gets the GDI brush.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>The brush</returns>
        public static Brush GetGDIBrush(SuperElement elem)
        {
            Brush res = null;

            if (elem == null)
            {
            }
            else if (elem.Attributes[SVG.ATTR_FILL] == null)
            {
                if (elem.Attributes[SVG.ATTR_STYLE] == null)
                {
                    res = GetGDIBrush(elem.Parent as SuperElement);
                }
                else
                {
                    res = GetGDIBrush((elem as IStyleAttribute).Style, elem.OwnerDocument);
                }
            }
            else
            {
                res = GetGDIBrush(elem as IFillAttributes, elem.OwnerDocument);
            }

            return res;
        }

        /// <summary>
        /// Gets the GDI pen.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>The pen</returns>
        public static Pen GetGDIPen(SuperElement elem)
        {
            Pen res = null;

            if (elem == null)
            {
            }
            else if (elem.Attributes[SVG.ATTR_FILL] == null)
            {
                if (elem.Attributes[SVG.ATTR_STYLE] == null)
                {
                    res = GetGDIPen(elem.Parent as SuperElement);
                }
                else
                {
                    res = GetGDIPen((elem as IStyleAttribute).Style);
                }
            }
            else
            {
                res = GetGDIPen(elem as IStrokeAttributes);
            }

            return res;
        }
        #endregion
    }
}
