#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.WP.Controls
{
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls
{
#else
#if WPF
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls
{
#else
using Windows.UI;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Controls
{
#endif
#endif
#endif
    /// <summary>
    /// Represents a helping class for the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.SfColorPicker"/> control.
    /// </summary>
    public static class ColorPickerHelper
    {
        /// <summary>
        /// Gets the ARGB values from HSV
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="value"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color FromHsv(double hue, double saturation, double value, double alpha = 1.0)
        {
            double chroma = value * saturation;
            double h1 = hue / 60;
            double x = chroma * (1 - Math.Abs(h1 % 2 - 1));
            double m = value - chroma;
            double r1, g1, b1;

            if (h1 < 1)
            {
                r1 = chroma;
                g1 = x;
                b1 = 0;
            }
            else if (h1 < 2)
            {
                r1 = x;
                g1 = chroma;
                b1 = 0;
            }
            else if (h1 < 3)
            {
                r1 = 0;
                g1 = chroma;
                b1 = x;
            }
            else if (h1 < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = chroma;
            }
            else if (h1 < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = chroma;
            }
            else
            {
                r1 = chroma;
                g1 = 0;
                b1 = x;
            }
            byte r, g, b;
            r = (byte)(255 * (r1 + m));
            g = (byte)(255 * (g1 + m));
            b = (byte)(255 * (b1 + m));

            byte a = (byte)(255 * alpha);

            return Color.FromArgb(a, r, g, b);
        } 

        /// <summary>
        /// Gets the color from the input bytes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int IntColorFromBytes(byte a, byte r, byte g, byte b)
        {
            var col =
                a << 24
                | r << 16
                | g << 8
                | b;
            return col;
        }

        internal static void RenderSaturationValueCore(double hue, int ph, int hw, double invPh, int pw, PixelBufferInfo pixels)
        {
            for (int y = 0; y < ph; y++)
            {
                double value = 1 - 1.0 * (ph - 1 - y) / ph;

                var xmin = (int)(hw * (1 - invPh * y));
                var xmax = pw - xmin;

                for (int x = xmin; x < xmax; x++)
                {
                    var saturation = 1 - (double)(x + xmax - pw) / pw;
                    var c = ColorPickerHelper.FromHsv(hue, saturation, value);
                    pixels[pw * y + x] = c.AsInt();
                }
            }
        }

        /// <summary>
        /// Update all the color values
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ph"></param>
        /// <param name="pw"></param>
        /// <param name="invPh"></param>
        /// <returns></returns>
        public static Color UpdateColorValue(double hue, double x, double y,double ph,double pw, double invPh)
        {
                double value = 1 - 1.0 * (ph - 1 - y) / ph;
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                var xmin = (int)((1 - invPh * y));
                var xmax = pw - xmin;
                var saturation = 1 - (double)(x + xmax - pw) / pw;
                if (saturation > 1)
                    saturation = 1;
                else if (saturation < 0)
                    saturation = 0;
                return ColorPickerHelper.FromHsv(hue, saturation,value);
        }

        /// <summary>
        /// Gets the canvas position
        /// </summary>
        /// <param name="hsv"></param>
        /// <param name="ph"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public static Point GetCanvasPosition(this HsvColor hsv,double ph,double pw)
        {
            double x, y, xmin, xmax, invph;
            invph = 1.0 / ph;
            y = (((hsv.V * ph) - ph) / 1.0) + ph - 1;
            xmin = ((1 - invph * y));
            xmax = pw - xmin;
            x= (2 * pw)-xmax- (hsv.S * pw);
            return new Point(x,y);
        }

        /// <summary>
        /// Returns the A, RGB values
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color FromHsl(double hue, double saturation, double lightness, double alpha = 1.0)
        {
            double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double h1 = hue / 60;
            double x = chroma * (1 - Math.Abs(h1 % 2 - 1));
            double m = lightness - 0.5 * chroma;
            double r1, g1, b1;

            if (h1 < 1)
            {
                r1 = chroma;
                g1 = x;
                b1 = 0;
            }
            else if (h1 < 2)
            {
                r1 = x;
                g1 = chroma;
                b1 = 0;
            }
            else if (h1 < 3)
            {
                r1 = 0;
                g1 = chroma;
                b1 = x;
            }
            else if (h1 < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = chroma;
            }
            else if (h1 < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = chroma;
            }
            else //if (h1 < 6)
            {
                r1 = chroma;
                g1 = 0;
                b1 = x;
            }

            byte r = (byte)(255 * (r1 + m));
            byte g = (byte)(255 * (g1 + m));
            byte b = (byte)(255 * (b1 + m));
            byte a = (byte)(255 * alpha);

            return Color.FromArgb(a, r, g, b);
        }

    }
}
