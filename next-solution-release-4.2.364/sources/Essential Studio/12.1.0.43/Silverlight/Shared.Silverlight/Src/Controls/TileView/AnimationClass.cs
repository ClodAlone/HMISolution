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
using System.Windows.Threading;
#if SILVERLIGHT
using Syncfusion.Windows.Controls;
#endif


namespace Syncfusion.Windows.Shared
{
#if WPF
#if SyncfusionFramework4_0
    /// <summary>
    /// 
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#endif

    /// <summary>
    /// Contains all the methods events related to animations happening in tile view control.
    /// </summary>
    public class TileViewItemAnimationBase : HeaderedContentControl
    {
        #region   Private Members

        /// <summary>
        /// Stores the size animation.
        /// </summary>
        private bool animatingSize;

        /// <summary>
        /// Stores the position animation.
        /// </summary>
        private bool animatingPosition;

        /// <summary>
        /// Stores the size animation timespan.
        /// </summary>
        private TimeSpan animationTimeSpanSize = new TimeSpan(0, 0, 0, 0, 700);

        /// <summary>
        /// Stores the position animation time span.
        /// </summary>
        private TimeSpan animationTimespanPosition = new TimeSpan(0, 0, 0, 0, 700);

        /// <summary>
        /// Stores the width key frame.
        /// </summary>
        private SplineDoubleKeyFrame animationWidthKeyFrameSize;

        /// <summary>
        /// Stores the height key frame.
        /// </summary>
        private SplineDoubleKeyFrame animationHeightKeyFrameSize;

        /// <summary>
        /// Stores the posisition X key frame.
        /// </summary>
        private SplineDoubleKeyFrame animationXKeyFramePosition;

        /// <summary>
        /// Stores the position Y keyframe.
        /// </summary>
        private SplineDoubleKeyFrame animationYKeyFramePosition;

        /// <summary>
        /// Stors the Storyboard Animation Size.
        /// </summary>
        private Storyboard animationSize;

        /// <summary>
        /// Stors the Storyboard Animation Position.
        /// </summary>
        internal Storyboard animationPosition;


        /// <summary>
        /// stores the details of rows in the TileViewControl
        /// </summary>
        public TileViewControl ParentTileViewControl = null;

        #endregion

        #region Region Constructor
        /// <summary>
        /// Initializes a new instance of the {TileViewItemAnimationBase} class
        /// </summary>
        internal TileViewItemAnimationBase()
        {
#if WPF
            try
            {
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
            }
            catch (InvalidOperationException)
            {
                if (Application.Current != null && Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        {
                            if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                                System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                        }));
                }
            }
#endif
            this.animationSize = new Storyboard();
            int timeSpan = 700;

            DoubleAnimationUsingKeyFrames widthAnimation = new DoubleAnimationUsingKeyFrames();

#if WPF
            widthAnimation.FillBehavior = FillBehavior.Stop;
#endif

            Storyboard.SetTarget(widthAnimation, this);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("(FrameworkElement.Width)"));
            this.animationWidthKeyFrameSize = new SplineDoubleKeyFrame();
            this.animationWidthKeyFrameSize.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.animationWidthKeyFrameSize.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.animationWidthKeyFrameSize.Value = 0;
            widthAnimation.KeyFrames.Add(this.animationWidthKeyFrameSize);

            DoubleAnimationUsingKeyFrames heightAnimation = new DoubleAnimationUsingKeyFrames();

#if WPF
            heightAnimation.FillBehavior = FillBehavior.Stop;
#endif

            Storyboard.SetTarget(heightAnimation, this);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("(FrameworkElement.Height)"));
            this.animationHeightKeyFrameSize = new SplineDoubleKeyFrame();
            this.animationHeightKeyFrameSize.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.animationHeightKeyFrameSize.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.animationHeightKeyFrameSize.Value = 0;
            heightAnimation.KeyFrames.Add(this.animationHeightKeyFrameSize);

            this.animationSize.Children.Add(widthAnimation);
            this.animationSize.Children.Add(heightAnimation);
            this.animationSize.Completed += new EventHandler(this.AnimationSize_Completed);

            this.animationPosition = new Storyboard();

            DoubleAnimationUsingKeyFrames positionXAnimation = new DoubleAnimationUsingKeyFrames();

#if WPF
            positionXAnimation.FillBehavior = FillBehavior.Stop;
#endif

            Storyboard.SetTarget(positionXAnimation, this);
            Storyboard.SetTargetProperty(positionXAnimation, new PropertyPath("(Canvas.Left)"));
            this.animationXKeyFramePosition = new SplineDoubleKeyFrame();
            this.animationXKeyFramePosition.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.animationXKeyFramePosition.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.animationXKeyFramePosition.Value = 0;
            positionXAnimation.KeyFrames.Add(this.animationXKeyFramePosition);

            DoubleAnimationUsingKeyFrames positionYAnimation = new DoubleAnimationUsingKeyFrames();

#if WPF
            positionYAnimation.FillBehavior = FillBehavior.Stop;
