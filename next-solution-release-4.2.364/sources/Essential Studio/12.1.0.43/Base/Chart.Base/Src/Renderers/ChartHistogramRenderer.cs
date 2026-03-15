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
    /// The Histogram Renderering class.
    /// </summary>
    internal class HistogramRenderer : ChartSeriesRenderer
    {
        #region Members
        private int m_numberOfNormalDistributionPoints = 500;
        private double m_dotWidthDivideFactor = 80.0d;
        #endregion

        #region Properties
        /// <summary>
        /// Gets count of require Y values of the points.
        /// </summary>
        /// <value></value>
        protected override int RequireYValuesCount
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HistogramRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public HistogramRenderer(ChartSeries series)
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
            ChartHistogramConfigItem config = m_series.ConfigItems.HistogramItem;

            double axisFactor = (m_series.ActualXAxis.RealLength * m_series.ActualYAxis.VisibleRange.Delta) / (m_series.ActualXAxis.VisibleRange.Delta * m_series.ActualYAxis.RealLength);
            double axisWidth = m_series.ActualXAxis.VisibleRange.Delta;
            int serCount = Chart.Series.VisibleCount;
            SizeF offset = GetSeriesOffset();
            SizeF serOffset = GetThisOffset();
            int serIndex = Chart.Series.IndexOf(m_series);
            bool series3D = Chart.Series3D;
            bool dropPoints = Chart.DropSeriesPoints;
            ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[m_series.Points.Count];
            ChartRenderArgs2D args = new ChartRenderArgs2D(Chart, m_series);
            args.Graph = new ChartGDIGraph(g);

            for (int i = 0, end = m_series.Points.Count; i < end; i++)
            {
                cpwiA[i] = new ChartPointWithIndex(m_series.Points[i], i);
            }

            if (dropPoints || Chart.EnableXZooming)
                Array.Sort(cpwiA, new ComparerPointWithIndexByX());

            ArrayList pathsList = new ArrayList(m_series.Points.Count);
            PointF lastAccPointF = PointF.Empty;

            double[] histogramIntervals, histogramValues;
            GetHistogramIntervalsValues(cpwiA, out histogramIntervals, out histogramValues);
            int numHistInt = histogramValues.Length;

            int pointsInd = 0;
            for (int i = 1; i <= numHistInt; i++)
            {
                double x1 = histogramIntervals[i - 1];
                double y1 = histogramValues[i - 1];
                double x2 = histogramIntervals[i];
                double y2 = 0;

                ChartPoint cpTopLeft = new ChartPoint(x1, y1);
                ChartPoint cpBottomRight = new ChartPoint(x2, y2);

                ////float m_height = Math.Abs( pt.Y - CustomOriginY );
                RectangleF rc = GetRectangle(cpTopLeft, cpBottomRight);
                ChartStyleInfo style = SeriesStyle;

                if (style.DisplayShadow && !series3D)
                {
                    RectangleF shadowRC = GetRectangle(cpTopLeft, cpBottomRight);
                    shadowRC.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);
                    BrushPaint.FillRectangle(g, shadowRC, style.ShadowInterior);
                }

                rc.X += serOffset.Width;
                rc.Y += serOffset.Height;

                GraphicsPath gp = null, gPathBoxRight = null, gPathBoxTop = null;
                gp = CreateBox(rc, series3D);
                if (args.Chart.Series3D)
                {
                    if (!args.Chart.Style3D)
                        gp = CreateBox(rc, true);
                    else
                    {
                        gp = this.CreateBox(rc, true);
                        gPathBoxRight = this.CreateBoxRight(rc, true);
                        gPathBoxTop = this.CreateBoxTop(rc, true);
                    }

                }

                ChartSeriesPath path = new ChartSeriesPath();
                 if (!args.Chart.Style3D)
                    path.AddPrimitive(gp, style.GdipPen, this.GetBrush());
                else
                {
                    path.AddPrimitive(gp, null, BrushInfo.Empty);

                    ChartSeriesPath csp1 = new ChartSeriesPath();
                    csp1.AddPrimitive(gp, style.GdipPen, this.GetBrush(), "BoxCenter");
                    csp1.Draw(args.Graph, "BoxCenter");

                    ChartSeriesPath csp2 = new ChartSeriesPath();
                    csp2.AddPrimitive(gPathBoxRight, style.GdipPen, this.GetBrush(), "BoxRight");
                    csp2.Draw(args.Graph, "BoxRight");

                    ChartSeriesPath csp3 = new ChartSeriesPath();
                    csp3.AddPrimitive(gPathBoxTop, style.GdipPen, this.GetBrush());
                    csp3.Draw(args.Graph, "BoxTop");
                }
                     
                path.Bounds = rc;
                path.RegionData = new ChartRegionData(serIndex, (i - 1), GetToolTip(), "Histogram Chart Region");

                if (series3D)
                {
                    pathsList.Add(path);
                }
                else
                {
                    path.Draw(g);
                    pathsList.Add(path);
                }
				if (config.ShowDataPoints)
               {
                ////drawing points on top of column
                double dotDiameter = axisWidth / m_dotWidthDivideFactor;
                double yDotDiameter = dotDiameter * axisFactor;
                double prevX = double.NaN;
                double dydd = yDotDiameter / 2.0;

                for (int j = pointsInd; j < cpwiA.Length; j++, pointsInd = j)
                {
                    int cpIndex = cpwiA[j].Index;
                    double x = cpwiA[j].Point.X;
                    ChartStyleInfo pstyle = GetStyleAt(cpIndex);

                    if ((x > x1) && (x <= x2))
                    {
                        ChartSeriesPath tgp = new ChartSeriesPath();
                        double dd = dotDiameter / 2.0;
                        double ydd = yDotDiameter;
                        if (x == prevX)
                            dydd += yDotDiameter;
                        else
                            dydd = yDotDiameter / 2.0;
                        ChartPoint wisCp1 = new ChartPoint(x - dd, y1 + dydd);
                        ChartPoint wisCp2 = new ChartPoint(x + dd, y1 + ydd + dydd);
                        RectangleF rectf = GetRectangle(wisCp1, wisCp2);
                        rectf.X += serOffset.Width;
                        rectf.Y += serOffset.Height;
                        GraphicsPath tgp2 = new GraphicsPath();
                        tgp2.AddEllipse(rectf);
                        tgp.AddPrimitive(tgp2, pstyle.GdipPen, GetBrush(cpIndex));
                        tgp.Bounds = rectf;

                        string s = GetToolTip(cpIndex);
                        if (Chart.NeedRegionUpdate)
                            tgp.RegionData = new ChartRegionData(serIndex, cpIndex, s, "Histogram Chart Region");

                        if (series3D)
                            pathsList.Add(tgp);
                        else
                        {
                            tgp.Draw(g);

                            if (Chart.NeedRegionUpdate)
                            {
                                ChartArea.ChartRegions.Add(tgp.GetChartRegion());
                            }
                        }
                    }
                    else
                    {
                        if ((x > x2))
                            break;
                    }

                    prevX = x;
                }
			}
            }

            #region drawing Normal distribution
            if (config.ShowNormalDistribution)
            {
                double m, dev;
                GetHistogramMeanAndDeviation(cpwiA, out m, out dev);

                double min = m_series.ActualXAxis.Range.Min;
                double max = m_series.ActualXAxis.Range.Max;
                double del = (max - min) / (m_numberOfNormalDistributionPoints - 1);
                PointF[] pa = new PointF[m_numberOfNormalDistributionPoints];

                for (int i = 0; i < m_numberOfNormalDistributionPoints; i++)
                {
                    double tx = min + i * del;
                    double ty = NormalDistribution(tx, m, dev) * cpwiA.Length * (histogramIntervals[1] - histogramIntervals[0]);
                    ////double ty = NormalDistribution( tx, m, dev ) * ( histogramIntervals[1] - histogramIntervals[0] );
                    ChartPoint cp = new ChartPoint(tx, ty);
                    pa[i] = new PointF(GetXFromValue(cp, 0) + serOffset.Width, GetYFromValue(cp, 0) + serOffset.Height);
                }

                GraphicsPath gpn = new GraphicsPath();
                gpn.AddLines(pa);

                g.DrawPath(SeriesStyle.GdipPen, gpn);
            }
            #endregion

            m_segments = (ChartSeriesPath[])pathsList.ToArray(typeof(ChartSeriesPath));
        }

        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics3D g)
        {
            int serIndex = Chart.Series.IndexOf(m_series);
            double axisFactor = (m_series.ActualXAxis.RealLength * m_series.ActualYAxis.VisibleRange.Delta) / (m_series.ActualXAxis.VisibleRange.Delta * m_series.ActualYAxis.RealLength);
            double axisWidth = m_series.ActualXAxis.VisibleRange.Delta;
            bool dropPoints = Chart.DropSeriesPoints;
            float fd = GetPlaceDepth();
            float dpth = GetSeriesDepth();
            float serDpth = fd;

            g.AddPolygon(CreateBoundsPolygon(serDpth));

            ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[m_series.Points.Count];
            for (int i = 0, end = m_series.Points.Count; i < end; i++)
            {
                cpwiA[i] = new ChartPointWithIndex(m_series.Points[i], i);
            }

            if (dropPoints || Chart.EnableXZooming)
                Array.Sort(cpwiA, new ComparerPointWithIndexByX());

            ArrayList pathsList = new ArrayList(m_series.Points.Count);
            PointF lastAccPointF = PointF.Empty;

            double[] histogramIntervals, histogramValues;
            GetHistogramIntervalsValues(cpwiA, out histogramIntervals, out histogramValues);
            int numHistInt = histogramValues.Length;

            int pointsInd = 0;
            for (int i = 1; i <= numHistInt; i++)
            {
                double x1 = histogramIntervals[i - 1];
                double y1 = histogramValues[i - 1];
                double x2 = histogramIntervals[i];
                double y2 = 0;

                ChartPoint cpTopLeft = new ChartPoint(x1, y1);
                ChartPoint cpBottomRight = new ChartPoint(x2, y2);

                ////float m_height = Math.Abs( pt.Y - CustomOriginY );
                RectangleF rc = GetRectangle(cpTopLeft, cpBottomRight);
                ChartStyleInfo style = SeriesStyle;

                Polygon[] plgs;                
                                
                if (!m_series.Rotate)
                {
                    plgs = g.CreateBoxV(new Vector3D(rc.Left, rc.Top, serDpth),
                      new Vector3D(rc.Right, rc.Bottom, serDpth + dpth), style.GdipPen, this.GetBrush());
                }
                else
                {
                    plgs = g.CreateBox(new Vector3D(rc.Left, rc.Top, serDpth),
                      new Vector3D(rc.Right, rc.Bottom, serDpth + dpth), style.GdipPen, this.GetBrush());
                }                

                if (Chart.NeedRegionUpdate)
                {
                    for (int k = 0; k < plgs.Length; k++)
                    {
                        plgs[k].RegionData = new ChartRegionData(serIndex, (i - 1), GetToolTip(), "Histogram Chart Region");
                    }
                }
				if (m_series.ConfigItems.HistogramItem.ShowDataPoints)
               {
                ////drawing points on top of column
                double dotDiameter = axisWidth / m_dotWidthDivideFactor;
                double yDotDiameter = dotDiameter * axisFactor;
                double prevX = double.NaN;
                double dydd = yDotDiameter / 2.0;

                for (int j = pointsInd; j < cpwiA.Length; j++, pointsInd = j)
                {
                    int cpIndex = cpwiA[j].Index;
                    double x = cpwiA[j].Point.X;
                    ChartStyleInfo pstyle = GetStyleAt(cpIndex);

                    if ((x > x1) && (x <= x2))
                    {
                        double dd = dotDiameter / 2.0;
                        double ydd = yDotDiameter;

                        if (x == prevX)
                            dydd += yDotDiameter;
                        else
                            dydd = yDotDiameter / 2.0;

                        ChartPoint wisCp1 = new ChartPoint(x - dd, y1 + dydd);
                        ChartPoint wisCp2 = new ChartPoint(x + dd, y1 + ydd + dydd);
                        RectangleF rectf = GetRectangle(wisCp1, wisCp2);
                        Vector3D v = new Vector3D((double)rectf.X, (double)rectf.Y, serDpth);
                        Polygon[] plgns = g.CreateEllipse(v, rectf.Size, 10, pstyle.GdipPen, GetBrush(cpIndex));


                        if (Chart.NeedRegionUpdate)
                        {
                            string s = GetToolTip(cpIndex);
                            for (int k = 0; k < plgns.Length; k++)
                            {
                                plgns[k].RegionData = new ChartRegionData(serIndex, cpIndex, s, "Histogram Chart Region");
                            }
                        }
                    }
                    else
                    {
                        if ((x > x2))
                            break;
                    }

                    prevX = x;
                }
			}
            }

            #region drawing Normal distribution
            if (m_series.ConfigItems.HistogramItem.ShowNormalDistribution)
            {
                double m, dev;
                GetHistogramMeanAndDeviation(cpwiA, out m, out dev);

                double min = m_series.ActualXAxis.Range.Min;
                double max = m_series.ActualXAxis.Range.Max;
                double del = (max - min) / (m_numberOfNormalDistributionPoints - 1);
                Vector3D[] pa = new Vector3D[m_numberOfNormalDistributionPoints * 2];

                for (int i = 0; i < m_numberOfNormalDistributionPoints; i++)
                {
                    double tx = min + i * del;
                    double ty = NormalDistribution(tx, m, dev) * cpwiA.Length * (histogramIntervals[1] - histogramIntervals[0]);
                    ////double ty = NormalDistribution( tx, m, dev ) * ( histogramIntervals[1] - histogramIntervals[0] );
                    ChartPoint cp = new ChartPoint(tx, ty);
                    pa[i] = new Vector3D(GetXFromValue(cp, 0), GetYFromValue(cp, 0), serDpth);
                }

                for (int i = 1; i <= m_numberOfNormalDistributionPoints; i++)
                {
                    double tx = min + (m_numberOfNormalDistributionPoints - i) * del;
                    double ty = NormalDistribution(tx, m, dev) * cpwiA.Length * (histogramIntervals[1] - histogramIntervals[0]);
                    ////double ty = NormalDistribution( tx, m, dev ) * ( histogramIntervals[1] - histogramIntervals[0] );
                    ChartPoint cp = new ChartPoint(tx, ty);
                    pa[m_numberOfNormalDistributionPoints + i - 1] = new Vector3D(GetXFromValue(cp, 0), GetYFromValue(cp, 0), serDpth);
                }

                Polygon pal = new Polygon(pa, SeriesStyle.GdipPen);
                g.AddPolygon(pal);
            }
            #endregion
        }

        /// <summary>
        /// Normal Distribution function.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="m">The m.</param>
        /// <param name="sigma">The sigma.</param>
        /// <returns></returns>
        private double NormalDistribution(double x, double m, double sigma)
        {
            return Math.Exp(-(x - m) * (x - m) / (2 * sigma * sigma)) / (sigma * Math.Sqrt(2 * Math.PI));
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

        /// <summary>
        /// Measures the Y range.
        /// </summary>
        /// <returns></returns>
        public override DoubleRange GetYDataMeasure()
        {
            return new DoubleRange(0, this.GetHistogramMax());
        }
        #endregion

        #region Implementation

        #region Histogram Renderer Math
        /// <summary>
        /// Gets the histogram intervals values.
        /// </summary>
        /// <param name="cpwiA">The cpwi A.</param>
        /// <param name="histogramIntervals">The histogram intervals.</param>
        /// <param name="histogramValues">The histogram values.</param>
        public void GetHistogramIntervalsValues(ChartPointWithIndex[] cpwiA, out double[] histogramIntervals, out double[] histogramValues)
        {
            int count = cpwiA.Length;
            int numHistInt = m_series.ConfigItems.HistogramItem.NumberOfIntervals;
            histogramIntervals = new double[numHistInt + 1];
            histogramValues = new double[numHistInt];
            double interval = m_series.ActualXAxis.Range.Delta / numHistInt;
            double min = m_series.ActualXAxis.Range.Min;

            for (int i = 0; i <= numHistInt; i++)
                histogramIntervals[i] = min + interval * i;

            for (int i = 0; i < numHistInt; i++)
                histogramValues[i] = 0.0;

            double prevLowHistInt = histogramIntervals[0];
            int pointsInd = 0;
            for (int i = 1; i <= numHistInt; i++)
            {
                double HiHistInt = histogramIntervals[i];

                for (int j = pointsInd; j < count; j++, pointsInd = j)
                {
                    double x = cpwiA[j].Point.X;
                    if ((x > prevLowHistInt) && (x <= HiHistInt))
                    {
                        ////histogramValues[i-1] += 1.0/count;
                        histogramValues[i - 1] += 1.0;
                    }
                    else
                    {
                        if ((x > HiHistInt))
                        {
                            break;
                        }
                    }
                }

                prevLowHistInt = HiHistInt;
            }
        }

        /// <summary>
        /// Gets the maximal value of histogram.
        /// </summary>
        /// <returns></returns>
        public double GetHistogramMax()
        {
            ChartStyledPoint[] styledPoints = this.PrepearePoints();

            ChartPointWithIndex[] cpwiA = new ChartPointWithIndex[m_series.Points.Count];
            for (int i = 0, end = m_series.Points.Count; i < end; i++)
            {
                cpwiA[i] = new ChartPointWithIndex(m_series.Points[i], i);
            }

            Array.Sort(cpwiA, new ComparerPointWithIndexByX());

            ArrayList pathsList = new ArrayList(m_series.Points.Count);
            PointF lastAccPointF = PointF.Empty;

            double[] histogramIntervals, histogramValues;
            GetHistogramIntervalsValues(cpwiA, out histogramIntervals, out histogramValues);
            int numHistInt = histogramValues.Length;

            double max = 0;
            for (int i = 0; i < numHistInt; i++)
            {
                if (max < histogramValues[i])
                {
                    max = histogramValues[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Gets the histogram mean and deviation.
        /// </summary>
        /// <param name="cpwiA">The cpwi A.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="standartDeviation">The standart deviation.</param>
        public void GetHistogramMeanAndDeviation(ChartPointWithIndex[] cpwiA, out double mean, out double standartDeviation)
        {
            int count = cpwiA.Length;
            int numHistInt = m_series.ConfigItems.HistogramItem.NumberOfIntervals;

            double sum = 0;
            for (int i = 0; i < count; i++)
                sum += cpwiA[i].Point.X;

            mean = sum / count;

            sum = 0;
            for (int i = 0; i < count; i++)
            {
                double dif = cpwiA[i].Point.X - mean;
                sum += dif * dif;
            }

            standartDeviation = Math.Sqrt(sum / count);
        }
        #endregion
        #endregion
    }
}