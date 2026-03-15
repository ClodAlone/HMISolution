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
    [ClassReference(IsReviewed = false)]
    public class FastScatterBitmapSeries : XyDataSeries
    {

        #region fields

        private FastScatterBitmapSegment Segment { get; set; }

        bool isAdornmentsBending;

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets scatter segment’s width.
        /// </summary>
        public double ScatterWidth
        {
            get { return (double)GetValue(ScatterWidthProperty); }
            set { SetValue(ScatterWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScatterWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScatterWidthProperty =
            DependencyProperty.Register("ScatterWidth", typeof(double), typeof(FastScatterBitmapSeries), new PropertyMetadata(3d, OnScatterWidthChanged));

        /// <summary>
        /// Gets or Sets scatter segment’s height.
        /// </summary>
        public double ScatterHeight
        {
            get { return (double)GetValue(ScatterHeightProperty); }
            set { SetValue(ScatterHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScatterHeightProperty =
            DependencyProperty.Register("ScatterHeight", typeof(double), typeof(FastScatterBitmapSeries), new PropertyMetadata(3d, OnScatterHeightChanged));

        #endregion

        #region methods

        [ClassReference(IsReviewed = false)]
        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            if (AdornmentsInfo != null && isAdornmentsBending)
            {
                List<double> xValues = GetXValues();
                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    for (int i = 0; i < DataCount; i++)
                    {
                        AddAdornments(xValues[i], YValues[i], i);
                    }
                }
                isAdornmentsBending = false;
            }
        }

        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            ClearUnUsedAdornments(this.DataCount);
            if (Segments == null || Segments.Count == 0)
            {
                Segment = new FastScatterBitmapSegment(ActualXValues as IList<double>, YValues, this);
                Segments.Add(Segment);
            }
            else if (ActualXValues != null)
            {
                Segment.SetData(ActualXValues as IList<double>, YValues);
                (Segment as FastScatterBitmapSegment).SetRange();
            }
            isAdornmentsBending = true;
        }

        [ClassReference(IsReviewed = false)]
        private void AddAdornments(double x, double yValue, int i)
        {
            double adornX = 0d, adornY = 0d;
            adornX = x;
            adornY = yValue;
            if (i < Adornments.Count)
            {
                Adornments[i].SetData(adornX, adornY, adornX, adornY);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, adornX, adornY, adornX, adornY));
            }
            Adornments[i].Item = ActualData[i];
        }

        private static void OnScatterWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FastScatterBitmapSeries series = d as FastScatterBitmapSeries;
            if (series != null)
                series.UpdateArea();
        }

        private static void OnScatterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FastScatterBitmapSeries series = d as FastScatterBitmapSeries;
            if (series != null)
                series.UpdateArea();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return
                base.CloneSeries(new FastScatterBitmapSeries()
                    {
                        ScatterHeight = this.ScatterHeight,
                        ScatterWidth = this.ScatterWidth
                    });
        }

        #endregion

    }
}
