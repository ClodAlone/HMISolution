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
    /// Represents the funnel type renderer.
    /// </summary>
    internal class FunnelRenderer : ChartSeriesRenderer
    {
        #region Constants
        protected const float c_spacing = 0.03f;
        protected const float LABEL_RADIUS_OUTER_SPACE_RATIO = 0.95f;
        protected const float MAX_LABELS_INTERSECT_ITER_COUNT = 100;
        private const float c_layerRectSpacing = 0.075f;
        #endregion

        #region Members
        protected ArrayList m_layers;
        protected ArrayList m_labels;
        protected ChartStyledPoint[] m_points;
        protected RectangleF m_drawingRect;
        protected Graphics m_g;
        #endregion

        #region Properties
        /// <summary>
        /// Indicates how much space this type will use.
        /// </summary>
        /// <value></value>
        public override ChartUsedSpaceType FillSpaceType
        {
            get
            {
                return ChartUsedSpaceType.All;
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
                return 1;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FunnelRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public FunnelRenderer(ChartSeries series)
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
            this.OnRender(g, m_series.ConfigItems.FunnelItem.ShowSeriesTitle);
        }

        /// <summary>
        /// Renders chart by the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            this.OnRender(args, m_series.ConfigItems.FunnelItem.ShowSeriesTitle);
        }

        /// <summary>
        /// Render the chart type.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="showTitle">if set to <c>true</c> title is shown.</param>
        protected void OnRender(Graphics g, bool showTitle)
        {
            RectangleF bounds = this.Bounds;
            float spc = c_spacing * Math.Min(bounds.Width, bounds.Height);
            int serIndex = this.Chart.Series.IndexOf(m_series);

            if (showTitle)
            {
                SizeF titleSize = g.MeasureString(m_series.Text, this.SeriesStyle.GdipFont);
                RectangleF titleRect = LayoutHelper.AlignRectangle(bounds, titleSize, ContentAlignment.BottomCenter);

                using (SolidBrush sb = new SolidBrush(this.SeriesStyle.TextColor))
                {
                    g.DrawString(m_series.Text, this.SeriesStyle.GdipFont, sb, titleRect);
                }

                bounds.Height -= titleSize.Height;
            }

            m_g = g;
            m_drawingRect = RectangleF.Inflate(bounds, -spc, -spc);
            m_points = this.PrepearePoints().Clone() as ChartStyledPoint[];

            CreateLayersAndLabels(m_points, g, m_drawingRect);

            if (m_layers != null)
            {
                //drawing part of renderer
                foreach (AccumulationChartsLayer layer in m_layers)
                {
                    ChartStyledPoint stylePoint = m_points[layer.Index];
                    Region r = layer.Draw(g, this.GetBrush(stylePoint.Index), stylePoint.Style.GdipPen, Chart.NeedRegionUpdate);

                    if (Chart.NeedRegionUpdate && r != null)
                    {
                        Chart.ChartRegions.Add(new ChartRegion(r, serIndex, stylePoint.Index, stylePoint.ToolTip, "Accumulation Chart Region"));
                    }
                }

                foreach (AccumulationChartsLabel label in m_labels)
                {
                    label.Draw(g);
                }
            }
        }

        /// <summary>
        /// Render the chart type.
        /// </summary>
        /// <param name="args">The <see cref="ChartRenderArgs3D"/>.</param>
        /// <param name="showTitle">if set to <c>true</c> title is shown.</param>
        protected void OnRender(ChartRenderArgs3D args, bool showTitle)
        {
            RectangleF bounds = this.Bounds;
            float spc = c_spacing * Math.Min(bounds.Width, bounds.Height);

            if (showTitle)
            {
                GraphicsPath gp = new GraphicsPath();
                SizeF titleSize = args.Graph.Graphics.MeasureString(m_series.Text, this.SeriesStyle.GdipFont);
                RectangleF titleRect = LayoutHelper.AlignRectangle(bounds, titleSize, ContentAlignment.BottomCenter);
                RenderingHelper.AddTextPath(gp, args.Graph.Graphics, m_series.Text, this.SeriesStyle.GdipFont, titleRect);

                args.Graph.AddPolygon(Path3D.FromGraphicsPath(gp, -0.5f * args.Depth, new SolidBrush(this.SeriesStyle.TextColor)));
                bounds.Height -= titleSize.Height;
            }

            m_g = args.Graph.Graphics;
            m_drawingRect = RectangleF.Inflate(bounds, -spc, -spc);
            m_points = this.PrepearePoints().Clone() as ChartStyledPoint[];

            this.CreateLayersAndLabels(m_points, m_g, m_drawingRect);

            //drawing part of renderer
            foreach (AccumulationChartsLayer layer in m_layers)
            {
                ChartStyledPoint stylePoint = m_points[layer.Index];
                ChartRegionData crd = new ChartRegionData(args.SeriesIndex, stylePoint.Index,
                    stylePoint.ToolTip, "Accumulation Chart Region");

                Polygon[] polygons = layer.Draw3D(GetBrush(stylePoint.Index), stylePoint.Style.GdipPen);

                for (int ip = 0; ip < polygons.Length; ip++)
                {
                    polygons[ip].RegionData = crd;
                    args.Graph.AddPolygon(polygons[ip]);
                }
            }

            foreach (AccumulationChartsLabel label in m_labels)
            {
                args.Graph.AddPolygon(label.Draw3D());
            }
        }

        /// <summary>
        /// Creates the layers and labels.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="g">The g.</param>
        /// <param name="drawingRect">The drawing rect.</param>
        protected virtual void CreateLayersAndLabels(ChartStyledPoint[] points, Graphics g, RectangleF drawingRect)
        {
            if (points.Length == 0)
                return;

            ChartFunnelMode funnelMode = m_series.ConfigItems.FunnelItem.FunnelMode;

            if (funnelMode == ChartFunnelMode.YIsWidth)
                Array.Sort(points, new ComparerPointWithIndexByY());

            bool series3D = Chart.Series3D;
            float offset3DRatio = (float)Math.Sin(ChartArea.Tilt * Math.PI /360);
            float rotateRatio = (float)((ChartArea.Rotation * ChartMath.ToRadians - Math.PI / 4) % ChartMath.HlfPI);
            float circleBaseHeightIn3Dmode = series3D ? 0.5f * offset3DRatio * drawingRect.Width : 0;

            //these vars, should be exposed as properties
            bool labelsRight = (m_series.ConfigItems.FunnelItem.LabelPlacement != ChartAccumulationLabelPlacement.Left);
            float gapRatio = m_series.ConfigItems.FunnelItem.GapRatio;
            ChartAccumulationLabelStyle lblStyle = m_series.ConfigItems.FunnelItem.LabelStyle;
            ChartAccumulationLabelPlacement lblPlacement = m_series.ConfigItems.FunnelItem.LabelPlacement;
            ChartFigureBase figureBase = m_series.ConfigItems.FunnelItem.FigureBase;
            /////////////////////////////////////////////

            float opposedLabels = labelsRight ? 1 : -1;
            double all = GetAllValue();
            int len = points.Length;

            double maxYVal = Math.Abs(points[points.Length - 1].Point.YValues[0]);

            double k_max = (drawingRect.Width / maxYVal);
            double k_min = k_max / 5;
            float xCenter = (opposedLabels + 1) / 2 * drawingRect.Left +
                            (1 - opposedLabels) / 2 * drawingRect.Right +
                            (float)(opposedLabels * k_max * maxYVal / 2);

            m_layers = new ArrayList(len - 1);
            m_labels = new ArrayList(len);

            if (funnelMode == ChartFunnelMode.YIsWidth)
            {
                #region YIsWidth mode
                float h_i = (drawingRect.Height - circleBaseHeightIn3Dmode) / (len - 1);

                //create layers
                for (int i = 0; i < len - 1; i++)
                {
                    PointF topCenterPoint = new PointF(xCenter, drawingRect.Bottom - h_i * (i + 1));
                    AccumulationChartsLayer fl = new AccumulationChartsLayer(i, topCenterPoint, h_i, series3D, offset3DRatio, funnelMode);
                    fl.Series = m_series;
                    fl.GapRatio = gapRatio;
                    fl.FigureBase = figureBase;
                    fl.RotationRation = rotateRatio;
                    fl.DepthPosition = drawingRect.Width / 2;
                    m_layers.Add(fl);

                    ChartStyleInfo si = GetStyleAt(points[i].Index);
                    if ((si.Text.Length == 0) || !(si.DisplayText)) continue;
                    AccumulationChartsLabel lbl = new AccumulationChartsLabel(i, points[i].Point, si, fl, AccumulationChartsLabelAttachMode.Bottom, m_series, points[i].Index);
                    lbl.LabelPlacement = lblPlacement;
                    lbl.LabelStyle = lblStyle;
                    m_labels.Add(lbl);
                }

                ChartStyleInfo sit = GetStyleAt(points[len - 1].Index);
                ((AccumulationChartsLayer)m_layers[len - 2]).TopLevel = true;
                AccumulationChartsLabel lblt = new AccumulationChartsLabel(len - 1, points[len - 1].Point, sit, (AccumulationChartsLayer)m_layers[len - 2], AccumulationChartsLabelAttachMode.Top, m_series, points[len - 1].Index);
                lblt.LabelPlacement = lblPlacement;
                lblt.LabelStyle = lblStyle;
                m_labels.Add(lblt);

                double k_res = double.NaN;
                for (int i = 0; i < 4; i++)
                {
                    k_res = ChartMath.SmartBisection(new ChartMath.DoubleFunc(OptimizationFunc_YIsWidth), k_min, k_max, k_max * 0.01, 20, 10);
                    if (double.IsNaN(k_res))
                    {
                        for (int j = 0; j < m_labels.Count; j++)
                        {
                            AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[j];
                            label.MaxTextWidth *= 0.75f;
                        }
                    }
                    else break;
                }

                if (double.IsNaN(k_res)) OptimizationFunc_YIsWidth(k_min);
                #endregion
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            double coTangens_max = drawingRect.Width / ((drawingRect.Height - circleBaseHeightIn3Dmode));
            double coTangens_min = coTangens_max / 12;
            xCenter = (opposedLabels + 1) / 2 * drawingRect.Left +
                      (1 - opposedLabels) / 2 * drawingRect.Right +
                      (float)(opposedLabels * coTangens_max * (drawingRect.Height - circleBaseHeightIn3Dmode));

            if (funnelMode == ChartFunnelMode.YIsHeight)
            {
                #region YIsHeight
                //create layers
                float h_all = 0;
                for (int i = 0; i < len; i++)
                {
                    float h_i = Math.Abs((float)((drawingRect.Height - circleBaseHeightIn3Dmode) * points[i].Point.YValues[0] / all));
                    h_all += h_i;

                    PointF topCenterPoint = new PointF(xCenter, drawingRect.Bottom - h_all);
                    AccumulationChartsLayer fl = new AccumulationChartsLayer(i, topCenterPoint, h_i, series3D, offset3DRatio, funnelMode);
                    fl.Series = m_series;
                    fl.FigureBase = figureBase;
                    fl.GapRatio = gapRatio;
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

                ((AccumulationChartsLayer)m_layers[len - 1]).TopLevel = true;

                double k_res = double.NaN;
                for (int i = 0; i < 4; i++)
                {
                    k_res = ChartMath.SmartBisection(new ChartMath.DoubleFunc(OptimizationFunc_YIsHeight), coTangens_min, coTangens_max, coTangens_max * 0.01, 20, 10);
                    if (double.IsNaN(k_res))
                    {
                        for (int j = 0; j < m_labels.Count; j++)
                        {
                            AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[j];
                            label.MaxTextWidth *= 0.75f;
                        }
                    }
                    else break;
                }
                if (double.IsNaN(k_res)) OptimizationFunc_YIsHeight(coTangens_min);
                #endregion
            }
        }

        /// <summary>
        /// Optimizations the width of the func_ Y is.
        /// </summary>
        /// <param name="k">The k.</param>
        /// <returns></returns>
        private double OptimizationFunc_YIsWidth(double k)
        {
            for (int i = 0; i < m_layers.Count; i++)
            {
                AccumulationChartsLayer layer = (AccumulationChartsLayer)m_layers[i];
                int topPointIndex = layer.Index + 1;
                layer.UpWidth = Math.Abs((float)(k * m_points[topPointIndex].Point.YValues[0]));
                int downPointIndex = layer.Index;
                layer.DownWidth = Math.Abs((float)(k * m_points[downPointIndex].Point.YValues[0]));
            }

            RectangleF funnelWithLabelsRect = CalcLayersAndLabelsSizeLocAndGetTheirBoundingRect();

            return (m_drawingRect.Width - funnelWithLabelsRect.Width);
        }

        /// <summary>
        /// Optimizations the height of the func_ Y is.
        /// </summary>
        /// <param name="ctg">The CTG.</param>
        /// <returns></returns>
        private double OptimizationFunc_YIsHeight(double ctg)
        {
            float h_all = 0;
            for (int i = 0; i < m_layers.Count; i++)
            {
                AccumulationChartsLayer layer = (AccumulationChartsLayer)m_layers[i];
                layer.DownWidth = (float)(2 * ctg * h_all);
                h_all += layer.Height;
                layer.UpWidth = (float)(2 * ctg * h_all);
            }

            RectangleF funnelWithLabelsRect = CalcLayersAndLabelsSizeLocAndGetTheirBoundingRect();

            return (m_drawingRect.Width - funnelWithLabelsRect.Width);
        }

        /// <summary>
        /// Calculates the layers and labels size loc and get their bounding rect.
        /// </summary>
        /// <returns></returns>
        protected RectangleF CalcLayersAndLabelsSizeLocAndGetTheirBoundingRect()
        {
            RectangleF funnelRect = GetLayersRect(m_layers);

            for (int i = 0; i < m_labels.Count; i++)
            {
                AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[i];
                label.CalcSize(m_g);
                label.CalcLocation(funnelRect);
            }

            FightWithLabelsIntersection();
            FightWithLabelsAndConnectionLinesIntersection();

            RectangleF labelsRect = GetLabelsRect(m_labels);
            RectangleF funnelWithLabelsRect = funnelRect;
            if (!labelsRect.IsEmpty)
                funnelWithLabelsRect = RectangleF.Union(funnelRect, labelsRect);

            #region offsetting
            PointF layersOffset = new PointF(m_drawingRect.Left - funnelWithLabelsRect.Left, m_drawingRect.Top + m_drawingRect.Height / 2 - funnelRect.Top - funnelRect.Height / 2);
            for (int i = 0; i < m_layers.Count; i++)
            {
                AccumulationChartsLayer layer = (AccumulationChartsLayer)m_layers[i];
                layer.TopCenterPoint = new PointF(layer.TopCenterPoint.X + layersOffset.X, layer.TopCenterPoint.Y + layersOffset.Y);
            }

            RectangleF labelsWithDrawingRect = RectangleF.Union(labelsRect, m_drawingRect);
            for (int i = 0; i < m_labels.Count; i++)
            {
                AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[i];

                PointF labelsOffset = new PointF(m_drawingRect.Left - funnelWithLabelsRect.Left,
                  label.AllowYOffset ? (m_drawingRect.Top + m_drawingRect.Height / 2 - labelsWithDrawingRect.Top - labelsWithDrawingRect.Height / 2) : 0);

                label.Rectangle = new RectangleF(label.Rectangle.X + labelsOffset.X, label.Rectangle.Y + labelsOffset.Y,
                                                 label.Rectangle.Width, label.Rectangle.Height);
            }
            #endregion

            for (int i = 0; i < m_labels.Count; i++)
            {
                AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[i];
                label.CalcLocation(funnelRect);
            }
            FightWithLabelsIntersection();
            FightWithLabelsAndConnectionLinesIntersection();

            labelsRect = GetLabelsRect(m_labels);
            funnelWithLabelsRect = GetLayersRect(m_layers);
            if (!labelsRect.IsEmpty)
                funnelWithLabelsRect = RectangleF.Union(funnelRect, labelsRect);

            return funnelWithLabelsRect;
        }

        /// <summary>
        /// Fights the with labels intersection.
        /// </summary>
        private void FightWithLabelsIntersection()
        {
            int intersection_count = int.MaxValue;
            for (int iter = 0; (iter < MAX_LABELS_INTERSECT_ITER_COUNT) && (intersection_count >= 0); iter++)
            {
                intersection_count = 0;
                for (int i = 0; i < m_labels.Count; i++)
                {
                    #region getting neighbour labels
                    AccumulationChartsLabel label2 = (AccumulationChartsLabel)m_labels[i];
                    RectangleF r2 = label2.Rectangle;

                    RectangleF r1 = RectangleF.Empty;
                    AccumulationChartsLabel label1 = null;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[j];
                        if (AreRectanglesStacked(r2, label.Rectangle))
                        {
                            label1 = label;
                            break;
                        }
                    }
                    if (label1 != null)
                    {
                        r1 = label1.Rectangle;
                    }

                    RectangleF r3 = RectangleF.Empty;
                    AccumulationChartsLabel label3 = null;
                    for (int j = i + 1; j < m_labels.Count; j++)
                    {
                        AccumulationChartsLabel label = (AccumulationChartsLabel)m_labels[j];
                        if (AreRectanglesStacked(r2, label.Rectangle))
                        {
                            label3 = label;
                            break;
                        }
                    }
                    if (label3 != null)
                    {
                        r3 = label3.Rectangle;
                    }

                    #endregion

                    if (r2.IntersectsWith(r1) || r2.IntersectsWith(r3))
                    {
                        intersection_count++;
                        label2.TryToAvoidRectangleIntersection(r1, r3);
                    }
                }
            }
        }

        /// <summary>
        /// Fights the with labels and connection lines intersection.
        /// </summary>
        private void FightWithLabelsAndConnectionLinesIntersection()
        {
            int intersection_count = int.MaxValue;
            for (int iter = 0; (iter < MAX_LABELS_INTERSECT_ITER_COUNT) && (intersection_count >= 0); iter++)
            {
                intersection_count = 0;
                for (int i = 0; i < m_labels.Count; i++)
                {
                    AccumulationChartsLabel label2 = (AccumulationChartsLabel)m_labels[i];
                    RectangleF r2 = label2.Rectangle;

                    AccumulationChartsLabel label1 = null;
                    if (i - 1 >= 0) label1 = (AccumulationChartsLabel)m_labels[i - 1];
                    PointF l1_p1 = PointF.Empty, l1_p2 = PointF.Empty;
                    if (label1 != null)
                        label1.GetConnectioLinePoints(out l1_p1, out l1_p2);

                    AccumulationChartsLabel label3 = null;
                    if (i + 1 < m_labels.Count) label3 = (AccumulationChartsLabel)m_labels[i + 1];
                    PointF l3_p1 = PointF.Empty, l3_p2 = PointF.Empty;
                    if (label3 != null)
                        label3.GetConnectioLinePoints(out l3_p1, out l3_p2);

                    if (ChartMath.RectanlgeIntersectsWithLine(r2, l1_p1, l1_p2))
                    {
                        intersection_count++;
                        label2.TryToAvoidLineIntersection(l1_p1, l1_p2);
                    }
                    if (ChartMath.RectanlgeIntersectsWithLine(r2, l3_p1, l3_p2))
                    {
                        intersection_count++;
                        label2.TryToAvoidLineIntersection(l3_p1, l3_p2);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the labels rect.
        /// </summary>
        /// <param name="labels">The labels.</param>
        /// <returns></returns>
        private RectangleF GetLabelsRect(ArrayList labels)
        {
            RectangleF labelsRect = RectangleF.Empty;

            foreach (AccumulationChartsLabel label in labels)
            {
                if (labelsRect.IsEmpty)
                {
                    labelsRect = label.Rectangle;
                }
                else
                {
                    labelsRect = RectangleF.Union(labelsRect, label.Rectangle);
                }
            }

            return labelsRect;
        }

        /// <summary>
        /// Gets the layers rect.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <returns></returns>
        public RectangleF GetLayersRect(ArrayList layers)
        {
            RectangleF funnelRect = RectangleF.Empty;

            foreach (AccumulationChartsLayer layer in layers)
            {
                if (funnelRect.IsEmpty)
                {
                    funnelRect = layer.GetFullDrawingRect();
                }
                else
                {
                    funnelRect = RectangleF.Union(funnelRect, layer.GetFullDrawingRect());
                }
            }

            return RectangleF.Inflate(funnelRect, c_layerRectSpacing * funnelRect.Width, 0);
        }

        /// <summary>
        /// Computes the size of necessary rectangle for the rendering.
        /// </summary>
        /// <param name="g"></param>
        /// <returns>
        /// 	<see cref="SizeF"/> of minimal rectangle.
        /// </returns>
        public override SizeF GetMinSize(Graphics g)
        {
            return SizeF.Empty;
        }

        /// <summary>
        /// Gets all value.
        /// </summary>
        /// <returns></returns>
        protected double GetAllValue()
        {
            double res = 0;
            int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];

            for (int i = 0; i < m_series.Points.Count; i++)
            {
                ChartPoint pt = m_series.Points[i];

                if (pt.IsEmpty)
                {
                    continue;
                }

                res += Math.Abs(pt.YValues[yIndex]);
            }

            return res;
        }

        /// <summary>
        /// Calculate value indicates that rectangles are stacked.
        /// </summary>
        /// <param name="r1">The  first rectangle to check.</param>
        /// <param name="r2">The second rectangle to check.</param>
        /// <returns>True if given rectangles are stacked, otherwise false.</returns>
        private static bool AreRectanglesStacked(RectangleF r1, RectangleF r2)
        {
            if ((r1.Left <= r2.Left) && (r1.Right > r2.Left)) return true;
            if ((r2.Left <= r1.Left) && (r2.Right > r1.Left)) return true;

            return false;
        }

        /// <summary>
        /// Gets the total depth.
        /// </summary>
        /// <returns></returns>
        internal override float GetTotalDepth()
        {
            return this.Bounds.Width;
        }
        #endregion
    }

    #region Helper classes
    /// <summary>
    /// The AccumulationChartsLayer class.
    /// </summary>
    internal class AccumulationChartsLayer
    {
        #region Constants
        private const int POLYGON_SECTORS = 18;
        private const double D_TO_R = Math.PI / 180d;
        private const double R_TO_D = 180d / Math.PI;
        private const double PI_2 = Math.PI / 2;
        #endregion

        #region Members
        private float m_upWidth;
        private float m_downWidth;
        private float m_height;
        private float m_gapRatio = 0;
        private float m_minWidth = 40;
        private int m_index;
        private PointF m_topCenter;
        private bool m_series3D = false;
        private float m_offset3DRation = 0.1f;
        private float m_rotationRation = (float)Math.PI / 3;
        private bool m_topLevel = false;
        private ChartFunnelMode m_funnelMode = ChartFunnelMode.YIsHeight;
        private ChartFigureBase m_figureBase = ChartFigureBase.Square;
        private ChartSeries m_series;
        private static double s_phongAngle = 0;
        private static ColorBlend s_colorBlend;
        private float m_depth;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets or sets up width.
        /// </summary>
        /// <value>Up width.</value>
        public float UpWidth
        {
            get
            {
                return m_upWidth;
            }
            set
            {
                m_upWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets down width.
        /// </summary>
        /// <value>Down width.</value>
        public float DownWidth
        {
            get
            {
                return m_downWidth;
            }
            set
            {
                m_downWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        /// <summary>
        /// Gets or sets the gap ratio.
        /// </summary>
        /// <value>The gap ratio.</value>
        public float GapRatio
        {
            get
            {
                return m_gapRatio;
            }
            set
            {
                m_gapRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the min.
        /// </summary>
        /// <value>The width of the min.</value>
        public float MinWidth
        {
            get
            {
                return m_minWidth;
            }
            set
            {
                m_minWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the top center point.
        /// </summary>
        /// <value>The top center point.</value>
        public PointF TopCenterPoint
        {
            get
            {
                return m_topCenter;
            }
            set
            {
                m_topCenter = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [series3 D].
        /// </summary>
        /// <value><c>true</c> if [series3 D]; otherwise, <c>false</c>.</value>
        public bool Series3D
        {
            get
            {
                return m_series3D;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [top level].
        /// </summary>
        /// <value><c>true</c> if [top level]; otherwise, <c>false</c>.</value>
        public bool TopLevel
        {
            get
            {
                return m_topLevel;
            }
            set
            {
                m_topLevel = value;
            }
        }

        /// <summary>
        /// Gets the funnel mode.
        /// </summary>
        /// <value>The funnel mode.</value>
        /// <returns></returns>
        public ChartFunnelMode FunnelMode
        {
            get
            {
                return m_funnelMode;
            }
        }

        /// <summary>
        /// Gets or sets the figure base.
        /// </summary>
        /// <value>The figure base.</value>
        public ChartFigureBase FigureBase
        {
            get { return m_figureBase; }
            set { m_figureBase = value; }
        }

        /// <summary>
        /// Gets or sets the rotation ration.
        /// </summary>
        /// <value>The rotation ration.</value>
        public float RotationRation
        {
            get { return m_rotationRation; }
            set
            {
                m_rotationRation = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset3 D ration.
        /// </summary>
        /// <value>The offset3 D ration.</value>
        public float Offset3DRation
        {
            get { return m_offset3DRation; }
            set { m_offset3DRation = value; }
        }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>The series.</value>
        public ChartSeries Series
        {
            get { return m_series; }

            set { m_series = value; }
        }

        /// <summary>
        /// Gets or sets the depth position.
        /// </summary>
        /// <value>The depth position.</value>
        public float DepthPosition
        {
            get { return m_depth; }

            set { m_depth = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the height of the gap ratio.
        /// </summary>
        /// <returns></returns>
        public float GetGapRatioHeight()
        {
            return (1 - 2 * GapRatio) * Height;
        }

        /// <summary>
        /// Gets the angle tangent.
        /// </summary>
        /// <returns></returns>
        public double GetAngleTangent()
        {
            double drawingHeight = GetGapRatioHeight();
            return (UpWidth - DownWidth) / (2 * drawingHeight);
        }

        /// <summary>
        /// Gets the min drawing rect.
        /// </summary>
        /// <returns></returns>
        public RectangleF GetMinDrawingRect()
        {
            return new RectangleF(TopCenterPoint.X - MinWidth / 2, TopCenterPoint.Y + GapRatio * Height, MinWidth, GetGapRatioHeight());
        }

        /// <summary>
        /// Gets the outer drawing rect.
        /// </summary>
        /// <returns></returns>
        public RectangleF GetOuterDrawingRect()
        {
            float minW = Math.Max(UpWidth, DownWidth);
            return new RectangleF(TopCenterPoint.X - minW / 2, TopCenterPoint.Y + GapRatio * Height, minW, GetGapRatioHeight());
        }

        /// <summary>
        /// Gets the inner drawing rect.
        /// </summary>
        /// <returns></returns>
        public RectangleF GetInnerDrawingRect()
        {
            float minW = Math.Min(UpWidth, DownWidth);
            return new RectangleF(TopCenterPoint.X - minW / 2, TopCenterPoint.Y + GapRatio * Height, minW, GetGapRatioHeight());
        }

        /// <summary>
        /// Gets down drawing rect.
        /// </summary>
        /// <returns></returns>
        public RectangleF GetDownDrawingRect()
        {
            float w = Math.Max(MinWidth, DownWidth);
            return new RectangleF(TopCenterPoint.X - w / 2, TopCenterPoint.Y + GapRatio * Height, w, GetGapRatioHeight());
        }

        /// <summary>
        /// Gets up drawing rect.
        /// </summary>
        /// <returns></returns>
        public RectangleF GetUpDrawingRect()
        {
            float w = Math.Max(MinWidth, UpWidth);
            return new RectangleF(TopCenterPoint.X - w / 2, TopCenterPoint.Y + GapRatio * Height, w, GetGapRatioHeight());
        }

        /// <summary>
        /// Gets the full drawing rect.
        /// </summary>
        /// <returns></returns>
        public RectangleF GetFullDrawingRect()
        {
            RectangleF r = GetOuterDrawingRect();

            if (m_series3D)
            {
                RectangleF mr = GetMinDrawingRect();

                PointF mrtp = new PointF(mr.Right, mr.Top);
                PointF mrbp = new PointF(mr.Right, mr.Bottom);
                PointF mltp = new PointF(mr.Left, mr.Top);
                PointF mlbp = new PointF(mr.Left, mr.Bottom);

                PointF p1 = new PointF(TopCenterPoint.X - UpWidth / 2, mr.Top);
                PointF p2 = new PointF(TopCenterPoint.X + UpWidth / 2, mr.Top);
                PointF p3 = new PointF(TopCenterPoint.X + DownWidth / 2, mr.Bottom);
                PointF p4 = new PointF(TopCenterPoint.X - DownWidth / 2, mr.Bottom);

                PointF intersection1 = ChartMath.LineSegmentIntersectionPoint(p2, p3, mrtp, mrbp);
                PointF intersection2 = ChartMath.LineSegmentIntersectionPoint(p4, p1, mltp, mlbp);

                if (!intersection1.IsEmpty || (p3.X < mrbp.X))
                    p3 = mrbp;

                if (!intersection2.IsEmpty || (p4.X > mlbp.X))
                    p4 = mlbp;

                float up3Dheight = m_offset3DRation * (p2.X - p1.X);
                float down3Dheight = m_offset3DRation * (p3.X - p4.X);

                float h = GetGapRatioHeight() + (up3Dheight + down3Dheight) / 2;

                r = new RectangleF(r.X, r.Y - up3Dheight / 2, r.Width, h);
            }

            return r;
        }

        /// <summary>
        /// Determines whether this instance is widding.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is widding; otherwise, <c>false</c>.
        /// </returns>
        private bool IsWidding()
        {
            return (m_minWidth < Math.Max(m_upWidth, m_downWidth)) && (m_minWidth > Math.Min(m_upWidth, m_downWidth));
        }

        /// <summary>
        /// Needs the top side.
        /// </summary>
        /// <returns></returns>
        private bool NeedTopSide()
        {
            return (m_series3D) && (m_topLevel || (m_gapRatio > 0f));
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="AccumulationChartsLayer"/> class.
        /// </summary>
        static AccumulationChartsLayer()
        {
            Color[] colors;
            float[] positions;
            ColorBlend colorBlend = new ColorBlend();

            ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(0x60, Color.Black), Color.FromArgb(0x60, Color.Black),
              Color.FromArgb(100, Color.White), s_phongAngle, 30, out colors, out positions);

            colorBlend.Positions = positions;
            colorBlend.Colors = colors;
            s_colorBlend = colorBlend;
        }

        /// <summary>
        /// Creates instance of the AccumulationChartsLayer.
        /// </summary>
        /// <param name="index">The layer index.</param>
        /// <param name="topCenterPoint">The top center point.</param>
        /// <param name="height">The layer height.</param>
        /// <param name="series3D">The value indicates that is 3D series.</param>
        /// <param name="offset3DRatio">The offset ratio.</param>
        /// <param name="funnelmode">The chart funnel mode.</param>
        public AccumulationChartsLayer(int index, PointF topCenterPoint, float height, bool series3D, float offset3DRatio, ChartFunnelMode funnelmode)
            : this(index, topCenterPoint, 0, 0, height, 0.0f, series3D, offset3DRatio, funnelmode)
        {
        }

        /// <summary>
        /// Creates instance of the AccumulationChartsLayer.
        /// </summary>
        /// <param name="index">The layer index.</param>
        /// <param name="topCenterPoint">The top center point.</param>
        /// <param name="upWidth">The upper width.</param>
        /// <param name="downWidth">The down width.</param>
        /// <param name="height">The layer height.</param>
        /// <param name="gapRatio">The gap ratio.</param>
        /// <param name="series3D">The value indicates that is 3D series.</param>
        /// <param name="offset3DRatio">The offset ratio.</param>
        /// <param name="funnelmode">The chart funnel mode.</param>
        public AccumulationChartsLayer(int index, PointF topCenterPoint, float upWidth, float downWidth
      , float height, float gapRatio, bool series3D, float offset3DRatio, ChartFunnelMode funnelmode)
        {
            m_topCenter = topCenterPoint;
            m_index = index;
            m_downWidth = downWidth;
            m_upWidth = upWidth;
            m_height = height;
            m_gapRatio = gapRatio;
            m_series3D = series3D;
            m_offset3DRation = offset3DRatio;
            m_funnelMode = funnelmode;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draw funnel series.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to renderer series.</param>
        /// <param name="brushInfo">The <see cref="Syncfusion.Drawing.BrushInfo"/> to fill series.</param>
        /// <param name="pen">The <see cref="System.Drawing.Pen"/> to render series border.</param>
        /// <param name="outputRegion">Calculated output <see cref="System.Drawing.Region"/>.</param>
        public Region Draw(Graphics g, BrushInfo brushInfo, Pen pen, bool outputRegion)
        {
            bool isWidding = IsWidding();
            bool needTopSide = m_series3D && (TopLevel || GapRatio > 0);

            float upXr = Math.Max(m_minWidth, m_upWidth) / 2;
            float downXr = Math.Max(m_minWidth, m_downWidth) / 2;
            float wdXr = m_minWidth / 2;

            float ctrX = TopCenterPoint.X;
            float upY = TopCenterPoint.Y + GapRatio * Height;
            float downY = upY + GetGapRatioHeight();
            float l_height = GetGapRatioHeight();
            float wdY = upY + l_height * (m_upWidth - m_minWidth) / (m_upWidth - m_downWidth);

            float up3Dr = m_offset3DRation * upXr;
            float down3Dr = m_offset3DRation * downXr;
            float wd3Dr = m_offset3DRation * wdXr;

            GraphicsPath gp = new GraphicsPath();
            GraphicsPath upGp = new GraphicsPath();
            GraphicsPath rightGp = new GraphicsPath();

            RectangleF upRect = new RectangleF(ctrX - upXr, upY - up3Dr, 2 * upXr, 2 * up3Dr);
            RectangleF downRect = new RectangleF(ctrX - downXr, downY - down3Dr, 2 * downXr, 2 * down3Dr);

            if (m_figureBase == ChartFigureBase.Circle)
            {
                #region Render Circular Layer
                if ((m_series3D) && (upXr > 0) && (up3Dr > 0))
                {
                    gp.AddArc(upRect, 0, 180);

                    if (needTopSide)
                    {
                        upGp.AddEllipse(upRect);
                    }
                }
                else
                {
                    gp.AddLine(ctrX + upXr, upY, ctrX - upXr, upY);
                }

                if (isWidding)
                {
                    gp.AddLine(ctrX - upXr, upY, ctrX - wdXr, wdY);
                }

                if ((m_series3D) && (downXr > 0) && (down3Dr > 0))
                {
                    gp.AddArc(downRect, 180, -180);
                }
                else
                {
                    gp.AddLine(ctrX - downXr, downY, ctrX + downXr, downY);
                }

                if (isWidding)
                {
                    gp.AddLine(ctrX + downXr, downY, ctrX + wdXr, wdY);
                }

                gp.CloseFigure();
                #endregion
            }
            else
            {
                #region Render Square Layer
                if (m_series3D)
                {
                    float sin = (float)Math.Sin(m_rotationRation);
                    float cos = (float)Math.Cos(m_rotationRation);
                    float cosAbs = Math.Abs(cos);
                    sin = sin / cosAbs;
                    cos = cos / cosAbs;

                    //PointF urPt = new PointF(ctrX + cos * upXr, upY - sin * up3Dr);
                    PointF urPt = new PointF(ctrX + cos * upXr, upY);
                    PointF ubPt = new PointF(ctrX + sin * upXr, upY + cos * up3Dr);
                    PointF ulPt = new PointF(ctrX - cos * upXr, upY + sin * up3Dr);
                    PointF utPt = new PointF(ctrX - sin * upXr, upY - cos * up3Dr);

                    //PointF drPt = new PointF(ctrX + cos * downXr, downY - sin * down3Dr);
                    PointF drPt = new PointF(ctrX + cos * downXr, downY);
                    PointF dbPt = new PointF(ctrX + sin * downXr, downY + cos * down3Dr);
                    PointF dlPt = new PointF(ctrX - cos * downXr, downY + sin * down3Dr);

                    if (needTopSide)
                    {
                        upGp.AddPolygon(new PointF[] { urPt, ubPt, ulPt, utPt });
                    }

                    if (isWidding)
                    {
                        PointF wrPt = new PointF(ctrX + cos * wdXr, wdY - sin * wd3Dr);
                        PointF wbPt = new PointF(ctrX + sin * wdXr, wdY + cos * wd3Dr);
                        PointF wlPt = new PointF(ctrX - cos * wdXr, wdY + sin * wd3Dr);

                        gp.AddPolygon(new PointF[] { ulPt, ubPt, wbPt, dbPt, dlPt, wlPt });
                        gp.AddPolygon(new PointF[] { urPt, ubPt, wbPt, dbPt, drPt, wrPt });
                        rightGp.AddPolygon(new PointF[] { urPt, ubPt, wbPt, dbPt, drPt, wrPt });
                    }
                    else
                    {
                        gp.AddPolygon(new PointF[] { ulPt, ubPt, dbPt, dlPt });
                        gp.AddPolygon(new PointF[] { urPt, ubPt, dbPt, drPt });
                        rightGp.AddPolygon(new PointF[] { urPt, ubPt, dbPt, drPt });
                    }
                }
                else
                {
                    if (isWidding)
                    {
                        gp.AddPolygon(new PointF[]{ new PointF( ctrX - upXr, upY ), new PointF( ctrX + upXr, upY ),
              new PointF( ctrX + wdXr, wdY ), new PointF( ctrX + downXr, downY ), 
              new PointF( ctrX - downXr, downY ), new PointF( ctrX - wdXr, wdY )});
                    }
                    else
                    {
                        gp.AddPolygon(new PointF[]{ new PointF( ctrX - upXr, upY ), new PointF( ctrX + upXr, upY ),
              new PointF( ctrX + downXr, downY ), new PointF( ctrX - downXr, downY )});
                    }
                }
                #endregion
            }

            BrushPaint.FillPath(g, gp, brushInfo);

            if (m_figureBase == ChartFigureBase.Circle)
            {
                if (m_series.ChartModel.ColorModel.AllowGradient)
                {
                    RectangleF rect = gp.GetBounds();

                    if (rect.Height > 0 && rect.Width > 0)
                    {
                        using (LinearGradientBrush pgb = new LinearGradientBrush(gp.GetBounds(), Color.Black, Color.White, LinearGradientMode.Horizontal))
                        {
                            pgb.InterpolationColors = s_colorBlend;
                            g.FillPath(pgb, gp);
                        }
                    }
                }
            }
            else if (m_series3D)
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(0xA0, Color.Black)))
                {
                    g.FillPath(sb, rightGp);
                }
            }

            g.DrawPath(pen, gp);

            if (needTopSide)
            {
                BrushPaint.FillPath(g, upGp, brushInfo);
                g.DrawPath(pen, upGp);
            }

            return outputRegion ? new Region(gp) : null;
        }

        /// <summary>
        /// Draw 3D.
        /// </summary>
        /// <param name="brushInfo">The brush info.</param>
        /// <param name="pen">The pen.</param>
        /// <returns></returns>
        public Polygon[] Draw3D(BrushInfo brushInfo, Pen pen)
        {
            ArrayList result = new ArrayList();

            int sectorsCount = m_figureBase == ChartFigureBase.Circle ? POLYGON_SECTORS : 4;
            double piCoef = 2 * Math.PI / sectorsCount;

            bool isWinding = IsWidding();

            float minRadius = m_minWidth / 2;
            float upRadius = Math.Max(m_upWidth, m_minWidth) / 2;
            float downRadius = Math.Max(m_downWidth, m_minWidth) / 2;

            Vector3D vTopCenter = new Vector3D(m_topCenter.X, m_topCenter.Y + GapRatio * Height, m_depth);
            Vector3D vBottomCenter = new Vector3D(vTopCenter.X, vTopCenter.Y + GetGapRatioHeight(), vTopCenter.Z);
            Vector3D vWiddingCenter = Vector3D.Empty;

            if (isWinding)
            {
                vWiddingCenter = new Vector3D(vTopCenter.X, vTopCenter.Y + m_minWidth * Height / (m_upWidth - m_downWidth), vTopCenter.Z);
            }

            Vector3D[] topVectors = new Vector3D[sectorsCount];
            Vector3D[] bottomVectors = new Vector3D[sectorsCount];
            Polygon[] sides = new Polygon[sectorsCount];
            Polygon[] windingSides = isWinding ? new Polygon[sectorsCount] : null;

            for (int i = 0; i < sectorsCount; i++)
            {
                double sin1 = Math.Sin(i * piCoef);
                double cos1 = Math.Cos(i * piCoef);

                double sin2 = Math.Sin((i + 1) * piCoef);
                double cos2 = Math.Cos((i + 1) * piCoef);

                Vector3D v1 = vTopCenter + new Vector3D(cos1 * upRadius, 0, sin1 * upRadius);
                Vector3D v2 = vBottomCenter + new Vector3D(cos1 * downRadius, 0, sin1 * downRadius);
                Vector3D v3 = vBottomCenter + new Vector3D(cos2 * downRadius, 0, sin2 * downRadius);
                Vector3D v4 = vTopCenter + new Vector3D(cos2 * upRadius, 0, sin2 * upRadius);

                if (isWinding)
                {
                    Vector3D vw1 = vWiddingCenter + new Vector3D(cos1 * minRadius, 0, sin1 * minRadius);
                    Vector3D vw2 = vWiddingCenter + new Vector3D(cos2 * minRadius, 0, sin2 * minRadius);

                    windingSides[i] = new Polygon(new Vector3D[] { v1, vw1, vw2, v4 }, brushInfo, null);
                    sides[i] = new Polygon(new Vector3D[] { vw1, v2, v3, vw2 }, brushInfo, null);
                }
                else
                {
                    sides[i] = new Polygon(new Vector3D[] { v1, v2, v3, v4 }, brushInfo, null);
                }

                topVectors[i] = v1;
                bottomVectors[i] = v2;
            }

            result.Add(new Polygon(topVectors, brushInfo, pen));
            result.Add(new Polygon(bottomVectors, brushInfo, pen));

            if (isWinding)
            {
                Vector3D sv1 = vWiddingCenter + new Vector3D(minRadius, 0, minRadius);
                Vector3D sv2 = vWiddingCenter + new Vector3D(-minRadius, 0, minRadius);
                Vector3D sv3 = vWiddingCenter + new Vector3D(-minRadius, 0, -minRadius);
                Vector3D sv4 = vWiddingCenter + new Vector3D(minRadius, 0, -minRadius);

                result.Add(new Polygon(new Vector3D[] { sv1, sv2, sv3, sv4 }));
                result.AddRange(windingSides);
            }

            result.AddRange(sides);

            return (Polygon[])result.ToArray(typeof(Polygon));
        }
        #endregion
    }

    /// <summary>
    /// The AccumulationChartsLabel class.
    /// </summary>
    internal class AccumulationChartsLabel
    {
        #region Members
        protected const float MAX_TEXT_WIDTH = 300;


        private int m_index;
        private RectangleF m_rect;
        private RectangleF m_columnInRect;
        private ChartStyleInfo m_style;
        private ChartPoint m_point;
        private PointF m_connectPoint;
        private PointF m_layerConnectPoint;
        private double m_value;
        private float m_maxTextWidth = MAX_TEXT_WIDTH;
        private AccumulationChartsLayer m_layer;
        private AccumulationChartsLabelAttachMode m_attachMode;
        private ChartAccumulationLabelPlacement m_labelPlacement = ChartAccumulationLabelPlacement.Center;
        private ChartAccumulationLabelStyle m_labelStyle = ChartAccumulationLabelStyle.OutsideInColumn;
        private float m_verticalPadding = 0.0f;
        private float m_horizontalPadding = 4.0f;
        private ChartSeries m_series = null;
        private int m_labelIndex = 0;
        private ChartAccumulationLabelPlacement m_changedLabelPlacement;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets or sets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public RectangleF Rectangle
        {
            get
            {
                if (LabelStyle == ChartAccumulationLabelStyle.Disabled) return RectangleF.Empty;
                return m_rect;
            }
            set
            {
                m_rect = value;
            }
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        public ChartStyleInfo Style
        {
            get
            {
                return m_style;
            }
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>The point.</value>
        public ChartPoint Point
        {
            get
            {
                return m_point;
            }
        }

        /// <summary>
        /// Gets or sets the connect point.
        /// </summary>
        /// <value>The connetc point.</value>
        public PointF ConnetcPoint
        {
            get
            {
                return m_connectPoint;
            }
            set
            {
                m_connectPoint = value;
            }
        }

        /// <summary>
        /// Gets the not correct point.
        /// </summary>
        /// <value>The not correct point.</value>
        public PointF NotCorrectPoint
        {
            get
            {
                AccumulationChartsLabelAttachMode attachMode = AttachMode;
                m_layerConnectPoint = PointF.Empty;

                RectangleF r = RectangleF.Empty;
                // attached mode check//////////////////////////////////////////////////////////////////////////////
                if (attachMode == AccumulationChartsLabelAttachMode.Top)
                {
                    r = m_layer.GetUpDrawingRect();
                    r.Height = 0;
                }
                else if (attachMode == AccumulationChartsLabelAttachMode.Bottom)
                {
                    r = m_layer.GetDownDrawingRect();
                    r.Y = r.Y + r.Height;
                    r.Height = 0;
                }
                else if (attachMode == AccumulationChartsLabelAttachMode.Center)
                {
                    RectangleF r1 = m_layer.GetUpDrawingRect();
                    RectangleF r2 = m_layer.GetDownDrawingRect();
                    r = new RectangleF((r1.X + r2.X) / 2, (r1.Top + r2.Bottom) / 2, (r1.Width + r2.Width) / 2, 0);
                }
                RectangleF someFunnelRect = r;
                if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Left)
                {
                    m_layerConnectPoint = new PointF(someFunnelRect.Left, someFunnelRect.Top);
                }
                else if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Right)
                {
                    m_layerConnectPoint = new PointF(someFunnelRect.Right, someFunnelRect.Top);
                }

                return m_layerConnectPoint;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the max text.
        /// </summary>
        /// <value>The width of the max text.</value>
        public float MaxTextWidth
        {
            get
            {
                return m_maxTextWidth;
            }
            set
            {
                m_maxTextWidth = value;
            }
        }

        /// <summary>
        /// Gets the layer.
        /// </summary>
        /// <value>The layer.</value>
        public AccumulationChartsLayer Layer
        {
            get
            {
                return m_layer;
            }
        }

        /// <summary>
        /// Gets the attach mode.
        /// </summary>
        /// <value>The attach mode.</value>
        public AccumulationChartsLabelAttachMode AttachMode
        {
            get
            {
                return m_attachMode;
            }
        }

        /// <summary>
        /// Gets or sets the label placement.
        /// </summary>
        /// <value>The label placement.</value>
        public ChartAccumulationLabelPlacement LabelPlacement
        {
            get
            {
                return m_labelPlacement;
            }
            set
            {
                m_labelPlacement = value;
            }
        }

        /// <summary>
        /// Gets or sets the label style.
        /// </summary>
        /// <value>The label style.</value>
        public ChartAccumulationLabelStyle LabelStyle
        {
            get
            {
                return m_labelStyle;
            }
            set
            {
                m_labelStyle = value;
                CalcLocation(m_columnInRect);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [allow Y offset].
        /// </summary>
        /// <value><c>true</c> if [allow Y offset]; otherwise, <c>false</c>.</value>
        internal bool AllowYOffset
        {
            get
            {
                return ((LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn) || (LabelStyle == ChartAccumulationLabelStyle.Disabled));
            }
        }

        /// <summary>
        /// Gets or sets the vertical padding.
        /// </summary>
        /// <value>The vertical padding.</value>
        private float VerticalPadding
        {
            get
            {
                return m_verticalPadding;
            }
            set
            {
                m_verticalPadding = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal padding.
        /// </summary>
        /// <value>The horizontal padding.</value>
        private float HorizontalPadding
        {
            get
            {
                return m_horizontalPadding;
            }
            set
            {
                m_horizontalPadding = value;
            }
        }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>The series.</value>
        private ChartSeries Series
        {
            get
            {
                return m_series;
            }
            set
            {
                m_series = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the label.
        /// </summary>
        /// <value>The index of the label.</value>
        private int LabelIndex
        {
            get
            {
                return m_labelIndex;
            }
            set
            {
                m_labelIndex = value;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AccumulationChartsLabel"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="point">The point.</param>
        /// <param name="style">The style.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="attachMode">The attach mode.</param>
        public AccumulationChartsLabel(int index, ChartPoint point, ChartStyleInfo style, AccumulationChartsLayer layer, AccumulationChartsLabelAttachMode attachMode)
        {
            m_index = index;
            m_point = point;
            m_style = style;
            m_rect = RectangleF.Empty;
            m_layer = layer;
            m_attachMode = attachMode;
            m_series = null;
            m_labelIndex = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccumulationChartsLabel"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="point">The point.</param>
        /// <param name="style">The style.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="attachMode">The attach mode.</param>
        /// <param name="series">The series.</param>
        /// <param name="LblIndex">Index of the LBL.</param>
        public AccumulationChartsLabel(int index, ChartPoint point, ChartStyleInfo style, AccumulationChartsLayer layer, AccumulationChartsLabelAttachMode attachMode, ChartSeries series, int LblIndex)
        {
            m_index = index;
            m_point = point;
            m_style = style;
            m_rect = RectangleF.Empty;
            m_layer = layer;
            m_attachMode = attachMode;
            m_series = series;
            m_labelIndex = LblIndex;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Calculatess the size.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public SizeF CalcSize(Graphics g)
        {
            if (this.Series != null)
            {
                if (this.Series.Type == ChartSeriesType.Pyramid)
                {
                    if (this.Series.ConfigItems.PyramidItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                    {
                        String str = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Text;
                        Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Font;
                        SizeF sz = g.MeasureString(str, fnt, (int)MaxTextWidth + 1);
                        return m_rect.Size = new SizeF(sz.Width + 2 * HorizontalPadding, sz.Height + 2 * VerticalPadding);
                    }
                    else
                    {
                        SizeF sz = g.MeasureString(m_style.Text, m_style.GdipFont, (int)MaxTextWidth + 1);
                        return m_rect.Size = new SizeF(sz.Width + 2 * HorizontalPadding, sz.Height + 2 * VerticalPadding);
                    }

                }
                else
                {
                    if (this.Series.ConfigItems.FunnelItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                    {
                        String str = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Text;
                        Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Font;
                        SizeF sz = g.MeasureString(str, fnt, (int)MaxTextWidth + 1);
                        return m_rect.Size = new SizeF(sz.Width + 2 * HorizontalPadding, sz.Height + 2 * VerticalPadding);

                    }
                    else
                    {
                        SizeF sz = g.MeasureString(m_style.Text, m_style.GdipFont, (int)MaxTextWidth + 1);
                        return m_rect.Size = new SizeF(sz.Width + 2 * HorizontalPadding, sz.Height + 2 * VerticalPadding);
                    }
                }
            }
            else
            {
                SizeF sz = g.MeasureString(m_style.Text, m_style.GdipFont, (int)MaxTextWidth + 1);
                return m_rect.Size = new SizeF(sz.Width + 2 * HorizontalPadding, sz.Height + 2 * VerticalPadding);
            }

        }

        /// <summary>
        /// Calculates the location.
        /// </summary>
        /// <param name="columnInRect">The column in rect.</param>
        /// <returns></returns>
        public PointF CalcLocation(RectangleF columnInRect)
        {
            m_columnInRect = columnInRect;
            PointF locPoint = PointF.Empty;
            m_layerConnectPoint = PointF.Empty;
            m_connectPoint = PointF.Empty;
            AccumulationChartsLabelAttachMode attachMode = AttachMode;
            ChartAccumulationLabelStyle labelStyle = LabelStyle;
            m_changedLabelPlacement = LabelPlacement;

            if (labelStyle == ChartAccumulationLabelStyle.OutsideInColumn || labelStyle == ChartAccumulationLabelStyle.Outside)
            {
                if ((m_changedLabelPlacement != ChartAccumulationLabelPlacement.Left) && (m_changedLabelPlacement != ChartAccumulationLabelPlacement.Right))
                    m_changedLabelPlacement = ChartAccumulationLabelPlacement.Right;
            }

            if (LabelStyle != ChartAccumulationLabelStyle.Disabled)
            {
                RectangleF r = RectangleF.Empty;

                // attached mode check//////////////////////////////////////////////////////////////////////////////
                if (attachMode == AccumulationChartsLabelAttachMode.Top)
                {
                    r = Layer.GetUpDrawingRect();
                    r.Height = 0;
                    if (Layer.Series3D && labelStyle == ChartAccumulationLabelStyle.Inside)
                    {
                        r = Layer.GetFullDrawingRect();
                        r.Y = r.Y + r.Height - Layer.GetGapRatioHeight();
                        r.Height = 0;
                    }
                }
                else if (attachMode == AccumulationChartsLabelAttachMode.Bottom)
                {
                    r = m_layer.GetDownDrawingRect();
                    r.Y = r.Y + r.Height;
                    r.Height = 0;
                    if (Layer.Series3D && labelStyle == ChartAccumulationLabelStyle.Inside)
                    {
                        r = Layer.GetFullDrawingRect();
                        r.Y = r.Y + r.Height;
                        r.Height = 0;
                    }
                }
                else if (attachMode == AccumulationChartsLabelAttachMode.Center)
                {
                    RectangleF r1 = m_layer.GetUpDrawingRect();
                    RectangleF r2 = m_layer.GetDownDrawingRect();
                    r = new RectangleF((r1.X + r2.X) / 2, (r1.Top + r2.Bottom) / 2, (r1.Width + r2.Width) / 2, 0);
                    if (Layer.Series3D && labelStyle == ChartAccumulationLabelStyle.Inside)
                    {
                        RectangleF tr = Layer.GetFullDrawingRect();
                        tr.Y = tr.Y + tr.Height - Layer.GetGapRatioHeight() / 2;
                        r.Y = tr.Y;
                    }
                }

                //label style check//////////////////////////////////////////////////////////////////////////////////
                if (labelStyle == ChartAccumulationLabelStyle.Inside)
                {
                    r.X = r.X + r.Width / 2;
                    r.Y = r.Y + r.Height / 2;
                    r.Width = 0;
                    r.Height = 0;
                }
                else if (labelStyle == ChartAccumulationLabelStyle.OutsideInColumn)
                {
                    r = new RectangleF(columnInRect.X, r.Y, columnInRect.Width, r.Height);
                }


                //label placement check //////////////////////////////////////////////////////////////////////////////
                if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Center)
                {
                    locPoint = new PointF(r.X + r.Width / 2 - m_rect.Size.Width / 2,
                                          r.Y + r.Height / 2 - m_rect.Size.Height / 2);
                }
                else if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Top)
                {
                    locPoint = new PointF(r.X + r.Width / 2 - m_rect.Size.Width / 2,
                                          r.Y + r.Height / 2 - m_rect.Size.Height);
                }
                else if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Bottom)
                {
                    locPoint = new PointF(r.X + r.Width / 2 - m_rect.Size.Width / 2,
                                          r.Y + r.Height / 2);
                }
                else if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Left)
                {
                    locPoint = new PointF(r.Left - m_rect.Size.Width,
                                          r.Y + r.Height / 2 - m_rect.Size.Height / 2);
                }
                else if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Right)
                {
                    locPoint = new PointF(r.Right,
                                          r.Y + r.Height / 2 - m_rect.Size.Height / 2);
                }

                m_rect.Location = locPoint;
            }
            return locPoint;
        }

        /// <summary>
        /// Gets the connectio line points.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public void GetConnectioLinePoints(out PointF p1, out PointF p2)
        {
            p1 = PointF.Empty;
            p2 = PointF.Empty;
            if (LabelStyle != ChartAccumulationLabelStyle.Inside && LabelStyle != ChartAccumulationLabelStyle.Outside)
            {
                CalcConnectionPoint();
                p1 = m_connectPoint;
                p2 = NotCorrectPoint;
            }
        }

        /// <summary>
        /// Tries to avoid rectangle intersection.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        public void TryToAvoidRectangleIntersection(RectangleF r1, RectangleF r2)
        {
            bool cycle = true;
            while (cycle)
            {
                RectangleF r = this.Rectangle;
                if (LabelStyle == ChartAccumulationLabelStyle.OutsideInColumn)
                {
                    if ((!r1.IsEmpty && r.IntersectsWith(r1)) && (!r2.IsEmpty && r.IntersectsWith(r2)))
                    {
                        float top = float.NaN, bottom = float.NaN;
                        if (r1.Bottom > r2.Top) { top = r1.Bottom; bottom = r2.Top; }
                        else if (r2.Bottom > r1.Top) { top = r2.Bottom; bottom = r1.Top; }
                        else
                        {
                            RectangleF union = RectangleF.Union(r1, r2);
                            top = union.Top;
                            bottom = union.Bottom;
                        }

                        float center = (top + bottom) / 2;

                        PointF locPoint = new PointF(m_rect.Location.X, center - m_rect.Height / 2);

                        m_rect.Location = locPoint;
                    }
                    else
                    {
                        RectangleF rect = RectangleF.Empty;
                        if ((!r1.IsEmpty) && r.IntersectsWith(r1)) rect = r1;
                        if ((!r2.IsEmpty) && r.IntersectsWith(r2)) rect = r2;

                        if (!rect.IsEmpty)
                            if (r.Top < rect.Top)
                                m_rect.Location = new PointF(r.Left, rect.Top - r.Height);
                            else
                                m_rect.Location = new PointF(r.Left, rect.Bottom);
                    }
                }
                else if ((LabelStyle == ChartAccumulationLabelStyle.Outside))
                {
                    LabelStyle = ChartAccumulationLabelStyle.OutsideInColumn;
                    continue;
                }
                else if ((LabelStyle == ChartAccumulationLabelStyle.Inside))
                {
                    bool c1 = (!r1.IsEmpty && r.IntersectsWith(r1));
                    bool c2 = (!r2.IsEmpty && r.IntersectsWith(r2));
                    if (Layer.FunnelMode == ChartFunnelMode.YIsHeight)
                    {
                        if (c1 || c2)
                        {
                            LabelStyle = ChartAccumulationLabelStyle.OutsideInColumn;
                            continue;
                        }
                    }
                    else
                    {
                        if (c1 && c2)
                        {
                            LabelStyle = ChartAccumulationLabelStyle.OutsideInColumn;
                            continue;
                        }
                    }
                }
                cycle = false;
            }
        }

        /// <summary>
        /// Tries to avoid line intersection.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public void TryToAvoidLineIntersection(PointF p1, PointF p2)
        {
            if ((LabelStyle == ChartAccumulationLabelStyle.Outside))
            {
                LabelStyle = ChartAccumulationLabelStyle.OutsideInColumn;
            }
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="g">The g.</param>
        public void Draw(Graphics g)
        {
            if (LabelStyle == ChartAccumulationLabelStyle.Disabled) return;

            if (LabelStyle != ChartAccumulationLabelStyle.Inside && LabelStyle != ChartAccumulationLabelStyle.Outside)
            {
                CalcConnectionPoint();
                g.DrawLine(Style.GdipPen, m_connectPoint, NotCorrectPoint);
            }

            Brush brsh = new SolidBrush(Style.TextColor);
            RectangleF r = this.Rectangle;
            RectangleF textRect = new RectangleF(r.Left + HorizontalPadding, r.Top + VerticalPadding, r.Width - 2 * HorizontalPadding, r.Height - 2 * VerticalPadding);

            if (this.Series != null)
            {
                if (this.Series.ConfigItems.PyramidItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                {
                    String str = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Text;
                    Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Font;
                    g.DrawString(str, fnt, brsh, textRect);
                }
                else if (this.Series.ConfigItems.FunnelItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                {
                    String str = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Text;
                    Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Font;
                    g.DrawString(str, fnt, brsh, textRect);
                }
                else
                {
                    g.DrawString(Style.Text, Style.GdipFont, brsh, textRect);
                }
            }
            else
            {
                g.DrawString(Style.Text, Style.GdipFont, brsh, textRect);
            }
        }

        /// <summary>
        /// Draw 3D.
        /// </summary>
        /// <returns></returns>
        public Polygon Draw3D()
        {
            Path3DCollect result = null;

            if (LabelStyle != ChartAccumulationLabelStyle.Disabled)
            {
                Brush brsh = new SolidBrush(Style.TextColor);
                RectangleF r = this.Rectangle;
                RectangleF textRect = new RectangleF(r.Left + HorizontalPadding, r.Top + VerticalPadding, r.Width - 2 * HorizontalPadding, r.Height - 2 * VerticalPadding);

                GraphicsPath textGp = new GraphicsPath();

                /*textGp.AddString(Style.Text, Style.GdipFont.FontFamily, (int)Style.GdipFont.Style,
                  RenderingHelper.GetFontSizeInPixels(Style.GdipFont), textRect, StringFormat.GenericDefault);*/

                Pseudo3DText p3dText = new Pseudo3DText(Style.Text, Style.GdipFont, brsh, new Vector3D(textRect.X, textRect.Y, m_layer.DepthPosition));

                if (this.Series != null)
                {
                    if (this.Series.ConfigItems.PyramidItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                    {
                        String str = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Text;
                        Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Font;
                        p3dText = new Pseudo3DText(str, fnt, brsh, new Vector3D(textRect.X, textRect.Y, m_layer.DepthPosition));
                    }
                    else if (this.Series.ConfigItems.FunnelItem.ShowDataBindLabels && this.Series.XAxis.LabelsImpl != null)
                    {
                        String str = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Text;
                        Font fnt = m_series.XAxis.LabelsImpl.GetLabelAt(LabelIndex).Font;
                        p3dText = new Pseudo3DText(str, fnt, brsh, new Vector3D(textRect.X, textRect.Y, m_layer.DepthPosition));
                    }
                }

                result = new Path3DCollect(p3dText);

                if (LabelStyle != ChartAccumulationLabelStyle.Inside && LabelStyle != ChartAccumulationLabelStyle.Outside)
                {
                    CalcConnectionPoint();

                    Vector3D v1 = new Vector3D(m_connectPoint.X, m_connectPoint.Y, m_layer.DepthPosition);
                    Vector3D v2 = new Vector3D(NotCorrectPoint.X, NotCorrectPoint.Y, m_layer.DepthPosition);
                    Vector3D v3 = new Vector3D(NotCorrectPoint.X, NotCorrectPoint.Y, m_layer.DepthPosition);

                    result.Add(new Polygon(new Vector3D[] { v1, v2, v3 }, Style.GdipPen));
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Calculates the connection point.
        /// </summary>
        /// <returns></returns>
        private PointF CalcConnectionPoint()
        {
            RectangleF r = Rectangle;
            if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Left)
            {
                m_connectPoint = new PointF(r.Right, r.Top + r.Height / 2);
            }
            else if (m_changedLabelPlacement == ChartAccumulationLabelPlacement.Right)
            {
                m_connectPoint = new PointF(r.Left, r.Top + r.Height / 2);
            }
            return m_connectPoint;
        }
    }

    /// <summary>
    /// The AccumulationChartsLabelComparer class.
    /// </summary>
    internal class AccumulationChartsLabelComparer : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other. </exception>
        public int Compare(object x, object y)
        {
            int res = 0;
            AccumulationChartsLabel pl1 = x as AccumulationChartsLabel;
            AccumulationChartsLabel pl2 = y as AccumulationChartsLabel;

            if (pl1 != null && pl2 != null)
            {
                if (pl1.Value != pl2.Value)
                {
                    res = pl1.Value > pl2.Value ? 1 : -1;
                }
            }

            return res;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// The AccumulationChartsLabelAttachMode enumerator.
    /// </summary>
    internal enum AccumulationChartsLabelAttachMode
    {
        /// <summary>
        /// AccumulationChartsLabelAttachMode is Top.
        /// </summary>
        Top,

        /// <summary>
        /// AccumulationChartsLabelAttachMode is Center.
        /// </summary>
        Center,

        /// <summary>
        /// AccumulationChartsLabelAttachMode is Bottom.
        /// </summary>
        Bottom
    }
}
