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
    /// The HiLoOpenClose Renderering class.
    /// </summary>
    internal class HiLoOpenCloseRenderer : HiLoRenderer
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
                return "HiLo Chart  Region";
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
                return 4;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HiLoOpenCloseRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public HiLoOpenCloseRenderer(ChartSeries series)
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
            bool rightToLeft = m_series.ActualXAxis.Inversed;

            SizeF seriesOffset = this.GetThisOffset();
            SizeF depthOffset = this.GetSeriesOffset();
            DoubleRange sbsInfo = this.GetSideBySideInfo();

            ChartOpenCloseDrawMode drawMode = m_series.ConfigItems.HiLoOpenCloseItem.DrawMode;
            Color openWingColor = m_series.ConfigItems.HiLoOpenCloseItem.OpenTipColor;
            Color closeWingColor = m_series.ConfigItems.HiLoOpenCloseItem.CloseTipColor;

            bool drawOpenWing = drawMode == ChartOpenCloseDrawMode.Open || drawMode == ChartOpenCloseDrawMode.Both;
            bool drawCloseWing = drawMode == ChartOpenCloseDrawMode.Close || drawMode == ChartOpenCloseDrawMode.Both;

            IndexRange indexedRange = this.CalculateVisibleRange();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (this.IsVisiblePoint(styledPoints[i].Point))
                {
                    ChartPoint pt = new ChartPoint(0, styledPoints[i].Point.YValues);
                    ChartRegionData crd = new ChartRegionData(seriesIndex, styledPoint.Index, styledPoint.ToolTip, this.RegionDescription);

                    GraphicsPath openGp = new GraphicsPath();
                    GraphicsPath closeGp = new GraphicsPath();
                    BrushInfo baseInterior = new BrushInfo(styledPoint.Style.Interior);

                    #region Draw open line
                    if (drawOpenWing)
                    {
                        pt.X = styledPoints[i].Point.X + sbsInfo.Median;
                        PointF ptf1o = this.GetPointFromValue(pt, 2);

                        pt.X = styledPoints[i].Point.X + sbsInfo.Start;
                        PointF ptf2o = this.GetPointFromValue(pt, 2);

                        if (is3d)
                        {
                            GraphicsPath gp = new GraphicsPath();

                            ptf1o = ChartMath.AddPoint(ptf1o, seriesOffset);
                            ptf2o = ChartMath.AddPoint(ptf2o, seriesOffset);

                            gp.AddLine(ptf1o, ChartMath.AddPoint(ptf1o, depthOffset));
                            gp.AddLine(ChartMath.AddPoint(ptf2o, depthOffset), ptf2o);
                            gp.CloseFigure();

                            if (openWingColor != Color.Empty)
                                styledPoint.Style.Interior = new BrushInfo(openWingColor);

                            BrushPaint.FillPath(g, gp, this.GetBrush(styledPoint.Index));
                            styledPoint.Style.Interior = baseInterior; 
                            openGp = gp;

                            if (needUpdateRegion)
                            {
                                this.Chart.ChartRegions.Add(new ChartRegion(new Region(gp), crd));
                            }
                        }
                        else
                        {
                            using (Pen linePen = styledPoint.Style.GdipPen.Clone() as Pen)
                            {
                                if (styledPoint.Style.DisplayShadow)
                                {
                                    Size sdwOffset = styledPoint.Style.ShadowOffset;
                                    PointF swpt1 = ChartMath.AddPoint(ptf1o, sdwOffset);
                                    PointF swpt2 = ChartMath.AddPoint(ptf2o, sdwOffset);

                                    linePen.Color = styledPoint.Style.ShadowInterior.BackColor;
                                    g.DrawLine(linePen, swpt1, swpt2);
                                }

                                linePen.Color = this.GetBrush(styledPoint.Index).BackColor;
                                if (openWingColor != Color.Empty)
                                    linePen.Color = openWingColor;
                                g.DrawLine(linePen, ptf1o, ptf2o);
                            }
                        }
                    }
                    #endregion

                                        
                    #region Draw close line
                    if (drawCloseWing)
                    {
                        pt.X = styledPoints[i].Point.X + sbsInfo.Median;
                        PointF ptf1c = this.GetPointFromValue(pt, 3);

                        pt.X = styledPoints[i].Point.X + sbsInfo.End;
                        PointF ptf2c = this.GetPointFromValue(pt, 3);

                        if (is3d)
                        {
                            GraphicsPath gp = new GraphicsPath();
                            ptf1c = ChartMath.AddPoint(ptf1c, seriesOffset);
                            ptf2c = ChartMath.AddPoint(ptf2c, seriesOffset);

                            gp.AddLine(ptf1c, ChartMath.AddPoint(ptf1c, depthOffset));
                            gp.AddLine(ChartMath.AddPoint(ptf2c, depthOffset), ptf2c);
                            gp.CloseFigure();

                            if (closeWingColor != Color.Empty)
                                styledPoint.Style.Interior = new BrushInfo(closeWingColor);

                            BrushPaint.FillPath(g, gp, this.GetBrush(styledPoint.Index));
                            styledPoint.Style.Interior = baseInterior; 
                            closeGp = gp;

                            if (needUpdateRegion)
                            {
                                this.Chart.ChartRegions.Add(new ChartRegion(new Region(gp), crd));
                            }
                        }
                        else
                        {
                            using (Pen linePen = styledPoint.Style.GdipPen.Clone() as Pen)
                            {
                                if (styledPoint.Style.DisplayShadow)
                                {
                                    Size sdwOffset = styledPoint.Style.ShadowOffset;
                                    PointF swpt1 = ChartMath.AddPoint(ptf1c, sdwOffset);
                                    PointF swpt2 = ChartMath.AddPoint(ptf2c, sdwOffset);

                                    linePen.Color = styledPoint.Style.ShadowInterior.BackColor;
                                    g.DrawLine(linePen, swpt1, swpt2);
                                }

                                linePen.Color = this.GetBrush(styledPoint.Index).BackColor;
                                if (closeWingColor != Color.Empty)
                                    linePen.Color = closeWingColor;
                                g.DrawLine(linePen, ptf1c, ptf2c);
                            }
                        }
                    }
                    #endregion                  

                    GraphicsPath startPath = rightToLeft ? closeGp : openGp;
                    if (startPath == openGp)
                        styledPoint.Style.Interior = new BrushInfo(openWingColor);
                    else
                        styledPoint.Style.Interior = new BrushInfo(openWingColor);
                    g.DrawPath(styledPoint.Style.GdipPen, startPath);
                    BrushPaint.FillPath(g, startPath, this.GetBrush(styledPoint.Index));
                    styledPoint.Style.Interior = baseInterior;

                    #region Draw hi-lo line
                    pt.X = styledPoints[i].Point.X + sbsInfo.Median;

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
                            this.Chart.ChartRegions.Add(new ChartRegion(new Region(gp), crd));
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
                        this.Chart.ChartRegions.Add(new ChartRegion(this.GetRegionFromCircle(ptf1, styledPoint.Style.HitTestRadius), crd));
                        this.Chart.ChartRegions.Add(new ChartRegion(this.GetRegionFromCircle(ptf2, styledPoint.Style.HitTestRadius), crd));
                    }
                    #endregion                  

                    GraphicsPath endPath = rightToLeft ? openGp : closeGp;
                    if (endPath == closeGp)
                        styledPoint.Style.Interior = new BrushInfo(closeWingColor);
                    else
                        styledPoint.Style.Interior = new BrushInfo(openWingColor);
                    g.DrawPath(styledPoint.Style.GdipPen, endPath);
                    BrushPaint.FillPath(g, endPath, this.GetBrush(styledPoint.Index));
                    styledPoint.Style.Interior = baseInterior;                                                                          
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
            ChartOpenCloseDrawMode drawMode = m_series.ConfigItems.HiLoOpenCloseItem.DrawMode;

            bool drawOpenWing = drawMode == ChartOpenCloseDrawMode.Open || drawMode == ChartOpenCloseDrawMode.Both;
            bool drawCloseWing = drawMode == ChartOpenCloseDrawMode.Close || drawMode == ChartOpenCloseDrawMode.Both;

            IndexRange indexedRange = this.CalculateVisibleRange();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();

            Polygon[] cPlgs = new Polygon[indexedRange.To - indexedRange.From + 1];
            Polygon[] oPlgs = new Polygon[indexedRange.To - indexedRange.From + 1];

            float fd = this.GetPlaceDepth();
            float bd = fd + this.GetSeriesDepth();

            g.AddPolygon(CreateBoundsPolygon(fd));

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (this.IsVisiblePoint(styledPoints[i].Point))
                {
                    ChartPoint pt = new ChartPoint(styledPoints[i].Point.X, styledPoints[i].Point.YValues);
                    ChartRegionData crd = new ChartRegionData(seriesIndex, styledPoint.Index, styledPoint.ToolTip, this.RegionDescription);

                    #region Draw hi-lo line
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
                        plg.RegionData = crd;
                    }
                    #endregion

                    #region Draw open line
                    if (drawOpenWing)
                    {
                        pt.X = styledPoints[i].Point.X + sbsInfo.Median;
                        PointF ptf1o = this.GetPointFromValue(pt, 2);

                        pt.X = styledPoints[i].Point.X + sbsInfo.Start;
                        PointF ptf2o = this.GetPointFromValue(pt, 2);

                        Vector3D[] verticesO = new Vector3D[]{ new Vector3D( ptf1o.X, ptf1o.Y, fd ),
							new Vector3D( ptf2o.X, ptf2o.Y, fd ),
							new Vector3D( ptf2o.X, ptf2o.Y, bd ),
							new Vector3D( ptf1o.X, ptf1o.Y, bd ) };

                        Polygon plgO = new Polygon(verticesO, this.GetBrush(styledPoint.Index), styledPoint.Style.GdipPen);
                        oPlgs[i] = plgO;

                        if (needUpdateRegion)
                        {
                            plgO.RegionData = crd;
                        }
                    }
                    #endregion

                    #region Draw close line
                    if (drawCloseWing)
                    {
                        pt.X = styledPoints[i].Point.X + sbsInfo.Median;
                        PointF ptf1c = this.GetPointFromValue(pt, 3);

                        pt.X = styledPoints[i].Point.X + sbsInfo.End;
                        PointF ptf2c = this.GetPointFromValue(pt, 3);

                        Vector3D[] verticesC = new Vector3D[]{ new Vector3D( ptf1c.X, ptf1c.Y, fd ),
							new Vector3D( ptf2c.X, ptf2c.Y, fd ),
							new Vector3D( ptf2c.X, ptf2c.Y, bd ),
							new Vector3D( ptf1c.X, ptf1c.Y, bd ) };

                        Polygon plgC = new Polygon(verticesC, this.GetBrush(styledPoint.Index), styledPoint.Style.GdipPen);
                        cPlgs[i] = plgC;

                        if (needUpdateRegion)
                        {
                            plgC.RegionData = crd;
                        }
                    }
                    #endregion
                }
            }

            for (int i = 0; i < cPlgs.Length; i++)
            {
                if (oPlgs[i] != null) g.AddPolygon(oPlgs[i]);
                if (cPlgs[i] != null) g.AddPolygon(cPlgs[i]);
            }
        }
        #endregion
    }
}