#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the standard CJK fonts.
    /// </summary>
    public class PdfCjkStandardFont : PdfFont
    {
        #region Constants
        /// <summary>
        /// First character position.
        /// </summary>
        private const int c_charOffset = 32;
        #endregion

        #region Fields
        /// <summary>
        /// Font family
        /// </summary>
        private PdfCjkFontFamily m_fontFamily;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCjkStandardFont"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public PdfCjkStandardFont(PdfCjkFontFamily fontFamily, float size, PdfFontStyle style)
            : base(size, style)
        {
            m_fontFamily = fontFamily;
            CheckStyle();

            InitializeInternals();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCjkStandardFont"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="size">The size.</param>
        public PdfCjkStandardFont(PdfCjkFontFamily fontFamily, float size)
            : this(fontFamily, size, PdfFontStyle.Regular)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCjkStandardFont"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size.</param>
        public PdfCjkStandardFont(PdfCjkStandardFont prototype, float size)
            : this(prototype.FontFamily, size, prototype.Style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCjkStandardFont"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public PdfCjkStandardFont(PdfCjkStandardFont prototype, float size, PdfFontStyle style)
            : this(prototype.FontFamily, size, style)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the font family.
        /// </summary>
        public PdfCjkFontFamily FontFamily
        {
            get
            {
                return m_fontFamily;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Checks whether fonts are equals.
        /// </summary>
        /// <param name="font">Font to compare.</param>
        /// <returns>
        /// True if fonts are equal, False otherwise.
        /// </returns>
        protected override bool EqualsToFont(PdfFont font)
        {
            bool equal = false;
            PdfCjkStandardFont cjkFont = font as PdfCjkStandardFont;

            if (cjkFont != null)
            {
                bool fontFamilyEqual = (FontFamily == cjkFont.FontFamily);
                bool styleEqual = (Style & (~(PdfFontStyle.Underline | PdfFontStyle.Strikeout))) ==
                    (cjkFont.Style & (~(PdfFontStyle.Underline | PdfFontStyle.Strikeout)));

                equal = (fontFamilyEqual && styleEqual);
            }

            return equal;
        }

        /// <summary>
        /// Returns width of the char.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <param name="format">String format.</param>
        /// <returns>Width of the symbol.</returns>
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
        protected internal override float GetLineWidth(string line, PdfStringFormat format)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            float width = 0f;

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
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the internals.
        /// </summary>
        private void InitializeInternals()
        {
            //Lock this code to avoid corrupting a static cache by another threads.
            lock (s_syncObject)
            {
                
                IPdfCache equalFont=null;
                if(PdfDocument.EnableCache)
                {
                   equalFont = PdfDocument.Cache.Search(this);
                }
               
                IPdfPrimitive internals = null;

                if (equalFont == null)
                {
                    // Create font metrics.
                    PdfFontMetrics metrics = PdfCjkStandardFontMetricsFactory.GetMetrics(m_fontFamily, Style, Size);

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
            dictionary[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.Type0);
            dictionary[DictionaryProperties.BaseFont] = new PdfName(Metrics.PostScriptName);

            dictionary[DictionaryProperties.Encoding] = GetEncoding(m_fontFamily);
            dictionary[DictionaryProperties.DescendantFonts] = GetDescendantFont();

            return dictionary;
        }

        /// <summary>
        /// Returns descendant font.
        /// </summary>
        /// <returns>Returns descendant font.</returns>
        private PdfArray GetDescendantFont()
        {
            PdfArray df = new PdfArray();
            PdfCidFont cidFont = new PdfCidFont(m_fontFamily, Style, Metrics);

            df.Add(cidFont);

            return df;
        }

        /// <summary>
        /// Gets the prope CJK encoding.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>Proper PDF name for the encoding.</returns>
        private static PdfName GetEncoding(PdfCjkFontFamily fontFamily)
        {
            string encoding = "Unknown";

            switch (fontFamily)
            {
                case PdfCjkFontFamily.HanyangSystemsGothicMedium:
                case PdfCjkFontFamily.HanyangSystemsShinMyeongJoMedium:
                    encoding = "UniKS-UCS2-H";
                    break;

                case PdfCjkFontFamily.HeiseiKakuGothicW5:
                case PdfCjkFontFamily.HeiseiMinchoW3:
                    encoding = "UniJIS-UCS2-H";
                    break;

                case PdfCjkFontFamily.MonotypeHeiMedium:
                case PdfCjkFontFamily.MonotypeSungLight:
                    encoding = "UniCNS-UCS2-H";
                    break;

                case PdfCjkFontFamily.SinoTypeSongLight:
                    encoding = "UniGB-UCS2-H";
                    break;

                default:
                    throw new ArgumentException("Unsupported font face.", "fontFamily");
            }

            PdfName name = new PdfName(encoding);

            return name;
        }

        /// <summary>
        /// Checks the style.
        /// </summary>
        private void CheckStyle()
        {
            PdfFontStyle style = Style;
            SetStyle(style);
        }

        /// <summary>
        /// Gets the char width internal.
        /// </summary>
        /// <param name="charCode">The character code.</param>
        /// <param name="format">The format.</param>
        /// <returns>The width of the character.</returns>
        private float GetCharWidthInternal(char charCode, PdfStringFormat format)
        {
            float width = 0f;
            int code = charCode;
            code = (code >= 0) ? code : 0;

            PdfFontMetrics metrics = Metrics;
            WidthTable widthTable = metrics.WidthTable;

            width = (float)widthTable[code];

            return width;
        }
        #endregion
    }
}
