#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class TrendlineBase : Control
    {

        #region Properties

        public double Slope { get; private set; }

        public double Intercept { get; private set; }

        internal ChartSeries Series
        {
            get;
            set;
        }

        public double[] PolynomialSlopes { get; private set; }
        
        internal ChartTrendlinePanel TrendlinePanel { get; set; }

        internal ObservableCollection<ChartSegment> TrendlineSegments { get; set; }

        private IList<double> _xValues;

        private double _xMin;

        private double _xMax;

       
        private IList<double> _trendXValues;
        private IList<double> _trendYValues;

        private List<double> _trendXSegmentValues;
        private List<double> _trendYSegmentValues;
       
        #endregion

        #region ctor
        public TrendlineBase()
        {
            DefaultStyleKey = typeof(TrendlineBase);
            TrendlineSegments = new ObservableCollection<ChartSegment>();
        }
        #endregion

        #region Dependency Prroperties


        /// <summary>
        /// Gets or Sets visibility for trendline.
        /// </summary>
        public bool IsTrendlineVisible
        {
            get { return (bool)GetValue(IsTrendlineVisibleProperty); }
            set { SetValue(IsTrendlineVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTrendlineVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTrendlineVisibleProperty =
            DependencyProperty.Register("IsTrendlineVisible", typeof(bool), typeof(TrendlineBase), new PropertyMetadata(true, OnIsTrendlineVisibleChanged));

        private static void OnIsTrendlineVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as TrendlineBase;
            if (instance != null && instance.Series != null)
                instance.IsTrendlineVisibleChanged(args);
        }

        private void IsTrendlineVisibleChanged(DependencyPropertyChangedEventArgs args)
        {
            var series = Series as CartesianSeries;
            if (series != null)
            {
                if ((bool)args.NewValue)
                   Visibility = Visibility.Visible;
                else
                   Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or Sets a value that determines whether to create a legend item for this trendline. By default, legend item will be visible for this trendline.
        /// </summary>
        public Visibility VisibilityOnLegend
        {
            get { return (Visibility)GetValue(VisibilityOnLegendProperty); }
            set { SetValue(VisibilityOnLegendProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibilityOnLegend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilityOnLegendProperty =
            DependencyProperty.Register("VisibilityOnLegend", typeof(Visibility), typeof(TrendlineBase), new PropertyMetadata(Visibility.Visible));

        // <summary>
        /// Gets or Sets DataTemplate for legend icon.
        /// </summary>
        public DataTemplate LegendIconTemplate
        {
            get { return (DataTemplate)GetValue(LegendIconTemplateProperty); }
            set { SetValue(LegendIconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconTemplateProperty =
            DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(TrendlineBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets ChartLegendIcon to be displayed in associated legend item.
        /// </summary>
        public ChartLegendIcon LegendIcon
        {
            get { return (ChartLegendIcon)GetValue(LegendIconProperty); }
            set { SetValue(LegendIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconProperty =
            DependencyProperty.Register("LegendIcon", typeof(ChartLegendIcon), typeof(TrendlineBase), new PropertyMetadata(ChartLegendIcon.SeriesType, OnLegendIconChanged));

        private static void OnLegendIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (instance != null && instance.Series != null)
                instance.UpdateLegendIconTemplate(true);
        }

        /// <summary>
        /// Gets or Sets the label that will be displayed in the associated legend item.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TrendlineBase), new PropertyMetadata(string.Empty));


        /// <summary>
        /// Gets or Sets the type of the trendline.
        /// </summary>
        public TrendlineType Type
        {
            get { return (TrendlineType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(TrendlineType), typeof(TrendlineBase), new PropertyMetadata(TrendlineType.Linear, OnTypeChanged));

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (instance != null && instance.Series != null && e.NewValue != null)
            {
                instance.Series.ActualArea.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Gets or Sets the brush to paint the stroke of the trendline.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(TrendlineBase), new PropertyMetadata(new SolidColorBrush(Colors.Blue), OnStrokeChanged));
        
        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as Trendline;
            if (e.NewValue != null && instance != null)
                foreach (var segment in instance.TrendlineSegments)
                    segment.Interior = (Brush)e.NewValue;
        }

        /// <summary>
        /// Gets or Sets the StrokeThickness of the trendline.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(TrendlineBase), new PropertyMetadata(2d, OnStrokeThicknessChanged));

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as Trendline;
            if (e.NewValue != null)
                foreach (ChartSegment segment in instance.TrendlineSegments)
                {
                    segment.StrokeThickness = (double)e.NewValue;
                }
        }

        /// <summary>
        /// Gets or sets a collection of Double values that indicates the pattern of
        /// dashes and gaps that is used to outline shapes.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(TrendlineBase), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (e.NewValue != null)
            {
                foreach (var segment in instance.TrendlineSegments)
                {
                    var collection = (DoubleCollection)e.NewValue;
                    if (collection != null && collection.Count > 0)
                    {
                        var doubleCollection = new DoubleCollection();
                        foreach (var value in collection)
                        {
                            doubleCollection.Add(value);
                        }
                        if (segment is SplineSegment)
                            (segment as SplineSegment).StrokeDashArray = doubleCollection;
                        else if (segment is LineSegment)
                            (segment as LineSegment).StrokeDashArray = doubleCollection;
                    }
                }
            }
        }

        // <summary>
        /// Gets or Sets the ForwardForecast of the trendline.
        /// </summary>
        public double ForwardForecast
        {
            get { return (double)GetValue(ForwardForecastProperty); }
            set { SetValue(ForwardForecastProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForwardForecast.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForwardForecastProperty =
            DependencyProperty.Register("ForwardForecast", typeof(double), typeof(TrendlineBase), new PropertyMetadata(0d, OnTypeChanged));
        
        /// <summary>
        /// Gets or Sets the BackwardForecast of the trendline.
        ///</summary>
        public double BackwardForecast
        {
            get { return (double)GetValue(BackwardForecastProperty); }
            set { SetValue(BackwardForecastProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackwardForecast.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackwardForecastProperty =
            DependencyProperty.Register("BackwardForecast", typeof(double), typeof(TrendlineBase), new PropertyMetadata(0d, OnTypeChanged));

        /// <summary>
        /// Gets or Sets the Polynomial Order of the trendline , it caluclate the order based equation and we are able to given order as 2 to 6 only.
        /// </summary>
        public int PolynomialOrder
        {
            get { return (int)GetValue(PolynomialOrderProperty); }
            set { SetValue(PolynomialOrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PolynomialOrder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PolynomialOrderProperty =
            DependencyProperty.Register("PolynomialOrder", typeof(int), typeof(TrendlineBase), new PropertyMetadata(2, OnPolynomialOrderChanged));

        private static void OnPolynomialOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (instance != null && instance.Series != null)
            {
                if (e.NewValue != null  && instance.Type == TrendlineType.Polynomial && (instance.Series.ActualData.Count > 2 && instance.Series.ActualData.Count > (int)e.NewValue))
                {
                    instance.Series.ActualArea.ScheduleUpdate();
                }
            }
        }

        #endregion

        #region Methods

        #region Major Methods

#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
    #else
        /// <summary>
        /// Invoke to render trendline.
        /// </summary>
        protected override void OnApplyTemplate()
    #endif
        {
            base.OnApplyTemplate();
            TrendlinePanel = this.GetTemplateChild("trendlinePanel") as ChartTrendlinePanel;
            if (TrendlinePanel != null)
            {
                TrendlinePanel.Trend = this;
                foreach (var segment in TrendlineSegments)
                {
                    if (!segment.IsAddedToVisualTree)
                    {
                        UIElement element = segment.CreateVisual(Size.Empty);
                        if (element != null)
                        {
                            TrendlinePanel.Children.Add(element);
                            segment.IsAddedToVisualTree = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update Legend Ion Template method
        /// </summary>
        /// <param name="iconChanged"></param>
        internal void UpdateLegendIconTemplate(bool iconChanged)
        {
            var legendIcon = LegendIcon.ToString();

            if (LegendIcon == ChartLegendIcon.SeriesType)
                legendIcon = this.Type.ToString().Replace("Trend", "");

            if ((LegendIconTemplate == null || iconChanged)
#if WINDOWS_PHONE
                && ChartDictionaries.GenericLegendDictionary.Contains(legendIcon))
#else
 && ChartDictionaries.GenericLegendDictionary.Keys.Contains(legendIcon))
#endif
            {
                LegendIconTemplate = ChartDictionaries.GenericLegendDictionary[legendIcon] as DataTemplate;
            }
        }

        /// <summary>
        /// Update Trendline elements method
        /// </summary>
        internal void UpdateElements()
        {
            _xValues = Series.GetXValues();
               
            TrendlineSegments.Clear();

            if (_xValues.Count > 2)
            {
                if ((Series.ActualXAxis is DateTimeAxis) && (Series.ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Auto)
                    (Series.ActualXAxis as DateTimeAxis).ActualRangeChanged += Trendline_ActualRangeChanged;
                else if (Series.ActualXAxis is DateTimeAxis) 
                    (Series.ActualXAxis as DateTimeAxis).ActualRangeChanged -= Trendline_ActualRangeChanged;
                _xMin = _xValues.Min();
                _xMax = _xValues.Max();

                //Check Trend line type to Calculate
                CheckTrendlineType(true);
            }
            UpdateLegendIconTemplate(true);
        }

        void CheckTrendlineType(bool update)
        {
            switch (Type)
            {
                case TrendlineType.Linear:
                    UpdateTrendSource();
                    CalculateLinearTrendline();
                    break;
                case TrendlineType.Exponential:
                    UpdateExponentialTrendSource();
                    CalculateExponentialTrendline();
                    break;
                case TrendlineType.Power:
                    UpdatePowerTrendSource();
                    CalculatePowerTrendline();
                    break;
                case TrendlineType.Logarithmic:
                    UpdateLogarithmicTrendSource();
                    CalculateLogarithmicTrendline();
                    break;
                case TrendlineType.Polynomial:
                    if (Series.ActualData.Count > PolynomialOrder && PolynomialOrder > 1 && PolynomialOrder <= 6)
                    {
                        UpdatePolynomialTrendSource();
                        CalculatePolynomialTrendLine();
                    }
                    break;
            }
        }

        #endregion

        #region Polynomial Trendline

        /// <summary>
        /// Update Polynomial trend source
        /// </summary>
        private void UpdatePolynomialTrendSource()
        {
            _trendXValues = new List<double>();
            if (Series is FinancialSeriesBase)
                _trendYValues = (Series as FinancialSeriesBase).CloseValues;
            else if (Series is RangeSeriesBase)
                _trendYValues = (Series as RangeSeriesBase).LowValues;
            else
                _trendYValues = (Series as XyDataSeries).YValues;
            for (int i = 0; i < Series.DataCount; i++)
            {
                _trendXValues.Add(i + 1);
            }
        }

        /// <summary>
        /// Calculate Gauss Jordan Eliminiation value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool GaussJordanEliminiation(double[,] a, double[] b)
        {
            var length = a.GetLength(0);
            var numArray1 = new int[length];
            var numArray2 = new int[length];
            var numArray3 = new int[length];
            for (var index = 0; index < length; ++index)
                numArray3[index] = 0;
            for (var index1 = 0; index1 < length; ++index1)
            {
                var num1 = 0.0;
                var index2 = 0;
                var index3 = 0;
                for (var index4 = 0; index4 < length; ++index4)
                {
                    if (numArray3[index4] != 1)
                    {
                        for (var index5 = 0; index5 < length; ++index5)
                        {
                            if (numArray3[index5] == 0 && Math.Abs(a[index4, index5]) >= num1)
                            {
                                num1 = Math.Abs(a[index4, index5]);
                                index2 = index4;
                                index3 = index5;
                            }
                        }
                    }
                }
                ++numArray3[index3];
                if (index2 != index3)
                {
                    for (var index4 = 0; index4 < length; ++index4)
                    {
                        var num2 = a[index2, index4];
                        a[index2, index4] = a[index3, index4];
                        a[index3, index4] = num2;
                    }
                    var num3 = b[index2];
                    b[index2] = b[index3];
                    b[index3] = num3;
                }
                numArray2[index1] = index2;
                numArray1[index1] = index3;
                if (a[index3, index3] == 0.0)
                    return false;
                double num4 = 1.0 / a[index3, index3];
                a[index3, index3] = 1.0;
               
                for (var index4 = 0; index4 < length; ++index4)
                    a[index3, index4] *= num4;
                
                b[index3] *= num4;
                
                for (var index4 = 0; index4 < length; ++index4)
                {
                    if (index4 != index3)
                    {
                        var num2 = a[index4, index3];
                        a[index4, index3] = 0.0;
                        for (var index5 = 0; index5 < length; ++index5)
                            a[index4, index5] -= a[index3, index5] * num2;
                        b[index4] -= b[index3] * num2;
                    }
                }
            }
            for (var index1 = length - 1; index1 >= 0; --index1)
            {
                if (numArray2[index1] != numArray1[index1])
                {
                    for (var index2 = 0; index2 < length; ++index2)
                    {
                        var num = a[index2, numArray2[index1]];
                        a[index2, numArray2[index1]] = a[index2, numArray1[index1]];
                        a[index2, numArray1[index1]] = num;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculate Polynomial Trendline with order
        /// </summary>
        private void CalculatePolynomialTrendLine()
        {
            var power = PolynomialOrder;

            // Calculate sum of y datapoints 1 X power matrix

            PolynomialSlopes = new double[power + 1];
            for (var index1 = 0; index1 < Series.DataCount; index1++)
            {
                var num2 = _trendXValues[index1];
                var yval = _trendYValues[index1];
                if (!double.IsNaN(num2) && !double.IsNaN(yval))
                {
                    for (var index2 = 0; index2 <= power; ++index2)
                        PolynomialSlopes[index2] += Math.Pow(num2, index2) * yval;
                }
            }

            // Calculate sum matrix of x datapoints

            var numArray = new double[1 + 2 * power];
            var matrixOfA = new double[power + 1, power + 1];
            var num1 = 0;
            for (var index1 = 0; index1 < _trendXValues.Count; ++index1)
            {
                var num2 = 1.0;
                var d = _trendXValues[index1];
                if (!double.IsNaN(d) && !double.IsNaN(_trendYValues[index1]))
                {
                    for (var index2 = 0; index2 < numArray.Length; ++index2)
                    {
                        numArray[index2] += num2;
                        num2 *= d;
                        ++num1;
                    }
                }
            }

            for (var index1 = 0; index1 <= power; ++index1)
            {
                for (var index2 = 0; index2 <= power; ++index2)
                    matrixOfA[index1, index2] = numArray[index1 + index2];
            }


            //Calculation Gauss jordan eliminiation value of a and b matrix
            if (!GaussJordanEliminiation(matrixOfA, PolynomialSlopes))
                PolynomialSlopes = null;

            //Create segments methods
            CreatePolynomialSegments();

        }

        /// <summary>
        /// Create the polynomial segments
        /// </summary>
        private void CreatePolynomialSegments()
        {
            _trendXSegmentValues = new List<double>();
            _trendYSegmentValues = new List<double>();

            double count = _trendXValues.Count;
            double x = 1;
            if (PolynomialSlopes != null)
            {
                for (var i = 1; i <= PolynomialSlopes.Length; i++)
                {
                    var axis = Series.ActualXAxis;
                   
                    if (i == 1)
                    {

                        if (Series.ActualXAxis is CategoryAxis)
                            _trendXSegmentValues.Add((i - 1) - BackwardForecast);
                        else
                        {
                            if (axis is DateTimeAxis)
                            {
                                double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast, (axis as DateTimeAxis).IntervalType);
                                _trendXSegmentValues.Add(foreCastStartValue);
                            }
                            else
                                _trendXSegmentValues.Add(_xMin - BackwardForecast);
                        }
                        _trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, i - BackwardForecast));


                    }
                    else if (i == PolynomialSlopes.Length)
                    {
                        if (Series.ActualXAxis is CategoryAxis)
                            _trendXSegmentValues.Add((double)(count - 1) + ForwardForecast);
                        else
                        {
                            if (axis is DateTimeAxis)
                            {
                                var foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast, (axis as DateTimeAxis).IntervalType);
                                _trendXSegmentValues.Add(foreCastEndValue);
                            }
                            else
                                _trendXSegmentValues.Add((_xMax) + ForwardForecast);
                        }
                        _trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, count + ForwardForecast));
                    }
                    else
                    {
                        x += (count + ForwardForecast) / PolynomialSlopes.Length;
                        if (!(count == x))
                        {
                            if (Series.ActualXAxis is CategoryAxis)
                                _trendXSegmentValues.Add(Math.Ceiling(x) - 1);
                            else
                                if (count > x)
                                    _trendXSegmentValues.Add(_xValues[(int)x - 1]);

                            _trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, Math.Ceiling(x)));
                        }
                    }
                }
            }

            //create spline segments
            CreateSpline();
        }
        
        #endregion

        #region Logarithmic Trendline
       
        /// <summary>
        /// Update Logarithmic Trend Source
        /// </summary>
        private void UpdateLogarithmicTrendSource()
        {
            _trendXValues = new List<double>();
            _trendYValues = new List<double>();
            if (Series is FinancialSeriesBase)
                _trendYValues = (Series as FinancialSeriesBase).CloseValues;
            else if (Series is RangeSeriesBase)
                _trendYValues = (Series as RangeSeriesBase).LowValues;
            else
                _trendYValues = (Series as XyDataSeries).YValues;
            for (int i = 0; i < Series.DataCount; i++)
            {
                _trendXValues.Add(Math.Log(i + 1));
            }
            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Caluculate Logarithmic Value and Draw Trendline
        /// </summary>
        private void CalculateLogarithmicTrendline()
        {
            var n = _trendXValues.Count;

            if (n > 0)
            {
                CalculateTrendXSegment(n);

                _trendYSegmentValues.Add(GetLogarithmicYValue(1) - BackwardForecast);
                _trendYSegmentValues.Add(GetLogarithmicYValue(Math.Round((double)n / 2)));
                _trendYSegmentValues.Add(GetLogarithmicYValue(n) + ForwardForecast);

                CreateSpline();

            }
        }

        #endregion 

        #region Exponential Trendline

        /// <summary>
        /// Update Exponential Trend Source
        /// </summary>
        private void UpdateExponentialTrendSource()
        {
            _trendXValues = new List<double>();
            _trendYValues = new List<double>();
            for (int i = 0; i < Series.DataCount; i++)
            {
                double y;
                if (Series is FinancialSeriesBase)
                    y = Math.Log((Series as FinancialSeriesBase).CloseValues[i]);
                else if (Series is RangeSeriesBase)
                    y = Math.Log((Series as RangeSeriesBase).LowValues[i]);
                else
                    y = Math.Log((Series as XyDataSeries).YValues[i]);
                _trendYValues.Add(y);
                _trendXValues.Add(i + 1);
            }

            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Calculate Exponential Value and Draw Trendline
        /// </summary>
        private void CalculateExponentialTrendline()
        {
            var n = _trendXValues.Count;
            if (n > 0)
            {
                CalculateTrendXSegment(n);

                _trendYSegmentValues.Add(GetExponentialYValue(1 - BackwardForecast));
                _trendYSegmentValues.Add(GetExponentialYValue(Math.Round((double)n / 2)));
                _trendYSegmentValues.Add(GetExponentialYValue(n + ForwardForecast));

                CreateSpline();
            }
        }

#endregion

        #region Power Trendline
        /// <summary>
        /// Update Power TrendSource
        /// </summary>
        private void UpdatePowerTrendSource()
        {
            _trendXValues = new List<double>();
            _trendYValues = new List<double>();
            for (int i = 0; i < Series.DataCount; i++)
            {
                double y;
                if (Series is FinancialSeriesBase)
                    y = Math.Log((Series as FinancialSeriesBase).CloseValues[i]);
                else if (Series is RangeSeriesBase)
                    y = Math.Log((Series as RangeSeriesBase).LowValues[i]);
                else
                    y = Math.Log((Series as XyDataSeries).YValues[i]);
                _trendYValues.Add(y);
                _trendXValues.Add(Math.Log(i + 1));
            }
            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Calculate Power Value and Draw Trendline
        /// </summary>
        private void CalculatePowerTrendline()
        {
            var n = _trendXValues.Count;
            if (n > 0)
            {
                CalculateTrendXSegment(n);

                _trendYSegmentValues.Add(GetPowerYValue(1) - BackwardForecast);
                _trendYSegmentValues.Add(GetPowerYValue(Math.Round((double)n / 2)));
                _trendYSegmentValues.Add(GetPowerYValue(n) + ForwardForecast);

                CreateSpline();

            }
        }
        #endregion

        #region Linear TrendLine
        /// <summary>
        /// Update Linear Trend Source
        /// </summary>
        private void UpdateTrendSource()
        {
           _trendXValues = new List<double>();
            _trendYValues = new List<double>();
           if (Series is FinancialSeriesBase)
                _trendYValues = (Series as FinancialSeriesBase).CloseValues;
            else if (Series is RangeSeriesBase)
                _trendYValues = (Series as RangeSeriesBase).LowValues;
            else
                _trendYValues = (Series as XyDataSeries).YValues;

            for (int i = 0; i < Series.DataCount; i++)
            {
               _trendXValues.Add(i + 1);
            }

            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Calculate Linear Value and Draw Trendline
        /// </summary>
        private void CalculateLinearTrendline()
        {
            var count = _trendXValues.Count;

            if (count > 0)
            {
                LineSegment linesegemnt;
                var startYValue = GetLinearYValue(1-BackwardForecast);
                var endYValue = GetLinearYValue(count+ ForwardForecast);

                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                {
                    linesegemnt = new LineSegment(0 - BackwardForecast, startYValue, (count - 1) + ForwardForecast, endYValue, this) { Interior = this.Stroke, Stroke = this.Stroke, StrokeThickness = this.StrokeThickness, StrokeDashArray = this.StrokeDashArray };
                    linesegemnt.SetData(0 - BackwardForecast, startYValue, (count - 1) + ForwardForecast, endYValue);
                }
                else
                {
                    var axis = Series.ActualXAxis as DateTimeAxis;
                    if (axis != null)
                    {
                        
                        double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,axis.IntervalType );

                        double foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast, axis.IntervalType);

                        linesegemnt = new LineSegment(foreCastStartValue, startYValue, foreCastEndValue, endYValue, this)
                        {
                            Interior = this.Stroke,
                            Stroke = this.Stroke,
                            StrokeThickness = this.StrokeThickness,
                            StrokeDashArray = this.StrokeDashArray
                        };
                        linesegemnt.SetData(foreCastStartValue, startYValue, foreCastEndValue, endYValue);

                    }
                    else
                    {
                        linesegemnt = new LineSegment(_xMin - BackwardForecast, startYValue, _xMax + ForwardForecast, endYValue, this)
                        {
                            Interior = this.Stroke,
                            Stroke = this.Stroke,
                            StrokeThickness = this.StrokeThickness,
                            StrokeDashArray = this.StrokeDashArray
                        };
                        linesegemnt.SetData(_xMin - BackwardForecast, startYValue, _xMax + ForwardForecast, endYValue);
                    }
                }
                linesegemnt.Series = this.Series;
                TrendlineSegments.Add(linesegemnt);

            }

        }

        #endregion

        #region Trendline calculation method

        /// <summary>
        /// Calculate Sum of x and y values
        /// </summary>
        private void CalculateSumXAndYValue()
        {
            var count = _trendXValues.Count;
            var sumX = _trendXValues.Sum(x => x);
            var sumX2 = _trendXValues.Sum(x => x * x);
            var sumY = _trendYValues.Sum(y => y);
            double sumXY = 0;
            for (var i = 0; i < count; i++)
                sumXY += _trendXValues[i]*_trendYValues[i];
            Slope = ((sumXY * count) - (sumX * sumY)) / ((sumX2 * count) - (sumX * sumX));
            if (Type == TrendlineType.Exponential || Type == TrendlineType.Power)
              Intercept=  Math.Exp((sumY - (Slope * sumX)) / count);
            else
                Intercept = (sumY - (Slope*sumX))/count;
        }

        /// <summary>
        /// Calculate Trend Segment X values
        /// </summary>
        /// <param name="n"></param>
        private void CalculateTrendXSegment(int n)
        {
            _trendXSegmentValues = new List<double>();
            _trendYSegmentValues = new List<double>();

            if (Series.ActualXAxis is CategoryAxis)
            {
                _trendXSegmentValues.Add(BackwardForecast);
                _trendXSegmentValues.Add(Math.Round((double)n / 2) - 1);
                _trendXSegmentValues.Add((n - 1) + ForwardForecast);
            }
            else
            {
                var axis = Series.ActualXAxis as DateTimeAxis;
                if (axis != null)
                {
                    double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,axis.IntervalType);

                    _trendXSegmentValues.Add(foreCastStartValue);

                    _trendXSegmentValues.Add(_xValues[(int)Math.Round((double)n / 2)]);

                    double foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast, axis.IntervalType);

                    _trendXSegmentValues.Add(foreCastEndValue);
                }
                else
                {
                    _trendXSegmentValues.Add(_xMin - BackwardForecast);
                    _trendXSegmentValues.Add(_xValues[(int)Math.Round((double)n / 2)]);
                    _trendXSegmentValues.Add((_xMax) + ForwardForecast);
                }
            }
        }

        private double CalculateDateTimeForecastValue(double value, double forecastValue, DateTimeIntervalType type)
        {
            DateTime start = Convert.ToDouble(value).FromOADate();
            DateTime foreCastStartDate = DateTimeAxisHelper.IncreaseInterval(start, forecastValue,type);
            double foreCastValue = Convert.ToDateTime(foreCastStartDate).ToOADate();
            return foreCastValue;
        }

        void Trendline_ActualRangeChanged(object sender, ActualRangeChangedEventArgs e)
        {
            var axis = sender as DateTimeAxis;
            if (sender is DateTimeAxis && axis.IntervalType == DateTimeIntervalType.Auto)
            {
                if (Type == TrendlineType.Linear)
                {
                    double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,
                        axis.ActualIntervalType);

                    double foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast,
                        axis.ActualIntervalType);

                    var startYValue = GetLinearYValue(1 - BackwardForecast);
                    var endYValue = GetLinearYValue(_xValues.Count + ForwardForecast);

                    TrendlineSegments[0].SetData(foreCastStartValue, startYValue, foreCastEndValue, endYValue);
                }
                else
                {
                    _trendXSegmentValues[0] = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,
                        axis.ActualIntervalType);
                    _trendXSegmentValues[_trendXSegmentValues.Count - 1] = CalculateDateTimeForecastValue(_xMax,
                        ForwardForecast, axis.ActualIntervalType);
                    CreateSpline();

                }

                var min = TrendlineSegments[0].XRange.Start;
                var max = TrendlineSegments[TrendlineSegments.Count - 1].XRange.End;
                var date = (DateTime)e.ActualMinimum;
                if (min < date.ToOADate())
                    e.ActualMinimum = Convert.ToDouble(min).FromOADate();
                date = (DateTime)e.ActualMaximum;
                if (max > date.ToOADate())
                    e.ActualMaximum = Convert.ToDouble(max).FromOADate();
            }
        }
        
        #endregion

        #region Trendline Y Equation
        /// <summary>
        /// Get Linear Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values</returns>
        private double GetLinearYValue(double xValue)
        {
            return Intercept + Slope * xValue;
        }
        /// <summary>
        /// Get Logarithmic Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values</returns>
        private double GetLogarithmicYValue(double xValue)
        {
            return Intercept + Slope * Math.Log(xValue);
        }
        /// <summary>
        /// Get Exponential Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values</returns>
        private double GetExponentialYValue(double xValue)
        {
            return (Intercept * Math.Exp(Slope * xValue));
        }
        /// <summary>
        /// Get Power Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values </returns>
        private double GetPowerYValue(double xValue)
        {
            return (Intercept * Math.Pow(xValue, Slope));
        }
        /// <summary>
        /// Get polynomial y value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns>Segment y values</returns>
        private double GetPolynomialYValue(double[] a, double x)
        {
            return a.Select((t, index) => t*Math.Pow(x, (double) index)).Sum();
        }

        #endregion

        #region Spline Segment Method
        /// <summary>
        /// Create Spline Segment of Trendline
        /// </summary>
        private void CreateSpline()
        {
            TrendlineSegments.Clear();
            int index = -1;
            double[] yCoef = null;

            NaturalSpline(_trendXSegmentValues, _trendYSegmentValues, out yCoef);
            for (int i = 0; i < _trendXSegmentValues.Count; i++)
            {
                index = i + 1;

                var startPoint = new Point(_trendXSegmentValues[i], _trendYSegmentValues[i]);
                if (index < _trendXSegmentValues.Count)
                {
                    var endPoint = new Point(_trendXSegmentValues[index], _trendYSegmentValues[index]);
                    Point startControlPoint;
                    Point endControlPoint;

                    GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);

                    var splineSegment = new SplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this.Series)
                    {
                       
                        Interior = this.Stroke,
                        StrokeDashArray = this.StrokeDashArray,
                        StrokeThickness = this.StrokeThickness
                    };
                    TrendlineSegments.Add(splineSegment);

                }
            }
        }

        /// <summary>
        /// Coefficient Of Natural Spline Segment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="ys2"></param>
        protected void NaturalSpline(List<double> xValues, List<double> yValues, out double[] ys2)
        {
            var count = (int)xValues.Count;

            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count];
            double p;

            ys2[0] = u[0] = 0;
            ys2[count - 1] = 0;

            for (int i = 1; i < count - 1; i++)
            {
                double d1 = xValues[i] - xValues[i - 1];
                double d2 = xValues[i + 1] - xValues[i - 1];
                double d3 = xValues[i + 1] - xValues[i];
                double dy1 = yValues[i + 1] - yValues[i];
                double dy2 = yValues[i] - yValues[i - 1];

                if (xValues[i] == xValues[i - 1] || xValues[i] == xValues[i + 1])
                {
                    ys2[i] = 0;
                    u[i] = 0;
                }
                else
                {
                    p = 1 / (d1 * ys2[i - 1] + 2 * d2);
                    ys2[i] = -p * d3;
                    u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = ys2[k] * ys2[k + 1] + u[k];
            }
        }

        /// <summary>
        /// Returns the controlPoints of the curve
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="ys1"></param>
        /// <param name="ys2"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        protected void GetBezierControlPoints(Point point1, Point point2, double ys1, double ys2, out Point controlPoint1, out Point controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;

            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = 2 * point1.X + point2.X;
            double dx2 = point1.X + 2 * point2.X;

            double dy1 = 2 * point1.Y + point2.Y;
            double dy2 = point1.Y + 2 * point2.Y;

            double y1 = One_thrid * (dy1 - One_thrid * deltaX2 * (ys1 + 0.5f * ys2));
            double y2 = One_thrid * (dy2 - One_thrid * deltaX2 * (0.5f * ys1 + ys2));

            controlPoint1 = new Point(dx1 * One_thrid, y1);
            controlPoint2 = new Point(dx2 * One_thrid, y2);
        }
        #endregion

        #endregion
    }

}