#endif

            Storyboard.SetTarget(positionYAnimation, this);
            Storyboard.SetTargetProperty(positionYAnimation, new PropertyPath("(Canvas.Top)"));
            this.animationYKeyFramePosition = new SplineDoubleKeyFrame();
            this.animationYKeyFramePosition.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.animationYKeyFramePosition.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.animationYKeyFramePosition.Value = 0;
            positionYAnimation.KeyFrames.Add(this.animationYKeyFramePosition);

            this.animationPosition.Children.Add(positionXAnimation);
            this.animationPosition.Children.Add(positionYAnimation);

            this.animationPosition.Completed += new EventHandler(this.AnimationPosition_Completed);
        }
        #endregion

        #region Get Set Region

        /// <summary>
        /// Gets or Sets the Duration for position Animation
        /// </summary>
        internal TimeSpan PositionAnimationDuration
        {
            get { return (TimeSpan)GetValue(PositionAnimationDurationProp); }
            set { SetValue(PositionAnimationDurationProp, value); }
        }

        /// <summary>
        /// Gets or Sets the Duration for Size Animation
        /// </summary>
        internal TimeSpan SizeAnimationDuration
        {
            get { return (TimeSpan)GetValue(SizeAnimationDurationProp); }
            set { SetValue(SizeAnimationDurationProp, value); }
        }

        #endregion

        #region DependencyProperty Region

        /// <summary>
        /// Identifies the SizeAnimationDuration Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SizeAnimationDurationProp =
            DependencyProperty.Register("SizeAnimationDuration", typeof(TimeSpan), typeof(TileViewItemAnimationBase), new PropertyMetadata(SetAnimationSizeDuration));

        /// <summary>
        /// Identifies the PositionAnimationDuration Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PositionAnimationDurationProp =
            DependencyProperty.Register("PositionAnimationDuration", typeof(TimeSpan), typeof(TileViewItemAnimationBase), new PropertyMetadata(SetPositionAnimationDuration));

        #endregion

        #region Events Region

        /// <summary>
        /// Sets the duration of the animation size.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SetAnimationSizeDuration(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TileViewItemAnimationBase instance = (TileViewItemAnimationBase)obj;
            instance.SetAnimationSizeDuration(e);
        }

        /// <summary>
        /// Sets the duration of the animation size.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void SetAnimationSizeDuration(DependencyPropertyChangedEventArgs e)
        {
            if (this.animationHeightKeyFrameSize != null)
            {
                this.animationHeightKeyFrameSize.KeyTime = KeyTime.FromTimeSpan(this.animationTimeSpanSize);
            }

            if (this.animationWidthKeyFrameSize != null)
            {
                this.animationWidthKeyFrameSize.KeyTime = KeyTime.FromTimeSpan(this.animationTimeSpanSize);
            }
        }

        /// <summary>
        /// Sets the duration of the position animation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SetPositionAnimationDuration(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TileViewItemAnimationBase instance = (TileViewItemAnimationBase)obj;
            instance.SetAnimationSizeDuration(e);
        }

        /// <summary>
        /// Sets the duration of the position animation.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void SetPositionAnimationDuration(DependencyPropertyChangedEventArgs e)
        {
            if (this.animationXKeyFramePosition != null)
            {
                this.animationXKeyFramePosition.KeyTime = KeyTime.FromTimeSpan(this.animationTimespanPosition);
            }

            if (this.animationYKeyFramePosition != null)
            {
                this.animationYKeyFramePosition.KeyTime = KeyTime.FromTimeSpan(this.animationTimespanPosition);
            }
        }

        /// <summary>
        /// This Event gets fired once the Animation Size is Completed
        /// </summary>
        /// <param name="sender">objects as sender</param>
        /// <param name="e">event as eventargs</param>
        private void AnimationSize_Completed(object sender, EventArgs e)
        {
            if (animationWidthKeyFrameSize.Value > 0)
            {
                this.Width = this.animationWidthKeyFrameSize.Value;
            }
            else
            {
                this.Width = 0;
            }
            if (animationHeightKeyFrameSize.Value > 0)
            {
                this.Height = this.animationHeightKeyFrameSize.Value;
            }
            else
            {
                this.Height = 0;
            }
        }

        /// <summary>
        /// This Event gets fired once the Animation Position is Completed
        /// </summary>
        /// <param name="sender">objects as sender</param>
        /// <param name="e">event as eventargs</param>
        private void AnimationPosition_Completed(object sender, EventArgs e)
        {
            Canvas.SetLeft(this, this.animationXKeyFramePosition.Value);
            Canvas.SetTop(this, this.animationYKeyFramePosition.Value);
        }

        #endregion

        #region Methods Region

        /// <summary>
        /// Animates the size of the control
        /// </summary>
        /// <param name="width">The target width</param>
        /// <param name="height">The target height</param>
        public void AnimateSize(double width, double height)
        {
            if (this.animatingSize)
            {
                this.animationSize.Pause();
            }

            if (VisualTreeHelper.GetParent(this) != null)
            {
                this.Width = this.ActualWidth;
                this.Height = this.ActualHeight;
                this.animatingSize = true;
                this.animationWidthKeyFrameSize.Value = width;
                this.animationHeightKeyFrameSize.Value = height;
                this.animationSize.Begin();
            }
        }

        /// <summary>
        /// Animates the Position of the control
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        internal virtual void AnimatePosition(double x, double y)
        {
            if (this.animatingPosition)
            {
                this.animationPosition.Pause();
            }

            if (VisualTreeHelper.GetParent(this) != null)
            {
                this.animatingPosition = true;
                this.animationXKeyFramePosition.Value = x;
                this.animationYKeyFramePosition.Value = y;
                this.animationPosition.Begin();

            }
        }

        #endregion

    }
}
