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

#region file using directives
using System;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The class WordColor implements routines working with Color.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WordColor
    {
        #region Constants
        internal const byte MaxRGB = 255;
        internal const int MaxHue = 360;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private static readonly int[] WordKnownColors =
      {
        0, -16777216, -16776961, -16711681, -16744448, -65281, -65536, -256, -1
        , -16777077, -16741493, -16751616, -7667573, -7667712, -256
        //// hot fix: for automatic color
        ////, -5658199, -2894893
      };

        /// <summary>
        /// 
        /// </summary>
        internal static readonly uint[] ArgbArray =
    { 
      0xff000000, ConvertColorToRGB(Color.Black), ConvertColorToRGB(Color.Blue), ConvertColorToRGB(Color.Cyan)
      , ConvertColorToRGB(Color.Green), ConvertColorToRGB(Color.Magenta), ConvertColorToRGB(Color.Red)
      , ConvertColorToRGB(Color.Yellow), ConvertColorToRGB(Color.White), ConvertColorToRGB(Color.DarkBlue)
      , ConvertColorToRGB(Color.DarkCyan), ConvertColorToRGB(Color.DarkGreen), ConvertColorToRGB(Color.DarkMagenta)
      , ConvertColorToRGB(Color.DarkRed), ConvertColorToRGB(Color.Gold)
      , 0x808080, ConvertColorToRGB(Color.LightGray)
    };

        /// <summary>
        /// 
        /// </summary>
        internal static readonly Color[] ColorsArray = 
    {
      Color.Empty, Color.Black, Color.Blue, Color.Cyan
      , Color.Green, Color.Magenta, Color.Red
      , Color.Yellow, Color.White, Color.DarkBlue
      , Color.DarkCyan, Color.DarkGreen, Color.DarkMagenta
      , Color.DarkRed, Color.Gold
      , Color.FromArgb(( int )0x808080 ),Color.LightGray
    };

        /// <summary>
        /// 
        /// </summary>
        private Color m_color;

        /// <summary>
        /// 
        /// </summary>
        private byte m_colorId;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the a null color.
        /// </summary>
        public static WordColor Empty
        {
            get
            {
                return new WordColor(0);
            }
        }

        /// <summary>
        ///  Gets Black color.
        /// </summary>
        public static WordColor Black
        {
            get
            {
                return new WordColor(1);
            }
        }

        /// <summary>
        /// Gets Blue color.
        /// </summary>
        public static WordColor Blue
        {
            get
            {
                return new WordColor(2);
            }
        }

        /// <summary>
        ///  Gets Cyan color.
        /// </summary>
        public static WordColor Cyan
        {
            get
            {
                return new WordColor(3);
            }
        }

        /// <summary>
        ///  Gets Green color.
        /// </summary>
        public static WordColor Green
        {
            get
            {
                return new WordColor(4);
            }
        }

        /// <summary>
        ///  Gets Magenta color.
        /// </summary>
        public static WordColor Magenta
        {
            get
            {
                return new WordColor(5);
            }
        }

        /// <summary>
        /// Gets Red color.
        /// </summary>
        public static WordColor Red
        {
            get
            {
                return new WordColor(6);
            }
        }

        /// <summary>
        /// Gets Yellow color.
        /// </summary>
        public static WordColor Yellow
        {
            get
            {
                return new WordColor(7);
            }
        }

        /// <summary>
        /// Gets White color.
        /// </summary>
        public static WordColor White
        {
            get
            {
                return new WordColor(8);
            }
        }

        /// <summary>
        /// Gets DarkBlue color.
        /// </summary>
        public static WordColor DarkBlue
        {
            get
            {
                return new WordColor(9);
            }
        }

        /// <summary>
        /// Gets DarkCyan color.
        /// </summary>
        public static WordColor DarkCyan
        {
            get
            {
                return new WordColor(10);
            }
        }

        /// <summary>
        /// Gets DarkGreen color.
        /// </summary>
        public static WordColor DarkGreen
        {
            get
            {
                return new WordColor(11);
            }
        }

        /// <summary>
        /// Gets DarkMagenta color.
        /// </summary>
        public static WordColor DarkMagenta
        {
            get
            {
                return new WordColor(12);
            }
        }

        /// <summary>
        ///  Gets DarkRed color.
        /// </summary>
        public static WordColor DarkRed
        {
            get
            {
                return new WordColor(13);
            }
        }

        /// <summary>
        ///  Gets DarkYellow color.
        /// </summary>
        public static WordColor DarkYellow
        {
            get
            {
                return new WordColor(14);
            }
        }

        /// <summary>
        ///  Gets DarkGray color.
        /// </summary>
        public static WordColor DarkGray
        {
            get
            {
                return new WordColor(15);
            }
        }

        /// <summary>
        ///  Gets LightGray color.
        /// </summary>
        public static WordColor LightGray
        {
            get
            {
                return new WordColor(16);
            }
        }

        /// <summary>
        ///  Gets the system Color.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_color;
            }
        }

        /// <summary>
        /// Gets the byte value for a Color.
        /// </summary>
        public byte ColorId
        {
            get
            {
                return m_colorId;
            }
        }

        /// <summary>
        /// Gets RGB Color.
        /// </summary>
        public int RGB
        {
            get
            {
                return WordKnownColors[m_colorId];
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordColor"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public WordColor(byte index)
        {
            m_color = Color.FromArgb(WordKnownColors[index]);
            m_colorId = index;
        }
        #endregion

        #region Class utilities
        /// <summary>
        /// Converts the HSL to RGB.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="luminance">The luminance.</param>
        /// <returns></returns>
        internal static uint ConvertHSLToRGB(double hue, double saturation, double luminance)
        {
            return ConvertColorToRGB(ConvertHSLToColor(hue, saturation, luminance), false);
        }
        /// <summary>
        /// Converts the color by shade.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="shade">The shade.</param>
        /// <returns></returns>
        internal static Color ConvertColorByShade(Color color, double shade)
        {
            //This algorithm returns result slightly different from MS Word.
            //TODO: revisit and improve
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            luminance = luminance * shade;
            return ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts the color by tint.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="tint">The tint.</param>
        /// <returns></returns>
        internal static Color ConvertColorByTint(Color color, double tint)
        {
            //This algorithm returns result slightly different from MS Word.
            //TODO: revisit and improve
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            luminance = luminance * tint + (1 - tint);
            return ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on modulation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="percent">The percent.</param>
        /// <returns></returns>
        internal static byte ConvertbyModulation(byte value, double percent)
        {
            value = (byte)(value * (percent / DLSConstants.HundredthsUnit));
            return value;
        }
        /// <summary>
        /// Converts based on offset.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="percent">The percent.</param>
        /// <returns></returns>
        internal static byte ConvertbyOffset(byte value, double percent)
        {
            value += (byte)(MaxRGB * (percent / DLSConstants.HundredthsUnit));
            return value;
        }
        /// <summary>
        /// Converts based on hue.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="angle">The angle.</param>
        internal static void ConvertbyHue(ref Color color, double angle)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            hue = angle / MaxHue;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on hue mod.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="ratio">The ratio.</param>
        internal static void ConvertbyHueMod(ref Color color, double ratio)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            hue = hue * ratio;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on hue offset.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="angle">The angle.</param>
        internal static void ConvertbyHueOffset(ref Color color, double angle)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            hue += angle / MaxHue;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on lum.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percent">The percent.</param>
        internal static void ConvertbyLum(ref Color color, double percent)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            luminance = percent / DLSConstants.HundredthsUnit;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on lum mod.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percent">The percent.</param>
        internal static void ConvertbyLumMod(ref Color color, double percent)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            luminance = luminance * (percent / DLSConstants.HundredthsUnit);
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on lum offset.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percent">The percent.</param>
        internal static void ConvertbyLumOffset(ref Color color, double percent)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            luminance += percent / DLSConstants.HundredthsUnit;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on sat.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percent">The percent.</param>
        internal static void ConvertbySat(ref Color color, double percent)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            saturation = percent / DLSConstants.HundredthsUnit;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on sat mod.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percent">The percent.</param>
        internal static void ConvertbySatMod(ref Color color, double percent)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            saturation = saturation * (percent / DLSConstants.HundredthsUnit);
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Converts based on sat offset.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percent">The percent.</param>
        internal static void ConvertbySatOffset(ref Color color, double percent)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            saturation += percent / DLSConstants.HundredthsUnit;
            color = ConvertHSLToColor(hue, saturation, luminance);
        }
        /// <summary>
        /// Complements the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        internal static Color ComplementColor(Color color)
        {
            double hue;
            double luminance;
            double saturation;

            ConvertColortoHSL(color, out hue, out saturation, out luminance);
            // Calculate the opposite hue
            double hue1 = hue + 0.5;
            if (hue1 > 1)
                hue1 -= 1;

            return ConvertHSLToColor(hue1, saturation, luminance);
        }
        /// <summary>
        /// Inverses the color of the gamma.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        internal static Color InverseGammaColor(Color color)
        {
            double red = (double)color.R / (double)MaxRGB;
            double green = (double)color.G / (double)MaxRGB;
            double blue = (double)color.B / (double)MaxRGB;

            red = Math.Round(ConvertsRGBtoLinearRGB(red) * MaxRGB);
            green = Math.Round(ConvertsRGBtoLinearRGB(green) * MaxRGB);
            blue = Math.Round(ConvertsRGBtoLinearRGB(blue) * MaxRGB);

            return Color.FromArgb(MaxRGB, (byte)red, (byte)green, (byte)blue);
        }
        /// <summary>
        /// Converts the RGB to linear RGB.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static double ConvertsRGBtoLinearRGB(double value)
        {
            if (value < 0.0)
                return 0.0;
            if (value <= 0.04045)
                return value / 12.92;
            if (value <= 1.0)
                return Math.Pow(((value + 0.055) / 1.055), 2.4);
            return 1.0;
        }
        /// <summary>
        /// Gammas the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        internal static Color GammaColor(Color color)
        {
            double red = (double)color.R / (double)MaxRGB;
            double green = (double)color.G / (double)MaxRGB;
            double blue = (double)color.B / (double)MaxRGB;

            red = Math.Round(ConvertsLinearRGBtoRGB(red) * MaxRGB);
            green = Math.Round(ConvertsLinearRGBtoRGB(green) * MaxRGB);
            blue = Math.Round(ConvertsLinearRGBtoRGB(blue) * MaxRGB);

            return Color.FromArgb(MaxRGB, (byte)red, (byte)green, (byte)blue);
        }
        /// <summary>
        /// Converts the linear RGB to RGB.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static double ConvertsLinearRGBtoRGB(double value)
        {
            if (value < 0.0)
                return 0.0;
            if (value <= 0.0031308)
                return value * 12.92;
            if (value <= 1.0)
                return 1.055 * Math.Pow(value, (1.0 / 2.4)) - 0.055;
            return 1.0;
        }
        /// <summary>
        /// Grays the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        internal static Color GrayColor(Color color)
        {
            double value = Math.Round((double)color.R * 0.2126 + (double)color.G * 0.7152 + (double)color.B * 0.0722);
            return Color.FromArgb(MaxRGB, (byte)value, (byte)value, (byte)value);
        }
        /// <summary>
        /// Inverses the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Color InverseColor(Color color)
        {
            byte red = (byte)(color.R ^ MaxRGB);
            byte green = (byte)(color.G ^ MaxRGB);
            byte blue = (byte)(color.B ^ MaxRGB);
            return Color.FromArgb(MaxRGB, (byte)red, (byte)green, (byte)blue);
        }
        /// <summary>
        /// Converts the colorto HSL.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="luminance">The luminance.</param>
        internal static void ConvertColortoHSL(Color color, out double hue, out double saturation, out double luminance)
        {
            hue = 0;
            luminance = 0;
            saturation = 0;

            double red = (double)color.R / (double)MaxRGB;
            double green = (double)color.G / (double)MaxRGB;
            double blue = (double)color.B / (double)MaxRGB;

            double minVal = Math.Min(red, Math.Min(green, blue));
            double maxVal = Math.Max(red, Math.Max(green, blue));
            double minMaxDiff = maxVal - minVal;
            double minMaxSum = maxVal + minVal;

            luminance = (maxVal + minVal) / 2;

            if (maxVal == minVal)
            {
                saturation = 0;
                hue = 0;
            }
            else
            {
                if (luminance < 0.5)
                    saturation = minMaxDiff / minMaxSum;
                else
                    saturation = minMaxDiff / (2 - minMaxSum);

                if (red == maxVal)
                    hue = (1.0 / 6.0) * (green - blue) / minMaxDiff - (blue > green ? 1 : 0);
                else if (green == maxVal)
                    hue = (1.0 / 6.0) * (blue - red) / minMaxDiff + (1.0 / 3.0);
                else if (blue == maxVal)
                    hue = (1.0 / 6.0) * (red - green) / minMaxDiff + (2.0 / 3.0);

                if (hue < 0)
                    hue += 1;

                if (hue > 1)
                    hue -= 1;
            }

            if (saturation < 0)
                saturation = 0;

            if (saturation > 1)
                saturation = 1;

            if (luminance < 0)
                luminance = 0;

            if (luminance > 1)
                luminance = 1;
        }
        /// <summary>
        /// Converts the color of the HSL to.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="luminance">The luminance.</param>
        /// <returns></returns>
        internal static Color ConvertHSLToColor(double hue, double saturation, double luminance)
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            if (saturation == 0)
            {
                blue = (int)Math.Round(luminance * MaxRGB);
                red = blue;
                green = blue;
            }
            else
            {
                double var1, var2;
                if (luminance < 0.5)
                    var2 = luminance * (1 + saturation);
                else
                    var2 = luminance + saturation - (luminance * saturation);
                var1 = 2 * luminance - var2;
                double hue1 = hue > (2.0 / 3.0)? (hue - 2.0 / 3.0) : (hue + 1.0 / 3.0);
                red = (int)Math.Round(MaxRGB * HueToRGB(var1, var2, hue1));
                green = (int)Math.Round(MaxRGB * HueToRGB(var1, var2, hue));
                hue1 = hue < (1.0 / 3.0) ? (hue + 2.0 / 3.0) : (hue - 1.0 / 3.0);
                blue = (int)Math.Round(MaxRGB * HueToRGB(var1, var2, hue1));
            }
            if (red < 0)
                red = 0;

            if (green < 0)
                green = 0;

            if (blue < 0)
                blue = 0;

            if (red > MaxRGB)
                red = MaxRGB;

            if (green > MaxRGB)
                green = MaxRGB;

            if (blue > MaxRGB)
                blue = MaxRGB;

            return Color.FromArgb(MaxRGB, (byte)red, (byte)green, (byte)blue);
        }
        /// <summary>
        /// Hues to RGB.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <param name="hue">The hue.</param>
        /// <returns></returns>
        internal static double HueToRGB(double n1, double n2, double hue)
        {
            double result;
            if (hue < 0)
                hue += 1;
            if (hue > 1)
                hue -= 1;

            if (6 * hue < 1)
                result = n1 + ((n2 - n1) * 6 * hue);
            else if (2 * hue < 1)
                result = n2;
            else if (3 * hue < 2)
                result = n1 + (n2 - n1) * ((2.0 / 3.0) - hue) * 6;
            else
                result = n1;
            return result;
        }
        /// <summary>
        /// Returns the Color value for the specified color ID.
        /// </summary>
        /// <param name="wordColorId">Specifies color ID.</param>
        /// <returns>Returns the Color value.</returns>
        public static Color IdToColor(int wordColorId)
        {
            if (wordColorId < 0 || wordColorId > WordKnownColors.Length - 1)
            {
                return Color.Empty;
            }

            return Color.FromArgb(WordKnownColors[wordColorId]);
        }

        /// <summary>
        /// Converts the Color value as an integer value.
        /// </summary>
        /// <param name="color">Specifies Color value.</param>
        /// <returns>Returns the corresponding ID value of the color.</returns>
        public static int ColorToId(Color color)
        {
            int argb = color.ToArgb();
            for (int i = 0; i < WordKnownColors.Length; i++)
            {
                if (WordKnownColors[i] == argb)
                    return i;
            }

            return ReduceColor(color);
        }

        /// <summary>
        /// Converts the color to RGB.
        /// </summary>
        /// <param name="color">Specifies the Color value.</param>
        /// <returns>Returns the converted RGB value.</returns>
        public static uint ConvertColorToRGB(Color color)
        {
            return ConvertColorToRGB(color, false);
        }

        /// <summary>
        /// Converts the color to RGB.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="ignoreAlpha">If set to <c>true</c> ignores alpha channel.</param>
        /// <returns>Returns the converted RGB value.</returns>
        public static uint ConvertColorToRGB(Color color, bool ignoreAlpha)
        {
            uint rgb = 0;
            rgb |= color.R;
            rgb |= ((uint)(color.G << 8));
            rgb |= ((uint)(color.B << 16));
            if (ignoreAlpha)
            {
                return rgb;
            }
            else
            {
                return (rgb | ((uint)(~color.A << 24)));
            }
        }

        /// <summary>
        /// Converts the specified ID value as RGB value.
        /// </summary>
        /// <param name="colorId">Color identification value.</param>
        /// <returns>Returns RGB value.</returns>
        public static uint ConvertIdToRGB(int colorId)
        {
            if (colorId < ArgbArray.Length)
            {
                return (uint)ArgbArray[colorId];
            }

            return 0xff000000;
        }

        /// <summary>
        ///  Converts RGB value as int value.
        /// </summary>
        /// <param name="rgb">Specifies RGB value.</param>
        /// <returns>Returns the ID value for a specified RGB.</returns>
        public static int ConvertRGBToId(uint rgb)
        {
            if (rgb == 0xff000000)
            {
                return 0;
            }

            double max = double.PositiveInfinity;
            int colorId = 1;
            for (int i = 0; i < ArgbArray.Length; i++)
            {
                double similar = FindSimilarColor(ArgbArray[i], rgb);
                if (similar <= max)
                {
                    colorId = i;
                    max = similar;
                }
            }

            return colorId;
        }

        /// <summary>
        /// Converts the RGB as Color value.
        /// </summary>
        /// <param name="rgb">RGB Value.</param>
        /// <returns>Returns Color value.</returns>
        public static Color ConvertRGBToColor(uint rgb)
        {
            if (rgb != 0xff000000)
            {
                byte r = (byte)(rgb & 0xff);
                byte g = (byte)((rgb & 0xff00) >> 8);
                byte b = (byte)((rgb & 0xff0000) >> 0x10);
                byte a = (byte)~((rgb & 0xff000000) >> 0x18);
                return Color.FromArgb(a, r, g, b);
            }

            // auto color
            return Color.Empty;
        }

        /// <summary>
        /// Converts the Color as a color ID.
        /// </summary>
        /// <param name="color">Specifies Color value.</param>
        /// <returns>Returns color ID.</returns>
        public static int ConvertColorToId(Color color)
        {
            ////      return ConvertRGBToId( ( uint )color.ToArgb() );
            return ConvertRGBToId(ConvertColorToRGB(color));
        }

        /// <summary>
        /// Gets the Color value for a specified ID.
        /// </summary>
        /// <param name="id">Specifies Color ID value.</param>
        /// <returns>Returns Color value/.</returns>
        public static Color ConvertIdToColor(int id)
        {
            return ConvertRGBToColor(ConvertIdToRGB(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static int ReduceColor(Color color)
        {
            int lastMinDiff = int.MaxValue;
            int resId = 0;

            for (int i = 0; i < WordKnownColors.Length; i++)
            {
                Color iColor = Color.FromArgb(WordKnownColors[i]);

                int redDiff = Math.Abs(iColor.R - color.R);
                int blueDiff = Math.Abs(iColor.B - color.B);
                int greenDiff = Math.Abs(iColor.G - color.G);

                int rgbDiff = redDiff + blueDiff + greenDiff;

                if (rgbDiff < lastMinDiff)
                {
                    resId = i;
                    lastMinDiff = rgbDiff;
                }
            }

            return resId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordColor"></param>
        /// <param name="rgbColor"></param>
        /// <returns></returns>
        private static double FindSimilarColor(uint wordColor, uint rgbColor)
        {
            byte iR = (byte)(wordColor & 0xff);
            byte iG = (byte)((wordColor & 0xff00) >> 8);
            byte iB = (byte)((wordColor & 0xff0000) >> 0x10);
            byte R = (byte)(rgbColor & 0xff);
            byte G = (byte)((rgbColor & 0xff00) >> 8);
            byte B = (byte)((rgbColor & 0xff0000) >> 0x10);
            return (double)((Math.Abs((int)(iR - R)) + Math.Abs((int)(iG - G))) + Math.Abs((int)(iB - B)));
        }

        #endregion
    }
}