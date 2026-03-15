#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Globalization;
using System.Text;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents one of the 14 standard PDF fonts.
    /// </summary>
    public class PdfStandardFont : PdfFont
    {
        #region Constants
        /// <summary>
        /// First character position.
        /// </summary>
        private const int c_charOffset = 32;
        #endregion

        #region Fields
        /// <summary>
        /// FontFamily of the font.
        /// </summary>
        private PdfFontFamily m_fontFamily;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStandardFont"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="size">The size.</param>
        public PdfStandardFont(PdfFontFamily fontFamily, float size)
            : this(fontFamily, size, PdfFontStyle.Regular)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStandardFont"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public PdfStandardFont(PdfFontFamily fontFamily, float size, PdfFontStyle style)
            : base(size, style)
        {
            m_fontFamily = fontFamily;
            CheckStyle();

            InitializeInternals();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStandardFont"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size.</param>
        public PdfStandardFont(PdfStandardFont prototype, float size)
            : this(prototype.FontFamily, size, prototype.Style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStandardFont"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public PdfStandardFont(PdfStandardFont prototype, float size, PdfFontStyle style)
            : this(prototype.FontFamily, size, style)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the FontFamily.
        /// </summary>
        public PdfFontFamily FontFamily
        {
            get
            {
                return m_fontFamily;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns the width of the char.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the symbol.</returns>
        #if !NETFX_CORE && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal override float GetCharWidth(char charCode, PdfStringFormat format)
        {
            float width = GetCharWidthInternal(charCode, format);

            float size = Metrics.GetSize(format);
            width *= (CharSizeMultiplier * size);

            return width;
        }

        /// <summary>
        /// Returns width of the line.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the line.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal override float GetLineWidth(string line, PdfStringFormat format)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            float width = 0f;
            line = PdfStandardFont.Convert(line);
            for (int i = 0, len = line.Length; i < len; i++)
            {
                char ch = line[i];
                float charWidth = GetCharWidthInternal(ch, format);

                width += charWidth;
            }

            float size = Metrics.GetSize(format);
            width *= (CharSizeMultiplier * size);
            width = ApplyFormatSettings(line, format, width);

            return width;
        }

        /// <summary>
        /// Checks whether fonts are equals.
        /// </summary>
        /// <param name="font">Font to compare.</param>
        /// <returns>True if fonts are equal, False otherwise.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override bool EqualsToFont(PdfFont font)
        {
            bool equal = false;
            PdfStandardFont stFont = font as PdfStandardFont;

            if (stFont != null)
            {
                bool fontFamilyEqual = (FontFamily == stFont.FontFamily);
                bool styleEqual = (Style & (~(PdfFontStyle.Underline | PdfFontStyle.Strikeout))) ==
                    (stFont.Style & (~(PdfFontStyle.Underline | PdfFontStyle.Strikeout)));

                equal = (fontFamilyEqual && styleEqual);
            }

            return equal;
        }

        /// <summary>
        /// Initializes font internals.
        /// </summary>
        private void InitializeInternals()
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
            {
                throw new PdfConformanceException("All the fonts must be embedded in PDF/A1-B document.");
            }
            else if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_X1A2001)
            {
                throw new PdfConformanceException("All the fonts must be embedded in PDF/X1A document.");
            }
#endif

            //Lock this code to avoid corrupting a static cache by another threads.
            lock (s_syncObject)
            {
                IPdfCache equalFont = null;
                if (PdfDocument.EnableCache)
                {
                   equalFont = PdfDocument.Cache.Search(this);
                }
                IPdfPrimitive internals = null;

                if (equalFont == null)
                {
                    // Create font metrics.
                    PdfFontMetrics metrics = PdfStandardFontMetricsFactory.GetMetrics(m_fontFamily, Style, Size);

                    Metrics = metrics;
                    internals = CreateInternals();
                }
                else
                {
                    internals = equalFont.GetInternals();

                    PdfFontMetrics metrics = ((PdfFont)equalFont).Metrics;
                    metrics = (PdfFontMetrics)metrics.Clone();
                    metrics.Size = Size;
                    Metrics = metrics;
                }

                ((IPdfCache)this).SetInternals(internals);
            }
        }

        /// <summary>
        /// Creates font's dictionary.
        /// </summary>
        /// <returns>font's dictionary.</returns>
        private PdfDictionary CreateInternals()
        {
            PdfDictionary dictionary = new PdfDictionary();

            dictionary[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Font);
            dictionary[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.Type1);
            dictionary[DictionaryProperties.BaseFont] = new PdfName(Metrics.PostScriptName);

            if (FontFamily != PdfFontFamily.Symbol && FontFamily != PdfFontFamily.ZapfDingbats)
            {
                string encoding = FontEncoding.WinAnsiEncoding.ToString();

                dictionary[DictionaryProperties.Encoding] = new PdfName(encoding);
            }

            return dictionary;
        }

        /// <summary>
        /// Checks font style of the font.
        /// </summary>
        private void CheckStyle()
        {
            if (FontFamily == PdfFontFamily.Symbol || FontFamily == PdfFontFamily.ZapfDingbats)
            {
                PdfFontStyle style = Style;

                style &= ~(PdfFontStyle.Bold | PdfFontStyle.Italic);
                SetStyle(style);
            }
        }

        /// <summary>
        /// Returns width of the char. This methods doesn't takes into consideration font's size.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the symbol.</returns>
        private float GetCharWidthInternal(char charCode, PdfStringFormat format)
        {
            float width = 0f;
            int code = charCode - c_charOffset;
            code = (code >= 0 && code != 128) ? code : 0;

            PdfFontMetrics metrics = Metrics;
            WidthTable widthTable = metrics.WidthTable;

            width = (float)widthTable[code];

            return width;
        }

        /// <summary>
        /// Converts the specified text.
        /// </summary>
        /// <param name="text">The unicode text.</param>
        /// <returns>The ANSI string.</returns>
        internal static string Convert(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }


#if !SILVERLIGHT && !NETFX_CORE && !WP
            byte[] orig = Encoding.Unicode.GetBytes(text);
            byte[] res = Encoding.Convert(Encoding.Unicode, Encoding.Default, orig);
#else
            Windows1252Encoding encode = new Windows1252Encoding();
            byte[] res = encode.GetBytes(text);            
#endif

            int count = res.Length;
            char[] result = new char[count];

            for (int i = 0; i < count; ++i)
            {
                result[i] = (char)res[i];
            }

            return new string(result);
        }
        #endregion
    }
}
