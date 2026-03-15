// <copyright file="ChartLineType.cs" company="Syncfusion">
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
    using System.Windows.Controls;
    using System.Collections;
    using System.Globalization;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents Line chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartLineType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartLineSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Initializes m_point1
        /// </summary>
        internal IChartDataPoint m_point1;

        /// <summary>
        /// Initializes m_point2
        /// </summary>
        internal IChartDataPoint m_point2;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ChartLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(ChartLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ChartLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ChartLineSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the starting X line co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The x1 value.</value>
        public double X1
        {
            get
            {
                return (double)GetValue(X1Property);
            }

            set
            {
                SetValue(X1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the ending X line co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The x2 value.</value>
        public double X2
        {
            get
            {
                return (double)GetValue(X2Property);
            }

            set
            {
                SetValue(X2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the starting Y line co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The y1 value.</value>
        public double Y1
        {
            get
            {
                return (double)GetValue(Y1Property);
            }

            set
            {
                SetValue(Y1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the ending Y line co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The y2 value.</value>
        public double Y2
        {
            get
            {
                return (double)GetValue(Y2Property);
            }

            set
            {
                SetValue(Y2Property, value);
            }
        }

        #region For 3D View
        /// <summary>
        /// Get or Set Z1 property 
        /// </summary>
        public double Z1
        {
            get { return (double)GetValue(Z1Property); }
            set { SetValue(Z1Property, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Z1Property.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty Z1Property =
            DependencyProperty.Register("Z1", typeof(double), typeof(ChartLineSegment), new UIPropertyMetadata(0d));



        /// <summary>
        /// Get or Set Z2 property
        /// </summary>
        public double Z2
        {
            get { return (double)GetValue(Z2Property); }
            set { SetValue(Z2Property, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Z2Property.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty Z2Property =
            DependencyProperty.Register("Z2", typeof(double), typeof(ChartLineSegment), new UIPropertyMetadata(0d));

        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartLineSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default line template is created automatically.
        /// </remarks>
        static ChartLineSegment()
        {
            Type type = typeof(ChartLineSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLineSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="correspondingPoint2">The corresponding point2.</param>
        /// <param name="series">The series.</param>
        public ChartLineSegment(
            IChartDataPoint point1,
            IChartDataPoint point2,
          ChartIndexedDataPoint correspondingPoint1,
            ChartIndexedDataPoint correspondingPoint2,
            ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint1, correspondingPoint2 })
        {
            m_point1 = point1;
            m_point2 = point2;

            this.SetXRange(point1.X, point2.X);
            this.SetYRange(point1.Y, point2.Y);
            if (this.Series.Area.EnableDepthAxis && point1.Values.Length > 1)
                this.SetZRange(point1.Values[1]);
            SetRange(series);
        }


        #endregion

        #region Implmentation

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }
            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }
            base.Update(transformer);
            if (!this.Series.Area.View3DMode)
            {
                Point point1 = new Point();
                Point point2 = new Point();
                double gapCount;

                ////condition check for BreakLineForNonIndexedData
                if (ChartLineType.GetBreakLineForNonIndexedData(this.Series) && !this.Series.IsIndexed && !this.Series.Area.View3DMode)
                {
                    if (this.Series.ActualXAxis.ValueType == ChartValueType.Double)
                    {
                        gapCount = ChartLineType.GetBreakLineForDoublePointsDistanceMoreThan(this.Series);
                    }
                    else
                    {
                        gapCount = (DateTime.Now + (TimeSpan)ChartLineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(this.Series)).ToOADate() - DateTime.Now.ToOADate();
                    }

                    if (gapCount > 0)
                    {
                        if (m_point2.X - m_point1.X <= gapCount)
                        {
                            point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y);
                            point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y);
                        }
                        else
                        {
                            point1 = transformer.TransformToVisible(0, m_point1.Y);
                            point2 = transformer.TransformToVisible(0, m_point2.Y);
                        }
                    }
                    else
                    {
                        point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y);
                        point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y);
                    }
                }
                else
                {
                    point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y);
                    point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y);
                }

                this.X1 = point1.X;
                this.X2 = point2.X;
                this.Y1 = point1.Y;
                this.Y2 = point2.Y;
            }
            else
            {
                Point3D point1 = new Point3D();
                Point3D point2 = new Point3D();
                double zindex = this.Series.Area.EnableDepthAxis && m_point1.Values.Length > 1 ? m_point1.Values[1] : 0.2;
                double gapCount;

                ////condition check for BreakLineForNonIndexedData
                if (ChartLineType.GetBreakLineForNonIndexedData(this.Series) && !this.Series.IsIndexed && !this.Series.Area.View3DMode)
                {
                    if (this.Series.ActualXAxis.ValueType == ChartValueType.Double)
                    {
                        gapCount = ChartLineType.GetBreakLineForDoublePointsDistanceMoreThan(this.Series);
                    }
                    else
                    {
                        gapCount = (DateTime.Now + (TimeSpan)ChartLineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(this.Series)).ToOADate() - DateTime.Now.ToOADate();
                    }

                    if (gapCount > 0)
                    {
                        if (m_point2.X - m_point1.X <= gapCount)
                        {
                            point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y,zindex);
                            point1 = new Point3D(point1.X, point1.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point1.Z) : 0.2);
                            point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y, zindex);
                            point2 = new Point3D(point2.X, point2.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point2.Z) : 0.2);
                        }
                        else
                        {
                            point1 = transformer.TransformToVisible(0, m_point1.Y, zindex);
                            point1 = new Point3D(point1.X, point1.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point1.Z) : 0.2);
                            point2 = transformer.TransformToVisible(0, m_point2.Y, zindex);
                            point2 = new Point3D(point2.X, point2.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point2.Z) : 0.2);
                        }
                    }
                    else
                    {
                        point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y, zindex);
                        point1 = new Point3D(point1.X, point1.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point1.Z) : 0.2);
                        point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y, zindex);
                        point2 = new Point3D(point2.X, point2.Y, this.Series.Area.EnableDepthAxis == true ? (1 - point2.Z) : 0.2);
                    }
                }
                else
                {
                    point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y, zindex);
                    point1 = new Point3D(point1.X, point1.Y,this.Series.Area.EnableDepthAxis==true?(1-point1.Z):0.2);
                    point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y, zindex);
                    point2 = new Point3D(point2.X, point2.Y,this.Series.Area.EnableDepthAxis==true?(1-point2.Z):0.2);
                }

                this.X1 = point1.X;
                this.X2 = point2.X;
                this.Y1 = point1.Y;
                this.Y2 = point2.Y;
                this.Z1 = point1.Z;
                this.Z2 = point2.Z;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="axis"></param>
        protected override void SetPointsForAllSeries(ChartSeries series, ChartAxis axis)
        {

        }


        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The Transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            
            DoubleRange axisRange = Series.XAxis.VisibleRange;
            DoubleRange axisYRange = Series.YAxis.VisibleRange;

            if (((xRange.Start + Series.XAxis.VisibleInterval) >= axisRange.Start && (xRange.End ) <= axisRange.End))
            {
                int index = this.CorrespondingPoints[0].Index;
                ChartLineSegment segmentPrev = null;
                ChartLineSegment segmentNext = null;
                
                if ((index + 1) < this.Series.Segments.Count && Series.AutoDiscard != AutoDiscardType.ResetRange)
                {
                    if (this.Series.Segments[index + 1].GetType() == typeof(ChartLineSegment))
                    {
                        segmentNext = (ChartLineSegment)this.Series.Segments[index + 1];
                        segmentNext.Update(transformer);
                    }
                }

                if ((index - 1) >= 0 && Series.AutoDiscard != AutoDiscardType.ResetRange)
                {                    
                    if (this.Series.Segments[index - 1].GetType() == typeof(ChartLineSegment))
                    {
                        segmentPrev = (ChartLineSegment)this.Series.Segments[index - 1];
                    }
                }
                else
                {
                    this.Update(transformer);
                }

                Vector3D resultP1;
                Vector3D resultP2;
                Vector3D resultP3;
                Vector3D resultP4;

                double width = 0.01d;
                Vector3D pt1 = new Vector3D(this.X1, 1 - this.Y1, this.Z1);
                Vector3D pt2 = new Vector3D(this.X2, 1 - this.Y2, this.Z2);

                Vector3D norm = pt2 - pt1;
                norm.Normalize();
                norm = new Vector3D(-norm.Y, norm.X, norm.Z);

                Vector3D ptt1 = pt1 + width * norm;
                Vector3D ptb1 = pt1 - width * norm;

                Vector3D ptt2 = pt2 + width * norm;
                Vector3D ptb2 = pt2 - width * norm;

                if (segmentPrev != null)
                {
                    Vector3D pt1Prev = new Vector3D(segmentPrev.X1, 1 - segmentPrev.Y1, segmentPrev.Z1);
                    Vector3D pt2Prev = new Vector3D(segmentPrev.X2, 1 - segmentPrev.Y2, segmentPrev.Z2);

                    norm = pt2Prev - pt1Prev;
                    norm.Normalize();
                    norm = new Vector3D(-norm.Y, norm.X, norm.Z);

                    Vector3D ptt1Prev = pt1Prev + width * norm;
                    Vector3D ptb1Prev = pt1Prev - width * norm;

                    Vector3D ptt2Prev = pt2Prev + width * norm;
                    Vector3D ptb2Prev = pt2Prev - width * norm;

                    resultP1 = GetCrossPoint(ptb1Prev, ptb2Prev, ptb1, ptb2);
                    resultP2 = GetCrossPoint(ptt1Prev, ptt2Prev, ptt1, ptt2);
                }
                else
                {
                    resultP1 = ptb1;
                    resultP2 = ptt1;
                }

                if (segmentNext != null)
                {
                    Vector3D pt1Next = new Vector3D(segmentNext.X1, 1 - segmentNext.Y1, segmentNext.Z1);
                    Vector3D pt2Next = new Vector3D(segmentNext.X2, 1 - segmentNext.Y2, segmentNext.Z2);

                    norm = pt2Next - pt1Next;
                    norm.Normalize();
                    norm = new Vector3D(-norm.Y, norm.X, norm.Z);

                    Vector3D ptt1Next = pt1Next + width * norm;
                    Vector3D ptb1Next = pt1Next - width * norm;

                    Vector3D ptt2Next = pt2Next + width * norm;
                    Vector3D ptb2Next = pt2Next - width * norm;

                    resultP3 = GetCrossPoint(ptt1, ptt2, ptt1Next, ptt2Next);
                    resultP4 = GetCrossPoint(ptb1, ptb2, ptb1Next, ptb2Next);
                }
                else
                {
                    resultP3 = ptt2;
                    resultP4 = ptb2;
                }
                int seriesIndex = this.Series.Area.Series.IndexOf(this.Series);
                GeometryModel3D geometryModel = new GeometryModel3D()
                {
                    Geometry = MeshGenerator.LineBar(resultP1, resultP2, resultP3, resultP4, seriesIndex, Series.Area.isClustered, .1 * Series.Area.Series.Count)                    
                    // Geometry=MeshGenerator.Cylinder((double)Series.Segments.Count/250,columnRect.Height,25)                    
                };
                
                MaterialGroup materialGroup;

                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior");
                binding.Source = Series;
                bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
                if (!colorEachValue)
                    BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                else
                    this.Series.UpdateColorEachSegments(this.Series, this, this.Series.Segments.IndexOf(this), difuseMaterial);
                
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);
                geometryModel.Material = materialGroup;

                geometryModel.Transform = new TranslateTransform3D(-0.5, -0.5, (this.Series.Area.Series.IndexOf(this.Series) + 2) * 0.05);
                                
                Geometry3DGroup.Children.Add(geometryModel);
            }
        }

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12 value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>the Vector cross point</returns>
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
            pt.Z = p11.Z + (p12.Z - p11.Z)*ub;
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
        #endregion
    }

    /// <summary>
    /// Represents ChartLineType class
    /// </summary>
    /// <remarks>
    /// Line Charts join points on a plot using straight lines showing trends in data at
    /// equal intervals. Line charts treats the input as non-numeric, categorical
    /// information, equally spaced along the X axis. This is appropriate for
    /// categorical data, such as text labels, but can produce unexpected results when
    /// the X values consist of numbers.
    /// </remarks>
    /// <seealso cref="ChartLineSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartLineType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Attached properties

        /// <summary>
        /// Gets the value of the BreakLineForNonIndexedData dependency property.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>bool value for BreakLineForNonIndexedData</returns>
        public static bool GetBreakLineForNonIndexedData(DependencyObject obj)
        {
            return (bool)obj.GetValue(BreakLineForNonIndexedDataProperty);
        }

        /// <summary>
        /// Sets the value of the BreakLineForNonIndexedData dependency property. 
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <seealso cref="ChartLineType"/>
        public static void SetBreakLineForNonIndexedData(DependencyObject obj, bool value)
        {
            obj.SetValue(BreakLineForNonIndexedDataProperty, value);
        }

        /// <summary>
        ///  Gets the value of the BreakLineForDoublePointsDistanceMoreThan dependency property.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The double value for BreakLineForDoublePointsDistanceMoreThan</returns>
        public static double GetBreakLineForDoublePointsDistanceMoreThan(DependencyObject obj)
        {
            return (double)obj.GetValue(BreakLineForDoublePointsDistanceMoreThanProperty);
        }

        /// <summary>
        /// Sets the value of the BreakLineForDoublePointsDistanceMoreThan dependency property. Used for Double ValueType
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value.</param>
        /// <seealso cref="ChartLineType"/>
        public static void SetBreakLineForDoublePointsDistanceMoreThan(DependencyObject obj, double value)
        {
            obj.SetValue(BreakLineForDoublePointsDistanceMoreThanProperty, value);
        }

        /// <summary>
        /// Gets the value of the BreakLineForTimeSpanPointsDistanceMoreThan dependency property.  Used for DateTime ValueType
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The Timespan for BreakLineForTimeSpanPointsDistanceMoreThan</returns>
        public static TimeSpan GetBreakLineForTimeSpanPointsDistanceMoreThan(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(BreakLineForTimeSpanPointsDistanceMoreThanProperty);
        }

        /// <summary>
        /// Sets the value of the BreakLineForTimeSpanPointsDistanceMoreThan dependency property. 
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value.</param>
        /// <seealso cref="ChartLineType"/>
        public static void SetBreakLineForTimeSpanPointsDistanceMoreThan(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(BreakLineForTimeSpanPointsDistanceMoreThanProperty, value);
        }

        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the BreakLineForNonIndexedData dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForNonIndexedDataProperty =
            DependencyProperty.RegisterAttached("BreakLineForNonIndexedData", typeof(bool), typeof(ChartLineType), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the BreakLineForDoublePointsDistanceMoreThan dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForDoublePointsDistanceMoreThanProperty =
            DependencyProperty.RegisterAttached("BreakLineForDoublePointsDistanceMoreThan", typeof(double), typeof(ChartLineType), new ChartPropertyMetadata(0d, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the BreakLineForTimeSpanPointsDistanceMoreThan dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineForTimeSpanPointsDistanceMoreThanProperty =
            DependencyProperty.RegisterAttached("BreakLineForTimeSpanPointsDistanceMoreThan", typeof(TimeSpan), typeof(ChartLineType), new ChartPropertyMetadata(new TimeSpan(), ChartPropertyMetadataOptions.AffectsUpdate));

        #endregion

        #region Public methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            if(series.Type != ChartTypes.Radar)
            {
            if (series.IsSortData && series.IsIndexed)
            {
                Comparer comparer = new Comparer(new CultureInfo("en-US"));
                for (int i = 0; i < points.Length - 1; i++)
                {
                    for (int j = i; j <= points.Length - 1; j++)
                    {
                        var compare = comparer.Compare(points[i].Index, points[j].Index);
                        if (series.SortDirection == Direction.Ascending ? compare > 0 : compare < 0)
                        {
                            var temp = points[i];
                            points[i] = points[j];
                            points[j] = temp;
                        }
                    }
                }
            }
            bool dotSegmentRequired = ChartLineType.GetBreakLineForNonIndexedData(series) && !series.IsIndexed && !series.Area.View3DMode;
            double gapCount = 0;
            if (dotSegmentRequired)
            {
                if (series.ActualXAxis.ValueType == ChartValueType.Double)
                {
                    gapCount = ChartLineType.GetBreakLineForDoublePointsDistanceMoreThan(series);
                }
                else
                {
                    gapCount = (DateTime.Now + (TimeSpan)ChartLineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(series)).ToOADate() - DateTime.Now.ToOADate();
                }
            }

            dotSegmentRequired = dotSegmentRequired && (gapCount > 0);
            for (int i = 0; i < points.Length; i++)
            {
                ////If Emptypoint, make a difference in segment rendering
                if (points[i].DataPoint.EmptyPoint || (i+1 > points.Length && points[i + 1].DataPoint.EmptyPoint))
                {
                    if (series.ShowEmptyPoints && points[i].DataPoint.EmptyPoint)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            if (series.Segments.Count > 0)
                            {
                                if (series.Segments[series.Segments.Count - 1].GetType() == typeof(ChartLineSegment))
                                {
                                    series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                    series.Segments[series.Segments.Count - 1].Stroke = Brushes.Transparent;
                                }
                            }

                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            if (i + 1 < points.Length)
                            {
                                series.Segments.Add(new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                            }

                            if (i > 0 && i < points.Length)
                            {
                                series.Segments[i - 1].Interior = series.EmptyPointInterior;
                                if(i != series.Segments.Count )
                                series.Segments[i].Interior = series.EmptyPointInterior;
                            }
                        }
                        else
                        {
                            if (series.Segments.Count > 0)
                            {
                                if (series.Segments[series.Segments.Count - 1].GetType() == typeof(ChartLineSegment))
                                {
                                    series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                    series.Segments[series.Segments.Count - 1].Stroke = Brushes.Transparent;
                                    
                                }
                            }

                            ChartEmptySymbolSegment segment = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                            series.Segments.Add(segment);
                        }
                    }
                    else
                    {
                        if (i + 1 < points.Length)
                        {
                            ChartLineSegment segment = new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series);
                            segment.Interior = Brushes.Transparent;
                            segment.Stroke = Brushes.Transparent;
                            series.Segments.Add(segment);
                        }
                    }
                }
                else
                {
                    if (i + 1 < points.Length)
                    {
                        series.Segments.Add(new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                        if ((double.IsNaN(points[i + 1].DataPoint.Y) && !series.ShowEmptyPoints )||( points[i + 1].DataPoint.EmptyPoint && !series.ShowEmptyPoints))
                        {
                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                            series.Segments[series.Segments.Count - 1].Stroke = Brushes.Transparent;
                        }
                    }

                    if (dotSegmentRequired && i + 2 <= points.Length)
                    {
                        if (points[i + 1].DataPoint.X - points[i].DataPoint.X > gapCount)
                        {
                            if (points.Length > i + 2 && points[i + 2].DataPoint.X - points[i + 1].DataPoint.X > gapCount)
                            {
                                ChartIndexedDataPoint indexedPoint2 = points[i + 1];
                            }
                        }
                    }
                }
               
            }
            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                    else
                    {
                        series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                }
            }
        }
            else
            {
                series.Area.m_isRadar = true;
                #region ChartRadarDrawType.Area
                if (ChartRadarType.GetDrawType(series.Area) == ChartRadarDrawType.Area)
                {
                    double origin = series.ActualXAxis.Origin;
                    double origin1 = series.ActualYAxis.Origin;

                    ObservableCollection<IChartDataPoint> areaPoints = new ObservableCollection<IChartDataPoint>();
                    IChartDataPoint[] act_areapoints;
                    bool isclose = ChartRadarType.GetIsClosed(series.Area);
                    if (series.Type == ChartTypes.Radar)
                    {
                        if (isclose)
                        {
                            areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, points[points.Length - 1].DataPoint.Y));
                        }
                        else
                        areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, origin1));
                        areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));


                        for (int i = 0; i < points.Length; i++)
                        {
                            areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));
                        }
                        if (!isclose && points.Length > 0)
                        {
                            areaPoints.Add(new ChartPoint(points[0].DataPoint.X, origin1));
                        }
                        // areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));
                        act_areapoints = new ChartPoint[areaPoints.Count];
                        for (int j = 0; j < areaPoints.Count; j++)
                        {
                            act_areapoints[j] = areaPoints[j];
                        }
                    }
                    else
                    {
                        areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, origin));
                        areaPoints.Add(new ChartPoint(points[0].DataPoint.X, origin));

                        int count = points.Length;
                        for (int i = 0; i < count; i++)
                        {
                            if (i < points.Length && points[i].DataPoint.EmptyPoint)
                            {
                                if (series.ShowEmptyPoints)
                                {
                                    areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));
                                }
                                else
                                {
                                    if (i > 0)
                                    {
                                        ChartPoint p = new ChartPoint();
                                        p.X = points[i - 1].DataPoint.X;
                                        p.Y = origin1;
                                        areaPoints.Add(p);

                                    }
                                    areaPoints.Add(new ChartPoint(points[i].DataPoint.X, origin1));

                                    if (i < points.Length - 1)
                                    {

                                        ChartPoint p1 = new ChartPoint();
                                        p1.X = points[i + 1].DataPoint.X;
                                        p1.Y = origin1;
                                        areaPoints.Add(p1);

                                    }
                                }
                            }
                            else
                            {

                                areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));

                            }
                        }
                        act_areapoints = new ChartPoint[areaPoints.Count];
                        for (int j = 0; j < areaPoints.Count; j++)
                        {
                            act_areapoints[j] = areaPoints[j];
                        }

                    }


                    ChartAreaSegment seg = new ChartAreaSegment(act_areapoints, points, series);
                    series.Segments.Add(seg);


                    if (series.AdornmentsInfo.Visible)
                    {
                        series.Adornments.Clear();
                        for (int i = 0; i < points.Length; i++)
                        {
                            if (points[i].DataPoint.EmptyPoint)
                            {
                                if (series.ShowEmptyPoints)
                                    series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                            }
                        }
                    }
                }
                #endregion
                #region ChartRadarDrawType.Line
                else if (ChartRadarType.GetDrawType(series.Area) == ChartRadarDrawType.Line)
                {
                   
                    bool dotSegmentRequired = ChartLineType.GetBreakLineForNonIndexedData(series) && !series.IsIndexed && !series.Area.View3DMode;
                    double gapCount = 0;
                    if (dotSegmentRequired)
                    {
                        if (series.ActualXAxis.ValueType == ChartValueType.Double)
                        {
                            gapCount = ChartLineType.GetBreakLineForDoublePointsDistanceMoreThan(series);
                        }
                        else
                        {
                            gapCount = (DateTime.Now + (TimeSpan)ChartLineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(series)).ToOADate() - DateTime.Now.ToOADate();
                        }
                    }

                    dotSegmentRequired = dotSegmentRequired && (gapCount > 0);
                    for (int i = 0; i < points.Length; i++)
                    {
                        ////If Emptypoint, make a difference in segment rendering
                        if (points[i].DataPoint.EmptyPoint || (i + 1 > points.Length && points[i + 1].DataPoint.EmptyPoint))
                        {
                            if (series.ShowEmptyPoints && points[i].DataPoint.EmptyPoint)
                            {
                                if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                                {
                                    if (series.Segments.Count > 0)
                                    {
                                        if (series.Segments[series.Segments.Count - 1].GetType() == typeof(ChartLineSegment))
                                        {
                                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                            series.Segments[series.Segments.Count - 1].Stroke = Brushes.Transparent;
                                        }
                                    }

                                    series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                                }
                                else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                                {
                                    if (i + 1 < points.Length)
                                    {
                                        series.Segments.Add(new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                                    }

                                    if (i > 0 && i < points.Length)
                                    {
                                        series.Segments[i - 1].Interior = series.EmptyPointInterior;
                                        if (i != series.Segments.Count)
                                            series.Segments[i].Interior = series.EmptyPointInterior;
                                    }
                                }
                                else
                                {
                                    if (series.Segments.Count > 0)
                                    {
                                        if (series.Segments[series.Segments.Count - 1].GetType() == typeof(ChartLineSegment))
                                        {
                                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                            series.Segments[series.Segments.Count - 1].Stroke = Brushes.Transparent;

                                        }
                                    }

                                    ChartEmptySymbolSegment segment = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                                    series.Segments.Add(segment);
                                }
                            }
                            else
                            {
                                if (i + 1 < points.Length)
                                {
                                    ChartLineSegment segment = new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series);
                                    segment.Interior = Brushes.Transparent;
                                    segment.Stroke = Brushes.Transparent;
                                    series.Segments.Add(segment);
                                }
                            }
                        }
                        else
                        {
                            if (i + 1 < points.Length)
                            {
                                series.Segments.Add(new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                                if (points[i + 1].DataPoint.EmptyPoint && !series.ShowEmptyPoints)
                                {
                                    series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                    series.Segments[series.Segments.Count - 1].Stroke = Brushes.Transparent;
                                }
                            }

                            if (dotSegmentRequired && i + 2 <= points.Length)
                            {
                                if (points[i + 1].DataPoint.X - points[i].DataPoint.X > gapCount)
                                {
                                    if (points.Length > i + 2 && points[i + 2].DataPoint.X - points[i + 1].DataPoint.X > gapCount)
                                    {
                                        ChartIndexedDataPoint indexedPoint2 = points[i + 1];
                                    }
                                }
                            }
                        }

                    }
                    bool isclose = ChartRadarType.GetIsClosed(series.Area);
                    if (isclose && points.Length > 0 && series.Type == ChartTypes.Radar)
                        series.Segments.Add(new ChartLineSegment(points[points.Length - 1].DataPoint, points[0].DataPoint, points[points.Length - 1], points[0], series));
                    if (series.AdornmentsInfo.Visible)
                    {
                        series.Adornments.Clear();
                        for (int i = 0; i < points.Length; i++)
                        {
                            if (points[i].DataPoint.EmptyPoint)
                            {
                                if (series.ShowEmptyPoints)
                                    series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                            }
                        }
                    }
                }
                #endregion
                #region ChartRadarDrawType.Symbol
                else
                {
                    for (int i = 0; i < points.Length; i++)
                    {                        
                       //Since ChartEmptySymbolSegment is similar implementation for drawing symbol, so here ChartEmptySymbolSegment class is used for DrawType "Symbol"
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, ChartRadarType.GetRadarSymbol(series)));

                      
                    }


                    if (series.AdornmentsInfo.Visible)
                    {
                        series.Adornments.Clear();
                        for (int i = 0; i < points.Length; i++)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                    }
                }
                #endregion
            }
        }

        

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartLineType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            CalculateSegments(series, points);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartLineType"/>
        public override string ToString()
        {
            return "Line";
        }
        #endregion
    }
}
