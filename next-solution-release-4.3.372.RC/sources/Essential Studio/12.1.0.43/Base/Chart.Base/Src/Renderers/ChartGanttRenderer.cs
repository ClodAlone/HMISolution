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
using System.Diagnostics;
using System.Drawing;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The Gantt chart rendering class.
    /// </summary>
    internal class GanttRenderer : BarRenderer
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
                return 2;
            }
        }

        /// <summary>
        /// Get description of regions.
        /// </summary>
        /// <value></value>
        protected override string RegionDescription
        {
            get
            {
                return "Gantt Chart Region";
            }
        }
        #endregion

        #region Coonstructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GanttRenderer"/> class.
        /// </summary>
        /// <param name="series">The ChartSeries.</param>
        public GanttRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics g)
        {
            ChartGanttConfigItem configItem = m_series.ConfigItems.GanttItem;
            DoubleRange sbsInfo = DoubleRange.Empty;

            if (configItem.DrawMode == ChartGanttDrawMode.CustomPointWidthMode)
            {
                sbsInfo = new DoubleRange(0, this.GetMinPointsDelta());
                sbsInfo = DoubleRange.Offset(sbsInfo, -sbsInfo.Delta / 2);
                sbsInfo = DoubleRange.Scale(sbsInfo, 0.01 * (100 - Chart.Spacing));
            }
            else
            {
                sbsInfo = this.GetSideBySideRange();
            }

            int serIndex = Chart.Series.IndexOf(m_series);
            bool series3D = Chart.Series3D;
            SizeF offset = GetSeriesOffset();
            PointF offsetSeries = GetThisOffset().ToPointF();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            int i = 0, mi = styledPoints.Length, di = 1;

            if (YAxis.Inversed)
            {
                di = -1;
                i = mi - 1;
                mi = -1;
            }

            for (; i != mi; i += di)
            {
                ChartStyledPoint styledPoint = styledPoints[i];
                ChartPoint ctPoint = m_series.Points[i];

                if (styledPoint.IsVisible)
                {
                    ChartStyleInfo style = styledPoint.Style;

                    styledPoint.X = Math.Min(Math.Max(styledPoint.X, m_series.ActualXAxis.Range.Min), m_series.ActualXAxis.Range.Max);
                    styledPoint.YValues[0] = Math.Min(Math.Max(styledPoint.YValues[0], m_series.ActualYAxis.Range.Min), m_series.ActualYAxis.Range.Max);
                    styledPoint.YValues[1] = Math.Min(Math.Max(styledPoint.YValues[1], m_series.ActualYAxis.Range.Min), m_series.ActualYAxis.Range.Max);
                    #region Prepeare coordinates

                    double x1 = styledPoint.Point.X;
                    double x2 = styledPoint.Point.X;
                    double y1 = styledPoint.Point.YValues[0];
                    double y2 = styledPoint.Point.YValues[1];
                  
                    if (configItem.DrawMode == ChartGanttDrawMode.CustomPointWidthMode)
                    {
                        DoubleRange ptSbsInfo = DoubleRange.Scale(sbsInfo, style.PointWidth);

                        x1 += ptSbsInfo.Start;
                        x2 += ptSbsInfo.End;
                    }
                    else
                    {
                        x1 += sbsInfo.Start;
                        x2 += sbsInfo.End;
                    }
                    #endregion
                    x2 = Math.Min(Math.Max(x2, m_series.ActualXAxis.Range.Min), m_series.ActualXAxis.Range.Max);
                    RectangleF rc = this.GetRectangle(new ChartPoint(x1, y1), new ChartPoint(x2, y2));

                    if (Chart.Indexed)
                    {
                        double xx1, xx2;                                                 
                        xx1 = xx2 = styledPoint.X;
                        ChartRenderArgs2D args = new ChartRenderArgs2D(Chart, m_series);
                        if (configItem.DrawMode == ChartGanttDrawMode.CustomPointWidthMode)
                        {
                            DoubleRange ptSbsInfo2 = DoubleRange.Scale(sbsInfo, style.PointWidth);
                            xx1 += ptSbsInfo2.Start;
                            xx2 += ptSbsInfo2.End;
                        }
                        else
                        {
                            this.CalculateSides(styledPoint, sbsInfo, out xx1, out xx2);
                        }

                        rc = args.GetRectangle(xx1, y1, xx2, y2);
                    }
                  
                    if (Chart.NeedRegionUpdate)
                    {
                        this.Chart.ChartRegions.Add(new ChartRegion(new Region(rc), serIndex,
                            styledPoint.Index, styledPoint.ToolTip, this.RegionDescription));
                    }

                    if (series3D)
                    {
                        this.Draw3DRectangle(g, rc, offset, GetBrush(i), style.GdipPen);
                    }
                    else
                    {
                        if (style.DisplayShadow)
                        {
                            RectangleF shadow = rc;
                            shadow.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);
                            BrushPaint.FillRectangle(g, shadow, style.ShadowInterior);
                        }

                        BrushPaint.FillRectangle(g, rc, GetBrush(i));
                        g.DrawRectangle(style.GdipPen, rc.X, rc.Y, rc.Width, rc.Height);
                    }

                    if (style.RelatedPoints.Points != null)
                    {
                        for (int j = 0; j < style.RelatedPoints.Count; j++)
                        {
                            PointF pt = this.GetPointFromIndex(style.RelatedPoints.Points[j]);
                            PointF[] connectionLine = GetConnectionLine(this.GetPointFromIndex(i, 1), pt, this.DividedIntervalSpace.Height / 2f);
                            g.DrawLines(style.RelatedPoints.GdipPen, connectionLine);

                            if (style.RelatedPoints.StartSymbol != null)
                            {
                                RenderingHelper.DrawRelatedPointSymbol(g, style.RelatedPoints.StartSymbol, style.RelatedPoints.Border, style.Images, connectionLine[0]);
                            }

                            if (style.RelatedPoints.EndSymbol != null)
                            {
                                RenderingHelper.DrawRelatedPointSymbol(g, style.RelatedPoints.EndSymbol, style.RelatedPoints.Border, style.Images, connectionLine[connectionLine.Length - 1]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics3D g)
        {
            ChartGanttConfigItem configItem = m_series.ConfigItems.GanttItem;
            DoubleRange sbsInfo = DoubleRange.Empty;

            if (configItem.DrawMode == ChartGanttDrawMode.CustomPointWidthMode)
            {
                sbsInfo = new DoubleRange(0, this.GetMinPointsDelta());
                sbsInfo = DoubleRange.Offset(sbsInfo, -sbsInfo.Delta / 2);
                sbsInfo = DoubleRange.Scale(sbsInfo, 0.01 * (100 - Chart.Spacing));
            }
            else
            {
                sbsInfo = this.GetSideBySideRange();
            }

            int serIndex = Chart.Series.IndexOf(m_series);
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            int i = 0, mi = styledPoints.Length, di = 1;
            float fd = this.GetPlaceDepth();
            float bd = this.GetSeriesDepth() + fd;

            if (YAxis.Inversed)
            {
                di = -1;
                i = mi - 1;
                mi = -1;
            }

            for (; i != mi; i += di)
            {
                ChartStyledPoint styledPoint = styledPoints[i];
                ChartPoint ctPoint = m_series.Points[i];

                if (this.IsVisiblePoint(ctPoint))
                {
                    ChartStyleInfo style = styledPoint.Style;

                    #region Prepeare coordinates
                    double x1 = styledPoint.Point.X;
                    double x2 = styledPoint.Point.X;
                    double y1 = styledPoint.Point.YValues[0];
                    double y2 = styledPoint.Point.YValues[1];

                    if (configItem.DrawMode == ChartGanttDrawMode.CustomPointWidthMode)
                    {
                        DoubleRange ptSbsInfo = DoubleRange.Scale(sbsInfo, -0.5 * style.PointWidth);

                        x1 += ptSbsInfo.Start;
                        x2 += ptSbsInfo.End;
                    }
                    else
                    {
                        x1 += sbsInfo.Start;
                        x2 += sbsInfo.End;
                    }
                    #endregion                   

                    RectangleF rc = this.GetRectangle(new ChartPoint(x1, y1), new ChartPoint(x2, y2));

                    if (Chart.Indexed)
                    {
                        double xx1, xx2;
                        xx1 = xx2 = styledPoint.X;
                        ChartRenderArgs2D args = new ChartRenderArgs2D(Chart, m_series);
                        if (configItem.DrawMode == ChartGanttDrawMode.CustomPointWidthMode)
                        {
                            DoubleRange ptSbsInfo2 = DoubleRange.Scale(sbsInfo, style.PointWidth);
                            xx1 += ptSbsInfo2.Start;
                            xx2 += ptSbsInfo2.End;
                        }
                        else
                        {
                            this.CalculateSides(styledPoint, sbsInfo, out xx1, out xx2);
                        }

                        rc = args.GetRectangle(xx1, y1, xx2, y2);
                    }

                    Polygon[] plgs = g.CreateBox(new Vector3D(rc.Left, rc.Top, fd), new Vector3D(rc.Right, rc.Bottom, bd), style.GdipPen, GetBrush(i));

                    if (Chart.NeedRegionUpdate)
                    {
                        ChartRegionData crd = new ChartRegionData(serIndex, styledPoint.Index, styledPoint.ToolTip, this.RegionDescription);

                        foreach (Polygon pl in plgs)
                        {
                            pl.RegionData = crd;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the connection line.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the PointF array.</returns>
        private PointF[] GetConnectionLine(PointF from, PointF to, float offset)
        {
            PointF[] ret = new PointF[]
      {
        from,
        new PointF( from.X + 10, from.Y ),
        new PointF( from.X + 10, to.Y - ( ( to.Y > from.Y ) ? offset : -offset ) ),
        new PointF( to.X - 10, to.Y - ( ( to.Y > from.Y ) ? offset : -offset ) ),
        new PointF( to.X - 10, to.Y ),
        to
      };

            return ret;
        }
        #endregion
    }
}