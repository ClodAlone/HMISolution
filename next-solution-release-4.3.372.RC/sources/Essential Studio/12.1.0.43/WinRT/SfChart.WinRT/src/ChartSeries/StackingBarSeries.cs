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
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    ///<summary>
    /// StackingBarSeries is typically preferred in cases of multiple series of type <see cref="BarSeries"/>.
    /// Each Series is then stacked horizontally side by side to each other.
    /// If there exists olyn single series, it will resemble like a simple <see cref="BarSeries"/> chart.
    /// </summary>
    /// <seealso cref="StackingBarSegment"/>
    /// <seealso cref="StackingColumnSeries"/>
    /// <seealso cref="StackingAreaSeries"/>
    /// <seealso cref="BarSeries"/>
    [ClassReference(IsReviewed = false)]
    public class StackingBarSeries : StackingSeriesBase, ISegmentSelectable
    {
        #region properties

        

        protected override bool IsStacked
        {
            get
            {
                return true;
            }
        }
        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

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
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(StackingBarSeries), new PropertyMetadata(null));

        #endregion

        #region Constructor
        public StackingBarSeries()
        {
           IsActualTransposed = true;
        }
        #endregion

        #region methods

        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = !val;
        }

        /// <summary>
        /// Creates the segments of StackingBarSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = new List<double>();
            double Origin = this.ActualXAxis.Origin;
            xValues = GetXValues();
            var stackingValues = GetCumulativeStackValues(this);
            if (stackingValues != null)
            {
                YRangeStartValues = stackingValues.StartValues;
                YRangeEndValues = stackingValues.EndValues;
                if (YRangeStartValues == null)
                {
                    YRangeStartValues = (from val in xValues select Origin).ToList();
                }

                if (xValues != null)
                {
                    ClearUnUsedSegments(this.DataCount);
                    ClearUnUsedAdornments(this.DataCount);
                    DoubleRange sbsInfo = this.GetSideBySideInfo(this);

                    double median = sbsInfo.Delta / 2;
                    for (int i = 0; i < DataCount; i++)
                    {
                        x1 = xValues[i] + sbsInfo.Start;
                        x2 = xValues[i] + sbsInfo.End;
                        y2 = YRangeStartValues[i];
                        y1 = YRangeEndValues[i];

                        if (i < Segments.Count)
                        {
                            (Segments[i]).SetData(x1, y1, x2, y2);
                        }
                        else
                        {
                            StackingBarSegment barSegment = new StackingBarSegment(x1, y1, x2, y2, this);
                            barSegment.XData = xValues[i];
                            barSegment.YData = ActualSeriesYValues[0][i];
                            barSegment.Item = ActualData[i];
                            Segments.Add(barSegment);
                        }
                        if (AdornmentsInfo != null)
                        {
                            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median);
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median);
                            else
                                AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median);
                        }
                    }
                    if (ShowEmptyPoints)
                        UpdateEmptyPointSegments(xValues);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingBarSeries() { SegmentSelectionBrush = this.SegmentSelectionBrush });
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        private Storyboard sb;

        internal override void Animate()
        {
            int i = 0;
            if (sb != null)
                sb.Stop();
            sb = new Storyboard();
            double startWidth = Area.ValueToPoint(Area.InternalSecondaryAxis, 0);
#if !WINDOWS_PHONE
            string path = IsActualTransposed ? "(Canvas.Left)" : "(Canvas.Top)";
#else
            object path = IsActualTransposed ? Canvas.LeftProperty : Canvas.TopProperty;
#endif
            string scalePath = IsActualTransposed ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";
            string adornTransPath = IsActualTransposed ? "(UIElement.RenderTransform).(TranslateTransform.X)" : "(UIElement.RenderTransform).(TranslateTransform.Y)";
            foreach (ChartSegment segment in Segments)
            {
                var element = (FrameworkElement)segment.GetRenderedVisual();

                double elementSize = IsActualTransposed ? element.Width : element.Height;
                if (!double.IsNaN(elementSize))
                {
                    double canvasSize = IsActualTransposed ? Canvas.GetLeft(element) : Canvas.GetTop(element);
                    element.RenderTransform = new ScaleTransform();
                    DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame.Value = startWidth;
                    keyFrames.KeyFrames.Add(keyFrame);

                    keyFrame = new SplineDoubleKeyFrame();
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);
                    keyFrame.Value = canvasSize;
                    KeySpline keySpline = new KeySpline();
                    keySpline.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline.ControlPoint2 = new Point(0.67, 0.95);
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                    Storyboard.SetTargetProperty(keyFrames, path);
#else
                    Storyboard.SetTargetProperty(keyFrames, new PropertyPath(path));
#endif
                    Storyboard.SetTarget(keyFrames, element);
                    sb.Children.Add(keyFrames);

                    DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame1.Value = 0;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                    KeySpline keySpline1 = new KeySpline();
                    keySpline1.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline1.ControlPoint2 = new Point(0.67, 0.95);
                    keyFrame1.KeySpline = keySpline1;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1.Value = 1;
#if !WINDOWS_PHONE
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, scalePath);
#else
                    Storyboard.SetTargetProperty(keyFrames1, new PropertyPath(scalePath));
#endif
                    Storyboard.SetTarget(keyFrames1, element);
                    sb.Children.Add(keyFrames1);
                    if (this.AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                    {
                        FrameworkElement label = this.AdornmentsInfo.LabelPresenters[i];
                        label.RenderTransform = new TranslateTransform() { };
                        keyFrames1 = new DoubleAnimationUsingKeyFrames();
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime =
                            KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100));
                        //keyFrame1.Value = (YValues[i] > 0) ? -(elementWidth * 10) / 100 : (elementWidth * 10) / 100;
                        keyFrame1.Value = -(elementSize * 10) / 100;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                        keySpline1 = new KeySpline();
                        keySpline1.ControlPoint1 = new Point(0.64, 0.84);
                        keySpline1.ControlPoint2 = new Point(0.67, 0.95);
                        keyFrame1.KeySpline = keySpline1;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1.Value = 0;
#if !WINDOWS_PHONE
                        keyFrames1.EnableDependentAnimation = true;
                        Storyboard.SetTargetProperty(keyFrames1, adornTransPath);
#else
                        Storyboard.SetTargetProperty(keyFrames1,
                                                     new PropertyPath(
                                                         adornTransPath));
#endif
                        Storyboard.SetTarget(keyFrames1, label);
                        sb.Children.Add(keyFrames1);
                        label.Opacity = 0;

                        DoubleAnimation animation = new DoubleAnimation()
                        {
                            From = 0,
                            To = 1,
                            Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 20) / 100),
                            BeginTime = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100)
                        };

                        Storyboard.SetTarget(animation, label);
#if WINDOWS_PHONE
                        Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
#else
                        Storyboard.SetTargetProperty(animation, "(UIElement.Opacity)");
#endif
                        sb.Children.Add(animation);
                    }
#if WPF
                    sb.Completed += sb_Completed;
#endif
                   
                    i++;
                }
            }
            sb.Begin();
        }
#if WPF

        void sb_Completed(object sender, EventArgs e)
        {
            Storyboard storyBoard = (Storyboard)(sender as ClockGroup).Timeline;
            foreach (Timeline timeline in storyBoard.Children)
            {
                FrameworkElement element = Storyboard.GetTarget(timeline) as FrameworkElement;
                element.BeginAnimation(Canvas.LeftProperty, null);
                element.BeginAnimation(Canvas.TopProperty, null);
            }

            //storyBoard.Completed -= sb_Completed;
        }

#endif
        #endregion
    }
}
