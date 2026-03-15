// <copyright file="FadeTransition.cs" company="Syncfusion">
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

#if WINDOWS_PHONE ||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Media.Animation;

namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media.Animation;
namespace Syncfusion.Tools.Controls
#else
#if WPF
using System.Windows;
using System.Windows.Media.Animation;
namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    ///  This Transition creates a fade effect animation that spans its duration. This
    /// is done by updating the opacity variable of the node at regular interval.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class FadeTransition : ContentTransition
    {
        #region Dependency Properties

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
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(FadeTransition), new PropertyMetadata(TimeSpan.FromSeconds(0.5)));



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
        /// Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EasingProperty =
            DependencyProperty.Register("Easing", typeof(EasingFunctionBase), typeof(FadeTransition), new PropertyMetadata(new BackEase() { Amplitude = -1 }));

        #endregion

        #region Helper Methods

        internal Timeline BuildAnimation(double from, double to, TimeSpan duration, EasingFunctionBase easingfunction)
        {
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame();
            frame1.Value = from;
            frame1.KeyTime = TimeSpan.FromSeconds(0);
            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame();
            frame2.Value = to;
            frame2.KeyTime = duration;
            frame2.EasingFunction = easingfunction;
            timeline.KeyFrames.Add(frame1);
            timeline.KeyFrames.Add(frame2);
            return timeline;
        }

        #endregion
    }
}
