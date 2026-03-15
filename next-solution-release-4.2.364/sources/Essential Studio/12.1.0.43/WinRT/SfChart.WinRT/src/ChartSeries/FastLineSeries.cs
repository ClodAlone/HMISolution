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
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastLineSeries is another version of LineSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// FastLineSeries uses polyline and is capable of rendering large amount of data in a milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineSegment"/>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="LineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastLineSeries: XyDataSeries
    {
        #region

        private FastLineSegment Segment { get; set; }

        bool isAdornmentPending;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets stroke dash array for series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeDashArray.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(FastLineSeries), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets stroke dash offset for series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(FastLineSeries), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or Sets stroke dash cap  for series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeDashCapProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeDashCap.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(FastLineSeries), new PropertyMetadata(PenLineCap.Flat));
        #endregion
        #region ctor

        #endregion

        #region methods

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
        /// Creates the segments of FastLineSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            ClearUnUsedAdornments(this.DataCount);

            if (Segment == null || Segments.Count == 0)
            {
                FastLineSegment segment = new FastLineSegment(ActualXValues as IList<double>, YValues, this);
                Segment = segment;
                Segments.Add(segment);
            }
            else if (ActualXValues != null)
            {
                Segment.SetData(ActualXValues as IList<double>, YValues);
                (Segment as FastLineSegment).SetRange();
            }

            isAdornmentPending = true;
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastLineSeries());
        }

#if !WPF
        private RectAnimation animation;

        internal override bool GetAnimationIsActive()
        {
            return animation != null && animation.IsActive;
        }
#else
        RectangleGeometry geometry;
        internal override bool GetAnimationIsActive()
        {
            return geometry != null;
        }
#endif

        internal override void Animate()
        {
            var seriesRect = Area.SeriesClipRect;
#if WPF
            geometry = new RectangleGeometry();
            SeriesRootPanel.Clip = geometry;
            System.Windows.Media.Animation.RectAnimation animation = new System.Windows.Media.Animation.RectAnimation()
#else
            if (animation != null)
            {
                animation.Stop();
            }
            animation = new RectAnimation()
#endif
            {
                From = (IsActualTransposed) ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height),
                To = (IsActualTransposed) ? new Rect(0, seriesRect.Y, 0, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height),
#if NETFX_CORE || SILVERLIGHT
                Duration = AnimationDuration.TotalSeconds == 1 ? TimeSpan.FromSeconds(0.4) : AnimationDuration
#else
                Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds + (AnimationDuration.TotalSeconds * 20) / 100)
#endif
            };
#if WPF
            Rect start = IsActualTransposed ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height);
            Rect end = IsActualTransposed ? new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height);
            RectAnimationUsingKeyFrames keyFrames = new RectAnimationUsingKeyFrames();
            SplineRectKeyFrame keyFrame = new SplineRectKeyFrame(start, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            keyFrames.KeyFrames.Add(keyFrame);
            keyFrame = new SplineRectKeyFrame(end, KeyTime.FromTimeSpan(AnimationDuration));
            keyFrames.KeyFrames.Add(keyFrame);
            keyFrame.KeySpline = new KeySpline(0.65, 0.84, 0.67, 0.95);
            AnimationClock clock = keyFrames.CreateClock();
            clock.Completed += animationclock_Completed;
            geometry.BeginAnimation(RectangleGeometry.RectProperty, keyFrames);
#else
            animation.SetTarget(SeriesRootPanel);
            animation.Begin();
#endif
            if (this.AdornmentsInfo != null)
            {
                Storyboard sb = new Storyboard();
                double secondsPerPoint = AnimationDuration.TotalSeconds / YValues.Count;
                int i = 0;
                foreach (ContentControl label in this.AdornmentsInfo.LabelPresenters)
                {
                    label.RenderTransform = new ScaleTransform() { ScaleX = 0.6, ScaleY = 0.6 };
                    label.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.6,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };

#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#endif
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.6,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };
#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    i++;
                }
                sb.Begin();
            }

        }

#if WPF
        void animationclock_Completed(object sender, EventArgs e)
        {
            (sender as AnimationClock).Completed -= animationclock_Completed;
            geometry = null;
        }
#endif

        #endregion
    }
}
