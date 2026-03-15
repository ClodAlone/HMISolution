// <copyright file="TransitionContentControl.cs" company="Syncfusion">
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
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;
using Syncfusion.WP.Primitives;

namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;
using Syncfusion.Tools.Primitives;

namespace Syncfusion.Tools.Controls
#else
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;
using Syncfusion.Licensing;
using Syncfusion.Windows.Primitives;

namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Syncfusion.UI.Xaml.Primitives;
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    ///TransitionContentControl is a <see
    /// cref="N:Windows.UI.Xaml.Controls.ContentControl">ContentControl</see> that contains
    /// the implementation for the controls that needs transitions.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TransitionContentControl : ContentControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.TransitionContentControl"/> class.
        /// </summary>
        public TransitionContentControl()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(TransitionContentControl));
            }
#endif

            DefaultStyleKey = typeof(TransitionContentControl);
        }

        #endregion

        #region Variables

        private ContentPresenter PART_Content;

        private ContentControl PART_TempContent;

        private Grid PART_LayoutRoot;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public ContentTransition Transition
        {
            get { return (ContentTransition)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Transition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(ContentTransition), typeof(TransitionContentControl), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value to animate while up/down click.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable animation]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(TransitionContentControl), new PropertyMetadata(true));

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.TransitionContentControl"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Content = GetTemplateChild("PART_Content") as ContentPresenter;
            PART_TempContent = GetTemplateChild("PART_TempContent") as ContentControl;
            PART_LayoutRoot = GetTemplateChild("PART_LayoutRoot") as Grid;
            base.OnApplyTemplate();
        }
        /// <summary>
        /// Invoked when the content is changed
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnContentChanged(object oldContent, object newContent)

        {
            if (PART_Content != null && PART_TempContent != null && PART_LayoutRoot != null)
            {
                if (EnableAnimation)
                {
                    if (Transition is SlideTransition)
                    {
                        PART_TempContent.Visibility = Visibility.Visible;
                        PART_TempContent.Content = oldContent;
#if WPF
                        PART_Content.RenderTransform = new TranslateTransform();
                        PART_TempContent.RenderTransform = new TranslateTransform();
#else
                        PART_Content.RenderTransform = new CompositeTransform();
                        PART_TempContent.RenderTransform = new CompositeTransform();
#endif
                        PART_LayoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
                        Timeline enteraction, exitaction;
                        if ((this.Transition as SlideTransition).Direction == SlideDirection.Left ||
                            (this.Transition as SlideTransition).Direction == SlideDirection.Right)
                        {
                            enteraction = (this.Transition as SlideTransition).CreateEnterAnimation(ActualWidth);
                            exitaction = (this.Transition as SlideTransition).CreateExitAnimation(ActualWidth);
                        }
                        else
                        {
                            enteraction = (this.Transition as SlideTransition).CreateEnterAnimation(ActualHeight);
                            exitaction = (this.Transition as SlideTransition).CreateExitAnimation(ActualHeight);
                        }

                        Storyboard enter = new Storyboard();
                        enter.Children.Add(enteraction);
                        Storyboard.SetTarget(enteraction, PART_Content);
                        if ((this.Transition as SlideTransition).Direction == SlideDirection.Left || (this.Transition as SlideTransition).Direction == SlideDirection.Right)
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
                        Storyboard.SetTargetProperty(enteraction, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
#else
#if WPF
                        Storyboard.SetTargetProperty(enteraction, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
#else
                        Storyboard.SetTargetProperty(enteraction, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
#endif
#endif
                        else
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
                        Storyboard.SetTargetProperty(enteraction, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
#else
#if WPF
                        Storyboard.SetTargetProperty(enteraction, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
#else
                            Storyboard.SetTargetProperty(enteraction, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
#endif
#endif 

                        var exit = new Storyboard();
                        exit.Children.Add(exitaction);
                        Storyboard.SetTarget(exitaction, PART_TempContent);
                        if ((this.Transition as SlideTransition).Direction == SlideDirection.Left || (this.Transition as SlideTransition).Direction == SlideDirection.Right)
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
                        Storyboard.SetTargetProperty(exitaction, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
#else
#if WPF
                        Storyboard.SetTargetProperty(exitaction, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
#else
                        Storyboard.SetTargetProperty(exitaction, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
#endif
#endif
                        else
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
                        Storyboard.SetTargetProperty(exitaction, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
#else
#if WPF
                        Storyboard.SetTargetProperty(exitaction, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
#else
                            Storyboard.SetTargetProperty(exitaction, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
#endif
#endif


                        exit.Begin();
                        enter.Begin();
                    }
                    else if (Transition is RotateTransition)
                    {
#if !WPF
                        PART_LayoutRoot.Projection = new PlaneProjection();
#endif
                        TimeSpan duration = TimeSpan.FromSeconds(1);

                        DoubleAnimationUsingKeyFrames timeline1;
                        if (PART_Content.Visibility == Visibility.Visible)
                        {
                            timeline1 = (this.Transition as RotateTransition).BuildTimeLine(0, 180);
                        }
                        else
                        {
                            timeline1 = (this.Transition as RotateTransition).BuildTimeLine(180, 360);
                        }
                        ObjectAnimationUsingKeyFrames timeline2 = (this.Transition as RotateTransition).BuildObjectTimeline(Visibility.Collapsed);
                        ObjectAnimationUsingKeyFrames timeline3 = (this.Transition as RotateTransition).BuildObjectTimeline(Visibility.Visible);

                        Storyboard story = new Storyboard();
                        story.Children.Add(timeline1);
                        story.Children.Add(timeline2);
                        story.Children.Add(timeline3);

                        Storyboard.SetTarget(timeline1, PART_LayoutRoot);
                        if (PART_Content.Visibility == Visibility.Visible)
                        {
                            PART_Content.Content = newContent;
                            PART_TempContent.Visibility = Visibility.Visible;
                            PART_TempContent.Content = oldContent;

                            PART_Content.Visibility = Visibility.Collapsed;
                            PART_TempContent.Visibility = Visibility.Visible;

                            PART_Content.RenderTransformOrigin = new Point(0.5, 0.5);
#if WPF
                            PART_Content.RenderTransform = new ScaleTransform() { ScaleY = -1 };
#else
                            PART_Content.RenderTransform = new CompositeTransform() { ScaleY = -1 };
#endif
                            Storyboard.SetTarget(timeline2, PART_TempContent);
                            Storyboard.SetTarget(timeline3, PART_Content);
                        }
                        else 
                        {
                            PART_Content.RenderTransformOrigin = new Point(0.5, 0.5);
                            Storyboard.SetTarget(timeline2, PART_Content);
                            Storyboard.SetTarget(timeline3, PART_TempContent);
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
                    else if (Transition is FadeTransition)
                    {
                        PART_TempContent.Visibility = Visibility.Visible;
                        PART_TempContent.Content = oldContent;
                        PART_TempContent.Opacity = 1;
                        PART_Content.Opacity = 0;

                        FadeTransition fade = Transition as FadeTransition;
                        Timeline old_timeline = fade.BuildAnimation(1, 0, fade.Duration, fade.Easing);
                        Timeline new_timeline = fade.BuildAnimation(0, 1, fade.Duration, fade.Easing);

                        Storyboard.SetTarget(old_timeline, PART_TempContent);
                        Storyboard.SetTarget(new_timeline, PART_Content);

#if !WINRT
                        Storyboard.SetTargetProperty(old_timeline, new PropertyPath("(UIElement.Opacity)"));
                        Storyboard.SetTargetProperty(new_timeline, new PropertyPath("(UIElement.Opacity)"));
#else
                        Storyboard.SetTargetProperty(old_timeline, "(UIElement.Opacity)");
                        Storyboard.SetTargetProperty(new_timeline, "(UIElement.Opacity)");
#endif

                        Storyboard story = new Storyboard();
                        story.Children.Add(old_timeline);
                        story.Children.Add(new_timeline);

                        story.Begin();
                    }
                }
            }
            base.OnContentChanged(oldContent, newContent);
        }
        #endregion       

    }

    /// <summary>
    /// Represents a convertor that inverts the transition
    /// </summary>
    [ClassReference(IsReviewed = false,ShouldInclude=false)]
    public class TransitionInverter : IValueConverter
    {
        /// <summary>
        /// Inverts the transition
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            return (double)value - 100.0;
        }

        /// <summary>
        /// Converts back into default type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
