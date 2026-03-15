// <copyright file="ChartCandleType.cs" company="Syncfusion">
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
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents price segment that is a part of candle chart type.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartCandleSegment : ChartColumnSegment
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
                Brush lowValue = values[0] as Brush;
                Brush highvalue = values[2] as Brush;
                Brush seriesInterior = values[1] as Brush;
                ChartCandleSegment targetSegment = parameter as ChartCandleSegment;
                if (targetSegment.CorrespondingPoints != null && targetSegment.CorrespondingPoints[0].DataPoint.EmptyPoint)
                {
                    if (targetSegment.Series.ShowEmptyPoints && targetSegment.Series.EmptyPointStyle == EmptyPointStyle.Interior)
                    {
                        return targetSegment.Series.EmptyPointInterior;
                    }
                }
                if (!targetSegment.IsBullValue)
                {
                   
                    return (lowValue != null) ? lowValue : seriesInterior;
                }
                else
                {
                    return (highvalue != null)? highvalue :  seriesInterior;
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

        #region dependency properties
        /// <summary>
        /// Identifies the HiX dependency property.
        /// </summary>
        public static readonly DependencyProperty HiXProperty =
            DependencyProperty.Register("HighX", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the HiY dependency property.
        /// </summary>
        public static readonly DependencyProperty HiYProperty =
            DependencyProperty.Register("HighY", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LoX dependency property.
        /// </summary>
        public static readonly DependencyProperty LoXProperty =
            DependencyProperty.Register("LowX", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LoY dependency property.
        /// </summary>
        public static readonly DependencyProperty LoYProperty =
            DependencyProperty.Register("LowY", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the FillBrush dependency property.
        /// Using a DependencyProperty as the backing store for FillBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FillBrushProperty =
                DependencyProperty.Register("FillBrush", typeof(Brush), typeof(ChartCandleSegment), new UIPropertyMetadata(null));
        
        #endregion

        #region Properties
        /// <summary>
        /// Get and Set IsBullValue
        /// </summary>
        protected bool IsBullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Hi X value
        /// </summary>
        /// <value>The hi X value.</value>
        public double HiX
        {
            get { return (double)GetValue(HiXProperty); }
            set { SetValue(HiXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the hi Y value.
        /// </summary>
        /// <value>The hi Y value.</value>
        public double HiY
        {
            get { return (double)GetValue(HiYProperty); }
            set { SetValue(HiYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low X.
        /// </summary>
        /// <value>The lo X value.</value>
        public double LoX
        {
            get { return (double)GetValue(LoXProperty); }
            set { SetValue(LoXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low Y.
        /// </summary>
        /// <value>The lo Y value.</value>
        public double LoY
        {
            get { return (double)GetValue(LoYProperty); }
            set { SetValue(LoYProperty, value); }
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

        #region Members
        /// <summary>
        /// Initializes m_point1
        /// </summary>
        private IChartDataPoint m_point1;

        /// <summary>
        /// Initializes m_point2
        /// </summary>
        private IChartDataPoint m_point2;

        internal bool m_isBull;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartCandleSegment"/> class.
        /// </summary>
        static ChartCandleSegment()
        {
            Type type = typeof(ChartCandleSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartCandleSegment">ChartCandleSegment</see> class. 
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT</param>
        /// <param name="topRightPnt">The top right PNT</param>
        /// <param name="hipt">The hi pt.</param>
        /// <param name="lopt">The lo pt.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        /// <param name="isbullValue"></param>
        /// <remarks></remarks>
        internal ChartCandleSegment(IChartDataPoint bottomLeftPnt, IChartDataPoint topRightPnt, IChartDataPoint hipt, IChartDataPoint lopt, ChartIndexedDataPoint correspondingPoint, ChartSeries series, bool isbullValue)
            : base(bottomLeftPnt, topRightPnt, correspondingPoint, series)
        {
            m_point1 = hipt;
            m_point2 = lopt;
          //  m_point1.Values = bottomLeftPnt.Values;
            this.IsBullValue = this.m_isBull = isbullValue;
            if (correspondingPoint.DataPoint.EmptyPoint)
            {
                if (series.ShowEmptyPoints && series.EmptyPointStyle == EmptyPointStyle.Interior)
                {
                   // this.SetCurrentValue( = series.EmptyPointInterior;
                }
            }
            this.SetXRange(bottomLeftPnt.X, topRightPnt.X, hipt.X, lopt.X);
            this.SetYRange(bottomLeftPnt.Y, topRightPnt.Y, hipt.Y, lopt.Y);
            if (this.Series.Area.EnableDepthAxis && bottomLeftPnt.Values.Length > 4)
                this.SetZRange(bottomLeftPnt.Values[4]);
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartCandleSegment"/>
        public override void Update(IChartTransformer transformer)
        {
            if (this.Interior != null && this.Stroke != null)
            {
                if (this.Interior.CanFreeze)
                {
                    this.Interior.Freeze();
                }
                if (this.Stroke.CanFreeze)
                {
                    this.Stroke.Freeze();
                }
            }

          
            Binding binding2 = new Binding();
            binding2.Path = new PropertyPath(ChartCandleType.BearFillColorProperty);
            binding2.Source = this.Series;

            Binding binding4 = new Binding();
            binding4.Path = new PropertyPath(ChartCandleType.BullFillColorProperty);
            binding4.Source = this.Series;

            Binding binding3 = new Binding("Interior");
            binding3.Source = this.Series;

            MultiBinding multiBinding = new MultiBinding();          
            multiBinding.Bindings.Add(binding2);
            multiBinding.Bindings.Add(binding3);
            multiBinding.Bindings.Add(binding4);
            multiBinding.Converter = new SegmentFillConverter();
            multiBinding.ConverterParameter = this;
            bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
            if(!colorEachValue)
            BindingOperations.SetBinding(this, FillBrushProperty, multiBinding);

            base.Update(transformer);

            Point point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y);
            Point point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y);

            this.HiX = point1.X;
            this.LoX = point2.X;
            this.HiY = point1.Y;
            this.LoY = point2.Y;
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            base.Draw3DSegment(transformer);
            DoubleRange axisRange = Series.XAxis.VisibleRange;
            ////Skipping the segment if it falls out of axis' range.
            if (xRange.Start >= axisRange.Start && xRange.End <= axisRange.End)
            {
                Point3D blPoint = transformer.TransformToVisible(m_point1.X, m_point1.Y,this.Series.Area.EnableDepthAxis && m_point1.Values.Length > 4 ? m_point1.Values[4] : 0.2);
                Point3D trPoint = transformer.TransformToVisible(m_point2.X, m_point2.Y, this.Series.Area.EnableDepthAxis && m_point1.Values.Length > 4 ? m_point1.Values[4] : 0.2);
                Rect columnRect = new Rect(new Point(blPoint.X, blPoint.Y), new Point(trPoint.X,trPoint.Y));

                GeometryModel3D geometryModel3D = new GeometryModel3D()
                {
                    Geometry = MeshGenerator.Cylinder((double)Series.Segments.Count / 1000, columnRect.Height, 25)
                };
                MaterialGroup materialGroup;
                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior") { Source = Series };
                bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
                if (!colorEachValue)
                    BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                else
                    this.Series.UpdateColorEachSegments(this.Series, this, this.Series.Segments.IndexOf(this), difuseMaterial);
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);

                geometryModel3D.Material = materialGroup;
                geometryModel3D.Transform = new TranslateTransform3D(columnRect.X + columnRect.Width / 2 - 0.5, 0.5 - columnRect.Y - columnRect.Height / 2, !this.Series.Area.IsClustered ? 0.06 : blPoint.Z);
                Geometry3DGroup.Children.Add(geometryModel3D);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
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
            base.Dispose();
        }
        #endregion
    }

    /// <summary>
    /// Represents candle chart type.
    /// </summary>
    /// <seealso>
    ///     <cref>ChartCandleeSegment</cref>
    /// </seealso>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartCandleType : ChartType
    {
        #region Properties

        /// <summary>
        /// Gets the required data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Attached properties
      
        /// <summary>
        /// Gets the value of the BearFillColor dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>The LowValueInterior Brush</returns>
        public static Brush GetBearFillColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BearFillColorProperty);
        }

        /// <summary>
        /// Sets the value of the BearFillColor dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetBearFillColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(BearFillColorProperty, value);
        }

        /// <summary>
        /// Indicates the BearFillColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty BearFillColorProperty =
                DependencyProperty.RegisterAttached("BearFillColor", typeof(Brush), typeof(ChartCandleType), new FrameworkPropertyMetadata(Brushes.Firebrick));


        /// <summary>
        /// Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetBullFillColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BullFillColorProperty);
        }

        /// <summary>
        /// Sets the value of the BearFillColor dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetBullFillColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(BullFillColorProperty, value);
        }

        /// <summary>
        /// Indicates the BearFillColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty BullFillColorProperty =
                DependencyProperty.RegisterAttached("BullFillColor", typeof(Brush), typeof(ChartCandleType), new FrameworkPropertyMetadata(Brushes.GreenYellow));
        #endregion

        #region Implementation
        /// <summary>
        /// Converts ChartCandleType to string 
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartCandleType"/>
        public override string ToString()
        {
            return "Candle";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            ////double origin0 = series.ActualYAxis.Origin;
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            double center = sbsInfo.Median;

            for (int i = 0; i < points.Length; i++)
            {
                double x1 = points[i].DataPoint.X + sbsInfo.Start;
                double x2 = points[i].DataPoint.X + sbsInfo.End;
                double y1 = points[i].DataPoint.Values[1];
                double y2 = points[i].DataPoint.Values[2];
                bool isbull = false;
                if (y1 < y2)
                {
                    isbull = true;
                }
                else
                {
                    isbull = false;
                }

                ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                ChartPoint cdpRightTop = new ChartPoint(x2, y2);

                ChartPoint hipoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[0]);
                ChartPoint lopoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[3]);
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointValue == EmptyPointValue.Zero)
                        {
                            hipoint = new ChartPoint(points[i].DataPoint.X + center, y1);
                            lopoint = new ChartPoint(points[i].DataPoint.X + center, y2);
                        }
                        //if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        //{ }
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else
                        {
                            ChartCandleSegment seg = new ChartCandleSegment(cdpBottomLeft, cdpRightTop, hipoint, lopoint, points[i], series, isbull);
                            seg.Interior = series.EmptyPointInterior;
                            series.Segments.Add(seg);
                            
                        }
                        //series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                    }

                }
                else
                {
                    series.Segments.Add(new ChartCandleSegment(cdpBottomLeft, cdpRightTop, hipoint, lopoint, points[i], series, isbull));
                }
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                int index = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    double y3 = points[i].DataPoint.Values[2];
                    double y4 = points[i].DataPoint.Values[3];
                    //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y3, y3, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y4, y4, y1, y2, points[i], ++index));
                            }
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            {
                                if (y1 > y2)
                                    series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                                if (y3 > y4)
                                    series.Adornments.Add(this.CreateAdornment(series, y3, y3, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, y4, y4, y1, y2, points[i], ++index));
                            }
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            {
                                if (y1 < y2)
                                    series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                                if (y3 < y4)
                                    series.Adornments.Add(this.CreateAdornment(series, y3, y3, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, y4, y4, y1, y2, points[i], ++index));
                            }
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y3, y3, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y4, y4, y1, y2, points[i], ++index));
                        }
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                        {
                            if (y1 > y2)
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            if (y3 > y4)
                                series.Adornments.Add(this.CreateAdornment(series, y3, y3, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, y4, y4, y1, y2, points[i], ++index));
                        }
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                        {
                            if (y1 < y2)
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            if (y3 < y4)
                                series.Adornments.Add(this.CreateAdornment(series, y3, y3, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, y4, y4, y1, y2, points[i], ++index));
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
        /// <seealso cref="ChartCandleType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            this.CalculateSegments(series, points);
        }

        #endregion
    }
}
