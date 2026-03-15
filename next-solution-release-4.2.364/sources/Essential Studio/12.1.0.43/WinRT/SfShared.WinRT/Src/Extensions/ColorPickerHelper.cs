#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

#if WINDOWS_PHONE
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.WP.Controls
{
#else
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Controls
{
#endif
    public static class ColorPickerHelper
    {
        #region ColorConverters

        public static int AsInt(this Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                      | ((byte)((color.R * a) >> 8) << 16)
                      | ((byte)((color.G * a) >> 8) << 8)
                      | ((byte)((color.B * a) >> 8));
            return col;
        } 

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

            byte r = (byte)(255 * (r1 + m));
            byte g = (byte)(255 * (g1 + m));
            byte b = (byte)(255 * (b1 + m));
            byte a = (byte)(255 * alpha);

            return Color.FromArgb(a, r, g, b);
        } 

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

        public static int IntColorFromBytes(byte a, byte r, byte g, byte b)
        {
            var col =
                a << 24
                | r << 16
                | g << 8
                | b;
            return col;
        }

        #endregion        

        public static void RenderColorPickerSaturationValueTriangle(this WriteableBitmap target, double hue = 0)
        {
            var pw = target.PixelWidth;
            var hw = pw / 2;
            var ph = target.PixelHeight;
            var invPh = 1.0 / ph;
#if WINDOWS_PHONE
            var pixels = target.Pixels.GetPixels();
#else
            var pixels = target.PixelBuffer.GetPixels();
#endif

            RenderColorPickerSaturationValueTriangleCore(hue, ph, hw, invPh, pw, pixels);

            target.Invalidate();
        }

        public static async Task RenderColorPickerSaturationValueTriangleAsync(this WriteableBitmap target, double hue = 0)
        {
            var pw = target.PixelWidth;
            var hw = 0;
            var ph = target.PixelHeight;
            var invPh = 1.0 / ph;
#if WINDOWS_PHONE
            var pixels = target.Pixels.GetPixels();
#else
            var pixels = target.PixelBuffer.GetPixels();
#endif
            await Task.Run(() => RenderColorPickerSaturationValueTriangleCore(hue, ph, 0, invPh, pw, pixels));
            target.Invalidate();
        }

        private static void RenderColorPickerSaturationValueTriangleCore(double hue, int ph, int hw, double invPh, int pw, PixelBufferInfo pixels)
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

        public static Color UpdateColorValue(double hue, double x, double y,double ph,double pw, double invPh)
        {
                double value = 1 - 1.0 * (ph - 1 - y) / ph;
                var xmin = (int)((1 - invPh * y));
                var xmax = pw - xmin;
                var saturation = 1 - (double)(x + xmax - pw) / pw;
                return ColorPickerHelper.FromHsv(hue, saturation,value);
        }

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

        public class PixelBufferInfo
        {
#if WINDOWS_PHONE
            public int[] pixels;
            public int this[int i]
            {
                get
                {
                    return pixels[i];
                }
                set
                {
                    pixels[i] = value;
                }
            }
#else
            private readonly Stream _pixelStream;
            public byte[] Bytes;
            public int this[int i]
            {
                get
                {
                    return ColorPickerHelper.IntColorFromBytes(
                        Bytes[i * 4 + 3],
                        Bytes[i * 4 + 2],
                        Bytes[i * 4 + 1],
                        Bytes[i * 4 + 0]);
                }
                set
                {
                    Bytes[i * 4 + 3] = (byte)((value >> 24) & 0xff);
                    Bytes[i * 4 + 2] = (byte)((value >> 16) & 0xff);
                    Bytes[i * 4 + 1] = (byte)((value >> 8) & 0xff);
                    Bytes[i * 4 + 0] = (byte)((value) & 0xff);
                    _pixelStream.Seek(i * 4, SeekOrigin.Begin);
                    _pixelStream.Write(Bytes, i * 4, 4);
                }
            }
#endif

#if WINDOWS_PHONE
            public PixelBufferInfo(int[] pixelBuffer)
            {
                this.pixels = pixelBuffer;
            }
#else
            public PixelBufferInfo(IBuffer pixelBuffer)

            {
                _pixelStream = pixelBuffer.AsStream();
                this.Bytes = new byte[_pixelStream.Length];
                _pixelStream.Seek(0, SeekOrigin.Begin);
                _pixelStream.Read(this.Bytes, 0, Bytes.Length);
            }
            
            public void UpdateFromBytes()
            {
                _pixelStream.Seek(0, SeekOrigin.Begin);
                _pixelStream.Write(Bytes, 0, Bytes.Length);
            }
#endif           
        }
#if WINDOWS_PHONE
        public static PixelBufferInfo GetPixels(this int[] pixelBuffer)
#else
        public static PixelBufferInfo GetPixels(this IBuffer pixelBuffer)
#endif
        {
            return new PixelBufferInfo(pixelBuffer);
        }

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

        public static Pixel GetPixel(this Color color)
        {
            return new Pixel(color.R, color.G, color.B, color.A);
        }

    }

    public struct HsvColor
    {
        /// <summary>
        /// The Hue in 0..360 range.
        /// </summary>
        public double H;
        /// <summary>
        /// The Saturation in 0..1 range.
        /// </summary>
        public double S;
        /// <summary>
        /// The Value in 0..1 range.
        /// </summary>
        public double V;
        /// <summary>
        /// The Alpha/opacity in 0..1 range.
        /// </summary>
        public double A;
    }


}
