// <copyright file="HsvColor.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Structure describes HSV color.
    /// </summary>
    public struct HsvColor
    {
        /// <summary>
        /// Hue parameter.
        /// </summary>
        public double H;

        /// <summary>
        /// Saturation parameter.
        /// </summary>
        public double S;

        /// <summary>
        /// Brightness parameter.
        /// </summary>
        public double V;

        /// <summary>
        /// Initializes a new instance of the <see cref="HsvColor"/> struct.
        /// </summary>
        /// <param name="h">The hue value.</param>
        /// <param name="s">The saturation.</param>
        /// <param name="v">The brightness.</param>
        public HsvColor(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        /// <summary>
        /// Converts RGB color to HSV color.
        /// </summary>
        /// <param name="r">The R parameter.</param>
        /// <param name="b">The B parameter.</param>
        /// <param name="g">The G parameter.</param>
        /// <returns>
        /// HSV color.
        /// </returns>
        public static HsvColor ConvertRgbToHsv(int r, int b, int g)
        {
            double delta, min;
            double h = 0, s, v;

            min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
            delta = v - min;

            if (v == 0.0)
            {
                s = 0;
            }
            else
            {
                s = delta / v;
            }

            if (s == 0)
            {
                h = 0.0;
            }
            else
            {
                if (r == v)
                {
                    h = (g - b) / delta;
                }
                else if (g == v)
                {
                    h = 2 + (b - r) / delta;
                }
                else if (b == v)
                {
                    h = 4 + (r - g) / delta;
                }

                h *= 60;
                if (h < 0.0)
                {
                    h = h + 360;
                }
            }

            HsvColor hsvColor = new HsvColor();
            hsvColor.H = h;
            hsvColor.S = s;
            hsvColor.V = v / 255;

            return hsvColor;

            ////double hue, sat, val, f, i;
            // int x;
            // x = Math.Min( Math.Min( r, g ), b );
            // val = Math.Max( Math.Max( r, g ), b );
            // if( x == val )
            // {
            // return new HsvColor();
            // }
            // f = ( r == x ) ? g - b : ( ( g == x ) ? b - r : r - g );
            // i = ( r == x ) ? 3 : ( ( g == x ) ? 5 : 1 );
            // hue = Math.Floor( ( i - f / ( val - x ) ) * 60 ) % 360;
            // sat = Math.Floor( ( ( val - x ) / val ) * 100 );
            // val = Math.Floor( val * 100 );
            // HsvColor hsvColor = new HsvColor();
            // hsvColor.H = hue;
            // hsvColor.S = sat;
            // hsvColor.V = val / 255;
            ////return hsvColor;
        }

        /// <summary>
        /// Converts HSV color to RGB color.
        /// </summary>
        /// <param name="h">The H parameter.</param>
        /// <param name="s">The S parameter.</param>
        /// <param name="v">The V parameter.</param>
        /// <returns>
        /// RGB color.
        /// </returns>
        public static Color ConvertHsvToRgb(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (h == 360)
                {
                    h = 0;
                }
                else
                {
                    h = h / 60;
                }

                i = (int)Math.Truncate(h);
                f = h - i;

                p = v * (1.0 - s);
                q = v * (1.0 - (s * f));
                t = v * (1.0 - (s * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }

            return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
