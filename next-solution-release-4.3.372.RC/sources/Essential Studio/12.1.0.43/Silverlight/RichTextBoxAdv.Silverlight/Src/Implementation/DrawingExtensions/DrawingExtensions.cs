#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    internal static class DrawingExtensions
    {
        private const int SizeOfArgb = 4;
        private const float StepFactor = 2f;

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        /// <param name="color">The color.</param>
        internal static void FillRectangle(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            bmp.FillRectangle(x1, y1, x2, y2, col);
        }

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        /// <param name="color">The color.</param>
        internal static void FillRectangle(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] pixels = bmp.Pixels;

            // Check boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 >= w) { x1 = w - 1; }
            if (y1 >= h) { y1 = h - 1; }
            if (x2 >= w) { x2 = w - 1; }
            if (y2 >= h) { y2 = h - 1; }


            // Fill first line
            int startY = y1 * w;
            int startYPlusX1 = startY + x1;
            int endOffset = startY + x2;
            for (int x = startYPlusX1; x <= endOffset; x++)
            {
                pixels[x] = color;
            }

            // Copy first line
            int len = (x2 - x1) * SizeOfArgb;
            int srcOffsetBytes = startYPlusX1 * SizeOfArgb;
            int offset2 = y2 * w + x1;
            for (int y = startYPlusX1 + w; y < offset2; y += w)
            {
                Buffer.BlockCopy(pixels, srcOffsetBytes, pixels, y * SizeOfArgb, len);
            }
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        internal static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            bmp.DrawLine(x1, y1, x2, y2, col);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        internal static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color)
        {
            DrawLine(bmp.Pixels, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA. 
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="pixels">An array containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        internal static void DrawLine(int[] pixels, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color)
        {
            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;
            int len = pixels.Length;

            const int PRECISION_SHIFT = 8;
            const int PRECISION_VALUE = 1 << PRECISION_SHIFT;

            // Determine slope (absoulte value)
            int lenX, lenY;
            int incy1;
            if (dy >= 0)
            {
                incy1 = PRECISION_VALUE;
                lenY = dy;
            }
            else
            {
                incy1 = -PRECISION_VALUE;
                lenY = -dy;
            }

            int incx1;
            if (dx >= 0)
            {
                incx1 = 1;
                lenX = dx;
            }
            else
            {
                incx1 = -1;
                lenX = -dx;
            }

            if (lenX > lenY)
            { // x increases by +/- 1
                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / lenX;
                int y = y1 << PRECISION_SHIFT;

                // Walk the line!
                for (int i = 0; i < lenX; i++)
                {
                    // Check boundaries
                    y1 = y >> PRECISION_SHIFT;
                    if (x1 >= 0 && x1 < pixelWidth && y1 >= 0 && y1 < pixelHeight)
                    {
                        var i2 = y1 * pixelWidth + x1;
                        pixels[i2] = color;
                    }
                    x1 += incx1;
                    y += incy;
                }
            }
            else
            {
                // Prevent divison by zero
                if (lenY == 0)
                {
                    return;
                }

                // Init steps and start
                // since y increases by +/-1, we can safely add (*h) before the for() loop, since there is no fractional value for y
                int incx = (dx << PRECISION_SHIFT) / lenY;
                int x = x1 << PRECISION_SHIFT;
                int y = y1 << PRECISION_SHIFT;
                int index = (x1 + y1 * pixelWidth) << PRECISION_SHIFT;

                // Walk the line!
                var inc = incy1 * pixelWidth + incx;
                for (int i = 0; i < lenY; i++)
                {
                    x1 = x >> PRECISION_SHIFT;
                    y1 = y >> PRECISION_SHIFT;
                    if (x1 >= 0 && x1 < pixelWidth && y1 >= 0 && y1 < pixelHeight)
                    {
                        pixels[index >> PRECISION_SHIFT] = color;
                    }
                    x += incx;
                    y += incy1;
                    index += inc;
                }
            }
        } 
    }
}
