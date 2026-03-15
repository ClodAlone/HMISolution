#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#if WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows.Media;

namespace Syncfusion.WP.Controls
{
#else
#if SILVERLIGHT
using System.Windows.Media;
namespace Syncfusion.Tools.Controls
{
#else
#if WPF
using System.Windows.Media;
namespace Syncfusion.Windows.Controls
{
#else
using Windows.UI;

namespace Syncfusion.UI.Xaml.Controls
{
#endif
#endif
#endif
    /// <summary>
    /// Represents a class for the color extension
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// Gets the color as an integer
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int AsInt(this Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                      | ((byte)((color.R * a) >> 8) << 16)
                      | ((byte)((color.G * a) >> 8) << 8)
                      | ((byte)((color.B * a) >> 8));
            return col;
        }

        /// <summary>
        /// Gets pixel value from the color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Pixel GetPixel(this Color color)
        {
            return new Pixel(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Converts the RGBA color to HSV
        /// </summary>
        /// <param name="rgba"></param>
        /// <returns></returns>
        public static HsvColor ToHsv(this Color rgba)
        {
            const double toDouble = 1.0 / 255;
            var r = toDouble * rgba.R;
            var g = toDouble * rgba.G;
            var b = toDouble * rgba.B;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chroma = max - min;
            double saturation = 0;
            double h1;

            if (max == 0)
            {
                saturation = 0;
            }
            else
            {
                saturation = chroma / max;
            }

            if (chroma == 0)
            {
                h1 = 0;
            }
            else if (max == r)
            {
                h1 = ((g - b) / chroma) % 6;
            }
            else if (max == g)
            {
                h1 = 2 + (b - r) / chroma;
            }
            else
            {
                h1 = 4 + (r - g) / chroma;
            }

            double lightness = 0.5 * (max - min);
            HsvColor ret;
            ret.H = 60 * h1;
            ret.S = saturation;
            ret.V = max;
            ret.A = toDouble * rgba.A;
            return ret;
        }
    }
}
