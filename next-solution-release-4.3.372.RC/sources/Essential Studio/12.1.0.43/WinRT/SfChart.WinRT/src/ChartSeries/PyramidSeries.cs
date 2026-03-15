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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
    /// PyramidSeries displays data as a proportion of the whole.PyramidSeries are most commonly used to make comparisons among a set of given data.
    /// </summary>
    /// <remarks>
    /// PyramidSeries does not have any axis. The segments in PyramidSeries can be explode to a certain distance from the center using <see>
    ///                                                                                                                                    <cref>PyramidSeries.ExplodeIndex</cref>
    ///                                                                                                                                </see>
    ///     property.
    /// The segments can be filled with a custom set of colors using <see cref="ChartColorModel.CustomBrushes"/> property.
    /// </remarks>
    /// <seealso cref="PyramidSegment"/>
    /// <seealso cref="FunnelSegment"/>
    /// <seealso cref="FunnelSeries"/>
    [ClassReference(IsReviewed = false)]
    public class PyramidSeries : TriangularSeriesBase, ISegmentSelectable
    {
        #region fields

        double currY = 0;

        #endregion

        #region properties

        ///<summary>
        ///Gets or Sets PyramidMode.
        ///</summary>
        [ClassReference(IsReviewed = false)]
        public ChartPyramidMode PyramidMode
        {
            get { return (ChartPyramidMode)GetValue(PyramidModeProperty); }
            set { SetValue(PyramidModeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for PyramidMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PyramidModeProperty =
            DependencyProperty.Register("PyramidMode", typeof(ChartPyramidMode), typeof(PyramidSeries), new PropertyMetadata(ChartPyramidMode.Linear));

        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for PyramidSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public PyramidSeries()
        {
            DefaultStyleKey = typeof(PyramidSeries);
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates the segment of PyramidSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            Adornments.Clear();
            this.Segments.Clear();
            int count = DataCount;
            List<double> xValues = GetXValues();
            double sumValues = 0;
            double gapRatio = this.GapRatio;
            ChartPyramidMode pyramidMode = this.PyramidMode;
            for (int i = 0; i < count; i++)
            {
                sumValues += Math.Max(0, Math.Abs(YValues[i]));
            }
            double gapHeight = gapRatio / (count - 1);
            if (pyramidMode == ChartPyramidMode.Linear)
                this.CalculateLinearSegments(sumValues, gapRatio, count, xValues);
            else
                this.CalculateSurfaceSegments(sumValues, count, gapHeight, xValues);
            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues);
        }

        /// <summary>
        /// Creates the adornment of PyramidSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        protected override ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double height, double currY)
        {
            return new TriangularAdornment(xVal, yVal, currY, height, series);
        }

        /// <summary>
        /// Creates the adornment of PyramidSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void UpdateSegments(int index, NotifyCollectionChangedAction action)
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
        /// Return IChartTranform value based upon the given size
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
        /// To calculate the segments if the pyramid mode is linear
        /// </summary>
        [ClassReference(IsReviewed = false)]
        private void CalculateLinearSegments(double sumValues, double gapRatio, int count, List<double> xValues)
        {
            currY = 0;
            double coef = 1d / (sumValues * (1 + gapRatio / (1 - gapRatio)));
#if WPF
            IEnumerator enumerator = (ItemsSource is DataTable) ? (ItemsSource as DataTable).Rows.GetEnumerator() : (ItemsSource as IEnumerable).GetEnumerator();
#else
            IEnumerator enumerator = ItemsSource.GetEnumerator();
#endif
            enumerator.MoveNext();
            for (int i = 0; i < count; i++)
            {
                double height = coef * Math.Abs(YValues[i]);
                PyramidSegment pyramidSegment = new PyramidSegment(currY, height, this.ExplodeOffset, this, i == ExplodeIndex ? true : false);
                pyramidSegment.Item = enumerator.Current;
                pyramidSegment.XData = xValues[i];
                pyramidSegment.YData = Math.Abs(YValues[i]);
                this.Segments.Add(pyramidSegment);
                currY += (gapRatio / (count - 1)) + height;
                if (AdornmentsInfo != null)
                {
                    Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], 0, double.IsNaN(currY) ? 1 - height / 2 : currY - height / 2));
                    Adornments[i].Item = ActualData[i];
                }
                enumerator.MoveNext();
            }
        }

        /// <summary>
        /// To calculate the segments if the pyramid mode is surface
        /// </summary>
        [ClassReference(IsReviewed = false)]
        private void CalculateSurfaceSegments(double sumValues, int count, double gapHeight, List<double> xValues)
        {
            currY = 0;
            double[] y = new double[count];
            double[] height = new double[count];
            double preSum = GetSurfaceHeight(0, sumValues);
#if WPF
            IEnumerator enumerator = (ItemsSource is DataTable) ? (ItemsSource as DataTable).Rows.GetEnumerator() : (ItemsSource as IEnumerable).GetEnumerator();
#else
            IEnumerator enumerator = ItemsSource.GetEnumerator();
#endif
            enumerator.MoveNext();
            for (int i = 0; i < count; i++)
            {
                y[i] = currY;
                height[i] = GetSurfaceHeight(currY, Math.Abs(YValues[i]));
                currY += height[i] + gapHeight * preSum;
            }
            double coef = 1 / (currY - gapHeight * preSum);
            for (int i = 0; i < count; i++)
            {
                double currHeight = coef * y[i];
                PyramidSegment pyramidSegment = new PyramidSegment(currHeight, coef * height[i], this.ExplodeOffset, this, i == this.ExplodeIndex ? true : false);
                pyramidSegment.Item = enumerator.Current;
                pyramidSegment.XData = xValues[i];
                pyramidSegment.YData = Math.Abs(YValues[i]);
                this.Segments.Add(pyramidSegment);

                if (AdornmentsInfo != null)
                {
                    Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], 0, double.IsNaN(currHeight) ? 1 - height[i] / 2 : currHeight + coef * height[i] / 2));
                }
                enumerator.MoveNext();
            }
        }

        /// <summary>
        /// To get the SurfaceHeight for PyramidSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public static double GetSurfaceHeight(double y, double surface)
        {
            double r1, r2;
            if (ChartMath.SolveQuadraticEquation(1, 2 * y, -surface, out r1, out r2))
            {
                return Math.Max(r1, r2);
            }
            return double.NaN;
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new PyramidSeries() { PyramidMode = this.PyramidMode });
        }

        #endregion
    }
}
