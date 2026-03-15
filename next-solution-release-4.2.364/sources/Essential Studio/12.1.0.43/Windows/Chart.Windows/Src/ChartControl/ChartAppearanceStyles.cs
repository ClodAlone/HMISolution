#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartAppearanceStyles class which provides various appearance styles to the chart.
    /// </summary>
    public sealed class ChartAppearanceStyles
    {
        #region Contants
        /// <summary>
        /// ChartAppearnceStyle is None
        /// </summary>
        public const string NoneFormat = "[NONE]";

        /// <summary>
        /// ChartAppearnceStyle is Black
        /// </summary>
        public const string BlackFormat = "Black";

        /// <summary>
        /// ChartAppearnceStyle is Contrast
        /// </summary>
        public const string ContrastFormat = "Contrast";

        /// <summary>
        /// ChartAppearnceStyle is Default
        /// </summary>
        public const string DefaultFormat = "Default";

        /// <summary>
        /// ChartAppearnceStyle is GainsBoro
        /// </summary>
        public const string GainsBoroFormat = "GainsBoro";

        /// <summary>
        /// ChartAppearnceStyle is Gradient
        /// </summary>
        public const string GradientFormat = "Gradient";

        /// <summary>
        /// ChartAppearnceStyle is LightOlive
        /// </summary>
        public const string LightOliveFormat = "LightOlive";

        /// <summary>
        /// ChartAppearnceStyle is Linen
        /// </summary>
        public const string LinenFormat = "Linen";
        
        /// <summary>
        /// ChartAppearnceStyle is MistyRoseFormat
        /// </summary>
        public const string MistyRoseFormat = "MistyRose";

        /// <summary>
        /// ChartAppearnceStyle is PaleYellow
        /// </summary>
        public const string PaleYellowFormat = "PaleYellow";

        /// <summary>
        /// ChartAppearnceStyle is PinkOverLay
        /// </summary>
        public const string PinkOverlayFormat = "PinkOverlay";

        /// <summary>
        /// ChartAppearnceStyle is SolidColor
        /// </summary>
        public const string SolidColorFormat = "SolidColor";

        /// <summary>
        /// ChartAppearnceStyle is TriColor
        /// </summary>
        public const string TriColorFormat = "TriColor";
        #endregion

        #region Public methods

        /// <summary>
        /// Applies the format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="formatName">Name of the format.</param>
        public static void ApplyFormat(ChartControl chart, string formatName)
        {
            switch (formatName)
            {
                case BlackFormat:
                    ApplyBlackFormat(chart);
                    break;

                case ContrastFormat:
                    ApplyContrastFormat(chart);
                    break;

                case DefaultFormat:
                    ApplyDefaultFormat(chart);
                    break;

                case GainsBoroFormat:
                    ApplyGainsBoroFormat(chart);
                    break;

                case GradientFormat:
                    ApplyGradientFormat(chart);
                    break;

                case LightOliveFormat:
                    ApplyLightOliveFormat(chart);
                    break;

                case LinenFormat:
                    ApplyLinenFormat(chart);
                    break;

                case MistyRoseFormat:
                    ApplyMistyRoseFormat(chart);
                    break;

                case PaleYellowFormat:
                    ApplyPaleYellowFormat(chart);
                    break;

                case PinkOverlayFormat:
                    ApplyPinkOverlayFormat(chart);
                    break;

                case SolidColorFormat:
                    ApplySolidColorFormat(chart);
                    break;

                case TriColorFormat:
                    ApplyTriColorFormat(chart);
                    break;

                case NoneFormat:
                    ApplyNoneFormat(chart);
                    break;

                default:
                    throw new ArgumentException(chart.Localization.UnknownAppearanceStyleException);
                ////break;
            }

            // Dafault settings.
            foreach (ChartLegend legend in chart.Legends)
            {
                legend.BackInterior = new BrushInfo(Color.Transparent);
            }
        }

        /// <summary>
        /// Applies the classic style.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public static void ApplyClassicStyle(ChartControl chart)
        {
            ApplyNoneFormat(chart);

            RevertPropertyValue(chart.Legend, "BackInterior", new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 209, 226), Color.FromArgb(227, 232, 243)));
            RevertPropertyValue(chart.Legend, "Alignment", ChartAlignment.Center);
            RevertPropertyValue(chart.Legend, "ShowBorder", true);
            RevertPropertyValue(chart.Legend, "Font", null);

            RevertPropertyValue(chart.Title, "Font", new Font("Arial", 18));

            foreach (ChartAxis axis in chart.Axes)
            {
                RevertPropertyValue(axis, "Font", null);
            }

            RevertPropertyValue(chart, "LegendsPlacement", ChartPlacement.Outside);
            RevertPropertyValue(chart, "SmoothingMode", SmoothingMode.AntiAlias);
            RevertPropertyValue(chart, "Spacing", 10);
            RevertPropertyValue(chart, "BackInterior", new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 209, 226), Color.FromArgb(227, 232, 243)));
            RevertPropertyValue(chart, "ChartInterior", new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 209, 226), Color.FromArgb(227, 232, 243)));
            RevertPropertyValue(chart, "AllowGradientPalette", true);

            RevertPropertyValue(chart.ChartArea, "BackInterior", null);

            if (chart.PrimaryYAxis.ForceZero == true)
            {
                chart.PrimaryYAxis.ForceZero = false;
            }

            if (chart.DockingManager.DockAlignment == true)
            {
                chart.DockingManager.DockAlignment = false;
            }
        }

        /// <summary>
        /// Reverts the default style.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public static void RevertDefaultStyle(ChartControl chart)
        {
            ApplyNoneFormat(chart);

            ResetPropertyValue(chart.Legend, "BackInterior");
            ResetPropertyValue(chart.Legend, "Alignment");
            ResetPropertyValue(chart.Legend, "ShowBorder");
            ResetPropertyValue(chart.Legend, "Font");

            ResetPropertyValue(chart.Title, "Font");

            foreach (ChartAxis axis in chart.Axes)
            {
                ResetPropertyValue(axis, "Font");
            }

            ResetPropertyValue(chart, "LegendsPlacement");
            ResetPropertyValue(chart, "SmoothingMode");
            ResetPropertyValue(chart, "Spacing");
            ResetPropertyValue(chart, "BackInterior");
            ResetPropertyValue(chart, "ChartInterior");
            ResetPropertyValue(chart, "AllowGradientPalette");

            ResetPropertyValue(chart.ChartArea, "BackInterior");

            if (chart.PrimaryYAxis.ForceZero == false)
            {
                chart.PrimaryYAxis.ForceZero = true;
            }

            if (chart.DockingManager.DockAlignment == false)
            {
                chart.DockingManager.DockAlignment = true;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Resets the format.
        /// </summary>
        private static void ApplyNoneFormat(ChartControl chart)
        {
            ChartAppearanceStyles.ResetPropertyValue(chart, "BackInterior");
            ChartAppearanceStyles.ResetPropertyValue(chart.ChartArea, "BackInterior");
            ChartAppearanceStyles.ResetPropertyValue(chart.ChartArea, "GridBackInterior");
            
            ChartAppearanceStyles.ResetPropertyValue(chart, "Palette");
            ChartAppearanceStyles.ResetPropertyValue(chart, "ForeColor");

            foreach (ChartAxis axis in chart.Axes)
            {
                ChartAppearanceStyles.ResetPropertyValue(axis.GridLineType, "ForeColor");
                ChartAppearanceStyles.ResetPropertyValue(axis.LineType, "ForeColor");
                ChartAppearanceStyles.ResetPropertyValue(axis, "TickColor");
            }
        }

        /// <summary>
        /// Applies the black format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyBlackFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(ColorFromRGB(0x2C2929));
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[] {ColorFromRGB(0xEE9022), ColorFromRGB(0x6BBE52), ColorFromRGB(0xE8DE25), ColorFromRGB(0x9E4299), ColorFromRGB(0x8C5A24), ColorFromRGB(0xDD4826)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0xB6B6B7);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.LineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the contrast format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyContrastFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ColorFromRGB(0x000838), ColorFromRGB(0x0099DB) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[] { ColorFromRGB(0xEE9022), ColorFromRGB(0x6BBE52), ColorFromRGB(0xE8DE25), ColorFromRGB(0x9E4299), ColorFromRGB(0x8C5A24), ColorFromRGB(0xDD4826)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0xFFFFFF);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.LineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the default format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyDefaultFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ColorFromRGB(0xDBDBDB), ColorFromRGB(0xC0E5E1) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(ColorFromRGB(0xD1D4EB));

            chart.CustomPalette = new Color[] { ColorFromRGB(0xB0A433), ColorFromRGB(0x887DBB), ColorFromRGB(0x1F6030), ColorFromRGB(0xCF5336), ColorFromRGB(0x2B296A), ColorFromRGB(0x8A6B42)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0x2B3085);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.LineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the gains boro format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyGainsBoroFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Horizontal, new Color[] { ColorFromRGB(0xDDDCDC), ColorFromRGB(0xFAF1E7), ColorFromRGB(0xDDDCDC) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ ColorFromRGB(0xE64599), ColorFromRGB(0xEE2C24), ColorFromRGB(0x4258A7), ColorFromRGB(0x275F2F), ColorFromRGB(0xCB531B), ColorFromRGB(0xBF1F40)};
            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0x656767);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xA99ECD);
                axis.LineType.ForeColor = ColorFromRGB(0xA99ECD);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the gradient format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyGradientFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ColorFromRGB(0x000000), ColorFromRGB(0xD0D0D0), ColorFromRGB(0x6A6A6A) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ColorFromRGB(0xEE9022), ColorFromRGB(0x6BBE52), ColorFromRGB(0xE8DE25), ColorFromRGB(0x9E4299), ColorFromRGB(0x8C5A24), ColorFromRGB(0xDD4826)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0xFFFFFF);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.LineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the light olive format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyLightOliveFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Horizontal,
                new Color[] { ColorFromRGB(0xD2DABE), ColorFromRGB(0xFFFFFF), ColorFromRGB(0xD2DABE) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ColorFromRGB(0x5E3780), ColorFromRGB(0x6BBE52), ColorFromRGB(0x0699BF), ColorFromRGB(0x9C247E), ColorFromRGB(0x8B3F39), ColorFromRGB(0x777C9B)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0x437D3B);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xA99ECD);
                axis.LineType.ForeColor = ColorFromRGB(0xA99ECD);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the linen format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyLinenFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Horizontal, new Color[] { ColorFromRGB(0xDAEEE5), ColorFromRGB(0xE8EEE6), ColorFromRGB(0xE8EEE6), ColorFromRGB(0xDAEEE5) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ ColorFromRGB(0x722B90), ColorFromRGB(0x994E65), ColorFromRGB(0x8E5F46), ColorFromRGB(0x756E7D), ColorFromRGB(0xCEB267), ColorFromRGB(0x485E7B)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0x437D3B);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xA99ECD);
                axis.LineType.ForeColor = ColorFromRGB(0xA99ECD);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the misty rose format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyMistyRoseFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ColorFromRGB(0xFFEED9), ColorFromRGB(0xFEDAC1) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ColorFromRGB(0x95A985), ColorFromRGB(0x636260), ColorFromRGB(0x865099), ColorFromRGB(0xA4A0AE), ColorFromRGB(0x73C48F), ColorFromRGB(0x83719C)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0xAF888B);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.LineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the pale yellow format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyPaleYellowFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ColorFromRGB(0xFFEABF), ColorFromRGB(0xDBDBDB), ColorFromRGB(0xFFEABF) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ ColorFromRGB(0xC971A6), ColorFromRGB(0xA82836), ColorFromRGB(0x4C5171), ColorFromRGB(0x542925), ColorFromRGB(0xA29836), ColorFromRGB(0x664558)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0x2C3084);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xA89ECD);
                axis.LineType.ForeColor = ColorFromRGB(0xA89ECD);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the pink overlay format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyPinkOverlayFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ColorFromRGB(0xD6D5D8), ColorFromRGB(0xE9C2DC) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ ColorFromRGB(0x547E78), ColorFromRGB(0xDE1D3B), ColorFromRGB(0xFED206), ColorFromRGB(0x7B2165), ColorFromRGB(0x32132A), ColorFromRGB(0x4B5147)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0x917388);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.LineType.ForeColor = ColorFromRGB(0xFFFFFF);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the solid color format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplySolidColorFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(ColorFromRGB(0x60675F));
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ColorFromRGB(0xE0C4DF), ColorFromRGB(0x6BBE52), ColorFromRGB(0x5DCAE9), ColorFromRGB(0x4E3716), ColorFromRGB(0x93B06C), ColorFromRGB(0x26B890)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0xB6B6B7);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.LineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Applies the tri color format.
        /// </summary>
        /// <param name="chart">The chart.</param>
        private static void ApplyTriColorFormat(ChartControl chart)
        {
            chart.BackInterior = new BrushInfo(GradientStyle.Horizontal,
                new Color[] { ColorFromRGB(0xFAB9A0), ColorFromRGB(0xFFFFFF), ColorFromRGB(0xF8BDD3) });
            chart.ChartArea.BackInterior = new BrushInfo(Color.Transparent);
            chart.ChartArea.GridBackInterior = new BrushInfo(Color.Transparent);

            chart.CustomPalette = new Color[]{ColorFromRGB(0xE0C4DF), ColorFromRGB(0x60485A), ColorFromRGB(0x485E7B), ColorFromRGB(0xA64F9E), ColorFromRGB(0xACC382), ColorFromRGB(0xDD6126)};

            chart.Palette = ChartColorPalette.Custom;
            chart.ForeColor = ColorFromRGB(0xA99ECD);

            foreach (ChartAxis axis in chart.Axes)
            {
                axis.GridLineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.LineType.ForeColor = ColorFromRGB(0xB6B6B7);
                axis.TickColor = axis.LineType.ForeColor;
            }
        }

        /// <summary>
        /// Resets the property value.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="propertyName">Name of the property.</param>
        private static void ResetPropertyValue(object component, string propertyName)
        {
            PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(component)[propertyName];

            if (propDescriptor != null)
            {
                propDescriptor.ResetValue(component);
            }
        }

        /// <summary>
        /// Reverts the property value.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        private static void RevertPropertyValue(object component, string propertyName, object value)
        {
            PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(component)[propertyName];

            if (!propDescriptor.ShouldSerializeValue(component))
            {
                propDescriptor.SetValue(component, value);
            }
        }

        /// <summary>
        /// Colors from RGB.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static Color ColorFromRGB(int value)
        {
            return Color.FromArgb(255, Color.FromArgb(value));
        }
        #endregion
    }
}
