#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    [ClassReference(IsReviewed = false)]
    public class ErrorBarSeries : XyDataSeries
    {

        #region Properties

        double _horizontalErrorValue;
        
        #region InternalClRProperties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        protected internal IList<double> HorizontalCustomValues
        {
            get;
            set;
        }

        protected internal IList<double> VerticalCustomValues
        {
            get;
            set;
        }

        internal string HorizontalErrorMemberPath { get; set; }

        internal string VerticalErrorMemberPath { get; set; }
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets and Sets Horizontal Error Member path. This property defines the member of the ItemsSource collection, which need to bind as Horizontal error value and its will be working only in Custom Type of ErrorBar..
        /// </summary>
        public string HorizontalErrorPath
        {
            get { return (string)GetValue(HorizontalErrorPathProperty); }
            set { SetValue(HorizontalErrorPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorValueMemberPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalErrorPathProperty =
            DependencyProperty.Register("HorizontalErrorPath", typeof(string), typeof(ErrorBarSeries), new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// Gets and Sets Vertical Error Member path. This property defines the member of the ItemsSource collection, which need to bind as vertical error value and its will be working in Custom Type of ErrorBar..
        /// </summary>
        public string VerticalErrorPath
        {
            get { return (string)GetValue(VerticalErrorPathProperty); }
            set { SetValue(VerticalErrorPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorValueMemberPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalErrorPathProperty =
            DependencyProperty.Register("VerticalErrorPath", typeof(string), typeof(ErrorBarSeries), new PropertyMetadata(null,OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorBarSeries = d as ErrorBarSeries;
            if (errorBarSeries != null && e.NewValue!=null) errorBarSeries.OnBindingPathChanged(e);
        }

        /// <summary>
        /// Gets and Sets Horizontal Line Style.
        /// </summary>
        public LineStyle HorizontalLineStyle
        {
            get { return (LineStyle)GetValue(HorizontalLineStyleProperty); }
            set { SetValue(HorizontalLineStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalLineStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalLineStyleProperty =
            DependencyProperty.Register("HorizontalLineStyle", typeof(LineStyle), typeof(ErrorBarSeries), new PropertyMetadata(null,OnHorizontalPropertyChanged));

        /// <summary>
        /// Gets and Sets Vertical Line Style.
        /// </summary>
        public LineStyle VerticalLineStyle
        {
            get { return (LineStyle)GetValue(VerticalLineStyleProperty); }
            set { SetValue(VerticalLineStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalLineStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalLineStyleProperty =
            DependencyProperty.Register("VerticalLineStyle", typeof(LineStyle), typeof(ErrorBarSeries), new PropertyMetadata(null, OnVerticalPropertyChanged));

        /// <summary>
        /// Gets and Sets Horizontal Cap Line Style.
        /// </summary>
        public CapLineStyle HorizontalCapLineStyle
        {
            get { return (CapLineStyle)GetValue(HorizontalCapLineStyleProperty); }
            set { SetValue(HorizontalCapLineStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalLineStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalCapLineStyleProperty =
            DependencyProperty.Register("HorizontalCapLineStyle", typeof(CapLineStyle), typeof(ErrorBarSeries), new PropertyMetadata(null,OnHorizontalCapPropertyChanged));

        /// <summary>
        /// Gets and Sets Vertical Cap Line Style.
        /// </summary>
        public CapLineStyle VerticalCapLineStyle
        {
            get { return (CapLineStyle)GetValue(VerticalCapLineStyleProperty); }
            set { SetValue(VerticalCapLineStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalLineStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalCapLineStyleProperty =
            DependencyProperty.Register("VerticalCapLineStyle", typeof(CapLineStyle), typeof(ErrorBarSeries), new PropertyMetadata(null, OnVerticalCapPropertyChanged));

        /// <summary>
        /// Gets and Sets Horizontal Error Value. This property will be working only in Fixed and Percentage type.
        /// </summary>
        public double HorizontalErrorValue
        {
            get { return (double)GetValue(HorizontalErrorValueProperty); }
            set { SetValue(HorizontalErrorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalErrorValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalErrorValueProperty =
            DependencyProperty.Register("HorizontalErrorValue", typeof(double), typeof(ErrorBarSeries), new PropertyMetadata(0d, OnHorizontalErrorValuePropertyChanged));

        /// <summary>
        /// Gets and Sets Vertical Error Value. This property will be working only in Fixed and Percentage type
        /// </summary>
        public double VerticalErrorValue
        {
            get { return (double)GetValue(VerticalErrorValueProperty); }
            set { SetValue(VerticalErrorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalErrorValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalErrorValueProperty =
            DependencyProperty.Register("VerticalErrorValue", typeof(double), typeof(ErrorBarSeries), new PropertyMetadata(0d, OnVerticalErrorValuePropertyChanged));
        
        /// <summary>
        /// Gets or Sets the ErrorBar Mode, This property is to customize the horizontal and vertical error bars.
        /// </summary>
        public ErrorBarMode Mode
        {
            get { return (ErrorBarMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ErrorBarMode), typeof(ErrorBarSeries), new PropertyMetadata(ErrorBarMode.Both, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets the ErrorBar Type, This property of type ErrorBarType, which is used to set the type of Error need to plot.
        /// </summary>
        public ErrorBarType Type
        {
            get { return (ErrorBarType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(ErrorBarType), typeof(ErrorBarSeries), new PropertyMetadata(ErrorBarType.Fixed, OnPropertyChanged));
       
        private static void OnHorizontalErrorValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.Area != null && (instance.Type == ErrorBarType.Fixed || instance.Type == ErrorBarType.Percentage) && instance.Mode != ErrorBarMode.Vertical)
                instance.Area.ScheduleUpdate();
        }

        private static void OnVerticalErrorValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.Area != null && (instance.Type == ErrorBarType.Fixed || instance.Type == ErrorBarType.Percentage) && instance.Mode != ErrorBarMode.Horizontal)
                instance.Area.ScheduleUpdate();
        }
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.Area != null)
               instance.Area.ScheduleUpdate();
        }

        private static void OnHorizontalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.HorizontalLineStyle != null )
            {
                instance.HorizontalLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                 if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }
        private static void OnHorizontalCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.HorizontalCapLineStyle != null )
            {
                instance.HorizontalCapLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                 if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }
        private static void OnVerticalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.VerticalLineStyle != null )
            {
                instance.VerticalLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                 if (instance.Area != null)
                   instance.Area.ScheduleUpdate();
            }
        }
        private static void OnVerticalCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.VerticalCapLineStyle != null )
            {
                instance.VerticalCapLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                 if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }

        protected override void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            base.OnXAxisChanged(oldAxis, newAxis);
            if (newAxis is DateTimeAxis)
                newAxis.ActualRangeChanged += ErrorBarSeries_ActualRangeChanged;
            if(oldAxis is DateTimeAxis)
                oldAxis.ActualRangeChanged -= ErrorBarSeries_ActualRangeChanged;
        }

        #endregion

        #endregion

        #region Contructor
        public ErrorBarSeries()
        {
            DefaultStyleKey = typeof(ErrorBarSeries);
          
            HorizontalCustomValues = new List<double>();
            VerticalCustomValues = new List<double>();
        }
        #endregion

        #region Method
        /// <summary>
        /// Creates the segments of ErrorBarSeries
        /// </summary>
        public override void CreateSegments()
        {
            var xValues = GetXValues();

            if (xValues != null)
            {
                var horSdvalue = GetSdErrorValue(xValues);
                var verSdvalue = GetSdErrorValue(YValues);

                double verticalErrorValue;
               
                if (Type == ErrorBarType.StandardErrors || Type == ErrorBarType.StandardDeviation)
                {
                    _horizontalErrorValue = horSdvalue[1];
                    verticalErrorValue = verSdvalue[1];
                }
                else
                {
                    _horizontalErrorValue = HorizontalErrorValue;
                    verticalErrorValue = VerticalErrorValue;
                }

                if (Segments.Count > this.DataCount)
                {
                    ClearUnUsedSegments(this.DataCount);
                }

                var dateTimeAxis = ActualXAxis as DateTimeAxis;
                
                if (dateTimeAxis != null && dateTimeAxis.IntervalType == DateTimeIntervalType.Auto)
                    dateTimeAxis.ActualRangeChanged += ErrorBarSeries_ActualRangeChanged;
                else if (dateTimeAxis != null && dateTimeAxis.IntervalType != DateTimeIntervalType.Auto)
                    dateTimeAxis.ActualRangeChanged -= ErrorBarSeries_ActualRangeChanged;

                for (var i = 0; i < this.DataCount; i++)
                {
                    switch (Type)
                    {
                        case ErrorBarType.Percentage:
                            if ((ActualXAxis is DateTimeAxis) || (ActualXAxis is CategoryAxis) || (ActualXAxis is DateTimeCategoryAxis))
                            {
                                _horizontalErrorValue = GetPercentageErrorBarValue(i + 1, HorizontalErrorValue);
                                verticalErrorValue = GetPercentageErrorBarValue(YValues[i], VerticalErrorValue);
                            }
                            else
                            {
                                _horizontalErrorValue = GetPercentageErrorBarValue(xValues[i], HorizontalErrorValue);
                                verticalErrorValue = GetPercentageErrorBarValue(YValues[i], VerticalErrorValue);

                            }
                            break;
                        case ErrorBarType.Custom:
                            _horizontalErrorValue = HorizontalCustomValues.Count > 0 ? HorizontalCustomValues[i] : 0;
                            verticalErrorValue = VerticalCustomValues.Count > 0 ? VerticalCustomValues[i] : 0;
                            break;
                    }

                    Point leftPt;
                    Point rightPt;
                    Point topPt;
                    Point bottomPt;
                    if (Type == ErrorBarType.StandardDeviation)
                    {
                        leftPt = new Point(GetMinusValue(horSdvalue[0], _horizontalErrorValue, true), YValues[i]);
                        rightPt = new Point(GetPlusValue(horSdvalue[0], _horizontalErrorValue, true), YValues[i]);
                        topPt = new Point(xValues[i], GetMinusValue(verSdvalue[0], verticalErrorValue, false));
                        bottomPt = new Point(xValues[i], GetPlusValue(verSdvalue[0], verticalErrorValue, false));
                    }
                    else
                    {
                        leftPt = new Point(GetMinusValue(xValues[i], _horizontalErrorValue, true), YValues[i]);
                        rightPt = new Point(GetPlusValue(xValues[i], _horizontalErrorValue, true), YValues[i]);
                        topPt = new Point(xValues[i], GetMinusValue(YValues[i], verticalErrorValue, false));
                        bottomPt = new Point(xValues[i], GetPlusValue(YValues[i], verticalErrorValue, false));
                    }

                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(leftPt, rightPt, topPt, bottomPt);
                    }
                    else
                    {
                        var errorBar = new ErrorBarSegment(leftPt, rightPt, topPt, bottomPt, this,
                            ActualData[i]);
                        Segments.Add(errorBar);
                    }
                }
            }
        }

        /// <summary>
        /// Actual Range Event hooked here for the suppose of DateTimeAxis with Auto type errorbar calculation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorBarSeries_ActualRangeChanged(object sender, ActualRangeChangedEventArgs e)
        {
            var axis = sender as DateTimeAxis;
           if (sender is DateTimeAxis && axis.IntervalType == DateTimeIntervalType.Auto)
            {
                double minrange = ((DateTime)e.ActualMinimum).ToOADate();
                double maxrange = ((DateTime)e.ActualMaximum).ToOADate();
                for (int i = 0; i < Segments.Count; i++)
                {
                    var chartSegment = Segments[i] as ErrorBarSegment;
                    Point point;
                    if(Type == ErrorBarType.Custom)
                        point = chartSegment.DateTimeIntervalCalculation(HorizontalCustomValues.Count > 0 ? HorizontalCustomValues[i] : 0, axis.ActualIntervalType);
                    else
                        point = chartSegment.DateTimeIntervalCalculation(_horizontalErrorValue, axis.ActualIntervalType);
                    if (minrange > point.X)
                        minrange = point.X;
                    if (maxrange < point.Y)
                        maxrange = point.Y;
                }
                var date = (DateTime)e.ActualMinimum;
                if (minrange < date.ToOADate())
                    e.ActualMinimum = Convert.ToDouble(minrange).FromOADate();
                date = (DateTime)e.ActualMaximum;
                if (maxrange > date.ToOADate())
                    e.ActualMaximum = Convert.ToDouble(maxrange).FromOADate();
            }
           }   

       /// <summary>
       /// Calculate StandardDeviation and StandardError value
       /// </summary>
       /// <param name="values"></param>
       /// <returns></returns>
        private double[] GetSdErrorValue(IList<double> values)
        {
            var sum = values.Sum();
            var mean = sum / values.Count;
            var dev = new List<double>();
            var sQDev = new List<double>();
            for (var i = 0; i < values.Count; i++)
            {
                dev.Add(values[i] - mean);
                sQDev.Add(dev[i] * dev[i]);
            }
            var sumSqDev = sQDev.Sum(x => x);
            var valueDoubles = new double[2];
            
            var sDValue = Math.Sqrt(sumSqDev / (values.Count - 1));
            var sDerrorValue = sDValue/ Math.Sqrt(DataCount);
            valueDoubles[0] = mean;
            valueDoubles[1] = Type == ErrorBarType.StandardDeviation ? sDValue : sDerrorValue;
            return valueDoubles;
        }

        /// <summary>
        /// Get Percentage ErrorBar Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        private double GetPercentageErrorBarValue(double value, double errorValue)
        {
          return (value * (errorValue / 100));
        }

       /// <summary>
       /// Calculate the Plus value of line
       /// </summary>
       /// <param name="value"></param>
       /// <param name="errorvalue"></param>
       /// <param name="axischeck"></param>
       /// <returns></returns>
        private double GetPlusValue(double value, double errorvalue, bool axischeck)
        {
            if ((ActualXAxis is DateTimeAxis) && axischeck)
            {
                var dateaxis = ActualXAxis as DateTimeAxis;
                DateTime verDate = Convert.ToDouble(value).FromOADate();
                return DateTimeAxisHelper.IncreaseInterval(verDate, errorvalue, dateaxis.IntervalType).ToOADate();
            }
            return value + errorvalue;
        }
        /// <summary>
        /// Calculate the Minus Value of line
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorvalue"></param>
        /// <param name="axischeck"></param>
        /// <returns></returns>
        private double GetMinusValue(double value, double errorvalue, bool axischeck)
        {
            if ((ActualXAxis is DateTimeAxis) && axischeck)
            {
                var dateaxis = ActualXAxis as DateTimeAxis;
                DateTime verDate = Convert.ToDouble(value).FromOADate();
                return DateTimeAxisHelper.IncreaseInterval(verDate, -errorvalue, dateaxis.IntervalType).ToOADate();
            }
            return value - errorvalue;
        }

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            HorizontalCustomValues.Clear();
            VerticalCustomValues.Clear();
            YValues.Clear();
           if (YBindingPath != null && HorizontalErrorPath != null &&  VerticalErrorPath != null )
               GeneratePoints(new[] { YBindingPath, HorizontalErrorPath, VerticalErrorPath }, YValues, HorizontalCustomValues, VerticalCustomValues);
            else if (YBindingPath != null && HorizontalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, HorizontalErrorPath }, YValues, HorizontalCustomValues);
            else if (YBindingPath != null &&  VerticalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, VerticalErrorPath }, YValues, VerticalCustomValues);
            else if (YBindingPath != null)
            {
                GeneratePoints(new[] { YBindingPath }, YValues);
            }
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            HorizontalCustomValues.Clear();
            VerticalCustomValues.Clear();
            YValues.Clear();
           if (YBindingPath != null && HorizontalErrorPath != null &&  VerticalErrorPath != null )
               GeneratePoints(new[] { YBindingPath, HorizontalErrorPath, VerticalErrorPath }, YValues, HorizontalCustomValues, VerticalCustomValues);
            else if (YBindingPath != null && HorizontalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, HorizontalErrorPath }, YValues, HorizontalCustomValues);
            else if (YBindingPath != null &&  VerticalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, VerticalErrorPath }, YValues, VerticalCustomValues);
            else if (YBindingPath != null)
            {
                GeneratePoints(new[] { YBindingPath }, YValues);
            } 
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            HorizontalCustomValues.Clear();
            VerticalCustomValues.Clear();
            YValues.Clear();
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Clone method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var errorBarSeries = obj as ErrorBarSeries;
            if (errorBarSeries != null)
            {
                errorBarSeries.HorizontalErrorPath = HorizontalErrorPath;
                errorBarSeries.VerticalErrorPath = VerticalErrorPath;
            }
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
