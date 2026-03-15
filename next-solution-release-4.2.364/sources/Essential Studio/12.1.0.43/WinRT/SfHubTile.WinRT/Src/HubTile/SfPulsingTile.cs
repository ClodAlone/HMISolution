// <copyright file="AnimatingTile.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Tools.Controls.Notification
#else

#if WPF
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Licensing;
namespace Syncfusion.Windows.Controls.Notification
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// PulsingTile Control resembles
    /// the Music and Video hub tile in Windows phone. The content will zooms
    /// out/in randomly and show a translation movement in X and Y axis randomly.
    /// </summary>
    [ClassReference(IsReviewed = false)]
#if WPF
    [CLSCompliant(false)]
#endif
    public class SfPulsingTile : HubTileBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfPulsingTile()
        {
#if WPF
            if (EnvironmentTestHubTile.IsSecurityGranted)
            {
                EnvironmentTestHubTile.StartValidateLicense(typeof(SfPulsingTile));
            }
#endif
            DefaultStyleKey = typeof(SfPulsingTile);
            this.Loaded += OnLoaded;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Defines a variable for the ContentPresenter framework element
        /// </summary>
        protected ContentPresenter PART_Content;

        Storyboard roatationStroyboard = null;

        Storyboard pulseStroyboard = null;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the value that specify the radius X with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile"/>.
        /// </summary>
        /// <value>
        /// The default values is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile.RadiusY"/>
        [ClassReference(IsReviewed = false)]
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RadiusX.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(SfPulsingTile), new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Gets or sets the value that specify the radius Y with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile"/>.
        /// </summary>
        /// <value>
        /// The default values is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile.RadiusX"/>
        [ClassReference(IsReviewed = false)]
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RadiusY.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
           DependencyProperty.Register("RadiusY", typeof(double), typeof(SfPulsingTile), new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged)));



        /// <summary>
        /// Gets or sets the <see cref="T:System.TimeSpan"/> that specify the pulse duration
        /// for animation.
        /// </summary>
        /// <value>
        /// The default value is 4 <see cref="P:System.TimeSpan.Milliseconds"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile.TranslateDuration"/>
        [ClassReference(IsReviewed = false)]
        public TimeSpan PulseDuration
        {
            get { return (TimeSpan)GetValue(PulseDurationProperty); }
            set { SetValue(PulseDurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PulseDuration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PulseDurationProperty =
            DependencyProperty.Register("PulseDuration", typeof(TimeSpan), typeof(SfPulsingTile), new PropertyMetadata(TimeSpan.FromSeconds(4), new PropertyChangedCallback(OnValueChanged)));




        /// <summary>
        /// Gets or sets the <see cref="T:System.TimeSpan"/> that specify the translate
        /// duration for animation.
        /// </summary>
        /// <value>
        /// The default value is 4 <see cref="P:System.TimeSpan.Milliseconds"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile.PulseDuration"/>
        [ClassReference(IsReviewed = false)]
        public TimeSpan TranslateDuration
        {
            get { return (TimeSpan)GetValue(TranslateDurationProperty); }
            set { SetValue(TranslateDurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TranslateDuration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TranslateDurationProperty =
            DependencyProperty.Register("TranslateDuration", typeof(TimeSpan), typeof(SfPulsingTile), new PropertyMetadata(TimeSpan.FromSeconds(4), new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Gets or sets the scale value for animation
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double PulseScale
        {
            get { return (double)GetValue(PulseScaleProperty); }
            set { SetValue(PulseScaleProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for PulseScale.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PulseScaleProperty =
            DependencyProperty.Register("PulseScale", typeof(double), typeof(SfPulsingTile), new PropertyMetadata(1d, new PropertyChangedCallback(OnValueChanged)));

        #endregion

        #region Helper Methods

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            HubTileService.Enqueue(this);
            this.Unloaded += PulsingTile_Unloaded;
            if(!IsFrozen)
            StartAnimation();

            IsEnabledChanged += OnIsEnabledChanged;
            UpdateVisualState();
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (!IsEnabled || IsFrozen)
                StopAnimation();
            else
                StartAnimation();

            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
        }

        void PulsingTile_Unloaded(object sender, RoutedEventArgs e)
        {
            HubTileService.Dequeue(this);
            IsEnabledChanged -= OnIsEnabledChanged;
            this.Unloaded -= PulsingTile_Unloaded;
        }

        private void StartAnimation()
        {
            AnimateContent();
        }

        private void StopAnimation()
        {
            if (pulseStroyboard != null)
                pulseStroyboard.Stop();

            if (roatationStroyboard != null)
                roatationStroyboard.Stop();
        }

        private void AnimateContent()
        {
            if (PART_Content != null)
            {
                this.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
#if WPF
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new ScaleTransform());
                transform.Children.Add(new TranslateTransform());
                PART_Content.RenderTransform = transform;
#else
                PART_Content.RenderTransform = new CompositeTransform();
#endif
                PART_Content.RenderTransformOrigin = new Point(0.5, 0.5);
                PART_Content.Visibility = Visibility.Visible;

                //Rotation
                DoubleAnimationUsingKeyFrames rotationtimeline1 = BuildXTimeLine(RadiusX, RadiusY);
                DoubleAnimationUsingKeyFrames rotationtimeline2 = BuildYTimeLine(RadiusX, RadiusY);

                //if (roatationStroyboard == null)
                roatationStroyboard = new Storyboard();
#if !WINRT
                roatationStroyboard.RepeatBehavior = RepeatBehavior.Forever;
#else
                roatationStroyboard.RepeatBehavior = new RepeatBehavior() { Type = RepeatBehaviorType.Forever };
#endif
                roatationStroyboard.Children.Add(rotationtimeline1);
                roatationStroyboard.Children.Add(rotationtimeline2);

                Storyboard.SetTarget(rotationtimeline1, PART_Content);
                Storyboard.SetTarget(rotationtimeline2, PART_Content);

#if WINRT
                Storyboard.SetTargetProperty(rotationtimeline1, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
                Storyboard.SetTargetProperty(rotationtimeline2, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
                
#elif WPF
                Storyboard.SetTargetProperty(rotationtimeline1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(rotationtimeline2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));
#else
                Storyboard.SetTargetProperty(rotationtimeline1, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                Storyboard.SetTargetProperty(rotationtimeline2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
#endif
                //Pulse
                DoubleAnimationUsingKeyFrames pulsetimeline1 = BuildPulseXTimeLine(PulseScale);
                DoubleAnimationUsingKeyFrames pulsetimeline2 = BuildPulseYTimeLine(PulseScale);

                //if (pulseStroyboard == null)
                pulseStroyboard = new Storyboard();

                pulseStroyboard.AutoReverse = true;
                pulseStroyboard.SpeedRatio = 0.4;
#if !WINRT
                pulseStroyboard.RepeatBehavior = RepeatBehavior.Forever;
#else
                pulseStroyboard.RepeatBehavior = new RepeatBehavior() { Type = RepeatBehaviorType.Forever };
#endif
                pulseStroyboard.Children.Add(pulsetimeline1);
                pulseStroyboard.Children.Add(pulsetimeline2);

                Storyboard.SetTarget(pulsetimeline1, PART_Content);
                Storyboard.SetTarget(pulsetimeline2, PART_Content);

#if WINRT
                Storyboard.SetTargetProperty(pulsetimeline1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
                Storyboard.SetTargetProperty(pulsetimeline2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
                
#elif WPF
                Storyboard.SetTargetProperty(pulsetimeline1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(pulsetimeline2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
#else
                Storyboard.SetTargetProperty(pulsetimeline1, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
                Storyboard.SetTargetProperty(pulsetimeline2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)")); 
#endif
                pulseStroyboard.Begin();
                roatationStroyboard.Begin();
            }
        }

        private DoubleAnimationUsingKeyFrames BuildXTimeLine(double radiusx, double radiusy)
        {
            TimeSpan timeduration = TranslateDuration;

            DiscreteDoubleKeyFrame doublekeyframe = new DiscreteDoubleKeyFrame();
            doublekeyframe.Value = 0;
            doublekeyframe.KeyTime = TimeSpan.FromSeconds(0);

            EasingDoubleKeyFrame rightkeyframe = new EasingDoubleKeyFrame();
            rightkeyframe.Value = radiusx;
            rightkeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeduration.TotalSeconds / 4));
            rightkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };

            EasingDoubleKeyFrame bottomkeyframe = new EasingDoubleKeyFrame();
            bottomkeyframe.Value = 0;
            bottomkeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeduration.TotalSeconds / 2));
            bottomkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };

            EasingDoubleKeyFrame leftkeyframe = new EasingDoubleKeyFrame();
            leftkeyframe.Value = -radiusx;
            leftkeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeduration.TotalSeconds * 3 / 4));
            leftkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };

            EasingDoubleKeyFrame topkeyframe = new EasingDoubleKeyFrame();
            topkeyframe.Value = 0;
            topkeyframe.KeyTime = KeyTime.FromTimeSpan(timeduration);
            topkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };

            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();

            timeline.KeyFrames.Add(doublekeyframe);
            timeline.KeyFrames.Add(rightkeyframe);
            timeline.KeyFrames.Add(bottomkeyframe);
            timeline.KeyFrames.Add(leftkeyframe);
            timeline.KeyFrames.Add(topkeyframe);

            return timeline;
        }

        private DoubleAnimationUsingKeyFrames BuildYTimeLine(double radiusx, double radiusy)
        {
            TimeSpan timeduration = TranslateDuration;

            DiscreteDoubleKeyFrame doublekeyframe = new DiscreteDoubleKeyFrame();
            doublekeyframe.Value = 0;
            doublekeyframe.KeyTime = TimeSpan.FromSeconds(0);

            EasingDoubleKeyFrame rightkeyframe = new EasingDoubleKeyFrame();
            rightkeyframe.Value = RadiusY;
            rightkeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeduration.TotalSeconds / 4));
            rightkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };

            EasingDoubleKeyFrame bottomkeyframe = new EasingDoubleKeyFrame();
            bottomkeyframe.Value = 2 * RadiusY;
            bottomkeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeduration.TotalSeconds / 2));
            bottomkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };

            EasingDoubleKeyFrame leftkeyframe = new EasingDoubleKeyFrame();
            leftkeyframe.Value = radiusy;
            leftkeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeduration.TotalSeconds * 3 / 4));
            leftkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };

            EasingDoubleKeyFrame topkeyframe = new EasingDoubleKeyFrame();
            topkeyframe.Value = 0;
            topkeyframe.KeyTime = KeyTime.FromTimeSpan(timeduration);
            topkeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };

            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();

            timeline.KeyFrames.Add(doublekeyframe);
            timeline.KeyFrames.Add(rightkeyframe);
            timeline.KeyFrames.Add(bottomkeyframe);
            timeline.KeyFrames.Add(leftkeyframe);
            timeline.KeyFrames.Add(topkeyframe);

            return timeline;
        }

        private DoubleAnimationUsingKeyFrames BuildPulseXTimeLine(double pulseScale)
        {
            TimeSpan timeduration = PulseDuration;

            SplineDoubleKeyFrame splinefkeyframe = new SplineDoubleKeyFrame();
            splinefkeyframe.Value = 1.0;
            splinefkeyframe.KeyTime = TimeSpan.FromSeconds(0);

            EasingDoubleKeyFrame pulsekeyframe = new EasingDoubleKeyFrame();
            pulsekeyframe.Value = pulseScale;
            pulsekeyframe.KeyTime = KeyTime.FromTimeSpan(PulseDuration);
            pulsekeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };

            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(splinefkeyframe);
            timeline.KeyFrames.Add(pulsekeyframe);

            return timeline;

        }

        private DoubleAnimationUsingKeyFrames BuildPulseYTimeLine(double pulseScale)
        {
            TimeSpan timeduration = PulseDuration;

            SplineDoubleKeyFrame splinefkeyframe = new SplineDoubleKeyFrame();
            splinefkeyframe.Value = 1.0;
            splinefkeyframe.KeyTime = TimeSpan.FromSeconds(0);

            EasingDoubleKeyFrame pulsekeyframe = new EasingDoubleKeyFrame();
            pulsekeyframe.Value = pulseScale;
            pulsekeyframe.KeyTime = KeyTime.FromTimeSpan(PulseDuration);
            pulsekeyframe.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };

            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(splinefkeyframe);
            timeline.KeyFrames.Add(pulsekeyframe);

            return timeline;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Invoked when the IsFrozen property is changed
        /// </summary>
        /// <param name="e"></param>
        /// <value> pulseStroyboard is
        /// <c>Stop</c> if IsFrozen is True; otherwise, <c>Begin</c>.
        /// </value>
        protected override void OnIsFrozenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsFrozen)
            {
                if (pulseStroyboard != null)
                    pulseStroyboard.Stop();
                if (roatationStroyboard != null)
                    roatationStroyboard.Stop();
            }
            else
            {
                if (pulseStroyboard != null)
                    pulseStroyboard.Begin();
                if (roatationStroyboard != null)
                    roatationStroyboard.Begin();
            }
        }

        /// <summary>
        /// Initializes all the child elements of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.PulsingTile"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            PART_Content = GetTemplateChild("PART_Content") as ContentPresenter;
#if SILVERLIGHT
            this.Loaded += OnLoaded;
#endif
        }

        #endregion

        #region Callback Methods

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfPulsingTile tile = sender as SfPulsingTile;

            if (tile != null)
            {
                tile.StopAnimation();
                tile.StartAnimation();
            }
        }

        #endregion

    }
}
