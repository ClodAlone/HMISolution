// <copyright file="HubTile.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Syncfusion.WP.Primitives;

namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Syncfusion.Tools.Primitives;

namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Primitives;
namespace Syncfusion.Windows.Controls.Notification
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Syncfusion.UI.Xaml.Primitives;

namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// HubTile control provides
    /// notification through various transition effects.
    /// </summary>
    [ClassReference(IsReviewed = false)]
#if WPF
    [CLSCompliant(false)]
#endif
    public class SfHubTile : HubTileBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfHubTile()
        {
#if WPF
            if (EnvironmentTestHubTile.IsSecurityGranted)
            {
                EnvironmentTestHubTile.StartValidateLicense(typeof(SfHubTile));
            }
#endif
            DefaultStyleKey = typeof(SfHubTile);
            this.Loaded += HubTile_Loaded;
            this.Unloaded += HubTile_Unloaded;

            //a customer reported issue. Designer error when setting the transitions. [But still app can compile and run without this line.]
            this.HubTileTransitions = new HubTileTransitionCollection();
        }

        #endregion

        #region Variables

        private FrameworkElement PART_SlideContent;

        private FrameworkElement PART_SlideRoot;

        private FrameworkElement PART_LayoutRoot;

        private FrameworkElement PART_HubTileContent;

        private FrameworkElement PART_RotateContent;

        private FrameworkElement PART_Root;

        private DispatcherTimer timer;
#if WINRT
        private Storyboard story = null;
