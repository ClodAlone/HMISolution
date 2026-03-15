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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;


namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The Candle Chart rendering class.
    /// </summary>
    internal class CandleRenderer : ColumnRenderer
    {
        #region Properties
        /// <summary>
        /// Gets count of require Y values of the points.
        /// </summary>
        /// <value>The Require YValues Count.</value>       
        protected override int RequireYValuesCount
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Get description of regions.
        /// </summary>
        /// <value>The RegionDescription. </value>
        protected override string RegionDescription
        {
            get
            {
                return "Candle Chart Region";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CandleRenderer"/> class.
        /// </summary>
        /// <param name="series">The ChartSeries.</param>
        public CandleRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does
        /// the rendering.
        /// </summary>		
        /// <param name="args">The graphics object that is to be used for rendering.</param>
        public override void Render(ChartRenderArgs2D args)
        {
            bool isInvertedAxes = this.IsInvertedAxes;
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool isCylinder = m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder;
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            ChartStyleInfo seriesStyle = this.SeriesStyle;
            SizeF cornerRadius = m_series.ConfigItems.ColumnItem.CornerRadius;
            SizeF seriesOffset = this.GetThisOffset();
            SizeF depthOffset = this.GetSeriesOffset();
            SizeF halfOffset = new SizeF(depthOffset.Width / 2, depthOffset.Height / 2);
            SizeF lineOffset1 = new SizeF(halfOffset.Width / 2, halfOffset.Height / 2);
            SizeF lineOffset2 = new SizeF(lineOffset1.Width + halfOffset.Width, lineOffset1.Height + halfOffset.Height);
            IList regions = this.ChartArea.ChartRegions;
            Pen widenPen = new Pen(Color.Black);

            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint stlPoint = styledPoints[i];
                ChartStyleInfo style = stlPoint.Style;
                ChartPoint cpt = stlPoint.Point;
                BrushInfo interior = this.GetBrush(stlPoint.Index);
                Pen borderPen = style.GdipPen;
                widenPen.Width = borderPen.Width + 2;

                if (stlPoint.IsVisible)
                {
                    PointF currentPoint = this.GetPointFromValue(cpt);

                    if (!Chart.AllowGapForEmptyPoints)
                    {
                        currentPoint = this.GetPointFromValue(stlPoint.X, stlPoint.YValues[0]);
                    }

                    if ((dropPoints && !previosPoint.IsEmpty)
                      && ((!isInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
                        || (isInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
                    {
                        continue;
                    }

                    #region Prepeare coordinates
                    double x1 = Math.Min(Math.Max(cpt.X, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max) + sbsInfo.Start;
                    double x2 = Math.Min(Math.Max(cpt.X, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max) + sbsInfo.End;
                    double y1 = Math.Min(Math.Max(cpt.YValues[0], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y2 = Math.Min(Math.Max(cpt.YValues[1], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y3 = Math.Min(Math.Max(cpt.YValues[2], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y4 = Math.Min(Math.Max(cpt.YValues[3], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    #endregion

                    PointF p1 = this.GetPointFromValue(new ChartPoint((x1 + x2) / 2, y1));
                    PointF p2 = this.GetPointFromValue(new ChartPoint((x1 + x2) / 2, y2));
                    RectangleF rect = this.GetRectangle(new ChartPoint(x1, y3), new ChartPoint(x2, y4));

                    if (!Chart.AllowGapForEmptyPoints)
                    {
                        x1 = stlPoint.X + sbsInfo.Start;
                        x2 = stlPoint.X + sbsInfo.End;
                        p1 = args.GetPoint((x1 + x2) / 2, y1);
                        p2 = args.GetPoint((x1 + x2) / 2, y2);
                        rect = args.GetRectangle(x1, y3, x2, y4);
                    }

                    this.CheckColumnBounds(args.IsInvertedAxes, ref rect);

                    GraphicsPath gPath = null, gPathBoxRight = null, gPathBoxTop = null;

                    #region Draw column
                    if (args.Is3D)
                    {
                        p1 = ChartMath.AddPoint(p1, seriesOffset);
                        p2 = ChartMath.AddPoint(p2, seriesOffset);
                        rect.Offset(seriesOffset.Width, seriesOffset.Height);
                        if (!args.Chart.Style3D)
                            gPath = this.CreateBox(rect, args.Is3D);
                        else
                        {
                            gPath = this.CreateBox(rect, args.Is3D);
                            gPathBoxRight = this.CreateBoxRight(rect, args.Is3D);
                            gPathBoxTop = this.CreateBoxTop(rect, args.Is3D);
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
                            args.Graph.DrawRect(style.ShadowInterior, null, shadowRC);
                        }
                    }
                    #endregion

                    Region rgn = args.UpdateRegions ? new Region(rect) : null;

                    if (args.Is3D)
                    {
                        GraphicsPath gLine = new GraphicsPath();

                        gLine.AddPolygon(new PointF[] { ChartMath.AddPoint(p1, lineOffset1),
              ChartMath.AddPoint(p2, lineOffset1),
              ChartMath.AddPoint(p2, lineOffset2),
              ChartMath.AddPoint(p1, lineOffset2) });

                        args.Graph.DrawPath(interior, borderPen, gLine);

                        if (rgn != null)
                        {
                            rgn.Union(gLine);
                        }
                    }
                    else
                    {
                        args.Graph.DrawLine(borderPen, p1, p2);

                        if (rgn != null && p1 != p2)
                        {
                            GraphicsPath gLine = new GraphicsPath();

                            gLine.AddLine(p1, p2);
                            gLine.Widen(widenPen);

                            rgn.Union(gLine);
                        }
                    }

                    ChartSeriesPath csp = new ChartSeriesPath();
                    if (!args.Chart.Style3D)
                        args.Graph.DrawPath(interior, borderPen, gPath);
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
                        csp3.AddPrimitive(gPathBoxTop, style.GdipPen, this.GetBrush(stlPoint.Index));
                        csp3.Draw(args.Graph, "BoxTop");
                    }

                    if (rgn != null)
                    {
                        rgn.Union(gPath);
                        gPath.Widen(widenPen);
                        rgn.Union(gPath);
                    }

                    if (args.Is3D)
                    {
                        if (rect.Top < Math.Max(p1.Y, p2.Y)
                          && rect.Top > Math.Min(p1.Y, p2.Y))
                        {
                            GraphicsPath gLine = new GraphicsPath();

                            PointF spt = new PointF(p1.X, rect.Top);
                            PointF ept = new PointF(p2.X, Math.Min(p2.Y, p1.Y));

                            gLine.AddPolygon(new PointF[] { ChartMath.AddPoint(spt, lineOffset1),
                ChartMath.AddPoint(ept, lineOffset1),
                ChartMath.AddPoint(ept, lineOffset2),
                ChartMath.AddPoint(spt, lineOffset2) });

                            args.Graph.DrawPath(interior, borderPen, gLine);

                            if (rgn != null)
                            {
                                rgn.Union(gLine);
                            }
                        }
                        else if (isInvertedAxes && rect.Right < Math.Max(p1.X, p2.X)
                          && rect.Right > Math.Min(p1.X, p2.X))
                        {
                            GraphicsPath gLine = new GraphicsPath();

                            PointF spt = new PointF(rect.Right, p1.Y);
                            PointF ept = new PointF(Math.Max(p2.X, p1.X), p2.Y);

                            gLine.AddPolygon(new PointF[] { ChartMath.AddPoint(spt, lineOffset1),
                ChartMath.AddPoint(ept, lineOffset1),
                ChartMath.AddPoint(ept, lineOffset2),
                ChartMath.AddPoint(spt, lineOffset2) });

                            args.Graph.DrawPath(interior, borderPen, gLine);

                            if (rgn != null)
                            {
                                rgn.Union(gLine);
                            }
                        }
                    }

                    if (args.UpdateRegions)
                    {
                        regions.Add(new ChartRegion(rgn, args.SeriesIndex, stlPoint.Index,
              stlPoint.ToolTip, this.RegionDescription));
                    }

                    previosPoint = currentPoint;
                }
            }

            widenPen.Dispose();
        }

        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The ChartRenderArgs3 args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            int serIndex = this.Chart.Series.IndexOf(m_series);
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool isCylinder = m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder;
            bool isInvertedAxes = this.IsInvertedAxes;

            int lowIndex = m_series.PointFormats[ChartYValueUsage.LowValue];
            int highIndex = m_series.PointFormats[ChartYValueUsage.HighValue];
            int openIndex = m_series.PointFormats[ChartYValueUsage.OpenValue];
            int closeIndex = m_series.PointFormats[ChartYValueUsage.CloseValue];

            float fd = GetPlaceDepth();
            float bd = fd + GetSeriesDepth();
            double origin = m_series.ActualYAxis.Origin;

            args.Graph.AddPolygon(CreateBoundsPolygon(fd));

            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];
                ChartPoint cpt = styledPoint.Point;
                ChartStyleInfo style = styledPoint.Style;
                int cpIndex = styledPoint.Index;
                BrushInfo interior = this.GetBrush(cpIndex);
                Pen borderPen = style.GdipPen;

                if (styledPoint.IsVisible)
                {
                    PointF currentPoint = args.GetPoint(styledPoint.X, styledPoint.YValues[lowIndex]);

                    if ((dropPoints && !previosPoint.IsEmpty)
                      && ((!isInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
                        || (isInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
                    {
                        continue;
                    }

                    double x1 = styledPoint.X + sbsInfo.Start;
                    double x2 = styledPoint.X + sbsInfo.End;

                    double y1 = styledPoint.YValues[lowIndex];
                    double y2 = styledPoint.YValues[highIndex];
                    double y3 = styledPoint.YValues[openIndex];
                    double y4 = styledPoint.YValues[closeIndex];

                    PointF p1 = args.GetPoint((x1 + x2) / 2, y1);
                    PointF p2 = args.GetPoint((x1 + x2) / 2, y2);
                    RectangleF rect = args.GetRectangle(x1, y3, x2, y4);

                    this.CheckColumnBounds(args.IsInvertedAxes, ref rect);

                    #region Render column
                    Polygon[] plgs;

                    Vector3D tlfVector = new Vector3D(rect.Left, rect.Top, fd);
                    Vector3D brbVector = new Vector3D(rect.Right, rect.Bottom, bd);

                    if (m_series.Rotate)
                    {
                        if (isCylinder)
                        {
                            args.Graph.CreateBox(tlfVector, brbVector, null, (BrushInfo)null);
                            plgs = args.Graph.CreateCylinderH(tlfVector, brbVector, POLYGON_SECTORS, borderPen, interior);
                        }
                        else
                        {
                            plgs = args.Graph.CreateBox(tlfVector, brbVector, borderPen, interior);
                        }
                    }
                    else
                    {
                        if (isCylinder)
                        {
                            args.Graph.CreateBoxV(tlfVector, brbVector, null, (BrushInfo)null);
                            plgs = args.Graph.CreateCylinderV(tlfVector, brbVector, POLYGON_SECTORS, borderPen, interior);
                        }
                        else
                        {
                            plgs = args.Graph.CreateBoxV(tlfVector, brbVector, borderPen, interior);
                        }
                    }

                    if (args.UpdateRegions)
                    {
                        ChartRegionData crd = new ChartRegionData(serIndex, cpIndex, styledPoint.ToolTip, this.RegionDescription);

                        for (int j = 0, c = plgs.Length; j < c; j++)
                        {
                            plgs[j].RegionData = crd;
                        }
                    }
                    #endregion

                    Polygon pol = new Polygon(new Vector3D[]{
            new Vector3D( p1.X, p1.Y, fd + (bd - fd) / 3 ),
            new Vector3D( p1.X, p1.Y, fd + 2 * (bd - fd) / 3 ),
            new Vector3D( p2.X, p2.Y, fd + 2 * (bd - fd) / 3 ), 
            new Vector3D( p2.X, p2.Y, fd + (bd - fd) / 3) },
                        interior, borderPen);
                    args.Graph.AddPolygon(pol);

                    previosPoint = currentPoint;
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