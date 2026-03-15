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
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StackingColumnSeries is typically preferred in cases of multiple series of type <see cref="ColumnSeries"/>.
    /// Each series is then stacked vertically one above the other.
    /// If there exists only single series, it will resemble like a simple <see cref="ColumnSeries"/> chart.
    /// </summary>
    /// <seealso cref="StackingColumnSegment"/>
    /// <seealso cref="StackingBarSeries"/>
    /// <seealso cref="StackingAreaSeries"/>
    /// <seealso cref="ColumnSeries"/>
    [ClassReference(IsReviewed = false)]
    public class StackingColumnSeries : StackingSeriesBase, ISegmentSelectable
    {
        #region Properties

        protected override bool IsStacked
        {
            get
            {
                return true;
            }
        }

        protected internal override bool IsSideBySide
        {
            get { return true; }
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
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(StackingColumnSeries), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets DataTemplate for segment
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        
     /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
     /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(StackingColumnSeries), new PropertyMetadata(null));


        #endregion

        #region constructor

        #endregion

        #region methods

        /// <summary>
        /// creates the segments of StackingColumnSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = new List<double>();
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            double x1, x2, y1, y2;
            double Origin = this.ActualXAxis.Origin;
            double median = sbsInfo.Delta / 2;
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
                            StackingColumnSegment columnSegment = new StackingColumnSegment(x1, y1, x2, y2, this);
                            columnSegment.XData = xValues[i];
                            columnSegment.YData = ActualSeriesYValues[0][i];
                            columnSegment.Item = ActualData[i];
                            Segments.Add(columnSegment);
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
#if !WINDOWS_PHONE
            string path = IsActualTransposed ? "(Canvas.Left)" : "(Canvas.Top)";
#else
            object path = IsActualTransposed ? Canvas.LeftProperty : Canvas.TopProperty;
#endif
            string scalePath = IsActualTransposed ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";
            string adornTransPath = IsActualTransposed ? "(UIElement.RenderTransform).(TranslateTransform.X)" : "(UIElement.RenderTransform).(TranslateTransform.Y)";
            double startHeight = Area.ValueToPoint(Area.InternalSecondaryAxis, 0);
            foreach (ChartSegment segment in Segments)
            {
                var element = (FrameworkElement)segment.GetRenderedVisual();

                double elementSize = IsActualTransposed ? element.Width : element.Height;
                if (!double.IsNaN(elementSize))
                {
                    element.RenderTransform = new ScaleTransform();
                    double canvasSize = IsActualTransposed ? Canvas.GetLeft(element) : Canvas.GetTop(element);
                    DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame.Value = startHeight;
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
                        keyFrame1.Value = (YValues[i] > 0) ? (elementSize * 10) / 100 : -(elementSize * 10) / 100;
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
                element.BeginAnimation(Canvas.TopProperty, null);
                element.BeginAnimation(Canvas.LeftProperty, null);
            }

            //storyBoard.Completed -= sb_Completed;
        }

#endif

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingColumnSeries() { CustomTemplate = this.CustomTemplate, SegmentSelectionBrush = this.SegmentSelectionBrush });
        }

        #endregion
    }
}
