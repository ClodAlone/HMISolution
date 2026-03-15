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
using Syncfusion.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// The StepLine chart Renderering class.
    /// </summary>
    internal class StepLineRenderer : LineRenderer
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
                return "Step Line Region";
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
        /// Initializes a new instance of the <see cref="StepLineRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public StepLineRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Implementation
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
            bool stepInverted = m_series.ConfigItems.StepItem.Inverted;

            int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];

            ChartStyleInfo seriesStyle = this.SeriesStyle;
            SizeF seriesOffset = this.GetThisOffset();
            SizeF depthOffset = this.GetSeriesOffset();

            IList regions = this.ChartArea.ChartRegions;

            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange indexedRange = this.CalculateVisibleRange();

            ChartStyledPoint first = null;
            ChartStyledPoint second = null;
            PointF firstPoint = PointF.Empty;
            PointF secondPoint = PointF.Empty;

            #region Draw lines
            int from = inversed ? indexedRange.To : 0;
            int to = inversed ? -1 : indexedRange.To + 1;
            int di = inversed ? -1 : 1;

            for (int i = from; i != to; i += di)
            {
                second = styledPoints[i];

                if (second.IsVisible)
                {
                    secondPoint = args.GetPoint(second.X, second.YValues[yIndex]);

                    if (first != null)
                    {
                        PointF stepPoint = stepInverted ?
                            args.GetPoint(first.X, second.YValues[yIndex]) : args.GetPoint(second.X, first.YValues[yIndex]);

                        if (args.Is3D)
                        {
                            #region Draw 3D line
                            GraphicsPath gp = new GraphicsPath();
                            GraphicsPath sgp = new GraphicsPath();

                            gp.AddLine(firstPoint, ChartMath.AddPoint(firstPoint, depthOffset));
                            gp.AddLine(ChartMath.AddPoint(stepPoint, depthOffset), stepPoint);
                            gp.CloseFigure();

                            sgp.AddLine(stepPoint, ChartMath.AddPoint(stepPoint, depthOffset));
                            sgp.AddLine(ChartMath.AddPoint(secondPoint, depthOffset), secondPoint);
                            sgp.CloseFigure();

                            args.Graph.DrawPath(this.GetBrush(first.Index), first.Style.GdipPen, gp);
                            args.Graph.DrawPath(this.GetBrush(first.Index), first.Style.GdipPen, sgp);

                            if (args.UpdateRegions)
                            {
                                Region rgn = new Region(gp);

                                rgn.Union(sgp);

                                regions.Add(new ChartRegion(rgn, args.SeriesIndex, first.Index,
                                    first.ToolTip, this.RegionDescription));
                            }
                            #endregion
                        }
                        else
                        {
                            #region Draw 2D line
                            using (Pen pen = first.Style.GdipPen.Clone() as Pen)
                            {
                                GraphicsPath gp = new GraphicsPath();

                                gp.AddLine(firstPoint, stepPoint);
                                gp.AddLine(stepPoint, secondPoint);

                                if (first.Style.DisplayShadow)
                                {
                                    Size sdwOffset = first.Style.ShadowOffset;

                                    pen.Color = first.Style.ShadowInterior.BackColor;

                                    args.Graph.PushTranfsorm();
                                    args.Graph.Transform = new Matrix(1, 0, 0, 1, sdwOffset.Width, sdwOffset.Height);
                                    args.Graph.DrawPath(pen, gp);
                                    args.Graph.PopTransform();
                                }

                                pen.Color = this.GetBrush(first.Index).BackColor;
                                args.Graph.DrawPath(pen, gp);

                                if (args.UpdateRegions)
                                {
                                    if (Math.Abs(firstPoint.X - secondPoint.X) > 1 || Math.Abs(firstPoint.Y - secondPoint.Y) > 1)
                                    {
                                        gp.Widen(pen);

                                        regions.Add(new ChartRegion(new Region(gp), args.SeriesIndex, first.Index,
                                            first.ToolTip, this.RegionDescription));
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    #region Add point region
                    if (args.UpdateRegions)
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
            int seriesIndex = this.Chart.Series.IndexOf(m_series);
            int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];
            bool stepInverted = m_series.ConfigItems.StepItem.Inverted;

            float fd = GetPlaceDepth();
            float dpth = GetSeriesDepth();
            float bd = fd + dpth;

            ChartStyleInfo seriesStyle = this.SeriesStyle;
            IList regions = this.ChartArea.ChartRegions;
            ChartStyledPoint[] styledPoints = PrepearePoints();
            IndexRange indexedRange = CalculateVisibleRange();

            ChartStyledPoint first = null;
            ChartStyledPoint second = null;
            PointF firstPoint = PointF.Empty;
            PointF secondPoint = PointF.Empty;

            args.Graph.AddPolygon(CreateBoundsPolygon(fd));

            ArrayList vPoly = new ArrayList(styledPoints.Length);
            ArrayList hPoly = new ArrayList(styledPoints.Length);

            #region Draw lines
            for (int i = indexedRange.From, ci = indexedRange.To + 1; i != ci; i++)
            {
                second = styledPoints[i];

                if (!second.Point.IsEmpty)
                {
                    secondPoint = args.GetPoint(second.X, second.YValues[yIndex]);

                    if (first != null)
                    {
                        PointF stepPoint = stepInverted ?
                            args.GetPoint(first.X, second.YValues[yIndex]) : args.GetPoint(second.X, first.YValues[yIndex]);

                        Vector3D[] vts1 = new Vector3D[]{ 
              new Vector3D( stepPoint.X, stepPoint.Y, fd ),
              new Vector3D( firstPoint.X, firstPoint.Y, fd ),
              new Vector3D( firstPoint.X, firstPoint.Y, bd ),
              new Vector3D( stepPoint.X, stepPoint.Y, bd )};

                        Vector3D[] vts2 = new Vector3D[]{ 
              new Vector3D( secondPoint.X, secondPoint.Y, fd ),
              new Vector3D( stepPoint.X, stepPoint.Y, fd ),
              new Vector3D( stepPoint.X, stepPoint.Y, bd ),
              new Vector3D( secondPoint.X, secondPoint.Y, bd )};

                        Polygon plg1 = new Polygon(vts1, GetBrush(first.Index), first.Style.GdipPen);
                        Polygon plg2 = new Polygon(vts2, GetBrush(first.Index), first.Style.GdipPen);

                        if (needRegionUpdate)
                        {
                            plg1.RegionData = new ChartRegionData(seriesIndex, first.Index,
                                first.ToolTip, this.RegionDescription);
                            plg2.RegionData = new ChartRegionData(seriesIndex, first.Index,
                                first.ToolTip, this.RegionDescription);
                        }

                        vPoly.Add(plg1);
                        hPoly.Add(plg2);
                    }

                    #region Add point region
                    if (needRegionUpdate)
                    {
                        PointF spt = this.ChartArea.Transform3D.ToScreen(new Vector3D(secondPoint.X, secondPoint.Y, args.Z));
                        Region rgn = this.GetRegionFromCircle(spt, second.Style.HitTestRadius);

                        regions.Add(new ChartRegion(rgn, seriesIndex, second.Index, second.ToolTip, this.RegionDescription));
                    }
                    #endregion

                    firstPoint = secondPoint;
                    first = second;
                }
                else
                {
                    first = null;
                }
            }
            #endregion

            if (stepInverted)
            {
                for (int i = 0; i < vPoly.Count; i++)
                {
                    args.Graph.AddPolygon(vPoly[i] as Polygon);
                }

                for (int i = 0; i < hPoly.Count; i++)
                {
                    args.Graph.AddPolygon(hPoly[i] as Polygon);
                }
            }
            else
            {
                for (int i = 0; i < hPoly.Count; i++)
                {
                    args.Graph.AddPolygon(hPoly[i] as Polygon);
                }

                for (int i = 0; i < vPoly.Count; i++)
                {
                    args.Graph.AddPolygon(vPoly[i] as Polygon);
                }
            }
        }
        #endregion
    }
}