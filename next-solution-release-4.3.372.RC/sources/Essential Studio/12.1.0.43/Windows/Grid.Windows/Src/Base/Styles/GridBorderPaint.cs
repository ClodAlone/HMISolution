//-------------------------------------------------------------------------------------------------
// <copyright file="GridBorderPaint.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides the rectangle routine for drawing a <see cref="GridBorder"/>.
    /// </summary>
    public sealed class GridBorderPaint
    {
        [ThreadStaticAttribute]
        static Bitmap[] borderBitmaps = null;

        static object[] borderSpecs = new object[]
        {
            // Horizontal
            new short[] { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 }, // 1: PS_DASH        /* ------- */
            new short[] { 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 }, // 2: PS_DOT         /* ....... */
            new short[] { 0xb1, 0xb1, 0xb1, 0xb1, 0xb1, 0xb1, 0xb1, 0xb1 }, // 3: PS_DASHDOT     /* _._._._ */
            new short[] { 0x51, 0x51, 0x51, 0x51, 0x51, 0x51, 0x51, 0x51 }, // 4: PS_DASHDOTDOT  /* _.._.._ */
            // Vertical
            new short[] { 0x00, 0x00, 0x00, 0xff, 0x00, 0x00, 0x00, 0xff }, // 1: PS_DASH        /* ------- */
            new short[] { 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff }, // 2: PS_DOT         /* ....... */
            new short[] { 0x00, 0x00, 0x00, 0xff, 0xff, 0x00, 0xff, 0xff }, // 3: PS_DASHDOT     /* _._._._ */
            new short[] { 0x00, 0x00, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff }, // 4: PS_DASHDOTDOT  /* _.._.._ */
        };

        private GridBorderPaint()
        {
        }

        /// <overload>
        /// Draws a rectangle with a <see cref="GridBorder"/>.
        /// </overload>
        /// <summary>
        /// Draws a rectangle with a <see cref="GridBorder"/>.
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> context</param>
        /// <param name="border">A <see cref="GridBorder"/>.</param>
        /// <param name="r"><see cref="Rectangle"/> structure that represents the rectangle to fill. </param>
        /// <param name="backColor">The back color.</param>
        /// <param name="borderSide">Specifies which border sides to draw.</param>
        public static void DrawRectangle(Graphics g, GridBorder border, Rectangle r, Color backColor, GridBorderSide borderSide)
        {
            DrawRectangle(g, border, r, backColor, borderSide, false);
        }
        
        private static void PrintRectangle(Graphics g, GridBorder border, Rectangle r, Color backColor, GridBorderSide borderSide)
        {
            float pp = 0.5F;
#if DEBUG
            if (Switches.GridPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(border, r, backColor, borderSide);
            }
#else
            ;
#endif
            float width = 0.5F;
            switch (border.Weight)
            {
                case GridBorderWeight.ExtraThin:
                    width = 0.347222F;        // 0.25 point
                    break;

                case GridBorderWeight.Thin:
                    width = 0.694444F;        // 0.5 point
                    break;

                case GridBorderWeight.Medium:
                    width = 1.388889F;        // 1 point
                    break;

                case GridBorderWeight.Thick:
                    width = 2.083333F;        // 1.5 point
                    break;

                case GridBorderWeight.ExtraThick:
                    width = 2.777778F;        // 2.0 point
                    break;

                case GridBorderWeight.ExtraExtraThick:
                    width = 4.166667F;        // 3.0 point
                    break;
            }

            float fixLen = 0;
            float w = width;
            if (width < pp)
            {
                width = 0.0F;
                fixLen = w;
            }

            RectangleF topEdge = new RectangleF(r.X, r.Y, r.Width - fixLen, width);
            RectangleF leftEdge = new RectangleF(r.X, r.Y, width, r.Height - fixLen);

            RectangleF rightEdge = new RectangleF(r.Right - w, r.Y, width, r.Height - fixLen);
            RectangleF bottomEdge = new RectangleF(r.X, r.Bottom - w, r.Width - fixLen, width);

            Brush brush = null;
            Bitmap bm = null;
            Bitmap bmV = null;

            switch (border.Style)
            {
                case GridBorderStyle.None:
                case GridBorderStyle.NotSet:
                    break;

                case GridBorderStyle.Solid:
                    if (width > pp)
                    {
                        brush = new SolidBrush(border.Color);
                        if ((borderSide & GridBorderSide.Left) != 0)
                        {
                            g.FillRectangle(brush, leftEdge);
                        }

                        if ((borderSide & GridBorderSide.Right) != 0)
                        {
                            g.FillRectangle(brush, rightEdge);
                        }

                        if ((borderSide & GridBorderSide.Top) != 0)
                        {
                            g.FillRectangle(brush, topEdge);
                        }

                        if ((borderSide & GridBorderSide.Bottom) != 0)
                        {
                            g.FillRectangle(brush, bottomEdge);
                        }

                        brush.Dispose();
                    }
                    else
                    {
                        Pen pen = new Pen(border.Color);
                        pen.EndCap = LineCap.Flat;
                        pen.StartCap = LineCap.Flat;
                        pen.Width = w;
                        if ((borderSide & GridBorderSide.Left) != 0)
                        {
                            g.DrawLine(pen, leftEdge.Location, new PointF(leftEdge.Left, leftEdge.Bottom));
                        }

                        if ((borderSide & GridBorderSide.Right) != 0)
                        {
                            g.DrawLine(pen, rightEdge.Location, new PointF(rightEdge.Left, rightEdge.Bottom));
                        }

                        if ((borderSide & GridBorderSide.Top) != 0)
                        {
                            g.DrawLine(pen, topEdge.Location, new PointF(topEdge.Right, topEdge.Top));
                        }

                        if ((borderSide & GridBorderSide.Bottom) != 0)
                        {
                            g.DrawLine(pen, bottomEdge.Location, new PointF(bottomEdge.Right, bottomEdge.Top));
                        }

                        pen.Dispose();
                    }

                    break;

                case GridBorderStyle.DashDot:
                case GridBorderStyle.DashDotDot:
                case GridBorderStyle.Dashed:
                case GridBorderStyle.Dotted:
                    {
                        if ((borderSide & (GridBorderSide.Left | GridBorderSide.Right)) != 0)
                        {
                            bmV = CreateBitmapFromBorderSpecs((int)border.Style + 2);
                        }

                        if ((borderSide & (GridBorderSide.Top | GridBorderSide.Bottom)) != 0)
                        {
                            bm = CreateBitmapFromBorderSpecs((int)border.Style - 2);
                        }

                        if (bm != null || bmV != null)
                        {
                            ColorMap colorMap1 = new ColorMap();
                            colorMap1.OldColor = Color.White;
                            colorMap1.NewColor = backColor;

                            ColorMap colorMap2 = new ColorMap();
                            colorMap2.OldColor = Color.Black;
                            colorMap2.NewColor = border.Color;

                            ColorMap[] colorMaps = new ColorMap[]
                        {
                            colorMap1, colorMap2
                        };

                            ImageAttributes ia = new ImageAttributes();
                            ia.SetRemapTable(colorMaps);

                            if (bmV != null)
                            {
                                TextureBrush brV = new TextureBrush(bmV, new Rectangle(new Point(0, 0), bmV.PhysicalDimension.ToSize()), ia);
                                brV.WrapMode = WrapMode.Tile;
                                if (width > 0)
                                {
                                    if ((borderSide & GridBorderSide.Left) != 0)
                                    {
                                        g.FillRectangle(brV, leftEdge);
                                    }

                                    if ((borderSide & GridBorderSide.Right) != 0)
                                    {
                                        g.FillRectangle(brV, rightEdge);
                                    }
                                }
                                else
                                {
                                    Pen pen = new Pen(brV);
                                    pen.EndCap = LineCap.Square;
                                    pen.StartCap = LineCap.Square;
                                    if ((borderSide & GridBorderSide.Left) != 0)
                                    {
                                        g.DrawLine(pen, leftEdge.Location, new PointF(leftEdge.Left, leftEdge.Bottom));
                                    }

                                    if ((borderSide & GridBorderSide.Right) != 0)
                                    {
                                        g.DrawLine(pen, rightEdge.Location, new PointF(rightEdge.Left, rightEdge.Bottom));
                                    }

                                    pen.Dispose();
                                }

                                brV.Dispose();
                            }

                            if (bm != null)
                            {
                                TextureBrush br = new TextureBrush(bm, new Rectangle(new Point(0, 0), bm.PhysicalDimension.ToSize()), ia);
                                br.WrapMode = WrapMode.Tile;
                                if (width > 0)
                                {
                                    if ((borderSide & GridBorderSide.Top) != 0)
                                    {
                                        g.FillRectangle(br, topEdge);
                                    }

                                    if ((borderSide & GridBorderSide.Bottom) != 0)
                                    {
                                        g.FillRectangle(br, bottomEdge);
                                    }
                                }
                                else
                                {
                                    Pen pen = new Pen(br);
                                    pen.EndCap = LineCap.Square;
                                    pen.StartCap = LineCap.Square;
                                    if ((borderSide & GridBorderSide.Top) != 0)
                                    {
                                        g.DrawLine(pen, topEdge.Location, new PointF(topEdge.Right, topEdge.Top));
                                    }

                                    if ((borderSide & GridBorderSide.Bottom) != 0)
                                    {
                                        g.DrawLine(pen, bottomEdge.Location, new PointF(bottomEdge.Right, bottomEdge.Top));
                                    }

                                    pen.Dispose();
                                }

                                br.Dispose();
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Draws a rectangle with a <see cref="GridBorder"/>.
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> context.</param>
        /// <param name="border">A <see cref="GridBorder"/>.</param>
        /// <param name="r"><see cref="Rectangle"/> structure that represents the rectangle to fill. </param>
        /// <param name="backColor">The back color.</param>
        /// <param name="borderSide">Specifies which border sides to draw.</param>
        /// <param name="usePenForThinLines">True if a <see cref="Pen"/> should be used for thin lines instead
        /// of an optimized method that uses brushes.</param>
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public static void DrawRectangle(Graphics g, GridBorder border, Rectangle r, Color backColor, GridBorderSide borderSide, bool usePenForThinLines)
        {
            if (usePenForThinLines)
            {
                PrintRectangle(g, border, r, backColor, borderSide);
                return;
            }
#if DEBUG

            if (Switches.GridPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(border, r, backColor, borderSide, usePenForThinLines);
            }
#else

            ;
#endif
            Point topLeft = new Point(r.X, r.Y);
            Point topRight = new Point(r.X, r.Right);
            Point bottomRight = new Point(r.Right, r.Bottom);
            Point bottomLeft = new Point(r.X, r.Bottom);

            int width = 1;
            switch (border.Weight)
            {
                case GridBorderWeight.ExtraThin:
                    width = 1;
                    break;

                case GridBorderWeight.Thin:
                    width = 1;
                    break;

                case GridBorderWeight.Medium:
                    width = 2;
                    break;

                case GridBorderWeight.Thick:
                    width = 3;
                    break;

                case GridBorderWeight.ExtraThick:
                    width = 4;
                    break;

                case GridBorderWeight.ExtraExtraThick:
                    width = 4;
                    break;
            }

            int fixLen = 0;
            int w = width;
            if (usePenForThinLines && width == 1)
            {
                width = 0;
                fixLen = 1;
            }

            Rectangle topEdge = new Rectangle(r.X, r.Y, r.Width - fixLen, width);
            Rectangle leftEdge = new Rectangle(r.X, r.Y, width, r.Height - fixLen);

            Rectangle rightEdge = new Rectangle(r.Right - w, r.Y, width, r.Height - fixLen);
            Rectangle bottomEdge = new Rectangle(r.X, r.Bottom - w, r.Width - fixLen, width);

            Brush brush = null;
            Bitmap bm = null;
            Bitmap bmV = null;

            switch (border.Style)
            {
                case GridBorderStyle.None:
                case GridBorderStyle.NotSet:
                    break;

                case GridBorderStyle.Solid:
                    if (width > 0)
                    {
                        brush = new SolidBrush(border.Color);
                        if ((borderSide & GridBorderSide.Left) != 0)
                        {
                            g.FillRectangle(brush, leftEdge);
                        }

                        if ((borderSide & GridBorderSide.Right) != 0)
                        {
                            g.FillRectangle(brush, rightEdge);
                        }

                        if ((borderSide & GridBorderSide.Top) != 0)
                        {
                            g.FillRectangle(brush, topEdge);
                        }

                        if ((borderSide & GridBorderSide.Bottom) != 0)
                        {
                            g.FillRectangle(brush, bottomEdge);
                        }

                        brush.Dispose();
                    }
                    else
                    {
                        Pen pen = new Pen(border.Color);
                        pen.EndCap = LineCap.Square;
                        pen.StartCap = LineCap.Square;
                        if ((borderSide & GridBorderSide.Left) != 0)
                        {   
                            g.DrawLine(pen, leftEdge.Location, new Point(leftEdge.Left, leftEdge.Bottom));
                        }

                        if ((borderSide & GridBorderSide.Right) != 0)
                        {  
                            g.DrawLine(pen, rightEdge.Location, new Point(rightEdge.Left, rightEdge.Bottom));
                        }

                        if ((borderSide & GridBorderSide.Top) != 0)
                        { 
                            g.DrawLine(pen, topEdge.Location, new Point(topEdge.Right, topEdge.Top));
                        }

                        if ((borderSide & GridBorderSide.Bottom) != 0)
                        {
                            g.DrawLine(pen, bottomEdge.Location, new Point(bottomEdge.Right, bottomEdge.Top));
                        }

                        pen.Dispose();
                    }

                    break;

                case GridBorderStyle.DashDot:
                case GridBorderStyle.DashDotDot:
                case GridBorderStyle.Dashed:
                case GridBorderStyle.Dotted:
                    {
                        if ((borderSide & (GridBorderSide.Left | GridBorderSide.Right)) != 0)
                        {
                            bmV = CreateBitmapFromBorderSpecs((int)border.Style + 2);
                        }

                        if ((borderSide & (GridBorderSide.Top | GridBorderSide.Bottom)) != 0)
                        {
                            bm = CreateBitmapFromBorderSpecs((int)border.Style - 2);
                        }

                        if (bm != null || bmV != null)
                        {
                            ColorMap colorMap1 = new ColorMap();
                            colorMap1.OldColor = Color.White;
                            colorMap1.NewColor = backColor;

                            ColorMap colorMap2 = new ColorMap();
                            colorMap2.OldColor = Color.Black;
                            colorMap2.NewColor = border.Color;

                            ColorMap[] colorMaps = new ColorMap[]
                        {
                            colorMap1, colorMap2
                        };

                            ImageAttributes ia = new ImageAttributes();
                            ia.SetRemapTable(colorMaps);

                            if (bmV != null)
                            {
                                TextureBrush brV = new TextureBrush(bmV, new Rectangle(new Point(0, 0), bmV.PhysicalDimension.ToSize()), ia);
                                brV.WrapMode = WrapMode.Tile;
                                if (width > 0)
                                {
                                    if ((borderSide & GridBorderSide.Left) != 0)
                                    {
                                        g.FillRectangle(brV, leftEdge);
                                    }

                                    if ((borderSide & GridBorderSide.Right) != 0)
                                    {
                                        g.FillRectangle(brV, rightEdge);
                                    }
                                }
                                else
                                {
                                    Pen pen = new Pen(brV);
                                    pen.EndCap = LineCap.Square;
                                    pen.StartCap = LineCap.Square;
                                    if ((borderSide & GridBorderSide.Left) != 0)
                                    {
                                        g.DrawLine(pen, leftEdge.Location, new Point(leftEdge.Left, leftEdge.Bottom));
                                    }

                                    if ((borderSide & GridBorderSide.Right) != 0)
                                    {
                                        g.DrawLine(pen, rightEdge.Location, new Point(rightEdge.Left, rightEdge.Bottom));
                                    }

                                    pen.Dispose();
                                }

                                brV.Dispose();
                            }

                            if (bm != null)
                            {
                                TextureBrush br = new TextureBrush(bm, new Rectangle(new Point(0, 0), bm.PhysicalDimension.ToSize()), ia);
                                br.WrapMode = WrapMode.Tile;
                                if (width > 0)
                                {
                                    if ((borderSide & GridBorderSide.Top) != 0)
                                    {
                                        g.FillRectangle(br, topEdge);
                                    }

                                    if ((borderSide & GridBorderSide.Bottom) != 0)
                                    {
                                        g.FillRectangle(br, bottomEdge);
                                    }
                                }
                                else
                                {
                                    Pen pen = new Pen(br);
                                    pen.EndCap = LineCap.Square;
                                    pen.StartCap = LineCap.Square;
                                    if ((borderSide & GridBorderSide.Top) != 0)
                                    {
                                        g.DrawLine(pen, topEdge.Location, new Point(topEdge.Right, topEdge.Top));
                                    }

                                    if ((borderSide & GridBorderSide.Bottom) != 0)
                                    {
                                        g.DrawLine(pen, bottomEdge.Location, new Point(bottomEdge.Right, bottomEdge.Top));
                                    }

                                    pen.Dispose();
                                }

                                br.Dispose();
                            }

                            ia.Dispose();
                        }
                    }

                    break;
            }
        }

        private static Bitmap CreateBitmapFromBorderSpecs(int n)
        {
            if (borderBitmaps == null)
            {
                borderBitmaps = new Bitmap[borderSpecs.Length];
            }

            if (borderBitmaps[n] == null)
            {
                IntPtr hBitmap = NativeMethods.CreateBitmap(8, 8, 1, 1, (short[])borderSpecs[n]);
                if (hBitmap != IntPtr.Zero)
                {
                    borderBitmaps[n] = Image.FromHbitmap(hBitmap);
                }

                NativeMethods.DeleteObject(hBitmap);
            }

            return borderBitmaps[n];
        }
    }
}