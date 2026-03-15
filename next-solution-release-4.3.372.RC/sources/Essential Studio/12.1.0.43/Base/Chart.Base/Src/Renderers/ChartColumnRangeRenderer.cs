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
    /// Summary description for ColumnRangeRenderer.
    /// </summary>
    internal class ColumnRangeRenderer : ColumnRenderer
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
                return "Column Range Chart Region";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed width.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is fixed width; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsFixedWidth
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnRangeRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public ColumnRangeRenderer(ChartSeries series)
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
            bool isInvertedAxes = this.IsInvertedAxes;
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool needRegionUpdate = this.Chart.NeedRegionUpdate;
            bool isCylinder = m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder;
            int seriesIndex = this.Chart.Series.IndexOf(m_series);
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            ChartStyleInfo seriesStyle = this.SeriesStyle;
            SizeF cornerRadius = m_series.ConfigItems.ColumnItem.CornerRadius;
            SizeF seriesOffset = this.GetThisOffset();
            SizeF depthOffset = this.GetSeriesOffset();
            IList regions = this.ChartArea.ChartRegions;
            ChartRenderArgs2D args = new ChartRenderArgs2D(Chart, m_series);
            args.Graph = new ChartGDIGraph(g);

            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;

            ArrayList pathsList = new ArrayList(m_series.Points.Count);

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint stlPoint = styledPoints[i];
                ChartStyleInfo style = stlPoint.Style;
                ChartPoint cpt = stlPoint.Point;

                if (IsVisiblePoint(cpt))
                {
                    PointF currentPoint = this.GetPointFromValue(cpt);

                    if (!Chart.AllowGapForEmptyPoints)
                        currentPoint = this.GetPointFromValue(stlPoint.X, stlPoint.YValues[0]);

                    if ((dropPoints && !previosPoint.IsEmpty)
                      && ((!isInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
                        || (isInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
                    {
                        continue;
                    }

                    #region Prepeare coordinates

                    double x1 = Math.Min(Math.Max(cpt.X + sbsInfo.Start, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                    double x2 = Math.Min(Math.Max(cpt.X + sbsInfo.End, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                    double y1 = Math.Min(Math.Max(cpt.YValues[0], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y2 = Math.Min(Math.Max(cpt.YValues[1], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    RectangleF rect = this.GetRectangle(new ChartPoint(x1, y1), new ChartPoint(x2, y2));

                    if (!Chart.AllowGapForEmptyPoints)
                    {
                        x1 = stlPoint.X + sbsInfo.Start;
                        x2 = stlPoint.X + sbsInfo.End;
                        rect = args.GetRectangle(x1, y1, x2, y2);
                    }

                    #endregion

                    GraphicsPath gPath = null,gPathBoxRight = null, gPathBoxTop = null; ;

                    #region Draw column
                    if (is3d)
                    {
                        rect.Offset(seriesOffset.Width, seriesOffset.Height);
                        gPath = this.CreateBox(rect, is3d);
                        if (!args.Chart.Style3D)
                            gPath = this.CreateBox(rect, is3d);
                        else
                        {
                            gPath = this.CreateBox(rect, is3d);
                            gPathBoxRight = this.CreateBoxRight(rect, is3d);
                            gPathBoxTop = this.CreateBoxTop(rect, is3d);
                        }

                        if (isCylinder)
                        {
                            if (isInvertedAxes)
                            {
                                if (!args.Chart.Style3D)
                                    gPath = this.CreateHorizintalCylinder3D(rect, depthOffset);
                                else
                                {
                                    gPath = this.CreateHorizintalCylinder3D(rect, depthOffset);
                                    gPathBoxTop = this.CreateHorizintalCylinder3DTop(rect, depthOffset);
                                    gPathBoxRight = null;
                                }
                            }
                            else
                            {
                                if (!args.Chart.Style3D)
                                    gPath = this.CreateVerticalCylinder3D(rect, depthOffset);
                                else
                                {
                                    gPath = this.CreateVerticalCylinder3D(rect, depthOffset);
                                    gPathBoxTop = this.CreateVerticalCylinder3DTop(rect, depthOffset);
                                    gPathBoxRight = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        gPath = RenderingHelper.CreateRoundRect(rect, cornerRadius);

                        if (stlPoint.Style.DisplayShadow)
                        {
                            RectangleF shadowRC = rect;
                            shadowRC.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);
                            BrushPaint.FillRectangle(g, shadowRC, style.ShadowInterior);
                        }
                    }
                    #endregion

                    ChartSeriesPath csp = new ChartSeriesPath();
                    if (!args.Chart.Style3D)
                        csp.AddPrimitive(gPath, style.GdipPen, this.GetBrush(stlPoint.Index));
                    else
                    {
                        csp.AddPrimitive(gPath, null, BrushInfo.Empty);

                        ChartSeriesPath csp1 = new ChartSeriesPath();
                        csp1.AddPrimitive(gPath, style.GdipPen, this.GetBrush(stlPoint.Index), "BoxCenter");
                        csp1.Draw(args.Graph, "BoxCenter");

                        ChartSeriesPath csp2 = new ChartSeriesPath();
                        csp2.AddPrimitive(gPathBoxRight, style.GdipPen, this.GetBrush(stlPoint.Index), "BoxRight");
                        csp2.Draw(args.Graph, "BoxRight");

                        ChartSeriesPath csp3 = new ChartSeriesPath();
                        csp3.AddPrimitive(gPathBoxTop, style.GdipPen, this.GetBrush(stlPoint.Index),"BoxTop");
                        csp3.Draw(args.Graph, "BoxTop");
                    }
                     
                    csp.Bounds = rect;

                    if (needRegionUpdate)
                    {
                        csp.RegionData = new ChartRegionData(seriesIndex, stlPoint.Index, stlPoint.ToolTip, this.RegionDescription);
                    }

                    if (is3d && Chart.ColumnDrawMode == ChartColumnDrawMode.PlaneMode)
                    {
                        pathsList.Add(csp);
                    }
                    else
                    {
                        csp.Draw(g);

                        if (needRegionUpdate)
                        {
                            regions.Add(csp.GetChartRegion());
                        }
                    }

                    previosPoint = currentPoint;
                }
            }

            m_segments = (ChartSeriesPath[])pathsList.ToArray(typeof(ChartSeriesPath));
        }

        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>
        /// <param name="g">The graphics object that is to be used for rendering.</param>
        public override void Render(Graphics3D g)
        {
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            int serIndex = this.Chart.Series.IndexOf(m_series);
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool isCylinder = m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder;
            bool isInvertedAxes = this.IsInvertedAxes;

            float fd = GetPlaceDepth();
            float bd = fd + GetSeriesDepth();
            double origin = m_series.ActualYAxis.Origin;

            g.AddPolygon(CreateBoundsPolygon(fd));

            ChartStyledPoint[] styledPoints = PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;
            ChartRenderArgs3D args = new ChartRenderArgs3D(Chart, m_series);

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];
                ChartPoint cpt = styledPoint.Point;
                ChartStyleInfo style = styledPoint.Style;
                int cpIndex = styledPoint.Index;
                BrushInfo interior = this.GetBrush(cpIndex);
                Pen borderPen = style.GdipPen;

                if (IsVisiblePoint(cpt))
                {
                    PointF currentPoint = this.GetPointFromValue(cpt);

                    if (!Chart.AllowGapForEmptyPoints)
                        currentPoint = this.GetPointFromValue(styledPoint.X, styledPoint.YValues[0]);

                    if ((dropPoints && !previosPoint.IsEmpty)
                      && ((!isInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
                        || (isInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
                    {
                        continue;
                    }

                    #region Prepeare coordinates
                    double x1 = cpt.X + sbsInfo.Start;
                    double x2 = cpt.X + sbsInfo.End;
                    double y1 = cpt.YValues[0];
                    double y2 = cpt.YValues[1];
                    RectangleF rect = this.GetRectangle(new ChartPoint(x1, y1), new ChartPoint(x2, y2));

                    if (!Chart.AllowGapForEmptyPoints)
                    {
                        x1 = styledPoint.X + sbsInfo.Start;
                        x2 = styledPoint.X + sbsInfo.End;
                        rect = args.GetRectangle(x1, y1, x2, y2);
                    }
                    #endregion

                    #region Render column
                    Polygon[] plgs;

                    Vector3D tlfVector = new Vector3D(rect.Left, rect.Top, fd);
                    Vector3D brbVector = new Vector3D(rect.Right, rect.Bottom, bd);

                    if (m_series.Rotate)
                    {
                        if (isCylinder)
                        {
                            g.CreateBox(tlfVector, brbVector, null, (BrushInfo)null);
                            plgs = g.CreateCylinderH(tlfVector, brbVector, POLYGON_SECTORS, borderPen, interior);
                        }
                        else
                        {
                            plgs = g.CreateBox(tlfVector, brbVector, borderPen, interior);
                        }
                    }
                    else
                    {
                        if (isCylinder)
                        {
                            g.CreateBoxV(tlfVector, brbVector, null, (BrushInfo)null);
                            plgs = g.CreateCylinderV(tlfVector, brbVector, POLYGON_SECTORS, borderPen, interior);
                        }
                        else
                        {
                            plgs = g.CreateBoxV(tlfVector, brbVector, borderPen, interior);
                        }
                    }

                    if (Chart.NeedRegionUpdate)
                    {
                        ChartRegionData crd = new ChartRegionData(serIndex, cpIndex, styledPoint.ToolTip, this.RegionDescription);

                        for (int j = 0, c = plgs.Length; j < c; j++)
                        {
                            plgs[j].RegionData = crd;
                        }
                    }
                    #endregion

                    previosPoint = currentPoint;
                }
            }
        }
        #endregion
    }
}
