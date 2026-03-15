#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Data;
using System.Globalization;

namespace Syncfusion.Windows.Chart
{

    /// <summary>
    /// Class implementation for ChartFastScatterSegment
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastScatterSegment : ChartSegment
    {
        
        #region Dependency properties
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartFastScatterSegment), new UIPropertyMetadata(null));

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
        private List<IChartDataPoint> m_points;

        /// <summary>
        /// Initializes m_points
        /// </summary>
        private PointCollection m_vpts1;

        private AutoDiscardType isAutoDiscard = AutoDiscardType.None;

        internal bool refresh = false;

        private double viewPortwidth;

        private double viewPortHeight;


        int lastPoint = 0;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartFastScatterSegment"/> class.
        /// </summary>
        /// <remarks>
        /// During initialization the default template for Fast line segment is being created.
        /// </remarks>
        static ChartFastScatterSegment()
        {
            Type type = typeof(ChartFastScatterSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(
                type,
              new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFastScatterSegment"/> class.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The <see cref="ChartSeries"/>.</param>
        internal ChartFastScatterSegment(List<IChartDataPoint> points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {

            //this.Series.ActualXAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);
            //this.Series.ActualYAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);


            m_points = points;
            m_vpts1 = new PointCollection(m_points.Count);

            for (int i = 0; i < m_points.Count; i++)
            {
                m_vpts1.Add(new Point());
            }

            isAutoDiscard = series.AutoDiscard;

            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            foreach (IChartDataPoint cdpt in points)
            {
                xRange += cdpt.X;
                yRange += cdpt.Y;
            }
            DoubleRange sbsinfo = series.Area.GetSideBySideInfo(series);
            //if (this.Series.ActualXAxis.RangeCalculationMode != RangeCalculationMode.ConsistentAcrossChartTypes)
            //{
            //    SetRange(ref sbsinfo);
            //}
            //else
            //{
            //    if (this.Series.ActualYAxis.RangePadding != ChartRangePaddingType.None)
            //    {
            //        var height = ChartScatterType.GetScatterHeight(this.Series);
            //        yRange += yRange.Start - height;
            //        yRange += yRange.End + height;
            //    }
            //}
           
            SetRange(series);
        }

        private void SetRange(ref DoubleRange sbsinfo)
        {
            var height = ChartScatterType.GetScatterHeight(this.Series);           
            if (!this.Series.IsIndexed)
            {
                xRange += xRange.Start - Math.Floor(this.Series.ActualXAxis.VisibleInterval * 0.5);
                xRange += xRange.End + Math.Ceiling(this.Series.ActualXAxis.VisibleInterval * 0.5);

            }
            else
            {
                xRange += xRange.Start - 0.5 + sbsinfo.Start;
                xRange += xRange.End + 0.5 + sbsinfo.End;
            }
            yRange += yRange.Start - height;
            yRange += yRange.End + height;
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

            if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            {
                xRange += xRange.Start - 0.5;
                xRange += xRange.End + 0.5;
            }


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
                    if (item.Segments[0].GetType() == typeof(ChartFastScatterSegment))
                    {
                        ((ChartFastScatterSegment)item.Segments[0]).SetPointToNull();
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

            for (int i = 0; i < m_points.Count; i++)
            {
                m_vpts1.Add(new Point());
            }

            isAutoDiscard = series.AutoDiscard;

            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            foreach (IChartDataPoint cdpt in points)
            {
                xRange += cdpt.X;
                yRange += cdpt.Y;
            }

            if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            {
                xRange += xRange.Start - 0.5;
                xRange += xRange.End + 0.5;
            }

        }

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        ///  <seealso cref="ChartFastScatterSegment"/>
        public override void Update(IChartTransformer transformer)
        {
           
            if (this.Interior!= null && this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }

            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }

            //Set the points to null  when Zomming is enabled to recalculate the series points.
            if (this.Series.Type == ChartTypes.FastLine)
            {
                if (this.Series.Area != null)
                {
                    if (this.Series.Area.ZoomSwitched == true)
                    {
                        this.Points = null;
                    }
                    if (this.Series.Area.PrimaryAxis != null)
                    {
                        if (this.Series.Area.PrimaryAxis.ZoomFactor != 1)
                        {
                            this.Points = null;
                        }
                        else if (this.Series.Area.PrimaryAxis.isNeedUpdate == true)
                        {
                            this.Points = null;
                            this.Series.Area.PrimaryAxis.isNeedUpdate = false;
                        }

                    }
                    if (this.Series.Area.SecondaryAxis != null)
                    {
                        if (this.Series.Area.SecondaryAxis.ZoomFactor != 1)
                        {
                            this.Points = null;
                        }
                        else if (this.Series.Area.SecondaryAxis.isNeedUpdate == true)
                        {
                            this.Points = null;
                            this.Series.Area.PrimaryAxis.isNeedUpdate = false;
                        }
                    }

                }
            }
            List<IChartDataPoint> chartPt = (from pt in m_points where this.Series.XAxis.VisibleRange.Inside(pt.X) select pt).ToList<IChartDataPoint>();
            if (Series.UseOptimization == false)
            {
                chartPt = m_points;
            }
            if (this.Points != null && chartPt.Count < this.Points.Count)
            {
                this.Points = null;
            }

            if (this.Points == null || viewPortwidth != transformer.Viewport.Width || viewPortHeight != transformer.Viewport.Height || this.Series.Area.ZoomAllAxes)
            {
                m_vpts1.Clear();
                for (int i = 0; i < chartPt.Count; i++)
                {
                    //m_vpts1[i] = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
                    m_vpts1.Add(transformer.TransformToVisible(chartPt[i].X, chartPt[i].Y));
                }
                this.Points = m_vpts1.Clone();
                lastPoint = this.Points.Count;
                refresh = (refresh = true ? false : true);
            }
            else
            {
                m_vpts1.Clear();
                for (int i = 0; i < chartPt.Count; i++)
                {
                    m_vpts1.Add(transformer.TransformToVisible(chartPt[i].X, chartPt[i].Y));
                }
                if (chartPt.Count - lastPoint == 1 && m_vpts1[m_vpts1.Count - 1].X == 0 && m_vpts1[m_vpts1.Count - 1].Y == 0)
                {
                    //System.Diagnostics.Debug.WriteLine("X value:" + m_points[m_points.Count - 1].X + "Y Value:" + m_points[m_points.Count - 1].Y);
                    m_vpts1[m_vpts1.Count - 1] = transformer.TransformToVisible(chartPt[chartPt.Count - 1].X, chartPt[chartPt.Count - 1].Y);
                    this.Points[this.Points.Count - 1] = m_vpts1[m_vpts1.Count - 1];
                    refresh = true;
                }
                else if (chartPt.Count != lastPoint)
                {
                    for (int i = lastPoint; i < chartPt.Count; i++)
                    {
                        m_vpts1[i] = transformer.TransformToVisible(chartPt[i].X, chartPt[i].Y);
                    }
                    this.Points = m_vpts1;
                    refresh = true;
                }

                lastPoint = chartPt.Count;
            }
            viewPortwidth = transformer.Viewport.Width;
            viewPortHeight = transformer.Viewport.Height;
        }

        
        //public override void Draw3DSegment(IChartTransformer transformer)
        //{
        //    Vector resultP1;
        //    Vector resultP2;
        //    Vector resultP3;
        //    Vector resultP4;

        //    Point pointStart1 = transformer.TransformToVisible(m_points[0].X, m_points[0].Y);
        //    Point pointStart2 = transformer.TransformToVisible(m_points[1].X, m_points[1].Y);

        //    double width = 0.01d;
        //    Vector startVector1 = new Vector(pointStart1.X, pointStart1.Y);
        //    Vector startVector2 = new Vector(pointStart2.X, pointStart2.Y);

        //    Vector norm = startVector2 - startVector1;
        //    norm.Normalize();
        //    norm = new Vector(-norm.Y, norm.X);

        //    resultP1 = startVector1 + width * norm;
        //    resultP2 = startVector1 - width * norm;

        //    for (int i = 1; i < m_points.Count; i++)
        //    {
        //        Point pointPrev = new Point();
        //        Point pointNext = new Point();
        //        Point pointCurrent = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);

        //        if ((i + 1) < m_points.Count)
        //        {
        //            pointNext = transformer.TransformToVisible(m_points[i + 1].X, m_points[i + 1].Y);
        //        }

        //        if ((i - 1) >= 0)
        //        {
        //            pointPrev = transformer.TransformToVisible(m_points[i - 1].X, m_points[i - 1].Y);
        //        }
        //        else
        //        {
        //            this.Update(transformer);
        //        }

        //        Vector currentVector = new Vector(pointCurrent.X, pointCurrent.Y);
        //        Vector nextVector = new Vector(pointNext.X, pointNext.Y);
        //        Vector prevVector = new Vector(pointPrev.X, pointPrev.Y);

        //        norm = currentVector - prevVector;
        //        norm.Normalize();
        //        norm = new Vector(-norm.Y, norm.X);

        //        Vector ptt1 = prevVector + width * norm;
        //        Vector ptb1 = prevVector - width * norm;

        //        Vector ptt2 = currentVector + width * norm;
        //        Vector ptb2 = currentVector - width * norm;

        //        if (pointNext != null)
        //        {
        //            Vector ptonenext = new Vector(pointCurrent.X, pointCurrent.Y);
        //            Vector pttwonext = new Vector(pointNext.X, pointNext.Y);

        //            norm = pttwonext - ptonenext;
        //            norm.Normalize();
        //            norm = new Vector(-norm.Y, norm.X);

        //            Vector ptt1Next = ptonenext + width * norm;
        //            Vector ptb1Next = ptonenext - width * norm;

        //            Vector ptt2Next = pttwonext + width * norm;
        //            Vector ptb2Next = pttwonext - width * norm;

        //            resultP3 = GetCrossPoint(ptt1, ptt2, ptt1Next, ptt2Next);
        //            resultP4 = GetCrossPoint(ptb1, ptb2, ptb1Next, ptb2Next);
        //        }
        //        else
        //        {
        //            resultP3 = ptt2;
        //            resultP4 = ptb2;
        //        }

        //        GeometryModel3D model = new GeometryModel3D();

        //        model.Geometry = MeshGenerator.FastLineBar(resultP2, resultP1, resultP3, resultP4);

        //        MaterialGroup materialGroup;

        //        DiffuseMaterial difuseMaterial = new DiffuseMaterial();
        //        Binding binding = new Binding("Interior");
        //        binding.Source = Series;
        //        BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
        //        materialGroup = new MaterialGroup();
        //        materialGroup.Children.Add(difuseMaterial);

        //        model.Material = materialGroup;
        //        model.Transform = new TranslateTransform3D(-0.5, -0.5, 0.1);

        //        Geometry3DGroup.Children.Add(model);

        //        resultP1 = resultP3;
        //        resultP2 = resultP4;
        //    }
        //}

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12. value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The cross point</returns>
        private static Vector GetCrossPoint(Vector p11, Vector p12, Vector p21, Vector p22)
        {
            Vector pt = new Vector();
            double z = (p12.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p12.X - p11.X);
            double ca = (p12.Y - p11.Y) * (p21.X - p11.X) - (p21.Y - p11.Y) * (p12.X - p11.X);
            double cb = (p21.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p21.X - p11.X);

            double ua = ca / z;
            double ub = cb / z;

            pt.X = p11.X + (p12.X - p11.X) * ub;
            pt.Y = p11.Y + (p12.Y - p11.Y) * ub;

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
    /// Class implementation for ChartFastScatterType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastScatterType : ChartType
    {
        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartType.ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "FastScatter";
        }

        #region Dependency Properties

        /// <summary>
        /// Return the double Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetFastScatterHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(FastScatterHeightProperty);
        }

        /// <summary>
        /// Sets the value of the FastScatterHeight dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFastScatterHeight(DependencyObject obj, double value)
        {
            obj.SetValue(FastScatterHeightProperty, value);
        }

        /// <summary>
        /// Indicates the FastScatterHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty FastScatterHeightProperty =
                DependencyProperty.RegisterAttached("FastScatterHeight", typeof(double), typeof(ChartFastScatterType), new FrameworkPropertyMetadata(10d, new PropertyChangedCallback(onHeightChanged)));


        private static void onHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries ser = d as ChartSeries;
            if (ser != null && ser.Area != null)
            {
                ser.Invalidate();
            }
        }

        /// <summary>
        /// Return the double Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetFastScatterWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(FastScatterWidthProperty);
        }

        /// <summary>
        /// Sets the value of the FastScatterWidth dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFastScatterWidth(DependencyObject obj, double value)
        {
            obj.SetValue(FastScatterWidthProperty, value);
        }

        /// <summary>
        /// Indicates the FastScatterWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty FastScatterWidthProperty =
                DependencyProperty.RegisterAttached("FastScatterWidth", typeof(double), typeof(ChartFastScatterType), new FrameworkPropertyMetadata(10d, new PropertyChangedCallback(OnWidthChanged)));

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries ser = d as ChartSeries;
            if (ser != null && ser.Area != null)
            {
                ser.Invalidate();
            }
        }
        #endregion

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            List<IChartDataPoint> linePoints = new List<IChartDataPoint>();

            if (series.ShowEmptyPoints == false && series.Area.EnableLazyLoading == true)
            {
                if (series.ActualYAxis.IsAutoSetRange == true || series.ActualXAxis.IsAutoSetRange == true)
                {
                    series.Segments.Clear();
                    series.Adornments.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                        linePoints.Add(points[i].DataPoint);
                    }
                    series.Segments.Add(new ChartFastScatterSegment(linePoints, points, series));

                    return;
                }

                if (series.Segments.Count != 0)
                {
                    ChartFastScatterSegment segment = ((ChartFastScatterSegment)series.Segments[0]);

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
                    series.Segments.Add(new ChartFastScatterSegment(linePoints, points, series));
                }
            }
            else
            {
                series.Segments.Clear();
                series.Adornments.Clear();
                List<ChartIndexedDataPoint> tempPointArray = new List<ChartIndexedDataPoint>();
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].DataPoint.EmptyPoint == false)
                    {
                        linePoints.Add(points[i].DataPoint);
                        tempPointArray.Add(points[i]);

                    }
                    else
                    {
                        if (linePoints.Count > 0)
                        {
                            series.Segments.Add(new ChartFastScatterSegment(linePoints, tempPointArray.ToArray(), series));
                            linePoints = new List<IChartDataPoint>();
                            tempPointArray = new List<ChartIndexedDataPoint>();

                        }
                    }
                }

                if (tempPointArray.Count != 0 && linePoints.Count != 0)
                {
                    series.Segments.Add(new ChartFastScatterSegment(linePoints, tempPointArray.ToArray(), series));
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
    }

    /// <summary>
    /// Class implementation fro 
    /// </summary>
    public class FastScatterPresenter : ChartFastSeriesPresenter
    {

        /// <summary>
        /// Get or Set PointsProperty
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        ///  Identifies the points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(FastScatterPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Get or Set SeriesProperty
        /// </summary>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        ///  Identifies the Series dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(FastScatterPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


/*
        DoubleRange sbsInfo = new DoubleRange(-1, 1);
*/

/*
        double count = 0d;
*/

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            double actHeight = this.ActualHeight;
            double actWidth = this.ActualWidth; 
            if (Points != null)
            {
                base.VisualCollection.Clear();                
                int i = 0;
                Pen pn = Series.FastTypePen;
                Brush br = Series.Interior;
                Brush adornBr = Series.AdornmentsInfo.SymbolInterior;
                double value = Series.Area.ValueToPoint(Series.YAxis, 0);
                PointCollection pp = Points;
                SyncDrawingVisual visual = new SyncDrawingVisual();                
                double width=ChartFastScatterType.GetFastScatterWidth(Series);
                double height=ChartFastScatterType.GetFastScatterHeight(Series);
                double adornHeight = Series.AdornmentsInfo.SymbolHeight;
                double adornWidth = Series.AdornmentsInfo.SymbolWidth;
                Pen adornPen = new Pen(adornBr, 3);
                using (DrawingContext context = visual.RenderOpen())
                {
                    if (Series.AdornmentsInfo.Visible == true)
                    {
                        foreach (var pt in pp)
                        {
                            visual.Index = i;

                            context.DrawEllipse(br, pn, pt, width, height);
                            FormattedText text = new FormattedText(Math.Ceiling(this.Series.Data[i].Y).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black);
                            Symbol adornSymbol = Series.AdornmentsInfo.Symbol;

                            switch (adornSymbol)
                            {
                                case Symbol.Square:
                                    context.DrawRectangle(adornBr, pn, new Rect(new Point(pt.X - adornWidth / 2, pt.Y + adornHeight / 2), new Point(pt.X + adornWidth / 2, pt.Y - adornHeight / 2)));
                                    break;
                                case Symbol.Ellipse:
                                    context.DrawEllipse(adornBr, pn, pt, adornWidth/2, adornHeight/2);
                                    break;
                                case Symbol.HorizontalLine:
                                    context.DrawLine(adornPen, new Point(pt.X - adornWidth / 2, pt.Y), new Point(pt.X + adornWidth / 2, pt.Y));
                                    break;
                                case Symbol.VerticalLine:
                                    context.DrawLine(adornPen, new Point(pt.X, pt.Y + adornHeight / 2), new Point(pt.X, pt.Y - adornHeight / 2));
                                    break;
                                case Symbol.Triangle:
                                    Point start = new Point(pt.X - adornWidth / 2, pt.Y + adornHeight / 2);
                                    LineSegment[] segments = new LineSegment[] { new LineSegment(new Point(pt.X, pt.Y - adornHeight / 2), true), new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y + adornHeight / 2), true) };
                                    PathFigure figure = new PathFigure(start, segments, true);
                                    PathGeometry geo = new PathGeometry(new PathFigure[] { figure });
                                    context.DrawGeometry(adornBr, null, geo);
                                    break;
                                case Symbol.InvertedTriangle:
                                    Point invertStart = new Point(pt.X - adornWidth / 2, pt.Y - adornHeight / 2);
                                    LineSegment[] invertSegments = new LineSegment[] { new LineSegment(new Point(pt.X, pt.Y + adornHeight / 2), true), new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y - adornHeight / 2), true) };
                                    PathFigure invertFigure = new PathFigure(invertStart, invertSegments, true);
                                    PathGeometry invertGeo = new PathGeometry(new PathFigure[] { invertFigure });
                                    context.DrawGeometry(adornBr, null, invertGeo);
                                    break;
                                case Symbol.Diamond:
                                    Point diamondStart = new Point(pt.X - adornWidth / 2, pt.Y);
                                    LineSegment[] diamondSegments = new LineSegment[] { new LineSegment(new Point(pt.X, pt.Y - adornHeight / 2), true), new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y), true), new LineSegment(new Point(pt.X, pt.Y + adornHeight / 2), true) };
                                    PathFigure diamondFigure = new PathFigure(diamondStart, diamondSegments, true);
                                    PathGeometry diamondGeo = new PathGeometry(new PathFigure[] { diamondFigure });
                                    context.DrawGeometry(adornBr, null, diamondGeo);
                                    break;
                                case Symbol.Hexagon:
                                    Point hexStart = new Point(pt.X - adornWidth / 2, pt.Y);
                                    LineSegment[] hexSegments = new LineSegment[] { new LineSegment(new Point(pt.X - adornWidth / 4, pt.Y - adornHeight / 2), true), new LineSegment(new Point(pt.X + adornWidth / 4, pt.Y - adornHeight / 2), true), new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y), true), new LineSegment(new Point(pt.X + adornWidth / 4, pt.Y + adornHeight / 2), true), new LineSegment(new Point(pt.X - adornWidth / 4, pt.Y + adornHeight / 2), true), };
                                    PathFigure hexFigure = new PathFigure(hexStart, hexSegments, true);
                                    PathGeometry hexGeo = new PathGeometry(new PathFigure[] { hexFigure });
                                    context.DrawGeometry(adornBr, null, hexGeo);
                                    break;
                                case Symbol.Pentagon:
                                    Point pentaStart = new Point(pt.X - adornWidth / 2, pt.Y);
                                    LineSegment[] pentaSegments = new LineSegment[] { new LineSegment(new Point(pt.X, pt.Y - adornHeight / 2), true), new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y), true), new LineSegment(new Point(pt.X + adornWidth / 4, pt.Y + adornHeight / 2), true), new LineSegment(new Point(pt.X - adornWidth / 4, pt.Y + adornHeight / 2), true), };
                                    PathFigure pentaFigure = new PathFigure(pentaStart, pentaSegments, true);
                                    PathGeometry pentaGeo = new PathGeometry(new PathFigure[] { pentaFigure });
                                    context.DrawGeometry(adornBr, null, pentaGeo);
                                    break;
                                case Symbol.Plus:
                                    Point plusStart = new Point(pt.X - adornWidth / 2, pt.Y - adornHeight / 6);
                                    LineSegment[] plusSegments = new LineSegment[] { 
                                        new LineSegment(new Point(pt.X - adornWidth / 6, pt.Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pt.X - adornWidth / 6, pt.Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 6, pt.Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 6, pt.Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y - adornHeight / 6), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y + adornHeight / 6), true),
                                        new LineSegment(new Point(pt.X + adornWidth / 6, pt.Y + adornHeight / 6), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 6, pt.Y + adornHeight / 2), true), 
                                        new LineSegment(new Point(pt.X - adornWidth / 6, pt.Y + adornHeight / 2), true),
                                        new LineSegment(new Point(pt.X - adornWidth / 6, pt.Y + adornHeight / 6), true),
                                        new LineSegment(new Point(pt.X - adornWidth / 2, pt.Y + adornHeight / 6), true),
                                    };
                                    PathFigure plusFigure = new PathFigure(plusStart, plusSegments, true);
                                    PathGeometry plusGeo = new PathGeometry(new PathFigure[] { plusFigure });
                                    context.DrawGeometry(adornBr, null, plusGeo);
                                    break;
                                case Symbol.Cross:
                                    Point crossStart = new Point(pt.X - adornWidth / 2, pt.Y - adornHeight / 4);
                                    LineSegment[] crossSegments = new LineSegment[] { 
                                         new LineSegment(new Point(pt.X - adornWidth / 4, pt.Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pt.X, pt.Y - adornHeight / 4), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 4, pt.Y - adornHeight / 2), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y - adornHeight / 4), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 4, pt.Y ), true), 
                                        new LineSegment(new Point(pt.X + adornWidth / 2, pt.Y + adornHeight / 4), true),
                                        new LineSegment(new Point(pt.X + adornWidth / 4, pt.Y + adornHeight / 2), true), 
                                        new LineSegment(new Point(pt.X, pt.Y + adornHeight / 4), true), 
                                        new LineSegment(new Point(pt.X - adornWidth / 4, pt.Y + adornHeight / 2), true),
                                        new LineSegment(new Point(pt.X - adornWidth / 2, pt.Y + adornHeight / 4), true),
                                        new LineSegment(new Point(pt.X - adornWidth / 4, pt.Y ), true),
                                    };
                                    PathFigure crossFigure = new PathFigure(crossStart, crossSegments, true);
                                    PathGeometry crossGeo = new PathGeometry(new PathFigure[] { crossFigure });
                                    context.DrawGeometry(adornBr, null, crossGeo);
                                    break;

                            }
                            Point point = new Point(pt.X - text.Width / 2, (pt.Y) - text.Height / 2);
                            double xPt = Math.Round(point.X, 3);
                            double yPt = Math.Round(point.Y, 3);

                            if (xPt == -Math.Round((text.Width / 2), 3))
                                point.X += (text.Width / 2);
                            else if (xPt == Math.Round((actWidth - (text.Width / 2)), 3))
                                point.X -= (text.Width / 2);
                            if (yPt == -Math.Round((text.Height / 2), 3))
                                point.Y += (text.Height / 2);
                            else if (yPt == Math.Round((actHeight - (text.Height / 2)), 3))
                                point.Y -= (text.Height / 2);
                            context.DrawText(text, point);
                            i++;

                        }

                    }
                    else
                    {
                        foreach (var pt in pp)
                        {
                        visual.Index = i;            

                        context.DrawEllipse(br, pn, pt,width ,height);
                        i++;
                        }
                    }
                }
                base.VisualCollection.Add(visual);
            }
            base.OnRender(drawingContext);
        }


    }
}
