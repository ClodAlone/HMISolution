#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// A Class representing Pdf document which is used for Cjk Font Descryptor Factory.
    /// </summary>
    internal sealed class PdfCjkFontDescryptorFactory
    {
        /// <summary>
        /// Gets the font descryptor.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        /// <returns>font Descryptor</returns>
        internal static PdfDictionary GetFontDescryptor(PdfCjkFontFamily fontFamily, PdfFontStyle fontStyle,
            PdfFontMetrics fontMetrics)
        {
            PdfDictionary fontDescryptor = new PdfDictionary();

            switch (fontFamily)
            {
                case PdfCjkFontFamily.HanyangSystemsGothicMedium:
                    FillHanyangSystemsGothicMedium(fontDescryptor, fontFamily, fontMetrics);
                    break;

                case PdfCjkFontFamily.HanyangSystemsShinMyeongJoMedium:
                    FillHanyangSystemsShinMyeongJoMedium(fontDescryptor, fontFamily, fontMetrics);
                    break;

                case PdfCjkFontFamily.HeiseiKakuGothicW5:
                    FillHeiseiKakuGothicW5(fontDescryptor, fontStyle, fontFamily, fontMetrics);
                    break;

                case PdfCjkFontFamily.HeiseiMinchoW3:
                    FillHeiseiMinchoW3(fontDescryptor, fontFamily, fontMetrics);
                    break;

                case PdfCjkFontFamily.MonotypeHeiMedium:
                    FillMonotypeHeiMedium(fontDescryptor, fontFamily, fontMetrics);
                    break;

                case PdfCjkFontFamily.MonotypeSungLight:
                    FillMonotypeSungLight(fontDescryptor, fontFamily, fontMetrics);
                    break;

                case PdfCjkFontFamily.SinoTypeSongLight:
                    FillSinoTypeSongLight(fontDescryptor, fontFamily, fontMetrics);
                    break;

                default:
                    break;
            }

            return fontDescryptor;
        }

        /// <summary>
        /// Fills the monotype sung light font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillMonotypeSungLight(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(-160, -249, 1015 + 160, 888 + 249);

            FillFontBBox(fontDescryptor, fontBBox);
            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = width;
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(880);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(616);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the heisei kaku gothic w5 font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillHeiseiKakuGothicW5(PdfDictionary fontDescryptor, PdfFontStyle fontStyle,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(-92, -250, 1010 + 92, 922 + 250);
            System.Drawing.Rectangle fontBBoxI = new System.Drawing.Rectangle(-92, -250, 1010 + 92, 922 + 1010); //Italic

            if ((fontStyle & (PdfFontStyle.Italic | PdfFontStyle.Bold)) != PdfFontStyle.Italic)
            {
                FillFontBBox(fontDescryptor, fontBBox);
            }
            else
            {
                FillFontBBox(fontDescryptor, fontBBoxI);
            }

            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = new PdfNumber(689);
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(718);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(500);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the hanyang systems shin myeong jo medium font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillHanyangSystemsShinMyeongJoMedium(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(0, -148, 1001, 880 + 148);

            FillFontBBox(fontDescryptor, fontBBox);
            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = width;
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(880);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(616);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the heisei mincho w3 font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillHeiseiMinchoW3(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(-123, -257, 1001 + 123, 910 + 257);

            FillFontBBox(fontDescryptor, fontBBox);
            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = new PdfNumber(702);
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(718);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(500);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the sino type song light font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillSinoTypeSongLight(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(-25, -254, 1000 + 25, 880 + 254);

            FillFontBBox(fontDescryptor, fontBBox);
            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = width;
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(880);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(616);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the monotype hei medium font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillMonotypeHeiMedium(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(-45, -250, 1015 + 45, 887 + 250);

            FillFontBBox(fontDescryptor, fontBBox);
            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = width;
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(880);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(616);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the hanyang systems gothic medium font descryptor.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillHanyangSystemsGothicMedium(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            System.Drawing.Rectangle fontBBox = new System.Drawing.Rectangle(-6, -145, 1003 + 6, 880 + 145);

            FillFontBBox(fontDescryptor, fontBBox);
            FillKnownInfo(fontDescryptor, fontFamily, fontMetrics);

            fontDescryptor[DictionaryProperties.Flags] = new PdfNumber(4);

            PdfNumber stem = new PdfNumber(93);
            fontDescryptor[DictionaryProperties.StemV] = stem;
            fontDescryptor[DictionaryProperties.StemH] = stem;

            PdfNumber width = new PdfNumber(1000);
            fontDescryptor[DictionaryProperties.AvgWidth] = width;
            fontDescryptor[DictionaryProperties.MaxWidth] = width;

            fontDescryptor[DictionaryProperties.CapHeight] = new PdfNumber(880);
            fontDescryptor[DictionaryProperties.XHeight] = new PdfNumber(616);

            fontDescryptor[DictionaryProperties.Leading] = new PdfNumber(250);
        }

        /// <summary>
        /// Fills the known info.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        private static void FillKnownInfo(PdfDictionary fontDescryptor,
            PdfCjkFontFamily fontFamily, PdfFontMetrics fontMetrics)
        {
            fontDescryptor[DictionaryProperties.FontName] = new PdfName(fontMetrics.PostScriptName);
            fontDescryptor[DictionaryProperties.Type] = new PdfName(DictionaryProperties.FontDescriptor);
            fontDescryptor[DictionaryProperties.ItalicAngle] = new PdfNumber(0);
            fontDescryptor[DictionaryProperties.MissingWidth] =
                new PdfNumber((fontMetrics.WidthTable as CjkWidthTable).DefaultWidth);

            fontDescryptor[DictionaryProperties.Ascent] = new PdfNumber(fontMetrics.Ascent);
            fontDescryptor[DictionaryProperties.Descent] = new PdfNumber(fontMetrics.Descent);

            FillFlags(fontDescryptor, fontFamily);
        }

        /// <summary>
        /// Fills the flags.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontFamily">The font family.</param>
        private static void FillFlags(PdfDictionary fontDescryptor, PdfCjkFontFamily fontFamily)
        {
            switch (fontFamily)
            {
                case PdfCjkFontFamily.MonotypeHeiMedium:
                case PdfCjkFontFamily.HanyangSystemsGothicMedium:
                case PdfCjkFontFamily.HeiseiKakuGothicW5:
                    fontDescryptor[DictionaryProperties.Flags] = new PdfNumber(4);
                    break;

                case PdfCjkFontFamily.SinoTypeSongLight:
                case PdfCjkFontFamily.MonotypeSungLight:
                case PdfCjkFontFamily.HanyangSystemsShinMyeongJoMedium:
                case PdfCjkFontFamily.HeiseiMinchoW3:
                    fontDescryptor[DictionaryProperties.Flags] = new PdfNumber(6);
                    break;

                default:
                    throw new ArgumentException("Unsupported font family: " + fontFamily, "fontFamily");
            }
        }

        /// <summary>
        /// Fills the font BBox.
        /// </summary>
        /// <param name="fontDescryptor">The font descryptor.</param>
        /// <param name="fontBBox">The font BBox.</param>
        private static void FillFontBBox(PdfDictionary fontDescryptor, System.Drawing.Rectangle fontBBox)
        {
            fontDescryptor[DictionaryProperties.FontBBox] = PdfArray.FromRectangle(fontBBox);
        }
    }
}
