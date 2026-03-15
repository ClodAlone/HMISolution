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
    /// Represents the stacking-column type renderer.
    /// </summary>
    internal class StackingColumnRenderer : ColumnRenderer
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
                return ChartUsedSpaceType.OneForAll;
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
                return "Stacking Column Chart Region";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StackingColumnRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public StackingColumnRenderer(ChartSeries series)
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
            bool dropPoints = this.Chart.DropSeriesPoints;
            bool isCylinder = (m_series.ConfigItems.ColumnItem.ColumnType == ChartColumnType.Cylinder);

            DoubleRange sbsInfo = this.GetSideBySideInfo();
            SizeF cornerRadius = m_series.ConfigItems.ColumnItem.CornerRadius;
            IList regions = this.Chart.ChartRegions;

            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange indexedRange = this.CalculateVisibleRange();
            PointF previosPoint = PointF.Empty;

            ArrayList pathsList = new ArrayList(m_series.Points.Count);

            for (int i = indexedRange.From, ci = indexedRange.To + 1; i < ci; i++)
            {
                ChartStyledPoint stlPoint = styledPoints[i];
                ChartStyleInfo style = stlPoint.Style;
                ChartPoint cpt = stlPoint.Point;

                if (stlPoint.IsVisible)
                {
                    PointF currentPoint = args.GetPoint(cpt.X, cpt.YValues[0]);

                    if ((dropPoints && !previosPoint.IsEmpty)
                      && ((!args.IsInvertedAxes && Math.Abs(currentPoint.X - previosPoint.X) < 1f)
                                      || (args.IsInvertedAxes && Math.Abs(currentPoint.Y - previosPoint.Y) < 1f)))
                    {
                        continue;
                    }

                    double x1 = 0;
                    double x2 = 0;
                    double y1 = Math.Min(Math.Max(this.GetStackInfoValue(stlPoint.Index, true), args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y2 = Math.Min(Math.Max(this.GetStackInfoValue(stlPoint.Index, false), args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);

                    this.CalculateSides(stlPoint, sbsInfo, out x1, out x2);
                    x1 = Math.Min(Math.Max(x1, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                    x2 = Math.Min(Math.Max(x2, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max); 
                    RectangleF rect = this.GetColumnBounds(args, stlPoint, x1, y1, x2, y2);
                    GraphicsPath gPath = null;

                    #region Draw column
                    if (args.Is3D)
                    {
                        if (isCylinder)
                        {
                            if (args.IsInvertedAxes)
                            {
                                gPath = this.CreateHorizintalCylinder3D(rect, args.DepthOffset);
                            }
                            else
                            {
                                gPath = this.CreateVerticalCylinder3D(rect, args.DepthOffset);
                            }
                        }
                        else
                        {
                            gPath = this.CreateBox(rect, true);
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

                    ChartSeriesPath csp = new ChartSeriesPath();

                    csp.AddPrimitive(gPath, style.GdipPen, this.GetBrush(stlPoint.Index));
                    csp.Bounds = rect;

                    if (args.UpdateRegions)
                    {
                        csp.RegionData = new ChartRegionData(args.SeriesIndex, stlPoint.Index,
                          stlPoint.ToolTip, this.RegionDescription);
                    }

                    pathsList.Add(csp);
                    previosPoint = currentPoint;
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
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            ArrayList pathsList = new ArrayList(m_series.Points.Count);
            IndexRange visibleRange = this.CalculateVisibleRange();
            DoubleRange sbsInfo = this.GetSideBySideInfo();

            for (int i = visibleRange.From; i <= visibleRange.To; i++)
            {
                ChartStyledPoint stylePoint = styledPoints[i];

                if (stylePoint.IsVisible)
                {
                    double x1 = 0;
                    double x2 = 0;
                    double y1 = this.GetStackInfoValue(stylePoint.Index, true);
                    double y2 = this.GetStackInfoValue(stylePoint.Index, false);

                    this.CalculateSides(stylePoint, sbsInfo, out x1, out x2);
                    RectangleF rc = this.GetColumnBounds(args, stylePoint, x1, y1, x2, y2);

                    Polygon[] plgs;

                    if (args.IsInvertedAxes)
                    {
                        plgs = args.Graph.CreateBox(new Vector3D(rc.Left, rc.Top, args.Z),
                            new Vector3D(rc.Right, rc.Bottom, args.Z + args.Depth),
                            stylePoint.Style.GdipPen, GetBrush(stylePoint.Index));
                    }
                    else
                    {
                        plgs = args.Graph.CreateBoxV(new Vector3D(rc.Left, rc.Top, args.Z),
                            new Vector3D(rc.Right, rc.Bottom, args.Z + args.Depth),
                            stylePoint.Style.GdipPen, GetBrush(stylePoint.Index));
                    }

                    if (args.UpdateRegions)
                    {
                        ChartRegionData crd = new ChartRegionData(args.SeriesIndex, stylePoint.Index,
                            stylePoint.ToolTip, this.RegionDescription);

                        foreach (Polygon pl in plgs)
                        {
                            pl.RegionData = crd;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the point by value for series.
        /// </summary>
        /// <param name="chpt">The <see cref="ChartPoint"/>.</param>
        /// <returns></returns>
        [Obsolete]
        public override PointF GetPointByValueForSeries(ChartPoint chpt)
        {
            ChartPoint newPoint = new ChartPoint(chpt.X, (double[])chpt.YValues.Clone());

            newPoint.YValues[0] = this.GetStackInfoValue(m_series.Points.IndexOf(chpt), true);

            return base.GetPointByValueForSeries(newPoint);
        }

        /// <summary>
        /// Measures the X range.
        /// </summary>
        /// <returns></returns>
        public override DoubleRange GetYDataMeasure()
        {         
            double max = 0;
            double min = 0;

            for (int i = 0; i < m_series.Points.Count; i++)
            {
                double val = this.GetStackInfoValue(i, true);

                if (val > max)
                {
                    max = val;
                }

                if (val < min)
                {
                    min = val;
                }
            }

            DoubleRange range = new DoubleRange(min, max);

            if (m_series.OriginDependent)
            {
                if (m_series.ActualYAxis.CustomOrigin)
                {
                    range += m_series.ActualYAxis.Origin;
                }
                else
                {
                    range += 0d;
                }
            }

            return range;
        }
        #endregion
    }
}