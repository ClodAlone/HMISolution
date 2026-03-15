#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Enum value for AnimationOptions mode
    /// </summary>
    public enum AnimationOptions
    {
        /// <summary>
        /// Move the segments from the Below to Top
        /// </summary>
        Top,

        /// <summary>
        /// Move the Segments from Left to Right
        /// </summary>
        Left,

        /// <summary>
        /// Move the Segments from the Right to Left
        /// </summary>
        Right,

        /// <summary>
        /// Move the segments from the Top to Bottom
        /// </summary>
        Bottom,

        /// <summary>
        /// Rotate the segments
        /// </summary>
        Rotate,

        /// <summary>
        /// change the Opacity of the segments
        /// </summary>
        Fade,

        /// <summary>
        /// Perform the scalling of segments.
        /// </summary>
        Scaling,

        /// <summary>
        /// Perform the  Interactive Animation during Load, Update and Mouse events of Segments
        /// </summary>
        Interactive
    }
    /// <summary>
    /// Class implementation for ChartAnimation
    /// </summary>
    public class ChartAnimation : DependencyObject,IDisposable
    {
        /// <summary>
        /// Idenfities Animated Chart Series
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(ChartAnimation), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the storyboard contains all the double animation values.
        /// </summary>
        internal Storyboard Storyboard
        {
            get;
            set;
        }

        internal bool IsRefreshAnimation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ChartSeries.
        /// </summary>
        /// <value>The ChartSeries.</value>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Called when instance created for ChartAnimation with single arguments 
        /// </summary>
        /// <param name="series"></param>
        public ChartAnimation(ChartSeries series)
        {
            this.Storyboard = new Storyboard();
            this.Series = series;
        }

        /// <summary>
        /// called when instance created for ChartAnimation
        /// </summary>
        public ChartAnimation()
        {
            this.IsRefreshAnimation = true;
            this.Storyboard = new Storyboard();
            this.Series = new ChartSeries();
        }

        /// <summary>
        /// Stop the Animation.
        /// </summary>
        internal void StopAnimation()
        {
            if (this.Series != null)
            {
                foreach (Segment seg in this.Series.Segments)
                {
                    seg.AnimatingStoryBoard.Stop();
                    seg.MouseAnimation.Stop();
                }
            }
        }

        /// <summary>
        /// Start the Animation
        /// </summary>
        internal void StartAnimation()
        {
            if (this.Series != null)
            {
                foreach (Segment seg in this.Series.Segments)
                {
                    seg.AnimatingStoryBoard.Begin();
                }
            }
        }

        /// <summary>
        /// Generate the Opacity animation to hide all the segments first and animation based on the requirements.
        /// </summary>
        internal void AnimateSeries(bool Refreshanimate)
        {
            this.IsRefreshAnimation = Refreshanimate;
            Storyboard sb = new Storyboard();
            foreach (UIElement element in this.Series.Presenter.Children)
            {
                element.Opacity = 0d;
                ContentPresenter presenter = element as ContentPresenter;
                if (VisualTreeHelper.GetChildrenCount(presenter) > 0)
                {
                    Canvas obj = VisualTreeHelper.GetChild(presenter, 0) as Canvas;
                    Segment segment = presenter.Content as Segment;
                    DoubleAnimation da = new DoubleAnimation();
                    da = getDoubleAnimation(segment, element, new PropertyPath(UIElement.OpacityProperty), (this.Series.IsDataModified && !this.IsRefreshAnimation) ? 1d : 0d, 1d);
                    sb.Children.Add(da);
                    da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0));
                }
            }

            sb.Begin();
            this.StopAnimation();
            //this.Storyboard.Stop();
            //this.Storyboard.Children.Clear();
            foreach (UIElement element in this.Series.Presenter.Children)
            {
                GenerateSeriesAnimation(element as ContentPresenter, Series.Presenter.TotalSize, this.Storyboard);
            }

            this.Series.IsDataModified = false;

            this.StartAnimation();
            //this.Storyboard.Begin();
        }

        /// <summary>
        /// Generate the Animation for each segments and add to the Story board
        /// </summary>
        /// <param name="presenter">series presenter</param>
        /// <param name="avilableSize">total size</param>
        /// <param name="sb">story board</param>
        private void GenerateSeriesAnimation(ContentPresenter presenter, Size avilableSize, Storyboard sb)
        {
            bool hasKey;
            double oldValue;
            ChartPieSegment oldSegmentValue;
            LineSegment oldlineSegmentValue;

            int count = VisualTreeHelper.GetChildrenCount(presenter);
            if (count > 0)
            {
                Canvas obj = VisualTreeHelper.GetChild(presenter, 0) as Canvas;
                Segment segment = presenter.Content as Segment;
                sb = segment.AnimatingStoryBoard;
                sb.Children.Clear();
                if (segment.MouseAnimation.Children.Count > 0)
                {
                    segment.MouseAnimation.Children.Clear();
                }

                sb.FillBehavior = FillBehavior.Stop;
                foreach (FrameworkElement sh in obj.Children)
                {
                    Timeline danimation = null;
                    DoubleAnimation mouseAnimation = null;
                    switch (Series.AnimateOption)
                    {
                        case AnimationOptions.Bottom:
                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.TopProperty), avilableSize.Height, Canvas.GetTop(sh));
                            break;
                        case AnimationOptions.Top:
                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.TopProperty), 0d - avilableSize.Height, Canvas.GetTop(sh));
                            break;
                        case AnimationOptions.Right:
                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.LeftProperty), avilableSize.Width, Canvas.GetLeft(sh));
                            break;
                        case AnimationOptions.Left:
                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.LeftProperty), 0d - (avilableSize.Width / 2), Canvas.GetLeft(sh));
                            break;
                        case AnimationOptions.Rotate:
                            RotateTransform t1 = new RotateTransform();
                            t1.CenterX = double.IsNaN(sh.Width) ? (sh is Path || sh is Polyline ? avilableSize.Width / 2d : sh.DesiredSize.Width / 2d) : sh.Width / 2d;
                            t1.CenterY = double.IsNaN(sh.Height) ? (sh is Path || sh is Polyline ? avilableSize.Height / 2d : sh.DesiredSize.Height / 2d) : sh.Height / 2d;
                            sh.RenderTransform = t1;
                            danimation = getDoubleAnimation(segment, t1, new PropertyPath(RotateTransform.AngleProperty), 0d, 360d);
                            break;
                        case AnimationOptions.Fade:
                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Shape.OpacityProperty), 0d, 1d);
                            break;
                        case AnimationOptions.Scaling:
                            ScaleTransform t = new ScaleTransform();
                            t.CenterX = double.IsNaN(sh.Width) ? (sh is Path || sh is Polyline ? avilableSize.Width / 2d : sh.DesiredSize.Width / 2d) : sh.Width / 2d;
                            t.CenterY = double.IsNaN(sh.Height) ? (sh is Path || sh is Polyline ? avilableSize.Height / 2d : sh.DesiredSize.Height / 2d) : sh.Height / 2d;
                            sh.RenderTransform = t;
                            danimation = getDoubleAnimation(segment, t, new PropertyPath(ScaleTransform.ScaleXProperty), 0d, 1d);
                            DoubleAnimation d2 = getDoubleAnimation(segment, t, new PropertyPath(ScaleTransform.ScaleYProperty), 0d, 1d);
                            sb.Children.Add(d2);
                            break;
                        case AnimationOptions.Interactive:
                            if (Series.EnableEffects)
                            {
                                danimation = getDoubleAnimation(segment, sh, new PropertyPath(Shape.OpacityProperty), 0d, 1d);
                                break;
                            }

                            if (segment is ChartAdornment)
                            {
                                danimation = getDoubleAnimation(segment, sh, new PropertyPath(Shape.OpacityProperty), 0d, 1d);
                            }
                            else
                            {
                                #region Interactive animation On Load, DataModify and Mouse events
                                switch (Series.Type)
                                {
                                    case ChartTypes.Pie:
                                    case ChartTypes.Doughnut:
                                        if (segment.Series.Segments.Count == 1)
                                        {
                                            danimation = getDoubleAnimation(segment, sh, new PropertyPath(Shape.OpacityProperty), 0d, 1d);
                                            break;
                                        }

                                        ChartPieSegment PiSeg = segment as ChartPieSegment;
                                        bool IsDoughnut = segment is ChartDoughnutSegment;
                                        Point startPoint = ChartMath.GeneralPointRotation(PiSeg.CenterPoint, PiSeg.startPoint, PiSeg.StartAngle * -1);
                                        PathGeometry pathGeo=(sh as Path).Data as PathGeometry;

                                        if (!PiSeg.DataPoint.Visible)
                                        {
                                            continue;
                                        }

                                        #region Initialize Mouse Animation
                                        Point explodedPoint = ChartMath.GeneralPointRotation(PiSeg.CenterPoint, new Point(PiSeg.CenterPoint.X + ChartPieType.GetExplodeRadius(segment.Series) * (PiSeg.IsExploded ? -1 : 1), PiSeg.CenterPoint.Y), PiSeg.AngleOfSlice);
                                        mouseAnimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.LeftProperty), 0, explodedPoint.X - PiSeg.CenterPoint.X);
                                        mouseAnimation.EasingFunction = new BounceEase();
                                        mouseAnimation.BeginTime = new TimeSpan(0);
                                        mouseAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                                        mouseAnimation.Completed += new EventHandler(mouseAnimation1_Completed);
                                        DoubleAnimation mouseAnimation1 = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.TopProperty), 0, explodedPoint.Y - PiSeg.CenterPoint.Y);
                                        mouseAnimation1.EasingFunction = new BounceEase();
                                        mouseAnimation1.BeginTime = new TimeSpan(0);
                                        mouseAnimation1.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                                        mouseAnimation1.Completed += new EventHandler(mouseAnimation1_Completed);

                                        segment.MouseAnimation.Children.Add(mouseAnimation);
                                        segment.MouseAnimation.Children.Add(mouseAnimation1);
                                        #endregion
                                        
                                        hasKey = Series.OldAnimatedValues.ContainsKey(segment.DataPoint.X);
                                        oldSegmentValue = (hasKey && !this.IsRefreshAnimation) ? (ChartPieSegment)Series.OldAnimatedValues[segment.DataPoint.X] : PiSeg;
                                        if (hasKey && !this.IsRefreshAnimation)
                                        {
                                            danimation = getPointAnimationUsingKeyFrames(segment, PiSeg.arcseg, new PropertyPath(ArcSegment.PointProperty), oldSegmentValue.endPoint, PiSeg.endPoint, oldSegmentValue.EndAngle, PiSeg.EndAngle, false);

                                            if (!IsDoughnut)
                                            {
                                                PointAnimationUsingKeyFrames lineAnimation = getPointAnimationUsingKeyFrames(segment, PiSeg.pielineseg, new PropertyPath(System.Windows.Media.LineSegment.PointProperty), oldSegmentValue.startPoint, PiSeg.startPoint, oldSegmentValue.StartAngle, PiSeg.StartAngle, false);
                                                sb.Children.Add(lineAnimation);
                                            }
                                            else
                                            {
                                                PointAnimationUsingKeyFrames lineAnimation = getPointAnimationUsingKeyFrames(segment, PiSeg.lineseg, new PropertyPath(System.Windows.Media.LineSegment.PointProperty), oldSegmentValue.endDPoint, PiSeg.endDPoint, oldSegmentValue.EndAngle, PiSeg.EndAngle, false);
                                                sb.Children.Add(lineAnimation);

                                                PointAnimationUsingKeyFrames lineAnimation1 = getPointAnimationUsingKeyFrames(segment, PiSeg.doughnutArgSeg, new PropertyPath(ArcSegment.PointProperty), oldSegmentValue.startDPoint, PiSeg.startDPoint, oldSegmentValue.StartAngle, PiSeg.StartAngle, false);
                                                sb.Children.Add(lineAnimation1);

                                                PointAnimationUsingKeyFrames lineAnimation2 = getPointAnimationUsingKeyFrames(segment, pathGeo.Figures[0], new PropertyPath(PathFigure.StartPointProperty), oldSegmentValue.startPoint, PiSeg.startPoint, oldSegmentValue.StartAngle, PiSeg.StartAngle, false);
                                                sb.Children.Add(lineAnimation2);
                                            }

                                        }
                                        else
                                        {
                                            danimation = getPointAnimationUsingKeyFrames(segment, PiSeg.arcseg, new PropertyPath(ArcSegment.PointProperty), PiSeg.startPoint, PiSeg.endPoint, PiSeg.StartAngle, PiSeg.EndAngle, true);

                                            if (IsDoughnut)
                                            {
                                                PointAnimationUsingKeyFrames d1 = getPointAnimationUsingKeyFrames(segment, PiSeg.lineseg, new PropertyPath(System.Windows.Media.LineSegment.PointProperty), PiSeg.startDPoint, PiSeg.endDPoint, PiSeg.StartAngle, PiSeg.EndAngle, true);
                                                sb.Children.Add(d1);
                                            }
                                        }

                                        if (hasKey)
                                        {
                                            Series.OldAnimatedValues[segment.DataPoint.X] = PiSeg;
                                        }
                                        else
                                        {
                                            Series.OldAnimatedValues.Add(segment.DataPoint.X, PiSeg);
                                        }

                                        break;

                                    case ChartTypes.Line:
                                        #region Initialize Mouse Animation
                                        mouseAnimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.TopProperty), Canvas.GetTop(sh) - 20, Canvas.GetTop(sh));
                                        mouseAnimation.EasingFunction = new BounceEase();
                                        mouseAnimation.BeginTime = new TimeSpan(0);
                                        mouseAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                                        segment.MouseAnimation.Children.Add(mouseAnimation);
                                        #endregion
                                        LineSegment lineSeg = segment as LineSegment;
                                        hasKey = Series.OldAnimatedValues.ContainsKey(segment.DataPoint.X);
                                        oldlineSegmentValue = (hasKey && !this.IsRefreshAnimation) ? (LineSegment)Series.OldAnimatedValues[segment.DataPoint.X] : lineSeg;
                                        bool isSame = oldlineSegmentValue.Equals(lineSeg);
                                        danimation = getDoubleAnimation(segment, lineSeg, new PropertyPath(LineSegment.X1Property), oldlineSegmentValue.X1, lineSeg.X1);
                                        DoubleAnimation danimation1 = getDoubleAnimation(segment, lineSeg, new PropertyPath(LineSegment.Y1Property), oldlineSegmentValue.Y1, lineSeg.Y1);
                                        DoubleAnimation danimation2 = getDoubleAnimation(segment, lineSeg, new PropertyPath(LineSegment.X2Property), isSame ? lineSeg.X1 : oldlineSegmentValue.X2, lineSeg.X2);
                                        DoubleAnimation danimation3 = getDoubleAnimation(segment, lineSeg, new PropertyPath(LineSegment.Y2Property), isSame ? lineSeg.Y1 : oldlineSegmentValue.Y2, lineSeg.Y2);
                                        sb.Children.Add(danimation1);
                                        sb.Children.Add(danimation2);
                                        sb.Children.Add(danimation3);
                                        if (hasKey)
                                        {
                                            Series.OldAnimatedValues[segment.DataPoint.X] = lineSeg;
                                        }
                                        else
                                        {
                                            Series.OldAnimatedValues.Add(segment.DataPoint.X, lineSeg);
                                        }
                                        break;

                                    case ChartTypes.Bar:
                                    case ChartTypes.Gantt:
                                    case ChartTypes.RotatedSpline:
                                    case ChartTypes.StackingBar:
                                    case ChartTypes.StackingBar100:
                                    case ChartTypes.Tornado:
                                        #region Initialize Mouse Animation
                                        mouseAnimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.LeftProperty), Canvas.GetLeft(sh) + 20, Canvas.GetLeft(sh));
                                        mouseAnimation.EasingFunction = new BounceEase();
                                        mouseAnimation.BeginTime = new TimeSpan(0);
                                        mouseAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                                        segment.MouseAnimation.Children.Add(mouseAnimation);
                                        #endregion

                                        hasKey = Series.OldAnimatedValues.ContainsKey(segment.DataPoint.X);
                                        oldValue = (hasKey && !this.IsRefreshAnimation) ? (double)Series.OldAnimatedValues[segment.DataPoint.X] : 0d;
                                        danimation = getDoubleAnimation(segment, sh, new PropertyPath(Shape.WidthProperty), oldValue, sh.ActualWidth);
                                        if (hasKey)
                                        {
                                            Series.OldAnimatedValues[segment.DataPoint.X] = sh.ActualWidth;
                                        }
                                        else
                                        {
                                            Series.OldAnimatedValues.Add(segment.DataPoint.X, sh.ActualWidth);
                                        }
                                        break;
                                    default:
                                        #region Initialize Mouse Animation
                                        mouseAnimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.TopProperty), Canvas.GetTop(sh) - 20, Canvas.GetTop(sh));
                                        mouseAnimation.EasingFunction = new BounceEase();
                                        mouseAnimation.BeginTime = new TimeSpan(0);
                                        mouseAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                                        segment.MouseAnimation.Children.Add(mouseAnimation);
                                        #endregion

                                        hasKey = Series.OldAnimatedValues.ContainsKey(segment.DataPoint.X);
                                        oldValue = (hasKey && !this.IsRefreshAnimation) ? (double)Series.OldAnimatedValues[segment.DataPoint.X] : avilableSize.Height;
                                        danimation = getDoubleAnimation(segment, sh, new PropertyPath(Canvas.TopProperty), oldValue, Canvas.GetTop(sh));
                                        if (hasKey)
                                        {
                                            Series.OldAnimatedValues[segment.DataPoint.X] = Canvas.GetTop(sh);
                                        }
                                        else
                                        {
                                            Series.OldAnimatedValues.Add(segment.DataPoint.X, Canvas.GetTop(sh));
                                        }
                                        break;
                                }
                                #endregion
                            }
                            break;

                    }

                    sb.Completed += new EventHandler(sb_Completed_ForSegment);
                    if (danimation != null)
                    {
                        sb.Children.Add(danimation);
                    }

                }
            }
        }

        void mouseAnimation1_Completed(object sender, EventArgs e)
        {
            SwapFromTo(sender as DoubleAnimation);
        }

        /// <summary>
        /// Swap the From and To values of DoubleAnimation.
        /// </summary>
        /// <param name="da"></param>
        void SwapFromTo(DoubleAnimation da)
        {
            if (da == null)
            {
                return;
            }

            DoubleAnimation animation = da;
            double? temp = animation.From;
            animation.From = animation.To;
            animation.To = temp;
        }

        /// <summary>
        /// Use to get the PointAnimationUsingKeyFrames option to update Pie and Doughnut chart types.
        /// </summary>
        /// <param name="segment">Segment to Animate</param>
        /// <param name="targetElement">Path </param>
        /// <param name="propertypath">Property path of Target</param>
        /// <param name="from">From Point for Animate</param>
        /// <param name="to">To Point for Animate</param>
        /// <param name="startAngle">Start angle of Pie</param>
        /// <param name="endAngle">End Angle of Pie</param>
        /// <param name="IsBeginTimeNeeded">Is BeginTime need</param>
        /// <returns></returns>
        private PointAnimationUsingKeyFrames getPointAnimationUsingKeyFrames(Segment segment, DependencyObject targetElement, PropertyPath propertypath, Point from, Point to, double startAngle, double endAngle, bool IsBeginTimeNeeded)
        {
            PointAnimationUsingKeyFrames animation = new PointAnimationUsingKeyFrames();
            ChartPieSegment piSegment = segment as ChartPieSegment;
            Storyboard.SetTarget(animation, targetElement);
            Storyboard.SetTargetProperty(animation, propertypath);
            TimeSpan timespan = Series.AnimationDuration;
            double totalmiliseconds = timespan.TotalMilliseconds;
            double averageseconds = 0d;
            averageseconds = totalmiliseconds / (double)(segment is ChartAdornment ? segment.Series.Adornments.Count : segment.Series.Segments.Count);
            if (Series.AnimateOneByOne == true)
            {
                int index = segment is ChartAdornment ? segment.Series.Adornments.IndexOf(segment) : segment.Series.Segments.IndexOf(segment);
                animation.BeginTime = new TimeSpan(0, 0, 0, 0, IsBeginTimeNeeded ? (int)averageseconds * index : 0);
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)averageseconds));
            }
            else
            {
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)totalmiliseconds));
            }

            double count = Math.Abs(endAngle - startAngle) / 10d;
            //TimeSpan keyFrametime = new TimeSpan(0, 0, 0, 0, (int)(totalmiliseconds / count));
            double angleseconds = ((totalmiliseconds / 360d) * Math.Abs(endAngle - startAngle)) / count;
            double sumSeconds = 0;
            if (endAngle > startAngle)
            {
                for (double i = startAngle; i <= endAngle; i = (i + 10 > endAngle && i != endAngle) ? endAngle : i + 10)
                {
                    TimeSpan keyFrametime = new TimeSpan(0, 0, 0, 0, (int)sumSeconds);
                    Point po = ChartMath.GeneralPointRotation(piSegment.CenterPoint, from, i - startAngle);
                    SplinePointKeyFrame keyframe = new SplinePointKeyFrame() { KeyTime = keyFrametime, Value = po };
                    sumSeconds += angleseconds;
                    animation.KeyFrames.Add(keyframe);
                }
            }
            else
            {
                for (double i = startAngle; i >= endAngle; i = (i - 10 < endAngle && i != endAngle) ? endAngle : i - 10)
                {
                    TimeSpan keyFrametime = new TimeSpan(0, 0, 0, 0, (int)sumSeconds);
                    Point po = ChartMath.GeneralPointRotation(piSegment.CenterPoint, from, i - startAngle);
                    SplinePointKeyFrame keyframe = new SplinePointKeyFrame() { KeyTime = keyFrametime, Value = po };
                    sumSeconds += angleseconds;
                    animation.KeyFrames.Add(keyframe);
                }
            }

            return animation;
        }

        /// <summary>
        /// Create Double Animation object based the Targment element, From and To
        /// </summary>
        /// <param name="segment">Segment to animate</param>
        /// <param name="targetElement">target element</param>
        /// <param name="propertypath">property path</param>
        /// <param name="from">from - double value</param>
        /// <param name="to">to double value</param>
        /// <returns>Double Animation</returns>
        private DoubleAnimation getDoubleAnimation(Segment segment, DependencyObject targetElement, PropertyPath propertypath, double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, targetElement);
            Storyboard.SetTargetProperty(animation, propertypath);
            animation.From = from;
            animation.To = to;
            animation.EasingFunction = segment.Series.EasingFunction;
            TimeSpan timespan = Series.AnimationDuration;
            double totalmiliseconds = timespan.TotalMilliseconds;
            double averageseconds = 0d;
            averageseconds = totalmiliseconds / (double)(segment is ChartAdornment ? segment.Series.Adornments.Count : segment.Series.Segments.Count);
            if (Series.AnimateOneByOne == true && !(targetElement is ChartSeries))
            {
                if (from != to)
                {
                    int index = segment is ChartAdornment ? segment.Series.Adornments.IndexOf(segment) : segment.Series.Segments.IndexOf(segment);
                    animation.BeginTime = new TimeSpan(0, 0, 0, 0, this.Series.IsDataModified && !this.IsRefreshAnimation ? 0 : (int)averageseconds * index);
                    animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)averageseconds));
                }
            }
            else
            {
                animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)totalmiliseconds));
            }

            return animation;
        }

        void sb_Completed(object sender, EventArgs e)
        {
            Storyboard sb = (Storyboard)sender;
            sb.Stop();
        }

        void sb_Completed_ForSegment(object sender, EventArgs e)
        {
            Storyboard sb = (Storyboard)sender;
            sb.Stop();
            foreach (Timeline line in sb.Children)
            {
                line.BeginTime = new TimeSpan(0);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Series != null)
            {
                this.Series = null;
            }
            if (this.Storyboard != null)
                this.Storyboard = null;
        }

        #endregion
    }
}
