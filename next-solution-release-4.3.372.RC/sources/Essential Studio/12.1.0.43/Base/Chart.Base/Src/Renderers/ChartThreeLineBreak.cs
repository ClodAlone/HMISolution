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
    /// Summary description for ThreeLineBreakRenderer.
    /// </summary>
    internal class ThreeLineBreakRenderer : ChartSeriesRenderer
    {
        #region Constants
        private const int breakLineCount = 3;
        #endregion

        #region TLBRectangle struct
        /// <summary>
        /// The TLBRectangle structure.
        /// </summary>
        protected struct TLBRectangle
        {
            #region Members
            private ChartPoint firstPoint;
            private ChartPoint secondPoint;
            private bool negativeValue;
            private ChartSeriesRenderer r;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the min Y.
            /// </summary>
            /// <value>The min Y.</value>
            public double MinY
            {
                get
                {
                    return Math.Min(firstPoint.YValues[0], secondPoint.YValues[0]);
                }
            }

            /// <summary>
            /// Gets the max Y.
            /// </summary>
            /// <value>The max Y.</value>
            public double MaxY
            {
                get
                {
                    return Math.Max(firstPoint.YValues[0], secondPoint.YValues[0]);
                }
            }

            /// <summary>
            /// Gets a value indicating whether [negative value].
            /// </summary>
            /// <value><c>true</c> if [negative value]; otherwise, <c>false</c>.</value>
            public bool NegativeValue
            {
                get
                {
                    return (negativeValue);
                }
            }

            /// <summary>
            /// Gets or sets the first point.
            /// </summary>
            /// <value>The first point.</value>
            public ChartPoint FirstPoint
            {
                get
                {
                    return firstPoint;
                }

                set
                {
                    firstPoint = value;
                }
            }

            /// <summary>
            /// Gets or sets the second point.
            /// </summary>
            /// <value>The second point.</value>
            public ChartPoint SecondPoint
            {
                get
                {
                    return secondPoint;
                }

                set
                {
                    secondPoint = value;
                }
            }

            /// <summary>
            /// Gets the empty.
            /// </summary>
            /// <value>The empty.</value>
            public static TLBRectangle Empty
            {
                get
                {
                    return new TLBRectangle(ChartPoint.Empty, ChartPoint.Empty, false, null);
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="TLBRectangle"/> struct.
            /// </summary>
            /// <param name="fPoint">The f point.</param>
            /// <param name="sPoint">The s point.</param>
            /// <param name="negVal">if set to <c>true</c> [neg val].</param>
            /// <param name="renderer">The renderer.</param>
            public TLBRectangle(ChartPoint fPoint, ChartPoint sPoint, bool negVal, ChartSeriesRenderer renderer)
            {
                firstPoint = fPoint;
                secondPoint = sPoint;
                negativeValue = negVal;
                r = renderer;
            }
            #endregion
        }
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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeLineBreakRenderer"/> class.
        /// </summary>
        /// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
        public ThreeLineBreakRenderer(ChartSeries series)
            : base(series)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Renders the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs2D args)
        {
            ChartStyleInfo style = SeriesStyle;
            TLBRectangle[] rects = CalcTreeLineBreak();

            BrushInfo brush = this.GetBrush();
            BrushInfo priceUpInterior = this.GetUpPriceInterior(brush);
            BrushInfo priceDownInterior = this.GetDownPriceInterior(brush);

            //===========================================
            int ir = 0, mi = rects.Length, di = 1;

            if (args.ActualXAxis.Inversed)
            {
                ir = mi - di;
                mi = -1;
                di = -1;
            }

            for (; ir != mi; ir += di)
            {
                TLBRectangle rect = rects[ir];
                RectangleF rc = this.GetRectangle(rect);

                if (args.Is3D)
                {
                    rc.Offset(args.Offset.Width, args.Offset.Height);

                    GraphicsPath gp = this.CreateBox(rc, true);

                    if (rect.NegativeValue)
                    {
                        args.Graph.DrawPath(priceDownInterior, style.GdipPen, gp);
                    }
                    else
                    {
                        args.Graph.DrawPath(priceUpInterior, style.GdipPen, gp);
                    }

                    if (args.UpdateRegions)
                    {
                        args.Chart.ChartRegions.Add(new ChartRegion(new Region(gp), args.SeriesIndex,
                            GetToolTip(), "ThreeLineBreak Chart Region"));
                    }
                }
                else
                {
                    if (style.DisplayShadow)
                    {
                        RectangleF shadowRC = rc;
                        shadowRC.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);
                        args.Graph.DrawRect(priceUpInterior, style.GdipPen, shadowRC);
                    }
                    if (rect.NegativeValue)
                    {
                        args.Graph.DrawRect(priceDownInterior, style.GdipPen, rc);
                    }
                    else
                    {
                        args.Graph.DrawRect(priceUpInterior, style.GdipPen, rc);
                    }

                    if (args.UpdateRegions)
                    {
                        args.Chart.ChartRegions.Add(new ChartRegion(new Region(rc), args.SeriesIndex,
                            GetToolTip(), "ThreeLineBreak Chart Region"));
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
            int serIndex = Chart.Series.IndexOf(m_series);
            float serDpth = GetPlaceDepth();
            float dpth = GetSeriesDepth();


            ChartStyleInfo style = SeriesStyle;
            TLBRectangle[] rects = CalcTreeLineBreak();
            int length = rects.Length;

            float maxY = float.MinValue;
            float minY = float.MaxValue;
            for (int i = 0; i < length; i++)
            {
                RectangleF rect = GetRectangle(rects[i]);

                if (maxY < rect.Top)
                {
                    maxY = rect.Top;
                }
                if (minY > rect.Bottom)
                {
                    minY = rect.Bottom;
                }
            }

            Vector3D[] back = new Vector3D[]{
                                        new Vector3D( GetRectangle(rects[ 0 ]).Left, minY, serDpth ),
                                        new Vector3D( GetRectangle(rects[ length - 1 ]).Left, minY, serDpth ),
                                        new Vector3D( GetRectangle(rects[ length - 1 ]).Left , maxY, serDpth ),
                                        new Vector3D( GetRectangle(rects[ 0 ]).Left, maxY, serDpth ),
      };
            Polygon backPoly = new Polygon(back, (BrushInfo)null, (Pen)null);
            g.AddPolygon(backPoly);

            //===========================================
            int ir = 0, mi = rects.Length, di = 1;
            bool yInversed = ChartArea.PrimaryYAxis.Inversed;
            if (ChartArea.PrimaryXAxis.Inversed)
            {
                ir = mi - di;
                mi = -1;
                di = -1;
            }
            for (; ir != mi; ir += di)
            {
                TLBRectangle rc = rects[ir];
                RectangleF rect = GetRectangle(rc);
                Polygon[] plgs = null;

                if (rc.NegativeValue)
                {
                    plgs = g.CreateBoxV(new Vector3D(rect.Left, rect.Top, serDpth),
                      new Vector3D(rect.Right, rect.Bottom, serDpth + dpth), style.GdipPen,
                      new BrushInfo(m_series.ConfigItems.FinancialItem.PriceDownColor));
                }
                else
                {
                    plgs = g.CreateBoxV(new Vector3D(rect.Left, rect.Top, serDpth),
                      new Vector3D(rect.Right, rect.Bottom, serDpth + dpth), style.GdipPen,
                                  new BrushInfo(m_series.ConfigItems.FinancialItem.PriceUpColor));
                }

                if (Chart.NeedRegionUpdate)
                {
                    string s = GetToolTip();
                    ChartRegionData crd = new ChartRegionData(serIndex, s, "ThreeLineBreak Chart Region");

                    for (int j = 0, c = plgs.Length; j < c; j++)
                    {
                        plgs[j].RegionData = crd;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the tree line break.
        /// </summary>
        /// <returns></returns>
        protected TLBRectangle[] CalcTreeLineBreak()
        {
            int pointCount = m_series.Points.Count;
            float[] heights = new float[pointCount];
            ArrayList rects = new ArrayList();

            //===========================================

            for (int i = 1; i < pointCount; i++)
            {
                //====================================
                if (rects.Count == 0)
                {
                    bool negVal = !(m_series.Points[i - 1].YValues[0] < m_series.Points[i].YValues[0]);
                    rects.Add(new TLBRectangle(m_series.Points[i - 1], m_series.Points[i], negVal, this));
                }
                else
                {
                    //==========================================
                    TLBRectangle fRect = (TLBRectangle)rects[rects.Count - 1];
                    TLBRectangle lRect = TLBRectangle.Empty;
                    bool isBreakLine = false;
                    if (rects.Count >= breakLineCount)
                    {
                        lRect = (TLBRectangle)rects[rects.Count - breakLineCount];
                        for (int j = 1; j < breakLineCount; j++)
                        {
                            TLBRectangle cRect = (TLBRectangle)rects[rects.Count - (j)];
                            TLBRectangle nRect = (TLBRectangle)rects[rects.Count - (j + 1)];
                            if (cRect.NegativeValue == nRect.NegativeValue)
                            {
                                isBreakLine = true;
                            }
                            else
                            {
                                isBreakLine = false;
                                break;
                            }
                        }
                    }
                    //==========================================
                    if (((isBreakLine) && (m_series.Points[i].YValues[0] < Math.Min(lRect.MinY, fRect.MinY)))
                      || ((!isBreakLine) && (m_series.Points[i].YValues[0] < fRect.MinY)))
                    {
                        rects.Add(new TLBRectangle(
                          new ChartPoint(fRect.SecondPoint.X, fRect.MinY),
                          m_series.Points[i], true, this));
                    }
                    else if (((isBreakLine) && (m_series.Points[i].YValues[0] > Math.Max(lRect.MaxY, fRect.MaxY)))
                      || ((!isBreakLine) && (m_series.Points[i].YValues[0] > fRect.MaxY)))
                    {
                        rects.Add(new TLBRectangle(
                          new ChartPoint(fRect.SecondPoint.X, fRect.MaxY),
                          m_series.Points[i], false, this));
                    }
                    else
                    {
                        rects[rects.Count - 1] = new TLBRectangle(fRect.FirstPoint,
                          new ChartPoint(m_series.Points[i].X, fRect.SecondPoint.YValues[0]), ((TLBRectangle)rects[rects.Count - 1]).NegativeValue, this);
                    }
                }
            }

            return (TLBRectangle[])rects.ToArray(typeof(TLBRectangle));
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <param name="tlbr">The TLBR.</param>
        /// <returns></returns>
        private RectangleF GetRectangle(TLBRectangle tlbr)
        {
            PointF firstPointF = new PointF(this.GetXFromValue(tlbr.FirstPoint, 0), this.GetYFromValue(tlbr.FirstPoint, 0));
            PointF secondPointF = new PointF(this.GetXFromValue(tlbr.SecondPoint, 0), this.GetYFromValue(tlbr.SecondPoint, 0));
            return new RectangleF(
              Math.Min(firstPointF.X, secondPointF.X),
              Math.Min(firstPointF.Y, secondPointF.Y),
              Math.Abs(secondPointF.X - firstPointF.X),
              Math.Abs(secondPointF.Y - firstPointF.Y));
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
            base.DrawIcon(g, bounds, isShadow, shadowColor);

            if (!isShadow)
            {
                int w4 = bounds.Width / 4;
                int h4 = bounds.Height / 4;

                SolidBrush gsb = new SolidBrush(m_series.ConfigItems.FinancialItem.PriceUpColor);
                SolidBrush rsb = new SolidBrush(m_series.ConfigItems.FinancialItem.PriceDownColor);

                g.FillRectangle(rsb, bounds.Left, bounds.Top + h4, w4, h4);
                g.FillRectangle(rsb, bounds.Left + w4, bounds.Top + 2 * h4, w4, h4);
                g.FillRectangle(gsb, bounds.Left + 2 * w4, bounds.Top + h4, w4, h4);
                g.FillRectangle(gsb, bounds.Left + 3 * w4, bounds.Top, w4, h4);

                gsb.Dispose();
                rsb.Dispose();
            }
        }
        #endregion
    }
}
