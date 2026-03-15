// <copyright file="RotateTransition.cs" company="Syncfusion">
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
using System.ComponentModel;
namespace Syncfusion.Windows.Controls
#else
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    ///  his Transition creates a rotation animation that spans its <see
    /// cref="P:Syncfusion.UI.Xaml.Controls.RotateTransition.Duration"/>. This is done
    /// by updating the rotate variable of the node at regular interval. The angle value
    /// is specified in degrees.
    /// </summary>
    [ClassReference(IsReviewed = false)]
#if WPF
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public class RotateTransition : ContentTransition
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
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(RotateTransition), new PropertyMetadata(TimeSpan.FromSeconds(0.6)));


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
            DependencyProperty.Register("Easing", typeof(EasingFunctionBase), typeof(RotateTransition), new PropertyMetadata(new ElasticEase() { EasingMode = EasingMode.EaseInOut, Oscillations = 0, Springiness = 1 }));


        #endregion

        #region Helper Methods

        internal DoubleAnimationUsingKeyFrames BuildTimeLine(double from, double to)
        {
            EasingDoubleKeyFrame keyframe1 = new EasingDoubleKeyFrame();
            keyframe1.Value = from;
            keyframe1.KeyTime = TimeSpan.FromSeconds(0);

            EasingDoubleKeyFrame keyframe2 = new EasingDoubleKeyFrame();
            keyframe2.Value = to;
            keyframe2.KeyTime = Duration;
            keyframe2.EasingFunction = Easing;

            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(keyframe1);
            timeline.KeyFrames.Add(keyframe2);
            return timeline;
        }

        internal ObjectAnimationUsingKeyFrames BuildObjectTimeline(object visibility)
        {
            DiscreteObjectKeyFrame keyframe = new DiscreteObjectKeyFrame();
            keyframe.KeyTime = TimeSpan.FromSeconds(Duration.TotalSeconds / 2.0);
            keyframe.Value = visibility;

            ObjectAnimationUsingKeyFrames timeline = new ObjectAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(keyframe);
            return timeline;
        }

        #endregion
    }
}
