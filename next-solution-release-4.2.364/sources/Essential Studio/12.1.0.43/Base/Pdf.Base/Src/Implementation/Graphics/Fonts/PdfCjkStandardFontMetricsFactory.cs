#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// A Class representing Pdf document which is used for Cjk Font Metrics Factory.
    /// </summary>
    internal sealed class PdfCjkStandardFontMetricsFactory
    {
        #region Constants
        /// <summary>
        /// Multiplier of subscript superscript.
        /// </summary>
        private const float c_subSuperScriptFactor = 1.52f;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCjkStandardFontMetricsFactory"/> class.
        /// </summary>
        private PdfCjkStandardFontMetricsFactory()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns font metrics depending on the font settings.
        /// </summary>
        /// <param name="fontFamily">Font family.</param>
        /// <param name="fontStyle">Font style.</param>
        /// <param name="size">Size of the font.</param>
        /// <returns>Returns font metrics depending on the font settings.</returns>
        public static PdfFontMetrics GetMetrics(PdfCjkFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = null;

            switch (fontFamily)
            {
                case PdfCjkFontFamily.HanyangSystemsGothicMedium:
                    metrics = GetHanyangSystemsGothicMediumMetrix(fontFamily, fontStyle, size);
                    break;

                case PdfCjkFontFamily.HanyangSystemsShinMyeongJoMedium:
                    metrics = GetHanyangSystemsShinMyeongJoMediumMetrix(fontFamily, fontStyle, size);
                    break;

                case PdfCjkFontFamily.HeiseiKakuGothicW5:
                    metrics = GetHeiseiKakuGothicW5Metrix(fontFamily, fontStyle, size);
                    break;

                case PdfCjkFontFamily.HeiseiMinchoW3:
                    metrics = GetHeiseiMinchoW3(fontFamily, fontStyle, size);
                    break;

                case PdfCjkFontFamily.MonotypeHeiMedium:
                    metrics = GetMonotypeHeiMedium(fontFamily, fontStyle, size);
                    break;

                case PdfCjkFontFamily.MonotypeSungLight:
                    metrics = GetMonotypeSungLightMetrix(fontFamily, fontStyle, size);
                    break;

                case PdfCjkFontFamily.SinoTypeSongLight:
                    metrics = GetSinoTypeSongLight(fontFamily, fontStyle, size);
                    break;

                default:
                    throw new ArgumentException("Unsupported font family", "fontFamily");
            }

            metrics.Name = fontFamily.ToString();
            metrics.SubScriptSizeFactor = c_subSuperScriptFactor;
            metrics.SuperscriptSizeFactor = c_subSuperScriptFactor;

            return metrics;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the hanyang systems gothic medium font metrix.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>The filled font metrix.</returns>
        private static PdfFontMetrics GetHanyangSystemsGothicMediumMetrix(PdfCjkFontFamily fontFamily,
            PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(8094, 8190, 500));

            metrics.Ascent = 880;
            metrics.Descent = -120;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HYGoThic-Medium,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "HYGoThic-Medium,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HYGoThic-Medium,Italic";
            }
            else
            {
                metrics.PostScriptName = "HYGoThic-Medium";
            }

            return metrics;
        }

        /// <summary>
        /// Gets the monotype hei medium metrix.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>The proper PdfFontMetrics class instance.</returns>
        private static PdfFontMetrics GetMonotypeHeiMedium(PdfCjkFontFamily fontFamily,
            PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(13648, 13742, 500));

            metrics.Ascent = 880;
            metrics.Descent = -120;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "MHei-Medium,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "MHei-Medium,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "MHei-Medium,Italic";
            }
            else
            {
                metrics.PostScriptName = "MHei-Medium";
            }

            return metrics;
        }

        /// <summary>
        /// Gets the monotype sung light metrix.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>The proper PdfFontMetrics class instance.</returns>
        private static PdfFontMetrics GetMonotypeSungLightMetrix(PdfCjkFontFamily fontFamily,
            PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(13648, 13742, 500));

            metrics.Ascent = 880;
            metrics.Descent = -120;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "MSung-Light,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "MSung-Light,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "MSung-Light,Italic";
            }
            else
            {
                metrics.PostScriptName = "MSung-Light";
            }

            return metrics;
        }

        /// <summary>
        /// Gets the sino type song light font metrics.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>PdfFontMetrics instance.</returns>
        private static PdfFontMetrics GetSinoTypeSongLight(PdfCjkFontFamily fontFamily,
            PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(814, 939, 500));
            widthTable.Add(new CjkDifferentWidth(7712, new int[] { 500 }));
            widthTable.Add(new CjkDifferentWidth(7716, new int[] { 500 }));

            metrics.Ascent = 880;
            metrics.Descent = -120;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "STSong-Light,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "STSong-Light,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "STSong-Light,Italic";
            }
            else
            {
                metrics.PostScriptName = "STSong-Light";
            }

            return metrics;
        }

        /// <summary>
        /// Gets the heisei mincho w3.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>The proper PdfFontMetrics class instance.</returns>
        private static PdfFontMetrics GetHeiseiMinchoW3(PdfCjkFontFamily fontFamily,
            PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(231, 632, 500));

            metrics.Ascent = 857;
            metrics.Descent = -143;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HeiseiMin-W3,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "HeiseiMin-W3,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HeiseiMin-W3,Italic";
            }
            else
            {
                metrics.PostScriptName = "HeiseiMin-W3";
            }

            return metrics;
        }

        /// <summary>
        /// Gets the heisei kaku gothic w5 metrix.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>The proper PdfFontMetrics class instance.</returns>
        private static PdfFontMetrics GetHeiseiKakuGothicW5Metrix(PdfCjkFontFamily fontFamily,
            PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(231, 632, 500));

            metrics.Ascent = 857;
            metrics.Descent = -125;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HeiseiKakuGo-W5,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "HeiseiKakuGo-W5,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HeiseiKakuGo-W5,Italic";
            }
            else
            {
                metrics.PostScriptName = "HeiseiKakuGo-W5";
            }

            return metrics;
        }

        /// <summary>
        /// Gets the hanyang systems shin myeong jo medium metrix.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="size">The size.</param>
        /// <returns>The proper PdfFontMetrics class instance.</returns>
        private static PdfFontMetrics GetHanyangSystemsShinMyeongJoMediumMetrix(PdfCjkFontFamily fontFamily, PdfFontStyle fontStyle, float size)
        {
            PdfFontMetrics metrics = new PdfFontMetrics();
            CjkWidthTable widthTable = new CjkWidthTable(1000);

            metrics.WidthTable = widthTable;
            widthTable.Add(new CjkSameWidth(1, 95, 500));
            widthTable.Add(new CjkSameWidth(8094, 8190, 500));

            metrics.Ascent = 880;
            metrics.Descent = -120;
            metrics.Size = size;
            metrics.Height = metrics.Ascent - metrics.Descent;

            if ((fontStyle & PdfFontStyle.Bold) != 0 && (fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HYSMyeongJo-Medium,BoldItalic";
            }
            else if ((fontStyle & PdfFontStyle.Bold) != 0)
            {
                metrics.PostScriptName = "HYSMyeongJo-Medium,Bold";
            }
            else if ((fontStyle & PdfFontStyle.Italic) != 0)
            {
                metrics.PostScriptName = "HYSMyeongJo-Medium,Italic";
            }
            else
            {
                metrics.PostScriptName = "HYSMyeongJo-Medium";
            }

            return metrics;
        }
        #endregion
    }
}
