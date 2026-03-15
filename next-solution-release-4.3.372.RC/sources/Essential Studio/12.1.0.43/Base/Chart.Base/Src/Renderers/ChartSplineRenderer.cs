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

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The Spline chart renderering class.
    /// </summary>
    internal class SplineRenderer : ChartSeriesRenderer
    {
        #region Properties
        /// <summary>
        /// Gets count of require Y values of the points.
        /// </summary>
        /// <value></value>
        protected override int RequireYValuesCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets description of regions.
        /// </summary>
        protected override string RegionDescription
        {
            get
            {
                return "Spline Chart Region";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SplineRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public SplineRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs2D args)
        {
            ChartStyledPoint[] styledPoints = this.PrepearePoints();

            if (styledPoints.Length > 1)
            {
                double[] ys2;
                IndexRange visibleRange = this.CalculateVisibleRange();
                IList regions = this.ChartArea.ChartRegions;
                ChartLineConfigItem config = args.Series.ConfigItems.LineSegment;

                this.NaturalSpline(styledPoints, out ys2);

                int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
                int from = args.ActualXAxis.Inversed ? visibleRange.To : visibleRange.From;
                int to = args.ActualXAxis.Inversed ? visibleRange.From - 1 : visibleRange.To + 1;
                int di = args.ActualXAxis.Inversed ? -1 : 1;

                ChartStyledPoint first = null;
                ChartStyledPoint second = null;
                int firstIndex = -1;

                for (int i = from; i != to; i += di)
                {
                    second = styledPoints[i];

                    if (second.IsVisible)
                    {
                        if (first != null)
                        {
                            ChartPoint controlPoint1 = null;
                            ChartPoint controlPoint2 = null;

                            this.GetBezierControlPoints(first, second, ys2[firstIndex], ys2[i],
                                out controlPoint1, out controlPoint2, yIndex);

                            DoubleRange xRange = new DoubleRange(first.X, second.X);
                            DoubleRange yRange = new DoubleRange(first.YValues[yIndex], second.YValues[yIndex]);

                            if (args.IsVisible(xRange, yRange))
                            {
                                PointF pt1 = args.GetPoint(first.X, first.YValues[yIndex]);
                                PointF pt2 = args.GetPoint(second.X, second.YValues[yIndex]);
                                PointF bpt1 = args.GetPoint(controlPoint1.X, controlPoint1.YValues[0]);
                                PointF bpt2 = args.GetPoint(controlPoint2.X, controlPoint2.YValues[0]);

                                BrushInfo interior = this.GetBrush(first.Index);
                                Pen stroke = first.Style.GdipPen;

                                ChartRegionData chartRegionData = null;

                                if (args.UpdateRegions && !config.DisableLineRegion)
                                {
                                    chartRegionData = new ChartRegionData(args.SeriesIndex, first.Index, first.ToolTip, "Line Segment Region");
                                }

                                if (args.Is3D)
                                {
                                    #region Draw 3D line
                                    Region rgn = new Region(RectangleF.Empty);

                                    args.Graph.DrawLine(stroke, pt1.X, pt1.Y,
                                                pt1.X + args.DepthOffset.Width, pt1.Y + args.DepthOffset.Height);

                                    double interator1, interator2;

                                    if (this.ComputeExtremums(first, second, controlPoint1, controlPoint2,
                                        out interator1, out interator2, yIndex))
                                    {
                                        #region First interator
                                        if (!double.IsNaN(interator1))
                                        {
                                            PointF tp1, tp2, tp3, tp4;
                                            ChartMath.SplitBezierCurve(pt1, bpt1, bpt2, pt2, (float)interator1,
                                                out tp1, out tp2, out tp3, out tp4, out pt1, out bpt1, out bpt2, out pt2);

                                            rgn.Union(this.DrawBezier(args, stroke, interior, tp1, tp2, tp3, tp4));

                                            using (Pen pen = new Pen(interior.BackColor))
                                            {
                                                args.Graph.DrawLine(pen, tp4.X, tp4.Y,
                                                    tp4.X + args.DepthOffset.Width, tp4.Y + args.DepthOffset.Height);
                                            }
                                        }
                                        #endregion

                                        #region Second interator
                                        if (!double.IsNaN(interator2))
                                        {
                                            PointF tp1, tp2, tp3, tp4;
                                            ChartMath.SplitBezierCurve(pt1, bpt1, bpt2, pt2, (float)interator2,
                                                out tp1, out tp2, out tp3, out tp4, out pt1, out bpt1, out bpt2, out pt2);

                                            rgn.Union(this.DrawBezier(args, stroke, interior, tp1, tp2, tp3, tp4));

                                            using (Pen pen = new Pen(interior.BackColor))
                                            {
                                                args.Graph.DrawLine(pen, tp4.X, tp4.Y,
                                                    tp4.X + args.DepthOffset.Width, tp4.Y + args.DepthOffset.Height);
                                            }
                                        }
                                        #endregion
                                    }

                                    rgn.Union(this.DrawBezier(args, stroke, interior, pt1, bpt1, bpt2, pt2));

                                    args.Graph.DrawLine(stroke, pt2.X, pt2.Y,
                                                pt2.X + args.DepthOffset.Width, pt2.Y + args.DepthOffset.Height);

                                    if (chartRegionData != null)
                                    {
                                        args.Chart.ChartRegions.Add(new ChartRegion(rgn, chartRegionData));
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Draw 2D line
                                    using (Pen pen = stroke.Clone() as Pen)
                                    {
                                        GraphicsPath gp = new GraphicsPath();

                                        gp.AddBezier(pt1, bpt1, bpt2, pt2);

                                        if (first.Style.DisplayShadow)
                                        {
                                            Size sdwOffset = first.Style.ShadowOffset;

                                            pen.Color = first.Style.ShadowInterior.BackColor;

                                            args.Graph.PushTranfsorm();
                                            args.Graph.Transform = new Matrix(1, 0, 0, 1, sdwOffset.Width, sdwOffset.Height);
                                            args.Graph.DrawPath(pen, gp);
                                            args.Graph.PopTransform();
                                        }

                                        pen.Color = this.GetBrush(first.Index).BackColor;
                                        if (!(Math.Abs(pt1.X - pt2.X) > 1 || Math.Abs(pt1.Y - pt2.Y) > 1))
                                        {
                                            pen.Width = 1;
                                        }
                                        args.Graph.DrawPath(pen, gp);

                                        if (chartRegionData != null)
                                        {
                                            if (Math.Abs(pt1.X - pt2.X) > 1 || Math.Abs(pt1.Y - pt2.Y) > 1)
                                            {
                                                gp.Widen(pen);
                                                regions.Add(new ChartRegion(new Region(gp), chartRegionData));
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }

                        first = second;
                        firstIndex = i;
                    }
                    else
                    {
                       if(this.Chart.AllowGapForEmptyPoints)
                        first = null;
                    }
                }
            }
        }

        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            if (styledPoints.Length > 1)
            {
                double[] ys2;
                IndexRange visibleRange = this.CalculateVisibleRange();
                IList regions = this.ChartArea.ChartRegions;
                ChartLineConfigItem config = args.Series.ConfigItems.LineSegment;

                this.NaturalSpline(styledPoints, out ys2);

                int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
                int from = args.ActualXAxis.Inversed ? visibleRange.To : visibleRange.From;
                int to = args.ActualXAxis.Inversed ? visibleRange.From - 1 : visibleRange.To + 1;
                int di = args.ActualXAxis.Inversed ? -1 : 1;

                ChartStyledPoint first = null;
                ChartStyledPoint second = null;
                int firstIndex = -1;

                for (int i = from; i != to; i += di)
                {
                    second = styledPoints[i];

                    if (second.IsVisible)
                    {
                        if (first != null)
                        {
                            ChartPoint controlPoint1 = null;
                            ChartPoint controlPoint2 = null;

                            this.GetBezierControlPoints(first, second, ys2[firstIndex], ys2[i],
                                out controlPoint1, out controlPoint2, yIndex);

                            DoubleRange xRange = new DoubleRange(first.X, second.X);
                            DoubleRange yRange = new DoubleRange(first.YValues[yIndex], second.YValues[yIndex]);

                            if (args.IsVisible(xRange, yRange))
                            {
                                PointF pt1 = args.GetPoint(first.X, first.YValues[yIndex]);
                                PointF pt2 = args.GetPoint(second.X, second.YValues[yIndex]);
                                PointF bpt1 = args.GetPoint(controlPoint1.X, controlPoint1.YValues[0]);
                                PointF bpt2 = args.GetPoint(controlPoint2.X, controlPoint2.YValues[0]);

                                BrushInfo interior = this.GetBrush(first.Index);
                                Pen stroke = first.Style.GdipPen;

                                ChartRegionData chartRegionData = null;

                                if (args.UpdateRegions && !config.DisableLineRegion)
                                {
                                    chartRegionData = new ChartRegionData(args.SeriesIndex, first.Index, first.ToolTip, "Line Segment Region");
                                }

                                PointF[] bezierPoints = ChartMath.InterpolateBezier(pt1, bpt1, bpt2, pt2, SPLINE_DIGITIZATION);

                                Vector3D v1 = new Vector3D(pt1.X, pt1.Y, args.Z);
                                Vector3D v2 = new Vector3D(pt1.X, pt1.Y, args.Z + args.Depth);

                                Pen pnInter = new Pen(interior.BackColor);

                                for (int j = 0; j < bezierPoints.Length; j++)
                                {
                                    Vector3D v3 = new Vector3D(bezierPoints[j].X, bezierPoints[j].Y, args.Z);
                                    Vector3D v4 = new Vector3D(bezierPoints[j].X, bezierPoints[j].Y, args.Z + args.Depth);

                                    Polygon plg = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, interior, pnInter);
                                    plg.RegionData = chartRegionData;
                                    args.Graph.AddPolygon(plg);

                                    v1 = v4;
                                    v2 = v3;
                                }
                            }
                        }

                        first = second;
                        firstIndex = i;
                    }
                    else
                    {
                        if (this.Chart.AllowGapForEmptyPoints)
                        first = null;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the 3D bezier line.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="interior">The interior.</param>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The first control point.</param>
        /// <param name="pt3">The second control point.</param>
        /// <param name="pt4">The end point.</param>
        /// <returns>The geometry of line.</returns>
        private GraphicsPath DrawBezier(ChartRenderArgs2D args, Pen pen, BrushInfo interior,
            PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            GraphicsPath frontGp = new GraphicsPath();
            GraphicsPath backGp = new GraphicsPath();
            GraphicsPath fillGp = new GraphicsPath();

            frontGp.AddBezier(pt1, pt2, pt3, pt4);
            fillGp.AddBezier(pt1, pt2, pt3, pt4);

            pt1 = ChartMath.AddPoint(pt1, args.DepthOffset);
            pt2 = ChartMath.AddPoint(pt2, args.DepthOffset);
            pt3 = ChartMath.AddPoint(pt3, args.DepthOffset);
            pt4 = ChartMath.AddPoint(pt4, args.DepthOffset);

            backGp.AddBezier(pt4, pt3, pt2, pt1);
            fillGp.AddBezier(pt4, pt3, pt2, pt1);
            fillGp.CloseFigure();          


            args.Graph.DrawPath(interior, pen, fillGp);

            return fillGp;
        }

        /// <summary>
        /// Computes the extremums of bezier line.
        /// </summary>
        /// <param name="point1">The start point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="controlPoint1">The first control point.</param>
        /// <param name="controlPoint2">The second control point.</param>
        /// <param name="intelator1">The first intelator.</param>
        /// <param name="intelator2">The second intelator.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>
        /// 	<c>true</c> if extremums is present. otherwise, <c>false</c>.
        /// </returns>
        private bool ComputeExtremums(ChartStyledPoint point1, ChartStyledPoint point2,
            ChartPoint controlPoint1, ChartPoint controlPoint2, out double intelator1, out double intelator2, int yIndex)
        {
            double x1 = point1.X;
            double x2 = controlPoint1.X;
            double x3 = controlPoint2.X;
            double x4 = point2.X;

            double y1 = point1.YValues[yIndex];
            double y2 = controlPoint1.YValues[0];
            double y3 = controlPoint2.YValues[0];
            double y4 = point2.YValues[yIndex];

            double cx = 3 * (x2 - x1);
            double cy = 3 * (y2 - y1);

            double bx = 3 * (x3 - x2) - cx;
            double by = 3 * (y3 - y2) - cy;

            double ax = x4 - x1 - bx - cx;
            double ay = y4 - y1 - by - cy;

            double r1, r2;

            intelator1 = double.NaN;
            intelator2 = double.NaN;

            if (ChartMath.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2))
            {
                bool isVisible1 = r1 > 0 && r1 < 1;
                bool isVisible2 = r2 > 0 && r2 < 1;

                if (isVisible1 && isVisible2)
                {
                    intelator1 = Math.Min(r1, r2);
                    intelator2 = Math.Max(r1, r2);
                }
                else if (isVisible1)
                {
                    intelator1 = r1;
                }
                else if (isVisible2)
                {
                    intelator1 = r2;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Draw icon.
        /// </summary>
        /// <param name="g">The graphics to render icon.</param>
        /// <param name="bounds">The bounds of the icon.</param>
        /// <param name="isShadow">The value indicates draw shadow or not.</param>
        /// <param name="shadowColor">The shadow color.</param>
        public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
        {
            int x1 = bounds.X + bounds.Width / 3;
            int x2 = bounds.X + 2 * bounds.Width / 3;
            GraphicsPath gp = new GraphicsPath();

            gp.AddCurve(new Point[5]{
                                   new Point( bounds.X, bounds.Bottom ),
                                   new Point( x1, bounds.Top ),
                                   new Point( x2, bounds.Top + bounds.Height / 2 ),
                                   new Point( bounds.Right, bounds.Top ),
                                   new Point( bounds.Right, bounds.Bottom )
                                 });

            using (Pen pen = this.SeriesStyle.GdipPen.Clone() as Pen)
            {
                pen.Color = isShadow ? shadowColor : SeriesStyle.Interior.BackColor;
                g.DrawPath(pen, gp);
            }
        }
        #endregion
    }
}