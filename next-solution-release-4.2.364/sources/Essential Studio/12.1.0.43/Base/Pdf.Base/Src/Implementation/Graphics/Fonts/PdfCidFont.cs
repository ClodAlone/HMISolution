#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// A Class representing Pdf document which is used for Cid Font Descryptor Factory.
    /// </summary>
    internal class PdfCidFont : PdfDictionary
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCidFont"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontMetrics">The font metrics.</param>
        public PdfCidFont(PdfCjkFontFamily fontFamily, PdfFontStyle fontStyle, PdfFontMetrics fontMetrics)
        {
            this[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Font);
            this[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.CIDFontType2);
            this[DictionaryProperties.BaseFont] = new PdfName(fontMetrics.PostScriptName);
            this[DictionaryProperties.DW] =
                new PdfNumber((fontMetrics.WidthTable as CjkWidthTable).DefaultWidth);

            this[DictionaryProperties.W] = fontMetrics.WidthTable.ToArray();
            this[DictionaryProperties.FontDescriptor] =
                PdfCjkFontDescryptorFactory.GetFontDescryptor(fontFamily, fontStyle, fontMetrics);
            this[DictionaryProperties.CIDSystemInfo] = GetSystemInfo(fontFamily);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Gets the system info.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>The properly formed dictionary.</returns>
        private PdfDictionary GetSystemInfo(PdfCjkFontFamily fontFamily)
        {
            PdfDictionary sysInfo = new PdfDictionary();

            sysInfo[DictionaryProperties.Registry] = new PdfString("Adobe");

            switch (fontFamily)
            {
                case PdfCjkFontFamily.HanyangSystemsGothicMedium:
                case PdfCjkFontFamily.HanyangSystemsShinMyeongJoMedium:
                    sysInfo[DictionaryProperties.Ordering] = new PdfString("Korea1");
                    sysInfo[DictionaryProperties.Supplement] = new PdfNumber(1);
                    break;

                case PdfCjkFontFamily.HeiseiKakuGothicW5:
                case PdfCjkFontFamily.HeiseiMinchoW3:
                    sysInfo[DictionaryProperties.Ordering] = new PdfString("Japan1");
                    sysInfo[DictionaryProperties.Supplement] = new PdfNumber(2);
                    break;

                case PdfCjkFontFamily.MonotypeHeiMedium:
                case PdfCjkFontFamily.MonotypeSungLight:
                    sysInfo[DictionaryProperties.Ordering] = new PdfString("CNS1");
                    sysInfo[DictionaryProperties.Supplement] = new PdfNumber(0);
                    break;

                case PdfCjkFontFamily.SinoTypeSongLight:
                    sysInfo[DictionaryProperties.Ordering] = new PdfString("GB1");
                    sysInfo[DictionaryProperties.Supplement] = new PdfNumber(2);
                    break;

                default:
                    throw new ArgumentException("Unsupported font family: " + fontFamily.ToString(), "fontFamily");
            }

            return sysInfo;
        }
        #endregion
    }
}
