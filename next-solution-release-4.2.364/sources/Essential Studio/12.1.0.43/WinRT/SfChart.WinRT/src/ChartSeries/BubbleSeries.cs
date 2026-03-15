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
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Bubble series diplays a set of circular symbols of varying size.
    /// </summary>
    /// <remarks>
    /// BubbleSeries requires an additional data binding parameter <see cref="BubbleSeries.Size"/> in addition to X,Y parameters.
    /// The size of each bubble depends on the size value given in the data point.<see cref="BubbleSeries.MinimumRadius"/> and <see cref="MaximumRadius"/> properties can be used to 
    /// control the minimum and maximum radius of the symbols.
    /// </remarks>
    /// <seealso cref="BubbleSegment"/>
    /// <seealso cref="ScatterSeries"/>
    [ClassReference(IsReviewed = false)]
    public class BubbleSeries : XyDataSeries, ISegmentSelectable
    {

        #region fields

        private List<double> sizeValues;

        #endregion

        #region properties

        internal override bool IsMultipleYPathRequired
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
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(BubbleSeries), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets minimum radius.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double MinimumRadius
        {
            get { return (double)GetValue(MinimumRadiusProperty); }
            set { SetValue(MinimumRadiusProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxWidth.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty MinimumRadiusProperty =
            DependencyProperty.Register("MinimumRadius", typeof(double), typeof(BubbleSeries), new PropertyMetadata(10d));

        /// <summary>
        /// Gets or Sets maximum radius.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double MaximumRadius
        {
            get { return (double)GetValue(MaximumRadiusProperty); }
            set { SetValue(MaximumRadiusProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumRadiusProperty =
            DependencyProperty.Register("MaximumRadius", typeof(double), typeof(BubbleSeries), new PropertyMetadata(30d));
        
        /// <summary>
        /// Gets or Sets the property path to retrieve data which determines the size of the bubble.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Size
        {
            get { return (string)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(string), typeof(BubbleSeries), new PropertyMetadata(null));


        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for BubbleSeries
        /// </summary>
        public BubbleSeries()
        {
            sizeValues = new List<double>();
        }

        #endregion

        #region methods
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            YValues.Clear();
            sizeValues.Clear();
            GeneratePoints(new string[] { YBindingPath, Size }, YValues, sizeValues);
            this.UpdateArea();            
        }

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { YBindingPath, Size }, YValues, sizeValues);
        }

        /// <summary>
        /// Creates the Segments of BubbleSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            double maximumSize = 0d, segmentRadius=0d;
            double xIndexValues = 0d;
            List<double> xValues = ActualXValues as List<double>;
            if (IsIndexed || xValues == null)
            {
                xValues = xValues != null ? (from val in (xValues) select (xIndexValues++)).ToList()
                      : (from val in (ActualXValues as List<string>) select (xIndexValues++)).ToList();
            }

            maximumSize = (from val in sizeValues select val).Max();
            double minRadius = this.MinimumRadius;
            double maxradius = this.MaximumRadius;
            double radius = maxradius - minRadius;

            if (xValues != null)
            {
                ClearUnUsedSegments(this.DataCount);
                ClearUnUsedAdornments(this.DataCount);

                for (int i = 0; i < this.DataCount; i++)
                {
                    segmentRadius = minRadius + radius * Math.Abs(sizeValues[i] / maximumSize);
                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(xValues[i], YValues[i]);
                        (Segments[i] as BubbleSegment).SegmentRadius = segmentRadius;
                        (Segments[i] as BubbleSegment).Item = ActualData[i];
                    }
                    else
                    {
                        BubbleSegment bubbleSegment = new BubbleSegment(xValues[i], YValues[i], segmentRadius, this);
                        bubbleSegment.Item = ActualData[i];
                        bubbleSegment.Size = sizeValues[i];
                        Segments.Add(bubbleSegment);
                    }
                    if (AdornmentsInfo != null)
                        AddAdornmentAtXY(xValues[i], YValues[i], i);
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }            
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new BubbleSeries() { MaximumRadius = this.MaximumRadius, MinimumRadius = this.MinimumRadius, SegmentSelectionBrush = this.SegmentSelectionBrush, Size = this.Size });
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

            foreach (BubbleSegment segment in this.Segments)
            {
                int randomValue = rand.Next(0, 20);
                TimeSpan beginTime = TimeSpan.FromMilliseconds(randomValue * 20);
                var element = (FrameworkElement)segment.GetRenderedVisual();
                element.RenderTransform = null;
                element.RenderTransform = new ScaleTransform() { ScaleY = 0, ScaleX = 0 };
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                double top = Canvas.GetTop(element);
                double left = Canvas.GetLeft(element);
                DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrames.BeginTime = beginTime;
                SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                keyFrame.Value = 0;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame();
                //keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration.TotalMilliseconds - (randomValue * (2 * AnimationDuration.TotalMilliseconds) / 100)));
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100));
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
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100));

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

                    DoubleAnimation animation = new DoubleAnimation() { To = 1, From = 0, BeginTime = TimeSpan.FromSeconds(beginTime.TotalSeconds + (beginTime.Seconds * 90) / 100), Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100) };

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

        #endregion
    }
}
