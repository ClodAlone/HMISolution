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
using System.Windows.Forms;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// 
    /// </summary>
    internal class BubbleRenderer : ChartSeriesRenderer
    {
        #region Constants
        private const float BLINK_RANGE = 0.3f;
        private const float BLINK_MIN = 0.4f;
        private const float LIGHT_ANGLE = (float)(Math.PI / 4);
        private const float c_minimalScaleFactor = 0.1f;
        private static readonly ColorBlend c_blinkColorBlend;
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
                return "Bubble Chart Region";
            }
        }
        /// <summary>
        /// This setting allows chart types that are normally not rendered inverted to be combined with those that are
        /// normally rendered inverted. For example Bar charts are rendered inverted. The Bubble chart can be combined with
        /// Bar charts because it sets IgnoreSeriesInversion to true. When this property is set to true the renderer will ignore
        /// the inversion setting on the series being rendered.
        /// </summary>
        /// <value></value>
        protected override bool IgnoreSeriesInversion
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="BubbleRenderer"/> class.
        /// </summary>
        static BubbleRenderer()
        {
            ColorBlend cb = new ColorBlend();

            cb.Positions = new float[] { 0f, 0.7f, 1f };
            cb.Colors = new Color[] { Color.FromArgb(144, Color.Black), Color.Transparent, Color.FromArgb(128, Color.White) };

            c_blinkColorBlend = cb;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BubbleRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public BubbleRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="args">The <see cref="ChartRenderArgs2D"/>.</param>
        public override void Render(ChartRenderArgs2D args)
        {
            int serIndex = args.SeriesIndex;
            bool is3D = args.Is3D;
            bool needUpdateRegion = args.UpdateRegions;

            int yValueIndex = args.Series.PointFormats[ChartYValueUsage.YValue];
            int sizeValueIndex = args.Series.PointFormats[ChartYValueUsage.PointSizeValue];

            ChartBubbleType bubbleType = m_series.ConfigItems.BubbleItem.BubbleType;
            SizeF minSize = m_series.ConfigItems.BubbleItem.MinBounds.Size;
            SizeF maxSize = m_series.ConfigItems.BubbleItem.MaxBounds.Size;
            bool enablePhongStyle = m_series.ConfigItems.BubbleItem.EnablePhongStyle;

            IndexRange indexedRange = this.CalculateVisibleRange();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            DoubleRange bubbleYRange = this.CalculateYRange(sizeValueIndex);
            SizeF serOffset = this.GetThisOffset();

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (styledPoint.IsVisible)
                {
                    float scaleFactor = 1;
                    bool isImage = false;

                    if (styledPoint.Point.YValues.Length > sizeValueIndex)
                    {
                        scaleFactor = c_minimalScaleFactor;

                        if (bubbleYRange.Delta != 0)
                        {
                            scaleFactor += (1 - c_minimalScaleFactor) * (float)((styledPoint.Point.YValues[sizeValueIndex] - bubbleYRange.Start) / bubbleYRange.Delta);
                        }
                    }

                    PointF ptf = args.GetPoint(styledPoint.X, styledPoint.YValues[yValueIndex]);
                    SizeF sz = new SizeF(minSize.Width + scaleFactor * (maxSize.Width - minSize.Width),
                        minSize.Height + scaleFactor * (maxSize.Height - minSize.Height));

                    RectangleF rc = new RectangleF(ptf.X - sz.Width / 2, ptf.Y - sz.Height / 2, sz.Width, sz.Height);
                    GraphicsPath gp = new GraphicsPath();

                    switch (bubbleType)
                    {
                        case ChartBubbleType.Circle:
                            gp.AddEllipse(rc);
                            break;
                        case ChartBubbleType.Square:
                            gp.AddRectangle(rc);
                            break;
                        case ChartBubbleType.Image:
                            gp.AddRectangle(rc);
                            isImage = true;
                            break;
                    }

                    if (needUpdateRegion)
                    {
                        this.Chart.ChartRegions.Add(new ChartRegion(new Region(gp), serIndex,
                            styledPoint.Index, styledPoint.ToolTip, this.RegionDescription));
                    }

                    if (isImage)
                    {
                        if (styledPoint.Style.ImageIndex >= 0
                            && styledPoint.Style.ImageIndex < styledPoint.Style.Images.Count)
                        {
                            args.Graph.DrawImage(styledPoint.Style.Images[styledPoint.Style.ImageIndex],
                                rc.X, rc.Y, rc.Width, rc.Height);
                        }
                    }
                    else
                    {
                        if (!is3D && styledPoint.Style.DisplayShadow)
                        {
                            args.Graph.PushTranfsorm();
                            args.Graph.Translate(styledPoint.Style.ShadowOffset);
                            args.Graph.DrawPath(styledPoint.Style.ShadowInterior, null, gp);
                            args.Graph.PopTransform();
                        }

                        BrushInfo brInfo = this.GetBrush(styledPoint.Index);

                        if (Chart.Model.ColorModel.AllowGradient && enablePhongStyle)
                        {
                            if (bubbleType == ChartBubbleType.Circle)
                            {
                                args.Graph.DrawPath(brInfo, null, gp);

                                if (brInfo.Style == BrushStyle.Solid)
                                {
                                    using (PathGradientBrush pgb = new PathGradientBrush(gp))
                                    {
                                        float cx = is3D ? (float)(Math.Cos(LIGHT_ANGLE) * Math.Sin(ChartMath.ToRadians * ChartArea.Rotation)) : 0f;
                                        float cy = is3D ? (float)(Math.Sin(LIGHT_ANGLE) * Math.Sin(ChartMath.ToRadians * ChartArea.Tilt)) : 0f;

                                        pgb.InterpolationColors = c_blinkColorBlend;
                                        pgb.CenterPoint = new PointF(rc.X + (BLINK_MIN - BLINK_RANGE * cx) * sz.Width,
                                            rc.Y + (BLINK_MIN - BLINK_RANGE * cy) * sz.Height);

                                        args.Graph.DrawPath(pgb, styledPoint.Style.GdipPen, gp);
                                    }
                                }
                            }
                            else if (bubbleType == ChartBubbleType.Square)
                            {
                                if (brInfo.Style == BrushStyle.Solid)
                                {
                                    float[] pos;
                                    Color[] col;

                                    ChartSeriesRenderer.PhongShadingColors(brInfo.BackColor, brInfo.BackColor,
                                        Color.White, -Math.PI / 4, 20, out col, out pos);
                                    brInfo = new BrushInfo(GradientStyle.Horizontal, new BrushInfoColorArrayList(col));
                                }

                                args.Graph.DrawPath(brInfo, styledPoint.Style.GdipPen, gp);
                            }
                        }
                        else
                        {
                            if (enablePhongStyle)
                                args.Graph.DrawPath(brInfo, styledPoint.Style.GdipPen, gp);
                            else
                                args.Graph.DrawPath(styledPoint.Style.GdipPen, gp);
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
            int serIndex = this.Chart.Series.IndexOf(m_series);
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool needUpdateRegion = Chart.NeedRegionUpdate;

            ChartBubbleType bubbleType = m_series.ConfigItems.BubbleItem.BubbleType;
            SizeF minSize = m_series.ConfigItems.BubbleItem.MinBounds.Size;
            SizeF maxSize = m_series.ConfigItems.BubbleItem.MaxBounds.Size;

            IndexRange indexedRange = this.CalculateVisibleRange();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            DoubleRange bubbleYRange = this.CalculateYRange(1);
            float dpth = this.GetSeriesDepth();
            float fd = this.GetPlaceDepth() + dpth / 2;


            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];
                Polygon[] plgs = null;

                if (this.IsVisiblePoint(styledPoints[i].Point))
                {
                    float scaleFactor = 1;

                    if (styledPoint.Point.YValues.Length > 1)
                    {
                        scaleFactor = c_minimalScaleFactor;

                        if (bubbleYRange.Delta != 0)
                        {
                            scaleFactor += (1 - c_minimalScaleFactor) * (float)((styledPoint.Point.YValues[1] - bubbleYRange.Start) / bubbleYRange.Delta);
                        }
                    }

                    PointF ptf = this.GetPointFromValue(styledPoint.Point);
                    SizeF sz = new SizeF(minSize.Width + (maxSize.Width - minSize.Width) * scaleFactor,
                        minSize.Height + (maxSize.Height - minSize.Height) * scaleFactor);

                    RectangleF rc = new RectangleF(ptf.X - sz.Width / 2, ptf.Y - sz.Height / 2, sz.Width, sz.Height);
                    BrushInfo brush = this.GetBrush(styledPoint.Index);

                    if (brush.Style == BrushStyle.Solid
                        && Chart.Model.ColorModel.AllowGradient)
                    {
                        brush = new BrushInfo(GradientStyle.PathEllipse, ControlPaint.Light(brush.BackColor), brush.BackColor);
                    }

                    brush = m_series.ConfigItems.BubbleItem.EnablePhongStyle ? brush : new BrushInfo(Color.White);
                    switch (bubbleType)
                    {
                        case ChartBubbleType.Circle:
                            plgs = g.CreateEllipse(new Vector3D(rc.X, rc.Y, fd), sz, 18, styledPoint.Style.GdipPen, brush);
                            break;

                        case ChartBubbleType.Square:
                            plgs = g.CreateRectangle(new Vector3D(rc.X, rc.Y, fd), sz, styledPoint.Style.GdipPen, brush);
                            break;

                        case ChartBubbleType.Image:
                            {
                                if (styledPoint.Style.ImageIndex >= 0
                                    && styledPoint.Style.ImageIndex < styledPoint.Style.Images.Count)
                                {
                                    Image3D img3D = Image3D.FromImage(styledPoint.Style.Images[styledPoint.Style.ImageIndex], rc, fd);
                                    g.AddPolygon(img3D);
                                    plgs = new Polygon[] { img3D };
                                }
                            }
                            break;
                    }

                    if (needUpdateRegion && plgs != null)
                    {
                        ChartRegionData crd = new ChartRegionData(serIndex, styledPoint.Index,
                            styledPoint.ToolTip, this.RegionDescription);

                        foreach (Polygon p in plgs)
                        {
                            p.RegionData = crd;
                        }
                    }
                }
            }
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
            ChartBubbleConfigItem item = m_series.ConfigItems.BubbleItem;

            if (item.BubbleType == ChartBubbleType.Circle)
            {
                if (isShadow)
                {
                    using (SolidBrush br = new SolidBrush(shadowColor))
                    {
                        g.FillEllipse(br, bounds);
                    }
                }
                else
                {
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(bounds);

                    BrushPaint.FillPath(g, gp, SeriesStyle.Interior);
                    g.DrawEllipse(SeriesStyle.GdipPen, bounds);
                }
            }
            else
            {
                base.DrawIcon(g, bounds, isShadow, shadowColor);

                if ((!isShadow) && (item.BubbleType == ChartBubbleType.Image)
                    && (SeriesStyle.Images != null) && (SeriesStyle.ImageIndex > -1))
                {
                    g.DrawImage(SeriesStyle.Images[SeriesStyle.ImageIndex], bounds);
                }
            }
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Calculates the Y range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private DoubleRange CalculateYRange(int index)
        {
            DoubleRange result = DoubleRange.Empty;

            foreach (ChartPoint point in m_series.Points)
            {
                if (!point.IsEmpty && point.YValues.Length > index)
                {
                    result = DoubleRange.Union(result, point.YValues[index]);
                }
            }

            return result;
        }
        #endregion
    }
}