//-------------------------------------------------------------------------------------------------
// <copyright file="GridPaintTriangle.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// This class encapsulates the painting logic for a triangle. 
    /// </summary>
    public class GridPaintTriangle
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridPaintTriangle()
            : base()
        {
        }

        /// <summary>
        /// Paints a triangle to a given graphics canvas.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="dir">The <see cref="GridTriangleDirection"/> which indicates the sort order.</param>
        /// <param name="backBrush">Brush to paint the inner region of the triangle.</param>
        /// <param name="backPen1">Pen 1 to draw a traingle boundary.</param>
        /// <param name="backPen2">Pen 2 to draw a traingle boundary.</param>
        /// <param name="backPen3">Pen 3 to draw a traingle boundary.</param>
        /// <param name="opaque">Specifies if the triangle should be drawn transparent.</param>
        public static void Paint(Graphics g, Rectangle bounds, GridTriangleDirection dir, Brush backBrush, Pen backPen1, Pen backPen2, Pen backPen3, bool opaque)
        {
            System.Drawing.Point[] points = GridPaintTriangle.BuildTrianglePoints(dir, bounds);
            g.DrawLine(backPen1, points[0], points[1]);
            g.DrawLine(backPen2, points[1], points[2]);
            g.DrawLine(backPen3, points[2], points[0]);
        }

        /// <summary>
        /// Paints a triangle to a given graphics canvas.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="dir">The <see cref="GridTriangleDirection"/> which indicates the sort order.</param>
        /// <param name="backBrush">Brush to paint the inner region of the triangle.</param>
        /// <param name="backPen">Pen to draw a traingle boundary.</param>
        /// <param name="opaque">Specifies if the triangle should be drawn transparent.</param>
        public static void Paint(Graphics g, Rectangle bounds, GridTriangleDirection dir, Brush backBrush, Pen backPen, bool opaque)
        {
            System.Drawing.Point[] points = GridPaintTriangle.BuildTrianglePoints(dir, bounds);
            if (opaque)
            {
                g.FillPolygon(backBrush, points);
            }

            g.DrawPolygon(backPen, points);
        }

        /// <summary>
        /// Paints a triangle to a given graphics canvas.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="dir">The <see cref="GridTriangleDirection"/> which indicates the sort order.</param>
        /// <param name="backBrush">Brush to paint the inner region of the triangle.</param>
        /// <param name="backPen">Pen to draw a traingle boundary.</param>
        public static void Paint(Graphics g, Rectangle bounds, GridTriangleDirection dir, Brush backBrush, Pen backPen)
        {
            GridPaintTriangle.Paint(g, bounds, dir, backBrush, backPen, true);
        }

        private static Point[] BuildTrianglePoints(GridTriangleDirection dir, Rectangle bounds)
        {
            System.Drawing.Point[] points;
            int n1;
            int n2;

            points = new System.Drawing.Point[3];

            n1 = (int)(((double)Math.Min(bounds.Height, bounds.Width)) * TRI_WIDTH_RATIO);
            if ((n1 % 2) == 1)
            {
                n1++;
            }
            
            n2 = (int)Math.Ceiling(((double)n1) / 2.0 * 2.2);
            if ((n2 % 2) == 1)
            {
                n2++;
            }

            switch (dir)
            {
                case GridTriangleDirection.Up:
                    n1 += 1;
                    n2 -= 3;
                    points[0] = new Point(-1, n2);
                    points[1] = new Point(n1, n2);
                    points[2] = new Point((n1 / 2), 0);
                    break;

                case GridTriangleDirection.Down:
                    n1 += 1;
                    n2 -= 3;
                    points[0] = new Point(-1, 0);
                    points[1] = new Point(n1, 0);
                    points[2] = new Point((n1 / 2), n2);
                    break;

                case GridTriangleDirection.Left:
                    points[0] = new Point(n2, 0);
                    points[1] = new Point(n2, n1);
                    points[2] = new Point(0, (n1 / 2));
                    break;

                case GridTriangleDirection.Right:
                    points[0] = new Point(0, 0);
                    points[1] = new Point(0, n1);
                    points[2] = new Point(n2, (n1 / 2));
                    break;
            }

            switch (dir)
            {
                case GridTriangleDirection.Left:
                case GridTriangleDirection.Right:
                    GridPaintTriangle.OffsetPoints(points, (bounds.X + ((bounds.Width - n2) / 2)), (bounds.Y + ((bounds.Height - n1) / 2)));
                    break;
                case GridTriangleDirection.Up:
                case GridTriangleDirection.Down:
                    GridPaintTriangle.OffsetPoints(points, (bounds.X + ((bounds.Width - n1) / 2)), (bounds.Y + ((bounds.Height - n2) / 2)));
                    break;
            }

            return points;
        }

        private static void OffsetPoints(Point[] points, int xOffset, int yOffset)
        {
            for (int n = 0; n < points.Length; n++)
            {
                points[n].X = points[n].X + xOffset;
                points[n].Y = points[n].Y + yOffset;
            }
        }

        // Fields
        private const double TRI_HEIGHT_RATIO = 1.5;
        private const double TRI_WIDTH_RATIO = 0.8;
    }
}

