#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;

#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastLineBitmapSeries is another version of LineSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    ///</remarks>
    ///<seealso cref="FastLineBitmapSegment"/>
    ///<seealso cref="FastLineSeries"/>
    ///<seealso cref="LineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastLineBitmapSeries: XyDataSeries
    {
        #region

        private FastLineBitmapSegment Segment { get; set; }

        bool isAdornmentPending;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets a value that determines whether to anti-alias line.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool EnableAntiAliasing
        {
            get { return (bool)GetValue(EnableAntiAliasingProperty); }
            set { SetValue(EnableAntiAliasingProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAntiAliasing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableAntiAliasingProperty =
            DependencyProperty.Register("EnableAntiAliasing", typeof(bool), typeof(FastLineBitmapSeries), new PropertyMetadata(false, OnSeriesPropertyChanged));

        /// <summary>
        /// Gets or sets a collection of Double values that indicates the pattern of
        /// dashes and gaps that is used to outline shapes.</summary>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(FastLineBitmapSeries), new PropertyMetadata(null, OnSeriesPropertyChanged));

        #endregion

        #region Methods

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            Segment = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        private static void OnSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastLineBitmapSeries).UpdateArea();
        }

        /// <summary>
        /// Called when VisibleRange property changed
        /// </summary>
        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            if (AdornmentsInfo != null && isAdornmentPending)
            {
                List<double> xValues = GetXValues();

                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    double xBase = ActualXAxis.IsLogarithmic ? (ActualXAxis as LogarithmicAxis).LogarithmicBase : 1;
                    bool xIsLogarithmic = ActualXAxis.IsLogarithmic;
                    double start = ActualXAxis.VisibleRange.Start;
                    double end = ActualXAxis.VisibleRange.End;
                    for (int i = 0; i < DataCount; i++)
                    {
                        double x = xValues[i];
                        double edgeValue = xIsLogarithmic ? Math.Log(x, xBase) : x;
                        if (edgeValue >= start && edgeValue <= end)
                        {
                            double y = YValues[i];

                            if (i < Adornments.Count)
                            {
                                Adornments[i].SetData(x, y, x, y);
                            }
                            else
                            {
                                Adornments.Add(this.CreateAdornment(this, x, y, x, y));
                            }
                            Adornments[i].Item = ActualData[i];
                        }
                    }
                }

                isAdornmentPending = false;
            }
        }

        /// <summary>
        /// Creates the segments of FastLineBitmapSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            ClearUnUsedAdornments(this.DataCount);

            if (Segment == null || Segments.Count == 0)
            {
                FastLineBitmapSegment segment = new FastLineBitmapSegment(ActualXValues as IList<double>, YValues, this);
                Segment = segment;
                Segments.Add(segment);
            }
            else if (ActualXValues != null)
            {
                Segment.SetData(ActualXValues as IList<double>, YValues);
                (Segment as FastLineBitmapSegment).SetRange();
            }

            isAdornmentPending = true;
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastLineBitmapSeries() { EnableAntiAliasing = this.EnableAntiAliasing });
        }

        #endregion
    }
}
