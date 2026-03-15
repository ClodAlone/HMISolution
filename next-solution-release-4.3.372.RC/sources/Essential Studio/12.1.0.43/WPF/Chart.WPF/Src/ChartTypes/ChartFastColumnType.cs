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
    /// Class implemenation for ChartFastColumnSegment
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastColumnSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartFastColumnSegment), new UIPropertyMetadata(null));

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

        private DoubleRange m_sbsinfo;
        int lastPoint = 0;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartFastColumnSegment"/> class.
        /// </summary>
        /// <remarks>
        /// During initialization the default template for Fast line segment is being created.
        /// </remarks>
        static ChartFastColumnSegment()
        {
            Type type = typeof(ChartFastColumnSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(
                type,
              new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFastColumnSegment"/> class.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The <see cref="ChartSeries"/>.</param>
        internal ChartFastColumnSegment(List<IChartDataPoint> points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {

            //this.Series.ActualXAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);
            //this.Series.ActualYAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);
          m_sbsinfo=  this.Series.Area.GetSideBySideInfo(series); 

            m_points = points;
            m_vpts1 = new PointCollection(m_points.Count * 2);

            for (int i = 0; i < m_points.Count * 2; i++)
            {
                m_vpts1.Add(new Point());
            }

            isAutoDiscard = series.AutoDiscard;

            xRange = DoubleRange.Empty;
            //SD12207-Fixed the issue that the secondary axis range is calculated based on starting datapoint value. The Range must be calculated based on origin value of primary axis. 
            yRange = new DoubleRange(series.ActualXAxis.Origin, series.ActualXAxis.Origin);
            

            foreach (IChartDataPoint cdpt in points)
            {
                xRange += cdpt.X;
                yRange += cdpt.Y;
            }
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            if (series.ActualXAxis.RangeCalculationMode != RangeCalculationMode.ConsistentAcrossChartTypes )
            {
                if (series.SegmentWidthMode == ChartSeries.Mode.Relative)
                {
                    var relativeStartPos = points[0].Values.Length > 1 ? points[0].Values[1] * 0.5 : 0;
                    var relativeEndPos = points[points.Count-1].Values.Length > 1 ? points[points.Count-1].Values[1] * 0.5 : 0;
                    xRange += xRange.Start - relativeStartPos;
                    xRange += xRange.End + relativeEndPos;
                }
                else
                {
                    xRange += xRange.Start - 0.5 ;
                    xRange += xRange.End + 0.5 ;
                }
            }
            else if (series.Area.SyncChartArea == null)
            {
                //xRange += xRange.Start - 0.5;
                //xRange += xRange.End + 0.5;
                //Commented for fix SD9792 
                //yRange += yRange.Start - 0.5;
                //yRange += yRange.End + 0.5;
            }
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
                    if (item.Segments[0].GetType() == typeof(ChartFastColumnSegment))
                    {
                        ((ChartFastColumnSegment)item.Segments[0]).SetPointToNull();
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
        ///  <seealso cref="ChartFastColumnSegment"/>
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
            //SBS Info calculation for column width is included for fast column with SF4863 feature
          //  DoubleRange sbsinfo = this.Series.Area.GetSideBySideInfo(this.Series); 
            if (this.Points != null && m_points.Count < this.Points.Count)
            {
                this.Points = null;
            }

            if (this.Points == null || viewPortwidth != transformer.Viewport.Width || viewPortHeight != transformer.Viewport.Height || this.Series.Area.ZoomAllAxes)
            {
                //SD12207- Here the fast column segment is made to draw from origin line  which the user has specified, as like segment drawn for column chart. 
                double seg_origin = this.Series.ActualXAxis.Origin;
                if (this.Series != null && this.Series.SegmentWidthMode == ChartSeries.Mode.Relative)
                {
                    for (int i = 0, j = 0; i < m_points.Count; i++, j = j + 2)
                    {
                        m_vpts1[j] = transformer.TransformToVisible(m_points[i].X - (m_points[i].Values.Length > 1 ? m_points[i].Values[1] * 0.5 : 0), seg_origin);
                        m_vpts1[j + 1] = transformer.TransformToVisible(m_points[i].X + (m_points[i].Values.Length > 1 ? m_points[i].Values[1] * 0.5 : 0), m_points[i].Y);
                    }
                }
                else
                {
                    for (int i = 0, j = 0; i < m_points.Count; i++, j = j + 2)
                    {
                        m_vpts1[j] = transformer.TransformToVisible(m_points[i].X + m_sbsinfo.Start, seg_origin);
                        m_vpts1[j + 1] = transformer.TransformToVisible(m_points[i].X + m_sbsinfo.End, m_points[i].Y);
                    }
                }
                                     
                this.Points = m_vpts1;
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
                else if (m_points.Count != lastPoint)
                {
                    for (int i = lastPoint; i < m_points.Count; i++)
                    {
                        m_vpts1[i] = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
                    }
                    this.Points = m_vpts1;
                    refresh = true;
                }

                lastPoint = m_points.Count;
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
    /// Class implementation for 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastColumnType : ChartType
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "FastColumn";
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartType.ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Indexed;
            }
        }

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
                    series.Segments.Add(new ChartFastColumnSegment(linePoints, points, series));

                    return;
                }

                if (series.Segments.Count != 0)
                {
                    ChartFastColumnSegment segment = ((ChartFastColumnSegment)series.Segments[0]);

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
                    series.Segments.Add(new ChartFastColumnSegment(linePoints, points, series));
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
                            if (i < points.Length - 1)
                                linePoints.Add(points[i + 1].DataPoint);
                            series.Segments.Add(new ChartFastColumnSegment(linePoints, tempPointArray.ToArray(), series));
                            linePoints = new List<IChartDataPoint>();
                            tempPointArray = new List<ChartIndexedDataPoint>();

                        }
                    }
                }

                if (tempPointArray.Count != 0 && linePoints.Count != 0)
                {
                    series.Segments.Add(new ChartFastColumnSegment(linePoints, tempPointArray.ToArray(), series));
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
    /// Class implementation for FastColumnPresenter
    /// </summary>
    public class FastColumnPresenter : ChartFastSeriesPresenter
    {

        /// <summary>
        /// Gets or Sets the PointsProperty
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(FastColumnPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


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
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(FastColumnPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

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
                PointCollection pp = Points;                
                Brush br = Series.Interior;
                Brush adornBr = Series.AdornmentsInfo.SymbolInterior;
                Pen pn = Series.FastTypePen;
                Pen adornPen = new Pen(adornBr, 3);
                double x1, x2, y1, y2;
                //pn.Freeze();
                //br.Freeze();
                SyncDrawingVisual visual = new SyncDrawingVisual();
                double adornHeight =  Series.AdornmentsInfo.SymbolHeight;
                double adornWidth =  Series.AdornmentsInfo.SymbolWidth;
                Brush myBrush = Series.AdornmentsInfo.SymbolInterior;
                using (DrawingContext context = visual.RenderOpen())
                {
                    if (Series.AdornmentsInfo.Visible == true)
                    {
                        for (int i = 0; i < pp.Count; i += 2)
                        {
                            context.DrawRectangle(br, pn, new Rect(new Point(pp[i].X, pp[i].Y), new Point(pp[i + 1].X, pp[i + 1].Y)));
                            x1 = pp[i].X;
                            x2 = pp[i + 1].X;
                            y1 = pp[i].Y;
                            y2 = pp[i + 1].Y;
                            if (Series.AdornmentsInfo.Visible == true)
                            {
                                FormattedText text = new FormattedText(Math.Ceiling(this.Series.Data[i / 2].Y).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black);
                                Symbol adornSymbol = Series.AdornmentsInfo.Symbol;

                                switch (adornSymbol)
                                {
                                    case Symbol.Square:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            context.DrawRectangle(myBrush, pn, new Rect(new Point(((x1 + x2) / 2) - adornWidth / 2, y2 + adornHeight / 2), new Point(((x1 + x2) / 2) + adornWidth / 2, y2 - adornHeight / 2)));
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            context.DrawRectangle(myBrush, pn, new Rect(new Point(x2 + adornWidth / 2, (y1 + y2) / 2 + adornHeight / 2), new Point(x2 - adornWidth / 2, (y1 + y2) / 2 - adornHeight / 2)));
                                        }
                                        break;
                                    case Symbol.Ellipse:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            context.DrawEllipse(myBrush, pn, new Point((x1 + x2) / 2, y2), adornWidth/2, adornHeight/2);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            context.DrawEllipse(myBrush, pn, new Point(x2, (y1 + y2) / 2), adornWidth/2, adornHeight/2);
                                        }
                                        break;
                                    case Symbol.HorizontalLine:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            context.DrawLine(adornPen, new Point(((x1 + x2) / 2) - adornWidth / 2, y2), new Point(((x1 + x2) / 2) + adornWidth / 2, y2));
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            context.DrawLine(adornPen, new Point(x2 + adornHeight / 2, ((y1 + y2) / 2)), new Point(x2 - adornHeight / 2, ((y1 + y2) / 2)));
                                            //context.DrawLine(adornPen, new Point(x2, ((y1 + y2) / 2) - adornWidth / 2), new Point(x2, ((y1 + y2) / 2) + adornWidth / 2));
                                        }
                                        break;
                                    case Symbol.VerticalLine:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            context.DrawLine(adornPen, new Point(((x1 + x2) / 2), y2 + adornHeight / 2), new Point(((x1 + x2) / 2), y2 - adornHeight / 2));
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            context.DrawLine(adornPen, new Point(x2, ((y1 + y2) / 2) - adornWidth / 2), new Point(x2, ((y1 + y2) / 2) + adornWidth / 2));
                                            //context.DrawLine(adornPen, new Point(x2 + adornHeight / 2, ((y1 + y2) / 2)), new Point(x2 - adornHeight / 2, ((y1 + y2) / 2)));
                                        }
                                        break;
                                    case Symbol.Triangle:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point start = new Point(((x1 + x2) / 2) - adornWidth / 2, y2 + adornHeight / 2);
                                            LineSegment[] segments = new LineSegment[] { new LineSegment(new Point(((x1 + x2) / 2), y2 - adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2 + adornHeight / 2), true) };
                                            PathFigure figure = new PathFigure(start, segments, true);
                                            PathGeometry geo = new PathGeometry(new PathFigure[] { figure });
                                            context.DrawGeometry(adornBr, null, geo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point start = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2) + adornHeight / 2);
                                            LineSegment[] segments = new LineSegment[] { new LineSegment(new Point(x2, ((y1 + y2) / 2) - adornHeight / 2), true), new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2) + adornHeight / 2), true) };
                                            PathFigure figure = new PathFigure(start, segments, true);
                                            PathGeometry geo = new PathGeometry(new PathFigure[] { figure });
                                            context.DrawGeometry(adornBr, null, geo);
                                        }
                                        break;
                                    case Symbol.InvertedTriangle:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point invertStart = new Point(((x1 + x2) / 2) - adornWidth / 2, y2 - adornHeight / 2);
                                            LineSegment[] invertSegments = new LineSegment[] { new LineSegment(new Point(((x1 + x2) / 2), y2 + adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2 - adornHeight / 2), true) };
                                            PathFigure invertFigure = new PathFigure(invertStart, invertSegments, true);
                                            PathGeometry invertGeo = new PathGeometry(new PathFigure[] { invertFigure });
                                            context.DrawGeometry(adornBr, null, invertGeo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point invertStart = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2) - adornHeight / 2);
                                            LineSegment[] invertSegments = new LineSegment[] { new LineSegment(new Point(x2, ((y1 + y2) / 2) + adornHeight / 2), true), new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2) - adornHeight / 2), true) };
                                            PathFigure invertFigure = new PathFigure(invertStart, invertSegments, true);
                                            PathGeometry invertGeo = new PathGeometry(new PathFigure[] { invertFigure });
                                            context.DrawGeometry(adornBr, null, invertGeo);
                                        }
                                        break;
                                    case Symbol.Diamond:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point diamondStart = new Point(((x1 + x2) / 2) - adornWidth / 2, y2);
                                            LineSegment[] diamondSegments = new LineSegment[] { new LineSegment(new Point(((x1 + x2) / 2), y2 - adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2), true), new LineSegment(new Point(((x1 + x2) / 2), y2 + adornHeight / 2), true) };
                                            PathFigure diamondFigure = new PathFigure(diamondStart, diamondSegments, true);
                                            PathGeometry diamondGeo = new PathGeometry(new PathFigure[] { diamondFigure });
                                            context.DrawGeometry(adornBr, null, diamondGeo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point diamondStart = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2));
                                            LineSegment[] diamondSegments = new LineSegment[] { new LineSegment(new Point(x2, ((y1 + y2) / 2) - adornHeight / 2), true), new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2)), true), new LineSegment(new Point(x2, ((y1 + y2) / 2) + adornHeight / 2), true) };
                                            PathFigure diamondFigure = new PathFigure(diamondStart, diamondSegments, true);
                                            PathGeometry diamondGeo = new PathGeometry(new PathFigure[] { diamondFigure });
                                            context.DrawGeometry(adornBr, null, diamondGeo);
                                        }
                                        break;
                                    case Symbol.Hexagon:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point hexStart = new Point(((x1 + x2) / 2) - adornWidth / 2, y2);
                                            LineSegment[] hexSegments = new LineSegment[] { new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 4, y2 - adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 4, y2 - adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 4, y2 + adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 4, y2 + adornHeight / 2), true), };
                                            PathFigure hexFigure = new PathFigure(hexStart, hexSegments, true);
                                            PathGeometry hexGeo = new PathGeometry(new PathFigure[] { hexFigure });
                                            context.DrawGeometry(adornBr, null, hexGeo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point hexStart = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2));
                                            LineSegment[] hexSegments = new LineSegment[] { new LineSegment(new Point(x2 - adornWidth / 4, ((y1 + y2) / 2) - adornHeight / 2), true), new LineSegment(new Point(x2 + adornWidth / 4, ((y1 + y2) / 2) - adornHeight / 2), true), new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2)), true), new LineSegment(new Point(x2 + adornWidth / 4, ((y1 + y2) / 2) + adornHeight / 2), true), new LineSegment(new Point(x2 - adornWidth / 4, ((y1 + y2) / 2) + adornHeight / 2), true), };
                                            PathFigure hexFigure = new PathFigure(hexStart, hexSegments, true);
                                            PathGeometry hexGeo = new PathGeometry(new PathFigure[] { hexFigure });
                                            context.DrawGeometry(adornBr, null, hexGeo);
                                        }
                                        break;
                                    case Symbol.Pentagon:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point pentaStart = new Point(((x1 + x2) / 2) - adornWidth / 2, y2);
                                            LineSegment[] pentaSegments = new LineSegment[] { new LineSegment(new Point(((x1 + x2) / 2), y2 - adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2), true), new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 4, y2 + adornHeight / 2), true), new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 4, y2 + adornHeight / 2), true), };
                                            PathFigure pentaFigure = new PathFigure(pentaStart, pentaSegments, true);
                                            PathGeometry pentaGeo = new PathGeometry(new PathFigure[] { pentaFigure });
                                            context.DrawGeometry(adornBr, null, pentaGeo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point pentaStart = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2));
                                            LineSegment[] pentaSegments = new LineSegment[] { new LineSegment(new Point(x2, ((y1 + y2) / 2) - adornHeight / 2), true), new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2)), true), new LineSegment(new Point(x2 + adornWidth / 4, ((y1 + y2) / 2) + adornHeight / 2), true), new LineSegment(new Point(x2 - adornWidth / 4, ((y1 + y2) / 2) + adornHeight / 2), true), };
                                            PathFigure pentaFigure = new PathFigure(pentaStart, pentaSegments, true);
                                            PathGeometry pentaGeo = new PathGeometry(new PathFigure[] { pentaFigure });
                                            context.DrawGeometry(adornBr, null, pentaGeo);
                                        }
                                        break;
                                    case Symbol.Plus:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point plusStart = new Point(((x1 + x2) / 2) - adornWidth / 2, y2 - adornHeight / 6);
                                            LineSegment[] plusSegments = new LineSegment[] { 
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 6, y2 - adornHeight / 6), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 6, y2 - adornHeight / 2), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 6, y2 - adornHeight / 2), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 6, y2 - adornHeight / 6), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2 - adornHeight / 6), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2 + adornHeight / 6), true),
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 6, y2 + adornHeight / 6), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 6, y2 + adornHeight / 2), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 6, y2 + adornHeight / 2), true),
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 6, y2 + adornHeight / 6), true),
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 2, y2 + adornHeight / 6), true),
                                            };
                                            PathFigure plusFigure = new PathFigure(plusStart, plusSegments, true);
                                            PathGeometry plusGeo = new PathGeometry(new PathFigure[] { plusFigure });
                                            context.DrawGeometry(adornBr, null, plusGeo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point plusStart = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2) - adornHeight / 6);
                                            LineSegment[] plusSegments = new LineSegment[] { 
                                                new LineSegment(new Point(x2 - adornWidth / 6, ((y1 + y2) / 2) - adornHeight / 6), true), 
                                                new LineSegment(new Point(x2 - adornWidth / 6, ((y1 + y2) / 2) - adornHeight / 2), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 6, ((y1 + y2) / 2) - adornHeight / 2), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 6, ((y1 + y2) / 2) - adornHeight / 6), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2) - adornHeight / 6), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2) + adornHeight / 6), true),
                                                new LineSegment(new Point(x2 + adornWidth / 6, ((y1 + y2) / 2) + adornHeight / 6), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 6, ((y1 + y2) / 2) + adornHeight / 2), true), 
                                                new LineSegment(new Point(x2 - adornWidth / 6, ((y1 + y2) / 2) + adornHeight / 2), true),
                                                new LineSegment(new Point(x2 - adornWidth / 6, ((y1 + y2) / 2) + adornHeight / 6), true),
                                                new LineSegment(new Point(x2 - adornWidth / 2, ((y1 + y2) / 2) + adornHeight / 6), true),
                                            };
                                            PathFigure plusFigure = new PathFigure(plusStart, plusSegments, true);
                                            PathGeometry plusGeo = new PathGeometry(new PathFigure[] { plusFigure });
                                            context.DrawGeometry(adornBr, null, plusGeo);
                                        }
                                        break;
                                    case Symbol.Cross:
                                        if ((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                        {
                                            Point crossStart = new Point(((x1 + x2) / 2) - adornWidth / 2, y2 - adornHeight / 4);
                                            LineSegment[] crossSegments = new LineSegment[] { 
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 4, y2 - adornHeight / 2), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2), y2 - adornHeight / 4), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 4, y2 - adornHeight / 2), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2 - adornHeight / 4), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 4, y2 ), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 2, y2 + adornHeight / 4), true),
                                                new LineSegment(new Point(((x1 + x2) / 2) + adornWidth / 4, y2 + adornHeight / 2), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2), y2 + adornHeight / 4), true), 
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 4, y2 + adornHeight / 2), true),
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 2, y2 + adornHeight / 4), true),
                                                new LineSegment(new Point(((x1 + x2) / 2) - adornWidth / 4, y2 ), true),
                                            };
                                            PathFigure crossFigure = new PathFigure(crossStart, crossSegments, true);
                                            PathGeometry crossGeo = new PathGeometry(new PathFigure[] { crossFigure });
                                            context.DrawGeometry(adornBr, null, crossGeo);
                                        }
                                        else if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                        {
                                            Point crossStart = new Point(x2 - adornWidth / 2, ((y1 + y2) / 2) - adornHeight / 4);
                                            LineSegment[] crossSegments = new LineSegment[] { 
                                                new LineSegment(new Point(x2 - adornWidth / 4, ((y1 + y2) / 2) - adornHeight / 2), true), 
                                                new LineSegment(new Point(x2, ((y1 + y2) / 2) - adornHeight / 4), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 4, ((y1 + y2) / 2) - adornHeight / 2), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2) - adornHeight / 4), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 4, ((y1 + y2) / 2) ), true), 
                                                new LineSegment(new Point(x2 + adornWidth / 2, ((y1 + y2) / 2) + adornHeight / 4), true),
                                                new LineSegment(new Point(x2 + adornWidth / 4, ((y1 + y2) / 2) + adornHeight / 2), true), 
                                                new LineSegment(new Point(x2, ((y1 + y2) / 2) + adornHeight / 4), true), 
                                                new LineSegment(new Point(x2 - adornWidth / 4, ((y1 + y2) / 2) + adornHeight / 2), true),
                                                new LineSegment(new Point(x2 - adornWidth / 2, ((y1 + y2) / 2) + adornHeight / 4), true),
                                                new LineSegment(new Point(x2 - adornWidth / 4, ((y1 + y2) / 2) ), true),
                                            };
                                            PathFigure crossFigure = new PathFigure(crossStart, crossSegments, true);
                                            PathGeometry crossGeo = new PathGeometry(new PathFigure[] { crossFigure });
                                            context.DrawGeometry(adornBr, null, crossGeo);
                                        }
                                        break;

                                }
                                if ((Series.Type == ChartTypes.FastBar && !Series.IsRotated) || (Series.Type == ChartTypes.FastColumn && Series.IsRotated))
                                {
                                    //Geometry geometry = text.BuildGeometry(new Point(x2 - text.Width / 2, (y1 + y2) / 2 - text.Height / 2));
                                    //context.DrawGeometry(br, pn, geometry);
                                    Point pt = new Point(x2 - text.Width / 2, (y1 + y2) / 2 - text.Height / 2);
                                    double xPt = Math.Round(pt.X, 3);
                                    double yPt = Math.Round(pt.Y, 3);

                                    if (xPt == -Math.Round((text.Width / 2),3))
                                        pt.X += (text.Width / 2);
                                    else if (xPt == Math.Round((actWidth - (text.Width / 2)),3))
                                        pt.X -= (text.Width / 2);
                                    if (yPt == -Math.Round((text.Height / 2), 3))
                                        pt.Y += (text.Height / 2);
                                    else if (yPt == Math.Round((actHeight - (text.Height / 2)),3))
                                        pt.Y -= (text.Height / 2);

                                    context.DrawText(text, pt);
                                }
                                else if((Series.Type == ChartTypes.FastColumn && !Series.IsRotated) || (Series.Type == ChartTypes.FastBar && Series.IsRotated))
                                {
                                    //Geometry geometry = text.BuildGeometry(new Point((x1 + x2) / 2 - text.Width / 2, y2 - text.Height / 2));
                                    //context.DrawGeometry(br, pn, geometry);
                                    Point pt = new Point((x1 + x2) / 2 - text.Width / 2, y2 - text.Height / 2);
                                    double xPt = Math.Round(pt.X, 3);
                                    double yPt = Math.Round(pt.Y, 3);

                                    if (xPt == -Math.Round((text.Width / 2), 3))
                                        pt.X += (text.Width / 2);
                                    else if (xPt == Math.Round((actWidth - (text.Width / 2)), 3))
                                        pt.X -= (text.Width / 2);
                                    if (yPt == -Math.Round((text.Height / 2), 3))
                                        pt.Y += (text.Height / 2);
                                    else if (yPt == Math.Round((actHeight - (text.Height / 2)), 3))
                                        pt.Y -= (text.Height / 2);
                                    context.DrawText(text, pt);
                                }


                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pp.Count; i += 2)
                        {
                            context.DrawRectangle(br, pn, new Rect(new Point(pp[i].X, pp[i].Y), new Point(pp[i + 1].X, pp[i + 1].Y)));
                        }
                    }
                }
                base.VisualCollection.Add(visual);
            }
            base.OnRender(drawingContext);
        }
    }

    /// <summary>
    /// Class implementation for SyncdrawingVisual
    /// </summary>
    public class SyncDrawingVisual : DrawingVisual
    {
        /// <summary>
        /// Get and Set Index proeprty
        /// </summary>
        public int Index { get; set; }

    }
}
