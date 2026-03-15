// <copyright file="ChartFastLineType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Linq;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Class represents FastLine chart type series' segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartFastLineSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartFastLineSegment), new UIPropertyMetadata(null));

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the points collection.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        #endregion

        #region Members
        /// <summary>
        /// Initializes m_points
        /// </summary>
        internal List<IChartDataPoint> m_points;

        /// <summary>
        /// Initializes m_points
        /// </summary>
        private PointCollection m_vpts1= new PointCollection();

        internal AutoDiscardType isAutoDiscard = AutoDiscardType.None;

        internal bool refresh = false;

        internal double ViewPortwidth=0;

        internal double ViewPortHeight=0;

        internal int dataPtr = 0;
        int lastPoint = 0;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartFastLineSegment"/> class.
        /// </summary>
        /// <remarks>
        /// During initialization the default template for Fast line segment is being created.
        /// </remarks>
        static ChartFastLineSegment()
        {
            Type type = typeof(ChartFastLineSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(
                type,
              new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFastLineSegment"/> class.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The <see cref="ChartSeries"/>.</param>
        internal ChartFastLineSegment(List<IChartDataPoint> points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {

            //this.Series.ActualXAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);
            //this.Series.ActualYAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);


            m_points = points;
            //m_vpts1 = new PointCollection(m_points.Count);

            //for (int i = 0; i < m_points.Count; i++)
            //{
            //    m_vpts1.Add(new Point());
            //}

            isAutoDiscard = series.AutoDiscard;


            //xRange = DoubleRange.Empty;
            //yRange = DoubleRange.Empty;

            double X_MAX = m_points.Max(x => x.X);
            double Y_MAX = m_points.Max(y => y.Y);
            double X_MIN = m_points.Min(x => x.X);
            double Y_MIN = m_points.Min(y => y.Y);
            double Z_MAX = m_points.Max(z => (this.Series.Area.EnableDepthAxis && z.Values.Length > 1) ? z.Values[1] : 0.0);
            double Z_MIN = m_points.Min(z => (this.Series.Area.EnableDepthAxis && z.Values.Length > 1) ? z.Values[1] : 1.0);
            //foreach�(IChartDataPoint�cdpt�in�points)
            //{
            xRange =  new DoubleRange(X_MIN, X_MAX);//xRange + cdpt.X;
            yRange =  new DoubleRange(Y_MIN, Y_MAX);//yRange + cdpt.Y;
            zRange = new DoubleRange(Z_MIN, Z_MAX);
           // }




            //if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            //{
            //    xRange += xRange.Start - 0.5;
            //    xRange += xRange.End + 0.5;
            //}

            SetRange(series);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFastLineSegment"/> class.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The <see cref="ChartSeries"/>.</param>
        /// <param name="dataptr">The <see />.</param>
        internal ChartFastLineSegment(List<IChartDataPoint> points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series, int dataptr)
            : base(series, correspondingPoints)
        {
            m_points = points;
            isAutoDiscard = series.AutoDiscard;
            dataPtr = dataptr;
            double X_MAX = m_points.Max(x => x.X);
            double Y_MAX = m_points.Max(y => y.Y);
            double X_MIN = m_points.Min(x => x.X);
            double Y_MIN = m_points.Min(y => y.Y);
            double Z_MAX = m_points.Max(z => (this.Series.Area.EnableDepthAxis && z.Values.Length > 1) ? z.Values[1] : 0.0);
            double Z_MIN = m_points.Min(z => (this.Series.Area.EnableDepthAxis && z.Values.Length > 1) ? z.Values[1] : 1.0);
            xRange = new DoubleRange(X_MIN, X_MAX);//xRange + cdpt.X;
            yRange = new DoubleRange(Y_MIN, Y_MAX);//yRange + cdpt.Y;
            zRange = new DoubleRange(Z_MIN, Z_MAX);
            SetRange(series);
        }

        void ActualXAxis_RangeChanged(object sender, ChartAxisRangeArgs e)
        {
            if (Series != null && Series.Area != null && Series.ChartType.indexedPointsList != null)
            {
                this.SetPointsForAllSeries(Series, Series.Area.PrimaryAxis);
                Series.ChartType.indexedPointsList.Clear();
                Series.ChartType.indexedPointsList = null;
            }
        }


        #endregion

        #region Implmentation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CheckForNewRange(int count)
        {
            if (m_points.Count >= count)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="series"></param>
        public void GetSegmet(IChartDataPoint point, ChartSeries series)
        {
            m_points.Add(point);

            m_vpts1.Add(new Point());

            xRange += point.X;
            yRange += point.Y;

            isAutoDiscard = series.AutoDiscard;

            //if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            //{
            //    xRange += xRange.Start - 0.5;
            //    xRange += xRange.End + 0.5;
            //}


            SetRange(series);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="axis"></param>
        protected override void SetPointsForAllSeries(ChartSeries series, ChartAxis axis)
        {
            foreach (ChartSeries item in series.Area.Series)
            {
                if ((item.ActualXAxis == axis || item.ActualYAxis == axis) && item.Segments.Count != 0)
                {
                    if (item.Segments[0].GetType() == typeof(ChartFastLineSegment))
                    {
                        ((ChartFastLineSegment)item.Segments[0]).SetPointToNull();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SetPointToNull()
        {
            this.Points = null;
        }

      
        //private void SetRange(ChartSeries series)
        //{

        //    if (series.IsAutoDiscard == AutoDiscardType.None)
        //        return;

        //    switch (series.ActualXAxis.ValueType)
        //    {

        //        case ChartValueType.Double:
        //            {
        //                if (series.ActualXAxis.IsAutoSetRange == true)
        //                {
        //                    return;
        //                }

        //                refresh = false;

        //                if (xRange.End >= series.ActualXAxis.VisibleRange.End)
        //                {
        //                    if (series.IsAutoDiscard == AutoDiscardType.ResetRange)
        //                    {
        //                        double range = xRange.End / 5;
        //                        DoubleRange newRange = new DoubleRange(Math.Round(range * 3), Math.Round(range * 3 + range * 4));
        //                        SetPointsForAllSeries(series, series.Area.PrimaryAxis);
        //                        series.ActualXAxis.Range = newRange;
        //                    }
        //                    else if (series.IsAutoDiscard == AutoDiscardType.ExpandRange)
        //                    {
        //                        DoubleRange newRange = new DoubleRange(series.ActualXAxis.Range.Start, 2 * series.ActualXAxis.Range.End - series.ActualXAxis.Range.Start);
        //                        SetPointsForAllSeries(series, series.Area.PrimaryAxis);
        //                        series.ActualXAxis.Range = newRange;
        //                    }
        //                    refresh = true;
        //                }

        //                if (series.ActualYAxis.IsAutoSetRange == true)
        //                {
        //                    return;
        //                }
        //                //if (yRange.End >= series.ActualYAxis.VisibleRange.End)
        //                //{
        //                //    if (series.IsAutoDiscard == true)
        //                //    {
        //                //        double range = yRange.End / 5;
        //                //        DoubleRange newRange = new DoubleRange(Math.Round(range * 3), Math.Round(range * 3 + range * 4));
        //                //        SetPointsForAllSeries(series, series.Area.SecondaryAxis);
        //                //        series.ActualYAxis.Range = newRange;
        //                //    }
        //                //    else
        //                //    {
        //                //        DoubleRange newRange = new DoubleRange(series.ActualYAxis.Range.Start, series.ActualYAxis.Range.End + series.ActualYAxis.Range.End - series.ActualYAxis.Range.Start);
        //                //        SetPointsForAllSeries(series, series.Area.SecondaryAxis);
        //                //        series.ActualYAxis.Range = newRange;
        //                //    }
        //                //    refresh = true;
        //                //}
        //            }
        //            break;
        //        case ChartValueType.DateTime:
        //            {
        //                if (series.ActualXAxis.IsAutoSetRange == true)
        //                {
        //                    return;
        //                }
        //                refresh = false;

        //                if (xRange.End >= series.ActualXAxis.VisibleRange.End)
        //                {

        //                    if (series.IsAutoDiscard == AutoDiscardType.ResetRange)
        //                    {
        //                        TimeSpan range = series.ActualXAxis.DateTimeRange.End - series.ActualXAxis.DateTimeRange.Start;
        //                        int sec = (int)((range.Seconds + range.Minutes * 60 + range.Hours * 3600) / 60);

        //                        range = new TimeSpan(0, sec - 2 * sec / 5, 0);

        //                        DateTime startTime = series.ActualXAxis.DateTimeRange.Start + range;
        //                        range = new TimeSpan(0, sec - sec / 5, 0);

        //                        DateTime endTime = startTime + range;
        //                        DateTimeRange newDTrange = new DateTimeRange(startTime, endTime);
        //                        SetPointsForAllSeries(series, series.Area.PrimaryAxis);
        //                        series.ActualXAxis.DateTimeRange = newDTrange;

        //                    }
        //                    else if (series.IsAutoDiscard == AutoDiscardType.ExpandRange)
        //                    {
        //                        TimeSpan range = series.ActualXAxis.DateTimeRange.End - series.ActualXAxis.DateTimeRange.Start;
        //                        DateTime endTime = series.ActualXAxis.DateTimeRange.End + range;
        //                        DateTimeRange newDTrange = new DateTimeRange(series.ActualXAxis.DateTimeRange.Start, endTime);
        //                        SetPointsForAllSeries(series, series.Area.PrimaryAxis);
        //                        series.ActualXAxis.DateTimeRange = newDTrange;
        //                    }
        //                    refresh = true;
        //                }
        //                if (series.ActualYAxis.IsAutoSetRange == true)
        //                {
        //                    return;
        //                }
        //                //if (yRange.End >= series.ActualYAxis.VisibleRange.End)
        //                //{
        //                //    if (series.IsAutoDiscard == true)
        //                //    {
        //                //        TimeSpan range = series.ActualYAxis.DateTimeRange.End - series.ActualYAxis.DateTimeRange.Start;
        //                //        int sec = (int)((range.Seconds + range.Minutes * 60 + range.Hours * 3600) / 60);

        //                //        range = new TimeSpan(0, sec - 2 * sec / 5, 0);

        //                //        DateTime startTime = series.ActualXAxis.DateTimeRange.Start + range;
        //                //        range = new TimeSpan(0, sec - sec / 5, 0);

        //                //        DateTime endTime = startTime + range;
        //                //        DateTimeRange newDTrange = new DateTimeRange(startTime, endTime);
        //                //        SetPointsForAllSeries(series, series.Area.SecondaryAxis);
        //                //        series.ActualYAxis.DateTimeRange = newDTrange;

        //                //    }
        //                //    else
        //                //    {
        //                //        TimeSpan range = series.ActualYAxis.DateTimeRange.End - series.ActualYAxis.DateTimeRange.Start;
        //                //        DateTime endTime = series.ActualYAxis.DateTimeRange.End + range;
        //                //        DateTimeRange newDTrange = new DateTimeRange(series.ActualYAxis.DateTimeRange.Start, endTime);
        //                //        SetPointsForAllSeries(series, series.Area.SecondaryAxis);
        //                //        series.ActualYAxis.DateTimeRange = newDTrange;

        //                //    }
        //                //    refresh = true;
        //                //}
        //            }
        //            break;
        //        case ChartValueType.String:
        //            break;
        //        default:
        //            break;
        //    }


        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="series"></param>
        ///  <seealso cref="ChartFastHiLoOpenCloseSegment"/>
        public void UpdateSegment(List<IChartDataPoint> points, ChartSeries series)
        {
            m_points = points;
            m_vpts1 = new PointCollection(m_points.Count);

            //for (int i = 0; i < m_points.Count; i++)
            //{
            //    m_vpts1.Add(new Point());
            //}

            isAutoDiscard = series.AutoDiscard;

            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            foreach (IChartDataPoint cdpt in points)
            {
                xRange += cdpt.X;
                yRange += cdpt.Y;
            }

            //if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            //{
            //    xRange += xRange.Start - 0.5;
            //    xRange += xRange.End + 0.5;
            //}

        }
        internal IChartTransformer temptrans;
        internal int tempcount;
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        ///  <seealso cref="ChartFastLineSegment"/>
        public override void Update(IChartTransformer transformer)
        {
            
            if(!this.Series.Presenter.Issizechanged && m_vpts1.Count >0 && this.Points.Count>0 && this.Series.XAxis.EnableAutoScrolling != true)
            {

                if (this.Points != null )
                {
                   // m_vpts1.Clear();
                    var v_portWidth = transformer.Viewport.Width;
                    var v_portHeight = transformer.Viewport.Height;
                    var xAxis = Series.XAxis;
                    var yAxis = Series.YAxis;
                    // PointCollection new_pt = new PointCollection();
                    Point pt = new Point();
                    Point lastp = new Point();
                    bool opt = Series.UseOptimization;
                    double resolution = Series.Resolution;
                    var points = m_points;
                    bool opposed = Series.IsRotated;
                    bool x_inverse = Series.XAxis.IsInversed;
                    bool y_inverse = Series.YAxis.IsInversed;
                    bool ptinit = false;
                    //m_vpts1.Add(transformer.TransformToVisible(m_points[i].X, m_points[i].Y));
                    bool Iszoomactivated = xAxis.Area.SecondaryAxis.ZoomFactor < 1;
                    if (xAxis.IsLogarithmic || yAxis.IsLogarithmic)
                    {
                        

                            //double x = points[i].X, y =points[i].Y;
                            if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false)//&& (Iszoomactivated ? ((xAxis.VisibleRange.Inside(points[i].X - xAxis.VisibleInterval) || xAxis.VisibleRange.Inside(points[i].X + xAxis.VisibleInterval))) : true))
                            {
                                pt = transformer.TransformToVisible(points[points.Count - 1].X, points[points.Count - 1].Y);
                                ptinit = true;
                                //pt = opposed ?
                                //  new Point(v_portWidth * yAxis.ValueToCoefficient1(points[i].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[i].X, x_inverse))) :
                                //  new Point(v_portWidth * xAxis.ValueToCoefficient1(points[i].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[i].Y, y_inverse)));
                            }



                            if (points.Count - 1 > 0)
                            {
                                lastp = m_vpts1[m_vpts1.Count - 1];
                                if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                                {
                                    // this.model.ChartPoints.Add(point);
                                    m_vpts1.Add(pt);
                                    lastp = pt;
                                }

                            }
                            else
                            {
                                if(ptinit)
                                m_vpts1.Add(pt);
                            }

                        
                    }
                    else
                    {

                        
                            //double x = points[i].X, y =points[i].Y;
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false )//&& (Iszoomactivated ? ((xAxis.VisibleRange.Inside(points[points.Count - 1].X - xAxis.VisibleInterval) || xAxis.VisibleRange.Inside(points[points.Count - 1].X + xAxis.VisibleInterval))) : true))
                        {
                                //x = x > 0 && xAxis.IsLogarithmic ? Math.Log(xAxis.LogarithmicBase) : x;
                                //y = y > 0 && yAxis.IsLogarithmic ? Math.Log(y,yAxis.LogarithmicBase) : y;
                                ptinit = true;
                                pt = opposed ?
                                  new Point(v_portWidth * yAxis.ValueToCoefficient1(points[points.Count - 1].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[points.Count - 1].X, x_inverse))) :
                                  new Point(v_portWidth * xAxis.ValueToCoefficient1(points[points.Count - 1].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[points.Count - 1].Y, y_inverse)));
                            }
                            //m_vpts1[i] = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
                            //pt = transformer.TransformToVisible(points[i].X, points[i].Y);

                                if (points.Count - 1 > 0)
                                {
                                    lastp = m_vpts1[m_vpts1.Count - 1];
                                    if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                                    {

                                        // this.model.ChartPoints.Add(point);
                                        if (ptinit)
                                            m_vpts1.Add(pt);
                                        lastp = pt;
                                    }

                                }
                                else
                                {
                                    if (ptinit)
                                        m_vpts1.Add(pt);
                                }
                        
                    }
                   // this.Points = m_vpts1;
                    lastPoint = this.Points.Count;
                    refresh = (refresh = true ? false : true);
                }
                else
                {
                    if (m_points.Count - lastPoint == 1 && m_vpts1[m_vpts1.Count - 1].X == 0 && m_vpts1[m_vpts1.Count - 1].Y == 0)
                    {
                        //System.Diagnostics.Debug.WriteLine("X value:" + m_points[m_points.Count - 1].X + "Y Value:" + m_points[m_points.Count - 1].Y);
                        m_vpts1[m_vpts1.Count - 1] = transformer.TransformToVisible(m_points[m_points.Count - 1].X, m_points[m_points.Count - 1].Y);
                        this.Points[this.Points.Count - 1] = m_vpts1[m_vpts1.Count - 1];
                        refresh = true;
                    }
                    else if (m_points.Count != lastPoint && (Series.UseOptimization == false))
                    {
                        for (int i = lastPoint; i < m_vpts1.Count; i++)
                        {
                            m_vpts1[i] = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
                        }
                        this.Points = m_vpts1;
                        refresh = true;
                    }

                    lastPoint = m_points.Count;
                }
            }
            else
            {
                temptrans = transformer;
                tempcount = m_points.Count;
            if (this.Interior != null && this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }

            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }

            //Set the points to null  when Zomming is enabled to recalculate the series points.
            //if (this.Series.Type == ChartTypes.FastLine)
            //{
            //    if (this.Series.Area != null)
            //    {
            //        if (this.Series.Area.ZoomSwitched == true)
            //        {
            //            this.Points = null;
            //        }
            //        if (this.Series.Area.PrimaryAxis != null)
            //        {
            //            if (this.Series.Area.PrimaryAxis.ZoomFactor != 1)
            //            {
            //                this.Points = null;
            //            }
            //            else if (this.Series.Area.PrimaryAxis.isNeedUpdate == true)
            //            {
            //                this.Points = null;
            //                this.Series.Area.PrimaryAxis.isNeedUpdate = false;
            //            }

            //        }
            //        if (this.Series.Area.SecondaryAxis != null)
            //        {
            //            if (this.Series.Area.SecondaryAxis.ZoomFactor != 1)
            //            {
            //                this.Points = null;
            //            }
            //            else if (this.Series.Area.SecondaryAxis.isNeedUpdate == true)
            //            {
            //                this.Points = null;
            //                this.Series.Area.PrimaryAxis.isNeedUpdate = false;
            //            }
            //        }

            //    }
            //}
            ////List<IChartDataPoint> chartPt = (from pt in m_points where this.Series.XAxis.VisibleRange.Inside(pt.X) select pt).ToList<IChartDataPoint>();
            //if (this.Points != null && m_points.Count < this.Points.Count)
            //{
            //    this.Points = null;
            //}

            //if (this.Points == null || viewPortwidth != transformer.Viewport.Width || viewPortHeight != transformer.Viewport.Height)
            //{
                m_vpts1.Clear();
                var v_portWidth = transformer.Viewport.Width;
                var v_portHeight = transformer.Viewport.Height;
                var xAxis = Series.XAxis;
                var yAxis = Series.YAxis;                
                Point pt = new Point();
                Point prept = new Point();
                Point postpt = new Point();
                Point lastp = new Point();
                bool opt = Series.UseOptimization;
                double resolution = Series.Resolution;
                var points = m_points;
                bool opposed = Series.IsRotated;
                bool x_inverse = Series.XAxis.IsInversed;
                bool y_inverse = Series.YAxis.IsInversed;
                int preIndex = 0, postIndex = 0, j = 0;
                DoubleRange dr= Series.XAxis.VisibleRange;
                int delta= (int)Series.XAxis.AutoScrollingDelta;
                var temppoint = (from pts in points where dr.Inside(pts.X) select pts).ToList();
                if (Series.XAxis.EnableAutoScrolling == true && temppoint.Count > delta)
                {
                    points = temppoint;// points.GetRange(points.Count - delta, delta);
                }
                bool ptini = false;
                bool Iszoomactivated = xAxis.Area.SecondaryAxis.ZoomFactor < 1 || xAxis.Area.PrimaryAxis.EnableAutoScrolling == true;
                if (xAxis.IsLogarithmic || yAxis.IsLogarithmic)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false)//&& (Iszoomactivated ? ((xAxis.VisibleRange.Inside(points[i].X - xAxis.VisibleInterval) || xAxis.VisibleRange.Inside(points[i].X + xAxis.VisibleInterval))) : true))
                        {
                            pt = transformer.TransformToVisible(points[i].X, points[i].Y);
                            
                            //pt = opposed ?
                            //  new Point(v_portWidth * yAxis.ValueToCoefficient1(points[i].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[i].X, x_inverse))) :
                            //  new Point(v_portWidth * xAxis.ValueToCoefficient1(points[i].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[i].Y, y_inverse)));
                        }



                        if (i > 0)
                        {
                            lastp = m_vpts1[m_vpts1.Count - 1];
                            if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                            {                              
                                
                                    m_vpts1.Add(pt);
                                    lastp = pt;
                                
                            }

                        }
                        else
                        {
                            
                            m_vpts1.Add(pt);
                        }

                    }
                }
                else
                {

                    for (int i = 0; i < points.Count; i++)
                    {
                        if (xAxis != null && yAxis != null && double.IsNaN(v_portWidth) == false && double.IsNaN(v_portHeight) == false &&(Iszoomactivated ? (xAxis.VisibleRange.Inside(points[i].X)) : true))// xAxis.VisibleRange.Inside(points[i].X))//(Iszoomactivated ? (xAxis.VisibleRange.Inside(points[i].X)) : true))
                        {
                            j++;
                            pt = opposed ?
                              new Point(v_portWidth * yAxis.ValueToCoefficient1(points[i].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[i].X, x_inverse))) :
                              new Point(v_portWidth * xAxis.ValueToCoefficient1(points[i].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[i].Y, y_inverse)));
                            if (j == 1)
                            {
                                preIndex = i;
                            }
                            postIndex = i;
                            ptini = true;
                        }

                        if (m_vpts1.Count > 0)
                        {

                            lastp = m_vpts1[m_vpts1.Count - 1];
                            if (opt == false || ((Math.Abs(pt.X - lastp.X) > resolution) || (Math.Abs(pt.Y - lastp.Y) > resolution)))
                            {
                                if (lastp != pt && ptini)
                                {
                                    m_vpts1.Add(pt);
                                    lastp = pt;
                                }
                            }
                            
                        }
                        else
                        {
                           
                                m_vpts1.Add(pt);
                                lastp = pt;
                           
                            
                        }

                    }
                    // add prept and postpt

                    if (preIndex > 0)
                    {
                        prept = opposed ?
                         new Point(v_portWidth * yAxis.ValueToCoefficient1(points[preIndex - 1].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[preIndex - 1].X, x_inverse))) :
                         new Point(v_portWidth * xAxis.ValueToCoefficient1(points[preIndex - 1].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[preIndex - 1].Y, y_inverse)));
                        m_vpts1[0] = prept;
                    }
                    if (postIndex + 1 != points.Count && m_vpts1.Count != 0)
                    {
                        if ((Iszoomactivated ? (xAxis.VisibleRange.Inside(points[postIndex].X)) : true))
                        {
                            postpt = opposed ?
                             new Point(v_portWidth * yAxis.ValueToCoefficient1(points[postIndex + 1].Y, y_inverse), v_portHeight * (1 - xAxis.ValueToCoefficient1(points[postIndex + 1].X, x_inverse))) :
                             new Point(v_portWidth * xAxis.ValueToCoefficient1(points[postIndex + 1].X, x_inverse), v_portHeight * (1 - yAxis.ValueToCoefficient1(points[postIndex + 1].Y, y_inverse)));
                            m_vpts1.Add(postpt);

                        }
                    }

                }
               
                this.Points = m_vpts1;
                lastPoint = this.Points.Count;
                refresh = (refresh = true ? false : true);
            //}
            //else
            //{
            //    if (m_points.Count - lastPoint == 1 && m_vpts1[m_vpts1.Count - 1].X == 0 && m_vpts1[m_vpts1.Count - 1].Y == 0)
            //    {
            //        //System.Diagnostics.Debug.WriteLine("X value:" + m_points[m_points.Count - 1].X + "Y Value:" + m_points[m_points.Count - 1].Y);
            //        m_vpts1[m_vpts1.Count - 1] = transformer.TransformToVisible(m_points[m_points.Count - 1].X, m_points[m_points.Count - 1].Y);
            //        this.Points[this.Points.Count - 1] = m_vpts1[m_vpts1.Count - 1];
            //        refresh = true;
            //    }
            //    else if (m_points.Count != lastPoint && (Series.UseOptimization == false))
            //    {
            //        for (int i = lastPoint; i < m_vpts1.Count; i++)
            //        {
            //            m_vpts1[i] = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
            //        }
            //        this.Points = m_vpts1;
            //        refresh = true;
            //    }

            //    lastPoint = m_points.Count;
            //}
            //viewPortwidth = transformer.Viewport.Width;
            //viewPortHeight = transformer.Viewport.Height;
            }
        }

        /// <summary
        /// >
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The Transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            DoubleRange axisRange = Series.XAxis.VisibleRange;
            DoubleRange axisYRange = Series.YAxis.VisibleRange;

           if ((xRange.End) <= axisRange.End)
            {
                Vector3D resultP1;
                Vector3D resultP2;
                Vector3D resultP3;
                Vector3D resultP4;
                if (m_points.Count > 1)
                {
                    Point3D pointStart1 = new Point3D();
                    Point3D pointStart2 = new Point3D();
                    if (m_points.Count <= 2)
                    {
                        pointStart1 = transformer.TransformToVisible(m_points[0].X, m_points[0].Y, 0.05);
                        pointStart2 = transformer.TransformToVisible(m_points[1].X, m_points[1].Y, 0.05);
                    }

                    double width = 0.01d;
                    Vector3D startVector1 = new Vector3D(pointStart1.X, pointStart1.Y, pointStart1.Z);
                    Vector3D startVector2 = new Vector3D(pointStart2.X, pointStart2.Y, pointStart2.Z);

                    Vector3D norm = startVector2 - startVector1;
                    norm.Normalize();
                    norm = new Vector3D(-norm.Y, norm.X, norm.Z);

                    resultP1 = startVector1 + width * norm;
                    resultP2 = startVector1 - width * norm;

                    for (int i = 1; i < m_points.Count; i++)
                    {
                        if (m_points[i].X + Series.XAxis.VisibleInterval > axisRange.Start && (m_points[i].Y + Series.YAxis.VisibleInterval>axisYRange.Start))
                        {
                            Point3D pointPrev = new Point3D();
                            Point3D pointNext = new Point3D();
                            Point3D pointCurrent = transformer.TransformToVisible(m_points[i].X, m_points[i].Y, 0.05);

                            if ((i + 1) < m_points.Count)
                            {
                                pointNext = transformer.TransformToVisible(m_points[i + 1].X, m_points[i + 1].Y, 0.05);
                            }

                            if ((i - 1) >= 0)
                            {
                                pointPrev = transformer.TransformToVisible(m_points[i - 1].X, m_points[i - 1].Y, 0.05);
                            }
                            else
                            {
                                this.Update(transformer);
                            }

                            Vector3D currentVector = new Vector3D(pointCurrent.X, pointCurrent.Y, pointCurrent.Z);
                            Vector3D nextVector = new Vector3D(pointNext.X, pointNext.Y, pointNext.Z);
                            Vector3D prevVector = new Vector3D(pointPrev.X, pointPrev.Y, pointPrev.Z);

                            norm = currentVector - prevVector;
                            norm.Normalize();
                            norm = new Vector3D(-norm.Y, norm.X, norm.Z);

                            Vector3D ptt1 = prevVector + width * norm;
                            Vector3D ptb1 = prevVector - width * norm;

                            Vector3D ptt2 = currentVector + width * norm;
                            Vector3D ptb2 = currentVector - width * norm;

                            if (pointNext != null)
                            {
                                Vector3D ptonenext = new Vector3D(pointCurrent.X, pointCurrent.Y, pointCurrent.Z);
                                Vector3D pttwonext = new Vector3D(pointNext.X, pointNext.Y, pointCurrent.Z);

                                norm = pttwonext - ptonenext;
                                norm.Normalize();
                                norm = new Vector3D(-norm.Y, norm.X, norm.Z);

                                Vector3D ptt1Next = ptonenext + width * norm;
                                Vector3D ptb1Next = ptonenext - width * norm;

                                Vector3D ptt2Next = pttwonext + width * norm;
                                Vector3D ptb2Next = pttwonext - width * norm;

                                resultP3 = GetCrossPoint(ptt1, ptt2, ptt1Next, ptt2Next);
                                resultP4 = GetCrossPoint(ptb1, ptb2, ptb1Next, ptb2Next);
                            }
                            else
                            {
                                resultP3 = ptt2;
                                resultP4 = ptb2;
                            }

                            GeometryModel3D model = new GeometryModel3D();

                            model.Geometry = MeshGenerator.FastLineBar(resultP2, resultP1, resultP3, resultP4);

                            MaterialGroup materialGroup;

                            DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                            Binding binding = new Binding("Interior");
                            binding.Source = Series;
                            BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                            materialGroup = new MaterialGroup();
                            materialGroup.Children.Add(difuseMaterial);

                            model.Material = materialGroup;
                            model.Transform = new TranslateTransform3D(-0.5, -0.5, 0.1);

                            Geometry3DGroup.Children.Add(model);

                            resultP1 = resultP3;
                            resultP2 = resultP4;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12. value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The cross point</returns>
        private static Vector3D GetCrossPoint(Vector3D p11, Vector3D p12, Vector3D p21, Vector3D p22)
        {
            Vector3D pt = new Vector3D();
            double z = (p12.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p12.X - p11.X);
            double ca = (p12.Y - p11.Y) * (p21.X - p11.X) - (p21.Y - p11.Y) * (p12.X - p11.X);
            double cb = (p21.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p21.X - p11.X);

            double ua = ca / z;
            double ub = cb / z;

            pt.X = p11.X + (p12.X - p11.X) * ub;
            pt.Y = p11.Y + (p12.Y - p11.Y) * ub;
            pt.Z = p11.Z + (p12.Z - p11.Z) * ub;

            if (pt.Y > 1)
            {
                pt.Y = 1;
            }
            if (pt.Y < 0)
            {
                pt.Y = 0;
            }
            return pt;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {

            this.seriesCorrespondingPoints = null;
            if (this.Points != null)
            {
                this.Points.Clear();
                this.Points = null;
            }

            if (this.m_points != null)
            {
                foreach (ChartPoint item in this.m_points)
                {
                    item.Dispose();
                }
                this.m_points.Clear();
                this.m_points = null;
            }

            if (this.m_vpts1 != null)
            {
                this.m_vpts1.Clear();
                this.m_vpts1 = null;
            }
            this.Item = null;
            base.Dispose();

        }

        #endregion

    }

    /// <summary>
    /// Represents ChartFastLineType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartFastLineType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        List<IChartDataPoint> linePoints;
        private int dataPtr = 0;

        #region Implementation
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartFastLineType"/>
        public override string ToString()
        {
            return "FastLine";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {

            List<List<IChartDataPoint>> ptCollection = new List<List<IChartDataPoint>>();
            List<List<ChartIndexedDataPoint>> tempPointArray = new List<List<ChartIndexedDataPoint>>();
            if (series.ShowEmptyPoints == true && series.EmptyPointValue == EmptyPointValue.Average)
            {
                 linePoints = new List<IChartDataPoint>();
                series.Segments.Clear();
                series.Adornments.Clear();
                ChartIndexedDataPoint[] pts = points;
                List<IChartDataPoint> linepts = new List<IChartDataPoint>();
                List<ChartIndexedDataPoint> tempPointArray1 = new List<ChartIndexedDataPoint>();
                for (int i = 0; i < pts.Length; i++)
                {
                    if (i < points.Length)
                        linePoints.Add(points[i].DataPoint);
                    tempPointArray1.Add(points[i]);
                }
                if (tempPointArray1.Count != 0 && linePoints.Count != 0)
                {
                    series.Segments.Add(new ChartFastLineSegment(linePoints, tempPointArray1.ToArray(), series));
                }
            }
            else if (series.ShowEmptyPoints == false && series.Area.EnableLazyLoading == true)
            {
                linePoints = new List<IChartDataPoint>();
                if (series.ActualYAxis.IsAutoSetRange == true || series.ActualXAxis.IsAutoSetRange == true)
                {
                    series.Segments.Clear();
                    series.Adornments.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                        linePoints.Add(points[i].DataPoint);
                    }
                    series.Segments.Add(new ChartFastLineSegment(linePoints, points, series));

                    return;
                }

                if (series.Segments.Count != 0)
                {
                    ChartFastLineSegment segment = ((ChartFastLineSegment)series.Segments[0]);

                    if (segment.Points != null && segment.Points.Count < points.Length)
                    {
                        segment.GetSegmet(points[points.Length - 1].DataPoint, series);
                    }
                    else if (segment.Points == null || segment.Points.Count > points.Length)
                    {
                        for (int i = 0; i < points.Length; i++)
                        {
                            linePoints.Add(points[i].DataPoint);
                        }
                        segment.UpdateSegment(linePoints, series);
                        segment.refresh = true;
                    }
                }
                else
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        linePoints.Add(points[i].DataPoint);
                    }
                    series.Segments.Add(new ChartFastLineSegment(linePoints, points, series));
                }
            }
            else if (series.Segments.Count == 0 || series.internaldata_modified || linePoints.Count > points.Length || series.Area is TimeLineControl)
            {
                linePoints = new List<IChartDataPoint>();
                series.Segments.Clear();
                series.Adornments.Clear();
                ChartIndexedDataPoint[] pts = points;
                List<IChartDataPoint> linepts = new List<IChartDataPoint>();
                List<ChartIndexedDataPoint> tempPoint = new List<ChartIndexedDataPoint>();
                for (int i = 0; i < points.Length; i++)
                {
                    if (!points[i].DataPoint.EmptyPoint || series.Area is TimeLineControl)
                    {
                        linepts.Add(points[i].DataPoint);
                        tempPoint.Add(points[i]);
                    }
                    else
                    {
                        if (linepts.Count > 0)
                        {
                            ptCollection.Add(linepts);
                            tempPointArray.Add(tempPoint);
                            linepts = new List<IChartDataPoint>();
                            tempPoint = new List<ChartIndexedDataPoint>();
                        }
                    }
                    linePoints.Add(points[i].DataPoint);
                }
                ptCollection.Add(linepts);
                tempPointArray.Add(tempPoint);
                
                for (int i = 0; i < ptCollection.Count && i < tempPointArray.Count; i++)
                {
                    if (ptCollection[i].Count > 0 && tempPointArray[i].Count > 0)
                    {
                        series.Segments.Add(new ChartFastLineSegment(ptCollection[i], tempPointArray[i].ToArray(), series, dataPtr));
                        dataPtr += tempPointArray[i].Count + 1;
                    }
                }
            }
            if (series.Segments.Count > 0)
            {
                for (int i = 0; i < series.Segments.Count; i++)
                {
                    if (series.Contains_emptypt)
                    {
                    }
                    else
                    {
                        int cnt = linePoints.Count;
                        while (linePoints.Count != points.Length && linePoints.Count < points.Length)
                        {
                            linePoints.Add(points[cnt].DataPoint);
                            cnt++;
                        }
                        (series.Segments[i] as ChartFastLineSegment).m_points = linePoints;
                    }
                   // (series.Segments[i] as ChartFastLineSegment).m_points = linePoints;
                    if (series.ActualXAxis.IsAutoSetRange || series.Zoomactionenabled)
                    {
                        (series.Segments[i] as ChartFastLineSegment).isAutoDiscard = series.AutoDiscard;

                        double X_MAX = linePoints.Max(x => x.X);
                        double X_MIN = linePoints.Min(x => x.X);
                        (series.Segments[i] as ChartFastLineSegment).xRange = new DoubleRange(X_MIN, X_MAX);//xRange + cdpt.X;
                        if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
                        {
                            (series.Segments[i] as ChartFastLineSegment).xRange += (series.Segments[i] as ChartFastLineSegment).xRange.Start - 0.5;
                            (series.Segments[i] as ChartFastLineSegment).xRange += (series.Segments[i] as ChartFastLineSegment).xRange.End + 0.5;
                        }

                        (series.Segments[i] as ChartFastLineSegment).SetRange(series);

                    }
                    if (series.ActualYAxis.IsAutoSetRange || series.Zoomactionenabled)
                    {
                        (series.Segments[i] as ChartFastLineSegment).isAutoDiscard = series.AutoDiscard;

                        double Y_MAX = linePoints.Max(y => y.Y);
                        double Y_MIN = linePoints.Min(y => y.Y);

                        (series.Segments[i] as ChartFastLineSegment).yRange = new DoubleRange(Y_MIN, Y_MAX);//yRange + cdpt.Y;

                        if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
                        {
                            (series.Segments[i] as ChartFastLineSegment).xRange += (series.Segments[i] as ChartFastLineSegment).xRange.Start - 0.5;
                            (series.Segments[i] as ChartFastLineSegment).xRange += (series.Segments[i] as ChartFastLineSegment).xRange.End + 0.5;
                        }

                        (series.Segments[i] as ChartFastLineSegment).SetRange(series);
                        if (series.Zoomactionenabled)
                        {
                            series.Zoomactionenabled = false;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        ///  <seealso>
        ///      <cref>ChartFastHiLoLineSegment</cref>
        ///  </seealso>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            //series.Segments.Clear();
            //series.Adornments.Clear();

            this.CalculateSegments(series, points);
        }

        #endregion
    }

    /// <summary>
    /// Class implementation for FastLinePresenter
    /// </summary>
    public class FastLinePresenter : ChartFastSeriesPresenter
    {

        /// <summary>
        /// Get or Set Points property 
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        ///  Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(FastLinePresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Get or Set Series property 
        /// </summary>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        ///  Identifies the Series  dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(FastLinePresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        DoubleRange sbsInfo = new DoubleRange(-1, 1);

        //double count = 0d;
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            bool useOptimization = Series.UseOptimization;

            ChartFastLineSegment fastLineSegment = this.DataContext as ChartFastLineSegment;
            double height = fastLineSegment.ViewPortHeight;
            double width = fastLineSegment.ViewPortwidth;

            //double xResolution;
            //double yResolution;
            //Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            //xResolution = m.M11;
            //yResolution = m.M22;
            if (Points != null)
            {
                base.VisualCollection.Clear();
                
                    SyncDrawingVisual visual = new SyncDrawingVisual();
                    PointCollection pp = this.Points;
                    ChartSeries ser = Series;
                    Pen pn = ser.FastTypePen;
                    double adornHeight = Series.AdornmentsInfo.SymbolHeight;
                    double adornWidth = Series.AdornmentsInfo.SymbolWidth;
                    Brush adornBr = Series.AdornmentsInfo.SymbolInterior;
                    Brush adornPenBr = Brushes.Black;
                    Pen adornPen = new Pen(adornBr, 3);
                    //Commented below line because when making pen to freez all its reference object(series becomes null) 
                    //pn.Freeze();
                    if (Series.AdornmentsInfo.Visible == true)
                    {
                    using (DrawingContext context = visual.RenderOpen())
                    {
                        int count = this.Series.Data.Count<pp.Count? this.Series.Data.Count: pp.Count;
                  
                        var ptr = fastLineSegment.dataPtr;
                        for (int j = 0; j <= count - 1; j++)
                        {
                            visual.Index = j;
                            FormattedText text = new FormattedText((this.Series.Data[j].Y).ToString(this.Series.AdornmentsInfo.SegmentLabelFormat), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black);
                            Symbol adornSymbol = Series.AdornmentsInfo.Symbol;
                            switch (adornSymbol)
                            {
                                case Symbol.Square:
                                    context.DrawRectangle(adornBr, adornPen, new Rect(new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 2), new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 2)));
                                    break;
                                case Symbol.Ellipse:
                                    context.DrawEllipse(adornBr, adornPen, pp[j], adornWidth/2, adornHeight/2);
                                    break;
                                case Symbol.HorizontalLine:
                                    context.DrawLine(adornPen, new Point(pp[j].X - adornWidth / 2, pp[j].Y), new Point(pp[j].X + adornWidth / 2, pp[j].Y));
                                    break;
                                case Symbol.VerticalLine:
                                    context.DrawLine(adornPen, new Point(pp[j].X, pp[j].Y + adornHeight / 2), new Point(pp[j].X, pp[j].Y - adornHeight / 2));
                                    break;
                                case Symbol.Triangle:
                                    Point start = new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 2);
                                    LineSegment[] segments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y + adornHeight / 2), true) };
                                    PathFigure figure = new PathFigure(start, segments, true);
                                    PathGeometry geo = new PathGeometry(new PathFigure[] { figure });
                                    context.DrawGeometry(adornBr, null, geo);
                                    break;
                                case Symbol.InvertedTriangle:
                                    Point invertStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y - adornHeight / 2);
                                    LineSegment[] invertSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y + adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 2), true) };
                                    PathFigure invertFigure = new PathFigure(invertStart, invertSegments, true);
                                    PathGeometry invertGeo = new PathGeometry(new PathFigure[] { invertFigure });
                                    context.DrawGeometry(adornBr, null, invertGeo);
                                    break;
                                case Symbol.Diamond:
                                    Point diamondStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y  );
                                    LineSegment[] diamondSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y  ), true), new LineSegment(new Point(pp[j].X , pp[j].Y + adornHeight/2), true) };
                                    PathFigure diamondFigure = new PathFigure(diamondStart, diamondSegments, true);
                                    PathGeometry diamondGeo = new PathGeometry(new PathFigure[] { diamondFigure });
                                    context.DrawGeometry(adornBr, null, diamondGeo);
                                    break;
                                case Symbol.Hexagon:
                                    Point hexStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y  );
                                    LineSegment[] hexSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y), true), new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y + adornHeight / 2), true), new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y + adornHeight / 2), true), };
                                    PathFigure hexFigure = new PathFigure(hexStart, hexSegments, true);
                                    PathGeometry hexGeo = new PathGeometry(new PathFigure[] { hexFigure });
                                    context.DrawGeometry(adornBr, null, hexGeo);
                                    break;
                                case Symbol.Pentagon:
                                    Point pentaStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y);
                                    LineSegment[] pentaSegments = new LineSegment[] { new LineSegment(new Point(pp[j].X , pp[j].Y - adornHeight / 2), true), new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y), true), new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y + adornHeight / 2), true), new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y + adornHeight / 2), true), };
                                    PathFigure pentaFigure = new PathFigure(pentaStart, pentaSegments, true);
                                    PathGeometry pentaGeo = new PathGeometry(new PathFigure[] { pentaFigure });
                                    context.DrawGeometry(adornBr, null, pentaGeo);
                                    break;
                                case Symbol.Plus:
                                    Point plusStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y - adornHeight/6);
                                    LineSegment[] plusSegments = new LineSegment[] { 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y + adornHeight / 6), true),
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y + adornHeight / 6), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 6, pp[j].Y + adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y + adornHeight / 2), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 6, pp[j].Y + adornHeight / 6), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 6), true),
                                    };
                                    PathFigure plusFigure = new PathFigure(plusStart, plusSegments, true);
                                    PathGeometry plusGeo = new PathGeometry(new PathFigure[] { plusFigure });
                                    context.DrawGeometry(adornBr, null, plusGeo);
                                    break;
                                case Symbol.Cross:
                                    Point crossStart = new Point(pp[j].X - adornWidth / 2, pp[j].Y - adornHeight / 4);
                                    LineSegment[] crossSegments = new LineSegment[] { 
                                         new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X, pp[j].Y - adornHeight / 4), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y - adornHeight / 4), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y ), true), 
                                        new LineSegment(new Point(pp[j].X + adornWidth / 2, pp[j].Y + adornHeight / 4), true),
                                        new LineSegment(new Point(pp[j].X + adornWidth / 4, pp[j].Y + adornHeight / 2), true), 
                                        new LineSegment(new Point(pp[j].X, pp[j].Y + adornHeight / 4), true), 
                                        new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y + adornHeight / 2), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 2, pp[j].Y + adornHeight / 4), true),
                                        new LineSegment(new Point(pp[j].X - adornWidth / 4, pp[j].Y ), true),
                                    };
                                    PathFigure crossFigure = new PathFigure(crossStart, crossSegments, true);
                                    PathGeometry crossGeo = new PathGeometry(new PathFigure[] { crossFigure });
                                    context.DrawGeometry(adornBr, null, crossGeo);
                                    break;
                                
                            }
                            Point point = new Point(pp[j].X - text.Width / 2, (pp[j].Y - text.Height / 2));
                           
                            double xPt = Math.Round(point.X, 3);
                            double yPt = Math.Round(point.Y, 3);

                            if (xPt == -Math.Round((text.Width / 2), 3))
                                point.X += (text.Width / 2);
                            else if (xPt == Math.Round((width - (text.Width / 2)), 3))
                                point.X -= (text.Width / 2);
                            if (yPt == -Math.Round((text.Height / 2), 3))
                                point.Y += (text.Height / 2);
                            else if (yPt == Math.Round((height - (text.Height / 2)), 3))
                                point.Y -= (text.Height / 2);
                            context.DrawText(text, point);
                            ptr++;
                        }
                    }
                }

                    base.VisualCollection.Add(visual);
                }
          
             base.OnRender(drawingContext);
            }
           
        }


    
}
