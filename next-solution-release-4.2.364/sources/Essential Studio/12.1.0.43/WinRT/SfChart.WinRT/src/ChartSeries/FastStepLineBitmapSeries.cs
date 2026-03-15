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
#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastStepLineBitmapSeries is another version of StepLineSeries which uses different technology for rendering Stepline in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    ///</remarks>
    ///<seealso cref="FastStepLineBitmapSegment"/>
    [ClassReference(IsReviewed = false)]
    public class FastStepLineBitmapSeries : XyDataSeries
    {
        #region fields

        private FastStepLineBitmapSegment Segment { get; set; }

        bool isAdornmentPending;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets a value that determines whether to anti-alias Stepline.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool EnableAntiAliasing
        {
            get { return (bool)GetValue(EnableAntiAliasingProperty); }
            set { SetValue(EnableAntiAliasingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAntiAliasing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAntiAliasingProperty =
            DependencyProperty.Register("EnableAntiAliasing", typeof(bool), typeof(FastStepLineBitmapSeries), new PropertyMetadata(false));

        #endregion

        #region Methods

        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            if (AdornmentsInfo != null && isAdornmentPending)
            {
                List<double> xValues = GetXValues();

                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    double start = ActualXAxis.VisibleRange.Start;
                    double end = ActualXAxis.VisibleRange.End;

                    for (int i = 0; i < DataCount; i++)
                    {
                        double x = xValues[i];

                        if (x >= start && x <= end)
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
        /// Creates the segments of FastStepLineBitmapSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            ClearUnUsedAdornments(DataCount);

            if (Segment == null || Segments.Count == 0)
            {
                Segment = new FastStepLineBitmapSegment(GetXValues(), YValues, this);
                Segments.Add(Segment);
            }
            else if (ActualXValues != null)
            {
                Segment.SetData(ActualXValues as IList<double>, YValues);
                Segment.SetRange();
            }

            isAdornmentPending = true;
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastStepLineBitmapSeries() {EnableAntiAliasing = this.EnableAntiAliasing});
        }

        #endregion
    }
}