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
    /// Represents the pyramid type renderer.
    /// </summary>
    internal class PyramidRenderer : FunnelRenderer
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PyramidRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public PyramidRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics g)
        {
            this.OnRender(g, m_series.ConfigItems.PyramidItem.ShowSeriesTitle);
        }

        /// <summary>
        /// Renders chart by the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            this.OnRender(args, m_series.ConfigItems.PyramidItem.ShowSeriesTitle);
        }

        /// <summary>
        /// Creates the layers and labels.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="g">The g.</param>
        /// <param name="drawingRect">The drawing rect.</param>
        protected override void CreateLayersAndLabels(ChartStyledPoint[] points, Graphics g, RectangleF drawingRect)
        {
            if (points.Length == 0)
                return;

            ChartPyramidMode pyramidMode = m_series.ConfigItems.PyramidItem.PyramidMode;

            bool series3D = Chart.Series3D;
            float offset3DRatio = (float)Math.Sin(ChartArea.Tilt * ChartMath.ToRadians);
            float rotateRatio = (float)((ChartArea.Rotation * Math.PI / 180 - Math.PI / 4) % Math.PI / 2);
            float circleBaseHeightIn3Dmode = (series3D && !Chart.RealMode3D) ? offset3DRatio * drawingRect.Width / 2 : 0;

            ////these vars, should be exposed as properties
            bool labelsRight = (m_series.ConfigItems.PyramidItem.LabelPlacement != ChartAccumulationLabelPlacement.Left);
            float gapRatio = m_series.ConfigItems.PyramidItem.GapRatio;
            ChartAccumulationLabelStyle lblStyle = m_series.ConfigItems.PyramidItem.LabelStyle;
            ChartAccumulationLabelPlacement lblPlacement = m_series.ConfigItems.PyramidItem.LabelPlacement;
            ChartFigureBase figureBase = m_series.ConfigItems.PyramidItem.FigureBase;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            float opposedLabels = labelsRight ? 1 : -1;

            double all = GetAllValue();
            int len = points.Length;
            int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
            double maxYVal = Math.Abs(points[points.Length - 1].Point.YValues[0]);
            m_layers = new ArrayList(len);
            m_labels = new ArrayList(len);

            double coTangens_max = drawingRect.Width / (2 * (drawingRect.Height - circleBaseHeightIn3Dmode));
            double coTangens_mid = drawingRect.Width / (4 * (drawingRect.Height - circleBaseHeightIn3Dmode));
            double coTangens_min = coTangens_max / 24;
            float xCenter = (opposedLabels + 1) / 2 * drawingRect.Left + (1 - opposedLabels) / 2 * drawingRect.Right +
                      (float)(opposedLabels * coTangens_max * (drawingRect.Height - circleBaseHeightIn3Dmode));

            ChartMath.DoubleFunc func = new ChartMath.DoubleFunc(OptimizationFunc_Pyramid);

            if (pyramidMode == ChartPyramidMode.Linear)
            {
                ////create layers
                float h_all = 0;
                for (int i = 0; i < len; i++)
                {
                    float h_i = (float)((drawingRect.Height - circleBaseHeightIn3Dmode) * Math.Abs(points[i].YValues[yIndex] / all));
                    PointF topCenterPoint = new PointF(xCenter, drawingRect.Top + h_all);
                    h_all += h_i;
                    AccumulationChartsLayer fl = new AccumulationChartsLayer(i, topCenterPoint, h_i, series3D, offset3DRatio, ChartFunnelMode.YIsHeight);
                    fl.Series = m_series;
                    fl.GapRatio = gapRatio;
                    fl.MinWidth = 0;
                    fl.FigureBase = figureBase;
                    fl.RotationRation = rotateRatio;
                    fl.DepthPosition = drawingRect.Width / 2;
                    m_layers.Add(fl);

                    ChartStyleInfo si = GetStyleAt(points[i].Index);
                    if ((si.Text.Length == 0) || !(si.DisplayText)) continue;
                    AccumulationChartsLabel lbl = new AccumulationChartsLabel(i, points[i].Point, si, fl, AccumulationChartsLabelAttachMode.Center, m_series, points[i].Index);
                    lbl.LabelPlacement = lblPlacement;
                    lbl.LabelStyle = lblStyle;
                    m_labels.Add(lbl);
                }
            }
            else
            {
                ////create layers
                float h_all = 0;
                for (int i = 0; i < len; i++)
                {
                    float h_i = (float)((drawingRect.Height - circleBaseHeightIn3Dmode) * Math.Abs(points[i].YValues[yIndex]) / all);
                    PointF topCenterPoint = new PointF(xCenter, drawingRect.Top + h_all);
                    h_all += h_i;
                    AccumulationChartsLayer fl = new AccumulationChartsLayer(i, topCenterPoint, h_i, series3D, offset3DRatio, ChartFunnelMode.YIsHeight);
                    fl.Series = m_series;
                    fl.GapRatio = gapRatio;
                    fl.FigureBase = figureBase;
                    fl.RotationRation = rotateRatio;
                    fl.MinWidth = 0;
                    fl.DepthPosition = drawingRect.Width / 2;
                    m_layers.Add(fl);

                    ChartStyleInfo si = GetStyleAt(points[i].Index);
                    if ((si.Text.Length == 0) || !(si.DisplayText)) continue;
                    AccumulationChartsLabel lbl = new AccumulationChartsLabel(i, points[i].Point, si, fl, AccumulationChartsLabelAttachMode.Center, m_series, points[i].Index);
                    lbl.LabelPlacement = lblPlacement;
                    lbl.LabelStyle = lblStyle;
                    m_labels.Add(lbl);
                }

                func = new ChartMath.DoubleFunc(OptimizationFunc_SurfacePyramid);
            }

            double k_res = double.NaN;
            for (int i = 0; i < 4; i++)
            {
                k_res = ChartMath.SmartBisection(func, coTangens_min, coTangens_max, coTangens_max * 0.01, 20, 10);
                if (double.IsNaN(k_res))
                {
                    for (int j = 0; j < m_labels.Count; j++)
                    {
                        AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[j];
                        label.MaxTextWidth *= 0.75f;
                    }
                }
                else
                    break;
            }

            if (double.IsNaN(k_res)) 
                func(coTangens_mid);

            m_layers.Reverse();
            return;
        }

        /// <summary>
        /// Optimizations the func_ pyramid.
        /// </summary>
        /// <param name="ctg">The CTG.</param>
        /// <returns></returns>
        private double OptimizationFunc_Pyramid(double ctg)
        {
            float h_all = 0;
            for (int i = 0; i < m_layers.Count; i++)
            {
                AccumulationChartsLayer layer = (AccumulationChartsLayer)m_layers[i];
                layer.UpWidth = (float)(2 * ctg * h_all);
                h_all += layer.Height;
                layer.DownWidth = (float)(2 * ctg * h_all);
            }

            RectangleF funnelWithLabelsRect = CalcLayersAndLabelsSizeLocAndGetTheirBoundingRect();

            return (m_drawingRect.Width - funnelWithLabelsRect.Width);
        }

        /// <summary>
        /// Optimizations the func_ surface pyramid.
        /// </summary>
        /// <param name="ctg">The CTG.</param>
        /// <returns>Returns double.</returns>
        private double OptimizationFunc_SurfacePyramid(double ctg)
        {
            bool series3D = Chart.Series3D;
            float offset3DRatio = (float)Math.Sin(ChartArea.Tilt * Math.PI / 360);

            float k = 2 * (float)ctg;

            float circleBaseHeightIn3DmodeCoeff = series3D ? offset3DRatio * k : 0;

            float height = m_drawingRect.Height / (1 + circleBaseHeightIn3DmodeCoeff);

            float s = k * height * height / 2;

            double all = this.GetAllValue();

            float h_all = 0;
            float h_i = 0;
            for (int i = 0; i < m_layers.Count; i++)
            {
                float s_i = s * (float)(Math.Abs(m_points[i].Point.YValues[0]) / all);
                double d = k * k * h_all * h_all + 2 * k * s_i;
                h_i = (-k * h_all + (float)Math.Sqrt(d)) / k;

                AccumulationChartsLayer layer = (AccumulationChartsLayer)m_layers[i];

                PointF topCenterPoint = new PointF(layer.TopCenterPoint.X, m_drawingRect.Top + h_all);
                layer.TopCenterPoint = topCenterPoint;

                layer.UpWidth = (float)(2 * ctg * h_all);
                layer.Height = h_i;
                h_all += h_i;
                layer.DownWidth = (float)(2 * ctg * h_all);
            }

            RectangleF funnelWithLabelsRect = CalcLayersAndLabelsSizeLocAndGetTheirBoundingRect();

            return (m_drawingRect.Width - funnelWithLabelsRect.Width);
        }
        #endregion
    }
}