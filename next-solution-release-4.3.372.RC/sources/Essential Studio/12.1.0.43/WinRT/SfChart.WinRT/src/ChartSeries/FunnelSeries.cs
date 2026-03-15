#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
#if WPF
using System.Data;
#endif
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FunnelSeries displays its path using a set of data's.
    /// </summary>
    /// <seealso cref="FunnelSegment"/>
    /// <seealso cref="PyramidSegment"/>
    /// <seealso cref="PyramidSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FunnelSeries : TriangularSeriesBase, ISegmentSelectable
    {
        #region fields

        double currY = 0d;

        #endregion

        #region constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FunnelSeries()
        {
            DefaultStyleKey = typeof(FunnelSeries);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the property FunnelMode.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartFunnelMode FunnelMode
        {
            get { return (ChartFunnelMode)GetValue(FunnelModeProperty); }
            set { SetValue(FunnelModeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for FunnelMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FunnelModeProperty =
            DependencyProperty.Register("FunnelMode", typeof(ChartFunnelMode), typeof(FunnelSeries), new PropertyMetadata(ChartFunnelMode.ValueIsHeight));

        /// <summary>
        /// Gets or sets the minwidth property.
        /// </summary>
        public new double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MinWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public new static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(FunnelSeries), new PropertyMetadata(40d));


        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of FunnelSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            Segments.Clear();
            Adornments.Clear();
            List<double> xValues = GetXValues();
            double sumValues = 0d, gapRatio = this.GapRatio;
            int count = DataCount;
            int explodedIndex = this.ExplodeIndex;
            ChartFunnelMode funnelmode = this.FunnelMode;

            for (int i = 0; i < count; i++)
            {
                sumValues += Math.Max(0, Math.Abs(YValues[i]));
            }

            if (funnelmode == ChartFunnelMode.ValueIsHeight)
                this.CalculateValueIsHeightSegments(YValues, xValues, sumValues, gapRatio, count, explodedIndex);
            else
                this.CalculateValueIsWidthSegments(YValues, xValues, sumValues, gapRatio, count, explodedIndex);
            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues);
        }

        /// <summary>
        /// Creates the adornment of FunnelSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        protected override ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double height, double currY)
        {
            return new TriangularAdornment(xVal, yVal, currY, height, series);
        }
        /// <summary>
        /// Method implementation for Create Transform
        /// </summary>
        /// <param name="size"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected internal override IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || ChartTransformer == null)
            {
                ChartTransformer = ChartTransform.CreateSimple(size);
            }

            return ChartTransformer;
        }

        /// <summary>
        /// Updates the segment at the specified index
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <param name="action">The action that caused the segments collection changed event</param>
        [ClassReference(IsReviewed = false)]
        public override void UpdateSegments(int index, System.Collections.Specialized.NotifyCollectionChangedAction action)
        {
            base.RemoveSegments();
            CreateSegments();
            UpdateRange();
            foreach (ChartSegment segment in Segments)
            {
                segment.Update(ChartTransformer);
            }
        }

        /// <summary>
        /// To calculate the segments if the pyramid mode is vlaueisHeight.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        private void CalculateValueIsHeightSegments(IList<double> yValues, List<double> xValues, double sumValues, double gapRatio, int dataCount, int explodedIndex)
        {
            currY = 0d;
            double coefHeight = 1 / sumValues;
            double spacing = gapRatio / (DataCount - 1);
#if WPF
            IEnumerator enumerator = (ItemsSource is DataTable) ? (ItemsSource as DataTable).Rows.GetEnumerator() : (ItemsSource as IEnumerable).GetEnumerator();
#else
            IEnumerator enumerator = ItemsSource.GetEnumerator();
#endif
            enumerator.MoveNext();
            for (int i = DataCount - 1; i >= 0; i--)
            {
                double height = 0;
                height = Math.Abs(yValues[i]) * coefHeight;
                FunnelSegment funnelSegment = new FunnelSegment(currY, height, this, explodedIndex == i ? true : false);
                funnelSegment.Item = enumerator.Current;
                funnelSegment.XData = xValues[i];
                funnelSegment.YData = yValues[i];
                Segments.Add(funnelSegment);
                if (AdornmentsInfo != null)
                {
                    Adornments.Add(this.CreateAdornment(this, xValues[i], yValues[i], 0, double.IsNaN(currY) ? 0 : currY + (height + spacing) / 2));
                }
                currY += height + spacing;
                enumerator.MoveNext();
            }
        }

        /// <summary>
        /// To calculate the segments if the pyramid mode is vlaueisWidth.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        private void CalculateValueIsWidthSegments(IList<double> yValues, List<double> xValues, double sumValues, double gapRatio, int count, int explodedIndex)
        {
            currY = 0d;
            double offset = 1d / (count - 1);
            double height = (1 - gapRatio) / (count - 1);
#if WPF
            IEnumerator enumerator = (ItemsSource is DataTable) ? (ItemsSource as DataTable).Rows.GetEnumerator() : (ItemsSource as IEnumerable).GetEnumerator();
#else
            IEnumerator enumerator = ItemsSource.GetEnumerator();
#endif
            enumerator.MoveNext();
            for (int i = DataCount - 1; i > 0; i--)
            {
                double w1 = Math.Abs(yValues[i]);
                double w2 = Math.Abs(yValues[i - 1]);
                FunnelSegment funnelSegment = new FunnelSegment(currY, height, w1 / sumValues, w2 / sumValues, this, explodedIndex == i ? true : false);
                funnelSegment.Item = ActualData[i];
                funnelSegment.XData = xValues[i];
                funnelSegment.YData = yValues[i];
                Segments.Add(funnelSegment);
                if (AdornmentsInfo != null)
                {
                    Adornments.Add(this.CreateAdornment(this, xValues[i], yValues[i], height, currY));
                    Adornments[Adornments.Count - 1].Item = ActualData[i];
                }
                currY += offset;
                enumerator.MoveNext();
            }
            if (AdornmentsInfo != null && DataCount > 0)
            {
                Adornments.Add(this.CreateAdornment(this, xValues[0], yValues[0], height, currY));
                Adornments[Adornments.Count - 1].Item = ActualData[0];
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FunnelSeries() { FunnelMode = this.FunnelMode, MinWidth = this.MinWidth });
        }

        #endregion
    }

}
