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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The Line chartrendering class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class LineRenderer : ChartSeriesRenderer
    {
        #region Members
        private DoubleRange m_xRange = DoubleRange.Empty;
        private DoubleRange m_yRange = DoubleRange.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Get description of regions.
        /// </summary>
        /// <value></value>
        protected override string RegionDescription
        {
            get
            {
                return "Line Chart Renderer";
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

        /// <summary>
        /// Gets a value indicating whether points should be sort.
        /// </summary>
        /// <value><c>true</c> if points should be sorted; otherwise, <c>false</c>.</value>
        protected override bool ShouldSort
        {
            get
            {
                return m_series.SortPoints;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LineRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public LineRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs2D args)
        {
            bool is3d = this.Chart.Series3D;
            bool isInvertedAxes = IsInvertedAxes;
            bool inversed = isInvertedAxes ? YAxis.Inversed : XAxis.Inversed;
            bool needRegionUpdate = this.Chart.NeedRegionUpdate;
            bool dropPoints = Chart.DropSeriesPoints;
            int yIndex = args.Series.PointFormats[ChartYValueUsage.YValue];
            ChartLineConfigItem config = args.Series.ConfigItems.LineSegment;

            ChartStyleInfo seriesStyle = this.SeriesStyle;
            SizeF seriesOffset = this.GetThisOffset();
            SizeF depthOffset = this.GetSeriesOffset();

            IList regions = this.ChartArea.ChartRegions;

            ChartStyledPoint[] styledPoints = this.StyledPoints;
            IndexRange indexedRange = this.CalculateVisibleRange();

            ChartStyledPoint first = null;
            ChartStyledPoint second = null;
            PointF firstPoint = PointF.Empty;
            PointF secondPoint = PointF.Empty;

            Pen seriesPen = new Pen(seriesStyle.GdipPen.Color, seriesStyle.GdipPen.Width);

            #region Draw lines
            int from = args.ActualXAxis.Inversed ? indexedRange.To : indexedRange.From;
            int to = args.ActualXAxis.Inversed ? indexedRange.From - 1 : indexedRange.To + 1;
            int di = args.ActualXAxis.Inversed ? -1 : 1;

            ArrayList lnpts = new ArrayList();
            for (int i = from; i != to; i += di)
            {
                second = styledPoints[i];

                if (second.IsVisible)
                {
                    secondPoint = args.GetPoint(second.X, second.YValues[yIndex]);

                    if (!((dropPoints == false && firstPoint.IsEmpty)
                    || ((Math.Abs((firstPoint.X) - secondPoint.X)) > args.Series.Resolution)
                      || (Math.Abs((secondPoint.Y) - firstPoint.Y)) > args.Series.Resolution))
                    {                   
                        continue;
                    }           

                    if (first != null)
                    {
                        DoubleRange xRange = new DoubleRange(first.X, second.X);
                        DoubleRange yRange = new DoubleRange(first.YValues[yIndex], second.YValues[yIndex]);

                        if (args.IsVisible(xRange, yRange))
                        {
                            if (args.Is3D)
                            {
                                #region Draw 3D line
                                GraphicsPath gp = new GraphicsPath();

                                gp.AddLine(firstPoint, ChartMath.AddPoint(firstPoint, depthOffset));
                                gp.AddLine(ChartMath.AddPoint(secondPoint, depthOffset), secondPoint);
                                gp.CloseFigure();
                                using (Pen pen = first.Style.GdipPen.Clone() as Pen)
                                {
                                    if (m_series.ConfigItems.LineItem.DisableLineCap)
                                    {
                                        pen.StartCap = LineCap.Flat;
                                        pen.EndCap = LineCap.Flat;
                                        pen.LineJoin = LineJoin.Bevel;
                                    }
                                    args.Graph.DrawPath(this.GetBrush(first.Index), pen, gp);
                                }
                                
                                if (args.UpdateRegions && !config.DisableLineRegion)
                                {
                                    regions.Add(new ChartRegion(new Region(gp), args.SeriesIndex, first.Index,
                                        first.ToolTip, this.RegionDescription));
                                }
                                #endregion
                            }
                            else
                            {
                                #region Draw 2D line

                                if (!this.Chart.NeedPerformance)
                                {
                                    using (Pen pen = first.Style.GdipPen.Clone() as Pen)
                                    {

                                        if (m_series.ConfigItems.LineItem.DisableLineCap)
                                        {
                                            pen.StartCap = LineCap.Flat;
                                            pen.EndCap = LineCap.Flat;
                                        }

                                        if (first.Style.DisplayShadow)
                                        {
                                            Size sdwOffset = first.Style.ShadowOffset;
                                            PointF swpt1 = ChartMath.AddPoint(firstPoint, sdwOffset);
                                            PointF swpt2 = ChartMath.AddPoint(secondPoint, sdwOffset);

                                            pen.Color = first.Style.ShadowInterior.BackColor;
                                            args.Graph.DrawLine(pen, swpt1.X, swpt1.Y, swpt2.X, swpt2.Y);
                                        }

                                        pen.Color = this.GetBrush(first.Index).BackColor;

                                        args.Graph.DrawLine(pen, firstPoint.X, firstPoint.Y, secondPoint.X, secondPoint.Y);

                                        if (args.UpdateRegions && (firstPoint != secondPoint) && !config.DisableLineRegion)
                                        {
                                            if (Math.Abs(firstPoint.X - secondPoint.X) > 1 || Math.Abs(firstPoint.Y - secondPoint.Y) > 1)
                                            {
                                                GraphicsPath rgnGp = new GraphicsPath();
                                                rgnGp.AddLine(firstPoint, secondPoint);
                                                rgnGp.Widen(pen);
                                                regions.Add(new ChartRegion(new Region(rgnGp), args.SeriesIndex, first.Index,
                                                    first.ToolTip, this.RegionDescription));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    #region Improve Performance

                                    Pen pen = seriesPen;
                                    if (first.Style.DisplayShadow)
                                    {
                                        Size sdwOffset = first.Style.ShadowOffset;
                                        PointF swpt1 = ChartMath.AddPoint(firstPoint, sdwOffset);
                                        PointF swpt2 = ChartMath.AddPoint(secondPoint, sdwOffset);

                                        pen.Color = first.Style.ShadowInterior.BackColor;
                                        args.Graph.DrawLine(pen, swpt1.X, swpt1.Y, swpt2.X, swpt2.Y);
                                    }

                                    args.Graph.DrawLine(pen, firstPoint.X, firstPoint.Y, secondPoint.X, secondPoint.Y);

                                    if (args.UpdateRegions && (firstPoint != secondPoint) && !config.DisableLineRegion)
                                    {
                                        if (Math.Abs(firstPoint.X - secondPoint.X) > 1 || Math.Abs(firstPoint.Y - secondPoint.Y) > 1)
                                        {
                                            GraphicsPath rgnGp = new GraphicsPath();
                                            rgnGp.AddLine(firstPoint, secondPoint);
                                            rgnGp.Widen(pen);
                                            regions.Add(new ChartRegion(new Region(rgnGp), args.SeriesIndex, first.Index,
                                                first.ToolTip, this.RegionDescription));
                                        }
                                    }

                                    #endregion
                                }

                                #endregion
                            }
                        }
                    }

                    #region Add point region
                    if (args.UpdateRegions && args.IsVisible(second.X, second.YValues[yIndex]))
                    {
                        Region rgn = this.GetRegionFromCircle(secondPoint, second.Style.HitTestRadius);

                        regions.Add(new ChartRegion(rgn, args.SeriesIndex, second.Index,
                            second.ToolTip, this.RegionDescription));
                    }
                    #endregion

                    firstPoint = secondPoint;
                    first = second;
                }
                else
                {
                   if (this.Chart.AllowGapForEmptyPoints)
                    first = null;
                }
            }

            #endregion
        }

        /// <summary>
        /// Renders chart by the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            bool dropPoints = Chart.DropSeriesPoints;
            bool isInvertedAxes = IsInvertedAxes;
            bool needRegionUpdate = this.Chart.NeedRegionUpdate;
            ChartLineConfigItem config = args.Series.ConfigItems.LineSegment;

            int seriesIndex = this.Chart.Series.IndexOf(m_series);
            int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];

            float fd = GetPlaceDepth();
            float dpth = GetSeriesDepth();
            float bd = fd + dpth;

            ChartStyleInfo seriesStyle = this.SeriesStyle;
            IList regions = this.ChartArea.ChartRegions;
            ChartStyledPoint[] styledPoints = PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();

            ChartErrorBarsConfigItem errorBarConfig = m_series.ConfigItems.ErrorBars;

            ChartStyledPoint first = null;
            ChartStyledPoint second = null;
            PointF firstPoint = PointF.Empty;
            PointF secondPoint = PointF.Empty;

            args.Graph.AddPolygon(CreateBoundsPolygon(fd));

            ArrayList polygonsWithTang = new ArrayList(styledPoints.Length);

            #region Draw lines
            for (int i = indexedRange.From, ci = indexedRange.To + 1; i != ci; i++)
            {
                second = styledPoints[i];

                if (!second.Point.IsEmpty)
                {
                    secondPoint = args.GetPoint(second.X, second.YValues[yIndex]);

                    if (first != null)
                    {
                        double tan = (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X);

                        Vector3D[] vts = new Vector3D[]{ 
              new Vector3D( secondPoint.X, secondPoint.Y, fd ),
              new Vector3D( firstPoint.X, firstPoint.Y, fd ),
              new Vector3D( firstPoint.X, firstPoint.Y, bd ),
              new Vector3D( secondPoint.X, secondPoint.Y, bd )};

                        Polygon plg = new Polygon(vts, GetBrush(first.Index), first.Style.GdipPen);

                        if (needRegionUpdate && !config.DisableLineRegion)
                        {
                            plg.RegionData = new ChartRegionData(seriesIndex, first.Index,
                                first.ToolTip, this.RegionDescription);
                        }

                        polygonsWithTang.Add(new PolygonWithTangent(plg, tan));
                    }

                    #region Add point region
                    if (needRegionUpdate)
                    {
                        Region rgn = this.GetRegionFromCircle(secondPoint, second.Style.HitTestRadius);

                        regions.Add(new ChartRegion(rgn, seriesIndex, second.Index,
                            second.ToolTip, this.RegionDescription));
                    }
                    #endregion

                    #region Draw errors bars
                    if (errorBarConfig.Enabled && second.YValues.Length > 1)
                    {
                        BrushInfo interior = this.GetBrush(second.Index);

                        GraphicsPath errorGp = new GraphicsPath();
                        PointF errPt1, errPt2;
                        SizeF errorBarSize = errorBarConfig.SymbolSize;

                        if (errorBarConfig.Orientation == ChartOrientation.Horizontal)
                        {
                            errPt1 = args.GetPoint(second.X + second.YValues[1], second.YValues[0]);
                            errPt2 = args.GetPoint(second.X - second.YValues[1], second.YValues[0]);
                        }
                        else
                        {
                            errPt1 = args.GetPoint(second.X, second.YValues[0] + second.YValues[1]);
                            errPt2 = args.GetPoint(second.X, second.YValues[0] - second.YValues[1]);
                        }

                        RectangleF errRc1 = new RectangleF(errPt1, errorBarSize);
                        RectangleF errRc2 = new RectangleF(errPt2, errorBarSize);

                        errRc1.X -= errRc1.Width / 2;
                        errRc1.Y -= errRc1.Height / 2;
                        errRc2.X -= errRc2.Width / 2;
                        errRc2.Y -= errRc2.Height / 2;

                        GraphicsPath errGp1 = ChartSymbolHelper.GetPathSymbol(errorBarConfig.SymbolShape, Rectangle.Ceiling(errRc1));
                        GraphicsPath errGp2 = ChartSymbolHelper.GetPathSymbol(errorBarConfig.SymbolShape, Rectangle.Ceiling(errRc2));
                        GraphicsPath lineGp = new GraphicsPath();

                        lineGp.AddLine(errPt1, errPt2);

                        PathGroup3D path3D = new PathGroup3D(args.Z);

                        path3D.AddPath(lineGp, null, null, second.Style.GdipPen);
                        path3D.AddPath(errGp1, null, interior, second.Style.GdipPen);
                        path3D.AddPath(errGp2, null, interior, second.Style.GdipPen);

                        args.Graph.AddPolygon(path3D);
                    }
                    #endregion

                    firstPoint = secondPoint;
                    first = second;
                }
                else
                {
                    if (this.Chart.AllowGapForEmptyPoints)
                    first = null;
                }
            }
            #endregion

            polygonsWithTang.Sort(new PolygonWithTangentComparer());

            for (int i = 0; i < polygonsWithTang.Count; i++)
            {
                args.Graph.AddPolygon(((PolygonWithTangent)polygonsWithTang[i]).Polygon);
            }
        }

        /// <summary>
        /// Renders the adornment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="point">The point.</param>
        protected override void RenderAdornment(Graphics g, ChartStyledPoint point)
        {
            ChartErrorBarsConfigItem errorBarsConfig = m_series.ConfigItems.ErrorBars;

            if (errorBarsConfig.Enabled)
            {
                int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
                int errorBarIndex = m_series.PointFormats[ChartYValueUsage.ErrorBarValue];

                ChartSymbolShape errorShape = errorBarsConfig.SymbolShape;
                Size sz = errorBarsConfig.SymbolSize.ToSize();
                PointF firstPoint = PointF.Empty;
                PointF secondPoint = PointF.Empty;

                if (errorBarsConfig.Orientation == ChartOrientation.Horizontal)
                {
                    firstPoint = this.GetPointFromValue(point.X + point.YValues[errorBarIndex], point.YValues[yIndex]);
                    secondPoint = this.GetPointFromValue(point.X - point.YValues[errorBarIndex], point.YValues[yIndex]);
                }
                else
                {
                    firstPoint = this.GetPointFromValue(point.X, point.YValues[yIndex] + point.YValues[errorBarIndex]);
                    secondPoint = this.GetPointFromValue(point.X, point.YValues[yIndex] - point.YValues[errorBarIndex]);
                }

                g.DrawLine(point.Style.GdipPen, firstPoint.X, firstPoint.Y, secondPoint.X, secondPoint.Y);

                using (Brush br = new SolidBrush(point.Style.Interior.BackColor))
                {
                    RenderingHelper.DrawPointSymbol(g, errorShape, null, sz, Size.Empty, 0,
                        br, point.Style.GdipPen, point.Style.Images, firstPoint, false);
                    RenderingHelper.DrawPointSymbol(g, errorShape, null, sz, Size.Empty, 0,
                        br, point.Style.GdipPen, point.Style.Images, secondPoint, false);
                }
            }

            base.RenderAdornment(g, point);
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

        /// <summary>
        /// Updates the points cache.
        /// </summary>
        /// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
        internal override void DataUpdate(ListChangedEventArgs args)
        {
            m_xRange = DoubleRange.Empty;
            m_yRange = DoubleRange.Empty;

            base.DataUpdate(args);
        }

        /// <summary>
        /// Updates by specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        internal override void Update(ChartUpdateFlags flags)
        {
            if ((flags & ChartUpdateFlags.Indexed) == ChartUpdateFlags.Indexed)
            {
                m_xRange = DoubleRange.Empty;
                m_yRange = DoubleRange.Empty;
            }

            base.Update(flags);
        }

        /// <summary>
        /// Measures the X range.
        /// </summary>
        /// <returns></returns>
        public override DoubleRange GetXDataMeasure()
        {
            if (m_xRange.IsEmpty)
            {
                m_xRange = base.GetXDataMeasure();
            }

            return m_xRange;
        }

        /// <summary>
        /// Measures the Y range.
        /// </summary>
        /// <returns></returns>
        public override DoubleRange GetYDataMeasure()
        {
            if (m_yRange.IsEmpty)
            {
                m_yRange = base.GetYDataMeasure();
            }

            return m_yRange;
        }
        #endregion
    }

    #region Helper classes
    /// <summary>
    /// Represents the polygon with the tangent.
    /// </summary>
    internal class PolygonWithTangent
    {
        #region Members
        private Polygon poly;
        private double tangent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the polygon.
        /// </summary>
        /// <value>The polygon.</value>
        public Polygon Polygon
        {
            get
            {
                return poly;
            }

            set
            {
                poly = value;
            }
        }

        /// <summary>
        /// Gets or sets the tangent.
        /// </summary>
        /// <value>The tangent.</value>
        public double Tangent
        {
            get
            {
                return tangent;
            }

            set
            {
                if (tangent != value)
                {
                    tangent = value;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonWithTangent"/> class.
        /// </summary>
        /// <param name="p">The polygon.</param>
        /// <param name="tan">The tangent.</param>
        public PolygonWithTangent(Polygon p, double tan)
        {
            poly = p;
            tangent = tan;
        }
        #endregion
    }

    /// <summary>
    /// Compares the <see cref="PolygonWithTangent"/> by <see cref="PolygonWithTangent.Tangent"/> value.
    /// </summary>
    internal class PolygonWithTangentComparer : IComparer
    {
        #region Implementation
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception>
        int IComparer.Compare(Object x, Object y)
        {
            double tan1 = Math.Abs(((PolygonWithTangent)x).Tangent);
            double tan2 = Math.Abs(((PolygonWithTangent)y).Tangent);

            if (tan1 > tan2)
            {
                return -1;
            }
            else if (tan1 < tan2)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
    #endregion
}