#endif
        private bool fadetileflag = false;

        private ContentTransition currentTransition;

        private Random random = new Random();

        #endregion

        /// <summary>
        /// Acts as an event handler when the animation is completed.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public delegate void AnimationCompletedEventHandler(Object Sender,AnimationCompletedArgs args);

        /// <summary>
        /// Invokes the event when the HubTileTransition is completed.
        /// </summary>
        public event AnimationCompletedEventHandler HubTileTransitionCompleted;

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the secondary content to displayed with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object SecondaryContent
        {
            get { return (object)GetValue(SecondaryContentProperty); }
            set { SetValue(SecondaryContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HubTileContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondaryContentProperty =
            DependencyProperty.Register("SecondaryContent", typeof(object), typeof(SfHubTile), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the template for the secondary content to displayed with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public DataTemplate SecondaryContentTemplate
        {
            get { return (DataTemplate)GetValue(SecondaryContentTemplateProperty); }
            set { SetValue(SecondaryContentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SecondaryContentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondaryContentTemplateProperty =
            DependencyProperty.Register("SecondaryContentTemplate", typeof(DataTemplate), typeof(SfHubTile), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTileTransitionCollection"/> that specifies transitions
        /// with <see cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public HubTileTransitionCollection HubTileTransitions
        {
            get { return (HubTileTransitionCollection)GetValue(HubTileTransitionsProperty); }
            set { SetValue(HubTileTransitionsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HubTileTransition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HubTileTransitionsProperty =
            DependencyProperty.Register("HubTileTransitions", typeof(HubTileTransitionCollection), typeof(SfHubTile), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the number of milliseconds to wait before initiating a postback.
        /// </summary>
        /// <value>
        /// It accepts the type of <see cref="T:System.TimeSpan"/>, The default values is
        /// 0 <see cref="P:System.TimeSpan.Milliseconds"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TimeSpan Interval
        {
            get { return (TimeSpan)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(SfHubTile), new PropertyMetadata(TimeSpan.FromSeconds(0), new PropertyChangedCallback(OnIntervalChanged)));



        #endregion

        #region Helper methods
#if !SILVERLIGHT
#endif
        void HubTile_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            HubTileService.Dequeue(this);
            IsEnabledChanged -= OnIsEnabledChanged;
            Unloaded-=HubTile_Unloaded;
            Loaded-=HubTile_Loaded;
        }

        void HubTile_Loaded(object sender, RoutedEventArgs e)
        {
            if (timer != null && !IsFrozen && Interval != TimeSpan.FromSeconds(0))
            {
                timer.Start();
            }
            HubTileService.Enqueue(this);
            IsEnabledChanged +=OnIsEnabledChanged;
            UpdateVisualState();
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (timer != null)
            {
                if (!IsEnabled||IsFrozen)
                    timer.Stop();
                else 
                    timer.Start();

                if (!IsEnabled)
                    VisualStateManager.GoToState(this, "Disabled", true);
                else
                    VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private void UpdateFadeTile(FadeTransition transition)
        {
            if (!fadetileflag)
            {
                AnimateFadeContent(1, transition.Duration, transition.Easing);
                fadetileflag = true;
            }
            else
            {
                AnimateFadeContent(0, transition.Duration, transition.Easing);
                fadetileflag = false;
            }
        }

        private void UpdateSlideTile(SlideTransition transition)
        {
            if (PART_HubTileContent != null)
            {
                if (transition.Direction == Primitives.SlideDirection.Default)
                {
                    AnimateDefaultSlideTransition(transition);
                }
                else if (transition.Direction == Primitives.SlideDirection.Down||transition.Direction==Primitives.SlideDirection.Right)
                {
                    AnimateDownSlideTransition(transition);
                }
                else
                {
                    AnimateRotateSlideTransition(transition);
                }
            }
        }

        private void AnimateRotateSlideTransition(SlideTransition transition)
        {
            if (transition.Position == Position.Bottom || transition.Position == Position.Right)
            {
                AnimateSlideContent(ActualHeight, 0, 0, -ActualHeight, transition.Duration, transition.Easing);
                transition.Position = (transition.Direction == SlideDirection.Left) ? Position.Left : Position.Top; ;
            }
            else if (transition.Position == Position.Top || transition.Position == Position.Left)
            {
                AnimateSlideContent(0, ActualHeight, -ActualHeight, 0, transition.Duration, transition.Easing);
                transition.Position = (transition.Direction == SlideDirection.Right) ? Position.Right : Position.Bottom; ;
            }
        }

        private void AnimateDownSlideTransition(SlideTransition transition)
        {
            if (transition.Position == Position.Top||transition.Position == Position.Left)
            {
                AnimateSlideContent(-ActualHeight, 0, 0, ActualHeight, transition.Duration, transition.Easing);
                transition.Position = (transition.Direction==SlideDirection.Right)?Position.Right:Position.Bottom;
            }
            else if (transition.Position == Position.Bottom || transition.Position == Position.Right)
            {
                AnimateSlideContent(0, -ActualHeight, ActualHeight, 0, transition.Duration, transition.Easing);
                transition.Position = (transition.Direction == SlideDirection.Left) ? Position.Left : Position.Top;
            }
        }

        private void AnimateDefaultSlideTransition(SlideTransition transition)
        {
            if (transition.Position == Position.Bottom || transition.Position==Position.Right)
            {
                AnimateSlideContent(ActualHeight, 0, 0, -ActualHeight, transition.Duration, transition.Easing);
                transition.Position = (transition.Direction==SlideDirection.Left)?Position.Left:Position.Top;
            }
            else if (transition.Position == Position.Top || transition.Position == Position.Left)
            {
                AnimateSlideContent(0, ActualHeight / 2, -ActualHeight, -(ActualHeight / 2), transition.Duration, transition.Easing);
                transition.Position = Position.Center;
            }
            else if (transition.Position == Position.Center)
            {
                AnimateSlideContent(ActualHeight / 2, ActualHeight, -(ActualHeight / 2), 0, transition.Duration, transition.Easing);
                transition.Position = (transition.Direction == SlideDirection.Right) ? Position.Right : Position.Bottom; ;
            }
        }

        private void UpdateRotateTile(RotateTransition transition)
        {
            if (PART_HubTileContent != null)
            {
                if (PART_HubTileContent.Visibility == Visibility.Collapsed)
                {
                    AnimateRotateContent(PART_LayoutRoot, 0, 180, transition.Duration, transition.Easing);
                }
                else
                {
                    AnimateRotateContent(PART_HubTileContent, 180, 360, transition.Duration, transition.Easing);
                }
            }

        }

        private void AnimateRotateContent(FrameworkElement displayelement, double from, double to, TimeSpan duration, EasingFunctionBase easing)
        {
            if (PART_Root != null && PART_LayoutRoot != null)
            {
#if !WPF
#if WINRT
                GC.Collect();
                story = new Storyboard();
                PART_Root.Projection = new PlaneProjection() { CenterOfRotationY = 0.5 };
#else
                    PART_Root.Projection = new PlaneProjection();
#endif
#endif
                    PART_LayoutRoot.Visibility = Visibility.Visible;

                DoubleAnimationUsingKeyFrames timeline1 = BuildTimeLine(from, to, duration, easing);
                ObjectAnimationUsingKeyFrames timeline2 = BuildObjectTimeline(Visibility.Collapsed, TimeSpan.FromTicks(duration.Ticks / 2));
                ObjectAnimationUsingKeyFrames timeline3 = BuildObjectTimeline(Visibility.Visible, TimeSpan.FromTicks(duration.Ticks / 2));

#if !WINRT
                Storyboard story = new Storyboard();
#endif
                story.Children.Add(timeline1);
                story.Children.Add(timeline2);
                story.Children.Add(timeline3);

                Storyboard.SetTarget(timeline1, PART_Root);
                if (displayelement == PART_LayoutRoot)
                {
                    PART_HubTileContent.RenderTransformOrigin = new Point(0.5, 0.5);
#if WPF
                    PART_HubTileContent.RenderTransform = new ScaleTransform() { ScaleY = -1 };
#else
                    PART_HubTileContent.RenderTransform = new CompositeTransform() { ScaleY = -1 };
#endif
                    Storyboard.SetTarget(timeline2, PART_LayoutRoot);
                    Storyboard.SetTarget(timeline3, PART_HubTileContent);
                }
                else if (displayelement == PART_HubTileContent)
                {
#if WINRT
					PART_LayoutRoot.Visibility = Visibility.Collapsed;
#endif
                    Storyboard.SetTarget(timeline2, PART_HubTileContent);
                    Storyboard.SetTarget(timeline3, PART_LayoutRoot);
                }
#if !WINRT
                Storyboard.SetTargetProperty(timeline1, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationX)"));
                Storyboard.SetTargetProperty(timeline2, new PropertyPath("(UIElement.Visibility)"));
                Storyboard.SetTargetProperty(timeline3, new PropertyPath("(UIElement.Visibility)"));
#else
                Storyboard.SetTargetProperty(timeline1, "(UIElement.Projection).(PlaneProjection.RotationX)");
                Storyboard.SetTargetProperty(timeline2, "(UIElement.Visibility)");
                Storyboard.SetTargetProperty(timeline3, "(UIElement.Visibility)");
#endif
                story.Begin();
            }
        }

        private void AnimateFadeContent(double hubcontentto, TimeSpan duration, EasingFunctionBase easing)
        {
            if (PART_LayoutRoot != null && PART_HubTileContent != null)
            {
                if (hubcontentto == 1)
                {
                    PART_HubTileContent.Opacity = 0;
                    PART_HubTileContent.Visibility = Visibility.Visible;
                    PART_LayoutRoot.Opacity = 1;
                    PART_LayoutRoot.Visibility = Visibility.Visible;
                }
                else
                {
                    PART_HubTileContent.Opacity = 1;
                    PART_HubTileContent.Visibility = Visibility.Visible;
                    PART_LayoutRoot.Opacity = 0;
                    PART_LayoutRoot.Visibility = Visibility.Visible;
                }

                DoubleAnimationUsingKeyFrames timeline1 = BuildTimeLine(0, 1, duration, easing);
                DoubleAnimationUsingKeyFrames timeline2 = BuildTimeLine(1, 0, duration, easing);

                if (hubcontentto == 1)
                {
                    Storyboard.SetTarget(timeline1, PART_HubTileContent);
                    Storyboard.SetTarget(timeline2, PART_LayoutRoot);
                }
                else
                {
                    Storyboard.SetTarget(timeline1, PART_LayoutRoot);
                    Storyboard.SetTarget(timeline2, PART_HubTileContent);
                }

#if !WINRT
                Storyboard.SetTargetProperty(timeline1, new PropertyPath("Opacity"));
                Storyboard.SetTargetProperty(timeline2, new PropertyPath("Opacity"));
#else
                Storyboard.SetTargetProperty(timeline1, "Opacity");
                Storyboard.SetTargetProperty(timeline2, "Opacity");
#endif
                Storyboard story = new Storyboard();
                story.Children.Add(timeline1);
                story.Children.Add(timeline2);

                story.Begin();
            }
        }

        private void AnimateSlideContent(double slidefromvalue, double slidetovalue, double rootfromvalue, double roottovalue, TimeSpan duration, EasingFunctionBase easing)
        {
            if (PART_LayoutRoot != null && PART_HubTileContent != null)
            {
                PART_SlideRoot.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, PART_SlideRoot.ActualWidth, PART_SlideRoot.ActualHeight) };
#if WPF
                PART_HubTileContent.RenderTransform = new TranslateTransform();
                PART_LayoutRoot.RenderTransform = new TranslateTransform();
#else
                PART_HubTileContent.RenderTransform = new CompositeTransform();
                PART_LayoutRoot.RenderTransform = new CompositeTransform();
#endif
                PART_HubTileContent.Visibility = Visibility.Visible;

                DoubleAnimationUsingKeyFrames timeline1 = BuildTimeLine(slidefromvalue, slidetovalue, duration, easing);
                DoubleAnimationUsingKeyFrames timeline2 = BuildTimeLine(rootfromvalue, roottovalue, duration, easing);
#if WPF
                DoubleAnimationUsingKeyFrames timeline3 = BuildTimeLine(0, 1, new TimeSpan(0, 0, 0, 0), easing);
#endif

                Storyboard story = new Storyboard();
                story.Children.Add(timeline1);
                story.Children.Add(timeline2);
#if WPF
                story.Children.Add(timeline3);
#endif

                Storyboard.SetTarget(timeline1, PART_HubTileContent);
                Storyboard.SetTarget(timeline2, PART_LayoutRoot);
#if WPF
                Storyboard.SetTarget(timeline3, PART_HubTileContent);
                if((currentTransition as SlideTransition).Direction==Primitives.SlideDirection.Left||(currentTransition as SlideTransition).Direction==Primitives.SlideDirection.Right)
                {
                    Storyboard.SetTargetProperty(timeline1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                    Storyboard.SetTargetProperty(timeline2, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                }
                else
                {
                    Storyboard.SetTargetProperty(timeline1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                    Storyboard.SetTargetProperty(timeline2, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                }
                Storyboard.SetTargetProperty(timeline3, new PropertyPath("Opacity"));                
#elif !WINRT
                if ((currentTransition as SlideTransition).Direction == Primitives.SlideDirection.Left || (currentTransition as SlideTransition).Direction == Primitives.SlideDirection.Right)
                {
                    Storyboard.SetTargetProperty(timeline1, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                    Storyboard.SetTargetProperty(timeline2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                }
                else
                {
                    Storyboard.SetTargetProperty(timeline1, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
                    Storyboard.SetTargetProperty(timeline2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
                }
#else
                if((currentTransition as SlideTransition).Direction==Primitives.SlideDirection.Left||(currentTransition as SlideTransition).Direction==Primitives.SlideDirection.Right)
                {
                    Storyboard.SetTargetProperty(timeline1, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
                    Storyboard.SetTargetProperty(timeline2, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
                }
                else
                {
                    Storyboard.SetTargetProperty(timeline1, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
                    Storyboard.SetTargetProperty(timeline2, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
                }
#endif
                story.Begin();
            }
        }

        private DoubleAnimationUsingKeyFrames BuildTimeLine(double from, double to, TimeSpan duration, EasingFunctionBase easing)
        {
            EasingDoubleKeyFrame keyframe1 = new EasingDoubleKeyFrame();
            keyframe1.Value = from;
            keyframe1.KeyTime = TimeSpan.FromSeconds(0);

            EasingDoubleKeyFrame keyframe2 = new EasingDoubleKeyFrame();
            keyframe2.Value = to;
            keyframe2.KeyTime = duration;
            keyframe2.EasingFunction = easing;

            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(keyframe1);
            timeline.KeyFrames.Add(keyframe2);
            return timeline;
        }

        private ObjectAnimationUsingKeyFrames BuildObjectTimeline(object visibility, TimeSpan duration)
        {
            DiscreteObjectKeyFrame keyframe = new DiscreteObjectKeyFrame();
            keyframe.KeyTime = duration;
            keyframe.Value = visibility;

            ObjectAnimationUsingKeyFrames timeline = new ObjectAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(keyframe);
            return timeline;
        }

        private bool CheckWhetherMovetoNextIteration()
        {
            if (currentTransition != null)
            {
                if (currentTransition is SlideTransition)
                {
                    SlideTransition slide = (SlideTransition)currentTransition;
                    if (slide.Position == Position.Center)
                    {
                        UpdateTransition(currentTransition);
                        return false;
                    }

                    if (slide.Direction == Primitives.SlideDirection.Default || slide.Direction == Primitives.SlideDirection.Up)
                    {
                        if (slide.Position == Position.Top)
                        {
                            UpdateTransition(currentTransition);
                            return false;
                        }
                    }
                    else
                    {
                        if (slide.Position == Position.Bottom)
                        {
                            UpdateTransition(currentTransition);
                            return false;
                        }
                    }
                }
                else if (currentTransition is RotateTransition)
                {
                    if (PART_HubTileContent.Visibility == Visibility.Visible)
                    {
                        UpdateTransition(currentTransition);
                        return false;
                    }
                }
                else if (currentTransition is FadeTransition)
                {
                    if (PART_HubTileContent.Opacity == 1)
                    {
                        UpdateTransition(currentTransition);
                        return false;
                    }
                }
            }
            return true;
        }

        private void timer_Tick(object sender, object e)
        {
            if (!CheckWhetherMovetoNextIteration())
            {
                return;
            }
#if WINRT
            ClearRotationAnimation();
#endif
            MoveToNextIteration();
        }

        private void MoveToNextIteration()
        {
            if (HubTileTransitions != null && HubTileTransitions.Count > 0 && PART_LayoutRoot !=null)
            {
                int index = random.Next(0, HubTileTransitions.Count);
                ContentTransition transition = HubTileTransitions[index];
                if (transition is RotateTransition)
                {
                    PART_LayoutRoot.Opacity = 1;
                    PART_HubTileContent.Opacity = 1;
                    PART_HubTileContent.Visibility = Visibility.Collapsed;
                    PART_LayoutRoot.Visibility = Visibility.Visible;
                }
                else if (transition is SlideTransition)
                {
                    PART_HubTileContent.Opacity = 1;
                    PART_LayoutRoot.Opacity = 1;

                }
                else if (transition is FadeTransition)
                {
                    PART_HubTileContent.RenderTransformOrigin = new Point(0.5, 0.5);
#if WPF
                    PART_HubTileContent.RenderTransform = new ScaleTransform() { ScaleY = 1 };
#else
                    PART_HubTileContent.RenderTransform = new CompositeTransform() { ScaleY = 1 };
#endif
                }
                currentTransition = transition;
                UpdateTransition(transition);
            }
        }

        private void UpdateTransition(ContentTransition transition)
        {
            if (transition is SlideTransition)
            {
                UpdateSlideTile((SlideTransition)transition);
            }
            else if (transition is RotateTransition)
            {
                UpdateRotateTile((RotateTransition)transition);
            }
            else if (transition is FadeTransition)
            {
                UpdateFadeTile((FadeTransition)transition);
            }
            if (HubTileTransitionCompleted != null)
                HubTileTransitionCompleted(this, new AnimationCompletedArgs(transition));
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Invoked when the IsFrozen property is changed
        /// </summary>
        /// <param name="e"></param>
        /// <value> Timer is
        /// <c>Stop</c> if IsFrozen is True; otherwise, <c>Start</c>.
        /// </value>
        protected override void OnIsFrozenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsFrozen)
            {
                if (this.timer != null)
                    this.timer.Stop();
            }
            else
            {
                if (this.timer != null)
                    this.timer.Start();
            }
        }

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfHubTile"/>
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_SlideContent = GetTemplateChild("PART_SlideContent") as FrameworkElement;
            PART_LayoutRoot = GetTemplateChild("PART_LayoutRoot") as FrameworkElement;
            PART_Root = GetTemplateChild("PART_Root") as FrameworkElement;
            PART_SlideRoot = GetTemplateChild("PART_SlideRoot") as FrameworkElement;
            PART_RotateContent = GetTemplateChild("PART_RotateContent") as FrameworkElement;
            PART_HubTileContent = GetTemplateChild("PART_HubTileContent") as FrameworkElement;
            base.OnApplyTemplate();
        }

        #endregion

        #region Callback Methods

        private static void OnIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfHubTile tile = sender as SfHubTile;
            if (tile != null)
            {
                if (e.NewValue != null && ((TimeSpan)e.NewValue).Seconds != 0)
                {
                    if (tile.timer == null)
                    {
                        tile.timer = new DispatcherTimer();
                    }
                    tile.timer.Interval = tile.Interval;
                    tile.timer.Tick -= tile.timer_Tick;
                    tile.timer.Tick += tile.timer_Tick;
                }
                else
                {
                    tile.timer.Tick -= tile.timer_Tick;
                }
            }
        }

        #endregion

#if WINRT
        public void ClearRotationAnimation()
        {
            if (PART_Root != null  && story!=null)
            {
                PART_Root.Projection = null;
                story.Stop();
                story.Children.Clear();
                story = null;
           		GC.Collect();
		        GC.WaitForPendingFinalizers(); 
            }
        }
#endif

    }
    /// <summary>
    /// Arguments for the AnimationCompletedEventHandler
    /// </summary>
    /// <remarks>
    /// Consists of a variable for ContentTransition
    /// </remarks>
    [ClassReference(IsReviewed = false)]
#if WPF
        [CLSCompliant(false)]
#endif
    public class AnimationCompletedArgs : RoutedEventArgs
    {
        private ContentTransition hubTileTransition;
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.AnimationCompletedArgs"/> class.
        /// </summary>
        /// <param name="_hubTileTransition"></param>
        [ClassReference(IsReviewed = false)]
        public AnimationCompletedArgs(ContentTransition _hubTileTransition)
        {
            hubTileTransition = _hubTileTransition;
        }

        /// <summary>
        /// Gets the value for ContentTransition
        /// </summary>
        public ContentTransition HubTileTransition { get { return hubTileTransition; } }
   } 

}
