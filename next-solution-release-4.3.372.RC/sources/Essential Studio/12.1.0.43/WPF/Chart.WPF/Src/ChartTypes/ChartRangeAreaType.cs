// <copyright file="ChartRangeAreaType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents chart range area segment.
    /// </summary>    
    /// <seealso cref="ChartRangeAreaType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartRangeAreaSegment : ChartAreaSegment
    {
        #region Internal types
        /// <summary>
        /// Represents SegmentFillConverter
        /// </summary>
        /// <seealso cref="ChartRangeAreaSegment"/>
        private class SegmentFillConverter : IMultiValueConverter
        {
            #region IMultiValueConverter Members
            /// <summary>
            /// The Convert method
            /// </summary>
            /// <param name="values">The object values</param>
            /// <param name="targetType">The targetType</param>
            /// <param name="parameter">The parameter</param>
            /// <param name="culture">The culture</param>
            /// <returns>Returns the value</returns>
            /// <seealso cref="SegmentFillConverter"/>
            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                Brush hiValue = values[0] as Brush;
                Brush lowValue = values[1] as Brush;
                Brush seriesInterior = values[2] as Brush;
                ChartRangeAreaSegment targetSegment = parameter as ChartRangeAreaSegment;
                if (targetSegment.IsHighLow)
                {
                    return (hiValue != null) ? hiValue : seriesInterior;
                }
                else
                {
                    return (lowValue != null) ? lowValue : seriesInterior;
                }
            }

            /// <summary>
            /// The ConvertBack method
            /// </summary>
            /// <param name="value">The object value</param>
            /// <param name="targetTypes">The targetTypes</param>
            /// <param name="parameter">The parameter</param>
            /// <param name="culture">The culture</param>
            /// <returns>Returns the value</returns>
            /// <seealso cref="SegmentFillConverter"/>
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException("Cannot convert back");
            }

            #endregion
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the FillBrush dependency property.
        /// Using a DependencyProperty as the backing store for FillBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FillBrushProperty =
                DependencyProperty.Register("FillBrush", typeof(Brush), typeof(ChartRangeAreaSegment), new UIPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is high low.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is high low; otherwise, <c>false</c>.
        /// </value>
        protected bool IsHighLow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fill brush.
        /// </summary>
        /// <value>The fill brush.</value>
        public Brush FillBrush
        {
            get
            {
                return (Brush)GetValue(FillBrushProperty);
            }

            set
            {
                SetValue(FillBrushProperty, value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartRangeAreaSegment"/> class.
        /// </summary>
        static ChartRangeAreaSegment()
        {
            Type type = typeof(ChartRangeAreaSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRangeAreaSegment"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        /// <param name="isHighLow">The isHighLow.</param>
        internal ChartRangeAreaSegment(IChartDataPoint[] points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series, bool isHighLow)
            : base(points, correspondingPoints, series)
        {
            IsHighLow = isHighLow;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        /// <seealso cref="ChartRangeAreaSegment"/>
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

            Binding binding1 = new Binding();
            binding1.Path = new PropertyPath(ChartRangeAreaType.HighValueInteriorProperty);
            binding1.Source = this.Series;

            Binding binding2 = new Binding();
            binding2.Path = new PropertyPath(ChartRangeAreaType.LowValueInteriorProperty);
            binding2.Source = this.Series;

            Binding binding3 = new Binding("Interior");
            binding3.Source = this.Series;

            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(binding1);
            multiBinding.Bindings.Add(binding2);
            multiBinding.Bindings.Add(binding3);
            multiBinding.Converter = new SegmentFillConverter();
            multiBinding.ConverterParameter = this;
            BindingOperations.SetBinding(this, FillBrushProperty, multiBinding);

            PathFigure figure = new PathFigure();

            int startIndex = 1;
            int endIndex = AreaPoints.Length % 2 == 0 ? AreaPoints.Length - 2 : AreaPoints.Length - 1;
            if (this.Series.Segments.IndexOf(this) == 0 && this.Series.Segments.Count == 1)
            {
                startIndex = 2;
            }

            if (this.Series.Segments.IndexOf(this) == this.Series.Segments.Count - 1 && this.Series.Segments.Count == 1)
            {
                endIndex = AreaPoints.Length - 1;
            }

            if (AreaPoints.Length > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(AreaPoints[0].X, AreaPoints[0].Y);

                for (int i = startIndex; i < AreaPoints.Length; i += 2)
                {
                    figure.Segments.Add(new LineSegment(transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y), true));
                }

                for (int i = endIndex; i >= 1; i -= 2)
                {
                    figure.Segments.Add(new LineSegment(transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y), true));
                }

                figure.IsClosed = true;
            }

            this.Geometry = new PathGeometry(new PathFigure[] { figure });
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = MeshGenerator.AreaSegmentFull(PrepareRangePoints(), 0.02, transformer);

            MaterialGroup materialGroup;

            DiffuseMaterial difuseMaterial = new DiffuseMaterial();
            Binding binding = new Binding("Interior");
            binding.Source = Series;
            BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
            materialGroup = new MaterialGroup();
            materialGroup.Children.Add(difuseMaterial);

            model.Material = materialGroup;
            model.BackMaterial = materialGroup;

            model.Transform = new TranslateTransform3D(-0.5, -0.5, (this.Series.Area.Series.IndexOf(this.Series) + 3) * 0.05);
            Geometry3DGroup.Children.Add(model);
        }

        /// <summary>
        /// Prepares the range points.
        /// </summary>
        /// <returns>Returns the range points</returns>
        private IChartDataPoint[] PrepareRangePoints()
        {
            List<IChartDataPoint> retValue = new List<IChartDataPoint>();

            int startIndex = 1;
            int endIndex = AreaPoints.Length - 2;

            if (this.Series.Segments.IndexOf(this) == 0)
            {
                startIndex = 2;
            }

            if (this.Series.Segments.IndexOf(this) == this.Series.Segments.Count - 1)
            {
                endIndex = AreaPoints.Length - 1;
            }

            if (AreaPoints.Length > 0)
            {
                retValue.Add(AreaPoints[0]);
                for (int i = startIndex; i < AreaPoints.Length; i += 2)
                {
                    retValue.Add(AreaPoints[i]);
                }

                retValue.Add(AreaPoints[AreaPoints.Length - 1]);
                for (int i = endIndex; i >= 1; i -= 2)
                {
                    retValue.Add(AreaPoints[i]);
                }
            }

            return retValue.ToArray();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.seriesCorrespondingPoints != null)
            {
                foreach (var item in this.seriesCorrespondingPoints)
                {
                    if (item.DataPoint is ChartPoint)
                    {
                        (item.DataPoint as ChartPoint).DisposePoint();
                    }
                }
            }
            SetValue(SeriesPropertyKey, null);
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartRangeAreaType class
    /// </summary>
    /// <remarks>
    /// Range Area Chart is a variation of Area Chart type that lets you plot bands of
    /// data in a chart, like Bollinger bands, weather patterns, etc. Each point in the
    /// chart is specified by 2 Y values – the lower and higher end of the band.
    /// </remarks>
    /// <seealso cref="ChartRangeAreaType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartRangeAreaType : ChartAreaType
    {
        #region Attached properties

        /// <summary>
        /// Gets the value of the HighValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObjectobj.</param>
        /// <returns>The HighValueInterior brush</returns>
        public static Brush GetHighValueInterior(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HighValueInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the HighValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetHighValueInterior(DependencyObject obj, Brush value)
        {
            obj.SetValue(HighValueInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the HighValueInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty HighValueInteriorProperty =
                DependencyProperty.RegisterAttached("HighValueInterior", typeof(Brush), typeof(ChartRangeAreaType), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the value of the LowValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>The LowValueInterior Brush</returns>
        public static Brush GetLowValueInterior(DependencyObject obj)
        {
            return (Brush)obj.GetValue(LowValueInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the LowValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetLowValueInterior(DependencyObject obj, Brush value)
        {
            obj.SetValue(LowValueInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the LowValueInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty LowValueInteriorProperty =
                DependencyProperty.RegisterAttached("LowValueInterior", typeof(Brush), typeof(ChartRangeAreaType), new FrameworkPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets the requirement for data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartType.ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRangeAreaType"/> class.
        /// </summary>
        internal ChartRangeAreaType()
        {
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            Point point1;
            Point point2;
            Point point3;
            Point point4;
            Point? crossPoint;
            List<Point> segPoints = new List<Point>();
            if (points.Length == 0)
            {
                return;
            }
            segPoints.Add(new Point(points[0].DataPoint.X, points[0].DataPoint.Values[0]));
            segPoints.Add(new Point(points[0].DataPoint.X, points[0].DataPoint.Values[1]));

            bool isHighLow = (segPoints[0].Y > segPoints[1].Y) ? true : false;
            int i;
            for ( i = 0; i < points.Length -1 ; i++)
            {
                point1 = new Point(points[i].DataPoint.X, points[i].DataPoint.Values[0]);
                point2 = new Point(points[i + 1].DataPoint.X, points[i + 1].DataPoint.Values[0]);
                point3 = new Point(points[i].DataPoint.X, points[i].DataPoint.Values[1]);
                point4 = new Point(points[i + 1].DataPoint.X, points[i + 1].DataPoint.Values[1]);
                crossPoint = ChartRangeAreaType.GetCrossPoint(point1, point2, point3, point4);

                if (crossPoint != null)
                {
                    if (crossPoint.Value.Y == 0 && (points[i + 1].DataPoint.EmptyPoint || points[i].DataPoint.EmptyPoint))
                    {
                    }
                    else
                    {
                        segPoints.Add(crossPoint.Value);
                    }
                    isHighLow = points[i].DataPoint.Values[0] > points[i].DataPoint.Values[1];
                    series.Segments.Add(new ChartRangeAreaSegment(Point2ChartPoint(segPoints), points, series, isHighLow));
                    ////series.Segments.Add(new ChartAreaSegment(Point2ChartPoint(segPoints), points, series));
                    //isHighLow = !isHighLow;
                    segPoints = new List<Point>();
                    if (crossPoint.Value.Y == 0 && (points[i + 1].DataPoint.EmptyPoint || points[i].DataPoint.EmptyPoint))
                    {
                    }
                    else
                    {
                        segPoints.Add(crossPoint.Value);
                    }
                }
                segPoints.Add(point2);
                segPoints.Add(point4);
            }
            isHighLow = points[i].DataPoint.Values[0] > points[i].DataPoint.Values[1];
            series.Segments.Add(new ChartRangeAreaSegment(Point2ChartPoint(segPoints), points, series, isHighLow));
            ////series.Segments.Add(new ChartAreaSegment(Point2ChartPoint(segPoints), points, series));

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                int index = -1;
                for (i = 0; i < points.Length; i++)
                {
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                      //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                            else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                     (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                        else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                 (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                        }
                        else
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The Chart Series</param>
        /// <param name="points">The series points</param>
        /// <seealso cref="ChartRangeAreaType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            if (!series.Area.Series.Contains(null))
            {
                this.CalculateSegments(series, points);
            }
        }

        /// <summary>
        /// Point2s the chart point.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Returns the point array</returns>
        internal IChartDataPoint[] Point2ChartPoint(IEnumerable<Point> points)
        {
            List<IChartDataPoint> cPoints = new List<IChartDataPoint>();

            foreach (Point pt in points)
            {
                cPoints.Add(new ChartPoint(pt.X, pt.Y));
            }

            return cPoints.ToArray();
        }

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12 value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The CrossPoint</returns>
        protected static Point? GetCrossPoint(Point p11, Point p12, Point p21, Point p22)
        {
            Point pt = new Point();
            double z = (p12.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p12.X - p11.X);
            double ca = (p12.Y - p11.Y) * (p21.X - p11.X) - (p21.Y - p11.Y) * (p12.X - p11.X);
            double cb = (p21.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p21.X - p11.X);

            if ((z == 0) && (ca == 0) && (cb == 0))
            {
                return null;
            }

            double ua = ca / z;
            double ub = cb / z;

            pt.X = p11.X + (p12.X - p11.X) * ub;
            pt.Y = p11.Y + (p12.Y - p11.Y) * ub;

            if ((0 <= ua) && (ua <= 1) && (0 <= ub) && (ub <= 1))
            {
                return pt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts ChartAreaType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartRangeAreaType"/>
        public override string ToString()
        {
            return "RangeArea";
        }
        #endregion
    }
}
