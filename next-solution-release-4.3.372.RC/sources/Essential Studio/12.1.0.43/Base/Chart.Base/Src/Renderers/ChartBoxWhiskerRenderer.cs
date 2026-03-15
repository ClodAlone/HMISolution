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
using System.Collections;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
    /// <summary>
    /// Provides the rendering of "Box and Whisker" chart type.
    /// </summary>
    internal class BoxWhiskerRenderer : ColumnRenderer
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
                return 1;
            }
        }

        /// <summary>
        /// Get description of regions.
        /// </summary>
        /// <value>The Region Description.</value>
        protected override string RegionDescription
        {
            get
            {
                return "Box and Whisker Chart Region";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxWhiskerRenderer"/> class.
        /// </summary>
        /// <param name="series"></param>
        public BoxWhiskerRenderer(ChartSeries series)
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
            double axisFactor = (m_series.ActualXAxis.RealLength * m_series.ActualYAxis.VisibleRange.Delta) / (m_series.ActualXAxis.VisibleRange.Delta * m_series.ActualYAxis.RealLength);

            DoubleRange sbsInfo = this.GetSideBySideInfo();
            ChartStyledPoint[] stylePoints = this.PrepearePoints();
            IndexRange visibleRange = this.CalculateVisibleRange();
            ArrayList pathsList = new ArrayList();

            double dotWidth = 0.25 * sbsInfo.Delta;           

            if (m_series.ConfigItems.BoxAndWhiskerItem.OutLierWidth !=0)
            {
                dotWidth = m_series.ConfigItems.BoxAndWhiskerItem.OutLierWidth / 100;              
            }

            double dotHeight = dotWidth * axisFactor;

            for (int i = visibleRange.From, end = visibleRange.To + 1; i < end; i++)
            {
                ChartStyledPoint styledPoint = stylePoints[i];

                if (styledPoint.IsVisible)
                {
                    styledPoint.X = Math.Min(Math.Max(styledPoint.X, args.ActualXAxis.Range.Min), args.ActualXAxis.Range.Max);
                    for (int j = 0; j < styledPoint.YValues.Length; j++)
                    {
                        styledPoint.YValues[j] = Math.Min(Math.Max(styledPoint.YValues[j], args.ActualYAxis.Range.Min), args.ActualYAxis.Range.Max);                        
                    }
                    #region  //preparing data of point to retrieve statistical median, quartiles, whiskers etc.
                    double[] dwiA = styledPoint.YValues.Clone() as double[];
                    int statLen = dwiA.Length;

                    Array.Sort(dwiA);

                    ChartPoint q1, q2;
                    double whisker1, whisker2;
                    double interquartileRange;
                    double median = this.GetStatisticalMedian(dwiA);

                    if (statLen % 2 == 0)
                    {
                        int len = statLen / 2;
                        double[] q1Stat = new double[len];
                        double[] q2Stat = new double[len];

                        Array.Copy(dwiA, 0, q1Stat, 0, len);
                        Array.Copy(dwiA, len, q2Stat, 0, len);

                        q1 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q1Stat));
                        q2 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q2Stat));
                    }
                    else
                    {
                        int len = statLen / 2 + 1;
                        double[] q1Stat = new double[len];
                        double[] q2Stat = new double[len];

                        Array.Copy(dwiA, 0, q1Stat, 0, len);
                        Array.Copy(dwiA, len - 1, q2Stat, 0, len);

                        q1 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q1Stat));
                        q2 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q2Stat));
                    }

                    interquartileRange = 1.5f * Math.Abs(q2.YValues[0] - q1.YValues[0]);

                    Dictionary<double, int> distantPointsQ1 = new Dictionary<double, int>();
                    Dictionary<double, int> distantPointsQ2 = new Dictionary<double, int>();

                    if ( m_series.ConfigItems.BoxAndWhiskerItem.PercentileMode )
                    {
                        double lowerInterMediateValue = dwiA[0];              
                        double percentile = m_series.ConfigItems.BoxAndWhiskerItem.Percentile;
                        double baseValue = (statLen - 1) * percentile;

                        int integerPart = (int)baseValue;
                        double decimalPart = 0;

                        if (integerPart != 0)
                            decimalPart = baseValue % integerPart;
                        else
                            decimalPart = baseValue;

                        if (m_series.ConfigItems.BoxAndWhiskerItem.Percentile == 0)
                        {
                            lowerInterMediateValue = dwiA[0];
                        }
                        else
                        {
                            // This is standard formula to calculate the percentile values .  0^th/ 100^th percentiles , 5^th/ 95^th percentiles , 10^th/ 90^th percentiles and so on. 
                            // baseValue, integerPart, decimalPart values takes place in the calculation.
                            lowerInterMediateValue = ((1 - decimalPart) * dwiA[integerPart]) + (decimalPart) * dwiA[integerPart + 1];
                        }

                        whisker1 = lowerInterMediateValue;
                       
                        for (int j = 0; j < statLen / 2; j++)
                        {
                            double dJ = dwiA[j];

                            if (dJ < lowerInterMediateValue)
                            {
                                if (distantPointsQ1.ContainsKey(dJ))
                                {
                                    distantPointsQ1[dJ]++;
                                }
                                else
                                {
                                    distantPointsQ1.Add(dJ, 1);
                                }
                            }
                        }                  

                        double upperInterMediateValue = dwiA[statLen - 1];

                        double upperPercentile = 1 - percentile;
                        double upperBaseValue  = (statLen - 1) * upperPercentile;

                        int upperIntegerPart = (int)upperBaseValue;
                        double upperDecimalPart = upperBaseValue % upperIntegerPart;

                        if (upperIntegerPart != 0)
                            upperDecimalPart = upperBaseValue % upperIntegerPart;
                        else
                            upperDecimalPart = upperBaseValue;

                        if (m_series.ConfigItems.BoxAndWhiskerItem.Percentile == 0)
                        {
                            upperInterMediateValue = dwiA[statLen - 1];
                        }
                        else
                        {
                            // This is standard formula to calculate the percentile values .  0^th/ 100^th percentiles , 5^th/ 95^th percentiles , 10^th/ 90^th percentiles and so on. 
                            // upperBaseValue, upperIntegerPart, upperDecimalPart values takes place in the calculation.
                            upperInterMediateValue = ((1 - upperDecimalPart) * dwiA[upperIntegerPart]) + (upperDecimalPart) * dwiA[upperIntegerPart + 1];
                        }                        

                        whisker2 = upperInterMediateValue;

                        for (int j = statLen - 1; j >= statLen / 2; j--)
                        {
                            double dJ = dwiA[j];

                            if (dJ > upperInterMediateValue)
                            {
                                if (distantPointsQ2.ContainsKey(dJ))
                                {
                                    distantPointsQ2[dJ]++;
                                }
                                else
                                {
                                    distantPointsQ2.Add(dJ, 1);
                                }
                            }
                        }                        
                    }
                    else 
                    { 
                        whisker1 = dwiA[0];

                        for (int j = 0; j < statLen; j++)
                        {
                            whisker1 = dwiA[j];
                            if (Math.Abs(whisker1 - q1.YValues[0]) <= interquartileRange)
                                break;
                        }

                        //getting number of dots at whiskers 
                        for (int j = 0; j < statLen / 2; j++)
                        {
                            double dJ = dwiA[j];

                            if (Math.Abs(dJ - q1.YValues[0]) > interquartileRange)
                            {
                                if (distantPointsQ1.ContainsKey(dJ))
                                {
                                    distantPointsQ1[dJ]++;
                                }
                                else
                                {
                                    distantPointsQ1.Add(dJ, 1);
                                }
                            }
                        }

                        whisker2 = dwiA[statLen - 1];

                        for (int j = statLen - 1; j >= 0; j--)
                        {
                            whisker2 = dwiA[j];
                            if (Math.Abs(whisker2 - q2.YValues[0]) <= interquartileRange)
                                break;
                        }

                        //getting number of dots at whiskers 
                        for (int j = statLen - 1; j >= statLen / 2; j--)
                        {
                            double dJ = dwiA[j];

                            if ((Math.Abs(dJ - q2.YValues[0]) > interquartileRange))
                            {
                                if (distantPointsQ2.ContainsKey(dJ))
                                {
                                    distantPointsQ2[dJ]++;
                                }
                                else
                                {
                                    distantPointsQ2.Add(dJ, 1);
                                }
                            }
                        }
                    }

                    #endregion //END preparing data of point to retrieve statistical median and quartiles

                    ChartStyleInfo style = styledPoint.Style;
                    ChartPoint medianCP = new ChartPoint(styledPoint.X + sbsInfo.Start, median);
                    ChartPoint w1M = new ChartPoint(styledPoint.X + sbsInfo.Median, whisker1);
                    ChartPoint w2M = new ChartPoint(styledPoint.X + sbsInfo.Median, whisker2);

                    // ^- E means End
                    // ^- M means Middle
                    // ^- B means Beginning
                    PointF q1BF = args.GetPoint(q1.X + sbsInfo.Start, q1.YValues[0]);
                    PointF q2BF = args.GetPoint(q2.X + sbsInfo.Start, q2.YValues[0]);

                    PointF q1EF = args.GetPoint(q1.X + sbsInfo.End, q1.YValues[0]);
                    PointF q2EF = args.GetPoint(q2.X + sbsInfo.End, q2.YValues[0]);

                    PointF q1MF = args.GetPoint(q1.X + sbsInfo.Median, q1.YValues[0]);
                    PointF q2MF = args.GetPoint(q2.X + sbsInfo.Median, q2.YValues[0]);

                    PointF w1BF = args.GetPoint(styledPoint.X + sbsInfo.Start, whisker1);
                    PointF w2BF = args.GetPoint(styledPoint.X + sbsInfo.Start, whisker2);

                    PointF w1MF = args.GetPoint(styledPoint.X + sbsInfo.Median, whisker1);
                    PointF w2MF = args.GetPoint(styledPoint.X + sbsInfo.Median, whisker2);

                    PointF w1EF = args.GetPoint(styledPoint.X + sbsInfo.End, whisker1);
                    PointF w2EF = args.GetPoint(styledPoint.X + sbsInfo.End, whisker2);

                    SizeF axisSize = m_series.XAxis.Size;
                    PointF axisLocat = m_series.XAxis.Location;

                    RectangleF rc1 = args.GetRectangle(medianCP.X, medianCP.YValues[0], q1.X + sbsInfo.End, q1.YValues[0]);
                    RectangleF rc2 = args.GetRectangle(medianCP.X, medianCP.YValues[0], q2.X + sbsInfo.End, q2.YValues[0]);

                    this.CheckColumnBounds(args.IsInvertedAxes, ref rc1);
                    this.CheckColumnBounds(args.IsInvertedAxes, ref rc2);

                    if (this.IsFixedWidth)
                    {
                        float w2 = 0.5f * this.Chart.ColumnFixedWidth;

                        if (args.IsInvertedAxes)
                        {
                            q1BF.Y = q2BF.Y = w1BF.Y = w2BF.Y = q1MF.Y - w2;
                            q1EF.Y = q2EF.Y = w1EF.Y = w2EF.Y = q1MF.Y + w2;
                        }
                        else
                        {
                            q1BF.X = q2BF.X = w1BF.X = w2BF.X = q1MF.X - w2;
                            q1EF.X = q2EF.X = w1EF.X = w2EF.X = q1MF.X + w2;
                        }
                    }

                    if (style.DisplayShadow && !args.Is3D)
                    {
                        RectangleF shadowRC1 = rc1;
                        RectangleF shadowRC2 = rc2;

                        shadowRC1.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);
                        shadowRC2.Offset(style.ShadowOffset.Width, style.ShadowOffset.Height);

                        args.Graph.DrawRect(style.ShadowInterior, null, shadowRC1);
                        args.Graph.DrawRect(style.ShadowInterior, null, shadowRC2);
                    }

                    GraphicsPath gp = new GraphicsPath();

                    gp.AddRectangle(rc1);
                    gp.CloseFigure();

                    gp.AddLine(q1MF, w1MF);
                    gp.CloseFigure();

                    gp.AddLine(w1BF, w1EF);
                    gp.CloseFigure();

                    #region Draw Whiskers dots
                    foreach (double wY in distantPointsQ1.Keys)
                    {
                        int cnt = distantPointsQ1[wY];
                        double dotsWdth = cnt * dotWidth;
                        double xd1 = styledPoint.X - 0.5f * dotsWdth;
                        double yd1 = wY - 0.5f * dotHeight;

                        for (int d = 0; d < cnt; d++)
                        {
                            RectangleF rectf = args.GetRectangle(xd1, yd1, xd1 + dotWidth, yd1 + dotHeight);
                            gp.AddEllipse(rectf);
                            xd1 += dotWidth;
                        }
                        yd1 += 1.5f * dotHeight;
                    }
                    #endregion

                    gp.AddRectangle(rc2);
                    gp.CloseFigure();

                    gp.AddLine(q2MF, w2MF);
                    gp.CloseFigure();

                    gp.AddLine(w2BF, w2EF);
                    gp.CloseFigure();

                    #region Draw Whiskers dots
                    foreach (double wY in distantPointsQ2.Keys)
                    {
                        int cnt = distantPointsQ2[wY];
                        double dotsWdth = cnt * dotWidth;
                        double xd1 = styledPoint.X - 0.5f * dotsWdth;
                        double yd1 = wY - 0.5f * dotHeight;


                        for (int d = 0; d < cnt; d++)
                        {
                            RectangleF rectf = args.GetRectangle(xd1, yd1, xd1 + dotWidth, yd1 + dotHeight);
                            gp.AddEllipse(rectf);
                            xd1 += dotWidth;
                        }

                        yd1 += 1.5f * dotHeight;
                    }
                    #endregion

                    if (args.Is3D)
                    {
                        ///////////////////////////// bottom right side 
                        gp.AddPolygon(new PointF[]{ new PointF( rc2.Right, rc2.Top ),
                                       new PointF( rc2.Right + args.DepthOffset.Width, rc2.Top + args.DepthOffset.Height ),
                                       new PointF( rc2.Right + args.DepthOffset.Width, rc2.Bottom + args.DepthOffset.Height ),
                                       new PointF( rc2.Right, rc2.Bottom ) });

                        ////////////////////////////// top right side
                        gp.AddPolygon(new PointF[]{ new PointF( rc2.Left, rc2.Top ),
                                        new PointF( rc2.Left + args.DepthOffset.Width, rc2.Top + args.DepthOffset.Height ),
                                        new PointF( rc2.Right + args.DepthOffset.Width, rc2.Top + args.DepthOffset.Height ),
                                        new PointF( rc2.Right, rc2.Top ) });

                        if (m_series.Rotate)
                        {
                            ////////////////////////////// top rect
                            gp.AddPolygon(new PointF[]{ new PointF( rc1.Left, rc1.Top ),
                                         new PointF( rc1.Left + args.DepthOffset.Width, rc1.Top + args.DepthOffset.Height ),
                                         new PointF( rc1.Right + args.DepthOffset.Width, rc1.Top + args.DepthOffset.Height ),
                                         new PointF( rc1.Right, rc1.Top ) });
                        }
                        else
                        {
                            ///////////////////////////// top right side 
                            gp.AddPolygon(new PointF[]{ new PointF( rc1.Right, rc1.Top ),
                                         new PointF( rc1.Right + args.DepthOffset.Width, rc1.Top + args.DepthOffset.Height ),
                                         new PointF( rc1.Right + args.DepthOffset.Width, rc1.Bottom + args.DepthOffset.Height ),
                                         new PointF( rc1.Right, rc1.Bottom ) });
                        }
                    }

                    ChartSeriesPath path1 = new ChartSeriesPath();
                    path1.AddPrimitive(gp, style.GdipPen, GetBrush(styledPoint.Index));
                    path1.Bounds = rc1;

                    ChartSeriesPath path2 = new ChartSeriesPath();
                    path2.AddPrimitive(gp, style.GdipPen, GetBrush(styledPoint.Index));
                    path2.Bounds = rc2;

                    if (Chart.NeedRegionUpdate)
                    {
                        ChartRegionData crd = new ChartRegionData(args.SeriesIndex, styledPoint.Index, styledPoint.ToolTip, this.RegionDescription);

                        path1.RegionData = crd;
                        path2.RegionData = crd;
                    }

                    if (args.Is3D)
                    {
                        pathsList.Add(path1);
                        pathsList.Add(path2);
                    }
                    else
                    {
                        path1.Draw(args.Graph);
                        path2.Draw(args.Graph);

                        if (path1.RegionData != null)
                        {
                            args.Chart.ChartRegions.Add(path1.GetChartRegion());
                        }

                        if (path2.RegionData != null)
                        {
                            args.Chart.ChartRegions.Add(path2.GetChartRegion());
                        }
                    }
                }
            }

            m_segments = (ChartSeriesPath[])pathsList.ToArray(typeof(ChartSeriesPath));
        }

        /// <summary>
        /// Renders chart by the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Render(ChartRenderArgs3D args)
        {
            double axisFactor = (m_series.ActualXAxis.RealLength * m_series.ActualYAxis.VisibleRange.Delta) / (m_series.ActualXAxis.VisibleRange.Delta * m_series.ActualYAxis.RealLength);

            DoubleRange sbsInfo = this.GetSideBySideInfo();
            int whskrDtFactor = m_series.YAxis.Inversed ? -1 : 1;

            double dotWidth = 0.25 * sbsInfo.Delta;

            if (m_series.ConfigItems.BoxAndWhiskerItem.OutLierWidth != 0)
            {
                dotWidth = m_series.ConfigItems.BoxAndWhiskerItem.OutLierWidth / 100;
            }

            double dotHeight = dotWidth * axisFactor;

            float fd = GetPlaceDepth();
            float dpth = GetSeriesDepth();
            float serDpth = fd;

            args.Graph.AddPolygon(CreateBoundsPolygon(serDpth));

            ChartStyledPoint[] stylePoints = this.PrepearePoints();
            IndexRange visibleRange = this.CalculateVisibleRange();
            ArrayList pathsList = new ArrayList();

            for (int i = visibleRange.From, end = visibleRange.To + 1; i < end; i++)
            {
                ChartStyledPoint styledPoint = stylePoints[i];

                if (styledPoint.IsVisible)
                {
                    #region  
                    
                    //preparing data of point to retrieve statistical median, quartiles, whiskers etc.

                    double[] dwiA = styledPoint.YValues.Clone() as double[];
                    int statLen = dwiA.Length;

                    Array.Sort(dwiA);

                    ChartPoint median, q1, q2, whisker1, whisker2;
                    double interquartileRange;

                    median = new ChartPoint(styledPoint.X, GetStatisticalMedian(dwiA));

                    if (statLen % 2 == 0)
                    {
                        int len = statLen / 2;
                        double[] q1Stat = new double[len];
                        double[] q2Stat = new double[len];

                        Array.Copy(dwiA, 0, q1Stat, 0, len);
                        Array.Copy(dwiA, len, q2Stat, 0, len);

                        q1 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q1Stat));
                        q2 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q2Stat));
                    }
                    else
                    {
                        int len = statLen / 2 + 1;
                        double[] q1Stat = new double[len];
                        double[] q2Stat = new double[len];

                        Array.Copy(dwiA, 0, q1Stat, 0, len);
                        Array.Copy(dwiA, len - 1, q2Stat, 0, len);

                        q1 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q1Stat));
                        q2 = new ChartPoint(styledPoint.X, GetStatisticalMedian(q2Stat));
                    }

                    interquartileRange = Math.Abs(q2.YValues[0] - q1.YValues[0]);

                    Dictionary<double, int> distantPointsQ1 = new Dictionary<double, int>();
                    Dictionary<double, int> distantPointsQ2 = new Dictionary<double, int>();

                    if ( m_series.ConfigItems.BoxAndWhiskerItem.PercentileMode )
                    {                        

                        double lowerInterMediateValue = dwiA[0];              

                        double percentile = m_series.ConfigItems.BoxAndWhiskerItem.Percentile;
                        double baseValue = (statLen - 1) * percentile;

                        int integerPart = (int)baseValue;
                        double decimalPart = 0;

                        if (integerPart != 0)
                            decimalPart = baseValue % integerPart;
                        else
                            decimalPart = baseValue;

                        if (m_series.ConfigItems.BoxAndWhiskerItem.Percentile == 0)
                        {
                            lowerInterMediateValue = dwiA[0];
                        }
                        else
                        {
                            // This is standard formula to calculate the percentile values .  0^th/ 100^th percentiles , 5^th/ 95^th percentiles , 10^th/ 90^th percentiles and so on. 
                            // baseValue, integerPart, decimalPart values takes place in the calculation.
                            lowerInterMediateValue = ((1 - decimalPart) * dwiA[integerPart]) + (decimalPart) * dwiA[integerPart + 1];
                        }                        

                        whisker1 = new ChartPoint(styledPoint.X, lowerInterMediateValue);
                      
                        for (int j = 0; j < statLen / 2; j++)
                        {
                            double dJ = dwiA[j];

                            if (dJ < lowerInterMediateValue)
                            {
                                if (distantPointsQ1.ContainsKey(dJ))
                                {
                                    distantPointsQ1[dJ]++;
                                }
                                else
                                {
                                    distantPointsQ1.Add(dJ, 1);
                                }
                            }
                        }                

                        double upperInterMediateValue = dwiA[statLen - 1];

                        double upperPercentile = 1 - percentile;
                        double upperBaseValue = (statLen - 1) * upperPercentile;

                        int upperIntegerPart = (int)upperBaseValue;
                        double upperDecimalPart = upperBaseValue % upperIntegerPart;

                        if (upperIntegerPart != 0)
                            upperDecimalPart = upperBaseValue % upperIntegerPart;
                        else
                            upperDecimalPart = upperBaseValue;

                        if (m_series.ConfigItems.BoxAndWhiskerItem.Percentile == 0)
                        {
                            upperInterMediateValue = dwiA[statLen - 1];
                        }
                        else
                        {
                            // This is standard formulae to calculate the percentile values .  0^th/ 100^th percentiles , 5^th/ 95^th percentiles , 10^th/ 90^th percentiles and so on. 
                            // upperBaseValue, upperIntegerPart, upperDecimalPart values takes place in the calculation.
                            upperInterMediateValue = ((1 - upperDecimalPart) * dwiA[upperIntegerPart]) + (upperDecimalPart) * dwiA[upperIntegerPart + 1];
                        }

                        whisker2 = new ChartPoint(styledPoint.X, upperInterMediateValue);

                        for (int j = statLen - 1; j >= statLen / 2; j--)
                        {
                            double dJ = dwiA[j];

                            if (dJ > upperInterMediateValue)
                            {
                                if (distantPointsQ2.ContainsKey(dJ))
                                {
                                    distantPointsQ2[dJ]++;
                                }
                                else
                                {
                                    distantPointsQ2.Add(dJ, 1);
                                }
                            }
                        }                        
                    }
                    else 
                    { 

                        whisker1 = new ChartPoint(styledPoint.X, dwiA[0]);
                        for (int j = 0; j < statLen; j++)
                        {
                            double dJ = dwiA[j];
                            whisker1 = new ChartPoint(styledPoint.X, dJ);
                            if (Math.Abs(whisker1.YValues[0] - q1.YValues[0]) <= 3.0f / 2 * interquartileRange)
                                break;
                        }
                        //getting number of dots at whiskers 
                        for (int j = 0; j < statLen / 2; j++)
                        {
                            double dJ = dwiA[j];
                            ChartPoint p1 = new ChartPoint(styledPoint.X, dJ);
                            if ( /*(p1.Y >= q1.Y) &&*/ (Math.Abs(p1.YValues[0] - q1.YValues[0]) > 3.0f / 2 * interquartileRange))
                            {
                                if (distantPointsQ1.ContainsKey(p1.YValues[0]))
                                {
                                    int ii = ((int)distantPointsQ1[p1.YValues[0]]);
                                    ii++;
                                    distantPointsQ1[p1.YValues[0]] = ii;
                                }
                                else
                                {
                                    distantPointsQ1.Add(p1.YValues[0], 1);
                                }
                            } 
                        }
                        whisker2 = new ChartPoint(styledPoint.X, dwiA[statLen - 1]);
                        for (int j = statLen - 1; j >= 0; j--)
                        {
                            double dJ = dwiA[j];
                            whisker2 = new ChartPoint(styledPoint.X, dJ);
                            if (Math.Abs(whisker2.YValues[0] - q2.YValues[0]) <= 3.0f / 2 * interquartileRange)
                                break;
                        }
                        //getting number of dots at whiskers 
                        for (int j = statLen - 1; j >= statLen / 2; j--)
                        {
                            double dJ = dwiA[j];
                            ChartPoint p2 = new ChartPoint(styledPoint.X, dJ);
                            if ((Math.Abs(p2.YValues[0] - q2.YValues[0]) > 3.0f / 2 * interquartileRange))
                            {
                                if (distantPointsQ2.ContainsKey(p2.YValues[0]))
                                {
                                    int ii = ((int)distantPointsQ2[p2.YValues[0]]);
                                    ii++;
                                    distantPointsQ2[p2.YValues[0]] = ii;
                                }
                                else
                                {
                                    distantPointsQ2.Add(p2.YValues[0], 1);
                                }
                            }
                        }
                    }
                    #endregion //END preparing data of point to retrieve statistical median and quartiles

                    ChartStyleInfo style = styledPoint.Style;
                    ChartPoint medianCP = new ChartPoint(styledPoint.X + sbsInfo.Start, median.YValues);
                    ChartPoint mE = new ChartPoint(styledPoint.X + sbsInfo.End, median.YValues);
                    ChartPoint w1M = new ChartPoint(whisker1.X + sbsInfo.Median, whisker1.YValues);
                    ChartPoint w2M = new ChartPoint(whisker2.X + sbsInfo.Median, whisker2.YValues);

                    // ^- E means End
                    // ^- M means Middle
                    // ^- B means Beginning

                    PointF q1BF = args.GetPoint(q1.X + sbsInfo.Start, q1.YValues[0]);
                    PointF q2BF = args.GetPoint(q2.X + sbsInfo.Start, q2.YValues[0]);

                    PointF q1EF = args.GetPoint(q1.X + sbsInfo.End, q1.YValues[0]);
                    PointF q2EF = args.GetPoint(q2.X + sbsInfo.End, q2.YValues[0]);

                    PointF q1MF = args.GetPoint(q1.X + sbsInfo.Median, q1.YValues[0]);
                    PointF q2MF = args.GetPoint(q2.X + sbsInfo.Median, q2.YValues[0]);

                    PointF w1BF = args.GetPoint(whisker1.X + sbsInfo.Start, whisker1.YValues[0]);
                    PointF w2BF = args.GetPoint(whisker2.X + sbsInfo.Start, whisker2.YValues[0]);

                    PointF w1MF = args.GetPoint(whisker1.X + sbsInfo.Median, whisker1.YValues[0]);
                    PointF w2MF = args.GetPoint(whisker2.X + sbsInfo.Median, whisker2.YValues[0]);

                    PointF w1EF = args.GetPoint(whisker1.X + sbsInfo.End, whisker1.YValues[0]);
                    PointF w2EF = args.GetPoint(whisker2.X + sbsInfo.End, whisker2.YValues[0]);

                    if (this.IsFixedWidth)
                    {
                        float w2 = 0.5f * this.Chart.ColumnFixedWidth;

                        if (args.IsInvertedAxes)
                        {
                            q1BF.Y = q2BF.Y = w1BF.Y = w2BF.Y = q1MF.Y - w2;
                            q1EF.Y = q2EF.Y = w1EF.Y = w2EF.Y = q1MF.Y + w2;
                        }
                        else
                        {
                            q1BF.X = q2BF.X = w1BF.X = w2BF.X = q1MF.X - w2;
                            q1EF.X = q2EF.X = w1EF.X = w2EF.X = q1MF.X + w2;
                        }
                    }

                    SizeF axisSize = m_series.XAxis.Size;
                    PointF axisLocat = m_series.XAxis.Location;

                    RectangleF rc1 = args.GetRectangle(medianCP.X, medianCP.YValues[0], q1.X + sbsInfo.End, q1.YValues[0]);
                    RectangleF rc2 = args.GetRectangle(medianCP.X, medianCP.YValues[0], q2.X + sbsInfo.End, q2.YValues[0]);

                    this.CheckColumnBounds(args.IsInvertedAxes, ref rc1);
                    this.CheckColumnBounds(args.IsInvertedAxes, ref rc2);

                    // DRAWING STARTS HERE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    BrushInfo serInterior = GetBrush(styledPoint.Index);
                    Polygon[] plgs1, plgs2;

                    if (!args.IsInvertedAxes)
                    {
                        plgs1 = args.Graph.CreateBoxV(new Vector3D(rc1.Left, rc1.Top, serDpth),
                            new Vector3D(rc1.Right, rc1.Bottom, serDpth + dpth), style.GdipPen, serInterior);
                        plgs2 = args.Graph.CreateBoxV(new Vector3D(rc2.Left, rc2.Top, serDpth),
                            new Vector3D(rc2.Right, rc2.Bottom, serDpth + dpth), style.GdipPen, serInterior);
                    }
                    else
                    {
                        plgs1 = args.Graph.CreateBox(new Vector3D(rc1.Left, rc1.Top, serDpth),
                            new Vector3D(rc1.Right, rc1.Bottom, serDpth + dpth), style.GdipPen, serInterior);
                        plgs2 = args.Graph.CreateBox(new Vector3D(rc2.Left, rc2.Top, serDpth),
                            new Vector3D(rc2.Right, rc2.Bottom, serDpth + dpth), style.GdipPen, serInterior);
                    }

                    //BOTTOM WHISKER
                    Polygon w2plgt = new Polygon(new Vector3D[]{
                                                     new Vector3D( w1BF.X, w1BF.Y, serDpth ),
                                                     new Vector3D( w1EF.X, w1EF.Y, serDpth ),
                                                     new Vector3D( w1EF.X, w1EF.Y, serDpth + dpth ),
                                                     new Vector3D( w1BF.X, w1BF.Y, serDpth + dpth ) },
                        serInterior,
                        style.GdipPen
                        );
                    args.Graph.AddPolygon(w2plgt);
                    Polygon w1plgt = new Polygon(new Vector3D[]{
                                                    new Vector3D( q1MF.X, q1MF.Y, serDpth ),
                                                    new Vector3D( w1MF.X, w1MF.Y, serDpth ),
                                                    new Vector3D( w1MF.X, w1MF.Y, serDpth + dpth ),
                                                    new Vector3D( q1MF.X, q1MF.Y, serDpth + dpth ) },
                        serInterior,
                        style.GdipPen
                    );
                    args.Graph.AddPolygon(w1plgt);

                    double width = sbsInfo.Delta;

                    #region Draw Whiskers dots
                    foreach (double wY in distantPointsQ1.Keys)
                    {
                        int cnt = distantPointsQ1[wY];
                        double dotsWdth = cnt * dotWidth;
                        double xd1 = styledPoint.X - 0.5f * dotsWdth;
                        double yd1 = wY - 0.5f * dotHeight;

                        for (int d = 0; d < cnt; d++)
                        {
                            RectangleF rectf = args.GetRectangle(xd1, yd1, xd1 + dotWidth, yd1 + dotHeight);
                            args.Graph.CreateEllipse(new Vector3D(rectf.X, rectf.Y, serDpth),
                                new SizeF(rectf.Width, rectf.Height), 10, style.GdipPen, serInterior);
                            xd1 += dotWidth;
                        }
                        yd1 += 1.5f * dotHeight;
                    }
                    #endregion

                    //        //TOP WHISKER
                    Polygon w2plgb = new Polygon(new Vector3D[]{
                                             new Vector3D( w2BF.X, w2BF.Y, serDpth ),
                                             new Vector3D( w2EF.X, w2EF.Y, serDpth ),
                                             new Vector3D( w2EF.X, w2EF.Y, serDpth + dpth ),
                                             new Vector3D( w2BF.X, w2BF.Y, serDpth + dpth ) },
                        serInterior,
                        style.GdipPen
                        );
                    args.Graph.AddPolygon(w2plgb);
                    Polygon w1plgb = new Polygon(new Vector3D[]{
                                             new Vector3D( q2MF.X, q2MF.Y, serDpth ),
                                             new Vector3D( w2MF.X, w2MF.Y, serDpth ),
                                             new Vector3D( w2MF.X, w2MF.Y, serDpth + dpth ),
                                             new Vector3D( q2MF.X, q2MF.Y, serDpth + dpth ) },
                        serInterior,
                        style.GdipPen
                        );
                    args.Graph.AddPolygon(w1plgb);

                    #region Draw Whiskers dots
                    foreach (double wY in distantPointsQ2.Keys)
                    {
                        int cnt = distantPointsQ2[wY];
                        double dotsWdth = cnt * dotWidth;
                        double xd1 = styledPoint.X - 0.5f * dotsWdth;
                        double yd1 = wY - 0.5f * dotHeight;


                        for (int d = 0; d < cnt; d++)
                        {
                            RectangleF rectf = args.GetRectangle(xd1, yd1, xd1 + dotWidth, yd1 + dotHeight);
                            args.Graph.CreateEllipse(new Vector3D(rectf.X, rectf.Y, serDpth),
                                new SizeF(rectf.Width, rectf.Height), 10, style.GdipPen, serInterior);
                            xd1 += dotWidth;
                        }

                        yd1 += 1.5f * dotHeight;
                    }
                    #endregion

                    if (Chart.NeedRegionUpdate)
                    {
                        string s = styledPoint.ToolTip;
                        ChartRegionData crd = new ChartRegionData(args.SeriesIndex, styledPoint.Index, s, "Box and Whisker Chart Region");

                        w2plgt.RegionData = crd;
                        w2plgb.RegionData = crd;
                        w1plgt.RegionData = crd;
                        w1plgb.RegionData = crd;

                        for (int j = 0, c = plgs1.Length; j < c; j++)
                        {
                            plgs1[j].RegionData = crd;
                            plgs2[j].RegionData = crd;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Measures the X range.
        /// </summary>
        /// <returns></returns>
        public override DoubleRange GetYDataMeasure()
        {
            if (m_series.Points.Count > 0)
            {
                double max = double.MinValue;
                double min = double.MaxValue;

                for (int i = 0; i < m_series.Points.Count; i++)
                {
                    double[] yValues = m_series.Points[i].YValues;

                    for (int j = 0, cj = yValues.Length; j < cj; j++)
                    {
                        if (yValues[j] > max)
                        {
                            max = yValues[j];
                        }

                        if (yValues[j] < min)
                        {
                            min = yValues[j];
                        }
                    }
                }

                return new DoubleRange(min, max);
            }

            return DoubleRange.Empty;
        }

        /// <summary>
        /// Computes the statistical median.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private double GetStatisticalMedian(double[] values)
        {
            int statLen = values.Length;
            double median;

            if (statLen % 2 == 1)
            {
                median = values[(statLen - 1) / 2];
            }
            else
            {
                median = 0.5 * (values[statLen / 2] + values[statLen / 2 - 1]);
            }

            return median;
        }
        #endregion
    }
}