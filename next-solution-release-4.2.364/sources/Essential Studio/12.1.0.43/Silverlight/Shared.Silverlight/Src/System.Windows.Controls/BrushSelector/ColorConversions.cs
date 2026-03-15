#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Color Conversions Class.
    /// </summary>
    internal class ColorConversions
    {
        private const byte MAX = 255;
        private const byte MIN = 0;

        /// <summary>
        /// Converts and returns the Color value for HSV values provided.
        /// </summary>
        /// <param name="hue">hue value of the color</param>
        /// <param name="sat">saturation value of the color</param>
        /// <param name="val">value of the color</param>
        /// <returns>Color value corresponding to the value provided</returns>
        public Color ConvertHSVToRGB(float hue, float sat, float val)
        {
            if (sat > 0)
            {
                if (hue >= 1)
                {
                    hue = 0;
                }

                hue = 6 * hue;
                int hueFloor = (int)Math.Floor(hue);
                double rval = MAX * val;
                double midval = sat * (hue - hueFloor);
                byte a = (byte)Math.Round(rval * (1.0 - sat));
                byte b = (byte)Math.Round(rval * (1.0 - midval));
                byte c = (byte)Math.Round(rval * (1.0 - (sat * (1.0 - (hue - hueFloor)))));
                byte d = (byte)Math.Round(rval);
                switch (hueFloor)
                {
                    case 0: return Color.FromArgb(MAX, d, c, a);
                    case 1: return Color.FromArgb(MAX, b, d, a);
                    case 2: return Color.FromArgb(MAX, a, d, c);
                    case 3: return Color.FromArgb(MAX, a, b, d);
                    case 4: return Color.FromArgb(MAX, c, a, d);
                    case 5: return Color.FromArgb(MAX, d, a, b);
                    default: return Color.FromArgb(0, 0, 0, 0);
                }
            }
            else
            {
                byte d = (byte)(val * MAX);
                return Color.FromArgb(255, d, d, d);
            }
        }

        /// <summary>
        /// Converts the RGB value to HSV value
        /// </summary>
        /// <param name="r">Red channel value of the color</param>
        /// <param name="g">Green channel value of the color</param>
        /// <param name="b">Blue channel value of the color</param>
        /// <returns>PointCollection with HSV values</returns>
        public PointCollection ConvertRGBToHSV(float r, float g, float b)
        {
            r = r / 255;
            g = g / 255;
            b = b / 255;
            float max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            float h = 0, s, v = max;
            float del = max - min;
            int i = 0;
            s = max == 0 ? 0 : del / max;
            if (max == r)
            {
                i = 1;
            }
            else if (max == g)
            {
                i = 2;
            }
            else
            {
                i = 3;
            }

            if (max == min)
            {
                h = 0;
            }
            else
            {
                switch (i)
                {
                    case 1: h = ((g - b) / del) + (g < b ? 6 : 0);
                        break;
                    case 2: h = ((b - r) / del) + 2;
                        break;
                    case 3: h = ((r - g) / del) + 4;
                        break;
                }

                h /= 6;
            }

            PointCollection pc = new PointCollection();
            pc.Add(new Point(s, v));
            pc.Add(new Point(h, 0));
            return pc;
        }
    }
}
