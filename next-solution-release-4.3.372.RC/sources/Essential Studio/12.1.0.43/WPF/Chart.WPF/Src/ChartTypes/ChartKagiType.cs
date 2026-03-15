// <copyright file="ChartKagiType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents Kagi chart segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartKagiType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartKagiSegment : ChartSegment
    {
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
                ChartKagiSegment targetSegment = parameter as ChartKagiSegment;
                if (targetSegment.IsPriceDown)
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

        #region Members
        /// <summary>
        /// Initializes m_points
        /// </summary>
        private IChartDataPoint[] m_points;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the IsPriceUp dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceUpProperty =
            DependencyProperty.Register("IsPriceUp", typeof(bool), typeof(ChartKagiSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsPriceDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceDownProperty =
            DependencyProperty.Register("IsPriceDown", typeof(bool), typeof(ChartKagiSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartKagiSegment), new PropertyMetadata(new PointCollection()));

        /// <summary>
        /// Identifies the FillBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty FillBrushProperty =
             DependencyProperty.Register("FillBrush", typeof(Brush), typeof(ChartKagiSegment), new UIPropertyMetadata(null));

        /// <summary>
        /// Get and Set FillBrush
        /// </summary>
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

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this segment is price up. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is price up; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceUp
        {
            get { return (bool)GetValue(IsPriceUpProperty); }
            set { SetValue(IsPriceUpProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment is price down. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is price down; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceDown
        {
            get { return (bool)GetValue(IsPriceDownProperty); }
            set { SetValue(IsPriceDownProperty, value); }
        }

        /// <summary>
        /// Gets or sets the points. This is a dependency property.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartKagiSegment"/> class.
        /// </summary>
        /// <remarks>
        /// During initialization the default template for Kagi segment is being created.
        /// </remarks>
        static ChartKagiSegment()
        {
            Type type = typeof(ChartKagiSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartKagiSegment"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartKagiSegment(IChartDataPoint[] points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            m_points = points;

            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            for (int i = 0; i < m_points.Length; i++)
            {
                xRange += points[i].X;
                yRange += points[i].Y;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The Transformer</param>
        ///  <seealso cref="ChartKagiSegment"/>
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
            Binding binding1 = new Binding();
            binding1.Path = new PropertyPath(ChartKagiType.PriceUpInteriorProperty);
            binding1.Source = this.Series;

            Binding binding2 = new Binding();
            binding2.Path = new PropertyPath(ChartKagiType.PriceDownInteriorProperty);
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


            bool shouldReassignPointCollection = false;
            PointCollection points = new PointCollection(m_points.Length);
            for (int i = 0; i < m_points.Length; i++)
            {
                Point point = transformer.TransformToVisible(m_points[i].X, m_points[i].Y);
                if (!shouldReassignPointCollection && !this.Points.Contains(point))
                {
                    shouldReassignPointCollection = true;
                }

                points.Add(point);
            }

            if (shouldReassignPointCollection)
            {
                this.Points = points;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartKagiType class
    /// </summary>
    /// <remarks>
    /// Kagi Charts are a Japanese invention and date since the late 1870's, but were
    /// popularized in the western world by Steven Nison. They contain a series of
    /// connecting vertical lines where the thickness and direction of those lines
    /// depend on price. If closing prices continue to move in the direction of the
    /// prior vertical Kagi line, then that line is extended. However, if the closing
    /// price reverses by a pre-determined &quot;reversal&quot; amount, a new Kagi line
    /// is drawn in the next column in the opposite direction.
    /// </remarks>
    /// <seealso cref="ChartKagiSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartKagiType : ChartType
    {
        #region Attached properties

        /// <summary>
        /// Gets the value of the PriceUpInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObjectobj.</param>
        /// <returns>The PriceUpInterior brush</returns>
        public static Brush GetPriceUpInterior(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PriceUpInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the PriceUpInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetPriceUpInterior(DependencyObject obj, Brush value)
        {
            obj.SetValue(PriceUpInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the PriceUpInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty PriceUpInteriorProperty =
                DependencyProperty.RegisterAttached("PriceUpInterior", typeof(Brush), typeof(ChartKagiType), new FrameworkPropertyMetadata(Brushes.Red));

        /// <summary>
        /// Gets the value of the PriceDownInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>The LowValueInterior Brush</returns>
        public static Brush GetPriceDownInterior(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PriceDownInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the PriceDownInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetPriceDownInterior(DependencyObject obj, Brush value)
        {
            obj.SetValue(PriceDownInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the PriceDownInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty PriceDownInteriorProperty =
                DependencyProperty.RegisterAttached("PriceDownInterior", typeof(Brush), typeof(ChartKagiType), new FrameworkPropertyMetadata(Brushes.Green));
       
       
        /// <summary>
        /// Identifies the ReversalAmount dependency property.
        /// </summary>
        public static readonly DependencyProperty ReversalAmountProperty =
          DependencyProperty.RegisterAttached("ReversalAmount", typeof(double), typeof(ChartKagiType), new ChartPropertyMetadata(1d, new PropertyChangedCallback(OnReversalAmountChanged), ChartPropertyMetadataOptions.AffectsUpdate));
        #endregion        

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartKagiType"/> class.
        /// </summary>
        internal ChartKagiType()
        {
        }

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

        #region Public methods
        /// <summary>
        /// Called when ReversalAmount property changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnReversalAmountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries ser = obj as ChartSeries;
            if (ser != null)
            {
                if (ser.ChartType is ChartKagiType)
                {
                    ((ChartKagiType)(ser.ChartType)).Update(ser);
                }
            }
        }

        /// <summary>
        /// Gets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Reversal Amount</returns>
        public static double GetReversalAmount(ChartSeries series)
        {
            return (double)series.GetValue(ReversalAmountProperty);
        }

        /// <summary>
        /// Sets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetReversalAmount(ChartSeries series, double value)
        {
            series.SetValue(ReversalAmountProperty, value);
        }

        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Calculate(ChartSeries series)
        {
            double reversalAmount = GetReversalAmount(series);
            int count = series.PointsCount;
            List<ChartSegment> drawingList = new List<ChartSegment>(count);
            ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
            }

            Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

            List<ChartIndexedDataPoint> correspondingPoints = new List<ChartIndexedDataPoint>();
            List<IChartDataPoint> points = new List<IChartDataPoint>();
            List<IChartDataPoint> adornments = new List<IChartDataPoint>();
            bool isNegative = true;
            bool isGoingDown = true;
            double prevY = cdpwiA[0].DataPoint.Y;
            double prevX = cdpwiA[0].DataPoint.X;
            double breakMinY = prevY;
            double breakMaxY = prevY;
            double currX = cdpwiA[0].DataPoint.X;
            correspondingPoints.Add(cdpwiA[0]);
            points.Add(new ChartPoint(currX, prevY));
            adornments.Add(new ChartPoint(currX, prevY));
            int index =0;
            for (int i = 1; i < cdpwiA.Length; i++)
            {
                correspondingPoints.Add(cdpwiA[i]);
                double currY = cdpwiA[i].DataPoint.Y;

                if (i == 1)
                {
                    isNegative = currY < prevY;
                    isGoingDown = currY < prevY;
                } 

                if (Math.Abs(currY - prevY) > reversalAmount)
                {
                    if ((isGoingDown && prevY < currY) || (!isGoingDown && prevY > currY))
                    {
                        currX = cdpwiA[i].DataPoint.X;
                        points.Add(new ChartPoint(currX, prevY));                       
                        breakMinY = currY > prevY ? prevY : breakMinY;
                        breakMaxY = currY < prevY ? prevY : breakMaxY;
                        isGoingDown = !isGoingDown;
                    }

                    if ((isNegative && (currY > breakMaxY)) || (!isNegative && (currY < breakMinY)))
                    {
                        IChartDataPoint icdp = new ChartPoint(currX, isNegative ? breakMaxY : breakMinY);
                        points.Add(icdp);

                        ////adding CIDs data to lists:
                        ChartKagiSegment segment = new ChartKagiSegment(points.ToArray(), correspondingPoints.ToArray(), series);
                        drawingList.Add(segment);
                        if (series.AdornmentsInfo.Visible)
                        {
                            index = 0;

                            for (int j = 0; j < adornments.Count; j++)
                            {
                                ChartIndexedDataPoint pt = new ChartIndexedDataPoint(adornments[j], index);
                                series.Adornments.Add(this.CreateAdornment(series,pt, index));
                                index++;
                            }
                        }
                        segment.IsPriceDown = isNegative;
                        segment.IsPriceUp = !isNegative;

                        points = new List<IChartDataPoint>();
                        adornments = new List<IChartDataPoint>();
                        correspondingPoints = new List<ChartIndexedDataPoint>();
                        points.Add(new ChartPoint(currX, isNegative ? breakMaxY : breakMinY));                        
                        correspondingPoints.Add(cdpwiA[i]);
                        isNegative = !isNegative;
                    }

                    points.Add(new ChartPoint(currX, currY));
                    adornments.Add(new ChartPoint(currX, currY));                    
                    prevY = currY;
                }
                else
                {
                    if ((isGoingDown && prevY < currY) || (!isGoingDown && prevY > currY))
                    {
                        currX = cdpwiA[i].DataPoint.X;
                        points.Add(new ChartPoint(currX, prevY));                       
                        breakMinY = currY > prevY ? prevY : breakMinY;
                        breakMaxY = currY < prevY ? prevY : breakMaxY;
                        isGoingDown = !isGoingDown;
                    }
                    {
                        points.Add(new ChartPoint(currX, currY));                        
                        prevY = currY;
                        breakMinY = prevY;
                        breakMaxY = prevY;
                    }
                }

                if (i == cdpwiA.Length - 1)
                {
                    ////adding CIDs data to lists:
                    ChartKagiSegment segment = new ChartKagiSegment(points.ToArray(), correspondingPoints.ToArray(), series);

                    segment.IsPriceDown = isNegative;
                    segment.IsPriceUp = !isNegative;

                    drawingList.Add(segment);

                    if (series.AdornmentsInfo.Visible)
                    {
                        index = 0;

                        for (int j = 0; j < correspondingPoints.Count; j++)
                        {
                             //Add for SD11357
                            if (correspondingPoints[j].DataPoint.EmptyPoint)
                            {
                                if (series.ShowEmptyPoints)
                                    series.Adornments.Add(this.CreateAdornment(series, correspondingPoints[j], index));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, correspondingPoints[j], index));
                                index++;
                            }
                        }
                    }
                }
            }            
            foreach (ChartSegment segment in drawingList)
            {
                //if (segment.CorrespondingPoints[0].DataPoint.EmptyPoint)
                //{
                //    if(segment.Series.ShowEmptyPoints)
                //    {
                //        series.Segments.Add(segment);
                //    }
                //}
                //else
                //{
                    series.Segments.Add(segment);
                //}
            }
        }

        /// <summary>
        /// Updates chart series.
        /// </summary>
        /// <param name="series">The series</param>
        /// <seealso cref="ChartKagiType"/>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartKagiType"/>
        public override string ToString()
        {
            return "Kagi";
        }
    }
}
