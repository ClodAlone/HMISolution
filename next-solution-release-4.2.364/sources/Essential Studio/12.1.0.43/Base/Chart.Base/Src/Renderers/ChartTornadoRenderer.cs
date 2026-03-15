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
    /// The Tornado chart Renderering class.
    /// </summary>
    internal class TornadoRenderer : BarRenderer
    {
        #region properties
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
                return "Tornado Chart Region";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TornadoRenderer"/> class.
        /// </summary>
        /// <param name="series"></param>
        public TornadoRenderer(ChartSeries series)
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
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange visibleRange = this.CalculateVisibleRange();

            int lowIndex = args.Series.PointFormats[ChartYValueUsage.LowValue];
            int highIndex = args.Series.PointFormats[ChartYValueUsage.HighValue];

            ArrayList pathsList = new ArrayList(m_series.Points.Count);

            for (int i = visibleRange.From; i <= visibleRange.To; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (styledPoint.IsVisible)
                {
                    double x1 = Math.Min(Math.Max(styledPoint.X + sbsInfo.Start, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                    double x2 = Math.Min(Math.Max(styledPoint.X + sbsInfo.End, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                    double y1 = Math.Min(Math.Max(styledPoint.YValues[lowIndex], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);
                    double y2 = Math.Min(Math.Max(styledPoint.YValues[highIndex], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);

                    PointF pt1 = args.GetPoint(x1, y1);
                    PointF pt2 = args.GetPoint(x2, y2);

                    RectangleF rc = ChartMath.CorrectRect(pt1.X, pt1.Y, pt2.X, pt2.Y);
                    ChartSeriesPath path = new ChartSeriesPath();

                    path.Bounds = rc;

                    if (args.Is3D)
                    {
                        path.AddPrimitive(this.CreateBox(rc, true), styledPoint.Style.GdipPen, this.GetBrush(styledPoint.Index));
                    }
                    else
                    {
                        if (styledPoint.Style.DisplayShadow)
                        {
                            RectangleF shadowRC = rc;
                            shadowRC.Offset(styledPoint.Style.ShadowOffset.Width, styledPoint.Style.ShadowOffset.Height);
                            args.Graph.DrawRect(styledPoint.Style.ShadowInterior, null, shadowRC);
                        }

                        GraphicsPath gp = new GraphicsPath();
                        gp.AddRectangle(rc);

                        path.AddPrimitive(gp, styledPoint.Style.GdipPen, this.GetBrush(styledPoint.Index));
                    }

                    if (args.UpdateRegions)
                    {
                        path.RegionData = new ChartRegionData(args.SeriesIndex, styledPoint.Index,
                            styledPoint.ToolTip, this.RegionDescription);
                    }

                    if (args.Is3D)
                    {
                        pathsList.Add(path);
                    }
                    else
                    {
                        path.Draw(args.Graph);

                        if (args.UpdateRegions)
                        {
                            this.Chart.ChartRegions.Add(path.GetChartRegion());
                        }
                    }
                }
            }

            if (args.Is3D)
            {
                m_segments = (ChartSeriesPath[])pathsList.ToArray(typeof(ChartSeriesPath));
            }
            else
            {
                m_segments = null;
            }
        }

        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            DoubleRange sbsInfo = this.GetSideBySideInfo();
            ChartStyledPoint[] styledPoints = this.PrepearePoints();
            IndexRange visibleRange = this.CalculateVisibleRange();

            int lowIndex = args.Series.PointFormats[ChartYValueUsage.LowValue];
            int highIndex = args.Series.PointFormats[ChartYValueUsage.HighValue];

            double fd = args.Z;
            double bd = args.Z + args.Depth;

            for (int i = visibleRange.From; i <= visibleRange.To; i++)
            {
                ChartStyledPoint styledPoint = styledPoints[i];

                if (styledPoint.IsVisible)
                {
                    double x1 = styledPoint.X + sbsInfo.Start;
                    double x2 = styledPoint.X + sbsInfo.End;
                    double y1 = styledPoint.YValues[lowIndex];
                    double y2 = styledPoint.YValues[highIndex];

                    PointF pt1 = args.GetPoint(x1, y1);
                    PointF pt2 = args.GetPoint(x2, y2);

                    Polygon[] plgs = args.Graph.CreateBox(new Vector3D(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y), fd),
                        new Vector3D(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y), bd), styledPoint.Style.GdipPen, GetBrush(styledPoint.Index));

                    if (args.UpdateRegions)
                    {
                        ChartRegionData crd = new ChartRegionData(args.SeriesIndex, styledPoint.Index,
                            styledPoint.ToolTip, this.RegionDescription);

                        for (int j = 0, c = plgs.Length; j < c; j++)
                        {
                            plgs[j].RegionData = crd;
                        }
                    }
                }
            }
        }
        #endregion
    }
}