// <copyright file="SlideTransition.cs" company="Syncfusion">
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
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
using System.Windows;
using System.Windows.Media.Animation;

namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media.Animation;
using Syncfusion.Tools.Primitives;
namespace Syncfusion.Tools.Controls
#else
#if WPF
using Syncfusion.Windows.Primitives;
using System.Windows;
using System.Windows.Media.Animation;
namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.UI.Xaml.Primitives;
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    ///  Animating the content one after another in a regular <see
    /// cref="P:Syncfusion.UI.Xaml.Controls.SlideTransition.Duration"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class SlideTransition : ContentTransition
    {
        #region Variables

        /// <summary>
        /// Defines a varible for <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Position"/> 
        /// </summary>
        public Position Position = Position.Bottom;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets direction of the slide.
        /// </summary>
        public SlideDirection Direction
        {
            get { return (SlideDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(SlideDirection), typeof(SlideTransition), new PropertyMetadata(SlideDirection.Default));


        /// <summary>
        /// Gets or sets the length of time for which this timeline plays, not counting
        /// repetitions.
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(SlideTransition), new PropertyMetadata(TimeSpan.FromSeconds(3)));


        /// <summary>
        /// Gets or sets <see
        /// cref="N:Windows.UI.Xaml.Media.EasingFunctionBase">EasingFunctionBase</see> that allow to
        /// apply custom mathematical formulas to animations
        /// </summary>
        public EasingFunctionBase Easing
        {
            get { return (EasingFunctionBase)GetValue(EasingProperty); }
            set { SetValue(EasingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Easing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EasingProperty =
            DependencyProperty.Register("Easing", typeof(EasingFunctionBase), typeof(SlideTransition), new PropertyMetadata(new PowerEase() { Power = 13 }, new PropertyChangedCallback(OnDirectionChanged)));

        #endregion

        #region Public Methods

        /// <summary>
        /// Create animation for the control while exit
        /// </summary>
        /// <param name="toValue"></param>
        /// <returns></returns>
        public Timeline CreateExitAnimation(double toValue)
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame() { Value = 0, KeyTime = TimeSpan.FromSeconds(0) };
            if (Direction == SlideDirection.Up || Direction == SlideDirection.Default||Direction==SlideDirection.Left)
            {
                toValue = -toValue;
            }
            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame() { KeyTime = Duration, Value = toValue };
            frame2.EasingFunction = Easing;
            animation.KeyFrames.Add(frame1);
            animation.KeyFrames.Add(frame2);
            return animation;
        }

        /// <summary>
        /// Create animation for the control while enter
        /// </summary>
        /// <param name="fromValue"></param>
        /// <returns></returns>
        public Timeline CreateEnterAnimation(double fromValue)
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            if (Direction == SlideDirection.Down||Direction==SlideDirection.Right)
            {
                fromValue = -fromValue;
            }
            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame() { Value = fromValue, KeyTime = TimeSpan.FromSeconds(0) };
            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame() { KeyTime = Duration, Value = 0 };
            frame2.EasingFunction = Easing;
            animation.KeyFrames.Add(frame1);
            animation.KeyFrames.Add(frame2);
            return animation;
        }

        #endregion

        #region Callback Methods

        private static void OnDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SlideTransition transition = (SlideTransition)sender;
            if (transition.Direction == SlideDirection.Down)
            {
                transition.Position = Position.Top;
            }
            else if (transition.Direction == SlideDirection.Up)
            {
                transition.Position = Position.Bottom;
            }
            else if (transition.Direction == SlideDirection.Left)
            {
                transition.Position = Position.Left;
            }
            else if (transition.Direction == SlideDirection.Right)
            {
                transition.Position = Position.Right;
            }
            else
            {
                transition.Position = Position.Bottom;
            }
        }

        #endregion

        
    }
}
