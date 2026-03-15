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
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The Chart HiLo Renderering class.
    /// </summary>
    internal class HiLoRenderer : ChartSeriesRenderer
    {
        #region Properties
        /// <summary>
        /// Get description of regions.
        /// </summary>
        /// <value></value>
        protected override string RegionDescription
        {
            get
            {
                return "HiLo Line";
            }
        }

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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HiLoRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public HiLoRenderer(ChartSeries series)
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
            bool is3d = this.Chart.Series3D;
            int seriesIndex = Chart.Series.IndexOf(m_series);
            bool needUpdateRegion = Chart.NeedRegionUpdate;

            SizeF seriesOffset = this.GetThisOffset();
            SizeF depthOffset = this.GetSeriesOffset();
            DoubleRange sbsInfo = this.GetSideBySideInfo();

            IndexRange indexedRange = this.CalculateVisibleRange();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (this.IsVisiblePoint(styledPoints[i].Point))
                {
                    ChartPoint pt = new ChartPoint(styledPoints[i].Point.X, styledPoints[i].Point.YValues);

                    pt.X += sbsInfo.Median;

                    PointF ptf1 = this.GetPointFromValue(pt, 0);
                    PointF ptf2 = this.GetPointFromValue(pt, 1);

                    if (is3d)
                    {
                        GraphicsPath gp = new GraphicsPath();

                        ptf1 = ChartMath.AddPoint(ptf1, seriesOffset);
                        ptf2 = ChartMath.AddPoint(ptf2, seriesOffset);

                        gp.AddLine(ptf1, ChartMath.AddPoint(ptf1, depthOffset));
                        gp.AddLine(ChartMath.AddPoint(ptf2, depthOffset), ptf2);
                        gp.CloseFigure();

                        BrushPaint.FillPath(g, gp, this.GetBrush(styledPoint.Index));
                        g.DrawPath(styledPoint.Style.GdipPen, gp);

                        if (needUpdateRegion)
                        {
                            this.Chart.ChartRegions.Add(new ChartRegion(new Region(gp),
                                seriesIndex, styledPoint.Index, styledPoint.ToolTip, this.RegionDescription));
                        }
                    }
                    else
                    {
                        using (Pen linePen = styledPoint.Style.GdipPen.Clone() as Pen)
                        {
                            if (styledPoint.Style.DisplayShadow)
                            {
                                Size sdwOffset = styledPoint.Style.ShadowOffset;
                                PointF swpt1 = ChartMath.AddPoint(ptf1, sdwOffset);
                                PointF swpt2 = ChartMath.AddPoint(ptf2, sdwOffset);

                                linePen.Color = styledPoint.Style.ShadowInterior.BackColor;
                                g.DrawLine(linePen, swpt1, swpt2);
                            }

                            linePen.Color = this.GetBrush(styledPoint.Index).BackColor;
                            g.DrawLine(linePen, ptf1, ptf2);
                        }
                    }

                    if (needUpdateRegion)
                    {
                        ChartRegionData crd = new ChartRegionData(seriesIndex, styledPoint.Index, styledPoint.ToolTip, this.RegionDescription);

                        this.Chart.ChartRegions.Add(new ChartRegion(this.GetRegionFromCircle(ptf1, styledPoint.Style.HitTestRadius), crd));
                        this.Chart.ChartRegions.Add(new ChartRegion(this.GetRegionFromCircle(ptf2, styledPoint.Style.HitTestRadius), crd));
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
            int seriesIndex = Chart.Series.IndexOf(m_series);
            bool needUpdateRegion = Chart.NeedRegionUpdate;

            DoubleRange sbsInfo = this.GetSideBySideInfo();

            IndexRange indexedRange = this.CalculateVisibleRange();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();

            float fd = this.GetPlaceDepth();
            float bd = fd + this.GetSeriesDepth();

            g.AddPolygon(CreateBoundsPolygon(fd));

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (this.IsVisiblePoint(styledPoints[i].Point))
                {
                    ChartPoint pt = new ChartPoint(styledPoints[i].Point.X, styledPoints[i].Point.YValues);

                    pt.X += sbsInfo.Median;

                    PointF ptf1 = this.GetPointFromValue(pt, 0);
                    PointF ptf2 = this.GetPointFromValue(pt, 1);

                    Vector3D[] vertices = new Vector3D[]{ new Vector3D( ptf1.X, ptf1.Y, fd ),
						new Vector3D( ptf2.X, ptf2.Y, fd ),
						new Vector3D( ptf2.X, ptf2.Y, bd ),
						new Vector3D( ptf1.X, ptf1.Y, bd ) };

                    Polygon plg = new Polygon(vertices, this.GetBrush(styledPoint.Index), styledPoint.Style.GdipPen);
                    g.AddPolygon(plg);

                    if (needUpdateRegion)
                    {
                        plg.RegionData = new ChartRegionData(seriesIndex, styledPoint.Index,
                            styledPoint.ToolTip, this.RegionDescription);
                    }
                }
            }
        }

        /// <summary>
        /// Brush information is retrieved from the style associated with the index of the point to be rendered.
        /// It is then changed for special cases such as when automatic highlighting is enabled.
        /// </summary>
        /// <returns>
        /// Brush information that is to be used for filling elements displayed at this index.
        /// </returns>
        protected override BrushInfo GetBrush()
        {
            BrushInfo brushInfo = base.GetBrush();
            ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

            if (Chart.Model.ColorModel.AllowGradient)
            {
                if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
                {
                    brushInfo = this.GetPhongInterior(brushInfo, config.LightColor, config.LightAngle, config.PhongAlpha);
                }
            }

            return brushInfo;
        }

        /// <summary>
        /// Brush information is retrieved from the style associated with the index of the point to be rendered.
        /// It is then changed for special cases such as when automatic highlighting is enabled.
        /// </summary>
        /// <param name="index">Index value of the point for which the brush information is required.</param>
        /// <returns>
        /// Brush information that is to be used for filling elements displayed at this index.
        /// </returns>
        protected override BrushInfo GetBrush(int index)
        {
            BrushInfo brushInfo = base.GetBrush(index);
            ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

            if (Chart.Model.ColorModel.AllowGradient)
            {
                if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
                {
                    brushInfo = this.GetPhongInterior(brushInfo, config.LightColor, config.LightAngle, config.PhongAlpha);
                }
            }

            return brushInfo;
        }

        /// <summary>
        /// Draws chart's icon.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to render icon.</param>
        /// <param name="bounds">The icon bounds.</param>
        /// <param name="isShadow">The value indicates to draw shadow or not.</param>
        /// <param name="shadowColor">The <see cref="System.Drawing.Color"/> to render shadow.</param>
        public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
        {
            int x1 = bounds.X + bounds.Width / 3;
            int x2 = bounds.X + 2 * bounds.Width / 3;
            GraphicsPath gp = new GraphicsPath();

            gp.AddLines(new Point[]{
                                     new Point( bounds.X, bounds.Bottom ),
                                     new Point( x1, bounds.Top ),
                                     new Point( x2, bounds.Bottom ),
                                     new Point( bounds.Right, bounds.Top )
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
