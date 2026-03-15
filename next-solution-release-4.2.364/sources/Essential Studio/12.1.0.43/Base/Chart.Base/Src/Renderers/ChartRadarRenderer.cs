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
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The Radar chart renderering class.
    /// </summary>
    internal class RadarRenderer : ChartSeriesRenderer
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
        /// Get description of regions.
        /// </summary>
        /// <value></value>
        protected override string RegionDescription
        {
            get
            {
                return "Radar Chart Renderer";
            }
        }
        #endregion

        #region Construector
        /// <summary>
        /// Initializes a new instance of the <see cref="RadarRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public RadarRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Renders chart by the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs2D args)
        {
            int serIndex = args.SeriesIndex;
            int yIndex = args.Series.PointFormats[ChartYValueUsage.YValue];
            ChartRadarConfigItem radarItem = m_series.ConfigItems.RadarItem;
            ChartStyledPoint[] styledPoint = this.PrepearePoints();
            PointF[] visiblePoints = new PointF[styledPoint.Length];
            ChartStyleInfo seriesStyle = this.SeriesStyle;
            BrushInfo interior = this.GetBrush();

            for (int i = 0; i < styledPoint.Length; i++)
            {
                if (styledPoint[i].IsVisible)
                {
                    visiblePoints[i] = this.GetPointFromValue(styledPoint[i].Point);
                }
                else
                {
                    visiblePoints[i] = this.ChartArea.Center;
                }
            }

            if (this.Chart.NeedRegionUpdate)
            {
                for (int i = 0; i < visiblePoints.Length; i++)
                {
                    Region rgn = this.GetRegionFromCircle(visiblePoints[i], styledPoint[i].Style.HitTestRadius);
                    this.ChartArea.ChartRegions.Add(new ChartRegion(rgn, serIndex, i, styledPoint[i].ToolTip, this.RegionDescription));
                }
            }

            if (visiblePoints.Length > 0)
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(visiblePoints);
                gp.CloseAllFigures();

                switch (radarItem.Type)
                {
                    case ChartRadarDrawType.Area:
                        {
                            if (seriesStyle.DisplayShadow)
                            {
                                GraphicsPath shadow = (GraphicsPath)gp.Clone();
                                Matrix translateMatrix = new Matrix();
                                translateMatrix.Translate(seriesStyle.ShadowOffset.Width, seriesStyle.ShadowOffset.Height);
                                shadow.Transform(translateMatrix);
                                args.Graph.DrawPath(seriesStyle.ShadowInterior, null, shadow);
                            }

                            args.Graph.DrawPath(interior, seriesStyle.GdipPen, gp);

                            if (this.Chart.NeedRegionUpdate)
                            {
                                this.ChartArea.ChartRegions.Add(new ChartRegion(new Region(gp),
                                    serIndex, GetToolTip(), this.RegionDescription));
                            }

                            break;
                        }

                    case ChartRadarDrawType.Line:
                        {
                            if (seriesStyle.DisplayShadow)
                            {
                                GraphicsPath shadow = (GraphicsPath)gp.Clone();
                                Matrix translateMatrix = new Matrix();
                                translateMatrix.Translate(seriesStyle.ShadowOffset.Width, seriesStyle.ShadowOffset.Height);
                                shadow.Transform(translateMatrix);
                                args.Graph.DrawPath(seriesStyle.ShadowInterior, null, shadow);
                            }

                            args.Graph.DrawPath(seriesStyle.GdipPen, gp);
                            break;
                        }

                    case ChartRadarDrawType.Symbol:
                        {
                            for (int i = 0; i < styledPoint.Length; i++)
                            {
                                ChartStyledPoint sPoint = styledPoint[i];

                                if (sPoint.Style.Symbol.Shape == ChartSymbolShape.None)
                                {
                                    GraphicsPath sblgp = new GraphicsPath();
                                    PointF ptF = this.GetSymbolPoint(sPoint);
                                    Size sz = sPoint.Style.Symbol.Size;

                                    sblgp.AddEllipse(ptF.X - sz.Width / 2, ptF.Y - sz.Height / 2, sz.Width, sz.Height);
                                    args.Graph.DrawPath(this.GetBrush(sPoint.Index), sPoint.Style.Border.GdipPen, sblgp);

                                    this.AddSymbolRegion(sPoint);
                                }
                            }
                        }

                        break;
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
            float dpth = ChartArea.Depth / PlaceSize;
            float serDpth = Place * dpth;
            ChartRadarConfigItem configItem = m_series.ConfigItems.RadarItem;
            int serIndex = Chart.Series.IndexOf(m_series);
            ChartStyleInfo style = m_series.GetOfflineStyle();

            Vector3D[] points = new Vector3D[m_series.Points.Count];

            for (int i = 0; i < m_series.Points.Count; i++)
            {
                PointF pt = GetPointFromIndex(i);
                points[i] = new Vector3D(pt.X, pt.Y, serDpth);
            }

            Polygon plg = null;

            switch (configItem.Type)
            {
                case ChartRadarDrawType.Area:
                    {                        
                        plg = new Polygon(points, this.GetBrush(), style.GdipPen);

                        if (Chart.NeedRegionUpdate)
                        {
                            string s = GetToolTip();
                            plg.RegionData = new ChartRegionData(serIndex, s, "Radar Chart	Region");
                        }

                        break;
                    }

                case ChartRadarDrawType.Line:
                    {
                        plg = new Polygon(points, style.GdipPen);
                        break;
                    }

                case ChartRadarDrawType.Symbol:
                    {
                        for (int i = 0; i < m_series.Points.Count; i++)
                        {
                            ChartStyleInfo ptStyle = this.GetStyleAt(i);

                            if (ptStyle.Symbol.Shape == ChartSymbolShape.None)
                            {
                                GraphicsPath ptgp = new GraphicsPath();
                                Vector3D pt = points[i];
                                Size sz = ptStyle.Symbol.Size;

                                ptgp.AddEllipse((float)(pt.X - sz.Width / 2), (float)(pt.Y - sz.Height / 2), sz.Width, sz.Height);
                                                                
                                Path3D p3d = Path3D.FromGraphicsPath(ptgp, pt.Z, this.GetBrush(), ptStyle.GdipPen);
                                p3d.RegionData = new ChartRegionData(Chart.Series.IndexOf(m_series), i, GetToolTip(i), "Symbol");
                                g.AddPolygon(p3d);
                            }
                        }
                    }

                    break;
            }

            g.AddPolygon(plg);
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
        /// Draws the icon on the legend.
        /// </summary>
        /// <param name="g">Instance of <see cref="Graphics"/>.</param>
        /// <param name="bounds">Bounds of icon.</param>
        /// <param name="isShadow">If is true method draws the shadow.</param>
        /// <param name="shadowColor"><see cref="Color"/> of shadow.</param>
        public override void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
        {
            if (isShadow)
            {
                using (SolidBrush br = new SolidBrush(shadowColor))
                {
                    g.FillRectangle(br, bounds);
                }
            }
            else
            {
                BrushInfo brushInfo = this.SeriesStyle.Interior;
                ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

                if (Chart.Model.ColorModel.AllowGradient)
                {
                    if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
                    {
                        brushInfo = this.GetPhongInterior(brushInfo, config.LightColor, config.LightAngle, config.PhongAlpha);
                    }
                }

                BrushPaint.FillRectangle(g, bounds, brushInfo);
                g.DrawRectangle(SeriesStyle.GdipPen, bounds);
            }
        }
        #endregion
    }
}
