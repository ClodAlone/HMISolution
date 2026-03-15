//-------------------------------------------------------------------------------------------------
// <copyright file="GridUtil.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// This class provides static helper functions.
    /// </summary>
    public class GridUtil
    {
        /// <summary>
        /// Align to right on surface.
        /// </summary>
        public const ContentAlignment AnyRight = (ContentAlignment)0x444;

        /// <summary>
        /// Align at bottom on surface.
        /// </summary>
        public const ContentAlignment AnyBottom = (ContentAlignment)0x700;

        /// <summary>
        /// Align horizontally centered on surface.
        /// </summary>
        public const ContentAlignment AnyCenter = (ContentAlignment)0x222;

        /// <summary>
        /// Align vertically centered on surface.
        /// </summary>
        public const ContentAlignment AnyMiddle = (ContentAlignment)112;

        GridUtil()
        {
        }

        static IntPtr CreatePatternHBrush(short[] pattern)
        {
            IntPtr hBitmap = NativeMethods.CreateBitmap(8, 8, 1, 1, pattern);

            NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH();
            lb.lbStyle = 3/*BS_PATTERN*/;
            lb.lbHatch = (int)hBitmap;
            IntPtr hBrush = NativeMethods.CreateBrushIndirect(ref lb);
            NativeMethods.DeleteObject(hBitmap);
            return hBrush;
        }

        static IntPtr CreateHalftoneHBRUSH()
        {
            short[] pattern = new short[8];

            for (int n = 0; n < 8; n++)
            {
                pattern[n] = (short)(0x5555 << (n & 1));
            }

            return CreatePatternHBrush(pattern);
        }

        private static void DrawDragLine(IntPtr hdc, int x, int y, int width, int height, Rectangle clip)
        {
            Rectangle t = Rectangle.Intersect(clip, new Rectangle(x, y, width, height));
            if (!t.IsEmpty)
            {
                NativeMethods.PatBlt(hdc, t.X, t.Y, t.Width, t.Height, 0x5a0049);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void DrawDragRectHelper(Graphics g, Rectangle rect, Rectangle clip)
        {
            IntPtr hdc = g.GetHdc();
            int width = 2;
            IntPtr hbrush = CreateHalftoneHBRUSH();
            IntPtr oldbrush = NativeMethods.SelectObject(hdc, hbrush);
            DrawDragLine(hdc, rect.X, rect.Y, rect.Width, width, clip);
            DrawDragLine(hdc, rect.X, (rect.Y + width), width, (rect.Height - width), clip);
            DrawDragLine(hdc, (rect.X + width), ((rect.Y + rect.Height) - width), (rect.Width - (width * 2)), width, clip);
            DrawDragLine(hdc, ((rect.X + rect.Width) - width), (rect.Y + width), width, (rect.Height - width), clip);
            NativeMethods.SelectObject(hdc, oldbrush);
            NativeMethods.DeleteObject(hbrush);
            g.ReleaseHdc(hdc);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void Draw3dFrame(Graphics g, int x0, int y0, int x1, int y1, int w, Color rgbTopLeft, Color rgbBottomRight)
        {
            Rectangle rc;

            for (int i = 0; i < w; i++)
            {
                // Top
                Brush brTL = new SolidBrush(rgbTopLeft);
                rc = Rectangle.FromLTRB(x0, y0, x1, y0 + 1);
                g.FillRectangle(brTL, rc);

                // Left
                rc = Rectangle.FromLTRB(x0, y0, x0 + 1, y1);
                g.FillRectangle(brTL, rc);
                brTL.Dispose();

                Brush brBR = new SolidBrush(rgbBottomRight);

                // Bottom
                rc = Rectangle.FromLTRB(x0, y1, x1 + 1, y1 + 1);
                g.FillRectangle(brBR, rc);

                // Right
                rc = Rectangle.FromLTRB(x1, y0, x1 + 1, y1);
                g.FillRectangle(brBR, rc);
                brBR.Dispose();

                if (i < w - 1)
                {
                    x0++;
                    y0++;
                    x1--;
                    y1--;
                }
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// <c>true</c> if the specified value is set; otherwise, <c>false</c>.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool IsSet(int value, int flags)
        {
            return (value & flags) != 0;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// <c>true</c> if [is not set] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool IsNotSet(int value, int flags)
        {
            return (value & flags) == 0;
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool AllSet(int value, int flags)
        {
            return (value & flags) == flags;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void ClearBits(ref int value, int flags)
        {
            value &= ~flags;
        }

        /// <overload>
        /// Returns the value not smaller and not larger than a specified range.
        /// </overload>
        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>The value not smaller than min and and not larger than max.</returns>
        public static int MinMax(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>The value not smaller than min and and not larger than max.</returns>
        public static float MinMax(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>The value not smaller than min and and not larger than max.</returns>
        public static double MinMax(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="point">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>The value not smaller than min and and not larger than max.</returns>
        /// <remarks>
        /// The calculation is made for both the X and Y coordinates separately.
        /// </remarks>
        public static Point MinMax(Point point, Point min, Point max)
        {
            point.X = Math.Min(Math.Max(point.X, min.X), max.X);
            point.Y = Math.Min(Math.Max(point.Y, min.Y), max.Y);
            return point;
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="point">The value to compare.</param>
        /// <param name="bounds">The rectangle with bounds.</param>
        /// <returns>The value not smaller than min and and not larger than max.</returns>
        /// <remarks>
        /// The calculation is made for both the X and Y coordinates separately and compared
        /// with the rectangles boundaries.
        /// </remarks>
        public static Point MinMax(Point point, Rectangle bounds)
        {
            return MinMax(point, bounds.Location, new Point(bounds.Right, bounds.Bottom));
        }

        /// <overload>
        /// Returns the point with smaller value for X and Y coordinates each of two points.
        /// </overload>
        /// <summary>
        /// Returns the point with smaller value for X and Y coordinates each of two points.
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>The point with smaller value for X and Y coordinates each of two points</returns>
        public static Point Min(Point point1, Point point2)
        {
            point1.X = Math.Min(point1.X, point2.X);
            point1.Y = Math.Min(point1.Y, point2.Y);
            return point1;
        }

        /// <overload>
        /// Returns the point with larger value for X and Y coordinates each of two points.
        /// </overload>
        /// <summary>
        /// Returns the point with larger value for X and Y coordinates each of two points.
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>The point with larger value for X and Y coordinates each of two points.</returns>
        public static Point Max(Point point1, Point point2)
        {
            point1.X = Math.Max(point1.X, point2.X);
            point1.Y = Math.Max(point1.Y, point2.Y);
            return point1;
        }

        /// <summary>
        /// Returns the size with smaller value for Width and Height coordinates for each of two values.
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>The size with smaller value for Width and Height coordinates for each of two values.</returns>
        public static Size Min(Size size1, Size size2)
        {
            size1.Width = Math.Min(size1.Width, size2.Width);
            size1.Height = Math.Min(size1.Height, size2.Height);
            return size1;
        }

        /// <summary>
        /// Returns the size with larger value for Width and Height coordinates for each of two values.
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>The size with larger value for Width and Height coordinates for each of two values.</returns>
        public static Size Max(Size size1, Size size2)
        {
            size1.Width = Math.Max(size1.Width, size2.Width);
            size1.Height = Math.Max(size1.Height, size2.Height);
            return size1;
        }

        /// <overload>
        /// Increments the left coordinate of the rectangle only, the right coordinate remains the same.
        /// </overload>
        /// <summary>
        /// Increments the left coordinate of the rectangle only, the right coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="dx">The delta for changing the left bounds of the rectangle.</param>
        public static void OffsetLeft(ref Rectangle r, int dx)
        {
            r.X += dx;
            r.Width = Math.Max(r.Width - dx, 0);
        }

        /// <overload>
        /// Increments the top coordinate of the rectangle only, the bottom coordinate remains the same.
        /// </overload>
        /// <summary>
        /// Increments the top coordinate of the rectangle only, the bottom coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="dy">The delta for changing the top bounds of the rectangle.</param>
        public static void OffsetTop(ref Rectangle r, int dy)
        {
            r.Y += dy;
            r.Height = Math.Max(r.Height - dy, 0);
        }

        /// <overload>
        /// Increments the top and left coordinate of the rectangle only, the bottom and right coordinates remain the same.
        /// </overload>
        /// <summary>
        /// Increments the top and left coordinate of the rectangle only, the bottom and right coordinates remain the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="dx">The delta for changing the left bounds of the rectangle.</param>
        /// <param name="dy">The delta for changing the top bounds of the rectangle.</param>
        public static void OffsetLeftTop(ref Rectangle r, int dx, int dy)
        {
            r.X += dx;
            r.Width = Math.Max(r.Width - dx, 0);
            r.Y += dy;
            r.Height = Math.Max(r.Height - dy, 0);
        }

        /// <overload>
        /// Changes the left coordinate of the rectangle only, the right coordinate remains the same.
        /// </overload>
        /// <summary>
        /// Changes the left coordinate of the rectangle only, the right coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="x">The new value for the left bounds of the rectangle.</param>
        public static void SetLeft(ref Rectangle r, int x)
        {
            r.Width = r.Right - x;
            r.X = x;
        }

        /// <summary>
        /// Changes the right coordinate of the rectangle only, the left coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="x">The new value for the right bounds of the rectangle.</param>
        public static void SetRight(ref Rectangle r, int x)
        {
            r.Width = x - r.Left;
        }

        /// <overload>
        /// Changes the top coordinate of the rectangle only, the bottom coordinate remains the same.
        /// </overload>
        /// <summary>
        /// Changes the top coordinate of the rectangle only, the bottom coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="y">The new value for the top bounds of the rectangle.</param>
        public static void SetTop(ref Rectangle r, int y)
        {
            r.Height = r.Bottom - y;
            r.Y = y;
        }

        /// <summary>
        /// Increments the left coordinate of the rectangle only, the right coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="dx">The delta for changing the left bounds of the rectangle.</param>
        public static void OffsetLeft(ref RectangleF r, float dx)
        {
            r.X += dx;
            r.Width = Math.Max(r.Width - dx, 0);
        }

        /// <summary>
        /// Increments the top coordinate of the rectangle only, the bottom coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="dy">The delta for changing the top bounds of the rectangle.</param>
        public static void OffsetTop(ref RectangleF r, float dy)
        {
            r.Y += dy;
            r.Height = Math.Max(r.Height - dy, 0);
        }

        /// <summary>
        /// Increments the top and left coordinate of the rectangle only, the bottom and right coordinates remain the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="dx">The delta for changing the left bounds of the rectangle.</param>
        /// <param name="dy">The delta for changing the top bounds of the rectangle.</param>
        public static void OffsetLeftTop(ref RectangleF r, float dx, float dy)
        {
            r.X += dx;
            r.Width = Math.Max(r.Width - dx, 0);
            r.Y += dy;
            r.Height = Math.Max(r.Height - dy, 0);
        }

        /// <summary>
        /// Changes the left coordinate of the rectangle only, the right coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="x">The new value for the left bounds of the rectangle.</param>
        public static void SetLeft(ref RectangleF r, float x)
        {
            r.Width = r.Right - x;
            r.X = x;
        }

        /// <summary>
        /// Changes the top coordinate of the rectangle only, the bottom coordinate remains the same.
        /// </summary>
        /// <param name="r">The rectangle to be modified.</param>
        /// <param name="y">The new value for the top bounds of the rectangle.</param>
        public static void SetTop(ref RectangleF r, float y)
        {
            r.Height = r.Bottom - y;
            r.Y = y;
        }

        internal static Rectangle ConvertRectangle(RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        internal static void Wait(int milliseconds)
        {
            int ticks = Environment.TickCount + milliseconds;
            while (Environment.TickCount < ticks);
        }

        internal static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        internal static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        /// <summary>
        /// Returns the low word of an integer.
        /// </summary>
        /// <param name="n">The integer.</param>
        /// <returns>The low word.</returns>
        public static int Loword(int n)
        {
            return n & 0xffff;
        }

        /// <summary>
        /// Returns the high word of an integer.
        /// </summary>
        /// <param name="n">The integer.</param>
        /// <returns>The high word.</returns>
        public static int Hiword(int n)
        {
            return (n >> 16) & 0xffff;
        }

        internal static int LOWORD(IntPtr n)
        {
            return LOWORD((int)n);
        }

        internal static int HIWORD(IntPtr n)
        {
            return HIWORD((int)n);
        }

        /// <summary>
        /// Creates a long from two integers.
        /// </summary>
        /// <param name="low">The low word.</param>
        /// <param name="high">The high word.</param>
        /// <returns>The combined value.</returns>
        public static int MakeLong(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        internal static int MAKELONG(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        internal static int MAKELPARAM(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <returns>returns Int SignedHIWORD</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int SignedHIWORD(IntPtr n)
        {
            return SignedHIWORD((int)n);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns Int SignedHIWORD</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int SignedHIWORD(int n)
        {
            int n0;
            n0 = (int)((short)((n >> 16) & 0xffff/*=~0x0000*/));
            n0 = n0 << 16;
            n0 = n0 >> 16;
            return n0;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns Int SignedLOWORD</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int SignedLOWORD(int n)
        {
            int n0;
            n0 = (int)((short)(n & 0xffff/*=~0x0000*/));
            n0 = n0 << 16;
            n0 = n0 >> 16;
            return n0;
        }

        /// <summary>
        /// Signeds the LOWORD.
        /// </summary>
        /// <returns>Returns int SignedLOWOR</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int SignedLOWORD(IntPtr n)
        {
            return SignedLOWORD((int)n);
        }

        internal static bool EmptyOrSpace(string str)
        {
            if (str != null)
            {
                return str.Trim().Length == 0;
            }

            return true;
        }

        /// <overload>
        /// Checks if value is double.NaN or float.NaN.
        /// </overload>
        /// <summary>
        /// Checks if value is double.NaN.
        /// </summary>
        /// <param name="b">The value to compare.</param>
        /// <returns>True if this is is double.NaN.</returns>
        public static bool IsNotValidNumber(double b)
        {
            return !(b >= double.MinValue && b <= double.MaxValue);
        }

        /// <summary>
        /// Checks if value is float.NaN.
        /// </summary>
        /// <param name="b">The value to compare.</param>
        /// <returns>True if this is is double.NaN.</returns>
        public static bool IsNotValidNumber(float b)
        {
            return !(b >= float.MinValue && b <= float.MaxValue);
        }

        internal static bool NotEmpty(string str)
        {
            if (str != null)
            {
                return str.Length == 0;
            }

            return false;
        }

        /// <summary>
        /// Checks if the string is NULL or length is 0.
        /// </summary>
        /// <param name="str">The string to be checked.</param>
        /// <returns>True if string is NULL or length is 0.</returns>
        public static bool IsEmpty(string str)
        {
            return str == null || str.Length == 0;
        }

        internal static bool NotEmptyAfterTrim(string str)
        {
            if (str != null)
            {
                return str.Trim().Length > 0;
            }

            return false;
        }

        internal static bool StringEqual(string str1, string str2, CultureInfo culture)
        {
            if (str1 == null && str2 != null)
            {
                return true;
            }

            if (str1 == null || str2 == null)
            {
                return false;
            }

            return String.Compare(str1, str2, false, culture) == 0;
        }

        /// <summary>
        /// Swaps the <see cref="StringAlignment"/> from Near to Far and vice versa.
        /// </summary>
        /// <param name="alignment">The value to be converted.</param>
        /// <returns>The resulting <see cref="StringAlignment"/>.</returns>
        public static StringAlignment SwapAlignment(StringAlignment alignment)
        {
            if (alignment == StringAlignment.Near)
            {
                return StringAlignment.Far;
            }
            else if (alignment == StringAlignment.Far)
            {
                return StringAlignment.Near;
            }

            return alignment;
        }

        /// <overload>
        /// Creates a <see cref="StringAlignment"/> from a <see cref="GridVerticalAlignment"/> or  <see cref="GridHorizontalAlignment"/>.
        /// </overload>
        /// <summary>
        /// Creates a <see cref="StringAlignment"/> from a <see cref="GridVerticalAlignment"/>.
        /// </summary>
        /// <param name="align">The value to be converted.</param>
        /// <returns>The resulting <see cref="StringAlignment"/>.</returns>
        public static StringAlignment ConvertToStringAlignment(GridVerticalAlignment align)
        {
            switch (align)
            {
                case GridVerticalAlignment.Bottom:
                    return StringAlignment.Far;
                case GridVerticalAlignment.Middle:
                    return StringAlignment.Center;
                default:
                    return StringAlignment.Near;
            }
        }

        /// <summary>
        /// Creates a <see cref="StringAlignment"/> from a <see cref="GridHorizontalAlignment"/>.
        /// </summary>
        /// <param name="align">The value to be converted.</param>
        /// <returns>The resulting <see cref="StringAlignment"/>.</returns>
        public static StringAlignment ConvertToStringAlignment(GridHorizontalAlignment align)
        {
            switch (align)
            {
                case GridHorizontalAlignment.Right:
                    return StringAlignment.Far;
                case GridHorizontalAlignment.Center:
                    return StringAlignment.Center;
                default:
                    return StringAlignment.Near;
            }
        }


        /// <summary>
        /// Creates a <see cref="StringLineAlignment"/> from a <see cref="GridHorizontalAlignment"/> and Creates a <see cref="StringAlignment"/> from a <see cref="GridVerticalAlignment"/>.
        /// </summary>
        /// <param name="strFormat">StringFormat to return format</param>
        /// <param name="hAlign">GridHorizontalAlignment</param>
        /// <param name="vAlign">GridVerticalAlignment</param>
        /// <returns>it returns StringFormat for rotation string  </returns>
        public static StringFormat ConvertToRotateStringAlignment(StringFormat strFormat, GridHorizontalAlignment hAlign, GridVerticalAlignment vAlign)
        {
            switch (hAlign)
            {
                case GridHorizontalAlignment.Left:
                    strFormat.LineAlignment = StringAlignment.Near;
                    switch (vAlign)
                    {
                        case GridVerticalAlignment.Top:
                            strFormat.Alignment = StringAlignment.Far;
                            break;
                        case GridVerticalAlignment.Middle:
                            strFormat.Alignment = StringAlignment.Center;
                            break;
                        default:
                            strFormat.Alignment = StringAlignment.Near;
                            break;
                    }
                    break;
                case GridHorizontalAlignment.Center:
                    strFormat.LineAlignment = StringAlignment.Center;
                    switch (vAlign)
                    {
                        case GridVerticalAlignment.Top:
                            strFormat.Alignment = StringAlignment.Far;
                            break;
                        case GridVerticalAlignment.Middle:
                            strFormat.Alignment = StringAlignment.Center;
                            break;
                        default:
                            strFormat.Alignment = StringAlignment.Near;
                            break;
                    }
                    break;
                default:
                    strFormat.LineAlignment = StringAlignment.Far;
                    switch (vAlign)
                    {
                        case GridVerticalAlignment.Top:
                            strFormat.Alignment = StringAlignment.Far;
                            break;
                        case GridVerticalAlignment.Middle:
                            strFormat.Alignment = StringAlignment.Center;
                            break;
                        default:
                            strFormat.Alignment = StringAlignment.Near;
                            break;
                    }
                    break;
            }
            return strFormat;
        }

        /// <overload>
        /// Creates a <see cref="ContentAlignment"/> from a <see cref="GridVerticalAlignment"/> or  <see cref="GridHorizontalAlignment"/>.
        /// </overload>
        /// <summary>
        /// Creates a <see cref="ContentAlignment"/> from a <see cref="GridVerticalAlignment"/>.
        /// </summary>
        /// <param name="align">The value to be converted.</param>
        /// <returns>The resulting <see cref="ContentAlignment"/>.</returns>
        public static ContentAlignment ConvertToContentAlignment(GridVerticalAlignment align)
        {
            switch (align)
            {
                case GridVerticalAlignment.Bottom:
                    return ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
                case GridVerticalAlignment.Middle:
                    return ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
                default:
                    return ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
            }
        }

        internal static GridBackgroundImageMode ConvertToBackgroundImageMode(PictureBoxSizeMode mode)
        {
            switch (mode)
            {
                case PictureBoxSizeMode.AutoSize:
                case PictureBoxSizeMode.Normal:
                    return GridBackgroundImageMode.Normal;

                case PictureBoxSizeMode.CenterImage:
                    return GridBackgroundImageMode.CenterImage;

                case PictureBoxSizeMode.StretchImage:
                    return GridBackgroundImageMode.StretchImage;
            }

            return GridBackgroundImageMode.Normal;
        }

        internal static PictureBoxSizeMode ConvertToPictureBoxSizeMode(GridBackgroundImageMode mode)
        {
            switch (mode)
            {
                case GridBackgroundImageMode.Normal:
                    return PictureBoxSizeMode.Normal;

                case GridBackgroundImageMode.CenterImage:
                    return PictureBoxSizeMode.CenterImage;

                case GridBackgroundImageMode.StretchImage:
                    return PictureBoxSizeMode.StretchImage;
            }

            return PictureBoxSizeMode.Normal;
        }

        internal static Rectangle GetImageRectangle(Image image, Size clientSize, PictureBoxSizeMode sizeMode)
        {
            Rectangle imageRect = new Rectangle(0, 0, 0, 0);
            if (image != null)
            {
                switch (sizeMode)
                {
                    case PictureBoxSizeMode.Normal:
                    case PictureBoxSizeMode.AutoSize:
                        imageRect.Size = image.Size;
                        break;
                    case PictureBoxSizeMode.StretchImage:
                        //imageRect.Size = clientSize;
						imageRect.Size = new Size(clientSize.Width + clientSize.Width / 10, clientSize.Height + clientSize.Width/10);   
                        break;

                    case PictureBoxSizeMode.CenterImage:
                        imageRect.Size = image.Size;
                        imageRect.X = (clientSize.Width - imageRect.Width) / 2;
                        imageRect.Y = (clientSize.Height - imageRect.Height) / 2;
                        break;
                }
            }

            return imageRect;
        }

        /// <summary>
        /// Creates a <see cref="ContentAlignment"/> from a <see cref="GridHorizontalAlignment"/>.
        /// </summary>
        /// <param name="align">The value to be converted.</param>
        /// <returns>The resulting <see cref="ContentAlignment"/>.</returns>
        public static ContentAlignment ConvertToContentAlignment(GridHorizontalAlignment align)
        {
            switch (align)
            {
                case GridHorizontalAlignment.Right:
                    return ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
                case GridHorizontalAlignment.Center:
                    return ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
                default:
                    return ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft;
            }
        }

        /// <summary>
        /// Returns a centered rectangle of a specified size within a given rectangle.
        /// </summary>
        /// <param name="rect">The outer rectangle.</param>
        /// <param name="size">The size of the rectangle to be centered.</param>
        /// <returns>The centered rectangle.</returns>
        static public Rectangle CenterInRect(Rectangle rect, Size size)
        {
            int dx = 0;
            if (size.Width < rect.Width)
            {
                dx = rect.Width - size.Width;
            }

            int dy = 0;
            if (size.Height < rect.Height)
            {
                dy = rect.Height - size.Height;
            }

            return new Rectangle(rect.Left + (dx / 2), rect.Top + (dy / 2), Math.Min(size.Width, rect.Width), Math.Min(size.Height, rect.Height));
        }

        /// <summary>
        /// Returns a centered point within a given rectangle.
        /// </summary>
        /// <param name="rect">The outer rectangle.</param>
        /// <returns>The centered point.</returns>
        static public Point CenterPoint(Rectangle rect)
        {
            return new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        /// <summary>
        /// Gets the form that the control is assigned to.
        /// </summary>
        /// <param name="control">The control whose parent form is searched.</param>
        /// <returns>The parent form of the control or NULL.</returns>
        static public Form GetParentFrame(Control control)
        {
            if (control is Form || control == null)
            {
                return (Form)control;
            }

            return GetParentFrame(control.Parent);
        }

        /// <summary>
        /// Returns a value indicating if the control is the active control in the parent container.
        /// </summary>
        /// <param name="control">The control to be tested.</param>
        /// <returns>True if active; False otherwise.</returns>
        /// <seealso cref="IContainerControl"/>
        static public bool IsActiveControl(Control control)
        {
            bool active = true;
            Control parent = GetParentControl(control, typeof(IContainerControl));
            while (parent != null)
            {
                active &= ((IContainerControl)parent).ActiveControl == control;
                control = parent;
                parent = GetParentControl(control, typeof(IContainerControl));
            }

            return active;
        }

        // IDynamicSplitterFrame

        /// <summary>
        /// Gets the parent control of a specific type that the control is assigned to.
        /// </summary>
        /// <param name="control">The control whose parent form is searched.</param>
        /// <param name="type">The type of the parent control to search.</param>
        /// <returns>The parent control or NULL.</returns>
        static public Control GetParentControl(Control control, Type type)
        {
            if (type.IsInstanceOfType(control) || control == null)
            {
                return control;
            }

            return GetParentControl(control.Parent, type);
        }

        static internal void Sleep(int ticks)
        {
            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 0, ticks));
        }
    }
}
