#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Pre-defined palettes for use with the ChartControl. Palettes are simply a group of colors that
    /// can be used to provide a better visual appearance when displaying multiple chart series.
    /// </summary>
    public enum ChartColorPalette
    {

        /// <summary>
        /// Default palette.
        /// </summary>
        Default,

        /// <summary>
        /// Default palette used in older chart versions.
        /// </summary>
        DefaultAlpha,

        /// <summary>
        /// 
        /// </summary>
        DefaultOld,

        /// <summary>
        /// Default palette with alpha blending.
        /// </summary>
        DefaultOldAlpha,

        /// <summary>
        /// Palette containing earth tone colors.
        /// </summary>
        EarthTone,

        /// <summary>
        /// Palette containing analog colors.
        /// </summary>
        Analog,

        /// <summary>
        /// Colorful palette.
        /// </summary>
        Colorful,

        /// <summary>
        /// Palette containing the colors of nature.
        /// </summary>
        Nature,

        /// <summary>
        /// Palette containing pastel colors.
        /// </summary>
        Pastel,

        /// <summary>
        /// Palette containing triad colors.
        /// </summary>
        Triad,

        /// <summary>
        /// Palette that contains mixed warm and cold colors.
        /// </summary>
        WarmCold,

        /// <summary>
        /// GrayScale color palette which can be used for monochrome printing.
        /// </summary>
        GrayScale,

        /// <summary>
        /// Palette that contains mixed SkyBlue and Violet colors.
        /// </summary>
        SkyBlueStyle,

        /// <summary>
        /// Palette that contains mixed Red and yellow colors.
        /// </summary>
        RedYellowStyle,

        /// <summary>
        /// Palette that contains mixed Green and yellow colors.
        /// </summary>
        GreenYellowStyle,

        /// <summary>
        /// Palette that contains pink Green and violet colors.
        /// </summary>
        PinkVioletStyle,

        /// <summary>
        /// Custom user assigned color palette.
        /// </summary>
        Custom
    }

    /// <summary>
    /// The ChartColorModel class serves as a repository for color information. Color information is used by the chart to render colored series.
    /// A group of colors is referred to as a palette of colors. You have the option of choosing from several predefined palettes or creating your
    /// own color palette.
    /// <seealso cref="ChartColorPalette"/>
    /// <seealso cref="ChartColorModel.Palette"/>
    /// <seealso cref="ChartColorModel.CustomColors"/>
    /// </summary>
    public sealed class ChartColorModel
    {
        #region Constants
        /// <summary>
        /// The number of colors in the ChartColorModel's palette. If the number of series exceeds the number of colors in the palette (16 in the
        /// current version), colors will be repeated.
        /// </summary>
        public const int NumColorsInPalette = 17;

        private const int c_alpha = 150;
        private static Color[] c_defaultColorTable;
        private static Color[] c_skyblueColorTable;
        private static Color[] c_redyellowColorTable;
        private static Color[] c_greenyellowColorTable;
        private static Color[] c_pinkvioletColorTable;
        private static Color[] c_defaultAlphaColorTable;
        private static Color[] c_defaultOldColorTable;
        private static Color[] c_defaultOldAlphaColorTable;
        private static Color[] c_earthTonesColorTable;
        private static Color[] c_analogColorTable;
        private static Color[] c_colorfulColorTable;
        private static Color[] c_natureColorTable;
        private static Color[] c_pastelColorTable;
        private static Color[] c_triadColorTable;
        private static Color[] c_warmColdColorTable;
        private static Color[] c_grayScaleColorTable;
        #endregion

        #region Members
        private Color[] m_activePalette;

        private ChartColorPalette m_colorPalette;
        private Color[] m_customColorTable;
        private bool m_allowGradient = false;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when palette is changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets or sets the table of custom colors to be used. Series will be colored with color data from this color table. Individual series
        ///     color can still be overriden by specifying style attributes. Palette information is used only when no specific style information
        ///     is available on the color to be used for the series.
        ///     <seealso cref="ChartColorModel"/>
        ///     <seealso cref="ChartColorModel.Palette"/>
        /// </summary>
        /// <value>The custom colors.</value>
        public Color[] CustomColors
        {
            get
            {
                return m_customColorTable;
            }

            set
            {
                if (m_customColorTable != value)
                {
                    m_customColorTable = value;

                    if (m_colorPalette == ChartColorPalette.Custom)
                    {
                        if (m_customColorTable.Length > 0)
                           m_activePalette = m_customColorTable;
                        this.RaiseChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the color palette to be used. Series will be colored with color data from this palette (color table). Individual series
        ///     color can still be overriden by specifying style attributes. Palette information is used only when no specific style information
        ///     is available on the color to be used for the series.
        ///     <seealso cref="ChartColorModel"/>
        ///     <seealso cref="ChartColorModel.CustomColors"/>
        /// </summary>
        public ChartColorPalette Palette
        {
            get
            {
                return m_colorPalette;
            }

            set
            {
                if (m_colorPalette != value)
                {
                    Color[] colors = this.GetPalette(value);
                    m_colorPalette = value;

                    if (colors != null)
                    {
                        m_activePalette = colors;
                        //m_colorPalette = value;
                        this.RaiseChanged(this, EventArgs.Empty);
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [allow gradient].
        /// </summary>
        /// <value><c>True</c> if [allow gradient]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool AllowGradient
        {
            get 
            {
                return m_allowGradient; 
            }

            set
            {
                if (m_allowGradient != value)
                {
                    m_allowGradient = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="ChartColorModel"/> class.
        /// </summary>
        static ChartColorModel()
        {
            InitializePalettes();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartColorModel"/> class.
        /// </summary>
        internal ChartColorModel()
        {
            m_colorPalette = ChartColorPalette.Default;
            m_activePalette = this.GetPalette(m_colorPalette);
        }
        #endregion

        #region Public methdos
        /// <summary>
        /// Returns the color (from the palette) corresponding to the specified index value.
        /// </summary>
        /// <param name="index" type="int">
        ///   <para>
        ///		The index value of the color to be returned.
        ///   </para>
        /// </param>
        /// <returns>
        ///     A System.Drawing.Color value that is used as the back color for the series.
        /// </returns>
        public Color GetColor(int index)
        {            
            if (index >= 0 && m_activePalette != null && (m_activePalette.Length > 0))
            {
                return m_activePalette[index % m_activePalette.Length];
            }

            return Color.Empty;
        }

        /// <summary>
        /// Creates the palette icon.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <param name="palette">The palette.</param>
        /// <param name="colorCount">The color count.</param>
        /// <returns></returns>
        public Image CreatePaletteIcon(Size sz, ChartColorPalette palette, int colorCount)
        {
            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            float xOffset = sz.Width / (float)colorCount;
            Color[] colors = this.GetPalette(palette);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (colors != null && colors.Length > 0)
                {
                    for (int i = 0; i < colorCount; i++)
                    {
                        using (SolidBrush sb = new SolidBrush(colors[i % colors.Length]))
                        {
                            g.FillRectangle(sb, i * xOffset, 0, xOffset, sz.Height);
                        }
                    }
                }

                g.DrawRectangle(Pens.Black, 0, 0, sz.Width - 1, sz.Height - 1);
            }

            return bmp;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the SkyBlue palette.
        /// </summary>
        
        private static void InitializeSkyBlueStyle()
        {

            c_skyblueColorTable = new Color[]{
            ColorTranslator.FromHtml("#2E7472"),
            ColorTranslator.FromHtml("#48C1BD"),
            ColorTranslator.FromHtml("#53BD8C"),
            ColorTranslator.FromHtml("#37855D"),
            ColorTranslator.FromHtml("#003A3A"),
            ColorTranslator.FromHtml("#3E9F95"),
            ColorTranslator.FromHtml("#3CB769"),
            ColorTranslator.FromHtml("#5B7D41"),
            ColorTranslator.FromHtml("#5CC3CA"),
            ColorTranslator.FromHtml("#6891CB"),
            ColorTranslator.FromHtml("#5B5FAB"),
            ColorTranslator.FromHtml("#613A5B"),
            ColorTranslator.FromHtml("#A470AC"),
            ColorTranslator.FromHtml("#180B42"),
            ColorTranslator.FromHtml("#5150A2"),
            ColorTranslator.FromHtml("#965DA2")
            };

        }

        /// <summary>
        /// Initializes the Red-Yellow palette.
        /// </summary>

        private static void InitializeRedYellowStyle()
        {

            c_redyellowColorTable = new Color[]{
            ColorTranslator.FromHtml("#6E3F98"),
            ColorTranslator.FromHtml("#DF1D3B"),
            ColorTranslator.FromHtml("#205F2F"),
            ColorTranslator.FromHtml("#FCAF17"),
            ColorTranslator.FromHtml("#86328C"),
            ColorTranslator.FromHtml("#FFF200"),
            ColorTranslator.FromHtml("#F4783B"),
            ColorTranslator.FromHtml("#2E3192"),
            ColorTranslator.FromHtml("#00A9A3"),
            ColorTranslator.FromHtml("#A51D35"),
            ColorTranslator.FromHtml("#EC008C"),
            ColorTranslator.FromHtml("#702C8D"),
            ColorTranslator.FromHtml("#00A651"),
            ColorTranslator.FromHtml("#E41E26"),
            ColorTranslator.FromHtml("#0071B5"),
            ColorTranslator.FromHtml("#B2D235")
            };

        }


        /// <summary>
        /// Initializes the Green-Yellow palette.
        /// </summary>

        private static void InitializeGreenYellowStyle()
        {

            c_greenyellowColorTable = new Color[]{
            ColorTranslator.FromHtml("#231F20"),
            ColorTranslator.FromHtml("#8C1846"),
            ColorTranslator.FromHtml("#EB2E92"),
            ColorTranslator.FromHtml("#592C8A"),
            ColorTranslator.FromHtml("#57C9E8"),
            ColorTranslator.FromHtml("#5894CE"),
            ColorTranslator.FromHtml("#F47820"),
            ColorTranslator.FromHtml("#FFCB05"),
            ColorTranslator.FromHtml("#57803A"),
            ColorTranslator.FromHtml("#CDDC29"),
            ColorTranslator.FromHtml("#76C043"),
            ColorTranslator.FromHtml("#52260F"),
            ColorTranslator.FromHtml("#EE7123"),
            ColorTranslator.FromHtml("#FBF281"),
            ColorTranslator.FromHtml("#B76E11"),
            ColorTranslator.FromHtml("#ED1C24")
            };

        }

        /// <summary>
        /// Initializes the Pink-Violet palette.
        /// </summary>

        private static void InitializePinkVioletStyle()
        {

            c_pinkvioletColorTable = new Color[]{
            ColorTranslator.FromHtml("#E16131"),
            ColorTranslator.FromHtml("#E8A92A"),
            ColorTranslator.FromHtml("#DEE340"),
            ColorTranslator.FromHtml("#585E2D"),
            ColorTranslator.FromHtml("#ADB03C"),
            ColorTranslator.FromHtml("#DFE791"),
            ColorTranslator.FromHtml("#EBBD22"),
            ColorTranslator.FromHtml("#F5D28D"),
            ColorTranslator.FromHtml("#C06231"),
            ColorTranslator.FromHtml("#C18C3F"),
            ColorTranslator.FromHtml("#5F411D"),
            ColorTranslator.FromHtml("#D4CB82"),
            ColorTranslator.FromHtml("#7D6F2F"),
            ColorTranslator.FromHtml("#AE9735"),
            ColorTranslator.FromHtml("#E1D055"),
            ColorTranslator.FromHtml("#B86A2F")
            };

        }

        /// <summary>
        /// Initializes the default palette.
        /// </summary>
        private static void InitializeDefaultPalette()
        {
            c_defaultColorTable = new Color[]{
                                Color.FromArgb(237, 139, 76),
                                Color.FromArgb(43, 63, 122),
                                Color.FromArgb(183, 69, 77),
                                Color.FromArgb(245, 76, 78),
                                Color.FromArgb(173, 209, 63),
                                Color.FromArgb(106, 106, 160),
                                Color.FromArgb(167, 185, 205),
                                Color.FromArgb(136, 149, 52),
                                Color.FromArgb(117, 187, 238),
                                Color.FromArgb(204, 135, 119),
                                Color.FromArgb(161, 100, 55),
                                Color.FromArgb(152, 147, 143),
                                Color.FromArgb(50, 50, 50),
                                Color.FromArgb(128, 0, 0),
                                Color.FromArgb(0, 64, 255),
                                Color.FromArgb(184, 2, 184)
      };
        }

        /// <summary>
        /// Initializes the default alpha palette.
        /// </summary>
        private static void InitializeDefaultAlphaPalette()
        {
            c_defaultAlphaColorTable = new Color[NumColorsInPalette];
            int index = 0;

            foreach (Color color in c_defaultColorTable)
            {
                c_defaultAlphaColorTable[index++] = Color.FromArgb(c_alpha, color);
            }
        }

        /// <summary>
        /// Initializes the default old alpha palette.
        /// </summary>
        private static void InitializeDefaultOldAlphaPalette()
        {
            c_defaultOldAlphaColorTable = new Color[NumColorsInPalette];
            int index = 0;

            foreach (Color color in c_defaultOldColorTable)
            {
                c_defaultOldAlphaColorTable[index++] = Color.FromArgb(c_alpha, color);
            }
        }

        /// <summary>
        /// Initializes the default old palette.
        /// </summary>
        private static void InitializeDefaultOldPalette()
        {
            c_defaultOldColorTable = new Color[NumColorsInPalette];
            c_defaultOldColorTable[0] = Color.FromArgb(153, 153, 255);
            c_defaultOldColorTable[1] = Color.FromArgb(153, 51, 102);
            c_defaultOldColorTable[2] = Color.FromArgb(255, 255, 204);
            c_defaultOldColorTable[3] = Color.FromArgb(102, 0, 102);
            c_defaultOldColorTable[4] = Color.FromArgb(204, 255, 255);
            c_defaultOldColorTable[5] = Color.FromArgb(255, 128, 128);
            c_defaultOldColorTable[6] = Color.FromArgb(0, 102, 204);
            c_defaultOldColorTable[7] = Color.FromArgb(204, 204, 255);
            c_defaultOldColorTable[8] = Color.FromArgb(0, 128, 128);
            c_defaultOldColorTable[9] = Color.FromArgb(255, 117, 186);
            c_defaultOldColorTable[10] = Color.FromArgb(255, 255, 153);
            c_defaultOldColorTable[11] = Color.FromArgb(3, 198, 198);
            c_defaultOldColorTable[12] = Color.FromArgb(128, 0, 128);
            c_defaultOldColorTable[13] = Color.FromArgb(128, 0, 0);
            c_defaultOldColorTable[14] = Color.FromArgb(0, 128, 255);
            c_defaultOldColorTable[15] = Color.FromArgb(184, 2, 184);
        }

        /// <summary>
        /// Initializes the earth tone palette.
        /// </summary>
        private static void InitializeEarthTonePalette()
        {
            c_earthTonesColorTable = new Color[NumColorsInPalette];

            c_earthTonesColorTable[0] = Color.FromArgb(142, 255, 0, 0);
            c_earthTonesColorTable[1] = Color.FromArgb(142, 0, 255, 0);
            c_earthTonesColorTable[2] = Color.FromArgb(142, 0, 0, 255);
            c_earthTonesColorTable[3] = Color.FromArgb(142, 255, 255, 0);
            c_earthTonesColorTable[4] = Color.FromArgb(142, 0, 255, 255);
            c_earthTonesColorTable[5] = Color.FromArgb(142, 255, 0, 255);
            c_earthTonesColorTable[6] = Color.FromArgb(142, 170, 120, 20);
            c_earthTonesColorTable[7] = Color.FromArgb(70, 255, 0, 0);
            c_earthTonesColorTable[8] = Color.FromArgb(70, 0, 255, 0);
            c_earthTonesColorTable[9] = Color.FromArgb(70, 0, 0, 255);
            c_earthTonesColorTable[10] = Color.FromArgb(70, 255, 255, 0);
            c_earthTonesColorTable[11] = Color.FromArgb(70, 0, 255, 255);
            c_earthTonesColorTable[12] = Color.FromArgb(70, 255, 0, 255);
            c_earthTonesColorTable[13] = Color.FromArgb(70, 170, 120, 20);
            c_earthTonesColorTable[14] = Color.FromArgb(132, 100, 120, 50);
            c_earthTonesColorTable[15] = Color.FromArgb(132, 40, 80, 150);
        }

        /// <summary>
        /// Initializes the analog palette.
        /// </summary>
        private static void InitializeAnalogPalette()
        {
            c_analogColorTable = new Color[NumColorsInPalette];

            c_analogColorTable[0] = Color.FromArgb(0, 134, 137);
            c_analogColorTable[1] = Color.FromArgb(32, 55, 189);
            c_analogColorTable[2] = Color.FromArgb(47, 166, 208);
            c_analogColorTable[3] = Color.FromArgb(96, 126, 218);
            c_analogColorTable[4] = Color.FromArgb(87, 161, 255);
            c_analogColorTable[5] = Color.FromArgb(82, 255, 254);
            c_analogColorTable[6] = Color.FromArgb(47, 122, 208);
            c_analogColorTable[7] = Color.FromArgb(47, 206, 208);
            c_analogColorTable[8] = Color.FromArgb(96, 157, 218);
            c_analogColorTable[9] = Color.FromArgb(0, 200, 202);
            c_analogColorTable[10] = Color.FromArgb(45, 15, 156);
            c_analogColorTable[11] = Color.FromArgb(56, 227, 228);
            c_analogColorTable[12] = Color.FromArgb(47, 65, 208);
            c_analogColorTable[13] = Color.FromArgb(47, 158, 208);
            c_analogColorTable[14] = Color.FromArgb(145, 187, 230);
            c_analogColorTable[15] = Color.FromArgb(36, 10, 132);
        }

        /// <summary>
        /// Initializes the colorful palette.
        /// </summary>
        private static void InitializeColorfulPalette()
        {
            c_colorfulColorTable = new Color[NumColorsInPalette];

            c_colorfulColorTable[0] = Color.FromArgb(0, 0, 255);
            c_colorfulColorTable[1] = Color.FromArgb(251, 59, 153);
            c_colorfulColorTable[2] = Color.FromArgb(0, 255, 255);
            c_colorfulColorTable[3] = Color.FromArgb(0, 128, 255);

            c_colorfulColorTable[4] = Color.FromArgb(255, 0, 128);
            c_colorfulColorTable[5] = Color.FromArgb(255, 255, 122);
            c_colorfulColorTable[6] = Color.FromArgb(128, 0, 255);
            c_colorfulColorTable[7] = Color.FromArgb(0, 255, 128);

            c_colorfulColorTable[8] = Color.FromArgb(218, 2, 2);
            c_colorfulColorTable[9] = Color.FromArgb(255, 255, 61);
            c_colorfulColorTable[10] = Color.FromArgb(122, 122, 255);
            c_colorfulColorTable[11] = Color.FromArgb(0, 255, 0);

            c_colorfulColorTable[12] = Color.FromArgb(255, 255, 61);
            c_colorfulColorTable[13] = Color.FromArgb(255, 0, 0);
            c_colorfulColorTable[14] = Color.FromArgb(0, 224, 224);
            c_colorfulColorTable[15] = Color.FromArgb(1, 70, 175);
        }

        /// <summary>
        /// Initializes the nature palette.
        /// </summary>
        private static void InitializeNaturePalette()
        {
            c_natureColorTable = new Color[NumColorsInPalette];

            c_natureColorTable[0] = Color.FromArgb(119, 149, 17);
            c_natureColorTable[1] = Color.FromArgb(119, 17, 119);
            c_natureColorTable[2] = Color.FromArgb(17, 99, 180);
            c_natureColorTable[3] = Color.FromArgb(241, 129, 17);

            c_natureColorTable[4] = Color.FromArgb(241, 223, 17);
            c_natureColorTable[5] = Color.FromArgb(66, 153, 42);
            c_natureColorTable[6] = Color.FromArgb(17, 68, 119);
            c_natureColorTable[7] = Color.FromArgb(119, 17, 17);

            c_natureColorTable[8] = Color.FromArgb(68, 119, 17);
            c_natureColorTable[9] = Color.FromArgb(17, 17, 119);
            c_natureColorTable[10] = Color.FromArgb(119, 17, 68);
            c_natureColorTable[11] = Color.FromArgb(224, 86, 19);

            c_natureColorTable[12] = Color.FromArgb(236, 191, 12);
            c_natureColorTable[13] = Color.FromArgb(95, 172, 18);
            c_natureColorTable[14] = Color.FromArgb(55, 130, 205);
            c_natureColorTable[15] = Color.FromArgb(1, 1, 105);
        }

        /// <summary>
        /// Initializes the pastel palette.
        /// </summary>
        private static void InitializePastelPalette()
        {
            c_pastelColorTable = new Color[NumColorsInPalette];

            c_pastelColorTable[0] = Color.FromArgb(163, 163, 245);
            c_pastelColorTable[1] = Color.FromArgb(163, 245, 163);
            c_pastelColorTable[2] = Color.FromArgb(239, 173, 108);
            c_pastelColorTable[3] = Color.FromArgb(53, 142, 232);

            c_pastelColorTable[4] = Color.FromArgb(53, 67, 232);
            c_pastelColorTable[5] = Color.FromArgb(158, 142, 198);
            c_pastelColorTable[6] = Color.FromArgb(245, 204, 163);
            c_pastelColorTable[7] = Color.FromArgb(163, 245, 245);

            c_pastelColorTable[8] = Color.FromArgb(204, 163, 245);
            c_pastelColorTable[9] = Color.FromArgb(245, 245, 163);
            c_pastelColorTable[10] = Color.FromArgb(163, 245, 204);
            c_pastelColorTable[11] = Color.FromArgb(68, 178, 232);

            c_pastelColorTable[12] = Color.FromArgb(53, 97, 232);
            c_pastelColorTable[13] = Color.FromArgb(245, 245, 163);
            c_pastelColorTable[14] = Color.FromArgb(201, 151, 118);
            c_pastelColorTable[15] = Color.FromArgb(176, 115, 238);
        }

        /// <summary>
        /// Initializes the triad palette.
        /// </summary>
        private static void InitializeTriadPalette()
        {
            c_triadColorTable = new Color[NumColorsInPalette];

            c_triadColorTable[0] = Color.FromArgb(190, 0, 255);
            c_triadColorTable[1] = Color.FromArgb(0, 255, 190);
            c_triadColorTable[2] = Color.FromArgb(255, 190, 0);
            c_triadColorTable[3] = Color.FromArgb(206, 255, 61);

            c_triadColorTable[4] = Color.FromArgb(155, 122, 255);
            c_triadColorTable[5] = Color.FromArgb(88, 4, 116);
            c_triadColorTable[6] = Color.FromArgb(0, 255, 127);
            c_triadColorTable[7] = Color.FromArgb(255, 127, 0);

            c_triadColorTable[8] = Color.FromArgb(254, 255, 61);
            c_triadColorTable[9] = Color.FromArgb(122, 122, 255);
            c_triadColorTable[10] = Color.FromArgb(0, 0, 102);
            c_triadColorTable[11] = Color.FromArgb(168, 168, 252);

            c_triadColorTable[12] = Color.FromArgb(0, 255, 204);
            c_triadColorTable[13] = Color.FromArgb(255, 204, 0);
            c_triadColorTable[14] = Color.FromArgb(254, 255, 120);
            c_triadColorTable[15] = Color.FromArgb(199, 199, 255);
        }

        /// <summary>
        /// Initializes the warm cold palette.
        /// </summary>
        private static void InitializeWarmColdPalette()
        {
            c_warmColdColorTable = new Color[NumColorsInPalette];

            c_warmColdColorTable[0] = Color.FromArgb(7, 89, 166);
            c_warmColdColorTable[1] = Color.FromArgb(255, 225, 0);
            c_warmColdColorTable[2] = Color.FromArgb(255, 72, 0);
            c_warmColdColorTable[3] = Color.FromArgb(255, 213, 61);

            c_warmColdColorTable[4] = Color.FromArgb(122, 151, 255);
            c_warmColdColorTable[5] = Color.FromArgb(35, 4, 116);
            c_warmColdColorTable[6] = Color.FromArgb(255, 141, 0);
            c_warmColdColorTable[7] = Color.FromArgb(255, 9, 0);

            c_warmColdColorTable[8] = Color.FromArgb(255, 163, 61);
            c_warmColdColorTable[9] = Color.FromArgb(122, 184, 255);
            c_warmColdColorTable[10] = Color.FromArgb(0, 47, 102);
            c_warmColdColorTable[11] = Color.FromArgb(168, 207, 252);

            c_warmColdColorTable[12] = Color.FromArgb(255, 234, 0);
            c_warmColdColorTable[13] = Color.FromArgb(255, 86, 0);
            c_warmColdColorTable[14] = Color.FromArgb(255, 192, 120);
            c_warmColdColorTable[15] = Color.FromArgb(199, 225, 255);
        }

        /// <summary>
        /// Initializes the gray scale palette.
        /// </summary>
        private static void InitializeGrayScalePalette()
        {
            c_grayScaleColorTable = new Color[NumColorsInPalette];

            c_grayScaleColorTable[0] = Color.FromArgb(204, 204, 204);
            c_grayScaleColorTable[1] = Color.FromArgb(221, 221, 221);
            c_grayScaleColorTable[2] = Color.FromArgb(238, 238, 238);
            c_grayScaleColorTable[3] = Color.FromArgb(255, 255, 255);

            c_grayScaleColorTable[4] = Color.FromArgb(68, 68, 68);
            c_grayScaleColorTable[5] = Color.FromArgb(85, 85, 85);
            c_grayScaleColorTable[6] = Color.FromArgb(102, 102, 102);

            c_grayScaleColorTable[7] = Color.FromArgb(119, 119, 119);
            c_grayScaleColorTable[8] = Color.FromArgb(136, 136, 136);
            c_grayScaleColorTable[9] = Color.FromArgb(153, 153, 153);

            c_grayScaleColorTable[10] = Color.FromArgb(170, 170, 170);
            c_grayScaleColorTable[11] = Color.FromArgb(187, 187, 187);
            c_grayScaleColorTable[12] = Color.FromArgb(0, 0, 0);
            c_grayScaleColorTable[13] = Color.FromArgb(17, 17, 17);
            c_grayScaleColorTable[14] = Color.FromArgb(34, 34, 34);
            c_grayScaleColorTable[15] = Color.FromArgb(51, 51, 51);

        }

        /// <summary>
        /// Initializes the palettes.
        /// </summary>
        private static void InitializePalettes()
        {
            InitializeSkyBlueStyle();
            InitializeRedYellowStyle();
            InitializeGreenYellowStyle();
            InitializePinkVioletStyle();
            InitializeDefaultPalette();
            InitializeDefaultAlphaPalette();
            InitializeDefaultOldPalette();
            InitializeDefaultOldAlphaPalette();
            InitializeEarthTonePalette();
            InitializeAnalogPalette();
            InitializeColorfulPalette();
            InitializeNaturePalette();
            InitializePastelPalette();
            InitializeTriadPalette();
            InitializeWarmColdPalette();
            InitializeGrayScalePalette();
        }

        /// <summary>
        /// Gets the palette.
        /// </summary>
        /// <param name="palette">The palette.</param>
        /// <returns></returns>
        private Color[] GetPalette(ChartColorPalette palette)
        {
            Color[] result = null;

            switch (palette)
            {
                case ChartColorPalette.Default:
                    result = c_defaultColorTable;
                    break;

                case ChartColorPalette.DefaultAlpha:
                    result = c_defaultAlphaColorTable;
                    break;

                case ChartColorPalette.DefaultOld:
                    result = c_defaultOldColorTable;
                    break;

                case ChartColorPalette.DefaultOldAlpha:
                    result = c_defaultOldAlphaColorTable;
                    break;

                case ChartColorPalette.EarthTone:
                    result = c_earthTonesColorTable;
                    break;

                case ChartColorPalette.Analog:
                    result = c_analogColorTable;
                    break;

                case ChartColorPalette.Colorful:
                    result = c_colorfulColorTable;
                    break;

                case ChartColorPalette.Nature:
                    result = c_natureColorTable;
                    break;

                case ChartColorPalette.Pastel:
                    result = c_pastelColorTable;
                    break;

                case ChartColorPalette.Triad:
                    result = c_triadColorTable;
                    break;

                case ChartColorPalette.WarmCold:
                    result = c_warmColdColorTable;
                    break;

                case ChartColorPalette.GrayScale:
                    result = c_grayScaleColorTable;
                    break;
                case ChartColorPalette.SkyBlueStyle:
                    result = c_skyblueColorTable;
                    break;
                case ChartColorPalette.RedYellowStyle:
                    result = c_redyellowColorTable;
                    break;
                case ChartColorPalette.GreenYellowStyle:
                    result = c_greenyellowColorTable;
                    break;
                case ChartColorPalette.PinkVioletStyle:
                    result = c_pinkvioletColorTable;
                    break;
                case ChartColorPalette.Custom:                    
                    if (m_customColorTable != null && (m_customColorTable.Length > 0))
                    {
                        result = m_customColorTable;
                    }
                    break;
                default:
                    result = c_defaultOldAlphaColorTable;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RaiseChanged(object sender, EventArgs args)
        {
            if (Changed != null)
            {
                Changed(sender, args);
            }
        }
        #endregion
    }
}