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
using System.Windows.Media.Animation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ScatterSeries displays data points as set of circular symbols. 
    /// Values are being represented by the position of the symbols on the chart.    
    /// </summary>
    /// <remarks>
    /// ScatterSeries are typically used to compare aggregated data across categories.
    /// </remarks>
    /// <seealso cref="ScatterSegment"/>
    /// <seealso cref="BubbleSeries"/>
    [ClassReference(IsReviewed = false)]
    public class ScatterSeries : XyDataSeries, ISegmentSelectable
    {
        #region Properties

        /// <summary>
        /// Gets or Sets selection brush
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for SegmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(ScatterSeries), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or Sets scatter segment’s width.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ScatterWidth
        {
            get { return (double)GetValue(ScatterWidthProperty); }
            set { SetValue(ScatterWidthProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ScatterWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
            DependencyProperty.Register("ScatterWidth", typeof(double), typeof(ScatterSeries), new PropertyMetadata(20d, OnScatterWidthChanged));

        /// <summary>
        /// Gets or Sets scatter segment’s height.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ScatterHeight
        {
            get { return (double)GetValue(ScatterHeightProperty); }
            set { SetValue(ScatterHeightProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
            DependencyProperty.Register("ScatterHeight", typeof(double), typeof(ScatterSeries), new PropertyMetadata(20d, OnScatterHeightChanged));

       
        #endregion

        #region constructor

        #endregion

        #region methods

        private static void OnScatterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterSeries series = d as ScatterSeries;

            foreach (ChartSegment segment in series.Segments)
            {
                (segment as ScatterSegment).ScatterHeight = series.ScatterHeight;
            }
        }

        private static void OnScatterWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterSeries series = d as ScatterSeries;

            foreach (ChartSegment segment in series.Segments)
            {
                (segment as ScatterSegment).ScatterWidth = series.ScatterWidth;
            }
        }

        /// <summary>
        /// Creates the segments of ScatterSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            
            if (xValues != null)
            {
                ClearUnUsedSegments(this.DataCount);
                ClearUnUsedAdornments(this.DataCount);

                for (int i = 0; i < this.DataCount; i++)
                {
                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(xValues[i], YValues[i]);
                    }
                    else
                    {
                        ScatterSegment scatterSegment = new ScatterSegment(xValues[i], YValues[i], this);
                        scatterSegment.YData = YValues[i];
                        scatterSegment.XData = xValues[i];
                        scatterSegment.Item = ActualData[i];
                        Segments.Add(scatterSegment);
                    }
                    if (AdornmentsInfo != null)
                        AddAdornments(xValues[i], YValues[i], i);
                }
                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
        }

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
                Adornments.Add(this.CreateAdornment(this, adornX, adornY,adornX, adornY));
            }
            Adornments[i].Item = ActualData[i];
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }
        private Storyboard sb;

        internal override void Animate()
        {
            int i = 0;
            Random rand = new Random();
            if (sb != null)
                sb.Stop();
            sb = new Storyboard();
            foreach (ScatterSegment segment in this.Segments)
            {
                int randomValue = rand.Next(0, 50);
                TimeSpan beginTime = TimeSpan.FromMilliseconds(randomValue * 20);
                var element = (FrameworkElement)segment.GetRenderedVisual();
                element.RenderTransform = new ScaleTransform() { ScaleY = 0, ScaleX = 0 };
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrames.BeginTime = beginTime;
                SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                keyFrame.Value = 0;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame();
                //keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration.TotalMilliseconds - (randomValue * (2 * AnimationDuration.TotalMilliseconds) / 100)));
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 30) / 100));

                KeySpline keySpline = new KeySpline();
                keySpline.ControlPoint1 = new Point(0.64, 0.84);
                keySpline.ControlPoint2 = new Point(0.67, 0.95);
                keyFrame.KeySpline = keySpline;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame.Value = 1;
#if !WINDOWS_PHONE
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
#else
                Storyboard.SetTargetProperty(keyFrames,
                                             new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif
                Storyboard.SetTarget(keyFrames, element);
                sb.Children.Add(keyFrames);

                keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrames.BeginTime = beginTime;
                keyFrame = new SplineDoubleKeyFrame();
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                keyFrame.Value = 0;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame();
                //keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration.TotalMilliseconds - (randomValue * (2 * AnimationDuration.TotalMilliseconds) / 100)));
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 30) / 100));


                keySpline = new KeySpline();
                keySpline.ControlPoint1 = new Point(0.64, 0.84);
                keySpline.ControlPoint2 = new Point(0.67, 0.95);
                keyFrame.KeySpline = keySpline;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame.Value = 1;
#if !WINDOWS_PHONE
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
#else
                Storyboard.SetTargetProperty(keyFrames,
                                             new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#endif
                Storyboard.SetTarget(keyFrames, element);
                sb.Children.Add(keyFrames);

                if (AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                {
                    UIElement label = this.AdornmentsInfo.LabelPresenters[i];
                    label.Opacity = 0;

                    DoubleAnimation animation = new DoubleAnimation() { To = 1, From = 0, BeginTime = TimeSpan.FromSeconds(beginTime.TotalSeconds + (beginTime.Seconds * 90) / 100), Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100) };

#if !WINDOWS_PHONE
                    Storyboard.SetTargetProperty(animation, "ContentControl.Opacity");
#else
                    Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
#endif
                    Storyboard.SetTarget(animation, label);
                    sb.Children.Add(animation);
                }
                i++;
            }
            sb.Begin();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new ScatterSeries() { ScatterHeight = ScatterHeight, ScatterWidth = ScatterWidth, SegmentSelectionBrush = SegmentSelectionBrush });
        }

        #endregion
    }

}