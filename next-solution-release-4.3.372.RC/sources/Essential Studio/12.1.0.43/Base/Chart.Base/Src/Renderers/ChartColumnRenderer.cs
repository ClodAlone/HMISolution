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
    /// Represents the column type renderer.
    /// </summary>
    internal class ColumnRenderer : ChartSeriesRenderer
    {
        #region Properties
        /// <summary>
        /// Indicated how much space this type will fill.
        /// </summary>
        /// <value></value>
        public override ChartUsedSpaceType FillSpaceType
        {
            get
            {
                return Chart.ColumnDrawMode == ChartColumnDrawMode.PlaneMode ?
                    ChartUsedSpaceType.OneForAll : ChartUsedSpaceType.OneForOne;
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
        /// Get description of regions.
        /// </summary>
        /// <value></value>
        protected override string RegionDescription
        {
            get
            {
                return "Column Chart Region";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed width.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fixed width; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool IsFixedWidth
        {
            get
            {
                return Chart.ColumnWidthMode == ChartColumnWidthMode.FixedWidthMode;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public ColumnRenderer(ChartSeries series)
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
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            SizeF cornerRadius = m_series.ConfigItems.ColumnItem.CornerRadius;
            bool isCylinder = m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder;
            bool needRegionUpdate = this.Chart.NeedRegionUpdate;
            double origin = args.ActualYAxis.CurrentOrigin;
            ChartErrorBarsConfigItem errorBarConfig = m_series.ConfigItems.ErrorBars;

            ChartRegionCollection regions = args.Chart.ChartRegions;
            ArrayList pathsList = new ArrayList(m_series.Points.Count);
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (styledPoint.IsVisible && styledPoint.YValues[0] != 0)
                {
                    #region Prepeare coordinates
                    double x1 = 0;
                    double x2 = 0;
                    double y1 = Math.Min(Math.Max(styledPoint.YValues[0], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y2 = Math.Min(Math.Max(origin, args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);                    

                    this.CalculateSides(styledPoint, sbsInfo, out x1, out x2);
                    #endregion

                    if (args.IsVisible(new DoubleRange(x1, x2), new DoubleRange(y1, y2)))
                    {
                        RectangleF rect = this.GetColumnBounds(args, styledPoint, x1, y1, x2, y2);
                        GraphicsPath gPath = null, gPathBoxRight = null, gPathBoxTop=null;

                        #region Draw column
                        if (args.Is3D)
                        {
                            if (isCylinder)
                            {
                                if (args.IsInvertedAxes)
                                {
                                    if (!args.Chart.Style3D)
                                        gPath = this.CreateHorizintalCylinder3D(rect, args.DepthOffset);
                                    else
                                    {
                                        gPath = this.CreateHorizintalCylinder3D(rect, args.DepthOffset);
                                        gPathBoxTop = this.CreateHorizintalCylinder3DTop(rect, args.DepthOffset);
                                    }
                                }
                                else
                                {
                                    if (!args.Chart.Style3D)
                                        gPath = this.CreateVerticalCylinder3D(rect, args.DepthOffset);
                                    else
                                    {
                                        gPath = this.CreateVerticalCylinder3D(rect, args.DepthOffset);
                                        gPathBoxTop = this.CreateVerticalCylinder3DTop(rect, args.DepthOffset);
                                    }
                                }
                            }
                            else
                            {
                                if (!args.Chart.Style3D)
                                    gPath = this.CreateBox(rect, true);
                                else
                                {
                                    gPath = this.CreateBox(rect, true);
                                    gPathBoxRight = this.CreateBoxRight(rect, true);
                                    gPathBoxTop = this.CreateBoxTop(rect, true);
                                }
                            }
                        }
                        else
                        {
                            gPath = RenderingHelper.CreateRoundRect(rect, cornerRadius);

                            if (styledPoint.Style.DisplayShadow)
                            {
                                RectangleF shadowRC = rect;
                                shadowRC.Offset(styledPoint.Style.ShadowOffset.Width,
                                    styledPoint.Style.ShadowOffset.Height);

                                args.Graph.DrawRect(styledPoint.Style.ShadowInterior, null,
                                    shadowRC.X, shadowRC.Y, shadowRC.Width, shadowRC.Height);
                            }
                        }

                        // this point are needed for drawing separator lines
                        if (m_series.DrawColumnSeparatingLines)
                        {
                            PointF sepBLpF = args.GetPoint(styledPoint.X - 0.5d, args.ActualYAxis.Range.Min);
                            PointF sepELpF = args.GetPoint(styledPoint.X - 0.5d, args.ActualYAxis.Range.Min);
                            PointF sepBHpF = args.GetPoint(styledPoint.X - 0.5d, args.ActualYAxis.Range.Max);
                            PointF sepEHpF = args.GetPoint(styledPoint.X - 0.5d, args.ActualYAxis.Range.Max);

                            gPath.CloseFigure();
                            gPath.AddLine(sepBLpF, sepBHpF);
                            gPath.CloseFigure();
                            gPath.AddLine(sepELpF, sepEHpF);
                            gPath.CloseFigure();
                        }

                        #endregion

                        ChartSeriesPath csp = new ChartSeriesPath();
                        if (!args.Chart.Style3D)
                            csp.AddPrimitive(gPath, styledPoint.Style.GdipPen, this.GetBrush(styledPoint.Index));
                        else
                        {                       
                            csp.AddPrimitive(gPath, styledPoint.Style.GdipPen, this.GetBrush(styledPoint.Index));
                            csp.AddPrimitive(gPathBoxRight, styledPoint.Style.GdipPen, this.GetBrush(styledPoint.Index), "BoxRight"); 
                            csp.AddPrimitive(gPathBoxTop, styledPoint.Style.GdipPen, this.GetBrush(styledPoint.Index), "BoxTop");
                         }
                        csp.Bounds = rect;

                        if (needRegionUpdate)
                        {
                            csp.RegionData = new ChartRegionData(args.SeriesIndex, styledPoint.Index,
                                styledPoint.ToolTip, this.RegionDescription);
                        }

                        if (args.Is3D)
                        {
                            pathsList.Add(csp);
                        }
                        else
                        {
                            csp.Draw(args.Graph);

                            if (needRegionUpdate)
                            {
                                regions.Add(csp.GetChartRegion());
                            }
                        }
                    }
                }
            }

            m_segments = (ChartSeriesPath[])pathsList.ToArray(typeof(ChartSeriesPath));
        }

        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            int serIndex = args.SeriesIndex;
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool isCylinder = m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder;
            bool isInvertedAxes = this.IsInvertedAxes;

            ChartErrorBarsConfigItem errorBarConfig = m_series.ConfigItems.ErrorBars;

            double fd = args.Z;
            double bd = fd + args.Depth;
            double origin = m_series.ActualYAxis.Origin;

            args.Graph.AddPolygon(CreateBoundsPolygon((float)fd));

            ChartStyledPoint[] styledPoints = PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (styledPoint.IsVisible && styledPoint.YValues[0] != 0)
                {
                    ChartPoint cpt = styledPoint.Point;
                    ChartStyleInfo style = styledPoint.Style;
                    int cpIndex = styledPoint.Index;
                    BrushInfo interior = this.GetBrush(cpIndex);
                    Pen borderPen = style.GdipPen;

                    PointF currentPoint = this.GetPointFromValue(cpt);

                    if ((dropPoints && !previosPoint.IsEmpty)
                        && ((!isInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
                        || (isInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
                    {
                        continue;
                    }

                    #region Prepeare coordinates
                    double x1 = 0;
                    double x2 = 0;
                    double y1 = styledPoint.YValues[0];
                    double y2 = origin;

                    this.CalculateSides(styledPoint, sbsInfo, out x1, out x2);
                    #endregion

                    RectangleF rect = this.GetColumnBounds(args, styledPoint, x1, y1, x2, y2);

                    #region Render column
                    Polygon[] plgs;

                    Vector3D tlfVector = new Vector3D(rect.Left, rect.Top, fd);
                    Vector3D brbVector = new Vector3D(rect.Right, rect.Bottom, bd);

                    if (args.IsInvertedAxes)
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

                    if (Chart.NeedRegionUpdate)
                    {
                        ChartRegionData crd = new ChartRegionData(serIndex, cpIndex, styledPoint.ToolTip, this.RegionDescription);

                        for (int j = 0, c = plgs.Length; j < c; j++)
                        {
                            plgs[j].RegionData = crd;
                        }
                    }
                    #endregion

                    #region Draw separating lines
                    if (m_series.DrawColumnSeparatingLines)
                    {
                        ChartPoint sepBL = new ChartPoint(cpt.X - 0.5d, m_series.ActualYAxis.Range.Min);
                        ChartPoint sepEL = new ChartPoint(cpt.X - 0.5d, m_series.ActualYAxis.Range.Min);
                        ChartPoint sepBH = new ChartPoint(cpt.X - 0.5d, m_series.ActualYAxis.Range.Max);
                        ChartPoint sepEH = new ChartPoint(cpt.X - 0.5d, m_series.ActualYAxis.Range.Max);
                        PointF sepBLpF = this.GetPointFromValue(sepBL);
                        PointF sepELpF = this.GetPointFromValue(sepEL);
                        PointF sepBHpF = this.GetPointFromValue(sepBH);
                        PointF sepEHpF = this.GetPointFromValue(sepEH);

                        Vector3D[] v1a = new Vector3D[]{ new Vector3D( (double)sepBLpF.X, (double)sepBLpF.Y, fd ), 
                                           new Vector3D( (double)sepBHpF.X, (double)sepBHpF.Y, fd ), 
                                           new Vector3D( (double)sepBHpF.X, (double)sepBHpF.Y, bd ),
                                           new Vector3D( (double)sepBLpF.X, (double)sepBLpF.Y, bd )
                                         };
                        Vector3D[] v2a = new Vector3D[]{ new Vector3D( (double)sepELpF.X, (double)sepELpF.Y, fd ), 
                                           new Vector3D( (double)sepEHpF.X, (double)sepEHpF.Y, fd ), 
                                           new Vector3D( (double)sepEHpF.X, (double)sepEHpF.Y, bd ),
                                           new Vector3D( (double)sepELpF.X, (double)sepELpF.Y, bd )
                                         };

                        args.Graph.AddPolygon(new Polygon(v1a, interior, borderPen));
                        args.Graph.AddPolygon(new Polygon(v2a, interior, borderPen));
                    }
                    #endregion

                    #region Draw errors bars
                    if (errorBarConfig.Enabled && cpt.YValues.Length > 1)
                    {
                        GraphicsPath errorGp = new GraphicsPath();
                        PointF errPt1, errPt2;
                        SizeF errorBarSize = errorBarConfig.SymbolSize;

                        if (errorBarConfig.Orientation == ChartOrientation.Horizontal)
                        {
                            errPt1 = args.GetPoint(styledPoint.X + styledPoint.YValues[1], styledPoint.YValues[0]);
                            errPt2 = args.GetPoint(styledPoint.X - styledPoint.YValues[1], styledPoint.YValues[0]);
                        }
                        else
                        {
                            errPt1 = args.GetPoint(styledPoint.X, styledPoint.YValues[0] + styledPoint.YValues[1]);
                            errPt2 = args.GetPoint(styledPoint.X, styledPoint.YValues[0] - styledPoint.YValues[1]);
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
                        // lineGp.CloseFigure();

                        PathGroup3D path3D = new PathGroup3D(args.Z);

                        path3D.AddPath(lineGp, null, null, borderPen);
                        path3D.AddPath(errGp1, null, interior, borderPen);
                        path3D.AddPath(errGp2, null, interior, borderPen);

                        args.Graph.AddPolygon(path3D);
                    }
                    #endregion

                    previosPoint = currentPoint;
                }
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

            if (errorBarsConfig.Enabled && point.YValues[0] != 0)
            {
                ChartSymbolShape errorShape = errorBarsConfig.SymbolShape;
                Size sz = errorBarsConfig.SymbolSize.ToSize();
                PointF firstPoint = PointF.Empty;
                PointF secondPoint = PointF.Empty;

                if (errorBarsConfig.Orientation == ChartOrientation.Horizontal)
                {
                    firstPoint = this.GetPointFromValue(point.X + point.YValues[1], point.YValues[0]);
                    secondPoint = this.GetPointFromValue(point.X - point.YValues[1], point.YValues[0]);
                }
                else
                {
                    firstPoint = this.GetPointFromValue(point.X, point.YValues[0] + point.YValues[1]);
                    secondPoint = this.GetPointFromValue(point.X, point.YValues[0] - point.YValues[1]);
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
        /// Gets the point by value for series.
        /// </summary>
        /// <param name="chpt">The <see cref="ChartPoint"/>.</param>
        /// <returns>Returns PointF.</returns>
        [Obsolete]
        public override PointF GetPointByValueForSeries(ChartPoint chpt)
        {
            double x1, x2;

            this.CalculateSides(chpt, this.GetSideBySideInfo(), out x1, out x2);

            return base.GetPointByValueForSeries(new ChartPoint(x1 + (x2 - x1) / 2, chpt.YValues));
        }

        /// <summary>
        /// Computes the necessary range of X axis.
        /// </summary>
        /// <returns>Returns the DoubleRange.</returns>
        public override DoubleRange GetXDataMeasure()
        {
            DoubleRange range = base.GetXDataMeasure();

            DoubleRange sbsInfo = new DoubleRange(0, 0);

            if (this.Chart.ColumnWidthMode == ChartColumnWidthMode.DefaultWidthMode)
            {
                sbsInfo = this.GetSideBySideInfo();
            }

            return new DoubleRange(range.Start + sbsInfo.Start, range.End + sbsInfo.End);
        }

        /// <summary>
        /// Calculates the sides.
        /// </summary>
        /// <param name="styledPoint">The <see cref="ChartSeriesRenderer.ChartStyledPoint"/>.</param>
        /// <param name="sbsInfo">The side-by-side info.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        protected void CalculateSides(ChartStyledPoint styledPoint, DoubleRange sbsInfo, out double x1, out double x2)
        {
            if (Chart.ColumnWidthMode != ChartColumnWidthMode.FixedWidthMode)
            {
                x1 = styledPoint.X + sbsInfo.Start;
                x2 = styledPoint.X + sbsInfo.End;
                x1 = Math.Min(Math.Max(x1, m_series.ActualXAxis.Range.Min), m_series.ActualXAxis.Range.Max);
                x2 = Math.Min(Math.Max(x2, m_series.ActualXAxis.Range.Min), m_series.ActualXAxis.Range.Max);

                if (Chart.ColumnWidthMode == ChartColumnWidthMode.RelativeWidthMode)
                {
                    int pointSizeIndex = m_series.PointFormats[ChartYValueUsage.PointSizeValue];

                    if (styledPoint.YValues.Length > pointSizeIndex)
                    {
                        x1 = styledPoint.X - styledPoint.YValues[pointSizeIndex] / 2;
                        x2 = x1 + styledPoint.YValues[pointSizeIndex];
                    }
                }
            }
            else
            {
                x1 = x2 = styledPoint.X;
            }
        }

        /// <summary>
        /// Gets the column bounds.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="stypedPoint">The styped point.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>Returns RectangleF.</returns>
        protected RectangleF GetColumnBounds(ChartRenderArgs args, ChartStyledPoint stypedPoint, double x1, double y1, double x2, double y2)
        {
            RectangleF rect = args.GetRectangle(x1, y1, x2, y2);

            if (this.IsFixedWidth)
            {
                int pointSizeIndex = m_series.PointFormats[ChartYValueUsage.PointSizeValue];
                float width = args.Chart.ColumnFixedWidth;

                if (pointSizeIndex < stypedPoint.YValues.Length)
                {
                    width = (float)stypedPoint.YValues[pointSizeIndex];

                    if (args.IsInvertedAxes)
                    {
                        rect.Inflate(0, width);
                    }
                    else
                    {
                        rect.Inflate(width, 0);
                    }
                }
                else
                {
                    DoubleRange sbsInfo = m_series.ChartModel.GetSideBySideInfo(this.ChartArea, m_series, width);

                    if (args.IsInvertedAxes)
                    {
                        rect.Y -= (float)(sbsInfo.End);
                        rect.Height = (float)(sbsInfo.Delta);
                    }
                    else
                    {
                        rect.X += (float)(sbsInfo.Start);
                        rect.Width = (float)(sbsInfo.Delta);
                    }
                }
            }

            return rect;
        }

        /// <summary>
        /// Checks the column bounds.
        /// </summary>
        /// <param name="isInverted">if set to <c>true</c> axes is inverted.</param>
        /// <param name="rect">The rect.</param>
        protected void CheckColumnBounds(bool isInverted, ref RectangleF rect)
        {
            if (this.IsFixedWidth)
            {
                float width = this.Chart.ColumnFixedWidth;

                if (isInverted)
                {
                    rect.Y += 0.5f * (rect.Height - width);
                    rect.Height = width;
                }
                else
                {
                    rect.X += 0.5f * (rect.Width - width);
                    rect.Width = width;
                }
            }
        }

        /// <summary>
        /// Checks the column bounds.
        /// </summary>
        /// <param name="isInverted">if set to <c>true</c> axes is inverted.</param>
        /// <param name="rect">The rect.</param>
        protected void CheckColumnPoints(bool isInverted, ref RectangleF rect)
        {
            if (this.IsFixedWidth)
            {
                float width = this.Chart.ColumnFixedWidth;

                if (isInverted)
                {
                    rect.Y += 0.5f * (rect.Height - width);
                    rect.Height = width;
                }
                else
                {
                    rect.X += 0.5f * (rect.Width - width);
                    rect.Width = width;
                }
            }
        }

        /// <summary>
        /// Calculates the sides.
        /// </summary>
        /// <param name="cpt">The <see cref="ChartPoint"/>.</param>
        /// <param name="sbsInfo">The side-by-side info.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        protected void CalculateSides(ChartPoint cpt, DoubleRange sbsInfo, out double x1, out double x2)
        {
            x1 = cpt.X + sbsInfo.Start;
            x2 = cpt.X + sbsInfo.End;

            if ((cpt.YValues.Length > 1) && (Chart.ColumnWidthMode == ChartColumnWidthMode.RelativeWidthMode))
            {
                // this part of code is needed for manually setted column widths, 
                // and can be changed. Width should be given in the same measure 
                // units like X values. 
                x1 = cpt.X;
                x2 = x1 + cpt.YValues[1];
            }
            else if ((cpt.YValues.Length > 1) && (Chart.ColumnWidthMode == ChartColumnWidthMode.FixedWidthMode))
            {
                // this part of code is needed for manually setted column widths, 
                // and can be changed. Width should be given in the same measure 
                // units like X values. 
                x1 = cpt.X;
                x2 = x1 + cpt.YValues[1] / DividedIntervalSpace.Width;
            }
        }

        /// <summary>
        /// Gets the symbol coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Returns Synbol Coordinates.</returns>
        protected override PointF GetSymbolCoordinates(ChartStyledPoint point)
        {
            PointF pt;

            double x = point.X;
            double y = point.YValues[0];

            if (m_series.BaseType == ChartSeriesBaseType.SideBySide)
            {
                if (!this.IsFixedWidth)
                {
                    x += this.GetSideBySideInfo().Median;
                }
            }

            if (m_series.BaseStackingType == ChartSeriesBaseStackingType.Stacked)
            {
                y += this.GetStackInfoValue(point.Index);
            }
            else if (m_series.BaseStackingType == ChartSeriesBaseStackingType.FullStacked)
            {
                y = this.GetStackInfoValue(point.Index, true);
            }

            if (m_series.Type == ChartSeriesType.Tornado && point.YValues.Length > 1)
            {
                pt = this.GetPointFromValue(x, point.YValues[1]);
            }
            else
            {
                pt = this.GetPointFromValue(x, y);
            }

            if (this.IsFixedWidth)
            {
                int pointSizeIndex = m_series.PointFormats[ChartYValueUsage.PointSizeValue];
                float width = this.Chart.ColumnFixedWidth;

                if (pointSizeIndex >= point.YValues.Length)
                {
                    DoubleRange sbsInfo = m_series.ChartModel.GetSideBySideInfo(this.ChartArea, m_series, width);

                    if (this.IsInvertedAxes)
                    {
                        pt.Y -= (float)sbsInfo.Median;
                    }
                    else
                    {
                        pt.X += (float)sbsInfo.Median;
                    }
                }
            }

            Size symbolOffset = point.Style.Symbol.Offset;

            pt.X += symbolOffset.Width;
            pt.Y += symbolOffset.Height;

            return pt;
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